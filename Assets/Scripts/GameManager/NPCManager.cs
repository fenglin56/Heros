using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class NPCManager : Controller, IEntityManager,ISingletonLifeCycle
{

    List<EntityModel> m_npcList = new List<EntityModel>();
    Dictionary<int, Transform> m_npcGuideList = new Dictionary<int, Transform>();
    private static NPCManager m_instance;

    public static NPCManager Instance
    {
        get
        {
            if (null == m_instance)
            {
                m_instance = new NPCManager();
                SingletonManager.Instance.Add(m_instance);
                EntityController.Instance.RegisteManager(TypeID.TYPEID_NPC, m_instance);
            }
            return m_instance;
        }
        
    }
	
    public static NPCManager GetInstance()
    {
        return Instance;
    }

    public void RegisteEntity(EntityModel npcData)
    {
        if (m_npcList.Exists(P => P.EntityDataStruct.SMsg_Header.uidEntity == npcData.EntityDataStruct.SMsg_Header.uidEntity))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"收到服务器重复创建同ID的实体！");
        }
        else
        {
            m_npcList.Add(npcData);

            int entryID = ((SMsgPropCreateEntity_SC_NPC)npcData.EntityDataStruct).BaseValue.OBJECT_FIELD_ENTRY_ID;
            //if (!m_npcGuideList.ContainsKey(entryID))
                //m_npcGuideList.Add(entryID, npcData.GO.transform);
			m_npcGuideList[entryID] = npcData.GO.transform;
        }
    }

    public void UnRegisteEntity(Int64 uidEntity)
    {
        var targetEntity = this.m_npcList.SingleOrDefault(P => ((NPCBehaviour)P.Behaviour).NPCDataModel.SMsg_Header.uidEntity == uidEntity);
        if (targetEntity != null)
        {
            if (targetEntity.Behaviour is ISendInfoToServer)
            {
                GameManager.Instance.TimedSendPackage.RemoveSendInfoObj(targetEntity.Behaviour as ISendInfoToServer);
            }
            m_npcList.Remove(targetEntity);

            targetEntity.DestroyEntity();
        }
        //检查Factory中是否有未创建的缓存数据，如果有，一并清除
        GameManager.Instance.NPCFactory.UnRegister(uidEntity);
    }

    protected override void RegisterEventHandler()
    {
    }

    public EntityModel GetEntityMode(long uid)
    {
        return m_npcList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == uid);
    }

    public Transform GetNpcTransform(int guideID)
    {
        return m_npcGuideList[guideID];
    }

    public void Instantiate()
    {
		
    }

    public void LifeOver()
    {
        m_instance = null;
    }
}
