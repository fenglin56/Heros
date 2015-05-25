using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.Login
{
	public class LoginUIManagerPlatform : MonoBehaviour {

//        private static LoginUIManagerPlatform m_instance;
//        public static LoginUIManagerPlatform Instance { get { return m_instance; } }

//        public GameObject LoginUIPanel, ServerListPanel, LoadingSceneUIPanel, CreatRoleUIPreafab, RoleSelectUIPrefab;
        
//        private Dictionary<LoginUIType, IUIPanel> UIList = new Dictionary<LoginUIType, IUIPanel>();

//        private GameObject m_curUIPanel = null;
//        private LoginUIType m_curUIType = LoginUIType.Loaing;
        

//        void Awake()
//        {
//            this.RegisterEventHandler();
//            UIEventManager.Instance.RegisterUIEvent(UIEventType.ShowLodingUI, OpenMainUI);
            
//        }

//        void Start()
//        {
//            m_instance = this;
//            OpenMainUI(LoginUIType.Login);
//            //1、初始化平台
//            LoginPlatformManager.Instance.InitPlatform();
//        }


//        private string m_logInfo = "";
//        public static void setSdkMessage(string msg)
//        {
//            Instance.m_logInfo += msg;
//        }

//        void OnDestroy()
//        {
//            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShowLodingUI, OpenMainUI);
//            LoginPlatformManager.Instance.OnLoginComplete -= this.Instance_OnLoginComplete;
//            RemoveEventHandler(EventTypeEnum.S_CUserLoginRes.ToString(), S_CUserLoginResHandle);
//            LoginPlatformManager.Instance.Release();
            
//        }

//        void Instance_OnLoginComplete(PhpBackObj phpBackObj)
//        {
//            ShowSelectServer(phpBackObj);
//        }



//        void OnApplicationPause(bool pauseStatus) 
//        {
//            if (!pauseStatus && !LoginPlatformManager.Instance.m_isLoginedPlatform)
//                Invoke("ReLoginPlatform", 3f);
//        }

//        void ReLoginPlatform()
//        {
//            if (!LoginPlatformManager.Instance.m_isLoginedPlatform)
//                LoginPlatformManager.Instance.ReLoginPlatform();
//        }


//        public void OnSendLoginInfo()
//        {
//#if (UNITY_ANDROID && !UNITY_EDITOR)
//#if ANDROID_UC
//        StartCoroutine(LoginPlatformManager.Instance.RequestPhpService(LoginPlatformManager.PHP_PLATFORM_SERVER, LoginPlatformManager.Instance.PhpServerCallback));
//#elif ANDROID_JIUYAO

//#elif ANDROID_XIAOMI
//        StartCoroutine(LoginPlatformManager.Instance.RequestPhpService(MiConfig.PHP_PLATFORM_SERVER, LoginPlatformManager.Instance.PhpServerCallback));
//#endif
//#endif
//        }

//        private void S_CUserLoginResHandle(INotifyArgs notify)
//        {
//            //LoginManager.Instance.CreateActorButtonEnable = true;
//            //LoginManager.Instance.DeleteActorButtonEnable = true;
//            //LoginManager.Instance.EnterTownButtonEnable = true;
//            LoginPlatformManager.Instance.NewSSUserLoginRes = (NewSSUserLoginRes)notify;
//            if (LoginPlatformManager.Instance.NewSSUserLoginRes.lPromptFlag == 0)
//            {
//                //±£´æÓÃ»§ÃûºÍÃÜÂë
//                PlayerPrefs.SetString("PlayerId", LoginPlatformManager.Instance.Account);
//                PlayerPrefs.SetString("PlayerPwd", LoginPlatformManager.Instance.Pwd);
//                if (LoginPlatformManager.Instance.NewSSUserLoginRes.lActorNum == 0)
//                {
//                    //ÏÔÊ¾´´½¨½ÇÉ«½çÃæ
//                    UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.CreatRole);
//                }
//                else
//                {
//                    //ÏÔÊ¾Ñ¡Ôñ½ÇÉ«½çÃæ
//                    GameDataManager.Instance.ResetData(DataType.ActorSelector, LoginPlatformManager.Instance.NewSSUserLoginRes.SSActorInfos);
//                    UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.SelectRole);
//                }
//            }
//            else
//            {
//                //ÌáÊ¾µÇÂ¼Ê§°Ü
//                UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_236"), LanguageTextManager.GetString("IDS_H2_13"), null);
//            }
			
//        }
		
//        public void ShowSelectServer(PhpBackObj phpBackObj)
//        {
            
//            if (phpBackObj != null)
//            {
//                //LoginPlatformManager.Instance.GetString += phpBackObj.ErrorCode;
//                if (phpBackObj.ErrorCode == "1")   //成功
//                {
//                    LoginPlatformManager.Instance.ServerInfo = phpBackObj.ServerInfo;
					
//                    //转到服务器选择界面 phpBackObj.ServerInfo是一个Server类的数组，里面是Server列表信息。
//                    //TraceUtil.Log(phpBackObj.ServerInfo.Length);
//                    LoginManager.Instance.GotoHttpServerButtonEnable = true;
//                    if (m_curUIType == LoginUIType.Login)
//                    {
//                        m_curUIPanel.GetComponent<PlatformLoginPanel>().ShowStartGameUI(true);
//                    }
//                }
//                else
//                {
//                    UI.MessageBox.Instance.Show(3, "", phpBackObj.ErrorDesc, LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
//                }
//            }
//        }

//        public void ShowServerList()
//        {
//            OpenMainUI(LoginUIType.ServerList);
//        }


//        void OpenMainUI(object obj)
//        {
//            m_curUIType = (LoginUIType)obj;
//            CloseAllPanel();
//            IUIPanel uiPanel = null;

//            if (UIList.TryGetValue(m_curUIType, out uiPanel))
//            {
//                uiPanel.Show();
//                m_curUIPanel = uiPanel.gameObject;
//            }
//            else
//            {
//                uiPanel = GetPanel(m_curUIType);
//                m_curUIPanel = uiPanel.gameObject;

//                uiPanel.Show();
//                UIList.Add(m_curUIType, uiPanel);
//            }

//            if (m_curUIType == LoginUIType.CreatRole || m_curUIType == LoginUIType.SelectRole)
//            {
//                SoundManager.Instance.StopBGM(0.0f);
//                SoundManager.Instance.PlayBGM("Music_UIBG_LoginCharacter", 0.0f);
//            }
//        }

//        void CloseAllPanel()
//        {
//            foreach (var child in UIList)
//            {
//                if (child.Value != null)
//                {
//                    child.Value.Close();
//                }
//            }
//        }


//        IUIPanel GetPanel(LoginUIType loginUIType)
//        {
//            GameObject CreatPanelPrefab = null;
//            switch (loginUIType)
//            {
//                case LoginUIType.Login:
//                    CreatPanelPrefab = LoginUIPanel;
//                    break;
//                //case LoginUIType.JoinGame:
//                //    CreatPanelPrefab = JoinUIPrefab;
//                //    break;
//                case LoginUIType.ServerList:
//                    CreatPanelPrefab = ServerListPanel;
//                    break;
//                case LoginUIType.CreatRole:
//                    CreatPanelPrefab = CreatRoleUIPreafab;
//                    break;
//                case LoginUIType.SelectRole:
//                    CreatPanelPrefab = RoleSelectUIPrefab;
//                    break;
//                case LoginUIType.Loaing:
//                    CreatPanelPrefab = LoadingSceneUIPanel;
//                    break;
//                default:
//                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"SHowUI:" + loginUIType);
//                    break;
//            }
//            return CreatObjectToNGUI.InstantiateObj(CreatPanelPrefab, transform).GetComponent<IUIPanel>();
//        }


//        void OnLoginFaildMessageBox()
//        {
//            //LoginManager.Instance.ResetLoginButtonState();
//            GameManager.Instance.QuitToLogin();
//            OpenMainUI(LoginUIType.Login);
//            LoginPlatformManager.Instance.ReLoginPlatform();
//        }



//        //void OnGUI()
//        //{
//        //    if (GUI.Button(new Rect(0, 0, 300, 50), "PlatformPay"))
//        //    {
//        //        LoginPlatformManager.Instance.PlatformPay(30);
//        //    }

//        //    if (GUI.Button(new Rect(0, 60, 300, 50), "PlatformPay"))
//        //    {
//        //        LoginPlatformManager.Instance.PlatformPay(50);
//        //    }

//            //if (GUI.Button(new Rect(0, 0, 300, 50), "UCProduct"))
//            //{
//            //    LoginPlatformManager.Instance.PhpServerCallback("");
//            //}

//        //    if (GUI.Button(new Rect(0, 60, 300, 50), "ShowPreLoginServer"))
//        //    {
//        //        if (m_curUIType == LoginUIType.Login)
//        //            m_LoginUIPanel.GetComponent<PlatformLoginPanel>().ShowStartGameUI(true);
//        //    }

//        //    GUI.Button(new Rect(0, 120, Screen.width, 220),"");

//           // GUI.Button(new Rect(0, 60, Screen.width, 200), "");
//           // GUI.Label(new Rect(0, 60, Screen.width, 200), LoginPlatformManager.Instance.GetString);
//            //GUI.Button(new Rect(0, 220, Screen.width, 100), m_logInfo);
//       // }


//        protected override void RegisterEventHandler()
//        {
//            LoginPlatformManager.Instance.OnLoginComplete += this.Instance_OnLoginComplete;
//            AddEventHandler(EventTypeEnum.S_CUserLoginRes.ToString(), S_CUserLoginResHandle);
//        }
    }

}