using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class LoadingTipsPostProcessor : AssetPostprocessor
{

    public static readonly string RESOURCE_DATA_FOLDER = "Assets/Data/LoadingTipsConfig/Res";
    public static readonly string ASSET_DATA_FOLDER = "Assets/Data/LoadingTipsConfig/Data";


    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            OnPostprocessEquipment();
        }

    }

    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    private static void OnPostprocessEquipment()
    {

        string path = System.IO.Path.Combine(RESOURCE_DATA_FOLDER, "LoadingTipsConfig.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("Equipment item file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<LoadingTipsData> tempList = new List<LoadingTipsData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                //if (0 == i) continue;
                LoadingTipsData data = new LoadingTipsData();
                data.TipsID = Convert.ToInt32(sheet["TipsID"][i]);
                string[] levels = Convert.ToString(sheet["Levels"][i]).Split('+');
                data.Levels_Min = int.Parse(levels[0]);
                data.Levels_Max = int.Parse(levels[1]);
                data.Weights = Convert.ToInt32(sheet["Weights"][i]);
                data.LoadingIDS = Convert.ToString(sheet["LoadingIDS"][i]);

                tempList.Add(data);
            }


            CreateMedicamentConfigDataList(tempList);
        }

    }


    static void CreateMedicamentConfigDataList(List<LoadingTipsData> list)
    {
        string fileName = typeof(LoadingTipsData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            LoadingTipsDataBase database = (LoadingTipsDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(LoadingTipsDataBase));

            if (null == database)
            {
                return;
            }

            database.LoadingDataList = new LoadingTipsData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.LoadingDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            LoadingTipsDataBase database = ScriptableObject.CreateInstance<LoadingTipsDataBase>();
            database.LoadingDataList = new LoadingTipsData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.LoadingDataList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
