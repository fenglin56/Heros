using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Linq;


//下发好友列表/附近玩家列表,包含如下头和多个SMsgRecvAnswerFriends_SC结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGetActorListHead:INotifyArgs
{
	public uint dwActorID;		//角色ActorID;
	public byte byFriendMaxNum;		//好友最大数 
	public byte	dwFriendNum;		//好友个数
	//.......................好友现场
    public SMsgRecvAnswerFriends_SC[] sMsgRecvAnswerFriends_SC;

    public static SMsgGetActorListHead ParsePackage(byte[] dataBuffer)
    {
        SMsgGetActorListHead sMsgGetActorListHead = new SMsgGetActorListHead();

        sMsgGetActorListHead.dwActorID = BitConverter.ToUInt32(dataBuffer, 0);
		sMsgGetActorListHead.byFriendMaxNum = dataBuffer[4];
        sMsgGetActorListHead.dwFriendNum = dataBuffer[5];
        sMsgGetActorListHead.sMsgRecvAnswerFriends_SC = new SMsgRecvAnswerFriends_SC[sMsgGetActorListHead.dwFriendNum];

        int offset = Marshal.SizeOf(typeof(SMsgRecvAnswerFriends_SC));
        //int offset = 19 + 4 + 4 + 4 + 4 + 4 + 4 + 4;
        for (int i = 0; i < sMsgGetActorListHead.dwFriendNum; ++i)
        {
            sMsgGetActorListHead.sMsgRecvAnswerFriends_SC[i] = SMsgRecvAnswerFriends_SC.ParsePackage(dataBuffer.Skip(4 + 1 + 1 + i * offset).Take(offset).ToArray());
        }

        return sMsgGetActorListHead;
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

//同步玩家信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgRecvAnswerFriends_SC
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[]  szName;					//应答好友姓名  :服务器端为char[19]
    public uint dwFriendID;                 //应答玩家ID
    public uint dbSysActorImageHeadID;		//同步玩家头像ID
    public uint dProfession;                //玩家职业
    public uint sActorLevel;				//应答好友等级
    public uint dwSex;                      //性别
    public uint bOnLine;					//玩家是否在线	
    public uint dwTeamType;                 //0代表无队伍，1代表队长，2代表队员
    public uint dwEctypeLevel;              //好友最高通关
	public uint dwInBattle;					//是否在战斗 0=未 1=是
	public int  dwFight;					//好友战力值
	public int dwButtonIndex;				//按钮进度 (任务进度)

    public string Name
    {
        get { return System.Text.Encoding.UTF8.GetString(szName); }
    }

    public bool IsOnLine
    {
        get { return Convert.ToBoolean(bOnLine); }
    }

    public static SMsgRecvAnswerFriends_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgRecvAnswerFriends_SC sMsgRecvAnswerFriends_SC = new SMsgRecvAnswerFriends_SC();
        int of = 0;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.szName,19);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.dwFriendID);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.dbSysActorImageHeadID);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.dProfession);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.sActorLevel);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.dwSex);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.bOnLine);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.dwTeamType);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.dwEctypeLevel);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.dwInBattle);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.dwFight);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgRecvAnswerFriends_SC.dwButtonIndex);        

        return sMsgRecvAnswerFriends_SC;
    }

	public void SetValue(byte index, uint value)
    {      
		//var bytes = PackageHelper.StructToBytes<SMsgRecvAnswerFriends_SC>(this);
		var bytes = new byte[63];
		this.szName.CopyTo(bytes, 0);
        PackageHelper.WriteData(this.dwFriendID).CopyTo(bytes, 19);
        PackageHelper.WriteData(this.dbSysActorImageHeadID).CopyTo(bytes, 23);
        PackageHelper.WriteData(this.dProfession).CopyTo(bytes, 27);
        PackageHelper.WriteData(this.sActorLevel).CopyTo(bytes, 31);
        PackageHelper.WriteData(this.dwSex).CopyTo(bytes, 35);
        PackageHelper.WriteData(this.bOnLine).CopyTo(bytes, 39);
        PackageHelper.WriteData(this.dwTeamType).CopyTo(bytes, 43);
        PackageHelper.WriteData(this.dwEctypeLevel).CopyTo(bytes, 47);
		PackageHelper.WriteData(this.dwInBattle).CopyTo(bytes, 51);
		PackageHelper.WriteData(this.dwFight).CopyTo(bytes, 55);
		PackageHelper.WriteData(this.dwButtonIndex).CopyTo(bytes, 59);

		int offset = 19 + index * 4;
		var source = BitConverter.GetBytes(value);
		bytes[offset] = source[0];
		bytes[offset + 1] = source[1];
		bytes[offset + 2] = source[2];
		bytes[offset + 3] = source[3];
		
	    this =  SMsgRecvAnswerFriends_SC.ParsePackage(bytes);

//        var bytes = new byte[Marshal.SizeOf(typeof(SMsgRecvAnswerFriends_SC))];
//        
//        this.szName.CopyTo(bytes, 0);
//        PackageHelper.WriteData(this.dwFriendID).CopyTo(bytes, 19);
//        PackageHelper.WriteData(this.dbSysActorImageHeadID).CopyTo(bytes, 19 + 4);
//        PackageHelper.WriteData(this.dProfession).CopyTo(bytes, 19 + 4 + 4);
//        PackageHelper.WriteData(this.sActorLevel).CopyTo(bytes, 19 + 4 + 4 + 4);
//        PackageHelper.WriteData(this.dwSex).CopyTo(bytes, 19 + 4 + 4 + 4 + 4);
//        PackageHelper.WriteData(this.bOnLine).CopyTo(bytes, 19 + 4 + 4 + 4 + 4 + 4);
//        PackageHelper.WriteData(this.dwTeamType).CopyTo(bytes, 19 + 4 + 4 + 4 + 4 + 4 + 4);
//        PackageHelper.WriteData(this.dwEctypeLevel).CopyTo(bytes, 19 + 4 + 4 + 4 + 4 + 4 + 4 + 4);
//
//        int offset = Convert.ToInt32(index) * 4 + 19;
//        var source = BitConverter.GetBytes(value);
//        bytes[offset] = source[0];
//        bytes[offset + 1] = source[1];
//        bytes[offset + 2] = source[2];
//        bytes[offset + 3] = source[3];
//
//        this = SMsgRecvAnswerFriends_SC.ParsePackage(bytes);
    }
};

//添加好友请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgAddFriends_CS
{
	public uint	dwActorID;
	public uint	dwFriendID;
};

//添加好友应答(被邀请的玩家会收到好友应答，发送请求的玩家无应答)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgAdddFriends_SC
{
	public uint dSendRequestActorID;			//发送好友请求的ID
	public int dReceiverActorID;				//接收请求者ID
    public UInt64 tCurTime;
    public SMsgRecvAnswerFriends_SC sMsgRecvAnswerFriends_SC;
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    //public char[] cSendRequestName;			//发送好友请求的玩家的名字

    public static SMsgAdddFriends_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgAdddFriends_SC sMsgAdddFriends_SC = new SMsgAdddFriends_SC();

        int of = 0;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgAdddFriends_SC.dSendRequestActorID);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgAdddFriends_SC.dReceiverActorID);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgAdddFriends_SC.tCurTime);
        byte[] buffer;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out buffer, Marshal.SizeOf(typeof(SMsgRecvAnswerFriends_SC)));
        sMsgAdddFriends_SC.sMsgRecvAnswerFriends_SC = SMsgRecvAnswerFriends_SC.ParsePackage(buffer);

        //sMsgAdddFriends_SC.dSendRequestActorID = BitConverter.ToUInt32(dataBuffer, 0);
        //sMsgAdddFriends_SC.tCurTime = BitConverter.ToUInt64(dataBuffer, 4);
        //int dataLength = 19 + 4 + 4 + 4 + 4 + 4 + 4 + 4;  //等于Mashal.Sizeof(SMsgRecvAnswerFriends_SC);
       
        //sMsgAdddFriends_SC.cSendRequestName = BitConverter.ToString(dataBuffer, 4, 19).ToCharArray();

        return sMsgAdddFriends_SC;
    }
};


//MSG_FRIEND_REQUESTLIST
//下发好友请求列表
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgFriendRequestSC
{
	public uint AskActorID;			//请求发起ID
	public uint dwExpireTime;			//请求过期时间
	public byte	byLevel;				//好友等级
	public byte	byKind;					//好友职业
	public byte	bySex;					//好友性别
	public uint	dwActorHeadID;			//角色头像ID
	public int	dwFightNum;				//好友战斗力
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	public byte[] szActorName;		//好友角色名
	public string Name
	{
		get { return System.Text.Encoding.UTF8.GetString(szActorName); }
	}

	//public SMsgRecvAnswerFriends_SC sMsgRecvAnswerFriends_SC;

	public static SMsgFriendRequestSC ParsePackage(byte[] dataBuffer, ref int off)
	{
		SMsgFriendRequestSC sMsgFriendRequestSC = new SMsgFriendRequestSC();
		off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMsgFriendRequestSC.AskActorID);
		off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMsgFriendRequestSC.dwExpireTime);
		off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMsgFriendRequestSC.byLevel);
		off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMsgFriendRequestSC.byKind);
		off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMsgFriendRequestSC.bySex);
		off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMsgFriendRequestSC.dwActorHeadID);
		off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMsgFriendRequestSC.dwFightNum);
		off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMsgFriendRequestSC.szActorName, 19);

		//byte[] buffer;
		//off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out buffer, Marshal.SizeOf(typeof(SMsgRecvAnswerFriends_SC)));
		//sMsgFriendRequestSC.sMsgRecvAnswerFriends_SC = SMsgRecvAnswerFriends_SC.ParsePackage(buffer);
		return sMsgFriendRequestSC;
	}
};
//下发好友请求列表
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgGetReuqestListHead
{
	public int	dwActorID;		//角色ID
	public byte	byRequestNum;	//请求个数

	public SMsgFriendRequestSC[] FriendRequests;

	public static SMsgGetReuqestListHead ParsePackage(byte[] dataBuffer)
	{
		SMsgGetReuqestListHead sMsgGetReuqestListHead = new SMsgGetReuqestListHead();
		int of = 0;
		of+= PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgGetReuqestListHead.dwActorID);
		of+= PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgGetReuqestListHead.byRequestNum);
		sMsgGetReuqestListHead.FriendRequests = new SMsgFriendRequestSC[sMsgGetReuqestListHead.byRequestNum];
		for(int i=0;i<sMsgGetReuqestListHead.byRequestNum;i++)
		{
			sMsgGetReuqestListHead.FriendRequests[i] = SMsgFriendRequestSC.ParsePackage(dataBuffer,ref of);
		}
		return sMsgGetReuqestListHead;
	}
};

//玩家发送接受或者拒绝好友请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgAnswerFriends_CS
{
	public byte	bAnswer;					//回馈给玩家的应答（0是拒绝，1是同意）
	public uint	dwActorID;					//应答的玩家ID
	public uint	dwbAnswerActorID;			//被回应的玩家ID；
};

//玩家在处理接收或者拒绝信息后的应答
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgAnswerFriends_SC
{
    public uint dwActorID;					//角色ID
    public uint dwFriendActorID;			//好友角色ID
    public SMsgRecvAnswerFriends_SC sMsgRecvAnswerFriends_SC;

    public static SMsgAnswerFriends_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgAnswerFriends_SC sMsgAnswerFriends_SC = new SMsgAnswerFriends_SC();
        int of = 0;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgAnswerFriends_SC.dwActorID);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgAnswerFriends_SC.dwFriendActorID);
        byte[] buffer;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out buffer, Marshal.SizeOf(typeof(SMsgRecvAnswerFriends_SC)));
        sMsgAnswerFriends_SC.sMsgRecvAnswerFriends_SC = SMsgRecvAnswerFriends_SC.ParsePackage(buffer);

        //sMsgAnswerFriends_SC.dwActorID = BitConverter.ToUInt32(dataBuffer, 0);
        //sMsgAnswerFriends_SC.dwFriendActorID = BitConverter.ToUInt32(dataBuffer, 4);
        //int dataLength = 19 + 4 + 4 + 4 + 4 + 4 + 4 + 4;  //等于Mashal.Sizeof(SMsgRecvAnswerFriends_SC);

        return sMsgAnswerFriends_SC;
    }
};

//删除好友应答信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgDelFriendInfo_CS
{
    public uint dwActorID;					//玩家角色ID
    public uint dwDelActorID;				//被删除信息中的角色ID
};

//下发某个玩家的线上信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgUpdateOnLine_SC
{
    public uint dwActorID;					//玩家角色ID
    public uint dwFriendActorID;			//玩家好友角色ID；
    public byte byBeOnLine;					//玩家上线与否
};

//删除好友请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgDelFriends_CS
{
	public uint	dwActorID;					//玩家ActorID;
	public uint dwbDelActorID;				//被删除玩家ID

};

//删除好友应答
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgDelFriends_SC
{
    public uint dwActorID;					//玩家ActorID;
    public uint dwbDelActorID;				//被删除玩家ID
};

//更新好友属性
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgUpdateFriendProp_SC
{
    public uint dwFriendActorID;					//玩家ActorID；
    public byte byIndex;					//现场属性编号
    public uint dwPropNum;					//属性值
};


//好友
//public enum EFriendFields
//{
//    FRIEND_FIELD_VISIBLEBGN,
//    FRIEND_FIELD_ACTORID = FRIEND_FIELD_VISIBLEBGN,		// 好友ActorID
//    FRIEND_FIELD_HEAD,			    // 头像
//    FRIEND_FIELD_KIND,				// 职业
//    FRIEND_FIELD_LEV,				// 等级
//    FRIEND_FIELD_EX,				// 性别
//    FRIEND_FIELD_ONLINE,			// 是否在线
//    FRIEND_FIELD_BELEADER,			//是否为队长(0无组队 1 队员 2 队长)

//    FRIEND_FIELD_VISIBLEEND,

//    FRIEND_FIELD_FRIENDVALUE = FRIEND_FIELD_VISIBLEEND,	// 好友亲密度
//    FRIEND_FIELD_MAPID,			// 场景ID(静态地图ID)
//    FRIEND_FIELD_ISVIP,			// 是否是VIP
//    FRIEND_FIELD_CURHP,			// 当前HP
//    FRIEND_FIELD_MAXHP,			// 最大HP
//    FRIEND_FIELD_CURMP,			// 当前MP
//    FRIEND_FIELD_MAXMP,			// 最大MP
//    FRIEND_FIELD_END,
//};