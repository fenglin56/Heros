using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class PlayerAuctionAssetPostProcessor : AssetPostprocessor 
{
	private static readonly string RESOURCE_SHOP_CONFIG_FOLDER = "Assets/Data/PlayerAuction/Res";
	private static readonly string ASSET_SHOP_CONFIG_FOLDER = "Assets/Data/PlayerAuction/Data";
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
			string path = System.IO.Path.Combine(RESOURCE_SHOP_CONFIG_FOLDER, "PlayerAuction.xml");
			TextReader tr = new StreamReader(path);
			string text = tr.ReadToEnd();
			
			if(text == null)
			{
				Debug.LogError("PlayerAuction config file not exist");
				return;	
			}
			else
			{
				XmlSpreadSheetReader.ReadSheet(text);
				XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
				string[] keys =  XmlSpreadSheetReader.Keys;
				
				object[] levelIds = sheet[keys[0]];
				
				List<AuctionConfigData> tempList = new List<AuctionConfigData>();
				
				for(int i = 2; i< levelIds.Length; i++ )
				{
					AuctionConfigData data = new AuctionConfigData();
					data.AuctionClass = Convert.ToInt32(sheet["AuctionClass"][i]);
					data.AuctionID = Convert.ToInt32(sheet["AuctionID"][i]);
					data.GoodsWeight = Convert.ToInt32(sheet["GoodsWeight"][i]);
					data.GoodsID = Convert.ToInt32(sheet["GoodsID"][i]);
					data.GoodsNum = Convert.ToInt32(sheet["GoodsNum"][i]);
					data.StartingBid = Convert.ToInt32(sheet["StartingBid"][i]);
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
	
	
	private static void CreateConfigDataBase(List<AuctionConfigData> list)
	{
		string fileName = typeof(AuctionConfigData).Name;
		string path = System.IO.Path.Combine(ASSET_SHOP_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
			AuctionConfigDataBase database = (AuctionConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(AuctionConfigDataBase));
			
			if(null == database)
			{
				return;
			}
			
			database._dataTable = new AuctionConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			AuctionConfigDataBase database = ScriptableObject.CreateInstance<AuctionConfigDataBase>();
			
			database._dataTable = new AuctionConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}

