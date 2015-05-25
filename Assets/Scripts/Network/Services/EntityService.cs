using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NetworkCommon;
using System.Runtime.InteropServices;

public class EntityService : Service
{

    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        Package package;
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        package = PackageHelper.ParseReceiveData(dataBuffer);
        //TraceUtil.Log("EntityService ê?μ??÷???￠:" + masterMsgType + "  ê?μ?×ó???￠￡o" + package.GetSubMsgType());
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_THING:                
                switch (package.GetSubMsgType())
                {
                    case CommonMsgDefineManager.MSG_ACTION_NEW_WORLD:  //í¨?a?í?§??′′?¨μ?í?  
                        this.AddToInvoker(this.ChangeToScene, dataBuffer, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_PROP_CREATEENTITY:  //í¨?a?í?§??′′?¨??é?
                        this.AddToInvoker(this.EntityCreateHandle, dataBuffer, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_PROP_DESTROYENTITY: //í¨?a?í?§??é?3y??é?
                        this.AddToInvoker(this.EntityDestroyHandle, dataBuffer, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_MOVE:   //éú??ò??ˉ
                        this.AddToInvoker(this.EntityMoveHandle, dataBuffer, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_DIE:    //éú???àí?
                        this.AddToInvoker(this.EntityDieHandle,dataBuffer,socketId);                            
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_RELIVE://í??ò?′??
                        this.AddToInvoker(this.ActionRelivePlayer, package.Data, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_WORLD_OBJECT_INIT_BUFF: //3?ê??ˉBuffer
                        this.AddToInvoker(this.BuffInitHandle, dataBuffer, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_WORLD_OBJECT_ADD_BUFF: //???óBuffer
                        this.AddToInvoker(this.BuffCreateHandle, dataBuffer, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_WORLD_OBJECT_REMOVE_BUFF: //é?3yBuffer
                        this.AddToInvoker(this.BuffRemoveHandle, dataBuffer, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_USEMEDICAMENTRESULT:  //S·￠?íò??·ê1ó?ó|′eμ?C
                        this.AddToInvoker(this.UseMddicamentResultHandle,package.Data,socketId);
                        ////TraceUtil.Log("S·￠?íò??·ê1ó?ó|′eμ?C");
                        break;
                    case CommonMsgDefineManager.MSG_PROP_UPDATEPROP:   //êμì?ê?D??üD?
                        //TraceUtil.Log("+++ê?μ?μ￥???üD?");
                        this.AddToInvoker(this.EntityValueUpdateHandle, package.Data, socketId);
                        //PlayerManager.Instance.UpdateHeroValues(
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_PLAYER_CLEAN_SHOW:  //?üD?í??òía1?×ê?′
                        this.AddToInvoker(this.PlayerCleanShowHandle, package.Data, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_COLD_WORK:  //à?è′ê±??í¨?a
                        this.AddToInvoker(this.ColdWorkHandle,  package.Data, socketId);
                        break;
					case CommonMsgDefineManager.MSG_ACTION_INIT_SKILL://技能数据初始化 技能数据下发
						this.AddToInvoker(this.InitHeroSkillHandel, package.Data, socketId);
						break;
					case CommonMsgDefineManager.MSG_ACTION_UPGRADE_SKILL:  //升级技能消息                      
						this.AddToInvoker(this.UpgradeSkillHandel,package.Data,socketId);
						break;
					case CommonMsgDefineManager.MSG_ACTION_EQUIP_SKILL:  //技能装备消息
                        this.AddToInvoker(this.EquipSkillHandle, package.Data, socketId);                       
                        break;
					case CommonMsgDefineManager.MSG_ACTION_OPEN_SKILLSTUDY_DLG:
						this.AddToInvoker(OpenUpgradSkillPanel,package.Data,socketId);
						break;
					case CommonMsgDefineManager.MSG_ACTION_ADVANCED_SKILL:   //进阶技能
						this.AddToInvoker(SkillAdvanceHandle,package.Data,socketId);
						break;
					case CommonMsgDefineManager.MSG_ACTION_STRENGTHEN_SKILL: //强化技能
						this.AddToInvoker(SkillStrengthenHandle,package.Data,socketId); 
						break;
					case CommonMsgDefineManager.MSG_ACTION_UNLOCK_SKILL: //解锁技能
						this.AddToInvoker(SkillUnLockHandle,package.Data,socketId);
						break;

                    case CommonMsgDefineManager.MSG_ACTION_HEART_FPS:
                        // uint dwIndex = BitConverter.ToUInt32(dataBuffer,0);
                        //TraceUtil.Log("ê?μ?D?ì?·μ??:" + dwIndex);
                        //HeartFPSManager.Instance.ReceiveHeartFps(dwIndex);
                        this.AddToInvoker(this.HeartFpsHandle, package.Data, socketId);                        
                        break; 
                     
                    case CommonMsgDefineManager.MSG_ACTION_MONSTER_MOVE:
                        this.AddToInvoker(this.ActionMonsterMoveHandle, package.Data, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_BULLETTEST:                        
                        this.AddToInvoker(this.BulletTestCreateHandle, dataBuffer, socketId);                            
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_STOPHERE:
                        this.AddToInvoker(this.EntityStopMoveHandle, dataBuffer, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_OPEN_LIANHUAUI:
                        this.AddToInvoker(this.OpenLianHuaUIHandle, package.Data, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_LIANHUA:
                        this.AddToInvoker(this.LianHuaHandle, package.Data, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_OPEN_FRUIT://′ò?a±|ê÷????￡???·￠1?êμD??￠
                        //TraceUtil.Log("′ò?a±|ê÷????￡???·￠1?êμD??￠");
                        this.AddToInvoker(this.ReceiveOpenFruitPanelInfo,package.Data,socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_SYS_FRUIT_STATUS://?üD?μ￥??1?êμD??￠
                        //TraceUtil.Log("?üD?μ￥??1?êμD??￠");
                        this.AddToInvoker(this.ReceiveFruitStatusInfo,package.Data,socketId);
                        break;

                    case CommonMsgDefineManager.MSG_ACTION_FRUIT_USEMANNA:
                        this.AddToInvoker(this.ReceiveUseMana, package.Data, socketId);
                        break;

                    case CommonMsgDefineManager.MSG_ACTION_GET_FRUIT:
                        this.AddToInvoker(this.ReceiveTreasureTreeGetReward, package.Data, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_ACCOUNT_XIULIAN:
                        this.AddToInvoker(this.ReceiveAccountXiuLianHandle, package.Data, socketId);
                        break;
                    case CommonMsgDefineManager.MSG_ACTION_BREAK_INFO:
                        break;   
					case CommonMsgDefineManager.MSG_ACTION_YAONV_FIGHTING:
						this.AddToInvoker(this.ReceiveYaoNvFightingHandle, package.Data, socketId);
						break;
						//每日签到主动下发数据
					case CommonMsgDefineManager.MSG_ACTION_DAYSIGNINUI:
						this.AddToInvoker(this.ReceiveDailySignMainDataHandle, package.Data, socketId);
						break;
						//签到请求回应
					case CommonMsgDefineManager.MSG_ACTION_DAYSIGNIN:
						this.AddToInvoker(this.ReceiveDailySignSuccessHandle, package.Data, socketId);
						break;
					case CommonMsgDefineManager.MSG_ACTION_YAONVCONDITION_UPDATE:
						this.AddToInvoker(this.ReceiveYaoNvConditionUpdateHandle,package.Data,socketId);
						break;
						//武学信息主动下发
					case CommonMsgDefineManager.MSG_ACTION_WUXUEUI:
						this.AddToInvoker(this.ReceiveWuXueDataHandle, package.Data, socketId);
						break;
						//请求武学学习升级回应
					case CommonMsgDefineManager.MSG_ACTION_WUXUE_STUDY:
						this.AddToInvoker(this.ReceiveWuXueStudyHandle, package.Data, socketId);
						break;
			case CommonMsgDefineManager.MSG_ACTION_GET_PVP_HISTORY:
				this.AddToInvoker(this.ReceiveWuXueStudyHandle, package.Data, socketId);
				break;
                    default:
                        ////TraceUtil.Log("NET_ROOT_THING:" + package.GetSubMsgType());
                        break;
                }
                break;           
            default:
                ////TraceUtil.Log("2??üê?±eμ??÷???￠:" + package.GetMasterMsgType());
                break;
        }
    }
    #region ?óê?Buff???￠′|àí
    /// <summary>
    /// S·￠?íò??·ê1ó?ó|′eμ?C
    /// </summary>
    /// <param name="dataBuffer"></param>
    CommandCallbackType UseMddicamentResultHandle(byte[] dataBuffer, int socketID)
    {
        SMsgActionUseMedicamentResult_SC sMsgActionUseMedicamentResult_SC = new SMsgActionUseMedicamentResult_SC() { byResult = dataBuffer[0] };
        UIEventManager.Instance.TriggerUIEvent(UIEventType.UseMedicamentResult, sMsgActionUseMedicamentResult_SC);
        TraceUtil.Log("ê?μ?ò??·ê1ó??á1????￠￡o" + sMsgActionUseMedicamentResult_SC.byResult);
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// ?üD????ü
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    CommandCallbackType UpgradeSkillHandel(byte[] dataBuffer, int socketId)
    {
        ushort skillId =BitConverter.ToUInt16(dataBuffer,0);
        byte skillLV = dataBuffer[2];
        var sskillInfo=new SSkillInfo() { wSkillID = skillId, wSkillLV = skillLV };
        PlayerManager.Instance.HeroSMsgSkillInit_SC.UpgradeSkill(sskillInfo);
        //TraceUtil.Log("???ü?üD?ê??t");
        UIEventManager.Instance.TriggerUIEvent(UIEventType.UpgrateSkillInfo, sskillInfo);
        return CommandCallbackType.Continue;


    }
    /// <summary>
    /// 3?ê??ˉ?÷?????ü
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    /// <returns></returns>
    CommandCallbackType InitHeroSkillHandel(byte[] dataBuffer, int socketId)
    {
        SMsgSkillInit_SC sMsgSkillInit_SC = SMsgSkillInit_SC.ParsePackage(dataBuffer);
        PlayerManager.Instance.HeroSMsgSkillInit_SC = sMsgSkillInit_SC;
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// 3?ê??ˉBuffêy?Y
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    CommandCallbackType BuffInitHandle(byte[] dataBuffer, int socketId)
    {
        //3?ê?Buff?á11ì?ó?′′?¨Buff?á11ì??aí?ò???
        SMsgActionWorldObjectAddBuff_SC sMsgActionWorldObjectAddBuff_SC = SMsgActionWorldObjectAddBuff_SC.ParsePackage(dataBuffer);
        //TraceUtil.Log("ê?μ?3?ê??ˉBuffμ????￠￡?");
        //??μ?Buffêy?Y
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// ′′?¨Buff
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    CommandCallbackType BuffCreateHandle(byte[] dataBuffer, int socketId)
    {
        if (GameManager.Instance.CreateEntityIM)
        {
            SMsgActionWorldObjectAddBuff_SC sMsgActionWorldObjectAddBuff_SC = SMsgActionWorldObjectAddBuff_SC.ParsePackage(dataBuffer);
            //TraceUtil.Log("ê?μ?′′?¨Buffμ????￠￡?");
            //??μ?Buffêy?Y
            //if (sMsgActionWorldObjectAddBuff_SC.SMsgActionSCHead.uidEntity != PlayerManager.Instance.FindHeroDataModel().UID)
            //    return CommandCallbackType.Continue;
            BuffController.Instance.LaunchBuff(sMsgActionWorldObjectAddBuff_SC);
            if (sMsgActionWorldObjectAddBuff_SC.SMsgActionSCHead.uidEntity == PlayerManager.Instance.FindHeroDataModel().UID)
            {
                RoleBuffList roleBuffList = GameDataManager.Instance.PeekData(DataType.GameBufferData) as RoleBuffList;
                if (roleBuffList == null) { roleBuffList = new RoleBuffList(); }
                roleBuffList.AddBuffer(sMsgActionWorldObjectAddBuff_SC);
                GameDataManager.Instance.ResetData(DataType.GameBufferData, roleBuffList);
            }
        }
        else
        {
			GameManager.Instance.PlayerFactory.RegisterPlayerAfterSceneLoadedFun("BuffCreateHandle",() =>
                {
                    BuffCreateHandle(dataBuffer, socketId);
                });
        }
        

        //UIEventManager.Instance.TriggerUIEvent(UIEventType.CreatBuffer, sMsgActionWorldObjectAddBuff_SC);
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// é?3yBuff
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    CommandCallbackType BuffRemoveHandle(byte[] dataBuffer, int socketId)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        SMsgActionWorldObjectRemoveBuff_SC sMsgActionWorldObjectRemoveBuff_SC = SMsgActionWorldObjectRemoveBuff_SC.ParsePackage(dataBuffer);
        RoleBuffList roleBuffList = GameDataManager.Instance.PeekData(DataType.GameBufferData) as RoleBuffList;
        if (roleBuffList == null) { roleBuffList = new RoleBuffList(); }
        if (sMsgActionWorldObjectRemoveBuff_SC.SMsgActionSCHead.uidEntity == PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
        {
            roleBuffList.RemoveBuffer((int)sMsgActionWorldObjectRemoveBuff_SC.DwIndex);
        }
        GameDataManager.Instance.ResetData(DataType.GameBufferData, roleBuffList);

        BuffController.Instance.Remove(sMsgActionWorldObjectRemoveBuff_SC);
        //UIEventManager.Instance.TriggerUIEvent(UIEventType.RemoveBuffer, sMsgActionWorldObjectRemoveBuff_SC);
        //TraceUtil.Log("ê?μ?é?3yBuffμ????￠￡?");
        return CommandCallbackType.Continue;
    }

    CommandCallbackType HeartFpsHandle(byte[] dataBuffer, int socketId)
    {       
        uint dwIndex = BitConverter.ToUInt32(dataBuffer,0);
        //TraceUtil.Log("ê?μ?D?ì?·μ??:" + dwIndex);
        HeartFPSManager.Instance.ReceiveHeartFps(dwIndex);
        return CommandCallbackType.Continue;
    }
    #endregion

    #region ?óê????·μ??????￠′|àí
    CommandCallbackType DropHandle(byte[] dataBuffer, int socketId)
    {
        //\×￠2áêμì?

        return CommandCallbackType.Continue;
    }
    #endregion

    #region ?óê????￠′|àí
    CommandCallbackType ReceiveFruitStatusInfo(byte[] dataBuffer, int socketId)
    {
        SMsgActionFruitContext_SC sMsgActionFruitContext_SC = SMsgActionFruitContext_SC.ParsePackage(dataBuffer);
        UI.MainUI.TreasureTreesData.Instance.ResetTreasureTreesDataInfo(sMsgActionFruitContext_SC);
        return CommandCallbackType.Continue;
    }


    CommandCallbackType ReceiveUseMana(byte[] dataBuffer, int socketId)
    {
        SMsgActionUseManna_SC SMsgActionUseManna_SC = SMsgActionUseManna_SC.ParsePackage(dataBuffer);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.TreasureTreesUseMana, SMsgActionUseManna_SC);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveTreasureTreeGetReward(byte[] dataBuffer, int socketId)
    {
        SMsgActionChooseFruit_SC sMsgActionChooseFruit_SC = SMsgActionChooseFruit_SC.ParsePackage(dataBuffer);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.TreasureTreesGetReward, sMsgActionChooseFruit_SC);
        return CommandCallbackType.Continue;
    }


    CommandCallbackType ReceiveOpenFruitPanelInfo(byte[] dataBuffer   , int socketId)
    {
        SMsgActionFruit_SC sMsgActionFruit_SC = SMsgActionFruit_SC.ParsePackage(dataBuffer);
        sMsgActionFruit_SC.SMsgActionFruitContextList.ApplyAllItem(P=>UI.MainUI.TreasureTreesData.Instance.ResetTreasureTreesDataInfo(P));
        TraceUtil.Log("ReceiveDataLeght:" + dataBuffer.Length + "," + sMsgActionFruit_SC.SMsgActionFruitContextList.Count);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType PlayerCleanShowHandle(byte[] dataBuffer, int socketId)
    {
        if (GameManager.Instance.CreateEntityIM)
        {
            SC_MSG_UPDATE_SHOW_CONTEXT SC_MSG_UPDATE_SHOW_CONTEXT = SC_MSG_UPDATE_SHOW_CONTEXT.ParsePackage(dataBuffer);
            if (SC_MSG_UPDATE_SHOW_CONTEXT.byUpdateShowResCount > 0)
            {
                if (SC_MSG_UPDATE_SHOW_CONTEXT.UPDATE_SHOW_ITEMs[0].Index == 0)   //????==0  ±íê????÷
                {
                    PlayerManager.Instance.ChangePlayerWeapon(SC_MSG_UPDATE_SHOW_CONTEXT.uidEntity, (int)SC_MSG_UPDATE_SHOW_CONTEXT.UPDATE_SHOW_ITEMs[0].GoodId);
                }
            }
        }
        else
        {
			GameManager.Instance.PlayerFactory.RegisterPlayerAfterSceneLoadedFun("PlayerCleanShowHandle",() =>
                {
                    PlayerCleanShowHandle(dataBuffer, socketId);
                });
        }
        

        return CommandCallbackType.Continue;
    }

    CommandCallbackType OpenUpgradSkillPanel(byte[] dataBuffer, int socketId)
    {
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UI.MainUI.UIType.Skill);
        return CommandCallbackType.Continue;
    }
	// 进阶技能
	CommandCallbackType SkillAdvanceHandle(byte[] dataBuffer, int socketId)
	{
		SMsgSkillAdvanced_CS sMsgSkillAdvanced_CS = SMsgSkillAdvanced_CS.ParsePackage(dataBuffer);
		//进阶成功
		UIEventManager.Instance.TriggerUIEvent(UIEventType.SkillAdvanceEvent,sMsgSkillAdvanced_CS);
		return CommandCallbackType.Continue;
	}
	// 强化技能
	CommandCallbackType SkillStrengthenHandle(byte[] dataBuffer, int socketId)
	{
		SMsgSkillStrengthen_SC sMsgSkillStrengthen_SC = SMsgSkillStrengthen_SC.ParsePackage(dataBuffer);
		//强化成功
		PlayerManager.Instance.HeroSMsgSkillInit_SC.StrengthenSkill(sMsgSkillStrengthen_SC);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.SkillStrengthenEvent,sMsgSkillStrengthen_SC);
		return CommandCallbackType.Continue;
	}
	//解锁
	CommandCallbackType SkillUnLockHandle(byte[] dataBuffer, int socketId)
	{
		SMsgSkillUnLock_SC sMsgSkillUnLock_SC = SMsgSkillUnLock_SC.ParsePackage(dataBuffer);
		//强化成功
		PlayerManager.Instance.HeroSMsgSkillInit_SC.UnlockSkill(sMsgSkillUnLock_SC);
		return CommandCallbackType.Continue;
	}

    /// <summary>
    /// à?è′???￠′|àí
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    CommandCallbackType ColdWorkHandle(byte[] dataBuffer, int socketId)
    {
        int headlength = Marshal.SizeOf(typeof(SMsgActionColdWorkHead_SC));
        int workBodyLength = Marshal.SizeOf(typeof(SMsgActionColdWork_SC));
        SMsgActionColdWorkHead_SC SMsgActionColdWorkHead_SC = PackageHelper.BytesToStuct<SMsgActionColdWorkHead_SC>(dataBuffer.Take(headlength).ToArray());
        SMsgActionColdWork_SC[] sMsgActionColdWork_SCs = new SMsgActionColdWork_SC[SMsgActionColdWorkHead_SC.bColdNum];
        int offset = headlength;
        for (int i = 0; i < SMsgActionColdWorkHead_SC.bColdNum; i++)
        {            
            sMsgActionColdWork_SCs[i] = PackageHelper.BytesToStuct<SMsgActionColdWork_SC>(dataBuffer.Skip(offset).Take(workBodyLength).ToArray());
            offset += workBodyLength;
        }
        //?à1??????÷′|àí?a??à?è′???￠
        SmsgActionColdWork smsgActionColdWork = new SmsgActionColdWork() {sMsgActionColdWorkHead_SC = SMsgActionColdWorkHead_SC,sMsgActionColdWork_SCs = sMsgActionColdWork_SCs };

        //smsgActionColdWork.sMsgActionColdWork_SCs.ApplyAllItem(p =>
        //    {
        //        Debug.LogWarning("[ê?μ?à?è′ê??t]class = " + p.byClassID + "," + p.dwColdID + "," + p.dwColdTime);
        //    });
        RaiseEvent(EventTypeEnum.ColdWork.ToString(), smsgActionColdWork);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.NewColdWorkFromSever,smsgActionColdWork);
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// êμì??üD?êy?μ′|àí
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    CommandCallbackType EntityValueUpdateHandle(byte[] dataBuffer, int socketId)
    {
        int headlength = 9;//SMsgPropUpdateProp_SC (uidEntity+nUpdateMode);
        var sMsgPropUpdateProp_SC = PackageHelper.BytesToStuct<SMsgPropUpdateProp_SC>(dataBuffer.Take(headlength).ToArray());
        Int64 entityId = sMsgPropUpdateProp_SC.uidEntity;
        short index;

        short[] indexs=null;
        int value;
        int[] values= null;
        bool hasUpdate = false;
        TypeID nEntityClass=TypeID.TYPEID_INVALID;
        bool isHero = false;
        switch (sMsgPropUpdateProp_SC.nUpdateMode)
        {
            case 1:  //μ￥??êy?μê?D??üD?                
                index=BitConverter.ToInt16(dataBuffer,headlength);
                indexs = new short[] { index };
                value=BitConverter.ToInt32(dataBuffer,headlength+2);
                values=new int[]{value};
                EntityController.Instance.UpdateEntityValue(entityId, index, value,out nEntityClass,out isHero);
                hasUpdate = true;

      //          Debug.LogWarning("ê?μ?μ￥???üD?￡o" + index + "   " + value + "   " + nEntityClass + "  " + isHero);
                break;
            case 4:  //?à??êy?μê?D??üD?
                var num = BitConverter.ToInt32(dataBuffer, headlength);
                int offset = 0;
                indexs = new short[num] ;
                values=new int[num];
                for (int i = 0; i < num; i++)
                {
                    offset=i * 6;   // 6 is index + value;
                    index = BitConverter.ToInt16(dataBuffer, headlength + 4 + offset); //first 4 is num length
                    indexs[i] = index;
                    value = BitConverter.ToInt32(dataBuffer, headlength + 4 + 2 + offset);
                     values[i]=value;
                    EntityController.Instance.UpdateEntityValue(entityId, index, value, out nEntityClass, out isHero);
                    TraceUtil.Log("ê?μ??à???üD?￡o" + i + "  " + index + "   " + value + "   " + nEntityClass + "  " + isHero);
                }
                hasUpdate = true;
                break;
        }
        if (hasUpdate&&nEntityClass!=TypeID.TYPEID_INVALID)
        {
            EntityDataUpdateNotify entityDataUpdateNotify = new EntityDataUpdateNotify() { EntityUID = entityId, nEntityClass = nEntityClass, IsHero = isHero, Indexs = indexs, Values = values, UpdateMode = sMsgPropUpdateProp_SC.nUpdateMode };
            //TraceUtil.Log("TypeId￡o" + entityDataUpdateNotify.nEntityClass);
            RaiseEvent(EventTypeEnum.EntityUpdateValues.ToString(), entityDataUpdateNotify);            
        }
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// ê?μ?êμì??ú?ùD??￠′|àí
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    CommandCallbackType EntityDestroyHandle(byte[] dataBuffer, int socketId)
    {
        var package = PackageHelper.ParseReceiveData(dataBuffer);
        SMsgActionDestroyNum_SC sMsgActionDestroyNum_SC = SMsgActionDestroyNum_SC.ParsePackage(package);

        var offset = Marshal.SizeOf(typeof(SMsgActionDestroyNum_SC));
        var length = Marshal.SizeOf(typeof(SMsgPropDestroyEntity_SC));

        SMsgPropDestroyEntity_SC[] sMsgPropDestroyEntity_SC = new SMsgPropDestroyEntity_SC[sMsgActionDestroyNum_SC.wDestroyNum];

        

        for (int i = 0; i < sMsgActionDestroyNum_SC.wDestroyNum; i++)
        {
            //??μ?òaé?3y???tμ?ID
            sMsgPropDestroyEntity_SC[i] = SMsgPropDestroyEntity_SC.ParseResultPackage(package.Data, offset, length);
            TraceUtil.Log("Destroy entity:" + sMsgPropDestroyEntity_SC[i].uidEntity);
            offset += length;
            EntityController.Instance.UnRegisteEntity(sMsgPropDestroyEntity_SC[i].uidEntity);
        }
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// ê?μ?3??°×a?????￠′|àí
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    CommandCallbackType ChangeToScene(byte[] dataBuffer, int socketId)
    {
        SMsgActionNewWorld_SC sMsgActionNewWorld_SC = SMsgActionNewWorld_SC.ParsePackage(dataBuffer);
        PlayerManager.Instance.SetHeroBirthPosAndRotation(sMsgActionNewWorld_SC.ptDestX, sMsgActionNewWorld_SC.ptDestY, sMsgActionNewWorld_SC.byDestOri);       
        CommandCallbackType commandCallbackType = CommandCallbackType.Continue;
        NetServiceManager.Instance.EctypeService.SendEctypeClearance();       
		TraceUtil.Log("NewWorld TeleportFlg:" + (eTeleportType)sMsgActionNewWorld_SC.byTeleportFlg+" "+Time.realtimeSinceStartup);
        switch ((eTeleportType)sMsgActionNewWorld_SC.byTeleportFlg)
        {            
            case eTeleportType.TELEPORTTYPE_FIRST:
            case eTeleportType.TELEPORTTYPE_NORMAL:
            case eTeleportType.TELEPORTTYPE_RECONNECTION:
            case eTeleportType.TELEPORTTYPE_JUMPSERVER:
                var loadSceneData = GameDataManager.Instance.PeekData(DataType.LoadingSceneData) as LoadSceneData;
                   if(loadSceneData!=null){ loadSceneData.LoadSceneInfo = sMsgActionNewWorld_SC;}
                    RaiseEvent(EventTypeEnum.SceneChange.ToString(), sMsgActionNewWorld_SC);                   
                break;
            case eTeleportType.TELEPORTTYPE_NULL:
                break;
            case eTeleportType.TELEPORTTYPE_CURMAP:
                commandCallbackType = CommandCallbackType.Continue;
                break;
        }
        if (GameManager.Instance.EnableHeart && !HeartFPSManager.Instance.StartHeart)
        {
            HeartFPSManager.Instance.StartHeart = true; //μ???1y3ìíê3é￡????ˉD?ì?
        }

        TraceUtil.Log(SystemModel.Rocky, "跳转场景");

        return commandCallbackType;        
    }
    /// <summary>
    /// ê?μ?êμì?′′?¨D??￠′|àí
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    CommandCallbackType EntityCreateHandle(byte[] dataBuffer, int socketId)
    {


        var package = PackageHelper.ParseReceiveData(dataBuffer);

        SMsgActionCreateNum_SC sMsgActionCreateNum_SC = SMsgActionCreateNum_SC.ParsePackage(package);        

        var headLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));

        int offset = 2;     //êμì?′′?¨D-òé  ·t???÷??·￠?÷???￠+×ó???￠+SMsgActionCreateNum_SC(2×??ú±íê?êyá?)+SMsgPropCreateEntity_SC+é?????
        for (int i = 0; i < sMsgActionCreateNum_SC.wCreateNum; i++)
        {
            var sMsgPropCreateEntity_SC_Header = SMsgPropCreateEntity_SC_Header.ParsePackage(package, offset);
            if (sMsgPropCreateEntity_SC_Header.nIsHero == 1)  //?÷í??ò
            {
                switch ((TypeID)sMsgPropCreateEntity_SC_Header.nEntityClass)
                {
                    case TypeID.TYPEID_BOX:
                        var boxSubMsgEntity = BoxSubMsgEntity.ParsePackage(package, offset);
                        EntityController.Instance.AddEntity(boxSubMsgEntity.UID, TypeID.TYPEID_BOX);                        
                        GameManager.Instance.DamageFactory.RegisterBox(boxSubMsgEntity);
                        break;
                    case TypeID.TYPEID_CHUNNEL:
                        ////TraceUtil.Log("=========′??í??");
                        var sMsgPropCreateEntity_SC_Channel = SMsgPropCreateEntity_SC_Channel.ParsePackage(package, offset);
                        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Channel.UID, TypeID.TYPEID_CHUNNEL);
                        GameManager.Instance.PortalFactory.Register(sMsgPropCreateEntity_SC_Channel);
                        break;
                    case TypeID.TYPEID_CONTAINER:   //±3°ü                       
                        break;
                    case TypeID.TYPEID_CORPSE:
                        break;
                    case TypeID.TYPEID_GAMEOBJECT:
                        break;
                    case TypeID.TYPEID_INVALID:
                        break;
                    case TypeID.TYPEID_ITEM:
                        //Debug.LogWarning("ItemCout:" + dataBuffer.Length);
                         var sMsgPropCreateEntity_SC_Container = SMsgPropCreateEntity_SC_Container.ParsePackage(package, offset, sMsgPropCreateEntity_SC_Header);
                         EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Container.SMsg_Header.uidEntity, TypeID.TYPEID_ITEM);
                         UI.MainUI.ContainerInfomanager.Instance.AddGoodsInfo(sMsgPropCreateEntity_SC_Container);
                         
                        break;
                    case TypeID.TYPEID_MAX:
                        break;
                    case TypeID.TYPEID_MONSTER:
                        this.AddToInvoker(this.CreateMonsterEntity, dataBuffer, offset);  
                        break;
                    case TypeID.TYPEID_NPC:
                        var sMsgPropCreateEntity_SC_NPC = SMsgPropCreateEntity_SC_NPC.ParsePackage(package, offset);
                        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_NPC.UID, TypeID.TYPEID_NPC);
                        GameManager.Instance.NPCFactory.Register(sMsgPropCreateEntity_SC_NPC);
                        break;
                    case TypeID.TYPEID_OBJECT:
                        break;
                    case TypeID.TYPEID_PET:
                        break;
                    case TypeID.TYPEID_PLAYER:
				{
                        var sMsgPropCreateEntity_SC_MainPlayer = SMsgPropCreateEntity_SC_MainPlayer.ParsePackage(package, offset);
                        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_MainPlayer.UID, TypeID.TYPEID_PLAYER);						
                        GameManager.Instance.PlayerFactory.Register(sMsgPropCreateEntity_SC_MainPlayer);
                        RaiseEvent(EventTypeEnum.EntityCreate_Player.ToString(), sMsgPropCreateEntity_SC_MainPlayer);
				}
                        break;

                    case TypeID.TYPEID_BULLET:  //\?ù?YDè?ó?Yú??ü???a×óμˉ
                        //TraceUtil.Log("==>í?????·￠×óμˉ′′?¨");
                        var sMsgPropCreateEntity_SC_Bullet = SMsgPropCreateEntity_SC_Bullet.ParsePackage(package, offset);
 
                        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Bullet.GUID, TypeID.TYPEID_BULLET);
                        //×óμˉ?÷′ò·??§2aê?
                        //if (sMsgPropCreateEntity_SC_Bullet.CasterUID == PlayerManager.Instance.FindHeroDataModel().UID)
                        //{
                        //    GameManager.Instance.BulletFactory.TestBullet(sMsgPropCreateEntity_SC_Bullet);
                        //    break;
                        //}

                        //test Log
                        //TraceUtil.Log("==>ê?μ?×óμˉ???￠" +sMsgPropCreateEntity_SC_Bullet.PosX + "," + sMsgPropCreateEntity_SC_Bullet.PosY+ "," + sMsgPropCreateEntity_SC_Bullet.BaseValue.OBJECT_FIELD_ENTRY_ID + "," + sMsgPropCreateEntity_SC_Bullet.CasterUID);       

                        BulletFactory.Instance.Register(sMsgPropCreateEntity_SC_Bullet);      
                      
                        break;
                }
            }
            else    //????í??ò
            {
                switch ((TypeID)sMsgPropCreateEntity_SC_Header.nEntityClass)
                {
                    case TypeID.TYPEID_BOX:
                        var boxSubMsgEntity = BoxSubMsgEntity.ParsePackage(package, offset);
                        EntityController.Instance.AddEntity(boxSubMsgEntity.UID, TypeID.TYPEID_BOX);
                        GameManager.Instance.DamageFactory.RegisterBox(boxSubMsgEntity);
                        break;
                    case TypeID.TYPEID_CHUNNEL:
                        ////TraceUtil.Log("=========′??í??");
                        var sMsgPropCreateEntity_SC_Channel = SMsgPropCreateEntity_SC_Channel.ParsePackage(package, offset);
                        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Channel.UID, TypeID.TYPEID_CHUNNEL);
                        GameManager.Instance.PortalFactory.Register(sMsgPropCreateEntity_SC_Channel);
                        break;
                    case TypeID.TYPEID_CONTAINER:  //±3°ü                       
                        break;
                    case TypeID.TYPEID_CORPSE:
                        break;
                    case TypeID.TYPEID_GAMEOBJECT:
                        break;
                    case TypeID.TYPEID_INVALID:
                        break;
                    case TypeID.TYPEID_ITEM:
                        TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"′′?¨·??÷?????·");
                         var sMsgPropCreateEntity_SC_Container = SMsgPropCreateEntity_SC_Container.ParsePackage(package, offset, sMsgPropCreateEntity_SC_Header);
                         //EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Container.SMsg_Header.uidEntity, TypeID.TYPEID_ITEM);
                        //TraceUtil.Log("ê?μ?±3°üêy?Y" + sMsgPropCreateEntity_SC_Container.ItemTemplateID);
                        break;
                    case TypeID.TYPEID_MAX:
                        break;
                    case TypeID.TYPEID_MONSTER:
						MonsterManager.Instance.Empty();
                        var sMsgPropCreateEntity_SC_Monster = SMsgPropCreateEntity_SC_Monster.ParsePackage(package, offset);
                        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Monster.UID, TypeID.TYPEID_MONSTER);
						EntityModel monsterEntityModel = new EntityModel();
						monsterEntityModel.EntityDataStruct = sMsgPropCreateEntity_SC_Monster;
						EntityController.Instance.RegisteEntity(sMsgPropCreateEntity_SC_Monster.SMsg_Header.uidEntity, monsterEntityModel);
                        MonsterFactory.Instance.Register(sMsgPropCreateEntity_SC_Monster);
                        break;
                    case TypeID.TYPEID_NPC:
                        var sMsgPropCreateEntity_SC_NPC = SMsgPropCreateEntity_SC_NPC.ParsePackage(package, offset);
                        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_NPC.UID, TypeID.TYPEID_NPC);
                        GameManager.Instance.NPCFactory.Register(sMsgPropCreateEntity_SC_NPC);
                        break;
                    case TypeID.TYPEID_OBJECT:
                        break;
                    case TypeID.TYPEID_PET:
                        break;
                    case TypeID.TYPEID_PLAYER:
                        var sMsgPropCreateEntity_SC_OtherPlayer = SMsgPropCreateEntity_SC_OtherPlayer.ParsePackage(package, offset);
                        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_OtherPlayer.UID, TypeID.TYPEID_PLAYER);
                        ////TraceUtil.Log("ê?μ?′′?¨????í??ò???￠￡o" + sMsgPropCreateEntity_SC_OtherPlayer.SMsg_Header.uidEntity);						
                        GameManager.Instance.PlayerFactory.Register(sMsgPropCreateEntity_SC_OtherPlayer);
                        RaiseEvent(EventTypeEnum.EntityCreate.ToString(), sMsgPropCreateEntity_SC_Header);
                        break;

                    case TypeID.TYPEID_TRAP:  //?Yú?
                        //′?′|ê?áùê±ó????μ?????￠D-òé￡?μè·t???????￠D-òé
                        var sMsgPropCreateEntity_SC_Trap = SMsgPropCreateEntity_SC_Trap.ParsePackage(package, offset);
                        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Trap.UID, TypeID.TYPEID_TRAP);
                        ////TraceUtil.Log("ê?μ??Yú????￠" + sMsgPropCreateEntity_SC_Trap.SMsg_Header.uidEntity);
                        GameManager.Instance.TrapFactory.Register(sMsgPropCreateEntity_SC_Trap);
                        break;
                    case TypeID.TYPEID_BULLET:
                        //TraceUtil.Log("==>í?????·￠×óμˉ′′?¨");
                        var sMsgPropCreateEntity_SC_Bullet = SMsgPropCreateEntity_SC_Bullet.ParsePackage(package, offset);

                        //×óμˉ?÷′ò·??§2aê?
                        //if (sMsgPropCreateEntity_SC_Bullet.CasterUID == PlayerManager.Instance.FindHeroDataModel().UID)
                        //{
                        //    GameManager.Instance.BulletFactory.TestBullet(sMsgPropCreateEntity_SC_Bullet);
                        //    break;
                        //}
                        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Bullet.GUID, TypeID.TYPEID_BULLET);                        
                        
                        //\test Log
                        //TraceUtil.Log("==>ê?μ?×óμˉ???￠" +sMsgPropCreateEntity_SC_Bullet.PosX + "," + sMsgPropCreateEntity_SC_Bullet.PosY
                        //    + "," + sMsgPropCreateEntity_SC_Bullet.DirX + ","+sMsgPropCreateEntity_SC_Bullet.DirY+"," + sMsgPropCreateEntity_SC_Bullet.BaseValue.OBJECT_FIELD_ENTRY_ID);
                        if(BulletFactory.Instance!=null)
						{
							BulletFactory.Instance.Register(sMsgPropCreateEntity_SC_Bullet);                        
						}

                        break;
                }
            }
            offset += sMsgPropCreateEntity_SC_Header.wContextLen + headLength;
        }
        return CommandCallbackType.Continue;
    }
    CommandCallbackType BulletTestCreateHandle(byte[] dataBuffer, int socketId)
    {
        var package = PackageHelper.ParseReceiveData(dataBuffer);

        SMsgActionCreateNum_SC sMsgActionCreateNum_SC = SMsgActionCreateNum_SC.ParsePackage(package);

        var headLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
        
        int offset = 2;     //êμì?′′?¨D-òé  ·t???÷??·￠?÷???￠+×ó???￠+SMsgActionCreateNum_SC(2×??ú±íê?êyá?)+SMsgPropCreateEntity_SC+é?????
        for (int i = 0; i < sMsgActionCreateNum_SC.wCreateNum; i++)
        {
            var sMsgPropCreateEntity_SC_Header = SMsgPropCreateEntity_SC_Header.ParsePackage(package, offset);            
            if (sMsgPropCreateEntity_SC_Header.nIsHero == 1)  //?÷í??ò
            {
                switch ((TypeID)sMsgPropCreateEntity_SC_Header.nEntityClass)
                {                    
                    case TypeID.TYPEID_BULLET:  //\?ù?YDè?ó?Yú??ü???a×óμˉ                        
                        var sMsgPropCreateEntity_SC_Bullet = SMsgPropCreateEntity_SC_Bullet.ParsePackage(package, offset);

                        //EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Bullet.GUID, TypeID.TYPEID_BULLET);
                        //×óμˉ?÷′ò·??§2aê?
                        
                        BulletFactory.Instance.TestBullet(sMsgPropCreateEntity_SC_Bullet);
                        break;
                        

                        //test Log
                        //TraceUtil.Log("==>ê?μ?×óμˉ???￠" +sMsgPropCreateEntity_SC_Bullet.PosX + "," + sMsgPropCreateEntity_SC_Bullet.PosY+ "," + sMsgPropCreateEntity_SC_Bullet.BaseValue.OBJECT_FIELD_ENTRY_ID + "," + sMsgPropCreateEntity_SC_Bullet.CasterUID);       

                        //GameManager.Instance.BulletFactory.Register(sMsgPropCreateEntity_SC_Bullet);

                }
            }
            else    //????í??ò
            {
                switch ((TypeID)sMsgPropCreateEntity_SC_Header.nEntityClass)
                {                    
                    case TypeID.TYPEID_BULLET:                        
                        var sMsgPropCreateEntity_SC_Bullet = SMsgPropCreateEntity_SC_Bullet.ParsePackage(package, offset);

                        //×óμˉ?÷′ò·??§2aê?

                        if(BulletFactory.Instance != null)
                        {
                            BulletFactory.Instance.TestBullet(sMsgPropCreateEntity_SC_Bullet);
                        }
                     
                        //EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Bullet.GUID, TypeID.TYPEID_BULLET);

                        //\test Log
                        ////TraceUtil.Log("==>ê?μ?×óμˉ???￠" +sMsgPropCreateEntity_SC_Bullet.PosX + "," + sMsgPropCreateEntity_SC_Bullet.PosY
                        //    + "," + sMsgPropCreateEntity_SC_Bullet.DirX + ","+sMsgPropCreateEntity_SC_Bullet.DirY+"," + sMsgPropCreateEntity_SC_Bullet.BaseValue.OBJECT_FIELD_ENTRY_ID);
                        //GameManager.Instance.BulletFactory.Register(sMsgPropCreateEntity_SC_Bullet);
                        break;
                }
            }
            offset += sMsgPropCreateEntity_SC_Header.wContextLen + headLength;
        }
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// ê?μ?éú??ò??ˉ???￠
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    CommandCallbackType EntityMoveHandle(byte[] dataBuffer, int socketId)
    {
        //TraceUtil.Log("éú??ò??ˉ???￠=>");
        var sMsgActionMove_SC = SMsgActionMove_SCS.ParsePackage(dataBuffer);        
        RaiseEvent(EventTypeEnum.EntityMove.ToString(), sMsgActionMove_SC);
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// ê?μ?1???ò??ˉ???￠(?·??μ?×é) D?2aê?D-òé
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketId"></param>
    /// <returns></returns>
    CommandCallbackType ActionMonsterMoveHandle(byte[] dataBuffer, int socketId)
    {
        var sMsgActionMonsterMove_SC = SMsgActionMonsterMove_SC.ParsePackage(dataBuffer);
        RaiseEvent(EventTypeEnum.MonsterMove.ToString(), sMsgActionMonsterMove_SC);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType CreateMonsterEntity(byte[] dataBuffer, int offset)
    {
        var sMsgPropCreateEntity_SC_Monster = SMsgPropCreateEntity_SC_Monster.ParsePackage(dataBuffer, offset);
        //TraceUtil.Log("==>ê?μ?′′?¨1??????￠" + sMsgPropCreateEntity_SC_Monster.SMsg_Header.uidEntity);

        EntityController.Instance.AddEntity(sMsgPropCreateEntity_SC_Monster.UID, TypeID.TYPEID_MONSTER);
		EntityModel entityModel = new EntityModel();
		entityModel.EntityDataStruct = sMsgPropCreateEntity_SC_Monster;
		EntityController.Instance.RegisteEntity(sMsgPropCreateEntity_SC_Monster.SMsg_Header.uidEntity, entityModel);
        MonsterFactory.Instance.Register(sMsgPropCreateEntity_SC_Monster);

        return CommandCallbackType.Continue;
    }

    // éú??í￡?1ò??ˉ???￠(?·??μ?×é) 
    CommandCallbackType EntityStopMoveHandle(byte[] dataBuffer, int socketId)
    {
        //TraceUtil.Log("===>ê?μ? éú??í￡?1ò??ˉ???￠");
        var sMsgActionStopHere = SMsgActionStopHere_SC.ParsePackage(dataBuffer);
        RaiseEvent(EventTypeEnum.EntityStopHere.ToString(), sMsgActionStopHere);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType EquipSkillHandle(byte[] dataBuffer, int socketId)
    {
        var skillEquipEntity = SkillEquipEntity.ParsePackage(dataBuffer);
        foreach(var child in skillEquipEntity.Skills){
        TraceUtil.Log( SystemModel.Rocky,child.Key+":"+child.Value);
        }

        PlayerManager.Instance.HeroSMsgSkillInit_SC.UpdateAssembleSkill(skillEquipEntity);
        RaiseEvent(EventTypeEnum.EquipSkill.ToString(),null);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType EntityDieHandle(byte[] dataBuffer, int socketId)
    {
        var sMsgActionDie_SC = SMsgActionDie_SC.ParsePackage(dataBuffer);
        //TraceUtil.Log("éú???àí?:" + sMsgActionDie_SC.uidEntity);
        RaiseEvent(EventTypeEnum.EntityDie.ToString(), sMsgActionDie_SC);
        return CommandCallbackType.Continue;     
    }
    /// <summary>
    /// ?′????é?
    /// </summary>
    /// <returns></returns>
    CommandCallbackType ActionRelivePlayer(byte[] dataBuffer, int socketID)
    {
        //SMsgActionSCHead sMsgActionSCHead = new SMsgActionSCHead() { uidEntity = BitConverter.ToInt64(dataBuffer, 0) };
        SMsgActionRelivePlayer_SC sMsgActionRelivePlayer_SC = new SMsgActionRelivePlayer_SC() 
        {
            playerUID = BitConverter.ToInt32(dataBuffer, 0) ,
            actorTarget = BitConverter.ToInt32(dataBuffer, 8),
        };
        RaiseEvent(EventTypeEnum.EntityRelive.ToString(), sMsgActionRelivePlayer_SC);
        //TraceUtil.Log("ê?μ??′????é????￠,UID:" + sMsgActionRelivePlayer_SC.playerUID + ",Target:" + sMsgActionRelivePlayer_SC.actorTarget + ",MyUID:" + PlayerManager.Instance.FindHeroDataModel().ActorID);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType OpenLianHuaUIHandle(byte[] dataBuffer, int socketID)
    {
        SMsgActionYaoNv_SC sMsgActionYaoNv_SC = SMsgActionYaoNv_SC.ParsePackage(dataBuffer);
        //sMsgActionYaoNv_SC.YaoNvContext.ApplyAllItem(p =>
        //    {
        //        TraceUtil.Log("==>[YaoNv]ID = " + p.byYaoNvID + " ,Level = " + p.byLevel);
        //    });
		SirenManager.Instance.AddYaoNvCondition(sMsgActionYaoNv_SC.YaoNvCondtionInfo);
        SirenManager.Instance.AddYaoNvContext(sMsgActionYaoNv_SC.YaoNvContext);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType LianHuaHandle(byte[] dataBuffer, int socketID)
    {
        SMsgActionLianHua_SC sMsgActionLianHua_SC = SMsgActionLianHua_SC.ParsePackage(dataBuffer);
        //string result = sMsgActionLianHua_SC.bySucess == 0 ? "ê§°ü" : "3é1|";
        //TraceUtil.Log("á??ˉ:" + result);
        if (sMsgActionLianHua_SC.bySucess == 1)
        {
            SirenManager.Instance.UpdateYaoNvContext(sMsgActionLianHua_SC);//?üD?′￠′?±í            
        }
        RaiseEvent(EventTypeEnum.LianHuaResult.ToString(), sMsgActionLianHua_SC);//í¨?a?üD?????
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveAccountXiuLianHandle(byte[] dataBuffer, int socketID)
    {
        SMsgActionXiuLianInfo_SC sMsgActionXiuLianInfo_SC = SMsgActionXiuLianInfo_SC.ParsePackage(dataBuffer);
		PlayerRoomManager.Instance.UpdateXiuLianInfo(sMsgActionXiuLianInfo_SC);
		if((PlayerRoomPanel.XiuLianType)sMsgActionXiuLianInfo_SC.byXiuLianType == PlayerRoomPanel.XiuLianType.ROOMDES_XIULIAN_TYPE)
		{
			//·??÷í?3?·????ú?ù
			GameManager.Instance.IsPlayerRoomerLeave = true;
		}
        RaiseEvent(EventTypeEnum.XiuLianAccount.ToString(), sMsgActionXiuLianInfo_SC);
        return CommandCallbackType.Continue;
    }
	CommandCallbackType ReceiveYaoNvFightingHandle(byte[] dataBuffer, int socketID)
	{
		SMsgActionYaoNvJoin_SC sMsgActionYaoNvJoin_SC = new SMsgActionYaoNvJoin_SC(); //SMsgActionYaoNvJoin_SC.ParsePackage(dataBuffer);
		sMsgActionYaoNvJoin_SC.byYaoNvID = dataBuffer[0];		
		SirenManager.Instance.UpdateYaoNvJoin(sMsgActionYaoNvJoin_SC.byYaoNvID);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.YaoNvJoinSuccess,null);
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveYaoNvConditionUpdateHandle(byte[] dataBuffer, int socketID)
	{
		SYaoNvCondtionInfo sYaoNvCondtionInfo = SYaoNvCondtionInfo.ParsePackage(dataBuffer);
		SirenManager.Instance.UpdateYaoNvCondition(sYaoNvCondtionInfo);
		return CommandCallbackType.Continue;
	}
	//每日签到主动下发数据
	CommandCallbackType ReceiveDailySignMainDataHandle(byte[] dataBuffer, int socketID)
	{
		DailySignModel.Instance.dailySignData = SMsgActionDaySignUI_SC.ParsePackage(dataBuffer);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.DailySignAllUpdate,null);
		DailySignModel.Instance.PopDailySignPanel(true);
		return CommandCallbackType.Continue;
	}
	//签到请求回应
	CommandCallbackType ReceiveDailySignSuccessHandle(byte[] dataBuffer, int socketID)
	{
		SMsgActionDaySign_SC sMsgActionDaySign_SC = SMsgActionDaySign_SC.ParsePackage(dataBuffer);
		DailySignModel.Instance.SetSignResponse (sMsgActionDaySign_SC);

		return CommandCallbackType.Continue;
	}


	//武学信息请求结果下发
	CommandCallbackType ReceiveWuXueDataHandle(byte[] dataBuffer, int socketID)
	{
		SMsgActionWuXue_SC sMsgActionWuXue_SC = SMsgActionWuXue_SC.ParsePackage(dataBuffer);
		//UIEventManager.Instance.TriggerUIEvent(UIEventType.DailySignAllUpdate,null);
		PlayerMartialDataManager.Instance.ReceivePlayerMartialData(sMsgActionWuXue_SC);
		return CommandCallbackType.Continue;
	}

	//武学学习升级请求结果下发
	CommandCallbackType ReceiveWuXueStudyHandle(byte[] dataBuffer, int socketID)
	{
		SMsgAcitonStudyWuXue_SC sMsgAcitonStudyWuXue_SC = SMsgAcitonStudyWuXue_SC.ParsePackage(dataBuffer);
		//UIEventManager.Instance.TriggerUIEvent(UIEventType.DailySignAllUpdate,null);
		PlayerMartialDataManager.Instance.UpdatePlayerMartialData(sMsgAcitonStudyWuXue_SC);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.MartialUpgrade, sMsgAcitonStudyWuXue_SC);
		return CommandCallbackType.Continue;
	}
	//武学学习升级请求结果下发
	CommandCallbackType ReceivePVPHistoryHandle(byte[] dataBuffer, int socketID)
	{
		SMsgActionPvpHistory_SC sMsgActionPvpHistory_SC = SMsgActionPvpHistory_SC.ParsePackage(dataBuffer);
		//UIEventManager.Instance.TriggerUIEvent(UIEventType.MartialUpgrade, sMsgAcitonStudyWuXue_SC);
		return CommandCallbackType.Continue;
	}
    #endregion


    #region ·￠?í???￠′|àí

    /// <summary>
    /// ·￠?í?aè?1?êμ
    /// </summary>
    /// 1?êμ????
    public void SendGetFruitMsgToServer(byte fruitPosition)
    {
        SMsgActionChooseFruit_CS sMsgActionChooseFruit_CS = new SMsgActionChooseFruit_CS() {byFruitPosition = fruitPosition };
        this.Request(sMsgActionChooseFruit_CS.GeneratePackage(MasterMsgType.NET_ROOT_THING,CommonMsgDefineManager.MSG_ACTION_GET_FRUIT));
    }
    /// <summary>
    /// ò??ü?aè??ùóD1?êμ
    /// </summary>
    public void SendGetAllFruitMsgToServer()
    {
        SMsgActionChooseFruit_CS sMsgActionChooseFruit_CS = new SMsgActionChooseFruit_CS();
        this.Request(sMsgActionChooseFruit_CS.GeneratePackage(MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_GET_ALL_FRUIT));
    }
    /// <summary>
    /// ???3??1?êμ????
    /// </summary>
    /// <param name="fruitPosition">1?êμ????</param>
    public void SendWateringFruitMsgToServer(byte fruitPosition)
    {
        SMsgActionChooseFruit_CS sMsgActionChooseFruit_CS = new SMsgActionChooseFruit_CS() { byFruitPosition = fruitPosition };
        this.Request(sMsgActionChooseFruit_CS.GeneratePackage(MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_WATERING_FRUIT));
    }
    /// <summary>
    /// ???ùóD1?êμ????
    /// </summary>
    public void SendWateringAllFruitMsgToServer()
    {
        SMsgActionChooseFruit_CS sMsgActionChooseFruit_CS = new SMsgActionChooseFruit_CS();
        this.Request(sMsgActionChooseFruit_CS.GeneratePackage(MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_WATERING_ALLFRUIT));
    }
    /// <summary>
    /// ·￠?íê1ó??ê??μ?·t???÷
    /// </summary>
    public void SendUserManaMsgToServer(byte fruitPosition)
    {
        SMsgActionChooseFruit_CS sMsgActionChooseFruit_CS = new SMsgActionChooseFruit_CS() { byFruitPosition = fruitPosition };
        this.Request(sMsgActionChooseFruit_CS.GeneratePackage(MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_FRUIT_USEMANNA));
    }
    /// <summary>
    /// ·￠?í?a???3??1?êμ????μ?·t???÷
    /// </summary>
    /// <param name="fruitPosition"></param>
    public void SendUnlockFruitMsgToServer(byte fruitPosition)
    {
        SMsgActionChooseFruit_CS sMsgActionChooseFruit_CS = new SMsgActionChooseFruit_CS() { byFruitPosition = fruitPosition };
        this.Request(sMsgActionChooseFruit_CS.GeneratePackage(MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_UNLOCK_FRUIT)); 
    }

    /// <summary>
    /// ??é?DTá??-??
    /// </summary>
    public void SendMsgActionMeridianParctice(int konfuID, int meridianID, int parcticeNum)
    {
        SMsgActionMeridianParctice_CS sMsgActionMeridianParctice_CS = new SMsgActionMeridianParctice_CS()
        {
            lKongfuID = konfuID,
            lMeridiansID = meridianID,
            lParcticeNum = parcticeNum,
        };
        this.Request(sMsgActionMeridianParctice_CS.GeneratePackage());
    }

    /// <summary>
    /// ·￠?íè?′??í?????￠
    /// </summary>
    /// <param name="portalUid"></param>
    public void SendEnterPortal(long portalUid)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_CLICKCHUNNEL);
        pkg.Data = BitConverter.GetBytes(portalUid);
        this.Request(pkg);
    }
    /// <summary>
    /// ·￠?íó?μ?NPC???￠
    /// </summary>
    /// <param name="npcUid"></param>
    public void SendMeetNPC(long npcUid)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_MEETNPC);
        pkg.Data = BitConverter.GetBytes(npcUid)
            .Concat(new byte[1]{0}).ToArray();
        this.Request(pkg);
    }
    /// <summary>
    /// ·￠?í???ü?′μ????ó
    /// </summary>
    public void SendSkillWash()
    {
        /*Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_WASH_SKILL);
               
        this.Request(pkg);*/
    }
    /// <summary>
    /// ·￠?í???ü×°±?/D??????ó
    /// </summary>
    /// <param name="skillEquipEntity"></param>
    public void SendSkillEquip(SkillEquipEntity skillEquipEntity)
    {
        Package pkg = skillEquipEntity.GeneratePackage();

        this.Request(pkg);
    }
    /// <summary>
    /// ·￠?í???ü?§?°/éy?????ó
    /// </summary>
    /// <param name="skillID"></param>
	public void SendSkillUpgrade(ushort skillID)
    {

		SMsgSkillUpgrade_CS sMsgSkillUpgrade_CS = new SMsgSkillUpgrade_CS()
		{
			wSkillId = skillID,
		};
		Package pak = sMsgSkillUpgrade_CS.GeneratePackage();
		this.Request(pak);
	}
		//进阶
	public void SendSkillAdvance(ushort skillID)
	{
		SMsgSkillAdvanced_CS sMsgSkillAdvanced_CS = new SMsgSkillAdvanced_CS()
		{
			wSkillId = skillID,
		};
		Package pak = sMsgSkillAdvanced_CS.GeneratePackage();
		this.Request(pak);
	}
	//强化
	public void SendSkillStrengthen(ushort skillID)
	{
		SMsgSkillStrengthen_CS sMsgSkillStrengthen_CS = new SMsgSkillStrengthen_CS()
		{
			wSkillId = skillID,
		};
		Package pak = sMsgSkillStrengthen_CS.GeneratePackage();
		this.Request(pak);
	}
	/// <summary>
    /// ·￠?í?′?????ó
    /// </summary>
    public void SendActionRelivePlayer(int actorPlayer,int actorTarget,byte ReliveType)
    {
        SMsgActionRelivePlayer_CS sMsgActionRelivePlayer_CS = new SMsgActionRelivePlayer_CS()
        {
            actorPlayer = actorPlayer,
            actorTarget = actorTarget,
            byReliveType = ReliveType,
        };
		Package pak = sMsgActionRelivePlayer_CS.GeneratePackage();
        this.Request(pak);
    }
    /// <summary>
    /// ·￠?íê°è????·???ó
    /// </summary>
    public void SendActionTouchBox(Int64 UIDPlayer, Int64 UIDBox)
    {
        SMsgActionTouchBox_CS sMsgActionTouchBox_CS = new SMsgActionTouchBox_CS()
        {
            uidPlayer = UIDPlayer,
            uidBox = UIDBox
        };
        Package pkg = sMsgActionTouchBox_CS.GeneratePackage();
        this.Request(pkg);
    }
	public SERVICE_CODE SendActionHeartFPS(uint dwIndex,ushort delay)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_HEART_FPS);

        pkg.Data = BitConverter.GetBytes(dwIndex).Concat(BitConverter.GetBytes(delay)).ToArray();

        //TraceUtil.Log("·￠?íD?ì?·t??:"+dwIndex);
        return this.Request(pkg);
    }
    public void SendActionHeartFPS(uint dwIndex)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_HEART_FPS);

        pkg.Data = BitConverter.GetBytes(dwIndex);

        //TraceUtil.Log("·￠?íD?ì?·t??:"+dwIndex);
        this.Request(pkg);
    }
    /// <summary>
    /// é?·￠á??ˉ???ó
    /// </summary>
    /// <param name="sirenID">?y??id</param>
    /// <param name="level">?y????ò?μè??</param>
    public void SendLianHua(int sirenID, int nextLevel)
    {
        SMsgActionLianHua_CS lianhuaPackage = new SMsgActionLianHua_CS()
        {
            byYaoNvID = (byte)sirenID,            

        };
        Package pkg = lianhuaPackage.GeneratePackage();
        this.Request(pkg);
    }

	public enum YaoNvOpType : byte
	{
		unlockNormal = 0,
		unlockByMoney,
		upgrade,
	}
	public void SendLianHua(int sirenID,YaoNvOpType type,int need)
	{
		SMsgActionLianHua_CS lianhuaPackage = new SMsgActionLianHua_CS()
		{
			byYaoNvID = (byte)sirenID,            
			YaoNvOpType = (byte)type,
			dwXiuWeiNum = need,
		};
		Package pkg = lianhuaPackage.GeneratePackage();
		this.Request(pkg);
	}
    /// <summary>
    /// é?·￠??è?DTá?D??￠???ó
    /// </summary>
    public void SendAccountXiuLian()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_ACCOUNT_XIULIAN);
        this.Request(pkg);
    }
    /// <summary>
    /// é?·￠í???DTá????ó
    /// </summary>
    public void SendBreakInfo()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_BREAK_INFO);
        this.Request(pkg);
    }

    /// <summary>
    /// é?·￠??è??y???úμ¤
    /// </summary>
    public void SendGetYaoNvNeiDan(int yaoNvID)
    {
        SMsgGetYaoNvNeiDan_CS sMsgGetYaoNvNeiDan_CS = new SMsgGetYaoNvNeiDan_CS();
        sMsgGetYaoNvNeiDan_CS.YaoNvID = (byte)yaoNvID;
        this.Request(sMsgGetYaoNvNeiDan_CS.GeneratePackage());
    }
	/// <summary>
	/// ????
	/// </summary>
	/// <param name="yaoNvID">??ID(int)</param>
	public void SendYaoNvJoin(int yaoNvID)
	{
		SMsgActionYaoNvJoin_CS sMsgActionYaoNvJoin_CS = new SMsgActionYaoNvJoin_CS();
		sMsgActionYaoNvJoin_CS.byYaoNvID = (byte)yaoNvID;
		this.Request(sMsgActionYaoNvJoin_CS.GeneratePackage());
	}

	//发送签到请求
	public void SendDailySignRequest(int SignType,int singID)
	{
		SMsgActionDaySign_CS sMsgActionDaySign_CS = new SMsgActionDaySign_CS ();
		sMsgActionDaySign_CS.SignType = (byte)SignType;
		sMsgActionDaySign_CS.SignID = (byte)singID;
		this.Request(sMsgActionDaySign_CS.GeneratePackage());
	}

	//发送领取请求
	public void SendGetRewardRequest()
	{
		SMsgInteract_GetReward_CS sMsgInteract_GetReward_CS = new SMsgInteract_GetReward_CS ();
		sMsgInteract_GetReward_CS.dwRewardID = (byte)DailySignModel.Instance.curSelectActivityID;
		this.Request(sMsgInteract_GetReward_CS.GeneratePackage());
	}


	//发送武学学习升级请求(参数只有武学ID)
	public void SendWuXueStudyRequest(int WuXueId)
	{
		SMsgActionStudyWuXue_CS sMsgActionStudyWuXue_CS = new SMsgActionStudyWuXue_CS();
		sMsgActionStudyWuXue_CS.dwWuXueID = WuXueId;
		this.Request(sMsgActionStudyWuXue_CS.GeneratePackage());
	}
	//发送武学学习升级请求
	public void SendGetPvpHistoryRequest()
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_GET_PVP_HISTORY);
		this.Request(pkg);
	}

    #endregion

}
