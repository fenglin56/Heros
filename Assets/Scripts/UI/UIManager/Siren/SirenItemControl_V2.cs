using UnityEngine;
using System.Collections;
using UI.Siren;
using System.Linq;
using System.Collections.Generic;
using System;
public class SirenItemControl_V2
{

    private int m_CurLevel = 0;//当前等级，初始为0

    private PlayerSirenConfigData m_PlayerSirenConfigData;

    public Dictionary<int, List<SirenGrowthEffect>> EffectDict = new Dictionary<int, List<SirenGrowthEffect>>();

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

        //关联委托
        m_SelectedDelegae = selected;

        //加成信息
        //显示属性加成信息
        m_PlayerSirenConfigData._sirenConfigDataList.ApplyAllItem(p =>
        {
            string[] growthItem = p._growthEffect.Split('|');
            int growthItemLength = growthItem.Length;
            List<SirenGrowthEffect> effectList = new List<SirenGrowthEffect>();
            for (int i = 0; i < growthItemLength; i++)
            {
                string[] growthEffect = growthItem[i].Split('+');
                //growthEffect[0] 属性名称
                //growthEffect[1] 属性加成
                var effectData = ItemDataManager.Instance.GetEffectData(growthEffect[0]);
                if (effectData != null)
                {
                    SirenGrowthEffect sirenGrowthEffect = new SirenGrowthEffect()
                    {
                        EffectData = effectData,
                        GrowthEffectValue = Convert.ToInt32(growthEffect[1])
                    };
                    effectList.Add(sirenGrowthEffect);
                }
            }
            EffectDict.Add(p._growthLevels, effectList);
        });
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

    public PlayerSirenConfigData GetPlayerSirenConfigData()
    {
        return m_PlayerSirenConfigData;
    }

    public List<SirenGrowthEffect> GetSirenGrowthEffect()
    {
        return EffectDict[m_CurLevel];
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
    /// 炼化进度
    /// </summary>
    /// <returns>string</returns>
    public string GetProcessValue()
    {
        int curLevel = m_CurLevel - 1;
        int maxLevel = m_PlayerSirenConfigData._growthMaxLevel - 1;
        return curLevel.ToString() + "/" + maxLevel.ToString();
    }

    //上发炼化请求
    public bool SendLianHuaMsg()
    {
        bool isSendSuccess = false;
        int copper = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
        if (copper < m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._growthCost)
        {
            //铜币不足
            UI.MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_231"), 1f);
            return false;
        }

        //int itemNum = UI.MainUI.ContainerInfomanager.Instance.GetItemNumber(m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._composeCost_itemID);        
        //TraceUtil.Log("[炼妖物品]ID: " + m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._composeCost_itemID + " , number: " + itemNum);
        //if (itemNum < m_PlayerSirenConfigData._sirenConfigDataList[m_CurLevel]._composeCost_itemNum)
        //{
        //    //物品不足
        //    return;
        //}
        if (!IsMaxLevel())
        {
            int nextLevel = m_CurLevel + 1;
            TraceUtil.Log("NetServiceManager.Instance.EntityService.SendLianHua(" + m_PlayerSirenConfigData._sirenID + ", " + nextLevel);
            NetServiceManager.Instance.EntityService.SendLianHua(m_PlayerSirenConfigData._sirenID, nextLevel);
            isSendSuccess = true;
        }
        else
        {
            TraceUtil.Log("[妖女满级]");
        }

        return isSendSuccess;
    }

    public void OnButtonClick(object obj)
    {
        m_SelectedDelegae(m_PlayerSirenConfigData._sirenID);
    }
}
