using UI.MainUI;
using UnityEngine;
using System;
using System.Linq;

public class EctypeService : Service
{

    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        //TraceUtil.Log("收到副本主消息" + masterMsgType + "收到子消息。" + package.GetSubMsgType());
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_ECTYPE:
                switch (package.GetSubMsgType())
                {
                    //case EctypeDefineManager.MSG_ECTYPE_WORLDMAP_SELECT:
                    //    //TraceUtil.Log("收到世界地图创建选择消息");
                    //    this.AddToInvoker(WorldMapSelectHandle, dataBuffer, socketId);
                    //    break;

                    //case EctypeDefineManager.MSG_ECTYPE_ECTYPECONTAINER_LEVELDATAS:
                    //    this.AddToInvoker(EctypeLevelDataHandle, dataBuffer, socketId);
                    //    //TraceUtil.Log("回应关卡信息！！！！！");
                    //    break;

                    //case EctypeDefineManager.MSG_ECTYPE_SETTLEACCOUNTS:
                    //    this.AddToInvoker(EctypeSettleAccountHandle, dataBuffer, socketId);
                    //    //TraceUtil.Log("收到副本结算消息！");
                    //    break;

                    //case EctypeDefineManager.MSG_ECTYPE_SYN_SKILLDATA:
                    //    this.AddToInvoker(ReceiveSkillDataHandle, dataBuffer, socketId);
                    //    break;

                    //case EctypeDefineManager.MSG_ECTYPE_CREATEERROR:
                    //    this.AddToInvoker(EctypeRequestError, dataBuffer, socketId);
                    //    //TraceUtil.Log("创建副本失败！！");
                    //    break ;                   
                    case ECTYPE_DefineManager.MSG_ECTYPE_SYN_SKILLDATA:
                        //TraceUtil.Log("收到技能同步消息");
                        this.AddToInvoker(ReceiveSkillDataHandle, dataBuffer, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_SELECT:
                        //TraceUtil.Log("收到跳转副本界面消息");
                        this.AddToInvoker(EctypeSelectHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_LEVELDATA:
                        //TraceUtil.Log("收到单个副本信息");
                        this.AddToInvoker(EctypeLevelDataHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_CANENTER_ECTYPE:
                        //TraceUtil.Log("所有队友加入副本成功");
                        this.AddToInvoker(ResponeTeamateEnterEctype, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_SETTLEACCOUNTS:
                        this.AddToInvoker(EctypeSettleAccountHandle, dataBuffer, socketId);
                        //TraceUtil.Log("收到副本结算消息！");
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_PLAYERREVIVE:
                        this.AddToInvoker(sMSGEctypePlayerReviveHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_TREASUREAWARD:
                        //TraceUtil.Log("收到宝箱奖励消息 ");
                        this.AddToInvoker(sMSGEctypeTreasureReward_SCHandel, package.Data, socketId);
                        break;
                    //\
                    case ECTYPE_DefineManager.MSG_TEST_BEGIN_LOG:
                        this.AddToInvoker(StartPrintHandle, package.Data, socketId);

                        break;
                    case ECTYPE_DefineManager.MSG_TEST_END_LOG:
                        this.AddToInvoker(EndPrintHandle, package.Data, socketId);

                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_ERRORCODE:
                        this.AddToInvoker(EctypeErrorCodeHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_CLEARANCE:
                        this.AddToInvoker(ECTYPE_CLEARANCEHandle, package.Data, socketId);

                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_PVP_FINDPLAYER:
                        this.AddToInvoker(PVPFindPlayerHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_PVP_READY:
                        this.AddToInvoker(PVPReadyHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_PVP_FIGHTING:
                        this.AddToInvoker(PVPFightingHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_PVP_SETTLEACCOUNTS:
                        this.AddToInvoker(PVPSettleAccountHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_INITIALIZE:
                        this.AddToInvoker(InitializeEctypeHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_YAOQIPROP:
                        this.AddToInvoker(UpdateYaoqiHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_PRACTICE_LIST://练功房列表
                        this.AddToInvoker(ReceiveMartialArtsRoomList, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_ALLSEAT_INFO://同步房间座位信息
                        this.AddToInvoker(ReceiveEctypeAllSeatInfo, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_SET_INFO://同步座位信息
                        this.AddToInvoker(ReceiveUpdateEctypeSeatInfo, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_TRIALS_INFO://试炼副本列表
                        this.AddToInvoker(ReceiveTrialsEctypeList, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_TRIALS_SUBRESULT://试炼副本波数结算
                        this.AddToInvoker(ReceiveSingleTrialsSettlement, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_TRIALS_TOTALRESULT://试炼副本结算
                        this.AddToInvoker(ReceiveTrialsSettlement, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_PRACIICE_YAONVUPDATE:
                        //TraceUtil.Log("[ReceivePracticeYaoNvUpdateHandle]");
                        this.AddToInvoker(ReceivePracticeYaoNvUpdateHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_SHOW_TIPS:
                        this.AddToInvoker(ReceiveMissionFailSettement, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_FIGHT_MODE:
                        this.AddToInvoker(EctypeFightModeHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_NORMAL_RESULT:
                        this.AddToInvoker(ReceiveBossDeadSettement, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_YAONVSKILL_USETIME:
                        this.AddToInvoker(ReceiveYaoNvSkillUseTime, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_OPEN_CHEST:
                        this.AddToInvoker(ReceiveTreasureThestsOpenMsg, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_DEFINE_LOOPNUM:
                        this.AddToInvoker(ReceiveLoopNumMsg, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_DEFINE_LOOPMAX:
                        this.AddToInvoker(ReceiveMaxLoopNUmMsg, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_FINISH:
                        this.AddToInvoker(ReceiveEctypeFinishMsg, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_DEFINE_INFO:
                        this.AddToInvoker(ReceiveEctypeDefineInfoMsg, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_DEFINE_UPDATA:
                        this.AddToInvoker(ReceiveEctypeDefineUpdateMsg, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_MEMBER_UPDATEPROP:
                        this.AddToInvoker(ReceiveEctypePropsHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_DEFINE_RESULT:
                        this.AddToInvoker(ReceiveDefenceEctypeResultHandle, package.Data, socketId);
                        break;
                    //常规副本信息
                    case ECTYPE_DefineManager.MSG_ECTYPE_NORMAL_INFO:
                        this.AddToInvoker(ReceiveEctypeNomalDataHandle, package.Data, socketId);
                        break;
                    //常规副本信息更新
                    case ECTYPE_DefineManager.MSG_ECTYPE_NORMAL_UPDATA:
                        this.AddToInvoker(ReceiveEctypeNomalUpdateHandle, package.Data, socketId);
                        break;
                    //更新副本宝箱信息
                    case ECTYPE_DefineManager.MSG_ECTYPE_NORMAL_UPDATACHEST:
                        this.AddToInvoker(ReceiveChessInfoUpdateHandle, package.Data, socketId);
                        break;

                    //无尽副本波数
                    case ECTYPE_DefineManager.MSG_ECTYPE_ENDLESS_LOOPNUM:
                        this.AddToInvoker(ReceiveEndlessLoopNumHandle, package.Data, socketId);
                        break;
                    //无尽副本获得奖励(无尽副本闯过的波数)
                    case ECTYPE_DefineManager.MSG_ECTYPE_ENDLESS_REWARD:
                        this.AddToInvoker(ReceiveEndlessPassLoopHandle, package.Data, socketId);
                        break;
                    //无尽副本结算
                    case ECTYPE_DefineManager.MSG_ECTYPE_ENDLESS_RESULT:
                        this.AddToInvoker(ReceiveEndlessFinishPassLoopHandle, package.Data, socketId);
                        break;
                    //无尽副本界面数据
                    case ECTYPE_DefineManager.MSG_ECTYPE_ENDLESS_INFO:
                        this.AddToInvoker(ReceiveEndlessBestDataHandle, package.Data, socketId);
                        break;
                    //无尽副本界面数据更新
                    case ECTYPE_DefineManager.MSG_ECTYPE_ENDLESS_UPDATA:
                        this.AddToInvoker(ReceiveEndlessBestDataUpdateHandle, package.Data, socketId);
                        break;
                    //无尽副本断线重连
                    case ECTYPE_DefineManager.MSG_ECTYPE_ENDLESS_LOOPTIME:
                        this.AddToInvoker(ReceiveEndlessAgainConnectHandle, package.Data, socketId);
                        break;
                    //无尽副本跳转场景
                    case ECTYPE_DefineManager.MSG_ECTYPE_ENDLESS_MAPJUMPTIME:
                        this.AddToInvoker(ReceiveEndlessSceneTimeHandle, package.Data, socketId);
                        break;
                    //讨伐副本结算
                    case ECTYPE_DefineManager.MSG_ECTYPE_CRUSADE_RESULT:
                        this.AddToInvoker(ReceiveEctypeCrusadeResultHandle, package.Data, socketId);
                        break;
                    case ECTYPE_DefineManager.MSG_ECTYPE_CRUSADE_TIME:
                        this.AddToInvoker(ReceiveEctypeCrusadeTime, package.Data, socketId);
						break;
					case ECTYPE_DefineManager.MSG_ECTYPE_UPDATE_BLOCK:
                        this.AddToInvoker(ReceiveEctypeUpdateBlock, package.Data, socketId);
                        break;
					case ECTYPE_DefineManager.MSG_ECTYPE_RESULT_SWEEP:
						this.AddToInvoker(ReceiveEctypeSweepReward, package.Data, socketId);
						break;
					case ECTYPE_DefineManager.MSG_ECTYPE_RANDOM_REWARD:
						this.AddToInvoker(ReceiveEctypeRandomReward,package.Data,socketId);
						break;
			//case ECTYPE_DefineManager.MSG_ECTYPE_PVP_RESULT:
			//	this.AddToInvoker(ReceiveEctypePVPSettlement, package.Data, socketId);
				break;
			default:
				//TraceUtil.Log("NET_ROOT_ECTYPE" + package.GetSubMsgType());
                        break;
                }
                break;
            default:
                //TraceUtil.Log("不能识别的主消息" + package.GetMasterMsgType());
                break;
        }
    }

    #region 接收消息处理
	CommandCallbackType ReceiveDefenceEctypeResultHandle(byte[] dataBuffer, int socketID)
	{
		SMSGECTYPEDEFINE_RESULT_SC defenceEctypeResult=SMSGECTYPEDEFINE_RESULT_SC.ParsePackage( dataBuffer); 
		GameDataManager.Instance.AddData(DataType.DefenceEctypeResult,defenceEctypeResult);
		RaiseEvent(EventTypeEnum.DefenceEctypeSettleAccount.ToString(),defenceEctypeResult);
		return CommandCallbackType.Continue;
	}
	/// <summary>
	/// 防守副本剩余次数初始化
	/// </summary>
	/// <returns>The ectype define info message.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType ReceiveEctypeDefineInfoMsg(byte[] dataBuffer, int socketID)
	{
		int offset=0;
		int count=0;
		offset=PackageHelper.ReadData(dataBuffer,out count);
		for(int i=0;i<count;i++)
		{
			int ectypeId=0,dwTimes=0;
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out ectypeId);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out dwTimes);

			var ectypeData=EctypeConfigManager.Instance.EctypeSelectConfigFile._dataTable.SingleOrDefault(P=>P._lEctypeID==ectypeId);
			if(ectypeData!=null)
			{
				ectypeData.DefenceChallengeRemainNum=dwTimes;
			}
		}
		return CommandCallbackType.Continue;
	}
	/// <summary>
	/// 防守副本剩余次数更新
	/// </summary>
	/// <returns>The ectype define update message.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType ReceiveEctypeDefineUpdateMsg(byte[] dataBuffer, int socketID)
	{
		int offset=0,ectypeId=0,dwTimes=0;
		offset+=PackageHelper.ReadData(dataBuffer,out ectypeId);
		offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out dwTimes);
		
		var ectypeData=EctypeConfigManager.Instance.EctypeSelectConfigFile._dataTable.SingleOrDefault(P=>P._lEctypeID==ectypeId);
		if(ectypeData!=null)
		{
			ectypeData.DefenceChallengeRemainNum=dwTimes;
		}
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveEctypeFinishMsg(byte[] dataBuffer, int socketID)
	{
		var data=new SMSGECTYPE_FINISH_SC(dataBuffer);
		//发送战斗数据统计
		RaiseEvent(EventTypeEnum.EctypeBattleStatistics.ToString(), null);
		RaiseEvent(EventTypeEnum.EctypeFinish.ToString(),data);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveLoopNumMsg(byte[] dataBuffer, int socketID)
	{
		UIEventManager.Instance.TriggerUIEvent(UIEventType.DefenceLoopNum,BitConverter.ToUInt32(dataBuffer,0));
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveMaxLoopNUmMsg(byte[] dataBuffer, int socketID)
	{
		UIEventManager.Instance.TriggerUIEvent(UIEventType.DefenceMaxLoopNum,BitConverter.ToUInt32(dataBuffer,0));
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveTreasureThestsOpenMsg(byte[] dataBuffer, int socketID)
	{
		TraceUtil.Log(SystemModel.Jiang,"收到翻宝箱成功信息");
		UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenTreasure,null);
		return CommandCallbackType.Continue;
	}

    CommandCallbackType ReceiveBossDeadSettement(byte[] dataBuffer, int socketID)
    {
        UIEventManager.Instance.TriggerUIEvent(UIEventType.BossDeathMsg,null);
        return CommandCallbackType.Continue;
    }

	CommandCallbackType ReceiveYaoNvSkillUseTime(byte[] dataBuffer, int socketID)
	{
		SMSGECTYPEYAONVSKILLUSETIME_SC sMSGECTYPEYAONVSKILLUSETIME_SC = SMSGECTYPEYAONVSKILLUSETIME_SC.ParsePackage(dataBuffer);

		//BattleManager.Instance.ResetSirenSkillUseTime(sMSGECTYPEYAONVSKILLUSETIME_SC.dwUseTime);
		return CommandCallbackType.Continue;
	}

    /// <summary>
    /// /收到失败结算
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    /// <returns></returns>
    CommandCallbackType ReceiveMissionFailSettement(byte[] dataBuffer, int socketID)
    {
        TraceUtil.Log("收到失败结算");
        if (TaskModel.Instance.TaskGuideType == TaskGuideType.Enforce) //NewbieGuideManager_V2.Instance.IsConstraintGuide == true)
        {
            TraceUtil.Log("正在强引导中");
            GameDataManager.Instance.ClearData(DataType.MissionFail);
            return CommandCallbackType.Continue;
        }
        GameDataManager.Instance.ResetData(DataType.MissionFail,"MissionFail");
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// 试炼副本总结算
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    /// <returns></returns>
    CommandCallbackType ReceiveTrialsSettlement(byte[] dataBuffer, int socketID)
    {
        SMSGEctypeTrialsTotalResult_SC sMSGEctypeTrialsTotalResult_SC = SMSGEctypeTrialsTotalResult_SC.ParsePackage(dataBuffer);
        TraceUtil.Log("收到试炼副本总结算:" + sMSGEctypeTrialsTotalResult_SC.dwProgress);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.TrialSettlement, sMSGEctypeTrialsTotalResult_SC);
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// 单个试炼副本波数结算
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    /// <returns></returns>
    CommandCallbackType ReceiveSingleTrialsSettlement(byte[] dataBuffer,int socketID)
    {
        SMSGEctypeTrialsSubResult_SC sMSGEctypeTrialsSubResult_SC = SMSGEctypeTrialsSubResult_SC.ParsePackage(dataBuffer);
        TraceUtil.Log("收到单个试炼副本波数结算消息:" + sMSGEctypeTrialsSubResult_SC.dwProgress);
        GameDataManager.Instance.ResetData(DataType.SingleTrialsSettlement, sMSGEctypeTrialsSubResult_SC);
        //UIEventManager.Instance.TriggerUIEvent(UIEventType.SingleTrialsSettlement,sMSGEctypeTrialsSubResult_SC);
        return CommandCallbackType.Continue;
    }

    /// <summary>
    ///  接收到练功房单个座位更新
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    /// <returns></returns>
    CommandCallbackType ReceiveUpdateEctypeSeatInfo(byte[] dataBuffer,int socketID)
    {
        SMSGEctypeSeatInfo_SC sMSGEctypeSeatInfo_SC = SMSGEctypeSeatInfo_SC.ParsePackage(dataBuffer);
        return CommandCallbackType.Continue;
    }
    
    /// <summary>
    /// 同步练功房信息
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    /// <returns></returns>
    CommandCallbackType ReceiveEctypeAllSeatInfo(byte[] dataBuffer, int socketID)
    {        
        SMSGEctypeALLSeatInfo_SC sMSGEctypeALLSeatInfo_SC = SMSGEctypeALLSeatInfo_SC.ParsePackage(dataBuffer);
        TraceUtil.Log("[sMSGEctypeALLSeatInfo_SC]" + sMSGEctypeALLSeatInfo_SC.dwRoomID);
        TraceUtil.Log("[byRoomType]" + sMSGEctypeALLSeatInfo_SC.byRoomType);
        PlayerRoomManager.Instance.UpdateRoomSeatInfo(sMSGEctypeALLSeatInfo_SC);
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// 试炼副本列表
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    /// <returns></returns>
    CommandCallbackType ReceiveTrialsEctypeList(byte[] dataBuffer, int socketID)
    {
        TraceUtil.Log("收到试炼副本列表:"+dataBuffer.Length);
        SMSGEctypeTrialsInfo_SC sMSGEctypeTrialsInfo_SC = SMSGEctypeTrialsInfo_SC.ParsePackage(dataBuffer);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.TrialsEctypeList, sMSGEctypeTrialsInfo_SC);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveMartialArtsRoomList(byte[] dataBuffer, int socketID)
    {
        TraceUtil.Log("收到练功房房间列表："+dataBuffer[0]);
        SMSGEctypePraicticeList_SC sMSGEctypePraicticeList_SC = SMSGEctypePraicticeList_SC.ParsePackage(dataBuffer);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.MartialArtsRoomList, sMSGEctypePraicticeList_SC);
        return CommandCallbackType.Continue;
    }
	//接收点击宝箱响应//
    CommandCallbackType sMSGEctypeTreasureReward_SCHandel(byte[] dataBuffer, int socketID)
    {
        SMSGEctypeTreasureReward_SC sMSGEctypeTreasureReward_SC = SMSGEctypeTreasureReward_SC.ParsePackage(dataBuffer);
        TraceUtil.Log(SystemModel.Jiang,"收到宝箱奖励：" + sMSGEctypeTreasureReward_SC.dwUID+",MyUID:"+PlayerManager.Instance.FindHeroDataModel().UID);
        EctypeTreasureRewardList ectypeTreasureRewardList = GameDataManager.Instance.PeekData(DataType.EctypeTreasureReward) as EctypeTreasureRewardList;
        ectypeTreasureRewardList = ectypeTreasureRewardList == null ? new EctypeTreasureRewardList() : ectypeTreasureRewardList;
        ectypeTreasureRewardList.TreasureList.Add(sMSGEctypeTreasureReward_SC);
        GameDataManager.Instance.ResetData(DataType.EctypeTreasureReward,ectypeTreasureRewardList);
        //UIEventManager.Instance.TriggerUIEvent(UIEventType.GetEctypeTreasureReward, sMSGEctypeTreasureReward_SC);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType sMSGEctypePlayerReviveHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypePlayerRevive_SC sMSGEctypePlayerRevive_SC = SMSGEctypePlayerRevive_SC.ParsePackage(dataBuffer);
		GameDataManager.Instance.ResetData(DataType.CountDownUI,sMSGEctypePlayerRevive_SC);
        //UIEventManager.Instance.TriggerUIEvent(UIEventType.CountDownUI, sMSGEctypePlayerRevive_SC);
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// 收到技能同步数据消息
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    CommandCallbackType ReceiveSkillDataHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypeSynSkillData_SC sMSGEctypeSynSkillData_SC = SMSGEctypeSynSkillData_SC.ParsePackage(dataBuffer);
        //TraceUtil.Log("收到技能同步数据消息");
        PlayerManager.Instance.EctypeSynSkillDataHandler(sMSGEctypeSynSkillData_SC);
        //UpgradeSkillInfoArray.Instance.SetSkillsInfo(sMSGEctypeSynSkillData_SC);
        return CommandCallbackType.Continue;
    }


    CommandCallbackType StartPrintHandle(byte[] dataBuffer, int socketID)
    {
        MonsterFactory.Instance.StartPrint(0);
        return CommandCallbackType.Continue;            
    }

    CommandCallbackType EndPrintHandle(byte[] dataBuffer, int socketID)
    {
        MonsterFactory.Instance.EndPrint();
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ECTYPE_CLEARANCEHandle(byte[] dataBuffer, int socketID)
    {
        //TraceUtil.Log("============SendEctypeGoBattle##########2");
        ECTYPE_CLEARANCE ectype_CLEARANCE = PackageHelper.BytesToStuct<ECTYPE_CLEARANCE>(dataBuffer);
        //RaiseEvent(EventTypeEnum.RequestEctypeUnlock.ToString(), ectype_CLEARANCE);

        uint _ectypeid = ectype_CLEARANCE.dwEctypeId;
        string ectypeid = ectype_CLEARANCE.dwEctypeId.ToString();

        //TraceUtil.Log("$$$$$$$$$$$$$$$$$$$$$$$+?????????±±?????ectype_CLEARANCE.dwEctypeID" + ectype_CLEARANCE.dwEctypeId);           
        if (ectypeid.Length > 5)
        {
            
            int _ectypeID = int.Parse(ectypeid.Substring(0, 5));
            int _diff = int.Parse(ectypeid.Substring(4, 3));

            EctypeSelectConfigData item = EctypeConfigManager.Instance.EctypeSelectConfigList[_ectypeID];
            _ectypeid = (uint)item._vectContainer[_diff - 1];
            //TraceUtil.Log("#################_ectypeid" + _ectypeid);
        }

        //NewbieGuideManager.Instance.SetPassEctypeID(_ectypeid);
        StroyLineDataManager.Instance.SetPassEctypeID(_ectypeid);
		
        return CommandCallbackType.Continue;
    }


    CommandCallbackType EctypeErrorCodeHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypeResult_SC sMSGEctypeResult_SC = SMSGEctypeResult_SC.ParsePackage(dataBuffer);
        RaiseEvent(EventTypeEnum.EctypeNoQualification.ToString(), sMSGEctypeResult_SC);
        return CommandCallbackType.Continue;
    }
    ///// <summary>
    ///// 收到难度选择页面消息
    ///// </summary>
    ///// <param name="dataBuffer"></param>
    ///// <param name="socketID"></param>
    //CommandCallbackType EctypeLevelDataHandle(byte[] dataBuffer, int socketID)
    //{
    //    SMSGOpenEctypeLevelDatas_SC sMSGOpenEctypeLevelDatas_SC = SMSGOpenEctypeLevelDatas_SC.ParsePackage(dataBuffer);

    //    RaiseEvent(EventTypeEnum.EctypeConfig.ToString(), sMSGOpenEctypeLevelDatas_SC);
    //    return CommandCallbackType.Continue;
    //}

    ///// <summary>
    ///// 收到世界地图的消息
    ///// </summary>
    ///// <param name="dataBuffer"></param>
    ///// <param name="socketID"></param>
    //CommandCallbackType WorldMapSelectHandle(byte[] dataBuffer, int socketID)
    //{
    //    var ectypeConfig = SMSGEctypeWorldMapSelect_SC.ParsePackage(dataBuffer);

    //    //已经过服务端验证，此处不做队长验证

    //    int[] desMapID = EctypeConfigManager.Instance.PortalConfigList[ectypeConfig.dwSid]._desMapID;
    //    //Created By WYN
    //    WorldMapController.Instance.InitIds(desMapID);
    //    MainUIController.Instance.OpenMainUI(UIType.WorldMap);
    //    return CommandCallbackType.Continue;
    //}

    //public CommandCallbackType EctypeRequestError(byte[] dataBuffer, int socketID)
    //{
    //    //TraceUtil.Log("请求副本失败...");
    //    return CommandCallbackType.Continue;
    //}
    /// <summary>
    /// 副本结算
    /// </summary>
    CommandCallbackType EctypeSettleAccountHandle(byte[] dataBuffer, int socketID)
    {
        var settleAccount = SMSGEctypeSettleAccounts.ParsePackage(dataBuffer);
        //Debu.log("连击" + settleAccount.dwHighestCombo);
        Debug.LogWarning("收到副本结算消息：");
        //NewbieGuideManager.Instance.IsCompleteEctype = true;
        RaiseEvent(EventTypeEnum.EctypeSettleAccount.ToString(), settleAccount);
        return CommandCallbackType.Continue;
    }
    
    //接收副本信息同步及打开副本消息
    CommandCallbackType EctypeSelectHandle(byte[] dataBuffer, int socketID)
    {
		EctypeModel.Instance.sMSGEctypeSelect_SC = SMSGEctypeSelect_SC.ParsePackage(dataBuffer);
		//UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypeUIInfo,sMSGEctypeSelect_SC);
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// 收到单个副本更新消息
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    /// <returns></returns>
    CommandCallbackType EctypeLevelDataHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypeLevelData_SC sMSGEctypeLevelData_SC = SMSGEctypeLevelData_SC.ParsePackage(dataBuffer);        
        UIEventManager.Instance.TriggerUIEvent(UIEventType.SingleEctypeUIInfo, sMSGEctypeLevelData_SC);
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// 收到服务器loading完成信息
    /// </summary>
    /// <returns></returns>
    CommandCallbackType ResponeTeamateEnterEctype(byte[] dataBuffer,int socketID)
    {
        SMSGResponeTeamateEnterEctype_SC sMSGResponeTeamateEnterEctype_SC = SMSGResponeTeamateEnterEctype_SC.ParsePackage(dataBuffer);
        TraceUtil.Log("收到服务器loading完成返回消息：" + sMSGResponeTeamateEnterEctype_SC.dwEctypeTimes);
        GameDataManager.Instance.ResetData(DataType.LoadingSceneComplete,sMSGResponeTeamateEnterEctype_SC);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.LoadBattleSceneCompleteSC,null);
        return CommandCallbackType.Continue;
    }

    //收到查找到的pvp玩家信息
    CommandCallbackType PVPFindPlayerHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypeFindPlayer_SC sMSGEctypeFindPlayer_SC = SMSGEctypeFindPlayer_SC.ParsePackage(dataBuffer);
        var info = sMSGEctypeFindPlayer_SC.EctypePvpPlayer;
        //TraceUtil.Log("收到查找到的pvp玩家信息: " + info.uidEntity + " , " + info.dwActorId + " , " + info.nMaxHP + " , " + info.szName + " , " + info.nLev);
        if (sMSGEctypeFindPlayer_SC.byFindNum > 0)
        {
            PVPBattleManager.Instance.SavePVPPlayerData(info);//储存pvp玩家信息            
        }
        RaiseEvent(EventTypeEnum.PVPFindPlayer.ToString(), sMSGEctypeFindPlayer_SC);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType PVPReadyHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypePvpReady_SC sMSGEctypePvpReady_SC = SMSGEctypePvpReady_SC.ParsePackage(dataBuffer);
        //TraceUtil.Log("=======准备读秒: " + sMSGEctypePvpReady_SC.dwReadyTime);
        RaiseEvent(EventTypeEnum.PVPReady.ToString(), sMSGEctypePvpReady_SC);
        
        return CommandCallbackType.Continue;
    }
    CommandCallbackType PVPFightingHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypePvpFighting_SC sMSGEctypePvpFighting_SC = SMSGEctypePvpFighting_SC.ParsePackage(dataBuffer);
        //TraceUtil.Log("=======战斗回合时间: " + sMSGEctypePvpFighting_SC.dwFightingTime);
        RaiseEvent(EventTypeEnum.PVPFighting.ToString(), sMSGEctypePvpFighting_SC);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType PVPSettleAccountHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypePvpSettleAccounts_SC sMSGEctypePvpSettleAccounts_SC = SMSGEctypePvpSettleAccounts_SC.ParsePackage(dataBuffer);
        //TraceUtil.Log("=======结算 获得威望: " + sMSGEctypePvpSettleAccounts_SC.dwPrestigeReward);
        RaiseEvent(EventTypeEnum.PVPSettleAccount.ToString(), sMSGEctypePvpSettleAccounts_SC);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType EctypeFightModeHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypeFightMode_SC sMSGEctypeFightMode_SC = SMSGEctypeFightMode_SC.ParsePackage(dataBuffer);
        if (sMSGEctypeFightMode_SC.byType == 0)
        {
            GameManager.Instance.CurrentGameMode = GameMode.MULTI_PLAYER;
        }
        else if (sMSGEctypeFightMode_SC.byType == 1)
        {
            GameManager.Instance.CurrentGameMode = GameMode.SINGLE_PLAYER;
        }
        return CommandCallbackType.Continue;
    }
    CommandCallbackType InitializeEctypeHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = SMSGEctypeInitialize_SC.ParsePackage(dataBuffer);        
        GameDataManager.Instance.ResetData(DataType.InitializeEctype, sMSGEctypeInitialize_SC);
		EctypeManager.Instance.Set(sMSGEctypeInitialize_SC);
		EctypeContainerData ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
		GameManager.Instance.ectypeType = (EEcytpeBattleType)ectypeData.lEctypeType;
        TraceUtil.Log("收到副本初始信息");
        return CommandCallbackType.Continue;
    }
	
	CommandCallbackType ReceiveEctypePropsHandle(byte[] dataBuffer, int socketID)
	{
		SMsgEctypeMemberUpdateProp_SC sMsgEctypeMemberUpdateProp_SC = SMsgEctypeMemberUpdateProp_SC.ParsePackage(dataBuffer);
		EctypeManager.Instance.UpdateProp(sMsgEctypeMemberUpdateProp_SC.dwActorID,sMsgEctypeMemberUpdateProp_SC.wProp,sMsgEctypeMemberUpdateProp_SC.nValue);
		return CommandCallbackType.Continue;
	}
    //更新妖气值
    CommandCallbackType UpdateYaoqiHandle(byte[] dataBuffer, int socketID)
    {
        SMSGEctypeYaoqiProp_SC sMSGEctypeYaoqiProp_SC = SMSGEctypeYaoqiProp_SC.ParsePackage(dataBuffer);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.UpdateYaoqiValue, sMSGEctypeYaoqiProp_SC);
        return CommandCallbackType.Continue;
    }
    //ReceivePracticeYaoNvUpdateHandle
    CommandCallbackType ReceivePracticeYaoNvUpdateHandle(byte[] dataBuffer, int socketID)
    {
        SMsgEctypePracice_YaoNvUpdate_CSC sMsgEctypePracice_YaoNvUpdate_CSC = SMsgEctypePracice_YaoNvUpdate_CSC.ParsePackage(dataBuffer);
        sMsgEctypePracice_YaoNvUpdate_CSC.dwYaoNvList.ApplyAllItem(p =>
            {
                TraceUtil.Log("[dwYaoNvList]" + p);
            });
        PlayerRoomManager.Instance.UpdateYaoNvUpdateInfo(sMsgEctypePracice_YaoNvUpdate_CSC);
        RaiseEvent(EventTypeEnum.UpdateRoomYaoNv.ToString(), sMsgEctypePracice_YaoNvUpdate_CSC);
        return CommandCallbackType.Continue;
    }
	//副本更新//
	//常规副本信息
	CommandCallbackType ReceiveEctypeNomalDataHandle(byte[] dataBuffer, int socketID)
	{
		EctypeModel.Instance.sMSGEctypeSelect_SC = SMSGEctypeSelect_SC.ParsePackage(dataBuffer);
		return CommandCallbackType.Continue;
	}
	//常规副本信息更新
	CommandCallbackType ReceiveEctypeNomalUpdateHandle(byte[] dataBuffer, int socketID)
	{
		SMSGEctypeData_SC sMSGEctypeData_SC = SMSGEctypeData_SC.ParsePackage(dataBuffer);
		EctypeModel.Instance.ReceiveEctypeNomalUpdate (sMSGEctypeData_SC);
		return CommandCallbackType.Continue;
	}
	//更新副本宝箱信息
	CommandCallbackType ReceiveChessInfoUpdateHandle(byte[] dataBuffer, int socketID)
	{
		SMSGEctypeChest_SC sMSGEctypeChest_SC = SMSGEctypeChest_SC.ParsePackage(dataBuffer);
		EctypeModel.Instance.ReceiveChessInfoUpdate (sMSGEctypeChest_SC);
		return CommandCallbackType.Continue;
	}

	//无尽试炼
	//无尽副本波数
	CommandCallbackType ReceiveEndlessLoopNumHandle(byte[] dataBuffer, int socketID)
	{
		SMsgEctypeEndless_LoopNum_SC sMsgEctypeEndless_LoopNum_SC = SMsgEctypeEndless_LoopNum_SC.ParsePackage(dataBuffer);
		EctypeModel.Instance.ReceiveCurLoopNumUdate (sMsgEctypeEndless_LoopNum_SC);
		return CommandCallbackType.Continue;
	}
	//无尽副本闯过的波数
	CommandCallbackType ReceiveEndlessPassLoopHandle(byte[] dataBuffer, int socketID)
	{
		SMsgEctypeEndless_Reward_SC sMsgEctypeEndless_Reward_SC = SMsgEctypeEndless_Reward_SC.ParsePackage(dataBuffer);
		EctypeModel.Instance.ReceivePassLoopNumUdate (sMsgEctypeEndless_Reward_SC);
		return CommandCallbackType.Continue;
	}
	//无尽副本结算时最终闯过的波数
	CommandCallbackType ReceiveEndlessFinishPassLoopHandle(byte[] dataBuffer, int socketID)
	{
		SMsgEctypeEndless_Result_SC sMsgEctypeEndless_Result_SC = SMsgEctypeEndless_Result_SC.ParsePackage(dataBuffer);
		EctypeModel.Instance.ReceiveFinishPassLoopNumUdate (sMsgEctypeEndless_Result_SC);
		return CommandCallbackType.Continue;
	}
	//无尽副本界面数据
	CommandCallbackType ReceiveEndlessBestDataHandle(byte[] dataBuffer, int socketID)
	{
		SMsgEctypeEndless_Info_SC sMsgEctypeEndless_Info_SC = SMsgEctypeEndless_Info_SC.ParsePackage(dataBuffer);
		EctypeModel.Instance.ReceiveBestData (sMsgEctypeEndless_Info_SC);
		return CommandCallbackType.Continue;
	}
	//无尽副本界面数据更新
	CommandCallbackType ReceiveEndlessBestDataUpdateHandle(byte[] dataBuffer, int socketID)
	{
		SMsgEctypeEndless_Info_Updata_SC sMsgEctypeEndless_Info_Updata_SC = SMsgEctypeEndless_Info_Updata_SC.ParsePackage(dataBuffer);
		EctypeModel.Instance.ReceiveBestDataUpdate (sMsgEctypeEndless_Info_Updata_SC);
		return CommandCallbackType.Continue;
	}
	//无尽副本断线重连
	CommandCallbackType ReceiveEndlessAgainConnectHandle(byte[] dataBuffer, int socketID)
	{
		SMsgEctypeEndless_LoopTime_SC sMsgEctypeEndless_LoopTime_SC = SMsgEctypeEndless_LoopTime_SC.ParsePackage(dataBuffer);
		EctypeModel.Instance.againConnectTime = sMsgEctypeEndless_LoopTime_SC.dwTime;
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EndLessAgainConnectTime, sMsgEctypeEndless_LoopTime_SC.dwTime);
		return CommandCallbackType.Continue;
	}
	//无尽副本，场景跳转倒计时
	CommandCallbackType ReceiveEndlessSceneTimeHandle(byte[] dataBuffer, int socketID)
	{
		SMsgEctypeEndless_MapJumpTime_SC sMsgEctypeEndless_MapJumpTime_SC = SMsgEctypeEndless_MapJumpTime_SC.ParsePackage(dataBuffer);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EndLessJumpSceneTime, sMsgEctypeEndless_MapJumpTime_SC.dwTime/1000);
		return CommandCallbackType.Continue;
	}

	//讨伐结算
	CommandCallbackType ReceiveEctypeCrusadeResultHandle(byte[] dataBuffer, int socketID)
	{
		SMSGECTYPE_CRUSADERESULT_SC sMSGECTYPE_CRUSADERESULT_SC = SMSGECTYPE_CRUSADERESULT_SC.ParsePackage(dataBuffer);
		TraceUtil.Log(SystemModel.Lee,"讨伐结算");
		UIEventManager.Instance.TriggerUIEvent(UIEventType.CrusadeSettlement,sMSGECTYPE_CRUSADERESULT_SC);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveEctypeCrusadeTime(byte[] dataBuffer, int socketID)
	{
		SMSGECTYPE_CRUSADETIME_SC sMSGECTYPE_CRUSADETIME_SC = SMSGECTYPE_CRUSADETIME_SC.ParsePackage(dataBuffer);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.CrusadeTiming,sMSGECTYPE_CRUSADETIME_SC);
		return CommandCallbackType.Continue;
	}
    CommandCallbackType ReceiveEctypeUpdateBlock(byte[] dataBuffer, int socketID)
    {
        SMsgEctypeUpDateBlock sMsgEctypeUpDateBlock = SMsgEctypeUpDateBlock.ParsePackage(dataBuffer);
        BattleManager.Instance.BlockAppear(sMsgEctypeUpDateBlock);
        return CommandCallbackType.Continue;
    }
	CommandCallbackType ReceiveEctypeSweepReward(byte[] dataBuffer, int socketID)
	{
		SMsgEctypeResult_Sweep_SC sMsgEctypeResult_Sweep_SC = SMsgEctypeResult_Sweep_SC.ParsePackage(dataBuffer);
		EctypeModel.Instance.SweepGetReward (sMsgEctypeResult_Sweep_SC);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveEctypeRandomReward(byte[] dataBuffer,int socketID)
	{
		SMsgEctypeRandom_Reward_SC sMsgEctypeRandom_Reward_SC = SMsgEctypeRandom_Reward_SC.ParsePackage(dataBuffer);
		UI.GoodsMessageManager.Instance.Show (sMsgEctypeRandom_Reward_SC.dwEquipId,sMsgEctypeRandom_Reward_SC.dwEquipNum);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.UpdateRandomRewardStatus,null);
		return CommandCallbackType.Continue;
	}

	//青龙会团队PVP结算消息处理
	CommandCallbackType ReceiveEctypePVPSettlement(byte[] databuffer, int socketID)
	{
		//SMsgEctypePVP_Result_SC sMsgEctypePVP_Result_SC = SMsgEctypePVP_Result_SC.ParsePackage(databuffer);
		//UIEventManager.Instance.TriggerUIEvent(UIEventType.QinglongPVPSettlementEvent, sMsgEctypePVP_Result_SC);
		return CommandCallbackType.Continue;
	}
	#endregion
	
	#region 发送副本请求消息处理
    /// <summary>
    /// 获取试炼副本列表
    /// </summary>
    public void SendGetTrialsEctypePanelInfo()
    {
        GetTrialsEctypeList getTrialsEctypeList = new GetTrialsEctypeList();
        this.Request(getTrialsEctypeList.GeneratePackage());
    }
    /// <summary>
    /// 进入触发区域
    /// </summary>
    public void SendEnterTriggerArea(int areaId)
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_ENTER_AREA);

        pak.Data = BitConverter.GetBytes(areaId);
        this.Request(pak);
    }
    /// <summary>
    /// 快速加入练功房
    /// </summary>
    public void SendQuickJoinMartialArtsRoom()
    {
        SMSGEctypePraicticeQuickEnter_SC sMSGEctypePraicticeQuickEnter_SC = new SMSGEctypePraicticeQuickEnter_SC();
        this.Request(sMSGEctypePraicticeQuickEnter_SC.GeneratePackage());
    }

    /// <summary>
    /// 发送加入练功房请求
    /// </summary>
    /// <param name="RoomID"></param>
    public void SendJoinMartialArtsRoom(uint RoomID)
    {
        SMSGEctypePraicticeEnter_SC sMSGEctypePraicticeEnter_SC = new SMSGEctypePraicticeEnter_SC() { dwRoomID = RoomID};
        this.Request(sMSGEctypePraicticeEnter_SC.GeneratePackage());
    }

    /// <summary>
    /// 获取练功房列表
    /// </summary>
    public void SendGetMartialArtsRoomListMsg()
    {
        GetMartialArtsRoomList getMartialArtsRoomList = new GetMartialArtsRoomList();
        this.Request(getMartialArtsRoomList.GeneratePackage());
    }
	/// <summary>
	/// 城镇内副本界面发送打开宝箱请求
	/// </summary>
	public void SendSmsgOpenTreasureInTown(int ectypeID)
	{
		SMSGECTYPEOPNE_CHEST_CSC sMSGECTYPEOPNE_CHEST_CSC = new SMSGECTYPEOPNE_CHEST_CSC()
		{
			dwEctypeID = (uint)ectypeID,
		};
		this.Request(sMSGECTYPEOPNE_CHEST_CSC.GeneratePackage());
	}

    /// <summary>
	/// 副本内发送打开宝箱请求
    /// </summary>
    public void SendSMSGEctypeClickTreasure_CS(byte type)
    {
        long UID = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
		SMSGEctypeClickTreasure_CS sMSGEctypeClickTreasure_CS = new SMSGEctypeClickTreasure_CS(){byClickType = type};
        this.Request(sMSGEctypeClickTreasure_CS.GeneratePackage());
    }

    /// <summary>
    /// 发送跳转副本请求
    /// </summary>
    public void SendEctypeChallengeComplete_CS()
    {
        long UID = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
        SMSGEctypeChallengeComplete_CS sMSGEctypeClickTreasure_CS = new SMSGEctypeChallengeComplete_CS()
        {
            uidEntity = UID,
        };
        this.Request(sMSGEctypeClickTreasure_CS.GeneratePackage());
    }

    /// <summary>
    /// 副本请求消息
    /// </summary>
    /// <param name="sMSGEctypeRequestCreate_CS"></param>
    public void SendEctypeRequest(SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS)
    {
        this.Request(sMSGEctypeRequestCreate_CS.GeneratePackage(MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_REQUESTCREATE));
    }

    ///// <summary>
    ///// 世界地图请求消息
    ///// </summary>
    ///// <param name="sMSGRequestEctypeLevelDatas_CS"></param>
    //public void SendWorldMapRequest(SMSGRequestEctypeLevelDatas_CS sMSGRequestEctypeLevelDatas_CS)
    //{
    //    this.Request(sMSGRequestEctypeLevelDatas_CS.GeneratePackage(MasterMsgType.NET_ROOT_ECTYPE, EctypeDefineManager.MSG_ECTYPE_OPENECTYPECONTAINER_SELECT));
    //}

    //public void SenPlayerDieRequest(long uid)
    //{
    //    Package pkg = new Package();
    //    pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, EctypeDefineManager.MSG_ECTYPE_PLAYERDONTREVIVE);
    //    pkg.Data = BitConverter.GetBytes(uid);

    //    this.Request(pkg);
    //}
    /// <summary>
    /// 发送打开副本UI请求
    /// </summary>
    /// <param name="uid"></param>
     public void SendEctypeGoBattleRequest(long uid)
     {
         Package pkg = new Package();
         SMSGEctypeGoBattle_CS sMSGEctypeGoBattle_CS = new SMSGEctypeGoBattle_CS(){ uidEntity = uid, };
         pkg = sMSGEctypeGoBattle_CS.GeneratePackage(MasterMsgType.NET_ROOT_ECTYPE,ECTYPE_DefineManager.MSG_ECTYPE_GOBATTLE);
         this.Request(pkg);
     }

     /// <summary>
     /// 发送玩家最高通关副本ID请求
     /// </summary>
     /// <param name="uid"></param>
     public void SendEctypeClearance()
     {
         Package pkg = new Package();
         pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_CLEARANCE);
         pkg.Data = null;

         this.Request(pkg);
     }

     /// <summary>
     /// 创建有新手引导副本,应亚健要求。2014-5-21把副本请求消息合并
     /// </summary>
     /// <param name="uid"></param>
     public void SendEctypeGuideCreate(SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS)
     {
         this.Request(sMSGEctypeRequestCreate_CS.GeneratePackage(MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_REQUESTCREATE));
         //this.Request(sMSGEctypeRequestCreate_CS.GeneratePackage(MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_GUIDECREATE));
     }

     /// <summary>
     /// 完成副本新手引导
     /// </summary>
     /// <param name="uid"></param>
     public void SendEctypeGuideFinish()
     {
         Package pkg = new Package();
         pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_GUIDEFINISH);
         //pkg.Data = BitConverter.GetBytes(uid);

         this.Request(pkg);
     }

    /// <summary>
    /// 发送获取单个副本信息请求
    /// </summary>
    /// <param name="DwEctypeId"></param>
    /// <param name="ByEctypeDiff"></param>
     public void SendRequestEctypeLevelDataRequest(int DwEctypeId, byte ByEctypeDiff)
     {
         long UidEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
         SMSGRequestEctypeLevelData_CS sMSGRequestEctypeLevelData_CS = new SMSGRequestEctypeLevelData_CS()
         {
             uidEntity = UidEntity,
             dwEctypeId = (uint)DwEctypeId,
             byEctypeDiff = ByEctypeDiff
         };
         this.Request(sMSGRequestEctypeLevelData_CS.GeneratePackge(MasterMsgType.NET_ROOT_ECTYPE,ECTYPE_DefineManager.MSG_ECTYPE_REQUESTLEVELDATA));
     }

    //通知服务器副本加载完成
     public void SendTeamateRequestEnterEctype()
     {
         SMSGTeamateRequestEnterEctype_CS sMSGTeamateRequestEnterEctype_CS = new SMSGTeamateRequestEnterEctype_CS()
         {
             uidEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity,
              byOperationType=(byte)(GameManager.Instance.UseJoyStick?1:0)
              //  = (byte)(NewbieGuideManager_V2.Instance.IsEctypeGuide?0:1)
         };
         TraceUtil.Log("通知服务器副本加载完成:" + sMSGTeamateRequestEnterEctype_CS.byOperationType);
         Log.Instance.WriteLog("通知服务器副本加载完成:" + sMSGTeamateRequestEnterEctype_CS.byOperationType);
         this.Request(sMSGTeamateRequestEnterEctype_CS.GeneratePackage(MasterMsgType.NET_ROOT_ECTYPE,ECTYPE_DefineManager.MSG_ECTYPE_ENTERECTYPE_READY));
     }
    /// <summary>
     /// 发送复活请求
    /// </summary>
     /// <param name="UIdEntityReq">请求者</param>
     /// <param name="UidEntityBeReq">被请求者</param>
     /// <param name="ectypeRevive">1/4状态, 或满状态</param>
     //public void SendEctypeRequestRevive(long UIdEntityReq, long UidEntityBeReq, EctypeRevive ectypeRevive)
     //{
     //    SMSGEctypeRequestRevive_CSC sMSGEctypeRequestRevive_CSC = new SMSGEctypeRequestRevive_CSC()
     //    {
     //        uidEntityReq = UIdEntityReq,
     //        uidEntityBeReq = UidEntityBeReq,
     //        byType = (byte)ectypeRevive,
     //    };
     //}
    /// <summary>
    /// 发送返回城镇请求
    /// </summary>
     public void SendEctypeRequestReturnCity(long UIDEntity)
     {
         SMSGEctypeRequestReturnCity_CS sMSGEctypeRequestReturnCity_CS = new SMSGEctypeRequestReturnCity_CS()
         {
             uidEntity = UIDEntity,
         };
         this.Request(sMSGEctypeRequestReturnCity_CS.GeneratePackage());
     }

    /// <summary>
    /// 上发是否挑战指令
    /// </summary>
    /// <param name="isChanllenging">true 确定;false 取消</param>
    public void SendEctypeChanllengePvp(bool isChanllenging)
    {
        SMSGEctypeChanllengePvp_CS sMSGEctypeChanllengePvp_CS = new SMSGEctypeChanllengePvp_CS()
        {
            byChallengeType = (byte)(isChanllenging == true? 1 : 2)
        };
        this.Request(sMSGEctypeChanllengePvp_CS.GeneratePackage());
    }
    /// <summary>
    /// 上发角色挑衅动画完成指令
    /// </summary>
    public void SendEctypePvpActionDone()
    {
        SMSGEctypePvpActionDone_CS sMSGEctypePvpActionDone_CS = new SMSGEctypePvpActionDone_CS();
        var playerData = PlayerManager.Instance.FindHeroDataModel();
        sMSGEctypePvpActionDone_CS.dwActorId = (uint)playerData.ActorID;
        this.Request(sMSGEctypePvpActionDone_CS.GeneratePackage());       
    }
    /// <summary>
    /// 上发逃跑指令
    /// </summary>
    public void SendEctypeRunAway()
    {
        SMSGEctypeRunAway_CS sMSGEctypeRunAway_CS = new SMSGEctypeRunAway_CS();
        var playerData = PlayerManager.Instance.FindHeroDataModel();
        sMSGEctypeRunAway_CS.dwActorID = (uint)playerData.ActorID;
        this.Request(sMSGEctypeRunAway_CS.GeneratePackage());
    }
    /// <summary>
    /// 上发增加妖气值请求
    /// </summary>
    /// <param name="ectypeSection"></param>
    public void SendAddYaoqi(int ectypeSection)
    {
        SMSGEctypeAddYaoqiProp_CS sMSGEctypeAddYaoqiProp_CS = new SMSGEctypeAddYaoqiProp_CS()
        {
            dwEctypeSection = (uint)ectypeSection,
            dwGoodId = 3050005
        };
        this.Request(sMSGEctypeAddYaoqiProp_CS.GeneratePackage());
    }
    /// <summary>
    /// 同步妖女更新请求
    /// </summary>
    /// <param name="yaoNvList"></param>
    public void SendYaoNvUpdate(uint[] yaoNvList)
    {
        SMsgEctypePracice_YaoNvUpdate_CSC sMsgEctypePracice_YaoNvUpdate_CSC = new SMsgEctypePracice_YaoNvUpdate_CSC()
        {
            dwYaoNvList = yaoNvList
        };
        this.Request(sMsgEctypePracice_YaoNvUpdate_CSC.GeneratePackage());
    }
	/// <summary>
	/// 发送副本统计数据到服务端
	/// </summary>
	/// <param name="sendData">Send data.</param>
	public void SendBattleCollectToSever(SMSGECTYPELEVEL_COLLECTINFO_CS sendData)
	{
		this.Request(sendData.GeneratePackage());
	}

	/// <summary>
	/// 使用药品
	/// </summary>
	public void SendUseMedicament()
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_USE_MEDICAMENT);
		this.Request(pkg);
	}
	//剧情播放完后，模糊请求副本
	public void SendStoryOverRequestEctype(SMsgActionTeleportTo_CS sendData)
	{
		this.Request(sendData.GeneratePackage());
	}
    #endregion
	
    #region sweep
	public void SendRequestOpenSweep(SMsgEctypeUnLock_Sweep_CS sendData)
	{
		this.Request(sendData.GeneratePackage());
	}
	public void SendRequestSweep(SMsgEctypeBegin_Sweep_CS sendData)
	{
		this.Request(sendData.GeneratePackage());
	}
    #endregion

	/// <summary>
	/// 获取首战奖励
	/// </summary>
	public void SendEctypeRandomReward()
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE,ECTYPE_DefineManager.MSG_ECTYPE_RANDOM_REWARD);
		this.Request(pkg);
	}

}


