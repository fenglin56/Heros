using UnityEngine;
using System.Collections;

public class PlayerStandState : PlayerState
{
    private string m_animationName;
    private float animClipLength;
    public PlayerStandState()
    {
        m_stateID = StateID.PlayerStand;
        FindCorrespondAniName();
    }

    private void FindCorrespondAniName()
    {
        m_animationName = "StandUp";
    }

    public override void Reason()
    {
        if (!IsStateReady)
            return;
        animClipLength -= Time.deltaTime;
        if (animClipLength<float.Epsilon)
        {
            this.OnChangeTransition(Transition.PlayerToIdle);
        }
    }

    public override void Act()
    {
        if (!IsStateReady)
            return;
    }


    public override void DoBeforeEntering()
    {
        animClipLength = this.m_roleAnimationComponent["StandUp"].clip.length;
        this.m_roleAnimationComponent.CrossFade(m_animationName);
        base.DoBeforeEntering();
    }

    public override void DoBeforeLeaving()
    {

        base.DoBeforeLeaving();
    }


}
