using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MonsterManager : Controller, IEntityManager, ISingletonLifeCycle
{
    public EntityModel LockedMonster { get; private set; }
	public bool IsDoubleBoss{get;set;}
    private List<EntityModel> m_monsterList = new List<EntityModel>();
    private static MonsterManager m_instance;    

    public static MonsterManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new MonsterManager();                
                EntityController.Instance.RegisteManager(TypeID.TYPEID_MONSTER, m_instance);
                SingletonManager.Instance.Add(m_instance);
            }

            return m_instance;
        }
    }

	private BossStatusPanel_V3 m_currentBossStatusPanel;
    public bool IsBossStatusPanelInit()
    {
        return m_currentBossStatusPanel != null;
    }
    public void SetBossStatusPanel(BossStatusPanel_V3 bossStatusPanelScripts)
    {
        m_currentBossStatusPanel = bossStatusPanelScripts;
    }
	public BossStatusPanel_V3 GetBossStatusPanel()
    {
        return m_currentBossStatusPanel;
    }


    

    //public void Init()
    //{
    //}
    //public List<EntityModel> FindTargetByFightResult(FightResult fightResult, out bool isFight)
    //{
    //    List<EntityModel> entityModels = new List<EntityModel>();
    //    var target = m_monsterList.SingleOrDefault(P => ((Monster)P.Behaviour).MonsterDataModel.SMsg_Header.uidEntity == fightResult.SMsgFightFightTo.uidFighter);
    //    if (target != null)
    //    {
    //        isFight = true;
    //        entityModels.Add(target);
    //    }
    //    else
    //    {
    //        isFight = false;
    //        foreach (var item in fightResult.TargetDatas)
    //        {
    //            target = m_monsterList.SingleOrDefault(P => ((Monster)P.Behaviour).MonsterDataModel.SMsg_Header.uidEntity == item.uidTarget);
    //            if (target != null)
    //            {
    //                entityModels.Add(target);
    //            }
    //        }
    //    }
    //    return entityModels;
    //}


    void ReceiveEntityMoveHandle(INotifyArgs notifyArgs)
    {
        var sMsgActionMove_SC = (SMsgActionMove_SCS)notifyArgs;        
        var target = m_monsterList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == sMsgActionMove_SC.uidEntity);
        if (target != null)
        {
            
            ((MonsterBehaviour)target.Behaviour).EntityMove(sMsgActionMove_SC);
            #region 移动test
            //if (Log.IsPrint)
            //{
            //    Log.Instance.StartLog();
            //    Log.Instance.AddLog((sMsgActionMove_SC.uidEntity << 32 >> 32).ToString(), "解下发移动包", sMsgActionMove_SC.floatX.ToString(), sMsgActionMove_SC.floatY.ToString(), sMsgActionMove_SC.fDirectX.ToString(), sMsgActionMove_SC.fDirectY.ToString());
            //    Log.Instance.AppendLine();
            //}            
            #endregion
            //TraceUtil.Log("网络数据怪物移动:" + sMsgActionMove_SC.floatX + " , " + sMsgActionMove_SC.floatY);
        }
        
    }

    //新路点测试
    //int _time;
    void ReceiveMonsterMoveHandle(INotifyArgs notifyArgs)
    {
        var sMsgActionMonsterMove_SC = (SMsgActionMonsterMove_SC)notifyArgs;

        //int i = 0;
        //sMsgActionMonsterMove_SC.dwPoints.ApplyAllItem(p =>
        //    {
        //        TraceUtil.Log("路点" + _time.ToString()  + " ," + i.ToString() + ": " + p.x + " , " + p.y);
        //        i++;
        //    });

        //_time++;
        //TraceUtil.Log("===>处理网络消息");        

        var target = m_monsterList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == sMsgActionMonsterMove_SC.uidMonster);
        if (target != null)
        {
            ((MonsterBehaviour)target.Behaviour).MonsterMove(sMsgActionMonsterMove_SC);
            //TraceUtil.Log("怪物: " + target.Behaviour.transform.position);
        }

       
    }
    
    void ReceiveBeatBackHandle(INotifyArgs args)
    {
        SMsgBattleBeatBack_SC sMsgBattleBeatBack_SC = (SMsgBattleBeatBack_SC)args;                
        var target = m_monsterList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == sMsgBattleBeatBack_SC.uidFighter);
        if (target != null)
        {
            ((MonsterBehaviour)target.Behaviour).BeAttacked(sMsgBattleBeatBack_SC);
        }
    }
    void ReceiveFightFlyHandle(INotifyArgs args)
    {
        SMsgFightHitFly_SC sMsgFightHitFly_SC = (SMsgFightHitFly_SC)args;
        var target = m_monsterList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == sMsgFightHitFly_SC.uidFighter);
        if (target != null)
        {
            ((MonsterBehaviour)target.Behaviour).BeHitFly(sMsgFightHitFly_SC);
        }
    }
    void ReceiveEntityTeleportHandle(INotifyArgs args)
    {
        SMsgFightTeleport_CSC sMsgFightTeleport_CSC = (SMsgFightTeleport_CSC)args;
        var target = m_monsterList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == sMsgFightTeleport_CSC.uidFighter);
        if (target != null)
        {
            ((MonsterBehaviour)target.Behaviour).BeTeleport(sMsgFightTeleport_CSC);
        }
    }
	//组队版AI战斗指令
    void ReceiveFightCommand(INotifyArgs args)
    {        
        var sMsgBattleCommand = (SMsgBattleCommand)args;        
        var target = m_monsterList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == sMsgBattleCommand.uidFighter);
        if (target != null)
        {
            ((MonsterBehaviour)target.Behaviour).FightCommand(sMsgBattleCommand);
            //同步服务器怪物子弹index
            BulletManager.Instance.SynchronousEntity(sMsgBattleCommand.bulletIndex, sMsgBattleCommand.uidFighter);
        }
    }
	//单人版AI战斗指令
	void ReceiveSingleFightCommondHandle(INotifyArgs notifyArgs)
	{
		SMsgFightCommand_SC sMsgFightCommand_SC = (SMsgFightCommand_SC)notifyArgs;
		var entityModel = GetEntityMode(sMsgFightCommand_SC.uidFighter);
		if(entityModel != null)
		{
			var monsterBehaviour = (MonsterBehaviour)entityModel.Behaviour;
			monsterBehaviour.SingleFightCommand(sMsgFightCommand_SC);
		}
	}
    void ReceiveEntityDieHandle(INotifyArgs args)
    {
        var sMsgActionDie_SC = (SMsgActionDie_SC)args;
        var entityModel = this.GetEntityMode(sMsgActionDie_SC.uidEntity);
        this.SetBossHeathValue(sMsgActionDie_SC.uidEntity); //二次调用
        if (entityModel != null)
        {
            //销毁箭头
            var arrowBehaviour = entityModel.GO.GetComponentInChildren<ArrowBehaviour>();
            if (arrowBehaviour != null)
            {
                GameObject.Destroy(arrowBehaviour);
            }

            //销毁光圈
            var guanquan = entityModel.GO.GetComponentInChildren<GuangHuanBehaviour>();
            if (guanquan != null && guanquan.gameObject != null)
            {
                GameObject.Destroy(guanquan.gameObject);
            }            

            ((MonsterBehaviour)entityModel.Behaviour).Die(sMsgActionDie_SC);
            
            //如果boss
            if (IsMonsterBossType(((MonsterBehaviour)entityModel.Behaviour).m_MonsterConfigData._monsterID))
            {                
                //判断当前是否妖女副本
                var peekData = GameDataManager.Instance.PeekData(DataType.InitializeEctype);
                if (peekData == null)
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"DataType.InitializeEctype is null");
                    return;
                }
                SMSGEctypeInitialize_SC ectypeSmg = (SMSGEctypeInitialize_SC)peekData;
                EctypeContainerData ectypeData;
                if (EctypeConfigManager.Instance.EctypeContainerConfigList.ContainsKey((int)ectypeSmg.dwEctypeContainerId))
                {
                    ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[(int)ectypeSmg.dwEctypeContainerId];
                    if (ectypeData.MapType == 3)//妖女副本
                    {
                        GameManager.Instance.DamageFactory.CreateSirenStone(entityModel.GO.transform);
                    }

                }
            }            
        }        
    }
    void ReceiveEntityStopHereHandle(INotifyArgs args)
    {
        var sMsgActionStopHere = (SMsgActionStopHere_SC)args;
        var entityModel = this.GetEntityMode(sMsgActionStopHere.uidEntity);
        if (entityModel != null)
        {
            ((MonsterBehaviour)entityModel.Behaviour).StopHere(sMsgActionStopHere);
        }

        #region 停止
        if (Log.IsPrint)
        {
            //Log.Instance.StartLog();
            //Log.Instance.AddLog((sMsgActionStopHere.uidEntity << 32 >> 32).ToString(), "停止路径",
            //    sMsgActionStopHere.ptHereX.ToString(), sMsgActionStopHere.ptHereY.ToString(), sMsgActionStopHere.fDirectX.ToString(), sMsgActionStopHere.fDirectY.ToString());
            //Log.Instance.AppendLine();
        }
        #endregion
    }
    void ReceiveEntityHordeHandle(INotifyArgs notifyArgs)
    {
        SMsgFightHorde_SC sMsgFightHorde_SC = (SMsgFightHorde_SC)notifyArgs;

        var entityModel = GetEntityMode(sMsgFightHorde_SC.uidFighter);
        if (entityModel != null)
        {
            var monsterBehaviour = (MonsterBehaviour)entityModel.Behaviour;
            var fsmState = monsterBehaviour.FSMSystem;
            var monsterBeHorde = fsmState.FindState(StateID.MonsterBeHorde) as MonsterBeHordeState;
            if (monsterBeHorde != null)
            {
				if(monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterBeAttacked
					|| monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterBeAdsorb
					|| monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterBeHitFly
					|| monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterBeHorde
                    ||monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterStand
					)
				{
					(((MonsterFsmState)(monsterBehaviour.FSMSystem.CurrentState))).DoNotSendBeatEnd = true;
				}
				else
				{
					(((MonsterFsmState)(monsterBehaviour.FSMSystem.CurrentState))).DoNotSendBeatEnd = false;
				}
				
				
                monsterBeHorde.BeHorde(sMsgFightHorde_SC, monsterBehaviour.transform.position);
                fsmState.PerformTransition(Transition.MonsterToBeHorde);
            }
        }
    }
	

	
    void ReceiveBeAdsorbHandle(INotifyArgs notifyArgs)
    {
        SMsgBattleBeAdsorb_SC sMsgBattleBeAdsorb_SC = (SMsgBattleBeAdsorb_SC)notifyArgs;
        var entityModel = GetEntityMode(sMsgBattleBeAdsorb_SC.uidFighter);
        if (entityModel != null)
        {
            var monsterBehaviour = (MonsterBehaviour)entityModel.Behaviour;
            var fsmState = monsterBehaviour.FSMSystem;
            var beatBack = fsmState.FindState(StateID.MonsterBeAdsorb);
            if (beatBack != null)
            {
                ((MonsterBeAdsorbState)beatBack).SetBeAdsorbData(sMsgBattleBeAdsorb_SC);
            }
			
			if(monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterBeAttacked
				|| monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterBeAdsorb
				|| monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterBeHitFly
				|| monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterBeHorde
                ||monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterStand
				)
			{
				(((MonsterFsmState)(monsterBehaviour.FSMSystem.CurrentState))).DoNotSendBeatEnd = true;
			}
			else
			{
				(((MonsterFsmState)(monsterBehaviour.FSMSystem.CurrentState))).DoNotSendBeatEnd = false;
			}
			
            fsmState.PerformTransition(Transition.MonsterToBeAdsorb);
        }
    }

    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.FightCommand.ToString(), ReceiveFightCommand);
        AddEventHandler(EventTypeEnum.BeatBack.ToString(), ReceiveBeatBackHandle);
        AddEventHandler(EventTypeEnum.FightFly.ToString(), ReceiveFightFlyHandle);
        AddEventHandler(EventTypeEnum.EntityMove.ToString(), ReceiveEntityMoveHandle);
        AddEventHandler(EventTypeEnum.MonsterMove.ToString(), ReceiveMonsterMoveHandle);
        AddEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);
        AddEventHandler(EventTypeEnum.EntityStopHere.ToString(), ReceiveEntityStopHereHandle);
        AddEventHandler(EventTypeEnum.BeAdsorb.ToString(), ReceiveBeAdsorbHandle);
        AddEventHandler(EventTypeEnum.EntityHorde.ToString(), ReceiveEntityHordeHandle);
		AddEventHandler(EventTypeEnum.SingleFigntCommand.ToString(), ReceiveSingleFightCommondHandle);
        AddEventHandler(EventTypeEnum.Teleport.ToString(), ReceiveEntityTeleportHandle);
        //AddEventHandler(EventTypeEnum.ReplyShield.ToString(), BossReplyShield);
    }

    //public void MonterSelectedClear()
    //{
    //    m_monsterList.ApplyAllItem(P => ((Monster)P.Behaviour).IsSelect = false);
    //}
	
	public void UnRegisteEntity(Int64 uidEntity)
    {
        TraceUtil.Log("怪物删除:" + uidEntity);
        var targetEntity = this.m_monsterList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == uidEntity);
        if (targetEntity != null)
        {            
            if (targetEntity.Behaviour is ISendInfoToServer)
            {
                GameManager.Instance.TimedSendPackage.RemoveSendInfoObj(targetEntity.Behaviour as ISendInfoToServer);
            }
            //血条Release
            BloodBarManager.Instance.DestroyBarViaEntityUid(uidEntity);
            m_monsterList.Remove(targetEntity);

            //如果是boss死亡
            //if (IsMonsterBossType(((MonsterBehaviour)targetEntity.Behaviour).m_MonsterConfigData._monsterID))
            //{
            //    TraceUtil.Log("boss死亡");                
            //}
            //else
            //{
            //    targetEntity.DestroyEntity();
            //}
            var monsterBehaviour = (MonsterBehaviour)targetEntity.Behaviour;
            if (monsterBehaviour.FSMSystem.CurrentStateID == StateID.MonsterDie)
            {
                if (monsterBehaviour.FSMSystem.FindState(StateID.MonsterDie).m_roleAnimationComponent == monsterBehaviour.NormalStatus.animation)
                {
					this.DelayDestroy(targetEntity, CommonDefineManager.Instance.CommonDefine.Normal_CorpseTime);
                }
                else
                {
                    this.DelayDestroy(targetEntity, CommonDefineManager.Instance.CommonDefine.Critical_CorpseTime);
                }                
            }
            else
            {                
                targetEntity.DestroyEntity();
            }
            
        }

        //检查Factory中是否有未创建的缓存数据，如果有，一并清除
        MonsterFactory.Instance.UnRegister(uidEntity);
    }
    /// <summary>
    /// Boss破防处理(服务器端已经取消此指令下发)
    /// </summary>
    /// <param name="uid"></param>
    //public void BossBreakShield(INotifyArgs e)
    //{
    //    BossShieldStruct bossShieldStruct = (BossShieldStruct)e;
    //    var bossEntityModel = GetEntityMode(bossShieldStruct.uidFighter);
    //    if (bossEntityModel != null)
    //    {
    //        //((MonsterBehaviour)bossEntityModel.Behaviour).MonsterShieldValue = 0;

    //    }
    //    else
    //    {
    //        TraceUtil.Log("破防消息找不到Boss");
    //    }
    //}
    /// <summary>
    /// Boss防护值恢复(服务器端已经取消此指令下发)
    /// </summary>
    /// <param name="uid"></param>
    //public void BossReplyShield(INotifyArgs e)
    //{
    //    BossShieldStruct bossShieldStruct = (BossShieldStruct)e;
    //    var bossEntityModel = GetEntityMode(bossShieldStruct.uidFighter);
    //    if (bossEntityModel != null)
    //    {
    //        ((MonsterBehaviour)bossEntityModel.Behaviour).ReplyMonsterShieldValue();
    //    }
    //    else
    //    {
    //        TraceUtil.Log("防护值恢复消息找不到Boss");
    //    }
    //}
    private void DelayDestroy(EntityModel entityModel, float time)
    {
        MonsterFactory.Instance.DelayDestroy(entityModel, time);     
    }

    //是否为boss
    public bool IsMonsterBossType(int monsterID)
    {
        var monsterData = BattleConfigManager.Instance.GetMonsterData(monsterID);
        bool isBoss = monsterData.MonsterSubType == 2;  //2为boss

        return isBoss;
	} 
	public bool IsMonsterBossType(long entityID)
	{
		var monster=this.m_monsterList.SingleOrDefault(P=>P.EntityDataStruct.SMsg_Header.uidEntity==entityID);
		if(monster!=null)
		{
			return IsMonsterBossType(((SMsgPropCreateEntity_SC_Monster) monster.EntityDataStruct).BaseObjectValues.OBJECT_FIELD_ENTRY_ID);
		}
		else
		{
			return false;
		}
	}      

    //设置boss状态栏血值
    public void SetBossHeathValue(Int64 monsterUID)
    {
        if (m_currentBossStatusPanel != null)
        {
            m_currentBossStatusPanel.PlayCutBloodAnimation(monsterUID);
        }        
    }
    /// <summary>
    /// 根据怪物ID获得怪物列表
    /// </summary>
    /// <param name="monsterId"></param>
    public IEnumerable<Transform> GetMonsterListByMonsterId(int monsterId)
    {
        return from monster in this.m_monsterList
               where ((MonsterBehaviour)monster.Behaviour).m_MonsterConfigData._monsterID == monsterId
               select monster.Behaviour.transform;
    }
   /// <summary>
    /// 查找在指定半径和方向上能被攻击的怪物
    /// (玩家会每帧调用，决定是否发起攻击)
   /// </summary>
    /// <param name="center">中心点</param>
    /// <param name="distance">判定有效距离</param>
    /// <param name="forward">判断方向</param>
    /// <param name="angle">扇形角度</param>
   /// <returns></returns>
    public IEnumerable<EntityModel> SearchBeAttackedMonster(Vector3 center, float distance, Vector3 forward, float angle)
    {
        //return this.m_monsterList.Where(P =>
        //    {
        //        var monsterBehaviour=P.Behaviour as MonsterBehaviour;
        //        if (!monsterBehaviour.IsDie&&monsterBehaviour.transform != null)
        //        {
        //            return this.CanAttackCheck(center, monsterBehaviour.transform.position
        //                , forward, angle, distance, 0);
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    });

        #region edit by lee
        if (PVPBattleManager.Instance.IsPVPBattle)
        {
            var pvpPlayerData = PVPBattleManager.Instance.GetPVPPlayerData();
            var pvpPlayerModel = PlayerManager.Instance.GetEntityMode(pvpPlayerData.uidEntity);
            if (pvpPlayerModel != null)
            {
                var pvpPlayerBehaviour = (PlayerBehaviour)pvpPlayerModel.Behaviour;
                if (!pvpPlayerBehaviour.IsDie && pvpPlayerBehaviour.transform != null)
                {
                    if (this.CanAttackCheck(center, pvpPlayerBehaviour.transform.position
                   , forward, angle, distance, 0))
                    {
                        return new List<EntityModel>() { pvpPlayerModel };
                    }                    
                }               
            }
            return null;
        }
        return this.m_monsterList.Where(P =>
        {
            var monsterBehaviour = P.Behaviour as MonsterBehaviour;
			if(PlayerManager.Instance.CheckMonsterHostility(P))
			{
				return false;
			}
            if (!monsterBehaviour.IsDie && !monsterBehaviour.Invincible && monsterBehaviour.transform != null)
            {
                return this.CanAttackCheck(center, monsterBehaviour.transform.position
                    , forward, angle, distance, 0);
            }
            else
            {
                return false;
            }
        });
        #endregion
    }
    /// <summary>
    /// 判断是否在攻击范围
    /// </summary>
    /// <param name="center">中心点</param>
    /// <param name="target">目标位置</param>
    /// <param name="forward">判断方向</param>
    /// <param name="angle">扇形角度</param>
    /// <param name="distance">判定有效距离</param>
    /// <param name="radius">目标碰撞半径</param>
    /// <returns></returns>
    private bool CanAttackCheck(Vector3 center,Vector3 target,Vector3 forward, float angle,float distance, float  radius)
    {
        return (Vector3.Distance(center, target) <= distance + radius) && CommonTools.AngleBetween2Vector(forward, (target - center)) <= angle /2;
            //&& Vector3.Angle(forward, (target - center)) <= angle/2;
    }
    public void RegisteEntity(EntityModel monsterData)
    {        
        //血条增加
        m_monsterList.Add(monsterData);

		/*
		Transform bloodBarMP;
        monsterData.GO.transform.RecursiveFindObject("BloodBarMP", out bloodBarMP);
        if (null != bloodBarMP)
        {                       
            GameObject bloodBarPrefab = null;
            switch (((SMsgPropCreateEntity_SC_Monster)monsterData.EntityDataStruct).MonsterValues.MONSTER_FIELD_TYPE)
            {
                case 0:
                    bloodBarPrefab = MonsterFactory.Instance.BloobBarPrefab_normal;
                    break;
                case 1:
                    bloodBarPrefab = MonsterFactory.Instance.BloobBarPrefab;
                    break;
                case 2:
                    bloodBarPrefab = MonsterFactory.Instance.BloobBarPrefab_boss;
                    break;
                default:
                    break;
            }
            if (bloodBarPrefab != null)
            {
                BloodBarManager.Instance.AttachBarToTarget(monsterData.EntityDataStruct.SMsg_Header.uidEntity, bloodBarMP, bloodBarPrefab);
            }            
        }
        //发UI事件，参数 EntityModel
        UIEventManager.Instance.TriggerUIEvent(UIEventType.CreatMoster, monsterData);
        */
    }
	

    public EntityModel GetEntityMode(long uid)
    {
        var targetModel = this.m_monsterList.SingleOrDefault(p => (p.EntityDataStruct.SMsg_Header.uidEntity == uid));        
        if (targetModel != null)
        {
            return targetModel;
        }
        return null;
    }

	/// <summary>
	/// 设置怪物是否可见
	/// </summary>
	public void SetMonsterVisible(long monsterUID, int isVisibleValue)
	{	
		int layer = isVisibleValue == 1?11:21; //11=enemy, 21=hide
		var monster=this.m_monsterList.SingleOrDefault(P=>P.EntityDataStruct.SMsg_Header.uidEntity==monsterUID);
		if(monster!=null && monster.Behaviour)
		{
			SetCildLayer(monster.GO.transform,layer);
		}
	}

	private void SetCildLayer(Transform m_transform, int layer)
	{
		m_transform.gameObject.layer = layer;
		if (m_transform.childCount > 0)
		{
			foreach (Transform child in m_transform)
			{                    
				SetCildLayer(child, layer);
			}
		}
	}

    //获取怪物列表
    public List<EntityModel> GetMonstersList()
    {
        return m_monsterList;
    }

    public void Instantiate()
    {

    }

	public void Empty()
	{

	}

    public void LifeOver()
    {
        this.ClearEvent();
        m_instance = null;
    }
	//获取场景npc
	public int GetCurSceneActEntityCount()
	{
		int count = 0;
		if (m_monsterList != null) {
			if(PlayerManager.Instance.FindHeroEntityModel() == null)
				return count;
			int hostility = ((IPlayerDataStruct)(PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct)).GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHT_HOSTILITY;
			foreach(EntityModel model in m_monsterList)
			{
				int monsterHostility = ((SMsgPropCreateEntity_SC_Monster)model.EntityDataStruct).MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY;
				if( monsterHostility != 3 && monsterHostility != hostility )
				{
					count++;
				}
			}
		}
		return count;
	}

    /// <summary>
    /// Lock skill attack target
    /// </summary>
    /// <param name="player"></param>
    public void LockAttackTarget(Transform player)
    {
        EntityModel lockedMonster=null;
        float minAngle = 360;
		float minInnerAngle = 360;
		float minDistance = 0;
		bool hasInnerMonster=false;
        float minDis = CommonDefineManager.Instance.CommonDefine.SearchRadius;
		Vector3 forward=player.forward;
        Vector3 playerPos=player.position;
        float searchAngle = CommonDefineManager.Instance.CommonDefine.SearchAngle/2;

		List<LockMonsterInfo> lockMonsterInfo = new List<LockMonsterInfo> ();
        this.m_monsterList.ApplyAllItem((monster) =>
            {
                
                var monsterBehaviour = monster.Behaviour as MonsterBehaviour; 
                bool isMonsterProtected= ((SMsgPropCreateEntity_SC_Monster)monster.EntityDataStruct).MonsterInvisibleValue.UINT_FILED_PROTECTED == 1;
                if (monsterBehaviour!= null 
                    &&!PlayerManager.Instance.CheckMonsterHostility(monster)&&!monsterBehaviour.IsDie 
                    && monsterBehaviour.transform != null
                    && !monsterBehaviour.Invincible )
                {                   
                     var monsterPos=monster.Behaviour.transform.position;
                     var distance = Vector3.Distance(monsterPos, playerPos);
					if(minDistance==0)
					{
						minDistance=distance;
					}
					if (distance <= minDis)
                     {
                         var angle = CommonTools.AngleBetween2Vector(forward, (monsterPos - playerPos));
						if (angle <= searchAngle && distance < CommonDefineManager.Instance.CommonDefine.SearchMinRange)
                         {
							hasInnerMonster=true;
							if(angle < minInnerAngle
						   		|| (angle == minInnerAngle&&distance<minDistance))
							{
								minDistance=distance;
								minInnerAngle = angle;
								lockedMonster = monster;
							}
                         }
						else if (!hasInnerMonster)
                         {
							lockMonsterInfo.Add(new LockMonsterInfo(monster,angle,distance));
                         }
                     }
                }               
            });

		if (!hasInnerMonster&&lockMonsterInfo.Count>0)
		{
			lockMonsterInfo.Sort((x, y) => { if (x.AreaIndex == y.AreaIndex) return 0; else if (x.AreaIndex > y.AreaIndex) return 1; else return -1; });
			int areaIndex=lockMonsterInfo[0].AreaIndex;
			minDistance=lockMonsterInfo[0].Distance;
			bool getTargetMonster=false;
			//没有优先锁中对象，进行第二轮判断
			foreach(var mon in lockMonsterInfo)
			{
				if(mon.AreaIndex==areaIndex && mon.Distance<=minDistance)
				{
					lockedMonster = mon.Monster;
				}
			}
			lockMonsterInfo.Clear ();
		}

        if (lockedMonster != null)
        {
            if (LockedMonster == null)
            {
                ((MonsterBehaviour)lockedMonster.Behaviour).BeLocked();
            }
            else
            {
                if (lockedMonster.EntityDataStruct.SMsg_Header.uidEntity != LockedMonster.EntityDataStruct.SMsg_Header.uidEntity)
                {
                    ((MonsterBehaviour)LockedMonster.Behaviour).ReleaseLockedStatus();
                    ((MonsterBehaviour)lockedMonster.Behaviour).BeLocked();
                }
            }
        }
        else  if (LockedMonster != null)
        {
            ((MonsterBehaviour)LockedMonster.Behaviour).ReleaseLockedStatus();
        }
        LockedMonster = lockedMonster;
    }
}

public struct LockMonsterInfo
{
	public EntityModel Monster;
	public float Angle;
	public float Distance;
	public int AreaIndex;
	public LockMonsterInfo(EntityModel monster,float angle,float distance)
	{
						Monster = monster;
						Angle = angle;
						Distance = distance;
		AreaIndex = Mathf.FloorToInt (angle / CommonDefineManager.Instance.CommonDefine.SearchAreaAngle);
	}

}
