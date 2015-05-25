using UnityEngine;
using System.Collections;

public class ActivityItem : MonoBehaviour {
	//public UILabel labelName;
	public GameObject iconPoint;
	public SingleButtonCallBack btnItem;
	public SpriteSwith bgSpriteSwith;
	public GameObject effObj;
	private SpriteSwith fgSpriteSwith;
	// Use this for initialization
	private ActivityConfigData activityConfig;
	private UI.MainUI.ActivityPanel actParent;
	private bool isRead = false;
	void Init () {
		if (isRead)
			return;
		isRead = true;
		GameObject picture = UI.CreatObjectToNGUI.InstantiateObj(activityConfig.ActivityPictruePrefab,iconPoint.transform);
		fgSpriteSwith = picture.GetComponentInChildren<SpriteSwith>();
		btnItem.SetCallBackFuntion (OnClickEvent);
	}
	public void Show(UI.MainUI.ActivityPanel parent,int activeID)
	{
		activityConfig = PlayerDataManager.Instance.GetActivityData(activeID);
		actParent = parent;
		Init ();
		ShowView ();
	}
	void ShowView()
	{
		int getRewardMark = DailySignModel.Instance.reachConditionMap[activityConfig.ActivityID];
		if (getRewardMark == 1) {
			//播放特效
			effObj.SetActive(true);
		} else {
			//隐藏特效		
			effObj.SetActive(false);
		}
		if (DailySignModel.Instance.curSelectActivityID == activityConfig.ActivityID) {
			//labelName.text = string.Format("[802800]{0}[-]",LanguageTextManager.GetString(activityConfig.ActivityName));
			bgSpriteSwith.ChangeSprite(2);
			fgSpriteSwith.ChangeSprite(2);
		}
		else{
			//labelName.text = string.Format("[ffffff]{0}[-]",LanguageTextManager.GetString(activityConfig.ActivityName));
			bgSpriteSwith.ChangeSprite(1);
			fgSpriteSwith.ChangeSprite(1);
		}
	}
	void OnClickEvent(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Choose_Activity");
		actParent.SetSelectItem (activityConfig.ActivityID);
	}
}
