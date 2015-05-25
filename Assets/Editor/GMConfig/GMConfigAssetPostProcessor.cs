using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class GMConfigAssetPostProcessor : AssetPostprocessor {

	public static readonly string RESOURCE_GM_CONFIG_FOLDER = "Assets/Data/GMConfig/Res";
	public static readonly string ASSET_GM_CONFIG_FOLDER = "Assets/Data/GMConfig/Data";
	
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
			string path = System.IO.Path.Combine( RESOURCE_GM_CONFIG_FOLDER, "GMCmdConfig.xml" );
			
			TextReader tr = new StreamReader(path);
			
			string text = tr.ReadToEnd();
			
			if(text == null)
			{
				Debug.LogError("GM config file not exist");
				return;	
			}
			else
			{
				XmlSpreadSheetReader.ReadSheet(text);
				
				XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
				
				string[] keys =  XmlSpreadSheetReader.Keys;
				
				object[] levelIds = sheet[keys[0]];
				
				List<GMConfigData> tempList = new List<GMConfigData>();
				
				for(int i = 2; i< levelIds.Length; i++ )
				{
					GMConfigData data = new GMConfigData();
					data.m_gmType = Convert.ToInt32(sheet["GMType"][i]);
					data.m_name = sheet["Name"][i].ToString();
					data.m_desc = sheet["FunctionDesc"][i].ToString();
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
			if(file.Contains(RESOURCE_GM_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}
	
	
	private static void CreatePlayerLevelConfigDataBase( List<GMConfigData> list)
	{
		string className = typeof(GMConfigDataBase).Name;
		string path = System.IO.Path.Combine(ASSET_GM_CONFIG_FOLDER, className + ".asset");
		
		if(File.Exists(path))
		{
			GMConfigDataBase database = (GMConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(GMConfigDataBase));
			
			if(null == database)
			{
				return;
			}
			
			database.m_dataTable = new GMConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database.m_dataTable[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			GMConfigDataBase database = ScriptableObject.CreateInstance<GMConfigDataBase>();
			
			database.m_dataTable = new GMConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database.m_dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
			
			
		}
		
	}
}
