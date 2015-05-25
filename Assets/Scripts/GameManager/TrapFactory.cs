using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// EnterPoint Scene GameManager
/// </summary>
public class TrapFactory : MonoBehaviour {

    private bool m_createTrap;  //是否立即创建陷井
    private List<IEntityDataStruct> m_preCreateTrapStructCache;

    private Dictionary<string, GameObject> m_goCache;
    public GameObject[] TrapObj;

    void Awake()
    {
        m_createTrap = false;
        m_preCreateTrapStructCache = new List<IEntityDataStruct>();

        m_goCache = new Dictionary<string, GameObject>();
        if (TrapObj != null && TrapObj.Length > 0)
        {
            for (int i = 0; i < TrapObj.Length; ++i)
            {
                string key = TrapObj[i].name;
                if (!m_goCache.ContainsKey(key))
                {
                    m_goCache.Add(key, TrapObj[i]);
                }
                else
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"有重复名称，请检查！");
                }
            }
        }
    }

    public GameObject FindByName(string prefabsName)
    {
        if (m_goCache.ContainsKey(prefabsName))
        {
            return m_goCache[prefabsName];
        }

        //TraceUtil.Log("未能找到名为" + prefabsName + "的物件");
        return null;
    }

    public void Register(IEntityDataStruct entityDataStruct)
    {
        if (!this.m_createTrap)
        {
            m_preCreateTrapStructCache.Add(entityDataStruct);
        }
        else
        {
            CreateTrap(entityDataStruct);
        }
    }

    public void CreateTrapGameObject()
    {
        this.m_createTrap = true;
        foreach (var dataStruct in this.m_preCreateTrapStructCache)
        {
            CreateTrap(dataStruct);
        }
        this.m_preCreateTrapStructCache.Clear();
    }

    private void CreateTrap(IEntityDataStruct entityDataStruct)
    {
        SMsgPropCreateEntity_SC_Trap sMsgPropCreateEntity_SC_Trap = (SMsgPropCreateEntity_SC_Trap)entityDataStruct;
        string trapName = "bushoujia";
        var trapPrefab = this.FindByName(trapName);

        //var trapPrefab = EctypeConfigManager.Instance.TrapConfigList[sMsgPropCreateEntity_SC_Trap.BaseValue.OBJECT_FIELD_ENTRY_ID]._TrapPrefab;

        var pos = Vector3.zero;
        pos = pos.GetFromServer(sMsgPropCreateEntity_SC_Trap.TrapX, sMsgPropCreateEntity_SC_Trap.TrapY);
        //var pos = new Vector3(120, 0, -100);
        ////TraceUtil.Log("创建TrapID=====>>>>" + sMsgPropCreateEntity_SC_Trap.SMsg_Header.uidEntity);

        var trap = (GameObject)GameObject.Instantiate(trapPrefab, pos, Quaternion.identity);
        var trapBehaviour = trap.GetComponent<TrapBehaviour>();

        trapBehaviour.TrapDataModel = sMsgPropCreateEntity_SC_Trap;

        EntityModel trapDataModel = new EntityModel();
        trapDataModel.GO = trap;
        trapDataModel.Behaviour = trapBehaviour;
        trapDataModel.EntityDataStruct = sMsgPropCreateEntity_SC_Trap;

        TrapManager.GetInstance();
        EntityController.Instance.RegisteEntity(sMsgPropCreateEntity_SC_Trap.SMsg_Header.uidEntity, trapDataModel);
    }
}
