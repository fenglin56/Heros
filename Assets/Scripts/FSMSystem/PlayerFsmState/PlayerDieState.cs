using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerDieState : PlayerState
{
    private string m_currentSkillAniName = "Dead";
    private bool m_isDied;

    public PlayerDieState()
    {
        this.m_stateID = StateID.PlayerDie;
    }

    public override void Reason()
    {

    }

    public override void Act()
    {
    }

    public override void DoBeforeEntering()
    {
        //this.m_roleAnimationComponent.CrossFade(m_currentSkillAniName);
		//m_roleBehaviour.animation[m_currentSkillAniName].wrapMode = UnityEngine.WrapMode.Loop;
		//bool  play = m_roleBehaviour.animation.Play(m_currentSkillAniName);
		TraceUtil.Log("DEAD PLAYED!!!!!!!_________________");
		//m_roleAnimationComponent[m_currentSkillAniName].wrapMode = UnityEngine.WrapMode.Loop;
		m_roleAnimationComponent.Play(m_currentSkillAniName);
		TraceUtil.Log("!!!!!!!____enter die state");
		
        base.DoBeforeEntering();
    }

    public override void DoBeforeLeaving()
    {
        
		TraceUtil.Log("!!!!!!______Leave Die State");
        base.DoBeforeLeaving();
    }
}
