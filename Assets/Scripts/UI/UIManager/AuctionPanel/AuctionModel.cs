using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;


public class AuctionModel:ISingletonLifeCycle {
	private static AuctionModel instance;
	public static AuctionModel Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AuctionModel();
				SingletonManager.Instance.Add(instance);
			}
			return instance;
		}
	}
	//服务器下发所有竞拍物品数据
	public SAuctionOpenUI_SC sAuctionOpenUI_SC;
	//是否有请求过竞拍数据
	public bool isHaveRequestAuctionData = false;
	//如果时间下发不是在界面存在时打开，还得存一个preSinceTime,在下发时存一下//
	public float auctionUpdatDownTime = 0;
	#region 封装
	//测试使用
	public void Test()
	{
		isHaveRequestAuctionData = true;
		sAuctionOpenUI_SC = new SAuctionOpenUI_SC ();
		sAuctionOpenUI_SC.TimeInterval = 122;
		sAuctionOpenUI_SC.byUintNum = 20;
		sAuctionOpenUI_SC.auctionMap = new Dictionary<byte, DAuctionUint> ();
		for (int i = 0; i < (int)sAuctionOpenUI_SC.byUintNum; i++) {
			DAuctionUint test = new DAuctionUint();
			test.byIndex = (byte)i;
			test.dwGoodsID = (uint)1000000;
			test.dwGoodsNum = (uint)(i+5);
			test.dwCurPrice = (uint)4000;
			test.dwAcotorID = (uint)i;
			test.szName = new byte[]{1,2,3};
			sAuctionOpenUI_SC.auctionMap.Add(test.byIndex,test);		
		}
	}
	public DAuctionUint GetGoodsInfo(int byIndex)
	{
		/*foreach (DAuctionUint auction in sAuctionOpenUI_SC.auctionList) {
		if(auction.byIndex == byIndex)
				return auction;
		}*/
		return sAuctionOpenUI_SC.auctionMap[(byte)byIndex];
	}
	//获取当前是谁出的价
	public string GetBuyerName(DAuctionUint auction)
	{
		if (auction.dwAcotorID == 0) {
			return "[b1dbff]"+LanguageTextManager.GetString ("IDS_I27_15")+"[-]";
		}
		return "[b1dbff]"+Encoding.UTF8.GetString (auction.szName)+"[-]";
	}
	#endregion
	#region 处理服务器数据
	//下发竞拍结果
	public void ReceiveAuctionResult(SAuctionGoods_SC sAuctionGoods_SC)
	{
		sAuctionOpenUI_SC.auctionMap.Remove(sAuctionGoods_SC.byIndex);
		sAuctionOpenUI_SC.auctionMap.Add (sAuctionGoods_SC.byIndex,sAuctionGoods_SC.auction);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.ReceiveServerAuctionResult,sAuctionGoods_SC.byIndex);
	}
	//请求竞拍
	public void SendAuctionRequest(int byIndex,int money)
	{
		SAuctionGoods_CS sAuctionGoods_CS = new SAuctionGoods_CS();
		sAuctionGoods_CS.dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel ().ActorID;
		sAuctionGoods_CS.byIndex = (byte)byIndex;
		sAuctionGoods_CS.dwAuctionMoney = (uint)money;
		NetServiceManager.Instance.TradeService.SendSAuctionGoods_CS(sAuctionGoods_CS);
	}
	                           
	#endregion

	public void Instantiate()
	{
		
	}
	
	public void LifeOver()
	{
		instance = null;
	}
}
