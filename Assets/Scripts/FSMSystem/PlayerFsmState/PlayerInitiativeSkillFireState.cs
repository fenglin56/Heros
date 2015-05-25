using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 主动技能施放
/// </summary>
public class PlayerInitiativeSkillFireState : PlayerState
{
    private SkillBase m_skillBase;
    private SkillActionData m_actData;
	private Vector3 m_attackDire;  //攻击方向
    private Vector3 m_playerStartPosition;
    private float dis;
    private float s = 0;

    public Vector3 m_touchPoint;  //用于范围型技能，并移动类型为必达（3）时做为目标地点
    //public Transform m_actionTarget;   //用于范围型技能，并移动类型为必达（3）且目标为游戏对象时做为动作攻击目标
    private EntityModel m_lockedTarget; //技能锁定目标，用于判断普攻突击计算,因为突击开始后不再追击，所以不能实时从PlayerBehaviour取ActionLockTarget
    private Camera m_skillCamera;
    
    private bool m_isCreateUIEffect = false;
    private float m_delayCreateUIEffectTime = 0;


    public PlayerInitiativeSkillFireState()
    {
        m_stateID = StateID.PlayerInitiativeSkill;
    }

    public override void Reason()
    {
        if (m_PlayerBehaviour.IsCopy) return;
        if (IsStateReady)
        {
            if (IsSkillBeBreaked)
            {
                m_skillBase.BreakSkill();
                if (m_skillCamera != null)
                    m_skillCamera.GetComponent<SkillCamera>().OnDestroy();
                m_PlayerBehaviour.GetSkillBySkillID(PlayerDataManager.Instance.GetBattleItemData(this.m_PlayerBehaviour.PlayerKind).NormalSkillID[0]);
                OnChangeTransition(Transition.PlayerToIdle);
            }
        }
    }

    public override void Act()
    {
        if (IsStateReady)
        {            

            CheckJoyStickInSkillFire();

            //位移
            if (m_actData.m_ani_followtype == 6)  //定速到达型
            {
                s = m_actData.m_startSpeed * Time.deltaTime;
            }
            else
            {
                //只有范围型技能才能与移动类型为3配合
                if (m_actData.m_ani_followtype != 3)
                {
                    var t = Time.deltaTime;
                    s = m_actData.m_startSpeed * t + m_actData.m_acceleration * Mathf.Pow(t, 2) * 0.5f;

                    m_actData.m_startSpeed += Mathf.FloorToInt(m_actData.m_acceleration * t);
                }

                if (!m_isAnimPlayed || m_roleAnimationComponent[m_actData.m_animationId].wrapMode == WrapMode.Loop)
                {
                    m_roleAnimationComponent.Play(m_actData.m_animationId);
                    m_isAnimPlayed = true;
                }
            }
            var motion = m_attackDire * s;

			if(m_actData.m_ani_followtype == 6 && SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
			{
				m_PlayerBehaviour.GetSkillBySkillID(PlayerDataManager.Instance.GetBattleItemData(this.m_PlayerBehaviour.PlayerKind).NormalSkillID[0]);
				OnChangeTransition(Transition.PlayerToIdle);
				return;
			}

            //if (!SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
            //{
            //    m_PlayerBehaviour.HeroCharactorController.Move(motion);               
            //}


            this.m_PlayerBehaviour.MoveTo(motion);
        }
        /*
        if (m_isCreateUIEffect)
        {
            m_delayCreateUIEffectTime -= Time.deltaTime;
            if (m_delayCreateUIEffectTime <= 0)
            {
                //UI特效
                var uiEffectGroups = m_skillBase.SkillData.m_UIEffectGroupList;
                uiEffectGroups.ApplyAllItem(p =>
                {
                    GameObject effect = (GameObject)GameObject.Instantiate(p._UIEffectPrefab);
                    effect.transform.parent = BattleManager.Instance.transform.parent;
                    effect.transform.localScale = Vector3.one;
                    effect.transform.localPosition = p._EffectStartPos;
                    var destroySelf = effect.AddComponent<DestroySelf>();
                    destroySelf.Time = p._EffectDuration;
                });


                m_isCreateUIEffect = false;
            }
           
        }
        */
    }

    public override void DoBeforeEntering()
    {
        //取消技能选择
        this.m_PlayerBehaviour.LeaveInitiativeSkillSelectedState();

        if (this.m_PlayerBehaviour.IsHero)
        {
            if (this.m_PlayerBehaviour.FSMSystem.IsStateBreak())
            {
                this.m_PlayerBehaviour.RaiseEvent(EventTypeEnum.SkillBreakForStatistics.ToString(), null);
                //TraceUtil.Log(SystemModel.Rocky,"技能被打断统计加一");
            }
            
            //在这给服务器发打断指令
            this.m_PlayerBehaviour.BreakSkill();

            this.m_PlayerBehaviour.GetSkillBySkillID(this.m_PlayerBehaviour.m_nextSkillID);
        }
       
        /*找到当前选择的技能
          * 施放技能
          * 监听技能Action委托，获得技能数据，并保存起来。
          * 在Act处理里将使用这些技能数据做表现
          * */
        //m_playerStartPosition =this.m_roleBehaviour.transform.position;
        if (m_PlayerBehaviour.IsCopy)
        {
            m_playerStartPosition = m_hero.transform.position;

            m_touchPoint = ((PlayerInitiativeSkillFireState)m_hero.GetComponent<PlayerBehaviour>().FSMSystem.FindState(global::StateID.PlayerInitiativeSkill)).m_touchPoint;
        }
        else
        {
            m_playerStartPosition = this.m_roleBehaviour.transform.position;
        }
        //add at 2014-10-09 for 20141008 skill modify        
        m_skillBase = m_PlayerBehaviour.SelectedSkillBase;
        
        m_skillBase.AddActionDelegate(ActionHandler);
		m_skillBase.AddSkillOverDelegate(SkillOverHandler);
        IsSkillBeBreaked = false;
        if (this.m_PlayerBehaviour.IsHero)
        {
            //for 20101008 skill modify
            if (m_skillBase.SkillData.AutoDirecting)
            {
                var lockMonster=MonsterManager.Instance.LockedMonster ;
                if (lockMonster != null)
                {
                    var targetPos=lockMonster.Behaviour.transform.position;
                    Vector3 lookAt=new Vector3(targetPos.x, m_PlayerBehaviour.ThisTransform.position.y, targetPos.z);
                    m_PlayerBehaviour.ThisTransform.LookAt(lookAt);
                }
            }
            SendNormalSkillCommand(m_skillBase.SkillId);
        }
        m_skillBase.Fire();
        IsPlaySkillCamera(m_skillBase.SkillData);
		m_PlayerBehaviour.ClientMove = false;

        //UI特效
        /*
        var uiEffectGroups = m_skillBase.SkillData.m_UIEffectGroupList;
        if (uiEffectGroups.Count > 0)
        {
            m_delayCreateUIEffectTime = uiEffectGroups[0]._EffectStartTime;
            m_isCreateUIEffect = true;
        }     
        */
    }

    /// <summary>
    /// 判断是否播放技能摄像机
    /// </summary>
    /// <param name="cameraData"></param>
    private void IsPlaySkillCamera(SkillConfigData skillData)
    {
		if (!m_PlayerBehaviour.IsHero)
			return;

        if(skillData.cameraIdList.Length == 1 && skillData.cameraIdList[0] == 0)
        {
            return;
        }

        Vector3 cameraPos = this.m_roleBehaviour.transform.position + skillData.cameraRangeOffset;
        var monsterList = MonsterManager.Instance.GetMonstersList();

		if(skillData.skillCameraRange == 0)
		{
			if (m_skillCamera == null)
			{
				m_skillCamera = CreateSkillCamera();
				m_skillCamera.gameObject.AddComponent<SkillCamera>();
			}
			
			m_skillCamera.GetComponent<SkillCamera>().InitCameraData(skillData.cameraIdList);

		}
		else
		{


			foreach (var item in monsterList)
			{
			    var distance = Vector3.Distance(item.GO.transform.position, cameraPos);
					if ( skillData.skillCameraRange >= distance)
			    {
			        if (m_skillCamera == null)
			        {
			            m_skillCamera = CreateSkillCamera();
						m_skillCamera.gameObject.AddComponent<SkillCamera>();
					}
						
					m_skillCamera.GetComponent<SkillCamera>().InitCameraData(skillData.cameraIdList);
					break;
			        
			    }
			}
		}
    }

    /// <summary>
    /// 创建技能摄像机
    /// </summary>
    /// <returns></returns>
    private Camera CreateSkillCamera()
    {
        Camera camera = Camera.Instantiate(Camera.main) as Camera;
        //BattleManager.Instance.FollowCamera.gameObject.SetActive(false);
		BattleManager.Instance.FollowCamera.camera.enabled = false;
        camera.GetComponent<FollowCamera>().enabled = false;
        camera.depth = 2;

        return camera;
    }

    public void ActionHandler(SkillActionData actData)
    {
		m_PlayerBehaviour.Invincible = (actData.m_invincible == 1);
		m_PlayerBehaviour.IronBody = (actData.m_ironBody == 1);
        m_isAnimPlayed = false;
        m_lockedTarget = null;
        m_actData = (SkillActionData)actData.Clone();
        ((PlayerBehaviour)this.m_roleBehaviour).ChangeDisplayState(m_actData.m_moveType);
        //2014-10-09 普通攻击的锁定目标计算规则有修改
        //if (actData.m_ani_followtype == 1 
        //    && m_PlayerBehaviour.ActionLockTarget!=null
        //    && m_PlayerBehaviour.ActionLockTarget.GO!=null
        //    && CheckAtAttackRange(m_PlayerBehaviour.ActionLockTarget))
        if (actData.m_ani_followtype == 6  //定速到达
            && m_PlayerBehaviour.ActionLockTarget != null
            && m_PlayerBehaviour.ActionLockTarget.GO != null)
        {
            m_lockedTarget = m_PlayerBehaviour.ActionLockTarget;
            var targetPosition = m_PlayerBehaviour.ActionLockTarget.GO.transform.position;
            m_PlayerBehaviour.ThisTransform.LookAt(new Vector3(targetPosition.x, m_PlayerBehaviour.ThisTransform.position.y, targetPosition.z));

        }
        else
        {
            m_PlayerBehaviour.ThisTransform.Rotate(0, m_actData.m_startAngle, 0);
        }
        m_attackDire = Quaternion.Euler(0, m_actData.m_angle, 0) * Vector3.forward;
        //影子玩家参考主角方向
        if (m_PlayerBehaviour.IsCopy)
            m_attackDire = m_hero.transform.TransformDirection(m_attackDire);
        else
            m_attackDire = m_PlayerBehaviour.ThisTransform.TransformDirection(m_attackDire);

        //判断Action的移动类型是否必达，如果是，则判断是为点还是可移动目标（目前只有点）。计算移动速度
        if (m_skillBase.SkillData.m_directionParam == 2 && m_actData.m_ani_followtype == 3)
        {
            m_PlayerBehaviour.ChangeForward(m_touchPoint);
            dis = Vector3.Distance(m_playerStartPosition, m_touchPoint);
            s = dis / (m_actData.m_duration / 1000.0f);
            s = s * Time.deltaTime;
        }
        //技能施放位置修正
        Vector3 correctPos = new Vector3(m_actData.m_startPos.x, 0, m_actData.m_startPos.y);
        correctPos = m_PlayerBehaviour.ThisTransform.TransformDirection(correctPos);
        m_PlayerBehaviour.ThisTransform.Translate(correctPos);
        this.IsStateReady = true;        
    }
	
	public void SkillOverHandler()
    {
        m_PlayerBehaviour.GetSkillBySkillID(PlayerDataManager.Instance.GetBattleItemData(this.m_PlayerBehaviour.PlayerKind).NormalSkillID[0]);
        OnChangeTransition(Transition.PlayerToIdle);
    }
    public override void DoBeforeLeaving()
    {        
		m_PlayerBehaviour.Invincible = false;
		m_PlayerBehaviour.IronBody = false;
		m_skillBase.RemoveActionDelegate(ActionHandler);
        m_skillBase.DeleteSkillOverDelegate(SkillOverHandler);
        m_skillBase.BreakSkill();
        /*if (m_skillCamera != null)
            m_skillCamera.GetComponent<SkillCamera>().OnDestroy();*/
        //技能结束，必须保证角色显身
        m_PlayerBehaviour.ChangeDisplayState(1);
        //m_PlayerBehaviour.ChangeDisplayState(1);
		
		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
			float xValue,yValue;
        	this.m_PlayerBehaviour.ThisTransform.position.SetToServer(out xValue, out yValue);
			SMsgFightCommand_CS sMsgFightCommand_CS = new SMsgFightCommand_CS();
			sMsgFightCommand_CS.uidFighter = this.m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
			sMsgFightCommand_CS.nFightCode = m_skillBase.SkillId;
			sMsgFightCommand_CS.byType = 0;
			sMsgFightCommand_CS.fighterPosX = xValue;
			sMsgFightCommand_CS.fighterPosY = yValue;
			
			NetServiceManager.Instance.BattleService.SendFightCommandCS(sMsgFightCommand_CS);
		}
		
        base.DoBeforeLeaving();
    }
	
	private void SendNormalSkillCommand(int skillId)
    {
        Int64 targetEntityId = 0;
        if(m_skillBase.SkillData.IsLockTarget)
        {
            //计算锁定目标，保存在PlayerBehaviour一个变量，并把目标发给服务器端
            var targetEntityModel = LockAttackMonster(this.m_attackDistance, this.m_attackAngle);
            if (targetEntityModel != null)
            {
                m_PlayerBehaviour.ActionLockTarget = targetEntityModel;
                targetEntityId = targetEntityModel.EntityDataStruct.SMsg_Header.uidEntity;
            }
        }
		
		float xValue,yValue;
        this.m_PlayerBehaviour.ThisTransform.position.SetToServer(out xValue, out yValue);
		if(GameManager.Instance.CurrentGameMode == GameMode.MULTI_PLAYER)
		{
		
	        SMsgBattleCommand sMsgBattleCommand = new SMsgBattleCommand();
	        sMsgBattleCommand.uidFighter = this.m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
	        sMsgBattleCommand.uidTarget=targetEntityId;
	        sMsgBattleCommand.nFightCode = skillId;
	       
			
			float xFireValue,yFireValue;
			
			if(2 == m_skillBase.SkillData.m_directionParam)
			{
				this.m_PlayerBehaviour.SkillFirePos.SetToServer(out xFireValue, out yFireValue);
			}
			else 
			{
				this.m_PlayerBehaviour.ThisTransform.position.SetToServer(out xFireValue, out yFireValue);	
			}
	
	        sMsgBattleCommand.xPlayer = xValue;
	        sMsgBattleCommand.yPlayer = yValue;
	        sMsgBattleCommand.xMouse = xFireValue;
	        sMsgBattleCommand.yMouse = yFireValue;
	
	        var euler = this.m_PlayerBehaviour.ThisTransform.rotation.eulerAngles;
	
	        var d = Quaternion.Euler(euler) * Vector3.forward;
	
	        sMsgBattleCommand.xDirect = d.x;
	        sMsgBattleCommand.yDirect = d.z * -1;
	
	        //add by lee        
	        sMsgBattleCommand.bulletIndex = (UInt32)BulletManager.Instance.ReadIndex(this.m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity);
	
	        NetServiceManager.Instance.BattleService.SendBattleCommand(sMsgBattleCommand);
		}
		
		else if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
			SMsgFightCommand_CS sMsgFightCommand_CS = new SMsgFightCommand_CS();
			sMsgFightCommand_CS.uidFighter = this.m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
			sMsgFightCommand_CS.nFightCode = skillId;
			sMsgFightCommand_CS.byType = 1;
			sMsgFightCommand_CS.fighterPosX = xValue;
			sMsgFightCommand_CS.fighterPosY = yValue;
			
			NetServiceManager.Instance.BattleService.SendFightCommandCS(sMsgFightCommand_CS);
		}



        //\edit by lee
        //SMsgActionClientOptMove_CS sMsgActionClientOptMove = new SMsgActionClientOptMove_CS();
        //sMsgActionClientOptMove.dwMapID = GameManager.Instance.GetCurSceneMapID;
        //sMsgActionClientOptMove.dwPathLen = 1;
        //NetServiceManager.Instance.BattleService.SendClientOptMoveCommand(sMsgActionClientOptMove);
    }

    /// <summary>
    /// 处理在施放技能中，还能接受用户输入，并改变攻击方向
    /// </summary>
    /// <param name="point"></param>
    public void OnTouch(Vector3 point)
    {
        if (m_actData.m_ani_followtype == 4)
        {           
                m_touchPoint = point;

                var heroPos=m_PlayerBehaviour.IsCopy?m_hero.transform.position:m_PlayerBehaviour.ThisTransform.position;

                var newDirect = (m_touchPoint -heroPos).normalized;

                SMsgFightChangeDirect_CS sMsgFightChangeDirect_CS = new SMsgFightChangeDirect_CS();
                sMsgFightChangeDirect_CS.DirX = newDirect.x;
                sMsgFightChangeDirect_CS.DirY = newDirect.z * -1;

                NetServiceManager.Instance.BattleService.SendFightChangeDirectCommand(sMsgFightChangeDirect_CS);

                m_attackDire = newDirect;
        }
    }   

    void CheckJoyStickInSkillFire()
    {
        if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
        {
            if (m_actData.m_ani_followtype == 4)
            {
                if(m_PlayerBehaviour.IsJoyStickPress && m_PlayerBehaviour.JoyStickDir != Vector3.zero)
                {
                    var newDirect = m_PlayerBehaviour.JoyStickDir.normalized;
                    
                    SMsgFightChangeDirect_CS sMsgFightChangeDirect_CS = new SMsgFightChangeDirect_CS();
                    sMsgFightChangeDirect_CS.DirX = newDirect.x;
                    sMsgFightChangeDirect_CS.DirY = newDirect.z * -1;
                    
                    NetServiceManager.Instance.BattleService.SendFightChangeDirectCommand(sMsgFightChangeDirect_CS);
                    
                    m_attackDire = newDirect;

                }

            }
        }
    }

    /// <summary>
    /// 收到网络端消息，改变动作打击方向
    /// </summary>
    /// <param name="newDirect"></param>
    public void ChangeAttackDirect(Vector3 newDirect)
    {
        if (m_actData.m_ani_followtype == 4)
        {
            m_attackDire = newDirect;
        }
    }
}
