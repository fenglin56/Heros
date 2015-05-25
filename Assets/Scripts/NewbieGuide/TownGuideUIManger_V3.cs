using UnityEngine;
using System.Collections;

/// <summary>
/// TownUI场景中的NewbieGuideUI物体
/// </summary>
//public class TownGuideUIManger_V3 : View {

//    /// <summary>
//    /// 任务面板
//    /// </summary>
//    public GameObject TaskPanel;
//    /// <summary>
//    /// 城镇对话框面板
//    /// </summary>
//    public GameObject GuideDialogPanel;
//    /// <summary>
//    /// 任务完成特效1
//    /// </summary>
//    public GameObject ComplateEffect;
//    /// <summary>
//    /// 任务完成特效2
//    /// </summary>
//    public GameObject TaskComplateEffectB;
//    /// <summary>
//    /// 引导遮罩
//    /// </summary>
//    public BoxCollider NebieGuideMask;
//    public Camera UICamera;
//    /// <summary>
//    /// 引导提示框
//    /// </summary>
//    private GameObject m_btnSignPanel;  
//    /// <summary>
//    /// 被拖拽按钮亮框或者是引导按钮亮框
//    /// </summary>
//    private GameObject m_sourceFrame;  
//    /// <summary>
//    /// 被拖拽目标区域亮框
//    /// </summary>
//    private GameObject m_targetFrame;  
//    /// <summary>
//    /// 拖拽的手
//    /// </summary>
//    private GameObject m_draggingArrow;
//    /// <summary>
//    /// 当前引导数据
//    /// </summary>
//    private GuideConfigData m_curGuideData;
//    /// <summary>
//    /// 当前任务数据
//    /// </summary>
//    private TaskConfigData m_curTaskData;
//    /// <summary>
//    /// 任务面板脚本组件
//    /// </summary>
//    private TaskPanel_V3 m_taskPanel;
//    /// <summary>
//    ///  当前引导序号
//    /// </summary>
//    private int m_guideOrder = 0;
//    /// <summary>
//    /// 当前引导活动按钮
//    /// </summary>
//    private GameObject m_guideActBtn;
//    // Use this for initialization
//    void Awake()
//    {
//        this.RegisterEventHandler();
//    }

//    // Use this for initialization
//    void Start () {
	
//    }
	
//    // Update is called once per frame
//    void Update () {
	
//    }
//    /// <summary>
//    /// 启动任务面板
//    /// </summary>
//    /// <param name="taskData"></param>
//    public void StartTaskGuide(TaskConfigData taskData)
//    {
//        if (taskData != null)
//        {
//            m_curTaskData = taskData;

//            if (m_taskPanel == null)
//            {
//                m_taskPanel = (Instantiate(TaskPanel) as GameObject).GetComponent<TaskPanel_V3>();
//                m_taskPanel.transform.parent = this.transform;
//                m_taskPanel.transform.localPosition = new Vector3(0, 0, 150);
//                m_taskPanel.transform.localScale = Vector3.one;
//                m_taskPanel.GetComponent<UIAnchor>().uiCamera = UICamera;
//            }

//            SoundManager.Instance.PlaySoundEffect("Sound_Voice_GetNewQuest");

//            m_taskPanel.InitTaskPanel(taskData);

//            if (taskData._GuideType == 1)  //强引导&弱引导
//                NewbieGuideManager_V2.Instance.IsConstraintGuide = true;
//            else
//                NewbieGuideManager_V2.Instance.IsConstraintGuide = false;
//        }
//    }
//    /// <summary>
//    /// 单击引导按钮后的回调函数
//    /// </summary>
//    /// <param name="notifyArgs"></param>
//    private void NextGuideHandle(object obj)
//    {       
//        if (m_curGuideData._GuideType == 2)  //如果是拖动引导，则不响应点击
//        {
//            return;
//        }
//        if (obj != null)
//        {
//            TraceUtil.Log("NextGuideHandle" + (string)obj);
//        }
//        if (m_guideActBtn)
//        {
//            if (m_guideActBtn.GetComponent<GuideButtonEvent>())
//                m_guideActBtn.GetComponent<GuideButtonEvent>().IsEnable = true;
//        }
//        //GuideBtnManager.Instance.GetButtonList[m_curGuideData._GuideBtnID[0]].AddComponent<GuideButtonEvent>();
//        ResetNewbieGuide();
//    }
//    /// <summary>
//    /// 重置新手引导
//    /// </summary>
//    private void ResetNewbieGuide()
//    {
//        if (m_guideOrder < NewbieGuideManager_V2.Instance.TownGuideList.Count - 1)
//        {
//            m_guideOrder += 1;
//            //ContinueGuide();
//        }
//        else
//        {
//            m_guideOrder = 0;
//            StopGuideHandle(null);
//            //NetServiceManager.Instance.InteractService.SendEctypeDialogOver();
//        }
//    }
//    /// <summary>
//    /// 引导结束处理，1、对话引导结束，2、服务器下发引导结束，3、要引导的按钮不存在时，4、重置新手引导时
//    /// </summary>
//    /// <param name="notifyArgs"></param>
//    private void StopGuideHandle(object obj)
//    {
//        if (m_sourceFrame != null)
//            DestroyImmediate(m_sourceFrame);
//        if (m_btnSignPanel != null)
//            DestroyImmediate(m_btnSignPanel);
//        if (m_draggingArrow != null)
//            DestroyImmediate(m_draggingArrow);
//        if (m_targetFrame != null)
//            DestroyImmediate(m_targetFrame);

//        if (m_taskPanel != null)
//            m_taskPanel.ResetContinue();

//        m_guideOrder = 0;

//        if (m_curGuideData != null)
//        {
//            GuideBtnManager.Instance.CloseGuide(m_curGuideData._GuideBtnID[0]); //TODO 如果拖动引导是否就没有按钮引导？
//        }
//        NewbieGuideManager_V2.Instance.DragSourceBtnID = 0;
//        NewbieGuideManager_V2.Instance.DragTargetBtnID = 0;

//        GuideBtnManager.Instance.IsEndGuide = true;
//    }
//    protected override void RegisterEventHandler()
//    {
//        UIEventManager.Instance.RegisterUIEvent(UIEventType.ClickOtherButton, StopGuideHandle); //弱引导时点击非引导按钮，停止引导步骤
//        UIEventManager.Instance.RegisterUIEvent(UIEventType.ClickTheGuideBtn, NextGuideHandle); //点击引导按钮，触发下一步引导
//    }
//}
