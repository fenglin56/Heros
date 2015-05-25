using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;




public class ItemDataPostProcessor : AssetPostprocessor 
{
	public static readonly string RESOURCE_ITEM_DATA_FOLDER = "Assets/Data/ItemData/Res";
    public static readonly string ASSET_ITEM_DATA_FOLDER = "Assets/Data/ItemData/Data";
    public static readonly string ASSET_ITEM_DATA_ICON_FOLDER = "Assets/Prefab/GUI/ItemIcon";
    public static readonly string ASSET_WEAPON_EFFECT_FOLDER="Assets/Effects/Prefab";
	
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
			OnPostprocessEquipment();
			OnPostprocessMedicament();
			OnPostprocessMateriel();
			OnPostprocessJewel();
		}
		
	}
	
	private static void OnPostprocessEquipment()
	{
		
        string path = System.IO.Path.Combine(RESOURCE_ITEM_DATA_FOLDER, "Equipment.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if(text == null)
		{
			Debug.LogError("Equipment item file not exist");
			return;	
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys =  XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];

            List<EquipmentData> tempList = new List<EquipmentData>();

			for(int i = 2; i< levelIds.Length; i++ )
			{
                if (0 == i || 1 == i) continue;
                EquipmentData data = new EquipmentData();
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
                if (!string.IsNullOrEmpty(data.smallDisplay) && data.smallDisplay != "0")
                {
                    var displayPath = Path.Combine(ASSET_ITEM_DATA_ICON_FOLDER, data.smallDisplay + ".prefab");
                    var displayGo = AssetDatabase.LoadAssetAtPath(displayPath, typeof(GameObject)) as GameObject;
                    data._picPrefab = data._DisplayIdSmall = displayGo;
                }
               
                //string btnIdRound = Convert.ToString(sheet["lDisplayIdRound"][i]);
                //if (!string.IsNullOrEmpty(btnIdRound))
                //{
                //    var displayPath = Path.Combine(ASSET_ITEM_DATA_ICON_FOLDER, btnIdRound + ".prefab");
                //    var displayGo = AssetDatabase.LoadAssetAtPath(displayPath, typeof(GameObject)) as GameObject;
                //    data._picPrefab = data.lDisplayIdRound = displayGo;
                //}
                data._ModelId = Convert.ToString(sheet["ModelId"][i]);
                data._DisplayIdBig = Convert.ToString(sheet["lDisplayIdBig"][i]);
                if (!string.IsNullOrEmpty(data._DisplayIdBig) && data._DisplayIdBig != "0")
                {
                    var displayPath = Path.Combine(ASSET_ITEM_DATA_ICON_FOLDER, data._DisplayIdBig + ".prefab");
                    var displayGo = AssetDatabase.LoadAssetAtPath(displayPath, typeof(GameObject)) as GameObject;
                    data.DisplayBig_prefab = displayGo;
                }
                data._szDesc = Convert.ToString(sheet["szDesc"][i]);
                data.LinkIds=Convert.ToString(sheet["Link"][i]).Split('+');
				data.CanBeFastSelect = Convert.ToString(sheet["CanBeFastSelect"][i]).Trim() == "1";

                data._DisplayID = Convert.ToInt32(sheet["lDisplayID"][i]);
                data._EquipmentKind = Convert.ToInt32(sheet["lEquipmentKind"][i]);
                data._SuitEquipID = Convert.ToInt32(sheet["lSuitEquipID"][i]);
                data._vectEquipLoc = Convert.ToString(sheet["vectEquipLoc"][i]);
                data._vectEffects = Convert.ToString(sheet["vectEffects"][i]);
                data._StrengFlag = Convert.ToInt32(sheet["lStrengFlag"][i]);
                data._SmeltFlag = Convert.ToInt32(sheet["lSmeltFlag"][i]);
                data._RecastFlag = Convert.ToInt32(sheet["lRecastFlag"][i]);
                data._VectAddNum = Convert.ToInt32(sheet["VectAddNum"][i]);
                data._vectEffectsAdd = Convert.ToString(sheet["vectEffectsAdd"][i]);
                data._vectSkillID = Convert.ToInt32(sheet["vectSkillID"][i]);
                data._HoleMax = Convert.ToInt32(sheet["lHoleMax"][i]);
                data._lThresholdValue = Convert.ToInt32(sheet["lThresholdValue"][i]);
				data.lUpgradeFlag = Convert.ToString(sheet["lUpgradeFlag"][i]).Replace(" ","") == "1";
				data.UpgradeID = Convert.ToInt32(sheet["UpgradeID"][i]);
				data.UpgradeCost = Convert.ToString(sheet["UpgradeCost"][i]);

                //2013-6-11  Add by rocky
                var strengthPara = Convert.ToString(sheet["Strength"][i]);
                string[] strengthArr = strengthPara.Split('|');
                data._StrengthParameter = new StrengthParameter[strengthArr.Length];
                for (int m = 0; m < strengthArr.Length; m++)
                {
                    var strength = strengthArr[m].Split('+');
                    data._StrengthParameter[m] = new StrengthParameter()
                    {

                        Index = m,
						Value=new int[strength.Length]
                    };
					for(int k=0;k<strength.Length;k++)
					{
						data._StrengthParameter[m].Value[k]=Convert.ToInt32(strength[k]);
					}
                };
                var strengthCostPara = Convert.ToString(sheet["StrengthCost"][i]);
                string[] strengthCostArr = strengthCostPara.Split('+');
                data._StrengthCost = new float[]
                {
                    float.Parse(strengthCostArr[0])
                    ,float.Parse(strengthCostArr[1])
                    ,float.Parse(strengthCostArr[2])
                    ,float.Parse(strengthCostArr[3])
                };        

				var starStrengthPara = Convert.ToString(sheet["StartStrength"][i]);
				var starStrengthArr = starStrengthPara.Split('|');
				data._StartStrengthParameter = new StrengthParameter[starStrengthArr.Length];
				for (int m = 0; m < starStrengthArr.Length; m++)
				{
					var strength = starStrengthArr[m].Split('+');
					data._StartStrengthParameter[m] = new StrengthParameter()
					{
						
						Index = m,
						Value=new int[strength.Length]
					};
					for(int k=0;k<strength.Length;k++)
					{
						data._StartStrengthParameter[m].Value[k]=Convert.ToInt32(strength[k]);
					}
				}
                var startStrengthCostPara = Convert.ToString(sheet["StartStrengthCost"][i]);
                string[] startStrengthCostArr = startStrengthCostPara.Split('|');
                data._StartStrengthCost = new StartStrengthLvCost[startStrengthCostArr.Length];
                for (int n = 0; n < startStrengthCostArr.Length; n++)
                {
                    var cost = startStrengthCostArr[n].Split('+');
                    data._StartStrengthCost[n] = new StartStrengthLvCost()
                    {

                        Lv = n+1
                         , ItemID_1=int.Parse(cost[0]), Value_1=int.Parse(cost[1])
//                         , ItemID_2=int.Parse(cost[2]), Value_2=int.Parse(cost[3])
//                         , ItemID_3=int.Parse(cost[4]), Value_3=int.Parse(cost[5])
                    };
                };
                
				var strengthRatePara = Convert.ToString(sheet["StrengthRate"][i]);
				string[] strengthRateParaArr = strengthRatePara.Split('+');
				data.NormalStrenPercent = new float[strengthRateParaArr.Length];
				for(int n=0;n<strengthRateParaArr.Length;n++)
				{
					data.NormalStrenPercent[n] =float.Parse(strengthRateParaArr[n])/100; 
				}				
				var starUpPercentPara = Convert.ToString(sheet["StartStrengthRate"][i]);
				string[] starUpPercentParaArr = starUpPercentPara.Split('+');
				data.StarUpPercent = new float[starUpPercentParaArr.Length];
				for (int n = 0; n < starUpPercentParaArr.Length; n++)
				{
					data.StarUpPercent[n] =float.Parse(starUpPercentParaArr[n])/100f;  
				};

				var saleItemPara = Convert.ToString(sheet["SaleItem"][i]);
				string[] saleItemParaArr = saleItemPara.Split('|');
				data.SaleItem = new SaleItemPrice[saleItemParaArr.Length];
				for (int n = 0; n < saleItemParaArr.Length; n++)
				{
					var cost = saleItemParaArr[n].Split('+');
					data.SaleItem[n] = new SaleItemPrice()
					{					
						ItemID=int.Parse(cost[0]), Price=int.Parse(cost[1])
					};
				};

                string prefabpath = Convert.ToString(sheet["lDisplayIdSmall"][i]);
                string pathRes = System.IO.Path.Combine(ASSET_ITEM_DATA_ICON_FOLDER, prefabpath + ".prefab");
                data._picPrefab = data._DisplayIdSmall = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

                string WeaponEffString=Convert.ToString(sheet["WeaponEff"][i]);
                if(WeaponEffString!="0")
                {
                    string EffPathRea=Path.Combine(ASSET_WEAPON_EFFECT_FOLDER,WeaponEffString+ ".prefab");
                    data.WeaponEff=(GameObject)AssetDatabase.LoadAssetAtPath(EffPathRea, typeof(GameObject));
                }
                tempList.Add(data);
			}

            CreateEquipmentDataList(tempList);
		}
		
	}
	
	private static void OnPostprocessMedicament()
	{
		string path = System.IO.Path.Combine(RESOURCE_ITEM_DATA_FOLDER, "Medicament.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if(text == null)
		{
			Debug.LogError("Equipment item file not exist");
			return;	
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys =  XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];

            List<MedicamentData> tempList = new List<MedicamentData>();

			for(int i = 2; i< levelIds.Length; i++ )
			{
                if (0 == i || 1 == i) continue;
                MedicamentData data = new MedicamentData();
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
                //data._SaleCost = Convert.ToInt32(sheet["lSaleCost"][i]);
                data._SaleCost = int.Parse(Convert.ToString(sheet["lSaleCost"][i]).Split('+')[1]);
                data._AllowSex = Convert.ToInt32(sheet["lAllowSex"][i]);
                data._ThrowFlag = Convert.ToInt32(sheet["lThrowFlag"][i]);
                data._TradeFlag = Convert.ToInt32(sheet["lTradeFlag"][i]);
                data._GiveFlag = Convert.ToInt32(sheet["lGiveFlag"][i]);
                data._BindFlag = Convert.ToInt32(sheet["lBindFlag"][i]);                
                data._DisplayIdBig = Convert.ToString(sheet["lDisplayIdBig"][i]);
                data._szDesc = Convert.ToString(sheet["szDesc"][i]);
                data.LinkIds=Convert.ToString(sheet["Link"][i]).Split('+');
				data.CanBeFastSelect = Convert.ToString(sheet["CanBeFastSelect"][i]).Replace(" ","") == "1";

                data._PassiveRace = Convert.ToInt32(sheet["lPassiveRace"][i]);
                data._BatchFlag = Convert.ToInt32(sheet["lBatchFlag"][i]);
                data._OnID = Convert.ToInt32(sheet["lOnID"][i]);
                data._vectEffects = Convert.ToString(sheet["vectEffects"][i]);
                data.smallDisplay = Convert.ToString(sheet["lDisplayIdSmall"][i]);

                string prefabpath = Convert.ToString(sheet["lDisplayIdSmall"][i]);
                string pathRes = System.IO.Path.Combine(ASSET_ITEM_DATA_ICON_FOLDER, prefabpath + ".prefab");
                data._picPrefab = data._DisplayIdSmall = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

                string btnIdRound = Convert.ToString(sheet["lDisplayIdRound"][i]);
                if (!string.IsNullOrEmpty(btnIdRound))
                {
                    var displayPath = Path.Combine(ASSET_ITEM_DATA_ICON_FOLDER, btnIdRound + ".prefab");
                    var displayGo = AssetDatabase.LoadAssetAtPath(displayPath, typeof(GameObject)) as GameObject;
                    data.lDisplayIdRound = displayGo;
                }

				tempList.Add(data);
			}

            CreateMedicamentDataList(tempList);
		}
	}
	
	private static void OnPostprocessMateriel()
	{
		string path = System.IO.Path.Combine(RESOURCE_ITEM_DATA_FOLDER, "Materiel.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if(text == null)
		{
			Debug.LogError("Equipment item file not exist");
			return;	
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys =  XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];

            List<MaterielData> tempList = new List<MaterielData>();

			for(int i = 2; i< levelIds.Length; i++ )
			{
                if (0 == i || 1 == i) continue;
                MaterielData data = new MaterielData();
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
                //data._SaleCost = Convert.ToInt32(sheet["lSaleCost"][i]);
                data._SaleCost = int.Parse(Convert.ToString(sheet["lSaleCost"][i]).Split('+')[1]);
                data._AllowSex = Convert.ToInt32(sheet["lAllowSex"][i]);
                data._ThrowFlag = Convert.ToInt32(sheet["lThrowFlag"][i]);
                data._TradeFlag = Convert.ToInt32(sheet["lTradeFlag"][i]);
                data._GiveFlag = Convert.ToInt32(sheet["lGiveFlag"][i]);
                data._BindFlag = Convert.ToInt32(sheet["lBindFlag"][i]);
                data._DisplayIdBig = Convert.ToString(sheet["lDisplayIdBig"][i]);
                data._szDesc = Convert.ToString(sheet["szDesc"][i]);
                data.LinkIds=Convert.ToString(sheet["Link"][i]).Split('+');
				data.CanBeFastSelect = Convert.ToString(sheet["CanBeFastSelect"][i]).Replace(" ","") == "1";

                data._szParam1 = Convert.ToString(sheet["szParam1"][i]);
                data._szParam2 = Convert.ToInt32(sheet["szParam2"][i]);
                data.smallDisplay = Convert.ToString(sheet["lDisplayIdSmall"][i]);

                string prefabpath = Convert.ToString(sheet["lDisplayIdSmall"][i]);
                string pathRes = System.IO.Path.Combine(ASSET_ITEM_DATA_ICON_FOLDER, prefabpath + ".prefab");
                data._picPrefab = data._DisplayIdSmall = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));

				tempList.Add(data);
			}

            CreateMaterielDataList(tempList);
		}
		
		
	}
	/// <summary>
	///???????????????
	/// </summary>
	private static void OnPostprocessJewel()
	{
		string path = System.IO.Path.Combine(RESOURCE_ITEM_DATA_FOLDER, "Jewel.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if(text == null)
		{
			Debug.LogError("Jewel item file not exist");
			return;	
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys =  XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];
			
			List<Jewel> tempList = new List<Jewel>();
			
			for(int i = 2; i< levelIds.Length; i++ )
			{
				if (0 == i || 1 == i) continue;
				Jewel data = new Jewel();
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
				//data._SaleCost = Convert.ToInt32(sheet["lSaleCost"][i]);
				data._SaleCost = int.Parse(Convert.ToString(sheet["lSaleCost"][i]).Split('+')[1]);
				data._AllowSex = Convert.ToInt32(sheet["lAllowSex"][i]);
				data._ThrowFlag = Convert.ToInt32(sheet["lThrowFlag"][i]);
				data._TradeFlag = Convert.ToInt32(sheet["lTradeFlag"][i]);
				data._GiveFlag = Convert.ToInt32(sheet["lGiveFlag"][i]);
				data._BindFlag = Convert.ToInt32(sheet["lBindFlag"][i]);
                data.LinkIds=Convert.ToString(sheet["Link"][i]).Split('+');
				data.CanBeFastSelect = Convert.ToString(sheet["CanBeFastSelect"][i]).Replace(" ","") == "1";

				data._DisplayIdBig = Convert.ToString(sheet["lDisplayIdBig"][i]);
				data._szDesc = Convert.ToString(sheet["szDesc"][i]);
				data.smallDisplay = Convert.ToString(sheet["lDisplayIdSmall"][i]);
				data.PassiveSkill=Convert.ToInt32(sheet["PassiveSkill"][i]);
				data.MaxLevel=Convert.ToInt16(sheet["MaxLevel"][i]);
				string[] _StoneExp=Convert.ToString(sheet["StoneExp"][i]).Split('+');
				data.StoneExp=new int[_StoneExp.Length];
				for(int j=0;j<_StoneExp.Length;j++)
				{
					data.StoneExp[j]=int.Parse(_StoneExp[j]);

				}
				data.StoneStartExp=Convert.ToInt32(sheet["StoneStartExp"][i]);
				data.StoneExpRate=Convert.ToInt32(sheet["StoneExpRate"][i]);
				data.StonePosition=Convert.ToString(sheet["StonePosition"][i]).Split('+');
				data.StoneType=Convert.ToInt32(sheet["StoneType"][i]);
				data.StoneGrop=Convert.ToInt16(sheet["StoneGrop"][i]);
				string[] _activePassiveSkill=Convert.ToString(sheet["ActivePassiveSkill"][i]).Split('+');
				data._activePassiveSkill=new ActivePassiveSkill(){skillID=int.Parse(_activePassiveSkill[0]), skillLevel=short.Parse(_activePassiveSkill[1])};
				data.StoneGropEquipName=Convert.ToString(sheet["StoneGropEquipName"][i]);

				string prefabpath = Convert.ToString(sheet["lDisplayIdSmall"][i]);
				string pathRes = System.IO.Path.Combine(ASSET_ITEM_DATA_ICON_FOLDER, prefabpath + ".prefab");
				data._picPrefab = data._DisplayIdSmall = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));
				
				tempList.Add(data);
			}
			
			CreateJewelDataList(tempList);
		}
		
		
	}

	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
            if (file.Contains(RESOURCE_ITEM_DATA_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}
	
	static void CreateEquipmentDataList(List<EquipmentData> list)
	{
		string fileName = typeof(EquipmentData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_ITEM_DATA_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            ItemDataList database = (ItemDataList)AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList));
			
			if(null == database)
			{
				return;
			}

            database._equipments = new EquipmentData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._equipments[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            ItemDataList database = ScriptableObject.CreateInstance<ItemDataList>();

            database._equipments = new EquipmentData[list.Count];
			for(int i = 0; i < list.Count; i++)
			{
               database._equipments[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
	
	static void CreateMedicamentDataList(List<MedicamentData> list)
	{
		string fileName = typeof(MedicamentData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_ITEM_DATA_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            ItemDataList database = (ItemDataList)AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList));
			
			if(null == database)
			{
				return;
			}

            database._medicaments = new MedicamentData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._medicaments[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            ItemDataList database = ScriptableObject.CreateInstance<ItemDataList>();

            database._medicaments = new MedicamentData[list.Count];
			for(int i = 0; i < list.Count; i++)
			{
                database._medicaments[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
	}
	
	static void CreateMaterielDataList(List<MaterielData> list)
	{
		string fileName = typeof(MaterielData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_ITEM_DATA_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            ItemDataList database = (ItemDataList)AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList));
			
			if(null == database)
			{
				return;
			}

            database._materiels = new MaterielData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._materiels[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            ItemDataList database = ScriptableObject.CreateInstance<ItemDataList>();
            database._materiels = new MaterielData[list.Count];
			for(int i = 0; i < list.Count; i++)
			{
                database._materiels[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
	}

	static void CreateJewelDataList(List<Jewel> list)
	{
		string fileName = typeof(Jewel).Name + "DataBase";
		Debug.Log (fileName);
		string path = System.IO.Path.Combine(ASSET_ITEM_DATA_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
			ItemDataList database = (ItemDataList)AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList));
			
			if(null == database)
			{
				return;
			}
			
			database._jewel = new Jewel[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._jewel[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			ItemDataList database = ScriptableObject.CreateInstance<ItemDataList>();
			database._jewel = new Jewel[list.Count];
			for(int i = 0; i < list.Count; i++)
			{
				database._jewel[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
	}
}
