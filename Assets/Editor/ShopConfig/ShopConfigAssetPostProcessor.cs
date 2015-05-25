using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class ShopConfig : AssetPostprocessor 
{
	private static readonly string RESOURCE_SHOP_CONFIG_FOLDER = "Assets/Data/ShopConfig/Res";
    private static readonly string ASSET_SHOP_CONFIG_FOLDER = "Assets/Data/ShopConfig/Data";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            string path = System.IO.Path.Combine(RESOURCE_SHOP_CONFIG_FOLDER, "Shopmall.xml");
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

                List<ShopConfigData> tempList = new List<ShopConfigData>();

				for(int i = 0; i< levelIds.Length; i++ )
				{
                    if (0 == i || 1 == i) continue;
                    ShopConfigData data = new ShopConfigData();
                    data._shopGoodsID = Convert.ToInt32(sheet["ShopGoodsID"][i]);
                    data._goodsNum = Convert.ToInt32(sheet["GoodsNum"][i]);
                    data._shopName = Convert.ToString(sheet["ShopName"][i]);
                    data._shopID = Convert.ToInt32(sheet["nShopID"][i]);
                    data.GoodsID = Convert.ToInt32(sheet["GoodsID"][i]);
                    data.BuyLvl = Convert.ToInt32(sheet["BuyLvl"][i]);
                    data.BuyType = Convert.ToInt32(sheet["lBuyType"][i]);
                    data.Price = Convert.ToInt32(sheet["lPrice"][i]);
                    data.ExChangeGoodID = Convert.ToString(sheet["lExChangeGoodID"][i]);
					data.GoodsPicture = Convert.ToString(sheet["GoodsPicture"][i]);
					data.GoodsNameIds = Convert.ToString(sheet["GoodsNameIds"][i]);
					if(!data.GoodsPicture.Equals("0"))
					{
						data.goodsPicturePrefab = (GameObject)Resources.LoadAssetAtPath(data.GoodsPicture + ".prefab", typeof(GameObject));//Prefab/GUI/IconPrefab/QuickBuyIcon
					}
					data.PackageNeed = Convert.ToInt32(sheet["PackageNeed"][i]);
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


    private static void CreateConfigDataBase(List<ShopConfigData> list)
	{
        string fileName = typeof(ShopConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_SHOP_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            ShopConfigDataBase database = (ShopConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(ShopConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new ShopConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] =  list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            ShopConfigDataBase database = ScriptableObject.CreateInstance<ShopConfigDataBase>();

            database._dataTable = new ShopConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
