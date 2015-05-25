using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EndLessSceneResultPanel : MonoBehaviour {
	public GameObject effObj;
	//2
	public GameObject centerObj;
	public UILabel labelSuccTip;
	public GameObject iconParent;
	public GameObject prefabIcon;
	//3
	public SingleButtonCallBack btnBack;
	public void Init()
	{
		btnBack.SetCallBackFuntion(OnBtnBackClick);
		centerObj.SetActive (false);
		btnBack.gameObject.SetActive (false);
		prefabIcon.SetActive (false);
	}
	public void Show()
	{
		NGUITools.AddChild(gameObject, effObj);
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EndlessResultIntro");
		Invoke ("ShowCenter",1);
	}
	void ShowCenter()
	{
		centerObj.SetActive (true);
		labelSuccTip.text = string.Format (LanguageTextManager.GetString ("IDS_I20_20"),EctypeModel.Instance.sMsgEctypeEndless_Result_SC.dwFinishLoopIndex);
		ShowReward ();
		Invoke ("ShowBtnBack",1.17f);
	}
	void ShowBtnBack()
	{
		btnBack.gameObject.SetActive (true);
	}


	//显示奖励模块
	private void ShowReward()
	{
		List<CGoodsInfo> rewardList = EctypeModel.Instance.GetAllRewardByLoopNum (EctypeModel.Instance.sMsgEctypeEndless_Result_SC.passLoopList);
		if (rewardList == null || rewardList.Count == 0) {
			ShowIcon(null,true,false,false);
			ShowIcon(null,false,false,false);
			return;
		}
		if (rewardList.Count == 1) {
			ShowIcon (rewardList[0],true, true, true);
			ShowIcon (null,false, false, true);
		} else {
			ShowIcon (rewardList[0],true, true, false);
			ShowIcon (rewardList[1],false, true, false);
		}
	}
	private GameObject firstItemObj;
	private GameObject secondItemObj;
	private void ShowIcon(CGoodsInfo reward,bool isFirst,bool isShow,bool isOnlyOne)
	{
		GameObject icon = firstItemObj;
		Vector3 pos = prefabIcon.transform.localPosition;
		ItemIconInfo info;
		if (!isFirst) {
			icon = secondItemObj;
		}
		if (!isShow && icon == null) {
			return;
		}
		if (icon == null) {
			prefabIcon.SetActive (true);
			icon = UI.CreatObjectToNGUI.InstantiateObj(prefabIcon,iconParent.transform);
			info = icon.GetComponent<ItemIconInfo>();
			if(reward != null)
			{
				ItemData getItem = ItemDataManager.Instance.GetItemData(reward.itemID);
				info.Init(getItem,reward.itemCount.ToString());
			}
			prefabIcon.SetActive (false);
		}
		if (isShow) {
			info = icon.GetComponent<ItemIconInfo>();
			if(reward != null)
			{
				ItemData getItem = ItemDataManager.Instance.GetItemData(reward.itemID);
				info.Show(getItem,reward.itemCount.ToString());
			}
			if (isOnlyOne) {
				pos = Vector3.zero;
			} else {
				if (!isFirst) {
					pos = new Vector3 (-1 * pos.x, pos.y, pos.z);
				}
			}
			icon.transform.localPosition = pos;
			icon.SetActive (true);
		} else {
			icon.SetActive (false);
		}
	}
	void OnBtnBackClick(object obj)
	{
		//返回城镇
		long UIDEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
		NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(UIDEntity);
		EctypeModel.Instance.EndLessDataClear ();
		SoundManager.Instance.PlaySoundEffect("Sound_Button_EndlessBack");
	}
}
