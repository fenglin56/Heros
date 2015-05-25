using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class BulletGroup
{
	public int m_bulletId;
	public float m_delay;
}

[Serializable]
public class SkillConfigData
{
	public int m_skillId;
	public int m_unlockLevel;
    public int m_maxLv;
    public int m_UpdateInterval;
	public int m_vocation;
    public int m_skillType;
    //[GameDataPostFlag(true)]
	public GameObject m_icon;
    public string m_iconName;
    public string icon_circle;
    public GameObject Icon_CirclePrefab;
	public string m_name;
	public string m_descSimple;
	public string m_desc;
	public float m_chantTime;
	public int m_range;
	public int m_element;
    public float[] m_mightParams;
	public float[] m_manaConsumeParams;
	public string m_upgradeMsg1;
    public string m_upgradeMsg2;
    public string m_upgradeMsg3;
    public string m_upgradeMsg4;
	public float[] m_upgradeSkillPointParams;
	public int[] m_upgradeMoneyParams;
	public int m_upgradeItemId;
	public int m_upgradeItemCount;
	public float[] m_upgradeParams;
	public float[] m_upgradeHurtParams;
    public int m_breakLevel;
	public int m_skillAttacktimes;
	//public float[] m_skillDamage;
	public List<int> m_skillDamageList;
	public float m_coolDown;
	public int m_triggerType;
	public int m_triggerTarget;
	public int[] m_triggerRange;
	public int m_directionParam;
	public int[] m_launchRange;
	public int[] m_actionId;
	public BulletGroup[] m_bulletGroups;
	public BulletGroup[] m_bulletStrengGroups;
    public bool m_IsSirenSkill;
	
	public string[] m_skillSfxGroup;

    /// <summary>
    /// 是否进行锁定目标，由技能动作是否有设置跟随参数为1而定
    /// </summary>
    public bool IsLockTarget = false;
	
	
	public int m_affectTarget;

    public List<UIEffectGroup> m_UIEffectGroupList;

    public float skillCameraRange;
    public Vector3 cameraRangeOffset;
    public int[] cameraIdList;

    public bool AutoDirecting;
	public string skillText;
	public List<int> HintTextList;
	public float energy_comsume;//
	public GameObject energyComsumePrefab;
	public int energy_comsumeParam;
    public int[] ComboSkill;   //多段技能里面的后续技能配置，若为0则没有后续技能
	//新增
	public int PreSkill;
	public int PostSkill;
	public int SkillTreeID;
	public string Advanced_item;
	public int advItemID;
	public int advItemCount;
	public int SkillStrengthen;//0表示不能强化，非0表示可强化的上限
	public string SkillStrengthen_Money;
	public List<int> skillStrMoneyList = new List<int> ();
	public string SkillStrengthen_bullet_id;
	public string SkillStrengthen_Text;
	public string SkillStrengthen_Damage;
	public List<int> skillStrDamegeList = new List<int> ();
    public int FatherSkill;
}


[Serializable]
public class UIEffectGroup
{    
    public GameObject _UIEffectPrefab;
    public float _EffectStartTime;
    public float _EffectDuration;
    public Vector3 _EffectStartPos;
}


public class SkillConfigDataBase : ScriptableObject
{
    public SkillConfigData[] _dataTable;
}
