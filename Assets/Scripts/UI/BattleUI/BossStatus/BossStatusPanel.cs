using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UI;

public class BossStatusPanel :  View 
{
    public GameObject[] Bloods;
    public UISlicedSprite Blood_bg;
    public GameObject BloodLight;
    public GameObject BloodLightFullPoint;
    public GameObject BloodLightEmptyPoint;
    public UISlicedSprite BossHeadSprite;
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

    private int m_BloodsNum = 1;    //血条数，默认1
    private float m_AbatementTime = 1f;//血值变化动画时间
    private Vector3 m_BloodScale;   //血条满血规格
    private float m_OneBloodValue; //每条血槽的值
    private int m_currentBloodNo; //当前血槽序号
    private float m_bloodLightLength;//光点长度
    private GameObject m_shardProgressEffect;

    //private EntityModel m_BossModel;
    private Dictionary<Int64, EntityModel> m_BossModelDic = new Dictionary<long, EntityModel>();

    private int m_hurtTime;

    private bool m_isInit = false;

    private bool m_isDead = false;

	private int m_previousshardValue = 0;//上次更新的防护值
	private GameObject m_shardTweenGO = null;
	private float m_monsterBreakTime = 0;//当前boss防护值恢复速度

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
            if (ectypeResData != null && ectypeResData.bossHead != null)
            {            
				Vector3 scale = ectypeResData.bossHead.transform.localScale;
                GameObject headObj = (GameObject)Instantiate(ectypeResData.bossHead);
                headObj.transform.parent = BossHeadPoint;
                headObj.transform.localPosition = Vector3.zero;
				headObj.transform.localScale = scale;                
            }

            int bossLifeLayer = ectypeData.BossLifeLayer;
            int MaxBloodValue = 0;
            ectypeData.BossIDs.ApplyAllItem(p=>
                {
                    var monsterData = BattleConfigManager.Instance.GetMonsterData(p);
                    if (monsterData != null)
                    {
                        MaxBloodValue += monsterData._maxHP;
                    }
                });
            m_BloodsNum = bossLifeLayer;
            m_BloodScale = Bloods[0].transform.localScale;
            int allNum = Bloods.Length;

            this.m_maxBloodValue = MaxBloodValue;
            m_OneBloodValue = MaxBloodValue * 1f / bossLifeLayer;
            m_currentBloodNo = bossLifeLayer - 1;
			Label_BloodNum.text = (m_currentBloodNo+1).ToString();
            m_bloodLightLength = Vector3.Distance(BloodLightFullPoint.transform.position, BloodLightEmptyPoint.transform.position);

            if (bossLifeLayer >= allNum)
                return;

            for (int i = bossLifeLayer; i < allNum; i++)
            {
                Bloods[i].SetActive(false);
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

    
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        StopAllCoroutines();
    //        StartCoroutine(ScaleBar(((SMsgPropCreateEntity_SC_Monster)m_BossModel.EntityDataStruct).MonsterUnitValues.UNIT_FIELD_CURHP));
    //    }
    //}
    private void UpdateBloodValue()
    {
        int bloodValue = 0;
        m_BossModelDic.ApplyAllItem(p =>
            {
                bloodValue += ((SMsgPropCreateEntity_SC_Monster)p.Value.EntityDataStruct).MonsterUnitValues.UNIT_FIELD_CURHP;
            });        
        int endNum = (int)(bloodValue / m_OneBloodValue);   //血槽最后显示的条数
        m_currentBloodNo = endNum;  //同步当前血槽序号*    
		Label_BloodNum.text = (m_currentBloodNo+1).ToString();
        Bloods.ApplyAllItem(p =>
            {
                p.transform.localScale = m_BloodScale;                
            });
        int allNum = Bloods.Length;
        if (endNum > allNum - 1)
        {
            endNum = allNum - 1;
        }
        for (int i = 0; i < endNum + 1; i++)
        {
            Bloods[i].SetActive(true);
        }
        for (int i = endNum + 1; i < m_BloodsNum; i++)
        {
            Bloods[i].SetActive(false);
        }
        if (IsShowStatusLabel)
        {
            BossBloodLabel.text = string.Format("{0}/{1}", bloodValue, m_maxBloodValue);
        }
        else
        {
            BossBloodLabel.text = "";
        }

        float thisBooldValue = bloodValue % m_OneBloodValue;
        float endLength = thisBooldValue * m_BloodScale.x / m_OneBloodValue;

        Bloods[m_currentBloodNo].transform.localScale = new Vector3(endLength, m_BloodScale.y, m_BloodScale.z);
        Blood_bg.transform.localScale = new Vector3(endLength, m_BloodScale.y, m_BloodScale.z);
        //光点
        //var lightShouldPos = Bloods[m_currentBloodNo].transform.localScale.x / m_BloodScale.x * m_bloodLightLength;
        //BloodLight.transform.position = BloodLightEmptyPoint.transform.position + new Vector3(lightShouldPos, 0, 0);
        
        //背景条深度
        Blood_bg.depth = 6 + m_currentBloodNo * 2;
        
    }

    public void PlayCutBloodAnimation(Int64 monsterUID)
    {
        if (m_BossModelDic.ContainsKey(monsterUID))
        {
            StopAllCoroutines();
            int currentHp = 0;
            m_BossModelDic.ApplyAllItem(p =>
                {
                    currentHp += ((SMsgPropCreateEntity_SC_Monster)p.Value.EntityDataStruct).MonsterUnitValues.UNIT_FIELD_CURHP;
                });
            //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"==>m_BossModelDic.count = " + m_BossModelDic.Count);
			if(m_maxBloodValue != currentHp)
			{
				StartCoroutine(ScaleBar(currentHp));
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
    
    IEnumerator ScaleBar( int bloodValue )
    {
        float i = 0;
        float rate = 1f / m_AbatementTime;

        //if (bloodValue < 1)
        //{
        //    bloodValue = 1;
        //}
        bool isChangeBlood = false;
        int endNum = (int)(bloodValue / m_OneBloodValue);   //血槽最后显示的条数

        int differNum = m_currentBloodNo - endNum;  //相差条数        

        if (differNum > 0) //
            isChangeBlood = true;

        float endvalue = bloodValue % m_OneBloodValue;
        float endLength = endvalue * m_BloodScale.x / m_OneBloodValue;
        float fistBloodEndLength = endLength - m_BloodScale.x * differNum;

        

        //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"endLength = " + endLength);
        //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"m_BloodScale = " + m_BloodScale);
        Vector3 startScale = Bloods[m_currentBloodNo].transform.localScale;

        while (i < 1f)
        {
            i += 0.02f * rate;
            //endValue/m_BloodsNum
            if (!isChangeBlood)//如果不用换血槽
            {
                //Bloods[m_currentBloodNo].transform.localScale = Vector3.Lerp(Bloods[m_currentBloodNo].transform.localScale, new Vector3(endLength, m_BloodScale.y, m_BloodScale.z), i);                
                Bloods[m_currentBloodNo].transform.localScale = new Vector3(endLength, m_BloodScale.y, m_BloodScale.z);
                //背景条
                Blood_bg.transform.localScale = Vector3.Lerp(startScale, new Vector3(endLength, m_BloodScale.y, m_BloodScale.z), i);  
                //光点
                //var lightShouldPos = Bloods[m_currentBloodNo].transform.localScale.x / m_BloodScale.x * m_bloodLightLength;                
                //BloodLight.transform.position = BloodLightEmptyPoint.transform.position + new Vector3(lightShouldPos, 0, 0);
                if (Mathf.Approximately(Bloods[m_currentBloodNo].transform.localScale.x, 0))
                {
                    Bloods[m_currentBloodNo].transform.localScale = new Vector3(0, m_BloodScale.y, m_BloodScale.z);//避免ngui警报
                    Bloods[m_currentBloodNo].SetActive(false);
                }
                
            }
            else
            {
                //Bloods[m_currentBloodNo].transform.localScale = Vector3.Lerp(Bloods[m_currentBloodNo].transform.localScale, new Vector3(fistBloodEndLength, m_BloodScale.y, m_BloodScale.z), i);
                Bloods[m_currentBloodNo].transform.localScale = new Vector3(fistBloodEndLength, m_BloodScale.y, m_BloodScale.z);
                //光点
                //var lightShouldPos = Bloods[m_currentBloodNo].transform.localScale.x / m_BloodScale.x * m_bloodLightLength;
                //BloodLight.transform.position = BloodLightEmptyPoint.transform.position + new Vector3(lightShouldPos, 0, 0);

                //背景条
                Blood_bg.transform.localScale = Vector3.Lerp(startScale, new Vector3(fistBloodEndLength, m_BloodScale.y, m_BloodScale.z), i);

                if (Bloods[m_currentBloodNo].transform.localScale.x <= 0)
                {
                    Bloods[m_currentBloodNo].transform.localScale = new Vector3(0, m_BloodScale.y, m_BloodScale.z);//避免ngui警报
                    Bloods[m_currentBloodNo].SetActive(false);                    
                    if (m_currentBloodNo > 0)
                    {
                        m_currentBloodNo--;
						Label_BloodNum.text = (m_currentBloodNo+1).ToString();
                        //背景条深度
                        Blood_bg.depth = 6 + m_currentBloodNo * 2;
                        //更新目前长度
                        startScale = Bloods[m_currentBloodNo].transform.localScale;
                    }
                    else
                    {
                        i = 1;
                        //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"error!");
                    }

                    differNum--;
                    if (differNum > 0)
                    {
                        fistBloodEndLength = endLength - m_BloodScale.x * differNum;
                    }
                    else
                    {
                        isChangeBlood = false;
                    }                    
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
        if (entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_MONSTER)
        {
            SMsgPropCreateEntity_SC_Monster monster = (SMsgPropCreateEntity_SC_Monster)MonsterManager.Instance.GetEntityMode(entityDataUpdateNotify.EntityUID).EntityDataStruct;            
			PlayCutBloodAnimation(monster.UID);//更新血量
			if (MonsterManager.Instance.IsMonsterBossType(monster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID))
            {
				if(ShardProgressBar==null)
				{
					//防守副本防甲为空
					return;
				}

                int bossShard = monster.MonsterUnitValues.UNIT_FIELD_SHARD;
                float shardValue = (float)bossShard / BattleConfigManager.Instance.MonsterConfigList[monster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID].m_shieldpoint;
                //ShardProgressBar.fillAmount = shardValue;

				//防护值恢复特效
				if(m_previousshardValue == 0 && bossShard >= BattleConfigManager.Instance.MonsterConfigList[monster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID].m_shieldpoint)
				{
					PlayRecoverProtectionEff();
				}
				m_previousshardValue = bossShard;

				TweenFloat.Begin(0.3f, ShardProgressBar.fillAmount, shardValue, ChangeShardProgressBar);
                if (IsShowStatusLabel)
                {
                    ShardNumLabel.text = string.Format("{0}/{1}", bossShard, BattleConfigManager.Instance.MonsterConfigList[monster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID].m_shieldpoint);
                }
                else
                {
                    ShardNumLabel.text = "";
                }
                if (shardValue <= 0)
                {
					PlayerShardEffect(monster.BaseObjectValues.OBJECT_FIELD_ENTRY_ID);
                    MonsterManager.Instance.GetEntityMode(entityDataUpdateNotify.EntityUID).GO.GetComponent<HurtFlash>().OnDisrupt(true);                 	
				}
                else
                {
                    MonsterManager.Instance.GetEntityMode(entityDataUpdateNotify.EntityUID).GO.GetComponent<HurtFlash>().OnDisrupt(false);
                    ShardProgressBar.enabled = true;
                    ShardBarFrame.enabled = true;
                    m_isFlag = true;
                }
            }
        }
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
        //m_shardProgressEffect = CreatObjectToNGUI.InstantiateObj(ShardProgressEffect, ShardProgressEffectPoint); //旧破防特效去掉

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
