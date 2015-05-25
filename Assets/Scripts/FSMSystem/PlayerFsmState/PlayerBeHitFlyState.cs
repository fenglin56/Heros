using UnityEngine;
using System.Collections;

public class PlayerBeHitFlyState : PlayerState
{
    private string m_animationName;
    private Vector3 m_motionVector = Vector3.zero;
    private Vector3 m_accelerationVector = Vector3.zero;
    private Vector3 m_highVector = Vector3.zero;
    private float m_g = 1f;
    private float m_angle;
    private float m_time;

    public PlayerBeHitFlyState()
    {
        m_stateID = StateID.PlayerBeHitFly;
        FindCorrespondAniName();
    }

    private void FindCorrespondAniName()
    {
        m_animationName = "HitFly";
    }

    public override void Reason()
    {
        if (!IsStateReady)
            return;
       
        if (m_time <= 0)
        {
            if(m_PlayerBehaviour.IsHero)
            {
                int standUpSkillId = PlayerDataManager.Instance.GetBattleItemData(this.m_PlayerBehaviour.PlayerKind).StandUpSkillID;
                m_PlayerBehaviour.m_nextSkillID = standUpSkillId;
                this.OnChangeTransition(Transition.PlayerFireInitiativeSkill);
            }

            //this.OnChangeTransition(Transition.PlayerToStand);
        }
    }

    Vector3 displacement;
    Vector3 highment;
    Vector3 gVector;
    public override void Act()
    {
        if (!IsStateReady)
            return;

        if (m_time > 0)
        {
            float t = Time.deltaTime;
            m_time -= t;

            //\加速度给的值有无符号?
            //末速度
            var v_motionVector = m_motionVector;
            m_motionVector += m_accelerationVector * t;

            var v_highVector = m_highVector;
            m_highVector -= gVector * t;

            //displacement = m_motionVector * t + 0.5f * m_accelerationVector * t * t;
            displacement = (v_motionVector + m_motionVector) * 0.5f * t;

            //highment =    m_highVector * t - 0.5f * gVector * t * t;
            highment = (v_highVector + m_highVector) * 0.5f * t;

            //水平方向:
            //Editor By rocky
            if (!SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + displacement))
            {
                m_PlayerBehaviour.MoveTo(displacement);//ThisTransform.Translate(displacement, Space.World);
            } 
            //竖直方向:
            //m_MonsterBehaviour.m_MonsterMeshRenderer.transform.Translate(highment, Space.World);

            if (!SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + highment))
            {
                m_PlayerBehaviour.ThisTransform.Translate(highment, Space.World);
            } 
        }
    }

    Vector3 mCorrectPos;
    public void BeHitFly(SMsgFightHitFly_SC smsg)
    {
        m_g = this.m_PlayerBehaviour.HitFlyHeight;
        
        m_time = smsg.hSpedd * 0.1f * 2 / m_g;  //公式        
        
        //位置处理
        mCorrectPos = mCorrectPos.GetFromServer(smsg.hitedPosX, smsg.hitedPosY);        

        Vector3 unitDir = new Vector3(smsg.directionX, 0, smsg.directionY * -1f);
        m_motionVector = unitDir.normalized * smsg.lSpeed * 0.1f;

        m_angle = 90 - Mathf.Atan2(smsg.directionY * -1f, smsg.directionX) * Mathf.Rad2Deg + 180;

        //TraceUtil.Log("下发加速度的值:" + smsg.Accelerated);

        if (smsg.Accelerated != 0)
        {
            m_accelerationVector = m_motionVector * (smsg.Accelerated * 1f / smsg.lSpeed);
        }
        else
        {
            m_accelerationVector = Vector3.zero;
        }

        //TraceUtil.Log("speed"+ m_motionVector + "accleration:" + m_accelerationVector +" ,g:" + m_g + " ,时间:" + m_time);

        //高度位移
        m_highVector = new Vector3(0, smsg.hSpedd * 0.1f, 0);
        gVector = new Vector3(0, m_g, 0);
    }


    public override void DoBeforeEntering()
    {
        //TraceUtil.Log("进入击飞");
        this.m_roleAnimationComponent.CrossFade(m_animationName);

        this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.Euler(0, m_angle, 0);

        //修正位置信息     
        if (!SceneDataManager.Instance.IsPositionInBlock(mCorrectPos))
        {
            m_PlayerBehaviour.ThisTransform.position = mCorrectPos;
        } 
       

        //清理此前的位移信息
        if (m_PlayerBehaviour.WalkToPosition != null)
            m_PlayerBehaviour.WalkToPosition = null;
        //m_PlayerBehaviour.PointQueue.Clear();
		
//		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
//		{
//            SMsgHitFlyContextNum_CS sMsgHitFlyContextNum_CS = new SMsgHitFlyContextNum_CS();
//            sMsgHitFlyContextNum_CS.byContextNum = 1;
//            sMsgHitFlyContextNum_CS.list = new System.Collections.Generic.List<SMsgFightHitFly_CS>();
//
//			SMsgFightHitFly_CS sMsgFightHitFly_CS = new SMsgFightHitFly_CS();
//			sMsgFightHitFly_CS.uidFighter = m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
//			sMsgFightHitFly_CS.byType = 1;
//			sMsgFightHitFly_CS.hitedPosX = m_PlayerBehaviour.ThisTransform.position.x * 10.0f;
//			sMsgFightHitFly_CS.hitedPosY = -m_PlayerBehaviour.ThisTransform.position.z *10.0f;
//
//            sMsgHitFlyContextNum_CS.list.Add(sMsgFightHitFly_CS);
//
//            NetServiceManager.Instance.BattleService.SendFightHitFlyCS(sMsgHitFlyContextNum_CS);
//		}

        base.DoBeforeEntering();
    }

    public override void DoBeforeLeaving()
    {
        //TraceUtil.Log("离开击飞");
        Vector3 pos = m_PlayerBehaviour.ThisTransform.position;
        //m_MonsterBehaviour.m_MonsterMeshRenderer.transform.position = new Vector3(pos.x, 0, pos.z);
        m_PlayerBehaviour.ThisTransform.position = new Vector3(pos.x, 0, pos.z);
        
		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
            SMsgHitFlyContextNum_CS sMsgHitFlyContextNum_CS = new SMsgHitFlyContextNum_CS();
            sMsgHitFlyContextNum_CS.byContextNum = 1;
            sMsgHitFlyContextNum_CS.list = new System.Collections.Generic.List<SMsgFightHitFly_CS>();

			SMsgFightHitFly_CS sMsgFightHitFly_CS = new SMsgFightHitFly_CS();
			sMsgFightHitFly_CS.uidFighter = m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
			sMsgFightHitFly_CS.byType = 0;
			sMsgFightHitFly_CS.hitedPosX = m_PlayerBehaviour.ThisTransform.position.x * 10.0f;
			sMsgFightHitFly_CS.hitedPosY = -m_PlayerBehaviour.ThisTransform.position.z *10.0f;
            sMsgHitFlyContextNum_CS.list.Add(sMsgFightHitFly_CS);

            NetServiceManager.Instance.BattleService.SendFightHitFlyCS(sMsgHitFlyContextNum_CS);
		}
		
        base.DoBeforeLeaving();
    }
}
