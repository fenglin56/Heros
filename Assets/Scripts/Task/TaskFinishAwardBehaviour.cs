using UnityEngine;
using System.Collections;
using UI.MainUI;

public class TaskFinishAwardBehaviour : BaseUIPanel
{

    public GameObject TaskAwardItemPrefab;
    public GameObject[] AwardPoints;
    public GameObject FinishEffPoint;
    public SingleButtonCallBack GotoFightBtn;
    public GameObject FinishEff;
    public UILabel TaskTitle;

    public void FinishAct(TaskNewConfigData taskNewConfigData)
    {
        SoundManager.Instance.PlaySoundEffect(taskNewConfigData.TaskCompleteSound);
        TaskTitle.text = LanguageTextManager.GetString(taskNewConfigData.TaskTitle);
        NGUITools.AddChild(FinishEffPoint, FinishEff);

        //任务奖励
        var awards = taskNewConfigData.GetTaskAwardInfo();
        int awardItemCount = Mathf.Min(awards.Length, 3);//最多三项奖励
        for (int i = 0; i < awardItemCount; i++)
        {
            var awardItem = NGUITools.AddChild(AwardPoints[i].gameObject, TaskAwardItemPrefab);
            var awardItemBehaviour = awardItem.GetComponent<TaskAwardItem>();
            int goodId = awards[i].GoodsId;
            var itemFileInfo = ItemDataManager.Instance.GetItemData(goodId);
            if (itemFileInfo != null)
            {
                awardItemBehaviour.InitItemData(itemFileInfo, awards[i].AwardAmount);
            }
            else
            {
                TraceUtil.Log(SystemModel.Rocky, TraceLevel.Error, "Task no good:" + goodId);
            }
        }

        GotoFightBtn.SetCallBackFuntion((obj) =>
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Task_Award");
                //确定按钮
                Destroy(gameObject);
            });
        var runningTask = TaskModel.Instance.FindRuningTaskState();
        bool registerBtnFlag = true;
        if (runningTask != null
            && runningTask.dwTaskID != taskNewConfigData.TaskID)
        {
            registerBtnFlag = false;
        }
        if (registerBtnFlag)
        {
            GotoFightBtn.gameObject.RegisterBtnMappingId(UIType.Task, BtnMapId_Sub.Task_FinishAwardConfirm);
        }
    }
}
