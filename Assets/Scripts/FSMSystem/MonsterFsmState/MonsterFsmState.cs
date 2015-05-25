using UnityEngine;
using System.Collections;


public abstract class MonsterFsmState : FSMState
{
    protected MonsterBehaviour m_MonsterBehaviour;
	
	public bool DoNotSendBeatStart;
	public bool DoNotSendBeatEnd;

    public MonsterFsmState()
    {
        this.AddTransition(Transition.MonsterToIdle, StateID.MonsterIdle);
        this.AddTransition(Transition.MonsterToMove, StateID.MonsterMove);
        this.AddTransition(Transition.MonsterToBeAttacked, StateID.MonsterBeAttacked);
        this.AddTransition(Transition.MonsterToAttack, StateID.MonsterAttack);
        this.AddTransition(Transition.MonsterToDie, StateID.MonsterDie);
        this.AddTransition(Transition.MonsterToBeHitFly, StateID.MonsterBeHitFly);
        this.AddTransition(Transition.MonsterToStand, StateID.MonsterStand);
        this.AddTransition(Transition.MonsterToBeAdsorb, StateID.MonsterBeAdsorb);
        this.AddTransition(Transition.MonsterToBeHorde, StateID.MonsterBeHorde);
    }
    /// <summary>
    /// 重写基类，怪物Prefab有两个状态，正常和死亡分离
    /// </summary>
    /// <param name="role"></param>
    public override void SetRole(View role)
    {
        this.m_roleBehaviour = role;
        m_MonsterBehaviour = (MonsterBehaviour)this.m_roleBehaviour;
        this.m_roleAnimationComponent = m_MonsterBehaviour.NormalStatus.animation;
    }

}
