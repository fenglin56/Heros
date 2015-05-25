using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Login
{

    public class SingleServerListPanel : MonoBehaviour
    {

        public enum ServerStatus
        {
            Normal = 0, Hot, New, Maintain,
        }

        public Transform BtnListParent;
        public SingleButtonCallBack[] ServerBtnList;
        public SingleButtonCallBack CloseBtn;
        public SingleButtonCallBack LastPageBtn;
        public SingleButtonCallBack NextPageBtn;

        public List<Server> m_serverInfo { get; private set; }
        public ServerListPanel_V3 MyParent { get; private set; }

        private Server SelectServerData;//选中的服务器
        private ServerDitionary serverDitionary = new ServerDitionary();
        private int PageNumber = 1;

        void Awake()
        {
            gameObject.SetActive(false);
            CloseBtn.SetCallBackFuntion(Close);
            LastPageBtn.SetCallBackFuntion(OnLastPageBtnClick);
            NextPageBtn.SetCallBackFuntion(OnNextPageBtnClick);
        }

        public void Show(List<Server> serverInfo, ServerListPanel_V3 myParent)
        {
            gameObject.SetActive(true);
            MyParent = myParent;
            if (m_serverInfo == null) { m_serverInfo = new List<Server>(); } else { m_serverInfo.Clear(); }
            foreach (var child in serverInfo)
            {
                m_serverInfo.Add(child);
            }
            m_serverInfo.Sort(delegate(Server a, Server b) { return (a.No).CompareTo(b.No); });
            m_serverInfo.Reverse();
            serverDitionary.PageServerList.Clear();
            for (int i = 0; i < m_serverInfo.Count; i++)
            {
                serverDitionary.AddSeverInfo(m_serverInfo[i]);
            }
            PageNumber = 1;
            UpdateButtonStatus();
            UpdateServerListPanel();
        }

        void OnNextPageBtnClick(object obj)
        {
            if (PageNumber < serverDitionary.PageServerList.Count)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
                PageNumber++;
                UpdateServerListPanel();
            }
            UpdateButtonStatus();
        }

        void OnLastPageBtnClick(object obj)
        {
            if (PageNumber > 1)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
                PageNumber--;
                UpdateServerListPanel();
            }
            UpdateButtonStatus();
        }

        void UpdateButtonStatus()
        {
            SetButtonActive(LastPageBtn, PageNumber < 2 ? false : true);
            SetButtonActive(NextPageBtn, PageNumber >= serverDitionary.PageServerList.Count ? false : true);
        }

        void SetButtonActive(SingleButtonCallBack button, bool Flag)
        {
            button.BackgroundSprite.alpha = Flag ? 1 : 0.5f;
            button.collider.enabled = Flag;
        }

        void UpdateServerListPanel()
        {
            List<Server> currentServerList = serverDitionary.PageServerList[PageNumber];
            for (int i = 0; i < ServerBtnList.Length; i++)
            {
                var serverBtn = ServerBtnList[i];
                if (currentServerList.Count > i)
                {
                    serverBtn.SetCallBackFuntion(OnServerBtnClick, currentServerList[i]);
                    serverBtn.gameObject.SetActive(true);
                    serverBtn.SetButtonText(currentServerList[i].Name);
                    ServerStatus currentStatus = ServerStatus.Normal;
                    if(currentServerList[i].Status == 0){currentStatus = ServerStatus.Maintain;}
                    else if (currentServerList[i].Status == 2) { currentStatus = ServerStatus.Hot; }
                    else if (currentServerList[i].Recommend_status == 1) { currentStatus = ServerStatus.New; }
                    else if (currentServerList[i].Status == 1) { currentStatus = ServerStatus.Normal; }
                    SetServerBtnStatus(serverBtn, currentServerList[i] == SelectServerData, currentStatus);
                    //serverBtn.SetButtonBackground(currentServerList[i].Recommend_status == 1 ? 2 : (currentServerList[i].Status ==2?1:0));
                }
                else
                {
                    serverBtn.gameObject.SetActive(false);
                }
            }
        }

        void SetServerBtnStatus(SingleButtonCallBack serverBtn, bool isSelect, ServerStatus serverStatus)
        {
            serverBtn.BackgroundSprite.gameObject.SetActive(isSelect);
            serverBtn.SetButtonBackground((int)serverStatus);
            serverBtn.textLabel.color = serverStatus == ServerStatus.Maintain ? Color.gray : Color.white;
            serverBtn.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(serverStatus == ServerStatus.Maintain?2:1));
        }

        void OnServerBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            this.SelectServerData = obj as Server;
            MyParent.OnSelectServer(SelectServerData);

            var serverBtn = ServerBtnList.First(P => P.ButtonCallBackInfo == obj);
            ServerBtnList.ApplyAllItem(P=>P.BackgroundSprite.gameObject.SetActive(P.ButtonCallBackInfo == obj));
            DoForTime.DoFunForFrame(3,Close,null);
        }


        void Close(object obj)
        {
            gameObject.SetActive(false);
        }

        public class ServerDitionary
        {
            public Dictionary<int, List<Server>> PageServerList;

            public ServerDitionary()
            {
                PageServerList = new Dictionary<int, List<Server>>();
            }

            public void AddSeverInfo(Server server)
            {
                for (int i = 1; i < 100; i++)
                {
                    List<Server> child = null;
                    if (PageServerList.TryGetValue(i, out child))
                    {
                        if (child.Count < 5)
                        {
                            child.Add(server);
                            return;
                        }
                    }
                    else
                    {
                        child = new List<Server>();
                        child.Add(server);
                        PageServerList.Add(i, child);
                        return;
                    }
                }
            }

        }

        [ContextMenu("InitBtn")]
        void InitBtnList()
        {
            ServerBtnList = BtnListParent.GetComponentsInChildren<SingleButtonCallBack>();
        }

    }
}