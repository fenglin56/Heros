using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UI.Friend
{
    public class FriendListPanel : MonoBehaviour
    {
        //public Transform Grid;
        //public GameObject FriendPanelPrefab;//单个好友面板的prefab

        //private List<PanelElementDataModel> m_panelElementList = new List<PanelElementDataModel>();

        public List<FriendElement_V2> MyFriendPanelList;
        public SingleButtonCallBack LastPageBtn;
        public SingleButtonCallBack NextPageBtn;
        public UILabel PageLabel;
        public UILabel RoleNumber;

        public GameObject UIBottomBtnPrefab;
        public Transform CreatBottomBtnPoint;
        private UI.MainUI.CommonUIBottomButtonTool commonUIBottomButtonTool;

        public SocialUIManager MyParent { get; private set; }

        private int CurrentPage = 1;
        private int[] m_guideBtnID;

        void Awake()
        {
            m_guideBtnID = new int[4];
            //TODO GuideBtnManager.Instance.RegGuideButton(LastPageBtn.gameObject, MainUI.UIType.SocialInfo, SubType.FriendPage, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(NextPageBtn.gameObject, MainUI.UIType.SocialInfo, SubType.FriendPage, out m_guideBtnID[1]);
            LastPageBtn.SetCallBackFuntion(OnLastPageBtnClick);
            NextPageBtn.SetCallBackFuntion(OnNextPageBtnClick);
        }

        void Start()
        {
                       
            FriendDataManager.Instance.GetFriendListData.ApplyAllItem(P =>
                {
                    InitFriendUI(P);
                });
			FriendDataManager.Instance.SortFriendList();
            ResetPageInfo();
        }

        public void Init(SocialUIManager MyParent)
        {
            this.MyParent = MyParent;
        }

//        void FixedUpdate()
//        {
//            {
//                if (FriendDataManager.Instance.IsCreateFriendUI)
//                {               
//                    while (FriendDataManager.Instance.FriendUIQueueList.Count > 0)
//                    {
//                        int length = FriendDataManager.Instance.FriendUIQueueList.Count;
//                        CreateUIListPanel(FriendDataManager.Instance.FriendUIQueueList[length - 1]);
//                        FriendDataManager.Instance.FriendUIQueueList.RemoveAt(length - 1);
//                    }
//
//                    FriendDataManager.Instance.IsCreateFriendUI = false;
//                    FriendDataManager.Instance.SortFriendList();
//
//                    if (FriendDataManager.Instance.CurPanelState == PanelState.MYFRIENDLIST)
//                        FriendDataManager.Instance.IsUpdateFriendList = true;
//                }
//
//                if (FriendDataManager.Instance.IsUpdateFriendList)
//                {
//                    FriendDataManager.Instance.SetFriendListDepth(FriendUIConst.UI_SHOW_ZDEPTH);
//                    ResetPageInfo();
//                    FriendDataManager.Instance.IsUpdateFriendList = false;
//                }
//            }
//        }

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
            FriendDataManager.Instance.GetFriendListData.ApplyAllItem(P=>MaxItemID= P.PositionID>MaxItemID?P.PositionID:MaxItemID);
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
            //TraceUtil.Log("刷新好友面板，好友个数："+FriendDataManager.Instance.GetFriendListData.Count);
            for (int i = 0; i < MyFriendPanelList.Count; i++)
            {
                PanelElementDataModel friendData = FriendDataManager.Instance.GetFriendListData.FirstOrDefault(P => P.PositionID == (CurrentPage - 1) * 4 + i);
                if (friendData == null)
                {
                    friendData = FriendDataManager.Instance.GetRequestListData.FirstOrDefault(P => P.PositionID == CurrentPage * i); 
                }
                if (friendData == null)
                {
                    //MyFriendPanelList[i].gameObject.SetActive(false);
                    MyFriendPanelList[i].SetElementActive(false);
                }
                else
                {
                    //MyFriendPanelList[i].gameObject.SetActive(true);
                    MyFriendPanelList[i].SetElementActive(true);
                    MyFriendPanelList[i].SetAttribute(friendData);
                }
            }

            Color enabelColor = new Color(1, 1, 1, 1);
            Color disabelColor = new Color(1, 1, 1, 0.3f);
            LastPageBtn.BackgroundSprite.color = (CurrentPage>1?enabelColor:disabelColor);
            int MaxItemID = 1;
            FriendDataManager.Instance.GetFriendListData.ApplyAllItem(P=>MaxItemID= P.PositionID>MaxItemID?P.PositionID:MaxItemID);
            MaxItemID++;
            NextPageBtn.BackgroundSprite.color = (MaxItemID / 4 + (MaxItemID % 4 > 0 ? 1 : 0)) > CurrentPage ? enabelColor : disabelColor;
            PageLabel.SetText(string.Format("{0}/{1}", CurrentPage, MaxItemID / 4 + (MaxItemID % 4 > 0 ? 1 : 0)));
            RoleNumber.SetText(string.Format("{0}/{1}", FriendDataManager.Instance.GetFriendListData.Count, 30));
        }

        public void ShowFriendList()
        {
            CurrentPage = 1;
            transform.localPosition = Vector3.zero;
            FriendDataManager.Instance.IsUpdateFriendList = true;
            if (commonUIBottomButtonTool == null)
            {
                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<UI.MainUI.CommonUIBottomButtonTool>();
                ShowBottomBtn();
            }
            else
            {
                commonUIBottomButtonTool.ShowAnim();
            }
        }

        void ShowBottomBtn()
        {
            UI.MainUI.CommonBtnInfo btnInfo = new UI.MainUI.CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonTapped);
            UI.MainUI.CommonBtnInfo btnInfo1 = new UI.MainUI.CommonBtnInfo(1, "JH_UI_Button_1116_14", "JH_UI_Button_1116_00", MyParent.ShowNearlyPlayerPanel);
            commonUIBottomButtonTool.Show(new List<UI.MainUI.CommonBtnInfo>() { btnInfo, btnInfo1});
            var btnInfoComponet = commonUIBottomButtonTool.GetButtonComponent(btnInfo);
            //if (btnInfoComponet.gameObject != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfoComponet.gameObject, MainUI.UIType.SocialInfo, SubType.ButtomCommon, out m_guideBtnID[2]);
            var btnInfo1Componet = commonUIBottomButtonTool.GetButtonComponent(btnInfo1);
            //if (btnInfo1Componet.gameObject != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfo1Componet.gameObject, MainUI.UIType.SocialInfo, SubType.ButtomCommon, out m_guideBtnID[3]);
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        private void InitFriendUI(PanelElementDataModel element)
        {
            //GameObject friendPanel = CreatObjectToNGUI.InstantiateObj(FriendPanelPrefab, Grid);
            //FriendElement friendBehaviour = friendPanel.GetComponent<FriendElement>();
            //friendBehaviour.SetAttribute(element);

            //FriendDataManager.Instance.GetFriendListData.Find(P => P.sMsgRecvAnswerFriends_SC.dwFriendID == 
            //    element.sMsgRecvAnswerFriends_SC.dwFriendID).BtnObj = friendPanel;
        }

        private void CreateUIListPanel(PanelElementDataModel element)
        {
            if (element.BtnType == ButtonType.FriendList)
            {
                if (FriendDataManager.Instance.GetFriendListData.Exists(P => P.sMsgRecvAnswerFriends_SC.dwFriendID == element.sMsgRecvAnswerFriends_SC.dwFriendID))
                    return;
            }

            if (element.BtnType == ButtonType.AddFriend)
            {
                if (FriendDataManager.Instance.GetRequestListData.Exists(P => P.sMsgRecvAnswerFriends_SC.dwFriendID == element.sMsgRecvAnswerFriends_SC.dwFriendID))
                    return;
            }

            //GameObject friendPanel = CreatObjectToNGUI.InstantiateObj(FriendPanelPrefab, Grid);
            //FriendElement friendBehaviour = friendPanel.GetComponent<FriendElement>();
            //friendBehaviour.SetAttribute(element);

            //element.BtnObj = friendPanel;

            if (element.BtnType == ButtonType.FriendList)
            {
                FriendDataManager.Instance.RegFriendData(element);
            }
            else
            {
                ///当申请好友数量大于或等于最大值时，删除最先申请的
                if (FriendDataManager.Instance.GetRequestListData.Count >= 15)
                {
                    //Destroy(FriendDataManager.Instance.GetAddFriendData[0].BtnObj);
                    FriendDataManager.Instance.GetRequestListData.RemoveAt(0);
                }

                //friendBehaviour.IsShowEmailIcon(true);
                FriendDataManager.Instance.IsUpdateMsgNum = true;
                FriendDataManager.Instance.RegRequestData(element);
            }
        }

        public void CloseFriendPanel()
        {
            //FriendDataManager.Instance.SetFriendListDepth(FriendUIConst.UI_NO_SHOW_ZDEPTH);
            transform.localPosition = new Vector3(0,0,-1000);
        }

        private void OnBackButtonTapped(object obj)
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            CloseParentPanel(obj);
        }

        void CloseParentPanel(object obj)
        {
            MyParent.OnBackBtnClick(null);
        }
    }

    /// <summary>
    /// 好友排序实现类，根据是否在线
    /// </summary>
    public class FriendComparer : IComparer<PanelElementDataModel>
    {
        public int Compare(PanelElementDataModel x, PanelElementDataModel y)
        {
            int compareResult = 0;

            if (IsFriendData(x))
            {
                if (IsFriendData(y))
                {
                    compareResult = CompareFriendOnline(x, y);
                }
                else
                {
                    compareResult = -1;
                }
            }
            else if (IsFriendData(y))
            {
                compareResult = 1;
            }

            return compareResult;
        }
        private int CompareFriendOnline(PanelElementDataModel x, PanelElementDataModel y)
        {
            int onlineCompare = 0;
//            if (x.sMsgRecvAnswerFriends_SC.bOnLine != y.sMsgRecvAnswerFriends_SC.bOnLine)
//            {
//				if(x.sMsgRecvAnswerFriends_SC.bOnLine > y.sMsgRecvAnswerFriends_SC.bOnLine)
//				{
//					onlineCompare = -1;
//				}else if(x.sMsgRecvAnswerFriends_SC.bOnLine < y.sMsgRecvAnswerFriends_SC.bOnLine)
//				{
//					onlineCompare = 1;
//				}else onlineCompare = 0;
                
//            }
			if(x.sMsgRecvAnswerFriends_SC.bOnLine != y.sMsgRecvAnswerFriends_SC.bOnLine)
			{
				onlineCompare = x.sMsgRecvAnswerFriends_SC.bOnLine < y.sMsgRecvAnswerFriends_SC.bOnLine
					? 1 : -1;
			}

//			if(x.sMsgRecvAnswerFriends_SC.bOnLine != y.sMsgRecvAnswerFriends_SC.bOnLine)
//			{
//				onlineCompare = x.sMsgRecvAnswerFriends_SC.bOnLine > y.sMsgRecvAnswerFriends_SC.bOnLine
//					? -1 : 1;
//			}
//			else
//			{
//				onlineCompare = x.sMsgRecvAnswerFriends_SC.dwFriendID >= y.sMsgRecvAnswerFriends_SC.dwFriendID
//					? -1 : 1;
//			}

            return onlineCompare;
        }

        public static bool IsFriendData(PanelElementDataModel friendInfo)
        {
            long friendId = friendInfo.sMsgRecvAnswerFriends_SC.dwFriendID;
            bool isFriendData = FriendDataManager.Instance.GetFriendListData.Exists(item => item.sMsgRecvAnswerFriends_SC.dwFriendID == friendId);
            return isFriendData;
        }
    }

}


