using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;
using System.Linq;

public class PlayerDataManager : MonoBehaviour {

    public PlayerGenerateConfigDataBase PlayerGenerateConfigData_Town;
    public PlayerGenerateConfigDataBase PlayerGenerateConfigData_Battle;
    public PlayerGenerateConfigDataBase PlayerGenerateConfigData_UI;
    public PlayerGenerateConfigDataBase PlayerGenerateConfigData_PlayerRoom;
	public PlayerPvpConfigDataBase PlayerGenerateConfigData_PVP;
    public PlayerPrestigeConfigDataBase PlayerPrestigeConfigData;

    public PlayerSirenConfigDataBase PlayerSirenConfigDataBase;

    public PlayerBasePropDataList PlayerBasePropConfigData;

    public ProfessionConfigDataBase ProfessionConfigData;

	public PlayerTitleConfigDataBase PlayerTitleConfigDataBase;
    public PlayerStrengthCostDataBase PlayerStrengthCostDataBase;
	public PlayerMartialArtsDataBase PlayerMartialDataBase;

    public AnimationMapDataBase AnimationMapDataBase_Cike;
    public AnimationMapDataBase AnimationMapDataBase_Tianshi;
    public AnimationMapDataBase AnimationMapDataBase_Daoke;
    public AnimationMapDataBase AnimationMapDataBase_Qinshi;

    public AnimationConfigDataBase AnimationConfigDataBase_Cike;
    public AnimationConfigDataBase AnimationConfigDataBase_Tianshi;
    public AnimationConfigDataBase AnimationConfigDataBase_Daoke;
    public AnimationConfigDataBase AnimationConfigDataBase_Qinshi;

    //VIP data
    public VIPConfigDataBase m_VIPConfigDataBase;
	//activity 活动
	public ActivityConfigDataBase activityDataBase;
	
	public GameObject LevelUpEffectPrefab;

    public static readonly int JEWEL_BASE_ID= 3010100 ;
    private static PlayerDataManager m_instance;
    public static PlayerDataManager Instance
    {
        get
        { 
            if(null == m_instance)
            {
                m_instance = GameObject.FindObjectOfType(typeof(PlayerDataManager)) as PlayerDataManager;
            }
            return m_instance;
        }
    }
    private Dictionary<byte, PlayerGenerateConfigData> m_TownItems = new Dictionary<byte, PlayerGenerateConfigData>();
    private Dictionary<byte, PlayerGenerateConfigData> m_BattleItems = new Dictionary<byte, PlayerGenerateConfigData>();
    private Dictionary<byte, PlayerGenerateConfigData> m_UIItems = new Dictionary<byte, PlayerGenerateConfigData>();
	private Dictionary<int, PlayerPvpConfigData> m_PVPItems = new Dictionary<int, PlayerPvpConfigData>();
    private Dictionary<byte, PlayerGenerateConfigData> m_PlayerRoomItems = new Dictionary<byte, PlayerGenerateConfigData>();
    private Dictionary<int, NewCharacterConfigData> NewCharacterConfigDataList = new Dictionary<int, NewCharacterConfigData>();
    private List<PlayerPrestigeConfigData> PlayerPrestigeList = new List<PlayerPrestigeConfigData>();
    private List<PlayerSirenConfigData> PlayerSirenList = new List<PlayerSirenConfigData>();
	private Dictionary<int ,PlayerTitleConfigData> m_PlayerTitles = new Dictionary<int, PlayerTitleConfigData>();
	private Dictionary<int, ProfessionConfigData> m_ProfessionConfigDic = new Dictionary<int, ProfessionConfigData>();
	private List<PlayerStrengthCost>m_StrengthCost=new List<PlayerStrengthCost>();
	private List<PlayerStrengthCost>m_StarUpCost=new List<PlayerStrengthCost>();
    //vip dic
    private Dictionary<int, VIPConfigData> m_vipConfigDic = new Dictionary<int, VIPConfigData>();
	private Dictionary<int, ActivityConfigData> m_activityData = new Dictionary<int, ActivityConfigData>();
	private Dictionary<MartialIndex, PlayerMartialArtsData> m_martialArtData = new Dictionary<MartialIndex, PlayerMartialArtsData>();
	// Use this for initialization
	void Start () {
        m_instance = this;
        InitTownConfigData();
        InitBattleConfigData();
        InitUIConfigData();
        InitPlayerRoomConfigData();
        InitPlayerPrestigeConfigData();
        InitPlayerSirenConfigData();
		InitPlayerTitleConfigData();
        InitProfessionConfigData();
        InitStrengthCostData();
		InitVIPConfigData();
		InitActivityConfigData ();
		InitPVPConfigData();
		InitMartialArtData();
	}
    void InitTownConfigData()
    {
        foreach (PlayerGenerateConfigData data in PlayerGenerateConfigData_Town._dataTable)
        {
            m_TownItems.Add(data.PlayerId, data);
        }
    }
    void InitBattleConfigData()
    {
        foreach (PlayerGenerateConfigData data in PlayerGenerateConfigData_Battle._dataTable)
        {
            m_BattleItems.Add(data.PlayerId, data);
        }
    }
    void InitUIConfigData()
    {
        foreach (PlayerGenerateConfigData data in PlayerGenerateConfigData_UI._dataTable)
        {
            m_UIItems.Add(data.PlayerId, data);
        }
    }

	void InitPVPConfigData()
	{
		foreach (PlayerPvpConfigData data in PlayerGenerateConfigData_PVP.PlayerPvpConfigDataList)
		{
			m_PVPItems.Add(data.PlayerId, data);
		}
	}

    void InitPlayerRoomConfigData()
    {
        foreach (PlayerGenerateConfigData data in PlayerGenerateConfigData_PlayerRoom._dataTable)
        {
            m_PlayerRoomItems.Add(data.PlayerId, data);
        }
    }
    void InitPlayerPrestigeConfigData()
    {
        foreach (PlayerPrestigeConfigData data in PlayerPrestigeConfigData._dataTable)
        {
            PlayerPrestigeList.Add(data);
        }
    }
    void InitPlayerSirenConfigData()
    {
        foreach(PlayerSirenConfigData data in PlayerSirenConfigDataBase._dataTable)
        {
            PlayerSirenList.Add(data);
        }
    }
	void InitPlayerTitleConfigData()
	{
		foreach(PlayerTitleConfigData data in PlayerTitleConfigDataBase._dataTable)
		{
			m_PlayerTitles.Add(data._lGoodsID, data);
		}
	}
    void InitProfessionConfigData()
    {
        foreach (ProfessionConfigData data in   ProfessionConfigData._dataTable)
        {
            m_ProfessionConfigDic.Add(data._professionID, data);
        }
    }

    void InitStrengthCostData()
    {
        foreach(PlayerStrengthCost data in PlayerStrengthCostDataBase.PlayerStrengthCostList)
        {
            if(data.GainType==UpgradeType.Strength)
            {
                m_StrengthCost.Add(data);
            }
            else
            {
                m_StarUpCost.Add(data);
            }
        }
    }
    void InitVIPConfigData()
    {
        foreach(VIPConfigData data in m_VIPConfigDataBase.m_dataTable)
        {
            m_vipConfigDic.Add(data.m_vipLevel, data);
        }
    }

	void InitActivityConfigData()
	{
		foreach(ActivityConfigData data in activityDataBase._dataTable)
		{
			data.GetActReward();
			m_activityData.Add(data.ActivityID, data);
		}
	}

	void InitMartialArtData()
	{
		foreach(PlayerMartialArtsData data in PlayerMartialDataBase._dataTable)
		{
			m_martialArtData.Add(data.martialIndex, data);
		}
	}

	public PlayerMartialArtsData GetPlayerMartialArtConfigData(MartialIndex martialIndex)
	{
		return m_martialArtData[martialIndex];
	}

    public PlayerGenerateConfigData GetTownItemData(byte playerKind)
    {
        return m_TownItems[playerKind];

    }
    public PlayerGenerateConfigData GetBattleItemData(byte playerKind)
    {
        return m_BattleItems[playerKind];

    }
    public PlayerGenerateConfigData GetUIItemData(byte playerKind)
    {
        //TraceUtil.Log("Player kind:" + playerKind);
        return m_UIItems[playerKind];

    }
	public PlayerPvpConfigData GetPVPItemData(int playerID)
	{

		return m_PVPItems[playerID];
	}
    public PlayerGenerateConfigData GetPlayerRoomItemData(byte playerKind)
    {
        return m_PlayerRoomItems[playerKind];
    }
    public List<PlayerPrestigeConfigData> GetPlayerPrestigeList()
    {
        return PlayerPrestigeList;
    }
    public List<PlayerSirenConfigData> GetPlayerSirenList()
    {
        return PlayerSirenList;
    }
	public PlayerTitleConfigData GetPlayerTitleConfigData(int GoodsID)
	{
		if(m_PlayerTitles.ContainsKey(GoodsID))
		{
			return m_PlayerTitles[GoodsID];
		}
		return null;
	}

    /// <summary>
    ///获取玩家战力
    /// </summary>
    /// <returns>The hero force.</returns>
    public int GetHeroForce()
    {
        return HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat,PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
    }
	public PlayerTitleConfigData[] GetPlayerTitleConfigArray()
	{
		return PlayerTitleConfigDataBase._dataTable;
	}
    public NewCharacterConfigData GetNewCharacterConfigData(int PlayerID)
    {
        return NewCharacterConfigDataList[PlayerID];
    }
    public ProfessionConfigData GetProfessionConfigData(int ProfessionID)
    {
        if (m_ProfessionConfigDic.ContainsKey(ProfessionID))
        {
            return m_ProfessionConfigDic[ProfessionID];
        }
        return null;
    }

    #region vip
    public int GetFreeDrugTimes()
    {
        int vipLevel = GetPlayerVIPLevel();
        return m_vipConfigDic[vipLevel].m_freeDrugTimes;
    }


    public int GetPlayerVIPLevel()
    {
        SMsgPropCreateEntity_SC_MainPlayer player = PlayerManager.Instance.FindHeroDataModel();
        return player.GetCommonValue().PLAYER_FIELD_VISIBLE_VIP;
    }

    public int GetenergyPurchaseTimes()
    {
        int vipLevel = GetPlayerVIPLevel();
        return m_vipConfigDic[vipLevel].m_energyPurchaseTimes;
    }

    public int GetUpgradeExp()
    {
        int vipLevel = GetPlayerVIPLevel();
        return m_vipConfigDic[vipLevel].m_upgradeExp;

    }

    public int GetEctypeExpBonus()
    {
        int vipLevel = GetPlayerVIPLevel();
        return m_vipConfigDic[vipLevel].m_ectypeExpBonus;
    }

    public int GetMainEctypeRewardTimes()
    {
        int vipLevel = GetPlayerVIPLevel();
        return m_vipConfigDic[vipLevel].m_mainEctypeRewardTimes;
    }
	//获取活动
	public ActivityConfigData GetActivityData(int activeID)
	{
		if (m_activityData.ContainsKey (activeID)) {
			return m_activityData[activeID];
		}
		return null;
	}
    public int GetMainEctypeVIPRewardMinLevel()
    {
        int minLevel = int.MaxValue;
        foreach(VIPConfigData data in m_VIPConfigDataBase.m_dataTable)
        {
            if(data.m_mainEctypeRewardTimes == 3 && data.m_vipLevel < minLevel)
            {
                minLevel = data.m_vipLevel;
            }
        }
        return minLevel;

    }

    public int GetLuckDrawTimes()
    {
        int vipLevel = GetPlayerVIPLevel();
        return m_vipConfigDic[vipLevel].m_luckDrawTimes;
    }

    public int GetVipAddDrawTimes()
    {
        int vipLevel = GetPlayerVIPLevel();
        return m_vipConfigDic[vipLevel].m_luckDrawTimes - m_vipConfigDic[0].m_luckDrawTimes;
    }

    public bool CanEquipmentQuickStrengthen()
    {
        int vipLevel = GetPlayerVIPLevel();
        return m_vipConfigDic[vipLevel].m_canEquipmentQuickStrengthen;
    }


    public bool CanEquipmentQuickUpgradeStar()
    {
        int vipLevel = GetPlayerVIPLevel();
        return m_vipConfigDic[vipLevel].m_canEquipmentQuickUpgradeStar;
    }
	//获取vip数据
	public VIPConfigData GetVipData(int level)
	{
		int maxLev = GetVIPMaxLevel ();
		if (level < 0 || level > maxLev) {
			level = maxLev;
		}
		return m_vipConfigDic [level];
	}
	//获取vip最大等级
	public int GetVIPMaxLevel()
	{
		return m_vipConfigDic [m_vipConfigDic.Count - 1].m_vipLevel;
	}
	//
	//获取奖励
	public List<VipLevelUpReward> GetVIPRewardList()
	{
		int vipLevel = GetPlayerVIPLevel();
		return m_vipConfigDic[vipLevel].m_RewardList;
	}
	//PLAYER_FIELD_VISIBLE_VOCATION
	//获取当前职业
	public List<VipLevelUpReward> GetVipCurVocatReward()
	{
		return GetVIPRewardOfVocation (PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
	}
	//获取对应职业奖励
	public List<VipLevelUpReward> GetVIPRewardOfVocation(int vocation)
	{
		List<VipLevelUpReward> oldRewardList = GetVIPRewardList ();
		List<VipLevelUpReward> rewardList = new List<VipLevelUpReward> ();
		foreach (VipLevelUpReward reward in oldRewardList) {
			if(reward.m_vocation == vocation)
			{
				rewardList.Add(reward);
			}
		}
		return rewardList;
	}
	//获取玩家当前vip充值积累了多少金币
	public int GetCurVipPayMoney()
	{
		return PlayerManager.Instance.FindHeroDataModel().PlayerValues.PALYER_FIELD_GOLD_TOTALTOPUP_VALUE;
	}

    public GameObject GetCurrentVipEmblemPrefab()
    {
        int vipLevel = GetPlayerVIPLevel();
        return m_vipConfigDic[vipLevel].m_vipEmblemPrefab;
    }

    public GameObject GetCurrentVipEmblemPrefab(int vipLevel)
    {
        return m_vipConfigDic[vipLevel].m_vipEmblemPrefab;
    }
    #endregion
    public int GetEquipmentStrengthLevel(EquiptSlotType itemType)
    {
        int normalStrengthenLv=0 ;
       // int starStrengthenLv ;
        
        switch(itemType)
        {
            case EquiptSlotType.Weapon:
                normalStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STRENGTH_VALUE;
               // starStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_START_VALUE;
                break;
            case EquiptSlotType.Heard:
                normalStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAT_STRENGTH_VALUE;
                // starStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_START_VALUE;
                break;
            case EquiptSlotType.Body:
                normalStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CLOTH_STRENGTH_VALUE;
                //starStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_START_VALUE;
                break;
            case EquiptSlotType.Shoes:
                normalStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHOES_STRENGTH_VALUE;
                // starStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_START_VALUE;
                break;
            case EquiptSlotType.Accessories:
                normalStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_RING_STRENGTH_VALUE;
                break;
                
        }
        return normalStrengthenLv;
    }

    public List<UpgradeRequire> GetStrengCost(EquiptType type, int level)
    {
        PlayerStrengthCost cost=m_StrengthCost.SingleOrDefault(c=>c.lGoodsSubClass==type&&c.GainLevel==level);
		if(cost!=null)
        {
            return cost.UpgradeRequires;
        }
        else
        {
            return new List<UpgradeRequire>();
        }
    }

	public List<UpgradeRequire> GetStarUpCost(EquiptType type, int level)
    {
		PlayerStrengthCost cost=m_StarUpCost.SingleOrDefault(c=>c.lGoodsSubClass==type&&c.GainLevel==level);
		if(cost!=null)
        {
            return cost.UpgradeRequires;
        }
        else
        {
            return new List<UpgradeRequire>();
        }
    }
    public int GetEquipmentStarLevel(EquiptSlotType itemType)
    {
        //int normalStrengthenLv ;
         int starStrengthenLv=0 ;
        
        switch(itemType)
        {
            case EquiptSlotType.Weapon:
               // normalStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STRENGTH_VALUE;
                starStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_START_VALUE;
                break;
            case EquiptSlotType.Heard:
               // normalStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STRENGTH_VALUE;
                starStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAT_START_VALUE;
                break;
            case EquiptSlotType.Body:
              //  normalStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STRENGTH_VALUE;
                starStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CLOTH_START_VALUE;
                break;
            case EquiptSlotType.Shoes:
               // normalStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STRENGTH_VALUE;
                starStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHOES_START_VALUE;
                break;
            case EquiptSlotType.Accessories:
                starStrengthenLv=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_RING_START_VALUE;
                break;
                
        }
        return starStrengthenLv;
    }

    public List<JewelInfo> GetJewelInfo(EquiptSlotType itemType)
    {    
        List<JewelInfo> infos=new List<JewelInfo>();
        JewelInfo info1=new JewelInfo();
        JewelInfo info2=new JewelInfo();
        switch(itemType)
        {
            case EquiptSlotType.Weapon:
                info1.JewelID=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STORE_ID1_VALUE;
                info1.JewelLevel=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STORE_LEVEL1_VALUE;
                info1.jewelExp=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STORE_EXP1_VALUE;
                info2.JewelID=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STORE_ID2_VALUE;
                info2.JewelLevel=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STORE_LEVEL2_VALUE;
                info2.jewelExp=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_WEAPON_STORE_EXP2_VALUE;;
                break;
            case EquiptSlotType.Heard:
                info1.JewelID=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAT_STORE_ID1_VALUE;
                info1.JewelLevel=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAT_STORE_LEVEL1_VALUE;
                info1.jewelExp=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAT_STORE_EXP1_VALUE;
                info2.JewelID=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAT_STORE_ID2_VALUE;
                info2.JewelLevel=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAT_STORE_LEVEL2_VALUE;
                info2.jewelExp=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HAT_STORE_EXP2_VALUE;;
                break;
            case EquiptSlotType.Body:
                info1.JewelID=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CLOTH_STORE_ID1_VALUE;
                info1.JewelLevel=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CLOTH_STORE_LEVEL1_VALUE;
                info1.jewelExp=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CLOTH_STORE_EXP1_VALUE;
                info2.JewelID=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CLOTH_STORE_ID2_VALUE;
                info2.JewelLevel=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CLOTH_STORE_LEVEL2_VALUE;
                info2.jewelExp=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CLOTH_STORE_EXP2_VALUE;
                break;
            case EquiptSlotType.Shoes:
                info1.JewelID=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHOES_STORE_ID1_VALUE;
                info1.JewelLevel=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHOES_STORE_LEVEL1_VALUE;
                info1.jewelExp=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHOES_STORE_EXP1_VALUE;
                info2.JewelID=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHOES_STORE_ID2_VALUE;
                info2.JewelLevel=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHOES_STORE_LEVEL2_VALUE;
                info2.jewelExp=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SHOES_STORE_EXP2_VALUE;
                break;
            case EquiptSlotType.Accessories:
                info1.JewelID=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_RING_STORE_ID1_VALUE;
                info1.JewelLevel=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_RING_STORE_LEVEL1_VALUE;
                info1.jewelExp=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_RING_STORE_EXP1_VALUE;
                info2.JewelID=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_RING_STORE_ID2_VALUE;
                info2.JewelLevel=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_RING_STORE_LEVEL2_VALUE;
                info2.jewelExp=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_RING_STORE_EXP2_VALUE;;
                break;
                
        }
        infos.Add(info1);
        infos.Add(info2);
        return infos;
    }
	//按等级弹出提示
	public bool CanPopTip(EViewType viewType)
	{
		int viewVal = (int)viewType;
		for (int i = 0; i < CommonDefineManager.Instance.CommonDefine.levelComeInTown.Count; i++) {
			if(CommonDefineManager.Instance.CommonDefine.levelComeInTown[i] == viewVal)
			{
				if(PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL >= CommonDefineManager.Instance.CommonDefine.viewComeInTown[i])
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	/// <summary>
	/// 根据二型结算id查找属性index
	/// </summary>
	/// <returns>属性index</returns>
	/// <param name="settleID">二型结算id</param>
	public int GetPropID(int settleID)
	{
		int propID = -1;
		var propData = PlayerBasePropConfigData.playerBasePropDatalist.SingleOrDefault(p=>p.nSettleID == settleID);
		if(propData!=null)
		{
			propID = propData.nPropID;
		}
		return propID;
	}

}
