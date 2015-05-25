using UnityEngine;
using System.Collections;
using System;

public class TaskListItemBehaviour : MonoBehaviour {

    public UILabel ActiveTaskName;
    public UILabel InactiveTaskName;
    public Action<TaskState, TaskListItemBehaviour> ItemClickAct;
    
    private SingleButtonCallBack m_callBackComponent;
    private SpriteSwith m_taskTypeIcon;
    private SpriteSwith m_taskBg;
    private TaskState m_taskState;
	private string m_taskTitle;
    void Awake()
    {
        m_callBackComponent = GetComponent<SingleButtonCallBack>();
        m_taskTypeIcon = m_callBackComponent.spriteSwithList[0];  //获得任务类型的Sprite切换器
        m_taskBg = m_callBackComponent.spriteSwithList[1];  
        m_callBackComponent .SetCallBackFuntion((obj)=>
            {
                ItemSelected();
            });
    }
    public void Init(TaskState taskState)
    {
        m_taskState = taskState;
        if (taskState != null)
        {
			m_taskTitle=LanguageTextManager.GetString(taskState.TaskNewConfigData.TaskTitle);
            InactiveTaskName.text = "[ffffff]" + m_taskTitle + "[-]";
            ActiveTaskName.text = string.Empty;
            //根据任务类型改变Sprite 1=新手任务；2=主线任务；3=支线任务；4=日常任务；5=循环任务
            m_taskTypeIcon.ChangeSprite(taskState.TaskNewConfigData.TaskSeries-1);
        }
    }
    public void ItemSelected()
    {
        if (ItemClickAct != null)
        {
            ItemClickAct(m_taskState,this);
        }
    }
    public void ItemFocusStatus(bool flag)
    {
        if (flag)
        {
            m_taskBg.ChangeSprite(2);
            InactiveTaskName.text = string.Empty;
			ActiveTaskName.text = "[802800]" + m_taskTitle + "[-]";

        }
        else
        {
            m_taskBg.ChangeSprite(1);
            InactiveTaskName.text = "[ffffff]" + m_taskTitle + "[-]";
            ActiveTaskName.text = string.Empty;
        }
    }
}
