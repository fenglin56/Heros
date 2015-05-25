using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using NetworkManager;
using NetworkCommon;
using UnityEngine;

/************************************************************************/
/*  UdpSocket继承于BaseSocket
 *  UdpSocket类可用作为服务器和客户端，但同时只具备一个属性，不能同时是客户端和服务端
 *  UdpSocket实现非阻塞式的异步接收，同步发送
 *  UdpSocket调用SetRemote时，将认为该UdpSocket为客户端
 *  调用StartRecv就会进行异步接收的监听，在这之前需要先调用SocketCallBackHandle设置好数据处理的回调函数
 * 
 *  在TCP连接传输数据的过程中，易出现丢包，乱包以及重复包等情况
 *  
/************************************************************************/

public class UdpSocket : BaseSocket
{   
    private IPEndPoint ipEndPoint;                      //本机Ip
    private EndPoint remotePoint;                       //远端Ip
    private event RecvUDPPackageHandle recvEvent;               //事件对象
    private string broadcastIp = "255.255.255.255";     //广播IP
    protected int udpport = IpManager.UdpPort;
    //临时判断
    public static string UdpId = "client";
    public UdpSocket()
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, ProtocolType.Udp);

            ipEndPoint = new IPEndPoint(IPAddress.Any, udpport);
            remotePoint = new IPEndPoint(IPAddress.Parse(broadcastIp), udpport);

            socket.Bind(ipEndPoint);

            state = BASESOCKET_STATE.SOCKET_BINDING;
            UDPSocketCallBackHandle(ServiceManager.OnRecvUDP);
            StartRecv();
        }
        catch(Exception ex)
        {
            //TraceUtil.Log("  UDP Port:"+udpport+ex.ToString());
        }
    }

    public UdpSocket(string ipBind, int p)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, ProtocolType.Udp);

            //更新绑定的IP和端口
            ipAddr = ipBind;
            udpport = p;

            //更新ipEndPoint
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipBind), p);
            //remotePoint = new IPEndPoint(IPAddress.Any, udpport);

            socket.Bind(ipEndPoint);

            state = BASESOCKET_STATE.SOCKET_BINDING;
            UDPSocketCallBackHandle(ServiceManager.OnRecvUDP);
            StartRecv();
        }
        catch (Exception ex)
        {
            //TraceUtil.Log("  UDP Port:" + udpport + ex.ToString());
        }
    }

    //可获取当前的目标Ip地址
    public EndPoint GetRemote()
    {
        return remotePoint;
    }

    //UdpSocket启动数据异步接收
    public SOCKET_CODE StartRecv()
    {
        if (state != BASESOCKET_STATE.SOCKET_BINDING
            && state != BASESOCKET_STATE.SOCKET_LISTENING
            && state != BASESOCKET_STATE.SOCKET_CONNECTING)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        if (null == recvBuffer)
        {
            recvBuffer = new byte[recvBufferSize];
        }

        socket.BeginReceiveFrom(recvBuffer, 0, recvBuffer.Length, SocketFlags.None, ref remotePoint, new AsyncCallback(OnRecv), ipEndPoint);

        return SOCKET_CODE.SUCCESS;
    }

    public override void TCPSocketCallBackHandle(RecvTCPBufferHandle fun)
    {
        //ForTCP,so do nothing here
    }

    protected override void OnRecv(IAsyncResult ar)
    {
        try
        {
            ////TraceUtil.Log("Realy Length1:" );
            //BoxStage.testStr = "Get UDP At OnRecv " + udpPackageAmount++;
            int recvLength = socket.EndReceiveFrom(ar, ref remotePoint);

            recvEvent(recvBuffer, recvLength, this);
        }
        catch 
        {
            ////TraceUtil.Log("UDP 接收出错：" + ex.ToString());
        }
        finally
        {
            //不考虑返回值 
            StartRecv();
        }
    }

    //设置远程主机
    public SOCKET_CODE SetRemote(string ip, int p)
    {
        if (state == BASESOCKET_STATE.SOCKET_ERROR)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        //设置为客户端状态
        state = BASESOCKET_STATE.SOCKET_CONNECTING;

        //主动发送给某台服务器之前要设定目标服务器的Ip和端口
        remotePoint = new IPEndPoint(IPAddress.Parse(ip), p);

        return SOCKET_CODE.SUCCESS;
    }

    public override SOCKET_CODE SocketSend(byte[] data, int offset, int size)
    {
        return SocketSend(data, offset, size, 1);
    }

    //对当前的目标Ip发送单播
    //Udp服务器属于迭代服务器，remotePoint将被自动设置为当前对本机发送数据的远端IP
    //通过调用SetRemote可以强制设定remotePoint为你希望接收到数据的目标服务器
    public SOCKET_CODE SocketSend(byte[] data, int offset, int size,ushort sendTimes)
    {
        if (state == BASESOCKET_STATE.SOCKET_ERROR)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        if (remotePoint == null)
        {
            return SOCKET_CODE.ERROR_SEND;
        }
        socket.SendTo(data.Skip(offset).Take(size).ToArray(), remotePoint);

        return SOCKET_CODE.SUCCESS;
    }

    //发送广播
    public SOCKET_CODE SocketSendBroadcast(byte[] data, int offset, int size)
    { 
        if (state == BASESOCKET_STATE.SOCKET_ERROR)
        {
            return SOCKET_CODE.ERROR_STATE;
        }		
		
        //启动广播
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

        //设置广播IP
        IPEndPoint remote = new IPEndPoint(IPAddress.Parse(broadcastIp), udpport);

        socket.SendTo(data.Skip(offset).Take(size).ToArray(), remote);       

        //关闭广播
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, false);

        return SOCKET_CODE.SUCCESS;
    }

    public SOCKET_CODE SocketSendBroadcast(byte[] data, int offset, int size, int specPort)
    {
        if (state == BASESOCKET_STATE.SOCKET_ERROR)
        {
            return SOCKET_CODE.ERROR_STATE;
        }
       
        //启动广播
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

        //设置广播IP
        IPEndPoint remote = new IPEndPoint(IPAddress.Parse(broadcastIp), specPort);

        socket.SendTo(data.Skip(offset).Take(size).ToArray(), remote);        

        //关闭广播
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, false);

        return SOCKET_CODE.SUCCESS;
    }    
    public override void UDPSocketCallBackHandle(RecvUDPPackageHandle fun)
    {        
        //只允许有一个回调函数
        recvEvent = fun;
    }    
}
