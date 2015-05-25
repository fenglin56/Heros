using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TweenUIAnimation : MonoBehaviour {

    public enum FinishEventCall {Alpha,Position,Scale }

    public FinishEventCall MyFinishCall = FinishEventCall.Alpha;

    public AlphaAnimationInfo[] AlphaAnimationList = new AlphaAnimationInfo[0];
    public PositionAnimationInfo[] PositionAnimationList = new PositionAnimationInfo[0];
    public ScaleAnimationInfo[] ScaleAnimationList = new ScaleAnimationInfo[0];

    TweenAlpha MyTweenAlpha;
    TweenPosition MyTweenPosition;
    TweenRotation MyTweenRotation;
    TweenScale MyTweenScale;

    int PlayAlphaAnimStep = 0;
    int PlayPositionAnimStep = 0;
    int PlayScaleAnimStep = 0;

    public void Play()
    {
        PlayAlphaAnimStep = 0;
        PlayPositionAnimStep = 0;
        PlayScaleAnimStep = 0;
        GetTweenComponent();


        PlayAlphaAnim(null);
        PlayPositionAnim(null);
        PlayScaleAnim(null);
        //PlayPositionAnim(null);
    }

    void PlayAlphaAnim(object obj)
    {
        if (PlayAlphaAnimStep + 1 < AlphaAnimationList.Length)
        {
            var currentAlphaData = AlphaAnimationList[PlayAlphaAnimStep];
            var nextAlphaData = AlphaAnimationList[PlayAlphaAnimStep+1];
            MyTweenAlpha.method = currentAlphaData.Method;
            MyTweenAlpha.from = currentAlphaData.Alpha;
            MyTweenAlpha.duration = currentAlphaData.Time;
            MyTweenAlpha.to = nextAlphaData.Alpha;
            MyTweenAlpha.CallBackWhenFinished = PlayAlphaAnim;
            PlayAlphaAnimStep++;
            UITweener.Begin<TweenAlpha>(gameObject, currentAlphaData.Time);
            //TraceUtil.Log("PlayAlphaAnim:" + PlayAlphaAnimStep + "," + MyTweenAlpha.to);
        }
        else
        {
            if (MyFinishCall == FinishEventCall.Alpha)
            {
                RecoverGameObject();
            }
            //TraceUtil.Log("PlayAlphaAnimComplete");
        }
    }

    void PlayPositionAnim(object obj)
    {
        if (PlayPositionAnimStep + 1 < PositionAnimationList.Length)
        {
            var currentPositionData = PositionAnimationList [PlayPositionAnimStep];
            var nextPositionData = PositionAnimationList[PlayPositionAnimStep + 1];
            MyTweenPosition.method = currentPositionData.Method;
            MyTweenPosition.from = currentPositionData.Position;
            MyTweenPosition.duration = currentPositionData.Time;
            MyTweenPosition.to = nextPositionData.Position;
            MyTweenPosition.CallBackWhenFinished = PlayPositionAnim;
            PlayPositionAnimStep++;
            UITweener.Begin<TweenPosition>(gameObject, currentPositionData.Time);
            //TraceUtil.Log("PlayPositionAnim:" + PlayPositionAnimStep + "," + MyTweenPosition.to);
        }
        else
        {
            if (MyFinishCall == FinishEventCall.Position)
            {
                RecoverGameObject();
            }
            //TraceUtil.Log("PlayPositionAnimComplete");
        }
    }

    void PlayScaleAnim(object obj)
    {
        if (PlayScaleAnimStep + 1 < ScaleAnimationList.Length)
        {
            var currentScaleData = ScaleAnimationList[PlayScaleAnimStep];
            var nextScaleData = ScaleAnimationList[PlayScaleAnimStep + 1];
            MyTweenScale.method = currentScaleData.Method;
            MyTweenScale.from = currentScaleData.Scale;
            MyTweenScale.duration = currentScaleData.Time;
            MyTweenScale.to = nextScaleData.Scale;
            MyTweenScale.CallBackWhenFinished = PlayScaleAnim;
            PlayScaleAnimStep++;
            UITweener.Begin<TweenScale>(gameObject, currentScaleData.Time);
            //TraceUtil.Log("PlayScaleAnim:" + PlayScaleAnimStep + "," + MyTweenScale.to);
        }
        else
        {
            if (MyFinishCall == FinishEventCall.Scale)
            {
                RecoverGameObject();
            }
            //TraceUtil.Log("PlayScaleAnimComplete");
        }
    }

    void RecoverGameObject()
    {
        TraceUtil.Log("RecoverGameObject");
        //GameObjectPool.Instance.Release(gameObject);
    }

    void GetTweenComponent()
    {
        if (MyTweenAlpha == null)
        {
            MyTweenAlpha = gameObject.GetComponent<TweenAlpha>();
            if (MyTweenAlpha == null)
            {
                MyTweenAlpha = gameObject.AddComponent<TweenAlpha>();
            }
        }
        if (MyTweenPosition == null)
        {
            MyTweenPosition = gameObject.GetComponent<TweenPosition>();
            if (MyTweenPosition == null)
            {
                MyTweenPosition = gameObject.AddComponent<TweenPosition>();
            }
            MyTweenPosition.to = MyTweenPosition.from = transform.localPosition;
        }
        if (MyTweenRotation == null)
        {
            MyTweenRotation = gameObject.GetComponent<TweenRotation>();
            if (MyTweenRotation == null)
            {
                MyTweenRotation = gameObject.AddComponent<TweenRotation>();
            }
            MyTweenRotation.to = MyTweenRotation.from = transform.localRotation.eulerAngles;
        }

        if (MyTweenScale == null)
        {
            MyTweenScale = gameObject.GetComponent<TweenScale>();
            if (MyTweenScale == null)
            {
                MyTweenScale = gameObject.AddComponent<TweenScale>();
            }
            MyTweenScale.to = MyTweenScale.from = transform.localScale;
        } 
    }
}

[Serializable]
public class AlphaAnimationInfo
{
    public UITweener.Method Method;
    public float Alpha;
    public float Time;
}
[Serializable]
public class PositionAnimationInfo
{
    public UITweener.Method Method;
    public Vector3 Position;
    public float Time;
}
[Serializable]
public class ScaleAnimationInfo
{
    public UITweener.Method Method;
    public Vector3 Scale;
    public float Time;
}

