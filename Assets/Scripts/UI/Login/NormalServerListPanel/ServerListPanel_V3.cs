using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;

namespace UI.Login
{
	/// <summary>
	/// 服务器列表，挂在LoginServerListV3预设件上
	/// </summary>
    public class ServerListPanel_V3 : IUIPanel
    {
		public VersionConfigDatabase VersionConfigDB;
        public SingleServerList[] singleServerList;
        public SingleServerListPanel SingleSeverListPanel;

        public UILabel LoginTipsLabel;
        public SingleButtonCallBack SelectServerBtn;
        public SingleButtonCallBack JoinGameBtn;
        public SingleCommonUIBottomButton GoBackButton;

        private Server SelectSever;
        private List<Server> m_serverInfo = new List<Server>();

        void Awake()
        {
            //PlatformLoginBehaviour.message += " ServerListPanel_V3 Awake:";
			//LoginManager.Instance.ServerVersion = VersionConfigDB.versionData.Version;
            LoginTipsLabel.SetText("");
            SelectServerBtn.SetCallBackFuntion(OnSelectServerBtnClick);
            JoinGameBtn.SetCallBackFuntion(OnJoinGameBtnClick);
            CommonBtnInfo commonBtnInfo = new CommonBtnInfo(OnBackButtonTapped);  //暂时没有返回回调
            GoBackButton.InitButton(commonBtnInfo);
        }     
       
        public override void Show()
        {
            m_serverInfo = LoginManager.Instance.ServerInfo.ToList();
			if(m_serverInfo.Count>0)
			{
            	OnSelectServer(m_serverInfo[0]);
			}
        }

        public override void Close()
        {
            PlatformLoginBehaviour.message += " ServerListPanelV3 Close";
            transform.localPosition = new Vector3(0,0,-1000);

            PlatformLoginBehaviour.message += " ServerListPanelV3 Position:" + transform.localPosition;
        }

        void OnSelectServerBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            SingleSeverListPanel.Show(m_serverInfo, this);
        }

        void OnDestroy()
        {
            DestroyPanel();
        }
        public override void DestroyPanel()
        {
            RemoveAllEvent();
        }

        public void OnSelectServer(Server server)
        {
            SelectServerBtn.SetButtonText(server.Name);
            LoginTipsLabel.SetText("");
            this.SelectSever = server;
        }

        //在服务器列表中选择服务器后。点击进入游戏的时候。
        void OnJoinGameBtnClick(object obj)
        {
		    //if(this.SelectSever.Version != LoginManager.Instance.ServerVersion)
			//{		
			//	MessageBox.Instance.Show(3, "", "该服务器不能使用", "确认", null);
			//	return;
			//}

            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            LoginManager.Instance.LoginGameServer(this.SelectSever);
			JHPlatformConnManager.Instance.GameWorldId = this.SelectSever.No;
			LoginTipsLabel.SetText(LanguageTextManager.GetString("IDS_H1_566"));
        }
		
		void OnBackButtonTapped(object obj)
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.Login);	
		}


    }
}