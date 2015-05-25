using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UI;

public class CommonService:Service
{   
    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        Package package;
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        package = PackageHelper.ParseReceiveData(dataBuffer);
        //Debug.Log("EntityService 收到主消息:" + masterMsgType + "  收到子消息：" + package.GetSubMsgType());
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_ERROR:
                LoginManager.Instance.ResetLoginButtonState();  //收到出错，重置登录按钮状态
                switch (package.GetSubMsgType())
                {
                    //服务器主动踢人
                    case SystemErrorCodeDefine.ERROR_CODE_KICK:
                        AddToInvoker(this.ReceiveKickCode, dataBuffer, socketId);
                        break;
                    //队伍没有找到返回信息
                    case SystemErrorCodeDefine.TEAM_CODE_NOLIST:
                        AddToInvoker(this.ReceiveNoListHandle, dataBuffer, socketId);
                        break;
                    case SystemErrorCodeDefine.TEAM_CODE_SELFHAVETEAM:
                        AddToInvoker(this.ReceiveSelfHaveTeamHandle, dataBuffer, socketId);
                        break;
                    case SystemErrorCodeDefine.ENTITY_CODE_NOENOUGHBINDPAY:                        
                        AddToInvoker(this.ReceiveNoEnoughBindPayHandle, dataBuffer, socketId);                        
                        break;
                    case SystemErrorCodeDefine.ENTITY_CODE_NOENOUGHACTIVELIFE://活力不足                        
                        AddToInvoker(this.ReceiveNoEnoughActivelife, dataBuffer, socketId);                        
                        break;
                    case SystemErrorCodeDefine.GOODS_CODE_PACKETFULL:
                        //背包已满
						AddToInvoker(this.ReceivePackageFull, dataBuffer, socketId);
                        break;
                    case SystemErrorCodeDefine.TEAM_CODE_NOEXIST:
                        AddToInvoker(this.ReceiveTeamNoExist, dataBuffer, socketId);     
                        break;
                    case SystemErrorCodeDefine.TEAM_CODE_TEAMFULL:
                        AddToInvoker(this.ReceiveTeamFull, dataBuffer, socketId);
                        break;
                    case SystemErrorCodeDefine.TEAM_CODE_TEAMMEMBERNOREADY:
                        AddToInvoker(this.ReceiveTeamMemberNoReady, dataBuffer, socketId);
                        break;
                    case SystemErrorCodeDefine.TEAM_CODE_TEAMFIGHTING:
                        AddToInvoker(this.ReceiveTeamFighting, dataBuffer, socketId);
                        break;                    
                    //创建人物失败 角色名重复
                    case SystemErrorCodeDefine.ERROR_CODE_CREATEFAILED_DUPLICATE:
                        AddToInvoker(this.ReceiveCreateFailedDuplicate, dataBuffer, socketId);
                        break;
                    //创建人物失败 角色达到上限
                    case SystemErrorCodeDefine.ERROR_CODE_CREATEFAILED_MAXNUM:
                        AddToInvoker(this.ReceiveCreateFailedMaxnum, dataBuffer, socketId);
                        break;
                    case SystemErrorCodeDefine.TRADE_CODE_SALE:
                        //出售物品成功
                        AddToInvoker(this.ReceiveDiscardContainerGoods,dataBuffer,socketId);
                        break;
					case SystemErrorCodeDefine.ECTYPE_CODE_NOENTERTIMES:						
						AddToInvoker(this.ReceiveNoEnterTimesHandle,dataBuffer,socketId);
					break;
                    case SystemErrorCodeDefine.ECTYPE_CODE_PVPREMATCHING:
                        AddToInvoker(this.ReceivePVPRematching, dataBuffer, socketId);
                        break;
                    case SystemErrorCodeDefine.ECTYPE_CODE_PVPNOTIMES:
                        AddToInvoker(this.ReceivePVPNoTimes, dataBuffer, socketId);
                        break;
                    case SystemErrorCodeDefine.ECTYPE_CODE_PLAYERLEVEL://副本等级不满足
                        AddToInvoker(this.ReceiveEctypePlayerLevelError, package.Data, socketId);
                        break;
                    case SystemErrorCodeDefine.ECTYPE_CODE_ROMMNOFOUND://找不到房间
                        AddToInvoker(this.ReceiveEctypeCantFindRoom,dataBuffer,socketId);
                        break;
                    case SystemErrorCodeDefine.ECTYPE_CODE_PLAYERNUMMAX://人数已满
                        AddToInvoker(this.ReceiveEctypeRoleFull,dataBuffer,socketId);
                        break;
                    case SystemErrorCodeDefine.TEAM_CODE_ECTYPE_UNLOCK://关卡未解锁
                        AddToInvoker(this.ReceiveEctypeLockError,dataBuffer,socketId);
                        break;
                    case SystemErrorCodeDefine.TEAM_CODE_TEAM_TIMELIMIT:
                        AddToInvoker(this.ReceiveTeamTimeLimit, package.Data, socketId);
                        break;
                    case SystemErrorCodeDefine.CHAT_CODE_FORBID:
                        AddToInvoker(this.ReceiveChatForbid, package.Data, socketId);
                        break;
					case SystemErrorCodeDefine.CHAT_PLAYER_OFFLINE:
						AddToInvoker(this.ReceiveChatPlayerOffLine,package.Data,socketId);
						break;
					case SystemErrorCodeDefine.FRIEND_CODE_FULL:
						AddToInvoker(this.ReceiveFriendCodeFull,package.Data,socketId);
						break;
					case SystemErrorCodeDefine.FRIEND_CODE_OTHERISFULL:
						AddToInvoker(this.ReceiveOtherFriendCodeFull,package.Data,socketId);
						break;
					case SystemErrorCodeDefine.FIREND_CODE_OFFLINE:
						AddToInvoker(this.ReceiveFriendOffLine,package.Data,socketId);
						break;
					case SystemErrorCodeDefine.FRIEND_CODE_EXIST:
						AddToInvoker(this.ReceiveFriendIsExist,package.Data,socketId);
						break;
					case SystemErrorCodeDefine.TEAM_CODE_TARGETHASTEAM:
						AddToInvoker(this.ReceiveTargetHasTeam,package.Data,socketId);
						break;
					case SystemErrorCodeDefine.FRIEND_CODE_ISOFFLINE:
						AddToInvoker(this.ReceiveFriendIsOffLine,package.Data, socketId);
						break;
					case SystemErrorCodeDefine.ECTYPE_CODE_NOITEM:
						AddToInvoker(this.ReceiveNoItem, package.Data, socketId);
						break;
					case SystemErrorCodeDefine.TRADE_AUCTION_OUTDATE:
						AddToInvoker(this.ReceiveAuctionFailTip,package.Data, socketId);
						break;
				UI.MessageBox.Instance.ShowTips (3, LanguageTextManager.GetString ("IDS_I27_20"), 1);
                    ////创建人物(登录)失败 账号已登录 倒计时x秒 重新操作(服务器在踢人)
                    //case SystemErrorCodeDefine.ERROR_CODE_CREATEFAILED_LOGGEDIN:
                    //    AddToInvoker(this.ReceiveCreateFailedLoggedin, dataBuffer, socketId);
                    //    break;
                    default:
                        AddToInvoker(this.ReceiveDefaultErrorCode, dataBuffer, socketId);
                        break;
                }
                break;
            default:
                //TraceUtil.Log("不能识别的主消息:" + package.GetMasterMsgType());
                break;
        }
    }
    #region 发送
    public void SendTimedPackage(Package timedPkg)
    {
        this.Request(timedPkg);
    }
    /// <summary>
    /// 向服务器发送登出消息
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="mapId"></param>
    public void SendPlayerLoginOut(long uid,uint mapId)
    {       
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_PLAYER_OP_LOGINOUT);
        
        pak.Data = BitConverter.GetBytes(uid).Concat(BitConverter.GetBytes(mapId)).ToArray();
        this.Request(pak);
    }
    #endregion
    #region 接收

    CommandCallbackType ReceiveEctypeLockError(byte[] dataBuffer, int socketId)
    {
        TraceUtil.Log("收到关卡未解锁消息");
        MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_507"), 1);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeLockError, null);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveTeamTimeLimit(byte[] dataBuffer, int socketId)
    {
        //TraceUtil.Log("[ReceiveTeamTimeLimit]");
        SMsgErrorCode_SC sMsgErrorCode_SC = SMsgErrorCode_SC.ParsePackage(dataBuffer);
        if (sMsgErrorCode_SC.iContent.Length > 0)
        {
            MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_528"), 1);
        }       
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveTeamRejectError(byte[] dataBuffer, int socketId)
    {
        SMsgTeamMemberJoin_CS sMsgTeamMemberJoin_CS = SMsgTeamMemberJoin_CS.ParsePackage(dataBuffer);
        var friendList = UI.Friend.FriendDataManager.Instance.GetFriendListData;
        var friendData = friendList.SingleOrDefault(p => p.sMsgRecvAnswerFriends_SC.dwFriendID == sMsgTeamMemberJoin_CS.dwActorID);
        if (friendData != null)
        {
			if(MessageBox.Instance!=null)
			{
				MessageBox.Instance.ShowTips(4, friendData.sMsgRecvAnswerFriends_SC.Name + LanguageTextManager.GetString("IDS_H2_22"), 1);
			}
            
        }
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveEctypeCantFindRoom(byte[] dataBuffer,int socketId)
    {
        UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeCantFindRoom,null);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveEctypeRoleFull(byte[] dataBuffer, int socketId)
    {
        UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeRoleFull,null);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveEctypePlayerLevelError(byte[] dataBuffer,int socketId)
    {
        EctypeLevelError ectypeLevelError = EctypeLevelError.ParcePackage(dataBuffer);
		MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_84"), 1);
        //UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeLevelError,ectypeLevelError);
        return CommandCallbackType.Continue;
    }

    public CommandCallbackType ReceiveKickCode(byte[] dataBuffer, int socketId)
    {
        //TraceUtil.Log("被踢出消息;"+Time.realtimeSinceStartup);
		CheatManager.Instance.isIDKickedMark = true;
		RaiseEvent(EventTypeEnum.PlayerBeKicked.ToString(), null);
        return CommandCallbackType.Pause;
    }

    CommandCallbackType ReceiveNoListHandle(byte[] dataBuffer, int socketId)
    {
        RaiseEvent(EventTypeEnum.TeamNoFoundList.ToString(), null);
        
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveSelfHaveTeamHandle(byte[] dataBuffer, int socketId)
    {
        //RaiseEvent(EventTypeEnum.TeamSelfHave.ToString(), null); 
        TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"已经拥有队伍");
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveNoEnoughBindPayHandle(byte[] dataBuffer, int socketId)
    {
        //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H2_44"), LanguageTextManager.GetString("IDS_H2_55"));
        UIEventManager.Instance.TriggerUIEvent(UIEventType.NotEnoughGoldMoney,null);
        return CommandCallbackType.Continue;
    }
	//活力不足
    CommandCallbackType ReceiveNoEnoughActivelife(byte[] dataBuffer, int socketId)
    {
		if (LoadingUI.Instance != null) {
			LoadingUI.Instance.Close ();
		}
        //RaiseEvent(EventTypeEnum.TeamActiveLifeNotEnough.ToString(), null);
        PopupObjManager.Instance.ShowAddVigour();
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceivePackageFull(byte[] dataBuffer, int socketId)
    {
        UIEventManager.Instance.RegisterUIEvent(UIEventType.PackageFull,null);
		MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_I17_5"), 1f);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveTeamNoExist(byte[] dataBuffer, int socketId)
    {
		if(MessageBox.Instance!=null)
		{
			//MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_I13_12"), LanguageTextManager.GetString("IDS_H2_55"), RefreshTeamList);        
			MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I13_12"),1f);
		}
        
        return CommandCallbackType.Continue;
    }
    void RefreshTeamList()
    {
        RaiseEvent(EventTypeEnum.TeamNoExist.ToString(), null);
    }
    CommandCallbackType ReceiveTeamFull(byte[] dataBuffer, int socketId)
    {
        RaiseEvent(EventTypeEnum.TeamFull.ToString(), null);
		MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_I13_12"), 1f);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveTeamFighting(byte[] dataBuffer, int socketId)
    {        
		MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H1_83"), 1f);
        return CommandCallbackType.Continue;
    }  
    CommandCallbackType ReceiveTeamMemberNoReady(byte[] dataBuffer, int socketId)
    {
        RaiseEvent(EventTypeEnum.TeamExistMemberNoReady.ToString(), null);

        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveCreateFailedLoggedin(byte[] dataBuffer, int socketId)
    {
        RaiseEvent(EventTypeEnum.CreateFailedLoggedin.ToString(), null);

        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveCreateFailedDuplicate(byte[] dataBuffer, int socketId)
    {
        //RaiseEvent(EventTypeEnum.CreateFailedDuplicate.ToString(), null);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.CreateFailedDuplicate, null);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveCreateFailedMaxnum(byte[] dataBuffer, int socketId)
    {
        //RaiseEvent(EventTypeEnum.CreateFailedMaxnum.ToString(), null);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.CreateFailedMaxnum, null);

        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveDiscardContainerGoods(byte[] dataBuffer, int socketId)
    {
        UIEventManager.Instance.TriggerUIEvent(UIEventType.DiscardContainerGoodsComplete,null);
        return CommandCallbackType.Continue;
    }
	CommandCallbackType ReceiveNoEnterTimesHandle(byte[] dataBuffer, int socketId)
	{
		LoadingUI.Instance.Close();
		//讨伐挑战次数用完
		MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I19_8"),1);
		return CommandCallbackType.Continue;
	}
    CommandCallbackType ReceivePVPRematching(byte[] dataBuffer, int socketID)
    {
        RaiseEvent(EventTypeEnum.PVPRematching.ToString(), null);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceivePVPNoTimes(byte[] dataBuffer, int socketID)
    {
        RaiseEvent(EventTypeEnum.PVPNoTimes.ToString(), null);
        return CommandCallbackType.Continue;       
    }
    CommandCallbackType ReceiveDefaultErrorCode(byte[] dataBuffer, int socketId)
    {
        var package = PackageHelper.ParseReceiveData(dataBuffer);
        short subMsg = package.GetSubMsgType();
        ServerError serverError = new ServerError();
        serverError.ErrorCode = subMsg;
        RaiseEvent(EventTypeEnum.ReceiveDefaultErrorCode.ToString(), serverError);

        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveChatForbid(byte[] dataBuffer, int socketID)
    {
        SMsgChatCodeForbid_SC sMsgChatCodeForbid_SC = SMsgChatCodeForbid_SC.ParsePackage(dataBuffer);
		MessageBox.Instance.ShowTips(4, string.Format(LanguageTextManager.GetString("IDS_I24_18"), sMsgChatCodeForbid_SC.dwTime), 1f);
        return CommandCallbackType.Continue;
    }
	CommandCallbackType ReceiveChatPlayerOffLine(byte[] dataBuffer, int socketID)
	{
		MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_I24_17"), 1f);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveFriendCodeFull(byte[] dataBuffer, int socketID)
	{
		MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I23_15"),1f);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveOtherFriendCodeFull(byte[] dataBuffer, int socketID)
	{
		MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_H1_86"),1f);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveFriendOffLine(byte[] dataBuffer, int socketID)
	{
		MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I23_23"),1f);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveTargetHasTeam(byte[] dataBuffer, int socketID)
	{
		MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I13_37"),1f);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveFriendIsOffLine(byte[] dataBuffer, int socketID)
	{
		MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I13_38"),1f);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveNoItem(byte[] dataBuffer, int socketID)
	{
		MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I19_19"),1f);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveFriendIsExist(byte[] dataBuffer, int socketID)
	{
		MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I23_19"),1f);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveAuctionFailTip(byte[] dataBuffer, int socketID)
	{
		UI.MessageBox.Instance.ShowTips (4,LanguageTextManager.GetString ("IDS_I27_20"), 1);
		return CommandCallbackType.Continue;
	}
    #endregion
}
