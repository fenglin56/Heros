using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerCastAbilityState : PlayerState
{
    private List<SirenSkill> m_sirenSkillList = new List<SirenSkill>();
    //private List<UIEffectGroup> m_uiEffectGroupList = new List<UIEffectGroup>();

    private bool m_IsAllSkillOver = false;
    private float m_durationTime = 0;

    private SkillBase m_skillBase;
    private SkillActionData m_actData;

    private bool m_isCreateUIEffect = false;
    private float m_delayCreateUIEffectTime = 0;

    private bool m_isCastCurSkill = false;
    private float m_delayCastCurSkillTime = 0;

    private Camera m_skillCamera;

    //private bool m_isCanFire = false;
    private List<UIEffectGroup> m_UIEffectGroup = new List<UIEffectGroup>();
    private UIEffectGroup m_CurUIEffectGroup;

    public PlayerCastAbilityState()
    {
        this.m_stateID = StateID.PlayerCastAbility;
    }

    public override void Reason()
    {
        if (!this.IsStateReady)
            return;
    }

    public override void Act()
    {
        //m_durationTime -= Time.deltaTime;
        //if (m_durationTime <= 0)
        //{

        //}
        if (!this.IsStateReady)
            return;

        if (m_actData != null)
        {
            if (!m_isAnimPlayed || m_roleAnimationComponent[m_actData.m_animationId].wrapMode == WrapMode.Loop)
            {
                m_roleAnimationComponent.Play(m_actData.m_animationId);
                m_isAnimPlayed = true;
            }
        }        

        /*
        if (m_isCreateUIEffect)
        {
            m_delayCreateUIEffectTime -= Time.deltaTime;
            if (m_delayCreateUIEffectTime <= 0)
            {
                CreateUIEffect();

                m_UIEffectGroup.RemoveAt(0);
                if (m_UIEffectGroup.Count > 0)
                {
                    m_delayCreateUIEffectTime = m_UIEffectGroup[0]._EffectStartTime;
                }
                else
                {
                    m_isCreateUIEffect = false;
                }                
            }
        }
        */

        if (m_isCastCurSkill)
        {
            m_delayCastCurSkillTime -= Time.deltaTime;
            if (m_delayCastCurSkillTime <= 0)
            {
                m_isCastCurSkill = false;

                CastCurSkill();
            }
        }
    }

    public override void DoBeforeEntering()
    {
        //获取可施行无双技
        var sirenSkillData = SkillDataManager.Instance.GetSirenSkillData(this.m_PlayerBehaviour.PlayerKind);        
        var levelList = SirenManager.Instance.GetYaoNvList();
        var sirenList = PlayerDataManager.Instance.GetPlayerSirenList();
        var sirenDefaltSkill = sirenSkillData._SirenSkills.SingleOrDefault(p => p._SirenID == 0);
        if(sirenDefaltSkill!=null)
        {
            //默认妖女无双技
            m_sirenSkillList.Add(sirenDefaltSkill);
        }
        sirenList.ApplyAllItem(p =>
            {
                SYaoNvContext? context = levelList.SingleOrDefault(k => k.byYaoNvID == p._sirenID);
                if (context != null)
                {
                    if (context.Value.byLevel > 0)//妖女解锁
                    {
                        SirenSkill sirenSkill = sirenSkillData._SirenSkills.SingleOrDefault(m => m._SirenID == p._sirenID);
                        if (sirenSkill != null)
                        {
                            m_sirenSkillList.Add(sirenSkill);
                        }                       
                    }
                }
            });

        if (m_sirenSkillList.Count > 0)
        {
            SirenSkill sirenSkill = m_sirenSkillList[0];
            m_sirenSkillList.RemoveAt(0);

            this.m_PlayerBehaviour.GetSkillBySkillID(sirenSkill._SkillID);
            m_skillBase = this.m_PlayerBehaviour.SelectedSkillBase;

            //如果找不到技能
            if (m_skillBase == null)
            {
                OnChangeTransition(Transition.PlayerToIdle);
            }

            m_skillBase.AddActionDelegate(ActionHandler);
            m_skillBase.AddSkillOverDelegate(SkillOverHandler);
            if (this.m_PlayerBehaviour.IsHero)
            {
                SendNormalSkillCommand(m_skillBase.SkillId);
            }
            m_skillBase.Fire();
            IsPlaySkillCamera(m_skillBase.SkillData);
            //UI特效
            /*
            m_UIEffectGroup = m_skillBase.SkillData.m_UIEffectGroupList;
            if (m_UIEffectGroup.Count > 0)
            {
                m_delayCreateUIEffectTime = m_UIEffectGroup[0]._EffectStartTime;
                m_isCreateUIEffect = true;
            }

            */
            //技能施放间隔
            m_delayCastCurSkillTime = sirenSkill._Duration;
            m_isCastCurSkill = true;

            UIEventManager.Instance.TriggerUIEvent(UIEventType.SirenSkillFire, true);
            PlayerManager.Instance.HideHeroTitle(true);
        }
        else
        {
            OnChangeTransition(Transition.PlayerToIdle);
        }

        base.DoBeforeEntering();
    }

    /// <summary>
    /// 判断是否播放技能摄像机
    /// </summary>
    /// <param name="cameraData"></param>
    private void IsPlaySkillCamera(SkillConfigData skillData)
    {
		if (!m_PlayerBehaviour.IsHero)
						return;

        Vector3 cameraPos = this.m_roleBehaviour.transform.position + skillData.cameraRangeOffset;
        var monsterList = MonsterManager.Instance.GetMonstersList();

        foreach (var item in monsterList)
        {
            var distance = Vector3.Distance(item.GO.transform.position, cameraPos);
            if (skillData.skillCameraRange >= distance)
            {
                if (m_skillCamera == null)
                {
                    m_skillCamera = CreateSkillCamera();
					m_skillCamera.gameObject.AddComponent<SkillCamera>();
                }
                
				m_skillCamera.GetComponent<SkillCamera>().InitCameraData(skillData.cameraIdList);;
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

    private void CreateUIEffect()
    {
        //UI特效
        var uiEffectGroups = m_UIEffectGroup[0];
        GameObject effect = (GameObject)GameObject.Instantiate(uiEffectGroups._UIEffectPrefab);
        effect.transform.parent = BattleManager.Instance.transform.parent;
        effect.transform.localScale = Vector3.one;
        effect.transform.position = uiEffectGroups._EffectStartPos;
        effect.AddComponent<DestroySelf>();      
    }

    public override void DoBeforeLeaving()
    {
        m_PlayerBehaviour.Invincible = false;
        m_PlayerBehaviour.IronBody = false;

        m_PlayerBehaviour.ChangeDisplayState(1);

        if (m_skillBase != null)
        {
            m_skillBase.RemoveActionDelegate(ActionHandler);
            m_skillBase.DeleteSkillOverDelegate(SkillOverHandler);
        }

        UIEventManager.Instance.TriggerUIEvent(UIEventType.SirenSkillFire, false);
        PlayerManager.Instance.HideHeroTitle(false);

        base.DoBeforeLeaving();
    }

    void ActionHandler(SkillActionData actData)
    {
        m_PlayerBehaviour.Invincible = (actData.m_invincible == 1);
        m_PlayerBehaviour.IronBody = (actData.m_ironBody == 1);
        
        m_actData = (SkillActionData)actData.Clone();
        m_isAnimPlayed = false;

        m_PlayerBehaviour.ChangeDisplayState(m_actData.m_moveType);
    }

    //IEnumerator LateFireEffect(UIEffectGroup uiEffectGroups)
    //{
    //    yield return new WaitForSeconds(uiEffectGroups._EffectStartTime);
    //    GameObject effect = (GameObject)GameObject.Instantiate(uiEffectGroups._UIEffectPrefab);
    //    effect.transform.parent = BattleManager.Instance.transform.parent;
    //    effect.transform.localScale = Vector3.one;
    //    effect.transform.position = uiEffectGroups._EffectStartPos;
    //    yield return new WaitForSeconds(uiEffectGroups._EffectDuration);
    //    GameObject.Destroy(effect);
    //}

    void CastCurSkill()
    {
        if (m_sirenSkillList.Count <= 0)
        {            
            return;
        }

        m_skillBase.RemoveActionDelegate(ActionHandler);
        m_skillBase.DeleteSkillOverDelegate(SkillOverHandler);

        SirenSkill sirenSkill = m_sirenSkillList[0];
        m_sirenSkillList.RemoveAt(0);

        this.m_PlayerBehaviour.GetSkillBySkillID(sirenSkill._SkillID);
        m_skillBase = this.m_PlayerBehaviour.SelectedSkillBase;
        m_skillBase.AddActionDelegate(ActionHandler);
        m_skillBase.AddSkillOverDelegate(SkillOverHandler);
        if (this.m_PlayerBehaviour.IsHero)
        {
            SendNormalSkillCommand(m_skillBase.SkillId);
        }
        m_skillBase.Fire();
        IsPlaySkillCamera(m_skillBase.SkillData);
        //UI特效
        /*
        m_UIEffectGroup = m_skillBase.SkillData.m_UIEffectGroupList;
        if (m_UIEffectGroup.Count > 0)
        {
            m_delayCreateUIEffectTime = m_UIEffectGroup[0]._EffectStartTime;
            m_isCreateUIEffect = true;
        }
        */

        //技能施放间隔
        m_delayCastCurSkillTime = sirenSkill._Duration;
        m_isCastCurSkill = true;
    }

    void SkillOverHandler()
    {
        if (GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
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

        if (m_sirenSkillList.Count <= 0)
        {
            OnChangeTransition(Transition.PlayerToIdle);
            return;
        }

        
        //m_skillBase.RemoveActionDelegate(ActionHandler);
        //m_skillBase.DeleteSkillOverDelegate(SkillOverHandler);

        //SirenSkill sirenSkill = m_sirenSkillList[0];
        //m_sirenSkillList.RemoveAt(0);

        //this.m_PlayerBehaviour.GetSkillBySkillID(sirenSkill._SkillID);
        //m_skillBase = this.m_PlayerBehaviour.SelectedSkillBase;
        //m_skillBase.AddActionDelegate(ActionHandler);
        //m_skillBase.AddSkillOverDelegate(SkillOverHandler);
        //if (this.m_PlayerBehaviour.IsHero)
        //{
        //    SendNormalSkillCommand(m_skillBase.SkillId);
        //}
        //m_skillBase.Fire();

        ////UI特效
        //var uiEffectGroups = m_skillBase.SkillData.m_UIEffectGroupList;
        //if (uiEffectGroups.Count > 0)
        //{
        //    m_delayCreateUIEffectTime = uiEffectGroups[0]._EffectStartTime;
        //    m_isCreateUIEffect = true;
        //}
    }

    private void FireSkillEffect()
    {
        
    }

    private void SendNormalSkillCommand(int skillId)
    {
        Int64 targetEntityId = 0;
        if (m_skillBase.SkillData.IsLockTarget)
        {
            //计算锁定目标，保存在PlayerBehaviour一个变量，并把目标发给服务器端
            var targetEntityModel = LockAttackMonster(this.m_attackDistance, this.m_attackAngle);
            if (targetEntityModel != null)
            {
                m_PlayerBehaviour.ActionLockTarget = targetEntityModel;
                targetEntityId = targetEntityModel.EntityDataStruct.SMsg_Header.uidEntity;

                m_PlayerBehaviour.ThisTransform.LookAt(new Vector3(targetEntityModel.GO.transform.position.x, m_PlayerBehaviour.ThisTransform.position.y, targetEntityModel.GO.transform.position.z));
            }
        }

        float xValue, yValue;
        this.m_PlayerBehaviour.ThisTransform.position.SetToServer(out xValue, out yValue);

        if (GameManager.Instance.CurrentGameMode == GameMode.MULTI_PLAYER)
        {
            SMsgBattleCommand sMsgBattleCommand = new SMsgBattleCommand();
            sMsgBattleCommand.uidFighter = this.m_PlayerBehaviour.RoleDataModel.SMsg_Header.uidEntity;
            sMsgBattleCommand.uidTarget = targetEntityId;
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
        else if (GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
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
