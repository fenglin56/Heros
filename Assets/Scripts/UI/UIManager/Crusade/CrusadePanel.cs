using UnityEngine;
using System.Collections;
using UI.Crusade;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI.MainUI
{
	public class CrusadePanel : BaseUIPanel 
	{
		public LocalButtonCallBack Button_Exit;

		public UIPanel LeaderListPanel;
		public UIPanel CrusadeInfoPanel;

		public GameObject CrusadeLeaderItemPrefab;
		public UIDraggablePanel DraggablePanel;
		public UIGrid Grid;

		public GameObject SelectFrame;

		//need crusade item
		public UISprite UI_Qualifications;
		public UILabel Label_QualificationsNum;
		public GameObject NeedCrusadeItemInterface;

		//dont need crusade item
		public SingleButtonCallBack Button_ViewDrops;
		public GameObject DontNeedCrusadeItemInterface;
		public UILabel Label_FreeTime;

		public GameObject SingleInterface;
		public GameObject TeamInterface;
		public UILabel Label_CrusadeTime;

		public EctypeDropItemDesPanel EctypeDropItemDesPanel;

		public LocalButtonCallBack Button_Start;
		public LocalButtonCallBack Button_CreateTeam;
		public LocalButtonCallBack Button_FindTeam;
		public LocalButtonCallBack Button_QuickJoin;

		public LocalButtonCallBack Button_Receive;
		public LocalButtonCallBack Button_RandomEctype;
		public LocalButtonCallBack Button_CancelRandomEctype;

		public UISprite UI_FirstRewardIcon;
		public GameObject HaveBeenIssued;

		public Transform CommonPanelTitlePoint;
		public GameObject CommonPanelTitlePrefab;
		private BaseCommonPanelTitle m_CommonPanelTitle;

		public GameObject CrusadeMatchingPanelPrefab;
		private CrusadeMatchingPanel MatchingPanel;

		public GameObject Label_MatchExplain;
		public GameObject Eff_AutoMatching;

        public SingleButtonCallBack RewardItemButton;
        private int CurrentRewardId;
		private int m_CurEctypeID = 0;
		private Dictionary<int, CrusadeLeaderItem> m_ectypeItemList = new  Dictionary<int, CrusadeLeaderItem>();

		private List<EctypeSelectConfigData> m_CrusadeList = new List<EctypeSelectConfigData>();//讨伐副本容器

		private Vector3 LeaderListPanelPos;
		private Vector3 CrusadeInfoPanelPos;

		void Awake()
		{
			var playerData = PlayerManager.Instance.FindHeroDataModel();
            RewardItemButton.SetCallBackFuntion(OnRewardItemClick);
			m_CrusadeList = EctypeConfigManager.Instance.EctypeSelectConfigList.Values.Where(p=>p.lEctypeType == 9).ToList();//首领讨伐副本 lEctypeType = 9
			bool isCreate = true;
			int index = 0;
			m_CrusadeList.ApplyAllItem(p=>{
				p._vectContainer.ApplyAllItem(k=>{
					if(isCreate)
					{
						index++;
						GameObject ectype = UI.CreatObjectToNGUI.InstantiateObj(CrusadeLeaderItemPrefab, Grid.transform);
						EctypeContainerData ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[k];
						CrusadeLeaderItem crusadeLeaderItem = ectype.GetComponent<CrusadeLeaderItem>();
						crusadeLeaderItem.Init(p._EctypeIconPrefab, ectypeContainerData,playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL,OnSelectEctypeClick,index);
						ectype.RegisterBtnMappingId(p._lEctypeID,UIType.Crusade,BtnMapId_Sub.Crusade_EctypeItem);
						m_ectypeItemList.Add(k,crusadeLeaderItem);
						if(ectypeContainerData.lMinActorLevel > playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL)
						{
							isCreate = false;
						}
					}
				});
			});
			Grid.Reposition();

			GameObject commonTitle = UI.CreatObjectToNGUI.InstantiateObj(CommonPanelTitlePrefab,CommonPanelTitlePoint);
			m_CommonPanelTitle = commonTitle.GetComponent<BaseCommonPanelTitle>();
			m_CommonPanelTitle.HidePos = new Vector3(100,0,0);
			m_CommonPanelTitle.ShowPos = Vector3.zero;
			m_CommonPanelTitle.Init(CommonTitleType.Power , CommonTitleType.GoldIngot);

			Button_Exit.SetCallBackFuntion(OnExitClick,null);
			Button_ViewDrops.SetPressCallBack(OnViewRewardsClick);
			Button_Start.SetCallBackFuntion(OnStartClick,null);
			Button_CreateTeam.SetCallBackFuntion(OnCreateTeamClick, null);
			Button_FindTeam.SetCallBackFuntion(OnFindTeamClick,null);		
			Button_QuickJoin.SetCallBackFuntion(OnQuickJoinClick,null);

			Button_Receive.SetCallBackFuntion(OnReceiveFirstRewardClick,null);
			Button_RandomEctype.SetCallBackFuntion(OnRandomEctypeClick, null);
			Button_CancelRandomEctype.SetCallBackFuntion(OnCancelRandomEctypeClick,null);
			Button_CancelRandomEctype.gameObject.SetActive(false);
		

			LeaderListPanelPos = LeaderListPanel.transform.localPosition;
			CrusadeInfoPanelPos = CrusadeInfoPanel.transform.localPosition;

			TaskGuideBtnRegister();

			RegisterEventHandler();
		}

        void OnRewardItemClick(object obj)
        {
            if(CurrentRewardId!=0)
            {
                UI.MainUI.ItemInfoTipsManager.Instance.Show(CurrentRewardId);
            }
        }
		/// <summary>
		/// 引导按钮注入代码
		/// </summary>
		private void TaskGuideBtnRegister()
		{
			Button_Exit.gameObject.RegisterBtnMappingId(UIType.Crusade, BtnMapId_Sub.Crusade_Back);		
			Button_FindTeam.gameObject.RegisterBtnMappingId(UIType.Crusade, BtnMapId_Sub.Crusade_FollowCrusade);
			Button_CreateTeam.gameObject.RegisterBtnMappingId(UIType.Crusade, BtnMapId_Sub.Crusade_GotoCrusade);
			Button_ViewDrops.gameObject.RegisterBtnMappingId(UIType.Crusade,BtnMapId_Sub.Crusade_EctypeDropOut);
			Button_QuickJoin.gameObject.RegisterBtnMappingId(UIType.Crusade,BtnMapId_Sub.Crusade_QuickJoin);
			//Button_ViewDrops_NeedCrusadeItem.gameObject.RegisterBtnMappingId(UIType.Crusade, BtnMapId_Sub.Crusade_EctypeDropOut);
			//jamfing
			Button_RandomEctype.gameObject.RegisterBtnMappingId(UIType.Crusade,BtnMapId_Sub.Crusade_RandomEctype);
		}

		public override void Show(params object[] value)
		{
			SelectFrame.SetActive(m_CurEctypeID != 0);

			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_CoopUIAppear");

			UpdateEctypeList();

			if(TeamManager.Instance.IsTeamExist())
			{
				if(TeamManager.Instance.GetCurrentEctypeType() == 9)
				{
					UI.MainUI.MainUIController.Instance.OpenMainUI( UIType.TeamInfo,1);
					return;
				}
			}

			//var playerData = PlayerManager.Instance.FindHeroDataModel();

			LeaderListPanel.alpha = 0;
			TweenPosition.Begin(LeaderListPanel.gameObject,0.1f,LeaderListPanelPos+Vector3.left*100,LeaderListPanelPos);
			TweenAlpha.Begin(LeaderListPanel.gameObject,0.1f,1);
			CrusadeInfoPanel.alpha = 0;
			TweenPosition.Begin(CrusadeInfoPanel.gameObject,0.1f,CrusadeInfoPanelPos+Vector3.right*80,CrusadeInfoPanelPos);
			TweenAlpha.Begin(CrusadeInfoPanel.gameObject,0.1f,1);

			m_CommonPanelTitle.TweenShow();

			//请求获取队伍数量
			NetServiceManager.Instance.TeamService.SendGetCrusadeTeamNumsMsg();

			//首战奖励
			UpdateFirstCrusadeRewardInfo();


			if(value.Length > 0)
			{
				OnSelectEctypeClick( Convert.ToInt32(value[0]));
			}
			else
			{
				//默认选择
				OnSelectEctypeClick(m_CrusadeList.First()._vectContainer.First());
			}

			base.Show(value);
		}
		
		public override void Close()
		{
			
			base.Close();
		}


		private void UpdateEctypeList()
		{
			var playerData = PlayerManager.Instance.FindHeroDataModel();
			int index = 0;
			m_CrusadeList = EctypeConfigManager.Instance.EctypeSelectConfigList.Values.Where(p=>p.lEctypeType == 9).ToList();//首领讨伐副本 lEctypeType = 9
			bool isCreate = true;
			m_CrusadeList.ApplyAllItem(p=>{
				p._vectContainer.ApplyAllItem(k=>{
					if(isCreate)
					{
						EctypeContainerData ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[k];
						if(!m_ectypeItemList.ContainsKey(k))
						{
							index++;
							GameObject ectype = UI.CreatObjectToNGUI.InstantiateObj(CrusadeLeaderItemPrefab, Grid.transform);
							CrusadeLeaderItem crusadeLeaderItem = ectype.GetComponent<CrusadeLeaderItem>();
							crusadeLeaderItem.Init(p._EctypeIconPrefab, ectypeContainerData,playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL,OnSelectEctypeClick,index);
							ectype.RegisterBtnMappingId(p._lEctypeID,UIType.Crusade,BtnMapId_Sub.Crusade_EctypeItem);
							m_ectypeItemList.Add(k,crusadeLeaderItem);
						}
						if(ectypeContainerData.lMinActorLevel > playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL)
						{
							isCreate = false;
						}
					}
				});
			});
			Grid.Reposition();

			m_ectypeItemList.Values.ApplyAllItem(p=>p.UpdateInfo(playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL));
		}

		private void UpdateFirstCrusadeRewardInfo()
		{
			var playerData = PlayerManager.Instance.FindHeroDataModel();
			var prizeData = EctypeConfigManager.Instance.GetFirstBattlePrizeData(playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL);
			if(prizeData!=null)
			{
				int[] prizeDataToday = GetTodayPrizeData(prizeData);
				var itemData =ItemDataManager.Instance.GetItemData(prizeDataToday[0]);
				if(itemData != null)
				{
					UI_FirstRewardIcon.spriteName = itemData.smallDisplay;
                    CurrentRewardId=itemData._goodID;
				}
			}
			switch( playerData.PlayerValues.PLAYER_FIELD_RANDOM_ECTYPEREWARD_VALUE)
			{
			case 0:
				Button_Receive.SetEnabled(false);
				HaveBeenIssued.SetActive(false);
				break;
			case 1:
				Button_Receive.SetEnabled(true);
				HaveBeenIssued.SetActive(false);
				break;
			case 2:
				Button_Receive.SetEnabled(false);
				HaveBeenIssued.SetActive(true);
				break;
			}
		}
		private int[] GetTodayPrizeData(FirstBattlePrizeData data)
		{
			string dateStr = DateTime.Now.DayOfWeek.ToString();
			switch(dateStr)
			{
			case "Monday":return data.Monday;
			case "Tuesday": return data.Tuesday;
			case "Wednesday": return data.Wednesday;
			case "Thursday": return data.Thursday;
			case "Friday": return data.Friday;
			case "Saturday": return data.Saturday;
			case "Sunday":return data.Sunday;
			}
			return data.Monday;
		}

		private bool IsCanCrusade()
		{
			bool isCanCrusade = true;
			var ectypeConfig = EctypeConfigManager.Instance.EctypeContainerConfigList[m_CurEctypeID];
			//判断等级
			if(PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL
			   < ectypeConfig.lMinActorLevel)
			{
				MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_H1_1"),1f);
				return false;
			}

			//判断今日剩余讨伐次数
//			if(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CRUSADE_TIMESVALUE <= 0)
//			{
//				MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I19_8"),1f);
//				return false;
//			}

			//判断是否需要讨伐令（每日免费次数）
			int maxTime = CommonDefineManager.Instance.CommonDefine.Coop_DailyLimit;
			int freeTime = CommonDefineManager.Instance.CommonDefine.Coop_FreeTimes;
			int remainTime = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CRUSADE_TIMESVALUE;
			if(maxTime - remainTime < freeTime)//freeTime次讨伐内免费
			{
				return true;
			}

			//判断所需讨伐令是否足够
			bool isEnoughtItem = false;
			if(ectypeConfig.Coop_ItemCost_GoodsID != 0)
			{			
				int goodsNum = ContainerInfomanager.Instance.GetItemNumber(ectypeConfig.Coop_ItemCost_GoodsID);
				isEnoughtItem = goodsNum >= ectypeConfig.Coop_ItemCost_GoodsNum;
			}
			else
			{
				isEnoughtItem = true;
			}

			if(!isEnoughtItem)//所需道具不足
			{
				isCanCrusade = false;
				var ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[m_CurEctypeID];

				if(ectypeContainerData.Coop_IsItemQuikBuy)
				{
					MessageBox.Instance.Show(4,"",LanguageTextManager.GetString("IDS_I19_20"),LanguageTextManager.GetString("IDS_I19_22"),LanguageTextManager.GetString("IDS_I19_21"),null,JumpToQuickBuy);
				}
				else
				{
					MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I19_19"),1f);
				}
			}

			return isCanCrusade;
		}

		private bool JudgeAndExitTeam()
		{
			bool isExit = false;
			if(TeamManager.Instance.IsTeamExist())
			{
				if(TeamManager.Instance.GetCurrentEctypeType() == 0)
				{
					isExit = true;
				}
			}
			return isExit;
		}
		private void LeaveExitTeam()
		{
			var playerData = PlayerManager.Instance.FindHeroDataModel();
			var teamSmg = TeamManager.Instance.MyTeamProp;
			if(playerData.ActorID == teamSmg.TeamContext.dwCaptainId)
			{
				NetServiceManager.Instance.TeamService.SendTeamDisbandMsg(new SMsgTeamDisband_CS{
					dwActorID = (uint)playerData.ActorID,
					dwTeamID = teamSmg.TeamContext.dwId
				});
			}
			else
			{
				NetServiceManager.Instance.TeamService.SendTeamMemberLeaveMsg(new SMsgTeamMemberLeave_SC(){
					dwActorID = (uint)playerData.ActorID,
					dwTeamID = teamSmg.TeamContext.dwId
				});
			}
		}

		void JumpToQuickBuy()
		{
			if(ContainerInfomanager.Instance.GetEmptyPackBoxNumber() == 0)
			{
				StartCoroutine("ShowTip");
				return;
			}
			var ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[m_CurEctypeID];
			int itemID = 0;
			switch(ectypeContainerData.Coop_ItemCost_GoodsID)
			{
			case 3071000:
				itemID = CommonDefineManager.Instance.CommonDefine.Coop_CostItemShop1;
				break;
			case 3071001:
				itemID = CommonDefineManager.Instance.CommonDefine.Coop_CostItemShop2;
				break;
			case 3071002:
				itemID = CommonDefineManager.Instance.CommonDefine.Coop_CostItemShop3;
				break;
			}				
			PopupObjManager.Instance.OpenQuickBuyPanel(itemID);
		}
		IEnumerator ShowTip()
		{
			yield return new WaitForEndOfFrame();
			MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I22_39"),1f);
		}

		void OnSelectEctypeClick(int ectypeID)
		{
			this.m_CurEctypeID = ectypeID;

			var ectypeConfig = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
//			bool singleMode = ectypeConfig.Coop_Solo == 1;
//			SingleInterface.SetActive(singleMode);
//			TeamInterface.SetActive(!singleMode);
			int toSelectID = EctypeConfigManager.Instance.GetSelectContainerID(m_CurEctypeID);
			var toSelectEctypeData = EctypeConfigManager.Instance.EctypeSelectConfigList[toSelectID];
			TeamManager.Instance.SetCurSelectEctypeContainerData(toSelectEctypeData);

			int remainTime = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CRUSADE_TIMESVALUE;
			int maxTime = CommonDefineManager.Instance.CommonDefine.Coop_DailyLimit;
			int freeTime = CommonDefineManager.Instance.CommonDefine.Coop_FreeTimes;

			Label_CrusadeTime.text = (maxTime - remainTime).ToString()+"/"+maxTime.ToString();
			Label_FreeTime.text = (freeTime - (maxTime - remainTime)).ToString();

			if(maxTime - remainTime < freeTime)//freeTime次讨伐内免费
			{
				//ItemInfo.SetActive(false);
				NeedCrusadeItemInterface.SetActive(false);
				DontNeedCrusadeItemInterface.SetActive(true);
			}
			else
			{
				if(ectypeConfig.Coop_ItemCost_GoodsID != 0)
				{
					//ItemInfo.SetActive(true);
					NeedCrusadeItemInterface.SetActive(true);
					DontNeedCrusadeItemInterface.SetActive(false);
					var itemData = ItemDataManager.Instance.GetItemData(ectypeConfig.Coop_ItemCost_GoodsID);
					int itemNum = ContainerInfomanager.Instance.GetItemNumber(itemData._goodID);
					UI_Qualifications.spriteName = itemData.smallDisplay;
					Label_QualificationsNum.text = ectypeConfig.Coop_ItemCost_GoodsNum.ToString()+"/"+itemNum.ToString(); // 消耗/拥有
					if(itemNum<ectypeConfig.Coop_ItemCost_GoodsNum)
					{
						Label_QualificationsNum.color = Color.red * 0.9f;
					}
					else
					{
						Label_QualificationsNum.color = Color.white;
					}
				}
				else
				{
					//ItemInfo.SetActive(false);
					NeedCrusadeItemInterface.SetActive(false);
					DontNeedCrusadeItemInterface.SetActive(true);
				}
			}

			if(m_CurEctypeID != 0)
			{
				SelectFrame.SetActive(true);
				var ectypeItem = m_ectypeItemList[m_CurEctypeID];//SingleOrDefault(p=>p.EctypeData.lEctypeContainerID == ectypeID);
				SelectFrame.transform.parent = ectypeItem.transform;
				SelectFrame.transform.localPosition = Vector3.zero;
			}
			else
			{
				SelectFrame.SetActive(false);
			}
		}

		void OnViewRewardsClick(bool isPressed)
		{
			if(isPressed)
			{
				var ectypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[m_CurEctypeID];
				EctypeDropItemDesPanel.TweenShow(ectypeContainerData);
			}
			else
			{
				EctypeDropItemDesPanel.TweenClose();
			}
		}

		void OnExitClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Back");
			Close();
		}

		void OnStartClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_CoopSolo");
			if(!IsCanCrusade())
			{
				return;
			}
			MessageBox.Instance.Show(4,"",LanguageTextManager.GetString("IDS_I19_32"),LanguageTextManager.GetString("IDS_I19_11"),LanguageTextManager.GetString("IDS_I19_12"),CancelCrusadeHandle ,StartCrusadeHandle);
		}

		void OnCreateTeamClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_CoopCreateTeam");
			if(!IsCanCrusade())
			{
				return;
			}

			if(CrusadeManager.Instance.IsMatchingEctype)
			{
				MessageBox.Instance.Show(4,"",LanguageTextManager.GetString("IDS_I19_25"),LanguageTextManager.GetString("IDS_I19_22"),LanguageTextManager.GetString("IDS_I19_12"),null,()=> {
					OnCancelRandomEctypeClick(null);
					m_CrusadeList.ApplyAllItem(p=>{
						for(int i = 0; i < p._vectContainer.Length; i++)
						{
							if(p._vectContainer[i] == m_CurEctypeID)
							{
								NetServiceManager.Instance.TeamService.SendTeamCreateMsg(p._lEctypeID, i+1, 0);
								int selectID = EctypeConfigManager.Instance.GetSelectContainerID(m_CurEctypeID);
								var selectEctypeData = EctypeConfigManager.Instance.EctypeSelectConfigList[selectID];
								TeamManager.Instance.SetCurSelectEctypeContainerData(selectEctypeData);
								MainUIController.Instance.OpenMainUI(UIType.TeamInfo,1,0);
								return;
							}
						}
					});
				});

				return;
			}

			if(JudgeAndExitTeam())
			{
				Action action = ()=>{
					LeaveExitTeam();
					TeamManager.Instance.SetWaitExitTeamAction(()=>{
						m_CrusadeList.ApplyAllItem(p=>{
							for(int i = 0; i < p._vectContainer.Length; i++)
							{
								if(p._vectContainer[i] == m_CurEctypeID)
								{
									NetServiceManager.Instance.TeamService.SendTeamCreateMsg(p._lEctypeID, i+1, 0);
									int selectID = EctypeConfigManager.Instance.GetSelectContainerID(m_CurEctypeID);
									var selectEctypeData = EctypeConfigManager.Instance.EctypeSelectConfigList[selectID];
									TeamManager.Instance.SetCurSelectEctypeContainerData(selectEctypeData);
									MainUIController.Instance.OpenMainUI(UIType.TeamInfo,1,0);
									return;
								}
							}
						});
					});
				};
				TeamManager.Instance.ShowLeaveTeamTip(action);
			}
			else
			{
				m_CrusadeList.ApplyAllItem(p=>{
					for(int i = 0; i < p._vectContainer.Length; i++)
					{
						if(p._vectContainer[i] == m_CurEctypeID)
						{
							NetServiceManager.Instance.TeamService.SendTeamCreateMsg(p._lEctypeID, i+1, 0);
							int selectID = EctypeConfigManager.Instance.GetSelectContainerID(m_CurEctypeID);
							var selectEctypeData = EctypeConfigManager.Instance.EctypeSelectConfigList[selectID];
							TeamManager.Instance.SetCurSelectEctypeContainerData(selectEctypeData);
							MainUIController.Instance.OpenMainUI(UIType.TeamInfo,1,0);
							return;
						}
					}
				});
			}
		}

		void OnFindTeamClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_CoopJoinTeam");
			if(!IsCanCrusade())
			{
				return;
			}

			if(CrusadeManager.Instance.IsMatchingEctype)
			{
				MessageBox.Instance.Show(4,"",LanguageTextManager.GetString("IDS_I19_25"),LanguageTextManager.GetString("IDS_I19_22"),LanguageTextManager.GetString("IDS_I19_12"),null,()=> {
					OnCancelRandomEctypeClick(null);
					int toSelectID = EctypeConfigManager.Instance.GetSelectContainerID(m_CurEctypeID);
					var toSelectEctypeData = EctypeConfigManager.Instance.EctypeSelectConfigList[toSelectID];
					TeamManager.Instance.SetCurSelectEctypeContainerData(toSelectEctypeData);
					MainUIController.Instance.OpenMainUI(UIType.TeamInfo,0,0);
				});
				return;
			}

			if(JudgeAndExitTeam())
			{
				Action action = ()=>{
					LeaveExitTeam();
					TeamManager.Instance.SetWaitExitTeamAction(()=>{
						int selectID = EctypeConfigManager.Instance.GetSelectContainerID(m_CurEctypeID);
						var selectEctypeData = EctypeConfigManager.Instance.EctypeSelectConfigList[selectID];
						TeamManager.Instance.SetCurSelectEctypeContainerData(selectEctypeData);
						MainUIController.Instance.OpenMainUI(UIType.TeamInfo,0,0);
					});
				};
				TeamManager.Instance.ShowLeaveTeamTip(action);
			}
			else
			{
				int selectID = EctypeConfigManager.Instance.GetSelectContainerID(m_CurEctypeID);
				var selectEctypeData = EctypeConfigManager.Instance.EctypeSelectConfigList[selectID];
				TeamManager.Instance.SetCurSelectEctypeContainerData(selectEctypeData);
				MainUIController.Instance.OpenMainUI(UIType.TeamInfo,0,0);
			}
		}
		void OnQuickJoinClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_CoopJoinTeam");
			if(!IsCanCrusade())
			{
				return;
			}
			if(JudgeAndExitTeam())
			{
				Action action = ()=>{
					LeaveExitTeam();
					TeamManager.Instance.SetWaitExitTeamAction(()=>{
						StartCoroutine("LateJoin");
					});
				};
				TeamManager.Instance.ShowLeaveTeamTip(action);
			}
			else
			{
				var player = PlayerManager.Instance.FindHeroDataModel();
				NetServiceManager.Instance.TeamService.SendTeamFastJoinMsg(new SMsgTeamFastJoin_CS(){
					dwEctypeId = (uint)m_CurEctypeID,
					dwActorId = (uint)player.ActorID,
					byEctypeDiff = 0,
				});
			}

		}
		IEnumerator LateJoin()
		{
			yield return new WaitForSeconds(1f);
			var player = PlayerManager.Instance.FindHeroDataModel();
			NetServiceManager.Instance.TeamService.SendTeamFastJoinMsg(new SMsgTeamFastJoin_CS() {
				dwEctypeId = (uint)m_CurEctypeID,
				dwActorId = (uint)player.ActorID,
				byEctypeDiff = 0,
			});
		}
		void OnReceiveFirstRewardClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default_Get");
			NetServiceManager.Instance.EctypeService.SendEctypeRandomReward();
		}
		void OnRandomEctypeClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_MatchOK");

			//判断有无可匹配的副本
			var playerData = PlayerManager.Instance.FindHeroDataModel();
		
			bool isCanCrusade = false;
			//等级足够匹配最低要求副本
			if(m_ectypeItemList.Count>0 && m_ectypeItemList.First().Value.EctypeData.lMinActorLevel <= playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL)
			{
				//判断是否需要讨伐令（每日免费次数）
				int maxTime = CommonDefineManager.Instance.CommonDefine.Coop_DailyLimit;
				int freeTime = CommonDefineManager.Instance.CommonDefine.Coop_FreeTimes;
				int remainTime = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CRUSADE_TIMESVALUE;
				if(maxTime - remainTime < freeTime)//freeTime次讨伐内免费
				{
					isCanCrusade = true;
				}
				else //需要讨伐令的情况
				{
					//判断是否存在任何一种讨伐令
					int[] goodIDs = new int[3]{3071000,3071001,3071002};
					bool isEnoughtItem = false;
					for(int i=0;i<goodIDs.Length;i++)
					{
						int goodsNum = ContainerInfomanager.Instance.GetItemNumber(goodIDs[i]);
						if(goodsNum > 0)
						{
							isEnoughtItem = true;
							break;
						}
					}
					if(!isEnoughtItem)
					{
						MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I19_19"),1f);
						return;
					}				
				}
			}
			else
			{
				MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I10_8"),1f);
				return;
			}

//			if(!isCanCrusade)
//			{
//				return;
//			}
			if(JudgeAndExitTeam())
			{
				TeamManager.Instance.ShowLeaveTeamTip(()=>{
					LeaveExitTeam();
					TeamManager.Instance.SetWaitExitTeamAction(()=>{
						CrusadeManager.Instance.IsMatchingEctype = true;
						Eff_AutoMatching.SetActive(true);
						Label_MatchExplain.SetActive(false);
						Button_RandomEctype.gameObject.SetActive(false);
						Button_CancelRandomEctype.gameObject.SetActive(true);
						//上发匹配请求
						NetServiceManager.Instance.TeamService.SendBegingCrusadeMatching();
					});
				});
			}
			else
			{
				CrusadeManager.Instance.IsMatchingEctype = true;
				Eff_AutoMatching.SetActive(true);
				Label_MatchExplain.SetActive(false);
				Button_RandomEctype.gameObject.SetActive(false);
				Button_CancelRandomEctype.gameObject.SetActive(true);
				NetServiceManager.Instance.TeamService.SendBegingCrusadeMatching();
			}
		}
		void OnCancelRandomEctypeClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_MatchCancel");
			CrusadeManager.Instance.IsMatchingEctype = false;
			Eff_AutoMatching.SetActive(false);
			Label_MatchExplain.SetActive(true);
			Button_RandomEctype.gameObject.SetActive(true);
			Button_CancelRandomEctype.gameObject.SetActive(false);
			//上发取消匹配请求
			NetServiceManager.Instance.TeamService.SendCancelCrusadeMatching();
		}

		void CancelCrusadeHandle()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_GetMatchCancel");
		}
		void StartCrusadeHandle()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_GetMatchOK");
			NetServiceManager.Instance.EctypeService.SendEctypeRequest(new SMSGEctypeRequestCreate_CS(){
				dwEctypeContainerID = m_CurEctypeID
			});
		}

		void UpdateEctypeTeamNumHandle(object obj)
		{
			SMsgEctypeTeamNum_SC sMsgEctypeTeamNum_SC = (SMsgEctypeTeamNum_SC)obj;
			sMsgEctypeTeamNum_SC.sTeamNumContext.ApplyAllItem(p=>{
				if(m_ectypeItemList.ContainsKey(p.dwEctypeID))
				{
					m_ectypeItemList[p.dwEctypeID].UpdateTeamNum(p.byTeamNum);
				}
			});

		}
		void ReceiveCrusadeMatchingHandle(object obj)
		{
			if(!CrusadeManager.Instance.IsMatchingEctype)
			{
				return;
			}
			SMsgConfirmMatching_SC sMsgConfirmMatching_SC = (SMsgConfirmMatching_SC)obj;
			if(MatchingPanel == null)
			{
				GameObject matchingPanel = UI.CreatObjectToNGUI.InstantiateObj(CrusadeMatchingPanelPrefab,transform);
				MatchingPanel = matchingPanel.GetComponent<CrusadeMatchingPanel>();
			}
			MatchingPanel.CrusadeMatching(sMsgConfirmMatching_SC);
		}
		void UpdateRandomRewardStatusHandle(object obj)
		{
			var playerData = PlayerManager.Instance.FindHeroDataModel();
			switch( playerData.PlayerValues.PLAYER_FIELD_RANDOM_ECTYPEREWARD_VALUE)
			{
			case 0:
				Button_Receive.SetEnabled(false);
				HaveBeenIssued.SetActive(false);
				break;
			case 1:
				Button_Receive.SetEnabled(true);
				HaveBeenIssued.SetActive(false);
				break;
			case 2:
				Button_Receive.SetEnabled(false);
				HaveBeenIssued.SetActive(true);
				break;
			}
		}


		void TeamNoFoundListHandle(INotifyArgs arg)
		{
			if(transform.localPosition == Vector3.zero)
			{
				MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_160"),
				                         LanguageTextManager.GetString("IDS_H2_4"), LanguageTextManager.GetString("IDS_H2_28"),SureCreateTeam , null);
			}
			
		}	
		void SureCreateTeam()
		{
			OnCreateTeamClick(null);
		}

		void MatchingCancelHandle(object obj)
		{
			CrusadeManager.Instance.IsMatchingEctype = false;
			Eff_AutoMatching.SetActive(false);
			Label_MatchExplain.SetActive(true);
			Button_RandomEctype.gameObject.SetActive(true);
			Button_CancelRandomEctype.gameObject.SetActive(false);		
		}

		void UpdateCrusadeItemHandle(object obj)
		{
			OnSelectEctypeClick(m_CurEctypeID);
		}

		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeTeamNum ,UpdateEctypeTeamNumHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CrusadeMatching, ReceiveCrusadeMatchingHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RandomMatchingCancel, MatchingCancelHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UpdateRandomRewardStatus,UpdateRandomRewardStatusHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CancelRandomRewardMatching,OnCancelRandomEctypeClick);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, UpdateCrusadeItemHandle);
			RemoveEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
		}

		protected override void RegisterEventHandler()
		{
			UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeTeamNum ,UpdateEctypeTeamNumHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.CrusadeMatching, ReceiveCrusadeMatchingHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.RandomMatchingCancel, MatchingCancelHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.UpdateRandomRewardStatus,UpdateRandomRewardStatusHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.CancelRandomRewardMatching,OnCancelRandomEctypeClick);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods, UpdateCrusadeItemHandle);
			AddEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
		}

	}
}
