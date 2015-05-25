using UnityEngine;
using System.Collections;
using UI.Login;

public class LoginBehaviour : View
{
    //private static LoginBehaviour m_instance;
    //public static LoginBehaviour Instance { get { return m_instance; } }

    ////private bool Allow
    ////登录平台UI按钮
    ////public SingleButtonCallBack Login91Btn;
    ////public SingleButtonCallBack RegisterAccountBtn;
    //////进入游戏UI按钮
    ////public SingleButtonCallBack LoginGameBtn;
    ////public SingleButtonCallBack ChangeAccountBtn;
    ////public LoginPanel loginPanel;
    ////public JoinGamePanel joinGamePanel;

    ////服务器选择界面的UI控制脚本
    //public ServerSelectedBehaviour ServerSelectedBehaviour;

    //private bool StartToMonitorPlatform = false;
    //void Start()
    //{
    //    m_instance = this;
    //    RegisterEventHandler();
    //    string perfsPlayerId = PlayerPrefs.GetString("PlayerId");
    //    string perfsSessionId = PlayerPrefs.GetString("SessionId");
    //    if (string.IsNullOrEmpty(perfsPlayerId) || string.IsNullOrEmpty(perfsSessionId))
    //    {
    //        //跳到登录平台UI
    //        LoginManager.Instance.GotoPlatformButtonEnable = true;
    //        UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.Login);
    //    }
    //    else
    //    {
    //        //跳到进入游戏UI            
    //        LoginManager.Instance.GotoHttpServerButtonEnable = true;
    //        LoginManager.Instance.PlayerId = long.Parse(perfsPlayerId);
    //        LoginManager.Instance.PlayerId = long.Parse(perfsSessionId);
    //        UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.JoinGame);
    //    }

    //}

    //void OnDestroy()
    //{

    //    LoginManager.Instance.Release();
    //    LoginManager.Instance.OnLoginComplete -= this.Instance_OnLoginComplete;

    //    m_instance = null;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (StartToMonitorPlatform)
    //    {
    //        if (LoginManager.Instance.IsLoginPlatform())
    //        {
    //            StartToMonitorPlatform = false;
    //            //转到进入游戏UI
    //            LoginManager.Instance.GotoHttpServerButtonEnable = true;
    //            UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.JoinGame);
    //        }
    //    }
    //}
    //public void OnRegisterBtnClick(object obj)
    //{

    //}
    //public void OnLogin91BtnClick(object obj)
    //{
    //    if (!LoginManager.Instance.GotoPlatformButtonEnable) return;
    //    LoginManager.Instance.GotoPlatformButtonEnable = false;
    //    if (!LoginManager.Instance.IsLoginPlatform())
    //    {
    //        LoginManager.Instance.LoginPlatform();            
    //    }
    //    StartToMonitorPlatform = true;
    //}
    //public void OnLoginGameBtnClick(object obj)
    //{
    //    if (!LoginManager.Instance.GotoHttpServerButtonEnable) return;
    //    LoginManager.Instance.GotoHttpServerButtonEnable = false;
    //    if(LoginManager.Instance.IsLoginPlatform())
    //    {
    //        //启动登录访问，出现Loading，等待返回
    //        //显示Loading界面
    //        StartCoroutine(LoginManager.Instance.RequestPhpService(LoginManager.PHPSERVER_ADDRESS, LoginManager.Instance.PhpServerCallback));
    //    }
    //    else
    //    {
    //        //转到登录平台UI
    //        LoginManager.Instance.GotoPlatformButtonEnable = true;
    //        UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.Login);
    //    }
    //}
    //public void OnChangeAccountBtnClick(object obj)
    //{
    //    LoginManager.Instance.GotoPlatformButtonEnable = true;
    //    UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.Login);
    //}
    //void Instance_OnLoginComplete(PhpBackObj phpBackObj)
    //{
    //    ShowSelectServer(phpBackObj);
    //}
    //public void ShowSelectServer(PhpBackObj phpBackObj)
    //{
    //    if (phpBackObj != null)
    //    {
    //        if (phpBackObj.ErrorCode == "1")   //成功
    //        {
    //            //转到服务器选择界面 phpBackObj.ServerInfo是一个Server类的数组，里面是Server列表信息。
    //            //TraceUtil.Log(phpBackObj.ServerInfo.Length);
    //            LoginManager.Instance.GotoHttpServerButtonEnable = true;
    //            ServerSelectedBehaviour.InitUI(phpBackObj.ServerInfo);
    //        }
    //        else
    //        {
    //            UI.MessageBox.Instance.Show(3, "", phpBackObj.ErrorDesc, LanguageTextManager.GetString("IDS_H2_13"), null);
    //        }
    //    }
    //}
    //private void S_CUserLoginResHandle(INotifyArgs notify)
    //{
    //    LoginManager.Instance.CreateActorButtonEnable = true;
    //    LoginManager.Instance.DeleteActorButtonEnable = true;
    //    LoginManager.Instance.EnterTownButtonEnable = true;
    //    LoginManager.Instance.NewSSUserLoginRes = (NewSSUserLoginRes)notify;
    //    if (LoginManager.Instance.NewSSUserLoginRes.lPromptFlag == 0)
    //    {
    //        //保存用户名和密码
    //        PlayerPrefs.SetString("PlayerId",LoginManager.Instance.Account);
    //        PlayerPrefs.SetString("PlayerPwd", LoginManager.Instance.Pwd);
    //        if (LoginManager.Instance.NewSSUserLoginRes.lActorNum == 0)
    //        {
    //            //显示创建角色界面
    //            UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.CreatRole);
    //        }
    //        else
    //        {
    //            //显示选择角色界面
    //            GameDataManager.Instance.ResetData(DataType.ActorSelector, LoginManager.Instance.NewSSUserLoginRes.SSActorInfos);
    //            UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.SelectRole);
    //        }
    //    }
    //    else
    //    {
    //        //提示登录失败
    //        UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_236"), LanguageTextManager.GetString("IDS_H2_13"), null);
    //    }

    //}
    //private void S_CEnterCodeHandle(INotifyArgs notify)
    //{
    //    NetServiceManager.Instance.LoginService.SubmitAccountInfo(LoginManager.Instance.Account, LoginManager.Instance.Pwd);
    //}
    protected override void RegisterEventHandler()
    {
        //LoginManager.Instance.OnLoginComplete += this.Instance_OnLoginComplete;
        //AddEventHandler(EventTypeEnum.S_CEnterCode.ToString(), S_CEnterCodeHandle);
        //AddEventHandler(EventTypeEnum.S_CUserLoginRes.ToString(), S_CUserLoginResHandle);
    }
   
}
