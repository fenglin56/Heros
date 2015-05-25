
#define LogOn //LogOn开启打印功能 LogOff关闭
//#define LogOff 

using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class Log
{
    public const bool IsPrint = true;
    private string errorFileName;
    private string fileName;
    private string otherFileName;
    private StringBuilder m_builderLog = new StringBuilder();

    private static Log instance;
    public static Log Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Log();                
            }
            return instance;
        }
    }

    private Log()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        fileName = Application.dataPath + "/" + "EntityLog.txt";
        otherFileName = Application.dataPath + "/" + "MonsterPosLog.txt";
        errorFileName = Application.dataPath + "/" + "ErrorLog.txt";
#elif UNITY_ANDROID
        fileName = "/sdcard/PartyLog.txt";
#endif
    }

    /// <summary>
    /// 清空txt文本日志内容
    /// </summary>
    public void ClearLog()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        StreamWriter sw = new StreamWriter(fileName);
        sw.WriteLine("");
        sw.Close();
        StreamWriter otherSw = new StreamWriter(otherFileName);
        otherSw.WriteLine("");
        otherSw.Close();
#elif UNITY_ANDROID
#endif
    }

    /// <summary>
    /// 写日志 到内存中
    /// </summary>
    /// <param name="msg">写入的内容</param>
    public void WriteLog(string msg)
    {
#if LogOn
        m_builderLog.AppendLine(System.DateTime.Now.ToString() + ":" + System.DateTime.Now.ToString("fff") + " debug: " + msg);
#elif LogOff
#endif
    }

    /// <summary>
    /// 写日志
    /// </summary>
    /// <param name="smg"></param>
    public void WriteLog(params string[] smg)
    {
        string context = "";
        for (int i = 0; i < smg.Length; i++)
        {
            context += "," + smg[i];
        }
        m_builderLog.AppendLine(System.DateTime.Now.ToString() + ":" + System.DateTime.Now.ToString("fff") + context);
    }

    float _fillTime;
    public void StartTiming()
    {        
        _fillTime = Time.time;
    }
    public void StartLog()
    {
        addContext = (Time.time - _fillTime).ToString();        
    }

    string addContext = "";
    public void AddLog(params string[] msg)
    {
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR
        for (int i = 0; i < msg.Length; i++)
        {
            addContext += "," + msg[i];
        }
        #elif UNITY_ANDROID
        #endif
    }
    string addOtherContext = "";
    public void AddOtherLog(params string[] msg)
    {
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR
        for (int i = 0; i < msg.Length; i++)
        {
            addOtherContext += "," + msg[i];
        }
#elif UNITY_ANDROID
#endif
    }
    public void AppendLine()
    {
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR
        m_builderLog.AppendLine(addContext);
        addContext = "";        
        #elif UNITY_ANDROID
        #endif
    }
    public void AppendOtherLine()
    {
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR
        m_builderLog.AppendLine(addOtherContext);
        addOtherContext = "";
        SaveOtherTxt();
        #elif UNITY_ANDROID
        #endif
    }

    /// <summary>
    /// 将记录在内存中的日志保存到txt文本中
    /// </summary>
    public void SaveTxt()
    {


        FileStream filewriter = new FileStream(@fileName, FileMode.Append, FileAccess.Write);
        StreamWriter sw = new StreamWriter(filewriter);

        sw.Write(m_builderLog.ToString());
        m_builderLog.Remove(0, m_builderLog.Length);
        sw.Close();
        filewriter.Close();

    }

    //\临时
    public void WirteOhterLog(string msg)
    {
        addOtherContext = msg;
        AppendOtherLine();
    }

    public void SaveOtherTxt()
    {
        FileStream filewriter = new FileStream(@otherFileName, FileMode.Append, FileAccess.Write);
        StreamWriter sw = new StreamWriter(filewriter);

        sw.Write(m_builderLog.ToString());
        m_builderLog.Remove(0, m_builderLog.Length);
        sw.Close();
        filewriter.Close();
    }

    public void RegisterLogCallback()
    {
        Application.RegisterLogCallback(CallBackhandle);
    }

    public void RegisterLogNull()
    {
        Application.RegisterLogCallback(null);
    }

    private void CallBackhandle(string errorType, string errorMsg, LogType logType)
    {
        if (logType == LogType.Error || logType == LogType.Exception)
        {
            //WriteLog("错误类型: " + errorType + "  错误信息: " + errorMsg);
            try
            {
                FileStream filewriter = new FileStream(@errorFileName, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(filewriter);
                StringBuilder builderLog = new StringBuilder() ;
                builderLog.AppendLine(System.DateTime.Now.ToString() + ":" + System.DateTime.Now.ToString("fff") + " debug: " + "错误类型: " + errorType + "  错误信息: " + errorMsg);
                sw.Write(builderLog.ToString());                
                sw.Close();
                filewriter.Close();
            }
            catch
            {
               
            }
        }
    }
}

