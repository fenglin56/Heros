using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 受击状态
/// </summary>
public class PlayerBeAttackedState : PlayerState
{
    private ClientBattleBeatBack m_clientBattleBeatBack;
    private float m_time;
    private Vector3 m_attackDire;  //攻击方向

    public PlayerBeAttackedState()
    {
        m_stateID = StateID.PlayerBeAttacked;
    }
    public override void Reason()
    {
        if (m_PlayerBehaviour.IsCopy) return;
        if (IsStateReady)
        {
            if (m_time >= m_clientBattleBeatBack.time )//|| m_clientBattleBeatBack.speed <= 0)
            {
                OnChangeTransition(Transition.PlayerToIdle);
            }
            
        }
    }

    public override void Act()
    {
        if (IsStateReady)
        {
            var t = Time.deltaTime;
            var s = m_clientBattleBeatBack.speed * t + m_clientBattleBeatBack.Accelerated * Mathf.Pow(t, 2) * 0.5f;
            s = s <= 0 ? 0 : s;  //当击退时间未结束时，击退速度≤0，则将击退速度为0。但击退状态不改变，直到击退时间结束。
            m_time += t;

            m_clientBattleBeatBack.speed += m_clientBattleBeatBack.Accelerated * t;

            var motion = m_attackDire * s;
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

        m_attackDire = m_clientBattleBeatBack.TargetRotation * Vector3.forward * -1;

		m_time = 0;


        this.m_PlayerBehaviour.ThisTransform.rotation = m_clientBattleBeatBack.TargetRotation;
		
//		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
//		{
//			SMsgFightBeatBack_CS sMsgFightBeatBack_CS = new SMsgFightBeatBack_CS();
//			sMsgFightBeatBack_CS.uidFighter = m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
//			sMsgFightBeatBack_CS.byType = 1;
//			sMsgFightBeatBack_CS.hitedPosX = m_PlayerBehaviour.ThisTransform.position.x * 10.0f;
//			sMsgFightBeatBack_CS.hitedPosY = -m_PlayerBehaviour.ThisTransform.position.z *10.0f;
//			NetServiceManager.Instance.BattleService.SendFightBeatBackCS(sMsgFightBeatBack_CS);
//		}

        base.DoBeforeEntering();
    }
    public override void DoBeforeLeaving()
    {
		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
            SMsgBeatBackContextNum_CS sMsgBeatBackContextNum_CS = new SMsgBeatBackContextNum_CS();
            sMsgBeatBackContextNum_CS.byContextNum = 1;
            sMsgBeatBackContextNum_CS.list = new List<SMsgFightBeatBack_CS>();

			SMsgFightBeatBack_CS sMsgFightBeatBack_CS = new SMsgFightBeatBack_CS();
			sMsgFightBeatBack_CS.uidFighter = m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
			sMsgFightBeatBack_CS.byType = 0;
			sMsgFightBeatBack_CS.hitedPosX = m_PlayerBehaviour.ThisTransform.position.x * 10.0f;
			sMsgFightBeatBack_CS.hitedPosY = -m_PlayerBehaviour.ThisTransform.position.z *10.0f;

            sMsgBeatBackContextNum_CS.list.Add(sMsgFightBeatBack_CS);

            NetServiceManager.Instance.BattleService.SendFightBeatBackCS(sMsgBeatBackContextNum_CS);
		}
		
        base.DoBeforeLeaving();
    }
    
    public void SetBeatBackData(SMsgBattleBeatBack_SC dataModel)
    {
        Vector3 mCorrectPos = this.m_PlayerBehaviour.ThisTransform.position;
        var sSMsgBattleBeatBack_SC = (SMsgBattleBeatBack_SC)dataModel;
        m_clientBattleBeatBack = new ClientBattleBeatBack();
       
        m_clientBattleBeatBack.Accelerated = sSMsgBattleBeatBack_SC.Accelerated;
        m_clientBattleBeatBack.TargetRotation = Quaternion.FromToRotation(Vector3.forward, new Vector3(-dataModel.DirX, 0, dataModel.DirY));   //玩家的朝向，与击退方向相反。所以x*-1,y*-1

        m_clientBattleBeatBack.speed = sSMsgBattleBeatBack_SC.speed*0.1f;
        m_clientBattleBeatBack.Accelerated = sSMsgBattleBeatBack_SC.Accelerated * 0.1f;
        m_clientBattleBeatBack.time = sSMsgBattleBeatBack_SC.time/1000f;
        
        mCorrectPos = mCorrectPos.GetFromServer(sSMsgBattleBeatBack_SC.PosX, sSMsgBattleBeatBack_SC.PosY);
        if (!SceneDataManager.Instance.IsPositionInBlock(mCorrectPos))
        {
            this.m_PlayerBehaviour.ThisTransform.position = mCorrectPos;
        }   
        
    }   
    struct ClientBattleBeatBack 
    {
        public Int64 uidFighter;		//被击退者的实体ID
        public Quaternion TargetRotation;
        public float speed;						//速度（分米每秒）
        public float Accelerated;					//加速度（分米每秒）
        public float time;								//时间(毫秒)
    }
}