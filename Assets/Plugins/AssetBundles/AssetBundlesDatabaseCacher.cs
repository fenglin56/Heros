#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class AssetBundlesDatabaseCacher : MonoBehaviour 
{
	public AssetBundlesDatabase _database;
	public string _bundlesDirectory;
	
	private IEnumerator<AssetBundlesDatabase.BundleData> _enumerator;
	
	private float _startTime;
	
#if UNITY_EDITOR		
	void Start()
	{
		PlayerPrefs.SetString("AssetBundlesDatabaseCacher.Exception", "OperationCancelledException");
		Caching.CleanCache();
		
		_enumerator = _database.Bundles.GetEnumerator();
		
		_startTime = Time.realtimeSinceStartup;
	}
	
	void Update()
	{
		// Recently switching to playmode cause some bugs with progress dialog UI, so configure a display for initial show
		const float initialProgressDisplayTime = 3.0f;
		
		try
		{
			if (_enumerator.MoveNext())
			{
				var bundleData = _enumerator.Current;
				
				if (!UnityEditorInternal.InternalEditorUtility.inBatchMode)
				{
					int current = _database.Bundles.IndexOf(bundleData) + 1;
					
					if (Time.realtimeSinceStartup - _startTime > initialProgressDisplayTime) 
					{
						bool cancelled = EditorUtility.DisplayCancelableProgressBar("Caching asset bundles", 
						                                                            bundleData.Name, 
						                                                            (float)(current) / (float)_database.Bundles.Count);
						
						if (cancelled)
						{
							throw new OperationCanceledException();
						}
					}
				}
				                                           
				Console.WriteLine("Caching bundle " + bundleData.Name + "...");
				
				using (var www = WWW.LoadFromCacheOrDownload("file://" + _bundlesDirectory + "/" + bundleData.Filename, 
				                                             CachingUtils.GetVersionFromHash(bundleData.Hash)))
				{
					www.assetBundle.Unload(true); // Will block to load
				}
			}
			else
			{
				PlayerPrefs.SetString("AssetBundlesDatabaseCacher.Exception", "");
				Destroy(gameObject);
				EditorApplication.isPlaying = false;
			}
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
			PlayerPrefs.SetString("AssetBundlesDatabaseCacher.Exception", e.GetType().AssemblyQualifiedName);
			Destroy(gameObject);
			EditorApplication.isPlaying = false;
		}
	}
	
	public void Setup(AssetBundlesDatabase database, string bundlesDirectory)
	{
		_database = database;
		_bundlesDirectory = bundlesDirectory;
	}
#endif
}
