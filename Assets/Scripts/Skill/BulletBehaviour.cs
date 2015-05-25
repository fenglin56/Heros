using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class BulletBehaviour : View
{
    public int BulletID { get; private set; }
    public Int64 FormEntityID { get; private set; }
    public ulong BulletIndex { get; private set; }
    public BulletData BulletData { get; private set; }
	
	
	//public CampType Camp;
	public int AffectTarget;
	
	private int m_skillId;
	private long m_targetId;
	
	
	
	private BulletImpactData m_impactData;
    
    private Vector3 m_motionVector;         //????????  (??/??)  
    private Vector3 m_accelerationVector;   //?????????? (??/??) 
    private Transform m_thisTransfrom;
    private Transform m_caster;
    private Transform m_followTargetTransform;
    private float m_followSpeed;
    private bool m_IsFollowType;

	private int m_srcLevel;
	private int m_srcHit;
	//private Hostility m_hostility = Hostility.FIGHT_HOSTILITY_NEUTRAL;	//阵营，默认中立
    private CampType m_ownerCampType = CampType.CAMP_NEUTRAL;      //阵营，默认中立

	private SMsgFightSaveProp_CS m_FightSavePropStruct = new SMsgFightSaveProp_CS();

    private Vector3 m_cachedBeatBackDir = Vector3.zero;


    public BulletImpactData GetBulletImpactData(int skillId, BulletData bData)
    {
        SkillConfigData skillData = SkillDataManager.Instance.GetSkillConfigData(skillId);
		if(skillData == null)
		{
			skillData = new SkillConfigData();//如果没有技能数据，填充默认值，例如死亡子弹
		}
        int calID = bData.m_calculateId;
        int strenthenSkillId = skillData.FatherSkill == 0? skillId : skillData.FatherSkill;
        SSkillInfo? skillInfo = SkillModel.Instance.GetCurSkill (strenthenSkillId);
        if(bData.m_bulletStrengthen == 1 && skillInfo != null && skillInfo.Value.byStrengthenLv > 1)
        {
            calID = bData.m_calculateId + skillInfo.Value.byStrengthenLv;
        }
		if(bData.m_calculateId == 0)
		{
			calID = 0;
		}

        BulletImpactData bulletImpactData = SkillDataManager.Instance.GetBulletImpactData(calID);
        if(null == bulletImpactData)
        {
            TraceUtil.Log("No impact Data");
        }
        return bulletImpactData;

    }

    public void InitBulletFromServer(int bulletID, Int64 entityID, BulletData data, long targetId)
	{
		BulletData = data;
		
		
		m_impactData = SkillDataManager.Instance.GetBulletImpactData(data.m_calculateId);
        BulletID = -1;
        FormEntityID = entityID;
        m_targetId = targetId;
        //BulletIndex = BulletManager.Instance.CalendarIndex(entityID);
        m_motionVector = Vector3.zero;
        m_accelerationVector = Vector3.zero;
        m_thisTransfrom = this.transform;
        //m_impactData = GetBulletImpactData(skillId, data);
		
		BattleManager.Instance.BulletBornShakeCamera(BulletData, FormEntityID);
//		if(!string.IsNullOrEmpty( BulletData.m_bornSfxId) && BulletData.m_bornSfxId != "0")
//		{
//			SoundManager.Instance.PlaySoundEffect(BulletData.m_bornSfxId);
//		}
		PlayBulletBornSound(BulletData);
        TypeID type;
        EntityModel ownerData = EntityController.Instance.GetEntityModel(FormEntityID, out type );
        m_cachedBeatBackDir = ownerData.GO.transform.TransformDirection(Vector3.forward);
       
        
        if(type == TypeID.TYPEID_PLAYER)
        {
            var playerDataStruct = (IPlayerDataStruct)ownerData.EntityDataStruct;
            m_srcHit = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_NICETY;
            m_srcLevel = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            m_ownerCampType= (CampType)playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHT_HOSTILITY;
        }
        else if(type == TypeID.TYPEID_MONSTER)
        {
            SMsgPropCreateEntity_SC_Monster monsterData = (SMsgPropCreateEntity_SC_Monster)ownerData.EntityDataStruct;
            m_srcHit = monsterData.MonsterInvisibleValue.UNIT_FIELD_NICETY;
            m_srcLevel = monsterData.MonsterUnitValues.UNIT_FIELD_LEVEL;
            m_ownerCampType= (CampType)monsterData.MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY;
        }
	}

    public void InitLocalBullet(int bulletID, Int64 entityID, BulletData data, int skillId, long targetId)
    {
        BulletData = data;
		
		AffectTarget = data.m_affectTarget;//affectTarget;
			
		//m_impactData = SkillDataManager.Instance.GetBulletImpactData(data.m_calculateId);
        BulletID = bulletID;
        FormEntityID = entityID;
        BulletIndex = BulletManager.Instance.ReadIndex(entityID);
        //TraceUtil.Log("==>×???behaviourIndex:" + BulletIndex + " m_bulletId:" + BulletData.m_bulletId + ":" + " entityID: " + entityID + " Time:" + Time.realtimeSinceStartup);
        m_motionVector = Vector3.zero;
        m_accelerationVector = Vector3.zero;
        m_thisTransfrom = this.transform;
		m_skillId = skillId;
		m_targetId = targetId;
        m_impactData = GetBulletImpactData(skillId, data);

		BattleManager.Instance.BulletBornShakeCamera(BulletData, FormEntityID);
//		if(!string.IsNullOrEmpty( BulletData.m_bornSfxId) && BulletData.m_bornSfxId != "0")
//		{
//			SoundManager.Instance.PlaySoundEffect(BulletData.m_bornSfxId);
//		}
		PlayBulletBornSound(BulletData);
        TypeID type;
        EntityModel ownerData = EntityController.Instance.GetEntityModel(FormEntityID, out type );
        m_cachedBeatBackDir = ownerData.GO.transform.TransformDirection(Vector3.forward);

		if(type == TypeID.TYPEID_PLAYER)
		{
			var playerDataStruct = (IPlayerDataStruct)ownerData.EntityDataStruct;
			m_srcHit = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_NICETY;
			m_srcLevel = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
			m_ownerCampType= (CampType)playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHT_HOSTILITY;
		}
		else if(type == TypeID.TYPEID_MONSTER)
		{
			SMsgPropCreateEntity_SC_Monster monsterData = (SMsgPropCreateEntity_SC_Monster)ownerData.EntityDataStruct;
			m_srcHit = monsterData.MonsterInvisibleValue.UNIT_FIELD_NICETY;
			m_srcLevel = monsterData.MonsterUnitValues.UNIT_FIELD_LEVEL;
            m_ownerCampType= (CampType)monsterData.MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY;
		}
		if(BulletData.m_breakType == 2)//死亡子弹 存储怪物属性
		{
			var model = MonsterManager.Instance.GetEntityMode(FormEntityID);
			SMsgPropCreateEntity_SC_Monster monsterProp = (SMsgPropCreateEntity_SC_Monster)model.EntityDataStruct;
			if(m_impactData.m_damage_type == 1)//普通结算
			{
				m_FightSavePropStruct.byPropNum = 6;
				m_FightSavePropStruct.nProp = new int[6];
				
				m_FightSavePropStruct.nProp[0] = monsterProp.MonsterInvisibleValue.UNIT_FIELD_ATTACK;
				m_FightSavePropStruct.nProp[1] = monsterProp.MonsterInvisibleValue.UNIT_FIELD_NICETY;
				m_FightSavePropStruct.nProp[2] = monsterProp.MonsterInvisibleValue.UNIT_FIELD_BURST;
				m_FightSavePropStruct.nProp[3] = monsterProp.MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY;
				m_FightSavePropStruct.nProp[4] = monsterProp.MonsterUnitValues.UNIT_FIELD_LEVEL;
				m_FightSavePropStruct.nProp[5] = monsterProp.MonsterInvisibleValue.UINT_FIELD_DEFBREAK;
			}
			else if(m_impactData.m_damage_type == 2)
			{
				m_FightSavePropStruct.byPropNum = (byte)m_impactData.m_affect_prop.Length;
				m_FightSavePropStruct.nProp = new int[m_FightSavePropStruct.byPropNum];
				
				for(int i=0;i<m_FightSavePropStruct.byPropNum;i++)
				{
					if(m_impactData.m_affect_src[i] == 1)
					{
						//int propIndex = CommonDefineManager.Instance.GetPropKey(m_impactData.m_affect_prop[i]);
						int propIndex = PlayerDataManager.Instance.GetPropID(m_impactData.m_affect_prop[i]);
						if(propIndex == -1)
						{
							Debug.LogError("PlayerPropParam.xml is not exit "+m_impactData.m_affect_prop[i]);
							continue;
						}
						m_FightSavePropStruct.nProp[i] = monsterProp.GetValue(propIndex);
					}
					else
					{
						m_FightSavePropStruct.nProp[i] = 0;
					}
				}
			}
		}
    }

    public void Fired(Vector3 startPos, float quaterionY, Vector3 motionVector, Vector3 accelerationVector, float lifeTime)
    {
        if (!m_thisTransfrom.gameObject.activeInHierarchy)
            m_thisTransfrom.gameObject.SetActive(true);
        
        m_thisTransfrom.position = startPos;
        Quaternion initQuaternion = m_thisTransfrom.rotation;
        m_thisTransfrom.rotation = Quaternion.Euler(initQuaternion.eulerAngles.x, quaterionY, initQuaternion.eulerAngles.z);
        m_motionVector = motionVector;
        m_accelerationVector = accelerationVector;
        StartCoroutine(DestroySelf(lifeTime));      
		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
			if(m_impactData != null)
			{
				StartCoroutine(DoImpact());
			}
		}
	}	

    public void Fired(float lifeTime,Transform caster)
    {
        this.m_caster = caster;
        Fired(lifeTime);
    }
    public void Fired(float lifeTime)
    {
        StartCoroutine(DestroySelf(lifeTime));
		if(GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
			if(m_impactData != null)
			{
				StartCoroutine(DoImpact());
			}
		}
    }

    /// <summary>
    /// 跟踪类型子弹
    /// </summary>
    /// <param name="lifeTime"></param>
    /// <param name="targetTrans"></param>
    /// <param name="speed"></param>
    public void FollowFired(float lifeTime, Vector3 startPos, Transform targetTrans, float speed)
    {
        m_thisTransfrom = this.transform;
        m_thisTransfrom.position = startPos;
        m_followTargetTransform = targetTrans;
        m_followSpeed = speed;
        m_accelerationVector = Vector3.zero;
        m_IsFollowType = true;

        StartCoroutine(DestroySelf(lifeTime));
        if (GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
        {
            if (m_impactData != null)
            {
                StartCoroutine(DoImpact());
            }
        }
    }


    //??·???????????
    public bool IsBreakType()
    {
        if (BulletData == null)
        {
            BulletData = SkillDataManager.Instance.GetBulletData(BulletID);
        }
        if (BulletData != null && BulletData.m_breakType == 1)
        {
            return true;               
        }
        return false;
    }

    IEnumerator DestroySelf(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ////TraceUtil.Log("delete ==>" + this.BulletIndex);
        //\??????÷??goPool????????
		
		DoDestroySelf();   
    }
	
	void DoDestroySelf()
	{
		//for follow bullet
		if(BulletData.m_bulletIdFollow != 0 && GameManager.Instance.CurrentGameMode == GameMode.SINGLE_PLAYER)
		{
			BulletFactory.Instance.CreateBullet(BulletData.m_bulletIdFollow, FormEntityID, m_thisTransfrom, m_skillId, m_targetId);
		}
		
		//for follow monster
		if(BulletData.m_monsterIdFollow != 0)
		{
            if(!(SceneDataManager.Instance.IsPositionInBlock(m_thisTransfrom.position)))
            {
    			float xValue,yValue;
    			m_thisTransfrom.position.SetToServer(out xValue, out yValue);
    			
    			SMsgFightSummonBullet_CS sMsgFightSummonBullet_CS = new SMsgFightSummonBullet_CS();
    			sMsgFightSummonBullet_CS.uidFighter = FormEntityID;
    			sMsgFightSummonBullet_CS.MonsterTemplateID = (uint)(BulletData.m_monsterIdFollow);
    			sMsgFightSummonBullet_CS.BulletPosX = xValue;
    			sMsgFightSummonBullet_CS.BulletPosY = yValue;

                var direct = transform.TransformDirection(Vector3.forward);
                sMsgFightSummonBullet_CS.BulletDirX = direct.x;
                sMsgFightSummonBullet_CS.BulletDirY= -direct.z;
    			
    			NetServiceManager.Instance.BattleService.SendFightSummonBullet_CS(sMsgFightSummonBullet_CS);
            }
		}
		
        //TraceUtil.Log("Time To Destroy Bullet:" + seconds + " Index:" + this.BulletIndex+"  "+transform.position);
        if (this.BulletIndex != 0)
        {
            BulletManager.Instance.UnRegisteEntity(this.BulletIndex, this.FormEntityID); 
        }
        
	}

	//播放子弹创建音效
	private void PlayBulletBornSound(BulletData data)
	{
		for(int i=0;i<data.m_bornSfxId.Length;i++)
		{
			if(data.m_bornSfxId[i].Id!= "0")
			{
				StartCoroutine(DelayPlayBulletBornSound(data.m_bornSfxId[i].DelayTime,data.m_bornSfxId[i].Id));
			}
		}
	}
	IEnumerator DelayPlayBulletBornSound(float delayTime, string soundName)
	{
		yield return new WaitForSeconds(delayTime);
		SoundManager.Instance.PlaySoundEffect(soundName);
	}


    Vector3 displacement;
    void Update()
    {
        float t = Time.deltaTime;

        if (m_IsFollowType)
        {
            if (m_followTargetTransform == null)
                return;
            
            float zVector = m_followTargetTransform.position.z - transform.position.z;
            float xVector = m_followTargetTransform.position.x - transform.position.x;
            float rad = Mathf.Atan2(zVector, xVector);
            m_motionVector = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * m_followSpeed;
            //子弹自身角度
            float QuaterionY = 90 - rad * Mathf.Rad2Deg;
            Quaternion initQuaternion = m_thisTransfrom.rotation;
            m_thisTransfrom.rotation = Quaternion.Euler(initQuaternion.eulerAngles.x, QuaterionY, initQuaternion.eulerAngles.z);
        }

        if (BulletData.m_mountType != 2)
        {
            displacement = m_motionVector * t + 0.5f * m_accelerationVector * t * t;
            m_motionVector += m_accelerationVector * Time.deltaTime;
        }
		if(BulletData.m_mountType == 2)
		{
			displacement = m_motionVector * Time.deltaTime;
		}
        m_thisTransfrom.Translate(displacement, Space.World);

        if (BulletData!=null&&BulletData.m_mountType == 1)
        {
            m_thisTransfrom.localRotation = Quaternion.identity;
        }
		

		
    }
	
	private IEnumerator DoImpact()
	{
		while(true)
		{
			Impact();
			yield return new WaitForSeconds(BulletData.m_cauculateInverval);
		}
	}
	
	private void Impact()
	{
		ClearSendList();
		//打印子弹测试
        float xValue,yValue;
        this.transform.position.SetToServer(out xValue, out yValue);
        var euler = this.transform.rotation.eulerAngles;
        var d = Quaternion.Euler(euler) * Vector3.forward;
        float xDirect = d.x;
        float yDirect = d.z * -1;
        BulletFactory.Instance.TestBullet(new SMsgPropCreateEntity_SC_Bullet()
            {
                BaseValue = new SMsgPropCreateEntity_SC_BaseValue() { OBJECT_FIELD_ENTRY_ID = BulletData .m_bulletId},
                PosX = (int)xValue,
                PosY = (int)yValue,
                DirX = xDirect,
                DirY = yDirect,
            });
        
        TypeID type;
		EntityModel ownerData = EntityController.Instance.GetEntityModel(FormEntityID, out type );
		if(null == ownerData && BulletData.m_breakType != 2)
		{
			return;	
		}
		//
		if(AffectTarget==4)
		{
			SendFightEffect(ownerData.EntityDataStruct.SMsg_Header.uidEntity);
			DoSend();
			return;
		}
        //20141011动态阻挡修改 新增传送结算 
        if (AffectTarget == 5)
        {
            var targetEntity = MonsterManager.Instance.GetEntityMode(m_targetId);
            if (targetEntity == null)
            {
                targetEntity = PlayerManager.Instance.GetEntityMode(m_targetId);
            }
            if (targetEntity != null)
            {

                SendFightEffect(m_targetId);
                //生成随机传送点，记录发传送的信息 SMsgFightTeleport_CSC 执行传送

                DoSend();
                if(m_impactData.m_teleportLevel!=0)
                {
                    var xRandom = UnityEngine.Random.Range(0, m_impactData.m_teleportArea.x);
                    var zRandom = UnityEngine.Random.Range(0, m_impactData.m_teleportArea.z);
                    var randomPos = m_impactData.m_teleportDestination + new Vector3(xRandom, 0, zRandom);
                    var teleportD = Quaternion.Euler(0, m_impactData.m_teleportAngle, 0) * Vector3.forward;
                    float xteleportDirect = teleportD.x;
                    float zteleportDirect = teleportD.z;
                    
                    Teleport(m_targetId, randomPos, new Vector3(xteleportDirect, 0, zteleportDirect));
                }
                return;

            }
        }
		if(AffectTarget == 6)//20141209 只命中技能目标 怪物施放
		{
			var targetEntity = PlayerManager.Instance.GetEntityMode(m_targetId);
			if (targetEntity != null)
			{
				ImpactTargetPlayer(targetEntity,ownerData, AffectTarget,m_srcLevel,m_srcHit);
				DoSend();
				return;
			}
			var monsterEntity = MonsterManager.Instance.GetEntityMode(m_targetId);
			if(monsterEntity!=null)
			{
                ImpactTargetMonster(monsterEntity,ownerData, AffectTarget,m_srcLevel,m_srcHit);
				DoSend();
			}
			return;
		}

//		int srcLevel= 0, srcHit =0;
//		Hostility hostility=Hostility.FIGHT_HOSTILITY_NEUTRAL;
//		if(type == TypeID.TYPEID_PLAYER)
//		{
//			var playerDataStruct = (IPlayerDataStruct)ownerData.EntityDataStruct;
//			srcHit = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_NICETY;
//			srcLevel = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
//			hostility= (Hostility)playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHT_HOSTILITY;
//		}
//		else if(type == TypeID.TYPEID_MONSTER)
//		{
//			SMsgPropCreateEntity_SC_Monster data = (SMsgPropCreateEntity_SC_Monster)ownerData.EntityDataStruct;
//			srcHit = data.MonsterInvisibleValue.UNIT_FIELD_NICETY;
//			srcLevel = data.MonsterUnitValues.UNIT_FIELD_LEVEL;
//			hostility= (Hostility)data.MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY;
//		}

		//AffectTarget 1-敌方阵营，2-已方阵营，3-所有阵营
        
		ImpactMonsters(ownerData,AffectTarget,m_srcLevel,m_srcHit);
        ImpactPlayer(ownerData,AffectTarget,m_srcLevel,m_srcHit);

        DoSend();
	}

    private void ClearSendList()
    {
        fightEffectTargetIdList.Clear();
        beatBackList.Clear();
        beatFlyList.Clear();
        adsorbList.Clear();
        hordeList.Clear();
    }

    private void DoSend()
    {
        DoSendFightEffect();
        DoSendBeatBack();
        DoSendBeatFly();
        DoSendAdsorb();
        DoSendHorde();
    }
	

	private void ImpactMonsters(EntityModel ownerData, int affectTarget, int srcLevel, int srcHit)
	{
		List<EntityModel> monsterModelList = MonsterManager.Instance.GetMonstersList();

		foreach (EntityModel monster in monsterModelList)
		{
			ImpactTargetMonster(monster,ownerData,affectTarget,srcLevel,srcHit);
		}
	}
	private void ImpactTargetMonster(EntityModel monster,EntityModel ownerData ,int affectTarget,int srcLevel,int srcHit)
	{
		MonsterBehaviour mBehavior = (MonsterBehaviour)monster.Behaviour;
		SMsgPropCreateEntity_SC_Monster mData = (SMsgPropCreateEntity_SC_Monster)monster.EntityDataStruct;
		int destLevel = mData.MonsterUnitValues.UNIT_FIELD_LEVEL;
		int destMiss = mData.MonsterInvisibleValue.UNIT_FIELD_JOOK;
		bool monsterProtected = ( mData.MonsterInvisibleValue.UINT_FILED_PROTECTED == 1 );

        CampType monsterCampType = (CampType)(mData.MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY);

        if( ( AffectTarget == 1 && monsterCampType!=m_ownerCampType )  //作用于敌方阵营，子弹和目标怪物阵营不一致
           ||( AffectTarget == 2 && monsterCampType==m_ownerCampType ) //作用于己方阵营，子弹和目标怪物阵营一致
		   || AffectTarget == 3   //所有
		   || AffectTarget == 6 )
		{
			if(IsInShape(monster.GO.transform.position))
			{
				if( !mBehavior.IsDie 
                   &&(!(mBehavior.Invincible || monsterProtected ))
                   )
				{
                    if( m_ownerCampType == monsterCampType || IsHit(srcLevel, destLevel, srcHit, destMiss))
					{
						SendFightEffect(monster.EntityDataStruct.SMsg_Header.uidEntity);
						if (EctGuideManager.Instance.IsEctypeGuide)
						{
							//发出命中消息，目前在副本引导里会进行命中后减速检测
							RaiseEvent(EventTypeEnum.HitMonsterForGuide.ToString(), new BulletHitData() { BulletId = BulletID, BeFightId = mData.BaseObjectValues.OBJECT_FIELD_ENTRY_ID });
						}
						
						EntityController.Instance.ShowHurtEffect(monster, (uint)BulletID, FormEntityID);
						EntityController.Instance.ShowHurtUiEffect(ownerData, (uint)BulletID);

						//新增 移除怪物 结算
						if(m_impactData.m_damage_type == 3)
						{
							DoDestroySelf();
							return;
						}

						if(m_impactData.m_beatBackSpeed == 0 && m_impactData.m_beatBackAcceleration == 0 && m_impactData.m_beatBackDuration != 0)
						{
							bool noneFreeze = false;
							if(mBehavior.FSMSystem.CurrentStateID == StateID.MonsterAttack )
							{
								MonsterAttackState attackState = ((MonsterAttackState)mBehavior.FSMSystem.CurrentState);
								noneFreeze = attackState.CurrentSkillBase != null 
									&& attackState.CurrentSkillBase.OnFire
										&& attackState.CurrentSkillBase.CurrentActionThresHold == CommonDefineManager.Instance.CommonDefine.NoneFreezeIronLevel;
							}
							//horde
							if(!noneFreeze)
							{
								Horde(monster.EntityDataStruct.SMsg_Header.uidEntity, monster.GO.transform.position);
							}
						}
						else if(mData.MonsterUnitValues.UNIT_FIELD_SHARD == 0 && !mBehavior.IronBody && mData.MonsterInvisibleValue.UNIT_FIELD_ARMOR != 1)  //shield broke
						{
							Vector3 beatBackDir = Vector3.zero;
							if(ownerData == null)//怪物实体不存在的情况 （死亡子弹）
							{
								beatBackDir = GetBeatBackDir(monster.GO.transform, monster.GO.transform);
							}
							else
							{
								beatBackDir = GetBeatBackDir(ownerData.GO.transform, monster.GO.transform);
							}

							
							if(m_impactData.m_beatFlyLevel > 0)
							{
								//fly
								BeatFly(monster.EntityDataStruct.SMsg_Header.uidEntity, monster.GO.transform.position, beatBackDir);
							}
							else if(m_impactData.m_beatBackLevel > 0)
							{
								if(m_impactData.m_beatBackDir == 6)
								{
									//absord
									Absord(monster.EntityDataStruct.SMsg_Header.uidEntity, monster.GO.transform.position, beatBackDir);
								}
								else
								{
									//beat back
									BeatBack(monster.EntityDataStruct.SMsg_Header.uidEntity, monster.GO.transform.position, beatBackDir);
								}
							}
						}
						if(BulletData.m_overParam == 2)
						{
							DoDestroySelf();
						}
					}
					else
					{
						Miss(monster.EntityDataStruct.SMsg_Header.uidEntity);
						if(AffectTarget == 6)//只结算一次
						{
							DoDestroySelf();
						}
					}
				}
			}
		}
	}


	private void ImpactPlayer(EntityModel ownerData , int affectTarget,int srcLevel,int srcHit)
	{
		List<EntityModel> playerModelList = PlayerManager.Instance.PlayerList;

		foreach (EntityModel player in playerModelList)
		{
			ImpactTargetPlayer(player,ownerData,affectTarget,srcLevel,srcHit);
		}
	}
	private void ImpactTargetPlayer(EntityModel player, EntityModel ownerData, int affectTarget,int srcLevel,int srcHit)
	{
		PlayerBehaviour pBehavior = (PlayerBehaviour)player.Behaviour;
		var playerDataStruct = (IPlayerDataStruct)player.EntityDataStruct;
		int destLevel = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
		int destMiss = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_JOOK;
		bool playerProtected = (playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitInvisibleValue.UINT_FILED_PROTECTED == 1);
        CampType playerCampType = (CampType)(playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHT_HOSTILITY);
		
        if( (AffectTarget == 2 && playerCampType == m_ownerCampType )//子弹作用已方，同一阵营
           ||( AffectTarget == 1 && playerCampType != m_ownerCampType )//子弹作用敌方，不在同一阵营
		   || AffectTarget == 3  //所有
		   || AffectTarget == 6 ) 
		{
			if(IsInShape(player.GO.transform.position))
			{
				
				if( !pBehavior.IsDie 
                   && (!(pBehavior.Invincible || playerProtected ))
                   )
				{
					if(m_ownerCampType == playerCampType || IsHit(srcLevel, destLevel, srcHit, destMiss))
					{
						long playerId = player.EntityDataStruct.SMsg_Header.uidEntity;
						SendFightEffect(playerId);
						EntityController.Instance.ShowHurtEffect(player, (uint)BulletID, FormEntityID);
						EntityController.Instance.ShowHurtUiEffect(ownerData, (uint)BulletID);
						
						if(m_impactData.m_beatBackSpeed == 0 && m_impactData.m_beatBackAcceleration == 0 && m_impactData.m_beatBackDuration != 0)
						{
							bool noneFreeze = false;
							if(pBehavior.FSMSystem.CurrentStateID == StateID.PlayerNormalSkill
							   || pBehavior.FSMSystem.CurrentStateID == StateID.PlayerInitiativeSkill)
							{
								noneFreeze = pBehavior.SelectedSkillBase != null
									&& pBehavior.SelectedSkillBase.OnFire
										&& pBehavior.SelectedSkillBase.CurrentActionThresHold == CommonDefineManager.Instance.CommonDefine.NoneFreezeIronLevel;
							}
							//horde
							if(!noneFreeze)
							{
								Horde(player.EntityDataStruct.SMsg_Header.uidEntity, player.GO.transform.position);
							}
						}
						else if(!pBehavior.IsExplodeState && !pBehavior.IronBody && playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_ARMOR != 1)  //shield broke
						{
							Vector3 beatBackDir = Vector3.zero;
							if(ownerData == null)//怪物实体不存在的情况 （死亡子弹）
							{
								beatBackDir = GetBeatBackDir(player.GO.transform, player.GO.transform);
							}
							else
							{
								beatBackDir = GetBeatBackDir(ownerData.GO.transform, player.GO.transform);
							}

							
							if(m_impactData.m_beatFlyLevel > 0)
							{
								//fly
								BeatFly(player.EntityDataStruct.SMsg_Header.uidEntity, player.GO.transform.position, beatBackDir);
							}
							else if(m_impactData.m_beatBackLevel > 0)
							{
								if(m_impactData.m_beatBackDir == 6 )
								{
									//absord
									Absord(player.EntityDataStruct.SMsg_Header.uidEntity, player.GO.transform.position, beatBackDir);
								}
								else if(m_impactData.m_beatBackDir != 7)
								{
									//beat back
									BeatBack(player.EntityDataStruct.SMsg_Header.uidEntity, player.GO.transform.position, beatBackDir);
								}
							}
						}
						if(BulletData.m_overParam == 2)
						{
							DoDestroySelf();
						}
					}
					else
					{
						//miss	
						Miss(player.EntityDataStruct.SMsg_Header.uidEntity);
						if(AffectTarget == 6)//只结算一次
						{
							DoDestroySelf();
						}
					}
				}
			}
		}
	}

    private List<Int64> fightEffectTargetIdList = new List<Int64>();
	private void SendFightEffect(long beFightedId)
	{
		
        fightEffectTargetIdList.Add(beFightedId);
		//NetServiceManager.Instance.BattleService.SendFightEffectCS(sMsgFightEffect_CS);
	}


    private void DoSendFightEffect()
    {
        if(fightEffectTargetIdList.Count == 0)
        {
            return;
        }


		if(BulletData.m_breakType == 2)//死亡子弹
		{
			if(m_ownerCampType != CampType.CAMP_MONSTER)
			{
				return;
			}
			SMsgDeadBulletFightEffect_CS sMsgDeadBulletFightEffect_CS = new SMsgDeadBulletFightEffect_CS();
			sMsgDeadBulletFightEffect_CS.BulletTemplateID = (uint)BulletData.m_bulletId;
			sMsgDeadBulletFightEffect_CS.BulletPosX = m_thisTransfrom.position.x * 10.0f;
			sMsgDeadBulletFightEffect_CS.BulletPosY = m_thisTransfrom.position.z * -10.0f;
			sMsgDeadBulletFightEffect_CS.DamageID = (uint)m_impactData.m_id;
			sMsgDeadBulletFightEffect_CS.SkillId = (uint)m_skillId;
			sMsgDeadBulletFightEffect_CS.byBeFightedNum = Convert.ToByte(fightEffectTargetIdList.Count);
			sMsgDeadBulletFightEffect_CS.bySrcLive = 0;//死亡


			sMsgDeadBulletFightEffect_CS.byPropNum = m_FightSavePropStruct.byPropNum;
			sMsgDeadBulletFightEffect_CS.dwProp = m_FightSavePropStruct.nProp;

			/*
			var model = MonsterManager.Instance.GetEntityMode(FormEntityID);
			SMsgPropCreateEntity_SC_Monster monsterProp = (SMsgPropCreateEntity_SC_Monster)model.EntityDataStruct;
			if(m_impactData.m_damage_type == 1)//普通结算
			{
				sMsgDeadBulletFightEffect_CS.byPropNum = 6;
				sMsgDeadBulletFightEffect_CS.dwProp = new int[6];

				sMsgDeadBulletFightEffect_CS.dwProp[0] = monsterProp.MonsterInvisibleValue.UNIT_FIELD_ATTACK;
				sMsgDeadBulletFightEffect_CS.dwProp[1] = monsterProp.MonsterInvisibleValue.UNIT_FIELD_NICETY;
				sMsgDeadBulletFightEffect_CS.dwProp[2] = monsterProp.MonsterInvisibleValue.UNIT_FIELD_BURST;
				sMsgDeadBulletFightEffect_CS.dwProp[3] = monsterProp.MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY;
				sMsgDeadBulletFightEffect_CS.dwProp[4] = monsterProp.MonsterUnitValues.UNIT_FIELD_LEVEL;
				sMsgDeadBulletFightEffect_CS.dwProp[5] = monsterProp.MonsterInvisibleValue.UINT_FIELD_DEFBREAK;
			}
			else if(m_impactData.m_damage_type == 2)
			{
				sMsgDeadBulletFightEffect_CS.byPropNum = (byte)m_impactData.m_affect_prop.Length;
				sMsgDeadBulletFightEffect_CS.dwProp = new int[sMsgDeadBulletFightEffect_CS.byPropNum];
				
				for(int i=0;i<sMsgDeadBulletFightEffect_CS.byPropNum;i++)
				{
					if(m_impactData.m_affect_src[i] == 1)
					{
						int propIndex = CommonDefineManager.Instance.GetPropKey(m_impactData.m_affect_prop[i]);
						if(propIndex == -1)
						{
							Debug.LogError("Propkey.xml is not exit "+m_impactData.m_affect_prop[i]);
							return;
						}
						sMsgDeadBulletFightEffect_CS.dwProp[i] = monsterProp.GetValue(propIndex);
					}
					else
					{
						sMsgDeadBulletFightEffect_CS.dwProp[i] = 0;
					}
				}
			}
			*/

			sMsgDeadBulletFightEffect_CS.uidBeFightedList = fightEffectTargetIdList;
			sMsgDeadBulletFightEffect_CS.uidFighter = FormEntityID;

			NetServiceManager.Instance.BattleService.SendDeadBulletFightEffectCS(sMsgDeadBulletFightEffect_CS);
		}
		else
		{
			SMsgFightEffect_CS  sMsgFightEffect_CS = new SMsgFightEffect_CS();
			sMsgFightEffect_CS.BulletTemplateID = (uint)BulletData.m_bulletId;
			sMsgFightEffect_CS.BulletPosX = m_thisTransfrom.position.x * 10.0f;
			sMsgFightEffect_CS.BulletPosY = m_thisTransfrom.position.z * -10.0f;
			sMsgFightEffect_CS.DamageID = (uint)m_impactData.m_id;
			sMsgFightEffect_CS.SkillId = (uint)m_skillId;
			sMsgFightEffect_CS.byBeFightedNum = Convert.ToByte(fightEffectTargetIdList.Count);
			sMsgFightEffect_CS.bySrcLive = 1;
			sMsgFightEffect_CS.uidBeFightedList = fightEffectTargetIdList;
			sMsgFightEffect_CS.uidFighter = FormEntityID;
			NetServiceManager.Instance.BattleService.SendFightEffectCS(sMsgFightEffect_CS);
		}       		             

    }
	
	private Vector3 GetBeatBackDir( Transform ownTrans, Transform beBeatTrans)
	{
		Vector3 beatBackDir = Vector3.forward;
								
		if(m_impactData.m_beatBackDir == 1)
		{
			beatBackDir = beBeatTrans.position - m_thisTransfrom.position;
			
			
		}
		else if(m_impactData.m_beatBackDir == 2)
		{
			beatBackDir = m_thisTransfrom.position - beBeatTrans.position;
		}
		else if(m_impactData.m_beatBackDir == 3)
		{
			beatBackDir = beBeatTrans.position - ownTrans.position;
		}
		else if(m_impactData.m_beatBackDir == 4)
		{
			beatBackDir = ownTrans.position -  beBeatTrans.position;
		}
		else if(m_impactData.m_beatBackDir == 5)
		{
            beatBackDir = m_cachedBeatBackDir;//ownTrans.TransformDirection(Vector3.forward);
		}
		else if(m_impactData.m_beatBackDir == 6)
		{
			beatBackDir = m_thisTransfrom.position - beBeatTrans.position;
		}

		if(beatBackDir == Vector3.zero)//打击方和受击方位置重叠的情况
		{
			beatBackDir = beBeatTrans.TransformDirection(Vector3.back);
		}

		beatBackDir.Normalize();
		return beatBackDir;
	}
	
	private void Miss(long uid)
	{
		//miss
		SMsgBattleCalculateEffect_SC sMsgBattleCalculateEffect_SC = new SMsgBattleCalculateEffect_SC();
		sMsgBattleCalculateEffect_SC.BulletTemplateID = (UInt32)(BulletData.m_bulletId);
		sMsgBattleCalculateEffect_SC.EffectType = 2;
		sMsgBattleCalculateEffect_SC.uidFighter = uid;
		sMsgBattleCalculateEffect_SC.uidEffectParam = FormEntityID;
		sMsgBattleCalculateEffect_SC.Value = 0;
		RaiseEvent(EventTypeEnum.S_CSMsgFightFightToResult.ToString(), sMsgBattleCalculateEffect_SC);
		//上发服务器
		NetServiceManager.Instance.BattleService.SendFightBattleMissEffect(new SMsgFightMissEffect_CS(){
			uidFighter = uid,
			uidMisser = FormEntityID,
		});
	}

	private void BeatBack(long uid, Vector3 pos, Vector3 dir)
	{
		SMsgBattleBeatBack_SC sMsgBattleBeatBack_SC = new SMsgBattleBeatBack_SC();
		sMsgBattleBeatBack_SC.uidFighter = uid;
		sMsgBattleBeatBack_SC.PosX = pos.x * 10.0f;
		sMsgBattleBeatBack_SC.PosY = -pos.z * 10.0f;
		sMsgBattleBeatBack_SC.DirX = dir.x;
		sMsgBattleBeatBack_SC.DirY = -dir.z;
		sMsgBattleBeatBack_SC.speed = (int)(m_impactData.m_beatBackSpeed * 10.0f);
		sMsgBattleBeatBack_SC.Accelerated = (int)(m_impactData.m_beatBackAcceleration * 10.0f);
		sMsgBattleBeatBack_SC.time = (int)(m_impactData.m_beatBackDuration);
		
		RaiseEvent(EventTypeEnum.BeatBack.ToString(), sMsgBattleBeatBack_SC);
		
		//send to server
		SMsgFightBeatBack_CS sMsgFightBeatBack_CS = new SMsgFightBeatBack_CS();
		sMsgFightBeatBack_CS.uidFighter = uid;
		sMsgFightBeatBack_CS.byType = 1;
		sMsgFightBeatBack_CS.hitedPosX = pos.x * 10.0f;
		sMsgFightBeatBack_CS.hitedPosY = -pos.z *10.0f;
		//NetServiceManager.Instance.BattleService.SendFightBeatBackCS(sMsgFightBeatBack_CS);
        beatBackList.Add(sMsgFightBeatBack_CS);
		
	}

    private List<SMsgFightBeatBack_CS> beatBackList = new List<SMsgFightBeatBack_CS>();
    private void DoSendBeatBack()
    {
        if(beatBackList.Count == 0)
        {
            return;
        }

        SMsgBeatBackContextNum_CS sMsgBeatBackContextNum_CS = new SMsgBeatBackContextNum_CS();
        sMsgBeatBackContextNum_CS.byContextNum = Convert.ToByte( beatBackList.Count );
        sMsgBeatBackContextNum_CS.list = beatBackList;
        NetServiceManager.Instance.BattleService.SendFightBeatBackCS(sMsgBeatBackContextNum_CS);

    }
    private void Teleport(long uid, Vector3 pos, Vector3 dir)
    {
        SMsgFightTeleport_CSC sMsgFightTeleport_CSC = new SMsgFightTeleport_CSC();
        sMsgFightTeleport_CSC.uidFighter = uid;
        sMsgFightTeleport_CSC.ptPosX = pos.x * 10.0f;
        sMsgFightTeleport_CSC.ptPosY = pos.z * 10.0f;
        sMsgFightTeleport_CSC.ptDirectX = dir.x;
        sMsgFightTeleport_CSC.ptDirectY = dir.z;
        RaiseEvent(EventTypeEnum.Teleport.ToString(), sMsgFightTeleport_CSC);

        NetServiceManager.Instance.BattleService.SendFightTeleportCS(sMsgFightTeleport_CSC);
    }
	private void BeatFly(long uid, Vector3 pos, Vector3 dir)
	{
		SMsgFightHitFly_SC sMsgFightHitFly_SC = new SMsgFightHitFly_SC();
		sMsgFightHitFly_SC.uidFighter = uid;
		sMsgFightHitFly_SC.hitedPosX = pos.x * 10.0f;
		sMsgFightHitFly_SC.hitedPosY = -pos.z * 10.0f;
		sMsgFightHitFly_SC.directionX = dir.x;
		sMsgFightHitFly_SC.directionY = -dir.z;
		sMsgFightHitFly_SC.lSpeed = (int)(m_impactData.m_beatFlySpeed * 10.0f);
		sMsgFightHitFly_SC.Accelerated = (int)(m_impactData.m_beatFlyAcceleration * 10.0f);
		sMsgFightHitFly_SC.hSpedd = (int)(m_impactData.m_beatFlyVerticalSpeed * 10.0f);
		RaiseEvent(EventTypeEnum.FightFly.ToString(), sMsgFightHitFly_SC);
		
		//send to server
		SMsgFightHitFly_CS sMsgFightHitFly_CS = new SMsgFightHitFly_CS();
		sMsgFightHitFly_CS.uidFighter = uid;
		sMsgFightHitFly_CS.byType = 1;
		sMsgFightHitFly_CS.hitedPosX = pos.x * 10.0f;
		sMsgFightHitFly_CS.hitedPosY = -pos.z *10.0f;
		//NetServiceManager.Instance.BattleService.SendFightHitFlyCS(sMsgFightHitFly_CS);
        beatFlyList.Add(sMsgFightHitFly_CS);
	}

    List<SMsgFightHitFly_CS> beatFlyList = new List<SMsgFightHitFly_CS>();
    private void DoSendBeatFly()
    {
        if(beatFlyList.Count == 0)
        {
            return;
        }

        SMsgHitFlyContextNum_CS sMsgHitFlyContextNum_CS = new SMsgHitFlyContextNum_CS();
        sMsgHitFlyContextNum_CS.byContextNum = Convert.ToByte( beatFlyList.Count );
        sMsgHitFlyContextNum_CS.list = beatFlyList;
        NetServiceManager.Instance.BattleService.SendFightHitFlyCS(sMsgHitFlyContextNum_CS);

    }
	
	private void Absord(long uid, Vector3 pos, Vector3 dir)
	{
		float distance = Vector3.Distance(m_thisTransfrom.position, pos);
		if(distance > m_impactData.m_beatBackSpeed && m_impactData.m_beatBackDuration != 0)
		{
			//int v = (int)(distance *m_impactData.m_beatBackAcceleration/m_impactData.m_beatBackDuration*10.0f);
			int v = (int)(distance/(m_impactData.m_beatBackDuration/1000.0f));
			SMsgBattleBeAdsorb_SC sMsgBattleBeAdsorb_SC = new SMsgBattleBeAdsorb_SC();
			sMsgBattleBeAdsorb_SC.uidFighter = uid;
			sMsgBattleBeAdsorb_SC.PosX = pos.x*10.0f;
			sMsgBattleBeAdsorb_SC.PosY = -pos.z *10.0f;
			sMsgBattleBeAdsorb_SC.DirX = dir.x;
			sMsgBattleBeAdsorb_SC.DirY = -dir.z;
			sMsgBattleBeAdsorb_SC.speed = v*10;
			sMsgBattleBeAdsorb_SC.time = (int)(m_impactData.m_beatBackDuration);
			
			RaiseEvent(EventTypeEnum.BeAdsorb.ToString(), sMsgBattleBeAdsorb_SC);
			
			//send to server
			SMsgFightAdsorption_CS sMsgFightAdsorption_CS = new SMsgFightAdsorption_CS();
			sMsgFightAdsorption_CS.uidFighter = uid;
			sMsgFightAdsorption_CS.hitedPosX = pos.x * 10.0f;
			sMsgFightAdsorption_CS.hitedPosY = -pos.z * 10.0f;
			sMsgFightAdsorption_CS.byType = 1;
			
			//NetServiceManager.Instance.BattleService.SendFightAdsorption_CS(sMsgFightAdsorption_CS);
            adsorbList.Add(sMsgFightAdsorption_CS);
		}
	}

    private List<SMsgFightAdsorption_CS> adsorbList = new List<SMsgFightAdsorption_CS>();
    private void DoSendAdsorb()
    {
        if(adsorbList.Count == 0)
        {
            return;
        }

        SMsgAdsorptionContextNum_CS sMsgAdsorptionContextNum_CS = new SMsgAdsorptionContextNum_CS();
        sMsgAdsorptionContextNum_CS.byContextNum = Convert.ToByte( adsorbList.Count );
        sMsgAdsorptionContextNum_CS.list = adsorbList;
        NetServiceManager.Instance.BattleService.SendFightAdsorption_CS(sMsgAdsorptionContextNum_CS);
    }

	
	private void Horde(long uid , Vector3 pos)
	{


		SMsgFightHorde_SC sMsgFightHorde_SC = new SMsgFightHorde_SC();
		sMsgFightHorde_SC.uidFighter = uid;
		sMsgFightHorde_SC.HitedPosX = pos.x*10.0f;
		sMsgFightHorde_SC.HitedPosY = -pos.z*10.0f;
		sMsgFightHorde_SC.HordeTime = (int)m_impactData.m_beatBackDuration;
		
		RaiseEvent(EventTypeEnum.EntityHorde.ToString(), sMsgFightHorde_SC);
		
		//send to server
		SMsgFightHorde_CS sMsgFightHorde_CS = new SMsgFightHorde_CS();
		sMsgFightHorde_CS.byType = 1;
		sMsgFightHorde_CS.uidFighter = uid;
		sMsgFightHorde_CS.hitedPosX = pos.x*10.0f;
		sMsgFightHorde_CS.hitedPosY = -pos.z*10.0f;
		
		//NetServiceManager.Instance.BattleService.SendFightHorde_CS(sMsgFightHorde_CS);
        hordeList.Add(sMsgFightHorde_CS);
	}

    private List<SMsgFightHorde_CS> hordeList = new List<SMsgFightHorde_CS>();
    private void DoSendHorde()
    {
        if(hordeList.Count == 0)
        {
            return;
        }

        SMsgHordeContextNum_CS sMsgHordeContextNum_CS = new SMsgHordeContextNum_CS();
        sMsgHordeContextNum_CS.byContextNum = Convert.ToByte( hordeList.Count);
        sMsgHordeContextNum_CS.list = hordeList;
        NetServiceManager.Instance.BattleService.SendFightHorde_CS(sMsgHordeContextNum_CS);
    }
	
	
	
	private bool IsHit( int srcLevel, int destLevel, int srcHit, int destMiss )
	{
		
//		int modulus = 22 * destLevel * destMiss;
//		
//		int dodge = modulus - (srcHit * 22 * destLevel + 10 * destMiss * (srcLevel - destLevel));
//		
//		int rnd = UnityEngine.Random.Range(0, modulus);
//		if (dodge > rnd)
//		{
//			
//			return false;
//		}
//   		 return true;

        int nDuck1 = (srcHit * 1000) / destMiss;
        int nDuck2 = (1000 * 1000 * (srcLevel - destLevel)) / (CommonDefineManager.Instance.CommonDefine.DodgePara * destLevel);
        
        int nDuck = nDuck1 + nDuck2;
        
        return nDuck >= UnityEngine.Random.Range(0, 1000);

	}

   

    protected override void RegisterEventHandler()
    {
        throw new System.NotImplementedException();
    }
	
	private bool IsInShape(Vector3 pointPos)
	{
		Vector3 posLocal = m_thisTransfrom.InverseTransformPoint(pointPos);
		
		if(BulletData.m_shapeParam1 == 1)
		{
			return IsInCircle(pointPos,(float)(BulletData.m_shapeParam2) );
		}
		else if(BulletData.m_shapeParam1 == 2)
		{
			return IsInRect(posLocal, (float)(BulletData.m_shapeParam3), (float)(BulletData.m_shapeParam2));
		}
		else if(BulletData.m_shapeParam1 == 3)
		{
			return IsInFan(posLocal, (float)(BulletData.m_shapeParam2), (float)(BulletData.m_shapeParam3));
		}
		return false;	
	}
	
	private bool IsInRect(Vector3 localPointPos, float width, float height)
	{
		float left = -width/2.0f;
		float right = width/2.0f;
		float up = height/2.0f;
		float down = -height/2.0f;
		if(localPointPos.x > left && localPointPos.x < right
			&& localPointPos.z > down && localPointPos.z < up)
		{
			return true;
		}
		
		return false;
	}
	
	private bool IsInCircle(Vector3 pointPos, float r)
	{
		Vector3 selfPos = m_thisTransfrom.position;
		float deltaX = pointPos.x - selfPos.x;
		float deltaY = pointPos.z - selfPos.z;
		if( (deltaX* deltaX + deltaY*deltaY) <= (r*r) )
		{
			return true;	
		}
		return false;
	}
	
	private bool IsInFan(Vector3 localPointPos, float r, float angle)
	{
		Vector3 selfPos = m_thisTransfrom.position;
		float deltaX = localPointPos.x;
		float deltaY = localPointPos.z;
		bool inAngle = false;
		if( Mathf.Abs(Vector3.Angle(Vector3.forward, localPointPos)) < angle/2)
		{
			inAngle = true;
		}
		if( inAngle && (deltaX* deltaX + deltaY*deltaY) <= (r*r)  )
		{
			return true;	
		}
		return false;	
	}
	
}