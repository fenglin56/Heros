using UnityEngine;
using System.Collections;
using System;

public class OPPOPlatformcallbackHandle : View,IPlatformSDKCallback 
{

#if (UNITY_ANDROID && !UNITY_EDITOR)  
    #if ANDROID_OPPO      
    AndroidJavaClass jc = null;

    AndroidJavaObject jo = null;
    #endif 
#endif
    private string m_message;

    private Action<bool> OnInitResult;
    private Action<bool> OnLoginResult;
    private Action<bool,string> OnUserJsonInfo;
    private Action<PhpBackObj> OnRequestPHPResult;
    private Action<bool> OnPaymentResult;
    private OPPOConfigData m_OPPOConfigData;
    private string tokenKey, tokenSecret;

    private long m_payerOrder = 0;
    private string m_productDesc = "铜币描述";
    private string m_productName = "铜币";
    private string m_payType = "Fixed";
    private int m_Count = 150;
    private bool HasInit;
    private bool m_OnApplicationFocus;
    void Awake()
    {
        RegisterEventHandler();
    }

    void OnApplicationFocus(bool focus)
    {
        m_OnApplicationFocus=focus;
        if(HasInit)
        {

        if(focus)
        {
            Debug.Log("返回游戏");
            if(GameManager.Instance.CurrentState==GameManager.GameState.GAME_STATE_TOWN||GameManager.Instance.CurrentState==GameManager.GameState.GAME_STATE_LOGIN)
            {
                DoShowSprite();
            }
          
        }
//        else
//        {
//            Debug.Log("home键");
//            DoHideSprite();
//        }
        }
    }
    public OPPOConfigData OPPOConfigData
    {
        get { 
            if(m_OPPOConfigData==null)
            {
                m_OPPOConfigData=(OPPOConfigData)PlatformConfig;
            }
            return m_OPPOConfigData;
        }
    }

    public PlatformType PlatformType{get;set;}

    private void SceneChangeHandle(INotifyArgs e)
    {
        if(GameManager.Instance.CurrentState==GameManager.GameState.GAME_STATE_TOWN||GameManager.Instance.CurrentState==GameManager.GameState.GAME_STATE_LOGIN)
        {
            DoShowSprite();
        }
        else
        {
            DoHideSprite();
        }
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.StateChange.ToString(), SceneChangeHandle);
    }
    void OnDestroy()
    {
        RemoveEventHandler(EventTypeEnum.StateChange.ToString(), SceneChangeHandle);
    }
    public void InitPlatform(Action<bool> initResult)
    {
        //OnInitResult = initResult;
        //TraceUtil.Log("OPPO Init finish!");
        ////OPPO初始化回来没有回调，用协同产生回调
        //StartCoroutine(OPPOInitBack());
#if (UNITY_ANDROID && !UNITY_EDITOR) 
#if ANDROID_OPPO  
        OnInitResult = initResult;
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        PlatformLoginBehaviour.message += " Call SDK_init";
        jo.Call("SDK_init", OPPOConfigData.appKey, OPPOConfigData.appSecret,false);

        //OPPO初始化回来没有回调，用协同产生回调
        StartCoroutine(OPPOInitBack());
#endif
#endif
    }
    private IEnumerator OPPOInitBack()
    {
        PlatformLoginBehaviour.message += " SDK_init Back";
        yield return new WaitForSeconds(1.5f);
        if (OnInitResult != null)
        {
            OnInitResult(true);
            HasInit=true;
            //DoShowSprite();

        }
    }
    public void ExtendInfoSubmit(string service, string role, string grade)
    {
        #if (UNITY_ANDROID && !UNITY_EDITOR) 
        #if ANDROID_OPPO  
        jo.Call("SDK_doExtendInfoSubmit",OPPOConfigData.gameId,service,role,grade);
        #endif
        #endif
    }

    public void DoShowSprite()
    {
        if(m_OnApplicationFocus)
        {
        #if (UNITY_ANDROID && !UNITY_EDITOR) 
        #if ANDROID_OPPO  
        jo.Call("SDK_doShowSprite");
        #endif
        #endif
        }
    }
    public void DoHideSprite()
    {
        #if (UNITY_ANDROID && !UNITY_EDITOR) 
        #if ANDROID_OPPO  
        jo.Call("SDK_doDissmissSprite");
        #endif
        #endif
    
    }
    public void LoginPlatform(Action<bool> loginResult)
    {
        //OnLoginResult = loginResult;
        //TraceUtil.Log("OPPO 开始登录平台!");
        //onNearMeLoginOKCallback("OK");
#if (UNITY_ANDROID && !UNITY_EDITOR)      
#if ANDROID_OPPO  
        OnLoginResult = loginResult;
        PlatformLoginBehaviour.message += " Call SDK_LoGin";
        jo.Call("SDK_doLogin");
#endif
#endif
    }

    public void GetUserInfo(Action<bool,string> userJsonInfo)
    {
        //OnUserJsonInfo = userJsonInfo;
        //TraceUtil.Log("OPPO 获得用户信息!");
        //onNearMeGetUserInfoOKCallback("OK");
#if (UNITY_ANDROID && !UNITY_EDITOR)  
#if ANDROID_OPPO 
        OnUserJsonInfo = userJsonInfo;
        jo.Call("SDK_doGetUserInfo");
#endif
#endif
    }

    public void RequestPHP(Action<PhpBackObj> requestPHPResult)
    {
        //OnRequestPHPResult = requestPHPResult;
        //TraceUtil.Log("OPPO 开始请求PHP服务器!");
        //StartCoroutine(AuthorPHP(tokenKey, tokenSecret));
 #if (UNITY_ANDROID && !UNITY_EDITOR)  
#if ANDROID_OPPO 
        OnRequestPHPResult = requestPHPResult;
        StartCoroutine(AuthorPHP(tokenKey, tokenSecret));
#endif
#endif
    }

    /// <summary>
    /// 支付接口 payArg用#分割，分别是money callbackUrl orderId productDesc productName  payType count
    /// </summary>
    /// <param name="paymentResult"></param>
    /// <param name="payArgs"></param>
    public void Payment(Action<bool> paymentResult, params object[] payArgs)
    {

#if (UNITY_ANDROID && !UNITY_EDITOR)  
#if ANDROID_OPPO 

        OnPaymentResult = paymentResult;
        m_payerOrder = System.DateTime.Now.Ticks + UnityEngine.Random.Range(0, 1000);
        string money=payArgs[0].ToString();
        string productDesc=payArgs[1].ToString();
        string productName=payArgs[2].ToString();
        string payType="Fixed";
        string Pro_count=payArgs[3].ToString();
        string callBack="http://jh.fanhougame.net/oppo/payCallback.php";
            var payArg = money+"#"+callBack+ "#"+m_payerOrder.ToString()+"#"+productDesc+"#"+productName+"#"+payType+"#"+Pro_count;
        jo.Call("SDK_doPayment", payArg);
#endif
#endif
    }
    public void Notify(string ticker, string title, string content, int waitingTime)
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_OPPO 
        jo.Call("Notify",ticker,title,content,waitingTime);
#endif
#endif
    }

    public void ShowPlatformInfo()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_OPPO 
        jo.Call("SDK_doShowGameCenter");
#endif
#endif
    }

    public PlatformConfigBase PlatformConfig{get;set; }

    #region Android工程回调
    void onNearMeLoginOKCallback(string message) 
	{
        PlatformLoginBehaviour.message += " LoginOK:" + message;
        if (OnLoginResult != null)
        {
            OnLoginResult(true);
        }
	}
    void onNearMeLoginFaildCallback(string message)
    {
        m_message += " LoginFailed:" + message;
        if (OnLoginResult != null)
        {
            OnLoginResult(false);
        }
    }
    void onNearMeReloginOKCallback(string message)
    {   
        GameManager.Instance.QuitToLogin();
    }
    void onNearMeReLoginFaildCallback()
    {

    }
    void onNearMeGetUserInfoOKCallback(string message)
    {
        string[] messgeArr = message.Split('#');
        int code = int.Parse(messgeArr[0]);
        string userId = messgeArr[1];
       
        tokenKey = messgeArr[2];
        tokenSecret = messgeArr[3];
        PlatformLoginBehaviour.message = " UserInfo:" + tokenKey + "  " + tokenSecret;

        LoginManager.Instance.Account = userId;
        LoginManager.Instance.Pwd = "123456";

        if (OnUserJsonInfo != null)
        {
            OnUserJsonInfo(true,message);
        }
    }
    private IEnumerator AuthorPHP(string tokenKey,string tokenSecret)
    {
        WWW www = new WWW("http://jh.fanhougame.net/oppo/demo.php?tokenSecret=" + tokenSecret + "&token=" + tokenKey);
        yield return www;
        if (www.isDone)
        {
            //m_message="Statue:"+www.error;
            //m_message+=www.text;
            if (OnRequestPHPResult != null)
            {
                PlatformLoginBehaviour.message = "PHP BackInfo:" + www.text.Replace("\"", "'");
                var jsonBackObj = JsonConvertor<PhpBackObj>.Json2Object(www.text.Replace("\"","'"));
                PlatformLoginBehaviour.message += jsonBackObj.ServerInfo.Length;
                OnRequestPHPResult(jsonBackObj);
            }
        }
    }
    void onNearMeGetUserInfoFailureCallback(string message)
    {
        PlatformLoginBehaviour.message += " GetUserInfoFailure:" + message;
    }
   
    void onNearMePaymentOKCallback(string message)
    {
        if (OnPaymentResult != null)
        {
            OnPaymentResult(true);
        }
    }
    void onNearMePaymentFailedCallback(string message)
    {
        if (OnPaymentResult != null)
        {
            OnPaymentResult(false);
        }
    }
    #endregion


    public void ResetAndroidCallGameObject(ref GameObject defaultObject)
    {
        
    }   
}
