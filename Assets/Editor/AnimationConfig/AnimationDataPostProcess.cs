using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEditor;
using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
using System;

public class AnimationDataPostProcess : AssetPostprocessor{
    private const string EW_RESOURCE_ANIMATION_CONFIG_FOLDER = "Assets/Data/AnimationConfig/Res";
    private const string EW_ASSET_ANIMATION_CONFIG_FOLDER = "Assets/Data/AnimationConfig/Data";

    private static void OnPostprocessAllAssets(string[] importedAssets,string[] deletedAssets
        , string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets)
            || CheckResModified(deletedAssets)
            || CheckResModified(movedAssets))
        {
            string fileName = "PlayerAnimationConfig";

            MakeClipMapDataList(fileName, "Cike_Map");
            MakeClipMapDataList(fileName, "Tianshi_Map");
            MakeClipMapDataList(fileName, "Daoke_Map");
            MakeClipMapDataList(fileName, "Qinshi_Map");

            MakeConfigDataList(fileName, "Cike_Config");
            MakeConfigDataList(fileName, "Tianshi_Config");
            MakeConfigDataList(fileName, "Daoke_Config");
            MakeConfigDataList(fileName, "Qinshi_Config");
        }
    }
    private static void MakeClipMapDataList(string xmlFileName, string sheetName)
    {
        string xmlPath = Path.Combine(EW_RESOURCE_ANIMATION_CONFIG_FOLDER, xmlFileName + ".xml");

        TextReader tr = new StreamReader(xmlPath);
        string text = tr.ReadToEnd();

        if (string.IsNullOrEmpty(text))
        {
            Debug.LogError("Player animation config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text, sheetName);


            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;

            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<AnimationMapData> tempList = new List<AnimationMapData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                AnimationMapData data = new AnimationMapData();
                data.GameState = Convert.ToString(sheet["GameState"][i]);
                data.PlayerStatus = Convert.ToString(sheet["PlayerStatus"][i]);
                data.Clip = Convert.ToString(sheet["Clip"][i]);
               
                tempList.Add(data);
            }
            CreateClipMapDataBase(tempList, xmlFileName + sheetName);
        }
    }
    private static void MakeConfigDataList(string xmlFileName, string sheetName)
    {
        string xmlPath = Path.Combine(EW_RESOURCE_ANIMATION_CONFIG_FOLDER, xmlFileName + ".xml");

        TextReader tr = new StreamReader(xmlPath);
        string text = tr.ReadToEnd();

        if (string.IsNullOrEmpty(text))
        {
            Debug.LogError("Player generate config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text, sheetName);


            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;

            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<AnimationConfigData> tempList = new List<AnimationConfigData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                AnimationConfigData data = new AnimationConfigData();
                data.Clip = Convert.ToString(sheet["Clip"][i]);
                data.Mode = Convert.ToString(sheet["WrapMode"][i]);
                data.Weight = Convert.ToInt32(sheet["Weight"][i]);
                data.Time = Convert.ToSingle(sheet["Time"][i]);
                data.EventType = Convert.ToString(sheet["EventType"][i]);
                data.AfterClip = Convert.ToString(sheet["AfterClip"][i]);
                data.StepValue = Convert.ToInt32(sheet["StepValue"][i]);
                
                tempList.Add(data);
            }
            CreateAnimationConfigDataBase(tempList, xmlFileName + sheetName);
        }
    }
    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(EW_RESOURCE_ANIMATION_CONFIG_FOLDER))
            {
                fileModified = true;
                break;
            }
        }

        return fileModified;
    }
    private static void CreateClipMapDataBase(List<AnimationMapData> list, string className)
    {
        string path = Path.Combine(EW_ASSET_ANIMATION_CONFIG_FOLDER, className + ".asset");

        if (File.Exists(path))
        {
            AnimationMapDataBase database = (AnimationMapDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationMapDataBase));

            if (database == null)
            {
                return;
            }
            database.AnimationMapDatas = new AnimationMapData[list.Count];
            list.CopyTo(database.AnimationMapDatas);
            EditorUtility.SetDirty(database);
        }
        else
        {
            AnimationMapDataBase database = ScriptableObject.CreateInstance<AnimationMapDataBase>();
            database.AnimationMapDatas = new AnimationMapData[list.Count];
            list.CopyTo(database.AnimationMapDatas);
            AssetDatabase.CreateAsset(database, path);
        }
    }
    private static void CreateAnimationConfigDataBase(List<AnimationConfigData> list, string className)
    {
        string path = Path.Combine(EW_ASSET_ANIMATION_CONFIG_FOLDER, className + ".asset");

        if (File.Exists(path))
        {
            AnimationConfigDataBase database = (AnimationConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationConfigDataBase));

            if (database == null)
            {
                return;
            }
            database.AnimationConfigDatas = new AnimationConfigData[list.Count];
            list.CopyTo(database.AnimationConfigDatas);
            EditorUtility.SetDirty(database);
        }
        else
        {
            AnimationConfigDataBase database = ScriptableObject.CreateInstance<AnimationConfigDataBase>();
            database.AnimationConfigDatas = new AnimationConfigData[list.Count];
            list.CopyTo(database.AnimationConfigDatas);
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
