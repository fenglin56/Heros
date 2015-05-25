
public enum ResourceType
{
    Hero,       //主角
    Monster,    //怪物
    Damaged,    //可破坏物
    NoDamaged,  //不可破坏物
    Trap,       //陷井
    Terrain,    //地形
    Portal,     //传送门
    NPC,        //NPC
	None,
}
public enum AniAction
{
    Stop,
    Start,
}
public enum StoryTallType
{
    Player=1,
    NPC=2,
}
public enum DialogBoxType
{
    /// <summary>
    /// 头像左
    /// </summary>
    LeftWithIcon=1,
    /// <summary>
    /// 头像右
    /// </summary>
    RightWithIcon,
    /// <summary>
    /// 无头像左
    /// </summary>
    Left,   
    /// <summary>
    /// 无头像右
    /// </summary>
    Right,
    /// <summary>
    /// 对白
    /// </summary>
    Dialogue,
}
//副本难度选择
public enum EctypeDifficulty
{
    ED_TYPE_INVALID,
    ED_TYPE_EASY,						// 简单难度
    ED_TYPE_NORMAL,						// 普通难度
    ED_TYPE_HARD,						// 困难难度
    ED_TYPE_HELL,						// 地狱难度
    ED_TYPE_MAX,						// 最大
};

enum eFCMD_TYPE
{
    FCT_ATTACK,				//!< 攻击
    FCT_WAIT,				//!< 待机
    FCT_SKILL,				//!< 招式
    FCT_DEFEND,				//!< 防御
    FCT_ITEM,				//!< 物品
    FCT_CONJURE,			//!< 召唤
    FCT_AUTO,				//!< 自动
    FCT_TALK,				//!< 说话，怪物AI命
    FCT_SPEATT,             //!< 特殊版攻击带强P功能
    FCT_SYS,                //!< 系统发送的
};

/// <summary>
/// Creat By WYN
/// </summary>
enum EctypeType
{
    ET_TYPE_INVALID = 0,
    ET_TYPE_PERSONAL,       //个人副本
    ET_TYPE_TEAM,           //组队副本
    ET_TYPE_MOREACTOR,      //多人副本
    ET_TYPE_TRIBE,          //氏族副本
    ET_TYPE_MAX,            //最大
}

public enum CampType
{
	CAMP_PLAYER = 1,    //友军阵营 ，包玩家和友军怪物
	CAMP_MONSTER = 2,   //敌军阵营
    CAMP_NEUTRAL = 3,   //中立阵营
}

//是否为我方怪物，1是友方，2 是敌方，3是中立。
///////////////////////// 战斗中的敌对关系(1: 2： 1、2对立，3：中立) ///////////////
public enum Hostility
{
	FIGHT_HOSTILITY_ONE = 1,			//敌对关系1 一般表示玩家
	FIGHT_HOSTILITY_TWO = 2,			//敌对关系2	一般表示怪物
	FIGHT_HOSTILITY_NEUTRAL = 3,		//中立
};

public enum ObjectType
{
    Hero,
    Member,
    Monster,
}

public enum FightEffectType
{
    NONE=-1,
    BATTLE_EFFECT_HP = 0,				//HP扣血
    BATTLE_EFFECT_CRIT = 1,				//暴击
    BATTLE_EFFECT_DODGE = 2,			//闪避 Miss
    BATTLE_EFFECT_HIT = 3,				//命中

    BATTLE_ADDHP=4,                     //补血
    BATTLE_ADDMP = 5,                   //补真气
    BATTLE_ADDMONEY = 6,                //加铜钱

    BATTLE_EFFECT_ROLLPOINT = 7,        //Roll点数据下发
    BATTLE_EFFECT_GOODSSHOW = 8,        //物品玩家拾取显示
    BATTLE_EFFECT_EXPSHOW = 9,				//怪物死亡经验显示
    BATTLE_EFFECT_XIUWEI = 10,          //增加修为显示
    BATTLE_EFFECT_SHILIAN_EXPSHOW = 11,				//试炼副本经验
    BATTLE_EFFECT_SHILIAN_XIUWEI = 12,      //试炼副本修为
    TOWN_EFFECT_ZHANLI = 13,        //增加战力
}
public enum EntityModelPartial
{
    DataStruct,
    GameObject,
}
public enum Equipment_Strength_Type:byte
{
    EQUIPMENT_NONE_STRENGTH_TYPE,
    EQUIPMENT_NORMAL_STRENGTH_TYPE,							//普通强化
    EQUIPMENT_START_STRENGTH_TYPE,							//星级强化
    EQUIPMENT_MAX_STRENGTH_TYPE,
};
// 地图传送类型
public enum eTeleportType:byte
{
    TELEPORTTYPE_NULL = 0,		// 空状态
    TELEPORTTYPE_FIRST,			// 第一次登录
    TELEPORTTYPE_RECONNECTION,	// 断线重连
    TELEPORTTYPE_NORMAL,		// 正常地图之间切换
    TELEPORTTYPE_CURMAP,		// 当前场景复活
    TELEPORTTYPE_JUMPSERVER,    //换服
};
public enum LoginType
{
    First,
    BeKicked,    
}
public enum GuideState
{
    /// <summary>
    /// 无引导
    /// </summary>
    None,
    /// <summary>
    /// 弱引导
    /// </summary>
    Weak,
    /// <summary>
    /// 强制引导
    /// </summary>
    Constrain,
}
public enum TextColorType
{
    ItemQuality0,
    ItemQuality1,
    ItemQuality2,
    ItemQuality3,
    Gray,
    Pink,
    Yelow,
    Red,
	EquipProperty,   //背包及装备界面上通用值的颜色

}
/// <summary>
/// 背包右边按钮类型
/// </summary>
public enum PackBtnType
{
	PutOn,	//穿上
	Strength,  //强化
	StarUpgrade,//升星
	Diamond,	//器魂
	Sell,		//出售
	Upgrade,//升级
	Package,  //背包
	Swallow,//器魂吞噬
    QuickStrengthen,//快速强化
    QuickUpgradeStar,//快速升星
    PutOff,         //卸下
}
public enum EmailBottonBtnType
{

}
public enum GoodsSubClass
{
	Weapon=1,  //武器
	Fashion,//服饰
	Headwear,//头饰
	Dress,//衣服
	Boots,//靴子
	Accessories,//饰品
	Badge,//徽章
}
/// <summary>
/// 玩家职业
/// </summary>
public enum Vocation
{
	/// <summary>
	/// 无职业
	/// </summary>
	None=0,
	/// <summary>
	/// 剑客
	/// </summary>
	Swordsman=1,   
	/// <summary>
	/// 刺客
	/// </summary>
	Assassin=4,
}
public enum DefenceSliderType
{
	/// <summary>
	/// 大门血量
	/// </summary>
	DoorBlood,
	/// <summary>
	/// 连击数
	/// </summary>
	DoubleHit,
	/// <summary>
	/// 受击数
	/// </summary>
	BeHit,
}
public enum CommonTitleType
{
	Money=1,  //铜币
	GoldIngot, //元宝
	Power,  //体力
	Practice, //修为
	Diamond, //钻石
}

public enum ItemQualityColor
{
	White = 0,
	Green,
	Blue,
	Purple,
}
