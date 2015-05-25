using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UI.Friend;

/// <summary>
/// Leo-Yu
/// 2013.06.10
/// </summary>
public class FriendService : Service
{

    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_FRIEND:
                //TraceUtil.Log("收到好友服务器消息：" + package.GetSubMsgType());
                switch (package.GetSubMsgType())
                {
                    case FriendDefineManager.MSG_FRIEND_FIND:   //查找好友 根据名称查找玩家详细信息
                        ////TraceUtil.Log("FriendService=-++-=收到查找好友消息");
                        break;
                    case FriendDefineManager.MSG_FRIEND_ADD:    //添加好友消息
                        ////TraceUtil.Log("FriendService=-++-=添加好友应答消息");
                        this.AddToInvoker(RecvAddFriendHandle, package.Data, socketId);
                        break;
                    case FriendDefineManager.MSG_FRIEND_DELETE:    //S发送删除好友应答给C
                        ////TraceUtil.Log("FriendService=-++-=更新好友列表消息");
                        this.AddToInvoker(DelFriendsResultHandle, package.Data, socketId);
                        break;
                    case FriendDefineManager.MSG_FRIEND_GETLIST:    //S下发好友列表给C
                        ////TraceUtil.Log("FriendService=-++-=收到好友列表消息");
                        this.AddToInvoker(RecvFriendListHandle, package.Data, socketId);
                        break;
					case FriendDefineManager.MSG_FRIEND_GETREQUESTLIST:
						this.AddToInvoker(RecvFriendRequestListHandle, package.Data, socketId);
						break;
                    case FriendDefineManager.MSG_STRANGE_GETLIST:    //获取附近玩家列表
                        ////TraceUtil.Log("FriendService=-++-=收到附近玩家列表消息");
                        this.AddToInvoker(RecvNearbyPlayerListHandle, package.Data, socketId);
                        break;
                    case FriendDefineManager.MSG_FRIEND_ANSWER_ADD:
                        ////TraceUtil.Log("FriendService=-++-=收到加好友拒绝同意列表消息");
                        this.AddToInvoker(RecvFriendAnswerHandle, package.Data, socketId);
                        break;
                    case FriendDefineManager.MSG_FRIEND_REMIND://S下发给线上好友上下线提醒
                        ////TraceUtil.Log("FriendService=-++-=收到好友上下线消息"); //好友上下线消息
                        this.AddToInvoker(RecvFriendRemindHandle, package.Data, socketId);
                        break;
                    case FriendDefineManager.MSG_FRIEND_UPDATE:
                        ////TraceUtil.Log("FriendService=-++-=收到好友更新属性消息");
                        this.AddToInvoker(RecvFriendUpdateHandle, package.Data, socketId);
                        break;
                    default:
                        break;
                }
                break;
            default:
                //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"不能识别好友主消息" + package.GetMasterMsgType());
                break;

        }
    }

    #region 接收消息处理

    /// <summary>
    /// 新好友请求
    /// </summary>
    CommandCallbackType RecvAddFriendHandle(byte[] dataBuffer, int socketID)
    {
        SMsgAdddFriends_SC sMsgAdddFriends_SC = SMsgAdddFriends_SC.ParsePackage(dataBuffer);        

        PanelElementDataModel panelElement = new PanelElementDataModel();
		panelElement.sMsgRecvAnswerFriends_SC = sMsgAdddFriends_SC.sMsgRecvAnswerFriends_SC;
        panelElement.RequestTime = sMsgAdddFriends_SC.tCurTime;
        panelElement.BtnType = ButtonType.AddFriend;

        FriendDataManager.Instance.IsCreateFriendUI = true;
        FriendDataManager.Instance.RegRequestData(panelElement);

		if(GameManager.Instance.CreateEntityIM)
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UI.MainUI.UIType.Friend);
		}
		else
		{
			PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("RecvAddFriendHandle",()=>{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UI.MainUI.UIType.Friend);
			});
		}

        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// 删除好友消息
    /// </summary>
    CommandCallbackType DelFriendsResultHandle(byte[] dataBuffer, int socketID)
    {
        //uint dActorID = BitConverter.ToUInt32(dataBuffer, 0);
        SMsgDelFriends_SC sMsgDelFriends_SC = PackageHelper.BytesToStuct<SMsgDelFriends_SC>(dataBuffer);	

        FriendDataManager.Instance.DeleteFriendData(sMsgDelFriends_SC.dwbDelActorID);

        if (FriendDataManager.Instance.CurPanelState == PanelState.MYFRIENDLIST)
            FriendDataManager.Instance.IsUpdateFriendList = true;

        if (FriendDataManager.Instance.IsDelFriendIsMe)
		{
			if(UI.MessageBox.Instance!=null)
			{
				UI.MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I23_12"),1f);
				FriendDataManager.Instance.IsDelFriendIsMe = false;
			}
		}
        UIEventManager.Instance.TriggerUIEvent(UIEventType.DeleteFriendSuccess, null);
		RaiseEvent(EventTypeEnum.RefreshFriendList.ToString(), null);
         
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// 接收好友列表消息
    /// </summary>
    CommandCallbackType RecvFriendListHandle(byte[] dataBuffer, int socketID)
    {
        SMsgGetActorListHead sMsgGetActorListHead = SMsgGetActorListHead.ParsePackage(dataBuffer);

        PanelElementDataModel panelElement;// = new PanelElementDataModel();
        FriendDataManager.Instance.GetFriendListData.Clear();
		FriendDataManager.Instance.SetFriendMaxNum((int)sMsgGetActorListHead.byFriendMaxNum);
        for (int i = 0; i < sMsgGetActorListHead.dwFriendNum; i++)
        {
            panelElement = new PanelElementDataModel();

			panelElement.sMsgRecvAnswerFriends_SC = sMsgGetActorListHead.sMsgRecvAnswerFriends_SC[i];
            panelElement.BtnType = ButtonType.FriendList;

            //FriendDataManager.Instance.IsCreateFriendUI = true;
            //FriendDataManager.Instance.FriendUIQueue.Enqueue(panelElement);
            FriendDataManager.Instance.RegFriendData(panelElement);
        }
        return CommandCallbackType.Continue;
    }

	/// <summary>
	/// 接收好友请求列表
	/// </summary>
	CommandCallbackType RecvFriendRequestListHandle(byte[] dataBuffer, int socketID)
	{
		SMsgGetReuqestListHead sMsgGetReuqestListHead = SMsgGetReuqestListHead.ParsePackage(dataBuffer);

		for(int i=0;i<sMsgGetReuqestListHead.byRequestNum;i++)
		{
			PanelElementDataModel panelElement = new PanelElementDataModel();
			panelElement.sMsgRecvAnswerFriends_SC = new SMsgRecvAnswerFriends_SC()
			{
				szName = sMsgGetReuqestListHead.FriendRequests[i].szActorName,
				dwSex = sMsgGetReuqestListHead.FriendRequests[i].bySex,
				sActorLevel = sMsgGetReuqestListHead.FriendRequests[i].byLevel,
				dwFriendID = sMsgGetReuqestListHead.FriendRequests[i].AskActorID,
				dbSysActorImageHeadID = sMsgGetReuqestListHead.FriendRequests[i].dwActorHeadID,
				dwFight = sMsgGetReuqestListHead.FriendRequests[i].dwFightNum,
				dProfession = sMsgGetReuqestListHead.FriendRequests[i].byKind,
				bOnLine = 1,
			};
			panelElement.RequestTime = sMsgGetReuqestListHead.FriendRequests[i].dwExpireTime;
			panelElement.BtnType = ButtonType.AddFriend;
			FriendDataManager.Instance.RegRequestData(panelElement);  
		}
		if(sMsgGetReuqestListHead.byRequestNum>0)
		{
			if(GameManager.Instance.CreateEntityIM)
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UI.MainUI.UIType.Friend);
			}
			else
			{
				PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("RecvAddFriendHandle",()=>{
					UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UI.MainUI.UIType.Friend);
				});
			}
		}
		return CommandCallbackType.Continue;
	}

    /// <summary>
    /// 接收附近玩家列表消息
    /// </summary>
    CommandCallbackType RecvNearbyPlayerListHandle(byte[] dataBuffer, int socketID)
    {
        SMsgGetActorListHead sMsgGetActorListHead = SMsgGetActorListHead.ParsePackage(dataBuffer);
        RaiseEvent(EventTypeEnum.RevNearlyPlayer.ToString(), sMsgGetActorListHead);
        return CommandCallbackType.Continue;
    }

	CommandCallbackType RecvFriendAnswerHandle(byte[] dataBuffer, int socketID)
	{
		SMsgAnswerFriends_SC sMsgAnswerFriends_SC = SMsgAnswerFriends_SC.ParsePackage(dataBuffer);
		PanelElementDataModel panelElement = new PanelElementDataModel();
		panelElement.sMsgRecvAnswerFriends_SC = sMsgAnswerFriends_SC.sMsgRecvAnswerFriends_SC;
		panelElement.BtnType = ButtonType.FriendList;
		FriendDataManager.Instance.RegFriendData(panelElement);
		
		FriendDataManager.Instance.IsCreateFriendUI = true;
		
		if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
		{
			if(UI.MessageBox.Instance!=null)
			{
				UI.MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_I23_18"),1f);
			}
		}
		FriendDataManager.Instance.DeleteRequest(sMsgAnswerFriends_SC.dwFriendActorID);
		
		RaiseEvent(EventTypeEnum.AddFriendSuccess.ToString(), null);
		return CommandCallbackType.Continue;
	}


	/// <summary>
	/// 好友上下线通知
	/// </summary>
    CommandCallbackType RecvFriendRemindHandle(byte[] dataBuffer, int socketID)
    {
        SMsgUpdateOnLine_SC sMsgUpdateOnLine_SC = PackageHelper.BytesToStuct<SMsgUpdateOnLine_SC>(dataBuffer);
        SMsgUpdateFriendProp_SC sMsgUpdateFriendProp_SC = new SMsgUpdateFriendProp_SC();
        sMsgUpdateFriendProp_SC.dwFriendActorID = sMsgUpdateOnLine_SC.dwFriendActorID;
        sMsgUpdateFriendProp_SC.byIndex = 5;
		sMsgUpdateFriendProp_SC.dwPropNum = sMsgUpdateOnLine_SC.byBeOnLine;

        FriendDataManager.Instance.UpdateFriendProp(sMsgUpdateFriendProp_SC);
        FriendDataManager.Instance.SortFriendList();
        //FriendDataManager.Instance.RegUpdateFriendStatus(sMsgUpdateFriendProp_SC);

        if (FriendDataManager.Instance.CurPanelState == PanelState.MYFRIENDLIST)
            FriendDataManager.Instance.IsUpdateFriendList = true;

        return CommandCallbackType.Continue;
    }

	/// <summary>
	/// 好友单属性更新
	/// </summary>
    CommandCallbackType RecvFriendUpdateHandle(byte[] dataBuffer, int socketID)
    {
        SMsgUpdateFriendProp_SC sMsgUpdateFriendProp_SC = PackageHelper.BytesToStuct<SMsgUpdateFriendProp_SC>(dataBuffer);
        FriendDataManager.Instance.UpdateFriendProp(sMsgUpdateFriendProp_SC);

        //FriendDataManager.Instance.RegUpdateFriendStatus(sMsgUpdateFriendProp_SC);

        return CommandCallbackType.Continue;
    }

    #endregion

    #region 发送消息处理
    /// <summary>
    /// 发送附近玩家请求
    /// </summary>
    /// <param name="dActorID">当前请求玩家ID</param>
    public void SendNearbyPlayerRequst(uint dActorID)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FRIEND, FriendDefineManager.MSG_STRANGE_GETLIST);
        pkg.Data = BitConverter.GetBytes(dActorID);

        this.Request(pkg);
    }

    /// <summary>
    /// 发送加好友请求消息
    /// </summary>
    /// <param name="sMsgAddFriend">请求者ID，被请求者ID</param>
    public void SendAddFriendRequst(SMsgAddFriends_CS sMsgAddFriend)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FRIEND, FriendDefineManager.MSG_FRIEND_ADD);
        pkg.Data = PackageHelper.StructToBytes<SMsgAddFriends_CS>(sMsgAddFriend);

        this.Request(pkg);
    }

    /// <summary>
    /// 回复好友请求消息，可为接受或者拒绝请求
    /// </summary>
    /// <param name="sMsgAnswerFriend">包括拒绝或者接受，回复者ID，被回复者ID</param>
    public void SendAnswerFriendRequst(SMsgAnswerFriends_CS sMsgAnswerFriend)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FRIEND, FriendDefineManager.MSG_FRIEND_ANSWER_ADD);
        pkg.Data = PackageHelper.StructToBytes<SMsgAnswerFriends_CS>(sMsgAnswerFriend);

        this.Request(pkg);
    }

    /// <summary>
    /// 删除好友消息
    /// </summary>
    /// <param name="sMsgDelFriend">请求删除好友者ID，被删除者ID</param>
    public void SendDelFriendRequst(SMsgDelFriends_CS sMsgDelFriend)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FRIEND, FriendDefineManager.MSG_FRIEND_DELETE);
        pkg.Data = PackageHelper.StructToBytes<SMsgDelFriends_CS>(sMsgDelFriend);

        this.Request(pkg);
    }
    #endregion


    //接收加好友成功消息
//    public void RecAddFriendSuccess()
//    {
//        //FriendDataManager.Instance.IsCreateFriendUI = true;
//        RaiseEvent(EventTypeEnum.AddFriendSuccess.ToString(), null);
//    }
}
