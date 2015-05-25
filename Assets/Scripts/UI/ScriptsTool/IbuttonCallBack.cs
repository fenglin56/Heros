using UnityEngine;
using System.Collections;
using System;

public abstract class IButtonCallBack : MonoBehaviour
{
    public delegate void OnPressDelegate(bool isPressed);
    public delegate void OnDragDelegate(Vector2 drag);

    public UISprite BackgroundSprite;
    public object ButtonCallBackInfo = null;
    public ButtonCallBack buttonCallback;
    public OnPressDelegate PressBtnCallBack;
    public OnDragDelegate DragCallBack;
    public Action<bool> OnActiveChanged;

    public UILabel textLabel;
    public UILabel textLabelB;

    public abstract void SetCallBackFuntion(ButtonCallBack ButtonCallBack, object obj);
    public abstract void OnClick();
    public abstract void SetMyButtonActive(bool Flag);
    public abstract void SetButtonBackground(int ButtonID);

    public void GetTextComponent()
    {
        if (null != gameObject.GetComponent<UILabel>())
        {
            textLabel = gameObject.GetComponent<UILabel>();
            return;
        }
        foreach (Transform child in transform)
        {
            if (null != child.GetComponent<UILabel>())
            {
                textLabel = child.GetComponent<UILabel>();
                return;
            }
        }
    }
}
