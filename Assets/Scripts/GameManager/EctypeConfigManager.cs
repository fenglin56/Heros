using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EctypeConfigManager : MonoBehaviour {
    
    //场景配置文件
    public SceneConfigDataBase SceneConfigFile;    
    private Dictionary<int, SceneConfigData> m_sceneConfigList = new Dictionary<int, SceneConfigData>();    
    
    //副本UI选择配置文件
    public EctypeSelectConfigDataBase EctypeSelectConfigFile;
    private Dictionary<int, EctypeSelectConfigData> m_ectypeSelectConfigList = new Dictionary<int, EctypeSelectConfigData>();

    public EctypeContainerDataList EctypeContainerConfigFile;
    private Dictionary<int, EctypeContainerData> m_ectypeContainerConfigList = new Dictionary<int, EctypeContainerData>();

    //场景传送门配置文件
    public PortalConfigDataBase PortalConfigFile;
    private Dictionary<int, PortalConfigData> m_portalConfigList = new Dictionary<int, PortalConfigData>();

    //陷阱配置文件
    public TrapConfigDataBase TrapConfigFile;
    private Dictionary<int, TrapConfigData> m_trapConfigList = new Dictionary<int, TrapConfigData>();

    //破坏物配置文件
    public DamageConfigDataBase DamageConfigFile;
    private Dictionary<int, DamageConfigData> m_damageConfigList = new Dictionary<int, DamageConfigData>();

	//无尽模式副本奖励表
	public EndLessEctypeConfigDataBase endLessRewardFile;
	private List<EndLessEctypeConfigData> m_endLessRewardList = new List<EndLessEctypeConfigData> ();

	//讨伐副本首次奖励
	public FirstBattlePrizeDataBase FirstBattlePrizeFile;
	private Dictionary<int, FirstBattlePrizeData> m_firstBattlePrizeDict = new Dictionary<int, FirstBattlePrizeData>();

    private static EctypeConfigManager m_instance;
    public static EctypeConfigManager Instance
    {
        get
        {
            return m_instance;
        }
    }
        
	// Use this for initialization
	void Awake () {
        m_instance = this;

        InitSceneConfig();          //初始场景配置
        InitEctypeSelectConfig();   //初始副本选择配置
        InitEctypeContainerConfig();//初始副本配置
        InitPortalConfig();         //初始传送门配置文件
        //InitWorldMapConfig();       //初始世界地图配置文件
        InitDamageConfig();         //初始可破坏配置文件
		InitEndLessRewardConfig();         //无尽模式副本奖励表
		InitFirstBattlePrize();		//讨伐副本每日首次奖励
	}

    void InitSceneConfig()
    {
        //创建场景名称对应配置列表
        if (null == SceneConfigFile)
        {
            TraceUtil.Log("SceneConfigData没有指定场景的配置文件！");
        }
        else
        {
            foreach (SceneConfigData element in SceneConfigFile._dataTable)
            {
                m_sceneConfigList.Add(element._lMapID, element);
            }
        }
    }

    void InitEctypeSelectConfig()
    {
        //创建副本选择UI对应配置列表
        if (null == EctypeSelectConfigFile)
        {
            TraceUtil.Log("EctypeSelectData没有指定副本UI选择的配置文件！");
        }
        else
        {
            foreach (EctypeSelectConfigData element in EctypeSelectConfigFile._dataTable)
            {
                m_ectypeSelectConfigList.Add(element._lEctypeID, element);
            }
        }
    }

    void InitEctypeContainerConfig()
    {
        if (null == EctypeContainerConfigFile)
        {
            TraceUtil.Log("EctypeContainerConfigFile没有关联进来");
        }
        else
        {
            foreach (EctypeContainerData child in EctypeContainerConfigFile.ectypeContainerDataList)
            {
				child.Init();
                m_ectypeContainerConfigList.Add(child.lEctypeContainerID,child);
            }
        }
    }

    void InitPortalConfig()
    {
        //创建场景传送门对应配置列表
        if (null == PortalConfigFile)
        {
            TraceUtil.Log("PortalConfigData没有指定传送门的配置文件！");
        }
        else
        {
            foreach (PortalConfigData element in PortalConfigFile._dataTable)
            {
                m_portalConfigList.Add(element._SID, element);
            }
        }
    }



    void InitTrapConfig()
    {
        //创建陷阱的对应配置列表
        if (null == TrapConfigFile)
        {
            TraceUtil.Log("TrapConfigDataBase没有指定配置文件！");
        }
        else
        {
            foreach (TrapConfigData element in TrapConfigFile._dataTable)
            {
                m_trapConfigList.Add(element._TrapID, element);
            }
        }
    }

    void InitDamageConfig()
    {
        //创建可破坏的对应配置列表
        if (null == DamageConfigFile)
        {
            TraceUtil.Log("DamageConfigDataBase没有指定配置文件！");
        }
        else
        {
            foreach (DamageConfigData element in DamageConfigFile._dataTable)
            {
                m_damageConfigList.Add(element._damageID, element);
            }
        }
    }

	void InitEndLessRewardConfig()
	{
		//创建可破坏的对应配置列表
		if (null == endLessRewardFile)
		{
			TraceUtil.Log("DamageConfigDataBase没有指定配置文件！");
		}
		else
		{
			foreach (EndLessEctypeConfigData element in endLessRewardFile._dataTable)
			{
				element.GetReward();
				m_endLessRewardList.Add(element);
			}
		}
	}

	void InitFirstBattlePrize()
	{
		if(null == FirstBattlePrizeFile)
		{
			TraceUtil.Log(SystemModel.Common, TraceLevel.Error,"DamageConfigDataBase没有指定配置文件！");
		}
		else
		{
			foreach (FirstBattlePrizeData data in FirstBattlePrizeFile._dataTable)
			{
				m_firstBattlePrizeDict.Add(data.UserLevel,data);
			}
		}
	}

    /// <summary>
    /// 获取场景配置列表
    /// </summary>
    public Dictionary<int, SceneConfigData> SceneConfigList
    {
        get { return this.m_sceneConfigList; }
    }

    /// <summary>
    /// 获取副本UI选择配置列表
    /// </summary>
    public Dictionary<int, EctypeSelectConfigData> EctypeSelectConfigList
    {
        get { return this.m_ectypeSelectConfigList; }
    }

    public Dictionary<int, EctypeContainerData> EctypeContainerConfigList
    {
        get { return this.m_ectypeContainerConfigList; }
    }

    /// <summary>
    /// 获取传送门配置列表
    /// </summary>
    public Dictionary<int, PortalConfigData> PortalConfigList
    {
        get { return this.m_portalConfigList; }
    }

    /// <summary>
    /// 获取陷阱配置列表
    /// </summary>
    public Dictionary<int, TrapConfigData> TrapConfigList
    {
        get { return this.m_trapConfigList; }
    }

    /// <summary>
    /// 获取可破坏物配置列表
    /// </summary>
    public Dictionary<int, DamageConfigData> DamageConfigList
    {
        get { return this.m_damageConfigList; }
    }

	/// <summary>
	/// 无尽模式副本奖励表
	/// </summary>
	public List<EndLessEctypeConfigData> EndLessRewardList
	{
		get { return this.m_endLessRewardList; }
	}

	/// <summary>
	/// 反查副本区域ID
	/// </summary>
	/// <returns>区域id</returns>
	/// <param name="ectypeID">副本id</param>
	public int GetSelectContainerID(int ectypeID)
	{
		int id = 0;
		m_ectypeSelectConfigList.Values.ToList().ApplyAllItem(p=>{
			if(p._vectContainer.LocalContains(ectypeID))
			{
				id = p._lEctypeID;
			}
		});
		return id;
	}

	public FirstBattlePrizeData GetFirstBattlePrizeData(int level)
	{
		FirstBattlePrizeData data = null;
		m_firstBattlePrizeDict.TryGetValue(level,out data);
		return data;
	}

}
