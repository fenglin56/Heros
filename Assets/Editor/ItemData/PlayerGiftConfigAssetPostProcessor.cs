using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
using UnityEditor;
using System;

public class PlayerGiftConfigAssetPostProcessor : AssetPostprocessor {

	private static readonly string RESOURCE_TRAP_CONFIG_FOLDER = "Assets/Data/ItemData/Res";
	private static readonly string ASSET_TRAP_CONFIG_FOLDER = "Assets/Data/ItemData/Data";    
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
	                                           string[] movedAssets, string[] movedFromPath)
	{        
		if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
		{
			string path = System.IO.Path.Combine(RESOURCE_TRAP_CONFIG_FOLDER, "PlayerGift.xml");
			TextReader tr = new StreamReader(path);
			string text = tr.ReadToEnd();
			
			if (text == null)
			{
				Debug.LogError("Player Room file not exist");
				return;
			}
			else
			{                
				XmlSpreadSheetReader.ReadSheet(text);
				XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
				string[] keys = XmlSpreadSheetReader.Keys;
				
				object[] levelIds = sheet[keys[0]];
				
				List<PlayerGiftConfigData> tempList = new List<PlayerGiftConfigData>();
				
				
				for (int i = 0; i < levelIds.Length; i++)
				{
					if (0 == i || 1 == i) continue;
					PlayerGiftConfigData data = new PlayerGiftConfigData();
					data._goodsID = Convert.ToInt32(sheet["GoodsID"][i]);
					data._packageNeed = Convert.ToInt32(sheet["PackageNeed"][i]);
					data._giftType = Convert.ToInt32(sheet["GiftType"][i]);
					//data._goodsWeight = Convert.ToString(sheet["GoodsWeight"][i]);
					//data._getGoodsID = Convert.ToString(sheet["GetGoodsID"][i]);
					//data._getGoodsNum = Convert.ToString(sheet["GetGoodsNum"][i]);


					data._ProfessionGoodsDisplay = new PlayerGiftConfigData.ProfessionGoodsDisplay[2]; 
					int[] intKey = new int[2]{1,4};	//职业id
					string[] strKey = new string[2]{"Player1GoodsDisplay","Player4GoodsDisplay"};//字段
					for(int k = 0; k<strKey.Length; k++)
					{
						string[] goodsDisplayStr = Convert.ToString(sheet[strKey[k]][i]).Split('|');
						data._ProfessionGoodsDisplay[k] = new PlayerGiftConfigData.ProfessionGoodsDisplay();
						data._ProfessionGoodsDisplay[k].GoodsDisplay = new PlayerGiftConfigData.GoodsDisplay[goodsDisplayStr.Length];
						data._ProfessionGoodsDisplay[k].Profession = intKey[k];
						for(int p=0;p<goodsDisplayStr.Length;p++)
						{
							string[] goodsStr = goodsDisplayStr[p].Split('+');
							data._ProfessionGoodsDisplay[k].GoodsDisplay[p] = new PlayerGiftConfigData.GoodsDisplay();
							data._ProfessionGoodsDisplay[k].GoodsDisplay[p].GoodsID = Convert.ToInt32(goodsStr[0]);
							data._ProfessionGoodsDisplay[k].GoodsDisplay[p].MinNum = Convert.ToInt32(goodsStr[1]);
							data._ProfessionGoodsDisplay[k].GoodsDisplay[p].MaxNum = Convert.ToInt32(goodsStr[2]);
						}
					}


//					string[] goodsIDStr = Convert.ToString(sheet["GetGoodsID"][i]).Split('+');
//					string[] goodsNumStr = Convert.ToString(sheet["GetGoodsNum"][i]).Split('+');
//					data._GetGoods = new PlayerGiftConfigData.GetGoods[goodsIDStr.Length];
//					for(int p=0;p<goodsIDStr.Length;p++)
//					{
//						data._GetGoods[p] = new PlayerGiftConfigData.GetGoods();
//						data._GetGoods[p].GoodsID = Convert.ToInt32(goodsIDStr[p]);
//						data._GetGoods[p].GoodsNum = Convert.ToInt32(goodsNumStr[p]);
//					}
					//data.PlayerAngleList.Add(vTenantAngle);

					//data._damagePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
					//string correspondingItemIDStr = Convert.ToString(sheet["BoxGoodsDrop"][i]);
					//string[] splitCorrespondingItemIDStrs = correspondingItemIDStr.Split("+".ToCharArray());
					//data._correspondingItemID = Convert.ToInt32(splitCorrespondingItemIDStrs[0]);
					
					tempList.Add(data);
				}
				
				
				
				
				CreateSceneConfigDataBase(tempList);
			}
		}
	}
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach (string file in files)
		{
			if (file.Contains(RESOURCE_TRAP_CONFIG_FOLDER))
			{
				fileModified = true;
				break;
			}
		}
		return fileModified;
	}
	
	
	private static void CreateSceneConfigDataBase(List<PlayerGiftConfigData> list)
	{
		string fileName = typeof(PlayerGiftConfigData).Name;
		string path = System.IO.Path.Combine(ASSET_TRAP_CONFIG_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			PlayerGiftConfigDataBase database = (PlayerGiftConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerGiftConfigDataBase));
			
			if (null == database)
			{
				return;
			}
			
			database._dataTable = new PlayerGiftConfigData[list.Count];
			list.CopyTo(database._dataTable);
			
			EditorUtility.SetDirty(database);
		}
		else
		{
			PlayerGiftConfigDataBase database = ScriptableObject.CreateInstance<PlayerGiftConfigDataBase>();
			
			database._dataTable = new PlayerGiftConfigData[list.Count];
			list.CopyTo(database._dataTable);
			
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}
