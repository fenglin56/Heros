using UnityEngine;
using System.Collections;
using UI.MainUI;

/// <summary>
/// 装备强化详细信息控制。挂在预设件Prefab_EquipDetails上。
/// </summary>
public class EquipDetails : MonoBehaviour {
	
	[HideInInspector]
	/// <summary>
	/// 当前装备数据实体
	/// </summary>
	public ItemFielInfo CurrItemFielInfo;
	public GameObject EquipItemMainPropertyPrefab;  //主属性
	public GameObject EquipStrenUpgradePropertyPrefab;    //强化及星阶属性
	public UILabel EquipedTips;   // 装备未装备提示
	public UILabel EquipedSellPrice;  // 装备售价，（从服务器协议获得）
    public GameObject MainPropertyPoint;
    public GameObject UpgradePropertyPoint;
    [HideInInspector]
	public EquipItemMainProperty m_equipItemMainProperty;
    [HideInInspector]
	public EquipStrenUpgradeProperty m_equipStrenUpgradeProperty;
	private bool m_isNormalStren;  //是否普通强化
	private TweenPosition m_tweenPosComponent;
	void Awake()
	{
        m_equipItemMainProperty = NGUITools.AddChild(MainPropertyPoint, EquipItemMainPropertyPrefab).GetComponent<EquipItemMainProperty>();
		m_equipItemMainProperty.transform.localPosition=EquipItemMainPropertyPrefab.transform.localPosition;
        m_equipStrenUpgradeProperty = NGUITools.AddChild(UpgradePropertyPoint, EquipStrenUpgradePropertyPrefab).GetComponent<EquipStrenUpgradeProperty>();
		m_equipStrenUpgradeProperty.transform.localPosition=EquipStrenUpgradePropertyPrefab.transform.localPosition;

		m_tweenPosComponent=GetComponent<TweenPosition>();
    }
    
	public void Init(ItemFielInfo itemFielInfo,bool isNormal)
	{
		m_isNormalStren=isNormal;
		CurrItemFielInfo=itemFielInfo;
		//装备未装备
		long equipId = itemFielInfo.equipmentEntity.SMsg_Header.uidEntity;
		bool isEquipEd = ContainerInfomanager.Instance.sSyncHeroContainerGoods_SCs.Exists(item => item.uidGoods == equipId);
		if(isEquipEd)
		{
			EquipedTips.text="[fffa6f]" + LanguageTextManager.GetString("IDS_I3_57") + "[-]";
		}
		else
		{
			EquipedTips.text="[ffffff]" +LanguageTextManager.GetString("IDS_I3_58")+"[-]"; 
		}
		//装备售价
		EquipedSellPrice.text=(itemFielInfo.LocalItemData._SaleCost+itemFielInfo.equipmentEntity.ITEM_FIELD_VISIBLE_COMM).ToString();
		m_equipItemMainProperty.Init(itemFielInfo);
		m_equipStrenUpgradeProperty.Init(itemFielInfo,isNormal);
	}
	/// <summary>
	///  打开界面，飞入动画
	/// </summary>
	public void ShowAnim()
	{
		m_tweenPosComponent.Play(true);
	}
	public void CloseAnim()
	{
		m_tweenPosComponent.Play(false);
	}
	public bool IsNormalStren
	{
		set{
			m_isNormalStren=value;
			if(CurrItemFielInfo!=null)
			{
				m_equipStrenUpgradeProperty.Init(CurrItemFielInfo,m_isNormalStren);
			}
		}
	}
}
