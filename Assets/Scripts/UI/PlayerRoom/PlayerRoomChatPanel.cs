using UnityEngine;
using System.Collections;
using Chat;
namespace UI.PlayerRoom
{
    public class PlayerRoomChatPanel : MonoBehaviour
    {
        public UIInput Input_box;
        public SingleButtonCallBack Button_Send;
        public UIDraggablePanel DraggablePanel;
        public UITable Table;

        public ChatLabelItem ChatLabelItemPrefab;

        void Awake()
        {
            Button_Send.SetCallBackFuntion(SendHandle, null);
        }

        public void ShowPanel()
        {
            if (transform.localPosition == Vector3.zero)
            {
                HidePanel();
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }            
        }

        public void HidePanel()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }

        void SendHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            string chat = Input_box.text;
            if (chat == "")
            {
                return;
            }
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            NetServiceManager.Instance.ChatService.SendChat((uint)playerData.ActorID, 0, chat, 0, Chat.ChatDefine.MSG_CHAT_CURRENT);

            Input_box.text = "";
        }

        public void CreateChat(SMsgChat_SC sMsgChat_SC)
        {
            string chatContent = "";
            chatContent = ChatPanelUIManager.ColoringChannel("[房间]") + ChatPanelUIManager.ColoringName(sMsgChat_SC.SenderName + " : ") + sMsgChat_SC.Chat;
            GameObject chat = (GameObject)Instantiate(ChatLabelItemPrefab.gameObject);
            chat.transform.parent = Table.transform;
            chat.transform.localScale = Vector3.one;
            chat.transform.localPosition = Vector3.zero;
            var chatControl = chat.GetComponent<ChatLabelItem>();
            bool isMyChat = sMsgChat_SC.senderActorID == PlayerManager.Instance.FindHeroDataModel().ActorID;
            chatControl.Init(isMyChat, chatContent);

            Table.Reposition();
            StartCoroutine("SetDragAmount");
        }
        IEnumerator SetDragAmount()
        {
            yield return new WaitForEndOfFrame();
            if (DraggablePanel.shouldMove)
            {
                DraggablePanel.SetDragAmount(0, 1, false);
            }
        }
    }
}