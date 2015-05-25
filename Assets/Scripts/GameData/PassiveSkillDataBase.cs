using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class PassiveSkillData
{
    public int SkillID;
    public int SkillLevel;
    public int MaxLevel;
    public string SkillName;
    public string SkillIcon;
    public string SkillDis;
    public int SkillType;

    public GameObject SkillIconPrefab;
}

public class PassiveSkillDataBase : ScriptableObject{

    public PassiveSkillData[] _dataTable;
}
