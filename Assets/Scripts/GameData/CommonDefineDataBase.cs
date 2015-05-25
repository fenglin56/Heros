using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class CommonDefineData
{
    public int HP_SHORT;    //生命下限值，对应最短生命条，生命低于此数值依然也只显示最短的生命条
    public int HP_LONG;     //生命上限值，对应最长生命条，生命高于此数值依然也只显示最长的生命条
    public int SP_SHORT;    //法力下限值，对应最短法力条，法力低于此数值依然也只显示最短的法力条
    public int SP_LONG;     //法力上限值，对应最长法力条，法力高于此数值依然也只显示最长的法力条
    public int POWER_MAXCOUNT;  //气力格数上限，用户最多可以获得的气力格数。用于限制气力技能的使用频率
    public int POWER_VOLUME;    //气力槽容量，即每次能获取的最大气力值，同时也是每获取一格气力格需要的气力值
    public int POWER_REDUCETIME;//满气力槽清空需要的动画时间，即气力值从100降到0需要的时间，如果气力值不为100，则按比例。单位-秒
    public int COMBO_TIME;      //连击结束的判定时间，这个时间内如果没有新的连击出现，则判定连击中断。单位-秒
    public float DRAIN_CHANCE;    //连击第三阶段时，触发切碎系统后，杀死敌人获取生命的概率
    public float DRAIN_PERCENT;   //连击第三阶段时，触发切碎系统后，杀死敌人获取生命的数值占自身生命上限的百分比
    public float DRAIN_MOVE_MINX; //回血光球随机运动向量值X分量最小值，单位分米/秒
    public float DRAIN_MOVE_MAXX; //回血光球随机运动向量值X分量最大值，单位分米/秒
    public float DRAIN_MOVE_MINY; //回血光球随机运动向量值Y分量最小值，单位分米/秒
    public float DRAIN_MOVE_MAXY; //回血光球随机运动向量值Y分量最大值，单位分米/秒
    public float DRAIN_MOVE_MINZ; //回血光球随机运动向量值Z分量最小值，单位分米/秒
    public float DRAIN_MOVE_MAXZ; //回血光球随机运动向量值Z分量最大值，单位分米/秒
    public int DRAIN_MINTIME;   //回血光球随机运动时间的下限，单位秒
    public int DRAIN_MAXTIME;   //回血光球随机运动时间的上限，单位秒
    public float DRAIN_BALLSPEED; //回血光球被吸收时的运动速度，单位分米/秒
    public float DRAIN_LIMITDISTANCE; //回血光球被吸收，消失时与人物之间的距离，即距离低于这个数值时会消失，同时生命恢复。单位分米
    public int THRESH_CONSUME;  //翻滚技能消耗的气力格
    public int THRESH_SKILLID1; //剑客翻滚技能对应的技能ID
    public int THRESH_SKILLID4; //刺客翻滚技能对应的技能ID
    public int BUFF_CONSUME;    //爆气技能消耗的气力格
    public int BUFF_SKILLID1;   //剑客爆气技能对应的技能ID
    public int BUFF_SKILLID4;   //刺客爆气技能对应的技能ID
    public int FATAL_CONSUME;   //最终奥义消耗的气力格
    public int FATAL_SKILLID1;  //剑客奥义技能对应的技能ID
    public int FATAL_SKILLID4;  //刺客奥义技能对应的技能ID
    public int SHIELDDAMAGE_IDLE;   //BOSS未放技能状态下，每次受到攻击，即被子弹结算时防护值的减少值
    public int SHIELDDAMAGE_SKILL;  //BOSS使用技能状态下，每次受到攻击，即被子弹结算时防护值的减少值
    public int MAPID_PLAYERROOM; //练功房地图ID
    public int TUTORIAL_ECTYPE_ID; //新手引导副本_ID，新手剧情之后要跳转的副本地图ID
	public float ATTACK_CONTINUEPERIOD; //普通攻击每两段之间，因移动中断后，再次触发后继续下一段攻击的最长时间间隔，单位毫秒。如果移动中断超过这个时间，则之后再触发普通攻击会重新从第一段开始
	public int SKILLVOICE;   //技能施放语音播放触发记录，单位%
    public int TrialsEctype_FreeTime;// 每天免费进入试练副本次数
    public int TrialsEctype_PayTime;//每天付费进入试练副本次数
    public int EnergyMax;//最大活力值
    public int EnergyPayTime;//每天可购买活力次数
    public int EnergyPay;//单次活力购买价格，单位元宝
    public int EnergyAdd;//单次可购买活力值
	public int SweepNumID;//增加扫荡次数的物品ID//
	public int SweepID;//扫荡匪令的ID//
	public int VitNumID1;//增加体力的物品ID//
	public int VitNumID2;//增加体力的物品ID//
	public int VitNumID3;//增加体力的物品ID//
	public int BuyEnergyConsumption1;//购买体力，变量A
	public int BuyEnergyConsumption2;//购买体力，变量B
	public int BuyEnergyConsumption3;//购买体力，变量C
	public int BuyEnergyConsumption4;//购买体力，变量D

    public int PackageUnlockConsumption1;//购买背包，变量A
    public int PackageUnlockConsumption2;//购买背包，变量B
    public int PackageUnlockConsumption3;//购买背包，变量C
    public int PackageUnlockConsumption4;//购买背包，变量D

    public int PVPPayTime;//每天PVP次数
    public int FruitMannan_FreeTime;//每天系统赠送的甘露数量
    public int FruitMannan_Count;//每次购买甘露数量
    public int FruitMannan_CountMax;//每天购买甘露上限
    public int FruitMannan_Pay;//每次购买甘露价格，单位元宝
    public float ItemReturn;//道具界面，道具图标返回原点的时间，单位毫秒（装备失败）
    public float ItemMoving;//道具界面，道具图标移动至目标点的时间，单位毫秒（装备成功）
    public float LoadingTransparent;//Loading界面美女衣服透明速率
    public float LoadingTransparentReturn;//Loading界面停止触摸衣服恢复时间，单位毫秒
    public int RoomAwardDisplay;//练功房领取修为效果，每个修为图标代表的数值
    public float HitNumber_VectorX;//伤害数字飘出的位置与受伤单位的间隔的X方向分量，单位分米
    public float HitNumber_VectorY;//伤害数字飘出的位置与受伤单位的间隔的Y方向分量，单位分米
    public float HitNumber_VectorZ;//伤害数字飘出的位置与受伤单位的间隔的Z方向分量，单位分米
    public float[] HitNumberPos_VectorX;//伤害数字飘出的位置X方向偏移量，单位分米；最终位置=受伤单位的X方向+世界坐标X方向合值
    public float[] HitNumberPos_VectorY;//伤害数字飘出的位置Y方向偏移量，单位分米；最终位置=受伤单位的Y方向+世界坐标Y方向合值
    public float[] HitNumberPos_VectorZ;//伤害数字飘出的位置Z方向偏移量，单位分米；最终位置=受伤单位的Z方向+世界坐标Z方向合值
    public int TurnRoundSpeed;//人物转身时的角速度，单位°每秒
    public float GameMoneyDropRadius;//战斗时怪物掉落铜币自动拾取半径，单位分米
    public float Display_MaxHp;//界面显示参数_生命值上限
    public float Display_CurHp;//界面显示参数_当前生命值
    public float Display_MaxMP;//界面显示参数_真气值上限
    public float Display_CurMp;//界面显示参数_当前真气值
    public float Display_HIT;//界面显示参数_命中
    public float Display_ATK;//界面显示参数_攻击
    public float Display_DEF;//界面显示参数_防御
    public float Display_EVA;//界面显示参数_躲闪
    public float Display_Crit;//界面显示参数_暴击值
    public float Display_ResCrit;//界面显示参数_抗暴击
    public float Display_Combat;//界面显示参数_战力
    public int UpgradeAnimationStartLevel;//开始播放升级动画的等级
    public int SkillTipsStartLevel;  //开始弹出新技能提示框的等级
    public int EquipmentTipsStartLevel;  //开始弹出新装备提示框的等级
    public float Critical_CorpseTime;//因切碎死亡后尸体留存时间
	public float Normal_CorpseTime;//普通死亡后尸体留存时间
    public float LookingForTeamTime;//寻找队伍动画使用时间，单位毫秒
    public int PlayerRoom_Pay;//打坐突破消费价格，单位元宝
    public int PlayerRoom_PayTime;//每天打坐突破消费次数
    public int RefiningPrama_Base;//装备炼化参数
    public int RefiningPrama_Level;//等级炼化参数
    public int RefiningPrama_ColorLevel;//品质炼化参数
    public int RefiningPrama_Discount;//炼化折扣参数
    public int RefiningLevel;//炼化等级上限
    public int KickedOutTeamTime;//踢出队伍冷却时间，单位：秒
    public int ShortcutItem_Refining;//快捷购买-妖女内丹（炼化用）
    public int ShortcutItem_PassiveSkill;//快捷购买-洗炼天工石（洗炼用）
    public int ShortcutItem_Siren;//快捷购买-妖气丹（增加妖气值）
    public int DropItem_Num_A;//道具在地图中掉落位置计算参数，单位厘米
    public int DropItem_Num_B;//道具在地图中掉落位置计算参数，单位厘米
    public List<Vector3> CameraDistanceList = new List<Vector3>();  //摄像机相对玩家的距离，除了城镇
    public List<Vector3> CameraDistanceTownList = new List<Vector3>();  //摄像机相对玩家的距离，在城镇
    public List<float> CameraBarrierDistanceList = new List<float>();   //摄像机运动的边缘保护
    public float Char01WalkSpeed;
    public float Char04WalkSpeed;
    public float RoleTurnSpeed;//角色和时装界面，角色转动的单位标准角度，单位°
    public int VitRecoverTime;//活力值恢复时间间隔（分钟）
    public List<RoleIconData> HeroIcon_TownAndTeam;//城镇/战斗界面队友头像，职业+时装ID+对应文件。
    public List<RoleIconData> HeroIcon_Battle;//战斗界面玩家头像，职业+时装ID+对应文件。
	public List<RoleIconData> HeroIcon_MailFriend;//邮件好友头像，职业+时装ID+对应文件。
    public List<RoleIconData> HeroIcon_BattleReward;//结算界面玩家头像，职业+时装ID+对应文件。
	public List<RoleIconData> HeroIcon_SettlementReward;//翻宝箱界面玩家头像
    public List<RoleIconData> HeroIcon_Ranking;//排行榜界面玩家头像
    public List<RoleIconDataWithPrefab> HeroIcon_Town;//城镇头像
    public List<StoryPlayerIconData> HeroIcon_NPCTalk;//剧情对话界面玩家头像，职业+时装ID+对应文件。
    public List<RoleIconDataWithPrefab> HeroIcon_Dialogue;//游戏内剧情对话头像
    public float[] DetectDistance;  //当角色前方多少距离范围内有敌人时才检测敌人的碰撞范围，单位分米+角度
    public Vector2 SirenTouchOffset;  //炼妖界面妖女触摸按钮的偏移量，单位分米

	public	int	DefenceLevelStartTipDelay	;	//	副本类型为8(防守副本)时，进入副本后延迟一定时间在界面中心显示进入副本提示，单位毫秒
	public	int	DefenceLevelTipsCoolDown	;	//	防守副本同一个事件提示的冷却时间
	public	string	DefenceLevelSpecialSkill	;	//	副本类型为8(防守副本)时，会显示特殊提示技能的ID，与下面美术资源对应
	public	string	DefenceLevelSpecialSkillTips	;	//	副本类型为8(防守副本)时，会显示特殊提示技能的美术资源
	public	int	DefenceLevelJingYanDailyLimit	;	//	经验副本每日挑战次数限制
	public	int	DefenceLevelTongBiDailyLimit	;	//	铜币副本每日挑战次数限制
	public	int	DefenceLevelYuanBaoDailyLimit	;	//	元宝副本每日挑战次数限制
	public	int	DefenceLevelJudgeGateHPLeft	;	//	防守副本评价中，大门剩余血量评价的左端值，即玩家开始计分的最低值
	public	int	DefenceLevelJudgeHitPointLeft	;	//	防守副本评价中，连击评价的左端值，即开始计分的最低值
	public	int	DefenceLevelJudgeBeHitParam1	;	//	防守副本评价中，受击评价的左端值，即开始计分的最低值
	public	int	DefenceLevelJudgeBeHitParam2	;	//	防守副本评价中，受击评价的右端值，即计分的最高值
	public	int	DefenceLevelJudgeThreshold1	;	//	副本类型为8(防守副本)时，通关评价阀值1，百分比
	public	int	DefenceLevelJudgeThreshold2	;	//	副本类型为8(防守副本)时，通关评价阀值2，百分比


    public float DropItem_AnimationDelay;//播放掉落动画时，动画水平运动的延迟时间，单位毫秒
    public float DropItem_IntervalTime;//播放拾取物品动画时，多个动画之间的间隔时间，单位毫秒
    public float DropItem_Dis;//播放掉落动画时，水平运动的距离参数

	public float GoodsDropAutoTime;//使用全部礼箱打开间隔 秒 = 毫秒 / 1000f;
	public float ItemMsgTimeDisappear;//道具获得消息多长时间后自动消失 秒 = 毫秒 / 1000f;
	public int ItemMsgLimit;//屏幕上允许同时出现的道具获得消息的数量上限
	public float ItemMsgSpeedVertical;//道具获得消息横向移动的速度 分米/秒
	public float ItemMsgSpeedHorizontal;//道具获得消息纵向移动的速度 分米/秒
	public float ItemMsgTimeHorizontal;//道具获得消息每次向下位移所用时间 秒 = 毫秒 / 1000f;
	public float ItemMsgTimeGap;//道具获得消息出现间隔 秒 = 毫秒 / 1000f;
	public int EctypeLoadingWaitingTime;//组队副本等待队友Loading的时间，单位秒。从点击开始挑战按钮开始计算，如果在该时间内队友没有Loading完，则先开始副本，不再等待;
	public string HeroIcon_Fashion;//时装界面玩家头像，职业+时装ID+对应文件;
	public float DefenceLevelTipsAppearTime;//防守副本事件提示显示时间，单位毫秒;
	public string Avatar_CenterSpeed;//时装界面_人物缩放_中心点运动速度；填写格式：X速度+Y速度;
	public string Avatar_EdgeSpeed;//时装界面_人物缩放_双边扩张速度；填写格式：X速度+Y速度;
	public float Avatar_Time;//时装界面_人物缩放_运动时间，单位毫秒;
    public int MailLimit;//邮箱邮件最大数量
	public Vector2 BornDialoguePosition1;//战斗副本中NPC对话框偏移量
	public Vector2 BornDialoguePosition2;
	public Vector2 BornDialoguePosition3;
	public Vector2 BornDialoguePosition4;
	public Vector2 BornDialoguePosition5;
	public Vector2 BornDialoguePosition6;
	public Vector2 BornDialoguePosition7;
	public Vector2 BornDialoguePosition8;
	public Vector2 BornDialoguePosition9;
	public Vector2 BornDialoguePosition10;
	public List<Vector2> BornDialoguePositionList = new List<Vector2>();
	//无尽试炼
	//无尽模式每日挑战次数
	public int EndlessDailyLimit;
	//下波刷怪时限
	public int EndlessNextWaveTime;
	//跳波时间
	public int EndlessWaveSkipTimeParam;
	//单条奖励信息显示时间
	public int EndlessSingleMassageShowTime;
	//信息显示追加时间
	public int EndlessMassageShowTimePlus;

    public int DialogSpeed;//对白出现速度，单位字/秒；
	public int SignInConsumption;//签到系统补签所需元宝数量
	//讨伐次数限制
	public int Coop_DailyLimit;

	public int SendMailConsumption;//发送邮件手续费

    #region TownstartPoint
    public TownBtnStartPoint TownstartPoint1;
    public TownBtnStartPoint TownstartPoint2;
    public TownBtnStartPoint TownstartPoint3;
    #endregion
    #region PlayerLuckDraw
    public int LotteryMultipleNum; //聚宝盆多倍掉落时的倍率
    public int LotteryOneCost; //聚宝一次时消耗元宝量。
    public int LotteryTenCost; //聚宝十次时消耗元宝量。
    public int LotteryOneDiscount;  //聚宝一次打折时按钮下方的折扣信息图标。没打折则填0。
    public int LotteryTenDiscount;  //聚宝十次次打折时按钮下方的折扣信息图标。没打折则填0。
    public float LotteryVelocity;    //抽奖项动画匀速运动时的速度，单位为格/帧。
    public float LotteryAcceleration;  //抽奖项动画加速度，单位为格/（帧^2）。
    public int LotteryThrowTime;     //掉落元宝动画持续多少毫秒
    #endregion

	#region Auction
	public int AuctionDefaultTime;
	public int AuctionBidRate;//玩家每次出价时最小加价的比例。
	public int AuctionTopBid;//最高出价，玩家无法出更高价格来竞价，一旦出该价则别的玩家无法再出价。
	#endregion
	#region CarryShop
	public int ShopSlotMaxNum;
	public int ShopSlotUnlockCost5;
	public int ShopSlotUnlockCost6;
	public int ShopSlotUnlockCost7;
	public int ShopSlotUnlockCost8;
	public int ShopSlotUnlockCost9;
	public int ShopSlotUnlockCost10;
	public int ShopSlotUnlockCost11;
	public int ShopSlotUnlockCost12;
	public int ShopChangeCost1;
	public int ShopChangeCost2;
	public int ShopChangeCost3;
	public int ShopChangeCost4;
	#endregion
	#region QuickBuy
	public int QuickBuyCopperCoin;
	public int QuickBuyWorldChatItem;
	#endregion
	#region 进入城镇按等级弹出提示
	public List<int> levelComeInTown = new List<int>();
	public List<int> viewComeInTown = new List<int>();
	//public Dictionary<int,int> levelComeTown = new Dictionary<int, int> (); 
	#endregion
	public int WorldChatItem;//世界频道聊天消耗的道具ID
	public List<RoleIconData> HeroIcon_Team;//组队 聊天 讨伐 头像

	public int Coop_CostItemShop1;//首领讨伐中，对应道具3100001的快速购买商店ID
	public int Coop_CostItemShop2;//首领讨伐中，对应道具3100002的快速购买商店ID
	public int Coop_CostItemShop3;//首领讨伐中，对应道具3100003的快速购买商店ID
	
    public int DodgePara;  //闪避系数
	public float TownNamePos;//城镇角色名高度
	public float BattleNamePos;//副本战斗角色名高度
    public int DefaultStroy;  //默认NPC 任务ID
	public int FriendRequestLimit;//好友请求保留上限，最多不超过50个
	public int Coop_FreeTimes;//首领讨伐，每日免费进入的次数
	public float gameStarTime;//开场动画时间
	public int createClass1StoryLineID;//新建角色剑客剧情ID
	public int createClass4StoryLineID;//新建角色刺客剧情ID

    public float SearchAngle;   //主角优先索敌的左右角度之和
    public int SearchRadius;   //锁定目标的半径 配置数据以分米为单位
	public int SearchMinRange;   //代表主角优先索敌范围的半径，单位为分米
	public float SearchAreaAngle;  //表示主角次级索敌的区域划分角度，单位为度
    public int SearchFrq;     //锁定目标的间隔时间，间隔锁定目标
    public int ButtonMemTime; //按键记忆时间
	public int PingDelayTime;//ping刷新时间
	public int MaxLoadingTime;//加载倒计时
    public int ChargeMinRange;//突进最近距离

	public int ChatBoxPosX;//城镇聊天框位置X
	public int ChatBoxPosY;//城镇聊天框位置Y
	public int TownRobotNum;//城镇机器人刷出个数
    public int ChargeOffset; //突进的最近距离  分米


    public float AddBuff_BornTime;

	public float SkillComboTime; //多段技能，在这段时间内，没有使用下一段技能B，则计时结束后，按键对应的技能重新替换为第一段技能，并显示第一段技能的当前CD时间

	public float BossDef_WeakTime;//Boss破防_Boss破防不会立刻恢复防护值的时间，单位秒
	public float AutoPickup_Time;//掉落自动拾取时间，单位秒
	public float AutoPickup_Speed;//掉落自动拾取特效飞行速度，单位分米/秒


    public int BuyFruitMannanConsumption1;
    public int BuyFruitMannanConsumption2;
    public int BuyFruitMannanConsumption3;
    public int BuyFruitMannanConsumption4;

	public int FastSelectQuality;		// 背包出售界面，可快速选择的物品品质

	public float Match_Delay;//匹配玩家确认时间（秒）

	public int DefaultSirenSkill1;//剑客默认奥义技
	public int DefaultSirenSkill4;//刺客默认奥义技

	public int ButtonWeakTipsLevel;
	public int Lose_BeShow;
	public int ShopChangeNum;

    public float AttackButtonDelay;  //普通攻击按下时，触发间隔
    public int HurtEffectNumber; //同屏显示受击特效最大值
    public int GameMoneyAbridge; //铜币显示缩减临界值

    public int NoneFreezeIronLevel; //当NoneFreezeIronLevel的值与结算对象当前技能动作的threshold相等时，结算对象不被冰冻（定身）

	public int StrengthLimit;//强化最大等级
	public int StartStrengthLimit;//升星最大等级

	////PVP
	public string PVPBattleWinIcon;	//团队PVP结算额外奖励图标（首胜、二胜……），填写Prefab文件名（文件在Assets/Prefab/GUI/IconPrefab/PVGAwardWin）
	public List<GameObject> PVPBattleWinIconPrefab;
	public string HeroIcon_PVPLoading;
	public List<RoleIconData> pvpLoadingIcon;
	public float DropItem_RadiusParam;
}



public class CommonDefineDataBase:ScriptableObject
{
    public CommonDefineData _dataTable;
}

[Serializable]
public class RoleIconData
{
    public int VocationID;
    public int FashionID;
    public string ResName;
}
[Serializable]
public class RoleIconDataWithPrefab
{
    public int VocationID;
    public int FashionID;
    public GameObject IconPrefab;
}
[Serializable]
public class TownBtnStartPoint
{
    public Vector2 BasePostion;
    public Vector2 BaseOffset;
    public Vector2 Direction;
}
[Serializable]
public class StoryPlayerIconData
{
    public int VocationID;
    public int FashionID;
    public GameObject TalkHead;
}

