using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class EctypeContainerConfig : AssetPostprocessor 
{

    public static readonly string RESOURCE_ECTYPE_DATA_FOLDER = "Assets/Data/EctypeConfig/Res";
    public static readonly string ASSET_ECTYPE_DATA_FOLDER = "Assets/Data/EctypeConfig/Data";
    public static readonly string IconAssetPath = "Assets/Prefab/GUI/IconPrefab/EctypeIcon";


    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            OnPostprocessEquipment();
        }

    }

    private static void OnPostprocessEquipment()
    {

        string path = System.IO.Path.Combine(RESOURCE_ECTYPE_DATA_FOLDER, "EctypeContainer.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("Equipment item file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<EctypeContainerData> tempList = new List<EctypeContainerData>();
            List<EctypeContainerResData> tempResList = new List<EctypeContainerResData>();
            List<EctypeContainerIconData> iconTempList = new List<EctypeContainerIconData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                EctypeContainerData data = new EctypeContainerData();
                EctypeContainerIconData iconData = new EctypeContainerIconData();
                //data.m_SzName = Convert.ToString(sheet["szName"][i]);
                //data.m_IEquipmentID = Convert.ToInt32(sheet["lEquimentID"][i]);
                data.lEctypeContainerID = Convert.ToInt32(sheet["lEctypeContainerID"][i]);
                data.lEctypeName = Convert.ToString(sheet["lEctypeName"][i]);
                data.lDifficulty = Convert.ToInt32(sheet["lDifficulty"][i]);
                data.lEctypePos = Convert.ToString(sheet["EctypePos"][i]).Split('+');
                data.lEctypeIcon = Convert.ToString(sheet["lEctypeIcon"][i]);


                data.vectMapID = Convert.ToString(sheet["vectMapID"][i]);
                data.lAllLoadFlag = Convert.ToInt32(sheet["lAllLoadFlag"][i]);
                data.lEctypeType = Convert.ToInt32(sheet["lEctypeType"][i]);
                data.lEctypeMode = Convert.ToInt32(sheet["lEctypeMode"][i]);
                data.lMinActorCount = Convert.ToInt32(sheet["lMinActorCount"][i]);
                data.lMaxActorCount = Convert.ToInt32(sheet["lMaxActorCount"][i]);
                data.lMinActorLevel = Convert.ToInt32(sheet["lMinActorLevel"][i]);
                data.lMaxActorLevel = Convert.ToInt32(sheet["lMaxActorLevel"][i]);
                string costStr = Convert.ToString(sheet["lCostEnergy"][i]);
                string[] splitCostStr = costStr.Split("+".ToCharArray());
                data.lCostType = Convert.ToInt32(splitCostStr[0]);
                data.lCostEnergy = Convert.ToString(splitCostStr[1]);

                
                data.lOutTime = Convert.ToInt32(sheet["lOutTime"][i]);
                data.lDayEnterTimes = Convert.ToInt32(sheet["lDayEnterTimes"][i]);
                data.lWeekEnterTimes = Convert.ToInt32(sheet["lWeekEnterTimes"][i]);
                data.vectChunnelID = Convert.ToString(sheet["vectChunnelID"][i]);
                data.DestDir = Convert.ToInt32(sheet["DestDir"][i]);
                data.lRadius = Convert.ToInt32(sheet["lRadius"][i]);
                data.lExperience = Convert.ToInt32(sheet["lExperience"][i]);
                data.lMoney = Convert.ToInt32(sheet["lMoney"][i]);
                data.DropInf = Convert.ToString(sheet["DropInf"][i]);
                data.dwBasicWinTime = Convert.ToInt32(sheet["dwBasicWinTime"][i]);
                data.dwBasicHitPoint = Convert.ToInt32(sheet["dwBasicHitPoint"][i]);
                data.BasicBeHit = Convert.ToInt32(sheet["BasicBeHit"][i]);
                data.MapType = Convert.ToInt32(sheet["lEctypeType"][i]);
                //data.lPropAwardDesc = Convert.ToString(sheet["lPropAwardDesc"][i]);
                //data.vectAwardID = Convert.ToInt32(sheet["vectAwardID"][i]);
                //data.vectAwardRate = Convert.ToInt32(sheet["vectAwardRate"][i]);
                //data.vectGoldTreasureID = Convert.ToInt32(sheet["vectGoldTreasureID"][i]);
                //data.vectGoldTresureRate = Convert.ToInt32(sheet["vectGoldTresureRate"][i]);
                data.wDelockLev = Convert.ToInt32(sheet["wDelockLev"][i]);
                //data.dwDelockTargetID = Convert.ToInt32(sheet["dwDelockTargetID"][i]); ;

                data.BossHead = Convert.ToString(sheet["BossHead"][i]);
                data.BossLifeLayer = Convert.ToInt32(sheet["BossLifeLayer"][i]);
                string[] CostType = Convert.ToString(sheet["byCostType"][i]).Split('+');
                data.ByCostType = int.Parse(CostType[0]);
                data.ByCost = int.Parse(CostType[1]);
                string bossStr = Convert.ToString(sheet["BossId"][i]);
                string[] bossIDs = bossStr.Split("+".ToCharArray());
                int bossNum = bossIDs.Length;
                data.BossIDs = new int[bossNum];
                for (int j = 0; j < bossNum; j++)
                {
                    data.BossIDs[j] = Convert.ToInt32(bossIDs[j]);
                }
                data.ComboValue = Convert.ToInt32(sheet["ComboValue"][i]);
                data.TrialsAward = Convert.ToString(sheet["TrialsAward"][i]);
                data.PlayerNum = Convert.ToInt32(sheet["PlayerNum"][i]);
                //
                //data.EctypeIconTexture = (Texture)Resources.LoadAssetAtPath("Assets/UI/Textures/Town/" + data.lEctypeIcon + ".png", typeof(Texture));

                string[] skillHideStr = Convert.ToString(sheet["PowerSkill_Hide"][i]).Split('+');
                data.PowerSkillHide = new int[skillHideStr.Length];
                for ( int j = 0; j < skillHideStr.Length; j++ )
                {
                    data.PowerSkillHide[j] = Convert.ToInt32(skillHideStr[j]);
                }
               
                data.bossAppearanceWord = Convert.ToString(sheet["BossAppearanceWord"][i]);
                data.bossAppearanceSound = Convert.ToString(sheet["BossAppearanceSound"][i]);

                string[] roleUpanishadsStr = Convert.ToString(sheet["Upanishads"][i]).Split('|');
                data.RoleUpanishads = new RoleUpanishads[roleUpanishadsStr.Length];
                for (int j = 0; j < roleUpanishadsStr.Length; j++)
                {
                    data.RoleUpanishads[j] = new RoleUpanishads();
                    string[] roleItem = roleUpanishadsStr[j].Split('+');

                    if (roleItem.Length == 2)
                    {
                        data.RoleUpanishads[j].Vocation = Convert.ToInt32(roleItem[0]);
                        data.RoleUpanishads[j].UpanishadId = Convert.ToInt32(roleItem[1]);
                    }
                }

                string[] startSkillStr = Convert.ToString(sheet["StartSkill"][i]).Split('|') ;
                data.StartSkills = new StartSkill[startSkillStr.Length];
                for (int skill = 0; skill < startSkillStr.Length; skill++)
                {
                    data.StartSkills[skill] = new StartSkill();
                    string[] startSkillItem = startSkillStr[skill].Split('+');
                    data.StartSkills[skill].Vocation = Convert.ToInt32(startSkillItem[0]);
                    data.StartSkills[skill].SkillID = Convert.ToInt32(startSkillItem[1]);
                }
                data.BattleVictoryLotteryTime = Convert.ToInt32(sheet["BattleVictoryLotteryTime"][i]);
                data.BattleFailTime = Convert.ToInt32(sheet["BattleFailTime"][i]);
				string[] dropListItemStr = Convert.ToString(sheet["DropListItem"][i]).Split('+');
				data.DropListItem = new List<int>();
				dropListItemStr.ApplyAllItem(c=>data.DropListItem.Add(int.Parse(c)));
				data.EctypeBossDescription = Convert.ToString(sheet["EctypeBossDescription"][i]);
				data.EctypeDescription = Convert.ToString(sheet["EctypeDescription"][i]);
				data.defenceLevel = Convert.ToString(sheet["DefenceLevel_Block"][i]);

				//defenceLevel_Block.ApplyAllItem(P=>Debug.Log(P));
				//data.DefenceLevel_Block=new int[3]{int.Parse(defenceLevel_Block[0]),int.Parse(defenceLevel_Block[1]),int.Parse(defenceLevel_Block[2])};
				data.DefenceLevelLoot=Convert.ToString(sheet["DefenceLevelLoot"][i]).Split('+');

				data.FightingCapacity = Convert.ToInt32(sheet["FightingCapacity"][i]);
				data.SirenSkillVaule = Convert.ToInt32(sheet["SirenSkillVaule"][i]);
				data.CanUseMedicament = Convert.ToInt32(sheet["CanUseMedicament"][i])==1?true:false;
				string[] freeMedicamentStr = Convert.ToString(sheet["FreeMedicament"][i]).Split('|');
				int freeStrLength = freeMedicamentStr.Length;
				data.FreeMedicaments = new FreeMedicament[freeStrLength];
				for(int p = 0; p < freeStrLength; p++)
				{
					string[] freeStrs = freeMedicamentStr[p].Split('+');
					data.FreeMedicaments[p] = new FreeMedicament();
					data.FreeMedicaments[p].VipLevel = Convert.ToInt32(freeStrs[0]);
					data.FreeMedicaments[p].Num = Convert.ToInt32(freeStrs[1]);
				}

				string[] medicamentIDStr = Convert.ToString(sheet["MedicamentID"][i]).Split('|');
				int IDStrLength = medicamentIDStr.Length;
				data.MedicamentIDs = new MedicamentID[IDStrLength];
				for(int p =0;p<IDStrLength;p++)
				{
					string[] idStrs = medicamentIDStr[p].Split('+');
					data.MedicamentIDs[p] = new MedicamentID();
					data.MedicamentIDs[p].VipLevel = Convert.ToInt32(idStrs[0]);
					data.MedicamentIDs[p].GoodsID = Convert.ToInt32(idStrs[1]);
				}
				//data.MedicamentPrice = Convert.ToString(sheet["MedicamentPrice"][i]);
				string[] medicamentPriceStr = Convert.ToString(sheet["MedicamentPrice"][i]).Split('+');
				int priceStrLength = medicamentPriceStr.Length;
				data.MedicamentPrice = new MedicamentPrice();
				for(int p=0;p<priceStrLength;p++)
				{
					data.MedicamentPrice.GoodsID = Convert.ToInt32(medicamentPriceStr[0]);
					data.MedicamentPrice.Param1 = Convert.ToInt32(medicamentPriceStr[1]);						
					data.MedicamentPrice.Param2 = Convert.ToInt32(medicamentPriceStr[2]);						
					data.MedicamentPrice.Param3 = Convert.ToInt32(medicamentPriceStr[3]);						
					data.MedicamentPrice.Param4 = Convert.ToInt32(medicamentPriceStr[4]);						
				}
				string[] buffIDStr = Convert.ToString(sheet["MedicamentBuffID"][i]).Split('|');
				int buffIDLength = buffIDStr.Length;
				data.MedicamentBuffIDs = new MedicamentBuffID[buffIDLength];
				for(int p=0;p<buffIDLength;p++)
				{
					string[] str = buffIDStr[p].Split('+');
					data.MedicamentBuffIDs[p] = new MedicamentBuffID();
					data.MedicamentBuffIDs[p].VipLevel = Convert.ToInt32(str[0]);
					data.MedicamentBuffIDs[p].BuffID = Convert.ToInt32(str[1]);
					data.MedicamentBuffIDs[p].BuffLevel = Convert.ToInt32(str[2]);
					data.MedicamentBuffIDs[p].ColdID = Convert.ToInt32(str[3]);
				}

				data.ReviveType = Convert.ToInt32(sheet["ReviveType"][i]);
				string[] reviveNumStr = Convert.ToString(sheet["ReviveNum"][i]).Split('|');
				int reviveNumLength = reviveNumStr.Length;
				data.ReviveNums = new ReviveNum[reviveNumLength];
				for(int p=0;p<reviveNumLength;p++)
				{
					string[] strs = reviveNumStr[p].Split('+');
					data.ReviveNums[p] = new ReviveNum();
					data.ReviveNums[p].VipLevel = Convert.ToInt32(strs[0]);
					data.ReviveNums[p].Num = Convert.ToInt32(strs[1]);
				}
				data.ReviveTime = Convert.ToInt32(sheet["ReviveTime"][i]);

				string[] simpleRevivePriceStr = Convert.ToString(sheet["SimpleRevivePrice"][i]).Split('+');
				int simpleRevivePriceLength = simpleRevivePriceStr.Length;
				data.SimpleRevivePrice = new SimpleRevivePrice();
				for(int p=0;p<simpleRevivePriceLength;p++)
				{
					data.SimpleRevivePrice.GoodsID = Convert.ToInt32(simpleRevivePriceStr[0]);
					data.SimpleRevivePrice.Parma1 = Convert.ToInt32(simpleRevivePriceStr[1]);
					data.SimpleRevivePrice.Parma2 = Convert.ToInt32(simpleRevivePriceStr[2]);
					data.SimpleRevivePrice.Parma3 = Convert.ToInt32(simpleRevivePriceStr[3]);
					data.SimpleRevivePrice.Parma4 = Convert.ToInt32(simpleRevivePriceStr[4]);
				}
				string[] pefectRevivePriceStr = Convert.ToString(sheet["PefectRevivePrice"][i]).Split('+');
				int pefectRevivePriceLength = pefectRevivePriceStr.Length;
				data.PefectRevivePrice = new PefectRevivePrice();
				for(int p=0;p<pefectRevivePriceLength;p++)
				{
					data.PefectRevivePrice.GoodsID = Convert.ToInt32(pefectRevivePriceStr[0]);
					data.PefectRevivePrice.Parma1 = Convert.ToInt32(pefectRevivePriceStr[1]);
					data.PefectRevivePrice.Parma2 = Convert.ToInt32(pefectRevivePriceStr[2]);
					data.PefectRevivePrice.Parma3 = Convert.ToInt32(pefectRevivePriceStr[3]);
					data.PefectRevivePrice.Parma4 = Convert.ToInt32(pefectRevivePriceStr[4]);
				}

				data.GateHPRemain = Convert.ToInt32(sheet["GateHPRemain"][i]);
				data.Coop_IsItemQuikBuy = Convert.ToInt32(sheet["Coop_IsItemQuikBuy"][i]) == 1;
				data.Coop_DailyLimit = Convert.ToInt32(sheet["Coop_DailyLimit"][i]);
				data.Coop_Solo = Convert.ToInt32(sheet["Coop_Solo"][i]);
				string[] coopItemCostStr = Convert.ToString(sheet["Coop_ItemCost"][i]).Split('+');
				data.Coop_ItemCost_GoodsID = Convert.ToInt32(coopItemCostStr[0]);
				data.Coop_ItemCost_GoodsNum = Convert.ToInt32(coopItemCostStr[1]);
				string[] bonusTimeStr = Convert.ToString(sheet["Coop_BonusTime"][i]).Split('+');
				data.Coop_BonusTime = new int[bonusTimeStr.Length];
				for(int j = 0;j<bonusTimeStr.Length;j++)
				{
					data.Coop_BonusTime[j] = Convert.ToInt32(bonusTimeStr[j]);
				}
				data.AllowCreatTeam = Convert.ToInt32(sheet["AllowCreatTeam"][i]);
				if(sheet.ContainsKey("ResultAppearDelay"))
				{
					data.ResultAppearDelay = Convert.ToInt32(sheet["ResultAppearDelay"][i]);
				}
				data.PickupDelay = Convert.ToSingle(sheet["PickupDelay"][i]);
                tempList.Add(data);
				data.IsMOBA = Convert.ToInt32(sheet["IsMOBA"][i]);
                EctypeContainerResData resData = new EctypeContainerResData();
                resData.lEctypeContainerID = data.lEctypeContainerID;
                string bossHead = Convert.ToString(sheet["BossHead"][i]);
                resData.bossHead = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/GUI/SirenHead/" + bossHead + ".prefab", typeof(GameObject));

				string[] bossHeadStr = bossHead.Split('+');
				if(bossIDs.Length != bossHeadStr.Length)
				{
					Debug.LogError("BossId字段中的BOSS数与BossHead字段中的美术资源数对不上");
				}
				else
				{
					resData.BossHeadReses = new EctypeContainerBossHeadRes[bossHeadStr.Length];
					for(int idIndex = 0;idIndex<bossHeadStr.Length;idIndex++)
					{
						resData.BossHeadReses[idIndex] = new EctypeContainerBossHeadRes();
						resData.BossHeadReses[idIndex].BossHeadID = data.BossIDs[idIndex];
						resData.BossHeadReses[idIndex].BossHeadGO = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/GUI/SirenHead/" + bossHeadStr[idIndex] + ".prefab", typeof(GameObject));
					}
				}

				string bossAppearancePortrait = Convert.ToString(sheet["BossAppearancePortrait"][i]);
                resData.bossAppearancePortrait = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/GUI/SirenHead/" + bossAppearancePortrait + ".prefab", typeof(GameObject));

                tempResList.Add(resData);


                iconData.lEctypeContainerID = data.lEctypeContainerID;
                iconData.lEctypeName = data.lEctypeName;
                iconData.lDifficulty = data.lDifficulty;
                string pathRes = System.IO.Path.Combine(IconAssetPath, data.lEctypeIcon + ".prefab");
                iconData.EctypeIconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));


                iconTempList.Add(iconData);
            }

            CreateMedicamentConfigDataList(tempList);
            CreateMedicamentConfigDataList(tempResList);
            CreateMedicamentConfigDataList(iconTempList);
        }

    }


    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_ECTYPE_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    static void CreateMedicamentConfigDataList(List<EctypeContainerData> list)
    {
        string fileName = typeof(EctypeContainerData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_ECTYPE_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            EctypeContainerDataList database = (EctypeContainerDataList)AssetDatabase.LoadAssetAtPath(path, typeof(EctypeContainerDataList));

            if (null == database)
            {
                return;
            }

            database.ectypeContainerDataList = new EctypeContainerData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.ectypeContainerDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            EctypeContainerDataList database = ScriptableObject.CreateInstance<EctypeContainerDataList>();
            database.ectypeContainerDataList = new EctypeContainerData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.ectypeContainerDataList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

    static void CreateMedicamentConfigDataList(List<EctypeContainerResData> list)
    {
        string fileName = typeof(EctypeContainerResData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_ECTYPE_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            EctypeContainerResDataBase database = (EctypeContainerResDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(EctypeContainerResDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new EctypeContainerResData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            EctypeContainerResDataBase database = ScriptableObject.CreateInstance<EctypeContainerResDataBase>();
            database._dataTable = new EctypeContainerResData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

    static void CreateMedicamentConfigDataList(List<EctypeContainerIconData> list)
    {
        string fileName = typeof(EctypeContainerIconData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_ECTYPE_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            EctypeContainerIconPrefabDataBase database = (EctypeContainerIconPrefabDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(EctypeContainerIconPrefabDataBase));

            if (null == database)
            {
                return;
            }

            database.iconDataList = new EctypeContainerIconData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.iconDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            EctypeContainerIconPrefabDataBase database = ScriptableObject.CreateInstance<EctypeContainerIconPrefabDataBase>();
            database.iconDataList = new EctypeContainerIconData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.iconDataList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
