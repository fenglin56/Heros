using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SirenSkillData 
{
    public int _SirenSkill_id;
    public int _vocation;
    public SirenSkill[] _SirenSkills;
}

[Serializable]
public class SirenSkill
{
    public int _SirenID;
    public string _icon;
    public string _icon_circle;   
    public int _SkillID;
    public float _Duration;
}

public class SirenSkillDataBase : ScriptableObject
{
    public SirenSkillData[] _dataTable;
}
