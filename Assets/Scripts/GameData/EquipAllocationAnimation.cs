using UnityEngine;
using System.Collections;
using System;

public class EquipAllocationAnimation : MonoBehaviour 
{
    private float m_riseTime = 1f;
    private float m_stayTime = 1f;
    private float m_endPosY = 20f;
    private GameObject m_heroGO;
    private Camera m_uiCamera;
    private Int64 m_PlayerUID;


    public void Init(GameObject heroGO, Int64 uid)
    {
        m_heroGO = heroGO;
        m_PlayerUID = uid;
    }

    public void Begin()
    {
        m_uiCamera = PopupObjManager.Instance.UICamera;
        
        //Vector3 startPos = m_heroGO.transform.position + new Vector3(0, 20f, 0);
        Vector3 startPos = new Vector3(0, 10f, 0);
        StartCoroutine(Rise(startPos, startPos + new Vector3(0, m_endPosY, 0)));
    }

    IEnumerator Rise(Vector3 startPos, Vector3 endPos)
    {
        float i = 0;
        float rate = 1f / m_riseTime;

        GameObject movePoint = new GameObject();
        movePoint.transform.parent = m_heroGO.transform;
        movePoint.transform.localPosition = Vector3.zero;
        movePoint.transform.localScale = Vector3.one;

        while (i < 1f)
        {
            i += Time.deltaTime * rate;

            movePoint.transform.localPosition = Vector3.Lerp(startPos, endPos, i);
            Vector3 uiPos = PopupTextController.GetPopupPos(movePoint.transform.position, m_uiCamera);
            
            transform.position = uiPos;

            yield return null;
        }

        Destroy(movePoint);
        yield return new WaitForSeconds(m_stayTime);
        Destroy(gameObject);        
    }
}
