using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SkillModel:ISingletonLifeCycle {
	private static SkillModel instance;
	public static SkillModel Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SkillModel();
				SingletonManager.Instance.Add(instance);
				instance.CountSkillData ();
			}
			return instance;
		}
	}
	public void Instantiate()
	{
		
	}
	
	public void LifeOver()
	{
		instance = null;
	}
	//当前选中的技能ID
	public int curSelectSkillID = 0;
	public int GetSkillTreeID(int skillID)
	{
		SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
		return configData.SkillTreeID;
	}
	public int GetSkillPosIndex(int skillID)
	{
		SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
		for (int i = 0; i < skillMap[configData.SkillTreeID].Count; i++) {
			if(skillMap[configData.SkillTreeID][i] == skillID)
			{
				return i+1;
			}
		}
		return 0;
	}
	//获取技能列表
	public Dictionary<int,List<int>> skillMap = new Dictionary<int, List<int>>();
	public void CountSkillData()
	{
		skillMap.Clear ();
		foreach (SkillConfigData info in SkillDataManager.Instance.m_skillConfigDataBase._dataTable) {
			if(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION != info.m_vocation)
				continue;
			if(info.SkillTreeID == 0)
				continue;
			if(!skillMap.ContainsKey(info.SkillTreeID))
			{
				List<int> skillList = new List<int>();
				skillList.Add(info.m_skillId);
				skillMap.Add(info.SkillTreeID,skillList);
			}
			else
			{
				skillMap[info.SkillTreeID].Add(info.m_skillId);
			}
		}
		RankSkillData ();
	}
	void RankSkillData()
	{
		Dictionary<int,List<int>> skillMapTemp = new Dictionary<int, List<int>> ();
		foreach (var map in skillMap) {
			//排序
			List<int> temp = new List<int>();
			int i = 0 ;
			for(i = 0 ; i < map.Value.Count; i++)
			{
				SkillConfigData configData0 = SkillDataManager.Instance.GetSkillConfigData (map.Value[i]);
				if(configData0.PreSkill == 0)
				{
					temp.Add(configData0.m_skillId);
				}
			}
			for(int j = 0 ; j < map.Value.Count; j++)
			{
				for(i = 0 ; i < map.Value.Count; i++)
				{
					SkillConfigData configData1 = SkillDataManager.Instance.GetSkillConfigData (map.Value[i]);
					if(configData1.PreSkill == temp[j])
					{
						temp.Add(configData1.m_skillId);
						break;
					}
				}
			}
			skillMapTemp.Add(map.Key,temp);
		}
		skillMap.Clear ();
		skillMap = skillMapTemp;
//		Debug.Log ("skillMap=="+skillMap.Count);
	}
	//获取 第一棵树 已装备的技能
	public int GetFirstEquipSkillID()
	{
		List<int> temp = skillMap [1];//.Count;
		for (int i = temp.Count-1; i >= 0; i--) {
			SSkillInfo? info = GetCurSkill(temp[i]);
			if(info != null)
			{
				return temp[i];
			}
		}
		return temp[0];
	}
	//获取 技能 数据
	public SSkillInfo? GetCurSkill(int skillID)
	{
		foreach (SSkillInfo info in PlayerManager.Instance.HeroSMsgSkillInit_SC.sInfos) {
			if(info.wSkillID == skillID)
			{
				return info;
			}
		}
		return null;
	}
	//技能不在解锁和进阶技能内[在服务器数据中]
	public bool IsOpenSkill(int skillID)
	{
		foreach (SSkillInfo info in PlayerManager.Instance.HeroSMsgSkillInit_SC.sInfos) {
			if(info.wSkillID == skillID)
			{
				return true;
			}
		}
		return false;
	}
	//技能是否解锁,只有每行的第一个才可能锁，其它的都解锁了
	public bool IsUnLockSkill(int skillID)
	{
		SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
		if (configData.PreSkill != 0)
			return true;
		if (PlayerManager.Instance.FindHeroDataModel ().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL < configData.m_unlockLevel) {
			//说明没有解锁
			return false;		
		}
		return true;
	}
	//是否可进阶（前提是没有进阶）
	public bool IsCanAdvanceSkill(int skillID)
	{
		SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
		if (configData.PreSkill == 0)
			return false;
		if (!IsOpenSkill (skillID)) {
			SkillConfigData configData1 = SkillDataManager.Instance.GetSkillConfigData (configData.PreSkill);
			SSkillInfo? curSkill = GetCurSkill(configData.PreSkill);
			if(curSkill != null && curSkill.Value.wSkillLV >= configData1.m_maxLv)
			{
				return true;
			}
		}
		return false;
	}
	public void DealSkillAdUpStrengthen()
	{
		if (IsHaveAdvanceUpgradeStrengthen ()) {
			UIEventManager.Instance.TriggerUIEvent (UIEventType.PlayMainBtnAnim, UI.MainUI.UIType.Skill);
		} else {
			UIEventManager.Instance.TriggerUIEvent (UIEventType.StopMainBtnAnim,UI.MainUI.UIType.Skill);			
		}
	}
	//只要存在升级，进阶，强化，即true
	public bool IsOnleAdvanceUpStrengthen()
	{
		foreach (SSkillInfo info in PlayerManager.Instance.HeroSMsgSkillInit_SC.sInfos) {
			if(IsCanAdvanceSkillModel(info.wSkillID) ||IsCanUpgradeSkillModel(info.wSkillID,false) ||(IsCanUpgradeSkillModel(info.wSkillID,false)))
			{
				return true;
			}
		}
		return false;
	}
	//存在两个或两个以上，在某等级限制情况下//
	private bool IsHaveAdvanceUpgradeStrengthen()
	{
		int oneTimesCount = 0;
		int timeAdvanceCount = 0;
		int twoTimesStrengthenCount = 0;
		int twoTimesUpgrade = 0;
		foreach (var skill in skillMap) {
			foreach(int skillID in skill.Value)
			{
				//foreach (SSkillInfo info in PlayerManager.Instance.HeroSMsgSkillInit_SC.sInfos) {
				if(IsCanUpgradeSkillModel(skillID,false))
				{
					oneTimesCount++;
				}
				if(IsCanStrengthenSkillModel(skillID,false))
				{
					oneTimesCount++;
				}
				if(IsCanAdvanceSkillModel(skillID))
				{
					timeAdvanceCount++;
				}
				if(IsCanUpgradeSkillModel(skillID,true))
				{
					twoTimesUpgrade++;
				}
				if(IsCanStrengthenSkillModel(skillID,true))
				{
					twoTimesStrengthenCount++;
				}
			}
		}
		if ((timeAdvanceCount+twoTimesUpgrade+twoTimesStrengthenCount >= 1) || (PlayerManager.Instance.FindHeroDataModel ().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL >= 
		                                                                        CommonDefineManager.Instance.CommonDefine.ButtonWeakTipsLevel && oneTimesCount >= 1)) {
			//Debug.Log ("true!!!!!!!timeAdvanceCount="+timeAdvanceCount+"twoTimesUpgrade="+twoTimesUpgrade+"twoTimesStrengthenCount="+twoTimesStrengthenCount+"oneTimesCount="+oneTimesCount);
			return true;		
		}
		//Debug.Log ("false!!!!!!!timeAdvanceCount="+timeAdvanceCount+"twoTimesUpgrade="+twoTimesUpgrade+"twoTimesStrengthenCount="+twoTimesStrengthenCount+"oneTimesCount="+oneTimesCount);
		return false;
	}
	//进阶判定//
	bool IsCanAdvanceSkillModel(int skillID)
	{
		SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
		SSkillInfo? curSkill = GetCurSkill(skillID);
		//if (!SkillModel.Instance.IsOpenSkill (skillID)) {
		if(configData.PreSkill != 0)
		{
			//进阶
			if(curSkill == null && SkillMiddleModel(skillID,false,false))
			{
				return true;
			}
		}
		//}
		return false;
	}
	bool IsCanUpgradeSkillModel(int skillID,bool isDouble)
	{
		SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
		SSkillInfo? curSkill = GetCurSkill(skillID);
		if (SkillModel.Instance.IsOpenSkill (skillID)) {
			if(curSkill.Value.wSkillLV < configData.m_maxLv)
			{
				//可升级
				if(SkillMiddleModel(skillID,true,isDouble))
				{
					return true;
				}
			}
		}
		return false;
	}
	bool IsCanStrengthenSkillModel(int skillID,bool isDouble)
	{
		SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
		SSkillInfo? curSkill = GetCurSkill(skillID);
		int strengthenLv = curSkill == null?1:(int)(curSkill.Value.byStrengthenLv);
		if (curSkill == null || configData.SkillStrengthen == 0 || curSkill.Value.byStrengthenLv >= configData.SkillStrengthen) {
			//不需要强化//
			return false;		
		}
		if(strengthenLv > 0 && strengthenLv-1 >= configData.skillStrMoneyList.Count)
			return false;
		int haveMon = strengthenLv > 0 ? configData.skillStrMoneyList[strengthenLv-1] : 0;
		if(isDouble)
		{
			if(strengthenLv < configData.SkillStrengthen-1)
			{
				haveMon += configData.skillStrMoneyList[strengthenLv];
				strengthenLv++;
			}
			else
			{
				return false;
			}
		}
		if (configData.SkillStrengthen != 0 && curSkill != null && strengthenLv < configData.SkillStrengthen) {
			if(PlayerManager.Instance.IsMoneyEnough(haveMon))
			{
				return true;
			}
		}
		return false;
	}
	//注意：可以升两级时//
	bool SkillMiddleModel(int skillID,bool isUpgrade,bool isDouble)
	{
		SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
		SSkillInfo? curSkill = GetCurSkill(skillID);
		bool isCanClick = false;
		//升级
		if (isUpgrade) {
			int haveLv = configData.m_unlockLevel + curSkill.Value.wSkillLV * configData.m_UpdateInterval;
			int haveMon = configData.m_upgradeMoneyParams [curSkill.Value.wSkillLV - 1];
			if(isDouble)
			{
				if(curSkill.Value.wSkillLV < configData.m_maxLv-1)
				{
					haveLv = configData.m_unlockLevel + (curSkill.Value.wSkillLV+1) * configData.m_UpdateInterval;
					haveMon += configData.m_upgradeMoneyParams [curSkill.Value.wSkillLV];
				}
				else
				{
					return false;
				}
			}
			if (PlayerManager.Instance.FindHeroDataModel ().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL >= haveLv) {
				isCanClick = true;
			}
			if (PlayerManager.Instance.IsMoneyEnough (haveMon)) {
				if(!isCanClick)
					isCanClick = false;
			} else {
				if(isCanClick)
					isCanClick = false;
			}
		} else {
			//进阶
			SkillConfigData configPre = SkillDataManager.Instance.GetSkillConfigData (configData.PreSkill);
			SSkillInfo? preSkill = SkillModel.Instance.GetCurSkill (configData.PreSkill);
			int preLv = preSkill==null?0:(int)preSkill.Value.wSkillLV;
			if(preSkill != null && preSkill.Value.wSkillLV >= configPre.m_maxLv)
			{
				isCanClick = true;
			}
			int goodsCount = UI.MainUI.ContainerInfomanager.Instance.GetItemNumber (configData.advItemID);
			if ( goodsCount >= configData.advItemCount)
			{
				if(!isCanClick)
					isCanClick = false;
			}
			else
			{
				if(isCanClick)
					isCanClick = false;
			}
		}
		return isCanClick;
	}
	//总技能伤害
	//技能总伤害=向下取整（(向下取整((参数1+参数2+参数3)/参数4)×参数4) /10）
	public float SkillHurt(int skillID)
	{
		SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
		SSkillInfo? curSkill = GetCurSkill(skillID);
		int skillLv = curSkill == null ? 1 : (int)(curSkill.Value.wSkillLV);
		if (skillLv >= configData.m_skillDamageList.Count) {
			TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到该等级配置信息");	
		}
		return configData.m_skillDamageList[skillLv-1]*0.1f;
//		return Mathf.FloorToInt ((Mathf.FloorToInt((configData.m_skillDamage[0]*skillLv*skillLv+
//		                                            configData.m_skillDamage[1]*skillLv+configData.m_skillDamage[2])/configData.m_skillDamage[3])*configData.m_skillDamage[3])/100)*10;
	}
}