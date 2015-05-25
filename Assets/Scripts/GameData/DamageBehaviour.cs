using UnityEngine;
using System.Collections;
using System;

public class DamageBehaviour :View , IEntityDataManager{

    private IEntityDataStruct m_damageDataModel;
    public IEntityDataStruct DamageDataModel
    {
        get { return this.m_damageDataModel; }
        set { this.m_damageDataModel = value; }
    }

    private bool m_IsTriggerEnable = true;
    private float m_riseTime = 0.3f;
    private float m_stayTime = 1f;
    private float m_dropTime = 0.2f;
    private float m_endPosY = 25f;

    public bool m_IsBePickUp = false;

	public GameObject TitleRef;

    //void OnTriggerEnter(Collider other)
    //{
    //    if (!m_IsBePickUp)
    //    {
    //        if (m_IsTriggerEnable)
    //        {
    //            m_IsBePickUp = true;
    //            var playerData = PlayerManager.Instance.FindHeroDataModel();
    //            var player = PlayerManager.Instance.FindHeroEntityModel();
    //            if (m_damageDataModel != null)
    //            {
    //                NetServiceManager.Instance.EntityService.SendActionTouchBox(playerData.UID, m_damageDataModel.SMsg_Header.uidEntity);
    //            }
    //        }
    //    }
    //}

    public void BePickUp(Int64 heroUID)
    {
        if (!m_IsBePickUp)
        {
            if (m_IsTriggerEnable)
            {
 
                NetServiceManager.Instance.EntityService.SendActionTouchBox(heroUID, m_damageDataModel.SMsg_Header.uidEntity);
                m_IsBePickUp = true;
            }    
        }        
    }

    //public void AllocationSuccess(Vector3 heroPos)
    //{
    //    m_IsTriggerEnable = false;
    //    StartCoroutine(Rise(heroPos, new Vector3(heroPos.x, m_endPosY, heroPos.z)));
    //}

    public void AllocationFailure(Vector3 heroPos)
    {
        m_IsTriggerEnable = false;
        StartCoroutine(Drop(heroPos, new Vector3(heroPos.x, m_endPosY, heroPos.z)));
    }

    IEnumerator Rise(Vector3 startPos, Vector3 endPos)
    {
        float i = 0;
        float rate = 1f / m_riseTime;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;

            transform.position = Vector3.Lerp(startPos, endPos, i);
            yield return null;
        }

        yield return new WaitForSeconds(m_stayTime);
        Destroy(gameObject);
    }
    

    IEnumerator Drop(Vector3 startPos, Vector3 endPos)
    {
        float i = 0;
        float rate = 1f / m_riseTime;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;

            transform.position = Vector3.Lerp(startPos, endPos, i);
            yield return null;
        }
        i = 0;
        rate = 1f / m_dropTime;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;

            transform.position = Vector3.Lerp(endPos, startPos, i);
            yield return null;
        }
        m_IsTriggerEnable = true;
    }

    protected override void RegisterEventHandler()
    {
        throw new System.NotImplementedException();
    }

    public IEntityDataStruct GetDataModel()
    {
        return this.DamageDataModel;
    }
}
