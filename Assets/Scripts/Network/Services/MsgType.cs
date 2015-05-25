using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum MasterMsgType:byte
{
    NET_ROOT_ERROR = 0x0001,		// 错误报警专用消息码
    NET_ROOT_LOGIN = 0x0002,		// 登录态消息码
    NET_ROOT_SELECTACTOR = 0x0003,		// 创建(选择)人物态消息码
    NET_ROOT_MANAGER = 0x0004,		// 管理员专用消息码（只有当服务器转到运行态，也就是说选定人物后，服务器方会处理）
    NET_ROOT_CHAT = 0x0005,		// 聊天消息管理消息码（只有当服务器转到运行态，也就是说选定人物后，服务器方会处理）
    NET_ROOT_THING = 0x0006,		// 实体定义
    NET_ROOT_INTERACT = 0x0007,		// 交互系统消息码专用
    NET_ROOT_CONTAINER = 0x0008,		// 容器类消息码
    NET_ROOT_TEAM = 0x0009,		// 组队类消息码
    NET_ROOT_FIGHT = 0x0010,		// 战斗系统专用消息码
    NET_ROOT_TRADE = 0x0011,		// 交易系统专用消息码
    NET_ROOT_WEATHER = 0x0012,		// 天气季节时辰消息码
    NET_ROOT_FRIEND = 0x0013,		// 好友系统消息码专用
    NET_ROOT_GOODSOPERATE = 0x0014,		// 物品操作系统消息码
    NET_ROOT_TITLE = 0x0015,		// 称号系统专用消息码
    NET_ROOT_EMAIL = 0x0016,		// 邮件系统
    NET_ROOT_ECTYPE = 0x0017,		// 副本专用消息码
    NET_ROOT_MAX = 0x0018,		// 最大ROOT消息码    
}

