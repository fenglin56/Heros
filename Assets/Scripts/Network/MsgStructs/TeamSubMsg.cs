using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Text;

public enum TEAM_ISHERO
{
    TEAM_ISHERO_NONE = 0,			// 无
    TEAM_ISHERO_YES,			// 是自己队伍
    TEAM_ISHERO_NO				// 不是自己队伍
};
//角色组队操作类型
public enum ACTOR_OP_TEAM_TYPE
{
	ACTOR_TEAMJOIN_TYPE = 0,					//加入队伍操作
	ACTOR_FASTJOIN_TYPE,					//快速加入队伍操作
	ACTOR_CHATJOIN_TYPE,					//聊天栏加入队伍操作
	ACTOR_INVITE_TYPE,						//好友邀请组队操作
};



// 队伍匹配
//MSG_TEAM_TEAMLIST = 0,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamList_CS
{
    public uint dwActorID;  //玩家ActorID

    public  Package GeneratePackage()
    {
        Package pkg = new Package();
        //主消息 走副本
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, (short)TeamDefineManager.MSG_TEAM_TEAMLIST);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamList_CS>(this);

        return pkg;
    }
}

//获取队伍列表 (走副本协议)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGGetTeamList_CS
{
    public Int64 uidEntity;     //自己id
    public uint dwEctypeID;     //副本id
    public byte byDifficulty;   //副本难度

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, (short)ECTYPE_DefineManager.MSG_ECTYPE_GETTEAMLIST);
        pkg.Data = PackageHelper.StructToBytes<SMSGGetTeamList_CS>(this);
        //TraceUtil.Log("发出去:"+pkg.Head.DataLength);
        return pkg;
    }
}

// 队伍创建(不是真正意义上的创建队伍) 打开创建副本面板请求 真正创建队伍是队长操作“创建副本”
//MSG_TEAM_CREATE = 1,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamCreate_CS
{
    public uint dwActorID;				//玩家ActorID
    public uint dwEctypeID;				//玩家请求创建的副本区域ID
	public byte byEctypeIndex;			//玩家请求创建的副本的编号ID
    public byte byDiff;					//玩家请求创建的副本难度
	public byte byCreateType;				//队伍创建方式，默认为0，即可，1为随机队伍
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_CREATE);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamCreate_CS>(this);
        return pkg;
    }
};

// 队伍解散
//MSG_TEAM_DISBAND = 2,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamDisband_CS
{
    public uint dwActorID;			//队长ActorID
    public uint dwTeamID;			//队伍ID

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_DISBAND);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamDisband_CS>(this);
        return pkg;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamDisband_SC : INotifyArgs
{
    public uint dwTeamID;					//队伍ID

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_DISBAND);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamDisband_SC>(this);
        return pkg;
    }

    public static SMsgTeamDisband_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgTeamDisband_SC>(dataBuffer);
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

// 队伍信息
//MSG_TEAM_PROP = 3,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamNum_SC : INotifyArgs
{
    public ushort wTeamNum;			// 队伍的数目

    public SMsgTeamProp_SC[] SMsgTeamProps;
    // ...wTeamNum个(SMsgTeamProp_SC+队伍的上下文)


    public static SMsgTeamNum_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgTeamNum_SC sMsgTeamNum = new SMsgTeamNum_SC();

        sMsgTeamNum.wTeamNum = BitConverter.ToUInt16(dataBuffer, 0);
        sMsgTeamNum.SMsgTeamProps = new SMsgTeamProp_SC[sMsgTeamNum.wTeamNum];

        int offset = 0;
        //sMsgTeamNum.SMsgTeamProps.ApplyAllItem(p =>
        //    {
        //        //p = p.ParsePackage(dataBuffer, 0 + 2 + offset);
        //        //offset += p.wContextLen;
        //        offset += Marshal.SizeOf(p);
        //    });
        for (int i = 0; i < sMsgTeamNum.wTeamNum; i++)
        {
            sMsgTeamNum.SMsgTeamProps[i] = SMsgTeamProp_SC.ParsePackage(dataBuffer, 0 + 2 + offset);
            int memberNum = sMsgTeamNum.SMsgTeamProps[i].TeamMemberNum_SC.SMsgTeamPropMembers.Length;
            for (int j = 0; j < memberNum; j++)
            {
                offset += sMsgTeamNum.SMsgTeamProps[i].TeamMemberNum_SC.SMsgTeamPropMembers[j].wContextLen + 1 + 2; //加上每个队员信息长度
            }
            //sMsgTeamNum.SMsgTeamProps[i].TeamMemberNum_SC.SMsgTeamPropMembers.ApplyAllItem(p =>
            //    {
            //        offset += p.wContextLen + 1 + 2;    //加上每个队员信息长度
            //    });
            offset += 2;    //队员数量数据长度
            offset += sMsgTeamNum.SMsgTeamProps[i].wContextLen + 1 + 2; //队伍信息长度
        }

        return sMsgTeamNum;
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamProp_SC
{
    //\服务器那边是char型
    public byte nIsHero;		// 是否本队队员：1：是，2：否
    public ushort wContextLen;	// 队伍现场上下文长度

    public STeamContext TeamContext;    // 创建现场(参考 ETeamFields\ETeamMemberFields)

    public SMsgTeamMemberNum_SC TeamMemberNum_SC;   //队员信息

    public static SMsgTeamProp_SC ParsePackage(byte[] dataBuffer, int offset)
    {
        SMsgTeamProp_SC sMsgTeamProp = new SMsgTeamProp_SC();
        sMsgTeamProp.nIsHero = dataBuffer[offset];
        sMsgTeamProp.wContextLen = BitConverter.ToUInt16(dataBuffer, offset + 1);
        
        byte[] teamContextBuffer = dataBuffer.Skip(offset + 1 + 2).Take(sMsgTeamProp.wContextLen).ToArray();
        sMsgTeamProp.TeamContext = STeamContext.ParsePackage(teamContextBuffer);

        sMsgTeamProp.TeamMemberNum_SC = SMsgTeamMemberNum_SC.ParsePackage(dataBuffer, offset + 1 + 2 + sMsgTeamProp.wContextLen);

        return sMsgTeamProp;
    }

    public void Clear()
    {
        //this.TeamContext.byCurNum = 0;
        this = new SMsgTeamProp_SC();
    }

};

// 队员信息
//MSG_TEAM_MEMBER_PROP = 4,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamMemberNum_SC
{
    public ushort wMemberNum;	// 队员的数目

    public SMsgTeamPropMember_SC[] SMsgTeamPropMembers; // ...wMemberNum个(SMsgPropMember_SC+队员的上下文)   

    public static SMsgTeamMemberNum_SC ParsePackage(byte[] dataBuffer, int offset)
    {
        SMsgTeamMemberNum_SC sMsgTeamMemberNum = new SMsgTeamMemberNum_SC();
        sMsgTeamMemberNum.wMemberNum = BitConverter.ToUInt16(dataBuffer, offset);

        sMsgTeamMemberNum.SMsgTeamPropMembers = new SMsgTeamPropMember_SC[sMsgTeamMemberNum.wMemberNum];

        int memberBufferLength = 0;
        //sMsgTeamMemberNum.SMsgTeamPropMembers.ApplyAllItem(p =>
        //    {
        //        p = p.ParsePackage(dataBuffer, offset + 2 + memberBufferLength);

        //        //加上上次队员数据长度
        //        memberBufferLength += p.wContextLen;
        //    });
        for (int i = 0; i < sMsgTeamMemberNum.wMemberNum; i++)
        {
            //TraceUtil.Log("memberBufferLength====>"+memberBufferLength);
            sMsgTeamMemberNum.SMsgTeamPropMembers[i] = SMsgTeamPropMember_SC.ParsePackage(dataBuffer, offset + 2 + memberBufferLength);
            
            memberBufferLength += sMsgTeamMemberNum.SMsgTeamPropMembers[i].wContextLen + 1 + 2;     //+1+2是是否英雄和上下文数据的长度
        }

        return sMsgTeamMemberNum;
    }
};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamPropMember_SC
{
    public byte nIsHero;		// 是否本队队员：1：是，2：否
    public ushort wContextLen;	// 队员现场上下文长度
  
    public STeamMemberContext TeamMemberContext;    // 创建现场(参考 ETeamFields\ETeamMemberFields)

    public static SMsgTeamPropMember_SC ParsePackage(byte[] dataBuffer, int offset)
    {
        SMsgTeamPropMember_SC sMsgTeamPropMember = new SMsgTeamPropMember_SC();
        sMsgTeamPropMember.nIsHero = dataBuffer[offset];
        sMsgTeamPropMember.wContextLen = BitConverter.ToUInt16(dataBuffer, offset + 1);

        sMsgTeamPropMember.TeamMemberContext = sMsgTeamPropMember.TeamMemberContext.ParsePackage(dataBuffer, offset + 1 + 2, (TEAM_ISHERO)sMsgTeamPropMember.nIsHero);
        
        return sMsgTeamPropMember;
    }
};

//队伍信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct STeamContext
{
    public uint dwId;							// 队伍ID
    public int dwEctypeId;                     // 副本区域ID
	public int dwEctypeIndex;				   // 副本编号
    public int byEctypeDifficulty;             // 副本难度(关卡)
    public int byTeamStatus;                   // 队伍状态 1空闲 2战斗 

    public int dwCaptainId;					// 队长ID
    public int byCurNum;						// 当前人数
    public int byMaxNum;						// 最大人数

    public static STeamContext ParsePackage(byte[] dataBuffer)
    {        

        return PackageHelper.BytesToStuct<STeamContext>(dataBuffer);
    }

    public STeamContext UpdateValue(int index, int value)
    {
        var bytes = PackageHelper.StructToBytes<STeamContext>(this);

        int offset = index * 4;
        var source = BitConverter.GetBytes(value);
        bytes[offset] = source[0];
        bytes[offset + 1] = source[1];
        bytes[offset + 2] = source[2];
        bytes[offset + 3] = source[3];

        return PackageHelper.BytesToStuct<STeamContext>(bytes);
    }

    //public static STeamContext ParsePackage(byte[] dataBuffer, int offset)
    //{
    //    STeamContext sTeamContext = new STeamContext();
    //    sTeamContext.dwId = BitConverter.ToUInt16(dataBuffer, offset);
    //    sTeamContext.dwEctypeId = BitConverter.ToUInt16(dataBuffer, offset+2);
    //    sTeamContext.byEctypeDifficulty = dataBuffer[offset + 2 + 2];
    //    sTeamContext.byTeamStatus = dataBuffer[offset + 2 + 2 + 1];
    //    sTeamContext.dwCaptainId = BitConverter.ToUInt16(dataBuffer, offset + 2 + 2 + 1+1);
    //    sTeamContext.byCurNum = dataBuffer[offset + 2 + 2 + 1 + 1 + 2];
    //    sTeamContext.byMaxNum = dataBuffer[offset + 2 + 2 + 1 + 1 + 2 + 1];
    //    return sTeamContext;
    //}
}
//队员信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct STeamMemberContext
{
	public enum CRT_STATE //生物行为状态
	{
		enCrt_State_Unknow	=	0,		//生物处于未知状态
		enCrt_State_Stand,				//生物处于站立状态
		enCrt_State_Move,				//生物处于走路状态
		enCrt_State_Die,				//生物处于死亡状态
		enCrt_State_ReChange,			//生物的施法状态
		enCrt_State_BeAttack,			//生物的受击状态
		enCrt_State_Stealth,			//生物处于隐身状态
		enCrt_State_Animation,			//生物处于动画状态
		enCrt_State_Run,				//生物处于跑动状态（怪物可用）
		enCrt_State_Climbs,				//生物处于爬起状态
		enCrt_State_Max,
	};

    public uint dwActorID;		// 角色ID
    //public Int64 uidEntity;		// UID    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    private byte[] _szName;		// 名称	[19]
    public string szName;       //名称
    public int nTeamID;		// 队伍ID
    public int nHead;			// 头像
    public int byKind;			// 职业
    public int nLev;			// 等级
    public int bySex;			// 性别

    public int byOnline;		// 是否在线
    public int dwSceneId;		// 场景ID(静态地图ID)
    public int bIsVip;			// 是否是VIP
	public int dwState;			// 角色状态
    public int nCurHP;			// 当前HP
    public int nMaxHP;			// 最大HP
    public int nCurMP;			// 当前MP
    public int nMaxMP;			// 最大MP
    public int nCurActiveLife;  // 当前活力值
    public int nCurPayMoney;  // 当前元宝数量
    public int nFashionID;   //当前时装
    public int nCurWeaponID;	// 当前角色武器ID
    public int byFightReady;	// 是否准备副本战斗   0无状态  1准备
	public int fightNum;		//队员战斗力

    public STeamMemberContext ParsePackage(byte[] dataBuffer, int offset, TEAM_ISHERO nIsHero)
    {
        STeamMemberContext sTeamMenmberContext = new STeamMemberContext();

        int of = offset;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.dwActorID);
        //of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.uidEntity);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext._szName,19);
        sTeamMenmberContext.szName = Encoding.UTF8.GetString(sTeamMenmberContext._szName);
        //都可见
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nTeamID);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nHead);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.byKind);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nLev);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.bySex);
        //如果是自己的队伍
        if (nIsHero == TEAM_ISHERO.TEAM_ISHERO_YES)
        {
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.byOnline);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.dwSceneId);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.bIsVip);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.dwState);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nCurHP);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nMaxHP);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nCurMP);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nMaxMP);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nCurActiveLife);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nCurPayMoney);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nFashionID);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.nCurWeaponID);
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.byFightReady);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sTeamMenmberContext.fightNum);
        }

        //sTeamMenmberContext.dwActorID = BitConverter.ToUInt32(dataBuffer, offset);
        //sTeamMenmberContext.uidEntity = BitConverter.ToInt64(dataBuffer, offset + 4);
        //sTeamMenmberContext._szName = dataBuffer.Skip(offset + 8 + 4).Take(19).ToArray();   
        
        ////都可见
        //sTeamMenmberContext.nTeamID = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19);
        //sTeamMenmberContext.nHead = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4);
        //sTeamMenmberContext.byKind = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4);
        //sTeamMenmberContext.nLev = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4 + 4);
        //sTeamMenmberContext.bySex = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4 + 4 + 4);
        
        ////如果是自己的队伍
        //if (nIsHero == TEAM_ISHERO.TEAM_ISHERO_YES)
        //{
        //    sTeamMenmberContext.byOnline = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4 + 4 + 4 + 4);
        //    sTeamMenmberContext.dwSceneId = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4 + 4 + 4 + 4 + 4);
        //    sTeamMenmberContext.bIsVip = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4 + 4 + 4 + 4 + 4 + 4);
        //    sTeamMenmberContext.nCurHP = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4);
        //    sTeamMenmberContext.nMaxHP = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4);
        //    sTeamMenmberContext.nCurMP = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4);
        //    sTeamMenmberContext.nMaxMP = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4);
        //    sTeamMenmberContext.byFightReady = BitConverter.ToInt32(dataBuffer, offset + 8 + 4 + 19 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4);
        //}
                
        return sTeamMenmberContext;
    }

    public void UpdateValue(int index, int value)
    {        
        switch(index)
        {
			case 7:
				this.bIsVip = value;
				break;
			case 8:
				this.dwState = value;
				break;				
            case 9:
                this.nCurHP = value;
                break;
            case 10:
                this.nMaxHP = value;
                break;
            case 11:
                this.nCurMP = value;
                break;
            case 12:
                this.nMaxMP = value;
                break;
            case 13:
                this.nCurActiveLife = value;
                break;
            case 14:
                this.nCurPayMoney = value;
                break;
			case 15:
				this.nFashionID = value;
				break;
			case 16:
				this.nCurWeaponID = value;
				break;
        }     
    }

    public void SetFightReadyValue(int value)
    {
        byFightReady = value;
    }

    public int GetFightReadyValue()
    {
        return byFightReady;
    }

}


// 队伍信息更新
//MSG_TEAM_UPDATEPROP = 5,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamUpdateProp_SC : INotifyArgs
{
    public uint dwTeamID;		//队伍ID
    public ushort wProp;			//属性Index
    public int nValue;			//属性值

    public static SMsgTeamUpdateProp_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgTeamUpdateProp_SC>(dataBuffer);
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

// 队员信息更新
//MSG_TEAM_MEMBER_UPDATEPROP = 6,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamMemberUpdateProp_SC : INotifyArgs
{
    public uint dwTeamID;		//队伍ID
    public uint dwActorID;		//队员ActorID
    public ushort wProp;			//属性Index
    public int nValue;			//属性值

    public static SMsgTeamMemberUpdateProp_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgTeamMemberUpdateProp_SC>(dataBuffer);
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

// 加入队伍
//MSG_TEAM_MEMBER_JOIN = 7,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamMemberJoin_CS
{
	public byte byJoinType;		//加入队伍类型（0普通加入，1快速加入，2聊天栏加入队伍操作，好友邀请）
    public byte byJoinAnswer;	//加入应答，拒绝为0，同意为1
    public uint dwTeamID;		//队伍ID   队伍ID可以为0，则为快速加入队伍，自适应匹配
    public uint dwActorID;		//玩家ActorID

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_MEMBER_JOIN);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamMemberJoin_CS>(this);
        return pkg;
    }

    public static SMsgTeamMemberJoin_CS ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgTeamMemberJoin_CS>(dataBuffer);
    }
};


//MSG_TEAM_MEMBER_LEAVE = 8,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamCaptainJoin_CS
{
    public uint dwActorID;		// 玩家ID
    public uint dwCaptainID;	// 拥有队伍玩家ID

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_CAPTAIN_JOIN);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamCaptainJoin_CS>(this);
        return pkg;
    }
};


// 离开队伍
//MSG_TEAM_MEMBER_LEAVE = 8,
//[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
//struct SMsgTeamMemberLeave_CS
//{
//    public uint dwTeamID;		//队伍ID
//    public uint dwActorID;		//队员ActorID

//    public Package GeneratePackage()
//    {
//        Package pkg = new Package();
//        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_MEMBER_LEAVE);
//        pkg.Data = PackageHelper.StructToBytes<SMsgTeamMemberLeave_CS>(this);
//        return pkg;
//    }
//};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamMemberLeave_SC : INotifyArgs
{
    public uint dwTeamID;		//队伍ID
    public uint dwActorID;		//队员ActorID

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_MEMBER_LEAVE);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamMemberLeave_SC>(this);
        return pkg;
    }

    public static SMsgTeamMemberLeave_SC ParsePackage(byte[] dataBuffer)
    {     
        return PackageHelper.BytesToStuct<SMsgTeamMemberLeave_SC>(dataBuffer);
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

// 踢人
//MSG_TEAM_MEMBER_KICK = 9,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamMemberKick_CS
{
    public uint dwTeamID;			//队伍ID
    public uint dwActorID;			//队长ActorID
    public uint dwTargetActorID;	//踢出目标ActorID

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_MEMBER_KICK);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamMemberKick_CS>(this);
        return pkg;
    }
};

// 邀请
//MSG_TEAM_MEMBER_INVITE = 10,
//[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
//struct SMsgTeamMemberInvite_CS
//{
//    public uint dwTeamID;			//队伍ID
//    public uint dwActorID;			//邀请者ActorID
//    public uint dwTargetActorID;	//邀请目标ActorID

//    public Package GeneratePackage()
//    {
//        Package pkg = new Package();
//        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_MEMBER_INVITE);
//        pkg.Data = PackageHelper.StructToBytes<SMsgTeamMemberInvite_CS>(this);
//        return pkg;
//    }
//};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamMemberInvite_SC : INotifyArgs
{
    public uint dwTeamID;			//队伍ID
    public uint dwActorID;			//邀请者ActorID
    public uint dwTargetActorID;	//邀请目标ActorID
    public uint dwEctypeId;         //邀请参加的副本id
	public byte byEctypeIndex;		//邀请参加的副本编号
    public byte byEctypDiff;        //邀请参加的副本难度

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_MEMBER_INVITE);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamMemberInvite_SC>(this);
        return pkg;
    }

    public static SMsgTeamMemberInvite_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgTeamMemberInvite_SC>(dataBuffer);
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

// 寻找队伍/队长
//MSG_TEAM_FIND = 11,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamFind_CS
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	public byte[] _szCaptainName;		//队长名字[19]    

    public static Package GeneratePackage(string szCaptainName)
    {          
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_FIND);        
        //pkg.Data = PackageHelper.StructToBytes<SMsgTeamFind_CS>(this);
        pkg.Data = System.Text.Encoding.ASCII.GetBytes(szCaptainName);
        return pkg;
    }
};

// 队员准备
// MSG_TEAM_MEMBER_READY = 12
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamMemberReady_CS
{
    public uint dwTeamID;
    public uint dwActorID;			// 请求的队员ID

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_MEMBER_READY);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamMemberReady_CS>(this);
        return pkg;
    }
};

// MSG_TEAM_MEMBER_READY = 12
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgTeamMemberReadyResult_SC : INotifyArgs
{
    public uint dwActorID;
    public byte byResultCode;		// 准备结构

    public static SMsgTeamMemberReadyResult_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgTeamMemberReadyResult_SC>(dataBuffer);
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }    
};

// 错误信息返回
//MSG_TEAM_ERROR_CODE = 13,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamErrorCode_CS : INotifyArgs
{
	public byte	byErrorCode;		//错误信息编码

    public static SMsgTeamErrorCode_CS ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgTeamErrorCode_CS>(dataBuffer);
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};


// 更改队长, 成功失败通过消息更新下发
// MSG_TEAM_CHANGE_CAPTAIN = 14
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamChangeCaptain_CS
{
    public uint dwTeamID;			// 队伍Id
    public uint dwCaptainID;		// 队长Id
    public uint dwTeammateID;		// 队友Id

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_CHANGE_CAPTAIN);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamChangeCaptain_CS>(this);
        return pkg;
    }
};

// 更改目标副本
// MSG_TEAM_CHANGE_ECTYPE = 15
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamChangeEctype_CS
{
    public uint dwCaptainID;			// 队长ID
    public uint dwEctypeID;			// 更改的副本ID
    byte byType;				// 更改的副本难度

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_CHANGE_ECTYPE);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamChangeEctype_CS>(this);
        return pkg;
    }
};

// 快速加入
// MSG_TEAM_FAST_JOIN = 16
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTeamFastJoin_CS
{
    public uint dwActorId;			// 快速加入的玩家ActorId
    public uint dwEctypeId;			// 快速加入的副本ID
    public byte byEctypeDiff;		// 快速加入的副本难度

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, (short)TeamDefineManager.MSG_TEAM_FAST_JOIN);
        pkg.Data = PackageHelper.StructToBytes<SMsgTeamFastJoin_CS>(this);
        return pkg;
    }
};

//18
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgTeamInviteError_SC
{
    public uint dwTeamID;			//队伍ID
    public uint dwActorID;			//邀请者ActorID
    public uint dwTargetActorID;	//邀请目标ActorID
    public uint dwErrorMsg;			//邀请失败错误码

    public static SMsgTeamInviteError_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgTeamInviteError_SC>(dataBuffer);
    }
};

//下个当前副本队伍个数列表
//MSG_GET_TEAMNUMLIST		= 20
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgEctypeTeamNum_SC
{
	public byte	byEctypeNum;			//副本个数
	public STeamNumContext[] sTeamNumContext;

	public static SMsgEctypeTeamNum_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgEctypeTeamNum_SC sMsgEctypeTeamNum_SC = new SMsgEctypeTeamNum_SC();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgEctypeTeamNum_SC.byEctypeNum);
		sMsgEctypeTeamNum_SC.sTeamNumContext = new STeamNumContext[sMsgEctypeTeamNum_SC.byEctypeNum];
		for(int i =0 ;i< sMsgEctypeTeamNum_SC.byEctypeNum;i++)
		{
			sMsgEctypeTeamNum_SC.sTeamNumContext[i] = new STeamNumContext();
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgEctypeTeamNum_SC.sTeamNumContext[i].dwEctypeID);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgEctypeTeamNum_SC.sTeamNumContext[i].byTeamNum);
		}
		return sMsgEctypeTeamNum_SC;
	}

};

//副本队伍个数现场
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct STeamNumContext
{
	public int dwEctypeID;		//FB ID
	public byte  byTeamNum;		//队伍个数
};


[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgErrorCode_SC
{
    public byte byNum;
    public int[] iContent;

    public static SMsgErrorCode_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgErrorCode_SC sMsgErrorCode_SC = new SMsgErrorCode_SC();

        int of = 0;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgErrorCode_SC.byNum);
        sMsgErrorCode_SC.iContent = new int[sMsgErrorCode_SC.byNum];
        for (int i = 0; i < sMsgErrorCode_SC.iContent.Length; i++)
        {
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgErrorCode_SC.iContent[i]);
        }

        return sMsgErrorCode_SC;
    }
}

//MSG_CONFIRM_MATCHING	= 23
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgConfirmMatching_SC
{
	public int dwEctypeContainerID;	//副本id
	
	public static SMsgConfirmMatching_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgConfirmMatching_SC>(dataBuffer);
	}
};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgConfirmMatching_CS
{
	public byte	byConfirm;			// 0=不为所动 1=前往追杀
	
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, TeamDefineManager.MSG_MATCHING_CONFIRM);		
		pak.Data = PackageHelper.StructToBytes<SMsgConfirmMatching_CS>(this);
		return pak;
	}	
};
/// <summary>
///邀请好友pvp 
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPvpInviteFriend_CSC
{
	public uint dwActorID;			//// C:被邀请者的ActorID S:邀请者的ACTORID
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, TeamDefineManager.MSG_PVP_INVITE_FRIEND);		
		pak.Data = PackageHelper.StructToBytes<SMsgPvpInviteFriend_CSC>(this);
		return pak;
	}	
	public static SMsgPvpInviteFriend_CSC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgPvpInviteFriend_CSC>(dataBuffer);
	}
};
/// <summary>
///好友确认邀请
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct  SMsgPVPFriendConfirm_CS
{
	public uint dwActorID;			// 邀请者的ActorID
	public static SMsgPVPFriendConfirm_CS ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgPVPFriendConfirm_CS>(dataBuffer);
	}
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_TEAM, TeamDefineManager.MSG_PVP_FRIEND_CONFIRM);		
		pak.Data = PackageHelper.StructToBytes<SMsgPVPFriendConfirm_CS>(this);
		return pak;
	}
};

public struct SGroupMemberInfo
{
	public uint dwActorID;//角色id
	public uint dwVocation;//职业
	public uint dwLevel;//等级
	public uint dwFighting;//战力
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	public byte[]  szName;//名字
	public uint dwFashion;// 时装ID
	public uint dwWEAPON;	// 武器
}
//队伍同步
public struct SMsgPVPMathingSynInfo_SC
{
	public  byte  byNum;
	public SGroupMemberInfo[] GroupMemberInfo;
	public static SMsgPVPMathingSynInfo_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgPVPMathingSynInfo_SC sMsgPVPMathingSynInfo_SC = new SMsgPVPMathingSynInfo_SC();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgPVPMathingSynInfo_SC.byNum);
		sMsgPVPMathingSynInfo_SC.GroupMemberInfo = new SGroupMemberInfo[sMsgPVPMathingSynInfo_SC.byNum];
		for(int i =0 ;i< sMsgPVPMathingSynInfo_SC.byNum;i++)
		{
			sMsgPVPMathingSynInfo_SC.GroupMemberInfo[i] = new SGroupMemberInfo();
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgPVPMathingSynInfo_SC.GroupMemberInfo[i].dwActorID);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgPVPMathingSynInfo_SC.GroupMemberInfo[i].dwVocation);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgPVPMathingSynInfo_SC.GroupMemberInfo[i].dwLevel);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgPVPMathingSynInfo_SC.GroupMemberInfo[i].dwFighting);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgPVPMathingSynInfo_SC.GroupMemberInfo[i].szName,19);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgPVPMathingSynInfo_SC.GroupMemberInfo[i].dwFashion);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgPVPMathingSynInfo_SC.GroupMemberInfo[i].dwWEAPON);
		}
		return sMsgPVPMathingSynInfo_SC;
	}
};

// MSG_PVP_MATCHING_LEAVE	= 35
public struct SMsgPVPMathingLeave_SC
{
	public uint dwActorID;		// 退出者的ID
	public uint dwLeaderID;		// 组长ID	
	public static SMsgPVPMathingLeave_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgPVPMathingLeave_SC>(dataBuffer);
	}
};


