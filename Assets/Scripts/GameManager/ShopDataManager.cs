using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopDataManager : MonoBehaviour {

    private static ShopDataManager m_instance;
    public static ShopDataManager Instance { get { return m_instance; } }

    public ShopConfigDataBase shopConfigDataBase;

    private Dictionary<int, ShopConfigData> shopConfigDataDictionaty = new Dictionary<int,ShopConfigData>();

    void Awake()
    {
        m_instance = this;
        InitShopData();
    }

    void OnDestroy()
    {
        m_instance = null;
    }

    void InitShopData()
    {
        foreach (var Child in shopConfigDataBase._dataTable)
        {
            shopConfigDataDictionaty.Add(Child._shopGoodsID,Child);
        }
    }

    public ShopConfigData GetShopData(int GoodsID)
    {
        ShopConfigData ConfigData = null;
        if (!shopConfigDataDictionaty.TryGetValue(GoodsID, out ConfigData))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"商店列表无法找到CoodsID:"+GoodsID);
        }
        return ConfigData;
    }

}
