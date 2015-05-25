using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class LocalServerListPostProcessor : AssetPostprocessor
{

	public static readonly string RESOURCE_DATA_FOLDER = "Assets/Data/LocalServerListConfig/Res";
	public static readonly string ASSET_DATA_FOLDER = "Assets/Data/LocalServerListConfig/Data";

	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
		{
			OnPostprocessServerList();
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

	private static void OnPostprocessServerList()
	{
		
		string path = System.IO.Path.Combine(RESOURCE_DATA_FOLDER, "LocalServerListConfig.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if (text == null)
		{
			Debug.LogError("LocalServerListCofing  file not exist");
			return;
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys = XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];
			
			List<Server> tempList = new List<Server>();
			
			for (int i = 2; i < levelIds.Length; i++)
			{
				//if (0 == i) continue;
				Server data = new Server();
				data.No = Convert.ToInt32(sheet["No"][i]);
				data.Name = Convert.ToString(sheet["Name"][i]);
				data.IP = Convert.ToString(sheet["IP"][i]);
				data.Port = Convert.ToInt32(sheet["Port"][i]);
				data.ActorNumber = Convert.ToInt32(sheet["ActorNumber"][i]);
				data.Status = Convert.ToInt32(sheet["Status"][i]);
				data.Recommend_status = Convert.ToInt32(sheet["Recommend_status"][i]);

				tempList.Add(data);
			}
			
			
			CreateServerListConfigDataList(tempList);
		}
		
	}
	
	
	static void CreateServerListConfigDataList(List<Server> list)
	{
		string fileName = typeof(LocalServerListConfigDatabase).Name;
		string path = System.IO.Path.Combine(ASSET_DATA_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			LocalServerListConfigDatabase database = (LocalServerListConfigDatabase)AssetDatabase.LoadAssetAtPath(path, typeof(LocalServerListConfigDatabase));
			
			if (null == database)
			{
				return;
			}
			
			database.LocalServerList = new Server[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				database.LocalServerList[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			LocalServerListConfigDatabase database = ScriptableObject.CreateInstance<LocalServerListConfigDatabase>();
			database.LocalServerList = new Server[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				database.LocalServerList[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
	}
}
