using System.Collections;
using NetworkCommon;
using System;

public abstract class Service :Notifier
{
    protected SocketInfoType SocketInfoType;

    public SocketInfoType GetSocketType()
    {
        return SocketInfoType;
    }  
    public Service()
    {
        SocketInfoType = SocketInfoType.TCPServer;
    }

    public Service(SocketInfoType sType)
    {
        SocketInfoType = sType; 
    }

    public SERVICE_CODE Request(Package pkg)
    {
        byte command = pkg.GetMasterMsgType();
#if UNITY_EDITOR
        String[] logText  = new String[3];
        logText[0] =  "Send:" + System.DateTime.Now.ToLocalTime().ToString();
        logText[1] = pkg.Head.MasterMsgType.ToString();
        logText[2] = pkg.Head.SubMsgType.ToString();
        LogManager.Instance.WriteLog("NetResponse", logText);
#endif
        
        
        SERVICE_CODE ret = SERVICE_CODE.SUCCESS;       
        
        if (!ServiceManager.RequestService(SocketInfoType, pkg))
        {
            ret = SERVICE_CODE.ERROR_NOFOUND;
        }

        return ret;
    }
    public SERVICE_CODE ConnectToServer()
    {
        return ServiceManager.ConnectToService(this.SocketInfoType);
    }
    public SERVICE_CODE CloseSocket()
    {
        return ServiceManager.DestorySocket((int)this.SocketInfoType);
    }
    public virtual void AddToInvoker(CommondResponseHandle commondResponseHandle, byte[] dataBuffer, int socketID)
    {
        ResponseHandleInvoker.Instance.Add(commondResponseHandle, dataBuffer, socketID);
    }
    
    public abstract void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype);
}
public abstract class UDPService : Service
{
    public UDPService()
        : base(SocketInfoType.UDPServer)
    {
    }
    public SERVICE_CODE Broadcast(Package pkg)
    {
        byte command = pkg.GetMasterMsgType();
        SERVICE_CODE ret = SERVICE_CODE.SUCCESS;

        if (!ServiceManager.UDPBroadcast(pkg))
        {
            ret = SERVICE_CODE.ERROR_NOFOUND;
        }

        return ret;
    }
    public SERVICE_CODE UDPSpecSend(Package pkg,IpInfo? targetIpInfo)
    {
        byte command = pkg.GetMasterMsgType();
        SERVICE_CODE ret = SERVICE_CODE.SUCCESS;

        if (!ServiceManager.UDPSpecSend(pkg,targetIpInfo))
        {
            ret = SERVICE_CODE.ERROR_NOFOUND;
        }

        return ret;
    }    
}
public delegate CommandCallbackType CommondResponseHandle(byte[] dataBuffer, int socketId);
public delegate void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, int command);
public enum CommandCallbackType
{
    Continue,
    Pause,
    Stop,
}
