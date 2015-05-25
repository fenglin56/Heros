using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class BuffConfig : AssetPostprocessor 
{
    private static readonly string RESOURCE_BUFF_CONFIG_FOLDER = "Assets/Data/BuffConfig/Res";
    private static readonly string ASSET_BUFF_CONFIG_FOLDER = "Assets/Data/BuffConfig/Data";
    private static readonly string ASSET_BUFF_RES_CONFIG_FOLDER = "Assets/Effects/Buff";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            string path = System.IO.Path.Combine(RESOURCE_BUFF_CONFIG_FOLDER, "BUFF.xml");
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

                List<BuffConfigData> tempList = new List<BuffConfigData>();

				for(int i = 0; i< levelIds.Length; i++ )
				{
                    if (0 == i || 1 == i) continue;
                    BuffConfigData data = new BuffConfigData();
                    data._buffID = Convert.ToInt32(sheet["buffid"][i]);
                    data._iconID = Convert.ToString(sheet["IconID"][i]);
                    //data._buffLv = Convert.ToInt32(sheet["bufflevel"][i]);
                    //data._durativeTime = Convert.ToSingle(sheet["DurativeNum"]);
                    data._buffName = Convert.ToString(sheet["buffname"][i]);
                    data._buffEffMount = Convert.ToString(sheet["BuffEffMount"][i]);

                    string prefabpath = Convert.ToString(sheet["BuffEffect"][i]);
                    string pathRes = System.IO.Path.Combine(ASSET_BUFF_RES_CONFIG_FOLDER, prefabpath + ".prefab");
                    data._buffEffect= (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

					data._buffSound = Convert.ToString(sheet["BuffSound"][i]);

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
            if (file.Contains(RESOURCE_BUFF_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateConfigDataBase(List<BuffConfigData> list)
	{
        string fileName = typeof(BuffConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_BUFF_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            BuffConfigDataBase database = (BuffConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(BuffConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new BuffConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new BuffConfigData();
                database._dataTable[i]._buffID = list[i]._buffID;
                database._dataTable[i]._iconID = list[i]._iconID;
                //database._dataTable[i]._buffLv = list[i]._buffLv;
                //database._dataTable[i]._durativeTime = list[i]._durativeTime;
                database._dataTable[i]._buffName = list[i]._buffName;
                database._dataTable[i]._buffEffect= list[i]._buffEffectPrefab;
                database._dataTable[i]._buffEffMount = list[i]._buffEffMount;
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            BuffConfigDataBase database = ScriptableObject.CreateInstance<BuffConfigDataBase>();

            database._dataTable = new BuffConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new BuffConfigData();
                database._dataTable[i]._buffID = list[i]._buffID;
                database._dataTable[i]._iconID = list[i]._iconID;
                //database._dataTable[i]._buffLv = list[i]._buffLv;
                //database._dataTable[i]._durativeTime = list[i]._durativeTime;
                database._dataTable[i]._buffName = list[i]._buffName;
                database._dataTable[i]._buffEffect= list[i]._buffEffectPrefab;
                database._dataTable[i]._buffEffMount = list[i]._buffEffMount;
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
