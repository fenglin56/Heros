using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using NetworkCommon;
using UnityEngine;

public enum BASESOCKET_STATE
{
    SOCKET_INITING,         //Socket已经初始化成功
    SOCKET_BINDING,         //Socket已经绑定到Ip：端口
    SOCKET_LISTENING,       //Socket已经在进行端口监听
    SOCKET_CONNECTING,      //Socket已经连接上服务器
    SOCKET_ERROR            //Socket发送错误
}

public enum BASESOCKET_TYPE
{
    SOCKET_TCP,             //TCP套接字
    SOCKET_UDP,             //UDP套接字
    SOCKET_UNKNOW           //未知套接字
}

public enum SOCKET_CODE
{
    SUCCESS,                //处理成功
    ERROR_STATE,            //状态错误
    ERROR_SOCKET,           //Socket套接字失效
    ERROR_BIND,             //绑定错误
    ERROR_CONNECT,          //连接错误
    ERROR_SEND,             //发送错误
    ERROR_RECV,             //接收错误
    ERROR_DATA              //数据内容错误
}

// Socket接收到数据后调用的委托事件
// 传入 接收到的数据数组，数组长度，socket类型
// 传出 所需数据包的大小(如果传入数组长度小于包头长度，返回包头长度
//      如果大于等于包头长度，返回包头的长度字段的值
public delegate int RecvTCPBufferHandle(byte[] recvBuffer, int length, BaseSocket socket);
public delegate void RecvUDPPackageHandle(byte[] recvBuffer, int length, BaseSocket socket);

public abstract class BaseSocket
{

    //Socket类型，用于内部标识
    protected BASESOCKET_STATE state = BASESOCKET_STATE.SOCKET_ERROR;
    //Socket类型，外部标识
    protected BASESOCKET_TYPE type = BASESOCKET_TYPE.SOCKET_UNKNOW;
    //Socket对象
    protected Socket socket;
    //Ip地址
    protected string ipAddr = "127.0.0.1";
    //端口
    protected int port = IpManager.TcpPort;
    //接收缓冲区空间大小
    protected int recvBufferSize = /*10240*/20480;
    //接收缓冲区
    protected byte[] recvBuffer = null;
    //Socket唯一ID
    protected int socketId = 0;
    //群发组ID
    private int socketGroupId = 0;
    //静态变量，用于维护唯一的SocketID
    protected static int maxSocketId = 0;

    ~BaseSocket()
    {
        SocketClose();
    }

    //绑定IP端口
    public SOCKET_CODE SocketBind()
    {
        //只有处于已初始化状态的Socket才能Bind
        if (BASESOCKET_STATE.SOCKET_INITING != state)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(endPoint);
            state = BASESOCKET_STATE.SOCKET_BINDING;
        }
        catch (System.Exception exception)
        {
            //TraceUtil.Log("Base Socket SocketBind:"+exception);
            state = BASESOCKET_STATE.SOCKET_ERROR;
            return SOCKET_CODE.ERROR_BIND;
        }

        return SOCKET_CODE.SUCCESS;
    }

    //绑定指定的IP端口
    public SOCKET_CODE SocketBind(string ip, int p)
    {
        //只有处于已初始化状态的Socket才能Bind
        if (BASESOCKET_STATE.SOCKET_INITING != state)
        {
            return SOCKET_CODE.ERROR_STATE;
        }

        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), p);
            socket.Bind(endPoint);
            state = BASESOCKET_STATE.SOCKET_BINDING;
        }
        catch (System.Exception exception)
        {
            //TraceUtil.Log("Base SocketBind(TCP):"+exception);
            state = BASESOCKET_STATE.SOCKET_ERROR;
            return SOCKET_CODE.ERROR_BIND;
        }

        return SOCKET_CODE.SUCCESS;
    }

    //接收回调函数
    protected abstract void OnRecv(IAsyncResult ar);

    //数据发送函数
    public abstract SOCKET_CODE SocketSend(byte[] data, int offset, int size);

    //查询当前Socket的状态
    public BASESOCKET_STATE SocketState()
    {
        return state;
    }

    //查询当前的Socket类型
    public BASESOCKET_TYPE SocketType()
    {
        return type;
    }

    //获取用于按组群发的组Id，一个Socket对象只能属于一个组
    public int GetGroupId()
    {
        return socketGroupId;
    }

    //设置用于按组群发的组Id，一个Socket对象只能属于一个组
    public void SetGroupId(int id)
    {
        socketGroupId = id;
    }

    //获取该Socket对象的全局唯一ID
    public int GetSocketId()
    {
        return socketId;
    }    
    //TCP异步接收调用的回调函数
    public abstract void TCPSocketCallBackHandle(RecvTCPBufferHandle fun);
    //UDP异步接收调用的回调函数
    public abstract void UDPSocketCallBackHandle(RecvUDPPackageHandle fun);

    public void SocketClose()
    {
        if (socket != null)
        {
            socket.Close();
        }
    }
}

