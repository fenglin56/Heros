using UnityEngine;
using System.Collections;

public class MonsterBeAdsorbState : MonsterFsmState
{

	private SMsgBattleBeAdsorb_SC m_SMsgBattleBeAdsorb_SC;
    private float m_time;
    private Vector3 m_adsorbDire = Vector3.zero;  //攻击方向
    private float m_adsorbSpeed;
    private Vector3 m_shouldPos = Vector3.zero;
    //private Vector3 m_moveDiret = Vector3.zero; //位移量

    public MonsterBeAdsorbState()
    {
        m_stateID = StateID.MonsterBeAdsorb;
    }
    public override void Reason()
    {
        if (IsStateReady)
        {
            if (m_time <= 0)//|| m_clientBattleBeatBack.speed <= 0)
            {
                OnChangeTransition(Transition.MonsterToIdle);
            }
        }
    }

    public override void Act()
    {
        if (IsStateReady)
        {
            float t = Time.deltaTime;

            var s = m_adsorbSpeed * t;

            Vector3 temporaryV = m_adsorbDire * s;

            //Editor By rocky
            if (!SceneDataManager.Instance.IsPositionInBlock(this.m_MonsterBehaviour.ThisTransform.position + temporaryV))
            {
                //m_MonsterBehaviour.ThisTransform.Translate(temporaryV, Space.World);
                m_MonsterBehaviour.MoveToPoint(this.m_MonsterBehaviour.ThisTransform.position + temporaryV);
            } 
                

            m_time -= t;
        }
    }
    public override void DoBeforeEntering()
    {
		DoNotSendBeatEnd = false;
        BulletManager.Instance.TryDestroyBreakBullets(this.m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity);
        //ResetAttackParameter();
        this.m_roleAnimationComponent.CrossFade("Hurt", 0.1f);

        m_shouldPos = m_shouldPos.GetFromServer(m_SMsgBattleBeAdsorb_SC.PosX, m_SMsgBattleBeAdsorb_SC.PosY);
        m_adsorbDire = (new Vector3(m_SMsgBattleBeAdsorb_SC.DirX,0,m_SMsgBattleBeAdsorb_SC.DirY * -1f)).normalized;
        
        float angel = 90 - Mathf.Atan2(m_SMsgBattleBeAdsorb_SC.DirY * -1f, m_SMsgBattleBeAdsorb_SC.DirX) * Mathf.Rad2Deg;

        //this.m_MonsterBehaviour.ThisTransform.position = m_shouldPos;
        m_MonsterBehaviour.MoveToPoint(m_shouldPos);
        this.m_MonsterBehaviour.ThisTransform.rotation = Quaternion.Euler(new Vector3(0, angel, 0));                

        base.DoBeforeEntering();
    }
    public void SetBeAdsorbData(SMsgBattleBeAdsorb_SC dataModel)
    {
        m_SMsgBattleBeAdsorb_SC = dataModel;
       // TraceUtil.Log("dir: x = " + m_SMsgBattleBeAdsorb_SC.DirX + " , y = " + m_SMsgBattleBeAdsorb_SC.DirY);
        TraceUtil.Log("time = " + m_SMsgBattleBeAdsorb_SC.time);
        m_adsorbSpeed = m_SMsgBattleBeAdsorb_SC.speed * 0.1f;
        m_time = m_SMsgBattleBeAdsorb_SC.time * 0.001f;
    }
    public override void DoBeforeLeaving()
    {
		if(DoNotSendBeatEnd)
		{
			if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
			{
                SMsgAdsorptionContextNum_CS sMsgAdsorptionContextNum_CS = new SMsgAdsorptionContextNum_CS();
                sMsgAdsorptionContextNum_CS.byContextNum = 1;

                sMsgAdsorptionContextNum_CS.list = new System.Collections.Generic.List<SMsgFightAdsorption_CS>();

				SMsgFightAdsorption_CS sMsgFightAdsorption_CS = new SMsgFightAdsorption_CS();
				sMsgFightAdsorption_CS.uidFighter = m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity;
				sMsgFightAdsorption_CS.hitedPosX = m_MonsterBehaviour.ThisTransform.position.x * 10.0f;
				sMsgFightAdsorption_CS.hitedPosY = -m_MonsterBehaviour.ThisTransform.position.z * 10.0f;
				sMsgFightAdsorption_CS.byType = 0;
				
                sMsgAdsorptionContextNum_CS.list.Add(sMsgFightAdsorption_CS);

                NetServiceManager.Instance.BattleService.SendFightAdsorption_CS(sMsgAdsorptionContextNum_CS);
			}
		}
		
        base.DoBeforeLeaving();
    }
}
