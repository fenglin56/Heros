using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;

public delegate void PageItemSelectedHandler(IPagerItem selectedItem);
public delegate string GetEquipItemInfoHandler(EquipInfoType equipInfoType);

public interface IPagerItem
{
    void OnGetFocus();
    void OnLoseFocus();
    void OnBeSelected();    
    Transform GetTransform();
}

public enum EquipInfoType
{
	/// <summary>
	/// 装备名称
	/// </summary>
    EquipName,
	/// <summary>
	/// 强化等级
	/// </summary>
    EquipStrenLevel,
	/// <summary>
	/// 星阶等级
	/// </summary>
	EquipStarLevel,
	/// <summary>
	/// 不同星阶在不同阶段星星的颜色不同
	/// </summary>
	EquipStarColorIndex,
	/// <summary>
	/// 主属性1名称
	/// </summary>
    Prop1Name,
	/// <summary>
	/// 主属性1总值
	/// </summary>
    Prop1Value,
	/// <summary>
	/// 主属性1总加成值
	/// </summary>
    Prop1TotalAdd,
	/// <summary>
	/// 主属性1升级后获得的加成值
	/// </summary>
	Prop1Add,
	/// <summary>
	/// 主属性1的基础强化加成
	/// </summary>
	Prop1MainAdd,
	/// <summary>
	/// 主属性1的星级强化加成
	/// </summary>
	Prop1StarAdd,
	/// <summary>
	/// 主属性1Icon名称
	/// </summary>
    Prop1IconName,
	/// <summary>
	/// 主属性2名称
	/// </summary>
    Prop2Name,
	/// <summary>
	/// 主属性2总值
	/// </summary>
    Prop2Value,
	/// <summary>
	/// 主属性2总加成值
	/// </summary>
	Prop2TotalAdd,
	/// <summary>
	/// 主属性2升级后获得的加成值
	/// </summary>
	Prop2Add,
	/// <summary>
	/// 主属性2的基础强化加成
	/// </summary>
	Prop2MainAdd,
	/// <summary>
	/// 主属性2的星级强化加成
	/// </summary>
	Prop2StarAdd,
	/// <summary>
	/// 主属性2Icon名称
	/// </summary>
    Prop2IconName,
}
/// <summary>
/// 装备排序实现类，先已装备，次品质，再等级
/// </summary>
public class EquipComparer : IComparer<ItemFielInfo>
{
    public int Compare(ItemFielInfo x, ItemFielInfo y)
    {
        int compareResult = 0;
        bool isXEquiped = Equiped(x);
        bool isYEquiped = Equiped(y);
        if (isXEquiped)
        {
            if (isYEquiped)
            {
                //比较品质，再比较等级
                compareResult = CompareEquipQuality(x, y);
            }
            else
            {
                compareResult = -1;
            }
        }
        else if (isYEquiped)
        {
            compareResult = 1;
        }
        else
        {
            //比较品质，再比较等级
            compareResult = CompareEquipQuality(x, y);
        }

        return compareResult;
    }
    private int CompareEquipQuality(ItemFielInfo x, ItemFielInfo y)
    {
        int qualityCompare = 0;
        if (x.LocalItemData._ColorLevel != y.LocalItemData._ColorLevel)
        {
            qualityCompare = x.LocalItemData._ColorLevel > y.LocalItemData._ColorLevel
                ? -1 : 1;
        }
        else
        {
            qualityCompare = CompareEquipLevel(x, y);
        }

        return qualityCompare;
    }
    private int CompareEquipLevel(ItemFielInfo x, ItemFielInfo y)
    {
        int levelCompare = 0;
        if (x.LocalItemData._AllowLevel != y.LocalItemData._AllowLevel)
        {
            levelCompare = x.LocalItemData._AllowLevel > y.LocalItemData._AllowLevel
                ? -1 : 1;
        }
        else
        {
            levelCompare = CompareEquipID(x, y);
        }
        return levelCompare;
    }
    private int CompareEquipID(ItemFielInfo x, ItemFielInfo y)
    {
        int idCompare = 0;
        if (x.LocalItemData._goodID != y.LocalItemData._goodID)
        {
            idCompare = x.LocalItemData._goodID > y.LocalItemData._goodID
                ? -1 : 1;
        }
        return idCompare;
    }
    public static bool Equiped(ItemFielInfo equipInfo)
    {
        long equipId = equipInfo.equipmentEntity.SMsg_Header.uidEntity;
        bool isEquipEd = ContainerInfomanager.Instance.sSyncHeroContainerGoods_SCs.Exists(item => item.uidGoods == equipId);
        return isEquipEd;
    }
}
