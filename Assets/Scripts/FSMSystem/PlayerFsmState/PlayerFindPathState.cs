using UnityEngine;
using System.Collections;

public class PlayerFindPathState : PlayerState
{
	
    public PlayerFindPathState()
    {
        m_stateID = StateID.PlayerFindPathing;        
    }
    public override void Reason()
    {

    }

	

     //<summary>
     //走向NPC
     //</summary>
    void GoToNPC()
    {
        var targetPos = this.m_PlayerBehaviour.WalkToPosition.Value;
        Vector3 from = new Vector3(targetPos.x, this.m_PlayerBehaviour.ThisTransform.position.y, targetPos.z);
        if (Vector3.Distance(this.m_PlayerBehaviour.ThisTransform.position, this.m_PlayerBehaviour.WalkToPosition.Value) >= ConfigDefineManager.DISTANCE_ARRIVED_NPC)
        {
            this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.LookRotation(from - this.m_PlayerBehaviour.ThisTransform.position, Vector3.up);
            //this.m_PlayerBehaviour.ThisTransform.rotation = Quaternion.Slerp(this.m_PlayerBehaviour.ThisTransform.rotation
            //   , Quaternion.LookRotation(from - this.m_PlayerBehaviour.ThisTransform.position, Vector3.up), Time.deltaTime * 10);
        }
        else
        {
            this.m_PlayerBehaviour.WalkToPosition = null;
            //Send Portal To Service
            ////TraceUtil.Log("到达传送门，发送传送消息");
            var npcBehaviour = this.m_PlayerBehaviour.TargetSelected.Target.GetComponent<NPCBehaviour>();
            if (npcBehaviour != null)
            {
                NetServiceManager.Instance.EntityService.SendMeetNPC(npcBehaviour.NPCDataModel.SMsg_Header.uidEntity);
                GameManager.Instance.MeetNpcEntityId = npcBehaviour.NPCDataModel.SMsg_Header.uidEntity;
                OnChangeTransition(Transition.PlayerToIdle);
            }
            else
            {
                //TraceUtil.Log("没找到PortalBehaviour组件");
            }
        }
    }

    private void GoToTarget()
    {
        if(this.m_PlayerBehaviour.TargetType == ResourceType.NPC)
        {
                GoToNPC();
        }
    }

    public override void Act()
    {		
        if (IsStateReady)
        {
			if(m_PlayerBehaviour.IsHero)
			{
                if (this.m_PlayerBehaviour.WalkToPosition != null)
                {
                    GoToTarget();
                }
            }

            string animName = string.Empty;
            switch (GameManager.Instance.CurrentState)
            {
                case GameManager.GameState.GAME_STATE_TOWN:
                    animName = "Walk01";
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
        this.m_PlayerBehaviour.GetSkillBySkillID(null);
        base.DoBeforeEntering();
    }
    public override void DoBeforeLeaving()
    {
        this.m_PlayerBehaviour.WalkToPosition = null;
        base.DoBeforeLeaving();
    }
}
