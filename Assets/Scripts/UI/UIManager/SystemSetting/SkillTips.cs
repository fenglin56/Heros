using UnityEngine;
using System.Collections;

public class SkillTips : MonoBehaviour {
    public SingleButtonCallBack Btn;
    public UILabel tipsLable1;
    public UILabel tipsLable2;
    private Vector3 Fpos;
    private Vector3 scale;
	void Awake()
    {
        tipsLable1.SetText(LanguageTextManager.GetString("IDS_I30_1"));
        tipsLable2.SetText(LanguageTextManager.GetString("IDS_I30_2"));
        Btn.SetPressCallBack(OnPress);
        Fpos=transform.localPosition;
        scale=transform.localScale;
    }
    void OnPress(bool isPress)
    {
        Vector3 Tpos=new Vector3(Fpos.x,Fpos.y-10,Fpos.z);
        Vector3 Tscale=0.6f*scale;
        if(isPress)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_System_Pop");
            TweenPosition.Begin(gameObject,0.17f,Fpos,Tpos);
            TweenAlpha.Begin(gameObject,0.17f,0,1,null);
            TweenScale.Begin(gameObject,0.17f,Tscale,scale,null);
        }
        else
        {
            TweenPosition.Begin(gameObject,0.17f,Fpos,Tpos);
            TweenAlpha.Begin(gameObject,0.17f,1,0,null);
            TweenScale.Begin(gameObject,0.17f,scale,Tscale,null);
        }
    }

}
