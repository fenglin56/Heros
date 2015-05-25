using UnityEngine;
using System.Collections;

public class AwardMoneyExpPanel : MonoBehaviour {

    public UISprite uiSprite;
    public UILabel AwardNum;
	// Use this for initialization
    public void InitPanel(string spriteName, int awardNum)
    {
        uiSprite.spriteName = spriteName;
        AwardNum.text = string.Format("+{0}", awardNum);
	}
	
    public void InitPanel(UISprite sprite, int awardNum)
    {
        uiSprite.atlas = sprite.atlas;
        uiSprite.spriteName = sprite.spriteName;
        AwardNum.text = string.Format("+{0}", awardNum);
    }

}
