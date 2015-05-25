/********************************************************************
	创建时间 :	2014/07/09
	创建人   :  jamfing
	功能作用 :	副本数据管理类（包括无尽试炼数据）
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CGoodsInfo{
	public int itemID;
	public int itemCount;
}

public enum EctypeState
{
	//普通副本
	ENormal,
	//宗师副本
	EDiff,
	//无尽副本
	EEndLess,
	//无副本
	ENone,
}
public class EctypeModel:ISingletonLifeCycle {
	private static EctypeModel instance;
	public static EctypeModel Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new EctypeModel();
				instance.GetEndLessID();
				SingletonManager.Instance.Add(instance);
			}
			return instance;
		}
	}
	public EctypeState curEctypeState = EctypeState.ENone;
	#region 副本数据属性
	//副本数据
	public SMSGEctypeSelect_SC sMSGEctypeSelect_SC;
	//判定该副本是否开启
	public bool IsOpenEctype(int ectypeID)
	{
		for (int i = 0; i < sMSGEctypeSelect_SC.sMSGEctypeData_SCs.Length; i++) {
			if (sMSGEctypeSelect_SC.sMSGEctypeData_SCs[i].dwEctypeContaienrID == ectypeID) {
				return true;
			}
		}
		return false;
	}
	//跳转到具体副本
	public void OpenPointToEctypePanel(int ectypeID)
	{
		//int areaEctypeID = EctypeConfigManager.Instance.GetSelectContainerID (ectypeID);
		UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Battle,ectypeID);
	}
	//计算购买体力消耗值
	public int GetActiveEnergyHaveGold()
	{
		// (向下取整 ((参数1×（购买次数）^2+参数2×购买次数+参数3)/参数4)×参数4)
		//CommonDefineManager.Instance.CommonDefine.BuyEnergyConsumption1
		int a = CommonDefineManager.Instance.CommonDefine.BuyEnergyConsumption1;
		int b = CommonDefineManager.Instance.CommonDefine.BuyEnergyConsumption2;
		int c = CommonDefineManager.Instance.CommonDefine.BuyEnergyConsumption3;
		int d = CommonDefineManager.Instance.CommonDefine.BuyEnergyConsumption4;
		int times = PlayerDataManager.Instance.GetenergyPurchaseTimes() - PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_CANBUYACTIVE_NUM;
		int val = Mathf.FloorToInt ( ((a*times*times + b*times + c)/(float)d) )*d;
		return val;
	}
	#endregion

	#region 无尽试炼数据属性
	//当前无尽试炼副本号[直接读配置表]
	[HideInInspector]
	public int curEndLessEctypeID = 0;
	public List<int> loopNumList = new List<int> ();
	//当前波数(只是指哪波要开始了)
	public int curLoopNum;
	//当前闯过的波数()
	public int passLoopNum;
	//本次闯到的最后波数(最终闯到的波数)
	public SMsgEctypeEndless_Result_SC sMsgEctypeEndless_Result_SC;
	//今日最佳波数
	public int todayBestLoopNum;
	//历史最佳波数
	public int historyBestLoopNum;
	//断线重连//
	public float againConnectTime = -1;
	#endregion

	#region 无尽试炼数据封装
	public void ClearPreEctypeWave()
	{
		loopNumList.Clear ();
	}

	public void GetEndLessID()
	{
		if (curEndLessEctypeID != 0)
			return;
		foreach (var data in EctypeConfigManager.Instance.EctypeContainerConfigList) {
			if(data.Value.lEctypeType == 10)
			{
				curEndLessEctypeID = data.Value.lEctypeContainerID;
				break;
			}
		}
	}
	//获取当前可以显示的波数
	public int GetCurCanShowWave()
	{
		return historyBestLoopNum + 5;
	}
	//服务器改成下发所通过哪些波//
	public List<CGoodsInfo> GetAllRewardByLoopNum(List<int> loopList)
	{
		if (loopList == null || loopList.Count == 0)
			return null;
		bool isHave = false;
		List<CGoodsInfo> getReward = new List<CGoodsInfo> ();
		foreach (int num in loopList) {
			List<CGoodsInfo> thisReward = GetRewardByLoopNum(num);
			//本波奖励
			foreach(CGoodsInfo thisTemp in thisReward)
			{
				isHave = false;
				//总记录奖励
				for(int j = 0 ; j < getReward.Count; j++)
				{
					if(thisTemp.itemID == getReward[j].itemID)
					{
						getReward[j].itemCount = getReward[j].itemCount + thisTemp.itemCount;
						isHave = true;
					}
				}
				if(!isHave)
				{
					CGoodsInfo temp = new CGoodsInfo();
					temp.itemID = thisTemp.itemID;
					temp.itemCount = thisTemp.itemCount;
					getReward.Add(temp);
				}
			}
		}
		/*Debug.Log ("GetAllRewardByLoopNum=="+loopNum+" RewardCount="+getReward.Count);
		if (getReward.Count == 1) {
			Debug.Log ("ID==" + getReward [0].itemID + " value=" + getReward [0].itemCount);
		} else {
			Debug.Log ("ID1==" + getReward [0].itemID + " value=" + getReward [0].itemCount+"ID2==" + getReward [1].itemID + " value=" + getReward [1].itemCount);
		}*/
		return getReward;
	}
	//获取当前波数及其以前所有奖励总和
	public List<CGoodsInfo> GetAllRewardByLoopNum(int loopNum)
	{
		if (loopNum == 0)
			return null;
		bool isHave = false;
		List<CGoodsInfo> getReward = new List<CGoodsInfo> ();
		for (int i = 1; i <= loopNum; i++) {
			List<CGoodsInfo> thisReward = GetRewardByLoopNum(i);
			//本波奖励
			foreach(CGoodsInfo thisTemp in thisReward)
			{
				isHave = false;
				//总记录奖励
				for(int j = 0 ; j < getReward.Count; j++)
				{
					if(thisTemp.itemID == getReward[j].itemID)
					{
						getReward[j].itemCount = getReward[j].itemCount + thisTemp.itemCount;
						isHave = true;
					}
				}
				if(!isHave)
				{
					CGoodsInfo temp = new CGoodsInfo();
					temp.itemID = thisTemp.itemID;
					temp.itemCount = thisTemp.itemCount;
					getReward.Add(temp);
				}
			}
		}
		/*Debug.Log ("GetAllRewardByLoopNum=="+loopNum+" RewardCount="+getReward.Count);
		if (getReward.Count == 1) {
			Debug.Log ("ID==" + getReward [0].itemID + " value=" + getReward [0].itemCount);
		} else {
			Debug.Log ("ID1==" + getReward [0].itemID + " value=" + getReward [0].itemCount+"ID2==" + getReward [1].itemID + " value=" + getReward [1].itemCount);
		}*/
		return getReward;
	}
	//根据波数获取其奖励
	public List<CGoodsInfo> GetRewardByLoopNum(int loopNum)
	{
		foreach (EndLessEctypeConfigData data in EctypeConfigManager.Instance.EndLessRewardList) {
			if(data.dwEctypeContainerId == curEndLessEctypeID && data.WaveIndex == loopNum)
			{
				return data.rewardList;
			}
		}
		return null;
	}
	//获取当前副本波数
	public List<EndLessEctypeConfigData> GetCurEctypeWave()
	{
		List<EndLessEctypeConfigData> curList = new List<EndLessEctypeConfigData> ();
		foreach (EndLessEctypeConfigData data in EctypeConfigManager.Instance.EndLessRewardList) {
			if(data.dwEctypeContainerId == curEndLessEctypeID)
			{
				curList.Add(data);
			}
		}
		return curList;
	}
	//清空当前无尽副本数据
	public void EndLessDataClear()
	{
		curLoopNum = 0;
		passLoopNum = 0;
		againConnectTime = -1;
	}
	#endregion


	#region 接收服务器数据
	//常规副本信息更新[开启扫荡后也走这里]
	public void ReceiveEctypeNomalUpdate(SMSGEctypeData_SC sMsgData)
	{
		bool isOpenSweep = sMSGEctypeSelect_SC.EctypeNormalInfoUpdate (sMsgData);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeNormalDataUpdate, isOpenSweep);
	}
	//更新副本宝箱信息
	public void ReceiveChessInfoUpdate(SMSGEctypeChest_SC sMsgData)
	{
		sMSGEctypeSelect_SC.EctypeChessInfoUpdate (sMsgData);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeChessDataUpdate, null);
	}
	//波数更新
	public void ReceiveCurLoopNumUdate(SMsgEctypeEndless_LoopNum_SC sMsgData)
	{
		//Debug.Log ("ReceiveCurLoopNumUdate==="+sMsgData.dwLoopNum);
		curLoopNum = sMsgData.dwLoopNum;
		loopNumList.Add (curLoopNum);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EndLessLoopNumUpdate, curLoopNum);
	}
	//当前闯过的波数
	public void ReceivePassLoopNumUdate(SMsgEctypeEndless_Reward_SC sMsgData)
	{
		//Debug.Log ("ReceivePassLoopNumUdate==="+sMsgData.dwLoopNum);
		passLoopNum = sMsgData.dwLoopNum;
		//发送出去的为前一次闯过的波数
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EndLessPassLoopNumUpdate, passLoopNum);
	}
	//最后结算时闯过的波数（结算界面使用）
	public void ReceiveFinishPassLoopNumUdate(SMsgEctypeEndless_Result_SC sMsgData)
	{
		sMsgEctypeEndless_Result_SC = sMsgData;
		//curFinishPassLoopNum = sMsgData.dwFinishLoopIndex;
		//Debug.Log ("ReceiveFinishPassLoopNumUdate==="+sMsgData.dwFinishLoopIndex);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EndLessFinishPassLoopData,null);
	}
	//无尽副本最佳数据【登录主动下发】
	public void ReceiveBestData(SMsgEctypeEndless_Info_SC sMsgData)
	{
		//Debug.Log ("ReceiveBestData==="+sMsgData.dwTodayBest+"hist==="+sMsgData.dwHistoryBest);
		todayBestLoopNum = sMsgData.dwTodayBest;
		historyBestLoopNum = sMsgData.dwHistoryBest;
	}
	//无尽副本最佳数据更新
	public void ReceiveBestDataUpdate(SMsgEctypeEndless_Info_Updata_SC sMsgData)
	{
		//Debug.Log ("ReceiveBestDataUpdate==="+sMsgData.wProp+" dwValue=="+sMsgData.dwValue);
		if (sMsgData.wProp == 0) {
			todayBestLoopNum = sMsgData.dwValue;	
		}
		else if (sMsgData.wProp == 1) {
			historyBestLoopNum = sMsgData.dwValue;	
		}
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EndLessBestUpdate, sMsgData.wProp);
	}
	#endregion

	#region 发送数据给服务器
	//用于进入副本处理剧情的标记, 0:未处理剧情 1：已处理剧情
	private int prEctypeID = 0;
	public void SetPreEctypeID(int ectypeID)
	{
		prEctypeID = ectypeID;
	}
	//未处理剧情时调用
	public void SendGoBattleToServer(int ectypeID)
	{
		SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS()
		{
			dwEctypeContainerID = ectypeID,
			byStory = 0,
			//dwEctypeContainerID = 31010,
		};
		prEctypeID = ectypeID;
		TraceUtil.Log(SystemModel.Jiang,"发送加入副本请求："+sMSGEctypeRequestCreate_CS.dwEctypeContainerID);
		NetServiceManager.Instance.EctypeService.SendEctypeGuideCreate(sMSGEctypeRequestCreate_CS);
	}
	//处理了剧情调用
	public void SendGoBattleToServer()
	{
		SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS()
		{
			dwEctypeContainerID = prEctypeID,
			byStory = 1,
		};
		if(prEctypeID == 0)
		{
			TraceUtil.Log("[ff0000]副本ID错误");
		}
		TraceUtil.Log(SystemModel.Jiang,"发送加入副本请求："+sMSGEctypeRequestCreate_CS.dwEctypeContainerID);
		NetServiceManager.Instance.EctypeService.SendEctypeGuideCreate(sMSGEctypeRequestCreate_CS);
	}
	//剧情播放完后，模糊请求副本
	public void StoryOverRequestEctype()
	{
		SMsgActionTeleportTo_CS sMsgActionTeleportTo_CS = new SMsgActionTeleportTo_CS (){
			byTeleportToType = 0,
		};
		NetServiceManager.Instance.EctypeService.SendStoryOverRequestEctype(sMsgActionTeleportTo_CS);
	}
	#endregion

	#region sweep
	public void SendRequestOpenSweep(int enctypeID)
	{
		SMsgEctypeUnLock_Sweep_CS sMsgEctypeUnLock_Sweep_CS = new SMsgEctypeUnLock_Sweep_CS (){
			dwEctypeContainerID = enctypeID
		};
		NetServiceManager.Instance.EctypeService.SendRequestOpenSweep(sMsgEctypeUnLock_Sweep_CS);
	}
	public void SendRequestSweep(int ectypeID,int eTimes,int selectState)
	{
		SMsgEctypeBegin_Sweep_CS sMsgEctypeBegin_Sweep_CS = new SMsgEctypeBegin_Sweep_CS (){
			dwEctypeContainerID = ectypeID,
			dwTimes = eTimes,										
			byClickType = (byte)selectState
		};
		NetServiceManager.Instance.EctypeService.SendRequestSweep(sMsgEctypeBegin_Sweep_CS);
	}
	public void SweepGetReward(SMsgEctypeResult_Sweep_SC data)
	{
		foreach(SEquipReward info in data.SEquipRewardList)
		{
			if(info.dwEquipId != 0 && info.dwEquipNum != 0)
			{
				UI.GoodsMessageManager.Instance.Show((int) info.dwEquipId, (int)info.dwEquipNum);
			}
		}
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EctypeSweepPrize");
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeSweepReward, null);
	}
	#endregion
	public void Instantiate()
	{
		
	}
	
	public void LifeOver()
	{
		instance = null;
	}
}
