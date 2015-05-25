using UnityEngine;
using System.Collections;
using System;

public class MiPlatformCallbackHandle :MonoBehaviour, IPlatformSDKCallback
{

#if (UNITY_ANDROID && !UNITY_EDITOR)           
    AndroidJavaClass miJc = null;
    AndroidJavaObject miJo = null;
#endif
    private Action<bool> OnInitBackResult;
    private Action<bool> OnLoginBackResult;
    private Action<bool, string> OnUserJsonInfoBack;
    private Action<PhpBackObj> OnRequestPHPBackResult;
    private Action<bool> OnPaymentBackResult;
    private MIConfigData m_MIConfigData;

    private string m_uid =  string.Empty;

    public MIConfigData MIConfigData
    {
        get
        {
            if (m_MIConfigData == null)
            {
                m_MIConfigData = (MIConfigData)PlatformConfig;
            }
            return m_MIConfigData;
        }
    }

    public PlatformType PlatformType
    {
        get;
        set;
    }

    public void InitPlatform(System.Action<bool> initResult)
    {
        
#if (UNITY_ANDROID && !UNITY_EDITOR)

        PlatformLoginBehaviour.message += "  Miinit";
        OnInitBackResult = initResult;
        miJc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        PlatformLoginBehaviour.message += "  MiJCCCCCC";
        miJo = miJc.GetStatic<AndroidJavaObject>("currentActivity");
        PlatformLoginBehaviour.message += "  MiJOOOOOOO";

        miJo.Call("InitSDK",this.name, MIConfigData.appId, MIConfigData.appKey, MIConfigData.orientation);
        //MiGameSdk.InitSDK(m_MIConfigData.CallbackGameObjectName, MIConfigData.appId, MIConfigData.appKey, MIConfigData.orientation);
        StartCoroutine(MiInitBack());

#endif
    }

    private IEnumerator MiInitBack()
    {
        yield return new WaitForSeconds(1.5f);

        if (OnInitBackResult != null)
            OnInitBackResult(true);
    }

    public void LoginPlatform(System.Action<bool> loginResult)
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)

        OnLoginBackResult = loginResult;
        miJo.Call("Login");
        //MiGameSdk.Login();

#endif
    }

    public void GetUserInfo(System.Action<bool, string> userJsonInfo)
    {
        OnUserJsonInfoBack = userJsonInfo;

        if(OnUserJsonInfoBack != null)
            OnUserJsonInfoBack(true, m_uid);
    }

    public void DoShowSprite()
    {
        //throw new NotImplementedException();
    }
    public void DoHideSprite()
    {
       // throw new NotImplementedException();
    }
    public void ExtendInfoSubmit(string service, string role, string grade)
    {
        //throw new NotImplementedException();
    }
    public void RequestPHP(System.Action<PhpBackObj> requestPHPResult)
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
        OnRequestPHPBackResult = requestPHPResult;
        StartCoroutine(AuthorPHP());
#endif
    }

    private IEnumerator AuthorPHP()
    {
        WWW www = new WWW("http://jh.fanhougame.net/xiaomi/loginCheck.php");
        yield return www;
        if (www.isDone)
        {
            if (OnRequestPHPBackResult != null)
            {
                var jsonBackObj = JsonConvertor<PhpBackObj>.Json2Object(www.text.Replace("\"", "'"));
                PlatformLoginBehaviour.message += "jsonBackObj:: " + jsonBackObj;
                OnRequestPHPBackResult(jsonBackObj);
            }
        }
    }

    public void Payment(System.Action<bool> paymentResult, params object[] payArgs)
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)

        OnPaymentBackResult = paymentResult;

        var playerData = PlayerManager.Instance.FindHeroDataModel();
        //没有正式传参, 暂时使用临时数据
        string orderId = (string.Format("{0:0.}", 10000000000000000000 + UnityEngine.Random.Range(0, 2000000000000000000)));
        miJo.Call("Pay",orderId, 1, playerData.Name, playerData.ActorID.ToString());
        //MiGameSdk.Pay(orderId, 1, playerData.Name, playerData.ActorID.ToString());
#endif
    }

    public void Notify(string ticker, string title, string content, int waitingTime)
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_XIAOMI 
        jo.Call("Notify",ticker,title,content);
#endif
#endif
    }
    public void ShowPlatformInfo()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_XIAOMI 
        //jo.Call("SDK_doShowGameCenter");
#endif
#endif
    }
    private void OnPurchaseCancelled(string erroCode)
    {
        if (OnPaymentBackResult != null)
            OnPaymentBackResult(false);

    }

    private void OnPurchaseFailed(string erroCode)
    {
        if (OnPaymentBackResult != null)
            OnPaymentBackResult(false);

    }

    private void OnPurchaseSuccedded(string erroCode)
    {
        if (OnPaymentBackResult != null)
            OnPaymentBackResult(true);
    }

    private void OnLoginResult(string data)
    {
        //debugInfo = "onLoginResult" + data;
        string[] strdata = data.Split('|');
        switch (strdata[0])
        {
            case "0": //login success

                m_uid = strdata[1];
            //LoginPlatformManager.Instance.PlayerId = long.Parse(strdata[1]);
                if (OnLoginBackResult != null)
                    OnLoginBackResult(true);
                break;
            case "1"://login failed
                //Login.Instance.PlayerId = 0;
                if (OnLoginBackResult != null)
                    OnLoginBackResult(false);
                break;
            case "2"://cancel
                //LoginPlatformManager.Instance.PlayerId = 0;
                if (OnLoginBackResult != null)
                    OnLoginBackResult(false);
                break;
        }
    }



    public PlatformConfigBase PlatformConfig
    {
        get;
        set;
    }
}
