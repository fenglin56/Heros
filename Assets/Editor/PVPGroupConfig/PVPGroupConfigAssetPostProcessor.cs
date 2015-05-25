using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class PVPGroupConfigAssetPostProcessor: AssetPostprocessor
{

	private static readonly string RESOURCE_PVP_CONFIG_FOLDER = "Assets/Data/PVPGroupConfig/Res";
	private static readonly string ASSET_PVP_CONFIG_FOLDER = "Assets/Data/PVPGroupConfig/Data";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
		
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
			OnPostprocessPVPGroupList();
		}
	}
	
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
			if (file.Contains(RESOURCE_PVP_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}

	private static void OnPostprocessPVPGroupList()
	{
		string path = System.IO.Path.Combine(RESOURCE_PVP_CONFIG_FOLDER, "PVPGroupConfig.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if (text == null)
		{
			Debug.LogError("pvpgroup config file not exist");
			return;
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys = XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];
			
			List<PVPGroupConfig> tempList = new List<PVPGroupConfig>();
			
			for (int i = 2; i < levelIds.Length; i++)
			{
				PVPGroupConfig data = new PVPGroupConfig();
				data.PVPGroupID = Convert.ToInt32(sheet["PVPGroupID"][i]);
				data.PVPGroupName = Convert.ToString(sheet["PVPGroupName"][i]);
				data.GroupLevelUp_Score = Convert.ToString(sheet["GroupLevelUp_Score"][i]);
				data.GroupLevelUp_Rank = Convert.ToInt32(sheet["GroupLevelUp_Rank"][i]);
				data.GroupLevelUpIDS = Convert.ToString(sheet["GroupLevelUpIDS"][i]);
				data.PVPGroupIcon = Convert.ToString(sheet["PVPGroupIcon"][i]);

				tempList.Add(data);
			}

			CreateConfigDataBase(tempList);
		}
	}


	
	private static void CreateConfigDataBase(List<PVPGroupConfig> list)
	{
		String fileName = typeof(PVPGroupConfig).Name + "DataBase";
		string path = System.IO.Path.Combine(ASSET_PVP_CONFIG_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			PVPGroupConfigDataBase database = (PVPGroupConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PVPGroupConfigDataBase));
			
			if (null == database)
			{
				return;
			}
			
			database._dataTable = new PVPGroupConfig[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			PVPGroupConfigDataBase database = ScriptableObject.CreateInstance<PVPGroupConfigDataBase>();
			database._dataTable = new PVPGroupConfig[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
