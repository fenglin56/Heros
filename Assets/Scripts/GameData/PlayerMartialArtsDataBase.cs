using System;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Collections.Generic;

[Serializable]
public class PlayerMartialArtsData
{
	public byte MartialArtsType;	// 武学类型
	public int MartialArtsID;		//武学ID
	public int MartialArtsUnlock;	//学习后解锁的武学类型等级
	public int MartialArtsLevels;	//武学等级
	public int MartialArtsMaxLevels;//武学等级上限
	public string MartialArtsName;	//武学名称
	//public string MartialArtsIcon;	//图标资源路径 prefab名字
	//[DataToObject(PrefabPath = "Assets/Prefab/GUI/IconPrefab/PVPSkillIcon")]
	public GameObject MartialArtsIconPrefab;	//图标prefab
	public string MartialArtsDes;	//武学介绍文字
	public int MartialArtsMaxScore;	//学习需求最高荣誉
	public int MartialArtsContribution;	//学习/升级需求贡献
	public string MartialArtsParamDes;	//武学效果属性加成说明文字
	public byte EffType;	//武学效果类型
	public byte EctypeType;	//武学生效副本类型
	public string MartialArtsStrengthParam;	//强化属性
	public int EctypeBuffID;		//进入副本附加Buff
	public int ReviveTime;			//PVP复活减少的时间
	public string FollowerPropParam;	//追随者强化属性
	public string MonsSkillReplace;	//怪物技能子弹替换
	public string SkillID1;			//攻击触发技能1
	public int AttExtraAccRate1;	//攻击效果触发概率1
	public int Accid1;				//攻击效果触发结算ID1
	public string SkillID2;			//攻击触发技能2
	public int AttExtraAccRate2;	//攻击效果触发概率2
	public int Accid2;				//攻击效果触发结算ID2
	public int DefAccRate1;	//防御效果触发概率1
	public int DefAccid1;		//防御效果触发结算1ID
	public int DefAccRate2;	//防御效果触发概率2
	public int DefAccid2;		//防御效果触发结算2ID
	public int JumpTigRate;	//气力结算触发概率
	public int QiID;			//气力结算触发ID
	public int AvoidTrgRate;	//闪避触发概率
	public int AvoidID;			//闪避触发结算ID

	public MartialIndex martialIndex;	//用于做该表的唯一键值
}

//包含武学ID和武学等级
[Serializable]
public struct MartialIndex
{
	public int MartialArtsID;
	public int MartialArtsLevel;
}

public class PlayerMartialArtsDataBase : ScriptableObject
{
	public PlayerMartialArtsData[] _dataTable;
}
