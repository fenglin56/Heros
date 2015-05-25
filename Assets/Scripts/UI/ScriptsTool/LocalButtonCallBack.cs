
using UnityEngine;
using System.Collections;


    public class LocalButtonCallBack : View
    {
        public int BtnType { get; set; }

        public ButtonCallBack BtnCallBack;
        public UILabel ButtonText;
        public UISlicedSprite ButtonBackground;

		public bool IsPressShow;
		public UISlicedSprite UIPressedSprite;

        private UIImageButton btnImag;
        protected object CallBackValue;

        public void SetCallBackFuntion(ButtonCallBack btnCB,object callBackValue)
        {
            this.BtnCallBack = new ButtonCallBack(btnCB);
            this.CallBackValue = callBackValue;
        }

        public void SetCallBackFuntion(ButtonCallBack btnCB)
        {
            this.BtnCallBack = new ButtonCallBack(btnCB);
        }

        void OnClick()
        {
            if (this.BtnCallBack != null)
            {
                this.BtnCallBack(CallBackValue);
            }
        }
        public void OnClick(object callBackValue)
        {
            if (this.BtnCallBack != null)
            {
                this.BtnCallBack(callBackValue);
            }
        }

		void OnPress(bool pressed)
		{
			if(!IsPressShow)
				return;
			if(pressed)
			{
				UIPressedSprite.enabled = true;
			}
			else
			{
				UIPressedSprite.enabled = false;
			}
		}

        public void SetButtonStatus(bool Flag)//设置按钮显示的状态，被按下或者没有被按下
        {
            if (btnImag == null)
            {
                btnImag = GetComponent<UIImageButton>();
            }
            if (Flag)
            {
                btnImag.target.spriteName = btnImag.pressedSprite;
            }
            else
            {
                btnImag.target.spriteName = btnImag.normalSprite;
            }
        }

        public virtual void SetButtonActive(bool Flag)//设置按钮可否被点击
        {
            if(GetComponent<BoxCollider>())
                GetComponent<BoxCollider>().enabled = Flag;
            if (GetComponent<UIImageButton>())
                GetComponent<UIImageButton>().enabled = Flag;
        }

        public void SetImageComponentActive(bool Flag)//设置图片切换代码的激活
        {
            this.GetComponent<UIImageButton>().enabled = Flag;
        }

        public void SetButtonText(string BtnText)//设置按钮文字
        {
            if (ButtonText == null)
            {
                this.ButtonText = transform.FindChild("Label").GetComponent<UILabel>();
            }
            this.ButtonText.text = BtnText;
        }

        public void SetButtonTextColor(Color btnTextColor)//设置按钮文字颜色
        {
            if (ButtonText == null)
            {
                this.ButtonText = transform.FindChild("Label").GetComponent<UILabel>();
            }
            this.ButtonText.color = btnTextColor;
        }

        public void SetButtonTextureColor(Color BtnColor)//设置按钮背景颜色
        {
            if (ButtonBackground == null)
            {
                ButtonBackground = transform.FindChild("Background").GetComponent<UISlicedSprite>() ;
            }
            ButtonBackground.color = BtnColor;
        }

        #region Edit by lee
        public void SetButtonSprite(string spriteName)//设置按钮图片
        {
            if (ButtonBackground == null)
            {
                ButtonBackground = transform.FindChild("Background").GetComponent<UISlicedSprite>();
            }
            var uiImageBtn = transform.GetComponent<UIImageButton>();
            if (uiImageBtn != null)
            {
                uiImageBtn.normalSprite = spriteName;
                uiImageBtn.pressedSprite = spriteName;
                uiImageBtn.hoverSprite = spriteName;
            }
            ButtonBackground.spriteName = spriteName;
        }

        public void SetAlpha(float value)
        {
            if (ButtonBackground != null)
            {
                ButtonBackground.alpha = value;
            }
        }

        public void SetBoxCollider(bool isFlag)
        {
            if (GetComponent<BoxCollider>())
                GetComponent<BoxCollider>().enabled = isFlag;
        }

        public void SetEnabled(bool flag)//无法点击 并且 图片切换为disable状态
        {                        
            GetComponent<UIImageButton>().isEnabled = flag;           
        }

		public void SetSwith(int index)
		{
			var children = GetComponentsInChildren<SpriteSwith>();
			children.ApplyAllItem(p=>p.ChangeSprite(index));
		}
        #endregion


        public void DestoryButton()//删除本按钮
        {
            Destroy(gameObject);
        }

        protected override void RegisterEventHandler()
        {
        }
    }