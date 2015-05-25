using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class GoldRechargeConfigPostProcessor : AssetPostprocessor 
{
    private static readonly string RESOURCE_SHOP_CONFIG_FOLDER = "Assets/Data/GoldRecharge/Res";
    private static readonly string ASSET_SHOP_CONFIG_FOLDER = "Assets/Data/GoldRecharge/Data";
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            string path = System.IO.Path.Combine(RESOURCE_SHOP_CONFIG_FOLDER, "GoldRecharge.xml");
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

                List<GoldRechargeData> tempList = new List<GoldRechargeData>();

				for(int i = 1; i< levelIds.Length; i++ )
				{
                    GoldRechargeData data = new GoldRechargeData();
                    data.RechargeID = Convert.ToInt32(sheet["RechargeID"][i]);
                    data.RechargeType = Convert.ToInt32(sheet["RechargeType"][i]);
                    data.RechargePosition = Convert.ToString(sheet["RechargePosition"][i]);
                    data.CurrencyPicture = Convert.ToString(sheet["CurrencyPicture"][i]);
                    data.GoldPicture = Convert.ToString(sheet["GoldPicture"][i]);
					data.goldPicturePrefab = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/GUI/IconPrefab/VIPIcon/" + data.GoldPicture + ".prefab", typeof(GameObject));
                    data.CurrencyNumber = Convert.ToSingle(sheet["CurrencyNumber"][i]);
                    data.GoldNumber = Convert.ToInt32(sheet["GoldNumber"][i]);
                    data.Discount = Convert.ToSingle(sheet["Discount"][i]);
                    data.TouchAnimation = Convert.ToString(sheet["TouchAnimation"][i]);
                    data.TouchVoice = Convert.ToString(sheet["TouchVoice"][i]);
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
            if (file.Contains(RESOURCE_SHOP_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateConfigDataBase(List<GoldRechargeData> list)
	{
        string fileName = typeof(GoldRechargeConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_SHOP_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            GoldRechargeConfigDataBase database = (GoldRechargeConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(GoldRechargeConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database.GoledRechargeDataList = new GoldRechargeData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database.GoledRechargeDataList[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            GoldRechargeConfigDataBase database = ScriptableObject.CreateInstance<GoldRechargeConfigDataBase>();

            database.GoledRechargeDataList = new GoldRechargeData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database.GoledRechargeDataList[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
