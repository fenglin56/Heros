using UnityEngine;
using System.Collections;

public class BattleService :Service {

    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        //TraceUtil.Log("收到战斗主消息" + masterMsgType + "收到子消息" + package.GetSubMsgType());
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_FIGHT:
                switch (package.GetSubMsgType())
                {                   
                    case FightDefineManager.MSG_FIGHT_FINISH:
                        break;
                    case FightDefineManager.MSG_FIGHT_TALK:
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_COMMAND:
                        AddToInvoker(ReceiveBattleCommand, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_CALCULATE_EFFECT:
                        AddToInvoker(ReceiveBattleCalculateEffect, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_BEAT_BACK:
                        AddToInvoker(ReceiveBattleBeatBack, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_UPDATE_BULLET:
                        AddToInvoker(ReceiveBattleUpdateBullet, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_BULLET_DESTORY:
                        AddToInvoker(ReceiveBattleBulletDestory, package.Data, socketId);                       
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_BREAK_SKILL:
                        AddToInvoker(ReceiveBattleBreakSkill, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_UPDATE_BATTERCOUNT:
                        AddToInvoker(ReceiveSMsgFightBatterCount, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_UPDATE_KILLCOUNT:
                        AddToInvoker(ReceiveSMsgFightKillCount, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_HIT_FLY:
                        AddToInvoker(ReceiveFightHitFly, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_TELEPORT:
                        AddToInvoker(ReceiveFightTeleport, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_CHANGE_DIRECT:
                        AddToInvoker(ReceiveFightChangeDirect, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_ADSORB:
                        AddToInvoker(ReceiveBattleBeAdsorb, package.Data, socketId);
                        break;
                    case FightDefineManager.MSG_FIGHT_BATTLE_HORDE:   //定身术
                        //TraceUtil.Log("收到定身消息");
                        AddToInvoker(ReceiveBattleHorde, package.Data, socketId);
                        break;
                    //case FightDefineManager.MSG_FIGHT_BATTLE_BLOODSUCKING:
                    //    TraceUtil.Log("吸血消息");
                    //    this.AddToInvoker(BattleBloodSuckingHandle, package.Data, socketId);
                    //    break;
				
					case FightDefineManager.MSG_FIGHT_BATTLE_ADSORPTIONEX:
						AddToInvoker(ReceiveBattleBeAdsorbEx, package.Data, socketId);
						break;
				
					case FightDefineManager.MSG_FIGHT_BATTLE_COMMAND_SINGLE:
						AddToInvoker(ReceiveBattleCommondSingle, package.Data, socketId);
						break;
                    default:
                        //TraceUtil.Log("NET_ROOT_FIGHT" + package.GetSubMsgType());
                        break;
                }
                break;
            default:
                //TraceUtil.Log("不能识别的主消息" + package.GetMasterMsgType());
                break;
        }
    }


    #region 接收消息处理

    CommandCallbackType ReceiveSMsgFightBatterCount(byte[] dataBuffer, int socketID)
    {
        SMsgFightBatterCount_SC sMsgFightBatterCount_SC = SMsgFightBatterCount_SC.ParsePackage(dataBuffer);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.DoubleHitUI, sMsgFightBatterCount_SC.batterNum);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveSMsgFightKillCount(byte[] dataBuffer, int socketID)
    {
        SMsgFightKillCount_SC sMsgFightBatterCount_SC = SMsgFightKillCount_SC.ParsePackage(dataBuffer);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.DoubleKillUI, sMsgFightBatterCount_SC.wKillNum);
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// 收到战斗指令
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    CommandCallbackType ReceiveBattleCommand(byte[] dataBuffer, int socketID)
    {   
        //场景加载完成前不处理战斗结果消息（组队）

		var sMsgBattleCommand = SMsgBattleCommand.ParseCommandPackage(dataBuffer);
        if (!GameManager.Instance.CreateEntityIM)
		{
			PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("ReceiveBattleCommand",()=>
			                                                         {			
				RaiseEvent(EventTypeEnum.FightCommand.ToString(), sMsgBattleCommand);
			});
		}
		else
		{
			RaiseEvent(EventTypeEnum.FightCommand.ToString(), sMsgBattleCommand);
		}			        	   
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveBattleCalculateEffect(byte[] dataBuffer, int socketID)
    {
        //场景加载完成前不处理战斗结果消息（组队）
        if (!GameManager.Instance.CreateEntityIM)
            return CommandCallbackType.Continue;
        var sMsgEffectContextNum_SC = SMsgEffectContextNum_SC.ParseResultPackage(dataBuffer);

        for(int i = 0; i < sMsgEffectContextNum_SC.byContextNum; i++)
        {
            RaiseEvent(EventTypeEnum.S_CSMsgFightFightToResult.ToString(), sMsgEffectContextNum_SC.list[i]);
        }
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveBattleBeatBack(byte[] dataBuffer, int socketID)
    {
        //场景加载完成前不处理战斗结果消息（组队）
        if (!GameManager.Instance.CreateEntityIM)
            return CommandCallbackType.Continue;
        SMsgBeatBackContextNum_SC sMsgBeatBackContextNum_SC = SMsgBeatBackContextNum_SC.ParseResultPackage(dataBuffer);
        for(int i = 0; i < sMsgBeatBackContextNum_SC.byContextNum; i++)
        {
            RaiseEvent(EventTypeEnum.BeatBack.ToString(), sMsgBeatBackContextNum_SC.list[i]);
        }
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveFightTeleport(byte[] dataBuffer, int socketID)
    {
        //场景加载完成前不处理战斗结果消息（组队）
        if (!GameManager.Instance.CreateEntityIM)
            return CommandCallbackType.Continue;
        SMsgFightTeleport_CSC sMsgFightTeleport_CSC = SMsgFightTeleport_CSC.ParseResultPackage(dataBuffer);

        RaiseEvent(EventTypeEnum.Teleport.ToString(), sMsgFightTeleport_CSC);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveFightHitFly(byte[] dataBuffer, int socketID)
    {
        //场景加载完成前不处理战斗结果消息（组队）
        if (!GameManager.Instance.CreateEntityIM)
            return CommandCallbackType.Continue;
        SMsgHitFlyContextNum_SC sMsgHitFlyContextNum_SC = SMsgHitFlyContextNum_SC.ParseResultPackage(dataBuffer);

        //TraceUtil.Log("击飞: uid=" + sMsgFightHitFly_SC.uidFighter+
        //    " posx=" + sMsgFightHitFly_SC.hitedPosX + " posy=" + sMsgFightHitFly_SC.hitedPosY +
        //     " directionX" + sMsgFightHitFly_SC.directionX + " directionY" + sMsgFightHitFly_SC.directionY + " high" + sMsgFightHitFly_SC.hSpedd);
        for(int i = 0; i < sMsgHitFlyContextNum_SC.byContextNum; i++)
        {
            RaiseEvent(EventTypeEnum.FightFly.ToString(), sMsgHitFlyContextNum_SC.list[i]);
        }
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveBattleUpdateBullet(byte[] dataBuffer, int socketID)
    {
        var sMsgSyncBulletID_SC = SMsgSyncBulletID_SC.ParseResultPackage(dataBuffer);

        //\
        BulletManager.Instance.SynchronousEntity(sMsgSyncBulletID_SC.BulletIndex, sMsgSyncBulletID_SC.uidFighter);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveBattleBulletDestory(byte[] dataBuffer, int socketID)
    {
        var sMsgDestoryBullet_SC = SMsgDestoryBullet_SC.ParseResultPackage(dataBuffer);

        //\
        //TraceUtil.Log("收到服务器下发删除子弹:" + sMsgDestoryBullet_SC.BulletIndex+" Time:"+Time.realtimeSinceStartup);
        BulletManager.Instance.UnRegisteEntity(sMsgDestoryBullet_SC.BulletIndex, sMsgDestoryBullet_SC.uidFighter);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveBattleBreakSkill(byte[] dataBuffer, int socketID)
    {
        var sMsgDestoryBullet_SC = SMsgFightBreakSkill_SC.ParseResultPackage(dataBuffer);
		if(GameManager.Instance.CreateEntityIM)
		{
			RaiseEvent(EventTypeEnum.BreakSkill.ToString(), sMsgDestoryBullet_SC);
		}
		else
		{
			PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("ReceiveBattleBreakSkill",()=>{
 				RaiseEvent(EventTypeEnum.BreakSkill.ToString(), sMsgDestoryBullet_SC);
			});
		}			
        
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveFightChangeDirect(byte[] dataBuffer, int socketID)
    {
        var sMsgDestoryBullet_SC = SMsgFightChangeDirect_SC.ParsePackage(dataBuffer);
		if(GameManager.Instance.CreateEntityIM)
		{
			RaiseEvent(EventTypeEnum.FightChangeDirect.ToString(), sMsgDestoryBullet_SC);
		}
		else
		{
			PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("ReceiveFightChangeDirect",()=>{
				RaiseEvent(EventTypeEnum.FightChangeDirect.ToString(), sMsgDestoryBullet_SC);
			});
		}        
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveBattleBeAdsorb(byte[] dataBuffer, int socketID)
    {
        SMsgAdsorptionContextNum_SC sMsgAdsorptionContextNum_SC = SMsgAdsorptionContextNum_SC.ParseResultPackage(dataBuffer);        
		if(GameManager.Instance.CreateEntityIM)
		{
            for(int i  = 0; i < sMsgAdsorptionContextNum_SC.byContextNum; i++)
            {
                RaiseEvent(EventTypeEnum.BeAdsorb.ToString(), sMsgAdsorptionContextNum_SC.list[i]);
            }
		}
		else
		{
			PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("ReceiveBattleBeAdsorb",()=>{
                for(int i  = 0; i < sMsgAdsorptionContextNum_SC.byContextNum; i++)
                {
                    RaiseEvent(EventTypeEnum.BeAdsorb.ToString(), sMsgAdsorptionContextNum_SC.list[i]);
                }
			});
		}    
        return CommandCallbackType.Continue;
    }
    //收到定身术消息
    CommandCallbackType ReceiveBattleHorde(byte[] dataBuffer, int socketID)
    {
        SMsgHordeContextNum_SC sMsgHordeContextNum_SC = SMsgHordeContextNum_SC.ParseResultPackage(dataBuffer);        
		if(GameManager.Instance.CreateEntityIM)
		{
            for(int i  = 0; i < sMsgHordeContextNum_SC.byContextNum; i++)
            {
                RaiseEvent(EventTypeEnum.EntityHorde.ToString(), sMsgHordeContextNum_SC.list[i]);
            }
		}
		else
		{
			PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("ReceiveBattleHorde",()=>{
                for(int i  = 0; i < sMsgHordeContextNum_SC.byContextNum; i++)
                {
                    RaiseEvent(EventTypeEnum.EntityHorde.ToString(), sMsgHordeContextNum_SC.list[i]);
                }
			});
		}    
        return CommandCallbackType.Continue;
    }
	
	CommandCallbackType ReceiveBattleBeAdsorbEx(byte[] dataBuffer, int socketID)
	{
        SMsgAdsorptionExContextNum_SC sMsgAdsorptionExContextNum_SC = SMsgAdsorptionExContextNum_SC.ParseResultPackage(dataBuffer);
		if(GameManager.Instance.CreateEntityIM)
		{
            for(int i  = 0; i < sMsgAdsorptionExContextNum_SC.byContextNum; i++)
            {
                RaiseEvent(EventTypeEnum.BeAdSorbEx.ToString(), sMsgAdsorptionExContextNum_SC.list[i]);
            }
		}
		else
		{
			PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("ReceiveBattleBeAdsorbEx",()=>{
                for(int i  = 0; i < sMsgAdsorptionExContextNum_SC.byContextNum; i++)
                {
                    RaiseEvent(EventTypeEnum.BeAdSorbEx.ToString(), sMsgAdsorptionExContextNum_SC.list[i]);
                }
			});
		}    
		return CommandCallbackType.Continue;
	}
	
    //收到破防消息
	
	//
	CommandCallbackType ReceiveBattleCommondSingle(byte[] dataBuffer, int socketID)
	{
		var sMsgFightCommondSC = SMsgFightCommand_SC.ParseResultPackage(dataBuffer);
		if(GameManager.Instance.CreateEntityIM)
		{
			RaiseEvent(EventTypeEnum.SingleFigntCommand.ToString(), sMsgFightCommondSC);
		}
		else
		{
			PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("ReceiveBattleCommondSingle",()=>{
				RaiseEvent(EventTypeEnum.SingleFigntCommand.ToString(), sMsgFightCommondSC);
			});
		}    
		return CommandCallbackType.Continue;	
	}
	
	
    CommandCallbackType ReceiveBattleBreakShield(byte[] dataBuffer, int socketID)
    {
        var bossShieldStruct = BossShieldStruct.ParseResultPackage(dataBuffer);
		if(GameManager.Instance.CreateEntityIM)
		{
			RaiseEvent(EventTypeEnum.BreakShield.ToString(), bossShieldStruct);
		}
		else
		{
			PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("ReceiveBattleBreakShield",()=>{
				RaiseEvent(EventTypeEnum.BreakShield.ToString(), bossShieldStruct);
			});
		}
        
        return CommandCallbackType.Continue;
    }
    //收到防护恢复消息
    CommandCallbackType ReceiveBattleReplyShield(byte[] dataBuffer, int socketID)
    {
        var bossShieldStruct = BossShieldStruct.ParseResultPackage(dataBuffer);        
		if(GameManager.Instance.CreateEntityIM)
		{
			RaiseEvent(EventTypeEnum.BreakShield.ToString(), bossShieldStruct);
		}
		else
		{
			PlayerFactory.Instance.RegisterPlayerAfterSceneLoadedFun("ReceiveBattleReplyShield",()=>{
				RaiseEvent(EventTypeEnum.BreakShield.ToString(), bossShieldStruct);
			});
		}
        return CommandCallbackType.Continue;
    }

    #endregion
    #region 发送消息处理

    /// <summary>
    /// 发送吸血消息数据处理
    /// </summary>
    public void SendBattleBloodSucking(SMsgFightBloodSucking_CS sMsgFightBloodSucking_CS)
    {
        this.Request(sMsgFightBloodSucking_CS.GeneratePackage(MasterMsgType.NET_ROOT_FIGHT, FightDefineManager.MSG_FIGHT_BATTLE_BLOODSUCKING));
    }

    public void SendBattleCommand(SMsgBattleCommand sMsgBattleCommand)
    {
        //TraceUtil.Log("发送技能攻击：" + sMsgBattleCommand.nFightCode);
        this.Request(sMsgBattleCommand.GeneratePackage(MasterMsgType.NET_ROOT_FIGHT, FightDefineManager.MSG_FIGHT_BATTLE_COMMAND));
    }

    public void SendClientOptMoveCommand(SMsgActionClientOptMove_CS sMsgActionClientOptMove)
    {
        this.Request(sMsgActionClientOptMove.GeneratePackage(MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_STOPHERE));
    }
    public void SendBreakSkillCommand(SMsgFightBreakSkill_CS sMsgFightBreakSkill_CS)
    {
        this.Request(sMsgFightBreakSkill_CS.GeneratePackage(MasterMsgType.NET_ROOT_FIGHT, FightDefineManager.MSG_FIGHT_BATTLE_BREAK_SKILL));
    }
    public void SendFightChangeDirectCommand(SMsgFightChangeDirect_CS sMsgFightChangeDirect_CS)
    {
        this.Request(sMsgFightChangeDirect_CS.GeneratePackage(MasterMsgType.NET_ROOT_FIGHT, FightDefineManager.MSG_FIGHT_BATTLE_CHANGE_DIRECT));
    }
	
	public void SendFightCommandCS(SMsgFightCommand_CS sMsgFightCommand_CS)
	{
		this.Request(sMsgFightCommand_CS.GeneratePackage());
		
	}
	
    public void SendFightEffectCS(SMsgFightEffect_CS sMsgFightEffect_CS)
	{
        this.Request(sMsgFightEffect_CS.GeneratePackage());
	}
	public void SendDeadBulletFightEffectCS(SMsgDeadBulletFightEffect_CS sMsgDeadBulletFightEffect_CS)
	{
		this.Request(sMsgDeadBulletFightEffect_CS.GeneratePackage());
	}
	
    public void SendFightBeatBackCS(SMsgBeatBackContextNum_CS sMsgBeatBackContextNum_CS)
	{
        this.Request(sMsgBeatBackContextNum_CS.GeneratePackage());	
	}
	
    public void SendFightHitFlyCS(SMsgHitFlyContextNum_CS sMsgHitFlyContextNum_CS)
	{
        this.Request(sMsgHitFlyContextNum_CS.GeneratePackage());	
	}
    public void SendFightTeleportCS(SMsgFightTeleport_CSC sMsgFightTeleport_CSC)
    {
        this.Request(sMsgFightTeleport_CSC.GeneratePackage());
    }
    public void SendFightAdsorption_CS(SMsgAdsorptionContextNum_CS sMsgAdsorptionContextNum_CS)
	{
        this.Request(sMsgAdsorptionContextNum_CS.GeneratePackage());
	}
	
    public void SendFightHorde_CS(SMsgHordeContextNum_CS sMsgHordeContextNum_CS)
	{
        this.Request(sMsgHordeContextNum_CS.GeneratePackage());
	}
	
	public void SendFightSummonBullet_CS(SMsgFightSummonBullet_CS sMsgFightSummonBullet_CS)
	{
		this.Request(sMsgFightSummonBullet_CS.GeneratePackage());	
	}

    public void SendFightClimb_CS(SMsgFightClimbs_CS sMsgFightClimbs_CS)
    {
        this.Request(sMsgFightClimbs_CS.GeneratePackage());
    }
	public void SendFightBattleMissEffect(SMsgFightMissEffect_CS sMsgFightMissEffect_CS)
	{
		this.Request(sMsgFightMissEffect_CS.GeneratePackage());
	}

    #endregion
}
