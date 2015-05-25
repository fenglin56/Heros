using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;
using System.Linq;
using UI.Friend;

namespace UI.Team
{
    /// <summary>
    /// 队伍房间面板
    /// </summary>
    public class TeamRoomPanel : MonoBehaviour
    {
        //public GameObject RoomUICamera;

        public TeamRoomMemberItem ATeamRoomMemberItem;          //队员实例
        public Transform[] MemberTrans = new Transform[3];      //队员位置
        public Transform[] WaitTabs = new Transform[3];

        public GameObject TeamCamaraRoot;
        private GameObject m_TeamMemberViewCamera;
        private ContainerHeroView[] m_MemberHeroViews;

        //public ContainerHeroView FirstHeroView;
        //public ContainerHeroView SecondHeroView;
        //public ContainerHeroView ThirdHeroView;

        public Transform CaptainInterface;                      //队长界面
        public Transform MemberInterface;                       //队员界面

        public TeamInvitePanel TeamInvitePanel;                 //好友邀请界面

        //public VigourBarManager VigourMessageManager;           //充值体力面板
        //public LocalButtonCallBack AddEnergyButton;
        //public EctypePanelMessageBox EctypepanelMessageBox;     //封魔副本开始战斗信息面板
        public GameObject Background_Recruit;
        public LocalButtonCallBack Button_Recruit;

        public LocalButtonCallBack CaptainDisbandButton;
        public LocalButtonCallBack CaptainSelectButton;
        public LocalButtonCallBack CaptainStartButton;
        public LocalButtonCallBack PosOneInviteButton;
        public LocalButtonCallBack PosTwoInviteButton;
        public LocalButtonCallBack PosThreeInviteButton;
        public LocalButtonCallBack PosOneKickButton;
        public LocalButtonCallBack PosTwoKickButton;
        public LocalButtonCallBack PosThreeKickButton;
        //public LocalButtonCallBack[] InviteButtons;
        //public LocalButtonCallBack[] KickButtons;

        public LocalButtonCallBack MemberLeaveButton;
        public LocalButtonCallBack MemberReadyButton;

        //private readonly string GREEN_BTN_SPR_NAME = "JH_UI_Button_0012_01";
        //private readonly string RED_BTN_SPR_NAME = "JH_UI_Button_0011_01";
        //private readonly string INVITE_TXT = "邀请队员";
        //private readonly string KICK_TXT = "请离队员";

        public UILabel AreaTitleLabel;
        public UILabel ExplanationLabel;
        public UILabel TitleLabel;

        //public UILabel ActiveLifeLabel;
        //public UISlider ActiveLifeSlider;
        //private string ActiveLifeFTx;
        public UILabel ExpLabel;
        private string ExpFTx;
        public UILabel CopperLabel;
        private string CopperFTx;


        public SpriteSwith CostTypeSpriteSwith;
        public UILabel ActiveCostLabel;
        private string ActiveCostFTx;
        public UILabel TimeLabel;
        //private string TimeFTx;
        public UILabel ComboLabel;
        //private string ComboFTx;

        public GameObject CostLabelPreafab;
        private bool IsShowCostLabel = false;

        private List<TeamRoomMemberItem> mTeamMemberList = new List<TeamRoomMemberItem>();
        //private MemberReadyProp m_MemberReadyProp = new MemberReadyProp();

        private int mRoomMaxTeammate = 3;   //房间队友最大显示数

        private int m_kickMemberNo = 0;     //当前被提出队员序号

        private Color m_greyColor = new Color(0.1776f, 0.1776f, 0.1776f);        

        private string m_nameColorStartCode = "[7bb6c2]";
        private string m_nameColorEndCode = "[-]";

        private int[] m_guideBtnID = new int[11];        

        void Awake()
        {
            for (int i = 0; i < mRoomMaxTeammate; i++)
            {
                TeamRoomMemberItem item = ((GameObject)Instantiate(ATeamRoomMemberItem.gameObject)).GetComponent<TeamRoomMemberItem>();
                item.InitInfo(MemberTrans[i], WaitTabs[i]);
                mTeamMemberList.Add(item);
            }

            //按钮事件
            //AddEnergyButton.SetCallBackFuntion(OnAddEnergyClick, null);
            CaptainDisbandButton.SetCallBackFuntion(OnCaptainDisbandClick, null);
            CaptainSelectButton.SetCallBackFuntion(OnCaptainSelectClick, null);
            CaptainStartButton.SetCallBackFuntion(OnCaptainStartClick, null);
            MemberLeaveButton.SetCallBackFuntion(OnMemberLeaveClick, null);
            MemberReadyButton.SetCallBackFuntion(OnMemberReadyClick, null);
            PosOneInviteButton.SetCallBackFuntion(OnPosOneInviteClick, null);
            PosTwoInviteButton.SetCallBackFuntion(OnPosTwoInviteClick, null);
            PosThreeInviteButton.SetCallBackFuntion(OnPosThreeInviteClick, null);
            PosOneKickButton.SetCallBackFuntion(OnPosOneKickButtonClick, null);
            PosTwoKickButton.SetCallBackFuntion(OnPosTwoKickButtonClick, null);
            PosThreeKickButton.SetCallBackFuntion(OnPosThreeKickButtonClick, null);
            Button_Recruit.SetCallBackFuntion(OnChatButtonClick, null);

            //TODO GuideBtnManager.Instance.RegGuideButton(CaptainDisbandButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(CaptainSelectButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(CaptainStartButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[2]);
            //TODO GuideBtnManager.Instance.RegGuideButton(MemberLeaveButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[3]);
            //TODO GuideBtnManager.Instance.RegGuideButton(MemberReadyButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[4]);
            //TODO GuideBtnManager.Instance.RegGuideButton(PosOneInviteButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[5]);
            //TODO GuideBtnManager.Instance.RegGuideButton(PosTwoInviteButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[6]);
            //TODO GuideBtnManager.Instance.RegGuideButton(PosThreeInviteButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[7]);
            //TODO GuideBtnManager.Instance.RegGuideButton(PosOneKickButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[8]);
            //TODO GuideBtnManager.Instance.RegGuideButton(PosTwoKickButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[9]);
            //TODO GuideBtnManager.Instance.RegGuideButton(PosThreeKickButton.gameObject, UIType.TeamInfo, SubType.TeamRoom, out m_guideBtnID[10]);

            //记录文字
            //ActiveLifeFTx = ActiveLifeLabel.text;
            ExpFTx = ExpLabel.text + "  ";
            CopperFTx = CopperLabel.text + "  ";
            //TimeFTx = TimeLabel.text + "  ";
            //ComboFTx = ComboLabel.text + "  ";
            ActiveCostFTx = "";
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++ )
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        public void ShowPanel()
        {
            transform.localPosition = Vector3.zero;
            UpdateAreaTitleLabel();
            //UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseWorldChatWindow, null);//关闭聊天窗口
        }        

        public void ClosePanel()
        {
            transform.localPosition = new Vector3(0, 0, -800);
            //MainUIController.Instance.
            SetTeamHeroView(false);
            DeleteHeroModels();//退出房间清除队员model
        }
        //恢复准备按钮
        public void RestoreReadyButton()
        {
            MemberReadyButton.SetButtonStatus(false);
            MemberReadyButton.SetButtonTextColor(Color.white);
            MemberReadyButton.SetButtonActive(true);
            MemberReadyButton.animation.Play();
        }
        //队长风格
        private void ShowCaptainInterface()
        {
            CaptainInterface.gameObject.SetActive(true);
            MemberInterface.gameObject.SetActive(false);
            Background_Recruit.SetActive(true);
            Button_Recruit.gameObject.SetActive(true);
        }
        //队员风格
        private void ShowMemberInterface()
        {
            CaptainInterface.gameObject.SetActive(false);
            MemberInterface.gameObject.SetActive(true);
            Background_Recruit.SetActive(false);
            Button_Recruit.gameObject.SetActive(false);
            

            //队员准备按钮重置
            this.RestoreReadyButton(); 
        }

        /// <summary>
        /// 受否在队伍界面
        /// </summary>
        /// <returns></returns>
        public bool IsInTeamRoomPanel()
        {
            return transform.localPosition == Vector3.zero;
        }

        public void SetTeamHeroView(bool isShow)
        {
            if (m_TeamMemberViewCamera != null)
            {
                m_TeamMemberViewCamera.SetActive(isShow);                
            }            
        }
		//离开组队界面
		public void ExitTeamUIPanel()
		{
			var teamProp = TeamManager.Instance.MyTeamProp;		
			var playerData = PlayerManager.Instance.FindHeroDataModel();
			//队长
			if (teamProp.TeamContext.dwCaptainId == playerData.ActorID)
			{
				this.DisbandTeam();
			}
			else
			{
				this.OnMemberLeaveClick(null);
			}
		}

        /// <summary>
        /// 删除所有队员model
        /// </summary>
        public void DeleteHeroModels()
        {
            //关闭hero model view
            m_MemberHeroViews.ApplyAllItem(p=>
                {
                    p.DeleteHeroModeView();
                });
        }

        private void UpdateAreaTitleLabel()
        {
            //更新副本名称显示
			var currentEctype = TeamManager.Instance.CurrentEctypeData;
			//Todo：onSelectEctypeData协议已经取消dwEctypeID和byDiff难度，如果使用需要从新更改
//            if (EctypeConfigManager.Instance.EctypeSelectConfigList.ContainsKey((int)currentEctype.dwEctypeID))
//            {
//                var ectypeSelect = EctypeConfigManager.Instance.EctypeSelectConfigList[(int)currentEctype.dwEctypeID];
//                var ectypeID = ectypeSelect._vectContainer[currentEctype.byDiff - 1];
//                var ectypeInfo = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
//                AreaTitleLabel.text = LanguageTextManager.GetString(ectypeInfo.lEctypeName);
//            }
        }

        public void UpdateTeammateInfo(SMsgTeamNum_SC sMsgTeamNum)
        {         
            var teamProp = sMsgTeamNum.SMsgTeamProps[0];
            TeamManager.Instance.RegisteTeam(teamProp);

            int teammateNo = 0;
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            //var propMembers = teamProp.TeamMemberNum_SC.SMsgTeamPropMembers.Where(p => p.TeamMemberContext.dwActorID != playerData.ActorID).ToArray();
            var propMembers = teamProp.TeamMemberNum_SC.SMsgTeamPropMembers;
            int teammateNum = propMembers.Length;

            //队长风格
            bool isCaptain = false;
            if (teamProp.TeamContext.dwCaptainId == playerData.ActorID)
            {
                ShowCaptainInterface();
                isCaptain = true;
            }
            else                                                     
            {
                ShowMemberInterface();
            }

            //初始化 hero model views
            if (m_MemberHeroViews == null)
            {
                m_TeamMemberViewCamera = (GameObject)Instantiate(TeamCamaraRoot);
                //m_TeamMemberViewCamera.transform.parent = transform;
                //m_TeamMemberViewCamera.transform.localScale = Vector3.one;
                //m_TeamMemberViewCamera.transform.localPosition = new Vector3(-2000, 0, 0);
                m_TeamMemberViewCamera.transform.localPosition = new Vector3(0, 0, 10);//z为10避免看到town的label
                m_MemberHeroViews = m_TeamMemberViewCamera.GetComponentsInChildren<ContainerHeroView>();
            }
            m_TeamMemberViewCamera.SetActive(true);            
            var camera = m_TeamMemberViewCamera.GetComponentInChildren<Camera>();
            camera.enabled = true;

            EctypeSelectConfigData ectypeData = null;
            ectypeData = EctypeConfigManager.Instance.EctypeSelectConfigList[teamProp.TeamContext.dwEctypeId];
            var ectypeID = ectypeData._vectContainer[teamProp.TeamContext.byEctypeDifficulty - 1];
            var ectypeInfo = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];

            AreaTitleLabel.text = LanguageTextManager.GetString(ectypeInfo.lEctypeName);//更新标题
            
            mTeamMemberList.ApplyAllItem(p =>
                {                    
                    if (teammateNo < teammateNum)
                    {
                        p.SetInfo(propMembers[teammateNo].TeamMemberContext, ectypeInfo);
                        //显示hero model view
					m_MemberHeroViews[teammateNo].ShowHeroModelView((int)propMembers[teammateNo].TeamMemberContext.dwActorID,propMembers[teammateNo].TeamMemberContext.byKind, propMembers[teammateNo].TeamMemberContext.nFashionID
                           , propMembers[teammateNo].TeamMemberContext.nCurWeaponID);
                    }
                    else
                    {
                        p.Close();                        
                    }

                    teammateNo++;
                });

            //如果是队长，更新邀请请离按钮
            if (isCaptain)
            {
                ResetButtons();
                
                //
                if (teamProp.TeamContext.byCurNum <= 1)
                {
                    ExplanationLabel.text = LanguageTextManager.GetString("IDS_H1_171");
                }
                else
                {
                    ExplanationLabel.text = LanguageTextManager.GetString("IDS_H1_170");
                }
            }
            else
            {
                ExplanationLabel.text = "";

                //自身是队员 判断准备按钮是否置灰
                var myTeamData = teamProp.TeamMemberNum_SC.SMsgTeamPropMembers.SingleOrDefault(p => p.TeamMemberContext.dwActorID == playerData.ActorID);
                if (myTeamData.TeamMemberContext.byFightReady == 1)
                {
                    MemberReadyButton.SetButtonStatus(true);
                    MemberReadyButton.SetButtonTextColor(m_greyColor);
                    MemberReadyButton.SetButtonActive(false);
                    MemberReadyButton.animation.Stop();
                }                
            }

            //判断是否全员准备
            bool isAllReady = true;           
            teamProp.TeamMemberNum_SC.SMsgTeamPropMembers.ApplyAllItem(p =>
            {                
                //TraceUtil.Log("[Weapean ID]"+p.TeamMemberContext.nCurWeaponID);
                if (p.TeamMemberContext.byFightReady == 0)
                {
                    isAllReady = false;
                }
            });
            if (isAllReady)
            {
                CaptainStartButton.animation.Play();
            }
            else
            {
                CaptainStartButton.animation.Stop();
            }
            /*

            //挑战界面信息
            var challengeData = TeamManager.Instance.CurrentEctypeLevelData;
            ComboLabel.text = ComboFTx + (challengeData.wHighestCombo == 0 ? "" : challengeData.wHighestCombo.ToString());

            //通关时间
            int Passtime = (int)challengeData.dwBestClearTime;
            int Minutes = Passtime / 60000;
            int Second = Passtime % 60000 / 1000;
            int MS = Passtime % 60000 % 1000 / 10;
            string PassTime = string.Format("{0}:{1}:{2}", Minutes, Second, MS);

            TimeLabel.text = TimeFTx + (PassTime == "0:0:0" ? "" : PassTime);

            //TimeLabel.text = TimeFTx + challengeData.dwBestClearTime.ToString();
             
            */
            

            //\如果一开始就有队伍，这里索取的值为空
            if(ectypeData!=null)
            {
                ectypeData.InitectContainer();
                int containerID = ectypeData.GetVectContainer(teamProp.TeamContext.byEctypeDifficulty);                
                var localEctypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[containerID];
                
                if (localEctypeContainerData != null)
                {
                    ExpLabel.text = ExpFTx + localEctypeContainerData.lExperience.ToString();
                    CopperLabel.text = CopperFTx + localEctypeContainerData.lMoney.ToString();

                    TitleLabel.text = LanguageTextManager.GetString(localEctypeContainerData.lEctypeName);

                    //储存副本信息
                    TeamManager.Instance.SetEctypeData(new SMSGEctypeData_SC()
					                                   {
						//Todo：onSelectEctypeData协议已经取消dwEctypeID和byDiff难度，如果使用需要从新更改
//                        dwEctypeID = (uint)ectypeData._lEctypeID,
//                        byDiff = (byte)localEctypeContainerData.lDifficulty
                    });
                }
                
                CostTypeSpriteSwith.ChangeSprite(localEctypeContainerData.lCostType);//消耗类型
                ActiveCostLabel.text = ActiveCostFTx + localEctypeContainerData.lCostEnergy.ToString();                
            }
            

            //队长如果是邀请界面
            if (TeamInvitePanel.gameObject.activeInHierarchy)
            {
                TeamInvitePanel.ShowFriendList();
            }

        }

        public void UpdateTeamMemberReadyState(uint dwActorId, int readyResult)
        {
            //m_MemberReadyProp.SetReadyValue(dwActorId, isReady);
            //m_MemberReadyProp.GetDictionary().ApplyAllItem(p =>
            //    {
            //        //TraceUtil.Log("Member Data : id ="+p.Key+" value ="+p.Value);
            //    });
            ////更新队员准备状态显示
            //mTeamMemberList.ApplyAllItem(p =>
            //{                
            //    if (m_MemberReadyProp.GetDictionary().ContainsKey(p.TeamMemberContext.dwActorID))
            //    {
            //        //TraceUtil.Log("===>找到并设置");
            //        p.SetReadyState(m_MemberReadyProp.GetReadyValue(p.TeamMemberContext.dwActorID));
            //        //TraceUtil.Log("设置value: " + m_MemberReadyProp.GetDictionary()[p.TeamMemberContext.dwActorID]);
            //    }
            //});            
            if (PlayerManager.Instance.FindHeroDataModel().ActorID == dwActorId)
            {
                //队员准备按钮置灰
                MemberReadyButton.SetButtonStatus(true);
                MemberReadyButton.SetButtonTextColor(m_greyColor);
                MemberReadyButton.SetButtonActive(false);
                MemberReadyButton.animation.Stop();

                ShowCostLabelInButton();
                //return;
            }


            TeamManager.Instance.SetTeamMemberReadyStatu(dwActorId, readyResult);

            var teamMember = mTeamMemberList.SingleOrDefault(p => p.TeamMemberContext.dwActorID == dwActorId);
            if (teamMember != null)
            {
                //TraceUtil.Log("teamMember 's actorID = "+dwActorId);
                
                teamMember.TeamMemberContext.SetFightReadyValue(readyResult);   //\                
                teamMember.SetReadyState(readyResult);
            }

            //判断是否全员准备
            bool isAllReady = true;
            var myTeamProp = TeamManager.Instance.MyTeamProp;

            myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.ApplyAllItem(p =>
            {
                if (p.TeamMemberContext.byFightReady == 0)
                {
                    isAllReady = false;
                }
            });
            if (isAllReady)
            {
                CaptainStartButton.animation.Play();
            }
            else
            {
                CaptainStartButton.animation.Stop();
            }
           
        }

        //封魔副本邀请
        public void ShowEctypepanelMessageBox(Vector3 messageBoxPos)
        {
            //0.1.3版已经去掉外部判断
            /*
            var inviteEctypeData = NetServiceManager.Instance.TeamService.GetTeamMemberInvite();
            int selectID = (int)inviteEctypeData.dwEctypeId;
            int diff = (int)inviteEctypeData.byEctypDiff;
            int ectypeID = 0;
            if (EctypeConfigManager.Instance.EctypeSelectConfigList.ContainsKey(selectID))
            {
                var selectData = EctypeConfigManager.Instance.EctypeSelectConfigList[selectID];
                ectypeID = selectData._vectContainer[diff - 1];                
            }
            EctypeContainerData ectypeData;
            if (EctypeConfigManager.Instance.EctypeContainerConfigList.ContainsKey(ectypeID))
            {
                ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
                int Cost = int.Parse(ectypeData.lCostEnergy);                
                int myCostEnergy = 0;
                switch (ectypeData.lCostType)
                {
                    case 2:
                        myCostEnergy = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
                        notEnoughtCostStr = LanguageTextManager.GetString("IDS_H2_44");
                        break;
                    case 3:
                        myCostEnergy = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
                        notEnoughtCostStr = LanguageTextManager.GetString("IDS_H1_231");
                        break;
                    default:
                        return;
                }
                if (myCostEnergy >= Cost)
                {
                    EctypepanelMessageBox.ShowPanel(LanguageTextManager.GetString("IDS_H1_319"), ectypeData.lCostType, Cost, EnoughCostEnergy);
                }
                else
                {
                    EctypepanelMessageBox.ShowPanel(LanguageTextManager.GetString("IDS_H1_319"), ectypeData.lCostType, Cost, NotEnoughCostEnergy);
                }                
                EctypepanelMessageBox.transform.localPosition = messageBoxPos;
            }  
            */
        }
        string notEnoughtCostStr = "";
        void NotEnoughCostEnergy(object obj)
        {
            //MessageBox.Instance.Show(4, "", notEnoughtCostStr, LanguageTextManager.GetString("IDS_H2_55"));
            MessageBox.Instance.ShowTips(4, notEnoughtCostStr, 1f);
        }
        void EnoughCostEnergy(object obj)
        {
            NetServiceManager.Instance.TeamService.SureJoinTargetTeam();
        }

        //购买活力按钮
        void OnAddEnergyClick(object obj)
        {
            MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_166"),
                LanguageTextManager.GetString("IDS_H2_55"), LanguageTextManager.GetString("IDS_H2_28"), BuyActivelife, null);
        }
        //队长解散按钮
        void OnCaptainDisbandClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_165"),
                LanguageTextManager.GetString("IDS_H2_19"), LanguageTextManager.GetString("IDS_H2_28"), DisbandTeam, null);
        }
        
        //队长选择副本按钮
        void OnCaptainSelectClick(object obj)
        {                        
            //UI.MainUI.MainUIController.Instance.SaveCurrentUIStatus(UIType.TeamInfo);
            UI.MainUI.MainUIController.Instance.OpenMainUI(UIType.Battle);
        }
        //队长开始按钮
        void OnCaptainStartClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            //判断是否全部队员准备
            bool isAllReady = true;
            var myTeamProp = TeamManager.Instance.MyTeamProp;

            myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.ApplyAllItem(p =>
                {
                    if (p.TeamMemberContext.byFightReady == 0)
                    {
                        isAllReady = false;
                    }
                });
            if (!isAllReady)
            {
                //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_168"), LanguageTextManager.GetString("IDS_H2_55"));
                MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_168"), 1f);
                return;
            }           

            //var heroDataModel = PlayerManager.Instance.FindHeroDataModel();
            //var challengeData = TeamManager.Instance.CurrentEctypeLevelData;

            //封魔副本
            /* 在外部判断，方法先保留
            if (m_currentEctypeContainerData == null)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"副本信息为空");
                return;
            }
            if (m_currentEctypeContainerData.lEctypeType == 1)
            {
                int Cost = int.Parse(m_currentEctypeContainerData.lCostEnergy);                
                EctypepanelMessageBox.ShowPanel(LanguageTextManager.GetString("IDS_H1_319"), m_currentEctypeContainerData.lCostType, Cost, SpecialEctypeStart);
            }
            else
            {
                if (heroDataModel.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE <= 0)
                {
                    VigourMessageManager.ShowNoEnoughVigourPanel();
                    return;
                }

                this.SpecialEctypeStart(null);
            }
            */
            //if (heroDataModel.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE <= 0)
            //{
            //    VigourMessageManager.ShowNoEnoughVigourPanel();
            //    return;
            //}

            //this.SpecialEctypeStart(null);
            ShowCostLabelInButton();
        }
        private void ShowCostLabelInButton()
        {
            if (IsShowCostLabel)
                return;
			var currentEctype = TeamManager.Instance.CurrentEctypeData;
			//Todo：onSelectEctypeData协议已经取消dwEctypeID和byDiff难度，如果使用需要从新更改
//            var ectypeSelect = EctypeConfigManager.Instance.EctypeSelectConfigList[(int)currentEctype.dwEctypeID];
//            var ectypeID = ectypeSelect._vectContainer[currentEctype.byDiff - 1];
			EctypeContainerData SelectContainerData = null;// = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
            bool isShowCostType = true;
            int localCostNumber = int.Parse(SelectContainerData.lCostEnergy);
            int costNumber = 0;
            switch (SelectContainerData.lCostType)
            {
                case 1:
                    costNumber = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
                    isShowCostType = costNumber > 0;
                    break;
                case 2:
                    costNumber = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
                    isShowCostType = costNumber >= localCostNumber;
                    break;
                case 3:
                    costNumber = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
                    isShowCostType = costNumber >= localCostNumber;
                    break;
                default:
                    break;
            }
            //if (isShowCostType)
            //{
            //    LocalButtonCallBack Tips = CreatObjectToNGUI.InstantiateObj(CostLabelPreafab, CaptainStartButton.transform).GetComponent<LocalButtonCallBack>();
            //    Vector3 fromPoint = new Vector3(0, 50, -30);
            //    Vector3 toPoint = new Vector3(0, 0, -30);
            //    TweenPosition.Begin(Tips.gameObject, 0.5f, fromPoint, toPoint, SpecialEctypeStart);
            //    TweenAlpha.Begin(Tips.gameObject, 0.5f, 1, 0, null);
            //    Tips.SetButtonBackground(SelectContainerData.lCostType);
            //    Tips.SetButtonText(string.Format("-{0}", costNumber>localCostNumber?localCostNumber:costNumber));
            //}
            //else
            //{
            //    SpecialEctypeStart(null);
            //}
            //GameObject Tips = CreatObjectToNGUI.InstantiateObj(CostLabelPreafab, CostLabelPreafab.transform.parent);
            //Vector3 curPoint = CostLabelPreafab.transform.localPosition;
            //Vector3 fromPoint = curPoint + new Vector3(0, 30, -30);
            //Vector3 toPoint = curPoint + new Vector3(0, 0, -30);

            //回调
            var teamProp = TeamManager.Instance.MyTeamProp;
            TeamManager.Instance.RegisteTeam(teamProp);            
            //队长
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            if (teamProp.TeamContext.dwCaptainId == playerData.ActorID)
            {
				if (isShowCostType)
				{
					SoundManager.Instance.PlaySoundEffect("Sound_Button_Fight");
					GameObject Tips = CreatObjectToNGUI.InstantiateObj(CostLabelPreafab, CostLabelPreafab.transform.parent);
					Vector3 curPoint = CostLabelPreafab.transform.localPosition;
					Vector3 fromPoint = curPoint + new Vector3(0, 30, -30);
					Vector3 toPoint = curPoint + new Vector3(0, 0, -30);
					TweenPosition.Begin(Tips, 0.5f, fromPoint, toPoint, null);
					TweenAlpha.Begin(Tips, 0.5f, 1, 0, null);
					StartCoroutine(SetShowCostLabelFalseForTime(1));
					StartCoroutine(LaterSpecialEctypeStart());
				}
				else
				{
					SpecialEctypeStart(null);
				}

            }
            else
            {
                if (isShowCostType)
                {
					SoundManager.Instance.PlaySoundEffect("Sound_Button_Fight");
                    GameObject Tips = CreatObjectToNGUI.InstantiateObj(CostLabelPreafab, CostLabelPreafab.transform.parent);
                    Vector3 curPoint = CostLabelPreafab.transform.localPosition;
                    Vector3 fromPoint = curPoint + new Vector3(0, 30, -30);
                    Vector3 toPoint = curPoint + new Vector3(0, 0, -30);
                    TweenPosition.Begin(Tips, 0.5f, fromPoint, toPoint, null);
                    TweenAlpha.Begin(Tips, 0.5f, 1, 0, null);
                    StartCoroutine(SetShowCostLabelFalseForTime(1));
                }                
            }                        
        }
        IEnumerator SetShowCostLabelFalseForTime(float waitTime)
        {
            IsShowCostLabel = true;
            yield return new WaitForSeconds(waitTime);
            IsShowCostLabel = false;
        }
		IEnumerator LaterSpecialEctypeStart()
		{
			yield return new WaitForSeconds(2f);
			SpecialEctypeStart(null);
		}

        void SpecialEctypeStart(object obj)
        {            
            var teamData = TeamManager.Instance.MyTeamProp;
            SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS()
			{
				//TODO:进入技能协议有更改，去掉副本iD和难度,需要从新修改
//                uidEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity,
//                dwEctypeId = (int)teamData.TeamContext.dwEctypeId,
//                byDifficulty = (byte)teamData.TeamContext.byEctypeDifficulty
            };
            NetServiceManager.Instance.EctypeService.SendEctypeRequest(sMSGEctypeRequestCreate_CS);
            LoadingUI.Instance.Show();
        }
        //位置一邀请按钮
        void OnPosOneInviteClick(object obj)
        {
            InviteMember();
        }
        //位置二邀请按钮
        void OnPosTwoInviteClick(object obj)
        {
            InviteMember();
        }
        //位置三邀请按钮
        void OnPosThreeInviteClick(object obj)
        {
            InviteMember();
        }
        //位置一请离按钮
        void OnPosOneKickButtonClick(object obj)
        {
            m_kickMemberNo = 0;
            MessageBox.Instance.Show(4, "", string.Format(LanguageTextManager.GetString("IDS_H1_164"), m_nameColorStartCode + mTeamMemberList[0].NickNameLabel.text + m_nameColorEndCode),
                LanguageTextManager.GetString("IDS_H2_27"), LanguageTextManager.GetString("IDS_H2_28"), KickMember, null);            
        }
        //位置二请离按钮
        void OnPosTwoKickButtonClick(object obj)
        {
            m_kickMemberNo = 1;
            MessageBox.Instance.Show(4, "", string.Format(LanguageTextManager.GetString("IDS_H1_164"), m_nameColorStartCode + mTeamMemberList[1].NickNameLabel.text + m_nameColorEndCode),
                LanguageTextManager.GetString("IDS_H2_27"), LanguageTextManager.GetString("IDS_H2_28"), KickMember, null);            
        }
        //位置三请离按钮
        void OnPosThreeKickButtonClick(object obj)
        {
            m_kickMemberNo = 2;
            MessageBox.Instance.Show(4, "", string.Format(LanguageTextManager.GetString("IDS_H1_164"), m_nameColorStartCode + mTeamMemberList[2].NickNameLabel.text + m_nameColorEndCode),
                LanguageTextManager.GetString("IDS_H2_27"), LanguageTextManager.GetString("IDS_H2_28"), KickMember, null);            
        }
        //队员离开按钮
        void OnMemberLeaveClick(object obj)
        {
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            var teamData = TeamManager.Instance.MyTeamProp;
            //TraceUtil.Log("队员离开click");
            NetServiceManager.Instance.TeamService.SendTeamMemberLeaveMsg(new SMsgTeamMemberLeave_SC() { dwActorID = (uint)playerData.ActorID, dwTeamID = teamData.TeamContext.dwId });
        }
        //队员准备按钮
        void OnMemberReadyClick(object obj)
        {
            var heroDataModel = PlayerManager.Instance.FindHeroDataModel();   
            var teamData = TeamManager.Instance.MyTeamProp;
            //var challengeData = TeamManager.Instance.CurrentEctypeLevelData;            

            ////不自己判断体力是否足够
            //if (heroDataModel.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE > 0)    
            //{
            //    NetServiceManager.Instance.TeamService.SendTeamMemberReadyMsg(new SMsgTeamMemberReady_CS()
            //    {
            //        dwActorID = (uint)heroDataModel.ActorID,
            //        dwTeamID = teamData.TeamContext.dwId,
            //    });
            //    //队员准备按钮置灰
            //    MemberReadyButton.SetButtonStatus(true);
            //    MemberReadyButton.SetButtonTextColor(m_greyColor);
            //    MemberReadyButton.SetButtonActive(false);
            //}
            //else
            //{
            //    VigourMessageManager.ShowNoEnoughVigourPanel();
            //    //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_161"), LanguageTextManager.GetString("IDS_H2_12"), LanguageTextManager.GetString("IDS_H2_28"), BuyActivelife, null);
            //}
            
            NetServiceManager.Instance.TeamService.SendTeamMemberReadyMsg(new SMsgTeamMemberReady_CS()
            {
                dwActorID = (uint)heroDataModel.ActorID,
                dwTeamID = teamData.TeamContext.dwId,
            });           
        }

        void OnChatButtonClick(object obj)
        {
            //UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenWorldChatWindow, null);
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            string chat = string.Format(LanguageTextManager.GetString("IDS_H2_76"), AreaTitleLabel.text);
            NetServiceManager.Instance.ChatService.SendChat((uint)playerData.ActorID, 0, chat, 1, Chat.ChatDefine.MSG_CHAT_WORLD);
        }

        private void ResetButtons()
        {
            //if (mTeamMemberList[0].gameObject.activeInHierarchy)
            //{
            //    PosOneInviteButton.gameObject.SetActive(false);
            //    PosOneKickButton.gameObject.SetActive(true);
            //}
            //else
            //{
            //    PosOneInviteButton.gameObject.SetActive(true);
            //    PosOneKickButton.gameObject.SetActive(false);
            //}

            if (mTeamMemberList[1].gameObject.activeInHierarchy)
            {
                PosTwoInviteButton.gameObject.SetActive(false);
                PosTwoKickButton.gameObject.SetActive(true);
            }
            else
            {
                PosTwoInviteButton.gameObject.SetActive(true);
                PosTwoKickButton.gameObject.SetActive(false);
            }

            if (mTeamMemberList[2].gameObject.activeInHierarchy)
            {
                PosThreeInviteButton.gameObject.SetActive(false);
                PosThreeKickButton.gameObject.SetActive(true);
            }
            else
            {
                PosThreeInviteButton.gameObject.SetActive(true);
                PosThreeKickButton.gameObject.SetActive(false);
            } 
        }
        //更新活力值显示
        private void UpdateActiveLife()
        {
            //var heroDataModel = PlayerManager.Instance.FindHeroDataModel();
            //ActiveLifeLabel.text =ActiveLifeFTx+heroDataModel.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE.ToString()+"/"+
            //    heroDataModel.PlayerValues.PLAYER_FIELD_MAX_ACTIVELIFE;
        }
       
        /// <summary>
        /// 检查背包 由外部判断玩家是否已经在队伍中是否需要检查
        /// </summary>
        public void CheckBackpack()
        {
            ushort maxNum = ContainerInfomanager.Instance.GetContainerClientContsext(2).wMaxSize;
            var backpack = ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(2).Where(p => p.uidGoods != 0).ToList();
            if (maxNum - backpack.Count < 2)
            {
                //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_201"), LanguageTextManager.GetString("IDS_H2_55"));
                MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_201"), 1f);
            }
        }

        //确定解散队伍
        void DisbandTeam()
        {
            //transform.parent.transform.localPosition = new Vector3(0, 0, -800);
            
            var teamData = TeamManager.Instance.MyTeamProp;
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            NetServiceManager.Instance.TeamService.SendTeamDisbandMsg(new SMsgTeamDisband_CS() { dwActorID = (uint)playerData.ActorID, dwTeamID = teamData.TeamContext.dwId });
        }
        //购买活力
        public void BuyActivelife()
        {

        }
        //请离队员
        void KickMember()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            var teamData = TeamManager.Instance.MyTeamProp;
            mTeamMemberList[m_kickMemberNo].BeKicked((uint)teamData.TeamContext.dwCaptainId, teamData.TeamContext.dwId);
            //NetServiceManager.Instance.TeamService.SendTeamMemberKickMsg(new SMsgTeamMemberKick_CS() { });
        }
        //邀请队员
        void InviteMember()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            TeamInvitePanel.gameObject.SetActive(true);
            TeamInvitePanel.ShowFriendList();
        }

        //活力不足窗口
        private void ShowActiveLifeNoEnoughWindow()
        {

        }

        //public class MemberReadyProp
        //{
        //    public uint dwTeamID;
        //    private Dictionary<uint, bool> m_dic = new Dictionary<uint, bool>();

        //    public void ReSet(uint dwTeamID)
        //    {
        //        if (this.dwTeamID != dwTeamID)
        //        {
        //            this.dwTeamID = dwTeamID;
        //            m_dic.Clear();
        //        }
        //    }

        //    public void AddActorID(uint dwActorID)
        //    {
        //        if (!m_dic.ContainsKey(dwActorID))
        //        {
        //            m_dic.Add(dwActorID, false);
        //        }
        //    }

        //    public bool GetReadyValue(uint dwActorID)
        //    {
        //        if (m_dic.ContainsKey(dwActorID))
        //        {
        //            return m_dic[dwActorID];
        //        }
        //        return false;
        //    }

        //    public void SetReadyValue(uint dwActorID, bool isReady)
        //    {
        //        if (m_dic.ContainsKey(dwActorID))
        //        {
        //            m_dic[dwActorID] = isReady;
        //        }
        //        else
        //        {
        //            this.AddActorID(dwActorID);
        //        }
        //    }

        //    public Dictionary<uint, bool> GetDictionary()
        //    {
        //        return m_dic;
        //    }
        //}
    }
}