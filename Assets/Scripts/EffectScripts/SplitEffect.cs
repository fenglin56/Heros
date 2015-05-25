using UnityEngine;
using System.Collections;
using System;

public class SplitEffect : MonoBehaviour {

    private float m_moveElapseTime;
    private float m_moveTime;
    private bool m_isMove = true;
    private Vector3 m_moveDirection = Vector3.zero;
    private float m_speed;
    private Transform m_mudTrans;
    private float m_hideDistance;
    private SMsgActionDie_SC m_SMsgActionDie_SC;

	// Use this for initialization
	void Start () {
        m_moveTime = UnityEngine.Random.Range(CommonDefineManager.Instance.CommonDefine.DRAIN_MINTIME, CommonDefineManager.Instance.CommonDefine.DRAIN_MAXTIME);
        float xAxis = UnityEngine.Random.Range(CommonDefineManager.Instance.CommonDefine.DRAIN_MOVE_MINX, CommonDefineManager.Instance.CommonDefine.DRAIN_MOVE_MAXX);
        float yAxis = UnityEngine.Random.Range(CommonDefineManager.Instance.CommonDefine.DRAIN_MOVE_MINY, CommonDefineManager.Instance.CommonDefine.DRAIN_MOVE_MAXY);
        float zAxis = UnityEngine.Random.Range(CommonDefineManager.Instance.CommonDefine.DRAIN_MOVE_MINZ, CommonDefineManager.Instance.CommonDefine.DRAIN_MOVE_MAXZ);
        m_hideDistance = CommonDefineManager.Instance.CommonDefine.DRAIN_LIMITDISTANCE;

        m_speed = CommonDefineManager.Instance.CommonDefine.DRAIN_BALLSPEED;
        m_moveDirection = new Vector3(xAxis, yAxis, zAxis);
        transform.localScale = new Vector3(2, 2, 2);
        m_isMove = true;
	}

    public void SetSplitData(SMsgActionDie_SC data)
    {
		EntityModel mudModel = PlayerManager.Instance.GetEntityMode(data.uidMuderer);
		if(mudModel == null)
		{
			TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"未找到谋杀者!!!!  uid: " + data.uidMuderer);
		}
		else
		{

			m_mudTrans = mudModel.GO.transform;
        	this.m_SMsgActionDie_SC = data;
		}
    }

	// Update is called once per frame
	void Update () {

        if (m_isMove)
        {
            m_moveElapseTime += Time.deltaTime;
            if (m_moveElapseTime < m_moveTime * 0.001)
            {
                transform.Translate(m_moveDirection * Time.deltaTime);
            }
            else
            {
                m_moveElapseTime = 0;
                m_isMove = false;
            }
        }
        else
        {
            var playerDirection = Vector3.Normalize(m_mudTrans.position - this.transform.position);
            var distance = Vector3.Distance(m_mudTrans.position, this.transform.position);
            
            if (distance <= m_hideDistance)
            {
                SMsgFightBloodSucking_CS sMsgFightBloodSucking_CS = new SMsgFightBloodSucking_CS();
                sMsgFightBloodSucking_CS.uidFighter = m_SMsgActionDie_SC.uidMuderer;
                sMsgFightBloodSucking_CS.uidTarget = m_SMsgActionDie_SC.uidEntity;
                sMsgFightBloodSucking_CS.nParam = m_SMsgActionDie_SC.nParam;

                NetServiceManager.Instance.BattleService.SendBattleBloodSucking(sMsgFightBloodSucking_CS);
                DestroyImmediate(this.gameObject);
            }
            else
            {
                transform.Translate(playerDirection * Time.deltaTime * m_speed);
            }
            
        }

	}


}
