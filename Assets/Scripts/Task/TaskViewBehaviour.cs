using UnityEngine;
using System.Collections;
using System;
using UI.MainUI;
using System.Linq;

public class TaskViewBehaviour : MonoBehaviour {

    public GameObject TaskAwardItemPrefab;
    public GameObject[] AwardPoints;
    public SpriteSwith TaskTypeSpriteSwitch;
    public UILabel TaskTitle;
    public UILabel TaskProcess;
    public UILabel TaskDesc;
    public UILabel TaskTarget;
    public SingleButtonCallBack GotoFightBtn;
    public Action<TaskState> GotoFightTaskAct;

    private UILabel m_gotoFightText;
    private TaskState m_viewTask;
    void Awake()
    {
        GotoFightBtn.SetCallBackFuntion((viewTask) =>
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Task_QuickGuide");
                if (GotoFightTaskAct != null)
                {
                    GotoFightTaskAct(m_viewTask);
                }
            });
        m_gotoFightText = GotoFightBtn.GetComponentInChildren<UILabel>();
        Init(null);
    }
    /// <summary>
    /// 初始化传入的任务
    /// </summary>
    public void Init(TaskState taskState)
    {
        m_viewTask = taskState;
        if (m_viewTask == null)
        {
            TaskTypeSpriteSwitch.ChangeSprite(0);
            TaskTitle.text = string.Empty;
            TaskDesc.text = string.Empty;
            TaskTarget.text = string.Empty;
            GotoFightBtn.gameObject.SetActive(false);
        }
        else
        {
            //根据任务类型改变Sprite 1=新手任务；2=主线任务；3=支线任务；4=日常任务；5=循环任务
            TaskTypeSpriteSwitch.ChangeSprite(m_viewTask.TaskNewConfigData.TaskSeries - 1);
            TaskTitle.text = LanguageTextManager.GetString(m_viewTask.TaskNewConfigData.TaskTitle);
            //进度待解决TODO
            TaskProcess.text = string.Format("{0}/{1}", taskState.byRate, taskState.byAllRate);
            TaskDesc.text = LanguageTextManager.GetString(m_viewTask.TaskNewConfigData.TaskDescription);
            //任务目标处理待解决TODO
            TaskTarget.text =LanguageTextManager.GetString(m_viewTask.TaskNewConfigData.TaskGoals);
            //任务奖励
            var awards=taskState.TaskNewConfigData.GetTaskAwardInfo();
            int awardItemCount = Mathf.Min(awards.Length, 3);//最多三项奖励
            for (int i = 0; i < 3; i++)
            {
                AwardPoints[i].transform.ClearChild();
            }
            for (int i = 0; i < awardItemCount; i++)
            {
                var awardItem = NGUITools.AddChild(AwardPoints[i].gameObject, TaskAwardItemPrefab);
                var awardItemBehaviour = awardItem.GetComponent<TaskAwardItem>();
                int goodId = awards[i].GoodsId;
                var itemFileInfo =ItemDataManager.Instance.GetItemData(goodId);
                if (itemFileInfo != null)
                {
                    awardItemBehaviour.InitItemData(itemFileInfo, awards[i].AwardAmount);
                }
                else
                {
                    TraceUtil.Log(SystemModel.Rocky, TraceLevel.Error, "Task no good:" + goodId);
                }
            }
            //如果箭头组为0，则不显示“前往”按钮
            if (m_viewTask.TaskNewConfigData.GuideGroup == "0" && m_viewTask.TaskNewConfigData.Link == 0)
            {
                GotoFightBtn.gameObject.SetActive(false);
            }
            else
            {
                m_gotoFightText.text = LanguageTextManager.GetString(m_viewTask.TaskNewConfigData.GuideText);
                GotoFightBtn.gameObject.SetActive(true);
            }
        }
    }
}
