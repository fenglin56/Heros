using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BattleUI Scene BattleDataManager 
/// </summary>
public class ArrowManager : MonoBehaviour 
{
    public GameObject MonsterArrowPrefab;
    public GameObject ChunnelArrowPrefab;
    public GameObject TeammateArrowPrefab;
    
    //private Dictionary<Int64, GameObject> m_ArrowDict = new Dictionary<Int64, GameObject>();    
    private List<GameObject> m_monsterArrowList = new List<GameObject>();

    private float[] m_criticalAngel = { 0, 0, 0, 0 };

    private float arrowPositionX = 0;
    private float arrowPositionY = 0;
    private const float PiexlHeight = 640f; //ngui视口高度    

    private float RestrictionsWidth = 0.35f;
    private float RestrictionsHeight = 0.3f;

    private static ArrowManager m_instance;
    public static ArrowManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType(typeof(ArrowManager)) as ArrowManager;
            }
            return m_instance;
        }
    }

    void OnDestroy()
    {
        m_instance = null;
    }

    public void Start()
    {
        m_instance = this;

        InitCriticalPoint();
    }

    [ContextMenu("ReSetCriticalPoint")]
    public void ReSetCriticalPoint()
    {
        InitCriticalPoint();
    }

    //[ContextMenu("MedalSet")]
    //public void SetMedal()
    //{
    //    MedalManager.Instance.SetHeroMedal(true);
    //}

    private void InitCriticalPoint()
    {
        float width = Screen.width;
        float height = Screen.height;

        //箭头的临边位置
        //arrowPositionX = width * 0.5f ;
        //arrowPositionY = height * 0.5f ;
        arrowPositionX = width / height * PiexlHeight * RestrictionsWidth;
        arrowPositionY = PiexlHeight * RestrictionsHeight;

        float sAngle = Mathf.Atan2(height, width) * Mathf.Rad2Deg;
        float bAngle = 90 - sAngle;
        //TraceUtil.Log("sAngle = " + sAngle + "  , bAngle = " + bAngle);

        m_criticalAngel[0] = sAngle;
        m_criticalAngel[1] = m_criticalAngel[0] + bAngle * 2;
        m_criticalAngel[2] = m_criticalAngel[1] + sAngle * 2;
        m_criticalAngel[3] = m_criticalAngel[2] + bAngle * 2;
    }


    public Vector3 Judge(float posX, float posY, float angle)
    {
        Vector3 arrowPos = Vector3.zero;
        float screenAngle = angle;
        if (screenAngle < 0)
        {
            screenAngle += 360;
        }
        //TraceUtil.Log(angle);

        if (screenAngle >= m_criticalAngel[0] && screenAngle < m_criticalAngel[1])
        {
            //TraceUtil.Log("上");
            //float x = 0.5f * arrowPositionY / Mathf.Tan(angle * Mathf.Deg2Rad);
            //arrowPos = new Vector3(x, arrowPositionY, 0);
            float x = posX / posY * arrowPositionY;
            arrowPos = new Vector3(x, arrowPositionY, 0);
        }
        else if (screenAngle >= m_criticalAngel[1] && screenAngle < m_criticalAngel[2])
        {
            //TraceUtil.Log("左");
            //float y = 0.5f * arrowPositionX / Mathf.Tan(angle * Mathf.Deg2Rad);
            //arrowPos = new Vector3(arrowPositionX, y, 0);
            float y = posY / posX * arrowPositionX;
            arrowPos = new Vector3(arrowPositionX, y, 0) * -1;
        }
        else if (screenAngle >= m_criticalAngel[2] && screenAngle < m_criticalAngel[3])
        {
            //TraceUtil.Log("下");
            //float x = 0.5f * arrowPositionY / Mathf.Tan(angle * Mathf.Deg2Rad);
            //arrowPos = new Vector3(x, arrowPositionY, 0);
            float x = posX / posY * arrowPositionY;
            arrowPos = new Vector3(x, arrowPositionY, 0) * -1;
        }
        else
        {
            //TraceUtil.Log("右");
            //float y = 0.5f * arrowPositionX / Mathf.Tan(angle * Mathf.Deg2Rad);
            //arrowPos = new Vector3(arrowPositionX, y, 0);
            float y = posY / posX * arrowPositionX;
            arrowPos = new Vector3(arrowPositionX, y, 0) ;
        }
        //TraceUtil.Log(arrowPos);
        return arrowPos;
    }

    public void AddMonsterArrowt(GameObject targetGameObj)
    {
        SkinnedMeshRenderer childSMRenderer = targetGameObj.GetComponentInChildren<SkinnedMeshRenderer>();
        if (childSMRenderer == null)
            return;
        ArrowBehaviour arrowBehaviour = childSMRenderer.gameObject.AddComponent<ArrowBehaviour>();
        GameObject arrowGameObj = (GameObject)GameObject.Instantiate(MonsterArrowPrefab);
        arrowBehaviour.SetArrow(arrowGameObj.transform);
        RecordMosterArrow(arrowGameObj);
    }

    public void AddChunnelArrow(GameObject targetGameObj)
    {
        MeshRenderer[] meshRenderers = targetGameObj.GetComponentsInChildren<MeshRenderer>();
        //MeshRenderer meshRenderer = targetGameObj.GetComponentInChildren<MeshRenderer>();
        ChunnalArrowBehaviour arrowBehaviour = meshRenderers[1].gameObject.AddComponent<ChunnalArrowBehaviour>();
        GameObject arrowGameObj = (GameObject)GameObject.Instantiate(ChunnelArrowPrefab);
        arrowBehaviour.SetArrow(arrowGameObj.transform, targetGameObj.transform);
    }

    public void AddTeammateArrow(GameObject targetGameObj)
    {
        SkinnedMeshRenderer childSMRenderer = targetGameObj.GetComponentInChildren<SkinnedMeshRenderer>();
        if (childSMRenderer == null)
            return;
        ArrowBehaviour arrowBehaviour = childSMRenderer.gameObject.AddComponent<ArrowBehaviour>();
        GameObject arrowGameObj = (GameObject)GameObject.Instantiate(TeammateArrowPrefab);
        arrowBehaviour.SetArrow(arrowGameObj.transform);
    }

    //public void JudgeToShowArrows()
    //{
    //    bool isShowAllArrows = false;
    //    var monsterList = MonsterManager.Instance.GetMonstersList();
    //    int length = monsterList.Count;
    //    for (int i = 0; i < length; i++)
    //    {
    //        if (((MonsterBehaviour)monsterList[i].Behaviour).IsInvisible() == false)
    //        {
    //            break;
    //        }
    //    }        
    //}

    public bool IsCanAddMonsterArrow()
    {
        int showNum = 0;
        for (int i = 0; i < m_monsterArrowList.Count; i++)
        {
            if (m_monsterArrowList[i] == null)
            {
                m_monsterArrowList.RemoveAt(i);
            }
            else
            {
                if (m_monsterArrowList[i].activeInHierarchy)
                {
                    showNum++;
                }
            }
        }
        return showNum < 2;
    }
    private void RecordMosterArrow(GameObject GO)
    {
        m_monsterArrowList.Add(GO);
    }

    public class MonsterArrow
    {
        public Int64 MonsterID;
        public GameObject MonsterGameObj;
    }
}
