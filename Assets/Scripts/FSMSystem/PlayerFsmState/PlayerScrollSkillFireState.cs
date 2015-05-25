using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 被动技能（翻滚技能）施放
/// </summary>
public class PlayerScrollSkillFireState : PlayerState
{
     private SkillBase m_skillBase;
    private SkillActionData m_actData;
    private Vector3 m_scrollDire;  //攻击方向    
    public PlayerScrollSkillFireState()
    {
        m_stateID = StateID.PlayerScrollSkill;
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
        }
    }

    public override void Act()
    {
        if (IsStateReady)
        {
            //位移
            var t = Time.deltaTime;
            var s = m_actData.m_startSpeed * t + m_actData.m_acceleration * Mathf.Pow(t, 2) * 0.5f;

            m_actData.m_startSpeed += Mathf.FloorToInt(m_actData.m_acceleration * t);

            m_roleAnimationComponent.Play(m_actData.m_animationId);

            var motion = m_scrollDire * s;
            //if (!SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
            //{
            //    m_PlayerBehaviour.HeroCharactorController.Move(m_scrollDire * s);
            //}
            this.m_PlayerBehaviour.MoveTo(motion);
            
            //TraceUtil.Log("滚动中");
        }
    }
    public override void DoBeforeEntering()
    {
        /*找到当前选择的技能
         * 施放技能
         * 监听技能Action委托，获得技能数据，并保存起来。
         * 在Act处理里将使用这些技能数据做表现
         * */
        this.m_PlayerBehaviour.GetSkillBySkillID(PlayerDataManager.Instance.GetBattleItemData(this.m_PlayerBehaviour.PlayerKind).ScrollSkillID); 
        m_skillBase = this.m_PlayerBehaviour.SelectedSkillBase;
        m_skillBase.AddActionDelegate(ActionHandler);
        m_skillBase.AddSkillOverDelegate(SkillOverHandler);
        IsSkillBeBreaked = false;
        if (this.m_PlayerBehaviour.IsHero)
        {
            SendNormalSkillCommand(m_skillBase.SkillId);

        }
        m_skillBase.Fire();
		//m_PlayerBehaviour.ClientEndPos = null;
		m_PlayerBehaviour.ClientMove = false;
    }
    public void ActionHandler(SkillActionData actData)
    {
		m_PlayerBehaviour.Invincible = (actData.m_invincible == 1);
		m_PlayerBehaviour.IronBody = (actData.m_ironBody == 1);
        m_actData = (SkillActionData)actData.Clone();
        m_scrollDire = Quaternion.Euler(0, m_actData.m_angle, 0) * Vector3.forward;
        //影子玩家参考主角方向
        if (m_PlayerBehaviour.IsCopy)
            m_scrollDire = m_hero.transform.TransformDirection(m_scrollDire);
        else
            m_scrollDire = m_PlayerBehaviour.ThisTransform.TransformDirection(m_scrollDire);
        //TraceUtil.Log("开始滚动技能");
        this.IsStateReady = true;
    }
    public void SkillOverHandler()
    {
        IsSkillBeBreaked = true;
        OnChangeTransition(Transition.PlayerToIdle);
    }
    public override void DoBeforeLeaving()
    {
		m_PlayerBehaviour.Invincible = false;
		m_PlayerBehaviour.IronBody = false;
        m_skillBase.RemoveActionDelegate(ActionHandler);
        m_skillBase.DeleteSkillOverDelegate(SkillOverHandler);
        IsSkillBeBreaked = true;
        m_skillBase.BreakSkill();
		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
			float xValue, yValue;
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
		float xValue, yValue;
        this.m_PlayerBehaviour.ThisTransform.position.SetToServer(out xValue, out yValue);
		if(GameManager.Instance.CurrentGameMode == GameMode.MULTI_PLAYER)
		{
	        SMsgBattleCommand sMsgBattleCommand = new SMsgBattleCommand();
	        sMsgBattleCommand.uidFighter = this.m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
	        sMsgBattleCommand.nFightCode = skillId;
	       
	
	        sMsgBattleCommand.xPlayer = xValue;
	        sMsgBattleCommand.yPlayer = yValue;
	        sMsgBattleCommand.xMouse = xValue;
	        sMsgBattleCommand.yMouse = yValue;
	
	        //var dire = this.m_PlayerBehaviour.ThisTransform.TransformDirection(this.m_PlayerBehaviour.ThisTransform.forward);
	
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
    }
}
