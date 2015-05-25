using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable] 
public class VipLevelUpReward
{
    public int m_vocation;
    public int m_itemID;
    public int m_itemCount;
}


[System.Serializable]
public class VIPConfigData
{
    public int m_vipLevel;
    public int m_upgradeExp;
    public int m_freeDrugTimes;
    public int m_energyPurchaseTimes;
    public int m_ectypeExpBonus;
    public int m_mainEctypeRewardTimes;
    public int m_luckDrawTimes;
    public bool m_canEquipmentQuickStrengthen;
    public bool m_canEquipmentQuickUpgradeStar;
    public GameObject m_vipEmblemPrefab;
    public List<VipLevelUpReward> m_RewardList;
	public int VipSweepNum;
}




public class VIPConfigDataBase : ScriptableObject {
    public VIPConfigData[] m_dataTable;
	
}
