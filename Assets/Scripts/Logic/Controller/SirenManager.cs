using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;

public class SirenManager : ISingletonLifeCycle
{
    private static SirenManager m_instance;
    public static SirenManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SirenManager();
                SingletonManager.Instance.Add(m_instance);
            }            
            return m_instance;
        }
    }

    private List<SYaoNvContext> m_YaoNvContextList = new List<SYaoNvContext>();
	private List<SYaoNvCondtionInfo> m_YaoNvConditionList = new List<SYaoNvCondtionInfo>();


    /// <summary>
    /// 添加妖女
    /// </summary>
    /// <param name="YaoNvArray">SYaoNvContext[]</param>
    public void AddYaoNvContext(SYaoNvContext[] YaoNvArray)
    {
        m_YaoNvContextList.AddRange(YaoNvArray);
    }

    /// <summary>
    /// 获得妖女列表
    /// </summary>
    public List<SYaoNvContext> GetYaoNvList()
    {
        return m_YaoNvContextList;
    }

	/// <summary>
	/// 是否已经收服此妖女
	/// </summary>
	/// <returns><c>true</c> if this instance is own siren the specified sirenID; otherwise, <c>false</c>.</returns>
	/// <param name="sirenID">妖女iD</param>
	public bool IsOwnSiren(int sirenID)
	{
		return m_YaoNvContextList.Any(p=>p.byYaoNvID == sirenID);
	}

    /// <summary>
    /// 更新妖女炼化等级
    /// </summary>
    /// <param name="lianhuaMsg"></param>
    public void UpdateYaoNvContext(SMsgActionLianHua_SC lianhuaMsg)
    {
        if (m_YaoNvContextList.Any(p => p.byYaoNvID == lianhuaMsg.byYaoNvID))
        {
            for (int i = 0; i < m_YaoNvContextList.Count; i++)
            {
                if (m_YaoNvContextList[i].byYaoNvID == lianhuaMsg.byYaoNvID)
                {
                    var yaoNvContext = m_YaoNvContextList[i];
                    yaoNvContext.byLevel = lianhuaMsg.byLianHuaLevel;
					yaoNvContext.lExperience = lianhuaMsg.dwCurXiuWeiNum;
                    m_YaoNvContextList[i] = yaoNvContext;
                }
            }
        }
        else
        {
            m_YaoNvContextList.Add(new SYaoNvContext()
            {
                byYaoNvID = lianhuaMsg.byYaoNvID,
                byLevel = lianhuaMsg.byLianHuaLevel,
				byAssembly = 0,
            });
        }        
    }

	public void UpdateYaoNvJoin(byte sirenID)
	{
		for(int i=0;i<m_YaoNvContextList.Count;i++)
		{
			if (m_YaoNvContextList[i].byYaoNvID == sirenID)
			{
				var yaoNvContext = m_YaoNvContextList[i];
				yaoNvContext.byAssembly = 1;
				m_YaoNvContextList[i] = yaoNvContext;				
			}
			else
			{
				var yaoNvContext = m_YaoNvContextList[i];
				yaoNvContext.byAssembly = 0;
				m_YaoNvContextList[i] = yaoNvContext;				
			}
		}
	}

	#region 妖女收服条件

	public void AddYaoNvCondition(SYaoNvCondtionInfo[] sYaoNvCondtionInfos)
	{
		m_YaoNvConditionList.AddRange(sYaoNvCondtionInfos);
	}
	public List<SYaoNvCondtionInfo> GetConditionList()
	{
		return m_YaoNvConditionList;
	}
	public void UpdateYaoNvCondition(SYaoNvCondtionInfo info)
	{
		for(int i=0;i<m_YaoNvConditionList.Count;i++)
		{
			if(m_YaoNvConditionList[i].byYaoNvID == info.byYaoNvID)
			{
				m_YaoNvConditionList[i] = info;
				break;
			}
		}
	}

	#endregion

	/// <summary>
	/// 是否存在妖女可炼化或者可突破
	/// </summary>
	/// <returns><c>true</c> if this instance is has siren satisfy increase; otherwise, <c>false</c>.</returns>
	public bool IsHasSirenSatisfyIncrease()
	{
		bool isHas = false;
		if(SirenDataManager.Instance == null)
		{
			return false;
		}
		var sirenConfig = SirenDataManager.Instance.GetPlayerSirenList();
		for(int i=0;i<sirenConfig.Count;i++)
		{
			var sirenInfo = GetYaoNvList().SingleOrDefault(p => p.byYaoNvID == sirenConfig[i]._sirenID);
			var playerData = PlayerManager.Instance.FindHeroDataModel();
			if(sirenInfo.byLevel <= 0 || sirenInfo.byLevel > playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL)//如果妖女未解锁或等级小于等于玩家等级
			{
				continue;
			}
			var sirenThisLvData = sirenConfig[i]._sirenConfigDataList.SingleOrDefault(p=>p._growthLevels == sirenInfo.byLevel);

			//是否突破
			if(sirenInfo.byLevel >= sirenThisLvData.BreakStageMaxLevel)
			{
				bool isEnough = true;
				for(int j=0;j< sirenThisLvData.BreakCondition.Length;j++)
				{
					int hadNum = ContainerInfomanager.Instance.GetItemNumber(sirenThisLvData.BreakCondition[j].ItemID);
					if(hadNum < sirenThisLvData.BreakCondition[j].ItemNum)
					{
						isEnough = false;
					}
				}
				if(isEnough)//够材料突破
				{
					isHas = true;
					break;
				}
			}
			else
			{
				if(sirenInfo.lExperience + playerData.PlayerValues.PLAYER_FIELD_PRACTICE_NUM>=sirenThisLvData._growthCost)//够修为升级
				{
					isHas = true;
					break;
				}
			}
		}
		return isHas;
	}



    public void Instantiate()
    {
    }

    public void LifeOver()
    {
        m_instance = null;
    }
}
