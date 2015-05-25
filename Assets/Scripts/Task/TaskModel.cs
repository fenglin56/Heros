using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// 任务模型，处理任务的初始化，接受，更新，完成等处理
/// 在Service接收到任务相关协议后，调用TaskModel的方法处理，完成后抛出事件
/// 接收任务中断消息，把当前任务置空
/// 当有任务触发时，抛出消息触发消息。监听者从Model获得触发的任务数据
/// 在自动触发任务时，有可能场景还没装备完毕，不能马上进行任务处理。等场景加载完成后检查触发
/// </summary>
using UI.MainUI;


public class TaskModel : Model, ISingletonLifeCycle
{
    public Action AcceptTaskAct;
    /// <summary>
    /// 当前要执行的任务
    /// </summary>
    public int? CurrentTaskId { get; private set; }
    /// <summary>
    /// 下一个要执行的任务（当任务要触发，但上一个任务有finish引导处理时，
    /// 把要执行的任务先缓存，等上一任务的finish引导完成后，调用ManualTriggerCacheTask触发）
    /// </summary>
    private TaskState m_cacheNextExecuteTaskState;
    private bool m_lockTaskAutoTrigger=true;   //第一次登录时，需要锁住服务器下发的[弱引导/自动触发]的任务的启动，（在UI启动完成后再触发）
    private static TaskModel m_instance;
    public static TaskModel Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new TaskModel();
                //SingletonManager.Instance.Add(m_instance);
				m_instance.RegisterEvent();

            }
            return m_instance;
        }
    }
	public bool isNpcTalking = false;//对话正在进行//
	public bool isNewFunctionEffing = false;//开启新功能特效进行中//
	private TaskModel()
    {
        m_taskNewConfigDataBase = GuideConfigManager.Instance.TaskNewConfigFile;
        m_quickGuideTask = new QuickGuideTaskList();
        m_sortedTask = new List<TaskState>();
        InitDefaultTask();
        //初始化任务引导管理器
        TownGuideManager.Instance.Init();        
    }
    public void MakeDemoData()
    {
        STaskState[] sTaskStates = new STaskState[16];
        sTaskStates[0] = new STaskState() { dwTaskID = 115, byRate = 1, byAllRate = 3 };
        sTaskStates[1] = new STaskState() { dwTaskID = 100, byRate = 1, byAllRate = 3 };
        sTaskStates[2] = new STaskState() { dwTaskID = 101, byRate = 1, byAllRate = 3 };
        sTaskStates[3] = new STaskState() { dwTaskID = 102, byRate = 1, byAllRate = 3 };
        sTaskStates[4] = new STaskState() { dwTaskID = 103, byRate = 1, byAllRate = 3 };
        sTaskStates[5] = new STaskState() { dwTaskID = 104, byRate = 1, byAllRate = 3 };
        sTaskStates[6] = new STaskState() { dwTaskID = 105, byRate = 1, byAllRate = 3 };
        sTaskStates[7] = new STaskState() { dwTaskID = 106, byRate = 1, byAllRate = 3 };
        sTaskStates[8] = new STaskState() { dwTaskID = 107, byRate = 1, byAllRate = 3 };
        sTaskStates[9] = new STaskState() { dwTaskID = 108, byRate = 1, byAllRate = 3 };
        sTaskStates[10] = new STaskState() { dwTaskID = 109, byRate = 1, byAllRate = 3 };
        sTaskStates[11] = new STaskState() { dwTaskID = 110, byRate = 1, byAllRate = 3 };
        sTaskStates[12] = new STaskState() { dwTaskID = 111, byRate = 1, byAllRate = 3 };
        sTaskStates[13] = new STaskState() { dwTaskID = 112, byRate = 1, byAllRate = 3 };
        sTaskStates[14] = new STaskState() { dwTaskID = 113, byRate = 1, byAllRate = 3 };
        sTaskStates[15] = new STaskState() { dwTaskID = 114, byRate = 1, byAllRate = 3 };
        InitTask(sTaskStates);
    }
    
    private List<TaskState> m_sortedTask ;
    private QuickGuideTaskList m_quickGuideTask;
    private TaskNewConfigDataBase m_taskNewConfigDataBase;
    private void InitDefaultTask()
    {
        int taskId=CommonDefineManager.Instance.CommonDefine.DefaultStroy;
        STaskState sTaskState=new STaskState();
        sTaskState.dwTaskID=taskId;
        DefaultNPCTask = new TaskState(sTaskState, m_taskNewConfigDataBase.GetTaskNewConfigData(taskId));
    }
    /// <summary>
    /// 任务初始化
    /// 根据任务类型 主线→支线→日常→循环 排序，同样类型给距任务ID从小到大排序
    /// </summary>
    /// <param name="sTaskStates"></param>
    public void InitTask(STaskState[] sTaskStates)
    {
        m_sortedTask.Clear();
        sTaskStates.ApplyAllItem(P =>
        {
            TaskState taskState = new TaskState(P,m_taskNewConfigDataBase.GetTaskNewConfigData(P.dwTaskID));
            m_sortedTask.Add(taskState);
			//jamfing 20141124//
            //AutoTriggerTask(taskState);
        });
        RefreshQuickTaskGuide();
        RefreshTask();

        if (AcceptTaskAct != null)
        {
            AcceptTaskAct();
        }
    }
	void OnSceneLoadedEvent(object obj)
	{
		if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN) {
			GameManager.Instance.DelayCall(0.2f,()=>{
				m_sortedTask.ApplyAllItem (P =>{
					AutoTriggerTask (P);
				});
			});
		}
	}
    public TaskState DefaultNPCTask
    {
        get;
        private set;
    }
    /// <summary>
    /// 获得当前主线任务
    /// </summary>
    public TaskState MainTaskState
    {
        get
        {
            return m_sortedTask.FirstOrDefault(P => P.TaskNewConfigData.TaskSeries == 2);
        }
    }
    public GameObject GetTaskOpenNewFunctionIcon()
    {
		return new GameObject();
        /*foreach (var task in m_sortedTask)
        {
            if (task.TaskNewConfigData.FunctionIcon != null)
            {
                return task.TaskNewConfigData.FunctionIcon;
            }
        }
        return null;*/
    }
	public string GetTaskNewFunction()
	{
		foreach (var task in m_sortedTask)
		{
			if (!task.TaskNewConfigData.FunctionName.Equals("0"))
			{
				return task.TaskNewConfigData.FunctionName;
			}
		}
		return "0";
	}
    public bool IsTaskFinished
    {
        get 
        {
            bool finishFlag = false;
            if (CurrentTaskId.HasValue)
            {
                var runningTask=FindRuningTaskState(CurrentTaskId.Value);
                if (runningTask == null)
                {
                    finishFlag = true;
                }
                else
                {
                    finishFlag = runningTask.Finished;
                }
            }
            else
            {
                finishFlag = true;
            }
            return finishFlag; 
        }
    }
    /// <summary>
    /// 接收任务
    /// </summary>
    /// <param name="sTaskState"></param>
    public void AcceptTask(STaskState sTaskState)
    {
		int tempI = -1;
		for (int i = 0; i < m_sortedTask.Count; i++) {
			if(m_sortedTask[i].dwTaskID == sTaskState.dwTaskID)
			{
				tempI = i;
			}
		}
		if(tempI != -1)
			m_sortedTask.RemoveAt (tempI);
        TaskState taskState = new TaskState(sTaskState, m_taskNewConfigDataBase.GetTaskNewConfigData(sTaskState.dwTaskID));
        m_sortedTask.Add(taskState);
        if (m_taskNewConfigDataBase.GetTaskNewConfigData(sTaskState.dwTaskID).GuideGroup != "0") //当引导组不为0时，表示需要加入到快速引导列表
        {
            RefreshQuickTaskGuide();
        }
        RefreshTask();
		if (!m_lockTaskAutoTrigger) {
			//第一次下发的不处理//
			AutoTriggerTask (taskState);
		} else {
			m_lockTaskAutoTrigger = false;  //解锁 任务触发后,自动或手动
		}
        if (AcceptTaskAct != null)
        {
            AcceptTaskAct();
        }
    }
	//手动点击任务引导//
	public void ManualTriggerTask(TaskState taskState,bool isFromNewBie)
	{
		if (taskState != null)
		{
			DealTriggerTask(taskState.dwTaskID,isFromNewBie);
		}
	}
    /// <summary>
    /// 手动触发任务
    /// </summary>
    public void ManualTriggerTask(TaskState taskState)
    {
        if (taskState != null)
        {
			DealTriggerTask(taskState.dwTaskID,false);
        }
    }
    public void ManualTriggerCacheTask()
    {
        ManualTriggerTask(m_cacheNextExecuteTaskState);
        m_cacheNextExecuteTaskState = null;
    }
    /// <summary>
    /// 获得缓存的下一步要执行的任务
    /// </summary>
    public TaskState CacheNextExecuteTaskState
    {
        get
        {
            return m_cacheNextExecuteTaskState;
        }
    }
    /// <summary>
    /// 获得下一个快速引导任务
    /// </summary>
    public TaskState FindNextQuickGuideTask()
    {
         return m_quickGuideTask.GetNextGuideTask();
    }

    public TaskState CacheFinishTastState;
    /// <summary>
    /// 放弃任务
    /// </summary>
    /// <param name="giveUpTaskId"></param>
    public void GiveUpTask(int giveUpTaskId)
    {
        TraceUtil.Log(SystemModel.Rocky, "任务放弃：" + giveUpTaskId);

        m_sortedTask.RemoveAll(P => P.dwTaskID == giveUpTaskId);
        RefreshTask();
        RefreshQuickTaskGuide();
    }
    /// <summary>
    /// 任务完成,如果有完成引导，进行引导
    /// </summary>
    /// <param name="finishTaskId"></param>
    public void FinishTask(int finishTaskId)
    {
        var finishTastState = FindRuningTaskState(finishTaskId);
        finishTastState.Finished = true;
        TraceUtil.Log(SystemModel.Rocky, "任务完成：" + CurrentTaskId);
        if (GameManager.Instance.CurrentState != GameManager.GameState.GAME_STATE_TOWN)
        {
            CacheFinishTastState = finishTastState;
        }
        RaiseEvent(EventTypeEnum.TaskFinish.ToString(), finishTastState);
		Debug.Log ("FinishTask=====task-=="+finishTaskId);
		//jamfing//
		/*
        m_sortedTask.RemoveAll(P => P.dwTaskID == finishTaskId);
		if (CurrentTaskId.HasValue && CurrentTaskId.Value == finishTaskId)
        {
            CurrentTaskId = null;*/
		//当完成的为非主线任务时，把数据重新生成一遍//
		if (finishTastState.TaskNewConfigData.TaskSeries != 2) {
			TownGuideManager.Instance.AgainGetTaskData (finishTastState);
		}
		if (!CurrentTaskId.HasValue) {
			TownGuideManager.Instance.AgainGetTaskData(finishTastState);
		}
		m_sortedTask.RemoveAll(P => P.dwTaskID == finishTaskId);
		CurrentTaskId = null;
		if (TownGuideManager.Instance.NeedFinishGuide())
		{
			//如果有任务完成后续引导，并且当前不在城镇中，则需要缓存持续引导，否则发后续引导消息
			if (GameManager.Instance.CurrentState != GameManager.GameState.GAME_STATE_TOWN)
			{
				TownGuideManager.Instance.CacheFinishGuide();
			}
			else
			{
				RaiseEvent(EventTypeEnum.TaskFinishGuideExecuteInvoke.ToString(), null);
			}
		}
		else
		{
			TaskBtnManager.Instance.ResetAllButtonStatus(true);
		}
        RefreshTask();
        RefreshQuickTaskGuide();
       
    }
   
    /// <summary>
    /// 任务完成，请空当前任务引导列表
    /// </summary>
    /// <param name="breakTaskId"></param>
    public void BreakTask(int breakTaskId)
    {
        m_quickGuideTask.TaskBeBreak(breakTaskId);
        if (CurrentTaskId.HasValue && CurrentTaskId.Value == breakTaskId)
        {
            TraceUtil.Log(SystemModel.Rocky, "任务被打断：" + CurrentTaskId);
            RaiseEvent(EventTypeEnum.TaskBreak.ToString(), null);

            CurrentTaskId = null;
        }
        else
        {
            TraceUtil.Log(SystemModel.Rocky, TraceLevel.Error, "要打断的任务不是当前执行任务");
        }
    }   
    /// <summary>
    /// 任务更新
    /// </summary>
    /// <param name="sTaskState"></param>
    public void UpdateTask(STaskState sTaskState)
    {
        var taskState = m_sortedTask.SingleOrDefault(P => P.dwTaskID == sTaskState.dwTaskID);
        taskState.Update(sTaskState);
    }
   /// <summary>
    /// 获得正在执行的任务，可能为null
    /// </summary>
    /// <returns></returns>
    public TaskState FindRuningTaskState()
    {
        if (CurrentTaskId.HasValue)
        {
            var currentTask=m_sortedTask.SingleOrDefault(P => P.dwTaskID == CurrentTaskId.Value);
            if (currentTask != null)
            {
                m_cacheNextExecuteTaskState = null;  //如果当前有任务，则清除缓存任务
            }
            return currentTask;
        }
        else
        {
            return null;
        }
    }
	public TaskState FindRuningTaskState(int taskId)
	{
		return m_sortedTask.SingleOrDefault(P=>P.dwTaskID==taskId);
	}
    /// <summary>
    /// 当前任务的引导类型（强/弱/无引导）
    /// </summary>
    public TaskGuideType TaskGuideType
    {
        get
        {
            var task = FindRuningTaskState();
            if (task != null)
            {
                return task.TaskNewConfigData.GuideType;
            }
            else if(m_cacheNextExecuteTaskState!=null)  //从副本回来城镇时，服务器下发的任务会缓存。这时要检查缓存任务的强弱类型
            {
                return m_cacheNextExecuteTaskState.TaskNewConfigData.GuideType;
            }
            return TaskGuideType.None;
        }

    }
    public bool ResponseClickEvent
    {
        get
        {
            bool flag = true;
            if (TaskGuideType == TaskGuideType.Enforce)
            {
                if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
                {
                    flag = false;
                }
            }
            return flag;
        }
    }
    /// <summary>
    /// 获得任务显示列表
    /// </summary>
    /// <returns></returns>
    public List<TaskState> TaskDisplayList
    {
        get
        {
            return m_sortedTask;
        }
    }
    public void TriggerMainTask()
    {
        var mainTask = TaskModel.Instance.MainTaskState;
        if (mainTask != null)
        {
            AutoTriggerTask(mainTask);
        }
    }
    /// <summary>
    ///  更新排序任务
    /// </summary>
    private void RefreshTask()
    {
        m_sortedTask.Sort((a, b) =>
        {
            int result=0;
            if (a.TaskNewConfigData.TaskSeries == b.TaskNewConfigData.TaskSeries)
            {
                if (a.dwTaskID > b.dwTaskID)
                {
                    result = 1;
                }
                else if (a.dwTaskID < b.dwTaskID)
                {
                    result = -1;
                }
            }
            else
            {
                result = a.TaskNewConfigData.TaskSeries > b.TaskNewConfigData.TaskSeries ? 1 : -1;
            }
            return result;
        }); 
        RaiseEvent(EventTypeEnum.TaskStateRefresh.ToString(), null);
    }
    /// <summary>
    /// 更新快速引导任务
    /// </summary>
    private void RefreshQuickTaskGuide()
    {
        var withArrowGuides = m_sortedTask.Where(P => m_taskNewConfigDataBase.GetTaskNewConfigData(P.dwTaskID).GuideGroup != "0");
        m_quickGuideTask.AddQuickGuideTasks(withArrowGuides);

        RaiseEvent(EventTypeEnum.QuickTaskGuideRefresh.ToString(), null);
    }
    /// <summary>
    /// 检查自动触发任务
    /// </summary>
    /// <param name="taskState"></param>
    private void AutoTriggerTask(TaskState taskState)
    {
        if (!GameManager.Instance.IsNewbieGuide)
        {
            return;
        }
        var task = m_taskNewConfigDataBase.GetTaskNewConfigData(taskState.dwTaskID);
        if (task != null && task.GuideStar==TaskStartType.Auto)  //自动触发
        {
            var eTeleportType = (eTeleportType)GameManager.Instance.GetNewWorldMsg.byTeleportFlg;
            //if (task.GuideType == TaskGuideType.Enforce || !m_lockTaskAutoTrigger||eTeleportType != eTeleportType.TELEPORTTYPE_FIRST)
            {                
				TaskTrigger(taskState);
            }
        }
    }
	void DealTriggerTask(int taskID,bool isManualTriggerMark)
	{
		if (isNewFunctionEffing) {
			GameManager.Instance.DelayCall (0.5f, () => {
				DealTriggerTask(taskID,isManualTriggerMark);
			});
		} else {
			TaskTrigger(taskID,isManualTriggerMark);
		}
	}
	private void TaskTrigger(int taskId,bool isManualTriggerMark)
    {
        if (m_lockTaskAutoTrigger)
        {
            m_lockTaskAutoTrigger = false;  //解锁 任务触发后,自动或手动
        }
//		if (isManualTriggerMark && this.CurrentTaskId != null && this.CurrentTaskId.Value == taskId) {
//			return;
//		}
        this.CurrentTaskId = taskId;
        RaiseEvent(EventTypeEnum.TaskTrigger.ToString(), null);
    }
    private void TaskTrigger(TaskState taskState)
    {
        if (taskState != null)
        {
            //如果上一个任务完成后有finish引导的话，等finish引导完成再触发
            if (TownGuideManager.Instance.NeedFinishGuide())
            {
				Debug.Log("TaskTrigger===NeedFinishGuide==have cache task="+taskState.dwTaskID);
                m_cacheNextExecuteTaskState = taskState;
                return;
            }
			Debug.Log("TaskTrigger=====current task="+taskState.dwTaskID);
			DealTriggerTask(taskState.dwTaskID,false);
        }
    }
    protected override void RegisterEventHandler()
    {

    }
	void RegisterEvent()
	{
		UIEventManager.Instance.RegisterUIEvent(UIEventType.LoadingComplete, OnSceneLoadedEvent);
	}
    public void Instantiate()
    {
    }

    public void LifeOver()
    {
        m_instance = null;
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.LoadingComplete, OnSceneLoadedEvent);
    }
	/// <summary>
	/// 获取要显示的非主线任务数据
	/// </summary>
	/// <param name="sTaskStates"></param>
	public TaskState GetShowUnMainTask(int isNotThisTaskID)
	{
		return m_sortedTask.FirstOrDefault (p=>p.TaskNewConfigData.TaskSeries == 3 && p.TaskNewConfigData.ShowLimit == 1 && p.TaskNewConfigData.TaskID != isNotThisTaskID);
	}
	public TaskState GetShowUnMainTask()
	{
		return m_sortedTask.FirstOrDefault (p=>p.TaskNewConfigData.TaskSeries == 3 && p.TaskNewConfigData.ShowLimit == 1);
	}
	#region 任务跳转到任意界面
	public bool IsOpenSysFun(UI.MainUI.UIType btnType)
	{
		bool Contain=false;
		UIType[] uiType =NewUIDataManager.Instance.InitMainButtonList.Single(P => P.ButtonProgress == GameManager.Instance.MainButtonIndex).MainButtonList;
		
        if (uiType.LocalContains(btnType))
		{
			Contain=true;
		}
		return Contain;
	}
   
	public void JumpView(LinkConfigItemData itemData)
	{
		switch(itemData.LinkType)
		{
		case LinkType.Battle:
			if(EctypeModel.Instance.IsOpenEctype(int.Parse( itemData.LinkPara)))
			{
				EctypeModel.Instance.OpenPointToEctypePanel(int.Parse( itemData.LinkPara));
			}
			else
			{
				return;
			}
			break;
		case LinkType.NoneLink:
			return;
		case LinkType.SystemFun:
			UI.MainUI.UIType type=(UI.MainUI.UIType)System.Convert.ToInt32( itemData.LinkPara);
			if(IsOpenSysFun(type))
			{
				//MainUIController.Instance.OpenMainUI(type);
				JumpTownView(type,itemData.LinkChildren);
			}
			else
			{
				return;
			}
			break;
		case LinkType.CrusadeBattle:
			if(EctypeManager.Instance.IsCrusadeEctypeUnlock(int.Parse( itemData.LinkPara)))
			{
				UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Crusade,int.Parse(itemData.LinkPara));
			}
			else
			{
				return;
			}
			break;
		case LinkType.DefenseBattle:
			if(DefenceEntryManager.DefenceEctypeEnabled(int.Parse( itemData.LinkPara)))
			{
				UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Defence,int.Parse(itemData.LinkPara));
			}
			else
			{
				return;
			}
			break;
		}
	}

	public void JumpTownView(UI.MainUI.UIType viewType,int linkChildView)
	{
		switch (viewType) {
		case UI.MainUI.UIType.Gem:
		{
			if(linkChildView == 1)
			{
				UI.MainUI.MainUIController.Instance.OpenMainUI(viewType);
			}
			else if(linkChildView == 2)
			{
				UI.MainUI.MainUIController.Instance.OpenMainUI(viewType,JewelState.JewelUpgrad);
			}
		}
		break;
		case UI.MainUI.UIType.Activity:
		case UI.MainUI.UIType.Siren:
		{
			UI.MainUI.MainUIController.Instance.OpenMainUI(viewType,linkChildView);
		}
			break;
		case UI.MainUI.UIType.EquipmentUpgrade:
		{
			if(linkChildView == 1)
			{
				UI.MainUI.MainUIController.Instance.OpenMainUI(viewType,UpgradeType.Strength);
			}
			else if(linkChildView == 2)
			{
				UI.MainUI.MainUIController.Instance.OpenMainUI(viewType,UpgradeType.StarUp);
			}
			else
			{
				UI.MainUI.MainUIController.Instance.OpenMainUI(viewType,UpgradeType.Upgrade);
			}
		}
			break;
		case UI.MainUI.UIType.Forging:
		{
			if(linkChildView == 0)
			{
				UI.MainUI.MainUIController.Instance.OpenMainUI(viewType);
			}
			else
			{
				UI.MainUI.MainUIController.Instance.OpenMainUI(viewType,linkChildView);
			}
		}
			break;
		default:
			UI.MainUI.MainUIController.Instance.OpenMainUI(viewType);
			break;
		}
	}
	#endregion
	//发送消息给服务器:是否有其他的附加数据 0:否
	public void SendTaskMessageToServer(int npcID,int taskType,int taskParam1,int taskParam2)
	{
		SMsgInteractCOMMON_CS msgInteract;
		msgInteract.dwNPCID = (uint)npcID;
		msgInteract.byOperateType = (byte)taskType;
		msgInteract.dwParam1 = (uint)taskParam1;
		msgInteract.dwParam2 = (uint)taskParam2;
		msgInteract.byIsContext = 0;
		NetServiceManager.Instance.InteractService.SendInteractCOMMON(msgInteract);
	}
}
public class TaskState:INotifyArgs
{
    public TaskState(STaskState sTaskState, TaskNewConfigData taskNewConfigData)
    {
        Finished = false;
        this.Update(sTaskState);
        TaskNewConfigData = taskNewConfigData;
    }
    public void Update(STaskState sTaskState)
    {
        this.byAllRate = sTaskState.byAllRate;
        this.byRate = sTaskState.byRate;
        this.dwTaskID = sTaskState.dwTaskID;
    }
    public int dwTaskID;					//任务ID
    public byte byRate;						//当前进度
    public byte byAllRate;					//总的进度

    public bool Finished;

    public TaskNewConfigData TaskNewConfigData
    {
        get;
        private set;
    }

    public int GetEventArgsType()
    {
        return 0;
    }
}

public class QuickGuideTaskList : List<TaskState>
{
    private int m_runningTaskIndex=0;
    private TaskState m_currentTaskState;
    public void AddQuickGuideTasks(IEnumerable<TaskState> taskStates)
    {
        this.Clear();
        foreach (var item in taskStates)
        {
            this.Add(item);
        }
        this.Sort((a, b) =>
        {
            int result = 0;
            if (a.TaskNewConfigData.TaskSeries == b.TaskNewConfigData.TaskSeries)
            {
                if (a.dwTaskID > b.dwTaskID)
                {
                    result = 1;
                }
                else if (a.dwTaskID < b.dwTaskID)
                {
                    result = -1;
                }
            }
            else
            {
                result = a.TaskNewConfigData.TaskSeries > b.TaskNewConfigData.TaskSeries ? 1 : -1;
            }
            return result;
        });
        //有更新，从头开始执行任务
        m_runningTaskIndex = 0;
    }
    /// <summary>
    /// 获得下一个要执行的快速引导任务
    /// </summary>
    /// <returns></returns>
    public TaskState GetNextGuideTask()
    {
        m_currentTaskState = null;
        if (m_runningTaskIndex < this.Count)
        {
            m_currentTaskState = this[m_runningTaskIndex++];
            if (m_runningTaskIndex == this.Count)
            {
                m_runningTaskIndex = 0;
            }
        }
        return m_currentTaskState;
    }
    public TaskState GetCurrentGuideTask()
    {
        return m_currentTaskState;
    }
    /// <summary>
    /// 任务被打断，自动到下一个任务
    /// </summary>
    public void TaskBeBreak(int taskId)
    {
        if (this.Exists(P => P.dwTaskID == taskId))
        {
            if (m_runningTaskIndex < this.Count)
            {
                m_runningTaskIndex++;
                if (m_runningTaskIndex == this.Count)
                {
                    m_runningTaskIndex = 0;
                }
            }
        }
    }
}

[Serializable]
public class StoryPersonInfo
{
    public Vector3 PersonPos = Vector3.zero;
    public float StartAngle;
    public GameObject SrotyPersonPrefab = null;
    public int StoryBoxStyle;
    public GameObject SrotyPersonHeadPrefab = null;
    public string PersonNameIDS = string.Empty;
    public string DialogIDS = string.Empty;
    public string DialogVoiceIDS = string.Empty;
    /// <summary>
    /// 由数据导入方法调用
    /// </summary>
    /// <param name="k"></param>
    /// <param name="value"></param>
    public void InitData(int k, object value)
    {
        switch (k)
        {
            case 0:
                PersonPos.x = float.Parse(value.ToString())*0.1f;
                break;
            case 1:
                PersonPos.z = float.Parse(value.ToString()) * -0.1f;
                break;
            case 2:
                PersonPos.y = float.Parse(value.ToString()) * 0.1f;
                break;
            case 3:
                StartAngle = float.Parse(value.ToString());
                break;
            case 4:
                SrotyPersonPrefab = value as GameObject;
                break;
            case 5:
                StoryBoxStyle = int.Parse(value.ToString());
                break;
            case 6:
                SrotyPersonHeadPrefab = value as GameObject;
                break;
            case 7:
                PersonNameIDS = value.ToString();
                break;
            case 8:
                DialogIDS = value.ToString();
                break;
            case 9:
                DialogVoiceIDS = value.ToString();
                break;
        }
    }
}
[Serializable]
public class AwardItemInfo
{
    public int Vocation;
    public int ItemId;
    public int Amount;

    public AwardItemInfo(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            var datas = data.Split('+');
            Vocation = int.Parse(datas[0]);
            ItemId = int.Parse(datas[1]);
            Amount = int.Parse(datas[2]);
        }
    }
}
//public class AB
//{
//    public void Trigger()
//    {
//        MakeStoryPerson();
//        if(TaskHasGuideGroup)
//        {
//            if (Time > DelayTime)
//            {
//                StartGuide();
//            }
//        }
//        else
//        {
//            //
//        }
//    }
//    public void StartGuide()
//    {
//        int guideType = FindGuideType();
//        case guideType;

//        NormalArrow();
//        AIFindBtn();
//        NavGuide();
//        OpenSpecUI();
//        TrgiggerTownStory();
//    }
//    public void NormalArrow()
//    {
//        FindBtn();
//        SetBtnSign();
//    }

//    private void SetBtnSign()
//    {

//    }
//}
