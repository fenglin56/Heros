using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.Friend;
using UI;
using System.Linq;

public class PvpDataManager  {


	private static PvpDataManager instance;
	public static PvpDataManager Instance
	{
		get
		{
			if(instance==null)
			{
				instance=new PvpDataManager();
			}
			return instance;
		}
	}
	private PvpDataManager()
	{

	}
	//是否在pvp界面
	public bool IsInPvpView;
	public bool IsTeamLeader;
	public int MyActorID;
	public PVPMatchState CurrentMatchState;
	public PVPViewState CurrentViewState=PVPViewState.NotInPage;
	private List<SGroupMemberInfo> GroupmeberInfoList=new List<SGroupMemberInfo>();

	public void SetGroupMeberInfo(SMsgPVPMathingSynInfo_SC sMsgPVPMathingSynInfo_SC )
	{
		///如果自己在队伍中要弹出提示有人加入队伍

		if(IfGroupmeberInfoListContain((uint)MyActorID))
		{
			ShowMessage(PVPMessageType.JoinTeam,sMsgPVPMathingSynInfo_SC.GroupMemberInfo[0].dwActorID);
		}
		//如果没有在队伍中则加入本地队伍缓存
		sMsgPVPMathingSynInfo_SC.GroupMemberInfo.ApplyAllItem(c=>{
			if(!IfGroupmeberInfoListContain(c.dwActorID))
			{
			GroupmeberInfoList.Add(c);
			}});

		UIEventManager.Instance.TriggerUIEvent(UIEventType.pvpSyncTeam,null);
		///如果当前是默认状态则当前状态改变成组队状态
		if(CurrentMatchState==PVPMatchState.defult)
		{
			CurrentMatchState=PVPMatchState.Team;
		}
	}  

	void ShowMessage(PVPMessageType type, uint actorID)
	{
		if(actorID!=0)
		{
			if(MessageBox.Instance!=null)
			{
				var friend=	FriendDataManager.Instance.GetFriendListData.SingleOrDefault(c=>c.sMsgRecvAnswerFriends_SC.dwFriendID==actorID).sMsgRecvAnswerFriends_SC;
				//PlayerDataManager.Instance.PlayerBasePropConfigDat
				var profession=PlayerDataManager.Instance.GetProfessionConfigData((int)friend.dProfession);
				switch(type)
				{
				case PVPMessageType.JoinTeam:
				{
					string text=string.Format(LanguageTextManager.GetString("IDS_I38_5"),friend.Name);
					MessageBox.Instance.ShowTips(1,text,1.0f);
				}
					break;
				case PVPMessageType.CancelTeam:
				{
					string text=string.Format(LanguageTextManager.GetString("IDS_I38_4"),friend.Name);
					MessageBox.Instance.ShowTips(1,text,1.0f);
				}
					break;
				case PVPMessageType.Invite:
				{
					string text=string.Format(LanguageTextManager.GetString("IDS_I38_7"),friend.Name,friend.sActorLevel,LanguageTextManager.GetString(profession._professionName));
					MessageBox.Instance.Show(1,"",text,LanguageTextManager.GetString("IDS_H2_22"),LanguageTextManager.GetString("IDS_H2_15"),()=>{MessageBox.Instance.CloseMsgBox();},()=>{ConfirmFriend(actorID);});
				}
					break;
				}

			}
		}
	}

	bool IfGroupmeberInfoListContain(uint ActorId)
	{
		bool flag=false;
	    foreach(var item in GroupmeberInfoList)
		{
			if(item.dwActorID==ActorId)
			{
				flag=true;
				break;
			}
		}
		return flag;
	}
	public void SetCancelTeamInfo(SMsgPVPMathingLeave_SC message)
	{
		for(int i=0;i<GroupmeberInfoList.Count;i++)
		{
			if(GroupmeberInfoList[i].dwActorID==message.dwActorID)
			{
				GroupmeberInfoList.Remove(GroupmeberInfoList[i]);
			}
		}
	
		if(MyActorID==message.dwActorID)
		{
			IsTeamLeader=true;
		}
		else
		{
			IsTeamLeader=false;
		}

		ShowMessage(PVPMessageType.CancelTeam,message.dwActorID);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.pvpFriendCancelTeam,(int)message.dwActorID);
	}

	public List<SGroupMemberInfo> GetGroupmeberInfoList()
	{
		return GroupmeberInfoList;
	}


	public void ClearGroupmeberInfoList()
	{
		GroupmeberInfoList.Clear();
	}


	public void PvpStartMatch()
	{
		NetServiceManager.Instance.TeamService.PVPBeginMatch();
	}


	public void CancelMatch()
	{
		NetServiceManager.Instance.TeamService.PVPCancelMatch();
	}

	public void InviteFriend(uint actorID)
	{
		NetServiceManager.Instance.TeamService.PvpSendInviteFriend(actorID);
	}

	/// <summary>
	/// 接受pvp邀请
	/// </summary>
	public void ConfirmFriend(uint inviterID)
	{
		NetServiceManager.Instance.TeamService.PvpSendConfirmFriend(inviterID);
	}
	public void CancelTeam()
	{
		//NetServiceManager.Instance.TeamService.PVPCancelTeam();
		NetServiceManager.Instance.TeamService.PVPCancelMatch();
		ClearGroupmeberInfoList();
	}

	public void ReceiveInviteHandl(SMsgPvpInviteFriend_CSC msg)
	{
	
		ShowMessage(PVPMessageType.Invite,msg.dwActorID);
	}
}
public enum PVPMatchState
{
	defult,//如果是主动进入默认为defult
	Team,//队伍信息变更后状态为teamr 比如 被邀请进入或者有玩家加入状态为team
	Match,//匹配开始匹配后
}
public enum PVPViewState
{
	NotInPage,//不在青龙会界面
	InPage,//离开，主动离开
}
public enum PVPMessageType
{
	JoinTeam,
	CancelTeam,
	Invite,
}