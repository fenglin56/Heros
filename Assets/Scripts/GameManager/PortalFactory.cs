using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// EnterPoint Scene GameManager
/// </summary>
public class PortalFactory : MonoBehaviour {

    private bool m_createPortal;  //是否立即创建传送门
    private List<IEntityDataStruct> m_preCreatePortalStructCache;

    private Dictionary<string, GameObject> m_goCache;
    public GameObject[] ChannelList;   

    void Awake()
    {
        m_createPortal = false;
        m_preCreatePortalStructCache = new List<IEntityDataStruct>();
        
        //this.PortalFactoryEntityList = new EntityModel[10];

        m_goCache = new Dictionary<string, GameObject>();
        if (ChannelList != null && ChannelList.Length > 0)
        {
            for (int i = 0; i < ChannelList.Length; ++i)
            {
                string key = ChannelList[i].name;
                if (!m_goCache.ContainsKey(key))
                {
                    m_goCache.Add(key, ChannelList[i]);
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
        if (!GameManager.Instance.CreateEntityIM)
        {
            m_preCreatePortalStructCache.Add(entityDataStruct);
        }
        else
        {
            CreatePortal(entityDataStruct);
        }
    }
    /// <summary>
    /// 当发生实体删除时，需要检查缓存里是否有未创建的实体数据，一并删除
    /// </summary>
    /// <param name="uid"></param>
    public void UnRegister(long uid)
    {
        m_preCreatePortalStructCache.RemoveAll(P => P.SMsg_Header.uidEntity == uid);
    }
    public void CreatePortalGameObject()
    {
        this.m_createPortal = true;
        foreach (var dataStruct in this.m_preCreatePortalStructCache)
        {
            CreatePortal(dataStruct);
        }
        this.m_preCreatePortalStructCache.Clear();
    }  
   
    private void CreatePortal(IEntityDataStruct entityDataStruct)
    {
        var sMsgPropCreateEntity_SC_Channel = (SMsgPropCreateEntity_SC_Channel)entityDataStruct;
        string portalName = "chuansongdian";
        var portalPrefab = this.FindByName(portalName);
        var pos = Vector3.zero;

        TraceUtil.Log("传送门的生成UID" + sMsgPropCreateEntity_SC_Channel.SMsg_Header.uidEntity + " -- responseOn:" + GameManager.Instance.ResponseHandlerOn.ToString());

        sMsgPropCreateEntity_SC_Channel.ChannelValue.MAST_FIELD_VISIBLE_TYPE = 
            EctypeConfigManager.Instance.PortalConfigList[sMsgPropCreateEntity_SC_Channel.BaseValue.OBJECT_FIELD_ENTRY_ID]._portalType;
		
        pos = pos.GetFromServer(sMsgPropCreateEntity_SC_Channel.ChannelX, sMsgPropCreateEntity_SC_Channel.ChannelY);

        var portal = (GameObject)GameObject.Instantiate(portalPrefab, pos, Quaternion.identity);
        var portalBehaviour = portal.GetComponent<PortalBehaviour>();

        //加入箭头挂载脚本
        if (ArrowManager.Instance == null)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"[ArrowManager.Instance == null]");
        }
        else
        {
            ArrowManager.Instance.AddChunnelArrow(portal);
        }        

        portalBehaviour.PortalDataModel = sMsgPropCreateEntity_SC_Channel;

        bool Active = sMsgPropCreateEntity_SC_Channel.ChannelValue.MAST_FIELD_VISIBLE_STATE == 0 ? true : false;
        if (portal.activeSelf!= Active)
        {
            portal.SetActive(Active);
        }

       

        EntityModel portalDataModel = new EntityModel();
        portalDataModel.GO = portal;
        portalDataModel.Behaviour = portalBehaviour;
		portalDataModel.EntityDataStruct = sMsgPropCreateEntity_SC_Channel;

        PortalManager.GetInstance();
        EntityController.Instance.RegisteEntity(entityDataStruct.SMsg_Header.uidEntity, portalDataModel);

        //if (!AddPortalObjToList(portalDataModel)) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"传送门列表已满：PortalFactory.PortalFactoryObjcetList"); }
    }

    //bool AddPortalObjToList(EntityModel entityModel)
    //{
    //    for (int i = 0; i < PortalFactoryEntityList.Length; i++)
    //    {
    //        if (PortalFactoryEntityList[i] == null)
    //        {
    //            PortalFactoryEntityList[i] = entityModel;
    //            return true;
    //        }
    //    }
    //    return false;
    //}
}
