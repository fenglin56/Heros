using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 主动技能走向目标
/// </summary>
public class PlayerInitiativeSkillGotoState : FSMState
{
    private SkillBase m_skillBase;
    public PlayerInitiativeSkillGotoState()
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