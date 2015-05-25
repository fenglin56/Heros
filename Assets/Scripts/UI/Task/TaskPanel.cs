using UnityEngine;
using System.Collections;
using System.Linq;

public class TaskPanel : MonoBehaviour {

    //public UILabel ChapterName;
    //public UILabel TaskGoalTitle;
    //public UILabel TaskAwardTitle;
    //public UILabel TaskGoalLabel;
    //public LocalButtonCallBack TaskContinueBtn;
    //public GameObject AwardMoneyExp;
    //public GameObject AwardItem;

    //private GameObject m_awardMoneyExp;
    //private GameObject m_awardItem;

    //void Awake()
    //{
    //    TaskContinueBtn.SetCallBackFuntion(TaskContinueHandle);
    //}

    //// Use this for initialization
    //public void InitTaskPanel(TaskConfigData taskItem)
    //{
    //    ChapterName.text = LanguageTextManager.GetString(taskItem._TaskTitle);
    //    TaskGoalLabel.text = LanguageTextManager.GetString(taskItem._TaskGoals);
    //    TaskContinueBtn.SetButtonActive(true);
        
    //    switch (taskItem._AwardType)
    //    {
    //        case 0:
    //            var curHeroVocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
                
    //            if (curHeroVocation == 0)
    //            {
    //                return;
    //            }

    //            var awardItem = taskItem._AwardItemList.Single(P => P._Vocation == curHeroVocation);
                
    //            var propData = ItemDataManager.Instance.GetItemData(awardItem._PropID);
    //            if (propData != null)
    //            {
    //                ShowAwardItemPanel(propData._DisplayIdSmall);
    //            }

    //            break;
    //        case 1:
    //            ShowAwardMoneyExpPanel("JH_UI_BG_1209", taskItem._AwardMoney);
    //            break;
    //        case 2:
    //            ShowAwardMoneyExpPanel("JH_UI_BG_1208", taskItem._AwardExp);
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //void ShowAwardMoneyExpPanel(string spriteName, int awardNum)
    //{
    //    if (m_awardItem != null)
    //    {
    //        DestroyImmediate(m_awardItem);
    //    }

    //    if (m_awardMoneyExp == null)
    //    {
    //        m_awardMoneyExp = Instantiate(AwardMoneyExp) as GameObject;
    //        m_awardMoneyExp.transform.parent = this.transform;
    //    }
    //    m_awardMoneyExp.transform.localPosition = new Vector3(0, -57, 0);
    //    m_awardMoneyExp.transform.localScale = Vector3.one;
    //    m_awardMoneyExp.GetComponent<AwardMoneyExpPanel>().InitPanel(spriteName, awardNum);
    //}

    //void ShowAwardItemPanel(GameObject propGo)
    //{
    //    if (m_awardMoneyExp != null)
    //    {
    //        DestroyImmediate(m_awardMoneyExp);
    //    }

    //    if (m_awardItem == null)
    //    {
    //        m_awardItem = Instantiate(AwardItem) as GameObject ;
    //        m_awardItem.transform.parent = this.transform;
    //    }

    //    m_awardItem.transform.localPosition = new Vector3(0, -57, 0);
    //    m_awardItem.transform.localScale = Vector3.one;
    //    //m_awardItem.GetComponent<AwardItemPanel>().InitPanel(propGo);
    //}

    //void TaskContinueHandle(object obj)
    //{
    //    SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
    //    TaskContinueBtn.SetButtonActive(false);
    //    Guide.TownGuideUIManger_V2.Instance.ContinueGuideButton();
    //    UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, null);
    //}
}
