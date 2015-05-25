using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StroyActionEffect : MonoBehaviour {

    private List<PlayDataStruct<Animation>> m_animations;
    private List<PlayDataStruct<ParticleSystem>> m_particleSystem;
    private int m_loopTimes;
    public float m_effectLifeTime;

    // Update is called once per frame
    void Update()
    {
        if (m_animations != null)
        {
            m_animations.ApplyAllItem(animation =>
            {
                animation.PlayingTime += Time.deltaTime;
                if (animation.PlayingTime >= animation.PlayTimeLength)
                {
                    animation.PlayedTimes++;
                    if (animation.PlayedTimes >= animation.LoopTimes)
                    {
                        animation.AnimComponent.Stop();
                    }
                    else
                    {
                        animation.PlayingTime = 0;
                        if (animation.AnimComponent.IsPlaying(animation.ComponentName))
                        {
                            animation.AnimComponent.CrossFade(animation.ComponentName);
                        }
                    }
                }
                else
                {
                    if (!animation.AnimComponent.IsPlaying(animation.ComponentName))
                    {
                        animation.AnimComponent.CrossFade(animation.ComponentName);
                    }
                }

            });
        }
    }

    IEnumerator TimeToReleaseEffect(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        GameObjectPool.Instance.Release(gameObject);
    }

    private void Fire()
    {
        if (m_animations != null)
            m_animations.ApplyAllItem(ani => ani.AnimComponent.Play());
        if (m_particleSystem != null)
            m_particleSystem.ApplyAllItem(par => par.AnimComponent.Play());

        StartCoroutine(TimeToReleaseEffect(m_effectLifeTime));
    }

    public void InitDataConfig(int effectLoopTimes)
    {
        transform.RecursiveGetComponent<Animation>("Animation", out m_animations);
        transform.RecursiveGetComponent<ParticleSystem>("ParticleSystem", out m_particleSystem);

        if (m_animations != null)
            m_animations.ApplyAllItem(ani => ani.AnimComponent.Stop());
        if (m_particleSystem != null)
            m_particleSystem.ApplyAllItem(par => par.AnimComponent.Stop());

        m_loopTimes = effectLoopTimes;

        if (m_animations != null)
        {
            m_animations.ApplyAllItem(animation =>
            {
                animation.LoopTimes = m_loopTimes;
                animation.PlayTimeLength = animation.AnimComponent.clip.length;
                animation.ComponentName = animation.AnimComponent.clip.name;
                animation.PlayingTime = 0;
                animation.PlayedTimes = 0;

                var clipTimeLength = (m_loopTimes + 1) * animation.PlayTimeLength;
                m_effectLifeTime = m_effectLifeTime >= clipTimeLength ? m_effectLifeTime : clipTimeLength; 
            });
        }
        if (m_particleSystem != null)
        {
            m_particleSystem.ApplyAllItem(particleSystem =>
            {
                particleSystem.LoopTimes = m_loopTimes;
				particleSystem.PlayTimeLength = particleSystem.AnimComponent.startDelay + particleSystem.AnimComponent.duration;
                particleSystem.ComponentName = particleSystem.ComponentName;
                particleSystem.PlayingTime = 0;
                particleSystem.PlayedTimes = 0;

                var clipTimeLength = (m_loopTimes + 1) * particleSystem.PlayTimeLength;
                m_effectLifeTime = m_effectLifeTime >= clipTimeLength ? m_effectLifeTime : clipTimeLength;
            });
        }

        Fire();
    }
}
