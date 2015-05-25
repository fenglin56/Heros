
//#define LogOn //LogOn开启打印功能 LogOff关闭
#define LogOff 

using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

public class EntityLog
{
    private string fileName;
    private Dictionary<MyLogType, StringBuilder> m_builderLogDic = new Dictionary<MyLogType, StringBuilder>();
    private StringBuilder m_builderLog;
    private MyLogType m_logType;
    private static EntityLog instance;
    public static EntityLog Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EntityLog();
            }
            return instance;
        }
    }

    private EntityLog()
    {
        //fileName = Application.dataPath + "/" + "EntityLog.txt";
        var m_logType = MyLogType.Error;
        fileName = Application.dataPath + "/" + m_logType.ToString() + ".txt";
        if (!m_builderLogDic.ContainsKey(m_logType))
        {
            m_builderLogDic.Add(m_logType, new StringBuilder());
        }
    }
    private void InitLog(MyLogType logType)
    {
        m_logType = logType;
        fileName = Application.dataPath + "/" + m_logType.ToString()+".txt";
        if (!m_builderLogDic.ContainsKey(logType))
        {
            m_builderLogDic.Add(logType, new StringBuilder());
        }
        m_builderLog = m_builderLogDic[logType];
    }
    /// <summary>
    /// 清空txt文本日志内容
    /// </summary>
    public void ClearLog(MyLogType logtype)
    {
        InitLog(logtype);
        StreamWriter sw = new StreamWriter(fileName);
        sw.WriteLine("");
        sw.Close();
    }

    /// <summary>
    /// 写日志 到内存中
    /// </summary>
    /// <param name="msg">写入的内容</param>
    public void WriteLog(MyLogType logtype, string msg)
    {
        InitLog(logtype);
        m_builderLog.AppendLine(System.DateTime.Now.ToString() + ":" + System.DateTime.Now.ToString("fff") + " debug: " + msg);
    }

    /// <summary>
    /// 写日志
    /// </summary>
    /// <param name="smg"></param>
    public void WriteLog(MyLogType logtype, params string[] smg)
    {
        InitLog(logtype);
        string context = "";
        for (int i = 0; i < smg.Length; i++)
        {
            context += "," + smg[i];
        }
        m_builderLog.AppendLine(System.DateTime.Now.ToString() + ":" + System.DateTime.Now.ToString("fff") + context);
    }

    /// <summary>
    /// 将记录在内存中的日志保存到txt文本中
    /// </summary>
    public void SaveTxt(MyLogType logtype)
    {
        InitLog(logtype);
        FileStream filewriter = new FileStream(@fileName, FileMode.Append, FileAccess.Write);
        StreamWriter sw = new StreamWriter(filewriter);

        sw.Write(m_builderLog.ToString());
        m_builderLog.Remove(0, m_builderLog.Length);
        sw.Close();
        filewriter.Close();

    }


    public void RegisterLogCallback()
    {
        //Application.RegisterLogCallback(CallBackhandle);
    }

    public void RegisterLogNull()
    {
        Application.RegisterLogCallback(null);
    }

    private void CallBackhandle(string errorType, string errorMsg, LogType logType)
    {
        InitLog(MyLogType.Error);
#if LogOn || LogOff
        if (logType == LogType.Error || logType == LogType.Exception)
        {
            //WriteLog("错误类型: " + errorType + "  错误信息: " + errorMsg);
            //SaveTxt();
            //try
            //{
                FileStream filewriter = new FileStream(@fileName, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(filewriter);
                m_builderLog.AppendLine(System.DateTime.Now.ToString() + ":" + System.DateTime.Now.ToString("fff") + " debug: " + "错误类型: " + errorType + "  错误信息: " + errorMsg);
                sw.Write(m_builderLog.ToString());
                m_builderLog.Remove(0, m_builderLog.Length);
                sw.Close();
                filewriter.Close();
            //}
            //catch
            //{

            //}
        }

#endif
    }
}
public enum MyLogType
{
    PlayerEntity,
    PlayerSendMove,
    Error,
}

