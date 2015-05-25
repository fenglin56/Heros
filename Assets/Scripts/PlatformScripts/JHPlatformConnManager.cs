using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// 江湖对接平台管理器，
/// 
/// 
/// </summary>
public class JHPlatformConnManager
{
    /// <summary>
    /// 获得服务器列表Json，触发此委托，由UI的服务器列表负责监听
    /// </summary>
    public Action<PhpBackObj> OnGetServerListJson;
    /// <summary>
    /// 支付结果返回
    /// </summary>
    public Action OnPaymentSuccess;
    private bool KekeSpriteIsShow;
    private bool m_platformReady = false;
    private PlatformType m_platformType;
    private IPlatformSDKCallback m_platformcallbackHandle;
    private PlatformConfigBase m_platformConfigBase;

	public int GameWorldId{get; set;}
    #region 单例
    private static JHPlatformConnManager m_instance;
    public static JHPlatformConnManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new JHPlatformConnManager();
            }
            return m_instance;
        }
    }
    #endregion

    /// <summary>
    /// 游戏启动平台接入口，由GameManager在Start调用。进行一系列初始化
    /// </summary>
    /// <param name="platformType"></param>
    /// <returns></returns>
    public void InitPlatform(PlatformType platformType, Action<bool> initResult)
    {
        m_platformType = platformType;
        //根据平台类型初始化连接信息。
        //创建接受SendMessage的游戏物体，挂载回调处理脚本。
		if(m_platformcallbackHandle == null)
		{
        	InitPlatformCallback(platformType);
		}
        PlatformLoginBehaviour.message += "  m_platformConfigBase.CallbackGameObjectName:" + m_platformConfigBase.CallbackGameObjectName + "  " + (GameObject.Find(m_platformConfigBase.CallbackGameObjectName) == null);

        PlatformLoginBehaviour.message += " COnfig Init finish";
        m_platformcallbackHandle.InitPlatform((successed) =>
        {
            
            if (successed)
            {
            }
            else
            {
            }
            if (initResult != null)
            {
                initResult(successed);
                //m_platformcallbackHandle.DoShowSprite();
            }
        });
        m_platformReady = true;
    }
    private void InitPlatformCallback(PlatformType platformType)
    {
        GameObject handleObject = new GameObject();
        handleObject.AddComponent<DontDestroy>();
        switch (platformType)
        {
            case PlatformType.OPPO:
                m_platformConfigBase = new OPPOConfigData();
                m_platformcallbackHandle = handleObject.AddComponent<OPPOPlatformcallbackHandle>();
                break;
            case PlatformType.MI:
                m_platformConfigBase = new MIConfigData();
                m_platformcallbackHandle = handleObject.AddComponent<MiPlatformCallbackHandle>();
                break;
            case PlatformType.Tencent:
                m_platformConfigBase = new TencentConfigData();
                m_platformcallbackHandle = handleObject.AddComponent<TencentPlatformCallbackhandle>();
                break;
            default:
                goto case PlatformType.OPPO;
        }
        m_platformcallbackHandle.PlatformConfig = m_platformConfigBase;
        handleObject.name = m_platformConfigBase.CallbackGameObjectName;
        m_platformcallbackHandle.PlatformType = platformType; 
    }
    /// <summary>
    /// 登录平台
    /// </summary>
    /// <returns></returns>
    public void LoginToPlatform(Action<bool> loginResult)
    {
        m_platformcallbackHandle.LoginPlatform((successed) =>
        {
            if (successed)
            {
            }
            else
            {
            }
            if (loginResult != null)
            {
                loginResult(successed);
            }
        });
    }
    /// <summary>
    /// 获得用户信息
    /// </summary>
    /// <returns></returns>
    public void GetUserInfo(Action<bool,string> userJsonInfo, params object[] args)
    {
        m_platformcallbackHandle.GetUserInfo((flag,userInfo) =>
        {
            if (userJsonInfo != null)
            {
                userJsonInfo(flag,userInfo);
            }
        });
    }
    /// <summary>
    /// 请求Php服务器列表
    /// </summary>
    /// <returns></returns>
    public void RequestPHP(Action<PhpBackObj> requestPHPResult)
    {
        m_platformcallbackHandle.RequestPHP((phpBackInfo) =>
        {
            if (requestPHPResult != null)
            {
                requestPHPResult(phpBackInfo);
            }
        });
    }
    /// <summary>
    /// 购买支付
    /// </summary>
    /// <returns></returns>
    public void Payment(Action<bool> paymentResult, params object[] args)
    {
        m_platformcallbackHandle.Payment((successed) =>
        {
            if (successed)
            {
            }
            else
            {
            }
            if (paymentResult != null)
            {
                paymentResult(successed);
            }
        }, args);
    }

	// Tencent 查询钻石余额
	public void TencentBalance(Action<PurchaseInfo> balanceResult)
	{
		if(m_platformcallbackHandle is TencentPlatformCallbackhandle)
		{
			var tencentPlatform = (TencentPlatformCallbackhandle)m_platformcallbackHandle;
			tencentPlatform.GetBalance(balanceResult);
		}
	}

	//Tencent 平台购买元宝
	public void TencentPurchase(Action<PurchaseInfo> purchaseResult, params object[] args)
	{
		if(m_platformcallbackHandle is TencentPlatformCallbackhandle)
		{
			var tencentPlatform = (TencentPlatformCallbackhandle)m_platformcallbackHandle;
			tencentPlatform.Purchase(purchaseResult, args);
		}
	}
	
	// Tencent 充值
	public void ChargeMoney(Action<bool> payResult)
	{
		if(m_platformcallbackHandle is TencentPlatformCallbackhandle)
		{
			var handle = (TencentPlatformCallbackhandle)m_platformcallbackHandle;
			handle.ChargeMoney(payResult);
		}
	}

    /// <summary>
    /// 通知
    /// </summary>
    /// <param name="ticker"></param>
    /// <param name="title"></param>
    /// <param name="content"></param>
    public void Notify(string ticker, string title, string content,int waitingTime)
    {
        m_platformcallbackHandle.Notify(ticker, title, content,waitingTime);
    }
    /// <summary>
    /// 点击平台图标，显示平台内容
    /// </summary>
    public void ShowPlatformInfo()
    {
        m_platformcallbackHandle.ShowPlatformInfo();
    }

    public void HideKekeSprite()
    {
        if(KekeSpriteIsShow)
        {
        m_platformcallbackHandle.DoHideSprite();
            KekeSpriteIsShow=false;
        }
    }
    public void ShowKekeSprite()
    {
        if(!KekeSpriteIsShow)
        {
            m_platformcallbackHandle.DoShowSprite();
            KekeSpriteIsShow=true;
        }
    }
    public void ExtendInfoSubmi(string server,string role,string level)
    {
        m_platformcallbackHandle.ExtendInfoSubmit(server,role,level);
    }
}
public enum PlatformType
{
    Local,
    UC,         //UC
    MI,         //小米
    OPPO,       //OPPO可可
    Tencent,//腾讯
}

/// <summary>
/// 平台回调接口，由游戏实现。并挂在场景DontDestroy游戏物体上
/// </summary>
public interface IPlatformSDKCallback
{
    PlatformConfigBase PlatformConfig{get;set;}
    PlatformType PlatformType { get; set; }
    /// <summary>
    /// 初始化平台
    /// </summary>
    /// <param name="initResult"></param>
    void InitPlatform(Action<bool> initResult);
    /// <summary>
    /// 登录平台
    /// </summary>
    /// <param name="loginResult"></param>
    void LoginPlatform(Action<bool> loginResult);
    /// <summary>
    /// 获得用户信息
    /// </summary>
    /// <param name="userJsonInfo"></param>
    void GetUserInfo(Action<bool,string> userJsonInfo);
    /// <summary>
    /// 调用PHP服务器
    /// </summary>
    /// <param name="requestPHPResult"></param>
    void RequestPHP(Action<PhpBackObj> requestPHPResult);
    /// <summary>
    /// 支付接口
    /// </summary>
    /// <param name="paymentResult"></param>
    /// <param name="payArgs"></param>
    void Payment(Action<bool> paymentResult, params object[] payArgs);
    /// <summary>
    /// 通知
    /// </summary>
    /// <param name="ticker">通知小提示</param>
    /// <param name="title">通知标题</param>
    /// <param name="content">通知内容</param>
    void Notify(string ticker, string title, string content, int waitingTime);

    void DoShowSprite();

    void DoHideSprite();
   /// <summary>
   /// Extends the info submit.
   /// </summary>
   /// <param name="gameId">Game identifier.</param>
   /// <param name="service">Service.</param>
   /// <param name="role">Role.</param>
   /// <param name="grade">Grade.</param>
    void ExtendInfoSubmit(String service,String role,String grade);

    void ShowPlatformInfo();
}
public class PhpBackObj
{
    public string ErrorCode;
    public string ErrorDesc;
    //public string UCID;
    public Server[] ServerInfo;
}

[Serializable]
public class Server
{
    public int No;     							//	服务器编号
    public string Name; 						//	服务器名称
	public int Version;							// 客户端和服务器比对版本号
    public string IP;   							//	服务器Ip
    public int Port;    							//	服务器端口
    public int ActorNumber;  				//	角色数量，用于显示角色数量，并用于判断服务器是否忙碌。服务器会给最大角色并存数，及进入忙碌状态的角色数
    public int Status;							//	2=繁忙，1=轻松，0=维护中
    public int Recommend_status; 		//	推荐  0不推荐 1推荐
}
