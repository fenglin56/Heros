using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// BattleUI Scene BattleDataManager 
/// </summary>
public class EctypeResDataManager : MonoBehaviour
{
    public EctypeContainerResDataBase EctypeContainerResDataList;
    private Dictionary<int, EctypeContainerResData> m_ectypeContainerResDataDict = new Dictionary<int, EctypeContainerResData>();

    private static EctypeResDataManager m_Instance;

    void Awake()
    {
        foreach (EctypeContainerResData child in EctypeContainerResDataList._dataTable)
        {
            m_ectypeContainerResDataDict.Add(child.lEctypeContainerID, child);
        }
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    public static EctypeResDataManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(EctypeResDataManager)) as EctypeResDataManager;
            }
            return m_Instance;
        }
    }

    public EctypeContainerResData GetEctypeContainerResData(int ectypeContainerID)
    {
        EctypeContainerResData resData;
        m_ectypeContainerResDataDict.TryGetValue(ectypeContainerID, out resData);        
        return resData;
    }

}
