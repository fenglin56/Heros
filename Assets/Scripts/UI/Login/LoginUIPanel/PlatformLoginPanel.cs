using UnityEngine;
using System.Collections;
using UI.Login;

public class PlatformLoginPanel : IUIPanel
{
    public GameObject PlatformLoginParentGO;  //挂载平台登录的物体
    public UILabel LoginMsg;
    //public UILabel ServerName;
    //public SingleButtonCallBack ServerListButton;
    //public SingleButtonCallBack EnterGameButton;
//    public SingleButtonCallBack LoginOutButton;
    public GameObject LoginSuccess;



    void Awake()
    {
        //如果不是登录本地，则禁用此脚本
        if (GameManager.Instance.PlatformType == PlatformType.Local)
        {
            PlatformLoginParentGO.SetActive(false);
            this.enabled = false;
            return;
        }
        this.RegisterEventHandler();
        //ServerListButton.SetCallBackFuntion(ShowServerList);
        //EnterGameButton.SetCallBackFuntion(EnterGameHandle);
//        LoginOutButton.SetCallBackFuntion(LoginOutHandle);
    }


	// Use this for initialization
	void Start () {

        LoginSuccess.SetActive(false);

        //自动登陆

	}

    public void ShowStartGameUI(bool flag)
    {
        LoginSuccess.SetActive(flag);
    }

    public void ShowServerList(object obj)
    {
        //UI.Login.LoginUIManagerPlatform.Instance.ShowServerList();
        UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, LoginUIType.ServerList);
    }

    void EnterGameHandle(object obj)
    {

    }
    protected void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.S_CEnterCode.ToString(), S_CEnterCodeHandle);
        AddEventHandler(EventTypeEnum.S_CUserLoginRes.ToString(), S_CUserLoginResHandle);
    }
//    void LoginOutHandle(object obj)
//    {
//        LoginManager.Instance.LogoutPlatform();
//        ShowStartGameUI(false);
//    }

    public override void Close()
    {
        transform.localPosition = new Vector3(0, 0, -1000);
    }


    public override void Show()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }
    private void S_CUserLoginResHandle(INotifyArgs notify)
    {
        LoginManager.Instance.CreateActorButtonEnable = true;
        LoginManager.Instance.DeleteActorButtonEnable = true;
        LoginManager.Instance.EnterTownButtonEnable = true;
        LoginManager.Instance.NewSSUserLoginRes = (NewSSUserLoginRes)notify;
        if (LoginManager.Instance.NewSSUserLoginRes.lPromptFlag == 0)
        {
            //保存用户名和密码
            PlayerPrefs.SetString("PlayerId", LoginManager.Instance.Account);
            PlayerPrefs.SetString("PlayerPwd", LoginManager.Instance.Pwd);
            if (LoginManager.Instance.NewSSUserLoginRes.lActorNum == 0)
            {
                //显示创建角色界面
                PlatformLoginBehaviour.message += " PlatformLoginPanel Trigger CreateRole :";
                UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.CreatRole);
            }
            else
            {
                //显示选择角色界面
                GameDataManager.Instance.ResetData(DataType.ActorSelector, LoginManager.Instance.NewSSUserLoginRes.SSActorInfos);
                UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.SelectRole);
            }
        }
        else
        {
            //提示登录失败
            UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_236"), LanguageTextManager.GetString("IDS_H2_13"), null);
        }

    }
    private void S_CEnterCodeHandle(INotifyArgs notify)
    {
        PlatformLoginBehaviour.message += " PlatformLoginPanel ReceiveNotify:" + LoginManager.Instance.Account + "  " + LoginManager.Instance.Pwd;
		NetServiceManager.Instance.LoginService.SubmitAccountInfo(LoginManager.Instance.Account, LoginManager.Instance.Pwd, LoginManager.Instance.ServerVersion);
    }
    public override void DestroyPanel()
    {
        RemoveEventHandler(EventTypeEnum.S_CEnterCode.ToString(), S_CEnterCodeHandle);
        RemoveEventHandler(EventTypeEnum.S_CUserLoginRes.ToString(), S_CUserLoginResHandle);
        Destroy(this.gameObject);
    }
}
