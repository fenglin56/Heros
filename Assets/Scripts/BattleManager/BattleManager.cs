using UnityEngine;
using System.Collections;
using NetworkCommon;
using System.Linq;
using System;
using System.Collections.Generic;

/// <summary>
/// BattleUI/TownUI Scene BattleManager 
/// </summary>
public class BattleManager : View {

    private bool m_heroCreated = false;
	private Transform m_mainCameraTarget;
    private bool m_isHeroFirstDead = false;
    
	private int m_CurEctypeSirenSkillTime = 0;
	public int SirenSkillTime{get {return m_CurEctypeSirenSkillTime;}}

    public FollowCamera FollowCamera;
    public Camera UICamera;

	private int m_SirenSkillUseTime = 0;

	private static BattleManager m_instance = null;
	public static BattleManager Instance
	{
		get { return m_instance; }	
	}
    public GameObject MonsterTargetArrow;/// 怪物选中箭头Prefab
    public GameObject GuidePointTargetEffect;/// 从玩家指向下一个触发区域的指引Prefab
    private Transform m_hero;  /// 主玩家
    private Vector3 m_fixedPos; /// 固定位置
    private bool m_dynamicPoint;  // 是否玩家身上动态指向
    private Transform m_PlayerAttachArrow;  /// 玩家身上挂载指引箭头
    private Dictionary<int, GameObject[]> m_blockStateEffct;

   
                                             /// 
    private bool m_blockPlayerToIdle = false;
    public bool BlockPlayerToIdle
    {
        get { return m_blockPlayerToIdle; }
        set 
        { 
            m_blockPlayerToIdle = value; 
            if(m_blockPlayerToIdle)
            {
                var item = PlayerManager.Instance.FindHeroEntityModel();
                PlayerBehaviour pb = (PlayerBehaviour)(item.Behaviour);
                pb.FSMSystem.PerformTransition(Transition.PlayerToIdle);
            }
        }
    }
    private Transform HeroTrans
    {
        get
        {
            if (m_hero == null)
            {
				GameObject heroGO = PlayerManager.Instance.FindHero();
				if(heroGO!=null)
				{
					m_hero = heroGO.transform;
				}                
            }
            return m_hero;
        }
    }

    private int currentHurtEffectCount = 0;
    public bool CanShowHurtEffect()
    {
        return currentHurtEffectCount <= CommonDefineManager.Instance.CommonDefine.HurtEffectNumber;
    }

    public void OnHurtEffectCreate()
    {
        currentHurtEffectCount++;
    }

    public void OnHurtEffectDestroy()
    {
        currentHurtEffectCount--;
        if(currentHurtEffectCount < 0)
        {
            currentHurtEffectCount = 0;
        }
    }



	void Awake()
	{
        if (FollowCamera == null)
        {
            FollowCamera = Camera.main.GetComponent<FollowCamera>();
        }
		m_instance = this;
        m_blockStateEffct = new Dictionary<int, GameObject[]>();
        RegisterEventHandler();      

	}
    void OnGUI()
    {       
            if(GameManager.Instance.ShowPlayerName)
            {
                var item = PlayerManager.Instance.FindHeroEntityModel();
                if (item != null)
                {
                    GUI.TextField(new Rect(350, 100, 120, 40), ((SMsgPropCreateEntity_SC_MainPlayer)(item.EntityDataStruct)).Name);
                }            
            }
    }
    protected override void OnDestroy()
    {
        CancelInvoke("HeartFPS");
        //玩家脚底指引箭头
        if (m_PlayerAttachArrow != null)
        {
            GameObject.Destroy(m_PlayerAttachArrow.gameObject);
			m_PlayerAttachArrow = null;
        }
        m_instance = null; 

		base.OnDestroy ();
    }
	public void SetCameraTarget( Transform target)
	{        
        if (target == null)
        {
            //TraceUtil.Log("相机未能找到target");
            //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"相机未能找到target");
            return;
        }

		//m_mainCameraTarget = target;
        if (null != FollowCamera)
        {
            FollowCamera.SetTarget(target);
            m_mainCameraTarget = target;//记录当前摄像机跟随角色
        }       
	}
    /// <summary>
    /// 阻挡生成/消失特效
    /// </summary>
    /// <param name="areaId"></param>
    public void BlockAppear(SMsgEctypeUpDateBlock sMsgEctypeUpDateBlock)
    {
        var mapDynamicData = SceneDataManager.Instance.GetMapDynamicBlockData(sMsgEctypeUpDateBlock.dwareaId,sMsgEctypeUpDateBlock.dwblockGroupID);
        if (mapDynamicData != null)
        {
            var groupId = sMsgEctypeUpDateBlock.dwblockGroupID;
            if (m_blockStateEffct.ContainsKey(groupId))
            {
                for (int i = 0; i < m_blockStateEffct[groupId].Length; i++)
                {
                    GameObject.Destroy(m_blockStateEffct[groupId][i]);
                }
                m_blockStateEffct.Remove(groupId);
            }
            m_blockStateEffct.Add(groupId, new GameObject[mapDynamicData.EffectPos.Length]);
            if (sMsgEctypeUpDateBlock.byBlockState == 0)
            {
                StartCoroutine(SceneDataManager.Instance.ChangeDynamicBlockGroupState(groupId, true, mapDynamicData.FadeDelay));
                
                for (int i = 0; i < mapDynamicData.EffectPos.Length; i++)
                {
                    m_blockStateEffct[groupId][i] = Instantiate(mapDynamicData.BlockFadeEffect, mapDynamicData.EffectPos[i], Quaternion.Euler(0, mapDynamicData.EffectAngle[i], 0)) as GameObject;
                }
                //指引箭头
                if (mapDynamicData.GuideArrow == 1)
                {
                    ShowNextTriggerAreaGuideArrow(mapDynamicData.AreaID + 1);
                }
            }
            else
            {
                StartCoroutine(SceneDataManager.Instance.ChangeDynamicBlockGroupState(groupId, false, 0));
                for (int i = 0; i < mapDynamicData.EffectPos.Length; i++)
                {
                    m_blockStateEffct[groupId][i] = Instantiate(mapDynamicData.BlockEffect, mapDynamicData.EffectPos[i], Quaternion.Euler(0, mapDynamicData.EffectAngle[i], 0)) as GameObject;
                }
            }
        }
        else
        {
            TraceUtil.Log(SystemModel.Rocky, "找不到组的阻挡数据:" + sMsgEctypeUpDateBlock.dwblockGroupID);
        }
    }
    void Update()
    {
        if (m_dynamicPoint)
        {
            if ( HeroTrans != null && m_PlayerAttachArrow != null)
            {
                var forward = new Vector3(m_fixedPos.x, m_PlayerAttachArrow.position.y, m_fixedPos.z)
                    - new Vector3(HeroTrans.position.x, m_PlayerAttachArrow.position.y, HeroTrans.position.z);

				m_PlayerAttachArrow.position = HeroTrans.position;
                m_PlayerAttachArrow.rotation = Quaternion.LookRotation(forward);
            }
            else
            {
                m_dynamicPoint = false;
            }
        }
    }
    public void ReachTriggerArea()
    {
        //玩家脚底指引箭头
        if (m_PlayerAttachArrow != null)
        {
            GameObject.Destroy(m_PlayerAttachArrow.gameObject);
        }
        m_dynamicPoint = false;
    }
    private void ShowNextTriggerAreaGuideArrow(int nextAreaId)
    {
        var triggerAreaInfo = SceneDataManager.Instance.GetTriggerAreaInfo(nextAreaId);
        if (triggerAreaInfo != null)
        {           
            if (m_PlayerAttachArrow == null)
            {
                var guidePointTargetEffet = Instantiate(GuidePointTargetEffect) as GameObject;
                guidePointTargetEffet.name = GuidePointTargetEffect.name;
                m_PlayerAttachArrow = guidePointTargetEffet.transform;

                m_PlayerAttachArrow.position = HeroTrans.position;
            }

            m_fixedPos = triggerAreaInfo.Center;
            m_fixedPos.y = GuidePointTargetEffect.transform.position.y;
            m_dynamicPoint = true;
        }
    }
	private void ResetSirenSkill()
	{
		var peekData = GameDataManager.Instance.PeekData(DataType.InitializeEctype);
		if (peekData == null)
		{
			TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"DataType.InitializeEctype is null");
			return;
		}
		SMSGEctypeInitialize_SC ectypeSmg = (SMSGEctypeInitialize_SC)peekData;
		EctypeContainerData ectypeData = null;
		if (EctypeConfigManager.Instance.EctypeContainerConfigList.ContainsKey((int)ectypeSmg.dwEctypeContainerId))
		{
			ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[(int)ectypeSmg.dwEctypeContainerId];
		}
		m_CurEctypeSirenSkillTime = ectypeData.SirenSkillVaule;
	}


    /// <summary>
    /// 重设摄像机的视野
    /// </summary>
    /// <param name="distance"></param>
    public void SetCameraDistance(Vector3 distance)
    {
        var hero = PlayerManager.Instance.FindHero();
        FollowCamera.SetInitDistanceFromPlayer(hero.transform, distance, false);
    }

	public void TryShakeCamera(BulletData bulletData)
	{
		
		if(null != bulletData)
		{
            if(bulletData.m_shakeAniName != "0")
            {
                FollowCamera.ShakeCamera(bulletData.m_shakeAniName);
            }

            /*
			if( bulletData.m_hurtShakeTime > 0)
			{
				if(null != FollowCamera)
				{
					FollowCamera.ShakeCamera(bulletData.m_hurtShakeTime, bulletData.m_hurtShakeAttenuation,
						bulletData.m_hurtShakeInitSpeed, bulletData.m_hurtShakeElasticity);
				}
			}
            */
        }
	}
	
	public void BulletBornShakeCamera(BulletData bulletData, Int64 ownerUID)
	{
		
		if(null != FollowCamera)
		{
            if("0" != bulletData.m_bornShakeAniName)
            {
                TypeID ownerType;
                var ownerEntity = EntityController.Instance.GetEntityModel(ownerUID, out ownerType);
                if(ownerType != TypeID.TYPEID_PLAYER || ownerEntity.EntityDataStruct.SMsg_Header.IsHero)
                {
                    FollowCamera.ShakeCamera(bulletData.m_bornShakeAniName);
                }
            }
		}
		
	}

	public void ResetSirenSkillUseTime(int useTime)
	{
		this.m_SirenSkillUseTime = useTime;
	}
	public void UseSirenSkill()
	{
		this.m_SirenSkillUseTime--;
	}
	public bool IsCanUseSirenSkill()
	{
		return this.m_SirenSkillUseTime > 0;
	}

    /// <summary>
    /// 玩家此副本已经死亡过
    /// </summary>
    public bool IsHeroFirstDead
    {
        get { return m_isHeroFirstDead; }
    }

    void ReceiveEntityDieHandle(INotifyArgs args)
    {
        var sMsgActionDie_SC = (SMsgActionDie_SC)args;
        var entityModel = PlayerManager.Instance.GetEntityMode(sMsgActionDie_SC.uidEntity);
        if (entityModel != null)
        {
            //TraceUtil.Log("收到玩家死亡消息 at BattleManager"); //如果是当前摄像机跟随角色，则需要改变摄像机跟随,随机跟随一个
            if(m_mainCameraTarget==entityModel.GO.transform)
            {
                //SetCameraTarget(PlayerManager.Instance.RandomCameraFollowTarget());
                GameManager.Instance.TryFindCameraTarget();
                m_isHeroFirstDead = true;
            }
        }
    }
    void ReceiveEntityReliveHandle(INotifyArgs args)
    {
        SMsgActionRelivePlayer_SC sMsgActionRelivePlayer_SC = (SMsgActionRelivePlayer_SC)args;
        var entityModel = PlayerManager.Instance.FindPlayerByActorId(sMsgActionRelivePlayer_SC.actorTarget);        
        if (entityModel != null&& entityModel.EntityDataStruct.SMsg_Header.IsHero)
        {
            //TraceUtil.Log("收到玩家复活");//重置摄像机跟随
            //SetCameraTarget(entityModel.Behaviour.transform);
            GameManager.Instance.TryFindCameraTarget();
        }
    }
	protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);
        AddEventHandler(EventTypeEnum.EntityRelive.ToString(), ReceiveEntityReliveHandle);
        AddEventHandler(EventTypeEnum.PlayerGotoSceneReady.ToString(), PlayerGotoSceneReadyHandle);        
    }
	
	// Use this for initialization
	void Start () {
        //场景加载完成事件
        //RaiseEvent(EventTypeEnum.SceneLoaded.ToString(), null);
        ResponseHandleInvoker.Instance.IsPaused = false;
        //RaiseEvent(EventTypeEnum.EntityCreate_Player.ToString(),null);

        SetBattleSceneLoaded();

		if(GameManager.Instance.CurrentState != GameManager.GameState.GAME_STATE_PLAYERROOM && GameManager.Instance.CurrentState != GameManager.GameState.GAME_STATE_STORYLINE)
		{
			CreateHeroTitle();                    
		}
		if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_STORYLINE)
		{
			StroyLineDataManager.Instance.TutorialScene();
			StroyLineManager.Instance.StartNextCameraGroup();
		}
	}
			
    private void PlayerGotoSceneReadyHandle(INotifyArgs notifyArgs)
    {        
				SetBattleSceneLoaded ();
				//检查阻挡，刷特效
				if (SceneDataManager.Instance.CacheInitBlocks != null && SceneDataManager.Instance.CacheInitBlocks.Count > 0) {
						SceneDataManager.Instance.CacheInitBlocks.ApplyAllItem (P => BlockAppear (P));
				}
		}
    private void SetBattleSceneLoaded()
    {
        if (!m_heroCreated)
        {
            if (PlayerManager.Instance.HeroCreated)
            {
                m_heroCreated = true;
                if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE)
                {
                    UIEventManager.Instance.TriggerUIEvent(UIEventType.LoadBattleSceneCompleteCS, null);
                }

                //var heroEntityModel = PlayerManager.Instance.FindHeroEntityModel();
                //if (null != heroEntityModel && heroEntityModel.GO != null)
                //{
                //    if (((PlayerBehaviour)heroEntityModel.Behaviour).FSMSystem.CurrentStateID != StateID.PlayerDie)
                //    {
                //        SetCameraTarget(PlayerManager.Instance.FindHero().transform);
                //    }
                //    else
                //    {

                //        SetCameraTarget(PlayerManager.Instance.RandomCameraFollowTarget());
                //    }
                //}
                GameManager.Instance.TryFindCameraTarget();
            }
        }
    }

    private void CreateHeroTitle()
    {
        StartCoroutine(FindHero());        
    }
    IEnumerator FindHero()
    {
        yield return new WaitForEndOfFrame();
        var playerData = PlayerManager.Instance.FindHeroEntityModel();
        if (playerData == null || playerData.GO == null)
        {
            StartCoroutine(FindHero());
        }
        else
        {
			var titleGO = PlayerFactory.Instance.CreateTitle(playerData.EntityDataStruct.SMsg_Header.uidEntity,((SMsgPropCreateEntity_SC_MainPlayer)playerData.EntityDataStruct).Name, playerData.EntityDataStruct.SMsg_Header.IsHero, playerData.GO.transform);
            PlayerManager.Instance.SetHeroTitle(titleGO);
        }
    }

    

}
