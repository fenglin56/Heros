using UnityEngine;
using System.Collections;

public class MedalEffectBehaviour : MonoBehaviour 
{
    public Camera m_MainCamera;
    private Transform m_thisTransform;
    private Transform m_heroTransform;
    private Transform m_childTransform;
    public float fCycleTime = 17f;
    public float fDurationTime = 10f;
    Vector3 m_effectPos = new Vector3(0, 20f, 0);
	// Use this for initialization
	void Start () 
    {
        m_MainCamera = Camera.main.camera;
        m_thisTransform = this.transform;
        m_childTransform = m_thisTransform.GetChild(0);        

        InvokeRepeating("Cycle", 0, fCycleTime);
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
	void LateUpdate () 
    {
        //方法一:朝向摄像机
        //if (m_MainCamera == null)
        //{
        //    if (Camera.main != null)
        //    {
        //        m_MainCamera = Camera.main.camera;
        //    }            
        //}
        //else
        //{
        //    if (m_heroTransform != null)
        //    {
        //        m_thisTransform.transform.position = m_heroTransform.position + m_effectPos;
        //        m_thisTransform.LookAt(m_MainCamera.transform);
        //    }
        //    else
        //    {
        //        Destroy(m_thisTransform.gameObject);
        //    }
        //}
        //方法二:固定角度
        if (m_heroTransform != null)
        {
            m_thisTransform.transform.position = m_heroTransform.position + m_effectPos;
        }
        else
        {
            Destroy(m_thisTransform.gameObject);
        }
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
