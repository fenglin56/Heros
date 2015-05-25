using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 被吸附状态
/// </summary>
public class PlayerBeAdsorbState : PlayerState
{
    private SMsgBattleBeAdsorb_SC m_SMsgBattleBeAdsorb_SC;
    private float m_time;
    private Vector3 m_adsorbDire;  //攻击方向
    private Vector3 m_runDire;  //英雄移动方向
    private float m_runSpeed;

    public PlayerBeAdsorbState()
    {
        m_stateID = StateID.PlayerBeAdsorb;
    }
    public override void Reason()
    {
        if (m_PlayerBehaviour.IsCopy) return;
        if (IsStateReady)
        {
			m_time+=Time.deltaTime;
            if (m_time*1000f >= m_SMsgBattleBeAdsorb_SC.time)
            {
                OnChangeTransition(Transition.PlayerToIdle);
            }           
        }
    }

    public override void Act()
    {
        if (IsStateReady)
        {
            var s_adsorb = m_SMsgBattleBeAdsorb_SC.speed * Time.deltaTime *0.1f;
            var s_run = m_runSpeed * Time.deltaTime;
            var motion = m_adsorbDire * s_adsorb;// +m_runDire * s_run;
            //if (!SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
            //{
            //    m_PlayerBehaviour.HeroCharactorController.Move(motion);
            //}    
            this.m_PlayerBehaviour.MoveTo(motion);
        }
    }
    public override void DoBeforeEntering()
    {
        this.m_roleAnimationComponent.CrossFade("Hurt", 0.1f);
        m_adsorbDire = (new Vector3(m_SMsgBattleBeAdsorb_SC.DirX, 0 ,-m_SMsgBattleBeAdsorb_SC.DirY)).normalized;
        m_time = 0;
        this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.LookRotation(-m_adsorbDire);
        m_runDire = Vector3.zero;
        m_runSpeed = this.m_PlayerBehaviour.WalkSpeed;
        
//		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
//		{
//			SMsgFightAdsorption_CS sMsgFightAdsorption_CS = new SMsgFightAdsorption_CS();
//			sMsgFightAdsorption_CS.uidFighter = m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
//			sMsgFightAdsorption_CS.byType = 1;
//			sMsgFightAdsorption_CS.hitedPosX = m_PlayerBehaviour.ThisTransform.position.x * 10.0f;
//			sMsgFightAdsorption_CS.hitedPosY = -m_PlayerBehaviour.ThisTransform.position.z *10.0f;
//			NetServiceManager.Instance.BattleService.SendFightAdsorption_CS(sMsgFightAdsorption_CS);
//		}

        base.DoBeforeEntering();
    }
    public void SetBeAdsorbData(SMsgBattleBeAdsorb_SC dataModel)
    {
        m_SMsgBattleBeAdsorb_SC = dataModel;
    }

    public void Run(Vector3 walkPoint)
    {
        this.m_roleAnimationComponent.CrossFade("Walk02");
        Vector3 from = new Vector3(walkPoint.x, this.m_PlayerBehaviour.ThisTransform.position.y, walkPoint.z);
        m_runDire = (from - this.m_PlayerBehaviour.ThisTransform.position).normalized;
        this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.LookRotation(m_runDire);
    }

    public override void DoBeforeLeaving()
    {
		
		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
            SMsgAdsorptionContextNum_CS sMsgAdsorptionContextNum_CS = new SMsgAdsorptionContextNum_CS();
            sMsgAdsorptionContextNum_CS.byContextNum = 1;
            sMsgAdsorptionContextNum_CS.list = new List<SMsgFightAdsorption_CS>();

			SMsgFightAdsorption_CS sMsgFightAdsorption_CS = new SMsgFightAdsorption_CS();
			sMsgFightAdsorption_CS.uidFighter = m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
			sMsgFightAdsorption_CS.byType = 0;
			sMsgFightAdsorption_CS.hitedPosX = m_PlayerBehaviour.ThisTransform.position.x * 10.0f;
			sMsgFightAdsorption_CS.hitedPosY = -m_PlayerBehaviour.ThisTransform.position.z *10.0f;

            sMsgAdsorptionContextNum_CS.list.Add(sMsgFightAdsorption_CS);

            NetServiceManager.Instance.BattleService.SendFightAdsorption_CS(sMsgAdsorptionContextNum_CS);
		}
        base.DoBeforeLeaving();
    }
}
