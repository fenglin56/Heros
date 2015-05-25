using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class TownRobotPosAssetPostProcessor : AssetPostprocessor
{
	private static readonly string RESOURCE_TOWN_ROBOT_POS_FOLDER = "Assets/Data/RobotConfig/Res";
	private static readonly string ASSET_TOWN_ROBOT_POS_FOLDER = "Assets/Data/RobotConfig/Data";
	
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
		string path = System.IO.Path.Combine(RESOURCE_TOWN_ROBOT_POS_FOLDER, "TownRobotPos.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if (text == null)
		{
			Debug.LogError("TownRobotPos file not exist");
			return;
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys = XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];
			
			List<TownRobotPosData> tempList = new List<TownRobotPosData>();
			
			for (int i = 0; i < levelIds.Length; i++)
			{
				if (0 == i || 1 == i) continue;
				TownRobotPosData data = new TownRobotPosData();
				data.PosId = Convert.ToInt32(sheet["PosId"][i]);
				string[] posStrArray = Convert.ToString(sheet["BornPos"][i]).Split('+');
				data.BornPos = new Vector3(Convert.ToInt32(posStrArray[0]),0,Convert.ToInt32(posStrArray[1]));
				data.BornOrientation = Convert.ToSingle(sheet["BornOrientation"][i]);

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
			if (file.Contains(RESOURCE_TOWN_ROBOT_POS_FOLDER))
			{
				fileModified = true;
				break;
			}
		}
		return fileModified;
	}
	
	
	private static void CreateRobotConfigDataBase(List<TownRobotPosData> list)
	{
		string fileName = typeof(TownRobotPosDataBase).Name;
		string path = System.IO.Path.Combine(ASSET_TOWN_ROBOT_POS_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			TownRobotPosDataBase database = (TownRobotPosDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(TownRobotPosDataBase));
			
			if (null == database)
			{
				return;
			}
			
			database._dataTable = new TownRobotPosData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				
				database._dataTable[i] = list[i];
				
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			TownRobotPosDataBase database = ScriptableObject.CreateInstance<TownRobotPosDataBase>();
			
			database._dataTable = new TownRobotPosData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
				
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
