using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Linq;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class MissionFaiBtnAssetPostProcessor : AssetPostprocessor
{
    private static readonly string RESOURCE_DATA_FOLDER = "Assets/Data/FailTips/Res";
    private static readonly string ASSET_DATA_FOLDER = "Assets/Data/FailTips/Data";
    private static readonly string IconPrefabPath = "Assets/Prefab/GUI/MainButton";



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

        string path = System.IO.Path.Combine(RESOURCE_DATA_FOLDER, "FailTips.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("FilBtn Res file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<MissionFailData> tempList = new List<MissionFailData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                MissionFailData data = new MissionFailData();
                data.BtnID = Convert.ToInt32(sheet["ButtonID"][i]);
                data.SysModule = Convert.ToInt32(sheet["SysModule"][i]);
                data.NameIDS = Convert.ToString(sheet["ButtonName"][i]);
                data.ButtonExplainIDS = Convert.ToString(sheet["ButtonExplain"][i]);
                data.BtnType = (UI.MainUI.UIType)data.SysModule;
                //data.NeedTask = Convert.ToInt32(sheet["NeedTask"][i]);

                string iconStr = Convert.ToString(sheet["Icon"][i]);
                string iconPath = System.IO.Path.Combine(IconPrefabPath,iconStr+".prefab");
                data.IconPrefab = AssetDatabase.LoadAssetAtPath(iconPath, typeof(GameObject)) as GameObject;

                tempList.Add(data);
            }


            CreateMedicamentConfigDataList(tempList);
        }

    }


    static void CreateMedicamentConfigDataList(List<MissionFailData> list)
    {
        string fileName = typeof(MissionFailData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            MissionFailBtnDataBase database = (MissionFailBtnDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(MissionFailBtnDataBase));

            if (null == database)
            {
                return;
            }

            database.MissionFailDataTable = new MissionFailData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.MissionFailDataTable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            MissionFailBtnDataBase database = ScriptableObject.CreateInstance<MissionFailBtnDataBase>();
            database.MissionFailDataTable = new MissionFailData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.MissionFailDataTable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }


    }

}
