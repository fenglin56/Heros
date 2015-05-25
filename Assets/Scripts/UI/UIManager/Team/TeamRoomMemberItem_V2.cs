using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Linq;
using System;
using System.Collections.Generic;
using UI.Friend;

namespace UI.Team
{

	public class TeamRoomMemberItem_V2 : MonoBehaviour
	{
		public SpriteSwith AvatarSwith;
		//public UILabel Label_Profession;
		//public UILabel LevelLabel;
		//public UILabel NickNameLabel;
		//public Transform ReadyTab;
		//public LocalButtonCallBack ViewInfoButtonCallBack;
		//public Transform WaitTab;
		public STeamMemberContext TeamMemberContext;
		//public Transform NoRequirementTab;
		//public UILabel Label_NoRequirement;
		
		private EctypeContainerData m_currentEctypeData;

		#region 修改
		public GameObject CaptionInterface;
		public GameObject MemberInterface;
		//public UISprite UI_Head;
		public UISprite UI_ProfessionIcon_Caption;
		public UISprite UI_ProfessionIcon_Member;
		public UILabel Label_Level;
		public UILabel Label_Name;
		public SpriteSwith Swith_Ready; //1未准备 2准备
		public LocalButtonCallBack Button_Kick;
		public UILabel Label_Combat;
		private bool m_isCaptain = false;
		private bool m_isFirst = false;
		#endregion 
		
		public void InitInfo(Transform posTrans, Transform waitTab)
		{
			transform.parent = posTrans;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			//ViewInfoButtonCallBack.SetCallBackFuntion(OnViewFriendInfoClick);
			
			//WaitTab = waitTab;
			
			//parentName = posTrans.name;
		}
		
		public void UpdateInfo()
		{
			if (!gameObject.activeInHierarchy)
			{
				gameObject.SetActive(true);                
			}
			//WaitTab.gameObject.SetActive(false);

			CaptionInterface.SetActive(m_isFirst);
			MemberInterface.SetActive(!m_isFirst);
			//Swith_Ready.gameObject.SetActive(!m_isFirst);

			Button_Kick.gameObject.SetActive(!m_isFirst &&  m_isCaptain);

			UI_ProfessionIcon_Caption.spriteName = SpriteName.PROFESSION_CHAR+TeamMemberContext.byKind.ToString();
			UI_ProfessionIcon_Member.spriteName = SpriteName.PROFESSION_CHAR+TeamMemberContext.byKind.ToString();

			Label_Level.text = LanguageTextManager.GetString("IDS_I12_10")+ TeamMemberContext.nLev.ToString();
			Label_Name.text = TeamMemberContext.szName;

			Label_Combat.text = TeamMemberContext.fightNum.ToString();
			//AvatarSwith.ChangeSprite(TeamMemberContext.byKind);
//			var resData = CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_TownAndTeam.FirstOrDefault(
//				P => P.VocationID == TeamMemberContext.byKind && P.FashionID == TeamMemberContext.nHead);
//			UI_Head.spriteName = resData.ResName;

			//var professionData = PlayerDataManager.Instance.GetProfessionConfigData(TeamMemberContext.byKind);
			//Label_Profession.text = LanguageTextManager.GetString(professionData._professionName);
					
			bool isReady = SetReadyState(TeamMemberContext.byFightReady);
			
			
			//是自己
//			if (PlayerManager.Instance.FindHeroDataModel().ActorID == TeamMemberContext.dwActorID)
//			{
//				//ViewInfoButtonCallBack.gameObject.SetActive(false);
//			}
//			else
//			{
//				//如果已经是好友:
//				List<PanelElementDataModel> onlineFreindList = new List<PanelElementDataModel>();
//				var friendQueue = FriendDataManager.Instance.FriendUIQueueList.Where(p => ((PanelElementDataModel)p).sMsgRecvAnswerFriends_SC.bOnLine == 1).ToList();
//				for (int i = 0; i < friendQueue.Count; i++)
//				{
//					onlineFreindList.Add((PanelElementDataModel)friendQueue[i]);
//				}
//				List<PanelElementDataModel> friendList = new List<PanelElementDataModel>();
//				friendList.AddRange(UI.Friend.FriendDataManager.Instance.GetFriendListData);                   
//				friendList.AddRange(onlineFreindList);
//				if (friendList.Exists(p => p.sMsgRecvAnswerFriends_SC.dwFriendID == TeamMemberContext.dwActorID))
//				{
//					//ViewInfoButtonCallBack.gameObject.SetActive(false);
//				}
//				else
//				{
//					//ViewInfoButtonCallBack.gameObject.SetActive(true);
//				}
//			}
			
			if (isReady == false)
			{
				//lEctypeType: 1=普通 , 2=封魔, 3=pvp
				if (m_currentEctypeData.lEctypeType == 0)
				{                    
					//如果没有活力
					if (TeamMemberContext.nCurActiveLife <= 0)
					{
//						Label_NoRequirement.text = LanguageTextManager.GetString("IDS_H1_455");
//						NoRequirementTab.gameObject.SetActive(true);
//						PlayAnimation(NoRequirementTab);
					}
					else
					{
//						NoRequirementTab.gameObject.SetActive(false);
					}
				}
				else if (m_currentEctypeData.lEctypeType == 1)
				{
					//元宝不足
					//TraceUtil.Log("nCurPayMoney = " + TeamMemberContext.nCurPayMoney + "   , cost = " + Int32.Parse(m_currentEctypeData.lCostEnergy));
					if (TeamMemberContext.nCurPayMoney < Int32.Parse(m_currentEctypeData.lCostEnergy))
					{
//						Label_NoRequirement.text = LanguageTextManager.GetString("IDS_H2_44");
//						NoRequirementTab.gameObject.SetActive(true);
//						PlayAnimation(NoRequirementTab);
					}
					else
					{
//						NoRequirementTab.gameObject.SetActive(false);
					}
				}
				else
				{
//					NoRequirementTab.gameObject.SetActive(false);
				}
			}
			else
			{
//				NoRequirementTab.gameObject.SetActive(false);
			}
		}
		
		public void SetInfo(STeamMemberContext memberContext, EctypeContainerData ectypeData, bool isCaptain , bool isFirst)
		{
			this.TeamMemberContext = memberContext;
			this.m_currentEctypeData = ectypeData;
			this.m_isCaptain = isCaptain;
			this.m_isFirst = isFirst;
			this.Button_Kick.SetCallBackFuntion(OnKickClick,null);
			this.UpdateInfo();
		}

		void OnKickClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamKick");
			MessageBox.Instance.Show(4, "", string.Format(LanguageTextManager.GetString("IDS_H1_164"),  TeamMemberContext.szName ),
			                         LanguageTextManager.GetString("IDS_H2_27"), LanguageTextManager.GetString("IDS_H2_28"), Kick, Cencel);  
		}


		void Kick()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamKickConfirmation");
			var prop = TeamManager.Instance.MyTeamProp;
			this.BeKicked((uint)prop.TeamContext.dwCaptainId,prop.TeamContext.dwId);
		}
		void Cencel()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamKickCancel");
		}

		private void ClearInfo()
		{
			this.TeamMemberContext = new STeamMemberContext();
			this.m_currentEctypeData = null;
		}
		
		public void Close()
		{
			this.ClearInfo();
			gameObject.SetActive(false);
		}
		
		//被踢出
		private void BeKicked(uint dwCaptainId, uint teamId)
		{
			NetServiceManager.Instance.TeamService.SendTeamMemberKickMsg(new SMsgTeamMemberKick_CS() 
			                                                             {
				dwActorID = dwCaptainId,
				dwTeamID = teamId,
				dwTargetActorID = TeamMemberContext.dwActorID
			});
		}
		
		//设置准备状态
		public bool SetReadyState(int statusCode)
		{
			bool isReady = ((TeamInfoUIManager.TEAM_MEMBER_STATUS)statusCode) == TeamInfoUIManager.TEAM_MEMBER_STATUS.TEAM_MEMBER_READY_STATUS ? true : false;
			//Swith_Ready.ChangeSprite(isReady == true? 2 : 1); 

			return isReady;
		}
		
		//播放提示出现动画
		private void PlayAnimation(Transform tip)
		{
			var tweenScale = tip.GetComponentInChildren<TweenScale>();
			StopAllCoroutines();
			StartCoroutine(LatePlayAnimation(tweenScale, 0.1f));
		}
		IEnumerator LatePlayAnimation(TweenScale ts, float delayTime)
		{            
			ts.Reset();
			ts.Play(true);
			yield return new WaitForSeconds(delayTime);
			ts.Play(false);
		}
		
		void OnViewFriendInfoClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
			//加对方为好友
			NetServiceManager.Instance.FriendService.SendAddFriendRequst(new SMsgAddFriends_CS()
			                                                             {
				dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID,
				dwFriendID = this.TeamMemberContext.dwActorID
			});
		}
	}
}