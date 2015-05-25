using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class PortalConfig : AssetPostprocessor 
{
    private static readonly string RESOURCE_PORTAL_CONFIG_FOLDER = "Assets/Data/PortalConfig/Res";
    private static readonly string ASSET_PORTAL_CONFIG_FOLDER = "Assets/Data/PortalConfig/Data";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            string path = System.IO.Path.Combine(RESOURCE_PORTAL_CONFIG_FOLDER, "MapChunnel.xml");
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

                List<PortalConfigData> tempList = new List<PortalConfigData>();

				for(int i = 0; i< levelIds.Length; i++ )
				{
                    if (0 == i || 1 == i) continue;
                    PortalConfigData data = new PortalConfigData();
                    data._SID = Convert.ToInt32(sheet["sid"][i]);
                    data._portalType = Convert.ToInt32(sheet["type"][i]);
                    string[] mapID = Convert.ToString(sheet["destmapid"][i]).Split('+');
                    data._desMapID = new int[mapID.Length];
                    for (int j = 0; j < mapID.Length; ++j)
                    {
                        data._desMapID[j] = int.Parse(mapID[j]);
                    }

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
            if (file.Contains(RESOURCE_PORTAL_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateConfigDataBase(List<PortalConfigData> list)
	{
        string fileName = typeof(PortalConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_PORTAL_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            PortalConfigDataBase database = (PortalConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PortalConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new PortalConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new PortalConfigData();
                database._dataTable[i]._SID = list[i]._SID;
                database._dataTable[i]._portalType = list[i]._portalType;
                database._dataTable[i]._desMapID = list[i]._desMapID;
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            PortalConfigDataBase database = ScriptableObject.CreateInstance<PortalConfigDataBase>();

            database._dataTable = new PortalConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new PortalConfigData();
                database._dataTable[i]._SID = list[i]._SID;
                database._dataTable[i]._portalType = list[i]._portalType;
                database._dataTable[i]._desMapID = list[i]._desMapID;
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
