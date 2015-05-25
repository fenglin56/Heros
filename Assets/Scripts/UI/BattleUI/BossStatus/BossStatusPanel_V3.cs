using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UI;

public class BossStatusPanel_V3 :  View 
{
	//new 
	public UISprite[] UI_Bloods = new UISprite[7];//最大为7

	public UISprite ShardProgressBar;
	public UISprite ShardShadowProgressBar;
	public GameObject ShardProgressEffect;
	public Transform ShardProgressEffectPoint;
	public UISprite ShardBarFrame;
	public Transform BossHeadPoint;
	public UILabel Label_BloodNum;
	
	public Transform Eff_BossProtection_Point;
	public GameObject Eff_BossProtection_Prefab;
	public GameObject Eff_BreakDefense_Prefab;

	public GameObject BreakDefenseTip;

	public float BreakDefenseTipAppearDelay = 1f;

	private int m_BloodsNum = 1;    //血条数，默认1
	private float m_AbatementTime = 1f;//血值变化动画时间
	private float m_OneBloodValue; //每条血槽的值
	
	private int m_previousHealth = 0;	//受伤害前生命值
	private GameObject m_shardProgressEffect;

	private Dictionary<Int64, EntityModel> m_BossModelDic = new Dictionary<long, EntityModel>();
	
	private int m_hurtTime;
	
	private bool m_isInit = false;
	
	private bool m_isDead = false;
	
	private int m_previousshardValue = 0;//上次更新的防护值
	private GameObject m_shardTweenGO = null;
	private float m_monsterBreakTime = 0;//当前boss防护值恢复速度

	private Int64 m_BossUID;//1.4新增 boss血条现在唯一对应

	/// <summary>
	/// 测试用代码
	/// </summary>
	protected bool IsShowStatusLabel = false;
	public UILabel BossBloodLabel;
	public UILabel ShardNumLabel;
	private float m_maxBloodValue;
	//测试代码结束
	
	void Awake()
	{
		IsShowStatusLabel = GameManager.Instance.IsShowBloodLabel;
		RegisterEventHandler();
		//UpdateViaNotify()
	}
	public bool IsInit
	{
		get{
			return m_isInit;
		}
	}
	public void SetBloodNum(EntityModel bossModel)
	{
		if (!m_isInit)
		{
			m_isInit = true;
			
			m_BossModelDic.Clear();
			m_BossModelDic.Add(bossModel.EntityDataStruct.SMsg_Header.uidEntity, bossModel);

			m_BossUID = bossModel.EntityDataStruct.SMsg_Header.uidEntity;
			int bossID = ((SMsgPropCreateEntity_SC_Monster)bossModel.EntityDataStruct).BaseObjectValues.OBJECT_FIELD_ENTRY_ID;

			//m_BossModel = bossModel;
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
			}
			else
			{
				return;
			}
			//BossHeadSprite.spriteName = ectypeData.BossHead; //boss头像
			//头像替换为prefab
			EctypeContainerResData ectypeResData = EctypeResDataManager.Instance.GetEctypeContainerResData(ectypeData.lEctypeContainerID);
			EctypeContainerBossHeadRes headRes = ectypeResData.BossHeadReses.SingleOrDefault(p=>p.BossHeadID == bossID);
			if (ectypeResData != null && headRes != null)
			{            
				Vector3 scale = headRes.BossHeadGO.transform.localScale;
				GameObject headObj = (GameObject)Instantiate(headRes.BossHeadGO);
				headObj.transform.parent = BossHeadPoint;
				headObj.transform.localPosition = Vector3.zero;
				headObj.transform.localScale = scale;                
			}
			
			int bossLifeLayer = ectypeData.BossLifeLayer;
			int MaxBloodValue = 0;
			var monsterData = BattleConfigManager.Instance.GetMonsterData(bossID);
			if(monsterData!=null)
			{
				MaxBloodValue = monsterData._maxHP;
			}
//			ectypeData.BossIDs.ApplyAllItem(p=>
//			                                {
//				var monsterData = BattleConfigManager.Instance.GetMonsterData(p);
//				if (monsterData != null)
//				{
//					MaxBloodValue += monsterData._maxHP;
//				}
//			});
			m_BloodsNum = bossLifeLayer;
			int allNum = UI_Bloods.Length;
			
			this.m_maxBloodValue = MaxBloodValue;
			m_OneBloodValue = MaxBloodValue * 1f / bossLifeLayer;


			//new 
			m_previousHealth = MaxBloodValue;


			if (bossLifeLayer >= allNum)
				return;
			
			for (int i = bossLifeLayer; i < allNum; i++)
			{
				UI_Bloods[i].enabled = false;
			}
			
			this.UpdateBloodValue();
			//TraceUtil.Log("SetBloodNum(EntityModel bossModel,int bossLifeLayer, int MaxBloodValue)");
		}
		//TraceUtil.Log("当前血槽序号 : " + m_currentBloodNo);
		
	}
	public void SetDataModel(EntityModel bossModel)
	{
		//m_BossModel = bossModel;
		if (m_BossModelDic.ContainsKey(bossModel.EntityDataStruct.SMsg_Header.uidEntity))
		{
			m_BossModelDic[bossModel.EntityDataStruct.SMsg_Header.uidEntity] = bossModel;
		}
		else
		{
			m_BossModelDic.Add(bossModel.EntityDataStruct.SMsg_Header.uidEntity, bossModel);
		}
		
		this.UpdateBloodValue();
	}
	

	private void UpdateBloodValue()
	{
		int bloodValue = 0;
		m_BossModelDic.ApplyAllItem(p =>
		                            {
			bloodValue += ((SMsgPropCreateEntity_SC_Monster)p.Value.EntityDataStruct).MonsterUnitValues.UNIT_FIELD_CURHP;
		});
//		int endNum = (int)(bloodValue / m_OneBloodValue);   //血槽最后显示的条数
//
//		UI_Bloods.ApplyAllItem(p=>{
//			p.fillAmount = 1f;
//		});
//		int allNum = UI_Bloods.Length;
//		if (endNum > allNum - 1)
//		{
//			endNum = allNum - 1;
//		}
//		for (int i = 0; i < endNum + 1; i++)
//		{
//			UI_Bloods[i].enabled = true;
//		}
//		for (int i = endNum + 1; i < m_BloodsNum; i++)
//		{
//			UI_Bloods[i].enabled = false;
//		}
		if (IsShowStatusLabel)
		{
			BossBloodLabel.text = string.Format("{0}/{1}", bloodValue, m_maxBloodValue);
		}
		else
		{
			BossBloodLabel.text = "";
		}

		StartCoroutine(FileBloods(bloodValue, bloodValue));
		m_previousHealth = bloodValue;
	}
	
	public void PlayCutBloodAnimation(Int64 monsterUID)
	{
		//if (m_BossModelDic.ContainsKey(monsterUID))
		if(m_BossUID == monsterUID)
		{
			int currentHp = 0;
			m_BossModelDic.ApplyAllItem(p =>
			                            {
				currentHp += ((SMsgPropCreateEntity_SC_Monster)p.Value.EntityDataStruct).MonsterUnitValues.UNIT_FIELD_CURHP;
			});
			//TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"==>m_BossModelDic.count = " + m_BossModelDic.Count);
			if(m_previousHealth != currentHp)
			{
				StopAllCoroutines();
				StartCoroutine(FileBloods(m_previousHealth, currentHp));
				m_previousHealth = currentHp;
			}
			if (currentHp <= 0)
			{
				RaiseEvent(EventTypeEnum.CloseRelivePanel.ToString(), null);
				StartCoroutine(DestroyBossStatusPanel());
			}
			if (IsShowStatusLabel)
			{
				BossBloodLabel.text = string.Format("{0}/{1}", currentHp, m_maxBloodValue);
			}
		}      
	}


	IEnumerator FileBloods(int curHpValue, int toHpValue)
	{
		float i = 0;
		float rate = 1f/m_AbatementTime;
		float hpValue = 0;
		int index = 0;
		while(i < 1f)
		{
			i+=rate*0.02f;
			//生命值显示
			hpValue = Mathf.Lerp(curHpValue, toHpValue, i);
			index =Mathf.CeilToInt( hpValue/m_OneBloodValue);
			Label_BloodNum.text = index.ToString();
			for(int j=0;j<m_BloodsNum;j++)
			{
				if(j+1>index)
				{
					UI_Bloods[j].fillAmount = 0;
				}
				else if(j+1 == index)
				{
					float remainingHp = hpValue - j*m_OneBloodValue;
					float fillAmount = remainingHp/m_OneBloodValue;
					UI_Bloods[j].fillAmount = fillAmount;
				}
			}
			yield return null;
		}
	}
	

	IEnumerator DestroyBossStatusPanel()
	{
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
	
	private bool m_isFlag = true;
	public void UpdateViaNotify(object inotifyArgs)//设置各种属性
	{
		EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
		//if (entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_MONSTER)
		if(m_BossUID == entityDataUpdateNotify.EntityUID)
		{
			SMsgPropCreateEntity_SC_Monster monster = (SMsgPropCreateEntity_SC_Monster)MonsterManager.Instance.GetEntityMode(entityDataUpdateNotify.EntityUID).EntityDataStruct;            
			PlayCutBloodAnimation(monster.UID);//更新血量

			//更新防护值
			if (MonsterManager.Instance.IsMonsterBossType(monster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID))
			{
				if(ShardProgressBar==null)
				{
					//防守副本防甲为空
					return;
				}
				MonsterConfigData monsterConfig = BattleConfigManager.Instance.MonsterConfigList[monster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID];
				int bossShard = monster.MonsterUnitValues.UNIT_FIELD_SHARD;
				float shardValue = 0;
				if(monsterConfig.m_shieldpoint == 0)
				{
					shardValue = 0;
				}
				else
				{
					shardValue = bossShard / monsterConfig.m_shieldpoint;
				}			
				if (shardValue <= 0)
				{
					if(monsterConfig.m_shieldpoint>0)//防护值最大值为0的怪没有防护
					{
						//StartCoroutine("BreakDefenseTipAppear",BreakDefenseTipAppearDelay);
						//PlayerShardEffect(monster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID);
						MonsterManager.Instance.GetEntityMode(entityDataUpdateNotify.EntityUID).GO.GetComponent<HurtFlash>().OnDisrupt(true);                 	
					}
				}
				else
				{
					MonsterManager.Instance.GetEntityMode(entityDataUpdateNotify.EntityUID).GO.GetComponent<HurtFlash>().OnDisrupt(false);
//					ShardProgressBar.enabled = true;
//					ShardBarFrame.enabled = true;
//					m_isFlag = true;
				}

				//=====显示防护值 如果是多boss，显示并仅显示一个存活boss的防护值
				SMsgPropCreateEntity_SC_Monster showMonster = monster;
				if(m_BossModelDic.Count > 1)
				{
					var bossModelArray = m_BossModelDic.Values.ToArray();
					for(int i=0;i<bossModelArray.Length;i++)
					{
						showMonster = (SMsgPropCreateEntity_SC_Monster)bossModelArray[i].EntityDataStruct;
						if(showMonster.MonsterUnitValues.UNIT_FIELD_CURHP > 0)
						{
							break;
						}
					}
				}
				MonsterConfigData showMonsterConfig = BattleConfigManager.Instance.MonsterConfigList[showMonster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID];
				int showBossShard = showMonster.MonsterUnitValues.UNIT_FIELD_SHARD;
				float showShardValue = 0;
				if(showMonsterConfig.m_shieldpoint == 0)
				{
					showShardValue = 0;
				}
				else
				{
					showShardValue = showBossShard / showMonsterConfig.m_shieldpoint;
				}	

				//防护值恢复特效
				if(m_previousshardValue == 0 && showBossShard >= showMonsterConfig.m_shieldpoint)
				{
					if(showMonsterConfig.m_shieldpoint > 0)
					{
						StopCoroutine("BreakDefenseTipAppear");
						BreakDefenseTip.SetActive(false);
						PlayRecoverProtectionEff();
					}
				}
				m_previousshardValue = showBossShard;

				//防护值减少动画
				TweenFloat.Begin(0.3f, ShardProgressBar.fillAmount, showShardValue, ChangeShardProgressBar);
				if (IsShowStatusLabel)
				{
					ShardNumLabel.text = string.Format("{0}/{1}", showBossShard, BattleConfigManager.Instance.MonsterConfigList[showMonster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID].m_shieldpoint);
				}
				else
				{
					ShardNumLabel.text = "";
				}

				if (showShardValue <= 0)
				{
					if(showMonsterConfig.m_shieldpoint>0)//防护值最大值为0的怪没有防护
					{
						if(!MonsterManager.Instance.IsDoubleBoss)//双boss血条情况下不显示 可击倒提示
						{
							StartCoroutine("BreakDefenseTipAppear",BreakDefenseTipAppearDelay);
						}
						PlayerShardEffect(monster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID);
					}
				}
				else
				{
					ShardProgressBar.enabled = true;
					ShardBarFrame.enabled = true;
					m_isFlag = true;
				}
			}
		}
	}

	IEnumerator BreakDefenseTipAppear(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		BreakDefenseTip.SetActive(true);
	}

	void ChangeShardProgressBar(float value)
	{
		ShardProgressBar.fillAmount = value;
	}
	void ReadyToRecoverShardValue(float value)
	{
	}
	void ReadyToRecoverShardValueOver(object obj)
	{
		m_shardTweenGO = TweenFloat.Begin(m_monsterBreakTime,0, 1f, RecoverShardValue);
	}
	void RecoverShardValue(float value)
	{
		ShardShadowProgressBar.fillAmount = value;
	}
	private void PlayRecoverProtectionEff()
	{
		GameObject eff = UI.CreatObjectToNGUI.InstantiateObj( Eff_BossProtection_Prefab,Eff_BossProtection_Point);
		eff.AddComponent<DestroySelf>();
		if(m_shardTweenGO!=null)
		{
			Destroy(m_shardTweenGO);
		}
		ShardShadowProgressBar.fillAmount = 0;
	}
	
	private void PlayerShardEffect(int monsterID)
	{
		if (!m_isFlag)
		{
			return;
		}
		
		if (m_shardProgressEffect != null)
		{
			DestroyImmediate(m_shardProgressEffect);
		}
		
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DefenseBurst");
		
		//恢复效果
		var monsterData = BattleConfigManager.Instance.GetMonsterData(monsterID);
		m_shardTweenGO = TweenFloat.Begin(CommonDefineManager.Instance.CommonDefine.BossDef_WeakTime,0, 1f,ReadyToRecoverShardValue, ReadyToRecoverShardValueOver);
		m_monsterBreakTime = monsterData.m_breaktime/1000;
		TraceUtil.Log(SystemModel.Lee, TraceLevel.Verbose, (monsterData.m_breaktime+CommonDefineManager.Instance.CommonDefine.BossDef_WeakTime).ToString());
		//破防动画
		GameObject eff_defense = UI.CreatObjectToNGUI.InstantiateObj(Eff_BreakDefense_Prefab,PopupObjManager.Instance.UICamera.transform);	
		eff_defense.AddComponent<DestroySelf>();
		
		//发出Boss破防消息
		RaiseEvent(EventTypeEnum.BossBreakProtectForGuide.ToString(), null);
		
		//m_shardProgressEffect.transform.localPosition = new Vector3(-208, -49, -3);
		ShardProgressBar.enabled = false;
		ShardBarFrame.enabled = false;
		m_isFlag = false;
	}
	
	
	void OnDestroy()
	{
		RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
	}
	
	protected override void RegisterEventHandler()
	{
		AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);  //角色受击更新
	}
}
