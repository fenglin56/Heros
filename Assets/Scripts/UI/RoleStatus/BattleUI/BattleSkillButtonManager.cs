using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Battle
{

    /// <summary>
    /// new
    /// </summary>
    public class BattleSkillButtonManager : View
    {
        private static BattleSkillButtonManager m_instance;
        public static BattleSkillButtonManager Instance { get { return m_instance; } }
        public const int EmptyColdWork = 9999999;

        public GameObject BattleButtonPrefab;
		public GameObject BattleMultiSegmentsSkillEff;
        private BattleSkillButton[] BattleButtons = new BattleSkillButton[0];
		private ComboBattleButton[] ComboBattleButton;
        //private SkillConfigData[] ButtonInfo;
        public Vector3[] JoyStickBtnPosition;

        public Vector3 m_rightBtnPos = new Vector3(-100, 85, 0);
        public float m_rightSpacing = 130;

        bool Isjoystick = true;

        void Awake()
        {
            Isjoystick = GameManager.Instance.UseJoyStick;
            m_instance = this;
            RegisterEventHandler(); 
        }

        void Start()
        {
            ShowButtons();
            ResetButtonStatus();
            GetColdWork();
        }

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.I))
			{
				ButtonDown(0);
			}
			if(Input.GetKeyDown(KeyCode.K))
			{
				ButtonDown(1);
			}
			if(Input.GetKeyDown(KeyCode.O))
			{
				ButtonDown(2);
			}
			if(Input.GetKeyDown(KeyCode.L))
			{
				ButtonDown(3);
			}
		}

		private void ButtonDown(int index)
		{
			if(BattleButtons.Length >= index)
			{
				BattleButtons[index].ButtonClick(null);
			}
		}

        protected override void RegisterEventHandler()
        {
            //AddEventHandler(EventTypeEnum.ColdWork.ToString(), ColdSkillButton);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.AddColdWork, AddColdWork);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.RemoveColdWork, RemoveColdWork);
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        }

        void OnDestroy()
        {
            m_instance = null;
            //RemoveEventHandler(EventTypeEnum.ColdWork.ToString(), ColdSkillButton);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.AddColdWork, AddColdWork);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RemoveColdWork, RemoveColdWork);
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);

            for (int i = 0; i < m_guideIDList.Count; i++)
            {
                GuideBtnManager.Instance.DelGuideButton(m_guideIDList[i]);//TODO 81
            }
            m_guideIDList.Clear();
        }

        public void UpdateViaNotify(INotifyArgs inotifyArgs)//设置各种属性
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                ResetButtonStatus();
            }
        }

        void ResetButtonStatus()
        {
            if (BattleButtons == null||BattleButtons.Length <= 0)
                return;

            foreach (BattleSkillButton childBtn in BattleButtons)
            {
                if (childBtn != null && childBtn.skillConfigData != null)
                {
                    var SkillData = childBtn.skillConfigData;
                    int Mp = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP;
                    int MaxMp = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXMP;
                    int MpTake = GetSkillMpTake(SkillData);
                    if (MpTake == EmptyColdWork) { return; }
                    //TraceUtil.Log("检测技能：" + LanguageTextManager.GetString(SkillData.m_name) + ",技能消耗:" + MpTake + ",  Current:" + Mp  +"  Max:"+MaxMp+"，是否足够：" + (Mp >= MpTake).ToString());                    
					if(SkillData.m_IsSirenSkill)
					{
						//childBtn.SetButtonMPStatus( Mp >= MpTake && EctypeManager.Instance.GetSirenSkillSurplusValue()>0);
						childBtn.SetButtonStatus(EctypeManager.Instance.GetSirenSkillSurplusValue()>0);
					}
					else
					{
						childBtn.SetButtonMPStatus(Mp >= MpTake);
					}
                }
            }
        }

     
        /// <summary>
        /// 返回999999为角色消息为空
        /// </summary>
        /// <param name="SkillData"></param>
        /// <returns></returns>
        int GetSkillMpTake(SkillConfigData SkillData)
        {
            if (PlayerManager.Instance.HeroSMsgSkillInit_SC.sInfos == null)
            { return EmptyColdWork; }
            float[] ParamList = SkillData.m_manaConsumeParams;
            var getSkill = PlayerManager.Instance.HeroSMsgSkillInit_SC.sInfos.FirstOrDefault(P => P.wSkillID == SkillData.m_skillId);
            if(getSkill .wSkillLV == 0)
            {
                //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"人物已经装备技能上无法找到对应SkillID:"+SkillData.m_skillId);
            }
            int roleLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            int MpTake = (int)(Mathf.FloorToInt((ParamList[0] * Mathf.Pow(roleLevel, 2) + ParamList[1] * roleLevel + ParamList[2]) / ParamList[3]) * ParamList[3]);
            //Debug.LogWarning("技能耗蓝："+LanguageTextManager.GetString(SkillData.m_name)+","+MpTake);
            return MpTake;
        }

        private List<int> m_guideIDList = new List<int>();

        public void ShowButtons()
        {
//            var hero = PlayerManager.Instance.FindHero();
//            if (hero != null)
//            {                
                List<SkillConfigData> M_Skills = new List<SkillConfigData>();
                var skillEquipList = PlayerManager.Instance.HeroSMsgSkillInit_SC.wSkillEquipList;
                PlayerManager.Instance.HeroSMsgSkillInit_SC.wSkillEquipList.ApplyAllItem(P => TraceUtil.Log("加载人物技能：" + P));

				ComboBattleButton=new ComboBattleButton[skillEquipList.Length];
                for (int i = 0; i < skillEquipList.Length; i++)
                {
                    var skillData = SkillDataManager.Instance.GetSkillConfigData(skillEquipList[i]);
                    if (skillData != null && !M_Skills.Contains(skillData))
                    {
                        M_Skills.Add(skillData);

						//InitComboSkill
						ComboBattleButton[i]=new UI.Battle.ComboBattleButton(skillEquipList[i]);
                    }
                }
                BattleButtons = new BattleSkillButton[M_Skills.Count];
                m_guideIDList.Clear();
                for (int i = 0; i < BattleButtons.Length; i++)
                {
                    GameObject creatBtn = CreatObjectToNGUI.InstantiateObj(BattleButtonPrefab, BattleUIManager.Instance.BottomRight);
                    //creatBtn.transform.localPosition = new Vector3(m_rightBtnPos.x, m_rightBtnPos.y + m_rightSpacing * i, 0);
                    SetSkillBtnPosition(creatBtn, i);
                    BattleButtons[i] = creatBtn.GetComponent<BattleSkillButton>();
                    int guideId = 0;

                    GuideBtnManager.Instance.RegGuideButton(creatBtn, MainUI.UIType.Empty, SubType.EctypeSkill, out guideId);
                    //如果有新手引导，并且步骤尚未开始，先屏蔽技能按钮
                    //if (GameManager.Instance.IsNewbieGuide && !NewbieGuideManager_V2.Instance.EctypeGuideStepReached)
                    //{
                    //    BattleButtons[i].SetButtonStatus(false);
                    //}
                    m_guideIDList.Add(guideId);

					SetButtonAttribute(i);

                    BattleUIManager.Instance.RememberRegiste(creatBtn, SkillBtnRemember.RememberBtnType.SkillBtn);
                }

                //for (int i = 0; i < BattleButtons.Length; i++)
                //{
                //    GameObject creatBtn = CreatObjectToNGUI.InstantiateObj(BattleButtonPrefab,transform);
                //    creatBtn.transform.localPosition = new Vector3(m_rightBtnPos.x, m_rightBtnPos.y + m_rightSpacing * (3 - i), 0);
                //    BattleButtons[i] = creatBtn.GetComponent<BattleSkillButton>();
                //    BattleButtons[i].SetButtonAttribute(SkillDataManager.Instance.GetSkillConfigData(PlayerManager.Instance.HeroSMsgSkillInit_SC.wSkillEquipList[i]), battleSkillButtonCallBack,SpecialSkillType.Normal);
                //}
//            }
//            else
//            {
//                TraceUtil.Log(SystemModel.Rocky,"BattleSkillButtonManager 172 line No found hero");
//            }
        }
		//ComboSkill Handle
		public void SetButtonAttribute(int skillIndex)
		{
			var skillConfigData = ComboBattleButton [skillIndex].GetSkillConfigData ();
			if (ComboBattleButton [skillIndex].CurrentIndex != 1) {
								//多段技能特效
								BattleButtons [skillIndex].ShowOrHideMultiSegmentsSkillEff (true);
								
						} else {
								BattleButtons [skillIndex].ShowOrHideMultiSegmentsSkillEff (false);
						}
			//Debug.Log("ComboBattleButton [skillIndex].CurrentIndex:"+ComboBattleButton [skillIndex].CurrentIndex);
			BattleButtons[skillIndex].SetButtonAttribute(skillConfigData, this.BattleSkillButtonCallBack, SpecialSkillType.Normal, skillIndex, Isjoystick);
		}
		public bool BattleSkillButtonCallBack(BattleSkillButton ButtonInstance)
		{
			string coroutineMethodName = "MultiSegmentComboTime"+ButtonInstance.MyBtnIndex;
			StopCoroutine (coroutineMethodName);
			bool flag=PlayerManager.Instance.FindHero ().GetComponent<PlayerBehaviour> ().OnButtonCallBack (ButtonInstance);
			//Debug.Log("Fire at:"+Time.realtimeSinceStartup+"  "+ flag);
			if(flag)
			{
				SetButtonAttribute (ButtonInstance.MyBtnIndex);
				//CD
				GetColdWork (ButtonInstance);
				if (ComboBattleButton [ButtonInstance.MyBtnIndex].CurrentIndex != 1) {
					StartCoroutine (coroutineMethodName);
				}
			}
			return flag;
		}
		IEnumerator MultiSegmentComboTime0()
		{
			yield return new WaitForSeconds (CommonDefineManager.Instance.CommonDefine.SkillComboTime / 1000f);
			ComboBattleButton [0].CurrentIndex = 0;
			SetButtonAttribute (0);
		}
		IEnumerator MultiSegmentComboTime1()
		{
			yield return new WaitForSeconds (CommonDefineManager.Instance.CommonDefine.SkillComboTime / 1000f);
			ComboBattleButton [1].CurrentIndex = 0;
			SetButtonAttribute (1);
		}
		IEnumerator MultiSegmentComboTime2()
		{
			yield return new WaitForSeconds (CommonDefineManager.Instance.CommonDefine.SkillComboTime / 1000f);
			ComboBattleButton [2].CurrentIndex = 0;
			SetButtonAttribute (2);
		}
		IEnumerator MultiSegmentComboTime3()
		{
			yield return new WaitForSeconds (CommonDefineManager.Instance.CommonDefine.SkillComboTime / 1000f);
			ComboBattleButton [3].CurrentIndex = 0;
			SetButtonAttribute (3);
		}
        void SetSkillBtnPosition(GameObject skillBtnObj, int BtnID)
        {
            if (Isjoystick)
            {
                skillBtnObj.transform.localPosition = JoyStickBtnPosition[BtnID];
            }
            else
            {
                skillBtnObj.transform.localPosition = new Vector3(m_rightBtnPos.x, m_rightBtnPos.y + m_rightSpacing * BtnID, 0);
            }
        }

        void GetColdWork()
        {
            long targetUID = PlayerManager.Instance.FindHeroDataModel().UID;
            foreach (BattleSkillButton childBtn in BattleButtons)
            {
                if (childBtn != null && childBtn.skillConfigData != null)
                {
                    ColdWorkInfo myColdWork = ColdWorkManager.Instance.GetColdWorkInfo(targetUID, ColdWorkClass.Skill, (uint)childBtn.skillConfigData.m_skillId);
                    if (myColdWork != null&&myColdWork.lMasterID == targetUID)
                    {
                        int MpTake = GetSkillMpTake(childBtn.skillConfigData);
                        if (MpTake == EmptyColdWork) { return; }
                        //int myMp = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP;
                        float coldTime = myColdWork.ColdTimeEnd - Time.realtimeSinceStartup;
						//TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"冷却技能：" + LanguageTextManager.GetString(childBtn.skillConfigData.m_name) + ",能否冷却：" + (myMp > MpTake)+",结束时间：" +myColdWork.ColdTimeEnd+ ",时间："+myColdWork.ColdTime+"," + coldTime);
                        //if (MpTake < myMp && coldTime > 0)
                        //{
                            if (coldTime != 0)
                            {
                                childBtn.SetButtonStatus(SkillButtonStatus.Recovering);
                            }
							childBtn.RecoverMyself((int)coldTime);
                        //}
                    }
                }
            }
        }
		void GetColdWork(BattleSkillButton skillButton)
		{
			long targetUID = PlayerManager.Instance.FindHeroDataModel().UID;
			ColdWorkInfo myColdWork = ColdWorkManager.Instance.GetColdWorkInfo(targetUID, ColdWorkClass.Skill, (uint)skillButton.skillConfigData.m_skillId);
			if (myColdWork != null&&myColdWork.lMasterID == targetUID)
			{
				int MpTake = GetSkillMpTake(skillButton.skillConfigData);
				if (MpTake == EmptyColdWork) { return; }
				//int myMp = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP;
				float coldTime = myColdWork.ColdTimeEnd - Time.realtimeSinceStartup;
				//TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"冷却技能：" + LanguageTextManager.GetString(childBtn.skillConfigData.m_name) + ",能否冷却：" + (myMp > MpTake)+",结束时间：" +myColdWork.ColdTimeEnd+ ",时间："+myColdWork.ColdTime+"," + coldTime);
				//if (MpTake < myMp && coldTime > 0)
				//{
				if (coldTime != 0)
				{
					skillButton.SetButtonStatus(SkillButtonStatus.Recovering);
				}
				skillButton.RecoverMyself((int)coldTime);
				//}
			}
		}

        void RemoveColdWork(object obj)
        {
            SMsgActionColdWork_SC sMsgActionColdWork_SC = (SMsgActionColdWork_SC)obj;
            ColdWorkInfo coldWorkItem = new ColdWorkInfo(PlayerManager.Instance.FindHeroDataModel().UID,
                           sMsgActionColdWork_SC.byClassID, sMsgActionColdWork_SC.dwColdID, sMsgActionColdWork_SC.dwColdTime);
            AddColdWork(coldWorkItem);
        }
        /// <summary>
        /// 加入普通突进攻击按钮，用于处理普攻CD
        /// </summary>
        public void AddNormalBtn(NormalAttakBtn normalAttakBtn)
        {
            m_normalAttakBtn = normalAttakBtn;
        }
        private NormalAttakBtn m_normalAttakBtn;
        void AddColdWork(object obj)
        {
            ColdWorkInfo myColdWork = (ColdWorkInfo)obj;
            long targetUID = PlayerManager.Instance.FindHeroDataModel().UID;
            if (myColdWork.ColdClass != ColdWorkClass.Skill || myColdWork.lMasterID != targetUID)
                return;
            //普攻突进CD
            if (m_normalAttakBtn != null && m_normalAttakBtn.SkillId == (int)myColdWork.ColdID)
            {
                if (myColdWork.ColdTime != 0)
                {
                    m_normalAttakBtn.SetButtonStatus(SkillButtonStatus.Recovering);

					m_normalAttakBtn.RecoverMyself((int)myColdWork.ColdTime / 1000f);
                }
                

                return;
            }
            //Debug.LogWarning("技能按钮收到冷却事件消息：" + myColdWork.ColdClass + "," + myColdWork.ColdID + "," + myColdWork.ColdTime);
            foreach (BattleSkillButton childBtn in BattleButtons)
            {
                if (childBtn != null && childBtn.skillConfigData != null && childBtn.skillConfigData.m_skillId == (int)myColdWork.ColdID)
                {
                    int MpTake = GetSkillMpTake(childBtn.skillConfigData);
                    if (MpTake == EmptyColdWork) { return; }
                    int myMp = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP;
                    //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"冷却技能：" + LanguageTextManager.GetString(childBtn.skillConfigData.m_name) + ",能否冷却：" + (myMp > MpTake));
                    //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"冷却技能：" + LanguageTextManager.GetString(childBtn.skillConfigData.m_name));
                    //if (MpTake < myMp)
                    //{
                    if (myColdWork.ColdTime != 0)
                    {
                        childBtn.SetButtonStatus(SkillButtonStatus.Recovering);
                    }
					childBtn.RecoverMyself((int)myColdWork.ColdTime / 1000);
                    //}
                }
            }
        }

        public void SetButtonStatus(int SkillID, SkillButtonStatus skillButtonStatus)
        {
            //TraceUtil.Log("外部设置技能按钮状态："+skillButtonStatus.ToString());
            foreach (BattleSkillButton child in BattleButtons)
            {
                if ( child != null && child.skillConfigData != null && child.skillConfigData.m_skillId == SkillID)
                {
                    child.SetButtonStatus(skillButtonStatus);
                }
            }
        }
        /// <summary>
        /// 设置所有等待状态的按钮为激活状态
        /// </summary>
        public void SetWaitBtnsEnable(BattleSkillButton battleSkillButton)
        {
            for (int i = 0; i < BattleButtons.Length; i++)
            {
                if(BattleButtons[i] != null)
                    BattleButtons[i].SetWaitButtonEnable(battleSkillButton);
            }
                //BattleButtons.ApplyAllItem(P=>P.SetWaitButtonEnable(battleSkillButton));
        }

        public void SetMyButtonsColliderActive(bool flag)
        {
            foreach (BattleSkillButton child in BattleButtons)
            {
                if (child != null)
                {
                    child.Active = flag;
                }
            }
        }

    }

	public class ComboBattleButton
	{
		public int CurrentIndex;
		public SkillConfigData[] ComboSkillConfigDatas;

		public ComboBattleButton(int skillId)
		{
			CurrentIndex = 0;
			var skillData = SkillDataManager.Instance.GetSkillConfigData (skillId);
			if (skillData.ComboSkill != null && skillData.ComboSkill.Length > 0) {
								ComboSkillConfigDatas = new SkillConfigData[skillData.ComboSkill.Length + 1];
								ComboSkillConfigDatas [0] = skillData;
								for (int k=0; k<skillData.ComboSkill.Length; k++) {
										ComboSkillConfigDatas [k+1] = SkillDataManager.Instance.GetSkillConfigData (skillData.ComboSkill [k]);
								}
						}
			else {
				ComboSkillConfigDatas = new SkillConfigData[1];
				ComboSkillConfigDatas [0] = skillData;
						}
		}
//		public SkillConfigData GetSkillConfigData(int segmentIndex)
//		{
//			if (segmentIndex >= ComboSkillConfigDatas.Length) {
//								CurrentIndex = 0;
//						} else {
//							CurrentIndex = segmentIndex;
//						}
//			CurrentIndex++;
//			return ComboSkillConfigDatas [CurrentIndex - 1];
//		}
		public SkillConfigData GetSkillConfigData()
		{
			if (CurrentIndex >= ComboSkillConfigDatas.Length) {
				CurrentIndex=0;
						}
			CurrentIndex++;
			return ComboSkillConfigDatas [CurrentIndex - 1];
		}
	}
}