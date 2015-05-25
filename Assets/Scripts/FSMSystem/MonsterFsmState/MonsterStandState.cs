using UnityEngine;
using System.Collections;

public class MonsterStandState : MonsterFsmState
{
    private string m_animationName;
    public MonsterStandState()
    {
        m_stateID = StateID.MonsterStand;
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

        if (!this.m_roleAnimationComponent.IsPlaying(m_animationName))
        {
            this.OnChangeTransition(Transition.MonsterToIdle);
        }
    }

    public override void Act()
    {
        if (!IsStateReady)
            return;
    }


    public override void DoBeforeEntering()
    {
        if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
        {
            SMsgFightClimbs_CS sMsgFightClimbs_CS = new SMsgFightClimbs_CS();
            sMsgFightClimbs_CS.uidFighter = m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity;
            sMsgFightClimbs_CS.byType = 1;
            sMsgFightClimbs_CS.hitedPosX = m_MonsterBehaviour.ThisTransform.position.x * 10.0f;
            sMsgFightClimbs_CS.hitedPosY = -m_MonsterBehaviour.ThisTransform.position.z *10.0f;
            NetServiceManager.Instance.BattleService.SendFightClimb_CS(sMsgFightClimbs_CS);
        }

        this.m_roleAnimationComponent.CrossFade(m_animationName);
        DoNotSendBeatEnd = false;
        base.DoBeforeEntering();
    }

    public override void DoBeforeLeaving()
    {
        if(!DoNotSendBeatEnd)
        {
            if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
            {
                SMsgFightClimbs_CS sMsgFightClimbs_CS = new SMsgFightClimbs_CS();
                sMsgFightClimbs_CS.uidFighter = m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity;
                sMsgFightClimbs_CS.byType = 0;
                sMsgFightClimbs_CS.hitedPosX = m_MonsterBehaviour.ThisTransform.position.x * 10.0f;
                sMsgFightClimbs_CS.hitedPosY = -m_MonsterBehaviour.ThisTransform.position.z *10.0f;
                NetServiceManager.Instance.BattleService.SendFightClimb_CS(sMsgFightClimbs_CS);


//                SMsgHitFlyContextNum_CS sMsgHitFlyContextNum_CS = new SMsgHitFlyContextNum_CS();
//                sMsgHitFlyContextNum_CS.byContextNum = 1;
//                sMsgHitFlyContextNum_CS.list = new System.Collections.Generic.List<SMsgFightHitFly_CS>();
//                
//                SMsgFightHitFly_CS sMsgFightHitFly_CS = new SMsgFightHitFly_CS();
//                sMsgFightHitFly_CS.uidFighter = m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity;
//                sMsgFightHitFly_CS.byType = 0;
//                sMsgFightHitFly_CS.hitedPosX = m_MonsterBehaviour.ThisTransform.position.x * 10.0f;
//                sMsgFightHitFly_CS.hitedPosY = -m_MonsterBehaviour.ThisTransform.position.z *10.0f;
//                
//                sMsgHitFlyContextNum_CS.list.Add(sMsgFightHitFly_CS);
//                
//                NetServiceManager.Instance.BattleService.SendFightHitFlyCS(sMsgHitFlyContextNum_CS);
            }
        }

        base.DoBeforeLeaving();
    }

    
}
