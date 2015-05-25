using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// EnterPoint Scene GameManager
/// </summary>
public class PlayerFactory : MonoBehaviour {

	private Dictionary<string,Action> m_sceneLoadedToHandler=new Dictionary<string,Action>();

    private bool m_createPlayerIm;  //收到玩家信息是否立即创建
    private List<IEntityDataStruct> m_preCreatePlayerStructCache;

    public GameObject[] Weapons;
    public Transform HeroClickPointEffect;
    /// <summary>
    /// 其他玩家头顶名字
    /// </summary>
    public HeroTitle HeroTitlePrefab;
    private static PlayerFactory m_instance;
    public static PlayerFactory Instance
    {
        get
        {
            return m_instance;
        }
    }
    void Awake()
    {
        m_instance = this;
        m_createPlayerIm = false;
        m_preCreatePlayerStructCache = new List<IEntityDataStruct>();
    }
	public void RegisterPlayerAfterSceneLoadedFun(string obj,Action act)
    {
		this.m_sceneLoadedToHandler[obj]=act;
    }
    public void CreatePlayerObject()
    {
        this.m_createPlayerIm = true;

        foreach (var dataStruct in this.m_preCreatePlayerStructCache)
        {
            CreatePlayer(dataStruct, EntityModelPartial.GameObject);
        }
        this.m_preCreatePlayerStructCache.Clear();

        //玩家创建后读取缓存的数据
        this.m_sceneLoadedToHandler.ApplyAllItem(P=>P.Value());
        this.m_sceneLoadedToHandler.Clear();
    }
    private void CreatePlayer(IEntityDataStruct entityDataStruct, EntityModelPartial entityModelPartial)
    {        
		//if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_STORYLINE)
		//{
		//	return;	
		//}
		
		
        switch (entityModelPartial)
        {
            case EntityModelPartial.DataStruct:
                EntityModel playerDataModel = new EntityModel();
                playerDataModel.EntityDataStruct = entityDataStruct;
                EntityController.Instance.RegisteEntity(entityDataStruct.SMsg_Header.uidEntity, playerDataModel);

                break;
            case EntityModelPartial.GameObject:

                if (null==PlayerManager.Instance.FindPlayerByActorId(((IPlayerDataStruct)entityDataStruct).PlayerActorID))
                {
                    return;
                }
                //应该按ID或类型读配置加载不同的角色。
                bool isHero = entityDataStruct.SMsg_Header.nIsHero == 1;
                float x = 100, z = -100;

                byte kind = 0;
                int fashionId=0;
                var playerDataStruct = (IPlayerDataStruct)entityDataStruct;
                x = playerDataStruct.PlayerX;
                z = playerDataStruct.PlayerY;
                kind = (byte)playerDataStruct.GetCommonValue().PLAYER_FIELD_VISIBLE_VOCATION;
                fashionId = playerDataStruct.GetCommonValue().PLAYER_FIELD_VISIBLE_FASHION;
                
                bool inTown = GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN;
                var configData = inTown ?
                    PlayerDataManager.Instance.GetTownItemData(kind)
                    : PlayerDataManager.Instance.GetBattleItemData(kind);

                if (configData == null)
                    TraceUtil.Log("角色类型:"+kind+" 找不到配置信息");
                var fashionItem=ItemDataManager.Instance.GetItemData(fashionId);

                var player = AssemblyPlayer(configData, kind, isHero, fashionItem == null?null:fashionItem._ModelId);

                player.layer =isHero?10:18; //Player
                var heroBehaviour = player.AddComponent<PlayerBehaviour>();
                player.AddComponent<PlayerHurtFlash>();  //增加被攻击或者爆气特效脚本
                //heroBehaviour.ClickPointEffect = HeroClickPointEffect;   主角点击场面的反馈特效。
                var shadowEff=GameObject.Instantiate(configData.ShadowEffect) as GameObject;
                shadowEff.name = "shadow";
                shadowEff.transform.parent = player.transform;
                shadowEff.transform.localPosition = new Vector3(0, 1, 0);
				//cache hurt point
				heroBehaviour.CacheHurtPoint();
                heroBehaviour.CacheShadow();

                PlayerGenerateConfigData battleConfig = PlayerDataManager.Instance.GetBattleItemData(kind);
                if(null != battleConfig.RunEffect)
                {
                    GameObject runEff = GameObject.Instantiate(battleConfig.RunEffect) as GameObject;
                    runEff.name = "run_effect";
                    runEff.transform.parent = player.transform;
                    runEff.transform.localPosition = Vector3.zero;
                    runEff.transform.localScale = Vector3.one;
                    heroBehaviour.CacheRunEffect();

                }


                var UIConfig = PlayerDataManager.Instance.GetUIItemData(kind);
                heroBehaviour.HitFlyHeight = UIConfig.HitFlyHeight;
                heroBehaviour.BeBeAttackBreakLV = UIConfig.BeAttackBreakLV;
                heroBehaviour.BeHitFlyBreakLV = UIConfig.BeHitFlyBreakLV;

                heroBehaviour.PlayerKind = kind;

                var playerPosition = new Vector3(0, 0.1f, 0);
                playerPosition = playerPosition.GetFromServer(x, z);

                player.transform.position = playerPosition;
                Quaternion playerDir = Quaternion.Euler(0, (float)playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_DIR/1000.0f, 0);
                player.transform.rotation = playerDir;
			
				TypeID type;
                heroBehaviour.EntityModel = EntityController.Instance.GetEntityModel( entityDataStruct.SMsg_Header.uidEntity, out type);

                if (isHero)
                {
                    player.AddComponent<DontDestroy>();                    
                }
                else
                {
                    if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE)
                    {
                        //加入箭头挂载脚本
                        #if !UNITY_EDITOR
                        ArrowManager.Instance.AddTeammateArrow(player);
                        #endif
                    }
                }

                #region 加入称号动画 (去掉)
                /*
                int prestigeLevel = playerDataStruct.GetCommonValue().PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL;
                var prestigeData = PlayerDataManager.Instance.GetPlayerPrestigeList().SingleOrDefault(p => p._pvpLevel == prestigeLevel);
                if (prestigeData != null)
                {
                    GameObject medalPrefab = prestigeData._titlePrefab;
                    if (medalPrefab != null)
                    {
                        GameObject medal = (GameObject)Instantiate(medalPrefab);
                        MedalEffectBehaviour medalBehaviour = medal.AddComponent<MedalEffectBehaviour>();
                        medalBehaviour.SetHeroTransform(heroBehaviour.transform);                        
                        if (entityDataStruct.SMsg_Header.IsHero)
                        {
                            medal.AddComponent<DontDestroy>();
                        }
                        MedalManager.Instance.RegisterMedal(entityDataStruct.SMsg_Header.uidEntity, prestigeLevel, medalBehaviour);
                    }                    
                }
                */
                #endregion

                //加入头顶名字
                //if (playerDataStruct is SMsgPropCreateEntity_SC_MainPlayer)
                //{
                //    CreateTitle(((SMsgPropCreateEntity_SC_MainPlayer)playerDataStruct).Name, isHero, player.transform);
                //}
                if(GameManager.Instance.CurrentState != GameManager.GameState.GAME_STATE_PLAYERROOM && playerDataStruct is SMsgPropCreateEntity_SC_OtherPlayer)
                {
				    var title = CreateTitle(entityDataStruct.SMsg_Header.uidEntity, ((SMsgPropCreateEntity_SC_OtherPlayer)playerDataStruct).Name, isHero, player.transform);
                    heroBehaviour.PlayerTitleRef = title;
                }


				//称号
				//CreateTitle(entityDataStruct.SMsg_Header.uidEntity,player.transform);


                PlayerManager.Instance.AttachEntityDataModel(player, entityDataStruct.SMsg_Header.uidEntity);
                PlayerManager.Instance.AttachEntityDataModel(heroBehaviour, entityDataStruct.SMsg_Header.uidEntity);
               
				if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE)
				{
               		GameManager.Instance.TryFindCameraTarget();
				}

                //根据当前游戏场景，改变玩家的技能，武器挂载等
                ////TraceUtil.Log("根据当前游戏场景，改变玩家的技能，武器挂载等在PlayerFactory");
				PlayerManager.Instance.PlayerChangeScene(GameManager.Instance.CurrentState,entityDataStruct.SMsg_Header.uidEntity);
                //PlayerManager.Instance.PlayerCreateProcess(player, heroBehaviour,GameManager.Instance.CurrentState);
                break;
        }
    }

    /// <summary>
    /// 创建头顶名字
    /// </summary>
    /// <param name="name"></param>
	public GameObject CreateTitle(long uid, string name, bool isMySelf, Transform followTarget)
    {        
        var npcTitleGo = (GameObject)GameObject.Instantiate(HeroTitlePrefab.gameObject, Vector3.left * 2000, Quaternion.identity);
        npcTitleGo.transform.parent = BattleManager.Instance.UICamera.transform;
        npcTitleGo.transform.localScale = new Vector3(1, 1, 1);
		npcTitleGo.GetComponent<HeroTitle>().SetHeroTitle(uid, name, isMySelf, followTarget);
        return npcTitleGo;
    }

	/// <summary>
	/// 创建称号
	/// </summary>
	/// <param name="playerUID">Player user interface.</param>
//	public GameObject CreateTitle(Int64 playerUID, Transform followTarget)
//	{
//		var playerModel = PlayerManager.Instance.GetEntityMode(playerUID);
//		if (playerModel == null)
//			return null;
//		bool isMainPlayer = false;
//		int titleID = 0;
//		if(playerModel.EntityDataStruct is SMsgPropCreateEntity_SC_MainPlayer)
//		{
//			isMainPlayer = true;
//			titleID =  ((SMsgPropCreateEntity_SC_MainPlayer)playerModel.EntityDataStruct).PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_TITLE;
//		}else
//		{
//			titleID =  ((SMsgPropCreateEntity_SC_OtherPlayer)playerModel.EntityDataStruct).PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_TITLE;
//		}
//		var titleData = PlayerDataManager.Instance.GetPlayerTitleConfigData(titleID);
//		if(titleData == null)
//			return null;
//		GameObject titleGameObj = (GameObject)UI.CreatObjectToNGUI.InstantiateObj( titleData._ModelIdPrefab, PopupObjManager.Instance.UICamera.transform);
//
//		//改为2DUIBack
//		var child = titleGameObj.GetComponentsInChildren<Transform>(true);
//		for(int i= 0;i<child.Length;i++)
//		{
//			child[i].gameObject.layer = 26;
//		}
//
//		TitleEffectBehaviour tb = titleGameObj.AddComponent<TitleEffectBehaviour>();
//		tb.SetHeroTransform(followTarget);
//		PlayerTitleManager.Instance.RegisterTitle(playerUID,titleGameObj);
//
//		return titleGameObj;
//	}
	/// <summary>
	/// Creates the title.
	/// </summary>
	/// <returns>The title.</returns>
	/// <param name="playerUID">Player user interface.</param>
	/// <param name="followPoint">Follow point.</param>
	public GameObject CreateTitle(Int64 playerUID, Transform followPoint)
	{
		var playerModel = PlayerManager.Instance.GetEntityMode(playerUID);
		if (playerModel == null)
			return null;
		bool isMainPlayer = false;
		int titleID = 0;
		if(playerModel.EntityDataStruct is SMsgPropCreateEntity_SC_MainPlayer)
		{
			isMainPlayer = true;
			titleID =  ((SMsgPropCreateEntity_SC_MainPlayer)playerModel.EntityDataStruct).PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_TITLE;
		}else
		{
			titleID =  ((SMsgPropCreateEntity_SC_OtherPlayer)playerModel.EntityDataStruct).PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_TITLE;
		}
		var titleData = PlayerDataManager.Instance.GetPlayerTitleConfigData(titleID);
		if(titleData == null)
			return null;
		GameObject titleGameObj = (GameObject)UI.CreatObjectToNGUI.InstantiateObj( titleData._ModelIdPrefab, followPoint);
		
		//改为2DUIBack
		var child = titleGameObj.GetComponentsInChildren<Transform>(true);
		for(int i= 0;i<child.Length;i++)
		{
			child[i].gameObject.layer = 26;
		}

		PlayerTitleManager.Instance.RegisterTitle(playerUID,titleGameObj);
		
		return titleGameObj;
	}

	/// <summary>
	/// 设置称号
	/// </summary>
	/// <param name="playerUID">Player user interface.</param>
	/// <param name="followPoint">Follow point.</param>
	/// <param name="titleID">Title I.</param>
	public void SetDesignation(Int64 playerUID, Transform followPoint, int titleID)
	{
		var titleData = PlayerDataManager.Instance.GetPlayerTitleConfigData(titleID);
		if(titleData == null)
			return ;
		GameObject titleGameObj = (GameObject)UI.CreatObjectToNGUI.InstantiateObj( titleData._ModelIdPrefab, followPoint);
		
		//改为2DUIBack
		var child = titleGameObj.GetComponentsInChildren<Transform>(true);
		for(int i= 0;i<child.Length;i++)
		{
			child[i].gameObject.layer = 26;
		}
		
		PlayerTitleManager.Instance.RegisterTitle(playerUID,titleGameObj);
	}


    /// <summary>
    /// 创建勋章
    /// </summary>
    public void CreateMedal(Int64 playerUID, int prestigeLevel)
    {
        var playerModel = PlayerManager.Instance.GetEntityMode(playerUID);
        if (playerModel == null)
            return;
        var prestigeData = PlayerDataManager.Instance.GetPlayerPrestigeList().SingleOrDefault(p => p._pvpLevel == prestigeLevel);
        GameObject medalPrefab = prestigeData._titlePrefab;
        if (medalPrefab != null)
        {
            GameObject medal = (GameObject)Instantiate(medalPrefab);
            MedalEffectBehaviour medalBehaviour = medal.AddComponent<MedalEffectBehaviour>();
            if (playerModel.Behaviour == null)
                return;
            medalBehaviour.SetHeroTransform(playerModel.Behaviour.transform);
            if (playerModel.EntityDataStruct.SMsg_Header.IsHero)
            {
                medal.AddComponent<DontDestroy>();
            }
            MedalManager.Instance.RegisterMedal(playerUID, prestigeLevel, medalBehaviour);
        }
    }

    /// <summary>
    /// 装配角色
    /// </summary>
    /// <param name="playerKind"></param>
    /// <param name="isHero"></param>
    /// <returns></returns>
    private GameObject AssemblyPlayer(PlayerGenerateConfigData configData, byte playerKind,bool isHero,string avatarName)
    {
        string defaultAnim,defaultAvatar,defaultWeapon;
        string[] attachAnims, weaponPosition;
        
        defaultAnim = configData.DefaultAnim;
        attachAnims = configData.Animations;
        defaultAvatar = string.IsNullOrEmpty(avatarName)? configData.DefaultAvatar:avatarName;
        defaultWeapon = configData.DefaultWeapon;
        weaponPosition = configData.WeaponPosition;

		//如果是客户端主角，使用高模，不是则使用低模
		var player = isHero ? RoleGenerate.GenerateRole(configData.PlayerName, defaultAvatar, isHero) : 
			RoleGenerate.GenerateRole(configData.PlayerName, defaultAvatar, isHero, MESHDENSITY.LOW);
	
        player.name = configData.PlayerName;

        RoleGenerate.AttachAnimation(player, configData.PlayerName, defaultAnim, attachAnims);

        var weapon = Weapons.SingleOrDefault(P => P.name == defaultWeapon);

        RoleGenerate.AttachWeapon(player, weaponPosition, weapon,null);  
		
        return player;
    }

	/// <summary>
	/// 装配机器人玩家
	/// </summary>
	/// <returns>The robot.</returns>
	/// <param name="configData">Config data.</param>
	/// <param name="playerKind">Player kind.</param>
	/// <param name="avatarName">Avatar name.</param>
	public GameObject AssemblyRobot(PlayerGenerateConfigData configData, byte playerKind ,string avatarName)
	{
		return this.AssemblyPlayer(configData,playerKind,false,avatarName);
	}


    public GameObject GetDefulWeaponPrefab(byte kind)
    {
        bool inTown = GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN;
        var configData = inTown ?
            PlayerDataManager.Instance.GetTownItemData(kind)
            : PlayerDataManager.Instance.GetBattleItemData(kind);

        string defaultWeaponName = configData.DefaultWeapon;
        return GetWeaponPrefab(defaultWeaponName);
    }
    public GameObject GetWeaponPrefab(string WeaponStr)
    {
        return Weapons.Single(P => P.name == WeaponStr);
    }

    /// <summary>
    ///  收到注册玩家信息，判定玩家创建开关是否打开，如未打开则存入
    /// </summary>
    /// <param name="sMsgPropCreateEntity_SC_Player"></param>
    public void Register(IEntityDataStruct entityDataStruct)
    {
        if (PlayerManager.Instance != null)
        {
            CreatePlayer(entityDataStruct, EntityModelPartial.DataStruct);
            if (!GameManager.Instance.CreateEntityIM)
            {
                m_preCreatePlayerStructCache.Add(entityDataStruct);
            }
            else
            {
                CreatePlayer(entityDataStruct, EntityModelPartial.GameObject);
            }
        }
    }
    /// <summary>
    /// 当发生实体删除时，需要检查缓存里是否有未创建的实体数据，一并删除
    /// </summary>
    /// <param name="uid"></param>
    public void UnRegister(long uid)
    {
        m_preCreatePlayerStructCache.RemoveAll(P => P.SMsg_Header.uidEntity == uid);
    }
}

/* 玩家工厂创建过程
     1、首次登录，收到创建玩家协议。 Register方法会被调用。
         （1）首先使用EntityModelPartial.DataStruct枚举 把收到的玩家数据缓存。
         （2）判断m_createPlayerIm 变量决定是否马上创建GameObject，如果不是，则缓存数据。
         （3）在收到SceneLoaded消息时，会把缓存的玩家数据提取出来，创建玩家GameObject，并清空缓存
     2、玩家GameObject的创建包括以下步骤：
         （1）从配置文档读取玩家类型。
         （2）装配玩家--玩家Transform，SkinnedRender，Animation，Weapon
         （3）附加PlayerBehaviour行为组件
         （4）创建玩家脚底阴影
         （5）定位玩家
         （6）摄像机跟随主角
         （7）调用PlayerManager.Instance.PlayerChangeScene(GameManager.Instance.CurrentState);
             根据游戏当前场景装配玩家身上技能，武器挂载点等
 * */
