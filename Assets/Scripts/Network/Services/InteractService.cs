using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class InteractService : Service
{

    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_INTERACT:
                switch (package.GetSubMsgType())
                {
                    case InteractDefineManager.INTERACT_MSG_COMMON:
                        this.AddToInvoker(InteractCommonHandle, package.Data, socketId);
                        break;
                    case InteractDefineManager.INTERACT_MSG_NEWBIEGUIDE_DATA:   //瑕佸垵濮嬪寲鍩庨晣瑕佸紑鍚殑涓绘寜閽紝鏂嚎閲嶈仈鍙?
                        this.AddToInvoker(MainButtonInitDataHandle, package.Data, socketId);
                        break;
                  
                    case InteractDefineManager.INTERACT_MSG_INITTASK:  //initialize tasks 
                        this.AddToInvoker(InitializeTaskDataHandle, package.Data, socketId);
                        break;
                    case InteractDefineManager.INTERACT_MSG_ACCEPTTASK:  //Accept task 
                        this.AddToInvoker(AcceptTaskDataHandle, package.Data, socketId);
                        break;
                    case InteractDefineManager.INTERACT_MSG_UPDATETASK:  //Update task 
                        this.AddToInvoker(UpdateTaskDataHandle, package.Data, socketId);
                        break;
                    case InteractDefineManager.INTERACT_MSG_FINISHTASK:  //Finish task 
                        this.AddToInvoker(FinishTaskDataHandle, package.Data, socketId);
                        break;
                    case InteractDefineManager.INTERACT_MSG_GIVEUPTASK:  //Give up task 
                        this.AddToInvoker(GiveUpTaskDataHandle, package.Data, socketId);
                        break;
                    case InteractDefineManager.INTERACT_MSG_GUIDESTEP:  //鏇存柊鍓湰鍐呭紩瀵兼楠?
                        this.AddToInvoker(GuideStepHandle, package.Data, socketId);
                        break;
                    case InteractDefineManager.INTERACT_MSG_RANKINGLIST_DATA:  //鏇存柊鍓湰鍐呭紩瀵兼楠?
                        this.AddToInvoker(GetRankingListDataHandle, package.Data, socketId);
                        break;
                    case InteractDefineManager.INTERACT_MSG_GETPLAYERRANKING: 
                        this.AddToInvoker(GetRankingDetailDataHandle, package.Data, socketId);
                        break;
					case CommonMsgDefineManager.INTERACT_MSG_OPEN_UI:
						this.AddToInvoker(this.ReceiveActivityDataHandle,package.Data,socketId);
						break;
					case CommonMsgDefineManager.INTERACT_MSG_GETREWARD:
						this.AddToInvoker(this.ReceiveActivityRewardHandle,package.Data,socketId);
						break;
					case InteractDefineManager.INTERACT_MSG_PVP_MYDATA:
						this.AddToInvoker(this.GetPVPRankingListDataHandle,package.Data,socketId);
						break;
					case InteractDefineManager.INTERACT_MSG_PVP_HISTORY:
						break;
                    default:
                        break;
                }

                break;

            default:
                break;
        }
    }


    #region 鎺ユ敹娑堟伅澶勭悊
    CommandCallbackType GuideStepHandle(byte[] dataBuffer, int socketID)
    {
        TraceUtil.Log("收到Service步骤信息");
        SC_GuideStepInfo sC_GuideStepInfo = SC_GuideStepInfo.ParsePackage(dataBuffer);
        //UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeGuideStep, sC_GuideStepInfo);
        EctGuideManager.Instance.ReceiveEctypeGuideStep(sC_GuideStepInfo);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType GiveUpTaskDataHandle(byte[] dataBuffer, int socketID)
    {
        SMsgInteractGiveUpTask_CSC sMsgInteractGiveUpTask_CSC;
        sMsgInteractGiveUpTask_CSC = SMsgInteractGiveUpTask_CSC.ParseResultPackage(dataBuffer);
        //
        TaskModel.Instance.GiveUpTask(sMsgInteractGiveUpTask_CSC.dwTaskID);

        return CommandCallbackType.Continue;
    }
    CommandCallbackType FinishTaskDataHandle(byte[] dataBuffer, int socketID)
    {
        SMsgInteractFinishTask_SC sMsgInteractFinishTask_SC;
        sMsgInteractFinishTask_SC = SMsgInteractFinishTask_SC.ParseResultPackage(dataBuffer);
        TaskModel.Instance.FinishTask(sMsgInteractFinishTask_SC.dwTaskID);

        return CommandCallbackType.Continue;
    }
    CommandCallbackType UpdateTaskDataHandle(byte[] dataBuffer, int socketID)
    {
        STaskState sTaskState;
        sTaskState = STaskState.ParseResultPackage(dataBuffer);
        TaskModel.Instance.UpdateTask(sTaskState);

        return CommandCallbackType.Continue;
    }
    CommandCallbackType AcceptTaskDataHandle(byte[] dataBuffer, int socketID)
    {
        STaskState sTaskState;
        sTaskState = STaskState.ParseResultPackage(dataBuffer);
        TaskModel.Instance.AcceptTask(sTaskState);

        return CommandCallbackType.Continue;
    }
    CommandCallbackType InitializeTaskDataHandle(byte[] dataBuffer, int socketID)
    {
        SMsgInteractInitTask_SC sMsgInteractInitTask_SC;
        sMsgInteractInitTask_SC = SMsgInteractInitTask_SC.ParseResultPackage(dataBuffer);
        TaskModel.Instance.InitTask(sMsgInteractInitTask_SC.STaskStates);

        return CommandCallbackType.Continue;
    }
    CommandCallbackType InteractCommonHandle(byte[] dataBuffer, int socketID)
    {
        SMsgInteractCOMMONPackage sMsgInteractCOMMONPackage;
        sMsgInteractCOMMONPackage.sMsgInteractCOMMON_SC = SMsgInteractCOMMON_SC.ParseResultPackage(dataBuffer);
        sMsgInteractCOMMONPackage.sMsgInteractCOMMONBtn_SC = new SMsgInteractCOMMONBtn_SC[sMsgInteractCOMMONPackage.sMsgInteractCOMMON_SC.byBtnNum];

        var offset = Marshal.SizeOf(sMsgInteractCOMMONPackage.sMsgInteractCOMMON_SC);
        var msgInteractCOMMONBtnLength = Marshal.SizeOf(typeof(SMsgInteractCOMMONBtn_SC));

        for (int i = 0; i < sMsgInteractCOMMONPackage.sMsgInteractCOMMON_SC.byBtnNum; ++i)
        {
            sMsgInteractCOMMONPackage.sMsgInteractCOMMONBtn_SC[i] = SMsgInteractCOMMONBtn_SC.ParseResultPackage(dataBuffer, offset, msgInteractCOMMONBtnLength);
            offset += msgInteractCOMMONBtnLength;
        }
        RaiseEvent(EventTypeEnum.NPCInteraction.ToString(), sMsgInteractCOMMONPackage);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType MainButtonInitDataHandle(byte[] dataBuffer, int socketID)
    {
        SMsgInteract_NewbieGuide_SCS sMsgInteract_NewbieGuide_SCS = PackageHelper.BytesToStuct<SMsgInteract_NewbieGuide_SCS>(dataBuffer);
        GameManager.Instance.MainButtonIndex = sMsgInteract_NewbieGuide_SCS.wGuideIndex;
        UIEventManager.Instance.TriggerUIEvent(UIEventType.InitMainButton, (int)sMsgInteract_NewbieGuide_SCS.wGuideIndex);
        return CommandCallbackType.Continue;
    }  
    CommandCallbackType GetRankingListDataHandle(byte[] dataBuffer, int socketID)
    {
        SMsgInteract_RankingList_SC sMsgInteract_RankingList_SC = SMsgInteract_RankingList_SC.ParsePackage(dataBuffer);
        PlayerRankingDataManager.Instance.SetRankingList(sMsgInteract_RankingList_SC);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.ReceiveRankingListRes,sMsgInteract_RankingList_SC);
        return CommandCallbackType.Continue;
    }  
    CommandCallbackType GetRankingDetailDataHandle(byte[] dataBuffer, int socketID)
    {
        SMsgInteract_GetPlayerRanking_SC sMsgInteract_GetPlayerRanking_SC = SMsgInteract_GetPlayerRanking_SC.ParsePackage(dataBuffer);
        PlayerRankingDataManager.Instance.RankingDetail=sMsgInteract_GetPlayerRanking_SC;
        UIEventManager.Instance.TriggerUIEvent(UIEventType.ReceiveRankingDetailRes, sMsgInteract_GetPlayerRanking_SC);
        return CommandCallbackType.Continue;
    }  

	CommandCallbackType GetPVPRankingListDataHandle(byte[] dataBuffer, int socketID)
	{
		SMsgInteract_PvpRanking_SC sMsgInteract_RankingList_SC = SMsgInteract_PvpRanking_SC.ParsePackage(dataBuffer);
		PvpRankingDataManager.Instance.SetRankingList(sMsgInteract_RankingList_SC);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.PVPReceiveRankingListRes,sMsgInteract_RankingList_SC);
		return CommandCallbackType.Continue;
	}  
    //CommandCallbackType InteractTaskHandle(byte[] dataBuffer, int socketID)
    //{
    //    int offset;
    //    STaskLogUpdate sTaskLogUpdate = STaskLogUpdate.ParseResultPackage(dataBuffer, out offset);
    //    STaskLogContext sTaskLogContext = STaskLogContext.ParseResultPackage(dataBuffer, offset);

    //    //TraceUtil.Log("##########################TaskID" + sTaskLogUpdate.nTaskID);
    //    //TraceUtil.Log("##########################TaskType" + sTaskLogUpdate.nTaskType);
    //    //TraceUtil.Log("##########################nStatus" + sTaskLogUpdate.nStatus);

    //    NewbieGuideManager_V2.Instance.ReceiveTaskState(sTaskLogUpdate);

    //    return CommandCallbackType.Continue;
    //}

    //CommandCallbackType InteractDailyTaskHandle(byte[] dataBuffer, int socketID)
    //{
    //    int offset;
    //    STaskLogUpdate sTaskLogUpdate = STaskLogUpdate.ParseResultPackage(dataBuffer, out offset);
    //    STaskLogContext sTaskLogContext = STaskLogContext.ParseResultPackage(dataBuffer, offset);

    //    //TraceUtil.Log("[TaskID]" + sTaskLogUpdate.nTaskID);
    //    //TraceUtil.Log("[TaskType]" + sTaskLogUpdate.nTaskType);
    //    //TraceUtil.Log("[nStatus]" + sTaskLogUpdate.nStatus);
    //    //TraceUtil.Log("[sTaskLogContext]" + sTaskLogContext.nParam3);

    //    DailyTaskManager.Instance.UpdateDailyTaskData(sTaskLogUpdate, sTaskLogContext);

    //    return CommandCallbackType.Continue;
    //}
   
    ///// <summary>
    /////寮曞杩涘害锛屼富瑕佺敤浜庡紑鍚摢浜涗富鎸夐挳
    ///// </summary>
    ///// <param name="dataBuffer"></param>
    ///// <param name="socketID"></param>
    ///// <returns></returns>
   
    #endregion

	#region activity
	//活动所有数据， 主动下发（包括在晚上12点时更新）
	CommandCallbackType ReceiveActivityDataHandle(byte[] dataBuffer, int socketID)
	{
		SMsgInteract_OpenUI sMsgInteract_OpenUI = SMsgInteract_OpenUI.ParsePackage(dataBuffer);
		DailySignModel.Instance.ReveiveServerData(sMsgInteract_OpenUI);
		return CommandCallbackType.Continue;
	}
	
	//活动领取完成
	CommandCallbackType ReceiveActivityRewardHandle(byte[] dataBuffer, int socketID)
	{
		SMsgInteract_GetReward_SC sMsgInteract_GetReward_SC = SMsgInteract_GetReward_SC.ParsePackage(dataBuffer);
		DailySignModel.Instance.UpdateActiveData(sMsgInteract_GetReward_SC);
		return CommandCallbackType.Continue;
	}
	#endregion

    #region 鍙戦€佹秷鎭鐞?
    public void SendInteractCOMMON(SMsgInteractCOMMON_CS sMsgInteractCOMMON_CS, SMsgInteractCOMMONContext_CS sMsgInteractCOMMONContext_CS)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_INTERACT, (short)InteractDefineManager.INTERACT_MSG_COMMON);
        pkg.Data = PackageHelper.StructToBytes<SMsgInteractCOMMON_CS>(sMsgInteractCOMMON_CS);
        
        if (1 == sMsgInteractCOMMON_CS.byIsContext)  //鏄惁鏈夊叾浠栫殑闄勫姞鏁版嵁 0:鍚?1:鏄?
        {
            byte[] dataPkg = new byte[pkg.Data.Length + Marshal.SizeOf(typeof(SMsgInteractCOMMONContext_CS))];
            pkg.Data.CopyTo(dataPkg, 0);
            sMsgInteractCOMMONContext_CS.szContext.CopyTo(dataPkg, pkg.Data.Length);
            pkg.Data = dataPkg;
        }

        this.Request(pkg);  
    }
	//是否有其他的附加数据 0:否
	public void SendInteractCOMMON(SMsgInteractCOMMON_CS sMsgInteractCOMMON_CS)
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_INTERACT, (short)InteractDefineManager.INTERACT_MSG_COMMON);
		pkg.Data = PackageHelper.StructToBytes<SMsgInteractCOMMON_CS>(sMsgInteractCOMMON_CS);
		this.Request(pkg);  
	}
    //鍙戦€佹湰鍦版柊鎵嬪紩瀵肩姸鎬佺粰鏈嶅姟鍣ㄣ€?
    public void SendNewbieGuide(SMsgInteract_NewbieGuide_SCS sMsgInteract_NewbieGuide_SCS)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_INTERACT, (short)InteractDefineManager.INTERACT_MSG_NEWBIEGUIDE_DATA);
        pkg.Data = PackageHelper.StructToBytes<SMsgInteract_NewbieGuide_SCS>(sMsgInteract_NewbieGuide_SCS);

        this.Request(pkg);
    }


    /// <summary>
    /// 鍙戦€佹柊鎵嬪叧鍗″璇濆紩瀵肩粨鏉?
    /// </summary>
    /// INTERACT_MSG_SPEEKOVER
    public void SendEctypeDialogOver()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_INTERACT, InteractDefineManager.INTERACT_MSG_SPEEKOVER);
        this.Request(pak);
    }


    /// <summary>
    /// 发送获取排行榜列表
    /// </summary>
    /// <param name="sMsgInteract_RankingList_CS">S message interact_ ranking list_ C.</param>
    public void SendSMsgInteract_RankingList_CS(SMsgInteract_RankingList_CS sMsgInteract_RankingList_CS)
    {

        this.Request(sMsgInteract_RankingList_CS.GeneratePackage());

    }


    public void SendSMsgInteract_GetPlayerRanking_CS(SMsgInteract_GetPlayerRanking_CS sMsgInteract_GetPlayerRanking_CS)
    {
        this.Request(sMsgInteract_GetPlayerRanking_CS.GeneratePackage());
    }

	/// <summary>
	/// 发送获取pvp排行榜列表请求
	/// </summary>
	/// <param name="sMsgInteract_RankingList_CS">S message interact_ ranking list_ C.</param>
	public void SendGetPvpRankingList_CS(SMsgInteract_PvpRanking_CS sMsgInteract_RankingList_CS)
	{
		
		this.Request(sMsgInteract_RankingList_CS.GeneratePackage());
		
	}

    #endregion

}
