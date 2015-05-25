using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class BuilderCache : ScriptableObject
{
	[Serializable]
	public class AssetInfo
	{
		public string _assetPath;
		public string _timeStamp;
		public byte[] _hash;

		public string AssetPath {
			get { return _assetPath; }
			set { _assetPath = value; }
		}

		public byte[] Hash {
			get { return _hash; }
			set { _hash = value; }
		}

		public string TimeStamp {
			get { return _timeStamp; }
			set { _timeStamp = value; }
		}
	}
	
	public List<AssetInfo> _assets;
	
	private static readonly string CachePath = "Assets/Temp/BuilderCache.asset";
	
	private static BuilderCache _instance;
	
	public static BuilderCache Instance
	{
		get
		{
			if (_instance == null || !_instance)
			{
				_instance = AssetDatabase.LoadMainAssetAtPath(CachePath) as BuilderCache;
				if (_instance == null || !_instance)
				{
					_instance = CreateInstance<BuilderCache>();
					AssetDatabase.CreateAsset(_instance, CachePath);
				}
			}
			
			return _instance;
		}
	}
	
	private Dictionary<string, AssetInfo> _assetInfoMap;
	
	private Dictionary<string, AssetInfo> AssetInfoMap
	{
		get
		{
			if (_assetInfoMap == null)
			{
				_assetInfoMap = new Dictionary<string, AssetInfo>();
				if (_assets != null)
				{
					foreach(var assetInfo in _assets)
					{
						_assetInfoMap.Add(assetInfo.AssetPath, assetInfo);
					}
				}
			}
			
			return _assetInfoMap;
		}
	}
	
	public byte[] GetHashForAsset(string assetPath)
	{
		var importer = AssetImporter.GetAtPath(assetPath);
		if (importer == null)
			return new byte[] {};
		
		AssetInfo assetInfo;
		
		if (!AssetInfoMap.TryGetValue(assetPath, out assetInfo))
		{
			assetInfo = new AssetInfo();
			assetInfo.AssetPath = assetPath;
			assetInfo.TimeStamp = importer.assetTimeStamp.ToString();
			assetInfo.Hash = BuildUtils.ComputeHashForAsset(assetPath);
			AssetInfoMap.Add(assetPath, assetInfo);
			return assetInfo.Hash;
		}
		
		if (assetInfo.TimeStamp == importer.assetTimeStamp.ToString())
			return assetInfo.Hash;
		
		Console.WriteLine("Changed: " + assetPath);
		assetInfo.TimeStamp = importer.assetTimeStamp.ToString();
		assetInfo.Hash = BuildUtils.ComputeHashForAsset(assetPath);
		return assetInfo.Hash;
	}

	void OnDisable()
	{
		Flush();
	}
	
	public void Flush()
	{
		_assets = new List<AssetInfo>(AssetInfoMap.Values);
		EditorUtility.SetDirty(this);
	}
}
