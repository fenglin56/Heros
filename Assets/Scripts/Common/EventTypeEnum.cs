using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum EventTypeEnum
{
    NavPointFromServer,  //服务器返回寻路的路点
    SceneChange,        //场景跳转
    StateChange,        //状态改变
    GotoStroyLine,     //跳转至剧情
    TargetSelected,  //选择消息
    EntityMove,         //生物移动消息
    EntityStopHere,     //生物停止移动消息
    MonsterMove,        //怪物移动路点消息(测试)
    SceneLoaded,        //场景加载完毕
    S_CEnterCode,       //S发送登录随机数给C
    S_CUserLoginRes,    //S发送角色信息给C
    S_CSMsgFightFightToResult,  //攻击结果消息
    AnimationEvent,         //动画事件
    PlayerGotoSceneReady,   //玩家在场景切换就绪
    EctypeConfig,       //副本难度选择
    EntityCreate,       //实体创建
    OpenEquipStrengthenUI,  //弹出装备强化界面
    EntityCreate_Player,//创建人物实体
    //****************
    ContainerCreate,  //物品篮
    NPCInteraction,     //NPC交互
    EctypeSettleAccount, //副本结算
	EctypeBattleStatistics,		// 副本战斗数据（玩家操作数据）统计
	DefenceEctypeSettleAccount, //副本结算
    EntityUpdateValues,   //主角属性更新
    PlayerLevelUpdate,   //玩家等级属性更新
	PlayerTitleUpdate,   //玩家称号属性更新
    PlayerFashionUpdate,//玩家时装更新
	PlayerHoldMoneyUpdate,//铜币更新
    OnTouchInvoke,        //触发点击事件
    OnDoubleTouchInvoke,        //触发双击事件
    ColdWork,//冷却时间
    EquipSkill, //技能装备/卸下
    FiredBullets,   //发射子弹
    BreakSkill,    //技能打断
    FightChangeDirect,
    //UpdateRollStrength,         //更新翻滚体力值
    //NoEnoughRollStrength,       //体力值不足
    UpdateRollAirSlot,  //更新翻滚气槽值
    NoEnoughRollAir,  //气值不足
    
    #region 组队系统事件
    TeamNoFoundList,            //没有队伍
    TeamNoExist,                //队伍不存在
    TeamFull,                   //队伍人数已满
    TeamList,                   //队伍匹配
    TeamCreate,                 //队伍创建
    TeamDisband,                //队伍解散
    TeamUpdateProp,             //队伍信息更新
    TeamMemberUpdateProp,       //队伍成员信息更新
    TeamMemberLeave,            //队员离开
    TeamMemberBeKick,           //队员请离
    TeamMemberDevilInvite,           //队员邀请
    TeamMemberReady,            //队员准备
    TeamExistMemberNoReady,     //有队员未准备
    TeamErrorCode,              //错误消息码
    TeamActiveLifeNotEnough,    //体力值不足
    TeamFighting,               //队伍正在战斗
    EctypeNoQualification,      //有队员没有挑战副本资格
    #endregion
    FightCommand,   //收到战斗指令
    BeatBack,   //收到被击退指令
    BeAdsorb,   //收到被吸附指令
	BeAdSorbEx, //收到特殊吸附指令
    FightFly,   //收到被击飞指令
    Teleport,   //收到被传送结算指令
    BreakShield,  //防护值破防
    ReplyShield,   //防护值恢复
    EntityHorde,   //生物被定身
	SingleFigntCommand,//收到单机战斗指令
    EntityDie,  //生物死亡
    EntityRelive,//玩家复活
    PlayerBeKicked, //玩家被系统踢出
    RevNearlyPlayer,  //收到附近玩家数据
    AddFriendSuccess, //加好友成功
	RefreshFriendList, //刷新好友列表
    ClearRankingList,//清理排行榜数据
    GetRankingList, //收到显示排行榜信息
    GetPlayerRanking,//收到显示个人排行信息
    PVPFindPlayer,  //匹配到玩家信息
    PVPReady,       //pvp战斗准备信息
    PVPFighting,    //pvp战斗开始信息
    PVPSettleAccount,   //pvp战斗结算
    PVPRematching,//pvp重新匹配
    PVPNoTimes,   //pvp挑战次数不足
    LianHuaResult,//妖女炼化结果
    XiuLianAccount,//修炼结算
    UpdateRoomSeatInfo,//更新房间信息
    UpdateRoomYaoNv,//更新房间妖女展示
    #region item operate ui
    OpenItemOperateUI,
	
	#endregion

    #region DB
    CreateFailedLoggedin,  //创建人物(登录)失败 账号已登录 倒计时x秒 重新操作(服务器在踢人)
    CreateFailedDuplicate, //创建人物失败 角色名重复
    CreateFailedMaxnum,     //创建人物失败 角色达到上限
    ReceiveDefaultErrorCode,   //默认错误处理
    #endregion

    //PageChanged,   //UI分页
    GoodsOperateSmelt,  //装备强化结果
    LostConect, //网络断线

    SkillBreakForStatistics,  //技能打断统计
	EctypeFinish, //副本结束 获得胜败信息

    TaskStateRefresh,  //任务列表更新
    QuickTaskGuideRefresh,  //快速引导任务列表更新
    AcceptTask,  //接收任务
    TaskTrigger,   //任务触发
    TaskFinish,   //任务完成
    TaskBreak,  //任务中断
    TaskExecuteInvoke,  //任务执行消息
    TaskFinishGuideExecuteInvoke, //开始任务完成后引导消息
    ClickNoneGuideBtn,  //点击了非引导按钮 
    HitMonsterForGuide,  //引导 打中指定怪物
    BossBreakProtectForGuide,  //引导 Boss破防
    FireSkillForGuide,  //引导 施放指定技能

    ReceiveGuideStep,//收到副本引导步骤
    FinishGuideStep,//副本引导步骤完成

    HideNpcTalk,//关闭打开着的NPC对话

    PackStateChange,//背包状态改变
    EctypeUpdateBlock,// 副本动态阻挡更新
	CloseRelivePanel,//关闭复活界面

    ShowPlayerEctypeGuideArrow, //玩家正前方指示箭头消失
    HidePlayerEctypeGuideArrow,  //玩家正前方指示箭头隐藏

	NormalContinueExpried,  //普通分段步骤记忆超时消息 
}
