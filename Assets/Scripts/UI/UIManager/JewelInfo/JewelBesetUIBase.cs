using UnityEngine;
using System.Collections;

public abstract class JewelBesetUIBase : View {


	public TweenPosition tween;
	public bool IsShowing { get; protected set; }

	protected override  void  RegisterEventHandler()
	{
		TraceUtil.Log("");
	}
    public virtual void ShowAnim()
	{
		if(!IsShowing)
		{
		
		tween.Reset();
		tween.Play(true);
		IsShowing=true;
		}

	}
    public virtual void HidePanel()
	{
		IsShowing=false;
		transform.localPosition=new Vector3(0,0,-1000);
	}

	public virtual void ShowPanel()
	{
		IsShowing=true;
		transform.localPosition=tween.to;
	}
    public virtual void CloseAnim()
	{
		if(IsShowing)
		{
		IsShowing=false;
		tween.Play(false);
		}
	}


}
