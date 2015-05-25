using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chat
{
    /// <summary>
    /// 创建窗口类型
    /// </summary>
    public enum CreateWindowType
    {
        Initiative,//创建并显示
        Passive,//创建但不显示
    }

    /// <summary>
    /// 私人聊天 窗口管理类
    /// </summary>
    public class PrivateChatWindowManager : MonoBehaviour
    {
        public Transform Child;
        public LocalButtonCallBack Button_Open;
        //tab
        public PrivateTalkerItem TalkerItemPrefab;
        public UIDraggablePanel DraggablePanel_Navigation;
        public UIGrid Grid_Navigation;
        //window
        public PrivateChatWindowItem PrivateChatWindowItemPrefab;        
        public Transform WindowManager;

        //local logic
        public SingleButtonCallBack Button_Send;
        public SingleButtonCallBack Button_Close;
        public UIInput Input_Chat;        

        private Dictionary<int, ChatWindowClass> m_windowDict = new Dictionary<int, ChatWindowClass>();
        private int m_CurActorID = 0;   //当前显示的窗口id

        private int m_myActorID = 0;
        private int m_guideBtnID = 0;

        void Awake()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.PrivateChatMsg, ReceiveChatMsgHandle);//监听私聊信息            

            Button_Send.SetCallBackFuntion(SendMsg, null);
            Button_Close.SetCallBackFuntion(CloseWindow, null);
            Button_Open.SetCallBackFuntion(OpenPrivateChatPanel, null);

            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Open.gameObject, UI.MainUI.UIType.Chat, SubType.ButtomCommon, out m_guideBtnID);

            if (ChatRecordManager.Instance.IsHasNewPrivateSmg())
            {
                Button_Open.gameObject.SetActive(true);
            }
            else
            {
                Button_Open.gameObject.SetActive(false);
            }

            InitLastChat();
        }
        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.PrivateChatMsg, ReceiveChatMsgHandle);//移除监听私聊信息
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        public void ClosePanel()
        {
            Child.transform.localPosition = new Vector3(-2000, 0, 0);
        }

        private void OpenPanel()
        {
            Child.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="targetActorID"></param>
        /// <param name="talkerName"></param>
        public void OpenPrivateWindow(int targetActorID, string talkerName)
        {
            //if (m_windowDict.Count == 0)//当前没有聊天窗口开启
            //{
            //    //Child.transform.localPosition = Vector3.zero;
                
            //}

            //如果不是当前窗口
            if (this.m_CurActorID != targetActorID)
            {
                CloseChatWindow();
                this.m_CurActorID = targetActorID;
                var chatWindowClass = this.RecallWindow(targetActorID, talkerName);
                //chatWindowClass.TalkerItem.BeSelect();
                chatWindowClass.PrivateChatWindowItem.SetEnable(true);                               
            }

            this.OpenPanel();
            //显示开关按钮
            //Button_Open.gameObject.SetActive(true);
        }

        //初始化上次聊天记录
        private void InitLastChat()
        {
            var recordDict = ChatRecordManager.Instance.GetPrivateChatRecordDict();
            recordDict.ApplyAllItem(p =>
                {
                    this.CreateChatMsg(p.Key, p.Value);
                });            
        }

        //显示窗口
        private void ShowWindow(int targetActorID)
        {
            //m_windowDict[targetActorID].TalkerItem.BeSelect();
            m_windowDict[targetActorID].PrivateChatWindowItem.SetEnable(true);
        }

        //调出窗口
        private ChatWindowClass RecallWindow(int targetActorID, string targetName)
        {            
            if (!m_windowDict.ContainsKey(targetActorID))//不存在聊天窗口
            {
                //create tab
                GameObject talkerItem = (GameObject)Instantiate(TalkerItemPrefab.gameObject);
                PrivateTalkerItem talkerItemScript = talkerItem.GetComponent<PrivateTalkerItem>();
                talkerItem.transform.parent = Grid_Navigation.transform;
                talkerItemScript.transform.localScale = Vector3.one;    
				talkerItem.transform.localPosition = Vector3.forward * -2;
                talkerItemScript.Init(targetActorID, targetName);
                Grid_Navigation.Reposition();
                //create window
                GameObject windowItem = (GameObject)Instantiate(PrivateChatWindowItemPrefab.gameObject);
                PrivateChatWindowItem windowItemScript = windowItem.GetComponent<PrivateChatWindowItem>();
                windowItemScript.SetEnable(false);
                windowItem.transform.parent = WindowManager;
                windowItem.transform.localPosition = Vector3.zero;
                windowItem.transform.localScale = Vector3.one;
                m_windowDict.Add(targetActorID, new ChatWindowClass() { ActorID = targetActorID, TalkerItem = talkerItemScript, PrivateChatWindowItem = windowItemScript });
            }            
            return m_windowDict[targetActorID];
        }

        //上发聊天信息
        void SendMsg(object obj)
        {
            string chat = Input_Chat.text;
            if (chat == "")
            {
                return;
            }
            if (m_myActorID == 0)
            {
                m_myActorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
            }
            NetServiceManager.Instance.ChatService.SendChat((uint)m_myActorID, (uint)m_CurActorID, chat,0, ChatDefine.MSG_CHAT_PRIVATE);
            Input_Chat.text = "";
        }

        //接收处理聊天信息
        void ReceiveChatMsgHandle(object obj)
        {
            SMsgChat_SC sMsgChat_SC = (SMsgChat_SC)obj;
            this.CreateChatMsg(sMsgChat_SC);
            //新消息提示
            if (Child.transform.localPosition != Vector3.zero)
            {
                Button_Open.gameObject.SetActive(true);
                SoundManager.Instance.PlaySoundEffect("Sound_Voice_ChatMsg");
            }
            else
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Msg_Chat");
            }            
        }
        //创建聊天记录
        private void CreateChatMsg(SMsgChat_SC sMsgChat_SC)
        {
            if (m_myActorID == 0)
            {
                m_myActorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
            }

            int talkerID = 0;
            if (sMsgChat_SC.senderActorID == m_myActorID)
            {
                talkerID = sMsgChat_SC.accepterActorID;
            }
            else
            {
                talkerID = sMsgChat_SC.senderActorID;
            }

            //调出窗口
            var chatWindowClass = this.RecallWindow(talkerID, sMsgChat_SC.SenderName);
            if (this.m_CurActorID != talkerID)
            {
                //闪烁提示
                //chatWindowClass.TalkerItem.StartFlashing();
            }            
            //添加聊天信息
            string chatContent = "";
            bool isMyChat = sMsgChat_SC.senderActorID == m_myActorID;
            chatContent = ChatPanelUIManager.ColoringChannel("[私人]") + ChatPanelUIManager.ColoringName(sMsgChat_SC.SenderName + " : ") + sMsgChat_SC.Chat;
            chatWindowClass.PrivateChatWindowItem.CreateChatItem(talkerID,isMyChat, chatContent);
        }
        //创建之前聊天记录
        private void CreateChatMsg(int talkerID, List<SMsgChat_SC> chatList)
        {
            var chatWindowClass = this.RecallWindow(talkerID, chatList.First().SenderName);
            chatList.ApplyAllItem(p =>
                {
                    string chatContent = "";
                    bool isMyChat = p.senderActorID == m_myActorID;
                    chatContent = ChatPanelUIManager.ColoringChannel("[私人]") + ChatPanelUIManager.ColoringName(p.SenderName + " : ") + p.Chat;                   
                    chatWindowClass.PrivateChatWindowItem.CreateChatItem(talkerID, isMyChat, chatContent);
                });
        }


        //关闭窗口
        void CloseWindow(object obj)
        {
            int closeWindowID = m_CurActorID;
            m_windowDict[closeWindowID].Delete();

            if (m_windowDict.Count <= 1)
            {
                //Child.gameObject.SetActive(false);
                this.ClosePanel();
                m_CurActorID = 0;//当前窗口id置零
                //关闭开关按钮
                Button_Open.gameObject.SetActive(false);
            }
            else
            {
                var actorList = m_windowDict.Keys.ToList();
                for (int i = 0; i < actorList.Count; i++)
                {
                    if (actorList[i] == closeWindowID)
                    {
                        if (i == 0)
                        {
                            m_CurActorID = actorList[i + 1];
                        }
                        else
                        {
                            m_CurActorID = actorList[i - 1];
                        }
                        break;
                    }
                }
                this.ShowWindow(m_CurActorID);    //调出窗口
                StartCoroutine("NavigationRepositionLater");//重新排版
            }            
            m_windowDict.Remove(closeWindowID);//此时安全移除旧窗口            
        }

        IEnumerator NavigationRepositionLater()
        {
            yield return new WaitForEndOfFrame();
            m_windowDict.First().Value.TalkerItem.transform.position = Vector3.zero;
            Grid_Navigation.Reposition();
            //DraggablePanel_Navigation.ResetPosition();
        }

        //打开私人聊天面板
        void OpenPrivateChatPanel(object obj)
        {
            if (Child.transform.localPosition == Vector3.zero)
            {
                ClosePanel();
            }
            else
            {
                OpenPanel();
                if (m_CurActorID == 0)
                {
                    OpenPrivateWindow(m_windowDict.Values.First().ActorID, null);
                }
                Button_Open.gameObject.SetActive(false);
            }
        }

        private void CloseChatWindow()
        {
            m_windowDict.Values.ApplyAllItem(p =>
                {
                   // p.TalkerItem.ReSetStatus();
                    p.PrivateChatWindowItem.SetEnable(false);
                });
        }
    }
}