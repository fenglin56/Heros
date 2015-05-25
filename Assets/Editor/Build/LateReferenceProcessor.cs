//#define DEBUG_KEEP_IDS
//#define KEEP_BROKEN_LINKS
	
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class LateReferenceProcessor : IDisposable
{
	public LateReferenceProcessor(IList<string> assetPaths)
	{
		var objSet = new HashSet<UnityEngine.Object>();
			
		AssetId.CurrentGenerator = obj => {
			string id = AssetId.DefaultGenerator(obj);
			
			if (!string.IsNullOrEmpty(id))
				objSet.Add(obj);
			
			return id;
		};
		
		foreach (string assetPath in ScanningUtils.ItemsProcessor(assetPaths, 
		                                                          "Scanning assets for late references", 
		                                                          p => Path.GetFileName(p)))
		{
			var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
			ProcessObject(asset);
		}
		

		AssetId.CurrentGenerator = AssetId.DefaultGenerator;
		
		_referencedObjects = objSet.ToList();
	}
	
	public void Dispose()
	{
		var idMap = new Dictionary<string, UnityEngine.Object>();
		foreach(var obj in _referencedObjects)
		{
			idMap.Add(AssetId.FromObject(obj), obj);
		}
		
		AssetId.CurrentResolver = (id, type) => 
		{
			var asset = idMap[id];
			if (type.IsAssignableFrom(asset.GetType()))
				return asset;
			
			throw new UnityException();
		};
		
		try
		{
			foreach(var obj in _processedObjects)
			{
				RetargetObject(obj);
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Unrecoverable error during late reference restore, close Unity without saving anything");
			throw e;
		}
		finally
		{
			AssetId.CurrentResolver = null;
			AssetId.CurrentGenerator = AssetId.DefaultGenerator;
		}	
		
		GC.SuppressFinalize(this);
	}
	
	public List<UnityEngine.Object> _processedObjects = new List<UnityEngine.Object>();
	public List<UnityEngine.Object> _referencedObjects = new List<UnityEngine.Object>();
		
	public List<UnityEngine.Object> ReferencedObjects {
		get {
			return _referencedObjects;
		}
	}
	
	private bool ProcessObject(UnityEngine.Object sourceObj)
	{
		if (!(sourceObj is GameObject) && !(sourceObj is ScriptableObject))
			return false;
		
		bool ret = false;

		foreach(var obj in ScanningUtils.ScanObject(sourceObj))
		{
			if (DetargetObject(obj))
			{
				_processedObjects.Add(obj);
				ret = true;
			}
		}
		
		return ret;
	}

	private bool DetargetObject(UnityEngine.Object obj)
	{
		bool modified = false;
		
		foreach(var info in ScanningUtils.ScanObjectFields(obj))
		{
			//if (DetargetLateAttributeField(info.Obj, info.Field))
			//	modified = true;
			
			if (DetargetLateAttributeField(info.Obj, info.Field))
			{
				modified = true;
								
				#if KEEP_BROKEN_LINKS
				if (modified)
				{
					EditorUtility.SetDirty(obj);
				}
				#endif

				//Console.WriteLine("---" + AssetDatabase.GetAssetPath(obj) + " true");
			}else
			{
				//Console.WriteLine("---" + AssetDatabase.GetAssetPath(obj) + " false");
			}
		}
		
		return modified;
	}

	private bool RetargetObject(UnityEngine.Object obj)
	{
		bool modified = false;
		
		foreach(var info in ScanningUtils.ScanObjectFields(obj))
		{
			if (RetargetLateAttributeField(info.Obj, info.Field))
				modified = true;
		}

		#if DEBUG_KEEP_IDS
		if (modified)
		{
			EditorUtility.SetDirty(obj);
		}
		#endif
		
		return modified;
	}
	
	private bool DetargetLateAttributeField(object obj, FieldInfo field)
	{
		FieldInfo targetField;
		if (IsLateAttributeField(obj, field, out targetField, true))
		{
			if (ScanningUtils.IsSerializableArray(field.FieldType))
			{
				IList referencesArray = (IList)field.GetValue(obj);
				
				var idsArray = new string[referencesArray.Count];
				for (int i = 0; i < referencesArray.Count; i++)
				{
					UnityEngine.Object val = (UnityEngine.Object)referencesArray[i];
					idsArray[i] = AssetId.FromObject(val);
					referencesArray[i] = null;
				}
				
				targetField.SetValue(obj, (object)idsArray);
			}
			else
			{
				UnityEngine.Object val = (UnityEngine.Object)field.GetValue(obj);
				targetField.SetValue(obj, (object)AssetId.FromObject(val));
				field.SetValue(obj, null);
			}
			
			return true;
		}
		
		return false;
	}

	private bool RetargetLateAttributeField(object obj, FieldInfo field)
	{
		FieldInfo targetField;
		if (IsLateAttributeField(obj, field, out targetField, false))
		{
			if (ScanningUtils.IsSerializableArray(field.FieldType))
			{
				IList idsArray = (IList)targetField.GetValue(obj);
				
				UnityEngine.Object[] referencesArray = (UnityEngine.Object[])Activator.CreateInstance(field.FieldType, new object[] { idsArray.Count } );
				for (int i = 0; i < idsArray.Count; i++)
				{
					referencesArray[i] = AssetId.FromId((string)idsArray[i], ScanningUtils.GetSerializableArrayType(field.FieldType));
				}
				
				field.SetValue(obj, (object)referencesArray);
				
				#if !DEBUG_KEEP_IDS
				targetField.SetValue(obj, null);
				#endif
			}
			else
			{
#if !KEEP_BROKEN_LINKS
				string val = (string)targetField.GetValue(obj);
				field.SetValue(obj, (object)AssetId.FromId(val, field.FieldType));
#endif
				
				#if !DEBUG_KEEP_IDS
				targetField.SetValue(obj, null);
				#endif
			}
			
			return true;
		}
		
		return false;
	}
	
	private bool IsLateAttributeField(object obj, FieldInfo field, out FieldInfo targetField, bool performChecks)
	{
		targetField = null;
		
		LateAttribute lazyAttribute = Attribute.GetCustomAttribute(field, typeof(LateAttribute)) as LateAttribute;
		if (lazyAttribute == null)
			return false;
		
		string targetIdField = lazyAttribute.TargetIdField ?? (field.Name + (field.FieldType.IsArray ? "Ids" : "Id"));
		
		targetField = obj.GetType().GetField(targetIdField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (targetField == null)
			throw new UnityException(FormatTargetFieldErrorMessage("does not exist", obj, field, lazyAttribute.TargetIdField));
		
		if (performChecks)
		{
			if (ScanningUtils.IsSerializableArray(field.FieldType))
			{
				if (!typeof(UnityEngine.Object).IsAssignableFrom(ScanningUtils.GetSerializableArrayType(field.FieldType)))
					throw new UnityException(FormatLateFieldErrorMessage("is not an UnityEngine.Object assignable array", obj, field.Name));
				
				if (ScanningUtils.GetSerializableArrayType(targetField.FieldType) != typeof(string))
					throw new UnityException(FormatTargetFieldErrorMessage("is not a string array", obj, field, targetIdField));
			}
			else
			{
				if (!typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
					throw new UnityException(FormatLateFieldErrorMessage("is not an UnityEngine.Object assignable", obj, field.Name));
				
				if (targetField.FieldType != typeof(string))
					throw new UnityException(FormatTargetFieldErrorMessage("is not a string", obj, field, targetIdField));
			}
			
			if (!targetField.IsPublic && Attribute.GetCustomAttribute(targetField, typeof(SerializeField)) == null)
				throw new UnityException(FormatTargetFieldErrorMessage("is not public or have [SerializeField] attribute", obj, field, lazyAttribute.TargetIdField));
			
			if (targetField.IsPublic && Attribute.GetCustomAttribute(targetField, typeof(HideInInspector)) == null)
			{
				Debug.LogWarning(FormatTargetFieldErrorMessage("is public but don't have the [HideInInspector] attribute, " + 
				                                               "this field is not meant to be editable by users", obj, field, lazyAttribute.TargetIdField));
			}
		}
		
		return true;
	}
	
	private string FormatTargetFieldErrorMessage(string message, object obj, FieldInfo field, string targetFieldName)
	{
		return "Target field " + targetFieldName + " " + message +
		                         " on " + field.Name + " at class " + obj.GetType().Name;
	}

	private string FormatLateFieldErrorMessage(string message, object obj, string fieldName)
	{
		return "Late field " + fieldName + " " + message + " at class " + obj.GetType().Name;
	}
}
