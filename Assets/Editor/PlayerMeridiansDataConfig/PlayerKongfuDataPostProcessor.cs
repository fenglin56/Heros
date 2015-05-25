using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class PlayerKongfuDataPostProcessor : AssetPostprocessor
{

    public static readonly string RESOURCE_MERIDIANS_DATA_FOLDER = "Assets/Data/PlayerMeridiansConfig/Res";
    public static readonly string ASSET_MERIDIANS_DATA_FOLDER = "Assets/Data/PlayerMeridiansConfig/Data";

    public static readonly string RESOURCE_KONGFUDATA_DATA_FOLDER = "Assets/Data/PlayerMeridiansConfig/Res";
    public static readonly string ASSET_KONGFUDATA_DATA_FOLDER = "Assets/Data/PlayerMeridiansConfig/Data";
    public static readonly string KongFuBackgroundResPath = "Assets/Prefab/GUI/KongfuBG";

    public static readonly string RESOURCE_EFFECTPOINT_DATA_FOLDER = "Assets/Data/PlayerMeridiansConfig/Res";
    public static readonly string ASSET_EFFECTPOINT_DATA_FOLDER = "Assets/Data/PlayerMeridiansConfig/Data";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckMeridiansResModified(importedAssets) || CheckMeridiansResModified(deletedAssets) || CheckMeridiansResModified(movedAssets))
        {
            OnPostprocessMERIDIANS();
        }
        if (CheckKongfuResModified(importedAssets) || CheckKongfuResModified(deletedAssets) || CheckKongfuResModified(movedAssets))
        {
            OnPostprocessKongfudata();
        }

        if (CheckEffectResModified(importedAssets) || CheckEffectResModified(deletedAssets) || CheckEffectResModified(movedAssets))
        {
            OnPostprocessEffectPointdata();
        }
    }

    private static bool CheckMeridiansResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_MERIDIANS_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }

    private static bool CheckKongfuResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_KONGFUDATA_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }

    private static bool CheckEffectResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_EFFECTPOINT_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }

    private static void OnPostprocessMERIDIANS()
    {

        string path = System.IO.Path.Combine(RESOURCE_MERIDIANS_DATA_FOLDER, "PlayerMeridiansData.xml");
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

            List<PlayerMeridiansData> tempList = new List<PlayerMeridiansData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                PlayerMeridiansData data = new PlayerMeridiansData();
                data.MeridiansLevel = Convert.ToInt32(sheet["MeridiansLevel"][i]);
                data.KongfuLevel = Convert.ToInt32(sheet["KongfuLevel"][i]);
                data.KongfuName = Convert.ToString(sheet["KongfuName"][i]);
                data.LevelUpNeed = Convert.ToInt32(sheet["LevelUpNeed"][i]);
                data.EffectAdd = Convert.ToString(sheet["EffectAdd"][i]);

                tempList.Add(data);
            }


            CreateMeridiansConfigDataList(tempList);
        }

    }

    private static void OnPostprocessKongfudata()
    {

        string path = System.IO.Path.Combine(RESOURCE_KONGFUDATA_DATA_FOLDER, "PlayerKongfuData.xml");
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

            List<PlayerKongfuData> tempList = new List<PlayerKongfuData>();

            for (int i = 3; i < levelIds.Length; i++)
            {
                PlayerKongfuData data = new PlayerKongfuData();
                data.KongfuLevel = Convert.ToInt32(sheet["KongfuLevel"][i]);
                data.LevelNeed = Convert.ToInt32(sheet["LevelNeed"][i]);
                data.MeridiansList = Convert.ToString(sheet["MeridiansSequence"][i]).Split('+');
                data.KongfuName = Convert.ToString(sheet["KongfuName"][i]);
                data.KongfuPic = Convert.ToString(sheet["KongfuPic"][i]);
                data.KongfuEff = Convert.ToString(sheet["KongfuEff"][i]);
                data.KongfuNameRes = Convert.ToString(sheet["KongfuNameRes"][i]);

                string KonfubakResPath = System.IO.Path.Combine(KongFuBackgroundResPath,data.KongfuPic+".prefab");
                data.KongfuPicPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(KonfubakResPath,typeof(GameObject));
                Debug.Log("Inport:" + data.KongfuPicPrefab + "," + KonfubakResPath);
                tempList.Add(data);
            }
            CreateKongfuConfigDataList(tempList);
        }

    }

    private static void OnPostprocessEffectPointdata()
    {

        string path = System.IO.Path.Combine(RESOURCE_EFFECTPOINT_DATA_FOLDER, "MeridiansEffectPosition.xml");
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

            List<MeridiansEffectPositionData> tempList = new List<MeridiansEffectPositionData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                MeridiansEffectPositionData data = new MeridiansEffectPositionData();
                data.effectID = Convert.ToInt32(sheet["EffectID"][i]);
                data.position = Convert.ToString(sheet["Position"][i]).Split('+');

                tempList.Add(data);
            }
            CreateEffectPositionConfigDataList(tempList);
        }

    }


    static void CreateMeridiansConfigDataList(List<PlayerMeridiansData> list)
    {
        string fileName = typeof(PlayerMeridiansData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_MERIDIANS_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            PlayerMeridiansDataBase database = (PlayerMeridiansDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerMeridiansDataBase));

            if (null == database)
            {
                return;
            }

            database.PlayermeridiansDataList = new PlayerMeridiansData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.PlayermeridiansDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerMeridiansDataBase database = ScriptableObject.CreateInstance<PlayerMeridiansDataBase>();
            database.PlayermeridiansDataList = new PlayerMeridiansData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.PlayermeridiansDataList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

    static void CreateKongfuConfigDataList(List<PlayerKongfuData> list)
    {
        string fileName = typeof(PlayerKongfuData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_KONGFUDATA_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            PlayerMeridiansDataBase database = (PlayerMeridiansDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerMeridiansDataBase));

            if (null == database)
            {
                return;
            }

            database.PlayerKongfuDataList = new PlayerKongfuData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.PlayerKongfuDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerMeridiansDataBase database = ScriptableObject.CreateInstance<PlayerMeridiansDataBase>();
            database.PlayerKongfuDataList = new PlayerKongfuData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.PlayerKongfuDataList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

    static void CreateEffectPositionConfigDataList(List<MeridiansEffectPositionData> list)
    {
        string fileName = typeof(MeridiansEffectPositionData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_KONGFUDATA_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            PlayerMeridiansDataBase database = (PlayerMeridiansDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerMeridiansDataBase));

            if (null == database)
            {
                return;
            }

            database.MeridiansEffectPositionDataList = new MeridiansEffectPositionData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.MeridiansEffectPositionDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerMeridiansDataBase database = ScriptableObject.CreateInstance<PlayerMeridiansDataBase>();
            database.MeridiansEffectPositionDataList = new MeridiansEffectPositionData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.MeridiansEffectPositionDataList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
