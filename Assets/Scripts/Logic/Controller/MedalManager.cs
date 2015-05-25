using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class MedalManager : ISingletonLifeCycle
{
    private static MedalManager m_instance;
    public static MedalManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new MedalManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }

    private Dictionary<Int64, MedalInfo> m_MedalDic = new Dictionary<long, MedalInfo>();

    //注册
    public void RegisterMedal(Int64 playerUID, int prestigeLevel ,MedalEffectBehaviour medalEffectBehaviour)
    {
        //if (m_MedalDic.ContainsKey(playerUID))
        //{
        //    m_MedalDic[playerUID] = new MedalInfo() { PrestigeLevel = prestigeLevel, MedalEffectBehaviour = medalEffectBehaviour };            
        //}
        //else
        //{
        //    m_MedalDic.Add(playerUID, new MedalInfo() { PrestigeLevel = prestigeLevel, MedalEffectBehaviour = medalEffectBehaviour });            
        //}
        m_MedalDic[playerUID] = new MedalInfo() { PrestigeLevel = prestigeLevel, MedalEffectBehaviour = medalEffectBehaviour }; 
    }

    private void HideMedal(Int64 playerUID, bool active)
    {        
        if (m_MedalDic.ContainsKey(playerUID))
        {
            if (m_MedalDic[playerUID] != null)
            {
                //m_MedalDic[playerUID].gameObject.SetActive(active);
                m_MedalDic[playerUID].MedalEffectBehaviour.SetMedalActive(active);
            }
            else
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"m_MedalDic[playerUID] is null");
            }
        }
        else
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"m_MedalDic[playerUID] is not found : " + playerUID);
        }
    }

    private void DeleteMedal(Int64 playerUID)
    {
        if (m_MedalDic.ContainsKey(playerUID))
        {            
            GameObject.Destroy(m_MedalDic[playerUID].MedalEffectBehaviour.gameObject);
            m_MedalDic.Remove(playerUID);
        }        
    }

    public void SetHeroMedal(bool active)
    {
        var player = PlayerManager.Instance.FindHeroDataModel();
        this.HideMedal(player.UID, active);
    }

    /// <summary>
    /// 更新勋章
    /// </summary>
    /// <param name="playerUID">玩家UID</param>
    /// <param name="prestigeLevel">当前威望等级</param>
    public void UpdateHeroMedal(Int64 playerUID)
    {
        if (m_MedalDic.ContainsKey(playerUID))
        {
            //TraceUtil.Log("[m_MedalDic.ContainsKey]");
            int prestigeLevel = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL;
            if (m_MedalDic[playerUID].PrestigeLevel < prestigeLevel)
            {
                m_MedalDic[playerUID].DeleteMedalEffect();
                PlayerFactory.Instance.CreateMedal(playerUID, prestigeLevel);
            }
        }
        else
        {
            //TraceUtil.Log("[!m_MedalDic.ContainsKey]");
            int prestigeLevel = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL;
            PlayerFactory.Instance.CreateMedal(playerUID, prestigeLevel);
        }
    }


    public class MedalInfo
    {
        public int PrestigeLevel;//威望等级
        public MedalEffectBehaviour MedalEffectBehaviour;//勋章脚本

        public void DeleteMedalEffect()
        {
            GameObject.Destroy(MedalEffectBehaviour.gameObject);
        }
    }

    public void Instantiate()
    {
    }

    public void LifeOver()
    {
        m_instance = null;
    }
}


