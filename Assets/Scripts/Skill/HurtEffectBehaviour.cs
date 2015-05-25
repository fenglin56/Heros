using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HurtEffectBehaviour : MonoBehaviour {

    public float m_effectLifeTime;
    private const float DEFAULT_EFFECT_LIFE_TIME = 2;
    private List<PlayDataStruct<Animation>> m_animations;
    private List<PlayDataStruct<ParticleSystem>> m_particleSystem;
    private bool m_switch = false;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (!m_switch)
        {
            InitDataConfig();
            m_switch = true;
        }
	}
    public void InitDataConfig()
    {
        transform.RecursiveGetComponent<Animation>("Animation", out m_animations);
        transform.RecursiveGetComponent<ParticleSystem>("ParticleSystem", out m_particleSystem);

        if (m_animations != null)
            m_animations.ApplyAllItem(ani => ani.AnimComponent.Stop());
        if (m_particleSystem != null)
		{
            m_particleSystem.ApplyAllItem(par => par.AnimComponent.Stop());
			m_particleSystem.ApplyAllItem(par => par.AnimComponent.Clear());
		}
      
        if (m_animations != null)
        {
            m_animations.ApplyAllItem(animation =>
            {

                m_effectLifeTime = m_effectLifeTime >= animation.PlayTimeLength ? m_effectLifeTime : animation.PlayTimeLength;

            });
        }
        if (m_particleSystem != null)
        {
            m_particleSystem.ApplyAllItem(particleSystem =>
            {
                m_effectLifeTime = m_effectLifeTime >= particleSystem.PlayTimeLength ? m_effectLifeTime : particleSystem.PlayTimeLength;
            });
        }
        if (m_effectLifeTime == 0) m_effectLifeTime = DEFAULT_EFFECT_LIFE_TIME;
        
        if (m_animations != null)
            m_animations.ApplyAllItem(ani => ani.AnimComponent.Play());
        if (m_particleSystem != null)
            m_particleSystem.ApplyAllItem(par => par.AnimComponent.Play());

        StartCoroutine(TimeToReleaseEffect(m_effectLifeTime));
    }
    IEnumerator TimeToReleaseEffect(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        if(BattleManager.Instance != null)
        {
            BattleManager.Instance.OnHurtEffectDestroy();
        }
        GameObjectPool.Instance.Release(gameObject);
        m_switch = false;
    }
}
