using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class ActivityConfigData
{
	public int ActivityID;
	public string ActivityPictrue;
	[HideInDataReaderAttribute]
	public GameObject ActivityPictruePrefab;
	public string ActivityName;
	public int UIOrder;
	public string RuleDescription;
	public string QualifiedDescription;
	public int Qualified1;
	public string Reward1;
	public int Qualified2;
	public string Reward2;
	public int Qualified3;
	public string Reward3;
	public int Qualified4;
	public string Reward4;
	public int Qualified5;
	public string Reward5;
	public int Qualified6;
	public string Reward6;
	public int Qualified7;
	public string Reward7;
	public int Qualified8;
	public string Reward8;
	public int Qualified9;
	public string Reward9;
	public int Qualified10;
	public string Reward10;
	public int Qualified11;
	public string Reward11;
	public int Qualified12;
	public string Reward12;

	public int Qualified13;
	public string Reward13;
	public int Qualified14;
	public string Reward14;
	public int Qualified15;
	public string Reward15;
	public int Qualified16;
	public string Reward16;
	public int Qualified17;
	public string Reward17;
	public int Qualified18;
	public string Reward18;
	public int Qualified19;
	public string Reward19;
	public int Qualified20;
	public string Reward20;
	public int Qualified21;
	public string Reward21;
	public int Qualified22;
	public string Reward22;
	public int Qualified23;
	public string Reward23;

	//解析奖励
	[HideInDataReaderAttribute]
	public Dictionary<int,int> qualifiedList = new Dictionary<int, int>();
	//<第几类奖励><职业>
	[HideInDataReaderAttribute]
	public Dictionary<int,Dictionary<int,List<CGoodsInfo>>> rewardList = new Dictionary<int, Dictionary<int, List<CGoodsInfo>>>();
	public void GetActReward()
	{
		if (rewardList.Count != 0)
			return;
		qualifiedList.Add (1,Qualified1);
		qualifiedList.Add (2,Qualified2);
		qualifiedList.Add (3,Qualified3);
		qualifiedList.Add (4,Qualified4);
		qualifiedList.Add (5,Qualified5);
		qualifiedList.Add (6,Qualified6);
		qualifiedList.Add (7,Qualified7);
		qualifiedList.Add (8,Qualified8);
		qualifiedList.Add (9,Qualified9);
		qualifiedList.Add (10,Qualified10);
		qualifiedList.Add (11,Qualified11);
		qualifiedList.Add (12,Qualified12);

		qualifiedList.Add (13,Qualified13);
		qualifiedList.Add (14,Qualified14);
		qualifiedList.Add (15,Qualified15);
		qualifiedList.Add (16,Qualified16);
		qualifiedList.Add (17,Qualified17);
		qualifiedList.Add (18,Qualified18);
		qualifiedList.Add (19,Qualified19);
		qualifiedList.Add (20,Qualified20);
		qualifiedList.Add (21,Qualified21);
		qualifiedList.Add (22,Qualified22);
		qualifiedList.Add (23,Qualified23);
		//解析星期奖励
		if (Reward1.Equals ("0"))
			return;
		rewardList.Add(1,GetReward (Reward1));
		if (Reward2.Equals ("0"))
			return;
		rewardList.Add(2,GetReward (Reward2));
		if (Reward3.Equals ("0"))
			return;
		rewardList.Add(3,GetReward (Reward3));
		if (Reward4.Equals ("0"))
			return;
		rewardList.Add(4,GetReward (Reward4));
		if (Reward5.Equals ("0"))
			return;
		rewardList.Add (5, GetReward (Reward5));
		if (Reward6.Equals ("0"))
			return;
		rewardList.Add(6,GetReward (Reward6));
		if (Reward7.Equals ("0"))
			return;
		rewardList.Add(7,GetReward (Reward7));
		if (Reward8.Equals ("0"))
			return;
		rewardList.Add(8,GetReward (Reward8));
		if (Reward9.Equals ("0"))
			return;
		rewardList.Add(9,GetReward (Reward9));
		if (Reward10.Equals ("0"))
			return;
		rewardList.Add(10,GetReward (Reward10));
		if (Reward11.Equals ("0"))
			return;
		rewardList.Add(11,GetReward (Reward11));
		if (Reward12.Equals ("0"))
			return;
		rewardList.Add(12,GetReward (Reward12));

		if (Reward13.Equals ("0"))
			return;
		rewardList.Add(13,GetReward (Reward13));
		if (Reward14.Equals ("0"))
			return;
		rewardList.Add(14,GetReward (Reward14));
		if (Reward15.Equals ("0"))
			return;
		rewardList.Add(15,GetReward (Reward15));
		if (Reward16.Equals ("0"))
			return;
		rewardList.Add(16,GetReward (Reward16));
		if (Reward17.Equals ("0"))
			return;
		rewardList.Add(17,GetReward (Reward17));
		if (Reward18.Equals ("0"))
			return;
		rewardList.Add(18,GetReward (Reward18));
		if (Reward19.Equals ("0"))
			return;
		rewardList.Add(19,GetReward (Reward19));
		if (Reward20.Equals ("0"))
			return;
		rewardList.Add(20,GetReward (Reward20));
		if (Reward21.Equals ("0"))
			return;
		rewardList.Add(21,GetReward (Reward21));
		if (Reward22.Equals ("0"))
			return;
		rewardList.Add(22,GetReward (Reward22));
		if (Reward23.Equals ("0"))
			return;
		rewardList.Add(23,GetReward (Reward23));
	}
	Dictionary<int,List<CGoodsInfo>> GetReward(string strReward)
	{
		if (strReward == "0")
			return null;
		Dictionary<int,List<CGoodsInfo>> rewardList = new Dictionary<int, List<CGoodsInfo>> ();
		string[] strArray = strReward.Split ('|');
		for (int i = 0; i < strArray.Length; i++) {
			string[] reArray = strArray[i].Split ('+');
			if(reArray.Length != 3)
			{
				continue;
			}
			int classVal = int.Parse(reArray[0]);
			if(!rewardList.ContainsKey(classVal))
			{
				List<CGoodsInfo> reward = new List<CGoodsInfo>();
				rewardList.Add(classVal,reward);
			}
			CGoodsInfo temp = new CGoodsInfo();
			temp.itemID = int.Parse(reArray[1]);
			temp.itemCount = int.Parse(reArray[2]);
			rewardList[classVal].Add (temp);
		}
		return rewardList;
	}
	
}

public class ActivityConfigDataBase : ScriptableObject{
	public ActivityConfigData[] _dataTable;
}