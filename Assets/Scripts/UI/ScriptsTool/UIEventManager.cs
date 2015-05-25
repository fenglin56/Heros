using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UIEventType
{
    LoginUI,
    ShowLodingUI,
    LoadingProgress,//加载场景进度条
	TeamComplete,
    LoadingComplete,//加载场景完成
	LoadingStartDownTime,//组队时加载倒计时
    UpgrateSkillInfo,
    UpgrateSkillPoints,
	//强化成功
	SkillStrengthenEvent,
	//进阶
	SkillAdvanceEvent,

    EctypeUIInfo,
    SingleEctypeUIInfo,

    LoadBattleSceneCompleteCS,//加载副本完成
    LoadBattleSceneCompleteSC,//加载副本完成

    CreatMoster,//副本生成怪物
    CreatPortal,//副本生成传送门 
    ReasetTeammateStatus,//队员状态更新
    TeamMemberLeave,//队员离队
    //EctypeLock,//副本未解锁

    OpentMainUI,//打开城镇主UI
    CloseMainUI, //单击返回按钮
    ShowTopCommonUI, //显示顶部公共按钮
    CountDownUI,//副本倒计时结算
    DoubleHitUI,//连击UI
    DoubleKillUI,//连杀UI
    AddAirValue, //增加气值
    FlagMPBar,//mp条闪动
    SirenSkillFire,//妖女无双技能施放
    AddForce,//战力增加
    //GetEctypeTreasureReward,//副本结算金色卡牌奖励
    PackageFull,//背包已满消息

    NotEnoughGoldMoney,//元宝不足
    NoEnoughActiveLife,//活力值不足
	ActiveLifeUpdate,//活力值更新
    UseMedicamentResult,//药品使用结果
	PlayerLevelUpdate,
	PlayerHoldMoneyUpdate,
    ResetContainerGoods,//刷新物品栏列表
    ResetContainerPack,//刷新背包容量
    DiscardContainerGoodsComplete,//出售物品成功
    ResetPackageContainerGoods,//更新背包容器

    CreateFailedDuplicate, //创建人物失败 角色名重复
    CreateFailedMaxnum ,    //创建人物失败 角色达到上限

    OpenTreasureUI,//打开宝箱UI
    OpenTreasureChest,//翻开宝箱消息
    ReceiveSettleAccount,//收到结算消息

    Disconneted,//掉线
    CheckNewItem,//检测是否获得了更好的装备

    SwitchOffShowPlayerInfo,//显示公共玩家信息开关

    OnRoleViewClick,//人物界面点击
    ResetDragComponentStatus,//刷新拖拽栏状态

    NewColdWorkFromSever,//服务器下发新的冷却事件
    AddColdWork,//添加新的冷却时间
    RemoveColdWork,//移除冷却时间
    ColdWorkComplete,//冷却完毕事件

    UpdateYaoqiValue,//更新妖气值
	YaoNvJoinSuccess,//妖女参战成功

    WorldChatMsg,//世界聊天信息
    PrivateChatMsg,//私人聊天信息
	ShowPrivateMessageTip,//是否显示私信提醒
	ClosePrivateMessageTip,//关闭私信提醒
    RoomChatMsg,//练功房信息
    OpenPrivateChatWindow,//打开私人聊天窗口
    OpenWorldChatWindow,//打开世界聊天窗口
    CloseWorldChatWindow,//强关闭世界聊天窗口
	NewTeamMessage,//组队新消息提示
	ShowChatButton,//显示聊天按钮

    OpenActiveChestUI,//女官宝箱界面请求
    OpenActiveChest,//女官宝箱开启奖励信息
    ShowCanReceiveActiveChest,//显示可领取宝箱奖励按钮

    UpdateTreasureTreesData,//更新宝树信息
    TreasureTreesUseMana,   //宝树使用仙露
    TreasureTreesGetReward, //宝树得到奖励
	ShopsBuySuccess,//商品购买成功
    QuickPurchase,//快速购买
	QuickBuySuccess,//快速购买成功
	ReceiveBesetJewel,//镶嵌宝石回复
	ReceiveRemoveJewel,//摘除宝石回复
	ReceiveSwallowJewel,//吞噬宝石回复
    GetEamilList,
	ReadEmail,
	GetAllAttachment,
	GetAttachment,
	DeleteEmail,
	DeleteAllEmail,
	SendEamil,
    UpdatedEmailList,
    DeleteFriendSuccess,
	UpdateRandomRewardStatus,//更新讨伐副本首战领取
	CancelRandomRewardMatching,//取消随机副本匹配
    #region 新手引导
    ClickTheGuideBtn, //指定的引导按钮
    ClickOtherButton, //非指定的引导按钮   
    EnableMainButton,  //开启功能
    InitMainButton,  //初始化主界面主按钮
    EctypePageSkip,  //副本选择页面跳转
    CloseAllUI,  //新手引导关闭所有UI
    OpenSystemButton, //展开系统主按钮
    EctypeGuideStep, //副本引导步骤
	NpcTalkTaskDealUI,
	IntellJumpSiren,
    #endregion

    MartialArtsRoomList,//练功房房间列表

    BossDeathMsg,//Boss死亡消息

    TrialsEctypeList,//试炼副本列表
    TrialSettlement,//试炼副本总结算

    UpdateEnegryTimeEvent,//刷新活力恢复时间

    //创建角色
    SelectRole,

    ShowMissionFailPanelLate,//延迟显示失败结算界面

    AddAPMNumber,//增加副本APM统计数
    AddBattleButtonClickNumber,//添加副本战斗按钮有效点击数

    #region//主按钮动画
	TownUIBtnLoadComplete,
    PlayMainBtnAnim,//开始播放动画
    StopMainBtnAnim,//停止播放动画
    #endregion
    #region//主按钮特效
    ShowMainBtnEffect,//显示特效
    HideMainBtnEffect,//隐藏特效
    ChangemailBtnEffect,//显示邮件特效
	MainBtnCloseEvent,
	MainBtnOpenEvent,
    #endregion

    #region //错误消息
    EctypeLevelError,//副本等级错误消息
    EctypeCantFindRoom,//找不到房间
    EctypeRoleFull,//人数已满
    EctypeLockError,//副本未解锁
	EctypeTeamNum,//讨伐副本队伍数量
	RandomMatchingCancel,//随机匹配取消
    #endregion

	OpenTreasure,//副本选择界面打开宝箱

	DefenceLoopNum,  //防守副本波数
	DefenceMaxLoopNum,//防守副本最大波数

	EqipmentLevelUp,//装备升级事件
    Forging,//铸造
    QuickSmelt,//强化十次
	EctypeSirenSkillPropUpdate,//副本奥义使用属性更新
	EctypeMedicamentPropUpdate,//副本使用药品属性更新
	EctypeRelivePropUpdate,//副本复活属性更新

	EctypeNormalDataUpdate,//常规副本信息更新
	EctypeChessDataUpdate,//更新副本宝箱信息
	EctypeSweepReward,
	#region 无尽试炼
	EndLessLoopNumUpdate,//无尽副本波数
	EndLessPassLoopNumUpdate,//无尽副本闯过的波数
	EndLessFinishPassLoopData,//无尽副本结算时最终闯过的波数
	EndLessBestUpdate,//无尽副本最佳成绩更新
	EndLessAgainConnectTime,
	EndLessJumpSceneTime,
	//新的一波开始
	EndLessNewWaveUpdate,
	//NPC数量更新
	EndLessNpcCountUpdate,
	//奖励显示
	EndLessRewardUpdate,
	//扫荡次数更新//
	SweepTimesUpdate,
	#endregion

	VipGradeUpdate,//vip升级
	VipPaySuccess,//充值成功
	#region 组队
	ShowTeamChildPanel, //显示组队子面板
	#endregion
	
	CrusadeSettlement,//讨伐副本结算
	CrusadeTiming,    //讨伐副本计时
	CrusadeMatching,  //讨伐副本匹配

    #region LuckDraw
    LuckDrawResult,  //抽奖结果
    #endregion

	#region dailySign
	//服务器0点更新本天数据
	DailySignAllUpdate,
	//x关闭弹出获取的奖励界面事件
	DailySignSuccessPopCloseEvent,
	//签到成功回应//
	DailySignResponseEvent,
	#endregion

	#region activity
	ReceiveActivityDataEvent,
	ActivityRewardEvetn,
	ActivityTimeUpdate,
	#endregion

	#region auction 竞拍系统
	//批量下发竞拍数据
	ReceiveServerAuctionData,
	//实时下发竞拍结果
	ReceiveServerAuctionResult,
	#endregion

	#region auction 随身商店
	//批量下发随身商店数据
	RcvCarryShopUIDataEvent,
	//购买结果
	RcvCarryShopBuyEvent,
	//解锁
	RcvCarryShopUnLockEvent,
	#endregion

    #region 排行榜
    ReceiveRankingListRes,
    ReceiveRankingDetailRes,
    #endregion
    #region 邮件循环列表
    ItemEnd,
    OnChangeItem,
    #endregion
	//主场景中 摇杆
	OnNpcTalkOpenEvent,
	OnNpcTalkCloseEvent,
	OnNpcGuideStartEvent,
	OnNpcGuideStopEvent,
	OnBossShowEvent,
	OnLostConectEvent,
	
	//通知播放失败特效
	OnFailEffPlayEvent,

	#region pvp
	pvpFriendCancelTeam,//好友离开队伍
	pvpSyncTeam,//同步pvp队伍
	pvpStartmatch,//匹配开始
	MartialUpgrade,	//武学升级
	pvpItemDataUpdate,//玩家场景加载是否完成
	ReceiveInvite,//收到pvp组队邀请
	PVPReceiveRankingListRes,//收到pvp排行榜
	PVPHonorUpdate,	//武学界面荣誉值更新
	PVPContributeUdate,	//武学界面贡献更新
	#endregion
}

 public class UIEventManager
 {

     Dictionary<UIEventType, UIEventClass> UIEventDictionary;  

     public UIEventManager()
     {
         this.UIEventDictionary = new Dictionary<UIEventType, UIEventClass>();
     }

     private static UIEventManager m_Instance;
     public static UIEventManager Instance 
     {
         get
         {
             if (m_Instance == null)
             {
                 m_Instance = new UIEventManager();
             }
             return m_Instance;  
         }
     }

     /// <summary>
     /// 触发事件
     /// </summary>
     /// <param name="uiEventType">事件type</param>
     /// <param name="eventInstance">传入的实例</param>
     public void TriggerUIEvent(UIEventType uiEventType, object EventObj)
     {
		UIEventClass eventClass = null;
         if (UIEventDictionary.TryGetValue(uiEventType,out eventClass))
         {
			eventClass.RaiseEvent(EventObj);
         }
         else
         {
             //Debug.LogWarning("UIEventType：" + uiEventType + "没有对象侦听！");
         }
     }

     /// <summary>
     /// 注册事件
     /// </summary>
     /// <param name="uiEventType">事件type</param>
     /// <param name="uiEventDelegate">注册的方法</param>
     public void RegisterUIEvent(UIEventType uiEventType, UIEventDelegate uiEventDelegate)
	{
		UIEventClass eventClass = null;
         if (UIEventDictionary.TryGetValue(uiEventType,out eventClass))
         {
			eventClass.AddEvent(uiEventDelegate);
         }
         else
         {
             UIEventDictionary.Add(uiEventType, new UIEventClass(uiEventDelegate));
         }
     }

     public void RemoveUIEventHandel(UIEventType uiEventType, UIEventDelegate uiEventDelegate)
	{
		UIEventClass eventClass = null;
         if (UIEventDictionary.TryGetValue(uiEventType,out eventClass))
         {
			eventClass.RemoveEvent(uiEventDelegate);
         }
     }

 }
 /// <summary>  
 /// 触发事件的类，每个事件对应一个实例
 /// </summary>
 public delegate void UIEventDelegate(object uiEventInsatance);
 public class UIEventClass
 {
     public event UIEventDelegate UIEvent;

     public UIEventClass(UIEventDelegate uiEventDelegate)
     {
         AddEvent(uiEventDelegate);
     }

     public void AddEvent(UIEventDelegate uiEventDelegate)
     {
         this.UIEvent += uiEventDelegate;
     }

     public void RemoveEvent(UIEventDelegate uiEventDelegate)
     {
         this.UIEvent -= uiEventDelegate;
     }

     public void RaiseEvent(object EventObj)
     {
         if (this.UIEvent != null)
         {
             this.UIEvent(EventObj);
         }
     }

 }
