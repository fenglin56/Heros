using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;


public enum UpgradeType
{
    /// <summary>
    /// 强化
    /// </summary>
    Strength=1,
    /// <summary>
    /// 升星
    /// </summary>
    StarUp,
    /// <summary>
    /// 升级
    /// </summary>
    Upgrade
}
public class EquipmentUpgradeDataManger : ISingletonLifeCycle {
    public readonly string[] PositionDic=new string[]{"物资","武器","时装","头饰","衣服","靴子","饰品","徽章"};
    private static EquipmentUpgradeDataManger instance;
    public static EquipmentUpgradeDataManger Instance
    {
        get
        {
            if(instance==null)
            {
                instance=new EquipmentUpgradeDataManger();
                SingletonManager.Instance.Add(instance);
            }
            return instance;
        }
    }
    private EquipmentUpgradeDataManger()
    {
        CurrentType=UpgradeType.Strength;
    }
    public UpgradeType CurrentType{get;set;}
    public ItemFielInfo CurrentSelectEquip;//升级使用物品，
    public EquiptSlotType CurrentSlotType;//升星强化使用
    public long NewItemUID=0;

 

    public int GetStrengthLevel(ItemFielInfo itemfileInfo)
    {
       return  PlayerDataManager.Instance.GetEquipmentStrengthLevel((EquiptSlotType)itemfileInfo.sSyncContainerGoods_SC.nPlace);
    }
    public int GetStarUpLevel(ItemFielInfo itemfileInfo)
    {
        return PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)itemfileInfo.sSyncContainerGoods_SC.nPlace);
    }
    #region ISingletonLifeCycle implementation
    public void Instantiate()
    {

    }
    public void LifeOver()
    {
        instance=null;
    }
    #endregion


}
