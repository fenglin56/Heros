using UnityEngine;
using System.Collections;

public class MonsterBeAttackState : MonsterFsmState
{
    private string m_animationName;
    //private SMsgBattleBeatBack_SC m_sMsgBattleBeatBack_SC;
    private Vector3 m_motionVector = Vector3.zero;
    private Vector3 m_accelerationVector = Vector3.zero;

    private float m_instantSpeed;
    private float m_accelerationSpeed;

    private float m_angle;
    private float m_time;
 
    public MonsterBeAttackState()
    {
        m_stateID = StateID.MonsterBeAttacked;
        FindCorrespondAniName();
    }

    private void FindCorrespondAniName()
    {
        //\
        m_animationName = "Hurt";
    }

    Vector3 mCorrectPos;
    public void BeAttacked(SMsgBattleBeatBack_SC smsg)
    {
        m_time = smsg.time / 1000f;

        //\暂时处理      
        mCorrectPos = mCorrectPos.GetFromServer(smsg.PosX, smsg.PosY);
        
        //TraceUtil.Log("接到结算数据包:" + smsg.DirX + " , " + smsg.DirY + " , " + smsg.speed + " , " + smsg.Accelerated);

        //float rad = smsg.Angel/1000f * Mathf.Deg2Rad;                  

        //float x = Mathf.Cos(rad);
        //float z = -1 * Mathf.Sin(rad);       
        //m_angle =90 - Mathf.Atan2(z, x) * Mathf.Rad2Deg + 180;      
        m_instantSpeed = smsg.speed;
        m_accelerationSpeed = smsg.Accelerated;

        Vector3 unitDir = new Vector3(smsg.DirX, 0, smsg.DirY * -1f);
        m_motionVector = unitDir.normalized * smsg.speed * 0.1f;

        m_angle = 90 - Mathf.Atan2(smsg.DirY * -1f, smsg.DirX) * Mathf.Rad2Deg + 180;      

        if (smsg.Accelerated != 0)
        {
            m_accelerationVector = m_motionVector * (smsg.Accelerated * 1.0f/ smsg.speed);
        }
        else
        {
            m_accelerationVector = Vector3.zero;
        }

    }


    public override void Reason()
    {
        if (!IsStateReady)
            return;

        //if (!this.m_MonsterBehaviour.animation.IsPlaying(m_animationName) && m_time <= 0)
        //{
        //    this.OnChangeTransition(Transition.MonsterToIdle);
        //}
        if (m_time <= 0)
        {
            this.OnChangeTransition(Transition.MonsterToIdle);
        }
        
    }

    Vector3 _Temporary_EnterPoint = Vector3.zero;

    Vector3 displacement;
    public override void Act()
    {
        if (!IsStateReady)
            return;

        float t = Time.deltaTime;
        m_time -= t;
        
        //\加速度给的值有无符号?
        displacement = m_motionVector * t + 0.5f * m_accelerationVector * t * t;
 
        m_instantSpeed += m_accelerationSpeed * Time.deltaTime;        
        if (m_instantSpeed <= 0)
        {           
            return;
        }

        //Editor By rocky
        if (!SceneDataManager.Instance.IsPositionInBlock(this.m_MonsterBehaviour.ThisTransform.position + displacement))
        {
            //m_MonsterBehaviour.ThisTransform.Translate(displacement, Space.World);
            m_MonsterBehaviour.MoveToPoint(this.m_MonsterBehaviour.ThisTransform.position + displacement);
        } 
                
        m_motionVector += m_accelerationVector * t;
    
    }

    public override void DoBeforeEntering()
    {        
		DoNotSendBeatEnd = false;
        //AnimationState hurtState = this.m_roleAnimationComponent[m_animationName];
        //hurtState.speed = hurtState.length / m_time;
        BulletManager.Instance.TryDestroyBreakBullets(this.m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity);
		if( this.m_roleAnimationComponent.IsPlaying(m_animationName))
		{
			 this.m_roleAnimationComponent.Stop( m_animationName );
		}
        this.m_roleAnimationComponent.CrossFade(m_animationName, 0.1f);
        
        this.m_MonsterBehaviour.ThisTransform.rotation = Quaternion.Euler(0, m_angle, 0);

        var player = PlayerManager.Instance.FindHeroEntityModel();
        Vector3 playerPos = player.GO.transform.position;
        float dis = Vector3.Distance(this.m_MonsterBehaviour.ThisTransform.position , mCorrectPos);

        float dif1 = Vector3.Distance(this.m_MonsterBehaviour.ThisTransform.position,playerPos );
        float dif2 = Vector3.Distance(mCorrectPos, playerPos);

        //string result = dif1 > dif2 ? "慢" : "快";
        //Vector3 difVec3 = (mCorrectPos - this.m_MonsterBehaviour.ThisTransform.position );
        //float difRad = Mathf.Atan2(difVec3.z, difVec3.x);
        //float diss = difVec3.x / Mathf.Cos(difRad);


        float difX = this.m_MonsterBehaviour.ThisTransform.position.x - mCorrectPos.x;
        float difY = this.m_MonsterBehaviour.ThisTransform.position.z - mCorrectPos.z;
        //TraceUtil.Log("怪物受击前后位置偏差 : " + dis + " " + result + "  x相差 : " + difX + "  y相差 : " + difY);        
        float serverX;
        float serverY;
        this.m_MonsterBehaviour.ThisTransform.position.SetToServer(out serverX,out serverY);
        //Log.Instance.WirteOhterLog("怪物修正前位置: " + serverX + " , " + serverY);

        //修正位置信息
        //m_MonsterBehaviour.ThisTransform.position = mCorrectPos;
        m_MonsterBehaviour.MoveToPoint(mCorrectPos);
        //清理此前的位移信息
        if (m_MonsterBehaviour.WalkToPosition != null)
            m_MonsterBehaviour.WalkToPosition = null;
        m_MonsterBehaviour.PointQueue.Clear();

        //\
        _Temporary_EnterPoint = this.m_MonsterBehaviour.ThisTransform.position;

        base.DoBeforeEntering();
    }
    public override void DoBeforeLeaving()
    {
        Vector3 _Temporary_LeavePoint = this.m_MonsterBehaviour.ThisTransform.position;
        float distance = Vector3.Distance(_Temporary_EnterPoint, _Temporary_LeavePoint);
        //TraceUtil.Log("击退结算 : " + distance);
		
		if(!DoNotSendBeatEnd)
		{
			if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
			{
                SMsgBeatBackContextNum_CS sMsgBeatBackContextNum_CS = new SMsgBeatBackContextNum_CS();
                sMsgBeatBackContextNum_CS.byContextNum = 1;
                sMsgBeatBackContextNum_CS.list = new System.Collections.Generic.List<SMsgFightBeatBack_CS>();

				SMsgFightBeatBack_CS sMsgFightBeatBack_CS = new SMsgFightBeatBack_CS();
				sMsgFightBeatBack_CS.uidFighter = m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity;
				sMsgFightBeatBack_CS.byType = 0;
				sMsgFightBeatBack_CS.hitedPosX = m_MonsterBehaviour.ThisTransform.position.x * 10.0f;
				sMsgFightBeatBack_CS.hitedPosY = -m_MonsterBehaviour.ThisTransform.position.z *10.0f;

                sMsgBeatBackContextNum_CS.list.Add(sMsgFightBeatBack_CS);

                NetServiceManager.Instance.BattleService.SendFightBeatBackCS(sMsgBeatBackContextNum_CS);
			}
		}
		
        base.DoBeforeLeaving();
    }
}
