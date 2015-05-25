using UnityEngine;
using System.Collections;

namespace UI.Login
{
    public class LoginPanel : IUIPanel
    {

        //public SingleButtonCallBack LoginButton;
        //public SingleButtonCallBack RegisterButton;

        //void Awake()
        //{
        //    LoginButton.SetCallBackFuntion(LoginBehaviour.Instance.OnLogin91BtnClick);
        //    RegisterButton.SetCallBackFuntion(LoginBehaviour.Instance.OnRegisterBtnClick);
        //    LoginButton.SetButtonText(LanguageTextManager.GetString("IDS_H2_66"));
        //    RegisterButton.SetButtonText(LanguageTextManager.GetString("IDS_H2_51"));
        //}


        public override void Show()
        {
            //transform.localPosition = Vector3.zero;
        }

        public override void Close()
        {
            //transform.localPosition = new Vector3(0, 0, -1000);
        }

        public override void DestroyPanel()
        {
        }

    }
}