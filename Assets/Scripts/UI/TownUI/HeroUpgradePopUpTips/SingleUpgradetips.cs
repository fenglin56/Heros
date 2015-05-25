using UnityEngine;
using System.Collections;

public class SingleUpgradetips : MonoBehaviour {

    public UILabel AddInfoLabel;
    public TweenPosition M_TweenPosition;

    public void Show(EffectData effectData,int AddNumber)
    {
        string AddInfo = string.Format("{0}+{1}",LanguageTextManager.GetString(effectData.IDS),HeroAttributeScale.GetScaleAttribute(effectData,AddNumber));
        AddInfoLabel.SetText(AddInfo);
        MoveUp();
        ScaleBig();
    }

    void MoveUp()
    {
        //M_TweenPosition.method = UITweener.Method.EaseIn;
        Vector3 fromPosition = Vector3.zero;
        Vector3 toPosition = new Vector3(0,150,0);
        TweenPosition.Begin(gameObject,2f,fromPosition,toPosition,null);
    }

    void ScaleBig()
    {
        Vector3 fromScale = transform.localScale;
        Vector3 toScale = transform.localScale * 1.2f;
        TweenScale.Begin(gameObject, 0.2f, fromScale, toScale, ScaleSmall);
        TweenAlpha.Begin(gameObject, 0.1f, 0, 1, null);
    }

    void ScaleSmall(object obj)
    {
        Vector3 fromScale = transform.localScale;
        Vector3 toScale = transform.localScale * 0.8f;
        TweenScale.Begin(gameObject, 1f, fromScale, toScale, null);
        TweenAlpha.Begin(gameObject,1f,1,0,DestroyObj);
    }

    //void MoveDown(object obj)
    //{
    //    M_TweenPosition.method = UITweener.Method.EaseIn;
    //    Vector3 fromPosition = transform.localPosition;
    //    Vector3 toPosition = new Vector3(0, 0, 0);
    //    TweenPosition.Begin(gameObject, 0.3f, fromPosition, toPosition, TweenHideObj);
    //}

    void DestroyObj(object obj)
    {
        Destroy(gameObject);
    }
}
