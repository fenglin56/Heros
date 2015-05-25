using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class ActivityConfigDataPostprocessor : AssetPostprocessor 
{
	private static readonly string RESOURCE_SHOP_CONFIG_FOLDER = "Assets/Data/ActivitieConfig/Res";
	private static readonly string ASSET_SHOP_CONFIG_FOLDER = "Assets/Data/ActivitieConfig/Data";
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
			string path = System.IO.Path.Combine(RESOURCE_SHOP_CONFIG_FOLDER, "ActivityConfig.xml");
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
				
				List<ActivityConfigData> tempList = new List<ActivityConfigData>();
				
				for(int i = 2; i< levelIds.Length; i++ )
				{
					ActivityConfigData data = new ActivityConfigData();
					data.ActivityID = Convert.ToInt32(sheet["ActivityID"][i]);
					data.ActivityPictrue = Convert.ToString(sheet["ActivityPictrue"][i]);
					string pathRes = System.IO.Path.Combine("Assets/Prefab/GUI/IconPrefab/OnlineActivityIcon/", data.ActivityPictrue + ".prefab");
					data.ActivityPictruePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));
					data.ActivityName = Convert.ToString(sheet["ActivityName"][i]);
					data.UIOrder = Convert.ToInt32(sheet["UIOrder"][i]);
					data.RuleDescription = Convert.ToString(sheet["RuleDescription"][i]);
					data.QualifiedDescription = Convert.ToString(sheet["QualifiedDescription"][i]);
					data.Qualified1 = Convert.ToInt32(sheet["Qualified1"][i]);
					data.Reward1 = Convert.ToString(sheet["Reward1"][i]);
					data.Qualified2 = Convert.ToInt32(sheet["Qualified2"][i]);
					data.Reward2 = Convert.ToString(sheet["Reward2"][i]);
					data.Qualified3 = Convert.ToInt32(sheet["Qualified3"][i]);
					data.Reward3 = Convert.ToString(sheet["Reward3"][i]);
					data.Qualified4 = Convert.ToInt32(sheet["Qualified4"][i]);
					data.Reward4 = Convert.ToString(sheet["Reward4"][i]);
					data.Qualified5 = Convert.ToInt32(sheet["Qualified5"][i]);
					data.Reward5 = Convert.ToString(sheet["Reward5"][i]);
					data.Qualified6 = Convert.ToInt32(sheet["Qualified6"][i]);
					data.Reward6 = Convert.ToString(sheet["Reward6"][i]);
					data.Qualified7 = Convert.ToInt32(sheet["Qualified7"][i]);
					data.Reward7 = Convert.ToString(sheet["Reward7"][i]);
					data.Qualified8 = Convert.ToInt32(sheet["Qualified8"][i]);
					data.Reward8 = Convert.ToString(sheet["Reward8"][i]);
					data.Qualified9 = Convert.ToInt32(sheet["Qualified9"][i]);
					data.Reward9 = Convert.ToString(sheet["Reward9"][i]);
					data.Qualified10 = Convert.ToInt32(sheet["Qualified10"][i]);
					data.Reward10 = Convert.ToString(sheet["Reward10"][i]);
					data.Qualified11 = Convert.ToInt32(sheet["Qualified11"][i]);
					data.Reward11 = Convert.ToString(sheet["Reward11"][i]);
					data.Qualified12 = Convert.ToInt32(sheet["Qualified12"][i]);
					data.Reward12 = Convert.ToString(sheet["Reward12"][i]);

					data.Qualified13 = Convert.ToInt32(sheet["Qualified13"][i]);
					data.Reward13 = Convert.ToString(sheet["Reward13"][i]);
					data.Qualified14 = Convert.ToInt32(sheet["Qualified14"][i]);
					data.Reward14 = Convert.ToString(sheet["Reward14"][i]);
					data.Qualified15 = Convert.ToInt32(sheet["Qualified15"][i]);
					data.Reward15 = Convert.ToString(sheet["Reward15"][i]);
					data.Qualified16 = Convert.ToInt32(sheet["Qualified16"][i]);
					data.Reward16 = Convert.ToString(sheet["Reward16"][i]);
					data.Qualified17 = Convert.ToInt32(sheet["Qualified17"][i]);
					data.Reward17 = Convert.ToString(sheet["Reward17"][i]);
					data.Qualified18 = Convert.ToInt32(sheet["Qualified18"][i]);
					data.Reward18 = Convert.ToString(sheet["Reward18"][i]);
					data.Qualified19 = Convert.ToInt32(sheet["Qualified19"][i]);
					data.Reward19 = Convert.ToString(sheet["Reward19"][i]);
					data.Qualified20 = Convert.ToInt32(sheet["Qualified20"][i]);
					data.Reward20 = Convert.ToString(sheet["Reward20"][i]);
					data.Qualified21 = Convert.ToInt32(sheet["Qualified21"][i]);
					data.Reward21 = Convert.ToString(sheet["Reward21"][i]);
					data.Qualified22 = Convert.ToInt32(sheet["Qualified22"][i]);
					data.Reward22 = Convert.ToString(sheet["Reward22"][i]);
					data.Qualified23 = Convert.ToInt32(sheet["Qualified23"][i]);
					data.Reward23 = Convert.ToString(sheet["Reward23"][i]);
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
	
	
	private static void CreateConfigDataBase(List<ActivityConfigData> list)
	{
		string fileName = typeof(ActivityConfigData).Name;
		string path = System.IO.Path.Combine(ASSET_SHOP_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
			ActivityConfigDataBase database = (ActivityConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(ActivityConfigDataBase));
			
			if(null == database)
			{
				return;
			}
			
			database._dataTable = new ActivityConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			ActivityConfigDataBase database = ScriptableObject.CreateInstance<ActivityConfigDataBase>();
			
			database._dataTable = new ActivityConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}


