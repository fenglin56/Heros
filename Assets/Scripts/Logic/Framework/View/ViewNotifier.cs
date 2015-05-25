using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewNotifier : MonoBehaviour {
    
    /// <summary>
    /// 自身的eventMap
    /// </summary>
    private Dictionary<string, NotifyManager.StandardDelegate> m_eventMap = new Dictionary<string, NotifyManager.StandardDelegate>();  //事件-方法, 用于销毁事件

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
        if (m_eventMap.ContainsKey(eventName))
        {
            NotifyManager.RemoveEventHandler(eventName, d);
            m_eventMap[eventName] -= d;
        }
    }

    //触发某事件（事件名 - 参数）
    public void RaiseEvent(string eventName, INotifyArgs e)
    {
        NotifyManager.RaiseEvent(eventName, e);
    }
	
	public void RemoveAllEvent()
	{
		foreach (KeyValuePair<string, NotifyManager.StandardDelegate> kv in m_eventMap)
        {
            NotifyManager.RemoveEventHandler(kv.Key, kv.Value);
        }
	}

    //在销毁的时候，将自己注册的事件从NotifyManager中注销掉
    protected virtual void OnDestroy()
    {
        RemoveAllEvent();
    }
}
