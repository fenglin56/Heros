using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class SceneConfig : AssetPostprocessor 
{
	public static readonly string RESOURCE_SCENE_CONFIG_FOLDER = "Assets/Data/SceneConfig/Res";
    public static readonly string ASSET_SCENE_CONFIG_FOLDER = "Assets/Data/SceneConfig/Data";
    public static readonly string MapIconPrefabPath = "Assets/Prefab/GUI/BattleIcon";
	
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            string path = System.IO.Path.Combine(RESOURCE_SCENE_CONFIG_FOLDER, "MapInfo.xml");
			TextReader tr = new StreamReader(path);
			string text = tr.ReadToEnd();
			
			if(text == null)
			{
				Debug.LogError("Scene config file not exist");
				return;	
			}
			else
			{
				XmlSpreadSheetReader.ReadSheet(text);
				XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
				string[] keys =  XmlSpreadSheetReader.Keys;
				
				object[] levelIds = sheet[keys[0]];

                List<SceneConfigData> tempList = new List<SceneConfigData>();

				for(int i = 2; i< levelIds.Length; i++ )
				{

                    SceneConfigData data = new SceneConfigData();
                    data._lMapID = Convert.ToInt32(sheet["lMapID"][i]);
                    data._szSceneName = Convert.ToString(sheet["szSceneName"][i]);
					data._szMapFile = Convert.ToString(sheet["szMapFile"][i]);
					data._mapWidth = Convert.ToInt32(sheet["lMapPixelWidth"][i])/10;
                    data._mapHeight = Convert.ToInt32(sheet["lMapPixelHeight"][i]) / 10;
                    data._szNameID = Convert.ToString(sheet["szName"][i]);
                    data._szMapIcon = Convert.ToString(sheet["szMapIcon"][i]);
                    string iconPath = System.IO.Path.Combine(MapIconPrefabPath,data._szMapIcon+".prefab");
                    data.MpIconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(iconPath,typeof(GameObject));
					data._mapBGM = Convert.ToString(sheet["MapBgm"][i]);
                    data._sceneType = Convert.ToInt32(sheet["lType"][i]);
                    data._isLockMode = Convert.ToBoolean(sheet["LockingOperationMode"][i]);
                    var triggerData = Convert.ToString(sheet["TrigerArea"][i]);
                    if (triggerData != "0")
                    {
                        var triggers = triggerData.Split('|');
                        var triggerNum=triggers.Length;
                        data._TriggerAreaPoint = new Vector3[triggerNum];
                        data._TriggerAreaRadius= new float[triggerNum];

                        for (int j = 0; j < triggerNum; j++)
                        {
                            var pointInfo = triggers[j].Split('+');
                            data._TriggerAreaPoint[j] = new Vector3(float.Parse(pointInfo[0])*0.1f, 0, float.Parse(pointInfo[1])*-0.1f);
                            data._TriggerAreaRadius[j] = float.Parse(pointInfo[2]) * 0.1f;
                        }
                    }
                    
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
            if (file.Contains(RESOURCE_SCENE_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateSceneConfigDataBase(List<SceneConfigData> list)
	{
        string fileName = typeof(SceneConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_SCENE_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            SceneConfigDataBase database = (SceneConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(SceneConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new SceneConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = list[i];
               
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            SceneConfigDataBase database = ScriptableObject.CreateInstance<SceneConfigDataBase>();

            database._dataTable = new SceneConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                 database._dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
