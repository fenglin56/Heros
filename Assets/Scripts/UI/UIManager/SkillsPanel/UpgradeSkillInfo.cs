using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UpgradeSkillInfoArray
{
    public List<UpgradeSkillInfo> ListSkillInfo;

    public UpgradeSkillInfoArray()
    {
        this.ListSkillInfo = new List<UpgradeSkillInfo>();
        List<SSkillInfo> sSkilInfo = PlayerManager.Instance.HeroSMsgSkillInit_SC.sInfos;
        sSkilInfo.Sort(delegate(SSkillInfo a, SSkillInfo b) { return (a.wSkillID).CompareTo(b.wSkillID); });
        foreach (SSkillInfo child in sSkilInfo)
        {
            if (SkillDataManager.Instance.GetSkillConfigData(child.wSkillID).m_triggerType == 0)
            {
                ListSkillInfo.Add(new UpgradeSkillInfo(child));
            }
        }
    }
}

public class UpgradeSkillInfo
{
    public bool CanUpgrade;//该技能是否已经达到等级上限,能否继续升级

    public int SkillLevel;//等级

    public SkillConfigData localSkillConfigData;

    public UpgradeSkillInfo(SSkillInfo sSkill)
    {
        this.localSkillConfigData = SkillDataManager.Instance.GetSkillConfigData(sSkill.wSkillID);
        this.SkillLevel = sSkill.wSkillLV;
        if (SkillLevel < 10) { this.CanUpgrade = true; } else { this.CanUpgrade = false; }

    }

    public UpgradeSkillInfo()
    {
        this.localSkillConfigData = SkillDataManager.Instance.GetSkillConfigData(1001);
        this.CanUpgrade = true;
        this.SkillLevel = 10;
    }

}

public class SingleSkillInfoList
{
    int HeroVocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;//玩家职业
    public List<SingleSkillInfo> singleSkillInfoList;
    public List<SingleSkillInfo> CanUpdateSkillInfos;
    public SingleSkillInfo[] EquipSkillsList;//装配到战斗技能区的技能
    public SingleSkillInfoList()
    {
        //TraceUtil.Log("获取职业技能信息："+HeroVocation);
        this.singleSkillInfoList = new List<SingleSkillInfo>();
        this.CanUpdateSkillInfos=new List<SingleSkillInfo>();
        List<SSkillInfo> sSkilInfo = PlayerManager.Instance.HeroSMsgSkillInit_SC.sInfos;
        foreach (SkillConfigData child in SkillDataManager.Instance.m_skillConfigDataBase._dataTable)
        {
            if (child.m_vocation == HeroVocation && child.m_skillType == (int)SkillType.General)
            {
                var item = new SingleSkillInfo(child);
                foreach (var sskill in sSkilInfo)
                {
                    if (sskill.wSkillID == child.m_skillId)
                    {
                        CanUpdateSkillInfos.Add(item);
                        break;
                    }
                }
                this.singleSkillInfoList.Add(item);
            }

        }
        CanUpdateSkillInfos.Sort(delegate(SingleSkillInfo a, SingleSkillInfo b) { return (a.localSkillData.m_skillId).CompareTo(b.localSkillData.m_skillId); });

        SetEquipSkills();
    }

    public void SetEquipSkills()//设置连接到战斗技能区的技能
    {
        ushort[] EquipSkillIDList = PlayerManager.Instance.HeroSMsgSkillInit_SC.wSkillEquipList;
        EquipSkillsList = new SingleSkillInfo[EquipSkillIDList.Length];
        for (int i = 0; i < EquipSkillIDList.Length; i++)
        {
            //TraceUtil.Log("战斗技能位置：" + i + ", " + EquipSkillIDList[i]);
            if (EquipSkillIDList[i] == 0)
            {
                EquipSkillsList[i] = null;
            }
            else
            {
                foreach (SingleSkillInfo child in singleSkillInfoList)
                {
                    if (EquipSkillIDList[i] == (byte)child.localSkillData.m_skillId)
                    {
                        EquipSkillsList[i] = child;
                    }
                    child.BattleIconPosition = 0;
                }
            }
        }
        for (int i = 0; i < EquipSkillsList.Length; i++)
        {
            if (EquipSkillsList[i] != null)
            {
                EquipSkillsList[i].BattleIconPosition = i + 1;
            }
        }
    }

    public void ResetSkillPoint()
    {
        List<SSkillInfo> sSkilInfo = PlayerManager.Instance.HeroSMsgSkillInit_SC.sInfos;
        foreach (SSkillInfo child in sSkilInfo)
        {
            singleSkillInfoList.ApplyAllItem(P=>
                {
                    if(P.localSkillData.m_skillId==child.wSkillID && P.localSkillData.m_triggerType==0)
                    {
                         P.SkillLevel = child.wSkillLV;
                    }
                });
        }
    }

}
public class SingleSkillInfo
{
    private int m_skillLevel;
    public SkillConfigData localSkillData;

	#region 旧代码
	public bool CanUpgrade;//该技能是否已经达到等级上限,能否继续升级
	public bool Lock = true;//是否解锁
	public bool OnSelect = false;//是否被选中
	#endregion    
    public int BattleIconPosition;//被连接到战斗技能区的位置，0为非链接，由于读表的关系，所以多了这一项
    public SingleButtonCallBack BattleIconButton;//被连接到战斗技能区的位置，null为未连接
    //当前技能等级
    public int SkillLevel
    {
        get
        {
            return m_skillLevel;
        }
        set
        {
            this.m_skillLevel = value;
            CanUpgrade = this.m_skillLevel < localSkillData.m_maxLv;
        }
    }

    public SingleSkillInfo(SSkillInfo sSkill)
    {
        this.localSkillData = SkillDataManager.Instance.GetSkillConfigData(sSkill.wSkillID);
        this.SkillLevel = sSkill.wSkillLV;
    }
    public SingleSkillInfo(SkillConfigData LocalData)
    {
        this.localSkillData = LocalData;
        //this.Lock = IsLock(LocalData.m_skillId);
        this.OnSelect = false;
        this.BattleIconPosition = 0;
        foreach (SSkillInfo child in PlayerManager.Instance.HeroSMsgSkillInit_SC.sInfos)
        {
            //TraceUtil.Log("已经解锁技能ID："+child.wSkillID);
            //if (child.wSkillID == SkillID && child.wSkillLV >= localSkillData.m_unlockLevel)
            if (child.wSkillID == LocalData.m_skillId)
            {
                this.Lock = false;
                this.SkillLevel = child.wSkillLV;
            }
        }
    }
	/// <summary>
	/// 是否解锁
	/// </summary>
	/// <returns><c>true</c> if this instance is unlock; otherwise, <c>false</c>.</returns>
	public bool IsUnlock()
	{
		int unLockLev=this.localSkillData.m_unlockLevel;
		int playerLev=PlayerManager.Instance.FindHeroDataModel().GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
		return unLockLev<=playerLev;
	}
	/// <summary>
	/// 是否满级
	/// </summary>
	/// <returns><c>true</c> if this instance is full lev; otherwise, <c>false</c>.</returns>
	public bool IsFullLev()
	{
		int skillLev=this.SkillLevel;
		int maxLev=this.localSkillData.m_maxLv;
		return skillLev>=maxLev;
	}
	/// <summary>
	/// 计算技能升级消耗
	/// </summary>
	/// <returns>The consume.</returns>
	/// <param name="enought">是否足够铜币升级</param>
	/// <param name="canUpgrade">是否满足升级所需等级.</param>
    public int UpgradeConsume(out bool enought,out bool reachUpLev,out int upNeedLev)
	{
		int nextLev=SkillLevel+1;
		float skillManaValue = localSkillData.m_upgradeMoneyParams[SkillLevel-1];//.m_manaConsumeParams[SkillLevel];// SkillValue.GetSkillValue(SkillLevel, localSkillData.m_manaConsumeParams);
		int playerMoney=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
		enought=playerMoney>=skillManaValue;
		int playerLev=PlayerManager.Instance.FindHeroDataModel().GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;

		upNeedLev=((nextLev-1)*localSkillData.m_UpdateInterval+localSkillData.m_unlockLevel);
		reachUpLev=playerLev>=upNeedLev;
		return (int)skillManaValue;
	}
}

//（0=天生技能，1=普通技能）

public enum SkillType
{
    Default = 0,   //天生
    General = 1,    //普通  
}