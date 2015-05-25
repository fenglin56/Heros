using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UI.MainUI;

[Serializable]
public class UpgradeRequire
{
    public int GoodsId;
    public int Count;

}

[Serializable]
public class PlayerStrengthCost
{
    public UpgradeType GainType;
    public int GainLevel;
	public EquiptType lGoodsSubClass;
	public List<UpgradeRequire> UpgradeRequires;
}

public class PlayerStrengthCostDataBase : ScriptableObject 
{
    public PlayerStrengthCost[] PlayerStrengthCostList;
	
}
