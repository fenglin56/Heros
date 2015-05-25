using UnityEngine;
using System.Collections;
using UI.MainUI;
using System;

/// <summary>
/// 任务引导按钮的注册和销毁脚本
/// </summary>
public class GuideBtnBehaviour : View {
    public UIType MainUiType;
    public BtnMapId_Sub SubBtnIdType;
    [HideInInspector]
    public bool ResponseOnClickEvent;
    public int BtnId;
    public int MappingId;  //与BtnId映射的Id值，可以不唯一用于查找按钮在TaskBtnManager中的注册

    /// <summary>
    /// 引导按钮的偏移光圈
    /// </summary>
    public GameObject BtnFrame { get; private set; }
    /// <summary>
    /// 引导按钮的偏移箭头
    /// </summary>
    public GameObject BtnArrow { get; private set; }
    private Action<float> m_noticeToDragSlerp;
    private Func<UIDraggablePanel, float, IEnumerator> m_dragAmountSlerpAct;
    private UIDraggablePanel m_panel;
    private float m_itemAmount;
    private Vector3 m_frameOffset = Vector3.zero, m_arrowOffset = Vector3.zero;
    void Awake()
    {
        ResponseOnClickEvent = true;
        if (BtnId == 0)
        {
            RegisterBtnMappingId(MainUiType, SubBtnIdType);
        }
    }
    public void SetBtnGuideFrame(GameObject btnFrame, Vector3 frameOffset, GameObject btnArrow, Vector3 arrowOffset)
    {
        BtnFrame = btnFrame;
        BtnArrow = btnArrow;
        m_frameOffset = frameOffset;
        m_arrowOffset = arrowOffset;
    }
    public void RegisterBtnMappingId(UIType mainUiType, BtnMapId_Sub subBtnIdType)
    {
		RegisterBtnMappingId (mainUiType,subBtnIdType,null,0);
    }
	public void RegisterBtnMappingId(UIType mainUiType, BtnMapId_Sub subBtnIdType, Action<float> noticeToDragSlerp, float itemAmount)
	{
		TaskBtnManager.Instance.DelGuideButton(BtnId);
		//ResponseOnClickEvent = keepBtnId;
		MainUiType = mainUiType;
		SubBtnIdType = subBtnIdType;
		BtnId = TaskBtnManager.Instance.RegGuideButton(gameObject, mainUiType, subBtnIdType);
		MappingId = BtnId;
		m_noticeToDragSlerp = noticeToDragSlerp;
		m_itemAmount = itemAmount;
		SetBtnColliderState();
	}
    public void RegisterBtnMappingId(int mappingId, UIType mainUiType, BtnMapId_Sub subBtnIdType)
    {
        RegisterBtnMappingId(mappingId, mainUiType, subBtnIdType, null,  0);
    }    
    public void RegisterBtnMappingId(int mappingId, UIType mainUiType, BtnMapId_Sub subBtnIdType
        , Action<float> noticeToDragSlerp, float itemAmount)
    {        
        TaskBtnManager.Instance.DelGuideButton(BtnId);
        MainUiType = mainUiType;
        SubBtnIdType = subBtnIdType;
        MappingId = mappingId;
        BtnId = TaskBtnManager.Instance.RegGuideButton(gameObject, mainUiType, subBtnIdType);
        m_noticeToDragSlerp = noticeToDragSlerp;
        m_itemAmount = itemAmount;
        SetBtnColliderState();
    }
    private void SetBtnColliderState()
    {
        if (MappingId != 0)
        {
            //var runningTask = TaskModel.Instance.FindRuningTaskState();
            //if (runningTask != null && runningTask.TaskNewConfigData.GuideType == TaskGuideType.Enforce)
            if(TaskModel.Instance.TaskGuideType==TaskGuideType.Enforce)
            {
                TaskBtnManager.Instance.FindGuideBtnParamViaMappingId(MappingId).BtnCollider.enabled = false;
            }
            TownGuideManager.Instance.SetGuideUIReady(MappingId, true);
        }
    }
    /// <summary>
    /// 跟随
    /// </summary>
    void Update()
    {
        var worldPos=transform.position;

        if (BtnFrame != null)
        {
            BtnFrame.transform.position = worldPos;
            BtnFrame.transform.localPosition += m_frameOffset;
        }
        if (BtnArrow != null)
        {
            BtnArrow.transform.position = worldPos;
            BtnArrow.transform.localPosition += m_arrowOffset;
        }
    }
    /// <summary>
    /// 滚动到指定项
    /// </summary>
    public void DragAmountSlerp()
    {
        //if (m_dragAmountSlerpAct != null)
        //{
        //    StartCoroutine(m_dragAmountSlerpAct(m_panel, m_itemAmount));
        //}
        if (m_noticeToDragSlerp != null)
        {
            m_noticeToDragSlerp(m_itemAmount);
        }
    }
    void OnDestroy()
    {
        TaskBtnManager.Instance.DelGuideButton(BtnId);
        GameObject.Destroy(BtnFrame);
        GameObject.Destroy(BtnArrow);
    }
    // Use this for initialization
    void OnClick()
    {
        if (!ResponseOnClickEvent)
            return;
        bool isGuidingBtn=TaskBtnManager.Instance.GuidingBtnId==BtnId;
		TraceUtil.Log ("OnClick GuidingBtnId=="+TaskBtnManager.Instance.GuidingBtnId+"BtnId="+BtnId);
        // TraceUtil.Log(SystemModel.Rocky,"GuideBtnBehaviour OnClick  BtnId:" + BtnId + "  GuidingBtnId:" +TaskBtnManager.Instance.GuidingBtnId);
        switch(TaskModel.Instance.TaskGuideType)
        {
            case TaskGuideType.None:  //无引导，不处理
                RaiseEvent(EventTypeEnum.ClickNoneGuideBtn.ToString(), null);
                return;
            case TaskGuideType.Weak:  //弱引导
                UIEventManager.Instance.TriggerUIEvent(isGuidingBtn ? UIEventType.ClickTheGuideBtn : UIEventType.ClickOtherButton, BtnId);
                break;
            case TaskGuideType.Enforce: //强引导
                if (isGuidingBtn)
                {
                    UIEventManager.Instance.TriggerUIEvent(UIEventType.ClickTheGuideBtn ,BtnId);
                }
                break;
        }
    }

    protected override void RegisterEventHandler()
    {
    }
}
