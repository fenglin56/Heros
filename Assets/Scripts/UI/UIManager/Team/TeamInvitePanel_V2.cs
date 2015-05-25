using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Friend;

namespace UI.Team
{

	public class TeamInvitePanel_V2 : MonoBehaviour 
	{
		public GameObject InviteFriendItemPrefab;
		public LocalButtonCallBack Button_Close;
		public GameObject Tip_NoneFriend;

		public UIDraggablePanel Draggable;
		public UIGrid Grid;

		//public ItemPagerManager ItemPagerManager_Friend;
		//public LocalButtonCallBack ClosePanelButton;
		//public TeamFriendInfoItem TeamFriendInfoItemPrefab;
		
		private Transform m_thisTransform;
		
		private List<TeamInviteFriendItem> m_friendItemList = new List<TeamInviteFriendItem>();
		
		void Awake()
		{
			Button_Close.SetCallBackFuntion(OnClosePanelButtonClick, null);
			m_thisTransform = this.transform;

		}

		public void Show()
		{
			m_thisTransform.localPosition = Vector3.zero;
			ShowFriendList();
		}

		public void Close()
		{
			m_thisTransform.localPosition = Vector3.back * 800;
		}

		public void FilterFriendList()
		{
			var teamMemberData = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.ToList();
			for(int i = 0; i < m_friendItemList.Count; i++)
			{
				if(teamMemberData.Exists(k => k.TeamMemberContext.dwActorID == m_friendItemList[i].FriendID))
			    {
					var item = m_friendItemList[i];
					m_friendItemList.RemoveAt(i);
					Destroy( item.gameObject);
				}
			}

			StartCoroutine("LateGridReposition");
		}

		private void ShowFriendList()
		{
			//避免下发消息先到问题
			if (m_thisTransform == null)
				return;
			
			if (!m_thisTransform.gameObject.activeInHierarchy)
			{
				m_thisTransform.gameObject.SetActive(true);
			}

			List<SMsgTeamPropMember_SC> teamMemberData = new List<SMsgTeamPropMember_SC>();
			if(TeamManager.Instance.IsTeamExist())
			{
				teamMemberData = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.ToList();
			}
			List<PanelElementDataModel> friendList =  new List<PanelElementDataModel>();			
			List<PanelElementDataModel> onlineFreindList = new List<PanelElementDataModel>();

//			var friendQueue = FriendDataManager.Instance.GetFriendListData.Where(p => ((PanelElementDataModel)p).sMsgRecvAnswerFriends_SC.bOnLine == 1).ToList();
//			for (int i = 0; i < friendQueue.Count; i++)
//			{
//				onlineFreindList.Add((PanelElementDataModel)friendQueue[i]);
//			}
			onlineFreindList.AddRange(FriendDataManager.Instance.GetOnlineFriendList);        
			onlineFreindList.ApplyAllItem(p =>
			                              {
				if (!teamMemberData.Exists(k => k.TeamMemberContext.dwActorID == p.sMsgRecvAnswerFriends_SC.dwFriendID))
				{
					if(!friendList.Any(j=>j.sMsgRecvAnswerFriends_SC.dwFriendID == p.sMsgRecvAnswerFriends_SC.dwFriendID))
				    {
						friendList.Add(p);
					}
				}
			});				
			
			m_friendItemList.ApplyAllItem(p =>
			                              {                
				Destroy(p.gameObject);
			});
			m_friendItemList.Clear();

			Tip_NoneFriend.SetActive(friendList.Count == 0);

			//排序
			friendList.Sort(new FriendComparerByCombat());

			var teamProp = TeamManager.Instance.MyTeamProp;
			int ectypeID = EctypeConfigManager.Instance.EctypeSelectConfigList[teamProp.TeamContext.dwEctypeId]._vectContainer[teamProp.TeamContext.dwEctypeIndex - 1];
			EctypeContainerData ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
			for (int i = 0; i < friendList.Count; i++)
			{
				AddTeamFriendItem();
				m_friendItemList[i].UpdateInfo(friendList[i], ectypeData);
			}
		
			StartCoroutine("LateGridReposition");
			
			//UIGridTrans.repositionNow = true;   //排列
		}

		IEnumerator LateGridReposition()
		{
			yield return new WaitForEndOfFrame();
			Grid.Reposition();
		}

		public bool IsShow()
		{
			return m_thisTransform.localPosition == Vector3.zero;
		}

		private void AddTeamFriendItem()
		{
			GameObject friendItem = UI.CreatObjectToNGUI.InstantiateObj(InviteFriendItemPrefab,Grid.transform);				
			TeamInviteFriendItem teamInviteFriendItem = friendItem.GetComponent<TeamInviteFriendItem>();
			teamInviteFriendItem.transform.localPosition -= Vector3.forward*12; //\			
			m_friendItemList.Add(teamInviteFriendItem);
		}


//		private void AddTeamFriendItem()
//		{
//			TeamFriendInfoItem item = ((GameObject)Instantiate(TeamFriendInfoItemPrefab.gameObject)).GetComponent<TeamFriendInfoItem>();
//			item.transform.parent = ItemPagerManager_Friend.transform;
//			item.transform.localScale = Vector3.one;
//			item.transform.localPosition -= Vector3.forward*12; //\
//			
//			m_friendItemList.Add(item);
//		}
		
		void OnClosePanelButtonClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamInvitationClose");
			Close();
		}

		public class FriendComparerByCombat : IComparer<PanelElementDataModel>
		{
			public int Compare(PanelElementDataModel x, PanelElementDataModel y)
			{
				int compareResult = 1;
				
				if(x.sMsgRecvAnswerFriends_SC.dwFight > y.sMsgRecvAnswerFriends_SC.dwFight)
				{
					compareResult = -1;
				}
				
				return compareResult;
			}
		}
	
	}
}