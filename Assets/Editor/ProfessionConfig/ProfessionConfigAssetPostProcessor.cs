using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class ProfessionConfig : AssetPostprocessor 
{
    private static readonly string RESOURCE_PROFESSION_CONFIG_FOLDER = "Assets/Data/ProfessionConfig/Res";
    private static readonly string ASSET_PROFESSION_CONFIG_FOLDER = "Assets/Data/ProfessionConfig/Data";
    private static readonly string ASSET_PLAYERICON_RES_CONFIG_FOLDER = "";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            string path = System.IO.Path.Combine(RESOURCE_PROFESSION_CONFIG_FOLDER, "ProfessionConfig.xml");
			TextReader tr = new StreamReader(path);
			string text = tr.ReadToEnd();
			
			if(text == null)
			{
				Debug.LogError("Portal config file not exist");
				return;	
			}
			else
			{
				XmlSpreadSheetReader.ReadSheet(text);
				XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
				string[] keys =  XmlSpreadSheetReader.Keys;
				
				object[] levelIds = sheet[keys[0]];

                List<ProfessionConfigData> tempList = new List<ProfessionConfigData>();

				for(int i = 0; i< levelIds.Length; i++ )
				{
                    if (0 == i || 1 == i) continue;
                    ProfessionConfigData data = new ProfessionConfigData();
                    data._professionID = Convert.ToInt32(sheet["ProfessionID"][i]);
                    data._professionName = Convert.ToString(sheet["ProfessionName"][i]);


                    string prefabpath = Convert.ToString(sheet["PlayerIcon"][i]);
                    string pathRes = System.IO.Path.Combine(ASSET_PLAYERICON_RES_CONFIG_FOLDER, prefabpath + ".prefab");
                    data._playerIcon = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

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
            if (file.Contains(RESOURCE_PROFESSION_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateConfigDataBase(List<ProfessionConfigData> list)
	{
        string fileName = typeof(ProfessionConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_PROFESSION_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            ProfessionConfigDataBase database = (ProfessionConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(ProfessionConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new ProfessionConfigData[list.Count];
            list.CopyTo(database._dataTable);
            //for(int i = 0; i < list.Count; i++)
            //{
            //    database._dataTable[i] = new ProfessionConfigData();
            //    database._dataTable[i]._professionID = list[i]._professionID;
            //    database._dataTable[i]._playerIcon = list[i]._playerIcon;                
            //}
			EditorUtility.SetDirty(database);
		}
		else
		{
            ProfessionConfigDataBase database = ScriptableObject.CreateInstance<ProfessionConfigDataBase>();

            database._dataTable = new ProfessionConfigData[list.Count];
            list.CopyTo(database._dataTable);
            //for(int i = 0; i < list.Count; i++)
            //{
            //    database._dataTable[i] = new ProfessionConfigData();
            //    database._dataTable[i]._playerIcon = list[i]._playerIcon;    		
            //}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
