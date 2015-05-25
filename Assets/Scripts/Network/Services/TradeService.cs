using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NetworkCommon;
using System.Runtime.InteropServices;

public class TradeService : Service
{


    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_TRADE:
                switch (package.GetSubMsgType())
                {
                    case TradeDefineManager.MSG_TRADE_OPENSHOP:
                        AddToInvoker(ReceiveOpenShopHandel, package.Data, socketId);
                        break;
                    case TradeDefineManager.MSG_TRADE_PAY_SUCESS:
                        AddToInvoker(TradePaySuccessHandle, package.Data, socketId);
                        break;
					case TradeDefineManager.MSG_TRADE_BUYGOODS:
						AddToInvoker(ReceiveShopsBuySuccessHandle, package.Data, socketId);
						break;
					case TradeDefineManager.MSG_TRADE_QUICK_BUY_GOODS:
						AddToInvoker(ReceiveQuickBuySuccessHandle, package.Data, socketId);
						break;

					case TradeDefineManager.MSG_TRADE_AUCTION_UI:
						AddToInvoker(ReceiveAuctionHandle, package.Data, socketId);
						break;
					case TradeDefineManager.MSG_TRADE_AUCTION_GOODS:
						AddToInvoker(ReceiveAuctionResultHandle, package.Data, socketId);
						break;
					case TradeDefineManager.MSG_TRADE_CARRYSHOP_UI:
						AddToInvoker(CarryShopUIResponseHandle, package.Data, socketId);
						break;
					case TradeDefineManager.MSG_TRADE_CARRYSHOP_BUY:
						AddToInvoker(CarryShopBuyResponseHandle, package.Data, socketId);
						break;
					case TradeDefineManager.MSG_TRADE_CARRYSHOP_UNLOCK:
						AddToInvoker(CarryShopUnLockResponseHandle, package.Data, socketId);
						break;
                    default:
                        break;
                }
                break;
            default:
                //TraceUtil.Log("不能识别的主消息" + package.GetMasterMsgType());
                break;
        }
    }

    #region//接收消息
    /// <summary>
    /// 打开商城
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <param name="sorketID"></param>
    /// <returns></returns>
    CommandCallbackType ReceiveOpenShopHandel(byte[] dataBuffer, int sorketID)
    {
        SMsgTradeOpenShop_SC sMsgTradeOpenShop_SC = SMsgTradeOpenShop_SC.ParsePackage(dataBuffer);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UI.MainUI.UIType.Empty);//清空其他UI界面
        if (UI.MainUI.MainUIController.Instance != null)
        {
            UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Shop, sMsgTradeOpenShop_SC);
        }
        return CommandCallbackType.Continue;
    }

    CommandCallbackType TradePaySuccessHandle(byte[] dataBuffer, int sorketID)
    {
        SPlatformResponePay_CSC_S sPlatformResponePay_CSC_S = SPlatformResponePay_CSC_S.ParsePackage(dataBuffer);
		UI.LoadingUI.Instance.Close();
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EctypeBuyActive");
        UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_524"), LanguageTextManager.GetString("IDS_H2_55"));
        return CommandCallbackType.Continue;
    }

	//商品购买成功回应【凡是属于商品的，如：铜币、喇叭...】//
	CommandCallbackType ReceiveShopsBuySuccessHandle(byte[] dataBuffer, int sorketID)
	{
		SMsgTradeBuyGoods_SC sMsgTradeBuyGoods_SC = SMsgTradeBuyGoods_SC.ParsePackage(dataBuffer);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.ShopsBuySuccess,sMsgTradeBuyGoods_SC.bySucess);
		return CommandCallbackType.Continue;
	}
	//[注意：此快捷购买协议买的都是商品，其实都是商店系统，而且真正的快速购买协议是指购买：体力，pvp次数，仙露等]
	//快速购买成功回应【体力、仙露、pvp次数】//
	CommandCallbackType ReceiveQuickBuySuccessHandle(byte[] dataBuffer, int sorketID)
	{
		SMsgTradeQuickBuy_SC sMsgTradeQuickBuy_SC = SMsgTradeQuickBuy_SC.ParsePackage(dataBuffer);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.QuickBuySuccess,sMsgTradeQuickBuy_SC);
		UI.MessageBox.Instance.ShowTips (3,LanguageTextManager.GetString("IDS_I7_43"), 1f);
		return CommandCallbackType.Continue;
	}

	//批量下发竞拍数据//
	CommandCallbackType ReceiveAuctionHandle(byte[] dataBuffer, int sorketID)
	{
		AuctionModel.Instance.sAuctionOpenUI_SC = SAuctionOpenUI_SC.ParsePackage(dataBuffer);
		AuctionModel.Instance.auctionUpdatDownTime = (float)AuctionModel.Instance.sAuctionOpenUI_SC.TimeInterval;
	//  AuctionModel.Instance.isHaveRequestAuctionData = true;
		UIEventManager.Instance.TriggerUIEvent(UIEventType.ReceiveServerAuctionData,null);
		return CommandCallbackType.Continue;
	}
	//下发竞拍结果数据//
	CommandCallbackType ReceiveAuctionResultHandle(byte[] dataBuffer, int sorketID)
	{
		AuctionModel.Instance.ReceiveAuctionResult (SAuctionGoods_SC.ParsePackage(dataBuffer));
		return CommandCallbackType.Continue;
	}

	///////////////////////////////////////////////////随身商店///////////////////////////////////////////////// 
	//批量下发请求数据
	CommandCallbackType CarryShopUIResponseHandle(byte[] dataBuffer, int sorketID)
	{
		CarryShopModel.Instance.sCarryShopOpenUI_SC = SCarryShopOpenUI_SC.ParsePackage(dataBuffer);
		CarryShopModel.Instance.carryShopUpdatDownTime = (int)CarryShopModel.Instance.sCarryShopOpenUI_SC.TimeInterval;
		CarryShopModel.Instance.carryShopRealtimeSinceStartup = Time.realtimeSinceStartup;
		CarryShopModel.Instance.RankCarryShopData();
		CarryShopModel.Instance.isRequestedServerData = true;
		UIEventManager.Instance.TriggerUIEvent(UIEventType.RcvCarryShopUIDataEvent,null);
		return CommandCallbackType.Continue;
	}
	//响应购买
	CommandCallbackType CarryShopBuyResponseHandle(byte[] dataBuffer, int sorketID)
	{
		CarryShopModel.Instance.ReceiveBuyResult (SCarryShopBuy_SC.ParsePackage(dataBuffer));
		return CommandCallbackType.Continue;
	}
	//响应解锁
	CommandCallbackType CarryShopUnLockResponseHandle(byte[] dataBuffer, int sorketID)
	{
		CarryShopModel.Instance.ReceiveUnLockResult(SCarryShopUnLock_SC.ParsePackage(dataBuffer));
		return CommandCallbackType.Continue;
	}
    #endregion

    #region//发送消息
    /// <summary>
    /// 发送购买物品
    /// </summary>
    public void SendTradeBuyGoods(SMsgTradeBuyGoods_CS sMsgTradeBuyGoods_CS)
    {
        this.Request(sMsgTradeBuyGoods_CS.GeneratePackage());
        //var sendBuff = PackageHelper.GetNetworkSendBuffer(sMsgTradeBuyGoods_CS.GeneratePackage());
        //TraceUtil.Log("SendBuff:" + sendBuff.Length);
    }

    /// <summary>
    /// 发送快捷购买
    /// </summary>
    /// <param name="GoodsID"></param>
    /// <param name="GoodsNumber"></param>
    public void SendTradeQuickBuyGoods(uint GoodsID, uint GoodsNumber)
    {
        SMsgTradeQuickBuy_CS sMsgTradeQuickBuy_CS = new SMsgTradeQuickBuy_CS();
        sMsgTradeQuickBuy_CS.GoodsID = GoodsID;
        sMsgTradeQuickBuy_CS.GoodsNum = GoodsNumber;
        this.Request(sMsgTradeQuickBuy_CS.GeneratePackage());
    }

    /// <summary>
    /// 发送竞拍UI数据请求
    /// </summary>
    public void SendMsg_Trade_Auction_UI()
    {
        Package Pak = new Package();
        Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TRADE, (short)TradeDefineManager.MSG_TRADE_AUCTION_UI);
        this.Request(Pak);
    }

    /// <summary>
    /// 发送竞拍请求
    /// </summary>
    /// <param name="sAuctionGoods_CS">S auction goods_ C.</param>
    public void SendSAuctionGoods_CS(SAuctionGoods_CS sAuctionGoods_CS)
    {
        this.Request(sAuctionGoods_CS.GeneratePackage());
    }


	///////////////////////////////////////////////////随身商店/////////////////////////////////////////////////
	/// <summary>
	/// 请求刷新界面 0-默认打开，1-刷新
	/// </summary>
	/// <param name="sAuctionGoods_CS">S auction goods_ C.</param>
	public void SendCarryShopUI_CS(int isRefresh)
	{
		SCarryShopOpenUI_CS sCarryShopOpenUI_CS = new SCarryShopOpenUI_CS();
		sCarryShopOpenUI_CS.byOpenType = (byte)isRefresh;
		this.Request(sCarryShopOpenUI_CS.GeneratePackage());
	}
	/// <summary>
	/// 请求刷新界面
	/// </summary>
	/// <param name="sAuctionGoods_CS">S auction goods_ C.</param>
	public void SendCarryShopBuy_CS(uint shopID,byte byIndex,uint shopCount)
	{
		SCarryShopBuy_CS sCarryShopBuy_CS = new SCarryShopBuy_CS();
		sCarryShopBuy_CS.dwShopID = shopID;
		sCarryShopBuy_CS.byIndex = byIndex;
		sCarryShopBuy_CS.dwShopNum = shopCount;
		this.Request(sCarryShopBuy_CS.GeneratePackage());
	}
	/// <summary>
	/// 请求刷新界面
	/// </summary>
	/// <param name="sAuctionGoods_CS">S auction goods_ C.</param>
	public void SendCarryShopUnLock_CS(int actorID,int index)
	{
		SCarryShopUnLock_CS sCarryShopUnLock_CS = new SCarryShopUnLock_CS();
		sCarryShopUnLock_CS.dwActorID = (uint)actorID;
		sCarryShopUnLock_CS.byIndex = (byte)index;
		this.Request(sCarryShopUnLock_CS.GeneratePackage());
	}
    #endregion


}
