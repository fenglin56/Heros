using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class SingleRewardItem_V3 : MonoBehaviour
    {

        public Transform CreatIconPoint;
        public UILabel NameLabel;
        public UILabel NumberLabel;

        public void Init(int itemID,int number)
        {
            ItemData getItem = ItemDataManager.Instance.GetItemData(itemID);
            CreatObjectToNGUI.InstantiateObj(getItem._picPrefab,CreatIconPoint);
            TextColor labelColor = TextColor.white;
            switch (getItem._ColorLevel)
            {
                case 0:
                    labelColor = TextColor.EquipmentGreen;
                    break;
                case 1:
                    labelColor = TextColor.EquipmentBlue;
                    break;
                case 2:
                    labelColor = TextColor.EquipmentMagenta;
                    break;
                case 3:
                    labelColor = TextColor.EquipmentYellow;
                    break;
            }
            NameLabel.SetText(NGUIColor.SetTxtColor(LanguageTextManager.GetString(getItem._szGoodsName),labelColor));
            NumberLabel.SetText(NGUIColor.SetTxtColor(string.Format("+{0}",number), labelColor));
        }
    }
}