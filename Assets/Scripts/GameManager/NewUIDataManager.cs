using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewUIDataManager : MonoBehaviour {

    public InitMainTownButtonDataBase InitMainButtonFile;
    public MainTownButtonConfigDataBase MainButtonConfigFile;
    
    private List<MainTownButtonConfigData> m_mainButtonConfigList = new List<MainTownButtonConfigData>();
    private List<InitMainTownButtonData> m_initMainButtonList = new List<InitMainTownButtonData>();
    
    private static NewUIDataManager m_instance;
    public static NewUIDataManager Instance
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
            foreach (var item in InitMainButtonFile.Datas)
            {
                m_initMainButtonList.Add(item);
            }
        }
        else
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"城镇主界面主按钮初始配置文件为空！");
        
        
        if (MainButtonConfigFile != null)
        {
            foreach (var item in MainButtonConfigFile.Datas)
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
    public List<MainTownButtonConfigData> TownMainButtonList
    {
        get { return m_mainButtonConfigList; }
    }
    
    public List<InitMainTownButtonData> InitMainButtonList
    {
        get { return m_initMainButtonList; }
    }

}
