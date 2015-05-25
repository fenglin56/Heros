using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class QuickBuy : MonoBehaviour {
	public ShopConfigDataBase shopConfigDataBase ;
	private List<ShopConfigData> curShopConfigList = new List<ShopConfigData> ();
	public SingleButtonCallBack btnBack;
	//buy
	public SingleButtonCallBack btnBuy;
	//界面
	public GameObject payPanel;
	private string goodStr = "GoodsCard";
	//6个商品//
	public GameObject goodsPerfab;
	private int curSelectIndex = 0;
	private List<UI.MainUI.SingleTopUpCard> SingleTopUpCardList = new List<UI.MainUI.SingleTopUpCard>();
	//当前选择标记
	public Transform curSelectCardMark ;
	private bool isRead = false;
	void Init(int shopConfigID)
	{
		if (isRead)
			return;
		isRead = true;
		btnBuy.SetCallBackFuntion(OnBuyButtonClick);
		btnBack.SetCallBackFuntion(OnBackButtonClick);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.ShopsBuySuccess, OnBuySuccess);
		UIEventManager.Instance.RegisterUIEvent (UIEventType.CloseAllUI,OnCloseAllUIEvent);
		InitData (shopConfigID);
	}
	void InitData(int shopConfigID)
	{
		for (int i = 1; i <= 6; i++) {
			Transform tran = payPanel.transform.Find (goodStr+i);
			GameObject goods = NGUITools.AddChild(tran.gameObject,goodsPerfab);
			SingleTopUpCardList.Add(goods.gameObject.GetComponent<UI.MainUI.SingleTopUpCard>());
			SingleTopUpCardList[i-1].Init(false,null,this,i);
		}
		goodsPerfab.SetActive (false);
		foreach(ShopConfigData data in shopConfigDataBase._dataTable)
		{
			if(data._shopID == shopConfigID)//110)
			{
				curShopConfigList.Add(data);
			}
		}
	}
	public void Show(int shopConfigID)
	{
		Init (shopConfigID);
		for (int i = 0; i < curShopConfigList.Count; i++) {
			SingleTopUpCardList[i].ShowQuickTip(curShopConfigList[i]);
		}
		SetCurPaySelectMark (0);
	}
	//点击选中某个
	public void OnTopUpCardSelect(UI.MainUI.SingleTopUpCard selectTopUpCard)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_QuickBuyChoice");
		curSelectIndex = selectTopUpCard.shopIndex - 1;
		SetCurPaySelectMark (selectTopUpCard.shopIndex-1);
	}
	//设置当前选中项
	void SetCurPaySelectMark(int shopID)
	{
		curSelectCardMark.parent = SingleTopUpCardList[shopID].transform;
		curSelectCardMark.localPosition = Vector3.zero;
	}
	void OnBuyButtonClick(object obj)
	{
		ShopConfigData shopData = curShopConfigList[curSelectIndex];
		GetCoinType ();
		SoundManager.Instance.PlaySoundEffect("Sound_Button_QuickBuySubmit");
		string tipStr = string.Format (LanguageTextManager.GetString("IDS_I7_36"),LanguageTextManager.GetString(shopData.GoodsNameIds));
		UI.MessageBox.Instance.Show(4,coinType, tipStr,price, LanguageTextManager.GetString("IDS_H2_55"),
		                            LanguageTextManager.GetString("IDS_H2_28"),OnBuySureClick,OnBuyCancelClick);
	}
	void OnBuySureClick()
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_QuickBuyConfirmation");
		//Invoke("NextMessageBox",0.1f);
		NextMessageBox ();
	}
	void OnBuyCancelClick()
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_QuickBuyCancel");
	}
	private UI.EMessageCoinType coinType ;
	private int price ;
	void GetCoinType()
	{
		ShopConfigData shopData = curShopConfigList [curSelectIndex];
		price = shopData.Price;
		coinType = UI.EMessageCoinType.EGoldType;
		if (shopData.BuyType == 1) {
			coinType = UI.EMessageCoinType.ECuType;
		} else if (shopData.BuyType == 3) {
			coinType = UI.EMessageCoinType.EGoldType;			
		}
	}
	void NextMessageBox()
	{
		if (!CarryShopModel.Instance.isJudgeMoneyEnough (coinType,price)) {
			if(coinType == UI.EMessageCoinType.ECuType)
			{
				UI.MessageBox.Instance.ShowNotEnoughMoneyMsg (null);
			}
			else
			{
				UI.MessageBox.Instance.ShowNotEnoughGoldMoneyMsg (()=>{
					Destroy(gameObject);
				});
			}
		}
		else
		{
			SendDataToServer();
		}
	}
	void GoldLessSureCB()
	{
		Destroy(gameObject);
	}

	void SendDataToServer()
	{
		ShopConfigData data = curShopConfigList[curSelectIndex];
		SMsgTradeBuyGoods_CS sMsgTradeBuyGoods_CS = new SMsgTradeBuyGoods_CS()
		{
			dwShopID = (uint)data._shopID,
			uidNPC = 0,
			lShopGoodsID = (uint)data._shopGoodsID,
			GoodsID = (uint)data.GoodsID,
			GoodsNum = 1,
		};
		NetServiceManager.Instance.TradeService.SendTradeBuyGoods(sMsgTradeBuyGoods_CS);
	}
	private void OnBackButtonClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
		Destroy(gameObject);
	}
	//收到服务器消息
	void OnBuySuccess(object obj)
	{
		//显示一个messagebox//
		UI.MessageBox.Instance.ShowTips (3,LanguageTextManager.GetString("IDS_I7_43"), 1f);
		//关闭界面//
		Destroy (gameObject);
	}
	void OnCloseAllUIEvent(object obj)
	{
		Destroy (gameObject);
	}
	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShopsBuySuccess, OnBuySuccess);
		UIEventManager.Instance.RemoveUIEventHandel (UIEventType.CloseAllUI,OnCloseAllUIEvent);
	}
}
