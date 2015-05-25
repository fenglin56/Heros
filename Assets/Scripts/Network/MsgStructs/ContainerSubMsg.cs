//物品消息体  装备，背包，药水
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;


//S发送创建客户端物品篮到C
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionSCHead
{
    public Int64 uidEntity;			// Object的guid
};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SBuildContainerClientContext
{
    public SMsgActionSCHead SMsgActionSCHead;
	public Int64 uidMaster;	// 虚拟物品蓝所属对象UID
	public ushort	wMaxSize;	// 虚拟物品篮最大容量
	public uint	dwPopedom;	// 对此虚拟物品篮的权限
	public uint	dwContainerID;	// 虚拟物品篮Id,容器ID
	public uint	dwContainerName;	// 虚拟物品篮名字,1是装备，2是包裹,3是药品		//EContainerName 对应数据里存储的容器ID
	public uint	dwContainerType;	// 虚拟物品篮类型		//EContainerType 真实的 虚拟的
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	public byte[]	szContainerName;	// 虚拟物品篮名字
	public byte	byMoneyType;	// 支付类型(0:普通, 1:兑换, 2:官职, 3:爵位)

    public static SBuildContainerClientContext ParsePackage(byte[] dataBuffer)
    {
        var headLength = Marshal.SizeOf(typeof(SMsgActionSCHead));

        SBuildContainerClientContext sBuildContainerClientContext = new SBuildContainerClientContext();      

        sBuildContainerClientContext.SMsgActionSCHead = new SMsgActionSCHead() { uidEntity = BitConverter.ToInt64(dataBuffer, 0) };
        int offset = headLength;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sBuildContainerClientContext.uidMaster);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sBuildContainerClientContext.wMaxSize);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sBuildContainerClientContext.dwPopedom);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sBuildContainerClientContext.dwContainerID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sBuildContainerClientContext.dwContainerName);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sBuildContainerClientContext.dwContainerType);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sBuildContainerClientContext.szContainerName,19);
        sBuildContainerClientContext.byMoneyType = dataBuffer[offset];

        //sBuildContainerClientContext.uidMaster = BitConverter.ToInt64(dataBuffer, 8+0);
        //sBuildContainerClientContext.wMaxSize = BitConverter.ToUInt16(dataBuffer, 8 + 8);
        //sBuildContainerClientContext.dwPopedom = BitConverter.ToUInt32(dataBuffer, 8 + 8 + 2);
        //sBuildContainerClientContext.dwContainerID = BitConverter.ToUInt32(dataBuffer, 8 + 8 + 2+4);
        //sBuildContainerClientContext.dwContainerName = BitConverter.ToUInt32(dataBuffer, 8 + 8 + 2 + 4 + 4);
        //sBuildContainerClientContext.dwContainerType = BitConverter.ToUInt32(dataBuffer, 8 + 8 + 2 + 4 + 4+4);
        //sBuildContainerClientContext.szContainerName = dataBuffer.Skip(8 + 8 + 2 + 4 + 4 + 4+4).Take(19).ToArray();
        //sBuildContainerClientContext.byMoneyType = dataBuffer[8 + 8 + 2 + 4 + 4 + 4 + 4+19];      

        return sBuildContainerClientContext;
    }

    public void SetMaxSize(ushort number)//增加物品栏数
    {
        this.wMaxSize = number;
        //TraceUtil.Log("ContainerSize:" + this.wMaxSize);
    }

};
//S发送背包栏物品位置同步到C
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgContainerCSCHead
{
    public uint dwContainerID;	// 虚拟物品栏Id,容器ID
    public byte dwSysGoodsNum; 
    public static SMsgContainerCSCHead ParsePackage(byte[] dataBuffer)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);

        return ParsePackage(package);
    }
    public static SMsgContainerCSCHead ParsePackage(Package package)
    {
        SMsgContainerCSCHead sMsgContainerCSCHead = new SMsgContainerCSCHead();
        sMsgContainerCSCHead.dwContainerID = BitConverter.ToUInt32(package.Data, 0);
        sMsgContainerCSCHead.dwSysGoodsNum = package.Data[4];
        //sMsgContainerCSCHead.dwSysGoodsNum = (byte)((package.Data.Length - 4) / (Marshal.SizeOf(typeof(SSyncContainerGoods_SC)) - Marshal.SizeOf(typeof(SMsgContainerCSCHead))));
        return sMsgContainerCSCHead;
    }
};
//同步虚拟物品栏中物品的位置
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSyncContainerGoods_SC
{
    public SMsgContainerCSCHead SMsgContainerCSCHead;
    public int nPlace;			// 物品在虚拟物品栏中的位置
    public Int64 uidGoods;		// 虚拟栏中物品的uid
    public byte byNum;			// 物品数量
    public uint dwRelayContainerID;	// 交互的虚拟物品篮Id
    public byte byLink;			// 真实物品是否已链接(0:否 1:是)

    public static SSyncContainerGoods_SC ParsePackage(byte[] pkgDataBuffer, int offset, SMsgContainerCSCHead sMsgContainerCSCHead)
    {
        var targetBuffer = pkgDataBuffer.Skip(offset).ToArray();
        SSyncContainerGoods_SC sSyncContainerGoods_SC = new SSyncContainerGoods_SC();
        sSyncContainerGoods_SC.SMsgContainerCSCHead = sMsgContainerCSCHead;
        sSyncContainerGoods_SC.nPlace = BitConverter.ToInt32(targetBuffer, 0);
        sSyncContainerGoods_SC.uidGoods = BitConverter.ToInt64(targetBuffer, 4);
        sSyncContainerGoods_SC.byNum = targetBuffer[12];
        sSyncContainerGoods_SC.dwRelayContainerID = BitConverter.ToUInt32(targetBuffer, 13);
        sSyncContainerGoods_SC.byLink = targetBuffer[17];
        //TraceUtil.Log("收到物品同步消息：" + sSyncContainerGoods_SC.nPlace + ",ContainerID:" + sSyncContainerGoods_SC.dwRelayContainerID + "," + sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID);
        return sSyncContainerGoods_SC;
    }  

};
//同步虚拟物品栏中物品的位置
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSyncContainerGoods_CS
{
    public uint ContainerID;
    public uint dwSrcContainerID;		//源容器ID
    public byte bySrcPlace;			//源容器位置
    public uint dwDstContainerID;		//目标容器ID
    public byte byDstPlace;			//目标容器位置

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_CONTAINER, ContainerDefineManager.MSG_CONTAINER_SYNC);
        pkg.Data = PackageHelper.StructToBytes<SSyncContainerGoods_CS>(this);
        TraceUtil.Log("SendData:" + pkg.Data.Length);
        return pkg;
    }  
};

//s发送修改背包容量
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgContainerChangeSize_SC
{
    public SMsgContainerCSCHead SMsgContainerCSCHead;
    public ushort vMaxSize;//虚拟物品栏最大容量

    public static SMsgContainerChangeSize_SC ParsePackage(byte[] DataBuffer)
    {
        SMsgContainerChangeSize_SC sMsgContainerChangeSize_SC = new SMsgContainerChangeSize_SC();

        SMsgContainerCSCHead sMsgContainerCSCHead = new SMsgContainerCSCHead();
        sMsgContainerCSCHead.dwContainerID = BitConverter.ToUInt32(DataBuffer, 0);
        sMsgContainerCSCHead.dwSysGoodsNum = DataBuffer[4];
        sMsgContainerChangeSize_SC.SMsgContainerCSCHead = sMsgContainerCSCHead;
        sMsgContainerChangeSize_SC.vMaxSize = BitConverter.ToUInt16(DataBuffer, 5); 
        
        return sMsgContainerChangeSize_SC;
    }
};
// 客户端到场景服消息头
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SC2SMsgHead
{
    //public byte m_wKeyRoot;
    //public uint clientId;	 // 客户端识别ID
    public byte m_byMainMsg; // 主消息码
    public ushort m_wSubMsgId; // 子消息ID 
};

// 物品使用请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgContainerUse_CS
{
    //public SC2SMsgHead sc2sMsgHead;
    public uint dwContainerID1;	// 容器ID
    public uint dwContainerID2;	// 容器ID
    public byte byPlace;		// 虚拟物品栏中的位置
    public Int64 uidTarget;		// 作用目标UID
    public int desPlace;     // 卸载物品到背包指定位置，其他不用该数值
    public byte byUseType;   //0默认装配，1药品装配，0为时装穿戴，1为时装卸下
    public byte byGoodsNum;     //物品使用个数

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SMsgContainerUse_CS>(this);
        return pkg;
    }  
};

// 整理容器内的物品
//  MSG_CONTAINER_TIDY
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgContainerTidy_CS
{
    public uint dwContainerID1;             // 要整理的容器ID
    public uint dwContainerID2;             // 要整理的容器ID

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SMsgContainerTidy_CS>(this);

        //TraceUtil.Log("DataLength:"+pkg.Head.DataLength);
        return pkg;
    }  
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgContainerObserver_CS
{
    public uint dwContainerID1;             // 要整理的容器ID
    public byte byOpen;			// (0:关闭 1:打开)

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SMsgContainerObserver_CS>(this);
        return pkg;
    }  
};
/// <summary>
/// 出售物品
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgContainerDoff_CS
{
	public uint	dwSrcContainerID_Heard;		//源容器ID
	public uint	dwSrcContainerID;		//源容器ID
	public byte	bySrcPlaceNum;		//出售道具位置数
	public Dictionary<byte,byte> sellItemList;//Key:位置，Value：数量
	//....位置组
//    public uint dwSrcContainerID1;		//源容器ID
//    public uint dwSrcContainerID2;		//源容器ID
//    public byte bySrcPlace;			//源容器位置

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
		List<byte> byteList = new List<byte>();
		byteList.AddRange(BitConverter.GetBytes(dwSrcContainerID_Heard));
		byteList.AddRange(BitConverter.GetBytes(dwSrcContainerID));
		byteList.Add(bySrcPlaceNum);
		foreach(var child in sellItemList)
		{
			byteList.Add(child.Key);
			byteList.Add(child.Value);
		}
        pkg.Data = byteList.ToArray();
        return pkg;
    }  
};

public struct SMsgContainerDoff_SC
{
	public byte	bySucess;			//是否出售成功
	public uint	dwSaleMoney;		//出售金币数
	public byte	byGoodsGroupNum;	//物品组
	public Dictionary<int,int>GetItemList;//获得得物品，Key:ID,Value:数量

	public static SMsgContainerDoff_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgContainerDoff_SC newData = new SMsgContainerDoff_SC();
		int offset = 0;
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out newData.bySucess);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out newData.dwSaleMoney);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out newData.byGoodsGroupNum);
		newData.GetItemList = new Dictionary<int, int>();
		for(int i = 0;i<newData.byGoodsGroupNum;i++)
		{
			int itemID = 0;
			int itemNum = 0;
			offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out itemID);
			offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out itemNum);
			newData.GetItemList.Add(itemID,itemNum);
		}
		return newData;
	}
}

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionUseMedicamentResult_SC
{
    //enum UseMedicamentResult
    //{
    //    USEMEDICAMENTRESULT_FAIL,		// 使用药品失败
    //    USEMEDICAMENTRESULT_SUCCESS,	// 使用药品成功
    //};
    public byte byResult;		// 使用药品结果

    public static SMsgActionUseMedicamentResult_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.ParseDataBufferToStruct<SMsgActionUseMedicamentResult_SC>(dataBuffer);
    }
};

//对容器内进行物品添加
//物品添加只能针对背包，装备栏无效
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgContainerAdd_CS
{
    //public SC2SMsgHead sc2sMsgHead;
    public uint dwSrcContainerID1;		//源容器ID
    public Int64 lUID;						//要拾取的物品UID

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SMsgContainerAdd_CS>(this);
        return pkg;
    }  
};

//解锁容器请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SmsgContainerChangeSize
{
    public long dwSrcContainerID;//容器ID

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SmsgContainerChangeSize>(this);
        return pkg;
    }  
}

//向服务器发送药品连接请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgContainerMedicamentLink_CS
{
    public uint dwSrcContainerID;//容器ID
    public uint LinkGoodsID;				//被链接的物品的ID,没有填0
    public uint GoodsID;					//物品的ID

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SMsgContainerMedicamentLink_CS>(this);
        return pkg;
    } 

};

//装备是否为最新协议
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgContainerGoodsNew_CS
{
	public int dwContainerID_Heard;
	public int dwContainerID;             // 对应的容器ID
	public byte dwPlace;					// 物品所在容器的位置

	public Package GeneratePackage()
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_CONTAINER,(short)ContainerDefineManager.MSG_CONTAINER_GOODS_NEW);
		pkg.Data = PackageHelper.StructToBytes<SMsgContainerGoodsNew_CS>(this);
		return pkg;
	}
};

//容器实体结构
//包括装备，背包，药丸等。每个物品的现场不一样。
//需要取得物品模板ID后，读取物品配置获得物品的类型，
//再将物品解析成相应的实体结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_Container : INotifyArgs, IEntityDataStruct
{
    private SMsgPropCreateEntity_SC_Header m_sMsg_Header;
    public int ItemtypeID;
    public int ItemTemplateID;//物品ID
    public byte[] ItemData; //长度及对应的物品实体由m_sMsg_Header里的物品模板ID决定
    /// <summary>
    /// 从整体字节数组获得实体上下文的具体数据
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <returns></returns>
    public static SMsgPropCreateEntity_SC_Container ParsePackage(byte[] dataBuffer, int offset)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);

        var structLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
        var head= PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_Header>(package.Data.Skip(offset).Take(structLength).ToArray());

        return ParsePackage(package, offset, head);
    }
    public static SMsgPropCreateEntity_SC_Container ParsePackage(Package package, int offset, SMsgPropCreateEntity_SC_Header sMsgPropCreateEntity_SC_Header)
    {
        var headLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
        var structLength = sMsgPropCreateEntity_SC_Header.wContextLen - 8;  //8 是ItemtypeID+ItemTemplateID的长度
        //int tempId = BitConverter.ToInt32(package.Data, offset+headLength);
        //int tempId1 = BitConverter.ToInt32(package.Data, offset + headLength+4);
        ////TraceUtil.Log("wContextLen:"+sMsgPropCreateEntity_SC_Header.wContextLen);

        return new SMsgPropCreateEntity_SC_Container()
        {
            m_sMsg_Header = sMsgPropCreateEntity_SC_Header,
            ItemtypeID= BitConverter.ToInt32(package.Data, offset+headLength),
            ItemTemplateID=BitConverter.ToInt32(package.Data,offset+headLength+4),
            ItemData = package.Data.Skip(offset + headLength + 4+4).Take(structLength).ToArray()
        };
    }

    public SMsgPropCreateEntity_SC_Header SMsg_Header
    {
        get
        {
            return this.m_sMsg_Header;
        }
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }


    public void UpdateValue(short index, int value)
    {
        //TraceUtil.Log("ItemData.Count()" + ItemData.Count());
        int offset = (index-2) * 4;
        var source = BitConverter.GetBytes(value);
        ItemData[offset] = source[0];
        ItemData[offset + 1] = source[1];
        ItemData[offset + 2] = source[2];
        ItemData[offset + 3] = source[3];        
    }
};
    /// <summary>
    /// 物品实体信息
    /// </summary>
    /// 

public struct EquipmentEntity : IEntityDataStruct//装备
    {
        public SMsgPropCreateEntity_SC_Header m_sMsg_Header;
        public int ITEM_FIELD_VISIBLE_LEFTTIME;//物品有效时间
        public int ITEM_FIELD_VISIBLE_BINDTYPE; //绑定类型
		public int ITEM_FIELD_VISIBLE_NEW;//是否为最新 0为最新 1为否
        public int ITEM_FIELD_VISIBLE_ACTIVE;//是否激活
        public int ITEM_FIELD_VISIBLE_SALENPC;//是否可出售NPC
		public int ITEM_FIELD_VISIBLE_COMM; //物品附加字段，装备为售价附加值
		public int ITEM_FIELD_VISIBLE_ISUSER;//是否帐号共用
        public int ITEM_FIELD_CONTAINER;//容器ID
        public int ITEM_FIELD_PLACE;//所在容器位置
        public int ITEM_FIELD_COUNT;//堆叠数量
        public int ITEM_FIELD_SURVIVETIME;//持续时间
        public int EQUIP_FIELD_QAULITY;//装备品质
        public int EQUIP_FIELD_EFFECTBASE0;//装备效果1
        public int EQUIP_FIELD_EFFECTBASE0_VALUE;//装备效果1的值
        public int EQUIP_FIELD_EFFECTBASE1;//装备效果2
        public int EQUIP_FIELD_EFFECTBASE1_VALUE;//装备效果2的值
        public int EQUIP_FIELD_EFFECTBASE2;//装备效果3
        public int EQUIP_FIELD_EFFECTBASE2_VALUE;//装备效果3的值
        public int EQUIP_FIELD_EFFECTBASE3;//装备效果4
        public int EQUIP_FIELD_EFFECTBASE3_VALUE;//装备效果4的值
        public int EQUIP_FIELD_EFFECTBASE4;//装备效果5
        public int EQUIP_FIELD_EFFECTBASE4_VALUE;//装备效果5的值
        public int EQUIP_FIELD_EFFECTBASE5;//装备效果6
        public int EQUIP_FIELD_EFFECTBASE5_VALUE;//装备效果6的值
        public int EQUIP_FIELD_EFFECTBASE6;//装备效果7
        public int EQUIP_FIELD_EFFECTBASE6_VALUE;//装备效果7的值
        public int EQUIP_FIELD_EFFECTBASE7;//装备效果8
        public int EQUIP_FIELD_EFFECTBASE7_VALUE;//装备效果8的值
        public int EQUIP_FIELD_EFFECTBASE8;//装备效果9
        public int EQUIP_FIELD_EFFECTBASE8_VALUE;//装备效果9的值
        public int EQUIP_FIELD_STRONGE_LEVEL;//强化等级
        public int EQUIP_FIELD_START_LEVEL;//星级强化
        public int EQUIP_FIELD_JEWEL_ID1;//宝石孔宝石ID1
        public int EQUIP_FIELD_JEWEL_ID1_ACTIVETYPE;//宝石孔ID1的激活类型
		public int EQUIP_FIELD_JEWEL_ID1_EXP1;	//经验
		public int EQUIP_FIELD_JEWEL_ID1_LEVEL1;//等级
        public int EQUIP_FIELD_JEWEL_ID2;//宝石孔宝石ID2
        public int EQUIP_FIELD_JEWEL_ID2_ACTIVETYPE;//宝石孔ID2的激活类型
		public int EQUIP_FIELD_JEWEL_ID2_EXP1;	//经验
		public int EQUIP_FIELD_JEWEL_ID2_LEVEL1;//等级
        public int EQUIP_FIELD_JEWEL_ID3;//宝石孔宝石ID3
        public int EQUIP_FIELD_JEWEL_ID3_ACTIVETYPE;//宝石孔ID3的激活类型
		public int EQUIP_FIELD_JEWEL_ID3_EXP1;	//经验
		public int EQUIP_FIELD_JEWEL_ID3_LEVEL1;//等级
        public int EQUIP_FIELD_JEWEL_ID4;//宝石孔宝石ID4
        public int EQUIP_FIELD_JEWEL_ID4_ACTIVETYPE;//宝石孔ID4的激活类型
		public int EQUIP_FIELD_JEWEL_ID4_EXP1;	//经验
		public int EQUIP_FIELD_JEWEL_ID4_LEVEL1;//等级
//        public int EQUIP_FIELD_SOPHISTICATION_LEVEL;//洗练等级
//        public int EQUIP_FIELD_SOPHISTICATION_PERCENT;//目前洗练进度，百分比值，如25代表25%
//        public int EQUIP_FIELD_SKILL_ID1;//被动技能ID
//        public int EQUIP_FIELD_SKILL_VALUE1;//被动技能等级
//        public int EQUIP_FIELD_SKILL_ID2;//被动技能
//        public int EQUIP_FIELD_SKILL_VALUE2;//被动技能等级
//        public int EQUIP_FIELD_SKILL_ID3;//被动技能
//        public int EQUIP_FIELD_SKILL_VALUE3;//被动技能等级


        public static EquipmentEntity ParsePackage(byte[] buffer, SMsgPropCreateEntity_SC_Header sMsgPropCreateEntity_SC_Header)
        {
            EquipmentEntity equipmentEntity = new EquipmentEntity();
            equipmentEntity.m_sMsg_Header = sMsgPropCreateEntity_SC_Header;
            int offset = 0;
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_VISIBLE_LEFTTIME);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_VISIBLE_BINDTYPE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_VISIBLE_NEW);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_VISIBLE_ACTIVE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_VISIBLE_SALENPC);
			offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_VISIBLE_COMM);
			offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_VISIBLE_ISUSER);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_CONTAINER);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_PLACE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_COUNT);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.ITEM_FIELD_SURVIVETIME);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_QAULITY);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE0);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE0_VALUE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE1);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE1_VALUE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE2);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE2_VALUE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE3);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE3_VALUE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE4);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE4_VALUE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE5);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE5_VALUE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE6);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE6_VALUE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE7);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE7_VALUE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE8);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_EFFECTBASE8_VALUE);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_START_LEVEL);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID1);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID1_ACTIVETYPE);
			offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID1_EXP1);
			offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID1_LEVEL1);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID2);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID2_ACTIVETYPE);
			offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID2_EXP1);
			offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID2_LEVEL1);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID3);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID3_ACTIVETYPE);
			offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID3_EXP1);
			offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID3_LEVEL1);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID4);
            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID4_ACTIVETYPE);
			offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID4_EXP1);
			offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_JEWEL_ID4_LEVEL1);

//            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_SOPHISTICATION_LEVEL);
//            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_SOPHISTICATION_PERCENT);
//            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_SKILL_ID1);
//            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_SKILL_VALUE1);
//            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_SKILL_ID2);
//            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_SKILL_VALUE2);
//            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_SKILL_ID3);
//            offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out equipmentEntity.EQUIP_FIELD_SKILL_VALUE3);
            return equipmentEntity;
        }

        public SMsgPropCreateEntity_SC_Header SMsg_Header
        {
            get { return this.m_sMsg_Header; }
        }

        private EquipmentEntity SetValue(int index, int value)
        {
            var bytes = PackageHelper.StructToBytes<EquipmentEntity>(this);

            int offset = (index - 1) * 4 +Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
            var source = BitConverter.GetBytes(value);
            bytes[offset] = source[0];
            bytes[offset + 1] = source[1];
            bytes[offset + 2] = source[2];
            bytes[offset + 3] = source[3];

            return PackageHelper.BytesToStuct<EquipmentEntity>(bytes);
        }

        public void UpdateValue(short index, int value)
        {
            //TraceUtil.Log("更新装备属性:" + index + "," + value);

            var equipment = this.SetValue(index, value);

            this.ITEM_FIELD_VISIBLE_BINDTYPE = equipment.ITEM_FIELD_VISIBLE_BINDTYPE;
            this.ITEM_FIELD_VISIBLE_NEW = equipment.ITEM_FIELD_VISIBLE_NEW;
            this.ITEM_FIELD_VISIBLE_ACTIVE = equipment.ITEM_FIELD_VISIBLE_ACTIVE;
            this.ITEM_FIELD_VISIBLE_SALENPC = equipment.ITEM_FIELD_VISIBLE_SALENPC;
			this.ITEM_FIELD_VISIBLE_COMM = equipment.ITEM_FIELD_VISIBLE_COMM;
            this.ITEM_FIELD_CONTAINER = equipment.ITEM_FIELD_CONTAINER;
            this.ITEM_FIELD_PLACE = equipment.ITEM_FIELD_PLACE;
            this.ITEM_FIELD_COUNT = equipment.ITEM_FIELD_COUNT;
            this.ITEM_FIELD_SURVIVETIME = equipment.ITEM_FIELD_SURVIVETIME;
            this.EQUIP_FIELD_QAULITY = equipment.EQUIP_FIELD_QAULITY;
            this.EQUIP_FIELD_EFFECTBASE0 = equipment.EQUIP_FIELD_EFFECTBASE0;
            this.EQUIP_FIELD_EFFECTBASE0_VALUE = equipment.EQUIP_FIELD_EFFECTBASE0_VALUE;
            this.EQUIP_FIELD_EFFECTBASE1 = equipment.EQUIP_FIELD_EFFECTBASE1;
            this.EQUIP_FIELD_EFFECTBASE1_VALUE = equipment.EQUIP_FIELD_EFFECTBASE1_VALUE;
            this.EQUIP_FIELD_EFFECTBASE2 = equipment.EQUIP_FIELD_EFFECTBASE2;
            this.EQUIP_FIELD_EFFECTBASE2_VALUE = equipment.EQUIP_FIELD_EFFECTBASE2_VALUE;
            this.EQUIP_FIELD_EFFECTBASE3 = equipment.EQUIP_FIELD_EFFECTBASE3;
            this.EQUIP_FIELD_EFFECTBASE3_VALUE = equipment.EQUIP_FIELD_EFFECTBASE3_VALUE;
            this.EQUIP_FIELD_EFFECTBASE4 = equipment.EQUIP_FIELD_EFFECTBASE4;
            this.EQUIP_FIELD_EFFECTBASE4_VALUE = equipment.EQUIP_FIELD_EFFECTBASE4_VALUE;
            this.EQUIP_FIELD_EFFECTBASE5 = equipment.EQUIP_FIELD_EFFECTBASE5;
            this.EQUIP_FIELD_EFFECTBASE5_VALUE = equipment.EQUIP_FIELD_EFFECTBASE5_VALUE;
            this.EQUIP_FIELD_EFFECTBASE6 = equipment.EQUIP_FIELD_EFFECTBASE6;
            this.EQUIP_FIELD_EFFECTBASE6_VALUE = equipment.EQUIP_FIELD_EFFECTBASE6_VALUE;
            this.EQUIP_FIELD_EFFECTBASE7 = equipment.EQUIP_FIELD_EFFECTBASE7;
            this.EQUIP_FIELD_EFFECTBASE7_VALUE = equipment.EQUIP_FIELD_EFFECTBASE7_VALUE;
            this.EQUIP_FIELD_EFFECTBASE8 = equipment.EQUIP_FIELD_EFFECTBASE8;
            this.EQUIP_FIELD_EFFECTBASE8_VALUE = equipment.EQUIP_FIELD_EFFECTBASE8_VALUE;
            this.EQUIP_FIELD_STRONGE_LEVEL = equipment.EQUIP_FIELD_STRONGE_LEVEL;
            this.EQUIP_FIELD_START_LEVEL = equipment.EQUIP_FIELD_START_LEVEL;
            this.EQUIP_FIELD_JEWEL_ID1 = equipment.EQUIP_FIELD_JEWEL_ID1;
            this.EQUIP_FIELD_JEWEL_ID1_ACTIVETYPE = equipment.EQUIP_FIELD_JEWEL_ID1_ACTIVETYPE;
			this.EQUIP_FIELD_JEWEL_ID1_EXP1 = equipment.EQUIP_FIELD_JEWEL_ID1_EXP1;
			this.EQUIP_FIELD_JEWEL_ID1_LEVEL1 = equipment.EQUIP_FIELD_JEWEL_ID1_LEVEL1;
            this.EQUIP_FIELD_JEWEL_ID2 = equipment.EQUIP_FIELD_JEWEL_ID2;
            this.EQUIP_FIELD_JEWEL_ID2_ACTIVETYPE = equipment.EQUIP_FIELD_JEWEL_ID2_ACTIVETYPE;
			this.EQUIP_FIELD_JEWEL_ID2_EXP1 = equipment.EQUIP_FIELD_JEWEL_ID2_EXP1;
			this.EQUIP_FIELD_JEWEL_ID2_LEVEL1 = equipment.EQUIP_FIELD_JEWEL_ID2_LEVEL1;
            this.EQUIP_FIELD_JEWEL_ID3 = equipment.EQUIP_FIELD_JEWEL_ID3;
            this.EQUIP_FIELD_JEWEL_ID3_ACTIVETYPE = equipment.EQUIP_FIELD_JEWEL_ID3_ACTIVETYPE;
			this.EQUIP_FIELD_JEWEL_ID3_EXP1 = equipment.EQUIP_FIELD_JEWEL_ID3_EXP1;
			this.EQUIP_FIELD_JEWEL_ID3_LEVEL1 = equipment.EQUIP_FIELD_JEWEL_ID3_LEVEL1;
            this.EQUIP_FIELD_JEWEL_ID4 = equipment.EQUIP_FIELD_JEWEL_ID4;
            this.EQUIP_FIELD_JEWEL_ID4_ACTIVETYPE = equipment.EQUIP_FIELD_JEWEL_ID4_ACTIVETYPE;
			this.EQUIP_FIELD_JEWEL_ID4_EXP1 = equipment.EQUIP_FIELD_JEWEL_ID4_EXP1;
			this.EQUIP_FIELD_JEWEL_ID4_LEVEL1 = equipment.EQUIP_FIELD_JEWEL_ID4_LEVEL1;
//            this.EQUIP_FIELD_SOPHISTICATION_LEVEL = equipment.EQUIP_FIELD_SOPHISTICATION_LEVEL;
//            this.EQUIP_FIELD_SOPHISTICATION_PERCENT = equipment.EQUIP_FIELD_SOPHISTICATION_PERCENT;
//            this.EQUIP_FIELD_SKILL_ID1 = equipment.EQUIP_FIELD_SKILL_ID1;
//            this.EQUIP_FIELD_SKILL_VALUE1 = equipment.EQUIP_FIELD_SKILL_VALUE1;
//            this.EQUIP_FIELD_SKILL_ID2 = equipment.EQUIP_FIELD_SKILL_ID2;
//            this.EQUIP_FIELD_SKILL_VALUE2 = equipment.EQUIP_FIELD_SKILL_VALUE2;
//            this.EQUIP_FIELD_SKILL_ID3 = equipment.EQUIP_FIELD_SKILL_ID3;
//            this.EQUIP_FIELD_SKILL_VALUE3 = equipment.EQUIP_FIELD_SKILL_VALUE3;
            //...
        }
    }

    public struct MedicamentEntity : IEntityDataStruct//药品
    {
        public SMsgPropCreateEntity_SC_Header m_sMsg_Header;
        public int ITEM_FIELD_VISIBLE_BINDTYPE; //绑定类型
        public int ITEM_FIELD_VISIBLE_NEW;//是否鉴定
        public int ITEM_FIELD_VISIBLE_ACTIVE;//是否激活
        public int ITEM_FIELD_VISIBLE_SALENPC;//是否可出售NPC
		public int ITEM_FIELD_VISIBLE_COMM; //物品附加字段，装备为售价附加值
        public int ITEM_FIELD_VISIBLE_ISUSER;//是否帐号共用
        public int ITEM_FIELD_CONTAINER;//容器ID
        public int ITEM_FIELD_PLACE;//所在容器位置
        public int ITEM_FIELD_COUNT;//堆叠数量
        public int ITEM_FIELD_SURVIVETIME;//持续时间`
        public int MEDICAMENT_FIELD_EFFECT1;//药品效果1
        public int MEDICAMENT_FIELD_EFFECT1_VALUE;//药品效果1的值
        public int MEDICAMENT_FIELD_EFFECT2;//药品效果2
        public int MEDICAMENT_FIELD_EFFECT2_VALUE;//药品效果2的值
        public int MEDICAMENT_FIELD_EFFECT3;//药品效果3
        public int MEDICAMENT_FIELD_EFFECT3_VALUE;//药品效果3的值
        public int MEDICAMENT_FIELD_EFFECT4;//药品效果4
        public int MEDICAMENT_FIELD_EFFECT4_VALUE;//药品效果4的值
        public int MEDICAMENT_FIELD_BELINKED;//药品是否被连接

        public static MedicamentEntity ParsePackage(byte[] buffer, SMsgPropCreateEntity_SC_Header sMsgPropCreateEntity_SC_Header)
        {
            //Debug.LogWarning("物品ID：" + sMsgPropCreateEntity_SC_Header.uidEntity + "包长：" + buffer.Length);
            MedicamentEntity goods = new MedicamentEntity()
            {
                m_sMsg_Header = sMsgPropCreateEntity_SC_Header,
                //ITEM_FIELD_VISIBLE_BINDTYPE = BitConverter.ToInt32(buffer, 0),
                //ITEM_FIELD_VISIBLE_IDENTIFY = BitConverter.ToInt32(buffer, 4),
                //ITEM_FIELD_VISIBLE_ACTIVE = BitConverter.ToInt32(buffer, 8),
                //ITEM_FIELD_VISIBLE_SALENPC = BitConverter.ToInt32(buffer, 12),
                //ITEM_FIELD_CONTAINER = BitConverter.ToInt32(buffer, 16),
                //ITEM_FIELD_PLACE = BitConverter.ToInt32(buffer, 20),
                //ITEM_FIELD_COUNT = BitConverter.ToInt32(buffer, 24),
                //ITEM_FIELD_SURVIVETIME = BitConverter.ToInt32(buffer, 28),
                //MEDICAMENT_FIELD_EFFECT1 = BitConverter.ToInt32(buffer, 32),
                //MEDICAMENT_FIELD_EFFECT1_VALUE = BitConverter.ToInt32(buffer, 36),
                //MEDICAMENT_FIELD_EFFECT2 = BitConverter.ToInt32(buffer, 40),
                //MEDICAMENT_FIELD_EFFECT2_VALUE = BitConverter.ToInt32(buffer, 44),
                //MEDICAMENT_FIELD_EFFECT3 = BitConverter.ToInt32(buffer, 48),
                //MEDICAMENT_FIELD_EFFECT3_VALUE = BitConverter.ToInt32(buffer, 52),
                //MEDICAMENT_FIELD_EFFECT4 = BitConverter.ToInt32(buffer, 56),
                //MEDICAMENT_FIELD_EFFECT4_VALUE = BitConverter.ToInt32(buffer, 60),
                //MEDICAMENT_FIELD_BELINKED = BitConverter.ToInt32(buffer,60),
            };
            int of = 0;
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.ITEM_FIELD_VISIBLE_BINDTYPE);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.ITEM_FIELD_VISIBLE_NEW);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.ITEM_FIELD_VISIBLE_ACTIVE);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.ITEM_FIELD_VISIBLE_SALENPC);
		    of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.ITEM_FIELD_VISIBLE_COMM);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.ITEM_FIELD_VISIBLE_ISUSER);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.ITEM_FIELD_CONTAINER);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.ITEM_FIELD_PLACE);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.ITEM_FIELD_COUNT);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.ITEM_FIELD_SURVIVETIME);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.MEDICAMENT_FIELD_EFFECT1);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.MEDICAMENT_FIELD_EFFECT1_VALUE);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.MEDICAMENT_FIELD_EFFECT2);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.MEDICAMENT_FIELD_EFFECT2_VALUE);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.MEDICAMENT_FIELD_EFFECT3);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.MEDICAMENT_FIELD_EFFECT3_VALUE);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.MEDICAMENT_FIELD_EFFECT4);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.MEDICAMENT_FIELD_EFFECT4_VALUE);
            of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out goods.MEDICAMENT_FIELD_BELINKED);

            return goods;
        }


        public SMsgPropCreateEntity_SC_Header SMsg_Header
        {
            get { return this.m_sMsg_Header; }
        }

        private MedicamentEntity SetValue(int index, int value)
        {
		TraceUtil.Log(SystemModel.Jiang,"Update:Index:"+index+",Value:"+value);
            var bytes = PackageHelper.StructToBytes<MedicamentEntity>(this);

            int offset = (index - 2) * 4 + Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
            var source = BitConverter.GetBytes(value);
            bytes[offset] = source[0];
            bytes[offset + 1] = source[1];
            bytes[offset + 2] = source[2];
            bytes[offset + 3] = source[3];

            return PackageHelper.BytesToStuct<MedicamentEntity>(bytes);
        }

        public void UpdateValue(short index, int value)
        {
            //TraceUtil.Log("更新药品属性");
            int offset = (index - 2) * 4;
            var equipment = this.SetValue(index, value);

            this.ITEM_FIELD_VISIBLE_BINDTYPE = equipment.ITEM_FIELD_VISIBLE_BINDTYPE;
            this.ITEM_FIELD_VISIBLE_NEW = equipment.ITEM_FIELD_VISIBLE_NEW;
            this.ITEM_FIELD_VISIBLE_ACTIVE = equipment.ITEM_FIELD_VISIBLE_ACTIVE;
            this.ITEM_FIELD_VISIBLE_SALENPC = equipment.ITEM_FIELD_VISIBLE_SALENPC;
		    this.ITEM_FIELD_VISIBLE_COMM = equipment.ITEM_FIELD_VISIBLE_COMM;
            this.ITEM_FIELD_VISIBLE_ISUSER = equipment.ITEM_FIELD_VISIBLE_ISUSER;
            this.ITEM_FIELD_CONTAINER = equipment.ITEM_FIELD_CONTAINER;
            this.ITEM_FIELD_PLACE = equipment.ITEM_FIELD_PLACE;
            this.ITEM_FIELD_COUNT = equipment.ITEM_FIELD_COUNT;
            this.ITEM_FIELD_SURVIVETIME = equipment.ITEM_FIELD_SURVIVETIME;
            this.MEDICAMENT_FIELD_EFFECT1 = equipment.MEDICAMENT_FIELD_EFFECT1;
            this.MEDICAMENT_FIELD_EFFECT1_VALUE = equipment.MEDICAMENT_FIELD_EFFECT1_VALUE;
            this.MEDICAMENT_FIELD_EFFECT2 = equipment.MEDICAMENT_FIELD_EFFECT2;
            this.MEDICAMENT_FIELD_EFFECT2_VALUE = equipment.MEDICAMENT_FIELD_EFFECT2_VALUE;
            this.MEDICAMENT_FIELD_EFFECT3 = equipment.MEDICAMENT_FIELD_EFFECT3;
            this.MEDICAMENT_FIELD_EFFECT3_VALUE = equipment.MEDICAMENT_FIELD_EFFECT3_VALUE;
            this.MEDICAMENT_FIELD_EFFECT4 = equipment.MEDICAMENT_FIELD_EFFECT4;
            this.MEDICAMENT_FIELD_EFFECT4_VALUE = equipment.MEDICAMENT_FIELD_EFFECT4_VALUE;
            this.MEDICAMENT_FIELD_BELINKED = equipment.MEDICAMENT_FIELD_BELINKED;
            //...
        }
    }

    public struct Materiel : IEntityDataStruct//材料物品
    {
        public SMsgPropCreateEntity_SC_Header m_sMsg_Header;
        public int ITEM_FIELD_VISIBLE_BINDTYPE; //绑定类型
        public int ITEM_FIELD_VISIBLE_NEW;//是否鉴定
        public int ITEM_FIELD_VISIBLE_ACTIVE;//是否激活
        public int ITEM_FIELD_VISIBLE_SALENPC;//是否可出售NPC
	    public int ITEM_FIELD_VISIBLE_COMM;//
        public int ITEM_FIELD_VISIBLE_ISUSER;
        public int ITEM_FIELD_CONTAINER;//容器ID
        public int ITEM_FIELD_PLACE;//所在容器位置
        public int ITEM_FIELD_COUNT;//堆叠数量
        public int ITEM_FIELD_SURVIVETIME;//持续时间

		#region 宝石，器魂新增
		public int ESTORE_FIELD_EXP;//堆叠数量
		public int ESTORE_FIELD_LEVEL;//持续时间
		#endregion

        public static Materiel ParsePackage(byte[] buffer, SMsgPropCreateEntity_SC_Header sMsgPropCreateEntity_SC_Header)
        {
           int of=0;
            var mat= new Materiel()
            {
            m_sMsg_Header = sMsgPropCreateEntity_SC_Header,
            };
          
               of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ITEM_FIELD_VISIBLE_BINDTYPE);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ITEM_FIELD_VISIBLE_NEW);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ITEM_FIELD_VISIBLE_ACTIVE);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ITEM_FIELD_VISIBLE_SALENPC);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ITEM_FIELD_VISIBLE_COMM);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ITEM_FIELD_VISIBLE_ISUSER);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ITEM_FIELD_CONTAINER);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ITEM_FIELD_PLACE);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ITEM_FIELD_COUNT);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ITEM_FIELD_SURVIVETIME);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ESTORE_FIELD_EXP);
            of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out mat.ESTORE_FIELD_LEVEL);
           

		    return mat;
        }

        public SMsgPropCreateEntity_SC_Header SMsg_Header
        {
            get { return this.m_sMsg_Header; }
        }

        private Materiel SetValue(int index, int value)
        {
            var bytes = PackageHelper.StructToBytes<Materiel>(this);

            int offset = (index - 2) * 4 + Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
            var source = BitConverter.GetBytes(value);
            bytes[offset] = source[0];
            bytes[offset + 1] = source[1];
            bytes[offset + 2] = source[2];
            bytes[offset + 3] = source[3];

            return PackageHelper.BytesToStuct<Materiel>(bytes);
        }

        public void UpdateValue(short index, int value)
        {
            //TraceUtil.Log("更新材料属性");
            int offset = (index - 2) * 4;
            var equipment = this.SetValue(index, value);

            this.ITEM_FIELD_VISIBLE_BINDTYPE = equipment.ITEM_FIELD_VISIBLE_BINDTYPE;
            this.ITEM_FIELD_VISIBLE_NEW = equipment.ITEM_FIELD_VISIBLE_NEW;
            this.ITEM_FIELD_VISIBLE_ACTIVE = equipment.ITEM_FIELD_VISIBLE_ACTIVE;
            this.ITEM_FIELD_VISIBLE_SALENPC = equipment.ITEM_FIELD_VISIBLE_SALENPC;
		    this.ITEM_FIELD_VISIBLE_COMM = equipment.ITEM_FIELD_VISIBLE_COMM;
            this.ITEM_FIELD_VISIBLE_ISUSER = equipment.ITEM_FIELD_VISIBLE_ISUSER;
            this.ITEM_FIELD_CONTAINER = equipment.ITEM_FIELD_CONTAINER;
            this.ITEM_FIELD_PLACE = equipment.ITEM_FIELD_PLACE;
            this.ITEM_FIELD_COUNT = equipment.ITEM_FIELD_COUNT;
            this.ITEM_FIELD_SURVIVETIME = equipment.ITEM_FIELD_SURVIVETIME;
		this.ESTORE_FIELD_EXP = equipment.ESTORE_FIELD_EXP;
		this.ESTORE_FIELD_LEVEL = equipment.ESTORE_FIELD_LEVEL;
            //...
        }
    }

    //public struct ItemFieldEntity 
    //{
    //    public SMsgPropCreateEntity_SC_Header m_sMsg_Header;
    //    public int ITEM_FIELD_VISIBLE_BINDTYPE; //绑定类型
    //    public int ITEM_FIELD_VISIBLE_IDENTIFY;//是否鉴定
    //    public int ITEM_FIELD_VISIBLE_ACTIVE;//是否激活
    //    public int ITEM_FIELD_VISIBLE_SALENPC;//是否可出售NPC
    //    public int ITEM_FIELD_CONTAINER;//容器ID
    //    public int ITEM_FIELD_PLACE;//所在容器位置
    //    public int ITEM_FIELD_DURABILITY;//当前耐久
    //    public int ITEM_FIELD_MAXDURABILITY;//最大耐久
    //    public int ITEM_FIELD_COUNT;//堆叠数量
    //    public int ITEM_FIELD_SURVIVETIME;//持续时间`


    //    public static ItemFieldEntity ParsePackage(byte[] buffer,SMsgPropCreateEntity_SC_Header sMsgPropCreateEntity_SC_Header)
    //    {
    //        return new ItemFieldEntity()
    //        {
    //            m_sMsg_Header = sMsgPropCreateEntity_SC_Header,
    //            ITEM_FIELD_VISIBLE_BINDTYPE = BitConverter.ToInt32(buffer, 0),
    //            ITEM_FIELD_VISIBLE_IDENTIFY = BitConverter.ToInt32(buffer, 4),
    //            ITEM_FIELD_VISIBLE_ACTIVE = BitConverter.ToInt32(buffer, 4 + 4),
    //            ITEM_FIELD_VISIBLE_SALENPC = BitConverter.ToInt32(buffer, 4 + 4 + 4),
    //            ITEM_FIELD_CONTAINER = BitConverter.ToInt32(buffer, 4 + 4 + 4 + 4),
    //            ITEM_FIELD_PLACE = BitConverter.ToInt32(buffer, 4 + 4 + 4 + 4 + 4),
    //            ITEM_FIELD_DURABILITY = BitConverter.ToInt32(buffer, 4 + 4 + 4 + 4 + 4 + 4),
    //            ITEM_FIELD_MAXDURABILITY = BitConverter.ToInt32(buffer, 4 + 4 + 4 + 4 + 4 + 4 + 4),
    //            ITEM_FIELD_COUNT = BitConverter.ToInt32(buffer, 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4),
    //            ITEM_FIELD_SURVIVETIME = BitConverter.ToInt32(buffer, 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4),
    //        };
    //    }
    //}
    ///// <summary>
    ///// 装备实体信息
    ///// </summary>
    //public struct EquidFieldEntity
    //{
    //    public SMsgPropCreateEntity_SC_Header m_sMsg_Header;
    //    public int EQUIP_FIELD_QAULITY;//装备品质
    //    public int EQUIP_FIELD_EFFECTBASE0;//装备效果1
    //    public int EQUIP_FIELD_EFFECTBASE0_VALUE;//装备效果1的值
    //    public int EQUIP_FIELD_EFFECTBASE1;//装备效果2
    //    public int EQUIP_FIELD_EFFECTBASE1_VALUE;//装备效果2的值
    //    public int EQUIP_FIELD_EFFECTBASE2;//装备效果3
    //    public int EQUIP_FIELD_EFFECTBASE2_VALUE;//装备效果3的值
    //    public int EQUIP_FIELD_EFFECTBASE3;//装备效果4
    //    public int EQUIP_FIELD_EFFECTBASE3_VALUE;//装备效果4的值
    //    public int EQUIP_FIELD_EFFECTBASE4;//装备效果5
    //    public int EQUIP_FIELD_EFFECTBASE4_VALUE;//装备效果5的值
    //    public int EQUIP_FIELD_EFFECTBASE5;//装备效果6
    //    public int EQUIP_FIELD_EFFECTBASE5_VALUE;//装备效果6的值
    //    public int EQUIP_FIELD_EFFECTBASE6;//装备效果7
    //    public int EQUIP_FIELD_EFFECTBASE6_VALUE;//装备效果7的值
    //    public int EQUIP_FIELD_EFFECTBASE7;//装备效果8
    //    public int EQUIP_FIELD_EFFECTBASE7_VALUE;//装备效果8的值
    //    public int EQUIP_FIELD_EFFECTBASE8;//装备效果9
    //    public int EQUIP_FIELD_EFFECTBASE8_VALUE;//装备效果9的值
    //    public int EQUIP_FIELD_STRONGE_LEVEL;//强化等级
    //    public int EQUIP_FIELD_JEWEL_ID1;//宝石孔宝石ID1
    //    public int EQUIP_FIELD_JEWEL_ID1_ACTIVETYPE;//宝石孔ID1的激活类型
    //    public int EQUIP_FIELD_JEWEL_ID2;//宝石孔宝石ID2
    //    public int EQUIP_FIELD_JEWEL_ID2_ACTIVETYPE;//宝石孔ID2的激活类型
    //    public int EQUIP_FIELD_JEWEL_ID3;//宝石孔宝石ID3
    //    public int EQUIP_FIELD_JEWEL_ID3_ACTIVETYPE;//宝石孔ID3的激活类型
    //    public int EQUIP_FIELD_JEWEL_ID4;//宝石孔宝石ID4
    //    public int EQUIP_FIELD_JEWEL_ID4_ACTIVETYPE;//宝石孔ID4的激活类型


    //    public static EquidFieldEntity ParsePackage(byte[] buffer, SMsgPropCreateEntity_SC_Header sMsgPropCreateEntity_SC_Header)
    //    {
    //        return new EquidFieldEntity()
    //        {
    //            m_sMsg_Header = sMsgPropCreateEntity_SC_Header,
    //            EQUIP_FIELD_QAULITY = BitConverter.ToInt32(buffer, 0),
    //            EQUIP_FIELD_EFFECTBASE0 = BitConverter.ToInt32(buffer, 4),
    //            EQUIP_FIELD_EFFECTBASE0_VALUE = BitConverter.ToInt32(buffer, 8),
    //            EQUIP_FIELD_EFFECTBASE1 = BitConverter.ToInt32(buffer,12),
    //            EQUIP_FIELD_EFFECTBASE1_VALUE = BitConverter.ToInt32(buffer, 16),
    //            EQUIP_FIELD_EFFECTBASE2 = BitConverter.ToInt32(buffer, 20),
    //            EQUIP_FIELD_EFFECTBASE2_VALUE = BitConverter.ToInt32(buffer,24),
    //            EQUIP_FIELD_EFFECTBASE3 = BitConverter.ToInt32(buffer, 28),
    //            EQUIP_FIELD_EFFECTBASE3_VALUE = BitConverter.ToInt32(buffer, 32),
    //            EQUIP_FIELD_EFFECTBASE4 = BitConverter.ToInt32(buffer, 36),
    //            EQUIP_FIELD_EFFECTBASE4_VALUE = BitConverter.ToInt32(buffer, 40),
    //            EQUIP_FIELD_EFFECTBASE5 = BitConverter.ToInt32(buffer, 44),
    //            EQUIP_FIELD_EFFECTBASE5_VALUE = BitConverter.ToInt32(buffer, 48),
    //            EQUIP_FIELD_EFFECTBASE6 = BitConverter.ToInt32(buffer, 52),
    //            EQUIP_FIELD_EFFECTBASE6_VALUE = BitConverter.ToInt32(buffer, 56),
    //            EQUIP_FIELD_EFFECTBASE7 = BitConverter.ToInt32(buffer, 60),
    //            EQUIP_FIELD_EFFECTBASE7_VALUE = BitConverter.ToInt32(buffer, 64),
    //            EQUIP_FIELD_EFFECTBASE8 = BitConverter.ToInt32(buffer, 68),
    //            EQUIP_FIELD_EFFECTBASE8_VALUE = BitConverter.ToInt32(buffer, 72),
    //            EQUIP_FIELD_STRONGE_LEVEL = BitConverter.ToInt32(buffer,76),
    //            EQUIP_FIELD_JEWEL_ID1 = BitConverter.ToInt32(buffer,80),
    //            EQUIP_FIELD_JEWEL_ID1_ACTIVETYPE = BitConverter.ToInt32(buffer, 84),
    //            EQUIP_FIELD_JEWEL_ID2 = BitConverter.ToInt32(buffer, 88),
    //            EQUIP_FIELD_JEWEL_ID2_ACTIVETYPE = BitConverter.ToInt32(buffer, 92),
    //            EQUIP_FIELD_JEWEL_ID3 = BitConverter.ToInt32(buffer, 96),
    //            EQUIP_FIELD_JEWEL_ID3_ACTIVETYPE = BitConverter.ToInt32(buffer, 100),
    //            EQUIP_FIELD_JEWEL_ID4 = BitConverter.ToInt32(buffer, 104),
    //            EQUIP_FIELD_JEWEL_ID4_ACTIVETYPE = BitConverter.ToInt32(buffer, 108),

    //        };
    //    }
    //}
    ///// <summary>
    ///// 药瓶实体信息
    ///// </summary>
    //public struct MedicamentFieldEntity
    //{
    //    public SMsgPropCreateEntity_SC_Header m_sMsg_Header;
    //    public int MEDICAMENT_FIELD_EFFECT1;//药品效果1
    //    public int MEDICAMENT_FIELD_EFFECT1_VALUE;//药品效果1的值
    //    public int MEDICAMENT_FIELD_EFFECT2;//药品效果2
    //    public int MEDICAMENT_FIELD_EFFECT2_VALUE;//药品效果2的值
    //    public int MEDICAMENT_FIELD_EFFECT3;//药品效果3
    //    public int MEDICAMENT_FIELD_EFFECT3_VALUE;//药品效果3的值
    //    public int MEDICAMENT_FIELD_EFFECT4;//药品效果4
    //    public int MEDICAMENT_FIELD_EFFECT4_VALUE;//药品效果4的值
    //    public int MEDICAMENT_FIELD_PROPADD1;//药品附加属性1
    //    public int MEDICAMENT_FIELD_PROPADD1_VALUE;//药品附加属性的值1
    //    public int MEDICAMENT_FIELD_PROPADD2;//药品附加属性2
    //    public int MEDICAMENT_FIELD_PROPADD2_VALUE;//药品附加属性的值2


    //    public static MedicamentFieldEntity ParsePackage(byte[] buffer, SMsgPropCreateEntity_SC_Header sMsgPropCreateEntity_SC_Header)
    //    {
    //        return new MedicamentFieldEntity()
    //        {
    //            m_sMsg_Header = sMsgPropCreateEntity_SC_Header,
    //            MEDICAMENT_FIELD_EFFECT1 = BitConverter.ToInt32(buffer, 0),
    //            MEDICAMENT_FIELD_EFFECT1_VALUE = BitConverter.ToInt32(buffer, 4),
    //            MEDICAMENT_FIELD_EFFECT2 = BitConverter.ToInt32(buffer, 8),
    //            MEDICAMENT_FIELD_EFFECT2_VALUE = BitConverter.ToInt32(buffer, 12),
    //            MEDICAMENT_FIELD_EFFECT3 = BitConverter.ToInt32(buffer, 16),
    //            MEDICAMENT_FIELD_EFFECT3_VALUE = BitConverter.ToInt32(buffer, 20),
    //            MEDICAMENT_FIELD_EFFECT4 = BitConverter.ToInt32(buffer, 24),
    //            MEDICAMENT_FIELD_EFFECT4_VALUE = BitConverter.ToInt32(buffer, 28),
    //            MEDICAMENT_FIELD_PROPADD1 = BitConverter.ToInt32(buffer, 32),
    //            MEDICAMENT_FIELD_PROPADD1_VALUE = BitConverter.ToInt32(buffer, 36),
    //            MEDICAMENT_FIELD_PROPADD2 = BitConverter.ToInt32(buffer, 40),
    //            MEDICAMENT_FIELD_PROPADD2_VALUE = BitConverter.ToInt32(buffer, 44),

    //        };
    //    }
    //}