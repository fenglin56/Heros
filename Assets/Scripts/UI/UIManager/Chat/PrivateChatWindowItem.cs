using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chat
{
    /// <summary>
    /// 私人聊天 窗口控制类
    /// </summary>
    public class PrivateChatWindowItem : MonoBehaviour
    {        
        public ChatLabelItem ChatLabelItem;
        public UIDraggablePanel DraggablePanel;        
        public UITable Table;
        public BoxCollider BoxCollider_Bg;

        private int m_talkerActorID;

        private List<ChatLabelItem> ChatRecordList = new List<ChatLabelItem>();

        public void CreateChatItem(int actorID, bool isMyChat, string str)
        {
            this.m_talkerActorID = actorID;
            GameObject item = (GameObject)Instantiate(ChatLabelItem.gameObject);
            ChatLabelItem chatLabelItem = item.GetComponent<ChatLabelItem>();
            chatLabelItem.Init(isMyChat, str);
            AddChatRecord(chatLabelItem);            
            item.transform.parent = Table.transform;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;            
            Table.Reposition();
            
            if (DraggablePanel.panel.enabled)
            {
                StartCoroutine("SetDragAmount");
            }            
        }

        IEnumerator SetDragAmount()
        {
            yield return new WaitForEndOfFrame();
            if (DraggablePanel.shouldMove)
            {
                DraggablePanel.SetDragAmount(0, 1, false);
            }
        }

        /// <summary>
        /// 重设位置
        /// </summary>
        public void Reposition()
        {
            if (DraggablePanel.shouldMove)
            {
                DraggablePanel.SetDragAmount(0, 1, false);
            }
        }

        private void AddChatRecord(ChatLabelItem item)
        {
            if (ChatRecordList.Count >= 100)
            {
                ChatRecordList[0].DestroySelf();
                ChatRecordList.RemoveAt(0);
            }
            ChatRecordList.Add(item);
        }


        public void SetEnable(bool flag)
        {
            DraggablePanel.panel.enabled = flag;
            BoxCollider_Bg.enabled = flag;

            if (flag == true)
            {
                StartCoroutine("SetDragAmount");
            }
        }
        
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
