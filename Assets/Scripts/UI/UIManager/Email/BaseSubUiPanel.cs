using UnityEngine;
using System.Collections;
namespace UI.MainUI
{
public class BaseSubUiPanel : View {

	public Vector3 TweenFrom;
	public Vector3 TweenTo;
	public float TweenDuration;
	public bool IsShowing { get; protected set; }
	
    protected override void RegisterEventHandler()
        {
           
        }
	public virtual void ShowAnim()
	{
		if(!IsShowing)
		{
		
			TweenPosition.Begin(gameObject,TweenDuration,TweenFrom,TweenTo);
			IsShowing=true;
		}
		
	}
	public virtual void HidePanel()
	{
			if(IsShowing)
			{
				IsShowing=false;
                StartCoroutine(Hide());
			}
	}
	
	IEnumerator Hide()
		{
		
			TweenAlpha.Begin(gameObject,0.2f,0);
			yield return new WaitForSeconds(0.2f);
			transform.localPosition=new Vector3(0,0,-1000);

        }
	public virtual void ShowPanel()
	{
			if(!IsShowing)
			{
		     IsShowing=true;
		     transform.localPosition=TweenTo;
             TweenAlpha.Begin(gameObject,0.2f,1);
			}
	}

	public virtual void CloseAnim()
	{
		if(IsShowing)
		{
			IsShowing=false;
			TweenPosition.Begin(gameObject,TweenDuration,TweenTo,TweenFrom);
		}
	}
}
}
