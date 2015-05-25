using UnityEngine;
using System.Collections;

public delegate void ChangeValueDelegate(float Number);

public class TweenFloat : MonoBehaviour {


    bool IsAdd = true;

    float FromValue;
    float ToValue;
    float AddNumber;
    ChangeValueDelegate changeValueDelegate;
    ButtonCallBack FinishCallBack;

    public static GameObject Begin( float Duration, float fromValue, float ToValue, ChangeValueDelegate changeValueDelegate)
    {
        if (fromValue == ToValue)
        {
            if (changeValueDelegate != null)
                changeValueDelegate(ToValue);
            return null; 
        }
        GameObject TweenObj = new GameObject();
        TweenObj.name = "TweenFloatObj";
        TweenObj.AddComponent<TweenFloat>();
        TweenFloat tweenFloat = TweenObj.GetComponent<TweenFloat>();
        tweenFloat.AddNumber = (ToValue - fromValue)/ Duration;
        tweenFloat.FromValue = fromValue;
        tweenFloat.ToValue = ToValue;
        tweenFloat.changeValueDelegate = changeValueDelegate;
        if (fromValue < ToValue) { tweenFloat.IsAdd = true; }
        else { tweenFloat.IsAdd = false; }
        return TweenObj;
    }


    public static GameObject Begin(float Duration, float fromValue, float ToValue, ChangeValueDelegate changeValueDelegate,ButtonCallBack FinishCallBack)
    {
        if (Duration == 0)
        {
            fromValue = ToValue;
        }
        if (fromValue == ToValue) 
        {
            if (FinishCallBack != null)
                FinishCallBack(null);
            if (changeValueDelegate != null)
                changeValueDelegate(ToValue);
            return null; 
        }
        GameObject TweenObj = new GameObject();
        TweenObj.AddComponent<TweenFloat>();
        TweenObj.name = "TweenFloatObj";
        TweenFloat tweenFloat = TweenObj.GetComponent<TweenFloat>();
        tweenFloat.AddNumber = (ToValue - fromValue) / Duration;
        tweenFloat.FromValue = fromValue;
        tweenFloat.ToValue = ToValue;
        tweenFloat.changeValueDelegate = changeValueDelegate;
        tweenFloat.FinishCallBack = FinishCallBack;
        if (fromValue < ToValue) { tweenFloat.IsAdd = true; }
        else { tweenFloat.IsAdd = false; }
        return TweenObj;
    }

    public static void Break(ChangeValueDelegate changeValueDelegate)
    { 
    }
    void Update()
    {
        FromValue += AddNumber * Time.deltaTime;
        if (IsAdd)
        {
            if (FromValue < ToValue)
            {
                if (changeValueDelegate != null) { changeValueDelegate(FromValue); }
            }
            else
            {
                FromValue = ToValue;
                if (changeValueDelegate != null) { changeValueDelegate(FromValue); }
                if (FinishCallBack != null) { FinishCallBack(null); }
                Destroy(gameObject); 
            }
        }
        else
        {
            if (FromValue > ToValue)
            {
                if (changeValueDelegate != null) { changeValueDelegate(FromValue); }
            }
            else
            {
                FromValue = ToValue;
                if (changeValueDelegate != null) { changeValueDelegate(FromValue); }
                if (FinishCallBack != null) { FinishCallBack(null); }
                Destroy(gameObject);
            }
        }
    }

}
