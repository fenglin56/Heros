using UnityEngine;
using System.Collections;
using System;

public class UCPlatformcallbackHandler : View, IPlatformSDKCallback 
{


    private Action<bool> OnInitBackResult;
    private Action<bool> OnLoginBackResult;
    private Action<bool, string> OnUserJsonInfoBack;
    private Action<PhpBackObj> OnRequestPHPBackResult;
    private Action<bool> OnPaymentBackResult;
    private UCConfigData m_UCConfigData;

    private string m_sid;

    protected override void RegisterEventHandler()
    {
    }
    public UCConfigData UCConfigData
    {
        get
        {
            if (m_UCConfigData == null)
            {
                m_UCConfigData = (UCConfigData)PlatformConfig;
            }
            return m_UCConfigData;
        }
    }
    public PlatformConfigBase PlatformConfig
    {
        get;
        set;
    }

    public PlatformType PlatformType
    {
        get;
        set;
    }
    public void DoShowSprite()
    {
        //throw new NotImplementedException();
    }
    
    public void DoHideSprite()
    {
        //throw new NotImplementedException();
    }
    
    public void ExtendInfoSubmit(string service, string role, string grade)
    {
        //throw new NotImplementedException();
    }
    public void InitPlatform(System.Action<bool> initResult)
    {
            #if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_UC
        //设置日志级别
        UCGameSdk.setLogLevel(UCConfig.logLevel);
        //设置屏幕方向
        UCGameSdk.setOrientation(UCConfig.orientation);

        //设置使用登录界面类型
        UCGameSdk.setLoginUISwitch(UCConfig.loginUISwitch);

        UCGameSdk.initSDK(UCConfigData.debugMode, UCConfigData.logLevel, UCConfigData.cpid, UCConfigData.gameid,
            UCConfigData.serverid, UCConfigData.servername, UCConfigData.enablePayHistory, UCConfigData.enableLogout);
#endif

#endif
    }

    public void LoginPlatform(System.Action<bool> loginResult)
    {
        #if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_UC
        OnLoginBackResult = loginResult;
        UCGameSdk.login(false, "");
#endif
#endif
    }

    public void GetUserInfo(System.Action<bool,string> userJsonInfo)
    {
                #if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_UC
        OnUserJsonInfoBack = userJsonInfo;
        m_sid = UCGameSdk.getSid();
        if (OnUserJsonInfoBack != null)
        {
            OnUserJsonInfoBack(true, m_sid);
        }
#endif
#endif
    }

    public void RequestPHP(System.Action<PhpBackObj> requestPHPResult)
    {
        OnRequestPHPBackResult = requestPHPResult;
        StartCoroutine(AuthorPHP());
    }

    public void Payment(System.Action<bool> paymentResult, params object[] payArgs)
    {
        #if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_UC
        OnPaymentBackResult = paymentResult;
        var playerData = PlayerManager.Instance.FindHeroDataModel();
        UCGameSdk.pay(false, 1, UCConfig.serverid, playerData.ActorID.ToString(), playerData.Name
        , playerData.PlayerValues.PLAYER_FIELD_GM_LEVEL.ToString(), "actorId=" + playerData.ActorID.ToString() + "&name=" + playerData.Name, "");
#endif
#endif
    }
    public void Notify(string ticker, string title, string content, int waitingTime)
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_UC 
        jo.Call("Notify",ticker,title,content);
#endif
#endif
    }
    public void ShowPlatformInfo()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_UC 
        //jo.Call("SDK_doShowGameCenter");
#endif
#endif
    }
    private IEnumerator AuthorPHP()
    {
        WWW www = new WWW(string.Format(m_UCConfigData.PhpLoginAuthorURL,m_sid));
        yield return www;
        if (www.isDone)
        {
            if (OnRequestPHPBackResult != null)
            {
                var jsonBackObj = JsonConvertor<PhpBackObj>.Json2Object(www.text.Replace("\"", "'"));
                OnRequestPHPBackResult(jsonBackObj);
            }
        }
    }
    #region UC Android Callback

        #if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_UC

	public void OnUCGameSdkCallback (string jsonstr)
	{
		log ("UCCallbackMessage - OnUCGameSdkCallback message: jsonstr=" + jsonstr);

		JsonData json = JsonMapper.ToObject (jsonstr);
		string callbackType = (string)json ["callbackType"];
		int code = (int)json ["code"];
		JsonData data = json ["data"];

		switch (callbackType) {
		case UCConstants.CALLBACKTYPE_InitSDK:
			OnInitResult (code, (string)data);
            
			break;

		case UCConstants.CALLBACKTYPE_Login:
			OnLoginResult (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_Logout:
			OnLogout (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_FloatMenu:
			OnFloatMenu (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_UserCenter:
			OnUserCenter (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_EnterUI:
			OnEnterUI (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_Pay:
			OnPayCallback (code, data);
			break;

		case UCConstants.CALLBACKTYPE_UPointCharge:
			OnUPointCharge (code, (string)data);
			break;

		case UCConstants.CALLBACKTYPE_IsUCVip:
			OnIsUCVipResult (code, (bool)data);
			break;

		case UCConstants.CALLBACKTYPE_GetUCVipInfo:
			OnGetUCVipInfoResult (code, data);
			break;

		}
	}

	private void OnInitResult (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnInitResult: code={0}, msg={1}", code, msg));
		//输出初始化结果到页面(接入后删除)
		SendMessage ("setMessage", string.Format ("UCCallbackMessage - OnInitResult: code={0}, msg={1}", code, msg));
        UI.Login.LoginUIManagerPlatform.setSdkMessage(string.Format("UCCallbackMessage - OnInitResult: code={0}, msg={1}", code, msg));

		if (code == UCStatusCode.SUCCESS) {
			log ("init succeeded");
            if (OnInitBackResult != null)
            {
                OnInitBackResult(true);
            }

			//TODO: 可把创建和显示悬浮按钮移到合适的逻辑中
			UCGameSdk.createFloatButton ();
		} else {
			log (string.Format ("Failed initing UC game sdk, code={0}, msg={1}", code, msg));

			//初始化失败处理
            if (OnInitBackResult != null)
            {
                OnInitBackResult(false);
            }
		}
	}

	private void OnLoginResult (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnLoginResult: code={0}, msg={1}", code, msg));

		//输出登录结果到页面(接入后删除)
		SendMessage ("setMessage", string.Format ("UCCallbackMessage - OnLoginResult: code={0}, msg={1}", code, msg));
        UI.Login.LoginUIManagerPlatform.setSdkMessage(string.Format("UCCallbackMessage - OnLoginResult: code={0}, msg={1}", code, msg));

		if (code == UCStatusCode.SUCCESS) {
            LoginPlatformManager.Instance.m_isLoginedPlatform = true;

			//string sid = UCGameSdk.getSid ();
			//log ("login succeeded: sid=" + sid);

            //UI.Login.LoginUIManagerPlatform.Instance.OnSendLoginInfo();
            if (OnLoginBackResult != null)
            {
                OnLoginBackResult(true);
            }
			//
			UCConfig.logined = true;
            
			//显示悬浮按钮
			UCGameSdk.showFloatButton (100, 20, true);

			//进入分区通知，//TODO: 应在进入分区时进行该调用
			//UCGameSdk.notifyZone ("66区-风起云涌", "R29924", "Role-大漠孤烟");

		} else if (code == UCStatusCode.LOGIN_EXIT) {
			//登录界面退出，返回到游戏画面
			log ("login UI exit, back to game UI");
            if(msg == "取消登录")
                Invoke("ReLoginUC", 2f);
            
		} else {
			log (string.Format ("Failed login, code={0}, msg={1}", code, msg));
			//登录失败
            if (OnLoginBackResult != null)
            {
                OnLoginBackResult(false);
            }
		}
	}

    void ReLoginUC()
    {
        LoginPlatformManager.Instance.LoginPlatform();
    }


	private void OnLogout (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnLogout: code={0}, msg={1}", code, msg));
		
		//输出退出登录结果到页面(接入后删除)
		UI.Login.LoginUIManagerPlatform.setSdkMessage (string.Format ("UCCallbackMessage - OnLogout: code={0}, msg={1}", code, msg));
		
		if (code == UCStatusCode.SUCCESS) {
			//当前登录用户已退出，应将游戏切换到未登录的状态。
			UCConfig.logined = false;
			UCGameSdk.destroyFloatButton ();
            
            LoginPlatformManager.Instance.m_isLoginedPlatform = false;
            LoginPlatformManager.Instance.LoginPlatform();
			//DemoControl.Restart();
		} else {
			//unknown error
			log (string.Format ("unknown error: code={0}, msg={1}", code, msg));
		}
	}

	private void OnFloatMenu (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnFloatMenu: code={0}, msg={1}", code, msg));
		
		//输出悬浮菜单进入页面前后状态信息到页面(接入后删除)
        UI.Login.LoginUIManagerPlatform.setSdkMessage(string.Format("UCCallbackMessage - OnFloatMenu: code={0}, msg={1}", code, msg));
		
		if (code == UCStatusCode.SDK_OPEN) {
			//打开了SDK界面
			log ("user opened the SDK UI");
		} else if (code == UCStatusCode.SDK_CLOSE) {
			//SDK界面已关闭，回到游戏画面，游戏应根据实际需要决定是否进行画面刷新
			log ("user closed the SDK UI, backed to game UI");
		}
	}

	private void OnPayCallback (int code, JsonData jsonOrder)
	{
		log (string.Format ("UCCallbackMessage - OnPayCallback: code={0}", code));
		
		//输出支付回调信息到页面(接入后删除)
        UI.Login.LoginUIManagerPlatform.setSdkMessage(string.Format("UCCallbackMessage - OnPayCallback: code={0}", code));
	
		if (code == UCStatusCode.SUCCESS) {
			string orderId = (string)jsonOrder ["orderId"];			
			int payWayId = (int)jsonOrder ["payWayId"];
			string payWayName = (string)jsonOrder ["payWayName"];
			
			float orderAmount = 0;
	
			JsonData jdAmount = (JsonData)jsonOrder ["orderAmount"];
			switch(jdAmount.GetJsonType()) 
			{
			case JsonType.Int:				
				orderAmount = (float)(int)jdAmount;			
				break;
			case JsonType.Double:				
				orderAmount = (float)(double)jdAmount;			
				break;
			case JsonType.String:
				try
				{
				
					orderAmount = (float)Convert.ToDouble((string)jdAmount);
				}
				catch(Exception e)
				{
					log ("order amount is not a valid number");
				}
				break;
			default:
				log ("order amount is not a valid json number");
				break;
			}
			
			//充值下单成功，游戏应对下单结果进行处理，一般需把订单号、下单金额、支付渠道ID、支付渠道名称等信息上传到游戏服务器进行保存
			log (string.Format ("UCCallbackMessage - received order info: code={0}, orderId={1}, orderAmount={2:0.00}, payWayId={3}, payWayName={4}",
					code, orderId, orderAmount, payWayId, payWayName));

			//游戏根据需要进行订单处理，一般需要把订单号传回游戏服务器，在服务器上保存
            if (OnPaymentBackResult != null)
            {
                OnPaymentBackResult(true);
            }

		} else if (code == UCStatusCode.PAY_USER_EXIT) {
			//充值界面已关闭，回到游戏画面，游戏应根据实际需要决定是否进行画面刷新
		} else {
			//充值调用失败
            if (OnPaymentBackResult != null)
            {
                OnPaymentBackResult(false);
            }
		}

	}

	private void OnUserCenter (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnUserCenter: code={0}, msg={1}", code, msg));		
		//输出将要退出九游社区（个人中心）结果到页面(接入后删除)
        UI.Login.LoginUIManagerPlatform.setSdkMessage(string.Format("UCCallbackMessage - OnUserCenter: code={0}, msg={1}", code, msg));	
		
		if (code == UCStatusCode.SUCCESS) {
			//用户退出了九游社区（个人中心）界面，返回游戏画面，游戏应根据实际需要决定是否进行画面刷新
			log ("user closed the user center UI, backed to game UI");
		} else {
			//fail
			log (string.Format ("unknown error: code={0}, msg={1}", code, msg));
		}
	}

	private void OnUPointCharge (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnUPointCharge: code={0}, msg={1}", code, msg));
		//输出U点充值页面返回结果到页面(接入后删除)
        UI.Login.LoginUIManagerPlatform.setSdkMessage(string.Format("UCCallbackMessage - OnUPointCharge: code={0}, msg={1}", code, msg));
		if (code == UCStatusCode.SDK_CLOSE) {
			//U点充值完成，返回游戏画面，游戏应根据实际需要决定是否进行画面刷新
			log ("user closed the user center UI, backed to game UI");
		} else if (code == UCStatusCode.NO_INIT) {
			log ("not inited");
		} else if (code == UCStatusCode.NO_LOGIN) {
			log ("not logined");
		} else {
			//unknown error
			log (string.Format ("unknown error: code={0}, msg={1}", code, msg));
		}
	}

	private void OnEnterUI (int code, string msg)
	{
		log (string.Format ("UCCallbackMessage - OnEnterUI: code={0}, msg={1}", code, msg));
		//输出进入的页面信息到页面(接入后删除)
        UI.Login.LoginUIManagerPlatform.setSdkMessage(string.Format("UCCallbackMessage - OnEnterUI: code={0}, msg={1}", code, msg));
	}

	private void OnIsUCVipResult (int code, bool isUCVip)
	{
		log (string.Format ("UCCallbackMessage - OnIsUCVipResult: code={0}, isUCVip={1}", code, isUCVip));
		//输出是否是UC会员结果到页面(接入后删除)
        UI.Login.LoginUIManagerPlatform.setSdkMessage(string.Format("UCCallbackMessage - OnIsUCVipResult: code={0}, isUCVip={1}", code, isUCVip));
		if (code == UCStatusCode.SUCCESS) {
			//成功获得结果
			//isUCVip
		} else {
			//fail
			log (string.Format ("failed get whether or not current user is a UC VIP member: code={0}", code));
		}
	}

	private void OnGetUCVipInfoResult (int code, JsonData jsonUCVipInfo)
	{
		log (string.Format ("UCCallbackMessage - OnGetUCVipInfoResult: code={0}, data={1}", code, jsonUCVipInfo.ToJson ()));
		//将当前用户的会员信息输出到页面(接入后删除)
        UI.Login.LoginUIManagerPlatform.setSdkMessage(string.Format("UCCallbackMessage - OnGetUCVipInfoResult: code={0}, data={1}", code, jsonUCVipInfo.ToJson()));
		if (code == UCStatusCode.SUCCESS) {
			//成功获得UC会员信息
			int status = (int)jsonUCVipInfo ["status"];
			int grade = (int)jsonUCVipInfo ["grade"];
			string validFrom = (string)jsonUCVipInfo ["validFrom"];
			string validTo = (string)jsonUCVipInfo ["validTo"];

			log (string.Format ("status={0}, grade={1}, validFrom={2}, validTo={3}", status, grade, validFrom, validTo));

			JsonData privilegeList = jsonUCVipInfo ["privilegeList"];
			if (privilegeList.IsArray) {
				int size = privilegeList.Count;
				JsonData privilege;

				int enjoy = 0;
				int pId = 0;

				for (int i = 0; i < size; i++) {
					privilege = privilegeList [i];
					enjoy = (int)privilege ["enjoy"];
					pId = (int)privilege ["pId"];

					log (string.Format ("privilege list[{0}]: enjoy={1}, pId={2}", i, enjoy, pId));
				}
			}

		} else {
			//fail
			log (string.Format ("failed get current user's UC VIP member info: code={0}", code));
		}


	}

	private void log (string msg)
	{
		//TraceUtil.Log(msg);
		print (msg);
	}
#endif

#endif
    #endregion


    public void ResetAndroidCallGameObject(ref GameObject defaultObject)
    {
        defaultObject = GameObject.FindGameObjectWithTag("MainCamera");
    }
}
