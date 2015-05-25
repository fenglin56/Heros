using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 新任务快速引导面板管理
/// </summary>
public class TaskQuickGuidePanelBehaviour : View {

    public SingleButtonCallBack TaskContinueBtn;
    public GameObject HeadIconPoint;
    public GameObject TaskTips;
    public GameObject NewFunctionEff;
    public UILabel m_taskTips;

    public Action<TaskState> ClickTaskStateAct;
    private TaskState m_taskState;
    void Awake()
    {
        TaskTips.SetActive(false);

        RegisterEventHandler();
        TaskContinueBtn.SetCallBackFuntion((obj) =>
        {
            StartCoroutine(TaskContinueClickEvent(m_taskState));
        });
        TaskContinueBtn.OnActiveChanged = (flag) =>
        {
            if (flag)
            {
                RefreshTaskQuickInfo();
            }
            else
            {
                TaskTips.SetActive(false);
                NewFunctionEff.SetActive(false);
            }
        };
        
    }
    /// <summary>
    /// 点击后，下一帧才能再次点击
    /// </summary>
    /// <returns></returns>
    IEnumerator TaskContinueClickEvent(TaskState tast)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Task_GuideHead");
        if (tast != null)
        {
            if (ClickTaskStateAct != null)
            {
                ClickTaskStateAct(tast);
            }
            TaskContinueBtn.Enable = false;
            yield return new WaitForSeconds(tast.TaskNewConfigData.DelayTime / 1000f);  //延时期间禁用快速引导按钮
            TaskContinueBtn.Enable = true;
        }
        else
        {
            yield return null;
        }
    }
    private void QuickTaskGuideInvoke(INotifyArgs args)
    {
        TaskState tast = TaskModel.Instance.FindRuningTaskState();
        StartCoroutine(ButtonDelayToEnable(tast));
    }
    private IEnumerator ButtonDelayToEnable(TaskState tast)
    {
        if (tast != null)
        {
            TaskContinueBtn.Enable = false;
            yield return new WaitForSeconds(tast.TaskNewConfigData.DelayTime / 1000f);  //延时期间禁用快速引导按钮
            TaskContinueBtn.Enable = true;
        }
        else
        {
            yield return null;
        }
    }
    void Start()
    {
        //引导
        TaskContinueBtn.gameObject.RegisterBtnMappingId(UI.MainUI.UIType.NewbieGuide, BtnMapId_Sub.Empty);
        TaskContinueBtn.GetComponent<GuideBtnBehaviour>().ResponseOnClickEvent = false;  //不会触发ClickTheGuideBtn和ClickOtherButton消息
    }
    /// <summary>
    /// 刷新任务快速引导的 新功能开启特效和任务提示文本
    /// </summary>
    /// <param name="args"></param>
    public void RefreshTaskQuickInfo()
    {
        var funEff = TaskModel.Instance.GetTaskOpenNewFunctionIcon();
        HeadIconPoint.transform.ClearChild();
        if (funEff != null)
        {
            NewFunctionEff.SetActive(true);
            var headIcon = NGUITools.AddChild(HeadIconPoint, funEff);
            headIcon.transform.localScale = funEff.transform.localScale;
        }
        else
        {
            NewFunctionEff.SetActive(false);
        }
        if (m_taskState != null)
        {
            TaskTips.SetActive(true);
            m_taskTips.text = LanguageTextManager.GetString(m_taskState.TaskNewConfigData.TaskTitle);
        }
        else
        {
            TaskTips.SetActive(false);
            NewFunctionEff.SetActive(false);
        }
    }
    public void ShowOrHideTips()
    {
        if (m_taskState != null)
        {
            TaskTips.SetActive(TaskModel.Instance.IsTaskFinished);
        }
        else
        {
            TaskTips.SetActive(false);
            NewFunctionEff.SetActive(false);
        }
    }
    public void InitTaskPanel(TaskState taskState)
    {        
        m_taskState = taskState;
        RefreshTaskQuickInfo();
    }

    void OnDestroy()
    {
        RemoveEventHandler(EventTypeEnum.TaskExecuteInvoke.ToString(), QuickTaskGuideInvoke);
    }

    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.TaskExecuteInvoke.ToString(), QuickTaskGuideInvoke);
    }
}
