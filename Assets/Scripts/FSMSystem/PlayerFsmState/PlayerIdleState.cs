using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 停顿
/// </summary>
public class PlayerIdleState : PlayerState
{
    public PlayerIdleState()
    {
        m_stateID = StateID.PlayerIdle;
    }
    public override void Reason()
    {
        if (m_PlayerBehaviour.IsCopy) 
        {
            return;
        }
		if(BattleManager.Instance != null && BattleManager.Instance.BlockPlayerToIdle)
        {
            return;
        }


        if (IsStateReady)
        {
			if(this.m_PlayerBehaviour.IsHero)
			{
				if( (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN && m_PlayerBehaviour.IsJoyStickPress) || GameManager.Instance.UseJoyStick )
				{
					//摇杆方式//
					if(m_PlayerBehaviour.NormalAttackButtonPress)
					{
						OnChangeTransition(Transition.PlayerFireNormalSkill);	
					}
					else if(m_PlayerBehaviour.IsJoyStickPress && m_PlayerBehaviour.JoyStickDir != Vector3.zero)
					{
						OnChangeTransition(Transition.PlayerToTarget);
					}	
				}
				else
				{
					//触屏方式//
					if (this.IsPlayerMoving())
					{
						OnChangeTransition(Transition.PlayerToTarget);
					}
				}
			}

			else
			{
				if(m_PlayerBehaviour.ClientMove)
				{
					OnChangeTransition(Transition.PlayerToTarget);	
				}
			}
            
        }
    }

    public override void Act()
    {
        if (IsStateReady)
        {           
            string animName = string.Empty;
            switch (GameManager.Instance.CurrentState)
            {
                case GameManager.GameState.GAME_STATE_TOWN:
                    animName = "TIdle01";
                    break;
                case GameManager.GameState.GAME_STATE_BATTLE:
                    animName = "BIdle";
                    break;
                case GameManager.GameState.GAME_STATE_STORYLINE:
                    animName = "TIdle01";
                    break;
                case GameManager.GameState.GAME_STATE_PLAYERROOM:
                    var playerRoomGenerate = PlayerDataManager.Instance.GetPlayerRoomItemData(this.m_PlayerBehaviour.PlayerKind);
                    animName = playerRoomGenerate.DefaultAnim;
                    break;
                default:
                    animName = "TIdle01";
                    break;
            }
			if(m_roleAnimationComponent.GetClip(animName) != null)
			{
            	m_roleAnimationComponent.CrossFade(animName);            
			}
			else
			{
				Debug.Log("this anim = "+animName+" is null!!!! role=="+m_roleAnimationComponent.gameObject);
			}
        }
		
		BeAdsorbExMove();
    }
	
	void BeAdsorbExMove()
	{
		if(m_PlayerBehaviour.IsBeAbsordEx)
		{
			Vector3 moveVector = (m_PlayerBehaviour.AdsorbExCenterPos - m_PlayerBehaviour.ThisTransform.position).normalized;
			var motion = moveVector * m_PlayerBehaviour.AdsorbExSpeed * Time.deltaTime;
			if (SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
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
		}
		
	}

    public override void DoBeforeEntering()
    {
		this.m_PlayerBehaviour.WalkToPosition = null;
		//this.m_PlayerBehaviour.TargetSelected = null;

        this.m_PlayerBehaviour.GetSkillBySkillID(null);
        //保护人物不嵌入地形
        if (this.m_PlayerBehaviour.ThisTransform.position.y < 0.1f)
        {
            this.m_PlayerBehaviour.ThisTransform.position = new Vector3(this.m_PlayerBehaviour.ThisTransform.position.x, 0.1f, this.m_PlayerBehaviour.ThisTransform.position.z);
        }
        base.DoBeforeEntering();
    }
    public override void DoBeforeLeaving()
    {
        base.DoBeforeLeaving();
    }
}