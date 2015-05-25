using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Security.Cryptography;
using UI;

public class TencentPlatformCallbackhandle :  View,IPlatformSDKCallback  {
    #region implemented abstract members of View

    protected override void RegisterEventHandler()
    {
        throw new NotImplementedException();
    }

    #endregion

    public PlatformConfigBase PlatformConfig{get;set;}
    public PlatformType PlatformType{ get;set;}
	///<summary>
	/// 登陆后的信息：字典中的key如下
	/// platform，openid， pf， pfkey ，accessToken， payToken
	/// </summary>
	private Dictionary<string, string> PlayerInfo;

	private float GoldNum, CurrencyNum;
	private Action<bool> OnInitResult;
	private Action<bool> OnLoginResult;
	private Action<bool,string> OnUserJsonInfo;
	//private Action<bool> OnPayProcessResult;
	private Action<bool> OnPaymentResult;
	private Action<PhpBackObj> OnRequestPHPResult;
	private Action<PurchaseInfo> OnPurchaseResult;
	private Action<PurchaseInfo> OnBalanceResult;

	private const string serverListUrl = "http://jh.fanhougame.net/tencent/getServerList.php";
	private const string payUrl = "http://jh.fanhougame.net/tencent/test_pay_m.php";
	private const string balanceUrl = "http://jh.fanhougame.net/tencent/getBalance.php";
	//private const string payUrl = "http://jh.fanhougame.net/tencent/pay_m.php";
    #if (UNITY_ANDROID && !UNITY_EDITOR)  
    #if ANDROID_TENCENT
    AndroidJavaClass jc = null;
    
    AndroidJavaObject jo = null;
    #endif 
    #endif

	void  Awake()
	{
		PlayerInfo = new Dictionary<string, string>();
	}

    #region IPlatformSDKCallback implementation
    public void InitPlatform(System.Action<bool> initResult)
    {

        #if (UNITY_ANDROID && !UNITY_EDITOR) 
        #if ANDROID_TENCENT  
        OnInitResult = initResult;
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        PlatformLoginBehaviour.message += " Call SDK_init";
		jo.Call("PlatformInit", TencentConfigData.appId, TencentConfigData.appKey,  TencentConfigData.Environment);
        
        //初始化回来没有回调，用协同产生回调
		StartCoroutine(TencentInitBack());
        #endif
        #endif
    }

    private IEnumerator TencentInitBack()
    {
        PlatformLoginBehaviour.message += " SDK_init Back";
        yield return new WaitForSeconds(1.5f);
        if (OnInitResult != null)
        {
            OnInitResult(true);    
        }
    }
    /// <summary>
    /// 登录平台
    /// </summary>
    /// <param name="loginResult"></param>
    public void LoginPlatform(System.Action<bool> loginResult)
    {
		#if (UNITY_ANDROID && !UNITY_EDITOR)  
		#if ANDROID_TENCENT
		OnLoginResult = loginResult;
		PlatformLoginBehaviour.message += " Call SDK_LoGin";
		jo.Call("QQLogin");
		#endif 
		#endif
    }
    /// <summary>
    /// 获得用户信息
    /// </summary>
    /// <param name="userJsonInfo"></param>
    public void GetUserInfo(System.Action<bool, string> userJsonInfo)
    {
		#if (UNITY_ANDROID && !UNITY_EDITOR)  
			#if ANDROID_TENCENT
			OnUserJsonInfo = userJsonInfo;
			//jo.Call("MyQQInfo");
			StartCoroutine(UserInfoCallback());
		#endif
		#endif

    }

	//public void UserInfoCallback(string  info)
	//{
		//info = "nickName:mike|gender:female|city:shenzhen";
	//	string[] infos = info.Split('|');
	//	OnUserJsonInfo(true, "");
	//}

	IEnumerator UserInfoCallback()
	{
		LoginManager.Instance.Account = PlayerInfo["openid"];
		LoginManager.Instance.Pwd = "123456";
		if(OnUserJsonInfo != null)
		{
			OnUserJsonInfo(true, "");
		}

		yield return null;
	}



    /// <summary>
    /// 调用PHP服务器
    /// </summary>
    /// <param name="requestPHPResult"></param>
    public void RequestPHP(System.Action<PhpBackObj> requestPHPResult)
    {
		#if (UNITY_ANDROID && !UNITY_EDITOR)  
		#if ANDROID_TENCENT 
		OnRequestPHPResult = requestPHPResult;
		StartCoroutine(AuthorPHP());
		#endif
		#endif
	}

	private IEnumerator AuthorPHP()
	{
		WWW www = new WWW(serverListUrl);
		yield return www;
		if (www.isDone)
		{
			if(!string.IsNullOrEmpty( www.error))
			{
				TraceUtil.Log("www error = " + www.error);
				if(OnRequestPHPResult != null)
				{
					OnRequestPHPResult(null);
				}
				yield break;
			}
			//m_message="Statue:"+www.error;
			//m_message+=www.text;
			//Debug.Log("Serverinfo = " + www.text);
			if (OnRequestPHPResult != null)
			{
				//Debug.Log("OnRequestPHPResult not null");
				PlatformLoginBehaviour.message = "PHP BackInfo:" + www.text.Replace("\"", "'");

				var jsonBackObj = JsonConvertor<PhpBackObj>.Json2Object(www.text.Replace("\"","'"));
				if(jsonBackObj != null)
				{
					PlatformLoginBehaviour.message += jsonBackObj.ServerInfo.Length;
					//Debug.Log("json convert right .........");
					OnRequestPHPResult(jsonBackObj);
				}
				//else
				//{
					//Debug.Log("json convert wrong.....");
					//Debug.Log("show again = " + www.text);
				//}
			}
			//else
			//{
			//	Debug.Log("OnRequestPHPResult is null");
			//}
		}

	}
	/// <summary>
    /// 支付接口
    /// </summary>
    /// <param name="paymentResult"></param>
    /// <param name="payArgs"></param>
    public void Payment(System.Action<bool> paymentResult, params object[] payArgs)
    {

    }

	public void GetBalance(Action<PurchaseInfo> balanceResult)
	{
		OnBalanceResult = balanceResult;
		StartCoroutine(CheckBalance());
	}

	IEnumerator CheckBalance()
	{
		StringBuilder sb = new StringBuilder(balanceUrl);
		sb.Append("?openid=").Append(PlayerInfo["openid"]);
		sb.Append("&openkey=").Append(PlayerInfo["accessToken"]);
		sb.Append("&pay_token=").Append(PlayerInfo["payToken"]);
		sb.Append("&pf=").Append(PlayerInfo["pf"]);
		sb.Append("&pfkey=").Append(PlayerInfo["pfkey"]);
		sb.Append("&zoneid=1");
		sb.Append("&mediaType=1");     //1：QQ 2: WX(微信)

		string wholeUrl = sb.ToString();
		string sig = HMACSHAEncoding(wholeUrl.Substring(wholeUrl.IndexOf("/tencent")), TencentConfigData.appKey + "&");
		sb.Append("&sig=").Append(sig);
		
		//Debug.Log("url = "  + sb.ToString());
		
		WWW www = new WWW(sb.ToString());
		yield return www;
		if(www.isDone)
		{
			if(!string.IsNullOrEmpty(www.error))
			{
				TraceUtil.Log("网络错误。。。。");
				if(OnBalanceResult != null)
				{
					OnBalanceResult(null);
				}
				yield break;
			}
		}

		//Debug.Log("//////////////////payment msg = " + www.text);
		var purchaseInfo = JsonConvertor<PurchaseInfo>.Json2Object(www.text);
		//Debug.Log("msg = " + purchaseInfo.balance);
		if(purchaseInfo != null && OnBalanceResult != null)
		{
			OnBalanceResult(purchaseInfo);
		}

	}

	// 购买
	public void Purchase(Action<PurchaseInfo> resultCallback, params object[] purchaseArgs)
	{
		OnPurchaseResult = resultCallback;
		CurrencyNum = (float)purchaseArgs[0];
		GoldNum = (int)purchaseArgs[1];
		StartCoroutine("PurchaseThroughPHP");
	}

	IEnumerator PurchaseThroughPHP()
	{
		StringBuilder sb = new StringBuilder(payUrl);
		//sb.Append("?servIp=").Append(LoginManager.Instance.LoginSSActorInfo.SZServerIP);
		//sb.Append("&servPort=").Append(LoginManager.Instance.LoginSSActorInfo.wPort.ToString());
		sb.Append("?actorId=").Append(LoginManager.Instance.LoginSSActorInfo.lActorID.ToString());
		sb.Append("&gameWorldId=").Append(JHPlatformConnManager.Instance.GameWorldId.ToString());
		sb.Append("&platformMoneyNum=").Append(CurrencyNum.ToString());
		sb.Append("&payNum=").Append(GoldNum.ToString());
		sb.Append("&openid=").Append(PlayerInfo["openid"]);
		sb.Append("&openkey=").Append(PlayerInfo["accessToken"]);
		sb.Append("&pay_token=").Append(PlayerInfo["payToken"]);
		sb.Append("&pf=").Append(PlayerInfo["pf"]);
		sb.Append("&pfkey=").Append(PlayerInfo["pfkey"]);
		sb.Append("&zoneid=1");
		sb.Append("&mediaType=1");     //1：QQ 2: WX(微信)

		string wholeUrl = sb.ToString();
		string sig = HMACSHAEncoding(wholeUrl.Substring(wholeUrl.IndexOf("/tencent")), TencentConfigData.appKey + "&");
		sb.Append("&sig=").Append(sig);

		//Debug.Log("url = "  + sb.ToString());

		WWW www = new WWW(sb.ToString());
		LoadingUI.Instance.Show();
		yield return www;
		LoadingUI.Instance.Close();
		if(www.isDone)
		{
			if(!string.IsNullOrEmpty(www.error))
			{
				TraceUtil.Log("网络错误。。。。");
				OnPurchaseResult(null);
				yield break;
			}
		}

		//Debug.Log("//////////////////payment msg = " + www.text);
		var purchaseInfo = JsonConvertor<PurchaseInfo>.Json2Object(www.text);
		if(purchaseInfo != null)
		{
			OnPurchaseResult(purchaseInfo);
		}
	}

	// 使用 HMACSHA1编码，用于链接验证
	public string HMACSHAEncoding(string source, string key)
	{
		HMACSHA1 mySHA = new HMACSHA1(Encoding.UTF8.GetBytes(key));
		byte[] result = mySHA.ComputeHash(Encoding.UTF8.GetBytes(source));
		
		StringBuilder sb = new StringBuilder();
		foreach(byte b in result)
		{
			sb.AppendFormat("{0:x2}", b);
		}

		return sb.ToString();
	}

	public void ChargeMoney(Action<bool> payResult)
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)  
		#if ANDROID_TENCENT
		jo.Call("ChargeGameCoin");
		OnPaymentResult = payResult;
		#endif 
		#endif
	}

    /// <summary>
    /// 通知
    /// </summary>
    /// <param name="ticker">通知小提示</param>
    /// <param name="title">通知标题</param>
    /// <param name="content">通知内容</param>
    /// <param name="waitingTime">Waiting time.</param>
    public void Notify(string ticker, string title, string content, int waitingTime)
    {
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		#if ANDROID_TENCENT 
		jo.Call("Notify",ticker,title,content,waitingTime);
		#endif
		#endif
    }
    /// <summary>
    ///这个不用 
    /// </summary>
    public void DoShowSprite()
    {
       
    }
    /// <summary>
    /// 这个不用 
    /// </summary>
    public void DoHideSprite()
    {
    
    }
    public void ExtendInfoSubmit(string service, string role, string grade)
    {

    }
    public void ShowPlatformInfo()
    {

    }
   
    #endregion

	#region Android工程回调
	public void LoginSuccessCallback(string param)
	{
		string[] infos = param.Split('|');
		foreach(string info in infos)
		{
			string[] KVpair = info.Split(':');
			if(!PlayerInfo.ContainsKey(KVpair[0]))
			{
				PlayerInfo.Add(KVpair[0], KVpair[1]);
			}
		}

		PlatformLoginBehaviour.message += " LoginOK:";
		if(OnLoginResult != null)
		{
			OnLoginResult(true);
		}

		//Debug.Log("Login Success ......................");
	}

	public void LoginFailCallback(string param)
	{
		if(OnLoginResult != null)
		{
			OnLoginResult(false);
		}
	}

	/*
	public void PayProcessFail()
	{
		if(OnPayProcessResult != null)
		{
			OnPayProcessResult(false);
		}
	}*/
	public void PayResultSucess()
	{
		if(OnPaymentResult != null)
		{
			OnPaymentResult(true);
		}
	}
	public void PayResultFail()
	{
		if(OnPaymentResult != null)
		{
			OnPaymentResult(false);
		}
	}

	#endregion
}

#region 购买返回信息
public enum PurchaseFlag
{
	Success = 0,						// 购买成功
	DeliveryFail = 1,				// 支付成功，发货失败
	PurchaseFail = 2,				// 购买失败
	BalanceNotEnough = 3,	// 余额不足
	NetworkError = 4,			//  网络错误（具体原因看msg）
}

public class PurchaseInfo
{
	public int ret;						// 对应PurchaseFlag
	public int tencentCode;			// 腾讯返回码
	public string billno;				// 账单号（支付成功才有意义）
	public int balance;					// 余额
	public string msg;					// 详细信息
}

#endregion
