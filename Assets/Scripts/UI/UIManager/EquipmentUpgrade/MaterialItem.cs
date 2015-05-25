using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;


public class MaterialItem : MonoBehaviour {
    public UILabel Lable_count;
    public UILabel Lable_name;
    public EquipIconItem iconItem;
    public void Init(UpgradeRequire upgradeRequire)
    {
        ItemFielInfo itemfileinfo=new ItemFielInfo(upgradeRequire.GoodsId);
        iconItem.Init(itemfileinfo);
        Lable_name.SetText(NGUIColor.SetTxtColor(LanguageTextManager.GetString(itemfileinfo.LocalItemData._szGoodsName),(TextColor)itemfileinfo.LocalItemData._ColorLevel));
        if(upgradeRequire.GoodsId==3050001)
        {
            if(upgradeRequire.Count<=ContainerInfomanager.Instance.GetOwnMaterialCount(upgradeRequire))
            {
                Lable_count.SetText(NGUIColor.SetTxtColor(upgradeRequire.Count,TextColor.green));
            }
            else
            {
                Lable_count.SetText(NGUIColor.SetTxtColor(upgradeRequire.Count,TextColor.red));
            }
        }
        else
        {
            if(upgradeRequire.Count<=ContainerInfomanager.Instance.GetOwnMaterialCount(upgradeRequire))
            {
                Lable_count.SetText(NGUIColor.SetTxtColor( ContainerInfomanager.Instance.GetOwnMaterialCount(upgradeRequire)+"/"+upgradeRequire.Count,TextColor.green));
            }
            else
            {
                Lable_count.SetText(NGUIColor.SetTxtColor( ContainerInfomanager.Instance.GetOwnMaterialCount(upgradeRequire)+"/"+upgradeRequire.Count,TextColor.red));
            }
        }
    }
}
