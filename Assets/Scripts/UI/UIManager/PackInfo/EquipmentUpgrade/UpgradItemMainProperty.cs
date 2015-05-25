using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class UpgradItemMainProperty : EquipItemMainProperty {

		public GameObject UpgradIcon;
		public UILabel LevelLabel;
		public UILabel AddLevelLabel;


		public new void Init(ItemFielInfo itemFileInfo)
		{
			base .Init(itemFileInfo);
			if(itemFileInfo == null)
			{
				LevelLabel.SetText("");
				AddLevelLabel.SetText("");
				UpgradIcon.gameObject.SetActive(false);
				return;
			}
			EquipmentData itemData = itemFileInfo.LocalItemData as EquipmentData;
			LevelLabel.SetText(itemData._AllowLevel);
			if(itemData.UpgradeID!=0)
			{
				int targetLevel = ItemDataManager.Instance.GetItemData(itemData.UpgradeID)._AllowLevel;
				UpgradIcon.SetActive(true);
				AddLevelLabel.SetText(targetLevel - itemData._AllowLevel);
			}else
			{
				UpgradIcon.SetActive(false);
				AddLevelLabel.SetText("");
			}
		}

	}
}