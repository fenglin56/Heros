using UnityEngine;
using System.Collections;

public class JoystickTownUI : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		UIEventManager.Instance.RegisterUIEvent(UIEventType.MainBtnOpenEvent, OnCloseJoystickEvent);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.MainBtnCloseEvent, OnOpenJoystickEvent);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcTalkOpenEvent, OnCloseJoystickEvent);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcTalkCloseEvent, OnOpenJoystickEvent);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcGuideStartEvent, OnCloseJoystickEvent);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcGuideStopEvent, OnOpenJoystickEvent);
	}
	void OpenJoystick()
	{
		gameObject.SetActive (false);
		gameObject.SetActive (true);
	}
	void CloseJoystick()
	{
		gameObject.SetActive (false);
	}

	void OnOpenJoystickEvent(object obj)
	{
		OpenJoystick ();
	}

	void OnCloseJoystickEvent(object obj)
	{
		CloseJoystick ();
	}
	void NpcSelectEvent(object obj)
	{
		CloseJoystick ();
	}
	void OnDestroy () {
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.MainBtnOpenEvent, OnCloseJoystickEvent);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.MainBtnCloseEvent, OnOpenJoystickEvent);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcTalkOpenEvent, OnCloseJoystickEvent);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcTalkCloseEvent, OnOpenJoystickEvent);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcGuideStartEvent, OnCloseJoystickEvent);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcGuideStopEvent, OnOpenJoystickEvent);
	}
}
