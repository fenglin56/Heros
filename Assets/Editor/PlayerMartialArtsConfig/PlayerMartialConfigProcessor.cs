using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
using UnityEditor;
using System;
using System.Linq;

public class PlayerMartialArtsConfigProcessor : AssetPostprocessor 
{
	private static readonly string RESOURCE_MARTIAL_CONFIG_FOLOER = "Assets/Data/PlayerConfig/Res";
	private static readonly string ASSET_MARTIAL_CONFIG_FOLOER = "Assets/Data/PlayerConfig/Data";
	private static readonly string ASSET_TRAP_RES_CONFIG_FOLDER = "Assets/Prefab/GUI/IconPrefab/PVPSkillIcon";

	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if(CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
		{
			string path = Path.Combine(RESOURCE_MARTIAL_CONFIG_FOLOER, "PlayerMartialArts.xml");
			TextReader tr = new StreamReader(path);
			string text = tr.ReadToEnd();

			if(text == null)
			{
				Debug.LogError("Player MartialArts config file not exit!");
				return;
			}
			else
			{
				XmlSpreadSheetReader.ReadSheet(text);
				XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
				string[] keys = XmlSpreadSheetReader.Keys;	//列

				object[] levelIds = sheet[keys[0]];	//行数

				List<PlayerMartialArtsData> tempList = new List<PlayerMartialArtsData>();
				for(int i = 0; i < levelIds.Length; i++)
				{
					if(i == 0 || i == 1) continue;	//过滤第一、二行

					PlayerMartialArtsData data = new PlayerMartialArtsData();
					data.MartialArtsType = Convert.ToByte(sheet["MartialArtsType"][i]);
					data.MartialArtsID = Convert.ToInt32(sheet["MartialArtsID"][i]);
					data.MartialArtsUnlock = Convert.ToInt32(sheet["MartialArtsUnlock"][i]);
					data.MartialArtsLevels = Convert.ToInt32(sheet["MartialArtsLevels"][i]);
					data.MartialArtsMaxLevels = Convert.ToInt32(sheet["MartialArtsMaxLevels"][i]);
					data.MartialArtsName = Convert.ToString(sheet["MartialArtsName"][i]);
					//data.MartialArtsIcon = Convert.ToString(sheet["MartialArtsIcon"][i]);
					string prefabName = Convert.ToString(sheet["MartialArtsIcon"][i]);
					string prefabPath = System.IO.Path.Combine(ASSET_TRAP_RES_CONFIG_FOLDER, prefabName + ".prefab");
					data.MartialArtsIconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

					data.MartialArtsDes = Convert.ToString(sheet["MartialArtsDes"][i]);
					data.MartialArtsMaxScore =  Convert.ToInt32(sheet["MartialArtsMaxScore"][i]);
					data.MartialArtsContribution = Convert.ToInt32(sheet["MartialArtsContribution"][i]);
					data.MartialArtsParamDes = Convert.ToString(sheet["MartialArtsParamDes"][i]);
					data.EffType = Convert.ToByte(sheet["EffType"][i]);
					data.EctypeType = Convert.ToByte(sheet["EctypeType"][i]);
					data.MartialArtsStrengthParam = Convert.ToString(sheet["MartialArtsStrengthParam"][i]);
					data.EctypeBuffID = Convert.ToInt32(sheet["EctypeBuffID"][i]);

					data.ReviveTime = Convert.ToInt32(sheet["ReviveTime"][i]);
					data.FollowerPropParam =  Convert.ToString(sheet["FollowerPropParam"][i]);
					data.MonsSkillReplace = Convert.ToString(sheet["MonsSkillReplace"][i]);
					data.SkillID1 = Convert.ToString(sheet["SkillID1"][i]);
					data.AttExtraAccRate1 = Convert.ToInt32(sheet["AttExtraAccRate1"][i]);
					data.Accid1 = Convert.ToInt32(sheet["Accid1"][i]);
					data.SkillID2 = Convert.ToString(sheet["SkillID2"][i]);
					data.AttExtraAccRate2 = Convert.ToInt32(sheet["AttExtraAccRate2"][i]);

					data.Accid2 = Convert.ToInt32(sheet["Accid2"][i]);
					data.DefAccRate1 =  Convert.ToInt32(sheet["DefAccRate1"][i]);
					data.DefAccid1 = Convert.ToInt32(sheet["DefAccid1"][i]);
					data.DefAccRate2 = Convert.ToInt32(sheet["DefAccRate2"][i]);
					data.DefAccid2 = Convert.ToInt32(sheet["DefAccid2"][i]);
					data.JumpTigRate = Convert.ToInt32(sheet["JumpTigRate"][i]);

					data.QiID = Convert.ToInt32(sheet["QiID"][i]);
					data.AvoidTrgRate =  Convert.ToInt32(sheet["AvoidTrgRate"][i]);
					data.AvoidID = Convert.ToInt32(sheet["AvoidID"][i]);
					data.martialIndex = new MartialIndex(){ MartialArtsID = data.MartialArtsID, MartialArtsLevel = data.MartialArtsLevels};
					tempList.Add(data);
				}

				CreateMartialsDataList(tempList);
			}
		}
	}

	static void CreateMartialsDataList(List<PlayerMartialArtsData> list)
	{
		string fileName = typeof(PlayerMartialArtsData).Name + "DataBase";
		string path = System.IO.Path.Combine(ASSET_MARTIAL_CONFIG_FOLOER, fileName + ".asset");
		
		if(File.Exists(path))
		{
			PlayerMartialArtsDataBase database = (PlayerMartialArtsDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerMartialArtsDataBase));
			
			if(null == database)
			{
				return;
			}
			
			database._dataTable = new PlayerMartialArtsData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			PlayerMartialArtsDataBase database = ScriptableObject.CreateInstance<PlayerMartialArtsDataBase>();
			
			database._dataTable = new PlayerMartialArtsData[list.Count];
			for(int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}

	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach (string file in files)
		{
			if (file.Contains(RESOURCE_MARTIAL_CONFIG_FOLOER))
			{
				fileModified = true;
				break;
			}
		}
		return fileModified;
	}
}
