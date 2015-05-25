using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;

public class EmailDataManager :ISingletonLifeCycle
{
    #region 单例基础
    private static EmailDataManager m_instance;

    public static EmailDataManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EmailDataManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }

    private EmailDataManager()
    {
        EamilList = new List<SEmailSendUint>();
    }

    public void Instantiate()
    {
        
    }
    
    public void LifeOver()
    {
        m_instance = null;
    }
    #endregion

    #region 变量
    /// <summary>
    ///当前邮件类型 
    /// </summary>
    public EmailType CurrentEamilType;
    /// <summary>
    /// The current main status.
    /// </summary>
    public EmailPageStatus CurrentMainStatus;
    /// <summary>
    /// The current sub statue.
    /// </summary>
    public EmaiSubPageStatus CurrentSubStatue;

    /// <summary>
    /// d登录时获取的邮件状态
    /// </summary>
    /// <value>The type of the email state.</value>
    public emEMAIL_STATE_TYPE EmailStateType{ get; private set; }

    /// <summary>
    ///进入邮件列表时获取的数据，包含了当前邮件列表
    /// </summary>
    /// <value>The email open U i_ S.</value>
    private List<SEmailSendUint> EamilList{ get; set; }

    //private List<SEmailSendUint> UserEamilList{get;set;}
    /// <summary>
    ///当前选中的好友ID 
    /// </summary>
    public uint CurrentFriendId;

    /// <summary>
    /// The end time list.
    /// </summary>
    public List<EmailEndTime> EndTimeList = new List<EmailEndTime>();
    /// <summary>
    ///正在阅读的邮件 
    /// </summary>
    /// <value>The email read.</value>
    public SEmailRead_SC EmailRead{ get; set; }

    public bool HasRequestEmail = false;
    #endregion

    #region 更新本地数据
    public void SetEmailList(SEmailOpenUI_SC _EmailOpenUI_SC)
    {
        // EmailOpenUI_SC=_EmailOpenUI_SC;
        EamilList.AddRange(_EmailOpenUI_SC.mailList);
        //EndTimeList.Clear();
        _EmailOpenUI_SC.mailList.ApplyAllItem(p => {
            EndTimeList.Add(new EmailEndTime(){MailID=p.llMailID,ExpireTime=p.dwExpireTime,UpdateTime=Time.realtimeSinceStartup});});
    }

    public EmailEndTime GetEmailEndTime(long mailID)
    {
        return EndTimeList.First(p => p.MailID == mailID);
    }
    /// <summary>
    /// Deletes the email from local list.
    /// </summary>
    /// <param name="Ids">Identifiers.</param>
    public void DeleteEmailFromLocalList(List<long> Ids)
    {
        for (int i=0; i<Ids.Count; i++)
        {
            EamilList.RemoveAll(p => p.llMailID == Ids [i]);
        }
        // UIEventManager.Instance.TriggerUIEvent(UIEventType.UpdatedEmailList,null);
        ChangeMainMailButton();
    }
    /// <summary>
    /// Updates the current email list_ read.
    /// </summary>
    /// <param name="id">Identifier.</param>
    public void UpdateCurrentEmailList_Read(long id)
    {
        
        for (int i=0; i<EamilList.Count; i++)
        {
            if (EamilList [i].llMailID == id)
            {
                SEmailSendUint email = EamilList [i];
                email.byState = (byte)1;
                EamilList [i] = email;
                //  EamilList[i].byState=(byte)1;
            }
        }
        ChangeMainMailButton();
        
        
    }

    public void UpdateCurrentEmailAttachmentStatus(List<long> Ids)
    {
        for (int i=0; i<Ids.Count; i++)
        {
            
            for (int j=0; j<EamilList.Count; j++)
            {
                if (EamilList [j].llMailID == Ids [i])
                {
                    SEmailSendUint email = EamilList [j];
                    email.byGoodsType = 0;
                    EamilList [j] = email;
                    //EamilList[j].byGoodsType=0;
                }
            }
            
        }
    }
    #endregion

    #region 获取本地数据
    public int GetUnReadCount(EmailType type)
    {
        int count = 0;
        if (EamilList != null)
        {
            foreach (var item in EamilList)
            {
                if (item.byIsSystem == (byte)type && item.byState == (byte)0)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public List<SEmailSendUint> GetEmailList(EmailType type)
    {
        CurrentEamilType = type;
        if (EamilList != null)
        {
            return EamilList.Where(c => System.Convert.ToByte(type) == c.byIsSystem).ToList();
        } else
        {
            return new List<SEmailSendUint>();
        }
    }

    public List<SEmailSendUint> GetEmailList()
    {
        
        if (EamilList != null)
        {
            return EamilList.Where(c => System.Convert.ToByte(CurrentEamilType) == c.byIsSystem).ToList();
        } else
        {
            return new List<SEmailSendUint>();
        }
        
    }

    /// <summary>
    ///h获取到的附件 
    /// </summary>
    /// <returns>The attachment from loacl.</returns>
    public List<Attachment> GetAttachmentFromLoacl(List<long> Ids)
    {
        List<Attachment> AttachmentList = new List<Attachment>();
        Attachment att = new Attachment();
        for (int i=0; i<Ids.Count; i++)
        {
            EamilList.ApplyAllItem(p => {
                if (p.llMailID == Ids [i])
                {
                    att.Type = (emEMAIL_EXTRA_TYPE)p.byGoodsType;
                    att.GoodsID = p.dwGoodsID;
                    att.count = p.dwGoodsNum;
                    AttachmentList.Add(att);
                }   
            });
        }
        return AttachmentList;
    }

    public bool  GetIfCurretEamilHasAttachment()
    {   
        if (GetEamilFromLocal(EmailRead.llEmailID).byGoodsType == 0)
            return false;
        else
            return true;
    }

    public bool  IfHasAttachmentInEmailList()
    {
        bool res = false;
        foreach (var item in EamilList)
        {
            if (item.byGoodsType != 0 && ((EmailType)item.byIsSystem == CurrentEamilType))
            {
                res = true;
                break;
            }
        }       
        return res;
    }

    public SEmailSendUint GetEamilFromLocal(long eamilID)
    {
        if (EamilList != null)
        {
            return EamilList.SingleOrDefault(p => p.llMailID == eamilID);
        } else
        {
            return  new SEmailSendUint();
        }
        
    }
    #endregion

    #region 服务器请求
    public void GetAllEmailsOnService()
    {
        if (!HasRequestEmail)
        {
            HasRequestEmail = true;
            NetServiceManager.Instance.EmailService.SendSEmailOpenUI_CS(new SEmailOpenUI_CS(){dwActorID=(uint)PlayerManager.Instance.FindHeroDataModel().ActorID});
        }
    }

    public void GetEmailAttachment()
    {
        SEmailUpdate_CS data = new SEmailUpdate_CS()
        {
            byUpdateType=System.Convert.ToByte(emEMAIL_UPDATE_TYPE.EMAIL_GETGOODS_UPDATE_TYPE),
            dwActorID=EmailRead.dwActorID,
            dwEmailID=EmailRead.llEmailID,
        };
        NetServiceManager.Instance.EmailService.SendSEmailUpdate_CS(data);
    }

    public void DeleteCurrentEmail()
    {
        // =EmailOpenUI_SC.mailList.Find(c=>c.)
        NetServiceManager.Instance.EmailService.SendSEmailDel_CS(new SEmailDel_CS(){dwActorID=EmailRead.dwActorID,llEmailID=EmailRead.llEmailID,byEmailPage=EmailRead.byEmailPage});
    }
    
    public void GetAllttachment()
    {
        NetServiceManager.Instance.EmailService.SendSEmailGetAllGoods_CS(new SEmailGetAllGoods_CS(){dwActorID=(uint)PlayerManager.Instance.FindHeroDataModel().ActorID});
    }
    
    public void DeleteAllEmail(EmailType type)
    {
        NetServiceManager.Instance.EmailService.SendSEmailDelAll_CS(new SEmailDelAll_CS(){dwActorID=(uint)PlayerManager.Instance.FindHeroDataModel().ActorID,Type=System.Convert.ToByte(type)});
    }
    
    public void ReadEmail(long EmailId, byte Page)
    {
        NetServiceManager.Instance.EmailService.SendSEmailRead_CS(new SEmailRead_CS(){dwActorID=(uint)PlayerManager.Instance.FindHeroDataModel().ActorID,llEmailID=EmailId,byEmailPage=Page});
    }
     #endregion  
    
    #region 主城镇按钮特效
    public void UpdateStateType(byte state)
    {
        EmailStateType = (emEMAIL_STATE_TYPE)state;
        DoForTime.DoFunForTime(1, SendMsgToMailBtn, null);
    }

    void SendMsgToMailBtn(object obj)
    {
        UIEventManager.Instance.TriggerUIEvent(UIEventType.ChangemailBtnEffect, EmailStateType);
    }

    public void ChangeMainMailButton()
    {
        if (GetEmailList(EmailType.systemEmail).Count == CommonDefineManager.Instance.CommonDefine.MailLimit || GetEmailList(EmailType.UserEmail).Count == CommonDefineManager.Instance.CommonDefine.MailLimit)
        {
            // UIEventManager.Instance.TriggerUIEvent(UIEventType.ChangemailBtnEffect,emEMAIL_STATE_TYPE.EMAIL_NONE_STATE_TYPE);
            EmailStateType = emEMAIL_STATE_TYPE.EMAIL_FULL_STATE_TYPE;
            SendMsgToMailBtn(null);
            return;
        } else
        {
            if ((GetUnReadCount(EmailType.systemEmail) > 0) || (GetUnReadCount(EmailType.UserEmail) > 0))
            {
                // UIEventManager.Instance.TriggerUIEvent(UIEventType.ChangemailBtnEffect,emEMAIL_STATE_TYPE.EMAIL_NOREAD_STATE_TYPE);
                EmailStateType = emEMAIL_STATE_TYPE.EMAIL_NOREAD_STATE_TYPE;
                SendMsgToMailBtn(null);
            } else
            {
                //UIEventManager.Instance.TriggerUIEvent(UIEventType.ChangemailBtnEffect,emEMAIL_STATE_TYPE.EMAIL_NONE_STATE_TYPE);
                EmailStateType = emEMAIL_STATE_TYPE.EMAIL_NONE_STATE_TYPE;
                SendMsgToMailBtn(null);
            }
        }
    }
   
    #endregion
}
/// <summary>
/// 邮件附件
/// </summary>
public struct Attachment
{
    //附件类型
    public emEMAIL_EXTRA_TYPE Type;
    /// <summary>
    /// 类型为物品时物品ID
    /// </summary>
    public uint GoodsID;
    //附件数量
    public uint count;
}

public class EmailEndTime
{
    public long MailID;
    public long ExpireTime;
    public float UpdateTime;
}

public enum EmailType
{
    UserEmail,
    systemEmail,
}
