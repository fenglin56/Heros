using UnityEngine;
using System.Collections.Generic;

struct AttachmentElement
{
    public AttachPoint attachPoint;
    public Transform parent;
}

public enum AttachPoint
{
    None,
    LHWeapon,
    RHWeapon,
    LBWeapon,
    RBWeapon,
}

[AddComponentMenu("RoleSetup/Weapon Attachment")]
public class WeaponAttachment : MonoBehaviour {

    //public List<WeaponBaseItem> items = new List<WeaponBaseItem>();
    AttachmentElement[] m_attachItems = new AttachmentElement[4];
    GameObject m_child;
    AttachPoint m_attachPoint;

    void Start()
    {
        transform.RecursiveFindObject("Attachment-LHWeapon", out m_attachItems[0].parent);
        m_attachItems[0].attachPoint = AttachPoint.LHWeapon;

        transform.RecursiveFindObject("Attachment-RHWeapon", out m_attachItems[1].parent);
        m_attachItems[1].attachPoint = AttachPoint.RHWeapon;

        transform.RecursiveFindObject("Attachment-LBWeapon", out m_attachItems[2].parent);
        m_attachItems[2].attachPoint = AttachPoint.LBWeapon;

        transform.RecursiveFindObject("Attachment-RBWeapon", out m_attachItems[3].parent);
        m_attachItems[3].attachPoint = AttachPoint.RBWeapon;

        //if (!m_attachItems[0].parent)
        //    //TraceUtil.Log("鏈兘鎵惧埌绗竴涓寕杞界偣锛岃妫€鏌ヨ鑹叉寕杞界偣鍛藉悕!");
        //if (!m_attachItems[1].parent)
        //    //TraceUtil.Log("鏈兘鎵惧埌绗簩涓寕杞界偣锛岃妫€鏌ヨ鑹叉寕杞界偣鍛藉悕!");
        //if (!m_attachItems[2].parent)
        //    //TraceUtil.Log("鏈兘鎵惧埌绗笁涓寕杞界偣锛岃妫€鏌ヨ鑹叉寕杞界偣鍛藉悕!");
        //if (!m_attachItems[3].parent)
        //    //TraceUtil.Log("鏈兘鎵惧埌绗洓涓寕杞界偣锛岃妫€鏌ヨ鑹叉寕杞界偣鍛藉悕!");
      
        /////娴嬭瘯鏁版嵁 Start
        //Attach(items[0].attachment, items[0].attachPoint);
        //Attach(items[1].attachment, items[1].attachPoint);
        ////娴嬭瘯end
    }

    /// <summary>
    /// 鎸傝浇鐗╀綋缁欒鑹?
    /// </summary>
    /// <param name="prefab">琚寕杞界殑鐗╀欢</param>
    /// <param name="attachPoint">鎸傝浇鐐圭殑浣嶇疆</param>
    /// <returns>宸茬粡琚寕杞界殑鐗╀欢</returns>
    public GameObject Attach(GameObject prefab, AttachPoint attachPoint)
    {
        // 鍚屼竴鎸傝浇鐐癸紝鍒犻櫎鍘熸潵鐨勭墿浠?
        if (m_child != null && m_attachPoint == attachPoint) Destroy(m_child);

        // If we have something to create, let's do so now
        if (prefab != null)
        {
            for (int i = 0; i < m_attachItems.Length; ++i)
            {
                if (attachPoint == m_attachItems[i].attachPoint)
                {
                    Transform t = m_attachItems[i].parent;
                    m_child = Instantiate(prefab, t.position, t.rotation) as GameObject;
                    m_attachPoint = m_attachItems[i].attachPoint;

                    // Parent the child to this object
                    Transform ct = m_child.transform;
                    ct.parent = t;

                    // Reset the pos/rot/scale, just in case
                    ct.localPosition = Vector3.zero;
                    ct.localRotation = Quaternion.identity;
                    ct.localScale = Vector3.one;
                }                    
            } 
        }

        return m_child;
    }
}
