using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class HealthAndMagicButton : BattleButton
    {
        public Transform EffectsPoint;

        public SpriteSwith[] JoyStickSprite;

        public Transform NumberLabelTransform;
        public Vector3 joysticPos;
        public Vector3 normalPos;

		public Transform SurplusTime;
		public Transform Copper;
		public UILabel Label_copper;

        private GameObject EffectsObj;

        bool IsJoystick = true;

        private int m_guideBtnID = 0;

        void Start()
        {
            IsJoystick = GameManager.Instance.UseJoyStick;
            JoyStickSprite.ApplyAllItem(P => P.ChangeSprite(IsJoystick?2:1));
            GuideBtnManager.Instance.RegGuideButton(this.gameObject, MainUI.UIType.Empty, SubType.EctypeHealth, out m_guideBtnID);
            ////如果有新手引导，并且步骤尚未开始，先屏蔽技能按钮
            //if (GameManager.Instance.IsNewbieGuide && !NewbieGuideManager_V2.Instance.EctypeGuideStepReached)
            //{
            //    SetMyButtonActive(false);
            //}
            NumberLabelTransform.localPosition = IsJoystick ? joysticPos : normalPos;
        }

        void OnDestroy()
        {
            GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        public void FlagEffects()
        {
            EffectsPoint.ClearChild();
            EffectsObj = CreatObjectToNGUI.InstantiateObj(IconPrefabManager.Instance.getIcon("JH_UI_BG_7002"), EffectsPoint);
            SpriteSmoothFlag spriteSmoothFlag = EffectsObj.AddComponent<SpriteSmoothFlag>();
            Color color1 = Color.white;
            color1.a = 0;
            Color color2 = Color.white;
            spriteSmoothFlag.BeginFlag(2,color1,color2,EffectsFlagComplete);
        }

        public override void SetCallBackFuntion(ButtonCallBack ButtonCallBack, object obj)
        {
            base.SetCallBackFuntion(ButtonCallBack, obj);
        }

        void EffectsFlagComplete(object obj)
        {
            if (EffectsObj != null)
            {
                Destroy(EffectsObj);
            }
        }

        protected override void SetRecoverFloat(float number)
        {
            RecoveSprite.fillAmount = number / 1;
            if (number == 0&&int.Parse(textLabel.text)>0) 
            {
                SetMyButtonActive(true);
                FlagEffects();
            }
        }

        public override void SetMyButtonActive(bool Flag)
        {
            base.SetMyButtonActive(Flag);
            if (Flag)
            {
                RecoveSprite.fillAmount = 0;
            }
            else
            {
                RecoveSprite.fillAmount = 1;
            }
        }

		public void ShowCopper(bool isFlag, int copperNum)
		{
			Copper.gameObject.SetActive(isFlag);
			SurplusTime.gameObject.SetActive(!isFlag);
			if(isFlag)
			{
				Label_copper.text = "x"+ copperNum.ToString();
			}
		}

    }
}