using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

//鎬墿瀹炰綋缁撴瀯
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgPropCreateEntity_SC_Monster : INotifyArgs,IEntityDataStruct
{
	private SMsgPropCreateEntity_SC_Header m_sMsg_Header;
	// ......................   // 鍒涘缓鐜板満
	public Int64 UID;           //GUID鍏ㄥ眬鍞竴
	public int MapID;           //鍦板浘ID
	public int MonsterX;         //鎬墿X鍧愭爣
	public int MonsterY;         //鎬墿Y鍧愭爣
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	public byte[] Name;         //鎬墿鍚嶇О
	public SMsgPropCreateEntity_SC_BaseValue BaseObjectValues;
	public SMsgPropCreateEntity_SC_UnitVisibleValue MonsterUnitValues;
	public SMsgPropCreateEntity_SC_UnitInvisibleValue MonsterInvisibleValue;
	public SMsgPropCreateEntity_SC_MonsterValue MonsterValues;
	
	/// <summary>
	/// 浠庢暣浣撳瓧鑺傛暟缁勮幏寰楀疄浣撲笂涓嬫枃鐨勫叿浣撴暟鎹?
	/// </summary>
	/// <param name="dataBuffer"></param>
	/// <returns></returns>
	public static SMsgPropCreateEntity_SC_Monster ParsePackage(byte[] dataBuffer, int offset)
	{
		Package package = PackageHelper.ParseReceiveData(dataBuffer);
		return ParsePackage(package, offset);
	}
	public static SMsgPropCreateEntity_SC_Monster ParsePackage(Package package, int offset)
	{
		var structLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Monster));
		var headLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
		var offsetBuffer = package.Data.Skip(offset).Take(structLength).ToArray();
		
		SMsgPropCreateEntity_SC_Monster sMsgPropCreateEntity_SC_Monster = new SMsgPropCreateEntity_SC_Monster();
		
		sMsgPropCreateEntity_SC_Monster.m_sMsg_Header = SMsgPropCreateEntity_SC_Header.ParsePackage(package, offset);
		int of = headLength;
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Monster.UID);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Monster.MapID);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Monster.MonsterX);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Monster.MonsterY);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_Monster.Name,19);

		byte[] baseValues, monsterUnitValues,monsterInvisibleValue, monsterValues;
		int baseValueSize=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_BaseValue>();
		int unitVisibleValueSize=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_UnitVisibleValue>();
		int unitInvisibleValueSize=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_UnitInvisibleValue>();
		int monsterValueSize=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_MonsterValue>();
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out baseValues,baseValueSize);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out monsterUnitValues, unitVisibleValueSize);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out monsterInvisibleValue, unitInvisibleValueSize);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out monsterValues, monsterValueSize);
		sMsgPropCreateEntity_SC_Monster.BaseObjectValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_BaseValue>(baseValues);
		sMsgPropCreateEntity_SC_Monster.MonsterUnitValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_UnitVisibleValue>(monsterUnitValues);
		sMsgPropCreateEntity_SC_Monster.MonsterInvisibleValue = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_UnitInvisibleValue>(monsterInvisibleValue);
		sMsgPropCreateEntity_SC_Monster.MonsterValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_MonsterValue>(monsterValues);
		
		
		//TraceUtil.Log("PlayerSpeed:" + sMsgPropCreateEntity_SC_Monster.MonsterUnitValues.UNIT_FIELD_SPEED + "  " + sMsgPropCreateEntity_SC_Monster.MonsterUnitValues.UNIT_FIELD_SHARD);
		
		return sMsgPropCreateEntity_SC_Monster;
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
		if(value == 8800)
		{
			int a = 0;
		}

		int baseValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_BaseValue>()/4;
		int unitVisibleValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_UnitVisibleValue>()/4;
		int unitInvisibleValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_UnitInvisibleValue>()/4;

		int unitIndex=unitVisibleValueIndex + unitInvisibleValueIndex;
		if (index < baseValueIndex)   
		{
			this.BaseObjectValues = this.BaseObjectValues.SetValue(index, value);
		}
		else if (index < baseValueIndex+unitVisibleValueIndex)   
		{
			this.MonsterUnitValues = this.MonsterUnitValues.SetValue(index - baseValueIndex, value);
		}
		else if(index < baseValueIndex + unitIndex)
		{
			this.MonsterInvisibleValue = this.MonsterInvisibleValue.SetValue(index - baseValueIndex - unitVisibleValueIndex, value);
		}
		else 
		{
			this.MonsterValues = this.MonsterValues.SetValue(index - baseValueIndex-unitIndex, value);
		}      
	}

	public int GetValue(int index)
	{
		int value = 0;
		int baseValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_BaseValue>()/4;
		int unitVisibleValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_UnitVisibleValue>()/4;
		int unitInvisibleValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_UnitInvisibleValue>()/4;
		
		int unitIndex=unitVisibleValueIndex + unitInvisibleValueIndex;
		if (index < baseValueIndex)   
		{
			value = this.BaseObjectValues.GetValue(index);
		}
		else if (index < baseValueIndex+unitVisibleValueIndex)   
		{
			value = this.MonsterUnitValues.GetValue(index - baseValueIndex);
		}
		else if(index < baseValueIndex + unitIndex)
		{
			value = this.MonsterInvisibleValue.GetValue(index - baseValueIndex - unitVisibleValueIndex);
		}
		else 
		{
			value = this.MonsterValues.GetValue(index - baseValueIndex-unitIndex);
		}     


		return value;
	}
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_MonsterValue
{
	public int MONSTER_FIELD_ATTR_TYPE ;									//鎬墿绫诲瀷(棣栭,鍙樺紓)
	public int MONSTER_FIELD_TYPE	;																								//鎬墿灞炴€х被鍨嬶紙鏅€氾紝鍙樺寲锛屽浐瀹氾級
	public int MONSTER_FIELD_UNITTYPE	;																						//鎬墿鍗曚綅绫诲瀷---------
	//public int MONSTER_FIELD_LEVEL;																						//鎬墿绛夌骇---------
	public int MONSTER_FIELD_IDCARD;																							//鎬墿韬唤ID-----
	public int MONSTER_FIELD_HPCOUNT;                                       //鎬墿鐨凥P鏉℃暟------
	public int MONSTER_FIELD_RUNSPEED;			    						//鎬墿璺戝姩閫熷害
	public int MONSTER_FIELD_ISSHOW;										//怪物是否可见
	public int GetValue(int index)
	{
		var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_MonsterValue>(this);
		
		int offset = index * 4;
		return BitConverter.ToInt32(bytes.Skip(offset).ToArray(),0);
	}
	public SMsgPropCreateEntity_SC_MonsterValue SetValue(int index, int value)
	{
		var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_MonsterValue>(this);
		
		int offset = index * 4;
		var source = BitConverter.GetBytes(value);
		bytes[offset] = source[0];
		bytes[offset + 1] = source[1];
		bytes[offset + 2] = source[2];
		bytes[offset + 3] = source[3];
		
		return PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_MonsterValue>(bytes);     
	}
	
	
}
