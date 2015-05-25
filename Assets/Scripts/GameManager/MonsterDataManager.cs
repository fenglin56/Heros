using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterDataManager : MonoBehaviour 
{
    public MonsterConfigDataBase MonsterConfigData;
    private Dictionary<int, MonsterConfigData> m_monsterConfigDict = new Dictionary<int, MonsterConfigData>();

    private static MonsterDataManager m_instance;
    public static MonsterDataManager Instance { get { return m_instance; } }
    void Awake()
    {
        m_instance = this;
        Load();
    }

    private void Load()
    {
        foreach (MonsterConfigData data in MonsterConfigData._dataTable)
        {
            m_monsterConfigDict[data._monsterID] = data;
        }
    }

    public MonsterConfigData GetMonsterData(int monsterID)
    {
        MonsterConfigData data = null;
        if(m_monsterConfigDict.ContainsKey(monsterID))
        {
            m_monsterConfigDict.TryGetValue(monsterID, out data);
        }
        return data;
    }


}
