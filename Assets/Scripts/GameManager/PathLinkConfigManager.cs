using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathLinkConfigManager : MonoBehaviour {
    private static PathLinkConfigManager m_instance;
    public static PathLinkConfigManager Instance { get { return m_instance; } }
    
    public LinkConfigDaraBase linkConfigDataBase;
    
    private Dictionary<string,LinkConfigItemData> linkConfigDataDictionaty = new Dictionary<string,LinkConfigItemData>();
    
    void Awake()
    {
        m_instance = this;
        InitLinkData();
    }
    
    void OnDestroy()
    {
        m_instance = null;
    }
    
    void InitLinkData()
    {
        foreach (var Child in linkConfigDataBase.LinkConfigItemList)
        {
            linkConfigDataDictionaty.Add(Child.LinkID,Child);
        }
    }
    
    public LinkConfigItemData GetLinkConfigItem(string LinkID)
    {
        LinkConfigItemData ConfigData = null;
        if (!linkConfigDataDictionaty.TryGetValue(LinkID, out ConfigData))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Link列表无法找到LinkID:"+LinkID);
        }
        return ConfigData;
    }

}
