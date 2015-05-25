using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class PlayerLevelConfigData
{
	public int _level;
	public int _hp;
	public int _xp;
	
	public int _skillPoint;
	public int _reviveHcCost;
	
	
}



public class PlayerLevelConfigDataBase :ScriptableObject
{

	public PlayerLevelConfigData[] _dataTable;
}
