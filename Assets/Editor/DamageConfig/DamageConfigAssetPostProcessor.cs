using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class DamageConfig : AssetPostprocessor 
{
    private static readonly string RESOURCE_TRAP_CONFIG_FOLDER = "Assets/Data/DamageConfig/Res";
    private static readonly string ASSET_TRAP_CONFIG_FOLDER = "Assets/Data/DamageConfig/Data";
    private static readonly string ASSET_TRAP_RES_CONFIG_FOLDER = "Assets/MapItems/Damages/Prefab";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            string path = System.IO.Path.Combine(RESOURCE_TRAP_CONFIG_FOLDER, "BoxConfig.xml");
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

                List<DamageConfigData> tempList = new List<DamageConfigData>();

				for(int i = 0; i< levelIds.Length; i++ )
				{
                    if (0 == i || 1 == i) continue;
                    DamageConfigData data = new DamageConfigData();
                    data._damageID = Convert.ToInt32(sheet["BoxID"][i]);
                    data._damageName = Convert.ToString(sheet["BoxName"][i]);
                    data._boxType = Convert.ToInt32(sheet["BoxType"][i]);

                    string prefabName = Convert.ToString(sheet["ResID"][i]);
                    string prefabPath = System.IO.Path.Combine(ASSET_TRAP_RES_CONFIG_FOLDER, prefabName + ".prefab");
                    data._damagePrefab= (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    string correspondingItemIDStr = Convert.ToString(sheet["BoxGoodsDrop"][i]);
                    string[] splitCorrespondingItemIDStrs = correspondingItemIDStr.Split("+".ToCharArray());                    
                    data._correspondingItemID = Convert.ToInt32(splitCorrespondingItemIDStrs[0]);

					tempList.Add(data);
				}

                CreateSceneConfigDataBase(tempList);
			}
		}
	}
	
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
            if (file.Contains(RESOURCE_TRAP_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateSceneConfigDataBase(List<DamageConfigData> list)
	{
        string fileName = typeof(DamageConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_TRAP_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            DamageConfigDataBase database = (DamageConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(DamageConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new DamageConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new DamageConfigData();
                database._dataTable[i] = list[i];
                //database._dataTable[i]._damageID = list[i]._damageID;
                //database._dataTable[i]._damageName = list[i]._damageName;
                //database._dataTable[i]._damagePrefab = list[i]._damagePrefab;
                //database._dataTable[i]._correspondingItemID = list[i]._correspondingItemID;
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            DamageConfigDataBase database = ScriptableObject.CreateInstance<DamageConfigDataBase>();

            database._dataTable = new DamageConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new DamageConfigData();
                database._dataTable[i] = list[i];
                //database._dataTable[i]._damageID = list[i]._damageID;
                //database._dataTable[i]._damageName = list[i]._damageName;
                //database._dataTable[i]._damagePrefab = list[i]._damagePrefab;
                //database._dataTable[i]._correspondingItemID = list[i]._correspondingItemID;
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
