using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI.PVP
{
    /// <summary>
    /// PVP战斗面板
    /// </summary>
    public class PVPPanelUIManager : View
    {
        //战斗准备界面
        public PVPInterfaceInfo PVPInterfaceInfoPanel;  //界面信息面板
        
        public LocalButtonCallBack ExitButton;      //退出按钮
        public LocalButtonCallBack ScoreTopButton;  //排行榜按钮
        public LocalButtonCallBack ChallengeButton; //挑战按钮    
        public GameObject ChallengeButtonEffect;  //战鼓按钮特效
        public GameObject ChallengeLabel;
        public GameObject ChallengeFoundTip;        

        public UILabel Label_SearchTime;

        public LocalButtonCallBack AddTimesButton;  //增加挑战次数按钮

        private bool m_isChallenging = false;

        public GameObject PVPSearchEffect;
        private ContainerHeroView HeroView;          //英雄view
        private ContainerHeroView CompetitorView;    //对手view
        public GameObject HeroViewCameraPrefab;     //
        public Transform HeroViewCameraTrans;

        public UISlider Slider_prestigeValue;//威望值
        public UILabel Label_TitleName;     //称号
        public UILabel Label_prestigeValue;

        public GameObject GO_DialogBoard;
        public UILabel Label_MyDialog;
        public UILabel Label_OtherDialog;

        public UILabel Label_myName;
        public GameObject Name_competitor;
        public UILabel Label_competitorName;

        public PVPTimesMessagePanel AddTimesMessagePanel;    //增加次数消息面板

        public TweenPosition InfoBoardTweenPos;
        private Vector3 m_infoBoardInitialPos;
        private Vector3 m_infoBoardHidePos;

        private int m_VocationID = 1;

        private bool m_IsFindPVPPlayer = false;

        //排行界面
        //public UIDraggablePanel UIDraggablePanel_Top;
        public PVPRankInfoControl RankingInfoControl;

        public CategoryTabControl CategoryTabControl_Rank;

        public RoleViewPanel RoleViewPanelPrefab;
        private RoleViewPanel m_RoleViewPanel;
        public Transform RoleViewPoint;

        public LocalButtonCallBack BackButton;      //返回按钮
        public ItemPagerManager ItemPageManager_Rank;
        private const int mRankItemNum = 20;

        //public UIGrid UIGrid_Top;
        //public PVPRankInfoControl MyRankInfoControl;

        public GameObject PVPInterface;
        public GameObject TopBoard;

        public UILabel Label_RuleTitle;
        public UILabel Label_RuleContent;

        private List<PVPRankInfoControl> m_rankInfoCtrlList = new List<PVPRankInfoControl>();

        private Dictionary<RoleAttributeType, SingleRoleAtrribute> m_MedalAttributeDic;
        private EquipmentData m_currentMedalData;

        private int m_searchTime = 0;//搜索计时

        private int[] m_guideBtnID;
        // Use this for initialization
        void Awake()
        {
            m_guideBtnID = new int[5];
            //MainUIController.Instance.SetPanelActivEvent += new MainUIController.SetPanelDelegate(SetPanelEnable);
            //MainUIController.Instance.SaveUIStatusEvent += new MainUIController.SaveUIStatusDelegate(SaveUIStatus);
            ExitButton.SetCallBackFuntion(OnExitClick, null);
            ScoreTopButton.SetCallBackFuntion(OnScoreTopClick, null);
            ChallengeButton.SetCallBackFuntion(OnChallengeClick, null);
            AddTimesButton.SetCallBackFuntion(OnAddTimesClick, null);
            BackButton.SetCallBackFuntion(OnBackClick, null);

            m_infoBoardInitialPos = InfoBoardTweenPos.from;
            m_infoBoardHidePos = InfoBoardTweenPos.to;

            Label_RuleTitle.text = LanguageTextManager.GetString("IDS_H1_397");
            Label_RuleContent.text = LanguageTextManager.GetString("IDS_H1_396").Replace(@"\\n", "\n") ;            

            this.RegisterEventHandler();
            InitHeroViewCamera();

            //TODO GuideBtnManager.Instance.RegGuideButton(ExitButton.gameObject, UIType.PVPBattle, SubType.ButtomCommon, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(ScoreTopButton.gameObject, UIType.PVPBattle, SubType.ButtomCommon, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(ChallengeButton.gameObject, UIType.PVPBattle, SubType.ButtomCommon, out m_guideBtnID[2]);
            //TODO GuideBtnManager.Instance.RegGuideButton(AddTimesButton.gameObject, UIType.PVPBattle, SubType.ButtomCommon, out m_guideBtnID[3]);
            //TODO GuideBtnManager.Instance.RegGuideButton(BackButton.gameObject, UIType.PVPBattle, SubType.ButtomCommon, out m_guideBtnID[4]);
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++ )
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        void SetPanelEnable(int[] UIStatus)
        {
            if (UIStatus[0] == (int)UIType.PVPBattle)
            {
                ShowPVPPanel();
            }
            else
            {
                ClosePVPPanel();
            }            
        }

        void SaveUIStatus()
        {
            //MainUIController.Instance.SaveUIPanelStatus(0, UIType.PVPBattle);            
        }

        
        void ShowPVPPanel()
        {
            transform.localPosition = Vector3.zero;

            m_IsFindPVPPlayer = false;

            PVPInterface.SetActive(true);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.SwitchOffShowPlayerInfo, false);
            
            PVPInterfaceInfoPanel.ReadPlayerInfo();

            UpdatePrestigeInfo();

            if (HeroView != null)
            {
                int Vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
                m_VocationID = Vocation;
                HeroView.ShowHeroModelView(Vocation);
            }

            if (InfoBoardTweenPos.gameObject.activeInHierarchy == false)
            {
                InfoBoardTweenPos.gameObject.SetActive(true);
            }            

            TweenPosition.Begin(InfoBoardTweenPos.gameObject, 0.3f, m_infoBoardHidePos, m_infoBoardInitialPos);

            //NetServiceManager.Instance.InteractService.SendGetPlayerRanking();//获取个人排名
        }
        void ClosePVPPanel()
        {
            transform.localPosition = new Vector3(0, 0, -800);
            PVPInterface.SetActive(false);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.SwitchOffShowPlayerInfo, true);
            if (HeroView != null)
            {
                HeroView.CloseHeroModelView();
            }
        }

        private void InitHeroViewCamera()
        {
            StartCoroutine("AddCamera");
        }
        IEnumerator AddCamera()
        {
            //yield return new WaitForSeconds(0.1f);
            yield return new WaitForEndOfFrame();
            int Vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            if (HeroView == null)
            {
                GameObject heroViewCamera = (GameObject)Instantiate(HeroViewCameraPrefab);
                heroViewCamera.transform.parent = HeroViewCameraTrans;
                heroViewCamera.transform.localPosition = Vector3.zero;
                heroViewCamera.transform.localScale = Vector3.one;
                ContainerHeroView[] containerHeroViews = heroViewCamera.GetComponentsInChildren<ContainerHeroView>();
                int length = containerHeroViews.Length;
                if (length >= 2)
                {                    
                    HeroView = containerHeroViews[0];
                    CompetitorView = containerHeroViews[1];
                }
                
            }
            HeroView.ShowHeroModelView(Vocation);
            Label_myName.text = PlayerManager.Instance.FindHeroDataModel().Name;            
        }

        //private void InitRankListBoard()
        //{
        //    ItemPageManager_Rank.InitPager(mRankItemNum);
        //}


        private void FreezeButtons(bool isFreeze)
        {
            if (isFreeze)
            {
                ExitButton.SetButtonActive(false);
                ExitButton.SetButtonTextureColor(Color.gray);
                //ScoreTopButton.SetButtonActive(false);
                var scoreIBtn = ScoreTopButton.GetComponent<UIImageButton>();
                if (scoreIBtn != null)
                {
                    scoreIBtn.isEnabled = false;
                }
                //ScoreTopButton.SetButtonTextureColor(Color.gray);
                AddTimesButton.SetButtonActive(false);
            }
            else
            {
                ExitButton.SetButtonActive(true);
                ExitButton.SetButtonTextureColor(Color.white);
                //ScoreTopButton.SetButtonActive(true);
                var scoreIBtn = ScoreTopButton.GetComponent<UIImageButton>();
                if (scoreIBtn != null)
                {
                    scoreIBtn.isEnabled = true;
                }
                //ScoreTopButton.SetButtonTextureColor(Color.white);
                AddTimesButton.SetButtonActive(true);
            }
        }

        private void UpdatePrestigeInfo()
        {
            int prestigeLevel = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL;            
            var prestigeDataList = PlayerDataManager.Instance.GetPlayerPrestigeList();
            var prestigeData = prestigeDataList.SingleOrDefault(p=>p._pvpLevel == prestigeLevel);
            if(prestigeData != null)
            {                
                var nextLevelData = prestigeDataList.SingleOrDefault(p => p._pvpLevel == prestigeLevel+1);
                if (nextLevelData != null)
                {
                    int curPrestige = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_PRESTIGE;
                    if ((nextLevelData._pvpExp - prestigeData._pvpExp) != 0)
                    {
                        Slider_prestigeValue.sliderValue = (curPrestige - prestigeData._pvpExp) * 1f / (nextLevelData._pvpExp - prestigeData._pvpExp);
                    }
                    else
                    {
                        Slider_prestigeValue.sliderValue = 0;
                    }
                    Label_prestigeValue.text = curPrestige.ToString() + "/" + nextLevelData._pvpExp;
                }
                else
                {
                    Slider_prestigeValue.sliderValue = 1f;
                }                
                Label_TitleName.text = LanguageTextManager.GetString(prestigeData._titleName);
            }            
        }

        //退出
        void OnExitClick(object obj)
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            ClosePVPPanel();
        }
        //排行榜
        void OnScoreTopClick(object obj)
        {
            TopBoard.SetActive(true);
            PVPInterface.SetActive(false);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.SwitchOffShowPlayerInfo, true);
            HeroView.CloseHeroModelView();
            if (m_RoleViewPanel == null)
            {
                m_RoleViewPanel = ((GameObject)Instantiate(RoleViewPanelPrefab.gameObject)).GetComponent<RoleViewPanel>();
                Camera uiCamera = UICamera.currentCamera;
                m_RoleViewPanel.SetPanelPosition(uiCamera, RoleViewPoint);
                //更改为显示勋章界面 并赋值
                int prestigeLevel = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL;
                var prestigeData = PlayerDataManager.Instance.GetPlayerPrestigeList().SingleOrDefault(p => p._pvpLevel == prestigeLevel);
                if (prestigeData != null)
                {
                    var itemData = ItemDataManager.Instance.GetItemData(prestigeData._pvpInsignia);                    
                    if (itemData != null)
                    {
                        m_currentMedalData = (EquipmentData)itemData;

                        m_MedalAttributeDic = m_RoleViewPanel.ChangeInterface("JH_UI_Button_1124_01", "JH_UI_Button_1124_02", LanguageTextManager.GetString(m_currentMedalData._szGoodsName), ChangeMedalInterface);
                        ChangeMedalInterface();
                    }
                    else
                    {
                        TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"itemData is null");
                    }
                }
                else
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"prestigeData is null");
                }
            }
            else
            {
                m_RoleViewPanel.Show();
            }

            CategoryTabControl_Rank.ShowDefaultTop();
        }

        void ChangeMedalInterface()
        {
            if (m_MedalAttributeDic == null)
            {
                return;
            }
            var attributeList = m_MedalAttributeDic.Values.ToList();
            int length = attributeList.Count;

            int prestigeLevel = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL;
            var prestigeData = PlayerDataManager.Instance.GetPlayerPrestigeList().SingleOrDefault(p => p._pvpLevel == prestigeLevel);
            if (prestigeData != null)
            {
                var itemData = ItemDataManager.Instance.GetItemData(prestigeData._pvpInsignia);
                m_currentMedalData = (EquipmentData)itemData;

                m_RoleViewPanel.ChangeInterface(LanguageTextManager.GetString(m_currentMedalData._szGoodsName));

                string[] neweffects = m_currentMedalData._vectEffects.Split('|');
                List<string> effectNameList = new List<string>();
                List<string> effectValueList = new List<string>();
                for (int i = 0; i < neweffects.Length; i++)
                {
                    string[] anyOneEffects = neweffects[i].Split('+');
                    effectNameList.Add(anyOneEffects[0]);
                    effectValueList.Add(anyOneEffects[1]);
                    
                }
                int effectIndex = effectNameList.Count - 1;
                for (int k = 0; k < attributeList.Count; k++)
                {
                    if (k <= effectIndex)
                    {
                        var effectData = ItemDataManager.Instance.GetEffectData(effectNameList[k]);
                        if (effectData != null)
                        {
                            attributeList[k].ResetInfo(effectData.EffectRes, effectData.IDS, effectValueList[k]);
                        }
                        else
                        {
                            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"effectData not found");
                            attributeList[k].gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        attributeList[k].gameObject.SetActive(false);
                    }
                }                   
            }
        }

        //挑战按钮
        void OnChallengeClick(object obj)
        {
            if (m_IsFindPVPPlayer)
            {
                return;
            }
            //本地判断挑战次数是否足够
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            if (playerData.PlayerValues.PLAYER_FIELD_PVP_TIMES <= 0)
            {
                m_isChallenging = false;
                //this.RefreshInterface();
                MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_349"), 1f);
                return;
            }

            if (obj != null)
            {                
                m_isChallenging = false;                
            }
            else
            {                
                m_isChallenging = !m_isChallenging;
                NetServiceManager.Instance.EctypeService.SendEctypeChanllengePvp(m_isChallenging);                
            }
            
            if (m_isChallenging)
            {      
                TweenPosition.Begin(InfoBoardTweenPos.gameObject, 0.3f, m_infoBoardHidePos);
                //显示
                //ChallengeButton.ButtonBackground.enabled = false;
                ChallengeButtonEffect.SetActive(false);
                //ChallengeButton.ButtonText.text = LanguageTextManager.GetString("IDS_H1_350") + "\n  " + LanguageTextManager.GetString("IDS_H1_351");
                ChallengeLabel.SetActive(true);
                Label_SearchTime.text = "1";
                m_searchTime = 1;
                CancelInvoke("BeginSearch");
                InvokeRepeating("BeginSearch", 1f, 1f);
                PVPSearchEffect.SetActive(true);
            }
            else
            {                
                TweenPosition.Begin(InfoBoardTweenPos.gameObject, 0.3f, m_infoBoardInitialPos);
                //显示
                ChallengeButton.SetButtonActive(true);
                ChallengeButtonEffect.SetActive(true);
                //ChallengeButton.ButtonBackground.enabled = true;                
                //ChallengeButton.ButtonText.text = "";
                ChallengeLabel.SetActive(false);
                CancelInvoke("BeginSearch");
                PVPSearchEffect.SetActive(false);
            }            
            FreezeButtons(m_isChallenging);            
            //CompetitorView.ShowHeroModelView(2);//test
        }
        private void BeginSearch()
        {
            m_searchTime++;
            Label_SearchTime.text = m_searchTime.ToString();
        }
        //增加pvp挑战次数
        void OnAddTimesClick(object obj)
        {
            //string ShowStr = string.Format(LanguageTextManager.GetString("IDS_H1_158"), 100);
            //AddTimesMessagePanel.Show(string.Format("{0}\n{1}", LanguageTextManager.GetString("IDS_H1_348"), ShowStr));
            AddTimesMessagePanel.Show(LanguageTextManager.GetString("IDS_H1_348"));
        }
        //排行榜返回
        void OnBackClick(object obj)
        {
            PVPInterface.SetActive(true);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.SwitchOffShowPlayerInfo, false);
            TopBoard.SetActive(false);
            HeroView.ShowHeroModelView(m_VocationID);
            m_RoleViewPanel.Close();
        }       
        void GetRankingListHandle(INotifyArgs arg)
        {
//            //m_rankInfoCtrlList.ApplyAllItem(p =>
//            //{ Destroy(p.gameObject); });
//           
//            SMsgInteract_RankingList_SC rankingList = (SMsgInteract_RankingList_SC)arg;
//            //TraceUtil.Log("====>GetRankingListHandle rankingList.count" + rankingList.byRankingListNum);
//            if (m_rankInfoCtrlList.Count < rankingList.byRankingListNum)
//            {
//                int num = rankingList.byRankingListNum - m_rankInfoCtrlList.Count;
//                for (int i = 0; i < num; i++)
//                {
//                    GameObject rankInfo = (GameObject)Instantiate(RankingInfoControl.gameObject);
//                    rankInfo.transform.parent = ItemPageManager_Rank.transform;
//                    rankInfo.transform.localScale = Vector3.one;
//                    PVPRankInfoControl rc = rankInfo.GetComponent<PVPRankInfoControl>();
//                    m_rankInfoCtrlList.Add(rc);
//                }
//            }
//            //匹配到自己信息
//            string name = PlayerManager.Instance.FindHeroDataModel().Name;
//            for (int i = 0; i < rankingList.byRankingListNum; i++)
//            {
//                //m_rankInfoCtrlList[i].Set(rankingList.rankingDatas[i]);
//                if (name == rankingList.rankingDatas[i].szName)
//                {
//                    m_rankInfoCtrlList[i].SetMyInfoColor();
//                }
//            }
//            int mItemNum = Mathf.Clamp(rankingList.byRankingListNum, 1, 100);   //防止空值出错
//            ItemPageManager_Rank.InitPager(mItemNum, 1, 0);
//            //ItemPageManager_Rank.UpdateItems(m_rankInfoCtrlList.Take(5).ToArray(), "rank");
//            //else
//            //{
//            //    int num = m_rankInfoCtrlList.Count-rankingList.byRankingListNum;
//            //    for (int i = 0; i < m_rankInfoCtrlList.Count; i++)
//            //    {
//            //        if (i < num)
//            //        {
//            //            m_rankInfoCtrlList[i].Set(rankingList.rankingDatas[i]);
//            //        }
//            //        else
//            //        {
//            //            Destroy(m_rankInfoCtrlList[i].gameObject);
//            //        }                    
//            //    }
//            //}
//
//            //UIGrid_Top.Reposition();
//            //UIDraggablePanel_Top.ResetPosition();
            
        }

        void PVPFindPlayerHandle(INotifyArgs arg)
        {
            SMSGEctypeFindPlayer_SC findPackage = (SMSGEctypeFindPlayer_SC)arg;            

            if (findPackage.byFindNum <= 0)
            {
                MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_352"), LanguageTextManager.GetString("IDS_H2_55"), RefreshInterface);   
                return;
            }

            m_IsFindPVPPlayer = true;

            var playerInfo = findPackage.EctypePvpPlayer;
            PVPBattleManager.Instance.SavePVPPlayerData(playerInfo);//储存pvp玩家信息

            CompetitorView.ShowHeroModelView(playerInfo.byKind);
            //Label_competitorName.gameObject.SetActive(true);
            Name_competitor.SetActive(true);
            Label_competitorName.text = findPackage.EctypePvpPlayer.szName;
            //显示
            ChallengeButton.SetButtonActive(false);
            ChallengeButtonEffect.SetActive(false);
            InfoBoardTweenPos.gameObject.SetActive(false);//强制不显示信息面板

            //ChallengeButton.ButtonText.text = LanguageTextManager.GetString("IDS_H1_353");
            ChallengeFoundTip.SetActive(true);
            ChallengeLabel.SetActive(false);
            CancelInvoke("BeginSearch");
            PVPSearchEffect.SetActive(false);
            StartCoroutine("ReadyEnter");
        }
        //刷新pvp界面
        private void RefreshInterface()
        {            
            this.ShowPVPPanel();
            CompetitorView.DeleteHeroModeView();
            //Label_competitorName.gameObject.SetActive(false);
            ChallengeFoundTip.SetActive(false);//对手找到提示
            Name_competitor.SetActive(false);//对手名字
            this.OnChallengeClick(0);   //这里要赋参数
            StopCoroutine("ReadyEnter");
            GO_DialogBoard.SetActive(false);
        }
        IEnumerator ReadyEnter()
        {
            yield return new WaitForSeconds(2f);
            GO_DialogBoard.SetActive(true);
            //随机挑衅语言
            var language = PlayerDataManager.Instance.GetPlayerPrestigeList()[0]._ProvocationWord;
            int maxNum = language.Length;
            Label_MyDialog.text = LanguageTextManager.GetString(language[UnityEngine.Random.Range(0, maxNum)]);
            Label_OtherDialog.text = LanguageTextManager.GetString(language[UnityEngine.Random.Range(0, maxNum)]);

            //动作
            //我方玩家
            byte kind = (byte)PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            var playerGenerate = PlayerDataManager.Instance.GetUIItemData(kind);
            if (playerGenerate != null)
            {                
                HeroView.PlayRandomAnim(playerGenerate.PVP_Ready);
            }
            //敌方
            var competitorData = PVPBattleManager.Instance.GetPVPPlayerData();            
            var competitorGenerate = PlayerDataManager.Instance.GetUIItemData(competitorData.byKind);
            if (competitorGenerate != null)
            {
                CompetitorView.PlayRandomAnim(competitorGenerate.PVP_Ready);                
            }
            yield return new WaitForSeconds(2f);
            NetServiceManager.Instance.EctypeService.SendEctypePvpActionDone();
        }
        //清理排行榜数据
        void ClearRankingListHandle(INotifyArgs arg)
        {
            m_rankInfoCtrlList.ApplyAllItem(p => { Destroy(p.gameObject); });            
            m_rankInfoCtrlList.Clear();

            ItemPageManager_Rank.InitPager(1, 1, 0);//重置默认值

        }
        void EntityUpdateValuesHandle(INotifyArgs arg)
        {
            UpdatePrestigeInfo();
            PVPInterfaceInfoPanel.ReadPlayerInfo();
        }
        //重置pvp界面
        void PVPRematchingHandle(INotifyArgs arg)
        {            
            this.RefreshInterface();
            PVPBattleManager.Instance.ClearPVPPlayerData();            
            MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_354"), 1f);
        }
        void PVPNoTimesHandle(INotifyArgs arg)
        {
            this.RefreshInterface();
            MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_349"), 1f);
        }
        void ItemPageChangedHandle(PageChangedEventArg pageChangedEventArg)
        {                        
            m_rankInfoCtrlList.ApplyAllItem(p =>
                {
                    p.transform.position = new Vector3(-2000, 0, 0);
                });
            int size = ItemPageManager_Rank.PagerSize;
            ItemPageManager_Rank.UpdateItems(m_rankInfoCtrlList.Skip((pageChangedEventArg.StartPage - 1) * size).Take(size).ToArray(), "rank");
        }
        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.GetRankingList.ToString(), GetRankingListHandle);
            //AddEventHandler(EventTypeEnum.GetPlayerRanking.ToString(), GetPlayerRankingHandle);
            AddEventHandler(EventTypeEnum.PVPFindPlayer.ToString(), PVPFindPlayerHandle);
            AddEventHandler(EventTypeEnum.ClearRankingList.ToString(), ClearRankingListHandle);
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), EntityUpdateValuesHandle);
            AddEventHandler(EventTypeEnum.PVPRematching.ToString(), PVPRematchingHandle);
            AddEventHandler(EventTypeEnum.PVPNoTimes.ToString(), PVPNoTimesHandle);
            ItemPageManager_Rank.OnPageChanged += this.ItemPageChangedHandle;            
        }       
    }
}

