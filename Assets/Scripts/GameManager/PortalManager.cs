using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

public class PortalManager : Controller, IEntityManager,ISingletonLifeCycle
{

    private List<EntityModel> m_portalList = new List<EntityModel>();
    public List<EntityModel> PortalList { get { return m_portalList; } }
    private static PortalManager m_instance;


    public static PortalManager Instance
    {
        get 
        {
            if (m_instance == null)
            {
                m_instance = new PortalManager();
                SingletonManager.Instance.Add(m_instance);
                EntityController.Instance.RegisteManager(TypeID.TYPEID_CHUNNEL, m_instance);
            }
            return m_instance;
        }
    }

    public static PortalManager GetInstance()
    {
        return Instance;
    }

    public void RegisteEntity(EntityModel portalData)
    {        
        if (m_portalList.Exists(P => P.EntityDataStruct.SMsg_Header.uidEntity == portalData.EntityDataStruct.SMsg_Header.uidEntity))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"收到服务器重复创建同ID的实体！");
        }
        else
        {
            m_portalList.Add(portalData);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CreatPortal,portalData);
        }
    }


    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetPortalObjcetStatus);
    }


    public void UnRegisteEntity(long uid)
    {
        var targetEntity = this.m_portalList.SingleOrDefault(P => ((PortalBehaviour)P.Behaviour).PortalDataModel.SMsg_Header.uidEntity == uid);
        if (targetEntity != null)
        {
            m_portalList.Remove(targetEntity);

            targetEntity.DestroyEntity();
        }
        //检查Factory中是否有未创建的缓存数据，如果有，一并清除
        GameManager.Instance.PortalFactory.UnRegister(uid);
    }

    public EntityModel GetEntityMode(long uid)
    {
        foreach (EntityModel child in m_portalList)
        {
            if (child.EntityDataStruct.SMsg_Header.uidEntity == uid)
            {
                return  child;
            }
        }
        return null;
    }

    void ResetPortalObjcetStatus(INotifyArgs inotifyArgs)
    {
        EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
        if (entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_CHUNNEL)
        {
            foreach (EntityModel child in m_portalList)
            {
                if (child != null)
                {
                    SMsgPropCreateEntity_SC_Channel sMsgPropCreateEntity_SC_Channel = (SMsgPropCreateEntity_SC_Channel)child.EntityDataStruct;
                    bool Active = sMsgPropCreateEntity_SC_Channel.ChannelValue.MAST_FIELD_VISIBLE_STATE == 0 ? true : false;
                    if (child.GO.activeSelf != Active)
                    {
                        child.GO.SetActive(Active);
                    }
                }
            }
        }
    }

    public void Instantiate()
    {
    }

    public void LifeOver()
    {
        RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetPortalObjcetStatus);
        m_instance = null;
    }
}
