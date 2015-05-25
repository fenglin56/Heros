using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class BundleLevelData
{
	public int _level;
	public string[] _bundleNames;
}

public class BundleLevelDataBase : ScriptableObject 
{
	public BundleLevelData[] _bundleDataList;
	
	public int GetBundleLevel(string bundleName)
	{
		foreach(BundleLevelData data in _bundleDataList)
		{
			foreach(string itemName in data._bundleNames)
			{
				if(bundleName.Equals(itemName))	
				{
					return data._level;	
				}
			}
			
		}
		Debug.LogError("______wrong bundleNme, check the configFile!!!!!!!!!" + bundleName);
		return -1;
	}
	
}
