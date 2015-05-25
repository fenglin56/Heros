using UnityEngine;
using System.Collections;

namespace Chat
{
    public class ChatLabelItem : MonoBehaviour
    {
        public UILabel Label_Chat;

        public void Init(bool isMyChat, string chat)
        {
            Label_Chat.text = chat;
            if (isMyChat)
            {
                Label_Chat.color = ChatPanelUIManager.ColorMy;
            }
            else
            {
                Label_Chat.color = ChatPanelUIManager.ColorOther;
            }
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
