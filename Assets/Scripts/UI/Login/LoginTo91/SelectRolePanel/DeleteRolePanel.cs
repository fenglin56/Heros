using UnityEngine;
using System.Collections;
using System;

namespace UI.Login
{

    public class DeleteRolePanel : IUIPanel
    {

        //public UILabel MsgLabel;
        public SingleButtonCallBack SureButton;
        public SingleButtonCallBack CancelButton;
        public UIInput InputLabel;        

        private SSActorInfo m_ssActorInfo;
        private bool CanRemove = false;
        private bool BtnCurrentStatus = false;

        void Awake()
        {
            //this.MsgLabel.SetText(LanguageTextManager.GetString("IDS_H1_217"));
            //this.SureButton.SetButtonText(LanguageTextManager.GetString("IDS_H2_55"));
            //this.CancelButton.SetButtonText(LanguageTextManager.GetString("IDS_H2_28"));
            this.SureButton.SetCallBackFuntion(OnSureButtonClick);
            this.CancelButton.SetCallBackFuntion(OnCancelButtonClick);
            SetButtonActive(SureButton,false);
            Close();
        }

        public override void Show()
        {
            this.InputLabel.text = "";
            transform.localPosition = new Vector3(0, 0, -80);
        }
         public void Show(SSActorInfo ssActorInfo)
        {
            this.m_ssActorInfo = ssActorInfo;
            this.InputLabel.text = "";
            transform.localPosition = new Vector3(0,0,-80);
        }
        public override void Close()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }

        public override void DestroyPanel()
        {
            throw new System.NotImplementedException();
        }

        void OnSureButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            if (!LoginManager.Instance.DeleteActorButtonEnable) return;
            LoginManager.Instance.DeleteActorButtonEnable = false;
            NetServiceManager.Instance.LoginService.SendDeleteActorMsg(this.m_ssActorInfo.lActorID);
            //
            Close();
        }

        void OnCancelButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            Close();
        }

        void SetButtonActive(SingleButtonCallBack btn, bool Flag)
        {
            btn.SetImageButtonComponentActive(Flag);
            btn.collider.enabled = Flag;
            btn.SetButtonBackground(Flag ? 1 : 2);
        }

        void Update()
        {
            if (string.Compare(InputLabel.text, "Delete", true) == 0&&InputLabel.text.Length == 6)
            {
                CanRemove = true;
                if (BtnCurrentStatus != CanRemove)
                {
                    BtnCurrentStatus = CanRemove;
                    SetButtonActive(SureButton, BtnCurrentStatus);
                }
            }
            else
            {
                CanRemove = false;
                if (BtnCurrentStatus != CanRemove)
                {
                    BtnCurrentStatus = CanRemove;
                    SetButtonActive(SureButton, BtnCurrentStatus);
                }
            }
        }

    }
}