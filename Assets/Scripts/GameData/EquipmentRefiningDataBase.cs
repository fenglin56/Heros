using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class EquipmentRefiningData
{
    public int lGoodsSubClass;
    public int lColorLevel;
    public int lLevel_Min;
    public int lLevel_Max;
    public List<int> lLevelUpNeed;
}

public class EquipmentRefiningDataBase :ScriptableObject{

    public EquipmentRefiningData[] EquipmentRefiningDatatable;
}
