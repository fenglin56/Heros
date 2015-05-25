using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkManager;
using UnityEngine;
using NetworkCommon;
using System.Runtime.InteropServices;

/************************************************************************/
/*  Cienter
 *  Accepter
 *  Connecter
/************************************************************************/

public enum SOCKET_TYPE
{
    SOCKET_DEFAULT,
    SOCKET_CLIENT,
    SOCKET_SERVER,
    SOCKET_ALL
}

public class SocketManager
{
    //Socket列表池
    public static Dictionary<int, BaseSocket> socketPool = new Dictionary<int, BaseSocket>();
    private static List<int> m_tryToRemoveSocketIdBuffer = new List<int>();

    public static bool SocketCheck(int id)
    {
        return socketPool.ContainsKey(id);
    }

    public static BaseSocket SocketFind(int id)
    {
        if (SocketCheck(id))
        {
            return socketPool[id];
        }
        else
        {
            return null;
        }
    }

    public static bool SocketAdd(int id, BaseSocket s)
    {
        if (null == s)
        {
            return false;
        }

        //如果不包含该Id
        if (!socketPool.ContainsKey(id))
        {
            socketPool[id] = s;
        }
        else
        {
            return false;
        }

        return true;
    }

    public static bool SocketDel(int id, BaseSocket s)
    {
        if (socketPool.ContainsKey(id))
        {
            socketPool[id].SocketClose();
            socketPool.Remove(id);
            return true;
        }

        return false;
    }


    //2012.11.26 ada 清空socket池
    public static void SocketDelAll()
    {
        foreach (BaseSocket bs in socketPool.Values)
        {
            bs.SocketClose();
        }
        socketPool.Clear();
    }

    //主动发送
    public static bool SocketSend(int id, Package pkg)
    {
        if (socketPool.ContainsKey(id))
        {
            try
            {
                BaseSocket s = socketPool[id];
                if (s.GetSocketId() != IpManager.ListenSocketId)
                {
                    var sendBytes = PackageHelper.GetNetworkSendBuffer(pkg);
                    if (s.SocketSend(sendBytes, 0, sendBytes.Length) != SOCKET_CODE.SUCCESS)
                    {
                        socketPool[id].SocketClose();
                        socketPool.Remove(id);
                    }
                    return true;
                }
            }
            catch
            {
                socketPool[id].SocketClose();
                socketPool.Remove(id);
            }
        }

        return false;
    }

    //全部发送
    public static void SocketSend(Package pkg)
    {
        var sendBytes1 = PackageHelper.GetNetworkSendBuffer(pkg);
       
        m_tryToRemoveSocketIdBuffer.Clear();
        foreach (int key in socketPool.Keys)
        {
            try
            {
                BaseSocket s = socketPool[key];
                var sendBytes = PackageHelper.GetNetworkSendBuffer(pkg);
                if (s.SocketSend(sendBytes, 0, sendBytes.Length) != SOCKET_CODE.SUCCESS)
                {
                    m_tryToRemoveSocketIdBuffer.Add(key);
                }
            }
            catch
            {
                m_tryToRemoveSocketIdBuffer.Add(key);
            }
        }
        foreach(int id in m_tryToRemoveSocketIdBuffer)
        {
            socketPool[id].SocketClose();
            socketPool.Remove(id);
        }
        m_tryToRemoveSocketIdBuffer.Clear();
    }  

    //按组发送
    public static void SocketSendByGroup(int groupId, Package pkg)
    {
        m_tryToRemoveSocketIdBuffer.Clear();
        foreach (int key in socketPool.Keys)
        {
            try
            {
                BaseSocket s = socketPool[key];
                //组Id匹配
                if (groupId == s.GetGroupId())
                {
                    var sendBytes = PackageHelper.GetNetworkSendBuffer(pkg);                    ;
                    if (s.SocketSend(sendBytes, 0, sendBytes.Length) != SOCKET_CODE.SUCCESS)
                    {
                        m_tryToRemoveSocketIdBuffer.Add(key);
                    }
                }
            }
            catch
            {
                m_tryToRemoveSocketIdBuffer.Add(key);
            }
        }
        foreach (int id in m_tryToRemoveSocketIdBuffer)
        {
            socketPool[id].SocketClose();
            socketPool.Remove(id);
        }
        m_tryToRemoveSocketIdBuffer.Clear();
    }
}

