using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleConfigManager : MonoBehaviour {

    //Buff配置文件
    public BuffConfigDataBase BuffConfigFile;
    private Dictionary<int, BuffConfigData> m_buffConfigList = new Dictionary<int, BuffConfigData>();

    //怪物配置文件
    public MonsterConfigDataBase MonsterConfigFile;
    private Dictionary<int, MonsterConfigData> m_monsterConfigList = new Dictionary<int, MonsterConfigData>();

    public GameObject CutUpEffect;  //切碎特效

    private static BattleConfigManager m_instance;
    public static BattleConfigManager Instance
    {
        get{ return m_instance; }
    }

    // Use this for initialization
	void Awake () {
        m_instance = this;
        InitBuffConfig();
        InitMonsterConfig();
	}

    /// <summary>
    ///初始化BuffConfig文件 
    /// </summary>
    void InitBuffConfig()
    {
        //创建Buff配置列表
        if (null == BuffConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"BuffConfigFile没有指定Buff的配置文件！");
        }
        else
        {
            foreach (BuffConfigData element in BuffConfigFile._dataTable)
            {
                m_buffConfigList.Add(element._buffID, element);
            }
        }
    }

    void InitMonsterConfig()
    {
        //创建Monster配置列表
        if (null == MonsterConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"MonsterConfigFile没有指定怪物的配置文件！");
        }
        else
        {
            foreach (MonsterConfigData element in MonsterConfigFile._dataTable)
            {
                m_monsterConfigList.Add(element._monsterID, element);
            }
        }
    }

    /// <summary>
    /// 获取Buff配置列表
    /// </summary>
    public Dictionary<int, BuffConfigData> BuffConfigList
    {
        get { return this.m_buffConfigList; }
    }

    /// <summary>
    /// 获取Monster配置列表
    /// </summary>
    public Dictionary<int, MonsterConfigData> MonsterConfigList
    {
        get { return this.m_monsterConfigList; }
    }

    /// <summary>
    /// 获取指定Monster配置信息
    /// </summary>
    /// <param name="monsterID">怪物id</param>
    /// <returns></returns>
    public MonsterConfigData GetMonsterData(int monsterID)
    {
        MonsterConfigData data = null;
        MonsterConfigList.TryGetValue(monsterID, out data);
        return data;
    }
}
