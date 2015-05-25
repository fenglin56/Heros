using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotifyManager
{  
    private static Dictionary<string, StandardDelegate> m_eventMap = new Dictionary<string, StandardDelegate>();
    public delegate void StandardDelegate(INotifyArgs e);
    //添加某事件处理
    public static void AddEventHandler(string eventName, StandardDelegate d)
    {
        if (!m_eventMap.ContainsKey(eventName))
        {
            m_eventMap[eventName] = d;
        }
        else
        {
            m_eventMap[eventName] += d;
        }       
    }

    //移除某事件处理函数
    public static void RemoveEventHandler(string eventName, StandardDelegate d)
    {
        if (m_eventMap.ContainsKey(eventName))
        {
            if (m_eventMap[eventName] != null)
            {
                m_eventMap[eventName] -= d;
            }
        }
    }

    public static void ClearEvents()
    {
        var buffer = new List<string>(m_eventMap.Keys);
        foreach (var key in buffer)
        {
            m_eventMap[key] = null;
        }
        m_eventMap.Clear();       
    }
    //触发某事件，将通知到所有的事件监听对象
    public static void RaiseEvent(string eventName, INotifyArgs e)
    {
        StandardDelegate fun = null;
		if(m_eventMap.TryGetValue(eventName, out fun))
		{
            if ( null != fun )
            {
                fun(e);
            }
		}
    }

    //判断某事件是否已经注册，可用于防止模型重命名
    public static bool HasEvent(string eventName)
    {
        return m_eventMap.ContainsKey(eventName);
    }
    #region AddedByRocky At 2012-12-06
    //发出清除事件注册通知，在场景转换的时候做,
    public delegate void ClearEventRegisteHandler(string eventPrefix);
    public static event ClearEventRegisteHandler OnClearEventRegiste;
    public static void InvokeClearEventRegiste(string eventPrefix)
    {
        if (OnClearEventRegiste != null)
        {
            OnClearEventRegiste(eventPrefix);
        }
    }
    #endregion
}