using UnityEngine;
using System;
using System.Text;
using System.Collections;
using NetworkCommon;

namespace UI.Login
{
    //目前的版本登录过程由这个脚本主导，LoginBehaviour主要是之前登录91平台做的。暂时保留以防将来应用
    public class LoginUIPanel : IUIPanel
    {
		public LocalServerListConfigDatabase LocalServerListConfig;
        public GameObject LocalLoginParentGO;  //挂载本地登录的物体
        public GameObject PlatformLoginParentGO;  //挂载平台登录的物体
        //public SingleButtonCallBack UserBtn;
        public SingleButtonCallBack RegisterBtn;
        public SingleButtonCallBack LoginBtn;
        public GameObject PhpNoticeBoard;

        public UIInput InputAccount;
        public UIInput Input_PassWord;
        //public UIInput Input_Server;
        //public UILabel Label_Server;
        public UILabel Input_Msg;
        public UICheckbox PasswordCheckBox;

        bool isFirstSetCheckBox = true;

        void Awake()
        {
            //如果不是登录本地，则禁用此脚本
            if (GameManager.Instance.PlatformType != PlatformType.Local)
            {
                LocalLoginParentGO.SetActive(false);
                PlatformLoginParentGO.SetActive(true);
                this.enabled = false;
                return;
            }
            else
            {
                PlatformLoginParentGO.SetActive(false);
            }

            this.RegisterEventHandler();
            LoginManager.Instance.ResetLoginButtonState();
            //if(!GameManager.Instance.IsInternalVersion)
            //{
                //if(this.Input_Server!=null)this.Input_Server.gameObject.SetActive(false);
                //if (this.Label_Server != null) this.Label_Server.gameObject.SetActive(false);
                this.SetLoginInfo();
            //}
                PasswordCheckBox.onStateChange = OnPasswordCheckBoxClick;
        }
        private void SetLoginInfo()
        {
            var prefPlayerId = PlayerPrefs.GetString("PlayerId");
            var prefPlayerPwd = PlayerPrefs.GetString("PlayerPwd");
            if (!string.IsNullOrEmpty(prefPlayerId))
            {
                this.InputAccount.text = prefPlayerId;
                var loginBtnLabel = LoginBtn.gameObject.transform.GetComponentInChildren<UILabel>();
                if(loginBtnLabel!=null)loginBtnLabel.text = LanguageTextManager.GetString("IDS_H2_66");
                if (!string.IsNullOrEmpty(prefPlayerPwd))
                {
                    this.Input_PassWord.text = prefPlayerPwd;
                }
            }
        }

        public void OnPasswordCheckBoxClick(bool state)
        {
            if (isFirstSetCheckBox)
            {
                isFirstSetCheckBox = false;
            }
            else
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Default"); 
            }
        }

        //不需要向平台发送，直接与游戏服务器相连有效
        protected void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.S_CEnterCode.ToString(), S_CEnterCodeHandle);
            AddEventHandler(EventTypeEnum.S_CUserLoginRes.ToString(), S_CUserLoginResHandle);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.LoginUI, SetLoginPanelActive);
        }
        void Start()
        {
            //if(this.Input_Server!=null)this.Input_Server.text = PlayerPrefs.GetString("ServerIP", "127.0.0.1");
            //if(UserBtn!=null)UserBtn.SetCallBackFuntion(OnUserBtnClick);
            RegisterBtn.SetCallBackFuntion(OnRegisterBtnClick);
            LoginBtn.SetCallBackFuntion(OnLoginBtnClick);
        }

        public void SetLoginPanelActive(object obj)
        {
            if ((LoginUIType)obj == LoginUIType.Login)
            {
                Show();
            }
            else
            {
                Close();
            }
        }

        public override void Show()
        {
            transform.localPosition = Vector3.zero;
        }

        public override void Close()
        {
            transform.localPosition = new Vector3(0,0,-1000);
        }

        void OnLoginBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            var account = InputAccount.text.Trim();
            var pwd= this.Input_PassWord.text.Trim();
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(pwd))
            {
                this.Input_Msg.text = "账号和密码不能为空";
            }
            else
            {
                //LoginManager.Instance.PlayerId = playerId;
                LoginManager.Instance.Account = account;
                LoginManager.Instance.Pwd = pwd;
                this.Input_Msg.text = "";

                LocalServerList();
                //PhpServerList();
            }
        }
        private void PhpServerList()
        {
            StartCoroutine(LoginManager.Instance.RequestPhpService(LoginManager.PHPSERVER_ADDRESS,PhpServerCallback));
        }

        private void LocalServerList()
        {
			//StartCoroutine(GetTestPHP("1"));


			StringBuilder strBuilder = new StringBuilder("");
			Server[] ServerList = LocalServerListConfig.LocalServerList;
			for(int i = 0, imax = ServerList.Length; i < imax; i++)
			{
				strBuilder.Append("{\"No\":").Append(ServerList[i].No.ToString()).Append(",");
				strBuilder.Append("\"Name\":\"").Append(ServerList[i].Name).Append("\",");
				strBuilder.Append("\"Version\":").Append(1).Append(",");
				strBuilder.Append("\"IP\":\"").Append(ServerList[i].IP).Append("\",");
				strBuilder.Append("\"Port\":").Append(ServerList[i].Port.ToString()).Append(",");
				strBuilder.Append("\"ActorNumber\":").Append(ServerList[i].ActorNumber.ToString()).Append(",");
				strBuilder.Append("\"Status\":").Append(ServerList[i].Status.ToString()).Append(",");
				strBuilder.Append("\"Recommend_status\":").Append(ServerList[i].Recommend_status.ToString()).Append("}");

				if(i < imax - 1)
				{ 
					strBuilder.Append(",");
				}
			}

			var testJson = "{\"ErrorCode\":\"1\",\"ErrorDesc\":\"\u6709\u6548\",\"ServerInfo\":[" + strBuilder.ToString() + "]}";
		
            testJson = testJson.Replace("\"", "'");
            var jsonBackObj = JsonConvertor<PhpBackObj>.Json2Object(testJson);
        	this.ShowSelectServer(jsonBackObj);
        	
        }

		IEnumerator GetTestPHP(string platform)
		{

			WWW www = new WWW("http://jh.fanhougame.net/getServerList.php?platform=" + platform);
			yield return www;
			if(www.isDone)
			{
				if(!string.IsNullOrEmpty(www.error))
				{
					MessageBox.Instance.Show(3, "", "获取服务器列表失败", "重试", () => LocalServerList());
					yield break;
				}

				var jsonBackObj = JsonConvertor<PhpBackObj>.Json2Object(www.text);
				if(jsonBackObj != null)
				{
					this.ShowSelectServer(jsonBackObj);
				}
				else
				{
					MessageBox.Instance.Show(3, "", "获取服务器列表失败", "重试", () => LocalServerList());
				}
			}
		}

        private void PhpServerCallback(string backJson)
        {
            backJson = backJson.Replace("\"", "'");
            var jsonBackObj = JsonConvertor<PhpBackObj>.Json2Object(backJson);
            TraceUtil.Log("BackJson:"+backJson);
            ShowSelectServer(jsonBackObj);
        }
        private void ShowSelectServer(PhpBackObj phpBackObj)
        {
            if (phpBackObj != null)
            {
                switch (phpBackObj.ErrorCode)
                {
                    case "1":  //正常
                        //转到服务器选择界面 phpBackObj.ServerInfo是一个Server类的数组，里面是Server列表信息。
                        //TraceUtil.Log(phpBackObj.ServerInfo.Length);
                        LoginManager.Instance.GotoHttpServerButtonEnable = true;
                        LoginManager.Instance.ServerInfo = phpBackObj.ServerInfo;
                        UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.ServerList);
                        break;
                    case "2":  //平台验证不通过
                        UI.MessageBox.Instance.Show(3, "", phpBackObj.ErrorDesc, LanguageTextManager.GetString("IDS_H2_13"), null);
                        break;
                    case "3":  //服务器维护，发公告
                         var phpNoticeBoard=CreatObjectToNGUI.InstantiateObj(PhpNoticeBoard, transform);
                        Transform noticeLabel;
                        phpNoticeBoard.transform.RecursiveFindObject("Notice", out noticeLabel);
                        noticeLabel.GetComponent<UILabel>().text = phpBackObj.ErrorDesc;

                        //隐藏按钮
                        LocalLoginParentGO.SetActive(false);
                        break;
                }
            }
        }        
        void OnRegisterBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        }

        void OnUserBtnClick(object obj)
        { 
        }

        private void ConnectToServer(string ip)
        {
            IpManager.InitServiceConfig(ip, 8000);
                  
            bool connectStatus=NetServiceManager.Instance.LoginService.ConnectToServer() == SERVICE_CODE.SUCCESS;
            if(connectStatus)
            {
             this.Input_Msg.text =  "";
             PlayerPrefs.SetString("ServerIP", ip);
            }
            else
            {
                this.Input_Msg.text="服务器连接失败";
            }
        }
        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.S_CEnterCode.ToString(), S_CEnterCodeHandle);
            RemoveEventHandler(EventTypeEnum.S_CUserLoginRes.ToString(), S_CUserLoginResHandle);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.LoginUI, SetLoginPanelActive); 
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
                    PlatformLoginBehaviour.message += " LoginUIPanel Trigger CreateRole :";
                    UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.CreatRole);
                }
                else
                {
                    //显示选择角色界面
                    PlatformLoginBehaviour.message += " LoginUIPanel Trigger RoleSelect :";
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
		/// <summary>
		/// 发送账号和密码，等待服务器返回用户角色信息，S_CUserLoginResHandle处理
		/// </summary>
		/// <param name="notify">Notify.</param>
        private void S_CEnterCodeHandle(INotifyArgs notify)
        {

            NetServiceManager.Instance.LoginService.SubmitAccountInfo(LoginManager.Instance.Account, LoginManager.Instance.Pwd, LoginManager.Instance.ServerVersion);
        }


        public override void DestroyPanel()
        {
        }
    }
}