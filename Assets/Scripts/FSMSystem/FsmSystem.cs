using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Battle;
using System;

public enum Transition  //转换
{
    //Use this transition to represent a non-existing transition in your system
    NullTransition = 0,
    #region Player Transition
    PlayerToIdle,       //Idle
    PlayerToTarget,  //Run
    PlayerBeAttacked,  //Attacked
    PlayerToBeHitFly,
    PlayerBeAdsorb,  //Attacked
    PlayerFireNormalSkill,  //Normal skill 被动技能施放-- 普通攻击
    PlayerFireScrollSkill,  //Scroll skill 被动技能施放-- 普通攻击
    PlayerInvokeInitiativeSkill,  //主动技能触发
    PlayerSelecteFireTarget, //主动技能选择目标
    PlayerFireInitiativeSkill,
	PlayerInitialtiveSkillSelect,
    PlayerToDie,
    PlayerToStand,
    PlayerToNpc,
    PlayerToCastAbility,
    #endregion
    #region Monster Transition
    MonsterToIdle,
    MonsterToMove,
    MonsterToAttack,
    MonsterToBeAttacked,
    MonsterToDie,
    MonsterToBeHitFly,
    MonsterToStand,
    MonsterToBeAdsorb,
    MonsterToBeHorde,
    #endregion
}
public enum StateID    //状态ID
{
    //Use this ID to represent a non-existing State in your system
    NullStateID = 0,
    #region Player StateID
    PlayerIdle,
    PlayerRun,
    PlayerBeAttacked,
    PlayerBeHitFly,
    PlayerBeAdsorb,
    PlayerNormalSkill,
    PlayerScrollSkill,
    PlayerInitiativeSkill,
	PlayerInitialtiveSkillSelect,
    PlayerDie,
    PlayerStand,
    PlayerFindPathing,
    PlayerCastAbility,
    #endregion
    #region Monster StateID
    MonsterIdle,
    MonsterMove,
    MonsterAttack,
    MonsterBeAttacked,
    MonsterDie,
    MonsterBeHitFly,
    MonsterStand,
    MonsterBeAdsorb,
    MonsterBeHorde,
    #endregion
}
public delegate void ChangeTransitionHandler(Transition trans);

public abstract class FSMState
{
    protected bool m_isAnimPlayed;
    protected View m_roleBehaviour;
    public Animation m_roleAnimationComponent;
    protected Dictionary<Transition, StateID> m_map = new Dictionary<Transition, StateID>();
    protected StateID m_stateID;
    public StateID StateID { get { return m_stateID; } }
    public bool IsStateReady { get; protected set; }
    public ChangeTransitionHandler OnChangeTransition;
    public float ReasonTime{get;set;}

    public void AddTransition(Transition trans, StateID id)
    {
        //check if anyone of the args is invalid
        if (trans == Transition.NullTransition)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSMState Error:NullTransition is not allowed for a real transition");
            return;
        }

        if (id == StateID.NullStateID)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSMState Error: NullStateID is not allowed for a real ID");
            return;
        }

        if (m_map.ContainsKey(trans))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSMState Error:State" + m_stateID.ToString() + " aleady has transition " + trans.ToString() + " Impossible to assign to another state");
            return;
        }
        m_map.Add(trans, id);
    }

    public void DeleteTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSMState ERROR:NullTransition is not allowed");
            return;
        }
        if (m_map.ContainsKey(trans))
        {
            m_map.Remove(trans);
            return;
        }
        TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSMState ERROR: Transition " + trans.ToString() + " passed to " + m_stateID.ToString() + " was not on the state's transition list");
    }
    public StateID GetOutputState(Transition trans)
    {
        if (m_map.ContainsKey(trans))
        {
            return m_map[trans];
        }
        return StateID.NullStateID;
    }

    public virtual void SetRole(View role) { this.m_roleBehaviour = role; m_roleAnimationComponent = this.m_roleBehaviour.animation; }
    public virtual void DoBeforeEntering() { this.IsStateReady = true; }
    public virtual void DoBeforeLeaving() { this.IsStateReady = false; m_isAnimPlayed = false; }
    public abstract void Reason();
    public abstract void Act();

    protected void ChangeTransition(Transition transition)
    {
        if (OnChangeTransition != null)
        {
            OnChangeTransition(transition);
        }
    }
}
public class FSMSystem
{   
    private List<FSMState> m_states;
    private View m_roleBehaviour;
    private Func<Transition,bool> m_allowStateChange;
    private StateID m_currentStateID;
    public StateID CurrentStateID { get { return m_currentStateID; } }
    private StateID m_previourStateID;
    private FSMState m_currentState;
    public FSMState CurrentState { get { return m_currentState; } }
    public ChangeTransitionHandler OnPerformTransition;

    public FSMSystem(View roleBehaviour, Func<Transition,bool> allowStateChange)
    {
        this.m_allowStateChange = allowStateChange;
        this.m_roleBehaviour = roleBehaviour;
        m_states = new List<FSMState>();
    }
    public FSMSystem(View roleBehaviour):this(roleBehaviour,null){}

    public void AddState(FSMState s)
    {
        if (s == null)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSM ERROR: Null reference is not allowed");
        }
        foreach (FSMState state in m_states)
        {
            if (state.StateID == s.StateID)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSM ERROR:Impossible to add state " + s.StateID.ToString() + " because state has already been added");

                return;
            }
        }
        s.OnChangeTransition += this.PerformTransition;
        s.SetRole(this.m_roleBehaviour);
        if (m_states.Count == 0)
        {
            m_states.Add(s);
            m_currentState = s;
            m_currentStateID = s.StateID;
            return;
        }
        m_states.Add(s);
    }

    public void DeleteState(StateID id)
    {
        if (id == StateID.NullStateID)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSM ERROR: NullStateID is not allowed for a real state");

            return;
        }
        foreach (FSMState state in m_states)
        {
            if (state.StateID == id)
            {
                m_states.Remove(state);

                return;
            }
        }
        TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSM ERROR: Impossible to delete state " + id.ToString() + ". It was not on the list of states");
    }

    public void PerformTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSM ERROR:NullTransition is not allowed for a real transition");
            return;
        }

        StateID id = m_currentState.GetOutputState(trans);
        if (id == StateID.NullStateID)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"FSM ERROR: State " + m_currentStateID.ToString() + " does not have a target state for transition " + trans.ToString());

            return;
        }
        if (m_allowStateChange != null && !m_allowStateChange(trans))
        {
            return;
        }
        m_previourStateID = m_currentStateID;
        m_currentStateID = id;
        foreach (FSMState state in m_states)
        {
            if (state.StateID == m_currentStateID)
            {
                m_currentState.DoBeforeLeaving();
                m_currentState = state;
                if (OnPerformTransition != null) OnPerformTransition(trans);
                m_currentState.DoBeforeEntering();
                break;
            }
        }
    }
    public bool IsStateBreak()
    {
        return this.m_previourStateID == m_currentStateID;
    }
    public FSMState FindState(StateID id)
    {
        FSMState targetState = null;
        foreach (FSMState state in m_states)
        {
            if (state.StateID == id)
            {
                targetState=state;
                break;
            }
        }

        return targetState;
    }
}

