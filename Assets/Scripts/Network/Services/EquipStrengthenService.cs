using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EquipStrengthenService : Service
{
    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        //TraceUtil.Log("收到装备操作主消息" + masterMsgType + "收到子消息" + package.GetSubMsgType());
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_GOODSOPERATE:
                switch (package.GetSubMsgType())
                {
                    case GoodsOperateDefineManager.MSG_GOODSOPERATE_OPEN_MAKEFACE:
                        AddToInvoker(ReceiveGoodsOperate_OpenMakeFaceCommand, package.Data, socketId);
                        break;
                    case GoodsOperateDefineManager.MSG_GOODSOPERATE_SMELT:
                        AddToInvoker(ReceiveGoodsOperate_SmeltCommand, package.Data, socketId);
                        break;
                    case GoodsOperateDefineManager.MSG_GOODSOPERATE_OPEN_TREASURE_UI:
                        AddToInvoker(ReceiveOpenTreasureUI,package.Data,socketId);
                        break;
                    case GoodsOperateDefineManager.MSG_GOODSOPERATE_MAKE:
                        AddToInvoker(ReceiveEquipMakeMsg,package.Data,socketId);
                        break;
                    case GoodsOperateDefineManager.MSG_GOODSOPERATE_CLICK_TREASURE:
                        AddToInvoker(ReceiveClickTreasure,package.Data,socketId);
                        break;
                    case GoodsOperateDefineManager.MSG_OPNE_ACTIVE_CHEST_UI:
                        AddToInvoker(ReceiveOpenActiveChestUI, package.Data, socketId);
                        break;
                    case GoodsOperateDefineManager.MSG_ACTIVE_VALUE_OPEN_CHEST:
                        AddToInvoker(ReceiveOpenChestValue, package.Data, socketId);
						break;
					case GoodsOperateDefineManager.MSG_EQUIP_LEVEL_UP:
						AddToInvoker(ReceiveEquipmentUpLevelMsg, package.Data, socketId);
                        break;
			        case GoodsOperateDefineManager.MSG_GOODSOPERATE_BESET:
				        AddToInvoker(ReceiveGoodsOperateBesetResults,package.Data,socketId);
				         break;
			        case GoodsOperateDefineManager.MSG_EQUIP_STORE_REMOVE:
				        AddToInvoker(ReceiveGoodsOperateRemoveResults,package.Data,socketId);
                        break;
                    case GoodsOperateDefineManager.MSG_STORE_SWALLOW:
                        AddToInvoker(ReceiveGoodsOperateSwallowResults,package.Data,socketId);
                        break;
                    case GoodsOperateDefineManager.MSG_LUCKDRAW:
                        AddToInvoker(ReceiveSMsgLuckDrawResult_SC, package.Data, socketId);
                        break;
                    case GoodsOperateDefineManager.MSG_GOODSOPERATE_QUICKSMELT:
                        AddToInvoker(ReceiveGoodsOperateQuickSmeltMsg, package.Data, socketId);
                        break;
                    default:
                        break;
			        
            }
                break;
        }
    }
    #region 接收消息处理

    CommandCallbackType ReceiveEquipMakeMsg(byte[] dataBuffer,int socketID)
    {
     
        SMsgGoodsOperateEquipMake_SC sMsgGoodsOperateEquipLevelUp_SC = SMsgGoodsOperateEquipMake_SC.ParsePackage(dataBuffer);
        TraceUtil.Log(SystemModel.wanglei,"铸造装备"+ sMsgGoodsOperateEquipLevelUp_SC.bySucess);
       UIEventManager.Instance.TriggerUIEvent(UIEventType.Forging,sMsgGoodsOperateEquipLevelUp_SC);
        return CommandCallbackType.Continue;
    }

	CommandCallbackType ReceiveEquipmentUpLevelMsg(byte[] dataBuffer,int socketID)
	{
		TraceUtil.Log(SystemModel.Jiang,"收到装备升级回复");
		SMsgGoodsOperateEquipLevelUp_SC sMsgGoodsOperateEquipLevelUp_SC = SMsgGoodsOperateEquipLevelUp_SC.ParsePackage(dataBuffer);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.EqipmentLevelUp,sMsgGoodsOperateEquipLevelUp_SC);
		return CommandCallbackType.Continue;
	}
    /// <summary>
    /// 接受强化十次回复
    /// </summary>
    /// <returns>The goods operate quick smelt message.</returns>
    /// <param name="dataBuffer">Data buffer.</param>
    /// <param name="socketID">Socket I.</param>
    CommandCallbackType ReceiveGoodsOperateQuickSmeltMsg(byte[] dataBuffer,int socketID)
    {
        TraceUtil.Log(SystemModel.wanglei,"收到装备强化十次");
        SGoodsOperateQuickSmelt_SC sMsgGoodsOperateEquipLevelUp_SC = SGoodsOperateQuickSmelt_SC.ParsePackage(dataBuffer);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.QuickSmelt,sMsgGoodsOperateEquipLevelUp_SC);
        return CommandCallbackType.Continue;
    }
    CommandCallbackType ReceiveOpenTreasureUI(byte[] dataBuffer, int socketID)
    {
        TraceUtil.Log("收到打开宝箱面板消息");
        SMsgGoodsOperateOpenTreasureUI_SC sMsgGoodsOperateOpenTreasureUI_SC = SMsgGoodsOperateOpenTreasureUI_SC.ParsePackage(dataBuffer);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UI.MainUI.UIType.PlayerLuckDraw);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenTreasureUI, sMsgGoodsOperateOpenTreasureUI_SC);
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ReceiveClickTreasure(byte[] dataBuffer, int socketID)
    {
        SMsgGoodsOperateClickTreasure_SC sMsgGoodsOperateClickTreasure_SC = SMsgGoodsOperateClickTreasure_SC.ParsePackage(dataBuffer);
        TraceUtil.Log("收到打开宝箱消息:" + sMsgGoodsOperateClickTreasure_SC.dwGoodsNum);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenTreasureChest,sMsgGoodsOperateClickTreasure_SC);
        return CommandCallbackType.Continue;
    }

    /// <summary>
    /// 收到装备S发送给客户端的NPC应答应答给C  新版本没有锻造NPC
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    CommandCallbackType ReceiveGoodsOperate_OpenMakeFaceCommand(byte[] dataBuffer, int socketID)
    {
        var sMsgGoodsOperateOpenMakeFace = SMsgGoodsOperateOpenMakeFace.ParseResultPackage(dataBuffer);

        //UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.EquipStrengthen);
        //RaiseEvent(EventTypeEnum.OpenEquipStrengthenUI.ToString(), sMsgGoodsOperateOpenMakeFace);
        //RaiseEvent(EventTypeEnum.OpenItemOperateUI.ToString(), sMsgGoodsOperateOpenMakeFace);
        //UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UI.MainUI.UIType.EquipStrengthen);
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// S发送强化应答给C
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="socketID"></param>
    /// <returns></returns>
    CommandCallbackType ReceiveGoodsOperate_SmeltCommand(byte[] dataBuffer, int socketID)
    {
        var sMsgGoodsOperateEquipmentStrength_SC = SMsgGoodsOperateEquipmentStrength_SC.ParseResultPackage(dataBuffer);

        RaiseEvent(EventTypeEnum.GoodsOperateSmelt.ToString(), sMsgGoodsOperateEquipmentStrength_SC);
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// 宝箱奖励界面请求
    /// </summary>
    CommandCallbackType ReceiveOpenActiveChestUI(byte[] dataBuffer, int socketID)
    {
        SMsgOpenActiveChestUISC sMsgOpenActiveChestUISC = SMsgOpenActiveChestUISC.ParsePackage(dataBuffer);
        //TraceUtil.Log("[sMsgOpenActiveChestUISC] process = " + sMsgOpenActiveChestUISC.dwProgress);
		DailyTaskManager.Instance.UpdateProcess(sMsgOpenActiveChestUISC.dwProgress);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenActiveChestUI, sMsgOpenActiveChestUISC);        
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// 打开宝箱的奖励信息
    /// </summary>    
    CommandCallbackType ReceiveOpenChestValue(byte[] dataBuffer, int socketID)
    {
        SMsgOpenActiveChestSC sMsgOpenActiveChestSC = SMsgOpenActiveChestSC.ParsePackage(dataBuffer);
        TraceUtil.Log("[ReceiveOpenChestValue]");
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenActiveChest, sMsgOpenActiveChestSC);
        return CommandCallbackType.Continue;
    }
	#region 接受器魂相关消息
	/// <summary>
	/// 接收宝石镶嵌回复
	/// </summary>
	/// <returns>The goods operate beset results.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType ReceiveGoodsOperateBesetResults(byte[] dataBuffer,int socketID)
	{
		SMsgGoodsOperateBeset_SC sMsgGoodsOperateBeset_SC = SMsgGoodsOperateBeset_SC.ParsePackage (dataBuffer);
		TraceUtil.Log ("ReceiveGoodsOperateBesetResults:"+sMsgGoodsOperateBeset_SC);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.ReceiveBesetJewel, sMsgGoodsOperateBeset_SC); 
		return CommandCallbackType.Continue;
	}
	/// <summary>
	/// 接收宝石移除回复
	/// </summary>
	/// <returns>The goods operate remove results.</returns>
	/// <param name="dataBuffer">Data buffer.</param>
	/// <param name="socketID">Socket I.</param>
	CommandCallbackType ReceiveGoodsOperateRemoveResults(byte[] dataBuffer,int socketID)
	{
		SMsgGoodsOperateRemove_SC sMsgGoodsOperateRemove_SC = SMsgGoodsOperateRemove_SC.ParsePackage(dataBuffer);
		TraceUtil.Log ("ReceiveGoodsOperateRemoveResults:"+sMsgGoodsOperateRemove_SC);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.ReceiveRemoveJewel, sMsgGoodsOperateRemove_SC); 
		return CommandCallbackType.Continue;
	}
	CommandCallbackType ReceiveGoodsOperateSwallowResults(byte[] dataBuffer,int socketID)
	{
		SMsgGoodsOperateSwallow_SC sMsgGoodsOperateSwallow_SC = SMsgGoodsOperateSwallow_SC.ParsePackage (dataBuffer);
		TraceUtil.Log ("ReceiveGoodsOperateSwallowResults:"+sMsgGoodsOperateSwallow_SC);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.ReceiveSwallowJewel, sMsgGoodsOperateSwallow_SC); 
		return CommandCallbackType.Continue;
	}
    #endregion

    CommandCallbackType ReceiveSMsgLuckDrawResult_SC(byte[] dataBuffer, int socketID)
    {
        SMsgLuckDrawResult_SC sMsgLuckDrawResult_SC = SMsgLuckDrawResult_SC.ParsePackage(dataBuffer);

        UIEventManager.Instance.TriggerUIEvent(UIEventType.LuckDrawResult, sMsgLuckDrawResult_SC);

        return CommandCallbackType.Continue;
    }

    #endregion


    #region 发送装备强化消息
    /// <summary>
    /// 发送打开宝箱请求
    /// </summary>
    /// <param name="TreasureType"></param>
    public void SendOpenTreasureToSever(byte TreasureType)
    {
        SMsgGoodsOperateClickTreasure_CS sMsgGoodsOperateClickTreasure_CS = new SMsgGoodsOperateClickTreasure_CS() { byTreasureType = TreasureType };
        this.Request(sMsgGoodsOperateClickTreasure_CS.GeneratePackage());
    }


    /// <summary>
    /// 发送点击NPC打开强化页面
    /// </summary>
    /// <param name="sMsgGoodsOperateOpenMakeFace"></param>
    public void SendGoodsOperateCommand(SMsgGoodsOperateOpenMakeFace sMsgGoodsOperateOpenMakeFace)
    {
        Package pkg = sMsgGoodsOperateOpenMakeFace.GeneratePackage();

        this.Request(pkg);
    }
    /// <summary>
    /// 发送装备强化指令
    /// </summary>
    /// <param name="sMsgGoodsOperateEquipmentStrength"></param>
    public void SendGoodsOperateEquipmentStrengthCommand(SMsgGoodsOperateEquipmentStrength sMsgGoodsOperateEquipmentStrength)
    {
        Package pkg = sMsgGoodsOperateEquipmentStrength.GeneratePackage();

        this.Request(pkg);
    }
    /// <summary>
    /// 发送装备强化十次指令
    /// </summary>
    /// <param name="sMsgGoodsOperateEquipmentStrength"></param>
    public void SendGoodsOperateQuickSmeltCommand(SGoodsOperateQuickSmelt_CS sGoodsOperateQuickSmelt_CS)
    {
        Package pkg = sGoodsOperateQuickSmelt_CS.GeneratePackage();
        
        this.Request(pkg);
    }
    #endregion
    /// <summary>
    /// 发送装备炼化到服务器
    /// </summary>
    /// <param name="equipUID"></param>
    /// <param name="artificeNum"></param>
    /// <param name="equipUidList"></param>
    public void SendGoodsOperateArtificeCommoand(long equipUID, uint artificeNum, List<long> equipUidList)
    {
        SMsgGoodsOperateArtificeCS sMsgGoodsOperateArtificeCS = new SMsgGoodsOperateArtificeCS()
        {
            EquipUid = equipUID,
            ArtificeNum = artificeNum,
            EquipUidList = equipUidList,
        };
        this.Request(sMsgGoodsOperateArtificeCS.GeneratePackage());
    }
    /// <summary>
    /// 发送装备洗练到服务器
    /// </summary>
    /// <param name="equipUID"></param>
    public void SendGoodsOperateSophisticationCommoand(long equipUID)
    {
        SMsgGoodsOperateSophisticationCS sMsgGoodsOperateSophisticationCS = new SMsgGoodsOperateSophisticationCS()
        {
            EquipUId = equipUID,
        };
        this.Request(sMsgGoodsOperateSophisticationCS.GeneratePackage());
    }
    /// <summary>
    /// 发送获取女官任务宝箱奖励进度
    /// </summary>
    public void SendRequestActiveChestProgressCommand()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, GoodsOperateDefineManager.MSG_OPNE_ACTIVE_CHEST_UI);

        this.Request(pkg);
    }
    /// <summary>
    /// 发送领取宝箱信息
    /// </summary>
    /// <param name="process">当前第一个未开启宝箱序号</param>
    public void SendOpenChestValueCommand(int process)
    {
        SMsgOpenActiveChestCS sMsgOpenActiveChestCS = new SMsgOpenActiveChestCS();
        sMsgOpenActiveChestCS.dwProgress = process;
        this.Request(sMsgOpenActiveChestCS.GeneratePackage());
    }
	#region 发送器魂相关消息
	/// <summary>
	/// 发送镶嵌宝石的请求
	/// </summary>
	/// <param name="lEquipUid">L装备UID</param>
	/// <param name="lStoreUid">宝石uid.</param>
	/// <param name="byPlace">镶嵌到哪个孔（1-4）.</param>
	public void SendRequestGoodsOperateBesetCommmand(long EquipUid,long StoreUid,byte Place)
	{
		SMsgGoodsOperateBeset_CS sMsgGoodsOperateBeset_CS = new SMsgGoodsOperateBeset_CS ()
		{
			lEquipUid=EquipUid,
			lStoreUid=StoreUid,
			byPlace=Place,
		};
		this.Request(sMsgGoodsOperateBeset_CS.GeneratePackage());
	}
	/// <summary>
	/// 发送摘除宝石请求
	/// </summary>
	/// <param name="EquipUid">Equip uid.</param>
	/// <param name="place">Place.</param>
	public void SendRequestGoodsOperateRemoveCommand(long EquipUid,byte place)
	{
		SMsgGoodsOperateRemove_CS sMsgGoodsOperateRemove_CS = new SMsgGoodsOperateRemove_CS ()
		{
			lEquipUid=EquipUid,
			byPlace=place,

		};
		this.Request (sMsgGoodsOperateRemove_CS.GeneratePackage ());
	}
	public void SendRequestGoodsOperateSwallowCommand(long Uid,byte num,long[] Uids)
	{
		SMsgGoodsOperateSwallow_CS sMsgGoodsOperateSwallow_CS = new SMsgGoodsOperateSwallow_CS ()
		{
			lStoreUid=Uid,
			lSwallowNum=num,
			BeSwallowIDS=Uids
			
		};
		this.Request (sMsgGoodsOperateSwallow_CS.GeneratePackage ());
	}
	/// <summary>
	/// 发送装备升级
	/// </summary>
	/// <param name="itemID">Item I.</param>
	public void SendEquipmentLevelUp(long itemID)
	{
		SMsgGoodsOperateEquipLevelUp_CS sMsgGoodsOperateEquipLevelUp_CS= new SMsgGoodsOperateEquipLevelUp_CS()
		{
			EquipUid = itemID,
		};
		this.Request(sMsgGoodsOperateEquipLevelUp_CS.GEneratePackage());
	}


    public void SendSMsgLuckDraw_CS(SMsgLuckDraw_CS sMsgLuckDraw_CS)
    {
        this.Request(sMsgLuckDraw_CS.GeneratePackage());
    }

    public void SendSEquipMakeMsg(uint recipeID)
    {
        SMsgGoodsOperateEquipMake_CS SMsgGoodsOperateEquipMake_SC=new SMsgGoodsOperateEquipMake_CS()
        {
            dwForgeID=recipeID,
        };
        this.Request(SMsgGoodsOperateEquipMake_SC.GeneratePackage());
    }
}
#endregion