using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuideConfigManager : MonoBehaviour {

    //新的任务系统
    public NewGuideConfigDataBase NewGuideConfigFile;
    public TaskNewConfigDataBase TaskNewConfigFile;
    public TalkIdConfigDataBase TalkIdConfig;

    public EctGuideStepConfigDataBase EctGuideStepConfigData;
    public EctGuideTalkDataBase EctGuideTalkDataBase;
    //新手引导配置文件
    public NewbieGuideConfigDataBase GuideConfigFile;
    //public EctypeHelperConfigDataBase EctypeHelperConfigFile;
    //public TaskConfigDataBase TaskConfigFile;
    public EctypeGuideConfigDataBase EctypeGuideConfigFile;
    //public EctypeGuideStepConfigDataBase EctypeGuideStepConfigFile;

    private Dictionary<int, GuideConfigData> m_guideConfigList = new Dictionary<int, GuideConfigData>();
    //private List<EctypeHelperConfigData> m_curEctypeHelperList = new List<EctypeHelperConfigData>();
    private List<TaskConfigData> m_taskConfigList = new List<TaskConfigData>();
    private Dictionary<int, EctypeGuideConfigData> m_ectypeGuideList = new Dictionary<int, EctypeGuideConfigData>();
    private Dictionary<int, EctypeGuideStepConfigData> m_ectypeStepList = new Dictionary<int, EctypeGuideStepConfigData>();

    private static GuideConfigManager m_instance;
    public static GuideConfigManager Instance
    {
        get
        {
            if (null == m_instance)
                m_instance = new GuideConfigManager();
            return m_instance;
        }
    }

    // Use this for initialization
    void Awake()
    {
        m_instance = this;

        //创建新手引导对应配置列表
        if (null == GuideConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"NewbieGuideConfigDataBase没有指定新手引导的配置文件！");
        }
        else
        {
            foreach (GuideConfigData element in GuideConfigFile._dataTable)
            {
                m_guideConfigList.Add(element._GuideID, element);
            }
        }

        //创建新手引导对应配置列表
        //if (null == EctypeHelperConfigFile)
        //{
        //    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"EctypeGuideConfigDataBase没有指定新手引导的配置文件！");
        //}
        //else
        //{
        //    foreach (EctypeHelperConfigData element in EctypeHelperConfigFile._dataTable)
        //    {
        //        m_curEctypeHelperList.Add(element);
        //    }
        //}

        //创建新手引导对应配置列表
        //if (null == TaskConfigFile)
        //{
        //    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"没有指定任务的配置文件！");
        //}
        //else
        //{
        //    foreach (TaskConfigData element in TaskConfigFile._dataTable)
        //    {
        //        m_taskConfigList.Add(element);
        //    }
        //}

        //创建副本引导对应配置列表
        if (null == EctypeGuideConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"没有指定副本引导的配置文件！");
        }
        else
        {
            foreach (EctypeGuideConfigData element in EctypeGuideConfigFile._dataTable)
            {
                m_ectypeGuideList.Add(element._EctypeID, element);
            }
        }

        //创建副本引导步对应配置列表
        //if (null == EctypeGuideStepConfigFile)
        //{
        //    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"没有指定副本引导的配置文件！");
        //}
        //else
        //{
        //    foreach (EctypeGuideStepConfigData element in EctypeGuideStepConfigFile._dataTable)
        //    {
        //        m_ectypeStepList.Add(element._GuideStep, element);
        //    }
        //}
    }

    public List<TaskConfigData> TaskConfigList
    {
        get { return m_taskConfigList; }
    }

    public Dictionary<int, GuideConfigData> TownGuideConfigList
    {
        get { return m_guideConfigList; }
    }

    //public List<EctypeHelperConfigData> EctypeHelperConfigList
    //{
    //    get { return m_curEctypeHelperList; }
    //}

    public Dictionary<int, EctypeGuideConfigData> EctypeGuideConfigList
    {
        get { return m_ectypeGuideList; }
    }

    public Dictionary<int, EctypeGuideStepConfigData> EctypeGuideStepConfigList
    {
        get { return m_ectypeStepList; }
    }

    /// <summary>
    /// 副本引导的当前列表数据
    /// </summary>
    /// <param name="ectypeID"></param>
    /// <returns></returns>
    //public List<EctypeGuideConfigData> GetCurEctypeIDDataList(uint ectypeID)
    //{
    //    m_curEctypeIDGuideList.Clear();

    //    //foreach (EctypeGuideConfigData item in m_curEctypeGuideList)
    //    //{
    //    //    if (item._EctypeID == ectypeID)
    //    //        m_curEctypeIDGuideList.Add(item);
    //    //}

    //    return m_curEctypeIDGuideList;
    //}

    ///// <summary>
    ///// 城镇新手引导
    ///// </summary>
    ///// <param name="ectypeID"></param>
    ///// <returns></returns>
    //public List<GuideConfigData> GetCurEctypeDataList(uint ectypeID)
    //{
    //    m_curEctypeList.Clear();

    //    //foreach (GuideConfigData item in m_guideConfigList)
    //    //{
    //    //    if (item._GuideID == ectypeID)
    //    //        m_curEctypeList.Add(item);
    //    //}

    //    return m_curEctypeList;
    //}

    ///// <summary>
    ///// 城镇新手引导获取当前引导阶段的数据
    ///// </summary>
    ///// <param name="level"></param>
    ///// <returns></returns>
    //public List<GuideConfigData> GetCurLevelDataList(int level)
    //{
    //    m_curGuideLevelList.Clear();

    //    foreach (GuideConfigData item in m_curEctypeList)
    //    {
    //        if (item._GuideLevel == level)
    //        {
    //            m_curGuideLevelList.Add(item);
    //        }
    //    }

    //    return m_curGuideLevelList;
    //}
}
