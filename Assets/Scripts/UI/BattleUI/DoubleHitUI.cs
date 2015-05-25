using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    enum UpdateKind
    {
        None,
        UpdateHit,
        UpdateKill,
        HideEff
    }
    public class DoubleHitUI : IgnoreTimeScale
    {


        private static DoubleHitUI m_instance;
        public static DoubleHitUI Instance
        {
            get { return m_instance; }
        }

        public GameObject JH_Eff_UI_Combo_CD_Star;
        public GameObject JH_Eff_UI_Combo_Txt_Star;
        public GameObject JH_Eff_UI_Combo_Num_Star;
        public GameObject JH_Eff_UI_Combo_KillTxt_Star;
        public GameObject JH_Eff_UI_Combo_KillNum_Star;
        public UILabel Combo_Num_Star_Lable;
        public UILabel Combo_KillNum_Lable;

//        public GameObject Go_Combo_CD_animator;
//        public GameObject Go_Combo_Txt_animator;
//        public GameObject Go_ComCombo_Num_animator;
//        public GameObject Go_Combo_KillTxt_animator;
//        public GameObject Go_Combo_KillNum_animator;

        public Animator Combo_CD_animator;
        public Animator Combo_Txt_animator;
        public Animator ComCombo_Num_animator;
        public Animator Combo_KillTxt_animator;
        public Animator Combo_KillNum_animator;
        public UISlider Slider;

        public Vector3 JoystickStatusPosition;//当为摇杆状态时的位置
        public Vector3 NormalStatusPosition;//为普通状态时的位置
//        public bool IsTest;
//        public int TestValue;
        private GameObject DoubleHitLableObj;
        private GameObject m_doubleHitCommon;
        private SingleButtonCallBack DoubleHitLable;

        private GameObject m_doubleLostBg;
        private GameObject m_doubleLostEffect;

        private float WaitTime = 5;
        private int m_curEctypeCombo;  //当前副本连击数
        private int m_comboSegement;   //当前连击段数
        private int m_hitNumber;  

        private ushort m_currentHitNum;
        private ushort m_currentKillNum;
        private float CoolDownTime;
        private float DefaultTime;
        private bool EffIsShow;
        private UpdateKind CurrentState;
        //private bool Stateupdate;
        private bool FirstHitAppear=false;
        private bool FirstKillAppear=false;
        void Awake()
        {
//
//            Combo_CD_animator=Go_Combo_CD_animator.GetComponent<Animator>();
//            Combo_Txt_animator=Go_Combo_Txt_animator.GetComponent<Animator>();
//            ComCombo_Num_animator=Go_ComCombo_Num_animator.GetComponent<Animator>();
//            Combo_KillTxt_animator=Go_Combo_KillTxt_animator.GetComponent<Animator>();
//            Combo_KillNum_animator=Go_Combo_KillNum_animator.GetComponent<Animator>();
            if(CommonDefineManager.Instance!=null)
            {
            DefaultTime=CommonDefineManager.Instance.CommonDefine.COMBO_TIME;
            }
            m_instance = this;
            UIEventManager.Instance.RegisterUIEvent(UIEventType.DoubleHitUI,ChangeHitCount);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.DoubleKillUI,ChangeKillCount);
            //transform.localPosition = GameManager.Instance.UseJoyStick ? JoystickStatusPosition : NormalStatusPosition;
        }

        void Start()
        {
            if (GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
            {
                GameDataManager.Instance.dataEvent.RegisterEvent(DataType.InitializeEctype, Init);
            }
            else
            {
                Init(null);
            }
        }

        void Init(object obj)
        {
            GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.InitializeEctype, Init);
            SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            m_curEctypeCombo = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId].ComboValue;
        }
        void OnDestroy()
        {
            m_instance = null;
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.DoubleHitUI, ChangeHitCount);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.DoubleKillUI,ChangeKillCount);
        }

        public void ChangeHitCount(object obj)
        {
           //Debug.Log("ChangeHitCount");
            m_currentHitNum=(ushort)obj;
            CoolDownTime=DefaultTime;
            CurrentState=UpdateKind.UpdateHit;
            ChangeState(CurrentState);
          
        }
        public void ChangeKillCount(object obj)
        {
            //Debug.Log("ChangeKillCount");
            m_currentKillNum=(ushort)obj; 
           // Stateupdate=true;
            CurrentState= UpdateKind.UpdateKill;
            ChangeState(CurrentState);
        }

        void ComCombo_Num_animatorPlay(HitState state)
        {
            if(!JH_Eff_UI_Combo_Num_Star.activeSelf)
            {
                JH_Eff_UI_Combo_Num_Star.SetActive(true);
            }
            if( ComCombo_Num_animator.GetInteger("state")==(int)state)
            {
                ComCombo_Num_animator.SetInteger("state",0);
            }
            ComCombo_Num_animator.SetInteger("state", (int)state);
        }

        void  Combo_Txt_animatorPlay(HitState state)
        {
            if(!JH_Eff_UI_Combo_Txt_Star.activeSelf)
            {
                JH_Eff_UI_Combo_Txt_Star.SetActive(true);
            }
            if( Combo_Txt_animator.GetInteger("state")==(int)state)
            {
                Combo_Txt_animator.SetInteger("state",0);
            }
            Combo_Txt_animator.SetInteger("state", (int)state);
        }

        void Combo_KillTxt_animatorPlay(HitState state)
        {
            if(!JH_Eff_UI_Combo_KillTxt_Star.activeSelf)
            {
                JH_Eff_UI_Combo_KillTxt_Star.SetActive(true);
            }
            if( Combo_KillTxt_animator.GetInteger("state")==(int)state)
            {
                Combo_KillTxt_animator.SetInteger("state",0);
            }
            Combo_KillTxt_animator.SetInteger("state", (int)state);
        }

        void Combo_KillNum_animatorPlay(HitState state)
        {
            if(!JH_Eff_UI_Combo_KillNum_Star.activeSelf)
            {
                JH_Eff_UI_Combo_KillNum_Star.SetActive(true);
            }
            if( Combo_KillNum_animator.GetInteger("state")==(int)state)
            {
                Combo_KillNum_animator.SetInteger("state",0);
            }
            Combo_KillNum_animator.SetInteger("state", (int)state);
        }

        void Combo_CD_animatorPlay(HitState state)
        {
            if(!JH_Eff_UI_Combo_CD_Star.activeSelf)
            {
                JH_Eff_UI_Combo_CD_Star.SetActive(true);
            }
            if( Combo_CD_animator.GetInteger("state")==(int)state)
            {
                Combo_CD_animator.SetInteger("state",0);
            }
            Combo_CD_animator.SetInteger("state", (int)state);
        }

        void ChangeState(UpdateKind updateKind)
        {
            if(updateKind==UpdateKind.None)
            {
                return ;
            }
        
            if(updateKind==UpdateKind.UpdateHit)
            {
                ComCombo_Num_animator.SetInteger("state",0);
                if(m_currentHitNum==0)
                {
                   // Debug.Log("清");
                   // Debug.Log("连击中断");
                    FirstHitAppear=false;
                    FirstKillAppear=false;
                    ComCombo_Num_animatorPlay(HitState.OnlyHit_BreakOff);
                    Combo_Txt_animatorPlay(HitState.OnlyHit_BreakOff);
                    Combo_KillTxt_animatorPlay(HitState.OnlyHit_BreakOff);
                    Combo_KillNum_animatorPlay(HitState.OnlyHit_BreakOff);
                    Combo_CD_animatorPlay(HitState.OnlyHit_BreakOff);
                    JH_Eff_UI_Combo_KillTxt_Star.SetActive(false);
                    JH_Eff_UI_Combo_Txt_Star.SetActive(false);
                    if(m_currentKillNum!=0)
                    {
                        Combo_Txt_animatorPlay(HitState.Kill_HitBreakOff);
                        m_currentKillNum=0;
                    }
                    m_currentKillNum=0;
                    return ;
                }
                if(m_currentKillNum<1)
                {
                    if(!FirstHitAppear)
                    {
                        FirstHitAppear=true;
                        ComCombo_Num_animatorPlay(HitState.StartHit);
                        Combo_Txt_animatorPlay(HitState.StartHit);
                        Combo_CD_animatorPlay(HitState.StartHit);
                        Combo_Num_Star_Lable.SetText(m_currentHitNum);
                        //Debug.Log(" 仅连击——连击chuanxian"+m_currentHitNum);
                    }
                    else if(m_currentHitNum>1)
                    {
                        if(JH_Eff_UI_Combo_Num_Star.activeSelf)
                        {
                            Combo_Num_Star_Lable.SetText(m_currentHitNum);
                            //
                            ComCombo_Num_animatorPlay(HitState.OnlyHit_HitAdd);
                            //ComCombo_Num_animator.Play("Eff_lzzjshuzi");
                        }
                        //Debug.Log(" 仅连击——连击增加"+m_currentHitNum);
                    }
                }
                else
                {
                    if(m_currentHitNum<=1)
                    {
                        //现有连杀后有连击，报错
                       // Debug.Log("先有连杀后有连击，报错");
                    }
                    else
                    {
                        Combo_Num_Star_Lable.SetText(m_currentHitNum);
                        //Combo_Txt_animator.Play("");
                       // ComCombo_Num_animator.SetInteger("state",0);
                        ComCombo_Num_animatorPlay(HitState.Kill_HitAdd);
                        //Debug.Log("连杀——连击增加"+m_currentHitNum);
                    }
                }

            }
            else if(updateKind==UpdateKind.UpdateKill)
            {
                Combo_KillNum_animatorPlay(HitState.DefultSate);
                if(m_currentHitNum==0)
                {
                   // Debug.Log("清0");
                    //Debug.Log("连击中断");
                    FirstHitAppear=false;
                    FirstKillAppear=false;
                    ComCombo_Num_animatorPlay(HitState.Kill_HitBreakOff);
                    Combo_Txt_animatorPlay(HitState.Kill_HitBreakOff);
                    Combo_KillTxt_animatorPlay(HitState.Kill_HitBreakOff);
                    Combo_KillNum_animatorPlay(HitState.Kill_HitBreakOff);
                    Combo_CD_animatorPlay(HitState.Kill_HitBreakOff);
                    JH_Eff_UI_Combo_KillTxt_Star.SetActive(false);
                    JH_Eff_UI_Combo_Txt_Star.SetActive(false);
                    m_currentKillNum=0;
                    return ;
                }
                    if(m_currentHitNum<1)
                    {
                    //Debug.Log("先有连杀后有连击，报错");
                    }
                    else
                    {
                    if(!FirstKillAppear)
                    {
                        FirstKillAppear=true;
//                        JH_Eff_UI_Combo_KillTxt_Star.SetActive(true);
//                        JH_Eff_UI_Combo_KillNum_Star.SetActive(true);
                        Combo_Txt_animatorPlay(HitState.StartKill);
                        ComCombo_Num_animatorPlay(HitState.StartKill);
                        Combo_KillTxt_animatorPlay(HitState.StartKill);
                        Combo_KillNum_animatorPlay(HitState.StartKill);
                        Combo_KillNum_Lable.SetText(m_currentKillNum);
                       //Debug.Log("连杀__连杀出现"+m_currentKillNum);

                    }
                    else
                    {

                        Combo_KillNum_animatorPlay(HitState.Kill_KillAdd);
                        //Combo_KillTxt_animator.SetInteger("state",6);
                        Combo_KillNum_Lable.SetText(m_currentKillNum);
                        //Debug.Log("连杀__连杀增加"+m_currentKillNum);
                    }
                    }
              
            }
            else
            {
                //Debug.Log("连击中断");
            }
           
        }
   

    
        void Update()
        {
//            if(IsTest)
//            {
//                JH_Eff_UI_Combo_CD_Star.SetActive(true);
//                JH_Eff_UI_Combo_Txt_Star.SetActive(true);
//                JH_Eff_UI_Combo_Num_Star.SetActive(true);
//                JH_Eff_UI_Combo_KillTxt_Star.SetActive(true);
//                JH_Eff_UI_Combo_KillNum_Star.SetActive(true);
//                ComCombo_Num_animator.SetInteger("state",TestValue);
//                Combo_Txt_animator.SetInteger("state",TestValue);
//                Combo_KillTxt_animator.SetInteger("state",TestValue);
//                Combo_KillNum_animator.SetInteger("state",TestValue);
//                Combo_CD_animator.SetInteger("state",TestValue);
//            }

            float delta=UpdateRealTimeDelta()*1000;
            if(CoolDownTime<=0)
            {
                if(EffIsShow)
                {
                ChangeState(UpdateKind.HideEff);
                    EffIsShow=false;
                }

            }
            else
            {
                Slider.sliderValue=CoolDownTime/DefaultTime;
                EffIsShow=true;
                CoolDownTime-=delta;
            }
        }
    }
    public enum HitState
    {
        DefultSate,
        /// <summary>
        /// 连击开始
        /// </summary>
        StartHit,
        /// <summary>
        /// 只有连击显示时连击增加
        /// </summary>
        OnlyHit_HitAdd,//
        /// <summary>
        /// 只有连击显示时连击中断
        /// </summary>
        OnlyHit_BreakOff,
        /// <summary>
        /// 连杀开始出现
        /// </summary>
        StartKill,
        /// <summary>
        /// 连杀时连击增加
        /// </summary>
        Kill_HitAdd,
        /// <summary>
        /// 连杀时连杀增加
        /// </summary>
        Kill_KillAdd,
        /// <summary>
        /// 连杀时连击中断
        /// </summary>
        Kill_HitBreakOff,//
    }
}