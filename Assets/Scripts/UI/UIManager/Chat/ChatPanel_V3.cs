using UnityEngine;
using System.Collections;
using Chat;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;

namespace Chat
{
//	public enum WindowType
//	{
//		Town = 1,
//		World,
//		System,
//		Team,
//		Private,
//	}

	public enum DisplayType
	{
		preview = 0,
		complete,
	}
}

public class ChatPanel_V3 : BaseUIPanel
{
	public GameObject ChatPanelPreviewPrefab;
	public GameObject ChatPanelPublicPrefab;
	public GameObject ChatPanelPrivatePrefab;
	public GameObject ChatPanelTeamPrefab;

	private ChatPanel_Preview m_Panel_Preview;
	private ChatPanel_Public m_Panel_Public;
	private ChatPanel_Private m_Panel_Private;
	private ChatPanel_Team m_Panel_Team;

	private DisplayType m_DisplayType = DisplayType.complete;
	
	void Awake()
	{	
		RegisterEventHandler();
		//初始化 预览窗口
		GameObject previewWindow = UI.CreatObjectToNGUI.InstantiateObj(ChatPanelPreviewPrefab, transform);
		m_Panel_Preview = previewWindow.GetComponent<ChatPanel_Preview>();
		//m_Panel_Preview.Close();

		//初始化 公共窗口
		GameObject publicWindow = UI.CreatObjectToNGUI.InstantiateObj(ChatPanelPublicPrefab, transform);
		m_Panel_Public = publicWindow.GetComponent<ChatPanel_Public>();

		//初始化 私聊窗口
		GameObject privateWindow = UI.CreatObjectToNGUI.InstantiateObj(ChatPanelPrivatePrefab, transform);
		m_Panel_Private = privateWindow.GetComponent<ChatPanel_Private>();
		m_Panel_Private.Close();

		//初始化 组队喊话窗口

	}
	WindowType type;
	public override void Show(params object[] value)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Chat");


		//ShowChatPanel();
		
		type = WindowType.Town;
		if(value.Length > 0)
		{
			type = (WindowType)value[0];
		}
		
		switch(type)
		{
		case WindowType.Init:
			m_Panel_Public.Close();			
			m_DisplayType =  DisplayType.preview;	
			ChatRecordManager.Instance.CurTownDisplayType = m_DisplayType;
			m_Panel_Preview.Show();
			break;
		case WindowType.Town:
		case WindowType.World:
		case WindowType.System:
			int typeIndex = (int)m_DisplayType;
			typeIndex = (typeIndex + 1) % 2;
			m_DisplayType = (DisplayType)typeIndex;	
			ChatRecordManager.Instance.CurTownDisplayType = m_DisplayType;
			switch(m_DisplayType)
			{
			case DisplayType.preview:
				m_Panel_Preview.Show();
				m_Panel_Public.Close();
				break;
			case DisplayType.complete:
				m_Panel_Preview.Close();
				m_Panel_Public.Show();
				m_Panel_Public.SwitchingPublicWindow(type);
				break;				
			}				
			break;
		case WindowType.Private:
			m_Panel_Private.UpdateTitleNames();
			m_Panel_Private.Show();
			if(value.Length > 2)
			{
				m_Panel_Private.SwitchingPrivateWindow((int)value[1], (string)value[2]);
			}
			else
			{
				//默认选第一个
				m_Panel_Private.SwitchingFirstWindow();
			}
			OpenPrivateChatWindowHandle(null);
			break;
		}
		//IsShow = false;
		transform.localPosition = Vector3.zero;
	}
	
	public override void Close()
	{
		//PrivateChatWindowMgr.ClosePanel();
		//m_Panel_Public.Close();
		if(MainUIController.Instance.CurrentUIStatus == UIType.Chat)
		{
			return;
		}

		m_Panel_Public.Close();

		m_DisplayType =  DisplayType.preview;	
		ChatRecordManager.Instance.CurTownDisplayType = m_DisplayType;
		m_Panel_Preview.Show();

		UIEventManager.Instance.TriggerUIEvent( UIEventType.ShowChatButton,null);

		//m_Panel_Team.Hide();
		//m_Panel_Private.Close();

		//base.Close();
		//transform.localPosition = Vector3.back * 1000;
	}
	

	
	private void CloseChatPanel()
	{
		//transform.localPosition = Vector3.back * 1000;
	}
	
	
	void OpenWorldPanel(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
		//MainUIController.Instance.OpenMainUI(UIType.Chat,WindowType.Town);
//		if(transform.localPosition == Vector3.zero)
//		{
//			Close();
//			return;
//		}
		Show(WindowType.Town);
	}
	
	void CloseWorldPanelHandle(object arg)
	{
		CloseChatPanel();
		//清除队伍记录
//		if(m_windowDict.ContainsKey(WindowType.Team))
//		{
//			m_windowDict[WindowType.Team].ClearChats();
//		}
		
		//PrivateChatWindowMgr.ClosePanel();
	}
	
	void AddFriendSuccessHandle(INotifyArgs arg)
	{
		m_Panel_Public.TalkerBoxControl.SetMakeFriendButtonActive();
	}
	


	//接收公共信息
	void ShowWorldChatHandle(object obj)
	{
		SMsgChat_SC sMsgChat_SC = (SMsgChat_SC)obj;
		

	}
	//接收私信
	void ReceiveChatMsgHandle(object obj)
	{
		SMsgChat_SC sMsgChat_SC = (SMsgChat_SC)obj;
		

	}
	void CloseUIHandle(object obj)
	{
		this.CloseChatPanel();
	}

	
	void OpenPrivateChatWindowHandle(object obj)
	{
		if(m_DisplayType == DisplayType.complete)
		{
			m_DisplayType = DisplayType.preview;
			ChatRecordManager.Instance.CurTownDisplayType = m_DisplayType;
			m_Panel_Preview.Show();
			m_Panel_Public.Close();
			UIEventManager.Instance.TriggerUIEvent( UIEventType.ShowChatButton,null);
		}
	}
	
	void OpenWorldChatWindowHandle(object obj)
	{
		this.OpenWorldPanel(obj);
	}
	
	void ClickChatTargetCallBack(object talkerInfo, Transform boxTrans)
	{
		TalkerInfo info = (TalkerInfo)talkerInfo;

	}
	void OnNpcTalkDealUI(object obj)
	{
		bool isShow = (bool)obj;
		if (isShow) {
			transform.localPosition = Vector3.zero;
		} else {
			transform.localPosition = Vector3.back*2000;
		}
	}
	void OnOpenTalkUIEvent(object obj)
	{
		transform.localPosition = new Vector3 (0,0,-2000);
		//gameObject.SetActive(false);
	}
	void OnCloseTalkUIEvent(object obj)
	{
		//gameObject.SetActive(true);
		transform.localPosition = Vector3.zero;
	}
	protected override void RegisterEventHandler()
	{
		this.AddEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccessHandle);
		//UIEventManager.Instance.RegisterUIEvent(UIEventType.WorldChatMsg, ShowWorldChatHandle);
		//UIEventManager.Instance.RegisterUIEvent(UIEventType.PrivateChatMsg, ReceiveChatMsgHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseAllUI, CloseUIHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenPrivateChatWindow, OpenPrivateChatWindowHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseWorldChatWindow, CloseWorldPanelHandle);
		//UIEventManager.Instance.RegisterUIEvent(UIEventType.ShopsBuySuccess, OnBuySuccess);
		//UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, CloseWorldPanelHandle);
		//UIEventManager.Instance.RegisterUIEvent(UIEventType.PrivateChatMsg, ShowPrivateChatHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.NpcTalkTaskDealUI, OnNpcTalkDealUI);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcTalkOpenEvent, OnOpenTalkUIEvent);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcTalkCloseEvent, OnCloseTalkUIEvent);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcGuideStartEvent, OnOpenTalkUIEvent);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcGuideStopEvent, OnCloseTalkUIEvent);

	}
	
	void OnDestroy()
	{
		this.RemoveEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccessHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.WorldChatMsg, ShowWorldChatHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.PrivateChatMsg, ReceiveChatMsgHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseAllUI, CloseUIHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenPrivateChatWindow, OpenPrivateChatWindowHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseWorldChatWindow, CloseWorldPanelHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShopsBuySuccess, OnBuySuccess);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, CloseWorldPanelHandle);
		//TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NpcTalkTaskDealUI, OnNpcTalkDealUI);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcTalkOpenEvent, OnOpenTalkUIEvent);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcTalkCloseEvent, OnCloseTalkUIEvent);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcGuideStartEvent, OnOpenTalkUIEvent);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcGuideStopEvent, OnCloseTalkUIEvent);
	}
}
