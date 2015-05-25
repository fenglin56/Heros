using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class EvaluateConfigAssetPostProcessor : AssetPostprocessor 
{
    public static readonly string RESOURCE_UI_CONFIG_FOLDER = "Assets/Data/EvaluateConfig/Res";
    public static readonly string ASSET_UI_CONFIG_FOLDER = "Assets/Data/EvaluateConfig/Data";
    public static readonly string ASSET_ECTYPE_RES_CONFIG_FOLDER = "Assets/Prefab/GUI/IconPrefab/EvaluateIcon";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            string path = System.IO.Path.Combine(RESOURCE_UI_CONFIG_FOLDER, "EvaluateConfig.xml");
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

				List<EvaluateData> tempList_Battle = new List<EvaluateData>();
				List<EvaluateData> tempList_Town = new List<EvaluateData>();

				for(int i = 2; i< levelIds.Length; i++ )
				{
					EvaluateData data_Battle = new EvaluateData();
					EvaluateData data_Town = new EvaluateData();

					data_Battle.Evaluate = Convert.ToString(sheet["Evaluate"][i]);
					data_Town.Evaluate = Convert.ToString(sheet["Evaluate"][i]);
                    string icon = Convert.ToString(sheet["Icon"][i]);

                    string pathRes = System.IO.Path.Combine(ASSET_ECTYPE_RES_CONFIG_FOLDER, icon + ".prefab");
					data_Battle.IconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));
					
					string Staricon = Convert.ToString(sheet["StarRating"][i]);
					string StarpathRes = System.IO.Path.Combine(ASSET_ECTYPE_RES_CONFIG_FOLDER, Staricon + ".prefab");
					data_Town.StarIconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(StarpathRes, typeof(GameObject));
					data_Battle.StarIconPrefab = data_Town.StarIconPrefab;

					tempList_Battle.Add(data_Battle);
					tempList_Town.Add(data_Town);
				}

				CreateSceneConfigDataBase(tempList_Battle,"_Battle");
				CreateSceneConfigDataBase(tempList_Town,"_Town");
			}
		}
	}
	
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
            if (file.Contains(RESOURCE_UI_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateSceneConfigDataBase(List<EvaluateData> list,string tag)
	{
        string fileName = typeof(EvaluateConfigDataBase).Name+tag;
        string path = System.IO.Path.Combine(ASSET_UI_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            EvaluateConfigDataBase database = (EvaluateConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(EvaluateConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database.EvaluateDataList = new EvaluateData[list.Count];

			for(int i = 0; i < list.Count; i++)
			{
                database.EvaluateDataList[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            EvaluateConfigDataBase database = ScriptableObject.CreateInstance<EvaluateConfigDataBase>();

            database.EvaluateDataList = new EvaluateData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
            {
                database.EvaluateDataList[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
