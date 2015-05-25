using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class ItemData
{
	public int _goodID;
	public string _szGoodsName;

    public int _GoodsClass;
    public int _GoodsSubClass;
    public int _ColorLevel;
    public string _AllowProfession;
    public int _Level;
    public int _AllowLevel;
    public int _PileQty;
    public int _BuyCost;
    public int _SaleCost;
    public int _AllowSex;
    public int _ThrowFlag;
    public int _TradeFlag;
    public int _GiveFlag;
    public int _BindFlag;
    [GameDataPostFlag(true)]
    public GameObject _DisplayIdSmall;//不用
    public string smallDisplay;
    public string _ModelId;
    [GameDataPostFlag(true)]
    public GameObject DisplayBig_prefab;
    public string _DisplayIdBig;
    public string _szDesc;
    [GameDataPostFlag(true)]
	 public GameObject _picPrefab; //thumb image of item
    public GameObject lDisplayIdRound;
    public string[] LinkIds;
	public bool CanBeFastSelect;
	public enum ItemType
	{
		Equipment,
		Medicament,
		Materiel,
		NONE
	}
	
	public static ItemType GetItemType(ItemData item)
	{
		if(item is EquipmentData)
		{
			return ItemType.Equipment;	
		}
		else if(item is MedicamentData)
		{
			return ItemType.Medicament;	
		}
		else if(item is MaterielData)
		{
			return ItemType.Materiel;
		}
		return ItemType.NONE;
	}
}

[Serializable]
public class EquipmentData:ItemData
{
    public int _DisplayID;
    public int _EquipmentKind;
    public int _SuitEquipID;
    public string _vectEquipLoc;
    public string _vectEffects;
	/// <summary>
	/// 是否可以强化标志
	/// </summary>
    public int _StrengFlag;
    public int _SmeltFlag;
    public int _RecastFlag;
    public int _VectAddNum;
    public string _vectEffectsAdd;
    public int _vectSkillID;
    public int _HoleMax;
    public int _lThresholdValue;     //强化阀值
	public bool lUpgradeFlag;//是否可升级
	public int UpgradeID;//升级后装备ID
	public string UpgradeCost;//升级消耗

    //2013-6-11 Add by Rocky
    public StrengthParameter[] _StrengthParameter; //普通强化加成参数（4个）
    public float[] _StrengthCost;      //普通强化消耗参数（4个）
	public StrengthParameter[] _StartStrengthParameter; //星级强化加成参数（4个）
    public int _StartStrengthPercent;  //星级强化加成百分比参数
    public StartStrengthLvCost[] _StartStrengthCost;   //星级强化消耗，数组长度等于星级等级
	public float[] NormalStrenPercent;  //普通强化成功率
	public float[] StarUpPercent;  //升星成功率
	public  SaleItemPrice[]  SaleItem ;//装备道具售价
    public GameObject WeaponEff;
}
//星级强化消耗按等级参数
[Serializable]
public class StartStrengthLvCost
{
    public int Lv; //等级
    public int ItemID_1;   //道具1ID
    public int Value_1;    //首具1消耗值
//    public int ItemID_2;   //道具2ID
//    public int Value_2;    //首具2消耗值
//    public int ItemID_3;   //道具3ID
//    public int Value_3;    //首具3消耗值
}
[Serializable]
public class StrengthParameter
{
    public int Index;  //主属性
	public int[] Value;
}

[Serializable]
public class MedicamentData: ItemData
{
    public int _PassiveRace;
    public int _BatchFlag;
    public int _OnID;
    public string _vectEffects;
}
[Serializable]
public class SaleItemPrice
{
	public int ItemID;   //道具ID
	public int Price;    //首具售价
}
[Serializable]
public class MaterielData: ItemData
{
    public string _szParam1;
    public int _szParam2;
}
[Serializable]
public class Jewel:ItemData
{
	/// <summary>
	///被动技能 
	/// </summary>

	public int PassiveSkill;
	/// <summary>
	///最大等级 
	/// </summary>
	public int MaxLevel;
	/// <summary>
	///器魂各等级经验 
	/// </summary>
	public int[] StoneExp;
	/// <summary>
	/// 器魂的初始当前经验值
	/// </summary>
	public int StoneStartExp;
	/// <summary>
	/// 器魂吞噬损耗系数,千分比
	/// </summary>
	public int StoneExpRate;
	/// <summary>
	/// 器魂镶嵌位置,1=武器、3=头饰、4=衣服、5=靴子、6=饰品
	/// </summary>
	public string[] StonePosition;
	/// <summary>
	/// 器魂类型,0=红魂，1=黄魂，2=蓝魂
	/// </summary>
	public int StoneType;
	/// <summary>
	/// 器魂套装ID
	/// </summary>
	public int StoneGrop;
	/// <summary>
	/// T套装激活属性,包含技能id和技能等级
	/// </summary>
	public ActivePassiveSkill _activePassiveSkill;
	/// <summary>
	/// T套装前缀,IDS
	/// </summary>
	public string StoneGropEquipName;

}
/// <summary>
/// 器魂套装被动技能，包含技能id和等级
/// </summary>
[Serializable]
public class ActivePassiveSkill
{
	public int skillID;
	public int skillLevel;

}
