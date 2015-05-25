using UnityEngine;
using System.Collections;
using UI.Team;
using System.Linq;
namespace UI.MainUI
{
    

    public class TeamInfoUIManager : BaseUIPanel
    {
        /// <summary>
        /// 组队管理面板
        /// </summary>
        public TeamOrganizePanel TeamOrganizePanel;
        public TeamRoomPanel TeamRoomPanel;

        public GameObject CommonToolPrefab;

        // 队员状态
        public enum TEAM_MEMBER_STATUS
        {
            TEAM_MEMBER_NONE_STATUS = 0,	// 无状态, 默认
            TEAM_MEMBER_READY_STATUS = 1,		// 准备状态
        };


        void Awake()
        {
            if (UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform); }
            if (TeamOrganizePanel == null)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"世界队伍信息面板实例未手动关联！");
            }
            transform.localPosition = new Vector3(0, 0, -800);  //位置先置前,避免队伍界面弹封魔副本邀请
            RegisterEventHandler();
        }

        public override void Show(params object[] value)
        {
            transform.localPosition = Vector3.zero;
            TeamOrganizePanel.UpdateAreaTitleLabel();
            TeamOrganizePanel.OnRefreshWorldTeamInfoClick(0);
            //RemoveEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
            //AddEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
            base.Show(value);
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            TeamRoomPanel.ShowEctypepanelMessageBox(new Vector3(0, 0, 800));
            TeamRoomPanel.SetTeamHeroView(false);
            //RemoveEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
            base.Close();
        }

        void ShowEctypeInvitePanelBox()
        {
            CleanUpUIStatus();
            Close();
        }

        private void ShowMyTeamInfo()
        {            
            TeamRoomPanel.ShowPanel();
            TeamOrganizePanel.ClosePanel();
            LoadingUI.Instance.Close();
        }

        private void ShowWorldTeamInfo()
        {
            TeamOrganizePanel.ShowPanel();
            TeamRoomPanel.ClosePanel();
            LoadingUI.Instance.Close();
        }

        //刷新队伍
        private void RefreshTeamList()
        {
            TeamOrganizePanel.OnRefreshWorldTeamInfoClick(0);
        }

        void CreateTeamHandle(INotifyArgs e)
        {
            SMsgTeamNum_SC sMsgTeamNum = (SMsgTeamNum_SC)e;

            StartCoroutine(CreateTeamLater(sMsgTeamNum));
        }
        //为了队员摄像机创建安全
        IEnumerator CreateTeamLater(SMsgTeamNum_SC sMsgTeamNum)
        {
            yield return new WaitForEndOfFrame();
            bool isInTeamRoomPanel = TeamRoomPanel.IsInTeamRoomPanel();
            ShowMyTeamInfo();
            TeamRoomPanel.UpdateTeammateInfo(sMsgTeamNum);
            if (!isInTeamRoomPanel)
            {
                //检查背包
                TeamRoomPanel.CheckBackpack();
            }
        }
        void CreateTeamInfoItemsHandle(INotifyArgs e)
        {
            SMsgTeamNum_SC sMsgTeamNum = (SMsgTeamNum_SC)e;

            //ShowWorldTeamInfo();
            TeamOrganizePanel.CreateTeamInfoItems(sMsgTeamNum);
        }


        bool isKick;
        void TeamMemberLeaveHandle(INotifyArgs e)
        {            
            SMsgTeamMemberLeave_SC teamMemberLeave = (SMsgTeamMemberLeave_SC)e;
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            if (teamMemberLeave.dwActorID == playerData.ActorID)
            {
                ShowWorldTeamInfo();
                if (isKick)
                {
                    //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_162"), LanguageTextManager.GetString("IDS_H2_55"));
                    MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_162"), 1f);
                    //刷新队伍
                    TeamOrganizePanel.OnRefreshWorldTeamInfoClick(0);
                }                
            }
            else
            {
                //先删除之前的队员model
                TeamRoomPanel.DeleteHeroModels();
                var teamData = TeamManager.Instance.MyTeamProp;
                TeamRoomPanel.UpdateTeammateInfo(new SMsgTeamNum_SC() { SMsgTeamProps = new SMsgTeamProp_SC[1] { teamData } });
            }
        }
        void TeamMemberBeKickHandle(INotifyArgs e)
        {
            isKick = true;
            this.TeamMemberLeaveHandle(e);
            isKick = false;
        }

        void TeamDisbandHandle(INotifyArgs e)
        {
            SMsgTeamDisband_SC teamDisbandMsg = (SMsgTeamDisband_SC)e;
            var teamData = TeamManager.Instance.MyTeamProp;
            if (teamData.TeamContext.dwId == teamDisbandMsg.dwTeamID)
            {
                ShowWorldTeamInfo();
                //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_163"), LanguageTextManager.GetString("IDS_H2_55"));
                MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_163"), 1f);
            }
            //清除队伍数据
            TeamManager.Instance.UnRegisteTeam();
            //刷新队伍
            TeamOrganizePanel.OnRefreshWorldTeamInfoClick(0);
        }
        void TeamMemberReadyHandle(INotifyArgs e)
        {
            SMsgTeamMemberReadyResult_SC readyResult = (SMsgTeamMemberReadyResult_SC)e;

            //bool isReady = ((TEAM_MEMBER_STATUS)readyResult.byResultCode) == TEAM_MEMBER_STATUS.TEAM_MEMBER_READY_STATUS ? true : false;
            TeamRoomPanel.UpdateTeamMemberReadyState(readyResult.dwActorID, readyResult.byResultCode);
        }
        void UpdateTeamInfoHandle(INotifyArgs e)
        {
            TeamRoomPanel.UpdateTeammateInfo(new SMsgTeamNum_SC()
                {
                    wTeamNum = 1,
                    SMsgTeamProps = new SMsgTeamProp_SC[1]{TeamManager.Instance.MyTeamProp},
                });
        }
        //没有队伍列表
        void TeamNoFoundListHandle(INotifyArgs e)
        {
            MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_160"),
            LanguageTextManager.GetString("IDS_H2_4"), LanguageTextManager.GetString("IDS_H2_28"), SureCreateTeam, null);
        }
        //队伍不存在 队伍已解散
        void TeamNoExistHandle(INotifyArgs e)
        {
            //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_82"), LanguageTextManager.GetString("IDS_H2_55"), RefreshTeamList);
            this.RefreshTeamList();
        }
        //队伍人数已满
        void TeamFullHandle(INotifyArgs e)
        {
            //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_81"), LanguageTextManager.GetString("IDS_H2_55"), RefreshTeamList);            
            this.RefreshTeamList();
        }
        //对方队伍正在副本战斗
        void TeamFightingHandle(INotifyArgs e)
        {
            //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_83"), LanguageTextManager.GetString("IDS_H2_55"));            
        }
        //有队员未准备
        void TeamExistMemberNoReadyHandle(INotifyArgs e)
        {
            //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_168"), LanguageTextManager.GetString("IDS_H2_55"));
            MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_168"), 1f);
        }
        //有队员没用挑战副本资格
        void EctypeNoQualificationHandle(INotifyArgs args)
        {
            SMSGEctypeResult_SC sMSGEctypeResult_SC = (SMSGEctypeResult_SC)args;
            string name = "";
            var teamData = TeamManager.Instance.MyTeamProp;
            sMSGEctypeResult_SC.dwActorIds.ApplyAllItem(p=>
                {
                    if(p != 0)
                    {
                        SMsgTeamPropMember_SC? memberData = teamData.TeamMemberNum_SC.SMsgTeamPropMembers.SingleOrDefault(k=>k.TeamMemberContext.dwActorID == p);
                        if(memberData != null)
                        {
                            name += " "+ memberData.Value.TeamMemberContext.szName;
                        }                        
                    }
                });
            //MessageBox.Instance.Show(4, "", string.Format(LanguageTextManager.GetString("IDS_H1_167"), name), LanguageTextManager.GetString("IDS_H2_55"));
            MessageBox.Instance.ShowTips(4, string.Format(LanguageTextManager.GetString("IDS_H1_167"), name), 1f);
        }
        private void SureCreateTeam()
        {
            NetServiceManager.Instance.TeamService.SendTeamCreateMsg();
        }
        //体力不足
        void TeamActiveLifeNotEnoughHandle(INotifyArgs e)
        {
            MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_161"), LanguageTextManager.GetString("IDS_H2_12"),
                LanguageTextManager.GetString("IDS_H2_28"), TeamRoomPanel.BuyActivelife, null);
        }
        //封魔副本邀请(特殊处理)
        void TeamMemberDevilInviteHandle(INotifyArgs e)
        {
            if (transform.localPosition == Vector3.zero)
            {
                TeamRoomPanel.ShowEctypepanelMessageBox(Vector3.zero);
            }
            else
            {
                TeamRoomPanel.ShowEctypepanelMessageBox(new Vector3(0, 0, 800));
            }
        }        

		void PlayerActiveUpdateHanlde(INotifyArgs arg)
		{
			UpdateTeamInfoHandle(null);
		}

        //翻页消息监听
        void ItemPageChangedHandle(PageChangedEventArg pageSmg)
        {             
            TeamOrganizePanel.ItemPageChanged(pageSmg);
        }

        //收到活力不足
        //void ShowNoEnoughVigourPanel(object obj)
        //{
            
        //}

        void RestoreTeamRoomPanelReadyButton(object obj)
        {
            LoadingUI.Instance.Close();
            TeamRoomPanel.RestoreReadyButton();
        }

        void TeamMemberEctypeLockErrorHandle(object obj)
        {
            LoadingUI.Instance.Close();
            TeamRoomPanel.RestoreReadyButton();
        }

		void OpenMainUIHandle(object obj)
		{
			UIType uiType = (UIType)obj;
			if(uiType== UIType.TopUp)
			{
				if(TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.wMemberNum > 0)
				{
					TeamRoomPanel.ExitTeamUIPanel();
				}
			}
		}

        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.TeamCreate.ToString(), CreateTeamHandle);
            RemoveEventHandler(EventTypeEnum.TeamList.ToString(), CreateTeamInfoItemsHandle);
            RemoveEventHandler(EventTypeEnum.TeamMemberLeave.ToString(), TeamMemberLeaveHandle);
            RemoveEventHandler(EventTypeEnum.TeamMemberBeKick.ToString(), TeamMemberBeKickHandle);
            RemoveEventHandler(EventTypeEnum.TeamDisband.ToString(), TeamDisbandHandle);
            RemoveEventHandler(EventTypeEnum.TeamMemberReady.ToString(), TeamMemberReadyHandle);
            RemoveEventHandler(EventTypeEnum.TeamUpdateProp.ToString(), UpdateTeamInfoHandle);
            //RemoveEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
            RemoveEventHandler(EventTypeEnum.TeamNoExist.ToString(), TeamNoExistHandle);
            RemoveEventHandler(EventTypeEnum.TeamFull.ToString(), TeamFullHandle);
            RemoveEventHandler(EventTypeEnum.TeamFighting.ToString(), TeamFightingHandle);
            RemoveEventHandler(EventTypeEnum.TeamExistMemberNoReady.ToString(), TeamExistMemberNoReadyHandle);
            RemoveEventHandler(EventTypeEnum.TeamMemberDevilInvite.ToString(), TeamMemberDevilInviteHandle);
			RemoveEventHandler(EventTypeEnum.TeamMemberUpdateProp.ToString(), PlayerActiveUpdateHanlde);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeLockError, TeamMemberEctypeLockErrorHandle);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NoEnoughActiveLife, RestoreTeamRoomPanelReadyButton);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NotEnoughGoldMoney, RestoreTeamRoomPanelReadyButton);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, OpenMainUIHandle);
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.TeamCreate.ToString(), CreateTeamHandle);
            AddEventHandler(EventTypeEnum.TeamList.ToString(), CreateTeamInfoItemsHandle);
            AddEventHandler(EventTypeEnum.TeamMemberLeave.ToString(), TeamMemberLeaveHandle);
            AddEventHandler(EventTypeEnum.TeamMemberBeKick.ToString(), TeamMemberBeKickHandle);
            AddEventHandler(EventTypeEnum.TeamDisband.ToString(), TeamDisbandHandle);
            AddEventHandler(EventTypeEnum.TeamMemberReady.ToString(), TeamMemberReadyHandle);                        
            AddEventHandler(EventTypeEnum.TeamUpdateProp.ToString(), UpdateTeamInfoHandle);
            //AddEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
            AddEventHandler(EventTypeEnum.TeamNoExist.ToString(), TeamNoExistHandle);
            AddEventHandler(EventTypeEnum.TeamFull.ToString(), TeamFullHandle);
            AddEventHandler(EventTypeEnum.TeamFighting.ToString(), TeamFightingHandle);
            AddEventHandler(EventTypeEnum.TeamExistMemberNoReady.ToString(), TeamExistMemberNoReadyHandle);
            //AddEventHandler(EventTypeEnum.EctypeNoQualification.ToString(), EctypeNoQualificationHandle);
            //AddEventHandler(EventTypeEnum.TeamActiveLifeNotEnough.ToString(), TeamActiveLifeNotEnoughHandle);
            AddEventHandler(EventTypeEnum.TeamMemberDevilInvite.ToString(), TeamMemberDevilInviteHandle);
			AddEventHandler(EventTypeEnum.TeamMemberUpdateProp.ToString(), PlayerActiveUpdateHanlde);
            TeamOrganizePanel.ItemPageManager_Team.OnPageChanged += ItemPageChangedHandle;
            UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeLockError, TeamMemberEctypeLockErrorHandle);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.NoEnoughActiveLife, RestoreTeamRoomPanelReadyButton);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.NotEnoughGoldMoney, RestoreTeamRoomPanelReadyButton);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, OpenMainUIHandle);
        }
    }
}
