using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
using UnityEditor;
using System;
using System.Linq;

public class PlayerTitleConfigAssetPostProcessor : AssetPostprocessor
{
	private static readonly string RESOURCE_TRAP_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Res";
	private static readonly string ASSET_TRAP_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Data";
	//private static readonly string ASSET_Prefab_GUI_IconPrefab_SirenIcon_FOLDER = "Assets/Prefab/GUI/IconPrefab/SirenIcon";    
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
	                                           string[] movedAssets, string[] movedFromPath)
	{        
		{
			
			if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
			{
				string path = System.IO.Path.Combine(RESOURCE_TRAP_CONFIG_FOLDER, "PlayerTitleConfig.xml");
				TextReader tr = new StreamReader(path);
				string text = tr.ReadToEnd();
				
				if (text == null)
				{
					Debug.LogError("Player level config file not exist");
					return;
				}
				else
				{
					XmlSpreadSheetReader.ReadSheet(text);
					XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
					string[] keys = XmlSpreadSheetReader.Keys;
					
					object[] levelIds = sheet[keys[0]];
					
					List<PlayerTitleConfigData> tempList = new List<PlayerTitleConfigData>();

					for (int i = 0; i < levelIds.Length; i++)
					{
						if (0 == i || 1 == i) continue;

						PlayerTitleConfigData data = new PlayerTitleConfigData();
						//itemData
						data._goodID = Convert.ToInt32(sheet["lGoodsID"][i]);
						data._szGoodsName = sheet["szGoodsName"][i].ToString();
						data._GoodsClass = Convert.ToInt32(sheet["lGoodsClass"][i]);
						data._GoodsSubClass = Convert.ToInt32(sheet["lGoodsSubClass"][i]);
						data._ColorLevel = Convert.ToInt32(sheet["lColorLevel"][i]);
						data._AllowProfession = Convert.ToString(sheet["lAllowProfession"][i]);
						data._Level = Convert.ToInt32(sheet["lLevel"][i]);
						data._AllowLevel = Convert.ToInt32(sheet["lAllowLevel"][i]);
						data._PileQty = Convert.ToInt32(sheet["lPileQty"][i]);
						data._BuyCost = Convert.ToInt32(sheet["lBuyCost"][i]);
						data._SaleCost = int.Parse(Convert.ToString(sheet["lSaleCost"][i]).Split('+')[1]);
						data._AllowSex = Convert.ToInt32(sheet["lAllowSex"][i]);
						data._ThrowFlag = Convert.ToInt32(sheet["lThrowFlag"][i]);
						data._TradeFlag = Convert.ToInt32(sheet["lTradeFlag"][i]);
						data._GiveFlag = Convert.ToInt32(sheet["lGiveFlag"][i]);
						data._BindFlag = Convert.ToInt32(sheet["lBindFlag"][i]);
						data.smallDisplay = Convert.ToString(sheet["lDisplayIdSmall"][i]);
						//playerTitleData
						data._lGoodsID = Convert.ToInt32(sheet["lGoodsID"][i]);
						data._TitleDroit = Convert.ToInt32(sheet["TitleDroit"][i]);
						data._ByHaveTimeLimit = Convert.ToInt32(sheet["ByHaveTimeLimit"][i]);
						data._lUseTerm = Convert.ToInt32(sheet["lUseTerm"][i]);
						data._lDisplayIdSmall = Convert.ToString(sheet["lDisplayIdSmall"][i]);
						data._lDisplayIdSmallPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/GUI/IconPrefab/TitleIcon/"+Convert.ToString(sheet["lDisplayIdSmall"][i])+".prefab",typeof(GameObject));
						//string modelIdStr = "Assets\Effects\Prefab"
						data._ModelIdPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Effects/Prefab/"+Convert.ToString(sheet["ModelId"][i])+".prefab",typeof(GameObject));
						data._szDesc = Convert.ToString(sheet["szDesc"][i]);
						data._lDisplayID = Convert.ToString(sheet["lDisplayID"][i]);
						string[] getReqStrs = Convert.ToString(sheet["GetReq"][i]).Split('|');
						data._GetReqs = new PlayerTitleGetReq[getReqStrs.Length];
						for(int p = 0;p<getReqStrs.Length;p++)
						{
							data._GetReqs[p] = new PlayerTitleGetReq();							
							string[] getReqStr = getReqStrs[p].Split('+');	
							data._GetReqs[p].ID = Convert.ToInt32(getReqStr[0]);
							data._GetReqs[p].Parameter = Convert.ToInt32(getReqStr[1]);
						}
						data._vectEffects = Convert.ToString(sheet["vectEffects"][i]);
						data._vectEffectsAdd = Convert.ToString(sheet["vectEffectsAdd"][i]);

						tempList.Add(data);
					}

					
					CreateSceneConfigDataBase(tempList);
				}
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
		
		
		private static void CreateSceneConfigDataBase(List<PlayerTitleConfigData> list)
		{
			string fileName = typeof(PlayerTitleConfigDataBase).Name;
			string path = System.IO.Path.Combine(ASSET_TRAP_CONFIG_FOLDER, fileName + ".asset");
			
			if (File.Exists(path))
			{
				PlayerTitleConfigDataBase database = (PlayerTitleConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerTitleConfigDataBase));
				
				if (null == database)
				{
					return;
				}
				
				database._dataTable = new PlayerTitleConfigData[list.Count];
				
				for (int i = 0; i < list.Count; i++)
				{
					database._dataTable[i] = new PlayerTitleConfigData();
					database._dataTable[i] = list[i];
				}
				EditorUtility.SetDirty(database);
			}
			else
			{
				PlayerTitleConfigDataBase database = ScriptableObject.CreateInstance<PlayerTitleConfigDataBase>();
				
				database._dataTable = new PlayerTitleConfigData[list.Count];
				
				for (int i = 0; i < list.Count; i++)
				{
				database._dataTable[i] = new PlayerTitleConfigData();
					database._dataTable[i] = list[i];
				}
				AssetDatabase.CreateAsset(database, path);
			}
			
		}
}