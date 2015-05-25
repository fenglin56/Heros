using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;
using System.Linq;
using System;
/// <summary>
/// 单个装备子项控制脚本，挂在Prefab_EquipItem预设上
/// </summary>
using System.Collections.Generic;


[RequireComponent(typeof(UIEventListener))]
public class EquipItem : MonoBehaviour,IPagerItem
{
    public Transform EquipIconPoint;
	public UISprite FocusEff;
	public SpriteSwith EquipBg;
	public SpriteSwith EquipStar;
    public UISprite EquipedIcon;
	public GameObject LevelInfo;
	public bool CanStrength { get; private set; }

	public Action<EquipItem> OnItemClick;//点击处理委托

    public ItemFielInfo ItemFielInfo { get; private set; }
    private Transform m_cacheTransform;    
	private UILabel EquipLevel;

	// Use this for initialization
	void Awake () {
		m_cacheTransform=transform;
		EquipLevel=LevelInfo.transform.FindChild("EquipLevel").GetComponent<UILabel>();
		//监听点击事件
		GetComponent<UIEventListener>().onClick=(obj)=>
		{
			if(OnItemClick!=null)
			{
				OnItemClick(this);
			}
		};
	}
	#region IPagerItem implementation
	public void OnBeSelected ()
	{
		OnItemClick(this);
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
	#endregion

    public Transform GetTransform()
    {
        if (m_cacheTransform == null)
        {
            m_cacheTransform = transform;
        }
        return m_cacheTransform;
    }
	/// <summary>
	/// 初始化装备信息
	/// </summary>
	/// <param name="itemFielInfo">Item fiel info.</param>
    public void InitItemData(ItemFielInfo itemFielInfo)
    {
        if (itemFielInfo != null)
        {
            ItemFielInfo = itemFielInfo;
			//装备图标
			SetEquipIcon();      
			//已装备提示
			if (EquipedIcon != null)
            {
                long equipId = itemFielInfo.equipmentEntity.SMsg_Header.uidEntity;
                bool isEquipEd = ContainerInfomanager.Instance.sSyncHeroContainerGoods_SCs.Exists(item => item.uidGoods == equipId);
				EquipedIcon.gameObject.SetActive(isEquipEd);
            }            
			//等级及星级
			var equipLevel=GetItemInfoDetail(this.ItemFielInfo,EquipInfoType.EquipStrenLevel);
			if(equipLevel!="+0")
			{
				LevelInfo.SetActive(true);
				EquipLevel.text=equipLevel;
			}
			else
			{
				LevelInfo.SetActive(false);
			}
			var equipStarIndex=EquipItem.GetItemInfoDetail(this.ItemFielInfo,EquipInfoType.EquipStarColorIndex);
			EquipStar.ChangeSprite(int.Parse(equipStarIndex));

            //引导
           // gameObject.RegisterBtnMappingId(itemFielInfo.LocalItemData._goodID, UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_Item);
        }
        else
        {
			TraceUtil.Log(SystemModel.Rocky,TraceLevel.Error,"装备为空-- EquipItem->InitItemData");
        }
    }  

    
	/// <summary>
	/// 设置装备图标
	/// </summary>
    void SetEquipIcon()
    {
        if (EquipIconPoint.childCount > 0)
        {
            EquipIconPoint.ClearChild();
        }
        //TraceUtil.Log(LanguageTextManager.GetString(this.ItemFielInfo.LocalItemData._szGoodsName) + "," + ItemFielInfo.LocalItemData._goodID);
        GameObject go=this.ItemFielInfo.LocalItemData._picPrefab;
        go.layer=  LayerMask.NameToLayer("PopUp");
        var skillIcon=CreatObjectToNGUI.InstantiateObj(go, EquipIconPoint);
        skillIcon.transform.localScale = new Vector3(90, 90, 1);

        EquipBg.ChangeSprite(this.ItemFielInfo.LocalItemData._ColorLevel+1);
    }

	#region 装备计算代码
	/// <summary>
	/// 计算物品信息
	/// </summary>
	/// <returns>The item info detail.</returns>
	/// <param name="equipInfoType">信息类型</param>
	/// <param name="isNormal">是否普通强化还是星阶提升</param>
	public static string GetItemInfoDetail(ItemFielInfo itemFileInfo,EquipInfoType equipInfoType)
	{
      
		return GetItemInfoDetail(itemFileInfo,equipInfoType,true);
	}

    static string GetEffNameOrValue(EquipmentData equipmentData,int index,bool IsName)
    {
        string Res="";
        string[] Effs =equipmentData._vectEffects.Split('|');
        string[] Eff1 = Effs[0].Split('+');
        string EffName0 = Eff1[0];
        string EffValue0 = Eff1[1];
        string[] Eff2 = Effs[1].Split('+');
        string EffName1 = Eff2[0];
        string EffValue1 = Eff2[1];
        if(index==0)
        {
            if(IsName)
            {
                Res= EffName0;
            }
            else
            {
                Res=EffValue0;
            }
        }
        else if(index==1)
        {
            if(IsName)
            {
                Res= EffName1;
            }
            else
            {
                Res=EffValue1;
            }
        }
        else
        {
            Debug.LogError("index只能是0、1");
        }
        return Res;
    }

	public static string GetItemInfoDetail(ItemFielInfo itemFileInfo,EquipInfoType equipInfoType,bool isNormal)
	{
		string result = string.Empty;
        var EffName0 = GetEffNameOrValue(itemFileInfo.LocalItemData as EquipmentData,0,true);
        var EffName1 = GetEffNameOrValue(itemFileInfo.LocalItemData as EquipmentData,1,true);

		if(itemFileInfo!=null)
		{
			switch (equipInfoType)
			{
			case EquipInfoType.EquipName:
				result = GetEquipName(itemFileInfo);
				break;
			case EquipInfoType.EquipStrenLevel:
                    if(itemFileInfo.sSyncContainerGoods_SC.uidGoods!=0)
                    {
                    result = "+"+PlayerDataManager.Instance.GetEquipmentStrengthLevel((EquiptSlotType)int.Parse( (itemFileInfo.LocalItemData as EquipmentData)._vectEquipLoc)).ToString();
				
                    }
                    else
                    {
                        result="+0";
                    }
                    break;
			case EquipInfoType.EquipStarLevel:
                    if(itemFileInfo.sSyncContainerGoods_SC.uidGoods!=0)
                    {
                    result = PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)int.Parse( (itemFileInfo.LocalItemData as EquipmentData)._vectEquipLoc)).ToString();
				
                    }
                    break;
			case EquipInfoType.EquipStarColorIndex:
				result=GetEquipStarIndex(itemFileInfo).ToString();
				break;
			case EquipInfoType.Prop1IconName:

				if (!string.IsNullOrEmpty( EffName0))
				{
					var effectData = ItemDataManager.Instance.GetEffectData(EffName0);
					if (effectData != null)
					{
						result = effectData.EffectRes;
					}
				}
				break;
			case EquipInfoType.Prop1Name:
                    if (!string.IsNullOrEmpty( EffName0))
				    {
                        var effectData = ItemDataManager.Instance.GetEffectData(EffName0);
                        result =LanguageTextManager.GetString( effectData.IDS);
				    }
				break;
			case EquipInfoType.Prop1Value:
                    if (!string.IsNullOrEmpty( EffName0))
				    {
                        var effectData = ItemDataManager.Instance.GetEffectData(EffName0);
    					result = HeroAttributeScale.GetScaleAttribute(effectData,StrengthenMainValue(itemFileInfo, 0)).ToString(); // string.Format("{0}+{1}", NormalStrengthenNormalValue(ItemFielInfo, 0), NormalStrengthenAddValue(ItemFielInfo, 0));
				    }
				break;
			case EquipInfoType.Prop1Add:
                    if (!string.IsNullOrEmpty( EffName0))
    				{
                        var effectData = ItemDataManager.Instance.GetEffectData(EffName0);
    					result = "+"+HeroAttributeScale.GetScaleAttribute(effectData,(int)StrengthenAddValue(itemFileInfo, 0,isNormal)); // string.Format("{0}+{1}", NormalStrengthenNormalValue(ItemFielInfo, 0), NormalStrengthenAddValue(ItemFielInfo, 0));
    				}
				break;
			case EquipInfoType.Prop1TotalAdd:
                    if (!string.IsNullOrEmpty( EffName0))
    				{
                        var effectData = ItemDataManager.Instance.GetEffectData(EffName0);
    					//总属性-基础属性
    					result = "+"+HeroAttributeScale.GetScaleAttribute(effectData,StrengthenMainValue(itemFileInfo, 0)-int.Parse( GetEffNameOrValue(itemFileInfo.LocalItemData as EquipmentData,0,false))); 
    				}
				break;
			case EquipInfoType.Prop2IconName:
                    if (!string.IsNullOrEmpty( EffName1))
    				{
                        var effectData = ItemDataManager.Instance.GetEffectData(EffName1);
    					if (effectData != null)
    					{
    						result = effectData.EffectRes;
    					}
    				}
				break;
			case EquipInfoType.Prop2Name:
                    if (!string.IsNullOrEmpty( EffName1))
    				{
                        var effectData = ItemDataManager.Instance.GetEffectData(EffName1);
                        result =LanguageTextManager.GetString( effectData.IDS);
    				}
				break;
			case EquipInfoType.Prop2Value:
                    if (!string.IsNullOrEmpty( EffName1))
    				{
                        var effectData = ItemDataManager.Instance.GetEffectData(EffName1);
    					result = HeroAttributeScale.GetScaleAttribute(effectData, StrengthenMainValue(itemFileInfo, 1)).ToString();  
    				}
				break;
			case EquipInfoType.Prop2Add:
                    if (!string.IsNullOrEmpty( EffName1))
    				{
                        var effectData = ItemDataManager.Instance.GetEffectData(EffName1);
    					result = "+" + HeroAttributeScale.GetScaleAttribute(effectData,(int)StrengthenAddValue(itemFileInfo, 1,isNormal)).ToString(); 
    				}
				break;
			case EquipInfoType.Prop2TotalAdd:
                    if (!string.IsNullOrEmpty( EffName1))
    				{
                        var effectData = ItemDataManager.Instance.GetEffectData(EffName1);
    					//总属性-基础属性
                        result = "+"+HeroAttributeScale.GetScaleAttribute(effectData,StrengthenMainValue(itemFileInfo, 1)-int.Parse( GetEffNameOrValue(itemFileInfo.LocalItemData as EquipmentData,1,false))); 
    				}
				break;
			case EquipInfoType.Prop1MainAdd:
				result ="+"+ GetNorMalMainProAdd(itemFileInfo,0).ToString();
				break;
			case EquipInfoType.Prop1StarAdd:
				result ="+"+  GetNormalMainProAddForStar(itemFileInfo,0).ToString();
				break;
			case EquipInfoType.Prop2MainAdd:
				result = "+"+ GetNorMalMainProAdd(itemFileInfo,1).ToString();
				break;
			case EquipInfoType.Prop2StarAdd:
				result = "+"+ GetNormalMainProAddForStar(itemFileInfo,1).ToString();
				break;
			}
		}
		return result;
	}

    public static float GetEquipForce(ItemFielInfo itemfielInfo)
    {
        var effectData = ItemDataManager.Instance.GetEffectData(GetEffNameOrValue(itemfielInfo.LocalItemData as EquipmentData,0,true));
        int value =  HeroAttributeScale.GetScaleAttribute(effectData,StrengthenMainValue(itemfielInfo, 0)); 
        var effectData1 = ItemDataManager.Instance.GetEffectData(GetEffNameOrValue(itemfielInfo.LocalItemData as EquipmentData,1,true));
        int value1 =  HeroAttributeScale.GetScaleAttribute(effectData,StrengthenMainValue(itemfielInfo, 1));
        var force= value*effectData.CombatPara/1000.0f+value1*effectData1.CombatPara/1000.0f;
        return force;
    }

    public static float GetNextLevelEquipForce(ItemFielInfo itemfielInfo,UpgradeType type)
    {
        float force=0;
        if(type==UpgradeType.Strength)
        {
            var effectData = ItemDataManager.Instance.GetEffectData(GetEffNameOrValue(itemfielInfo.LocalItemData as EquipmentData,0,true));
        int value =  HeroAttributeScale.GetScaleAttribute(effectData,nextStrengthLevelValue(itemfielInfo, 0)); 
            var effectData1 = ItemDataManager.Instance.GetEffectData(GetEffNameOrValue(itemfielInfo.LocalItemData as EquipmentData,1,true));
        int value1 =  HeroAttributeScale.GetScaleAttribute(effectData,nextStrengthLevelValue(itemfielInfo, 1));
        force= value*effectData.CombatPara/1000.0f+value1*effectData1.CombatPara/1000.0f;
        }
        else if(type==UpgradeType.StarUp)
        {
            var effectData = ItemDataManager.Instance.GetEffectData(GetEffNameOrValue(itemfielInfo.LocalItemData as EquipmentData,0,true));
            int value =  HeroAttributeScale.GetScaleAttribute(effectData,nextStarUpLevelValue(itemfielInfo, 0)); 
            var effectData1 = ItemDataManager.Instance.GetEffectData(GetEffNameOrValue(itemfielInfo.LocalItemData as EquipmentData,1,true));
            int value1 =  HeroAttributeScale.GetScaleAttribute(effectData,nextStarUpLevelValue(itemfielInfo, 1));
            force= value*effectData.CombatPara/1000.0f+value1*effectData1.CombatPara/1000.0f;
        }
        else
        {
            GetEquipForce(itemfielInfo);
        }
        return force;
    }
	/// <summary>
	///  计算强化消耗
	/// </summary>
	/// <returns>The strengthen consume.</returns>
	/// <param name="itemFielInfo">Item fiel info.</param>
	/// <param name="isEnough">Is enough.</param>
	public static string NormalStrengthenConsume(ItemFielInfo itemFielInfo, out bool isEnough)
	{
		if(itemFielInfo==null)
		{
			isEnough=false;
			return string.Empty;
		}
		ItemData itemData = itemFielInfo.LocalItemData;
		EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
		int normalStrengthenLv = equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL + 1;
		var equipItemData = ItemDataManager.Instance.GetItemData(itemData._goodID) as EquipmentData;
		var costParameter = equipItemData._StrengthCost;
		
		var consumeValue = Mathf.FloorToInt((costParameter[0] * Mathf.Pow(normalStrengthenLv, 2) + costParameter[1] * normalStrengthenLv + costParameter[2]) / costParameter[3]) * costParameter[3];
		isEnough = consumeValue <= PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
		return consumeValue.ToString();
	}
	/// <summary>
	/// 获得装备星阶对应的星星SpriteSwitch的索引
	/// 　　0~10	明珠	　　银色星星  1
	///　　11~20	夏炉	　　绿色星星  2
	///	　　21~30	桐琴	　　蓝色星星  3
	///　　31~40	御盾	　　紫色星星  4
	///　　41~50	玉麟	　　金色星星  5
	///　　51~60	周鼎	　　红色星星  6
	/// </summary>
	public static int GetEquipStarIndex(ItemFielInfo itemFileInfo)
	{
		int index=0;
		if(itemFileInfo!=null)
		{
            int starLevel;
            if(itemFileInfo.sSyncContainerGoods_SC.uidGoods!=0)
            {
            starLevel=PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)int.Parse( (itemFileInfo.LocalItemData as EquipmentData)._vectEquipLoc));
            }
            else
            {
                starLevel=0;
            }
		index=starLevel==0?0:Mathf.CeilToInt(starLevel/10f);
		}
		return index;
	}
	/// <summary>
	/// 装备名称+装备强化等级  颜色与品质相关
	/// </summary>
	/// <returns>The equip name.</returns>
	/// <param name="itemFileInfo">Item file info.</param>
	private static string GetEquipName(ItemFielInfo itemFileInfo)
	{

		if(itemFileInfo==null)
		{
			return string.Empty;
		}
        if(itemFileInfo.sSyncContainerGoods_SC.uidGoods!=0)
        {
        string format =PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)int.Parse( (itemFileInfo.LocalItemData as EquipmentData)._vectEquipLoc)) == 0 ? "{0}" : "{0}({1}{2})";// itemFileInfo.equipmentEntity.EQUIP_FIELD_START_LEVEL == 0 ? "{0}" : "{0}({1}{2})";
		
		//var equipName = string.Format(format, LanguageTextManager.GetString(this.m_itemFielInfo.LocalItemData._szGoodsName),
		//    this.m_itemFielInfo.equipmentEntity.EQUIP_FIELD_START_LEVEL,
		//    LanguageTextManager.GetString("IDS_H1_190"));  //风雨刀（1星）
        int equipLv =PlayerDataManager.Instance.GetEquipmentStrengthLevel((EquiptSlotType)int.Parse( (itemFileInfo.LocalItemData as EquipmentData)._vectEquipLoc));
		
		string equipStr = string.Empty;
		if (equipLv == 0)
		{
			equipStr = "";
		}
		else
		{
			equipStr = "+" + equipLv.ToString();
		}
		
		var equipName = LanguageTextManager.GetString(itemFileInfo.LocalItemData._szGoodsName);
        List< JewelInfo>  jewelInfos=PlayerDataManager.Instance.GetJewelInfo((EquiptSlotType)int.Parse( (itemFileInfo.LocalItemData as EquipmentData)._vectEquipLoc));

        if(jewelInfos[0].JewelID!=0&&jewelInfos[1].JewelID!=0)
        {
            Jewel jewel1=ItemDataManager.Instance.GetItemData(jewelInfos[0].JewelID) as Jewel;
            Jewel jewel2=ItemDataManager.Instance.GetItemData(jewelInfos[1].JewelID) as Jewel;
            if(jewel1.StoneGrop!=0&&jewel1.StoneGrop==jewel2.StoneGrop)
            {
                equipName=LanguageTextManager.GetString(jewel1.StoneGropEquipName)+equipName;
            }
        }
		return equipName.SetColor((TextColorType)itemFileInfo.LocalItemData._ColorLevel) + equipStr.SetColor((TextColorType)itemFileInfo.LocalItemData._ColorLevel);
        }
        else
        {
            return NGUIColor.SetTxtColor(LanguageTextManager.GetString( itemFileInfo.LocalItemData._szGoodsName),(TextColor)itemFileInfo.LocalItemData._ColorLevel);
        }
	}
	/// <summary>
	/// 计算强化主属性加成（普通或升星）
	/// </summary>
	/// <param name="itemFielInfo"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	private static float StrengthenAddValue(ItemFielInfo itemFielInfo, int index,bool isNormal)
	{
		if(itemFielInfo==null)
		{
			return 0;
		}
		ItemData itemData = itemFielInfo.LocalItemData;
		EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
		var normalValue = EquipMainProp(itemData, equipmentEntity, index, true, isNormal);
		float normalAdd = EquipMainProp(itemData, equipmentEntity, index, false, isNormal) - normalValue;

		//2013-6-9 阀值取消
//		int equipLevel = itemFielInfo.equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL+1;
//		var procValue = (itemFielInfo.LocalItemData as EquipmentData)._lThresholdValue;        
		
		return normalAdd;
	}

	/// <summary>
	/// 计算强化主属性
	/// </summary>
	/// <param name="itemFielInfo"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	private static int StrengthenMainValue(ItemFielInfo itemFielInfo, int index)
	{
		if(itemFielInfo==null)
		{
			return 0;
		}
		ItemData itemData = itemFielInfo.LocalItemData;
		EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
		var normalValue = EquipMainProp(itemData, equipmentEntity, index, true, true);
		
		return normalValue;
	}
    private static int nextStrengthLevelValue(ItemFielInfo itemFielInfo, int index)
    {
        if(itemFielInfo==null)
        {
            return 0;
        }
        ItemData itemData = itemFielInfo.LocalItemData;
        EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
        var normalValue = EquipMainProp(itemData, equipmentEntity, index, false, true);
        
        return normalValue;
    }
    private static int nextStarUpLevelValue(ItemFielInfo itemFielInfo, int index)
    {
        if(itemFielInfo==null)
        {
            return 0;
        }
        ItemData itemData = itemFielInfo.LocalItemData;
        EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
        var normalValue = EquipMainProp(itemData, equipmentEntity, index, false, false);
        
        return normalValue;
    }
	/// <summary>
	/// 计算主属性名称
	/// </summary>
	/// <returns>The main pro name.</returns>
	/// <param name="itemFielInfo">Item fiel info.</param>
	/// <param name="index">Index.</param>
	private static string GetMainProName(ItemFielInfo itemFielInfo, int index)
	{
		int mainProId;
		string mainProName = string.Empty;
		if(itemFielInfo!=null)
		{
			EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;

			switch (index)
			{
			case 1:
				mainProId = equipmentEntity.EQUIP_FIELD_EFFECTBASE0;
				var mainEffect1 = ItemDataManager.Instance.EffectDatas._effects.SingleOrDefault(P => P.m_IEquipmentID == mainProId);
				mainProName = mainEffect1.IDS;  //命中
				break;
			case 2:
				mainProId = equipmentEntity.EQUIP_FIELD_EFFECTBASE1;
				var mainEffect2 = ItemDataManager.Instance.EffectDatas._effects.SingleOrDefault(P => P.m_IEquipmentID == mainProId);
				mainProName = mainEffect2.IDS;//闪避
				break;
			}
		}
		return LanguageTextManager.GetString(mainProName);
	}
	/// <summary>
	/// 计算装备主属性值
	/// </summary>
	/// <param name="itemFielInfo">装备数据</param>
	/// <param name="index">装备主属性索引</param>
	/// <param name="isBefore">是否装备前的值</param>
	/// <param name="isNormal">是否普通强化</param>
	/// <returns></returns>
	private static int EquipMainProp(ItemData itemData, EquipmentEntity equipmentEntity, int index, bool isBefore, bool isNormal)
	{
        EquipmentData equipItemData=itemData as EquipmentData;
		if(itemData==null)
		{
			return 0;
		}
        int normalStrengthenLv =0;
        int starStrengthenLv=0;
        if(equipmentEntity.ITEM_FIELD_CONTAINER!=0)
        {
            normalStrengthenLv=PlayerDataManager.Instance.GetEquipmentStrengthLevel((EquiptSlotType) int.Parse(equipItemData._vectEquipLoc));// equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL;
            starStrengthenLv=PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType) int.Parse(equipItemData._vectEquipLoc));// equipmentEntity.EQUIP_FIELD_START_LEVEL;
        }


		if (!isBefore)
		{
			if (isNormal)
			{
				normalStrengthenLv += 1;
			}
			else
			{
				starStrengthenLv += 1;
			}
		}
		
		//var equipItemData = ItemDataManager.Instance.GetItemData(itemData._goodID) as EquipmentData;
		StrengthParameter strengthParameter = equipItemData._StrengthParameter[index];
		StrengthParameter starStrengthParameter = equipItemData._StartStrengthParameter[index];
		int normalMainProAdd =normalStrengthenLv==0||normalStrengthenLv>CommonDefineManager.Instance.CommonDefine.StrengthLimit?0:strengthParameter.Value[normalStrengthenLv-1];

		int normalMainProAddForStar =starStrengthenLv==0||starStrengthenLv>CommonDefineManager.Instance.CommonDefine.StartStrengthLimit?0:starStrengthParameter.Value[starStrengthenLv-1];
		
		//float startAddPercent = 0;  //新版装备强化，不再加星级加成数值  2014-6-9  新版又加上星阶加成，但不再手百分比，计算方法与普通强化一样
		
		int sourceMainProValue = 0;
        sourceMainProValue=int.Parse( GetEffNameOrValue(equipItemData,index,false));
//		switch (index)
//		{
//		case 0:
//			sourceMainProValue = 
//			break;
//		case 1:
//			sourceMainProValue = equipmentEntity.EQUIP_FIELD_EFFECTBASE1_VALUE;
//			break;
//		}
//		var procValue = (itemData as EquipmentData)._lThresholdValue;   //强化阀值在2014-6-9取消
//		
//		normalMainProAdd += Mathf.FloorToInt((Mathf.Max(normalStrengthenLv, procValue) - procValue) * 0.05f * sourceMainProValue);
		
		//TraceUtil.Log("index:" + index + " 基础属性：" + sourceMainProValue + "  isBefore:" + isBefore + " AddValue:" + normalMainProAdd);
		//return Mathf.FloorToInt((sourceMainProValue + normalMainProAdd+normalMainProAddForStar) * (1 + startAddPercent));
		return sourceMainProValue + normalMainProAdd+normalMainProAddForStar;
	}
	/// <summary>
	/// 获取装备普通强化加成
	/// </summary>
	/// <returns>The nor mal main pro add.</returns>
	/// <param name="itemData">Item data.</param>
	/// <param name="index">Index.</param>
	static int GetNorMalMainProAdd(ItemFielInfo itemFielInfo,int index)
	{
		if(itemFielInfo==null)
		{
			return 0;
		}
        int normalStrengthenLv;
        if(itemFielInfo.sSyncContainerGoods_SC.uidGoods!=0)
        {
            normalStrengthenLv=PlayerDataManager.Instance.GetEquipmentStrengthLevel((EquiptSlotType)int.Parse( (itemFielInfo.LocalItemData as EquipmentData)._vectEquipLoc));// itemFielInfo.equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL;
        }
        else
        {
            normalStrengthenLv=0;
        }
        var equipItemData = ItemDataManager.Instance.GetItemData(itemFielInfo.LocalItemData._goodID) as EquipmentData;
		StrengthParameter strengthParameter = equipItemData._StrengthParameter[index];
		int normalMainProAdd =normalStrengthenLv==0||normalStrengthenLv>CommonDefineManager.Instance.CommonDefine.StrengthLimit?0:strengthParameter.Value[normalStrengthenLv-1] ;
		return normalMainProAdd;
	}
	/// <summary>
	/// 获取装备升星强化加成
	/// </summary>
	/// <returns>The normal main pro add for star.</returns>
	static int GetNormalMainProAddForStar(ItemFielInfo itemFielInfo,int index)
	{
		if(itemFielInfo==null)
		{
			return 0;
		}
        int starStrengthenLv;
        if(itemFielInfo.sSyncContainerGoods_SC.uidGoods!=0)
        {
         starStrengthenLv = PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)int.Parse( (itemFielInfo.LocalItemData as EquipmentData)._vectEquipLoc));// itemFielInfo.equipmentEntity.EQUIP_FIELD_START_LEVEL;
        }
        else
        {
            starStrengthenLv=0;
        }
        var equipItemData = ItemDataManager.Instance.GetItemData(itemFielInfo.LocalItemData._goodID) as EquipmentData;
		StrengthParameter starStrengthParameter = equipItemData._StartStrengthParameter[index];
		int normalMainProAddForStar =starStrengthenLv==0||starStrengthenLv>CommonDefineManager.Instance.CommonDefine.StartStrengthLimit?0:starStrengthParameter.Value[starStrengthenLv-1]; 
		return normalMainProAddForStar;
	}

	#endregion

//	#region 旧代码
//	/// <summary>
//	/// 设置装备属性(新的强化案，装备Item不用计算属性)
//	/// </summary>
//	private void SetProp()
//	{
//		if (ItemFielInfo.equipmentEntity.EQUIP_FIELD_EFFECTBASE0 != 0)
//		{
//			var effectData = ItemDataManager.Instance.GetEffectData(ItemFielInfo.equipmentEntity.EQUIP_FIELD_EFFECTBASE0);
//			if (effectData != null)
//			{
//				//EquipProp1.SetText(HeroAttributeScale.GetScaleAttribute(effectData,NormalStrengthenNormalValue(ItemFielInfo, 0)));
//			}
//		}
//		
//		if (ItemFielInfo.equipmentEntity.EQUIP_FIELD_EFFECTBASE1 != 0)
//		{
//			var effectData = ItemDataManager.Instance.GetEffectData(ItemFielInfo.equipmentEntity.EQUIP_FIELD_EFFECTBASE1);
//			if (effectData != null)
//			{
//				//EquipProp2Icon.spriteName = effectData.EffectRes;
//				//EquipProp2.SetText(HeroAttributeScale.GetScaleAttribute(effectData,NormalStrengthenNormalValue(ItemFielInfo, 1)));
//			}
//		}
//	}
//	
//
//	
//
//
//	#endregion

}
