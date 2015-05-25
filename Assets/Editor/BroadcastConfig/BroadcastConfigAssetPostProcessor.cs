using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class BroadcastConfigAssetPostProcessor : AssetPostprocessor
{
	private static readonly string RESOURCE_Broadcast_CONFIG_FOLDER = "Assets/Data/BroadcastConfig/Res";
	private static readonly string ASSET_Broadcast_CONFIG_FOLDER = "Assets/Data/BroadcastConfig/Data";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
		{
			RobotConfigPostprocess();
		}
	}
	
	private static void RobotConfigPostprocess()
	{
		string path = System.IO.Path.Combine(RESOURCE_Broadcast_CONFIG_FOLDER, "BroadcastConfig.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if (text == null)
		{
			Debug.LogError("Broadcast Config file not exist");
			return;
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys = XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];
			
			List<BroadcastConfigData> tempList = new List<BroadcastConfigData>();
			
			for (int i = 0; i < levelIds.Length; i++)
			{
				if (0 == i || 1 == i) continue;
				BroadcastConfigData data = new BroadcastConfigData();
				data.BroadcastId = Convert.ToInt32(sheet["BroadcastId"][i]);
				data.BroadcastType = Convert.ToInt32(sheet["BroadcastType"][i]);
				data.BroadcastConditions = Convert.ToInt32(sheet["BroadcastConditions"][i]);
				data.BroadcastContent = Convert.ToString(sheet["BroadcastContent"][i]);				
				
				tempList.Add(data);
			}
			
			CreateBroadcastConfigDataBase(tempList);
		}
	}
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach (string file in files)
		{
			if (file.Contains(RESOURCE_Broadcast_CONFIG_FOLDER))
			{
				fileModified = true;
				break;
			}
		}
		return fileModified;
	}
	
	
	private static void CreateBroadcastConfigDataBase(List<BroadcastConfigData> list)
	{
		string fileName = typeof(BroadcastConfigDataBase).Name;
		string path = System.IO.Path.Combine(ASSET_Broadcast_CONFIG_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			BroadcastConfigDataBase database = (BroadcastConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(BroadcastConfigDataBase));
			
			if (null == database)
			{
				return;
			}	

			database._dataTable = new BroadcastConfigData[list.Count];

			for (int i = 0; i < list.Count; i++)
			{
				
				database._dataTable[i] = list[i];
				
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			BroadcastConfigDataBase database = ScriptableObject.CreateInstance<BroadcastConfigDataBase>();
			
			database._dataTable = new BroadcastConfigData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
				
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
