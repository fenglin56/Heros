using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UI.MainUI;

public class TaskListBehaviour : View {

    public UIGrid Layout;
    public UIDraggablePanel DraggablePanel;
    public GameObject TaskListItemPrefab;
    public Action<TaskState> ItemSelectedAct;

    private List<TaskListItemBehaviour> m_taskListItemBehaviour;

    void Awake()
    {
        RegisterEventHandler();
    }
    public IEnumerator Init(List<TaskState> taskState)
    {
        if (m_taskListItemBehaviour!=null&&m_taskListItemBehaviour.Count > 0)
        {
            m_taskListItemBehaviour.Clear();
        }
        Layout.transform.ClearChild();
        yield return null;
        m_taskListItemBehaviour = new List<TaskListItemBehaviour>();
        for (int i = 0; i < taskState.Count; i++)
        {
            var task = taskState[i].TaskNewConfigData;
            var taskItem=NGUITools.AddChild(Layout.gameObject, TaskListItemPrefab);
            var taskItemBehaviour = taskItem.GetComponent<TaskListItemBehaviour>();

            #region`    引导注入代码
            taskItem.RegisterBtnMappingId(taskState[i].dwTaskID, UIType.Task, BtnMapId_Sub.Task_TaskStateItem);
            #endregion

            taskItemBehaviour.Init(taskState[i]);
            m_taskListItemBehaviour.Add(taskItemBehaviour);
            taskItemBehaviour.ItemClickAct = (taskData,itemBehaviour) =>
                {
                    SoundManager.Instance.PlaySoundEffect("Sound_Button_Task_Name");
                    m_taskListItemBehaviour.ApplyAllItem(P=>P.ItemFocusStatus(false));
                    itemBehaviour.ItemFocusStatus(true);
                    ItemSelectedAct(taskData);
                };
            if (i == 0)  //默认选中第一项
            {
                taskItemBehaviour.ItemSelected();
            }
        }
        yield return null;
        DraggablePanel.ResetPosition();
        Layout.Reposition();
    }
    /// <summary>
    /// 刷新任务快速引导的 新功能开启特效和任务提示文本
    /// </summary>
    /// <param name="args"></param>
    private void RefreshTaskInfo(INotifyArgs args)
    {
        StartCoroutine(Init(TaskModel.Instance.TaskDisplayList));
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.TaskStateRefresh.ToString(), RefreshTaskInfo);
    }
}
