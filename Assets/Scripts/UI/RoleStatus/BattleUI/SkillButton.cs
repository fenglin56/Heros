//using UnityEngine;
//using System.Collections;

//namespace UI.Battle
//{


//    public enum BattleButtonPosition
//    {
//        SkillButton1 = 0,
//        SkillButton2,
//        SkillButton3,
//        SkillButton4,
//        HealthButton,
//        MagicButton,
//        PauseButton,
//    }

//    public class SkillButton : MonoBehaviour
//    {
//        //*************单个技能按钮状态*******************
//        //SkillButtonStatus ButtonStatus= SkillButtonStatus.Disable;
//        SkillButtonStatus LastButtonStatus = SkillButtonStatus.Disable;

//        SkillButtonInfo m_skillButtonInfo;

//        public UILabel SkillLable;//按钮文字
//        public UISprite CancelSkillStatus;//按钮打叉图标
//        public Flag RecoverCompleteFlag;//技能恢复完成时的闪烁提示
//        public UIFilledSprite RecoverStatus;//显示按钮恢复时的状态条
//        public BattleButtonPosition ButtonID;//按钮ID,一共有四个技能按钮，每个按钮ID手动设置

//        bool ShowRecoiverAnim;//显示状态恢复动画
//        //private int SkillID;
//        //private float RecoverTime;//恢复时间
//        private SkillButtonCallBack ButtonCallBack;
		
//        void Awake()
//        {
//            m_skillButtonInfo = new SkillButtonInfo();
//            m_skillButtonInfo.ButtonPosition = ButtonID;
//             SetButtonStatus(SkillButtonStatus.Empty);
//        }
		
//        void Update()
//        {
//            if (ShowRecoiverAnim)
//            {
//                RecoverStatus.fillAmount += Time.deltaTime / m_skillButtonInfo.RecoverTime;
//                if (RecoverStatus.fillAmount >= 1)//此处测试用
//                {
//                    SetButtonStatus(SkillButtonStatus.Enable);
//                }
//            }
//        }

//        public void SetButtonAttribute(SkillButtonInfo skillButtonInfo)//此处设置按钮，包括按钮文字及图片
//        {
//            if (m_skillButtonInfo == null)
//            {
//                m_skillButtonInfo = new SkillButtonInfo(skillButtonInfo.buttonCallBack, skillButtonInfo.ButtonPosition);
//            }
//            if (skillButtonInfo.ButtonPosition != m_skillButtonInfo.ButtonPosition)
//                return;
//            skillButtonInfo.Copy(ref m_skillButtonInfo);
            
//            this.ButtonCallBack = new SkillButtonCallBack(skillButtonInfo.buttonCallBack);
//            SetButtonStatus(m_skillButtonInfo.ButtonStatus);
//            //TraceUtil.Log("SetMyBtnEnable :"+gameObject.name);
//        }

//        public void ClearButton(BattleButtonPosition skillButtonInfo)//此处清空图片及文字
//        {
//            if (skillButtonInfo != m_skillButtonInfo.ButtonPosition)
//                return;
//            SetButtonStatus(SkillButtonStatus.Empty);
//        }

//        void SetButtonStatus(SkillButtonStatus buttonStatus)//设置此时按钮状态
//        {
//            m_skillButtonInfo.ButtonStatus = buttonStatus;
//            switch (buttonStatus)
//            {
//                case SkillButtonStatus.Enable:
//                    SetButtonEnable();
//                    if (LastButtonStatus == SkillButtonStatus.Recovering){ RecoverCompleteFlag.StartTwinkling(2);}
//                    break;
//                case SkillButtonStatus.Wait:
//                    SetButtonWait();
//                    break;
//                case SkillButtonStatus.Recovering:
//                    SetButtonRecover();
//                    break;
//                case SkillButtonStatus.Disable:
//                    SetButtonDisable();
//                    break;
//                case SkillButtonStatus.Empty:
//                    SetButtonEmpty();
//                    break;
//                default:
//                    break;
//            }
//            LastButtonStatus = buttonStatus;
//        }

//        void SetButtonEnable()
//        {
//            ShowRecoiverAnim = false;
//            RecoverStatus.fillAmount = 1;
//            CancelSkillStatus.enabled = false;
//        }

//        void SetButtonWait()
//        {
//            CancelSkillStatus.enabled = true;
//        }

//        void SetButtonRecover()
//        { 
//            RecoverStatus.fillAmount = 0;
//            ShowRecoiverAnim = true;
//            RecoverCompleteFlag.StopFlag();
//        }

//        void SetButtonDisable()
//        {
//            RecoverStatus.fillAmount = 0;
//            CancelSkillStatus.enabled = false;
//        }

//        void SetButtonEmpty()
//        {
//            ShowRecoiverAnim = false;
//            this.ButtonCallBack = null;
//            RecoverStatus.fillAmount = 0;
//            CancelSkillStatus.enabled = false;
//            RecoverCompleteFlag.StopFlag();
//        }

//        void OnClick()
//        {
//            if (this.ButtonCallBack != null)
//            {
//                this.ButtonCallBack(m_skillButtonInfo.ButtonStatus,this.m_skillButtonInfo);
//            }
//            switch (m_skillButtonInfo.ButtonStatus)
//            {
//                case SkillButtonStatus.Enable://触发技能
//                    SetButtonStatus(SkillButtonStatus.Recovering);
//                    break;
//                case SkillButtonStatus.Wait://取消技能释放
//                    SetButtonStatus(SkillButtonStatus.Enable);
//                    break;
//                case SkillButtonStatus.Recovering:  //技能恢复中
//                    break;
//                case SkillButtonStatus.Disable://技能不能被激发
//                    break;
//                default:
//                    break;
//            }
//        }
        
//    }
//}
