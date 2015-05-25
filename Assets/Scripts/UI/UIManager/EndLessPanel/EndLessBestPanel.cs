using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class EndLessBestPanel : MonoBehaviour {
	public UILabel isBestLoopNumLabel ;
	public Transform rewardObj;
	public GameObject prefabIcon;
	private GameObject firstItemObj;
	private GameObject secondItemObj;
	//无奖励时
	public UILabel noRewardLabel;
	//今日挑战次数
	public UILabel todayExchangeTimesLabel;
	//开始挑战
	public SingleButtonCallBack btnStartBattle;
	private int todayLeftExchangeTimes = 0;
	private string messageBoxTip ;
	public void Init()
	{
		noRewardLabel.enabled = false;
		btnStartBattle.SetCallBackFuntion(OnBtnStartBattleClick);
		string strReward = LanguageTextManager.GetString ("IDS_I20_23");
		strReward = strReward.Replace ("\\n","\n");
		noRewardLabel.text = strReward;
		messageBoxTip = LanguageTextManager.GetString ("IDS_I20_24");
		TaskGuideBtnRegister();
	}
	/// <summary>
	/// 引导按钮注入代码
	/// </summary>
	private void TaskGuideBtnRegister()
	{
		btnStartBattle.gameObject.RegisterBtnMappingId(UI.MainUI.UIType.Trial, BtnMapId_Sub.Trial_GotoFight);
	}

	public void Show()
	{
		isBestLoopNumLabel.text = EctypeModel.Instance.historyBestLoopNum.ToString();
		//暂时写死，单属性//
		todayLeftExchangeTimes = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_ENDLESS_TIMESVALUE;
		todayExchangeTimesLabel.text = GetDownWaveStr();
		ShowReward ();
	}
	//波数获取
	private string GetDownWaveStr()
	{
		return string.Format ("{0}/{1}",CommonDefineManager.Instance.CommonDefine.EndlessDailyLimit-todayLeftExchangeTimes,CommonDefineManager.Instance.CommonDefine.EndlessDailyLimit);
	}
	//显示奖励模块
	private void ShowReward()
	{
		List<CGoodsInfo> rewardList = EctypeModel.Instance.GetAllRewardByLoopNum (EctypeModel.Instance.historyBestLoopNum);
		if (rewardList == null || rewardList.Count == 0) {
			noRewardLabel.enabled = true;
			rewardObj.gameObject.SetActive(false);
			ShowIcon(null,true,false,false);
			ShowIcon(null,false,false,false);
			return;
		}
		noRewardLabel.enabled = false;
		rewardObj.gameObject.SetActive (true);
		if (rewardList.Count == 1) {
			ShowIcon (rewardList[0],true, true, true);
			ShowIcon (null,false, false, true);
		} else {
			ShowIcon (rewardList[0],true, true, false);
			ShowIcon (rewardList[1],false, true, false);
		}
	}
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
			icon = UI.CreatObjectToNGUI.InstantiateObj(prefabIcon,rewardObj);
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
	private void OnBtnStartBattleClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EndlessStart");
	
		if(TeamManager.Instance.IsTeamExist())
		{
			TeamManager.Instance.ShowLeaveTeamTip(()=>{
				ExitTeam();
				if (todayLeftExchangeTimes <= 0) {
					UI.MessageBox.Instance.ShowTips (4, messageBoxTip, 1);
				} else {
					//EctypeModel.Instance.curEctypeState = EctypeState.EEndLess; 
					EctypeModel.Instance.SendGoBattleToServer (EctypeModel.Instance.curEndLessEctypeID);
				}
			});
		}
		else
		{
			if (todayLeftExchangeTimes <= 0) {
				UI.MessageBox.Instance.ShowTips (4, messageBoxTip, 1);
			} else {
				//EctypeModel.Instance.curEctypeState = EctypeState.EEndLess; 
				EctypeModel.Instance.SendGoBattleToServer (EctypeModel.Instance.curEndLessEctypeID);
			}
		}		
	}
	private void ExitTeam()
	{
		var playerData = PlayerManager.Instance.FindHeroDataModel();
		var teamSmg = TeamManager.Instance.MyTeamProp;
		if(playerData.ActorID == teamSmg.TeamContext.dwCaptainId)
		{
			NetServiceManager.Instance.TeamService.SendTeamDisbandMsg(new SMsgTeamDisband_CS{
				dwActorID = (uint)playerData.ActorID,
				dwTeamID = teamSmg.TeamContext.dwId
			});
		}
		else
		{
			NetServiceManager.Instance.TeamService.SendTeamMemberLeaveMsg(new SMsgTeamMemberLeave_SC(){
				dwActorID = (uint)playerData.ActorID,
				dwTeamID = teamSmg.TeamContext.dwId
			});
		}
	}
}
