using UnityEngine;
using System.Collections;

public class ChunnalArrowBehaviour : MonoBehaviour {

    //private Transform m_thisTransform;
    private Transform m_arrowTransform;
    private Transform m_heroTransform;
    private Transform m_chunnalTransform;

    public bool isShowArrow = false;
    public float ArrowFormHeroDistance = 15f;

    public float ShowDurtionDisance = 100f;

	void Start ()
    {        
        var player = PlayerManager.Instance.FindHero();
        m_heroTransform = player.transform;

        InvokeRepeating("JudgeShow", 1f, 0.5f);        
	}

    void OnDestroy()
    {
        if (m_arrowTransform != null)
        {
            if (m_arrowTransform.gameObject != null)
            {
                Destroy(m_arrowTransform.gameObject);
            }            
        }
    }

    public void SetArrow(Transform arrowTrans, Transform chunnalTrans)
    {
        m_arrowTransform = arrowTrans;
        m_chunnalTransform = chunnalTrans;
        //m_arrowTransform.parent = PopupObjManager.Instance.UICamera.transform;  //
        //m_arrowTransform.localScale = Vector3.one;
        m_arrowTransform.gameObject.SetActive(false);
        
    }

    private void Init()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(m_chunnalTransform.position);
        if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1)
        {
            this.SetVisible(true);
        }        
    }
	// Update is called once per frame
	void Update () 
    {
        if (!isShowArrow)
            return;

        m_arrowTransform.LookAt(m_chunnalTransform);

        if (m_heroTransform == null)
            return;

        //Vector3 pos = m_heroTransform.position + (m_chunnalTransform.position - m_heroTransform.position).normalized * ArrowFormHeroDistance;
        Vector3 pos = m_heroTransform.position;
        m_arrowTransform.position = pos + Vector3.up * 0.2f ;
	}

    void JudgeShow()
    {
        if (m_heroTransform == null)
        {
            CancelInvoke("JudgeShow");
            return;
        }
        
        float dis = Vector3.Distance(m_chunnalTransform.position, m_heroTransform.position);
        if (dis <= ShowDurtionDisance)
        {
            this.SetVisible(false);
        }
        else
        {
            this.SetVisible(true);
        }
    }

    //void OnBecameVisible()
    //{
    //    SetVisible(false);
    //}

    //void OnBecameInvisible()
    //{
    //    SetVisible(true);
    //}

    private void SetVisible(bool flag)
    {
        isShowArrow = flag;
        if (m_arrowTransform != null)
        {
            m_arrowTransform.gameObject.SetActive(flag);
        }
    }
}
