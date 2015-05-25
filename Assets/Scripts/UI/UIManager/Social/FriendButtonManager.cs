using UnityEngine;
using System.Collections;

public class FriendButtonManager : MonoBehaviour 
{	
	void Start ()
	{
		if(UI.Friend.FriendDataManager.Instance.GetRequestListData.Count > 0)
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UI.MainUI.UIType.Friend);
		}
	}

}
