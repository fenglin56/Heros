using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//奖励界面来源类型
public enum ERewardPopType{
	//vip升级
	EVipUpgrade,
	//每日签到(累积奖励弹框)
	EDailySignAccumReward,
	//每日签到(单天奖励弹框)
	EDailySignSingleReward,
}
public class VipUpgradeEffPanel : MonoBehaviour {
	public VipPrevillegeResDataBase vipPreResDataBase ;
	[HideInInspector]
	public ERewardPopType rewarPopType;
	public GameObject haveReward;
	public GameObject vipAwardItemPrefab;
	public GameObject AwardPoints;
	public SingleButtonCallBack colseBtn;
	public GameObject title;
	//每日签到
	private GameObject dailySignTitle;
	private GameObject dailySignAccumTitle;
	private UILabel labelTitle;
	private GameObject dailySignSingleTitle;
	//vip
	private GameObject vipRewardTitle;
	//奖励
	public GameObject RewardEffPoint;
	public GameObject RewardEff;
	private GameObject rewardTitleImage;
	//是否存在奖励
	private bool isHaveReward = false;
	bool isRead = false;
	void Init()
	{
		if (isRead)
			return;
		isRead = true;
		//大标题
		rewardTitleImage = RewardEffPoint.transform.Find ("Title").gameObject;
		//小标题
		dailySignTitle = title.transform.Find ("DailySign").gameObject;//
		dailySignAccumTitle = dailySignTitle.transform.Find("AccumReward").gameObject;
		labelTitle = dailySignTitle.transform.Find("AccumLabel").GetComponent<UILabel>();
		dailySignSingleTitle = dailySignTitle.transform.Find("SigleReward").gameObject;
		vipRewardTitle = title.transform.Find ("VipReward").gameObject;//

        //引导
        colseBtn.gameObject.RegisterBtnMappingId(UI.MainUI.UIType.SignIn, BtnMapId_Sub.SignIn_AwardConfirm);
		UIEventManager.Instance.RegisterUIEvent (UIEventType.CloseAllUI,OnCloseAllUIEvent);
	}
	//外部调用，传入奖励列表//
	public void Show(ERewardPopType popType,params object[] value)
	{
		Init ();
		List<VipLevelUpReward> listGoods = (List<VipLevelUpReward>)value[0];
		rewarPopType = popType;
		if (listGoods == null && listGoods.Count == 0)
			isHaveReward = false;
		else
			isHaveReward = true;
		//开始播放特效//
		FinishAct ();
		if (isHaveReward) {
			haveReward.SetActive(true);
			ShowReward(listGoods);
		} else {
			haveReward.SetActive(false);
			Invoke("ClosePanel",2);
		}
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_VIPUpgrade");
	}
	//刷新界面
	private void ShowReward(List<VipLevelUpReward> listGoods)
	{
		colseBtn.SetCallBackFuntion((obj) =>{
			//确定按钮
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SignInRewardConfirmation");
			ClosePanel();
		});
		ShowIcon (listGoods);
	}
	private void ShowIcon(List<VipLevelUpReward> listGoods)
	{
		string awardPoint = "AwardPoint";
		if (listGoods == null)
			return;
		if (listGoods.Count == 1) {
			CreateIcon(AwardPoints.transform,listGoods[0]);
		} else if (listGoods.Count == 2) {
			CreateIcon(AwardPoints.transform.Find(awardPoint+"21"),listGoods[0]);
			CreateIcon(AwardPoints.transform.Find(awardPoint+"22"),listGoods[1]);
		} else {
			CreateIcon(AwardPoints.transform.Find(awardPoint+"31"),listGoods[0]);
			CreateIcon(AwardPoints.transform,listGoods[1]);
			CreateIcon(AwardPoints.transform.Find(awardPoint+"33"),listGoods[2]);
		}
	}
	private void CreateIcon(Transform pointParent,VipLevelUpReward goodItem)
	{
		GameObject go = UI.CreatObjectToNGUI.InstantiateObj(vipAwardItemPrefab,pointParent);
		go.GetComponent<VipAwardItemIcon>().Show(goodItem.m_itemID,goodItem.m_itemCount);
	}
	//关闭
	public void ClosePanel()
	{
		if (rewarPopType == ERewardPopType.EDailySignAccumReward || rewarPopType == ERewardPopType.EDailySignSingleReward) {
			UIEventManager.Instance.TriggerUIEvent(UIEventType.DailySignSuccessPopCloseEvent, null);
		}
		PopupObjManager.Instance.RemovePopVip();
		Destroy(gameObject);
	}
	public void FinishAct()
	{
		switch (rewarPopType) {
		case ERewardPopType.EVipUpgrade:
		{
			dailySignTitle.SetActive(false);
			rewardTitleImage.SetActive(false);
			UI.CreatObjectToNGUI.InstantiateObj(RewardEff,RewardEffPoint.transform);// (RewardEffPoint, RewardEff);
			VipPrevillegeResData vipResData = vipPreResDataBase.m_dataTable[PlayerDataManager.Instance.GetPlayerVIPLevel ()];
			GameObject go = UI.CreatObjectToNGUI.InstantiateObj(vipResData.m_ipLevelIcon,RewardEffPoint.transform);
			go.transform.localPosition = new Vector3(0,0,-20);
		}
			break;
		case ERewardPopType.EDailySignAccumReward:
		{
			vipRewardTitle.SetActive(false);
			dailySignSingleTitle.SetActive(false);
			labelTitle.text = DailySignModel.Instance.GetAccumSignDays().ToString();
		}
			break;
		case ERewardPopType.EDailySignSingleReward:
		{
			vipRewardTitle.SetActive(false);
			dailySignAccumTitle.SetActive(false);
			labelTitle.enabled = false;
		}
			break;
		}
	}
	public void OnCloseAllUIEvent(object obj)
	{
		ClosePanel ();
	}
	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel (UIEventType.CloseAllUI,OnCloseAllUIEvent);
	}
}