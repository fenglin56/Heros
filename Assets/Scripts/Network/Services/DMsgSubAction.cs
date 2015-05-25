//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Runtime.InteropServices;
//
//// 事件类型ID定义
//enum emEventType
//{
//    SRC_TYPE_ID_ROOT = 0,	// 根
//    SRC_TYPE_ID_SYSTEM,				// 系统
//    SRC_TYPE_ID_ACTOR,				// 玩家
//    SRC_TYPE_ID_NPC,				// NPC
//    SRC_TYPE_ID_MONSTER,			// 怪物 
//    SRC_TYPE_ID_PET,				// 宠物
//    SRC_TYPE_ID_ITEM,				// 物品
//    SRC_TYPE_ID_CONTAINER,			// 容器
//    SRC_TYPE_ID_CHAT,				// 聊天 
//    SRC_TYPE_ID_BOX,				// 盒子 
//    SRC_TYPE_ID_CHUNNEL,			// 传送门类
//    SRC_TYPE_ID_TEAM,				// 组队
//    SRC_TYPE_ID_GAMEOBJ,			// 场景物件
//    SRC_TYPE_ID_UNION,				// 家族
//    SRC_TYPE_ID_FIGHT,              // 战斗
//    SRC_TYPE_ID_COMMAND,            // 指令
//    SRC_TYPE_ID_BUFF,               // BUFF
//    SRC_TYPE_ID_DAMAGE,             // 伤害
//    SRC_TYPE_ID_FRIEND,				// 好友
//    SRC_TYPE_ID_TRADE,				// 交易相关
//    SRC_TYPE_ID_ECTYPE,				// 副本相关
//    SRC_TYPE_ID_EMAIL,				// 邮件
//    SRC_TYPE_ID_BRUSHNPC,			// 刷NPC类型相关
//    SRC_TYPE_ID_COLD,		// 冷却时间 超时
//    SRC_TYPE_ID_MAX
//};
////////////////////////////////////////////////////////////////////////////
///////////////////////// SRC_TYPE_ID_ACTOR 实体行为事件的子消息码 ///////////////////////////
//enum MAINACTION_MSGID
//{
//    MAINACTION_MSGID_BEGIN = EventDefineManager.EVENT_SYSTEM_EVENTEND,
//
//	// 发送系统消息
//	MAINACTION_MSGID_SYSTEMMESSAGE = MAINACTION_MSGID_BEGIN,
//
//	// 玩家登录成功
//	MAINACTION_MSGID_PLAYERLOGIN,
//
//	// 玩家登出成功
//	MAINACTION_MSGID_PLAYERLOGOUT,
//
//	//切换场景服务器 跨服事件
//	MAINACTION_MSGID_TRANSMAP,
//
//	// 登录数据库返回，通知其他模块加载数据库
//	MAINACTION_MSGID_PLAYERLOADDATABASE,
//
//	// 玩家定时保存数据
//	MAINACTION_MSGID_PLAYERSAVE,
//
//	// 逻辑对象创建 
//	MAINACTION_MSGID_CREATE ,
//
//	// 逻辑对象销毁
//	MAINACTION_MSGID_DESTORY,
//
//	// 移动
//	MAINACTION_MSGID_MOVE,
//
//	// 站立
//	MAINACTION_MSGID_STOPHERE,
//
//	// 客户端站立当前位置
//	MAINACTION_MSGID_STAND,
//
//	// 客户端使用 上行移动信息到服务器的
//	MAINACTION_MSGID_PREPMOVE,
//
//	// 死亡
//	MAINACTION_MSGID_DIE,
//
//	// 上马
//	MAINACTION_MSGID_RIDEMOUNT,
//
//	// 切换场景
//	MAINACTION_MSGID_ACTOR_CHANGE_MAP,
//
//	// 切换场景 完毕
//	MAINACTION_MSGID_ACTOR_CHANGE_MAPOK,
//	// 玩家升级
//	MAINACTION_MSGID_LEVELUP,
//
//	// 玩家分配属性点
//	MAINACTION_MSGID_ATTR_ASSIGN,
//
//	// 玩家自动恢复血蓝精力(包括召唤兽)
//	MAINACTION_MSGID_ATTR_AUTORECRUIT,
//
//	// 宠物分配属性点
//	MAINACTION_MSGID_ATTR_ASSIGN_PET,
//
//	// 放生宠物
//	MAINACTION_MSGID_PET_DELETE,
//
//	// 点击宠物参战按钮
//	MAINACTION_MSGID_PET_FIGHT,
//
//	// 修改宠物名字（还原宠物名）
//	MAINACTION_MSGID_PET_RENAME,
//
//	// 宠物喂养
//	MAINACTION_MSGID_PET_FEED,
//
//	// 宠物绑定
//	MAINACTION_MSGID_PET_BLIEND,
//
//	// 宠物变动 获得 失去
//	MAINACTION_MSGID_PET_EXCHANGE,
//
//
//	////////////////////////////////////////////////////////
//	//描        述：任务数据加载ok
//	//支持流通类型：任务系统
//	/////////////////////////////////////////////////////////
//	MAINACTION_MSGID_TASK_DATA_LOADED_OK,
//
//	// 任务进度条事件开始
//	MAINACTION_MSGID_EVENT_PROGRESSBARSTART,
//
//	// 任务进度条事件结束
//	MAINACTION_MSGID_EVENT_PROGRESSBAREND,
//
//	// 玩家状态切换
//	MAINACTION_MSGID_PLAYER_SWITCHSTATE,
//
//	// 添加buff 事件
//	MAINACTION_MSGID_CREATURE_ADDBUFF,
//
//	// 生物buff移除
//	MAINACTION_MSGID_CREATURE_REMOVEBUFF,
//
//	// 使用药品
//	MAINACTION_MSGID_PLAYER_USEMEDICAMENT,
//
//	// 服务器使用道具(穿装备)
//	MAINACTION_MSGID_ONEQUIP,
//
//	// 服务器使用道具(卸装备)
//	MAINACTION_MSGID_UNEQUIP,
//
//	// 整理包裹
//	MAINACTION_MSGID_PACKET_TIDY,
//
//	// 背包发生变化
//	MAINACTION_MSGID_PACKET_CHG,
//
//	// 移动完成
//	EVENT_CREATURE_DONEMOVE,
//	
//	//实体位置变更 宠物
//	EVENT_CREATURE_CHANGELOC,
//
//	// 组队变更事件处理
//	MAINACTION_MSGID_TEAM_CHANGE,
//
//	// 好友变更事件处理
//	MAINACTION_MSGID_FRIEND_CHANGE,
//
//	// 改变自身模型 - 变身
//	MAINACTION_MSGID_CHANGE_MODEL,
//
//	// 属性更新事件定义
//	MAINACTION_MSGID_UPDATEPROP,
//
//	// 战斗中使用普通攻击
//	MAINACTION_MSGID_CMD_ATTACK,
//
//	// 战斗中使用招式指令
//	MAINACTION_MSGID_CMD_SKILL,
//
//	// 战斗中使用物品指令
//	MAINACTION_MSGID_CMD_ITEM,
//
//	// 战斗中使用防御(格挡)指令
//	MAINACTION_MSGID_CMD_DEFFEND,
//
//	// 战斗中使用召唤指令
//	MAINACTION_MSGID_CMD_CONJURE,
//
//	// 战斗开始
//	EVENT_FIGHTSTART,
//
//	// 战斗结束
//	EVENT_FIGHTOVER,
//
//	// 回合开始
//	EVENT_FIGHTROUNDSTART,
//
//	// 回合结束
//	EVENT_FIGHTROUNDOVER,
//
//	// 填充指令
//	EVENT_FIGHTFILLCMD,
//
//
//	// 与NPC对话
//	MAINACTION_MSGID_MEETNPC,
//
//	/////////////////////////////////////////////////////////
//	//描        述： 任务完成监控事件消息
//	//支持流通类型：SS
//	MAINACTION_MSGID_TASK_SYS_TASK_ACCEPT,
//	MAINACTION_MSGID_TASK_SYS_TASK_FINISH,
//
//	/////////////////////////////////////////////////////////
//
//	/////////////////////////////////////////////////////////
//	//消  息    MAINACTION_MSGID_MURDEROUS
//	//描        述：杀手对敌人造成死亡
//	//支持流通类型：SS
//	/////////////////////////////////////////////////////////
//	MAINACTION_MSGID_MURDEROUS,
//
//	// 点传送通道
//	MAINACTION_MSGID_CLICKCHUNNEL,
//
//	// 人物外形数据
//	MAINACTION_MSGID_PERSONFORM,
//
//	// 查看角色信息
//	MAINACTION_MSGID_LOOK_ACTOR_DATA,
//
//	// 打开盒子
//	MAINACTION_MSGID_OPENBOX,
//
//	// 实体被伤害
//	EVENT_CREATURE_INJURED,
//
//	// 实体主动伤害
//	EVENT_CREATURE_HARM,
//
//	// 设置巡逻定时器
//	EVENT_AI_PATROL_TIMER,
//
//	// 组队发生 暂离归队改变消息
//	EVENT_TEAM_CHEANGE,
//
//	// 创建队伍或者加入队伍
//	EVENT_TEAM_CREAT_OR_ADD,
//
//	// 广播伤害执行
//	MSG_ACTION_PREPARE_DAMAGE_RUN,
//
//	// OSS任务统计相关
//	MAINACTION_MSGID_OSS_TASK,
//
//	/////////////////////////////////////////////////////////
//	//描        述：技能事件
//	/////////////////////////////////////////////////////////
//	// 初始化技能
//	MAINACTION_MSGID_INIT_SKILL,
//	// 添加技能
//	MAINACTION_MSGID_ADD_SKILL,
//	// 删除技能
//	MAINACTION_MSGID_DEL_SKILL,
//	// 替换技能
//	MAINACTION_MSGID_REPLACE_SKILL,
//	// 升级技能
//	MAINACTION_MSGID_UPGRADE_SKILL,
//	// 使用技能
//	MAINACTION_MSGID_USE_SKILL,
//
//	/////////////////////////////////////////////////////////
//	//描        述：邮件事件
//	/////////////////////////////////////////////////////////
//	// 删除邮件
//	MAINACTION_MSGID_DESTROY,
//
//	// 设置获取组队移动位置
//	MAINACTION_MSGID_TEAMMOVEPOS,
//
//	MAINACTION_MSGID_END,
//
//};
//
//
//
//// 场景服
//struct SEventCreatureDoneMove_S
//{
//
//};
//
//// 实体位置变更
//// 场景服
////struct SEventCreatureChangeLoc_S
////{
////    uint		    dwOldZoneID;	// 旧场景ID
////    POINT2			ptOldTile;		// 旧Tile
////    uint			dwNewZoneID;	// 新场景ID
////    POINT2			ptNewTile;		// 新Tile
////    bool			bSameServer;	// 旧场景与新场景是否为同服务器
////};
//
//// 客户端
////struct SEventCreatureChangeLoc_C
////{
////    POINT2			ptOldTile;		// 旧Tile
////    POINT2			ptNewTile;		// 新Tile
////};
//
//////////////////////////////////////////////////////////////////////////
//// 切换地图
////struct SS_TANSMAP_ACTION_CONTEXT
////{
////    enum TANSMAP_MODE
////    {
////        TANSMAP_MODE_NEXTZONE = 0,
////    };
////    long	lTansMode;
////    long	lTargetMapID;
////    POINT	ptTargetPos;
////    //SS_TANSMAP_ACTION_CONTEXT()
////    //{
////    //    memset( this, 0, sizeof(*this) );
////    //}
////};
//
//struct SS_ActorChangeMap
//{
//    public uint dwActorID;		// 角色ID
//    public uint dwOldSceneID;	// 旧场景ID
//    public uint dwNewSceneID;	// 新场景ID
//    public bool bFlgChangeScene;	// 是否改变场景服务器 false 不更换场景， true 更换场景
//    //SS_ActorChangeMap()
//    //{
//    //    memset(this,0,sizeof(*this));
//    //}
//};
//
////MAINACTION_MSGID_PACKET_CHG
//struct SEventPlayerPacketChg
//{
//    public uint lGoodsID;
//    //SEventPlayerPacketChg()
//    //{
//    //    memset(this,0,sizeof(*this));
//    //}
//};
//
//
///***************************************************************/
/////////////////////////// 系统事件码 ////////////////////////////
///***************************************************************/
//
//// 场景服（vote / execute）
//struct SEventSystemBuildZone_S
//{
//    public uint dwMapID;		// 地图ID
//    public uint dwZoneID;		// 场景ID
//};
//
//// 客户端
//struct SEventSystemBuildZone_C
//{
//    public uint dwMapID;		// 地图ID
//    public uint dwZoneID;		// 场景ID
//};
//
// // 场景服（vote / execute）
//struct SEventSystemDestoryZone_S
//{
//    public uint dwMapID;		// 地图ID
//    public uint dwZoneID;		// 场景ID
//};
//
//// 客户端
//struct SEventSystemDestoryZone_C
//{
//
//};
//
//// 场景服登录或者登出商城服
////EVENT_SYSTEM_MS_LOGINOUT,
//struct SEventMSLoginOut_S
//{
//    public byte chType;			//1：场景服登录，2：场景服登出
//    public uint dwAppServer;		//对应场景服IAppServer的指针
//
//    //SEventMSLoginOut_S()
//    //{
//    //    chType = 0;
//    //    dwAppServer = 0;
//    //}
//};
//
//
//
//// 创建实体 MAINACTION_MSGID_CREATE
//// 场景服
//struct SEventEntityCreateEntity_S
//{
//    public Int64 uidEntity;		// 实体uid
//};
//
//// 属性更新 MAINACTION_MSGID_UPDATEPROP
//// 客户端
//struct SEventEntityUpdateProp_C
//{
//    public ushort wPropId;	// 属性ID
//    public int nValue;		// 属性值
//};
//
//// 怪物或者玩家死亡
//// 场景服
//struct SEventCreatureDie_S
//{
//    public Int64 uidMurderer;		// 谋杀者的uid
//};
//// 客户端
//struct SEventCreatureDie_C
//{
//    public Int64 uidMurderer;	// 凶手
//};
//
//// 删除实体
//// 场景服
//struct SEventEntityDestroryEntity_S
//{
//    public Int64 uidEntity;		// 实体uid
//};
//// 客户端
//struct SEventEntityDestroryEntity_C
//{
//    public Int64 uidEntity;		// 实体uid
//};
//
//// 服务器，设置宠物参战
//struct SEventSetPetFight_S
//{
//    public Int64 uidEntity;		// 实体uid 玩家
//    public long lPetModelID;	// 宠物模板ID
//    public byte byFightFlag;	// 0:休息 1:参战 
//};
//
////MAINACTION_MSGID_PET_EXCHANGE
//// 服务器，宠物变化
//struct SEventEntityPetExchange_S
//{
//    public Int64 uidEntity;		// 实体uid 玩家
//    public long lPetModelID;	// 宠物模板ID
//    public byte byExchangeFlag;	// 0:失去 1:获得 
//};
////MAINACTION_MSGID_CREATURE_ADDBUFF
//struct SEventCreatureAddBuff_S
//{
//    public uint dwBuffID;		// buff id
//    public uint dwLevel;		// 等级
//    public Int64 uidEffect;		// 本buff的作用对像
//    public Int64 uidAdd;			// 本buff的添加者
//    public uint dwBuffFlag;		// buff标志
//};
//// 客户端
//struct SEventCreatureAddBuff_C
//{
//    public uint dwIndex;		// buff index
//    public uint dwFlashID;		// 光效ID
//    public uint dwRandFlashID;	// 随机效果光效
//};
//
//// MAINACTION_MSGID_CREATURE_REMOVEBUFF
//// 场景服（vote/execute）
//struct SEventCreatureRemoveBuff_S
//{
//    public uint dwBuffID;		// buff id
//    public uint dwLevel;		// 等级
//    public Int64 uidEffect;		// 本buff的作用对像
//    public Int64 uidAdd;			// 本buff的添加者
//    public uint dwBuffFlag;		// buff标志
//
//    public Int64 uidRemove;		// 移除者
//};
//
//// 客户端
//struct SEventCreatureRemoveBuff_C
//{
//    public uint dwIndex;		// buff index
//};
//
//// 人物进度条状态开始事件
//// 场景服（vote/execute）
//// MAINACTION_MSGID_EVENT_PROGRESSBARSTART
//struct SEventPersonProgressBarStart_S
//{
//    public byte byBarType;					// 进度条类型
//    public int nTime;						// 持续时间
//    [MarshalAsAttribute(UnmanagedType.ByValArray,SizeConst = 32)]
//    public char[] szName;				// 进度条的名字
//};
//
//// 场景服（vote/execute）
//// MAINACTION_MSGID_EVENT_PROGRESSBAREND
////struct SEventPersonProgressBarEnd_S
////{
////    enum
////    {
////        PB_INVALID = 0,
////        PB_WORK_SKILL,				// 工作技能进度条
////        PB_OPEN_DOOR,				// 打开场景物件门进度条
////        PB_STAND_FLAG,				// 打开场景物件占领点进度条
////        PB_OPEN_BOX,				// 打开场景盒子进度条
////        PB_RIDE_STATE,				// 骑马状态
////        PB_TREASURE_IDENTIFY,		// 藏宝图鉴定进度条
////        PB_TREASURE_USE,			// 藏宝图使用进度条
////    };
//
////    int			nEndReason;			// 结束原因
////    Int64	uidMaster;			// 进度条部件的主人
////    char		userData[128];		// 第一个字节用于放进度条类型
////    int			nDataLen;
////};
//
///***************************************************************/
///////////////////// 通用物品篮事件码 ////////////////////////////
///***************************************************************/
//// 向通用物品篮添加物品
//// EVENT_CONTAINER_ADDCONTAINERGOODS
//// 场景服（vote / execute）
///* 只有凭空添加物品时，才会触发此事件，比如从商店里买进一个物品就
//会触发此事件，从一个物品篮转移到另一个物品篮是不会触发此事件，比
//如从仓库拖物品到包裹栏。也就只有实在通用物品栏才会触发//*/
//struct SEventContainerAddContainerGoods_S
//{
//    public Int64 uidOperater;	// 操作者
//    public Int64 uidGoods;		// 物品UID
//    public int nPlace;			// 添加位置
//};
//
//// 向通用物品篮删除物品
//// EVENT_CONTAINER_REMOVECONTAINERGOODS
//// 场景服（vote / execute）
///* 只有凭空移除物品时，才会触发此事件，比如任务NPC从包裹栏收走任务
//物品。也就只有实在通用物品栏才会触发//*/
//struct SEventContainerRemoveContainerGoods_S
//{
//    public Int64 uidOperater;	// 操作者
//    public Int64 uidGoods;		// 物品UID	
//    public int nPlace;			// 移除位置
//};
//
//// 清空通用物品篮
//// EVENT_CONTAINER_CLEANSASHCONTAINE
//// 场景服（vote / execute）
///* 外部调用IContainer::Clean清空物品篮时，会触发此事件，不论链接物品篮还
//是实在物品篮都会触发//*/
//struct SEventContainerCleanSashContainer_S
//{
//    public Int64 uidOperater;	// 操作者
//};
//
//
//// 链接物品
//// EVENT_CONTAINER_LINKCONTAINERGOODS
//// 场景服（vote / execute）
///* 链接物品栏：将当物品从一个物品篮拖到链接物品栏时，在做真正执行操
//作时，触发否决事件。执行操作有以下几种结果：
//1、将本物品栏的物品从一个位置移动到别的位置
//2、将其他实在物品栏链接到本物品栏
//实在物品栏：将当物品从一个物品栏拖到实在物品栏时，在做真正执行操
//作时，触发否决事件，执行操作有以下几种结果：
//1、如果是链接物品，则还原物品链接关系，使本物品篮不
//再链接了。成功后，发EVENT_CONTAINER_LINKCONTAINERGOODS事件
//2、如果是实在物品，并且目标位置没有物品，则转移此物
//品从本物品篮至本物品篮，比如从仓库拖到物品栏，成
//功后发EVENT_CONTAINER_LINKCONTAINERGOODS.
//3、如果是实在物品，并且目标位置有物品，则有可能需要
//触发合并事件，当触发EVENT_CONTAINER_UNITECONTAINERGOODS事件，
//则表示合并成功，也就是全部合并成功
//4、如果不能合并，就会在两个实在物品栏之间交换物品，
//先发EVENT_CONTAINER_EXCHANGECONTAINERGOODS投票事件，如果没
//有否决则会执行交换事件，再发
//EVENT_CONTAINER_EXCHANGECONTAINERGOODS执行事件。不再发
//EVENT_CONTAINER_LINKCONTAINERGOODS事件了
////*/
//struct SEventContainerLinkContainerGoods_S
//{
//    public Int64 uidOperater;	// 操作者
//    public Int64 uidGoods;		// 物品UID
//
//    public uint dwSrcContainerID;	// 源物品篮
//    public uint dwSrcPlace;		// 源位置
//
//    public uint dwTargetContainerID;	// 目标物品篮
//    public int nTargetPlace;	// 目标位置
//    public byte byTargetNum;		// 链接到目标容器 物品数量
//};
//
//// 客户端（vote）
//struct SEventContainerLinkContainerGoods_C
//{
//    public Int64 uidGoods;		// 物品UID
//
//    public uint dwSrcContainerID;	// 源物品篮
//    public uint dwSrcPlace;		// 源位置
//
//    public uint dwTargetContainerID;	// 目标物品篮
//    public int nTargetPlace;	// 目标位置
//    public byte byTargetNum;		// 链接到目标容器 物品数量
//};
//
//// 交换物品
//// EVENT_CONTAINER_EXCHANGECONTAINERGOODS	
//// 场景服（vote / execute）
//struct SEventContainerExchangeContainerGoods_S
//{
//    public Int64 uidOperater;	// 操作者
//
//	//////////////////////////////////////////////
//    public uint dwContainerID1;		// 物品篮1
//    public int nContainerPlace1;	// 物品篮1位置
//    public Int64 uidGoods1;		// 物品UID
//
//	//////////////////////////////////////////////
//    public uint dwContainerID2;		// 物品篮2
//    public int nContainerPlace2;	// 物品篮2位置
//    public Int64 uidGoods2;		// 物品UID
//};
//
//// 合并物品（只适用于全部合并）
//// EVENT_CONTAINER_UNITECONTAINERGOODS	
//// 场景服（execute）
//struct SEventContainerUniteContainerGoods_S
//{
//    public Int64 uidOperater;	// 操作者
//    public Int64 uidGoods;		// 物品UID
//
//    public uint dwContainerID;		// 物品篮
//    public int nPlace;			// 物品篮位置
//
//    public int nUniteQty;		// 合并数量
//
//    public bool bAddTime;		// 是否为添加时
//
//    public bool bLinkTime;		// 是否为链接时
//    public uint dwSrcContainerID;	// 链接时的源物品篮
//};
//
//// 通用物品篮拆分物品
//// EVENT_CONTAINER_SPLITGOODS
//// 场景服（vote / execute）
//struct SEventContainerSplitGoods_S
//{
//    public Int64 uidOperator;	// 操作者
//    public Int64 uidGoods;		// 物品UID
//
//    public uint dwContainerID;		// 物品篮ID
//    public uint dwPlace;		// 位置	
//
//    public uint dwSplitNum;		// 拆分数量	
//
//    public Int64 uidNewGoods;	// 拆分后物品
//};
//
//// 客户端
//struct SEventContainerSplitGoods_C
//{
//    public Int64 uidGoods;		// 物品UID
//
//    public uint dwContainerID;		// 物品篮ID
//    public uint dwPlace;		// 位置	
//
//    public uint dwSplitNum;		// 拆分数量		
//};
//
//
//// 通用物品篮内某位置发生了变化
//// EVENT_CONTAINER_PLACECHANGE	
//// 客户端
//struct SEventContainerPlaceChange_C
//{
//    public uint dwContainerID;		// 物品篮ID
//    public uint dwPlace;		// 位置
//};
//
//// 通用物品篮内修改了容量
//// EVENT_CONTAINER_CHANGESIZE
//// 客户端
//struct SEventContainerChangeSize_C
//{
//    public uint dwContainerID;		// 物品篮ID
//    public uint dwMaxSize;		// 大小
//};
//
//
// 
//// 通用物品篮内要释放了
//// EVENT_CONTAINER_DESTORYCONTAINER		
//// 服务器
//struct SEventContainerDestoryContainer_S
//{
//    public uint dwContainerID;		// 物品篮ID
//}; 
//
//// 客户端
//// 内部不负责释放客户端skep，因为担心外部很多地方
//// 保存了它的指针，让他的管理者来释放，比如物品篮
//// 的skep，让物品篮来释放
//struct SEventContainerDestoryContainer_C
//{
//    public uint dwContainerID;		// 物品篮ID
//}; 
//
//// 消息体 MAINACTION_MSGID_PLAYER_USEMEDICAMENT
//struct SEventPlayerUseMedicament_C
//{
//    public uint dwContainerID;
//    public Int64 uidGoods;
//    public Int64 dwPlace;
//};
//struct SEventContainerUseMedicament_SS
//{
//    public Int64 uidMedicament;	// 药品uid
//};
//
///***************************************************************/
///////////////////// 交易相关事件码 ////////////////////////////
///***************************************************************/
//// 玩家给予(玩家\NPC)
////EVENT_TRADE_GIVE,
//
//
//// 玩家交易
////EVENT_TRADE_TRADE,
//
//// 玩家摆摊
////EVENT_TRADE_STALL_CONFIRM,
//struct SEventTradeStallConfirm
//{
//    [MarshalAsAttribute(UnmanagedType.ByValArray,SizeConst = 18)]
//    public char[] szTitle;
//};
//// 玩家摆摊取消
////EVENT_TRADE_STALL_CANCEL,
//struct SEventTradeStallCancel
//{
//	
//};
//// 玩家关注
//struct SEventTradeStallAttention_C
//{
//    public Int64 uidTarget;		// 关注目标
//    public bool bAttention;	// false:取消关注 true:关注
//};
//
//
//
////场景服 组队情况变更
////子消息码 MAINACTION_MSGID_TEAM_CHANGE
////struct STeamChangeInfo_SS
////{
////    enum
////    {
////        STC_CREATE = 10,
////        STC_Join,
////        STC_Leave,
////        STC_Kick,
////        STC_CaptainChange,
////        STC_CommanderChange,
////    };
//
////    byte bChangeType;     //变更类型 见上面枚举
////    uint dwTeamID;       //队伍ID
////    uint dwMemberID;     //变更成员ID
////};
//
////场景服 好友情况变更
////子消息码 MAINACTION_MSGID_FRIEND_CHANGE
////struct SFriendChangeInfo_SS
////{
////    enum
////    {
////        STC_ADD = 10,
////    };
//
////    byte bChangeType;	// 变更类型 见上面枚举
////    uint dwActorID;	// 玩家ID
////};
//
//// 点击NPC
////EVENT_PERSON_MEETNPC	
//// 场景服
//struct SEventPersonMeetNpc_S
//{
//    public Int64 uidMaster;		// 谁点的NPC
//    public Int64 uidNPC;			// 何个NPC
//    public byte nClickNPCType;	// 0 默认的直接访问 1为直接攻击(穿透)
//
//    //SEventPersonMeetNpc_S()
//    //{
//    //    memset(this,0,sizeof(*this));
//    //}
//};
//
//
//
///////////////////////////////////////////////////////////
////描        述： 任务完成监控事件消息
////支持流通类型：SS
////MAINACTION_MSGID_TASK_SYS_TASK_FINISH,
//// 完成任务事件
//struct SFinishTask_SS
//{
//    public long lActorID;	// 角色ID
//    public long lTaskID;	// 任务ID
//
//    //SFinishTask_SS()
//    //{
//    //    memset(this,0,sizeof(*this));
//    //}
//};
//
//
//// 实体被伤害
//// EVENT_CREATURE_INJURED	
//struct SEventCreatureInjured_S
//{
//    public Int64 uidMurderer;			// 凶手
//    public uint dwHPDamage;				// HP伤害
//    public uint dwMPDamage;				// MP伤害
//    public byte byDamageAttackType;		// 攻击(穿透)伤害类型
//    public byte byDamageType;			// 伤害类型
//    public bool bMultiDamage;			// 是否多体伤害
//
//	public byte			IsImmune	;		// 是否免疫
//    public byte IsCrazyDamage ;		// 是否致命
//    public byte IsBreakSkill ;		// 是否打断技能
//    public byte IsBuffDmg;		// 是否buff伤害（主要用于区分反弹伤害，防止反弹伤害死循环）
//    public byte IsAbsorb;		// 是否被吸收了
//    public byte Unk2 ;		// 备用
//
//    public Int64 lReboundParam;			// 反弹参数，由效果驱动(0xffff ffff ffff ffff,分别为 是否发生了反弹、反弹技能等级、反弹技能类型、反弹技能ID)
//    public Int64 lHitBackParam;			// 反击参数，由效果驱动(0xffff ffff ffff ffff,分别为 是否发生了反击、反击技能等级、反击技能类型、反击技能ID)
//    public Int64 lComboParam;			// 连击参数，由效果驱动(0xffff ffff ffff ffff,分别为 是否发生了连击、连击技能等级、连击技能类型、连击技能ID)
//	//LONGLONG		lChaseParam;			// 追击参数，由效果驱动(0xffff ffff ffff ffff,分别为 是否发生了追击、追击技能等级、追击技能类型、追击技能ID)
//	
//    //SEventCreatureInjured_S()
//    //{
//    //    memset(this,0,sizeof(*this));
//    //}
//};
//
//
//// 实体主动伤害 EVENT_CREATURE_HARM	
//struct SEventCreatureHarm_S
//{
//    public Int64 uidCasualty;			// 被攻击(穿透)者
//    public uint dwHPDamage;				// HP伤害
//    public uint dwMPDamage;				// MP伤害
//    public byte byDamageAttackType;		// 攻击(穿透)伤害类型
//    public byte byDamageType;			// 伤害类型
//    public bool bMultiDamage;			// 是否多体伤害
//
//    public byte IsImmune ;		// 是否免疫
//    public byte IsCrazyDamage ;		// 是否致命
//    public byte IsBreakSkill ;		// 是否打断技能
//    public byte IsBuffDmg;		// 是否buff伤害（主要用于区分反弹伤害，防止反弹伤害死循环）
//    public byte IsAbsorb ;		// 是否被吸收了
//    public byte Unk2 ;		// 备用
//
//    public Int64 lReboundParam;			// 反弹参数，由效果驱动(0xffff ffff ffff ffff,分别为 是否发生了反弹、反弹技能等级、反弹技能类型、反弹技能ID)
//    public Int64 lHitBackParam;			// 反击参数，由效果驱动(0xffff ffff ffff ffff,分别为 是否发生了反击、反击技能等级、反击技能类型、反击技能ID)
//    public Int64 lComboParam;			// 连击参数，由效果驱动(0xffff ffff ffff ffff,分别为 是否发生了连击、连击技能等级、连击技能类型、连击技能ID)
//	//LONGLONG		lChaseParam;			// 追击参数，由效果驱动(0xffff ffff ffff ffff,分别为 是否发生了追击、追击技能等级、追击技能类型、追击技能ID)
//
//    //SEventCreatureHarm_S()
//    //{
//    //    memset(this,0,sizeof(*this));
//    //}
//};
//
//// 组队模块的暂离回归请求 消息 EVENT_TEAM_CHEANGE
//struct SEventTeamChange
//{
//    //enum EventTeamState
//    //{
//    //    EventTeamState_Captain,	// 队长改变
//    //    EventTeamState_Add,		// 加入队伍
//    //    EventTeamState_Leave,	// 暂离队伍
//    //    EventTeamState_Back,	// 回归队伍
//    //    EventTeamState_Exit,	// 退出队伍
//    //};
//
//    public byte bTeamState;	// 状态
//    public uint dwTeamId;		// 队伍ID
//    public uint dwActorId;	// 角色ID
//    public uint dwBackActorID;// 归队跟随的角色ID
//
//    //SEventTeamChange()
//    //{
//    //    memset(this,0,sizeof(*this));
//    //}
//};
//
//// 组队模块是否可以组队请求 EVENT_TEAM_CREAT_OR_ADD
//struct SEventTeamCanBeSuccess
//{
//    public uint dwCreaterID;		// 创建队伍者ID
//    public uint dwInviterID;		// 邀请者ID
//    public uint dwApplicantID;		// 申请者ID
//
//    //SEventTeamCanBeSuccess()
//    //{
//    //    memset(this,0,sizeof(*this));
//    //}
//};
//
//
//
//
//
////交易给予发起&执行
////EVENT_TRADE_TRADE
//struct SEventTrade
//{
//    public Int64 uidMaster;		//本方
//    public Int64 uidTarget;		//对方
//};
//
///**
//*@brief 技能事件数据
//*/
//struct SEventSkillData_S
//{
//    public byte byEntityType;			//!< 实体类型(玩家、宠物)
//    public Int64 lEntityId;				//!< 实体Id
//    public byte bySkillType;			//!< 技能类型
//    public ushort wSkillId;				//!< 技能Id
//    public byte bySkillLevel;			//!< 技能等级
//    public uint dwParam;				//!< 参数
//
//    //SEventSkillData_S()
//    //{
//    //    memset(this, 0, sizeof(SEventSkillData_S));
//    //}
//};
//
//
//////////////////////////////////////////////////////////////冷却类型////////////////////////////////////////////////////////////
////冷却时间TimerOut
////EVENT_COLD_COLDTIMEROUT
//struct SEventColdTimerOut_S
//{
//    public Int64 uidEntity;
//    public byte byClassID;
//    public int nColdID;
//};
