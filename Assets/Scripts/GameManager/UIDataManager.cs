using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDataManager : MonoBehaviour {
    
    public InitMainButtonDataBase InitMainButtonFile;
    public MainButtonConfigDataBase MainButtonConfigFile;

    private List<MainButtonConfigData> m_mainButtonConfigList = new List<MainButtonConfigData>();
    private List<InitMainButtonData> m_initMainButtonList = new List<InitMainButtonData>();
    
    private static UIDataManager m_instance;
    public static UIDataManager Instance
    {
        get { return m_instance; }
    }

    void OnDestroy()
    {
        m_instance = null;
    }

    void Awake()
    {
        m_instance = this;
        if (InitMainButtonFile != null)
        {
            foreach (var item in InitMainButtonFile._dataTable)
            {
                m_initMainButtonList.Add(item);
            }
        }
        else
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"城镇主界面主按钮初始配置文件为空！");


        if (MainButtonConfigFile != null)
        {
            foreach (var item in MainButtonConfigFile._dataTable)
            {
                m_mainButtonConfigList.Add(item);
            }
        }
        else
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"城镇主界面主按钮配置文件为空！");
    }

    /// <summary>
    ///城镇主界面功能按钮配置数据 
    /// </summary>
    public List<MainButtonConfigData> TownMainButtonList
    {
        get { return m_mainButtonConfigList; }
    }

    public List<InitMainButtonData> InitMainButtonList
    {
        get { return m_initMainButtonList; }
    }

}
