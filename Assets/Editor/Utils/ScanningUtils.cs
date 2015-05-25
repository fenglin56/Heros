using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public static class ScanningUtils
{
	// Return path to all .unity assets
	public static string[] GetAllScenesPaths()
	{
		return GetFilteredAssetPaths(new string[] {".unity"});
	}
	
	// Return all the prefabs and scriptable objects paths
	public static string[] GetAllPrefabsAndAssetsPaths()
	{
		return GetFilteredAssetPaths(new string[] {".prefab", ".asset"});
	}
	
	public static string[] GetFilteredAssetPaths(string[] extensions)
	{
		return AssetDatabase.GetAllAssetPaths().Where(p => p.StartsWith("assets/") && 
		                                              extensions.Contains(Path.GetExtension(p))).ToArray();
	}
	
	public static IEnumerable<UnityEngine.Object> ScanCurrentSceneAssets()
	{
		var sceneAssets = new HashSet<UnityEngine.Object>();
			

        UnityEngine.Object[] rootObj = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));

        foreach(UnityEngine.Object obj in rootObj)
        {
            
            UnityEngine.Object[] checkObj = new UnityEngine.Object[1];
            checkObj[0] = obj;
            var checkDepens = EditorUtility.CollectDependencies(checkObj);
            foreach(UnityEngine.Object dObj in checkDepens)
            {
                /*
                if(dObj != null &&PrefabUtility.GetPrefabType(dObj) == PrefabType.Prefab)
                {
                    UnityEngine.Object[] checkPrefabObj = new UnityEngine.Object[1];
                    checkPrefabObj[0] = dObj;
                    var prefabDepens = EditorUtility.CollectDependencies(checkPrefabObj);
                    foreach(UnityEngine.Object pObj in prefabDepens)
                    {
                        if(null == pObj)
                        {
                            Debug.LogError("Wrong Prefab Name: " + dObj.name);
                        }
                    }

                }
                */


                if(dObj == null && ((GameObject)obj).transform.parent == null)
                {

                    Debug.LogError("Root Obj name: " + obj.name);
                }
            }

        }




		var sceneGoDependencies = EditorUtility.CollectDependencies(ScanCurrentSceneRootGameObjects().ToArray());
		
        int i = 0;

        foreach (UnityEngine.Object obj in sceneGoDependencies)
        {
            if(obj == null)
            {
                Debug.LogError("null ojb:" + i );
            }
            i++;
        }
		
		sceneAssets.UnionWith(sceneGoDependencies.Where(o => null != o && EditorUtility.IsPersistent(o)));
		
		Material skybox = RenderSettings.skybox;
		if (skybox != null)
		{
			sceneAssets.Add(skybox);
		}
		
		foreach(var lightmap in LightmapSettings.lightmaps)
		{
			if (lightmap.lightmapFar != null)
				sceneAssets.Add(lightmap.lightmapFar);
			
			if (lightmap.lightmapNear != null)
				sceneAssets.Add(lightmap.lightmapNear);
		}

		return sceneAssets;
	}
	
	public static IEnumerable<UnityEngine.GameObject> ScanCurrentSceneRootGameObjects()
	{
		// Resources.FindObjectsOfTypeAll is the only find function that returns disabled objects also
		GameObject[] allGameObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
				
		// Only grab the root objects, sort by name and recurse thru them, this is needed 
		// since FindObjectsOfTypeAll returns objects in a non deterministic order
		return allGameObjects.Where(go => !EditorUtility.IsPersistent(go) &&
		                            go.transform.parent == null).OrderBy(x => x.name);
	}
	
	public static IEnumerable<UnityEngine.Object> ScanCurrentScene()
	{
		var sceneObjects = ScanCurrentSceneRootGameObjects();
		
		foreach(var go in sceneObjects)
		{
			foreach(var subGo in ScanGameObject(go))
			{
				yield return subGo;
			}
		}
	}
	
	public static IEnumerable<UnityEngine.Object> ScanObject(UnityEngine.Object source)
	{
		if (source is GameObject)
			return ScanGameObject((GameObject)source);
		
		return new UnityEngine.Object[] { source };
	}
			
	public static IEnumerable<UnityEngine.Object> ScanGameObject(GameObject sourceGo)
	{
		yield return sourceGo;

		foreach(Component component in sourceGo.GetComponents<Component>()) 
		{
			yield return component;
		}
		
		foreach(Transform childTransform in sourceGo.transform)
		{
			foreach(var childGo in ScanGameObject(childTransform.gameObject))
			{
				yield return childGo; 
			}
		}
	}
	
	public static IEnumerable<T> ItemsProcessor<T>(IList<T> items, string title)
	{
		return ItemsProcessor(items, title, null, false);
	}
	
	public static IEnumerable<T> ItemsProcessor<T>(IList<T> items, string title, Func<T, string> itemNameProcessor)
	{
		return ItemsProcessor(items, title, itemNameProcessor, false);
	}

	// Enumerate a item array with a cancellable progress bar, throws OperationCanceledException if cancelled
	public static IEnumerable<T> ItemsProcessor<T>(IList<T> items, string title, Func<T, string> itemNameProcessor, bool willTakeoverProgressBar)
	{
		Console.WriteLine(title + "...");
		
		if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
		{
			foreach(var item in items)
			{
				yield return item;
			}			
			
			yield break;
		}
		
		int total = items.Count;
		int current = 1;
		
		const double minimumRefreshTime = 0.1;
		const double maximumNoProgressDisplayTime = 5.0; 
		const double progressShowTime = 1.0;

		var startTime = EditorApplication.timeSinceStartup;
		var lastUpdateTime = startTime;
		var lastProgressDisplayTime = startTime - maximumNoProgressDisplayTime;
		
		try
		{
			foreach(var item in items)
			{
				var now = EditorApplication.timeSinceStartup;
				
				// To prevent progress bar update for taking longer than the processing itself
				if (current == 1 || now - lastUpdateTime > minimumRefreshTime)
				{
					string itemName = itemNameProcessor != null ? itemNameProcessor(item) : item.ToString();
					
					if (DisplayCancellableProgressBar(title, current, total, itemName))
						throw new OperationCanceledException();

					if (willTakeoverProgressBar)
					{
						// Some commands takes over the progress display, so from time to time delay the progress to allow the user to cancel
						if (now - lastProgressDisplayTime > maximumNoProgressDisplayTime)
						{
							var waitStartTime = EditorApplication.timeSinceStartup;
							while (EditorApplication.timeSinceStartup - waitStartTime < progressShowTime)
							{
								if (DisplayCancellableProgressBar(title, current, total, itemName))
									throw new OperationCanceledException();
							}
							
							lastProgressDisplayTime = EditorApplication.timeSinceStartup;
						}
					}				
					
					lastUpdateTime = now;
				}
				
				yield return item;
				
				current++;
			}
		}
		finally
		{
			EditorUtility.ClearProgressBar();
		}
	}
	
	private static bool DisplayRefreshToggle;
	
	private static bool DisplayCancellableProgressBar(string title, int current, int total, string itemName)
	{
		DisplayRefreshToggle = !DisplayRefreshToggle; // Workaround to force Unity to repaint the dialog
		
		return EditorUtility.DisplayCancelableProgressBar(title + " (" + current + "/" + total + ")", 
		                                                  itemName + (DisplayRefreshToggle ? " " : ""),
		                                                  ((float)current / (float)total));
	}

	// Enumerate all the objects at asset path
	public static IEnumerable<UnityEngine.Object> ScanAssetPath(string assetPath)
	{
		var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath); // Already returns the components
		foreach(var asset in assets)
		{
			yield return asset;
		}
	}
	
	// Output in the format assetPath|objectNamePath|objectTypeName
	public static string GetObjectFullAssetPath(UnityEngine.Object obj)
	{
		if (obj == null)
			return "(null)";
		
		string objNamePath = GetObjectNamePath(obj);
		
		if (EditorUtility.IsPersistent(obj))
		{
			objNamePath = AssetDatabase.GetAssetPath(obj).ToLower() + "|" + objNamePath;
		}
		else
		{
			objNamePath = "|" + objNamePath;
		}
		
		string[] typeInfo = obj.GetType().AssemblyQualifiedName.Split(new char[] {','});
		
		return objNamePath + "|" + typeInfo[0].Trim() + "," + typeInfo[1].Trim();
	}
	
	private static string GetObjectNamePath(UnityEngine.Object obj)
	{
		string objPath = obj.name;

		GameObject go = obj as GameObject;
		if (go == null && obj is Component)
			go = ((Component)obj).gameObject;

		if (go != null)
		{
			while (go.transform.parent != null)
			{
				go = go.transform.parent.gameObject;
				objPath = go.name + "/" + objPath;
			}
		}
		
		return objPath;
	}
	
	private static UnityEngine.Object GetObjectByPath(UnityEngine.Object obj, string objName, string objChildPath, Type objType)
	{
		if (obj.name != objName)
			return null;
		
		GameObject go = obj as GameObject;
		
		if (go != null)
		{
			if (string.IsNullOrEmpty(objChildPath))
			{
				if (objType == typeof(GameObject))
					return go;
				
				return go.GetComponent(objType);
			}
		
			Transform child = go.transform.FindChild(objChildPath);
			if (child != null)
			{
				if (objType == typeof(GameObject))
					return child.gameObject;
				
				return child.gameObject.GetComponent(objType);
			}
		}
		else
		{
			if (obj.GetType() == objType)
				return obj;
		}
		
		throw new UnityException();
	}
	
	public static UnityEngine.Object GetObjectFromFullAssetPath(string path)
	{
		if (string.IsNullOrEmpty(path) || path == "(null)")
			return null;
		
		string[] pathInfo = path.Split(new char[] {'|'});
		
		string assetPath = pathInfo[0];
		string objNamePath = pathInfo[1];
		string objTypeName = pathInfo[2];

		Type objType = Type.GetType(objTypeName);
		if (objType == null)
			throw new UnityException("Invalid object type: " + objTypeName);
		
		string objName = objNamePath;
		string objChildPath = null;
		
		int firstSlash = objNamePath.IndexOf('/');
		if (firstSlash != -1)
		{
			objName = objNamePath.Substring(0, firstSlash);
			objChildPath = objNamePath.Substring(firstSlash + 1);
		}
		
		#region Check scene object
		if (string.IsNullOrEmpty(assetPath))
		{
			var sceneObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject));
			foreach(var go in sceneObjects)
			{
				if (EditorUtility.IsPersistent(go))
					continue;
				
				if (go.transform.parent != null)
					continue;
				
				var retObj = GetObjectByPath(go, objName, objChildPath, objType);
				if (retObj != null)
					return retObj;
			}
			
			throw new UnityException();
		}
		#endregion
		
		var mainObj = AssetDatabase.LoadMainAssetAtPath(assetPath);
		if (mainObj == null)
			throw new FileNotFoundException(assetPath);
		
		var retMainObj = GetObjectByPath(mainObj, objName, objChildPath, objType);
		if (retMainObj != null)
			return retMainObj;
		
		var representationObjs = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
		if (representationObjs == null)
			throw new FileNotFoundException(assetPath);
		
		foreach(var obj in representationObjs)
		{
			var retRepresentationObj = GetObjectByPath(obj, objName, objChildPath, objType);
			if (retRepresentationObj != null)
				return retRepresentationObj;
		}
		
		throw new UnityException();
	}
		
	// More flexible Type.IsAssignableFrom(), support unbounded generic types
	public static bool IsAssignableFrom(Type baseType, Type extendType)
	{
	    while (!baseType.IsAssignableFrom(extendType))
	    {
	        if (extendType == null || extendType.Equals(typeof(object)))
	        {
	            return false;
	        }
	        if (extendType.IsGenericType && !extendType.IsGenericTypeDefinition)
	        {
	            extendType = extendType.GetGenericTypeDefinition();
	        }
	        else
	        {
	            extendType = extendType.BaseType;
	        }
	    }
		
		return true;
	}
	
	public static bool IsSerializable(Type type)
	{
		return !type.IsValueType && 
				type != typeof(string) &&
				type.IsSerializable && 
				!(typeof(UnityEngine.Object).IsAssignableFrom(type)) && 
				(Attribute.GetCustomAttribute(type, typeof(SerializableAttribute)) != null);
	}
	
	public static bool IsSerializableArray(Type type)
	{
		return type.IsArray || IsAssignableFrom(typeof(List<>), type);
	}
	
	public static Type GetSerializableArrayType(Type type)
	{
		if (type.IsArray)
			return type.GetElementType();
		
		if (IsAssignableFrom(typeof(List<>), type))
			return type.GetGenericTypeDefinition();
		
		return null;
	}
	
	public static bool IsObjectReference(Type type)
	{
		return IsAssignableFrom(typeof(UnityEngine.Object), type);
	}
	
	public struct FieldScanInfo
	{
		public FieldScanInfo(object obj, FieldInfo field, string fieldPath, int depth, int arrayIndex)
		{
			Obj = obj;
			Field = field;
			FieldPath = fieldPath;
			Depth = depth;
			ArrayIndex = arrayIndex;
		}
		
		public object Obj;
		public FieldInfo Field;
		public string FieldPath;
		public int ArrayIndex;
		public int Depth;
		
		public string GetDebugInfo()
		{
			return  new String('\u00BB', Depth) +
					Field.FieldType.Name +
					" " + FieldPath +
					"  # " + Obj.GetType().Name;
		}
	}
	
	// Enumerate all the objects and child objects serializable fields
	public static IEnumerable<FieldScanInfo> ScanObjectFields(object obj)
	{
		if (obj == null)
			Debug.DebugBreak();
		
		// MetadataToken reflects the declaration order
		FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderBy(f => f.MetadataToken).ToArray();
	
		foreach(FieldInfo field in fields)
		{
			if (!field.IsPublic && Attribute.GetCustomAttribute(field, typeof(SerializeField)) == null)
				continue;
			
			yield return new FieldScanInfo(obj, field, field.Name, 0, 0);
			
			object fieldValue = field.GetValue(obj);
			
			if (fieldValue == null)
				continue;
			
			if(IsSerializableArray(field.FieldType))
			{
				System.Collections.IList itemList = (System.Collections.IList)fieldValue;
				
				Type arrType = GetSerializableArrayType(field.FieldType);
				if (IsSerializable(arrType))
				{
					for(int i = 0; i < itemList.Count; i++)
					{		
						foreach(var subInfo in ScanObjectFields(itemList[i]))
						{
							yield return new FieldScanInfo(subInfo.Obj, subInfo.Field, field.Name + "[" + i + "]." + subInfo.FieldPath, subInfo.Depth + 1, i);
						}
					}
				}
			}
			else if(IsSerializable(field.FieldType))
			{
				foreach(var subInfo in ScanObjectFields(fieldValue))
				{
					yield return new FieldScanInfo(subInfo.Obj, subInfo.Field, field.Name + "." + subInfo.FieldPath, subInfo.Depth + 1, 0);
				}
			}
		}
	}
}