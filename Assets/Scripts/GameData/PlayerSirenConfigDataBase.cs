using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class PlayerSirenConfigData 
{
    public int _sirenID;
    public string _name;
    public string _nameRes;
    public string _portraitID;//头像id

    //public GameObject _portraitPrefab;

    //public string _rejectAnim;
    //public string _shyAnim;
    //public string _temptationAnim;    
    

    public int _growthMaxLevel;//最高炼化等级
    public int _composeCost_itemID;//解锁需要物品id
    public int _composeCost_itemNum;//解锁需要物品数量
    public string _unlockTips;//解锁道具提示
    //public int _sitEffect;//修为加成
    public float _defaultWordCd;//默认对白冷却时间
    public int _refiningColdTime;//冷却id

    public List<SirenConfigData> _sirenConfigDataList;    

	public UnlockCondition[] Unlock;//收服条件"Unlock"

	public string _SirenText;//未解锁妖女介绍文字
	public string _UnlockText;//收服妖女条件界面文字
	public int _SirenPrice;//强行收服妖女消耗
	public string _BattleVoice;//参战语音
	
}

[System.Serializable]
public class SirenConfigData
{
    public string _dwDisplayID;//资源id
    public GameObject _prefab;

	public GameObject _TitlePrefab;//称号prefab
	public GameObject _NamePrefab;//名字

    public string _dzDisplayID;
    public GameObject _dzPrefab;

    public int _growthCost;//所需经验
    public string _growthEffect;//加成
    public int _growthLevels;//等级

    public int _sitEffect;//打坐加成
    public string _sitEffectTips;//打坐加成提示ids

    public int _sirenPower;//妖女威力(奥义)

    public string _defaultAnim;//默认动作
    public string _fearAnim;//炼妖动作
    public string _touchAnim;//触摸动作

    public string _touchSound;//触摸对白

    public SirenDialogConfigData _touchWord;//触摸对白
    public SirenDialogConfigData _defaultWord;
    public SirenDialogConfigData _successWord;
    public SirenDialogConfigData _failWord;

	public string _BattleVoice;//参战语音

    public Vector3 _cameraPosition; //镜头位置偏移
    public int _refiningShakeTime;//振动次数
    public float _refiningShakeAttenuation;//振动衰减
    public float _refiningShakeInitSpeed;//振动初速度
    public float _refiningShakeElasticity;//皮筋弹力系数

    public Vector3 _sirenPosition; //妖女位置
    public int _refiningItem_itemID;//炼化需要物品id
    public int _refiningItem_itemNum;//炼化需要物品数量
	public int _growthLvlLimit;//炼化所需等级

	public string _GrowthEffect;//生命值上限|真气值上限|命中|攻击|防御|躲闪|暴击值|抗暴击
	public string _MaxGrowthEffect;
	public string _LevelUpText;//下一级收益提示文字
	//public int _LevelUpExp;//达到下一级所需经验
	//public int _GrowthCost;//炼妖花费修为
	//private string _sirenSkillID;
	public SirenSkillID[] _SirenSkillIDs;//妖女奥义ID
	public string _SirenSkillIDText;//妖女奥义作用文字描述

	//突破
	public int SirenBreakStage;			//阶段	
	public int BreakStageMaxLevel;		//当前阶段妖女上限等级
	public SirenBreakCondition[] BreakCondition;			//突破条件，下一等级所需材料
	public string BreakDesc;				//突破描述
	public int SirenTeamBuffID;			//妖女团队buff id
	public string SirenTeamBuffText;	//妖女团队buff描述
}

[System.Serializable]
public class SirenSkillID
{
	public int Vocation;
	public int SkillID;
}

[System.Serializable]
public class SirenDialogConfigData  //妖女对白配置
{
    public string IDS;
    public Vector2 Pos;
    public int Rows;
}
[System.Serializable]
public class UnlockCondition
{
	public int Type;
	public int Condition1;
	public int Condition2;
}

[System.Serializable]
public class SirenBreakCondition
{
	public int ItemID;
	public int ItemNum;
}

public class PlayerSirenConfigDataBase : ScriptableObject
{
    public PlayerSirenConfigData[] _dataTable;
}

public class SirenGrowthEffect
{
    public EffectData EffectData;   //加成信息
    public int GrowthEffectValue;   //加成值
	public int GrowthEffectMaxValue;//最大加成值
}
