using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class EctypeGuideStepConfigData
{
    public int _GuideStep;
    /// <summary>
    /// 0=玩家查看对白；1=移动至指定范围；2=所有的怪物死亡；3=施放指定技能；4=增加1格气力；5=将敌人切碎；6=自己成功技能打断；7=减速引导；8=指定技能击中怪物；9=图片引导；
    /// </summary>
    public int _StepType;
    public float _ReductionDelayTime; //减速延迟时间
    public int _ReductionRatio;  //减速倍数
    public float _StepDuration;   //步骤持续时间
    public int[] _DisableButtonList; //屏蔽按钮列表
    public string _SignTips;
    public TipsType[] _TipsType;
    public Vector3 _TipsPrefabOffset;
    public int[] _GuideIdList;
    public int _DelayTime;
    public float _ButtonEffectInterval;
    //public string _NpcIcon;//2014-8-15 关卡对话引导细节修改
    //public string _NpcName;
    //public string[] _DialogList;
    //public string _DialogTitle;
    public StepDialogInfo[] StepDialogInfos;
    public int _MountMonsterID;
    public GameObject _MonsterEffect;
    public Vector3 _EffectPos;
    public GameObject _GuideEffect;
    public GameObject _ButtonFlshing;
    public float _EffectAngle;
    public GameObject[] _GuidePicPrefabs;
    public string _StepSound;
    /// <summary>
    /// 目标挂载类型 0=不显示箭头，1=地图绝对位置，2=怪物
    /// </summary>
    public int MountType;  
    /// <summary>
    /// 目标信息 目标类型=0时，该字段填0；目标类型=1时，填写地图坐标；目标类型=2时，填写怪物ID
    /// </summary>
    public string TargetInformation;
}



public class EctypeGuideStepConfigDataBase : ScriptableObject
{
    public EctypeGuideStepConfigData[] _dataTable;
}
[Serializable]
public class StepDialogInfo
{
    public GameObject NpcIcon;
    public string NpcName;
    public string DialogContent;
    public Vector3 Offset;
    public bool IsHeroFlag;
}

