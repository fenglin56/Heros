using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Login
{

    public class LoginUIManagerFor91 : MonoBehaviour
    {
        public GameObject LoginUIPrefab, JoinUIPrefab,ServerListUIPrefab,CreatRoleUIPreafab,RoleSelectUIPrefab,LoadingSceneUIPreafab;
        public Camera Camera;

        private IUIPanel LoginUI, JoinUI,ServerListUI,CreatRoleUI,RoleSelectUI,LoadingSceneUI;

        private Dictionary<LoginUIType, IUIPanel> UIList = new Dictionary<LoginUIType,IUIPanel>();


        void Awake()
        {
            if (GameManager.Instance.PlatformType != PlatformType.Local)
            {
                this.enabled = false;
                return;
            }
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ShowLodingUI, OpenMainUI);
            
            OpenMainUI(LoginUIType.Login);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShowLodingUI, OpenMainUI);
        }
        void Update() { }
        void OpenMainUI(object obj)
        {
            PlatformLoginBehaviour.message += " For91 OpenUI:";
            var loginUIType = (LoginUIType)obj;
            CloseAllPanel();
            IUIPanel uiPanel = null;
            if (UIList.TryGetValue(loginUIType, out uiPanel))
            {
                uiPanel.Show();
            }
            else
            {
                uiPanel = GetPanel(loginUIType);
                uiPanel.Show();
                UIList.Add(loginUIType,uiPanel);
            }

            if (loginUIType == LoginUIType.CreatRole)
            {
                SetCamera();
            }

			
			if(loginUIType == LoginUIType.CreatRole || loginUIType == LoginUIType.SelectRole)
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
            this.Camera.gameObject.SetActive(false);
            foreach (var child in UIList)
            {
                if (child.Value != null)
                {
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
                    CreatPanelPrefab = LoginUIPrefab;
					CheatManager.Instance.isIDKickedMark = false;
					CheatManager.Instance.isLogined = false;
				return CreatObjectToNGUI.InstantiateObj(CreatPanelPrefab,transform).GetComponent<LoginUIPanel>() as IUIPanel;
                    break;
                case LoginUIType.JoinGame:
                    CreatPanelPrefab = JoinUIPrefab;
                    break;
                case LoginUIType.ServerList:
                    CreatPanelPrefab = ServerListUIPrefab;
                    break;
                case LoginUIType.CreatRole:
                    PlatformLoginBehaviour.message += " LoginUIManagerFor91 Handler CreateRole :";
                    CreatPanelPrefab = CreatRoleUIPreafab;
					CheatManager.Instance.isLogined = true;
                    break;
                case LoginUIType.SelectRole:
                    CreatPanelPrefab = RoleSelectUIPrefab;
					CheatManager.Instance.isLogined = true;
                    break;
                case LoginUIType.Loaing:
                    CreatPanelPrefab = LoadingSceneUIPreafab;
                    break;
                default:
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"SHowUI:"+loginUIType);
                    break;
            }
            return CreatObjectToNGUI.InstantiateObj(CreatPanelPrefab,transform).GetComponent<IUIPanel>();
        }

    }
}