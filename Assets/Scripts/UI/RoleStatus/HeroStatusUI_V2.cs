using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Battle
{
    public class HeroStatusUI_V2 : View
    {

        public UISprite MainPlayerIcon;
        public UISprite GasSlotProgressBar;
		public UISprite UI_ManaProgress;
		public UISprite UI_ManaShadowProgress;
		public UISprite UI_BloodProgress;
		public UISprite UI_BloodShadowProgress;
        public GameObject TeammateUIPrefab;
        public SpriteSmoothFlag ManaBarBackground;
        public GameObject GasSlotFullEffect;
		public Transform GasSlotFullEffectPos;
        public HeroRollStrengthTrough RollStrengthTrough;

        public bool ShowBlood = true;
        public GameObject BloodEffecet;
        public Transform BloodEffectPos;
        GameObject BloodEffectIstance;

        public UILabel HPLabel;
        public UILabel MPLabel;

		public UILabel Label_SirenSkillValue;
		public UILabel Label_HeroLevel;

        bool IsShowProgressBarLabel = false;

        private GameObject m_TweenHP;
        private GameObject m_TweenMP;
        private GameObject m_airSlotFullEffect;
        
        private float m_curBloodValue = 0;      //当前
        private float m_curManaValue = 0;
        private float m_curGasValue = -1;

        private Vector3 m_HPScale;
        private Vector3 m_MPScale;

        private int m_hpShort;
        private int m_spShort;

        SMsgPropCreateEntity_SC_MainPlayer_PlayerValue m_playerValue;
        SMsgPropCreateEntity_SC_Player_UnitValue m_unitValue;
        private List<TeammateStatus_V2> m_teamMemberList = new List<TeammateStatus_V2>();

		private int m_sirenSkillValue = 0;

        void Awake()
        {
			IsShowProgressBarLabel = GameManager.Instance.IsShowBloodLabel;
            this.RegisterEventHandler();

            m_hpShort = CommonDefineManager.Instance.CommonDefine.HP_SHORT;
            m_spShort = CommonDefineManager.Instance.CommonDefine.SP_SHORT;

//            if (BloodShadowProgressBar.foreground != null)
//            {
//                m_HPScale = BloodShadowProgressBar.foreground.transform.localScale;
//            }
//            if (ManaShadowProgressBar.foreground != null)
//            {
//                m_MPScale = ManaShadowProgressBar.foreground.transform.localScale;
//            }


        }

        void Start()
        {
            GasSlotProgressBar.fillAmount = 0f;
            InitHeroStatusUI();
            InitTeamMembers(null);
			//UpdateSirenSkill();
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStatus);  //角色受击更新
            AddEventHandler(EventTypeEnum.EntityRelive.ToString(), SetHeroAlive);  //角色复活
            AddEventHandler(EventTypeEnum.NoEnoughRollAir.ToString(), NoEnoughRollStrengthHandle);
            AddEventHandler(EventTypeEnum.UpdateRollAirSlot.ToString(), UpdateRollStrengthHandle);
            AddEventHandler(EventTypeEnum.EctypeNoQualification.ToString(), TeamMemberQuitMessage);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.FlagMPBar, FlagMPProgressBar);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ReasetTeammateStatus, UpdateTeamMember);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.TeamMemberLeave, InitTeamMembers);
			//UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeSirenSkillPropUpdate, SirenSkillPropUpdateHandle);
        }

        public void Show()
        {
            transform.localPosition = new Vector3(0,0,100);
        }

        public void InitHeroStatusUI()
        {
            UpdateHeroStatus();
            UpdateHeroIcon();
			Label_HeroLevel.text = PlayerManager.Instance.FindHeroDataModel().GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL.ToString();
        }

        void ResetStatus(INotifyArgs inotifyArgs)
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                UpdateHeroStatus();

                int explode = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BREAKOUT_FLAG;

                if (explode == 0)
                {
                    ((PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour).IsExplodeState = false;
                    //PlayerManager.Instance.FindHeroEntityModel().GO.GetComponent<PlayerHurtFlash>().OnBurst(false);  //关闭玩家爆气效果
                }
                else
                {
                    ((PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour).IsExplodeState = true;
                    //PlayerManager.Instance.FindHeroEntityModel().GO.GetComponent<PlayerHurtFlash>().OnBurst(true);  //打开玩家爆气效果
                }
            }
        }

        void UpdateHeroStatus()//此处设置主角显示的所有状态
        {
            m_playerValue = PlayerManager.Instance.FindHeroDataModel().PlayerValues;
            m_unitValue = PlayerManager.Instance.FindHeroDataModel().UnitValues;

            //float MaxHp = m_unitValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP;
            //if (MaxHp > m_hpShort)
            //{
            //    BloodProgressBar.transform.localScale = Vector3.one;
            //}
            //else
            //{
            //    BloodProgressBar.transform.localScale = Vector3.one;
            //}
            //float MaxMP = m_unitValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXMP;
            //if (MaxMP > m_spShort)
            //    ManaProgressBar.transform.localScale = Vector3.one;
            //else
            //    ManaProgressBar.transform.localScale = Vector3.one;
            int newBlood = m_unitValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP;
            if (m_curBloodValue != newBlood)
            {
                m_TweenHP = TweenFloat.Begin(1, m_curBloodValue, newBlood, ChangeHp);
				float MaxHP = m_unitValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP;
				//BloodProgressBar.sliderValue = newBlood / MaxHP;
				UI_BloodProgress.fillAmount = newBlood/MaxHP;
                m_curBloodValue = newBlood;
            }

            int newMp =  m_unitValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP;
            if (m_curManaValue != newMp)
            {
                //Debug.LogWarning("角色耗蓝量刷新："+m_curManaValue+"=>"+newMp);
                m_TweenMP = TweenFloat.Begin(1, m_curManaValue, newMp, ChangeMP);
				float MaxMP = m_unitValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXMP;
				//ManaProgressBar.sliderValue = newMp / MaxMP;
				UI_ManaProgress.fillAmount = newMp/MaxMP;
                m_curManaValue = newMp;
            }

			/* 去掉能量值
            float gasValue = (float)m_playerValue.PLAYER_FIELD_ENERGY_NUM / (float)m_playerValue.PLAYER_FIELD_MAX_ENERGY_NUM;

            if (m_curGasValue != gasValue)
            {
                float beginFloat = 0.32f;
                float endFloat = 0.85f;
                gasValue = gasValue * (endFloat - beginFloat) + beginFloat;  //转换成UI数据

                CancelInvoke("AirSlotAnim");
                if (gasValue <= beginFloat)
                {
                    if (m_airSlotFullEffect != null)
                    {
                        DestroyImmediate(m_airSlotFullEffect);
                    }
                    InvokeRepeating("AirSlotAnim", 0f, 0.1f);
                }
                else
                {
                    TweenFloat.Begin(0.3f, GasSlotProgressBar.fillAmount, gasValue, ChangeGas);
                }
                if (gasValue >= endFloat)
                {
                    if (m_airSlotFullEffect == null)
                    {
						m_airSlotFullEffect = CreatObjectToNGUI.InstantiateObj(GasSlotFullEffect, GasSlotFullEffectPos);
                        //m_airSlotFullEffect.transform.localPosition = new Vector3(75, -145, -30);
                    }
                }
                m_curGasValue = gasValue;
            }
            */
			PlayerGasSlotManager.Instance.UpdateRollStrength(m_playerValue.PLAYER_FIELD_ENERGY_NUM);
        }

        void AirSlotAnim()
        {
            if (GasSlotProgressBar.fillAmount >= 0.32f)
            {
                GasSlotProgressBar.fillAmount -= CommonDefineManager.Instance.CommonDefine.POWER_REDUCETIME * 0.001f;
            }
        }

        void SetHeroAlive(INotifyArgs args)
        {
            SMsgActionRelivePlayer_SC sMsgActionRelivePlayer_SC = (SMsgActionRelivePlayer_SC)args;
            if (sMsgActionRelivePlayer_SC.actorTarget == PlayerManager.Instance.FindHeroDataModel().ActorID)
            {
                //MainPlayerIcon.ChangeSprite(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
                UpdateHeroIcon();
                UpdateHeroStatus();
            }
        }

        void UpdateHeroIcon()
        {
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            int vocationID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            int fashionID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
            var resData = CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_Battle.FirstOrDefault(P => P.VocationID == vocationID && P.FashionID == fashionID);
            if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + fashionID); }
            MainPlayerIcon.spriteName = resData.ResName;
        }

        void ChangeHp(float Number)
        {
            if (Number < 0)
                return;
            float MaxHp = m_unitValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP;
            //TraceUtil.Log("CurrentHP:"+m_unitValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP+ ",MaxHP:"+MaxHp);
            //this.BloodShadowProgressBar.sliderValue = Number / MaxHp;
			UI_BloodShadowProgress.fillAmount = Number/MaxHp;
            if (IsShowProgressBarLabel)
            {
                HPLabel.SetText(string.Format("{0}/{1}",Number,MaxHp));
            }
            else
            {
                HPLabel.SetText("");
            }
			if (ShowBlood && UI_BloodShadowProgress.fillAmount < 0.3f)
            {
                ShowBlood = false;
                DestroyBloodEffect(null);
                BloodEffectIstance = CreatObjectToNGUI.InstantiateObj(BloodEffecet, BloodEffectPos);
                //DoForTime.DoFunForTime(3,DestroyBloodEffect,null);
            }
			else if (UI_BloodShadowProgress.fillAmount > 0.3f)
            {
                if (!ShowBlood)
                {
                    ShowBlood = true;
                }
                else
                {
                    DestroyBloodEffect(null);
                }
            }
        }

        void DestroyBloodEffect(object obj)
        {
            if (BloodEffectIstance != null) { Destroy(BloodEffectIstance); }
        }

        void ChangeMP(float Number)
        {
            if (Number < 0)
                return;


            float MaxMP = m_unitValue.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXMP;
            //this.ManaShadowProgressBar.sliderValue = Number / MaxMP;
			UI_ManaShadowProgress.fillAmount = Number/MaxMP;

			if (IsShowProgressBarLabel)
            {
                MPLabel.SetText(string.Format("{0}/{1}",Number,MaxMP));
            }
            else
            {
                MPLabel.SetText("");
            }
        }

        void ChangeGas(float value)
        {
            GasSlotProgressBar.fillAmount = value;
        }

        void UpdateRollStrengthHandle(INotifyArgs args)
        {
            SUpdateRollStrengthStruct updateStruct = (SUpdateRollStrengthStruct)args;
            RollStrengthTrough.SetValue(updateStruct.strengthValue);
        }

        void NoEnoughRollStrengthHandle(INotifyArgs args)
        {
            RollStrengthTrough.ShowNoEnoughStrengthTip();
        }

        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStatus);
            RemoveEventHandler(EventTypeEnum.EntityRelive.ToString(), SetHeroAlive);
            RemoveEventHandler(EventTypeEnum.UpdateRollAirSlot.ToString(), UpdateRollStrengthHandle);
            RemoveEventHandler(EventTypeEnum.NoEnoughRollAir.ToString(), NoEnoughRollStrengthHandle);
            RemoveEventHandler(EventTypeEnum.EctypeNoQualification.ToString(), TeamMemberQuitMessage);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.FlagMPBar, FlagMPProgressBar);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReasetTeammateStatus, UpdateTeamMember);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TeamMemberLeave, InitTeamMembers);
			//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeSirenSkillPropUpdate, SirenSkillPropUpdateHandle);
        }

        public void FlagMPProgressBar(object obj)
        {
            ManaBarBackground.BeginFlag(3, 0.5f, Color.white, Color.red, null);
        }

        public void Close()
        {
            transform.localPosition = new Vector3(0, 0, -800);
        }

        /// <summary>
        /// 初始化组队成员
        /// </summary>
        /// <param name="obj"></param>
        private Vector3 vFirstTeammatePos = new Vector3(300, -27, 0);
        private const float fTeammateSpacing = 76;

		void UpdateSirenSkill()
		{
			int time = EctypeManager.Instance.GetCurrentEctypeData().SirenSkillVaule - EctypeManager.Instance.GetEctypeProps().dwYaoNvSkillTimes;
			//Label_SirenSkillValue.text = time.ToString();

		}

		void SirenSkillPropUpdateHandle(object obj)
		{
			UpdateSirenSkill();
		}

        void InitTeamMembers(object obj)
        {
            if (!TeamManager.Instance.IsTeamExist())
                return;

            SMsgTeamPropMember_SC[] SMsgTeamPropMembers = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers;
            List<SMsgTeamPropMember_SC> NewSMsgTeamPropMembers = new List<SMsgTeamPropMember_SC>();
            foreach (SMsgTeamPropMember_SC child in SMsgTeamPropMembers)
            {
                //if (child.TeamMemberContext.uidEntity != PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
                //{
                //    NewSMsgTeamPropMembers.Add(child);
                //}
                if (child.TeamMemberContext.dwActorID != PlayerManager.Instance.FindHeroDataModel().ActorID)
                {
                    NewSMsgTeamPropMembers.Add(child);
                }
            }

            for (int i = 0; i < m_teamMemberList.Count; i++)
            {
                Destroy(m_teamMemberList[i].gameObject);
            }
            if (m_teamMemberList.Count > 0)
                m_teamMemberList.Clear();

            for (int i = 0; i < NewSMsgTeamPropMembers.Count; i++)
            {
                GameObject creatObj = CreatObjectToNGUI.InstantiateObj(TeammateUIPrefab, transform);
                creatObj.transform.localPosition = new Vector3(vFirstTeammatePos.x + fTeammateSpacing * i, vFirstTeammatePos.y, vFirstTeammatePos.z);
                var teammateScripts = creatObj.GetComponent<TeammateStatus_V2>();
                int vocation = NewSMsgTeamPropMembers[i].TeamMemberContext.byKind;
                int fashion = NewSMsgTeamPropMembers[i].TeamMemberContext.nFashionID;
                uint actorID = NewSMsgTeamPropMembers[i].TeamMemberContext.dwActorID;
                //long uid = NewSMsgTeamPropMembers[i].TeamMemberContext.uidEntity;
                //bool isDead = NewSMsgTeamPropMembers[i].TeamMemberContext.nCurHP <= 0;
				bool isDead = NewSMsgTeamPropMembers[i].TeamMemberContext.dwState == (int)STeamMemberContext.CRT_STATE.enCrt_State_Die;
                teammateScripts.InitMemberIcon(vocation,fashion, actorID, isDead);
                m_teamMemberList.Add(teammateScripts);
            }
        }

        void UpdateTeamMember(object obj)//更新单个组队成员属性
        {
            uint dwActorID = (uint)obj;
            SMsgTeamPropMember_SC[] SMsgTeamPropMembers = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers;

            foreach (SMsgTeamPropMember_SC child in SMsgTeamPropMembers)
            {
                if (child.TeamMemberContext.dwActorID == dwActorID)
                {
                    foreach (TeammateStatus_V2 UIChild in m_teamMemberList)
                    {
                        if (UIChild.ActorID == dwActorID)
                        {
                            int vocation = child.TeamMemberContext.byKind;
                            int fashion = child.TeamMemberContext.nFashionID;
                            uint actorID = child.TeamMemberContext.dwActorID;
                            //long uid = child.TeamMemberContext.uidEntity;
                            //bool isDead = child.TeamMemberContext.nCurHP <= 0;
							bool isDead = child.TeamMemberContext.dwState == (int)STeamMemberContext.CRT_STATE.enCrt_State_Die;
                            UIChild.InitMemberIcon(vocation,fashion, actorID, isDead);
                            return;
                        }
                    }
                }
            }
        }

        void TeamMemberQuitMessage(INotifyArgs inotifyArgs)
        {
            var ReceiveMsg = (SMSGEctypeResult_SC)inotifyArgs;
        }

    }
}