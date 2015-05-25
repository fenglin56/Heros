using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Notifier
{
    private Dictionary<string, NotifyManager.StandardDelegate> m_eventMap = new Dictionary<string, NotifyManager.StandardDelegate>();  //事件-方法, 用于销毁事件
    public Notifier()
    {
    }
   
    public void AddEventHandler(string eventName, NotifyManager.StandardDelegate d)
    {
        NotifyManager.AddEventHandler(eventName, d);
        if (!m_eventMap.ContainsKey(eventName))
            m_eventMap[eventName] = d;
        else
            m_eventMap[eventName] += d;        
    }

    public void RemoveEventHandler(string eventName, NotifyManager.StandardDelegate d)
    {
        NotifyManager.RemoveEventHandler(eventName, d);
        m_eventMap[eventName] -= d;
    }

    public void ClearEvent()
    {
        var buffer = new List<string>(m_eventMap.Keys);
        foreach (var key in buffer)
        {
            m_eventMap[key] = null;
        }
        m_eventMap.Clear();
    }
    //触发某事件（事件名 - 参数）
    public void RaiseEvent(string eventName, INotifyArgs e)
    {
        NotifyManager.RaiseEvent(eventName, e);
    }

    //在销毁的时候，将自己注册的事件从NotifyManager中注销掉
    ~Notifier()
    {
        foreach (KeyValuePair<string, NotifyManager.StandardDelegate> kv in m_eventMap)
        {
            NotifyManager.RemoveEventHandler(kv.Key, kv.Value);
        }
        m_eventMap.Clear();
    }
}
