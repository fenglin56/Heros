using UnityEngine;
using System.Collections;

/// <summary>
/// 调用SDK和回调消息响应可能需要用到的常量
/// </summary>
public class UCConstants
{

	//错误信息级别，记录错误日志
	public const int LOGLEVEL_ERROR = 0;
	//警告信息级别，记录错误和警告日志
	public const int LOGLEVEL_WARN = 1;
	//调试信息级别，记录错误、警告和调试信息，为最详尽的日志级别
	public const int LOGLEVEL_DEBUG = 2;


	//竖屏
	//public const int ORIENTATION_PORTRAIT = 0;
	//横屏
	//public const int ORIENTATION_LANDSCAPE = 1;
	
	
	// 游戏默认使用"简版"登录界面
    public const int DEFAULT = 0;
    // 简版登录界面
    public const int USE_WIDGET = 1;
    // 标准版登录界面
    public const int USE_STANDARD = 2;



	//回调消息类型
	//初始化结果回调
	public const string CALLBACKTYPE_InitSDK = "InitSDK";

	//登录结果回调
	public const string CALLBACKTYPE_Login = "Login";

	//退出登录回调
	public const string CALLBACKTYPE_Logout = "Logout";

	//悬浮菜单通知回调
	public const string CALLBACKTYPE_FloatMenu = "FloatMenu";

	//九游社区（个人中心）通知回调
	public const string CALLBACKTYPE_UserCenter = "UserCenter";

	//通过 EnterUI 方法打开的SDK界面通知回调
	public const string CALLBACKTYPE_EnterUI = "EnterUI";

	//充值下单结果回调
	public const string CALLBACKTYPE_Pay = "Pay";

	//U点充值界面关闭回调
	public const string CALLBACKTYPE_UPointCharge = "UPointCharge";

	//判断是否UC会员结果回调
	public const string CALLBACKTYPE_IsUCVip = "IsUCVip";

	//获取UC会员信息结果回调
	public const string CALLBACKTYPE_GetUCVipInfo = "GetUCVipInfo";


}
