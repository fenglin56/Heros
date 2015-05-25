using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 被动技能（普通技能）施放
/// </summary>
public class PlayerNormalSkillFireState : PlayerState
{
    /// <summary>
    /// 普通攻击SkillID
    /// </summary>
    
    private SkillBase m_skillBase;
    private SkillActionData m_actData;
    private Vector3 m_attackDire;  //攻击方向  
	
	private int m_normalAttackStep;
    private EntityModel m_lockedTarget; //技能锁定目标，用于判断普攻突击计算,因为突击开始后不再追击，所以不能实时从PlayerBehaviour取ActionLockTarget
    private Vector3 marchForwardTargetPos;
    private float hitRadius;
    
    public PlayerNormalSkillFireState()
    {
        m_stateID = StateID.PlayerNormalSkill;        
    }
    public override void Reason()
    {
        if (m_PlayerBehaviour.IsCopy) return;
        if (IsStateReady)
        {
            if (IsSkillBeBreaked && this.IsPlayerMoving())
            {
                OnChangeTransition(Transition.PlayerToTarget);
            }
            else if (m_actData!=null
                &&m_actData.m_ani_followtype == 6 
                && m_lockedTarget != null)  //如果是普攻突击，判断是否位移到位，如果到位则开始下一普攻技能
            {
                //Debug.Log("TargetPos:" + marchForwardTargetPos);
                //Debug.Log("Player:" + m_PlayerBehaviour.ThisTransform.position);
                //Debug.Log("Distance:" + Vector3.Distance(m_PlayerBehaviour.ThisTransform.position, marchForwardTargetPos));
                var dis = Vector3.Distance(m_PlayerBehaviour.ThisTransform.position, marchForwardTargetPos) - CommonDefineManager.Instance.CommonDefine.ChargeOffset;
                if (dis <= m_actData.m_startSpeed * Time.deltaTime)
                {
					if(m_normalAttackStep==0)  //如果是第一段突进技能，结束后直接接上第二希技能。
					{
						m_PlayerBehaviour.m_keepNormalSkillStep=true;
						m_normalAttackStep++;
						DoBeforeEntering();
						m_PlayerBehaviour.m_keepNormalSkillStep=false;
					}
                    //SkillOverHandler();
                }
            }
        }
    }

    public override void Act()
    {
        if (IsStateReady)
        {
            float s = 0;
            //位移
            if (m_actData.m_ani_followtype == 6)  //定速到达型
            {
                s = m_actData.m_startSpeed * Time.deltaTime;
            }
            else
            {
                var t = Time.deltaTime;
                s = m_actData.m_startSpeed * t + m_actData.m_acceleration * Mathf.Pow(t, 2) * 0.5f;

                m_actData.m_startSpeed += Mathf.FloorToInt(m_actData.m_acceleration * t);

                if (!m_isAnimPlayed || m_roleAnimationComponent[m_actData.m_animationId].wrapMode == WrapMode.Loop)
                {
                    m_roleAnimationComponent.Play(m_actData.m_animationId);
                    m_isAnimPlayed = true;
                }
            }
            //Debug.Log("m_attackDire:" + m_attackDire+"  speed:"+s);
            //Debug.Log("DesDir:" +  (marchForwardTargetPos- m_PlayerBehaviour.transform.position).normalized + "  speed:" + s);
            var motion = m_attackDire * s;
            //if (!SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
            //{
            //    m_PlayerBehaviour.HeroCharactorController.Move(motion);
            //}   

			if(m_actData.m_ani_followtype == 6 && SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
			{
				m_PlayerBehaviour.GetSkillBySkillID(PlayerDataManager.Instance.GetBattleItemData(this.m_PlayerBehaviour.PlayerKind).NormalSkillID[0]);
				OnChangeTransition(Transition.PlayerToIdle);
				return;
			}

            this.m_PlayerBehaviour.MoveTo(motion);

        }
    }
    public override void DoBeforeEntering()
    {
        /*找到当前选择的技能
         * 施放技能
         * 监听技能Action委托，获得技能数据，并保存起来。
         * 在Act处理里将使用这些技能数据做表现
         * */
		if(m_PlayerBehaviour.IsHero)
		{
			if(!m_PlayerBehaviour.m_keepNormalSkillStep)
			{
				m_normalAttackStep = 0;
			}
		}
        //普攻技能步骤过滤 如果是突进，判断是否突进技能CD完毕，如果完毕则正常，否则跳过两个技能。
        if (this.m_PlayerBehaviour.IsHero&&m_normalAttackStep == 0)
        {
            if (m_PlayerBehaviour.MarchForwardCDTime > 0
                || MonsterManager.Instance.LockedMonster==null
                || Vector3.Distance(m_PlayerBehaviour.ThisTransform.position,MonsterManager.Instance.LockedMonster.GO.transform.position)<=CommonDefineManager.Instance.CommonDefine.ChargeMinRange)  // still colddowning or no target
            {
                m_normalAttackStep += 2;  // skip march forward skill
            }
        }
        SelectNormalSkill();
        //Debug.Log("SKill Id:" + m_skillBase.SkillId+" AttackStep:"+m_normalAttackStep);
        IsSkillBeBreaked = false;  
        if (this.m_PlayerBehaviour.IsHero)
        {
            SendNormalSkillCommand(m_skillBase.SkillId);
        }
        if (this.m_PlayerBehaviour.IsHero && m_normalAttackStep == 0)
        {
            m_PlayerBehaviour.MarchForwardCDTime = m_skillBase.SkillData.m_coolDown;  // reset colddown time
        }
        m_skillBase.Fire();

		m_PlayerBehaviour.ClientMove = false;
		
    }
    public void ActionHandler(SkillActionData actData)
    {
		m_PlayerBehaviour.Invincible = (actData.m_invincible == 1);
		m_PlayerBehaviour.IronBody = (actData.m_ironBody == 1);
        m_isAnimPlayed = false;
        m_lockedTarget = null;
        marchForwardTargetPos = m_PlayerBehaviour.ThisTransform.position;
        hitRadius = 0;
        //Debug.Log("Action Start Time:" + Time.realtimeSinceStartup);
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
            marchForwardTargetPos = new Vector3(m_lockedTarget.GO.transform.position.x, m_PlayerBehaviour.ThisTransform.position.y, m_lockedTarget.GO.transform.position.z);
            m_PlayerBehaviour.ThisTransform.LookAt(marchForwardTargetPos);
            //hitRadius = ((MonsterBehaviour)m_lockedTarget.Behaviour).m_MonsterConfigData._hitRadius;
            hitRadius = CommonDefineManager.Instance.CommonDefine.ChargeOffset;
        }
        else
        {
            m_PlayerBehaviour.ThisTransform.Rotate(0, actData.m_startAngle, 0);
        }
        m_actData = (SkillActionData)actData.Clone();
        m_attackDire = Quaternion.Euler(0, m_actData.m_angle, 0) * Vector3.forward;
       //影子玩家参考主角方向
        if (m_PlayerBehaviour.IsCopy) 
            m_attackDire = m_hero.transform.TransformDirection(m_attackDire);
        else
            m_attackDire = m_PlayerBehaviour.ThisTransform.TransformDirection(m_attackDire);



        this.IsStateReady = true;        
    }
    public void SkillOverHandler()
    {
        if(!PlayerManager.Instance.NormalAttackRemembering)
        {
		    m_PlayerBehaviour.NormalAttackButtonPress = false;
        }

		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
			float xValue,yValue;        	this.m_PlayerBehaviour.ThisTransform.position.SetToServer(out xValue, out yValue);
			SMsgFightCommand_CS sMsgFightCommand_CS = new SMsgFightCommand_CS();
			sMsgFightCommand_CS.uidFighter = this.m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
			sMsgFightCommand_CS.nFightCode = m_skillBase.SkillId;
			sMsgFightCommand_CS.byType = 0;
			sMsgFightCommand_CS.fighterPosX = xValue;
			sMsgFightCommand_CS.fighterPosY = yValue;
			
			NetServiceManager.Instance.BattleService.SendFightCommandCS(sMsgFightCommand_CS);
		}
		
		if(!GameManager.Instance.UseJoyStick)
		{
			if(m_PlayerBehaviour.LastWalkToPosition != null)
			{
				m_PlayerBehaviour.WalkToPosition = m_PlayerBehaviour.LastWalkToPosition;
				IsSkillBeBreaked = true;
				m_normalAttackStep++;
				if(m_normalAttackStep > 5)
				{
					m_normalAttackStep = 0;
				}
	        	OnChangeTransition(Transition.PlayerToTarget);
				return;	
			}
		}
		else
		{
			if(!m_PlayerBehaviour.NormalAttackButtonPress)
			{
				IsSkillBeBreaked = true;
				m_normalAttackStep++;
				if(m_normalAttackStep > 5)
				{
					m_normalAttackStep = 0;
				}
	        	OnChangeTransition(Transition.PlayerToIdle);
				return;
			}
		}
		
		
		if(m_normalAttackStep == 5)
		{
			m_normalAttackStep = 0;
        	IsSkillBeBreaked = true;

			if(!GameManager.Instance.UseJoyStick)
			{
				m_PlayerBehaviour.WalkToPosition = m_PlayerBehaviour.ThisTransform.TransformPoint(Vector3.forward);
				OnChangeTransition(Transition.PlayerToTarget);
			}
			else
			{
        		OnChangeTransition(Transition.PlayerToIdle);
			}
			return;
		}
		m_normalAttackStep++;

        SelectNormalSkill();

		if (this.m_PlayerBehaviour.IsHero)
        {
            SendNormalSkillCommand(m_skillBase.SkillId);
        }
		if(GameManager.Instance.UseJoyStick)
		{
			if(m_PlayerBehaviour.IsJoyStickPress && m_PlayerBehaviour.JoyStickDir != Vector3.zero)
			{
				Vector3 lootToPos = m_PlayerBehaviour.ThisTransform.position + m_PlayerBehaviour.JoyStickDir;
				m_PlayerBehaviour.ChangeForward(lootToPos);
			}
		}
        m_skillBase.Fire();
		m_PlayerBehaviour.ClientMove = false;
    }
    /// <summary>
    /// 选择普通攻击的技能，【向前还是向后】
    /// </summary>
    /// <param name="step"></param>
    private void SelectNormalSkill()
    {
        int step = m_normalAttackStep;
        if (m_skillBase != null)
        {
            m_skillBase.RemoveActionDelegate(ActionHandler);
            m_skillBase.DeleteSkillOverDelegate(SkillOverHandler);
        }
        //检测的距离和角度
        var detectDistance = CommonDefineManager.Instance.CommonDefine.DetectDistance[0];
        var detectAngle = CommonDefineManager.Instance.CommonDefine.DetectDistance[1];
        var monsters = MonsterManager.Instance.SearchBeAttackedMonster(this.m_PlayerBehaviour.transform.position, detectDistance, m_attackDire, detectAngle);

        var triggerCollidder=monsters.Any(P=>
            {
                var distance=Vector3.Distance(P.Behaviour.transform.position,this.m_PlayerBehaviour.ThisTransform.position);
                return ((MonsterBehaviour)P.Behaviour).m_MonsterConfigData._hitRadius >= distance;
            });
        //TraceUtil.Log("SkillStep:" + step + "  ISBAckup:" + triggerCollidder);
        //TraceUtil.Log("SkillID:" + PlayerDataManager.Instance.GetBattleItemData(this.m_PlayerBehaviour.PlayerKind).GetPlayerNormalSkillId(step, triggerCollidder));
        int skillId = PlayerDataManager.Instance.GetBattleItemData(this.m_PlayerBehaviour.PlayerKind).GetPlayerNormalSkillId(step, triggerCollidder);
        //Debug.Log("skillId:" + skillId + "  step:" + step);
        this.m_PlayerBehaviour.GetSkillBySkillID(skillId);
        //TraceUtil.Log("skillbase is null?:" + (this.m_PlayerBehaviour.SelectedSkillBase == null));
        m_skillBase = this.m_PlayerBehaviour.SelectedSkillBase;
        m_skillBase.AddActionDelegate(ActionHandler);
        m_skillBase.AddSkillOverDelegate(SkillOverHandler);
    }
    
    public override void DoBeforeLeaving()
    {
        //Debug.Log("Skill Over Time[DoBeforeLeaving]:" + Time.realtimeSinceStartup);
        m_lockedTarget = null;
        marchForwardTargetPos = m_PlayerBehaviour.ThisTransform.position;
        hitRadius = 0;
		m_PlayerBehaviour.Invincible = false;
		m_PlayerBehaviour.IronBody = false;
        m_skillBase.RemoveActionDelegate(ActionHandler);
        m_skillBase.DeleteSkillOverDelegate(SkillOverHandler);
        IsSkillBeBreaked = true;
        m_skillBase.BreakSkill();
		
		m_PlayerBehaviour.StartCountResetNormalSkillStep();
        base.DoBeforeLeaving();
    }    
    private void SendNormalSkillCommand(int skillId)
    {
        Int64 targetEntityId = 0;
        //if (m_skillBase.SkillData.IsLockTarget)  //20141008 技能细节修改。动作的followType=1 为跟随目标的动作不再有用，而技能的AutoDirecting判断
        if (m_skillBase.SkillData.AutoDirecting)
        {
            //计算锁定目标，保存在PlayerBehaviour一个变量，并把目标发给服务器端
            //2014-10-09 普通攻击的锁定目标计算规则有修改
            //var targetEntityModel = LockAttackMonster(this.m_attackDistance, this.m_attackAngle);
            var targetEntityModel = MonsterManager.Instance.LockedMonster;
            if (targetEntityModel != null)
            {
                m_PlayerBehaviour.ActionLockTarget = targetEntityModel;
                targetEntityId = targetEntityModel.EntityDataStruct.SMsg_Header.uidEntity;

                m_PlayerBehaviour.ThisTransform.LookAt(new Vector3(targetEntityModel.GO.transform.position.x, m_PlayerBehaviour.ThisTransform.position.y, targetEntityModel.GO.transform.position.z));
            }
        }
		
		float xValue,yValue;
        this.m_PlayerBehaviour.ThisTransform.position.SetToServer(out xValue, out yValue);
		
		if(GameManager.Instance.CurrentGameMode == GameMode.MULTI_PLAYER)
		{
	        SMsgBattleCommand sMsgBattleCommand = new SMsgBattleCommand();
	        sMsgBattleCommand.uidFighter = this.m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
	        sMsgBattleCommand.uidTarget = targetEntityId;
	        sMsgBattleCommand.nFightCode = skillId;
	        
	        sMsgBattleCommand.xPlayer = xValue;
	        sMsgBattleCommand.yPlayer = yValue;
	        sMsgBattleCommand.xMouse= xValue;
	        sMsgBattleCommand.yMouse = yValue;
	        //var dire = this.m_PlayerBehaviour.ThisTransform.TransformDirection(this.m_PlayerBehaviour.ThisTransform.forward);
	        var euler= this.m_PlayerBehaviour.ThisTransform.rotation.eulerAngles;
	        var d = Quaternion.Euler(euler) * Vector3.forward;
	  
	        sMsgBattleCommand.xDirect = d.x;
	        sMsgBattleCommand.yDirect = d.z*-1;
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
    }   

}