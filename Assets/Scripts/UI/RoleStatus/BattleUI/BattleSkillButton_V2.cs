using UnityEngine;
using System.Collections;



namespace UI.Battle
{
    public class BattleSkillButton_V2 : BattleButton
    {

        private SkillButtonStatus HistroySkillButtonStatus;
        public SkillButtonStatus skillButtonStatus { get; private set; }
        public SkillButtonStatus MpNotEnoughButtonStatus { get; private set; }//当为mp不足时改变的按钮状态
        public SkillConfigData skillConfigData { get; private set; }

        BattleSkillButtonDelegate battleSkillButtonCallBack;


        public UISprite MPNotEnoughICON;
        public UISprite DisableIcon;
        public UISprite WaitIcon;
        private GameObject RecoverFloatObj;
        public Transform EffectsPoint;
        private GameObject EffectsObj;

        public void SetButtonAttribute(SkillConfigData skillConfigData, BattleSkillButtonDelegate battleSkillButtonCallBack)
        {
            this.battleSkillButtonCallBack = battleSkillButtonCallBack;
            skillButtonStatus = SkillButtonStatus.Enable;
            RecoveSprite.fillAmount = 0;
            this.skillConfigData = skillConfigData;
            if (skillConfigData == null)
            {
                SetButtonIcon(null);
                SetCallBackFuntion(null, null);
            }
            else
            {
                SetButtonIcon(skillConfigData.m_icon);
                SetCallBackFuntion(ButtonClick, null);
            }
        }

        void ButtonClick(object obj)
        {
            switch (skillButtonStatus)
            {
                case SkillButtonStatus.Enable:
                    if (battleSkillButtonCallBack != null) { battleSkillButtonCallBack(null); }
                    break;
                case SkillButtonStatus.MPNotEnough:
                    break;
                case SkillButtonStatus.Wait:
                    if (battleSkillButtonCallBack != null) { battleSkillButtonCallBack(null); }
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// 此处直接设置按钮状态
        /// </summary>
        /// <param name="skillButtonStatus"></param>
        public void SetButtonStatus(SkillButtonStatus m_skillButtonStatus)
        {
            switch (skillButtonStatus)
            {
                case SkillButtonStatus.MPNotEnough:
                    if (skillButtonStatus == SkillButtonStatus.MPEnough)
                    {
                        SetButtonMPEnoughStatus();
                    }
                    else
                    {
                        this.MpNotEnoughButtonStatus = m_skillButtonStatus;
                    }
                    break;
                default:
                    ChangeButtonStatus(m_skillButtonStatus);
                    break;
            }
        }

        void ChangeButtonStatus(SkillButtonStatus m_skillButtonStatus)
        {
            switch (m_skillButtonStatus)
            {
                case SkillButtonStatus.Enable:
                    SetButtonEnabel();
                    break;
                case SkillButtonStatus.Disable:
                    SetButtonDisabel();
                    break;
                case SkillButtonStatus.MPEnough:
                    SetButtonMPEnoughStatus();
                    break;
                case SkillButtonStatus.MPNotEnough:
                    SetButtonMPNotEnoughStatus();
                    break;
                case SkillButtonStatus.Recovering:
                    SetButtonRecovering();
                    break;
                case SkillButtonStatus.Wait:
                    SetButtonWait();
                    break;
            }
        }

        void SetButtonMPNotEnoughStatus()
        {
            EffectsPoint.gameObject.SetActive(false);
            RecoveSprite.enabled = false;
            MPNotEnoughICON.enabled = true;
            WaitIcon.enabled = false;
            DisableIcon.enabled = false;
        }

        void SetButtonMPEnoughStatus()
        {
            EffectsPoint.gameObject.SetActive(false);
            RecoveSprite.enabled = false;
            MPNotEnoughICON.enabled = false;
            WaitIcon.enabled = false;
            DisableIcon.enabled = false;
        }

        void SetButtonEnabel()
        {
            EffectsPoint.gameObject.SetActive(true);
            RecoveSprite.enabled = false;
            MPNotEnoughICON.enabled = false;
            WaitIcon.enabled = false;
            DisableIcon.enabled = false;
        }

        void SetButtonDisabel()
        {
            EffectsPoint.gameObject.SetActive(false);
            RecoveSprite.enabled = false;
            MPNotEnoughICON.enabled = false;
            WaitIcon.enabled = false;
            DisableIcon.enabled = true;
        }

        void SetButtonRecovering()
        {
            EffectsPoint.gameObject.SetActive(true);
            RecoveSprite.enabled = true;
            MPNotEnoughICON.enabled = false;
            WaitIcon.enabled = false;
            DisableIcon.enabled = false;
        }

        void SetButtonWait()
        {
            EffectsPoint.gameObject.SetActive(false);
            RecoveSprite.enabled = false;
            MPNotEnoughICON.enabled = false;
            WaitIcon.enabled = true;
            DisableIcon.enabled = false;
        }

        public override void RecoverMyself(float RecoverTime)
        {
            if (this.RecoverFloatObj != null) { Destroy(this.RecoverFloatObj); }
            this.RecoverFloatObj = TweenFloat.Begin(RecoverTime, 1, 0, SetRecoverFloat, this.PlayEffects);
        }

        protected override void SetRecoverFloat(float number)
        {
            RecoveSprite.fillAmount = number / 1;
            if (number == 0) { SetButtonStatus(SkillButtonStatus.Enable); }
        }

        void PlayEffects(object obj)
        {
            EffectsPoint.ClearChild();
            EffectsObj = CreatObjectToNGUI.InstantiateObj(IconPrefabManager.Instance.getIcon("JH_UI_BG_7002"), EffectsPoint);
            SpriteSmoothFlag spriteSmoothFlag = EffectsObj.AddComponent<SpriteSmoothFlag>();
            Color color1 = Color.white;
            color1.a = 0;
            Color color2 = Color.white;
            spriteSmoothFlag.BeginFlag(3, 0.5f, color1, color2, FlagComplete);
        }

        void FlagComplete(object obj)
        {
            if (EffectsObj != null)
            {
                Destroy(EffectsObj);
            }
        }

    }
}
