using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class TraceConfigData
{
    public SystemModel SystemModel;
    public TraceLevel TraceLevel;
}
public class TraceConfigDataBase : ScriptableObject
{
    public bool Off = false;
    public TraceConfigData[] TraceConfigDataTable;

    /// <summary>
    /// 校验日志是否通过配置属性
    /// </summary>
    /// <param name="systemModel"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool PassToTrace(SystemModel systemModel, TraceLevel level)
    {
        bool flag = false;
        if (!Off)
        {
            if (TraceConfigDataTable != null && TraceConfigDataTable.Length > 0)
            {
                for (int i = 0; i < TraceConfigDataTable.Length; i++)
                {
                    if (systemModel == TraceConfigDataTable[i].SystemModel)
                    {
                        if (level == TraceConfigDataTable[i].TraceLevel || TraceConfigDataTable[i].TraceLevel == TraceLevel.Verbose)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
            }
        }
        return flag;
    }
}
