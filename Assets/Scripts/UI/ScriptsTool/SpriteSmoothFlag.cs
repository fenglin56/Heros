using UnityEngine;
using System.Collections;

public class SpriteSmoothFlag : MonoBehaviour
{
    float FlagSpeed =1;
    public UISprite TargetSprite;

    private  Color FlagColor1, FlagColor2;
    private int FlagTime;
    private int CurrentFlagTime = 0;
    ButtonCallBack CompleteCallBack;

    bool FlagNow = false;

    public void BeginFlag(int flagTime, Color FlagColor1, Color FlagColor2, ButtonCallBack CompleteCallBack)
    {
        if (FlagNow||!gameObject.active)
            return;
        FlagNow = true;
        this.FlagTime = flagTime * 2;
        this.FlagColor1 = FlagColor1;
        this.FlagColor2 = FlagColor2;
        this.CompleteCallBack = CompleteCallBack;
        CurrentFlagTime = 0;
        TweenColor.Begin(gameObject, FlagSpeed,FlagColor1, FlagColor2);
        StartCoroutine(ChangeColor(FlagColor2));
    }
    /// <summary>
    /// 开始闪动
    /// </summary>
    /// <param name="flagTime">闪动次数</param>
    /// <param name="FlagSpeed">闪动速度,越大越慢</param>
    /// <param name="FlagColor1"></param>
    /// <param name="FlagColor2"></param>
    /// <param name="CompleteCallBack"></param>
    public void BeginFlag(int flagTime, float FlagSpeed, Color FlagColor1, Color FlagColor2, ButtonCallBack CompleteCallBack)
    {
        this.FlagSpeed = FlagSpeed;
        BeginFlag(flagTime, FlagColor1, FlagColor2, CompleteCallBack);
    }

    IEnumerator ChangeColor(Color FlagColor)
    {
        ////TraceUtil.Log("Flag");
        yield return new WaitForSeconds(FlagSpeed);
        CurrentFlagTime++;
        if (CurrentFlagTime < FlagTime || FlagColor == FlagColor2)
        {
            if (FlagColor == FlagColor1)
            {
                TweenColor.Begin(gameObject, FlagSpeed, FlagColor2);
                StartCoroutine(ChangeColor(FlagColor2));
            }
            else
            {
                TweenColor.Begin(gameObject, FlagSpeed, FlagColor1);
                StartCoroutine(ChangeColor(FlagColor1));
            }
        }
        else
        {
            if (CompleteCallBack != null)
            {
                CompleteCallBack(null);
            }
            FlagNow = false;
        }
    }
}
