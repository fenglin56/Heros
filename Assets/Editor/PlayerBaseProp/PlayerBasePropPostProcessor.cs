using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class PlayerBasePropPostProcessor : AssetPostprocessor
{

    public static readonly string RESOURCE_PLAYER_DATA_FOLDER = "Assets/Data/PlayerBaseProp/Res";
    public static readonly string ASSET_PLAYER_DATA_FOLDER = "Assets/Data/PlayerBaseProp/Data";


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
            if (file.Contains(RESOURCE_PLAYER_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    private static void OnPostprocessEquipment()
    {

        string path = System.IO.Path.Combine(RESOURCE_PLAYER_DATA_FOLDER, "PlayerPropParam.xml");
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

            List<PlayerBasePropData> tempList = new List<PlayerBasePropData>();

            for (int i = 1; i < levelIds.Length; i++)
            {
                if (1 >= i) continue;
                PlayerBasePropData data = new PlayerBasePropData();
                //data.PlayerKind = Convert.ToInt32(sheet["PlayerKind"][i]);
                //data.BasePropID = Convert.ToInt32(sheet["BasePropID"][i]);
                //data.BaseProp = Convert.ToString(sheet["BaseProp"][i]);
                //data.ParamA = Convert.ToSingle(sheet["ParamA"][i])/1000;
                //data.ParamB = Convert.ToSingle(sheet["ParamB"][i]) / 1000;
                //data.ParamC = Convert.ToSingle(sheet["ParamC"][i]) / 1000;
                //data.ParamD = Convert.ToSingle(sheet["ParamD"][i]) / 1000;
				data.nPropID = Convert.ToInt32(sheet["nPropID"][i]);
				data.nSettleID = Convert.ToInt32(sheet["nSettleID"][i]);

                tempList.Add(data);
            }


            CreateMedicamentConfigDataList(tempList);
        }

    }


    static void CreateMedicamentConfigDataList(List<PlayerBasePropData> list)
    {
        string fileName = typeof(PlayerBasePropData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_PLAYER_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            PlayerBasePropDataList database = (PlayerBasePropDataList)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerBasePropDataList));

            if (null == database)
            {
                return;
            }

            database.playerBasePropDatalist = new PlayerBasePropData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.playerBasePropDatalist[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerBasePropDataList database = ScriptableObject.CreateInstance<PlayerBasePropDataList>();
            database.playerBasePropDatalist = new PlayerBasePropData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.playerBasePropDatalist[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
