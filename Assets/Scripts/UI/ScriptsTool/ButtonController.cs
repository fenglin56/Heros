using UnityEngine;
using System.Collections;

public class ButtonController : IButtonCallBack {

    public UISprite Background;
    public SpriteSwith spriteSwith;
    public bool Flag { get; private set; }

    void Start()
    {
        Flag = true;
    }

    public void SetCallBackFuntion(ButtonCallBack buttonCallBack)
    {
        base.buttonCallback = buttonCallBack;
    }

    public override void SetCallBackFuntion(ButtonCallBack ButtonCallBack, object obj)
    {
        base.buttonCallback = ButtonCallBack;
        base.ButtonCallBackInfo = obj;
    }

    public override void OnClick()
    {
        if (base.buttonCallback != null)
        {
            buttonCallback(ButtonCallBackInfo);
        }
    }

    public void SetButtonFlag(bool flag)
    {
        this.Flag = flag;
    }

    public override void SetMyButtonActive(bool Flag)
    {
        GetComponent<BoxCollider>().enabled = Flag;
    }


    public override void SetButtonBackground(int ButtonID)
    {
        spriteSwith.ChangeSprite(ButtonID);
    }

    public void SetBackgroundColor(Color color)
    {
        Background.color = color;
    }

    public void SetButtonText(string text)
    {
        textLabel.text = text;
    }

    public void SetTextColor(Color color)
    {
        textLabel.color = color;
    }

}
