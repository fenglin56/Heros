using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// BattleUI场景中的NewbieGuideUI游戏物件上。
/// </summary>
public class EctypeGuideUIManager : MonoBehaviour {

    //public GameObject EctypeGuidePanel;
    //private GameObject m_ectypeGuidePanel;
    //private Dictionary<int, EctypeGuideStepConfigData> m_guideList = new Dictionary<int, EctypeGuideStepConfigData>();  //副本引导数据列表
    ////private Dictionary<int, EctypeGuideStepConfigData> m_stepList = new Dictionary<int, EctypeGuideStepConfigData>();
    //private bool m_isEctypeGuide = false;
    

    ///// <summary>
    ///// 之前Awake的处理放在Start里，为了保证处理HP&MP按钮时可以知道当前副本的引导状态，2014-5-8转入Awake处理
    ///// </summary>
    //void Awake()
    //{
    //    NewbieGuideManager_V2.Instance.IsConstraintGuide = false;
    //    UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeGuideStep, UpdateStepState);
    //    //如果场景尚未初始化，则监听初始化消息，否则直接初始化。取副本ID进行任务启动
    //    if (GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
    //    {
    //        GameDataManager.Instance.dataEvent.RegisterEvent(DataType.InitializeEctype, Init);
    //    }
    //    else
    //    {
    //        Init(null);
    //    }
    //}
    //void Update()
    //{
    //}
    //void Init(object obj)
    //{
    //    GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.InitializeEctype, Init);
    //    SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
    //    CurSelectEctype(sMSGEctypeInitialize_SC.dwEctypeContainerId); 
    //}

    ///// <summary>
    ///// 副本初始化是判定进否进行副本新手引导
    ///// </summary>
    ///// <param name="onSelectEctypeData">当前所选择的副本</param>
    //public void CurSelectEctype(int ectypeid)
    //{
    //    if (GuideConfigManager.Instance.EctypeGuideConfigList.ContainsKey(ectypeid))
    //    {
    //        TraceUtil.Log("收到副本信息，并且有副本引导数据");
    //        EctypeGuideConfigData guideItem = GuideConfigManager.Instance.EctypeGuideConfigList[ectypeid];

    //        m_guideList.Clear();
    //        int[] steps = GameManager.Instance.UseJoyStick ? guideItem._JoyStickStepList : guideItem._StepList;
    //        foreach (int step in steps)
    //        {
    //            if (GuideConfigManager.Instance.EctypeGuideStepConfigList.ContainsKey(step))
    //            {
    //                EctypeGuideStepConfigData stepItem = GuideConfigManager.Instance.EctypeGuideStepConfigList[step];
    //                m_guideList.Add(stepItem._GuideStep, stepItem);                    
    //            }
    //        }

    //        m_isEctypeGuide = true;  //进行副本新手引导,
    //        NewbieGuideManager_V2.Instance.IsEctypeGuide = true;
    //        //副本有引导数据，需要等引导步骤下发才解开技能按钮
    //        NewbieGuideManager_V2.Instance.EctypeGuideStepReached = false;
    //    }
    //    else
    //    {
    //        m_guideList.Clear();
    //        m_isEctypeGuide = false;
    //        //副本没有引导数据，直接打开
    //        NewbieGuideManager_V2.Instance.EctypeGuideStepReached = true;
    //        NewbieGuideManager_V2.Instance.IsEctypeGuide = false ;
    //    }
    //}
    //void SetGuideBtnEnable(bool flag) 
    //{
    //    var guideButtonList = GuideBtnManager.Instance.GetButtonList;
    //    foreach (var btn in guideButtonList.Values)
    //    {
    //        if (!btn.GuideBtn)
    //        {
    //            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,btn.UIType + "  " + btn.SubUIType + "  ");
    //            break;
    //        }
    //        var skillComponent = btn.GuideBtn.GetComponent<UI.Battle.BattleSkillButton>();
    //        if (skillComponent != null)
    //        {
    //            skillComponent.SetButtonStatus(flag);

    //        }
    //        else
    //        {
    //            var healthComponent = btn.GuideBtn.GetComponent<UI.Battle.HealthAndMagicButton>();
    //            if (healthComponent != null)
    //                healthComponent.SetMyButtonActive(flag);
    //        }
    //    }

    //}
    //void UpdateStepState(object obj)
    //{
    //    TraceUtil.Log("收到步骤信息");
    //    //第一次步骤到达会执行
    //    if (!NewbieGuideManager_V2.Instance.EctypeGuideStepReached )
    //    {
    //        NewbieGuideManager_V2.Instance.EctypeGuideStepReached = true;
    //        //开放技能和翻滚，爆气按钮
    //        SetGuideBtnEnable(true);
    //    }
    //    SC_GuideStepInfo sC_GuideStepInfo = (SC_GuideStepInfo)obj;

    //    if (!GameManager.Instance.IsNewbieGuide)
    //       // return;

    //    if (!m_isEctypeGuide) return;

    //    if (!m_guideList.ContainsKey(sC_GuideStepInfo.dwStepID))
    //    {
    //        TraceUtil.Log("当前副本不包含此引导步数据！");
    //        return;
    //    }

    //    //TraceUtil.Log("##########IsEctypeGuide" + NewbieGuideManager_V2.Instance.IsEctypeGuide);

    //    EctypeGuideStepConfigData stepItem = m_guideList[sC_GuideStepInfo.dwStepID];
		
    //    //if(!m_stepList.ContainsKey(stepItem._GuideStep))
    //    //{
    //    //    m_stepList.Add(stepItem._GuideStep, stepItem);
    //    //}
		
    //    switch(sC_GuideStepInfo.byStepStute) //步骤状态 nStatus = 1 正执行状态 nStatus = 2 已完成状态
    //    {
    //        case 1:
    //            if (m_ectypeGuidePanel == null)
    //            {
    //                m_ectypeGuidePanel = (GameObject)GameObject.Instantiate(EctypeGuidePanel, this.transform.position, Quaternion.identity);
    //                m_ectypeGuidePanel.transform.parent = this.transform;
    //                m_ectypeGuidePanel.transform.localScale = Vector3.one;
    //            }

    //            GuideBtnManager.Instance.IsEndGuide = false;
    //            if (stepItem._StepType == 9)  //图片向导
    //            {
    //                m_ectypeGuidePanel.GetComponent<EctypeGuidePanel>().StartPicStep(stepItem);
    //            }
    //            else
    //            {
    //                m_ectypeGuidePanel.GetComponent<EctypeGuidePanel>().StartGuideStep(stepItem);
    //            }
    //            break;
    //        case 2:
    //            if (m_ectypeGuidePanel != null)
    //                m_ectypeGuidePanel.GetComponent<EctypeGuidePanel>().ComplateGuideStep(stepItem);

    //            //m_stepList.Remove(stepItem._GuideStep);

    //            //if(m_stepList.Count > 0)
    //            //     Invoke("NextStep", 1f);
    //            break;
    //        default:
    //            break;
    //    }
    //}

    ///// <summary>
    ///// 下一步
    ///// </summary>
    //void NextStep()
    //{
    //    //m_ectypeGuidePanel.GetComponent<EctypeGuidePanel>().StartGuideStep(m_stepList[0]);
    //}

    //void OnDestroy()
    //{
    //    GuideBtnManager.Instance.IsEndGuide = true;
    //    UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeGuideStep, UpdateStepState);
    //}

    ////void OnGUI()
    ////{
    ////    if (GUILayout.Button("Step1开始任务"))
    ////    {
    ////        UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeGuideStep, new SC_GuideStepInfo { dwStepID = 32, dwStepType = 9, byStepStute = 1 });
    ////    }

    ////    if (GUILayout.Button("Step1完成任务"))
    ////    {
    ////        UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeGuideStep, new SC_GuideStepInfo { dwStepID = 1, dwStepType = 1, byStepStute = 2 });
    ////    }

    ////    if (GUILayout.Button("Step2开始任务"))
    ////    {
    ////        UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeGuideStep, new SC_GuideStepInfo { dwStepID = 2, dwStepType = 1, byStepStute = 1 });
    ////    }
    ////}
}
