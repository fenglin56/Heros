using UnityEngine;
using System.Collections;

namespace UI.Team
{
	public class WorldTeamItem : MonoBehaviour 
	{
		public UISprite UI_EctypeIcon;
		public Transform EctypeIconPoint;	
		public GameObject m_EctypeIcon;
		public UILabel Label_EctypeName;
		public UILabel Label_EctypeHard;
		public UILabel Label_Level;
		public WorldTeamMemberItem[] TeamMemberItems = new WorldTeamMemberItem[2];
		public LocalButtonCallBack Button_Join;

		private int mTeamID;
		private SMsgTeamProp_SC SMsgTeamProp;

		void Awake()
		{
			////TODO GuideBtnManager.Instance.RegGuideButton(ButtonJoinTeam.gameObject, MainUI.UIType.TeamInfo, SubUIType.JoinTeam, out m_guideBtnID);
		}
		
		public void Start()
		{
			Button_Join.SetCallBackFuntion(OnJoinTeamClick);
		}
		
		public void InitInfo(int teamID)
		{
			//transform.parent = guidParent;
			//transform.localPosition = Vector3.zero;
			//transform.localScale = Vector3.one;
			
			this.mTeamID = teamID;
		}
		
		//public void InitInfo(SMsgTeamProp_SC sMsgTeamProp, Transform guidParent)
		//{
		//    transform.parent = guidParent;
		//    transform.localPosition = Vector3.zero;
		//    transform.localScale = Vector3.one;
		
		//    this.SMsgTeamProp = sMsgTeamProp;
		//}
		
		public void UpdateInfo(SMsgTeamProp_SC sMsgTeamProp)
		{
			this.SMsgTeamProp = sMsgTeamProp;
			
			if (!gameObject.activeInHierarchy)
			{
				gameObject.SetActive(true);
			}
			//\read world team info
			
			//\读取队员信息   
			int memberNum = SMsgTeamProp.TeamMemberNum_SC.wMemberNum;   
			
			int listLength = TeamMemberItems.Length;
//			if (memberNum > TeamMemberItems.Length)
//			{
//				for (int i = listLength; i < memberNum; i++)
//				{
//					TeamMemberItem item = ((GameObject)Instantiate(ATeamMemberItem.gameObject)).GetComponent<TeamMemberItem>();
//					item.InitInfo(MemberTrans[i]);
//					TeamMemberItemList.Add(item);
//				}                                
//			}
			int memberNo = 0;
			TeamMemberItems.ApplyAllItem(p => 
			                                {
				if(memberNo<memberNum)
				{
					//p.UpdateInfo(SMsgTeamProp.);
					p.UpdateInfo(SMsgTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers[memberNo]);
				}
				else
				{
					p.Close();
				}
				memberNo++;
			});

			if(m_EctypeIcon != null)
			{
				Destroy(m_EctypeIcon);
			}
			int ectypeID = EctypeConfigManager.Instance.EctypeSelectConfigList[sMsgTeamProp.TeamContext.dwEctypeId]._vectContainer[sMsgTeamProp.TeamContext.dwEctypeIndex - 1];
			var containerIconData = TownEctypeResDataManager.Instance.GetEctypeContainerResData(ectypeID);
			if(containerIconData.EctypeIconPrefab!=null)
			{
				m_EctypeIcon = UI.CreatObjectToNGUI.InstantiateObj(containerIconData.EctypeIconPrefab, EctypeIconPoint);
				m_EctypeIcon.transform.localScale = m_EctypeIcon.transform.localScale;
			}
			var ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
			Label_EctypeName.text = LanguageTextManager.GetString( ectypeData.lEctypeName);
			string hardStr = sMsgTeamProp.TeamContext.byEctypeDifficulty == 0 ? "IDS_I13_10" : "IDS_I13_11";
			Label_EctypeHard.text = LanguageTextManager.GetString(hardStr);
			Label_Level.text = ectypeData.lMinActorLevel.ToString()+ LanguageTextManager.GetString("IDS_H1_156");

			//var ectypeConfig = EctypeConfigManager.Instance.EctypeSelectConfigList.SingleOrDefault(p => p.Value._lEctypeID == SMsgTeamProp.TeamContext.dwEctypeId).Value;
			//if (ectypeConfig != null)
			//    TranscriptName.text = LanguageTextManager.GetString(ectypeConfig._szName);
			//\临时
			//string difficultyName = "简单";
			//switch (SMsgTeamProp.TeamContext.byEctypeDifficulty)
			//{
			//    case 1:
			//        difficultyName = "普通";
			//        break;
			//}
			//DifficultyName.text = difficultyName;
			
			
			transform.name = "Team" + (mTeamID+100).ToString();
		}

		public void PlayAppearAnimation(float delayTime)
		{
			transform.localScale = new Vector3(1f,0.01f,1f);
			StartCoroutine(LateAppear(delayTime));
		}
		IEnumerator LateAppear(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			TweenScale.Begin(gameObject,0.1f,Vector3.one);
		}

		public void PlayShutterAnimation(float delayTime)
		{
			StopAllCoroutines();
			//StartCoroutine(TweenPosition(targetTrans, startPos, endPos, duration));
			transform.localPosition = new Vector3(-2000, 0, 0);
			
			StartCoroutine(ShutterAnimation(delayTime));
		}
		
		IEnumerator ShutterAnimation(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			transform.localPosition = Vector3.zero;
			var tweenSceleArray = gameObject.GetComponentsInChildren<TweenScale>();
			
			tweenSceleArray.ApplyAllItem(p =>
			                             {
				p.Reset();
				p.Play(true);
			});
			
			//Vector3 from = tweenSceleArray[0].from;
			//Vector3 to = tweenSceleArray[0].to;
			
			yield return new WaitForSeconds(tweenSceleArray[0].duration);
			
			tweenSceleArray[0].Play(false);
		}
		
		public void Close()
		{
			gameObject.SetActive(false);
		}
		
		//IEnumerator TweenPosition(Transform targetTrans, Vector3 startPos, Vector3 endPos, float duration)
		//{
		//    float i = 0;
		//    float rate = 1.0f / duration;
		//    while (i < 1.0)
		//    {
		//        i += Time.deltaTime * rate;
		//        targetTrans.position = Vector3.Lerp(startPos, endPos, i);
		//        yield return null;
		//    }
		//}
		
		void OnDestroy()
		{
			////TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
		}
		
		void OnJoinTeamClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamJoin");

			int ectypeID = EctypeConfigManager.Instance.EctypeSelectConfigList[SMsgTeamProp.TeamContext.dwEctypeId]._vectContainer[SMsgTeamProp.TeamContext.dwEctypeIndex - 1];
			var ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
			if(ectypeData.lCostType == 1)
			{
				int costEnergy = int.Parse(ectypeData.lCostEnergy);
				if(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE < costEnergy)
				{
					PopupObjManager.Instance.ShowAddVigour();
					return;
				}
			}

			var playerData = PlayerManager.Instance.FindHeroDataModel();
			if(JudgeAndExitTeam())
			{
				TeamManager.Instance.SetWaitExitTeamAction(()=>{
					NetServiceManager.Instance.TeamService.SendTeamMemberJoinMsg(new SMsgTeamMemberJoin_CS(){
						byJoinType = 0,
						byJoinAnswer = 1,
						dwTeamID = SMsgTeamProp.TeamContext.dwId,
						dwActorID = (uint)playerData.ActorID,
					});
					Destroy(gameObject);
					UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowTeamChildPanel, ChildPanel.RepositionTeamList);
				});
			}
			else
			{
				NetServiceManager.Instance.TeamService.SendTeamMemberJoinMsg(new SMsgTeamMemberJoin_CS(){
					byJoinType = 0,
					byJoinAnswer = 1,
					dwTeamID = SMsgTeamProp.TeamContext.dwId,
					dwActorID = (uint)playerData.ActorID,
				});
				Destroy(gameObject);
				UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowTeamChildPanel, ChildPanel.RepositionTeamList);
			}

		}

		private bool JudgeAndExitTeam()
		{
			bool isExit = false;
			if(TeamManager.Instance.IsTeamExist())
			{
				if(TeamManager.Instance.GetCurrentEctypeType() == 0 || TeamManager.Instance.GetCurrentEctypeType() == 9)
				{
					isExit = true;
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
			return isExit;
		}


	}
}