using UnityEngine;
using System.Collections;


namespace UI.Login
{
    public class JoinGamePanel : IUIPanel
    {

        //public SingleButtonCallBack JoinGameButton;
        //public SingleButtonCallBack ReplaceUserButton;

        //void Awake()
        //{
        //    JoinGameButton.SetCallBackFuntion(LoginBehaviour.Instance.OnLoginGameBtnClick);
        //    ReplaceUserButton.SetCallBackFuntion(LoginBehaviour.Instance.OnChangeAccountBtnClick);
        //    JoinGameButton.SetButtonText(LanguageTextManager.GetString("IDS_H2_21"));
        //    ReplaceUserButton.SetButtonText(LanguageTextManager.GetString("IDS_H2_67"));
        //}


        public override void Show()
        {
           // transform.localPosition = Vector3.zero;
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