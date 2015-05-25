using UnityEngine;
using System.Collections;

public class GoldBehaviour : MonoBehaviour {

    public GameObject Effect_gold;    
    public float m_showTime = 0.8f;
    private Transform m_thisTransform;
    private BoxCollider m_boxCollider;
    private Renderer[] m_childRendererArray;
    private GameObject m_EffectGO;
    private bool m_isShow;
    public bool IsShow { get { return m_isShow; } }

    private GameObject FontGObject;

    //void Start () 
    //{
    //    m_thisTransform = this.transform;
    //    m_childRendererArray = m_thisTransform.GetComponentsInChildren<Renderer>();
    //    m_childRendererArray.ApplyAllItem(p =>
    //        {
    //            p.enabled = false;
    //        });
    //    m_boxCollider = m_thisTransform.GetComponent<BoxCollider>();
    //    if (m_boxCollider != null)
    //    {
    //        m_boxCollider.enabled = false;
    //    }
    //    m_EffectGO = (GameObject)Instantiate(Effect_gold, m_thisTransform.position, Quaternion.identity);
    //    //Effect_gold.Emit();
    //    StartCoroutine(Show());
    //}


    public void Play(GameObject fontObj, Vector3 form, Vector3 to)
    {
        m_thisTransform = this.transform;
        this.FontGObject = fontObj;
        m_childRendererArray = m_thisTransform.GetComponentsInChildren<Renderer>();
        m_childRendererArray.ApplyAllItem(p =>
        {
            p.enabled = false;
        });
        if (FontGObject != null)
        {
            FontGObject.SetActive(false);
        }        
        m_boxCollider = m_thisTransform.GetComponent<BoxCollider>();
        if (m_boxCollider != null)
        {
            m_boxCollider.enabled = false;
        }
        m_EffectGO = (GameObject)Instantiate(Effect_gold, m_thisTransform.position, Quaternion.identity);
        var goldMoveBehaviour = m_EffectGO.AddComponent<GoldMoveBehaviour>();
        goldMoveBehaviour.Begin(form, to, m_showTime);
        //Effect_gold.Emit();
        StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        yield return new WaitForSeconds(m_showTime);
        m_childRendererArray.ApplyAllItem(p =>
        {
            p.enabled = true;
        });
        if (FontGObject != null)
        {
            FontGObject.SetActive(true);
        }
        if (m_boxCollider != null)
        {
            m_boxCollider.enabled = true;
            m_isShow = true;
        }
        //DestroyImmediate(m_EffectGO);
        Destroy(m_EffectGO);
    }
}
