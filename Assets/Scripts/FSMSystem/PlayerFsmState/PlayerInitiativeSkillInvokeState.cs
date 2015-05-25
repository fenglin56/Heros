using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 主动技能触发
/// </summary>
public class PlayerInitiativeSkillInvokeState : FSMState
{
    /// <summary>
    /// 技能ID
    /// </summary>
    private SkillBase m_skillBase;

    public PlayerInitiativeSkillInvokeState()
    {
        m_stateID = StateID.PlayerInitiativeSkill;
    }

    public override void Reason()
    {
        if (IsStateReady)
        {

        }
    }

    public override void Act()
    {
        if (IsStateReady)
        {

        }
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