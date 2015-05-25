using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class CommonIconDataPostProcessor : AssetPostprocessor {

    public static readonly string RESOURCE_ICON_DATA_FOLDER = "Assets/Data/CommonIcon/Res";
    public static readonly string ASSET_ICON_DATA_FOLDER = "Assets/Data/CommonIcon/Data";
    public static readonly string ASSET_ICON_DATA_FOLDER_PATH = "Assets/Prefab/GUI/ItemIcon/CommonIcon";


    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            OnPostprocessEquipment("TownUI");
            OnPostprocessEquipment("BattleUI");
        }

    }

    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_ICON_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    private static void OnPostprocessEquipment(string tag)
    {

        string path = System.IO.Path.Combine(RESOURCE_ICON_DATA_FOLDER, "CommonIconConfig.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("Equipment item file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text,tag);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<CommonIconData> tempList = new List<CommonIconData>();

            for (int i = 1; i < levelIds.Length; i++)
            {
                if (0 == i) continue;
                CommonIconData data = new CommonIconData();
                data._IconName = Convert.ToString(sheet["IConName"][i]);
                string prefabpath = Convert.ToString(sheet["IconPrefab"][i]);
                string pathRes = System.IO.Path.Combine(ASSET_ICON_DATA_FOLDER_PATH, prefabpath + ".prefab");
                data._IconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

                tempList.Add(data);
            }


            CreateMedicamentConfigDataList(tempList,tag);
        }

    }


    static void CreateMedicamentConfigDataList(List<CommonIconData> list, string tag)
    {
        string fileName = typeof(CommonIconData).Name + "DataBase" + tag;
        string path = System.IO.Path.Combine(ASSET_ICON_DATA_FOLDER, fileName +".asset");

        if (File.Exists(path))
        {
            CommonIconDataList database = (CommonIconDataList)AssetDatabase.LoadAssetAtPath(path, typeof(CommonIconDataList));

            if (null == database)
            {
                return;
            }

            database._CommonIcons = new CommonIconData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._CommonIcons[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            CommonIconDataList database = ScriptableObject.CreateInstance<CommonIconDataList>();
            database._CommonIcons = new CommonIconData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database._CommonIcons[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
