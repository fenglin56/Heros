using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System;

/// <summary>
/// ???¡¤?????¨¢????
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightFightTo
{
    public long    uidFighter;  //¡¤???UID
    public byte byFightCmd;
    public int nFightCode;
    public short xMouse;
    public short yMouse;

    public byte byNum;  //?¨²?¨²?¡ì????????

    public Package GenerateInputPackage(MasterMsgType masterMsgType, short subMsgType,long[] targetUids)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightFightTo>(this);

        for (int i = 0; i < targetUids.Length; i++)
        {
            //pkg.Data = pkg.Data.Concat(PackageHelper.StructToBytes<long>(targetUids[i])).ToArray();
            pkg.Data = pkg.Data.Concat(BitConverter.GetBytes(targetUids[i])).ToArray();
        }
        return pkg;
    }

    public static SMsgFightFightTo ParseResultPackage(byte[] dataBuffer, out SCmdImpactData[] targetDatas)
    {
        SMsgFightFightTo sMsgFightFightTo;
        int offset = 0;
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        var msgFightLength = Marshal.SizeOf(typeof(SMsgFightFightTo));
        var sCmdImpactDataLength = Marshal.SizeOf(typeof(SCmdImpactData));
        sMsgFightFightTo = PackageHelper.BytesToStuct<SMsgFightFightTo>(package.Data.Take(msgFightLength).ToArray());
        targetDatas = new SCmdImpactData[sMsgFightFightTo.byNum];
        offset += msgFightLength;
        for (int i = 0; i < sMsgFightFightTo.byNum; i++)
        {
            targetDatas[i] = PackageHelper.BytesToStuct<SCmdImpactData>(package.Data.Skip(offset).Take(sCmdImpactDataLength).ToArray());
            offset += sCmdImpactDataLength;
        }

        return sMsgFightFightTo;
    }  
};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SCmdImpactData
{
    public long uidTarget;
    public byte byCmdType;
    public short nAttprop;
    public int nValuse;
}
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionColdWorkHead_SC
{
    public long lMasterID;			//??¡À¨ºUID
    public byte bColdNum;			//?¨¨??????????
}
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionColdWork_SC
{
    public byte byClassID;		// ?????¨®?¨¤???????????¡¤?????¨¬????
    public uint dwColdID;		// ????ID
    public uint dwColdTime;		// ?????¡À??(????)
}

public struct SmsgActionColdWork : INotifyArgs
{
    public SMsgActionColdWorkHead_SC sMsgActionColdWorkHead_SC;
    public SMsgActionColdWork_SC[] sMsgActionColdWork_SCs;

    public int GetEventArgsType()
    {
        return 0;
    }
}

//¡Á?¡¤????¡¤????server?¨²client??client->Server?????????¨¢????
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgBattleCommand : INotifyArgs
{
    public Int64 uidFighter;			//???¡¤?????¨¢????
    public Int64 uidTarget;			    //???¡Â??????¡À¨º
    public UInt32 bulletIndex;				//¡Á????¨°??
    public int nFightCode;			//????ID
    public float xPlayer;					//??¡¤???????X
    public float yPlayer;					//??¡¤???????Y
    public float xMouse;				//¡¤??¡ì???¡Â ????x
    public float yMouse;				//¡¤??¡ì???¡Â ????y
    public float xDirect;					//¡¤??¨°X
    public float yDirect;					//¡¤??¨°Y

    public static SMsgBattleCommand ParseCommandPackage(byte[] pkgData)
    {
        return PackageHelper.BytesToStuct<SMsgBattleCommand>(pkgData);
    }
    public Package GeneratePackage(MasterMsgType masterType,short subMsg)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterType, subMsg);
        pkg.Data = PackageHelper.StructToBytes<SMsgBattleCommand>(this);

        return pkg;
    }

    public int GetEventArgsType()
    {
        return 0;
    }
};

//???¡¤?¡ì??server?¨²client
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgBattleCalculateEffect_SC : INotifyArgs
{
    public Int64 uidFighter;       //被作用者UID

    public Int64 uidEffectParam;    //效果参数
    public uint EffectType;       //对应DFightNetMsg.h文件FightEffectType 枚举类型

    public uint BulletTemplateID;	//????ID;
    public int Value;

    public static SMsgBattleCalculateEffect_SC ParseResultPackage(byte[] pkgData)
    {
        return PackageHelper.BytesToStuct<SMsgBattleCalculateEffect_SC>(pkgData);
    }
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_CALCULATE_EFFECT);
        pkg.Data = PackageHelper.StructToBytes<SMsgBattleCalculateEffect_SC>(this);

        return pkg;
    }
    public int GetEventArgsType()
    {
        return 0;
    }
};

//MSG_FIGHT_BATTLE_BLOODSUCKING			=11,				//ÎüÑªÇëÇó
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightBloodSucking_CS
{
    public Int64 uidFighter;		//ÎüÑªÕßµÄÊµÌåID
    public Int64 uidTarget;		//Ä¿±ê(±»É±ËÀµÄ¹ÖÎïÊµÌåID)
    public int nParam;			//ÎüÑª²ÎÊý ²Î¿¼SMsgActionDie_SC

    public Package GeneratePackage(MasterMsgType masterType, short subMsg)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterType, subMsg);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightBloodSucking_CS>(this);

        return pkg;
    }
};

//????¡À??¡Â??server?¨²client
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgBattleBeatBack_SC : INotifyArgs
{
    public Int64 uidFighter;		//¡À??¡Â??????????ID
    public float PosX;              //¡À??¡Â???¡ãx¡Á?¡À¨º(????10)
    public float PosY;              //¡À??¡Â???¡ãy¡Á?¡À¨º(????10)
    public float DirX;						//¡¤??¨°x
    public float DirY;						//¡¤??¨°y
    public int speed;						//?????????¡Á???? ????10??
    public int Accelerated;					//???????????¡Á???? ????10??
    public int time;								//?¡À??(????)

    public static SMsgBattleBeatBack_SC ParseResultPackage(byte[] pkgData)
    {
        return PackageHelper.BytesToStuct<SMsgBattleBeatBack_SC>(pkgData);
    }
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_BEAT_BACK);
        pkg.Data = PackageHelper.StructToBytes<SMsgBattleBeatBack_SC>(this);

        return pkg;
    }
    public int GetEventArgsType()
    {
        return 0;
    }
};
//[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
//public struct SMsgBattleBeatBack_SC : INotifyArgs
//{
//    public Int64 uidFighter;		//¡À??¡Â??????????ID
//    public float PosX;              //¡À??¡Â???¡ãx¡Á?¡À¨º(????10)
//    public float PosY;              //¡À??¡Â???¡ãy¡Á?¡À¨º(????10)
//    public int Angel;						//¡À??¡Â??¡¤??¨°??????1000??
//    public int speed;						//??????¡¤??¡Á??????
//    public int Accelerated;					//????????¡¤??¡Á??????
//    public int time;								//?¡À??(????)

//    public static SMsgBattleBeatBack_SC ParseResultPackage(byte[] pkgData)
//    {
//        return PackageHelper.BytesToStuct<SMsgBattleBeatBack_SC>(pkgData);
//    }
//    public Package GeneratePackage()
//    {
//        Package pkg = new Package();
//        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_BEAT_BACK);
//        pkg.Data = PackageHelper.StructToBytes<SMsgBattleBeatBack_SC>(this);

//        return pkg;
//    }
//    public int GetEventArgsType()
//    {
//        return 0;
//    }
//};

// ?¡Â¡¤??¡ì??????
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightHitFly_SC : INotifyArgs
{
    public Int64 uidFighter;		//¡À??¡Â¡¤?????????ID
    public float hitedPosX;		//¡À??¡Â¡¤??????¡À?¡ã¡Á?¡À¨º
    public float hitedPosY;
    public float directionX;		//?¡Â¡¤??¨°??
    public float directionY;
    public int lSpeed;			//????
    public int Accelerated;	    //??????
    public int hSpedd;			//????

    public static SMsgFightHitFly_SC ParseResultPackage(byte[] pkgData)
    {
        return PackageHelper.BytesToStuct<SMsgFightHitFly_SC>(pkgData);
    }
    //public Package GeneratePackage()
    //{
    //    Package pkg = new Package();
    //    pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_HIT_FLY);
    //    pkg.Data = PackageHelper.StructToBytes<SMsgFightHitFly_SC>(this);

    //    return pkg;
    //}

    public int GetEventArgsType()
    {
        return 0;
        //throw new NotImplementedException();
    }
};


//????????¡Á???index
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgSyncBulletID_SC
{
    public Int64 uidFighter;    //¡À??¡Â??????????ID
    public UInt32 BulletIndex;   // ¡Á???Index

    public static SMsgSyncBulletID_SC ParseResultPackage(byte[] pkgData)
    {
        return PackageHelper.BytesToStuct<SMsgSyncBulletID_SC>(pkgData);
    }
}

//??????????????¡Á???
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgDestoryBullet_SC
{
    public Int64 uidFighter;    //¡Á???????????????ID
    public UInt32 BulletIndex;   // ¡Á???Index

    public static SMsgDestoryBullet_SC ParseResultPackage(byte[] pkgData)
    {
        return PackageHelper.BytesToStuct<SMsgDestoryBullet_SC>(pkgData);
    }
}
//?????¨°??(S->C)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightBreakSkill_SC : INotifyArgs
{
    public Int64 uidFighter;    //????ID
    public int SkillID;   // ????ID

    public static SMsgFightBreakSkill_SC ParseResultPackage(byte[] pkgData)
    {
        return PackageHelper.BytesToStuct<SMsgFightBreakSkill_SC>(pkgData);
    }

    public int GetEventArgsType()
    {
        return 0;
    }
}
//?????¨°??(C->S)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightBreakSkill_CS
{
    public int SkillID;   // ????ID

    public Package GeneratePackage(MasterMsgType masterType, short subMsg)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterType, subMsg);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightBreakSkill_CS>(this);

        return pkg;
    }
}

/// <summary>
/// ???¡Â??????
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightBatterCount_SC
{
    public ushort batterNum;
    //public byte batterLev;
    public static SMsgFightBatterCount_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgFightBatterCount_SC sMsgFightBatterCount_SC;
        sMsgFightBatterCount_SC = PackageHelper.BytesToStuct<SMsgFightBatterCount_SC>(dataBuffer);
        return sMsgFightBatterCount_SC;
    }
};
/// <summary>
/// Á¬É±
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightKillCount_SC
{
    public ushort wKillNum;
    //public byte batterLev;
    public static SMsgFightKillCount_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgFightKillCount_SC sMsgFightKillCount_SC;
        sMsgFightKillCount_SC = PackageHelper.BytesToStuct<SMsgFightKillCount_SC>(dataBuffer);
        return sMsgFightKillCount_SC;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightChangeDirect_CS
{
    public float DirX;						//¡¤??¨°x
    public float DirY;						//¡¤??¨°y
    public Package GeneratePackage(MasterMsgType masterType, short subMsg)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterType, subMsg);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightChangeDirect_CS>(this);

        return pkg;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightChangeDirect_SC:INotifyArgs
{
    public Int64 uidFighter;
    public float DirX;						//¡¤??¨°x
    public float DirY;						//¡¤??¨°y
    public static SMsgFightChangeDirect_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgFightChangeDirect_SC sMsgFightChangeDirect_SC;
        sMsgFightChangeDirect_SC = PackageHelper.BytesToStuct<SMsgFightChangeDirect_SC>(dataBuffer);
        return sMsgFightChangeDirect_SC;
    }

    public int GetEventArgsType()
    {
        return 0;
    }
};
//????¡À??¨¹??
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgBattleBeAdsorb_SC : INotifyArgs
{
    public Int64 uidFighter;		//¡À??¨¹??????????ID
    public float PosX;              //¡À??¨¹???¡ãx¡Á?¡À¨º(????10)
    public float PosY;              //¡À??¨¹???¡ãy¡Á?¡À¨º(????10)
    public float DirX;						//¡¤??¨°x
    public float DirY;						//¡¤??¨°y
    public int speed;						//?????????¡Á???? ????10??
    public int time;								//?¡À??(????)

    public static SMsgBattleBeAdsorb_SC ParseResultPackage(byte[] pkgData)
    {
        return PackageHelper.BytesToStuct<SMsgBattleBeAdsorb_SC>(pkgData);
    }
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_BEAT_BACK);
        pkg.Data = PackageHelper.StructToBytes<SMsgBattleBeAdsorb_SC>(this);

        return pkg;
    }
    public int GetEventArgsType()
    {
        return 0;
    }
};

public struct BossShieldStruct : INotifyArgs
{
    public Int64 uidFighter;  //BossId;
    public static BossShieldStruct ParseResultPackage(byte[] pkgData)
    {
        long uidFighter = System.BitConverter.ToInt64(pkgData, 0);

        return new BossShieldStruct() { uidFighter = uidFighter };
    }
    public int GetEventArgsType()
    {
        return 0;
    }
}

// ???¨ª?¡ì??????
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightHorde_SC : INotifyArgs
{
    public Int64 uidFighter;
    public float HitedPosX;              //¡À????¨ª????x¡Á?¡À¨º(????10)
    public float HitedPosY;              //¡À????¨ª????y¡Á?¡À¨º(????10)
    public int HordeTime;						//???¨ª?¡À??
    public static SMsgFightHorde_SC ParseResultPackage(byte[] pkgData)
    {
        SMsgFightHorde_SC sMsgFightHorde_SC;
        sMsgFightHorde_SC = PackageHelper.BytesToStuct<SMsgFightHorde_SC>(pkgData);
        return sMsgFightHorde_SC;
    }
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_COMMAND);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightHorde_SC>(this);

        return pkg;
    }
    public int GetEventArgsType()
    {
        return 0;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightCommand_SC: INotifyArgs
{
	public Int64	uidFighter;					//Õ½¶·Ö¸ÁîÌá½»Õß
	public Int64	uidTarget;					//Õ½¶·Ä¿±ê
	public int			nFightCode;					//¼¼ÄÜID
	public float		xMouse;						//·¶Î§¹¥»÷ ÖÐÐÄx
	public float		yMouse;						//·¶Î§¹¥»÷ ÖÐÐÄy
	public float		xDirect;					//·½ÏòX
	public float		yDirect;					//·½ÏòY
	
	public static SMsgFightCommand_SC ParseResultPackage(byte[] pkgData)
	{
		SMsgFightCommand_SC sMsgFightCommand_SC;
		sMsgFightCommand_SC = PackageHelper.BytesToStuct<SMsgFightCommand_SC>(pkgData);
		return sMsgFightCommand_SC;
	}
	
	 public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_COMMAND_SINGLE);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightCommand_SC>(this);

        return pkg;
    }
	
	public int GetEventArgsType()
    {
        return 0;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightCommand_CS
{
	public Int64	uidFighter;					//Õ½¶·Ö¸ÁîÌá½»Õß
	public int			nFightCode;					//¼¼ÄÜID
	public float		fighterPosX;
	public float       fighterPosY; //Ìá½»Õßµ±Ç°×ø±ê
	public byte		byType;						//Ö¸ÁîÀàÐÍ 1:¿ªÊ¼ 0:½áÊø
	
	public static SMsgFightCommand_CS ParseResultPackage(byte[] pkgData)
	{
		SMsgFightCommand_CS sMsgFightCommand_CS;
		sMsgFightCommand_CS = PackageHelper.BytesToStuct<SMsgFightCommand_CS>(pkgData);
		return sMsgFightCommand_CS;
	}
	
	 public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_COMMAND_SINGLE);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightCommand_CS>(this);

        return pkg;
    }
	
	public int GetEventArgsType()
    {
        return 0;
    }
	
};

//Õ½¶·Ð§¹ûÏÖ³¡(µ¥»ú°æ ¿Í»§¶ËÌá½»)(¿Í»§¶ËÌá½»¸ø·þÎñÆ÷×Óµ¯ID¼ÆËãÉËº¦)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct  SMsgFightEffect_CS
{
	public Int64	uidFighter;
								
	public UInt32	BulletTemplateID;	//±»»÷ÖÐµÄ×Óµ¯ID
	public float 	BulletPosX;
	public float 	BulletPosY;
	public UInt32   SkillId;
	public UInt32   DamageID;
	public byte		bySrcLive;	//攻击者是否存在(存在的话 bySrcLive = 1 不存在 0)
	public byte     byBeFightedNum;

    public List<Int64> uidBeFightedList;  //±»Ê°È¡£¬±»¹¥»÷µÄ¶ÔÏóUID
	
	 public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_CALCULATE_EFFECT);
        pkg.Data = BitConverter.GetBytes(uidFighter)
            .Concat(BitConverter.GetBytes(BulletTemplateID))
                .Concat(BitConverter.GetBytes(BulletPosX))
                .Concat(BitConverter.GetBytes(BulletPosY))
                .Concat(BitConverter.GetBytes(SkillId))
                .Concat(BitConverter.GetBytes(DamageID))
				.Concat(new byte[] { this.bySrcLive })
                .Concat(new byte[] { this.byBeFightedNum }).ToArray();
        for(int i = 0; i < uidBeFightedList.Count; i++)
        {
            pkg.Data = pkg.Data.Concat(BitConverter.GetBytes(uidBeFightedList[i])).ToArray();
        }
        return pkg;
    }

	public int GetEventArgsType()
    {
        return 0;
    }
	
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct  SMsgDeadBulletFightEffect_CS
{
	public Int64	uidFighter;
	
	public UInt32	BulletTemplateID;	//±»»÷ÖÐµÄ×Óµ¯ID
	public float 	BulletPosX;
	public float 	BulletPosY;
	public UInt32   SkillId;
	public UInt32   DamageID;
	public byte		bySrcLive;	//攻击者是否存在(存在的话 bySrcLive = 1 不存在 0)
	public byte     byBeFightedNum;
	public byte     byPropNum;	//自身属性现场 属性个数
	public int[]    dwProp;		//属性的值
	public List<Int64> uidBeFightedList;

	public Package GeneratePackage()
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_CALCULATE_EFFECT);
		pkg.Data = BitConverter.GetBytes(uidFighter)
			.Concat(BitConverter.GetBytes(BulletTemplateID))
				.Concat(BitConverter.GetBytes(BulletPosX))
				.Concat(BitConverter.GetBytes(BulletPosY))
				.Concat(BitConverter.GetBytes(SkillId))
				.Concat(BitConverter.GetBytes(DamageID))
				.Concat(new byte[] { this.bySrcLive})
				.Concat(new byte[] { this.byBeFightedNum })
				.Concat(new byte[] { this.byPropNum})
				.ToArray();
		for(int i = 0; i<this.byPropNum;i++)
		{
			pkg.Data = pkg.Data.Concat(BitConverter.GetBytes(dwProp[i])).ToArray();
		}
		for(int i = 0; i < this.uidBeFightedList.Count; i++)
		{
			//Debug.Log("死亡子弹结算:"+uidBeFightedList[i]);
			pkg.Data = pkg.Data.Concat(BitConverter.GetBytes(uidBeFightedList[i])).ToArray();
		}
		return pkg;
	}
}

//怪物死亡储存属性现场
public struct SMsgFightSaveProp_CS
{
	public byte byPropNum;
	public int[] nProp ;
}


// »÷ÍËÐ§¹ûÏÖ³¡(µ¥»ú°æ ¿Í»§¶ËÌá½»)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightBeatBack_CS
{
	public Int64	uidFighter;		//±»»÷ÍËÕßµÄÊµÌåID
	public float		hitedPosX;
	public float       hitedPosY;//±»»÷ÍËÕßµ±Ç°×ø±ê
	public byte		byType;			//±»»÷ÍËÀàÐÍ 1:¿ªÊ¼ 0:½áÊø
	
	public static SMsgFightBeatBack_CS ParseResultPackage(byte[] pkgData)
	{
		SMsgFightBeatBack_CS sMsgFightBeatBack_CS;
		sMsgFightBeatBack_CS = PackageHelper.BytesToStuct<SMsgFightBeatBack_CS>(pkgData);
		return sMsgFightBeatBack_CS;
	}
	
	 public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_BEAT_BACK);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightBeatBack_CS>(this);

        return pkg;
    }
	
	public int GetEventArgsType()
    {
        return 0;
    }
};


// »÷·ÉÐ§¹ûÏÖ³¡(µ¥»ú°æ ¿Í»§¶ËÌá½»)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightHitFly_CS
{
	public Int64	uidFighter;		//±»»÷·ÉÕßµÄÊµÌåID
	public float       hitedPosX;
	public float		hitedPosY;		//±»»÷·ÉÕßµÄµ±Ç°×ø±ê(ÓÃÓÚ·þÎñÆ÷×ø±êÐÞÕý)
	public byte		byType;			//±»»÷·ÉÀàÐÍ 1:¿ªÊ¼ 0:½áÊø
	
	public static SMsgFightHitFly_CS ParseResultPackage(byte[] pkgData)
	{
		SMsgFightHitFly_CS sMsgFightHitFly_CS;
		sMsgFightHitFly_CS = PackageHelper.BytesToStuct<SMsgFightHitFly_CS>(pkgData);
		return sMsgFightHitFly_CS;
	}
	
	 public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_HIT_FLY);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightHitFly_CS>(this);

        return pkg;
    }
	
	public int GetEventArgsType()
    {
        return 0;
    }
	
};

// Îü¸½Ð§¹ûÏÖ³¡(µ¥»ú°æ ¿Í»§¶ËÌá½»)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightAdsorption_CS
{
	public Int64	uidFighter;		//±»Îü¸½ÕßµÄÊµÌåID
	public float		hitedPosX;	
	public float		hitedPosY;	//±»Îü¸½ÕßµÄµ±Ç°×ø±ê
	public byte		byType;			//±»Îü¸½ÀàÐÍ 1:¿ªÊ¼ 0:½áÊø
	
	public static SMsgFightAdsorption_CS ParseResultPackage(byte[] pkgData)
	{
		SMsgFightAdsorption_CS sMsgFightAdsorption_CS;
		sMsgFightAdsorption_CS = PackageHelper.BytesToStuct<SMsgFightAdsorption_CS>(pkgData);
		return sMsgFightAdsorption_CS;
	}
	
	 public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_ADSORB);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightAdsorption_CS>(this);

        return pkg;
    }
	
	public int GetEventArgsType()
    {
        return 0;
    }
	
};

// ¶¨ÉíÐ§¹ûÏÖ³¡(µ¥»ú°æ ¿Í»§¶ËÌá½»)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightHorde_CS
{
	public Int64	uidFighter;		//±»¶¨ÉíÕßµÄÊµÌåID
	public float		hitedPosX;		
	public float		hitedPosY;//±»¶¨ÉíÕßµÄµ±Ç°×ø±ê
	public byte		byType;			//±»¶¨ÉíÀàÐÍ 1:¿ªÊ¼ 0:½áÊø
	
	public static SMsgFightHorde_CS ParseResultPackage(byte[] pkgData)
	{
		SMsgFightHorde_CS sMsgFightHorde_CS;
		sMsgFightHorde_CS = PackageHelper.BytesToStuct<SMsgFightHorde_CS>(pkgData);
		return sMsgFightHorde_CS;
	}
	
	 public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_HORDE);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightHorde_CS>(this);

        return pkg;
    }
	
	public int GetEventArgsType()
    {
        return 0;
    }
	
	
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightClimbs_CS
{
    public Int64    uidFighter;     //ÊµÌåID
    public float        hitedPosX;      
    public float        hitedPosY;//µ±Ç°×ø±ê
    public byte     byType;         //ÀàÐÍ 1:¿ªÊ¼ 0:½áÊø
    
    public static SMsgFightClimbs_CS ParseResultPackage(byte[] pkgData)
    {
        SMsgFightClimbs_CS sMsgFightClimbs_CS;
        sMsgFightClimbs_CS = PackageHelper.BytesToStuct<SMsgFightClimbs_CS>(pkgData);
        return sMsgFightClimbs_CS;
    }
    
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_CLIMBS);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightClimbs_CS>(this);
        
        return pkg;
    }
    
    public int GetEventArgsType()
    {
        return 0;
    }
    
    
};

//ÕÙ»½×Óµ¯Ð§¹ûÏÖ³¡(µ¥»ú°æ ¿Í»§¶ËÌá½»)(¿Í»§¶ËÌá½»¸ø·þÎñÆ÷×Óµ¯IDÕÙ»½¹ÖÎï)
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct  SMsgFightSummonBullet_CS
{
	public Int64	uidFighter;
	public UInt32		MonsterTemplateID;							//±»»÷ÖÐµÄ×Óµ¯ID
	public float		BulletPosX;			
	public float		BulletPosY;		//×Óµ¯Î»ÖÃ
    public float        BulletDirX;
    public float        BulletDirY;
	
	public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_SUMMON_BULLET);
        pkg.Data = PackageHelper.StructToBytes<SMsgFightSummonBullet_CS>(this);

        return pkg;
    }
	
	public int GetEventArgsType()
    {
        return 0;
    }
	
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightAdsorptionEx_SC : INotifyArgs
{
	public Int64		uidFighter;		//±»Îü¸½ÕßµÄÊµÌåID
	public float		ptCenterPosX;	
	public float 		ptCenterPosY;//ÖÐÐÄÎ»ÖÃ
	public UInt32		dwRadius; //Í£Ö¹°ë¾¶
	public UInt32		dwSpeed;		//Îü¸½ËÙ¶È
	public UInt32		dwTime;			//Îü¸½Ê±¼ä
	
	
	public static SMsgFightAdsorptionEx_SC ParseResultPackage(byte[] pkgData)
	{
		SMsgFightAdsorptionEx_SC sMsgFightAdsorptionEx_SC;
		sMsgFightAdsorptionEx_SC = PackageHelper.BytesToStuct<SMsgFightAdsorptionEx_SC>(pkgData);
		return sMsgFightAdsorptionEx_SC;
	}
	
	public int GetEventArgsType()
    {
        return 0;
    }
};


[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEffectContextNum_SC
{
    public byte byContextNum;
    public List<SMsgBattleCalculateEffect_SC> list;
    
    public static SMsgEffectContextNum_SC ParseResultPackage(byte[] pkgData)
    {
        int offset = 0;
        int singleContextLength = Marshal.SizeOf(typeof(SMsgBattleCalculateEffect_SC));
        
        SMsgEffectContextNum_SC sMsgEffectContextNum_SC;
        offset += PackageHelper.ReadData(pkgData.Skip(offset).ToArray(), out sMsgEffectContextNum_SC.byContextNum); 
        sMsgEffectContextNum_SC.list = new List<SMsgBattleCalculateEffect_SC>();
        for(int i = 0; i < sMsgEffectContextNum_SC.byContextNum; i++)
        {
            SMsgBattleCalculateEffect_SC sMsgBattleCalculateEffect_SC = SMsgBattleCalculateEffect_SC.ParseResultPackage(pkgData.Skip(offset).Take(singleContextLength).ToArray());
            sMsgEffectContextNum_SC.list.Add(sMsgBattleCalculateEffect_SC);
            offset += singleContextLength;
        }
        return sMsgEffectContextNum_SC;
        
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgBeatBackContextNum_SC
{
    public byte byContextNum;
    public List<SMsgBattleBeatBack_SC> list;

    public static SMsgBeatBackContextNum_SC ParseResultPackage(byte[] pkgData)
    {
        int offset = 0;
        int singleContextLength = Marshal.SizeOf(typeof(SMsgBattleBeatBack_SC));

        SMsgBeatBackContextNum_SC sMsgBeatBackContextNum_SC;
        offset += PackageHelper.ReadData(pkgData.Skip(offset).ToArray(), out sMsgBeatBackContextNum_SC.byContextNum); 
        sMsgBeatBackContextNum_SC.list = new List<SMsgBattleBeatBack_SC>();
        for(int i = 0; i < sMsgBeatBackContextNum_SC.byContextNum; i++)
        {
            SMsgBattleBeatBack_SC sMsgBattleBeatBack_SC = SMsgBattleBeatBack_SC.ParseResultPackage(pkgData.Skip(offset).Take(singleContextLength).ToArray());
            sMsgBeatBackContextNum_SC.list.Add(sMsgBattleBeatBack_SC);
            offset += singleContextLength;
        }
        return sMsgBeatBackContextNum_SC;

    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgHitFlyContextNum_SC
{
    public byte byContextNum;
    public List<SMsgFightHitFly_SC> list;

    public static SMsgHitFlyContextNum_SC ParseResultPackage(byte[] pkgData)
    {
        int offset = 0;
        int singleContextLength = Marshal.SizeOf(typeof(SMsgFightHitFly_SC));
        
        SMsgHitFlyContextNum_SC sMsgHitFlyContextNum_SC;
        offset += PackageHelper.ReadData(pkgData.Skip(offset).ToArray(), out sMsgHitFlyContextNum_SC.byContextNum); 
        sMsgHitFlyContextNum_SC.list = new List<SMsgFightHitFly_SC>();
        for(int i = 0; i < sMsgHitFlyContextNum_SC.byContextNum; i++)
        {
            SMsgFightHitFly_SC sMsgFightHitFly_SC = SMsgFightHitFly_SC.ParseResultPackage(pkgData.Skip(offset).Take(singleContextLength).ToArray());
            sMsgHitFlyContextNum_SC.list.Add(sMsgFightHitFly_SC);
            offset += singleContextLength;
        }
        return sMsgHitFlyContextNum_SC;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgAdsorptionContextNum_SC
{
    public byte byContextNum;
    public List<SMsgBattleBeAdsorb_SC> list;

    public static SMsgAdsorptionContextNum_SC ParseResultPackage(byte[] pkgData)
    {
        int offset = 0;
        int singleContextLength = Marshal.SizeOf(typeof(SMsgBattleBeAdsorb_SC));
        
        SMsgAdsorptionContextNum_SC sMsgAdsorptionContextNum_SC;
        offset += PackageHelper.ReadData(pkgData.Skip(offset).ToArray(), out sMsgAdsorptionContextNum_SC.byContextNum); 
        sMsgAdsorptionContextNum_SC.list = new List<SMsgBattleBeAdsorb_SC>();
        for(int i = 0; i < sMsgAdsorptionContextNum_SC.byContextNum; i++)
        {
            SMsgBattleBeAdsorb_SC sMsgBattleBeAdsorb_SC = SMsgBattleBeAdsorb_SC.ParseResultPackage(pkgData.Skip(offset).Take(singleContextLength).ToArray());
            sMsgAdsorptionContextNum_SC.list.Add(sMsgBattleBeAdsorb_SC);
            offset += singleContextLength;
        }
        return sMsgAdsorptionContextNum_SC;
    }

};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgHordeContextNum_SC
{
    public byte byContextNum;
    public List<SMsgFightHorde_SC> list;

    public static SMsgHordeContextNum_SC ParseResultPackage(byte[] pkgData)
    {
        int offset = 0;
        int singleContextLength = Marshal.SizeOf(typeof(SMsgFightHorde_SC));
        
        SMsgHordeContextNum_SC sMsgHordeContextNum_SC;
        offset += PackageHelper.ReadData(pkgData.Skip(offset).ToArray(), out sMsgHordeContextNum_SC.byContextNum); 
        sMsgHordeContextNum_SC.list = new List<SMsgFightHorde_SC>();
        for(int i = 0; i < sMsgHordeContextNum_SC.byContextNum; i++)
        {
            SMsgFightHorde_SC sMsgFightHorde_SC = SMsgFightHorde_SC.ParseResultPackage(pkgData.Skip(offset).Take(singleContextLength).ToArray());
            sMsgHordeContextNum_SC.list.Add(sMsgFightHorde_SC);
            offset += singleContextLength;
        }
        return sMsgHordeContextNum_SC;
    }
};


[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgAdsorptionExContextNum_SC
{
    public byte byContextNum;
    public List<SMsgFightAdsorptionEx_SC> list;

    public static SMsgAdsorptionExContextNum_SC ParseResultPackage(byte[] pkgData)
    {
        int offset = 0;
        int singleContextLength = Marshal.SizeOf(typeof(SMsgFightAdsorptionEx_SC));
        
        SMsgAdsorptionExContextNum_SC sMsgAdsorptionExContextNum_SC;
        offset += PackageHelper.ReadData(pkgData.Skip(offset).ToArray(), out sMsgAdsorptionExContextNum_SC.byContextNum); 
        sMsgAdsorptionExContextNum_SC.list = new List<SMsgFightAdsorptionEx_SC>();
        for(int i = 0; i < sMsgAdsorptionExContextNum_SC.byContextNum; i++)
        {
            SMsgFightAdsorptionEx_SC sMsgFightAdsorptionEx_SC = SMsgFightAdsorptionEx_SC.ParseResultPackage(pkgData.Skip(offset).Take(singleContextLength).ToArray());
            sMsgAdsorptionExContextNum_SC.list.Add(sMsgFightAdsorptionEx_SC);
            offset += singleContextLength;
        }
        return sMsgAdsorptionExContextNum_SC;
    }

};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEffectContextNum_CS
{
    public byte byContextNum;
    public List<SMsgFightEffect_CS> list;

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_CALCULATE_EFFECT);
        pkg.Data = new byte[]{this.byContextNum};
        for(int i = 0; i < byContextNum; i++)
        {
            pkg.Data = pkg.Data.Concat(PackageHelper.StructToBytes(list[i])).ToArray();
        }
        return pkg;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgBeatBackContextNum_CS
{
    public byte byContextNum;
    public List<SMsgFightBeatBack_CS> list;

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_BEAT_BACK);
        pkg.Data = new byte[]{this.byContextNum};
        for(int i = 0; i < byContextNum; i++)
        {
            pkg.Data = pkg.Data.Concat(PackageHelper.StructToBytes(list[i])).ToArray();
        }
        return pkg;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgHitFlyContextNum_CS
{
    public byte byContextNum;
    public List<SMsgFightHitFly_CS> list;

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_HIT_FLY);
        pkg.Data = new byte[]{this.byContextNum};
        for(int i = 0; i < byContextNum; i++)
        {
            pkg.Data = pkg.Data.Concat(PackageHelper.StructToBytes(list[i])).ToArray();
        }
        return pkg;
    }

};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgAdsorptionContextNum_CS
{
    public byte byContextNum;
    public List<SMsgFightAdsorption_CS> list;

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_ADSORB);
        pkg.Data = new byte[]{this.byContextNum};
        for(int i = 0; i < byContextNum; i++)
        {
            pkg.Data = pkg.Data.Concat(PackageHelper.StructToBytes(list[i])).ToArray();
        }
        return pkg;
    }
};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightTeleport_CSC :INotifyArgs
{
    public long uidFighter;
    public float ptPosX;
    public float ptPosY;	//±»Îü¸½ÕßµÄµ±Ç°×ø±ê
    public float ptDirectX;
    public float ptDirectY;	//±»Îü¸½ÕßµÄµ±Ç°×ø±ê
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_TELEPORT);
        pkg.Data = PackageHelper.StructToBytes <SMsgFightTeleport_CSC>(this);
        return pkg;
    }
    public static SMsgFightTeleport_CSC ParseResultPackage(byte[] pkgData)
    {
        SMsgFightTeleport_CSC sMsgFightTeleport_CSC;
        sMsgFightTeleport_CSC = PackageHelper.BytesToStuct<SMsgFightTeleport_CSC>(pkgData);
        return sMsgFightTeleport_CSC;
    }

    public int GetEventArgsType()
    {
        return 0;
    }
};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgHordeContextNum_CS
{
    public byte byContextNum;
    public List<SMsgFightHorde_CS> list;

    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_HORDE);
        pkg.Data = new byte[]{this.byContextNum};
        for(int i = 0; i < byContextNum; i++)
        {
            pkg.Data =  pkg.Data.Concat(PackageHelper.StructToBytes(list[i])).ToArray();
        }
        return pkg;
    }
};

//MSG_FIGHT_BATTLE_MISSEFFECT		=		24
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightMissEffect_CS
{
	public Int64 uidFighter;		//攻击者UID
	public Int64 uidMisser;			//闪避者UID
	public Package GeneratePackage()
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_FIGHT, (short)FightDefineManager.MSG_FIGHT_BATTLE_MISSEFFECT);
		pkg.Data = PackageHelper.StructToBytes<SMsgFightMissEffect_CS>(this);
		return pkg;
	}
};



