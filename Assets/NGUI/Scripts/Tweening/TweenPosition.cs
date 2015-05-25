//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

[AddComponentMenu("NGUI/Tween/Position")]
public class TweenPosition : UITweener
{
	public Vector3 from;
	public Vector3 to;
	public bool PossionTweenUseWorldPos;
	Transform mTrans;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }
	public Vector3 position { get { return cachedTransform.localPosition; } set { cachedTransform.localPosition = value; } }
	/// <summary>
	/// 
	/// </summary>
	/// <param name="factor">Factor.</param>
	/// <param name="isFinished">If set to <c>true</c> is finished.</param>
	override protected void OnUpdate (float factor, bool isFinished) 
	{
		if(!PossionTweenUseWorldPos)
		{
			cachedTransform.localPosition = from * (1f - factor) + to * factor;
		} 
	     else 
		{
			cachedTransform.position=from*(1f-factor)+to*factor;
		}
	    }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>


	static public TweenPosition Begin (GameObject go, float duration, Vector3 pos)
	{
		TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
		comp.from = comp.position;
		comp.to = pos;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
    /// <summary>
    /// 新增From 到to方法
    /// 修改人：江志祥；2013/3/28
    /// </summary>
    static public TweenPosition Begin(GameObject go, float duration,Vector3 From, Vector3 pos)
    {
        TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
        comp.from = From;
        comp.to = pos;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }
    static public TweenPosition Begin(GameObject go, float duration, Vector3 From, Vector3 pos,ButtonCallBack CallBack)
    {
        TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
        comp.CallBackWhenFinished = CallBack;
        comp.from = From;
        comp.to = pos;
        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
            CallBack(comp.gameObject);
        }
        return comp;
    }
}