using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

//INTERACT_MSG_COMMON		0
//NPC对话 消息结构体
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteractCOMMON_SC
{
    public byte byDialogType;	//对话文字类型 1：NPC闲话 2:任务    
    public uint dwNPCID;		//对话在NPCID   
    public uint dwParam;		//对话文字参数 与byDialogType对应 查找相应在配置文档 显示字符串
    public byte byBtnNum;		//NPC对话面板上显示在功能按钮个数

    public static SMsgInteractCOMMON_SC ParseResultPackage(byte[] dataBuffer)
    {
        SMsgInteractCOMMON_SC sMsgInteractCOMMON_SC;

       // Package package = PackageHelper.ParseReceiveData(dataBuffer);
        var msgInteractCommonLength = Marshal.SizeOf(typeof(SMsgInteractCOMMON_SC));

        sMsgInteractCOMMON_SC = PackageHelper.BytesToStuct<SMsgInteractCOMMON_SC>(dataBuffer.Take(msgInteractCommonLength).ToArray());

        return sMsgInteractCOMMON_SC;
    }

    public int GetEventArgsType()
    {
        return 0;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteractCOMMONBtn_SC
{
    public byte byBtnType;		//NPC对话面板上显示在功能按钮类型 1：特殊功能 2：任务 3：GM
    public uint dwParam1;		//按钮参数1 与byBtnType对应 做相应处理 客户端点击操作之后需要提交服务器,按钮类型，是什么商店
    public uint dwParam2;       //按钮参数2 与byBtnType对应 做相应处理 客户端点击操作之后需要提交服务器，按钮

    public static SMsgInteractCOMMONBtn_SC ParseResultPackage(byte[] dataBuffer, int offset, int length)
    {
        SMsgInteractCOMMONBtn_SC sMsgInteractCOMMONBtn_SC;

        sMsgInteractCOMMONBtn_SC = PackageHelper.BytesToStuct<SMsgInteractCOMMONBtn_SC>(dataBuffer.Skip(offset).Take(length).ToArray());
        TraceUtil.Log("npcButton:ID1:" + sMsgInteractCOMMONBtn_SC.dwParam1 + "," + sMsgInteractCOMMONBtn_SC.dwParam2);
        return sMsgInteractCOMMONBtn_SC;
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteractCOMMONPackage:INotifyArgs
{
    public SMsgInteractCOMMON_SC sMsgInteractCOMMON_SC;
    public SMsgInteractCOMMONBtn_SC[] sMsgInteractCOMMONBtn_SC;

    public int GetEventArgsType()
    {
        throw new System.NotImplementedException();
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteractCOMMON_CS
{
    public uint dwNPCID;		//NPCID(无NPC NPCID==0时 不判定NPC)
    public byte byOperateType;	//操作类型 1：特殊功能 2：任务 3：GM 与传过去的byBtnType一样
    public uint dwParam1;		//参数1 与byOperateType对应 (1:   2:,dwParam1 = 0 空，=1 接收任务，=2 完成任务 目前只有完成任务) 
    public uint dwParam2;      //参数2 与byOperateType对应 (与dwParam1对应的参数数值)
    public byte byIsContext;    //是否有其他的附加数据 0:否 1:是 
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteractCOMMONContext_CS
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] szContext;	//其他的附加数据 可以和上文的byDialogType对应 ，示例：GM类型的附加数据为 客户端按照规格填写的字符串
};


// 放弃任务
//INTERACT_MSG_GIVEUPTASK			= 5,
public struct SMsgInteractGiveUpTask_CSC
{
    public int dwTaskID;				//放弃的任务ID
    public static SMsgInteractGiveUpTask_CSC ParseResultPackage(byte[] dataBuffer)
    {
        SMsgInteractGiveUpTask_CSC sMsgInteractGiveUpTask_CSC = new SMsgInteractGiveUpTask_CSC();
        PackageHelper.ReadData(dataBuffer, out sMsgInteractGiveUpTask_CSC.dwTaskID);

        return sMsgInteractGiveUpTask_CSC;
    }
};

//--------------------------------------------------------------------------------------------------------------------------
// 完成任务
//INTERACT_MSG_FINISHTASK			= 4,
public struct SMsgInteractFinishTask_SC
{
    public int dwTaskID;				//完成的任务ID
    public static SMsgInteractFinishTask_SC ParseResultPackage(byte[] dataBuffer)
    {
        SMsgInteractFinishTask_SC sMsgInteractFinishTask_SC = new SMsgInteractFinishTask_SC();
        PackageHelper.ReadData(dataBuffer, out sMsgInteractFinishTask_SC.dwTaskID);

        return sMsgInteractFinishTask_SC;
    }
}

//--------------------------------------------------------------------------------------------------------------------------
//任务结构体
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct STaskState
{
    public int dwTaskID;					//任务ID
    public byte byRate;						//当前进度
    public byte byAllRate;					//总的进度
    public static STaskState ParseResultPackage(byte[] dataBuffer)
    {
        STaskState sTaskState;

        sTaskState = PackageHelper.BytesToStuct<STaskState>(dataBuffer);
        return sTaskState;
    }
}


//--------------------------------------------------------------------------------------------------------------------------
// 初始化任务
//INTERACT_MSG_INITTASK			= 2,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteractInitTask_SC			//前端没有语音提示
{
    public byte byNum;					//下发的任务Num
    public STaskState[] STaskStates;

    public static SMsgInteractInitTask_SC ParseResultPackage(byte[] dataBuffer)
    {
        SMsgInteractInitTask_SC sMsgInteractInitTask_SC = new SMsgInteractInitTask_SC();
        int offset = 0;
        byte byNum;
        offset += PackageHelper.ReadData(dataBuffer.ToArray(), out byNum);
        sMsgInteractInitTask_SC.STaskStates = new STaskState[byNum];
        int length = Marshal.SizeOf(typeof(STaskState));
        for (int i = 0; i < byNum; i++)
        {
            sMsgInteractInitTask_SC.STaskStates[i] = STaskState.ParseResultPackage(dataBuffer.Skip(offset).Take(length).ToArray());
            offset += length;
        }

        return sMsgInteractInitTask_SC;
    }
}
//副本内引导步骤  SMsgInteractGuideStep_SC
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SC_GuideStepInfo
{
    public int dwStepID;				// 步骤ID
    public int dwStepType;			    // 步骤类型
    public byte byStepStute;			// 步骤状态 nStatus = 1 正执行状态 nStatus = 2 已完成状态

    public static SC_GuideStepInfo ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SC_GuideStepInfo>(dataBuffer);
    }
};

//新手引导结构客户端至服务器至客户端
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteract_NewbieGuide_SCS:INotifyArgs
{
    public uint dwActorId;
    public ushort wGuideIndex;

    public int GetEventArgsType()
    {
        return 0;
    }
};

#region rank
public enum RANK_TYPEDWROD
{
    RANK_NONE_TYPE,
    RANK_ACTORFIGNT_TYPE,                   //角色战斗力排行榜
    RANK_YAONVFIGHT_TYPE,                   //妖女排行榜
    RANK_EQUIPFIGHT_TYPE,                   //武器排行榜
    RANK_LEVEL_TYPE,
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct PlayerRankingData
{
    public uint QueryTime;        //查询时间点
    public uint RankingIndex;     //排行名次

    
};

//下发角色战斗力排行单元
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct RankingActorFightData
{
    public byte    wRankingIndex;      //排名
    public uint   dwActorID;          //角色ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[]    szName;         //用户名
    public byte    byKind;             //职业
    public byte    byLevel;            //玩家等级
    public byte    byVipLevel;         //VIP等级
    public uint   dwTitleID;          //角色称号ID
    public uint   dwFashionID;          //时装ID
    public uint   dwFighter;          //玩家战斗力

};

//下发妖女排行单元
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct RankingYaoNvFightData
{
    public byte                        wRankingIndex;                          //排名
    public uint                       dwActorID;                              //角色ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[]                        szName;                             //用户名
    public byte                        byKind;                                 //职业
    public byte                        byLevel;                                //玩家等级
    public byte                        byVipLevel;                             //VIP等级
    public uint                       dwTitleID;                              //角色称号ID
    public uint   dwFashionID;                                                 //时装ID
    public uint                       dwYaoNvFight;                           //妖女战斗力
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public SYaoNvRankingData[]           YaoNvData;                           //妖女信息

};

//武器排行榜单元
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct RankingEquipFightData
{       
    public byte                        wRankingIndex;                          //排名
    public uint                       dwActorID;                              //角色ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[]                        szName;                             //用户名
    public byte                        byKind;                                 //职业
    public byte                        byLevel;                                //玩家等级
    public byte                        byVipLevel;                             //VIP等级
    public uint                       dwTitleID;                              //角色称号ID
    public uint                       dwFashionID;                            //时装ID
    public uint                       dwEquipFight;                           //装备战斗力
    public SEquipRankingData           EquipData;                              //装备信息
    

};

//REQUEST_RANKING_UPDATE
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEquipRankingData        //排行榜武器详细信息
{
    public uint    dwGoodsID;                               //武器ID
    public byte    byStrengLevel;
    public byte    byStartLevel;
    
    public byte    dwGemID1;
    public byte    byGemLevel1;
    public byte    dwGemID2;
    public byte    byGemLevel2;
    public byte    dwGemID3;
    public byte    byGemLevel3;
    public byte    dwGemID4;
    public byte    byGemLevel4;            
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SEquipInfo       //角色装备概要信息
{
    public uint   dwGoodsID;
    public byte    byStrengLevel;
    public byte    byStartLevel;
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SYaoNvRankingData
{
    public byte    byYaoNvId;
    public byte    byYaoNvLevel;
};

//--------------------------------------------------------------------------------------------------------------------------
//排行榜系统消息
//INTERACT_MSG_RANKINGLIST_DATA = 16,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteract_RankingList_CS
{
    public byte byRankingType;         //排行榜类型(1-战斗力排行榜，2-妖女排行榜，3-已装备武器战斗力排行榜)
    public byte byIndex;               //请求页面
    public Package GeneratePackage()
    {
        Package Pak = new Package();
        Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_INTERACT,InteractDefineManager.INTERACT_MSG_RANKINGLIST_DATA);
        Pak.Data = PackageHelper.StructToBytes<SMsgInteract_RankingList_CS>(this);
        return Pak; 
    }

};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteract_RankingList_SC
{
    public byte byRankingType;         //排行榜类型(1-战斗力排行榜，2-妖女排行榜，3-已装备武器战斗力排行榜)
    public byte byRankingNum;          //排行数据个数
    public byte byTotalIndex;//总页数
    public byte byIndex;               //请求页面
    public byte byActorRanking;        //当前玩家排名
    public uint UpdateRankInterval;   //排行榜更新时间
    //....排行榜数据
    public RankingActorFightData[] rankingActorFightData;
    public RankingYaoNvFightData[] rankingYaoNvFightData;
    public RankingEquipFightData[] RankingEquipFightData;
    public static SMsgInteract_RankingList_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgInteract_RankingList_SC sMsgInteract_RankingList_SC = new SMsgInteract_RankingList_SC();
        int offset = 0;
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMsgInteract_RankingList_SC.byRankingType);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMsgInteract_RankingList_SC.byRankingNum);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMsgInteract_RankingList_SC.byTotalIndex);
        offset += PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMsgInteract_RankingList_SC.byIndex);
        offset +=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMsgInteract_RankingList_SC.byActorRanking);
        offset +=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(), out sMsgInteract_RankingList_SC.UpdateRankInterval);
        switch(sMsgInteract_RankingList_SC.byRankingType)
        {
            case 1:
             
                sMsgInteract_RankingList_SC.rankingActorFightData=new RankingActorFightData[sMsgInteract_RankingList_SC.byRankingNum];
                for(int i=0;i<sMsgInteract_RankingList_SC.byRankingNum;i++)
                {
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingActorFightData[i].wRankingIndex);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingActorFightData[i].dwActorID);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingActorFightData[i].szName,19);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingActorFightData[i].byKind);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingActorFightData[i].byLevel);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingActorFightData[i].byVipLevel);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingActorFightData[i].dwTitleID);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingActorFightData[i].dwFashionID);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingActorFightData[i].dwFighter);
                }
                break;
            case 2:
                sMsgInteract_RankingList_SC.rankingYaoNvFightData=new RankingYaoNvFightData[sMsgInteract_RankingList_SC.byRankingNum];
                for(int i=0;i<sMsgInteract_RankingList_SC.byRankingNum;i++)
                {
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].wRankingIndex);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].dwActorID);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].szName,19);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].byKind);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].byLevel);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].byVipLevel);

                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].dwTitleID);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].dwFashionID);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].dwYaoNvFight);
                    sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].YaoNvData=new SYaoNvRankingData[5];
                    for(int j=0;j<5;j++)
                    {
                        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].YaoNvData[j].byYaoNvId);
                        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.rankingYaoNvFightData[i].YaoNvData[j].byYaoNvLevel);
                    }
                }
                break;
            case 3:
                sMsgInteract_RankingList_SC.RankingEquipFightData=new RankingEquipFightData[sMsgInteract_RankingList_SC.byRankingNum];
                for(int i=0;i<sMsgInteract_RankingList_SC.byRankingNum;i++)
                {
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].wRankingIndex);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].dwActorID);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].szName,19);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].byKind);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].byLevel);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].byVipLevel);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].dwTitleID);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].dwFashionID);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].dwEquipFight);
                    sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData=new SEquipRankingData();

                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.dwGoodsID);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.byStrengLevel);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.byStartLevel);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.dwGemID1);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.byGemLevel1);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.dwGemID2);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.byGemLevel2);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.dwGemID3);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.byGemLevel3);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.dwGemID4);
                    offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_RankingList_SC.RankingEquipFightData[i].EquipData.byGemLevel4);
                }
                break;

        }
       
        return sMsgInteract_RankingList_SC;
        
    }

};

//--------------------------------------------------------------------------------------------------------------------------
//排行榜上某个角色消息
//INTERACT_MSG_GETPLAYERRANKING  = 17,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteract_GetPlayerRanking_CS
{
    public byte    byRankingType;          //排行榜类型(1-战斗力排行榜，2-妖女排行榜，3-已装备武器战斗力排行榜)
    public uint   dwActorID;          //查询者(自己)角色ID
    public uint   dwRankActorID;      //被查询的角色ID

    public Package GeneratePackage()
    {
        Package Pak = new Package();
        Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_INTERACT,InteractDefineManager.INTERACT_MSG_GETPLAYERRANKING);
        Pak.Data = PackageHelper.StructToBytes<SMsgInteract_GetPlayerRanking_CS>(this);
        return Pak; 
    }
};


[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgInteract_GetPlayerRanking_SC
{
    public uint               dwActorID;                          //角色ID
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[]                szActorName;                    //姓名
    public byte                byKind;                             //职业
    public byte                byVipLevel;                         //Vip等级
    public byte                byActorLevel;                       //角色等级
    public uint               dwTitleID;                          //称号ID
    public uint               dwFashionID;          //时装ID
    public uint               dwActorFinght;                      //角色战斗力
    public uint               dwEquipFight;                       //武器战斗力
    public uint               dwYaoNvFight;                       //妖女战斗力
    public uint               dwCurExp;                           //角色当前经验值
    public uint               dwMaxExp;                           //角色升级所需经验值
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public SYaoNvRankingData[]   YaoNvData;                       //妖女信息，目前5个
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public SEquipInfo[]          dwGoods;                         //角色已装备装备
    public uint               dwAttack;                           //攻击
    public uint               dwdefend;                           //防御
    public uint               dwMaxHp;                            //最大血量
    public uint               dwMaxMp;                            //最大蓝量
    public uint               dwBurst;                            //暴击值
    public uint               dwUnBurst;                          //抗暴击值
    public uint               dwNicety;                           //命中值
    public uint               dwJook;                             //闪避值
    
    public static SMsgInteract_GetPlayerRanking_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgInteract_GetPlayerRanking_SC sMsgInteract_GetPlayerRanking_SC=new SMsgInteract_GetPlayerRanking_SC();
        int offset = 0;
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwActorID);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.szActorName,19);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.byKind);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.byVipLevel);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.byActorLevel);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwTitleID);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwFashionID);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwActorFinght);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwEquipFight);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwYaoNvFight);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwCurExp);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwMaxExp);
        sMsgInteract_GetPlayerRanking_SC.YaoNvData=new SYaoNvRankingData[5];
        for(int j=0;j<5;j++)
        {
            offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.YaoNvData[j].byYaoNvId);
            offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.YaoNvData[j].byYaoNvLevel);
        }
        sMsgInteract_GetPlayerRanking_SC.dwGoods=new SEquipInfo[6];
        for(int i=0;i<5;i++)
        {
            offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwGoods[i].dwGoodsID);
            offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwGoods[i].byStrengLevel);
            offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwGoods[i].byStartLevel);
        }
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwAttack);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwdefend);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwMaxHp);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwMaxMp);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwBurst);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwUnBurst);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwNicety);
        offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_GetPlayerRanking_SC.dwJook);

        return sMsgInteract_GetPlayerRanking_SC;
    }
};

//-------------------------------------------------------------------------------------------------------------------------
//请求pvp排行榜
//INTERACT_MSG_PVP_RANKING		= 20
public struct SMsgInteract_PvpRanking_CS
{
  public	byte	byIndex;		//排行榜页数,如果是默认，填写0
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_INTERACT,InteractDefineManager.INTERACT_MSG_PVP_MYDATA);
		Pak.Data = PackageHelper.StructToBytes<SMsgInteract_PvpRanking_CS>(this);
		return Pak; 
	}
};
//--------------------------------------------------------------------------------------------------------------------------
//获取PVP排行榜信息
public struct SMsgInteract_PvpRanking_SC
{
	public	byte	byIndex;					//当前数据是排行榜第几页
	public	byte	byRankingNum;				//当前页有多少条数据
	public	byte	byTotalIndex;				//排行榜总页数
	public	uint	dwFreshInterval;			//排行榜刷新时间
	public  SPVPRankingData[]  RankingDataList;//

	public static SMsgInteract_PvpRanking_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgInteract_PvpRanking_SC sMsgInteract_PvpRanking_SC=new SMsgInteract_PvpRanking_SC();
		int offset = 0;
		offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.byIndex);
		offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.byRankingNum);
		offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.byTotalIndex);
		offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.dwFreshInterval);

		sMsgInteract_PvpRanking_SC.RankingDataList=new SPVPRankingData[sMsgInteract_PvpRanking_SC.byRankingNum];
		for(int j=0;j<sMsgInteract_PvpRanking_SC.byRankingNum;j++)
		{
			//SPVPRankingData sPVPRankingData=new SPVPRankingData();
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].byIndex);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].byWinRate);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].byVipLevel);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].byLevel);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].byKind);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].byGroupID);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].dwHonorNum);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].dwContrib);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].dwFashionID);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].dwActorID);
			offset+=PackageHelper.ReadData(dataBuffer.Skip(offset).ToArray(),out sMsgInteract_PvpRanking_SC.RankingDataList[j].szActorName,19);
		}

		
		return sMsgInteract_PvpRanking_SC;
	}
};

public struct SPVPRankingData
{
	public	byte	byIndex;			//角色排名（1-50名）
	public	byte	byWinRate;			//角色胜率
	public	byte	byVipLevel;			//角色VIP等级
	public	byte	byLevel;			//角色等级
	public	byte	byKind;				//角色职业
	public	byte	byGroupID;			//角色所属组
	public	uint	dwHonorNum;			//角色荣誉值
	public	uint	dwContrib;			//角色贡献值
	public	uint	dwFashionID;		//角色时装ID/头像
	public  uint    dwActorID;          //角色id
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	public byte[]   szActorName;	//角色名


};


#endregion







//INTERACT_MSG_TASKLOG	1
// 任务日志更新信息结构
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct STaskLogUpdate
{
    public int nTaskID;	// 任务ID
    public int nTaskType;	// 更新类型 //根据任务类型 对应不同现场 1 = 通关指定关卡 2 = 强化任意一件装备 3 = 购买任意一件道具 
    // 4 = 升级任意一个技能 5 = 进行一次PVP 6 = 通关一次任意封魔副本 7 = 装备任意一个技能
    public int nStatus;	// 任务当前状态 nStatus = 1 可执行状态 nStatus = 2 已完成状态

    public static STaskLogUpdate ParseResultPackage(byte[] dataBuffer, out int offset)
    {
        offset = Marshal.SizeOf(typeof(STaskLogUpdate));
        return PackageHelper.BytesToStuct<STaskLogUpdate>(dataBuffer.Take(offset).ToArray());
    }
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct STaskLogContext
{
    public int nParam1;    // 参数索引
    public int nParam2;	// 参数ID(一般是物品、怪物ID)
    public int nParam3;	// 数值(一般表示数值)

    public static STaskLogContext ParseResultPackage(byte[] dataBuffer, int offset)
    {
        var length = Marshal.SizeOf(typeof(STaskLogContext));
        return PackageHelper.BytesToStuct<STaskLogContext>(dataBuffer.Skip(offset).Take(length).ToArray());
    }
};
