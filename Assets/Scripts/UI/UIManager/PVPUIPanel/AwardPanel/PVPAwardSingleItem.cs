using UnityEngine;
using System.Collections;

public class PVPAwardSingleItem : MonoBehaviour 
{
	public GameObject AwardType;				// 奖励部分
	public GameObject DesType;					// 组别说明部分： 和奖励部分同一时间，只能有一个出现
	public GameObject LeftBackgroundDeep;
	public GameObject LeftBackgroundShallow;
	public GameObject RightBackgroundDeep;
	public GameObject RightBackgroundShallow;

	public UISprite LevelIcon;							// 组别图标
	public UILabel LeftLabel;							// 组别名称

	public UILabel DescriptionLabel;				//	组别说明时显示：组别描述

	public UISprite AwardLeftIcon;					//	奖励左边的图标
	public UILabel AwardLeftDes;					//	奖励左边的描述
	public UISprite AwardRightIcon;				//	奖励右边的图标
	public UILabel AwardRightDes;					// 奖励右边的描述

	private bool HasInit;

	public void LevelDesInit(int sequence,string levelName, string levelDes)
	{
		//LevelIcon.ChangeSprite(sequence + 1);
		//LeftLabel.text =  LanguageTextManager.GetString(levelName);
		DescriptionLabel.text = LanguageTextManager.GetString(levelDes);

		AwardType.SetActive(false);
		DesType.SetActive(true);
	}

	public void AwardDesInit(int sequence, string iconName, string levelName,string leftIconID, string leftDesID, string rightIconID, string rightDesID)
	{

		if(!HasInit)
		{
			LevelIcon.spriteName = iconName;
			bool showDeep = (sequence % 2 == 0);

			LeftBackgroundDeep.SetActive(showDeep);
			LeftBackgroundShallow.SetActive(!showDeep);
			RightBackgroundDeep.SetActive(showDeep);
			RightBackgroundShallow.SetActive(!showDeep);

			LeftLabel.text =  LanguageTextManager.GetString(levelName);

			HasInit = true;
		}

		AwardLeftIcon.spriteName = leftIconID;
		AwardLeftDes.text = LanguageTextManager.GetString(leftDesID);
		AwardRightIcon.spriteName = rightIconID;
		AwardRightDes.text = LanguageTextManager.GetString(rightDesID);

		AwardType.SetActive(true);
		DesType.SetActive(false);
	}
}
