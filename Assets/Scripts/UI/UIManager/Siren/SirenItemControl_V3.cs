using UnityEngine;
using System.Collections;
using UI.Siren;
using System.Linq;
using System.Collections.Generic;
using System;
using UI;


public class SirenItemControl_V3 
{
	private int m_CurLevel = 0;//当前等级，初始为0
	public int CurLevel{get{return m_CurLevel;}}

	private PlayerSirenConfigData m_PlayerSirenConfigData;
	
	public Dictionary<int, List<SirenGrowthEffect>> EffectDict = new Dictionary<int, List<SirenGrowthEffect>>();
	private string[] m_UnlockTexts;
	
	private int m_CurExperience = 0;
	public int CurExperience{get{return m_CurExperience;}}
	public int MaxExperience{get{return m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._growthCost;}}

	public delegate void SelectedSirenDelegate(int sirenID);
	private SelectedSirenDelegate m_SelectedDelegae;
	
	private int m_guideBtnID = 0;
	
	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="data">PlayerSirenConfigData</param>
	/// <param name="selected">SelectedSirenDelegate</param>
	public void Init(PlayerSirenConfigData data, SelectedSirenDelegate selected)
	{
		//赋值
		m_PlayerSirenConfigData = data;
		var sirenInfo = SirenManager.Instance.GetYaoNvList().SingleOrDefault(p => p.byYaoNvID == data._sirenID);
		m_CurLevel = sirenInfo.byLevel;//sirenInfo可能是空值
		m_CurExperience = sirenInfo.lExperience;

		//关联委托
		m_SelectedDelegae = selected;
		
		//加成信息
		//显示属性加成信息
		m_PlayerSirenConfigData._sirenConfigDataList.ApplyAllItem(p =>
		                                                          {
			string[] growthItem = p._GrowthEffect.Split('|');
			string[] growthMaxItem = p._MaxGrowthEffect.Split('|');
			int growthItemLength = growthItem.Length;
			List<SirenGrowthEffect> effectList = new List<SirenGrowthEffect>();
			for (int i = 0; i < growthItemLength; i++)
			{
				string[] growthEffect = growthItem[i].Split('+');
				string[] growthMaxEffect = growthMaxItem[i].Split('+');
				//growthEffect[0] 属性名称
				//growthEffect[1] 属性加成
				var effectData = ItemDataManager.Instance.GetEffectData(growthEffect[0]);
				if (effectData != null)
				{
					SirenGrowthEffect sirenGrowthEffect = new SirenGrowthEffect()
					{
						EffectData = effectData,
						GrowthEffectValue = Convert.ToInt32(growthEffect[1]),
						GrowthEffectMaxValue = Convert.ToInt32(growthMaxEffect[1])
					};
					effectList.Add(sirenGrowthEffect);
				}
			}
			EffectDict.Add(p._growthLevels, effectList);
		});

		//解锁条件
		m_UnlockTexts = m_PlayerSirenConfigData._UnlockText.Split('|');

	}
	
	void OnDestroy()
	{
		//TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
	}
	
	public SirenConfigData GetSirenConfigData()
	{
		var data = m_PlayerSirenConfigData._sirenConfigDataList.SingleOrDefault(p => p._growthLevels == m_CurLevel);
		return data;
	}

	public SirenConfigData GetNextLevelSirenConfigData()
	{
		var data = m_PlayerSirenConfigData._sirenConfigDataList.SingleOrDefault(p => p._growthLevels == m_CurLevel+1);		
		return data;
	}
	
	public PlayerSirenConfigData GetPlayerSirenConfigData()
	{
		return m_PlayerSirenConfigData;
	}
	
	public List<SirenGrowthEffect> GetSirenGrowthEffect()
	{
		return EffectDict[m_CurLevel];
	}
	public List<SirenGrowthEffect> GetNextSirenGrowthEffect()
	{
		int nextLevel = m_CurLevel + (IsMaxLevel() ? 0:1);

		return EffectDict[nextLevel];
	}

	public UnlockCondition[] GetSirenUnlockCondition()
	{
		return m_PlayerSirenConfigData.Unlock;
	}
	public string[] GetSirenUnlockTxt()
	{
		return m_UnlockTexts;
	}

	/// <summary>
	/// 是否满级
	/// </summary>
	/// <returns>bool</returns>
	public bool IsMaxLevel()
	{
		return m_CurLevel >= m_PlayerSirenConfigData._growthMaxLevel;
	}

	/// <summary>
	/// 是否此阶段满级
	/// </summary>
	/// <returns><c>true</c> if this instance is break stage max level; otherwise, <c>false</c>.</returns>
	public bool IsBreakStageMaxLevel()
	{
		return CurLevel >= m_PlayerSirenConfigData._sirenConfigDataList[CurLevel].BreakStageMaxLevel;
	}

	/// <summary>
	/// 是否解锁
	/// </summary>
	/// <returns>bool</returns>
	public bool IsUnlock()
	{
		return m_CurLevel > 0;
	}
	
	public void UpdateView(int lianHuaLevel)
	{
		m_CurLevel = lianHuaLevel;
		this.OnButtonClick(null);
	}
	
	/// <summary>
	/// 炼化进度(此阶段)
	/// </summary>
	/// <returns>string</returns>
	public string GetProcessValue()
	{
		int curLevel = GetSirenConfigData()._growthLevels;
		//int maxLevel = m_PlayerSirenConfigData._growthMaxLevel;
		int maxLevel = m_PlayerSirenConfigData._sirenConfigDataList[CurLevel].BreakStageMaxLevel;
		return curLevel.ToString() + "/" + maxLevel.ToString();
	}
	/// <summary>
	/// 炼化进度百分比
	/// </summary>
	/// <returns>The process value.</returns>
	public float GetProcessPercentage()
	{
		int curLevel = m_CurLevel - 1;
		int maxLevel = m_PlayerSirenConfigData._growthMaxLevel - 1;
		return curLevel * 1f / maxLevel;
	}

	public string GetLevelUpText()
	{
		return m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._LevelUpText;;
	}

	//上发炼化请求
	public bool SendLianHuaMsg()
	{
		bool isSendSuccess = false;
//		int copper = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
//		if (copper < m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._growthCost)
//		{
//			//铜币不足
//			UI.MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_231"), 1f);
//			return false;
//		}
		
		//int itemNum = UI.MainUI.ContainerInfomanager.Instance.GetItemNumber(m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._composeCost_itemID);        
		//TraceUtil.Log("[炼妖物品]ID: " + m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._composeCost_itemID + " , number: " + itemNum);
		//if (itemNum < m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._composeCost_itemNum)
		//{
		//    //物品不足
		//    return;
		//}

		//int popNeedExp = m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._GrowthCost;		
		//修为不足
		var playerData = PlayerManager.Instance.FindHeroDataModel();
		if(playerData.PlayerValues.PLAYER_FIELD_PRACTICE_NUM > 0)
		{
			
			isSendSuccess = true;
		}
		else
		{
			MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I2_7"),1);
			return false;
		}

		var context = SirenManager.Instance.GetYaoNvList().SingleOrDefault(p=>p.byYaoNvID == m_PlayerSirenConfigData._sirenID);
		int needExp = m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._growthCost - context.lExperience;
		int popExp = 0;
		if( playerData.PlayerValues.PLAYER_FIELD_PRACTICE_NUM >= needExp)
		{
			popExp = needExp - playerData.PlayerValues.PLAYER_FIELD_PRACTICE_NUM;
		}
		else
		{
			popExp = playerData.PlayerValues.PLAYER_FIELD_PRACTICE_NUM;
		}

		if(IsUnlock())
		{
			if (!IsMaxLevel())
			{		
				//NetServiceManager.Instance.EntityService.SendLianHua(m_PlayerSirenConfigData._sirenID, nextLevel);
				NetServiceManager.Instance.EntityService.SendLianHua(m_PlayerSirenConfigData._sirenID, 
				                                                     EntityService.YaoNvOpType.upgrade,popExp);
				
				isSendSuccess = true;
			}
			else
			{
				isSendSuccess = false;
				TraceUtil.Log("[妖女满级]");
			}
		}
		else
		{
			isSendSuccess = true;
			NetServiceManager.Instance.EntityService.SendLianHua(m_PlayerSirenConfigData._sirenID, 
			                                                     EntityService.YaoNvOpType.unlockNormal,0);
		}

		
		return isSendSuccess;
	}
	
	public void OnButtonClick(object obj)
	{
		m_SelectedDelegae(m_PlayerSirenConfigData._sirenID);
	}

}
