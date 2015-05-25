using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Chat
{
	public class ChatButtonManager : View
	{
		public GameObject NewMessageTip;
		public GameObject UnreadTip;
		public LocalButtonCallBack Button_Click;

		private bool m_isShow = false;

		private Vector3 ShowPos = new Vector3(-170,-276,500);
		private Vector3 HidePos = new Vector3(-470,-276,500);

		private List<ParticleSystem> particleList = new  List<ParticleSystem>();

		private bool m_isEnable = true;
		private UIPanel m_Panel_Button;

		void Awake()
		{
			if (ChatRecordManager.Instance.IsHasNewPrivateSmg())
			{
				NewMessageTip.SetActive(true);
			}
			else
			{
				NewMessageTip.SetActive(false);
			}

			transform.localPosition = ShowPos;
			m_Panel_Button = GetComponent<UIPanel>();

			Button_Click.SetCallBackFuntion(OnButtonClick,null);

			StartCoroutine("SetHidePos");
			//默认开启聊天 预览窗口
			UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Chat,Chat.WindowType.Init);

			particleList.AddRange(Button_Click.GetComponentsInChildren<ParticleSystem>(true));

			RegisterEventHandler();
		}

		IEnumerator SetHidePos()
		{
			yield return new WaitForSeconds(1f);
			Camera topLeft = PopupObjManager.Instance.UICamera;
			Vector3 topLeftPoint = topLeft.transform.FindChild("Anchor_TopLeft").localPosition;
			HidePos = new Vector3(topLeftPoint.x + CommonDefineManager.Instance.CommonDefine.TownstartPoint1.BasePostion.x, -276, 100);
		}

		void ReceiveChatMsgHandle(object obj)
		{
			m_isShow = true;
			StartCoroutine(LateShowNewMessageTip());
		}

		IEnumerator LateShowNewMessageTip()
		{
			yield return new WaitForEndOfFrame();
			if(m_isShow)
			{
				NewMessageTip.SetActive(true);
			}
		}

		void CloseMessageTipHandle(object obj)
		{
			m_isShow = false;
			NewMessageTip.SetActive(false);
		}

		void OnClick()
		{
			if(!m_isEnable)
				return;
			UnreadTip.SetActive(false);
			m_Panel_Button.enabled = false;
			particleList.ApplyAllItem(p=>p.gameObject.SetActive(false));
			UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Chat);
		}

		void OnButtonClick(object obj)
		{
			if(!m_isEnable)

				return;
			CloseMessageTipHandle(null);
			UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Chat,Chat.WindowType.Private);
		}

		void  MainBtnOpenEventHandle(object obj)
		{
			TweenPosition.Begin(gameObject,0.16f,transform.localPosition,HidePos,null);
			TweenAlpha.Begin(gameObject,0.16f,0);
			m_isEnable = false;
		}

		void MainBtnCloseEventHandle(object obj)
		{
			TweenPosition.Begin(gameObject,0.16f,transform.localPosition,ShowPos,null);
			TweenAlpha.Begin(gameObject,0.16f,1f);
			m_isEnable = true;
			m_Panel_Button.enabled = true;
			particleList.ApplyAllItem(p=>p.gameObject.SetActive(true));
		}

		void ReceiveWorldChatHandle(object obj)
		{
			SMsgChat_SC sMsgChat_SC = (SMsgChat_SC)obj;

			if(sMsgChat_SC.L_Channel == (int)WindowType.Town || sMsgChat_SC.L_Channel == (int)WindowType.System || sMsgChat_SC.L_Channel == (int)WindowType.World)
			{
				if(ChatRecordManager.Instance.CurTownDisplayType == DisplayType.preview)
				{
					UnreadTip.SetActive(true);
				}
			}
		}
		void OpenWorldChatWindowHandle(object obj)
		{
			m_Panel_Button.enabled = true;
			particleList.ApplyAllItem(p=>p.gameObject.SetActive(true));
		}

		void ShowChatButtonHandle(object obj)
		{
			m_Panel_Button.enabled = true;
			particleList.ApplyAllItem(p=>p.gameObject.SetActive(true));
		}
		void OnNpcTalkDealUI(object obj)
		{
			bool isShow = (bool)obj;
			if (isShow) {
				gameObject.SetActive(true);
			} else {
				gameObject.SetActive(false);
			}
		}
		void OnOpenTalkUIEvent(object obj)
		{
			gameObject.SetActive(false);
		}
		void OnCloseTalkUIEvent(object obj)
		{
			gameObject.SetActive(true);
		}
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.WorldChatMsg, ReceiveWorldChatHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShowPrivateMessageTip, ReceiveChatMsgHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ClosePrivateMessageTip, CloseMessageTipHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.MainBtnOpenEvent,MainBtnOpenEventHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.MainBtnCloseEvent,MainBtnCloseEventHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
			UIEventManager.Instance.RemoveUIEventHandel( UIEventType.ShowChatButton, ShowChatButtonHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NpcTalkTaskDealUI, OnNpcTalkDealUI);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcTalkOpenEvent, OnOpenTalkUIEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcTalkCloseEvent, OnCloseTalkUIEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcGuideStartEvent, OnOpenTalkUIEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcGuideStopEvent, OnCloseTalkUIEvent);
		}
		protected override void RegisterEventHandler ()
		{	
			UIEventManager.Instance.RegisterUIEvent(UIEventType.WorldChatMsg, ReceiveWorldChatHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ShowPrivateMessageTip, ReceiveChatMsgHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ClosePrivateMessageTip, CloseMessageTipHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.MainBtnOpenEvent,MainBtnOpenEventHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.MainBtnCloseEvent,MainBtnCloseEventHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
			UIEventManager.Instance.RegisterUIEvent( UIEventType.ShowChatButton, ShowChatButtonHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.NpcTalkTaskDealUI, OnNpcTalkDealUI);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcTalkOpenEvent, OnOpenTalkUIEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcTalkCloseEvent, OnCloseTalkUIEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcGuideStartEvent, OnOpenTalkUIEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcGuideStopEvent, OnCloseTalkUIEvent);
		}
	}
}