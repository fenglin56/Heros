using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
using System;

public class DailyTaskDataPostprocessor : AssetPostprocessor
{
    private static readonly string RESOURCE_TRAP_CONFIG_FOLDER = "Assets/Data/DailyTaskConfig/Res";
    private static readonly string ASSET_TRAP_CONFIG_FOLDER = "Assets/Data/DailyTaskConfig/Data";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {

        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(RESOURCE_TRAP_CONFIG_FOLDER, "DailyTask.xml");
            TextReader tr = new StreamReader(path);
            string text = tr.ReadToEnd();

            if (text == null)
            {
                Debug.LogError("Player level config file not exist");
                return;
            }
            else
            {
                XmlSpreadSheetReader.ReadSheet(text);
                XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
                string[] keys = XmlSpreadSheetReader.Keys;

                object[] levelIds = sheet[keys[0]];

                List<DailyTaskConfigData> tempList = new List<DailyTaskConfigData>();

                for (int i = 0; i < levelIds.Length; i++)
                {
                    if (0 == i || 1 == i) continue;
                    DailyTaskConfigData data = new DailyTaskConfigData();

                    data._taskID = Convert.ToInt32(sheet["TaskID"][i]);
                    data._taskType = Convert.ToInt32(sheet["TaskType"][i]);
                    data._triggerCondition = Convert.ToInt32(sheet["TriggerCondition"][i]);
                    data._taskDescription = Convert.ToString(sheet["TaskDescription"][i]);
                    data._conditionParameter = Convert.ToInt32(sheet["ConditionParameter"][i]);
                    data._activeValue = Convert.ToInt32(sheet["ActiveValue"][i]);
                    data._quickTripTo = Convert.ToInt32(sheet["QuickTripTo"][i]);

                    tempList.Add(data);
                }

                CreateSceneConfigDataBase(tempList);
            }
        }
    }


    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_TRAP_CONFIG_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    private static void CreateSceneConfigDataBase(List<DailyTaskConfigData> list)
    {
        string fileName = typeof(DailyTaskConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_TRAP_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            DailyTaskConfigDataBase database = (DailyTaskConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(DailyTaskConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new DailyTaskConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new DailyTaskConfigData();
                database._dataTable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            DailyTaskConfigDataBase database = ScriptableObject.CreateInstance<DailyTaskConfigDataBase>();

            database._dataTable = new DailyTaskConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new DailyTaskConfigData();
                database._dataTable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }

    }
}
