using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionEffectBehaviour : View {

    private const float DEFAULT_EFFECT_LIFE_TIME=3;
    private float m_speed;
    private float m_accleration;
    private int m_loopTimes;
    public float m_effectLifeTime;
    private List<PlayDataStruct<Animation>> m_animations;
    private List<PlayDataStruct<ParticleSystem>> m_particleSystem;
    private bool flag;
    private long m_entityUid;

    void Awake()
    {
        RegisterEventHandler();
    }
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (flag)
        {
            var t = Time.deltaTime;
            var s = m_speed * t + m_accleration * Mathf.Pow(t, 2) * 0.5f;

            m_speed += Mathf.FloorToInt(m_accleration * t);

            transform.Translate(Vector3.forward * s);

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
	}
    public void InitDataConfig(SkillActionData skillActionData,long entityID)
    {
        m_entityUid = entityID;

        transform.RecursiveGetComponent<Animation>("Animation", out m_animations);
        transform.RecursiveGetComponent<ParticleSystem>("ParticleSystem", out m_particleSystem);

        //TraceUtil.Log("m_particleSystem:" + m_particleSystem.Count);

        if (m_animations != null)
            m_animations.ApplyAllItem(ani => ani.AnimComponent.Stop());
        if (m_particleSystem != null)
            m_particleSystem.ApplyAllItem(par => par.AnimComponent.Stop(true));

        m_speed = skillActionData.m_effect_move_speed;
        m_accleration = skillActionData.m_effect_move_accleration;
        m_loopTimes = skillActionData.m_effect_loop_time;

        if (m_animations != null)
        {
            m_animations.ApplyAllItem(animation =>
               {
                   animation.LoopTimes = m_loopTimes;
                   animation.PlayTimeLength = animation.AnimComponent.clip.length;
                   animation.ComponentName = animation.AnimComponent.clip.name;
                   animation.PlayingTime = 0;
                   animation.PlayedTimes = 0;

                   var clipTimeLength=(m_loopTimes+1)*animation.PlayTimeLength;
                   m_effectLifeTime = m_effectLifeTime >= clipTimeLength ? m_effectLifeTime : clipTimeLength; 

               });
        }
        if (m_particleSystem != null)
        {
            m_particleSystem.ApplyAllItem(particleSystem =>
            {
                particleSystem.LoopTimes = m_loopTimes;
                particleSystem.PlayTimeLength = particleSystem.AnimComponent.duration;
                particleSystem.ComponentName = particleSystem.ComponentName;
                particleSystem.PlayingTime = 0;
                particleSystem.PlayedTimes = 0;

                var clipTimeLength = (m_loopTimes + 1) * particleSystem.PlayTimeLength;
                m_effectLifeTime = m_effectLifeTime >= clipTimeLength ? m_effectLifeTime : clipTimeLength;
            });
        }
        if (m_effectLifeTime == 0) m_effectLifeTime = DEFAULT_EFFECT_LIFE_TIME;
        Fire();
    }
    public void StopByBreak()
    {
        if (m_particleSystem != null)
        {
            m_particleSystem.ApplyAllItem(par => par.AnimComponent.Clear());
        }
        StopCoroutine("TimeToReleaseEffect");
    }
    IEnumerator TimeToReleaseEffect(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        m_entityUid = 0;
        GameObjectPool.Instance.Release(gameObject);
    }
    private void Fire()
    {
        flag = true;
        if (m_animations != null)
            m_animations.ApplyAllItem(ani => ani.AnimComponent.Play());
        if (m_particleSystem != null)
            m_particleSystem.ApplyAllItem(par => par.AnimComponent.Play(true));

        StartCoroutine("TimeToReleaseEffect", m_effectLifeTime);
        //StartCoroutine(TimeToReleaseEffect(m_effectLifeTime));
    }
    void ReceiveBeatBackHandle(INotifyArgs notifyArgs)
    {
        if (this.m_entityUid != 0)
        {
            SMsgBattleBeatBack_SC sMsgBattleBeatBack_SC = (SMsgBattleBeatBack_SC)notifyArgs;

            if (sMsgBattleBeatBack_SC.uidFighter == this.m_entityUid)
            {
                flag = false;
            }
        }
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.BeatBack.ToString(), ReceiveBeatBackHandle);
    }
}
