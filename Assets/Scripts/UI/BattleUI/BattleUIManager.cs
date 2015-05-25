using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BattleUI Scene BattleUIManager 
/// </summary>
namespace UI.Battle
{
    public enum ScreenPositionType {Top,Left,Right,Center,TopRight,BottonRight,TopLeft,BottomLeft }
    public class BattleUIManager : View
    {
        private bool m_heroCreated = false;
        private static BattleUIManager m_instance;
        public static BattleUIManager Instance { get { return m_instance; } }

        public GameObject DoubleHitTipsPrefab;
        public GameObject HeroleStatusPrefab;
		public GameObject BattleMessageManagerPrefab;
        public GameObject RememberBtnEff;   //按钮记忆动画
		public GameObject RememberBtnEffNormal;   //普攻按钮记忆动画
        //玩家身上的正前方方向指示
        public GameObject HeroDirectEffect;
        private Transform m_PlayerDirect;  
        private bool m_dynamicPoint;  // 是否玩家身上动态指向
        private Transform m_hero;  /// 主玩家
        private Transform HeroTrans
        {
            get
            {
                var hero=PlayerManager.Instance.FindHero();
                if (m_hero == null && hero!=null)
                {
                    m_hero = hero.transform;
                }
                return m_hero;
            }
        }
        public SingleButtonCallBack SysSettingButton;

        public Transform BottomLeft, TopLeft, BottomRight, TopRight, Center,Left,Top;

        public GameObject SirenDialogPrefab;
        
        private DoubleHitUI doubleHitUI;
        public HeroStatusUI_V2 RoleStatuUI { get; private set; }

		public GameObject StoreCoverUI;//剧情黑边
		private StoreCoverUI m_StoreCoverUI;

        private int m_guideBtnID = 0;

        void Awake()
        {
            m_instance = this;
            RegisterEventHandler();
            LoadUI();

            //加载玩家正前方指示箭头 ,同时监听引导方向箭头消息，确保与引导方向箭头不冲突
            ShowPlayerDirectArrow();
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
            var peekData = GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            SMSGEctypeInitialize_SC ectypeSmg = (SMSGEctypeInitialize_SC)peekData;

            if (ectypeSmg.dwEctypeContainerId == CommonDefineManager.Instance.CommonDefine.TUTORIAL_ECTYPE_ID)
            {
                SysSettingButton.gameObject.SetActive(false);
            }
            GuideBtnManager.Instance.RegGuideButton(this.SysSettingButton.gameObject, MainUI.UIType.Empty, SubType.EctypeSysSetting, out m_guideBtnID);
        }
        void Update()
        {
            if (m_dynamicPoint)
            {
                if (HeroTrans != null && m_PlayerDirect != null)
                {
                    m_PlayerDirect.rotation = HeroTrans.rotation;
                }
                else
                {
                    m_dynamicPoint = false;
                }
            }
        }
        private void ShowPlayerDirectArrow()
        {
            if (HeroTrans != null)
            {
                m_PlayerDirect = HeroTrans.FindChild(HeroDirectEffect.name);
                if (m_PlayerDirect == null)
                {
                    var guidePointTargetEffet = Instantiate(HeroDirectEffect) as GameObject;
                    guidePointTargetEffet.name = HeroDirectEffect.name;
                    m_PlayerDirect = guidePointTargetEffet.transform;
                    m_PlayerDirect.parent = HeroTrans;
                    m_PlayerDirect.localPosition = Vector3.zero;

                    m_dynamicPoint = true;
                }                
            }
        }
        private void HidePlayerDirectArrow(INotifyArgs args)
        {
            //玩家脚底指引箭头
            if (m_PlayerDirect != null)
            {
                GameObject.Destroy(m_PlayerDirect.gameObject);
            }
            m_dynamicPoint = false;
        }
        private void ShowPlayerDirectArrow(INotifyArgs args)
        {
            ShowPlayerDirectArrow();
        }        
        protected override void RegisterEventHandler()
        {
			//AddEventHandler(EventTypeEnum.PlayerGotoSceneReady.ToString(), SceneChangeHandle);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.LoadBattleSceneCompleteCS, SendLoadingCompleteStatusToSever);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.LoadBattleSceneCompleteSC,StartGame);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.SirenSkillFire, HideInterface);
            AddEventHandler(EventTypeEnum.PlayerGotoSceneReady.ToString(),SceneceReady);
            //AddEventHandler(EventTypeEnum.ShowPlayerEctypeGuideArrow.ToString(), HidePlayerDirectArrow);  //策划修改，在引导箭头出现时，原指示箭头不消失。【保留代码】
            //AddEventHandler(EventTypeEnum.HidePlayerEctypeGuideArrow.ToString(), ShowPlayerDirectArrow);
        }

        protected override void OnDestroy()
        {
            HidePlayerDirectArrow(null);

            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.LoadBattleSceneCompleteCS, SendLoadingCompleteStatusToSever);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.LoadBattleSceneCompleteSC, StartGame);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.SirenSkillFire, HideInterface);
            RemoveEventHandler(EventTypeEnum.PlayerGotoSceneReady.ToString(), SceneceReady);
            //RemoveEventHandler(EventTypeEnum.ShowPlayerEctypeGuideArrow.ToString(), HidePlayerDirectArrow);
            //RemoveEventHandler(EventTypeEnum.HidePlayerEctypeGuideArrow.ToString(), ShowPlayerDirectArrow);
			//RemoveEventHandler(EventTypeEnum.PlayerGotoSceneReady.ToString(), SceneChangeHandle);
            m_instance = null;

            RememberUnRegiste();
			GameManager.Instance.isTeamBattleMark = false;
            GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        void SceneceReady(INotifyArgs iNotifyArgs)
        {
            LoadUI();
			ShowPlayerDirectArrow();
        }
        private void LoadUI()
        {
            if (!m_heroCreated)
            {
                if (PlayerManager.Instance.HeroCreated)
                {
                    m_heroCreated = true;

                    SysSettingButton.SetCallBackFuntion(ShowBackToTownBox);
                    //ShowSkillButtonUI();
                    //ShowMagicAndHealthButtonUI();
                    ShowDoubleHitUI();
                    ShowBattleRoleStatuUI();
                    //ShowSpecialSkillUI();
					if(m_StoreCoverUI== null)
					{	
						GameObject storeCoverUI = UI.CreatObjectToNGUI.InstantiateObj(StoreCoverUI,Center);
						m_StoreCoverUI = storeCoverUI.GetComponent<StoreCoverUI>();
					}
					UI.CreatObjectToNGUI.InstantiateObj(BattleMessageManagerPrefab,Center);
                }
            }
			/*if (GameManager.Instance.isTeamBattleMark) {
				UIEventManager.Instance.TriggerUIEvent (UIEventType.LoadingStartDownTime, null);
			}*/
			//LoadingUI.Instance.StartShowDownTime ();
        }
        public void SendLoadingCompleteStatusToSever(object obj)
        {
			//Debug.Log ("SendLoadingCompleteStatusToSever==========");
            LoadingUI.Instance.Show();
           // NetServiceManager.Instance.EctypeService.SendTeamateRequestEnterEctype();
            //MessageBox.Instance.Show(3, "", "SendTeamateRequestToServer","Yes",null);
        }

        public void StartGame(object obj)
        {
		//	Debug.Log ("StartGame==========");
            //MessageBox.Instance.Show(3, "", "ReceiveTeamateRequestFromServer", "Yes", null);
			//Debug.Log("IsTeamExist()=="+TeamManager.Instance.IsTeamExist()+" isTeamBattleMark="+GameManager.Instance.isTeamBattleMark);
			if (TeamManager.Instance.IsTeamExist())//GameManager.Instance.isTeamBattleMark) 
			{
				UIEventManager.Instance.TriggerUIEvent (UIEventType.TeamComplete, null);		
			}
            LoadingUI.Instance.Close();
        }


        public Transform GetScreenTransform(ScreenPositionType screenPositionType)
        {
            switch (screenPositionType)
            {
                case ScreenPositionType.BottomLeft:
                    return BottomLeft;
                case ScreenPositionType.BottonRight:
                    return BottomRight;
                case ScreenPositionType.Center:
                    return Center;
                case ScreenPositionType.Left:
                    return Left;
                case ScreenPositionType.Right:
                    break;
                case ScreenPositionType.Top:
                    return Top;
                case ScreenPositionType.TopLeft:
                    return TopLeft;
                case ScreenPositionType.TopRight:
                    return TopRight;
                default:
                    break;
            }
            return null;
        }

		/// <summary>
		/// 出现剧情黑边
		/// </summary>
		/// <param name="isFlag">If set to <c>true</c> is flag.</param>
		public void ShowStoryCover(bool isFlag)
		{
			float posZ = isFlag? 1000 : 0;
			TopLeft.transform.localPosition = new Vector3(TopLeft.transform.localPosition.x,TopLeft.transform.localPosition.y,posZ);
			TopRight.transform.localPosition = new Vector3(TopRight.transform.localPosition.x,TopRight.transform.localPosition.y,posZ);
			BottomRight.transform.localPosition = new Vector3(BottomRight.transform.localPosition.x,BottomRight.transform.localPosition.y,posZ);
			BottomLeft.transform.localPosition = new Vector3(BottomLeft.transform.localPosition.x,BottomLeft.transform.localPosition.y,posZ);
			m_StoreCoverUI.Appear(isFlag);
		}

        public void ShowBackToTownBox(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            /* 新UI修改后去掉判断
            if (BattleSettingPanel == null)
            {
                BattleSettingPanel = CreatObjectToNGUI.InstantiateObj(BattleSettingPrefab,Center).GetComponent<BattleSettingPanel>();
            }
            BattleSettingPanel.ShowPanel();             
             */
            if (BattleSettlementManager.Instance().IsGameSettlement||GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
                return;
            SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            EctypeContainerData ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
            if (ectypeData.MapType == 5)
            {
                MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_490"), LanguageTextManager.GetString("IDS_H2_14"), LanguageTextManager.GetString("IDS_H2_9"), null, BackToTown);
            }
            else
            {
                MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_71"), LanguageTextManager.GetString("IDS_H2_14"), LanguageTextManager.GetString("IDS_H2_9"), null, BackToTown);
            }
        }
        void BackToTown()
        {
			//发送战斗数据统计
			RaiseEvent(EventTypeEnum.EctypeBattleStatistics.ToString(), null);
            
			//TraceUtil.Log("返回城镇");
            long UIDEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
            NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(UIDEntity);
        }
        
        public void ShowBattleRoleStatuUI()
        {
            #region add by lee
            if (PVPBattleManager.Instance.IsPVPBattle)
            {
                return;
            }
            #endregion
            if (RoleStatuUI == null)
            {
                RoleStatuUI = CreatObjectToNGUI.InstantiateObj(HeroleStatusPrefab, TopLeft).GetComponent<HeroStatusUI_V2>();
                RoleStatuUI.Show();
            }

            //判断当前是否妖女副本 //\暂时去掉
            //var peekData = GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            //if (peekData == null)
            //{
            //    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"DataType.InitializeEctype is null");
            //    return;
            //}
            //SMSGEctypeInitialize_SC ectypeSmg = (SMSGEctypeInitialize_SC)peekData;
            //EctypeContainerData ectypeData;
            //if (EctypeConfigManager.Instance.EctypeContainerConfigList.ContainsKey((int)ectypeSmg.dwEctypeContainerId))
            //{
            //    ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[(int)ectypeSmg.dwEctypeContainerId];
            //    if (ectypeData.MapType == 3)//妖女副本
            //    {
            //        StartCoroutine(LateShowSirenDialog(ectypeData));
            //    }
            //}            
        }

        IEnumerator LateShowSirenDialog(EctypeContainerData ectypeData)
        {
            yield return new WaitForSeconds(2f);
            ShowSirenDialog(ectypeData);
        }

        public void ShowDoubleHitUI()
        {
            if (doubleHitUI == null)
            {
                doubleHitUI = CreatObjectToNGUI.InstantiateObj(DoubleHitTipsPrefab, BottomRight).GetComponent<DoubleHitUI>();
            }
        }

        //public void SetMyButtonsColliderActive(bool Flag)//暂不使用
        //{
        //    battleSkillButtonManager.SetMyButtonsColliderActive(Flag);
        //    battleMagicAndHealthButtons.SetMyButtonsColliderActive(Flag);
        //}

        /// <summary>
        /// 显示妖女对白
        /// </summary>
        private void ShowSirenDialog(EctypeContainerData ectypeData)
        {
            //TraceUtil.Log("[ectypeData]" + ectypeData.lEctypeContainerID);
            GameObject IconPrefab = CreatObjectToNGUI.InstantiateObj(SirenDialogPrefab, transform);
            IconPrefab.transform.localPosition = new Vector3(0, 0, 10);//避免和任务指引面板重叠
            SirenDialogEctypeBehaviour sirenDialogEctypeBehaviour = IconPrefab.GetComponent<SirenDialogEctypeBehaviour>();

            var resData = EctypeResDataManager.Instance.GetEctypeContainerResData(ectypeData.lEctypeContainerID);
            if (resData != null)
            {
                //sirenDialogEctypeBehaviour.Init(resData.bossAppearancePortrait, LanguageTextManager.GetString(ectypeData.bossAppearanceWord));
            }            
            SoundManager.Instance.PlaySoundEffect(ectypeData.bossAppearanceSound);
            IconPrefab.AddComponent<DestroySelf>();                   
        }

        void HideInterface(object obj)
        {
            bool flag = (bool)obj;
            int posZ = flag == true ? -2000 : 0;
            BottomLeft.localPosition = new Vector3(BottomLeft.localPosition.x, BottomLeft.localPosition.y, posZ);
            BottomRight.localPosition = new Vector3(BottomRight.localPosition.x, BottomRight.localPosition.y, posZ);
            TopLeft.localPosition = new Vector3(TopLeft.localPosition.x, TopLeft.localPosition.y, posZ);
        }

        #region 20141008 Skill button pressed remember
        List<SkillBtnRemember> m_skillBtnRemembers = new List<SkillBtnRemember>();
        SkillBtnRemember RememberBtn = null;
        public void RememberRegiste(GameObject skillBtn, SkillBtnRemember.RememberBtnType rememberBtnType)
        {
            var skillBtnRemember = skillBtn.GetComponent<SkillBtnRemember>();
            if (skillBtnRemember == null)
            {
                skillBtnRemember = skillBtn.AddComponent<SkillBtnRemember>();
            }
            skillBtnRemember.BtnRememberType = rememberBtnType;
            skillBtnRemember.OnSkillBtnClicked = (flag) =>
                {
                    StopCoroutine("BtnRemberTimeDown");
                    if (RememberBtn != null)
                    {
                        RememberBtn.ShowRememberEff(false);
                        RememberBtn.BtnMemTime = 0;
                        RememberBtn = null;
                    }
                    //技能施放失败，进入记忆状态并开始倒计时
					//Debug.Log(skillBtnRemember.name+" Fire:"+flag);
                    if (!flag)
                    {
                        RememberBtn = skillBtn.GetComponent<SkillBtnRemember>();
                        RememberBtn.ResetBtnMemTime();
						RememberBtn.ShowRememberEff(true);
                        StartCoroutine("BtnRemberTimeDown");
                    }
                };
            m_skillBtnRemembers.Add(skillBtnRemember);
        }
        IEnumerator BtnRemberTimeDown()
        {
            while (true)
            {

                if (RememberBtn != null)
                {
                    if (RememberBtn.BtnMemTime > 0)
                    {
                        RememberBtn.BtnMemTime -= Time.deltaTime;
                        switch (RememberBtn.BtnRememberType)
                        {
                            case SkillBtnRemember.RememberBtnType.NormalSkillBtn:
							if(PlayerManager.Instance.OnRememberNormalPress())
								{
									StopCoroutine("BtnRemberTimeDown");
									RememberBtn.ShowRememberEff(false);
									RememberBtn.BtnMemTime = 0;
									RememberBtn = null;
                                    PlayerManager.Instance.NormalAttackRemembering = false;
								}	
                                break;
                            case SkillBtnRemember.RememberBtnType.ScrollBtn:
                            case SkillBtnRemember.RememberBtnType.ExplosiveBtn:
                            case SkillBtnRemember.RememberBtnType.LoreBtn:
                            case SkillBtnRemember.RememberBtnType.SkillBtn:
                                if (RememberBtn.GetComponent<BattleSkillButton>().ButtonClickWithRet(null))
                                {
                                    StopCoroutine("BtnRemberTimeDown");
                                    RememberBtn.ShowRememberEff(false);
                                    RememberBtn.BtnMemTime = 0;
                                    RememberBtn = null;
                                }
							//else
							//{
								//Debug.Log(RememberBtn.name+" Fire:"+false);
							//}
                                break;
                        }
                    }
                    else
                    {
                        StopCoroutine("BtnRemberTimeDown");
                        RememberBtn.ShowRememberEff(false);
                        RememberBtn.BtnMemTime = 0;
                        RememberBtn = null;
                    }
                }
                else
                {
                    StopCoroutine("BtnRemberTimeDown");
                }
                yield return null;
            }
        }
        private void RememberUnRegiste()
        {
            m_skillBtnRemembers.ApplyAllItem(P => P.OnSkillBtnClicked = null);
            m_skillBtnRemembers.Clear();
        }
        public SkillBtnRemember FindRememberBtn(SkillBtnRemember.RememberBtnType rememberBtnType)
        {
            return m_skillBtnRemembers.FirstOrDefault(P => P.BtnRememberType == rememberBtnType);
        }
        #endregion
    }
}