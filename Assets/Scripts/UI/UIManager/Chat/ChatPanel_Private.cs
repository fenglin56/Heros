using UnityEngine;
using System.Collections;
using Chat;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;

public class ChatPanel_Private : MonoBehaviour
{	
	public GameObject ChatPanel;
	public LocalButtonCallBack Button_Send;
	public UIInput Input_Chat;   

	private uint iTalkToWorldID = 0;
	
	private List<ChatInfoItemControl> ChatRecordList = new List<ChatInfoItemControl>();
	
	#region 新增
	public LocalButtonCallBack Button_Close;
	
	public Transform TitlePoint;
	private Vector3 TitleInitialPos;
	
	public GameObject TalkerItemPrefab;
	
	public GameObject ChatWindowItemPrefab;
	public Transform DraggablePanelList;
	
	public LocalButtonCallBack Button_PageUp;
	public LocalButtonCallBack Button_PageDown;
	public GameObject Tip_PageUp;

	private Dictionary<int, ChatWindowItem> m_privateWindowDict = new Dictionary<int, ChatWindowItem>();
	private Dictionary<int, PrivateTalkerItem> m_privateTalkerDict = new Dictionary<int, PrivateTalkerItem>();
	private WindowType m_curWindowType;
	
	private int m_PageNum = 0;
	private int m_CurTalkerIndex = 0;
	const float SPACING_TITLE_BUTTON = 100f;
	const int MAXNUM_TITLE_ONEPAGE = 5;
	#endregion
	
	
	void Awake()
	{      
		Button_Send.SetCallBackFuntion(SendChat, null);
		
		//TODO GuideBtnManager.Instance.RegGuideButton(Button_OpenWorldPanel.gameObject, UIType.Chat, SubType.ButtomCommon, out m_guideBtnID);
		
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
		
		Button_PageUp.SetCallBackFuntion(OnPageUpClick, null);
		Button_PageDown.SetCallBackFuntion(OnPageDownClick, null);
		
		TitleInitialPos = TitlePoint.transform.localPosition;

		//监听
		UIEventManager.Instance.RegisterUIEvent(UIEventType.PrivateChatMsg, ReceiveChatMsgHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenPrivateChatWindow, OpenPrivateChatWindowHandle);

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

	//切换到第一个窗口
	public void SwitchingFirstWindow()
	{
		List<int> list = m_privateTalkerDict.Keys.ToList();
		int id = 0;
		if(list.Count>0)
		{
			id = list[0];
		}
		SwitchingPrivateWindow(id,"");
	}

	//切换私聊窗口
	public void SwitchingPrivateWindow(int chaterID, string chaterName)
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
		if(transform.localPosition == Vector3.zero && m_curWindowType == WindowType.Private)
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ClosePrivateMessageTip,null);
		}

		Button_Send.gameObject.SetActive(true);
		Input_Chat.gameObject.SetActive(true);
		
		UpdatePageButtons(false);
	}
	
	

	//私聊标题按钮更新
	public void UpdateTitleNames()
	{
		m_privateTalkerDict.ApplyAllItem(p=>p.Value.gameObject.SetActive(true));
	}
	

	public void Show()
	{
		transform.localPosition = Vector3.zero;
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
	
//	public override void Close()
//	{
//		//PrivateChatWindowMgr.ClosePanel();
//		
//		CloseChatPanel();
//		//base.Close();
//	}
	public void Close()
	{
		transform.localPosition = Vector3.back * 1000;
	}
	
	private void ShowChatPanel()
	{
//		ChatPanel.transform.localPosition = Vector3.zero;
//		if (NewChat_prompt.activeInHierarchy)
//		{
//			NewChat_prompt.SetActive(false);
//		}
	}
	
	private void CloseChatPanel()
	{
		transform.localPosition = Vector3.back * 800;
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

		if(iTalkToWorldID == 0)
		{
			SMsgChat_SC sMsgChat = new SMsgChat_SC();
			sMsgChat.SenderName = playerData.Name;
			sMsgChat.Chat = chat;
			sMsgChat.L_LabelChat = ChatRecordManager.Instance.ParseColorChat(Chat.WindowType.Private,sMsgChat);
			sMsgChat.L_Channel = (int)Chat.WindowType.Private;
			ReceiveChatMsgHandle(sMsgChat);
		}
		else
		{
			NetServiceManager.Instance.ChatService.SendChat((uint)playerData.ActorID, iTalkToWorldID, accpecterName, chat,0, chatDefine);
		}
		
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
		if(transform.localPosition != Vector3.zero || sMsgChat_SC.L_ChaterID != iTalkToWorldID)
		{
			m_privateTalkerDict[sMsgChat_SC.L_ChaterID].ShowNewTip();
		}
		
		//关闭私信提醒
		//		if(ChatPanel.transform.localPosition == Vector3.zero && !m_privateTalkerDict.Values.Any(p=>p.IsHasNewMessage))
		//		{
		//			UIEventManager.Instance.TriggerUIEvent(UIEventType.ClosePrivateMessageTip,null);
		//		}
		if(transform.localPosition == Vector3.zero && m_curWindowType == WindowType.Private)
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ClosePrivateMessageTip,null);
		}
		else
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowPrivateMessageTip,null);
		}
	}
	void CloseUIHandle(object obj)
	{
		this.CloseChatPanel();
	}
	
	void OpenPrivateChatWindowHandle(object obj)
	{
		TalkerInfo talkerInfo = (TalkerInfo)obj;
		//TraceUtil.Log("[OpenPrivateChatWindowHandle talkerInfo]" + talkerInfo.ActorID + " , " + talkerInfo.Name);
		//PrivateChatWindowMgr.OpenPrivateWindow(talkerInfo.ActorID, talkerInfo.Name);
		UpdateTitleNames();
		SwitchingPrivateWindow(talkerInfo.ActorID, talkerInfo.Name);
		Show();
	}	
	
//	protected override void RegisterEventHandler()
//	{
//		this.AddEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccessHandle);
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
	
	void OnDestroy()
	{
		//this.RemoveEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccessHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.WorldChatMsg, ShowWorldChatHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.PrivateChatMsg, ReceiveChatMsgHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseAllUI, CloseUIHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenPrivateChatWindow, OpenPrivateChatWindowHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseWorldChatWindow, CloseWorldPanelHandle);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShopsBuySuccess, OnBuySuccess);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, CloseWorldPanelHandle);
		//TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
	}
}
