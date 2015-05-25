using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class OPPOConfigData : PlatformConfigBase
{
    public OPPOConfigData()
    {
        CallbackGameObjectName = "OPPOSDK";
        PhpLoginAuthorURL = "http://jh.fanhougame.net/oppo/demo.php?tokenSecret={0}&token={1}";
    }
    public const string appKey = "36SxjfY56Tc0g0OoCo4OwosSo";
    public const string appSecret = "8386DFFdD9077Ef25202fe96E21a72f9";
    public const string gameId="2297";
    
}

public class TencentConfigData: PlatformConfigBase
{
	public TencentConfigData()
	{
		CallbackGameObjectName = "MSDKReceiver";
	}

	public const string appId = "1103477340";
	public const string appKey = "vzWOjkXRGrOWy3Sw";

	// 联调支付环境
	public const string Environment = "test";

	//正式环境
	//public const string Environment = "release";
}

public class UCConfigData : PlatformConfigBase
{
    public UCConfigData()
    {
        PhpLoginAuthorURL = "http://jh.fanhougame.net/HttpServer/loginCheck.php?sid={0}";
    }
    //联调环境参数
    //public static int cpid = 20087;
    //public static int gameid = 119474;
    //public static int serverid = 1333;
    //public static string servername = "test1";

    //正式环境参数
    public const int cpid = 22353;
    public const int gameid = 536608;
    public const int serverid = 2590;
    public const string servername = "饭后江湖";


    public const bool debugMode = true;        //是否联调模式， false=连接SDK的正式生产环境，true=连接SDK的测试联调环境
    public const int logLevel = UCConstants.LOGLEVEL_DEBUG;    //日志级别：0=错误信息级别，记录错误日志，1=警告信息级别，记录错误和警告日志， 2=调试信息级别，记录错误、警告和调试信息，为最详尽的日志级别 
    public const int orientation = ScreenModel.ORIENTATION_LANDSCAPE; //竖屏 ORIENTATION_PORTRAIT = 0;  横屏 ORIENTATION_LANDSCAPE = 1;
    public const bool enablePayHistory = false;        //是否启用支付查询功能
    public const bool enableLogout = false;        //是否启用用户切换功能


    public const bool inited = false;
    public const int initRetryTimes = 0;
    public const bool logined = false;

    public const int loginUISwitch = UCConstants.USE_WIDGET;
}
public class MIConfigData : PlatformConfigBase
{
    public MIConfigData()
    {
        this.PhpLoginAuthorURL = "http://jh.fanhougame.net/xiaomi/loginCheck.php";
        this.CallbackGameObjectName = "MiSDK";
    }
    //正式环境参数
    public const int appId = 24308;
    public const string appKey = "374573d5-5ddb-7307-2127-5327bb9287b4";
    public const string SDK_JAVA_CLASS = "com.Pushcraft.mi.MainActivity";

    public const int orientation = ScreenModel.ORIENTATION_LANDSCAPE; //竖屏 ORIENTATION_PORTRAIT = 0;  横屏 ORIENTATION_LANDSCAPE = 1;
}
public abstract class PlatformConfigBase
{
    public string CallbackGameObjectName="SDKCallbackGO";
    public string PhpLoginAuthorURL = "";
}
