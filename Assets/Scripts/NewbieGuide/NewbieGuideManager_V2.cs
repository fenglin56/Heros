using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;


/// <summary>
/// 城镇及副本引导数据管理
/// </summary>
//public class NewbieGuideManager_V2:ISingletonLifeCycle {

//    /// <summary>
//    /// 城镇引导数据
//    /// </summary>
//    private List<GuideConfigData> m_townGuideList = new List<GuideConfigData>();        //城镇引导数据列表
//    /// <summary>
//    /// 当前执行任务
//    /// </summary>
//    private TaskConfigData m_curExecuteTask;  
//    /// <summary>
//    /// 新任务等待新功能开启延时时间。读取任务配置表字段_NewFunDelayTime
//    /// </summary>
//    private float m_nextTaskdelayTime;            //新任务等待新功能开启延时时间

//    private static NewbieGuideManager_V2 m_instance;
//    public static NewbieGuideManager_V2 Instance
//    {
//        get
//        {
//            if (m_instance == null)
//            {
//                m_instance = new NewbieGuideManager_V2();
//                SingletonManager.Instance.Add(m_instance);
//            }
//            return m_instance;
//        }
//    }
//    /// <summary>
//    /// 是否副本引导
//    /// </summary>
//    public bool IsEctypeGuide { set; get; }
//    /// <summary>
//    /// 服务器副本步骤引导消息是否到达
//    /// --用于在消息未到达时锁定界面，第一次步骤消息到达，解除所有按钮
//    /// --在技能按钮管理器 BattleSkillButtonManager 的ShowButton方法会检测这个值
//    /// </summary>
//    public bool EctypeGuideStepReached;

//    /// <summary>
//    /// 接收服务器任务更新数据，包括引导执行和引导结束【城镇引导】
//    /// 初始分任务对应的引导数据【任务表与引导数据表用 GuideId关联】
//    /// 前后两个任务，服务器会保证前一个任务的结束消息先于后任务的开始消息到达
//    /// </summary>
//    /// <param name="sTaskLogUpdate"></param>
//    public void ReceiveTaskState(STaskLogUpdate sTaskLogUpdate)
//    {
//        TraceUtil.Log("收到任务数据");
//        if (!GameManager.Instance.IsNewbieGuide)
//            return;

//        var taskItem = GuideConfigManager.Instance.TaskConfigList.SingleOrDefault(P => P._TaskID == sTaskLogUpdate.nTaskID);

//        if (taskItem == null)
//        {
//            TraceUtil.Log("当前任务配置表中无任务ID为" + sTaskLogUpdate.nTaskID + "的任务");
//            return;
//        }

//        //判断当前任务是否有引导数据
//        if (/*taskItem._EctypeGuideList.Length <= 0 &&*/ taskItem._TownGuideList.Length <= 0)
//            return;

//        switch (sTaskLogUpdate.nStatus)
//        {
//            case 1: //当前任务在执行状态
//                m_curExecuteTask = taskItem;

//                if (taskItem._TownGuideList.Length > 0)
//                {
//                    UpdateTownGuideData();
//                    if (m_nextTaskdelayTime <= 0)
//                    {
//                        //TraceUtil.Log("无需等待，开始新任务：");
//                        m_isStartTask = true;
//                    }
//                    else
//                    {
//                        //需要等待，会在CheckNextTaskStart方法里检测时间并开启
//                        m_isStartTask = false;
//                    }
//                }

//                break;
//            case 2: //任务完成通知
//                    m_isComplateTaskFromServer = true;
//                    // 判断是否开启新功能按钮
//                    if (m_curExecuteTask != null)
//                    {
//                        m_enableButton = m_curExecuteTask._EnableFunc;
//                        m_nextTaskdelayTime = m_curExecuteTask._NewFunDelayTime*0.001f;
//                    }
//                    TaskFinish();

//                break;
//            default:
//                break;
//        }
//    }
//    private void TaskFinish()
//    {
//        m_curExecuteTask = null;
//        this.m_townGuideList.Clear();
//    }
//    /// <summary>
//    /// 检查是否到时间开启新任务，为了等待新功能按钮开启动画完成
//    /// </summary>
//    public void CheckNextTaskStart()
//    {
//        if (m_curExecuteTask == null)  //如果没有新任务，直接返回
//        {
//            return;
//        }
//        if (m_nextTaskdelayTime > 0)
//        {
//            if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN
//                && GameManager.Instance.SceneLoaded)
//            {
//                m_nextTaskdelayTime -= Time.deltaTime;
//                //TraceUtil.Log("城镇场景加载完成：  " + Time.realtimeSinceStartup);
//            }
//            //TraceUtil.Log("还需要等：" + m_nextTaskdelayTime);
//        }
//        else
//        {
//            //TraceUtil.Log("时间到，开始新任务：");
//            m_isStartTask = true;
//        }
//    }
//    /// <summary>
//    /// 当前执行任务
//    /// </summary>
//    public TaskConfigData ExecuteTask
//    {
//        get {return m_curExecuteTask; }
//    }
//    /// <summary>
//    /// 是否可以开启引导任务开关，控制TownGuideUIManager_V2里面的FixedUpdate的执行，依赖于m_nextTaskdelayTime变量时间控制
//    /// </summary>
//    public bool m_isStartTask = false;
//    /// <summary>
//    /// 引导任务是否完成开关，控制TownGuideUIManager_V2里面的FixedUpdate的执行
//    /// </summary>
//    public bool m_isComplateTaskFromServer = false;
//    public UI.MainUI.UIType m_enableButton = 0;
//    /// <summary>
//    /// 是否约束引导
//    /// </summary>
//    public bool IsConstraintGuide { set; get; }
//    /// <summary>
//    /// 更新当前城镇引导数据
//    /// </summary>
//    private void UpdateTownGuideData()
//    {
//        m_townGuideList.Clear();
//        foreach(var item in m_curExecuteTask._TownGuideList)
//        {
//            var guideItem = GuideConfigManager.Instance.TownGuideConfigList[item];

//            if (guideItem != null)
//            {
//                m_townGuideList.Add(guideItem);
//            }
//        }
//    }
//    /// <summary>
//    /// 城镇引导数据
//    /// </summary>
//    public List<GuideConfigData> TownGuideList
//    {
//        get { return m_townGuideList; }
//    }


//    //public List<EctypeHelperConfigData> GuideHelpList
//    //{
//    //    get { return GuideConfigManager.Instance.EctypeHelperConfigList; }
//    //}

//    public int DragTargetBtnID { set;private get; }
//    public int DragSourceBtnID { set;private get; }

//    public void IsDragGuide(int targetBtnID, int sourceBtnID)
//    {
//        //TraceUtil.Log("@@@@@@@@@@@@@@@@@@targetBtnID" + targetBtnID);
//        //TraceUtil.Log("@@@@@@@@@@@@@@@@@@DragTargetBtnID" + DragTargetBtnID);
//        //TraceUtil.Log("$$$$$$$$$$$$$$$$$$$sourceBtnID" + sourceBtnID);
//        //TraceUtil.Log("$$$$$$$$$$$$$$$$$$$DragSourceBtnID" + DragSourceBtnID);
//        if (DragSourceBtnID == sourceBtnID && DragTargetBtnID == targetBtnID)
//        {
//            UIEventManager.Instance.TriggerUIEvent(UIEventType.ClickTheGuideBtn, null);
//        }
//        else
//            UIEventManager.Instance.TriggerUIEvent(UIEventType.ClickOtherButton, null);
//    }
//    public void LifeOver()
//    {
//        m_curExecuteTask = null;
//        m_instance=null;
//    }

//    public void Instantiate()
//    {

//    }

//}
