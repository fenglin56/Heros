using UnityEngine;
using System.Collections;
using Chat;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;

namespace Chat
{
	public enum WindowType
	{
		Init,
		Town = 1,
		World,
		System,
		Team,
		Private,
	}
}

public class ChatPanel_V2 : BaseUIPanel
{
	public LocalButtonCallBack Button_OpenWorldPanel;
	public GameObject NewChat_prompt;
	
	public GameObject ChatPanel;
	public LocalButtonCallBack Button_Send;
	public UIInput Input_Chat;
	public ChatInfoItemControl ChatInfoItemPrefab;
	public UIDraggablePanel DraggablePanel;
	public UITable Table_Chat;
	public ClickTalkerBoxControl TalkerBoxControl;    
	
	private uint iTalkToWorldID = 0;
	
	private List<ChatInfoItemControl> ChatRecordList = new List<ChatInfoItemControl>();

	#region 新增
	public LocalButtonCallBack Button_Close;

	public LocalButtonCallBack Button_Town;
	public LocalButtonCallBack Button_World;
	public LocalButtonCallBack Button_System;
	public LocalButtonCallBack Button_Team;
	
	public Transform TitlePoint;
	private Vector3 TitleInitialPos;

	public GameObject TalkerItemPrefab;

	public GameObject ChatWindowItemPrefab;
	public Transform DraggablePanelList;

	public GameObject Speaker;
	public UILabel Label_SpeakerNum;

	public LocalButtonCallBack Button_PageUp;
	public LocalButtonCallBack Button_PageDown;
	public GameObject Tip_PageUp;

	private Dictionary<WindowType, LocalButtonCallBack> m_buttonsDict = new Dictionary<WindowType, LocalButtonCallBack>();
	private Dictionary<WindowType, ChatWindowItem> m_windowDict = new Dictionary<WindowType, ChatWindowItem>();
	private Dictionary<int, ChatWindowItem> m_privateWindowDict = new Dictionary<int, ChatWindowItem>();
	private Dictionary<int, PrivateTalkerItem> m_privateTalkerDict = new Dictionary<int, PrivateTalkerItem>();
	private WindowType m_curWindowType;

	private int m_PageNum = 0;
	private int m_CurTalkerIndex = 0;
	const float SPACING_TITLE_BUTTON = 100f;
	const int MAXNUM_TITLE_ONEPAGE = 5;
	#endregion

	#region 预览窗口

	public GameObject ChatPanelPreviewPrefab;
	private ChatPanel_Preview m_Panel_Preview;



	#endregion


	void Awake()
	{
		Button_OpenWorldPanel.SetCallBackFuntion(OpenWorldPanel, null);        
		Button_Send.SetCallBackFuntion(SendChat, null);
		
		//TODO GuideBtnManager.Instance.RegGuideButton(Button_OpenWorldPanel.gameObject, UIType.Chat, SubType.ButtomCommon, out m_guideBtnID);
		
		RegisterEventHandler();
		
		//初始化
		m_buttonsDict.Add(WindowType.Town, Button_Town);
		m_buttonsDict.Add(WindowType.World, Button_World);
		m_buttonsDict.Add(WindowType.System, Button_System);
		m_buttonsDict.Add(WindowType.Team, Button_Team);

		//初始化私聊窗口
		ChatRecordManager.Instance.GetPrivateChatRecordDict().ApplyAllItem(p=>{
			GameObject talker = UI.CreatObjectToNGUI.InstantiateObj(TalkerItemPrefab,TitlePoint);
			PrivateTalkerItem privateTalkerItem = talker.GetComponent<PrivateTalkerItem>();
			string talkerName = "";
			if(p.Value[0].SenderName == PlayerManager.Instance.FindHeroDataModel().Name)
			{
				talkerName = p.Value[0].AccepterName;
			}
			else
			{
				talkerName = p.Value[0].SenderName;
			}
			privateTalkerItem.Init(p.Value[0].L_ChaterID,talkerName);
			m_privateTalkerDict.Add(p.Value[0].L_ChaterID,privateTalkerItem);						
			InitPrivateWindow(p.Value[0].L_ChaterID,ChatRecordManager.Instance.GetPrivateChatRecordList(p.Value[0].L_ChaterID));
			//开启未读动画提示
			m_privateTalkerDict[p.Value[0].L_ChaterID].ShowNewTip();
		});

		int index = 0;
		m_privateTalkerDict.ApplyAllItem(p=>{
			p.Value.transform.localPosition = Vector3.right * SPACING_TITLE_BUTTON * index;
			index++;
		});	

		Button_Close.SetCallBackFuntion(OnCloseClick, null);

		Button_Town.SetCallBackFuntion(OnTownClick,null);
		Button_World.SetCallBackFuntion(OnWorldClick, null);
		Button_System.SetCallBackFuntion(OnSystemClick, null);
		Button_Team.SetCallBackFuntion(OnTeamClick, null);

		Button_PageUp.SetCallBackFuntion(OnPageUpClick, null);
		Button_PageDown.SetCallBackFuntion(OnPageDownClick, null);

		TitleInitialPos = TitlePoint.transform.localPosition;

	}
	#region Test Data
#if UNITY_EDITOR
	void Update()
	{
		if(Application.platform == RuntimePlatform.WindowsEditor)
		{
			if(Input.GetKeyDown( KeyCode.F1))
			{
				TalkerInfo talkerInfo = new TalkerInfo();
				talkerInfo.Name = "饭后"+(Time.time*100).ToString();
				talkerInfo.ActorID = 100+(int)Time.time;
				OpenPrivateChatWindowHandle(talkerInfo);
			}
		}
	}
#endif
	#endregion
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
	private void InitPrivateWindow(int chaterID, List<SMsgChat_SC> list)
	{
		GameObject window = UI.CreatObjectToNGUI.InstantiateObj(ChatWindowItemPrefab,DraggablePanelList);
		ChatWindowItem chatWindowItem = window.GetComponent<ChatWindowItem>();
		m_privateWindowDict.Add(chaterID,chatWindowItem);
		chatWindowItem.InitWindow(list, null);
		if(iTalkToWorldID != chaterID)
		{
			chatWindowItem.Close();
		}
	}

	void OnCloseClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatClose");
		if(m_curWindowType == WindowType.Private)
		{
			int nextChaterID = 0;//下个窗口id
			if(m_privateTalkerDict.Count > 1)
			{
				List<int> list = m_privateTalkerDict.Keys.ToList();

				for(int i=0;i<list.Count;i++)
				{
					if(list[i] == iTalkToWorldID)
					{
						if(i > 0)
						{
							if(i == list.Count - 1)
							{
								nextChaterID = list[i-1];
							}
							else
							{
								nextChaterID = list[i+1];
							}
						}
						else
						{
							nextChaterID = list[1];
						}
					}
				}
				DeletePrivateWindow((int)iTalkToWorldID);
				SwitchingPrivateWindow(nextChaterID,"");

				UpdatePageButtons(false);
			}
			else
			{
				DeletePrivateWindow((int)iTalkToWorldID);
				CloseChatPanel();
			}
		}
		else
		{
			CloseChatPanel();
		}
	}
		
	//销毁私聊单窗口
	private void DeletePrivateWindow(int chaterID)
	{
		var talker = m_privateTalkerDict[chaterID];
		var window = m_privateWindowDict[chaterID];
		m_privateTalkerDict.Remove(chaterID);
		m_privateWindowDict.Remove(chaterID);
		Destroy(talker.gameObject);
		Destroy(window.gameObject);

		StartCoroutine("LateUpdateTitlePos");
	}
	IEnumerator LateUpdateTitlePos()
	{
		yield return new WaitForEndOfFrame();
		int index = 0;
		m_privateTalkerDict.ApplyAllItem(p=>{
			p.Value.transform.localPosition = Vector3.right * SPACING_TITLE_BUTTON * index;
			index++;
		});
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

	void OnPageUpClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatChoice");
		TitlePoint.transform.localPosition += Vector3.right * MAXNUM_TITLE_ONEPAGE * SPACING_TITLE_BUTTON;
		m_PageNum--;
		UpdatePageButtons(true);
	}
	void OnPageDownClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatChoice");
		TitlePoint.transform.localPosition += Vector3.left	* MAXNUM_TITLE_ONEPAGE * SPACING_TITLE_BUTTON;
		m_PageNum++;
		UpdatePageButtons(true);
	}
	//更新翻页按钮状态
	private void UpdatePageButtons(bool isPage)
	{
		int curIndex = 0;
		List<int> list = m_privateTalkerDict.Keys.ToList();			
		for(int i=0;i<list.Count;i++)
		{
			if(list[i] == iTalkToWorldID)
			{
				curIndex = i + 1;
				break;
			}
		}
		if(!isPage)
		{
			m_PageNum = Mathf.CeilToInt(curIndex*1f/MAXNUM_TITLE_ONEPAGE)-1;
			TitlePoint.transform.localPosition = TitleInitialPos + Vector3.left * MAXNUM_TITLE_ONEPAGE * SPACING_TITLE_BUTTON * m_PageNum;			
		}

		//更新按钮状态
		if(m_privateTalkerDict.Count > MAXNUM_TITLE_ONEPAGE)
		{
			Button_PageDown.gameObject.SetActive(true);
			Button_PageUp.gameObject.SetActive(true);

			int maxNum = Mathf.CeilToInt(m_privateTalkerDict.Count *1f /MAXNUM_TITLE_ONEPAGE);

			bool isCanClickLeftBtn = m_PageNum > 0;
			bool isCanClickRightBtn = m_PageNum < maxNum -1;
			Button_PageUp.SetBoxCollider(isCanClickLeftBtn);
			Button_PageUp.SetAlpha(isCanClickLeftBtn == true ?1f:0.5f);
			Button_PageDown.SetBoxCollider(isCanClickRightBtn);
			Button_PageDown.SetAlpha(isCanClickRightBtn== true ?1f:0.5f);
		}
		else
		{
			Button_PageDown.gameObject.SetActive(false);
			Button_PageUp.gameObject.SetActive(false);
		}		

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

		//界面功能
		bool isWorldWindow = type == WindowType.World;
		if(isWorldWindow)
		{
			int itemNum = ContainerInfomanager.Instance.GetItemNumber(CommonDefineManager.Instance.CommonDefine.WorldChatItem);
			Label_SpeakerNum.text = itemNum.ToString();			
			Label_SpeakerNum.color = itemNum>0?new Color(1f,0.98f,0.435f):Color.red;
		}
		Speaker.SetActive(isWorldWindow);
		Button_Send.gameObject.SetActive(type != WindowType.System);
		Input_Chat.gameObject.SetActive(type != WindowType.System);
	}
	//切换私聊窗口
	private void SwitchingPrivateWindow(int chaterID, string chaterName)
	{
		m_curWindowType = WindowType.Private;

		iTalkToWorldID = (uint)chaterID;
		if(!m_privateTalkerDict.ContainsKey(chaterID))
		{
			//初始化
			GameObject talker = UI.CreatObjectToNGUI.InstantiateObj(TalkerItemPrefab,TitlePoint);
			PrivateTalkerItem privateTalkerItem = talker.GetComponent<PrivateTalkerItem>();
			privateTalkerItem.Init(chaterID,chaterName);
			m_privateTalkerDict.Add(chaterID,privateTalkerItem);
			int index = 0;
			m_privateTalkerDict.ApplyAllItem(p=>{
				p.Value.transform.localPosition = Vector3.right * SPACING_TITLE_BUTTON * index;
				index++;
			});
//			for(int i=0;i<m_privateTalkerDict.Count;i++)
//			{
//				m_privateTalkerDict[i].transform.localPosition = Vector3.right * SPACING_TITLE_BUTTON * i;
//			}
			InitPrivateWindow(chaterID,ChatRecordManager.Instance.GetPrivateChatRecordList(chaterID));
		}

		m_privateTalkerDict.ApplyAllItem(p=>{
			if(p.Key == chaterID)
			{
				p.Value.Show(true);
				m_privateWindowDict[p.Key].Show();
			}
			else
			{
				p.Value.Show(false);
				m_privateWindowDict[p.Key].Close();
			}
		});

		//关闭私信提醒
//		if(!m_privateTalkerDict.Values.Any(p=>p.IsHasNewMessage))
//		{
//			UIEventManager.Instance.TriggerUIEvent(UIEventType.ClosePrivateMessageTip,null);
//		}
		if(ChatPanel.transform.localPosition == Vector3.zero && m_curWindowType == WindowType.Private)
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ClosePrivateMessageTip,null);
		}

		Speaker.SetActive(false);
		Button_Send.gameObject.SetActive(true);
		Input_Chat.gameObject.SetActive(true);

		UpdatePageButtons(false);
	}


	//公共标题按钮更新
	private void UpdateTitleButtons(params WindowType[] type)
	{
		m_privateTalkerDict.ApplyAllItem(p=>p.Value.gameObject.SetActive(false));
		m_privateWindowDict.ApplyAllItem(p=>p.Value.Close());
//		m_buttonsDict.ApplyAllItem(p=>{
//			if(type.Any(k=>k == p.Key))
//			{
//				p.Value.gameObject.SetActive(true);
//			}
//			else
//			{
//				p.Value.gameObject.SetActive(false);
//			}
//		});
		m_buttonsDict.ApplyAllItem(p=>p.Value.gameObject.SetActive(false));
		for(int i=0;i<type.Length;i++)
		{
			m_buttonsDict[type[i]].gameObject.SetActive(true);
			m_buttonsDict[type[i]].transform.localPosition = TitlePoint.localPosition + (Vector3.right * SPACING_TITLE_BUTTON)*i;
		}

		Button_PageDown.gameObject.SetActive(false);
		Button_PageUp.gameObject.SetActive(false);
	}
	//私聊标题按钮更新
	private void UpdateTitleNames()
	{
		m_buttonsDict.ApplyAllItem(p=>p.Value.gameObject.SetActive(false));
		m_windowDict.ApplyAllItem(p=>p.Value.Close());
		m_privateTalkerDict.ApplyAllItem(p=>p.Value.gameObject.SetActive(true));

	}


	public override void Show(params object[] value)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Chat");

		ShowChatPanel();

		WindowType type = WindowType.Town;
		if(value.Length > 0)
		{
			type = (WindowType)value[0];
		}

		switch(type)
		{
		case WindowType.Town:
		case WindowType.World:
		case WindowType.System:
			UpdateTitleButtons(WindowType.Town,WindowType.World,WindowType.System);
			SwitchingPublicWindow(type);
			break;
		case WindowType.Team:
			UpdateTitleButtons(WindowType.Team);
			SwitchingPublicWindow(type);
			break;
		case WindowType.Private:
			UpdateTitleNames();
			if(value.Length > 2)
			{
				SwitchingPrivateWindow((int)value[1], (string)value[2]);
			}
			else
			{
				//默认选第一个
				List<int> list = m_privateTalkerDict.Keys.ToList();
				int id = 0;
				if(list.Count>0)
				{
					id = list[0];
				}
				SwitchingPrivateWindow(id,"");
			}
			break;
		}
		//base.Show(value);
	}
	
	public override void Close()
	{
		//PrivateChatWindowMgr.ClosePanel();

		CloseChatPanel();
		//base.Close();
	}
	
	
	private void ShowChatPanel()
	{
		ChatPanel.transform.localPosition = Vector3.zero;
		if (NewChat_prompt.activeInHierarchy)
		{
			NewChat_prompt.SetActive(false);
		}
	}
	
	private void CloseChatPanel()
	{
		ChatPanel.transform.localPosition = Vector3.back * 800;
	}
	
	
	void OpenWorldPanel(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
		//MainUIController.Instance.OpenMainUI(UIType.Chat,WindowType.Town);
		if(transform.localPosition == Vector3.zero)
		{
			Close();
			return;
		}
		Show(WindowType.Town);
	}
	
	void CloseWorldPanelHandle(object arg)
	{
		CloseChatPanel();
		//清除队伍记录
		if(m_windowDict.ContainsKey(WindowType.Team))
		{
			m_windowDict[WindowType.Team].ClearChats();
		}

		//PrivateChatWindowMgr.ClosePanel();
	}

	void OnBuySuccess(object obj)
	{
		if(m_curWindowType == WindowType.World)
		{
			int itemNum = ContainerInfomanager.Instance.GetItemNumber(CommonDefineManager.Instance.CommonDefine.WorldChatItem);
			Label_SpeakerNum.text = itemNum.ToString();
			Label_SpeakerNum.color = itemNum>0?new Color(1f,0.98f,0.435f):Color.red;
		}
	}

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
			chatDefine = ChatDefine.MSG_CHAT_PRIVATE;
			accpecterName = m_privateTalkerDict[(int)iTalkToWorldID].TalkerInfo.Name;
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
		if(sMsgChat_SC.L_Channel == (int)WindowType.World)
		{
			int itemNum = ContainerInfomanager.Instance.GetItemNumber(CommonDefineManager.Instance.CommonDefine.WorldChatItem);
			Label_SpeakerNum.text = itemNum.ToString();			
			Label_SpeakerNum.color = itemNum>0?new Color(1f,0.98f,0.435f):Color.red;
		}

		if(sMsgChat_SC.L_Channel == (int)WindowType.Team)
		{
			if(ChatPanel.transform.localPosition != Vector3.zero)
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.NewTeamMessage,null);
			}
		}
	}
	//接收私信
	void ReceiveChatMsgHandle(object obj)
	{
		SMsgChat_SC sMsgChat_SC = (SMsgChat_SC)obj;

		if(m_privateWindowDict.ContainsKey(sMsgChat_SC.L_ChaterID))
		{
			m_privateWindowDict[sMsgChat_SC.L_ChaterID].AddChat(sMsgChat_SC);
		}
		else
		{
			//初始化
			GameObject talker = UI.CreatObjectToNGUI.InstantiateObj(TalkerItemPrefab,TitlePoint);
			PrivateTalkerItem privateTalkerItem = talker.GetComponent<PrivateTalkerItem>();
			privateTalkerItem.Init(sMsgChat_SC.L_ChaterID,sMsgChat_SC.SenderName);
			m_privateTalkerDict.Add(sMsgChat_SC.L_ChaterID,privateTalkerItem);
			int index = 0;
			m_privateTalkerDict.ApplyAllItem(p=>{
				p.Value.transform.localPosition = Vector3.right * SPACING_TITLE_BUTTON * index;
				index++;
			});		
			InitPrivateWindow(sMsgChat_SC.L_ChaterID,ChatRecordManager.Instance.GetPrivateChatRecordList(sMsgChat_SC.L_ChaterID));
			if(m_curWindowType != WindowType.Private)
			{
				talker.SetActive(false);
			}
		}

		//开启未读动画提示
		if(ChatPanel.transform.localPosition != Vector3.zero || sMsgChat_SC.L_ChaterID != iTalkToWorldID)
		{
			m_privateTalkerDict[sMsgChat_SC.L_ChaterID].ShowNewTip();
		}

		//关闭私信提醒
//		if(ChatPanel.transform.localPosition == Vector3.zero && !m_privateTalkerDict.Values.Any(p=>p.IsHasNewMessage))
//		{
//			UIEventManager.Instance.TriggerUIEvent(UIEventType.ClosePrivateMessageTip,null);
//		}
		if(ChatPanel.transform.localPosition == Vector3.zero && m_curWindowType == WindowType.Private)
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ClosePrivateMessageTip,null);
		}
	}
	void CloseUIHandle(object obj)
	{
		this.CloseChatPanel();
	}

	IEnumerator SetDragAmount()
	{
		yield return new WaitForEndOfFrame();
		if (DraggablePanel.shouldMove)
		{
			DraggablePanel.SetDragAmount(0, 1, false);
		}   
	}
	
	void OpenPrivateChatWindowHandle(object obj)
	{
		TalkerInfo talkerInfo = (TalkerInfo)obj;
		//TraceUtil.Log("[OpenPrivateChatWindowHandle talkerInfo]" + talkerInfo.ActorID + " , " + talkerInfo.Name);
		//PrivateChatWindowMgr.OpenPrivateWindow(talkerInfo.ActorID, talkerInfo.Name);
		UpdateTitleNames();
		SwitchingPrivateWindow(talkerInfo.ActorID, talkerInfo.Name);
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

	
	protected override void RegisterEventHandler()
	{
		this.AddEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccessHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.WorldChatMsg, ShowWorldChatHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.PrivateChatMsg, ReceiveChatMsgHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseAllUI, CloseUIHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenPrivateChatWindow, OpenPrivateChatWindowHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseWorldChatWindow, CloseWorldPanelHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.ShopsBuySuccess, OnBuySuccess);
		//UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, CloseWorldPanelHandle);
		//UIEventManager.Instance.RegisterUIEvent(UIEventType.PrivateChatMsg, ShowPrivateChatHandle);
	}
	
	void OnDestroy()
	{
		this.RemoveEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccessHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.WorldChatMsg, ShowWorldChatHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.PrivateChatMsg, ReceiveChatMsgHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseAllUI, CloseUIHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenPrivateChatWindow, OpenPrivateChatWindowHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseWorldChatWindow, CloseWorldPanelHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShopsBuySuccess, OnBuySuccess);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, CloseWorldPanelHandle);
		//TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
	}
}
