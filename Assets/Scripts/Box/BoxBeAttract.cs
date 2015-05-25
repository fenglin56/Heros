using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxBeAttract : MonoBehaviour 
{
	public enum BoxStatus
	{
		none = 0,
		unpick,
		pick,
		fly,
	}


    public GoldBehaviour GoldBehaviour;

    private EntityModel m_target;
    private List<EntityModel> m_HerosList = new List<EntityModel>();

    private bool m_IsInit = false;
    private bool m_IsAttract = false;

    const float ATTRACT_SPEED = 1f;        //吸引的速度

	private float m_fly_speed = 1f;		//飞行速度

	private float m_fly_high = 12f;

	private BoxStatus m_boxStatus = BoxStatus.unpick;
//	private long m_playerUID;
//	private long m_itemUID;
//	private int m_itemID;
	private float m_moneyDropRadius = 1f;

	private Transform m_thisTransfrom;
	private GameObject m_eff_fly = null;


	void Awake()
	{
		m_thisTransfrom = this.transform;
	}

    void Update()
    {
		switch(m_boxStatus)
		{
		case BoxStatus.unpick:
			if (m_IsAttract)
			{
				Attract();
			}
			else
			{
				if (m_IsInit)
				{
					Judge();
				}
				else
				{
					Init();
				}
			}
			break;
		case BoxStatus.pick:
			break;
		case BoxStatus.fly:
			Fly();
			break;
		}       
    }

	IEnumerator LateAutoPickUp()
	{
		yield return new WaitForSeconds(CommonDefineManager.Instance.CommonDefine.AutoPickup_Time);
		SetFly();
	}

	private void Fly()
	{
		if (m_target == null || ((PlayerBehaviour)m_target.Behaviour).FSMSystem.CurrentStateID == StateID.PlayerDie)
		{
			m_IsAttract = false;
			m_IsInit = false;
			m_boxStatus = BoxStatus.unpick;
			if(m_eff_fly !=null)
			{
				Destroy(m_eff_fly);
			}
			SetDamageChildActive(true);
			return;
		}
		Vector3 targetPos = m_target.GO.transform.position;
		//targetPos.y = 0;
		Vector3 myPos = m_thisTransfrom.position;
		//myPos.y = 0;
		Vector3 v = targetPos - myPos;
		m_thisTransfrom.position += (v.normalized * m_fly_speed * Time.deltaTime);
		if (Vector3.Distance(m_thisTransfrom.position, m_target.GO.transform.position) <= CommonDefineManager.Instance.CommonDefine.DropItem_RadiusParam)
		{
			GameObject pickEff = UI.CreatObjectToNGUI.InstantiateObj(GameManager.Instance.DamageFactory.Eff_AutomaticPick_BePick_Prefab,m_target.GO.transform);
			pickEff.AddComponent<DestroySelf>();
			DamageBehaviour damageBehaviour = GetComponent<DamageBehaviour>();
			if (damageBehaviour != null)
			{
				damageBehaviour.BePickUp(m_target.EntityDataStruct.SMsg_Header.uidEntity);
			} 
			m_boxStatus = BoxStatus.none;
		}
	}

    //吸引
    private void Attract()
    {
        if (m_target == null)
        {
            m_IsAttract = false;
            m_IsInit = false;
            return;
        }

        /*
        Vector3 v = m_target.GO.transform.position - transform.position;
        transform.Translate(v.normalized * ATTRACT_SPEED);
        if (Vector3.Distance(transform.position, m_target.GO.transform.position) <= ATTRACT_SPEED)
        {
            DamageBehaviour damageBehaviour = GetComponent<DamageBehaviour>();
            if (damageBehaviour != null)
            {
                damageBehaviour.BePickUp(m_target.EntityDataStruct.SMsg_Header.uidEntity);
            }
        }
         */ //去掉吸引，直接拾取
        DamageBehaviour damageBehaviour = GetComponent<DamageBehaviour>();
        if (damageBehaviour != null)
        {
            damageBehaviour.BePickUp(m_target.EntityDataStruct.SMsg_Header.uidEntity);
        }
		m_boxStatus = BoxStatus.none;
    }
    //初始化
    private void Init()
    {
        var heroModel = PlayerManager.Instance.FindHeroEntityModel();
        if (heroModel != null)
        {
            m_HerosList.Add(heroModel);
        }        
        if (m_HerosList.Count >= 1)
        {
            m_IsInit = true;
			m_moneyDropRadius =  PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_PICK_RANGE_VALUE/10;
			StartCoroutine("LateAutoPickUp");
        }
    }
    //判断范围内最近的英雄
    private void Judge()
    {
        if (GoldBehaviour != null && GoldBehaviour.IsShow == false)
            return;      

        //float distance = CommonDefineManager.Instance.CommonDefine.GameMoneyDropRadius;
        m_HerosList.ApplyAllItem(p =>
            {
                if (p != null && p.GO != null)
                {
					float dis = Vector3.Distance(m_thisTransfrom.position, p.GO.transform.position);
                    // TraceUtil.Log("[Distance]"+dis);
					if (dis <= m_moneyDropRadius)
                    {
                        m_target = p;                        
                        m_IsAttract = true;
                    }
                }                
            });
    }

	//判断范围内最近英雄拾取
	public void JudgeAndPickUp()
	{
		if(m_boxStatus != BoxStatus.unpick)
		{
			return;
		}
		float lastDis = 0;
		m_HerosList.ApplyAllItem(p =>
		                         {
			if (p != null && p.GO != null)
			{
				float dis = Vector3.Distance(m_thisTransfrom.position, p.GO.transform.position);
				// TraceUtil.Log("[Distance]"+dis);
				if (dis > lastDis)
				{
					m_target = p;
					dis = lastDis;
//					m_IsAttract = true;
				}
			}                
		});
		GameObject dust = (GameObject)Instantiate(GameManager.Instance.DamageFactory.Eff_AutomaticPick_Start_Prefab);
		dust.transform.position = m_thisTransfrom.position;
		dust.AddComponent<DestroySelf>();
		GameObject flyEff = (GameObject)Instantiate(GameManager.Instance.DamageFactory.Eff_AutomaticPick_Fly_Prefab);
		flyEff.transform.position = new Vector3(m_thisTransfrom.position.x,1,m_thisTransfrom.position.z);

		//GameObject flyEff = UI.CreatObjectToNGUI.InstantiateObj(GameManager.Instance.DamageFactory.Eff_AutomaticPick_Fly_Prefab,m_thisTransfrom);
		flyEff.transform.parent = null;
		BoxFlyEffect effectBehaviour = flyEff.AddComponent<BoxFlyEffect>();
		effectBehaviour.Init(m_target,CommonDefineManager.Instance.CommonDefine.AutoPickup_Speed,
		                     CommonDefineManager.Instance.CommonDefine.DropItem_RadiusParam);

		//上发拾取
		DamageBehaviour damageBehaviour = GetComponent<DamageBehaviour>();
		if (damageBehaviour != null)
		{
			damageBehaviour.BePickUp(m_target.EntityDataStruct.SMsg_Header.uidEntity);
		}

		m_boxStatus = BoxStatus.none;
	}


	public void PickOver(long playerUID,long itemUID, EntityModel model, int itemID)//暂时不可用
	{
//		m_playerUID = playerUID;
//		m_itemUID = itemUID;
//		m_itemID = itemID;
//
//		m_target = model;
//
//		m_fly_speed = CommonDefineManager.Instance.CommonDefine.AutoPickup_Speed;
//		m_boxStatus = BoxStatus.pick;
	}

	private void SetFly()
	{
		//\
		m_moneyDropRadius = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_PICK_RANGE_VALUE/10 ;

		m_fly_speed = CommonDefineManager.Instance.CommonDefine.AutoPickup_Speed;

		SetDamageChildActive(false);

		//GameObject dust = UI.CreatObjectToNGUI.InstantiateObj(GameManager.Instance.DamageFactory.Eff_AutomaticPick_Start_Prefab,m_thisTransfrom.parent);
		GameObject dust = (GameObject)Instantiate(GameManager.Instance.DamageFactory.Eff_AutomaticPick_Start_Prefab);
		dust.transform.position = m_thisTransfrom.position;
		dust.AddComponent<DestroySelf>();
		m_eff_fly = UI.CreatObjectToNGUI.InstantiateObj(GameManager.Instance.DamageFactory.Eff_AutomaticPick_Fly_Prefab,m_thisTransfrom);

		float dis = 0;
		int index = 0;
		for(int i = 0; i<m_HerosList.Count;i++)
		{
			if(m_HerosList[i] != null && m_HerosList[i].GO != null)
			{
				float newDis = Vector3.Distance(m_thisTransfrom.position, m_HerosList[i].GO.transform.position);
				if(i == 0)
				{
					dis = newDis;
					index = 0;
				}
				else
				{
					if(newDis < dis)
					{
						dis = newDis;
						index = i;
					}
				}
			}
		}
		m_target = m_HerosList[index];
		m_IsAttract = true;

		//设置高度
		//m_thisTransfrom.position = new Vector3(m_thisTransfrom.position.x, m_fly_high, m_thisTransfrom.position.z);

		m_boxStatus = BoxStatus.fly;
	}

	private void SetDamageChildActive(bool isActive)
	{
		DamageBehaviour damageBehaviour = GetComponent<DamageBehaviour>();
		if(damageBehaviour.TitleRef != null)
		{
			damageBehaviour.TitleRef.SetActive(isActive);
		}
		int childCount = m_thisTransfrom.childCount;
		if (childCount > 0)
		{
			for (int i = childCount-1; i >=0; i--)
			{
				m_thisTransfrom.GetChild(i).gameObject.SetActive(isActive);
			}
		}
	}


}
