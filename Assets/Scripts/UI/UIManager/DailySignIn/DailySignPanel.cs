using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI{
	public class DailySignPanel : BaseUIPanel {
		public DailySignInConfigDataBase signConfigDataBase ;
		private DailySignInConfigData signConfigData;
		//查看界面预设
		public GameObject checkRewardPrefab;
		//进度面板
		public GameObject progressPanel;
		public UISprite progressFg;
		public GameObject progressFgEff;
		public SingleButtonCallBack btnBack;
		public GameObject proReward;
		private List<SignFruitReward> rewardList = new List<SignFruitReward>();
		//一周面板
		public GameObject weekPanel;
		public GameObject weekContent;
		public GameObject weekDayPrefab;
		private List<SignWeekInfo> weekList = new List<SignWeekInfo>();
		private string weekStr = "GoodsCard";
		public Transform selectMark ;
		public SingleButtonCallBack btnSign;
		private UISprite spriteBtnWord;
		public GameObject signBtnEff;
		[HideInInspector]
		public int curSelectIndex = 0;
		private bool isRead = false;

        void Awake()
        {
            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            btnBack.gameObject.RegisterBtnMappingId(UIType.SignIn, BtnMapId_Sub.SignIn_CloseIcon);
            btnSign.gameObject.RegisterBtnMappingId(UIType.SignIn, BtnMapId_Sub.SignIn_Water);
        }
		#region 
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			btnSign.SetCallBackFuntion(OnSignButtonClick);
			btnBack.SetCallBackFuntion(OnBackButtonClick);
			spriteBtnWord = btnSign.gameObject.transform.Find("Word").GetComponent<UISprite>();
			UIEventManager.Instance.RegisterUIEvent(UIEventType.DailySignAllUpdate,OnSignAllUpdate);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.DailySignResponseEvent, OnSignResponseSuccess);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.DailySignSuccessPopCloseEvent, OnCloseCheckReward);
			InitData ();
		}
		void InitData()
		{
			for (int i = 1; i <= 7; i++) {
				Transform tran = weekContent.transform.Find (weekStr+i);
				GameObject goods = NGUITools.AddChild(tran.gameObject,weekDayPrefab);
				weekList.Add(goods.gameObject.GetComponent<SignWeekInfo>());
			}
			weekDayPrefab.SetActive (false);
			ConfigDataUpdate (signConfigDataBase);
			for (int j = 0; j < 3; j++) {
				rewardList.Add (proReward.transform.Find ("Item"+j).GetComponent<SignFruitReward> ());
				SingleButtonCallBack btnCB = rewardList[j].gameObject.GetComponent<SingleButtonCallBack>();//.SetCallBackFuntion();;
				btnCB.ButtonCallBackInfo = j;
				btnCB.SetCallBackFuntion(OnClickProgressBtn);
			}
		}
		//当组ID发生变化时，调用这里
		void ConfigDataUpdate(DailySignInConfigDataBase configDataBase)
		{
			foreach (DailySignInConfigData data in configDataBase._dataTable) {
				if(data.RewardId == DailySignModel.Instance.dailySignData.dwGroupID)
				{
					signConfigData = data;
					signConfigData.GetDailyReward();
				}
			}
		}
		#endregion
		public override void Show(params object[] value)
		{
			base.Show (value);
			Init ();
			ShowProgress ();
			ShowContent (DailySignModel.Instance.dailySignData.CurDay);
		}
		#region 上端进度条
		void ProgressEff(float progressVal)
		{
			if (progressVal < 0.01f || progressVal > 0.99f) {
				progressFgEff.SetActive (false);			
			} else {
				progressFgEff.SetActive(transform);
				progressFgEff.transform.localPosition = new Vector3(849*progressVal,0,0);
			}
		}
		void ShowProgress()
		{
			float progressVal = DailySignModel.Instance.GetAccumSignDays ()*1f/7f;
			progressFg.fillAmount = 1-progressVal;
			ProgressEff (progressVal);
			rewardList[0].Show(signConfigData.CumulativeRewardDays1,LanguageTextManager.GetString("IDS_I26_4"),signConfigData.CumulativeRewardTips1);
			rewardList[1].Show(signConfigData.CumulativeRewardDays2,LanguageTextManager.GetString("IDS_I26_5"),signConfigData.CumulativeRewardTips2);
			rewardList[2].Show(signConfigData.CumulativeRewardDays3,LanguageTextManager.GetString("IDS_I26_6"),signConfigData.CumulativeRewardTips3);
		}
		void OnClickProgressBtn(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SignInCumulativeReward");
			int index = (int)obj;
			int day = rewardList [index].index;
			ShowCheckReward(day);
		}
		void ShowCheckReward(int days)
		{
			GameObject go = NGUITools.AddChild (gameObject,checkRewardPrefab);
			go.transform.localPosition = new Vector3 (0,0,-20);
			CheckDailySignRewardPop popGo = go.GetComponent<CheckDailySignRewardPop>();
			popGo.Show (days,DailySignModel.Instance.GetAccumRewardList(signConfigData,days));
		}
		//积累天数增加，播放进度条
		private float curVal = 0;
		private float endVal = 0;
		private float everyVal = 0f;
		//大概会播15下
		private float progressTime = 0.1f;
		void PlayerProgress()
		{
			int accumDays = DailySignModel.Instance.GetAccumSignDays ();
			curVal = (accumDays-1)*1f/7f;
			progressFg.fillAmount = 1-curVal;
			endVal = accumDays*1f/7f;
			everyVal = (endVal-curVal)/15f;
			if (IsInvoking ("UpdatePlayerProgress")) {
				CancelInvoke("UpdatePlayerProgress");			
			}
			InvokeRepeating ("UpdatePlayerProgress",0,progressTime);

		}
		void UpdatePlayerProgress()
		{
			curVal += everyVal;
			if (curVal > endVal) {
				CancelInvoke("UpdatePlayerProgress");
				int accumDays = DailySignModel.Instance.GetAccumSignDays ();
				if (accumDays == signConfigData.CumulativeRewardDays1||accumDays == signConfigData.CumulativeRewardDays2||accumDays == signConfigData.CumulativeRewardDays3) {
					PopupObjManager.Instance.OpenVipUpgradePanel (ERewardPopType.EDailySignAccumReward,DailySignModel.Instance.GetAccumRewardList(signConfigData,accumDays));
				}
				if(curVal > 0.99f)
				{
					curVal = 1f;
				}
			}
			ProgressEff (curVal);
			progressFg.fillAmount = 1-curVal;
		}
		#endregion

		#region 下端每日情况
		void ShowContent(int showDay)
		{
			RefreshWeekShow ();
			SetCurPaySelectMark (showDay);//DailySignModel.Instance.dailySignData.CurDay);
		}
		//刷新面板
		void RefreshWeekShow()
		{
			for (int i = 0; i < weekList.Count; i++) {
				EDailySignType signType = EDailySignType.ECanRepairSign ;
				if(i+1 > DailySignModel.Instance.dailySignData.CurDay)
				{
					signType = EDailySignType.ECanNotSign;
				}
				else if(DailySignModel.Instance.dailySignData.Sign[i]==1)
				{
					signType = EDailySignType.EAlreadySign;
				}
				else if(i+1 == DailySignModel.Instance.dailySignData.CurDay)
				{
					signType = EDailySignType.ENoneSign;
				}
				weekList[i].Show(this,i+1,signType,signConfigData);
			}
		}
		//点击签到//
		void OnSignButtonClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SignInRegist");
			NetServiceManager.Instance.EntityService.SendDailySignRequest(0,curSelectIndex);
		}

		void SetSignBtnState(SignWeekInfo weekInfo)
		{
			//UIImageButton image = btnSign.gameObject.GetComponent<UIImageButton>();
			if (weekInfo.curSignType == EDailySignType.ENoneSign) {
				btnSign.Enable = true;
				//btnSign.SetButtonBackground(1);
				spriteBtnWord.spriteName = "JH_UI_Typeface_1332";
				//image.isEnabled = true;
				signBtnEff.SetActive(true);
			} else {
				//image.isEnabled = false;
				btnSign.Enable = false;
				//btnSign.SetButtonBackground(3);
				spriteBtnWord.spriteName = "JH_UI_Typeface_1372";
				signBtnEff.SetActive(false);
			}
		}
		//点击返回
		void OnBackButtonClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SignInClose");
			Close ();
		}
		//签到成功
		void OnSignResponseSuccess(object obj)
		{
			SMsgActionDaySign_SC sMsgActionDaySign_SC = (SMsgActionDaySign_SC)obj;
			CGoodsInfo goodsInfo = signConfigData.dailyRewardList [sMsgActionDaySign_SC.SignID] [0];
			List<VipLevelUpReward> listGoods = new List<VipLevelUpReward> ();
			VipLevelUpReward reward = new VipLevelUpReward ();
			reward.m_itemID = goodsInfo.itemID;
			reward.m_itemCount = goodsInfo.itemCount;
			listGoods.Add (reward);
			ShowContent (curSelectIndex);
			PopupObjManager.Instance.OpenVipUpgradePanel (ERewardPopType.EDailySignSingleReward,listGoods);
		}
		//关闭(签到成功的奖励界面)
		void OnCloseCheckReward(object obj)
		{
			if(DailySignModel.Instance.isHaveAccumReward)
			{
				PlayerProgress();
			}
			DailySignModel.Instance.isHaveAccumReward = false;
		}
		//当选中某天时//
		public void OnWeekSelect(SignWeekInfo info)
		{
			SetCurPaySelectMark (info.weekIndex);
		}
		//设置当前选中项【1-7】
		void SetCurPaySelectMark(int index)
		{
			curSelectIndex = index;
			selectMark.parent = weekList[index-1].transform;
			selectMark.localPosition = Vector3.zero;
			SetSignBtnState (weekList[index-1]);
		}
		private int selectRepairIndex = 0;
		//点击补签//
		public void OnRepairSignBtnClick(SignWeekInfo weekInfo)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SignInRegist");
			selectRepairIndex = weekInfo.weekIndex;
			CGoodsInfo goodsInfo = signConfigData.dailyRewardList [weekInfo.weekIndex] [0];
			ItemData getItem = ItemDataManager.Instance.GetItemData(goodsInfo.itemID);
			string goodName = NGUIColor.SetTxtColor(LanguageTextManager.GetString (getItem._szGoodsName),(TextColor)getItem._ColorLevel);
			string tipStr = string.Format (LanguageTextManager.GetString("IDS_I26_20"),goodName,goodsInfo.itemCount,
			                               CommonDefineManager.Instance.CommonDefine.SignInConsumption);
			UI.MessageBox.Instance.Show (4, "", tipStr, LanguageTextManager.GetString ("IDS_H2_55"),
			                            LanguageTextManager.GetString ("IDS_H2_28"), OnBuySureClick, OnBuyCancelClick);
		}
		void OnBuyCancelClick()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SignInRegistCancel");
		}
		void OnBuySureClick()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_QuickBuyConfirmation");
			Invoke("NextMessageBox",0.1f);
		}
		void NextMessageBox()
		{
			//钱不够
			if(!PlayerManager.Instance.IsBindPayEnough(CommonDefineManager.Instance.CommonDefine.SignInConsumption))
			{
				UI.MessageBox.Instance.ShowNotEnoughGoldMoneyMsg(GoldLessSureCB);
				return;
			}
			else
			{
				NetServiceManager.Instance.EntityService.SendDailySignRequest(1, selectRepairIndex);
			}
		}
		void GoldLessSureCB()
		{

		}
		#endregion
		void OnSignAllUpdate(object obj)
		{
			if (!IsShow)
				return;
			ConfigDataUpdate (signConfigDataBase);
			Show (null);		
		}
		public override void Close()
		{
			if (!IsShow)
				return;
			base.Close();
		}
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.DailySignAllUpdate,OnSignAllUpdate);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.DailySignResponseEvent, OnSignResponseSuccess);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.DailySignSuccessPopCloseEvent, OnCloseCheckReward);
		}
	}
}