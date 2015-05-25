using UnityEngine;
using System.Collections;
using UI.Login;
public class LoginPlatformFail : IUIPanel {
    #region implemented abstract members of IUIPanel

    public override void Show()
    {
        transform.localPosition = Vector3.zero;
    }

    public override void Close()
    {
        transform.localPosition = new Vector3(0,0,-1000);
        Debug.Log("关闭");
    }

    public override void DestroyPanel()
    {
        RemoveAllEvent();
    }

    #endregion

    public SingleButtonCallBack LoginBtn;
    void Awake()
    {
        LoginBtn.SetCallBackFuntion(LoginPlatform);
    }
    void LoginPlatform(object obj)
    {
        LoginDataManager.Instance.PlatformLoginUI.LoginPlatform();
    }

  
}
