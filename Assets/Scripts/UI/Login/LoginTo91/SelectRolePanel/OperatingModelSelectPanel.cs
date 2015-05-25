using UnityEngine;
using System.Collections;

namespace UI.Login
{

    public class OperatingModelSelectPanel : MonoBehaviour
    {
        public enum btnType { JoyStick,NorMal,None}

        public SingleButtonCallBack JoystickBtn;
        public SingleButtonCallBack NormalBtn;
        public SingleButtonCallBack SureBtn;

        btnType CurrentSelectBtnType = btnType.None;
        RoleSelectPanelV2 MyParent;

        void Awake()
        {
            JoystickBtn.SetCallBackFuntion(OnModelBtnClick,btnType.JoyStick);
            NormalBtn.SetCallBackFuntion(OnModelBtnClick,btnType.NorMal);
            SureBtn.SetCallBackFuntion(OnSureBtnClick);
            OnModelBtnClick(btnType.None);
        }

        public void Show(RoleSelectPanelV2 myParent)
        {
            MyParent = myParent;
            OnModelBtnClick(btnType.None);
        }

        void OnModelBtnClick(object obj)
        {
            CurrentSelectBtnType = (btnType)obj;
            JoystickBtn.BackgroundSprite.gameObject.SetActive(CurrentSelectBtnType == btnType.JoyStick);
			JoystickBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(CurrentSelectBtnType == btnType.JoyStick ? 2:1));
            NormalBtn.BackgroundSprite.gameObject.SetActive(CurrentSelectBtnType == btnType.NorMal);
			NormalBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(CurrentSelectBtnType == btnType.NorMal ? 2 : 1));
        }

        void OnSureBtnClick(object obj)
        {
            if (CurrentSelectBtnType == btnType.None)
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_546"), 1);
                return;
            }
            GameManager.Instance.m_gameSettings.JoyStickMode = CurrentSelectBtnType == btnType.JoyStick;
            gameObject.SetActive(false);
            GameManager.Instance.m_gameSettings.Save();
        }
    }
}