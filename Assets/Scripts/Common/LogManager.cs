using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class LogManager 
{
    List<TxtInfo> TxtList = new List<TxtInfo>();

    //本地时间
    public static string SystemTime = System.DateTime.Now.ToString() + ":" + System.DateTime.Now.ToString("fff");

    private static LogManager m_instance;
    public static LogManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new LogManager();
            }
            return m_instance;
        }
    }

    /// <summary>
    /// 清空txt文本日志内容
    /// </summary>
    /// <param name="txtFileName">目标文本名(不需要后缀)</param>
    public void ClearTxt(string txtFileName)
    {
        var txt = TxtList.SingleOrDefault(p => p.Key == txtFileName);
        if (txt != null)
        {
            txt.ClearTxt();
        }
    }

    /// <summary>
    /// 写日志到内存
    /// </summary>
    /// <param name="txtFileName">目标文本名(不需要后缀)</param>
    /// <param name="content">内容</param>
    public void WriteLog(string txtFileName, params string[] content)
    {
        var txt = TxtList.SingleOrDefault(p => p.Key == txtFileName);
        if (txt == null)
        {
            TxtInfo txtInfo = new TxtInfo(txtFileName);
            TxtList.Add(txtInfo);
            txt = txtInfo;
        }
        txt.WirteLog(content);
    }

    /// <summary>
    /// 将记录在内存中的日志保存到txt文本中
    /// </summary>
    /// <param name="txtFileName">目标文本名(不需要后缀)</param>
    public void SaveTxt(string txtFileName)
    {
        var txt = TxtList.SingleOrDefault(p => p.Key == txtFileName);
        if (txt != null)
        {
            txt.SaveTxt();
        }
    }


    class TxtInfo
    {
        public string Key;
        public string FileName;
        public StringBuilder Builder;
		string m_content;

        public TxtInfo(string keyName)
        {
            this.Key = keyName;
            this.FileName = Application.dataPath + "/" + keyName + ".txt";
            Builder = new StringBuilder();
        }

        public void ClearTxt()
        {
            StreamWriter sw = new StreamWriter(FileName);
            sw.WriteLine("");
            sw.Close();
        }

        public void WirteLog(params string[] msg)
        {
            string context = "";
            if (msg.Length > 0)
            {
                context += msg[0];
            }
            for (int i = 1; i < msg.Length; i++)
            {
                context += "," + msg[i];
            }
			Builder.AppendLine(context);
        }

        /// <summary>
        /// 将记录在内存中的日志保存到txt文本中
        /// </summary>
        public void SaveTxt()
        {
            FileStream filewriter = new FileStream(@FileName, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(filewriter);

            sw.Write(Builder.ToString());
            Builder.Remove(0, Builder.Length);
            sw.Close();
            filewriter.Close();
        }

    }

}



