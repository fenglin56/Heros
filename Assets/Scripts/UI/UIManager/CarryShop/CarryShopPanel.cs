using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{
	public class CarryShopPanel : BaseUIPanel {
		//public CarryShopConfigDataBase carryShopData;
		//各按钮
		public SingleButtonCallBack btnBack;
		public SingleButtonCallBack btnRefreshNextTimes;
		public SingleButtonCallBack btnInfo;
		public SingleButtonCallBack btnArrowLeft;
		public SingleButtonCallBack btnArrowRight;
		public CommonPanelTitle comPanelTitle;
		//弹出
		public GameObject popInfoPanelPrefab;
		private GameObject popInfoPanel;
		//倒计时
		public UILabel downTimeNameLabel;
		public UILabel downTimeLabel;
		//主体部分
		public GameObject rankParent;
		public GameObject itemPrefab;
		public Transform selectMark;
		//npc相关信息
		public GameObject NPCPrefab;
		public Camera NPCCamera;
		private GameObject NpcObj;
		private string DefaultAnim;
		public Vector3 NpcPos;
		//list data
		private List<CarryShopItem> gridList = new List<CarryShopItem>();
		private string itemStr = "GoodsCard";
		private bool isRead = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			//npc展示
			NpcObj = GameObject.Instantiate(NPCPrefab) as GameObject;
			NpcObj.transform.parent = NPCCamera.transform.Find ("ModelParent");
			NpcObj.transform.localPosition = NpcPos;
			NpcObj.transform.localRotation = Quaternion.Euler(0,197f,0);
			NpcObj.transform.localScale = Vector3.one;
			DefaultAnim = "LadyTIdle01";//NpcObj.animation.clip.name;
			NpcObj.animation.CrossFade(DefaultAnim);
			btnBack.SetCallBackFuntion (OnClickBackEvent);
			btnRefreshNextTimes.SetCallBackFuntion (OnClickNextTimesEvent);
			btnArrowLeft.SetCallBackFuntion (OnClickLeftEvent);
			btnArrowRight.SetCallBackFuntion (OnClickRightEvent);
			btnInfo.SetPressCallBack (OnPressInfoEvent);
			downTimeNameLabel.text = LanguageTextManager.GetString("IDS_I29_6");
			CreateListItem ();
			NetServiceManager.Instance.TradeService.SendCarryShopUI_CS(0);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.RcvCarryShopUIDataEvent,OnRcvCarryShopUIDataEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.RcvCarryShopBuyEvent,OnRcvCarryShopBuyEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.RcvCarryShopUnLockEvent,OnRcvCarryShopUnLockEvent);
		}
		void CreateListItem ()
		{
			for(int i = 1 ; i < 7; i++)
			{
				Transform cardParent = rankParent.transform.Find(itemStr+i);
				GameObject card = NGUITools.AddChild(cardParent.gameObject,itemPrefab);
				gridList.Add(card.GetComponent<CarryShopItem>());
			}
			itemPrefab.SetActive (false);
		}
		public override void Show(params object[] value)
		{
			base.Show ();
			NPCCamera.gameObject.SetActive(true);
			Init ();
			if (CarryShopModel.Instance.isRequestedServerData) {
				ShowPanel (true);
			}
			comPanelTitle.TweenShow ();
			SetSelectMarkUnShow ();
		}
		void ShowPanel(bool isRefreshAll)
		{
			if (isRefreshAll) {
				CarryShopModel.Instance.curShowIndexView = 0;
				StartDownTime ();
				RefreshPanel ();
				PlayItemEff ();
				SetSelectMarkUnShow ();
			} else {
				RefreshPanel ();
			}
		}
		#region 主体界面
		//当移动时，界面更新
		void RefreshPanel()
		{
			UISprite sprite = btnArrowLeft.transform.Find("Background").GetComponent<UISprite>();
			if (CarryShopModel.Instance.curShowIndexView == 0) {
				sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
				btnArrowLeft.gameObject.GetComponent<BoxCollider>().enabled = false;
			} else {
				sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, 1f);
				btnArrowLeft.gameObject.GetComponent<BoxCollider>().enabled = true;
			}
			sprite = btnArrowRight.transform.Find("Background").GetComponent<UISprite>();
			int maxCount = CarryShopModel.Instance.sCarryShopOpenUI_SC.shopUintMap.Count >= CommonDefineManager.Instance.CommonDefine.ShopSlotMaxNum ? CarryShopModel.Instance.sCarryShopOpenUI_SC.shopUintMap.Count:CarryShopModel.Instance.sCarryShopOpenUI_SC.shopUintMap.Count+1;
			if (CarryShopModel.Instance.curShowIndexView == ((maxCount-1)/6)) {
				sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
				btnArrowRight.gameObject.GetComponent<BoxCollider>().enabled = false;
			} else {
				sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, 1f);
				btnArrowRight.gameObject.GetComponent<BoxCollider>().enabled = true;
			}
			UpdateList ();
		}
		void PlayItemEff()
		{
			foreach(CarryShopItem item in gridList)
			{
				if(item.gameObject.activeSelf && !item.isItemLock)
				{
					item.PlayEff();
				}
			}
		}
		void UpdateList ()
		{
			int startIndex = CarryShopModel.Instance.curShowIndexView*6;
			int count = 0;
			bool isOver = false;
			for (int i = startIndex; i <= CarryShopModel.Instance.sCarryShopOpenUI_SC.shopUintMap.Count && count < 6; i++) {
				isOver = false;
				if(i == CarryShopModel.Instance.sCarryShopOpenUI_SC.shopUintMap.Count)
				{
					if(i >= CommonDefineManager.Instance.CommonDefine.ShopSlotMaxNum)
					{
						//已经最大，无再开锁数据
						break;
					}
					DCarryShopUint item = new DCarryShopUint();
					item.byIndex = (byte)(i+1);
					gridList[count].gameObject.SetActive(true);
					gridList[count].Show(this,item,count,true,false);
					count++;
					break;
				}
				else
				{
					if(CarryShopModel.Instance.sCarryShopOpenUI_SC.shopUintMap[i].byIsSale == 1)
					{
						isOver = true;
					}
					gridList[count].gameObject.SetActive(true);
					gridList[count].Show(this,CarryShopModel.Instance.sCarryShopOpenUI_SC.shopUintMap[i],count,false,isOver);
				}
				count++;
			}
			if (count < 6) {
				for(; count < 6; count++)
				{
					gridList[count].gameObject.SetActive(false);
				}
			}
		}
		#endregion

		#region npc
		public void NpcPlayAnim(string animationClip)
		{
			this.NpcObj.animation.CrossFade(animationClip);
			StopAllCoroutines();
			StartCoroutine(PlayIdleForTime(NpcObj.animation[animationClip].clip.length));
		}
		
		IEnumerator PlayIdleForTime(float time)
		{
			yield return new WaitForSeconds(time);
			NpcObj.animation.CrossFade(DefaultAnim,0.1f);
		}
		#endregion

		#region 长按弹介绍
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
			CarryShopInfoPanel popInfo = popInfoPanel.GetComponent<CarryShopInfoPanel>();
			popInfo.Show ();
		}
		#endregion
		//
		void RefreshTimes()
		{
			//刷新次数更新
			/*if (PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SHOPREFRESH_VALUE > 0) {
				btnRefreshNextTimes.gameObject.GetComponent<BoxCollider> ().enabled = true;		
			} else {
				btnRefreshNextTimes.gameObject.GetComponent<BoxCollider> ().enabled = false;	
			}*/
		}
		#region 倒计时
		private float sinceTime = 0;
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
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RcvCarryShopUIDataEvent,OnRcvCarryShopUIDataEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RcvCarryShopBuyEvent,OnRcvCarryShopBuyEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RcvCarryShopUnLockEvent,OnRcvCarryShopUnLockEvent);
		}
		void StartDownTime()
		{
			DownTimeReset ();
			/*if (IsInvoking ("TimeUpdate")) {
				CancelInvoke("TimeUpdate");
			}
			InvokeRepeating("TimeUpdate",0.5f ,0.5f);*/
		}
		private void DownTimeReset()
		{
			UpdateTime ();
		}
		private void UpdateTime()
		{
			/*sinceTime = Time.realtimeSinceStartup - CarryShopModel.Instance.carryShopRealtimeSinceStartup;
			CarryShopModel.Instance.carryShopRealtimeSinceStartup = Time.realtimeSinceStartup;
			CarryShopModel.Instance.carryShopUpdatDownTime = CarryShopModel.Instance.carryShopUpdatDownTime - sinceTime;
			if (CarryShopModel.Instance.carryShopUpdatDownTime < 0) {
				CarryShopModel.Instance.carryShopUpdatDownTime = 0;
				//时间到//
			}
			int hour = (int)CarryShopModel.Instance.carryShopUpdatDownTime/3600;
			int minue = ((int)CarryShopModel.Instance.carryShopUpdatDownTime%3600)/60;
			int second = (int)CarryShopModel.Instance.carryShopUpdatDownTime%3600%60;*/
			System.DateTime showTime = TreasureTreesData.Instance.GetNoralTime (CarryShopModel.Instance.carryShopUpdatDownTime.ToString());
			downTimeLabel.text = string.Format (LanguageTextManager.GetString("IDS_I29_24"),showTime.Hour);//
		}
		void TimeUpdate()
		{
			UpdateTime ();
		}
		#endregion

		void OnRcvCarryShopUIDataEvent(object obj)
		{
			if (!gameObject.activeInHierarchy)
				return;
			ShowPanel (true);
		}
		void OnRcvCarryShopBuyEvent(object obj)
		{
			if (!gameObject.activeInHierarchy)
				return;
			ShowPanel (false);
		}
		void OnRcvCarryShopUnLockEvent(object obj)
		{
			if (!gameObject.activeInHierarchy)
				return;
			ShowPanel (false);
			SetSelectMarkUnShow ();
		}

		void OnClickBackEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_Exit");
			Close ();
		}
		//刷新次数
		void OnClickNextTimesEvent(object obj)
		{
			int costMon = CarryShopModel.Instance.GetCarryShopCost (PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SHOPREFRESH_VALUE);
			if (PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SHOPREFRESH_VALUE > 0) {
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_ShiftWindow");
				UI.MessageBox.Instance.Show(3,EMessageCoinType.EGoldType,string.Format(LanguageTextManager.GetString("IDS_I29_5"),PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SHOPREFRESH_VALUE),
				                            costMon,LanguageTextManager.GetString("IDS_H2_55"),LanguageTextManager.GetString("IDS_H2_28"),
				                            ()=>{DealChangeData(EMessageCoinType.EGoldType,costMon);},
				()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_Cancel");});
			} else {
				UI.MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I29_4"),1);	
			}
		}
		void DealChangeData(EMessageCoinType coinType,int price)
		{
			if (!CarryShopModel.Instance.isJudgeMoneyEnough (coinType, price)) {
				SoundManager.Instance.PlaySoundEffect ("Sound_Button_Auction_Fail");
				if(coinType == EMessageCoinType.ECuType)
				{
					UI.MessageBox.Instance.ShowNotEnoughMoneyMsg (null);
				}
				else
				{
					UI.MessageBox.Instance.ShowNotEnoughGoldMoneyMsg ();
				}
			} else {
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_ShiftConfirm");
				NetServiceManager.Instance.TradeService.SendCarryShopUI_CS(1);
			}
		}
		void OnClickRightEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_PageChange");
			CarryShopModel.Instance.curShowIndexView++;
			int maxCount = CarryShopModel.Instance.sCarryShopOpenUI_SC.shopUintMap.Count >= CommonDefineManager.Instance.CommonDefine.ShopSlotMaxNum ? CarryShopModel.Instance.sCarryShopOpenUI_SC.shopUintMap.Count:CarryShopModel.Instance.sCarryShopOpenUI_SC.shopUintMap.Count+1;
			if (CarryShopModel.Instance.curShowIndexView > ((maxCount - 1) / 6)) {
				CarryShopModel.Instance.curShowIndexView--;			
			}
			ShowPanel (false);
			DealSelectMark ();
		}
		void OnClickLeftEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_PageChange");
			CarryShopModel.Instance.curShowIndexView--;
			if (CarryShopModel.Instance.curShowIndexView < 0) {
						
				CarryShopModel.Instance.curShowIndexView = 0 ;
			}
			ShowPanel (false);
			DealSelectMark ();
		}

		public void OnSelectGoods(CarryShopItem item)
		{
			CarryShopModel.Instance.curSelectIndex = item.curIndex+CarryShopModel.Instance.curShowIndexView*6;
			SetCurSelectMark (item);
			EMessageCoinType coinType = EMessageCoinType.EGoldType;
			int price = 0;
			if (item.shopData != null) {
				if (item.shopData.BuyType == 1) {
					coinType = EMessageCoinType.ECuType;
				} else if (item.shopData.BuyType == 3) {
					coinType = EMessageCoinType.EGoldType;			
				}
			}
			if (item.isItemLock) {
				//弹出解锁//
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_UnlockWindow");
				price = CarryShopModel.Instance.GetUnLockPrice (item.shopUint.byIndex);
				UI.MessageBox.Instance.Show(3,coinType,LanguageTextManager.GetString("IDS_I29_3"),price,
				                            LanguageTextManager.GetString("IDS_H2_55"),LanguageTextManager.GetString("IDS_H2_28"),
				                            ()=>{OnDealSendData(item,coinType,price,true);},()=>{
					//取消
					SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_Cancel");
				});
			} else if (item.isItemOver) {
				//不动//
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_Unavailable");
			} else {
				//弹出购买//
				price = item.shopData.Price;
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_BuyWindow");
				UI.MessageBox.Instance.Show(3,coinType,string.Format(LanguageTextManager.GetString("IDS_I29_1"),LanguageTextManager.GetString(item.shopData.GoodsNameIds)),
				                            price,LanguageTextManager.GetString("IDS_H2_55"),LanguageTextManager.GetString("IDS_H2_28"),
				                            ()=>{OnDealSendData(item,coinType,price,false);},()=>{
					//取消
					SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_Cancel");
				});
			}
		}
		bool isShowSelectMark = false;
		void DealSelectMark()
		{
			if(!isShowSelectMark)
				return ;
			if (CarryShopModel.Instance.curSelectIndex <= CarryShopModel.Instance.curShowIndexView * 6 + 5 && CarryShopModel.Instance.curSelectIndex >= CarryShopModel.Instance.curShowIndexView * 6) {
				selectMark.gameObject.SetActive (true);
			} else {
				selectMark.gameObject.SetActive(false);		
			}
		}
		void SetSelectMarkUnShow()
		{
			CarryShopModel.Instance.curSelectIndex = 0;
			selectMark.gameObject.SetActive (false);	
			isShowSelectMark = false;
		}
		void SetCurSelectMark(CarryShopItem item)
		{
			isShowSelectMark = true;
			selectMark.gameObject.SetActive (true);
			selectMark.parent = item.transform;
			selectMark.localPosition = Vector3.zero;
		}
		public override void Close()
		{
			if (!IsShow)
				return;
			base.Close();
			NPCCamera.gameObject.SetActive(false);
			comPanelTitle.tweenClose ();
		}
		//处理解锁
		void OnDealSendData(CarryShopItem item,EMessageCoinType coinType,int price,bool isUnLock)
		{
			if (!CarryShopModel.Instance.isJudgeMoneyEnough (coinType,price)) {
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Auction_Fail");
				if(coinType == EMessageCoinType.ECuType)
				{
					UI.MessageBox.Instance.ShowNotEnoughMoneyMsg (null);
				}
				else
				{
					UI.MessageBox.Instance.ShowNotEnoughGoldMoneyMsg ();
				}
			}
			else
			{
				if(isUnLock)
				{
					SoundManager.Instance.PlaySoundEffect("Sound_Button_Shop_UnlockConfirm");
					NetServiceManager.Instance.TradeService.SendCarryShopUnLock_CS(PlayerManager.Instance.FindHeroDataModel ().ActorID,item.shopUint.byIndex);
				}else
				{
					int pack = GetGoodsHavePacks((int)item.shopUint.dwShopID);
					if( pack > 0 && ContainerInfomanager.Instance.PackIsFull())
					{
						UI.MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_H2_2"),1);// IDS_H2_2
						return;
					}
					NpcPlayAnim ("LadyAction07");
					SoundManager.Instance.PlaySoundEffect("Sound_UIEff_ShopLady");
					NetServiceManager.Instance.TradeService.SendCarryShopBuy_CS(item.shopUint.dwShopID,item.shopUint.byIndex,item.shopUint.dwShopNum);
				}
			}
		}
		int GetGoodsHavePacks(int shopGoodsID)
		{
			foreach(ShopConfigData data in ShopDataManager.Instance.shopConfigDataBase._dataTable)
			{
				if(data._shopGoodsID == shopGoodsID)
				{
					return data.PackageNeed;
				}
			}
			return 0;
		}
	}
}