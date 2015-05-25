using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 城镇引导管理器
/// 保存当前触发任务的引导组
/// 接收任务中断消息，把当前引导组清空
/// </summary>
public class TownGuideManager:Controller,ISingletonLifeCycle
{    
    private NewGuideConfigDataBase m_newGuideConfigDataBase;

    private List<TaskGuideExtendData> m_currentTaskGuides = new List<TaskGuideExtendData>();
    private List<TaskGuideExtendData> m_finishTaskGuides = new List<TaskGuideExtendData>();
    private List<TaskGuideExtendData> m_CachfinishTaskGuides = new List<TaskGuideExtendData>();
    private static TownGuideManager m_instance;
    public int m_guideIndex = 0;  //引导索引
    private int m_fishGuideIndex = 0;  //后续引导索引
    public static TownGuideManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new TownGuideManager();
                //SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }
    public void Init()
    {
        if (m_newGuideConfigDataBase == null)
        {
            m_newGuideConfigDataBase = GuideConfigManager.Instance.NewGuideConfigFile;
            m_newGuideConfigDataBase.Datas.ApplyAllItem(P =>
                {
                    P.TalkIdConfigDatas = GuideConfigManager.Instance.TalkIdConfig.GetTalkIdConfigDataByGroup(P.GetTalkIDIds());
                });
        }
    }
    //触发一个任务，获得该任务的引导
    private void TaskTriggerHandler(INotifyArgs args)
    {
        //Get CurrentTrgigger Task
		if (UI.SystemFuntionButton.Instance != null)
		{
			UI.SystemFuntionButton.Instance.CloseBtnpanel();
		}
        var taskConfigData = TaskModel.Instance.FindRuningTaskState();
        
        if (taskConfigData != null)
        {
            TraceUtil.Log(SystemModel.Rocky, "触发任务Id：" + taskConfigData.dwTaskID);
            //Search task's guide group to execute.
            ClearTaskGuides(true);
            var groudIds=taskConfigData.TaskNewConfigData.GetStartGuideGroupIds();
            if (groudIds!=null&&groudIds.Length > 0)
            {
                var runningGuides = m_newGuideConfigDataBase.GetNewGuideConfigDataByGroup(groudIds);
                m_currentTaskGuides.AddRange(runningGuides);
                //重置所有引导完成标记为False，未完成
                m_currentTaskGuides.ApplyAllItem(P => { P.FinishFlag = false; P.IsGuideUIReady = false; P.NewGuideConfigDatas.TaskNewConfigData = taskConfigData.TaskNewConfigData; });
            }
            var finishGroudIds = taskConfigData.TaskNewConfigData.GetCompleteGuideGroupIds();
            if (finishGroudIds != null && finishGroudIds.Count() > 0)
            {
                var finishGuides = m_newGuideConfigDataBase.GetNewGuideConfigDataByGroup(finishGroudIds);
                if (finishGuides != null)
                {
                    m_finishTaskGuides.AddRange(finishGuides);
                    //重置所有引导完成标记为False，未完成
                    m_finishTaskGuides.ApplyAllItem(P => {P.FinishFlag = false;P.IsGuideUIReady = false; P.NewGuideConfigDatas.TaskNewConfigData = taskConfigData.TaskNewConfigData; });
                }
            }
            //任务开始引导(TODO)
            m_guideIndex = 0;
            m_fishGuideIndex = 0;
            RaiseEvent(EventTypeEnum.TaskExecuteInvoke.ToString(), null);
        }
    }
	//当任务完成时，发现没有当前任务，数据重新灌//
	public void AgainGetTaskData(TaskState taskConfigData)
	{
		ClearTaskGuides(true);
		var groudIds=taskConfigData.TaskNewConfigData.GetStartGuideGroupIds();
		if (groudIds!=null&&groudIds.Length > 0)
		{
			var runningGuides = m_newGuideConfigDataBase.GetNewGuideConfigDataByGroup(groudIds);
			m_currentTaskGuides.AddRange(runningGuides);
			//重置所有引导完成标记为False，未完成
			m_currentTaskGuides.ApplyAllItem(P => { P.FinishFlag = false; P.IsGuideUIReady = false; P.NewGuideConfigDatas.TaskNewConfigData = taskConfigData.TaskNewConfigData; });
		}
		var finishGroudIds = taskConfigData.TaskNewConfigData.GetCompleteGuideGroupIds();
		if (finishGroudIds != null && finishGroudIds.Count() > 0)
		{
			var finishGuides = m_newGuideConfigDataBase.GetNewGuideConfigDataByGroup(finishGroudIds);
			if (finishGuides != null)
			{
				m_finishTaskGuides.AddRange(finishGuides);
				//重置所有引导完成标记为False，未完成
				m_finishTaskGuides.ApplyAllItem(P => {P.FinishFlag = false;P.IsGuideUIReady = false; P.NewGuideConfigDatas.TaskNewConfigData = taskConfigData.TaskNewConfigData; });
			}
		}
	}
    public void ClearTaskGuides(bool clearAllGuides)
    {
        m_currentTaskGuides.ApplyAllItem(P => P.GuideBtnUIReadyAct = null);
        m_currentTaskGuides.Clear();
        if (clearAllGuides)
        {
            m_finishTaskGuides.ApplyAllItem(P => P.GuideBtnUIReadyAct = null);
            m_finishTaskGuides.Clear();
            m_CachfinishTaskGuides.Clear();
        }

    }
    //任务完成，请空当前任务引导列表
    private void TaskFinished(INotifyArgs args)
    {
        var taskConfigData = (TaskState)args;
		if (taskConfigData == null)//jamfing//if (taskConfigData != null)
        {
			TraceUtil.Log(SystemModel.Rocky, "(完成或中断)清空任务引导，任务Id：" + TaskModel.Instance.CurrentTaskId);//taskConfigData.dwTaskID);
        }
        else
        {
			//jamfing add :当服务器下发任务完成时，前端认为当前任务中断了，CurrentTaskId为null，故不会走下面的数据清空，造成报错//
			if (!TaskModel.Instance.CurrentTaskId.HasValue || (TaskModel.Instance.CurrentTaskId.HasValue && taskConfigData.dwTaskID == TaskModel.Instance.CurrentTaskId))
            {
                ClearTaskGuides(false);
                //任务完成后续引导(TODO)
                m_guideIndex = 0;
                m_fishGuideIndex = 0;
            }
        }
    }
    private void TaskBreak(INotifyArgs args)
    {
        var taskConfigData = TaskModel.Instance.FindRuningTaskState();
        if (taskConfigData != null)
        {
            TraceUtil.Log(SystemModel.Rocky, "(完成或中断)清空任务引导，任务Id：" + taskConfigData.dwTaskID);
        }
        ClearTaskGuides(true);
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.TaskTrigger.ToString(), TaskTriggerHandler);
        AddEventHandler(EventTypeEnum.TaskFinish.ToString(), TaskFinished);
        AddEventHandler(EventTypeEnum.TaskBreak.ToString(), TaskBreak);
    }
    #region 任务引导箭头处理
    /// <summary>
    /// 获得下一步要执行的引导数据
    /// </summary>
    /// <returns></returns>
    public TaskGuideExtendData GetNextGuideConfigData()
    {
        if (m_currentTaskGuides.Count > m_guideIndex)
        {            
            return m_currentTaskGuides[m_guideIndex];
        }
        else
        {
            TraceUtil.Log(SystemModel.Rocky, "想开始引导，但是当前引导列表为空");
            return null;
        }
    }
	//判定是否是最后一个步骤//
	public bool IsLastGuideOfTask()
	{
		if (m_currentTaskGuides.Count != 0 && m_currentTaskGuides.Count <= m_guideIndex) {
			return true;		
		}
		return false;
	}
    /// <summary>
    /// 设置引导相关引导的UI准备情况
    /// </summary>
    /// <param name="mappingId"></param>
    public void SetGuideUIReady(int mappingId, bool flag)
    {
        bool found = false;
        foreach (var guideConfigData in m_currentTaskGuides)
        {
            int dataMappingId = guideConfigData.MappingId;

            if (dataMappingId == mappingId)
            {
                guideConfigData.IsGuideUIReady = flag;
                found = true;
            }            
        }
        if (!found && m_finishTaskGuides != null)
        {
            foreach (var guideConfigData in m_finishTaskGuides)
            {
                int dataMappingId = guideConfigData.MappingId;

                if (dataMappingId == mappingId)
                {
                    guideConfigData.IsGuideUIReady = flag;
                }
            }
        }
    }
    /// <summary>
    /// 引导完成
    /// </summary>
    /// <param name="newGuideConfigData"></param>
    public void GuideFinish(TaskGuideExtendData newGuideConfigData, bool isFinishGuide)
    {
        if (isFinishGuide)
        {
            if (m_finishTaskGuides == null || m_finishTaskGuides.Count <= m_fishGuideIndex)
            {
                return;
            }
            if (m_finishTaskGuides[m_fishGuideIndex] == newGuideConfigData)
            {
                newGuideConfigData.FinishFlag = true;
                m_fishGuideIndex++;
            }
            else
            {
                TraceUtil.Log(SystemModel.Rocky, "想完成的引导与引导管理中的当前引导不一致");
            }
        }
        else
        {
            if (m_currentTaskGuides == null || m_currentTaskGuides.Count <= m_guideIndex)
            {
                return;
            }
            if (m_currentTaskGuides[m_guideIndex].NewGuideConfigDatas == newGuideConfigData.NewGuideConfigDatas)
            {
                newGuideConfigData.FinishFlag = true;
                m_guideIndex++;
            }
            else
            {
                TraceUtil.Log(SystemModel.Rocky, "想完成的引导与引导管理中的当前引导不一致");
            }
        }
    }
    #endregion
    #region 后续引导
    /// <summary>
    /// 是否有任务完成后续引导
    /// </summary>
    /// <returns></returns>
    public bool NeedFinishGuide()
    {
        return m_finishTaskGuides.Count > 0 ;
    }
    public bool NeedCacheFinishGuide()
    {
        return m_CachfinishTaskGuides.Count > 0;
    }
    /// <summary>
    /// 缓存任务完成后续引导
    /// </summary>
    public void CacheFinishGuide()
    {
        m_CachfinishTaskGuides.AddRange( m_finishTaskGuides);
    }
    public void CheckCacheFinishGuides()
    {
        if (m_CachfinishTaskGuides.Count>0)
        {
            RaiseEvent(EventTypeEnum.TaskFinishGuideExecuteInvoke.ToString(), null);
        }
        else
        {
            TaskBtnManager.Instance.ResetAllButtonStatus(true);
        }
    }
    /// <summary>
    /// 获得下一步要执行的引导数据
    /// </summary>
    /// <returns></returns>
    public TaskGuideExtendData GetNextFinishGuideConfigData()
    {
        if (m_finishTaskGuides.Count > m_fishGuideIndex)
        {
            return m_finishTaskGuides[m_fishGuideIndex];
        }
        else
        {
            TraceUtil.Log(SystemModel.Rocky, "想开始任务完成引导，但是当前引导列表为空");
            ClearTaskGuides(true);
            return null;
        }
    }
    #endregion
    public void Instantiate()
    {

    }
    /// <summary>
    /// 重新监听事件，由于换服时，任务信息不再重发任务完成。所以现在的处理是任务数据不清除。重新注册一下事件
    /// </summary>
    public void ResetEventHandler()
    {
        RemoveEventHandler(EventTypeEnum.TaskTrigger.ToString(), TaskTriggerHandler);
        RemoveEventHandler(EventTypeEnum.TaskFinish.ToString(), TaskFinished);
        RemoveEventHandler(EventTypeEnum.TaskBreak.ToString(), TaskBreak);

        AddEventHandler(EventTypeEnum.TaskTrigger.ToString(), TaskTriggerHandler);
        AddEventHandler(EventTypeEnum.TaskFinish.ToString(), TaskFinished);
        AddEventHandler(EventTypeEnum.TaskBreak.ToString(), TaskBreak);

    }
    public void LifeOver()
    {
        RemoveEventHandler(EventTypeEnum.TaskTrigger.ToString(), TaskTriggerHandler);
        RemoveEventHandler(EventTypeEnum.TaskFinish.ToString(), TaskFinished);
        RemoveEventHandler(EventTypeEnum.TaskBreak.ToString(), TaskBreak);
        m_instance = null;
       
    }
}
