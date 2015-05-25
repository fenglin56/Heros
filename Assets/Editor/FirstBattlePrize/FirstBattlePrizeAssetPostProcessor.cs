using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class FirstBattlePrizeAssetPostProcessor : AssetPostprocessor 
{
	private static readonly string RESOURCE_FIRST_BATTLE_PRIZE_FOLDER = "Assets/Data/EctypeConfig/Res";
	private static readonly string ASSET_FIRST_BATTLE_PRIZE_FOLDER = "Assets/Data/EctypeConfig/Data";
	
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
		string path = System.IO.Path.Combine(RESOURCE_FIRST_BATTLE_PRIZE_FOLDER, "EctypeFirstBattlePrize.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if (text == null)
		{
			Debug.LogError("EctypeFirstBattlePrize file not exist");
			return;
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys = XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];
			
			List<FirstBattlePrizeData> tempList = new List<FirstBattlePrizeData>();
			
			for (int i = 0; i < levelIds.Length; i++)
			{
				if (0 == i || 1 == i) continue;
				FirstBattlePrizeData data = new FirstBattlePrizeData();
				data.UserLevel = Convert.ToInt32(sheet["UserLevel"][i]);
				string[] mondayStr = Convert.ToString(sheet["Monday"][i]).Split('+');
				data.Monday = new int[mondayStr.Length];
				data.Monday[0] = Convert.ToInt32(mondayStr[0]);
				data.Monday[1] = Convert.ToInt32(mondayStr[1]);
				string[] tuesdayStr = Convert.ToString(sheet["Tuesday"][i]).Split('+');
				data.Tuesday = new int[tuesdayStr.Length];
				data.Tuesday[0] = Convert.ToInt32(tuesdayStr[0]);
				data.Tuesday[1] = Convert.ToInt32(tuesdayStr[1]);
				string[] wednesdayStr = Convert.ToString(sheet["Wednesday"][i]).Split('+');
				data.Wednesday = new int[wednesdayStr.Length];
				data.Wednesday[0] = Convert.ToInt32(wednesdayStr[0]);
				data.Wednesday[1] = Convert.ToInt32(wednesdayStr[1]);
				string[] thursdayStr = Convert.ToString(sheet["Thursday"][i]).Split('+');
				data.Thursday = new int[thursdayStr.Length];
				data.Thursday[0] = Convert.ToInt32(thursdayStr[0]);
				data.Thursday[1] = Convert.ToInt32(thursdayStr[1]);
				string[] fridayStr = Convert.ToString(sheet["Friday"][i]).Split('+');
				data.Friday = new int[fridayStr.Length];
				data.Friday[0] = Convert.ToInt32(fridayStr[0]);
				data.Friday[1] = Convert.ToInt32(fridayStr[1]);
				string[] saturdayStr = Convert.ToString(sheet["Saturday"][i]).Split('+');
				data.Saturday = new int[saturdayStr.Length];
				data.Saturday[0] = Convert.ToInt32(saturdayStr[0]);
				data.Saturday[1] = Convert.ToInt32(saturdayStr[1]);
				string[] sundayStr = Convert.ToString(sheet["Sunday"][i]).Split('+');
				data.Sunday = new int[sundayStr.Length];
				data.Sunday[0] = Convert.ToInt32(sundayStr[0]);
				data.Sunday[1] = Convert.ToInt32(sundayStr[1]);

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
			if (file.Contains(RESOURCE_FIRST_BATTLE_PRIZE_FOLDER))
			{
				fileModified = true;
				break;
			}
		}
		return fileModified;
	}
	
	
	private static void CreateRobotConfigDataBase(List<FirstBattlePrizeData> list)
	{
		string fileName = typeof(FirstBattlePrizeDataBase).Name;
		string path = System.IO.Path.Combine(ASSET_FIRST_BATTLE_PRIZE_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			FirstBattlePrizeDataBase database = (FirstBattlePrizeDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(FirstBattlePrizeDataBase));
			
			if (null == database)
			{
				return;
			}
			
			database._dataTable = new FirstBattlePrizeData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				
				database._dataTable[i] = list[i];
				
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			FirstBattlePrizeDataBase database = ScriptableObject.CreateInstance<FirstBattlePrizeDataBase>();
			
			database._dataTable = new FirstBattlePrizeData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
				
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
