using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PVPGroupListAward
{
	public int ListAward;								// 排行榜奖励ID
	public int ListAwardType;						// 排行榜奖励类型:1=每日奖励、2=赛季奖励；
	public int ListAwardGroup;					// 排行榜组别
	public string ListAwardPlace;					// 奖励排名段, 格式：排名下限 + 排名上限
	public string ListAwardParam1;				// 奖励参数1：填写格式“奖励类型+奖励数值”，奖励类型可填写现有货币资源奖励、贡献、道具奖励；
	public string ListAwardParam2;				// 奖励数数2：格式同奖励参数1
	public string ListAwardParam3;				// 奖励数数3：格式同奖励参数1
	public int ListAwardMail;						// 奖励发送邮件，读取邮件标题、正文内容，邮件附件奖励读取ListAwardParam1-3，每个奖励发送一封邮件
	public string ListAwardIcon;					// 奖励组别图标
	public string ListAwardName;				// 奖励组别名称
	public string ListAward01Icon;				// 奖励1图标
	public string ListAward01Des;				// 奖励1文字
	public string ListAward02Icon;				// 奖励2图标
	public string ListAward02Des;				// 奖励2文字
}

public class PVPGroupListAwardDataBase : ScriptableObject
{

	public PVPGroupListAward[] _dataTable;

}
