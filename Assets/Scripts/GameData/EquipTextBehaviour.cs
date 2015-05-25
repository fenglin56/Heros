using UnityEngine;
using System.Collections;

public class EquipTextBehaviour : MonoBehaviour {

    private Camera m_UICamera;
    private Transform m_thisEquipTrans;
    private Transform m_thisTransform;
    private bool m_IsEnable = false;
    private float m_posZ = 2f;    //安全距离，在所有ui下面
    private Vector3 adjustVect = new Vector3(0, 40f, 0);

    public void Init(Transform equipTrans)
    {
        m_UICamera = PopupObjManager.Instance.UICamera;
        m_thisEquipTrans = equipTrans;
        m_thisTransform = this.transform;
        m_IsEnable = true;
    }

	void Update () 
    {
        if (m_IsEnable)
        {
            if (m_thisEquipTrans != null)
            {
                Vector3 uiPos = PopupTextController.GetPopupPos(m_thisEquipTrans.position, m_UICamera);
                m_thisTransform.position = new Vector3(uiPos.x, uiPos.y , m_posZ);
                m_thisTransform.localPosition = m_thisTransform.localPosition + adjustVect;
            }
            else
            {
                m_IsEnable = false;
                Destroy(gameObject);
            }
        }        
	}
}
