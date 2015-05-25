using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using UnityEngine;
using NetworkCommon;
using System.Runtime.InteropServices;

/************************************************************************/
/*  TcpSocket继承于BaseSocket
 *  TcpSocket类可用作为服务器和客户端，但同时只具备一个属性，不能同时是客户端和服务端
 *  TcpSocket实现同步和异步的连接，非阻塞式的异步接收，同步发送
 *  TcpSocket实现对缓冲区的管理，连包半包等情况交由上层与协议相关的调用者来判断
 *  当客户端Socket连接上，或服务端Socket开始监听，就会进行异步接收的监听
 * 
 *  使用TCP连接会出现连接不上，连接中断等异常
 *  在TCP连接传输数据的过程中，易出现连包，半包的情况
 *  
/************************************************************************/

public class TcpSocket : BaseSocket
{
    //事件对象
    public event RecvTCPBufferHandle recvEvent;
    //监听队列大小
    private int maxClient = 4;
    //备份缓冲区，用来缓存半包
    private byte[] backupBuffer = null;
    //数据包的最大长度
    private int maxPackage = 10240;

    public TcpSocket()
    {
        //创建流式套接字TCP
        socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
        state = BASESOCKET_STATE.SOCKET_INITING;
        type = BASESOCKET_TYPE.SOCKET_TCP;
        socket.SendTimeout = 500;
        //默认将ServiceManager的OnRecv作为默认回调函数
        TCPSocketCallBackHandle(ServiceManager.OnRecv);
    }

    public TcpSocket(int id):this()
    {
        socketId = id;
    }

    public TcpSocket(Socket s, int id)
    {
        socket = s;
        socketId = id;

        state = BASESOCKET_STATE.SOCKET_CONNECTING;
        type = BASESOCKET_TYPE.SOCKET_TCP;

        TCPSocketCallBackHandle(ServiceManager.OnRecv);
    }

    public override void TCPSocketCallBackHandle(RecvTCPBufferHandle fun)
    {
        //只允许有一个回调函数
        recvEvent = fun;
    }

    #region 连接

    //阻塞式连接,使用默认Ip端口进行连接
    public SOCKET_CODE SocketConnect()
    {
        //如果Socket未初始化且不是绑定状态
        if (BASESOCKET_STATE.SOCKET_INITING != state
            && BASESOCKET_STATE.SOCKET_BINDING != state)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        IPAddress addr = IPAddress.Parse(ipAddr.Trim());

        try
        {
            //使用默认的端口和地址进行连接
            socket.Connect(addr, port);
        }
        catch 
        {
            state = BASESOCKET_STATE.SOCKET_ERROR;
            return SOCKET_CODE.ERROR_CONNECT;
        }

        //连接成功更新状态为已连接
        state = BASESOCKET_STATE.SOCKET_CONNECTING;

        //启动接收
        StartRecv();

        return SOCKET_CODE.SUCCESS;
    }

    //阻塞式连接
    public SOCKET_CODE SocketConnect(string ip, int p)
    {
        ipAddr = ip;
        port = p;

        return SocketConnect();
    }

    //非阻塞式连接,使用默认Ip端口进行连接
    public SOCKET_CODE SocketConnectAsync()
    {
        //如果Socket未初始化且不是绑定状态
        if (BASESOCKET_STATE.SOCKET_INITING != state
            && BASESOCKET_STATE.SOCKET_BINDING != state)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        IPAddress addr = IPAddress.Parse(ipAddr.Trim());
        IPEndPoint endPoint = new IPEndPoint(addr, port);

        try
        {
            //使用默认的端口和地址进行连接
            socket.BeginConnect(endPoint, new AsyncCallback(OnConnect), null);
        }
        catch 
        {
            state = BASESOCKET_STATE.SOCKET_ERROR;
            return SOCKET_CODE.ERROR_CONNECT;
        }

        return SOCKET_CODE.SUCCESS;
    }

    //非阻塞式连接
    public SOCKET_CODE SocketConnectAsync(string ip, int p)
    {
        ipAddr = ip;
        port = p;

        return SocketConnectAsync();
    }

    //连接的回调函数
    private void OnConnect(IAsyncResult ar)
    {
        //连接完成
        socket.EndConnect(ar);

        //连接成功更新状态为已连接
        state = BASESOCKET_STATE.SOCKET_CONNECTING;

        //启动接收
        StartRecv();
    }

    #endregion

    #region 监听

    //开始异步接受连接请求
    private SOCKET_CODE StartAccept()
    {
        if (BASESOCKET_STATE.SOCKET_LISTENING != state)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        socket.BeginAccept(new AsyncCallback(OnAccept), null);

        return SOCKET_CODE.SUCCESS;
    }

    //服务端Socket启动监听，同时开始接受新连接
    public SOCKET_CODE Listen()
    {
        //如果不为已绑定状态
        if (BASESOCKET_STATE.SOCKET_BINDING != state)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        socket.Listen(maxClient);

        state = BASESOCKET_STATE.SOCKET_LISTENING;

        StartAccept();

        return SOCKET_CODE.SUCCESS;
    }

    //接受新连接的回调函数，子类需重新实现
    protected void OnAccept(IAsyncResult ar)
    {
        //自增最大SocketId
        ++maxSocketId;
        Socket client = socket.EndAccept(ar);

        //创建一个客户端
        TcpSocket tcpClient = new TcpSocket(client, maxSocketId);

        tcpClient.TCPSocketCallBackHandle(recvEvent);

        //启动此客户端的异步接收
        tcpClient.StartRecv();

        //添加到Socket池进行管理
        //如果添加失败
	        if (!SocketManager.SocketAdd(maxSocketId, tcpClient))
	        {
	            throw (new Exception("Add Socket To Socket Pool Faile"));
	        }

        //重新开始接收新连接
        StartAccept();
    }

    #endregion

    #region 发送

    //异步发送数据
    public override SOCKET_CODE SocketSend(byte[] data, int offset, int size)
    {
        
        //只有以下两种情况可以发送数据
        //1.当我是服务器，我可以对已经连接上我的客户端进行发送数据
        //2.当我是客户端，且已经连接上服务器
        //tcp服务端套接字只能用于接收连接，无法用于发送数据
        if (BASESOCKET_STATE.SOCKET_CONNECTING != state)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        try
        {
            socket.BeginSend(data, offset, size, SocketFlags.None, new AsyncCallback(OnSend), null);
        }
        //当发生套接字错误，表示该套接字已经出现异常
        catch 
        {
            state = BASESOCKET_STATE.SOCKET_ERROR;
            return SOCKET_CODE.ERROR_SOCKET;
        }

        return SOCKET_CODE.SUCCESS;
    }

    //结束异步发送
    private void OnSend(IAsyncResult ar)
    {
        socket.EndSend(ar);
    }

    #endregion

    #region 接收

    private SOCKET_CODE StartRecv()
    {
        if (BASESOCKET_STATE.SOCKET_CONNECTING != state)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        if (recvBuffer == null)
        {
            recvBuffer = new byte[recvBufferSize];
        }

        socket.BeginReceive(recvBuffer, 0, recvBufferSize, SocketFlags.None, new AsyncCallback(OnRecv), null);

        return SOCKET_CODE.SUCCESS;
    }

    protected override void OnRecv(IAsyncResult ar)
    {
        try
        {
            int recvLength = socket.EndReceive(ar);
            int pkgLength = 0;
            byte[] data;
            //如果半包数据不为空
            if (null != backupBuffer)
            {
                //将半包和后面接收到的数据报拼接起来
                data = backupBuffer.Concat(recvBuffer.Take(recvLength).ToArray()).ToArray();
            }
            else
            {
                data = recvBuffer.Take(recvLength).ToArray();
            }            
            //解决连包问题
            while (true)
            {
                // 1. recvEvent返回一个完整的数据包长度
                pkgLength = recvEvent(data, data.Length, this);
                
                if (pkgLength <= 0 || pkgLength > maxPackage)
                {
                    //出现异常，连接关闭                    
                    state = BASESOCKET_STATE.SOCKET_ERROR;
                    break;
                }

                // 2. 如果正常处理完,则直接启动下次接收
                if (pkgLength == data.Length)
                {
                    ////TraceUtil.Log("正常");
                    backupBuffer = null;
                    Array.Clear(recvBuffer, 0, recvBuffer.Length);
                    pkgLength = 0;
                    break;
                }
                // 3. 如果还未接收完,半包则在偏移处等待
                else if (pkgLength > data.Length)
                {
                    //将半包数据拷贝到backupBuffer
                    ////TraceUtil.Log("半包");
                    backupBuffer =  data.Take(data.Length).ToArray();
                    break;
                }
                // 4. 如果是连包，则循环处理
                else if (pkgLength < data.Length)
                {
                    //自增偏移量并跳过已经处理的包
                    ////TraceUtil.Log("连包");
                    data = data.Skip(pkgLength).ToArray();
                    backupBuffer = null;
                }
                else
                {
                    break;
                }
            }

            StartRecv();
        }
        catch
        {
            state = BASESOCKET_STATE.SOCKET_ERROR;
            this.SocketClose();
        }
    }

    #endregion

    
    public override void UDPSocketCallBackHandle(RecvUDPPackageHandle fun)
    {
        //ForUDP,so do nothing here 
    }
}

