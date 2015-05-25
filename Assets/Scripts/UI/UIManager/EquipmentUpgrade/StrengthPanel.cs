using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;


public class StrengthPanel : MonoBehaviour {
    public EquipIconItem IconItem;
    public UILabel Lable_position;
    public UILabel Lable_Currentforce;
    public UILabel Lable_addForce;
    public UILabel Des_StrengthLevel;
    public UILabel Lable_strengthLevle;
    public UILabel Lable_name;
	public void Init(ItemFielInfo itemfileInfo)
    {
        IconItem.Init(itemfileInfo);
        Lable_name.SetText(NGUIColor.SetTxtColor(LanguageTextManager.GetString( itemfileInfo.LocalItemData._szGoodsName),(TextColor)itemfileInfo.LocalItemData._ColorLevel));
        Lable_position.SetText(EquipmentUpgradeDataManger.Instance.PositionDic[itemfileInfo.LocalItemData._GoodsSubClass]);
        Lable_Currentforce.SetText((int)EquipItem.GetEquipForce(itemfileInfo));
		int addforce=((int)EquipItem. GetNextLevelEquipForce(itemfileInfo,UpgradeType.Strength)-(int)EquipItem.GetEquipForce(itemfileInfo));
		if(addforce>0)
		{
			Lable_addForce.gameObject.SetActive(true);
        	Lable_addForce.SetText("+"+addforce);
		}
		else
		{
			Lable_addForce.gameObject.SetActive(false);
		}
        Lable_strengthLevle.SetText( EquipmentUpgradeDataManger.Instance.GetStrengthLevel(itemfileInfo));
		TaskGuideBtnRegister ();
    }
	void Awake()
	{
		TaskGuideBtnRegister ();
	}
	private void TaskGuideBtnRegister()
	{
		IconItem.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_Stren_RightIconBtn);
	}
}
