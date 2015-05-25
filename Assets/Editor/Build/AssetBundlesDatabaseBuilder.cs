using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

[System.Serializable]
public class AssetBundlesDatabaseBuilder
{
	public AssetBundlesDatabase _database;
	private string dbPath = "";
	
	public AssetBundlesDatabaseBuilder(string assetPath)
	{
		_database = ScriptableObject.CreateInstance<AssetBundlesDatabase>();
		_database.Id = "11";
		_database.Hash = new byte[] {};
		_database.Size = 0;
		dbPath = assetPath;
		
		AssetDatabase.CreateAsset(_database, dbPath);
				
	}

	public AssetBundlesDatabase Database {
		get {
			return _database;
		}
	}

	public AssetBundlesDatabase.BundleData AddBundle(string name, byte[] hash, int level, string bundlePath, List<string> assetIds)
	{
		var bundle = new AssetBundlesDatabase.BundleData();
		bundle.Name = name;
		bundle.AssetsIds = assetIds;
		bundle.DataLevel = level;
		AddToDatabase(bundle, hash, bundlePath);
		
		return bundle;
	}
	
	public AssetBundlesDatabase.BundleData AddSceneBundle(string name, byte[] hash,int level, string[] sceneNames, string bundlePath)
	{
		var bundle = new AssetBundlesDatabase.BundleData();
		bundle.Name = name;
		bundle.SceneNames = sceneNames;
		bundle.DataLevel = level;
		AddToDatabase(bundle, hash, bundlePath);
		
		return bundle;
	}

	public void BuildAssetBundle(string path, BuildTarget target)
	{
		
		// Check for collisions
		var idSet = new HashSet<string>();
		AssetBundlesDatabase sedataBase = ScriptableObject.CreateInstance<AssetBundlesDatabase>();
		
		sedataBase.Id=_database.Id;
		sedataBase.Hash = _database.Hash;
		sedataBase.Bundles = _database.Bundles;
		sedataBase.Size = _database.Size;
		foreach(var bundle in _database.Bundles)
		{
			if (bundle.AssetsIds != null)
			{
				if (idSet.Overlaps(bundle.AssetsIds))
					throw new UnityException("AssetId conflict");
				
				idSet.UnionWith(bundle.AssetsIds);
			}
		}
		//EditorUtility.SetDirty(_database);
		AssetDatabase.CreateAsset( sedataBase, "Assets/Resources/AssetBundlesDatabase.asset");
		
		//EditorUtility.SetDirty(_database);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		
		string objPath =  AssetDatabase.GetAssetPath(_database);
		
		if (!BuildPipeline.BuildAssetBundle(sedataBase, 
		                                    null, 
											path, 
											BuildAssetBundleOptions.CompleteAssets |
											BuildAssetBundleOptions.CollectDependencies |
		                               		BuildAssetBundleOptions.DeterministicAssetBundle,
											target))
		{
			throw new UnityException("Error building asset bundles database bundle");
		}
		
	}

	private void AddToDatabase(AssetBundlesDatabase.BundleData bundle, byte[] hash, string bundlePath)
	{
		bundle.Filename = Path.GetFileName(bundlePath);
		
		using (FileStream stream = File.OpenRead(bundlePath))
		{
			bundle.Size = (int)stream.Length;
			bundle.Hash = hash;
			
			var str = bundle.Filename + "@" + CachingUtils.GetVersionFromHash(bundle.Hash);
			bundle.CacheName = BuildUtils.ToHexString(BuildUtils.GetASCIIStringHash(str));
		}

		_database.Bundles.Add(bundle);

		// Update hash, id and size
		_database.Hash = BuildUtils.Sha1.ComputeHash(_database.Hash.Concat(bundle.Hash).ToArray());
		_database.Id = BuildUtils.ToHexString(_database.Hash.Take(4));
		_database.Size += bundle.Size;
		//EditorUtility.SetDirty(_database);
	}
}