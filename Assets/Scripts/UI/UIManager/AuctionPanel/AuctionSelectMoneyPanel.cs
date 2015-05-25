using UnityEngine;
using System.Collections;

public class AuctionSelectMoneyPanel : MonoBehaviour {
	//价格信息
	public UILabel preBuyerInfo;
	public UILabel preBuyMoney;
	public UILabel myBuyInfo;
	public UILabel myBuyMoney;
	//进度条
	public UISlider progressSlider;
	//输入价格
	public UILabel inputNameLabel;
	public UIInput inputMoney;
	//按钮
	public SingleButtonCallBack cancelBtn;
	public SingleButtonCallBack sureBtn;
	private int curByIndex;
	private int minMoney = 100;
	private int maxMoney = 1000;
	private int finishMoney;
	private bool isRead = false;
	void Init()
	{
		if (isRead)
			return;
		isRead = true;
		cancelBtn.transform.Find ("Label").GetComponent<UILabel> ().text = LanguageTextManager.GetString ("IDS_I27_22");
		sureBtn.transform.Find ("Label").GetComponent<UILabel> ().text = LanguageTextManager.GetString ("IDS_I27_21");
		cancelBtn.SetCallBackFuntion (OnClickCancelEvent);
		sureBtn.SetCallBackFuntion (OnClickSureEvent);
		progressSlider.onValueChange = OnSliderChange;
		inputMoney.onSubmit = OnInputSubmit;
	}
	public void Show(int byIndex)
	{
		curByIndex = byIndex;
		Init ();
		ShowInfo ();
	}
	void ShowInfo()
	{
		DAuctionUint? auctionData = AuctionModel.Instance.GetGoodsInfo (curByIndex);
		ItemData item = ItemDataManager.Instance.GetItemData((int)auctionData.Value.dwGoodsID);
		preBuyerInfo.text = string.Format (LanguageTextManager.GetString("IDS_I27_10"),AuctionModel.Instance.GetBuyerName(auctionData.Value));
		preBuyMoney.text = auctionData.Value.dwCurPrice.ToString ();
		myBuyInfo.text = LanguageTextManager.GetString ("IDS_I27_11");
		float rateVal = CommonDefineManager.Instance.CommonDefine.AuctionBidRate;
		if(auctionData.Value.dwAcotorID == 0)
			rateVal = 0f;
		minMoney = (int)(auctionData.Value.dwCurPrice*(rateVal/100f+1));
		if (minMoney >= CommonDefineManager.Instance.CommonDefine.AuctionTopBid) {
			minMoney = CommonDefineManager.Instance.CommonDefine.AuctionTopBid;		
		}
		maxMoney = CommonDefineManager.Instance.CommonDefine.AuctionTopBid;
		myBuyMoney.text = minMoney.ToString();
		//进度条
		SetProgessInfo (0);
		inputNameLabel.text = LanguageTextManager.GetString ("IDS_I27_12");
	}
	void SetProgessInfo(float progress)
	{
		SetProgress(progress);
		SetShowProgressMoney (progress);
	}
	void SetProgress(float progress)
	{
		progressSlider.sliderValue = progress;
	}
	//设置输入框中数字//
	void SetShowProgressMoney(float progress)
	{
		inputMoney.text = ((int)(minMoney+progress*(maxMoney-minMoney))).ToString ();
	}
	float CountProgressVal(int inputVal)
	{
		if (maxMoney == minMoney)
			return 1f;
		return (inputVal-minMoney)*1f/(maxMoney-minMoney);
	}
	//获取最后出价
	void GetFinishMoney()
	{
		finishMoney = int.Parse(inputMoney.text);
	}
	/*void Update()
	{
		if (inputMoney.selected) {
			OnInputSubmit(inputMoney.text);	
		}
	}*/
	#region 进度条及输入价格
	private bool isActiveSlider = false;
	private bool isActiveInput = false;
	//进度条变动时，接收消息
	void OnSliderChange(float val)
	{
		/*if(isActiveInput)
		{
			return;
		}*/
		isActiveSlider = true;
		isActiveInput = false;
		SetShowProgressMoney (progressSlider.sliderValue);
	}
	//输入框变化时接收消息
	void OnInputSubmit(string inputStr)
	{
		//【注意：别在这里死循环了】
		/*if(isActiveSlider)
		{
			return;
		}*/
		int inputVal = 0;
		//判定是否输入的全是数字，如果不是把非数字去除//
		inputStr = JugdeString (inputStr);
		//当输入的值大于最大值，显示最大值
		inputVal = int.Parse (inputStr);
		if (inputVal > maxMoney) {
			inputVal = maxMoney;		
		} else if (inputVal < minMoney) {
			inputVal = minMoney;		
		}
		//更新进度条
		SetProgessInfo (CountProgressVal(inputVal));
		isActiveSlider = false;
		isActiveInput = true;
		inputMoney.text = inputVal.ToString ();
	}
	string JugdeString(string inputStr)
	{
		int count = inputStr.Length;
		string str = "";
		for (int i = 0; i < count; i++) {
			if(inputStr[i] >= '0' && inputStr[i] <= '9')
			{
				str = str + inputStr[i];
			}
		}
		if (str.Equals (""))
			str = "0";
		return str;
	}
	#endregion

	//事件点击//
	void OnClickCancelEvent(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Auction_Cancel");
		Destroy (gameObject);
	}
	void OnClickSureEvent(object obj)
	{
		GetFinishMoney ();
		DAuctionUint? auctionData = AuctionModel.Instance.GetGoodsInfo (curByIndex);
		if ((auctionData.Value.dwAcotorID == 0 && finishMoney < auctionData.Value.dwCurPrice) || (auctionData.Value.dwAcotorID != 0 && finishMoney <= auctionData.Value.dwCurPrice)) {
			//已过期
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Auction_Fail");
			UI.MessageBox.Instance.ShowTips (3, LanguageTextManager.GetString ("IDS_I27_20"), 1);
		} else if (!PlayerManager.Instance.IsMoneyEnough (finishMoney)) {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Auction_Fail");
			UI.MessageBox.Instance.ShowNotEnoughMoneyMsg (null);
		}else{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Auction_Pay");
			AuctionModel.Instance.SendAuctionRequest(curByIndex,finishMoney);
		}
		Destroy (gameObject);
	}
}
