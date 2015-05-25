

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class AssetBundlesConfigPostProcessor : AssetPostprocessor
{

	public static readonly string BUNDLE_CONFIG_RES_FOLDER = "Assets/Editor/Build/";
    public static readonly string BUNDLE_CONFIG_DATA_FOLDER = "Assets/Editor/Build/";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(BUNDLE_CONFIG_RES_FOLDER, "AssetBundlesConfig.xml");
            TextReader tr = new StreamReader(path);
            string text = tr.ReadToEnd();

            if (text == null)
            {
                Debug.LogError("Bullet data file not exist");
                return;
            }
            else
            {
                XmlSpreadSheetReader.ReadSheet(text);
                XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
                string[] keys = XmlSpreadSheetReader.Keys;

                object[] levelIds = sheet[keys[0]];

                List<AssetBundlesConfig.Rule> tempList = new List<AssetBundlesConfig.Rule>();

                for (int i = 2; i < levelIds.Length; i++)
                {
					AssetBundlesConfig.Rule rule = new AssetBundlesConfig.Rule();
					rule._name = Convert.ToString(sheet["Name"][i]);
					rule._bundleName = Convert.ToString(sheet["BundleName"][i]);
					rule._enabled = true;
					rule._bundleOrder = i-1;
					rule._isolate = false;
					rule._pattern = Convert.ToString(sheet["Pattern"][i]);
					
					rule._patternOptions = new AssetBundlesConfig.RegexOptionsWrapper();
					rule._patternOptions._ignoreCase = Convert.ToBoolean(sheet["IgnoreCase"][i]);
					rule._patternOptions._ignoreWhitespace = true;
					rule._patternOptions._rightToLeft = false;
					
					rule._patternReplaceModifier = AssetBundlesConfig.RegexReplaceModifier.Lowercase;
					rule._type = AssetBundlesConfig.RuleType.Include;
					tempList.Add(rule);
                }

                CreateAssetBundlesConfigDataBase(tempList);
            }
        }
    }
	
	private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
		string path = System.IO.Path.Combine(BUNDLE_CONFIG_RES_FOLDER, "AssetBundlesConfig.xml");
        foreach (string file in files)
        {
			
            if (file.Contains(path))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }
	
	private static void CreateAssetBundlesConfigDataBase(List<AssetBundlesConfig.Rule> list)
    {
        string fileName = typeof(AssetBundlesConfig).Name;
        string path = System.IO.Path.Combine(BUNDLE_CONFIG_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            AssetBundlesConfig database = (AssetBundlesConfig)AssetDatabase.LoadAssetAtPath(path, typeof(AssetBundlesConfig));

            if (null == database)
            {
                return;
            }
            database._rules = list;
			
            EditorUtility.SetDirty(database);
        }
        else
        {
            AssetBundlesConfig database = ScriptableObject.CreateInstance<AssetBundlesConfig>();

            database._rules = list;
            AssetDatabase.CreateAsset(database, path);
        }

    }
}
