using UnityEngine;
using System.Collections;
using System;

public class GuideClick : MonoBehaviour {
    /// <summary>
    /// 按钮Id
    /// </summary>
    public int BtnId;

    public Action<GuideClick> ClickAct;
    void OnClick( )
    {
		if(!GuideBtnManager.Instance.IsEndGuide)
		{
            UIEventManager.Instance.TriggerUIEvent(UIEventType.ClickTheGuideBtn, BtnId);
	        this.gameObject.RemoveComponent<GuideClick>("GuideClick");
		}
        if (ClickAct != null)
        {
            ClickAct(this);
        }
    }

}
