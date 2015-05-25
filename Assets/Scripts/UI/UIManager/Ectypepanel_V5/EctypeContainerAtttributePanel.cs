using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI{

	public class EctypeContainerAtttributePanel : MonoBehaviour {
		public enum TitleBtnType{}

		public SingleButtonCallBack TitleBtn_Esy;
		public SingleButtonCallBack TitleBtn_Diff;
		public SingleButtonCallBack ViewDropBtn;
		public SingleButtonCallBack EctypeDesBtn;
		public SingleButtonCallBack SingleJoyBtn;
		public SingleButtonCallBack TeamJoyBtn;
		public SingleButtonCallBack MyAtkForceLabel;
		public SingleButtonCallBack SeggesionForcelabel;
		public SingleButtonCallBack JoinTackLabel;
		public SingleButtonCallBack SweepBtn;
		public SpriteSwith sweepBtnState;
		public GameObject sweepHLight;
		public GameObject sweepEff;
		public EctypeDesPanel m_EctypeDesPanel;//副本介绍面板
		public EctypeDropItemDesPanel m_EctypeDropItemDesPanel;//副本掉落信息

		public EctypeContainerListPanel MyParent{get;private set;}
		public EctypeContainerData EsyEctypeContainerData{get;private set;}
		public EctypeContainerData DiffEctypeContainerData{get;private set;}
		public EctypeContainerData CurrentSelectEctypeContaienrData{get;private set;}

		void Awake()
		{
			ViewDropBtn.SetPressCallBack(OnDropItemDesBtnPress);
			EctypeDesBtn.SetPressCallBack(OnEctypeDesBtnPress);
			SingleJoyBtn.SetCallBackFuntion(OnSingleJoinBtnClick);
			SingleJoyBtn.SetPressCallBack(OnSingleJoinBtnPress);
			TeamJoyBtn.SetCallBackFuntion(OnTeamJoinBtnClick);
			SweepBtn.SetCallBackFuntion(OnSweepBtnClick);
			SweepBtn.SetPressCallBack(OnSweepBtnPress);
			sweepHLight.SetActive (false);
            TaskGuideBtnRegister();
			sweepEff.SetActive (false);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeNormalDataUpdate, OnEctypeNormalOpenListUpdateEvent);
		}
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            //TitleBtn_Esy.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_Difficulty01);
            //TitleBtn_Diff.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_Difficulty02);
            ViewDropBtn.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_DropList);
            EctypeDesBtn.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_BossDescription);
            SingleJoyBtn.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_Start);
            TeamJoyBtn.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_Team);
            //MyAtkForceLabel.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_Difficulty02);
            //SeggesionForcelabel.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_Difficulty02);
            //JoinTackLabel.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_Difficulty02);
        }
		public void Init(int esyEctypeContaienrID,int diffEctypeContaienrID,EctypeContainerListPanel myParent)
		{
			EctypeContainerData esyData;
			EctypeConfigManager.Instance.EctypeContainerConfigList.TryGetValue(esyEctypeContaienrID,out esyData);
			EctypeContainerData diffData;
			EctypeConfigManager.Instance.EctypeContainerConfigList.TryGetValue(diffEctypeContaienrID,out diffData);
			this.EsyEctypeContainerData = esyData;
			this.DiffEctypeContainerData = diffData;
			/*if (diffData == null) {
				TitleBtn_Diff.gameObject.SetActive (false);	
			} else {
				TitleBtn_Diff.gameObject.SetActive(true);		
			}*/
			//TitleBtn_Esy.SetCallBackFuntion(OnTitleBtnClick,EsyEctypeContainerData);
			//TitleBtn_Diff.SetCallBackFuntion(OnTitleBtnClick,DiffEctypeContainerData);
			MyParent = myParent;
			//选中默认难度按钮
			SelectTitleBtn(EsyEctypeContainerData);

			if(MyParent.EctypeIDIsLock(diffEctypeContaienrID))
			{
				/*TitleBtn_Diff.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(1));
				TitleBtn_Diff.spriteSwith.ChangeSprite(3);
				TitleBtn_Diff.Enable = false;*/
				SetCreateTeam(EsyEctypeContainerData.AllowCreatTeam);
			}else
			{
				//TitleBtn_Diff.Enable = true;
				SetCreateTeam(DiffEctypeContainerData.AllowCreatTeam);
			}
			InitSweep (esyEctypeContaienrID,true);
			UpdateLabel();
		}

		void SetCreateTeam(int mark)
		{
			if (mark == 0) {
				TeamJoyBtn.gameObject.SetActive (false);	
			} else {
				TeamJoyBtn.gameObject.SetActive (true);			
			}
		}
		void SelectTitleBtn(object obj)
		{

			CurrentSelectEctypeContaienrData = obj as EctypeContainerData;
			MyParent.curSelectEasyEctypeID = EsyEctypeContainerData.lEctypeContainerID;
			//TitleBtn_Esy.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(CurrentSelectEctypeContaienrData == EsyEctypeContainerData?2:1));
			if(DiffEctypeContainerData!=null&&!MyParent.EctypeIDIsLock(DiffEctypeContainerData.lEctypeContainerID))
			{
				//TitleBtn_Diff.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(CurrentSelectEctypeContaienrData == DiffEctypeContainerData?2:1));
			}
			UpdateLabel ();
		}
		void OnTitleBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_EctypeDifficultyChoice");
			SelectTitleBtn (obj);
		}

		void UpdateLabel()
		{
			int atk = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat, PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
			MyAtkForceLabel.SetButtonText(atk.ToString());
			SeggesionForcelabel.SetButtonText(CurrentSelectEctypeContaienrData.FightingCapacity.ToString());
			JoinTackLabel.spriteSwith.ChangeSprite(CurrentSelectEctypeContaienrData.lCostType);
			JoinTackLabel.SetButtonText(CurrentSelectEctypeContaienrData.lCostEnergy);
		}

		void OnSingleJoinBtnClick(object obj)
		{
			if(TeamManager.Instance.IsTeamExist())
			{
				if(TeamManager.Instance.GetCurrentEctypeType() == 9)
				{
					TeamManager.Instance.ShowLeaveTeamTip(()=>{
						CheckGotoBattle(SendGoBattleToServer);
					});
				}
			}
			else
			{
				CheckGotoBattle(SendGoBattleToServer);
			}
		}
		/// <summary>
		/// 当快速加入的时候检查背包
		/// </summary>
//		public void CheckQuickJionBackpack()
//		{
//			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
//			ushort maxNum = ContainerInfomanager.Instance.GetContainerClientContsext(2).wMaxSize;
//			var backpack = ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(2).Where(p => p.uidGoods != 0).ToList();
//			if (maxNum - backpack.Count < 2)
//			{
//				MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_201"), LanguageTextManager.GetString("IDS_H2_55"), LanguageTextManager.GetString("IDS_H2_28"), SendGoBattleToServer, null);
//			}
//			else
//			{
//				CheckGotoBattle(SendGoBattleToServer);
//			}
//		}
		
		/// <summary>
		/// 检测是否足够资源加入副本
		/// </summary>
		bool CheckGotoBattle(ButtonCallBack SureBtnCallback)
		{
			bool Flag = true;
			EctypeContainerData SelectContainerData = CurrentSelectEctypeContaienrData;
			switch (SelectContainerData.lCostType)
			{
			case 1:
				int Cost = int.Parse(SelectContainerData.lCostEnergy);
				int PayMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
				if (PayMoney >= Cost)
				{
					JudgeAndExitTeam();
					if (SureBtnCallback != null) { SureBtnCallback(null); }
				}
				else
				{
					//UIEventManager.Instance.TriggerUIEvent(UIEventType.NoEnoughActiveLife, null);
					PopupObjManager.Instance.ShowAddVigour();
				}
				break;
			case 2:
				Cost = int.Parse(SelectContainerData.lCostEnergy);
				PayMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
				if (PayMoney < Cost)
				{
					//MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H2_44"), LanguageTextManager.GetString("IDS_H2_55"), null);
					MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"), 1);
					Flag = false;
				}
				else
				{
					JudgeAndExitTeam();
					if (SureBtnCallback != null) { SureBtnCallback(null); }
				}
				break;
			case 3:
				int CostGold = int.Parse(SelectContainerData.lCostEnergy);
				int BINDPAY = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
				if (BINDPAY < CostGold)
				{
					//MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_231"), LanguageTextManager.GetString("IDS_H2_55"), null);
					MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_231"), 1);
					Flag = false;
				}
				else
				{
					JudgeAndExitTeam();
					if (SureBtnCallback != null) { SureBtnCallback(null); }
				}
				break;
			default:
				break;
			}
			return Flag;
		}
		void LoadingUICallBack()
		{
			LoadingUI.Instance.Close ();
		}
		void SendGoBattleToServer(object obj)
		{
			LoadingUI.Instance.Show ();
			if (IsInvoking ("LoadingUICallBack")) {
				CancelInvoke("LoadingUICallBack");		
			}
			Invoke ("LoadingUICallBack",10);
			EctypeModel.Instance.SendGoBattleToServer (CurrentSelectEctypeContaienrData.lEctypeContainerID);
		}
		void OnEctypeNormalOpenListUpdateEvent(object obj)
		{
			bool isOpenSweep = (bool)obj;
			if (isOpenSweep) {
				InitSweep(curEctypeID,true);	
			}
		}
		//扫荡
		private int curEctypeID;
		private SMSGEctypeData_SC ectypeServerData;
		private bool isNowRequestServerOpenSweep = false;
		void InitSweep(int ectypeID,bool isEasy)
		{
			sweepEff.SetActive(false);
			if (!isEasy) {
				SweepBtn.gameObject.SetActive (false);
				isNowRequestServerOpenSweep = false;
				return;
			} else {
				SweepBtn.gameObject.SetActive(true);			
			}
			curEctypeID = ectypeID;
			ectypeServerData = EctypeModel.Instance.sMSGEctypeSelect_SC.sMSGEctypeData_SCs.FirstOrDefault(c=>c.dwEctypeContaienrID == curEctypeID);
			if (ectypeServerData.bySweep == 1) {
				if(isNowRequestServerOpenSweep)
				{
					//开启特效,就闪一下//
					SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EctypeOpenSweep");
					sweepEff.SetActive(true);
				}
				sweepBtnState.ChangeSprite (2);
			} else {
				sweepBtnState.ChangeSprite (1);

			}
			isNowRequestServerOpenSweep = false;
		}
		void OnSweepBtnPress(bool isPressed)
		{
			if (isPressed) {
				sweepHLight.SetActive (true);
			} else {
				sweepHLight.SetActive(false);			
			}
		}
		void OnSweepBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_EctypeSweep");
			if (ectypeServerData.bySweep == 1) {
				//已经开启过
				if(isCanSweepNoPop())
				{
					//直接弹出扫荡框
					MyParent.sweepPopPanel.Show(MyParent);
				}
			} else {
				//开启扫荡		
				if(isCanOpenSweep())
				{
					//请求开启
					EctypeModel.Instance.SendRequestOpenSweep(curEctypeID);
					isNowRequestServerOpenSweep = true;
				}
			}
		}
		bool isCanOpenSweep()
		{
			if (ectypeServerData.byGrade < 6) {
				//要3s才能开启
				MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I34_1"), 1);
				return false;
			}
			if (ContainerInfomanager.Instance.GetItemNumber(CommonDefineManager.Instance.CommonDefine.SweepID) < 1) {
				//扫荡令不够
				MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I34_2"), 1);
				return false;	
			}
			return true;
		}
		bool isCanSweepNoPop()
		{
			if (TeamManager.Instance.IsTeamExist ()) {
				//组队不能
				MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I34_3"), 1);
				return false;			
			}
			if (ContainerInfomanager.Instance.PackIsFull ()) {
				//背包满
				MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I34_4"), 1);
				return false;			
			}
			return true;
		}
		void OnTeamJoinBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_EctypeTeam");
			System.Action action = ()=>{
				JudgeAndExitTeam();
				var playerData = PlayerManager.Instance.FindHeroDataModel();
				//发送获取队伍列表
				NetServiceManager.Instance.TeamService.SendGetTeamListMsg(new SMSGGetTeamList_CS() { 
					uidEntity = playerData.UID,
					dwEctypeID = (uint)MyParent.EctypeSelectData._lEctypeID,
					byDifficulty = 0,
				});
				TeamManager.Instance.SetCurSelectEctypeContainerData(MyParent.EctypeSelectData);
				MainUIController.Instance.OpenMainUI(UIType.TeamInfo);
			};
			if(TeamManager.Instance.IsTeamExist())
			{
				if(TeamManager.Instance.GetCurrentEctypeType() == 9)
				{
					TeamManager.Instance.ShowLeaveTeamTip(action);
				}
			}
			else
			{
				action();
			}


		}

		void OnSingleJoinBtnPress(bool isPressed)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_EctypeStart");
			SingleJoyBtn.BackgroundSprite.gameObject.SetActive(isPressed);
		}

		void OnEctypeDesBtnPress(bool isPressed)
		{
			EctypeDesBtn.BackgroundSprite.gameObject.SetActive(isPressed?true:false);
			if(isPressed){m_EctypeDesPanel.TweenShow(CurrentSelectEctypeContaienrData);}
			else{m_EctypeDesPanel.TweenClose();}
		}

		void OnDropItemDesBtnPress(bool isPressed)
		{
			ViewDropBtn.BackgroundSprite.gameObject.SetActive(isPressed?true:false);
			if(isPressed){m_EctypeDropItemDesPanel.TweenShow(CurrentSelectEctypeContaienrData);}
			else{m_EctypeDropItemDesPanel.TweenClose();}
		}
		void OnDestroy()
		{
			if (IsInvoking ("LoadingUICallBack")) {
				CancelInvoke("LoadingUICallBack");			
			}
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeNormalDataUpdate, OnEctypeNormalOpenListUpdateEvent);
		}

		private void JudgeAndExitTeam()
		{
			if(TeamManager.Instance.IsTeamExist())
			{
				if(TeamManager.Instance.GetCurrentEctypeType() == 9)
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
			}
		}
	}
}