using UnityEngine;
using System;
using System.Collections;
[Serializable]
public class JokeConfigData
{
	public int ID;  
	public string IDS;
	public int DelayTime;
}

public class JokeConfigDataBase : ScriptableObject
{
	public JokeConfigData[] JokeDataList;
	
}
