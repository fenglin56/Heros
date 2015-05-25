using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;

public class MonsterAttackState : MonsterFsmState
{
    private string m_currentSkillAniName;
    private int m_currentSkillID;
    private Vector3 m_shouldPos = Vector3.zero;
	private Vector3 m_actionBeginPos = Vector3.zero;
	
    private Vector3 m_targetPos = Vector3.zero;
    private Vector3 m_rangeTypePos = Vector3.zero;
    private Vector3 m_attackLookAtPos = Vector3.zero;
    private Int64 m_FightTargetID;

    private SkillBase m_currentSkillBase;
    private SkillActionData m_currentActionData;

    private Vector3 m_attackDirt = Vector3.zero;
    private float m_attackDurTime = 0;

    private float m_delayFireTime;
    private float m_currentAngle;

    //位移
    private float m_displacement = 0;
    //瞬间到达
    private bool m_isMomentArrives = false;
    //怪物高度
    private float m_high = 0;


    public MonsterAttackState()
    {
        this.m_stateID = StateID.MonsterAttack;
    }

    public override void Reason()
    {
        if (!IsStateReady)
            return;
    }
	
	public SkillBase CurrentSkillBase
	{
		get { return m_currentSkillBase;}	
	}

    public override void Act()
    {
        if (!IsStateReady)
            return;

        #region hxz

        if (m_currentActionData == null)
        {
            //TraceUtil.Log("current actionData is null");
            return;
        }

        float dis = 0;

        if ( m_currentActionData.m_ani_followtype == 3)
        {
			dis = Vector3.Distance(m_actionBeginPos, m_targetPos);
			if(m_currentActionData.m_duration > 0)
			{
				m_displacement = dis / (m_currentActionData.m_duration / 1000.0f);
			}

			m_displacement = m_displacement * Time.deltaTime;             
        }
        else if (m_currentActionData.m_ani_followtype == 5)
        {
			if (m_currentActionData.m_duration > 0)
			{
				dis = Vector3.Distance(m_actionBeginPos, m_targetPos);
				m_displacement = dis / (m_currentActionData.m_duration / 1000.0f);
				m_displacement = m_displacement * Time.deltaTime;
			}   
        }
        else
        {
            var t = Time.deltaTime;
            m_displacement = m_currentActionData.m_startSpeed * t + m_currentActionData.m_acceleration * t * t * 0.5f;

            m_currentActionData.m_startSpeed += m_currentActionData.m_acceleration * t;
        }
        
        #endregion

        //TraceUtil.Log("Speed:" + s + " Dis:" + dis);
        //m_roleAnimationComponent.Play(m_actData.m_animationId);
        //\避免后退
        
        
        
        if (m_displacement > 0)
        {            
         	//Editor by lee
			if(m_displacement > 5 && !m_currentActionData.IsIgnoreBlock)//做细分处理
			{
				var segmentation = CountSegmentation(m_displacement, m_attackDirt);
				m_MonsterBehaviour.AttackToPoint(this.m_MonsterBehaviour.ThisTransform.position + segmentation);
			}
			else
			{		
				var translateDis = m_attackDirt * m_displacement;
				if (!SceneDataManager.Instance.IsPositionInBlock(this.m_MonsterBehaviour.ThisTransform.position + translateDis))
				{
					m_MonsterBehaviour.AttackToPoint(this.m_MonsterBehaviour.ThisTransform.position + translateDis);
				} 
			}
        }            
        

    }

	private Vector3 CountSegmentation(float displacement, Vector3 dirt)
	{
		Vector3 thisPos = this.m_MonsterBehaviour.ThisTransform.position;
		int n = (int)(displacement/5) + 1;
		float dis = displacement/n;
		Vector3 segmentation = dis * dirt;
		//Vector3 countSegmentation = Vector3.zero;
		for(int i=1;i<n;i++)
		{
			Vector3 pos = thisPos + segmentation * i;
			if(SceneDataManager.Instance.IsPositionInBlock(pos))
		    {
				return segmentation * (i-1);
			}
		}
		return segmentation * n;
	}

    public override void DoBeforeEntering()
    {
		//m_MonsterBehaviour.ClientEndPos.Value = null;
        m_currentSkillBase = this.m_MonsterBehaviour.m_SkillBaseList.FirstOrDefault(p => p.SkillId == m_currentSkillID);
        if (m_currentSkillBase == null)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到相关技能");
			TraceUtil.Log(SystemModel.NotFoundInTheDictionary,TraceLevel.Error,"找不到相关技能");
        }
        else
        {
            m_currentSkillBase.AddActionDelegate(ActionHandle);
            m_currentSkillBase.AddSkillBulletFireDelegate(FireBulletHandle);
            m_currentSkillBase.AddSkillEffectFireDelegate(FireSkillActionEffect);
            m_currentSkillBase.AddSkillOverDelegate(ActionOverHandle);
            m_currentSkillBase.Fire();
        }
                

        if (Vector3.Distance(m_MonsterBehaviour.ThisTransform.position, m_shouldPos) >= 10)
        {
            //相差太大的情况:
            m_MonsterBehaviour.MoveToPoint(m_shouldPos);
        }
        else
        {
            m_MonsterBehaviour.MoveToPoint(m_shouldPos);
        }

        m_MonsterBehaviour.ThisTransform.LookAt(new Vector3(m_attackLookAtPos.x, m_MonsterBehaviour.ThisTransform.position.y, m_attackLookAtPos.z));

        //清零位移
        m_displacement = 0;

        //清除路点信息
        m_MonsterBehaviour.PointQueue.Clear();
		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
			float xValue,yValue;
        	this.m_MonsterBehaviour.ThisTransform.position.SetToServer(out xValue, out yValue);
			SMsgFightCommand_CS sMsgFightCommand_CS = new SMsgFightCommand_CS();
			sMsgFightCommand_CS.uidFighter = this.m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity;
			sMsgFightCommand_CS.nFightCode = m_currentSkillBase.SkillId;
			sMsgFightCommand_CS.byType = 1;
			sMsgFightCommand_CS.fighterPosX = xValue;
			sMsgFightCommand_CS.fighterPosY = yValue;
			
			NetServiceManager.Instance.BattleService.SendFightCommandCS(sMsgFightCommand_CS);
		}
        m_MonsterBehaviour.FixMonsterYToOrigin();
        base.DoBeforeEntering();
    }

    public override void DoBeforeLeaving()
    {        
		m_MonsterBehaviour.Invincible = false;
		m_MonsterBehaviour.IronBody = false;
        if (m_currentSkillBase != null)
        {            
            m_currentSkillBase.RemoveActionDelegate(ActionHandle);
            m_currentSkillBase.RemoveSkillBulletFireDelegate(FireBulletHandle);
            m_currentSkillBase.RemoveSkillEffectFireDelegate(FireSkillActionEffect);
            m_currentSkillBase.DeleteSkillOverDelegate(ActionOverHandle);

            m_currentSkillBase.BreakSkill();
        }        
        //保证显身
		
		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
			float xValue,yValue;
        	this.m_MonsterBehaviour.ThisTransform.position.SetToServer(out xValue, out yValue);
			SMsgFightCommand_CS sMsgFightCommand_CS = new SMsgFightCommand_CS();
			sMsgFightCommand_CS.uidFighter = this.m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity;
			sMsgFightCommand_CS.nFightCode = m_currentSkillBase.SkillId;
			sMsgFightCommand_CS.byType = 0;
			sMsgFightCommand_CS.fighterPosX = xValue;
			sMsgFightCommand_CS.fighterPosY = yValue;
			
			NetServiceManager.Instance.BattleService.SendFightCommandCS(sMsgFightCommand_CS);
		}
		
        m_MonsterBehaviour.ChangeDisplayState(1);        
        base.DoBeforeLeaving();
    }

    void FireBulletHandle(int bulletID, bool useFirePos)
    {
		SMsgPropCreateEntity_SC_Monster mData = (SMsgPropCreateEntity_SC_Monster)m_MonsterBehaviour.EntityModel.EntityDataStruct;
		CampType ct = (CampType)(mData.MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY);

        if (useFirePos)
        {
			BulletFactory.Instance.CreateBullet(bulletID, m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity, m_MonsterBehaviour.ThisTransform, m_rangeTypePos, m_currentSkillBase.SkillData.m_skillId, m_FightTargetID);
        }
        else
        {
			BulletFactory.Instance.CreateBullet(bulletID, m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity, m_MonsterBehaviour.ThisTransform,  m_currentSkillBase.SkillData.m_skillId, m_FightTargetID);
        }
        
    }
    private void FireSkillActionEffect(int actionId, int skillId)
    {
        GameManager.Instance.ActionEffectFactory.CreateActionEffect(actionId, skillId, m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity, m_MonsterBehaviour.ThisTransform);
    }
    void ActionHandle(SkillActionData skillActionData)
    {
        if(skillActionData.m_actionId >= 57000011 && skillActionData.m_actionId <= 57000421)
        {
            float angel  = m_MonsterBehaviour.ThisTransform.eulerAngles.y;
            TraceUtil.Log(SystemModel.Common, TraceLevel.Error, "UID:{0}, 动作ID: {1}, 动作开始角度：{2}", m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity, skillActionData.m_actionId,  angel);
        }

		m_MonsterBehaviour.Invincible = (skillActionData.m_invincible == 1);
		m_MonsterBehaviour.IronBody = (skillActionData.m_ironBody == 1);
        m_currentActionData = (SkillActionData)skillActionData.Clone();
        m_isMomentArrives = false;

        ((MonsterBehaviour)this.m_roleBehaviour).ChangeDisplayState(m_currentActionData.m_moveType);

		switch(m_currentActionData.m_ani_followtype)
		{
		case 1://跟随类型
			TypeID typeID;
			var target = EntityController.Instance.GetEntityModel(m_FightTargetID, out typeID);
			if (target != null)
			{
				Vector3 lookAtPos = new Vector3(target.GO.transform.position.x, m_MonsterBehaviour.ThisTransform.position.y, target.GO.transform.position.z);
				m_MonsterBehaviour.ThisTransform.LookAt(lookAtPos);
			}
			break;
		case 3://
			m_actionBeginPos = m_MonsterBehaviour.ThisTransform.position;
			var player = PlayerManager.Instance.GetEntityMode(m_FightTargetID);
			if (player != null)
			{
				m_targetPos = player.Behaviour.transform.TransformPoint(new Vector3(m_currentActionData.m_followPositionOffset.y, m_MonsterBehaviour.ThisTransform.position.y, m_currentActionData.m_followPositionOffset.x));
				m_targetPos.y = m_high;
				m_MonsterBehaviour.ThisTransform.LookAt(m_targetPos);
			}
			break;
		case 5:
			m_actionBeginPos = m_MonsterBehaviour.ThisTransform.position;
			m_targetPos = new Vector3(skillActionData.m_followPositionOffset.x, m_MonsterBehaviour.ThisTransform.position.y, -skillActionData.m_followPositionOffset.y);
			m_MonsterBehaviour.ThisTransform.LookAt(m_targetPos);
			if (skillActionData.m_duration <= 0)
			{
				m_isMomentArrives = true;

				if (skillActionData.IsIgnoreBlock || !SceneDataManager.Instance.IsPositionInBlock(m_targetPos))
				{
					m_MonsterBehaviour.AttackToPoint(m_targetPos);
				}
			}
			break;
		default:
			//起始转角
			m_MonsterBehaviour.ThisTransform.Rotate(0, skillActionData.m_startAngle, 0);
			break;
		}

		//m_attackDirt = Quaternion.Euler(0,skillActionData.m_angle,0) * m_MonsterBehaviour.ThisTransform.forward;// * Vector3.forward;
		m_attackDirt = m_MonsterBehaviour.ThisTransform.forward;	//位移方向就是怪物朝向

        m_currentSkillAniName = skillActionData.m_animationId;
        if(skillActionData.IsIgnoreBlock)
        {
            m_roleAnimationComponent.Play(m_currentSkillAniName);
        }
        else
        {
            m_roleAnimationComponent.CrossFade(m_currentSkillAniName);
        }

        //技能施放位置修正
        Vector3 correctPos = new Vector3(m_currentActionData.m_startPos.x, 0, m_currentActionData.m_startPos.y);        
        correctPos = m_MonsterBehaviour.ThisTransform.TransformPoint(correctPos);

		bool isPositionInBlock = !skillActionData.IsIgnoreBlock && SceneDataManager.Instance.IsPositionInBlock(correctPos);

		//如果是follow类型为3 ，持续时间为0，则无视移动中的阻挡
		if(!isPositionInBlock && m_currentActionData.m_ani_followtype == 3 && m_currentActionData.m_duration == 0)
		{
			//Vector3 moveToPos = correctPos + Vector3.Distance(m_actionBeginPos, m_targetPos)*m_attackDirt;
			Vector3 moveToPos = (correctPos - m_actionBeginPos) + m_targetPos;
			m_MonsterBehaviour.MoveToPoint(moveToPos);
			return;
		}

		m_MonsterBehaviour.ThisTransform.Rotate(0,m_currentActionData.m_angle, 0);//最后转怪物的朝向，因为在前面会影响到TransformPoint的计算

        //如果进入阻挡
        if (isPositionInBlock)
        {
            Vector3 curPos = m_MonsterBehaviour.ThisTransform.position; //当前位置
            Vector3 move = (curPos - correctPos).normalized * 5;
            float allDistance = Vector3.Distance(curPos, correctPos);
            float moveDistance = move.magnitude;
            int allMoveTime = (int)(allDistance / moveDistance);
            for (int i = 0; i <= allMoveTime; i++)
            {
                if (i == allMoveTime)
                {
                    correctPos = curPos;
                    break;
                }
                if (SceneDataManager.Instance.IsPositionInBlock(correctPos))
                {
                    correctPos += move;
                }
                else
                {
                    break;
                }
            }
        }               

        m_MonsterBehaviour.MoveToPoint(correctPos);
        if(skillActionData.m_actionId >= 57000011 && skillActionData.m_actionId <= 57000421)
        {
            float endangel  = m_MonsterBehaviour.ThisTransform.eulerAngles.y;
            TraceUtil.Log(SystemModel.Common, TraceLevel.Error, "UID:{0}, 动作ID: {1}, 动作结束角度：{2}", m_MonsterBehaviour.RoleDataModel.SMsg_Header.uidEntity, skillActionData.m_actionId,  endangel);
        }

    }

    void ActionOverHandle()
    {
        //if (m_isMomentArrives)//瞬移
        //{
        //    if (!SceneDataManager.Instance.IsPositionInBlock(m_targetPos))
        //    {
        //        m_MonsterBehaviour.ThisTransform.position = m_targetPos;
        //    }
        //}

        OnChangeTransition(Transition.MonsterToIdle);
    }

    public void Attack(SMsgBattleCommand smsgBattleCommand)
    {
		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
			TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"!!!!!!!!!!!!!Wrong Monster battle Command!");	
		}
		
		
        //TraceUtil.Log("==>攻击技能id = "+smsgBattleCommand.nFightCode);

        //var skillConfig = SkillDataManager.Instance.GetSkillConfigData(smsgBattleCommand.nFightCode);   //获取技能配置信息
        m_currentSkillID = smsgBattleCommand.nFightCode;
        m_FightTargetID = smsgBattleCommand.uidTarget;

        m_shouldPos = Vector3.zero.GetFromServer(smsgBattleCommand.xPlayer, smsgBattleCommand.yPlayer);
        m_shouldPos.y = 0;
        m_targetPos = Vector3.zero.GetFromServer(smsgBattleCommand.xMouse, smsgBattleCommand.yMouse);
        m_rangeTypePos = m_targetPos;

        float rad = Mathf.Atan2(-1 * smsgBattleCommand.yDirect, smsgBattleCommand.xDirect);        
        m_currentAngle = 90 - (rad * Mathf.Rad2Deg);

        Vector3 diretPos = new Vector3(smsgBattleCommand.xDirect, m_high, -1 * smsgBattleCommand.yDirect);
        m_attackLookAtPos = m_shouldPos + diretPos;
        
        
        //m_diretPos.Normalize();
        //TraceUtil.Log("monster pos = " + m_shouldPos + " targetPos = " + m_targetPos + " diretPos = " + m_diretPos);
        //rad = m_currentAngle * Mathf.Deg2Rad;
        //m_attackDirt = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
        
        //m_attackDirt = new Vector3(smsgBattleCommand.xDirect, 0, -1 * smsgBattleCommand.yDirect);
        //m_attackDirt.Normalize();
        #region 攻击
        if (Log.IsPrint)
        {
            //Log.Instance.StartLog();
            //Log.Instance.AddLog("99999", "解下发攻击包", smsgBattleCommand.xPlayer.ToString(), smsgBattleCommand.yPlayer.ToString(), smsgBattleCommand.xDirect.ToString(), smsgBattleCommand.yDirect.ToString());
            //Log.Instance.AppendLine();
        }
        #endregion
    }
	
	public void SingleAttack(SMsgFightCommand_SC sMsgFightCommand_SC)
	{
		m_currentSkillID = sMsgFightCommand_SC.nFightCode;
        m_FightTargetID = sMsgFightCommand_SC.uidTarget;

        m_shouldPos = m_MonsterBehaviour.ThisTransform.position;
		
        m_targetPos = Vector3.zero.GetFromServer(sMsgFightCommand_SC.xMouse, sMsgFightCommand_SC.yMouse);
        m_rangeTypePos = m_targetPos;

        float rad = Mathf.Atan2(-1 * sMsgFightCommand_SC.yDirect, sMsgFightCommand_SC.xDirect);        
        m_currentAngle = 90 - (rad * Mathf.Rad2Deg);

        Vector3 diretPos = new Vector3(sMsgFightCommand_SC.xDirect, m_high, -1 * sMsgFightCommand_SC.yDirect);
        m_attackLookAtPos = m_shouldPos + diretPos;
		
	}
}
