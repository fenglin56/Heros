using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class RobotConfigAssetPostProcessor : AssetPostprocessor
{
	private static readonly string RESOURCE_ROBOT_CONFIG_FOLDER = "Assets/Data/RobotConfig/Res";
	private static readonly string ASSET_ROBOT_CONFIG_FOLDER = "Assets/Data/RobotConfig/Data";
	
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
		string path = System.IO.Path.Combine(RESOURCE_ROBOT_CONFIG_FOLDER, "RobotConfig.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if (text == null)
		{
			Debug.LogError("Robot config file not exist");
			return;
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys = XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];
			
			List<RobotConfigData> tempList = new List<RobotConfigData>();
			
			for (int i = 0; i < levelIds.Length; i++)
			{
				if (0 == i || 1 == i) continue;
				RobotConfigData data = new RobotConfigData();

				data.RobotId = Convert.ToInt32(sheet["RobotId"][i]);
				data.RobotName = Convert.ToString(sheet["RobotName"][i]);
				data.RobotLevel = Convert.ToInt32(sheet["RobotLevel"][i]);
				data.RobotOccupation = Convert.ToInt32(sheet["RobotOccupation"][i]);
				data.RobotFashion = Convert.ToInt32(sheet["RobotFashion"][i]);
				data.RobotWeapon = Convert.ToInt32(sheet["RobotWeapon"][i]);
				data.RobotHat = Convert.ToInt32(sheet["RobotHat"][i]);
				data.RobotClothes = Convert.ToInt32(sheet["RobotClothes"][i]);
				data.RobotShoes = Convert.ToInt32(sheet["RobotShoes"][i]);
				data.RobotJewelry = Convert.ToInt32(sheet["RobotJewelry"][i]);
				data.RobotVipLevel = Convert.ToInt32(sheet["RobotVipLevel"][i]);
				data.RobotTitle = Convert.ToInt32(sheet["RobotTitle"][i]);


				tempList.Add(data);
			}
			
			CreateRobotConfigDataBase(tempList);
		}
	}
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach (string file in files)
		{
			if (file.Contains(RESOURCE_ROBOT_CONFIG_FOLDER))
			{
				fileModified = true;
				break;
			}
		}
		return fileModified;
	}
	
	
	private static void CreateRobotConfigDataBase(List<RobotConfigData> list)
	{
		string fileName = typeof(RobotConfigDataBase).Name;
		string path = System.IO.Path.Combine(ASSET_ROBOT_CONFIG_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			RobotConfigDataBase database = (RobotConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(RobotConfigDataBase));
			
			if (null == database)
			{
				return;
			}
			
			database._dataTable = new RobotConfigData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				
				database._dataTable[i] = list[i];
				
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			RobotConfigDataBase database = ScriptableObject.CreateInstance<RobotConfigDataBase>();
			
			database._dataTable = new RobotConfigData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
				
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
