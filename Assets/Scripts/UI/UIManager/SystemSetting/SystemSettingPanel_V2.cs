using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{

    public class SystemSettingPanel_V2 : BaseUIPanel
    {

        public UILabel UserID;
        public UISlider MusicBar;
        public UISlider SoundBar;
        public UISlider ViewBar;

        public SingleButtonCallBack MagicTypeBtn;
        public SingleButtonCallBack ControllerTypeBtn;

        public SingleButtonCallBack OpenBaseSkillBtn;
        public SingleButtonCallBack CloseBaseSkillBtn;

        public SingleButtonCallBack EnabelJoyStickBtn;
        public SingleButtonCallBack DisabelJoyStickBtn;

        public SingleButtonCallBack ShowHurtNumBtn;
        public SingleButtonCallBack HideHurtNumBtn;

        public SingleButtonCallBack BackBtn;
        public SingleButtonCallBack QuitBtn;

        //public GameObject UIBottomBtnPrefab;
       
        private bool MagicBtnType = true;
        private bool ControllerType = true;

        void Awake()
        {
           // if (UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform); }
            OpenBaseSkillBtn.SetCallBackFuntion(OnOpenBaseSkillBtnClick);
            CloseBaseSkillBtn.SetCallBackFuntion(OnCloseBaseSkillBtnClick);
            EnabelJoyStickBtn.SetCallBackFuntion(OnEnabelJoyStickBtnClick);
            DisabelJoyStickBtn.SetCallBackFuntion(OnDesabelJoyStickBtnClick);
            if(ShowHurtNumBtn&&HideHurtNumBtn)
            {
            ShowHurtNumBtn.SetCallBackFuntion(ShowHurtNumBtnClick);
            HideHurtNumBtn.SetCallBackFuntion(HideHurtNumBtnClick);
            }
            QuitBtn.SetCallBackFuntion(OnQuitBtnClick);
            BackBtn.SetCallBackFuntion(OnBackButtonTapped);
            MusicBar.onValueChange += OnMusicBarChange;
            SoundBar.onValueChange += OnSoundBarChange;
            ViewBar.onValueChange += OnDragViewBarFinish;
            MagicTypeBtn.SetCallBackFuntion(OnMagicTypeBtnClick);
            ControllerTypeBtn.SetCallBackFuntion(OnControllerTypeBtnClick);
            MusicBar.sliderValue = GameManager.Instance.m_gameSettings.BgmVolume;
            SoundBar.sliderValue = GameManager.Instance.m_gameSettings.SfxVolume;
            ViewBar.sliderValue = (GameManager.Instance.m_gameSettings.GameViewLevel) / (float)(CommonDefineManager.Instance.CommonDefineFile._dataTable.CameraDistanceList.Count -1);
            MagicBtnType = GameManager.Instance.m_gameSettings.DoubleClickSkill;
            MagicTypeBtn.SetButtonBackground(MagicBtnType ? 2 : 1);
            ResetBaseSkillBtnStatus();
            ResetJoyBtnStatus();
            ResetHurtNumStatus();
            UserID.SetText(string.Format(LanguageTextManager.GetString("IDS_I30_6"),PlayerManager.Instance.FindHeroDataModel().ActorID));
        }

   

        public override void Close()
        {
            if (!IsShow)
                return;
            GameManager.Instance.m_gameSettings.Save();
            base.Close();
        }
//        void ShowBottomBtn()
//        {
//            CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonTapped);
//            CommonBtnInfo btnInfo1 = new CommonBtnInfo(1, "JH_UI_Button_1116_16", "JH_UI_Button_1116_00", OnQuitBtnClick);
//           // commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() { btnInfo,btnInfo1 });
//        }

        void OnEnabelJoyStickBtnClick(object obj)
        {
            TraceUtil.Log("SetJoyStick:True");
            OnOpenBaseSkillBtnClick(null);
            GameManager.Instance.m_gameSettings.JoyStickMode = true;
            ResetJoyBtnStatus();
        }

        void OnDesabelJoyStickBtnClick(object obj)
        {
            TraceUtil.Log("SetJoyStick:false");
            GameManager.Instance.m_gameSettings.JoyStickMode = false;
            ResetJoyBtnStatus();
        }

        void ResetJoyBtnStatus()
        {
            EnabelJoyStickBtn.SetButtonBackground(GameManager.Instance.m_gameSettings.JoyStickMode ? 2 : 1);
            DisabelJoyStickBtn.SetButtonBackground(GameManager.Instance.m_gameSettings.JoyStickMode ? 1 : 2);
        }

        void ShowHurtNumBtnClick(object obj)
        {
            GameManager.Instance.m_gameSettings.ShowHurtNum=true;
            ResetHurtNumStatus();
        }

        void HideHurtNumBtnClick(object obj)
        {
            GameManager.Instance.m_gameSettings.ShowHurtNum=false;
            ResetHurtNumStatus();
        }

        void ResetHurtNumStatus()
        {
            ShowHurtNumBtn.SetButtonBackground(GameManager.Instance.m_gameSettings.ShowHurtNum ? 2 : 1);
            HideHurtNumBtn.SetButtonBackground(GameManager.Instance.m_gameSettings.ShowHurtNum ? 1 : 2);
        }
        void OnMusicBarChange(float value)
        {
            GameManager.Instance.m_gameSettings.BgmVolume = value;                      
        }

        void OnSoundBarChange(float value)
        {
            GameManager.Instance.m_gameSettings.SfxVolume = value;   
        }
        void OnDragViewBarFinish(float value)
        {
            float step= ViewBar.numberOfSteps-1;

			GameManager.Instance.m_gameSettings.GameViewLevel = Mathf.RoundToInt(ViewBar.sliderValue * step);
			TraceUtil.Log(SystemModel.wanglei,"ChangeViewPoint:" + GameManager.Instance.m_gameSettings.GameViewLevel);
			Vector3 distance = CommonDefineManager.Instance.GetCameraDistanceFromPlayer();
			BattleManager.Instance.SetCameraDistance(distance);
        }

        void OnMagicTypeBtnClick(object obj)
        {
            MagicBtnType = !MagicBtnType;
            MagicTypeBtn.SetButtonBackground(MagicBtnType ? 2 : 1);
            //TraceUtil.Log("SetMagicBtnType:" + MagicBtnType);
			
			GameManager.Instance.m_gameSettings.DoubleClickSkill = MagicBtnType;
        }

        void OnControllerTypeBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_System_Close");
            ControllerType = !ControllerType;
            ControllerTypeBtn.SetButtonBackground(ControllerType ? 2 : 1);
            //TraceUtil.Log("SetControllerBtnType:" + ControllerType);
        }

        void OnOpenBaseSkillBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_System_Activate");
            GameManager.Instance.m_gameSettings.DoubleClickSkill = false;
            ResetBaseSkillBtnStatus();
            //TraceUtil.Log("OpenBaseSkill" );
        }

        void OnCloseBaseSkillBtnClick(object obj)
        {
            if (GameManager.Instance.m_gameSettings.JoyStickMode)
            {
                MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_H1_509"),1);
                return;
            }
            SoundManager.Instance.PlaySoundEffect("Sound_Button_System_Close");
			GameManager.Instance.m_gameSettings.DoubleClickSkill = true;
            ResetBaseSkillBtnStatus();
            //TraceUtil.Log("CloseBaseSkill");
        }

        void ResetBaseSkillBtnStatus()
        {
          
            bool isBaseSkillClick = !GameManager.Instance.m_gameSettings.DoubleClickSkill;
            CloseBaseSkillBtn.SetButtonBackground(isBaseSkillClick?1:2);
            OpenBaseSkillBtn.SetButtonBackground(isBaseSkillClick?2:1);
        }

        private void OnBackButtonTapped(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_System_Leave");
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            OnBackBtnClick(obj);
        }

        void OnBackBtnClick(object obj)
        {
            CleanUpUIStatus();
            Close();
        }

        void OnQuitBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_System_Longin");
            MessageBox.Instance.Show(1,"",LanguageTextManager.GetString("IDS_I30_3"),LanguageTextManager.GetString("IDS_I30_5"),LanguageTextManager.GetString("IDS_I30_4"),SureQuit,Canel);

            //TraceUtil.Log("QuitGame");

        }
        void SureQuit()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_System_Longin_OK");
            GameManager.Instance.QuitToLogin();
        }
        void Canel()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_System_Longin_Cancel");
        }

    }
}