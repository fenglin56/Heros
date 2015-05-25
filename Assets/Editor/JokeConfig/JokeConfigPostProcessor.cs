using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class JokeConfigPostProcessor : AssetPostprocessor
{
	public static readonly string RESOURCE_DATA_FOLDER = "Assets/Data/JokeData/Res";
	public static readonly string ASSET_DATA_FOLDER = "Assets/Data/JokeData/Data";
	
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
		{
			OnPostprocessEquipment();
		}
		
	}
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach (string file in files)
		{
			if (file.Contains(RESOURCE_DATA_FOLDER))
			{
				fileModified = true;
				break;
			}
		}
		return fileModified;
	}
	
	
	private static void OnPostprocessEquipment()
	{
		
		string path = System.IO.Path.Combine(RESOURCE_DATA_FOLDER, "JokeConfig.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if (text == null)
		{
			Debug.LogError("Equipment item file not exist");
			return;
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys = XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];
			
			List<JokeConfigData> tempList = new List<JokeConfigData>();
			
			for (int i = 2; i < levelIds.Length; i++)
			{
				//if (0 == i) continue;
				JokeConfigData data = new JokeConfigData();
				data.ID = Convert.ToInt32(sheet["ID"][i]);
				data.IDS = Convert.ToString(sheet["IDS"][i]);
				data.DelayTime = Convert.ToInt32(sheet["Time"][i]);
				tempList.Add(data);
			}
			
			
			CreateMedicamentConfigDataList(tempList);
		}
		
	}
	
	
	static void CreateMedicamentConfigDataList(List<JokeConfigData> list)
	{
		string fileName = typeof(JokeConfigData).Name + "DataBase";
		string path = System.IO.Path.Combine(ASSET_DATA_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			JokeConfigDataBase database = (JokeConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(JokeConfigDataBase));
			
			if (null == database)
			{
				return;
			}
			
			database.JokeDataList = new JokeConfigData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				database.JokeDataList[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			JokeConfigDataBase database = ScriptableObject.CreateInstance<JokeConfigDataBase>();
			database.JokeDataList = new JokeConfigData[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				database.JokeDataList[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
	}
}
