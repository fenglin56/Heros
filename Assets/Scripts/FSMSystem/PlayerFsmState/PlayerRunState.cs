using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 跑步
/// </summary>
public class PlayerRunState : PlayerState
{
	private float m_smoothStepTime;
	private float m_smoothTotalTime;
    private bool m_rotationToTarget;
	
	public enum PlayerMoveMode
	{
		RUN, 
		WALK,
	}
	
	private int m_lockStep = 0;
	
	
	private Vector3 m_moveVector;

    public Vector3 MoveVector
    {
        get { return m_moveVector; }
    }

	private float m_runTime;
	private float m_walkTime;
	
	private bool m_moveEnd = false;
	private PlayerMoveMode m_playerMoveMode;
	
	public float CurrentMoveSpeed
	{
		get { 
			if(m_playerMoveMode == PlayerMoveMode.WALK && GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE)
			{
                if(m_PlayerBehaviour.PlayerKind == 1)
                {
                    return CommonDefineManager.Instance.CommonDefine.Char01WalkSpeed;
                }
                else if(m_PlayerBehaviour.PlayerKind == 4)
                {
                    return CommonDefineManager.Instance.CommonDefine.Char04WalkSpeed;
                }

				return this.m_PlayerBehaviour.WalkSpeed*0.5f; 
			}
			else
			{
				return this.m_PlayerBehaviour.WalkSpeed;
			}
		}	
	}
	
	public PlayerMoveMode MoveMode
	{
		get {return m_playerMoveMode;}
		set { m_playerMoveMode = value; }
		
	}
	
	public void StartToRun(float runTime, float walkTime)
	{
		m_runTime = runTime;
		m_walkTime = walkTime;
		m_playerMoveMode = PlayerMoveMode.RUN;
		m_moveEnd = false;
		m_moveVector =( this.m_PlayerBehaviour.WalkToPosition.Value - this.m_PlayerBehaviour.ThisTransform.position );
		m_moveVector.y = 0;
		m_moveVector.Normalize();
       
		
		Vector2 V1 = new Vector2(this.m_PlayerBehaviour.ThisTransform.position.x,this.m_PlayerBehaviour.ThisTransform.position.z);
        Vector2 V2 = new Vector2(this.m_PlayerBehaviour.WalkToPosition.Value.x, this.m_PlayerBehaviour.WalkToPosition.Value.z);
        var walkDistance = Vector2.Distance(V1,V2);//this.m_PlayerBehaviour.ThisTransform.position, this.m_PlayerBehaviour.WalkToPosition.Value);
        var targetPos = this.m_PlayerBehaviour.WalkToPosition.Value;
        //Vector3 from = new Vector3(targetPos.x, this.m_PlayerBehaviour.ThisTransform.position.y, targetPos.z);
		//this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.LookRotation(from - this.m_PlayerBehaviour.ThisTransform.position, Vector3.up);
	}
	
	
    public PlayerRunState()
    {
        m_stateID = StateID.PlayerRun;        
    }
    public override void Reason()
    {
        if (m_PlayerBehaviour.IsCopy) return;
        if (IsStateReady)
        {
            if (m_PlayerBehaviour.IsHero)
            {
				if( (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN && m_PlayerBehaviour.IsJoyStickPress) || GameManager.Instance.UseJoyStick )
				//if(!GameManager.Instance.UseJoyStick)
				{
					if(m_PlayerBehaviour.NormalAttackButtonPress)
					{
						OnChangeTransition(Transition.PlayerFireNormalSkill);	
					}
					else if( !m_PlayerBehaviour.IsJoyStickPress || m_PlayerBehaviour.JoyStickDir == Vector3.zero)
					{
						OnChangeTransition(Transition.PlayerToIdle);
					}
				}
				else
				{
					InvokeAttackMonster();
					if (m_attackModel != null && m_attackModel.Count() > 0)
					{
						if(m_lockStep <= 0)
						{
							var targetEntityModel = LockAttackMonster(this.m_attackDistance, this.m_attackAngle);
							if (targetEntityModel != null)
							{
								
								
								m_PlayerBehaviour.ThisTransform.LookAt(new Vector3(targetEntityModel.GO.transform.position.x, m_PlayerBehaviour.ThisTransform.position.y, targetEntityModel.GO.transform.position.z));
							}
							
							OnChangeTransition(Transition.PlayerFireNormalSkill);
						}
					}
					else if ((this.m_PlayerBehaviour.WalkToPosition == null && GameManager.Instance.CurrentState != GameManager.GameState.GAME_STATE_BATTLE )
					         || (this.m_PlayerBehaviour.TargetType == ResourceType.Terrain && m_moveEnd && GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE) )
					{
						OnChangeTransition(Transition.PlayerToIdle);
					}
					m_lockStep--;
				}
            }
            else
            {
                if (m_PlayerBehaviour.ClientEndPos != null && Vector3.Distance(m_PlayerBehaviour.ThisTransform.position, m_PlayerBehaviour.ClientEndPos.Value) < 3f)
                //m_smoothStepTime > m_smoothTotalTime 
                //&& m_PlayerBehaviour.CurrentNode.fSpeed <= 0.01f)
                {
                    OnChangeTransition(Transition.PlayerToIdle);
                }
            }
        }
    }
	
	public void ResetSmooth(float totalSmoothTime)
	{
		m_smoothStepTime = 0.0f;
		m_smoothTotalTime = totalSmoothTime;
		
	}
	
	
	public void UpdateRunState()
	{
		float dt = Time.deltaTime;
		if(m_playerMoveMode == PlayerMoveMode.RUN)
		{
			if(m_runTime <= 0)
			{
				m_runTime = 0;
				m_playerMoveMode = PlayerMoveMode.WALK;
			}
			else
			{
				m_runTime -= dt;
			}
			
		}
		else if(m_playerMoveMode == PlayerMoveMode.WALK)
		{
			if(m_walkTime <= 0)
			{
				m_moveEnd = true;
			}
			else 
			{
				m_walkTime -= dt;
			}
			
		}
	}

    /// <summary>
    /// 地面行走
    /// </summary>
    void GoToTerrain()
    {
        if (m_PlayerBehaviour.IsHero)
        {
			if(m_PlayerBehaviour.TargetType == ResourceType.Terrain && GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE)
			{
				Vector3 targetPos = m_PlayerBehaviour.ThisTransform.position + m_moveVector;
				UpdateRotation(targetPos);
				UpdateRunState();
			}
			else
			{
				m_playerMoveMode = PlayerMoveMode.RUN;
		        Vector2 V1 = new Vector2(this.m_PlayerBehaviour.ThisTransform.position.x,this.m_PlayerBehaviour.ThisTransform.position.z);
		        Vector2 V2 = new Vector2(this.m_PlayerBehaviour.WalkToPosition.Value.x, this.m_PlayerBehaviour.WalkToPosition.Value.z);
		        var walkDistance = Vector2.Distance(V1,V2);//this.m_PlayerBehaviour.ThisTransform.position, this.m_PlayerBehaviour.WalkToPosition.Value);
		        var targetPos = this.m_PlayerBehaviour.WalkToPosition.Value;
		        Vector3 from = new Vector3(targetPos.x, this.m_PlayerBehaviour.ThisTransform.position.y, targetPos.z);
				m_moveVector = from - this.m_PlayerBehaviour.ThisTransform.position;
				m_moveVector.y = 0;
				m_moveVector.Normalize();
				UpdateRotation(targetPos);
		        //this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.LookRotation(from - this.m_PlayerBehaviour.ThisTransform.position, Vector3.up);
		        //TraceUtil.Log(walkDistance);
		        if (walkDistance < ConfigDefineManager.DISTANCE_ARRIVED_TARGET)
		        {
		            this.m_PlayerBehaviour.WalkToPosition = null;
		            //TraceUtil.Log("Stop:"+walkDistance);
		        }
			}
        }
        else
        {

        }
    }
	
	void UpdateRotation( Vector3 targetPos )
	{
		Vector3 targetDirect = targetPos - m_PlayerBehaviour.ThisTransform.position;
		targetDirect.y = 0;
		Quaternion wantedRotation=Quaternion.LookRotation(targetDirect, Vector3.up);
		float t =CommonDefineManager.Instance.CommonDefine.TurnRoundSpeed/Quaternion.Angle(m_PlayerBehaviour.ThisTransform.rotation,wantedRotation)*Time.deltaTime;
		m_PlayerBehaviour.ThisTransform.rotation = Quaternion.Lerp(m_PlayerBehaviour.ThisTransform.rotation, wantedRotation, t);
	}
	
    /// <summary>
    /// 走向传送门
    /// </summary>
    void GoToPortal()
    {
        var targetPos = this.m_PlayerBehaviour.WalkToPosition.Value;
        Vector3 from = new Vector3(targetPos.x, this.m_PlayerBehaviour.ThisTransform.position.y, targetPos.z);
        if (Vector3.Distance(this.m_PlayerBehaviour.ThisTransform.position, this.m_PlayerBehaviour.WalkToPosition.Value) >= ConfigDefineManager.DISTANCE_ARRIVED_TARGET)
        {
			//var targetPos = this.m_PlayerBehaviour.WalkToPosition.Value;
		    //Vector3 from = new Vector3(targetPos.x, this.m_PlayerBehaviour.ThisTransform.position.y, targetPos.z);
			m_moveVector = from - this.m_PlayerBehaviour.ThisTransform.position;
			m_moveVector.y = 0;
			m_moveVector.Normalize();
			UpdateRotation(targetPos);
            //this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.LookRotation(from - this.m_PlayerBehaviour.ThisTransform.position, Vector3.up);
            //this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.Slerp(this.m_PlayerBehaviour.ThisTransform.rotation
            //   , Quaternion.LookRotation(from - this.m_PlayerBehaviour.ThisTransform.position, Vector3.up), Time.deltaTime * 10);
        }
        else
        {
            this.m_PlayerBehaviour.WalkToPosition = null;
            //Send Portal To Service
            var portalBehaviour = this.m_PlayerBehaviour.TargetSelected.Target.GetComponent<PortalBehaviour>();
            if (portalBehaviour != null)
            {
                SMsgPropCreateEntity_SC_Channel dataModel = (SMsgPropCreateEntity_SC_Channel)portalBehaviour.PortalDataModel;

                NetServiceManager.Instance.EntityService.SendEnterPortal(dataModel.SMsg_Header.uidEntity);
            }
            else
            {
                //TraceUtil.Log("没找到PortalBehaviour组件");
            }
        }
    }
    /// <summary>
    /// 走向NPC
    /// </summary>
    void GoToNPC()
    {
        var targetPos = this.m_PlayerBehaviour.WalkToPosition.Value;
        Vector3 from = new Vector3(targetPos.x, this.m_PlayerBehaviour.ThisTransform.position.y, targetPos.z);
        if (Vector3.Distance(this.m_PlayerBehaviour.ThisTransform.position, this.m_PlayerBehaviour.WalkToPosition.Value) >= ConfigDefineManager.DISTANCE_ARRIVED_NPC)
        {
			m_moveVector = from - this.m_PlayerBehaviour.ThisTransform.position;
			m_moveVector.y = 0;
			m_moveVector.Normalize();
			UpdateRotation(targetPos);
            //this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.LookRotation(from - this.m_PlayerBehaviour.ThisTransform.position, Vector3.up);
            //this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.Slerp(this.m_PlayerBehaviour.ThisTransform.rotation
            //   , Quaternion.LookRotation(from - this.m_PlayerBehaviour.ThisTransform.position, Vector3.up), Time.deltaTime * 10);
        }
        else
        {
            this.m_PlayerBehaviour.WalkToPosition = null;
            //Send Portal To Service
            ////TraceUtil.Log("到达传送门，发送传送消息");
            /// 
            /// 
            if(null != this.m_PlayerBehaviour.TargetSelected.Target)
            {
                var npcBehaviour = this.m_PlayerBehaviour.TargetSelected.Target.GetComponent<NPCBehaviour>();
                if (npcBehaviour != null)
                {
                    NetServiceManager.Instance.EntityService.SendMeetNPC(npcBehaviour.NPCDataModel.SMsg_Header.uidEntity);
    				GameManager.Instance.MeetNpcEntityId = npcBehaviour.NPCDataModel.SMsg_Header.uidEntity;
                }
                else
                {
                    //TraceUtil.Log("没找到PortalBehaviour组件");
                }
            }
            else
            {

            }
        }
    }
    private void GoToTarget()
    {
		//如果摇杆在动，点击就没有起效,在城镇界面UseJoyStick一直为false，城镇和这个变量一点关系都没有//
		if( (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN && m_PlayerBehaviour.IsJoyStickPress) || GameManager.Instance.UseJoyStick )
		{
			Vector3 moveDir = m_PlayerBehaviour.JoyStickDir;
			if(moveDir != Vector3.zero)
			{
				this.m_PlayerBehaviour.WalkToPosition = null;
				this.m_PlayerBehaviour.LastWalkToPosition = null;
				m_playerMoveMode = PlayerMoveMode.RUN;
				Vector3 lootAtPos = m_PlayerBehaviour.ThisTransform.position + moveDir*10;
				lootAtPos.y = m_PlayerBehaviour.ThisTransform.position.y;
				this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.LookRotation(lootAtPos - this.m_PlayerBehaviour.ThisTransform.position, Vector3.up);
				m_moveVector = moveDir;
				m_moveVector.y = 0;
				m_moveVector.Normalize();
			}
		}
		else
		{
	        switch (this.m_PlayerBehaviour.TargetType)
	        {
	            case ResourceType.Terrain:
	                GoToTerrain();
	                break;
	            case ResourceType.Portal:
	                GoToPortal();
	                break;
	            case ResourceType.NPC:
	                GoToNPC();
	                break;
	        }	
		}
    }
    public override void Act()
    {
		if(SceneDataManager.Instance.IsPositionInBlock(m_PlayerBehaviour.ThisTransform.position))
		{
			//TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"_++++++++++++Player in block!!!!!!!!!!!");	
		}
		
        if (IsStateReady)
        {
			if(m_PlayerBehaviour.IsHero)
			{
				if (this.m_PlayerBehaviour.WalkToPosition != null || ((GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN && m_PlayerBehaviour.IsJoyStickPress) || GameManager.Instance.UseJoyStick) )
	            {
					
	                GoToTarget();                    
					
					float moveSpeed = 0;
					
					moveSpeed = CurrentMoveSpeed;
	                //this.m_PlayerBehaviour.HeroCharactorController.SimpleMove(this.m_PlayerBehaviour.ThisTransform.TransformDirection(Vector3.forward) * this.m_PlayerBehaviour.WalkSpeed);
			
					
                    var motion = m_moveVector * moveSpeed * Time.deltaTime;//this.m_PlayerBehaviour.ThisTransform.TransformDirection(Vector3.forward) * moveSpeed * Time.deltaTime;
					
					if(m_PlayerBehaviour.IsBeAbsordEx)
					{
						Vector3 absordVector = (m_PlayerBehaviour.AdsorbExCenterPos - m_PlayerBehaviour.ThisTransform.position).normalized;	
						motion += absordVector * m_PlayerBehaviour.AdsorbExSpeed * Time.deltaTime;
						
					}

                    if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
                    {
                        this.m_PlayerBehaviour.HeroCharactorController.Move(motion);
                    }
                    else if (SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
                    {
						//modify by xun.wu
                        //this.m_PlayerBehaviour.WalkToPosition = null;
						SceneDataManager.ColideType colide = SceneDataManager.Instance.GetColideType(this.m_PlayerBehaviour.ThisTransform.position, 
							this.m_PlayerBehaviour.ThisTransform.position + motion);
						Vector3 oldMotion = new Vector3(motion.x, motion.y, motion.z);
						if(colide == SceneDataManager.ColideType.COLIDE_LEFTRIGHT)
						{
							motion.x = 0;
						}
						else
						{
							motion.z = 0;
						}
						if(SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
						{
							if(colide == SceneDataManager.ColideType.COLIDE_LEFTRIGHT)
							{
								motion.x = oldMotion.x;	
								motion.z = 0;
							}
							else
							{
								motion.x = 0;
								motion.z = oldMotion.z;	
							}
							if(SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
							{
									
							}
							else
							{
								//this.m_PlayerBehaviour.HeroCharactorController.Move(motion);
                                this.m_PlayerBehaviour.MoveTo(motion);
							}
						}
						else
						{
							//this.m_PlayerBehaviour.HeroCharactorController.Move(motion);
                            this.m_PlayerBehaviour.MoveTo(motion);
						}
                    }
                    else
                    {
                        //this.m_PlayerBehaviour.HeroCharactorController.Move(motion);
                        this.m_PlayerBehaviour.MoveTo(motion);
                    }
	            }
			}
			else
			{
				if(null == m_PlayerBehaviour.ClientEndPos)
				{
					m_smoothStepTime += Time.deltaTime;
					if(m_smoothStepTime <= m_smoothTotalTime)
					{
						Vector3 nextPos = m_PlayerBehaviour.Smooth.GetCurrentPos(m_smoothStepTime/m_smoothTotalTime);
						//m_PlayerBehaviour.ThisTransform.LookAt(nextPos);
                        m_PlayerBehaviour.ThisTransform.rotation = m_PlayerBehaviour.Smooth.GetCurrentQuaternion(m_smoothStepTime/m_smoothTotalTime);
						m_PlayerBehaviour.ThisTransform.position = nextPos;
						
					}
					else
					{
						m_PlayerBehaviour.ThisTransform.position = m_PlayerBehaviour.Smooth.GetCurrentPos(1.0f);
                        m_PlayerBehaviour.ThisTransform.rotation = m_PlayerBehaviour.Smooth.GetCurrentQuaternion(1.0f);
						SMsgActionMove_SCS currentNode = m_PlayerBehaviour.CurrentNode;
						Vector3 speedVector =  new Vector3(currentNode.fDirectX, 0, -1*currentNode.fDirectY)*currentNode.fSpeed/10.0f;
						m_PlayerBehaviour.HeroCharactorController.Move(speedVector * (m_smoothStepTime - m_smoothTotalTime));
					}
					
					
				}
				else
				{
					Vector3 moveVector = (m_PlayerBehaviour.ClientEndPos.Value - m_PlayerBehaviour.ThisTransform.position).normalized * m_PlayerBehaviour.WalkSpeed*Time.deltaTime;
					moveVector.y = 0;
					if(moveVector != Vector3.zero)
					{
						//m_PlayerBehaviour.ThisTransform.LookAt(m_PlayerBehaviour.ThisTransform.position + moveVector);
						m_PlayerBehaviour.ThisTransform.position += moveVector;
					}
				}
			}

            string animName = string.Empty;
            switch (GameManager.Instance.CurrentState)
            {
                case GameManager.GameState.GAME_STATE_TOWN:
                    animName = "Walk01";
                    break;
                case GameManager.GameState.GAME_STATE_BATTLE:
					if(m_playerMoveMode == PlayerMoveMode.RUN)
					{
                    	animName = "Walk02";
					}
					else if(m_playerMoveMode == PlayerMoveMode.WALK)
					{
						animName = "Walk03";
					}
                    break;
                case GameManager.GameState.GAME_STATE_STORYLINE:
                    animName = "Walk02";
                    break;
                default :
                    animName = "Walk01";
                    break;
            }
            if (!m_roleAnimationComponent.IsPlaying(animName))
            {
                m_roleAnimationComponent.CrossFade(animName);
            }
        }
    }

    public override void DoBeforeEntering()
    {
		m_lockStep = 3;
		if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE && m_PlayerBehaviour.IsHero)
		{
            if(m_PlayerBehaviour.m_runEffect != null)
            {
                //TODO; set to active when the effect artwork is done
                m_PlayerBehaviour.m_runEffect.gameObject.SetActive(true);
            }
			if(!GameManager.Instance.UseJoyStick)
			{
				StartToRun(2.0f, 2.0f);
			}
			m_playerMoveMode = PlayerMoveMode.RUN;
		}
		
        m_rotationToTarget = true;
        this.m_PlayerBehaviour.GetSkillBySkillID(null);
        base.DoBeforeEntering();
    }
    public override void DoBeforeLeaving()
    {
        if(m_PlayerBehaviour.IsHero)
        {
            if(m_PlayerBehaviour.m_runEffect != null)
            {
                m_PlayerBehaviour.m_runEffect.gameObject.SetActive(false);
            }
        }

        this.m_PlayerBehaviour.WalkToPosition = null;
        base.DoBeforeLeaving();
    }
}