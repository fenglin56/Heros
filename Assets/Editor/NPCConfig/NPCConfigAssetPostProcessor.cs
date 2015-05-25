using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class NPCConfig : AssetPostprocessor 
{
	private static readonly string RESOURCE_NPC_CONFIG_FOLDER = "Assets/Data/NPCConfig/Res";
    private static readonly string ASSET_NPC_CONFIG_FOLDER = "Assets/Data/NPCConfig/Data";
    private static readonly string ASSET_NPC_RES_CONFIG_FOLDER = "Assets/Prefab/NPC/Prefab";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            NPCConfigPostprocess();
            NPCSpecialPostprocess();
            NPCTalkPostprocess();
		}
	}
	
	private static void NPCConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_NPC_CONFIG_FOLDER, "NPCConfig.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
			
		if(text == null)
		{
			Debug.LogError("npc config file not exist");
			return;	
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys =  XmlSpreadSheetReader.Keys;
				
			object[] levelIds = sheet[keys[0]];

            List<NPCConfigData> tempList = new List<NPCConfigData>();

			for(int i = 0; i< levelIds.Length; i++ )
			{
                if (0 == i || 1 == i) continue;
                NPCConfigData data = new NPCConfigData();
                data._NPCID = Convert.ToInt32(sheet["NpcID"][i]);
                data._szName = Convert.ToString(sheet["NpcName"][i]);
                data._npcTitle = Convert.ToString(sheet["NpcTitle"][i]);
                string PrefabName = Convert.ToString(sheet["PortraitID"][i]);
                string PrefabPath=System.IO.Path.Combine("Assets/Prefab/GUI/IconPrefab/StroyPersonHead", PrefabName + ".prefab");
                data.PortraitID=(GameObject)AssetDatabase.LoadAssetAtPath(PrefabPath, typeof(GameObject));
				string cameraOffsetStr = Convert.ToString(sheet["CameraOffset"][i]);
				string[] splitStrs = cameraOffsetStr.Split('|');
				data.CameraOffset = new Vector3(Convert.ToSingle(splitStrs[0]), Convert.ToSingle(splitStrs[1]),Convert.ToSingle(splitStrs[2]));
                string prefabpath = Convert.ToString(sheet["NpcPrefabPath"][i]);
                string pathRes = System.IO.Path.Combine(ASSET_NPC_RES_CONFIG_FOLDER, prefabpath + ".prefab");
                data._NPCPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

				tempList.Add(data);
			}

            CreateNPCConfigDataBase(tempList);
		}
    }

    private static void NPCSpecialPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_NPC_CONFIG_FOLDER, "NPCSpecial.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("Special config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<NPCSpecialConfigData> tempList = new List<NPCSpecialConfigData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                NPCSpecialConfigData data = new NPCSpecialConfigData();
                data._NPCID = Convert.ToInt32(sheet["NpcID"][i]);
                data._FunctionDesc = Convert.ToString(sheet["FunctionDesc"][i]);
                data._FunctionType = Convert.ToInt32(sheet["FunctionType"][i]);
                data.Parameters = Convert.ToInt32(sheet["Parameters"][i]);
                data.ShopIcon = Convert.ToString(sheet["ShopIcon"][i]);
                data.ShopTitle = Convert.ToString(sheet["ShopTitle"][i]);
                //string prefabpath = Convert.ToString(sheet["NpcPrefabPath"][i]);
                //string pathRes = System.IO.Path.Combine(ASSET_ECTYPE_RES_CONFIG_FOLDER, prefabpath + ".prefab");
                //data._NPCPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

                tempList.Add(data);
            }

            CreateNPCSpecialConfigDataBase(tempList);
        }
    }

    private static void NPCTalkPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_NPC_CONFIG_FOLDER, "ChitTalk.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("chitTalk config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<NPCTalkConfigData> tempList = new List<NPCTalkConfigData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                NPCTalkConfigData data = new NPCTalkConfigData();
                data._SID = Convert.ToInt32(sheet["SID"][i]);
                data._szTalk = Convert.ToString(sheet["szTalk"][i]);
                data._szVoice = Convert.ToString(sheet["szVoice"][i]);

                //string prefabpath = Convert.ToString(sheet["prefabPath"][i]);
                //string pathRes = System.IO.Path.Combine(ASSET_ECTYPE_RES_CONFIG_FOLDER, prefabpath + ".prefab");
                //data._prefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

                tempList.Add(data);
            }

            CreateTalkConfigDataBase(tempList);
        }
    }

	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
            if (file.Contains(RESOURCE_NPC_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateNPCConfigDataBase(List<NPCConfigData> list)
	{
        string fileName = typeof(NPCConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_NPC_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            NPCConfigDataBase database = (NPCConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(NPCConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new NPCConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            NPCConfigDataBase database = ScriptableObject.CreateInstance<NPCConfigDataBase>();

            database._dataTable = new NPCConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}

    private static void CreateNPCSpecialConfigDataBase(List<NPCSpecialConfigData> list)
    {
        string fileName = typeof(NPCSpecialConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_NPC_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            NPCSpecialConfigDataBase database = (NPCSpecialConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(NPCSpecialConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new NPCSpecialConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            NPCSpecialConfigDataBase database = ScriptableObject.CreateInstance<NPCSpecialConfigDataBase>();

            database._dataTable = new NPCSpecialConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

    private static void CreateTalkConfigDataBase(List<NPCTalkConfigData> list)
    {
        string fileName = typeof(NPCTalkConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_NPC_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            NPCTalkConfigDataBase database = (NPCTalkConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(NPCTalkConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new NPCTalkConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            NPCTalkConfigDataBase database = ScriptableObject.CreateInstance<NPCTalkConfigDataBase>();

            database._dataTable = new NPCTalkConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }

    }

}
