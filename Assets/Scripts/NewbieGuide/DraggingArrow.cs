using UnityEngine;
using System.Collections;

public class DraggingArrow : MonoBehaviour {

    //public TrailRenderer TrailComponent;
    //public GameObject DraggingIcon;
    //private Transform m_sourcePos;
    //private Transform m_targetPos;
    //private Vector3 m_direction;
    //// Use this for initialization
    //private float m_length;
    //private Vector3 m_position;

    //void Start()
    //{
    //    //m_position = DraggingIcon.transform.localPosition;
    //    //TraceUtil.Log("##############m_position" + m_position);
    //}
    
    //public void InitDragging(Vector3 srcTrans, Vector3 targetTrans)//, Vector3 direction)
    //{
    //    srcTrans.z = targetTrans.z  = -50;
    //    targetTrans.x *= 0.5f;  //偏移按钮中心点
    //    m_direction = targetTrans - srcTrans;
    //    m_position = DraggingIcon.transform.localPosition = new Vector3(srcTrans.x, srcTrans.y + 40f, srcTrans.z);
    //    m_length = Vector3.Distance(srcTrans, targetTrans) * 0.5f;
    //}

    //float m_moveElapseTime = 0;
    //float m_speed = 0.5f;
    

    //// Update is called once per frame
    //void Update () {
    //    float timeDelta = Time.deltaTime;
    //    m_moveElapseTime += timeDelta;

    //    if (m_moveElapseTime * m_speed < m_length)
    //    {
    //        TrailComponent.time = 0.5f;
    //        var m_moveTime = m_speed * timeDelta;
    //        DraggingIcon.transform.Translate(m_direction.normalized * m_moveTime);
    //    }
    //    else
    //    {
    //        TrailComponent.time = 0.0f;
    //        DraggingIcon.transform.localPosition = m_position;
    //        m_moveElapseTime = 0;
    //    }
    //}
}
