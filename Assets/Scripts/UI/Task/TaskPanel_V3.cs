using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// TownUIScene - NewbieGuideUI 对象上
/// </summary>
//public class TaskPanel_V3 : MonoBehaviour {

//    public LocalButtonCallBack TaskContinueBtn;
//    public LocalButtonCallBack TaskViewBtn;
//    public UISprite DefaultHeadIcon;
//    public UISprite TaskHeadIcon;
//    public UILabel TaskGoalLabel;
//    public UILabel ChapterName;
//    public GameObject AwardMoneyExp;
//    public UISprite AwardXiuWei;
//    public UISprite AwardYuanBao;
//    public GameObject AwardItem;
//    public GameObject ViewLeftPanel;
//    public GameObject ContinueEffect;
//    public Transform AttachPoint;
//    public GameObject NewTaskEff;
//    public GameObject StartTextEff;
   
//    private UIImageButton m_imageButton;

//    private bool m_isShowTaskPanel;
//    private bool m_isShowEnableTips = false;
//    private Animation m_leftViewExtCloseAnim;

//    private const string DEFAULT_SPRITE_NAME = "JH_UI_Icon_1116_02";
//    private string m_taskSpriteName=string.Empty;
//    private TweenAlpha m_defaultHeadIcon;
//    private TweenAlpha m_taskHeadIcon;
//    /// <summary>
//    /// 是否显示新任务即将开始图标
//    /// </summary>
//    private bool m_newTaskIcon=false;

//    void Awake()
//    {
//        UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, CloseViewPanel);
//        m_leftViewExtCloseAnim = ViewLeftPanel.GetComponentInChildren<Animation>();

//        m_isShowTaskPanel = true;
//        TaskViewHandle(null);
//        NewTaskEff.SetActive(false);
//        StartTextEff.SetActive(false);

//        m_defaultHeadIcon = DefaultHeadIcon.GetComponent<TweenAlpha>();
//        m_taskHeadIcon = TaskHeadIcon.GetComponent<TweenAlpha>();
//    }

//    void Start()
//    {
//        TaskContinueBtn.SetCallBackFuntion(TaskContinueHandle);
//        TaskViewBtn.SetCallBackFuntion(TaskViewHandle);
//        m_imageButton = TaskViewBtn.GetComponent<UIImageButton>();
//    }

//    public void ResetContinue()
//    {
//        //TaskContinueBtn.GetComponent<UIButtonColor>().enabled = true;
//        TaskContinueBtn.SetButtonActive(true);

//        ContinueEffect.SetActive(true);
//        TraceUtil.Log("开始协同 SwitchHeadIcon");
//        StartCoroutine("SwitchHeadIcon");
//    }
//    void CloseViewPanel(object obj)
//    {
//        m_isShowTaskPanel = true;
//        TaskViewHandle(null);
//        //ViewLeftPanel.SetActive(m_isShowTaskPanel);
//    }

//    void OnDestroy()
//    {
//        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, CloseViewPanel);
//    }

//    public void InitTaskPanel(TaskConfigData taskItem)
//    {
//        //TaskContinueBtn.GetComponent<UIButtonColor>().enabled = true;
//        TaskContinueBtn.SetButtonActive(true);

//        ContinueEffect.SetActive(true);
//        m_taskSpriteName = taskItem._IsEnableLvTips ? taskItem._FunctionIconName : DEFAULT_SPRITE_NAME;
//        TaskHeadIcon.spriteName = m_taskSpriteName;
//        TraceUtil.Log("开始协同 SwitchHeadIcon");
//        StartCoroutine("SwitchHeadIcon");
//        TaskGoalLabel.text = LanguageTextManager.GetString(taskItem._TaskGoals);
//        ChapterName.text = LanguageTextManager.GetString(taskItem._TaskTitle);

//        var curHeroVocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
//        if (curHeroVocation == 0)
//        {
//            return;
//        }

//        switch (taskItem._AwardType)
//        {
//            case 0:  //奖励物品
//                var awardItem = taskItem._AwardItemList.Single(P => P._Vocation == curHeroVocation);

//                var propData = ItemDataManager.Instance.GetItemData(awardItem._PropID);
//                if (propData.smallDisplay != "0")
//                {
//                    ShowAwardItemPanel(propData.smallDisplay);
//                }
//                else
//                    TraceUtil.Log("物品缩略图为空！");

//                break;
//            case 1:  //奖励铜钱
//                ShowAwardMoneyExpPanel("JH_UI_BG_1209", taskItem._AwardMoney);
//                break;
//            case 2:  //奖励经验
//                ShowAwardMoneyExpPanel("JH_UI_BG_1208", taskItem._AwardExp);
//                break;
//            case 3: //奖励活力值
//                ShowAwardMoneyExpPanel("JH_UI_BG_1251", taskItem._AwardActive);
//                break;
//            case 4: //奖励修为值
//                ShowAwardItemPanel(AwardXiuWei, taskItem._AwardXiuWei);
//                break;
//            case 5:  //奖励元宝
//                ShowAwardItemPanel(AwardYuanBao, taskItem._AwardMoney);
//                break;
//            default:
//                break;
//        }
//        Invoke("TaskContinue", 1f);  //强引导延后1秒触发
        
//        if (taskItem._GuideType == 1)
//        {
//            NewbieGuideManager_V2.Instance.IsConstraintGuide = true;
//        }

//    }
//    /// <summary>
//    /// 策划要求来回切换任务头像，美术做的动画。新任务特效 105帧，即将开启特效 210帧。在105帧的时候任务头像完全消失。0帧时完全显示
//    /// </summary>
//    /// <returns></returns>
//    IEnumerator SwitchHeadIcon()
//    {
//        WaitForSeconds ws=new WaitForSeconds(3);
//        while (true)
//        {           
//            NewTaskEff.SetActive(false);
//            NewTaskEff.SetActive(true);
//            StartTextEff.SetActive(false);
//            StartTextEff.SetActive(true);
//            m_newTaskIcon = true;
//            m_defaultHeadIcon.from = 1;
//            m_defaultHeadIcon.to= 0;
//            m_defaultHeadIcon.Play(true);
//            m_taskHeadIcon.from = 0;
//            m_taskHeadIcon.to = 1;
//            m_taskHeadIcon.Play(true);           

//            yield return ws;
//            NewTaskEff.SetActive(false);
//            NewTaskEff.SetActive(true);
//            StartTextEff.SetActive(false);
//            m_newTaskIcon = false;
//            m_defaultHeadIcon.from = 0;
//            m_defaultHeadIcon.to = 1;
//            m_defaultHeadIcon.Play(true);
//            m_taskHeadIcon.from = 1;
//            m_taskHeadIcon.to = 0;
//            m_taskHeadIcon.Play(true);

//            yield return ws;

//            //m_defaultHeadIcon.from = 1;
//            //m_defaultHeadIcon.to = 0;
//            //m_taskHeadIcon.from = 0;
//            //m_taskHeadIcon.to = 1;
//        }       
//    }
//    void Update()
//    {
//        if (UI.MainUI.MainUIController.Instance.CurrentUIStatus != UI.MainUI.UIType.Empty)
//        {
//            StartTextEff.SetActive(false);
//        }
//        else if(m_newTaskIcon)
//        {
//            StartTextEff.SetActive(true);
//        }
//    }
//    void TaskContinue()
//    {
//        TaskContinueHandle(null);
//    }

//    void ShowAwardItemPanel(UISprite sprite, int awardNum)
//    {
//        AwardItem.SetActive(false);
//        AwardMoneyExp.SetActive(true);

//        AwardMoneyExp.GetComponent<AwardMoneyExpPanel>().InitPanel(sprite, awardNum);
//    }

//    void ShowAwardMoneyExpPanel(string spriteName, int awardNum)
//    {
//        AwardItem.SetActive(false);
//        AwardMoneyExp.SetActive(true);
//        AwardMoneyExp.GetComponent<AwardMoneyExpPanel>().InitPanel(spriteName, awardNum);
//    }

//    void ShowAwardItemPanel(string spriteName)
//    {
//            AwardMoneyExp.SetActive(false);

//            AwardItem.SetActive(true);

//            AwardItem.GetComponent<AwardItemPanel>().InitPanel(spriteName);
//    }

//    void TaskContinueHandle(object obj)
//    {
//        //任务开始主按钮的点击
//        TraceUtil.Log("开始任务：" + UI.SystemFuntionButton.Instance.MyBtnEneble);
//        UI.SystemFuntionButton.Instance.MyBtnEneble = true;
//        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
//        //TaskContinueBtn.GetComponent<UIButtonColor>().enabled = false;
//        TaskContinueBtn.SetButtonActive(false);
//        ContinueEffect.SetActive(false);
//        TraceUtil.Log("协同停止 SwitchHeadIcon");
//        StopCoroutine("SwitchHeadIcon");
//        TaskHeadIcon.alpha =0;
//        DefaultHeadIcon.alpha = 1;
//        Guide.TownGuideUIManger_V2.Instance.ContinueGuideButton();
//    }

//    void TaskViewHandle(object obj)
//    {
//        m_isShowTaskPanel = !m_isShowTaskPanel;
//        string animName = m_isShowTaskPanel ? "Eff_UI_NewbieGuide_Continue03_ui_a_01" : "Eff_UI_NewbieGuide_Continue03_ui_a_02";
//        m_leftViewExtCloseAnim.Play(animName);
//        //ViewLeftPanel.SetActive(m_isShowTaskPanel);
//    }
//}
