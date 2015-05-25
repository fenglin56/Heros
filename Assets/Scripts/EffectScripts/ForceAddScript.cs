using UnityEngine;
using System.Collections;

public class ForceAddScript :MonoBehaviour{
    public UILabel ForceLable;
    public TweenAlpha LableShow_tween;

    public void ShowEffect( int force)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_TownMain_Up");
        gameObject.SetActive(false);
      
        ForceLable.SetText(force);
        gameObject.SetActive(true);
        LableShow_tween.enabled=true;
        LableShow_tween.Reset();
        LableShow_tween.Play(true);
    }

}
