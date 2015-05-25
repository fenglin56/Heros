using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class FirstBattlePrizeData
{
	public int UserLevel;
	public int[] Monday; //itemID + itemNum
	public int[] Tuesday;
	public int[] Wednesday;
	public int[] Thursday; 
	public int[] Friday;
	public int[] Saturday;
	public int[] Sunday;
}



public class FirstBattlePrizeDataBase : ScriptableObject
{
	public FirstBattlePrizeData[] _dataTable;
}
