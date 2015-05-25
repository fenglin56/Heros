using UnityEngine;
using System.Collections;

public class ForceUnit : MonoBehaviour {
    public UISprite ForceSprite;
    public UILabel ForceLabel;
    public void InitData(string springName,string Force)
    {
        ForceSprite.spriteName=springName;
        ForceSprite.transform.localScale=GetSpriteSize(ForceSprite);
        ForceLabel.SetText(Force);
    }
    Vector3 GetSpriteSize(UISprite sprite)
    {
        Rect rect=  sprite.GetAtlasSprite().outer;
        return new Vector3(rect.width,rect.height,1);
    }
}
