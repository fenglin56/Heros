using UnityEngine;
using System.Collections;

public class ServerSelectedBehaviour : View
{
    private Server[] m_serverInfo;
    private Server SelectedServer=null;    //用户选中的服务器项，
    //public SingleButtonCallBack ConnectGameServer;

    public Server[] ServerInfo { get { return m_serverInfo; } }

    void Awake()
    {
        LoginManager.Instance.GotoGameServerButtonEnable= true;
        RegisterEventHandler();
    }
	// Use this for initialization
    //void Start () {
    //    ConnectGameServer.SetCallBackFuntion(OnLogin91BtnClick);
    //}
    /// <summary>
    /// 拿服务器列表参数呈现UI
    /// </summary>
    /// <param name="serverInfo"></param>
    public void InitUI(Server[] serverInfo)
    {
        m_serverInfo = serverInfo;
        UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI,UI.Login.LoginUIType.ServerList);
    }
	// Update is called once per frame
	void Update () {
	
	}
    public void LoginGameServer(object obj)
    {
        if (!LoginManager.Instance.GotoGameServerButtonEnable) return;
       
        this.SelectedServer = obj as Server;
        if (SelectedServer != null)
        {
            //if (!LoginManager.Instance.IsLoginPlatform())
            //{
            LoginManager.Instance.GotoGameServerButtonEnable = false;
                if (!LoginManager.Instance.ConnectToServer(this.SelectedServer))
                {
                    LoginManager.Instance.GotoGameServerButtonEnable = true;
                    //提示用户进入服务器失败
                    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_237"), LanguageTextManager.GetString("IDS_H2_13"), null);
                }
            //}
            //else
            //{
                //转到登录平台UI
                //LoginManager.Instance.GotoPlatformButtonEnable= true;
            //    UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.Login);
            //}
        }
        else
        {
            //提示用户选择服务器
            UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_238"), LanguageTextManager.GetString("IDS_H2_13"), null);
        }
    }
    protected override void RegisterEventHandler()
    {
        //
    }
}
