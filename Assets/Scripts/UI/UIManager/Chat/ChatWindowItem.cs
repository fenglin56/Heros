using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI;
namespace Chat
{
	public class ChatWindowItem : MonoBehaviour 
	{
		public GameObject ChatInfoItemPrefab;
		public UIDraggablePanel DraggablePanel;
		public UITable Table;

		public GameObject New_Tip;

		const string ISVIP = "             ";
		const string ISNOTVIP = "          ";

		private SelectedChatTargetDelegate m_Delegate;
		private List<ChatInfoItemControl> m_itemList = new List<ChatInfoItemControl>();

		void Awake()
		{
			DraggablePanel.ConstraintCallBack = Drag;
		}
		void Drag(Vector3 v3)
		{
			if(New_Tip.activeInHierarchy)
			{
				New_Tip.SetActive(false);
			}
		}
		public void Show()
		{
			transform.localPosition = Vector3.zero;
		}

		public void Close()
		{
			transform.localPosition = Vector3.back * 800;
		}

		public void ClearChats()
		{
			for(int i= 0;i<m_itemList.Count;i++)
			{
				Destroy(m_itemList[i].gameObject);
			}
			m_itemList.Clear();
		}

		public void InitWindow(List<SMsgChat_SC> list, SelectedChatTargetDelegate callBack)
		{
			m_Delegate = callBack;

			list.ApplyAllItem(p=>{
				this.CreateChatItem(p);
			});
			Table.Reposition();
			StartCoroutine("SetDragAmount");
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

			GameObject chat = CreatObjectToNGUI.InstantiateObj(ChatInfoItemPrefab,Table.transform);
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

}