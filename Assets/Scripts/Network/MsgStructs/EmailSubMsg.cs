using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

public enum emEMAIL_STATE_TYPE
{
    EMAIL_NONE_STATE_TYPE,                      //一般状态
    EMAIL_NOREAD_STATE_TYPE,                    //有未读邮件
    EMAIL_FULL_STATE_TYPE,                      //邮件箱满的状态
};

public enum emEMAIL_UPDATE_TYPE
{
    EMAIL_NONE_UPDATE_TYPE,
    EMAIL_READ_UPDATE_TYPE  = 1,            //更新邮件为已读状态
    EMAIL_GETGOODS_UPDATE_TYPE = 2,         //获取邮件的附件
};

public enum emEMAIL_BENEW_TYPE
{
    EMAIL_NONE_TYPE,                        //邮件不是最新邮件
    EMAIL_BENEW_TYPE,                       //邮件为最新邮件
};

//附件物品类型
public enum emEMAIL_EXTRA_TYPE
{
	/// <summary>
	/// 无附件
	/// </summary>
    EMAIL_NONE_EXTRA_TYPE,
	/// <summary>
	/// 元宝类型
	/// </summary>
    EMAIL_GOLD_EXTRA_TYPE,   
	/// <summary>
	///    铜币类型.
	/// </summary>
    EMAIL_HOLDMONEY_EXTRA_TYPE,     
	/// <summary>
	/// 物品类型
	/// </summary>
    EMAIL_GOODS_EXTRA_TYPE,                 
};


//每个邮件信息结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailSendUint
{
    public byte        byBeNew;                                            //邮件是否为新添加
    public byte        bySendFlag;                                         //邮件是否下发
    public byte        byState;                                            //邮件状态，是否已读
    /// <summary>
    /// 0s是玩家邮件，1是系统邮件
    /// </summary>
    public byte        byIsSystem;                                         //是否为系统邮件
    public ushort        wEmailType;                                         //邮件类型，系统邮件使用
    public Int64       llMailID;                                              //邮件ID
    public byte        byEmailPage;                                       //邮件存放在哪一页
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[]        szSendActorName;                //发送者角色名
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[]        szTitle;                             //邮件标题
    public uint       dwExpireTime;                                       //过期时间点
    public byte        byGoodsType;                                        //物品类型，如果是元宝活力等数据，则物品个数为相应的数目
    public uint       dwGoodsID;                                          //物品ID
    public uint       dwGoodsNum;                                         //物品个数


    public static SEmailSendUint ParsePackage(byte[] dataBuffer, ref int offset)
    {
        SEmailSendUint sEmailSendUint = new SEmailSendUint();
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.byBeNew, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.bySendFlag, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.byState, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.byIsSystem, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.wEmailType, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.llMailID, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.byEmailPage, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.szSendActorName, 19, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.szTitle, 19, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.dwExpireTime, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.byGoodsType, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.dwGoodsID, offset);
        offset += PackageHelper.ReadData(dataBuffer, out sEmailSendUint.dwGoodsNum, offset);
        
        return sEmailSendUint;
    }
};

//MSG_EMAIL_NOREAD_EMAIL
//登录下发角色邮件箱状态，是否有未读邮件，是否满
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SLoginEmailState_SC
{
    public  byte    byEmailState;

    public static SLoginEmailState_SC ParsePackage(byte[] dataBuffer)
    {
        SLoginEmailState_SC sLoginEmailState_SC = PackageHelper.BytesToStuct<SLoginEmailState_SC>(dataBuffer);
        return sLoginEmailState_SC;
    }
};


//MSG_EMAIL_OPEN_UI_TYPE
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailOpenUI_CS
{
    public uint dwActorID;                                    //角色ID

    public Package GeneratePackage()
    {
        Package Pak = new Package();
        Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_EMAIL,EmailDefineManager.MSG_EMAIL_OPEN_UI_TYPE);
        Pak.Data = PackageHelper.StructToBytes<SEmailOpenUI_CS>(this);
        return Pak; 
    }
};

//打开邮件UI
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailOpenUI_SC
{
    public uint   dwActorID;                          //角色ID
    public uint   dwEmailNum;                         //邮件封数
    //多个SEmailSendUint
    public List<SEmailSendUint> mailList;

    public static SEmailOpenUI_SC ParsePackage(byte[] dataBuffer)
    {
        SEmailOpenUI_SC sEmailOpenUI_SC = new SEmailOpenUI_SC();
        int offset = 0;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailOpenUI_SC.dwActorID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailOpenUI_SC.dwEmailNum);
        sEmailOpenUI_SC.mailList = new List<SEmailSendUint>();
        for(int i = 0; i < sEmailOpenUI_SC.dwEmailNum ; i++)
        {
            SEmailSendUint sEmailSendUint = SEmailSendUint.ParsePackage(dataBuffer, ref offset);
            sEmailOpenUI_SC.mailList.Add(sEmailSendUint);
        }
        return sEmailOpenUI_SC;

    }
};

//MSG_EMAIL_SEND
//写邮件
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailWrite_CS
{
    public uint       dwActorID;                                          //角色ID
    public uint       dwRecvActorID;                               //接收角色ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[]        szTitle;                             //邮件标题
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 160)]
    public byte[]        szContext;                   //邮件内容
    public byte        byHaveFlag;                                         //是否有附件
    public byte        byGoodsType;                                        //物品类型，如果是元宝活力等数据，则物品个数为相应的数目
    public uint       dwGoodsID;                                          //物品ID
    public uint       dwGoodsNum;                                         //物品个数

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_EMAIL, EmailDefineManager.MSG_EMAIL_SEND);
        pkg.Data = BitConverter.GetBytes(dwActorID)
            .Concat(BitConverter.GetBytes(dwRecvActorID))
                .Concat(PackageHelper.GetByteFromArray(this.szTitle, 19))
                .Concat(PackageHelper.GetByteFromArray(this.szContext, 160))           
                .Concat(new byte[] { this.byHaveFlag })
                .Concat(new byte[] { this.byGoodsType })
                .Concat(BitConverter.GetBytes(this.dwGoodsID))
                .Concat(BitConverter.GetBytes(this.dwGoodsNum)).ToArray();
        
        return pkg;
    }
};

//写邮件应答
//只成功不失败
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailWrite_SC
{
    public byte    bySucess;                                                       //是否发送成功

    public static SEmailWrite_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SEmailWrite_SC>(dataBuffer);
    }
};

//MSG_EMAIL_UPDATE
//获取附件时调用
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailUpdate_CS
{
    public byte    byUpdateType;                                       //更新类型，1为更新已读，2更新为获取附件
    public uint   dwActorID;                                          //角色ID
	public Int64 dwEmailID;  //邮件ID
    public byte byEmailPage;
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_EMAIL, EmailDefineManager.MSG_EMAIL_UPDATE);
        pkg.Data = PackageHelper.StructToBytes<SEmailUpdate_CS>(this);

        return pkg;
    }
};


[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailUpdate_SC
{
    public byte   SEmailUpdate;
    public uint   dwActorID;                                          //角色ID
	public Int64 dwEmailID;                                            //邮件ID

    public static SEmailUpdate_SC ParsePackage(byte[] dataBuffer)
    {

        return PackageHelper.BytesToStuct<SEmailUpdate_SC>(dataBuffer);
    }

};


//MSG_EAMIL_DEL
//删除邮件
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailDel_CS
{
    public uint       dwActorID;                                          //角色ID
    public Int64    llEmailID;                                          //邮件ID
    public byte     byEmailPage;
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_EMAIL, EmailDefineManager.MSG_EAMIL_DEL);
        pkg.Data = PackageHelper.StructToBytes<SEmailDel_CS>(this);
        
        return pkg;
    }
};

//删除邮件成功
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailDel_SC
{
	public uint dwActorID;                                //角色ID
	public uint dwEmailNum;
	//EmailID 列表
	public List<Int64> mailIdList;
	
	public static SEmailDel_SC ParsePackage(byte[] dataBuffer)
	{
		
		SEmailDel_SC sEmailDel_SC = new SEmailDel_SC();
		int offset = 0;
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailDel_SC.dwActorID);
		offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailDel_SC.dwEmailNum);
		
		sEmailDel_SC.mailIdList = new List<Int64>();
		for(int i = 0; i < sEmailDel_SC.dwEmailNum; i++)
		{
			Int64 mailId = 0;
			offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out mailId);
			sEmailDel_SC.mailIdList.Add(mailId);
		}
		
		return sEmailDel_SC;
	}
};

//一键获取邮件请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailGetAllGoods_CS
{
    public uint dwActorID;                                    //角色ID

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_EMAIL, EmailDefineManager.MSG_EMAIL_ALLGOODSGET);
        pkg.Data = PackageHelper.StructToBytes<SEmailGetAllGoods_CS>(this);
        
        return pkg;
    }
};

//一键获取应答
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailGetAllGoods_SC
{
    public uint dwActorID;                                    //角色ID
    public uint dwEmailNum;
    //EmailID 列表
    public List<Int64> mailIdList;

    public static SEmailGetAllGoods_SC ParsePackage(byte[] dataBuffer)
    {
        SEmailGetAllGoods_SC sEmailGetAllGoods_SC = new SEmailGetAllGoods_SC();
        int offset = 0;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailGetAllGoods_SC.dwActorID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailGetAllGoods_SC.dwEmailNum);

        sEmailGetAllGoods_SC.mailIdList = new List<Int64>();
        for(int i = 0; i < sEmailGetAllGoods_SC.dwEmailNum; i++)
        {
            Int64 mailId = 0;
            offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out mailId);
            sEmailGetAllGoods_SC.mailIdList.Add(mailId);
        }

        return sEmailGetAllGoods_SC;
    }
};

//一键获取应答 每操作一次，发送一个应答SEmailUpdate_SC

//一键删除邮件操作
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailDelAll_CS
{
    public uint dwActorID;                                //角色ID
	public byte Type;//系统邮件还是用户邮件
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_EMAIL, EmailDefineManager.MSG_EMAIL_ALLDEL);
        pkg.Data = PackageHelper.StructToBytes<SEmailDelAll_CS>(this);
        
        return pkg;
    }
};

//一键删除应答
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailDelAll_SC
{
    public uint dwActorID;                                //角色ID
    public uint dwEmailNum;
    //EmailID 列表
    public List<Int64> mailIdList;
    
    public static SEmailDelAll_SC ParsePackage(byte[] dataBuffer)
    {
        
        SEmailDelAll_SC sEmailDelAll_SC = new SEmailDelAll_SC();
        int offset = 0;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailDelAll_SC.dwActorID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailDelAll_SC.dwEmailNum);
        
        sEmailDelAll_SC.mailIdList = new List<Int64>();
        for(int i = 0; i < sEmailDelAll_SC.dwEmailNum; i++)
        {
            Int64 mailId = 0;
            offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out mailId);
            sEmailDelAll_SC.mailIdList.Add(mailId);
        }
        
        return sEmailDelAll_SC;
    }



};

//MSG_EMAIL_READ
//打开邮件阅读
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailRead_CS
{
    public uint       dwActorID;
    public Int64    llEmailID;
    public byte     byEmailPage;
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_EMAIL, EmailDefineManager.MSG_EMAIL_READ);
        pkg.Data = PackageHelper.StructToBytes<SEmailRead_CS>(this);
        
        return pkg;
    }
    
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEmailRead_SC
{
    public uint       dwActorID;
    public Int64    llEmailID;
    public byte      byEmailPage;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 160)]
    public byte[]        szEmailContext;
    
    public static SEmailRead_SC ParsePackage(byte[] dataBuffer)
    {
        
        SEmailRead_SC sEmailRead_SC = new SEmailRead_SC();
        int offset = 0;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailRead_SC.dwActorID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailRead_SC.llEmailID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailRead_SC.byEmailPage);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sEmailRead_SC.szEmailContext, 160);
        
        return sEmailRead_SC;
    }
};




