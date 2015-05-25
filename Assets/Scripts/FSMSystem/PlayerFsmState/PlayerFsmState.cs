using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class PlayerState : FSMState
{
    protected PlayerBehaviour m_PlayerBehaviour;
    protected IEnumerable<EntityModel> m_attackModel;
    protected float m_timeLockState=0;
    protected float m_attackDistance;
    protected float m_attackAngle;
    protected GameObject m_hero;

    public bool IsSkillBeBreaked= false;

    public PlayerState()
    {
        this.AddTransition(Transition.PlayerToIdle, StateID.PlayerIdle);
        this.AddTransition(Transition.PlayerToTarget, StateID.PlayerRun);
        this.AddTransition(Transition.PlayerFireNormalSkill, StateID.PlayerNormalSkill);
        this.AddTransition(Transition.PlayerFireScrollSkill, StateID.PlayerScrollSkill);
        this.AddTransition(Transition.PlayerBeAttacked, StateID.PlayerBeAttacked);
        this.AddTransition(Transition.PlayerFireInitiativeSkill, StateID.PlayerInitiativeSkill);
        this.AddTransition(Transition.PlayerInitialtiveSkillSelect, StateID.PlayerInitialtiveSkillSelect);
        this.AddTransition(Transition.PlayerToDie, StateID.PlayerDie);
        this.AddTransition(Transition.PlayerBeAdsorb, StateID.PlayerBeAdsorb);
        this.AddTransition(Transition.PlayerToBeHitFly, StateID.PlayerBeHitFly);
        this.AddTransition(Transition.PlayerToStand, StateID.PlayerStand);
        this.AddTransition(Transition.PlayerToNpc, StateID.PlayerFindPathing);
        this.AddTransition(Transition.PlayerToCastAbility, StateID.PlayerCastAbility);
    }
    public override void SetRole(View role)
    {
        base.SetRole(role);
        m_hero = PlayerManager.Instance.FindHero();
        m_PlayerBehaviour = (PlayerBehaviour)this.m_roleBehaviour;

        int skillID = PlayerDataManager.Instance.GetBattleItemData(this.m_PlayerBehaviour.PlayerKind).NormalSkillID[0];
        if (skillID != 0)
        {
            TraceUtil.Log("技能ID为：" + skillID);
            m_attackDistance = SkillDataManager.Instance.GetSkillConfigData(skillID).m_triggerRange[0];
            m_attackAngle = SkillDataManager.Instance.GetSkillConfigData(skillID).m_triggerRange[1] * 2;  //配置的角度是正前方偏移的角度，所以范围角度要乘2
        }
        else
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"技能ID为0");
        }
    }   
    public void SetTarget(TargetSelected target)
    {
        m_PlayerBehaviour.TargetSelected = target;
        switch (target.Type)
        {
            case ResourceType.Monster:
                m_PlayerBehaviour.TargetType = target.Type;
                m_PlayerBehaviour.WalkToPosition = target.Target.position;
                break;
            case ResourceType.Portal:
                m_PlayerBehaviour.TargetType = target.Type;
                m_PlayerBehaviour.WalkToPosition = target.Target.position;
                break;
            case ResourceType.NPC:
                m_PlayerBehaviour.TargetType = target.Type;
                m_PlayerBehaviour.WalkToPosition = target.Target.position;
                break;
        }
    }
    public void InvokeAttackMonster()
    {
        InvokeAttackMonster(m_attackDistance, m_attackAngle);       
    }
    public void InvokeAttackMonster(float radius,float angle)
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE)
        {
            if(m_stateID == StateID.PlayerRun)
            {
                Vector3 dir = ((PlayerRunState)this).MoveVector;
                this.m_attackModel = MonsterManager.Instance.SearchBeAttackedMonster(m_PlayerBehaviour.ThisTransform.position, radius
                                                                                     , dir, angle);
               // this.m_attackModel = 
            }
            else
            {
                this.m_attackModel = MonsterManager.Instance.SearchBeAttackedMonster(m_PlayerBehaviour.ThisTransform.position, radius
                    , m_PlayerBehaviour.ThisTransform.forward, angle);
            }
        }
        else
        {
            this.m_attackModel = null;
        }
    }
    /// <summary>
    /// 判断目标是否还在攻击范围
    /// </summary>
    /// <param name="entityModel"></param>
    /// <returns></returns>
    protected bool CheckAtAttackRange(EntityModel entityModel)
    {
        Vector3 center = this.m_PlayerBehaviour.ThisTransform.position;
        Vector3 target = entityModel.GO.transform.position;
        //float distance = this.m_PlayerBehaviour.LockRadius;
        //float angle = this.m_PlayerBehaviour.LockAngle;
        return (Vector3.Distance(center, target) <= this.m_attackDistance) && CommonTools.AngleBetween2Vector(m_PlayerBehaviour.ThisTransform.forward, (target - center)) <= this.m_attackAngle / 2;
    }
    public EntityModel LockAttackMonster(float radius, float angle)
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE && !PVPBattleManager.Instance.IsPVPBattle)
        {
           
            var forward = m_PlayerBehaviour.ThisTransform.forward;
            if(m_stateID == StateID.PlayerRun)
            {
                forward = ((PlayerRunState)this).MoveVector;
            }
            var center = m_PlayerBehaviour.ThisTransform.position;
            var monstersTarget = MonsterManager.Instance.SearchBeAttackedMonster(center, radius, forward, angle);
            if(monstersTarget == null)
            {
                return null;   
            }
            var monsters = monstersTarget.ToList();
            if (monsters.Count > 0)
            {
                monsters.Sort((a, b) => 
                {
                    float anga = CommonTools.AngleBetween2Vector(forward, (a.GO.transform.position - center));
                    float angb = CommonTools.AngleBetween2Vector(forward, (b.GO.transform.position - center));
                    if (anga == angb)
                        return 0;
                    else if (anga > angb)
                        return 1;
                    else
                        return -1;

                }); 
                return monsters[0];
            }
        }
        return null;
    }
    protected bool IsPlayerMoving()
    {
        if (this.m_PlayerBehaviour.WalkToPosition != null
            && Vector3.Distance(this.m_PlayerBehaviour.ThisTransform.position, this.m_PlayerBehaviour.WalkToPosition.Value) >= ConfigDefineManager.DISTANCE_ARRIVED_TARGET)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public override void DoBeforeEntering()
    {
        //控制玩家阴影
        this.PlayerShadowRendererControll();
        base.DoBeforeEntering();
    }
    public override void DoBeforeLeaving()
    {
        base.DoBeforeLeaving();
    }
    private void PlayerShadowRendererControll()
    {
        if (this.m_roleBehaviour != null)
        {
            var roleBehaviour = (RoleBehaviour)this.m_roleBehaviour;
            switch (this.StateID)
            {
                case global::StateID.PlayerBeAttacked:
                case global::StateID.PlayerIdle:
                case global::StateID.PlayerRun:
                case global::StateID.PlayerInitialtiveSkillSelect:
                case global::StateID.PlayerFindPathing:        
                   if(!roleBehaviour.ShadowRenderer.enabled)
                       roleBehaviour.ShadowRenderer.enabled = true;                   
                    break;
                default:
                    if (roleBehaviour.ShadowRenderer.enabled)
                        roleBehaviour.ShadowRenderer.enabled = false;      
                    break;

            }
        }
    }
}


