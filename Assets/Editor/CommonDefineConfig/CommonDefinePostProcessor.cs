using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class CommonDefinePostProcessor : AssetPostprocessor
{

    public static readonly string RESOURCE_ICON_DATA_FOLDER = "Assets/Data/CommonDefine/Res";
    public static readonly string ASSET_ICON_DATA_FOLDER = "Assets/Data/CommonDefine/Data";
    //public static readonly string ASSET_ICON_DATA_FOLDER_PATH = "Assets/Prefab/GUI/ItemIcon/CommonIcon";


    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            OnPostprocessEquipment();
        }

    }

    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_ICON_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }

    private static void OnPostprocessEquipment()
    {

        string path = System.IO.Path.Combine(RESOURCE_ICON_DATA_FOLDER, "CommonDefine.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("Equipment item file not exist");
            return;
        } else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet [keys [0]];

            //List<CommonIconData> tempList = new List<CommonIconData>();
            CommonDefineData data = new CommonDefineData();

            for (int i = 1; i < levelIds.Length; i++)
            {
                if (0 == i)
                    continue;
         
                var keyWords = Convert.ToString(sheet ["KeyWords"] [i]);
         
                switch (keyWords)
                {
                    case "HP_SHORT":
                        data.HP_SHORT = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "HP_LONG":
                        data.HP_LONG = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "SP_SHORT":
                        data.SP_SHORT = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "SP_LONG":
                        data.SP_LONG = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "POWER_MAXCOUNT":
                        data.POWER_MAXCOUNT = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "POWER_VOLUME":
                        data.POWER_VOLUME = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "POWER_REDUCETIME":
                        data.POWER_REDUCETIME = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "COMBO_TIME":
                        data.COMBO_TIME = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_CHANCE":
                        data.DRAIN_CHANCE = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_PERCENT":
                        data.DRAIN_PERCENT = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_MINVECTORX":
                        data.DRAIN_MOVE_MINX = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_MAXVECTORX":
                        data.DRAIN_MOVE_MAXX = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_MINVECTORY":
                        data.DRAIN_MOVE_MINY = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_MAXVECTORY":
                        data.DRAIN_MOVE_MAXY = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_MINVECTORZ":
                        data.DRAIN_MOVE_MINZ = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_MAXVECTORZ":
                        data.DRAIN_MOVE_MAXZ = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_MINTIME":
                        data.DRAIN_MINTIME = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_MAXTIME":
                        data.DRAIN_MAXTIME = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_BALLSPEED":
                        data.DRAIN_BALLSPEED = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "DRAIN_LIMITDISTANCE":
                        data.DRAIN_LIMITDISTANCE = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "THRESH_CONSUME":
                        data.THRESH_CONSUME = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "THRESH_SKILLID1":
                        data.THRESH_SKILLID1 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "THRESH_SKILLID4":
                        data.THRESH_SKILLID4 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "BUFF_CONSUME":
                        data.BUFF_CONSUME = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "BUFF_SKILLID1":
                        data.BUFF_SKILLID1 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "BUFF_SKILLID4":
                        data.BUFF_SKILLID4 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "FATAL_CONSUME":
                        data.FATAL_CONSUME = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "FATAL_SKILLID1":
                        data.FATAL_SKILLID1 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "FATAL_SKILLID4":
                        data.FATAL_SKILLID4 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "SHIELDDAMAGE_IDLE":
                        data.SHIELDDAMAGE_IDLE = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "SHIELDDAMAGE_SKILL":
                        data.SHIELDDAMAGE_SKILL = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "MAPID_PLAYERROOM":
                        data.MAPID_PLAYERROOM = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "TUTORIAL_ECTYPE_ID":
                        data.TUTORIAL_ECTYPE_ID = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ATTACK_CONTINUEPERIOD":
                        data.ATTACK_CONTINUEPERIOD = Convert.ToSingle(sheet ["Volume"] [i]) / 1000.0f;
                        break;
                    case "SKILLVOICE":
                        data.SKILLVOICE = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "TrialsEctype_FreeTime":
                        data.TrialsEctype_FreeTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "TrialsEctype_PayTime":
                        data.TrialsEctype_PayTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "EnergyMax":
                        data.EnergyMax = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "EnergyPayTime":
                        data.EnergyPayTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
					case "EnergyPay":
                        data.EnergyPay = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "EnergyAdd":
                        data.EnergyAdd = Convert.ToInt32(sheet ["Volume"] [i]);
						break;
					case "SweepNumID":
						data.SweepNumID = Convert.ToInt32(sheet ["Volume"] [i]);
						break;
					case "SweepID":
						data.SweepID = Convert.ToInt32(sheet ["Volume"] [i]);
						break;
					case "VitNumID1":
						data.VitNumID1 = Convert.ToInt32(sheet ["Volume"] [i]);
						break;
					case "VitNumID2":
						data.VitNumID2 = Convert.ToInt32(sheet ["Volume"] [i]);
						break;
					case "VitNumID3":
						data.VitNumID3 = Convert.ToInt32(sheet ["Volume"] [i]);
						break;
                    case "BuyEnergyConsumption1":
                        data.BuyEnergyConsumption1 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "BuyEnergyConsumption2":
                        data.BuyEnergyConsumption2 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "BuyEnergyConsumption3":
                        data.BuyEnergyConsumption3 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "BuyEnergyConsumption4":
                        data.BuyEnergyConsumption4 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;

                    case "PackageUnlockConsumption1":
                        data.PackageUnlockConsumption1 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "PackageUnlockConsumption2":
                        data.PackageUnlockConsumption2 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "PackageUnlockConsumption3":
                        data.PackageUnlockConsumption3 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "PackageUnlockConsumption4":
                        data.PackageUnlockConsumption4 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "PVPPayTime":
                        data.PVPPayTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "FruitMannan_FreeTime":
                        data.FruitMannan_FreeTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "FruitMannan_Count":
                        data.FruitMannan_Count = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "FruitMannan_CountMax":
                        data.FruitMannan_CountMax = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "FruitMannan_Pay":
                        data.FruitMannan_Pay = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ItemReturn":
                        data.ItemReturn = Convert.ToSingle(sheet ["Volume"] [i]) / 1000.0f;
                        break;
                    case "ItemMoving":
                        data.ItemMoving = Convert.ToSingle(sheet ["Volume"] [i]) / 1000.0f;
                        break;
                    case "LoadingTransparent":
                        data.LoadingTransparent = Convert.ToSingle(sheet ["Volume"] [i]) / 1000.0f;
                        break;
                    case "LoadingTransparentReturn":
                        data.LoadingTransparentReturn = Convert.ToSingle(sheet ["Volume"] [i]) / 1000.0f;
                        break;
                    case "RoomAwardDisplay":
                        data.RoomAwardDisplay = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "HitNumber_VectorX":
                        data.HitNumber_VectorX = Convert.ToSingle(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "HitNumber_VectorY":
                        data.HitNumber_VectorX = Convert.ToSingle(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "HitNumber_VectorZ":
                        data.HitNumber_VectorX = Convert.ToSingle(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "HitNumberPos_VectorX":
                        data.HitNumberPos_VectorX = StringToFloat(Convert.ToString(sheet ["Volume"] [i]).Split('+'));
                        break;
                    case "HitNumberPos_VectorY":
                        data.HitNumberPos_VectorY = StringToFloat(Convert.ToString(sheet ["Volume"] [i]).Split('+'));
                        break;
                    case "HitNumberPos_VectorZ":
                        data.HitNumberPos_VectorZ = StringToFloat(Convert.ToString(sheet ["Volume"] [i]).Split('+'));
                        break;
                    case "TurnRoundSpeed":
                        data.TurnRoundSpeed = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "GameMoneyDropRadius":
                        data.GameMoneyDropRadius = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_MaxHp":
                        data.Display_MaxHp = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_CurHp":
                        data.Display_CurHp = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_MaxMP":
                        data.Display_MaxMP = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_CurMp":
                        data.Display_CurMp = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_HIT":
                        data.Display_HIT = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_ATK":
                        data.Display_ATK = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_DEF":
                        data.Display_DEF = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_EVA":
                        data.Display_EVA = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_Crit":
                        data.Display_Crit = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_ResCrit":
                        data.Display_ResCrit = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Display_Combat":
                        data.Display_Combat = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "UpgradeAnimationStartLevel":
                        data.UpgradeAnimationStartLevel = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "SkillTipsStartLevel":
                        data.SkillTipsStartLevel = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "EquipmentTipsStartLevel":
                        data.EquipmentTipsStartLevel = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "Critical_CorpseTime":
                        data.Critical_CorpseTime = Convert.ToInt32(sheet ["Volume"] [i]) / 1000f;
                        break;
					case "Normal_CorpseTime":
						data.Normal_CorpseTime = Convert.ToInt32(sheet ["Volume"] [i]) / 1000f;
						break;
                    case "LookingForTeamTime":
                        data.LookingForTeamTime = Convert.ToInt32(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "PlayerRoom_Pay":
                        data.PlayerRoom_Pay = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "PlayerRoom_PayTime":
                        data.PlayerRoom_PayTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "RefiningPrama_Base":
                        data.RefiningPrama_Base = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "RefiningPrama_Level":
                        data.RefiningPrama_Level = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "RefiningPrama_ColorLevel":
                        data.RefiningPrama_ColorLevel = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "RefiningPrama_Discount":
                        data.RefiningPrama_Discount = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "RefiningLevel":
                        data.RefiningLevel = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "KickedOutTeamTime":
                        data.KickedOutTeamTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShortcutItem_Refining":
                        data.ShortcutItem_Refining = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShortcutItem_PassiveSkill":
                        data.ShortcutItem_PassiveSkill = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShortcutItem_Siren":
                        data.ShortcutItem_Siren = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DropItem_Num":
                        string[] dropStr = Convert.ToString(sheet ["Volume"] [i]).Split('+');
                        data.DropItem_Num_A = Convert.ToInt32(dropStr [0]);
                        data.DropItem_Num_B = Convert.ToInt32(dropStr [1]);
                        break;
                    case "Char01WalkSpeed":
                        data.Char01WalkSpeed = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "Char04WalkSpeed":
                        data.Char04WalkSpeed = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "RoleTurnSpeed":
                        data.RoleTurnSpeed = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "VitRecoverTime":
                        data.VitRecoverTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "HeroIcon_Town/Team":
                        string[] getDataStr = Convert.ToString(sheet ["Volume"] [i]).Split('|');
                        List<RoleIconData> getData = new List<RoleIconData>();
                        foreach (var child in getDataStr)
                        {
                            string[] dataItem = child.Split('+');
                            getData.Add(new RoleIconData() {VocationID = int.Parse(dataItem[0]),FashionID = int.Parse(dataItem[1]),ResName = dataItem[2]});
                        }
                        data.HeroIcon_TownAndTeam = getData;
                        break;
                    case "HeroIcon_MailFriend":
                        string[] mailFriendStr = Convert.ToString(sheet ["Volume"] [i]).Split('|');
                        List<RoleIconData> mailFriendData = new List<RoleIconData>();
                        foreach (var child in mailFriendStr)
                        {
                            string[] dataItem = child.Split('+');
                            mailFriendData.Add(new RoleIconData() {VocationID = int.Parse(dataItem[0]),FashionID = int.Parse(dataItem[1]),ResName = dataItem[2]});
                        }
                        data.HeroIcon_MailFriend = mailFriendData;
                        break;
                    case "HeroIcon_Ranking":
                        string[] RankingStr = Convert.ToString(sheet ["Volume"] [i]).Split('|');
                        List<RoleIconData> RankingData = new List<RoleIconData>();
                        foreach (var child in RankingStr)
                        {
                            string[] dataItem = child.Split('+');
                            RankingData.Add(new RoleIconData() {VocationID = int.Parse(dataItem[0]),FashionID = int.Parse(dataItem[1]),ResName = dataItem[2]});
                        }
                        data.HeroIcon_Ranking = RankingData;
                        break;
                    case "HeroIcon_Battle":
                        getDataStr = Convert.ToString(sheet ["Volume"] [i]).Split('|');
                        getData = new List<RoleIconData>();
                        foreach (var child in getDataStr)
                        {
                            string[] dataItem = child.Split('+');
                            getData.Add(new RoleIconData() { VocationID = int.Parse(dataItem[0]), FashionID = int.Parse(dataItem[1]), ResName = dataItem[2] });
                        }
                        data.HeroIcon_Battle = getData;
                        break;
                    case "HeroIcon_BattleReward":
                        getDataStr = Convert.ToString(sheet ["Volume"] [i]).Split('|');
                        getData = new List<RoleIconData>();
                        foreach (var child in getDataStr)
                        {
                            string[] dataItem = child.Split('+');
                            getData.Add(new RoleIconData() { VocationID = int.Parse(dataItem[0]), FashionID = int.Parse(dataItem[1]), ResName = dataItem[2] });
                        }
                        data.HeroIcon_BattleReward = getData;
                        break;
                    case "HeroIcon_SettlementReward":
                        getDataStr = Convert.ToString(sheet ["Volume"] [i]).Split('|');
                        getData = new List<RoleIconData>();
                        foreach (var child in getDataStr)
                        {
                            string[] dataItem = child.Split('+');
                            getData.Add(new RoleIconData() { VocationID = int.Parse(dataItem[0]), FashionID = int.Parse(dataItem[1]), ResName = dataItem[2] });
                        }
                        data.HeroIcon_SettlementReward = getData;
                        break;
                    case "HeroIcon_Town":
                        getDataStr = Convert.ToString(sheet ["Volume"] [i]).Split('|');
                        List<RoleIconDataWithPrefab> IconData = new List<RoleIconDataWithPrefab>();
                        foreach (var child in getDataStr)
                        {
                            string[] dataItem = child.Split('+');
                            string prefabpath = dataItem [2];
                            string pathRes = System.IO.Path.Combine("Assets/Prefab/GUI/IconPrefab/CharHeadIcon", prefabpath + ".prefab");
                            IconData.Add(new RoleIconDataWithPrefab() { VocationID = int.Parse(dataItem[0]), FashionID = int.Parse(dataItem[1]), IconPrefab =(GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject)) });
                        }
                        data.HeroIcon_Town = IconData;
                        break;
                    case "HeroIcon_NPCTalk":
                        getDataStr = Convert.ToString(sheet ["Volume"] [i]).Split('|');
                        var storyTalkGetData = new List<StoryPlayerIconData>();
                        foreach (var child in getDataStr)
                        {
                            string[] dataItem = child.Split('+');
                            storyTalkGetData.Add(new StoryPlayerIconData()
                        {
                            VocationID = int.Parse(dataItem[0]),
                            FashionID = int.Parse(dataItem[1])
                            ,
                            TalkHead = (GameObject)AssetDatabase.LoadAssetAtPath(System.IO.Path.Combine("Assets/Prefab/GUI/IconPrefab/StroyPersonHead", dataItem[2] + ".prefab"), typeof(GameObject))
                        });
                        }
                        data.HeroIcon_NPCTalk = storyTalkGetData;
                        break;
                    case "HeroIcon_Dialogue":
                    
                        getDataStr = Convert.ToString(sheet ["Volume"] [i]).Split('|');
                        List<RoleIconDataWithPrefab> DialogueIconData = new List<RoleIconDataWithPrefab>();
                        foreach (var child in getDataStr)
                        {
                            string[] dataItem = child.Split('+');
                            string prefabpath = dataItem [2];
                            string pathRes = System.IO.Path.Combine("Assets/Prefab/GUI/SirenHead", prefabpath + ".prefab");
                            GameObject iconObj = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));
                            DialogueIconData.Add(new RoleIconDataWithPrefab() { VocationID = int.Parse(dataItem[0]), FashionID = int.Parse(dataItem[1]), IconPrefab =iconObj });
                        }
                        data.HeroIcon_Dialogue = DialogueIconData;

                    
                        break;
                    case "DetectDistance":
                        getDataStr = Convert.ToString(sheet ["Volume"] [i]).Split('+');
                        
                        data.DetectDistance = new float[]
                        {
                            float.Parse(getDataStr [0]),
                            float.Parse(getDataStr [1])
                        };
                        ;
                        break;                  
                    case "SirenTouchOffset":
                        getDataStr = Convert.ToString(sheet ["Volume"] [i]).Split('+');

                        data.SirenTouchOffset = new Vector2(){ x= float.Parse(getDataStr[1]),y= float.Parse(getDataStr[2]) }; 
                        break;
                    case "DropItem_AnimationDelay":
                        data.DropItem_AnimationDelay = Convert.ToInt32(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "DropItem_IntervalTime":
                        data.DropItem_IntervalTime = Convert.ToInt32(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "DropItem_Dis":
                        data.DropItem_Dis = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "GoodsDropAutoTime":
                        data.GoodsDropAutoTime = Convert.ToInt32(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "ItemMsgTimeDisappear":
                        data.ItemMsgTimeDisappear = Convert.ToInt32(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "ItemMsgLimit":
                        data.ItemMsgLimit = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ItemMsgSpeedVertical":
                        data.ItemMsgSpeedVertical = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "ItemMsgSpeedHorizontal":
                        data.ItemMsgSpeedHorizontal = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "ItemMsgTimeHorizontal":
                        data.ItemMsgTimeHorizontal = Convert.ToSingle(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "ItemMsgTimeGap":
                        data.ItemMsgTimeGap = Convert.ToInt32(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "EctypeLoadingWaitingTime":
                        data.EctypeLoadingWaitingTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "HeroIcon_Fashion":
                        data.HeroIcon_Fashion = Convert.ToString(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelTipsAppearTime":
                        data.DefenceLevelTipsAppearTime = Convert.ToInt32(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "Avatar_CenterSpeed":
                        data.Avatar_CenterSpeed = Convert.ToString(sheet ["Volume"] [i]);
                        break;
                    case "Avatar_EdgeSpeed":
                        data.Avatar_EdgeSpeed = Convert.ToString(sheet ["Volume"] [i]);
                        break;
                    case "Avatar_Time":
                        data.Avatar_Time = Convert.ToInt32(sheet ["Volume"] [i]) / 1000f;
                        break;
                    case "MailLimit":
                        data.MailLimit = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;

                    case "DefenceLevelStartTipDelay":
                        data.DefenceLevelStartTipDelay = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelTipsCoolDown":
                        data.DefenceLevelTipsCoolDown = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelSpecialSkill":
                        data.DefenceLevelSpecialSkill = Convert.ToString(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelSpecialSkillTips":
                        data.DefenceLevelSpecialSkillTips = Convert.ToString(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelJingYanDailyLimit":
                        data.DefenceLevelJingYanDailyLimit = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelTongBiDailyLimit":
                        data.DefenceLevelTongBiDailyLimit = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelYuanBaoDailyLimit":
                        data.DefenceLevelYuanBaoDailyLimit = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelJudgeGateHPLeft":
                        data.DefenceLevelJudgeGateHPLeft = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelJudgeHitPointLeft":
                        data.DefenceLevelJudgeHitPointLeft = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelJudgeBeHitParam1":
                        data.DefenceLevelJudgeBeHitParam1 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelJudgeBeHitParam2":
                        data.DefenceLevelJudgeBeHitParam2 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelJudgeThreshold1":
                        data.DefenceLevelJudgeThreshold1 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DefenceLevelJudgeThreshold2":
                        data.DefenceLevelJudgeThreshold2 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "EndlessDailyLimit":
                        data.EndlessDailyLimit = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "EndlessNextWaveTime":
                        data.EndlessNextWaveTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "EndlessWaveSkipTimeParam":
                        data.EndlessWaveSkipTimeParam = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "EndlessSingleMassageShowTime":
                        data.EndlessSingleMassageShowTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "EndlessMassageShowTimePlus":
                        data.EndlessMassageShowTimePlus = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;

                    case "LotteryMultipleNum":
                        data.LotteryMultipleNum = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "LotteryOneCost":
                        data.LotteryOneCost = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "LotteryTenCost":
                        data.LotteryTenCost = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "LotteryOneDiscount":
                        data.LotteryOneDiscount = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "LotteryTenDiscount":
                        data.LotteryTenDiscount = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "LotteryVelocity":
                        data.LotteryVelocity = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "LotteryAcceleration":
                        data.LotteryAcceleration = Convert.ToSingle(sheet ["Volume"] [i]);
                        break;
                    case "LotteryThrowTime":
                        data.LotteryThrowTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "SignInConsumption":
                        data.SignInConsumption = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "Coop_DailyLimit":
                        data.Coop_DailyLimit = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "SendMailConsumption":
                        data.SendMailConsumption = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "DialogSpeed":
                        data.DialogSpeed = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "TownstartPoint1":
                    {
                        string str = Convert.ToString(sheet ["Volume"] [i]);
                        string[] strlist = str.Remove(0, 2).Split('|');
                        string[]  posStrList=strlist[0].Split('+');
                        string[]  offsetStrList=strlist[1].Split('+');
                        string[]  DirStrList=strlist[2].Split('+');
                        data.TownstartPoint1 =new TownBtnStartPoint()
                        {
                            BasePostion=new Vector2(float.Parse(posStrList [0]),float.Parse(posStrList [1])),
                            BaseOffset=new Vector2(float.Parse(offsetStrList [0]),float.Parse(offsetStrList [1])),
                            Direction=new Vector2(int.Parse(DirStrList [0]),int.Parse(DirStrList [1]))
                        };
                    }
                        break;
                    case "TownstartPoint2":
                    {
                        string str = Convert.ToString(sheet ["Volume"] [i]);
                        string[] strlist = str.Remove(0, 2).Split('|');
                        string[]  posStrList=strlist[0].Split('+');
                        string[]  offsetStrList=strlist[1].Split('+');
                        string[]  DirStrList=strlist[2].Split('+');
                        data.TownstartPoint2 =new TownBtnStartPoint()
                        {
                            BasePostion=new Vector2(float.Parse(posStrList [0]),float.Parse(posStrList [1])),
                            BaseOffset=new Vector2(float.Parse(offsetStrList [0]),float.Parse(offsetStrList [1])),
                            Direction=new Vector2(float.Parse(DirStrList [0]),float.Parse(DirStrList [1]))
                        };
                    }
                        break;
                    case "TownstartPoint3":
                    {
                        string str = Convert.ToString(sheet ["Volume"] [i]);
                        string[] strlist = str.Remove(0, 2).Split('|');
                        string[]  posStrList=strlist[0].Split('+');
                        string[]  offsetStrList=strlist[1].Split('+');
                        string[]  DirStrList=strlist[2].Split('+');
                        data.TownstartPoint3 =new TownBtnStartPoint()
                        {
                            BasePostion=new Vector2(float.Parse(posStrList [0]),float.Parse(posStrList [1])),
                            BaseOffset=new Vector2(float.Parse(offsetStrList [0]),float.Parse(offsetStrList [1])),
                            Direction=new Vector2(float.Parse(DirStrList [0]),float.Parse(DirStrList [1]))
                        };
                    }
                        break;
                    case "AuctionDefaultTime":
                        data.AuctionDefaultTime = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "AuctionBidRate":
                        data.AuctionBidRate = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "AuctionTopBid":
                        data.AuctionTopBid = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "WorldChatItem":
                        data.WorldChatItem = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShopSlotMaxNum":
                        data.ShopSlotMaxNum = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShopChangeCost1":
                        data.ShopChangeCost1 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
					case "ShopChangeCost2":
						data.ShopChangeCost2 = Convert.ToInt32(sheet ["Volume"] [i]);
						break;
					case "ShopChangeCost3":
						data.ShopChangeCost3 = Convert.ToInt32(sheet ["Volume"] [i]);
						break;
					case "ShopChangeCost4":
						data.ShopChangeCost4 = Convert.ToInt32(sheet ["Volume"] [i]);
						break;
                    case "ShopSlotUnlockCost5":
                        data.ShopSlotUnlockCost5 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShopSlotUnlockCost6":
                        data.ShopSlotUnlockCost6 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShopSlotUnlockCost7":
                        data.ShopSlotUnlockCost7 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShopSlotUnlockCost8":
                        data.ShopSlotUnlockCost8 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShopSlotUnlockCost9":
                        data.ShopSlotUnlockCost9 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShopSlotUnlockCost10":
                        data.ShopSlotUnlockCost10 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShopSlotUnlockCost11":
                        data.ShopSlotUnlockCost11 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "ShopSlotUnlockCost12":
                        data.ShopSlotUnlockCost12 = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "QuickBuyCopperCoin":
                        data.QuickBuyCopperCoin = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
                    case "QuickBuyWorldChatItem":
                        data.QuickBuyWorldChatItem = Convert.ToInt32(sheet ["Volume"] [i]);
                        break;
					case "PopUpBoxUnlockLevel":
					{
						string levelStr = Convert.ToString(sheet ["Volume"] [i]);
						string[] levelArray = levelStr.Split('|');
						foreach(string lev in levelArray)
						{
							string[] levView = lev.Split('+');
							data.levelComeInTown.Add(int.Parse(levView[0]));
							data.viewComeInTown.Add(int.Parse(levView[1]));
						}
					}
					break; 
					case "Coop_CostItemShop1":
						data.Coop_CostItemShop1 = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "Coop_CostItemShop2":
						data.Coop_CostItemShop2 = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "Coop_CostItemShop3":
						data.Coop_CostItemShop3 = Convert.ToInt32(sheet["Volume"][i]);
					break;
                    case "DodgePara":
                        data.DodgePara = Convert.ToInt32(sheet["Volume"][i]);
                        break;
                    case "TownNamePos":
                        data.TownNamePos= Convert.ToSingle(sheet["Volume"][i]);
                        break;
                    case "BattleNamePos":
                        data.BattleNamePos= Convert.ToSingle(sheet["Volume"][i]);
                        break;
                    case "DefaultStroy":
                        data.DefaultStroy = Convert.ToInt32(sheet["Volume"][i]);
                        break;
					case "FriendRequestLimit":
						data.FriendRequestLimit = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "Coop_FreeTimes":
						data.Coop_FreeTimes = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "GameStarTime":
						data.gameStarTime = Convert.ToSingle(sheet["Volume"][i]);
						break;
					case "BornStoryID_Char01":
						data.createClass1StoryLineID = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "BornStoryID_Char04":
						data.createClass4StoryLineID = Convert.ToInt32(sheet["Volume"][i]);
                        break;
                    case "SearchAngle":
                        data.SearchAngle = Convert.ToSingle(sheet["Volume"][i]);
                        break;                        
                    case "SearchRange":
                        data.SearchRadius = Convert.ToInt32(sheet["Volume"][i]);
                        break;
					case "SearchMinRange":
						data.SearchMinRange = Convert.ToInt32(sheet["Volume"][i]);
					break;
					case "SearchAreaAngle":
						data.SearchAreaAngle = Convert.ToSingle(sheet["Volume"][i]);
						break;
                    case "SearchFrq":
                        data.SearchFrq = Convert.ToInt32(sheet["Volume"][i]);
                        break;
                    case "ButtonmemTime":
                        data.ButtonMemTime = Convert.ToInt32(sheet["Volume"][i]);
					break;
					case "Ping_Space":
						data.PingDelayTime = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "MaxLoadingTime":
						data.MaxLoadingTime = Convert.ToInt32(sheet["Volume"][i]);
						break;
                    case "ChargeMinRange":
                        data.ChargeMinRange = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "ChatBoxPosX":
						data.ChatBoxPosX = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "ChatBoxPosY":
						data.ChatBoxPosY = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "TownRobotNum":
						data.TownRobotNum = Convert.ToInt32(sheet["Volume"][i]);
						break;
                    case "ChargeOffset":
                        data.ChargeOffset = Convert.ToInt32(sheet["Volume"][i]);
                        break;
                    case "AddBuff_BornTime":
                        data.AddBuff_BornTime = Convert.ToSingle(sheet["Volume"][i]);
                        break;
					case "SkillComboTime":
						data.SkillComboTime = Convert.ToSingle(sheet["Volume"][i]);
						break;
					case "BossDef_WeakTime":
						data.BossDef_WeakTime = Convert.ToSingle(sheet["Volume"][i]);
						break;
					case "AutoPickup_Time":
						data.AutoPickup_Time = Convert.ToSingle(sheet["Volume"][i]);
						break;
					case "AutoPickup_Speed":
						data.AutoPickup_Speed = Convert.ToSingle(sheet["Volume"][i]);
						break;
                    case "BuyFruitMannanConsumption1":
                        data.BuyFruitMannanConsumption1 = Convert.ToInt32(sheet["Volume"][i]);
                        break;
                    case "BuyFruitMannanConsumption2":
                        data.BuyFruitMannanConsumption2 = Convert.ToInt32(sheet["Volume"][i]);
                        break;
                    case "BuyFruitMannanConsumption3":
                        data.BuyFruitMannanConsumption3 = Convert.ToInt32(sheet["Volume"][i]);
                        break;
                    case "BuyFruitMannanConsumption4":
                        data.BuyFruitMannanConsumption4 = Convert.ToInt32(sheet["Volume"][i]);
                        break;
					case "FastSelectQuality":
						data.FastSelectQuality = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "Match_Delay":
						data.Match_Delay = Convert.ToSingle(sheet["Volume"][i]);
						break;
					case "ButtonWeakTipsLevel":
						data.ButtonWeakTipsLevel = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "Lose_BeShow":
						data.Lose_BeShow = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "ShopChangeNum":
						data.ShopChangeNum = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "DefaultSirenSkill1":
						data.DefaultSirenSkill1 = Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "DefaultSirenSkill4":
						data.DefaultSirenSkill4 = Convert.ToInt32(sheet["Volume"][i]);
						break;

                    case "AttackButtonDelay":
                        data.AttackButtonDelay = Convert.ToSingle(sheet["Volume"][i])/1000.0f;
                        break;

                    case "HurtEffectNumber":
                        data.HurtEffectNumber = Convert.ToInt32(sheet["Volume"][i]);
                        break;
                    case "GameMoneyAbridge":
                        data.GameMoneyAbridge = Convert.ToInt32(sheet["Volume"][i]);
                        break;

                    case "NoneFreezeIronLevel":
                        data.NoneFreezeIronLevel = Convert.ToInt32(sheet["Volume"][i]);
					break;
					case "StrengthLimit":
						data.StrengthLimit=Convert.ToInt32(sheet["Volume"][i]);
                        break;
					case "StartStrengthLimit":
						data.StartStrengthLimit=Convert.ToInt32(sheet["Volume"][i]);
						break;
					case "PVPBattleWinIcon":
						{
							string IconStr = Convert.ToString(sheet ["Volume"] [i]);
							data.PVPBattleWinIcon = IconStr;
							string[] strList = IconStr.Split('+');
							List<GameObject> iconPrefab = new List<GameObject>();
							foreach(string str in strList)
							{
								string pathRes = System.IO.Path.Combine("Assets/Prefab/GUI/IconPrefab/PVGAwardWin", str + ".prefab");
								iconPrefab.Add((GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject)));
							}
							data.PVPBattleWinIconPrefab = iconPrefab;
						}
						break;
					case "HeroIcon_PVPLoading":
					{
						string IconStr = Convert.ToString(sheet ["Volume"] [i]);
						data.HeroIcon_PVPLoading = IconStr;
						string[] strList = IconStr.Split('|');
						data.pvpLoadingIcon = new List<RoleIconData>();
						foreach(string str in strList)
						{
							RoleIconData roleData = new RoleIconData();
							string[] strArray = str.Split('+');
							roleData.VocationID = int.Parse(strArray[0]);
							roleData.FashionID = int.Parse(strArray[1]);
							roleData.ResName = strArray[2];
							data.pvpLoadingIcon.Add(roleData);
						}
					}
						break;
					case "DropItem_RadiusParam":
					{
						data.DropItem_RadiusParam = Convert.ToSingle(sheet["Volume"][i]);

					}
						break;
				default:
                        break;
                }                
                for (int cameraNum = 1; cameraNum <= 8; cameraNum++)
                {
                    if (keyWords == "Camera0" + cameraNum.ToString() + "Distance")
                    {
                        string cameraPosStr = Convert.ToString(sheet ["Volume"] [i]);
                        string[] posArray = cameraPosStr.Split('+');
                        data.CameraDistanceList.Add(new Vector3(Convert.ToSingle(posArray [1]), Convert.ToSingle(posArray [2]), Convert.ToSingle(posArray [3])));
                    }
                    if (keyWords == "TCamera0" + cameraNum.ToString() + "Distance")
                    {
                        string cameraPosStr = Convert.ToString(sheet ["Volume"] [i]);
                        string[] posArray = cameraPosStr.Split('+');
                        data.CameraDistanceTownList.Add(new Vector3(Convert.ToSingle(posArray [1]), Convert.ToSingle(posArray [2]), Convert.ToSingle(posArray [3])));

                    }


                    if (keyWords == "Camera0" + cameraNum.ToString() + "BarrierDistance")
                    {
                        float distance = Convert.ToSingle(sheet ["Volume"] [i]);
                        data.CameraBarrierDistanceList.Add(distance);
                    }
                }
                for (int bornDialoguePositionNum = 1; bornDialoguePositionNum<=10; bornDialoguePositionNum++)
                {
                    if (keyWords == "BornDialoguePosition" + bornDialoguePositionNum.ToString())
                    {
                        string[] dialogPosStr = Convert.ToString(sheet ["Volume"] [i]).Split('+');
                        Vector2 v2 = new Vector2(Convert.ToSingle(dialogPosStr [1]), Convert.ToSingle(dialogPosStr [2]));
                        data.BornDialoguePositionList.Add(v2);
                    }
                }

                 
            }

            CreateMedicamentConfigDataList(data);
        }

    }

    static float[] StringToFloat(string[] str)
    {
        List<float> getFloat = new List<float>();
        str.ApplyAllItem(P => getFloat.Add(float.Parse(P) / 1000f));
        return getFloat.ToArray();
    }

    static void CreateMedicamentConfigDataList(CommonDefineData item)
    {
        string fileName = typeof(CommonDefineData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_ICON_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            CommonDefineDataBase database = (CommonDefineDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(CommonDefineDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = item;

            EditorUtility.SetDirty(database);
        } else
        {
            CommonDefineDataBase database = ScriptableObject.CreateInstance<CommonDefineDataBase>();

            database._dataTable = item;

            AssetDatabase.CreateAsset(database, path);
        }
    }
}
