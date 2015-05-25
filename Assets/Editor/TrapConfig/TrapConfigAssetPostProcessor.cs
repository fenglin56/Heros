using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class TrapConfig : AssetPostprocessor 
{
    private static readonly string RESOURCE_TRAP_CONFIG_FOLDER = "Assets/Data/TrapConfig/Res";
    private static readonly string ASSET_TRAP_CONFIG_FOLDER = "Assets/Data/TrapConfig/Data";
    private static readonly string ASSET_TRAP_RES_CONFIG_FOLDER = "Assets/MapItems/Traps/Pitfall/Prefab";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            string path = System.IO.Path.Combine(RESOURCE_TRAP_CONFIG_FOLDER, "TrapConfig.xml");
			TextReader tr = new StreamReader(path);
			string text = tr.ReadToEnd();
			
			if(text == null)
			{
				Debug.LogError("Trap config file not exist");
				return;	
			}
			else
			{
				XmlSpreadSheetReader.ReadSheet(text);
				XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
				string[] keys =  XmlSpreadSheetReader.Keys;
				
				object[] levelIds = sheet[keys[0]];

                List<TrapConfigData> tempList = new List<TrapConfigData>();

				for(int i = 0; i< levelIds.Length; i++ )
				{
                    if (0 == i || 1 == i) continue;
                    TrapConfigData data = new TrapConfigData();
                    data._TrapID = Convert.ToInt32(sheet["TrapID"][i]);
                    data._szName = Convert.ToString(sheet["TrapName"][i]);

                    string prefabpath = Convert.ToString(sheet["TrapObjPath"][i]);
                    string pathRes = System.IO.Path.Combine(ASSET_TRAP_RES_CONFIG_FOLDER, prefabpath + ".prefab");
                    data._TrapPrefab= (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

					tempList.Add(data);
				}

                CreateTrapConfigDataBase(tempList);
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


    private static void CreateTrapConfigDataBase(List<TrapConfigData> list)
	{
        string fileName = typeof(TrapConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_TRAP_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            TrapConfigDataBase database = (TrapConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(TrapConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new TrapConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new TrapConfigData();
                database._dataTable[i]._TrapID = list[i]._TrapID;
                database._dataTable[i]._szName = list[i]._szName;
                database._dataTable[i]._TrapPrefab = list[i].TrapPrefab;
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            TrapConfigDataBase database = ScriptableObject.CreateInstance<TrapConfigDataBase>();

            database._dataTable = new TrapConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new TrapConfigData();
                database._dataTable[i]._TrapID = list[i]._TrapID;
                database._dataTable[i]._szName = list[i]._szName;
                database._dataTable[i]._TrapPrefab = list[i].TrapPrefab;
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
