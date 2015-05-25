using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class PropKeyConfigData
{
	public string szDescription;
	public int nPropID;
	public int nSettleID;
}

public class PropKeyConfigDataBase : ScriptableObject 
{
	public  PropKeyConfigData[] _dataTable;
}
