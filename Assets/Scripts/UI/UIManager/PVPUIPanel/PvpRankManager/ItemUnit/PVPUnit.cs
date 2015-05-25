using UnityEngine;
using System.Collections;

public class PVPUnit : MonoBehaviour {
	public UILabel HonourLable;
	public UILabel WinRateLable;
	public UISprite GroupSprite;
	public void ShowUnit(int GroupID,int Honour,int winRate)
	{
		var config=PvpUiPanelManager.Instance.PvpGetGroupConfig(GroupID);
		HonourLable.SetText(Honour);
		WinRateLable.SetText(winRate+"%");
		GroupSprite.spriteName=config.PVPGroupIcon;
	}
}
