using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

// Note: The class is derived from ScriptableObject to allow it to serialize its state while entering/exiting playmode during build
//
// TODO: Implement hash caching and incremental bundle builds 
public class Builder : ScriptableObject, IDisposable
{	
	// To notify users that the class will require further processing
	public class ContinueRequiredException : Exception { }
	
	[Serializable]
	public class Params
	{
		public bool IsStageBuild = true;
		public BuildTarget Target = BuildTarget.WebPlayer;
		public BuildOptions Options = BuildOptions.None;
		public string Location = "";
		
		public string AssetBundleConfigPath = "Assets/Editor/Build/AssetBundlesConfig.asset";
		public string BundleLevelDataBasePath = "Assets/Editor/Build/BundleLevelDataBase.asset";
		
		[Flags]
		public enum ExtraBuildOptions
		{
			None = 0,
			BundlesOnly = 1 << 0,
			BundleBasedPlayer = 1 << 1,
			IncludeBundles = 1 << 2,
			IncludeBundlesCache = 1 << 3,
			QuickBundlePlayer = 1 << 4,
		}
		
		public ExtraBuildOptions ExtraOptions = ExtraBuildOptions.None;
		
		public string BuildNumber = "";
		
		public AndroidBuildSubtarget AndroidSubtarget;
		
		public string BundlesLocation {
			get { 
				if ((ExtraOptions & ExtraBuildOptions.BundlesOnly) != 0)
					return Location;
				
				return Location + ".Bundles"; 
			}
		}
				
		public Params()
		{
			Target = BuildUtils.SelectedBuildTarget;
			
			BuildOptions buildOptions = BuildOptions.None;
			
			if (!UnityEditorInternal.InternalEditorUtility.inBatchMode)
			{
				if (EditorUserBuildSettings.allowDebugging)
					buildOptions |= BuildOptions.AllowDebugging;
				
				if (EditorUserBuildSettings.development)
					buildOptions |= BuildOptions.Development;
				
				if (EditorUserBuildSettings.appendProject)
					buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
				
				if (EditorUserBuildSettings.symlinkLibraries)
					buildOptions |= BuildOptions.SymlinkLibraries;
				
				if (EditorUserBuildSettings.connectProfiler)
					buildOptions |= BuildOptions.ConnectWithProfiler;
				
				if (EditorUserBuildSettings.installInBuildFolder)
					buildOptions |= BuildOptions.InstallInBuildFolder;
				
				if (EditorUserBuildSettings.webPlayerOfflineDeployment)
					buildOptions |= BuildOptions.WebPlayerOfflineDeployment;
				
				AndroidSubtarget = EditorUserBuildSettings.androidBuildSubtarget;
			}
			
			Options = buildOptions;

			string buildLocation = EditorUserBuildSettings.GetBuildLocation(Target);
			if (!string.IsNullOrEmpty(buildLocation)) 
			{
				Location = buildLocation;
			}
		}
		
		public override string ToString()
		{
			return 	"        Target: " + Target + Environment.NewLine +
			  	   	"      Location: " + Location + Environment.NewLine +
				   	"       Options: " + Options + Environment.NewLine +
					"  ExtraOptions: " + ExtraOptions + Environment.NewLine +
					"     SubTarget: " + (Target == BuildTarget.Android ? AndroidSubtarget.ToString() : "Generic") + Environment.NewLine +
					"  Build Number: " + BuildNumber + Environment.NewLine;
		}
	}
	
	public static Builder CreateInstance(Params buildParams)
	{
		var instance = CreateInstance<Builder>();
		instance.Init(buildParams);
		return instance;
	}
		
	public void Build()
	{
		Console.WriteLine("Building...");
		BuildInternal();
	}
	
	public void Dispose()
	{
		if (!_continueRequired)
			FinalizeAll();
	}
	
	public string[] _scenes;
	public string[] _buildAssets;
	public byte[][] _buildAssetsHashes;
	
    public Params _params;
	public LateReferenceProcessor _lateProcessor;
	
	[Serializable]
	public class AssetBundleBuildInfo
	{
		public string Name;
		public List<string> AssetsPaths = new List<string>();
		public bool IsScene = false;
		public AssetBundlesDatabase.BundleData Data;
		public AssetBundlesConfig.Rule Rule;
		
		public List<UnityEngine.Object> Dependencies;

		public int Order 
		{
			get { return Rule.BundleOrder; }
		}
		
		public bool Isolate
		{
			get { return Rule.Isolate; }
		}
		
		public AssetBundleBuildInfo(string name, AssetBundlesConfig.Rule rule, bool isScene)
		{
			Name = name;
			IsScene = isScene;
			Rule = rule;
		}
	}
	
	public AssetBundlesConfig _bundlesConfig;
	public BundleLevelDataBase _bundleLevelDataBase;
	public AssetBundlesDatabaseBuilder _bundlesDatabaseBuilder;
	public List<AssetBundleBuildInfo> _bundles;
	
	public StrippingLevel _oldStrippingLevel;
	public int _dependencyStackLevel;
	public string _oldScene;
	public bool _oldRunInBackground;
	public bool _oldRunInBackgroundSettings;
	public AndroidBuildSubtarget _oldAndroidBuildSubtarget;
	
	public List<UnityEngine.Object> _objectsToLateReference;
	
	public string _bundlesPath;
	public string _bundlesCachePath;
	
	private bool _continueRequired;
	
	private string StreamingBundlesDirectory {
		get {
			return Path.Combine(BuildUtils.StreamingAssetsFolder, "Bundles");
		}
	}
	
	private string StreamingBundlesCacheDirectory {
		get {
			return Path.Combine(BuildUtils.StreamingAssetsFolder, "BundlesCache");
		}
	}
	
	private static readonly string BuildInfoPath = "Assets/Resources/BuildInfo.asset";
	private static readonly string AssetBundlesDatabasePath = "Assets/Resources/AssetBundlesDatabase.asset";
	
	private static readonly string TempAssetsPath = "Assets/Temp";
	private static readonly string BuilderTempPath = TempAssetsPath + "/Builder.asset";
 	private static readonly string AssetBundlesDatabaseTempPath =  "Assets/AssetBundlesDatabase.asset";
	
	private void Init(Params buildParams)
	{
		InitBuildParams(buildParams);
		
		PlayerPrefs.SetInt("IgnoreExecuteInEditMode", 1);
		
		if (!Directory.Exists(TempAssetsPath))
			AssetDatabase.CreateFolder(Path.GetDirectoryName(TempAssetsPath), Path.GetFileName(TempAssetsPath)); 
		
		AssetDatabase.SaveAssets();

		// To save the build state to disk, since entering playmode will destroy this class
		AssetDatabase.CreateAsset(this, BuilderTempPath);
		
		_scenes = BuildUtils.CollectBuildScenes();
		_buildAssets = BuildUtils.CollectBuildAssets();
		
		_lateProcessor = new LateReferenceProcessor(_buildAssets);
		
		_oldStrippingLevel = PlayerSettings.strippingLevel;
		_oldScene = EditorApplication.currentScene;
		_oldRunInBackground = Application.runInBackground;
		_oldRunInBackgroundSettings = PlayerSettings.runInBackground;
		_oldAndroidBuildSubtarget = EditorUserBuildSettings.androidBuildSubtarget;
	}

	private void InitBuildParams(Params buildParams)
	{
		_params = buildParams;
				
		if (String.IsNullOrEmpty(_params.Location))
			throw new ArgumentException("Missing build location");
		
		if (_params.Target == BuildTarget.WebPlayer || _params.Target == BuildTarget.WebPlayerStreamed)
		{
			if ((_params.ExtraOptions & Params.ExtraBuildOptions.BundleBasedPlayer) != 0 &&
			    (_params.ExtraOptions & Params.ExtraBuildOptions.IncludeBundlesCache) != 0) 
			{
				throw new UnityException("Include Bundles Cache isn't supported on WebPlayers");
			}
		}
		
		EditorPrefs.SetString("LastBundlesLocation", _params.BundlesLocation);
		
		Console.WriteLine("Build Params: " + Environment.NewLine + _params.ToString());	
		
		if (_params.Target == BuildTarget.Android)
			EditorUserBuildSettings.androidBuildSubtarget = _params.AndroidSubtarget;
	}
	
	public static string LastBundlesLocation
	{
		get
		{
			return EditorPrefs.GetString("LastBundlesLocation");
		}
	}
					
	private void BuildInternal()
	{
		if ((_params.ExtraOptions & Params.ExtraBuildOptions.BundlesOnly) != 0)
		{
			BuildBundles();
			
			if (!UnityEditorInternal.InternalEditorUtility.inBatchMode)
				EditorUtility.OpenWithDefaultApp(_bundlesPath);
			
			Console.WriteLine("Build completed!");
		}
		else if ((_params.ExtraOptions & Params.ExtraBuildOptions.BundleBasedPlayer) != 0)
		{
			if ((_params.ExtraOptions & Params.ExtraBuildOptions.QuickBundlePlayer) == 0)
			{
				BuildBundles();
				
				if (_objectsToLateReference.Count != 0)
					CreateLateResources(_objectsToLateReference);
				
				if ((_params.ExtraOptions & Params.ExtraBuildOptions.IncludeBundlesCache) != 0)
				{
					CacheBundles(); // ContinueRequiredException() will be throw
					return;
				}
			}
			
			BuildBundleBasedPlayer();
		}
		else
		{
			FileUtil.DeleteFileOrDirectory(StreamingBundlesDirectory);
			FileUtil.DeleteFileOrDirectory(StreamingBundlesCacheDirectory);
			
			// All late references will be setup as late resources
			CreateLateResources(_lateProcessor.ReferencedObjects);
			
			BuildStandardPlayer();
		}
	}
	
	private void PreparePlayerBuild()
	{
		Console.WriteLine("Building player...");
		
		// Assure that parent folder for build location does exists
		Directory.CreateDirectory(Path.GetDirectoryName(_params.Location) ?? "");

		WriteBuildInfo();
	}

    [MenuItem("Tools/Build/WriteBuildInfo")]
    static void WriteBuildVersionInfo()
    {
        var buildInfo = ScriptableObject.CreateInstance<BuildInfo>();
        buildInfo.AppVersion = PlayerSettings.bundleVersion;
       
        buildInfo.BuildDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss (K)");
      
        buildInfo.CompanyName = PlayerSettings.companyName;
        buildInfo.ProductName = PlayerSettings.productName;

        AssetDatabase.CreateAsset(buildInfo, BuildInfoPath);

    }

	private void WriteBuildInfo()
	{
		var buildInfo = ScriptableObject.CreateInstance<BuildInfo>();
		buildInfo.AppVersion = PlayerSettings.bundleVersion;
		buildInfo.BuildNumber = _params.BuildNumber;
		buildInfo.BuildDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss (K)");
		buildInfo.IsBundleBased = (_params.ExtraOptions & Params.ExtraBuildOptions.BundleBasedPlayer) != 0;
		buildInfo.BundlesIncluded = (_params.ExtraOptions & Params.ExtraBuildOptions.IncludeBundles) != 0;
		buildInfo.BundlesCacheIncluded = (_params.ExtraOptions & Params.ExtraBuildOptions.IncludeBundlesCache) != 0;
		buildInfo.CompanyName = PlayerSettings.companyName;
		buildInfo.ProductName = PlayerSettings.productName;
		buildInfo.BuildSubtarget = _params.Target == BuildTarget.Android ? _params.AndroidSubtarget.ToString() : "Generic";
		buildInfo.IsStageBuild = _params.IsStageBuild;
		AssetDatabase.CreateAsset(buildInfo, BuildInfoPath);
	}
	
	private void BuildStandardPlayer()
	{
		PreparePlayerBuild();
		
		string buildResult = BuildPipeline.BuildPlayer(_scenes, _params.Location, _params.Target, _params.Options);
		if (!String.IsNullOrEmpty(buildResult))
			throw new UnityException(buildResult);
		
		FinishPlayerBuild();
	}
	
	private void BuildBundleBasedPlayer()
	{
		if ((_params.ExtraOptions & Params.ExtraBuildOptions.QuickBundlePlayer) == 0)
		{
			AddBundlesToStreamingAssets();
			
			//AssetDatabase.DeleteAsset(AssetBundlesDatabasePath);
			//AssetDatabase.CopyAsset(AssetBundlesDatabaseTempPath, AssetBundlesDatabasePath);
			
			AssetDatabase.Refresh();
			
			PreparePlayerBuild();
		}
		
		if (!File.Exists("Assets/link.xml"))
		{
			Console.WriteLine("Warning: link.xml not found, disabling stripping");
			PlayerSettings.strippingLevel = StrippingLevel.Disabled;
		}
		
		string buildResult = BuildPipeline.BuildPlayer(new string[] { _scenes[0] }, _params.Location, _params.Target, _params.Options);
		
		if (!String.IsNullOrEmpty(buildResult))
			throw new UnityException(buildResult);
		
		if ((_params.ExtraOptions & Params.ExtraBuildOptions.QuickBundlePlayer) == 0)
		{
			FinishPlayerBuild();
		}
	}
	
	private void FinishPlayerBuild()
	{
		BuildUtils.SyncStreamingAssets(_params.Location, _params.Target);
		BuildUtils.CleanDirectoryHiddenAndMetaFiles(_params.Location);
		BuildUtils.DeleteEmptyDirectories(_params.Location);
		Console.WriteLine("Build completed!");
	}
	
	private void CreateLateResources(IList<UnityEngine.Object> objects)
	{
		const string lateResourcesDirectory = "Assets/Resources/Late";
		BuildUtils.PrepareCleanDirectory(lateResourcesDirectory);
		
		foreach(var obj in ScanningUtils.ItemsProcessor(objects, 
		                                                 "Creating late resources",
		                                                 p => AssetDatabase.GetAssetPath(p)))
		{
			var lateResource = ScriptableObject.CreateInstance<LateResource>();
			lateResource.Target = obj;
			AssetDatabase.CreateAsset(lateResource, lateResourcesDirectory + "/" + AssetId.FromObject(obj) + ".asset");
		}
	}
	
	private void AddBundlesToStreamingAssets()
	{
		FileUtil.DeleteFileOrDirectory(StreamingBundlesDirectory);
		FileUtil.DeleteFileOrDirectory(StreamingBundlesCacheDirectory);
		
		if ((_params.ExtraOptions & Params.ExtraBuildOptions.IncludeBundles) != 0)
			BuildUtils.CreateSymlink(_bundlesPath, StreamingBundlesDirectory);

		if ((_params.ExtraOptions & Params.ExtraBuildOptions.IncludeBundlesCache) != 0)
			BuildUtils.CreateSymlink(_bundlesCachePath, StreamingBundlesCacheDirectory);
	}
	
	#region Bundles
	private void BuildBundles()
	{
		SetupBundles();
		ComputeBuildAssetsHashes();
		MapBundles();
		PackBundles();
		
		WriteBundlesDatabase();
		//AssetDatabase.DeleteAsset(AssetBundlesDatabasePath);
		//AssetDatabase.CopyAsset(AssetBundlesDatabaseTempPath, AssetBundlesDatabasePath);
	}
	
	private void SetupBundles()
	{
		_bundlesConfig = AssetDatabase.LoadMainAssetAtPath(_params.AssetBundleConfigPath) as AssetBundlesConfig;
		_bundleLevelDataBase = AssetDatabase.LoadMainAssetAtPath(_params.BundleLevelDataBasePath) as BundleLevelDataBase;
		_bundlesDatabaseBuilder = new AssetBundlesDatabaseBuilder(AssetBundlesDatabaseTempPath);
		
		if ((_params.Options & BuildOptions.AcceptExternalModificationsToPlayer) == 0)
			FileUtil.DeleteFileOrDirectory(_params.BundlesLocation);
	}
	
	// Note: Hashes are calculated for source files since unity seems to 
	// serialize material maps in a way that yields a random order
	private void ComputeBuildAssetsHashes()
	{
		_buildAssetsHashes = new byte[_buildAssets.Length][];
		
		int i = 0;
		foreach(var assetPath in ScanningUtils.ItemsProcessor(_buildAssets, "Computing assets hashes", p => Path.GetFileName(p)))
		{
			_buildAssetsHashes[i] = BuilderCache.Instance.GetHashForAsset(assetPath);
			i++;
		}
	}
	
	private Dictionary<string, byte[]> _assetHashesMap;
	
	private byte[] GetHashForBuildAsset(string assetPath)
	{
		if (_assetHashesMap == null)
		{
			_assetHashesMap = new Dictionary<string, byte[]>();
			for (int i = 0; i < _buildAssets.Length; i++)
			{
				_assetHashesMap.Add(_buildAssets[i], _buildAssetsHashes[i]);
			}
		}
		
		byte[] hash;
		if (_assetHashesMap.TryGetValue(assetPath, out hash))
			return hash;
		
		return new byte[]{};
	}
	
	private byte[] GetHashForBuildAssets(IEnumerable<string> assetPaths)
	{
		byte[] hash = new byte[] {};
		
		foreach (var assetPath in assetPaths)
		{
			var assetHash = GetHashForBuildAsset(assetPath);
			hash = BuildUtils.Sha1.ComputeHash(hash.Concat(assetHash).ToArray());
		}
		
		return hash;
	}
	
	private void MapBundles()
	{
		var bundlesMap = new Dictionary<string, AssetBundleBuildInfo>();
		
		foreach (string assetPath in ScanningUtils.ItemsProcessor(_buildAssets, "Mapping asset to bundles", p => Path.GetFileName(p)))
		{
			string matchAssetPath = assetPath.ToLower().Substring("assets/".Length);
			
			var rule = _bundlesConfig.MatchRules(matchAssetPath);
			if (rule != null && rule.Type == AssetBundlesConfig.RuleType.Include && !string.IsNullOrEmpty(rule.BundleName))
			{
				bool isSceneAsset = Path.GetExtension(assetPath) == ".unity";
				
                string bundleName = rule.GetReplacedBundleName(matchAssetPath);
				
				AssetBundleBuildInfo bundleInfo;
				if (!bundlesMap.TryGetValue(bundleName, out bundleInfo))
				{
					bundleInfo = new AssetBundleBuildInfo(bundleName, rule, isSceneAsset);
					bundlesMap.Add(bundleName, bundleInfo);
				}
				
				if (isSceneAsset)
				{
					if (!bundleInfo.IsScene)
						throw new UnityException("Scenes can only be added to scene bundles");
				}
				else
				{
					var importer = AssetImporter.GetAtPath(assetPath);
					if (importer != null)
					{
						AudioImporter audioImporter = importer as AudioImporter;
						if (audioImporter != null)
						{
							
							//if (audioImporter.loadType == AudioImporterLoadType.StreamFromDisc)
							//	throw new UnityException("Can't add streamed audio clips to asset bundles: " + assetPath);
						}
					}
				}
				
				bundleInfo.AssetsPaths.Add(assetPath);
			}
		}
		
		_bundles = bundlesMap.Values.OrderBy(b => b.Order).ThenBy(b => b.Name).ToList();
	}
		
	private void PackBundles()
	{
		var referencedObjects = new HashSet<UnityEngine.Object>(_lateProcessor.ReferencedObjects);
		var packedObjects = new HashSet<UnityEngine.Object>();
		
		var tempPath = Path.Combine(Path.GetTempPath(), "bundles_" + DateTime.UtcNow.Ticks.ToString());
		BuildUtils.PrepareCleanDirectory(tempPath);
		
		PushAssetDependencies();
		{
			foreach(var bundleInfo in ScanningUtils.ItemsProcessor(_bundles, "Packing asset bundles", p => Path.GetFileName(p.Name), true))
			{
				Console.WriteLine("Building bundle " + bundleInfo.Name + "...");

				string bundlePath = Path.Combine(tempPath, bundleInfo.Name);//BuildUtils.GetPathHashString(bundleInfo.Name));
				
				if (bundleInfo.IsScene)
				{
					var scenePaths = bundleInfo.AssetsPaths.ToArray();

					PushAssetDependencies();
					{
						string buildResult = BuildPipeline.BuildStreamedSceneAssetBundle(scenePaths, 
																		  				 bundlePath, 
																						 _params.Target);
						if (!String.IsNullOrEmpty(buildResult))
							throw new UnityException(buildResult);
						
					}
					PopAssetDependencies();
					
					// Find which assets were packed
					var packedDependencies = new HashSet<UnityEngine.Object>();
					foreach(var scenePath in scenePaths)
					{
						Debug.Log(scenePath);
						EditorApplication.OpenScene(scenePath);
						packedDependencies.UnionWith(ScanningUtils.ScanCurrentSceneAssets());
						packedDependencies.ExceptWith(packedObjects);
					}

					bundleInfo.Dependencies = packedDependencies.ToList();
					
					var hash = GetHashForBuildAssets(scenePaths.Concat(BuildUtils.GetAssetPathsForObjects(packedDependencies)));
					
					
					int bundleLevel = _bundleLevelDataBase.GetBundleLevel(bundleInfo.Name);
					bundleInfo.Data = _bundlesDatabaseBuilder.AddSceneBundle(bundleInfo.Name, hash, bundleLevel, 
					                                                         scenePaths.Select(p => Path.GetFileNameWithoutExtension(p)).ToArray(), 
					                                                         bundlePath);
				}
				else
				{
					// Add only the main asset + any late referenced asset
										
					var mainObjects = bundleInfo.AssetsPaths.Select(p => AssetDatabase.LoadMainAssetAtPath(p));
					var representations = bundleInfo.AssetsPaths.SelectMany(p => AssetDatabase.LoadAllAssetRepresentationsAtPath(p));
					var objects = mainObjects.Concat(representations.Where(o => referencedObjects.Contains(o))).ToArray();
					
					var assetIds = objects.Select(o => AssetId.FromObject(o)).ToArray();
					
					
					foreach(var obj in objects)
					{
						string assetPath = AssetDatabase.GetAssetPath(obj);
						string guid = 	AssetDatabase.AssetPathToGUID(assetPath);
						Console.WriteLine("path: " + assetPath + "   guid:"+ guid);
					}
					
					
					if (bundleInfo.Isolate) {
						PushAssetDependencies();
					}
					{
						if (!BuildPipeline.BuildAssetBundleExplicitAssetNames(objects, assetIds, bundlePath, 
						                               BuildAssetBundleOptions.CompleteAssets | 
						                               BuildAssetBundleOptions.CollectDependencies |
						                               BuildAssetBundleOptions.DeterministicAssetBundle,
						                               _params.Target))
						{
							throw new UnityException("Error building bundle " + bundleInfo.Name);
						}
					}
					if (bundleInfo.Isolate) {
						PopAssetDependencies();
					}
					
					// Find which assets were packed
					var packedDependencies = new HashSet<UnityEngine.Object>(EditorUtility.CollectDependencies(objects.ToArray()));
					packedDependencies.ExceptWith(packedObjects);
					bundleInfo.Dependencies = packedDependencies.ToList();
					if (!bundleInfo.Isolate)
						packedObjects.UnionWith(packedDependencies);
					
					var hash = GetHashForBuildAssets(BuildUtils.GetAssetPathsForObjects(packedDependencies));
					int bundleLevel = _bundleLevelDataBase.GetBundleLevel(bundleInfo.Name);
					bundleInfo.Data = _bundlesDatabaseBuilder.AddBundle(bundleInfo.Name, hash, bundleLevel, bundlePath, assetIds.ToList());
					
					foreach(var obj in objects)
					{
						referencedObjects.Remove(obj);
					}
				}
			}	
		}
		PopAssetDependencies();

		// Move to right destination
		Directory.CreateDirectory(_params.BundlesLocation);
		_bundlesPath = Path.Combine(_params.BundlesLocation, _bundlesDatabaseBuilder.Database.Id);
		FileUtil.DeleteFileOrDirectory(_bundlesPath);
		FileUtil.MoveFileOrDirectory(tempPath, _bundlesPath);
		
		// To create late reference for any late object not added to bundles
		_objectsToLateReference = referencedObjects.ToList();
	}
	
	private void PushAssetDependencies()
	{
		_dependencyStackLevel++;
		BuildPipeline.PushAssetDependencies();
	}
	
	private void PopAssetDependencies()
	{
		_dependencyStackLevel--;
		BuildPipeline.PopAssetDependencies();
	}
	
	public string GetAssetId(string assetPath)
	{	
		if (!string.IsNullOrEmpty(assetPath))
		{
			string guid = AssetDatabase.AssetPathToGUID(assetPath);
			return guid.Substring(0, 8) /*+ (AssetDatabase.IsMainAsset(obj) ? "": "#" + obj.name)*/;
		}
		return "null";
	}
			
	private void WriteBundlesDatabase()
	{
		Console.WriteLine("Writing asset bundles database...");
		

		_bundlesDatabaseBuilder.BuildAssetBundle(Path.Combine(_bundlesPath, "index"), _params.Target);
		
		string infoPath = Path.Combine(_params.BundlesLocation, _bundlesDatabaseBuilder.Database.Id + ".txt");
		
		using (StreamWriter writer = File.CreateText(infoPath))
		{
			writer.WriteLine("[" + PlayerSettings.companyName + " - " + PlayerSettings.productName + "]");
			writer.WriteLine("Id: " + _bundlesDatabaseBuilder.Database.Id);
			writer.WriteLine("Version: " + CachingUtils.GetVersionFromId(_bundlesDatabaseBuilder.Database.Id));
			writer.WriteLine("Hash: " + BuildUtils.ToHexString(_bundlesDatabaseBuilder.Database.Hash));
			writer.WriteLine("Size: " + _bundlesDatabaseBuilder.Database.Size);
			writer.WriteLine("Created: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss (K)"));
			writer.WriteLine("App Version: " + PlayerSettings.bundleVersion);
			writer.WriteLine("Platform: " + _params.Target.ToString());
			writer.WriteLine("Subtarget: " + (_params.Target == BuildTarget.Android ? _params.AndroidSubtarget.ToString() : "Generic"));
			
			writer.WriteLine();
			
			foreach(var bundleInfo in _bundles)
			{
				writer.WriteLine("[" + bundleInfo.Name + "]"); 
				writer.WriteLine("Filename: " + bundleInfo.Data.Filename);
				writer.WriteLine("Size: " + bundleInfo.Data.Size);
				writer.WriteLine("Hash: " + BuildUtils.ToHexString(bundleInfo.Data.Hash));
				writer.WriteLine("Cache Name: " + bundleInfo.Data.CacheName);
				writer.WriteLine("Version: " + CachingUtils.GetVersionFromHash(bundleInfo.Data.Hash));
				
				if (bundleInfo.IsScene)
				{
					writer.WriteLine("Scenes:");
					foreach(var scene in bundleInfo.AssetsPaths)
					{
						writer.WriteLine("\t" + scene + "\t(" + BuildUtils.ToHexString(GetHashForBuildAsset(scene)) + ")");
					}
				}
				
				writer.WriteLine("Bundle Assets:");
				foreach(var assetPath in bundleInfo.AssetsPaths)
				{
					writer.WriteLine("\t" + assetPath + "\t" + GetAssetId(assetPath) +"\t(" + BuildUtils.ToHexString(GetHashForBuildAsset(assetPath)) + ")");
				}
				

				writer.WriteLine("Dependencies Assets:");
				var assetPaths = BuildUtils.GetAssetPathsForObjects(bundleInfo.Dependencies);
				foreach(var assetPath in assetPaths)
				{
					writer.WriteLine("\t" + assetPath + "\t(" + BuildUtils.ToHexString(GetHashForBuildAsset(assetPath)) + ")");
				}
				writer.WriteLine();
			}
		}
	}
	#endregion	
	
	#region Caching
	private void CacheBundles()
	{
		Console.WriteLine("Caching bundles...");
		
		_bundlesCachePath = Path.Combine(_params.BundlesLocation, _bundlesDatabaseBuilder.Database.Id + "_cache");
		
		BuildUtils.PrepareCleanDirectory(_bundlesCachePath);
		
		// Note: All this flow break is required because WWW.LoadFromCacheOrDownload() is allowed only in play mode
		EditorUtility.SetDirty(this);
		
		EditorApplication.NewScene();
		var go = new GameObject("AssetsBundleDatabaseCacher", typeof(AssetBundlesDatabaseCacher));
		var databaseCacher = go.GetComponent<AssetBundlesDatabaseCacher>();
		databaseCacher.Setup(_bundlesDatabaseBuilder.Database, Path.GetFullPath(_bundlesPath));
		
		// Application will only actually enter playmode after the call stack returns to the editor
		Application.runInBackground = true;
		EditorApplication.isPaused = false;
		EditorApplication.isPlaying = true;
		
		EditorPrefs.SetBool("Builder.ContinuePending", true);
		_continueRequired = true; // Prevents FinishAll() on Dispose
		throw new ContinueRequiredException();
	}
		
	public static void CheckCacheDone()
	{
		if (EditorApplication.isPlaying)
			return;
		
		// Load the build state and continue
		var builder = AssetDatabase.LoadMainAssetAtPath(BuilderTempPath) as Builder;
		if (builder != null)
			builder.ContinueBundleCache();
		
		EditorApplication.update = null;
	}

	void OnEnable()
	{
		if (EditorPrefs.GetBool("Builder.ContinuePending"))
		{
			EditorApplication.update = CheckCacheDone;
		}
	}
		
	public void ContinueBundleCache()
	{
		EditorPrefs.SetBool("Builder.ContinuePending", false);
		_continueRequired = false;
		
		try
		{
			string exception = PlayerPrefs.GetString("AssetBundlesDatabaseCacher.Exception");
			if (!string.IsNullOrEmpty(exception))
			{
				throw (System.Exception)BuildUtils.InstantiateByTypeName(exception);
			}
			
			var cacheDirectory = CachingUtils.GetCacheDirectory(PlayerSettings.companyName, PlayerSettings.productName);
			foreach(var cachePath in Directory.GetDirectories(cacheDirectory))
			{
				string path = Path.Combine(_bundlesCachePath, Path.GetFileName(cachePath));
				Directory.Move(cachePath, path);
			}
			
			BuildBundleBasedPlayer();
		}
		finally
		{
			FinalizeAll();
			if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
				EditorApplication.Exit(0);
		}
	}
	#endregion
		
	private void FinalizeAll()
	{
		Console.WriteLine("Cleaning build temporary data...");
		
		PlayerPrefs.DeleteKey("IgnoreExecuteInEditMode");
		PlayerPrefs.DeleteKey("AssetBundlesDatabaseCacher.Exception");
		PlayerPrefs.Save();
		
		while(_dependencyStackLevel > 0)
		{
			BuildPipeline.PopAssetDependencies();
			_dependencyStackLevel--;
		}
		
		if (!UnityEditorInternal.InternalEditorUtility.inBatchMode)
			EditorUtility.ClearProgressBar();
		
		EditorPrefs.DeleteKey("Builder.ContinuePending");
		
		if (PlayerSettings.strippingLevel != _oldStrippingLevel)
			PlayerSettings.strippingLevel = _oldStrippingLevel;
		
		if (Application.runInBackground != _oldRunInBackground)
			Application.runInBackground = _oldRunInBackground;
		
		if (PlayerSettings.runInBackground != _oldRunInBackgroundSettings)
			PlayerSettings.runInBackground = _oldRunInBackgroundSettings;
		
		if (EditorUserBuildSettings.androidBuildSubtarget != _oldAndroidBuildSubtarget)
			EditorUserBuildSettings.androidBuildSubtarget = _oldAndroidBuildSubtarget;
		
		EditorApplication.OpenScene(_oldScene);
		
		//AssetDatabase.DeleteAsset(AssetBundlesDatabaseTempPath);
		AssetDatabase.DeleteAsset(BuilderTempPath);
		
		BuilderCache.Instance.Flush();

		_lateProcessor.Dispose();

		GC.SuppressFinalize(this);
	}

    
}

