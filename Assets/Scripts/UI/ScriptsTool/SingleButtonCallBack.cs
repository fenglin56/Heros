using UnityEngine;
using System.Collections;
using System;

public class SingleButtonCallBack : IButtonCallBack
{
    public Transform CreatIconPoint;//需要实例出icon的参照物
    public UILabel textLabel01;

    public SpriteSwith spriteSwith;//收集的sprite列表
    public SpriteSwith[] spriteSwithList;//这个目前好像没有封装，是有人使用时才在外面自己调用//
    public SpriteSwith[] BackgroundSwithList;//�����״̬�ı�ı����б�
	private bool isEnable = true;
	public bool Enable {
		get{
			return isEnable;
		}
		set{
			//if(isEnable != value)
			{
				isEnable = value;
				if(isEnable)
				{
					BackgroundSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
				}
				else
				{
					BackgroundSwithList.ApplyAllItem(p=>p.ChangeSprite(3));
				}
			}
		}
	}
    //public UISprite IconSprite;//切换显示ICON的sprite


    public override void SetCallBackFuntion(ButtonCallBack ButtonCallBack, object obj)
    {
        this.buttonCallback = ButtonCallBack;
        base.ButtonCallBackInfo = obj;
    }

    public void SetCallBackFuntion(ButtonCallBack buttonCallBack)
    {
        this.buttonCallback = buttonCallBack;
    }

    public void SetPressCallBack(OnPressDelegate pressDelegate)
    {
        base.PressBtnCallBack = pressDelegate;
    }

    public void SetDragCallback(OnDragDelegate dragDelegate)
    {
        base.DragCallBack = dragDelegate;
    }

    void OnDrag(Vector2 delta)
    {
        if (base.DragCallBack != null)
        {
            base.DragCallBack(delta);
        }
    }

    public override void OnClick()
    {
        if (!Enable || this.buttonCallback == null)
            return;
        this.buttonCallback(this.ButtonCallBackInfo);
    }

    void OnPress(bool isPressed)
    {
        if (Enable)
        {
            if (base.PressBtnCallBack != null)
            {
                base.PressBtnCallBack(isPressed);
            }
            if (BackgroundSwithList != null && BackgroundSwithList.Length > 0)
            {
                BackgroundSwithList.ApplyAllItem(P => P.ChangeSprite(isPressed ? 2 : 1));
            }
        }
    }

    public override void SetMyButtonActive(bool Flag)
    {
        this.Enable = Flag;
    }

    public void SetButtonColliderActive(bool Flag)
    {
        GetComponent<BoxCollider>().enabled = Flag;
    }

    public void SetImageButtonComponentActive(bool Flag)
    {
        UIImageButton ImageButton = GetComponent<UIImageButton>();
        if (ImageButton != null)
        {
            ImageButton.enabled = Flag;
        }
    }

    public override void SetButtonBackground(int ButtonID)
    {
        if (this.spriteSwith == null)
        {
            this.spriteSwith = gameObject.GetComponent<SpriteSwith>();
        }
        this.spriteSwith.ChangeSprite(ButtonID);
    }

    public void SetButtonBackground(string spriteName)
    {
        this.BackgroundSprite.spriteName = spriteName;
    }

    public void SetButtonIcon(GameObject ButtonPrefab)
    {
        if (CreatIconPoint.childCount > 0)
        {
            CreatIconPoint.ClearChild();
        }
        if (ButtonPrefab != null)
        {
            UI.CreatObjectToNGUI.InstantiateObj(ButtonPrefab, CreatIconPoint);
        }
    }
    public void SetButtonIcon(GameObject ButtonPrefab,int depth,int zDepth)
    {
        if (CreatIconPoint.childCount > 0)
        {
            CreatIconPoint.ClearChild();
        }
        if (ButtonPrefab != null)
        {
            var icon=UI.CreatObjectToNGUI.InstantiateObj(ButtonPrefab, CreatIconPoint);
            Vector3 pos=icon.transform.localPosition;
            icon.transform.localPosition = pos+Vector3.forward*zDepth;
            var iconSprite = icon.GetComponent<UISprite>();
            if (iconSprite != null)
            {
                iconSprite.depth = depth;
            }
        }
    }

    public void HideMyself()
    {
        gameObject.SetActive(false);
    }

    public void ShowMyself()
    {
        gameObject.SetActive(true);
    }

    public void SetButtonText(string Txt)
    {
        if (textLabel == null)
        {
            GetTextComponent();
        }
        textLabel.text = Txt;
    }

    public void SetButtonTextB(string Text)
    {
        if(textLabelB != null)
            textLabelB.text = Text;
    }

    public void SetButtonText01(string Txt)
    {
        if (textLabel01 == null)
        {
            GetTextComponent();
        }
        textLabel01.text = Txt;
    }

    public void SetTextColor(Color textColor)
    {
        if (textLabel == null) { GetTextComponent(); }
        textLabel.color = textColor;
    }

    public void SetImageColor(Color color)
    {
        if (BackgroundSprite != null)
        {
            BackgroundSprite.color = color;
        }
    }

    public void SetImageAlpha(float alpha)
    {
        if (BackgroundSprite != null)
        {
            BackgroundSprite.alpha = alpha;
        }
	}
    void OnDisable()
    {
        if (OnActiveChanged != null)
        {
            OnActiveChanged(false);
        }
    }
    void OnEnable()
    {
        if (OnActiveChanged != null)
        {
            OnActiveChanged(true);
        }
    }
}
