using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EctypeContainerDataList : ScriptableObject {

    public EctypeContainerData[] ectypeContainerDataList;   
}


[Serializable]
public class EctypeContainerData
{ 
    public int lEctypeContainerID;//副本ID
    public string lEctypeName;//副本名称
    public int lDifficulty;//副本难度
    public string[] lEctypePos;//位置
    public string lEctypeIcon;//副本图标
    public string vectMapID;//包含的场景ID
    public int lAllLoadFlag;//是否全部创建
	public int lEctypeType;//副本类型,0=常规副本，1=封魔副本 2=pvp副本 3=封妖副本 4=练功房 5=试炼副本 6=新手副本 7= 8=防守副本 9=首领讨伐 10=无尽试炼
	public int lEctypeMode;//副本模式,单人多人
    public int lMinActorCount;//组队最少人数
    public int lMaxActorCount;//组队最大人数
    public int lMinActorLevel;//角色最小等级
    public int lMaxActorLevel;//角色最大等级
    public int lCostType;//挑战消耗类型,1活力，2元宝，3铜币
    public string lCostEnergy;//挑战消耗
    public int lOutTime;//存在倒计时
    public int lDayEnterTimes;//日进入次数
    public int lWeekEnterTimes;//周进入次数
    public string vectChunnelID;//进入传送点ID
    public int DestDir;//朝向
    public int lRadius;//半径
    public int lExperience;//经验值
    public int lMoney;//钱币
    public string lPropAwardDesc;//道具奖励描述
    public int vectAwardID;//奖励ID
    public int vectAwardRate;//奖励概率
    public int vectGoldTreasureID;//元宝宝箱ID
    public int vectGoldTresureRate;//元宝宝箱ID对应概率
    public int wDelockLev;//解锁等级
    public int dwDelockTargetID;//解锁副本容器ID
    public string DropInf;//关卡掉落信息
    public int dwBasicWinTime;//通关时间基准值（秒）
    public int dwBasicHitPoint;//通关连击基准值
    public int BasicBeHit;//通关受击基准值
    public string BossHead;//boss头像
    public int BossLifeLayer;//boss血条数
    public int[] BossIDs;//副本boss的id
    public int MapType;// 副本类型，0=常规副本，1=封魔副本 2=pvp副本 3=封妖副本 4=练功房 5=试炼副本 6=新手副本
    public int ByCostType;//翻牌花费类型
    public int ByCost;//翻牌花费
    public int ComboValue; //连击数
    public string TrialsAward;//试炼通关奖励
    public int PlayerNum;//建议进入人数

    public int BattleVictoryLotteryTime;//副本结算翻牌倒计时
    public int BattleFailTime;//副本失败倒计时
	public List<int> DropListItem;//掉落物列表
	public string EctypeBossDescription;//boss 简介
	public string EctypeDescription;//副本描述

	public int FightingCapacity;//推荐战斗力

    //public Texture EctypeIconTexture;//副本图片资源
    //public GameObject EctypeIconPrefab;//副本图片prefab

    //public GameObject bossAppearancePortrait;//BOSS出场动画的头像
    public string bossAppearanceWord;//BOSS出场动画的对白内容
    public string bossAppearanceSound;//BOSS出场动画的语音
	public string defenceLevel ;
	public Dictionary<int,string> DefenceLevel_BlockMap;
	//public int[] DefenceLevel_Block; //防守副本充当路障的MonsterId

    public int[] PowerSkillHide;
    public RoleUpanishads[] RoleUpanishads;

	public int SirenSkillVaule;//妖女奥义使用次数

    public StartSkill[] StartSkills;

	//1.3新增
	public bool CanUseMedicament;//是否可使用药品 0=false, 1=true
	public FreeMedicament[] FreeMedicaments;//副本免费使用药品次数

	public MedicamentID[] MedicamentIDs;//副本使用药品的ID vip等级+药品ID
	public MedicamentPrice MedicamentPrice;//药品价格 //(向下取整((参数1×〖付费使用次数〗^2+参数2×付费使用次数+参数3)/参数4)×参数4)
	public MedicamentBuffID[] MedicamentBuffIDs;//副本中使用药品固定buffID
	public int ReviveType;//复活类型 0=不能复活，1=手动复活，2=自动复活
	public ReviveNum[] ReviveNums;//副本复活次数 vip等级+次数 复活次数为-1时无次数限制
	//public int ReviveNum;//副本复活次数 vip等级+次数 复活次数为-1时无次数限制
	public int ReviveTime;//复活时间 秒

	public SimpleRevivePrice SimpleRevivePrice;//道具ID+参数1+参数2+参数3+参数4，3050001=铜币，3050002=元宝，其他为消耗道具
	public PefectRevivePrice PefectRevivePrice;//道具ID+参数1+参数2+参数3+参数4，3050001=铜币，3050002=元宝，其他为消耗道具

	public string[] DefenceLevelLoot;  //日常防守副本掉落
	public int GateHPRemain;  //通关门剩血基准值

	//首领讨伐
	public bool Coop_IsItemQuikBuy;//玩家在首领讨伐界面进入组队界面讨伐令不足时，如果该值为1则弹出快速购买栏，为0则不弹
	public int Coop_DailyLimit;//首领讨伐限次 0=无次数限制 1=有次数限制
	public int Coop_Solo;//如果=1则选择BOSS界面右下方2个按钮合并为1个【单人讨伐】按钮；=0则可组队讨伐
	public int Coop_ItemCost_GoodsID;//消耗物品
	public int Coop_ItemCost_GoodsNum;//消耗数量
	public int[] Coop_BonusTime;//评级
	//允许创建队伍，填0，则在副本选择界面隐藏组队按钮；填1，则在副本选择界面显示组队按钮
	public int AllowCreatTeam;	//此副本可否组队
	public float ResultAppearDelay;	//BOSS被击杀时到结算界面弹出的时间间隔
	public int IsMOBA;
	public void Init()
	{
		if (DefenceLevel_BlockMap != null)
			return;
		DefenceLevel_BlockMap = new Dictionary<int, string>();
		if(defenceLevel != "0")
		{
			var defenceLevel_Block=defenceLevel.Split('|');
			foreach(string str in defenceLevel_Block)
			{
				string[] array = str.Split('+');
				DefenceLevel_BlockMap.Add(int.Parse(array[0]),array[1]);
			}
		}
	}
	public float PickupDelay;//自动拾取延时
}
[Serializable]
public class SimpleRevivePrice
{
	public int GoodsID;
	public int Parma1;
	public int Parma2;
	public int Parma3;
	public int Parma4;
}
[Serializable]
public class PefectRevivePrice
{
	public int GoodsID;
	public int Parma1;
	public int Parma2;
	public int Parma3;
	public int Parma4;
}

[Serializable]
public class VIPProperty
{
	public int VipLevel;
	public int FreeMedicament;

}

[Serializable]
public class RoleUpanishads
{
    public int Vocation;        //职业ID
    public int UpanishadId;     //奥义ID
}

[Serializable]
public class FreeMedicament
{
	public int VipLevel;
	public int Num;
}

[Serializable]
public class MedicamentID
{
	public int VipLevel;
	public int GoodsID;
}

[Serializable]
public class MedicamentPrice
{
	public int GoodsID;
	public int Param1;
	public int Param2;
	public int Param3;
	public int Param4;
}
[Serializable]
public class MedicamentBuffID
{
	public int VipLevel;
	public int BuffID;
	public int BuffLevel;
	public int ColdID;
}

[Serializable]
public class ReviveNum
{
	public int VipLevel;
	public int Num;
}

[Serializable]
public class StartSkill
{
    public int Vocation;        //职业ID
    public int SkillID;         //技能ID
}

