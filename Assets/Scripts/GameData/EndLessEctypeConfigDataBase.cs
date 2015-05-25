using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class EndLessEctypeConfigData
{
	public int dwEctypeContainerId;
	public int WaveIndex;
	public string Reward;
	//解析奖励
	[HideInDataReaderAttribute]
	public List<CGoodsInfo> rewardList = new List<CGoodsInfo> ();
	public void GetReward()
	{
		string[] strArray = Reward.Split ('|');
		for (int i = 0; i < strArray.Length; i++) {
			string[] reArray = strArray[i].Split ('+');
			if(reArray.Length != 2)
			{
				continue;
			}
			CGoodsInfo temp = new CGoodsInfo();
			temp.itemID = int.Parse(reArray[0]);
			temp.itemCount = int.Parse(reArray[1]);
			rewardList.Add (temp);
		}
	}
}

public class EndLessEctypeConfigDataBase : ScriptableObject{
	public EndLessEctypeConfigData[] _dataTable;
}