using UnityEngine;
using System.Collections;

public class MonsterIdleState : MonsterFsmState
{

    private string m_animationName;

    public MonsterIdleState()
    {
        m_stateID = StateID.MonsterIdle;
        FindCorrespondAniName();
    }
   
    public override void Reason()
    {
        if (!IsStateReady)
            return;

        if (MonsterBehaviour.UsePathSmooth)
        {
            //if (m_MonsterBehaviour.WalkToPosition != null && (m_MonsterBehaviour.WalkToPosition.Value - m_MonsterBehaviour.ThisTransform.position).sqrMagnitude >= 0.2f)
            //{
                //OnChangeTransition(Transition.MonsterToMove);
            //}
        }
        else
        {
            //路点移动
            if (m_MonsterBehaviour.PointQueue.Count > 0)
            {
                var nextPoint = m_MonsterBehaviour.PointQueue.Dequeue();
                Vector3 nextPos = Vector3.zero;
                nextPos = nextPos.GetFromServer(nextPoint.x, 0.1f, nextPoint.y, -0.1f);
                if ((new Vector3(nextPos.x, 0, nextPos.y) - m_MonsterBehaviour.ThisTransform.position).sqrMagnitude >= 0.2f)
                {
                    m_MonsterBehaviour.WalkToPosition = nextPos;
                    OnChangeTransition(Transition.MonsterToMove);
                }
            }
        }                
    }

    public override void Act()
    {
        if (!IsStateReady)
            return;
       
        m_roleAnimationComponent.CrossFade(m_animationName);
    }

    private void FindCorrespondAniName()
    {
        //\假设找得此怪物所用动画名称为:

        m_animationName = "BIdle";
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
    }
    public override void DoBeforeLeaving()
    {
        base.DoBeforeLeaving();
    }
}
