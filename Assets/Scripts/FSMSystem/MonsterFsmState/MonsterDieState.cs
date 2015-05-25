using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;

public class MonsterDieState : MonsterFsmState
{
    private string m_normalDieAniName = "Dead";
    private string m_splitDieAniName = "Dead_1";
    private bool m_isDied;

    public MonsterDieState()
    {
        this.m_stateID = StateID.MonsterDie;
    }

    public override void Reason()
    {

    }

    public override void Act()
    {
        //if (!this.m_roleAnimationComponent.IsPlaying(m_currentSkillAniName))
        //{
        //    if (!m_isDied)
        //    {
        //        EntityController.Instance.UnRegisteEntity(m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity);
        //        m_isDied = true;
        //    }
            
        //}
    }
    
    public override void DoBeforeEntering()
    {
        //if(MonsterManager.Instance.IsMonsterBossType(this.m_MonsterBehaviour.m_MonsterConfigData._monsterID))
        //    TraceUtil.Log("Boss死亡");
        this.m_roleAnimationComponent.CrossFade(this.m_MonsterBehaviour.SplitToDie ? m_splitDieAniName : m_normalDieAniName);

        base.DoBeforeEntering();
    }

    public override void DoBeforeLeaving()
    {
       
        base.DoBeforeLeaving();
    }

   
}
