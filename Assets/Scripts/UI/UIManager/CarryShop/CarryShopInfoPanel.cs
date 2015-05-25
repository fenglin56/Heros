using UnityEngine;
using System.Collections;

public class CarryShopInfoPanel : MonoBehaviour {
	public UILabel labelInfo;
	// Use this for initialization
	bool isRead = false;
	void Init()
	{
		if (isRead)
			return;
		isRead = true;
		labelInfo.text = LanguageTextManager.GetString ("IDS_I29_8").Replace ("\\n","\n");
	}
	public void Show () {
		Init ();
	}
}
