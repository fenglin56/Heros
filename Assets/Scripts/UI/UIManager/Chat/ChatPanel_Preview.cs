using UnityEngine;
using System.Collections;
using UI;
using Chat;
using System.Collections.Generic;

public class ChatPanel_Preview : MonoBehaviour 
{
	public Vector3 showPos ;
	public GameObject ChatInfoItem_Preview;

	public Transform ScalePointTrans;

	public UIDraggablePanel DraggablePanel;
	public UITable Table;

	const string ISVIP = "             ";
	const string ISNOTVIP = "          ";

	private List<ChatInfoItemControl> m_itemList = new List<ChatInfoItemControl>();

	private SelectedChatTargetDelegate m_Delegate;

	void Awake()
	{
		UIEventManager.Instance.RegisterUIEvent(UIEventType.WorldChatMsg, ShowWorldChatHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.MainBtnOpenEvent, ShrinkHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.MainBtnCloseEvent, StretchHandle);
	}
	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.WorldChatMsg, ShowWorldChatHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.MainBtnOpenEvent, ShrinkHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.MainBtnCloseEvent, StretchHandle);
	}

	void ShowWorldChatHandle(object obj)
	{
		SMsgChat_SC sMsgChat_SC = (SMsgChat_SC)obj;
		if(sMsgChat_SC.L_Channel == (int)WindowType.Town || sMsgChat_SC.L_Channel == (int)WindowType.World || sMsgChat_SC.L_Channel == (int)WindowType.System)
		{
			AddChat(sMsgChat_SC);
		}
	}

	public void Show()
	{
		transform.localPosition = showPos;//Vector3.zero;
	}

	public void Close()
	{
		transform.localPosition = Vector3.back * 800;
	}

	void ShrinkHandle(object obj)
	{
		TweenScale.Begin(ScalePointTrans.gameObject,0.16f,ScalePointTrans.localScale,new Vector3(0.001f,1,1),ShrinkOver);
	}
	void StretchHandle(object obj)
	{
		GetComponent<UIPanel>().enabled = true;
		TweenScale.Begin(ScalePointTrans.gameObject,0.16f,ScalePointTrans.localScale,Vector3.one,StretchOver);
	}

	void ShrinkOver(object obj)
	{
		GetComponent<UIPanel>().enabled = false;
	}

	void StretchOver(object obj)
	{
	}

	public void AddChat(SMsgChat_SC sChat)
	{
		this.CreateChatItem(sChat);
		Table.Reposition();
		StartCoroutine("SetDragAmount");
	}
	
	IEnumerator SetDragAmount()
	{
		yield return new WaitForEndOfFrame();
		if (DraggablePanel.shouldMove)
		{
			DraggablePanel.SetDragAmount(0, 1, false);
			//New_Tip.SetActive(true);
		}
	}
	
	private void CreateChatItem(SMsgChat_SC sChat)
	{
		string chatContent = "";
		if(sChat.bySenderVipLevel > 0)
		{
			chatContent = ISVIP + sChat.L_LabelChat;
		}
		else
		{
			chatContent = ISNOTVIP+ sChat.L_LabelChat;
		}
		
		//////
		/// 判断颜色 vip
		/// 
		
		//			chatContent = NGUIColor.SetTxtColor(sChat.SenderName + " : ", TextColor.ChatBlue)
		//				+ NGUIColor.SetTxtColor(sChat.Chat,  TextColor.white);
		
		GameObject chat = CreatObjectToNGUI.InstantiateObj(ChatInfoItem_Preview,Table.transform);
		chat.transform.localPosition += Vector3.back * 5;
		var chatControl = chat.GetComponent<ChatInfoItemControl>();
		chatControl.Init(sChat.L_Channel, sChat.senderActorID, sChat.SenderName, sChat.bySenderVipLevel, chatContent, sChat.bChatType, sChat.friendTeamID, m_Delegate);
		m_itemList.Add(chatControl);
		
		
		if(m_itemList.Count > 100)
		{
			var item = m_itemList[0];
			m_itemList.RemoveAt(0);
			Destroy(item.gameObject);
		}		
	}

}
