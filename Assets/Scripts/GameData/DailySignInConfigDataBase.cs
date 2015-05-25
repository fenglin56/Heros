using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class DailySignInConfigData
{
	public int RewardId;
	//public int RewardLevel;
	public string SignInReward1;
	public string SignInReward2;
	public string SignInReward3;
	public string SignInReward4;
	public string SignInReward5;
	public string SignInReward6;
	public string SignInReward7;
	public int CumulativeRewardDays1;
	public int CumulativeRewardDays2;
	public int CumulativeRewardDays3;
	public string CumulativeRewardItem1;
	public string CumulativeRewardItem2;
	public string CumulativeRewardItem3;
	public string CumulativeRewardRes1;
	public string CumulativeRewardRes2;
	public string CumulativeRewardRes3;
	public int CumulativeRewardTips1;
	public int CumulativeRewardTips2;
	public int CumulativeRewardTips3;
	//解析星期奖励
	[HideInDataReaderAttribute]
	public Dictionary<int,List<CGoodsInfo>> dailyRewardList = new Dictionary<int, List<CGoodsInfo>>();
	//解析累计奖励
	[HideInDataReaderAttribute]
	public Dictionary<int,List<CGoodsInfo>> accumRewardList = new Dictionary<int, List<CGoodsInfo>>();
	public void GetDailyReward()
	{
		if (dailyRewardList.Count != 0)
			return;
		//解析星期奖励
		dailyRewardList.Add(1,GetReward (SignInReward1));
		dailyRewardList.Add(2,GetReward (SignInReward2));
		dailyRewardList.Add(3,GetReward (SignInReward3));
		dailyRewardList.Add(4,GetReward (SignInReward4));
		dailyRewardList.Add(5,GetReward (SignInReward5));
		dailyRewardList.Add(6,GetReward (SignInReward6));
		dailyRewardList.Add(7,GetReward (SignInReward7));
		//解析累计奖励
		accumRewardList.Add (CumulativeRewardDays1,GetReward (CumulativeRewardItem1));
		accumRewardList.Add (CumulativeRewardDays2,GetReward (CumulativeRewardItem2));
		accumRewardList.Add (CumulativeRewardDays3,GetReward (CumulativeRewardItem3));
	}
	List<CGoodsInfo> GetReward(string reward)
	{
		List<CGoodsInfo> rewardList = new List<CGoodsInfo> ();
		string[] strArray = reward.Split ('|');
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
		return rewardList;
	}

}

public class DailySignInConfigDataBase : ScriptableObject{
	public DailySignInConfigData[] _dataTable;
}