using UnityEngine;
using System.Collections;

public class BuffEffectController : MonoBehaviour
{
	private ParticleSystem [] m_particleList;
	private Animation[] m_aniList;

	private bool m_ready = false;
	//private bool m_isPlaying = false;
    //private float m_curDurativeTime;
    //private float m_afterTime;
	
	void Awake()
	{
		m_particleList = GetComponentsInChildren<ParticleSystem>();
		m_aniList = GetComponentsInChildren<Animation>();
		m_ready = true;	
	}

	
	[ContextMenu("emit")]
	public void Emit()
	{
		StartCoroutine(TryEmit());
	}
	
	public IEnumerator TryEmit()
	{	
		while(!m_ready)
		{
			yield return null;
		}
		foreach(ParticleSystem ps in m_particleList)
		{
			ps.enableEmission = true;
			ps.Play();
		}
		foreach(Animation ani in m_aniList)
		{
			ani.Play();	
		}	
		//m_isPlaying = true;
		
	}

    //public float SetDurativeTime
    //{
    //    set { m_curDurativeTime = value; }
    //}

    //void Update()
    //{
    //    if (m_isPlaying)
    //    {
    //        m_afterTime += Time.deltaTime;
    //        if (m_curDurativeTime <= m_afterTime)
    //        {
    //            m_isPlaying = false;
    //            m_afterTime = 0;
    //            SelfRelease();
    //        }
    //    }
    //}
	
	public void SelfRelease()
	{
		GameObjectPool.Instance.Release(gameObject);
	}
}