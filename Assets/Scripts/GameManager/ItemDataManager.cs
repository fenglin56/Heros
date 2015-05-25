using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class ItemDataManager : MonoBehaviour {
	
	
	public ItemDataList[] ItemDatas;

    public ItemDataList EffectDatas;
    public EquipStrengthenDataList EquipStrengthenDataList;

	public PlayerGiftConfigDataBase PlayerGiftConfigDataBase;
	public PlayerTitleConfigDataBase PlayerTitleConfigDataBase;

	private static ItemDataManager m_instance;
    public static ItemDataManager Instance
    {
        get
        {
            return m_instance;
        }
    }
	
	
	private Dictionary<int, ItemData> m_Items = new Dictionary<int, ItemData>();
	// Use this for initialization
	void Awake()
	{
		m_instance = this;
		InitItems();
	}
	
	void Start () {
		
	}

	void InitItems()
	{
   
		foreach(ItemDataList list in ItemDatas)
		{
            //foreach(ItemData data in list._items)
            //{
            //    m_Items.Add(data._goodID, data);	
            //}
            foreach (EquipmentData data in list._equipments)
            {
                m_Items.Add(data._goodID,data);
            }
            foreach (MedicamentData data in list._medicaments)
            {
                m_Items.Add(data._goodID, data);
            }
            foreach (MaterielData data in list._materiels)
            {
                m_Items.Add(data._goodID, data);
            }
			foreach (Jewel data in list._jewel)
			{
				m_Items.Add(data._goodID, data);
			}

           
		}
		foreach(PlayerTitleConfigData data in PlayerTitleConfigDataBase._dataTable)
		{
			m_Items.Add(data._goodID, data);			
		}
	}
	
	public ItemData GetItemData(int itemId)
	{
        ItemData getData = null;
        m_Items.TryGetValue(itemId, out getData);
        //if (getData == null)
        //{
        //    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"NullGetItemID:" + itemId);
        //}
        return getData;	
		
	}

    public EffectData GetEffectData(int EquipmentID)
    {
        foreach (EffectData child in EffectDatas._effects)
        {
            if (child.m_IEquipmentID == EquipmentID)
            {
                return child;
            }
        }
        return null;
    }
    public EffectData GetEffectData(string szName)
    {
        foreach (EffectData child in EffectDatas._effects)
        {
            if (child.m_SzName == szName)
            {
                return child;
            }
        }
        return null;
    }

	public PlayerGiftConfigData GetGiftData(int goodID)
	{
		foreach (PlayerGiftConfigData child in PlayerGiftConfigDataBase._dataTable)
		{
			if (child._goodsID == goodID)
			{
				return child;
			}
		}
		return null;
	}

	// Update is called once per frame
    //void Update () {
	
    //}
}
