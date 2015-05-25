//#define DEBUG_USEBUNDLESINEDITOR
//#define DEBUG_RESETDATAONNEWBUILDS
//#define _LANGUAGE_TEST_VERSION_
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AppManager : MonoBehaviour
{
	
	private static AppManager _instance = null;

	public static AppManager Instance
	{
		get
		{
			if (_instance == null || !_instance)
			{
				_instance = FindObjectOfType(typeof(AppManager)) as AppManager;
			}

			return _instance;
		}
	}
	
	void OnDestroy()
	{
		//_instance = null;
	}
	
	public static bool _appManagerExist = false;
	

	public string _firstSceneName;

	public bool _checkVersionWithoutBundle;

	public string _UpHttpUrl;

    public string _currentAppVersion;
	public string _versionUrl;
	public string _updateUrlBase;
	
	
	public float _connectionTimeout = 30.0f;
	public float _connectionRetryDelay = 8.0f;
	
	public bool _allowDownloadCancel = false;
	
	public string _appUpdateUrl;
	public bool _appUpdateRequired = true;
	
	
	public int _defaultBundleLevel = 2;
	private int _currentBundleLevel;
	
	#region Public	
	public enum UserConfirmation
	{
		None,
		Accepted,
		Cancelled
	}
	
	public bool IsWaitingUserConfirmation
	{
		get { return _waitingUserConfirmation; }
	}
	
	public void RequestConfirmation()
	{
		_waitingUserConfirmation = true;
	}
	
	public void Confirm(UserConfirmation confirmation)
	{
		_waitingUserConfirmation = false;
		_userConfirmation = confirmation;
	}
	
	public bool CanCancelDownload
	{
		get { return _allowDownloadCancel && _isCancellableDownload; }
	}
	
	public void CancelDownload()
	{
		if (CanCancelDownload)
			_cancelDownload = true;
	}
	
	public bool RestartNeeded
	{
		get { 
			
			if(!BundlesEnabled)
			{
				return false;
			}
			
			
			if(Application.internetReachability == NetworkReachability.NotReachable)
			{
				return false;
			}
			
			return AppUpdateAvailable() || BundleUpdateAvailable();
		}
	}
	
	public void RestartIfNeeded()
	{
		if (RestartNeeded && Application.loadedLevel != 0)
		{
			RestartApplication();
		}
	}
	
	public void Restart()
	{
		if (Application.loadedLevel != 0)
			RestartApplication();
	}
		
	
	public enum UpdateErrorType
	{
		None,
		NoInternetAvailable,
		ConnectionError,
		ServerError,
		Cancelled,
	}

	public event Action CloseLogoPanel;
	
	public event Action NoInternetConnection;
	public event Action ServerConnectionError;
	
	// string newVersion, bool required
	public event Action<string, bool> AppUpdate;
	
	public event Action MissingFiles;
	
	public event Action CheckingVersion;
	
	// string databaseId
	public event Action<string> DownloadingDatabase;
	
	// AssetBundlesDatabase database, List<AssetBundlesDatabase.BundleData> bundlesToDownload, int totalDownloadSize
	public event Action<AssetBundlesDatabase, List<AssetBundlesDatabase.BundleData>, int> DownloadStart;
	
	// int bundleCounter, int totalDownloadedBytes
	public event Action<int, int> DownloadProgress; 
	
	// AssetBundlesDatabase database
	public event Action<AssetBundlesDatabase> LoadStart;
		
	// int bundleCounter
	public event Action<int> LoadProgress; 
	
	
	
	public event Action DownloadingStart;
	public event Action DownloadingFinish;
	public event Action LoadingStart;
	public event Action LoadingFinish;
	
	
	public event Action DownloadingVersionStart;
	public event Action<string> DownloadingVersionEnd;
	
	public event Action DownloadingBundleStart;
	public event Action <string, string> DownloadingBundleEnd;
	
	
	public string GetBuildNumber()
	{
		if(BundlesEnabled)
		{
			return _buildInfo.BuildNumber;
		}
		return "";
	}
	
	public BuildInfo BuildInfo {
		get { return _buildInfo; }
	}
	
	public string LatestAssetBundlesId
	{
		get { return _latestDatabaseId; }
	}

	public string LoadedAssetBundlesId
	{
		get { return _lastRunDatabaseId; }
	}
	#endregion

	private struct BundleInfo
	{
		public BundleInfo(AssetBundlesDatabase.BundleData data)
		{
			Data = data;
			Bundle = null;
		}
		
		public AssetBundlesDatabase.BundleData Data;
		public AssetBundle Bundle;
	}
	
	private class VersionInfo
	{
		public string AppVersion;
		public string AssetBundlesDatabaseId;
	}
	
	private UpdateErrorType _updateErrorType = UpdateErrorType.None;
	
	private bool _waitingUserConfirmation;
	private UserConfirmation _userConfirmation;
	
	private bool _isCancellableDownload;
	private bool _cancelDownload;
	
	private AssetBundlesDatabase _shippedDatabase;
	
	private bool _changeLevelPending;
	
	private string _latestDatabaseId;
	private string _lastRunDatabaseId;
	private string _latestAppVersion;
	
	private BuildInfo _buildInfo;
	
	private Dictionary<string, BundleInfo> _bundlesMap = new Dictionary<string, BundleInfo>();
	private Dictionary<string, BundleInfo> _scenesMap = new Dictionary<string, BundleInfo>();
	private Dictionary<string, BundleInfo> _assetIdMap = new Dictionary<string, BundleInfo>();
	
	private DateTime _lastUpdateCheck;
	
	public bool BundlesEnabled
	{
		get
		{
			if (_buildInfo == null || !_buildInfo.IsBundleBased)
				return false;//false;
            else if(_buildInfo._isBundleBased)
            {
                return true;
            }
			
			#if !UNITY_EDITOR || (UNITY_EDITOR && DEBUG_USEBUNDLESINEDITOR)
			return true;
			#else
			return false;
			#endif
		}
	}
	
	private void Awake()
	{
		if(_appManagerExist)
		{
			Debug.Log("destroy new AppManager");
			Destroy(gameObject);
			return ;
		}
		_appManagerExist = true;
				
		GameObject.DontDestroyOnLoad(this.gameObject);
		
		_buildInfo = Resources.Load("BuildInfo") as BuildInfo;
		_shippedDatabase = Resources.Load("AssetBundlesDatabase") as AssetBundlesDatabase;
		
#if !UNITY_EDITOR || (UNITY_EDITOR && DEBUG_USEBUNDLESINEDITOR)
				AssetId.CurrentResolver = AssetIdResolver;
#endif
		
		if (_buildInfo != null)
		{	

			Console_WriteLine(">>>> Build Information <<<<");
			Console_WriteLine("   App Version: " + _buildInfo.AppVersion);
			Console_WriteLine("  Build Number: " + _buildInfo.BuildNumber);
			Console_WriteLine("    Build Date: " + _buildInfo.BuildDate);
			Console_WriteLine("  Bundle Based: " + _buildInfo.IsBundleBased.ToString());
			Console_WriteLine("    Shipped Id: " + (_shippedDatabase != null? _shippedDatabase.Id : "(null)"));
			Console_WriteLine("   Development: " + Debug.isDebugBuild.ToString());
			Console_WriteLine(" Unity Version: " + Application.unityVersion);
			Console_WriteLine(" Is Stage Build: " + _buildInfo.IsStageBuild);

			
			
			
			//_isStageBuild = _buildInfo.IsStageBuild;
		}else
		{
			//_isStageBuild = Debug.isDebugBuild;
		}
		
		if(_buildInfo != null)
		{
			//string versionState = "live";//_isStageBuild ? "stage" :"live";
			//string verionNumber = "v"+ _buildInfo.AppVersion;
			
			//_defaultVersionUrl = string.Format(_defaultVersionUrl, versionState, Platform, verionNumber);	
		}
		
	}
	
	private string Platform{
		get{
			string val = "";
			#if UNITY_WEBPLAYER || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
			val = "standalone";
			#elif UNITY_ANDROID
			val = "android";
			#elif UNITY_IPHONE
			val = "ios";
			#endif
			return val;
		}
	}
	

	private IEnumerator Start()
	{	
		yield return new  WaitForSeconds(3);
		if(CloseLogoPanel != null)
		{
			CloseLogoPanel();
		}

		/*
		if (BundlesEnabled)
		{
			//_lastRunDatabaseId = PlayerPrefs.GetString("LastRunAssetBundleDatabaseId", _shippedDatabase.Id);
			_latestDatabaseId = PlayerPrefs.GetString("LatestAssetBundleDatabaseId", _lastRunDatabaseId);
			_latestAppVersion = PlayerPrefs.GetString("LatestAppVersion", _buildInfo.AppVersion);
			
			string str = PlayerPrefs.GetString("LastUpdateCheck", DateTime.UtcNow.Ticks.ToString());
			_lastUpdateCheck = new DateTime(long.Parse(str));
			
		
			Console_WriteLine(">> Update Information <<");
			Console_WriteLine("        Last Run Id: " + _lastRunDatabaseId);
			Console_WriteLine("          Latest Id: " + _latestDatabaseId);
			Console_WriteLine(" Latest App Version: " + _latestAppVersion);
			Console_WriteLine("  Last Update Check: " + _lastUpdateCheck);
			
		}
		*/
		
		//dont player video
		/*	
		if (Started != null)
			Started();
			*/
		
		yield return null;

		if (!BundlesEnabled)
		{
			if(_checkVersionWithoutBundle)
			{
				yield return StartCoroutine(DoCheckVersion(true));
				yield return null;

				if(null == _downloadedVersionInfo)
				{
					yield break;

				}

				if(_currentAppVersion != _latestAppVersion)
				{
					if(null != AppUpdate)
					{
						AppUpdate(_latestAppVersion, true);
					}
					yield break;
				}
				else
				{
					yield return StartCoroutine(DoLoadFirstScene());
					yield break;

				}
			}
			else
			{
				yield return StartCoroutine(DoLoadFirstScene());
				yield break;
			}
		}
		else
		{
		
			yield return StartCoroutine(DoLoad());
		}
		//StartCoroutine(DoBackgroundCheckVersion());
	}
	
	private IEnumerator DoRequiredUpdate()
	{
		_isCancellableDownload = false;
		
		yield return StartCoroutine(DoUserAwareDownload(() => { return DoCheckVersion(true); }));
		
		if (AppUpdateAvailable())
		{
			yield return StartCoroutine(DoRequiredAppUpdate());
		}
		
		yield return StartCoroutine(DoUserAwareDownload(() => { return DoDownloadBundles(_latestDatabaseId); }));
	}
	
	private bool HasConnectionError()
	{
		if (_updateErrorType == UpdateErrorType.ConnectionError)
			return true;

		if (_updateErrorType == UpdateErrorType.NoInternetAvailable)
			return true;

		if (_updateErrorType == UpdateErrorType.ServerError)
			return true;
		
		return false;
	}
	
	private IEnumerator DoUserAwareDownload(Func<IEnumerator> download)
	{
		
		int retryTimes = 0;
		
		do
		{
			yield return StartCoroutine(download());
			
			switch(_updateErrorType)
			{
				case UpdateErrorType.NoInternetAvailable:
				{
					if (NoInternetConnection != null)
						NoInternetConnection();
				
					if (_waitingUserConfirmation)
						yield return StartCoroutine(DoWaitForUserConfirmation());
								
					yield return new WaitForSeconds(_connectionRetryDelay);
					yield break;
				}
				
				case UpdateErrorType.ConnectionError:
				case UpdateErrorType.ServerError:
				{
					if (ServerConnectionError != null)
						ServerConnectionError();
				
					if (_waitingUserConfirmation)
						yield return StartCoroutine(DoWaitForUserConfirmation());

					yield return new WaitForSeconds(_connectionRetryDelay);
					break;
				}
								
				case UpdateErrorType.Cancelled:
				{
					yield break;
				}
			}
			
			retryTimes++;
			
			if(retryTimes >=3)
			{
				yield break;
			}
			
			
		} while (_updateErrorType != UpdateErrorType.None);
	}
	
	private IEnumerator DoRequiredAppUpdate()
	{
		// New app version is available
		while (true) 
		{
			yield return StartCoroutine(DoConfirmAppUpdate(true));
		}
	}
	
	private IEnumerator DoConfirmAppUpdate(bool required)
	{
		_userConfirmation = UserConfirmation.None;
		
		if (AppUpdate != null)
			AppUpdate(_latestAppVersion, required);

		if (_waitingUserConfirmation)
			yield return StartCoroutine(DoWaitForUserConfirmation());
		
		if (required || _userConfirmation == UserConfirmation.Accepted)
		{
		
			Console_WriteLine("Launching update url...");
			
			Application.OpenURL(_appUpdateUrl);
			Application.Quit(); // In theory it has no effect on iOS, but it seems to work
			yield return new WaitForSeconds(1.0f);
		}
	}
	
	private IEnumerator DoWaitForUserConfirmation()
	{
		while (_waitingUserConfirmation)
		{
			yield return null;
		}
	}
	
	private void OnLevelWasLoaded(int level)
	{
		
	}

    public void LaunchAppUpdate()
    {
        string appUpdateUrl = GetAppUpdateUrl();
        Application.OpenURL(appUpdateUrl);
    }

    public string GetAppUpdateUrl()
    {
		string url = _UpHttpUrl;
        return url;
    }
	

    //Step 1
	private IEnumerator DoLoad()
	{
		yield return null;
		

		UnloadBundles();
		yield return Resources.UnloadUnusedAssets();

		
		_isCancellableDownload = true;

        //Step 2, do check version
		yield return StartCoroutine( DoCheckVersion(true));//StartCoroutine(DoUserAwareDownload(() => { return DoCheckVersion(true); }));
		
		yield return null;

        if(_currentAppVersion != _latestAppVersion)
        {
            if(null != AppUpdate)
            {
                AppUpdate(_latestAppVersion, true);
            }
            yield break;
        }


		
		/*
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			if (AppUpdateAvailable())
			{
				if (_appUpdateRequired)
					yield return StartCoroutine(DoRequiredAppUpdate());
				else
					yield return StartCoroutine(DoConfirmAppUpdate(false));
			}
		}
		*/
		
		string databaseIdToLoad = _latestDatabaseId;
		
		if (!IsDatabaseCached(databaseIdToLoad))
		{
			_isCancellableDownload = true;
			
			// Latest database not available, try to download it
            //Step 3, downLoad needed bundles
			yield return StartCoroutine( DoDownloadBundles(databaseIdToLoad));
			
			if (_updateErrorType == UpdateErrorType.Cancelled)
			{
				databaseIdToLoad = _lastRunDatabaseId;
				
				if (!IsDatabaseCached(databaseIdToLoad))
				{
					// Last loaded database files missing 
					if (MissingFiles != null)
						MissingFiles();
					
					if (_waitingUserConfirmation)
						yield return StartCoroutine(DoWaitForUserConfirmation());
					
					yield return StartCoroutine(DoRequiredUpdate());
				}
			}
		}

        ////Step 4, load bundles
		yield return StartCoroutine(DoLoadBundles(databaseIdToLoad));

			
		_lastRunDatabaseId = databaseIdToLoad;
		PlayerPrefs.SetString("LastRunAssetBundleDatabaseId", databaseIdToLoad);
		PlayerPrefs.Save();

		yield return StartCoroutine(DoLoadFirstScene());

	}
	
	private IEnumerator DoDownloadBundles(string databaseId)
	{
		ResetUpdateError();


		Console_WriteLine("Downloading database " + databaseId + "...");
		
			
		if(DownloadingStart != null)
		{
			DownloadingStart();
		}
				
		if(DownloadingBundleStart != null)
		{
			DownloadingBundleStart();	
		}	
			
			
		if (DownloadingDatabase != null)
			DownloadingDatabase(databaseId);
		
		AssetBundlesDatabase database = _shippedDatabase;
				
		if (databaseId != _shippedDatabase.Id)
		{
			if (!Caching.IsVersionCached("index", CachingUtils.GetVersionFromId(databaseId)))
			{
				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					ThrowUpdateError(UpdateErrorType.NoInternetAvailable);
					yield break;
				}
			}
			
								
			// Download requested database
			int version = CachingUtils.GetVersionFromId(databaseId);
		
				
			
			// This do-while: workaround of unity bug of caching startup delay
			do
			{
				using (WWW www = WWW.LoadFromCacheOrDownload(UpdateUrl + "/" + databaseId + "/index", version))
				{
					yield return StartCoroutine(WwwDone(www));
	
					if (WwwHasBundleError(www))
					{
						
					}
					else
					{
						database = www.assetBundle.mainAsset as AssetBundlesDatabase;
						www.assetBundle.Unload(false);
					}
				}
				
			} while (!Caching.IsVersionCached("index", version));
		}
		else
		{
			database = _shippedDatabase;
		}
		
		int totalDownloadSize = 0;
		
		var bundlesToDownload = new List<AssetBundlesDatabase.BundleData>();
		
		// Check which bundles must be downloaded/cached
		foreach (var bundleData in EnumerateUncachedBundles(database))
		{
			totalDownloadSize += bundleData.Size;
			bundlesToDownload.Add(bundleData);
		}
		
		if (bundlesToDownload.Count == 0)
		{
			yield break;
		}
		
		if (DownloadStart != null)
			DownloadStart(database, bundlesToDownload, totalDownloadSize);
		
		int totalDownloadedBytes = 0;

		if (bundlesToDownload.Count > 0)
		{
		
			Console_WriteLine("Will download " + bundlesToDownload.Count + " bundles, with a total of " + totalDownloadSize + " bytes");
			
			
			string url = UpdateUrl + "/" + databaseId + "/";

			//foreach (var bundleData in bundlesToDownload)
			for(int i = 0; i< bundlesToDownload.Count;)		
			{
				
				var bundleData = bundlesToDownload.ElementAt(i);
					
				Console_WriteLine("Downloading bundle " + bundleData.Name + "...");
				
				using (var www = WWW.LoadFromCacheOrDownload(url + bundleData.Filename, 
				                                             CachingUtils.GetVersionFromHash(bundleData.Hash)))
				{
					int previousDownloadedBytes = totalDownloadedBytes;
					
					yield return StartCoroutine(WwwDone(www, () => {
						totalDownloadedBytes = previousDownloadedBytes + (int)(www.progress * (float)bundleData.Size);
						
						if (DownloadProgress != null)
							DownloadProgress(bundlesToDownload.IndexOf(bundleData), totalDownloadedBytes);
					}));
			
					if (WwwHasBundleError(www))
					{
						
					}
					else
					{
						www.assetBundle.Unload(false);
						totalDownloadedBytes = previousDownloadedBytes + bundleData.Size;
					}
				}
					
				i++;	
			}	
		}
		
		Console_WriteLine("Database downloaded");
		if(DownloadingBundleEnd != null)
		{
			DownloadingBundleEnd("success","url");	
		}		
		
		if(DownloadingFinish != null)
		{
			DownloadingFinish();
		}
	}
	
		


		
	public IEnumerator DoLoadOneBundle(string bundleId)
	{
			AssetBundlesDatabase database = LoadAssetBundlesDatabaseFromId(_lastRunDatabaseId);
	
			AssetBundlesDatabase.BundleData bundleData = null;
			
			foreach (var bData in database.Bundles)
			{
				if(bData.Name== bundleId)
				{
					bundleData =  bData;
					break;	
				}
			}
			
			if(bundleData != null)
			{
				BundleInfo bundleInfo = new BundleInfo(bundleData);
					
					
				int version = CachingUtils.GetVersionFromHash(bundleData.Hash);
		
					string  databaseId	= _latestDatabaseId;
					string url = UpdateUrl + "/" + databaseId + "/";	
					using (var www = WWW.LoadFromCacheOrDownload(url + bundleData.Filename, 
			                                             version))
					{
						yield return www;
						bundleInfo.Bundle = www.assetBundle;
					}
					
				foreach(var sceneName in bundleData.SceneNames)
				{
					_scenesMap.Add(sceneName, bundleInfo);
				}

				foreach(var assetId in bundleData.AssetsIds)
				{
					_assetIdMap[assetId] = bundleInfo;
				}

				_bundlesMap.Add(bundleData.Name, bundleInfo);	
			}

			
	}
		
	public void UnloadOneBundle(string bundleId,bool unloadAll)
	{
		if(_bundlesMap.ContainsKey(bundleId))
		{
			BundleInfo bundleInfo = _bundlesMap[bundleId];
				
          	if(bundleInfo.Bundle != null)
			{
				AssetBundlesDatabase.BundleData bundleData = bundleInfo.Data;
					
				foreach(var sceneName in bundleData.SceneNames)
				{
					if(bundleInfo.Bundle == _scenesMap[sceneName].Bundle)
					{
						_scenesMap.Remove(sceneName);	
					}
				}

				foreach(var assetId in bundleData.AssetsIds)
				{
					_assetIdMap.Remove(assetId);
				}

				_bundlesMap.Remove(bundleId);	
				
				bundleInfo.Bundle.Unload(unloadAll);
				bundleInfo.Bundle = null;	
				Resources.UnloadUnusedAssets();
				GC.Collect();	
			}
		}
	}
		
	
	private IEnumerator DoLoadBundles(string databaseId)
	{
		
		Console_WriteLine("Loading database " + databaseId + "...");
		
		
		if(LoadingStart != null)
		{
			LoadingStart();	
		}
		
		var database = LoadAssetBundlesDatabaseFromId(databaseId);
		
		if (LoadStart != null)
			LoadStart(database);
		
		//Console.WriteLine("==== Current language: " + LanguageTextManager.CurSystemLang);
			
		float lastYieldTime = Time.realtimeSinceStartup;
		const float maximumYieldTime = 1.0f / 5.0f;
		
        int i = 0;
		foreach (var bundleData in database.Bundles)
		{
			BundleInfo bundleInfo = new BundleInfo(bundleData);
				
			Console_WriteLine("Loading bundle " + bundleData.Name + "...");
			
			
			string url = UpdateUrl + "/" + databaseId + "/";
			
			int version = CachingUtils.GetVersionFromHash(bundleData.Hash);
			
			if (!Caching.IsVersionCached(bundleData.Filename, version) &&
			    IsBundleInShippedDatabase(bundleData.Name, version))
			{
				if (_buildInfo.BundlesCacheIncluded)
				{
					// Uncompressed cache copy (faster)
					
					Console_WriteLine("Caching " + bundleData.Name);
					
					CopyToCache(_shippedDatabase[bundleData.Name].CacheName);
				}
				else
				{
					
					Console_WriteLine("Caching decompressing " + bundleData.Name);
					
					url = StreamingAssetsUrl + "/Bundles/";
				}
			}
			
			if (LoadProgress != null)
				LoadProgress(database.Bundles.IndexOf(bundleData));
			
			using (var www = WWW.LoadFromCacheOrDownload(url + bundleData.Filename, 
			                                             version))
			{
				yield return www;
				lastYieldTime = Time.realtimeSinceStartup;
				bundleInfo.Bundle = www.assetBundle;
			}
            yield return null;

			foreach(var sceneName in bundleData.SceneNames)
			{
				_scenesMap.Add(sceneName, bundleInfo);
			}

			foreach(var assetId in bundleData.AssetsIds)
			{
				_assetIdMap[assetId] = bundleInfo;
			}

			_bundlesMap.Add(bundleData.Name, bundleInfo);
			
			// Allow some processing if hanging for too much long
			if (Time.realtimeSinceStartup - lastYieldTime > maximumYieldTime)
			{
				yield return null;
				lastYieldTime = Time.realtimeSinceStartup;
			}

            i++;
            if(i%30 == 0)
            {
                yield return new WaitForSeconds(0.5f);
            }
		}
		
		
			Console_WriteLine("Database loaded");
		
		
		if(LoadingFinish != null)
		{
			LoadingFinish();
		}
	}
	
	private void UnloadBundles()
	{
		foreach(var bundleInfo in _bundlesMap.Values)
		{
			bundleInfo.Bundle.Unload(true);	
		}
			
		_bundlesMap.Clear();
		_assetIdMap.Clear();
		_scenesMap.Clear();
	}
	
	private IEnumerable<AssetBundlesDatabase.BundleData> EnumerateUncachedBundles(AssetBundlesDatabase database)
	{
		foreach (var bundleData in database.Bundles)
		{
			int version = CachingUtils.GetVersionFromHash(bundleData.Hash);
			if (!Caching.IsVersionCached(bundleData.Filename, version) && !IsBundleInShippedDatabase(bundleData.Name, version))
			{
			    yield return bundleData;
			}
		}
	}
	
	private AssetBundlesDatabase LoadAssetBundlesDatabaseFromId(string databaseId)
	{

		if (databaseId == _shippedDatabase.Id)
			return _shippedDatabase;

			
		int version = CachingUtils.GetVersionFromId(databaseId);
		
		if (!Caching.IsVersionCached("index", version))
			return null;
		
		using (WWW www = WWW.LoadFromCacheOrDownload("index", version))
		{
			var database = www.assetBundle.mainAsset as AssetBundlesDatabase;
			www.assetBundle.Unload(false);
			return database;
		}
	}
	                                       
	private bool IsDatabaseCached(string databaseId)
	{
		var database = LoadAssetBundlesDatabaseFromId(databaseId);
		if (database == null)
			return false;
		
		return !EnumerateUncachedBundles(database).Any();
	}
	
	
	private bool IsBundleInShippedDatabase(string name, int version)
	{
		if (!_buildInfo.BundlesIncluded && !_buildInfo.BundlesCacheIncluded)
			return false;
		
		if (_shippedDatabase[name] != null)
		{
			int shippedVersion = CachingUtils.GetVersionFromHash(_shippedDatabase[name].Hash);
			if (version == shippedVersion)
			{
				return true;
			}
		}
			
		
		return false;
	}
		
		
	
	private void RemoveCache(string cacheName)
	{
		var target = CachingUtils.GetCacheDirectory(_buildInfo.CompanyName, _buildInfo.ProductName) + "/" + cacheName;
		CleanDirectory(target);	
		Console_WriteLine("RemoveCache :" + cacheName);	
	}
	
	private void CopyToCache(string cacheName)
	{
		var original = StreamingAssetsDirectory + "/BundlesCache/" + cacheName;
		var target = CachingUtils.GetCacheDirectory(_buildInfo.CompanyName, _buildInfo.ProductName) + "/" + cacheName;
		
		Directory.CreateDirectory(target);
		foreach(var file in Directory.GetFiles(original))
		{
			File.Copy(file, file.Replace(original, target), true);
		}
			
#if UNITY_IPHONE
		iPhone.SetNoBackupFlag(CachingUtils.CacheDirectory);
			
	
		Console_WriteLine("=== not backing up ===" + CachingUtils.CacheDirectory);
		
#endif		
			
	}
	
	private IEnumerator DoLoadFirstScene()
	{
		
		Console_WriteLine("Loading scene " + _firstSceneName + "...");
		
		yield return Application.LoadLevelAsync(_firstSceneName);
	}
	
	private IEnumerator DoCheckVersion(bool notify)
	{
		
		Console_WriteLine("Checking version...");
		
		
		if (notify)
		{
			if (CheckingVersion != null)
				CheckingVersion();
		}
		
		yield return StartCoroutine(DownloadVersionInfo());
		
		if (_downloadedVersionInfo != null)
		{
			_latestAppVersion = _downloadedVersionInfo.AppVersion;
			PlayerPrefs.SetString("LatestAppVersion", _latestAppVersion);
			
			_latestDatabaseId = _downloadedVersionInfo.AssetBundlesDatabaseId;
			PlayerPrefs.SetString("LatestAssetBundleDatabaseId", _latestDatabaseId);
			PlayerPrefs.Save();
			
			if (_downloadedVersionInfo.AppVersion != _buildInfo.AppVersion)
			{
				
				Console_WriteLine("New application version available: " + _downloadedVersionInfo.AppVersion);
				
			}
			else if (_downloadedVersionInfo.AssetBundlesDatabaseId != _lastRunDatabaseId)
			{
				
				Console_WriteLine("New bundles version available: " + _downloadedVersionInfo.AssetBundlesDatabaseId);
				
			}
			else
			{
				
				Console_WriteLine("No new version available");
				
			}
		}
	}
	
	
	private bool _downloadVersionInfoFinish = false;
		
	public bool DownloadVersionInfoFinish
	{
		get{return  _downloadVersionInfoFinish;}			
	}
	
	public void ForceDownloadVersionInfo()
	{
		_downloadVersionInfoFinish = false;	
		StartCoroutine(DoForceDownloadVersionInfo());
	}
		
	public IEnumerator DoForceDownloadVersionInfo()
	{		
		yield return StartCoroutine(DoCheckVersion(false));
		_downloadVersionInfoFinish = true;	
	}
		
		
	private VersionInfo _downloadedVersionInfo;
	 	
	public bool IsVersionDownloadSuccess()
	{
		if(_downloadedVersionInfo == null)
		{
			return false;	
		}else
		{
			return true;		
		}
	}
	
	private IEnumerator DownloadVersionInfo()
	{
		ResetUpdateError();

		_downloadedVersionInfo = null;

		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			ThrowUpdateError(UpdateErrorType.NoInternetAvailable);
			yield break;
		}
		
		string version_url = UpdateUrl + "/version.txt";//  + (Debug.isDebugBuild? "_" + WWW.EscapeURL(_buildInfo.BuildNumber): "");
		
		Console_WriteLine("version_url: "+ version_url);
			
		if(DownloadingVersionStart != null)
		{
			DownloadingVersionStart();	
		}
			
		using (WWW www = new WWW( version_url))
		{
			www.threadPriority = ThreadPriority.BelowNormal;
			yield return StartCoroutine(WwwDone(www));
				
				
			if (_updateErrorType != UpdateErrorType.None)
			{
				if(DownloadingVersionEnd != null)
				{
					DownloadingVersionEnd("fail");
				}
				yield break;		
			}
			if (!string.IsNullOrEmpty(www.error))
			{
				ThrowUpdateError(UpdateErrorType.ConnectionError);
					
				if(DownloadingVersionEnd != null)
				{
					DownloadingVersionEnd("fail");
				}	
				yield break;
			}

				
			if(DownloadingVersionEnd != null)
			{
				DownloadingVersionEnd("success");
			}	
			Console_WriteLine("www.text: ["+ www.text+"]");
			_downloadedVersionInfo = ReadVersionInfo(www.text);
			if (_downloadedVersionInfo == null)
			{
				ThrowUpdateError(UpdateErrorType.ServerError);
				yield break;
			}
		}
	}
	
	[ContextMenu("RestartApplication")]
	private void RestartApplication()
	{
		Console_WriteLine("Restarting application...");
	
		if (AppUpdateAvailable())
		{
			Console_WriteLine("Launching update url... ["+ _appUpdateUrl+"]");
			Application.OpenURL(_appUpdateUrl);
		}
		
		Application.Quit(); // In theory it has no effect on iOS, but it seems to work
	}
	
	private void Update()
	{
		if (_changeLevelPending)
		{
			_changeLevelPending = false;
			Application.LoadLevel(0);
		}
	}
		
	public string UpdateUrl
	{
		get {
			string baseUrl = /*Debug.isDebugBuild _isStageBuild ? _developmentUpdateUrlBase :*/ _updateUrlBase;
						
			#if UNITY_WEBPLAYER || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
			string platform = "standalone";
			#elif UNITY_ANDROID
			string platform = "android";
			#elif UNITY_IPHONE
			string platform = "ios";
			#endif

            string spName = "/original";
            #if ANDROID_TENCENT
            spName = "/tencent";
            #elif ANDROID_OPPO
            spName = "/oppo";
            #endif


			
			//return  + "/" + platform + "/" + _buildInfo.BuildSubtarget.ToLower() + "/v"+ _buildInfo.AppVersion;
			return baseUrl + platform + spName;
		}
	}
		
	private string StreamingAssetsDirectory
	{
		get {
			switch (Application.platform)
			{
				case RuntimePlatform.Android:
					return Application.dataPath + "!/assets";

				case RuntimePlatform.IPhonePlayer:
					return Application.dataPath + "/Raw";
				
				case RuntimePlatform.OSXPlayer:
					return Application.dataPath + "/Data/StreamingAssets";

				case RuntimePlatform.WindowsWebPlayer:
				case RuntimePlatform.OSXWebPlayer:
					return null;

				default:
					return Application.dataPath + "/StreamingAssets";
			}
		}
	}
	
	private string StreamingAssetsUrl
	{
		get {
			switch (Application.platform)
			{
				case RuntimePlatform.Android:
					return "jar:file://" + StreamingAssetsDirectory;

				case RuntimePlatform.WindowsWebPlayer:
				case RuntimePlatform.OSXWebPlayer:
					return Application.dataPath + "/StreamingAssets";
				
				default:
					return "file://" + StreamingAssetsDirectory;
			}
		}
	}
		
	//#if !UNITY_WEBPLAYER && (DEVELOPMENT_BUILD || UNITY_EDITOR)
	private static void CleanDirectory(string path)
	{
		if (!Directory.Exists(path))
			return;
		
		foreach(var file in Directory.GetFiles(path))
		{
			File.Delete(file);
		}
		
		foreach(var directory in Directory.GetDirectories(path))
		{
			Directory.Delete(directory, true);
		}
	}	
	//#endif
		
	private VersionInfo ReadVersionInfo(string data)
	{
	
		VersionInfo info = new VersionInfo();
		
		using (StringReader reader = new StringReader(data))
		{
			if (reader.ReadLine() != "Version")
			{
				Console_WriteLine("read version error");
			}
			
			info.AppVersion = reader.ReadLine();
			
				
			Console_WriteLine("info.AppVersion:" + info.AppVersion);
				
			//if (info.AppVersion == "none" || info.AppVersion == "skip")
				//info.AppVersion = _buildInfo.AppVersion;
				
			info.AssetBundlesDatabaseId = reader.ReadLine();
				
			Console_WriteLine("info.AssetBundlesDatabaseId:" + info.AssetBundlesDatabaseId);	
				
			//if (info.AssetBundlesDatabaseId == "none" || info.AssetBundlesDatabaseId == "skip")
				//info.AssetBundlesDatabaseId = _shippedDatabase.Id;
		}
		
		return info;
	}
	
	private void ResetUpdateError()
	{
		_updateErrorType = UpdateErrorType.None;
		_cancelDownload = false;
	}
	
	private void ThrowUpdateError(UpdateErrorType errorType)
	{
		_updateErrorType = errorType;
		
	
		Console_WriteLine("Update error: " + errorType.ToString());
		
	}
	
	public struct CountdownTimer
	{
		private float _startTime;
		private float _duration;
			
		public CountdownTimer(float seconds)
		{
			_duration = seconds;
			_startTime = Time.realtimeSinceStartup;
		}
		
		public void Reset()
		{
			_startTime = Time.realtimeSinceStartup;
		}
		
		public bool Finished()
		{
			if ((Time.realtimeSinceStartup - _startTime) > _duration)
			{
				return true;
			}
			
			return false;
		}
	}
	
	private IEnumerator WwwDone(WWW www)
	{
		return WwwDone(www, null);
	}
	
	public IEnumerator WwwDone(WWW www, Action onProgress)
	{
		var timer = new CountdownTimer(_connectionTimeout);
		
		float lastProgress = -1.0f;
		
		do
		{
			if (!string.IsNullOrEmpty(www.error))
			{
				ThrowUpdateError(UpdateErrorType.ConnectionError);
				yield break;
			}

			if (!Mathf.Approximately(www.progress, lastProgress))
			{
				if (onProgress != null)
					onProgress();
				
				timer.Reset();
				lastProgress = www.progress;
			}
			
			if (_cancelDownload)
			{
				ThrowUpdateError(UpdateErrorType.Cancelled);
				yield break;
			}
			
			if (timer.Finished())
			{
				ThrowUpdateError(UpdateErrorType.ConnectionError);
				yield break;
			}

			yield return null;

		} while (!www.isDone && www.progress != 1.0f);
		
		yield return www;
	}
	
	private bool WwwHasBundleError(WWW www)
	{
		if (_updateErrorType != UpdateErrorType.None)
			return true;
	
		if (!string.IsNullOrEmpty(www.error))
		{
			ThrowUpdateError(UpdateErrorType.ConnectionError);
			return true;
		}
		
		if (www.assetBundle == null)
		{
			ThrowUpdateError(UpdateErrorType.ServerError);
			return true;
		}
		
		return false;
	}
	
	private bool BundleUpdateAvailable()
	{
		return _lastRunDatabaseId != _latestDatabaseId;
	}
	
	private bool AppUpdateAvailable()
	{
		return 	_latestAppVersion != _buildInfo.AppVersion;
	}
	


		
public event Action<string> LogoutMem;
		
	
public static void LogMem(string message)
{
	if(AppManager.Instance != null && AppManager.Instance.LogoutMem != null)
	{
		AppManager.Instance.LogoutMem(message);
	}
}
	
	private UnityEngine.Object AssetIdResolver(string id, Type type)
	{
		
		BundleInfo bundleInfo;
		if (_assetIdMap.TryGetValue(id, out bundleInfo))
		{
			return bundleInfo.Bundle.Load(id, type);
		}

		// Late reference not present in any bundle, check if present on Resources
		var lateResource = Resources.Load("Late/" + id) as LateResource;
		if (lateResource != null)
		{
			if (lateResource.Target.GetType().IsAssignableFrom(type))
				return lateResource.Target;
		}

		return null;

	}
		
		
	
	public void Console_WriteLine(string message)
	{
		//if(AppManager._isStageBuild)
		{
			Debug.Log(message);
		}		
	}
	
}
	
	
	


