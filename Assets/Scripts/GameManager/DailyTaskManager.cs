using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DailyTaskManager : ISingletonLifeCycle
{
    private static DailyTaskManager m_instance;
    public static DailyTaskManager Instance
    {
        get 
        {
            if (m_instance == null)
            {
                m_instance = new DailyTaskManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance; 
        }
    }

    private Dictionary<int, DailyTaskLog> m_taskLogDict = new Dictionary<int, DailyTaskLog>();
	private int m_rewardProcess;
	public int RewardProcess{get{return m_rewardProcess;}}


    public void UpdateDailyTaskData(STaskLogUpdate sTaskLogUpdate, STaskLogContext sTaskLogContext)
    {
        //if (m_taskLogDict.ContainsKey(sTaskLogUpdate.nTaskID))
        //{
        //    TraceUtil.Log("[ContainsKey] " + sTaskLogUpdate.nTaskID);
        //    m_taskLogDict[sTaskLogUpdate.nTaskID] = sTaskLogUpdate;
            
        //}
        //else
        //{
        //    TraceUtil.Log("[AddKey] " + sTaskLogUpdate.nTaskID);
        //    m_taskLogDict.Add(sTaskLogUpdate.nTaskID, sTaskLogUpdate);                        
        //}
        m_taskLogDict[sTaskLogUpdate.nTaskID] = new DailyTaskLog()
        {
            LogUpdate = sTaskLogUpdate,
            LogContext = sTaskLogContext
        };
    }

	public void UpdateProcess(int value)
	{
		this.m_rewardProcess = value;
		if(DailyTaskDataManager.Instance != null)
		{
			DailyTaskDataManager.Instance.CheckNewChest(m_rewardProcess);
		}
	}

    public DailyTaskLog[] GetTaskLogList()
    {
        return m_taskLogDict.Values.ToArray(); 
    }



    public class DailyTaskLog
    {
        public STaskLogUpdate LogUpdate;
        public STaskLogContext LogContext;
    }


    public void Instantiate()
    {
    }

    public void LifeOver()
    {
        m_instance = null;
    }
}
