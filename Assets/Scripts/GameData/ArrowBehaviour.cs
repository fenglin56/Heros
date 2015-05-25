using UnityEngine;
using System.Collections;

public class ArrowBehaviour : MonoBehaviour
{
    private Transform m_thisTransform;

    private Transform m_arrowTransform;

    public bool isShowArrow = false;
    private bool m_isReady = false;

    private Vector3 m_arrowPos = Vector3.zero;

    //void Start () 
    //{       
    //    //else
    //    //{
    //    //    this.OnBecameVisible();
    //    //}
    //}

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
    private void Init()
    {
        m_thisTransform = this.transform;

        Vector3 pos = Camera.main.WorldToViewportPoint(m_thisTransform.position);
        if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1)
        {
            if (ArrowManager.Instance.IsCanAddMonsterArrow())
            {
                this.SetVisible(true);
            }            
        }

        this.m_isReady = true;
    }
    public void SetArrow(Transform arrowTrans)
    {
        m_arrowTransform = arrowTrans;
        m_arrowTransform.parent = PopupObjManager.Instance.UICamera.transform;  //
        m_arrowTransform.localScale = Vector3.one;
        m_arrowTransform.gameObject.SetActive(false);

        Init();
    }


	// Update is called once per frame
    void Update()
    {
        //var player = PlayerManager.Instance.FindHero();
        //if (player == null)
        //{
        //    return;
        //}
        //float atan2 = Mathf.Atan2(m_thisTransform.position.z - player.transform.position.z, m_thisTransform.position.x - player.transform.position.x);
        //float angel = atan2 * Mathf.Rad2Deg;
        if (!isShowArrow/* || !m_arrowTransform.gameObject.activeSelf*/)
            return;

        Vector3 pos = Camera.main.WorldToViewportPoint(m_thisTransform.position);
        pos = new Vector3(pos.x - 0.5f, pos.y - 0.5f, pos.z);

        //TraceUtil.Log(pos);
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        m_arrowPos = ArrowManager.Instance.Judge(pos.x, pos.y, angle);

        m_arrowTransform.localPosition = m_arrowPos;
        m_arrowTransform.localEulerAngles = new Vector3(0, 0, angle - 90);

    }

    void OnBecameVisible()
    {
        if (m_isReady)
        {
            SetVisible(false);
        }
    }

    void OnBecameInvisible()
    {
        if (m_isReady)
        {
            if (ArrowManager.Instance != null)
            {
                if (ArrowManager.Instance.IsCanAddMonsterArrow())
                {
                    SetVisible(true);
                }            
            }            
        }        
    }
    private void SetVisible(bool flag)
    {
        isShowArrow = flag;
        if (m_arrowTransform != null)
        {
            m_arrowTransform.gameObject.SetActive(flag);
        }
    }
}
