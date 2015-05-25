using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using NetworkManager;
using System.Text;
using System.Linq;
using UnityEngine;

public struct IpInfo
{
    public string ip;
    public int port;
    public BASESOCKET_TYPE baseType;
}

public enum SERVICE_CODE
{
    SUCCESS,                 
    ERROR_NOFOUND,           
    ERROR_EXIST,             
    ERROR_NOCONF,            
    ERROR_CONNECT            
}

public sealed class ServiceManager
{
    private static Dictionary<byte, Service> ServiceMap = new Dictionary<byte, Service>();
    private static ASCIIEncoding m_dataEncoding = new ASCIIEncoding();

    private static Dictionary<int, IpInfo> AddrConfMap = new Dictionary<int, IpInfo>();
    public  static Dictionary<int, BaseSocket> connectorMap = new Dictionary<int, BaseSocket>();
       
    public static string ServerInfo()
    {
        return AddrConfMap[0].ip + "  " + AddrConfMap[0].port;
    }
    public static SERVICE_CODE SetConfig(IpInfo info)
    {
        AddrConfMap[0] = info;
        return SERVICE_CODE.SUCCESS;
    }
    public static SERVICE_CODE SetConfig(SocketInfoType socketInfoType, IpInfo info)
    {
        int socketType = (int)socketInfoType;
        AddrConfMap[socketType] = info;
        return SERVICE_CODE.SUCCESS;
    }
    public static int CheckPackageLength(byte[] recvBuffer, int length, out byte masterMsgType)
    {
        //CommonTrace.Log("Length:" + length);
        if (length < 5)   //5    Package length field(2) MasterMsgType 1 SubMsgType 2
        {
            masterMsgType = 0;
            return 5;
        }
        var binaryHead = PackageHelper.BytesToStuct<PkgHead>(recvBuffer);

        masterMsgType = binaryHead.MasterMsgType;

        return binaryHead.DataLength + 2;  //整个包的长度
    } 
   
    public static void DestorySockets()
    {
        foreach (int key in connectorMap.Keys)
        {
            connectorMap[key].SocketClose();
        }

        connectorMap.Clear();
    }

    public static SERVICE_CODE DestorySocket(int type)
    {
        if (connectorMap.ContainsKey(type))
        {
            connectorMap[type].SocketClose();
            connectorMap.Remove(type);
            return SERVICE_CODE.SUCCESS;
        }

        return SERVICE_CODE.ERROR_NOFOUND;
    }

    public static int OnRecv(byte[] buffer, int length, BaseSocket socket)
    {
        int ret = 0;

        byte commandType=0;

        BASESOCKET_TYPE socketType = socket.SocketType();
                
        switch (socketType)
        {
            case BASESOCKET_TYPE.SOCKET_TCP:               
                {                    
                    ret = CheckPackageLength(buffer, length, out commandType);
                    ////TraceUtil.Log("ServiceManager OnRecv length:" + length + "ret " + ret);
                    if (ret > length)
                    {
                        return ret;
                    }
                }
                break;

            case BASESOCKET_TYPE.SOCKET_UDP:
                ret = length;
                break;

            default:
                break;
        }

        if (ServiceMap.ContainsKey(commandType))
        {
            //TraceUtil.Log("Data Length:" + length + "  CheckLength:" + ret + " BuffLength:" + buffer.Length);
            //TraceUtil.Log("ReceiveTCP:" + commandType + "," + (byte)MasterMsgType.NET_ROOT_TRADE);
            ServiceMap[commandType].SaveResponseHandleToInvoke(buffer.Take(ret).ToArray(), socket.GetSocketId(), commandType);
        }
        else
        {
            //TraceUtil.Log("Data Length:" + length + "  CheckLength:" + ret + " BuffLength:" + buffer.Length);
        }

        return ret;
    }
    public static void OnRecvUDP(byte[] recvBuffer, int length, BaseSocket socket)
    {
        //MultiGameView.Log("Recieve collect Player Request ServiceManager:");
        byte commandType;
        //CommonTrace.Log(string.Format("收到UDP：数据类型{0}", packageType)); 
        var binaryHead = PackageHelper.BytesToStuct<PkgHead>(recvBuffer);

        commandType = binaryHead.MasterMsgType;

        int ret = binaryHead.DataLength;

        if (ServiceMap.ContainsKey(commandType))
        {
            ServiceMap[commandType].SaveResponseHandleToInvoke(recvBuffer.Take(ret).ToArray(), socket.GetSocketId(), commandType);
        }
    }
    public static SERVICE_CODE RegistService(byte cmd, Service registerService)
    {
        ServiceMap[cmd] = registerService;
        //ConnectToService(SocketInfoType.TCPServer);
        PrepareUDPSocket();

        return SERVICE_CODE.SUCCESS;
    }

    public static bool UnRegistService(byte cmd)
    {
        return ServiceMap.Remove(cmd);
    }
    public static bool RequestService(SocketInfoType socketInfoType, Package pkg)
    {
        int type = (int)socketInfoType;
        //if (ConnectToService(socketInfoType) == SERVICE_CODE.SUCCESS) //注掉这行代码 意味着用户需要显式连接后再发送
        {
            if (connectorMap.ContainsKey(type))
            {
                BaseSocket s = connectorMap[type];

                var sendBuff = PackageHelper.GetNetworkSendBuffer(pkg);
                if (s.SocketSend(sendBuff, 0, sendBuff.Length) == SOCKET_CODE.SUCCESS)
                {
                    return true;
                }
                else
                {
                    connectorMap.Remove(type);
                    return false;
                }
            }
        }

        return false;
    }
	 //TCP客户端向服务器发出请求    
    public static bool UDPBroadcast(Package pkg)
    {
        int type = (int)SocketInfoType.UDPServer;
        if (connectorMap.ContainsKey(type))
        {
            UdpSocket s = connectorMap[type] as UdpSocket;

            var sendBuff = PackageHelper.GetNetworkSendBuffer(pkg);

            s.SocketSendBroadcast(sendBuff, 0, sendBuff.Length);
            return true;
        }

        return false;
    }
    public static bool UDPSpecSend(Package pkg, IpInfo? targetIpInfo)
    {
        int type = (int)SocketInfoType.UDPServer;
        if (connectorMap.ContainsKey(type))
        {
            UdpSocket s = connectorMap[type] as UdpSocket;
            var sendBuff = PackageHelper.GetNetworkSendBuffer(pkg);

            if (targetIpInfo != null)
            {
                s.SetRemote(targetIpInfo.Value.ip, targetIpInfo.Value.port);
                s.SocketSend(sendBuff, 0, sendBuff.Length);
            }
            else
            {
                s.SocketSendBroadcast(sendBuff, 0, sendBuff.Length);
            }

            
            return true;
        }

        return false;
    }    
    private static SERVICE_CODE PrepareUDPSocket()
    {
        SERVICE_CODE service_code = SERVICE_CODE.SUCCESS;
        int socketType = (int)SocketInfoType.UDPServer;
        if (!connectorMap.ContainsKey(socketType))
        {
             if (AddrConfMap.ContainsKey(socketType))
            {
                IpInfo info = AddrConfMap[socketType];
                UdpSocket udpSocket = new UdpSocket();
                udpSocket.SetRemote(info.ip, info.port);
                connectorMap[socketType] = udpSocket;
             }
            else
            {
                service_code = SERVICE_CODE.ERROR_NOCONF;
            }
        }
        return service_code;
    }

    public static SERVICE_CODE ConnectToService(SocketInfoType socketInfoType)
    {
        SERVICE_CODE  service_code=SERVICE_CODE.SUCCESS;
        int socketType = (int)socketInfoType;
        if (!connectorMap.ContainsKey(socketType))
        {
            //CommonTrace.Log("ConnectToService");
            if (AddrConfMap.ContainsKey(socketType))
            {
                //CommonTrace.Log("AddrConfMap.ContainsKey(socketType)");
                IpInfo info = AddrConfMap[socketType];
                switch (info.baseType)
                {
                    case BASESOCKET_TYPE.SOCKET_TCP:
                        TcpSocket tcpSocket = new TcpSocket();
                        try
                        {
                            SOCKET_CODE socketCode= tcpSocket.SocketConnect(info.ip, info.port);
                            if (socketCode == SOCKET_CODE.SUCCESS)
                            {
                                connectorMap[socketType] = tcpSocket;
                            }
                            else
                            {
                                service_code = SERVICE_CODE.ERROR_CONNECT;
                            }
                        }
                        catch (System.Exception)
                        {
                            service_code= SERVICE_CODE.ERROR_CONNECT;
                        }
                        break;

                    case BASESOCKET_TYPE.SOCKET_UDP:
                        UdpSocket udpSocket = new UdpSocket();
                        udpSocket.SetRemote(info.ip, info.port);
                        connectorMap[socketType] = udpSocket;
                       
                        break;

                    default:
                        break;
                }
            }
            else
            {
                //CommonTrace.Log("!AddrConfMap.ContainsKey(socketType)");
                service_code = SERVICE_CODE.ERROR_NOCONF;
            }
        }
        return service_code;
    }
}
public enum SocketInfoType
{
    TCPServer = 0,
    UDPServer = 1,
}
