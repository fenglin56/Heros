using UnityEngine;
using System.Collections;

public class CostMoneyMessageBox : MonoBehaviour
{
    public UILabel MsgLabel;
    public UILabel SureBtnLabel;
    public UILabel CancelBtnLabel;
    public UILabel CostNumberLabel;
    public SpriteSwith CostTypeIcon;
    public SingleButtonCallBack SureBtn;
    public SingleButtonCallBack CancelBtn;

    ButtonCallBack SureCallBack;
    ButtonCallBack CancelCallBack;

    public void Show(CostMoneyType CostMoneyType,int CostMoneyNumber, string Msg, string SureBtnStr, string CancelBtnStr, ButtonCallBack SureBtnCallBack, ButtonCallBack CancelBtnCallBack)
    {
        transform.localPosition = new Vector3(0,0,-100);
        this.MsgLabel.SetText(Msg);
        CostTypeIcon.ChangeSprite((int)CostMoneyType);
        CostNumberLabel.SetText(CostMoneyNumber);
        SureBtn.SetCallBackFuntion(OnSureBtnClick);
        CancelBtn.SetCallBackFuntion(OnCancelBtnClick);
        SureBtnLabel.SetText(SureBtnStr);
        CancelBtnLabel.SetText(CancelBtnStr);
        this.SureCallBack = SureBtnCallBack;
        this.CancelCallBack = CancelBtnCallBack;
    }

    public void SetSureLabelColor(Color color)
    {
        CostNumberLabel.color = color;
        SureBtnLabel.color = color;
    }

    void OnSureBtnClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Pay");
        if (SureCallBack != null)
        {
            this.SureCallBack(null);
        }
        Close();
    }

    void OnCancelBtnClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        if (CancelCallBack != null)
        {
            this.CancelCallBack(null);
        }
        Close();
    }

    public void Close()
    {
        transform.localPosition = new Vector3(0, 0, -1000);
    }
}

public enum  CostMoneyType
{
    Gold = 1,
    Copper,
}