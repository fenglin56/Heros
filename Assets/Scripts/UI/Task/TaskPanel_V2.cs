using UnityEngine;
using System.Collections;
using System.Linq;


//public class TaskPanel_V2 : MonoBehaviour {

//    public LocalButtonCallBack TaskContinueBtn;
//    public LocalButtonCallBack TaskViewBtn;
//    public UILabel TaskGoalLabel;
//    public UILabel ChapterName;
//    public GameObject AwardMoneyExp;
//    public UISprite AwardXiuWei;
//    public UISprite AwardYuanBao;
//    public GameObject AwardItem;
//    public GameObject ViewLeftPanel;
//    public GameObject ViewButtomPanel;
//    public GameObject ContinueEffect;
//    public UILabel EnableLevel;
//    public UILabel FunctionName;
//    public UISprite FunctionIcon;

//    private GameObject m_awardMoneyExp;
//    private GameObject m_awardItem;
//    private UIImageButton m_imageButton;

//    private bool m_isShowTaskPanel;
//    private bool m_isShowEnableTips = false;


//    void Awake()
//    {
//        UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, CloseViewPanel);
//        //UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseMainUI, UpdateViewButton);

//        m_isShowTaskPanel = false;
//        ViewLeftPanel.SetActive(m_isShowTaskPanel);
//        ViewButtomPanel.SetActive(m_isShowTaskPanel);
//    }

//    void Start()
//    {
//        TaskContinueBtn.SetCallBackFuntion(TaskContinueHandle);
//        TaskViewBtn.SetCallBackFuntion(TaskViewHandle);
//        m_imageButton = TaskViewBtn.GetComponent<UIImageButton>();
//    }

//    public void ResetContinue()
//    {
//        TaskContinueBtn.GetComponent<UIButtonColor>().enabled = true;
//        TaskContinueBtn.SetButtonActive(true);

//        ContinueEffect.SetActive(true);
//    }

//    //void UpdateViewButton(object obj)
//    //{
//    //    TaskViewBtn.gameObject.SetActive(false);
//    //    TweenScale.Begin(TaskViewBtn.gameObject, 0.3f, new Vector3(0.1f, 0.1f, 0.1f), Vector3.one, null);
//    //    TaskViewBtn.gameObject.SetActive(true);
//    //}

//    void CloseViewPanel(object obj)
//    {
//        m_isShowTaskPanel = false;
//        ViewLeftPanel.SetActive(m_isShowTaskPanel);
//        //ViewButtomPanel.SetActive(m_isShowTaskPanel);
//        //m_imageButton.normalSprite = "JH_UI_Button_1200_01";
//        //m_imageButton.hoverSprite = "JH_UI_Button_1200_02";
//        //m_imageButton.pressedSprite = "JH_UI_Button_1200_02";
//        //TaskViewBtn.gameObject.SetActive(false);
//        //TweenScale.Begin(TaskViewBtn.gameObject, 0.3f, new Vector3(0.1f, 0.1f, 0.1f), Vector3.one, null);
//        //TaskViewBtn.gameObject.SetActive(true);
//    }

//    void OnDestroy()
//    {
//        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, CloseViewPanel);
//        //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseMainUI, UpdateViewButton);
//    }

//    public void InitTaskPanel(TaskConfigData taskItem)
//    {
//        TaskContinueBtn.GetComponent<UIButtonColor>().enabled = true;
//        TaskContinueBtn.SetButtonActive(true);

//        ContinueEffect.SetActive(true);
//        TaskGoalLabel.text = LanguageTextManager.GetString(taskItem._TaskGoals);
//        ChapterName.text = LanguageTextManager.GetString(taskItem._TaskTitle);

//        EnableLevel.text = string.Format("{0}{1}", taskItem._EnableLevel, LanguageTextManager.GetString("IDS_H1_518")); //级开启
//        FunctionName.text = LanguageTextManager.GetString(taskItem._FunctionName);
//        FunctionIcon.spriteName = taskItem._FunctionIconName;

//        if (taskItem._IsEnableLvTips)
//        {
//            ViewButtomPanel.SetActive(true);
//        }
//        else
//        {
//            ViewButtomPanel.SetActive(false);
//        }

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



//    void TaskContinue()
//    {
//        TaskContinueHandle(null);
//    }

//    void ShowAwardItemPanel(UISprite sprite, int awardNum)
//    {
//        if (m_awardItem != null)
//        {
//            DestroyImmediate(m_awardItem);
//        }

//        if (m_awardMoneyExp == null)
//        {
//            m_awardMoneyExp = Instantiate(AwardMoneyExp) as GameObject;
//            m_awardMoneyExp.transform.parent = this.ViewLeftPanel.transform;
//        }

//        m_awardMoneyExp.transform.localPosition = new Vector3(-25, -75, 0);
//        m_awardMoneyExp.transform.localScale = Vector3.one;
//        m_awardMoneyExp.GetComponent<AwardMoneyExpPanel>().InitPanel(sprite, awardNum);
//    }

//    void ShowAwardMoneyExpPanel(string spriteName, int awardNum)
//    {
//        if (m_awardItem != null)
//        {
//            DestroyImmediate(m_awardItem);
//        }

//        if (m_awardMoneyExp == null)
//        {
//            m_awardMoneyExp = Instantiate(AwardMoneyExp) as GameObject;
//            m_awardMoneyExp.transform.parent = this.ViewLeftPanel.transform;
//        }
//        m_awardMoneyExp.transform.localPosition = new Vector3(-25, -75, 0);
//        m_awardMoneyExp.transform.localScale = Vector3.one;
//        m_awardMoneyExp.GetComponent<AwardMoneyExpPanel>().InitPanel(spriteName, awardNum);
//    }

//    void ShowAwardItemPanel(string spriteName)
//    {
//        if (m_awardMoneyExp != null)
//        {
//            DestroyImmediate(m_awardMoneyExp);
//        }

//        if (m_awardItem == null)
//        {
//            m_awardItem = Instantiate(AwardItem) as GameObject;
//            m_awardItem.transform.parent = this.ViewLeftPanel.transform;
//        }

//        m_awardItem.transform.localPosition = new Vector3(-23, -65, 0);
//        m_awardItem.transform.localScale = Vector3.one;
//        m_awardItem.GetComponent<AwardItemPanel>().InitPanel(spriteName);
//    }

//    void TaskContinueHandle(object obj)
//    {
//        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
//        TaskContinueBtn.GetComponent<UIButtonColor>().enabled = false;
//        TaskContinueBtn.SetButtonActive(false);
//        ContinueEffect.SetActive(false);
//        Guide.TownGuideUIManger_V2.Instance.ContinueGuideButton();
//    }

//    void TaskViewHandle(object obj)
//    {
//        m_isShowTaskPanel = !m_isShowTaskPanel;
//        ViewLeftPanel.SetActive(m_isShowTaskPanel);

//        //if(m_isShowEnableTips)
//        //    ViewButtomPanel.SetActive(m_isShowTaskPanel);
//        //TaskViewBtn.SetButtonActive(false);
        
//        //if (m_isShowTaskPanel)
//        //{
//        //    m_imageButton.normalSprite = "JH_UI_Button_1201_01";
//        //    m_imageButton.hoverSprite = "JH_UI_Button_1201_02";
//        //    m_imageButton.pressedSprite = "JH_UI_Button_1201_02";
//        //    TweenScale.Begin(TaskViewBtn.gameObject, 0.3f, new Vector3(0.1f, 0.1f, 0.1f), TaskViewBtn.transform.localScale, EnableTaskView);
//        //}
//        //else
//        //{

//        //    m_imageButton.normalSprite = "JH_UI_Button_1200_01";
//        //    m_imageButton.hoverSprite = "JH_UI_Button_1200_02";
//        //    m_imageButton.pressedSprite = "JH_UI_Button_1200_02";
//        //    TweenScale.Begin(TaskViewBtn.gameObject, 0.3f, new Vector3(0.1f, 0.1f, 0.1f), TaskViewBtn.transform.localScale, EnableTaskView);
//        //}
//    }

//    //void EnableTaskView(object obj)
//    //{
//    //    TaskViewBtn.SetButtonActive(true);
//    //}
//}
