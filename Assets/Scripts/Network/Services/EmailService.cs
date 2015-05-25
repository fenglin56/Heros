using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmailService : Service 
{

    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_EMAIL:
                switch (package.GetSubMsgType())
                {
                    case EmailDefineManager.MSG_EMAIL_NOREAD_EMAIL:
				AddToInvoker(ReceiveMailLoginState, package.Data, socketId);
                
                
                break;
                    case EmailDefineManager.MSG_EMAIL_OPEN_UI_TYPE:
                        AddToInvoker(ReceiveMailOpenUI, package.Data, socketId);
                        
                        
                        break;
                    case EmailDefineManager.MSG_EMAIL_UPDATE:
				AddToInvoker(ReceiveMailUpdate, package.Data, socketId);
                
                        
                        break;
                    case EmailDefineManager.MSG_EMAIL_SEND:
                        AddToInvoker(ReceiveMailSend,package.Data,socketId);
                        break;
                    case EmailDefineManager.MSG_EAMIL_DEL:
				AddToInvoker(ReceiveMailDel, package.Data, socketId);
                
                        
                        break;
                    case EmailDefineManager.MSG_EMAIL_ALLGOODSGET:
				AddToInvoker(ReceiveMailGetGoodsAll, package.Data, socketId);
                
                        
                        break;
                    case EmailDefineManager.MSG_EMAIL_ALLDEL:
				AddToInvoker(ReceiveMailDelAll, package.Data, socketId);
                
                break;

                    case EmailDefineManager.MSG_EMAIL_READ:
                AddToInvoker(ReceiveMailRead, package.Data, socketId);
                
                        break;
                   
                    default:
                        break;
                }
                break;
            default:
                //TraceUtil.Log("不能识别的主消息" + package.GetMasterMsgType());
                break;
        }
    }

    #region Receive Msg

    //登陆收到邮箱状态
    CommandCallbackType ReceiveMailLoginState(byte[] dataBuffer, int sorketID)
    {
        SLoginEmailState_SC sLoginEmailState_SC = SLoginEmailState_SC.ParsePackage(dataBuffer);
        EmailDataManager.Instance.UpdateStateType(sLoginEmailState_SC.byEmailState);
        return CommandCallbackType.Continue;
    }

    //打开UI接收到邮件列表信息
    CommandCallbackType ReceiveMailOpenUI(byte[] dataBuffer, int sorketID)
    {
        SEmailOpenUI_SC sEmailOpenUI_SC = SEmailOpenUI_SC.ParsePackage(dataBuffer);
        EmailDataManager.Instance.SetEmailList(sEmailOpenUI_SC);
       // UIEventManager.Instance.TriggerUIEvent(UIEventType.UpdatedEmailList,sEmailOpenUI_SC);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.GetEamilList,sEmailOpenUI_SC);
        return CommandCallbackType.Continue;
    }

    //打开UI接收到邮件列表信息
    CommandCallbackType ReceiveMailSend(byte[] dataBuffer, int sorketID)
    {
        SEmailWrite_SC sEmailOpenUI_SC = SEmailWrite_SC.ParsePackage(dataBuffer);
       // EmailDataManager.Instance.EmailOpenUI_SC=sEmailOpenUI_SC;
        UIEventManager.Instance.TriggerUIEvent(UIEventType.SendEamil,sEmailOpenUI_SC);
        return CommandCallbackType.Continue;
    }


    //接收到邮件状态更新
    CommandCallbackType ReceiveMailUpdate(byte[] dataBuffer, int sorketID)
    {
        SEmailUpdate_SC sEmailUpdate_SC = SEmailUpdate_SC.ParsePackage(dataBuffer);
        List<long> EmailIDs=new List<long>();
        EmailIDs.Add(sEmailUpdate_SC.dwEmailID);
        EmailDataManager.Instance.UpdateCurrentEmailAttachmentStatus(EmailIDs);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.GetAttachment,sEmailUpdate_SC);
        return CommandCallbackType.Continue;
    }

    //接收到邮件删除
    CommandCallbackType ReceiveMailDel(byte[] dataBuffer, int sorketID)
    {
        SEmailDel_SC sEmailDel_SC = SEmailDel_SC.ParsePackage(dataBuffer);

        EmailDataManager.Instance.DeleteEmailFromLocalList(sEmailDel_SC.mailIdList);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.DeleteEmail,sEmailDel_SC);
        return CommandCallbackType.Continue;
    }

    //接收到获取所有附件结果
    CommandCallbackType ReceiveMailGetGoodsAll(byte[] dataBuffer, int sorketID)
    {
        SEmailGetAllGoods_SC sEmailGetAllGoods_SC = SEmailGetAllGoods_SC.ParsePackage(dataBuffer);
        EmailDataManager.Instance.UpdateCurrentEmailAttachmentStatus(sEmailGetAllGoods_SC.mailIdList);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.GetAllAttachment,sEmailGetAllGoods_SC);
        return CommandCallbackType.Continue;
    }


    //接收到删除所有附件结果
    CommandCallbackType ReceiveMailDelAll(byte[] dataBuffer, int sorketID)
    {
        SEmailDelAll_SC sEmailDelAll_SC = SEmailDelAll_SC.ParsePackage(dataBuffer);
        EmailDataManager.Instance.DeleteEmailFromLocalList(sEmailDelAll_SC.mailIdList);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.DeleteEmail,sEmailDelAll_SC);
        return CommandCallbackType.Continue;
    }

    //接收到阅读邮件回应
    CommandCallbackType ReceiveMailRead(byte[] dataBuffer, int sorketID)
    {
        SEmailRead_SC sEmailRead_SC = SEmailRead_SC.ParsePackage(dataBuffer);
		EmailDataManager.Instance.EmailRead=sEmailRead_SC;
        EmailDataManager.Instance.UpdateCurrentEmailList_Read(sEmailRead_SC.llEmailID);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.ReadEmail,sEmailRead_SC);
        return CommandCallbackType.Continue;
    }




    #endregion
	




    #region SendMsg
    //打开UI，请求邮件
    public void SendSEmailOpenUI_CS(SEmailOpenUI_CS sEmailOpenUI_CS)
    {
        this.Request(sEmailOpenUI_CS.GeneratePackage());
    }

    //发送邮件
    public void SendSEmailWrite_CS(SEmailWrite_CS sEmailWrite_CS)
    {
        this.Request(sEmailWrite_CS.GeneratePackage());
    }

    //发送获取附件请求
    public void SendSEmailUpdate_CS(SEmailUpdate_CS sEmailUpdate_CS)
    {
        this.Request(sEmailUpdate_CS.GeneratePackage());
    }

    //发送删除邮件
    public void SendSEmailDel_CS(SEmailDel_CS sEmailDel_CS)
    {
        this.Request(sEmailDel_CS.GeneratePackage());
    }

    //发送领取所有物品
    public void SendSEmailGetAllGoods_CS(SEmailGetAllGoods_CS sEmailGetAllGoods_CS)
    {
        this.Request(sEmailGetAllGoods_CS.GeneratePackage());
    }

    //发送删除所有邮件
    public void SendSEmailDelAll_CS(SEmailDelAll_CS sEmailDelAll_CS)
    {
        this.Request(sEmailDelAll_CS.GeneratePackage());
    }

    //发送阅读邮件请求
    public void SendSEmailRead_CS(SEmailRead_CS sEmailRead_CS)
    {
        this.Request(sEmailRead_CS.GeneratePackage());
    }


    #endregion


}
