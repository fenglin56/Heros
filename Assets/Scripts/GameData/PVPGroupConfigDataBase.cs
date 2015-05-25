using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class PVPGroupConfig
{
	public int PVPGroupID;									// 组别编号
	public string PVPGroupName;						// 组别名称
	public string GroupLevelUp_Score;				// 本组别积分段， 格式为：积分下限 + 积分上限
	public int GroupLevelUp_Rank;						// 晋级至本组所需排名：填0，表示无要求
	public string GroupLevelUpIDS;					// 晋级说明文字
	public string PVPGroupIcon;							// 组别图标
}

public class PVPGroupConfigDataBase : ScriptableObject 
{
	public PVPGroupConfig[] _dataTable;
}
