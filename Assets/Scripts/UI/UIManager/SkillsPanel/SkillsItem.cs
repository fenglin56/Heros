using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;
using System.Linq;
using System;

public class SkillsItem : MonoBehaviour
{
	public SingleSkillInfo ItemFielInfo { get; private set; }

	public GameObject Normal;
	public GameObject Empty;
	public GameObject UnLock;
	public GameObject FullLev;
	public GameObject NotFullLev;
	//Normal Info
	public UILabel BreakLev;
	public UILabel SkillLev;
	public Transform SkillIconPoint;
	public UISprite FocusEff;
	public UILabel EuipPos;
	//UnLock Info
	public UILabel UnLockLev;

	public GameObject SkillUpSuccessEff;
	[HideInInspector]
	public GameObject SelectedEff;  //技能栏待装备循环特效

	private Transform m_cacheTransform; 
	public Action<SkillsItem> OnItemClick;//点击处理委托
	/// <summary>
	///  技能所在的DragUnit在List的UIGrid中的位置百分比。用于代码控制DragablePanel的滚动
	/// </summary>
	[HideInInspector]
	public float DragAmount;

	void Awake () {
		m_cacheTransform=transform;
		//监听点击事件
		GetComponent<UIEventListener>().onClick=(obj)=>
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_SkillChoice");
				if(OnItemClick!=null)
				{
					OnItemClick(this);
				}
		};
	}
    // Use this for initialization
	void Start () {
        
	}
	public void OnBeSelected ()
	{
		OnItemClick(this);
	}
	public void WaitToAssembly(bool flag)
	{
		if(SelectedEff!=null)
		{
			SelectedEff.SetActive(flag);
		}
	}
	/// <summary>
	/// 获得焦点
	/// </summary>
	public void OnGetFocus()
	{
		FocusEff.gameObject.SetActive(true);
	}
	/// <summary>
	/// 失去焦点
	/// </summary>
	public void OnLoseFocus()
	{
		FocusEff.gameObject.SetActive(false);
	}
	/// <summary>
	/// 根据传入数据实体，初始化技能
	/// </summary>
	/// <param name="itemFielInfo">Item fiel info.</param>
	public void InitItemData(SingleSkillInfo itemFielInfo)
	{
		var hasInfo=itemFielInfo!=null;
		Normal.SetActive(hasInfo);
		Empty.SetActive(!hasInfo);
		UnLock.SetActive(hasInfo);
		ItemFielInfo = itemFielInfo;

		if (SkillIconPoint.childCount > 0)
		{
			SkillIconPoint.ClearChild();
		}
		if(hasInfo)
		{
			if(itemFielInfo.IsUnlock())  
			{
				Normal.SetActive(true);
				UnLock.SetActive(false);
				BreakLev.text=(itemFielInfo.localSkillData.m_breakLevel-1).ToString();
				int skillLev=itemFielInfo.SkillLevel;
				int maxLev=itemFielInfo.localSkillData.m_maxLv;
				bool isFullLev=itemFielInfo.IsFullLev();
				FullLev.SetActive(isFullLev);
				NotFullLev.SetActive(!isFullLev);
				if(!isFullLev)
				{
					SkillLev.text=skillLev.ToString();
				}
				EquipChange();
			}
			else    
			{
				Normal.SetActive(false);
				UnLock.SetActive(true);
				UnLockLev.text=itemFielInfo.localSkillData.m_unlockLevel.ToString();
			}
			var skillIcon =NGUITools.AddChild(SkillIconPoint.gameObject,ItemFielInfo.localSkillData.m_icon);
            skillIcon.transform.localScale = new Vector3(90, 90, 1);
		}
	}
	/// <summary>
	/// 装备信息修改.
	/// </summary>
	public void EquipChange()
	{
		if(ItemFielInfo.BattleIconPosition!=0)
		{
			EuipPos.transform.parent.gameObject.SetActive(true);

			EuipPos.text=string.Format(LanguageTextManager.GetString("IDS_I7_10"),ItemFielInfo.BattleIconPosition);
		}
		else
		{
			EuipPos.transform.parent.gameObject.SetActive(false);
		}
	}
	public void SkillUpgradeSuccess(bool playEff)
	{
        InitItemData(ItemFielInfo);
		if(playEff)
		{
            //var eff = NGUITools.AddChild(gameObject, SkillUpSuccessEff);
			StartCoroutine(PlaySuccessEff());
		}
	}
	private IEnumerator PlaySuccessEff()
	{
        //GameObject eff = GameObject.Instantiate(SkillUpSuccessEff, transform.position, SkillUpSuccessEff.transform.rotation) as GameObject;
		var eff=NGUITools.AddChild(gameObject,SkillUpSuccessEff);
        yield return new WaitForSeconds(1);
        GameObject.Destroy(eff);
	}
	public void InitGuideID(int type)
	{
//		switch(type)
//        {
//            case 1:  //技能界面的已解锁的技能列表ID
//                //TODO GuideBtnManager.Instance.RegGuideButton(gameObject, UIType.SkillMain, SubType.SkillMainItem, out m_guideBtnID[0]);
//                //TODO GuideBtnManager.Instance.RegGuideButton(ViewSkillButton.gameObject, UIType.SkillMain, SubType.SkillMainViewSkill, out m_guideBtnID[1]);
//                break;
//            case 2:  //技能界面已经装配的技能列表
//                //TODO GuideBtnManager.Instance.RegGuideButton(gameObject, UIType.SkillMain, SubType.SkillMainItemed, out m_guideBtnID[0]);
//                //TODO GuideBtnManager.Instance.RegGuideButton(ViewSkillButton.gameObject, UIType.SkillMain, SubType.SkillEmpty, out m_guideBtnID[1]);
//                break;
//            case 3:
//                //TODO GuideBtnManager.Instance.RegGuideButton(gameObject, UIType.SkillMain, SubType.SkillMainTips, out m_guideBtnID[0]);
//                //TODO GuideBtnManager.Instance.RegGuideButton(ViewSkillButton.gameObject, UIType.SkillMain, SubType.SkillMainTips, out m_guideBtnID[1]);
//                break;
//            default:
//                break;
//        }
       
    }
//
//    void OnDestroy()
//    {
//        for (int i = 0; i < m_guideBtnID.Length; i++ )
//        {
//            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
//        }
//    }
//
//
//	
	public void IsUpgradeState(bool bFlag)
    {
//        if (bFlag)
//        {
//            ViewSkillButton.ShowMyself();
//        }
//        else
//        {
//            ViewSkillButton.HideMyself();
//        }
//
//        m_isDrag = bFlag;
//        GetComponent<SkillUIDragBox>().enabled = bFlag;
    }

//    void ShowViewSkillPanel(object obj)
//    {
//
//        var viewSkillPanel = CreatObjectToNGUI.InstantiateObj(ViewSkillPanel, transform.parent.parent.parent.parent).GetComponent<ViewSkillPanel>();
//        viewSkillPanel.InitViewInfo(ItemFielInfo);
//
//    }
//
//   
//
//
//    public Transform GetTransform()
//    {
//        if (m_cacheTransform == null)
//        {
//            m_cacheTransform = transform;
//        }
//        return m_cacheTransform;
//    }
//
//
//    public void InitItemData(SingleSkillInfo itemFielInfo)
//    {
//        ItemFielInfo = itemFielInfo;
//        SkillName.text = LanguageTextManager.GetString(itemFielInfo.localSkillData.m_name) ;
//        SkillLevel.text = itemFielInfo.SkillLevel.ToString();
//        int skillManaValue = SkillValue.GetSkillValue(itemFielInfo.SkillLevel, itemFielInfo.localSkillData.m_manaConsumeParams);
//        SkillProp1.text = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_CurMp,skillManaValue).ToString();
//        int skillHurtValue = SkillValue.GetSkillValue(itemFielInfo.SkillLevel, itemFielInfo.localSkillData.m_mightParams);
//        SkillProp2.text = skillHurtValue.ToString();
//        SetSkillIcon();
//    }
//
//    public void UpdateSkillItem(SingleSkillInfo itemFileInfo)
//    {
//        InitItemData(itemFileInfo);
//        ShowUpgradeEffect();
//    }
//
//    void ShowUpgradeEffect()
//    {
//        if (m_upgradeEffect != null)
//        {
//            DestroyImmediate(m_upgradeEffect);
//        }
//
//        m_upgradeEffect = Instantiate(UpgradeEffect) as GameObject;
//        m_upgradeEffect.transform.parent = this.SkillLevelIcon.transform;
//        m_upgradeEffect.transform.localScale = Vector3.one * 0.5f;
//        m_upgradeEffect.transform.localPosition = new Vector3(0, 0, -2);
//    }
//
//    
//
//    public string NormalStrengthenConsume(ItemFielInfo itemFielInfo, out bool isEnough)
//    {
//        ItemData itemData = itemFielInfo.LocalItemData;
//        EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
//        int normalStrengthenLv = equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL + 1;
//        var equipItemData = ItemDataManager.Instance.GetItemData(itemData._goodID) as EquipmentData;
//        var costParameter = equipItemData._StrengthCost;
//
//        var consumeValue = Mathf.FloorToInt((costParameter[0] * Mathf.Pow(normalStrengthenLv, 2) + costParameter[1] * normalStrengthenLv + costParameter[2]) / costParameter[3]) * costParameter[3];
//        isEnough = consumeValue <= PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
//        return consumeValue.ToString();
//    }



    


//    public void OnDrag(Vector2 delta)
//    {
//        if (!m_isDrag)
//            return;
//
//        int index = GetIndex(this.transform.parent.name);
//        var pager = this.transform.parent.parent.GetComponent<ItemPagerManager>().GetCurPageItem;
//
//        if (pager != null && index < pager.Length)
//        {
//            for (int i = 0; i < pager.Length; i++ )
//            {
//                pager[i].OnLoseFocus();
//            }
//            pager[index].OnGetFocus();
//            pager[index].OnBeSelected();
//        }
//    }
//
//    private int GetIndex(string name)
//    {
//        switch (name)
//        {
//            case "ListItemBG1":
//                return 0;
//            case "ListItemBG2":
//                return 1;
//            case "ListItemBG3":
//                return 2;
//            case "ListItemBG4":
//                return 3;
//            case "ListItemBG5":
//                return 4;
//            default:
//                return 0;
//        }
//    }
   
//    /// <summary>
//    /// 计算普通强化主属性加成
//    /// </summary>
//    /// <param name="itemFielInfo"></param>
//    /// <param name="index"></param>
//    /// <returns></returns>
//    private string NormalStrengthenAddValue(ItemFielInfo itemFielInfo, int index)
//    {
//        ItemData itemData = itemFielInfo.LocalItemData;
//        EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
//        var normalValue = EquipMainProp(itemData, equipmentEntity, index, true, true);
//        var normalAdd = EquipMainProp(itemData, equipmentEntity, index, false, true) - normalValue;
//
//        return normalAdd.ToString();
//    }
//
//    /// <summary>
//    /// 计算普通强化主属性
//    /// </summary>
//    /// <param name="itemFielInfo"></param>
//    /// <param name="index"></param>
//    /// <returns></returns>
//    private string NormalStrengthenNormalValue(ItemFielInfo itemFielInfo, int index)
//    {
//        ItemData itemData = itemFielInfo.LocalItemData;
//        EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
//        var normalValue = EquipMainProp(itemData, equipmentEntity, index, true, true);
//
//        return normalValue.ToString();
//    }
//    private string GetMainProName(ItemFielInfo itemFielInfo, int index)
//    {
//        EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
//        int mainProId;
//        string mainProName = string.Empty;
//        switch (index)
//        {
//            case 0:
//                mainProId = equipmentEntity.EQUIP_FIELD_EFFECTBASE0;
//                var mainEffect1 = ItemDataManager.Instance.EffectDatas._effects.SingleOrDefault(P => P.m_IEquipmentID == mainProId);
//                mainProName = mainEffect1.IDS;  //命中
//                break;
//            case 1:
//                mainProId = equipmentEntity.EQUIP_FIELD_EFFECTBASE1;
//                var mainEffect2 = ItemDataManager.Instance.EffectDatas._effects.SingleOrDefault(P => P.m_IEquipmentID == mainProId);
//                mainProName = mainEffect2.IDS;//闪避
//                break;
//        }
//        return LanguageTextManager.GetString(mainProName);
//    }
//    /// <summary>
//    /// 计算装备主属性值
//    /// </summary>
//    /// <param name="itemFielInfo">装备数据</param>
//    /// <param name="index">装备主属性索引</param>
//    /// <param name="isBefore">是否装备前的值</param>
//    /// <param name="isNormal">是否普通强化</param>
//    /// <returns></returns>
//    private int EquipMainProp(ItemData itemData, EquipmentEntity equipmentEntity, int index, bool isBefore, bool isNormal)
//    {
//        int normalStrengthenLv = equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL;
//        int starStrengthenLv = equipmentEntity.EQUIP_FIELD_START_LEVEL;
//        if (!isBefore)
//        {
//            if (isNormal)
//            {
//                normalStrengthenLv += 1;
//            }
//            else
//            {
//                starStrengthenLv += 1;
//            }
//        }
//
//        var equipItemData = ItemDataManager.Instance.GetItemData(itemData._goodID) as EquipmentData;
//        StrengthParameter strengthParameter = equipItemData._StrengthParameter[index];
//
//        int normalMainProAdd = Mathf.FloorToInt((strengthParameter.P1 * Mathf.Pow(normalStrengthenLv, 2)
//            + strengthParameter.P2 * normalStrengthenLv + strengthParameter.P3) / strengthParameter.P4 * strengthParameter.P4);
//        float startAddPercent = 0.05f * starStrengthenLv;
//
//        int sourceMainProValue = 0;
//        switch (index)
//        {
//            case 0:
//                sourceMainProValue = equipmentEntity.EQUIP_FIELD_EFFECTBASE0_VALUE;
//                break;
//            case 1:
//                sourceMainProValue = equipmentEntity.EQUIP_FIELD_EFFECTBASE1_VALUE;
//                break;
//        }
//        return Mathf.FloorToInt((sourceMainProValue + normalMainProAdd) * (1 + startAddPercent));
//    }
}
