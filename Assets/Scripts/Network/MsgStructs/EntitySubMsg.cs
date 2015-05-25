using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

// 传送消息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionNewWorld_SC : INotifyArgs
{
	public byte byTeleportFlg;		// 登录标志 false 第一次登录
	public uint dwMapId;		// 地图Id
	public uint dwMapParam;		// 地图参数(主要用于剧情地图 剧情ID)jamfing
	public int ptDestX;		// X坐标
	public int ptDestY;		// Y坐标
	public uint byDestOri;		// 方向	
	
	public static SMsgActionNewWorld_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.ParseDataBufferToStruct<SMsgActionNewWorld_SC>(dataBuffer);
	}
	public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
	{
		Package pkg = new Package();
		pkg.Head = new PkgHead((byte)masterMsgType, (short)subMsgType);
		pkg.Data = PackageHelper.StructToBytes<SMsgActionNewWorld_SC>(this);
		return pkg;
	}
	
	public int GetEventArgsType()
	{
		throw new NotImplementedException();
	}
};
///////////////////////////////////////////////////////////////////
// 主动请求跳转地图
//#define MSG_ACTION_TELEPORTTO				=	20,
public struct SMsgActionTeleportTo_CS
{
	public byte byTeleportToType;		//跳转类型 0:模糊跳转(一般指返回城镇(备用地图))   2:精确跳转(需要指定地图ID/X/Y)
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_TELEPORTTO);
		Pak.Data = PackageHelper.StructToBytes<SMsgActionTeleportTo_CS>(this);
		return Pak;
	}
};
struct SMsgActionTeleportToContext_CS	//只有类型是 1：精确跳转时，才使用数据现场
{
	public int dwMapId;		// 地图Id
	public int ptDestX;		// X坐标
	public int ptDestY;		// Y坐标
	public int dwDestOri;		// 方向
};

/***************************************************************/
///////////////////// 场景服的实体类的消息码/////////////////////
// 主消息码必定是NET_ROOT_THING
/***************************************************************/
// 通知客户端创建实体

/// <summary>
/// 实体基础属性
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_BaseValue
{
	public int OBJECT_FIELD_TYPE;	//对象类型
	public int OBJECT_FIELD_ENTRY_ID; //对象模版id
	
	public int GetValue(int index)
	{
		var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_BaseValue>(this);
		
		int offset = index * 4;
		return BitConverter.ToInt32(bytes.Skip(offset).ToArray(),0);
	}
	public SMsgPropCreateEntity_SC_BaseValue SetValue(int index, int value)
	{
		var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_BaseValue>(this);
		
		int offset = index * 4;
		var source = BitConverter.GetBytes(value);
		bytes[offset] = source[0];
		bytes[offset + 1] = source[1];
		bytes[offset + 2] = source[2];
		bytes[offset + 3] = source[3];
		
		return PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_BaseValue>(bytes);      
	}
}
/// <summary>
/// 生物可见属性
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_UnitVisibleValue
{
	/***************** 生物可见属性 *******************************/
	public int UNIT_FIELD_LEVEL;							// 等级
	public int UNIT_FIELD_DIR;								// 方向   收到服务器值要除1000，发送到服务器要乘以1000
	public int UNIT_FIELD_PORTIAITID;                       //生物头像ID
	public int UNIT_FIELD_DISPLAYID;                        //美术资源ID
	public int UNIT_FIELD_ISGROUP;							//是否在组队状态 0无组队 1队长 2队员
	//public int UNIT_FIELD_ISFIGHT;							//是否在战斗状态 0为非战斗，1为战斗
	public int UINT_FIELD_STATE;							//UINT_FIELD_ISFIGHT修改为UINT_FIELD_STATE值的枚举值,表示玩家的状态 CRT_STATE
	public int UNIT_FIELD_ISTASK;							//是否在任务状态 0为非任务，1为任务
	
	/***************** 战斗中可见属性 ***************************/
	public int UNIT_FIELD_CURHP;						    // 当前HP
	public int UNIT_FIELD_MAXHP;							// 最大HP
	public int UNIT_FIELD_FIGHTING;							// 战斗力
	public int UNIT_FIELD_FIGHT_HOSTILITY;                  //战斗中的敌对关系
	public int UNIT_FIELD_SPEED;                  //速度
	public int UNIT_FIELD_SHARD;                  //防护值
	public int UNIT_FIELD_CURMP;							// 当前真气
	public int UNIT_FIELD_MAXMP;							// 最大真气
	
	public int GetValue(int index)
	{
		var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_UnitVisibleValue>(this);
		
		int offset = index * 4;
		return BitConverter.ToInt32(bytes.Skip(offset).ToArray(),0);
	}
	public SMsgPropCreateEntity_SC_UnitVisibleValue SetValue(int index, int value)
	{
		if(index == 11)
		{
			TraceUtil.Log("__________!!!!!!!!!MOVE SPEED__  " + value );	
		}
		
		var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_UnitVisibleValue>(this);
		
		int offset = index * 4;
		var source = BitConverter.GetBytes(value);
		bytes[offset] = source[0];
		bytes[offset + 1] = source[1];
		bytes[offset + 2] = source[2];
		bytes[offset + 3] = source[3];
		
		return PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_UnitVisibleValue>(bytes);     
	}
	
}
//生物行为状态
public enum CRT_STATE
{
	enCrt_State_Unknow = 0,		    //生物处于未知状态
	enCrt_State_Stand,				//生物处于站立状态
	enCrt_State_Move,				//生物处于走路状态
	enCrt_State_Die,				//生物处于死亡状态
	enCrt_State_ReChange,			//生物的施法状态
	enCrt_State_BeAttack,			//生物的受击状态
	enCrt_State_Stealth,			//生物处于隐身状态
	enCrt_State_Animation,         //生物处于动画状态 
	enCrt_State_Run,            //生物处于移动状态（怪物可用） 
	enCrt_State_Max,
};

/// <summary>
/// 生物不可见属性
/// </summary>
public struct SMsgPropCreateEntity_SC_UnitInvisibleValue
{
	/***************** 生物不可见属性 *******************************/
	//一级属性
	public int UNIT_FIELD_CONSTITUTION;			            //	体质 
	public int UNIT_FIELD_WEIGHT;							//单位重量		--------------
	
	//Add by 黄城
	public int UNIT_FIELD_ATTACK;							//攻击力
	public int UNIT_FIELD_DEFEND;							//防御力
	public int UNIT_FIELD_BURST;							//暴击率
	public int UNIT_FIELD_UNBURST;							//抗暴击率
	//End Add
	
	//特性属性
	public int UNIT_FIELD_NICETY;							//	命中
	public int UNIT_FIELD_JOOK;								//	闪避
	public int UNIT_FIELD_ARMOR;                            //是否霸体 0，不霸体； 1，霸体
	public int UINT_FILED_PROTECTED;						// 是否无敌		
	public int UNIT_FIELD_ARMOR_LEVEL;						// 霸体等级
	public int UINT_FIELD_DEFBREAK;					        // 破防属性

	
	public int GetValue(int index)
	{
		var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_UnitInvisibleValue>(this);
		
		int offset = index * 4;
		return BitConverter.ToInt32(bytes.Skip(offset).ToArray(),0);
	}
	
	public SMsgPropCreateEntity_SC_UnitInvisibleValue SetValue(int index, int value)
	{
		var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_UnitInvisibleValue>(this);
		
		int offset = index * 4;
		var source = BitConverter.GetBytes(value);
		bytes[offset] = source[0];
		bytes[offset + 1] = source[1];
		bytes[offset + 2] = source[2];
		bytes[offset + 3] = source[3];
		
		return PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_UnitInvisibleValue>(bytes); 
	}
}

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgActionCreateNum_SC
{
	public ushort wCreateNum;			// 创建实体的数目
	// ...wCreateNum个(SMsgPropCreateEntity_SC+创建实体的上下文)
	/// <summary>
	/// 从整体字节数组获得实体的描述结构体，包括实体ID，数量，类型，实体上下文长度
	/// </summary>
	/// <param name="dataBuffer"></param>
	/// <returns></returns>
	public static SMsgActionCreateNum_SC ParsePackage(byte[] dataBuffer)
	{
		Package package = PackageHelper.ParseReceiveData(dataBuffer);
		
		return ParsePackage(package);
	}
	public static SMsgActionCreateNum_SC ParsePackage(Package package)
	{
		var structLength = Marshal.SizeOf(typeof(SMsgActionCreateNum_SC));
		return PackageHelper.BytesToStuct<SMsgActionCreateNum_SC>(package.Data.Take(structLength).ToArray());
	}
	
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_Header:INotifyArgs
{
	public Int64 uidEntity;     //实体ID
	public byte nEntityClass;	// 实体类型
	public byte nIsHero;		// 是否客户端主角：1：是，2：否
	public ushort wContextLen;	// 创建实体现场上下文长度
	
	/// <summary>
	/// 从整体字节数组获得实体的描述结构体，包括实体ID，数量，类型，实体上下文长度
	/// </summary>
	/// <param name="dataBuffer"></param>
	/// <returns></returns>
	public static SMsgPropCreateEntity_SC_Header ParsePackage(byte[] dataBuffer, int offset)
	{
		Package package = PackageHelper.ParseReceiveData(dataBuffer);
		return ParsePackage(package, offset);
	}
	public static SMsgPropCreateEntity_SC_Header ParsePackage(Package package, int offset)
	{
		var structLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
		return PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_Header>(package.Data.Skip(offset).Take(structLength).ToArray());
	}
	public bool IsHero
	{
		get { return nIsHero == 1; }
	}
	
	public int GetEventArgsType()
	{
		return 0;
	}
}

///////////////////////////////////////////////////////////////////
// 通知客户端删除实体
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionDestroyNum_SC
{
	public ushort wDestroyNum;		// 销毁实体的数目
	
	public static SMsgActionDestroyNum_SC ParsePackage(byte[] dataBuffer)
	{
		Package package = PackageHelper.ParseReceiveData(dataBuffer);
		
		return ParsePackage(package);
	}
	public static SMsgActionDestroyNum_SC ParsePackage(Package package)
	{
		var structLength = Marshal.SizeOf(typeof(SMsgActionDestroyNum_SC));
		return PackageHelper.BytesToStuct<SMsgActionDestroyNum_SC>(package.Data.Take(structLength).ToArray());
	}
	// ....wDestroyNum个SMsgPropDestroyEntity_SC
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropDestroyEntity_SC : INotifyArgs
{
	public Int64 uidEntity; //实体ID
	
	public static SMsgPropDestroyEntity_SC ParseResultPackage(byte[] dataBuffer, int offset, int length)
	{
		return PackageHelper.BytesToStuct<SMsgPropDestroyEntity_SC>(dataBuffer.Skip(offset).Take(length).ToArray());
	}
	
	
	public int GetEventArgsType()
	{
		throw new NotImplementedException();
	}
};

//单元基本属性
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_BoxValue
{
	public int BOX_FIELD_STATE;		        // 盒子状态
	public int BOX_FIELD_TYPE;				// 盒子类型
	public int BOX_FIELD_DIR;
	public int UNIT_FIELD_FIGHT_HOSTILITY;                              //战斗中的敌对关系
}

//单元基本属性
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_TrapValue
{
	public int TRAP_FIELD_STATE;		        // 陷阱状态
	public int TRAP_FIELD_TYPE;				    // 陷阱类型
	public int TRAP_FIELD_OPENNEEDTIME;		    // 打开时间
	public int TRAP_FIELD_RESID;				// 陷阱外观资源ID
	public int TRAP_FIELD_MAPINLINEIDX;		    // 场景内嵌索引
}
//陷阱实体结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgPropCreateEntity_SC_Trap : INotifyArgs, IEntityDataStruct
{
	private SMsgPropCreateEntity_SC_Header m_sMsg_Header;
	
	// ......................   // 创建现场
	public Int64 UID;           //GUID全局唯一
	public int MapID;           //地图ID
	public int TrapX;         //陷阱X坐标
	public int TrapY;         //陷阱Y坐标
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	public byte[] Name;         //陷阱名称
	public SMsgPropCreateEntity_SC_BaseValue BaseValue;
	public SMsgPropCreateEntity_SC_TrapValue TrapValue;
	
	/// <summary>
	/// 从整体字节数组获得实体上下文的具体数据
	/// </summary>
	/// <param name="dataBuffer"></param>
	/// <returns></returns>
	public static SMsgPropCreateEntity_SC_Trap ParsePackage(byte[] dataBuffer, int offset)
	{
		Package package = PackageHelper.ParseReceiveData(dataBuffer);
		return ParsePackage(package, offset);
	}
	public static SMsgPropCreateEntity_SC_Trap ParsePackage(Package package, int offset)
	{
		var structLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Trap));
		var headLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
		var offsetBuffer = package.Data.Skip(offset).Take(structLength).ToArray();
		
		SMsgPropCreateEntity_SC_Trap sMsgPropCreateEntity_SC_Trap = new SMsgPropCreateEntity_SC_Trap();
		
		sMsgPropCreateEntity_SC_Trap.m_sMsg_Header = SMsgPropCreateEntity_SC_Header.ParsePackage(package, offset);
		int of = headLength;
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Trap.UID);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Trap.MapID);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Trap.TrapX);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Trap.TrapY);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Trap.Name,19);
		
		byte[] baseValues, trapValues;
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out baseValues,4*2);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out trapValues, 4 * 5);
		
		//sMsgPropCreateEntity_SC_Trap.UID = BitConverter.ToInt64(offsetBuffer, headLength);
		//sMsgPropCreateEntity_SC_Trap.MapID = BitConverter.ToInt32(offsetBuffer, headLength + 8);
		//sMsgPropCreateEntity_SC_Trap.TrapX = BitConverter.ToInt32(offsetBuffer, headLength + 8 + 4);
		//sMsgPropCreateEntity_SC_Trap.TrapY = BitConverter.ToInt32(offsetBuffer, headLength + 8 + 4 + 4);
		//sMsgPropCreateEntity_SC_Trap.Name = offsetBuffer.Skip(headLength + 8 + 4 + 4 + 4).Take(19).ToArray();
		
		sMsgPropCreateEntity_SC_Trap.BaseValue = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_BaseValue>(baseValues);
		sMsgPropCreateEntity_SC_Trap.TrapValue = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_TrapValue>(trapValues);
		
		return sMsgPropCreateEntity_SC_Trap;
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
		throw new NotImplementedException();
	}
};
//子弹实体结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_Bullet : INotifyArgs, IEntityDataStruct
{
	public SMsgPropCreateEntity_SC_Header m_sMsg_Header;
	
	public Int64 GUID;          //GUID全局唯一
	public int MapID;           //地图ID
	public int PosX;            //子弹X坐标
	public int PosY;            //子弹Y坐标
	public float DirX;            //向量
	public float DirY;
    public Int64 TargetId;          //子弹目标Id
	public int TargetX;           //目标点X坐标
	public int TargetY;      //目标点Y坐标
	public Int64 CasterUID;     //子弹所属实体ID
	public SMsgPropCreateEntity_SC_BaseValue BaseValue;     
	
	public static SMsgPropCreateEntity_SC_Bullet ParsePackage(Package pkg, int offer)
	{
		SMsgPropCreateEntity_SC_Bullet sc_bullet = new SMsgPropCreateEntity_SC_Bullet();
		sc_bullet.m_sMsg_Header = SMsgPropCreateEntity_SC_Header.ParsePackage(pkg, offer);
		int headLength = Marshal.SizeOf(sc_bullet.m_sMsg_Header);
		
		int of = offer + headLength;
        of += PackageHelper.ReadData(pkg.Data, out sc_bullet.GUID, of);
        of += PackageHelper.ReadData(pkg.Data, out sc_bullet.MapID, of);
        of += PackageHelper.ReadData(pkg.Data, out sc_bullet.PosX, of);
        of += PackageHelper.ReadData(pkg.Data, out sc_bullet.PosY, of);
        of += PackageHelper.ReadData(pkg.Data, out sc_bullet.DirX, of);
        of += PackageHelper.ReadData(pkg.Data, out sc_bullet.DirY, of);
		of += PackageHelper.ReadData(pkg.Data, out sc_bullet.TargetId, of);
        of += PackageHelper.ReadData(pkg.Data, out sc_bullet.TargetX, of);        
        of += PackageHelper.ReadData(pkg.Data, out sc_bullet.TargetY, of);
        of += PackageHelper.ReadData(pkg.Data, out sc_bullet.CasterUID, of);
		byte[] baseValues;
        of += PackageHelper.ReadData(pkg.Data, out baseValues, 4 * 2, of);
		
		sc_bullet.BaseValue = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_BaseValue>(baseValues);
		return sc_bullet;
	}
	
	public int GetEventArgsType()
	{
		throw new NotImplementedException();
	}
	
	public SMsgPropCreateEntity_SC_Header SMsg_Header
	{
		//\test by lee
		set
		{
			this.m_sMsg_Header = value;
		}
		
		get
		{
			return this.m_sMsg_Header;
		}
	}
	
	public void UpdateValue(short index, int value)
	{
		//find and update
		//TraceUtil.Log("同步子弹" + GUID.ToString() + " 属性id:" + index + " 更新的值:" + value);
	}
}

//子弹位置实体
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct sMsgButtlePos
{
	public float dirX;
	public float dirY;
	public float posX;
	public float posY;
	
	public static sMsgButtlePos ParsePackage(byte[] dataBuffer)
	{
		sMsgButtlePos buttlePos = new sMsgButtlePos();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out buttlePos.dirX);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out buttlePos.dirY);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out buttlePos.posX);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out buttlePos.posY);
		
		//buttlePos.dirX = BitConverter.ToSingle(dataBuffer, 0);
		//buttlePos.dirY = BitConverter.ToSingle(dataBuffer, 0 + 4);
		//buttlePos.posX = BitConverter.ToSingle(dataBuffer, 0 + 4 + 4);
		//buttlePos.posY = BitConverter.ToSingle(dataBuffer, 0 + 4 + 4 + 4);
		
		return buttlePos;
		//return PackageHelper.ParseDataBufferToStruct<sMsgButtlePos>(dataBuffer);
	}
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_ChannelValue
{
	public int MAST_FIELD_VISIBLE_RESID;		        // 资源Id
	public int MAST_FIELD_VISIBLE_TYPE;				    // 传送点类型
	public int MAST_FIELD_VISIBLE_STATE;
	
	public SMsgPropCreateEntity_SC_ChannelValue SetValue(int index, int value)
	{
		var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_ChannelValue>(this);
		
		int offset = index * 4;
		var source = BitConverter.GetBytes(value);
		bytes[offset] = source[0];
		bytes[offset + 1] = source[1];
		bytes[offset + 2] = source[2];
		bytes[offset + 3] = source[3];
		return PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_ChannelValue>(bytes);
	}
}
//传送门实体结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgPropCreateEntity_SC_Channel : INotifyArgs, IEntityDataStruct
{
	private SMsgPropCreateEntity_SC_Header m_sMsg_Header;
	
	// ......................   // 创建现场
	public Int64 UID;           //GUID全局唯一
	public int MapID;           //地图ID
	public int ChannelX;         //传送门X坐标
	public int ChannelY;         //传送门Y坐标
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	public byte[] Name;         //传送门名称
	public SMsgPropCreateEntity_SC_BaseValue BaseValue;
	public SMsgPropCreateEntity_SC_ChannelValue ChannelValue;
	
	/// <summary>
	/// 从整体字节数组获得实体上下文的具体数据
	/// </summary>
	/// <param name="dataBuffer"></param>
	/// <returns></returns>
	public static SMsgPropCreateEntity_SC_Channel ParsePackage(byte[] dataBuffer, int offset)
	{
		Package package = PackageHelper.ParseReceiveData(dataBuffer);
		return ParsePackage(package, offset);
	}
	public static SMsgPropCreateEntity_SC_Channel ParsePackage(Package package, int offset)
	{
		var structLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Channel));
		var headLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
		var offsetBuffer = package.Data.Skip(offset).Take(structLength).ToArray();
		
		SMsgPropCreateEntity_SC_Channel sMsgPropCreateEntity_SC_Channel = new SMsgPropCreateEntity_SC_Channel();
		sMsgPropCreateEntity_SC_Channel.m_sMsg_Header = SMsgPropCreateEntity_SC_Header.ParsePackage(package, offset);
		
		int of = headLength;
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Channel.UID);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Channel.MapID);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Channel.ChannelX);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Channel.ChannelY);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Channel.Name,19);
		byte[] baseValues, channelValues;
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out baseValues,4*2);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out channelValues,4*3);
		
		//sMsgPropCreateEntity_SC_Channel.UID = BitConverter.ToInt64(offsetBuffer, headLength);
		//sMsgPropCreateEntity_SC_Channel.MapID = BitConverter.ToInt32(offsetBuffer, headLength + 8);
		//sMsgPropCreateEntity_SC_Channel.ChannelX = BitConverter.ToInt32(offsetBuffer, headLength + 8 + 4);
		//sMsgPropCreateEntity_SC_Channel.ChannelY = BitConverter.ToInt32(offsetBuffer, headLength + 8 + 4 + 4);
		//sMsgPropCreateEntity_SC_Channel.Name = offsetBuffer.Skip(headLength + 8 + 4 + 4 + 4).Take(19).ToArray();
		sMsgPropCreateEntity_SC_Channel.BaseValue = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_BaseValue>(baseValues);
		sMsgPropCreateEntity_SC_Channel.ChannelValue = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_ChannelValue>(channelValues);
		//Debug.LogWarning("传送门状态：" + sMsgPropCreateEntity_SC_Channel.ChannelValue.MAST_FIELD_VISIBLE_TYPE);
		return sMsgPropCreateEntity_SC_Channel;
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
		if (index < 2)   //2
		{
			this.BaseValue = this.BaseValue.SetValue(index, value);
		}
		else if (index < 13)   // 2+11
		{
			this.ChannelValue = this.ChannelValue.SetValue(index - 2, value);
		}
	}
};

//NPC实体结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgPropCreateEntity_SC_NPCUnitValue
{
	public int UNIT_FIELD_LEVEL;        //等级
	public int UNIT_FIELD_DIR;          //方向
	public int UNIT_FIELD_PORTIAITID;   //生物头像ID
	public int UNIT_FIELD_DISPLAYID;    //美术资源ID
	public int UNIT_FIELD_ISGROUP;      //是否在组队状态 0无组队 1队员 2队长
	public int UNIT_FIELD_ISFIGHT;      //是否在战斗状态 0为非战斗，1为战斗
	public int UNIT_FIELD_ISTASK;       //是否在任务状态 0为非任务，1为任务
}

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgPropCreateEntity_SC_NPCValue
{
	public int CREATURE_FIELD_DISPLAYID;							// Creature的模型Id
	public int CREATURE_FIELD_NPC_TYPE;		    					// NPC类型
	public int CREATURE_FIELD_CAN_TALK;		    					// 是否可以对话
}

//NPC实体结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgPropCreateEntity_SC_NPC : INotifyArgs, IEntityDataStruct
{
	private SMsgPropCreateEntity_SC_Header m_sMsg_Header;
	
	// ......................   // 创建现场
	public Int64 UID;           //GUID全局唯一
	public int MapID;           //地图ID
	public int NPCX;            //NPCX坐标
	public int NPCY;            //NPCY坐标
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	public byte[] Name;         //陷阱名称
	
	public SMsgPropCreateEntity_SC_BaseValue BaseValue;
	public SMsgPropCreateEntity_SC_NPCUnitValue UnitValue;
	public SMsgPropCreateEntity_SC_NPCValue NPCValue;
	
	/// <summary>
	/// 从整体字节数组获得实体上下文的具体数据
	/// </summary>
	/// <param name="dataBuffer"></param>
	/// <returns></returns>
	public static SMsgPropCreateEntity_SC_NPC ParsePackage(byte[] dataBuffer, int offset)
	{
		Package package = PackageHelper.ParseReceiveData(dataBuffer);
		return ParsePackage(package, offset);
	}
	public static SMsgPropCreateEntity_SC_NPC ParsePackage(Package package, int offset)
	{
		////TraceUtil.Log("***************************************package.Data.Length" + package.Data.Length);
		var structLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_NPC));
		var headLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
		var offsetBuffer = package.Data.Skip(offset).Take(structLength).ToArray();
		
		SMsgPropCreateEntity_SC_NPC sMsgPropCreateEntity_SC_NPC = new SMsgPropCreateEntity_SC_NPC();
		sMsgPropCreateEntity_SC_NPC.m_sMsg_Header = SMsgPropCreateEntity_SC_Header.ParsePackage(package, offset);
		
		int of = headLength;
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_NPC.UID);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_NPC.MapID);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_NPC.NPCX);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_NPC.NPCY);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_NPC.Name,19);
		byte[] baseValues, unitValues, npcValues;
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out baseValues,4*2);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out unitValues,4*7);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out npcValues,4*3);
		
		//sMsgPropCreateEntity_SC_NPC.UID = BitConverter.ToInt64(offsetBuffer, headLength);
		//sMsgPropCreateEntity_SC_NPC.MapID = BitConverter.ToInt32(offsetBuffer, headLength + 8);
		//sMsgPropCreateEntity_SC_NPC.NPCX = BitConverter.ToInt32(offsetBuffer, headLength + 8 + 4);
		//sMsgPropCreateEntity_SC_NPC.NPCY = BitConverter.ToInt32(offsetBuffer, headLength + 8 + 4 + 4);
		//sMsgPropCreateEntity_SC_NPC.Name = offsetBuffer.Skip(headLength + 8 + 4 + 4 + 4).Take(19).ToArray();
		
		sMsgPropCreateEntity_SC_NPC.BaseValue = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_BaseValue>(baseValues);
		sMsgPropCreateEntity_SC_NPC.UnitValue = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_NPCUnitValue>(unitValues);
		sMsgPropCreateEntity_SC_NPC.NPCValue = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_NPCValue>(npcValues);
		
		return sMsgPropCreateEntity_SC_NPC;
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
		throw new NotImplementedException();
	}
};


public class RoleBuffList
{
	public List<SMsgActionWorldObjectAddBuff_SC> BufferList;
	public RoleBuffList()
	{
		BufferList = new List<SMsgActionWorldObjectAddBuff_SC>();
	}
	public void AddBuffer(SMsgActionWorldObjectAddBuff_SC sMsgActionWorldObjectAddBuff_SC)
	{
		if (!this.BufferList.Contains(sMsgActionWorldObjectAddBuff_SC))
		{
			foreach (var child in this.BufferList)
			{
				if (child.dwBuffId == sMsgActionWorldObjectAddBuff_SC.dwBuffId)
				{
					this.BufferList.Remove(child);
					break;
				}
			}
			BufferList.Add(sMsgActionWorldObjectAddBuff_SC);
		}
	}
	public void RemoveBuffer(int Index)
	{
		for(int i = this.BufferList.Count-1;i>=0;i--)
		{
			if(BufferList[i].dwIndex == Index)
			{
				BufferList.RemoveAt(i);
			}
		}
	}
}
/// <summary>
/// Buff实体结构
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionWorldObjectAddBuff_SC
{
	public SMsgActionSCHead SMsgActionSCHead;
	public byte dwLevel;				// 等级 // 调整顺序，等级放在第一位，当等级为0的时候，代表需要额外保存目标模板ID
	public uint dwIndex;				// 索引	
	public ushort dwBuffId;				// buff ID
	public byte nDurativeType;			// 持续类型 enum Buff_LastOutType：
	public uint dwLeftNum;				// 持续时间	或者剩余数量
	public byte bRandEffect;		    // 是否启动随机效果		
	public byte bIndexRandFlash;		// 随机光效下标（第一个为1）
	public byte byEffectNum;			// 效果数量
	
	public static SMsgActionWorldObjectAddBuff_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.ParseDataBufferToStruct<SMsgActionWorldObjectAddBuff_SC>(dataBuffer);
	}
	
	//public static SMsgActionWorldObjectAddBuff_SC ParsePackage(byte[] dataBuffer)
	//{
	//    var headLength = Marshal.SizeOf(typeof(SMsgActionSCHead));
	//    SMsgActionWorldObjectAddBuff_SC sMsgActionWorldObjectAddBuff_SC = new SMsgActionWorldObjectAddBuff_SC();
	
	//    sMsgActionWorldObjectAddBuff_SC.SMsgActionSCHead = new SMsgActionSCHead() { uidEntity = BitConverter.ToInt64(dataBuffer, 0) };
	//    sMsgActionWorldObjectAddBuff_SC.dwLevel = dataBuffer[8];
	//    sMsgActionWorldObjectAddBuff_SC.dwIndex = BitConverter.ToUInt32(dataBuffer, 8 + 1);
	//    sMsgActionWorldObjectAddBuff_SC.dwBuffId = BitConverter.ToUInt16(dataBuffer, 8 + 1 + 4);
	//    sMsgActionWorldObjectAddBuff_SC.nDurativeType = dataBuffer[8 + 1 + 4 + 2];
	//    sMsgActionWorldObjectAddBuff_SC.dwLeftNum = BitConverter.ToUInt32(dataBuffer, 8 + 1 + 4 + 2 + 1);
	//    sMsgActionWorldObjectAddBuff_SC.bRandEffect = dataBuffer[8 + 1 + 4 + 2 + 1 + 4];
	//    sMsgActionWorldObjectAddBuff_SC.bIndexRandFlash = dataBuffer[8 + 1 + 4 + 2 + 1 + 4 + 1];
	//    sMsgActionWorldObjectAddBuff_SC.byEffectNum = dataBuffer[8 + 1 + 4 + 2 + 1 + 4 + 1 + 1];
	
	//    return sMsgActionWorldObjectAddBuff_SC;
	//}
};

/// <summary>
/// Buff实体结构
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionWorldObjectRemoveBuff_SC
{
	public SMsgActionSCHead SMsgActionSCHead;
	public uint DwIndex;
	
	public static SMsgActionWorldObjectRemoveBuff_SC ParsePackage(byte[] dataBuffer)
	{
		return PackageHelper.ParseDataBufferToStruct<SMsgActionWorldObjectRemoveBuff_SC>(dataBuffer);
	}
	
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgPropUpdateProp_SC
{
	public long uidEntity;
	public byte nUpdateMode;	// 更新模式：1、单个数值属性更新，2：整个属性更新
	//       3：单个字符属性更新，4：多个数值属性更新 5：单独属性 按第几个字节（从0开始）更新  6:多个单独属性 按第几个字节（从0开始）更新
	// ......................   // 更新现场
};
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SingleNumUpdateElement
{
	public short ID;
	public int Value;
}
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct MultiNumUpdateElement
{
	public int Num;
	public short ID;
	public int Value;
}
public struct EntityDataUpdateNotify:INotifyArgs
{
	public Int64 EntityUID;
	public TypeID nEntityClass;	// 实体类型
	public bool IsHero;
	public byte UpdateMode;
	public short[] Indexs;
	public int[] Values;
	public int GetEventArgsType()
	{
		return 0;
	}
}
public class EntityIndexReCalc
{
	static IEntityIndexCalc entityIndexCalc;
	/// <summary>
	/// 按一定策略重新计算实体属性的Index
	/// </summary>
	/// <param name="type"></param>
	/// <param name="sourceIndex"></param>
	/// <returns></returns>
	public static short Calc(TypeID type, short sourceIndex)
	{
		short index = sourceIndex;
		bool needReCalc = true;
		switch (type)
		{
		case TypeID.TYPEID_PLAYER:
			entityIndexCalc =new PlayerIndexReCalc();                
			break;
		case TypeID.TYPEID_MONSTER:
			entityIndexCalc = new MonsterIndexReCalc();
			break;
		case TypeID.TYPEID_CHUNNEL:
			entityIndexCalc = new ChunnelIndexReCalc();
			break;
		default:
			needReCalc = false;
			break;
		}
		if (needReCalc)
		{
			return entityIndexCalc.ReCalcIndex(index);
		}
		else
		{
			return index;
		}
	}
}
public class PlayerIndexReCalc:IEntityIndexCalc
{
	public short ReCalcIndex(short sourceIndex)
	{
		int baseValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_BaseValue>()/4;
		int unitValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_Player_UnitValue>()/4;

		if (sourceIndex < CommonMsgDefineManager.SERVER_SKIP_INDEX)
		{
			return (short)(sourceIndex);  //-2表示上个if跳过的Index  +2表示基础属性
		}
		else
		{
			int index=sourceIndex - CommonMsgDefineManager.SERVER_SKIP_INDEX + baseValueIndex + unitValueIndex;
			return (short)(index);
		}
	}
}
public class MonsterIndexReCalc : IEntityIndexCalc
{
	public short ReCalcIndex(short sourceIndex)
	{
		int baseValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_BaseValue>()/4;
		int unitValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_Player_UnitValue>()/4;

		if (sourceIndex < CommonMsgDefineManager.SERVER_SKIP_INDEX)
		{
			return (short)sourceIndex; 
		}
		else
		{
			return (short)(sourceIndex - CommonMsgDefineManager.SERVER_SKIP_INDEX + baseValueIndex + unitValueIndex);
		}
	}
}
public class ChunnelIndexReCalc : IEntityIndexCalc
{
	public short ReCalcIndex(short sourceIndex)
	{
		return sourceIndex;
	}
}
/// <summary>
/// 玩家复活请求
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionRelivePlayer_CS
{
	public int actorPlayer;			//复活发起者
	public int actorTarget;			//复活接收者
	public byte byReliveType;		// 复活类型
	
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_RELIVE);
		Pak.Data = PackageHelper.StructToBytes<SMsgActionRelivePlayer_CS>(this);
		return Pak;
	}
}
// 复活类型
public enum EctypeRevive
{
	ER_NORMAL = 0,				// 普通复活
	ER_PREFECT,					// 完美复活
}

///// <summary>
///// 复活消息头
///// </summary>
//[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
//public struct SMsgActionSCHead
//{
//    public long uidEntity;			// 被复活的对象
//    public static SMsgActionSCHead ParsePackge(byte[] dataBuffer)
//    {
//        SMsgActionSCHead sMsgActionSCHead = PackageHelper.BytesToStuct<SMsgActionSCHead>(dataBuffer);
//        return sMsgActionSCHead;
//    }
//}
/// <summary>
/// 复活消息体
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionRelivePlayer_SC:INotifyArgs
{
	public Int64 playerUID;
	public int actorTarget;//被复活的人
	
	public int GetEventArgsType()
	{
		throw new NotImplementedException();
	}
};

/// <summary>
/// 触碰箱子
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionTouchBox_CS
{
	public Int64 uidPlayer;			//箱子触发者
	public Int64 uidBox;				//被触发的箱子
	
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_TOUCHBOX);
		Pak.Data = PackageHelper.StructToBytes<SMsgActionTouchBox_CS>(this);
		return Pak;
	}
};
// 更新玩家外观资源
//#define MSG_ACTION_PLAYER_CLEAN_SHOW
// 场景服
public struct SC_MSG_UPDATE_SHOW_CONTEXT
{
	public Int64 uidEntity;				// 实体ID	
	public byte byUpdateShowResCount;	// 更新数量
	
	public UPDATE_SHOW_ITEM[] UPDATE_SHOW_ITEMs;
	
	public static SC_MSG_UPDATE_SHOW_CONTEXT ParsePackage(byte[] buffer)
	{
		int of = 0;
		SC_MSG_UPDATE_SHOW_CONTEXT SC_MSG_UPDATE_SHOW_CONTEXT = new SC_MSG_UPDATE_SHOW_CONTEXT();
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out SC_MSG_UPDATE_SHOW_CONTEXT.uidEntity);
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out SC_MSG_UPDATE_SHOW_CONTEXT.byUpdateShowResCount);
		if(SC_MSG_UPDATE_SHOW_CONTEXT.byUpdateShowResCount>0)
		{
			SC_MSG_UPDATE_SHOW_CONTEXT.UPDATE_SHOW_ITEMs = new UPDATE_SHOW_ITEM[SC_MSG_UPDATE_SHOW_CONTEXT.byUpdateShowResCount];
			
			for (int i = 0; i < SC_MSG_UPDATE_SHOW_CONTEXT.byUpdateShowResCount; i++)
			{
				of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out  SC_MSG_UPDATE_SHOW_CONTEXT.UPDATE_SHOW_ITEMs[i].Index);
				of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out  SC_MSG_UPDATE_SHOW_CONTEXT.UPDATE_SHOW_ITEMs[i].GoodId);
			}
		}
		
		return SC_MSG_UPDATE_SHOW_CONTEXT;
	}
	
};
public struct UPDATE_SHOW_ITEM
{
	public byte Index;                 //更新位置， 0 表示第一个位置  武器。 1 表示第二个位置  外观资源
	public uint GoodId;                //配置表上的Item的GoodId  武器，在Equipment  lGoodsId , 外观 暂时没有配表。先不处理。
}

//角色修炼经脉
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionMeridianParctice_CS
{
	public int lKongfuID;				//秘籍ID
	public int lMeridiansID;			//经脉ID
	public int lParcticeNum;			//角色要修炼的修炼值
	
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING,CommonMsgDefineManager.MSG_ACTION_PARCTICE);
		pak.Data = PackageHelper.StructToBytes<SMsgActionMeridianParctice_CS>(this);
		return pak;
	}
	
};

//妖女信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgActionYaoNv_SC
{
	public byte byYaoNvNum;				//已修炼妖女个数
	public SYaoNvContext[] YaoNvContext;//妖女现场 妖女ID+妖女炼化等级

	public byte	byTotalYaoNvNum;			//总妖女个数
	public SYaoNvCondtionInfo[] YaoNvCondtionInfo;//条件现场

	public static SMsgActionYaoNv_SC ParsePackage(byte[] buffer)
	{
		SMsgActionYaoNv_SC YaoNv = new SMsgActionYaoNv_SC();
		int of = 0;
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out YaoNv.byYaoNvNum);
		YaoNv.YaoNvContext = new SYaoNvContext[YaoNv.byYaoNvNum];
		for (int i = 0; i < YaoNv.byYaoNvNum; i++)
		{
			YaoNv.YaoNvContext[i] = SYaoNvContext.ParsePackage(ref of, buffer);
		}

		// **新增 妖女收服条件
		of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out YaoNv.byTotalYaoNvNum);
		YaoNv.YaoNvCondtionInfo = new SYaoNvCondtionInfo[YaoNv.byTotalYaoNvNum];
		for(int i =0; i< YaoNv.byTotalYaoNvNum ; i++)
		{
			of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out YaoNv.YaoNvCondtionInfo[i].byYaoNvID );
			YaoNv.YaoNvCondtionInfo[i].byCondition = new byte[5];
			for(int j =0 ;j<YaoNv.YaoNvCondtionInfo[i].byCondition.Length;j++)
			{
				of+= PackageHelper.ReadData(buffer.Skip(of).ToArray(), out YaoNv.YaoNvCondtionInfo[i].byCondition[j]);
			}
		}
		// **
		return YaoNv;
	}
};

//条件现场
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SYaoNvCondtionInfo
{
	public byte byYaoNvID;
	public byte[] byCondition;//暂定长度为5  0-无条件 1-代表不满足，2-代表满足

	public static SYaoNvCondtionInfo ParsePackage(byte[] dataBuffer)
	{
		SYaoNvCondtionInfo sYaoNvCondtionInfo = new SYaoNvCondtionInfo();
		int of = 0;
		of+= PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sYaoNvCondtionInfo.byYaoNvID);
		sYaoNvCondtionInfo.byCondition = new byte[5];
		for(int i =0 ;i<sYaoNvCondtionInfo.byCondition.Length;i++)
		{
			of+= PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sYaoNvCondtionInfo.byCondition[i]);
		}
		return sYaoNvCondtionInfo;
	}
}

//妖女现场
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SYaoNvContext
{
	public byte byYaoNvID;
	public byte byLevel;
	public byte byAssembly;//0 为未装配 1为装配
	public int lExperience;
		
	public static SYaoNvContext ParsePackage(ref int offset, byte[] buffer)
	{
		SYaoNvContext context = new SYaoNvContext();
		offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out context.byYaoNvID);
		offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out context.byLevel);
		offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out context.byAssembly);
		offset += PackageHelper.ReadData(buffer.Skip(offset).ToArray(), out context.lExperience);
		return context;
	}
}

//角色炼化妖女
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgActionLianHua_CS
{
	public byte byYaoNvID;				//炼化妖女ID
	//public byte byLianHuaLevel;			//炼化到下一等级
	public byte YaoNvOpType;//0-获取妖女，1-元宝获取妖女，2-升级妖女
	public int dwXiuWeiNum;	//炼化灌注修为，如果是使用元宝则不用填值
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_LIANHUA);
		pak.Data = PackageHelper.StructToBytes<SMsgActionLianHua_CS>(this);
		return pak;
	}
};

//角色炼化妖女应答
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionLianHua_SC : INotifyArgs
{
	public byte bySucess;				//是否成功
	public byte byYaoNvID;				//炼化妖女ID
	public byte byLianHuaLevel;			//炼化到下一等级
	public int dwCurXiuWeiNum;			//当前妖女拥有的修为值
	
	public static SMsgActionLianHua_SC ParsePackage(byte[] buffer)
	{
		SMsgActionLianHua_SC lianhua = new SMsgActionLianHua_SC();
		int of = 0;
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out lianhua.bySucess);
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out lianhua.byYaoNvID);
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out lianhua.byLianHuaLevel);
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out lianhua.dwCurXiuWeiNum);
		return lianhua;
	}
	
	public int GetEventArgsType()
	{
		return 0;
	}
};

//妖女参战请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgActionYaoNvJoin_CS
{
	public byte byYaoNvID;				//妖女ID

	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_YAONV_FIGHTING);
		pak.Data = PackageHelper.StructToBytes<SMsgActionYaoNvJoin_CS>(this);
		return pak;
	}
};

//妖女参战应答
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgActionYaoNvJoin_SC
{
	public byte byYaoNvID;				//妖女ID

	public static SMsgActionYaoNvJoin_SC ParsePackage(byte[] Buffer)
	{
		SMsgActionYaoNvJoin_SC sMsgActionYaoNvJoin_SC = new SMsgActionYaoNvJoin_SC();
		sMsgActionYaoNvJoin_SC.byYaoNvID = Buffer[0];
		//return PackageHelper.ParseDataBufferToStruct<SMsgActionYaoNvJoin_SC>(Buffer);
		return sMsgActionYaoNvJoin_SC;
	}

};


//下发宝树果实信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionFruit_SC
{
	public byte byFruitNum;				//果实个数
	public List<SMsgActionFruitContext_SC> SMsgActionFruitContextList;
	//果实数据现场
	public static SMsgActionFruit_SC ParsePackage(byte[] buffer)
	{
		SMsgActionFruit_SC sMsgActionFruit_SC = new SMsgActionFruit_SC();
		int off = 1;
		sMsgActionFruit_SC.byFruitNum = buffer[0];
		sMsgActionFruit_SC.SMsgActionFruitContextList = new List<SMsgActionFruitContext_SC>();
		for (int i = 0; i < sMsgActionFruit_SC.byFruitNum; i++)
		{
			SMsgActionFruitContext_SC sMsgActionFruitContext = new SMsgActionFruitContext_SC();
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext.byFruitPosition);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext.dwFruitID);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext.byFruitStatus);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext.dwStartTimes);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext.dwEndTime);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext.byFruitDryStatus);
			sMsgActionFruit_SC.SMsgActionFruitContextList.Add(sMsgActionFruitContext);
		}
		return sMsgActionFruit_SC;
	}
};

//宝树果实现场
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionFruitContext_SC
{
	public byte byFruitPosition;		//果实在树上的位置
	public uint dwFruitID;				//果实ID
	public byte byFruitStatus;			//果实状态，开花，结果，成熟
	public uint dwStartTimes;			//果实的开始时间
	public uint dwEndTime;				//当前时间结束时间戳
	public byte byFruitDryStatus;		//是否为干旱状态，是为1，不是为0
	
	public static SMsgActionFruitContext_SC ParsePackage(byte[] buffer)
	{
		SMsgActionFruitContext_SC sMsgActionFruitContext_SC = new SMsgActionFruitContext_SC();
		int off = 0;
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext_SC.byFruitPosition);
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext_SC.dwFruitID);
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext_SC.byFruitStatus);
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext_SC.dwStartTimes);
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext_SC.dwEndTime);
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionFruitContext_SC.byFruitDryStatus);
		return sMsgActionFruitContext_SC;
	}
};


//客户端向服务端发送获取某个果实
//应答为同步果实数据现场
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionChooseFruit_CS
{
	public byte byFruitPosition;
	
	public Package GeneratePackage(MasterMsgType masterMsgType, short subMsgType)
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)masterMsgType, subMsgType);
		pak.Data = PackageHelper.StructToBytes<SMsgActionChooseFruit_CS>(this);
		return pak;
	}
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionUseManna_SC
{
    public UInt32      dwFruitID;                   //果实ID
    public byte       bySucess;                    //使用仙露成功应答

    public static SMsgActionUseManna_SC ParsePackage(byte[] buffer)
    {
        SMsgActionUseManna_SC sMsgActionUseManna_SC = new SMsgActionUseManna_SC();
        int of = 0;
        of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgActionUseManna_SC.dwFruitID);
        of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgActionUseManna_SC.bySucess);
        return sMsgActionUseManna_SC;
    }

};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgActionChooseFruit_SC
{
    public UInt32 dwFruitID;                    //果实ID
    public byte   byFruitPosition;                    //奖励位置
    public UInt32 dwGoodsID;
    public UInt32 dwGoodsNum;

    public static SMsgActionChooseFruit_SC ParsePackage(byte[] buffer)
    {
        SMsgActionChooseFruit_SC sMsgActionChooseFruit_SC = new SMsgActionChooseFruit_SC();
        int of = 0;
        of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgActionChooseFruit_SC.dwFruitID);
        of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgActionChooseFruit_SC.byFruitPosition);
        of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgActionChooseFruit_SC.dwGoodsID);
        of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgActionChooseFruit_SC.dwGoodsNum);
        return sMsgActionChooseFruit_SC;
    }

};






//下发修炼信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionXiuLianInfo_SC : INotifyArgs
{
	public byte byXiuLianType;			//修炼类型(1.离线下发，2，在线下发，3，突破修为下发)
	public int XiuLianTime;			//修炼时间
	public int XiuLianNum;			//可获取的修为值
	
	public static SMsgActionXiuLianInfo_SC ParsePackage(byte[] buffer)
	{
		//return PackageHelper.ParseDataBufferToStruct<SMsgActionXiuLianInfo_SC>(buffer);
		SMsgActionXiuLianInfo_SC sMsgActionXiuLianInfo_SC = new SMsgActionXiuLianInfo_SC();
		int off = 0;
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionXiuLianInfo_SC.byXiuLianType);
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionXiuLianInfo_SC.XiuLianTime);
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionXiuLianInfo_SC.XiuLianNum);
		return sMsgActionXiuLianInfo_SC;
	}
	
	public int GetEventArgsType()
	{
		return 0;
	}
};

//获取妖女内丹请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgGetYaoNvNeiDan_CS
{
	public byte YaoNvID;				//妖女ID
	
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_GET_YAONV_NEIDAN);
		pak.Data = PackageHelper.StructToBytes<SMsgGetYaoNvNeiDan_CS>(this);
		return pak;
	}
};

//签到一开始主动下发
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionDaySignUI_SC
{
	public int dwGroupID;				//下发
	public byte CurDay;					//当前为星期几
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
	public byte[] Sign;				//签到标识集合
	
	public static SMsgActionDaySignUI_SC ParsePackage(byte[] buffer)
	{
		SMsgActionDaySignUI_SC sMsgActionDaySignUI_SC = new SMsgActionDaySignUI_SC();
		int signVal;
		int off = 0;
		sMsgActionDaySignUI_SC.Sign = new byte[7];
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionDaySignUI_SC.dwGroupID);
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionDaySignUI_SC.CurDay);
		off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sMsgActionDaySignUI_SC.Sign, 7);
		return sMsgActionDaySignUI_SC;
	}
};

//签到请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionDaySign_CS
{
	public byte	SignType;						//签到类型 0-正常签到，1-补签
	public byte	SignID;							//签到的位置，即签那一天 
	
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_DAYSIGNIN);
		pak.Data = PackageHelper.StructToBytes<SMsgActionDaySign_CS>(this);
		return pak;
	}
};

//签到请求回应
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionDaySign_SC
{
	public byte SignID;							//签到的位置，即签那一天 
	public byte bSucess;						//签到是否成功
	
	public static SMsgActionDaySign_SC ParsePackage(byte[] buffer)
	{
		SMsgActionDaySign_SC sMsgActionDaySign_SC = new SMsgActionDaySign_SC();
		int of = 0;
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgActionDaySign_SC.SignID);
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgActionDaySign_SC.bSucess);
		return sMsgActionDaySign_SC;
	}
};

//活动
//主消息：交互数据 NET_ROOT_INTERACT = 7
	//活动奖励页面数据下发
	//INTERACT_MSG_OPEN_UI			= 18
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public	struct SMsgInteract_OpenUI
{
	public byte	byActiveNum;			//活动个数
	//。。活动现场
	public DGameActiveData[] activeList ;
	public static SMsgInteract_OpenUI ParsePackage(byte[] buffer)
	{
		SMsgInteract_OpenUI sMsgInteract_OpenUI = new SMsgInteract_OpenUI();
		sMsgInteract_OpenUI.byActiveNum = buffer[0];
		sMsgInteract_OpenUI.activeList = new DGameActiveData[sMsgInteract_OpenUI.byActiveNum];
		int off = 1;
		for (int i = 0; i < sMsgInteract_OpenUI.byActiveNum; i++)
		{
			DGameActiveData dGameActiveData = new DGameActiveData();
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out dGameActiveData.dwActiveID);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out dGameActiveData.dwActiveParam);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out dGameActiveData.byIndex);
			sMsgInteract_OpenUI.activeList[i] = dGameActiveData;
		}
		return sMsgInteract_OpenUI;
	}
};

//活动数据
public struct DGameActiveData
{
	public int dwActiveID;		//活动ID
	public int dwActiveParam;	//活动参数数据
	public byte  byIndex;			//活动已领取奖励索引 如一次都没领0，如第一个领取1，如最后一个领取为12//
};

//获取活动奖励发送
//INTERACT_MSG_GETREWARD			= 19
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteract_GetReward_CS
{
	public int dwRewardID;		//活动ID
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_INTERACT, CommonMsgDefineManager.INTERACT_MSG_GETREWARD);
		pak.Data = PackageHelper.StructToBytes<SMsgInteract_GetReward_CS>(this);
		return pak;
	}
};
//点击领取奖励收到
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteract_GetReward_SC
{
	public int dwRewardID;		//活动ID
	public byte byIndex;		//领取奖励索引(当值为0时，说明领取失败)
	public static SMsgInteract_GetReward_SC ParsePackage(byte[] buffer)
	{
		SMsgInteract_GetReward_SC sMsgInteract_GetReward_SC = new SMsgInteract_GetReward_SC();
		int of = 0;
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgInteract_GetReward_SC.dwRewardID);
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgInteract_GetReward_SC.byIndex);
		return sMsgInteract_GetReward_SC;
	}
};

/************************************
/ 武学
/********************************** */
//MSG_ACTION_WUXUEUI
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSendWuXueData	//武学数据单元
{
	public int dwWuXueID;				//武学ID
	public byte  byWuXueLevel;			//武学等级
};

//武学信息下发
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgActionWuXue_SC
{
	public byte	byWuXueNum;
	//..武学现场
	public SSendWuXueData[] WuXueDataList ;
	public static SMsgActionWuXue_SC ParsePackage(byte[] buffer)
	{
		SMsgActionWuXue_SC sMsgInteract_GetWuXue_SC = new SMsgActionWuXue_SC();
		sMsgInteract_GetWuXue_SC.byWuXueNum = buffer[0];
		sMsgInteract_GetWuXue_SC.WuXueDataList = new SSendWuXueData[sMsgInteract_GetWuXue_SC.byWuXueNum];
		int off = 1;
		for(int i = 0; i < sMsgInteract_GetWuXue_SC.byWuXueNum; i++)
		{
			SSendWuXueData sSendWuXueData = new SSendWuXueData();
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sSendWuXueData.dwWuXueID);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sSendWuXueData.byWuXueLevel);
			sMsgInteract_GetWuXue_SC.WuXueDataList[i] = sSendWuXueData;
		}
		return sMsgInteract_GetWuXue_SC;
	}
};

//MSG_ACTION_WUXUE_STUDY
//向服务端发送武学学习请求
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgActionStudyWuXue_CS
{
	public int dwWuXueID;				//武学ID
	public Package GeneratePackage()
	{
		Package pak = new Package();
		pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_WUXUE_STUDY);
		pak.Data = PackageHelper.StructToBytes<SMsgActionStudyWuXue_CS>(this);
		return pak;
	}
};

//服务端下发的武学学习结果
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgAcitonStudyWuXue_SC
{
	public int dwWuXueID;				//武学ID
	public byte  byWuXueLevel;				//武学等级
	public static SMsgAcitonStudyWuXue_SC ParsePackage(byte[] buffer)
	{
		SMsgAcitonStudyWuXue_SC sMsgActionStudyWuXue_SC = new SMsgAcitonStudyWuXue_SC();
		int of = 0;
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgActionStudyWuXue_SC.dwWuXueID);
		of += PackageHelper.ReadData(buffer.Skip(of).ToArray(), out sMsgActionStudyWuXue_SC.byWuXueLevel);
		return sMsgActionStudyWuXue_SC;
	}
};
//服务端下发的pvp历史战绩
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgActionPvpHistory_SC
{
	public byte	byHistoryNum;			//个人战绩个数
	public SPvPPersonRankDataSC[] PvPPersonRankData;

	public static SMsgActionPvpHistory_SC ParsePackage(byte[] buffer)
	{
		SMsgActionPvpHistory_SC sMsgActionPvpHistory_SC = new SMsgActionPvpHistory_SC();
		sMsgActionPvpHistory_SC.byHistoryNum = buffer[0];
		sMsgActionPvpHistory_SC.PvPPersonRankData=new SPvPPersonRankDataSC[sMsgActionPvpHistory_SC.byHistoryNum];
		int off = 1;
		for(int i = 0; i < sMsgActionPvpHistory_SC.byHistoryNum; i++)
		{
			SPvPPersonRankDataSC sPvPPersonRankDataSC = new SPvPPersonRankDataSC();
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sPvPPersonRankDataSC.dwSeasonID);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sPvPPersonRankDataSC.byGroupID);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sPvPPersonRankDataSC.dwRankIndex);
			off += PackageHelper.ReadData(buffer.Skip(off).ToArray(), out sPvPPersonRankDataSC.dwScore);
			sMsgActionPvpHistory_SC.PvPPersonRankData[i] = sPvPPersonRankDataSC;
		}
		return sMsgActionPvpHistory_SC;
	}
};
struct SPvPPersonRankDataSC
{
	public uint dwSeasonID;			//赛季ID
	public byte  byGroupID;			//赛季所在组
	public uint dwRankIndex;			//PVP排名
	public uint dwScore;				//赛季积分
};