using UnityEngine;
using System.Collections;


public enum SkillButtonStatus
{
    Enable,//激活的，点击即可释放
    Wait,//正在释放中，点击可以取消
    Recovering,//释放后，正在恢复中，点击无效
    Disable,//冻结的,真气不足时
    MPNotEnough,//MP不足
    MPEnough,//MP不足
    Empty,//空的
}

namespace UI.Battle
{
    public enum SpecialSkillType
    {
		NormalBtn = -1,  // 普通攻击按钮
        Normal = 0,  //普通技能
        Roll = 1,   //翻滚
        Explode = 2,  //爆气
        Meaning = 3, //奥义
    }

    public enum BarCutDownMode {MaxToMin,MinToMax }

    public delegate bool BattleSkillButtonDelegate(BattleSkillButton ButtonInstance);
    /// <summary>
    /// new
    /// </summary>
    public class BattleSkillButton : BattleButton
    {

        public SkillButtonStatus skillButtonStatus
        {
            get { return btnStatus; }
            protected set
            {
//                if (this.skillConfigData != null)
//                {
//                    Debug.LogWarning("设置按钮状态：" + LanguageTextManager.GetString(this.skillConfigData.m_name) + ":" + value);
//                }
                btnStatus = value;
                switch (value)
                {
                    case SkillButtonStatus.Disable:
                    case SkillButtonStatus.MPNotEnough:
                        //Debug.LogWarning("设置按钮半透明状态：" + LanguageTextManager.GetString(this.skillConfigData.m_name) + ":" + value);
                        PanelList.ApplyAllItem(P=>P.alpha = 0.3f);
                        break;
                    default:
                        PanelList.ApplyAllItem(P => P.alpha = 1f);
                        break;
                }
                //if (skillConfigData != null) { Debug.LogWarning(LanguageTextManager.GetString(skillConfigData.m_name) + "：设置按钮状态：" + value); }
            }
        }        
        public SkillConfigData skillConfigData { get; private set; }
        public SpriteSwith[] JoystickSprite;
        public UISprite MPNotEnoughICON;
        public SingleButtonCallBack BreakLevelLabel;
        BattleSkillButtonDelegate battleSkillButtonCallBack;
        public Transform EffectsPoint;

        public Transform BreackLevelLabelTransform;
        public Vector3 BreackLevelLabelJoySticPos;
        public Vector3 BreackLevelLabelNormalPos;

        // Vector2 UIBarRecoverRange = new Vector2(0,1);
        public SpecialSkillType SpecialType { set; get; }
        protected GameObject EffectsObj;
        protected bool CanChangeButton = true;
        protected bool CanClick = true;
        protected bool m_canChange = false;
        SkillButtonStatus btnStatus;

        protected SkillButtonStatus HistroySkillButtonStatus;
        protected GameObject RecoverFloatObj;
        public int MyBtnIndex { get; private set; }
        public float barFromValue = 0;
        public float barToValue = 1;
        public BarCutDownMode BarCutDownMode = BarCutDownMode.MaxToMin;

		public UILabel Label_CDTime;

		public Transform Trans_SpecialConsumptionPoint;
		public GameObject[] SpecialConsumption = new GameObject[3];

        //妖女技能次数
		public GameObject SirenSkillInterface;
		public UILabel Label_SirenSkillValue;
		public GameObject MultiSegmentsSkillEff{ get; private set; }   //多段技能特效
		public GameObject EffectPrefab;//闪动特效
		public GameObject SirenSkillEffectPrefab;//妖女技能特效
        public bool IsResidentEffect = false;//闪动特效是否持续显示

        bool Isjoystick;

        public UIPanel[] PanelList;

		private GameObject m_CDTimeTweenObj = null;

        void Awake()
        {
            Isjoystick = GameManager.Instance.UseJoyStick;
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveSettleAccount,SetBtnActiveFalse);
            this.BackgroundSwitch.ChangeSprite(1);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveSettleAccount, SetBtnActiveFalse);
        }

        void SetBtnActiveFalse(object obj)
        {
            if (SpecialType == SpecialSkillType.Normal)
                this.CanClick = false;
        }
		public void ShowOrHideMultiSegmentsSkillEff(bool flag)
		{
			if (flag) {
								if (MultiSegmentsSkillEff == null) {
										MultiSegmentsSkillEff = NGUITools.AddChild (gameObject, BattleSkillButtonManager.Instance.BattleMultiSegmentsSkillEff);
								}
						}
			else {
				Destroy(MultiSegmentsSkillEff);
				MultiSegmentsSkillEff=null;
						}
				}
        public void SetButtonAttribute(SkillConfigData skillConfigData, BattleSkillButtonDelegate battleSkillButtonCallBack, SpecialSkillType type, int buttonIndex, bool IsJoyStick)
        {
            this.Isjoystick = IsJoyStick;
            JoystickSprite.ApplyAllItem(P=>P.ChangeSprite(IsJoyStick?2:1));
            if (BreackLevelLabelTransform != null)
            {
                BreackLevelLabelTransform.transform.localPosition = Isjoystick ? BreackLevelLabelJoySticPos : BreackLevelLabelNormalPos;
            }
            MPNotEnoughICON.enabled = false;
            this.battleSkillButtonCallBack = battleSkillButtonCallBack;
            SpecialType = type;
            skillButtonStatus = SkillButtonStatus.Enable;
            MyBtnIndex = buttonIndex;
            RecoveSprite.fillAmount = BarCutDownMode == Battle.BarCutDownMode.MaxToMin ? 0 : 1;
            this.skillConfigData = skillConfigData;
            if (skillConfigData == null)
            {
                SetButtonIcon(null);
                SetCallBackFuntion(null, null);
            }
            else
            {
                SetButtonIcon(Isjoystick?skillConfigData.Icon_CirclePrefab:skillConfigData.m_icon);
                SetCallBackFuntion(ButtonClick, null);
            }
            if (type == SpecialSkillType.Normal)
            {
                BreakLevelLabel.gameObject.SetActive(false);
                BreakLevelLabel.SetButtonText((skillConfigData.m_breakLevel - 1).ToString());
            }
            else
            {
                BreakLevelLabel.gameObject.SetActive(false); 
				UI.CreatObjectToNGUI.InstantiateObj(skillConfigData.energyComsumePrefab,Trans_SpecialConsumptionPoint);
            }
        }

        public override void SetMyButtonActive(bool Flag)
        {
//            Debug.Log ("SetChildButtonActice:" + Flag);
            if (Flag) 
            { 
                if(RecoveSprite!=null) 
                    RecoveSprite.fillAmount =BarCutDownMode == Battle.BarCutDownMode.MaxToMin?0:1; 
            }
            else 
            { 
                if (RecoveSprite != null) 
                    RecoveSprite.fillAmount =BarCutDownMode == Battle.BarCutDownMode.MaxToMin? 1:0; 
            }
            //if (boxCollider == null)
            //{
            //    boxCollider = gameObject.GetComponent<BoxCollider>();
            //}            
            //boxCollider.enabled = Flag;
        }

        /// <summary>
        /// 设置等待状态的按钮为激活状态
        /// </summary>
        /// <param name="?"></param>
        public void SetWaitButtonEnable(BattleSkillButton battleSkillButton)
        {
            if (battleSkillButton != this && skillButtonStatus == SkillButtonStatus.Wait)
            {
                SetButtonStatus(SkillButtonStatus.Enable);
            }
        }
        public void ButtonClick(object obj)
        {
            if (!CanClick)
                return ;
            //if (!CanChangeButton) { return; }
            //print("点击特殊技能按钮");
            var clickBtn = GetComponent<SkillBtnRemember>();
            if (clickBtn != null)
            {
                bool needRemember = false;
                switch (skillButtonStatus)
                {
                    case SkillButtonStatus.Recovering:  //CD中 按钮记忆处理                    
                        needRemember = true;
                        break;
                    case SkillButtonStatus.Enable:
                        if (clickBtn != null && !((PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour).SkillCanBeFire(this))
                        {
                            needRemember = true;
                        }
                        break;
                }
                if (needRemember)
                {
					//Debug.Log("NeedRemember");
                    clickBtn.OnSkillBtnClicked(false);
                }
				else
				{
					ButtonClickWithRet(obj);
				}
            }
			else
			{
            	ButtonClickWithRet(obj);
			}
        }
        public bool ButtonClickWithRet(object obj)
        {
            //Debug.Log(CanClick + "  " + skillButtonStatus);
            bool flag= false;
            if (!CanClick)
                return flag;
            //if (!CanChangeButton) { return; }
            //print("点击特殊技能按钮");
            switch (skillButtonStatus)
            {
                case SkillButtonStatus.Enable:
                    if (EffectsObj != null) 
                    {
                        Destroy(EffectsObj);
                    }
                    if (battleSkillButtonCallBack != null) { flag=battleSkillButtonCallBack(this); }
                    BattleSkillButtonManager.Instance.SetWaitBtnsEnable(this);
                    break;
                case SkillButtonStatus.Wait:
                    if (battleSkillButtonCallBack != null) { flag=battleSkillButtonCallBack(this); }
                    SetButtonStatus(SkillButtonStatus.Enable);
                    SetSecondIcon(null);
                    break;
                case SkillButtonStatus.Recovering:  //CD中 按钮记忆处理
                    //var clickBtn = GetComponent<SkillBtnRemember>();
                    //if (clickBtn != null && clickBtn.BtnMemTime<=0)
                    //{
                    //    clickBtn.OnSkillBtnClicked(false);
                    //}
                    break;
                case SkillButtonStatus.Disable:
                    break;
                case SkillButtonStatus.Empty:
                    break;
                case SkillButtonStatus.MPNotEnough:
                    if (SpecialType == SpecialSkillType.Normal)
                    {
                        //MessageBox.Instance.ShowUnlockTips(3, LanguageTextManager.GetString("IDS_H1_97"), 1);
                        UIEventManager.Instance.TriggerUIEvent(UIEventType.FlagMPBar, null);
                    }
                    break;
                default:
                    break;
            }

            return flag;
        }

        /// <summary>
        /// 此处直接设置按钮状态
        /// </summary>
        /// <param name="skillButtonStatus"></param>
        public void SetButtonStatus(SkillButtonStatus skillButtonStatus)
        {
            //TraceUtil.Log("尝试设置按钮状态：" +LanguageTextManager.GetString( this.skillConfigData.m_name)+ skillButtonStatus);
            //if (!CanChangeButton) { return; }
            //Debug.LogWarning(LanguageTextManager.GetString(skillConfigData.m_name) + "：设置按钮状态：" + skillButtonStatus+",CurrentStatus:"+this.skillButtonStatus);
            switch (skillButtonStatus)
            {
                case SkillButtonStatus.Enable:
                    if (this.skillButtonStatus == SkillButtonStatus.Recovering||this.skillButtonStatus == SkillButtonStatus.MPNotEnough) { return; }
                    if (RecoverFloatObj != null)
                    {
                        Destroy(this.RecoverFloatObj);						
                        RecoveSprite.fillAmount =BarCutDownMode == Battle.BarCutDownMode.MaxToMin? 0:1;
                    }
					if(m_CDTimeTweenObj != null)
					{
						Destroy(m_CDTimeTweenObj);
					}
                    SetMyButtonActive(true);
                    SetSecondIcon(null);
                    break;
                case SkillButtonStatus.Wait:
                    SetSecondIcon(IconPrefabManager.Instance.getIcon("JH_UI_Icon_001"));
                    break;
                case SkillButtonStatus.Recovering:
                    SetMyButtonActive(false);
                    SetSecondIcon(null);
                    //if (this.skillButtonStatus == SkillButtonStatus.MPNotEnough)
                    //{
                        HistroySkillButtonStatus = skillButtonStatus;
                    //}
                    break;
                case SkillButtonStatus.Disable:
                    SetSecondIcon(null);
                    SetMyButtonActive(false);
                    break;
                case SkillButtonStatus.MPNotEnough:
                    break;
                case SkillButtonStatus.Empty:
                    break;
                default:
                    break;
            }
            if (this.skillButtonStatus != SkillButtonStatus.MPNotEnough)
            {
                this.skillButtonStatus = skillButtonStatus;
            }
        }
        
        public void PlayEffect(bool Flag)
        {
            if (Flag)
            {
                if (m_canChange||skillButtonStatus == SkillButtonStatus.Recovering)
                {
                    return;
                }
                PlayEffects(null);
            }

            m_canChange = Flag;
        }

        public void ActiveStepButton()
        {
            if (this.skillButtonStatus == SkillButtonStatus.Enable)
            {
                PlayEffects(null);
            }
        }
        protected bool m_isDisable = false;

		public void SetStepDisable(bool flag)
		{
			m_isDisable = !flag;

//			if(m_isDisable)
//			{
//
//			}
//			else
//			{
//
//			}
		}

        public void SetButtonStatus(bool Flag)
        {
            //m_isDisable = !Flag;

			if (CanChangeButton != Flag)
            {
				this.CanChangeButton = Flag;
            if (Flag)
            {
                ActiveStepButton();
                //SetButtonStatus(SkillButtonStatus.Enable);
                if (RecoverFloat != 0)
                {
                    this.skillButtonStatus = this.HistroySkillButtonStatus;
                }
                else
                {
                    this.skillButtonStatus = SkillButtonStatus.Enable;
                    SetMyButtonActive(true);
                }
                this.MPNotEnoughICON.enabled = false;
                this.RecoveSprite.gameObject.SetActive(true);
                this.IconPoint2.gameObject.SetActive(true);
            }
            else
            {
                SetMyButtonActive(false);
				if(EffectsObj!=null)
				{
					Destroy(EffectsObj);
				}
                HistroySkillButtonStatus = this.skillButtonStatus;
                this.skillButtonStatus = SkillButtonStatus.MPNotEnough;
                this.MPNotEnoughICON.enabled = true;
                this.RecoveSprite.gameObject.SetActive(false);
                this.IconPoint2.gameObject.SetActive(false);
            }
			}
        }
		//jamfing add of use task guide//
		public int skillEnergyComsume = 0;
        public void SetButtonMPStatus(bool Flag)
        {
            if (m_isDisable)
            {
                return;
            }
     
            if (CanChangeButton != Flag)
            {
                //Debug.LogWarning("改变技能图标状态：" + Flag + ",CurrentStatus：" + this.skillButtonStatus);
                //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"设置技能：" + LanguageTextManager.GetString(this.skillConfigData.m_name) + ":" + Flag.ToString());
                this.CanChangeButton = Flag;
                if (Flag)
                {
                    //SetButtonStatus(SkillButtonStatus.Enable);
                    if (RecoverFloat != 0)
                    {
                        this.skillButtonStatus = this.HistroySkillButtonStatus;
                    }
                    else
                    {
                        this.skillButtonStatus = SkillButtonStatus.Enable;
                        SetMyButtonActive(true);
                    }
                    this.MPNotEnoughICON.enabled = false;
                    this.RecoveSprite.gameObject.SetActive(true);
                    this.IconPoint2.gameObject.SetActive(true);
                }
                else
                {
                    SetMyButtonActive(true);
                    EffectsPoint.ClearChild();
                    HistroySkillButtonStatus = this.skillButtonStatus;
                    this.skillButtonStatus = SkillButtonStatus.MPNotEnough;
                    this.MPNotEnoughICON.enabled = true;
                    this.RecoveSprite.gameObject.SetActive(false);
                    this.IconPoint2.gameObject.SetActive(false);
                }
            }
        }

		public void SetSirenSkillButtonMPStatus(bool Flag)
		{
			if (m_isDisable)
			{
				return;
			}
			
			if (CanChangeButton != Flag)
			{
				this.CanChangeButton = true;
				if (Flag)
				{
					if (RecoverFloat != 0)
					{
						this.skillButtonStatus = this.HistroySkillButtonStatus;
					}
					else
					{
						this.skillButtonStatus = SkillButtonStatus.Enable;
						SetMyButtonActive(true);
					}
					this.MPNotEnoughICON.enabled = false;
					this.RecoveSprite.gameObject.SetActive(true);
					this.IconPoint2.gameObject.SetActive(true);
				}
				else
				{
					SetMyButtonActive(true);
					EffectsPoint.ClearChild();
					HistroySkillButtonStatus = this.skillButtonStatus;
					this.skillButtonStatus = SkillButtonStatus.MPNotEnough;
					this.MPNotEnoughICON.enabled = true;
					this.RecoveSprite.gameObject.SetActive(true);
					this.IconPoint2.gameObject.SetActive(false);
				}
			}
		}


        public override void RecoverMyself(float RecoverTime)
        {
            if (this.RecoverFloatObj != null) { Destroy(this.RecoverFloatObj); }
			if(m_CDTimeTweenObj != null){Destroy(m_CDTimeTweenObj);}
            RecoveSprite.fillAmount = 0;
            RecoverFloat = 0;
            if (skillButtonStatus == SkillButtonStatus.MPNotEnough && RecoverTime == 0) { return; }
            int fromValue = BarCutDownMode == Battle.BarCutDownMode.MaxToMin ? 1 : 0;
            int toValue = BarCutDownMode == Battle.BarCutDownMode.MaxToMin ? 0 : 1;
            //Debug.LogWarning(string.Format("冷却技能：{0}=>{1}", fromValue, toValue));
			if(EffectsObj!=null){Destroy(EffectsObj);}
            this.RecoverFloatObj = TweenFloat.Begin(RecoverTime, fromValue, toValue, SetRecoverFloat,this.PlayEffects);
			m_CDTimeTweenObj = TweenFloat.Begin(RecoverTime, RecoverTime, 0, ShowCDTime, ShowCDTimeOver);
			if(RecoverTime > 0)
			{
				Label_CDTime.gameObject.SetActive(true);
			}
        }
		void ShowCDTime(float cdTime)
		{
			Label_CDTime.text = ((int)cdTime+1).ToString();
		}
		void ShowCDTimeOver(object obj)
		{
			Label_CDTime.gameObject.SetActive(false);
		}

        public float RecoverFloat = 0;
        protected override void SetRecoverFloat(float number)
        {
            this.RecoverFloat = number;
            if (!CanChangeButton)
            {
                //Destroy(this.RecoverFloatObj);
                RecoveSprite.fillAmount = 0 ;
                //SetMyButtonActive(true);
                return;
            }
            RecoveSprite.fillAmount = number* (barToValue - barFromValue) + barFromValue;
		
            //TraceUtil.Log("RecoverFloat:" + RecoveSprite.fillAmount);
            //if (number == 0) { SetMyButtonActive(true); }
        }

        public void PlayEffects(object obj)
        {
            if (!CanChangeButton) { return; }
            this.skillButtonStatus = SkillButtonStatus.Enable;
            SetMyButtonActive(true);
            EffectsPoint.ClearChild();
            if (IconPrefabManager.Instance != null)
            {
                //EffectsObj = CreatObjectToNGUI.InstantiateObj(IconPrefabManager.Instance.getIcon("JH_UI_BG_7002"), EffectsPoint);
				if(SpecialType == SpecialSkillType.Meaning)
				{
					EffectsObj = CreatObjectToNGUI.InstantiateObj(SirenSkillEffectPrefab, EffectsPoint);
				}
				else
				{
					EffectsObj = CreatObjectToNGUI.InstantiateObj(EffectPrefab, EffectsPoint);
					SpriteSmoothFlag spriteSmoothFlag = EffectsObj.AddComponent<SpriteSmoothFlag>();
					Color color1 = Color.white;
					color1.a = 0;
					Color color2 = Color.white;
					spriteSmoothFlag.BeginFlag(3,0.5f,color1,color2,FlagComplete);
				}
            }
           
			Label_CDTime.gameObject.SetActive(false);

            //Color color1 = Color.white;
            //color1.a = 0;
            //EffectsObj.GetComponent<UISprite>().alpha = 0;
            //Color color2 = Color.white;
            //TweenAlpha.Begin(EffectsObj,0.2f,1);
            ////spriteSmoothFlag.BeginFlag(2, color1, color2, EffectsFlagComplete);
            //StartCoroutine(EffectsDisable());
        }

        void FlagComplete(object obj)
        {
            if (EffectsObj != null && !IsResidentEffect)
            {
                Destroy(EffectsObj);
            }
        }

        //IEnumerator EffectsDisable()
        //{
        //    yield return new WaitForSeconds(0.2f);
        //    TweenAlpha.Begin(EffectsObj,0.2f,0);
        //    StartCoroutine(EffectsFlagComplete());
        //}

        //IEnumerator EffectsFlagComplete()
        //{
        //    yield return new WaitForSeconds(0.2f);
        //    if (EffectsObj != null)
        //    {
        //        Destroy(EffectsObj);
        //    }
        //}

    }
}