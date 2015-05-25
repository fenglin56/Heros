using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum EDailySignType{
	//已签到
	EAlreadySign,
	//可签到，但未签到//
	ENoneSign,
	//未签，但可补签//
	ECanRepairSign,
	//不可签到，时间未到//
	ECanNotSign
}

public class SignWeekInfo : MonoBehaviour
{
	public UILabel DiscountLabel;
	//补签
	public GameObject repairSignBtn;
	public GameObject alreadySingMark;
	//商品
	public ItemIconInfo itemIconInfo;
	[HideInInspector]
	public int weekIndex;//从1开始//
	public DailySignInConfigData dailySignData { get; private set; }
	public UI.MainUI.DailySignPanel MyParent { get; private set; }
	[HideInInspector]
	public EDailySignType curSignType;
	void Awake()
	{
		repairSignBtn.GetComponent<SingleButtonCallBack> ().SetCallBackFuntion (OnClickRepairEvent);
	}
	public void Show(UI.MainUI.DailySignPanel parent,int index,EDailySignType signType,DailySignInConfigData signData)
	{
		weekIndex = index;//int.Parse(transform.parent.name.Substring (9,1));
		MyParent = parent;
		curSignType = signType;
		alreadySingMark.SetActive (false);
		repairSignBtn.SetActive (false);
		if (curSignType == EDailySignType.EAlreadySign) {
			alreadySingMark.SetActive(true);
		} else if (curSignType == EDailySignType.ECanRepairSign) {
			repairSignBtn.SetActive (true);
		}
		ShowInfo (signData);
	}
	private void ShowInfo(DailySignInConfigData signData)
	{
		gameObject.SetActive(true);
		this.dailySignData = signData;
		string weekStr = "";
		switch (weekIndex) {
		case 1:
			weekStr = "IDS_I26_7";
			break;
		case 2:
			weekStr = "IDS_I26_8";
			break;
		case 3:
			weekStr = "IDS_I26_9";
			break;
		case 4:
			weekStr = "IDS_I26_10";
			break;
		case 5:
			weekStr = "IDS_I26_11";
			break;
		case 6:
			weekStr = "IDS_I26_12";
			break;
		case 7:
			weekStr = "IDS_I26_13";
			break;
		}
		DiscountLabel.text = LanguageTextManager.GetString (weekStr);
		CGoodsInfo goodsInfo = dailySignData.dailyRewardList [weekIndex] [0];
		ItemData getItem = ItemDataManager.Instance.GetItemData(goodsInfo.itemID);
		itemIconInfo.Init(getItem,"x"+goodsInfo.itemCount);
	}
	//点击补签
	void OnClickRepairEvent(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_SignInRegist");
		MyParent.OnRepairSignBtnClick (this);
	}
	//点击选中
	void OnClick()
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
		MyParent.OnWeekSelect (this);	
	}
}