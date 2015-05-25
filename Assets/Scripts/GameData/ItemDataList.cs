using UnityEngine;
using System;

[Serializable]
public class EffectData
{
    public string m_SzName;
    public int m_IEquipmentID;
    public int m_IskillID;
    public string IDS;
    public int BaseProp;
    public int BasePropView;
    public string EffectRes;//图标
    public int CombatPara;//战力计算千分比
}

public class ItemDataList : ScriptableObject 
{
	public ItemData[] _items;
    public EquipmentData[] _equipments;
    public MedicamentData[] _medicaments;
    public MaterielData[] _materiels;
	public Jewel[] _jewel;
    public ItemMedicamentConfigData[] _itemMedicamentConfigs;
    public EffectData[] _effects;
}


