using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class BuildSettingsWindow : EditorWindow
{	
	[MenuItem("Tools/Build Game Settings... &#b", false, 0)]
	static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(BuildSettingsWindow), true, "Build Game Settings", true);
	}
	
	[MenuItem("Tools/Build And Run Game &b", false, 1)]
	static void BuildAndRun()
	{
		string buildNumber;
		bool makeBundleBasedPlayer;
		bool includeBundles;
		bool includeBundlesCache;
		bool quickPlayer;
		
		ReadParams(out buildNumber, out makeBundleBasedPlayer, out includeBundles, out includeBundlesCache, out quickPlayer);

		Make(true, buildNumber, makeBundleBasedPlayer, includeBundles, includeBundlesCache, quickPlayer);
	}
	
	static void ReadParams(out string buildNumber, out bool makeBundleBasedPlayer, out bool includeBundles, out bool includeBundlesCache, out bool quickPlayer)
	{
		buildNumber = EditorPrefs.GetString("BuildSettingsWindow.BuildNumber", "0");
		makeBundleBasedPlayer = EditorPrefs.GetBool("BuildSettingsWindow.MakeBundleBasedPlayer", false);
		includeBundles = EditorPrefs.GetBool("BuildSettingsWindow.IncludeBundles", false);
		includeBundlesCache = EditorPrefs.GetBool("BuildSettingsWindow.IncludeBundlesCache", false);
		quickPlayer = EditorPrefs.GetBool("BuildSettingsWindow.QuickPlayer", false);
	}
	
	static void SaveParams(string buildNumber, bool makeBundleBasedPlayer, bool includeBundles, bool includeBundlesCache, bool quickPlayer)
	{
		EditorPrefs.SetString("BuildSettingsWindow.BuildNumber", buildNumber);
		EditorPrefs.SetBool("BuildSettingsWindow.MakeBundleBasedPlayer", makeBundleBasedPlayer);
		EditorPrefs.SetBool("BuildSettingsWindow.IncludeBundles", includeBundles);
		EditorPrefs.SetBool("BuildSettingsWindow.IncludeBundlesCache", includeBundlesCache);
		EditorPrefs.SetBool("BuildSettingsWindow.QuickPlayer", quickPlayer);
	}
	
	private static bool Make(bool autoRun, string buildNumber, bool makeBundleBasedPlayer, bool includeBundles, bool includeBundlesCache, bool quickPlayer)
	{
		if (makeBundleBasedPlayer)
		{
			if (!EditorApplication.SaveCurrentSceneIfUserWantsTo())
				return false;
		}
		
		string buildLocation = EditorUserBuildSettings.GetBuildLocation(BuildUtils.SelectedBuildTarget);

		if (string.IsNullOrEmpty(buildLocation)) 
		{
			buildLocation = BuildUtils.OpenSaveBuildPanel(BuildUtils.SelectedBuildTarget, "Select build location for " + BuildUtils.SelectedBuildTarget, null, null, null);
		}
		else
		{
			string directory = Path.GetDirectoryName(buildLocation);
			string file = Path.GetFileNameWithoutExtension(buildLocation);
			string extension = Path.GetExtension(buildLocation);
			if (!string.IsNullOrEmpty(extension))
				extension = extension.Substring(1); // Remove the initial dot
			
			buildLocation = BuildUtils.OpenSaveBuildPanel(BuildUtils.SelectedBuildTarget, 
			                                   			  "Select build location for " + BuildUtils.SelectedBuildTarget, directory, file, extension);
		}
		
		if (!string.IsNullOrEmpty(buildLocation))
		{
			EditorUserBuildSettings.SetBuildLocation(BuildUtils.SelectedBuildTarget, buildLocation);
		}
		else
		{
			return false;
		}
		
		var buildParams = new Builder.Params();
		
		buildParams.BuildNumber = buildNumber;
		
		if (autoRun)
		{
			buildParams.Options |= BuildOptions.AutoRunPlayer;
		}
		else 
		{
			buildParams.Options |= BuildOptions.ShowBuiltPlayer;
		}
		
		if (makeBundleBasedPlayer)
		{
			buildParams.ExtraOptions |= Builder.Params.ExtraBuildOptions.BundleBasedPlayer;

			if (includeBundles)
			{
				buildParams.ExtraOptions |= Builder.Params.ExtraBuildOptions.IncludeBundles;
			}

			if (includeBundlesCache)
			{
				buildParams.ExtraOptions |= Builder.Params.ExtraBuildOptions.IncludeBundlesCache;
			}

			if (quickPlayer)
			{
				buildParams.ExtraOptions |= Builder.Params.ExtraBuildOptions.QuickBundlePlayer;
			}
		}
		
		try
		{
			Build.Make(buildParams);
		}
		catch(System.OperationCanceledException) { }
		catch(Builder.ContinueRequiredException) { }
		
		return true;
	}
	
	#region Window
	private bool _makePending = false;
	
	private bool _autoRun = false;
	private bool _makeBundleBasedPlayer = false;
	private bool _includeBundles = false;
	private bool _includeBundlesCache = false;
	private bool _quickPlayer = false;
    private bool _buildAndroidPlatformVersion;
	private string _buildNumber = "0";
    private string _androidPlatform;
	
	void Update()
	{
		if (_makePending)
		{
			_makePending = false;
			
			SaveParams(_buildNumber, _makeBundleBasedPlayer, _includeBundles, _includeBundlesCache, _quickPlayer);
			Close();

			Make(_autoRun, _buildNumber, _makeBundleBasedPlayer, _includeBundles, _includeBundlesCache, _quickPlayer);
		}
	}
	
	void OnEnable()
	{
		ReadParams(out _buildNumber, out _makeBundleBasedPlayer, out _includeBundles, out _includeBundlesCache, out _quickPlayer);
		minSize = new Vector2(470, 200);
		maxSize = minSize;
		Update();
	}
			
	void OnGUI()
	{
		GUILayout.Label("Build settings are taken from the standard Build Settings window");
		
		GUILayout.Space(15);
		
		_buildNumber = EditorGUILayout.TextField("Build Number", _buildNumber);

		_makeBundleBasedPlayer = GUILayout.Toggle(_makeBundleBasedPlayer, "Make bundle-based player");
		
		GUI.enabled = _makeBundleBasedPlayer;
		_includeBundles = GUILayout.Toggle(_includeBundles, "Include bundles");
		_includeBundlesCache = GUILayout.Toggle(_includeBundlesCache, "Include bundles cache");
		_quickPlayer = GUILayout.Toggle(_quickPlayer, "Quick test player (won't generate bundles)");
		GUI.enabled = true;
		
		GUILayout.Space(15);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Player Settings"))
		{
			EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
		}

		if (GUILayout.Button("Build Settings"))
		{
			BuildUtils.OpenBuildPlayerWindow();
		}

		GUILayout.Space(60);
		
		if (GUILayout.Button("Build"))
		{
			_makePending = true;
			_autoRun = false;
		}
		
		if (GUILayout.Button("Build And Run"))
		{
			_makePending = true;
			_autoRun = true;
		}
		
		GUILayout.EndHorizontal();
	}	
	#endregion
}


