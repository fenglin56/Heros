using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// EnterPoint Scene GameManager
/// </summary>
public class NPCFactory :MonoBehaviour {

    private bool m_createNPC;
    private List<IEntityDataStruct> m_preCreateNPCStructCache = new List<IEntityDataStruct>();
    public GameObject NpcTitle;

    void Awake()
    {
        //m_createNPC = false;
    }

    public void Register(IEntityDataStruct entityDataStruct)
    {
        if (!GameManager.Instance.CreateEntityIM)
        {
            CreateNPC(entityDataStruct, EntityModelPartial.DataStruct);
        } 
        else
        {
            CreateNPC(entityDataStruct, EntityModelPartial.GameObject);
        }
    }
    /// <summary>
    /// 当发生实体删除时，需要检查缓存里是否有未创建的实体数据，一并删除
    /// </summary>
    /// <param name="uid"></param>
    public void UnRegister(long uid)
    {
        m_preCreateNPCStructCache.RemoveAll(P => P.SMsg_Header.uidEntity == uid);
    }
    private void CreateNPC(IEntityDataStruct entityDataStruct, EntityModelPartial entityModelPartial)
    {
        switch (entityModelPartial)
        {
            case EntityModelPartial.DataStruct:
                m_preCreateNPCStructCache.Add(entityDataStruct);
                break;
            case EntityModelPartial.GameObject:
                var sMsgPropCreateEntity_SC_NPC = (SMsgPropCreateEntity_SC_NPC)entityDataStruct;

                ////TraceUtil.Log("============>>>>>>>>>>.MapID" + sMsgPropCreateEntity_SC_NPC.MapID);
                ////TraceUtil.Log("============>>>>>>>>>.NPCX" + sMsgPropCreateEntity_SC_NPC.NPCX);
                ////TraceUtil.Log("============>>>>>>>>>.BaseValue.OBJECT_FIELD_ENTRY_ID" + sMsgPropCreateEntity_SC_NPC.BaseValue.OBJECT_FIELD_ENTRY_ID);
                ////TraceUtil.Log("============>>>>>>>>>.UnitValue.UNIT_FIELD_DIR" + sMsgPropCreateEntity_SC_NPC.UnitValue.UNIT_FIELD_DIR);
                ////TraceUtil.Log("============>>>>>>>>>.UnitValue.UNIT_FIELD_LEVEL" + sMsgPropCreateEntity_SC_NPC.UnitValue.UNIT_FIELD_LEVEL);
                ////TraceUtil.Log("============>>>>>>>>>.NPCValue.CREATURE_FIELD_CAN_TALK" + sMsgPropCreateEntity_SC_NPC.NPCValue.CREATURE_FIELD_CAN_TALK);

                //string npcName = "NPC_QHY";
                //var npcPrefab = this.FindByName(npcName);
                ////TraceUtil.Log("npcID==========" + sMsgPropCreateEntity_SC_NPC.BaseValue.OBJECT_FIELD_ENTRY_ID);
                var npcPrefab = NPCConfigManager.Instance.NPCConfigList[sMsgPropCreateEntity_SC_NPC.BaseValue.OBJECT_FIELD_ENTRY_ID].NPCPrefab;
                var pos = Vector3.zero;
                pos = pos.GetFromServer(sMsgPropCreateEntity_SC_NPC.NPCX, sMsgPropCreateEntity_SC_NPC.NPCY);
                //var pos = new Vector3(100, 0, -100);
                //sMsgPropCreateEntity_SC_NPC.UnitValue.UNIT_FIELD_DIR
                Quaternion npcDir = Quaternion.Euler(0, (float)sMsgPropCreateEntity_SC_NPC.UnitValue.UNIT_FIELD_DIR / 1000f, 0);

                ////TraceUtil.Log("NPC333333333333333333333333NPC===" + sMsgPropCreateEntity_SC_NPC.UnitValue.UNIT_FIELD_DIR);
                var npc = (GameObject)GameObject.Instantiate(npcPrefab, pos, npcDir);

                var npcBehaviour = npc.GetComponent<NPCBehaviour>();

                string npcName = NPCConfigManager.Instance.NPCConfigList[sMsgPropCreateEntity_SC_NPC.BaseValue.OBJECT_FIELD_ENTRY_ID]._szName;
                string szNpcTitle = NPCConfigManager.Instance.NPCConfigList[sMsgPropCreateEntity_SC_NPC.BaseValue.OBJECT_FIELD_ENTRY_ID]._npcTitle;
                var npcTitleGo = (GameObject)GameObject.Instantiate(NpcTitle, Vector3.left * 2000, Quaternion.identity);
                npcTitleGo.transform.parent = BattleManager.Instance.UICamera.transform;
                npcTitleGo.transform.localScale = new Vector3(20, 20, 20);
                npcTitleGo.GetComponent<NPCTitle>().SetNpcTitle(npcName, szNpcTitle, npc.transform.FindChild("NPCTitle").position);

                npcBehaviour.NPCDataModel = sMsgPropCreateEntity_SC_NPC;

                EntityModel npcDataModel = new EntityModel();
                npcDataModel.Behaviour = npcBehaviour;
                npcDataModel.GO = npc;
                npcDataModel.EntityDataStruct = sMsgPropCreateEntity_SC_NPC;

                NPCManager.GetInstance();
                EntityController.Instance.RegisteEntity(entityDataStruct.SMsg_Header.uidEntity, npcDataModel);
                break;
        }

    }

    public void CreateNPCGameObject()
    {
        //this.m_createNPC = true;
        foreach (var dataStruct in this.m_preCreateNPCStructCache)
        {
            CreateNPC(dataStruct, EntityModelPartial.GameObject);
        }

        this.m_preCreateNPCStructCache.Clear();
    }

}
