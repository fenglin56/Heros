using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class EffectIDDataPostProcessor : AssetPostprocessor
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

        string path = System.IO.Path.Combine(RESOURCE_EFFECT_DATA_FOLDER, "EffectID.xml");
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

            List<EffectData> tempList = new List<EffectData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                EffectData data = new EffectData();
                data.m_SzName = Convert.ToString(sheet["szName"][i]);
                data.m_IEquipmentID = Convert.ToInt32(sheet["lEquimentID"][i]);
                data.m_IskillID = Convert.ToInt32(sheet["lSkillID"][i]);
                data.BaseProp = Convert.ToInt32(sheet["lBaseProp"][i]);
                data.BasePropView = Convert.ToInt32(sheet["BasePropView"][i]);
                data.IDS = Convert.ToString(sheet["EffectNameIDS"][i]);
                data.EffectRes = Convert.ToString(sheet["EffectRes"][i]);
                data.CombatPara=Convert.ToInt32(sheet["CombatPara"][i]);
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


    static void CreateMedicamentConfigDataList(List<EffectData> list)
    {
        string fileName = typeof(EffectData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_EFFECT_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            ItemDataList database = (ItemDataList)AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList));

            if (null == database)
            {
                return;
            }

            database._effects = new EffectData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._effects[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            ItemDataList database = ScriptableObject.CreateInstance<ItemDataList>();
            database._effects = new EffectData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database._effects[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
	
}
