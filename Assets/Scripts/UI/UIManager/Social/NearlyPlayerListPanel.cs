using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Friend
{
    public class NearlyPlayerListPanel :View
    {
        //public Transform Grid;
        //public GameObject NearlyPanelPrefab;//单个好友面板的prefab
        //public LocalButtonCallBack AddFriendBtn;

        public List<NearlyPlayerElement_v2> MyFriendPanelList;
        public SingleButtonCallBack LastPageBtn;
        public SingleButtonCallBack NextPageBtn;
        public UILabel PageLabel;
        public UILabel RoleNumber;

        public GameObject UIBottomBtnPrefab;
        public Transform CreatBottomBtnPoint;
        private UI.MainUI.CommonUIBottomButtonTool commonUIBottomButtonTool;

        private List<NearlyItem> m_panelElementList = new List<NearlyItem>();

        public SocialUIManager MyParent { get; private set; }

        SMsgGetActorListHead NearlySMsgGetActorListHead;

        private int CurrentPage = 1;
        private int[] m_guideBtnID;

        class NearlyItem
        {
            public int Index;
            public uint dwFriendID; 
            public bool m_isFriend;
            public PanelElementDataModel element;
        }

        void Awake()
        {
            this.RegisterEventHandler();
            m_guideBtnID = new int[4];
            //TODO GuideBtnManager.Instance.RegGuideButton(LastPageBtn.gameObject, MainUI.UIType.SocialInfo, SubType.FriendPage, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(NextPageBtn.gameObject, MainUI.UIType.SocialInfo, SubType.FriendPage, out m_guideBtnID[1]);
            NextPageBtn.SetCallBackFuntion(OnNextPageBtnClick);
            LastPageBtn.SetCallBackFuntion(OnLastPageBtnClick);
        }


        public void Init(SocialUIManager MyParent)
        {
            this.MyParent = MyParent;
        }

        void ShowBottomBtn()
        {
            UI.MainUI.CommonBtnInfo btnInfo = new UI.MainUI.CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", MyParent.OnBackButtonTapped);
            UI.MainUI.CommonBtnInfo btnInfo1 = new UI.MainUI.CommonBtnInfo(1, "JH_UI_Button_1116_15", "JH_UI_Button_1116_00", MyParent.ShowFriendListPanel);
            commonUIBottomButtonTool.Show(new List<UI.MainUI.CommonBtnInfo>() { btnInfo, btnInfo1});
            var btnInfoComponet = commonUIBottomButtonTool.GetButtonComponent(btnInfo);
            //if (btnInfoComponet != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfoComponet.gameObject, MainUI.UIType.SocialInfo, SubType.ButtomCommon, out m_guideBtnID[2]);
            var btnInfo1Componet = commonUIBottomButtonTool.GetButtonComponent(btnInfo1);
            //if (btnInfo1Componet != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfo1Componet.gameObject, MainUI.UIType.SocialInfo, SubType.ButtomCommon, out m_guideBtnID[3]);
        }

        void OnLastPageBtnClick(object obj)
        {
            if (CurrentPage > 1)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
                CurrentPage--;
                ResetPageInfo();
            }
        }

        void OnNextPageBtnClick(object obj)
        {
            int MaxItemID = 0;
            m_panelElementList.ApplyAllItem(P => MaxItemID = P.Index > MaxItemID ? P.Index : MaxItemID);
            MaxItemID++;
            if ((MaxItemID / 4 + (MaxItemID % 4 > 0 ? 1 : 0)) > CurrentPage)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
                CurrentPage++;
                ResetPageInfo();
            }
        }

        void ResetPageInfo()
        {
            //TraceUtil.Log("刷新附近人数面板，附近人数：" + m_panelElementList.Count);
            for (int i = 0; i < MyFriendPanelList.Count; i++)
            {
                NearlyItem friendData = m_panelElementList.FirstOrDefault(P => P.Index == (CurrentPage-1)*4+i);
                if (friendData== null)
                {
                    //MyFriendPanelList[i].gameObject.SetActive(false);
                    MyFriendPanelList[i].SetElementActive(false);
                }
                else
                {
                    //MyFriendPanelList[i].gameObject.SetActive(true);
                    MyFriendPanelList[i].SetElementActive(true);
                    MyFriendPanelList[i].SetAttribute(friendData.element,friendData.m_isFriend);
                    //TraceUtil.Log("刷新附近人物信息："+friendData.element.sMsgRecvAnswerFriends_SC.Name);
                }
            }

            Color enabelColor = new Color(1, 1, 1, 1);
            Color disabelColor = new Color(1, 1, 1, 0.3f);
            LastPageBtn.BackgroundSprite.color = (CurrentPage > 1 ? enabelColor : disabelColor);
            int MaxItemID = 1;
            m_panelElementList.ApplyAllItem(P => MaxItemID = P.Index > MaxItemID ? P.Index : MaxItemID);
            MaxItemID++;
            NextPageBtn.BackgroundSprite.color = (MaxItemID / 4 + (MaxItemID % 4 > 0 ? 1 : 0)) > CurrentPage ? enabelColor : disabelColor;
            PageLabel.SetText(string.Format("{0}/{1}", CurrentPage, MaxItemID / 4 + (MaxItemID % 4 > 0 ? 1 : 0)));
            RoleNumber.SetText(string.Format("{0}/{1}", m_panelElementList.Count, 30));
        }


        private void CreateUIListPanel(PanelElementDataModel element, int index)
        {

            if (m_panelElementList.Exists(P => P.dwFriendID == element.sMsgRecvAnswerFriends_SC.dwFriendID))
                return;

            NearlyItem nearlyItem = new NearlyItem() ;
            nearlyItem.Index = index;
            nearlyItem.dwFriendID = element.sMsgRecvAnswerFriends_SC.dwFriendID;
            nearlyItem.element = element;
            nearlyItem.m_isFriend = FriendDataManager.Instance.GetFriendListData.Exists(P => P.sMsgRecvAnswerFriends_SC.dwFriendID == element.sMsgRecvAnswerFriends_SC.dwFriendID);
            m_panelElementList.Add(nearlyItem);
        }

        //private void CreateUIListPanel(PanelElementDataModel element, int index)
        //{
        //    LocalButtonCallBack addFriendBtn;

        //    if (m_panelElementList.Exists(P => P.dwFriendID == element.sMsgRecvAnswerFriends_SC.dwFriendID))
        //        return;

        //    NearlyItem nearlyItem;
        //    //nearlyItem.ItemPanel = CreatObjectToNGUI.InstantiateObj(NearlyPanelPrefab, Grid);
        //    nearlyItem.dwFriendID = element.sMsgRecvAnswerFriends_SC.dwFriendID;

        //    NearlyPlayerElement friendBehaviour = nearlyItem.ItemPanel.GetComponent<NearlyPlayerElement>();

        //    if (FriendDataManager.Instance.GetFriendListData.Exists(P => P.sMsgRecvAnswerFriends_SC.dwFriendID == element.sMsgRecvAnswerFriends_SC.dwFriendID))
        //    {
        //        m_isFriend = true;
        //    }

        //    if (!m_isFriend)
        //    {
        //        addFriendBtn = (LocalButtonCallBack)Instantiate(AddFriendBtn);
        //        addFriendBtn.transform.parent = nearlyItem.ItemPanel.transform;
        //        addFriendBtn.transform.localScale = Vector3.one;
        //        addFriendBtn.transform.localPosition = new Vector3(250, 0, 0);
        //        friendBehaviour.SetAttribute(element, addFriendBtn);
        //    }
        //    else
        //    {
        //        friendBehaviour.SetAttribute(element);
        //        m_isFriend = false;
        //    }

        //    nearlyItem.ItemPanel.transform.localPosition = new Vector3(0, 150 - 150 * index, 0);
        //    m_panelElementList.Add(nearlyItem);

        //}

        public void CloseNearlyPlayerPanel()
        {
            transform.localPosition = new Vector3(0,0,-1000) ;
            m_panelElementList.Clear();
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.RevNearlyPlayer.ToString(), ShowNearlyPlayerHandle);
        }

        public void Show()
        {
            if (LoadingUI.Instance != null)
            {
                LoadingUI.Instance.Show();
            }            
            NetServiceManager.Instance.FriendService.SendNearbyPlayerRequst((uint)PlayerManager.Instance.FindHeroDataModel().ActorID);
            StartCoroutine(ShowPanelForTime(1));
        }

        IEnumerator ShowPanelForTime(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            ShowNearlyPlayerHandle(NearlySMsgGetActorListHead);
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="notifyArgs"></param>
        private void ShowNearlyPlayerHandle(INotifyArgs notifyArgs)
        {
            StopAllCoroutines();
            if (LoadingUI.Instance != null)
            {
                LoadingUI.Instance.Close();
            }            
            NearlySMsgGetActorListHead = (SMsgGetActorListHead)notifyArgs;
            m_panelElementList.Clear();
            for (int i = 0; i < NearlySMsgGetActorListHead.dwFriendNum; i++)
            {
                PanelElementDataModel playerElementData = new PanelElementDataModel();
                playerElementData.sMsgRecvAnswerFriends_SC.dwFriendID = NearlySMsgGetActorListHead.sMsgRecvAnswerFriends_SC[i].dwFriendID;
                playerElementData.sMsgRecvAnswerFriends_SC.szName = NearlySMsgGetActorListHead.sMsgRecvAnswerFriends_SC[i].szName;
                playerElementData.sMsgRecvAnswerFriends_SC.sActorLevel = NearlySMsgGetActorListHead.sMsgRecvAnswerFriends_SC[i].sActorLevel;
                playerElementData.sMsgRecvAnswerFriends_SC.bOnLine = NearlySMsgGetActorListHead.sMsgRecvAnswerFriends_SC[i].bOnLine;
                playerElementData.sMsgRecvAnswerFriends_SC.dProfession = NearlySMsgGetActorListHead.sMsgRecvAnswerFriends_SC[i].dProfession;
                playerElementData.BtnType = ButtonType.NearlyPlayer;
                //TraceUtil.Log("附近玩家列表=====>>>>>.dProfession" + sMsgGetActorListHead.sMsgRecvAnswerFriends_SC[i].dProfession);
                if (!m_panelElementList.Exists(P => P.dwFriendID == playerElementData.sMsgRecvAnswerFriends_SC.dwFriendID))
                {

                    NearlyItem nearlyItem = new NearlyItem();
                    nearlyItem.Index = i;
                    nearlyItem.dwFriendID = playerElementData.sMsgRecvAnswerFriends_SC.dwFriendID;
                    nearlyItem.element = playerElementData;
                    nearlyItem.m_isFriend = FriendDataManager.Instance.GetFriendListData.Exists(P => P.sMsgRecvAnswerFriends_SC.dwFriendID == playerElementData.sMsgRecvAnswerFriends_SC.dwFriendID);
                    m_panelElementList.Add(nearlyItem);
                }
            }
            CurrentPage = 1;
            ResetPageInfo();
            if (commonUIBottomButtonTool == null)
            {
                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<UI.MainUI.CommonUIBottomButtonTool>();
                ShowBottomBtn();
            }
            else
            {
                commonUIBottomButtonTool.ShowAnim();
            }
            transform.localPosition = Vector3.zero;
        }
    }
}
