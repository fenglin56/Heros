using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;
using System.Linq;
using UI.Friend;

namespace UI.Team
{
	public class TeamRoomPanel_V2 : MonoBehaviour
	{
		public Transform EctypeInfoBoard;
		private Vector3 m_EctypeIncoBoardInitialPos;

		public EctypeDropItemDesPanel EctypeDropItemDesPanel;
		public EctypeDesPanel EctypeDesPanel;

		public GameObject TeamCamaraRoot;
		private GameObject m_TeamMemberViewCamera;
		private ContainerHeroView[] m_MemberHeroViews;

		public GameObject Eff_Team_ChangeCool_Prefab;

		public Transform CaptainInterface;                      //队长界面
		public Transform MemberInterface;                       //队员界面
		
		public TeamInvitePanel_V2 TeamInvitePanel;                 //好友邀请界面

		public LocalButtonCallBack Button_Recruit;
		public LocalButtonCallBack Button_Chat;
		public GameObject Chat_NewMessage;	// 队伍频道新消息到达

		public LocalButtonCallBack CaptainDisbandButton;

		public LocalButtonCallBack CaptainStartButton;
		public LocalButtonCallBack PosOneInviteButton;

		
		public LocalButtonCallBack MemberLeaveButton;
		public LocalButtonCallBack MemberReadyButton;
		public LocalButtonCallBack MemberCancelReadyButton;

		public LocalButtonCallBack Button_HidePanel;//返回城镇

		public UILabel AreaTitleLabel;

		private bool IsShowCostLabel = false;
		
		private List<TeamRoomMemberItem> mTeamMemberList = new List<TeamRoomMemberItem>();
		
		private int mRoomMaxTeammate = 3;   //房间队友最大显示数
		
		private int m_kickMemberNo = 0;     //当前被提出队员序号
		
		private Color m_greyColor = new Color(0.1776f, 0.1776f, 0.1776f);        
		
		private string m_nameColorStartCode = "[7bb6c2]";
		private string m_nameColorEndCode = "[-]";
		
		private int[] m_guideBtnID = new int[11];        

		#region 修改

		//副本信息
		public UILabel Label_EctypeName;
		public UILabel Label_Hard;
		public UILabel Label_Level;
		public Transform EctypeIconPoint;
		public GameObject EctypeIcon;

		//public SpriteSwith Swith_ReadyTxt;	//1 = 准备  , 2 = 取消

		public SingleButtonCallBack Button_ViewEctype;
		public SingleButtonCallBack Button_ViewRewards;

		public UILabel Label_MyCombat;
		public UILabel Label_RecommendCombat;
		public SpriteSwith Swith_Consume;
		public UILabel Label_Consume;

		public Transform FirstMemberPoint;
		int m_MemberInfoSpacing = 204;
		public GameObject TeamRoomMemberItemPrefab;	

		public GameObject CaptainStartButtonEff;

		private List<TeamRoomMemberItem_V2> m_MemberList = new List<TeamRoomMemberItem_V2>();

		private int m_EctypeContainerID = 0;

		private List<SMsgTeamPropMember_SC> m_lastMembersList = new List<SMsgTeamPropMember_SC>();

		public GameObject ChatPanelTeamPrefab;
		private ChatPanel_Team m_ChatPanel_Team;

		#endregion


		void Awake()
		{

			m_EctypeIncoBoardInitialPos = EctypeInfoBoard.localPosition;
			//按钮事件
			CaptainDisbandButton.SetCallBackFuntion(OnCaptainDisbandClick, null);

			CaptainStartButton.SetCallBackFuntion(OnCaptainStartClick, null);
			MemberLeaveButton.SetCallBackFuntion(OnMemberLeaveClick, null);
			MemberReadyButton.SetCallBackFuntion(OnMemberReadyClick, null);
			MemberCancelReadyButton.SetCallBackFuntion(OnMemeberCancelReadyClick, null);
			PosOneInviteButton.SetCallBackFuntion(OnPosOneInviteClick, null);

			Button_Recruit.SetCallBackFuntion(OnRecruitButtonClick, null);
			Button_Chat.SetCallBackFuntion(OnChatButtonClick,null);

			Button_HidePanel.SetCallBackFuntion(OnHidePanelClick, null);

			Button_ViewEctype.SetPressCallBack(OnViewEctypeClick);
			Button_ViewRewards.SetPressCallBack(OnViewRewardsClick);

			TaskGuideBtnRegister();

			//队伍频道聊天窗
			//GameObject chatPanel = UI.CreatObjectToNGUI.InstantiateObj(ChatPanelTeamPrefab,transform);
			GameObject chatPanel = (GameObject)Instantiate(ChatPanelTeamPrefab);
			chatPanel.transform.parent = transform;
			chatPanel.transform.localPosition = Vector3.back * 1000;
			chatPanel.transform.localScale = Vector3.one;
			m_ChatPanel_Team = chatPanel.GetComponent<ChatPanel_Team>();
		}

		/// <summary>
		/// 引导按钮注入代码
		/// </summary>
		private void TaskGuideBtnRegister()
		{
			CaptainDisbandButton.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_JoinTeam_Back);
			Button_Chat.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_JoinTeam_Chat);		
			PosOneInviteButton.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_JoinTeam_Invite);	
			Button_Recruit.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_JoinTeam_QuickInvite);	
			CaptainStartButton.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_JoinTeam_GotoFight);	
		}

		void OnDestroy()
		{
			for (int i = 0; i < m_guideBtnID.Length; i++ )
			{
				//TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
			}
		}

		/// <summary>
		/// 重现隐藏的面板
		/// </summary>
		public void ReShowPanel()
		{
			SetTeamHeroView(true);
			this.ShowPanel();
			if(m_MemberHeroViews!= null)
			{
				m_MemberHeroViews.ApplyAllItem(p=>{
					if(p!=null && p.IsCreateObj())
					{
						p.PlayerIdleAnim();
					}
				});
			}
		}

		public void ShowPanel()
		{
			if(transform.localPosition != Vector3.zero)
			{
				SoundManager.Instance.PlaySoundEffect("Sound_UIEff_TeamRoomUIAppear");
				TweenPosition.Begin(EctypeInfoBoard.gameObject, 0.2f, m_EctypeIncoBoardInitialPos + Vector3.left * 200, m_EctypeIncoBoardInitialPos,
				                    MoveInAccomplishHandle);
			}
			//GameManager.Instance.isTeamBattleMark = false;
			transform.localPosition = Vector3.zero;
			UpdateAreaTitleLabel();
			Chat_NewMessage.SetActive(false);
			//UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseWorldChatWindow, null);//关闭聊天窗口
		}        
		
		public void ClosePanel()
		{
			GameManager.Instance.isTeamBattleMark = false;
			transform.localPosition = new Vector3(0, 0, -800);
			//MainUIController.Instance.
			SetTeamHeroView(false);
			DeleteHeroModels();//退出房间清除队员model
			m_ChatPanel_Team.Close();//关闭并清理队伍聊天信息
			m_lastMembersList.Clear();
		}
		//恢复准备按钮
		public void RestoreReadyButton()
		{
			MemberReadyButton.gameObject.SetActive(true);
			MemberCancelReadyButton.gameObject.SetActive(false);
		}
		//队长风格
		private void ShowCaptainInterface()
		{
			CaptainInterface.gameObject.SetActive(true);
			MemberInterface.gameObject.SetActive(false);
			Button_Recruit.gameObject.SetActive(true);
		}
		//队员风格
		private void ShowMemberInterface()
		{
			CaptainInterface.gameObject.SetActive(false);
			MemberInterface.gameObject.SetActive(true);
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
				//m_TeamMemberViewCamera.SetActive(isShow);
				var camera = m_TeamMemberViewCamera.GetComponentInChildren<Camera>();
				camera.enabled = isShow;
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
			if(m_MemberHeroViews!=null)
			{
				m_MemberHeroViews.ApplyAllItem(p=>{
					p.DeleteHeroModeView();
				});
			}
		}
		
		private void UpdateAreaTitleLabel()
		{
			//更新副本名称显示
			var teamProp = TeamManager.Instance.MyTeamProp;

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
			TeamManager.Instance.SetCurSelectEctypeContainerData(EctypeConfigManager.Instance.EctypeSelectConfigList[teamProp.TeamContext.dwEctypeId]);

			var playerData = PlayerManager.Instance.FindHeroDataModel();
			//var propMembers = teamProp.TeamMemberNum_SC.SMsgTeamPropMembers.Where(p => p.TeamMemberContext.dwActorID != playerData.ActorID).ToArray();
			var propMembers = teamProp.TeamMemberNum_SC.SMsgTeamPropMembers;
			int teammateNum = propMembers.Length;

			if(EctypeIcon != null)
			{
				Destroy(EctypeIcon);
			}
			int ectypeID = EctypeConfigManager.Instance.EctypeSelectConfigList[teamProp.TeamContext.dwEctypeId]._vectContainer[teamProp.TeamContext.dwEctypeIndex - 1];
			var containerIconData = TownEctypeResDataManager.Instance.GetEctypeContainerResData(ectypeID);
			EctypeIcon = UI.CreatObjectToNGUI.InstantiateObj(containerIconData.EctypeIconPrefab, EctypeIconPoint);
			EctypeIcon.transform.localPosition = Vector3.back *2;
			EctypeIcon.AddComponent<UIPanel>();

			//难度
			string hardStr = teamProp.TeamContext.byEctypeDifficulty == 0 ? "IDS_I13_10" : "IDS_I13_11";
			Label_Hard.text = LanguageTextManager.GetString(hardStr);

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
				m_TeamMemberViewCamera.transform.localPosition = new Vector3(0, 0, 10);//z为10避免看到town的label
				m_MemberHeroViews = m_TeamMemberViewCamera.GetComponentsInChildren<ContainerHeroView>();
			}

			//m_TeamMemberViewCamera.SetActive(true);

			if(MainUIController.Instance.CurrentUIStatus == UIType.TeamInfo)//隐藏面板时不显示队员
			{
				SetTeamHeroView(true);
			}

//			var camera = m_TeamMemberViewCamera.GetComponentInChildren<Camera>();
//			camera.enabled = true;
			
			//EctypeSelectConfigData ectypeData = null;

			//\
			//ectypeData = EctypeConfigManager.Instance.EctypeSelectConfigList[teamProp.TeamContext.dwEctypeId];
			//var ectypeID = ectypeData._vectContainer[teamProp.TeamContext.byEctypeDifficulty - 1];
			var ectypeInfo = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];


			AreaTitleLabel.text = LanguageTextManager.GetString(ectypeInfo.lEctypeName);//更新标题
			//等级
			Label_Level.text = ectypeInfo.lMinActorLevel.ToString()+LanguageTextManager.GetString("IDS_H1_156");

			m_MemberList.ApplyAllItem(p=>{
				if(p != null)
				{
					Destroy( p.gameObject);
				}
			});
			m_MemberList.Clear();

			int teamCombat = 0;
			//自己模型放在最前面
			List<SMsgTeamPropMember_SC> sortList = new List<SMsgTeamPropMember_SC>();
			for(int i=0;i<propMembers.Length;i++)
			{
				if(propMembers[i].TeamMemberContext.dwActorID == playerData.ActorID)
				{
					sortList.Insert(0,propMembers[i]);
				}
				else
				{
					sortList.Add(propMembers[i]);
				}
			}

			for(int i = 0;i<teammateNum;i++)
			{
				GameObject roomMemberItem = UI.CreatObjectToNGUI.InstantiateObj(TeamRoomMemberItemPrefab, FirstMemberPoint);
				TeamRoomMemberItem_V2 teamRoomMemberItem_V2 = roomMemberItem.GetComponent<TeamRoomMemberItem_V2>();
				teamRoomMemberItem_V2.transform.localPosition = Vector3.right * m_MemberInfoSpacing * i;
				bool isCaptaionPos = teamProp.TeamContext.dwCaptainId == sortList[i].TeamMemberContext.dwActorID;
				teamRoomMemberItem_V2.SetInfo(sortList[i].TeamMemberContext, ectypeInfo, isCaptain, isCaptaionPos);
				m_MemberList.Add(teamRoomMemberItem_V2);
				//显示hero model view
				m_MemberHeroViews[i].ShowHeroModelView((int)sortList[i].TeamMemberContext.dwActorID,sortList[i].TeamMemberContext.byKind, sortList[i].TeamMemberContext.nFashionID
				                                       , sortList[i].TeamMemberContext.nCurWeaponID);

				if(m_lastMembersList.Count > 0)
				{
					if(!m_lastMembersList.Any(p=>p.TeamMemberContext.dwActorID == sortList[i].TeamMemberContext.dwActorID))
					{
						m_MemberHeroViews[i].PlayJoinInAnimation();
					}
				}
				else
				{
					m_MemberHeroViews[0].PlayJoinInAnimation();
				}


				teamCombat += sortList[i].TeamMemberContext.fightNum;
			}

			//队伍功力
			Label_MyCombat.text = teamCombat.ToString();
			Label_RecommendCombat.text = ectypeInfo.FightingCapacity.ToString();
			//消耗
			Swith_Consume.ChangeSprite(ectypeInfo.lCostType);//消耗类型
			Label_Consume.text = ectypeInfo.lCostEnergy.ToString(); 
			
			//如果是队长，更新邀请请离按钮
			if (isCaptain)
			{
				ResetButtons();

			}
			else
			{
				
				//自身是队员 判断准备按钮是否置灰
				var myTeamData = teamProp.TeamMemberNum_SC.SMsgTeamPropMembers.SingleOrDefault(p => p.TeamMemberContext.dwActorID == playerData.ActorID);
//				if (myTeamData.TeamMemberContext.byFightReady == 1)
//				{
////					MemberReadyButton.SetButtonStatus(true);
////					MemberReadyButton.SetButtonTextColor(m_greyColor);
////					MemberReadyButton.animation.Stop();
//					var uisprites = MemberReadyButton.GetComponentsInChildren<UISprite>();
//					uisprites.ApplyAllItem(p=>p.alpha = 0.5f);
//					MemberReadyButton.SetBoxCollider(false);
//				}
				bool isReady = myTeamData.TeamMemberContext.byFightReady  == 1;
				MemberReadyButton.gameObject.SetActive(!isReady);
				MemberCancelReadyButton.gameObject.SetActive(isReady);
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
				CaptainStartButtonEff.SetActive(true);
				//CaptainStartButton.animation.Play();
			}
			else
			{
				CaptainStartButtonEff.SetActive(false);
				//CaptainStartButton.animation.Stop();
			}
			
			//队长如果是邀请界面
			if (TeamInvitePanel.IsShow())
			{
				TeamInvitePanel.FilterFriendList();
			}

			//记录当前的队员信息
			m_lastMembersList.Clear();
			m_lastMembersList.AddRange(propMembers);
		}
		
		public void UpdateTeamMemberReadyState(uint dwActorId, int readyResult)
		{
			        
			if (PlayerManager.Instance.FindHeroDataModel().ActorID == dwActorId)
			{
				//队员准备按钮置灰
				bool isReady = readyResult == 1;
				MemberReadyButton.gameObject.SetActive(!isReady);
				MemberCancelReadyButton.gameObject.SetActive(isReady);
				ShowCostLabelInButton();
				//return;
			}
			
			
			TeamManager.Instance.SetTeamMemberReadyStatu(dwActorId, readyResult);
			
			var teamMember = m_MemberList.SingleOrDefault(p => p.TeamMemberContext.dwActorID == dwActorId);
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
				CaptainStartButtonEff.SetActive(true);
				//CaptainStartButton.animation.Play();
			}
			else
			{
				CaptainStartButtonEff.SetActive(false);
				//CaptainStartButton.animation.Stop();
			}
			
		}

		public void UpdateMemberHeroView(int actorID)
		{
			SMsgTeamPropMember_SC memberProp = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.SingleOrDefault(p=>p.TeamMemberContext.dwActorID == actorID);
			if(memberProp.TeamMemberContext.dwActorID != 0)
			{
				var heroView = m_MemberHeroViews.SingleOrDefault(p=>p.ActorID == actorID);
				heroView.ShowHeroModelView((int)memberProp.TeamMemberContext.dwActorID,memberProp.TeamMemberContext.byKind,
				                           memberProp.TeamMemberContext.nFashionID, memberProp.TeamMemberContext.nCurWeaponID);
			}
		}

		/// <summary>
		/// 显示新消息提示
		/// </summary>
		public void ShowNewTeamMessage()
		{
			Chat_NewMessage.SetActive(true);
		}	

		//封魔副本邀请
		public void ShowEctypepanelMessageBox(Vector3 messageBoxPos)
		{
		}
		string notEnoughtCostStr = "";
		void NotEnoughCostEnergy(object obj)
		{
			MessageBox.Instance.ShowTips(4, notEnoughtCostStr, 1f);
		}
		void EnoughCostEnergy(object obj)
		{
			NetServiceManager.Instance.TeamService.SureJoinTargetTeam();
		}

		void MoveInAccomplishHandle(object obj)
		{
		}

		//购买活力按钮
		void OnAddEnergyClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
			MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_166"),
			                         LanguageTextManager.GetString("IDS_H2_55"), LanguageTextManager.GetString("IDS_H2_28"), BuyActivelife, null);
		}
		//队长解散按钮
		void OnCaptainDisbandClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamRoomBack");

			MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_I13_43"),
			                         LanguageTextManager.GetString("IDS_H2_28"),  LanguageTextManager.GetString("IDS_H2_19"), CancelDisbandTeam, DisbandTeam);
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
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamStart");
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

			//this.SpecialEctypeStart(null);
			var teamProp = TeamManager.Instance.MyTeamProp;
			int ectypeID = EctypeConfigManager.Instance.EctypeSelectConfigList[teamProp.TeamContext.dwEctypeId]._vectContainer[teamProp.TeamContext.dwEctypeIndex - 1];
			EctypeContainerData SelectContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
			if(SelectContainerData.lEctypeType == 9 && myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.Length<=1)
			{
				MessageBox.Instance.Show(4,"",LanguageTextManager.GetString("IDS_I19_32").Replace(@"\n", "\n"),LanguageTextManager.GetString("IDS_I19_11"),LanguageTextManager.GetString("IDS_I19_12"),CancelCrusade ,ShowCostLabelInButton);
			}
			else
			{
				ShowCostLabelInButton();
			}
		}

		void CancelCrusade()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_GetMatchCancel");
		}

		private void ShowCostLabelInButton()
		{
			if (IsShowCostLabel)
				return;
			//var currentEctype = TeamManager.Instance.CurrentEctypeData;
			//Todo：onSelectEctypeData协议已经取消dwEctypeID和byDiff难度，如果使用需要从新更改
			//            var ectypeSelect = EctypeConfigManager.Instance.EctypeSelectConfigList[(int)currentEctype.dwEctypeID];
			//            var ectypeID = ectypeSelect._vectContainer[currentEctype.byDiff - 1];
			var teamProp = TeamManager.Instance.MyTeamProp;
			int ectypeID = EctypeConfigManager.Instance.EctypeSelectConfigList[teamProp.TeamContext.dwEctypeId]._vectContainer[teamProp.TeamContext.dwEctypeIndex - 1];
			EctypeContainerData SelectContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
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
			
			//回调
			//var teamProp = TeamManager.Instance.MyTeamProp;
			TeamManager.Instance.RegisteTeam(teamProp);            
			//队长
			var playerData = PlayerManager.Instance.FindHeroDataModel();
			if (teamProp.TeamContext.dwCaptainId == playerData.ActorID)
			{
				if (isShowCostType)
				{
					SoundManager.Instance.PlaySoundEffect("Sound_UIEff_TeamSatrtBattle");
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
					SoundManager.Instance.PlaySoundEffect("Sound_UIEff_TeamSatrtBattle");
//					GameObject Tips = CreatObjectToNGUI.InstantiateObj(CostLabelPreafab, CostLabelPreafab.transform.parent);
//					Vector3 curPoint = CostLabelPreafab.transform.localPosition;
//					Vector3 fromPoint = curPoint + new Vector3(0, 30, -30);
//					Vector3 toPoint = curPoint + new Vector3(0, 0, -30);
//					TweenPosition.Begin(Tips, 0.5f, fromPoint, toPoint, null);
//					TweenAlpha.Begin(Tips, 0.5f, 1, 0, null);
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
			//yield return new WaitForSeconds(2f);
			yield return new WaitForEndOfFrame();
			SpecialEctypeStart(null);
		}
		
		void SpecialEctypeStart(object obj)
		{            
			var teamData = TeamManager.Instance.MyTeamProp;
			int ectypeID = EctypeConfigManager.Instance.EctypeSelectConfigList[teamData.TeamContext.dwEctypeId]._vectContainer[teamData.TeamContext.dwEctypeIndex - 1];
			SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS()
			{
				dwEctypeContainerID  = ectypeID,
//				                uidEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity,
//				                dwEctypeId = (int)teamData.TeamContext.dwEctypeId,
//				                byDifficulty = (byte)teamData.TeamContext.byEctypeDifficulty
			};
			NetServiceManager.Instance.EctypeService.SendEctypeRequest(sMSGEctypeRequestCreate_CS);
			LoadingUI.Instance.Show();
			GameManager.Instance.isTeamBattleMark = true;
//			Debug.Log("IsTeamExist()=="+TeamManager.Instance.IsTeamExist()+" isTeamBattleMark="+GameManager.Instance.isTeamBattleMark);
		}
		//位置一邀请按钮
		void OnPosOneInviteClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamInvitation");
			InviteMember();
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
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamReady");

			var heroDataModel = PlayerManager.Instance.FindHeroDataModel();   
			var teamData = TeamManager.Instance.MyTeamProp;
			GameManager.Instance.isTeamBattleMark = true;
//			Debug.Log("IsTeamExist()=="+TeamManager.Instance.IsTeamExist()+" isTeamBattleMark="+GameManager.Instance.isTeamBattleMark);
			NetServiceManager.Instance.TeamService.SendTeamMemberReadyMsg(new SMsgTeamMemberReady_CS()
			                                                              {
				dwActorID = (uint)heroDataModel.ActorID,
				dwTeamID = teamData.TeamContext.dwId,
			});           
		}
		//队员取消准备
		void OnMemeberCancelReadyClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamReadyCancel");
			GameManager.Instance.isTeamBattleMark = false;
			var heroDataModel = PlayerManager.Instance.FindHeroDataModel();   
			var teamData = TeamManager.Instance.MyTeamProp;
			
			NetServiceManager.Instance.TeamService.SendTeamMemberReadyMsg(new SMsgTeamMemberReady_CS()
			                                                              {
				dwActorID = (uint)heroDataModel.ActorID,
				dwTeamID = teamData.TeamContext.dwId,
			});     
		}
		
		void OnRecruitButtonClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamRecruit");
			GameObject eff = UI.CreatObjectToNGUI.InstantiateObj(Eff_Team_ChangeCool_Prefab, Button_Recruit.transform);
			eff.transform.localPosition += new Vector3(-29.8f,4.9f,-10f);

			var playerData = PlayerManager.Instance.FindHeroDataModel();
			string chat = string.Format(LanguageTextManager.GetString("IDS_I13_45"), AreaTitleLabel.text , Label_Hard.text);
			NetServiceManager.Instance.ChatService.SendChat((uint)playerData.ActorID, 0, chat, 1, Chat.ChatDefine.MSG_CHAT_CURRENT);
		}

		void OnChatButtonClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Chat");
			Chat_NewMessage.SetActive(false);
			//MainUIController.Instance.OpenMainUI(UIType.Chat, Chat.WindowType.Team);
			m_ChatPanel_Team.Show();
		}

		void OnHidePanelClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamRoomBack");
			MainUIController.Instance.CloseAllPanel();
		}

		void OnViewEctypeClick(bool isPressed)
		{
			if(isPressed)
			{
				var teamProp = TeamManager.Instance.MyTeamProp;
				int ectypeID = EctypeConfigManager.Instance.EctypeSelectConfigList[teamProp.TeamContext.dwEctypeId]._vectContainer[teamProp.TeamContext.dwEctypeIndex - 1];
				var ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
				EctypeDesPanel.TweenShow(ectypeContainerData);
			}
			else
			{
				EctypeDesPanel.TweenClose();
			}
		}
		void OnViewRewardsClick(bool isPressed)
		{
			if(isPressed)
			{
				var teamProp = TeamManager.Instance.MyTeamProp;
				int ectypeID = EctypeConfigManager.Instance.EctypeSelectConfigList[teamProp.TeamContext.dwEctypeId]._vectContainer[teamProp.TeamContext.dwEctypeIndex - 1];
				var ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
				EctypeDropItemDesPanel.TweenShow(ectypeContainerData);
			}
			else
			{
				EctypeDropItemDesPanel.TweenClose();
			}
		}
		
		private void ResetButtons()
		{

		}
		//更新活力值显示
		private void UpdateActiveLife()
		{
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
		//取消解散队伍
		void CancelDisbandTeam()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamDissolutionCancel");
		}
		//确定解散队伍
		void DisbandTeam()
		{
			//transform.parent.transform.localPosition = new Vector3(0, 0, -800);
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamDissolutionConfirmation");

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
		//邀请队员ect
		void InviteMember()
		{
			//UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseWorldChatWindow,null);//如果聊天窗口打开，关闭
			if(m_ChatPanel_Team.IsShow())
			{
				m_ChatPanel_Team.Hide();
			}
			TeamInvitePanel.Show();
		}
		
		//活力不足窗口
		private void ShowActiveLifeNoEnoughWindow()
		{
			
		}
	}
}