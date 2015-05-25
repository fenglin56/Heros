using UnityEngine;
using System.Collections;
namespace UI.MainUI
{
	public class BattleFailItem : MonoBehaviour {
		public GameObject canUpgrade;
		public UILabel upgradeLabel;
		private bool isRead = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			upgradeLabel.text = LanguageTextManager.GetString ("IDS_I35_2");
		}
		public void Show(EFailType btnType)
		{
			Init ();
			bool isUpgrade = false;
			switch (btnType) {
			case EFailType.EEquipBtn:
				isUpgrade = ContainerInfomanager.Instance.HasEquipmentCanUP();
				break;
			case EFailType.EGemBtn:
				isUpgrade = ContainerInfomanager.Instance.CheckHasJewelCanBeset();
				break;
			case EFailType.ESirenBtn:
				isUpgrade = SirenManager.Instance.IsHasSirenSatisfyIncrease();
				break;
			case EFailType.EEsolericaBtn:
				isUpgrade = SkillModel.Instance.IsOnleAdvanceUpStrengthen();
				break;
			case EFailType.EShopBtn:
				isUpgrade = false;
				break;
			}
			if (isUpgrade) {
				canUpgrade.SetActive (true);		
			} else {
				canUpgrade.SetActive(false);		
			}
		}
	}
}