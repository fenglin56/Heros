using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class SingleEctypedropItem : MonoBehaviour {
		public SpriteSwith qualityColor;
		public Transform ItemIconPos;
		public UILabel ItemNameLabel;
		public void Init(ItemData itemData)
		{
			ItemIconPos.ClearChild();
			qualityColor.ChangeSprite (itemData._ColorLevel+1);
			CreatObjectToNGUI.InstantiateObj(itemData._picPrefab,ItemIconPos);
			string nameStr = UI.NGUIColor.SetTxtColor (LanguageTextManager.GetString(itemData._szGoodsName),(UI.TextColor)itemData._ColorLevel);
			ItemNameLabel.SetText(nameStr);           
		}

	}
}