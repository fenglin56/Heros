using UnityEngine;
using System.Collections;
public enum EViewType
{
	//新装备
	ENewEquipType = 1,
	//新技能
	ENewSkillType,
	//签到
	EDailySign,
}
public class DailySignLoadManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("PopDailySign",0.1f);
	}
	void PopDailySign()
	{
		DailySignModel.Instance.PopDailySignPanel(false);
		//UI.MainUI.MainUIController.Instance.OpenMainUI (UI.MainUI.UIType.AuctionPanel);
	}
}
