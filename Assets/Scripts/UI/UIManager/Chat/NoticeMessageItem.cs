using UnityEngine;
using System.Collections;
using Chat;
using System;


public class NoticeMessageItem : MonoBehaviour 
{
	public UIPanel Panel_Bg;
	public ChatInfoItemControl ChatItemControl;
	public UILabel Label_chat;

	private Vector3 m_LabelPos;
	private Vector3 m_endPos;
	private bool m_isMoving = false;

	const float MOVE_SPEED = 150f;
	const string ISVIP = "             ";
	const string ISNOTVIP = "          ";

	public void Show(SMsgChat_SC sChat)
	{
		//label
		string chatContent = "";
		if(sChat.bySenderVipLevel > 0)
		{
			chatContent = ISVIP + sChat.L_LabelChat;
		}
		else
		{
			chatContent = ISNOTVIP+ sChat.L_LabelChat;
		}
		ChatItemControl.Init(sChat.L_Channel, sChat.senderActorID, sChat.SenderName, sChat.bySenderVipLevel, chatContent, sChat.bChatType, sChat.friendTeamID,null);
		ChatItemControl.Swith_CannelIcon.gameObject.SetActive(false);
		m_LabelPos = ChatItemControl.transform.localPosition;
		//TweenPosition.Begin(ChatItemControl.gameObject, 0.2f, m_LabelPos,m_LabelPos+Vector3.left * 1000)
		Panel_Bg.alpha = 0;
		TweenAlpha.Begin(Panel_Bg.gameObject,0.2f,0,1f,(obj)=>{m_isMoving = true;});

		m_endPos = m_LabelPos + Vector3.left * (Label_chat.relativeSize.x * Label_chat.transform.localScale.x + 800);

//		Debug.Log("==>"+Label_chat.border);
//		Debug.Log("==>"+Label_chat.lineWidth);
//		Debug.Log("==>"+Label_chat.pivotOffset);
//		Debug.Log("==>"+Label_chat.relativePadding);
//		Debug.Log("==>"+Label_chat.relativeSize);

	}

	void FixedUpdate()
	{
		if(m_isMoving)
		{
			m_LabelPos += Vector3.left * MOVE_SPEED*Time.fixedDeltaTime;
			ChatItemControl.transform.localPosition = m_LabelPos;

			if(m_LabelPos.x < m_endPos.x)
			{
				m_isMoving = false;
				UI.GoodsMessageManager.Instance.ClearCurrentNoticeMessage();
				Destroy(gameObject);
			}	
		}
	}
}
