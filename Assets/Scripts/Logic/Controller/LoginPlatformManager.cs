using UnityEngine;
using System.Collections;
using NetworkCommon;

public class LoginPlatformManager
{

//    public const string PHP_PLATFORM_SERVER = "http://jh.fanhougame.net/HttpServer/loginCheck.php"; //--"http://192.168.2.76/dev91/loginCheck.php";
//    public event LoginCompleteHandle OnLoginComplete;
//    public ReturnHandler OnReturnHandler;

//    public Server[] ServerInfo { get; set; }
//    public long PlayerId { get; set; }
//    public string Account { get; set; }
//    public string Pwd { get; set; }

//    public string GetString = string.Empty;

//    private Server SelectedServer = null;    //用户选中的服务器项，
//    public NewSSUserLoginRes NewSSUserLoginRes { get; set; }
//    public SSActorInfo LoginSSActorInfo { get; set; }
//    public bool m_isLoginedPlatform = false;


//    private static LoginPlatformManager m_instance;
//    public static LoginPlatformManager Instance
//    {
//        get
//        {
//            if (m_instance == null)
//            {
//                m_instance = new LoginPlatformManager();
//            }
//            return m_instance;
//        }
//    }
	

//    public void LoginGameServer(object obj)
//    {
//        //if (!this.GotoGameServerButtonEnable) return;

//        this.SelectedServer = obj as Server;
//        if (SelectedServer != null)
//        {
//            //this.GotoGameServerButtonEnable = false;
//            if (!this.ConnectToServer(this.SelectedServer))
//            {
//                //this.GotoGameServerButtonEnable = true;
//                //提示用户进入服务器失败
//                UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_237"), LanguageTextManager.GetString("IDS_H2_13"), null);
//            }

//            DestroyFloatButton();

//        }
//        else
//        {
//            //提示用户选择服务器
//            UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_238"), LanguageTextManager.GetString("IDS_H2_13"), null);
//        }
//    }

//    public bool ConnectToServer(Server selectedServer)
//    {
//        bool flag = false;
//        IpManager.InitServiceConfig(selectedServer.IP, selectedServer.Port);

//        flag = NetServiceManager.Instance.LoginService.ConnectToServer() == SERVICE_CODE.SUCCESS;
//        if (flag)
//        {
//            PlayerPrefs.SetString("ServerIP", selectedServer.IP);
//        }
//        return flag;
//    }

//    public void PhpServerCallback(string backJson)
//    {
//        //var testJson = "{\"ErrorCode\":\"1\",\"ErrorDesc\":\"\u6709\u6548\",\"UCID\":\"201047882\",\"ServerInfo\":[{\"No\":1,\"Name\":\"192.168.2.87\",\"IP\":\"192.168.2.87\",\"Port\":8000,\"ActorNumber\":150,\"Status\":1}]}";//,{\"No\":2,\"Name\":\"192.168.0.190\",\"IP\":\"192.168.0.190\",\"Port\":8000,\"ActorNumber\":150,\"Status\":2},{\"No\":3,\"Name\":\"192.168.2.91\",\"IP\":\"192.168.2.91\",\"Port\":8000,\"ActorNumber\":150,\"Status\":2},{\"No\":4,\"Name\":\"192.168.0.40\",\"IP\":\"192.168.0.40\",\"Port\":8000,\"ActorNumber\":150,\"Status\":1},{\"No\":5,\"Name\":\"192.168.3.21\",\"IP\":\"192.168.3.21\",\"Port\":8000,\"ActorNumber\":150,\"Status\":0},{\"No\":6,\"Name\":\"192.168.1.67\",\"IP\":\"192.168.1.67\",\"Port\":8000,\"ActorNumber\":150,\"Status\":0},{\"No\":7,\"Name\":\"127.0.0.1\",\"IP\":\"127.0.0.1\",\"Port\":8000,\"ActorNumber\":3,\"Status\":1},{\"No\":8,\"Name\":\"192.168.2.62\",\"IP\":\"192.168.2.62\",\"Port\":8000,\"ActorNumber\":3,\"Status\":1}]}";
//        //var testJson = "{'ErrorCode':1,'ErrorDesc':'','UCID':201047882,'ServerInfo':[{'No':1011,'Name':'\\u7535\\u4fe11\\u533a','IP':'192.168.0.228','Port':8000,'ActorNumber':0,'Status':0}]}";
//        //GetString = backJson.Replace("\"","'");
//        //backJson = testJson;
//        var jsonBackObj = JsonConvertor<PhpBackObj>.Json2Object(backJson.Replace("\"", "'"));

//#if (UNITY_ANDROID && !UNITY_EDITOR)
//#if ANDROID_UC
//        PlayerId = long.Parse(jsonBackObj.UCID);
//#elif ANDROID_JIUYAO

//#elif ANDROID_XIAOMI

//#endif

//#endif


//        if (OnLoginComplete != null)
//        {
//            OnLoginComplete(jsonBackObj);
//        }
//        else
//        {
//            TraceUtil.Log("没有监听Php服务返回事件");
//        }
//    }

//    public IEnumerator RequestPhpService(string phpServerUri, ReturnHandler returnHandler)
//    {
//        //WWWForm wwwform = new WWWForm();

//        //wwwform.AddField("act", 4);    //Php接口编号，登录验证请填4 
//        //wwwform.AddField("sid", "sst1game4a474b931d50439fbe8c71ddc7dd7038194270");//UCGameSdk.getSid());//PlayerId.ToString());  //测试用1
//        //wwwform.AddField("sessionId", SessionId.ToString());
//        //wwwform.AddField("version", "android-91");    
//        WWW www = null;
//#if (UNITY_ANDROID && !UNITY_EDITOR)
//#if ANDROID_UC
//        www = new WWW(phpServerUri + "?sid=" + UCGameSdk.getSid());//, wwwform);
//#elif ANDROID_JIUYAO

//#elif ANDROID_XIAOMI

//#endif

//#endif

//        yield return www;

//        if (www.isDone && string.IsNullOrEmpty(www.error))
//        {
//            returnHandler(www.text);
//        }
//        else
//        {
//            TraceUtil.Log("访问PHP错误");
//        }
//    }

//    #region  UC平台接入接口
//    /// <summary>
//    /// 初始化UC平台
//    /// </summary>
//    public void InitPlatform()
//    {
//#if (UNITY_ANDROID && !UNITY_EDITOR)
//#if ANDROID_UC
//        //设置日志级别
//        UCGameSdk.setLogLevel(UCConfig.logLevel);
//        //设置屏幕方向
//        UCGameSdk.setOrientation(UCConfig.orientation);

//        //设置使用登录界面类型
//        UCGameSdk.setLoginUISwitch(UCConfig.loginUISwitch);

//        UCGameSdk.initSDK(UCConfig.debugMode, UCConfig.logLevel, UCConfig.cpid, UCConfig.gameid, UCConfig.serverid, UCConfig.servername, UCConfig.enablePayHistory, UCConfig.enableLogout);
//#elif ANDROID_JIUYAO
//        LoginPlatformManager.Instance.Init91();
//#elif ANDROID_XIAOMI
//        //MiGameSdk.InitSDK(string.Empty, MiConfig.appId, MiConfig.appKey, ScreenModel.ORIENTATION_LANDSCAPE);
//        //LoginPlatform();
//#endif

//#endif
//    }

//    /// <summary>
//    /// 登陆UC平台
//    /// </summary>
//    public void LoginPlatform()
//    {
//#if (UNITY_ANDROID && !UNITY_EDITOR)
//#if ANDROID_UC
//        UCGameSdk.login(false, "");
//#elif ANDROID_JIUYAO

//#elif ANDROID_XIAOMI
//        //TraceUtil.Log("小米登陆####");
//        //MiGameSdk.Login();
//#endif

//#endif

//    }

//    public void ReLoginPlatform()
//    {

//#if (UNITY_ANDROID && !UNITY_EDITOR)
//#if ANDROID_UC
//        UCGameSdk.logout();
//        InitPlatform();
//        //UCGameSdk.login(false, "");
//#elif ANDROID_JIUYAO

//#elif ANDROID_XIAOMI

//#endif

//#endif
//    }


//    /// <summary>
//    /// 退出当前登陆帐号
//    /// </summary>
//    public void LoginOut()
//    {

//#if (UNITY_ANDROID && !UNITY_EDITOR)
//#if ANDROID_UC
//        UCGameSdk.logout();
//#elif ANDROID_JIUYAO

//#elif ANDROID_XIAOMI

//#endif

//#endif

//    }

//    /// <summary>
//    /// 打开平台充值界面
//    /// </summary>
//    public void PlatformPay(float moneyNum)
//    {

//#if (UNITY_ANDROID && !UNITY_EDITOR)
//#if ANDROID_UC
//        var playerData = PlayerManager.Instance.FindHeroDataModel();
//        UCGameSdk.pay(false, moneyNum, UCConfig.serverid, playerData.ActorID.ToString(), playerData.Name, playerData.PlayerValues.PLAYER_FIELD_GM_LEVEL.ToString(), "actorId=" + playerData.ActorID.ToString() + "&name=" + playerData.Name, "");
//#elif ANDROID_JIUYAO

//#elif ANDROID_XIAOMI
//        //string orderId = (string.Format("{0:0.}", 10000000000000000000 + UnityEngine.Random.Range(0, 2000000000000000000)));
//        //MiGameSdk.Pay(orderId, (int)moneyNum);
//#endif

//#endif

//    }

//    public void DestroyFloatButton()
//    {

//#if (UNITY_ANDROID && !UNITY_EDITOR)
//#if ANDROID_UC
//        UCGameSdk.destroyFloatButton();
//#elif ANDROID_JIUYAO

//#elif ANDROID_XIAOMI

//#endif

//#endif
//    }


//    /// <summary>
//    /// 退出UC平台连接
//    /// </summary>
//    public void ExitPlatformSDK()
//    {

//#if (UNITY_ANDROID && !UNITY_EDITOR)
//#if ANDROID_UC
//        UCGameSdk.exitSDK();
//#elif ANDROID_JIUYAO

//#elif ANDROID_XIAOMI

//#endif

//#endif

//    }

//    #endregion

//    public void Release()
//    {
//        m_instance = null;
//    }

}
