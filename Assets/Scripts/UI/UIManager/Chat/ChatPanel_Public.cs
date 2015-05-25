using UnityEngine;
using System.Collections;
using Chat;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;


public class ChatPanel_Public : MonoBehaviour
{	
	public GameObject ChatPanel;
	public LocalButtonCallBack Button_Send;
	public UIInput Input_Chat;

	public ClickTalkerBoxControl TalkerBoxControl;    
	
	private uint iTalkToWorldID = 0;
	
	private List<ChatInfoItemControl> ChatRecordList = new List<ChatInfoItemControl>();

	public LocalButtonCallBack Button_Close;
	
	public LocalButtonCallBack Button_Town;
	public LocalButtonCallBack Button_World;
	public LocalButtonCallBack Button_System;
	public LocalButtonCallBack Button_Team;

	public Transform TitlePoint;
	private Vector3 TitleInitialPos;

	public GameObject ChatWindowItemPrefab;
	public Transform DraggablePanelList;
	
	private Dictionary<WindowType, LocalButtonCallBack> m_buttonsDict = new Dictionary<WindowType, LocalButtonCallBack>();
	private Dictionary<WindowType, ChatWindowItem> m_windowDict = new Dictionary<WindowType, ChatWindowItem>();

	private WindowType m_curWindowType;
	
	private int m_PageNum = 0;
	private int m_CurTalkerIndex = 0;
	const float SPACING_TITLE_BUTTON = 100f;
	const int MAXNUM_TITLE_ONEPAGE = 5;

	//public GameObject ChatPanel;
	public Vector3 Hide_Pos = new Vector3(0,-360,0);
	public Vector3 Show_Pos = new Vector3(0,-107,0);
	
	void Awake()
	{
		Button_Send.SetCallBackFuntion(SendChat, null);
		
		//初始化
		m_buttonsDict.Add(WindowType.Town, Button_Town);
		m_buttonsDict.Add(WindowType.World, Button_World);
		m_buttonsDict.Add(WindowType.System, Button_System);
		m_buttonsDict.Add(WindowType.Team, Button_Team);
		
		Button_Close.SetCallBackFuntion(OnCloseClick, null);
		
		Button_Town.SetCallBackFuntion(OnTownClick,null);
		Button_World.SetCallBackFuntion(OnWorldClick, null);
		Button_System.SetCallBackFuntion(OnSystemClick, null);
		Button_Team.SetCallBackFuntion(OnTeamClick, null);
	
		TitleInitialPos = TitlePoint.transform.localPosition;



		//监听
		UIEventManager.Instance.RegisterUIEvent(UIEventType.WorldChatMsg, ShowWorldChatHandle);
	}

	private void InitPublicWindow(WindowType type, List<SMsgChat_SC> list)
	{
		GameObject window = UI.CreatObjectToNGUI.InstantiateObj(ChatWindowItemPrefab,DraggablePanelList);
		ChatWindowItem chatWindowItem = window.GetComponent<ChatWindowItem>();
		m_windowDict.Add(type,chatWindowItem);
		if(type == WindowType.Town)
		{
			chatWindowItem.InitWindow(list, ClickChatTargetCallBack);
		}
		else
		{
			chatWindowItem.InitWindow(list, null);
		}
	}
	
	void OnCloseClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatClose");
		//Close();
		UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenWorldChatWindow,null);
	}

	
	void OnTownClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatChoice");
		SwitchingPublicWindow( WindowType.Town);
	}
	void OnWorldClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatChoice");
		SwitchingPublicWindow( WindowType.World);
	}
	void OnSystemClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatChoice");
		SwitchingPublicWindow( WindowType.System);
	}
	void OnTeamClick(object obj)
	{
	}


	//切换公共窗口
	public void SwitchingPublicWindow(WindowType type)
	{
		m_curWindowType = type;
		iTalkToWorldID = 0;
		m_buttonsDict.ApplyAllItem(p=>{
			if(p.Key == type)
			{
				//更新按钮状态
				p.Value.SetBoxCollider(false);
				p.Value.SetSwith(2);
				//更新面板显示
				if(!m_windowDict.ContainsKey(type))
				{
					//初始化
					InitPublicWindow(type, ChatRecordManager.Instance.GetPublicChatRecordList(type));
				}
				m_windowDict[type].Show();
			}
			else
			{
				p.Value.SetBoxCollider(true);
				p.Value.SetSwith(1);
				
				if(m_windowDict.ContainsKey(p.Key))
				{
					m_windowDict[p.Key].Close();
				}
			}
		});
		
		//界面功能
//		bool isWorldWindow = type == WindowType.World;
//		if(isWorldWindow)
//		{
//			int itemNum = ContainerInfomanager.Instance.GetItemNumber(CommonDefineManager.Instance.CommonDefine.WorldChatItem);
//			Label_SpeakerNum.text = itemNum.ToString();			
//			Label_SpeakerNum.color = itemNum>0?new Color(1f,0.98f,0.435f):Color.red;
//		}
		//Speaker.SetActive(isWorldWindow);
		Button_Send.gameObject.SetActive(type != WindowType.System);
		Input_Chat.gameObject.SetActive(type != WindowType.System);
	}
	
	
	//公共标题按钮更新
	private void UpdateTitleButtons(params WindowType[] type)
	{
		m_buttonsDict.ApplyAllItem(p=>p.Value.gameObject.SetActive(false));
		for(int i=0;i<type.Length;i++)
		{
			m_buttonsDict[type[i]].gameObject.SetActive(true);
			m_buttonsDict[type[i]].transform.localPosition = TitlePoint.localPosition + (Vector3.right * SPACING_TITLE_BUTTON)*i;
		}
	}

	public void Show()
	{
		transform.localPosition = Vector3.zero;
		TweenPosition.Begin(ChatPanel,0.2f,ChatPanel.transform.localPosition,Show_Pos,null);
	}

	public void Hide()
	{
		FinishHideHandle(null);
	}

	public void Close()
	{
		TweenPosition.Begin(ChatPanel,0.2f,ChatPanel.transform.localPosition,Hide_Pos,FinishHideHandle);
	}

	void FinishHideHandle(object obj)
	{
		transform.localPosition = Vector3.back * 1000;
	}
//	public override void Show(params object[] value)
//	{
//		SoundManager.Instance.PlaySoundEffect("Sound_Button_Chat");
//		
//		ShowChatPanel();
//		
//		WindowType type = WindowType.Town;
//		if(value.Length > 0)
//		{
//			type = (WindowType)value[0];
//		}
//		
//		switch(type)
//		{
//		case WindowType.Town:
//		case WindowType.World:
//		case WindowType.System:
//			UpdateTitleButtons(WindowType.Town,WindowType.World,WindowType.System);
//			SwitchingPublicWindow(type);
//			break;
//		case WindowType.Team:
//			UpdateTitleButtons(WindowType.Team);
//			SwitchingPublicWindow(type);
//			break;
//		case WindowType.Private:
//			UpdateTitleNames();
//			if(value.Length > 2)
//			{
//				SwitchingPrivateWindow((int)value[1], (string)value[2]);
//			}
//			else
//			{
//				//默认选第一个
//				List<int> list = m_privateTalkerDict.Keys.ToList();
//				int id = 0;
//				if(list.Count>0)
//				{
//					id = list[0];
//				}
//				SwitchingPrivateWindow(id,"");
//			}
//			break;
//		}
//		//base.Show(value);
//	}
//	
//	public override void Close()
//	{
//		//PrivateChatWindowMgr.ClosePanel();
//		
//		CloseChatPanel();
//		//base.Close();
//	}


	void OpenWorldPanel(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
		if(transform.localPosition == Vector3.zero)
		{
			//Close();
			return;
		}
		//Show(WindowType.Town);
	}
	
//	void OnBuySuccess(object obj)
//	{
//		if(m_curWindowType == WindowType.World)
//		{
//			int itemNum = ContainerInfomanager.Instance.GetItemNumber(CommonDefineManager.Instance.CommonDefine.WorldChatItem);
//			Label_SpeakerNum.text = itemNum.ToString();
//			Label_SpeakerNum.color = itemNum>0?new Color(1f,0.98f,0.435f):Color.red;
//		}
//	}
	
	void AddFriendSuccessHandle(INotifyArgs arg)
	{
		TalkerBoxControl.SetMakeFriendButtonActive();
	}
	
	void SendChat(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
		string chat = Input_Chat.text;
		if (chat == "")
		{
			return;
		}
		string accpecterName = "";
		var playerData = PlayerManager.Instance.FindHeroDataModel();
		
		ChatDefine chatDefine = ChatDefine.MSG_CHAT_CURRENT;
		switch(m_curWindowType)
		{
		case WindowType.Town:
			chatDefine = ChatDefine.MSG_CHAT_CURRENT;
			break;
		case WindowType.World:
			
			if(ContainerInfomanager.Instance.GetItemNumber(CommonDefineManager.Instance.CommonDefine.WorldChatItem) <= 0)
			{
				UI.MessageBox.Instance.Show(4,"",LanguageTextManager.GetString("IDS_I24_11"),LanguageTextManager.GetString("IDS_I24_12"),
				                            LanguageTextManager.GetString("IDS_I24_13"),CancelQuickBuySpeaker,SureQuickBuySpeaker);
				return;
			}
			
			chatDefine = ChatDefine.MSG_CHAT_WORLD;
			break;
		case WindowType.System:
			chatDefine = ChatDefine.MSG_CHAT_SYSTEM;
			break;
		case WindowType.Team:
			chatDefine = ChatDefine.MSG_CHAT_TEAM;
			break;
		case WindowType.Private:
			break;
		}
		
		NetServiceManager.Instance.ChatService.SendChat((uint)playerData.ActorID, iTalkToWorldID, accpecterName, chat,0, chatDefine);
		
		Input_Chat.text = "";
	}
	void CancelQuickBuySpeaker()
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatBuyHornCancel");
	}
	void SureQuickBuySpeaker()
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatBuyHornConfirmation");
		if(ContainerInfomanager.Instance.GetEmptyPackBoxNumber() < 1)
		{
			StartCoroutine(LateShowNotEnoughtPackageTip());
		}
		else
		{
			PopupObjManager.Instance.OpenQuickBuyPanel(CommonDefineManager.Instance.CommonDefine.QuickBuyWorldChatItem);
		}
	}
	IEnumerator LateShowNotEnoughtPackageTip()
	{
		yield return new WaitForEndOfFrame();
		UI.MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I22_39"),1f);
	}
	//接收公共信息
	void ShowWorldChatHandle(object obj)
	{
		SMsgChat_SC sMsgChat_SC = (SMsgChat_SC)obj;
		
		if(m_windowDict.ContainsKey((WindowType)sMsgChat_SC.L_Channel))
		{
			m_windowDict[(WindowType)sMsgChat_SC.L_Channel].AddChat(sMsgChat_SC);
		}	
		
		//更新喇叭数量显示
//		if(sMsgChat_SC.L_Channel == (int)WindowType.World)
//		{
//			int itemNum = ContainerInfomanager.Instance.GetItemNumber(CommonDefineManager.Instance.CommonDefine.WorldChatItem);
//			Label_SpeakerNum.text = itemNum.ToString();			
//			Label_SpeakerNum.color = itemNum>0?new Color(1f,0.98f,0.435f):Color.red;
//		}

	}


	void OpenWorldChatWindowHandle(object obj)
	{
		this.OpenWorldPanel(obj);
	}
	
	void ClickChatTargetCallBack(object talkerInfo, Transform boxTrans)
	{
		TalkerInfo info = (TalkerInfo)talkerInfo;
		
		TalkerBoxControl.transform.position = boxTrans.position;
		TalkerBoxControl.gameObject.SetActive(true);
		
		TalkerBoxControl.SetTargetTalkerInfo(info);
		//this.sTalkTargetName = name;
		//Label_ToChatTarger.text = "@" + sTalkTargetName + ":";
	}
	
	
//	protected override void RegisterEventHandler()
//	{
//		//this.AddEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccessHandle);
//		UIEventManager.Instance.RegisterUIEvent(UIEventType.WorldChatMsg, ShowWorldChatHandle);
//		UIEventManager.Instance.RegisterUIEvent(UIEventType.PrivateChatMsg, ReceiveChatMsgHandle);
//		UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseAllUI, CloseUIHandle);
//		UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenPrivateChatWindow, OpenPrivateChatWindowHandle);
//		UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
//		UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseWorldChatWindow, CloseWorldPanelHandle);
//		UIEventManager.Instance.RegisterUIEvent(UIEventType.ShopsBuySuccess, OnBuySuccess);
//		//UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, CloseWorldPanelHandle);
//		//UIEventManager.Instance.RegisterUIEvent(UIEventType.PrivateChatMsg, ShowPrivateChatHandle);
//	}
//	
	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.WorldChatMsg, ShowWorldChatHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.PrivateChatMsg, ReceiveChatMsgHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseAllUI, CloseUIHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenPrivateChatWindow, OpenPrivateChatWindowHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseWorldChatWindow, CloseWorldPanelHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShopsBuySuccess, OnBuySuccess);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, CloseWorldPanelHandle);
		//TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
	}
}
