using UnityEngine;
using System.Collections;

public class NPCBehaviour : View, IEntityDataManager {

    private Transform m_preTrans;
	// Use this for initialization
    private IEntityDataStruct m_NPCDataModel;
    public IEntityDataStruct NPCDataModel
    {
        get { return this.m_NPCDataModel; }
        set { this.m_NPCDataModel = value; }
    }

    void Awake()
    {
        RegisterEventHandler();
    }

    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.OnTouchInvoke.ToString(), OnTouchDown);
    }

    public void OnTouchDown(INotifyArgs e)
    {
        TouchInvoke touchInvoke = (TouchInvoke)e;
        if (touchInvoke.TouchGO == gameObject)
        {
            RaiseEvent(EventTypeEnum.TargetSelected.ToString(), new TargetSelected() { Target = transform, Type = ResourceType.NPC });
            var playerBehaviour = (PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour;
            if (playerBehaviour.FSMSystem.CurrentStateID == StateID.PlayerFindPathing)
            {
                PlayerManager.Instance.HeroAgent.enabled = false;
                //playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerToIdle);
            }

            m_preTrans = transform;
        }
	}
	/*void OnBecameVisible()
	{
		Debug.Log ("OnBecameVisible gaooooo==="+gameObject.name);
	}
	void OnBecameInvisible()
	{
		Debug.Log ("OnBecameInvisible gaooooo==="+gameObject.name);
	}
	void OnWillRenderObject()
	{
		Debug.Log ("OnWillRenderObject gaooooo==="+gameObject.name);
	}*/
    void Destroy()
    {
        RemoveAllEvent();
    }

    public IEntityDataStruct GetDataModel()
    {
        return this.NPCDataModel;
    }
}
