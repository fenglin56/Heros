using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;
using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class EquipStrengthenDataPostProcessor : AssetPostprocessor
{
    public static readonly string RESOURCE_EQUIPSTREN_DATA_FOLDER = "Assets/Data/ItemData/Res";
    private const string Data_EQUIPSTREN_DATA_FOLDER = "Assets/Data/ItemData/Data";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            OnPostprocessEquipStrengthen();
        }

    }
    private static void OnPostprocessEquipStrengthen()
    {

        string path = System.IO.Path.Combine(RESOURCE_EQUIPSTREN_DATA_FOLDER, "EquipStrengthen.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("EquipStrengthen file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<EquipStrengthenData> tempList = new List<EquipStrengthenData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                EquipStrengthenData data = new EquipStrengthenData();
                var range = Convert.ToString(sheet["NormalLVRange"][i]);
                var lvs = range.Split('+');
                data.StartLv = Convert.ToInt32(lvs[0]);
                data.EndLv = Convert.ToInt32(lvs[1]);
                data.Prefix = Convert.ToString(sheet["Prefix"][i]);
                data.Postfix = Convert.ToString(sheet["Postfix"][i]);

                tempList.Add(data);
            }
            CreateEquipStrengthenConfigDataBase(tempList);
        }

    }
    private static void CreateEquipStrengthenConfigDataBase(List<EquipStrengthenData> list)
    {
        string fileName = typeof(EquipStrengthenData).Name + "DataBase";
        string path = System.IO.Path.Combine(Data_EQUIPSTREN_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            EquipStrengthenDataList database = (EquipStrengthenDataList)AssetDatabase.LoadAssetAtPath(path, typeof(EquipStrengthenDataList));

            if (null == database)
            {
                return;
            }

            database._EquipStrengthenDatas = new EquipStrengthenData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._EquipStrengthenDatas[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            EquipStrengthenDataList database = ScriptableObject.CreateInstance<EquipStrengthenDataList>();
            database._EquipStrengthenDatas = new EquipStrengthenData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database._EquipStrengthenDatas[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_EQUIPSTREN_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }
}
