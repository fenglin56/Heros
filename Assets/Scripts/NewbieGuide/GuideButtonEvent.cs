
using UnityEngine;
using System.Collections;
using System;

public class GuideButtonEvent : MonoBehaviour
{
    /// <summary>
    /// 自定义引导按钮激活委托处理，主要用于控制引导按钮的子物体的激活控制
    /// </summary>
    private Action<bool> m_customerGuideBtnEnableHandler;  
    private bool m_isEnable = true;
    /// <summary>
    /// 是否引导中
    /// </summary>
    public bool IsGuidingBtn{get;private set;}
    public int BtnId;

    public bool IsEnable 
    { 
        get { return m_isEnable; } 
        set { m_isEnable = value; } 
    }

	// Use this for initialization
    void OnClick()
    {
        if (m_isEnable)
        {
            UIEventManager.Instance.TriggerUIEvent(IsGuidingBtn ? UIEventType.ClickTheGuideBtn : UIEventType.ClickOtherButton, BtnId);
        }
    }
    /// <summary>
    /// 初始化委托
    /// </summary>
    /// <param name="customerGuideBtnEnableHandler"></param>
    public void InitCustomerBtnEnableHandler(Action<bool> customerGuideBtnEnableHandler)
    {
        m_customerGuideBtnEnableHandler = customerGuideBtnEnableHandler;
    }
    /// <summary>
    /// 设置引导按钮的激活状态
    /// </summary>
    /// <param name="flag"></param>
    public void SetActHandler(bool flag,bool triggerEvent)
    {
        IsGuidingBtn = flag;
        if (triggerEvent&&m_customerGuideBtnEnableHandler != null)
        {
            m_customerGuideBtnEnableHandler(flag);
        }
    }
}
