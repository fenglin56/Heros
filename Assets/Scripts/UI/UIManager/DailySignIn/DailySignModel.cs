using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DailySignModel:ISingletonLifeCycle {
	private static DailySignModel instance;
	public static DailySignModel Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new DailySignModel();
				instance.Init();
				SingletonManager.Instance.Add(instance);
			}
			return instance;
		}
	}
	void Init()
	{
		UIEventManager.Instance.RegisterUIEvent(UIEventType.TownUIBtnLoadComplete, TownUISceneLoadComplete);
	}
	//当选择当前组ID(服务器下发)
	public SMsgActionDaySignUI_SC dailySignData;
	//签到成功时，是否存在积累天数奖励//
	public bool isHaveAccumReward = false;

	//今天是否第一次登录(给登录是检测今天如果没有签到主动弹出签到界面)
	public bool isFirstPopDailySign = false;
	#region 封装
	//根据累计天数，计算其奖励
	public List<VipLevelUpReward> GetAccumRewardList(DailySignInConfigData signConfigData,int days)
	{
		List<VipLevelUpReward> rewardList = new List<VipLevelUpReward> ();
		List<CGoodsInfo> goodsList = signConfigData.accumRewardList [days];
		foreach (CGoodsInfo info in goodsList) {
			VipLevelUpReward reward = new VipLevelUpReward();
			reward.m_itemID = info.itemID;
			reward.m_itemCount = info.itemCount;
			rewardList.Add(reward);
		}
		return rewardList;
	}
	//设置签到信息
	public void SetSignDaysInfo(int day ,byte mark ){
		dailySignData.Sign [day - 1] = mark;
	}
	//获取当前累积签到天数
	public int GetAccumSignDays()
	{
		int count = 0;
		for (int i = 0; i < dailySignData.Sign.Length; i++) {
			if(dailySignData.Sign[i] == 1)
			{
				count++;
			}
		}
		return count;
	}
	//判定当天是否签到[1-7]
	public bool isSignToday(int day)
	{
		return day <= dailySignData.CurDay && dailySignData.Sign[day-1] == 1;
	}
	#endregion

	//签到请求回应
	public void SetSignResponse(SMsgActionDaySign_SC sMsgActionDaySign_SC)
	{
		//当成功时
		if (sMsgActionDaySign_SC.bSucess == 1) {
			int preCount = GetAccumSignDays();
			SetSignDaysInfo(sMsgActionDaySign_SC.SignID,sMsgActionDaySign_SC.bSucess);
			int curCount = GetAccumSignDays();
			if(preCount != curCount)
			{
				isHaveAccumReward = true;
			}
			else
			{
				isHaveAccumReward = false;
			}
			if(sMsgActionDaySign_SC.SignID == dailySignData.CurDay)
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UI.MainUI.UIType.SignIn);
				isFirstPopDailySign = false;
			}
			UIEventManager.Instance.TriggerUIEvent(UIEventType.DailySignResponseEvent,sMsgActionDaySign_SC);
		}
	}
	//界面是否load完毕
	public bool isLoadedDailySign;
	//服务器消息是否发送过来
	public bool isRecivedDailySign;
	//每天首次登录时，检测是否要主动弹出签到界面
	public void PopDailySignPanel(bool isServerData)
	{
		if (isFirstPopDailySign)
			return;
		//接收到服务器消息
		if (isServerData && isLoadedDailySign) {
			if(!OpenDailySignPanel())
				isRecivedDailySign = true;
		} else if(isServerData){
			isRecivedDailySign = true;
		}
		////接收到界面加载完毕消息
		if (!isServerData && isRecivedDailySign) {
			if(!OpenDailySignPanel())
				isLoadedDailySign = true;
		} else if(!isServerData){
			isLoadedDailySign = true;
		}
	}
	bool OpenDailySignPanel()
	{
		if (dailySignData.Sign [dailySignData.CurDay - 1] == 0 && PlayerDataManager.Instance.CanPopTip(EViewType.EDailySign)&& 
		    GameManager.Instance.GetNewWorldMsg.byTeleportFlg == (int)eTeleportType.TELEPORTTYPE_FIRST ) {
			isFirstPopDailySign = true;
			UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UI.MainUI.UIType.SignIn);
			UI.MainUI.MainUIController.Instance.OpenMainUI (UI.MainUI.UIType.SignIn, null);
			return true;
		}
		UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UI.MainUI.UIType.SignIn);
		return false;
	}
	void TownUISceneLoadComplete(object obj)
	{
		if (isFirstPopDailySign) {
			UIEventManager.Instance.TriggerUIEvent (UIEventType.PlayMainBtnAnim, UI.MainUI.UIType.SignIn);
		} else {
			UIEventManager.Instance.TriggerUIEvent (UIEventType.StopMainBtnAnim, UI.MainUI.UIType.SignIn);		
		}
		JudgeReachCondition ();
	}

	#region 每日活动
	private bool isReceiveServerData = false;
	//记录活动是否可领取
	public bool preHaveActivityCanGetReward = false;
	public float serverDataStartTime ;
	public readonly int levelActivityID = 102;
	//需要倒计时的活动ID
	public readonly int timerActivityID = 103;
	//<活动ID><0没达到条件不能领;1可以领取;2到顶了不能领>
	public Dictionary<int,int> reachConditionMap = new Dictionary<int, int> ();
	public int curSelectActivityID;
	public SMsgInteract_OpenUI sActiveMsgInteract_OpenUI;
	public void OnPlayerUpdate()
	{
		if (!isReceiveServerData)
			return;
		//等级更新
		int index = GetActiveRewardSub (levelActivityID);
		sActiveMsgInteract_OpenUI.activeList [index].dwActiveParam = PlayerManager.Instance.FindHeroDataModel ().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
		JudgeReachCondition ();
		UIEventManager.Instance.TriggerUIEvent(UIEventType.ActivityTimeUpdate,null);
	}
	public void ReveiveServerData(SMsgInteract_OpenUI msg)
	{
		isReceiveServerData = true;
		sActiveMsgInteract_OpenUI = msg;
		serverDataStartTime = Time.realtimeSinceStartup;
		/*int index = GetActiveRewardSub (timerActivityID);
		//把分数转化成秒,本身就是s
		int serverTime = DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList [index].dwActiveParam;
		DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList [index].dwActiveParam = serverTime * 60;*/
		UIEventManager.Instance.TriggerUIEvent(UIEventType.ReceiveActivityDataEvent,null);
	}
	//活动领取完成,更新数据
	public void UpdateActiveData(SMsgInteract_GetReward_SC data)
	{
		int index = GetActiveRewardSub (data.dwRewardID);
		if (data.byIndex != 0) {
			sActiveMsgInteract_OpenUI.activeList[index].byIndex = data.byIndex;
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ActivityRewardEvetn,data);
		}
	}
	//获取列表中的下标
	public int GetActiveRewardSub(int activeID)
	{
		for(int i = 0 ; i < sActiveMsgInteract_OpenUI.activeList.Length; i++)
		{
			if(sActiveMsgInteract_OpenUI.activeList[i].dwActiveID == activeID)
			{
				return i;
			}
		}
		return 0;
	}

	public int GetActivityMaxIndex(int activityID)
	{
		//int classVocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
		ActivityConfigData activityConfig = PlayerDataManager.Instance.GetActivityData(activityID);
		return activityConfig.rewardList.Count;
	}
	//获取当前显示index
	//返回值为是否领取过
	public bool GetCurActShowIndex(int activityID,ref int curIndex)
	{
		ActivityConfigData activityConfig = PlayerDataManager.Instance.GetActivityData(activityID);
		DGameActiveData actData = sActiveMsgInteract_OpenUI.activeList[GetActiveRewardSub (activityID)];
		if (actData.byIndex >= activityConfig.rewardList.Count) {
			curIndex = actData.byIndex;
			return true;
		}
		curIndex = actData.byIndex + 1;
		return false;
	}
	//倒计时
	public void ActivityTimeUpdate()
	{
		if (!isReceiveServerData)
			return;
		int index = GetActiveRewardSub (timerActivityID);
		int tempTime = (int)(Time.realtimeSinceStartup - serverDataStartTime);
		serverDataStartTime = Time.realtimeSinceStartup;
		sActiveMsgInteract_OpenUI.activeList[index].dwActiveParam += tempTime;
		JudgeReachCondition ();
		UIEventManager.Instance.TriggerUIEvent(UIEventType.ActivityTimeUpdate,null);
	}
	//判定是否达到领取条件
	public bool JudgeReachCondition()
	{
		if (!isReceiveServerData)
			return false;
		bool isCanReach = false;
		foreach (DGameActiveData actData in sActiveMsgInteract_OpenUI.activeList) {
			ActivityConfigData activityConfig = PlayerDataManager.Instance.GetActivityData(actData.dwActiveID);
			//已经是最后一个了，不能再领取了//
			if (actData.byIndex >= activityConfig.rewardList.Count) {
				reachConditionMap[actData.dwActiveID] = 2;
			}
			else
			{
				int dwActiveParam = actData.dwActiveParam;
				if (actData.dwActiveID == timerActivityID) {
					dwActiveParam = dwActiveParam/60;
				}
				if(dwActiveParam >= activityConfig.qualifiedList[actData.byIndex+1])
				{
					isCanReach = true;
					reachConditionMap[actData.dwActiveID] = 1;
				}
				else
				{
					reachConditionMap[actData.dwActiveID] = 0;
				}
			}
		}
		//if (preHaveActivityCanGetReward != isCanReach) {
			if (isCanReach) {
				UIEventManager.Instance.TriggerUIEvent (UIEventType.PlayMainBtnAnim,UI.MainUI.UIType.Activity);
			} else {
				UIEventManager.Instance.TriggerUIEvent (UIEventType.StopMainBtnAnim,UI.MainUI.UIType.Activity);		
			}
		//}
		preHaveActivityCanGetReward = isCanReach;
		return isCanReach;
	}
	#endregion

	public void Instantiate()
	{
		
	}
	public void LifeOver()
	{
		instance = null;
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TownUIBtnLoadComplete, TownUISceneLoadComplete);
	}
}
