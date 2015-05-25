using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;


public class SKillUIConfigAssetPostProcessor : AssetPostprocessor
{
    public static readonly string RESOURCE_SKILLUI_DATA_FOLDER = "Assets/Data/SkillUIConfig/Res";
    public static readonly string ASSET_SKILLUI_DATA_FOLDER = "Assets/Data/SkillUIConfig/Data";
    public static readonly string ASSET_SKILLUI_DATA_ICON_FOLDER = "Assets/Prefab/GUI/ItemIcon/SkillICon";




    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            OnPostprocessSkillUI();
        }

    }

    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_SKILLUI_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }

    private static void OnPostprocessSkillUI()
    {

        string path = System.IO.Path.Combine(RESOURCE_SKILLUI_DATA_FOLDER, "SkillUIConfig.xml");
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

            List<SkillUIConfigData> tempList = new List<SkillUIConfigData>();

            for (int i = 1; i < levelIds.Length; i++)
            {
                if (0 == i) continue;
                SkillUIConfigData data = new SkillUIConfigData();
                data._SkillID = Convert.ToInt32(sheet["SkillID"][i]);
                data._SkillName = Convert.ToString(sheet["SkillName"][i]);
                data._SkillDes = Convert.ToString(sheet["SkillDes"][i]);
                data._SkillClass = Convert.ToString(sheet["SkillClass"][i]);
                data._SkillLv = Convert.ToString(sheet["SkillLv"][i]);
                data._UnlockLv = Convert.ToString(sheet["UnlockLv"][i]);
                data._UpgradeReq = Convert.ToString(sheet["UpgradeReq"][i]);
                data._CD = Convert.ToString(sheet["CD"][i]);
                data._Consume = Convert.ToString(sheet["Consume"][i]);
                data._TriggetType = Convert.ToString(sheet["TriggetType"][i]);
                data._DirectionType = Convert.ToString(sheet["DirectionType"][i]);
                data._DirectionValue = Convert.ToString(sheet["DirectionValue"][i]);
                data._WorkTarget = Convert.ToString(sheet["WorkTarget"][i]);
                data._BulletIds = Convert.ToString(sheet["BulletIds"][i]);
                data._SpMove = Convert.ToString(sheet["SpMove"][i]);
                data._SpMoveSpeed = Convert.ToString(sheet["SpMoveSpeed"][i]);
                data._SpMoveAcc = Convert.ToString(sheet["SpMoveAcc"][i]);
                data._SpMoveDir = Convert.ToString(sheet["SpMoveDir"][i]);
                data._SpMoveTime = Convert.ToString(sheet["SpMoveTime"][i]);
                data._SpMoveAcc = Convert.ToString(sheet["SpMoveAcc"][i]);
                data._BaTiLV = Convert.ToString(sheet["BaTiLV"][i]);
                data._StopDir = Convert.ToString(sheet["StopDir"][i]);

                string prefabpath = Convert.ToString(sheet["SkillImgID"][i]);
                string pathRes = System.IO.Path.Combine(ASSET_SKILLUI_DATA_ICON_FOLDER, prefabpath + ".prefab");
                data._IconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

                tempList.Add(data);
            }

            CreateEquipmentDataList(tempList);
        }
        //string path = System.IO.Path.Combine(RESOURCE_SKILLUI_DATA_FOLDER, "SkillUIConfig.xml");
        //TextReader tr = new StreamReader(path);
        //string text = tr.ReadToEnd();

        //if (text == null)
        //{
        //    Debug.LogError("Equipment item file not exist");
        //    return;
        //}
        //else
        //{
        //    XmlSpreadSheetReader.ReadSheet(text);
        //    XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
        //    string[] keys = XmlSpreadSheetReader.Keys;

        //    object[] levelIds = sheet[keys[0]];

        //    List<SkillUIConfigData> tempList = new List<SkillUIConfigData>();

        //    for (int i = 1; i < levelIds.Length; i++)
        //    {
        //        if (0 == i ) continue;
        //        SkillUIConfigData data = new SkillUIConfigData();
        //        data._SkillID = Convert.ToInt32(sheet["SkillID"][i]);
        //        data._SkillName = Convert.ToString(sheet["SkillName"][i]);
        //        data._SkillDes = Convert.ToString(sheet["SkillDes"][i]);
        //        data._SkillClass = Convert.ToString(sheet["SkillClass"][i]);
        //        data._SkillLv = Convert.ToString(sheet["SkillLv"][i]);
        //        data._UnlockLv = Convert.ToString(sheet["UnlockLv"][i]);
        //        data._UpgradeReq = Convert.ToString(sheet["UpgradeReq"][i]);
        //        data._CD = Convert.ToString(sheet["CD"][i]);
        //        data._Consume = Convert.ToString(sheet["Consume"][i]);
        //        data._TriggetType = Convert.ToString(sheet["TriggetType"][i]);
        //        data._DirectionType = Convert.ToString(sheet["DirectionType"][i]);
        //        data._DirectionValue = Convert.ToString(sheet["DirectionValue"][i]);
        //        data._WorkTarget = Convert.ToString(sheet["WorkTarget"][i]);
        //        data._BulletIds = Convert.ToString(sheet["BulletIds"][i]);
        //        data._SpMove = Convert.ToString(sheet["SpMove"][i]);
        //        data._SpMoveSpeed = Convert.ToString(sheet["SpMoveSpeed"][i]);
        //        data._SpMoveAcc = Convert.ToString(sheet["SpMoveAcc"][i]);
        //        data._SpMoveDir = Convert.ToString(sheet["SpMoveDir"][i]);
        //        data._SpMoveTime = Convert.ToString(sheet["SpMoveTime"][i]);
        //        data._SpMoveAcc = Convert.ToString(sheet["SpMoveAcc"][i]);
        //        data._BaTiLV = Convert.ToString(sheet["BaTiLV"][i]);
        //        data._StopDir = Convert.ToString(sheet["StopDir"][i]);

        //        string prefabpath = Convert.ToString(sheet["SkillImgID"][i]);
        //        string pathRes = System.IO.Path.Combine(ASSET_SKILLUI_DATA_ICON_FOLDER, prefabpath + ".prefab");
        //        data._IconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

        //        tempList.Add(data);
        //    }

        //    CreateEquipmentDataList(tempList);
        //}

    }

    static void CreateEquipmentDataList(List<SkillUIConfigData> list)
    {
        string fileName = typeof(SkillUIConfigData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_SKILLUI_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            SkillUIConfigDataList database = (SkillUIConfigDataList)AssetDatabase.LoadAssetAtPath(path, typeof(SkillUIConfigDataList));

            if (null == database)
            {
                return;
            }

            database.skillConfigDatalist = new SkillUIConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.skillConfigDatalist[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            SkillUIConfigDataList database = ScriptableObject.CreateInstance<SkillUIConfigDataList>();

            database.skillConfigDatalist = new SkillUIConfigData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.skillConfigDatalist[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }

    }

}
