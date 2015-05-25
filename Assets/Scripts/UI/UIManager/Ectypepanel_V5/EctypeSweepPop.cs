using UnityEngine;
using System.Collections;
namespace UI.MainUI{
	public class EctypeSweepPop : MonoBehaviour {
		public SingleButtonCallBack btnColse;
		public UILabel vipLabel;
		public UILabel sweepTopTimesLabel;
		public SingleButtonCallBack selectLeftBtn;
		public SingleButtonCallBack selectRightBtn;
		public SingleButtonCallBack sweepTokenBtn;
		//进度条
		public UILabel showTimesLabel;
		public UISlider progressSlider;
		private BoxCollider progressBoxCollider;
		private int minTimes;
		private int maxTimes;
		public UILabel sweepTip;
		//下方
		public UILabel normalMoney;
		public SingleButtonCallBack normalBtn;
		public UILabel normalBtnWord;
		public UILabel middleMoney;
		public SingleButtonCallBack middleBtn;
		public UILabel middleBtnWord;
		public UILabel middleTip;
		public GameObject highGrade;
		public UILabel highMoney;
		public SingleButtonCallBack highGradeBtn;
		public UILabel highGradeBtnWord;
		public UILabel highTip;
		private int curVipLevel;
		private int curSelectTimes;
		private EctypeContainerListPanel MyParent ;
		private int costActive = 0;
		private int costMoney = 0;
		private int preSweepValue = 0;
		// Use this for initialization
		private bool isRead = false;
		void Init () {
			if (isRead)
				return;
			isRead = true;
			progressBoxCollider = progressSlider.GetComponent<BoxCollider>();
			middleTip.text = LanguageTextManager.GetString ("IDS_I34_24");
			highTip.text = LanguageTextManager.GetString ("IDS_I34_25");
			normalBtnWord.text = LanguageTextManager.GetString ("IDS_I34_21");
			middleBtnWord.text = LanguageTextManager.GetString ("IDS_I34_22");
			highGradeBtnWord.text = LanguageTextManager.GetString ("IDS_I34_23");
			btnColse.SetCallBackFuntion (OnBtnColseEvent);
			selectLeftBtn.SetCallBackFuntion (OnselectLeftBtnEvent);
			selectRightBtn.SetCallBackFuntion (OnselectRightBtnEvent);
			sweepTokenBtn.SetCallBackFuntion (OnSweepTokenBtnEvent);
			normalBtn.SetCallBackFuntion (OnnormalBtnEvent);
			middleBtn.SetCallBackFuntion (OnmiddleBtnEvent);
			highGradeBtn.SetCallBackFuntion (OnhighGradeBtnEvent);
			progressSlider.onValueChange = OnSliderChange;
			preSweepValue = PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SWEEP_VALUE;
			UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeSweepReward, OnEctypeSweepRewardEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.SweepTimesUpdate, OnEctypeSweepUpdateEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ActiveLifeUpdate, OnEctypeSweepUpdateEvent);
		}
		public void Show(EctypeContainerListPanel myParent)
		{
			gameObject.SetActive (true);
			MyParent = myParent;
			Init ();
			ShowPanel ();
		}
		void ShowPanel()
		{
			curVipLevel = PlayerDataManager.Instance.GetPlayerVIPLevel ();
			vipLabel.text = curVipLevel.ToString();
			VIPConfigData curVipData = PlayerDataManager.Instance.GetVipData(curVipLevel);
			int leftTimes = PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SWEEP_VALUE;
			preSweepValue = leftTimes;
			sweepTopTimesLabel.text = (leftTimes == 0 ? "[FF0000]"+leftTimes+"[-]":leftTimes.ToString())+"/"+curVipData.VipSweepNum;

			EctypeContainerData esyData;
			EctypeConfigManager.Instance.EctypeContainerConfigList.TryGetValue(MyParent.curSelectEasyEctypeID,out esyData);
			costActive = int.Parse(esyData.lCostEnergy);
			costMoney = esyData.ByCost;
			int canTimesAct = PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE/costActive;
			maxTimes = PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SWEEP_VALUE > canTimesAct ? canTimesAct 
				: PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SWEEP_VALUE;//剩余次数
			if (maxTimes != 0) {
				curSelectTimes = 1;
				minTimes = 1;//
				progressSlider.sliderValue = curSelectTimes * 1f / maxTimes;
			} else {
				curSelectTimes = 0;
				minTimes = 0;//
				progressSlider.sliderValue = 0 ;
			}
			ScrollBar ();
		}
		void ScrollBar()
		{
			//可以拖动
			selectLeftBtn.Enable = true;
			selectRightBtn.Enable = true;
			progressBoxCollider.enabled = true;
			//当次数为0，最多剩下0，体力不够一次//
			if(maxTimes == 0)
			{
				selectLeftBtn.Enable = false;
				selectRightBtn.Enable = false;
				progressBoxCollider.enabled = false;
			}
			if(curVipLevel < 5)
			{
				highGrade.SetActive(false);
				highGradeBtn.Enable = false;
			}
			else
			{
				highGrade.SetActive(true);
				highGradeBtn.Enable = true;
			}
			if (PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE < costActive) {
				sweepTip.transform.parent.gameObject.SetActive (true);
				sweepTip.text = LanguageTextManager.GetString ("IDS_I34_5");
			} else if (PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SWEEP_VALUE == 0) {
				sweepTip.transform.parent.gameObject.SetActive (true);
				sweepTip.text = LanguageTextManager.GetString ("IDS_I34_6");
			} else {
				sweepTip.transform.parent.gameObject.SetActive (false);
			}
			normalMoney.text = "x" + costActive * curSelectTimes;
			middleMoney.text = "x" + costMoney * curSelectTimes;
			highMoney.text = "x" + costMoney * 2 * curSelectTimes;
			showTimesLabel.text = curSelectTimes.ToString ();



			/*if (curSelectTimes == 0 || maxTimes == 0 || PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE < costActive) {
				if(PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE < costActive)
				{
					sweepTip.transform.parent.gameObject.SetActive(true);
					sweepTip.text = LanguageTextManager.GetString("IDS_I34_5");
					normalBtn.Enable = false;
					middleBtn.Enable = false;
					highGradeBtn.Enable = false;
				}
				else if(PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SWEEP_VALUE == 0)
				{
					sweepTip.transform.parent.gameObject.SetActive(true);
					sweepTip.text = LanguageTextManager.GetString("IDS_I34_6");
				}
				else
				{
					sweepTip.transform.parent.gameObject.SetActive(false);
				}
			} else {
				if(curVipLevel < 5)
				{
					highGrade.SetActive(false);
					highGradeBtn.Enable = false;
				}
				else
				{
					highGradeBtn.Enable = true;
					highGrade.SetActive(true);
				}
				normalBtn.Enable = true;
				middleBtn.Enable = true;
			}
			normalMoney.text = "x" + costActive * curSelectTimes;
			middleMoney.text = "x" + costMoney * curSelectTimes;
			highMoney.text = "x" + costMoney * 2 * curSelectTimes;
			showTimesLabel.text = curSelectTimes.ToString ();
			*/
		}
		void OnBtnColseEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Sweep_Leave");
			gameObject.SetActive (false);
		}
		void OnselectLeftBtnEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Sweep_Choose");
			if (curSelectTimes <= minTimes) {
				curSelectTimes = minTimes;			
			} else {
				curSelectTimes--;
			}
			progressSlider.sliderValue = curSelectTimes*1f / maxTimes;
			//ScrollBar ();
		}
		void OnselectRightBtnEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Sweep_Choose");
			if (curSelectTimes >= maxTimes) {
				curSelectTimes = maxTimes;			
			} else {
				curSelectTimes++;
			}
			progressSlider.sliderValue = curSelectTimes*1f / maxTimes;
			//ScrollBar ();
		}
		void OnSweepTokenBtnEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SweepNumUse");
			int goodsCount = ContainerInfomanager.Instance.GetOwnMaterialCount (CommonDefineManager.Instance.CommonDefine.SweepNumID);
			if (goodsCount > 0) {
				//使用物品
				ContainerInfomanager.Instance.UseUsableGoods (CommonDefineManager.Instance.CommonDefine.SweepNumID,false);
			} else {
				MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I34_9"),1);
			}
		}
		//进度条变动时，接收消息
		void OnSliderChange(float val)
		{
			float temp = 0.001f;
			float vale = (progressSlider.sliderValue<temp?temp:progressSlider.sliderValue) * (maxTimes - 0);
			curSelectTimes = Mathf.CeilToInt(vale);
			ScrollBar ();
		}
		bool DealClickSweepBtn()
		{
			int sweepTimes = PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SWEEP_VALUE;
			int goodsCount = ContainerInfomanager.Instance.GetOwnMaterialCount (CommonDefineManager.Instance.CommonDefine.SweepNumID);
			int vit1Count = ContainerInfomanager.Instance.GetOwnMaterialCount (CommonDefineManager.Instance.CommonDefine.VitNumID1);
			int vit2Count = ContainerInfomanager.Instance.GetOwnMaterialCount (CommonDefineManager.Instance.CommonDefine.VitNumID2);
			int vit3Count = ContainerInfomanager.Instance.GetOwnMaterialCount (CommonDefineManager.Instance.CommonDefine.VitNumID3);
			if (PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE < costActive) {
				if(vit1Count != 0)
				{
					UseGoodsPanel.Instance.Show(CommonDefineManager.Instance.CommonDefine.VitNumID1);
				}
				else if(vit2Count != 0)
				{
					UseGoodsPanel.Instance.Show(CommonDefineManager.Instance.CommonDefine.VitNumID2);
				}
				else if(vit3Count != 0)
				{
					UseGoodsPanel.Instance.Show(CommonDefineManager.Instance.CommonDefine.VitNumID3);
				}
				else
				{
					MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I34_5"),1);
				}
				return false;
			}
			if (sweepTimes == 0 && goodsCount == 0) {
				MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I34_6"),1);
				return false;
			}
			if (sweepTimes == 0 && goodsCount > 0) {
				UseGoodsPanel.Instance.Show(CommonDefineManager.Instance.CommonDefine.SweepNumID);
				return false;
			}
			if (curSelectTimes <= 0)
				return false;
			return true;
		}
		void OnnormalBtnEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Sweep_Go");
			if (!DealClickSweepBtn ())
				return;
			EctypeModel.Instance.SendRequestSweep (MyParent.curSelectEasyEctypeID,curSelectTimes,0);
		}
		void OnmiddleBtnEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Sweep_Go");
			if (!DealClickSweepBtn ())
				return;
			if(PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_BINDPAY < costMoney*curSelectTimes)
			{
				MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I34_7"), 1);
				return;
			}
			EctypeModel.Instance.SendRequestSweep (MyParent.curSelectEasyEctypeID,curSelectTimes,1);
		}
		void OnhighGradeBtnEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Sweep_Go");
			if (!DealClickSweepBtn ())
				return;
			if(PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_BINDPAY < costMoney*2*curSelectTimes)
			{
				MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I34_7"), 1);
				return;
			}
			EctypeModel.Instance.SendRequestSweep (MyParent.curSelectEasyEctypeID,curSelectTimes,2);
		}
		void OnEctypeSweepRewardEvent(object obj)
		{
			gameObject.SetActive (false);
		}
		void OnEctypeSweepUpdateEvent(object obj)
		{
			if (preSweepValue < PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_SWEEP_VALUE) {
				MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I34_8"), 1);	
			}
			ShowPanel ();
		}
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeSweepReward, OnEctypeSweepRewardEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.SweepTimesUpdate, OnEctypeSweepUpdateEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ActiveLifeUpdate, OnEctypeSweepUpdateEvent);
		}
	}
}
