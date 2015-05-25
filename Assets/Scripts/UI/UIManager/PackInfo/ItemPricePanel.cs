using UnityEngine;
using System.Collections;

public class ItemPricePanel : MonoBehaviour {
    public UILabel Lable_Price;

	public void SetPrice(int price)
    {
        Lable_Price.SetText(LanguageTextManager.GetString("IDS_H1_35")+price);
    }
}
