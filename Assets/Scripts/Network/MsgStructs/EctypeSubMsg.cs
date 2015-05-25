using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Text;

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct EEctypeMemberFields
{
	//ECTYPE_MEMBER_FIELD_VISIBLEBGN;		// 可见属性

	public int ECTYPE_MEMBER_FIELD_ECTYPECONTAINERID;		// 副本容器ID
	public int ECTYPE_MEMBER_FIELD_YAONVSKILLTIMES;			// 技能使用次数	
	public int ECTYPE_MEMBER_FIELD_MEDICAMENTTIMES;			// 药品使用次数
	public int ECTYPE_MEMBER_FIELD_RELIVETIMES;			// 复活次数
	
	public static EEctypeMemberFields PrasePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<EEctypeMemberFields>(dataBuffer);
	}

	public EEctypeMemberFields SetValue(int index, int value)
	{
		var bytes = PackageHelper.StructToBytes<EEctypeMemberFields>(this);
		
		int offset = index * 4;
		var source = BitConverter.GetBytes(value);
		bytes[offset] = source[0];
		bytes[offset + 1] = source[1];
		bytes[offset + 2] = source[2];
		bytes[offset + 3] = source[3];
		
		return PackageHelper.BytesToStuct<EEctypeMemberFields>(bytes);
	}
}

// 进入副本请求
//MSG_ECTYPE_ENTER     ,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeRequestCreate_CS
{
	public int dwEctypeContainerID;//副本容器ID
	public byte byStory;    //用于进入副本处理剧情的标记, 0:未处理剧情 1：已处理剧情
    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SMSGEctypeRequestCreate_CS>(this);

        return pkg;
    }
};
public struct SMSGECTYPE_FINISH_SC: INotifyArgs
{
	public SMSGECTYPE_FINISH_SC(byte[] datas)
	{
		bySucess=datas[0];
		bySettlement=datas[1];
	}
	#region INotifyArgs implementation

	public int GetEventArgsType ()
	{
		throw new NotImplementedException ();
	}

	#endregion

	public byte bySucess;		// 是否成功 0 失败，1成功
	public byte bySettlement;	// 是否结算
};

//请求关卡信息
// MSG_ECTYPE_OPENECTYPECONTAINER_SELECT,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGRequestEctypeLevelDatas_CS
{
	public long uidEntity;			// 请求者ID
	public int dwEctypeId;			// 副本ID 不是副本容器

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)masterMsgType, subMsgType);
        pkg.Data = PackageHelper.StructToBytes<SMSGRequestEctypeLevelDatas_CS>(this);

        return pkg;
    }
};
//回应关卡信息
// MSG_ECTYPE_ECTYPECONTAINER_LEVELDATAS
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGOpenEctypeLevelDatas_SC: INotifyArgs
{
    public long uidEntity;          // 请求者ID
    public int dwEctypeId;			// 副本ID 不是副本容器
    public byte dwCurContainerIndex;// 当前进行中的副本index(01-10);
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] byGrade;          // 10个关卡的评级数据
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public int[] dwBestClearTime; // 10个关卡的通关时间数据
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10 * 2)]
    public byte[] wHighestCombo;    // 10个关卡的最高连击数数据

    public static SMSGOpenEctypeLevelDatas_SC ParsePackage(byte[] dataBuffer)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        SMSGOpenEctypeLevelDatas_SC sMSGOpenEctypeLevelDatas_SC = new SMSGOpenEctypeLevelDatas_SC();

        int of = 0;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMSGOpenEctypeLevelDatas_SC.uidEntity);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMSGOpenEctypeLevelDatas_SC.dwEctypeId);
        sMSGOpenEctypeLevelDatas_SC.dwCurContainerIndex=dataBuffer[of];
        of += 1;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMSGOpenEctypeLevelDatas_SC.byGrade,10);
        

        //sMSGOpenEctypeLevelDatas_SC.uidEntity = BitConverter.ToInt64(package.Data, 0);
        //sMSGOpenEctypeLevelDatas_SC.dwEctypeId = BitConverter.ToInt32(package.Data, 8);
        //sMSGOpenEctypeLevelDatas_SC.dwCurContainerIndex = package.Data[8 + 4];
        //sMSGOpenEctypeLevelDatas_SC.byGrade = package.Data.Skip(8 + 4 + 1).Take(10).ToArray();
        
        sMSGOpenEctypeLevelDatas_SC.dwBestClearTime = new int[10];
        byte[] bestTime;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out bestTime,40);        

        for (int i = 0; i < 10; i++)
        {
            var offset = i * 4;
            sMSGOpenEctypeLevelDatas_SC.dwBestClearTime[i] = BitConverter.ToInt32(bestTime, offset);
        }
        of+= PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMSGOpenEctypeLevelDatas_SC.wHighestCombo, 20);

        return sMSGOpenEctypeLevelDatas_SC;
    }

    public int GetEventArgsType()
    {
        return 0;
    }
};

// 通知客户端副本难度选择界面
//MSG_ECTYPE_DIFFICULTY_SELECT     ,
//[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
//public struct SMSGEctypeWorldMapSelect_SC
//{
//    public long uidEntity;		//实体UID
//    public int dwSid;		    //当前传送点SID
//    public uint dwEctypeID;		// 当前进行中的副本ID 非副本容器ID
//    public ushort nEctypeCount;	// 玩过的副本个数 包括进行中的
//    public SMSGEctypeData_SC[] recordEctypeData;

//    public static SMSGEctypeWorldMapSelect_SC ParsePackage(byte[] dataBuffer)
//    {
//        Package package = PackageHelper.ParseReceiveData(dataBuffer);
//        SMSGEctypeWorldMapSelect_SC sMSGOpenEctypeLevelDatas_SC = new SMSGEctypeWorldMapSelect_SC();

//        sMSGOpenEctypeLevelDatas_SC.uidEntity = BitConverter.ToInt64(package.Data, 0);
//        sMSGOpenEctypeLevelDatas_SC.dwSid = BitConverter.ToInt32(package.Data, 8);
//        sMSGOpenEctypeLevelDatas_SC.dwEctypeID = BitConverter.ToUInt32(package.Data, 8 + 4);
//        sMSGOpenEctypeLevelDatas_SC.nEctypeCount = BitConverter.ToUInt16(package.Data, 8 + 4 + 4);
        
//        sMSGOpenEctypeLevelDatas_SC.recordEctypeData = new SMSGEctypeData_SC[sMSGOpenEctypeLevelDatas_SC.nEctypeCount];
//        var length = Marshal.SizeOf(typeof(SMSGEctypeData_SC));

//        for (int i = 0; i < sMSGOpenEctypeLevelDatas_SC.nEctypeCount; ++i)
//        {
//            sMSGOpenEctypeLevelDatas_SC.recordEctypeData[i] = SMSGEctypeData_SC.ParsePackage(package.Data.Skip(8 + 4 + 4 + 2 + length * i).Take(length).ToArray());
//        }

//        return sMSGOpenEctypeLevelDatas_SC;
//    }

//};

//// 根据SMSGEctypeWorldMapSelect_SC 协议中的nEctypeCount
//[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
//public struct SMSGEctypeData_SC
//{
//    public uint dwEctypeID;				// 玩过的副本ID
//    public byte byStarTotal;				// 玩过的副本ID相关星星总数

//    public static SMSGEctypeData_SC ParsePackage(byte[] dataBuffer)
//    {
//        return PackageHelper.BytesToStuct<SMSGEctypeData_SC>(dataBuffer);
//    }
//};

public struct SEquipReward
{
    public uint dwEquipId;        // 装备Id
    public uint dwEquipNum;       // 装备数量
};

/// <summary>
/// 通知客户端副本结算界面
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeSettleAccounts : INotifyArgs
{
    public long uidEntity;      // 结算的玩家
    //public byte dwKillPercent;	// 连击击杀率百分比
    public uint dwHighestCombo;	// 最高连击
    public byte byGrade;		// 评分
    public string sGrade;      // 评分字体
    public uint dwGradeCount;  //评分分数
    public uint dwHPRate;  //血量
    public uint dwTime;			// 通关时间
    public uint dwBaseExp;		// 基础经验
    public uint dwBaseMoney;	// 基础奖励钱币
    public uint dwGradeExp;		// 评分经验值
    public uint dwGradeMoney;	// 评分奖励钱币
	public int	dwEquipId;		// 普通翻牌奖励物品
	public int	dwEquipNum;		// 普通翻牌奖励物品个数
    public byte byItemNum;    //获得物品数量
    public List<SEquipReward> SEquipRewardList;        //获得物品列表

    public static SMSGEctypeSettleAccounts ParsePackage(byte[] dataBuffer)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        SMSGEctypeSettleAccounts sMSGEctypeSettleAccounts = new SMSGEctypeSettleAccounts();
        int of = 0;
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.uidEntity);
        //of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwKillPercent);
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwHighestCombo);
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.byGrade);
        sMSGEctypeSettleAccounts.sGrade = GetGrade(sMSGEctypeSettleAccounts.byGrade);
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwGradeCount);
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwHPRate);
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwTime);
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwBaseExp);
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwBaseMoney);
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwGradeExp);
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwGradeMoney);
		of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwEquipId);
		of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.dwEquipNum);		
        of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sMSGEctypeSettleAccounts.byItemNum);

        TraceUtil.Log("收到获得物品个数：" + sMSGEctypeSettleAccounts.byItemNum);
        sMSGEctypeSettleAccounts.SEquipRewardList = new List<SEquipReward>();
        for (byte i = 0; i < sMSGEctypeSettleAccounts.byItemNum; i++)
        {
            SEquipReward sEquipReward=new SEquipReward();
            of += PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sEquipReward.dwEquipId);
            of+=PackageHelper.ReadData(package.Data.Skip(of).ToArray(), out sEquipReward.dwEquipNum);
            sMSGEctypeSettleAccounts.SEquipRewardList.Add(sEquipReward);
        }

        return sMSGEctypeSettleAccounts;
    }

    private static string GetGrade(byte _byGrade)
    {
        switch ((EctypeGrade)_byGrade)
        {
            case EctypeGrade.ECTYPE_GRADE_A:
                return "A";
            case EctypeGrade.ECTYPE_GRADE_B:
                return "B";
            case EctypeGrade.ECTYPE_GRADE_C:
                return "C";
            case EctypeGrade.ECTYPE_GRADE_S:
                return "S";
            case EctypeGrade.ECTYPE_GRADE_SS:
                return "SS";
            case EctypeGrade.ECTYPE_GRADE_SSS:
                return "SSS";
            default:
                return "INVAILD";
        }
    }

    public int GetEventArgsType()
    {
        return 0;
    }

    // 通关评级
    enum EctypeGrade
    {
        ECTYPE_GRADE_INVAILD = 0,
        ECTYPE_GRADE_C,					// C级
        ECTYPE_GRADE_B,					// B级
        ECTYPE_GRADE_A,					// A级
        ECTYPE_GRADE_S,					// S级
        ECTYPE_GRADE_SS,				// SS级
        ECTYPE_GRADE_SSS				// SSS级
    };
};


public struct SMSGEctypeSettleAccounts2_SC
{
    public long uidPlayer;		// 玩家UID
    public byte byGrade;		// 评分
    public string sGrade;
    public int dwAwardEquipId;	// 奖励物品ID
    public int dwAwardEquipNum;// 奖励物品个数
    public byte byPickUpEquipNum;// 展示物品个数
    public List<int> EquipItemList;//拾取的物品列表
    /*
    DWORD			dwEquipId(根据前面的个数来确定压入个数)
    */
};

// 每个玩家的技能 
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSkillShowRes
{
    public long uidEntity;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 42)]
    public SSkill[] sSkills;

    public static SSkillShowRes ParsePackage(byte[] dataBuffer)
    {
        SSkillShowRes sSkillShowRes = new SSkillShowRes();
        sSkillShowRes.uidEntity = BitConverter.ToInt64(dataBuffer, 0);
        sSkillShowRes.sSkills = new SSkill[42];
        int offset = Marshal.SizeOf(typeof(SSkill));

        for (int i = 0; i < 42; i++)
        {
            sSkillShowRes.sSkills[i] = SSkill.ParsePackage(dataBuffer.Skip(8 + offset * i).Take(offset).ToArray());
        }

        return sSkillShowRes;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSkill
{
    public ushort bySkillID;
    public byte bySkillLV;

    public static SSkill ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SSkill>(dataBuffer);
    }
};


// 创建副本应答
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeCreateResult_SC
{
    byte byResult;
};

// 通知服务器玩家Loading成功
// MSG_ECTYPE_ENTERECTYPE_READY
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGTeamateRequestEnterEctype_CS
{
    public long uidEntity;				// 准备完成角色ID
    public byte byOperationType;			// 操作方式(0=点击式，1=摇杆式)
    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)masterMsgType, subMsgType);
        pak.Data = PackageHelper.StructToBytes<SMSGTeamateRequestEnterEctype_CS>(this);
        return pak;
    }
};

// 所有Loading完成 角色可以进入副本
// MSG_ECTYPE_CANENTER_ECTYPE 
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGResponeTeamateEnterEctype_SC
{
    public uint dwEctypeContainerId;				// 接收者实体ID
    public uint dwEctypeTimes;//                //剩余时间
    //public byte byResult;				// 跳转状态 1 成功, 2 不成功
    public static SMSGResponeTeamateEnterEctype_SC ParsePackage(byte[] dataBuffer)
    {
        //SMSGResponeTeamateEnterEctype_SC sMSGResponeTeamateEnterEctype_SC = new SMSGResponeTeamateEnterEctype_SC()
        //{
        //    uidEntity = BitConverter.ToInt64(dataBuffer, 0),
        //    //byResult = dataBuffer[9],
        //};
        return PackageHelper.BytesToStuct<SMSGResponeTeamateEnterEctype_SC>(dataBuffer);
    }
};


//打开副本界面请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeGoBattle_CS
{
    public long uidEntity;//玩家实体UID	
    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)masterMsgType, subMsgType);
        pak.Data = PackageHelper.StructToBytes<SMSGEctypeGoBattle_CS>(this);
        return pak;
    }
}

//打开副本UI应答
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeSelect_SC
{
    public ushort nEctypeCount;	// 玩过的副本个数 包括进行中的
	public SMSGEctypeData_SC[] sMSGEctypeData_SCs;
	public ushort wChestCount;//宝箱状态个数
	public List<SMSGEctypeChest_SC> SMSGEctypeChestStatus;//宝箱可否使用状态列表

    public static SMSGEctypeSelect_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypeSelect_SC sMSGEctypeSelect_SC = new SMSGEctypeSelect_SC();
        sMSGEctypeSelect_SC.nEctypeCount = BitConverter.ToUInt16(dataBuffer,0);        
		sMSGEctypeSelect_SC.sMSGEctypeData_SCs = new SMSGEctypeData_SC[sMSGEctypeSelect_SC.nEctypeCount];
		int dataLegth = Marshal.SizeOf(typeof(SMSGEctypeData_SC));
        for (ushort i = 0; i < sMSGEctypeSelect_SC.nEctypeCount; i++)
        {
			sMSGEctypeSelect_SC.sMSGEctypeData_SCs[i] = SMSGEctypeData_SC.ParsePackage(dataBuffer.Skip(2+dataLegth*i).Take(dataLegth).ToArray());
		}
		int off = sMSGEctypeSelect_SC.nEctypeCount*dataLegth+2;
		off+=PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(),out sMSGEctypeSelect_SC.wChestCount);
		sMSGEctypeSelect_SC.SMSGEctypeChestStatus = new List<SMSGEctypeChest_SC>();
		for(int i = 0;i<sMSGEctypeSelect_SC.wChestCount;i++)
		{
			SMSGEctypeChest_SC sMSGEctypeChest_SC;
			off+=PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(),out sMSGEctypeChest_SC.dwEctypeID);
			off+=PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(),out sMSGEctypeChest_SC.byHasOpen);
			sMSGEctypeSelect_SC.SMSGEctypeChestStatus.Add(sMSGEctypeChest_SC);
		}
		//offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMSGEctypeFindPlayer_SC.byFindStatus);
        return sMSGEctypeSelect_SC;
    }

	//有副本数据更新时,如果是更新开启扫荡，返回true
	public bool EctypeNormalInfoUpdate(SMSGEctypeData_SC newSmsge)
	{
		bool isOpenSweep = false;
		bool isFind = false;
		for (int i = 0; i < sMSGEctypeData_SCs.Length; i++) {
			if(sMSGEctypeData_SCs[i].dwEctypeContaienrID == newSmsge.dwEctypeContaienrID)
			{
				sMSGEctypeData_SCs[i].byGrade = newSmsge.byGrade;
				if(sMSGEctypeData_SCs[i].bySweep != newSmsge.bySweep)
				{
					isOpenSweep = true;
				}
				sMSGEctypeData_SCs[i].bySweep = newSmsge.bySweep;
				//Debug.Log("newSmsge.dwEctypeContaienrID=="+newSmsge.dwEctypeContaienrID+"newSmsge.byGrade="+newSmsge.byGrade);
				isFind = true;
				break;
			}
		}
		if (!isFind) {
			SMSGEctypeData_SC[] temp = sMSGEctypeData_SCs;
			sMSGEctypeData_SCs = new SMSGEctypeData_SC[sMSGEctypeData_SCs.Length+1];
			int i = 0 ;
			for(; i < temp.Length; i++)
			{
				sMSGEctypeData_SCs[i] = temp[i];
			}
			temp = null;
			sMSGEctypeData_SCs[i] = newSmsge;
		}
		return isOpenSweep;
		//TraceUtil.Log ("EctypeNormalInfoUpdate is error!!! no find this Ectype ID =" + newSmsge.dwEctypeContaienrID);
	}
	//有副本宝箱信息更新时
	public void EctypeChessInfoUpdate(SMSGEctypeChest_SC newSmsge)
	{
		int mark = -1;
		for (int i = 0; i < SMSGEctypeChestStatus.Count; i++) {
			if(SMSGEctypeChestStatus[i].dwEctypeID == newSmsge.dwEctypeID)
			{
				mark = i;
				break;
			}	
		}
		if (mark != -1) {
			SMSGEctypeChestStatus.RemoveAt(mark);
			SMSGEctypeChestStatus.Insert(mark,newSmsge);
			//Debug.Log("newSmsge.dwEctypeID=="+newSmsge.dwEctypeID+"newSmsge.byHasOpen="+newSmsge.byHasOpen);
		} else {
			//TraceUtil.Log ("EctypeChessInfoUpdate is error!!! no find this Ectype ID =" + newSmsge.dwEctypeID);
		}
	}
}

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeData_SC
{
	public uint dwEctypeContaienrID;				// 副本容器ID
	public byte byGrade;					// 玩过的副本ID的评级
	public byte bySweep;					// 是否能扫荡(0 = 还未开启扫荡， 1 = 已经开启扫荡)
	public static SMSGEctypeData_SC ParsePackage(byte[] databuffer)
	{
		SMSGEctypeData_SC sMSGEctypeData_SC = PackageHelper.BytesToStuct<SMSGEctypeData_SC>(databuffer);
		return sMSGEctypeData_SC;
	}
}

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeChest_SC
{
	public uint dwEctypeID;			// 副本ID
	public byte byHasOpen;			// 是否开启过

	public static SMSGEctypeChest_SC ParsePackage(byte[] databuffer)
	{
		SMSGEctypeChest_SC sMSGEctypeChest_SC = PackageHelper.BytesToStuct<SMSGEctypeChest_SC>(databuffer);
		return sMSGEctypeChest_SC;
	}
}

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGECTYPEOPNE_CHEST_CSC
{
	public uint dwEctypeID;
	public Package GeneratePackage()
	{
		Package package = new Package();
		package.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_OPEN_CHEST);
		package.Data = PackageHelper.StructToBytes<SMSGECTYPEOPNE_CHEST_CSC>(this);
		return package;
	}
}


//单个区域副本妖气信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeYaoqiProp_SC
{
    public uint dwEctypeSection;
    public uint dwYaoqiValue;

    public static SMSGEctypeYaoqiProp_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypeYaoqiProp_SC sMSGEctypeYaoqiProp_SC = PackageHelper.BytesToStuct<SMSGEctypeYaoqiProp_SC>(dataBuffer);
        return sMSGEctypeYaoqiProp_SC;
    }
};

//C发送给S增加妖气值
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeAddYaoqiProp_CS
{
    public uint dwEctypeSection;
    public uint dwGoodId;

    public Package GeneratePackage()
    {
        Package package = new Package();
        package.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_ADD_YAOQI);
        package.Data = PackageHelper.StructToBytes<SMSGEctypeAddYaoqiProp_CS>(this);
        return package;
    }
};

//向服务器请求单个副本关卡数据
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGRequestEctypeLevelData_CS
{
    public long uidEntity;			// 请求者ID
    public uint dwEctypeId;        // 副本ID 不是副本容器ID
    public byte byEctypeDiff;		// 副本难度
    public Package GeneratePackge(MasterMsgType masterMsgType, short subMsgType)
    {
        Package package = new Package();
        package.Head = new PkgHead((byte)masterMsgType,subMsgType);
        package.Data = PackageHelper.StructToBytes<SMSGRequestEctypeLevelData_CS>(this);
        return package;
    }
}
//服务器回应单个副本关卡数据
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeLevelData_SC
{
    public long uidEntity;			 // 请求者ID
    public uint dwEctypeId;		 // 请求数据对应副本ID 不是副本容器ID
    public byte byEctypeDiff;		 // 副本难度
    public ushort wTeamCount;		 // 队伍个数
    public byte byGrade;			 // 当前关卡的评级数据	
    public uint dwBestClearTime;    // 当前关卡的通关时间数据
    public ushort wHighestCombo;      // 当前关卡的最高连击数数据
    public ushort wEctypeCost;		 // 消耗活力值

    public static SMSGEctypeLevelData_SC ParsePackage(byte[] dataBuffer)
    {

        SMSGEctypeLevelData_SC sMSGEctypeLevelData_SC;
        sMSGEctypeLevelData_SC = PackageHelper.BytesToStuct<SMSGEctypeLevelData_SC>(dataBuffer);        
        return sMSGEctypeLevelData_SC;
    }

}


// 副本请求复活
// MSG_ECTYPE_PLAYERREVIVE
//[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
//struct SMSGEctypeRequestRevive_CSC
//{
//    public long uidEntityReq;	// 请求者
//    public long uidEntityBeReq;	// 被请求者
//    public byte byType;			// 1/4状态, 或满状态

//    public static SMSGEctypeRequestRevive_CSC ParsePackage(byte[] dataBuffer)
//    {
//        SMSGEctypeRequestRevive_CSC sMSGEctypeRequestRevive_CSC = new SMSGEctypeRequestRevive_CSC()
//        {
//            uidEntityReq = BitConverter.ToInt64(dataBuffer, 0),
//            uidEntityBeReq = BitConverter.ToInt64(dataBuffer, 8),
//            byType = dataBuffer[16],
//        };
//        return sMSGEctypeRequestRevive_CSC;
//    }

//    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
//    {
//        Package pak = new Package();
//        pak.Head = new PkgHead((byte)masterMsgType,subMsgType);
//        pak.Data = PackageHelper.StructToBytes<SMSGEctypeRequestRevive_CSC>(this);
//        return pak;
//    }

//};

// 副本复活应答
// MSG_ECYTPE_PLAYERREVIVE_RESP
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypePlayerRevive_CSC
{
    public long uidEntity;
    public long uidTeammate;
    public byte dwType;			// 1/4状态, 或者满状态
    public static SMSGEctypePlayerRevive_CSC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypePlayerRevive_CSC sMSGEctypePlayerRevive_CSC = new SMSGEctypePlayerRevive_CSC()
        {
            uidEntity = BitConverter.ToInt64(dataBuffer, 0),
            uidTeammate = BitConverter.ToInt64(dataBuffer,8),
            dwType = dataBuffer[16],
        };
        return sMSGEctypePlayerRevive_CSC;
    }
    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)masterMsgType,subMsgType);
        pak.Data = PackageHelper.StructToBytes<SMSGEctypePlayerRevive_CSC>(this);
        return pak;
    }
}

// 角色不复活
// MSG_ECTYPE_PLAYERDONTREVIVE
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypeDontRevive_CS
{
    public long uidEntity;		// 不复活角色ID

    public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)masterMsgType,subMsgType);
        pak.Data = PackageHelper.StructToBytes<SMSGEctypeDontRevive_CS>(this);
        return pak;
    }
}

/// <summary>
/// 玩家请求返回城镇
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypeRequestReturnCity_CS
{
    public long uidEntity;				// 玩家实体id
    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE,ECTYPE_DefineManager.MSG_ECTYPE_RETURN_CITY);
        pak.Data = PackageHelper.StructToBytes<SMSGEctypeRequestReturnCity_CS>(this);
        return pak;
    }
};

/// <summary>
/// 副本倒计时结算
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypePlayerRevive_SC
{
	public int dwActorID;
    public int dwReliveTime;	// 剩余时间`
	public float ReceiveMsgTime;//前段自己添加，收到倒计时时间
    public static SMSGEctypePlayerRevive_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypePlayerRevive_SC sMSGEctypePlayerRevive_SC;
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMSGEctypePlayerRevive_SC.dwActorID);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMSGEctypePlayerRevive_SC.dwReliveTime);
		sMSGEctypePlayerRevive_SC.ReceiveMsgTime = Time.realtimeSinceStartup;
        //sMSGEctypePlayerRevive_SC = PackageHelper.BytesToStuct<SMSGEctypePlayerRevive_SC>(dataBuffer);
        return sMSGEctypePlayerRevive_SC;
    }
};

/// <summary>
/// 玩家点击宝箱
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeClickTreasure_CS
{
	public byte byClickType;			// 点击的卡片类型 (普通是0, 付费为1，2 VIP宝箱)
    public Package GeneratePackage()
    {
        Package Pak = new Package();
        Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE,ECTYPE_DefineManager.MSG_ECTYPE_CLICKTREASURE);
		Pak.Data = PackageHelper.StructToBytes<SMSGEctypeClickTreasure_CS>(this);
        return Pak;
    }
};
/// <summary>
/// 抽中的宝箱奖励
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeTreasureReward_SC
{
    public long dwUID;				// 玩家UID
    public int dwEquipId;				// 奖励的装备ID
    public int dwEquipNum;				// 奖励的装备数量
	public byte byClickType;			// 点击的卡片类型 (普通是0, 付费为1，2 VIP宝箱)
    //public byte byPosIndex;			    // 奖励位置(从0开始, 以此递增)

    public static SMSGEctypeTreasureReward_SC ParsePackage(byte[] DataBuffer)
    {
        SMSGEctypeTreasureReward_SC sMSGEctypeTreasureReward_SC = PackageHelper.BytesToStuct<SMSGEctypeTreasureReward_SC>(DataBuffer);
        return sMSGEctypeTreasureReward_SC;
        
    }
};

public class EctypeTreasureRewardList
{
    public List<SMSGEctypeTreasureReward_SC> TreasureList;
    public EctypeTreasureRewardList()
    {
        TreasureList = new List<SMSGEctypeTreasureReward_SC>(); 
    }
}

//MSG_ECTYPE_ERRORCODE = 20
/// <summary>
/// 副本选择结果
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeResult_SC : INotifyArgs
{
    public enum ErrorType { PLAYERLEAVE = 804, };
    //public int dwActorId_0;            // 不符合资格的玩家ID
    //public int dwActorId_1;
    //public int dwActorId_2;
    //public int dwActorId_3;
    public int[] dwActorIds;            // 不符合资格的玩家ID
    public ushort byResult;               // 错误类型
    

    public static SMSGEctypeResult_SC ParsePackage(byte[] dataBuffer)
    {        
        SMSGEctypeResult_SC sMsgectypeResult_SC = new SMSGEctypeResult_SC();
        sMsgectypeResult_SC.dwActorIds = new int[4];
        int of = 0;
        for (int i = 0; i < 4; i++)
        {
            of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgectypeResult_SC.dwActorIds[i]);
        }
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgectypeResult_SC.byResult);

        return sMsgectypeResult_SC;
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};


[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct ECTYPE_CLEARANCE : INotifyArgs
{
    public uint dwEctypeId;		 // 请求数据对应副本ID 不是副本容器ID

    public int GetEventArgsType()
    {
        return 0;
    }
}
//MSG_ECTYPE_CHANLLETE_COMPLETE
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeChallengeComplete_CS
{
    public long uidEntity;             // 玩家实体id
    public Package GeneratePackage()
    {
        Package Pak = new Package();
        Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_CHANLLETE_COMPLETE);
        Pak.Data = PackageHelper.StructToBytes<SMSGEctypeChallengeComplete_CS>(this);
        return Pak;
    }
};

/// <summary>
/// 选择挑战pvp, 或者选择取消pvp
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypeChanllengePvp_CS
{
	public byte byChallengeType;   // ECTYPE_PVP_CHALLENGE = 1, ECTYPE_PVP_CANCEL = 2    
    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_PVP_CHALLENGE);
        pak.Data = PackageHelper.StructToBytes<SMSGEctypeChanllengePvp_CS>(this);
        return pak;
    }
};

/// <summary>
/// 查找玩家结果
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypeFindPlayer_SC : INotifyArgs
{
    public byte byFindStatus;     		// 是否查找到
    public byte byFindNum;				// 查找到的玩家个数
    public SEctypePvpPlayer EctypePvpPlayer;    //pvp玩家信息

    public static SMSGEctypeFindPlayer_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypeFindPlayer_SC sMSGEctypeFindPlayer_SC = new SMSGEctypeFindPlayer_SC();
        int offset = 0;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMSGEctypeFindPlayer_SC.byFindStatus);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMSGEctypeFindPlayer_SC.byFindNum);
        sMSGEctypeFindPlayer_SC.EctypePvpPlayer = new SEctypePvpPlayer();
        if (sMSGEctypeFindPlayer_SC.byFindNum > 0)
        {
            sMSGEctypeFindPlayer_SC.EctypePvpPlayer = SEctypePvpPlayer.ParsePackage(dataBuffer, offset);
        }        

        return sMSGEctypeFindPlayer_SC;
    }


    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};
//PVP玩家信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEctypePvpPlayer
{
    public Int64  uidEntity;			// UID
	public uint dwActorId;			// 角色ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    private byte[] _szName;		// 名称	[19]
    public string szName;       //名称
    public int nHead;				// 头像
    public byte byKind;				// 职业
    public byte nLev;					// 等级
    public byte bySex;				// 性别

    public byte bIsVip;				// 是否是VIP
    public int nCurHp;				// 当前HP
    public int nMaxHP;				// 最大HP
    public int nCurMP;				// 当前MP
    public int nMaxMP;				// 最大MP
    public int nFighting;           //战力
    public int nPrestige;			// 威望
    public int nPrestigeGroup;		// 所属竞技组
    public byte byPrestigeLevel;		// 当前称号等级
    public byte byTopPrestigeLevel;	// 最高称号等级
    public int nWinningStreak;		// 连胜等级

    public static SEctypePvpPlayer ParsePackage(byte[] dataBuffer, int offset)
    {
        SEctypePvpPlayer sEctypePvpPlayer = new SEctypePvpPlayer();
        int of = offset;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.uidEntity);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.dwActorId);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer._szName, 19);
        sEctypePvpPlayer.szName = Encoding.UTF8.GetString(sEctypePvpPlayer._szName);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.nHead);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.byKind);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.nLev);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.bySex);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.bIsVip);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.nCurHp);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.nMaxHP);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.nCurMP);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.nMaxMP);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.nFighting);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.nPrestige);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.nPrestigeGroup);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.byPrestigeLevel);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.byTopPrestigeLevel);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sEctypePvpPlayer.nWinningStreak);

        return sEctypePvpPlayer;
    }
}

/// <summary>
/// 动作播放完成
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypePvpActionDone_CS
{
    public uint dwActorId;			// 动作播放完成的角色

    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_PVP_ACTIONDONE);
        pak.Data = PackageHelper.StructToBytes<SMSGEctypePvpActionDone_CS>(this);
        return pak;
    }    
};

/// <summary>
/// 准备战斗
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypePvpReady_SC : INotifyArgs
{
    public uint dwReadyTime;
    //public uint dwFightingTime;

    public static SMSGEctypePvpReady_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypePvpReady_SC sMSGEctypePvpReady_SC;
        sMSGEctypePvpReady_SC = PackageHelper.BytesToStuct<SMSGEctypePvpReady_SC>(dataBuffer);
        return sMSGEctypePvpReady_SC;
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

/// <summary>
/// 开始战斗
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypePvpFighting_SC : INotifyArgs
{
    public uint dwFightingTime;
    //public uint dwActorID2;

    public static SMSGEctypePvpFighting_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypePvpFighting_SC sMSGEctypePvpFighting_SC;
        sMSGEctypePvpFighting_SC = PackageHelper.BytesToStuct<SMSGEctypePvpFighting_SC>(dataBuffer);
        return sMSGEctypePvpFighting_SC;
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};

/// <summary>
/// 玩家逃跑
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypeRunAway_CS
{
    public uint dwActorID;      // PVP逃跑玩家

    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_PVP_RUNAWAY);
        pak.Data = PackageHelper.StructToBytes<SMSGEctypeRunAway_CS>(this);
        return pak;
    }        
};

/// <summary>
/// pvp结算
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypePvpSettleAccounts_SC : INotifyArgs
{
    public uint dwWinerActorID;	  // 胜利者id
    public uint dwLoserActorID;	  // 失败者id
    public uint dwPrestigeReward; // 名望奖励
    public uint dwLoserPrestigeDeduct;  //名望扣除

    public static SMSGEctypePvpSettleAccounts_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypePvpSettleAccounts_SC sMSGEctypePvpSettleAccounts_SC;
        sMSGEctypePvpSettleAccounts_SC = PackageHelper.BytesToStuct<SMSGEctypePvpSettleAccounts_SC>(dataBuffer);
        return sMSGEctypePvpSettleAccounts_SC;
    }

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
};




/// <summary>
/// 队友副本属性
/// </summary>
public struct TeammateEctypeInitialize
{
	public int teammateActorID;	//队友actorID
	public int reliveTimes;		//复活次数
}

/// <summary>
/// 初始化副本属性信息
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeInitialize_SC
{
	public enum EctypeMemberFields
	{
		ECTYPE_MEMBER_FIELD_RELIVETIMES = 0,			// 复活次数
		ECTYPE_MEMBER_FIELD_ECTYPECONTAINERID,		// 副本容器ID
		ECTYPE_MEMBER_FIELD_YAONVSKILLTIMES,			// 技能使用次数	
		ECTYPE_MEMBER_FIELD_MEDICAMENTTIMES,			// 药品使用次数
	}

	public int dwReliveTimes;			// 复活次数
    public int dwEctypeContainerId; //副本id
	public int dwYaoNvSkillTimes;			// 技能使用次数	
	public int dwMedicamentTimes;			// 药品使用次数


	//public byte byType;				//是否为单机（0 为非单机，1 为单机） 
    public static SMSGEctypeInitialize_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC;
        sMSGEctypeInitialize_SC = PackageHelper.BytesToStuct<SMSGEctypeInitialize_SC>(dataBuffer);
        return sMSGEctypeInitialize_SC;
    } 

	public SMSGEctypeInitialize_SC SetValue(int index, int value)
	{
		var bytes = PackageHelper.StructToBytes<SMSGEctypeInitialize_SC>(this);
		
		int offset = index * 4;
		var source = BitConverter.GetBytes(value);
		bytes[offset] = source[0];
		bytes[offset + 1] = source[1];
		bytes[offset + 2] = source[2];
		bytes[offset + 3] = source[3];
		
		return PackageHelper.BytesToStuct<SMSGEctypeInitialize_SC>(bytes);
	}
};

//副本属性单属性更新
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgEctypeMemberUpdateProp_SC
{
	public int dwActorID;		//玩家的ActorID
	public short wProp;			//属性Index
	public int	nValue;			//属性值

	public static SMsgEctypeMemberUpdateProp_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgEctypeMemberUpdateProp_SC>(dataBuffer);
	}
};


// 副本战斗模式 S->C
// MSG_ECTYPE_FIGHT_MODEL
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMSGEctypeFightMode_SC
{
    public byte byType;				//是否为单机（0 为非单机，1 为单机）
    public static SMSGEctypeFightMode_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypeFightMode_SC sMSGEctypeInitialize_SC = new SMSGEctypeFightMode_SC() { byType = dataBuffer[0] };
        return sMSGEctypeInitialize_SC;
    } 
};


//本地通知选择排行表列表组
struct SMSGPVPGetRankingListGroup : INotifyArgs
{
    public int groupID;

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// ???????±±?
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypePraicticeList_SC
{
    public byte RoomNumber;
    public List<EctypePraictice> EctypePraicticeList;

    public static SMSGEctypePraicticeList_SC ParsePackage(byte[] dataBuffer)
    {
        TraceUtil.Log(dataBuffer.Length);
        SMSGEctypePraicticeList_SC sMSGEctypePraicticeList_SC = new SMSGEctypePraicticeList_SC();
        sMSGEctypePraicticeList_SC.EctypePraicticeList = new List<EctypePraictice>();
        int off = 0;
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypePraicticeList_SC.RoomNumber);
        for (int i = 0; i < sMSGEctypePraicticeList_SC.RoomNumber; i++)
        {
            EctypePraictice ectypePraictice = new EctypePraictice();
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out ectypePraictice.dwRoomID);
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out ectypePraictice.dwEctypeID);
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out ectypePraictice._szName, 19);
            ectypePraictice.Name = Encoding.UTF8.GetString(ectypePraictice._szName);
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out ectypePraictice.dwPlayerNum);
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out ectypePraictice.dwPraicticeSpeed);
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out ectypePraictice.dwPraicticeMax);
            sMSGEctypePraicticeList_SC.EctypePraicticeList.Add(ectypePraictice);
            TraceUtil.Log("房间人数：" + ectypePraictice.dwPlayerNum);
        }
        return sMSGEctypePraicticeList_SC;
    }

};

/// <summary>
/// 单个练功房信息
/// </summary>
public struct EctypePraictice
{
    public uint dwRoomID;			//房间ID
    public uint dwEctypeID;			//副本ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[] _szName;		// 名称	[19]
    public string Name;         //名字
    public uint dwPlayerNum;		//人数
    public uint dwPraicticeSpeed;	//修炼速度
    public uint dwPraicticeMax;		//修为上限
};

/// <summary>
/// 获取练功房列表请求消息
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct GetMartialArtsRoomList
{
    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE,ECTYPE_DefineManager.MSG_ECTYPE_PRACTICE_LIST);
        return pak;
    }
}

/// <summary>
/// 进入练功房
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypePraicticeEnter_SC
{
    public uint dwRoomID;			//房间ID (0表示随机进入)

    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE,ECTYPE_DefineManager.MSG_ECTYPE_PRACTICE_ENTER);
        pak.Data = PackageHelper.StructToBytes<SMSGEctypePraicticeEnter_SC>(this);
        return pak;
    }
};
/// <summary>
/// 快速进入练功房
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypePraicticeQuickEnter_SC
{
    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_FAST_ENTER);
        return pak;
    }
};
//房间位置信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeALLSeatInfo_SC
{
	public uint	dwRoomID;				// 房间ID
    public byte byRoomType;             //房间类型
	public uint	dwPlayerNum;			// 房间玩家人数
    public uint dwHomerActorID;         //房主actorID
    public long dwHomerUID;             //房主UID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    private byte[] _szName;             //房主名字
    public string HomerName;
    //public List<EctypeSeatInfo> SeatInfoList;	// 座位信息

    public static SMSGEctypeALLSeatInfo_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypeALLSeatInfo_SC sMSGEctypeALLSeatInfo_SC = new SMSGEctypeALLSeatInfo_SC();
        int off = 0;
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypeALLSeatInfo_SC.dwRoomID);
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypeALLSeatInfo_SC.byRoomType);
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypeALLSeatInfo_SC.dwPlayerNum);
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypeALLSeatInfo_SC.dwHomerActorID);
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypeALLSeatInfo_SC.dwHomerUID);
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypeALLSeatInfo_SC._szName, 19);
        sMSGEctypeALLSeatInfo_SC.HomerName = Encoding.UTF8.GetString(sMSGEctypeALLSeatInfo_SC._szName);
        //sMSGEctypeALLSeatInfo_SC.SeatInfoList = new List<EctypeSeatInfo>();
        //for (int i = 0; i < sMSGEctypeALLSeatInfo_SC.dwPlayerNum; i++)
        //{
        //    EctypeSeatInfo ectypeSeatInfo = new EctypeSeatInfo();
        //    off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out ectypeSeatInfo.dwSeatIndex);
        //    off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out ectypeSeatInfo.uidPlayer);
        //    sMSGEctypeALLSeatInfo_SC.SeatInfoList.Add(ectypeSeatInfo);
        //}
        return sMSGEctypeALLSeatInfo_SC;
    }
}
//房间位置更新信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGEctypeSeatInfo_SC
{
    public uint dwPlayerNum;			// 房间玩家人数
    public EctypeSeatInfo sSeatInfo;		// 座位信息
    public static SMSGEctypeSeatInfo_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypeSeatInfo_SC sMSGEctypeSeatInfo_SC = new SMSGEctypeSeatInfo_SC();
        int of = 0;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMSGEctypeSeatInfo_SC.dwPlayerNum);
        sMSGEctypeSeatInfo_SC.sSeatInfo = new EctypeSeatInfo();
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMSGEctypeSeatInfo_SC.sSeatInfo.dwSeatIndex);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMSGEctypeSeatInfo_SC.sSeatInfo.uidPlayer);
        return sMSGEctypeSeatInfo_SC;
    }
};
//座位信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct EctypeSeatInfo
{
    public uint dwSeatIndex;		// 座位序号
    public long uidPlayer;			// 玩家UID
    
    public static EctypeSeatInfo ParsePackage(byte[] dataBuffer)
    {
        EctypeSeatInfo ectypeSeatInfo = new EctypeSeatInfo();
        int off = 0;
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out ectypeSeatInfo.dwSeatIndex);
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out ectypeSeatInfo.uidPlayer);
        return ectypeSeatInfo;
    }
};

//妖女显示
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypePracice_YaoNvUpdate_CSC : INotifyArgs
{
    public uint[] dwYaoNvList;

    public static SMsgEctypePracice_YaoNvUpdate_CSC ParsePackage(byte[] buffer)
    {
        SMsgEctypePracice_YaoNvUpdate_CSC sMsgEctypePracice_YaoNvUpdate_CSC = new SMsgEctypePracice_YaoNvUpdate_CSC();
        int off = 0;
        int yaoNvLength = buffer.Length / 4;
        sMsgEctypePracice_YaoNvUpdate_CSC.dwYaoNvList = new uint[yaoNvLength];
        for (int i = 0; i < yaoNvLength; i++)
        {
            off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgEctypePracice_YaoNvUpdate_CSC.dwYaoNvList[i]);
        }
        return sMsgEctypePracice_YaoNvUpdate_CSC;
    }

    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_PRACIICE_YAONVUPDATE);
        List<byte> list = new List<byte>();
        this.dwYaoNvList.ApplyAllItem(p => {
            list.AddRange(BitConverter.GetBytes(p));
            TraceUtil.Log(p);
        });
        pak.Data = list.ToArray();        
        return pak;
    }

    public int GetEventArgsType()
    {
        return 0;
    }
};

/// <summary>
/// 下发试炼副本列表
/// </summary>
public struct SMSGEctypeTrialsInfo_SC
{
	//public uint	dwTotalTimes;			// 已挑战次数
	public uint dwEctypeNum;			// 个数
	public List<SEctypeTrialsInfo> sInfos;

    public static SMSGEctypeTrialsInfo_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypeTrialsInfo_SC sMSGEctypeTrialsInfo_SC = new SMSGEctypeTrialsInfo_SC();
        int off = 0;
        //off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypeTrialsInfo_SC.dwTotalTimes);
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypeTrialsInfo_SC.dwEctypeNum);
        sMSGEctypeTrialsInfo_SC.sInfos = new List<SEctypeTrialsInfo>();
        for (int i = 0; i < sMSGEctypeTrialsInfo_SC.dwEctypeNum; i++)
        {
            SEctypeTrialsInfo sEctypeTrialsInfo = new SEctypeTrialsInfo();
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sEctypeTrialsInfo.dwEctypeID);
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sEctypeTrialsInfo.byDiff);
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sEctypeTrialsInfo.byClearance);
            sMSGEctypeTrialsInfo_SC.sInfos.Add(sEctypeTrialsInfo);
        }
        return sMSGEctypeTrialsInfo_SC;
    }
};

public struct SEctypeTrialsInfo
{
	public uint dwEctypeID;				// 玩过的副本ID
	public byte byDiff;					// 当前进行中的难度
	public byte byClearance;			// 是否通关
};


/// <summary>
/// 获取试炼副本列表信息
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct GetTrialsEctypeList
{
    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_TRIALS_INFO);
        return pak;
    }
}

/// <summary>
/// 试炼副本通关单个波数结算
/// </summary>
public struct SMSGEctypeTrialsSubResult_SC
{
    public byte dwProgress;		// 波数
    //public uint dwExp;			// 经验
    //public uint dwXiuLian;		// 修为
    public static SMSGEctypeTrialsSubResult_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypeTrialsSubResult_SC sMSGEctypeTrialsSubResult_SC = new SMSGEctypeTrialsSubResult_SC();
        int Off = 0;
        Off += PackageHelper.ReadData(dataBuffer.Skip(Off).ToArray(), out sMSGEctypeTrialsSubResult_SC.dwProgress);
        //Off += PackageHelper.ReadData(dataBuffer.Skip(Off).ToArray(), out sMSGEctypeTrialsSubResult_SC.dwExp);
        //Off += PackageHelper.ReadData(dataBuffer.Skip(Off).ToArray(), out sMSGEctypeTrialsSubResult_SC.dwXiuLian);
        return sMSGEctypeTrialsSubResult_SC;
    }
};

/// <summary>
/// 试炼副本通关结算
/// </summary>
public struct SMSGEctypeTrialsTotalResult_SC
{
    public byte byClearance;		// 是否通关（0 为没通关， 1为通关）
	public byte	dwProgress;					// 波数
	public  List<SEquipRewardInfo> dwEquipReward;	// 奖励列表

    public static SMSGEctypeTrialsTotalResult_SC ParsePackage(byte[] dataBuffer)
    {
        SMSGEctypeTrialsTotalResult_SC sMSGEctypeTrialsTotalResult_SC = new SMSGEctypeTrialsTotalResult_SC();
        int off = 0;
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypeTrialsTotalResult_SC.byClearance);
        off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMSGEctypeTrialsTotalResult_SC.dwProgress);
        sMSGEctypeTrialsTotalResult_SC.dwEquipReward = new List<SEquipRewardInfo>();
        for (int i = 0; i < sMSGEctypeTrialsTotalResult_SC.dwProgress;i++)
        {
            SEquipRewardInfo sEquipRewardInfo = new SEquipRewardInfo();
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sEquipRewardInfo.dwEquipId);
            off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sEquipRewardInfo.dwEquipNum);
            sMSGEctypeTrialsTotalResult_SC.dwEquipReward.Add(sEquipRewardInfo);
        }
        TraceUtil.Log("收到试炼结算物品个数:" + sMSGEctypeTrialsTotalResult_SC.dwProgress);
        sMSGEctypeTrialsTotalResult_SC.dwEquipReward.ApplyAllItem(P=>TraceUtil.Log("收到试炼结算物品："+P.dwEquipId));
        return sMSGEctypeTrialsTotalResult_SC;
    }
};

// 装备奖励
public struct SEquipRewardInfo
{
	public uint dwEquipId;				// 奖励的装备ID
    public uint dwEquipNum;				// 奖励的装备数量
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGECTYPELEVEL_COLLECTINFO_CS
{
    public float nDelay;				//副本平均延迟
    public int nFrames;			//副本平均帧数
    public int nSkillBreak;		//技能打断次数
    public int nAPM;				//副本APM值
    public int nLoadingTime;		//关卡loading时间
    public byte byButtonNum;		//按钮个数
    public int[] ClickBtnInfoList; //按钮次数列表

    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_LEAVE_COLLECTINFO);
        List<byte> getData = new List<byte>();
        getData.AddRange(BitConverter.GetBytes(nDelay));
        getData.AddRange(BitConverter.GetBytes(nFrames));
        getData.AddRange(BitConverter.GetBytes(nSkillBreak));
        getData.AddRange(BitConverter.GetBytes(nAPM));
        getData.AddRange(BitConverter.GetBytes(nLoadingTime));
        getData.Add(byButtonNum);
        foreach (var child in ClickBtnInfoList)
        {
            getData.AddRange(BitConverter.GetBytes(child));
        }
        pak.Data = getData.ToArray();
        return pak;
    }

}

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGECTYPEYAONVSKILLUSETIME_SC
{
	public int dwUseTime;
	
	public static SMSGECTYPEYAONVSKILLUSETIME_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMSGECTYPEYAONVSKILLUSETIME_SC>(dataBuffer);
	}
}
/// <summary>
/// 防守副本
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGECTYPEDEFINE_RESULT_SC:INotifyArgs
{
	public int dwBossPH;
	public int dwBatterCount;
	public int dwHitCount;

	public static SMSGECTYPEDEFINE_RESULT_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMSGECTYPEDEFINE_RESULT_SC>(dataBuffer);
	}
	#region INotifyArgs implementation
	
	public int GetEventArgsType ()
	{
		throw new NotImplementedException ();
	}
	
	#endregion
};

#region 无尽试炼

// 无尽副本波数[主动下发]
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypeEndless_LoopNum_SC
{
	public int dwLoopNum;
	
	public static SMsgEctypeEndless_LoopNum_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgEctypeEndless_LoopNum_SC>(dataBuffer);
	}
}
// 无尽副本获得奖励
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypeEndless_Reward_SC
{
	public int dwLoopNum;
	
	public static SMsgEctypeEndless_Reward_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgEctypeEndless_Reward_SC>(dataBuffer);
	}
}

// 无尽副本结算
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypeEndless_Result_SC
{
	public int dwFinishLoopIndex;
	public List<int> passLoopList ;
	public static SMsgEctypeEndless_Result_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgEctypeEndless_Result_SC sMsgEctypeEndless_Result_SC = new SMsgEctypeEndless_Result_SC ();
		int off = 0;
		off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out sMsgEctypeEndless_Result_SC.dwFinishLoopIndex);
		sMsgEctypeEndless_Result_SC.passLoopList = new List<int>();
		for (int i = 0; i < sMsgEctypeEndless_Result_SC.dwFinishLoopIndex;i++)
		{
			int val = 0;
			off += PackageHelper.ReadData(dataBuffer.Skip(off).ToArray(), out val);
			sMsgEctypeEndless_Result_SC.passLoopList.Add(val);
		}
		return sMsgEctypeEndless_Result_SC;
	}
}

// 无尽副本界面数据
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypeEndless_Info_SC
{
	public int dwTodayBest;
	public int dwHistoryBest;
	public static SMsgEctypeEndless_Info_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgEctypeEndless_Info_SC>(dataBuffer);
	}
}

// 无尽副本界面数据更新
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypeEndless_Info_Updata_SC
{
	public byte wProp;// 0为今日最佳 1为历史最佳
	public int dwValue;
	public static SMsgEctypeEndless_Info_Updata_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgEctypeEndless_Info_Updata_SC>(dataBuffer);
	}
}
// 无尽副本断线重连
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgEctypeEndless_LoopTime_SC
{
	public int dwTime;
	public static SMsgEctypeEndless_LoopTime_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgEctypeEndless_LoopTime_SC>(dataBuffer);
	}
};
//无尽副本场景跳转倒计时
// MSG_ECTYPE_ENDLESS_MAPJUMPTIME = 64
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgEctypeEndless_MapJumpTime_SC
{
	public int dwTime;
	public static SMsgEctypeEndless_MapJumpTime_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgEctypeEndless_MapJumpTime_SC>(dataBuffer);
	}
};
// 讨伐副本结算
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGECTYPE_CRUSADERESULT_SC
{
	public byte byGrade;		// 评分
	public int dwTime;		// 时间
	public SEquipRewardInfo[] sEquip; // 获取装备 定长2个
	public byte byPlayerNum;	// 玩家个数
	public SHitNumInfo[] sHitNumInfo;

	public static SMSGECTYPE_CRUSADERESULT_SC ParsePackage(byte[] dataBuffer)
	{
		SMSGECTYPE_CRUSADERESULT_SC sMSGECTYPE_CRUSADERESULT_SC = new SMSGECTYPE_CRUSADERESULT_SC();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMSGECTYPE_CRUSADERESULT_SC.byGrade);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMSGECTYPE_CRUSADERESULT_SC.dwTime);
		sMSGECTYPE_CRUSADERESULT_SC.sEquip = new SEquipRewardInfo[2];
		for(int i = 0 ; i < sMSGECTYPE_CRUSADERESULT_SC.sEquip.Length; i++)
		{
			sMSGECTYPE_CRUSADERESULT_SC.sEquip[i] = new SEquipRewardInfo();
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMSGECTYPE_CRUSADERESULT_SC.sEquip[i].dwEquipId);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMSGECTYPE_CRUSADERESULT_SC.sEquip[i].dwEquipNum);
		}
		of+= PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMSGECTYPE_CRUSADERESULT_SC.byPlayerNum);
		sMSGECTYPE_CRUSADERESULT_SC.sHitNumInfo = new SHitNumInfo[sMSGECTYPE_CRUSADERESULT_SC.byPlayerNum];
		for(int i = 0 ; i< sMSGECTYPE_CRUSADERESULT_SC.byPlayerNum;i++)
		{
			sMSGECTYPE_CRUSADERESULT_SC.sHitNumInfo[i] = new SHitNumInfo();
			of+= PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMSGECTYPE_CRUSADERESULT_SC.sHitNumInfo[i].uid);
			of+= PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMSGECTYPE_CRUSADERESULT_SC.sHitNumInfo[i].dwHitNum);
		}

		return sMSGECTYPE_CRUSADERESULT_SC;
	}

};

//讨伐计时
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMSGECTYPE_CRUSADETIME_SC
{
	public int dwTime;

	public static SMSGECTYPE_CRUSADETIME_SC ParsePackage(byte[] dataBuffer)
	{
		SMSGECTYPE_CRUSADETIME_SC sMSGECTYPE_CRUSADETIME_SC = new SMSGECTYPE_CRUSADETIME_SC();
		sMSGECTYPE_CRUSADETIME_SC.dwTime = BitConverter.ToInt32(dataBuffer,0);
		return sMSGECTYPE_CRUSADETIME_SC;
	}
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SHitNumInfo
{
	public long uid;		// 玩家UID
	public int dwHitNum;	// 玩家伤害
}

#endregion

public struct SMSGEctypeSynSkillData_SC
{
	//public long uidEntity;
	public byte byMemberNum;
	public SSkillShowRes[] sSkillShowRes;
	
	public static SMSGEctypeSynSkillData_SC ParsePackage(byte[] dataBuffer)
	{
		Package pkg = PackageHelper.ParseReceiveData(dataBuffer);
		SMSGEctypeSynSkillData_SC sMSGEctypeSynSkillData_SC = new SMSGEctypeSynSkillData_SC();
		
		//sMSGEctypeSynSkillData_SC.uidEntity = BitConverter.ToInt64(pkg.Data, 0);
		
		sMSGEctypeSynSkillData_SC.byMemberNum = pkg.Data[0];
		sMSGEctypeSynSkillData_SC.sSkillShowRes = new SSkillShowRes[sMSGEctypeSynSkillData_SC.byMemberNum];
		int offset = 8 + Marshal.SizeOf(typeof(SSkill)) * 42; //Marshal.SizeOf(typeof(SSkillShowRes));
		
		for (int i = 0; i < sMSGEctypeSynSkillData_SC.byMemberNum; i++)
		{
			sMSGEctypeSynSkillData_SC.sSkillShowRes[i] = SSkillShowRes.ParsePackage(pkg.Data.Skip(1 + offset * i).Take(offset).ToArray());
		}
		
		return sMSGEctypeSynSkillData_SC;
	}
};
#region
//扫荡
//MSG_ECTYPE_RESULT_SWEEP				= 94,			// 扫荡结算
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypeResult_Sweep_SC
{
	public byte byItemNum;
	public List<SEquipReward> SEquipRewardList;
	public static SMsgEctypeResult_Sweep_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgEctypeResult_Sweep_SC sMsgEctypeResult_Sweep_SC = new SMsgEctypeResult_Sweep_SC();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgEctypeResult_Sweep_SC.byItemNum);
		sMsgEctypeResult_Sweep_SC.SEquipRewardList = new List<SEquipReward> ();
		for (byte i = 0; i < sMsgEctypeResult_Sweep_SC.byItemNum; i++) {
			SEquipReward sEquipReward = new SEquipReward ();
			of += PackageHelper.ReadData (dataBuffer.Skip (of).ToArray (), out sEquipReward.dwEquipId);
			of += PackageHelper.ReadData (dataBuffer.Skip (of).ToArray (), out sEquipReward.dwEquipNum);
			sMsgEctypeResult_Sweep_SC.SEquipRewardList.Add (sEquipReward);
		}
		return sMsgEctypeResult_Sweep_SC;
	}
};
//MSG_ECTYPE_UNLOCK_SWEEP				= 92,			// 开启扫荡
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypeUnLock_Sweep_CS
{
	public int dwEctypeContainerID;
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_UNLOCK_SWEEP);
		Pak.Data = PackageHelper.StructToBytes<SMsgEctypeUnLock_Sweep_CS>(this);
		return Pak;
	}
};
//MSG_ECTYPE_BEGIN_SWEEP				= 93,			// 开始扫荡
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypeBegin_Sweep_CS
{
	public int dwEctypeContainerID;
	public int dwTimes;										
	public byte byClickType;// 0 = 免费，1 = 元宝， 2 = 至尊(vip)
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_BEGIN_SWEEP);
		Pak.Data = PackageHelper.StructToBytes<SMsgEctypeBegin_Sweep_CS>(this);
		return Pak;
	}
};
#endregion
//MSG_ECTYPE_RANDOM_REWARD				=	95,	    //首战奖励
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgEctypeRandom_Reward_SC
{
	public int dwEquipId;		// 装备Id
	public int dwEquipNum;		// 装备数量

	public static SMsgEctypeRandom_Reward_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.BytesToStuct<SMsgEctypeRandom_Reward_SC>(dataBuffer);
	}
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypeEnter_Area_CS
{
    public int dwAreaID;

    public Package GeneratePackage()
    {
        Package pak = new Package();
        pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_ECTYPE, ECTYPE_DefineManager.MSG_ECTYPE_ENTER_AREA);
       
        pak.Data = BitConverter.GetBytes(dwAreaID);
        return pak;
    }
   
};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgEctypeUpDateBlock : INotifyArgs
{
    public int dwareaId;
    public int dwblockGroupID;    
    /// <summary>
    /// 0 消失，1 显示
    /// </summary>
    public byte byBlockState;

    public static SMsgEctypeUpDateBlock ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgEctypeUpDateBlock>(dataBuffer);
    }
    #region INotifyArgs implementation

    public int GetEventArgsType()
    {
        return 0;
    }

    #endregion
};

//团队PVP结算单元
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SPVPResult
{
	public int dwActorID;		
	public int dwKillEnemy;			// 杀敌数
	public int dwDeathTimes;			// 死亡次数
	public int dwDamage;				// 造成伤害
	public int dwInjured;				// 被伤害
	public int dwHonor;				// 荣誉
	public int dwHonorExtra;			// 额外奖励荣誉
	public int dwContribute;			// 贡献
	public int dwContributeExtra;		// 额外奖励贡献
};

/// <summary>
/// 青龙会团队PVP结算
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgEctypePVP_Result_SC : INotifyArgs
{
	public byte	bySucessFlag;		// 胜利标识(0 = 神兵谷 1 = 合欢教)
	public byte	bySucessNum;		// 胜利人数
	public byte	byFailedNum;		// 失败人数
	
	public SPVPResult[] sPVPResultSucess;	//胜利队伍
	public SPVPResult[] sPVPResultFailed;	//失败队伍
	
	public static SMsgEctypePVP_Result_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgEctypePVP_Result_SC sMsgEctypePVP_Result_SC = new SMsgEctypePVP_Result_SC();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.bySucessFlag);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.bySucessNum);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.byFailedNum);
		
		sMsgEctypePVP_Result_SC.sPVPResultSucess = new SPVPResult[sMsgEctypePVP_Result_SC.bySucessNum];
		for(int i = 0 ; i < sMsgEctypePVP_Result_SC.bySucessNum; i++)
		{
			sMsgEctypePVP_Result_SC.sPVPResultSucess[i] = new SPVPResult();
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwActorID);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwKillEnemy);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwDeathTimes);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwDamage);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwInjured);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwHonor);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwHonorExtra);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwContribute);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwContributeExtra);
		}
		
		sMsgEctypePVP_Result_SC.sPVPResultFailed = new SPVPResult[sMsgEctypePVP_Result_SC.byFailedNum];
		for(int i = 0 ; i < sMsgEctypePVP_Result_SC.byFailedNum; i++)
		{
			sMsgEctypePVP_Result_SC.sPVPResultFailed[i] = new SPVPResult();
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwActorID);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwKillEnemy);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwDeathTimes);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwDamage);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwInjured);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwHonor);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwHonorExtra);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwContribute);
			of+=PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(),out sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwContributeExtra);
		}
		
		return sMsgEctypePVP_Result_SC;
	}
	
	public int GetEventArgsType()
	{
		throw new NotImplementedException();
	}
};

