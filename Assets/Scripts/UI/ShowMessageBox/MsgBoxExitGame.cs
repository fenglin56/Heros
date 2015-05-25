using UnityEngine;
using System.Collections;

public class MsgBoxExitGame : MonoBehaviour
{
    public UILabel MsgLable;
    public LocalButtonCallBack EnSurebuttonCallBack;
    public LocalButtonCallBack CancelbuttonCallBack;

    public void ShowBox(string str, ButtonCallBack enSurebuttonCallBack, ButtonCallBack cancelbuttonCallBack )
    {
        MsgLable.text = str;
        EnSurebuttonCallBack.SetCallBackFuntion(enSurebuttonCallBack);
        CancelbuttonCallBack.SetCallBackFuntion(cancelbuttonCallBack);
    }
}
