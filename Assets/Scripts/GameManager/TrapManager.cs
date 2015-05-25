using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class TrapManager : Controller ,IEntityManager{

    private List<EntityModel> m_trapList = new List<EntityModel>();
    private static TrapManager m_instance;

    public static TrapManager Instance
    {
        get 
        {
            if (m_instance == null)
            {
                m_instance = new TrapManager();
                EntityController.Instance.RegisteManager(TypeID.TYPEID_TRAP, m_instance);
            }
            return m_instance;
        }
    }

    public static TrapManager GetInstance()
    {
        return Instance;
    }

    public void RegisteEntity(EntityModel trapData)
    {
        if (m_trapList.Exists(P => P.EntityDataStruct.SMsg_Header.uidEntity == trapData.EntityDataStruct.SMsg_Header.uidEntity))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"收到服务器重复创建同ID的实体！");
        }
        else
        {
            m_trapList.Add(trapData);
        }
    }


    protected override void RegisterEventHandler()
    {
        
    }


    public void UnRegisteEntity(Int64 uid)
    {
        var targetEntity = this.m_trapList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == uid);
        if (targetEntity != null)
        {
            m_trapList.Remove(targetEntity);

            targetEntity.DestroyEntity();
        }
    }

    public EntityModel GetEntityMode(long uid)
    {
        throw new NotImplementedException();
    }
}
