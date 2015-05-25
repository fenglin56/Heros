using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class TreasureTreesPanelManager : BaseUIPanel
    {
        public List<Transform> TreasureTreeFruitAnchors;
        public GameObject TreasureTreesFruitPointPrefab;
        public List<TreasureTreesFruitPoint> TreasureTreesFruitPointList;//八个果实挂载点
        public TreasureTreesDataBase TreasureTreesDataBase;//果实配表
        public TreasureTreesButtonToolManager treasureTreesButtonToolManager;// 果实操作按钮组
        public GetAwardAnimManager GetAwardAnimManager;//奖励动画控制器
        public FruitLogMessageWindow FruitLogMessageWindow;//操作果实信息打印
        public UILabel TimeLeftLabel;
        public GameObject NextGrowUpTip;
        public GameObject AllGrowUpTip;

        public SingleButtonCallBack BackButton;
        //common title
        public CommonPanelTitle m_commonTitle;

        //public GameObject CostMoneyMessagePrefab;
        //public GameObject UIBottomBtnPrefab;
        public SingleButtonCallBack LogPanelBtn;
        public GameObject NewMessageTip;
       

        //public GameObject CostMoneyMessageboxPrefab;
        //private CostMoneyMessageBox M_CostMoneyMessageBox;

        //private CostMoneyMessageBox costMoneyMessageBox;

        //public Transform CreatBottomBtnPoint;
        //private CommonUIBottomButtonTool commonUIBottomButtonTool;

        private int[] m_guideBtnID = new int[3];

        //public GameObject CommonToolPrefab;
        void Awake()
        {
            TreasureTreesFruitPointList = new List<TreasureTreesFruitPoint>();
            for(int i = 0; i < TreasureTreeFruitAnchors.Count; i++)
            {
                GameObject obj = UI.CreatObjectToNGUI.InstantiateObj(TreasureTreesFruitPointPrefab, TreasureTreeFruitAnchors[i]);
                TreasureTreesFruitPoint point =obj.GetComponent<TreasureTreesFruitPoint>();
                TreasureTreesFruitPointList.Add(point);
            }


            //if (UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform); }
            for (int i = 0; i < TreasureTreesFruitPointList.Count; i++)
            {

                TreasureTreesFruitPointList[i].Init(this,i+1);
            }
            treasureTreesButtonToolManager.Init(this);
            GetAwardAnimManager.Init(this);
            FruitLogMessageWindow.Init(this);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.UpdateTreasureTreesData, UpdatePanel);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.TreasureTreesUseMana, OnUseManaFromServer);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.TreasureTreesGetReward, OnGetRewardFromServer);


            //MainUIController.Instance.SetPanelActivEvent += new MainUIController.SetPanelDelegate(SetPanelActive);
            InvokeRepeating("ShwoFruitLeftTime",0,1);
            LogPanelBtn.SetCallBackFuntion(FruitLogMessageWindow.ShowPanel);
            SetNewTipShow(false);
            BackButton.SetCallBackFuntion(OnBackButtonTapped);

            //TODO GuideBtnManager.Instance.RegGuideButton(this.LogPanelBtn.gameObject, UIType.Treasure, SubType.TreasureTreesMainButton, out m_guideBtnID[0]);
            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            LogPanelBtn.gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Message);
            m_commonTitle.GoldMoneyLabel.gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_BuyIngot);
            m_commonTitle.CopperLabel.gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_BuyMoney);
            BackButton.gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Back);
        }      

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UpdateTreasureTreesData, UpdatePanel);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TreasureTreesUseMana, OnUseManaFromServer);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TreasureTreesGetReward, OnGetRewardFromServer);
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        //public void SetPanelActive(int[] UIStatus)
        //{
        //    int Vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
        //    switch ((UIType)UIStatus[0])
        //    {
        //        case UIType.TreasureTrees:
        //            Show();
        //            break;
        //        default:
        //            Close(null);
        //            break;
        //    }
        //}

        public void SetNewTipShow(bool show)
        {
            NewMessageTip.SetActive(show);
        }

        public override void Show(params object[] value)
        {
            //SoundManager.Instance.StopBGM();
            //SoundManager.Instance.PlayBGM("Music_UIBG_Farm", 0.0f);
            UpdatePanel(null);
            m_commonTitle.TweenShow();
//            if (commonUIBottomButtonTool == null)
//            {
//                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
//                ShowBottomBtn();
//            }
//            else
//            {
//                commonUIBottomButtonTool.ShowAnim();
//            }
            base.Show(value);
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            //GameManager.Instance.PlaySceneMusic();
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            m_commonTitle.tweenClose();
            base.Close();
        }

        /// <summary>
        /// 给果实浇水
        /// </summary>
        /// <param name="fruitPosition"></param>
        public void WateringFruit(byte fruitPosition)
        {
            TraceUtil.Log("给果实浇水");
            StartCoroutine(TreasureTreesFruitPointList.First(P => P.MyFruitData.byFruitPosition == fruitPosition).WateringFruit());
            StartCoroutine(SendWateringToSeverForTime(1f,fruitPosition));
        }

        IEnumerator SendWateringToSeverForTime(float waitTime,byte fruitPosition)
        {
            yield return new WaitForSeconds(waitTime);
            NetServiceManager.Instance.EntityService.SendWateringFruitMsgToServer(fruitPosition);
        }

        /// <summary>
        /// 给所有果实浇水
        /// </summary>
        public void WateringAllFruit()
        {
            TraceUtil.Log("给所有果实浇水");
            var IsDryFruitList = TreasureTreesFruitPointList.FindAll(P => P.MyFruitData.byFruitDryStatus == 1 && (FruitPrucStatusType)P.MyFruitData.byFruitStatus!= FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE);
            IsDryFruitList.ApplyAllItem(P=>P.StartCoroutine(P.WateringFruit()));
            StartCoroutine(SendWateringAllToSeverForTime(1f));
        }

        IEnumerator SendWateringAllToSeverForTime(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            NetServiceManager.Instance.EntityService.SendWateringAllFruitMsgToServer();
        }
        /// <summary>
        /// 摘取果实
        /// </summary>
        /// <param name="fruitPosition"></param>
        public void PickUpFruit(byte fruitPosition)
        {
            TraceUtil.Log("摘取果实");
            //GetAwardAnimManager.PickUpFruit(fruitPosition);
            StartCoroutine(TreasureTreesFruitPointList.First(P => P.MyFruitData.byFruitPosition == fruitPosition).PickUpFruit());
            NetServiceManager.Instance.EntityService.SendGetFruitMsgToServer(fruitPosition);
        }
        /// <summary>
        /// 摘取所有果实
        /// </summary>
        public void PickUpAllFruit()
        {
            TraceUtil.Log("摘取所有果实");
            foreach (var child in TreasureTreesFruitPointList)
            {
                if (child.MyFruitData.byFruitStatus == (byte)FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE)
                {
                    StartCoroutine(child.PickUpFruit());
                    //GetAwardAnimManager.PickUpFruit((byte)child.MyPositionID);
                }
            }
            NetServiceManager.Instance.EntityService.SendGetAllFruitMsgToServer();
        }

        /// <summary>
        /// 解锁果实位置
        /// </summary>
        /// <param name="fruitPosition"></param>
        public void SendUnlockFruitPointToSever(byte fruitPosition)
        {
            TraceUtil.Log("解锁果实位置:"+fruitPosition);
            StartCoroutine(TreasureTreesFruitPointList.First(P => P.MyPositionID == fruitPosition).UnLockFruitPoint());
            NetServiceManager.Instance.EntityService.SendUnlockFruitMsgToServer(fruitPosition);
        }
        /// <summary>
        /// 使用甘露
        /// </summary>
        /// <param name="fruitPosition"></param>
        public void SendUseAmritaToSever(byte fruitPosition)
        {
            TraceUtil.Log("使用甘露");
            NetServiceManager.Instance.EntityService.SendUserManaMsgToServer(fruitPosition);
            StartCoroutine(TreasureTreesFruitPointList.First(P => P.MyFruitData.byFruitPosition == fruitPosition).UsingAmrita());
        }

        /// <summary>
        /// 购买仙露
        /// </summary>
        public void SendBuyAmritaToSever(byte BuyNumber)
        {
            TraceUtil.Log("购买仙露");
            NetServiceManager.Instance.TradeService.SendTradeQuickBuyGoods(50000003, (uint)BuyNumber);
        }

        //public void ShowCostMoneyMessageBox(CostMoneyType CostMoneyType, int CostMoneyNumber, string Msg, string SureBtnStr, string CancelBtnStr, ButtonCallBack SureBtnCallBack, ButtonCallBack CancelBtnCallBack)
        //{
        //    if (costMoneyMessageBox == null)
        //    {
        //        costMoneyMessageBox = CreatObjectToNGUI.InstantiateObj(CostMoneyMessagePrefab,transform).GetComponent<CostMoneyMessageBox>();
        //    }
        //    costMoneyMessageBox.Show(CostMoneyType, CostMoneyNumber, Msg, SureBtnStr, CancelBtnStr, SureBtnCallBack, CancelBtnCallBack);
        //}

        /// <summary>
        /// 显示果实成熟时间
        /// </summary>
        void ShwoFruitLeftTime()
        {
            string ShowMsg = "";
            SMsgActionFruitContext_SC GrowUpFruit = TreasureTreesData.Instance.FruitDataList.FirstOrDefault(P=>P.byFruitStatus!=4);
            if (GrowUpFruit.dwFruitID != 0)
            {
                foreach (var child in TreasureTreesData.Instance.FruitDataList)
                {
                    if (child.byFruitStatus != 4 && child.dwEndTime < GrowUpFruit.dwEndTime)
                    {
                        GrowUpFruit = child;
                    }
                }
                long LeftSconds = GrowUpFruit.dwEndTime - TreasureTreesData.Instance.GetNowTimes();
                //TraceUtil.Log("FastFruit:" + GrowUpFruit.dwEndTime.ToString() + "NowTime:" + TreasureTreesData.Instance.GetNowTimes());
                //TraceUtil.Log("LeftSconds:"+LeftSconds);
                long leftDay = LeftSconds / 60 / 60 / 24;
                long leftHour = LeftSconds / 60 / 60 % 24;
                long leftminute = LeftSconds /60% 60;
                long m_leftSconds = LeftSconds % 60;
                ShowMsg = string.Format(LanguageTextManager.GetString("IDS_H1_456"), leftHour<0?0:leftHour, leftminute<0?0:leftminute, m_leftSconds<0?0:m_leftSconds);
                NextGrowUpTip.SetActive(true);
                AllGrowUpTip.SetActive(false);
            }
            else
            {
                NextGrowUpTip.SetActive(false);
                AllGrowUpTip.SetActive(true);
                ShowMsg = "";//全部成熟提示
            }
            TimeLeftLabel.SetText(ShowMsg);
        }

        void UpdatePanel(object obj)
        {
            TreasureTreesFruitPointList.ApplyAllItem(P=>P.UpdateFruitPointStatus());
            treasureTreesButtonToolManager.ResetBtnStatus();
        }

        void OnBackButtonTapped(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Tree_Leave");
			
            CleanUpUIStatus();
            Close();
        }

        public void ShowCostMoneyMessageBox(bool CanBuy,EMessageCoinType CostMoneyType, int CostMoneyNumber, string Msg, string SureBtnStr, string CancelBtnStr, MessageBoxCallBack SureBtnCallBack, MessageBoxCallBack CancelBtnCallBack)
        {
            MessageBox.Instance.Show(3, CostMoneyType, Msg, CostMoneyNumber, SureBtnStr, CancelBtnStr, SureBtnCallBack, BuyCancelCallBack);
        }

        void BuyCancelCallBack()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Tree_BuyCancel");
        }

        void OnUseManaFromServer(object obj)
        {
            SMsgActionUseManna_SC sMsgActionUseManna_SC = (SMsgActionUseManna_SC)obj;
            FruitData fruitData = TreasureTreesDataBase.FruitDataList.FirstOrDefault(P => P.FruitID == sMsgActionUseManna_SC.dwFruitID);
            FruitLogMessageWindow.AddUserAmritaLogInfoFromServer(fruitData);


        }

        void OnGetRewardFromServer(object obj)
        {
            SMsgActionChooseFruit_SC sMsgActionChooseFruit_SC = (SMsgActionChooseFruit_SC)obj;
            FruitData fruitData = TreasureTreesDataBase.FruitDataList.FirstOrDefault(P => P.FruitID == sMsgActionChooseFruit_SC.dwFruitID);
            FruitLogMessageWindow.AddGetFruitLogInfoServer(fruitData, (int)sMsgActionChooseFruit_SC.dwGoodsID, (int)sMsgActionChooseFruit_SC.dwGoodsNum);
            GetAwardAnimManager.PickUpFruit(sMsgActionChooseFruit_SC.byFruitPosition, (int)sMsgActionChooseFruit_SC.dwGoodsID, (int)sMsgActionChooseFruit_SC.dwGoodsNum);
        }


    }
}