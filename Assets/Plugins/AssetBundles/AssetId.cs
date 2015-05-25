//#define DEBUG_GENERATOR

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class AssetId
{
	public delegate Object Resolver(string id, System.Type type);
	
	public static Resolver CurrentResolver = null;
	
	public static Object FromId(string id, System.Type type)
	{
		if (string.IsNullOrEmpty(id))
			return null;
		
		return CurrentResolver(id, type);
	}
	
	public static T FromId<T>(string id) where T : UnityEngine.Object
	{
		return FromId(id, typeof(T)) as T;
	}
	
	public static T Resolve<T>(T target, string id) where T : UnityEngine.Object
	{
		
		if (CurrentResolver != null)
		{
			return target != null? target : FromId<T>(id);
		}
		

		return target;
	}
	
#if UNITY_EDITOR
	public delegate string Generator(Object obj);
	public static Generator CurrentGenerator = DefaultGenerator;

	public static string FromObject(Object obj)
	{
		if (obj == null)
			return null;
		
		return CurrentGenerator(obj);
	}
	
	public static string DefaultGenerator(Object obj)
	{
		#if DEBUG_GENERATOR		
		string assetPath = AssetDatabase.GetAssetPath(obj);
		if (!string.IsNullOrEmpty(assetPath))
		{
			return assetPath.Substring("assets/".Length).ToLower().Replace("/","_") + 
				   (AssetDatabase.IsMainAsset(obj) ? "": "#" + obj.name);
		}
		return null;
		#else
		string assetPath = AssetDatabase.GetAssetPath(obj);
		if (!string.IsNullOrEmpty(assetPath))
		{
			string guid = AssetDatabase.AssetPathToGUID(assetPath);
			return guid.Substring(0, 8) + (AssetDatabase.IsMainAsset(obj) ? "": "#" + obj.name);
		}
		return null;
		#endif
	}

#endif
}

