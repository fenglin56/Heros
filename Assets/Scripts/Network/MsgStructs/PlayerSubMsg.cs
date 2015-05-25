using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

/*
 * 生物属性值增加与减少，需要检查和调整的地方：
 * 1、SMsgPropCreateEntity_SC_MainPlayer结构体的ParsePackage方法
 * 2、SMsgPropCreateEntity_SC_MainPlayer结构体的UpdateValue方法
 * 3、SMsgPropCreateEntity_SC_OtherPlayer结构体的ParsePackage方法
 * 4、SMsgPropCreateEntity_SC_OtherPlayer结构体的UpdateValue方法
 * 5、SMsgPropCreateEntity_SC_Monster结构体的ParsePackage方法
 * 6、SMsgPropCreateEntity_SC_Monster结构体的UpdateValue方法
 * 7、EntityController类的UpdateEntityValue方法
 * 8、PlayerIndexReCalc类的Calc接口实现
 * 9、MonsterIndexReCalc类的Calc接口实现
 * */

//主玩家实体结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_MainPlayer : INotifyArgs, IEntityDataStruct, IPlayerDataStruct
{
    private SMsgPropCreateEntity_SC_Header m_sMsg_Header;

    // ......................   // 创建现场
    public int ActorID;         //角色ID
    public Int64 UID;           //GUID全局唯一
    public int MapID;           //地图ID
    public int m_playerX;         //角色X坐标  除以10
    public int m_playerY;         //角色Y坐标  除以10
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[] m_name;         //角色名称
	public SMsgPropCreateEntity_SC_BaseValue BaseObjectValues;
	public SMsgPropCreateEntity_SC_Player_UnitValue UnitValues;
	public SMsgPropCreateEntity_SC_MainPlayer_PlayerValue PlayerValues;


    public string Name
    {
        set
        {
            m_name = new byte[128];
            Encoding.UTF8.GetBytes(value).CopyTo(m_name, 0);

        }
        get
        {
            return m_name != null ? Encoding.UTF8.GetString(m_name) : string.Empty;
        }
    }
    /// <summary>
    /// 从整体字节数组获得实体上下文的具体数据
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <returns></returns>
    public static SMsgPropCreateEntity_SC_MainPlayer ParsePackage(byte[] dataBuffer,int offset)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        return ParsePackage(package, offset);
    }
    public static SMsgPropCreateEntity_SC_MainPlayer ParsePackage(Package package, int offset)
    {
        var structLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_MainPlayer));
        var headLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
        var offsetBuffer = package.Data.Skip(offset).Take(structLength).ToArray();

        SMsgPropCreateEntity_SC_MainPlayer sMsgPropCreateEntity_SC_MainPlayer = new SMsgPropCreateEntity_SC_MainPlayer();
        sMsgPropCreateEntity_SC_MainPlayer.m_sMsg_Header = SMsgPropCreateEntity_SC_Header.ParsePackage(package, offset);

        int of = headLength;
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_MainPlayer.ActorID);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_MainPlayer.UID);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_MainPlayer.MapID);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_MainPlayer.m_playerX);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_MainPlayer.m_playerY);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_MainPlayer.m_name,19);

        byte[] baseValues, playerUnitValues, playerValues;
		int baseValueSize=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_BaseValue>();
		int unitValueSize=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_Player_UnitValue>();
		int playValueSize=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_MainPlayer_PlayerValue>();
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out baseValues,baseValueSize);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out playerUnitValues,unitValueSize);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out playerValues,playValueSize);	
        sMsgPropCreateEntity_SC_MainPlayer.BaseObjectValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_BaseValue>(baseValues);
        sMsgPropCreateEntity_SC_MainPlayer.UnitValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_Player_UnitValue>(playerUnitValues);
        sMsgPropCreateEntity_SC_MainPlayer.PlayerValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_MainPlayer_PlayerValue>(playerValues);


        return sMsgPropCreateEntity_SC_MainPlayer;
    }

    public void UpdateValue(short index, int value)
    {
        #region UpdateValueSwitch
		int baseValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_BaseValue>()/4;
		int unitValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_Player_UnitValue>()/4;
		if (index < baseValueIndex)   //SMsgPropCreateEntity_SC_BaseValue
        {
            this.BaseObjectValues=this.BaseObjectValues.SetValue(index,value);
        }
		else if(index < baseValueIndex+unitValueIndex)   //SMsgPropCreateEntity_SC_Player_UnitValue
        {
			this.UnitValues=this.UnitValues.SetValue(index-baseValueIndex,value);
        }
        else    //SMsgPropCreateEntity_SC_MainPlayer_PlayerValue
        {
			this.PlayerValues=this.PlayerValues.SetValue(index-baseValueIndex-unitValueIndex,value);
        }        
        #endregion
    }
    #region INotify
     public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
    #endregion  
   
    //public UpdatePart[] GetFiledNameByIndex(int[] indexs)
    //{
    //    UpdatePart[] fieldNames=new UpdatePart[indexs.Length];
    //    foreach(int i in indexs)
    //    {
    //         if(i>)
    //    }
    //}

     public SMsgPropCreateEntity_SC_Header SMsg_Header
     {
         get
         {
             return this.m_sMsg_Header;
         }
     }
     public byte GetPlayerKind(out string career)
     {
         byte kind = GetPlayerKind();
         switch (kind)
         {
             case 1:
                 career = "Char01";
                 break;
             case 2:
                 career = "Char04";
                 break;
             case 3:
                 career = "Char02";
                 break;
             case 4:
                 career = "Char03";
                 break;
             default:
                 career = "Char01";
                 break;
         }

         return kind;
     }
     public byte GetPlayerKind()
     {
         return (byte)this.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
     }

     public SMsgPropCreateEntity_SC_BaseValue GetBaseValue()
     {
         return this.BaseObjectValues;
     }

     public SMsgPropCreateEntity_SC_Player_UnitValue GetUnitValue()
     {
         return this.UnitValues;
     }

     public SMsgPropCreateEntity_SC_Player_CommonValue GetCommonValue()
     {
         return this.PlayerValues.PlayerCommonValue;
     }

     public int PlayerX
     {
         get
         {
             return this.m_playerX;
         }
     }

     public int PlayerY
     {
         get
         {
             return this.m_playerY;
         }
     }
     public int PlayerActorID
     {
         get
         {
             return this.ActorID;
         }
     }
};
public enum UpdatePart
{
    EquipStrength
}
/// <summary>
/// 单元基本属性
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_Player_UnitValue
{
    public SMsgPropCreateEntity_SC_UnitVisibleValue sMsgPropCreateEntity_SC_UnitVisibleValue;
    public SMsgPropCreateEntity_SC_UnitInvisibleValue sMsgPropCreateEntity_SC_UnitInvisibleValue;

    public int GetValue(int index)
    {
        var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_Player_UnitValue>(this);

        int offset = index * 4;
        return BitConverter.ToInt32(bytes.Skip(offset).ToArray(), 0);
    }
    public SMsgPropCreateEntity_SC_Player_UnitValue SetValue(int index, int value)
    {
        var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_Player_UnitValue>(this);

        int offset = index * 4;
        var source = BitConverter.GetBytes(value);
        bytes[offset] = source[0];
        bytes[offset + 1] = source[1];
        bytes[offset + 2] = source[2];
        bytes[offset + 3] = source[3];

        return PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_Player_UnitValue>(bytes);
    }
}

/// 
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_Player_CommonValue
{
	/***************** 玩家可见属性 基本属性 *******************************/
	public int PLAYER_FIELD_VISIBLE_VOCATION;							// 职业 每个创建的角色职业唯一的
	public int PLAYER_FIELD_VISIBLE_SEX;																							// 性别
	public int PLAYER_FIELD_VISIBLE_VIP;																	// vip等级
	public int PLAYER_FIELD_VISIBLE_MATEID;																					// 角色ID
    public int PLAYER_FIELD_VISIBLE_FASHION;                                    //时装ID
	public int PLAYER_FIELD_VISIBLE_TITLE;								//称号
	public int PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL;																					//玩家的威望等级
		/***************** 玩家可见属性 *******************************/
    public int GetValue(int index)
    {
        return index;
    }
    public void SetValue(int index, int value)
    {
        switch (index)
        {
            default:
                //TraceUtil.Log("Index有误");
                break;
        }
    }
}
/// <summary>
/// 主玩家基本属性
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_MainPlayer_PlayerValue
{
	public SMsgPropCreateEntity_SC_Player_CommonValue PlayerCommonValue;
	/***************** 玩家不可见属性 *****************************/
	public int PLAYER_FIELD_EXP;															// 经验
	public int PLAYER_FIELD_NEXT_LEVEL_EXP;																					// 升级所需经验
	public int PLAYER_FIELD_HOLDMONEY;																						// 铜钱
	public int PLAYER_FIELD_BINDMONEY;																						// 绑定铜钱
	public int PLAYER_FIELD_BINDPAY;																								// 元宝   (充值进来)
	public int PLAYER_FIELD_CURRENCY_COWRY;																				// 通宝(绑定元宝) (活动赠送 只能买特定东西)
    public int PLAYER_FIELD_UNCLAIMED_PAY;																				// 充值后尚未领取的元宝数
	public int PLAYER_FIELD_CURRENCY_ACTIVELIFE;																			//玩家当前活力值----
	public int PLAYER_FIELD_MAX_ACTIVELIFE;																					//玩家最大活力值----

	public int PLAYER_FIELD_POTENTIAL;																							// 当前潜力点数(技能点)
	public int PLAYER_FIELD_GM_LEVEL;																								// GM权限等级

	public int PLAYER_FIELD_PACKET_C_MAX_SIZE;																			//  背包格子数量 （当前最大格子数量）
	public int PLAYER_FIELD_STORAGE_C_MAX_SIZE;																			//  仓库格子数量
    public int PLAYER_FIELD_PRESTIGE;                                                   //威望值
    public int PLAYER_FIELD_NEXT_LEVEL_PRESTIGE;                                        //升级到下一威望等级所需威望
    public int PLAYER_FIELD_ARRIVE_MAX_PRESTIGE_LEVEL;                                  //玩家到达的最高威望等级
    public int PLAYER_FIELD_PVP_TIMES;                                                  //玩家能PVP的次数
    public int PLAYER_FIELD_WINNINGSTREAK_TIMES;                                        //玩家连胜次数

    //经脉系统属性
    public int PLAYER_FIELD_MERIDIANS_LEVEL;                                            //经脉等级
    public int PLAYER_FIELD_HAVEPRACTICE_NUM;                                           //内功已经修炼的修炼值
    public int PLAYER_FIELD_PRACTICE_NUM;                                               //角色拥有的可用修炼值

    public int PLAYER_FIELD_MANNA_NUM;                                                  //角色拥有的仙露数
    public int PLAYER_FIELD_MAXMANNA_BUYNUM;                                            //玩家当天购买的仙露数

    public int PLAYER_FIELD_ENERGY_NUM;													//角色能量值
	public int PLAYER_FIELD_MAX_ENERGY_NUM;												//角色最高能量值
	public int PLAYER_FILED_ENERGY_GRID_NUM;											//角色能量格
	public int PLAYER_FIELD_MAX_ENERGY_GRID_NUM;										//角色最大能量格
    public int PLAYER_FIELD_BREAKOUT_FLAG;												//曝气标识
	public int PLAYER_FIELD_BREAKTHROUGHNUM;				    						//房间中修为突破次数
	public int PLAYER_FIELD_HOUSEID;                    	                            //炼功房ID
	public int PLAYER_FIELD_STARTHOUSETIME;             	                            //炼功房开始时间
	public int PLAYER_FIELD_STARTXIULIANTIME;           	                            //修炼开始时间
    public int PLAYER_FIELD_ENDXIULIANTIME;             	                            //修炼结束时间
    public int PLAYER_FIELD_SHILIAN_TIMES;                                              //试炼次数

    public int PLAYER_FIELD_CANBUYACTIVE_NUM;                                           //角色可购买活力次数	
    
    public int PLAYER_FIELD_LASTACTIVE_TIME;											//角色活力回复已经过去多少时间
	public int PLAYER_FIELD_ACTIVE_VALUE;												//角色活跃值
    public int PLAYER_FIELD_MAX_ACTIVE_VALUE;											//角色最大活跃值
	public int PLAYER_FIELD_GOLD_TOTALCOST_VALUE;											//角色累积消耗元宝数

    public int PALYER_FIELD_GOLD_TOTALTOPUP_VALUE;                                                            //角色累积充值数
    public int PLAYER_FIELD_LUCKDRAW_VALUE;                                             //剩余抽奖数
	public int PLAYER_FIELD_ENDLESS_TIMESVALUE;											//无尽副本次数

	public int PLAYER_FIELD_CRUSADE_TIMESVALUE; 										//讨伐副本次数
	public int PLAYER_FIELD_SHOPSIZE_VALUE; 										//随身商店栏位数
	public int PLAYER_FIELD_SHOPREFRESH_VALUE; 										//附身商店刷新次数
    public int PLAYER_FIELD_EXPDEFIEND_VALUE;																	//经验御敌副本次数 (133)
	public int PLAYER_FIELD_GOLDDEFINED_VALUE;																	//元宝御敌副本次数(134)
    public int PLAYER_FIELD_COINDEFINED_VALUE;																	//铜币御敌副本次数(135)
	public int PLAYER_FIELD_SWEEP_VALUE;																	//剩余副本扫荡次数(136)

	public int PLAYER_FIELD_PICK_RANGE_VALUE;																	//角色自动拾取范围
	public int PLAYER_FIELD_WEAPON_STRENGTH_VALUE;															//武器强化等级
	public int PLAYER_FIELD_WEAPON_START_VALUE;																//武器星级等级
	public int PLAYER_FIELD_HAT_STRENGTH_VALUE;																//帽子强化等级
	public int PLAYER_FIELD_HAT_START_VALUE;																	//帽子星级等级
	public int PLAYER_FIELD_CLOTH_STRENGTH_VALUE;																//衣服强化等级
	public int PLAYER_FIELD_CLOTH_START_VALUE;																	//衣服星级等级
	public int PLAYER_FIELD_SHOES_STRENGTH_VALUE;																//鞋子强化等级
	public int PLAYER_FIELD_SHOES_START_VALUE;																	//鞋子星级等级
	public int PLAYER_FIELD_RING_STRENGTH_VALUE;																//戒指强化等级
	public int PLAYER_FIELD_RING_START_VALUE;																	//戒指星级等级
	public int PLAYER_FIELD_WEAPON_STORE_ID1_VALUE;															//武器嵌入宝石1ID
	public int PLAYER_FIELD_WEAPON_STORE_LEVEL1_VALUE;															//武器嵌入宝石1等级
	public int PLAYER_FIELD_WEAPON_STORE_EXP1_VALUE;															//武器嵌入宝石1经验
	public int PLAYER_FIELD_WEAPON_STORE_ID2_VALUE;															//武器嵌入宝石2ID
	public int PLAYER_FIELD_WEAPON_STORE_LEVEL2_VALUE;															//武器嵌入宝石2等级
	public int PLAYER_FIELD_WEAPON_STORE_EXP2_VALUE;															//武器嵌入宝石2经验
	public int PLAYER_FIELD_HAT_STORE_ID1_VALUE;																//帽子嵌入宝石1ID
	public int PLAYER_FIELD_HAT_STORE_LEVEL1_VALUE;															//帽子嵌入宝石1等级
	public int PLAYER_FIELD_HAT_STORE_EXP1_VALUE;																//帽子嵌入宝石1经验
	public int PLAYER_FIELD_HAT_STORE_ID2_VALUE;																//帽子嵌入宝石2ID
	public int PLAYER_FIELD_HAT_STORE_LEVEL2_VALUE;															//帽子嵌入宝石2等级
	public int PLAYER_FIELD_HAT_STORE_EXP2_VALUE;																//帽子嵌入宝石2经验
	public int PLAYER_FIELD_CLOTH_STORE_ID1_VALUE;																//衣服嵌入宝石1ID
	public int PLAYER_FIELD_CLOTH_STORE_LEVEL1_VALUE;															//衣服嵌入宝石1等级
	public int PLAYER_FIELD_CLOTH_STORE_EXP1_VALUE;															//衣服嵌入宝石1经验
	public int PLAYER_FIELD_CLOTH_STORE_ID2_VALUE;																//衣服嵌入宝石2ID
	public int PLAYER_FIELD_CLOTH_STORE_LEVEL2_VALUE;															//衣服嵌入宝石2等级
	public int PLAYER_FIELD_CLOTH_STORE_EXP2_VALUE;															//衣服嵌入宝石2经验
	public int PLAYER_FIELD_SHOES_STORE_ID1_VALUE;																//鞋子嵌入宝石1ID
	public int PLAYER_FIELD_SHOES_STORE_LEVEL1_VALUE;															//鞋子嵌入宝石1等级
	public int PLAYER_FIELD_SHOES_STORE_EXP1_VALUE;															//鞋子嵌入宝石1经验
	public int PLAYER_FIELD_SHOES_STORE_ID2_VALUE;																//鞋子嵌入宝石2ID
	public int PLAYER_FIELD_SHOES_STORE_LEVEL2_VALUE;															//鞋子嵌入宝石2等级
	public int PLAYER_FIELD_SHOES_STORE_EXP2_VALUE;															//鞋子嵌入宝石2经验
	public int PLAYER_FIELD_RING_STORE_ID1_VALUE;																//戒子嵌入宝石1ID
	public int PLAYER_FIELD_RING_STORE_LEVEL1_VALUE;															//戒子嵌入宝石1等级
	public int PLAYER_FIELD_RING_STORE_EXP1_VALUE;																//戒子嵌入宝石1经验
	public int PLAYER_FIELD_RING_STORE_ID2_VALUE;																//戒子嵌入宝石2ID
	public int PLAYER_FIELD_RING_STORE_LEVEL2_VALUE;															//戒子嵌入宝石2等级
	public int PLAYER_FIELD_RING_STORE_EXP2_VALUE;																//戒子嵌入宝石2经验
	public int PLAYER_USER_VIP_VALUE;																			//角色账号VIP等级
	public int PLAYER_FIELD_RANDOM_ECTYPEREWARD_VALUE;											//随机副本奖励值 0=不可领取,1=可领取,2=已领取




	public int PLAYER_FIELD_PVP_DRUCETIME_VALUE;																//复活事件减少值
	public int PALYER_FIELD_PVP_PROPCOMPRESS_VALUE;																//属性压缩比例，百分比
	public int PLAYER_FIELD_SCORE_VALUE;																		//PVP当前积分
	public int PLAYER_FIELD_MAX_SCORE_VALUE;																	//PVP的最大积分
	public int PLAYER_FIELD_CONTRIB_VALUE;																		//PVP当前贡献
	public int PLAYER_FIELD_WIN_VALUE;																			//PVP当天胜利次数
	public int PLAYER_FIELD_CURSEASONID_VALUE;																	//当前PVP赛季ID
	public int PLAYER_FIELD_PVPGROUP_VALUE;																		//当前赛季所在组ID
	public int PLAYER_FIELD_PVP_RANKINDEX_VALUE;																//当前赛季所在排名

	public int PLAYER_FIELD_SERVER_STORAGE_HP;											// 生命包
	public int PLAYER_FIELD_SERVER_STORAGE_MP;					    					// 法力包

    public int GetValue(int index)
    {
        var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_MainPlayer_PlayerValue>(this);

        int offset = index * 4;
        return BitConverter.ToInt32(bytes.Skip(offset).ToArray(), 0);
    }
    public SMsgPropCreateEntity_SC_MainPlayer_PlayerValue SetValue(int index, int value)
    {
        var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_MainPlayer_PlayerValue>(this);

        int offset = index * 4;
        var source = BitConverter.GetBytes(value);
        bytes[offset] = source[0];
        bytes[offset + 1] = source[1];
        bytes[offset + 2] = source[2];
        bytes[offset + 3] = source[3];

        return PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_MainPlayer_PlayerValue>(bytes);
    }

}
/// <summary>
/// 其他玩家基本属性
/// </summary>
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_OtherPlayer_PlayerValue
{
	
	public SMsgPropCreateEntity_SC_Player_CommonValue PlayerCommonValue;
	
	public byte State;  //当前状态

    public uint WeaponID;        //武器ID
    public uint AvatarID;        //时装ID
    
    
    public int GetValue(int index)
    {
        var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_OtherPlayer_PlayerValue>(this);

        int offset = index * 4;
        return BitConverter.ToInt32(bytes.Skip(offset).ToArray(), 0);
    }
    public SMsgPropCreateEntity_SC_OtherPlayer_PlayerValue SetValue(int index, int value)
    {
        var bytes = PackageHelper.StructToBytes<SMsgPropCreateEntity_SC_OtherPlayer_PlayerValue>(this);

        int offset = index * 4;
        var source = BitConverter.GetBytes(value);
        bytes[offset] = source[0];
        bytes[offset + 1] = source[1];
        bytes[offset + 2] = source[2];
        bytes[offset + 3] = source[3];

        return PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_OtherPlayer_PlayerValue>(bytes);
    }
} 

//其他玩家实体结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_SC_OtherPlayer : INotifyArgs, IEntityDataStruct, IPlayerDataStruct
{
    private SMsgPropCreateEntity_SC_Header m_sMsg_Header;

    // ......................   // 创建现场
    public int ActorID;         //角色ID
    public Int64 UID;           //GUID全局唯一
    public int MapID;           //地图ID
    public int m_playerX;         //角色X坐标
    public int m_playerY;         //角色Y坐标
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[] m_name;         //角色名称
	public SMsgPropCreateEntity_SC_BaseValue BaseObjectValues;
	public SMsgPropCreateEntity_SC_Player_UnitValue UnitValues;
	public SMsgPropCreateEntity_SC_OtherPlayer_PlayerValue PlayerValues;

    public string Name
    {
        set
        {
            m_name = new byte[128];
            Encoding.UTF8.GetBytes(value).CopyTo(m_name, 0);
		}
        get
        {
            return m_name != null ? Encoding.UTF8.GetString(m_name) : string.Empty;
        }
    }
    /// <summary>
    /// 从整体字节数组获得实体上下文的具体数据
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <returns></returns>
    public static SMsgPropCreateEntity_SC_OtherPlayer ParsePackage(byte[] dataBuffer, int offset)
    {

        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        return ParsePackage(package, offset);
    }
    public static SMsgPropCreateEntity_SC_OtherPlayer ParsePackage(Package package, int offset)
    {
        var structLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_OtherPlayer));
        var headLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
        var offsetBuffer = package.Data.Skip(offset).Take(structLength).ToArray();

        SMsgPropCreateEntity_SC_OtherPlayer sMsgPropCreateEntity_SC_OtherPlayer = new SMsgPropCreateEntity_SC_OtherPlayer();

        sMsgPropCreateEntity_SC_OtherPlayer.m_sMsg_Header = SMsgPropCreateEntity_SC_Header.ParsePackage(package, offset);

        int of = headLength;
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_OtherPlayer.ActorID);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_OtherPlayer.UID);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_OtherPlayer.MapID);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_OtherPlayer.m_playerX);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_OtherPlayer.m_playerY);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out sMsgPropCreateEntity_SC_OtherPlayer.m_name, 19);
        byte[] baseValues, playerUnitValues, playerValues;

		int baseValueSize=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_BaseValue>();
		int unitValueSize=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_Player_UnitValue>();
		int playValueSize=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_OtherPlayer_PlayerValue>();
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out baseValues, baseValueSize);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out playerUnitValues, unitValueSize);
		of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out playerValues, playValueSize);
        sMsgPropCreateEntity_SC_OtherPlayer.BaseObjectValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_BaseValue>(baseValues);
        sMsgPropCreateEntity_SC_OtherPlayer.UnitValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_Player_UnitValue>(playerUnitValues);
        sMsgPropCreateEntity_SC_OtherPlayer.PlayerValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_OtherPlayer_PlayerValue>(playerValues);

        //TraceUtil.Log("其他玩家收到:" + sMsgPropCreateEntity_SC_OtherPlayer.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURMP + "  " + sMsgPropCreateEntity_SC_OtherPlayer.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXMP);

	    return sMsgPropCreateEntity_SC_OtherPlayer;
    }
    public void UpdateValue(short index, int value)
    {
        #region UpdateValueSwitch
		int baseValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_BaseValue>()/4;
		int unitValueIndex=CommonTools.GetStructSize<SMsgPropCreateEntity_SC_Player_UnitValue>()/4;
		if (index < baseValueIndex)   //SMsgPropCreateEntity_SC_BaseValue
        {
            this.BaseObjectValues = this.BaseObjectValues.SetValue(index, value);
        }
		else if (index <baseValueIndex+unitValueIndex)   //SMsgPropCreateEntity_SC_Player_UnitValue
        {
			this.UnitValues = this.UnitValues.SetValue(index - baseValueIndex, value);
        }
        else 		   //SMsgPropCreateEntity_SC_MainPlayer_PlayerValue
        {
			this.PlayerValues = this.PlayerValues.SetValue(index - baseValueIndex-unitValueIndex, value);
        }
        #endregion
    }

    #region INotify
    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
    #endregion
    public SMsgPropCreateEntity_SC_Header SMsg_Header
    {
        get
        {
            return this.m_sMsg_Header;
        }
    }
    public byte GetPlayerKind(out string career)
    {
        byte kind = GetPlayerKind();
        switch (kind)
        {
            case 1:
                career = "Char01";
                break;
            case 2:
                career = "Char04";
                break;
            case 3:
                career = "Char02";
                break;
            case 4:
                career = "Char03";
                break;
            default:
                career = "Char01";
                break;
        }

        return kind;
    }
    public byte GetPlayerKind()
    {
        return (byte)this.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
    }

    public SMsgPropCreateEntity_SC_BaseValue GetBaseValue()
    {
        return this.BaseObjectValues;
    }

    public SMsgPropCreateEntity_SC_Player_UnitValue GetUnitValue()
    {
        return this.UnitValues;
    }

    public SMsgPropCreateEntity_SC_Player_CommonValue GetCommonValue()
    {
        return this.PlayerValues.PlayerCommonValue;
    }
    public int PlayerX
    {
        get
        {
            return this.m_playerX;
        }
    }

    public int PlayerY
    {
        get
        {
            return this.m_playerY;
        }
    }
    public int PlayerActorID
    {
        get
        {
            return this.ActorID;
        }
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgPropCreateEntity_PlayerEquip
{
    public uint EquipID;        //装备ID
    //public byte EquipLevel;     //装备等级
    //public byte EquipGlory;     //装备光环
}
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgFightFightTo_CS
{
    public long uidFighter;   //战斗指令提交者。
    public int nFightCode;      //技能ID
    public int xMouse;          //范围攻击 中心X
    public int yMouse;          //范围攻击 中心Y
    public int xDir;            //攻击方向X
    public int yDir;            //攻击方向Y
}


