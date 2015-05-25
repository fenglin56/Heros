using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class BulletData
{
	public int m_bulletId;
    //[GameDataPostFlag(true)]
    //public GameObject m_resource;

    public string m_bulletResPath;
    /*
    [Late]
    public GameObject m_resource;
    [HideInInspector]
    public string m_resourceId;

    public GameObject m_resourcePrefab
    {
        get
        {
            if (m_resource!= null)
            {
                return m_resource;
            }

            m_resource = AssetId.Resolve(m_resource, m_resourceId);
            return m_resource;
            //return AssetId.Resolve(_prefab,_prefabId);
        }
    }
    */
	public int m_shapeParam1;
	public int m_shapeParam2;
	public int m_shapeParam3;
	public float m_startSpeed;
	public float m_acceleration;
	public float m_angle;
	public Vector2 m_initPos;
	public float m_lifeTime;
	public float m_cauculateInverval;
	public int m_overParam;
	
	public int m_bulletIdFollow;
	public int m_monsterIdFollow;
	
	//caldulate
	public int m_calculateId;
	
	//sound effect
    public string m_sfx_id;   
	
	//hurt flash
    public int m_hurtFlash;  
	//shake time
    public int m_hurtShakeTime; 
	//shake attenuation
	public float m_hurtShakeAttenuation;
	//shake init speed
	public float m_hurtShakeInitSpeed;
	//shake elasticity
	public float m_hurtShakeElasticity;
	
    //public GameObject m_hurt_effect;  //打击光效
    public string m_hurtEffectPath;
	
	public GameObject m_hurt_Ui_Effect;
	
	
    //hurt_effect_RotationFlag
    public byte m_hurtEffectRotationFlag;

    public byte m_followtype;   //子弹跟随类型

    public byte m_mountType;    //子弹挂载类型

    public byte m_breakType;    //子弹连接属性 0不连接 1连接
	
	public int m_affectTarget;
	
	// born shake time
    public int m_bornShakeTime; 
	// born shake attenuation
	public float m_bornShakeAttenuation;
	//born shake init speed
	public float m_bornShakeInitSpeed;
	//born shake elasticity
	public float m_bornShakeElasticity;
	
	//born sfx
	public BulletBornSfx[] m_bornSfxId; //x = soundID,y=


    public string m_shakeAniName;
    public string m_bornShakeAniName;

    public int m_bulletStrengthen;
	
	
}

[Serializable]
public class BulletBornSfx
{
	public string Id;
	public float DelayTime;
}


public class BulletDataBase : ScriptableObject {

	public BulletData[] _dataTable;
	
}
