using UnityEngine;
using System.Collections;

namespace UI.Login
{
    
    public class LoginUIManager : MonoBehaviour
    {
        //private static LoginUIManager m_instance;
        //public static LoginUIManager Instance { get { return m_instance; } }

        ////public delegate void ShowPanelDelegate(LoginUIType loginUIType );
        ////public event ShowPanelDelegate SetLoginPanelActiveEvent;

        //public GameObject CreateRolePanelPrefab, LoginUIPrefab,LoadingUIPrefab;

        //private GameObject CreatRolePanel, LoginUIPanel,LoadingUIPanel;

        //void Awake()
        //{
        //    m_instance = this;
        //    UIEventManager.Instance.RegisterUIEvent(UIEventType.ShowLodingUI, OpenMainUI);
        //    //GameDataManager.Instance.dataEvent.RegisterEvent(DataType.LoadingSceneData,OpenMainUI);
        //}

        //void Start()
        //{
			
        //    ShowMainUI(LoginUIType.Login);
        //}

        //void OnDestroy()
        //{
			
        //    m_instance = null;
        //    UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShowLodingUI, OpenMainUI);
        //}

        //void OpenMainUI(object obj)
        //{
        //    var loginUIType = (LoginUIType)obj;
        //    ShowMainUI((LoginUIType)obj);
        //    if(loginUIType == LoginUIType.CreatRole || loginUIType == LoginUIType.SelectRole)
        //    {
        //        SoundManager.Instance.PlayBGM("Music_UIBG_LoginCharacter", 0.0f);	
        //    }
        //}

        //public void ShowMainUI(LoginUIType loginUIType)
        //{
        //    switch (loginUIType)
        //    {
        //        case LoginUIType.Login:
        //            if (LoginUIPanel == null) { LoginUIPanel = CreatObjectToNGUI.InstantiateObj(LoginUIPrefab, transform); }
        //            break;
        //        case LoginUIType.CreatRole:
        //            if (CreatRolePanel == null) { CreatRolePanel = CreatObjectToNGUI.InstantiateObj(CreateRolePanelPrefab,transform); }
        //            break;
        //        case LoginUIType.Loaing:
        //            //if (LoadingUIPanel == null) { LoadingUIPanel = CreatObjectToNGUI.InstantiateObj(LoadingUIPrefab,transform); }
        //            break;
        //        default:
        //            break;
        //    }

        //    UIEventManager.Instance.TriggerUIEvent(UIEventType.LoginUI,loginUIType);

        //}
    }   
}