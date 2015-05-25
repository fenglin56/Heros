using UnityEngine;
using System.Collections;


public class MonsterMoveState  : MonsterFsmState
{
    private string m_animationName;
	
	private float m_smoothStepTime;
	private float m_smoothTotalTime;
    private EntityModel m_monsterEntityModel;
    private float m_speed;

    public MonsterMoveState()
    {
        m_stateID = StateID.MonsterMove;
        //FindCorrespondAniName();        
    }


    public override void Reason()
    {
        if (!IsStateReady)
            return;
		
		
		if(!MonsterBehaviour.UsePathSmooth)
		{
	        if (m_MonsterBehaviour.WalkToPosition == null)
	        {
	            OnChangeTransition(Transition.MonsterToIdle);
	        }
		}
		else
		{
			if( m_MonsterBehaviour.ClientEndPos != null && Vector3.Distance(m_MonsterBehaviour.ThisTransform.position, m_MonsterBehaviour.ClientEndPos.Value) < 3f )
				//m_smoothStepTime > m_smoothTotalTime 
					//&& m_PlayerBehaviour.CurrentNode.fSpeed <= 0.01f)
				{
					OnChangeTransition(Transition.MonsterToIdle);	
				}
		} 
    }
	
	public void ResetSmooth(float totalSmoothTime)
	{
		m_smoothStepTime = 0.0f;
		m_smoothTotalTime = totalSmoothTime;
		
	}

    public override void Act()
    {
		
        if (!IsStateReady)
            return;
		
		FindCorrespondAniNameAndSpeed();
		if(!MonsterBehaviour.UsePathSmooth)
		{
	        if (Vector3.Distance(m_MonsterBehaviour.ThisTransform.position, m_MonsterBehaviour.WalkToPosition.Value) >= ConfigDefineManager.DISTANCE_ARRIVED_TARGET)
	        {
	           // m_MonsterBehaviour.ThisTransform.position = Vector3.MoveTowards(m_MonsterBehaviour.ThisTransform.position,
               //                                                                 m_MonsterBehaviour.WalkToPosition.Value, Time.deltaTime * m_speed);
                m_MonsterBehaviour.MoveToPoint(Vector3.MoveTowards(m_MonsterBehaviour.ThisTransform.position, m_MonsterBehaviour.WalkToPosition.Value, Time.deltaTime * m_speed));
	            m_MonsterBehaviour.ThisTransform.rotation = Quaternion.Slerp(m_MonsterBehaviour.ThisTransform.rotation
	               , Quaternion.LookRotation(m_MonsterBehaviour.WalkToPosition.Value - m_MonsterBehaviour.ThisTransform.position, Vector3.up), Time.deltaTime * 10);
	        }
	        else
	        {        
				//m_MonsterBehaviour.ThisTransform.position = m_MonsterBehaviour.WalkToPosition.Value;
                m_MonsterBehaviour.MoveToPoint(m_MonsterBehaviour.WalkToPosition.Value);
	            m_MonsterBehaviour.WalkToPosition = null;
	        }
		}
		else
		{
			if(null == m_MonsterBehaviour.ClientEndPos)
			{
				m_smoothStepTime += Time.deltaTime;
				if(m_smoothStepTime <= m_smoothTotalTime)
				{
					Vector3 nextPos = m_MonsterBehaviour.Smooth.GetCurrentPos(m_smoothStepTime/m_smoothTotalTime);
					//Vector3 lookPos = m_MonsterBehaviour.Smooth.GetCurrentPos( ( m_smoothStepTime + Time.deltaTime )/m_smoothTotalTime);
					//m_MonsterBehaviour.ThisTransform.LookAt(lookPos);
					//m_MonsterBehaviour.ThisTransform.position = nextPos;
                    m_MonsterBehaviour.MoveToPoint(nextPos);
					
				}
				else
				{
					//m_MonsterBehaviour.ThisTransform.position = m_MonsterBehaviour.Smooth.GetCurrentPos(1.0f);
                    m_MonsterBehaviour.MoveToPoint(m_MonsterBehaviour.Smooth.GetCurrentPos(1.0f));
					SMsgActionMove_SCS currentNode = m_MonsterBehaviour.CurrentNode;
					Vector3 speedVector =  new Vector3(currentNode.fDirectX, 0, -1*currentNode.fDirectY)*currentNode.fSpeed/10.0f;
					m_MonsterBehaviour.ThisTransform.position += speedVector * (m_smoothStepTime - m_smoothTotalTime);

					
					//m_PlayerBehaviour.HeroCharactorController.Move(speedVector * (m_smoothStepTime - m_smoothTotalTime));
				}
				
			}
			else
			{
                Vector3 moveVector = (m_MonsterBehaviour.ClientEndPos.Value - m_MonsterBehaviour.ThisTransform.position).normalized * m_speed * Time.deltaTime;
				//m_MonsterBehaviour.ThisTransform.LookAt(m_MonsterBehaviour.ThisTransform.position + moveVector);
				m_MonsterBehaviour.ThisTransform.position += moveVector;
			}	
			float targetAngle = 90 - Mathf.Atan2( -m_MonsterBehaviour.CurrentNode.fDirectY, m_MonsterBehaviour.CurrentNode.fDirectX)*180/Mathf.PI;
			m_MonsterBehaviour.ThisTransform.rotation = Quaternion.Slerp(m_MonsterBehaviour.ThisTransform.rotation, Quaternion.Euler(0, targetAngle, 0), 10*Time.deltaTime);
			
		}
        m_roleAnimationComponent.CrossFade(m_animationName);
    }
    private void FindCorrespondAniNameAndSpeed()
    {
        m_animationName = "Walk02";
        //\假设找得此怪物所用动画名称为:
        switch (((SMsgPropCreateEntity_SC_Monster)m_monsterEntityModel.EntityDataStruct).MonsterUnitValues.UINT_FIELD_STATE)
        {
            case (int)CRT_STATE.enCrt_State_Move:
                m_speed = this.m_MonsterBehaviour.WalkSpeed;
                m_animationName = "Walk02";
                break;
            case (int)CRT_STATE.enCrt_State_Run:
                m_speed = this.m_MonsterBehaviour.RunSpeed;
                m_animationName = "Walk03";
                break;
        }
    }

    public override void DoBeforeEntering()
    {
        m_MonsterBehaviour.FixMonsterYToOrigin();
        if (m_monsterEntityModel==null)
        {
            m_monsterEntityModel = MonsterManager.Instance.GetEntityMode(this.m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity);
        }
        FindCorrespondAniNameAndSpeed();
        base.DoBeforeEntering();
    }
    public override void DoBeforeLeaving()
    {       
        base.DoBeforeLeaving();
    }
}
