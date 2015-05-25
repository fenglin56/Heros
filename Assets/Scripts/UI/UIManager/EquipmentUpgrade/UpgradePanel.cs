using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;


public class UpgradePanel : MonoBehaviour {

    public EquipIconItem IconItem_current;
    public UILabel Lable_CurrentName;
    public EquipIconItem IconItem_nextLv;
    public UILabel Lable_NextLvName;
	void Awake()
	{
		TaskGuideBtnRegister ();
	}
	private void TaskGuideBtnRegister()
	{
		IconItem_current.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_Upgrade_RightIconBtn);
	}
    public void Init(ItemFielInfo itemfileInfo)
    {
        ItemFielInfo NextItem=new ItemFielInfo((itemfileInfo.LocalItemData as EquipmentData).UpgradeID);
        IconItem_current.Init(itemfileInfo);
        Lable_CurrentName.SetText(NGUIColor.SetTxtColor(LanguageTextManager.GetString( itemfileInfo.LocalItemData._szGoodsName),(TextColor)itemfileInfo.LocalItemData._ColorLevel));
        IconItem_nextLv.Init(NextItem);
        Lable_NextLvName.SetText(NGUIColor.SetTxtColor(LanguageTextManager.GetString( NextItem.LocalItemData._szGoodsName),(TextColor)NextItem.LocalItemData._ColorLevel));
    }
}
