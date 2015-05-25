using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class SingleTreasureChestsGetItem : MonoBehaviour {

		public UILabel Namelabel;
		public UILabel NumberLabel;
		public Transform IconPos;

		public void Init(EctypeRewardItem ectypeRewardItem)
		{
			ItemData itemData = ItemDataManager.Instance.GetItemData(ectypeRewardItem.ItemID);
			Namelabel.SetText(LanguageTextManager.GetString(itemData._szGoodsName));
			NumberLabel.SetText(string.Format("+{0}",ectypeRewardItem.ItemNum));
			IconPos.ClearChild();
			CreatObjectToNGUI.InstantiateObj(itemData._picPrefab,IconPos);
		}

	}
}