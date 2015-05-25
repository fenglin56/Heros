using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using UI;
using UI.Battle;

/// <summary>
/// BattleUI Scene BattleDataManager  怪物工厂
/// </summary>
public class MonsterFactory : MonoBehaviour
{

    private bool m_createMonster;  //收到怪物信息是否立即创建
    private List<IEntityDataStruct> m_preCreateMonsterStructCache;
   

    #region 转移到BattleResManager
    public GameObject BossStatusPanelPrefab
    {
        get
        {
            return BattleResManager.Instance.BossStatusPanelPrefab;
        }
    }
    //血条（普通，Boss，精英）
    public GameObject BloobBarPrefab_normal
    {
        get
        {
            return BattleResManager.Instance.BloobBarPrefab_normal;
        }
    }
    public GameObject BloobBarPrefab_boss
    {
        get
        {
            return BattleResManager.Instance.BloobBarPrefab_boss;
        }
    }
    public GameObject BloobBarPrefab
    {
        get
        {
            return BattleResManager.Instance.BloobBarPrefab;
        }
    }
    public GameObject MonsterDialogPrefab
    {
        get
        {
            return BattleResManager.Instance.MonsterDialogPrefab;
        }
    }
    #endregion

    private static MonsterFactory m_instance = null;
    public static MonsterFactory Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType(typeof(MonsterFactory)) as MonsterFactory;
            }
            return m_instance;
        }
    }

    void OnDestroy()
    {
        m_instance = null;
    }

    void Awake()
    {
        m_instance = this;

        m_createMonster = false;
        m_preCreateMonsterStructCache = new List<IEntityDataStruct>();
    }

    //\打印初始化 暂放此处处理
    void Start()
    {
#if UNITY_EDITOR        
#elif UNITY_STANDALONE_WIN 
        Log.Instance.ClearLog();
        Log.Instance.RegisterLogCallback();
#elif UNITY_ANDROID
#endif

        //StartPrint(10f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Log.Instance.SaveTxt();
        }
        
    }

    void OnApplicationQuit()
    {
        Log.Instance.SaveTxt();
    }

    #region  打印测试
    public void StartPrint(float delayTime)
    {
        #if UNITY_EDITOR 
        Log.Instance.StartTiming();
        if (Log.IsPrint)
        {
            InvokeRepeating("WriteLog", delayTime, 0.1f);
        }
        #elif UNITY_STANDALONE_WIN               
        #endif
    }
    public void EndPrint()
    {
        CancelInvoke("WriteLog");
    }
    private string TransStateFont(StateID stateID)
    {
        switch (stateID)
        {
            case StateID.PlayerRun:
            case StateID.MonsterMove:
                return "走动";
            case StateID.MonsterIdle:
            case StateID.PlayerIdle:
                return "空闲";
            case StateID.PlayerNormalSkill:
            case StateID.PlayerScrollSkill:
            case StateID.PlayerInitiativeSkill:
            case StateID.MonsterAttack:
                return "攻击";
            case StateID.MonsterDie:
            case StateID.PlayerDie:
                return "死亡";
            case StateID.PlayerBeAttacked:
            case StateID.MonsterBeAttacked:
                return "受攻";
        }
        return "不明状态";
    }

    void WriteLog()
    {
        var monsterList = MonsterManager.Instance.GetMonstersList();
        monsterList.Sort(Comparer);

        SMsgPropCreateEntity_SC_MainPlayer? playerData = PlayerManager.Instance.FindHeroDataModel();
        var playerEntity = PlayerManager.Instance.FindHeroEntityModel();

        if (playerData == null || playerEntity == null || playerEntity.GO == null)
            return;

        float rad = playerEntity.GO.transform.eulerAngles.y * Mathf.Deg2Rad;
        //float dirX = Mathf.Cos(rad);
        //float dirY = Mathf.Sin(rad);        

        var euler = playerEntity.Behaviour.transform.rotation.eulerAngles;
        var d = Quaternion.Euler(euler) * Vector3.forward;
        float dirX = d.x;
        float dirY = d.z;
        
        Log.Instance.StartLog();
        //Log.Instance.AddLog((playerData.Value.UID << 32 >> 32).ToString(), TransStateFont(((PlayerBehaviour)playerEntity.Behaviour).FSMSystem.CurrentStateID),
        //    ((int)(playerEntity.GO.transform.position.x * 10)).ToString(), ((int)(playerEntity.GO.transform.position.z * -10)).ToString(),
        //    dirX.ToString("F2"), (dirY * -1f).ToString("F2"));
        
        monsterList.ApplyAllItem(p =>
            {
                //Log.Instance.WriteLog(p.EntityDataStruct.SMsg_Header.uidEntity.ToString(),p.GO.transform.position.ToString(), p.GO.transform.rotation.eulerAngles.ToString());
                float mrad = (90 - p.GO.transform.eulerAngles.y) * Mathf.Deg2Rad;
                float mdirX = Mathf.Cos(mrad);
                float mdirY = Mathf.Sin(mrad);

                Log.Instance.AddLog((p.EntityDataStruct.SMsg_Header.uidEntity << 32 >> 32).ToString(), TransStateFont(((MonsterBehaviour)p.Behaviour).FSMSystem.CurrentStateID),
                    ((int)(p.GO.transform.position.x * 10)).ToString(), ((int)(p.GO.transform.position.z * -10)).ToString(),
                    mdirX.ToString("F2"), (mdirY * -1f).ToString("F2")); 
            });
        Log.Instance.AppendLine();  //换行写入并保存        
    }

    private int Comparer(EntityModel x, EntityModel y)
    {
        return (int)(x.EntityDataStruct.SMsg_Header.uidEntity - y.EntityDataStruct.SMsg_Header.uidEntity);
    }
    #endregion

    public void CreateMonsterObject()
    {
        this.m_createMonster = true;

        foreach (var dataStruct in this.m_preCreateMonsterStructCache)
        {
            CreateMonster(dataStruct);
        }
        this.m_preCreateMonsterStructCache.Clear();
    }
    public void Register(IEntityDataStruct entityDataStruct)
    {
        if (MonsterManager.Instance != null)
        {
            if (!GameManager.Instance.CreateEntityIM)
            {
                m_preCreateMonsterStructCache.Add(entityDataStruct);
            }
            else
            {
                CreateMonster(entityDataStruct);
            }
        }
    }
    /// <summary>
    /// 当发生实体删除时，需要检查缓存里是否有未创建的实体数据，一并删除
    /// </summary>
    /// <param name="uid"></param>
    public void UnRegister(long uid)
    {
        m_preCreateMonsterStructCache.RemoveAll(P => P.SMsg_Header.uidEntity == uid);
    }

    private void CreateMonster(IEntityDataStruct entityDataStruct)
    {
		EntityModel monsterDataModel = MonsterManager.Instance.GetEntityMode(entityDataStruct.SMsg_Header.uidEntity);
		if(monsterDataModel == null)
		{
			Debug.LogError("monster uid : "+entityDataStruct.SMsg_Header.uidEntity.ToString()+" cant find");
		}
		var sMsgPropCreateEntity_SC_Monster = (SMsgPropCreateEntity_SC_Monster)monsterDataModel.EntityDataStruct;		                                                                           		                                                                         
		                                                                            
	    int monsterId = sMsgPropCreateEntity_SC_Monster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID;
        //TraceUtil.Log("==>创建怪物ID: " + monsterId);
        var monsterData = BattleConfigManager.Instance.GetMonsterData(monsterId);
        if (monsterData == null)
        {
            Debug.LogWarning("找不到怪物配置");
            return;
        }                         
        var pos = Vector3.zero;
        pos = pos.GetFromServer(sMsgPropCreateEntity_SC_Monster.MonsterX, sMsgPropCreateEntity_SC_Monster.MonsterY);
        GameObject monsterPrefab = MapResManager.Instance.GetMapMonsterPrefab(monsterId);
		float angle = sMsgPropCreateEntity_SC_Monster.MonsterUnitValues.UNIT_FIELD_DIR / 1000f + 90;
		var monster = (GameObject)GameObject.Instantiate(monsterPrefab, pos, Quaternion.Euler(0, angle , 0));
        #region edit by lee
          
        var monsterBehaviour = monster.GetComponent<MonsterBehaviour>();
		
        if (monsterBehaviour == null)
        {
            Debug.LogWarning("怪物" + monsterId.ToString() + "未挂载脚本");
            monsterBehaviour = monster.AddComponent<MonsterBehaviour>();
        }			

		//cache hurt point
		monsterBehaviour.CacheHurtPoint();            
        //怪物生成特效
        if (monsterData._bornEffects != "0")
        {
            GameObject bornEffectPrefab = MapResManager.Instance.GetMapEffectPrefab(monsterData._bornEffects);
            if (bornEffectPrefab != null)
            {
                GameObject bornEffect = GameObjectPool.Instance.AcquireLocal(bornEffectPrefab, Vector3.zero, bornEffectPrefab.transform.rotation);
                bornEffect.AddComponent<DestroySelf>();
                bornEffect.transform.position = monster.transform.position;
            }
        }        
        //怪物对白
        if (false == BattleManager.Instance.IsHeroFirstDead)
        {
//            if (monsterData._dialogPortrait != "0")
//            {
//                var resData = MapResManager.Instance.GetMapEffectPrefab(monsterData._dialogPortrait);
//                if (resData != null)
//                {
//                    GameObject IconPrefab = CreatObjectToNGUI.InstantiateObj(MonsterDialogPrefab, BattleUIManager.Instance.GetScreenTransform(ScreenPositionType.Center));
//                    IconPrefab.transform.localPosition = new Vector3(-240, 160, 10);//避免和任务指引面板重叠
//                    SirenDialogEctypeBehaviour sirenDialogEctypeBehaviour = IconPrefab.GetComponent<SirenDialogEctypeBehaviour>();
//
//                    sirenDialogEctypeBehaviour.Init(resData, LanguageTextManager.GetString(monsterData._bornDialogue), LanguageTextManager.GetString(monsterData._dialogMonsterName));
//                    if (monsterData._bornSound != "0")
//                    {
//                        SoundManager.Instance.PlaySoundEffect(monsterData._bornSound);
//                    }
//                    IconPrefab.AddComponent<DestroySelf>();
//                }
//            }
			if(monsterData._BornDialogueFulls[0].Portrait != "0")
			{
				GameObject IconPrefab = CreatObjectToNGUI.InstantiateObj(MonsterDialogPrefab, BattleUIManager.Instance.GetScreenTransform(ScreenPositionType.Center));
				SirenDialogEctypeBehaviour sirenDialogEctypeBehaviour = IconPrefab.GetComponent<SirenDialogEctypeBehaviour>();
				sirenDialogEctypeBehaviour.Init(MonsterDialogPrefab, monsterData._BornDialogueFulls);
				if (monsterData._bornSound != "0")
				{
					SoundManager.Instance.PlaySoundEffect(monsterData._bornSound);
				}
			}
        }        

        //特殊镜头
        if (monsterData._cameraFix_pos != Vector3.zero)
        {            
            //BattleManager.Instance.FollowCamera.SetFixed(monsterData._cameraFix_pos, monsterData._cameraFix_time);
            BattleManager.Instance.FollowCamera.BeginMoveToPosAndGoBack(monsterData._cameraFix_pos, monsterData._cameraFix_time, monsterData._cameraStay_time, monsterData._cameraBack_time, monsterData._blockPlayerToIdle);
        }

        #endregion                


        //TypeID type;
        //EntityModel monsterDataModel = monsterBehaviour.EntityModel = EntityController.Instance.GetEntityModel( entityDataStruct.SMsg_Header.uidEntity, out type);
        //monsterBehaviour.EntityModel.GO = monsterBehaviour.gameObject;
        //monsterBehaviour.EntityModel.Behaviour = monsterBehaviour;


		monsterDataModel.GO = monster;
        monsterDataModel.Behaviour = monsterBehaviour;
        //monsterDataModel.EntityDataStruct = sMsgPropCreateEntity_SC_Monster;
		
		monsterBehaviour.EntityModel = monsterDataModel;
		monsterBehaviour.SetMonsterConfigData(monsterData);  
		
        monsterBehaviour.InitFSM();
        //MonsterManager.Instance.Init();

		//出生是否隐藏
		if(sMsgPropCreateEntity_SC_Monster.MonsterValues.MONSTER_FIELD_ISSHOW == 0)
		{
			SetCildLayer(monsterDataModel.GO.transform,21);//21=hide
		}

		//血条
		Transform bloodBarMP;
		monster.transform.RecursiveFindObject("BloodBarMP", out bloodBarMP);
		if (null != bloodBarMP)
		{                       
			GameObject bloodBarPrefab = null;
			switch (sMsgPropCreateEntity_SC_Monster.MonsterValues.MONSTER_FIELD_TYPE)
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
			case 3:
				bloodBarPrefab = MonsterFactory.Instance.BloobBarPrefab_normal;
				break;
			default:
				break;
			}
			if (bloodBarPrefab != null)
			{
				BloodBarManager.Instance.AttachBarToTarget(sMsgPropCreateEntity_SC_Monster.SMsg_Header.uidEntity, bloodBarMP, bloodBarPrefab);
			}            
		}
		//发UI事件，参数 EntityModel
		UIEventManager.Instance.TriggerUIEvent(UIEventType.CreatMoster, monsterData);

		//如果是boss，激活boss状态栏
		if (MonsterManager.Instance.IsMonsterBossType(monsterId))
		{
			if(DefencePanel.Instance.ISDefenceEctype)
			{
				DefencePanel.Instance.DefenceBossCreate(monsterDataModel);
			}
			else
			{
				if (!MonsterManager.Instance.IsBossStatusPanelInit())
	            {
	                GameObject bossStatusPanel = (GameObject)Instantiate(BossStatusPanelPrefab);
	                //bossStatusPanel.transform.parent = PopupObjManager.Instance.UICamera.transform;
	                bossStatusPanel.transform.parent = UI.Battle.BattleUIManager.Instance.GetScreenTransform(UI.Battle.ScreenPositionType.TopRight);
	                bossStatusPanel.transform.localPosition = Vector3.zero;
	                bossStatusPanel.transform.localScale = Vector3.one;
	                //赋值
					BossStatusPanel_V3 bossStatusPanelScript = bossStatusPanel.GetComponent<BossStatusPanel_V3>();
	                MonsterManager.Instance.SetBossStatusPanel(bossStatusPanelScript);

	                bossStatusPanelScript.SetBloodNum(monsterDataModel);
	            }
	            else
	            {
					//第二个boss
					GameObject secondBossStatusPanel = (GameObject)Instantiate(BattleResManager.Instance.SecondBossStatusPanelPrefab);
					secondBossStatusPanel.transform.parent = UI.Battle.BattleUIManager.Instance.GetScreenTransform(UI.Battle.ScreenPositionType.TopRight);
					secondBossStatusPanel.transform.localPosition = Vector3.zero;
					secondBossStatusPanel.transform.localScale = Vector3.one;
					BossStatusPanel_V3 secondBossPanelScript = secondBossStatusPanel.GetComponent<BossStatusPanel_V3>();
					secondBossPanelScript.SetBloodNum(monsterDataModel);
					MonsterManager.Instance.IsDoubleBoss = true;
					//BossStatusPanel_V3 bossStatusPanelScript = MonsterManager.Instance.GetBossStatusPanel();
	                //bossStatusPanelScript.SetDataModel(monsterDataModel);
	            }
	            //bossStatusPanelScript.SetBloodNum();
			}
			//如果是普通副本，boss出场，播特效//
			SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
			EctypeContainerData m_ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
			if(m_ectypeData.lEctypeType == 0 || m_ectypeData.lEctypeType == 9)
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.OnBossShowEvent, null);
				SoundManager.Instance.PlaySoundEffect("Sound_UIEff_BossAppear");
			}
        }
        else
        {
            //加入箭头挂载脚本
#if !UNITY_EDITOR
			if(sMsgPropCreateEntity_SC_Monster.MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY != 
			   PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHT_HOSTILITY)
			{
				if(monster.GetComponent<MonsterBehaviour>().m_MonsterConfigData._isShowGuideArrow == 1)
				{
					ArrowManager.Instance.AddMonsterArrowt(monster);
				}
			}        
#endif     
        }
		//jamfing//
		//if(DefencePanel.Instance.ISDefenceEctype)
		{
			DefencePanel.Instance.DefenceMonsterCreated(monsterDataModel,monsterData);
		}
        //EntityController.Instance.RegisteEntity(entityDataStruct.SMsg_Header.uidEntity, monsterDataModel);
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

    //延迟销毁
    public void DelayDestroy(EntityModel entityModel, float time)
    {
        StartCoroutine(DestroyGameObj(entityModel, time));
    }
    IEnumerator DestroyGameObj(EntityModel entityModel, float time)
    {
        yield return new WaitForSeconds(time);
        if (entityModel != null)
        {
            if (entityModel.GO != null)
            {
                entityModel.DestroyEntity();
            }            
        }        
    }
}
