using UnityEngine;
using System.Collections;
using UI.MainUI;

/// <summary>
/// BattleUI Scene JoyStickUIManager 
/// </summary>
namespace UI.Battle
{

    public class JoystickUIManager : MonoBehaviour
    {
        public UIFilledSprite RecoveSprite;
        public SingleButtonCallBack AttackBtn;
        public JoyStick JoyStickBtn;
		public GameObject NormalAttackTouchFeedbackEff;
		public GameObject RecoveFinishEff;
        bool IsJoystick;
        int[] m_guideBtnID;

		private GameObject m_mormalAttackTouchFeedbackEff;

		const float REPEAT_TIME = 0.2f;
		private bool m_isPressing = false;

		void Awake()
        {
            IsJoystick = GameManager.Instance.UseJoyStick;
            AttackBtn.gameObject.SetActive(IsJoystick);
            JoyStickBtn.gameObject.SetActive(IsJoystick);
            AttackBtn.SetPressCallBack(OnAtkBtnPressed);
            if (IsJoystick)
            {
                BattleUIManager.Instance.RememberRegiste(AttackBtn.gameObject, SkillBtnRemember.RememberBtnType.NormalSkillBtn);
            }
            RecoveSprite.fillAmount = 1f;
        }
        void Start()
        {
            //注册左右两个按钮到BtnManager
            RegBtn(JoyStickBtn.gameObject, AttackBtn.gameObject);

            //Add to BattleSkillButtonManager to receive server ColdDown message
            var kind = PlayerManager.Instance.FindHeroDataModel().GetPlayerKind();
            int skillId = PlayerDataManager.Instance.GetBattleItemData(kind).GetPlayerNormalSkillId(0, false);  //普攻突进

			NormalAttakBtn normalAttakBtn = new NormalAttakBtn() { SkillId = skillId, RecoveSprite = RecoveSprite,RecoveFinishEff=RecoveFinishEff };

            BattleSkillButtonManager.Instance.AddNormalBtn(normalAttakBtn);
        }
		void Update()
		{
			if(Input.GetKeyDown( KeyCode.J))
			{
				OnAtkBtnPressed(true);
			}
			if(Input.GetKeyUp(KeyCode.J))
			{
				OnAtkBtnPressed(false);
			}
		}
        /// <summary>
        /// 注册摇杆和普通攻击
        /// </summary>
        /// <param name="joyBtn">摇杆</param>
        /// <param name="attackBtn">普通攻击</param>
        public void RegBtn(GameObject joyBtn, GameObject attackBtn)
        {
            m_guideBtnID = new int[2] ;
            GuideBtnManager.Instance.RegGuideButton(joyBtn,UIType.Empty,SubType.JoySticker,out  m_guideBtnID[0]);  //TODO 81
            GuideBtnManager.Instance.RegGuideButton(attackBtn, UIType.Empty, SubType.JoySticker, out m_guideBtnID[1]); //TODO 81
        }
        public void UnRegBtn()
        {
            GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[0]);  //摇杆 //TODO 81
            GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[1]);  //普通攻击 //TODO 81
        }
        void OnAtkBtnPressed(bool isPressed)
        {
			m_isPressing = isPressed;
      	
			if(isPressed)
			{
                float repeatTime = CommonDefineManager.Instance.CommonDefine.AttackButtonDelay;
				RepeatAttack();
                InvokeRepeating("RepeatAttack",repeatTime,repeatTime);
			}
			else
			{
				CancelInvoke("RepeatAttack");
			}
        }

		void RepeatAttack()
		{
			if (m_isPressing)
			{
				if(m_mormalAttackTouchFeedbackEff==null)
				{
					m_mormalAttackTouchFeedbackEff=NGUITools.AddChild(AttackBtn.gameObject,NormalAttackTouchFeedbackEff);
					m_mormalAttackTouchFeedbackEff.transform.localPosition=new Vector3(0,0,-100);
				}
				else
				{
					m_mormalAttackTouchFeedbackEff.SetActive(false);
					m_mormalAttackTouchFeedbackEff.SetActive(true);
				}
				
				UIEventManager.Instance.TriggerUIEvent(UIEventType.AddAPMNumber,null);
				UIEventManager.Instance.TriggerUIEvent(UIEventType.AddBattleButtonClickNumber, UI.Battle.SpecialSkillType.NormalBtn);
			}
			if (!PlayerManager.Instance.OnNormalSkillButtonPress (m_isPressing)) {
				//如果施放不成功，进入记忆
				var clickBtn = AttackBtn.GetComponent<SkillBtnRemember> ();
				if (clickBtn != null) {
					clickBtn.OnSkillBtnClicked (false);
				}
			}
		}

        void OnDoubleClick()
        { 
        }
        void OnDestroy()
        {
            UnRegBtn();
        }
    }
    /// <summary>
    /// 普通攻击突进，注入BattleSkillButtonManager里面进行管理的结构
    /// </summary>
    public class NormalAttakBtn
    {
        public UIFilledSprite RecoveSprite;
		public GameObject RecoveFinishEff;
        public int SkillId { get; set; }
        private float m_recoverFloat = 0;
        private float m_barFromValue = 0;
        private float m_barToValue = 1;
        private GameObject m_recoverFloatObj;
        private BarCutDownMode m_barCutDownMode = BarCutDownMode.MinToMax;
		private GameObject m_recoveFinishEff;
        public void SetButtonStatus(SkillButtonStatus skillButtonStatus)
        {
            switch (skillButtonStatus)
            {
                case SkillButtonStatus.Recovering:
                    if (RecoveSprite != null)
                        RecoveSprite.fillAmount = m_barCutDownMode == Battle.BarCutDownMode.MaxToMin ? 1 : 0;
                    break;
                default:
                    break;
            }
        }
         public void RecoverMyself(float recoverTime)
        {
            if (this.m_recoverFloatObj != null) { GameObject.Destroy(this.m_recoverFloatObj); }
            RecoveSprite.fillAmount = 0;
            m_recoverFloat = 0;
            if (recoverTime == 0) { return; }
            int fromValue = m_barCutDownMode == Battle.BarCutDownMode.MaxToMin ? 1 : 0;
            int toValue = m_barCutDownMode == Battle.BarCutDownMode.MaxToMin ? 0 : 1;
            //Debug.LogWarning(string.Format("冷却技能：{0}=>{1}", fromValue, toValue));
            //if (EffectsObj != null) { Destroy(EffectsObj); }
            this.m_recoverFloatObj = TweenFloat.Begin(recoverTime, fromValue, toValue, SetRecoverFloat);
        }
         private void SetRecoverFloat(float number)
         {
             this.m_recoverFloat = number;            
             RecoveSprite.fillAmount = number * (m_barToValue - m_barFromValue) + m_barFromValue;
             //TraceUtil.Log("RecoverFloat:" + RecoveSprite.fillAmount);
             if (number == 1) 
			{ 
				//if(m_recoveFinishEff==null)
				{
					m_recoveFinishEff=NGUITools.AddChild(RecoveSprite.transform.parent.gameObject,RecoveFinishEff);
					m_recoveFinishEff.transform.localPosition=new Vector3(0,0,-100);
					m_recoveFinishEff.AddComponent<DestroySelf>();
				}
				//else
				//{
				//	m_recoveFinishEff.SetActive(false);
				//	m_recoveFinishEff.SetActive(true);
				//}
			}
         }
    }
}