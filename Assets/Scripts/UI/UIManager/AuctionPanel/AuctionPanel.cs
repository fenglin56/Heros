using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace UI.MainUI
{
	public class AuctionPanel : BaseUIPanel {
		public CommonPanelTitle comPanelTitle;
		//弹出商行输入价格
		public GameObject popInputPricePrefab;
		//返回
		public SingleButtonCallBack btnBack;
		//商行介绍
		public SingleButtonCallBack btnInfo;
		public GameObject popInfoPanelPrefab;
		private GameObject popInfoPanel;
		//倒计时
		public UILabel downTimeNameLabel;
		public UILabel downTimeLabel;
		//中间部分标题
		public UILabel goodsLabel;
		public UILabel prefessionLabel;
		public UILabel typeLabel;
		public UILabel priceLabel;
		public UILabel buyerLabel;
		public UILabel buyLabel;
		//中间部分
		public GameObject content;
		public GameObject rankParent;
		public GameObject itemPrefab;
		private List<AuctionItemInfo> gridList = new List<AuctionItemInfo>();
		private bool isRead = false;
		private float preSinceStartUp = 0f;
		private float sinceTime = 0;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			itemPrefab.SetActive (false);
			goodsLabel.text = LanguageTextManager.GetString("IDS_I27_1");
			prefessionLabel.text = LanguageTextManager.GetString("IDS_I27_2");
			typeLabel.text = LanguageTextManager.GetString("IDS_I27_3");
			priceLabel.text = LanguageTextManager.GetString("IDS_I27_4");
			buyerLabel.text = LanguageTextManager.GetString("IDS_I27_5");
			buyLabel.text = LanguageTextManager.GetString("IDS_I27_6");
			btnBack.SetCallBackFuntion (OnClickBackEvent);
			btnInfo.SetPressCallBack (OnPressInfoEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveServerAuctionData,OnReceiveAuctionDataHandler);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveServerAuctionResult,OnReceiveAuctionResultHandler);
			downTimeNameLabel.text = LanguageTextManager.GetString("IDS_I27_7");

		}
		public override void Show(params object[] value)
		{
			base.Show ();
			Init ();
			//AuctionModel.Instance.Test ();
			if (AuctionModel.Instance.isHaveRequestAuctionData) {
				ShowPanel (false);
			} else {
				NetServiceManager.Instance.TradeService.SendMsg_Trade_Auction_UI();
				comPanelTitle.GetComponent<CommonPanelTitle> ().TweenShow ();
			}
		}
		void ShowPanel(bool isRefresh)
		{
			StartDownTime ();
			ShowRank ();
			if (!isRefresh) {
				comPanelTitle.GetComponent<CommonPanelTitle> ().TweenShow ();	
			}
		}
		//刷新某一项数据界面
		void RefreshPanel(int index)
		{
			foreach (AuctionItemInfo info in gridList) {
				if(info.curByIndex == index)
				{
					info.Show(this,index);
				}
			}
		}
		void ShowRank()
		{
			CreateRankItem ();
			UpdateRank ();
		}
		//创建新的
		private void CreateRankItem()
		{
			int Count = AuctionModel.Instance.sAuctionOpenUI_SC.auctionMap.Count;
			if (gridList.Count >= Count) {
				return;
			}
			int startCount = gridList.Count;
			itemPrefab.SetActive (true);

			for (int i = startCount; i < Count; i++) {
				GameObject item = UI.CreatObjectToNGUI.InstantiateObj(itemPrefab,rankParent.transform);
				item.name = string.Format("Item{0:d2}",i);
				AuctionItemInfo rankItem = item.GetComponent<AuctionItemInfo>();
				gridList.Add(rankItem);
			}
			itemPrefab.SetActive (false);
		}
		public override void Close()
		{
			if (!IsShow)
				return;
			base.Close();
			if (IsInvoking ("TimeUpdate")) {
				CancelInvoke("TimeUpdate");
			}
			comPanelTitle.GetComponent<CommonPanelTitle> ().tweenClose ();
		}
		//刷新
		private void UpdateRank()
		{
			int count = 0;
			foreach (var auction in AuctionModel.Instance.sAuctionOpenUI_SC.auctionMap) {
				gridList[count++].Show(this,(int)auction.Key);
			}
			rankParent.GetComponent<UIGrid> ().Reposition ();
		}
		//点击竞拍
		public void OnClickBuyGoods(int index)
		{
			GameObject go = UI.CreatObjectToNGUI.InstantiateObj (popInputPricePrefab,transform);
			go.transform.localPosition = new Vector3 (0,0,-20);
			AuctionSelectMoneyPanel moneyInfo = go.GetComponent<AuctionSelectMoneyPanel>();
			moneyInfo.Show (index);
		}
		void OnClickBackEvent(object obj)
		{
			Close ();
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Auction_Exit");
			//Show ();
		}
		private bool isPopMark = false;
		void OnPressInfoEvent(bool isPressed)
		{
			if (isPressed) {
				CountPressTime();
			} else {
				if(isPopMark)
				{
					isPopMark = false;
					popInfoPanel.SetActive (false);
				}
			}
		}
		void CountPressTime()
		{
			PopInfoPanel ();
		}
		void PopInfoPanel()
		{
			isPopMark = true;
			if (popInfoPanel == null) {
				popInfoPanel = UI.CreatObjectToNGUI.InstantiateObj (popInfoPanelPrefab, transform);
				popInfoPanel.transform.localPosition = new Vector3 (0, 0, -20);
			} else {
				popInfoPanel.SetActive (true);
			}
			AuctionInfoPanel popInfo = popInfoPanel.GetComponent<AuctionInfoPanel>();
			popInfo.Show ();
		}
		//更新整个界面
		void OnReceiveAuctionDataHandler(object obj)
		{
			if (!gameObject.activeInHierarchy)
				return;
			//刷新一下界面
			ShowPanel (true);
		}
		//更新某一项数据时的更新
		void OnReceiveAuctionResultHandler(object obj)
		{
			if (!gameObject.activeInHierarchy)
				return;
			//刷新一下界面
			int index = Convert.ToInt32(obj);
			RefreshPanel (index);
		}
		void OnDisable()
		{
			if (IsInvoking ("TimeUpdate")) {
				CancelInvoke("TimeUpdate");
			}
		}
		void OnDestroy()
		{
			if (IsInvoking ("TimeUpdate")) {
				CancelInvoke("TimeUpdate");
			}
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveServerAuctionData,OnReceiveAuctionDataHandler);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveServerAuctionResult,OnReceiveAuctionResultHandler);
		}
		void StartDownTime()
		{
			preSinceStartUp = Time.realtimeSinceStartup;
			DownTimeReset ();
			if (IsInvoking ("TimeUpdate")) {
				CancelInvoke("TimeUpdate");
			}
			InvokeRepeating("TimeUpdate",0.5f ,0.5f);
		}
		private void DownTimeReset()
		{
			UpdateTime ();
		}
		private void UpdateTime()
		{
			sinceTime = Time.realtimeSinceStartup - preSinceStartUp;
			preSinceStartUp = Time.realtimeSinceStartup;
			AuctionModel.Instance.auctionUpdatDownTime = AuctionModel.Instance.auctionUpdatDownTime - sinceTime;
			if (AuctionModel.Instance.auctionUpdatDownTime < 0) {
				AuctionModel.Instance.auctionUpdatDownTime = 0;
				//时间到//
				if (IsInvoking ("TimeUpdate")) {
					CancelInvoke("TimeUpdate");
				}
				NetServiceManager.Instance.TradeService.SendMsg_Trade_Auction_UI();
			}
			int hour = (int)AuctionModel.Instance.auctionUpdatDownTime/3600;
			int minue = ((int)AuctionModel.Instance.auctionUpdatDownTime%3600)/60;
			int second = (int)AuctionModel.Instance.auctionUpdatDownTime%3600%60;
			downTimeLabel.text = string.Format ("{0:d2}:{1:d2}:{2:d2}",hour,minue,second);
		}
		void TimeUpdate()
		{
			UpdateTime ();
		}
	}
}
