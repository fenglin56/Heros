using UnityEngine;
using System.Collections;
using UI;
using System.Linq;

public class TeamService : Service
{

    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        Package package;
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        package = PackageHelper.ParseReceiveData(dataBuffer);

        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_TEAM:
                {
                    switch (package.GetSubMsgType())
                    {
                        case TeamDefineManager.MSG_TEAM_TEAMLIST:
                            this.AddToInvoker(this.ReceiveTeamListHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_CREATE:
                            this.AddToInvoker(this.ReceiveTeamCreateHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_DISBAND:
                            this.AddToInvoker(this.ReceiveTeamDisbandHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_UPDATEPROP:
                            this.AddToInvoker(this.ReceiveTeamUpdatePropHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_MEMBER_UPDATEPROP:
                            this.AddToInvoker(this.ReceiveTeamMemberUpdatePropHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_MEMBER_JOIN:
                            this.AddToInvoker(this.ReceiveTeamMemberJoinHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_CAPTAIN_JOIN:
                            this.AddToInvoker(this.ReceiveTeamMemberJoinHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_MEMBER_LEAVE:
                            this.AddToInvoker(this.ReceiveTeamMemberLeaveHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_MEMBER_KICK:
                            this.AddToInvoker(this.ReceiveMemberKickHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_MEMBER_INVITE:
                            this.AddToInvoker(this.ReceiveTeamMemberInviteHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_MEMBER_READY:
                            this.AddToInvoker(this.ReceiveMemberReadyHandle, package.Data, socketId);
                            break;
                        case TeamDefineManager.MSG_TEAM_ERROR_CODE:
                            this.AddToInvoker(this.ReceiveTeamErrorCodeHandle, package.Data, socketId);
                            break;    
                        case TeamDefineManager.MSG_TEAM_INVITER_FAILED:
                            this.AddToInvoker(this.ReceiveTeamInviterFailedHandle, package.Data, socketId);
                            break;
						case TeamDefineManager.MSG_GET_TEAMNUMLIST:
							this.AddToInvoker(this.ReceiveCrusadeTeamNumListHandle, package.Data, socketId);
							break;				
						case TeamDefineManager.MSG_MATCHING_CANCEL:
							this.AddToInvoker(this.ReceiveMatchingCancelHandle, package.Data, socketId);
							break;
						case TeamDefineManager.MSG_MATCHING_CONFIRM:
							this.AddToInvoker(ConfirmMatchindHandle, package.Data, socketId);
							break;
						case TeamDefineManager.MSG_PVP_INVITE_FRIEND://收到邀请PvpReceiveInviteFriendHandle
							this.AddToInvoker(PvpReceiveInviteFriendHandle, package.Data, socketId);
							break;
						case TeamDefineManager.MSG_PVP_ENTER_CONFIRM://收到加入成功
							this.AddToInvoker(PvpReceiveEnterConfirmHandle, package.Data, socketId);
							break;
						case TeamDefineManager.MSG_PVP_MATCHING_BEGIN://收到开始匹配通知
							this.AddToInvoker(PvpReceiveBeginMatchHandle, package.Data, socketId);
							break;
						case TeamDefineManager.MSG_PVP_MATCHING_CANCEL://收到取消匹配通知
							this.AddToInvoker(PvpReceiveCancelMatchHandle, package.Data, socketId);
							break;
						case TeamDefineManager.MSG_PVP_MATCHING_LEAVE://收到队友离开通知
							this.AddToInvoker(PvpReceiveCancelTeamHandle, package.Data, socketId);
							break;
						case TeamDefineManager.MSG_PVP_MATCHING_SYNINFO://收到同步队伍通知
							this.AddToInvoker(PvpMatchindSyncHandle, package.Data, socketId);
							break;
						case TeamDefineManager.MSG_PVP_MATCHING_SUCESS://匹配成功
							this.AddToInvoker(PvpMatchindSyncHandle, package.Data, socketId);
							break;
                    }
                } break;            
            default:
                //TraceUtil.Log("不能识别的主消息:" + package.GetMasterMsgType());
                break;
        }
    }

    
    #region 接收

    CommandCallbackType ReceiveTeamListHandle(byte[] dataBuffer, int socketId)
    {
        SMsgTeamNum_SC sTeamNum = SMsgTeamNum_SC.ParsePackage(dataBuffer);

        //sTeamNum.SMsgTeamProps.ApplyAllItem(p=>
        //  {
        //      TeamManager.Instance.RegisteMember(p);
        //  }           
        //);

        RaiseEvent(EventTypeEnum.TeamList.ToString(), sTeamNum);

        return CommandCallbackType.Continue;
    }
    //队伍创建
    CommandCallbackType ReceiveTeamCreateHandle(byte[] dataBuffer, int socketId)
    {
        SMsgTeamNum_SC sTeamNum = SMsgTeamNum_SC.ParsePackage(dataBuffer);

        //TraceUtil.Log("创建队伍: 队伍数量 = " + sTeamNum.wTeamNum);
        //sTeamNum.SMsgTeamProps.ApplyAllItem(p =>
        //    {
        //        //TraceUtil.Log("队员数量 = "+p.TeamMemberNum_SC.wMemberNum);
        //        //TraceUtil.Log("队长id = "+p.TeamContext.dwCaptainId + " 队伍id = "+p.TeamContext.dwId);                
        //        p.TeamMemberNum_SC.SMsgTeamPropMembers.ApplyAllItem(k =>
        //            {
        //                //TraceUtil.Log("队员dwActorID = " + k.TeamMemberContext.dwActorID);
        //                //TraceUtil.Log("队员 头像 = " + k.TeamMemberContext.byKind);
        //            });
        //    });

        RaiseEvent(EventTypeEnum.TeamCreate.ToString(), sTeamNum);

        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveTeamDisbandHandle(byte[] dataBuffer, int socketId)
    {
        SMsgTeamDisband_SC sTeamDisband = SMsgTeamDisband_SC.ParsePackage(dataBuffer);
		if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)		
		{
			MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I13_44"),1f);
		}
        RaiseEvent(EventTypeEnum.TeamDisband.ToString(), sTeamDisband);
		TeamManager.Instance.DoWaitExitTeamAction();
        return CommandCallbackType.Continue;   
    }

    CommandCallbackType ReceiveTeamUpdatePropHandle(byte[] dataBuffer, int socketId)
    {
        SMsgTeamUpdateProp_SC sTeamUpdateProp = SMsgTeamUpdateProp_SC.ParsePackage(dataBuffer);
        //TraceUtil.Log("队伍更新: index=" + sTeamUpdateProp.wProp + " , value=" + sTeamUpdateProp.nValue);

        //有更新成功再raise消息
        if (TeamManager.Instance.UpdateTeamValue(sTeamUpdateProp))
        {
            //\服务器下发有误，暂时注销
            RaiseEvent(EventTypeEnum.TeamUpdateProp.ToString(), sTeamUpdateProp);
        }
        
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveTeamMemberUpdatePropHandle(byte[] dataBuffer, int socketId)
    {
        SMsgTeamMemberUpdateProp_SC sTeamMemberUpdateProp = SMsgTeamMemberUpdateProp_SC.ParsePackage(dataBuffer);

        TeamManager.Instance.UpdateTeamMemberValue(sTeamMemberUpdateProp);
		RaiseEvent(EventTypeEnum.TeamMemberUpdateProp.ToString(), sTeamMemberUpdateProp);
        return CommandCallbackType.Continue;
    }
    //队员加入
    CommandCallbackType ReceiveTeamMemberJoinHandle(byte[] dataBuffer, int socketId)
    {
        SMsgTeamNum_SC sTeamNum = SMsgTeamNum_SC.ParsePackage(dataBuffer);

        if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
        {
			if(!TeamManager.Instance.IsTeamExist())
			{
				UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.TeamInfo, 1);
			}            

			//提示
			if(TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.wMemberNum > 0)
			{
				TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.ApplyAllItem(p=>{
					sTeamNum.SMsgTeamProps[0].TeamMemberNum_SC.SMsgTeamPropMembers.ApplyAllItem(k=>{
						if(k.TeamMemberContext.dwActorID != p.TeamMemberContext.dwActorID)
						{
							MessageBox.Instance.ShowTips(4,string.Format(LanguageTextManager.GetString("IDS_I13_28"),k.TeamMemberContext.szName),1f);
						}
					});
				});
			}
            RaiseEvent(EventTypeEnum.TeamCreate.ToString(), sTeamNum);
        }
        else
        {
            //副本重连情况下
            //TraceUtil.Log("======副本重连情况下=======");
            //var teamProp = sTeamNum.SMsgTeamProps[0];
            //TeamManager.Instance.RegisteTeam(teamProp);
        }
        var teamProp = sTeamNum.SMsgTeamProps[0];
        TeamManager.Instance.RegisteTeam(teamProp);
        return CommandCallbackType.Continue;
    } 
    //队员离开
    CommandCallbackType ReceiveTeamMemberLeaveHandle(byte[] dataBuffer, int socketId)
    {
        SMsgTeamMemberLeave_SC sTeamMemberLeave = SMsgTeamMemberLeave_SC.ParsePackage(dataBuffer);
        //TraceUtil.Log("离开: dwActorID = " + sTeamMemberLeave.dwActorID + " dwTeamID = " + sTeamMemberLeave.dwTeamID);
        //更新队伍信息
		if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
		{
			if(PlayerManager.Instance.FindHeroDataModel().ActorID != sTeamMemberLeave.dwActorID)
			{
				var memberProp = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.SingleOrDefault(p=>p.TeamMemberContext.dwActorID==sTeamMemberLeave.dwActorID);
				MessageBox.Instance.ShowTips(4,string.Format(LanguageTextManager.GetString("IDS_I13_42"),memberProp.TeamMemberContext.szName),1f);
			}
		}

		TeamManager.Instance.RemoveMember(sTeamMemberLeave.dwActorID);
		//Edit by lee 更新战斗ui
        UIEventManager.Instance.TriggerUIEvent(UIEventType.TeamMemberLeave, null);
        RaiseEvent(EventTypeEnum.TeamMemberLeave.ToString(), sTeamMemberLeave);
		TeamManager.Instance.DoWaitExitTeamAction();
        return CommandCallbackType.Continue;
    }
    //队员踢出
    CommandCallbackType ReceiveMemberKickHandle(byte[] dataBuffer, int socketId)
    {
        SMsgTeamMemberLeave_SC sTeamMemberLeave = SMsgTeamMemberLeave_SC.ParsePackage(dataBuffer);
		if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
		{
			var memberProp = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.SingleOrDefault(p=>p.TeamMemberContext.dwActorID==sTeamMemberLeave.dwActorID);
			MessageBox.Instance.ShowTips(4,string.Format(LanguageTextManager.GetString("IDS_I13_30"),memberProp.TeamMemberContext.szName),1f);
		}
        //更新队伍信息
        TeamManager.Instance.RemoveMember(sTeamMemberLeave.dwActorID);
        //TraceUtil.Log("踢出: dwActorID = " + sTeamMemberLeave.dwActorID + " dwTeamID = " + sTeamMemberLeave.dwTeamID);
        RaiseEvent(EventTypeEnum.TeamMemberBeKick.ToString(), sTeamMemberLeave);
        return CommandCallbackType.Continue;
    }
    //队员被邀请
    CommandCallbackType ReceiveTeamMemberInviteHandle(byte[] dataBuffer, int socketId)
    {        
        temporaryTeamMemberInvite = SMsgTeamMemberInvite_SC.ParsePackage(dataBuffer);

        if (TaskModel.Instance.TaskGuideType!=TaskGuideType.Enforce)// NewbieGuideManager_V2.Instance.IsConstraintGuide)
        {
            this.TeamMemberInviteHandle();
        }        
        return CommandCallbackType.Continue;
    }

    #region 受邀请弹窗特别处理
    SMsgTeamMemberInvite_SC temporaryTeamMemberInvite;
    public SMsgTeamMemberInvite_SC GetTeamMemberInvite()
    {
        return temporaryTeamMemberInvite;
    }
    private void TeamMemberInviteHandle()
    {        
        //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"selectID = " + selectID + " , diff = " + diff + " , ectypeID = " + ectypeID);        
		if(!GameManager.Instance.CreateEntityIM)
			return;

        var friendData = UI.Friend.FriendDataManager.Instance.GetFriendListData.SingleOrDefault(p => temporaryTeamMemberInvite.dwActorID == p.sMsgRecvAnswerFriends_SC.dwFriendID);
        if (friendData == null)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"friendList is null");
            return;
        }

        string name = System.Text.Encoding.UTF8.GetString(friendData.sMsgRecvAnswerFriends_SC.szName);
		int ectypeContainerID = EctypeConfigManager.Instance.EctypeSelectConfigList[(int)temporaryTeamMemberInvite.dwEctypeId]._vectContainer[temporaryTeamMemberInvite.byEctypeIndex-1];
		var ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeContainerID];
		string diffStr = temporaryTeamMemberInvite.byEctypDiff == 0? "IDS_I13_10":"IDS_I13_11";
		if(MessageBox.Instance!=null)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_TeamInvitationTips");
			MessageBox.Instance.CloseMsgBox();
//			MessageBox.Instance.Show(4, "", string.Format(LanguageTextManager.GetString("IDS_H1_92"), "[7bb6c2]" + name + "[-]", ProfessionTrans(friendData.sMsgRecvAnswerFriends_SC.dProfession),
//			                                              friendData.sMsgRecvAnswerFriends_SC.sActorLevel), LanguageTextManager.GetString("IDS_H2_16"), LanguageTextManager.GetString("IDS_H2_22"), SureJoinTargetTeam, RejectJoinTargetTeam);
			string str = string.Format( LanguageTextManager.GetString("IDS_I13_39"),name,LanguageTextManager.GetString(ectypeData.lEctypeName),LanguageTextManager.GetString(diffStr));
			MessageBox.Instance.Show(4,"",str,LanguageTextManager.GetString("IDS_H2_22"), LanguageTextManager.GetString("IDS_H2_16"),RejectJoinTargetTeam,SureJoinTargetTeam);
		}
		   
    }

    
    public void SureJoinTargetTeam()
    {
		SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamInvitationAccept");
        NetServiceManager.Instance.TeamService.SendTeamMemberJoinMsg(new SMsgTeamMemberJoin_CS()
        {
            byJoinType = 3,
            byJoinAnswer = 1,
            dwTeamID = temporaryTeamMemberInvite.dwTeamID,
            dwActorID = temporaryTeamMemberInvite.dwTargetActorID
        });
        //UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UI.MainUI.UIType.Empty);
        
        //if (NewbieGuideManager_V2.Instance.IsEctypeGuide)
        //{
        //    Guide.TownGuideUIManger_V2.Instance.DelGuideArrow();
        //}
    }
    void RejectJoinTargetTeam()
    {
		SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamInvitationRefuse");
        //NetServiceManager.Instance.TeamService.SendTeamMemberJoinMsg(new SMsgTeamMemberJoin_CS()
        //{
        //    byJoinType = 0,
        //    byJoinAnswer = 0,
        //    dwTeamID = temporaryTeamMemberInvite.dwTeamID,
        //    dwActorID = temporaryTeamMemberInvite.dwTargetActorID
        //});
    }
    #endregion


    //队员准备结果
    CommandCallbackType ReceiveMemberReadyHandle(byte[] dataBuffer, int socketId)
    {
        SMsgTeamMemberReadyResult_SC readyResult = SMsgTeamMemberReadyResult_SC.ParsePackage(dataBuffer);
        RaiseEvent(EventTypeEnum.TeamMemberReady.ToString(), readyResult);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveTeamErrorCodeHandle(byte[] dataBuffer, int socketId)
    {
        SMsgTeamErrorCode_CS sTeamErrorCode = SMsgTeamErrorCode_CS.ParsePackage(dataBuffer);
        RaiseEvent(EventTypeEnum.TeamErrorCode.ToString(), sTeamErrorCode);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveTeamInviterFailedHandle(byte[] dataBuffer, int socketId)
    {
       SMsgTeamInviteError_SC sMsgTeamInviteError_SC = SMsgTeamInviteError_SC.ParsePackage(dataBuffer);
       TraceUtil.Log( SystemModel.Lee, "[sMsgTeamInviteError_SC.dwErrorMsg]" + sMsgTeamInviteError_SC.dwErrorMsg);
       if (sMsgTeamInviteError_SC.dwErrorMsg == SystemErrorCodeDefine.TEAM_CODE_TEAM_REJECT)
       {
           //受邀好友拒绝
           var friendList = UI.Friend.FriendDataManager.Instance.GetFriendListData;
           var friendData = friendList.SingleOrDefault(p => p.sMsgRecvAnswerFriends_SC.dwFriendID == sMsgTeamInviteError_SC.dwTargetActorID);
           if (friendData != null)
           {
				if(MessageBox.Instance!=null)
				{
					MessageBox.Instance.ShowTips(4, string.Format(LanguageTextManager.GetString("IDS_H1_504"), friendData.sMsgRecvAnswerFriends_SC.Name), 1);
				}               
           }
       }
       else
       {
           var friendList = UI.Friend.FriendDataManager.Instance.GetFriendListData;
           var friendData = friendList.SingleOrDefault(p => p.sMsgRecvAnswerFriends_SC.dwFriendID == sMsgTeamInviteError_SC.dwTargetActorID);
           if (friendData != null)
           {
				if(MessageBox.Instance!=null)
				{
					MessageBox.Instance.ShowTips(4, string.Format(LanguageTextManager.GetString("IDS_H1_505"), friendData.sMsgRecvAnswerFriends_SC.Name), 1);
				}               
           }
       }
       return CommandCallbackType.Continue;
    }

	CommandCallbackType ReceiveCrusadeTeamNumListHandle(byte[] dataBuffer, int socketID)
	{
		SMsgEctypeTeamNum_SC sMsgEctypeTeamNum_SC = SMsgEctypeTeamNum_SC.ParsePackage(dataBuffer);
		TraceUtil.Log(SystemModel.Lee, TraceLevel.Verbose,"sMsgEctypeTeamNum_SC : "+sMsgEctypeTeamNum_SC.byEctypeNum);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeTeamNum ,sMsgEctypeTeamNum_SC);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveMatchingCancelHandle(byte[] dataBuffer, int socketID)
	{
		UIEventManager.Instance.TriggerUIEvent(UIEventType.RandomMatchingCancel ,null);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ConfirmMatchindHandle(byte[] dataBuffer, int socketID)
	{
		SMsgConfirmMatching_SC sMsgConfirmMatching_SC = SMsgConfirmMatching_SC.ParsePackage(dataBuffer);
		UIEventManager.Instance.TriggerUIEvent( UIEventType.CrusadeMatching,sMsgConfirmMatching_SC);
		return CommandCallbackType.Continue;
	}

	/// <summary>
	/// 收到加入队伍成功通知
	/// </summary>
	/// <returns>The receive enter confirm handle.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType PvpReceiveEnterConfirmHandle(byte[] dataBuffer, int socketID)
	{
		SMsgPVPFriendConfirm_CS sMsgConfirmMatching_SC = SMsgPVPFriendConfirm_CS.ParsePackage(dataBuffer);
		//UIEventManager.Instance.TriggerUIEvent( UIEventType.CrusadeMatching,sMsgConfirmMatching_SC);
		return CommandCallbackType.Continue;
	}

	/// <summary>
	/// 收到开始匹配通知
	/// </summary>
	/// <returns>The receive begin match handle.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType PvpReceiveBeginMatchHandle(byte[] dataBuffer, int socketID)
	{
		UIEventManager.Instance.TriggerUIEvent( UIEventType.pvpStartmatch,null);
		return CommandCallbackType.Continue;
	}
	/// <summary>
	///接受到取消匹配通知 
	/// </summary>
	/// <returns>The receive cancel match handle.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType PvpReceiveCancelMatchHandle(byte[] dataBuffer, int socketID)
	{
		SMsgPVPMathingLeave_SC sMsgPVPMathingLeave_SC = SMsgPVPMathingLeave_SC.ParsePackage(dataBuffer);
		PvpDataManager.Instance.SetCancelTeamInfo(sMsgPVPMathingLeave_SC);
		return CommandCallbackType.Continue;
	}
	/// <summary>
	/// 收到好友邀请
	/// </summary>
	/// <returns>The receive invite friend handle.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType PvpReceiveInviteFriendHandle(byte[] dataBuffer, int socketID)
	{
		SMsgPvpInviteFriend_CSC sMsgPvpInviteFriend_CSC = SMsgPvpInviteFriend_CSC.ParsePackage(dataBuffer);
		PvpDataManager.Instance.ReceiveInviteHandl(sMsgPvpInviteFriend_CSC);
		UIEventManager.Instance.TriggerUIEvent( UIEventType.ReceiveInvite,sMsgPvpInviteFriend_CSC);
		return CommandCallbackType.Continue;
	}

	/// <summary>
	/// 收到队友离开
	/// </summary>
	/// <returns>The receive cancel team handle.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType PvpReceiveCancelTeamHandle(byte[] dataBuffer, int socketID)
	{
		SMsgPVPMathingLeave_SC sMsgPVPMathingLeave_SC = SMsgPVPMathingLeave_SC.ParsePackage(dataBuffer);
		PvpDataManager.Instance.SetCancelTeamInfo(sMsgPVPMathingLeave_SC);
		return CommandCallbackType.Continue;
	}
	/// <summary>
	/// 匹配队伍同步
	/// </summary>
	/// <returns>The matchind sync handle.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType PvpMatchindSyncHandle(byte[] dataBuffer, int socketID)
	{
		SMsgPVPMathingSynInfo_SC sMsgConfirmMatching_SC = SMsgPVPMathingSynInfo_SC.ParsePackage(dataBuffer);
		PvpDataManager.Instance.SetGroupMeberInfo(sMsgConfirmMatching_SC);
		return CommandCallbackType.Continue;
	}
	/// <summary>
	/// 匹配成功.
	/// </summary>
	/// <returns>The matchind sucess handle.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType PvpMatchindSucessHandle(byte[] dataBuffer, int socketID)
	{

		return CommandCallbackType.Continue;
	}
    #endregion
    
    #region 发送

    /// <summary>
    /// 加入好友队伍
    /// </summary>
    /// <param name="myActorID">自己actorID</param>
    /// <param name="friendActorID">好友actorID</param>
    public void SendJoinFriendTeamMsg(uint myActorID, uint friendActorID)
    {
        SMsgTeamCaptainJoin_CS smg = new SMsgTeamCaptainJoin_CS();
        smg.dwActorID = myActorID;
        smg.dwCaptainID = friendActorID;
        Package pkg = smg.GeneratePackage();
        this.Request(pkg);
    }

    //获取队伍列表
    public void SendGetTeamListMsg(SMSGGetTeamList_CS sMSGGetTeamList_CS)
    {
        Package pkg = sMSGGetTeamList_CS.GeneratePackage();
        this.Request(pkg);
    }
    //队伍创建
    public void SendTeamCreateMsg()
    {
        var player = PlayerManager.Instance.FindHeroDataModel();        
		var teamProp = TeamManager.Instance.MyTeamProp;
        SMsgTeamCreate_CS sMsgTeamCreate = new SMsgTeamCreate_CS()
        {
			dwEctypeID = (uint)teamProp.TeamContext.dwEctypeId,
            dwActorID = (uint)player.ActorID,
			byDiff = (byte)teamProp.TeamContext.byEctypeDifficulty
        };        
        Package pkg = sMsgTeamCreate.GeneratePackage();
        //TraceUtil.Log("发送队伍创建");
        this.Request(pkg);
    }
	/// <summary>
	/// 队伍创建
	/// </summary>
	/// <param name="containerID">容器ID</param>
	/// <param name="hard">难度级别</param>
	public void SendTeamCreateMsg(int areaID, int index , int hard)
	{
		var player = PlayerManager.Instance.FindHeroDataModel();
		SMsgTeamCreate_CS sMsgTeamCreate = new SMsgTeamCreate_CS()
		{
			dwEctypeID = (uint)areaID,
			dwActorID = (uint)player.ActorID,
			byEctypeIndex = (byte)index,
			byDiff = (byte)hard
		};     
		Package pkg = sMsgTeamCreate.GeneratePackage();
		this.Request(pkg);
	}

    //队伍解散
    public void SendTeamDisbandMsg(SMsgTeamDisband_CS sMsgTeamDisband)
    {
        Package pkg = sMsgTeamDisband.GeneratePackage();
        this.Request(pkg);
    }
    //加入队伍
    public void SendTeamMemberJoinMsg(SMsgTeamMemberJoin_CS sMsgTeamMemberJoin)
    {
        Package pkg = sMsgTeamMemberJoin.GeneratePackage();
        this.Request(pkg);
    }    
    //离开队伍
    public void SendTeamMemberLeaveMsg(SMsgTeamMemberLeave_SC sMsgTeamMemberLeave)
    {
        Package pkg = sMsgTeamMemberLeave.GeneratePackage();
        this.Request(pkg);
    }
    //踢人
    public void SendTeamMemberKickMsg(SMsgTeamMemberKick_CS sMsgTeamMemberKick)
    {
        Package pkg = sMsgTeamMemberKick.GeneratePackage();
        this.Request(pkg);
    }
    //邀请
    public void SendTeamMemberInviteMsg(SMsgTeamMemberInvite_SC sMsgTeamMemberInvite)
    {
        Package pkg = sMsgTeamMemberInvite.GeneratePackage();
        this.Request(pkg);
    }
    //准备
    public void SendTeamMemberReadyMsg(SMsgTeamMemberReady_CS sMsgTeamMemberReady)
    {
        Package pkg = sMsgTeamMemberReady.GeneratePackage();
        this.Request(pkg);
    }
    //寻找队长
    public void SendTeamFindMsg(string captainName)
    {
        Package pkg = SMsgTeamFind_CS.GeneratePackage(captainName);
        this.Request(pkg);
    }
    //更改副本
    public void SendTeamChangeEctypeMsg(SMsgTeamChangeEctype_CS sMsgTeamChangeEctype)
    {
        Package pkg = sMsgTeamChangeEctype.GeneratePackage();
        this.Request(pkg);
    }
    //快速加入
    public void SendTeamFastJoinMsg(SMsgTeamFastJoin_CS sMsgTeamFastJoin)
    {
        Package pkg = sMsgTeamFastJoin.GeneratePackage();
        this.Request(pkg);
    }

	/// <summary>
	/// 获取讨伐队伍数量
	/// </summary>
	public void SendGetCrusadeTeamNumsMsg()
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_GET_TEAMNUMLIST);
		this.Request(pkg);
	}


	/// <summary>
	/// 开始讨伐匹配
	/// </summary>
	public void SendBegingCrusadeMatching()
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_MATCHING_BEGING);
		this.Request(pkg);
	}

	/// <summary>
	/// 取消匹配
	/// </summary>
	public void SendCancelCrusadeMatching()
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_MATCHING_CANCEL);
		this.Request(pkg);
	}
	/// <summary>
	/// PVP邀请好友
	/// </summary>
	/// <param name="FriendID">Friend I.</param>
	public void PvpSendInviteFriend(uint FriendID)
	{
		SMsgPvpInviteFriend_CSC sMsgPvpInviteFriend_CSC=new SMsgPvpInviteFriend_CSC
		{
			dwActorID=FriendID,
		};
		Package Pkg=sMsgPvpInviteFriend_CSC.GeneratePackage();
		this.Request(Pkg);
	}
	/// <summary>
	/// Pvps 确认接受好友邀请
	/// </summary>
	public void  PvpSendConfirmFriend(uint InviterID)
	{
		SMsgPVPFriendConfirm_CS sMsgPVPFriendConfirm_CS=new SMsgPVPFriendConfirm_CS
		{
			dwActorID=InviterID
		};
		Package Pkg=sMsgPVPFriendConfirm_CS.GeneratePackage();
		this.Request(Pkg);
				
	}
	/// <summary>
	/// PVP请求开始匹配
	/// </summary>
	public void PVPBeginMatch()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, TeamDefineManager.MSG_PVP_MATCHING_BEGIN);
		this.Request(pak);
	}
	/// <summary>
	/// PVPs 请求取消匹配
	/// </summary>
	public void PVPCancelMatch()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, TeamDefineManager.MSG_PVP_MATCHING_CANCEL);
		this.Request(pak);
	}
	/// <summary>
	/// 是否前往匹配副本
	/// </summary>
	/// <param name="isGO">If set to <c>true</c> is G.</param>
	public void SendCrusadeMatchingAnswer(bool isGO)
	{
		SMsgConfirmMatching_CS sMsgConfirmMatching_CS = new SMsgConfirmMatching_CS();
		sMsgConfirmMatching_CS.byConfirm = (byte)(isGO?1:0);
		this.Request(sMsgConfirmMatching_CS.GeneratePackage());
	}

	/// <summary>
	/// PVPs 请求取消组队
	/// </summary>
	public void PVPCancelTeam()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, TeamDefineManager.MSG_PVP_MATCHING_LEAVE);
		this.Request(pak);
	}
    #endregion

    private string ProfessionTrans(uint proNum)
    {
        string proName = "";
        switch (proNum)
        {
            case 1:
                proName = LanguageTextManager.GetString("IDS_D2_11");
                break;
            case 2:
                proName = LanguageTextManager.GetString("IDS_D2_12");
                break;
            case 3:
                proName = LanguageTextManager.GetString("IDS_D2_13");
                break;
            case 4:
                proName = LanguageTextManager.GetString("IDS_D2_14");
                break;
            default:
                break;

        }
        return proName;
    }
    
}
