using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;
using System.Linq;

/// <summary>
/// 引导UI组件
/// </summary>
public class TaskGuideUIManager : View {

    /// <summary>
    /// 任务面板
    /// </summary>
    public GameObject TaskPanel;
	public GameObject taskGuidePanel;
	private UI.MainUI.TaskGuidePanel m_taskGuidePanel;
    /// <summary>
    /// 新功能开启特效
    /// </summary>
    //public GameObject TaskFunctionActiveEffect1;
    /// <summary>
    /// 任务完成特效2
    /// </summary>
    //public GameObject TaskFunctionActiveEffect2;
    /// <summary>
    /// 引导遮罩
    /// </summary>
    public BoxCollider NebieGuideMask;
    public Camera UICamera;
	public Transform npcTalkParent;
    public GameObject NpcTitle;
    public GameObject TaskListManager;//任务列表
    public GameObject TaskAwardPrefab;  //任务奖励面板
    public GameObject TaskNpcTalkPanelGo;
    public GameObject StoryDialogMask; //场景对话遮罩预设
    public GameObject AutoWalkEffPrefab;  //自动寻路特效
    public GameObject AutoWalkEffPoint;
    public GameObject SkipStoryPrefab;  //跳过剧情对话按钮

    public GameObject[] StoryDialogPrefabs;
    private int m_currnetTalkIndex = 0;  //当前对话索引，用于取剧情对话引导中的对话内容
//	public bool isNpcTalking = false;//对话正在进行//
    private TaskQuickGuidePanelBehaviour m_taskPanel;
    private GameObject[] m_taskNPCs;   //任务剧情NPC列表
    private List<GameObject> m_NPCTitles;   //NpcTitle列表

    private int m_taskFinishAppearButton;  //任务完成后开启新功能的按钮ID
    private TaskGuideExtendData m_newGuideConfigData;   //当前引导数据   
    private UIEventListener m_storyDialogClickEvent;
    private GameObject m_showingNpcTalkPanel;  //正在显示的NPC对话框
    private GameObject m_autoWalkEffObj;
    private GameObject m_taskAwardGo;  //任务奖励面板
    private GameObject m_storyPanel;
    private LocalButtonCallBack m_skipStoryBtn; //路过剧情
    void Awake()
    {
        m_taskNPCs = new GameObject[16];
        m_NPCTitles = new List<GameObject>();
        if (TaskModel.Instance != null)
        {
            if (!MakeMainNPC())
            {
                MakeDefaultNPC();
            }
            TaskModel.Instance.AcceptTaskAct = () => MakeMainNPC();
        }
        RegisterEventHandler();

        TownGuideManager.Instance.m_guideIndex = 0;
        TaskBtnManager.Instance.UIRoot = transform;
    }
	// Use this for initialization
    void Start()
    {
        
        //InitTaskQuickPanel();
		InitTaskGuidePanel ();
        if (TaskModel.Instance.CacheFinishTastState != null)
        {
            TaskFinished(TaskModel.Instance.CacheFinishTastState);
            TaskModel.Instance.CacheFinishTastState = null;
        }
        if (TownGuideManager.Instance.NeedCacheFinishGuide())
        {
			RaiseEvent(EventTypeEnum.TaskFinishGuideExecuteInvoke.ToString(), null);
        }
        else
        {
			//jamfing 20141201//
            /*var runningTask = TaskModel.Instance.FindRuningTaskState();
            if (runningTask != null)
            {
                //有任务需要处理
                RaiseEvent(EventTypeEnum.TaskExecuteInvoke.ToString(), null);
            }
            else  //
            {
				//jamfing 20141124//
                //TaskModel.Instance.TriggerMainTask();
            }*/
        }
        
    }
    private bool MakeNPC(TaskState taskState)
    {
        bool flag = false;
        //初始化剧情NPC，
        if (taskState != null && taskState.TaskNewConfigData != null)
        {
            ResetStoryNPC(taskState.TaskNewConfigData);
            flag = true;
        }
        return flag;
    }
    private bool MakeMainNPC()
    {
        var mainTask = TaskModel.Instance.MainTaskState;
        return MakeNPC(mainTask);
    }
    private bool MakeDefaultNPC()
    {
        var defaultTask = TaskModel.Instance.DefaultNPCTask;
        return MakeNPC(defaultTask);
    }
    private void InitTaskQuickPanel()
    {
        m_taskPanel = (Instantiate(TaskPanel) as GameObject).GetComponent<TaskQuickGuidePanelBehaviour>();
        m_taskPanel.transform.parent = this.transform;
        m_taskPanel.transform.localPosition = new Vector3(0, 0, 150);
        m_taskPanel.transform.localScale = Vector3.one;
        m_taskPanel.GetComponent<UIAnchor>().uiCamera = UICamera;

        m_taskPanel.InitTaskPanel(TaskModel.Instance.FindNextQuickGuideTask());
        m_taskPanel.ClickTaskStateAct = (taskState) =>
            {
                TaskModel.Instance.ManualTriggerTask(taskState);
                m_taskPanel.InitTaskPanel(TaskModel.Instance.FindNextQuickGuideTask());
            };

        RefreshTaskQuickInfo(null);
    }
	private void InitTaskGuidePanel()
	{
		m_taskGuidePanel = (Instantiate(taskGuidePanel) as GameObject).GetComponent<UI.MainUI.TaskGuidePanel>();
		m_taskGuidePanel.transform.parent = this.transform;
		m_taskGuidePanel.transform.localPosition = new Vector3(0, 0, 150);
		m_taskGuidePanel.transform.localScale = Vector3.one;
		m_taskGuidePanel.GetComponent<UIAnchor>().uiCamera = UICamera;
		
		m_taskGuidePanel.ShowPanel();
		/*m_taskGuidePanel.ClickTaskStateAct = (taskState) =>
		{
			TaskModel.Instance.ManualTriggerTask(taskState);
			m_taskGuidePanel.InitTaskPanel(TaskModel.Instance.FindNextQuickGuideTask());
		};*/
		RefreshTaskQuickInfo(null);
	}

	private void TaskExecuteInvokeHandle(INotifyArgs args)
	{
		TaskBtnManager.Instance.RemoveGuideFrame();
		m_taskGuidePanel.ShowPanel();
		//m_taskPanel.ShowOrHideTips();
		var runningTaskConfigData=TaskModel.Instance.FindRuningTaskState();
		//任务触发时，如果是强引导，关闭所有引导管理按钮。否则，开放所有按钮
		TaskBtnManager.Instance.ResetAllButtonStatus(runningTaskConfigData.TaskNewConfigData.GuideType!=TaskGuideType.Enforce);
		if (runningTaskConfigData.TaskNewConfigData.CloseUI == 1)//收起主功能按钮
		{
			//前面已经关了//
			/*if (UI.SystemFuntionButton.Instance != null)
                {
                    UI.SystemFuntionButton.Instance.CloseBtnpanel();
                }*/
			MainUIController.Instance.CloseAllPanel();
			UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseAllUI, null);
		}
		StartCoroutine(CoroutineExecuteTaskGuide(runningTaskConfigData.TaskNewConfigData));
	}
    /// <summary>
    /// 延时启动引导
    /// </summary>
    /// <param name="runningTaskConfigData"></param>
    /// <returns></returns>
    private IEnumerator CoroutineExecuteTaskGuide(TaskNewConfigData runningTaskConfigData)
    {
		if (runningTaskConfigData.DelayTime != 0) {
			yield return new WaitForSeconds(runningTaskConfigData.DelayTime / 1000f);
		}
        if (m_showingNpcTalkPanel != null)
        {
            GameObject.Destroy(m_showingNpcTalkPanel);
        }
        ExecuteTaskGuide();
		yield break;
    }
    /// <summary>
    /// 任务完成后续引导检查处理
    /// </summary>
    /// <param name="args"></param>
    private void TaskFinishGuideInvokeHandle(INotifyArgs args)
    {
        if (m_newGuideConfigData != null)
        {
            m_newGuideConfigData.GuideBtnUIReadyAct = null;
        }
		m_newGuideConfigData = TownGuideManager.Instance.GetNextFinishGuideConfigData();
		if (m_newGuideConfigData != null)
        {
            if (m_newGuideConfigData.IsGuideUIReady)
            {
                ReadyToExecuteTaskGuid(true);
            }
            //else
            {
				if(m_newGuideConfigData != null)
               		m_newGuideConfigData.GuideBtnUIReadyAct = ReadyToExecuteTaskGuid;
            }
            //TaskGuideType taskGuideType = m_newGuideConfigData.NewGuideConfigDatas.TaskNewConfigData.GuideType;
            //if (taskGuideType == TaskGuideType.Enforce)
            //{
            //    TaskBtnManager.Instance.ResetAllButtonStatus(false);
            //}
            //switch (m_newGuideConfigData.NewGuideConfigDatas.GuideType)
            //{
            //    case GuideConfigType.NormalArrow:
            //    TaskBtnManager.Instance.SetGuideButton(m_newGuideConfigData);
            //        break;
            //    case GuideConfigType.FindMaterial:
            //        FindMaterial(m_newGuideConfigData);
            //        break;
            //    case GuideConfigType.TownStory:
            //        TownStory(m_newGuideConfigData);
            //        break;
            //}
        }
        else     //没有finish 引导，尝试执行缓存任务
        {
            //开启新功能
            if (m_taskFinishAppearButton != 0)
            {
                UIType uiType = (UIType)m_taskFinishAppearButton;
                UIEventManager.Instance.TriggerUIEvent(UIEventType.EnableMainButton, uiType);
                m_taskFinishAppearButton = 0;
            }
            //没有缓存要触发的任务则把所有禁用的引导激活
            if (TaskModel.Instance.CacheNextExecuteTaskState == null)
            {
                TaskBtnManager.Instance.ResetAllButtonStatus(true);
            }
            TaskModel.Instance.ManualTriggerCacheTask();
//            m_taskPanel.ShowOrHideTips();
			m_taskGuidePanel.ShowPanel();


        }
    }
    private void TaskAward(TaskState taskState)
    {
        if (taskState != null && m_taskAwardGo==null)
        {
            m_taskAwardGo = NGUITools.AddChild(transform.parent.gameObject, TaskAwardPrefab);
            if (m_taskAwardGo != null)
            {
                m_taskAwardGo.layer = TaskAwardPrefab.layer;
                m_taskAwardGo.transform.localPosition = TaskAwardPrefab.transform.localPosition;
                var taskFinishAwardBehaviour = m_taskAwardGo.GetComponent<TaskFinishAwardBehaviour>();
                taskFinishAwardBehaviour.FinishAct(taskState.TaskNewConfigData);
            }
        }
    }
    /// <summary>
    /// 启动引导(来源：1.开始任务时入口，2.点击完成任务某步骤执行下一步)
    /// </summary>
    private void ExecuteTaskGuide()
    {
        if (m_newGuideConfigData != null)
        {
            m_newGuideConfigData.GuideBtnUIReadyAct = null;
        }
        m_newGuideConfigData = TownGuideManager.Instance.GetNextGuideConfigData();
		if (m_newGuideConfigData != null) {
			//引导终止和跳过条件判定
			if (OverRuleCheck () || SkipRuleCheck ()) {
				TaskGuideFinish ();
				//ClickOtherButtonHandle(null);
			} else {
				//如果引导的UI已经装备完毕，直接执行，否则监听UI完成Action  
				//TaskBtnManager.Instance.ResetNextGuideMapping(m_newGuideConfigData);
				if (m_newGuideConfigData.IsGuideUIReady) {
					ReadyToExecuteTaskGuid (true);
				}
				//会有递归//
				if(m_newGuideConfigData != null)
					m_newGuideConfigData.GuideBtnUIReadyAct = ReadyToExecuteTaskGuid;
			}
		} else {
			if (TownGuideManager.Instance.IsLastGuideOfTask ()) {
				TaskModel.Instance.SendTaskMessageToServer(0,2,2,TaskModel.Instance.CurrentTaskId.Value);		
			}		
		}
    }
    private void ReadyToExecuteTaskGuid(bool isReady)
    {
        if (!isReady || m_newGuideConfigData==null) return;
        //重置UI装备标记和准备委托
        //把该步骤的UI完成状态重置一下        
        TownGuideManager.Instance.SetGuideUIReady(m_newGuideConfigData.MappingId, false);
        //m_newGuideConfigData.GuideBtnUIReadyAct = null;    
        TaskGuideType taskGuideType = m_newGuideConfigData.NewGuideConfigDatas.TaskNewConfigData.GuideType;
        if (taskGuideType == TaskGuideType.Enforce)
        {
           TaskBtnManager.Instance.ResetAllButtonStatus(false);
        }    
		if (TownGuideManager.Instance.IsLastGuideOfTask ()) {
			TaskModel.Instance.SendTaskMessageToServer(0,2,2,m_newGuideConfigData.NewGuideConfigDatas.TaskNewConfigData.TaskID);		
		}
		switch (m_newGuideConfigData.NewGuideConfigDatas.GuideType) {
			case GuideConfigType.NormalArrow:
				TaskBtnManager.Instance.SetGuideButton (m_newGuideConfigData);
				break;
			case GuideConfigType.FindTask:
			case GuideConfigType.FindEctype:
			case GuideConfigType.FindEquipPosition://都是找按钮，其实都一样，没有特殊要求//
				FindEctype (m_newGuideConfigData);
				break;
			case GuideConfigType.OpenSpecUI:
			{
					UIEventManager.Instance.TriggerUIEvent (UIEventType.OpentMainUI, TaskBtnManager.Instance.GetUITypeByBtnId (m_newGuideConfigData));
					//当前步骤算了已完成，马上执行下一步//
					if (!TownGuideManager.Instance.IsLastGuideOfTask ())
						RaiseEvent(EventTypeEnum.ClickNoneGuideBtn.ToString(), null);
			}
        		break;
            case GuideConfigType.TownStory:
                TownStory(m_newGuideConfigData);
				break;
            case GuideConfigType.FindMaterial:
                FindMaterial(m_newGuideConfigData);
                break;
			case GuideConfigType.FindIntellSiren:
			{
				FindIntellJumpSiren(m_newGuideConfigData);
				if (!TownGuideManager.Instance.IsLastGuideOfTask ())
					RaiseEvent(EventTypeEnum.ClickNoneGuideBtn.ToString(), null);
			}
				break;
        }        
	}
	private void FindIntellJumpSiren(TaskGuideExtendData newGuideConfigData)
	{
		UIEventManager.Instance.TriggerUIEvent (UIEventType.IntellJumpSiren,newGuideConfigData.MappingId);
	}

    private void FindMaterial(TaskGuideExtendData newGuideConfigData)
    {
        int mappingId = newGuideConfigData.MappingId;        
        if (mappingId != 0)
        {
            var guideBtnParams = TaskBtnManager.Instance.FindGuideBtnParamViaMappingId(newGuideConfigData.MappingCategory,mappingId);
            if (guideBtnParams != null)
            {
                TaskBtnManager.Instance.SetGuideButton(guideBtnParams.BtnBehaviour.BtnId, newGuideConfigData);
                guideBtnParams.BtnBehaviour.DragAmountSlerp();   //移动到指定
            }
            else
            {
                TraceUtil.Log(SystemModel.Rocky, TraceLevel.Error, "Can't found ectype diff button at taskbtnmanager" + mappingId.ToString());
            }
        }
        else
        {
            TraceUtil.Log(SystemModel.Rocky, TraceLevel.Error, "Can't material guide " + newGuideConfigData.NewGuideConfigDatas.GuideBtnID);
        }
    }
    private void FindEctype(TaskGuideExtendData newGuideConfigData)
    {
        int mappingId = newGuideConfigData.MappingId;
        var guideBtnParams = TaskBtnManager.Instance.FindGuideBtnParamViaMappingId( mappingId);
        if (guideBtnParams != null)
        {
            TaskBtnManager.Instance.SetGuideButton(guideBtnParams.BtnBehaviour.BtnId, newGuideConfigData);
            guideBtnParams.BtnBehaviour.DragAmountSlerp();   //移动到指定
        }
        else
        {
            TraceUtil.Log(SystemModel.Rocky, TraceLevel.Error, "Can't found ectype diff button at taskbtnmanager" + mappingId.ToString());
        }
    }
    /// <summary>
    /// 城镇副本剧情
    /// </summary>
    /// <param name="newGuideConfigData"></param>
    private void TownStory(TaskGuideExtendData newGuideConfigData)
    {
		if(TaskModel.Instance.isNpcTalking)
			return;
        //close all the ui panel
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UIType.Empty);
		//send event to joystick of TownUI
		UIEventManager.Instance.TriggerUIEvent(UIEventType.OnNpcGuideStartEvent, null);
        //保存UI按钮状态并隐藏
        TaskBtnManager.Instance.SaveAndResetAllButtonStatus(false);
        TaskBtnManager.Instance.RemoveGuideFrame();
        //找到剧情NPC，根据配置的偏移和角度。寻路过去指定点
        string[] targetPosVal=newGuideConfigData.NewGuideConfigDatas.StroyPos.Split('+');
        Vector3 targetPos = Vector3.zero;
        targetPos=targetPos.GetFromServer(float.Parse(targetPosVal[0]), float.Parse(targetPosVal[1]));
        //开启剧情播放
        var playerBehaviour = (PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour;
        var nav = PlayerManager.Instance.HeroAgent;
        nav.enabled = true;
        nav.ResetPath();
        nav.speed = playerBehaviour.WalkSpeed;
        nav.updateRotation = true;
        nav.updatePosition = true;
        nav.SetDestination(targetPos);
       
        playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerToNpc);
        var camLookAt = newGuideConfigData.NewGuideConfigDatas.StroyCamera.Split('+');
        Vector3 camLookAtPos = Vector3.zero;
        camLookAtPos = camLookAtPos.GetFromServer(float.Parse(camLookAt[0]), float.Parse(camLookAt[1]));
        camLookAtPos.y = float.Parse(camLookAt[2]) * 0.1f;
        StartCoroutine(PathFinding(nav, camLookAtPos, newGuideConfigData, playerBehaviour));
    }
    /// <summary>
    /// 寻路到剧情人物
    /// </summary>
    /// <param name="navAgent"></param>
    /// <param name="npcTransform"></param>
    /// <param name="newGuideConfigData"></param>
    /// <param name="playerBehaviour"></param>
    /// <returns></returns>
    IEnumerator PathFinding(NavMeshAgent navAgent, Vector3 targetPos, TaskGuideExtendData newGuideConfigData, PlayerBehaviour playerBehaviour)
    {
		if (m_autoWalkEffObj == null) {
			//寻路特效
			m_autoWalkEffObj = NGUITools.AddChild (AutoWalkEffPoint, AutoWalkEffPrefab);
		} else {
			m_autoWalkEffObj.SetActive(true);
		}
        while (true)
        {
            yield return null;
            if(navAgent.pathPending)
            {
                continue;
            }            
            //点击屏幕，中断寻路
            if (!navAgent.enabled)
            {
				if(TaskModel.Instance.isNpcTalking)
					break;
				//其实这下面的都不需要//
                if (m_autoWalkEffObj != null)
                {
					m_autoWalkEffObj.SetActive(false);
                }
                //navAgent.Stop();
                playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerToIdle);
                //点击其他按钮，中断引导
                ClickOtherButtonHandle(null);
				//send event to joystick of TownUI
				UIEventManager.Instance.TriggerUIEvent(UIEventType.OnNpcGuideStopEvent, null);
                break;
            }
            if (!navAgent.hasPath)
            {
				TaskModel.Instance.isNpcTalking = true;
                if (m_autoWalkEffObj != null)
                {
					m_autoWalkEffObj.SetActive(false);
                }
                navAgent.enabled = false;
				UIEventManager.Instance.TriggerUIEvent(UIEventType.NpcTalkTaskDealUI, false);
                playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerToIdle);
                var cameraOffset = Vector3.zero;
                var cameraOffsetConfigString = newGuideConfigData.NewGuideConfigDatas.StroyCameraTarget;
                if (cameraOffsetConfigString != "0")
                {
                    var offset = cameraOffsetConfigString.Split('+');//A+-125+15+25
                    cameraOffset = cameraOffset.GetFromServer(float.Parse(offset[1]), float.Parse(offset[2]));
                    cameraOffset.y = float.Parse(offset[3]) * 0.1f;
                    BattleManager.Instance.FollowCamera.SetSmoothMoveTarget(targetPos, cameraOffset);
                }
                //转向
                StartCoroutine(UpdateRotation(playerBehaviour, newGuideConfigData.NewGuideConfigDatas));
                while (BattleManager.Instance.FollowCamera.IsInSmoothMove)
                {
                    yield return null;
                }
                //开始剧情对话
                var guideConfigData = newGuideConfigData.NewGuideConfigDatas;
                if (guideConfigData.TalkIdConfigDatas != null && guideConfigData.TalkIdConfigDatas.Length > 0)
                {
                    if (guideConfigData.TaskNewConfigData.TownStroyMusic != "0")
                    {
                        SoundManager.Instance.StopBGM();
                        SoundManager.Instance.PlayBGM(guideConfigData.TaskNewConfigData.TownStroyMusic);
                    }
                    //隐藏其他玩家
                    PlayerManager.Instance.HideAllPlayerButHero(true);
                    ShowDialog(guideConfigData);
					if(m_skipStoryBtn == null)
					{
                    	m_skipStoryBtn = NGUITools.AddChild(gameObject, SkipStoryPrefab).GetComponent<LocalButtonCallBack>();
					}
					else
					{
						m_skipStoryBtn.gameObject.SetActive(false);
						m_skipStoryBtn.gameObject.SetActive(true);
					}
                    m_skipStoryBtn.GetComponent<UIAnchor>().uiCamera = BattleManager.Instance.UICamera;
                    m_skipStoryBtn.SetCallBackFuntion((obj) =>
                    {
						if(m_newGuideConfigData != null)
						{
							StoryDialogFinish(m_newGuideConfigData.NewGuideConfigDatas, (bool)obj);
						}
						else
						{
							StartCoroutine(CameraSmoothBackFinish(false));
						}
						m_skipStoryBtn.gameObject.SetActive(false);
						TaskModel.Instance.isNpcTalking = false;
                        //Destroy(m_skipStoryBtn.gameObject);
                    },false);
                }
                break;
            }
        }
    }
    /// <summary>
    /// 主角转到正确角度后开始剧情对话
    /// </summary>
    /// <param name="npcTransform"></param>
    /// <param name="playerBehaviour"></param>
    /// <param name="newGuideConfigData"></param>
    /// <returns></returns>
    /// 
    IEnumerator UpdateRotation(PlayerBehaviour playerBehaviour, NewGuideConfigData newGuideConfigData)
    {
        var angle = float.Parse(newGuideConfigData.StroyPos.Split('+')[2]);
        while (true)
        {
            yield return null;
            Quaternion wantedRotation = Quaternion.Euler(new Vector3(0, angle, 0));
            float t = CommonDefineManager.Instance.CommonDefine.TurnRoundSpeed / Quaternion.Angle(playerBehaviour.ThisTransform.rotation, wantedRotation) * Time.deltaTime;
            playerBehaviour.ThisTransform.rotation = Quaternion.Lerp(playerBehaviour.ThisTransform.rotation, wantedRotation, t);

            if (t == 1 || float.IsInfinity(t))
            {
                break;
            }
        }
    }
    /// <summary>
    /// 显示剧情对话内容
    /// </summary>
    /// <param name="newGuideConfigData"></param>
    private void ShowDialog(NewGuideConfigData newGuideConfigData)
    {
        if (m_currnetTalkIndex < newGuideConfigData.TalkIdConfigDatas.Length)
        {            
            var talkIdConfigData = newGuideConfigData.TalkIdConfigDatas[m_currnetTalkIndex];
            int index = (int)talkIdConfigData.DialogPrefab - 1;
			if(m_storyPanel != null)
			{
				DestroyImmediate(m_storyPanel);
				m_storyPanel = null;
			}
			m_storyPanel = UI.CreatObjectToNGUI.InstantiateObj (StoryDialogPrefabs[index],npcTalkParent);// NGUITools.AddChild(gameObject, StoryDialogPrefabs[index]);
            var offsetPosStr=talkIdConfigData.TextPos.Split('+');
            var offsetPos=new Vector3(float.Parse(offsetPosStr[0]),float.Parse(offsetPosStr[1]),0);
            m_storyPanel.transform.localPosition = offsetPos;
            var storyPanelBehaviour = m_storyPanel.GetComponent<StoryDialogBehaviour>();
			talkIdConfigData.isTaskTalkMark = true;
            storyPanelBehaviour.Init(talkIdConfigData);
            storyPanelBehaviour.StoryGuideFinishAct = () =>
            {
                if (m_storyPanel != null)
                {
                    Destroy(m_storyPanel);
					m_storyPanel = null;
                }
                SoundManager.Instance.PlaySoundEffect("Sound_Button_TaskStory_Next");
                m_currnetTalkIndex++;
                ShowDialog(newGuideConfigData);            
            };
        }
        else
        {
            if (m_skipStoryBtn != null)
            {
				m_skipStoryBtn.gameObject.SetActive(false);
                //Destroy(m_skipStoryBtn.gameObject);
            }
            StoryDialogFinish(newGuideConfigData,false);
        }
    }
    private void StoryDialogFinish(NewGuideConfigData newGuideConfigData,bool isBreak)
    {
        if (m_storyPanel != null)
        {
            Destroy(m_storyPanel);
			m_storyPanel = null;
        }
        //send event to joystick of TownUI
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OnNpcTalkCloseEvent, null);
        m_storyDialogClickEvent = NGUITools.AddChild(gameObject, StoryDialogMask).GetComponent<UIEventListener>();
        BattleManager.Instance.FollowCamera.SmoothMoveTargetOriginal(PlayerManager.Instance.FindHeroEntityModel().Behaviour.transform);
        m_currnetTalkIndex = 0;
        GameManager.Instance.PlaySceneMusic();
        //32=与剧情人物对话（前端对话） 完成后向服务器发消息，告诉服务器任务完成
        if (!isBreak&&newGuideConfigData != null && newGuideConfigData.TaskNewConfigData.TaskType == 32)
        {
            SMsgInteractCOMMON_CS msgInteract;
            msgInteract.dwNPCID = 0;
            msgInteract.byOperateType = 2;
            msgInteract.dwParam1 = 2;
            msgInteract.dwParam2 = 0;
            msgInteract.byIsContext = 0;

            SMsgInteractCOMMONContext_CS msgContext;
            msgContext.szContext = new byte[32];
            NetServiceManager.Instance.InteractService.SendInteractCOMMON(msgInteract, msgContext);
        }
        StartCoroutine(CameraSmoothBackFinish(isBreak));
    }
    /// <summary>
    /// 等镜头返回再显示其他玩家和功能按钮
    /// </summary>
    /// <returns></returns>
    private IEnumerator CameraSmoothBackFinish(bool isBreak)
    {
        while (BattleManager.Instance.FollowCamera.IsInSmoothMove)
        {
            yield return null;            
        }
        GameObject.Destroy(m_storyDialogClickEvent.gameObject);
        //显示其他玩家
        PlayerManager.Instance.HideAllPlayerButHero(false);
		Debug.Log ("CameraSmoothBackFinish===="+isBreak);
        //恢复UI按钮状态并隐藏
        TaskBtnManager.Instance.RecoverAllButtonStatus();
		UIEventManager.Instance.TriggerUIEvent(UIEventType.NpcTalkTaskDealUI, true);
		TaskModel.Instance.isNpcTalking = false;
        if (!isBreak)
        {
            TaskGuideFinish();
        }
    }
    private void ResetStoryNPC(TaskNewConfigData runningTaskConfigData)
    {
        m_taskNPCs.ApplyAllItem(P => { if (P != null)GameObjectPool.Instance.Release(P); });
        m_NPCTitles.ApplyAllItem(P => { if (P != null) GameObjectPool.Instance.Release(P); });
        m_NPCTitles.Clear();
        for (int i = 0; i < 16; i++)
        {
            m_taskNPCs[i] = MakeNPC(i, runningTaskConfigData);
        }
    }
    private GameObject MakeNPC(int index,TaskNewConfigData runningTaskConfigData)
    {
        GameObject npc = null;
        StoryPersonInfo storyPersonInfo = runningTaskConfigData.GetStoryPersonInfo(index);
        
        if (storyPersonInfo != null&&storyPersonInfo.SrotyPersonPrefab!=null)
        {
            var pos = storyPersonInfo.PersonPos;
            Quaternion npcDir = Quaternion.Euler(0, storyPersonInfo.StartAngle, 0);
            npc = GameObjectPool.Instance.AcquireLocal(storyPersonInfo.SrotyPersonPrefab, pos, npcDir);
            if (npc.GetComponent<NPCBehaviour>()!=null)
            {
                npc.RemoveComponent<NPCBehaviour>("NPCBehaviour");
            }
            var tasknpcBehaviour = npc.GetComponent<TaskNPCBehaviour>();
            if (tasknpcBehaviour == null)
            {
                tasknpcBehaviour = npc.AddComponent<TaskNPCBehaviour>();
            }
            tasknpcBehaviour.InitTaskNPCData(storyPersonInfo);
            tasknpcBehaviour.ShowTaskNpcPanel = (personInfo) =>
                {
                    if (personInfo != null)
                    {
                        var playerBehaviour = (PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour;
                        if (playerBehaviour.FSMSystem.CurrentStateID == StateID.PlayerFindPathing)
                        {
                            PlayerManager.Instance.HeroAgent.enabled = false;
                            //playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerToIdle);
                        }
                        //显示面板
					var taskNpcTalkPanelBehaviour = UI.CreatObjectToNGUI.InstantiateObj (TaskNpcTalkPanelGo,npcTalkParent).GetComponent<TaskNpcTalkPanelBehaviour>();// NGUITools.AddChild(gameObject, TaskNpcTalkPanelGo).GetComponent<TaskNpcTalkPanelBehaviour>();
                        int storyBoxStyle = personInfo.StoryBoxStyle - 1;
                        var storyPanel = StoryDialogPrefabs[storyBoxStyle];
                        TalkIdConfigData talkIdConfigData = new TalkIdConfigData();
                        talkIdConfigData.DialogPrefab = (DialogBoxType)personInfo.StoryBoxStyle;
                        talkIdConfigData.NPCName = personInfo.PersonNameIDS;
                        talkIdConfigData.TalkHead = personInfo.SrotyPersonHeadPrefab;
                        talkIdConfigData.TalkID = 0;
                        talkIdConfigData.TalkText = personInfo.DialogIDS;
                        talkIdConfigData.TalkType = StoryTallType.NPC;
						talkIdConfigData.isTaskTalkMark = false;
                        talkIdConfigData.TextPos = "0";

                        taskNpcTalkPanelBehaviour.Show(storyPanel, talkIdConfigData);
                        if (personInfo.DialogVoiceIDS != "0")
                        {
                            SoundManager.Instance.PlaySoundEffect(personInfo.DialogVoiceIDS);
                        }
                        m_showingNpcTalkPanel = taskNpcTalkPanelBehaviour.gameObject;
						//send event to joystick of TownUI
						UIEventManager.Instance.TriggerUIEvent(UIEventType.OnNpcTalkOpenEvent, null);
                    }
                };
            string npcName = storyPersonInfo.PersonNameIDS;//
            string szNpcTitle = "";
            var npcTitleGo = GameObjectPool.Instance.AcquireLocal(NpcTitle, Vector3.zero, Quaternion.identity);
            npcTitleGo.transform.parent = BattleManager.Instance.UICamera.transform;
            npcTitleGo.transform.localScale = new Vector3(20, 20, 20);
            npcTitleGo.GetComponent<NPCTitle>().SetNpcTitle(npcName, szNpcTitle, npc.transform.FindChild("NPCTitle").position);
            m_NPCTitles.Add(npcTitleGo);
        }
        return npc;
    }
    /// <summary>
    /// 引导按钮点击监听处理
    /// </summary>
    /// <param name="btnIdObj"></param>
    private void GuideFinishHandle(object btnIdObj)
    {
        //int btnId = (int)btnIdObj;
        TaskGuideFinish();        
    }
    /// <summary>
    /// 引导完成理，可以是任务内引导，也可以是任务完成后引导
    /// </summary>
    private void TaskGuideFinish()
    {

        bool isFinishGuide = TaskModel.Instance.IsTaskFinished;
        TaskBtnManager.Instance.GuideFinish();
        TownGuideManager.Instance.GuideFinish(m_newGuideConfigData,isFinishGuide);
        if (isFinishGuide)
        {
            TaskFinishGuideInvokeHandle(null);
        }
        else
        {
            ExecuteTaskGuide();
        }
    }
    
    /// <summary>
    /// 弱引导其他按钮点击监听处理
    /// </summary>
    /// <param name="btnIdObj"></param>
    private void ClickOtherButtonHandle(object btnIdObj)
    {
        if (m_newGuideConfigData != null)
        {
			if(TaskModel.Instance.isNpcTalking)
				return;
			//jamfing20141204//
            //如果正在进行剧情对话，取消对话恢复镜头
			/*
            if (m_skipStoryBtn != null)
            {
                m_skipStoryBtn.OnClick(true);
            }*/
            PlayerManager.Instance.HeroAgent.enabled = false;
            TaskBtnManager.Instance.BreakGuide();
            TaskModel.Instance.BreakTask(m_newGuideConfigData.NewGuideConfigDatas.TaskNewConfigData.TaskID);
            m_newGuideConfigData.GuideBtnUIReadyAct = null;
            m_newGuideConfigData = null;
//            m_taskPanel.ShowOrHideTips();
			m_taskGuidePanel.ShowPanel();
        }

    }
    //任务完成，请空当前任务引导列表
    private void TaskFinished(INotifyArgs args)
    {
        var taskConfigData = (TaskState)args;
        m_taskFinishAppearButton = taskConfigData.TaskNewConfigData.AppearButton;
        TaskAward(taskConfigData);       
    }
    /// <summary>
    /// 刷新任务快速引导的 新功能开启特效和任务提示文本
    /// </summary>
    /// <param name="args"></param>
    private void RefreshTaskQuickInfo(INotifyArgs args)
    {
//        m_taskPanel.InitTaskPanel(TaskModel.Instance.FindNextQuickGuideTask());
//        m_taskPanel.ShowOrHideTips();
		m_taskGuidePanel.ShowPanel();
    }
    /// <summary>
    /// 没有引导时 点击处理
    /// </summary>
    /// <param name="args"></param>
    private void ClickNoneGuideBtnHandle(INotifyArgs args)
    {
		if (m_newGuideConfigData != null)
        {
            TaskFinishGuideInvokeHandle(null);
        }        
    }
    /// <summary>
    /// 按钮终止规则
    /// </summary>
    private bool OverRuleCheck()
    {
        bool flag = false;
        
        return flag;
    }

    /// <summary>
    /// 按钮跳过规则
    /// </summary>
    private bool SkipRuleCheck()
    {
        bool flag = false;
        if (m_newGuideConfigData != null)
        {
            var playerModel = PlayerManager.Instance.FindHeroDataModel();

            ///如果在当前按钮管理列表中无当前引导按钮ID，则判断当前按钮是否可以跳过，否则就会报错
            switch (m_newGuideConfigData.NewGuideConfigDatas.SkipRole)
            {
                case 1: //铜币小于背包中第一个装备强化需求的铜币。或者背包中第一个不是武器装备。
                    if (SirenManager.Instance.IsOwnSiren(1))
                    {
                        TraceUtil.Log(SystemModel.NewbieGuide,"跳过规则1：ID为1的妖女已经收服。");
                        flag = true;
                    }
                    break;
			case 2: //当玩家在装备栏内查找不到箭头要查找的装备ID时。跳过这行箭头。装备ID读取的是Equipment 表中的lGoodsID字段
					{
						if(!ContainerInfomanager.Instance.IsItemEquipped(m_newGuideConfigData.MappingId))
						{
								TraceUtil.Log(SystemModel.NewbieGuide,"跳过规则2：ID装备栏中找不到。");
							flag = true;
						}
					}
					break;
			case 3: //当玩家在装备栏内查找到箭头要查找的装备ID时。跳过这行箭头。装备ID读取的是Equipment 表中的lGoodsID字段
					{
						if(ContainerInfomanager.Instance.IsItemEquipped(m_newGuideConfigData.MappingId))
						{
							TraceUtil.Log(SystemModel.NewbieGuide,"跳过规则2：ID装备栏中找不到。");
							flag = true;
						}
					}
					break;
			default:
                    break;
            }
        }
        return flag;
    }       
    void OnDestroy()
    {
        TaskModel.Instance.AcceptTaskAct = null;
        RemoveEventHandler(EventTypeEnum.ClickNoneGuideBtn.ToString(),GuideFinishHandle);// ClickNoneGuideBtnHandle);
        RemoveEventHandler(EventTypeEnum.TaskExecuteInvoke.ToString(), TaskExecuteInvokeHandle);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ClickTheGuideBtn, GuideFinishHandle);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ClickOtherButton, ClickOtherButtonHandle);
        RemoveEventHandler(EventTypeEnum.TaskFinishGuideExecuteInvoke.ToString(), TaskFinishGuideInvokeHandle);
        RemoveEventHandler(EventTypeEnum.TaskFinish.ToString(), TaskFinished);
        RemoveEventHandler(EventTypeEnum.QuickTaskGuideRefresh.ToString(), RefreshTaskQuickInfo);
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.ClickNoneGuideBtn.ToString(), GuideFinishHandle);//ClickNoneGuideBtnHandle);
        AddEventHandler(EventTypeEnum.TaskFinishGuideExecuteInvoke.ToString(), TaskFinishGuideInvokeHandle);
        AddEventHandler(EventTypeEnum.TaskExecuteInvoke.ToString(), TaskExecuteInvokeHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.ClickTheGuideBtn, GuideFinishHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.ClickOtherButton, ClickOtherButtonHandle);
        AddEventHandler(EventTypeEnum.TaskFinish.ToString(), TaskFinished);
        AddEventHandler(EventTypeEnum.QuickTaskGuideRefresh.ToString(), RefreshTaskQuickInfo);        
    }
}
