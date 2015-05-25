using UnityEngine;
using System.Collections;

public class EctypePanelMessageBox : IUIPanel {


    public UILabel MsgTitle;
    public SingleButtonCallBack SureBtn;
    public SingleButtonCallBack CancelBtn;
    public SingleButtonCallBack IconBtn;

    ButtonCallBack SureBtnCallBack;

    public void Awake()
    {
        Close();
        SureBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_55"));
        CancelBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_28"));
        SureBtn.SetCallBackFuntion(OnSureBtnClick);
        CancelBtn.SetCallBackFuntion(OnCancelBtnClick);
    }

    public void ShowPanel(string Msg, int Type,int PayNumber, ButtonCallBack SureBtnCallBack)
    {
        Show();
        MsgTitle.SetText(Msg);
        IconBtn.SetButtonBackground(Type-1);
        IconBtn.SetButtonText(PayNumber.ToString());
        this.SureBtnCallBack = SureBtnCallBack;
    }

    void OnSureBtnClick(object obj)
    {
        Close();
        if (SureBtnCallBack != null)
        {
            SureBtnCallBack(null);
        }
    }

    void OnCancelBtnClick(object obj)
    {
        Close();
    }




    public override void Show()
    {
        transform.localPosition = Vector3.zero;
    }

    public override void Close()
    {
        transform.localPosition = new Vector3(0,0,-1000) ;
    }

    public override void DestroyPanel()
    {
        throw new System.NotImplementedException();
    }
}
