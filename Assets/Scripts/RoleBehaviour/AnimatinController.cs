using UnityEngine;
using System.Collections;

public class AnimatinController : View {

	// Use this for initialization
	void Start () {
        this.RegisterEventHandler();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StopAnimation(string aniName)
    {
        var animationEvent=new AniEventStruct(){ EventName=aniName, aniAction=AniAction.Stop};
        RaiseEvent(EventTypeEnum.AnimationEvent.ToString(), animationEvent);
    }  
    protected override void RegisterEventHandler()
    {
        
    }
    private void TestAnimationEvent()
    {
        AnimationClip clip = new AnimationClip();
//        AniEventStruct ae=new AniEventStruct();
        
    }
}
public struct AniEventStruct:INotifyArgs
{
    public string EventName;
    public AniAction aniAction;

    public int GetEventArgsType()
    {
        throw new System.NotImplementedException();
    }
}
