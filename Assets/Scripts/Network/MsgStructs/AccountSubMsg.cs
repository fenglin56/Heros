using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

// 登录随机数现场(S->C)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSEnterCodeContext : INotifyArgs
{
    public int dwEnterCode;	// [0,99]
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] randStrBuf;	// 随机串

    public string RandStrBuf
    {
        set
        {
            randStrBuf = new byte[32];
            //try
            //{
            Encoding.Default.GetBytes(value).CopyTo(randStrBuf, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    Debug.LogException(ex);
            //}

        }
        get
        {
            return randStrBuf != null ? Encoding.Default.GetString(randStrBuf) : "randStrBuf is null";
        }
    }
    public static SSEnterCodeContext ParsePackage(byte[] dataBuffer)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);

        SSEnterCodeContext sSEnterCodeContext = new SSEnterCodeContext();
        int offset = 0;
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSEnterCodeContext.dwEnterCode);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSEnterCodeContext.randStrBuf, 32);
        //sSEnterCodeContext.dwEnterCode =BitConverter.ToInt32(package.Data.Take(4).ToArray(),0);
        //sSEnterCodeContext.randStrBuf = package.Data.Skip(4).Take(32).ToArray();
        return sSEnterCodeContext;
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
// 登录现场(C->S)
// 此处根据需要，登录现场不断完善
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSUserLoginContext
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
    private byte[] szUserName;	// 用户名 33
    public int PlatformID;						// 平台ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37)]
    private byte[] szPlatformKey;		// 平台Key  37
    public ushort wServerID;						// 游戏服ID
    public byte byCM;						// 防沉迷标记  1是 0否


    public string SZUserName
    {
        set
        {
            szUserName = new byte[33];
            //try
            //{
            Encoding.UTF8.GetBytes(value).CopyTo(szUserName, 0);
            string codename = Encoding.Default.BodyName;
            //}
            //catch (System.Exception ex)
            //{
            //    Debug.LogException(ex);
            //}

        }
        get
        {
            return szUserName != null ? Encoding.UTF8.GetString(szUserName) : string.Empty;
        }
    }
    public string SZPlatFormKey
    {
        set
        {
            szPlatformKey = new byte[37];
            //try
            //{
            Encoding.Default.GetBytes(value).CopyTo(szPlatformKey, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    //TraceUtil.Log(ex.Message);
            //}

        }
        get
        {
            return szPlatformKey != null ? Encoding.Default.GetString(szPlatformKey) : string.Empty;
        }
    }
    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        byte[] byCMArray = new byte[] { byCM };
        pkg.Data = PackageHelper.GetByteFromArray(this.szUserName, 33)
            .Concat(BitConverter.GetBytes(this.PlatformID))
            .Concat(PackageHelper.GetByteFromArray(this.szPlatformKey, 37))
            .Concat(BitConverter.GetBytes(this.wServerID))
            .Concat(byCMArray).ToArray();
        return pkg;
    }
};
// 用户登录返回(S->C)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSUserLoginRes : INotifyArgs
{
    public int lUserID;			// 用户ID
    public int lActorID;			// 角色ID
    public int lMapID;				// 地图ID 
    public int lBackMapID;			// 备用地图ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    private byte[] szServerIP;		// 所在服务器IP 16
    public ushort wPort;				// 所在服务器端口

    public int lPromptFlag;		// 默认值返回0，返回1表示要弹出提示信息的窗口，具体内容会通过lPromptMsg提供
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    private byte[] lPromptMsg;	// 提示信息 256
    public uint m_dwVersion;		// 服务器版本，见DHardGlobal.h的VERSION_NUMBER定义

    public string SZServerIP
    {
        set
        {
            szServerIP = new byte[16];
            //try
            //{
            Encoding.Default.GetBytes(value).CopyTo(szServerIP, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,ex);
            //}

        }
        get
        {
            return szServerIP != null ? Encoding.Default.GetString(szServerIP) : string.Empty;
        }
    }
    public string LPromptMsg
    {
        set
        {
            lPromptMsg = new byte[256];
            //try
            //{
            Encoding.Default.GetBytes(value).CopyTo(lPromptMsg, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    //TraceUtil.Log(ex.Message);
            //}

        }
        get
        {
            return lPromptMsg != null ? Encoding.Default.GetString(lPromptMsg) : string.Empty;
        }
    }

    public static SSUserLoginRes ParsePackage(byte[] dataBuffer)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);

        SSUserLoginRes sSUserLoginRes = new SSUserLoginRes();
        int offset = 0;
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.lUserID);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.lActorID);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.lMapID);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.lBackMapID);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.szServerIP, 16);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.wPort);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.lPromptFlag);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.lPromptMsg, 256);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.m_dwVersion);

        //sSUserLoginRes.lUserID = BitConverter.ToInt32(package.Data, 0);
        //sSUserLoginRes.lActorID = BitConverter.ToInt32(package.Data, 4+0);
        //sSUserLoginRes.lMapID = BitConverter.ToInt32(package.Data, 4+4);
        //sSUserLoginRes.lBackMapID = BitConverter.ToInt32(package.Data, 4+4+4);
        //sSUserLoginRes.szServerIP = package.Data.Skip(4+4+4+4).Take(16).ToArray();
        //sSUserLoginRes.wPort = BitConverter.ToUInt16(package.Data, 4+4+4+4+16);
        //sSUserLoginRes.lPromptFlag = BitConverter.ToInt32(package.Data, 4 + 4 + 4 + 4 + 16 + 2);
        //sSUserLoginRes.lPromptMsg = package.Data.Skip(4 + 4 + 4 + 4 + 16+2+4).Take(256).ToArray();
        //sSUserLoginRes.m_dwVersion = BitConverter.ToUInt32(package.Data, 4 + 4 + 4 + 4 + 16 + 2 + 4 + 256);

        return sSUserLoginRes;
    }
    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};
// 创建角色现场(C->S)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SSActorCreateContext
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    private byte[] szName;	// 角色名 19
    public byte byKind;						// 职业

    public string SZName
    {
        set
        {
            szName = new byte[19];
            //try
            //{
            //Encoding.Default.GetBytes(value).CopyTo(szName, 0);
            Encoding.UTF8.GetBytes(value).CopyTo(szName, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    //TraceUtil.Log(ex.Message);
            //}

        }
        get
        {
            return szName != null ? Encoding.UTF8.GetString(szName) : string.Empty;
        }
    }

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = PackageHelper.GetByteFromArray(szName, 19)
            .Concat(new byte[] { byKind }).ToArray();
        //TraceUtil.Log("asdfasfjasdgjasd;lgjk" + Encoding.UTF8.GetString(pkg.Data.Take(19).ToArray()) +"  Name:"+ SZName);
        return pkg;
    }
};
// ROOTSELECTACTOR_CS_MAIN_JUMP_START
// 已登录玩家服务器跳转开始(进入场景前) - 输入结构体 C->S
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SLoginUserJumpStartContext
{
    public int lUserID;		// 用户ID
    public int lActorID;		// 角色ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
    private byte[] szRandBuf;	// 客户端标示串Buffer128

    public string SZRandBuf
    {
        set
        {
            szRandBuf = new byte[128];
            //try
            //{
            Encoding.Default.GetBytes(value).CopyTo(szRandBuf, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    //TraceUtil.Log(ex.Message);
            //}

        }
        get
        {
            return szRandBuf != null ? Encoding.Default.GetString(szRandBuf) : string.Empty;
        }
    }

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = BitConverter.GetBytes(this.lUserID)
            .Concat(BitConverter.GetBytes(lActorID))
            .Concat(PackageHelper.GetByteFromArray(this.szRandBuf, 128)).ToArray();
        return pkg;
    }
};
// ROOTSELECTACTOR_SC_MAIN_JUMP_START
// 已登录玩家服务器跳转开始(进入场景前) - 输出结构体  S->C
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SLoginUserJumpStartRes
{
    public int lReturn;			// 返回值 1 成功
    public int lUserID;			// UserID
    public int lActorID;			// 角色ID
    public uint ValidationCode;	// 验证码

    public static SLoginUserJumpStartRes ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.ParseDataBufferToStruct<SLoginUserJumpStartRes>(dataBuffer);
    }
};
/////////////////////////////////////////////////////////
//// 描  述：客户端发给服务器的选择人物态消息码  C->S
/////////////////////////////////////////////////////////
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct CS_SELECTACTOR_HEAD
{
    public byte m_wKeyRoot;
    public ushort m_wKeyMain;
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct CS_SELECTACTOR_MAC_HEAD
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
    private byte[] m_MACBuf;			//存放客户端随机串的空间 128
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    private byte[] randChar16_V;		//存放16字节随机串的空间 16

    public int m_dwUserID;			//帐号的UserID
    public int m_dwActorID;			//选择的人物ActorID
    
    public byte m_byServerID;		// 地图场景服务器ID
    public int m_MapID;			// 地图ID
    public byte m_byBackServerID;	// 备用地图场景服务器ID
    public int m_BakMapID;			// 备用地图ID

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    private byte[] szSerialNO;		//网卡序列号  20
    public byte m_LinkType;			// 网络连接类型 0=wifi，1=3G，2=其他
    public byte m_OPType;			// 设置操作类型 0=触屏，1=摇杆
    public byte m_ViewType;			// 设置视野类型 视野类型为：1-8

    public string M_MACBuf
    {
        set
        {
            m_MACBuf = new byte[128];
            //try
            //{
            Encoding.UTF8.GetBytes(value).CopyTo(m_MACBuf, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    //TraceUtil.Log(ex.Message);
            //}

        }
        get
        {
            return m_MACBuf != null ? Encoding.Default.GetString(m_MACBuf) : string.Empty;
        }
    }
    public string RandChar16_V
    {
        set
        {
            randChar16_V = new byte[16];
            //try
            //{
            Encoding.UTF8.GetBytes(value).CopyTo(randChar16_V, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    //TraceUtil.Log(ex.Message);
            //}

        }
        get
        {
            return randChar16_V != null ? Encoding.Default.GetString(randChar16_V) : string.Empty;
        }
    }
    public string SZSerialNO
    {
        set
        {
            szSerialNO = new byte[20];
            //try
            //{
            Encoding.UTF8.GetBytes(value).CopyTo(szSerialNO, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    //TraceUtil.Log(ex.Message);
            //}

        }
        get
        {
            return szSerialNO != null ? Encoding.Default.GetString(szSerialNO) : string.Empty;
        }
    }

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, subMsgType);
        pkg.Data = PackageHelper.GetByteFromArray(m_MACBuf, 128)
            .Concat(PackageHelper.GetByteFromArray(randChar16_V, 16))
            .Concat(BitConverter.GetBytes(this.m_dwUserID))
            .Concat(BitConverter.GetBytes(this.m_dwActorID))           
                .Concat(new byte[] { this.m_byServerID })
            .Concat(BitConverter.GetBytes(this.m_MapID))
            .Concat(new byte[] { this.m_byBackServerID })
            .Concat(BitConverter.GetBytes(this.m_BakMapID))
             .Concat(PackageHelper.GetByteFromArray(szSerialNO, 20))
             .Concat(new byte[] { this.m_LinkType })
             .Concat(new byte[] { this.m_OPType })
             .Concat(new byte[] { this.m_ViewType })
             .ToArray();

        return pkg;
    }

    public static CS_SELECTACTOR_MAC_HEAD ParsePackage(byte[] dataBuffer)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);

        CS_SELECTACTOR_MAC_HEAD cS_SELECTACTOR_MAC_HEAD = new CS_SELECTACTOR_MAC_HEAD();
        int offset = 0;
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.m_MACBuf, 128);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.randChar16_V, 16);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.m_dwUserID);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.m_dwActorID);        
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.m_byServerID);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.m_MapID);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.m_byBackServerID);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.m_BakMapID);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.szSerialNO, 20);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.m_LinkType);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.m_OPType);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out cS_SELECTACTOR_MAC_HEAD.m_ViewType);

        //cS_SELECTACTOR_MAC_HEAD.m_MACBuf = package.Data.Take(128).ToArray();
        //cS_SELECTACTOR_MAC_HEAD.randChar16_V = package.Data.Skip(128).Take(16).ToArray();
        //cS_SELECTACTOR_MAC_HEAD.m_dwUserID = BitConverter.ToInt32(package.Data, 128+16);
        //cS_SELECTACTOR_MAC_HEAD.m_dwActorID = BitConverter.ToInt32(package.Data, 128+16+4);
        //cS_SELECTACTOR_MAC_HEAD.szSerialNO = package.Data.Skip(128+16+4+4).Take(20).ToArray();
        //cS_SELECTACTOR_MAC_HEAD.m_MapID = BitConverter.ToUInt16(package.Data, 128+16+4+4+20);
        //cS_SELECTACTOR_MAC_HEAD.m_BakMapID = BitConverter.ToUInt16(package.Data, 128 + 16 + 4 + 4 + 20+2);

        return cS_SELECTACTOR_MAC_HEAD;
    }
};


// 生物移动消息体(包括所有移动路径点)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionMove_SCS : INotifyArgs
{
    public Int64 uidEntity;     //实体ID
    public uint dwMapID;
    public float floatX;        // 坐标X
    public float floatY;        // 坐标Y
    public float fDirectX;      // 方向X
    public float fDirectY;      // 方向Y   
    public float fSpeed;

    public static SMsgActionMove_SCS ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.ParseDataBufferToStruct<SMsgActionMove_SCS>(dataBuffer);
    }
    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SMsgActionMove_SCS>(this);
        return pkg;
    }
    public int GetEventArgsType()
    {
        return 0;
    }

    public SMsgActionMove_SCS Copy(SMsgActionMove_SCS s1)
    {
        return (SMsgActionMove_SCS)(s1.MemberwiseClone());
    }
}

//// 怪物或玩家死亡
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionDie_SC : INotifyArgs
{
    public Int64 uidEntity;     //实体ID
    public Int64 uidMuderer;		//谋杀者的uid
    public byte byDieType;		//死亡类型，0：普通 1：被切碎
    public byte byTrigger;		//死亡触发：0: 普通 1：触发吸血
    public int nParam;			//参数 用于消息交互(示例：若触发吸血,则为吸血序号(后台处理机制))

    public static SMsgActionDie_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.ParseDataBufferToStruct<SMsgActionDie_SC>(dataBuffer);
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

// 生物停止移动消息体
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionClientOptMove_CS
{
    public uint dwMapID;    //地图id
    public uint dwPathLen;  //路径长度

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SMsgActionClientOptMove_CS>(this);
        return pkg;
    }
}

//#define  MSG_ACTION_STOPHERE
// 路径点 停止移动消息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionStopHere_SC : INotifyArgs
{
    public Int64 uidEntity;
    public uint dwMapID;		// 地图ID
    public float ptHereX;		// X坐标
    public float ptHereY;		// Y坐标
    public float fDirectX;		// 方向X
    public float fDirectY;		// 方向Y
    public byte byForceSync;	// 是否强制拉人 0:不强制， 1:强制(初定为两倍速走完当前路径点)

    public static SMsgActionStopHere_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.ParseDataBufferToStruct<SMsgActionStopHere_SC>(dataBuffer);
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

//服务器怪物移动消息(路径点)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionMonsterMove_SC : INotifyArgs
{
    public Int64 uidMonster;      //怪物id
    public uint dwMapID;		// 地图ID
    public uint dwPathLen;		// 路径长度

    public Vector2[] dwPoints;  // 路径节点，POINT2 * dwPathLen

    public static SMsgActionMonsterMove_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgActionMonsterMove_SC monsterMoveSmg = new SMsgActionMonsterMove_SC();
        int offset = 0;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out monsterMoveSmg.uidMonster);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out monsterMoveSmg.dwMapID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out monsterMoveSmg.dwPathLen);
        uint arrayLength = monsterMoveSmg.dwPathLen;
        monsterMoveSmg.dwPoints = new Vector2[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out monsterMoveSmg.dwPoints[i].x);
            offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out monsterMoveSmg.dwPoints[i].y);
        }
        return monsterMoveSmg;
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

//*********************************************************

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NewSSUserLoginContext
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 47)]
    private byte[] szUserName;	// 用户名 47
    public int PlatformID;						// 平台ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 47)]
    private byte[] szPlatformKey;		// 平台Key  47
    public ushort wWorldID;						// 游戏服ID
    public byte byCM;						// 防沉迷标记  1是 0否
	public int nVersion;						//  客户端和服务端的对比号

    public string SZUserName
    {
        set
        {
            szUserName = new byte[47];
            //try
            //{
            Encoding.UTF8.GetBytes(value).CopyTo(szUserName, 0);
            string codename = Encoding.Default.BodyName;
            //}
            //catch (System.Exception ex)
            //{
            //    Debug.LogException(ex);
            //}

        }
        get
        {
            return szUserName != null ? Encoding.UTF8.GetString(szUserName) : string.Empty;
        }
    }
    public string SZPlatFormKey
    {
        set
        {
            szPlatformKey = new byte[47];
            //try
            //{
            Encoding.Default.GetBytes(value).CopyTo(szPlatformKey, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    //TraceUtil.Log(ex.Message);
            //}

        }
        get
        {
            return szPlatformKey != null ? Encoding.Default.GetString(szPlatformKey) : string.Empty;
        }
    }
    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        byte[] byCMArray = new byte[] { byCM };
        pkg.Data = PackageHelper.GetByteFromArray(this.szUserName, 47)
            .Concat(BitConverter.GetBytes(this.PlatformID))
            .Concat(PackageHelper.GetByteFromArray(this.szPlatformKey, 47))
            .Concat(BitConverter.GetBytes(this.wWorldID))
			.Concat(byCMArray)
			.Concat(BitConverter.GetBytes(this.nVersion)).ToArray();
        return pkg;
    }
};
// 用户登录返回(S->C)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NewSSUserLoginRes : INotifyArgs
{
    public int lUserID;			// 用户ID
    public int lActorNum;			// 角色数目    

    public int lPromptFlag;		// 默认值返回0，返回1表示要弹出提示信息的窗口，具体内容会通过lPromptMsg提供
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    private byte[] lPromptMsg;	// 提示信息 256

    public SSActorInfo[] SSActorInfos;
    public string LPromptMsg
    {
        set
        {
            lPromptMsg = new byte[256];
            //try
            //{
            Encoding.Default.GetBytes(value).CopyTo(lPromptMsg, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    //TraceUtil.Log(ex.Message);
            //}

        }
        get
        {
            return lPromptMsg != null ? Encoding.Default.GetString(lPromptMsg) : string.Empty;
        }
    }

    public static NewSSUserLoginRes ParsePackage(byte[] dataBuffer)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);

        NewSSUserLoginRes sSUserLoginRes = new NewSSUserLoginRes();
        int offset = 0;
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.lUserID);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.lActorNum);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.lPromptFlag);
        offset += PackageHelper.ReadData(package.Data.Skip(offset).ToArray(), out sSUserLoginRes.lPromptMsg, 256);

        sSUserLoginRes.SSActorInfos = new SSActorInfo[sSUserLoginRes.lActorNum];
        for (int i = 0; i < sSUserLoginRes.lActorNum; i++)
        {
            sSUserLoginRes.SSActorInfos[i] = SSActorInfo.ParsePackage(package.Data, ref offset);
        }
        return sSUserLoginRes;
    }
    public int GetEventArgsType()
    {
        return 0;
    }
};
// 角色信息现场(S->C)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSActorInfo
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[] szName;			// 角色名
    public int lActorID;			// 角色ID

    public byte byKind;				//	每个创建的角色职业唯一
    public byte lLevel;				// 级别

    public byte byServerID;			// 地图场景服ID
    public int lMapID;				// 地图ID
    public byte byBackServerID;		// 备用地图场景服ID
    public int lBackMapID;			// 备用地图ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] AppearRes;		// 外观资源ID  资源外观数据区  (装备ID 4字节) * 2
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] szServerIP;		// 所在服务器IP
    public ushort wPort;				// 所在服务器端口
    public byte lKillFlag;			// 封号标志，0：正常，1：封号

    public static SSActorInfo ParsePackage(byte[] dataBuffer, ref int offset)
    {
        SSActorInfo sSActorInfo = new SSActorInfo();
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.szName, 19);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.lActorID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.byKind);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.lLevel);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.byServerID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.lMapID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.byBackServerID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.lBackMapID);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.AppearRes, 8);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.szServerIP, 16);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.wPort);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sSActorInfo.lKillFlag);

        return sSActorInfo;
    }
    /// <summary>
    /// 武器资源ID
    /// </summary>
    public int WeaponResID
    {
        get
        {
            return BitConverter.ToInt32(AppearRes, 0);
        }

    }
    /// <summary>
    /// 服装资源ID
    /// </summary>
    public int FasionResID
    {
        get
        {
            return BitConverter.ToInt32(AppearRes, 4);
        }

    }
    public string SZName
    {
        get
        {
            return szName != null ? Encoding.UTF8.GetString(szName) : string.Empty;
        }
    }
    public string SZServerIP
    {
        set
        {
            szServerIP = new byte[16];
            //try
            //{
            Encoding.Default.GetBytes(value).CopyTo(szServerIP, 0);
            //}
            //catch (System.Exception ex)
            //{
            //    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,ex);
            //}

        }
        get
        {
            return szServerIP != null ? Encoding.UTF8.GetString(szServerIP) : string.Empty;
        }
    }
}
// 删除角色现场(C->S)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSActorDeleteContext
{
    public int lActorID;	// 角色ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
    public byte[] szKeyMD5;	// 二级密码的 MD5

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
        pkg.Data = BitConverter.GetBytes(this.lActorID)
            .Concat(szKeyMD5).ToArray();
        return pkg;
    }
};

//更新翻滚体力值
public struct SUpdateRollStrengthStruct : INotifyArgs
{
    public int strengthValue;

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}