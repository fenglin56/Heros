using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class FHObject:View
{
    public abstract ObjectType GetFHObjType();
}
public abstract class RoleBehaviour : FHObject
{
    [HideInInspector]
    public Transform ThisTransform;
    //protected IEntityDataStruct m_roleDataModel;
	protected EntityModel m_entityModel;
    protected bool m_isHero;
    protected float m_hurtDuration;
	private Transform m_hurtPoint;
    private Renderer m_shadowRenderer;

    public Transform m_runEffect;

    protected List<PlayDataStruct<Animation>> m_AttachAnimations;



	public Transform HurtPoint
	{
		get { return m_hurtPoint; }	
	}
    public Renderer ShadowRenderer
    {
        get { return m_shadowRenderer; }
    }
	
	private HurtFlash m_hurtFlash;
	
	public void ShowHurtFlash(bool isNormal,float duration)
	{
		if(null != m_hurtFlash)
		{
            m_hurtDuration = duration;
            m_hurtFlash.OnAttack(isNormal,duration);	
		}
	}

    public void ShowHordeFlash(bool horde)
    {
        if(null != m_hurtFlash)
        {
            m_hurtFlash.OnHorde(horde);
        }
    }
	

    public bool IsHero 
    {
        get { return this.m_isHero; }        
    }
    public virtual void CacheTransform()
    {
        ThisTransform = transform;
    }
    public void CacheShadow()
    {
        Transform shadow;
        transform.RecursiveFindObject("shadow", out shadow);
        if (shadow != null)
        {
            m_shadowRenderer = shadow.GetComponent<Renderer>();
        }
    }

    public void CacheRunEffect()
    {
        transform.RecursiveFindObject("run_effect", out m_runEffect);
        if(null != m_runEffect)
        {
            m_runEffect.gameObject.SetActive(false);
        }
    }


    protected void CachEntityAnimation()
    {
        transform.RecursiveGetComponent<Animation>("Animation", out m_AttachAnimations);
    }
	public void CacheHurtFlash()
	{
		m_hurtFlash = GetComponent<HurtFlash>();	
	}
	
	public void CacheHurtPoint()
	{
		transform.RecursiveFindObject("HurtPoint", out m_hurtPoint);
	}

	public EntityModel EntityModel
	{
		get { return m_entityModel;}
		set 
		{
			this.m_entityModel = value;
			//this.m_roleDataModel = m_entityModel.EntityDataStruct;
            this.m_isHero = (this.m_entityModel!=null&&this.m_entityModel.EntityDataStruct.SMsg_Header.nIsHero == 1);
			CheckRoleIsHeroOrNot();
		}
	}

    public IEntityDataStruct RoleDataModel
    {
        get { return this.m_entityModel.EntityDataStruct; }
//        set { 
//            this.m_roleDataModel = value; 
//            this.m_isHero = (m_roleDataModel != null && m_roleDataModel.SMsg_Header.nIsHero == 1);
//            CheckRoleIsHeroOrNot();
//        }
    }
    protected virtual void CheckRoleIsHeroOrNot()
    {    }
    public void RotateRole(float angle)
    {
        ThisTransform.Rotate(0, angle, 0);
    }
}
