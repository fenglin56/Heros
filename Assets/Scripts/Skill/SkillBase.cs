using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UI.Battle;

public enum SkillEventType
{
	DoAction,
	PlayViewEffect,
	PlaySoundEffect,
	FireRangeDamage,
	FireBullet,
	SkillOver,
	ShakeCamera,
    PlayUIEffect,
}

[System.Serializable]
public class SkillEvent
{
	public float EventTimeAfterLaunch;
	public SkillEventType Type;
	public int Param;
	private bool m_handled = false;
	public bool Handled
	{
		set { m_handled = value; } 	
		get { return m_handled; }
	}
}

public class SkillAction
{
	public SkillActionData m_actionData;
	public GameObject[] m_effectList;
	
	public void Setup(SkillActionData data)
	{
		m_actionData = data;
		
		if(m_actionData != null)
		{
			//TODO:setup effect List
		}
	}
}

public class SkillBase : View {
	
	public delegate void DoActionDelegate(SkillActionData actData);
	public delegate void SkillOverDelegate();
	public delegate void SkillStateChangeDelegate(Transition trans,SkillBase skillbase);
	public delegate void SkillBulletFireDelegate(int bulletId, bool useFirePos);
    public delegate void SkillEffectFireDelegate(int actionId, int skillId);
    public delegate void SkillUIEffectFireDelegate(GameObject effect, Vector3 startPos, float lifeTime);
	
	private DoActionDelegate onDoAction;
	private SkillOverDelegate onSkillOver;
	private SkillStateChangeDelegate onSkillStateChange;
	private SkillBulletFireDelegate onBulletFire;
    private SkillEffectFireDelegate onEffectPlay;
    private SkillUIEffectFireDelegate onUIEffectPlay;

    private int m_currentActionThresHold;
    public int CurrentActionThresHold
    {
        get { return m_currentActionThresHold; }
    }
	

    public void AddSkillUIEffectFireDelegate(SkillUIEffectFireDelegate del)
    {
        onUIEffectPlay += del;
    }


    public void RemoveSkillUIEffectFireDelegate(SkillUIEffectFireDelegate del)
    {
        onUIEffectPlay -= del;
    }

	public void AddActionDelegate(DoActionDelegate del)
	{
		onDoAction += del;
	}
	public void RemoveActionDelegate(DoActionDelegate del)
	{
		onDoAction -= del;	
	}
	
	public void AddSkillOverDelegate(SkillOverDelegate del)
	{
		onSkillOver += del;	
	}
	
	public void DeleteSkillOverDelegate(SkillOverDelegate del)
	{
		onSkillOver -= del;	
	}
	
	public void AddSkillStateChangeDelegate(SkillStateChangeDelegate del)
	{
		onSkillStateChange += del;	
	}
	
	public void RemoveSkillStateChangeDelegate(SkillStateChangeDelegate del)
	{
		onSkillStateChange -= del;	
	}
	
	public void AddSkillBulletFireDelegate(SkillBulletFireDelegate del)
	{
		onBulletFire += del;	
	}
	
	public void RemoveSkillBulletFireDelegate(SkillBulletFireDelegate del)
	{
		onBulletFire -= del;	
	}
    public void AddSkillEffectFireDelegate(SkillEffectFireDelegate del)
    {
        onEffectPlay += del;
    }

    public void RemoveSkillEffectFireDelegate(SkillEffectFireDelegate del)
    {
        onEffectPlay -= del;
    }

	private IEntityDataManager m_entityDataManager;
	
	private Action m_onOver;
	
	public int SkillId;
    private Int64 m_UserID;

	private SkillConfigData m_skillData;
	public SkillConfigData SkillData
	{
		get { return m_skillData; }	
	}
	
	
	private SkillAction[] m_skillActionList;
	
	public string AniStr;
	
	private float m_timeAfterFire;
	
	private bool m_onFire;
	
	private List<SkillEvent> events = new List<SkillEvent>();
	
	private List<string> sfxList = new List<string>();
	
	private float m_skillDuration = 0;
	
	//Sound ids
	public string[] SoundList;
	
	private SkillEffectController[] m_effectControllers;
	
	void Awake()
	{
		
	}
	
	void OnDestroy()
	{
		onDoAction = null;
		onSkillOver = null;
	}
	
    public void Init( int id)
    {
		SkillId = id;
		
		m_skillData = SkillDataManager.Instance.GetSkillConfigData(SkillId);
        if (m_skillData == null)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"怪物技能:" + SkillId.ToString()+" 不存在");
			TraceUtil.Log(SystemModel.NotFoundInTheDictionary,TraceLevel.Error,"怪物技能:" + SkillId.ToString()+" 不存在");
        }
		int skillActionCount = m_skillData.m_actionId.Length;
		m_skillActionList = new SkillAction[skillActionCount];
		
		for(int i = 0; i < skillActionCount; i++)
		{
			m_skillActionList[i] = new SkillAction();
			m_skillActionList[i].m_actionData = SkillDataManager.Instance.GetSkillActionData(m_skillData.m_actionId[i]);
		
			float currentActionDuration = m_skillActionList[i].m_actionData.m_startTime + m_skillActionList[i].m_actionData.m_duration;
			if(m_skillDuration < currentActionDuration)
			{
				m_skillDuration = currentActionDuration;	
			}
			
			//skill action event
			SkillEvent evt = new SkillEvent();
			evt.Type = SkillEventType.DoAction;
			evt.Param = i;
			evt.EventTimeAfterLaunch = m_skillActionList[i].m_actionData.m_startTime/1000.0f;
			events.Add(evt);
			
			
			//action sfx event
			if(m_skillActionList[i].m_actionData.m_soundEffectName != "0")
			{
				sfxList.Add(m_skillActionList[i].m_actionData.m_soundEffectName);
				SkillEvent sfxEvt = new SkillEvent();
				sfxEvt.Type = SkillEventType.PlaySoundEffect;
				sfxEvt.Param = sfxList.Count - 1;
				sfxEvt.EventTimeAfterLaunch = m_skillActionList[i].m_actionData.m_startTime/1000.0f + m_skillActionList[i].m_actionData.m_sfxDelay;
				events.Add(sfxEvt);
			
			}

			//TODO:effect event
            SkillEvent effectEvt = new SkillEvent();
            effectEvt.Type = SkillEventType.PlayViewEffect;
            effectEvt.Param = m_skillActionList[i].m_actionData.m_actionId;
            effectEvt.EventTimeAfterLaunch = m_skillActionList[i].m_actionData.m_effect_start_time / 1000.0f;
            events.Add(effectEvt);
		}
		
		//bullet event
		int bulletCount = m_skillData.m_bulletGroups.Length;
		for(int i = 0; i < bulletCount; i++)
		{
			SkillEvent evt = new SkillEvent();
			evt.Type = SkillEventType.FireBullet;
			evt.Param = m_skillData.m_bulletGroups[i].m_bulletId;
			evt.EventTimeAfterLaunch = m_skillData.m_bulletGroups[i].m_delay/1000.0f;
			events.Add(evt);
		}

		//strengthen bullet event
        int strenthenSkillId = m_skillData.FatherSkill == 0? m_skillData.m_skillId : m_skillData.FatherSkill;

		int bulletCount1 =  m_skillData.m_bulletStrengGroups == null?0:m_skillData.m_bulletStrengGroups.Length;
        SSkillInfo? skillInfo = SkillModel.Instance.GetCurSkill (strenthenSkillId);
		if(m_skillData.SkillStrengthen != 0 && skillInfo != null && skillInfo.Value.byStrengthenLv > 1)
		{
			for(int i = 0; i < bulletCount1; i++)
			{
				SkillEvent evt = new SkillEvent();
				evt.Type = SkillEventType.FireBullet;
				evt.Param = m_skillData.m_bulletStrengGroups[i].m_bulletId;
				evt.EventTimeAfterLaunch = m_skillData.m_bulletStrengGroups[i].m_delay/1000.0f;
				events.Add(evt);
			}
		}

        //UI effect Event
        int UIeffectCount = m_skillData.m_UIEffectGroupList.Count;
		for(int i = 0; i < UIeffectCount; i++)
        {

            SkillEvent evt = new SkillEvent();
            evt.Type = SkillEventType.PlayUIEffect;
            evt.Param = i;
            evt.EventTimeAfterLaunch = m_skillData.m_UIEffectGroupList[i]._EffectStartTime;
            events.Add(evt);
        }
		
		//convert from ms to s
		m_skillDuration = m_skillDuration/1000.0f;
		SkillEvent endEvent = new SkillEvent();
		endEvent.EventTimeAfterLaunch = m_skillDuration;
		endEvent.Type = SkillEventType.SkillOver;
		endEvent.Param = 0;
		events.Add(endEvent);
		
        //
		
    }

    public Int64 SetUserID
    {
        set { m_UserID = value; }
    }

	[ContextMenu( "prepare" ) ]
	public void PrePare()
	{
        BattleSkillButtonManager.Instance.SetButtonStatus(this.SkillId, SkillButtonStatus.Wait);

        TraceUtil.Log("[directionParam]" + m_skillData.m_directionParam);

//        if (m_skillData.m_IsSirenSkill)
//        {
//            onSkillStateChange(Transition.PlayerToCastAbility, this);
//            return;
//        }

        switch (m_skillData.m_directionParam)
        {
            case 0: //??????
                {
                    onSkillStateChange(Transition.PlayerFireInitiativeSkill, this);
                }
                break;
            case 1://??????
            case 2:
            case 3:
                {
                    onSkillStateChange(Transition.PlayerInitialtiveSkillSelect, this);
                    //UI.Battle.BattleSkillButtonManager.Instance.SetButtonStatus(m_skillData.m_skillId, SkillButtonStatus.Wait);
                }
                break;

            default:
                break;

        }
	}
	
	[ContextMenu( "fire" ) ]
	public void Fire()
	{
        ResetCurrentActionThresHold();
		ResetEvents();
		m_onFire = true;
		m_timeAfterFire = 0;
        //BattleSkillButtonManager.Instance.SetButtonStatus(this.SkillId, SkillButtonStatus.Disable);
        if (transform.GetComponent<RoleBehaviour>().IsHero && m_skillData.m_triggerType == 0)  //????????????????????????????????????
        {
            if(UI.Battle.BattleSkillButtonManager.Instance != null)
            {
                UI.Battle.BattleSkillButtonManager.Instance.SetButtonStatus(m_skillData.m_skillId, SkillButtonStatus.Disable);
            }
        }
        float fireTime=Time.realtimeSinceStartup;
        BulletFactory.Instance.StartBulletFireTime = fireTime;
        //TraceUtil.Log("Skill Fire At:" + fireTime);
		
		//random sfx
		if(m_skillData.m_skillSfxGroup != null && m_skillData.m_skillSfxGroup.Length > 0 && m_skillData.m_skillSfxGroup[0] != "0")
		{
			int randomNum = UnityEngine.Random.Range(0, 100);
			
			if(randomNum < CommonDefineManager.Instance.CommonDefine.SKILLVOICE)
			{
				int index = UnityEngine.Random.Range(0, m_skillData.m_skillSfxGroup.Length);
				SoundManager.Instance.PlaySoundEffect(m_skillData.m_skillSfxGroup[index]);
			}
		}
        if (EctGuideManager.Instance.IsEctypeGuide&&transform.GetComponent<RoleBehaviour>().IsHero)
        {
            //发出消息 ，目前副本引导监听所施放的技能Id
            RaiseEvent(EventTypeEnum.FireSkillForGuide.ToString(), new SkillFireData() { SkillId = SkillId });
        }
        
	}

    public void PlaySkill()
    {
        ResetEvents();
        m_onFire = true;
        m_timeAfterFire = 0;
    }

	
	public void BreakSkill()
	{
        //TraceUtil.Log("Skill:"+this.SkillData.m_name+" Break!");
		if(!m_onFire)
		{
			return;
		}
        SkillEffectManager.Instance.RemoveEffect(new SkillEffectKey(m_UserID, SkillId));
		m_timeAfterFire = 0;
		m_onFire = false;
        //TraceUtil.Log("Skill:" + this.SkillData.m_name + " Reset!");
		ResetEvents();
        ResetCurrentActionThresHold();
	}

    public void ResetCurrentActionThresHold()
    {
        m_currentActionThresHold = m_skillActionList[0].m_actionData.m_threshold;
    }
	
	public void Stop()
	{
		if(!m_onFire)
		{
			return;
		}

        SkillEffectManager.Instance.RemoveEffect(new SkillEffectKey(m_UserID, SkillId));
		m_timeAfterFire = 0;
		m_onFire = false;
		ResetEvents();
		if(null != onSkillOver)
		{
			onSkillOver();
		}
		else
		{
			//TraceUtil.Log("Skill Action NULL!!!!");
		}
        ResetCurrentActionThresHold();
	}
	
	void ResetEvents()
	{
		foreach(SkillEvent evt in events)
		{
			evt.Handled = false;	
		}
		
	}
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		float timeDelta = Time.deltaTime;
		if(m_onFire)
		{
			m_timeAfterFire += timeDelta;
			foreach(SkillEvent evt in events)
			{
				if(!m_onFire)
				{
					break;	
				}
				if(!evt.Handled && evt.EventTimeAfterLaunch <= m_timeAfterFire)
				{
					evt.Handled = true;
					LaunchEvent(evt);
				}
			}
		}
	}
	
	void LaunchEvent(SkillEvent evt)
	{
		////TraceUtil.Log("--skill event--:  " + evt.Type.ToString() + "---" + evt.Param.ToString());
		
		switch(evt.Type)
		{
		case SkillEventType.DoAction:
		{
			if(null != onDoAction)
			{
                m_currentActionThresHold = m_skillActionList[evt.Param].m_actionData.m_threshold;
				onDoAction(m_skillActionList[evt.Param].m_actionData);	
                
			}
			else
			{
				TraceUtil.Log( SystemModel.Common, TraceLevel.Error,"怪物动作参数:"+evt.Param.ToString()+" 不存在");
				TraceUtil.Log( SystemModel.NotFoundInTheDictionary, TraceLevel.Error,"怪物动作参数:"+evt.Param.ToString()+" 不存在");
			}
		}
			break;
			
		case SkillEventType.FireBullet:
		{
			FireBullet(evt.Param);
		}
			break;
		case SkillEventType.PlayViewEffect:
		{
            PlayActionEffect(evt.Param);
		}
			break;
		case SkillEventType.PlaySoundEffect:
		{
			PlaySoundEffect(evt.Param);	
		}
			break;
			
		case SkillEventType.FireRangeDamage:
		{
			FireRangeDamage(evt.Param);
		}
			break;
		case SkillEventType.SkillOver:
		{
			Stop();
		}
			break;
        case SkillEventType.PlayUIEffect:
        {
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            if (playerData.UID == m_UserID)
            {
                PlayUIEffect(evt.Param);
            }            
        }
            break;
		default:
			break;
		}
	}

    void PlayUIEffect(int index)
    {
        if(null != onUIEffectPlay)
        {
            UIEffectGroup group = SkillData.m_UIEffectGroupList[index];
            onUIEffectPlay(group._UIEffectPrefab, group._EffectStartPos, group._EffectDuration);
        }

    }
	
	void PlaySoundEffect(int index)
	{
		SoundManager.Instance.PlaySoundEffect(sfxList[index]);
	}
	
	void FireRangeDamage(int range)
	{
		
	}
    void PlayActionEffect(int actionId)
    {
        if (onEffectPlay != null)
            onEffectPlay(actionId, SkillId);
    }
	void FireBullet(int bulletId)
	{
        if(onBulletFire!=null)
		{
            onBulletFire(bulletId, m_skillData.m_directionParam == 2);  //==2  ???????????????????????????????????????
		}
	}
    public bool IsNormalSkill()
    {
        return (SkillId == PlayerDataManager.Instance.GetBattleItemData(gameObject.GetComponent<PlayerBehaviour>().PlayerKind).NormalSkillID[0]
				||SkillId == PlayerDataManager.Instance.GetBattleItemData(gameObject.GetComponent<PlayerBehaviour>().PlayerKind).NormalSkillID[1]
				||SkillId == PlayerDataManager.Instance.GetBattleItemData(gameObject.GetComponent<PlayerBehaviour>().PlayerKind).NormalSkillID[2]
				||SkillId == PlayerDataManager.Instance.GetBattleItemData(gameObject.GetComponent<PlayerBehaviour>().PlayerKind).NormalSkillID[3]
						   );
    }
    public bool OnFire
    {
        get { return this.m_onFire; }
    }

    protected override void RegisterEventHandler()
    {
    }
}

