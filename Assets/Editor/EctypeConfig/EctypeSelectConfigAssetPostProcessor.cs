using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class EctypeSelectConfig : AssetPostprocessor 
{
    public static readonly string RESOURCE_UI_CONFIG_FOLDER = "Assets/Data/EctypeConfig/Res";
    public static readonly string ASSET_UI_CONFIG_FOLDER = "Assets/Data/EctypeConfig/Data";
	public static readonly string ASSET_ECTYPE_RES_CONFIG_FOLDER = "Assets/Prefab/GUI/IconPrefab/EctypeIcon";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            string path = System.IO.Path.Combine(RESOURCE_UI_CONFIG_FOLDER, "EctypeDifficulty.xml");
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

                List<EctypeSelectConfigData> tempList = new List<EctypeSelectConfigData>();

				for(int i = 3; i< levelIds.Length; i++ )
				{
                    EctypeSelectConfigData data = new EctypeSelectConfigData();
                    data._lEctypeID = Convert.ToInt32(sheet["lEctypeID"][i]);
                    data._szName = Convert.ToString(sheet["szName"][i]);
//					string[] vectDifficultyValue = Convert.ToString(sheet["byContainerNum"][i]).Split('+');
//                    data._vectDifficulty = new int[vectDifficultyValue.Length];
//                    for (int j = 0; j < vectDifficultyValue.Length; ++j )
//                    {
//                        data._vectDifficulty[j] = int.Parse(vectDifficultyValue[j]);
//                    }
					string[] difficult2Container =  Convert.ToString(sheet["Difficult2Container"][i]).Split('+');
					List<int> difficult2ContainerData = new List<int>();
					difficult2Container.ApplyAllItem(P=>difficult2ContainerData.Add(int.Parse(P)));
					data.Difficult2Container = new List<int>();
					if(difficult2ContainerData[0] != 0)
					{
						data.Difficult2Container.AddRange(difficult2ContainerData);
					}
					//data.Difficult2Container = difficult2ContainerData.ToArray();
                    //data._vectDifficulty = vectDifficultyValue.Split('+');
                    string[] vectContainerValue = Convert.ToString(sheet["vectContainer"][i]).Split('+');
                    data._vectContainer = new int[vectContainerValue.Length];
                    for (int k = 0; k < vectContainerValue.Length; ++k)
                    {
                        data._vectContainer[k] = int.Parse(vectContainerValue[k]);
                    }

					data._EctypeIcon = Convert.ToString(sheet["lEctypeIcon"][i]);
					data.EctypeRewardIcon = Convert.ToString(sheet["EctypeRewardIcon"][i]);
					List<EctypeRewardItem> rewardItem = new List<EctypeRewardItem>();
					string[] rewardItemStr = Convert.ToString(sheet["AwardItem"][i]).Split('|');
					rewardItemStr.ApplyAllItem(P=>rewardItem.Add(new EctypeRewardItem(){ItemID = int.Parse(P.Split('+')[0]),ItemNum = int.Parse(P.Split('+')[1])}));
					data.AwardItem = rewardItem.ToArray();
					data._lEctypeYaoqiMax = Convert.ToInt32(sheet["lEctypeYaoqiMax"][i]);
                    data._sirenEctypeContainerID = Convert.ToInt32(sheet["SirenEctypeContainerID"][i]);
                    data.lEctypeType = Convert.ToInt32(sheet["lEctypeType"][i]);

                    string pathRes = System.IO.Path.Combine(ASSET_ECTYPE_RES_CONFIG_FOLDER, data._EctypeIcon + ".prefab");
                    data._EctypeIconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

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
            if (file.Contains(RESOURCE_UI_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateSceneConfigDataBase(List<EctypeSelectConfigData> list)
	{
        string fileName = typeof(EctypeSelectConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_UI_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            EctypeSelectConfigDataBase database = (EctypeSelectConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(EctypeSelectConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new EctypeSelectConfigData[list.Count];

			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            EctypeSelectConfigDataBase database = ScriptableObject.CreateInstance<EctypeSelectConfigDataBase>();

            database._dataTable = new EctypeSelectConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
                //database._dataTable[i] = new EctypeSelectConfigData();
                //database._dataTable[i]._lEctypeID = list[i]._lEctypeID;
                //database._dataTable[i]._szName = list[i]._szName;
                //database._dataTable[i]._vectDifficulty = list[i]._vectDifficulty;
                //database._dataTable[i]._vectContainer = list[i]._vectContainer;
                //database._dataTable[i]._EctypeIcon = list[i]._EctypeIcon;
                //database._dataTable[i]._lEctypeYaoqiMax = list[i]._lEctypeYaoqiMax;
                //database._dataTable[i]._sirenEctypeContainerID = list[i]._sirenEctypeContainerID;
                //database._dataTable[i]._prefab = list[i]._prefab;
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
