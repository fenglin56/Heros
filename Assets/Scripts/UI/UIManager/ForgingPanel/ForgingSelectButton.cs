using UnityEngine;
using System.Collections;

public class ForgingSelectButton:IButtonCallBack
{
    public GameObject SelectIcon;
    public GameObject CanForgingFlag;
    private ButtonCallBack m_Selcetcallback;
    private ButtonCallBack m_CanelSelcetcallback;
    private bool ISSelecting;
    private bool m_canCanel;
    private bool m_ShowFlag;
    private bool isEnable = true;

    public bool Enable {
        get{
            return isEnable;
        }
        set{
            if(isEnable != value)
            {
                isEnable = value;
            }
        }
    }

    public void Init(ButtonCallBack SelectCallBack,ButtonCallBack CanelSelectCallBack,bool CanCanel,bool ShowFlag)
    {
        m_Selcetcallback=SelectCallBack;
        m_CanelSelcetcallback=CanelSelectCallBack;
        m_canCanel=CanCanel;
        m_ShowFlag=ShowFlag;
        UpdateFlag(ShowFlag);
    }

    public void CancelSelect()
    {
        ISSelecting=false;
        SelectIcon.SetActive(false);
        if(m_CanelSelcetcallback!=null)
        {
            m_CanelSelcetcallback(null);
        }
        
    }
    public void Select()
    {

        if(m_Selcetcallback!=null)
        {
            ISSelecting=true;
            SelectIcon.SetActive(true);
            m_Selcetcallback(null);
        }
    }

    public void UpdateFlag(bool show)
    {
        if(CanForgingFlag!=null)
        {
        if(show)
        {
            CanForgingFlag.SetActive(true);
        }
        else
        {
            CanForgingFlag.SetActive(false);
        }
        }
    }
    public void SetCallBackFuntion(ButtonCallBack SelectButtonCallBack,bool ShowFlag)
    {
        Init(SelectButtonCallBack,null,false,ShowFlag);
    }
    public void SetCallBackFuntion(ButtonCallBack SelectButtonCallBack,ButtonCallBack DeSelectButtonCallBack)
    {
        Init(SelectButtonCallBack,DeSelectButtonCallBack,true,true);
    }
    #region implemented abstract members of IButtonCallBack

    public override void SetCallBackFuntion(ButtonCallBack ButtonCallBack, object obj)
    {
        this.buttonCallback = ButtonCallBack;
        base.ButtonCallBackInfo = obj;
    }

    public override void OnClick()
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Forge_Click");
        if(m_canCanel)
        {
        if(!ISSelecting)
        {
            ISSelecting=true;
            SelectIcon.SetActive(true);
            if(m_Selcetcallback!=null)
            {
                m_Selcetcallback(this.ButtonCallBackInfo);
            }
        }
        else
        { 
                ISSelecting=false;
                SelectIcon.SetActive(false);
                if(m_CanelSelcetcallback!=null)
                {
                    m_CanelSelcetcallback(this.ButtonCallBackInfo);
                } 
        }
        }else
        {
            ISSelecting=true;
            SelectIcon.SetActive(true);
            if(m_Selcetcallback!=null)
            {
                m_Selcetcallback(this.ButtonCallBackInfo);
            }
        }
    }

    public override void SetMyButtonActive(bool Flag)
    {

    }

    public override void SetButtonBackground(int ButtonID)
    {
       
    }

    #endregion


}


