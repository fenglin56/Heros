using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI{

	public class SigleUpgradeItem : SinglePackItemSlot {

		public UISprite EquiptItemIcon;

		public void Init(ItemFielInfo itemFileInfo,ButtonCallBack clickCallBack)
		{
            base.Init(itemFileInfo,false,ItemStatus.Sell,clickCallBack);
            bool isEquipt = ContainerInfomanager.Instance.IsItemEquipped(itemFileInfo);
			EquiptItemIcon.gameObject.SetActive(isEquipt);

            //引导注入
            gameObject.RegisterBtnMappingId(itemFileInfo.LocalItemData._goodID, UIType.Package, BtnMapId_Sub.Package_EquipmentUpgradePanel_UpgradeItem);
		}
	}
}