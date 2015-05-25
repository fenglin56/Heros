using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class DailySignInDataPostprocessor : AssetPostprocessor 
{
	private static readonly string RESOURCE_SHOP_CONFIG_FOLDER = "Assets/Data/ActivitieConfig/Res";
	private static readonly string ASSET_SHOP_CONFIG_FOLDER = "Assets/Data/ActivitieConfig/Data";
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
			string path = System.IO.Path.Combine(RESOURCE_SHOP_CONFIG_FOLDER, "DailySignInConfig.xml");
			TextReader tr = new StreamReader(path);
			string text = tr.ReadToEnd();
			
			if(text == null)
			{
				Debug.LogError("EndlessEctypeReward config file not exist");
				return;	
			}
			else
			{
				XmlSpreadSheetReader.ReadSheet(text);
				XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
				string[] keys =  XmlSpreadSheetReader.Keys;
				
				object[] levelIds = sheet[keys[0]];
				
				List<DailySignInConfigData> tempList = new List<DailySignInConfigData>();
				
				for(int i = 2; i< levelIds.Length; i++ )
				{
					DailySignInConfigData data = new DailySignInConfigData();
					data.RewardId = Convert.ToInt32(sheet["RewardId"][i]);
					//data.RewardLevel = Convert.ToInt32(sheet["RewardLevel"][i]);
					data.SignInReward1 = Convert.ToString(sheet["SignInReward1"][i]);
					data.SignInReward2 = Convert.ToString(sheet["SignInReward2"][i]);
					data.SignInReward3 = Convert.ToString(sheet["SignInReward3"][i]);
					data.SignInReward4 = Convert.ToString(sheet["SignInReward4"][i]);
					data.SignInReward5 = Convert.ToString(sheet["SignInReward5"][i]);
					data.SignInReward6 = Convert.ToString(sheet["SignInReward6"][i]);
					data.SignInReward7 = Convert.ToString(sheet["SignInReward7"][i]);
					data.CumulativeRewardDays1 = Convert.ToInt32(sheet["CumulativeRewardDays1"][i]);
					data.CumulativeRewardDays2 = Convert.ToInt32(sheet["CumulativeRewardDays2"][i]);
					data.CumulativeRewardDays3 = Convert.ToInt32(sheet["CumulativeRewardDays3"][i]);
					data.CumulativeRewardItem1 = Convert.ToString(sheet["CumulativeRewardItem1"][i]);
					data.CumulativeRewardItem2 = Convert.ToString(sheet["CumulativeRewardItem2"][i]);
					data.CumulativeRewardItem3 = Convert.ToString(sheet["CumulativeRewardItem3"][i]);
					data.CumulativeRewardRes1 = Convert.ToString(sheet["CumulativeRewardRes1"][i]);
					data.CumulativeRewardRes2 = Convert.ToString(sheet["CumulativeRewardRes2"][i]);
					data.CumulativeRewardRes3 = Convert.ToString(sheet["CumulativeRewardRes3"][i]);
					data.CumulativeRewardTips1 = Convert.ToInt32(sheet["CumulativeRewardTips1"][i]);
					data.CumulativeRewardTips2 = Convert.ToInt32(sheet["CumulativeRewardTips2"][i]);
					data.CumulativeRewardTips3 = Convert.ToInt32(sheet["CumulativeRewardTips3"][i]);
					tempList.Add(data);
				}
				
				CreateConfigDataBase(tempList);
			}
		}
	}
	
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
			if (file.Contains(RESOURCE_SHOP_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}
	
	
	private static void CreateConfigDataBase(List<DailySignInConfigData> list)
	{
		string fileName = typeof(DailySignInConfigData).Name;
		string path = System.IO.Path.Combine(ASSET_SHOP_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
			DailySignInConfigDataBase database = (DailySignInConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(DailySignInConfigDataBase));
			
			if(null == database)
			{
				return;
			}
			
			database._dataTable = new DailySignInConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			DailySignInConfigDataBase database = ScriptableObject.CreateInstance<DailySignInConfigDataBase>();
			
			database._dataTable = new DailySignInConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}

