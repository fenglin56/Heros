using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class EndLessEctypeConfigAssetPostProcessor : AssetPostprocessor 
{
	private static readonly string RESOURCE_SHOP_CONFIG_FOLDER = "Assets/Data/EndlessEctypeConfig/Res";
	private static readonly string ASSET_SHOP_CONFIG_FOLDER = "Assets/Data/EndlessEctypeConfig/Data";
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
			string path = System.IO.Path.Combine(RESOURCE_SHOP_CONFIG_FOLDER, "EndlessEctypeReward.xml");
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
				
				List<EndLessEctypeConfigData> tempList = new List<EndLessEctypeConfigData>();
				
				for(int i = 2; i< levelIds.Length; i++ )
				{
					EndLessEctypeConfigData data = new EndLessEctypeConfigData();
					data.dwEctypeContainerId = Convert.ToInt32(sheet["dwEctypeContainerId"][i]);
					data.WaveIndex = Convert.ToInt32(sheet["WaveIndex"][i]);
					data.Reward = Convert.ToString(sheet["Reward"][i]);
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
	
	
	private static void CreateConfigDataBase(List<EndLessEctypeConfigData> list)
	{
		string fileName = typeof(EndLessEctypeConfigDataBase).Name;
		string path = System.IO.Path.Combine(ASSET_SHOP_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
			EndLessEctypeConfigDataBase database = (EndLessEctypeConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(EndLessEctypeConfigDataBase));
			
			if(null == database)
			{
				return;
			}
			
			database._dataTable = new EndLessEctypeConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			EndLessEctypeConfigDataBase database = ScriptableObject.CreateInstance<EndLessEctypeConfigDataBase>();
			
			database._dataTable = new EndLessEctypeConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
