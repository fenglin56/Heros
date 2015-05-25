using UnityEngine;
using System.Collections;

public class CrusadeManager : ISingletonLifeCycle
{
	private static CrusadeManager m_instance;
	public static CrusadeManager Instance
	{
		get{
			if(m_instance == null)
			{
				m_instance = new CrusadeManager();
				SingletonManager.Instance.Add(m_instance);
			}
			return m_instance;
		}
	}

	public bool IsMatchingEctype{get;set;}//是否在匹配中

	public void Instantiate ()
	{
	}

	public void LifeOver ()
	{
		m_instance = null;
	}
	
}
