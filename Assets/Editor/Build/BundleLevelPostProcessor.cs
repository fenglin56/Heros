


using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class BundleLevelPostProcessor : AssetPostprocessor
{

	public static readonly string BUNDLE_LEVEL_CONFIG_RES_FOLDER = "Assets/Editor/Build/";
    public static readonly string BUNDLE_LEVEL_CONFIG_DATA_FOLDER = "Assets/Editor/Build/";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(BUNDLE_LEVEL_CONFIG_RES_FOLDER, "BundleLevelDataBase.xml");
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

                List<BundleLevelData> tempList = new List<BundleLevelData>();

                for (int i = 2; i < levelIds.Length; i++)
                {
					BundleLevelData data = new BundleLevelData();
					data._level = Convert.ToInt32(sheet["Level"][i]);
					
					string bundleNamesStr = Convert.ToString(sheet["BundleNames"][i]);
					string[] splitBundleNameStrs = bundleNamesStr.Split('+');
					
					data._bundleNames = splitBundleNameStrs;
					
					tempList.Add(data);
                }
                CreateAssetBundlesConfigDataBase(tempList);
            }
        }
    }
	
	private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
		string path = System.IO.Path.Combine(BUNDLE_LEVEL_CONFIG_RES_FOLDER, "BundleLevelDataBase.xml");
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
	
	private static void CreateAssetBundlesConfigDataBase(List<BundleLevelData> list)
    {
        string fileName = typeof(BundleLevelDataBase).Name;
        string path = System.IO.Path.Combine(BUNDLE_LEVEL_CONFIG_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            BundleLevelDataBase database = (BundleLevelDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(BundleLevelDataBase));

            if (null == database)
            {
                return;
            }
            database._bundleDataList = new BundleLevelData[list.Count];

            list.CopyTo(database._bundleDataList);
			
            EditorUtility.SetDirty(database);
        }
        else
        {
            BundleLevelDataBase database = ScriptableObject.CreateInstance<BundleLevelDataBase>();
			
			database._bundleDataList = new BundleLevelData[list.Count];

            list.CopyTo(database._bundleDataList);

            AssetDatabase.CreateAsset(database, path);
        }

    }
}

