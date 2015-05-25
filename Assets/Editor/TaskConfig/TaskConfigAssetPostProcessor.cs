using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class TaskConfig : AssetPostprocessor 
{
	private static readonly string RESOURCE_TASK_CONFIG_FOLDER = "Assets/Data/TaskConfig/Res";
    private static readonly string ASSET_TASK_CONFIG_FOLDER = "Assets/Data/TaskConfig/Data";
    //private static readonly string ASSET_GUIDE_RES_CONFIG_FOLDER = "Assets/Prefab/GUI/NewbieGuide/Prefab/";
    //private static readonly string ASSET_GUIDE_ATALS_CONFIG_FOLDER = "Assets/UI/Textures/EctypeGuide/";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
        return;
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            TaskConfigPostprocess();
		}
	}

    private static void TaskConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_TASK_CONFIG_FOLDER, "TaskConfig.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
			
		if(text == null)
		{
			Debug.LogError("newbie guide config file not exist");
			return;	
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys =  XmlSpreadSheetReader.Keys;
				
			object[] levelIds = sheet[keys[0]];

            List<TaskConfigData> tempList = new List<TaskConfigData>();

			for(int i = 0; i< levelIds.Length; i++ )
			{
                if (0 == i || 1 == i) continue;
                TaskConfigData data = new TaskConfigData();
                data._TaskID = Convert.ToInt32(sheet["TaskID"][i]);
                data._TaskTitle= Convert.ToString(sheet["TaskTitle"][i]);
                data._TaskType = Convert.ToInt32(sheet["TaskType"][i]);
                data._TaskDesc = Convert.ToString(sheet["TaskDescription"][i]);
                data._TaskGoals = Convert.ToString(sheet["TaskGoals"][i]);
                data._AwardType = Convert.ToInt32(sheet["AwardType"][i]);
                string[] awardItemListStr = Convert.ToString(sheet["AwardItem"][i]).Split('|');
                data._AwardItemList = new AwardItem[awardItemListStr.Length];
                for (int k = 0; k < awardItemListStr.Length; k++)
                {
                    string[] awardItemStr = Convert.ToString(awardItemListStr[k]).Split('+');
                    if (awardItemStr.Length == 3)
                    {
                        data._AwardItemList[k] = new AwardItem();
                        data._AwardItemList[k]._Vocation = Convert.ToInt32(awardItemStr[0]);
                        data._AwardItemList[k]._PropID = Convert.ToInt32(awardItemStr[1]);
                        data._AwardItemList[k]._PropNum = Convert.ToInt32(awardItemStr[2]);
                    }
                }

                string[] awardMoneyStr = Convert.ToString(sheet["AwardMoney"][i]).Split('+');
                if (awardMoneyStr.Length == 2)
                {
                    data._AwardMoney = Convert.ToInt32(awardMoneyStr[1]);
                }

                string[] awardExpStr = Convert.ToString(sheet["AwardExp"][i]).Split('+');
                if (awardExpStr.Length == 2)
                {
                    data._AwardExp = Convert.ToInt32(awardExpStr[1]);
                }
                data._AwardActive = Convert.ToInt32(sheet["AwardActive"][i]);
                data._AwardXiuWei = Convert.ToInt32(sheet["AwardXiuwei"][i]);
                data._GuideType = Convert.ToInt32(sheet["GuideType"][i]);
                string[] townGuideStr = Convert.ToString(sheet["GuideGroup"][i]).Split('+');
                data._TownGuideList = new int[townGuideStr.Length];
                for (int k = 0; k < townGuideStr.Length; k++ )
                {
                    data._TownGuideList[k] = Convert.ToInt32(townGuideStr[k]);
                }              
               

                data._CloseUI = Convert.ToInt32(sheet["CloseUI"][i]);
				data._IsEnableLvTips = Convert.ToBoolean(sheet["FunctionTips"][i]);
				data._EnableLevel = Convert.ToInt32(sheet["FunctionOpenLevel"][i]);
				data._FunctionIconName = Convert.ToString(sheet["FunctionIcon"][i]);
				data._FunctionName = Convert.ToString(sheet["FunctionName"][i]);
                data._NewFunDelayTime = Convert.ToInt32(sheet["DelayTime"][i]);
				
                tempList.Add(data);
			}

            CreateTaskConfigDataBase(tempList);
		}
    }

	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
            if (file.Contains(RESOURCE_TASK_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateTaskConfigDataBase(List<TaskConfigData> list)
	{
        string fileName = typeof(TaskConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_TASK_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            TaskConfigDataBase database = (TaskConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(TaskConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new TaskConfigData[list.Count];

            for(int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new TaskConfigData();
                database._dataTable[i] = list[i];
            //    database._dataTable[i]._TaskID = list[i]._TaskID;
            //    database._dataTable[i]._TaskName = list[i]._TaskName;
            //    database._dataTable[i]._TaskType = list[i]._TaskType;
            //    database._dataTable[i]._TaskDesc = list[i]._TaskDesc;
            //    database._dataTable[i]._TaskGoals = list[i]._TaskGoals;
            //    database._dataTable[i]._AwardType = list[i]._AwardType;
            //    database._dataTable[i]._AwardItemList = list[i]._AwardItemList;
            //    database._dataTable[i]._AwardMoney = list[i]._AwardMoney;
            //    database._dataTable[i]._AwardExp = list[i]._AwardExp;
            //    database._dataTable[i]._GuideType = list[i]._GuideType;
            //    database._dataTable[i]._TownGuideList = list[i]._TownGuideList;
            //    database._dataTable[i]._EctypeGuideList = list[i]._EctypeGuideList;
            //    database._dataTable[i]._EnableFunc = list[i]._EnableFunc;
            }
			EditorUtility.SetDirty(database);
		}
		else
		{
            TaskConfigDataBase database = ScriptableObject.CreateInstance<TaskConfigDataBase>();

            database._dataTable = new TaskConfigData[list.Count];

            for(int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new TaskConfigData();
                database._dataTable[i] = list[i];
            //    database._dataTable[i]._TaskID = list[i]._TaskID;
            //    database._dataTable[i]._TaskName = list[i]._TaskName;
            //    database._dataTable[i]._TaskType = list[i]._TaskType;
            //    database._dataTable[i]._TaskDesc = list[i]._TaskDesc;
            //    database._dataTable[i]._TaskGoals = list[i]._TaskGoals;
            //    database._dataTable[i]._AwardType = list[i]._AwardType;
            //    database._dataTable[i]._AwardItemList = list[i]._AwardItemList;
            //    database._dataTable[i]._AwardMoney = list[i]._AwardMoney;
            //    database._dataTable[i]._AwardExp = list[i]._AwardExp;
            //    database._dataTable[i]._GuideType = list[i]._GuideType;
            //    database._dataTable[i]._TownGuideList = list[i]._TownGuideList;
            //    database._dataTable[i]._EctypeGuideList = list[i]._EctypeGuideList;
            //    database._dataTable[i]._EnableFunc = list[i]._EnableFunc;
            }
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
