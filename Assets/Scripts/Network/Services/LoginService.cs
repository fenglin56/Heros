using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCommon;
using UnityEngine;

public class LoginService : Service
{
    private bool m_isAutoLogin = false;
    private string m_ip = "112.124.61.76";
    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        try
        {
            Package package;
            MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
            package = PackageHelper.ParseReceiveData(dataBuffer);
            switch (masterMsgType)
            {
                case MasterMsgType.NET_ROOT_LOGIN:
                    switch (package.GetSubMsgType())
                    {
                        //S发送登录随机数到C
                        case RootDefineManager.ROOTLOGIN_SC_MAIN_ENTERCODE:
                            AddToInvoker(this.ReceiveEnterCode, dataBuffer, socketId);
                            break;
                        //S发送角色信息到C
                        case RootDefineManager.ROOTLOGIN_SC_MAIN_LOGINRES:
                            //if (GameManager.Instance.IsLogin91Version)
                            {
                                AddToInvoker(this.NewReceiveLoginRes, dataBuffer, socketId);
                            }
                            //else
                            //{
                            //    AddToInvoker(this.ReceiveLoginRes, dataBuffer, socketId);
                            //}
                            break;
                        default:
                            //TraceUtil.Log("NET_ROOT_LOGIN:" + package.GetSubMsgType());
                            break;
                    }
                    break;
                case MasterMsgType.NET_ROOT_SELECTACTOR:
                    switch (package.GetSubMsgType())
                    {
                        //S发送跳转网关到C
                        case RootDefineManager.ROOTSELECTACTOR_CS_MAIN_JUMP_START:
                            if (GameManager.Instance.EnableHeart && HeartFPSManager.Instance.StartHeart)
                            {
                                HeartFPSManager.Instance.StartHeart = false;
                            }
                            SLoginUserJumpStartRes sLoginUserJumpStartRes = SLoginUserJumpStartRes.ParsePackage(dataBuffer);
                            this.CloseSocket();
                            //if (GameManager.Instance.IsLogin91Version)
                            {
                                IpManager.InitServiceConfig(LoginManager.Instance.LoginSSActorInfo.SZServerIP, LoginManager.Instance.LoginSSActorInfo.wPort);
                            }
                            //else
                            //{
                            //    IpManager.InitServiceConfig(LoginManager.Instance.LoginSSActorInfo.SZServerIP, LoginManager.Instance.LoginSSActorInfo.wPort);
                            //}
                            this.ConnectToServer();
                            break;
                        //S发送登录随机数到C
                        case RootDefineManager.ROOTSELECTACTOR_CS_MAIN_ENTERCODE:
                            //if (GameManager.Instance.IsLogin91Version)
                            {
                                this.NewSendMacToGateway();
                            }
                            //else
                            //{
                            //    this.SendMacToGateway();
                            //}

                            break;
                        default:
                            //TraceUtil.Log("NET_ROOT_SELECTACTOR:" + package.GetSubMsgType());
                            break;
                    }
                    break;
                default:
                    //TraceUtil.Log("不能识别的主消息:" + package.GetMasterMsgType());
                    break;
            }
        }
        catch (Exception e)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,e.ToString());
        }
    }

    #region 接收到网络消息处理
    public CommandCallbackType ReceiveEnterCode(byte[] dataBuffer, int socketId)
    {
        SSEnterCodeContext sSEnterCodeContext = SSEnterCodeContext.ParsePackage(dataBuffer);
        //string ucSid = "sst1gamef3307f3c766140a7ace73e809d570c3c111492";
        //SubmitAccountInfo(ucSid, ucSid.Substring(ucSid.Length - 12, 12));
        //LoginPlatformManager.Instance.PlayerId.ToString();
        RaiseEvent(EventTypeEnum.S_CEnterCode.ToString(), sSEnterCodeContext);

        //#if (UNITY_ANDROID && !UNITY_EDITOR)

        //#if ANDROID_UC
        //        string ucSid = LoginPlatformManager.Instance.PlayerId.ToString();
        //        SubmitAccountInfo(ucSid, ucSid.Substring(ucSid.Length - 6, 6));  //平台保留用户ID的最后12位。

        //#elif ANDROID_JIUYAO

        //#elif ANDROID_XIAOMI

        //#else
        //        RaiseEvent(EventTypeEnum.S_CEnterCode.ToString(), sSEnterCodeContext);
        //#endif


        //#else
        //        RaiseEvent(EventTypeEnum.S_CEnterCode.ToString(), sSEnterCodeContext);
        //#endif
        return CommandCallbackType.Continue;
    }
    public CommandCallbackType NewReceiveLoginRes(byte[] dataBuffer, int socketId)
    {
        NewSSUserLoginRes sSUserLoginRes = NewSSUserLoginRes.ParsePackage(dataBuffer);
        LoginManager.Instance.NewSSUserLoginRes = sSUserLoginRes;
        //LoginPlatformManager.Instance.NewSSUserLoginRes = sSUserLoginRes;

        if (m_isAutoLogin)
        {
            if (sSUserLoginRes.SSActorInfos.Count() > 0)
            {
                LoginManager.Instance.LoginSSActorInfo = sSUserLoginRes.SSActorInfos[0];
                //LoginPlatformManager.Instance.LoginSSActorInfo = sSUserLoginRes.SSActorInfos[0];

                NewJumpToGamtway();
            }
            else
            {
                UI.MessageBox.Instance.Show(3, "", "没有角色信息", "确定", null);
            }
        }
        else
        {
            RaiseEvent(EventTypeEnum.S_CUserLoginRes.ToString(), sSUserLoginRes);
        }

        return CommandCallbackType.Continue;
    }
    #endregion
    #region 发送消息处理
    //发送账号信息
    public void SubmitAccountInfo(string accountNo)
    {
        //SSUserLoginContext ssUserLoginContext = new SSUserLoginContext();
        //ssUserLoginContext.SZUserName = accountNo; 
        //ssUserLoginContext.PlatformID = 1;
        //ssUserLoginContext.SZPlatFormKey = "JHGameServer1";
        //ssUserLoginContext.byCM = 0;
        //ssUserLoginContext.wServerID = 1;

        //this.Request(ssUserLoginContext.GeneratePackage(MasterMsgType.NET_ROOT_LOGIN,RootDefineManager.ROOTLOGIN_CS_MAIN_USERLOGIN));
    }
    public void SubmitAccountInfo(string accountNo, string pwd, int versionNo)
    {
        if (GameManager.Instance.EnableHeart && !HeartFPSManager.Instance.StartHeart)
        {
            HeartFPSManager.Instance.StartHeart = true;
        }
        NewSSUserLoginContext ssUserLoginContext = new NewSSUserLoginContext();

        ssUserLoginContext.SZUserName = accountNo;

        ssUserLoginContext.PlatformID = 1;
        ssUserLoginContext.SZPlatFormKey = pwd;
        ssUserLoginContext.byCM = 0;
        ssUserLoginContext.wWorldID = 1;
		ssUserLoginContext.nVersion = versionNo;

        //LoginPlatformManager.Instance.GetString = "提交帐户信息：" + UCGameSdk.getSid();
		this.Request(ssUserLoginContext.GeneratePackage(MasterMsgType.NET_ROOT_LOGIN, RootDefineManager.ROOTLOGIN_CS_MAIN_USERLOGIN));
    }
    public void SubmitAccountInfo(string accountNo, long sessionId, int versionNo)
    {
        SubmitAccountInfo(accountNo.ToString(), sessionId.ToString(), versionNo);
    }
    public void SubmitAccountInfo(long accountNo, long sessionId, int versionNo)
    {
        SubmitAccountInfo(accountNo.ToString(), sessionId, versionNo);
    }
    //发送角色信息
    public void SubmitCharacterInfo(string roleName, byte kind)
    {
        SSActorCreateContext ssActorCreateContext = new SSActorCreateContext();
        ssActorCreateContext.SZName = roleName;
        ssActorCreateContext.byKind = kind;


        this.Request(ssActorCreateContext.GeneratePackage(MasterMsgType.NET_ROOT_SELECTACTOR, RootDefineManager.ROOTSELECTACTOR_CS_MAIN_CREATEACTOR));
    }

    public void NewJumpToGamtway()
    {
        SLoginUserJumpStartContext sLoginUserJumpStartContext = new SLoginUserJumpStartContext();
        sLoginUserJumpStartContext.lActorID = LoginManager.Instance.LoginSSActorInfo.lActorID;
        sLoginUserJumpStartContext.lUserID = LoginManager.Instance.NewSSUserLoginRes.lUserID;
        sLoginUserJumpStartContext.SZRandBuf = "ABC";

        this.Request(sLoginUserJumpStartContext.GeneratePackage(MasterMsgType.NET_ROOT_SELECTACTOR, RootDefineManager.ROOTSELECTACTOR_CS_MAIN_JUMP_START));
    }

    public void NewSendMacToGateway()
    {
        CS_SELECTACTOR_MAC_HEAD cS_SELECTACTOR_MAC_HEAD = new CS_SELECTACTOR_MAC_HEAD();
        cS_SELECTACTOR_MAC_HEAD.m_dwUserID = LoginManager.Instance.NewSSUserLoginRes.lUserID;
        cS_SELECTACTOR_MAC_HEAD.m_dwActorID = LoginManager.Instance.LoginSSActorInfo.lActorID;
        cS_SELECTACTOR_MAC_HEAD.m_byServerID = LoginManager.Instance.LoginSSActorInfo.byServerID;
        cS_SELECTACTOR_MAC_HEAD.m_MapID = LoginManager.Instance.LoginSSActorInfo.lMapID;
        cS_SELECTACTOR_MAC_HEAD.m_byBackServerID = LoginManager.Instance.LoginSSActorInfo.byBackServerID;
        cS_SELECTACTOR_MAC_HEAD.m_BakMapID = LoginManager.Instance.LoginSSActorInfo.lBackMapID;

        cS_SELECTACTOR_MAC_HEAD.m_LinkType = (byte)GameManager.InternetReachability;
        cS_SELECTACTOR_MAC_HEAD.m_OPType =(byte)( GameManager.JoyStickMode ? 1 : 0);
        cS_SELECTACTOR_MAC_HEAD.m_ViewType = (byte)GameManager.GameViewLevel;

        
        this.Request(cS_SELECTACTOR_MAC_HEAD.GeneratePackage(MasterMsgType.NET_ROOT_SELECTACTOR, RootDefineManager.ROOTSELECTACTOR_CS_MAIN_MAC));
    }
    public void SendDeleteActorMsg(int actorId)
    {
        SSActorDeleteContext sSActorDeleteContext = new SSActorDeleteContext();
        sSActorDeleteContext.lActorID = actorId;
        sSActorDeleteContext.szKeyMD5 = new byte[41];


        this.Request(sSActorDeleteContext.GeneratePackage(MasterMsgType.NET_ROOT_SELECTACTOR, RootDefineManager.ROOTSELECTACTOR_CS_MAIN_DELETEACTOR));
    }
    #endregion

    public void AutoLogin()
    {
        this.m_isAutoLogin = true;
        IpManager.InitServiceConfig(m_ip, 8000);

        ConnectToServer();
    }
}
