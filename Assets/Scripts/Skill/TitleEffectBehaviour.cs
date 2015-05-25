using UnityEngine;
using System.Collections;

public class TitleEffectBehaviour : MonoBehaviour 
{
	public Camera m_MainCamera;
	public Camera m_uiCamera;
	private Transform m_thisTransform;
	private Transform m_heroTransform;
	private Transform m_childTransform;
	public float fCycleTime = 17f;
	public float fDurationTime = 10f;
	Vector3 m_effectPos = new Vector3(0, 28f, 0);
	// Use this for initialization

	#region 粒子显示在UI后
	private int renderQueue = 2500; //普通是在3000
	#endregion


	void Start () 
	{
		m_MainCamera = Camera.main.camera;
		m_uiCamera = BattleManager.Instance.UICamera;
		m_thisTransform = this.transform;
		m_childTransform = m_thisTransform.GetChild(0);        
		
		//InvokeRepeating("Cycle", 0, fCycleTime);
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		renderers.ApplyAllItem(p=>{
			p.material.renderQueue = renderQueue;
		});

	}
	
	public void SetHeroTransform(Transform heroTrans)
	{
		m_heroTransform = heroTrans;
	}
	
	public void SetMedalActive(bool active)
	{
		m_childTransform.gameObject.SetActive(active);
		
		StopCoroutine("Duration");
		CancelInvoke("Cycle");        
		
		if (active)
		{
			InvokeRepeating("Cycle", 0, fCycleTime);
		}
	}
	
	// Update is called once per frame
//	void Update () 
//	{
//		//方法一:朝向摄像机
//		//if (m_MainCamera == null)
//		//{
//		//    if (Camera.main != null)
//		//    {
//		//        m_MainCamera = Camera.main.camera;
//		//    }            
//		//}
//		//else
//		//{
//		//    if (m_heroTransform != null)
//		//    {
//		//        m_thisTransform.transform.position = m_heroTransform.position + m_effectPos;
//		//        m_thisTransform.LookAt(m_MainCamera.transform);
//		//    }
//		//    else
//		//    {
//		//        Destroy(m_thisTransform.gameObject);
//		//    }
//		//}
//		//方法二:固定角度
//		if (m_heroTransform != null)
//		{
//			m_thisTransform.position = GetPopupPos(m_heroTransform.position+ m_effectPos, m_uiCamera);
//		}
//		else
//		{
//			Destroy(m_thisTransform.gameObject);
//		}
//
//
//	}

	public Vector3 GetPopupPos(Vector3 sPos, Camera uiCamera)
	{
		var worldPos = Camera.main.WorldToViewportPoint(sPos);
		var uipos = uiCamera.ViewportToWorldPoint(worldPos);
		
		uipos.z = 2;
		return uipos;
	}

	void Cycle()
	{
		StartCoroutine("Duration");
	}
	

	IEnumerator Duration()
	{
		m_childTransform.gameObject.SetActive(true);
		yield return new WaitForSeconds(fDurationTime);
		m_childTransform.gameObject.SetActive(false);
	}
}
