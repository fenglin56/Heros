using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

using UnityEngine;
using UnityEditor;

public static class BuildUtils
{
	public static string OpenSaveBuildPanel(BuildTarget target, string title, string directory, string defaultName, string extension)
	{
		// HACK: Use reflection to call private unity methods
		var editorAssembly = Assembly.LoadFrom(UnityEditorInternal.InternalEditorUtility.GetEditorAssemblyPath());
		var editorType = editorAssembly.GetType("UnityEditor.EditorUtility");
	
		var method = editorType.GetMethod("SaveBuildPanel", BindingFlags.Static | BindingFlags.NonPublic);
		
		object[] options = 
		{
			target,
			title,
			directory,
			defaultName,
			extension
		};
		
		return (string)method.Invoke(null, options);
	}
	
	public static void OpenBuildPlayerWindow()
	{
		// HACK: Use reflection to call private unity methods
		var editorAssembly = Assembly.LoadFrom(UnityEditorInternal.InternalEditorUtility.GetEditorAssemblyPath());
		var editorType = editorAssembly.GetType("UnityEditor.BuildPlayerWindow");
		
		var method = editorType.GetMethod("ShowBuildPlayerWindow", BindingFlags.Static | BindingFlags.NonPublic);
		method.Invoke(null, new object[0]);
	}
	
	public static string[] CollectBuildScenes()
	{
		return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
	}
	
	public static string[] CollectBuildAssets()
	{
		string oldScene = EditorApplication.currentScene;
		try
		{
			var buildAssets = new HashSet<string>();
			var scenes = CollectBuildScenes();
			
			foreach (var scene in ScanningUtils.ItemsProcessor(scenes, "Collecting build assets", p => Path.GetFileName(p)))
			{
				EditorApplication.OpenScene(scene);

				buildAssets.Add(scene);
                UnityEngine.Debug.Log("current scene: " + scene);
				buildAssets.UnionWith(ScanningUtils.ScanCurrentSceneAssets().Select(o => AssetDatabase.GetAssetPath(o)));
			}
			
			buildAssets.Remove("");
			return buildAssets.OrderBy(p => p).ToArray();
		}
		finally
		{
			EditorApplication.OpenScene(oldScene);
		}
	}
		
	public static void CopyDirectoryRecursiveIgnoreMeta(string source, string target)
	{
		BuildUtils.PrepareCleanDirectory(target);
		
		// HACK: Another unity non public useful method...
		var editorAssembly = Assembly.LoadFrom(UnityEditorInternal.InternalEditorUtility.GetEditorAssemblyPath());
		var editorType = editorAssembly.GetType("UnityEditor.FileUtil");
		
		var method = editorType.GetMethod("CopyDirectoryRecursiveIgnoreMeta", BindingFlags.Static | BindingFlags.NonPublic);
		
		object[] options = 
		{
			source,
			target
		};
		
		method.Invoke(null, options);
	}	
	
	public static BuildTarget SelectedBuildTarget
	{
		get {
			if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Standalone)
				return EditorUserBuildSettings.selectedStandaloneTarget;
		
			return EditorUserBuildSettings.activeBuildTarget;
		}
	}
	
	public static int RunCommandLine(string command, string args)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo(command, args);
		startInfo.UseShellExecute = false;
 		startInfo.RedirectStandardOutput = true;
 
		using (Process process = Process.Start(startInfo)) 
		{
			string line = null;
			while (null != (line = process.StandardOutput.ReadLine()))
			{
				System.Console.WriteLine(line);
			}
			
			process.WaitForExit();
			return process.ExitCode;
		}
	}
	
	public static readonly string StreamingAssetsFolder = Path.Combine("Assets", "StreamingAssets");
	
	public static void PrepareCleanDirectory(string path)
	{
		if (!Directory.Exists(path))
		{
			if (path.ToLower().StartsWith("assets"))
				AssetDatabase.CreateFolder(Path.GetDirectoryName(path), Path.GetFileName(path));
			else
				Directory.CreateDirectory(path);		
			
			return;
		}
				
		foreach(var file in Directory.GetFiles(path))
		{
			FileUtil.DeleteFileOrDirectory(file);
		}
		
		foreach(var directory in Directory.GetDirectories(path))
		{
			FileUtil.DeleteFileOrDirectory(directory);
		}
	}

	public static void DeleteEmptyDirectories(string path)
	{
		if (!Directory.Exists(path))
			return;
		
		foreach(var directory in Directory.GetDirectories(path))
		{
			DeleteEmptyDirectories(directory);
		}

		if (Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0)
		{
			FileUtil.DeleteFileOrDirectory(path);
		}
	}
	
	// Unity sometimes leave .svn and .meta files in undesirable places, this method clean them
	public static void CleanDirectoryHiddenAndMetaFiles(string path)
	{
		if (!Directory.Exists(path))
			return;
		
		foreach(var file in Directory.GetFiles(path))
		{
			if (Path.GetFileName(file).StartsWith(".") || Path.GetExtension(file) == ".meta")
				FileUtil.DeleteFileOrDirectory(file);
		}
		
		foreach(var directory in Directory.GetDirectories(path))
		{
			if (Path.GetFileName(directory).StartsWith("."))
				FileUtil.DeleteFileOrDirectory(directory);
			else
				CleanDirectoryHiddenAndMetaFiles(directory);
		}
	}
	
	// Create symlinks instead of copying (for speed)
	public static void CreateSymlink(string originalPath, string targetPath)
	{
		if (Application.platform == RuntimePlatform.OSXEditor)
		{
			int ret = RunCommandLine("ln", 
			                         "-s " + Path.GetFullPath(originalPath) + 
			                         " " + targetPath);
			if (ret != 0)
				throw new UnityException("Error symlinking " + originalPath + " to " + targetPath);
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			// TODO: Test on windows 
            int ret = RunCommandLine("cmd.exe", "/c mklink /d "+ "" +
			                         targetPath +
			                         " " + Path.GetFullPath(originalPath));
			if (ret != 0)
				throw new UnityException("Error adding bundle " + originalPath + " to " + targetPath);
		}
	}	

	// Some platforms don't bundle the streaming assets folder, so do it manually
	public static void SyncStreamingAssets(string location, BuildTarget target)
	{
		if (!Directory.Exists(StreamingAssetsFolder))
			return;
		
		switch(target)
		{
			case BuildTarget.StandaloneOSXIntel:
			//case BuildTarget.StandaloneOSXPPC:
			//case BuildTarget.StandaloneOSXUniversal:
			{
				string dataDirectory = Path.Combine(location, Path.Combine("Contents", "Data"));
				string targetLocation = Path.Combine(dataDirectory, "StreamingAssets");

				CopyDirectoryRecursiveIgnoreMeta(StreamingAssetsFolder, targetLocation);
				break;			
			}

			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
			{
				string dataDirectory = Path.Combine(Path.GetDirectoryName(location), 
			                                        Path.GetFileNameWithoutExtension(location) + "_Data");
			
				string targetLocation = Path.Combine(dataDirectory, "StreamingAssets");
			
				CopyDirectoryRecursiveIgnoreMeta(StreamingAssetsFolder, targetLocation);
				break;			
			}

			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
			{
				string targetLocation = Path.Combine(location, "StreamingAssets");
				CopyDirectoryRecursiveIgnoreMeta(StreamingAssetsFolder, targetLocation);
				break;			
			}
		}
	}
	
	private static SHA1 _sha1;
	
	public static SHA1 Sha1
	{
		get {
			if (_sha1 == null)
				_sha1 = SHA1.Create();
			
			return _sha1;
		}
	}
	
	public static string GetPathHashString(string path)
	{
		return ToHexString(GetASCIIStringHash(path).Take(4));
	}

	public static byte[] GetASCIIStringHash(string str)
	{
		return Sha1.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
	}
	
	public static byte[] ComputeHashForAsset(string assetPath)
	{
		byte[] assetHash = ComputeHashForFile(assetPath);
		byte[] metaHash = ComputeHashForFile(AssetDatabase.GetTextMetaDataPathFromAssetPath(assetPath));
		return Sha1.ComputeHash(assetHash.Concat(metaHash).ToArray());
	}
	
	public static byte[] ComputeHashForFile(string file)
	{
		if (string.IsNullOrEmpty(file) || !File.Exists(file))
			return new byte[] {};
		
		using (FileStream stream = File.OpenRead(file))
		{
			return Sha1.ComputeHash(stream);
		}
	}
	
	public static string ToHexString(IEnumerable<byte> data)
	{
		return string.Concat(data.Select(b => b.ToString("x2")).ToArray());
	}
	
	public static object InstantiateByTypeName(string name)
	{
		return System.Activator.CreateInstance(System.Type.GetType(name));
	}
	
	public static string[] GetAssetPathsForObjects(IEnumerable<UnityEngine.Object> objs)
	{
		var paths = new HashSet<string>(objs.Select(o => AssetDatabase.GetAssetPath(o)));
		paths.Remove("");
		return paths.OrderBy(p => p).ToArray();
	}
}
