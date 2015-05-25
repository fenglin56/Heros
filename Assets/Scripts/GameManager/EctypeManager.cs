using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EctypeManager : ISingletonLifeCycle
{
	private static EctypeManager m_instance;
	public static EctypeManager Instance
	{
		get{
			if(m_instance == null)
			{
				m_instance = new EctypeManager();
				SingletonManager.Instance.Add(m_instance);
			}
			return m_instance;
		}
	}

	private SMSGEctypeInitialize_SC m_curEctypeProps = new SMSGEctypeInitialize_SC();
	private Dictionary<int,TeammateEctypeInitialize> m_teammateEctypeInitializeDict = new Dictionary<int, TeammateEctypeInitialize>();

	public void Set(SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC)
	{
		m_curEctypeProps = sMSGEctypeInitialize_SC;
	}

	public void UpdateProp(int actorID,int index, int value)
	{
		if(actorID == PlayerManager.Instance.FindHeroDataModel().ActorID)
		{
			m_curEctypeProps = m_curEctypeProps.SetValue(index, value);
			switch((SMSGEctypeInitialize_SC.EctypeMemberFields)index)
			{
			case SMSGEctypeInitialize_SC.EctypeMemberFields.ECTYPE_MEMBER_FIELD_YAONVSKILLTIMES:
				UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeSirenSkillPropUpdate, null);
				break;
			case SMSGEctypeInitialize_SC.EctypeMemberFields.ECTYPE_MEMBER_FIELD_MEDICAMENTTIMES:
				UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeMedicamentPropUpdate, null);
				break;
			case SMSGEctypeInitialize_SC.EctypeMemberFields.ECTYPE_MEMBER_FIELD_RELIVETIMES:
				UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeRelivePropUpdate, null);
				break;
			}
		}
		else
		{
			if(!m_teammateEctypeInitializeDict.ContainsKey(actorID))
			{
				m_teammateEctypeInitializeDict.Add(actorID, new TeammateEctypeInitialize(){
					teammateActorID = actorID,
					reliveTimes = value,
				});
			}
			else
			{
				m_teammateEctypeInitializeDict[actorID] = new TeammateEctypeInitialize(){
					teammateActorID = actorID,
					reliveTimes = value,
				};
			}
		}
	}

	public SMSGEctypeInitialize_SC GetEctypeProps()
	{
		return m_curEctypeProps;
	}
	/// <summary>
	/// 获取队友复活次数
	/// </summary>
	/// <returns>The teammate relive times.</returns>
	/// <param name="teammateActorID">Teammate actor I.</param>
	public int GetTeammateReliveTimes(int teammateActorID)
	{
		int time = 0;
		if(m_teammateEctypeInitializeDict.ContainsKey(teammateActorID))
		{
			time = m_teammateEctypeInitializeDict[teammateActorID].reliveTimes;
		}
		return time;
	}

	public EctypeContainerData GetCurrentEctypeData()
	{
		if(m_curEctypeProps.dwEctypeContainerId != 0)
		{
			return EctypeConfigManager.Instance.EctypeContainerConfigList[m_curEctypeProps.dwEctypeContainerId];
		}
		return null;
	}
	
	public int GetSirenSkillSurplusValue()
	{
		int value = 0;
		if(m_curEctypeProps.dwEctypeContainerId != 0)
		{
			value = EctypeConfigManager.Instance.EctypeContainerConfigList[m_curEctypeProps.dwEctypeContainerId].SirenSkillVaule-m_curEctypeProps.dwYaoNvSkillTimes;
		}
		return value;
	}

	public bool IsCrusadeEctypeUnlock(int ectypeID)
	{
		EctypeContainerData ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList.Values.SingleOrDefault(p=>
				p.lEctypeContainerID == ectypeID);
		var playerData = PlayerManager.Instance.FindHeroDataModel();
		if(ectypeContainerData.lMinActorLevel > playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL)
		{
			return  false;
		}
		return true;
	}

	public void Instantiate ()
	{
	}
	
	public void LifeOver ()
	{
		m_instance = null;
	}

}
