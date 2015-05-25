using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MainActionDefineManager
{
    //////////////////////////////////////////////////////////////////////////
    ///////////////////////////// 消息流通类型 ///////////////////////////////
    //[CSC]:  服务器与客户端相互通信的消息码。
    public const short MAINACTION_CIRCULTYPE_CSC = 1;

    //[BCM]:  服务器各功能与逻辑对像通知广播的消息码（只会用在服务器）
    public const short MAINACTION_CIRCULTYPE_BCM = 2;

    //[SS]:  服务器各功能相互通信的消息码（只会用在服务器）
    public const short MAINACTION_CIRCULTYPE_SS = 3;

    //[CC]:  客户端各功能相互通信的消息码（只会用在客户端）
    public const short MAINACTION_CIRCULTYPE_CC = 4;

    //[IC]:  客户端输入设备与客户端功能模块通信的消息码（只会用在客户端）
    public const short MAINACTION_CIRCULTYPE_IC = 5;

    //[CSC_S]:  服务器发给服务器模块的CSC类型，相当于模拟客户端发的CSC消息（只会用在服务器）
    public const short MAINACTION_CIRCULTYPE_CSC_S = 6;

    //[CSC_C]:  客户端发给客户端模块的CSC类型，相当于模拟服务器发的CSC消息（只会用在客户端）
    public const short MAINACTION_CIRCULTYPE_CSC_C = 7;

    //[IC_C]:  客户端发给客户端模块的IC类型，相当于模拟输入设备发的IC消息（只会用在客户端）
    public const short MAINACTION_CIRCULTYPE_IC_C = 8;

    public const short MAINACTION_MAX = MAINACTION_CIRCULTYPE_IC_C;
}
public class EventDefineManager
{

    public const short EVENT_SYSTEM_EVENTBEGIN = 0;

    // 场景服登录或者登出社会服
    public const short EVENT_SYSTEM_CS_LOGINOUT = EVENT_SYSTEM_EVENTBEGIN;

    // 服务器启动成功事件
    public const short EVENT_SYSTEM_START_OK = 1;

    // 构建了场景
    public const short EVENT_SYSTEM_BUILDZONE = 2;

    // 场景销毁
    public const short EVENT_SYSTEM_DESTORYZONE = 3;

    // 场景服登录或者登出商城服
    public const short EVENT_SYSTEM_MS_LOGINOUT = 4;

    public const short EVENT_SYSTEM_EVENTEND = 5;

    /***************************************************************/
    /////////////////// 通用物品篮事件码 ////////////////////////////
    /***************************************************************/
    //    public const short EVENT_CONTAINER_BEGIN = (short)MAINACTION_MSGID.MAINACTION_MSGID_END;
    //
    //    // 向通用物品篮添加物品
    //    public const short EVENT_CONTAINER_ADDCONTAINERGOODS = EVENT_CONTAINER_BEGIN;
    //    // 向通用物品篮删除物品
    //    public const short EVENT_CONTAINER_REMOVECONTAINERGOODS = EVENT_CONTAINER_BEGIN + 1;
    //
    //    // 清空通用物品篮
    //    public const short EVENT_CONTAINER_CLEANSASHCONTAINER = EVENT_CONTAINER_REMOVECONTAINERGOODS + 1;
    //
    //    // 链接物品
    //    public const short EVENT_CONTAINER_LINKCONTAINERGOODS = EVENT_CONTAINER_CLEANSASHCONTAINER + 1;
    //
    //    // 通用物品篮内要释放了
    //    public const short EVENT_CONTAINER_DESTORYCONTAINER = EVENT_CONTAINER_LINKCONTAINERGOODS + 1;
    //
    //    // 通用物品篮内修改了容量
    //    public const short EVENT_CONTAINER_CHANGESIZE = EVENT_CONTAINER_DESTORYCONTAINER + 1;
    //
    //    //通用物品篮内某位置发生了变化
    //    public const short EVENT_CONTAINER_PLACECHANGE = EVENT_CONTAINER_CHANGESIZE + 1;
    //
    //    // 通用物品篮拆分物品
    //    public const short EVENT_CONTAINER_SPLITGOODS = EVENT_CONTAINER_PLACECHANGE + 1;
    //
    //    //合并物品（只适用于全部合并）
    //    public const short EVENT_CONTAINER_UNITECONTAINERGOODS = EVENT_CONTAINER_SPLITGOODS + 1;
    //
    //    //交换物品
    //    public const short EVENT_CONTAINER_EXCHANGECONTAINERGOODS = EVENT_CONTAINER_UNITECONTAINERGOODS + 1;
    //
    //    public const short EVENT_CONTAINER_END = EVENT_CONTAINER_EXCHANGECONTAINERGOODS + 1;
    //
    //    /***************************************************************/
    //    /////////////////// 交易相关事件码 ////////////////////////////
    //    /***************************************************************/
    //    public const short EVENT_TRADE_BEGIN = EVENT_CONTAINER_END;
    //
    //    // 玩家给予(玩家\NPC)
    //    public const short EVENT_TRADE_GIVE = EVENT_TRADE_BEGIN;
    //
    //    // 玩家交易
    //    public const short EVENT_TRADE_TRADE = EVENT_TRADE_GIVE + 1;
    //
    //    // 玩家摆摊请求
    //    public const short EVENT_TRADE_STALL_REQUEST = EVENT_TRADE_TRADE + 1;
    //
    //    // 玩家摆摊
    //    public const short EVENT_TRADE_STALL_CONFIRM = EVENT_TRADE_STALL_REQUEST + 1;
    //
    //    // 玩家取消摆摊
    //    public const short EVENT_TRADE_STALL_CANCEL = EVENT_TRADE_STALL_CONFIRM + 1;
    //
    //    // 玩家点击其他摆摊
    //    public const short EVENT_TRADE_STALL_CLICK = EVENT_TRADE_STALL_CANCEL + 1;
    //
    //    // 玩家关注其他摆摊商店
    //    public const short EVENT_TRADE_STALL_ATTENTION = EVENT_TRADE_STALL_CLICK + 1;
    //
    //    public const short EVENT_TRADE_END = EVENT_TRADE_STALL_ATTENTION + 1;
    //
    //    //冷却事件
    //
    //    public const short EVENT_COLD_BEGIN = EVENT_TRADE_END;
    //
    //    /////////////////////////////////////////////////////////
    //    //描        述：更新CD时间
    //    //支持流通类型：MAINACTION_CIRCULTYPE_SS
    //    /////////////////////////////////////////////////////////
    //    public const short EVENT_COLD_COLDTIMEROUT = EVENT_COLD_BEGIN;
    //    public const short EVENT_COLD_END = EVENT_COLD_COLDTIMEROUT + 1;
    //
    //    //最大事件数
    //
    //    //消息的最大值 永远在最下面
    //    public const short EVENT_MSGID_MAX = EVENT_COLD_END;
}
public class RootDefineManager
{
    //////////////////////////////////////////////////////////////////////////
    ///////////////// 登陆主消息码（根：NET_ROOT_LOGIN） /////////////////////

    // 发送登录随机数
    public const short ROOTLOGIN_SC_MAIN_ENTERCODE = 1;
    public const short ROOTLOGIN_CS_MAIN_ENTERCODE = 1;

    // 登录返回 调至状态
    public const short ROOTLOGIN_SC_MAIN_LOGINRES = 2;
    public const short ROOTLOGIN_CS_MAIN_LOGINRES = 2;

    // 队列位置，从0开始
    public const short ROOTLOGIN_SC_MAIN_LISTNUM = 3;
    public const short ROOTLOGIN_CS_MAIN_LISTNUM = 3;

    // 发送客户端版本号
    public const short ROOTLOGIN_SC_MAIN_CLIENTVER = 4;
    public const short ROOTLOGIN_CS_MAIN_CLIENTVER = 4;

    // 用户登录
    public const short ROOTLOGIN_SC_MAIN_USERLOGIN = 5;
    public const short ROOTLOGIN_CS_MAIN_USERLOGIN = 5;


    // 已登录玩家跳转服务器 结束
    public const short ROOTLOGIN_SC_MAIN_JUMP_FINISH = 6;
    public const short ROOTLOGIN_CS_MAIN_JUMP_FINISH = 6;

    // 需要验证矩阵卡
    public const short ROOTLOGIN_SC_MAIN_SUPERCARD = 7;
    public const short ROOTLOGIN_CS_MAIN_SUPERCARD = 7;

    // 玩家游戏世界角色分布情况
    public const short ROOTLOGIN_SC_MAIN_ACTOR_NUM = 8;

    // 游戏世界当前在线人数分布情况
    public const short ROOTLOGIN_SC_MAIN_WORLD_ONLINE = 9;

    // 登录中心通知客户端发送账号密码
    public const short ROOTLOGIN_SC_NOTICE_LOGIN = 10;


    //////////////////////////////////////////////////////////////////////////
    ///////////// 选择角色主消息码（根：NET_ROOT_SELECTACTOR） ///////////////
    // 选择人物
    public const short ROOTSELECTACTOR_SC_MAIN_SELECT = 1;
    public const short ROOTSELECTACTOR_CS_MAIN_SELECT = 1;

    // 将调到运行态
    public const short ROOTSELECTACTOR_SC_MAIN_TURNRUN = 2;
    public const short ROOTSELECTACTOR_CS_MAIN_TURNRUN = 2;

    // 创建角色
    public const short ROOTSELECTACTOR_SC_MAIN_CREATEACTOR = 3;
    public const short ROOTSELECTACTOR_CS_MAIN_CREATEACTOR = 3;

    // 发送登录随机数
    public const short ROOTSELECTACTOR_SC_MAIN_ENTERCODE = 4;
    public const short ROOTSELECTACTOR_CS_MAIN_ENTERCODE = 4;

    // 发送MAC
    public const short ROOTSELECTACTOR_SC_MAIN_MAC = 5;
    public const short ROOTSELECTACTOR_CS_MAIN_MAC = 5;

    // 已登录玩家跳转服务器 开始 
    public const short ROOTSELECTACTOR_SC_MAIN_JUMP_START = 6;
    public const short ROOTSELECTACTOR_CS_MAIN_JUMP_START = 6;

    // 删除角色
    public const short ROOTSELECTACTOR_SC_MAIN_DELETEACTOR	=	7;
    public const short ROOTSELECTACTOR_CS_MAIN_DELETEACTOR	=	7;

}

public class CommonMsgDefineManager
{
	//public const int SERVER_SKIP_INDEX = 81;   //在实体更新时，服务器与客户端Index的差异值，（服务器不下发的部分）
	public const int SERVER_SKIP_INDEX = /*82; //2014/11/27之前index值*/88;   //在实体更新时，服务器与客户端Index的差异值，（服务器不下发的部分）
    public const short MSG_ACTION_NEW_WORLD = 5;
    public const short MSG_PROP_CREATEENTITY = 1;    //创建实体
    public const short MSG_PROP_DESTROYENTITY = 2;   //删除实体
    public const short MSG_PROP_UPDATEPROP = 3;    //实体属性更新
    public const short MSG_ACTION_PLAYER_CLEAN_SHOW = 6; //更新玩家外观资源
    public const short MSG_ACTION_DIE = 7;          //生物死亡
    public const short MSG_ACTION_MOVE = 12;
    //public const short MSG_CS_ACTION_CLIENTOPT_MOVE = 14;
    public const short MSG_ACTION_MONSTER_MOVE = 14;    // 服务器怪物移动消息(路径点)
    public const short MSG_ACTION_PLAYER_OP_LOGINOUT = 15;  //主动退出游戏消息
	public const short INTERACT_MSG_OPEN_UI = 18;//活动奖励页面数据下发 
	public const short INTERACT_MSG_GETREWARD = 19;//活动领取奖励回应
	// 主动请求跳转地图
	public const short MSG_ACTION_TELEPORTTO = 20;
    public const short MSG_ACTION_CLICKCHUNNEL = 24;
    public const short MSG_ACTION_RELIVE = 25;      //玩家复活
    public const short MSG_ACTION_TOUCHBOX = 26;    //实体碰撞箱子消息
    public const short MSG_ACTION_PARCTICE = 27;    //修炼经脉  
	public const short MSG_ACTION_OPEN_LIANHUAUI = 28;//打开炼化女妖界面
	public const short MSG_ACTION_LIANHUA = 29;//炼化女妖
	
    public const short MSG_ACTION_OPEN_FRUIT				=   30;//宝树果实界面
	public const short MSG_ACTION_GET_FRUIT				=   31;//获取某个果实
	public const short MSG_ACTION_GET_ALL_FRUIT			=	32;//一键获取所有果实
	public const short MSG_ACTION_SYS_FRUIT_STATUS			=	33;//同步果实状态
	public const short MSG_ACTION_FRUIT_USEMANNA			=	34;//使用果实仙露
	public const short MSG_ACTION_WATERING_FRUIT			=	35;//给干旱果实浇水
	public const short MSG_ACTION_WATERING_ALLFRUIT		=	36;//给所有干旱果实浇水
    public const short MSG_ACTION_UNLOCK_FRUIT = 37;//强行开启未解锁果实

    public const short MSG_ACTION_ACCOUNT_XIULIAN = 38; //下发修炼结算信息
    public const short MSG_ACTION_BREAK_INFO = 39;//修为突破消息

    public const short MSG_ACTION_GET_YAONV_NEIDAN = 40;//获取妖女内丹
	public const short MSG_ACTION_YAONV_FIGHTING = 41;//妖女参战
	public const short MSG_ACTION_DAYSIGNINUI = 42;				//签到界面UI数据下发
	public const short MSG_ACTION_DAYSIGNIN = 43;				//角色签到
	public const short MSG_ACTION_YAONVCONDITION_UPDATE = 44;	//妖女收服条件更新

    public const short MSG_ACTION_USEMEDICAMENTRESULT = 11;

    public const short MSG_ACTION_STOPHERE = 13;    //停止移动

    public const short MSG_ACTION_MEETNPC = 19;
    public const short MSG_ACTION_WORLD_OBJECT_INIT_BUFF = 8;   // 初始化buff
    public const short MSG_ACTION_WORLD_OBJECT_ADD_BUFF = 9;    // 添加Buff
    public const short MSG_ACTION_WORLD_OBJECT_REMOVE_BUFF = 10;  //移除Buff
    public const short MSG_ACTION_COLD_WORK = 92;   //冷却通知

    public const short MSG_ACTION_INIT_SKILL = 52;  //初始化技能数据消息
	public const short MSG_ACTION_UNLOCK_SKILL=53;     //解锁技能消息
    public const short MSG_ACTION_UPGRADE_SKILL = 54;  //升级/学习技能
    public const short MSG_ACTION_EQUIP_SKILL=55;    //装备/卸载技能   
    public const short MSG_ACTION_OPEN_SKILLSTUDY_DLG = 56;//打开升级面板
	// 进阶技能(C<=>S)
	public const short MSG_ACTION_ADVANCED_SKILL = 57;
	// 强化技能(C<=>S)
	public const short MSG_ACTION_STRENGTHEN_SKILL = 58;
	public const short MSG_ECTYPE_USE_MEDICAMENT = 70;//使用药品
    public const short MSG_ACTION_HEART_FPS = 95;   //心跳协议
    public const short MSG_ACTION_BULLETTEST = 96;  //子弹测试

	//武学
	public const short MSG_ACTION_WUXUE_STUDY = 45;	//升级 
	public const short MSG_ACTION_WUXUEUI =46;	
	//pvp历史战绩
	public const short MSG_ACTION_GET_PVP_HISTORY =1120;	
}

public class GoodsOperateDefineManager
{
    //宝石合成
    public const short  MSG_GOODSOPERATE_MAKE=0;
    //强化
    public const short MSG_GOODSOPERATE_SMELT=1;
    //镶嵌
    public const short MSG_GOODSOPERATE_BESET=2;
    //打开物品操作界面
    public const short MSG_GOODSOPERATE_OPEN_MAKEFACE=3;
    //获得制造消耗
    public const short MSG_GOODSOPERATE_GET_MAKECOST = 4;
    
    public const short MSG_GOODSOPERATE_OPEN_TREASURE_UI = 5; // 打开宝箱界面
    public const short MSG_GOODSOPERATE_CLICK_TREASURE = 6;//翻开宝箱请求
	//洗练
	public const short MSG_GOODSOPERATE_SOPHISTICATION = 7;
	//炼化
    public const short MSG_GOODSOPERATE_ARTIFICE = 8;
    //打开开活力值宝箱UI
	public const short MSG_OPNE_ACTIVE_CHEST_UI = 9;
	//活跃值开宝箱
    public const short MSG_ACTIVE_VALUE_OPEN_CHEST = 10;

	public const short MSG_EQUIP_LEVEL_UP = 11;
	 
	/// /// <summary>
	/// 装备宝石取摘除
	/// </summary>
	public const short MSG_EQUIP_STORE_REMOVE = 12;
	/// <summary>
	/// 宝石吞噬
	/// </summary>
	public const short MSG_STORE_SWALLOW=13;

    public const short MSG_LUCKDRAW = 14;//聚宝盘
    public const short MSG_GOODSOPERATE_QUICKSMELT=15 ;//强化/生星十次
};

public class FightDefineManager
{
    //public const short MSG_FIGHT_INPUT = 1;
    //public const short MSG_FIGHT_RESULT = 2;
    public const short MSG_FIGHT_FINISH = -1;
    public const short MSG_FIGHT_TALK = -2;

    public const short MSG_FIGHT_BATTLE_COMMAND		 = 0;			//提交战斗指令	
	public const short MSG_FIGHT_BATTLE_CALCULATE_EFFECT		=1;		//战斗结算效果
    public const short MSG_FIGHT_BATTLE_BEAT_BACK = 2;		//实体被击退(只对敌人和玩家有效)
    public const short MSG_FIGHT_BATTLE_UPDATE_BULLET = 3;  //更新玩家实体所发出的子弹Index
    public const short MSG_FIGHT_BATTLE_BULLET_DESTORY = 4; //删除技能所创建的子弹
    public const short MSG_FIGHT_BATTLE_BREAK_SKILL = 5;  //技能打断协议
    public const short MSG_FIGHT_BATTLE_UPDATE_BATTERCOUNT = 6;//连击数显式
    public const short MSG_FIGHT_BATTLE_HIT_FLY = 7;        //击飞
    public const short MSG_FIGHT_BATTLE_CHANGE_DIRECT = 8; //施法状态改变人物朝向
    public const short MSG_FIGHT_BATTLE_ADSORB = 9; //吸附技能
    //public const short MSG_FIGHT_BATTLE_BREAK_SHIELD			=10;				//破防       改成属性更新为0时破防
    //public const short MSG_FIGHT_BATTLE_REPLY_SHIELD			=11;				//防御回复   改成属性更新为最大值时回复
    public const short MSG_FIGHT_BATTLE_HORDE = 10;            //定身术
    public const short MSG_FIGHT_BATTLE_BLOODSUCKING = 11;				//吸血请求
	public const short MSG_FIGHT_BATTLE_ADSORPTIONEX = 12;              //特殊吸附
    public const short MSG_FIGHT_BATTLE_TELEPORT = 14;     //传送结算
    public const short MSG_FIGHT_BATTLE_UPDATE_KILLCOUNT = 13;//连击数显式
	public const short MSG_FIGHT_BATTLE_COMMAND_SINGLE = 21;   //单机战斗指令
	public const short MSG_FIGHT_BATTLE_SUMMON_BULLET			 = 22;   //单机召唤子弹指令
    public const short MSG_FIGHT_BATTLE_CLIMBS                 =23;                //爬起
	public const short MSG_FIGHT_BATTLE_MISSEFFECT	    =24;	//丢失

}

public class ECTYPE_DefineManager
{
    public const short MSG_ECTYPE_GOBATTLE = 1; //客户端发出挑战副本请求
    public const short MSG_ECTYPE_SELECT = 2;//通知客户端弹出选择副本界面
    public const short MSG_ECTYPE_REQUESTCREATE = 3;//进入创建副本请求
    public const short MSG_ECTYPE_REQUESTLEVELDATA = 4;//点击副本后请求副本所需关卡信息
    public const short MSG_ECTYPE_LEVELDATA = 5;//服务器返回给角色关卡信息

    public const short MSG_ECTYPE_GETTEAMLIST = 6;//通过副本创建队伍
    public const short MSG_ECTYPE_STARTECTYPE_STATE = 7;//

    public const short MSG_ECTYPE_PLAYERREVIVE = 8;//副本倒计时结算
    public const short MSG_ECYTPE_PLAYERREVIVE_RESP = 9;//回应角色复活
    public const short MSG_ECTYPE_PLAYERDONTREVIVE = 10;//角色不复活

    public const short MSG_ECTYPE_SETTLEACCOUNTS = 11;//进入副本结算界面

    public const short MSG_ECTYPE_CLICKTREASURE = 12;//玩家点击宝箱
    public const short MSG_ECTYPE_TREASUREAWARD = 13;//玩家宝箱物品

    public const short MSG_ECTYPE_ENTERECTYPE_READY = 14;//客户端告诉服务点loading完成，
    public const short MSG_ECTYPE_CANENTER_ECTYPE = 15;//通知玩家显示副本

    public const short MSG_ECTYPE_RETURN_CITY = 16;//角色请求返回城镇
    public const short MSG_ECTYPE_CHANLLETE_COMPLETE = 17; //副本完成
    public const short MSG_ECTYPE_SYN_SKILLDATA = 18;//同步技能信息

    public const short MSG_TEST_BEGIN_LOG = 19;
    public const short MSG_TEST_END_LOG = 20;
    
    public const short MSG_ECTYPE_ERRORCODE = 21;//错误    

    public const short MSG_ECTYPE_CLEARANCE = 22; //玩家通过的最高副本ID;
    
    public const short MSG_ECTYPE_GUIDEFINISH = 24;//完成副本新手引导

    public const short MSG_ECTYPE_PVP_CHALLENGE = 25;//选择挑战pvp, 或者选择取消pvp
    public const short MSG_ECTYPE_PVP_FINDPLAYER = 26;//后端发送查找到的玩家

    public const short MSG_ECTYPE_PVP_ACTIONDONE = 27;//动作播放完成
    public const short MSG_ECTYPE_PVP_READY = 28;//准备战斗
    public const short MSG_ECTYPE_PVP_FIGHTING = 29;//开始战斗
    public const short MSG_ECTYPE_PVP_RUNAWAY = 30;//玩家逃跑
    public const short MSG_ECTYPE_PVP_SETTLEACCOUNTS = 31;//pvp结算

    public const short MSG_ECTYPE_INITIALIZE = 32;//初始化副本

    public const short MSG_ECTYPE_YAOQIPROP = 33;//  妖气值变化
    public const short MSG_ECTYPE_ADD_YAOQI = 34;//  前端请求增加妖气值    
	public const short MSG_ECTYPE_SHOW_LIANYAO = 35;//  妖气值满了，弹出妖气副本
    public const short MSG_ECTYPE_CLOSE_LIANYAO = 36;//  关闭炼妖副本

    public const short MSG_ECTYPE_PRACTICE_LIST = 37;//  练功房列表(S->C, C->S)
    public const short MSG_ECTYPE_PRACTICE_ENTER = 38;//  进入练功房
    public const short MSG_ECTYPE_ALLSEAT_INFO = 39;//  同步房间座位信息
    public const short MSG_ECTYPE_SET_INFO = 40;//  同步座位信息
    public const short MSG_ECTYPE_FAST_ENTER = 41;//快速进入练功房

	/////////////////试炼副本协议//////////////////
	public const short MSG_ECTYPE_TRIALS_INFO = 42;// 试炼副本信息
	public const short MSG_ECTYPE_TRIALS_SUBRESULT = 43;//小关结算
    public const short MSG_ECTYPE_TRIALS_TOTALRESULT = 44;//总结算

    public const short MSG_ECTYPE_PRACIICE_YAONVUPDATE = 45;//练功房妖女显示
    public const short MSG_ECTYPE_SHOW_TIPS = 46;//普通副本失败结算
    public const short MSG_ECTYPE_FIGHT_MODE   =47; //战斗模式
    public const short MSG_ECTYPE_NORMAL_RESULT = 48;//boss 死亡消息
    public const short MSG_ECTYPE_LEAVE_COLLECTINFO = 49;//服务器统计数据上传

	public const short MSG_ECTYPE_YAONVSKILL_USETIME = 50;		// 妖女技能使用剩余次数
	public const short MSG_ECTYPE_OPEN_CHEST = 51;		// 翻宝箱
	public const short MSG_ECTYPE_DEFINE_RESULT = 52;   //防守结算
	public const short MSG_ECTYPE_DEFINE_LOOPNUM = 53;
	public const short 	MSG_ECTYPE_DEFINE_LOOPMAX = 54;

	public const short MSG_ECTYPE_CRUSADE_RESULT = 55;		// 讨伐副本结算
	public const short MSG_ECTYPE_CRUSADE_TIME	= 56;			// 讨伐副本时间

	public const short MSG_ECTYPE_ENDLESS_LOOPNUM = 60; //无尽副本波数
	public const short MSG_ECTYPE_ENDLESS_REWARD = 61;   //无尽副本获得奖励
	public const short MSG_ECTYPE_ENDLESS_RESULT = 62;   //无尽副本结算
	public const short MSG_ECTYPE_ENDLESS_LOOPTIME = 63;			// 当前波数时间
	public const short MSG_ECTYPE_ENDLESS_MAPJUMPTIME = 64;
	public const short MSG_ECTYPE_USE_MEDICAMENT = 70; //使用药品
	public const short MSG_ECTYPE_FINISH = 71;   //副本胜败消息
	public const short MSG_ECTYPE_UPDATEPROP = 72; //副本更新
	public const short MSG_ECTYPE_MEMBER_UPDATEPROP	= 73;//副本单属性更新

	public const short MSG_ECTYPE_DEFINE_INFO = 80;
	public const short MSG_ECTYPE_DEFINE_UPDATA = 81;   

	//副本更新
	public const short MSG_ECTYPE_NORMAL_INFO = 82;// 常规副本信息	
	public const short MSG_ECTYPE_NORMAL_UPDATA = 83;// 常规副本信息更新
	public const short MSG_ECTYPE_NORMAL_UPDATACHEST = 84;// 更新副本宝箱信息

	public const short MSG_ECTYPE_ENDLESS_INFO = 85; //无尽副本界面数据
	public const short MSG_ECTYPE_ENDLESS_UPDATA = 86;   //无尽副本界面数据更新

    public const short MSG_ECTYPE_ENTER_AREA = 90;   //玩家进入区域
    public const short MSG_ECTYPE_UPDATE_BLOCK = 91;   //更新地图的阻挡

	//////////////////////副本阻挡相关协议///////////////
	public const short MSG_ECTYPE_UNLOCK_SWEEP				= 92; 			// 开启扫荡
	public const short MSG_ECTYPE_BEGIN_SWEEP				= 93; 			// 开始扫荡
	public const short MSG_ECTYPE_RESULT_SWEEP				= 94; 			// 扫荡结算

	public const short MSG_ECTYPE_RANDOM_REWARD				= 95;			// 首战奖励领取
}

public class InteractDefineManager
{
    // 通常的交互信息消息码
    public const short INTERACT_MSG_COMMON = 1;

    // 初始化任务
    public const int INTERACT_MSG_INITTASK = 2;

    // 接受任务
    public const int INTERACT_MSG_ACCEPTTASK = 3;

    // 更新任务
    public const int INTERACT_MSG_UPDATETASK = 4;

    // 完成任务
    public const int INTERACT_MSG_FINISHTASK = 5;

    // 放弃任务
    public const int INTERACT_MSG_GIVEUPTASK = 6;

    // 更新副本内引导步骤
    public const int INTERACT_MSG_GUIDESTEP = 7;

    //对话结束
    public const int INTERACT_MSG_SPEEKOVER = 8;
   
    //新手引导等级数据
    public const short INTERACT_MSG_NEWBIEGUIDE_DATA = 15;

    //排行榜系统消息
    public const short INTERACT_MSG_RANKINGLIST_DATA = 16;

    //个人排行消息
    public const short INTERACT_MSG_GETPLAYERRANKING = 17;

	//获取个人PVP排行榜信息
	public const short INTERACT_MSG_PVP_MYDATA		= 20;
	//获取个人历史战绩信息
	public const short INTERACT_MSG_PVP_HISTORY		= 21;
};

public class TeamDefineManager
{
    public const short MSG_TEAM_TEAMLIST = 0;//自适应队伍列表  走副本
    public const short MSG_TEAM_CREATE = 1;//队伍创建
    public const short MSG_TEAM_DISBAND = 2;//队伍解散
    public const short MSG_TEAM_PROP = 3;//队伍信息
    public const short MSG_TEAM_MEMBER_PROP = 4;//队员信息
    public const short MSG_TEAM_UPDATEPROP = 5;//队伍信息更新
    public const short MSG_TEAM_MEMBER_UPDATEPROP = 6;//队员信息更新
    public const short MSG_TEAM_MEMBER_JOIN = 7;// 加入队伍
    public const short MSG_TEAM_CAPTAIN_JOIN = 8;// 加入玩家队伍
    public const short MSG_TEAM_MEMBER_LEAVE = 9;// 离开队伍
    public const short MSG_TEAM_MEMBER_KICK = 10;// 踢人
    public const short MSG_TEAM_MEMBER_INVITE = 11;// 邀请
    public const short MSG_TEAM_FIND = 12;// 寻找队伍/队长
    public const short MSG_TEAM_MEMBER_READY = 13;//队伍的准备状态
    public const short MSG_TEAM_CHANGE_CAPTAIN = 14;//更改队长, 成功失败通过消息更新下发
    public const short MSG_TEAM_CHANGE_ECTYPE = 15;//更改目标副本
    public const short MSG_TEAM_FAST_JOIN = 16;//快速加入
    public const short MSG_TEAM_ERROR_CODE = 17;// 错误信息返回
    public const short MSG_TEAM_INVITER_FAILED = 18;    //邀请者收到失败应答
	public const short MSG_GET_TEAMNUMLIST = 20;//下个当前副本队伍个数列表
	public const short MSG_MATCHING_BEGING = 21;//开始匹配 (C->S)
	public const short MSG_MATCHING_CANCEL = 22;//取消匹配 (C->S)	
	public const short MSG_MATCHING_CONFIRM	= 23;//匹配确认 (C<=>S)

	public const short MSG_PVP_INVITE_FRIEND=30;//PVP邀请好友(C<=>S)
	public const short MSG_PVP_FRIEND_CONFIRM=31;//PVP被邀请者确认(C->S)
	public const short MSG_PVP_ENTER_CONFIRM=32;//PVP加入确认(S=>C)
	public const short MSG_PVP_MATCHING_BEGIN=33;//PVP匹配开始(C<=>S)
	public const short MSG_PVP_MATCHING_CANCEL=34;//PVP匹配退出(C<=>S)
	public const short MSG_PVP_MATCHING_LEAVE	= 35;// 玩家退出匹配组(C<=>S)
	public const short MSG_PVP_MATCHING_SYNINFO = 36;// 同步玩家信息(S=>C)
	public const short MSG_PVP_MATCHING_SUCESS = 37;	// 匹配成功(S->C)
}

public class TradeDefineManager
{
    public const short MSG_TRADE_OPENSHOP = 2;
    public const short MSG_TRADE_BUYGOODS = 1;
    public const short MSG_TRADE_QUICK_BUY_GOODS = 47;
    public const short MSG_TRADE_PAY_SUCESS = 49;//充值成功
    //竞拍UI数据下发
    public const short MSG_TRADE_AUCTION_UI            = 50;
    //竞拍某件物品
    public const short MSG_TRADE_AUCTION_GOODS         = 51;
    //随身商店数据下发
    public const short MSG_TRADE_CARRYSHOP_UI          = 52;
    //随身商店物品购买
    public const short MSG_TRADE_CARRYSHOP_BUY         = 53;
    //解锁随身商店格
    public const short MSG_TRADE_CARRYSHOP_UNLOCK      = 54;

    //public const short MSG_TRADE_SALEGOODS = 1;
    
}

/// <summary>
/// 定义一些数值，以后可能会通过配置方式
/// </summary>
public class ConfigDefineManager
{
    public const float DISTANCE_TO_ATTACK_ENEMY = 30;
    public const float DISTANCE_ARRIVED_TARGET = 2f;
    public const float DISTANCE_TRIGGER_ROTATION = 8f;
    public const float VALID_ATTACK_ANGLE = 120;
    public const float DISTANCE_ARRIVED_NPC = 25;
    public const float TIME_LOCK_STATE = 0.5f; //锁定状态时间，在状态机切换到某个状态后，在这个时间内状态不应该被影响
	public const float TIME_MOVE_SYNC_FOWARD = 0.25f;
}

public class ContainerDefineManager
{
    public const short MSG_CONTAINER_CREATE = 1;
    public const short MSG_CONTAINER_SYNC = 3;
    public const short MSG_CONTAINER_USE = 8;   
    public const short MSG_CONTAINER_OBSERVER = 9;
    public const short MSG_CONTAINER_DOFF = 10;
    public const short MSG_ACTION_USEMEDICAMENTRESULT = 11;
	public const short MSG_CONTAINER_TIDY = 12;
    public const short MSG_CONTAINER_GOODS_ADD = 14;
	public const short MSG_CONTAINER_GOODS_NEW = 15;	//更新物品是否为最新
	public const short MSG_CONTAINER_CHANGESIZE = 16;
    public const short MSG_ACTION_COLD_WORK = 50;
}
public class InputUtilDefineManager
{
    public const float ClickInternal = 0.2f;
    public const float DoubleInternal = 0.25f;
    public const float SlideTrackLength = 50f;
}

public class ConstDefineManager
{
    private const string BasePath = "assets/Resources/players/";
    public const string TexturePath = "assets/players/SwordMan/Textures/";
    public const string RoleModelPath = "assets/players/SwordMan/Res/";
    public const string RoleBaseAssetPath = BasePath + "BaseAssets/";
    public const string RoleAnimationPath = BasePath + "Animations/";
    public const string RoleMaterialPath = BasePath + "Mats/";
    public const string RoleSkinPath = BasePath + "SkinAssets/";

    public const string BaseShaderName = "JH/PlayerHurtFlash(With RimLight)";
    public const string BumpShaderName = "Bumped Diffuse";

    public const string LHWeaponPos = "Attachment-LHWeapon";  //左手挂载点
    public const string RHWeaponPos = "Attachment-RHWeapon"; //右手挂载点
    public const string LBWeaponPos = "Attachment-LBWeapon"; //左背挂载点
    public const string RBWeaponPos = "Attachment-RBWeapon"; //右背挂载点

    public static AttachPoint GetWeaponPosByName(string pointName)
    {
        AttachPoint attachPoint = AttachPoint.None;
        switch (pointName)
        {
            case ConstDefineManager.LHWeaponPos:
                attachPoint = AttachPoint.LHWeapon;
                break;
            case ConstDefineManager.RHWeaponPos:
                attachPoint = AttachPoint.RHWeapon;
                break;
            case ConstDefineManager.LBWeaponPos:
                attachPoint = AttachPoint.LBWeapon;
                break;
            case ConstDefineManager.RBWeaponPos:
                attachPoint = AttachPoint.RBWeapon;
                break;
        }

        return attachPoint;
    }
}


public class FriendDefineManager
{
    public const short MSG_FRIEND_FIND	= 0;		        //查找好友 根据名称查找玩家详细信息  
    public const short MSG_FRIEND_ADD = 1;					//添加好友
    public const short MSG_FRIEND_ANSWER_ADD = 2;			//添加好友应答(好友接受或者拒绝)
    public const short MSG_FRIEND_DELETE = 3;				//删除好友
    public const short MSG_FRIEND_UPDATE = 4;				//更新好友关系 密友 仇人 黑名单
                       //MSG_FRIEND_CHAT,				    //好友聊天消息
    public const short MSG_FRIEND_GETLIST = 5;				//我的好友列表
    public const short MSG_STRANGE_GETLIST = 6;			    //获取附近玩家列表
    public const short MSG_FRIEND_VIEW = 7;				    //好友信息查看
    public const short MSG_FRIEND_REMIND = 8;               //提醒好友上下线情况
    public const short SYS_FRIEND_CHANGESCENESERVER =9;		//切换场景服

	public const short MSG_FRIEND_GETREQUESTLIST = 12;		//获取好友请求列表

                    //交互消息 interactive
//    public const short ISEND_FRIEND_CHAT = 10;				//好友聊天信息发送
//    public const short IRECV_FRIEND_CHAT = 11;				//好友聊天信息接收
//    public const short IREFUSE_FRIEND_AFFIRM = 12;			//拒绝好友请求
//    public const short IACCEPT_FRIEND_AFFIRM = 13;			//通过好友请求
//    public const short ICHANGE_ONLINE_STATE = 14;			//上下线通知加了他为好友、临时好友、黑名单的玩家
 }

public class ErrorCodeDefineManager
{
    public const short TEAM_CODE_NOLIST=	501;            //没有队伍列表
    public const short TEAM_CODE_NOEXIST=	502;            //队伍不存在
    public const short TEAM_CODE_SELFHAVETEAM=	503;        //已经有队伍
    public const short TEAM_CODE_SELFHAVENTTEAM=	504;    //本身没有队伍
    public const short TEAM_CODE_TAGETHASTEAM=	505;        //目标已经拥有队伍
    public const short TEAM_CODE_TAGETHASNTTEAM=	506;    //目标没有队伍
    public const short TEAM_CODE_NOMEMBERFORTEAM=	507;    //不是队伍队员
    public const short TEAM_CODE_ISTEAMMEMBER=	508;        //已经是队伍队员
    public const short TEAM_CODE_NOCAPTAIN = 509;           //不是队长
    public const short TEAM_CODE_TEAMFULL = 510;            //目标队伍人数已满

}

public class EmailDefineManager
{
    public const short MSG_EMAIL_NONE_TYPE          = 0,
    MSG_EMAIL_NOREAD_EMAIL      = 1,    //  登录下发未读邮件通知
    MSG_EMAIL_OPEN_UI_TYPE      = 2,    //  打开邮件UI时下发邮件数据
    MSG_EMAIL_SEND              = 3,    //  写邮件
    MSG_EMAIL_UPDATE            = 4,    //  玩家更新邮件
    MSG_EAMIL_DEL               = 5,    //  邮件删除
    MSG_EMAIL_ALLGOODSGET       = 6,    //  一键获取所有附件
    MSG_EMAIL_ALLDEL            = 7,    //  一键删除所有不带附件的邮件
    MSG_EMAIL_LOGINOUT          = 8,   //  角色登出时，邮件处理，客户端不用管
    MSG_EMAIL_READ              = 9;    //  阅读邮件   
    
    
}

public class ChatDefineManager
{
    public const short MSG_CHAT_BEGIN = -1;
	public const short MSG_CHAT_CURRENT=0;		//当前
	public const short MSG_CHAT_WORLD=1;		//世界
	public const short MSG_CHAT_GROUP=2;		//阵营
	public const short MSG_CHAT_TIP=12;		//悬浮提示
	public const short MSG_CHAT_SPACE=32;		//附近频道
	public const short MSG_CHAT_TRUMPET=18;	//广播频道
	public const short MSG_CHAT_TEAM=3;		//组队
	public const short MSG_CHAT_FACTION=4;		//帮派
	public const short MSG_CHAT_FAME=5;		//传闻
	public const short MSG_CHAT_RUMOR=6;		//谣言
	public const short MSG_CHAT_COUPLE=7;		//夫妻
	public const short MSG_CHAT_PRIVATE=8;		//私聊
	public const short MSG_CHAT_TEMP=9;		//零时(本地系统提示)
	public const short MSG_CHAT_SYSTEM=64;		//系统提示
	public const short MSG_CHAT_ROLLTIP=11;	//滚动提示
	public const short MSG_CHAT_GETITEMTIP=13;	//获得物品的时候的提示，带图标的
	public const short MSG_CHAT_MONSTERBUBBLE=14; //怪物头顶泡泡
	public const short MSG_CHAT_FIRE=15;		//战斗
	public const short MSG_CHAT_NUMBER=16;		//播放数字频道
	public const short MSG_CHAT_ALL=17;		//所有频道
	public const short MSG_CHAT_END = -2;
}

public class SystemErrorCodeDefine
{
    //////////////////////////////////////////////////数据库操作/////////////////////////////////////////////
    public const short ERROR_CODE_DBRET_ERROR_LOGIC		= -1;	// <=该值的为逻辑错误(输入异常 数据库返回)，具体由DB定义(对应DBRET_ERROR_LOGIC)
	public const short ERROR_CODE_INVALIDUSER				= -2;	//无此用户
    public const short ERROR_CODE_KEYERROR = -3;	//平台Key或者SID错误 或者 格式错误 (为空 等等)(密码错误)
	public const short ERROR_CODE_CREATEFAILED_NULLNAME	= -4;	//创建人物失败 角色名为空
	public const short ERROR_CODE_CREATEFAILED_LOGGEDIN	= -5;	//创建人物(登录)失败 账号已登录 倒计时x秒 重新操作(服务器在踢人)
	public const short ERROR_CODE_CREATEFAILED_DUPLICATE	= -6;	//创建人物失败 角色名重复
	public const short ERROR_CODE_CREATEFAILED_MAXNUM		= -7;	//创建人物失败 角色达到上限
	public const short ERROR_CODE_NOACTOR					= -8;	//无此角色
    public const short ERROR_CODE_PADLOCK                   = -9;	//帐号被禁
	public const short ERROR_CODE_PADLOCKACTOR				= -10;	//角色已被禁止登陆
	public const short ERROR_CODE_DELETEFAILED				= -11;	//删除人物失败
    //////////////////////////////////////////////////系统///////////////////////////////////////////////////
    public const short ERROR_CODE_LOGINBUSY				=1;			//登陆服务器忙
	public const short ERROR_CODE_SCENEBUSY				=2;			//场景服务器忙
	public const short ERROR_CODE_NOFINDSCENE				=3;			//找不到指定场景服务器
	public const short ERROR_CODE_INVALIDCITY				=4;			//人物所在地图不合法
	public const short ERROR_CODE_SERVER_NOTREADY			=5;			//服务器未开启
	public const short ERROR_CODE_INVALIDLOC				=6;			//人物坐标不合法
	public const short ERROR_CODE_PUTACTORDATA				=7;			//提交的角色数据不正确
	public const short ERROR_CODE_TANSMAP					=8;			//切换地图失败
	public const short ERROR_CODE_KICK						=9;			//服务器主动踢人
	public const short ERROR_CODE_JUMP_START				=10;		//已登录玩家跳转开始处理失败
	public const short ERROR_CODE_JUMP_FINISH				=11;		//已登录玩家跳转完成处理失败
	public const short ERROR_CODE_SERVER_STOP				=12;		//场景服正在关闭
	public const short ERROR_CODE_USER_REGISTER			=13;		//注册帐号失败
	public const short ERROR_CODE_ADD_PP					=14;		//添加密保资料失败
	public const short ERROR_CODE_GAME_WORLD_EMPTY 		=15;		//没有可登录的游戏世界
	public const short ERROR_CODE_USER_CENTER_DISCONNECT	=16;		//通知客户端，用户中心断开连接了
	public const short ERROR_CODE_KICK_SYS = 17;										//服务器主动踢人（系统相关 封号）
	public const short ERROR_CODE_VERSION_NOTSUPPORTED = 18;			//服务器-客户端版本不匹配,请更新
	public const short ERROR_CODE_END						=100;		//系统提示消息结束
	//////////////////////////////////////////////////聊天///////////////////////////////////////////////////
	public const short CHAT_CODE_XXX						= ERROR_CODE_END+1;
    public const short CHAT_CODE_FORBID                     = 101;	//角色聊天被禁言
	public const short CHAT_PLAYER_OFFLINE					= 102;	//私聊对象不在线
	public const short CHAT_CODE_END						=200;		//聊天提示消息结束
	//////////////////////////////////////////////////实体///////////////////////////////////////////////////
	public const short ENTITY_CODE_NOENOUGHBINDPAY			= 201;		// 没有足够的元宝 (200+1)
	public const short ENTITY_CODE_NOENOUGHHOLDMONEY		= 202;					// 没有足够的铜币
	public const short ENTITY_CODE_NOENOUGHACTIVELIFE		= 203;					// 没有足够活力
	public const short ENTITY_CODE_END						=300;		//实体提示消息结束
	//////////////////////////////////////////////////任务///////////////////////////////////////////////////
	public const short TASK_CODE_END						= 400;
	//////////////////////////////////////////////////物品///////////////////////////////////////////////////
	public const short GOODS_CODE_PACKETFULL				= 401;		// 背包已满 (400+1 = 401)
	public const short GOODS_CODE_END						= 500;
	//////////////////////////////////////////////////组队///////////////////////////////////////////////////
	public const short TEAM_CODE_NOLIST					= 501 ;	// 没有队伍列表 (500+1 = 501)
	public const short TEAM_CODE_NOEXIST				    = 502;					// 队伍不存在
	public const short TEAM_CODE_SELFHAVETEAM				= 503;					// 自己本身已经拥有队伍
	public const short TEAM_CODE_SELFHAVENTTEAM			= 504;					// 自己本身没有队伍
	public const short TEAM_CODE_TARGETHASTEAM				= 505;					// 目标已经拥有队伍
	public const short TEAM_CODE_TARGETHASNTTEAM			= 506;					// 目标没有队伍
	public const short TEAM_CODE_NOMEMBERFORTEAM			= 507;					// 不是队伍队员 自己还是队友都发送这个消息
	public const short TEAM_CODE_ISTEAMMEMBER				= 508;					// 已经是队伍队员
	public const short TEAM_CODE_NOCAPTAIN					= 509;					// 不是队长
	public const short TEAM_CODE_TEAMFULL					= 510;					// 队伍人数已满
    public const short TEAM_CODE_TEAMFIGHTING              = 511;                  // 队伍正在战斗
    public const short TEAM_CODE_TEAMMEMBERNOREADY         = 512;                  // 队员未准备
    public const short TEAM_CODE_ECTYPE_UNLOCK              = 513;                  //关卡未解锁
    public const short TEAM_CODE_TEAM_REJECT                = 515;                  //队员拒绝
    public const short TEAM_CODE_TEAM_TIMELIMIT             = 516;				    //角色加入队伍已经被踢出过，需要等待一定的时间再加入
	public const short TEAM_CODE_END						= 600;
	//////////////////////////////////////////////////交易///////////////////////////////////////////////////
    public const short TRADE_CODE_SALE                      =601;
	public const short TRADE_AUCTION_OUTDATE				= 602;
	public const short TRADE_CODE_END						= 700;
	//////////////////////////////////////////////////好友///////////////////////////////////////////////////
	public const short FRIEND_CODE_FULL					= 701;		//好友已满
	public const short FRIEND_CODE_EXIST					= 702;						//好友已经存在
	public const short FRIEND_CODE_ISOFFLINE				= 703;						//好友已经下线
	public const short FRIEND_CODE_OTHERISFULL				= 704;						//对方好友已满
	public const short FIREND_CODE_OFFLINE					= 705;						//对方不在线
	public const short FRIEND_CODE_END						= 800;						//
	//////////////////////////////////////////////////副本///////////////////////////////////////////////////
	public const short ECTYPE_CODE_HAVEECTYPE			    = 801;	 // 已经拥有副本(800+1 = 801)
	public const short ECTYPE_CODE_NOQUALIFICATION			= 802; 
	public const short ECTYPE_CODE_NOTALLREADY				= 803;
    public const short ECTYPE_CODE_PLAYERLEAVE             = 804;                   // 玩家离开提示使用
    public const short ECTYPE_CODE_NOENTERTIMES = 805;                   // 没有进入的次数了
    public const short ECTYPE_CODE_PVPREMATCHING            = 806;   // pvp匹配错误码消息, 重新匹配
    public const short ECTYPE_CODE_PVPNOTIMES               = 807;   // pvp挑战次数不足
    public const short ECTYPE_CODE_PLAYERLEVEL              = 808;                   // 等级不满足, 大于最大或小于最小
	public const short ECTYPE_CODE_ROMMNOFOUND				= 809;					 // 房间不存在	
    public const short ECTYPE_CODE_PLAYERNUMMAX             = 810;			 // 人数已满
	public const short ECTYPE_CODE_NOITEM				    = 815;			 // 没有物品
	public const short ECTYPE_CODE_END						= 900;
    //////////////////////////////////////////////////技能///////////////////////////////////////////////////
	public const short SKILL_CODE_SKILLRESULT				= 901;
    public const short SKILL_CODE_END                       = 1000;
}

public class VersionDefine
{
	public const bool IS_TEST_VERSION = false;
	
}