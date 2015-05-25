using UnityEngine;
using System.Collections;

public class VipShowPanel : MonoBehaviour {
	//特效
	public GameObject jH_Eff_UI_VIP_BarFlashing;
	public GameObject jH_Eff_UI_VIP_BarGrowth;
	public UILabel curGradeLabel;
	public GameObject curVipSpParent;
	private GameObject curVipSp;
	public UILabel maxGradeLabel;
	public GameObject nextTip;
	public UILabel nextVipTip1;
	public UILabel nextVipTip2;
	public UILabel nextVipMoney;
	public GameObject nextVipSpParent;
	private GameObject nextVipSp;
	public SingleButtonCallBack vipBtn;
	//private SpriteSwith spriteBtn;
	public GameObject progressObj;
	public UILabel progressLabel;
	private int maxProgressValue = 658; 
	public UISprite progressFarSprite;
	private int curVipLev = 0;
	private UI.MainUI.TopUpPanel_V2 uiParent;
	//界面移动//
	private Vector3 originalPos ;
	private Vector3 movePos = new Vector3(51,0,0);
	private float moveTime = 0.167f;
	//进度条移动
	public float effBarFlasingTime = 0.33f;
	public float effProgressTime = 0.33f;
	public float effBarGrowthTime = 0.33f;
	public void Init(UI.MainUI.TopUpPanel_V2 parent)
	{
		uiParent = parent;
		curGradeLabel.text = LanguageTextManager.GetString ("IDS_I4_23");
		maxGradeLabel.text = LanguageTextManager.GetString ("IDS_I4_6");
		nextVipTip1.text = LanguageTextManager.GetString ("IDS_I4_1");
		nextVipTip2.text = LanguageTextManager.GetString ("IDS_I4_22");
		vipBtn.SetCallBackFuntion(OnChangeVipViewClick);
		originalPos = transform.localPosition;
		preVipLev = PlayerDataManager.Instance.GetPlayerVIPLevel ();
		//spriteBtn = vipBtn.gameObject.GetComponent<SpriteSwith>();//vipBtn.gameObject.transform.Find ("SpriteWord").GetComponent<UISprite>();
	}
	//触发情况
	public void Show(bool isTweenMark)
	{
		curVipLev = PlayerDataManager.Instance.GetPlayerVIPLevel ();
		int nextLev = GetNextVipLev ();
		VipPrevillegeResData vipResData = uiParent.vipPreResDataBase.m_dataTable[curVipLev];
		SetCurVip (curVipLev,vipResData);
		if (nextLev == 0) {
			nextTip.SetActive (false);
			maxGradeLabel.enabled = true;
			SetProgress (curVipLev,false);
		} else {
			nextTip.SetActive (true);
			maxGradeLabel.enabled = false;
			VIPConfigData curVipData = PlayerDataManager.Instance.GetVipData(curVipLev);
			int preMoney = curVipData.m_upgradeExp;
			VIPConfigData vipData = PlayerDataManager.Instance.GetVipData(nextLev);
			VipPrevillegeResData resData = uiParent.vipPreResDataBase.m_dataTable[nextLev];
			SetNextVip(nextLev,vipData.m_upgradeExp-preMoney,resData);
			SetProgress (nextLev,false);
		}
		if (isTweenMark) {
			TweenShow();
			//SoundManager.Instance.PlaySoundEffect("Sound_UIEff_VIPUIAppear");
		}
		vipBtn.SetButtonBackground (uiParent.isCurPayPanel?1:2);
		//spriteBtn.ChangeSprite(uiParent.isCurPayPanel ?1:2);//.spriteName = uiParent.isCurPayPanel ? "JH_UI_Typeface_1313" : "JH_UI_Typeface_1313";
	}
	int GetNextVipLev()
	{
		curVipLev = PlayerDataManager.Instance.GetPlayerVIPLevel ();
		VipPrevillegeResData vipResData = uiParent.vipPreResDataBase.m_dataTable[curVipLev];
		if (curVipLev >= PlayerDataManager.Instance.GetVIPMaxLevel ()) {
			return 0;
				} else {
			return curVipLev+1;
		}
	}
	//显示当前vip图标
	void SetCurVip(int newLev,VipPrevillegeResData resData)
	{
		DestroyImmediate (curVipSp);
		curVipSp = UI.CreatObjectToNGUI.InstantiateObj (resData.m_ipLevelIcon,curVipSpParent.transform);
	}
	//显示下一等级
	void SetNextVip(int newLev,int goldCount,VipPrevillegeResData resData)
	{
		DestroyImmediate (nextVipSp);
		nextVipSp = UI.CreatObjectToNGUI.InstantiateObj (resData.m_ipLevelIcon,nextVipSpParent.transform);
		nextVipMoney.text = goldCount.ToString ();
	}
	void ShowProgressRate(int vipLev)
	{
		VIPConfigData vipData = PlayerDataManager.Instance.GetVipData(vipLev);
		int money = PlayerDataManager.Instance.GetCurVipPayMoney ();
		progressLabel.text = string.Format("{0}/{1}",money,vipData.m_upgradeExp);
	}
	void SetProgressFg(float progressVal)
	{
		if(progressVal > (float)maxProgressValue)
			progressVal = (float)maxProgressValue;
		Vector3 sale = progressFarSprite.gameObject.transform.localScale;
		progressFarSprite.gameObject.transform.localScale = new Vector3(progressVal,sale.y,sale.z);
	}
	void FinishProgress()
	{
		int money = PlayerDataManager.Instance.GetCurVipPayMoney ();
		VIPConfigData vipData = PlayerDataManager.Instance.GetVipData(PlayerDataManager.Instance.GetPlayerVIPLevel ()+1);
		float progressVal = 1f;
		if (vipData != null) {
			progressVal = (money * 1f / vipData.m_upgradeExp);
		}
		if (progressVal > 0.99f) {
			progressVal = 1f;		
		}
		progressVal = progressVal*maxProgressValue;
		SetProgressFg (progressVal);
	}
	//200/300比例值及进行条
	void SetProgress(int nextLev,bool isUpgrade)
	{
		ShowProgressRate (nextLev);
		VIPConfigData vipData = PlayerDataManager.Instance.GetVipData(nextLev);
		int money = PlayerDataManager.Instance.GetCurVipPayMoney ();
		float progressVal = (money * 1f / vipData.m_upgradeExp)*maxProgressValue;
		if (!isUpgrade)
			SetProgressFg (progressVal);
	}

	private GameObject effFlash = null;
	private GameObject effGrowth = null;
	private int nextVipLev = 0;
	private int progressVipLev = 0;
	private float startProVal = 0;
	private float endProVal ;
	private float allProVal;
	private int progressCount = 0;
	#region 当充值成功时播放动画
	public void StartProgressEff()
	{
		SetPaySuccessPlayEff ();
		PlayProgressFlash ();
	}
	//[当充值成功时播放动画]播放第一段
	void PlayProgressFlash()
	{
		effFlash = NGUITools.AddChild(progressObj, jH_Eff_UI_VIP_BarFlashing);
		Invoke ("PlayOverFlash",effBarFlasingTime);
	}
	void PlayOverFlash()
	{
		//SetProgress (nextVipLev,true);
		VipPayMoneyPlayEff ();
	}
	//播放进度条
	void PlayerProgress()
	{
		if (IsInvoking ("UpdateProgress")) {
			CancelInvoke("UpdateProgress");		
		}
		progressCount = 0;
		InvokeRepeating ("UpdateProgress",0,0.033f);
	}
	//播放第三段
	void PlayProgressGrowth()
	{
		effGrowth = NGUITools.AddChild(progressObj, jH_Eff_UI_VIP_BarGrowth);
		Invoke ("PlayOverGrowth",1);
	}
	
	void PlayOverGrowth()
	{
		DestroyImmediate (effFlash);
		DestroyImmediate (effGrowth);
	}
	private int preVipLev = 0;
	//private float preVipProgress = 0;
	private void SetPaySuccessPlayEff()
	{
		progressVipLev = PlayerDataManager.Instance.GetPlayerVIPLevel ();
		startProVal = progressFarSprite.gameObject.transform.localScale.x / maxProgressValue;
		//如果没升级//
		if (preVipLev >= progressVipLev) {
			preVipLev = progressVipLev;
			progressVipLev = progressVipLev + 1;
			VIPConfigData curVipData = PlayerDataManager.Instance.GetVipData(progressVipLev);
			endProVal = (PlayerDataManager.Instance.GetCurVipPayMoney () * 1f) / curVipData.m_upgradeExp;
		} else {
			endProVal = 1;
			preVipLev = progressVipLev;
		}
	}
	//先发vip升级再发充值成功//
	private void VipPayMoneyPlayEff()
	{
		if (endProVal >= 1)
			endProVal = 1;
		allProVal = endProVal - startProVal;
		if (IsInvoking ("PlayerMoveProgress")) {
			CancelInvoke("PlayerMoveProgress");		
		}
		progressCount = 0;
		InvokeRepeating ("PlayerMoveProgress",0,0.033f);
	}
	//开始播放进度条
	private void PlayerMoveProgress()
	{
		if (progressCount > 10) {
			progressCount = 10;
			CancelInvoke("PlayerMoveProgress");
			ShowProgressRate(PlayerDataManager.Instance.GetPlayerVIPLevel ()+1);
			FinishProgress();
			PlayProgressGrowth ();
			return;
		}
		UpdateShowProgress ();
	}
	void UpdateShowProgress()
	{
		progressCount++;
		float rateVal = progressCount * 1f / 10;
		float curVal = startProVal + allProVal * rateVal;
		if (curVal > 1.0f) {
			curVal = 1;
		}
		SetProgressFg (maxProgressValue*curVal);
	}


	#endregion

	#region 当升级时更新界面
	public void VipUpgradeSuccess()
	{
		Show (false);
	}
	#endregion

	void VipProgress()
	{
		//nextVipLev = nextLev;
		PlayProgressFlash ();
	}

	void OnChangeVipViewClick(object obj)
	{
		if (uiParent.isCurPayPanel) {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_VIPRecharge");
				} else {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_VIPPrivilege");
		}
		uiParent.isCurPayPanel = !uiParent.isCurPayPanel;
		vipBtn.SetButtonBackground (uiParent.isCurPayPanel?1:2);
		uiParent.OnVipShowButtonClick(uiParent.isCurPayPanel);
		//spriteBtn.ChangeSprite(uiParent.isCurPayPanel ?1:2);//spriteBtn.spriteName = uiParent.isCurPayPanel ? "JH_UI_Typeface_1313" : "JH_UI_Typeface_1313";
	}
	public void TweenShow()
	{
		TweenPosition.Begin(gameObject,moveTime,originalPos-movePos,originalPos);
	}
	
	public void TweenClose()
	{
		TweenPosition.Begin(gameObject,moveTime,originalPos,originalPos-movePos);
	}
}
