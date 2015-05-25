using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Battle;
using UI.MainUI;
using System.Collections;

public class PlayerManager : Controller, IEntityManager,ISingletonLifeCycle
{
    private List<EntityModel> m_playerList = new List<EntityModel>();
    private List<SSkillShowRes> m_sSkillShowRes = new List<SSkillShowRes>();
    public List<EntityModel> PlayerList { get { return m_playerList; } }
    private SMsgSkillInit_SC m_heroSMsgSkillInit_SC;
    private const string SKILL_SELECTED_EFFECT_NAME = "SkillSelectedEffect";
    private const string NEWBIEGUIDE_ARROW06_NAME = "JH_Eff_Scenes_NewbieGuide_Arrow06";
    private System.Object m_lockObj = new object();
    private GameObject m_HeroTitle;
    private int CurrentForce=-1;
    public RoleUpanishads m_heroUpanishads = null;
    private Dictionary<GameObject, int> m_cachePlayerLayers=new Dictionary<GameObject,int>();

    private static PlayerManager m_instance;
    //是否隐藏其他玩家标记位
    public bool m_playerHideFlag{get;private set;}
    public static PlayerManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new PlayerManager();
                SingletonManager.Instance.Add(m_instance);
                EntityController.Instance.RegisteManager(TypeID.TYPEID_PLAYER, m_instance);
            }
            return m_instance;
        }
    }

    protected override void RegisterEventHandler()
    {
        TraceUtil.Log("PlayerManager RegisterEvent");
        AddEventHandler(EventTypeEnum.TargetSelected.ToString(), ReceiveTargetSelectedHandle);

        AddEventHandler(EventTypeEnum.FightCommand.ToString(), ReceiveFightCommandHandle);
        AddEventHandler(EventTypeEnum.BeatBack.ToString(), ReceiveBeatBackHandle);
        AddEventHandler(EventTypeEnum.BeAdsorb.ToString(), ReceiveBeAdsorbHandle);
        AddEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);
        AddEventHandler(EventTypeEnum.EntityRelive.ToString(), ReceiveEntityReliveHandle);
        AddEventHandler(EventTypeEnum.BreakSkill.ToString(), ReceiveBreakSkillHandle);
        AddEventHandler(EventTypeEnum.EntityMove.ToString(), ReceiveEntityMoveHandle);
        AddEventHandler(EventTypeEnum.OnTouchInvoke.ToString(), OnTouchDown);
        AddEventHandler(EventTypeEnum.SceneChange.ToString(), SceneChangeHandle);
        AddEventHandler(EventTypeEnum.FightChangeDirect.ToString(), FightChangeDirectHandle);
        AddEventHandler(EventTypeEnum.FightFly.ToString(), ReceiveFightFlyHandle);
        AddEventHandler(EventTypeEnum.EntityHorde.ToString(), ReceiveEntityHordeHandle);
        //AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), EntityUpdateValuesHandle);
		AddEventHandler(EventTypeEnum.BeAdSorbEx.ToString(), ReceiveBeAdsorbExHandle);
        AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), EntityUpdateValuesHandle);
        AddEventHandler(EventTypeEnum.Teleport.ToString(), ReceiveEntityTeleportHandle);
    }

   
    /// <summary>
    /// 在EntityService收到切换场景或复活时，重设主玩家的出生位置
    /// </summary>
    /// <param name="serverX"></param>
    /// <param name="serverY"></param>
    public void SetHeroBirthPosAndRotation(float serverX, float serverY, UInt32 dir)
    {
        var heroModel = this.FindHeroEntityModel();
        if (heroModel != null && heroModel.GO != null)
        {
            if (heroModel.GO.transform != null)
            {
                heroModel.GO.transform.position = new Vector3(0, 0.1f, 0);
                heroModel.GO.transform.position = heroModel.GO.transform.position.GetFromServer(serverX, serverY);  
                Quaternion playerDir = Quaternion.Euler(0, (float)dir/1000.0f, 0);
                heroModel.GO.transform.rotation = playerDir;
            }
        }
    }
	/// <summary>
	/// 判断怪物是否已方阵营
	/// </summary>
	/// <returns><c>true</c>, if monster hostility was checked, <c>false</c> otherwise.</returns>
	/// <param name="monsterEntityModel">Monster entity model.</param>
	public bool CheckMonsterHostility(EntityModel monsterEntityModel)
	{
		return CheckMonsterHostility(this.FindHeroEntityModel(),monsterEntityModel);
	}
	public bool CheckMonsterHostility(EntityModel playerEntityModel,EntityModel monsterEntityModel)
	{
		var playerHostility=((IPlayerDataStruct)playerEntityModel.EntityDataStruct).GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHT_HOSTILITY;
		var monsterHostility=((SMsgPropCreateEntity_SC_Monster)monsterEntityModel.EntityDataStruct).MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY;
		return playerHostility==monsterHostility;
	}

    public EntityModel FindPlayerByActorId(int actorId)
    {
        return this.m_playerList.FirstOrDefault(P => ((IPlayerDataStruct)P.EntityDataStruct).PlayerActorID == actorId);
    }
    /// <summary>
    /// 隐藏除主角外的其他玩家 hideFlag为True表示隐藏，false表示显示
    /// </summary>
	public void HideAllPlayerButHero(bool hideFlag)
	{
		m_playerHideFlag = hideFlag;
		this.m_playerList.ApplyAllItem(P =>
		                               {
			HideAllPlayerButHero(hideFlag, P);
		});
		TownRobotManager.Instance.robotInfoList.ApplyAllItem (P=>{
			HideAllRobotPlayer(hideFlag,P.robot,P.titleRef);
		});
		if (!hideFlag)
		{
			m_cachePlayerLayers.Clear();
		}
	}
	private void HideAllRobotPlayer(bool hideFlag,GameObject robot,GameObject titleRef)
	{
		if (hideFlag)
		{
			int layer = LayerMask.NameToLayer("Hide");
			TownRobotManager.Instance.GetRobotRender(robot);
			TownRobotManager.Instance.PlayerRendererDatas.ApplyAllItem(render =>{ 
				if(!m_cachePlayerLayers.ContainsKey(render.gameObject)) 
				{
					m_cachePlayerLayers.Add(render.gameObject, render.gameObject.layer); 
					render.gameObject.layer = layer; 
				}
			});
			//隐藏Title
			if (titleRef != null)
			{
				var childTrans = titleRef.GetChildTransforms();
				childTrans.ApplyAllItem(trans =>
				                        { 
					if(!m_cachePlayerLayers.ContainsKey(trans.gameObject)) 
					{
						m_cachePlayerLayers.Add(trans.gameObject, trans.gameObject.layer); 
						trans.gameObject.layer = layer;
					}
				});
				if(!m_cachePlayerLayers.ContainsKey(titleRef)) 
				{
					m_cachePlayerLayers.Add(titleRef, titleRef.layer);
					titleRef.layer = layer;
				}
			}
		}
		else
		{
			TownRobotManager.Instance.GetRobotRender(robot);
			TownRobotManager.Instance.PlayerRendererDatas.ApplyAllItem(render =>{
				if (m_cachePlayerLayers.ContainsKey(render.gameObject))
				{
					render.gameObject.layer = m_cachePlayerLayers[render.gameObject];
				}
				
			});
			//显示Title
			if (titleRef != null)
			{
				var childTrans = titleRef.GetChildTransforms();
				childTrans.ApplyAllItem(trans =>
				                        {
					if (m_cachePlayerLayers.ContainsKey(trans.gameObject))
					{
						trans.gameObject.layer = m_cachePlayerLayers[trans.gameObject];
					}
				});
				if (m_cachePlayerLayers.ContainsKey(titleRef))
				{
					titleRef.layer = m_cachePlayerLayers[titleRef]; ;
				}
			}
		}
		
	}
    private void HideAllPlayerButHero(bool hideFlag,EntityModel playerModel)
    {
        if (hideFlag)
        {
            int layer = LayerMask.NameToLayer("Hide");
            if (playerModel.Behaviour != null)
            {
                var playerBehaviour = (PlayerBehaviour)playerModel.Behaviour;
                if (!playerBehaviour.IsHero)
                {
                    playerBehaviour.RefreshRenderCach();
                    playerBehaviour.PlayerRendererDatas.ApplyAllItem(render =>
                    { 
                        if(!m_cachePlayerLayers.ContainsKey(render.gameObject)) 
                        {
                            m_cachePlayerLayers.Add(render.gameObject, render.gameObject.layer); 
                            render.gameObject.layer = layer; 
                        }
                    });
                    //隐藏Title
                    if (playerBehaviour.PlayerTitleRef != null)
                    {
                        var childTrans = playerBehaviour.PlayerTitleRef.GetChildTransforms();
                        childTrans.ApplyAllItem(trans =>
                        { 
                            if(!m_cachePlayerLayers.ContainsKey(trans.gameObject)) 
                            {
                                m_cachePlayerLayers.Add(trans.gameObject, trans.gameObject.layer); 
                                trans.gameObject.layer = layer;
                            }
                        });
                        if(!m_cachePlayerLayers.ContainsKey(playerBehaviour.PlayerTitleRef)) 
                        {
                            m_cachePlayerLayers.Add(playerBehaviour.PlayerTitleRef, playerBehaviour.PlayerTitleRef.layer);
                            playerBehaviour.PlayerTitleRef.layer = layer;
                        }
                    }
                }
            }
        }
        else
        {
            if (playerModel.Behaviour != null)
            {
                var playerBehaviour = (PlayerBehaviour)playerModel.Behaviour;
                playerBehaviour.RefreshRenderCach();
                playerBehaviour.PlayerRendererDatas.ApplyAllItem(render =>
                {
                    if (m_cachePlayerLayers.ContainsKey(render.gameObject))
                    {
                        render.gameObject.layer = m_cachePlayerLayers[render.gameObject];
                    }

                });
                //显示Title
                if (playerBehaviour.PlayerTitleRef != null)
                {
                    var childTrans = playerBehaviour.PlayerTitleRef.GetChildTransforms();
                    childTrans.ApplyAllItem(trans =>
                    {
                        if (m_cachePlayerLayers.ContainsKey(trans.gameObject))
                        {
                            trans.gameObject.layer = m_cachePlayerLayers[trans.gameObject];
                        }
                    });
                    if (m_cachePlayerLayers.ContainsKey(playerBehaviour.PlayerTitleRef))
                    {
                        playerBehaviour.PlayerTitleRef.layer = m_cachePlayerLayers[playerBehaviour.PlayerTitleRef]; ;
                    }
                }
            }
        }

    }
    #region 玩家头顶名字
    public void SetHeroTitle(GameObject title)
    {
        m_HeroTitle = title;
    }
    public void HideHeroTitle(bool isFlag)
    {
        if (null != m_HeroTitle)
        {
            m_HeroTitle.SetActive(!isFlag);
        }
        
    }
    #endregion

    #region 查找Player相关方法
    public bool HeroCreated
    {
        get { return FindHero()!=null; }
    }
     /// <summary>
    /// 查找主角
    /// </summary>
    /// <returns></returns>
    public GameObject FindHero()
    {
        EntityModel item = FindHeroEntityModel();
        if (item != null)
        {
            return item.GO;
        }

        return null;
    }

    public NavMeshAgent HeroAgent
    {
        get { return FindHero().GetComponent<NavMeshAgent>(); }
    }

    public EntityModel FindHeroEntityModel()
    {
		if(m_playerList == null)
		{
			return null;
		}
        EntityModel item;
        for (int i = 0; i < this.m_playerList.Count; i++)
        {
            item = this.m_playerList[i];

            var playerDataModel = item.EntityDataStruct;
            if (playerDataModel != null && playerDataModel.SMsg_Header.nIsHero == 1)
            {
                return item;
            }
        }

        return null;
    }

    public SMsgPropCreateEntity_SC_MainPlayer FindHeroDataModel()
    {
        EntityModel item = FindHeroEntityModel();
        if (item != null)
        {
            return (SMsgPropCreateEntity_SC_MainPlayer)(item.EntityDataStruct);
        }
        else
        {
            return default(SMsgPropCreateEntity_SC_MainPlayer);
        }
    }

    public void LockPlayOnSceneChange(bool enable)
    {
        var heroModel = this.FindHeroEntityModel();
        if (heroModel != null && heroModel.Behaviour!=null)
        {
            heroModel.Behaviour.enabled = enable;
        }
    }
    /// <summary>
    /// 查找玩家
    /// </summary>
    /// <returns></returns>
    public GameObject FindPlayer(Int64 entityUid)
    {
        foreach (EntityModel child in m_playerList)
        {
            if (child.EntityDataStruct.SMsg_Header.uidEntity == entityUid)
            {
                return child.GO;
            }
        }
        return null;
    }
    /// <summary>
    /// 随机获得相机跟随对象
    /// </summary>
    /// <returns></returns>
    public Transform RandomCameraFollowTarget()
    {
        var alivePlayer = this.m_playerList.Where(P => !((PlayerBehaviour)P.Behaviour).IsDie).ToArray();
        int count = alivePlayer.Length;
        //TraceUtil.Log("随机玩家总数："+count);//重置摄像机跟随
        if (count > 0)
        {
            int index = UnityEngine.Random.Range(0, count);

            return alivePlayer[index].GO.transform;
        }
        else
        {
            return null;
        }
    }

    #region add by lee
    public Transform FindCameraFollowTarget()
    {
        var heroEntityModel = this.FindHeroEntityModel();
        if (m_playerList.Count == 1)
        {
             return heroEntityModel.GO.transform;
        }

       
           
        if (heroEntityModel == null || heroEntityModel.Behaviour == null)
        {
            return null;   
        }
        

        if (!((PlayerBehaviour)heroEntityModel.Behaviour).IsDie)
        {
            return heroEntityModel.GO.transform;
        }
        else
        {
            var alivePlayer = this.m_playerList.Where(P =>P.Behaviour!=null && !((PlayerBehaviour)P.Behaviour).IsDie).ToArray();
            int count = alivePlayer.Length;
            if (count > 0)
            {
                int index = UnityEngine.Random.Range(0, count);

                return alivePlayer[index].GO.transform;
            }
            else
            {
                return null;
            }            
        }
    }
    #endregion

    public List<EntityModel> FindTargetByFightResult(FightResult fightResult, out bool isFight)
    {
        List<EntityModel> entityModels = new List<EntityModel>();
        var target = m_playerList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == fightResult.SMsgFightFightTo.uidFighter);
        if (target != null)
        {
            isFight = true;
            entityModels.Add(target);
        }
        else
        {
            isFight = false;
            foreach (var item in fightResult.TargetDatas)
            {
                target = m_playerList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == item.uidTarget);
                if (target != null)
                {
                    entityModels.Add(target);
                }
            }
        }
        return entityModels;
    }
    
    #endregion

    #region 场景切换，玩家动画，技能，武器 更换
    public void PlayerCreateProcess(GameObject player,PlayerBehaviour playerView, GameManager.GameState gameState)
    {
        if (player != null)
        {
            SwitchHeroAnimAndWeapon(player, gameState);
            var playerBehaviour = playerView;
            
            SwitchHeroSkillSelectedEffect(player, gameState);
            SwitchPlayerAttachSkillBase(gameState);

            if (playerBehaviour.FSMSystem == null)
            {
                playerBehaviour.InitFSM();
            }
            SwitchPlayerState(player, gameState);

            playerBehaviour.LeaveInitiativeSkillSelectedState();
            playerBehaviour.ClearWalkPositionInfo();
        }
    }
    /// <summary>
    /// 主玩家切换场景,需要切换配置（动画，武器位置）
    /// </summary>
    public void PlayerChangeScene(GameManager.GameState gameState)
    {
        if (GameManager.Instance.CreateEntityIM)
        {
            LockPlayOnSceneChange(true);

            this.PlayerList.ApplyAllItem(P =>
                {
                    var player = P.GO;
                    if (player != null)
                    {
                        SwitchHeroAnimAndWeapon(player, gameState);
                        //如果在隐藏状态，隐藏玩家
                        //if (m_playerHideFlag)
                        //{
                        //    HideAllPlayerButHero(m_playerHideFlag, P);
                        //}
                        var playerBehaviour = (PlayerBehaviour)P.Behaviour;
                       
                        SwitchHeroSkillSelectedEffect(player, gameState);
                        SwitchPlayerAttachSkillBase(P, gameState);
                        if (playerBehaviour.FSMSystem == null)
                        {
                            playerBehaviour.InitFSM();
                        }
                        SwitchPlayerState(player, gameState);

                        playerBehaviour.LeaveInitiativeSkillSelectedState();
                        playerBehaviour.ClearWalkPositionInfo();
                    }
                });


            //玩家完成场景切换时触发
            RaiseEvent(EventTypeEnum.PlayerGotoSceneReady.ToString(), new PlayerGotoSceneReadyStruct(gameState));
			this.ResetPlayerState();
        }
        else
        {
			GameManager.Instance.PlayerFactory.RegisterPlayerAfterSceneLoadedFun("PlayerChangeScene",() =>
            {
                PlayerChangeScene(gameState);
            });
        }
    }
	public void PlayerChangeScene(GameManager.GameState gameState,long uid)
	{
		var P = this.PlayerList.SingleOrDefault(p=>p.EntityDataStruct.SMsg_Header.uidEntity==uid);
		if(P!=null)
		{
			var player = P.GO;
			if (player != null)
			{
				SwitchHeroAnimAndWeapon(player, gameState);

                //如果在隐藏状态，隐藏玩家
                if (m_playerHideFlag)
                {
                    HideAllPlayerButHero(m_playerHideFlag, P);
                }

				var playerBehaviour = (PlayerBehaviour)P.Behaviour;
				
				SwitchHeroSkillSelectedEffect(player, gameState);
				SwitchPlayerAttachSkillBase(P, gameState);
                if (playerBehaviour.FSMSystem == null)
                {
                    playerBehaviour.InitFSM();
                }
                SwitchPlayerState(player, gameState);
				playerBehaviour.LeaveInitiativeSkillSelectedState();
				playerBehaviour.ClearWalkPositionInfo();
			}

			int UINT_FIELD_STATE;
			var playerDataStruct = (IPlayerDataStruct)P.EntityDataStruct;
			UINT_FIELD_STATE = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UINT_FIELD_STATE;
			
			switch (UINT_FIELD_STATE)
			{
			case (int)CRT_STATE.enCrt_State_Die:
					((PlayerBehaviour)P.Behaviour).Die();
				break;                        
			default:
				//((PlayerBehaviour)P.Behaviour).Relive();
				break;
			}
		}
	}
    private void SwitchPlayerState(GameObject hero, GameManager.GameState gameState)
    {
        var playerBehaviour = hero.GetComponent<PlayerBehaviour>();
        switch (gameState)
        {
            case GameManager.GameState.GAME_STATE_TOWN:
                playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerToIdle);
                break;
            case GameManager.GameState.GAME_STATE_BATTLE:
                playerBehaviour.FSMSystem.PerformTransition(playerBehaviour.IsDie?Transition.PlayerToDie:Transition.PlayerToIdle);
                break;
            default:
                break;

        }

    }
    /// <summary>
    /// 切换主角技能选择状态
    /// </summary>
    /// <param name="hero"></param>
    /// <param name="gameState"></param>
    private void SwitchHeroSkillSelectedEffect(GameObject hero, GameManager.GameState gameState)
    {
        var heroiBehaviour=hero.GetComponent<PlayerBehaviour>();
        switch (gameState)
        {
            case GameManager.GameState.GAME_STATE_TOWN:
                Transform skillSelectedEffect;
                if (hero.transform.RecursiveFindObject(SKILL_SELECTED_EFFECT_NAME, out skillSelectedEffect))
                {
                    skillSelectedEffect.parent = null;

                    GameObject.Destroy(skillSelectedEffect.gameObject);
                    heroiBehaviour.SkillSelectEffectController = null;
                }
                if (heroiBehaviour.IsHero)
                {
                    if (hero.transform.RecursiveFindObject(NEWBIEGUIDE_ARROW06_NAME, out skillSelectedEffect))
                    {
                        skillSelectedEffect.parent = null;

                        GameObject.Destroy(skillSelectedEffect.gameObject);
                    }
                }
                break;
            case GameManager.GameState.GAME_STATE_BATTLE:
                if (heroiBehaviour.SkillSelectEffectController == null)
                {
                    PlayerGenerateConfigData playerData = PlayerDataManager.Instance.GetBattleItemData(hero.GetComponent<PlayerBehaviour>().PlayerKind);
                    if (playerData.SkillEffect != null)
                    {
                        var skillEffect = GameObject.Instantiate(playerData.SkillEffect) as GameObject;
                        if (skillEffect != null)
                        {
                            skillEffect.name = SKILL_SELECTED_EFFECT_NAME;
                            skillEffect.transform.parent = hero.transform;
                            skillEffect.transform.localPosition = new Vector3(0, 0.5f, 0);
                            skillEffect.transform.localRotation = Quaternion.identity;
                            heroiBehaviour.SkillSelectEffectController = skillEffect.GetComponent<SkillSelectEffectController>();
                        }
                    }
                }
                break;
        }

    }
    /// <summary>
    /// 切换主角动画和武器
    /// </summary>
    /// <param name="player"></param>
    /// <param name="gameState"></param>
    private void SwitchHeroAnimAndWeapon(GameObject player, GameManager.GameState gameState)
    {
        var playerBehaviour=player.GetComponent<PlayerBehaviour>();
        var kind = playerBehaviour.PlayerKind;
        PlayerGenerateConfigData playerData = PlayerDataManager.Instance.GetTownItemData(kind);
        if (playerBehaviour.IsHero)
        {
            if (gameState == GameManager.GameState.GAME_STATE_BATTLE)
            {
                playerBehaviour.InvokeEnterTriggerAreaCheck(true);
            }
            else
            {
                playerBehaviour.InvokeEnterTriggerAreaCheck(false);
            }
        }
        switch (gameState)
        {
            case GameManager.GameState.GAME_STATE_TOWN:
                break;
            case GameManager.GameState.GAME_STATE_BATTLE:
                playerData = PlayerDataManager.Instance.GetBattleItemData(kind);
                break;
            case GameManager.GameState.GAME_STATE_PLAYERROOM:
                playerData = PlayerDataManager.Instance.GetPlayerRoomItemData(kind);
                break;
        }

        RoleGenerate.AttachAnimation(player.gameObject, player.name, playerData.DefaultAnim, playerData.Animations);
        GameObject Eff=null;
        string weaponName;
        if (playerBehaviour.IsHero)
        {
            weaponName = ContainerInfomanager.Instance.GetCurrentWeapon();  //主角武器从背包拿
            var eq=ContainerInfomanager.Instance.GetCurrentWeaponItemInfo(); 
                if(eq!=null)
                 {
                  Eff=  (ItemDataManager.Instance.GetItemData(eq.LocalItemData._goodID)as EquipmentData).WeaponEff;
                 }
        }
        else
        {
            int weaponId=(int)((SMsgPropCreateEntity_SC_OtherPlayer)playerBehaviour.RoleDataModel).PlayerValues.WeaponID;
            if (weaponId == 0)
            {
                weaponName = playerData.DefaultWeapon;
            }
            else
            {
                weaponName = ItemDataManager.Instance.GetItemData(weaponId)._ModelId;
                Eff=(ItemDataManager.Instance.GetItemData(weaponId)as EquipmentData).WeaponEff;
            }
        }
        var weaponObj = PlayerFactory.Instance.GetWeaponPrefab(weaponName);
        RoleGenerate.AttachWeapon(player.gameObject, playerData.WeaponPosition, weaponObj,Eff);
    }
    public void ChangePlayerWeapon(Int64 playerId,int weaponGoodId)
    {
        var player = this.m_playerList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == playerId);
        if (player != null)
        {
            string weapon = string.Empty;
            if (weaponGoodId == 0)
            {
                var kind = ((PlayerBehaviour)player.Behaviour).PlayerKind;
                PlayerGenerateConfigData playerData = PlayerDataManager.Instance.GetTownItemData(kind);
                weapon = playerData.DefaultWeapon;
            }
            else
            {
                 weapon = ItemDataManager.Instance.GetItemData(weaponGoodId)._ModelId;
            }
            var weaponObj = PlayerFactory.Instance.GetWeaponPrefab(weapon);
            var itemdata=(ItemDataManager.Instance.GetItemData(weaponGoodId)as EquipmentData);
            if(itemdata!=null)
            {
            var WeaponEffPrefab=itemdata.WeaponEff;
            RoleGenerate.ChangeWeapon(player.GO, weaponObj,WeaponEffPrefab);
            }
        }
    }
    /// <summary>
    /// 更换时装
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="fashionId"></param>
    public void ChangePlayerAvatar(Int64 playerId, int fashionId)
    {
        var player = this.m_playerList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == playerId);
		bool isHero = player.EntityDataStruct.SMsg_Header.IsHero;	//是否是客户端主角
		MESHDENSITY meshDensity = MESHDENSITY.HIGHT;
		if(isHero == false)
			meshDensity = MESHDENSITY.LOW;

        if (player != null)
        {
            string fashion = string.Empty;
            var fashionItem = ItemDataManager.Instance.GetItemData(fashionId);
            if (fashionItem != null)
            {
				RoleGenerate.GenerateRole(player.GO, fashionItem._ModelId, meshDensity);
            }
            else
            {
                var kind=((IPlayerDataStruct)player.EntityDataStruct).GetCommonValue().PLAYER_FIELD_VISIBLE_VOCATION;
                var defaultAvatarName=PlayerDataManager.Instance.GetTownItemData((byte)kind).DefaultAvatar;
				RoleGenerate.GenerateRole(player.GO, defaultAvatarName, meshDensity);
            }
        }
    }
    private void SwitchPlayerAttachSkillBase(EntityModel player, GameManager.GameState gameState)
    {
        if (player == null || player.Behaviour == null)
        {
            return;
        }
        var playerBehaviour = player.Behaviour as PlayerBehaviour;
        switch (gameState)
        {
            case GameManager.GameState.GAME_STATE_TOWN:
               
                playerBehaviour.RemoveSkillBase();
                break;
            case GameManager.GameState.GAME_STATE_BATTLE:
                //附加普通攻击技能
                PlayerGenerateConfigData playerData = PlayerDataManager.Instance.GetBattleItemData(playerBehaviour.PlayerKind);

                foreach (int normalSkillId in playerData.NormalSkillID)
                {
                    playerBehaviour.AddSkillBase(normalSkillId);
                }
                foreach (var normalBackupSkill in playerData.NormalBackAttackSkillID)
                {
                    playerBehaviour.AddSkillBase(normalBackupSkill);
                }
                playerBehaviour.AddSkillBase(playerData.ScrollSkillID);
                if (playerData.BUFF_SKILLID != 0)
                {
                    playerBehaviour.AddSkillBase(playerData.BUFF_SKILLID);
                }
                if (playerData.StandUpSkillID != 0)
                {
                    playerBehaviour.AddSkillBase(playerData.StandUpSkillID);
                }
                if (playerData.OpeningSkillID_1 != 0)
                {
                    playerBehaviour.AddSkillBase(playerData.OpeningSkillID_1);
                }
                if (playerData.OpeningSkillID_2 != 0)
                {
                    playerBehaviour.AddSkillBase(playerData.OpeningSkillID_2);
                }

                var vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
                var peekData = GameDataManager.Instance.PeekData(DataType.InitializeEctype);
                if (peekData == null)
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"DataType.InitializeEctype is null");
                    return;
                }
                SMSGEctypeInitialize_SC ectypeSmg = (SMSGEctypeInitialize_SC)peekData;

                var ectypeUpanishads = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeSmg.dwEctypeContainerId].RoleUpanishads;

                m_heroUpanishads = null;

                m_heroUpanishads = ectypeUpanishads.SingleOrDefault(K => K.Vocation == vocation);

                if (m_heroUpanishads != null)
                {
                    playerBehaviour.AddSkillBase(m_heroUpanishads.UpanishadId);
                }
                else
                {
                    if (playerData.FATAL_SKILLID != 0)
                    {
                        playerBehaviour.AddSkillBase(playerData.FATAL_SKILLID);
                    }
                }

                break;
        }
    }
    private void SwitchPlayerAttachSkillBase(GameManager.GameState gameState)
    {
        this.m_playerList.ApplyAllItem(P =>
        {
            SwitchPlayerAttachSkillBase(P, gameState);
        });    
    }

	/// <summary>
	/// Stops the state of the player.
	/// </summary>
	public void StopPlayersSkill()
	{
		PlayerList.ApplyAllItem(p=>{
			if(((PlayerBehaviour)p.Behaviour).FSMSystem.CurrentStateID == StateID.MonsterAttack) 
			{
				((PlayerBehaviour)p.Behaviour).FSMSystem.PerformTransition(Transition.PlayerToIdle);
			}
		});
	}

    /// <summary>
    /// 施放入场技能
    /// </summary>
    public void PlayStartSkill()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.GAME_STATE_BATTLE)
            return;
        var peekData = GameDataManager.Instance.PeekData(DataType.InitializeEctype);
        if (GameManager.Instance.GetNewWorldMsg.byTeleportFlg != (int)eTeleportType.TELEPORTTYPE_CURMAP &&
                GameManager.Instance.GetNewWorldMsg.byTeleportFlg != (int)eTeleportType.TELEPORTTYPE_RECONNECTION)
        {            
            if (peekData == null)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"DataType.InitializeEctype is null");
                return;
            }
            SMSGEctypeInitialize_SC ectypeSmg = (SMSGEctypeInitialize_SC)peekData;
            var ectypeConfig = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeSmg.dwEctypeContainerId];
            int mapID = Convert.ToInt32(ectypeConfig.vectMapID.Split('+')[0]);
            if (mapID == GameManager.Instance.GetCurSceneMapID)
            {
                int vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
                var startSkill = ectypeConfig.StartSkills.SingleOrDefault(p => p.Vocation == vocation);
                if (startSkill != null)
                {
                    if (startSkill.SkillID != 0)
                    {
                        var player = (PlayerBehaviour)FindHeroEntityModel().Behaviour;
                        player.m_nextSkillID = startSkill.SkillID;
                        player.FSMSystem.PerformTransition(Transition.PlayerFireInitiativeSkill);
                    }

                }
            }
        }

        //SMSGEctypeInitialize_SC ectypeSmg = (SMSGEctypeInitialize_SC)peekData;
        //EctypeContainerData ectypeData;
        //if (EctypeConfigManager.Instance.EctypeContainerConfigList.ContainsKey((int)ectypeSmg.dwEctypeContainerId))
        //{
        //    ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[(int)ectypeSmg.dwEctypeContainerId];
        //    int vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
        //    var startSkill = ectypeData.StartSkills.SingleOrDefault(p => p.Vocation == vocation);
        //    if (startSkill != null)
        //    {
        //        if (startSkill.SkillID != 0)
        //        {
        //            var player = (PlayerBehaviour)FindHeroEntityModel().Behaviour;
        //            player.m_nextSkillID = startSkill.SkillID;
        //            player.FSMSystem.PerformTransition(Transition.PlayerFireInitiativeSkill);
        //        }

        //    }
        //}
    }
    #endregion   

    #region 玩家工厂创建玩家及设置玩家数据实体模型
    public void AttachEntityDataModel(GameObject go, Int64 uid)
    {
        var targetEntity = this.m_playerList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == uid);
        if (targetEntity != null)
        {
            targetEntity.GO = go;
        }
        else
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"AttachEntityDataModel GameObject 找不到EntityModel：" + uid);
        }
    }
    public void AttachEntityDataModel(View view, Int64 uid)
    {
        var targetEntity = this.m_playerList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == uid);
        if (targetEntity != null)
        {
            targetEntity.Behaviour = view;

            SyncSkills(targetEntity);
        }
        else
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"AttachEntityDataModel View 找不到EntityModel：" + uid);
        }        
    }
    /// <summary>
    /// 主玩家换形象
    /// </summary>
    public void HeroChangeAvatar(string avatarName)
    {
        RoleGenerate.GenerateRole(this.FindHero(), avatarName);        
    }
    #endregion

    #region IEntityManager实现
    public void RegisteEntity(EntityModel playerDataModel)
    {
        StringBuilder playerInfo = new StringBuilder();
        //playerInfo.Append("新增玩家：");
        //playerInfo.Append(" ID - "+ playerDataModel.EntityDataStruct.SMsg_Header.uidEntity);
        //string name=playerDataModel.EntityDataStruct.SMsg_Header.IsHero?((SMsgPropCreateEntity_SC_MainPlayer)playerDataModel.EntityDataStruct).Name:((SMsgPropCreateEntity_SC_OtherPlayer)playerDataModel.EntityDataStruct).Name;
        //playerInfo.Append(" Name - "+ name);

        if (!m_playerList.Exists(P => P.EntityDataStruct.SMsg_Header.uidEntity == playerDataModel.EntityDataStruct.SMsg_Header.uidEntity))
        {
            if(m_playerList.Exists(P => ((IPlayerDataStruct)P.EntityDataStruct).PlayerActorID== ((IPlayerDataStruct)playerDataModel.EntityDataStruct).PlayerActorID))
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"收到重复ActorId的玩家" + ((IPlayerDataStruct)playerDataModel.EntityDataStruct).PlayerActorID);
            }
            m_playerList.Add(playerDataModel);            
        }
        else
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"收到重复玩家的创建指令");
        }
       if (playerDataModel.EntityDataStruct.SMsg_Header.IsHero)
        {
            CurrentForce=PlayerDataManager.Instance.GetHeroForce();
        }
        //playerInfo.Append("周围玩家：");
        //m_playerList.ApplyAllItem(P =>
        //    {
        //        if(P.EntityDataStruct.SMsg_Header.uidEntity!= playerDataModel.EntityDataStruct.SMsg_Header.uidEntity)
        //        {
                   
        //            playerInfo.Append(" ID - "+ playerDataModel.EntityDataStruct.SMsg_Header.uidEntity);
        //            name=playerDataModel.EntityDataStruct.SMsg_Header.IsHero?((SMsgPropCreateEntity_SC_MainPlayer)playerDataModel.EntityDataStruct).Name:((SMsgPropCreateEntity_SC_OtherPlayer)playerDataModel.EntityDataStruct).Name;
        //            playerInfo.Append(" Name - "+ name);
        //        }
        //    });
        //EntityLog.Instance.WriteLog(MyLogType.PlayerEntity, playerInfo.ToString());
    }
    public void UnRegisteEntity(Int64 uidEntity)
    {
        var targetEntity = this.m_playerList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == uidEntity);
        if (targetEntity != null)
        {
            if (targetEntity.Behaviour is ISendInfoToServer)
            {
                GameManager.Instance.TimedSendPackage.RemoveSendInfoObj(targetEntity.Behaviour as ISendInfoToServer);
            }            
            m_playerList.Remove(targetEntity);

            targetEntity.DestroyEntity();
        }
        //检查Factory中是否有未创建的缓存数据，如果有，一并清除
        PlayerFactory.Instance.UnRegister(uidEntity);
    }

    public EntityModel GetEntityMode(long uid)
    {
        EntityModel item;
        for (int i = 0; i < this.m_playerList.Count; i++)
        {
            item = this.m_playerList[i];
            var playerDataModel = item.EntityDataStruct;
            if (playerDataModel != null && playerDataModel.SMsg_Header.uidEntity == uid)
            {
                return item;
            }
        }

        return null;
    }
    
    #endregion

    #region 框架消息处理函数
     /// <summary>
    /// 收到目标选择消息，驱动主角走向目标
    /// </summary>
    /// <param name="notifyArgs"></param>
    void ReceiveTargetSelectedHandle(INotifyArgs notifyArgs)
    {
        var targetSelected = (TargetSelected)notifyArgs;
        var hero = FindHero();
        if (hero != null)
        {
            var playerBehaviour = hero.GetComponent<PlayerBehaviour>();
            ((PlayerState)playerBehaviour.FSMSystem.CurrentState).SetTarget(targetSelected);
        }
        else
        {
            //TraceUtil.Log("玩家没找到");
        }

    }   
    void ReceiveFightCommandHandle(INotifyArgs notifyArgs)
    {
        SMsgBattleCommand sMsgBattleCommand = (SMsgBattleCommand)notifyArgs;
        //TraceUtil.Log("收到战斗指令:" + sMsgBattleCommand.uidFighter);
        var entityModel = GetEntityMode(sMsgBattleCommand.uidFighter);
        if (entityModel != null)
        {
            var playerBehaviour = (PlayerBehaviour)entityModel.Behaviour;
            if (playerBehaviour == null)
                return;
            if(sMsgBattleCommand.uidTarget!=0)
            {
                playerBehaviour.ActionLockTarget = MonsterManager.Instance.GetEntityMode(sMsgBattleCommand.uidTarget);
            }
            playerBehaviour.ThisTransform.position = playerBehaviour.ThisTransform.position.GetFromServer(sMsgBattleCommand.xPlayer, sMsgBattleCommand.yPlayer);
            var playerDirection = new Vector3(sMsgBattleCommand.xDirect, playerBehaviour.ThisTransform.position.y, -sMsgBattleCommand.yDirect);
            playerBehaviour.ThisTransform.eulerAngles = new Vector3(0, 90.0f - Mathf.Atan2(-sMsgBattleCommand.yDirect, sMsgBattleCommand.xDirect) * 180 / Mathf.PI, 0);
            var battleItemData = PlayerDataManager.Instance.GetBattleItemData(playerBehaviour.PlayerKind);

            //TraceUtil.Log("FightCode:" + sMsgBattleCommand.nFightCode);
            //TraceUtil.Log("普通攻击:" + battleItemData.NormalSkillID);
            //TraceUtil.Log("滚动技能:" + battleItemData.ScrollSkillID);
            if (sMsgBattleCommand.nFightCode == battleItemData.NormalSkillID[0]
				||sMsgBattleCommand.nFightCode == battleItemData.NormalSkillID[1]
				||sMsgBattleCommand.nFightCode == battleItemData.NormalSkillID[2]
				||sMsgBattleCommand.nFightCode == battleItemData.NormalSkillID[3]
                || sMsgBattleCommand.nFightCode == battleItemData.NormalSkillID[4]
                || sMsgBattleCommand.nFightCode == battleItemData.NormalSkillID[5])  //普通攻击
            {
                //playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerFireNormalSkill);
				playerBehaviour.GetSkillBySkillID(sMsgBattleCommand.nFightCode);
                if (playerBehaviour.SelectedSkillBase == null)
                {
                    return;
                }
                playerBehaviour.SkillFirePos = Vector3.zero.GetFromServer(sMsgBattleCommand.xMouse, sMsgBattleCommand.yMouse);
                playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerFireInitiativeSkill);
				
            }
            else if (sMsgBattleCommand.nFightCode == battleItemData.ScrollSkillID)
            {
                if (playerBehaviour.FSMSystem.CurrentStateID != StateID.PlayerScrollSkill)
                {
                    playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerFireScrollSkill);
                }                
            }
            else
            {
                //playerBehaviour.ThisTransform.position = playerBehaviour.ThisTransform.position.GetFromServer(sMsgBattleCommand.xPlayer, sMsgBattleCommand.yPlayer);
                //var playerDirection = new Vector3(sMsgBattleCommand.xDirect, playerBehaviour.ThisTransform.position.y, -sMsgBattleCommand.yDirect);
                //playerBehaviour.ThisTransform.eulerAngles = new Vector3(0, 90.0f - Mathf.Atan2(-sMsgBattleCommand.yDirect, sMsgBattleCommand.xDirect)*180/Mathf.PI, 0);

                playerBehaviour.GetSkillBySkillID(sMsgBattleCommand.nFightCode);
                if (playerBehaviour.SelectedSkillBase == null)
                {
                    return;
                }
                playerBehaviour.SkillFirePos = Vector3.zero.GetFromServer(sMsgBattleCommand.xMouse, sMsgBattleCommand.yMouse);
                playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerFireInitiativeSkill);
            }
        }
    }
    /// <summary>
    /// 收到击退指令处理
    /// </summary>
    /// <param name="notifyArgs"></param>
    void ReceiveBeatBackHandle(INotifyArgs notifyArgs)
    {
        SMsgBattleBeatBack_SC sMsgBattleBeatBack_SC = (SMsgBattleBeatBack_SC)notifyArgs;

        var entityModel = GetEntityMode(sMsgBattleBeatBack_SC.uidFighter);
        if (entityModel != null)
        {           
            var playerBehaviour=(PlayerBehaviour)entityModel.Behaviour;
            if (playerBehaviour == null)
                return;
            //恢复主角所选按钮的状态
            if (playerBehaviour.IsHero&&playerBehaviour.SelectedSkillBase != null)
            {
                BattleSkillButtonManager.Instance.SetButtonStatus(playerBehaviour.SelectedSkillBase.SkillId, SkillButtonStatus.Enable);
            }
            var fsmState = playerBehaviour.FSMSystem;
            var beatBack = fsmState.FindState(StateID.PlayerBeAttacked);
            if (beatBack != null)
            {
                ((PlayerBeAttackedState)beatBack).SetBeatBackData(sMsgBattleBeatBack_SC);
            }
            fsmState.PerformTransition(Transition.PlayerBeAttacked);
			BulletManager.Instance.TryDestroyBreakBullets(sMsgBattleBeatBack_SC.uidFighter);
        }
    }
    /// <summary>
    /// 收到击飞指令处理
    /// </summary>
    /// <param name="notifyArgs"></param>   
    void ReceiveFightFlyHandle(INotifyArgs notifyArgs)
    {
        SMsgFightHitFly_SC sMsgFightHitFly_SC = (SMsgFightHitFly_SC)notifyArgs;

        var entityModel = GetEntityMode(sMsgFightHitFly_SC.uidFighter);
        if (entityModel != null)
        {
            var playerBehaviour = (PlayerBehaviour)entityModel.Behaviour;
            if (playerBehaviour == null)
                return;
            //恢复主角所选按钮的状态
            if (playerBehaviour.IsHero && playerBehaviour.SelectedSkillBase != null)
            {
                BattleSkillButtonManager.Instance.SetButtonStatus(playerBehaviour.SelectedSkillBase.SkillId, SkillButtonStatus.Enable);
            }
            var fsmState = playerBehaviour.FSMSystem;
            var beatBack = fsmState.FindState(StateID.PlayerBeHitFly);
            if (beatBack != null)
            {
                ((PlayerBeHitFlyState)beatBack).BeHitFly(sMsgFightHitFly_SC);
            }
            fsmState.PerformTransition(Transition.PlayerToBeHitFly);
			BulletManager.Instance.TryDestroyBreakBullets(sMsgFightHitFly_SC.uidFighter);
        }
    }
    void ReceiveEntityTeleportHandle(INotifyArgs args)
    {
        SMsgFightTeleport_CSC sMsgFightTeleport_CSC = (SMsgFightTeleport_CSC)args;
        var target = m_playerList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == sMsgFightTeleport_CSC.uidFighter);
        if (target != null)
        {
            ((PlayerBehaviour)target.Behaviour).BeTeleport(sMsgFightTeleport_CSC);
        }
    }
    void ReceiveEntityHordeHandle(INotifyArgs notifyArgs)
    {
        SMsgFightHorde_SC sMsgFightHorde_SC = (SMsgFightHorde_SC)notifyArgs;

        var entityModel = GetEntityMode(sMsgFightHorde_SC.uidFighter);
        if (entityModel != null)
        {
            var playerBehaviour = (PlayerBehaviour)entityModel.Behaviour;
            if (playerBehaviour == null)
                return;
            var fsmState = playerBehaviour.FSMSystem;
            //var beatBack = fsmState.FindState(StateID.PlayerBeHitFly);
            //if (beatBack != null)
            //{
                
            //}
            //fsmState.PerformTransition(Transition.PlayerToBeHitFly);
			BulletManager.Instance.TryDestroyBreakBullets(sMsgFightHorde_SC.uidFighter);
        }
    }
    void ReceiveEntityMoveHandle(INotifyArgs notifyArgs)
    {
        var sMsgActionMove_SC = (SMsgActionMove_SCS)notifyArgs;
        var target = this.m_playerList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == sMsgActionMove_SC.uidEntity);
        if (target != null && target.Behaviour!=null)
        {
            ((PlayerBehaviour)target.Behaviour).EntityMove(sMsgActionMove_SC);            
        }
    }
    public void OnTouchDown(INotifyArgs e)
    {
        var hero = FindHero();
        if (hero != null)
        {
            var playerBehaviour = hero.GetComponent<PlayerBehaviour>();
            TouchInvoke touchInvoke = (TouchInvoke)e;
            playerBehaviour.GetTouchDown(touchInvoke);
        }
        else
        {
            TraceUtil.Log("玩家没找到");
        }        
    }
	
	public bool OnNormalSkillButtonPress(bool press)
	{
		bool normalAttackOfficient = true;
		if(null != HeroBehaviour)
		{
			HeroBehaviour.NormalAttackButtonPress = press;
			//普通攻击的记忆功能
			if(press)
			{
				if(HeroBehaviour.FSMSystem.CurrentStateID==StateID.PlayerIdle||HeroBehaviour.FSMSystem.CurrentStateID==StateID.PlayerRun)
				{
					NormalAttackRemembering=false;
				}
				else
				{
					NormalAttackRemembering=true;
					normalAttackOfficient= false;
				}
			}
		}
		return normalAttackOfficient;
	}
	PlayerBehaviour m_heroBehaviour=null;
	public bool NormalAttackRemembering=false;
	PlayerBehaviour HeroBehaviour
	{
		get{
			if (m_heroBehaviour == null) {
				EntityModel hero = FindHeroEntityModel();
				if(null != hero)
				{
					m_heroBehaviour = (PlayerBehaviour)(hero.Behaviour);	
				}
			}
			return m_heroBehaviour;
				}
	}
	public bool OnRememberNormalPress()
	{
		//如果不记忆【由于多段记忆时间到了】，直接返回施放成功。通知记忆
		if (!NormalAttackRemembering) 
        {
			HeroBehaviour.NormalAttackButtonPress = false;
			return true;
		}
		//如果当前处于Idle或Run状态，则施放技能
		if (HeroBehaviour.FSMSystem.CurrentStateID == StateID.PlayerIdle || HeroBehaviour.FSMSystem.CurrentStateID == StateID.PlayerRun) 
        {
			HeroBehaviour.NormalAttackButtonPress = true;

			return true;
		}
        else 
        {
			HeroBehaviour.NormalAttackButtonPress = false;
			return false;
				
        }
	}
	
	public void ConnectHeroJoyStick(JoyStick joyStick)
	{
		joyStick.AddPressDelegate(PadPress);
	}
	
	public void PadPress(bool isPress, Vector3 dir)
	{
		EntityModel hero = FindHeroEntityModel();
		if(null != hero&&hero.Behaviour!=null)
		{
			PlayerBehaviour pb = (PlayerBehaviour)(hero.Behaviour);	
			pb.PadPress(isPress, dir);
		}
	}
	
    private void SceneChangeHandle(INotifyArgs e)
    {
        var hero = FindHero();
        if (hero != null)
        {
            var playerBehaviour = hero.GetComponent<PlayerBehaviour>();
            playerBehaviour.Freeze(true);
        }
        else
        {
            //TraceUtil.Log("玩家没找到");
        }
    }
    public void SceneLoadedHandle(INotifyArgs e)
    {
        var hero = FindHero();
        if (hero != null)
        {
            var playerBehaviour = hero.GetComponent<PlayerBehaviour>();
            playerBehaviour.Freeze(false);                    
        }
        else
        {
            //TraceUtil.Log("玩家没找到");
        }

        ResetPlayerState();

    }
    private void ResetPlayerState()
    {
        //重置玩家状态,根据服务器下发状态，重置玩家状态
        this.m_playerList.ApplyAllItem(P =>
            {
                int UINT_FIELD_STATE;
                var playerDataStruct = (IPlayerDataStruct)P.EntityDataStruct;
                UINT_FIELD_STATE = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UINT_FIELD_STATE;

                switch (UINT_FIELD_STATE)
                {
                    case (int)CRT_STATE.enCrt_State_Die:
						if(P.Behaviour != null)
						{
							((PlayerBehaviour)P.Behaviour).Die();
						}   
						else
						{
							Debug.LogError("Player Behaviour is null");
						}
                        break;                        
                    default:
                        //((PlayerBehaviour)P.Behaviour).Relive();
                        break;
                }
            });
    }
    void ReceiveEntityDieHandle(INotifyArgs args)
    {        
        var sMsgActionDie_SC = (SMsgActionDie_SC)args;
        if (GameManager.Instance.CreateEntityIM)
        {
            var entityModel = this.GetEntityMode(sMsgActionDie_SC.uidEntity);
            if (entityModel != null)
            {
                TraceUtil.Log("收到玩家死亡消息2" + sMsgActionDie_SC.uidEntity); //如果是主玩家，则需要改变摄像机跟随
                ((PlayerBehaviour)entityModel.Behaviour).Die();
            }
        }
        else
        {
			GameManager.Instance.PlayerFactory.RegisterPlayerAfterSceneLoadedFun("ReceiveEntityDieHandle",() =>
            {
                ReceiveEntityDieHandle(sMsgActionDie_SC);
            });
        }
    }
    void ReceiveEntityReliveHandle(INotifyArgs args)
    {
        SMsgActionRelivePlayer_SC sMsgActionRelivePlayer_SC = (SMsgActionRelivePlayer_SC)args;
        var entityModel = PlayerManager.Instance.FindPlayerByActorId(sMsgActionRelivePlayer_SC.actorTarget);
        if (entityModel != null)
        {
            //TraceUtil.Log("收到玩家复活");
            ((PlayerBehaviour)entityModel.Behaviour).Relive();
        }
    }
    void ReceiveBreakSkillHandle(INotifyArgs args)
    {
        SMsgFightBreakSkill_SC sMsgFightBreakSkill_SC = (SMsgFightBreakSkill_SC)args;
        if (sMsgFightBreakSkill_SC.uidFighter != FindHeroDataModel().SMsg_Header.uidEntity)
        {
            var entityModel = this.GetEntityMode(sMsgFightBreakSkill_SC.uidFighter);
            if (entityModel != null)
            {
                //TraceUtil.Log("收到技能打断");
                ((PlayerBehaviour)entityModel.Behaviour).BreakCurrentSkill(sMsgFightBreakSkill_SC.SkillID);
            }
        }
    }
    void FightChangeDirectHandle(INotifyArgs args)
    {
        var sMsgDestoryBullet_SC = (SMsgFightChangeDirect_SC)args;
        if (sMsgDestoryBullet_SC.uidFighter != FindHeroDataModel().SMsg_Header.uidEntity)
        {
            var entityModel = this.GetEntityMode(sMsgDestoryBullet_SC.uidFighter);
            if (entityModel != null)
            {
                //TraceUtil.Log("收到技能改变方向");
                ((PlayerBehaviour)entityModel.Behaviour).FightChangeDirect(sMsgDestoryBullet_SC.DirX, sMsgDestoryBullet_SC.DirY);
            }
        }

    }
    void ReceiveBeAdsorbHandle(INotifyArgs notifyArgs)
    {
        SMsgBattleBeAdsorb_SC sMsgBattleBeAdsorb_SC = (SMsgBattleBeAdsorb_SC)notifyArgs;

        var entityModel = GetEntityMode(sMsgBattleBeAdsorb_SC.uidFighter);
        if (entityModel != null)
        {
            var playerBehaviour = (PlayerBehaviour)entityModel.Behaviour;
            var fsmState = playerBehaviour.FSMSystem;
            var beatBack = fsmState.FindState(StateID.PlayerBeAdsorb);
            if (beatBack != null)
            {
                ((PlayerBeAdsorbState)beatBack).SetBeAdsorbData(sMsgBattleBeAdsorb_SC);
            }
            fsmState.PerformTransition(Transition.PlayerBeAdsorb);
        }
    }   
	
	void ReceiveBeAdsorbExHandle(INotifyArgs notifyArgs)
	{
		SMsgFightAdsorptionEx_SC sMsgFightAdsorptionEx_SC = (SMsgFightAdsorptionEx_SC)notifyArgs;
		var entityModel = GetEntityMode(sMsgFightAdsorptionEx_SC.uidFighter);
        if (entityModel != null)
        {
			var playerBehavior = (PlayerBehaviour)entityModel.Behaviour;
			playerBehavior.BeAdsorbEx(sMsgFightAdsorptionEx_SC);
		}
	}
    #endregion

    #region 玩家技能同步 与 技能公开属性
    public SMsgSkillInit_SC HeroSMsgSkillInit_SC
    {
        get { return m_heroSMsgSkillInit_SC; }
        set
        {
            m_heroSMsgSkillInit_SC = value;
        }
    }
    /// <summary>
    /// 副本技能同步协议下发，找到相关实体，加载技能
    /// </summary>
    /// <param name="sMSGEctypeSynSkillData_SC"></param>
    public void EctypeSynSkillDataHandler(SMSGEctypeSynSkillData_SC sMSGEctypeSynSkillData_SC)
    {
        SyncSkills(sMSGEctypeSynSkillData_SC);
    }
    private SMSGEctypeSynSkillData_SC? m_synSkillDataCache;
    private void SyncSkills(SMSGEctypeSynSkillData_SC sMSGEctypeSynSkillData_SC)
    {
        if (GameManager.Instance.CreateEntityIM)
        {
            for (int i = 0; i < sMSGEctypeSynSkillData_SC.byMemberNum; i++)
            {
                var msg = sMSGEctypeSynSkillData_SC.sSkillShowRes[i];
                var targetPlayer = this.m_playerList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == msg.uidEntity);

                if (targetPlayer != null)
                {
                    msg.sSkills.ApplyAllItem(skill =>
                    {
                        if (skill.bySkillID != 0)
                        {
                            //TraceUtil.Log("装配技能,是否主角：" + targetPlayer.EntityDataStruct.SMsg_Header.IsHero + " 技能ID:" + skill.bySkillID);
                            ((PlayerBehaviour)targetPlayer.Behaviour).AddSkillBase(skill.bySkillID);
                        }
                    });
                }
                else
                {
                    m_sSkillShowRes.Add(msg);
                }
            }
        }
        else
        {
            m_synSkillDataCache = sMSGEctypeSynSkillData_SC;
			GameManager.Instance.PlayerFactory.RegisterPlayerAfterSceneLoadedFun("SyncSkills",() =>
                {
                    if (m_synSkillDataCache != null)
                    {
                        SyncSkills(m_synSkillDataCache.Value);
                    }
                });
        }
    }
    private void SyncSkills(EntityModel playerModel)
    {
        if (playerModel != null)
        {
            lock (m_lockObj)
            {
                var skills = m_sSkillShowRes.Where(P => P.uidEntity == playerModel.EntityDataStruct.SMsg_Header.uidEntity);
                skills.ApplyAllItem(skill =>
                    {
                        skill.sSkills.ApplyAllItem(item =>
                        {
                            if (item.bySkillID != 0)
                            {
                                ((PlayerBehaviour)playerModel.Behaviour).AddSkillBase(item.bySkillID);
                            }
                        });                        

                    });
                m_sSkillShowRes.RemoveAll(skillRes => skillRes.uidEntity == playerModel.EntityDataStruct.SMsg_Header.uidEntity);
            }
        }        
    }
    #endregion

    #region ISingletonLifeCycle 实现
    public void Instantiate()
    {
        
    }

    public void LifeOver()
    {
        var heroEntityMode=this.FindHeroEntityModel();
        if (heroEntityMode != null)
        {
            //heroEntityMode.GO.RemoveComponent<DontDestroy>("DontDestroy");
            GameManager.Instance.TimedSendPackage.RemoveSendInfoObj((PlayerBehaviour)heroEntityMode.Behaviour);
        }
        foreach (var player in this.PlayerList)
        {
            if (player != null && player.GO != null)
            {
                GameObject.Destroy(player.GO);
            }
        }
		PlayerList.Clear();
		m_playerList = null;
        this.ClearEvent();
        m_instance = null;
    }
    #endregion



    #region gold and cash
    //玩家元宝是否足够
    public bool IsBindPayEnough(int pay)
    {
        return PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY >= pay;
    }

    //铜钱是否足够
    public bool IsMoneyEnough(int money)
    {
        return PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY >= money;
    }


    #endregion

    void EntityUpdateValuesHandle(INotifyArgs args)
    {
        //DoForTime.stop();
        DoForTime.DoFunForTime(0.5f, ChangeCurrentValue, null);
        if(CurrentForce<PlayerDataManager.Instance.GetHeroForce())
            {
                int force=PlayerDataManager.Instance.GetHeroForce()-CurrentForce;
            if(PopupObjManager.Instance!=null)
            {
                PopupObjManager.Instance.ForceAddEffect(force);
            }
                ///UIEventManager.Instance.TriggerUIEvent(UIEventType.AddForce,force);
            }

       
    }
    void ChangeCurrentValue(object obj)
    {
        CurrentForce=PlayerDataManager.Instance.GetHeroForce();
    }
}
