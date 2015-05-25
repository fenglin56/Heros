using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class BattleButton : IButtonCallBack
    {
        
        //public BattleButtonPosition buttonType = BattleButtonPosition.HealthButton;
        public SpriteSwith BackgroundSwitch;
        public Transform IconPoint1;
        public Transform IconPoint2;
        public UIFilledSprite RecoveSprite;
        protected BoxCollider boxCollider;
        public bool Active = true;

		public bool m_isLocalColding = false;

        public override void SetCallBackFuntion(ButtonCallBack ButtonCallBack, object obj)
        {
            base.buttonCallback = ButtonCallBack;
            base.ButtonCallBackInfo = obj;
        }
        public void SetPressCallBack(OnPressDelegate pressDelegate)
        {
            base.PressBtnCallBack = pressDelegate;
        }
        public override void OnClick()
        {
			if(m_isLocalColding)
				return;
            if (Active&& base.buttonCallback != null)
            {
                base.buttonCallback(base.ButtonCallBackInfo);
				//m_isLocalColding = true;
				//StartCoroutine(RelieveColding());
            }
        }
        void OnPress(bool isPressed)
        {
            this.BackgroundSwitch.ChangeSprite(isPressed ? 3 : 1);  //1和2由原来的代码所用，3为点击反馈--Rocky
            if (base.PressBtnCallBack != null)
            {
                base.PressBtnCallBack(isPressed);
            }
        }
		IEnumerator RelieveColding()
		{
			yield return new WaitForSeconds(1f);
			m_isLocalColding = false;
		}

        public void SetBoxCollitionActive(bool Flag)
        {
            //TraceUtil.Log("SetBoxCollitionActive:"+Flag);
            if (boxCollider == null)
            {
                boxCollider = gameObject.GetComponent<BoxCollider>();
            }
            boxCollider.enabled = Flag;            
        }

        public override void SetMyButtonActive(bool Flag)
        {
            //print("SetParentButtonActice");
            if (boxCollider == null)
            {
                boxCollider = gameObject.GetComponent<BoxCollider>();
            }
            boxCollider.enabled = Flag;
        }

        public override void SetButtonBackground(int ButtonID)
        {
            throw new System.NotImplementedException();
        }

        public void SetButtonIcon(GameObject IconPrefab)
        {
            if (IconPoint1.childCount > 0) { IconPoint1.ClearChild(); }
            var icon = CreatObjectToNGUI.InstantiateObj(IconPrefab,IconPoint1);
            //if(icon != null)
            //    icon.transform.localScale = new Vector3(50, 50, 1);
        }

        public void SetSecondIcon(GameObject IconPrefab)
        {
			//if(IconPoint2==null)return;
            if (IconPoint2.childCount > 0) { IconPoint2.ClearChild(); }
            var icon = CreatObjectToNGUI.InstantiateObj(IconPrefab, IconPoint2);
            if (icon != null)
                icon.transform.localScale = new Vector3(50, 50, 1);
        }

		public void SetAllSpriteAlpha(float value)
		{
			var children = this.transform.GetComponentsInChildren<UISprite>();
			children.ApplyAllItem(p=>{
				p.alpha = value;
			});
		}

        public virtual void SetButtonText(string BtnText)
        {
            textLabel.text = BtnText;
        }

        public virtual void RecoverMyself(float RecoverTime)
        {
            TweenFloat.Begin(RecoverTime, 1, 0, SetRecoverFloat);
        }

        protected virtual void SetRecoverFloat(float number)
        {
            RecoveSprite.fillAmount = number / 1;
            if (number == 0) { SetMyButtonActive(true); }
        }
    }
}
