using UnityEngine;
using System.Collections;
using UI.Team;
using System.Linq;
using System.Collections.Generic;
using Chat;

namespace UI.MainUI
{

	public class TeamPanel_V2 : BaseUIPanel
	{
		public TeamListPanel_V2 TeamListPanel;
		public SelectEctypePanel_V2 SelectEctypePanel;
		public SelectAreaPanel_V2 SelectAreaPanel;

		public TeamRoomPanel_V2 TeamRoomPanel;

		public GameObject TitleBar_Team;
		public GameObject TitleBar_Crusade;

		public Transform CommonPanelTitlePoint;
		public GameObject CommonPanelTitlePrefab;
		private BaseCommonPanelTitle m_CommonPanelTitle;

		// 队员状态
		public enum TEAM_MEMBER_STATUS
		{
			TEAM_MEMBER_NONE_STATUS = 0,	// 无状态, 默认
			TEAM_MEMBER_READY_STATUS = 1,		// 准备状态
		};
		
		
		void Awake()
		{
			transform.localPosition = new Vector3(0, 0, -800);  //位置先置前,避免队伍界面弹封魔副本邀请

			GameObject commonTitle = UI.CreatObjectToNGUI.InstantiateObj(CommonPanelTitlePrefab,CommonPanelTitlePoint);
			m_CommonPanelTitle = commonTitle.GetComponent<BaseCommonPanelTitle>();
			m_CommonPanelTitle.HidePos = new Vector3(100,0,0);
			m_CommonPanelTitle.ShowPos = Vector3.zero;
			m_CommonPanelTitle.Init(CommonTitleType.Power, CommonTitleType.GoldIngot);

			RegisterEventHandler();

			TaskGuideBtnRegister();


		}

		/// <summary>
		/// 引导按钮注入代码
		/// </summary>
		private void TaskGuideBtnRegister()
		{
//			Button_AddIngot.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_BuyIngot);
//			Button_AddActive.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_BuyActivity);		
		}


		/// <summary>
		/// 0 或 不填   =   普通副本列表
		/// 1          =   普通副本房间
		/// 2          =   首领讨伐列表
		/// 3          =   首领讨伐房间
		/// </summary>
		/// <param name="value">Value.</param>
		public override void Show(params object[] value)
		{
			if(value.Length > 0)
			{
				switch((int)value[0])
				{
				case 0:
					TeamRoomPanel.ClosePanel();
					TeamListPanel.ShowPanel();
					break;
				case 1:
					TeamListPanel.ClosePanel();
					TeamRoomPanel.ReShowPanel();
					break;
				case 2:break;
				case 3:break;
				}
			}
			else
			{
				if(TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.wMemberNum > 0)
				{
					TeamRoomPanel.ShowPanel();
				}
				else
				{
					TeamListPanel.ShowPanel();
				}

			}
			//transform.localPosition = Vector3.zero;
			bool isNormalType = TeamManager.Instance.CurSelectEctypeAreaData.lEctypeType != 9;
			TitleBar_Team.SetActive(isNormalType);
			TitleBar_Crusade.SetActive(!isNormalType);
			
			TeamListPanel.LastPanel = (TeamListPanel_V2.PANEL_TYPE)(isNormalType == true ? 0 : 1);

			var playerData = PlayerManager.Instance.FindHeroDataModel();
//			Label_Ingot.text = playerData.PlayerValues.PLAYER_FIELD_BINDPAY.ToString();
//			Label_Active.text = playerData.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE.ToString();

			m_CommonPanelTitle.TweenShow();

			base.Show(value);
		}

		public override void Close()
		{
			if (!IsShow)
				return;

			if(MainUIController.Instance.NextUIStatus == UIType.Chat)
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
			//UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseWorldChatWindow,null);
			TeamRoomPanel.ShowPanel();
			TeamListPanel.ClosePanel();
			LoadingUI.Instance.Close();
		}
		
		private void ShowWorldTeamInfo()
		{
			//UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseWorldChatWindow,null);
			ChatRecordManager.Instance.ClearPublicChatRecord( Chat.WindowType.Team);
			TeamListPanel.ShowPanel();
			TeamRoomPanel.ClosePanel();
			LoadingUI.Instance.Close();
		}

		void OnAddIngotClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");            
			UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UIType.TopUp);
		}


		//刷新队伍
		private void RefreshTeamList()
		{
			TeamListPanel.OnRefreshWorldTeamInfoClick(0);
		}
		
		void CreateTeamHandle(INotifyArgs e)
		{
			SMsgTeamNum_SC sMsgTeamNum = (SMsgTeamNum_SC)e;

			//设置标题
			var ectypeData = EctypeConfigManager.Instance.EctypeSelectConfigList[ sMsgTeamNum.SMsgTeamProps[0].TeamContext.dwEctypeId];
			bool isNormalType = ectypeData.lEctypeType != 9;
			TitleBar_Team.SetActive(isNormalType);
			TitleBar_Crusade.SetActive(!isNormalType);
			
			TeamListPanel.LastPanel = (TeamListPanel_V2.PANEL_TYPE)(isNormalType == true ? 0 : 1);
			ShowMyTeamInfo();

			StartCoroutine(CreateTeamLater(sMsgTeamNum));
		}
		//为了队员摄像机创建安全
		IEnumerator CreateTeamLater(SMsgTeamNum_SC sMsgTeamNum)
		{
			yield return new WaitForEndOfFrame();
			bool isInTeamRoomPanel = TeamRoomPanel.IsInTeamRoomPanel();

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
			TeamListPanel.CreateTeamInfoItems(sMsgTeamNum);
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
					TeamListPanel.OnRefreshWorldTeamInfoClick(0);
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
			TeamListPanel.OnRefreshWorldTeamInfoClick(0);
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
			TeamListPanel.OnRefreshWorldTeamInfoClick(0);
			MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_160"),
			                         LanguageTextManager.GetString("IDS_H2_28"), LanguageTextManager.GetString("IDS_H2_4"),null , SureCreateTeam);
		}
		//队伍不存在 队伍已解散
		void TeamNoExistHandle(INotifyArgs e)
		{
			//MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_82"), LanguageTextManager.GetString("IDS_H2_55"), RefreshTeamList);
			//this.RefreshTeamList();
			TeamListPanel.Grid.Reposition();
		}
		//队伍人数已满
		void TeamFullHandle(INotifyArgs e)
		{
			//MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_81"), LanguageTextManager.GetString("IDS_H2_55"), RefreshTeamList);            
			//this.RefreshTeamList();
			TeamListPanel.Grid.Reposition();
		}
		//对方队伍正在副本战斗
		void TeamFightingHandle(INotifyArgs e)
		{
			//MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_83"), LanguageTextManager.GetString("IDS_H2_55"));            
			TeamListPanel.Grid.Reposition();
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
			//NetServiceManager.Instance.TeamService.SendTeamCreateMsg();
			var currentEctypeArea = TeamManager.Instance.CurSelectEctypeAreaData;
			if(currentEctypeArea.lEctypeType == 0)
			{
				ShowChildPanelHandle(ChildPanel.SelectEctype);
			}
			else if(currentEctypeArea.lEctypeType == 9)
			{
				NetServiceManager.Instance.TeamService.SendTeamCreateMsg(currentEctypeArea._lEctypeID, 1, 0);
				if(transform.localPosition != Vector3.zero)
				{
					MainUIController.Instance.OpenMainUI(UIType.TeamInfo,1,0);
				}
			}
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
			SMsgTeamMemberUpdateProp_SC updateProp = (SMsgTeamMemberUpdateProp_SC)arg;
			if(transform.localPosition == Vector3.zero)
			{
				UpdateTeamInfoHandle(null);
			}
			TeamRoomPanel.UpdateMemberHeroView((int)updateProp.dwActorID);
		}

//		void UpdateViaNotify(INotifyArgs arg)
//		{
//			var playerData = PlayerManager.Instance.FindHeroDataModel();
//			Label_Ingot.text = playerData.PlayerValues.PLAYER_FIELD_BINDPAY.ToString();
//			Label_Active.text = playerData.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE.ToString();
//		}

		//翻页消息监听
		void ItemPageChangedHandle(PageChangedEventArg pageSmg)
		{             
			TeamListPanel.ItemPageChanged(pageSmg);
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

		void ShowChildPanelHandle(object obj)
		{
			ChildPanel cp = (ChildPanel)obj;
			switch(cp)
			{
			case ChildPanel.SelectAreaForFilter:
				SelectAreaPanel.Show(SelectAreaPanel_V2.PanelType.Filter);
				break;
			case ChildPanel.SelectAreaForCreate:
				SelectAreaPanel.Show(SelectAreaPanel_V2.PanelType.Create);
				break;
			case ChildPanel.SelectEctype:
				List<EctypeContainerData> list = new List<EctypeContainerData>();

				//var EctypeSelectData = EctypeConfigManager.Instance.EctypeSelectConfigList[SelectAreaPanel.EctypeID];
				var EctypeSelectData = TeamManager.Instance.CurSelectEctypeAreaData;

				//普通副本
				if(EctypeSelectData.lEctypeType == 0)
				{
					EctypeSelectData._vectContainer.ApplyAllItem(p=>{
						if(EctypeModel.Instance.sMSGEctypeSelect_SC.sMSGEctypeData_SCs.Any(k=>k.dwEctypeContaienrID == p))
						{
							if(EctypeConfigManager.Instance.EctypeContainerConfigList[p].AllowCreatTeam == 1)
							{
								list.Add( EctypeConfigManager.Instance.EctypeContainerConfigList[p]);
							}
						}
					});
					SelectEctypePanel.Show(EctypeSelectData,list.ToArray());
				}
				//讨伐副本
//				if(EctypeSelectData.lEctypeType == 9)
//				{
//					var playerData = PlayerManager.Instance.FindHeroDataModel();
//					EctypeSelectData._vectContainer.ApplyAllItem(p=>{
//						if(EctypeConfigManager.Instance.EctypeContainerConfigList[p].lMinActorLevel <= 
//							playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL)
//						{
//							list.Add( EctypeConfigManager.Instance.EctypeContainerConfigList[p]);
//						}
//					});
//				}

				break;
			case ChildPanel.FriendInvite:

				break;
			case ChildPanel.RefreshTeamList:
				TeamListPanel.CreateRefreshCoolEff();
				TeamListPanel.Grid.Reposition();
				TeamListPanel.UpdateAreaTitleLabel();
				break;
			case ChildPanel.RepositionTeamList:
				TeamListPanel.UpdateNoneTeamTip();
				TeamListPanel.RepositionList();
				break;
			case ChildPanel.UpdateAreaInfo:
				TeamListPanel.UpdateAreaTitleLabel();
				break;
			case ChildPanel.AddActive:
				//this.OnAddActiveClick(null);
				break;
			}
		}

		void ShowNewTeamMessageHandle(object obj)
		{
			TeamRoomPanel.ShowNewTeamMessage();
		}
		void JudgeShowNewTeamMessageHandle(object obj)
		{
			SMsgChat_SC sMsgChat_SC = (SMsgChat_SC)obj;
			if((WindowType)sMsgChat_SC.L_Channel == WindowType.Team)
			{
				if(MainUIController.Instance.GetPanel(UIType.Chat)==null)
				{
					TeamRoomPanel.ShowNewTeamMessage();
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
			RemoveEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
			RemoveEventHandler(EventTypeEnum.TeamNoExist.ToString(), TeamNoExistHandle);
			RemoveEventHandler(EventTypeEnum.TeamFull.ToString(), TeamFullHandle);
			RemoveEventHandler(EventTypeEnum.TeamFighting.ToString(), TeamFightingHandle);
			RemoveEventHandler(EventTypeEnum.TeamExistMemberNoReady.ToString(), TeamExistMemberNoReadyHandle);
			RemoveEventHandler(EventTypeEnum.TeamMemberDevilInvite.ToString(), TeamMemberDevilInviteHandle);
			RemoveEventHandler(EventTypeEnum.TeamMemberUpdateProp.ToString(), PlayerActiveUpdateHanlde);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeLockError, TeamMemberEctypeLockErrorHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NoEnoughActiveLife, RestoreTeamRoomPanelReadyButton);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NotEnoughGoldMoney, RestoreTeamRoomPanelReadyButton);
			//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, OpenMainUIHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShowTeamChildPanel, ShowChildPanelHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NewTeamMessage, ShowNewTeamMessageHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.WorldChatMsg, JudgeShowNewTeamMessageHandle);
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
			AddEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
			AddEventHandler(EventTypeEnum.TeamNoExist.ToString(), TeamNoExistHandle);
			AddEventHandler(EventTypeEnum.TeamFull.ToString(), TeamFullHandle);
			AddEventHandler(EventTypeEnum.TeamFighting.ToString(), TeamFightingHandle);
			AddEventHandler(EventTypeEnum.TeamExistMemberNoReady.ToString(), TeamExistMemberNoReadyHandle);
			//AddEventHandler(EventTypeEnum.EctypeNoQualification.ToString(), EctypeNoQualificationHandle);
			//AddEventHandler(EventTypeEnum.TeamActiveLifeNotEnough.ToString(), TeamActiveLifeNotEnoughHandle);
			AddEventHandler(EventTypeEnum.TeamMemberDevilInvite.ToString(), TeamMemberDevilInviteHandle);
			AddEventHandler(EventTypeEnum.TeamMemberUpdateProp.ToString(), PlayerActiveUpdateHanlde);
			//AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);

			//TeamListPanel.ItemPageManager_Team.OnPageChanged += ItemPageChangedHandle;
			UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeLockError, TeamMemberEctypeLockErrorHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.NoEnoughActiveLife, RestoreTeamRoomPanelReadyButton);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.NotEnoughGoldMoney, RestoreTeamRoomPanelReadyButton);
			//UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, OpenMainUIHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ShowTeamChildPanel, ShowChildPanelHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.NewTeamMessage,ShowNewTeamMessageHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.WorldChatMsg, JudgeShowNewTeamMessageHandle);
		}
	}
}

namespace UI.Team
{
	public enum ChildPanel
	{
		SelectAreaForFilter = 0,				//筛选更换区域界面
		SelectAreaForCreate,					//创建副本更换区域界面 
		SelectEctype,							//选择副本界面
		FriendInvite,							//好友邀请
		RefreshTeamList,						//刷新队伍列表
		RepositionTeamList,						//重新排列队伍列表
		UpdateAreaInfo,							//更新区域信息
		AddActive,								//购买活力
	}

	public class SpriteName
	{
		public const string PROFESSION_ICON = "JH_UI_Icon_0103_0";
		public const string PROFESSION_CHAR = "JH_UI_Typeface_1211_0";
	}
}
