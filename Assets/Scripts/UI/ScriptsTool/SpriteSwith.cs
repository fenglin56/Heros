using UnityEngine;


[ExecuteInEditMode]
public class SpriteSwith : MonoBehaviour
{
    public int SpriteCount = 0;
    public UISprite target;
    public string[] SpriteArray;

    [ExecuteInEditMode]
    void Start()
    {
        if (target == null)
        {
            target = GetComponent<UISprite>();
        }
    }

    public void ChangeSprite(int SpriteID)//0为什么都没有，最低从1开始
    {
        if (SpriteID == 0)
        {
            this.target.enabled = false;
        }
        else
        {
            if (!this.target.enabled) { this.target.enabled = true; }
			if (SpriteArray != null && SpriteArray.Length > SpriteID - 1)
            {
                this.target.spriteName = SpriteArray[SpriteID - 1];
            }
			else if(SpriteArray.Length <= SpriteID - 1)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"SpriteSwith ChangeSprite 有错误:" + SpriteID);
            }
        }
    }

    public void ChangeSprite(string SpriteName)
    {
        target.spriteName = SpriteName;
    }

    public string getSpritName(int spriteID)
    {
        if (!this.target.enabled) { this.target.enabled = true; }
        return SpriteArray[spriteID];
    }

}