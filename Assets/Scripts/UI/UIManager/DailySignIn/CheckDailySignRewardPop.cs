using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckDailySignRewardPop : MonoBehaviour {
	public GameObject haveReward;
	public GameObject awardItemPrefab;
	public GameObject AwardPoints;
	public SingleButtonCallBack colseBtn;
	public UILabel labelDays;
	//是否存在奖励
	private bool isHaveReward = false;
	//private UI.MainUI.DailySignPanel uiParent;
	bool isRead = false;
	void Init()
	{
		if (isRead)
			return;
		isRead = true;
	}
	//外部调用，传入奖励列表//
	public void Show(int accumDays,params object[] value)
	{
		Init ();
		//uiParent = uiparent;
		List<VipLevelUpReward> listGoods = (List<VipLevelUpReward>)value[0];
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
		labelDays.text = accumDays.ToString ();
	}
	//刷新界面
	private void ShowReward(List<VipLevelUpReward> listGoods)
	{
		colseBtn.SetCallBackFuntion((obj) =>{
			//确定按钮
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
		GameObject go = UI.CreatObjectToNGUI.InstantiateObj(awardItemPrefab,pointParent);
		go.GetComponent<VipAwardItemIcon>().Show(goodItem.m_itemID,goodItem.m_itemCount);
	}
	//关闭
	public void ClosePanel()
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_SignInCumulativeRewardClose");
		Destroy(gameObject);
		//uiParent.OnCloseCheckReward ();
	}
	public void FinishAct()
	{
	}
}