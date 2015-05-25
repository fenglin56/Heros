using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class TownRobotPosData
{
	public int PosId;
	public Vector3 BornPos;
	public float BornOrientation;
}

public class TownRobotPosDataBase : ScriptableObject 
{
	public TownRobotPosData[] _dataTable;
}
