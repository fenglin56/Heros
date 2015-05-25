using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class CharacterNamePostProcessor : AssetPostprocessor
{


    public static readonly string RESOURCE_CHARATERNAME_DATA_FOLDER = "Assets/Data/CharacterNameConfig/Res";
    public static readonly string ASSET_CHARATERNAME_DATA_FOLDER = "Assets/Data/CharacterNameConfig/Data";


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
            if (file.Contains(RESOURCE_CHARATERNAME_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    private static void OnPostprocessEquipment()
    {

        string path = System.IO.Path.Combine(RESOURCE_CHARATERNAME_DATA_FOLDER, "CharacterNameConfig.xml");
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

            List<CharacterData> tempList = new List<CharacterData>();

            for (int i = 1; i < levelIds.Length; i++)
            {
                if (0 == i) continue;
                CharacterData data = new CharacterData();
                data.FamilyNameID = Convert.ToInt32(sheet["ID1"][i]);
                data.FamilyName = Convert.ToString(sheet["FamilyName"][i]);
                data.MaleNameID = Convert.ToInt32(sheet["ID2"][i]);
                data.MaleName = Convert.ToString(sheet["MaleName"][i]);
                data.FemaleNameID = Convert.ToInt32(sheet["ID3"][i]);
                data.FemaleName = Convert.ToString(sheet["FemaleName"][i]);

                tempList.Add(data);
            }


            CreateMedicamentConfigDataList(tempList);
        }

    }


    static void CreateMedicamentConfigDataList(List<CharacterData> list)
    {
        string fileName = typeof(CharacterData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_CHARATERNAME_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            CharacterNameDataBase database = (CharacterNameDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(CharacterNameDataBase));

            if (null == database)
            {
                return;
            }

            database.CharacterDataList = new CharacterData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.CharacterDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            CharacterNameDataBase database = ScriptableObject.CreateInstance<CharacterNameDataBase>();
            database.CharacterDataList = new CharacterData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.CharacterDataList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
