using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.Login;
using UI;
using System.Linq;

public class PlatformLoginBehaviour : View
{
    public Camera Camera;
    public GameObject PhpNoticeBoard;
    public GameObject LoginUIPanel, ServerListPanel, LoadingSceneUIPanel, CreatRoleUIPreafab, RoleSelectUIPrefab,LoginFailPanelPrefab;
    public NewSSUserLoginRes NewSSUserLoginRes { get; set; }
    public Server[] ServerInfo { get; set; }
    public string Account { get; set; }
    public string Pwd { get; set; }
    private Dictionary<LoginUIType, IUIPanel> UIList = new Dictionary<LoginUIType, IUIPanel>();

    private GameObject m_curUIPanel = null;
    private LoginUIType m_curUIType = LoginUIType.Loaing;

    public static string message = " Start";
    public static bool hasLogin=false;

	// tencent登陆超时处理，进入登陆状态开始计时，进入
#if (UNITY_ANDROID && !UNITY_EDITOR)  
#if ANDROID_TENCENT
	private const float LOGIN_TIME_OUT = 5f;
	private const int AUTO_CONNECT_TIMES = 1;

	private int autoTimes = 0;
	private float crtLoginTime = 0;
	private bool isRecord = false;
#endif
#endif

    void Awake()
    {
        if (GameManager.Instance.PlatformType == PlatformType.Local)
        {
            this.enabled = false;
            return;
        }
        this.RegisterEventHandler();
        LoginManager.Instance.ResetLoginButtonState();
        UIEventManager.Instance.RegisterUIEvent(UIEventType.ShowLodingUI, OpenMainUI);
		//UIEventManager.Instance.RegisterUIEvent(UIEventType.Login, Login);
    }
	// Use this for initialization
	void Start () {

        if(!hasLogin)
        {
            OpenMainUI(LoginUIType.Login);

        	JHPlatformConnManager.Instance.InitPlatform(GameManager.Instance.PlatformType, InitPlatformHandler);
        }
        else
        {
//            LoginManager.Instance.GotoPlatformButtonEnable = true;
//            LoginManager.Instance.GotoHttpServerButtonEnable=true;
//            LoginManager.Instance.GotoGameServerButtonEnable=true;
//            LoginManager.Instance.CreateActorButtonEnable=true;
//            LoginManager.Instance.EnterTownButtonEnable=true;
//            LoginManager.Instance.DeleteActorButtonEnable=true;
            OpenMainUI(LoginUIType.Login);
            JHPlatformConnManager.Instance.GetUserInfo(GetUserInfoHandler);
        }
    }

	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShowLodingUI, OpenMainUI);
    }
    void Update() 
	{
#if (UNITY_ANDROID && !UNITY_EDITOR)  
#if ANDROID_TENCENT
		if(!hasLogin && isRecord)
		{
			crtLoginTime += Time.deltaTime;
			if(crtLoginTime >= LOGIN_TIME_OUT)
			{ 
				crtLoginTime = 0;
				isRecord = false;
				if(autoTimes < AUTO_CONNECT_TIMES)
				{ 
					autoTimes++;
					JHPlatformConnManager.Instance.InitPlatform(GameManager.Instance.PlatformType, InitPlatformHandler);
				}
				else
				{
					MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_I32_1"),  LanguageTextManager.GetString("IDS_I32_2"), () => {
						SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
						JHPlatformConnManager.Instance.InitPlatform(GameManager.Instance.PlatformType, InitPlatformHandler);
					});
				}
			}
		}
#endif
#endif
	}

    void OnGUI()
    {
        //GUILayout.Label(message);
    } 
    void OpenMainUI(object obj)
    {
        m_curUIType = (LoginUIType)obj;
        CloseAllPanel();
        IUIPanel uiPanel = null;

        if (UIList.TryGetValue(m_curUIType, out uiPanel))
        {
            uiPanel.Show();
            m_curUIPanel = uiPanel.gameObject;
        }
        else
        {
            uiPanel = GetPanel(m_curUIType);
            m_curUIPanel = uiPanel.gameObject;

            uiPanel.Show();
            UIList.Add(m_curUIType, uiPanel);
        }
        if (m_curUIType == LoginUIType.CreatRole)
        {
            SetCamera();
        }
        if (m_curUIType == LoginUIType.CreatRole || m_curUIType == LoginUIType.SelectRole)
        {
            //SoundManager.Instance.StopBGM(0.0f);
            //SoundManager.Instance.PlayBGM("Music_UIBG_LoginCharacter", 0.0f);
        }
    }
    public void SetCamera()
    {
        var cameraData = LoginDataManager.Instance.GetCreateRoleUIData.Single(P => P._VocationID == 1);
        Camera.gameObject.SetActive(true);
        Camera.transform.position = cameraData._CameraPosition;
        Camera.transform.LookAt(cameraData._CameraTarget);
    }
    void CloseAllPanel()
    {
        if(this.Camera!=null)
        {
        this.Camera.gameObject.SetActive(false);
        }
        foreach (var child in UIList)
        {
            if (child.Value != null)
            {
                PlatformLoginBehaviour.message += " Child:" + child.Value.name;
                child.Value.Close();
            }
        }
    }
    IUIPanel GetPanel(LoginUIType loginUIType)
    {
        GameObject CreatPanelPrefab = null;
        switch (loginUIType)
        {
            case LoginUIType.Login:
                CreatPanelPrefab = LoginUIPanel;
			return CreatObjectToNGUI.InstantiateObj(CreatPanelPrefab,transform).GetComponent<PlatformLoginPanel>() as IUIPanel;
                break;
            case LoginUIType.ServerList:
                CreatPanelPrefab = ServerListPanel;
                break;
            case LoginUIType.CreatRole:
                CreatPanelPrefab = CreatRoleUIPreafab;
                break;
            case LoginUIType.SelectRole:
                CreatPanelPrefab = RoleSelectUIPrefab;
                break;
            case LoginUIType.Loaing:
                CreatPanelPrefab = LoadingSceneUIPanel;
                break;
            case LoginUIType.LoginPlatformFail:
                CreatPanelPrefab=LoginFailPanelPrefab;
                break;
            default:
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"SHowUI:" + loginUIType);
                break;
        }
        return CreatObjectToNGUI.InstantiateObj(CreatPanelPrefab, transform).GetComponent<IUIPanel>();
    }
    void OnLoginFaildMessageBox()
    {
        GameManager.Instance.QuitToLogin();
        OpenMainUI(LoginUIType.Login);
        //LoginPlatformManager.Instance.ReLoginPlatform();
    }
    //private void S_CUserLoginResHandle(INotifyArgs notify)
    //{
    //    NewSSUserLoginRes = (NewSSUserLoginRes)notify;
    //    if (NewSSUserLoginRes.lPromptFlag == 0)
    //    {
    //        PlayerPrefs.SetString("PlayerId", Account);
    //        PlayerPrefs.SetString("PlayerPwd", Pwd);
    //        if (NewSSUserLoginRes.lActorNum == 0)
    //        {
    //            OpenMainUI(LoginUIType.CreatRole);
    //            //UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.CreatRole);
    //        }
    //        else
    //        {
    //            GameDataManager.Instance.ResetData(DataType.ActorSelector, NewSSUserLoginRes.SSActorInfos);
    //            OpenMainUI(LoginUIType.SelectRole);
    //            //UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.SelectRole);
    //        }
    //    }
    //    else
    //    {
    //        UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_236"), LanguageTextManager.GetString("IDS_H2_13"), null);
    //    }

    //}

    /// <summary>
    /// 初始化完成处理
    /// </summary>
    /// <param name="flag"></param>
    private void InitPlatformHandler(bool flag)
    {
        if (flag)
        {
            LoginPlatform();
        }
        else
        {
            TraceUtil.Log("InitPlatformFailed");
        }
    }
    public void LoginPlatform()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)  
#if ANDROID_TENCENT
		isRecord = true;
#endif
#endif
        JHPlatformConnManager.Instance.LoginToPlatform(LoginPlatformHandler);
    }
    /// <summary>
    /// 登录完成处理
    /// </summary>
    /// <param name="flag"></param>
    private void LoginPlatformHandler(bool flag)
    {
        if (flag)
        {
            hasLogin=true;
#if (UNITY_ANDROID && !UNITY_EDITOR)  
#if ANDROID_TENCENT
			MessageBox.Instance.CloseMsgBox();
#endif
#endif
            JHPlatformConnManager.Instance.GetUserInfo(GetUserInfoHandler);
        }
        else
        {
            TraceUtil.Log("LoginPlatformFailed");
            MessageBox.Instance.Show(1,"",LanguageTextManager.GetString("IDS_I32_1"),LanguageTextManager.GetString("IDS_I32_2"),SureCallBack);
        }
    }
    void SureCallBack ()
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_LoginFail");
        OpenMainUI(LoginUIType.LoginPlatformFail);
    }
    /// <summary>
    /// 获得用户信息处理
    /// </summary>
    /// <param name="sucessed"></param>
    /// <param name="jsonInfo"></param>
    private void GetUserInfoHandler(bool sucessed, string jsonInfo)
    {
        if (sucessed)
        {
            JHPlatformConnManager.Instance.RequestPHP(RequestPHPBackHandler);
        }
        else
        {
            TraceUtil.Log("GetUserInfoPlatformFailed");
        }
    }
    /// <summary>
    /// 请求PHP服务器回调
    /// </summary>
    /// <param name="phpBackObj"></param>
    private void RequestPHPBackHandler(PhpBackObj phpBackObj)
    {
		if(phpBackObj == null)
		{
			MessageBox.Instance.Show(3, "", "network is bad, try it again", "Cancel", "Sure", null,	() => JHPlatformConnManager.Instance.RequestPHP(RequestPHPBackHandler));
		}

        if (phpBackObj != null)
        {
            switch (phpBackObj.ErrorCode)
            {
                case "1":  //正常
                    LoginManager.Instance.ServerInfo = phpBackObj.ServerInfo;
                //转到服务器选择界面 phpBackObj.ServerInfo是一个Server类的数组，里面是Server列表信息。
                    if (m_curUIType == LoginUIType.Login||m_curUIType==LoginUIType.LoginPlatformFail)
                    {
                        OpenMainUI(LoginUIType.ServerList);
                        //m_curUIPanel.GetComponent<PlatformLoginPanel>().ShowServerList(null);
                    }
                    break;
                case "2":  //平台验证不通过
                    UI.MessageBox.Instance.Show(3, "", phpBackObj.ErrorDesc, LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                    break;
                case "3":  //服务器维护，发公告
                    var phpNoticeBoard = CreatObjectToNGUI.InstantiateObj(PhpNoticeBoard, transform);
                    Transform noticeLabel;
                    phpNoticeBoard.transform.RecursiveFindObject("Notice", out noticeLabel);
                    noticeLabel.GetComponent<UILabel>().text = phpBackObj.ErrorDesc;
                    break;
            }
        }
    }

	void  OnApplicationFocus(bool focus)
	{
#if (UNITY_ANDROID && !UNITY_EDITOR)  
#if ANDROID_TENCENT
		isRecord = focus;
#endif
#endif
	}

    protected override void RegisterEventHandler()
    {
        //AddEventHandler(EventTypeEnum.S_CUserLoginRes.ToString(), S_CUserLoginResHandle);
    }
}
