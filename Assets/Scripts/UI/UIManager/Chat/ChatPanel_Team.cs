using UnityEngine;
using System.Collections;
using Chat;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;


public class ChatPanel_Team : MonoBehaviour
{
	public LocalButtonCallBack Button_OpenWorldPanel;
	public GameObject NewChat_prompt;
	
	public GameObject ChatPanel;
	public LocalButtonCallBack Button_Send;
	public UIInput Input_Chat;


	public LocalButtonCallBack Button_Team;
	//public ClickTalkerBoxControl TalkerBoxControl;    
	
	private uint iTalkToWorldID = 0;
	
	#region 新增
	public LocalButtonCallBack Button_Close;
	
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
	#endregion
	
	
	void Awake()
	{
		//Button_OpenWorldPanel.SetCallBackFuntion(OpenWorldPanel, null);        
		Button_Send.SetCallBackFuntion(SendChat, null);
		
		//TODO GuideBtnManager.Instance.RegGuideButton(Button_OpenWorldPanel.gameObject, UIType.Chat, SubType.ButtomCommon, out m_guideBtnID);

		m_buttonsDict.Add(WindowType.Team, Button_Team);

		//监听
		UIEventManager.Instance.RegisterUIEvent(UIEventType.WorldChatMsg, ShowWorldChatHandle);

		Button_Close.SetCallBackFuntion(OnCloseClick, null);
		
		TitleInitialPos = TitlePoint.transform.localPosition;

	}

	private void InitPublicWindow(WindowType type, List<SMsgChat_SC> list)
	{
		GameObject window = UI.CreatObjectToNGUI.InstantiateObj(ChatWindowItemPrefab,DraggablePanelList);
		ChatWindowItem chatWindowItem = window.GetComponent<ChatWindowItem>();
		m_windowDict.Add(type,chatWindowItem);
		chatWindowItem.InitWindow(list, null);
	}
	
	void OnCloseClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatClose");
		CloseChatPanel();
	}
	
		
	//切换公共窗口
	private void SwitchingPublicWindow(WindowType type)
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
		SwitchingPublicWindow(WindowType.Team);
	}

	public void Hide()
	{
		CloseChatPanel();
	}

	public void Close()
	{
		CloseChatPanel();
		//清除队伍记录
		if(m_windowDict.ContainsKey(WindowType.Team))
		{
			m_windowDict[WindowType.Team].ClearChats();
		}
	}

	public bool IsShow()
	{
		return transform.localPosition == Vector3.zero;
	}

	private void ShowChatPanel()
	{
		transform.localPosition = Vector3.zero;
		if (NewChat_prompt.activeInHierarchy)
		{
			NewChat_prompt.SetActive(false);
		}
	}
	
	void CloseChatPanel()
	{
		transform.localPosition = Vector3.back * 800;
	}
	
	
//	void OpenWorldPanel(object obj)
//	{
//		SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
//
//	}
	
//	void CloseWorldPanelHandle(object arg)
//	{
//		CloseChatPanel();
//		//清除队伍记录
//		if(m_windowDict.ContainsKey(WindowType.Team))
//		{
//			m_windowDict[WindowType.Team].ClearChats();
//		}
//	
//	}
	
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
		
		ChatDefine chatDefine = ChatDefine.MSG_CHAT_TEAM;
//		switch(m_curWindowType)
//		{
//		case WindowType.Town:
//			chatDefine = ChatDefine.MSG_CHAT_CURRENT;
//			break;
//		case WindowType.World:
//			
//			if(ContainerInfomanager.Instance.GetItemNumber(CommonDefineManager.Instance.CommonDefine.WorldChatItem) <= 0)
//			{
//				UI.MessageBox.Instance.Show(4,"",LanguageTextManager.GetString("IDS_I24_11"),LanguageTextManager.GetString("IDS_I24_12"),
//				                            LanguageTextManager.GetString("IDS_I24_13"),CancelQuickBuySpeaker,SureQuickBuySpeaker);
//				return;
//			}
//			
//			chatDefine = ChatDefine.MSG_CHAT_WORLD;
//			break;
//		case WindowType.System:
//			chatDefine = ChatDefine.MSG_CHAT_SYSTEM;
//			break;
//		case WindowType.Team:
//			chatDefine = ChatDefine.MSG_CHAT_TEAM;
//			break;
//		case WindowType.Private:
//			break;
//		}
		
		NetServiceManager.Instance.ChatService.SendChat((uint)playerData.ActorID, iTalkToWorldID, accpecterName, chat,0, chatDefine);
		
		Input_Chat.text = "";
	}
	
	//接收公共信息
	void ShowWorldChatHandle(object obj)
	{
		SMsgChat_SC sMsgChat_SC = (SMsgChat_SC)obj;
	
		if(sMsgChat_SC.L_Channel == (int)WindowType.Team)
		{
			if(m_windowDict.ContainsKey((WindowType)sMsgChat_SC.L_Channel))
			{
				m_windowDict[(WindowType)sMsgChat_SC.L_Channel].AddChat(sMsgChat_SC);
			}	

			if(transform.localPosition != Vector3.zero)
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.NewTeamMessage,null);
			}
		}
	}
	
	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.WorldChatMsg, ShowWorldChatHandle);
	}
}
