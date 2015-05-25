using UnityEngine;
using System.Collections;


public class ScreenModel
{
    public const int ORIENTATION_PORTRAIT = 0;  //竖屏
    public const int ORIENTATION_LANDSCAPE = 1;   //横屏
}


//此类用于对参数进行集中管理
public static class UCConfig
{

	//联调环境参数
    //public static int cpid = 20087;
    //public static int gameid = 119474;
    //public static int serverid = 1333;
    //public static string servername = "test1";

	//正式环境参数
    public static int cpid = 22353;
    public static int gameid = 536608;
    public static int serverid = 2590;
    public static string servername = "饭后江湖";
    public static string PHP_PLATFORM_SERVER = "http://jh.fanhougame.net/HttpServer/loginCheck.php"; 

    public static bool debugMode = true;        //是否联调模式， false=连接SDK的正式生产环境，true=连接SDK的测试联调环境
    public static int logLevel = UCConstants.LOGLEVEL_DEBUG;    //日志级别：0=错误信息级别，记录错误日志，1=警告信息级别，记录错误和警告日志， 2=调试信息级别，记录错误、警告和调试信息，为最详尽的日志级别 
    public static int orientation = ScreenModel.ORIENTATION_LANDSCAPE; //竖屏 ORIENTATION_PORTRAIT = 0;  横屏 ORIENTATION_LANDSCAPE = 1;
    public static bool enablePayHistory = false;        //是否启用支付查询功能
    public static bool enableLogout = false;        //是否启用用户切换功能


	public static bool inited = false;
	public static int initRetryTimes = 0;
	public static bool logined = false;
	
	public static int loginUISwitch = UCConstants.USE_WIDGET;

}

public static class MiConfig
{
    //正式环境参数
    public static int appId = 24308;  
    public static string appKey = "374573d5-5ddb-7307-2127-5327bb9287b4";
    public static string SDK_JAVA_CLASS = "com.Pushcraft.mi.MainActivity";
    public static string PHP_PLATFORM_SERVER = "http://jh.fanhougame.net/xiaomi/loginCheck.php";

    public static int orientation = ScreenModel.ORIENTATION_LANDSCAPE; //竖屏 ORIENTATION_PORTRAIT = 0;  横屏 ORIENTATION_LANDSCAPE = 1;

}
