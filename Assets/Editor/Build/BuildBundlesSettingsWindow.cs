using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class BuildBundlesSettingsWindow : EditorWindow
{	
	[MenuItem("Tools/Build Bundles Settings...", false, 4)]
	static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(BuildBundlesSettingsWindow), true, "Build Bundles Settings", true);
	}
		
	private static bool Make()
	{
		if (!EditorApplication.SaveCurrentSceneIfUserWantsTo())
			return false;
		
		string buildLocation = Builder.LastBundlesLocation;

		if (string.IsNullOrEmpty(buildLocation)) 
		{
			buildLocation = EditorUtility.SaveFolderPanel("Select a folder to save the bundles", "", "Bundles");
		}
		else
		{
			buildLocation = EditorUtility.SaveFolderPanel("Select a folder to save the bundles", buildLocation, "");
		}
		
		if (string.IsNullOrEmpty(buildLocation))
			return false;
		
		var buildParams = new Builder.Params();

		if (Directory.Exists(buildLocation))
		{
			int ret = EditorUtility.DisplayDialogComplex("Folder already exists", "What you want to do?", "Cancel", "Append", "Replace");
			switch (ret)
			{
				case 0:
					return false;
				
				case 1:
					buildParams.Options |= BuildOptions.AcceptExternalModificationsToPlayer;
					break;

				case 2:
					buildParams.Options &= ~BuildOptions.AcceptExternalModificationsToPlayer;
					break;
			}
		}
		
		buildParams.Location = buildLocation;
				
		buildParams.ExtraOptions |= Builder.Params.ExtraBuildOptions.BundlesOnly;
		
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
	
	void Update()
	{
		if (_makePending)
		{
			_makePending = false;
			Close();
			Make();
		}
	}
	
	void OnEnable()
	{
		minSize = new Vector2(400, 90);
		maxSize = minSize;
		Update();
	}
			
	void OnGUI()
	{
		GUILayout.Label("Nothing to configure...yet");
		
		GUILayout.Space(30);

		GUILayout.BeginHorizontal();
		
		GUILayout.Space(100);
		
		if (GUILayout.Button("Build"))
		{
			_makePending = true;
		}
		
		GUILayout.EndHorizontal();
	}	
	
	private void OpenBuildPlayerWindow()
	{
		// HACK: Use reflection to call private unity methods
		var editorAssembly = Assembly.LoadFrom(UnityEditorInternal.InternalEditorUtility.GetEditorAssemblyPath());
		var editorType = editorAssembly.GetType("UnityEditor.BuildPlayerWindow");
		
		var method = editorType.GetMethod("ShowBuildPlayerWindow", BindingFlags.Static | BindingFlags.NonPublic);
		method.Invoke(null, new object[0]);
	}
	#endregion
}



