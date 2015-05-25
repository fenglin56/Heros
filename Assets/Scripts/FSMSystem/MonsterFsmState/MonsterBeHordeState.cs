using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MonsterBeHordeState : MonsterFsmState
{
    private SMsgFightHorde_SC m_SMsgFightHorde_SC;
    private Vector3 m_hordeBeforePos;
    private float m_hordeKeepTime=0;

    public MonsterBeHordeState()
    {
        m_stateID = StateID.MonsterBeHorde;
    }

    public override void Reason()
    {
        m_hordeKeepTime += Time.deltaTime;
        if (( m_hordeKeepTime*1000.0f) >= m_SMsgFightHorde_SC.HordeTime)
        {
            this.OnChangeTransition(Transition.MonsterToIdle);
        }
    }

    public override void Act()
    {

    }
    public void BeHorde(SMsgFightHorde_SC sMsgFightHorde_SC, Vector3 hordeBeforePos)
    {

        this.m_MonsterBehaviour.BeHorde(sMsgFightHorde_SC);
        this.m_SMsgFightHorde_SC = sMsgFightHorde_SC;
        this.m_hordeBeforePos = hordeBeforePos;
    }
    public override void DoBeforeEntering()
    {
        m_MonsterBehaviour.ShowHordeFlash(true);
		DoNotSendBeatEnd = false;
        //this.m_MonsterBehaviour.ThisTransform.position = this.m_hordeBeforePos.GetFromServer(this.m_SMsgFightHorde_SC.HitedPosX, this.m_SMsgFightHorde_SC.HitedPosY);
        m_MonsterBehaviour.MoveToPoint(this.m_hordeBeforePos.GetFromServer(this.m_SMsgFightHorde_SC.HitedPosX, this.m_SMsgFightHorde_SC.HitedPosY));
        base.DoBeforeEntering();
    }
    public override void DoBeforeLeaving()
    {
        m_MonsterBehaviour.ShowHordeFlash(false);
        m_hordeKeepTime = 0;
        //this.m_MonsterBehaviour.HordeRelease();
		
		if(!DoNotSendBeatEnd)
		{
			if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
			{
                SMsgHordeContextNum_CS sMsgHordeContextNum_CS = new SMsgHordeContextNum_CS();
                sMsgHordeContextNum_CS.byContextNum = 1;
                sMsgHordeContextNum_CS.list = new List<SMsgFightHorde_CS>();

				SMsgFightHorde_CS sMsgFightHorde_CS = new SMsgFightHorde_CS();
				sMsgFightHorde_CS.byType = 0;
				sMsgFightHorde_CS.uidFighter = m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity;
				sMsgFightHorde_CS.hitedPosX = m_MonsterBehaviour.ThisTransform.position.x*10.0f;
				sMsgFightHorde_CS.hitedPosY = -m_MonsterBehaviour.ThisTransform.position.z*10.0f;

                sMsgHordeContextNum_CS.list.Add(sMsgFightHorde_CS);
				
                NetServiceManager.Instance.BattleService.SendFightHorde_CS(sMsgHordeContextNum_CS);
			}
		}
		
        base.DoBeforeLeaving();
    }
}
