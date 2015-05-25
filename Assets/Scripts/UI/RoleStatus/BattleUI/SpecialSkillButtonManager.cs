using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Battle
{
    public class SpecialSkillButtonManager : View
    {
        public GameObject BattleButtonPrefab;
        //public Vector3[] JoyStickBtnPosition;

        
        private struct SpecialSkill
        {
            public BattleSkillButton SkillButton;
            public int SkillEnable;
            public int SkillID;
			public int SkillEnergyComsume;
        }

        private SpecialSkill[] m_spcialSkill;

        private List<int> m_guideIDList = new List<int>();
        
        public Vector3 m_leftBtnPos = new Vector3(80, 55, 0);
        public int m_leftSpacing = 120;

        public Vector3 JoystickLeftPos;
        public Vector3 JoystickRightPos;

		public Vector3[] JoystickSpecialBtnPos = new Vector3[3];

        void Awake()
        {
            RegisterEventHandler();

            if (GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"InitializeEctype Is Null");
                GameDataManager.Instance.dataEvent.RegisterEvent(DataType.InitializeEctype, Init);
            }
            else
            {
                Init(null);
            }
        }

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.U))
			{
				ButtonDown(0);
			}
			if(Input.GetKeyDown(KeyCode.M))
			{
				ButtonDown(1);
			}
			if(Input.GetKeyDown(KeyCode.P))
			{
				ButtonDown(2);
			}

		}
		private void ButtonDown(int index)
		{
			if(m_spcialSkill[index].SkillButton.gameObject.activeInHierarchy)
			{
				m_spcialSkill[index].SkillButton.ButtonClick(null);
			}
		}

        void Init(object obj)
        {
            GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.InitializeEctype, Init);
            SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            int[] skillEnable = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId].PowerSkillHide;
            m_spcialSkill = new SpecialSkill[skillEnable.Length];
            for (int i = 0; i < m_spcialSkill.Length; i++)
            {
                m_spcialSkill[i].SkillEnable = skillEnable[i];
            }
            SetSpecialSkillData();
			StartCoroutine("InitLater");
            UpdateSpecialStatus(null);
        }

		IEnumerator InitLater()
		{
			yield return new WaitForSeconds(1f);
			InitSpecialSkillBtn(null);
		}

        protected override void RegisterEventHandler()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.AddColdWork, AddColdWork);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.RemoveColdWork, RemoveColdWork);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeSirenSkillPropUpdate, SirenSkillPropUpdateHandle);
			AddEventHandler(EventTypeEnum.UpdateRollAirSlot.ToString(), UpdateSpecialStatus);
        }

        //void Start()
        //{
        //}

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.AddColdWork, AddColdWork);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RemoveColdWork, RemoveColdWork);
			//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RemoveColdWork, RemoveColdWork);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeSirenSkillPropUpdate, SirenSkillPropUpdateHandle);
            RemoveEventHandler(EventTypeEnum.UpdateRollAirSlot.ToString(), UpdateSpecialStatus);
            for (int i = 0; i < m_guideIDList.Count; i++)
            {
               GuideBtnManager.Instance.DelGuideButton(m_guideIDList[i]);
            }
            m_guideIDList.Clear();
        }

        //奥义、爆气等...
        public void InitSpecialSkillBtn(object obj)
        {
            TraceUtil.Log("InitSpecialSkillBtn");
            //m_spcialSkill = new BattleSkillButton[3];
			if(PlayerManager.Instance.FindHero()==null)
			{
				return;
			}

            BattleSkillButtonDelegate battleSkillButtonCallBack = PlayerManager.Instance.FindHero().GetComponent<PlayerBehaviour>().OnButtonCallBack;
            m_guideIDList.Clear();

            bool isJoyStick = GameManager.Instance.UseJoyStick;
            for (int i = 0; i < m_spcialSkill.Length; i++)
            {
                if (m_spcialSkill[i].SkillEnable == 0)
                {
                    GameObject creatBtn;
//                    if (i < 2)
//                    {
//                        creatBtn = CreatObjectToNGUI.InstantiateObj(BattleButtonPrefab, BattleUIManager.Instance.BottomLeft);
//                        if (!isJoyStick)
//                        { creatBtn.transform.localPosition = new Vector3(m_leftBtnPos.x + m_leftSpacing * i, m_leftBtnPos.y, 0); }
//                        else
//                        {
//                            if (i == 0)
//                            {
//                                creatBtn.transform.parent = BattleUIManager.Instance.BottomRight;
//                                creatBtn.transform.localPosition = JoystickRightPos;
//                            }
//                            else
//                            {
//                                creatBtn.transform.parent = BattleUIManager.Instance.BottomLeft;
//                                creatBtn.transform.localPosition = JoystickLeftPos;
//                            }
//                        }
//                    }
//                    else //人物角色头像上的奥义按钮
//                    {
//                        creatBtn = BattleUIManager.Instance.RoleStatuUI.ButtonSkillObj;
//                        creatBtn.gameObject.SetActive(true);
//                    }
					creatBtn = CreatObjectToNGUI.InstantiateObj(BattleButtonPrefab, BattleUIManager.Instance.BottomRight);
					if(isJoyStick)
					{
						switch(i)
						{
						case 0:
							creatBtn.transform.localPosition = JoystickSpecialBtnPos[0];
							break;
						case 1:
							creatBtn.transform.localPosition = JoystickSpecialBtnPos[1];
							break;
						case 2:
							creatBtn.transform.localPosition = JoystickSpecialBtnPos[2];
							break;
						}

					}
					else
					{
						creatBtn.transform.localPosition = new Vector3(m_leftBtnPos.x + m_leftSpacing * i, m_leftBtnPos.y, 0);
					}


                    m_spcialSkill[i].SkillButton = creatBtn.GetComponent<BattleSkillButton>();
                    int guideId = 0;
                    GuideBtnManager.Instance.RegGuideButton(creatBtn, MainUI.UIType.Empty, SubType.EctypeSpecialSkill, out guideId);
                    m_guideIDList.Add(guideId);

                    //BattleButtons[i].SetButtonAttribute(null, null);
					var skillConfigData = SkillDataManager.Instance.GetSkillConfigData(m_spcialSkill[i].SkillID);
					if(skillConfigData!=null)
					{
						m_spcialSkill[i].SkillButton.SetButtonAttribute(skillConfigData, battleSkillButtonCallBack, (SpecialSkillType)i + 1, 4, isJoyStick);
						m_spcialSkill[i].SkillEnergyComsume = skillConfigData.energy_comsumeParam;

						//如果等级不够开启
						if(PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL
						   < skillConfigData.m_unlockLevel)
						{
							creatBtn.SetActive(false);
						}
					}
					else
					{
						creatBtn.SetActive(false);
					}

                    //加入按钮记忆管理 0-翻滚 1-爆气  2-奥义 【参照 SetSpecialSkillData 方法】
                    switch (i)
                    {
                        case 0:
                            BattleUIManager.Instance.RememberRegiste(creatBtn, SkillBtnRemember.RememberBtnType.ScrollBtn);
                            break;
                        case 1:
                            BattleUIManager.Instance.RememberRegiste(creatBtn, SkillBtnRemember.RememberBtnType.ExplosiveBtn);
                            break;
                        case 2:
							m_spcialSkill[i].SkillButton.SirenSkillInterface.SetActive(true);
							SirenSkillPropUpdateHandle(null);
                            BattleUIManager.Instance.RememberRegiste(creatBtn, SkillBtnRemember.RememberBtnType.LoreBtn);
                            break;
                    }
				}
            }
            UpdateSpecialStatus(null);
            //如果有新手引导，并且步骤尚未开始，先屏蔽技能按钮
            //if (GameManager.Instance.IsNewbieGuide && !NewbieGuideManager_V2.Instance.EctypeGuideStepReached)
            //{
            //    foreach (var child in m_spcialSkill)
            //    {
            //        if (child.SkillButton != null)
            //        {
            //            child.SkillButton.SetButtonStatus(false);
            //        }
            //    }
            //}
			GetColdWork();
        }

        /// <summary>
        /// 设置特殊技能数据 滚动、爆气、奥义
        /// </summary>
        void SetSpecialSkillData()
        {
            int vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            switch (vocation)
            {
                case 1:
                    m_spcialSkill[0].SkillID = CommonDefineManager.Instance.CommonDefine.THRESH_SKILLID1;
                    m_spcialSkill[1].SkillID = CommonDefineManager.Instance.CommonDefine.BUFF_SKILLID1;
//                    if (PlayerManager.Instance.m_heroUpanishads != null)
//                        m_spcialSkill[2].SkillID = PlayerManager.Instance.m_heroUpanishads.UpanishadId;
//                    else
//                        m_spcialSkill[2].SkillID = CommonDefineManager.Instance.CommonDefine.FATAL_SKILLID1;
                    break;
                case 4:
                        m_spcialSkill[0].SkillID = CommonDefineManager.Instance.CommonDefine.THRESH_SKILLID4;
                        m_spcialSkill[1].SkillID = CommonDefineManager.Instance.CommonDefine.BUFF_SKILLID4;
//                    if (PlayerManager.Instance.m_heroUpanishads != null)
//                        m_spcialSkill[2].SkillID = PlayerManager.Instance.m_heroUpanishads.UpanishadId;
//                    else
//                        m_spcialSkill[2].SkillID = CommonDefineManager.Instance.CommonDefine.FATAL_SKILLID4;
                    break;
                default:
                    break;
            }
			//妖女奥义技能
			SYaoNvContext yaoNvContext = new SYaoNvContext();
			yaoNvContext = SirenManager.Instance.GetYaoNvList().FirstOrDefault(p=>p.byAssembly == 1);

			PlayerSirenConfigData sirenConfig = null;
			sirenConfig = PlayerDataManager.Instance.GetPlayerSirenList().SingleOrDefault(p=>p._sirenID == yaoNvContext.byYaoNvID);

			if(sirenConfig != null)
			{
				var sirenData = sirenConfig._sirenConfigDataList.SingleOrDefault(p=>yaoNvContext.byLevel == p._growthLevels);
				var skillID = sirenData._SirenSkillIDs.SingleOrDefault(p=>p.Vocation == vocation);
				m_spcialSkill[2].SkillID = skillID.SkillID;
			}
			else
			{
				switch(vocation)
				{
				case 1:
					m_spcialSkill[2].SkillID = CommonDefineManager.Instance.CommonDefine.DefaultSirenSkill1;
					break;
				case 4:
					m_spcialSkill[2].SkillID = CommonDefineManager.Instance.CommonDefine.DefaultSirenSkill4;
					break;
				}
			}
        }

		void SirenSkillPropUpdateHandle(object obj)
		{
			TraceUtil.Log(SystemModel.Lee, TraceLevel.Verbose,EctypeManager.Instance.GetCurrentEctypeData().SirenSkillVaule.ToString()+
			              "  ,  "+EctypeManager.Instance.GetEctypeProps().dwYaoNvSkillTimes.ToString());
			int time = EctypeManager.Instance.GetCurrentEctypeData().SirenSkillVaule - EctypeManager.Instance.GetEctypeProps().dwYaoNvSkillTimes;
			m_spcialSkill[2].SkillButton.Label_SirenSkillValue.text = time.ToString();

			if(time <= 0)
			{
				m_spcialSkill[2].SkillButton.SetButtonStatus( SkillButtonStatus.Disable);
			}
		}

        void RemoveColdWork(object obj)
        {
            SMsgActionColdWork_SC sMsgActionColdWork_SC = (SMsgActionColdWork_SC)obj;
            ColdWorkInfo coldWorkItem = new ColdWorkInfo(PlayerManager.Instance.FindHeroDataModel().UID,
                           sMsgActionColdWork_SC.byClassID, sMsgActionColdWork_SC.dwColdID, sMsgActionColdWork_SC.dwColdTime);
            AddColdWork(coldWorkItem);
        }

        void GetColdWork()
        {
            long targetUID = PlayerManager.Instance.FindHeroDataModel().UID;
            foreach (SpecialSkill childBtnInfo in m_spcialSkill)
            {
                if (childBtnInfo.SkillButton != null && childBtnInfo.SkillButton.skillConfigData != null)
                {
                    ColdWorkInfo myColdWork = ColdWorkManager.Instance.GetColdWorkInfo(targetUID, ColdWorkClass.Skill, (uint)childBtnInfo.SkillButton.skillConfigData.m_skillId);
                    if (myColdWork != null && myColdWork.lMasterID == targetUID)
                    {
                        int MpTake = GetSkillMpTake(childBtnInfo.SkillButton.skillConfigData);
                        int myMp = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP;
                        float coldTime = myColdWork.ColdTimeEnd - Time.realtimeSinceStartup; ;
                       // TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"冷却技能：" + LanguageTextManager.GetString(childBtn.skillConfigData.m_name) + ",能否冷却：" + (myMp > MpTake)+",结束时间：" +myColdWork.ColdTimeEnd+ ",时间："+myColdWork.ColdTime+"," + coldTime);
                        if (MpTake < myMp && coldTime > 0)
                        {
                            childBtnInfo.SkillButton.RecoverMyself((int)coldTime);
                            childBtnInfo.SkillButton.SetButtonStatus(SkillButtonStatus.Recovering);
                        }
                    }
                }
            }
        }

        void AddColdWork(object obj)
        {
            ColdWorkInfo myColdWork = (ColdWorkInfo)obj;
            //Debug.LogWarning("技能按钮收到冷却事件消息：" + myColdWork.ColdClass + "," + myColdWork.ColdID + "," + myColdWork.ColdTime);
            long targetUID = PlayerManager.Instance.FindHeroDataModel().UID;
			if ((myColdWork.ColdClass != ColdWorkClass.Skill && myColdWork.ColdClass != ColdWorkClass.ECold_ClassID_SpecialSkill)  || myColdWork.lMasterID != targetUID)
                return;
            foreach (SpecialSkill childBtnInfo in m_spcialSkill)
            {
                if (childBtnInfo.SkillButton != null && childBtnInfo.SkillButton.skillConfigData != null && childBtnInfo.SkillButton.skillConfigData.m_skillId == (int)myColdWork.ColdID)
                {
                    childBtnInfo.SkillButton.SetButtonStatus(SkillButtonStatus.Recovering);
                    childBtnInfo.SkillButton.RecoverMyself((int)myColdWork.ColdTime / 1000);
                }
            }
        }

        int GetSkillMpTake(SkillConfigData SkillData)
        {
            return -1;
        }

        /// <summary>
        /// 刷新按钮气力值
        /// </summary>
        /// <param name="inotifyArgs"></param>
        void UpdateSpecialStatus(INotifyArgs inotifyArgs)
        {
            if (m_spcialSkill == null)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"m_spcialSkill Is Null");
                return;
            }
			bool flag = false;
            TraceUtil.Log("UpdateSpecialButtonStatus");
            foreach (SpecialSkill childBtn in m_spcialSkill)
            {
                if (childBtn.SkillButton != null && childBtn.SkillButton.skillConfigData != null)
                {
                    switch (childBtn.SkillButton.SpecialType)
                    {
                        case SpecialSkillType.Roll:
                            //bool flag = PlayerGasSlotManager.Instance.GetAirSlotValue < 1 ? false : true;
							flag = PlayerGasSlotManager.Instance.GetAirSlotValue < childBtn.SkillEnergyComsume ? false : true;
							childBtn.SkillButton.SetButtonMPStatus(flag);
							childBtn.SkillButton.skillEnergyComsume = childBtn.SkillEnergyComsume;
                            TraceUtil.Log("SetSkillBtnStatus:"+flag);
                            childBtn.SkillButton.PlayEffect(flag);
                            break;
                        case SpecialSkillType.Explode:
                            //flag = PlayerGasSlotManager.Instance.GetAirSlotValue < 2 ? false : true;
							flag = PlayerGasSlotManager.Instance.GetAirSlotValue < childBtn.SkillEnergyComsume ? false : true;
                            childBtn.SkillButton.SetButtonMPStatus(flag);
							childBtn.SkillButton.skillEnergyComsume = childBtn.SkillEnergyComsume;
                            childBtn.SkillButton.PlayEffect(flag);
                            TraceUtil.Log("SetSkillBtnStatus:"+flag);
                            break;
                        case SpecialSkillType.Meaning:
							//flag = PlayerGasSlotManager.Instance.GetAirSlotValue < 3  ? false : true;
							flag = PlayerGasSlotManager.Instance.GetAirSlotValue < childBtn.SkillEnergyComsume ? false : true;
							childBtn.SkillButton.skillEnergyComsume = childBtn.SkillEnergyComsume;
						if(flag)
						{
							int a = 0 ;
						}
							if(EctypeManager.Instance.GetSirenSkillSurplusValue() <= 0)
							{
								childBtn.SkillButton.SetButtonMPStatus(false);
							}
							else
							{
								childBtn.SkillButton.SetButtonMPStatus(flag);
								childBtn.SkillButton.PlayEffect(flag);
							}
                            
                            TraceUtil.Log("SetSkillBtnStatus:"+flag);
                            break;
                        default:
                            break;
                    }

                }
            }
        }



    }
}
