using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

// 向客户端发送 打开NPC商店
// MSG_TRADE_OPENSHOP
// 消息体
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTradeOpenShop_CS
{
    public Int64 uidNPC;			// NPC商人	
    public uint dwShopID;		// 商店ID
}
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTradeOpenShop_SC
{
	public Int64 uidNPC;			// NPC商人	
    public uint dwShopID;		// 商店ID
    //[MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]
    //public byte[]	szShopName;	// 商店名字32
    //public string ShopName;
	//uint			dwContainerID;	// 商店容器ID
	//byte			byBuyType;		// 商店购买类型
	public byte bShopGoodsNum;	// 商店中物品数量
    public SMsgTradeOpenShopGoodsInfo_SC[] ShopGoodsInfo;

    public static SMsgTradeOpenShop_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgTradeOpenShop_SC sMsgTradeOpenShop_SC = new SMsgTradeOpenShop_SC();
        int of = 0;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShop_SC.uidNPC);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShop_SC.dwShopID);
        //sMsgTradeOpenShop_SC.szShopName = dataBuffer.Skip(of).Take(32).ToArray();
        //of += 32;
        //sMsgTradeOpenShop_SC.ShopName = Encoding.UTF8.GetString(sMsgTradeOpenShop_SC.szShopName);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShop_SC.bShopGoodsNum);
        
        SMsgTradeOpenShopGoodsInfo_SC[] GoodsInfoValue = new SMsgTradeOpenShopGoodsInfo_SC[sMsgTradeOpenShop_SC.bShopGoodsNum];
        for (int i = 0; i < GoodsInfoValue.Length;i++ )
        {
            GoodsInfoValue[i] = SMsgTradeOpenShopGoodsInfo_SC.ParsePackage(dataBuffer.Skip(of).ToArray());
            of += Marshal.SizeOf(typeof(SMsgTradeOpenShopGoodsInfo_SC));
        }
        sMsgTradeOpenShop_SC.ShopGoodsInfo = GoodsInfoValue;

        return sMsgTradeOpenShop_SC;
    }
}
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTradeOpenShopGoodsInfo_SC
{
    public uint dGoodsID;			//商品物品ID
    public uint dShopGoodsID;			//商品ID
    public byte bType;				//商店购买类型(1.铜币，2=绑定元宝，3=金元宝)
    public uint dPrice;				//商店价格
    public uint dwExChangeGoodsID;  //兑换ID
    public uint dwExChangeGoodsNumber;//兑换数量
    public byte byRow;				//商品行位置
    public byte byList;				//商品列位置
    public byte bPrecent;			//商城折扣比
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] dShevesTime;		//商品上架时间(两个uint的数组)
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] dDiscountTime;	//商店折扣时间(两个uint的数组)

    public static SMsgTradeOpenShopGoodsInfo_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgTradeOpenShopGoodsInfo_SC sMsgTradeOpenShopGoodsInfo_SC = new SMsgTradeOpenShopGoodsInfo_SC();
        //int of = Marshal.SizeOf(typeof(SMsgTradeOpenShop_SC));
        int of = 0;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShopGoodsInfo_SC.dGoodsID);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShopGoodsInfo_SC.dShopGoodsID);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShopGoodsInfo_SC.bType);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShopGoodsInfo_SC.dPrice);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShopGoodsInfo_SC.dwExChangeGoodsID);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShopGoodsInfo_SC.dwExChangeGoodsNumber);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShopGoodsInfo_SC.byRow);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShopGoodsInfo_SC.byList);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTradeOpenShopGoodsInfo_SC.bPrecent);
        byte[] TimeValue = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out TimeValue[i]);
        }
        sMsgTradeOpenShopGoodsInfo_SC.dShevesTime = TimeValue;
        for (int i = 0; i < 8; i++)
        {
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out TimeValue[i]);
        }
        sMsgTradeOpenShopGoodsInfo_SC.dDiscountTime = TimeValue;
        return sMsgTradeOpenShopGoodsInfo_SC;
    }
}


// 跟商人买物品
// MSG_TRADE_BUYGOODS
// 消息体
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public  struct SMsgTradeBuyGoods_CS
{
    public uint dwShopID;	// 商店ID
    public Int64 uidNPC;		// NPC商人
    public uint lShopGoodsID; //商品ID
    public uint GoodsID;	// 买入物品
    public uint GoodsNum;//买入的商品个数

    public Package GeneratePackage()
    {
        Package Pak = new Package();
        Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TRADE,TradeDefineManager.MSG_TRADE_BUYGOODS);
        Pak.Data = PackageHelper.StructToBytes<SMsgTradeBuyGoods_CS>(this);
        return Pak; 
    }
};

/// <summary>
/// 发送快捷购买协议请求[注意：此快捷购买协议买的都是商品，其实都是商店系统，而且真正的快速购买协议是指购买：体力，pvp次数，仙露等]
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgTradeQuickBuy_CS
{
    public uint GoodsID;		//物品ID，包括体力，等
    public uint GoodsNum;		//物品数量，包括体力值
    public Package GeneratePackage()
    {
        Package Pak = new Package();
        Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TRADE, (short)TradeDefineManager.MSG_TRADE_QUICK_BUY_GOODS);
        Pak.Data = PackageHelper.StructToBytes<SMsgTradeQuickBuy_CS>(this);
        return Pak;
    }
};
//快捷购买响应
struct SMsgTradeQuickBuy_SC
{
	public int	GoodsID;				//购买的物品ID
	public int	bySucess;				//是否购买成功
	public static SMsgTradeQuickBuy_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgTradeQuickBuy_SC sMsgTradeQuickBuy_SC = PackageHelper.BytesToStuct<SMsgTradeQuickBuy_SC>(dataBuffer);
		return sMsgTradeQuickBuy_SC;
	}
};
//社会服接收HTTP服务器数据后，发送给场景服的请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SPlatformResponePay_CSC_S
{
	// Int64 被改成 int
	public int lActorID;
	public int lPayNum;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
    public char[] OrderID;				//订单号

    public static SPlatformResponePay_CSC_S ParsePackage(byte[] dataBuffer)
    {
        SPlatformResponePay_CSC_S sPlatformResponePay_CSC_S = new SPlatformResponePay_CSC_S();

        int offset = 0;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sPlatformResponePay_CSC_S.lActorID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sPlatformResponePay_CSC_S.lPayNum);

        byte[] orderID = new byte[dataBuffer.Length - offset];
        for (int i = 0; i < orderID.Length; i++)
        {
            offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out orderID[i]);
        }
        sPlatformResponePay_CSC_S.OrderID = System.Text.Encoding.Default.GetChars(orderID);

        return sPlatformResponePay_CSC_S;
    }
};

//MSG_TRADE_BUYGOODS
//商品购买成功应答
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTradeBuyGoods_SC
{
	public  byte bySucess;
	
	public static SMsgTradeBuyGoods_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgTradeBuyGoods_SC sMsgTradeBuyGoods_SC = PackageHelper.BytesToStuct<SMsgTradeBuyGoods_SC>(dataBuffer);
		return sMsgTradeBuyGoods_SC;
	}
};

#region 竞拍
public struct DAuctionUint
{
    public byte    byIndex;            //物品编号
    public uint   dwGoodsID;          //物品ID
    public uint   dwGoodsNum;         //物品个数
    public uint   dwCurPrice;         //当前竞拍价格
    public uint   dwAcotorID;         //出价者角色ID,如果为0，表示无人竞拍
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[]    szName;         //出价者姓名
	public static DAuctionUint ParsePackage(byte[] dataBuffer, ref int offset)
	{
		DAuctionUint dAuctionUint = new DAuctionUint();
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out dAuctionUint.byIndex);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out dAuctionUint.dwGoodsID);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out dAuctionUint.dwGoodsNum);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out dAuctionUint.dwCurPrice);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out dAuctionUint.dwAcotorID);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out dAuctionUint.szName, 19);
		return dAuctionUint;
	}

};

//////////////////////////////////////////竞拍////////////////////////////////////////////////////////////
//打开UI
//MSG_TRADE_AUCTION_UI
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SAuctionOpenUI_SC
{
    public uint   TimeInterval;               //距离下次刷新时间点
    public byte    byUintNum;                  //单位个数
    //DAuctionUint
	public Dictionary<byte,DAuctionUint> auctionMap;
	
	public static SAuctionOpenUI_SC ParsePackage(byte[] dataBuffer)
	{
		SAuctionOpenUI_SC sAuctionOpenUI_SC = new SAuctionOpenUI_SC();
		int offset = 0;
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sAuctionOpenUI_SC.TimeInterval);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sAuctionOpenUI_SC.byUintNum);
		sAuctionOpenUI_SC.auctionMap = new Dictionary<byte, DAuctionUint>();
		for(int i = 0; i < sAuctionOpenUI_SC.byUintNum ; i++)
		{
			DAuctionUint dAuctionUint = DAuctionUint.ParsePackage(dataBuffer, ref offset);
			sAuctionOpenUI_SC.auctionMap.Add(dAuctionUint.byIndex,dAuctionUint);
		}
		return sAuctionOpenUI_SC;
		
	}
};

//MSG_TRADE_AUCTION_GOODS
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SAuctionGoods_CS
{
    public uint   dwActorID;                  //竞拍角色ID
    public byte    byIndex;                    //竞拍物品索引
    public uint   dwAuctionMoney;             //竞拍出价
    public Package GeneratePackage()
    {
        Package Pak = new Package();
        Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TRADE, (short)TradeDefineManager.MSG_TRADE_AUCTION_GOODS);
        Pak.Data = PackageHelper.StructToBytes<SAuctionGoods_CS>(this);
        return Pak;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SAuctionGoods_SC
{
    public byte byIndex;                       //竞拍物品索引
	public DAuctionUint auction;
    //更新现场 DAuctionUint
	public static SAuctionGoods_SC ParsePackage(byte[] dataBuffer)
	{
		SAuctionGoods_SC sAuctionGoods_SC = new SAuctionGoods_SC();
		int offset = 0;
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sAuctionGoods_SC.byIndex);
		sAuctionGoods_SC.auction = DAuctionUint.ParsePackage(dataBuffer, ref offset);
		return sAuctionGoods_SC;
	}
};


#endregion
////////////////////////////////////////////随身商店//////////////////////////////////////////////////////////////
//MSG_TRADE_CARRYSHOP_UI
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct DCarryShopUint
{
	public byte  byIndex;				  //上架编号
    public uint dwShopID;                 //商品ID
    public uint dwShopNum;                //商品个数
	public byte  byIsSale;                //是否卖完 0-未售完，1-售完
	public static DCarryShopUint ParsePackage(byte[] dataBuffer, ref int offset)
	{
		DCarryShopUint dCarryShopUint = new DCarryShopUint();
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out dCarryShopUint.byIndex);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out dCarryShopUint.dwShopID);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out dCarryShopUint.dwShopNum);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out dCarryShopUint.byIsSale);
		return dCarryShopUint;
	}
};

//打开UI
//MSG_TRADE_CARRYSHOP_UI
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SCarryShopOpenUI_CS
{
    public byte    byOpenType;                 //打开类型，0-默认打开，1-刷新
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TRADE, (short)TradeDefineManager.MSG_TRADE_CARRYSHOP_UI);
		Pak.Data = PackageHelper.StructToBytes<SCarryShopOpenUI_CS>(this);
		return Pak;
	}
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SCarryShopOpenUI_SC
{
    public uint   TimeInterval;               //距离下次刷新时间点
    public byte    byUintNum;                  //单位个数
    //DCarryShopUint
	public List<DCarryShopUint> shopUintMap ;
	public static SCarryShopOpenUI_SC ParsePackage(byte[] dataBuffer)
	{
		SCarryShopOpenUI_SC sCarryShopOpenUI_SC = new SCarryShopOpenUI_SC();
		int offset = 0;
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sCarryShopOpenUI_SC.TimeInterval);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sCarryShopOpenUI_SC.byUintNum);
		sCarryShopOpenUI_SC.shopUintMap = new List<DCarryShopUint> ();
		for(int i = 0; i < sCarryShopOpenUI_SC.byUintNum ; i++)
		{
			DCarryShopUint dCarryShopUint = DCarryShopUint.ParsePackage(dataBuffer, ref offset);
			sCarryShopOpenUI_SC.shopUintMap.Add(dCarryShopUint);
		}
		return sCarryShopOpenUI_SC;
	}
};

//随身商店物品购买
//MSG_TRADE_CARRYSHOP_BUY
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SCarryShopBuy_CS
{
	public byte byIndex;
    public uint dwShopID;                 //商品ID
    public uint dwShopNum;                //商品个数
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TRADE, (short)TradeDefineManager.MSG_TRADE_CARRYSHOP_BUY);
		Pak.Data = PackageHelper.StructToBytes<SCarryShopBuy_CS>(this);
		return Pak;
	}
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SCarryShopBuy_SC
{
	public byte    byIndex;            //对应第几个格子
    public byte    bySucess;           //是否购买成功
	public static SCarryShopBuy_SC ParsePackage(byte[] dataBuffer)
	{
		SCarryShopBuy_SC sCarryShopBuy_SC = PackageHelper.BytesToStuct<SCarryShopBuy_SC>(dataBuffer);
		return sCarryShopBuy_SC;
	}
};

//MSG_TRADE_CARRYSHOP_UNLOCK
//解锁商店格
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SCarryShopUnLock_CS
{
    public uint dwActorID;          //角色ID
    public byte byIndex;            //解锁第几个商店格
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TRADE, (short)TradeDefineManager.MSG_TRADE_CARRYSHOP_UNLOCK);
		Pak.Data = PackageHelper.StructToBytes<SCarryShopUnLock_CS>(this);
		return Pak;
	}
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SCarryShopUnLock_SC
{
	public byte byIndex;            //解锁第几个商店格
    public byte bySucess;              //是否成功
	public DCarryShopUint dCarryShopUint;
	public static SCarryShopUnLock_SC ParsePackage(byte[] dataBuffer)
	{
		SCarryShopUnLock_SC sCarryShopUnLock_SC = new SCarryShopUnLock_SC();
		int offset = 0;
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sCarryShopUnLock_SC.byIndex);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sCarryShopUnLock_SC.bySucess);
		sCarryShopUnLock_SC.dCarryShopUint = DCarryShopUint.ParsePackage(dataBuffer, ref offset);
		return sCarryShopUnLock_SC;
	}
};

#region 随机商店




#endregion



