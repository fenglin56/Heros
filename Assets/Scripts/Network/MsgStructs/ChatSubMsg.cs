using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Chat
{
    public enum ChatDefine
    {
        MSG_CHAT_BEGIN = -1,
        MSG_CHAT_CURRENT = 0,		//当前
        MSG_CHAT_WORLD = 1,		//世界
        MSG_CHAT_GROUP = 2,		//阵营
        MSG_CHAT_TIP = 12,		//悬浮提示
        MSG_CHAT_SPACE = 32,		//附近频道
        MSG_CHAT_TRUMPET = 18,	//广播频道
        MSG_CHAT_TEAM = 3,		//组队
        MSG_CHAT_FACTION = 4,		//帮派
        MSG_CHAT_FAME = 5,		//传闻
        MSG_CHAT_RUMOR = 6,		//谣言
        MSG_CHAT_COUPLE = 7,		//夫妻
        MSG_CHAT_PRIVATE = 8,		//私聊
        MSG_CHAT_TEMP = 9,		//零时(本地系统提示)
        MSG_CHAT_SYSTEM = 64,		//系统提示
        MSG_CHAT_ROLLTIP = 11,	//滚动提示
        MSG_CHAT_GETITEMTIP = 13,	//获得物品的时候的提示，带图标的
        MSG_CHAT_MONSTERBUBBLE = 14, //怪物头顶泡泡
        MSG_CHAT_FIRE = 15,		//战斗
        MSG_CHAT_NUMBER = 16,		//播放数字频道
        MSG_CHAT_ALL = 17,		//所有频道
    }

	public enum CHAT_MSG_SYSTYPE
	{
		NONE_SYS_CHAT_TYPE = 0,			//默认系统提示类型
		WARING_SYS_CHAT_TYPE,		//警告类型系统提示
		BROADCAST_SYS_CHAT_TYPE,	//广播系统提示类型
	}
}

//发送聊天信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgChat_CS
{
	public uint	dwActorID;          //发送者actorID
    public byte bChatType;//谈话类型 0 普通 1 邀请入队;         //
	public uint	buffLen;			//超链接buffer
    
    public uint dwAccepterActorID;  //接收者actorID
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	public byte[] accepterName;     //接收者名字

    public byte[] chat; //内容

    public static Package GeneratePackage(uint actorID, uint accepterActorID,string accepterName, string chat, byte chatType, Chat.ChatDefine define)
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_CHAT, (short)define);
        SMsgChat_CS sMsgChat = new SMsgChat_CS();
        sMsgChat.dwActorID = actorID;
        sMsgChat.bChatType = chatType;
        sMsgChat.accepterName = new byte[19];
		System.Text.Encoding.UTF8.GetBytes(accepterName).CopyTo(sMsgChat.accepterName, 0);
        sMsgChat.dwAccepterActorID = accepterActorID;
        sMsgChat.chat = System.Text.Encoding.UTF8.GetBytes(chat);
        sMsgChat.buffLen = (uint)(sMsgChat.chat.Length);
        //组包
        List<byte> data = new List<byte>();
        data.AddRange(BitConverter.GetBytes(sMsgChat.dwActorID));
        data.Add(sMsgChat.bChatType);
        data.AddRange(BitConverter.GetBytes(sMsgChat.buffLen));
        data.AddRange(BitConverter.GetBytes(sMsgChat.dwAccepterActorID));
		data.AddRange(sMsgChat.accepterName);
        data.AddRange(sMsgChat.chat);
        //打包
        pkg.Data = data.ToArray();        
        
        return pkg;
    }
	
};

//接收聊天信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgChat_SC
{
    public int senderActorID;
	public byte	bySenderVipLevel;
    public int accepterActorID;
    public Int64 lGuid; //UID
    public byte bChatType;//谈话类型 0 普通 1 邀请入队 2物品获取tips
    public int buffLen;	//聊天长度
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    private byte[] senderName;
    public string SenderName;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    private byte[] accepterName;
    public string AccepterName;
    public byte senderKind;
    public byte senderSex;
    public byte accepterKind;
    public byte accepterSex;

	public int extenMessageLength;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
    public byte[] extenMessage;
    public int friendTeamID;

    private byte[] chat; //内容
    public string Chat;

	public int tipsLength;
	public byte[] tipsMessage;

	public string L_LabelChat;//颜色格式
	public int L_Channel;//频道
	public int L_ChaterID;//私聊id

    public static SMsgChat_SC ParsePackage(byte[] dataBuffer)
    {
        SMsgChat_SC sMsgChat_SC = new SMsgChat_SC();
        int of = 0;
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.senderActorID);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.bySenderVipLevel);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.accepterActorID);        
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.lGuid);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.bChatType);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.buffLen);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.senderName, 19);
        sMsgChat_SC.SenderName = System.Text.Encoding.UTF8.GetString(sMsgChat_SC.senderName);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.accepterName, 19);
        sMsgChat_SC.AccepterName = System.Text.Encoding.UTF8.GetString(sMsgChat_SC.accepterName);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.senderKind);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.senderSex);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.accepterKind);
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.accepterSex);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.extenMessage, 128);
		if(sMsgChat_SC.bChatType == 1)
		{
			sMsgChat_SC.friendTeamID = BitConverter.ToInt32(sMsgChat_SC.extenMessage, 0);
		}
        of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.chat, sMsgChat_SC.buffLen);
        sMsgChat_SC.Chat = System.Text.Encoding.UTF8.GetString(sMsgChat_SC.chat);
		if(sMsgChat_SC.bChatType == 2)
		{
			sMsgChat_SC.tipsLength = BitConverter.ToInt32(sMsgChat_SC.extenMessage, 0);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgChat_SC.tipsMessage, sMsgChat_SC.tipsLength);
		}
        return sMsgChat_SC;
    }	
}

//
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgTipChat_SC
{
	public enum CHAT_TIPS_LEVEL
	{
		NONE_TIPS_LEVEL,
		CHAT_TIPS_LEVEL1,
		CHAT_TIPS_LEVEL2,				//提示2
		CHAT_TIPS_LEVEL3,				//提示3，礼包提示
	};

	public byte	byTipLevel;			//提示级别，控制前端显示，目前，礼包才有提示
	public byte	byNumber;
	public ShowGoodsInfo[] ShowGoodsInfos; 

	public static SMsgTipChat_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgTipChat_SC sMsgTipChat_SC = new SMsgTipChat_SC();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTipChat_SC.byTipLevel);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTipChat_SC.byNumber);
		sMsgTipChat_SC.ShowGoodsInfos = new ShowGoodsInfo[sMsgTipChat_SC.byNumber];
		for(int i = 0;i<sMsgTipChat_SC.byNumber;i++)
		{
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTipChat_SC.ShowGoodsInfos[i].dwGoodsID);
			of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgTipChat_SC.ShowGoodsInfos[i].dwGoodsNum);
		}
		return sMsgTipChat_SC;
	}
};
//物品现场
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct ShowGoodsInfo
{
	public int	dwGoodsID;
	public int	dwGoodsNum;
};

//系统信息
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgSysChat_SC
{
	public byte	SysMessageType;
	public int	StartTime;
	public int	EndTime;
	public int	RollTimes;
	public short ContextLen;		//系统聊天内容长度
	public byte[] chat; //内容
	public string Chat;

	public static SMsgSysChat_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgSysChat_SC sMsgSysChat_SC = new SMsgSysChat_SC();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSysChat_SC.SysMessageType);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSysChat_SC.StartTime);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSysChat_SC.EndTime);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSysChat_SC.RollTimes);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSysChat_SC.ContextLen);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSysChat_SC.chat, sMsgSysChat_SC.ContextLen);
		sMsgSysChat_SC.Chat = System.Text.Encoding.UTF8.GetString(sMsgSysChat_SC.chat);
		return sMsgSysChat_SC;
	}
}; 


//禁言
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgChatCodeForbid_SC
{
	public byte bNum;
	public byte bType; // 0= 数字 1=字符串
	public byte bLength; //长度
	public uint dwTime;

    public static SMsgChatCodeForbid_SC ParsePackage(byte[] dataBuffer)
    {
        return PackageHelper.BytesToStuct<SMsgChatCodeForbid_SC>(dataBuffer);
    }
}

//错误码现场
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgError_SC
{
	public byte bType; // 0= 数字 1=字符串
	public int dwLength; //长度
	//public byte[]
}

////广播
//[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
//struct SMsgBroadCastSysTips_SC
//{
//	public int dwSysTipsID;				//系统提示ID
//	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
//	private byte[] szActorName;			//角色名
//	public string SZActorName;
//
//	public SMsgBroadCastSysTips_SC ParsePackage(byte[] dataBuffer)
//	{
//		SMsgBroadCastSysTips_SC sMsgBroadCastSysTips_SC = new SMsgBroadCastSysTips_SC();
//		int of = 0;
//		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgBroadCastSysTips_SC.dwSysTipsID);
//		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgBroadCastSysTips_SC.szActorName,19);
//		sMsgBroadCastSysTips_SC.SZActorName = System.Text.Encoding.UTF8.GetString(sMsgBroadCastSysTips_SC.szActorName);
//	}
//};

//广播
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
struct SMsgBroadCastSysTips_SC
{
	public byte	SysMessageType;
	public int	StartTime;
	public int	EndTime;
	public int	RollTimes;
	public short ContextLen;
	public int dwSysTipsID;				//系统提示ID
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
	private byte[] szActorName;			//角色名
	public string SZActorName;
	
	public static SMsgBroadCastSysTips_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgBroadCastSysTips_SC sMsgBroadCastSysTips_SC = new SMsgBroadCastSysTips_SC();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgBroadCastSysTips_SC.SysMessageType);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgBroadCastSysTips_SC.StartTime);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgBroadCastSysTips_SC.EndTime);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgBroadCastSysTips_SC.RollTimes);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgBroadCastSysTips_SC.ContextLen);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgBroadCastSysTips_SC.dwSysTipsID);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgBroadCastSysTips_SC.szActorName,19);
		sMsgBroadCastSysTips_SC.SZActorName = System.Text.Encoding.UTF8.GetString(sMsgBroadCastSysTips_SC.szActorName);
		return sMsgBroadCastSysTips_SC;
	}
};

//自动语音
public class PhpAutoPropaganda
{
	public int time;
	public string[] name;
	public string[] content;
}
public class PhpPropaganda
{
	public string PropagandaName;
	public string PropagandaContent;
}



