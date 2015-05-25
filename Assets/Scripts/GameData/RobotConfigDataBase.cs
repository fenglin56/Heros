using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class RobotConfigData
{
	public int RobotId;
	public string RobotName;
	public int RobotLevel;
	public int RobotOccupation;
	public int RobotFashion;
	public int RobotWeapon;
	public int RobotHat;
	public int RobotClothes;
	public int RobotShoes;
	public int RobotJewelry;
	public int RobotVipLevel;
	public int RobotTitle;
}


public class RobotConfigDataBase : ScriptableObject 
{
	public RobotConfigData[] _dataTable;
}
