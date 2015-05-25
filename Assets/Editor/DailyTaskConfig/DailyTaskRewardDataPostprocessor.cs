using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
using System;
public class DailyTaskRewardDataPostprocessor : AssetPostprocessor
{

    private static readonly string RESOURCE_TRAP_CONFIG_FOLDER = "Assets/Data/DailyTaskConfig/Res";
    private static readonly string ASSET_TRAP_CONFIG_FOLDER = "Assets/Data/DailyTaskConfig/Data";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {

        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(RESOURCE_TRAP_CONFIG_FOLDER, "DailyTaskReward.xml");
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

                List<DailyTaskRewardConfigData> tempList = new List<DailyTaskRewardConfigData>();

                for (int i = 0; i < levelIds.Length; i++)
                {
                    if (0 == i || 1 == i) continue;
                    DailyTaskRewardConfigData data = new DailyTaskRewardConfigData();

                    data._boxSequence = Convert.ToInt32(sheet["BoxSequence"][i]);
                    data._boxDisplayId = Convert.ToString(sheet["BoxDisplayId"][i]);
                    data._requirementActiveValue = Convert.ToInt32(sheet["RequirementActiveValue"][i]);

                    string[] awardTypeStr = Convert.ToString(sheet["AwardType"][i]).Split('+');
                    data._awardType = new int[awardTypeStr.Length];
                    for (int num = 0; num < awardTypeStr.Length; num++)
                    {
                        data._awardType[num] = Convert.ToInt32(awardTypeStr[num]);
                    }

                    string[] awardItems = Convert.ToString(sheet["AwardItem"][i]).Split('|');
                    data._awardItem = new DailyTaskRewardConfigData.AwardItem[awardItems.Length];
                    
                    for (int num = 0; num < awardItems.Length; num++)
                    {
                        string[] items = awardItems[num].Split('+');
                        data._awardItem[num] = new DailyTaskRewardConfigData.AwardItem();
                        data._awardItem[num].Profession = Convert.ToInt32(items[0]);
                        data._awardItem[num].PropID = Convert.ToInt32(items[1]);
                        data._awardItem[num].Num = Convert.ToInt32(items[2]);
                    }

                    data._awardMoney = Convert.ToInt32(sheet["AwardMoney"][i]);
                    data._awardExp = Convert.ToInt32(sheet["AwardExp"][i]);
                    data._awardActive = Convert.ToInt32(sheet["AwardActive"][i]);
                    data._awardXiuwei = Convert.ToInt32(sheet["AwardXiuwei"][i]);
                    data._awardIngot = Convert.ToInt32(sheet["AwardIngot"][i]);

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


    private static void CreateSceneConfigDataBase(List<DailyTaskRewardConfigData> list)
    {
        string fileName = typeof(DailyTaskRewardConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_TRAP_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            DailyTaskRewardConfigDataBase database = (DailyTaskRewardConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(DailyTaskRewardConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new DailyTaskRewardConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new DailyTaskRewardConfigData();
                database._dataTable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            DailyTaskRewardConfigDataBase database = ScriptableObject.CreateInstance<DailyTaskRewardConfigDataBase>();

            database._dataTable = new DailyTaskRewardConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new DailyTaskRewardConfigData();
                database._dataTable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }

    }
}
