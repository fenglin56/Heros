using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MonsterBehaviour : RoleBehaviour, IEntityDataManager	
{
    public GameObject NormalStatus;
    public GameObject SplitStatus;
    [HideInInspector]
    public bool SplitToDie = false;
    [HideInInspector]
    public EnemyHealthBar EnemyHealthBar;    

	public static bool UsePathSmooth = false;
	
	private SMsgActionMove_SCS m_lastNode;
	private SMsgActionMove_SCS m_currentNode;
    private bool m_beLocked;
    private float m_maxShieldValue;
    private float m_currentShieldValue;
    
	private Vector3 ?m_clientEndPos = null;
	public Vector3 ?ClientEndPos
	{
		get { return m_clientEndPos; }	
	}
	
	public SMsgActionMove_SCS CurrentNode
	{
		get { return m_currentNode; }	
	}
	private MoveSmooth m_smooth = new MoveSmooth();
	
	public MoveSmooth Smooth
	{
		get {return m_smooth;}	
	}
	[HideInInspector]
	public bool ClientMove = false;
	
    private SMsgActionMove_SCS m_SMsgActionMove_SC;  //生物移动结构体

    public MonsterConfigData m_MonsterConfigData { get; private set; } //怪物配置属性
    public float MonsterShieldValue
    {
        get { return this.m_maxShieldValue; }
    }

    public float WalkSpeed
    {
        get { 
			TypeID type;
			SMsgPropCreateEntity_SC_Monster data = (SMsgPropCreateEntity_SC_Monster)(EntityController.Instance.GetEntityModel(this.RoleDataModel.SMsg_Header.uidEntity, out type).EntityDataStruct);
			return data.MonsterUnitValues.UNIT_FIELD_SPEED *0.1f;
			
			//return ((SMsgPropCreateEntity_SC_Monster)this.m_roleDataModel).MonsterUnitValues.UNIT_FIELD_SPEED*0.1f; }
		}
    }
    public float RunSpeed
    {
        get { return ((SMsgPropCreateEntity_SC_Monster)this.RoleDataModel).MonsterValues.MONSTER_FIELD_RUNSPEED * 0.1f; }
    }
    public Vector3? WalkToPosition { get; set; }

    public SkinnedMeshRenderer m_MonsterMeshRenderer { get; private set; }

    public List<SkillBase> m_SkillBaseList{get;private set;}

    private FSMSystem m_FSMSystem;
    public FSMSystem FSMSystem { get { return m_FSMSystem; } }

    private MonsterBeAttackState m_MonsterBeAttackState;
    private MonsterAttackState m_MonsterAttackState;
    private MonsterBeHitFlyState m_MonsterBeHitFlyState;
    //private MonsterBeAdsorbState m_Monster
	
	private List<Vector3> monsterMoveNodes = new List<Vector3>();
	private List<Vector3> nodes = new List<Vector3>();

    private List<Renderer> m_monsterRendererDatas;

    private const float m_minBoundValueX = 100f;
    private const float m_minBoundValueY = 100f;
    private float m_sceneWidth;
    private float m_sceneHeight;
    private Transform m_attachPoint;
    private GameObject m_arrow;
    //private ArrowBehaviour m_ArrowBehaviour;

    private bool m_isDie;
    public bool IsDie
    {
        get { return m_isDie; }
    }
	
	public bool Invincible = false;
	public bool IronBody = false;

    void Awake()
    {
        if (this.NormalStatus != null)
        {
            this.NormalStatus.SetActive(true);
        }
        else
        {
            this.NormalStatus = gameObject;
        }
        if (this.SplitStatus != null)
        {
            this.SplitStatus.SetActive(false);
        }
		this.CacheHurtFlash();
        this.CacheTransform();
        this.RegisterEventHandler();
        GetMeshRenderer();
        InitBoundHandle();


        transform.RecursiveFindObject("BloodBarMP", out m_attachPoint);

        //InitFSM();  
    }

    void OnDestroy()
    {
        RemoveAllEvent();
    }
    //public override void CacheTransform()
    //{
    //    ThisTransform = NormalStatus.transform;
    //}
    private void InitBoundHandle()
    {
		if(GameManager.Instance == null)
			return;
		
        int SceneConfigId = (int)GameManager.Instance.GetCurSceneMapID;
        SceneConfigData sData = EctypeConfigManager.Instance.SceneConfigList[SceneConfigId];
        m_sceneWidth = sData._mapWidth;
        m_sceneHeight = sData._mapHeight;
    }
    public void InitFSM()
    {
        CachEntityAnimation();

        m_FSMSystem = new FSMSystem(this);

        MonsterMoveState moveState = new MonsterMoveState();
        m_FSMSystem.AddState(moveState);
        MonsterIdleState idleState = new MonsterIdleState();
        m_FSMSystem.AddState(idleState);
        m_MonsterAttackState = new MonsterAttackState();
        m_FSMSystem.AddState(m_MonsterAttackState);
        m_MonsterBeAttackState = new MonsterBeAttackState();
        m_FSMSystem.AddState(m_MonsterBeAttackState);
        MonsterDieState dieState = new MonsterDieState();
        m_FSMSystem.AddState(dieState);
        MonsterStandState standState = new MonsterStandState();
        m_FSMSystem.AddState(standState);
        m_MonsterBeHitFlyState = new MonsterBeHitFlyState();
        m_FSMSystem.AddState(m_MonsterBeHitFlyState);
        MonsterBeAdsorbState monsterBeAdsorbState = new MonsterBeAdsorbState();
        m_FSMSystem.AddState(monsterBeAdsorbState);
        m_FSMSystem.AddState(new MonsterBeHordeState());

        m_FSMSystem.PerformTransition(Transition.MonsterToIdle);
    }

    private void AddSkillBases(MonsterConfigData monsterConfigData)
    {
        m_SkillBaseList = new List<SkillBase>();
        monsterConfigData._skillGroup.ApplyAllItem(p =>
        {
			if(!m_SkillBaseList.Exists(skill => skill.SkillId == p._skillID))
			{
            	SkillBase skillBase = RoleGenerate.AttachSkill(gameObject, p._skillID);
                skillBase.SetUserID = this.RoleDataModel.SMsg_Header.uidEntity;

            	m_SkillBaseList.Add(skillBase);
			}
        });
    }

    void Update()
    {
        if(m_FSMSystem!=null&&m_FSMSystem.CurrentState!=null)
        {
            m_FSMSystem.CurrentState.Reason();
            m_FSMSystem.CurrentState.Act();
        }
    }
    

    private void GetMeshRenderer()
    {
        m_MonsterMeshRenderer = ThisTransform.GetComponentInChildren<SkinnedMeshRenderer>();
    }
    /// <summary>
    /// 防护值恢复
    /// </summary>
    public void ReplyMonsterShieldValue()
    {
       this.m_maxShieldValue = m_MonsterConfigData.m_shieldpoint;
    }
    public void SetMonsterConfigData(MonsterConfigData configData)
    {
        m_MonsterConfigData = configData;
        ReplyMonsterShieldValue();
        AddSkillBases(m_MonsterConfigData);
    }

    //public void SetArrowBehaviour(ArrowBehaviour arrowBehaviour)
    //{
    //    this.m_ArrowBehaviour = arrowBehaviour;
    //}

    public void EntityMove(SMsgActionMove_SCS sMsgActionMove)
    {		
		monsterMoveNodes.Add(new Vector3(sMsgActionMove.floatX/10.0f, 0.1f, sMsgActionMove.floatY/-10.0f));
		
        if (m_FSMSystem.CurrentStateID == StateID.MonsterBeHitFly 
			|| m_FSMSystem.CurrentStateID == StateID.MonsterStand
			|| m_FSMSystem.CurrentStateID == StateID.MonsterBeAttacked
			|| m_FSMSystem.CurrentStateID == StateID.MonsterAttack )
			
		{
			TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"wrong node");
            return;
		}
		
		if (this.RoleDataModel.SMsg_Header.uidEntity == sMsgActionMove.uidEntity && MonsterBehaviour.UsePathSmooth)
        {
			m_currentNode = sMsgActionMove;
			Vector2 lastPos = new Vector2(m_lastNode.floatX, m_lastNode.floatY);
			Vector2 currentPos = new Vector2(ThisTransform.position.x*10.0f, ThisTransform.position.z*(-10.0f));
			if(m_currentNode.fSpeed > 0.1f)//Vector2.Distance( currentPos , lastPos) > 0.1f && lastPos != Vector2.zero)
			{
				if(m_FSMSystem.CurrentStateID == StateID.MonsterIdle)	
				{
					m_smooth.Init(ThisTransform.position,
							  ThisTransform.TransformDirection(Vector3.forward) * WalkSpeed, 
							  new Vector3(sMsgActionMove.floatX/10.0f, 0, -sMsgActionMove.floatY/10.0f), 
					          new Vector3(sMsgActionMove.fDirectX, 0, -sMsgActionMove.fDirectY) * sMsgActionMove.fSpeed/10.0f, 
							  ConfigDefineManager.TIME_MOVE_SYNC_FOWARD);
					nodes.Clear();
					for(int i = 0; i<=10; i++)
					{
						
						Vector3 nodepos = m_smooth.GetCurrentPos(0.1f*i);
						nodes.Add(new Vector3(nodepos.x, 1, nodepos.z));
					}
					
					
					m_FSMSystem.PerformTransition(Transition.MonsterToMove);
					MonsterMoveState state = (MonsterMoveState)m_FSMSystem.CurrentState;
					state.ResetSmooth(ConfigDefineManager.TIME_MOVE_SYNC_FOWARD);
					
				}
				else if(m_FSMSystem.CurrentStateID == StateID.MonsterMove)
				{
					MonsterMoveState state = (MonsterMoveState)m_FSMSystem.CurrentState;
					state.ResetSmooth(ConfigDefineManager.TIME_MOVE_SYNC_FOWARD);
					m_smooth.Init(ThisTransform.position,
							  ThisTransform.TransformDirection(Vector3.forward) * WalkSpeed, 
							  new Vector3(sMsgActionMove.floatX/10.0f, 0, -sMsgActionMove.floatY/10.0f), 
					          new Vector3(sMsgActionMove.fDirectX, 0, -sMsgActionMove.fDirectY) * sMsgActionMove.fSpeed/10.0f, 
							  ConfigDefineManager.TIME_MOVE_SYNC_FOWARD);
					nodes.Clear();
					for(int i = 0; i<=10; i++)
					{
						Vector3 nodepos = m_smooth.GetCurrentPos(0.1f*i);
						nodes.Add(new Vector3(nodepos.x, 1, nodepos.z));
					}
				}
					
				m_clientEndPos = null;
				ClientMove = true;
			}
			else
			{
				m_clientEndPos= new Vector3(CurrentNode.floatX/10.0f,0, CurrentNode.floatY/-10.0f);
				ClientMove = false;
			}
			m_lastNode = m_currentNode;
		
		}
		
        WalkToPosition = ThisTransform.position;
        WalkToPosition = WalkToPosition.Value.GetFromServer(sMsgActionMove.floatX, 0.1f, sMsgActionMove.floatY, -0.1f);

    }

    #region 根据路点移动
    public Queue<Vector2> PointQueue = new Queue<Vector2>();
    //(测试)怪物路点移动
    public void MonsterMove(SMsgActionMonsterMove_SC sMsgActionMonsterMove)
    {
        //TraceUtil.Log("==>进入到MonsterMove");

        if (m_FSMSystem.CurrentStateID == StateID.MonsterBeHitFly
            || m_FSMSystem.CurrentStateID == StateID.MonsterStand
            || m_FSMSystem.CurrentStateID == StateID.MonsterBeAttacked)
            return;
		monsterMoveNodes.Clear();
        PointQueue.Clear();
        sMsgActionMonsterMove.dwPoints.ApplyAllItem(p=>
            {
				monsterMoveNodes.Add(new Vector3(p.x * 0.1f, 1.0f, -0.1f* p.y));
                PointQueue.Enqueue(p);
            });
        int length = sMsgActionMonsterMove.dwPoints.Length;
        var endPoint = sMsgActionMonsterMove.dwPoints[length - 1];
        string result = "";
        if (SceneDataManager.Instance.IsPositionInBlock(new Vector3(endPoint.x * 0.1f, 1.0f, -0.1f * endPoint.y)))
        {
            //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Move To Block !!!!!!!!!   " + (sMsgActionMonsterMove.uidMonster << 32 >> 32).ToString() + "---  pos: x=" +p.x + "  y="+ (p.y) );
            result = "进入阻挡点";
        }
        else
        {
            result = "正常";
        }
        Log.Instance.StartLog();        
        Log.Instance.AddLog((sMsgActionMonsterMove.uidMonster << 32 >> 32).ToString(), result, sMsgActionMonsterMove.dwPoints[0].ToString(), sMsgActionMonsterMove.dwPoints[length - 1].ToString());
        Log.Instance.AppendLine();  //换行写入并保存


        if (PointQueue.Count > 0)
        {
            Vector2 point = PointQueue.Dequeue();
            WalkToPosition = Vector3.zero;
            WalkToPosition = WalkToPosition.Value.GetFromServer(point.x, 0.1f, point.y, -0.1f);            
        }
        var list = PointQueue.ToList();
        int listLength = list.Count;
        var pointValue = list[listLength - 1];

        if (Log.IsPrint)
        {
            //Log.Instance.StartLog();
            //Log.Instance.AddLog((sMsgActionMonsterMove.uidMonster << 32 >> 32).ToString(), "收到路径点",
            //    pointValue.x.ToString(), pointValue.y.ToString(), "0", "0");
            //Log.Instance.AppendLine();
        }
        
        //\
        //PointQueue.ApplyAllItem(p =>
        //    {
        //        monsterMoveNodes.Add(new Vector3( p.x / 10.0f, 0.1f, p.y / -10.0f));
        //    });

    }
    #endregion

    public void BeHorde(SMsgFightHorde_SC sMsgFightHorde_SC)
    {
        //停掉动画，重置位置        
        m_AttachAnimations.ApplyAllItem(P => P.AnimComponent.Stop());
    }
    
    //受击退
    public void BeAttacked(SMsgBattleBeatBack_SC sMsgBattleBeatBack_SC)
    {      
		monsterMoveNodes.Clear();
		
		if(m_FSMSystem.CurrentStateID == StateID.MonsterBeAttacked
			|| m_FSMSystem.CurrentStateID == StateID.MonsterBeAdsorb
			|| m_FSMSystem.CurrentStateID == StateID.MonsterBeHitFly
			|| m_FSMSystem.CurrentStateID == StateID.MonsterBeHorde
            ||m_FSMSystem.CurrentStateID == StateID.MonsterStand
			)
		{
			((MonsterFsmState)m_FSMSystem.CurrentState).DoNotSendBeatEnd = true;
		}
		else
		{
			((MonsterFsmState)m_FSMSystem.CurrentState).DoNotSendBeatEnd = false;
		}
		
        m_MonsterBeAttackState.BeAttacked(sMsgBattleBeatBack_SC);
        m_FSMSystem.PerformTransition(Transition.MonsterToBeAttacked);
    }
    /// <summary>
    /// 20141011 动态阻挡传送结算 
    /// </summary>
    /// <param name="sMsgFightTeleport_CSC"></param>
    public void BeTeleport(SMsgFightTeleport_CSC sMsgFightTeleport_CSC)
    {
        //	传送前先打断目标所有动作，将其置为站立状态，无视霸体和无敌
        m_FSMSystem.PerformTransition(Transition.MonsterToIdle);
        ThisTransform.position = new Vector3(0.1f * sMsgFightTeleport_CSC.ptPosX, ThisTransform.position.y, -0.1f * sMsgFightTeleport_CSC.ptPosY);
        var euler = new Vector3(0.1f * sMsgFightTeleport_CSC.ptDirectX, 0, -0.1f * sMsgFightTeleport_CSC.ptDirectY);
        ThisTransform.rotation = Quaternion.Euler(euler);
    }
    //受击飞
    public void BeHitFly(SMsgFightHitFly_SC sMsgFightHitFly_SC)
    {        
        //if (m_FSMSystem.CurrentStateID == StateID.MonsterBeHitFly)
        //{
        //    m_MonsterBeHitFlyState.BeHitWhenFlying(sMsgFightHitFly_SC);
        //}
        //else
        //{
        //    m_MonsterBeHitFlyState.BeHitFly(sMsgFightHitFly_SC);
        //    m_FSMSystem.PerformTransition(Transition.MonsterToBeHitFly);
        //}
		if(m_FSMSystem.CurrentStateID == StateID.MonsterBeAttacked
			|| m_FSMSystem.CurrentStateID == StateID.MonsterBeAdsorb
			|| m_FSMSystem.CurrentStateID == StateID.MonsterBeHitFly
			|| m_FSMSystem.CurrentStateID == StateID.MonsterBeHorde
            ||m_FSMSystem.CurrentStateID == StateID.MonsterStand

			)
		{
			((MonsterFsmState)m_FSMSystem.CurrentState).DoNotSendBeatEnd = true;
		}
		else
		{
			((MonsterFsmState)m_FSMSystem.CurrentState).DoNotSendBeatEnd = false;
		}
		
        m_MonsterBeHitFlyState.BeHitFly(sMsgFightHitFly_SC);
        m_FSMSystem.PerformTransition(Transition.MonsterToBeHitFly);
    }
    //攻击
    public void FightCommand(SMsgBattleCommand smsgBattleCommand)
    {
        m_MonsterAttackState.Attack(smsgBattleCommand);
		if(m_FSMSystem.CurrentStateID == StateID.MonsterAttack)
		{
            BulletManager.Instance.TryDestroyBreakBullets(this.RoleDataModel.SMsg_Header.uidEntity);
		}
        m_FSMSystem.PerformTransition(Transition.MonsterToAttack);
    }
	
	public void SingleFightCommand(SMsgFightCommand_SC sMsgFightCommand_SC)
	{
		if(m_FSMSystem.CurrentStateID == StateID.MonsterAttack)
		{
			int currentLevel = ((MonsterAttackState)m_FSMSystem.CurrentState).CurrentSkillBase.SkillData.m_breakLevel;
			int nextLevel = SkillDataManager.Instance.GetSkillConfigData(sMsgFightCommand_SC.nFightCode).m_breakLevel;
			if(nextLevel <= currentLevel)
			{
				return;	
			}
			BulletManager.Instance.TryDestroyBreakBullets(this.RoleDataModel.SMsg_Header.uidEntity);
		}
		m_MonsterAttackState.SingleAttack(sMsgFightCommand_SC);
        m_FSMSystem.PerformTransition(Transition.MonsterToAttack);
	}

    private void ShowSplitEffect(SMsgActionDie_SC sMsgActionDie_SC)
    {
        var cutUpEffect = Instantiate(BattleConfigManager.Instance.CutUpEffect) as GameObject;
        cutUpEffect.transform.position = this.ThisTransform.position;
        cutUpEffect.AddComponent<SplitEffect>().SetSplitData(sMsgActionDie_SC);
    }

    //死亡
    public void Die(SMsgActionDie_SC sMsgActionDie_SC)
    {
        this.m_isDie = true;

		//是否施放死亡子弹
		if(m_MonsterConfigData._DeathBullet.Length>0)
		{
			for(int i=0;i<m_MonsterConfigData._DeathBullet.Length;i++)
			{
				int bulletID = m_MonsterConfigData._DeathBullet[i];
				BulletFactory.Instance.CreateBullet(bulletID,RoleDataModel.SMsg_Header.uidEntity,ThisTransform,0,0);
				//BulletFactory.Instance.bullet
				//BulletData bulletData = SkillDataManager.Instance.GetBulletData(bulletID);
			}
		}

		//怪物死亡特效
		bool isDeadEffect = m_MonsterConfigData._deadEffect != "0";
		if (isDeadEffect)
		{
			GameObject deadEffectPrefab = MapResManager.Instance.GetMapEffectPrefab(m_MonsterConfigData._deadEffect);
			if (deadEffectPrefab != null)
			{
				GameObject deadEffect = (GameObject)Instantiate(deadEffectPrefab);
				deadEffect.transform.position = ThisTransform.position;
				deadEffect.AddComponent<DestroySelf>();
			}
			//normal
			this.NormalStatus.SetActive(false);
			m_FSMSystem.FindState(StateID.MonsterDie).m_roleAnimationComponent = this.NormalStatus.animation;
			SplitToDie = false;
		}
		else
		{
			if (this.SplitStatus != null)
			{        
				switch (sMsgActionDie_SC.byDieType)
				{
				case 0:   //Normal
					this.NormalStatus.SetActive(true);
					this.SplitStatus.SetActive(false);
					m_FSMSystem.FindState(StateID.MonsterDie).m_roleAnimationComponent = this.NormalStatus.animation;
					SplitToDie = false;
					break;
				case 1:   //Split
					this.NormalStatus.SetActive(false);
					this.SplitStatus.SetActive(true);
					m_FSMSystem.FindState(StateID.MonsterDie).m_roleAnimationComponent = this.SplitStatus.animation;
					this.ShowHurtFlash(false, this.m_hurtDuration);
					SplitToDie = true;
					ShowBloodEffect();
					break;
				}				
			}
		}
				       

        if (sMsgActionDie_SC.byDieType == 1)
        {
            if (sMsgActionDie_SC.byTrigger == 1)
            {
                ShowSplitEffect(sMsgActionDie_SC);
            }
        }
        

        m_FSMSystem.PerformTransition(Transition.MonsterToDie);

        //if (this.m_MonsterConfigData.MonsterSubType == 2)
        //{
        //    PopupObjManager.Instance.ShowBattleResult(true);
        //}
    }
	
	public void ShowBloodEffect()
	{
        GameObject bloodPrefab = MapResManager.Instance.GetMapMonsterBloodEffectPrefab(m_MonsterConfigData._monsterID);

        GameObject bloodObj = GameObjectPool.Instance.AcquireLocal(bloodPrefab, ThisTransform.position, ThisTransform.rotation);
		Transform bloodObjTrans = bloodObj.transform;
		bloodObjTrans.parent = this.ThisTransform;
		bloodObjTrans.localPosition = Vector3.zero;
		bloodObjTrans.localRotation = Quaternion.identity;
		bloodObjTrans.localScale = new Vector3(1, 1, 1);
	}

    //停止移动
    public void StopHere(SMsgActionStopHere_SC actionStopHere)
    {
        if (actionStopHere.byForceSync == 0)
        {
            PointQueue.Clear();
            this.WalkToPosition = null;            
        }
        else
        {
            //强制类型
            PointQueue.Clear();
            this.WalkToPosition = Vector3.zero.GetFromServer(actionStopHere.ptHereX, actionStopHere.ptHereY);
            //TraceUtil.Log("解出位置" + WalkToPosition);
            //this.WalkSpeed *= 2;

            //\
            SetMonsterStopPoint(WalkToPosition.Value);
        }
    }

    /// <summary>
    /// 根据技能动作类型，隐身或显身
    /// </summary>
    /// <param name="moveType"></param>
    public void ChangeDisplayState(int moveType)
    {
        RefreshRenderCach();
        if (moveType == 0)
        {
            m_monsterRendererDatas.ApplyAllItem(P => { if (P.enabled) P.enabled = false; });
            //血条消失
            if (this.EnemyHealthBar != null)
            {
                this.EnemyHealthBar.gameObject.SetActive(false);
            }
        }
        else
        {
            m_monsterRendererDatas.ApplyAllItem(P =>
            {
                //TraceUtil.Log(P.name + ": "+ P.enabled);
                if (!P.enabled) P.enabled = true;
            });
            //血条重现
            if (this.EnemyHealthBar != null)
            {
                this.EnemyHealthBar.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 刷新玩家自身与装备的Renderer列表
    /// </summary>
    public void RefreshRenderCach()
    {
        List<PlayDataStruct<Renderer>> partRenderer;
        List<PlayDataStruct<SkinnedMeshRenderer>> mainRenderer;
        ThisTransform.RecursiveGetComponent<Renderer>("Renderer", out partRenderer);
        ThisTransform.RecursiveGetComponent<SkinnedMeshRenderer>("SkinnedMeshRenderer", out mainRenderer);

        if (this.m_monsterRendererDatas == null)
        {
            this.m_monsterRendererDatas = new List<Renderer>();
        }
        this.m_monsterRendererDatas.Clear();

        this.m_monsterRendererDatas.AddRange(partRenderer.Select(P => P.AnimComponent));
        this.m_monsterRendererDatas.AddRange(mainRenderer.Select(P => (Renderer)P.AnimComponent));
    }

    //public bool IsInvisible()
    //{
    //    return m_ArrowBehaviour.isShowArrow;
    //}

    public override ObjectType GetFHObjType()
    {
        return ObjectType.Monster;
    }
    /// <summary>
    /// Release Monster Locked Status
    /// </summary>
    public void ReleaseLockedStatus()
    {
        if (m_beLocked)
        {
            m_beLocked = false;
            if (m_arrow != null)
            {
                GameObject.Destroy(m_arrow);
            }
        }
    }
    /// <summary>
    /// Monster Be Locked
    /// </summary>
    public void BeLocked()
    {
        m_beLocked = true;
        if (m_arrow ==null&&m_attachPoint != null)
        {
            m_arrow = GameObject.Instantiate(BattleManager.Instance.MonsterTargetArrow) as GameObject;
            var originPosition = m_arrow.transform.position;
            m_arrow.transform.position = Vector3.zero;
            m_arrow.transform.parent = m_attachPoint;
            m_arrow.transform.localPosition = originPosition;
        }
    }
    protected override void RegisterEventHandler()
    {
        //AddEventHandler(EventTypeEnum.EntityMove.ToString(), ReceiveEntityMoveHandle);
    }

    public IEntityDataStruct GetDataModel()
    {
        return null;
    }

	public void AttackToPoint(Vector3 pos)
	{
		ThisTransform.position = pos;
	}

    public void MoveToPoint(Vector3 pos)
    {
        if(!SceneDataManager.Instance.IsPositionInBlock(pos))
        {
            ThisTransform.position = pos;
        }
        else
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Monster pos in Block!!!!!!!!");
        }
    }

    public void FixMonsterYToOrigin()
    {
        ThisTransform.position = new Vector3(ThisTransform.position.x, 0, ThisTransform.position.z);
    }

    #region Add by lee
    Vector3 monsterStopPoint = new Vector3(-100f, 0, 0);
    public void SetMonsterStopPoint(Vector3 pos)
    {
        monsterStopPoint = pos;
    }

    //Vector3 monsterBeAdsorbPoint = Vector3.zero;
    //Vector3 monsterBeAdsorbDiret = Vector3.zero;
    //public void SetMonsterBeAdsorbDiret(Vector3 point, Vector3 diret)
    //{
    //    monsterBeAdsorbPoint = point;
    //    monsterBeAdsorbDiret = diret;
    //}
    #endregion


#if UNITY_EDITOR



    void OnDrawGizmos ()
	{
		for(int i = 0; i < nodes.Count -1; i++)
		{
			
			Gizmos.color = Color.red;
			Gizmos.DrawLine(nodes[i], nodes[i + 1]);
		}
		for(int i = 0; i < monsterMoveNodes.Count -1; i++)
		{
			
			Gizmos.color = Color.green;
			if( 0 == i )
			{
				Gizmos.DrawSphere(monsterMoveNodes[i], 3.0f);		
			}
			else if(( monsterMoveNodes.Count - 2 ) == i)
			{
				
			}
			
			Gizmos.DrawLine(monsterMoveNodes[i], monsterMoveNodes[i + 1]);
		}
		int count = monsterMoveNodes.Count;
		if(count > 0)
		{
			Gizmos.DrawCube(monsterMoveNodes[monsterMoveNodes.Count - 1 ], new Vector3(3.0f, 3.0f, 3.0f));
        }

        #region Add by lee
        
        Gizmos.color = Color.red;
        Gizmos.DrawCube(monsterStopPoint, new Vector3(3.0f, 3.0f, 3.0f));
               
        //Gizmos.color = Color.black;
        //Gizmos.DrawLine(monsterBeAdsorbPoint, monsterBeAdsorbDiret);
        //Gizmos.DrawSphere(monsterBeAdsorbDiret, 3.0f);
        #endregion
    }	


#endif
}
