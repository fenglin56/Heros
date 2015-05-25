using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

//打开物品操作界面类型
public enum OPENOPERATETYPE
{
	//宝石合成
	OPENOPERATE_MAKE_TYPE = 0,
	//强化
	OPENOPERATE_STRENGTH_TYPE,
	//镶嵌
	OPENOPERATE_BESET_TYPE,
	OPENOPERATE_MAX_TYPE
};


//打开生产生活制造界面(装备强化)
//MSG_LIVESKILL_OPEN_MAKEFACE,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGoodsOperateOpenMakeFace:INotifyArgs
{
    public byte byMakeType;				//NPC类型
    public Int64 uidNPC;					//NPC实体对象ID
    public static SMsgGoodsOperateOpenMakeFace ParseResultPackage(byte[] pkgData)
    {
        return PackageHelper.BytesToStuct<SMsgGoodsOperateOpenMakeFace>(pkgData);
    }
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, (short)GoodsOperateDefineManager.MSG_GOODSOPERATE_MAKE);
        pkg.Data = PackageHelper.StructToBytes<SMsgGoodsOperateOpenMakeFace>(this);

        return pkg;
    }
    public int GetEventArgsType()
    {
        return 0;
    }
};
struct SMsgGoodsOperateEquipmentStrength_SC : INotifyArgs
{
	public byte byStrengthType;	     //强化类型
    public byte bySucess;					//强化是否成功(0表示失败，1表示成功)
    public static SMsgGoodsOperateEquipmentStrength_SC ParseResultPackage(byte[] pkgData)
    {
        var entity = new SMsgGoodsOperateEquipmentStrength_SC();
		entity.byStrengthType = pkgData[0];
        entity.bySucess = pkgData[1];
        return entity;
    }

    public int GetEventArgsType()
    {
        return 0;
    }
};
//装备强化
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGoodsOperateEquipmentStrength : INotifyArgs
{
    /// <summary>
    /// 强化类型
    /// </summary>
    public byte byStrengthType;			
    /// <summary>
    /// 物品的模板ID
    /// </summary>
    public uint dGoodsID;				
    /// <summary>
    /// 强化物品的实体UID
    /// </summary>
    public Int64 uidGoods;				

    public static SMsgGoodsOperateEquipmentStrength ParseResultPackage(byte[] pkgData)
    {
        return PackageHelper.BytesToStuct<SMsgGoodsOperateEquipmentStrength>(pkgData);
    }
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, (short)GoodsOperateDefineManager.MSG_GOODSOPERATE_SMELT);
        pkg.Data = PackageHelper.StructToBytes<SMsgGoodsOperateEquipmentStrength>(this);

        return pkg;
    }
    public int GetEventArgsType()
    {
        return 0;
    }
};

//装备强化十次
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SGoodsOperateQuickSmelt_CS 
{
    /// <summary>
    /// 强化类型
    /// </summary>
    public byte byStrengthType;         
    /// <summary>
    /// 物品的模板ID
    /// </summary>
    public uint dGoodsID;               
    /// <summary>
    /// 强化物品的实体UID
    /// </summary>
    public Int64 uidGoods;              
    

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, (short)GoodsOperateDefineManager.MSG_GOODSOPERATE_QUICKSMELT);
        pkg.Data = PackageHelper.StructToBytes<SGoodsOperateQuickSmelt_CS>(this);
        
        return pkg;
    }

};

//强化十次应答
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SGoodsOperateQuickSmelt_SC
{
    public byte byStrengthType;
    public byte TipsNum;                                //角色ID
    //EmailID 列表
    public List<byte> TipsList;
    
    public static SGoodsOperateQuickSmelt_SC ParsePackage(byte[] dataBuffer)
    {
        
        SGoodsOperateQuickSmelt_SC QuickSmelt_SC = new SGoodsOperateQuickSmelt_SC();
        int offset = 0;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out QuickSmelt_SC.byStrengthType);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out QuickSmelt_SC.TipsNum);
        
        QuickSmelt_SC.TipsList = new List<byte>();
        for(int i = 0; i < QuickSmelt_SC.TipsNum; i++)
        {
            byte TipsType = 0;
            offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out TipsType);
            QuickSmelt_SC.TipsList.Add(TipsType);
        }
        
        return QuickSmelt_SC;
    }
}
/// <summary>
/// 收到的宝箱UI信息
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGoodsOperateOpenTreasureUI_SC
{
    uint dwNum;                        //界面显示的宝箱个数
    public List<SMsgTreasureData> TreasureDataList;  //宝箱详情

    public static SMsgGoodsOperateOpenTreasureUI_SC ParsePackage(byte[] DataBuffer)
    {
        SMsgGoodsOperateOpenTreasureUI_SC sMsgGoodsOperateOpenTreasureUI_SC = new SMsgGoodsOperateOpenTreasureUI_SC();
        int off = 0;
        off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgGoodsOperateOpenTreasureUI_SC.dwNum);
        sMsgGoodsOperateOpenTreasureUI_SC.TreasureDataList = new List<SMsgTreasureData>();
        for(int i = 0;i<sMsgGoodsOperateOpenTreasureUI_SC.dwNum;i++)
        {
            SMsgTreasureData sMsgTreasureData = new SMsgTreasureData();
            off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgTreasureData.dwTreasureType);
            off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgTreasureData.dwTreasureNum);
            off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgTreasureData.dwTreasureCostNum);
            off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgTreasureData.dwBuyTreasureCostNum);
            off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgTreasureData.byCostType);
            sMsgGoodsOperateOpenTreasureUI_SC.TreasureDataList.Add(sMsgTreasureData);
        }
        return sMsgGoodsOperateOpenTreasureUI_SC;
    }
}

public struct SMsgTreasureData
{
    public byte dwTreasureType;                 //宝箱的类型，1铁，2银，3金
    public uint dwTreasureNum;                //宝箱的数量
    public uint dwTreasureCostNum;            //开启宝箱所需消耗
    public uint dwBuyTreasureCostNum;			//买宝箱开启所需消耗（无宝箱但开宝箱）
    public byte byCostType;                   //消耗类型 0是铜钱, 1是元宝
};

/// <summary>
/// 打开宝箱请求
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public  struct SMsgGoodsOperateClickTreasure_CS
{
    public byte byTreasureType;          // 宝箱类型

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE,GoodsOperateDefineManager.MSG_GOODSOPERATE_CLICK_TREASURE);
        pkg.Data = PackageHelper.StructToBytes<SMsgGoodsOperateClickTreasure_CS>(this);
        return pkg;
    }
};

/// <summary>
/// 收到的打开的宝箱信息
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGoodsOperateClickTreasure_SC
{
    public byte byTreasureType;          // 宝箱类型
    public uint dwTreasureBoxNum;       //宝箱剩余数量
    public uint dwGoodsNum;              // 物品种类
    public List<SMsgGoodsOperateGetData> GoodsList;

    public static SMsgGoodsOperateClickTreasure_SC ParsePackage(byte[] DataBuffer)
    {
        SMsgGoodsOperateClickTreasure_SC sMsgGoodsOperateClickTreasure_SC = new SMsgGoodsOperateClickTreasure_SC();
        int off = 0;
        off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgGoodsOperateClickTreasure_SC.byTreasureType);
        off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgGoodsOperateClickTreasure_SC.dwTreasureBoxNum);
        off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgGoodsOperateClickTreasure_SC.dwGoodsNum);
        sMsgGoodsOperateClickTreasure_SC.GoodsList = new List<SMsgGoodsOperateGetData>();
        for (int i = 0; i < sMsgGoodsOperateClickTreasure_SC.dwGoodsNum; i++)
        {
            SMsgGoodsOperateGetData sMsgGoodsOperateGetData = new SMsgGoodsOperateGetData();
            off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgGoodsOperateGetData.dwGoodsID);
            off += PackageHelper.ReadData(DataBuffer.Skip(off).ToArray(), out sMsgGoodsOperateGetData.dwGoodsCount);
            sMsgGoodsOperateClickTreasure_SC.GoodsList.Add(sMsgGoodsOperateGetData);
        }
        return sMsgGoodsOperateClickTreasure_SC;
    }
}

public struct SMsgGoodsOperateGetData
{
    public uint dwGoodsID;               //物品ID
    public uint dwGoodsCount;              //物品数量
};


//装备洗练
//MSG_GOODSOPERATE_SOPHISTICATION
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGoodsOperateSophisticationCS
{
    public long EquipUId;					//装备UID

    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, GoodsOperateDefineManager.MSG_GOODSOPERATE_SOPHISTICATION);
        pak.Data = PackageHelper.StructToBytes<SMsgGoodsOperateSophisticationCS>(this);
        return pak;
    }
}


//装备炼化
//MSG_GOODSOPERATE_ARTIFICE
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGoodsOperateArtificeCS
{
    public long EquipUid;				//炼化的主装备UID
    public uint ArtificeNum;			//被炼化的装备个数，目前策划不超过六件
    public List<long> EquipUidList;//被选中的炼化装备
    //炼化现场 LONGLONG EquipUid 被炼化的装备UID(8个字节一个，根据被炼化装备个数来判定)

    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, GoodsOperateDefineManager.MSG_GOODSOPERATE_ARTIFICE);
        byte[] sendData;
        sendData = BitConverter.GetBytes(EquipUid).Concat(BitConverter.GetBytes(ArtificeNum)).ToArray();
        foreach (long child in EquipUidList)
        {
            sendData = sendData.Concat(BitConverter.GetBytes(child)).ToArray();
        }
        pak.Data = sendData;
        return pak;
    }
};


//MSG_OPNE_ACTIVE_CHEST_UI
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgOpenActiveChestUISC
{
    public int dwProgress;    

    public static SMsgOpenActiveChestUISC ParsePackage(byte[] DataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgOpenActiveChestUISC>(DataBuffer);
    }
};


//MSG_ACTIVE_VALUE_OPEN_CHEST
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgOpenActiveChestCS
{
    public int dwProgress;

    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, GoodsOperateDefineManager.MSG_ACTIVE_VALUE_OPEN_CHEST);
        pak.Data = PackageHelper.StructToBytes<SMsgOpenActiveChestCS>(this);
        return pak;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgOpenActiveChestSC
{
    public int dwGoodsID;									// 物品ID
    public int dwGoodsNum;									// 物品个数
    public int dwAwardMoney;									// 奖励经验
    public int dwAwardExp;									// 奖励经验
    public int dwAwardActive;								// 奖励活力
    public int dwAwardXiuwei;								// 奖励修为

    public static SMsgOpenActiveChestSC ParsePackage(byte[] DataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgOpenActiveChestSC>(DataBuffer);
    }
};

#region 器魂
/// <summary>
/// C发送宝石吞噬请求
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgGoodsOperateSwallow_CS 
{
	/// <summary>
	/// 器魂宝石ID
	/// </summary>
	public long lStoreUid;	
	/// <summary>
	///被吞噬的宝石个数
	/// </summary>
	public byte lSwallowNum;	
	/// <summary>
	/// The be swallow identifier.
	/// </summary>
	public long[] BeSwallowIDS;
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, GoodsOperateDefineManager.MSG_STORE_SWALLOW);
		byte[] sendData;
		sendData = BitConverter.GetBytes(lStoreUid).Concat(new byte[1]{lSwallowNum}).ToArray();
		foreach (long child in BeSwallowIDS)
		{
			sendData = sendData.Concat(BitConverter.GetBytes(child)).ToArray();
		}
		pak.Data = sendData;
		return pak;
	}
}
/// <summary>
/// S发送宝石吞噬应答
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgGoodsOperateSwallow_SC
{
	/// <summary>
	/// 吞噬是否成功
	/// </summary>
	public byte bySucess;		
	public static SMsgGoodsOperateSwallow_SC ParsePackage(byte[] DataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgGoodsOperateSwallow_SC>(DataBuffer);
	}
}
/// <summary>
/// C发送宝石摘除请求
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgGoodsOperateRemove_CS
{
	/// <summary>
	/// 装备UID
	/// </summary>
	public long lEquipUid;
	/// <summary>
	/// /装备上宝石孔位置
	/// </summary>
	public byte byPlace;				
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, GoodsOperateDefineManager.MSG_EQUIP_STORE_REMOVE);
		pak.Data = PackageHelper.StructToBytes<SMsgGoodsOperateRemove_CS> (this);;
		return pak;
	}
}
/// <summary>
/// S发送宝石摘除应答
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgGoodsOperateRemove_SC
{
	public byte Place;
    /// <summary>
	/// 摘除是否成功
	/// </summary>
	public byte bySucess;

	public static SMsgGoodsOperateRemove_SC ParsePackage(byte[] DataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgGoodsOperateRemove_SC>(DataBuffer);
	}
}
/// <summary>
/// C发送宝石镶嵌请求
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgGoodsOperateBeset_CS
{
	/// <summary>
	/// 装备UID
	/// </summary>
	public long lEquipUid;
	/// <summary>
	///宝石UID
	/// </summary>
	public long lStoreUid;	
	/// <summary>
	///装备上宝石孔位置(1-4)
	/// </summary>
	public byte byPlace;	
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, GoodsOperateDefineManager.MSG_GOODSOPERATE_BESET);
		pak.Data = PackageHelper.StructToBytes<SMsgGoodsOperateBeset_CS> (this);;
		return pak;
	}
}
/// <summary>
/// S发送宝石镶嵌应答
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgGoodsOperateBeset_SC
{
	/// <summary>·
	/// 宝石是否镶嵌成功
	/// </summary>
	public byte bySucess;			
	public static SMsgGoodsOperateBeset_SC ParsePackage(byte[] DataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgGoodsOperateBeset_SC>(DataBuffer);
	}
}

#endregion
/// <summary>
/// 装备升级请求
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGoodsOperateEquipLevelUp_CS
{
	public long	EquipUid;				//要升级装备的UID
	public Package GEneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, GoodsOperateDefineManager.MSG_EQUIP_LEVEL_UP);
		pak.Data = PackageHelper.StructToBytes<SMsgGoodsOperateEquipLevelUp_CS> (this);
		return pak;
	}
}

/// <summary>
/// 装备升级回复
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGoodsOperateEquipLevelUp_SC
{
	public long NewItemUID;					//升级后的新物品UID
	public byte	bySucess;					//装备是否升级成功
	public static SMsgGoodsOperateEquipLevelUp_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgGoodsOperateEquipLevelUp_SC>(dataBuffer);
	}
};


#region luckdraw

//MSG_LUCKDRAW
//发送抽奖类型给服务器
public struct SMsgLuckDraw_CS
{
    public byte        byType;                     // 1为一次，10为十次

    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, GoodsOperateDefineManager.MSG_LUCKDRAW);
        pak.Data = PackageHelper.StructToBytes<SMsgLuckDraw_CS> (this);;
        return pak;
    }

};

public enum LuckDrawResultType
{
    Normal = 0,        //普通中奖
    TripleHit = 1,     //连中三元
    Double = 2,        //双龙戏珠
    RewardMultiple = 3, //三倍奖励
    GetAll = 4,         //乾坤一掷

    TenTimeDrawResult = 10,  //抽奖十次

};


//抽奖结果
public struct SMsgLuckDrawResult_SC                
{   
    public byte        byType;                     //平常类型为0，连中三元为1，双龙戏珠为2，三倍奖励为3，乾坤一掷为4。 抽奖十次为10

    public byte        byNum;                      //显示个数滴

    public List<SLuckDrawResultInfo> resultInfoList;

    public static SMsgLuckDrawResult_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgLuckDrawResult_SC sMsgLuckDrawResult_SC = new SMsgLuckDrawResult_SC();
        int offset = 0;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMsgLuckDrawResult_SC.byType);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMsgLuckDrawResult_SC.byNum); 

        sMsgLuckDrawResult_SC.resultInfoList = new List<SLuckDrawResultInfo>();
        for(byte i = 0; i < sMsgLuckDrawResult_SC.byNum; i++)
        {
            SLuckDrawResultInfo sLuckDrawResultInfo = SLuckDrawResultInfo.ParsePackage(dataBuffer, ref offset);
            sMsgLuckDrawResult_SC.resultInfoList.Add(sLuckDrawResultInfo);
        }

        return sMsgLuckDrawResult_SC;
    }
};

//抽奖结果单个现场
public struct SLuckDrawResultInfo
{
    public byte        byID;
    public uint       dwGoodsID;
    public uint       dwGoodsNum;
    public static SLuckDrawResultInfo ParsePackage(byte[] dataBuffer, ref int offset)
    {
        SLuckDrawResultInfo sLuckDrawResultInfo = new SLuckDrawResultInfo();
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sLuckDrawResultInfo.byID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sLuckDrawResultInfo.dwGoodsID); 
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sLuckDrawResultInfo.dwGoodsNum); 
        return sLuckDrawResultInfo;
    }
};


#endregion

#region forging
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGoodsOperateEquipMake_CS
{
    public uint dwForgeID;
    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, GoodsOperateDefineManager.MSG_GOODSOPERATE_MAKE);
        pak.Data = PackageHelper.StructToBytes<SMsgGoodsOperateEquipMake_CS> (this);;
        return pak;
    }
}
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgGoodsOperateEquipMake_SC
{
    /// <summary>·
    /// 合成是否成功
    /// </summary>
    public byte bySucess;           
    public static SMsgGoodsOperateEquipMake_SC ParsePackage(byte[] DataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgGoodsOperateEquipMake_SC>(DataBuffer);
    }
}

#endregion





