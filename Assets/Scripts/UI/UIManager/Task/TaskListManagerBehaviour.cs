using UnityEngine;
using System.Collections;
using UI.MainUI;

public class TaskListManagerBehaviour : BaseUIPanel{
    public GameObject Prefab_TaskList;
    public GameObject Prefab_ViewTask;
    public GameObject TaskListPoint;
    public GameObject TaskViewPoint;
    public SingleButtonCallBack BackBtn;
    public GameObject CommonPanelTitle_Prefab;

    private GameObject m_taskListInstance, m_viewTaskInstance;
    private TaskViewBehaviour m_viewTask;
    private TaskListBehaviour m_taskListBehaviour;
    private BaseCommonPanelTitle m_commonPanelTitle;
    //是否创建实例
    private bool m_createInstance, m_isUpgradeBack;
    // Use this for initialization
    void Awake()
    {
        //返回按钮点击
        BackBtn.SetCallBackFuntion((obj) =>
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Task_Leave");
            this.Close();
        });
        //返回按钮按下/松开效果
        BackBtn.SetPressCallBack((isPressed) =>
        {
            BackBtn.spriteSwithList.ApplyAllItem(P => P.ChangeSprite(isPressed ? 2 : 1));
        });
        m_createInstance = true;
        var commonPanel = NGUITools.AddChild(gameObject, CommonPanelTitle_Prefab);
        commonPanel.transform.localPosition = CommonPanelTitle_Prefab.transform.localPosition;
        m_commonPanelTitle = commonPanel.GetComponent<BaseCommonPanelTitle>();
        m_commonPanelTitle.Init(CommonTitleType.Money, CommonTitleType.GoldIngot);
        RegisterEventHandler();
		TaskGuideBtnRegister ();
    }
    public void Init()
    {
        if (m_createInstance)
        {
            m_createInstance = false;

            m_viewTaskInstance = NGUITools.AddChild(TaskViewPoint, Prefab_ViewTask);
            m_viewTaskInstance.transform.localPosition = Prefab_ViewTask.transform.localPosition;
            m_viewTask = m_viewTaskInstance.GetComponent<TaskViewBehaviour>();
            m_viewTask.GotoFightTaskAct=(taskState) =>
                {
				Close();
                //TaskModel.Instance.ManualTriggerTask(taskState);//修改
				LinkConfigItemData item=PathLinkConfigManager.Instance.GetLinkConfigItem(taskState.TaskNewConfigData.Link.ToString());
				TaskModel.Instance.JumpView(item);
                };

            m_taskListInstance = NGUITools.AddChild(TaskListPoint, Prefab_TaskList);
            m_taskListBehaviour = m_taskListInstance.GetComponent<TaskListBehaviour>();
            m_taskListBehaviour.ItemSelectedAct = (taskState) =>
                {
                    m_viewTask.Init(taskState);
                };
        }
        StartCoroutine(m_taskListBehaviour.Init(TaskModel.Instance.TaskDisplayList));
    }
   
    protected override void RegisterEventHandler()
    {
       
    }
    public override void Show(params object[] value)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Task_Enter");
        Init();
        base.Show(value);

        m_commonPanelTitle.TweenShow();
    }

    public override void Close()
    {
        if (!IsShow)
            return;
        UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);        
        StartCoroutine(AnimToClose());
        m_commonPanelTitle.tweenClose();
    }
    /// <summary>
    /// 播放关闭动画
    /// </summary>
    /// <returns>The to close.</returns>
    private IEnumerator AnimToClose()
    {
        yield return new WaitForSeconds(0.16f);   //动画时长
        UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
        base.Close();
    }
	/// <summary>
	/// 引导按钮注入代码
	/// </summary>
	private void TaskGuideBtnRegister()
	{
		BackBtn.gameObject.RegisterBtnMappingId(UIType.Task, BtnMapId_Sub.Task_Back);
	}
}
