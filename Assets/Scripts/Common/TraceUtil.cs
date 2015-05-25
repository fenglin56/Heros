using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 日志跟踪工具
/// </summary>
public class TraceUtil
{
    /// <summary>
    /// 写日志（默认级别 Verbose）
    /// </summary>
    /// <param name="msg">日志信息</param>
    public static void Log(SystemModel systemModel, string msg)
    {
        Log(systemModel, TraceLevel.Verbose, msg);
    }
    /// <summary>
    /// 写日志（默认级别 Verbose）
    /// </summary>
    /// <param name="msg">日志信息</param>
    public static void Log(string msg)
    {
        Log(SystemModel.Common, TraceLevel.Verbose, msg);
    }
    /// <summary>
    /// 写日志（默认级别 Verbose）
    /// </summary>
    /// <param name="msg">日志信息</param>
    public static void Log(object msg)
    {
        Log(SystemModel.Common, TraceLevel.Verbose, msg.ToString());
    }
    /// <summary>
    /// 打印日志（默认级别 Verbose）
    /// </summary>
    /// <param name="systemModel">功能模块</param>
    /// <param name="format">日志格式："{0},{1}"</param>
    /// <param name="args">日志格式化参数</param>
    public static void Log(SystemModel systemModel, string format, params object[] args)
    {
        Log(systemModel, TraceLevel.Verbose, format, args);
    }   
    /// <summary>
    /// 打印日志
    /// </summary>
    /// <param name="systemModel">功能模块</param>
    /// <param name="level">日志级别</param>
    /// <param name="format">日志格式："{0},{1}"</param>
    /// <param name="args">日志格式化参数</param>
    public static void Log(SystemModel systemModel, TraceLevel level, string format, params object[] args)
    {
        if (CommonDefineManager.Instance == null)
        {
            return;
        }
        if (!CommonDefineManager.Instance.TraceConfigDataBase.PassToTrace(systemModel, level))
        {
            return;
        }
        switch (level)
        {
            case TraceLevel.Info:
            case TraceLevel.Verbose:
                Debug.Log(string.Format(format, args));
                break;
            case TraceLevel.Error:
                Debug.LogError(string.Format(format, args));
                break;
            case TraceLevel.Warning:
                Debug.LogWarning(string.Format(format, args));
                break;
            case TraceLevel.Exception:
                Debug.LogException(new Exception(string.Format(format,args)));
                break;
        }
    }
}

public enum TraceLevel
{
    /// <summary>
    /// 所有日志
    /// </summary>
    Verbose,  
    /// <summary>
    /// 异常
    /// </summary>
    Exception,
    /// <summary>
    /// 错误
    /// </summary>
    Error,
    /// <summary>
    /// 警告
    /// </summary>
    Warning,
    /// <summary>
    /// 信息
    /// </summary>
    Info,
}
/// <summary>
/// 功能模块
/// </summary>
public enum SystemModel
{
    /// <summary>
    /// 新手引导
    /// </summary>
    NewbieGuide,
    /// <summary>
    /// 登录模块
    /// </summary>
    Login,
    /// <summary>
    /// 城镇UI-装备强化
    /// </summary>
    Town_UI_EquipStrength,
    /// <summary>
    /// 通用Log
    /// </summary>
    Common,

    Rocky,
	Jiang,
	wanglei,
	Lee,
	Jamfing,
    Xun,
	NotFoundInTheDictionary,
}

