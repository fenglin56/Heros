using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCommon;
using UnityEngine;
using System.Collections;

public class LoginManager:Controller
{
    //public const string PHPSERVER_ADDRESS = "http://jh.fanhougame.net/HttpServer/loginCheck.php"; //--"http://192.168.2.76/dev91/loginCheck.php";
    //public const string PHPSERVER_ADDRESS = "http://192.168.4.26/jh/logintest/demo.php";  //智敬本机
    public const string PHPSERVER_ADDRESS = "http://jh.fanhougame.net/logintest/demo.php";  //外网测试
    public event LoginCompleteHandle OnLoginComplete;
    private Server SelectedServer = null;    //用户选中的服务器项，
    private static LoginManager m_instance;
    
    public static LoginManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new LoginManager();
            }
            return m_instance;
        }
    }
    public void Init91()
    {
        //Bonjour.init();
    }
    public NewSSUserLoginRes NewSSUserLoginRes { get; set; }
    public SSActorInfo LoginSSActorInfo { get; set; }
    public Server[] ServerInfo{get;set;}
	public int ServerVersion { get; set; }						// 客户端保存的服务器版本号，用于和服务器端对比
    public long PlayerId { get;  set; }
    public long SessionId { get;  set; }

    public string Account { get; set; }
    public string Pwd { get; set; }

    //按钮控制开关   超时机制暂不作
    public bool GotoPlatformButtonEnable = true;   //打开界面后Enable，点击后Disable  
    public bool GotoHttpServerButtonEnable = true;  //打开界面后Enable，点击后Disabel
    public bool GotoGameServerButtonEnable = true;  //打开界面后Enable，点击后Disable，服务器返回结果后Enable
    public bool CreateActorButtonEnable = true;     //打开界面后Enable，点击后Disable，服务器返回结果后Enable
    public bool EnterTownButtonEnable = true;     //打开界面后Enable，点击后Disable，服务器返回结果后Enable
    public bool DeleteActorButtonEnable = true;     //打开界面后Enable，点击后Disable，服务器返回结果后Enable

    /// <summary>
    /// 登陆平台
    /// </summary>
    public void LoginPlatform()
    {        
        //if (!Bonjour.isLogined())
        //{
        //    Bonjour.login();
        //}
    }
    public void LogoutPlatform()
    {
        //Bonjour.loginOut();
    }
    public bool IsLoginPlatform()
    {
        SessionId = 0;
        bool flag = true;
        return flag;
    }
    public void ResetLoginButtonState()
    {
        this.GotoPlatformButtonEnable = true;
        this.GotoHttpServerButtonEnable = true;
        this.GotoGameServerButtonEnable = true;
        this.CreateActorButtonEnable = true;
        this.EnterTownButtonEnable = true;
        this.DeleteActorButtonEnable = true;       
    }
    public IEnumerator RequestPhpService(string phpServerUri, ReturnHandler returnHandler)
    {
        WWWForm wwwform = new WWWForm();

        wwwform.AddField("uin", 1);//PlayerId.ToString());  //测试用1
        wwwform.AddField("sessionId", SessionId.ToString());

        WWW www = new WWW(phpServerUri, wwwform);
       
        yield return www;

        if (www.isDone && string.IsNullOrEmpty(www.error))
        {
            returnHandler(www.text);
        }
        else
        {
            TraceUtil.Log("访问PHP错误");
        }
    }
    /// <summary>
    /// PhpService CallBack Handler. 目前暂不调用，Php服务器目前仅返回服务器IP列表，没有做平台验证。
    /// 需要修改服务器Json串请请看LoginUIPanel。
    /// </summary>
    /// <param name="backJson"></param>
    public void PhpServerCallback(string backJson)
    {
        var testJson = "{\"ErrorCode\":\"1\",\"ErrorDesc\":\"\u6709\u6548\",\"ServerInfo\":[{\"No\":1,\"Name\":\"112.124.54.108\",\"IP\":\"112.124.54.108\",\"Port\":8000,\"ActorNumber\":3,\"Status\":1},{\"No\":2,\"Name\":\"192.168.0.190\",\"IP\":\"192.168.0.190\",\"Port\":8000,\"ActorNumber\":150,\"Status\":2},{\"No\":3,\"Name\":\"192.168.2.91\",\"IP\":\"192.168.2.91\",\"Port\":8000,\"ActorNumber\":150,\"Status\":3},{\"No\":4,\"Name\":\"192.168.0.40\",\"IP\":\"192.168.0.40\",\"Port\":8000,\"ActorNumber\":150,\"Status\":1},{\"No\":5,\"Name\":\"192.168.3.21\",\"IP\":\"192.168.3.21\",\"Port\":8000,\"ActorNumber\":150,\"Status\":0},{\"No\":6,\"Name\":\"192.168.1.67\",\"IP\":\"192.168.1.67\",\"Port\":8000,\"ActorNumber\":150,\"Status\":0},{\"No\":7,\"Name\":\"127.0.0.1\",\"IP\":\"127.0.0.1\",\"Port\":8000,\"ActorNumber\":3,\"Status\":1},{\"No\":8,\"Name\":\"192.168.2.62\",\"IP\":\"192.168.2.62\",\"Port\":8000,\"ActorNumber\":3,\"Status\":1}]}";
        var jsonBackObj= JsonConvertor<PhpBackObj>.Json2Object(testJson);
     
        if (OnLoginComplete != null)
        {
            OnLoginComplete(jsonBackObj);
        }
        else
        {
            TraceUtil.Log("没有监听Php服务返回事件");
        }
        
    }
  
    /// <summary>
    /// 登录到游戏网关服务器
    /// </summary>
    public void LoginGameGateway()
    {

    }
	/// <summary>
	/// 发起服务器连接，等待服务器登录随机数返回,在LodginUIPanel里监听S_CEnterCode消息，S_CEnterCodeHandle处理。
	/// </summary>
	/// <returns><c>true</c>, if to server was connected, <c>false</c> otherwise.</returns>
	/// <param name="selectedServer">Selected server.</param>
    public bool ConnectToServer(Server selectedServer)
    {
        bool flag=false;
        IpManager.InitServiceConfig(selectedServer.IP, selectedServer.Port);

        flag = NetServiceManager.Instance.LoginService.ConnectToServer() == SERVICE_CODE.SUCCESS;
        if (flag)
        {
            PlayerPrefs.SetString("ServerIP", selectedServer.IP);
            PlayerPrefs.SetInt("ServerPort", selectedServer.Port);
        }
        return flag;
    }
    public void LoginGameServer(object obj)
    {
        if (!this.GotoGameServerButtonEnable) return;

        this.SelectedServer = obj as Server;
        if (SelectedServer != null)
        {
            this.GotoGameServerButtonEnable = false;
            if (!this.ConnectToServer(this.SelectedServer))
            {
                this.GotoGameServerButtonEnable = true;
                //提示用户进入服务器失败
                UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_237"), LanguageTextManager.GetString("IDS_H2_13"), null);				
            }			
            //连接完成，服务器应该会下发一个登录随机码
        }
        else
        {
            //提示用户选择服务器
            UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_238"), LanguageTextManager.GetString("IDS_H2_13"), null);
        }
    }
    /// <summary>
    /// 获得上次保留的服务器
    /// </summary>
    /// <returns></returns>
    public Server InitSelectedServer()
    {
        string preferIp = PlayerPrefs.GetString("ServerIP");
        int preferPort = PlayerPrefs.GetInt("ServerPort");
        return ServerInfo.FirstOrDefault(P=>P.IP==preferIp&&P.Port==preferPort);
    }
    public void Release()
    {
        m_instance = null;
    }
    void ReceiveDbError_CreateFailedLoggedin(INotifyArgs e)
    {
        //不需要客户端处理，等服务器处理完成后会自动进入游戏。
    }
    
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.CreateFailedLoggedin.ToString(), ReceiveDbError_CreateFailedLoggedin);
    }
}

public delegate void ReturnHandler(string backData);
public delegate void LoginCompleteHandle(PhpBackObj phpBackObj);
