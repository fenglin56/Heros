using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UI.MainUI;

public class EntityController : Controller, ISingletonLifeCycle
{
    private List<EntityInfo> m_entityInfos = new List<EntityInfo>();
    Dictionary<TypeID, IEntityManager> m_typeMapManager = new Dictionary<TypeID, IEntityManager>();
    private static EntityController m_instance;

    public static EntityController Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EntityController();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }

    public void AddEntity(Int64 uid, TypeID type)
    {
        var entityInfo = m_entityInfos.SingleOrDefault(P => P.Uid == uid);

        if (entityInfo == null)
        {
            entityInfo = new EntityInfo() { Uid = uid, EntityType = type };
            m_entityInfos.Add(entityInfo);
        } else
        {
            entityInfo.EntityType = type;
        }
    }

    public void RegisteManager(TypeID type, IEntityManager manager)
    {
        var map = m_typeMapManager.SingleOrDefault(P => P.Key == type);

        if (m_typeMapManager.ContainsKey(type))
        {
            m_typeMapManager [type] = manager;
        } else
        {
            m_typeMapManager.Add(type, manager);
        }
    }

    public void RegisteEntity(Int64 uid, EntityModel entityModel)
    {
        var entityInfo = m_entityInfos.SingleOrDefault(P => P.Uid == uid);
        if (entityInfo != null)
        {
            if (m_typeMapManager.ContainsKey(entityInfo.EntityType))
            {
                m_typeMapManager [entityInfo.EntityType].RegisteEntity(entityModel);
            }
        }
    }

    public EntityModel GetEntityModel(Int64 uid, out TypeID entityType)
    {
        EntityModel entityModel = null;
        entityType = TypeID.TYPEID_MONSTER;
        var entityInfo = m_entityInfos.SingleOrDefault(P => P.Uid == uid);
        if (entityInfo != null)
        {
            entityType = entityInfo.EntityType;
            if (m_typeMapManager.ContainsKey(entityInfo.EntityType))
            {
                entityModel = m_typeMapManager [entityInfo.EntityType].GetEntityMode(uid);
            }
        }

        return entityModel;
    }

	public EntityModel GetEntityModel(Int64 uid, TypeID typeID)
	{
		EntityModel entityModel = null;
		if (m_typeMapManager.ContainsKey(typeID))
		{
			entityModel = m_typeMapManager [typeID].GetEntityMode(uid);
		}
		return entityModel;
	}

    /// <summary>
    /// 更新实体的值
    /// </summary>
    /// <param name="uid">实体ID</param>
    /// <param name="index">索引</param>
    /// <param name="value">更新值</param>
    /// <param name="TypeID">返回值</param>
    public void UpdateEntityValue(Int64 uid, short index, int value, out TypeID entityType, out bool isHero)
    {
	
        entityType = TypeID.TYPEID_INVALID;
        isHero = false;
        var entityInfo = m_entityInfos.SingleOrDefault(P => P.Uid == uid);
        if (entityInfo != null)
        {
            if (m_typeMapManager.ContainsKey(entityInfo.EntityType))
            {
                var entityModel = m_typeMapManager [entityInfo.EntityType].GetEntityMode(uid);
                if (entityModel != null)
                {
                    short myIndex = EntityIndexReCalc.Calc(entityInfo.EntityType, index);
                    isHero = entityModel.EntityDataStruct.SMsg_Header.IsHero;
                    entityModel.EntityDataStruct.UpdateValue(myIndex, value);

                    if (entityInfo.EntityType == TypeID.TYPEID_PLAYER)
                    {
                        switch (index)  //这个index服务器端Index
                        {
                            case 2:  //玩家等级更新
								UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayerLevelUpdate, null);
								RaiseEvent(EventTypeEnum.PlayerLevelUpdate.ToString(), (INotifyArgs)entityModel.EntityDataStruct);
                                break;
							case CommonMsgDefineManager.SERVER_SKIP_INDEX+2:  //Vip等级更新(播放升级特效)
                                if (entityModel.EntityDataStruct is SMsgPropCreateEntity_SC_MainPlayer)
                                {
                                    //RaiseEvent(EventTypeEnum.VipGradeUpdate.ToString(),  ((IPlayerDataStruct)entityModel.EntityDataStruct).GetCommonValue().PLAYER_FIELD_VISIBLE_VIP);
                                    GameManager.Instance.OnVipGradeUpdate(((IPlayerDataStruct)entityModel.EntityDataStruct).GetCommonValue().PLAYER_FIELD_VISIBLE_VIP);
                                }
                                break;
							case CommonMsgDefineManager.SERVER_SKIP_INDEX+4:  //时装更新
                                PlayerManager.Instance.ChangePlayerAvatar(uid, ((IPlayerDataStruct)entityModel.EntityDataStruct).GetCommonValue().PLAYER_FIELD_VISIBLE_FASHION);
								RaiseEvent(EventTypeEnum.PlayerFashionUpdate.ToString(), null);
                                break;
							case CommonMsgDefineManager.SERVER_SKIP_INDEX+5:  //称号更新
                                PlayerTitleManager.Instance.UpdateTitle(uid);
                                RaiseEvent(EventTypeEnum.PlayerTitleUpdate.ToString(), null);
                                break;
							case CommonMsgDefineManager.SERVER_SKIP_INDEX+9:  //铜币更新
								UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayerHoldMoneyUpdate, null);
                                RaiseEvent(EventTypeEnum.PlayerHoldMoneyUpdate.ToString(), (INotifyArgs)entityModel.EntityDataStruct);
								break;
							case CommonMsgDefineManager.SERVER_SKIP_INDEX+14:  //体力更新
								UIEventManager.Instance.TriggerUIEvent(UIEventType.ActiveLifeUpdate, null);
								break;
							case CommonMsgDefineManager.SERVER_SKIP_INDEX+27://修为值
								if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
								{
									if(SirenManager.Instance.IsHasSirenSatisfyIncrease())
									{
										UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Siren);
									}
								}
								break;
							case CommonMsgDefineManager.SERVER_SKIP_INDEX+42: //活跃值更新
                                NetServiceManager.Instance.EquipStrengthenService.SendRequestActiveChestProgressCommand();//获取奖励进度                               
                                break;
							case CommonMsgDefineManager.SERVER_SKIP_INDEX+46: //
                                UIEventManager.Instance.TriggerUIEvent(UIEventType.VipPaySuccess, null);
                                break;
							case CommonMsgDefineManager.SERVER_SKIP_INDEX+55: //剩余副本扫荡次数.原
								UIEventManager.Instance.TriggerUIEvent(UIEventType.SweepTimesUpdate, null);
								break;
						case CommonMsgDefineManager.SERVER_SKIP_INDEX + 101:
							UIEventManager.Instance.TriggerUIEvent(UIEventType.PVPHonorUpdate, value);
							break;
						case CommonMsgDefineManager.SERVER_SKIP_INDEX+102:	//更新荣誉
							UIEventManager.Instance.TriggerUIEvent(UIEventType.PVPHonorUpdate, value);
							break;
						case CommonMsgDefineManager.SERVER_SKIP_INDEX + 103://更新贡献
							UIEventManager.Instance.TriggerUIEvent(UIEventType.PVPContributeUdate, value);
							break;
                        //case 15:
                        //  TraceUtil.Log("更新Index 15:" + value);
                        //break;
                        }
                    }

					if(entityInfo.EntityType == TypeID.TYPEID_MONSTER)
					{
						switch(index)
						{
							case CommonMsgDefineManager.SERVER_SKIP_INDEX+6:	//怪物隐藏属性更新
								MonsterManager.Instance.SetMonsterVisible(uid, value);
								break;
						}
					}

                     

                    entityType = (TypeID)entityModel.EntityDataStruct.SMsg_Header.nEntityClass;                    


                } else
                {
                    //TraceUtil.Log("找不到实体模型:" + entityInfo.EntityType + "   " + uid);
                }
            } else
            {
                //TraceUtil.Log("找不到实体类型:" + entityInfo.EntityType);
            }
        } else
        {
            //TraceUtil.Log("找不到实体:" + uid);
        }
    }
    /// <summary>
    /// 销毁实体处理
    /// </summary>
    /// <param name="uid"></param>
    public void UnRegisteEntity(Int64 uid)
    {
        var entityInfo = m_entityInfos.SingleOrDefault(P => P.Uid == uid);
        if (entityInfo != null)
        {
            if (m_typeMapManager.ContainsKey(entityInfo.EntityType))
            {
                m_typeMapManager [entityInfo.EntityType].UnRegisteEntity(uid);
            }
            m_entityInfos.Remove(entityInfo);
        }
    }

    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.S_CSMsgFightFightToResult.ToString(), ReceiveFightResultHandle);
    }

    void ReceiveFightResultHandle(INotifyArgs notifyArgs)
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.GAME_STATE_BATTLE)
            return;
        SMsgBattleCalculateEffect_SC fightResult = (SMsgBattleCalculateEffect_SC)notifyArgs;
		bool isHero = fightResult.uidFighter == PlayerManager.Instance.FindHeroDataModel().UID;
        var entityInfo = m_entityInfos.SingleOrDefault(P => P.Uid == fightResult.uidFighter);
        if (entityInfo != null)
        {
            if (m_typeMapManager.ContainsKey(entityInfo.EntityType))
            {
                var entityModel = m_typeMapManager [entityInfo.EntityType].GetEntityMode(fightResult.uidFighter);
                TypeID bulletOwnerEntityType;
                
                
                if (entityModel != null)
                {
                    string displayContent = string.Empty;
                    var fightEffectType = (FightEffectType)fightResult.EffectType;
                    switch (fightEffectType)
                    {
                        case FightEffectType.BATTLE_EFFECT_CRIT:   //暴击
                            //TraceUtil.Log("CritHit");
                            //displayContent = "-" + fightResult.Value.ToString() ;
                            displayContent = fightResult.Value.ToString();
                            if (GameManager.Instance.CurrentGameMode == GameMode.MULTI_PLAYER)
                            {
                                ShowHurtEffect(entityModel, fightResult.BulletTemplateID, fightResult.uidEffectParam);
                                EntityModel bulletOwnerEntityModel = GetEntityModel(fightResult.uidEffectParam, out bulletOwnerEntityType);
                                if (bulletOwnerEntityModel != null)
                                {
                                    
                                    ShowHurtUiEffect(bulletOwnerEntityModel, fightResult.BulletTemplateID);
                                }
                            }
                            break;
                        case FightEffectType.BATTLE_EFFECT_DODGE:  //闪避 Miss
                            displayContent = LanguageTextManager.GetString("IDS_D2_15");
                            break;
                        case FightEffectType.BATTLE_EFFECT_HIT:  //命中
                            //displayContent = LanguageTextManager.GetString("IDS_D2_16") + " -" + fightResult.Value.ToString() + "HP";
                            displayContent = fightResult.Value.ToString();
                            //命中效果
                            if (GameManager.Instance.CurrentGameMode == GameMode.MULTI_PLAYER)
                            {
                                ShowHurtEffect(entityModel, fightResult.BulletTemplateID, fightResult.uidEffectParam);
                                EntityModel bulletOwnerEntityModel = GetEntityModel(fightResult.uidEffectParam, out bulletOwnerEntityType);
                                if (bulletOwnerEntityModel != null)
                                {
                                    
                                    ShowHurtUiEffect(bulletOwnerEntityModel, fightResult.BulletTemplateID);
                                }
                            }
							if(!GameManager.Instance.m_gameSettings.ShowHurtNum)
							{
								return;
							}
						break;
                        case FightEffectType.BATTLE_EFFECT_HP:  //HP扣血
                            displayContent = "-" + fightResult.Value;
                            return;
                        case FightEffectType.BATTLE_ADDHP:  //加HP
                            displayContent = "+" + fightResult.Value;
                            break;
                        case FightEffectType.BATTLE_ADDMP:  //加MP
                            if (fightResult.Value > 0)
                            {
                                displayContent = "+" + fightResult.Value;
                            } else
                            {
                                displayContent = fightResult.Value.ToString();
                            }
                            break;
                        case FightEffectType.BATTLE_ADDMONEY:  //加铜钱
                            SoundManager.Instance.PlaySoundEffect("Sound_Button_Pay");                            
                            displayContent = fightResult.Value.ToString();
                            UI.Battle.BattleMessangeManager.Instance.Show(displayContent, LanguageTextManager.GetString("IDS_D2_17"));
                            break;
                        case FightEffectType.BATTLE_EFFECT_ROLLPOINT:   //Roll点
                            //当前还不需要显示Roll点
                            GameManager.Instance.DamageFactory.PlayDiceAnimation();
                            return;
                        case FightEffectType.BATTLE_EFFECT_EXPSHOW:   //怪物死亡经验显示  
                            TraceUtil.Log("获得经验显示");
                            //displayContent = string.Format(LanguageTextManager.GetString("IDS_H1_395"), fightResult.Value);
                            displayContent = fightResult.Value.ToString();                            
                            break;
                        case FightEffectType.BATTLE_EFFECT_GOODSSHOW:   //获得物品显示                           
                            DamageManager.Instance.AllocationToPlayer(fightResult.uidFighter, fightResult.uidEffectParam, fightResult.Value);                         
							//DamageManager.Instance.AllocationEquip(fightResult.uidFighter, fightResult.uidBeFighted, fightResult.Value);
							return;
                        case FightEffectType.BATTLE_EFFECT_XIUWEI://获得修为显示
                            displayContent = fightResult.Value.ToString();
                            TraceUtil.Log("获得修为显示");
                            break;
                        case FightEffectType.BATTLE_EFFECT_SHILIAN_XIUWEI:
                            TraceUtil.Log("获得试炼副本修为显示");
                            //var m_hostPosition = entityModel.GO.transform.position + UnityEngine.Vector3.up * 18;
                            //displayContent = string.Format(LanguageTextManager.GetString("IDS_H1_471"), fightResult.Value);
                            displayContent = fightResult.Value.ToString();
                            PopupTextController.SettleResultForTime(entityModel, displayContent, fightEffectType);
                            return;
                        case FightEffectType.BATTLE_EFFECT_SHILIAN_EXPSHOW:
                            TraceUtil.Log("获得试炼副本经验值显示");
                            //displayContent = string.Format(LanguageTextManager.GetString("IDS_H1_395"), fightResult.Value);
                            displayContent = fightResult.Value.ToString();
                            break;
                            
                    }
                    //var hostPosition = entityModel.GO.transform.position + new Vector3(CommonDefineManager.Instance.CommonDefine.HitNumber_VectorX, CommonDefineManager.Instance.CommonDefine.HitNumber_VectorY, CommonDefineManager.Instance.CommonDefine.HitNumber_VectorZ);
                    var hostPosition = getUIPosition(entityModel);
					PopupTextController.SettleResult(hostPosition, displayContent, fightEffectType, isHero);
                }
            }
        }
    }

    Vector3 getUIPosition(EntityModel entityModel)
    {
        int index = UnityEngine.Random.Range(0, CommonDefineManager.Instance.CommonDefine.HitNumberPos_VectorX.Length);
        return entityModel.GO.transform.position + new Vector3(CommonDefineManager.Instance.CommonDefine.HitNumberPos_VectorX [index], CommonDefineManager.Instance.CommonDefine.HitNumberPos_VectorY [index], CommonDefineManager.Instance.CommonDefine.HitNumberPos_VectorZ [index]);
    }

    public void ShowHurtEffect(EntityModel entityModel, uint bulletId, Int64 ownerUID)
    {
		if(entityModel==null)
		{
			return;
		}
        var bulletData = SkillDataManager.Instance.GetBulletData((int)bulletId);
        TypeID type;
        EntityModel ownerEntityModel = EntityController.Instance.GetEntityModel(ownerUID, out type);
        if(type != TypeID.TYPEID_PLAYER || ownerEntityModel.EntityDataStruct.SMsg_Header.IsHero)
        {
            BattleManager.Instance.TryShakeCamera(bulletData);
        }
        RoleBehaviour roleBehavior = entityModel.GO.GetComponent<RoleBehaviour>();
        
        
        if (null != roleBehavior && null != roleBehavior.HurtPoint)
        {
            if (bulletData != null)
            {
                if ("0" != bulletData.m_hurtEffectPath)
                {
                    if(BattleManager.Instance != null && BattleManager.Instance.CanShowHurtEffect())
                    {
                        BattleManager.Instance.OnHurtEffectCreate();
                        GameObject hurtPrefab = MapResManager.Instance.GetMapEffectPrefab(bulletData.m_hurtEffectPath);
                        GameObjectPool.Instance.AcquireLocal(hurtPrefab, roleBehavior.HurtPoint.position
                                                             , bulletData.m_hurtEffectRotationFlag == 0 ? Quaternion.identity : Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0));
                    }


                    //bulletData.m_sfx_id;音效
                    //bulletData.m_hurt_flash;闪光
                    //bulletData.m_hurt_shake;震屏
                }
                if (bulletData.m_hurtFlash > 0)
                {
                    float hurtDuration = (float)(bulletData.m_hurtFlash) / 1000.0f;
                    roleBehavior.ShowHurtFlash(true, hurtDuration); 
                }
                if ("0" != bulletData.m_sfx_id)
                {
                    SoundManager.Instance.PlaySoundEffect(bulletData.m_sfx_id); 
                }
                
            } else
            {
                TraceUtil.Log(SystemModel.Common, TraceLevel.Error, "没打到子弹数据");
            }
            
        } else
        {
            TraceUtil.Log(SystemModel.Common, TraceLevel.Error, "没打到受击点");
        }
    }
    
    public void ShowHurtUiEffect(EntityModel entityModel, uint bulletId)
    {
		if(entityModel==null)
		{
			return;
		}

        BulletData bulletData = SkillDataManager.Instance.GetBulletData((int)bulletId);
        RoleBehaviour roleBehavior = entityModel.GO.GetComponent<RoleBehaviour>();
        
        
        if (null != roleBehavior && null != roleBehavior.HurtPoint)
        {
            if (bulletData != null)
            {
                if (null != bulletData.m_hurt_Ui_Effect)
                {
                    GameObject uiEffectObj = GameObjectPool.Instance.AcquireLocal(bulletData.m_hurt_Ui_Effect, Vector3.zero, Quaternion.identity);
                    uiEffectObj.transform.parent = PopupObjManager.Instance.UICamera.transform;
                    Vector3 uiPosition = PopupTextController.GetPopupPos(roleBehavior.HurtPoint.position, PopupObjManager.Instance.UICamera);
                    uiEffectObj.transform.position = uiPosition;
                }
            } else
            {
                TraceUtil.Log(SystemModel.Common, TraceLevel.Error, "没子弹数据");
            }
            
        } else
        {
            TraceUtil.Log(SystemModel.Common, TraceLevel.Error, "没受击点");
        }
        
    }

    public void Instantiate()
    {

    }

    public void LifeOver()
    {
        this.ClearEvent();
        m_instance = null;
    }
}

public class EntityInfo
{
    public Int64 Uid { get; set; }

    public TypeID EntityType { get; set; }
}

