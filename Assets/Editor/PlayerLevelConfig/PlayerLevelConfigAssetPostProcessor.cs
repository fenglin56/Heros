using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class PlayerLevelConfig : AssetPostprocessor 
{
	public static readonly string EW_RESOURCE_PLAYER_LEVEL_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Res";
	public static readonly string EW_ASSET_PLAYER_LEVEL_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Data";
	
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
			string path = System.IO.Path.Combine( EW_RESOURCE_PLAYER_LEVEL_CONFIG_FOLDER, "PlayerLevelConfig.xml" );
			
			TextReader tr = new StreamReader(path);
			
			string text = tr.ReadToEnd();
			
			if(text == null)
			{
				Debug.LogError("Player level config file not exist");
				return;	
			}
			else
			{
				XmlSpreadSheetReader.ReadSheet(text);
				
				XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
				
				string[] keys =  XmlSpreadSheetReader.Keys;
				
				
				object[] levelIds = sheet[keys[0]];
				
				List<PlayerLevelConfigData> tempList = new List<PlayerLevelConfigData>();
				
				for(int i = 0; i< levelIds.Length; i++ )
				{
					PlayerLevelConfigData data = new PlayerLevelConfigData();
					data._hp =  Convert.ToInt32( sheet["hp"][i] );
					data._level = Convert.ToInt32( sheet["level"][i] );
					data._xp = Convert.ToInt32(sheet["xp"][i]);
					data._skillPoint = Convert.ToInt32(sheet["skillpoint"][i]);
					data._reviveHcCost = Convert.ToInt32(sheet["revive_hc"][i]);
					
					tempList.Add(data);
				}
				
				CreatePlayerLevelConfigDataBase(tempList);
			}
		}
		
	}
	
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
			if(file.Contains(EW_RESOURCE_PLAYER_LEVEL_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}
	
	
	private static void CreatePlayerLevelConfigDataBase( List<PlayerLevelConfigData> list)
	{
		string className = typeof(PlayerLevelConfigDataBase).Name;
		string path = System.IO.Path.Combine(EW_ASSET_PLAYER_LEVEL_CONFIG_FOLDER, className + ".asset");
		
		if(File.Exists(path))
		{
			PlayerLevelConfigDataBase database = (PlayerLevelConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerLevelConfigDataBase));
			
			if(null == database)
			{
				return;
			}
			
			database._dataTable = new PlayerLevelConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = new PlayerLevelConfigData();
				database._dataTable[i]._level = list[i]._level;
				database._dataTable[i]._hp = list[i]._hp;
				database._dataTable[i]._xp = list[i]._xp;
				database._dataTable[i]._skillPoint = list[i]._skillPoint;
				database._dataTable[i]._reviveHcCost = list[i]._reviveHcCost;
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			PlayerLevelConfigDataBase database = ScriptableObject.CreateInstance<PlayerLevelConfigDataBase>();
			
			database._dataTable = new PlayerLevelConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = new PlayerLevelConfigData();
				database._dataTable[i]._level = list[i]._level;
				database._dataTable[i]._hp = list[i]._hp;
				database._dataTable[i]._xp = list[i]._xp;
				database._dataTable[i]._skillPoint = list[i]._skillPoint;
				database._dataTable[i]._reviveHcCost = list[i]._reviveHcCost;
			}
			AssetDatabase.CreateAsset(database, path);
			
			
		}
		
	}
}
