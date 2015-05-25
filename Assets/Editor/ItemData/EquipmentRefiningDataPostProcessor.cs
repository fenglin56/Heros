using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class EquipmentRefiningDataPostProcessor : AssetPostprocessor
{

    public static readonly string RESOURCE_EFFECT_DATA_FOLDER = "Assets/Data/ItemData/Res";
    public static readonly string ASSET_EFFECT_DATA_FOLDER = "Assets/Data/ItemData/Data";



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

        string path = System.IO.Path.Combine(RESOURCE_EFFECT_DATA_FOLDER, "EquipmentRefining.xml");
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

            List<EquipmentRefiningData> tempList = new List<EquipmentRefiningData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                EquipmentRefiningData data = new EquipmentRefiningData();
                data.lGoodsSubClass = Convert.ToInt32(sheet["lGoodsSubClass"][i]);
                data.lColorLevel = Convert.ToInt32(sheet["lColorLevel"][i]);
                string[] levels = Convert.ToString(sheet["lLevel"][i]).Split('+');
                data.lLevel_Min = int.Parse(levels[0]);
                data.lLevel_Max = int.Parse(levels[1]);
                string[] lvUpNeed = Convert.ToString(sheet["lLevelUpNeed"][i]).Split('+');
                data.lLevelUpNeed = new List<int>();
                lvUpNeed.ApplyAllItem(P=>data.lLevelUpNeed.Add(int.Parse(P)));
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
            if (file.Contains(RESOURCE_EFFECT_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    static void CreateMedicamentConfigDataList(List<EquipmentRefiningData> list)
    {
        string fileName = typeof(EquipmentRefiningData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_EFFECT_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            EquipmentRefiningDataBase database = (EquipmentRefiningDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(EquipmentRefiningDataBase));

            if (null == database)
            {
                return;
            }

            database.EquipmentRefiningDatatable = new EquipmentRefiningData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.EquipmentRefiningDatatable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            EquipmentRefiningDataBase database = ScriptableObject.CreateInstance<EquipmentRefiningDataBase>();
            database.EquipmentRefiningDatatable = new EquipmentRefiningData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.EquipmentRefiningDatatable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
	
}
