using UnityEngine;
using System.Collections.Generic;

public class NPCConfigManager : MonoBehaviour {

    //NPC闲话配置文件
    public NPCTalkConfigDataBase NPCTalkConfigFile;
    private Dictionary<int, NPCTalkConfigData> m_npcTalkConfigList = new Dictionary<int, NPCTalkConfigData>();

    //NPC配置文件
    public NPCConfigDataBase NPCConfigFile;
    private Dictionary<int, NPCConfigData> m_npcConfigList = new Dictionary<int, NPCConfigData>();

    //NPC特殊功能表
    public NPCSpecialConfigDataBase NPCSpecialConfigFile;
    private List<NPCSpecialConfigData> m_npcSpecialConfigList = new List<NPCSpecialConfigData>();
    
    //

    private static NPCConfigManager m_instance;
    public static NPCConfigManager Instance
    {
        get
        {
            if (null == m_instance)
                m_instance = new NPCConfigManager();
            return m_instance;
        }
    }

    // Use this for initialization
    void Awake()
    {
        m_instance = this;

        //创建NPC闲话对应配置列表
        if (null == NPCTalkConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"NPCTalkConfigData没有指定NPC对话的配置文件！");
        }
        else
        {
            foreach (NPCTalkConfigData element in NPCTalkConfigFile._dataTable)
            {
                m_npcTalkConfigList.Add(element._SID, element);
            }
        }

        //创建NPC闲话对应配置列表
        if (null == NPCConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"NPCConfigDataBase没有指定NPC的配置文件！");
        }
        else
        {
            foreach (NPCConfigData element in NPCConfigFile._dataTable)
            {
                m_npcConfigList.Add(element._NPCID, element);
            }
        }

        //创建NPC特殊功能配置表
        if (null == NPCSpecialConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"NPCSpecialConfigDataBase没有指定NPC特殊功能的配置文件！");
        } 
        else
        {
            foreach (NPCSpecialConfigData element in NPCSpecialConfigFile._dataTable)
            {
                m_npcSpecialConfigList.Add(element);
            }
        }

    }

    void OnDestroy()
    {
        m_instance = null;
    }

    /// <summary>
    /// 获得NPC闲话配置列表
    /// </summary>
    public Dictionary<int, NPCTalkConfigData> NPCTalkConfigList
    {
        get { return this.m_npcTalkConfigList; }
    }

    //获得NPC配置列表
    public Dictionary<int, NPCConfigData> NPCConfigList
    {
        get { return this.m_npcConfigList; }
    }

    //获得NPC特殊功能配置列表
    public List<NPCSpecialConfigData> NPCSpecialConfigList
    {
        get { return this.m_npcSpecialConfigList; }
	}
	
	public NPCSpecialConfigData GetNpcSpecialConfigData(int funcType)
	{
		NPCSpecialConfigData nSCdata = null;
		foreach(NPCSpecialConfigData data in NPCSpecialConfigList)
		{
			if(data._FunctionType.Equals(funcType))
			{
				nSCdata = data;
				break;
			}
		}
		return nSCdata;
	}
}
