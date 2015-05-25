using UnityEngine;
using System.Collections;

public class AuctionInfoPanel : MonoBehaviour {
	public UILabel labelInfo;
	// Use this for initialization
	bool isRead = false;
	void Init()
	{
		if (isRead)
			return;
		isRead = true;
		string text = LanguageTextManager.GetString ("IDS_I27_14").Replace ("\\n","\n");
		labelInfo.text = string.Format (text,CommonDefineManager.Instance.CommonDefine.AuctionDefaultTime,CommonDefineManager.Instance.CommonDefine.AuctionTopBid);
	}
	public void Show () {
		Init ();
	}
}
