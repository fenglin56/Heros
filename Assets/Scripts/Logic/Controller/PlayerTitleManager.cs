using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerTitleManager : ISingletonLifeCycle
{
	private static PlayerTitleManager m_instance;
	public static PlayerTitleManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = new PlayerTitleManager();
				SingletonManager.Instance.Add(m_instance);
			}
			return m_instance;
		}
	}

	private Dictionary<Int64, GameObject> m_TitleDict = new Dictionary<long, GameObject>();
	//private Dictionary<Int64, MedalInfo> m_MedalDic = new Dictionary<long, MedalInfo>();
	
	//注册
//	public void RegisterMedal(Int64 playerUID, int prestigeLevel ,MedalEffectBehaviour medalEffectBehaviour)
//	{
//		//m_MedalDic[playerUID] = new MedalInfo() { PrestigeLevel = prestigeLevel, MedalEffectBehaviour = medalEffectBehaviour }; 
//	}

	public void RegisterTitle(Int64 playerUID,  GameObject titleEffectGameObj)
	{
		m_TitleDict[playerUID] = titleEffectGameObj;
	}
	public void UpdateTitle(Int64 playerUID)
	{
		if(!m_TitleDict.ContainsKey(playerUID))
		{
			m_TitleDict.Add(playerUID, null);
		}
		if(m_TitleDict[playerUID] != null)
		{
			GameObject.Destroy(m_TitleDict[playerUID]);
		}
		//var playerModel = PlayerManager.Instance.GetEntityMode(playerUID);
		//PlayerFactory.Instance.CreateTitle(playerUID, playerModel.GO.transform);
	}



	private void HideMedal(Int64 playerUID, bool active)
	{        
//		if (m_MedalDic.ContainsKey(playerUID))
//		{
////			if (m_MedalDic[playerUID] != null)
////			{
////				//m_MedalDic[playerUID].gameObject.SetActive(active);
////				m_MedalDic[playerUID].MedalEffectBehaviour.SetMedalActive(active);
////			}
////			else
////			{
////				TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"m_MedalDic[playerUID] is null");
////			}
//		}
//		else
//		{
//			TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"m_MedalDic[playerUID] is not found : " + playerUID);
//		}
	}
	
	private void DeleteMedal(Int64 playerUID)
	{
//		if (m_MedalDic.ContainsKey(playerUID))
//		{            
//			GameObject.Destroy(m_MedalDic[playerUID].MedalEffectBehaviour.gameObject);
//			m_MedalDic.Remove(playerUID);
//		}        
	}
	
	public void SetHeroMedal(bool active)
	{
		var player = PlayerManager.Instance.FindHeroDataModel();
		this.HideMedal(player.UID, active);
	}

	public void Instantiate()
	{
	}
	
	public void LifeOver()
	{
		m_instance = null;
	}
}
	
	/// <summary>
	/// 更新勋章
	/// </summary>
	/// <param name="playerUID">玩家UID</param>
	/// <param name="prestigeLevel">当前威望等级</param>
//	public void UpdateHeroMedal(Int64 playerUID)
//	{
//		if (m_MedalDic.ContainsKey(playerUID))
//		{
//			//TraceUtil.Log("[m_MedalDic.ContainsKey]");
////			int prestigeLevel = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL;
////			if (m_MedalDic[playerUID].PrestigeLevel < prestigeLevel)
////			{
////				m_MedalDic[playerUID].DeleteMedalEffect();
////				PlayerFactory.Instance.CreateMedal(playerUID, prestigeLevel);
////			}
//		}
//		else
//		{
//			//TraceUtil.Log("[!m_MedalDic.ContainsKey]");
////			int prestigeLevel = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL;
////			PlayerFactory.Instance.CreateMedal(playerUID, prestigeLevel);
//		}
//	}
	
	
//	public class MedalInfo
//	{
//		public int PrestigeLevel;//威望等级
//		public MedalEffectBehaviour MedalEffectBehaviour;//勋章脚本
//		
//		public void DeleteMedalEffect()
//		{
//			GameObject.Destroy(MedalEffectBehaviour.gameObject);
//		}
//	}
	



