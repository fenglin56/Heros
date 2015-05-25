using UnityEngine;
using System.Collections;

public class SkillEffectController : MonoBehaviour
{
	private ParticleSystem [] m_particleList;
	private Animation[] m_aniList;
	
	private bool m_ready = false;
	
	private bool m_isPlaying = false;
	
	void Awake()
	{
		m_particleList = GetComponentsInChildren<ParticleSystem>();
		m_aniList = GetComponentsInChildren<Animation>();
		m_ready = true;
		
	}
	
	void Start()
	{
		
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
		m_isPlaying = true;
		StartCoroutine("CheckOver");
		
	}
	
	IEnumerator CheckOver()
	{
		bool isPlaying = true;
		while(isPlaying)
		{
			foreach(ParticleSystem ps in m_particleList)
			{
				if(ps.isPlaying)
				{
					isPlaying = true;
					break;	
				}
				else
				{
					isPlaying = false;	
				}
			}
			
			foreach(Animation ani in m_aniList)
			{
				if(ani.isPlaying)
				{
					isPlaying = true;
					break;	
				}
				else
				{
					isPlaying = false;	
				}
			}
			
			yield return new WaitForSeconds(1.0f);
		}
		SelfRelease();
	}
	
	void SelfRelease()
	{
		GameObjectPool.Instance.Release(gameObject);
	}
}