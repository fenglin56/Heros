using UnityEngine;
using System.Collections;
using System.Linq;
/// <summary>
/// 强化及星阶属性计算 ，挂在Prefab_EquipStrenUpgradProperty上（0，-86，0）
/// </summary>
using UI.MainUI;


public class EquipStrenUpgradeProperty : View {

	[HideInInspector]
	/// <summary>
	/// 当前装备数据实体
	/// </summary>
	public ItemFielInfo CurrItemFielInfo;
	public UILabel Prop1Name;
	public UILabel Prop2Name;
	public UILabel Prop1Value;
	public UILabel Prop2Value;
	public UISprite Prop1IconSprite;
	public UISprite Prop2IconSprite;
	public UILabel Prop1TotalAdd;
	public UILabel Prop2TotalAdd;
	public UILabel Prop1Up;
	public UILabel Prop2Up;

	public GameObject[] NormalOrStarGO;//  Index 0 为普通强化信息框 ，1为星阶升星信息框
	public GameObject[] UpgradeEff;// 属性提升的特效
	//下面是普通强化消耗面板
	public UILabel StrenConsumeValue;
	public UILabel StrenRateValue;
	//下面是升星消耗面板
	public UILabel StarConsumeValue;
	public UILabel StarRateValue;
	public UILabel StarMatName;

    private bool m_enoughToStren;
	private bool m_isNormal;//是否普通强化--名字有些怪，将就着用吧

	void Awake()
	{
        RegisterEventHandler();
		Init(null,true);
	}
    public bool EnoughToStren
    {
        get
        {
            SwitchNormalOtStar(m_isNormal);
            return m_enoughToStren;
        }
    }
	/// <summary>
	/// 初始化属性，
	/// </summary>
	/// <param name="itemFielInfo">装备信息</param>
	/// <param name="isNormal">是否普通强化<c>true</c> is normal.</param>
	public void Init(ItemFielInfo itemFielInfo,bool isNormal)
	{
		m_isNormal=isNormal;
		CurrItemFielInfo=itemFielInfo;
		if(CurrItemFielInfo==null)
		{
			Prop1Name.text=string.Empty;
			Prop2Name.text=string.Empty;
			Prop1Value.text=string.Empty;
			Prop1TotalAdd.text=string.Empty;
			Prop2Value.text=string.Empty;
			Prop2TotalAdd.text=string.Empty;
			Prop1Up.text=string.Empty;
			Prop2Up.text=string.Empty;

			StrenConsumeValue.text=string.Empty;
			StrenRateValue.text=string.Empty;

			StarConsumeValue.text=string.Empty;
			StarRateValue.text=string.Empty;
			StarMatName.text=string.Empty;
			UpgradeEff.ApplyAllItem(P=>P.SetActive(false));
			return;
		}
		UpgradeEff.ApplyAllItem(P=>P.SetActive(true));
		//初始化其他属性（名称，其他属性.....）
		Prop1Name.text=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1Name);
		Prop2Name.text=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2Name);
		Prop1Value.text=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1Value,isNormal);
		Prop1TotalAdd.text=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1TotalAdd,isNormal);
		Prop2Value.text=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2Value,isNormal);
		Prop2TotalAdd.text=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2TotalAdd,isNormal);
		Prop1IconSprite.spriteName=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1IconName);
		Prop2IconSprite.spriteName=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2IconName);

		Prop1Up.text=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1Add);
		Prop2Up.text=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2Add);

		SwitchNormalOtStar(isNormal);
	}
	/// <summary>
	/// Switchs the normal ot star.
	/// </summary>
	/// <param name="isNormal">是否普通强化</param>
	private void SwitchNormalOtStar(bool isNormal)
	{
		NormalOrStarGO[0].SetActive(m_isNormal);
		NormalOrStarGO[1].SetActive(!m_isNormal);

		var equipItemData = ItemDataManager.Instance.GetItemData(CurrItemFielInfo.LocalItemData._goodID) as EquipmentData;

		if(isNormal)
		{
			var strenLev=CurrItemFielInfo.equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL;
            //bool enoughToStren;
            var consume = EquipItem.NormalStrengthenConsume(this.CurrItemFielInfo, out m_enoughToStren);
            StrenConsumeValue.text = "X" + (m_enoughToStren ? consume : consume.SetColor(TextColorType.Red));
            if (strenLev < equipItemData.NormalStrenPercent.Length)
            {
                StrenRateValue.text = equipItemData.NormalStrenPercent[strenLev].ToString();
            }
		}
		else
		{
			var starLev=CurrItemFielInfo.equipmentEntity.EQUIP_FIELD_START_LEVEL;

			var consume=equipItemData._StartStrengthCost[starLev];

			var consumeMat=ItemDataManager.Instance.GetItemData(consume.ItemID_1);
			string consumeValue=consume.Value_1.ToString();
            m_enoughToStren = ContainerInfomanager.Instance.GetItemNumber(consume.ItemID_1) >= consume.Value_1;

            string displayValue = "x" + consumeValue;
            StarConsumeValue.text = (m_enoughToStren ? displayValue : displayValue.SetColor(TextColorType.Red));
            if (starLev < equipItemData.StarUpPercent.Length)
            {
                StarRateValue.text = equipItemData.StarUpPercent[starLev].ToString();  
            }
			
            string matName=LanguageTextManager.GetString(consumeMat._szGoodsName);
            StarMatName.text = (m_enoughToStren ? matName : matName.SetColor(TextColorType.Red));
		}
	}
    private void ResetStrengthConsume(INotifyArgs args)
    {
        SwitchNormalOtStar(m_isNormal);
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStrengthConsume);
    }
    void OnDestroy()
    {
        RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStrengthConsume);
    }
}
