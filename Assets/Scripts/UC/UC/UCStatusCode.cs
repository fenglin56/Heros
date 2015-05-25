using System.Collections;

/// <summary>
/// 定义 SDK 方法调用的返回状态码
/// </summary>
public class UCStatusCode
{

	// 调用成功
	public const int SUCCESS = 0;

	// 调用失败
	public const int FAIL = -2;

	// 没有初始化后不允许执行
	public const int NO_INIT = -10;

	// 没有登录后不允许执行
	public const int NO_LOGIN = -11;

	// 检测不了网咯不允许执行
	public const int NO_NETWORK = -12;

	// 初始化失败
	public const int INIT_FAIL = -100;

	// 登录失败
	public const int LOGIN_FAIL = -200;


	// 登录失败,需要输入内测码才能进行
	public const int LOGIN_ALPHA_CODE_MISSING = -201;


	// 游戏帐户密码错误导致登录失败
	public const int LOGIN_GAME_USER_AUTH_FAIL = -201;

	// 网络原因导致游戏帐户登录失败
	public const int LOGIN_GAME_USER_NETWORK_FAIL = -202;

	// 其他原因导致的游戏帐户登录失败
	public const int LOGIN_GAME_USER_OTHER_FAIL = -203;


	// 获取好友关系失败
	public const int GETFRINDS_FAIL = -300;

	// 获取用户是否会员时失败
	public const int VIP_FAIL = -400;

	// 获取用户会员特权信息时失败
	public const int VIPINFO_FAIL = -401;


	// 用户退出充值界面
	public const int PAY_USER_EXIT = -500;


	// 用户退出登录界面
	public const int LOGIN_EXIT = -600;


	// SDK界面将要显示
	public const int SDK_OPEN = -700;

	// SDK界面将要关闭，返回到游戏画面
	public const int SDK_CLOSE = -701;


	// 游客状态
	public const int GUEST = -800;


	// uc账户登录状态
	public const int UC_ACCOUNT = -801;


	// 退出游客试玩激活绑定页面回调状态码
	public const int BIND_EXIT = -900;

}
