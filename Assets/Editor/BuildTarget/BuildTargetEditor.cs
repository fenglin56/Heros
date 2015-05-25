using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class BuildTargetEditor {

    static string[] Scenes = FindEnabledEditorScenes();
    const string PLATFORM_ANDROID_PATH = "Assets/PlatformPluginsCache/";
    const string UI_PATH_NAME = "UC_Plugins";
    const string MI_PATH_NAME = "MI_Plugins";
//    const string OPPO_PATH_NAME = "OPPO_Plugins";
    const string OPPONEW_PATH_NAME="OPPO_Plugins_0917";
	const string TENCENT_TEST_PATH = "Tencent_Test";
	const string TENCENT_RELEASE_PATH = "Tencent_Release";
    const string PLATFORM_PLUGINS_TARGET="Assets/Plugins/Android/";
    [MenuItem("Build Target/Build Android UC")]
    static void PerformAndroidUCBuild()
    {
        BuildTarget("UC", "Android");
    }

    [MenuItem("Build Target/Build Android JiuYao")]
    static void PerformAndroidJiuYaoBuild()
    {
        BuildTarget("JIUYAO", "Android");
    }

    [MenuItem("Build Target/Build Android XIAOMI")]
    static void PerformAndroidMiBuild()
    {
        BuildTarget("XIAOMI", "Android");
    }

//    [MenuItem("Build Target/Build Android OPPO")]
//    static void PerformAndroidOPPOBuild()
//    {
//        BuildTarget("OPPO", "Android");
//    }

    [MenuItem("Build Target/Build Android OPPO 0917")]
    static void PerformAndroidOPPONewBuild()
    {
        BuildTarget("OPPO_0917", "Android");
    }

    [MenuItem("Build Target/      Build Android Tencent( Test)")]
    static void PerformTencentTestBuild()
    {
        BuildTarget("TencentTest", "Android");
    }

	[MenuItem("Build Target/      Build Android Tencent( Release )")]
	static void PerformTencentRealseBuild()
	{
		BuildTarget("TencentRelease", "Android");
	}

    [MenuItem("Build Target/Build Android ALL")]
    static void PerformAndroidAllBuild()
    {
        BuildTarget("UC", "Android");
        BuildTarget("JIUYAO", "Android");
		BuildTarget("XIAOMI", "Android");
//        BuildTarget("OPPO", "Android");
        BuildTarget("OPPO_0917", "Android");
		BuildTarget("TencentRelease", "Android");
    }
//    [MenuItem("Build Target/PreBuidTarget")]
//    static void PerformCopyToBuidTarget()
//    {
//        PreBuildTarget("OPPO", "Android");
//    }
//
//    [MenuItem("Build Target/ClearBuidTarget")]
//    static void ClearBuidTarget()
//    {
//        CleanAndroidBuildTarget();
//    }

    static void BuildTarget(string name, string target)
    {

        string app_name = "JiangHu" + DateTime.Now.ToString("yyyyMMdd_") + name;
        string target_dir = string.Empty;// Application.dataPath + "/TargetAndroid";

        string target_name = string.Empty;// app_name + ".apk";
        BuildTargetGroup targetGroup = BuildTargetGroup.Android;
        BuildTarget buildTarget = UnityEditor.BuildTarget.Android;

        string applicationPath = Application.dataPath.Replace("/Assets", "");

        if (target == "Android")
        {
            target_dir = applicationPath + "/TargetAndroid";
            target_name = app_name + ".apk";
            targetGroup = BuildTargetGroup.Android;
        }
        else if (target == "IOS")
        {
            target_dir = applicationPath + "/TargetIOS";
            target_name = app_name;
            targetGroup = BuildTargetGroup.iPhone;
            buildTarget = UnityEditor.BuildTarget.iPhone;
        }
       
        //FileInfo buildFile = new FileInfo(target_dir);
        //if (buildFile.Exists)
        //    buildFile.Delete();

		if (File.Exists (target_name))
			File.Delete (target_name);

        if (!Directory.Exists(target_dir))
        {
            Directory.CreateDirectory(target_dir);
        }
        string sourcePath="";
        switch (name)
        {
            case "UC":
                PlayerSettings.bundleIdentifier = "com.fanhou.jianghu.uc";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_UC");
                sourcePath=UI_PATH_NAME;
                break;
            case "JIUYAO":
                PlayerSettings.bundleIdentifier = "com.fanhou.jianghu.jiuyao";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_JIUYAO");                
                break;
            case "XIAOMI":
                PlayerSettings.bundleIdentifier = "Pushcraft.mi";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_XIAOMI");
                sourcePath=MI_PATH_NAME;
                break;
//            case "OPPO":
//                PlayerSettings.bundleIdentifier = "com.fanhou.jianghu.nearme";
//                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_OPPO");
//                sourcePath=OPPO_PATH_NAME;
//                break;
            case "OPPO_0917":
                PlayerSettings.bundleIdentifier = "com.fanhou.jianghu.nearme.gamecenter";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_OPPO");
                sourcePath=OPPONEW_PATH_NAME;
                break;
			case "TencentTest":
				PlayerSettings.bundleIdentifier = "com.tencent.tmgp.yjws";
			    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_TENCENT; TENCENT_TEST");
				PlayerSettings.keyaliasPass = "888888";
				PlayerSettings.keystorePass = "888888";
				sourcePath=TENCENT_TEST_PATH;
			break;
			case "TencentRelease":
				PlayerSettings.bundleIdentifier = "com.tencent.tmgp.yjws";
				PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_TENCENT; TENCENT_RELEASE");
				PlayerSettings.keyaliasPass = "888888";
				PlayerSettings.keystorePass = "888888";
				sourcePath=TENCENT_RELEASE_PATH;
			break;
			default:
			break;
		}
		
		//string target_folder = EditorUtility.SaveFolderPanel("111", "", "2222");
        string target_fileName = EditorUtility.SaveFilePanel("BuildTarget", target_dir, target_name, "");

        if (string.IsNullOrEmpty(target_fileName))
            return;

        //拷贝相应平台的Plugins/Android到Plugins/Android
        BuildUtils.CopyDirectoryRecursiveIgnoreMeta(Path.Combine(PLATFORM_ANDROID_PATH, sourcePath), PLATFORM_PLUGINS_TARGET);
        AssetDatabase.Refresh();

        //开始Build场景
        GenericBuild(Scenes, target_fileName, buildTarget, BuildOptions.None);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "");

        BuildUtils.PrepareCleanDirectory(PLATFORM_PLUGINS_TARGET);
    }

    public static void CopyToBuildTarget(string name, string target)
    {
        BuildTargetGroup targetGroup = BuildTargetGroup.Android;
        BuildTarget buildTarget = UnityEditor.BuildTarget.Android;
        string applicationPath = Application.dataPath.Replace("/Assets", "");
        if (target == "IOS")
        {

            targetGroup = BuildTargetGroup.iPhone;
            buildTarget = UnityEditor.BuildTarget.iPhone;
        }    
        string sourcePath="";
        switch (name)
        {
            case "UC":
                PlayerSettings.bundleIdentifier = "com.fanhou.jianghu.uc";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_UC");
                sourcePath=UI_PATH_NAME;
                break;
            case "JIUYAO":
                PlayerSettings.bundleIdentifier = "com.fanhou.jianghu.jiuyao";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_JIUYAO");                
                break;
            case "XIAOMI":
                PlayerSettings.bundleIdentifier = "Pushcraft.mi";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_XIAOMI");
                sourcePath=MI_PATH_NAME;
                break;
//            case "OPPO":
//                PlayerSettings.bundleIdentifier = "com.fanhou.jianghu.nearme";
//                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_OPPO");
//                sourcePath=OPPO_PATH_NAME;
//                break;
            case "OPPO_0917":
                PlayerSettings.bundleIdentifier = "com.fanhou.jianghu.nearme.gamecenter";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_OPPO");
                sourcePath=OPPONEW_PATH_NAME;
                break;
		case "TencentTest":
			PlayerSettings.bundleIdentifier = "com.tencent.tmgp.yjws";
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_TENCENT; TENCENT_TEST");
			PlayerSettings.keyaliasPass = "888888";
			PlayerSettings.keystorePass = "888888";
			sourcePath=TENCENT_TEST_PATH;
			break;
		case "TencentRelease":
			PlayerSettings.bundleIdentifier = "com.tencent.tmgp.yjws";
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "ANDROID_TENCENT; TENCENT_RELEASE");
			PlayerSettings.keyaliasPass = "888888";
			PlayerSettings.keystorePass = "888888";
			sourcePath=TENCENT_RELEASE_PATH;
			break;
		default:
			break;
		}
		
		BuildUtils.CopyDirectoryRecursiveIgnoreMeta(Path.Combine(PLATFORM_ANDROID_PATH, sourcePath), PLATFORM_PLUGINS_TARGET);
        AssetDatabase.Refresh();
    }


    [MenuItem("Build Target/Clean Android BuildTarget")]
    public static void CleanAndroidBuildTarget()
    {
         PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
         BuildUtils.PrepareCleanDirectory(PLATFORM_PLUGINS_TARGET);
         AssetDatabase.Refresh();
       
    }
    /// <summary>
    /// 创建包
    /// </summary>
    /// <param name="scenes"></param>
    /// <param name="target_dir"></param>
    /// <param name="build_target"></param>
    /// <param name="build_options"></param>
    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);

        string res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
        
        
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure" + res);
        }
    }


    /// <summary>
    /// 获取当前要打包的场景列表
    /// </summary>
    /// <returns></returns>
    private static string[] FindEnabledEditorScenes()
    {
        List<string> editorScenes = new List<string>();
        foreach (EditorBuildSettingsScene item in EditorBuildSettings.scenes)
        {
            if (!item.enabled) continue;

            editorScenes.Add(item.path);
        }

        return editorScenes.ToArray();
    }


    [MenuItem("Build Target/Copy Android OPPO")]
    public static void CopyAndroidOPPO()
    {

        CopyToBuildTarget("OPPO_0917", "Android");
    }

	[MenuItem("Build Target/      Copy Android Tencent(Test)")]
	public static void CopyTencentTest()
	{
		
		CopyToBuildTarget("TencentTest", "Android");
	}

	[MenuItem("Build Target/      Copy Android Tencent (Release)")]
	public static void CopyTencentRelease()
	{
		
		CopyToBuildTarget("TencentRelease", "Android");
	}

}
