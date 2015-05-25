using UnityEngine;
using System.Collections;
namespace Chat
{
    /// <summary>
    /// 私人聊天 窗口信息类
    /// </summary>
    public class ChatWindowClass
    {
        public int ActorID;
        public PrivateTalkerItem TalkerItem;//对话人信息
        public PrivateChatWindowItem PrivateChatWindowItem;//窗口信息

        public void Delete()
        {
            TalkerItem.DestroySelf();
            PrivateChatWindowItem.DestroySelf();
        }
    }
}
