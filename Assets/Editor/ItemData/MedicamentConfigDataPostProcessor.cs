using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;




public class MedicamentConfigDataPostProcessor : AssetPostprocessor
{
    public static readonly string RESOURCE_ITEM_DATA_FOLDER = "Assets/Data/ItemData/Res";
    public static readonly string ASSET_ITEM_DATA_FOLDER = "Assets/Data/ItemData/Data";



    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            OnPostprocessEquipment();
        }

    }

    private static void OnPostprocessEquipment()
    {

        string path = System.IO.Path.Combine(RESOURCE_ITEM_DATA_FOLDER, "MedicamentConfig.xml");
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

            List<ItemMedicamentConfigData> tempList = new List<ItemMedicamentConfigData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                ItemMedicamentConfigData data = new ItemMedicamentConfigData();
                data._PressID = Convert.ToInt32(sheet["lPresID"][i]);
                data._szPresName = Convert.ToString(sheet["szPresName"][i]);
                data._isFightUse = Convert.ToInt32(sheet["isFightUse"][i]);
                data.lType = Convert.ToInt32(sheet["lType"][i]);
                data.IntParam1 = Convert.ToInt32(sheet["IntParam1"][i]);
                data.IntParam2 = Convert.ToInt32(sheet["IntParam2"][i]);
                data.szParam = Convert.ToString(sheet["szParam"][i]);

                tempList.Add(data);
            }

            CreateMedicamentConfigDataList(tempList);
        }

    }


    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_ITEM_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    static void CreateMedicamentConfigDataList(List<ItemMedicamentConfigData> list)
    {
        string fileName = typeof(ItemMedicamentConfigData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_ITEM_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            ItemDataList database = (ItemDataList)AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList));

            if (null == database)
            {
                return;
            }

            database._itemMedicamentConfigs = new ItemMedicamentConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._itemMedicamentConfigs[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            ItemDataList database = ScriptableObject.CreateInstance<ItemDataList>();
            database._itemMedicamentConfigs = new ItemMedicamentConfigData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database._itemMedicamentConfigs[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
