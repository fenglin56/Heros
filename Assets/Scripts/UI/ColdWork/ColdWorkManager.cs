using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// EnterPoint Scene ColdWorkManager
/// </summary>
public class ColdWorkManager : View, ISingletonLifeCycle
{
    private static ColdWorkManager m_instance;
    public static ColdWorkManager Instance { get { return m_instance; } }
    public List<ColdWorkInfo> ColdWorkList = new List<ColdWorkInfo>();

    void Awake()
    {
        m_instance = this;

        //InvokeRepeating("CheckColdItem", 1f, 0.3f);   //注释，全部由网络那边控制
        //AddEventHandler(EventTypeEnum.ColdWork.ToString(), AddColdWork);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.NewColdWorkFromSever,AddColdWork);
    }

    void Start()
    {
        SingletonManager.Instance.Add(this,false); 
    }

    void AddColdWork(object obj)
    {        
        SmsgActionColdWork smsgActionColdWork = (SmsgActionColdWork) obj;
        //smsgActionColdWork.sMsgActionColdWork_SCs.ApplyAllItem(p =>
        //    {
        //        Debug.LogWarning("[添加冷却事件]class = " + p.byClassID + "," + p.dwColdID + "," + p.dwColdTime);
        //    });
        
        foreach (var child in smsgActionColdWork.sMsgActionColdWork_SCs)
        {
            bool isContains= false;
            for (int i = 0; i < ColdWorkList.Count; i++)
            {
                if (ColdWorkList[i].ColdID == child.dwColdID && (byte)ColdWorkList[i].ColdClass == child.byClassID)
                {
                    isContains = true;
                    if (child.dwColdTime == 0)
                    {
                        ColdWorkList.RemoveAt(i);
                        UIEventManager.Instance.TriggerUIEvent(UIEventType.RemoveColdWork, child);
                    }
                    else
                    {
                        ColdWorkInfo coldWorkItem = new ColdWorkInfo(smsgActionColdWork.sMsgActionColdWorkHead_SC.lMasterID,
                                    child.byClassID, child.dwColdID, child.dwColdTime);
                        ColdWorkList[i] = coldWorkItem;
                        UIEventManager.Instance.TriggerUIEvent(UIEventType.AddColdWork, coldWorkItem); 
                        //ColdWorkList[i].ColdTime = child.dwColdTime;
                    }
                }
            }
            if (!isContains)
            {
                ColdWorkInfo newColdWorkItem = new ColdWorkInfo(smsgActionColdWork.sMsgActionColdWorkHead_SC.lMasterID,
                            child.byClassID, child.dwColdID, child.dwColdTime);
                ColdWorkList.Add(newColdWorkItem);
                UIEventManager.Instance.TriggerUIEvent(UIEventType.AddColdWork, newColdWorkItem);
            }
        }        
    }

    public void AddColdWorkInfo(long UID, ColdWorkClass coldWorkClass, uint ColdID, uint coldTime)
    {
        ColdWorkInfo coldWorkInfo = new ColdWorkInfo(UID,(byte)coldWorkClass,ColdID,coldTime);
        ColdWorkList.Add(coldWorkInfo);
    }

    void CheckColdItem()
    {
        //Debug.LogWarning("CheckColdWork");
        if (ColdWorkList.Count <= 0)
            return;
        for (int i = ColdWorkList.Count - 1; i >= 0; i--)
        {
            if (ColdWorkList[i].ColdTimeEnd <= Time.realtimeSinceStartup)
            {
                //当前物品冷却结束
                //Debug.LogWarning("物品冷却结束："+ColdWorkList[i].ColdID);
                UIEventManager.Instance.TriggerUIEvent(UIEventType.ColdWorkComplete, null);
                ColdWorkList.RemoveAt(i);
            }
        }
    }

    public ColdWorkInfo GetColdWorkInfo(long MasterID, ColdWorkClass coldWorkClass, uint coldID)
    {
        //TraceUtil.Log("[GetColdWorkInfo]MasterID=" + MasterID + " , coldWorkClass=" + coldWorkClass + " , coldID=" + coldID);
        foreach (ColdWorkInfo child in ColdWorkList)
        {            
            if (child.lMasterID == MasterID && child.ColdClass == coldWorkClass && child.ColdID == coldID)
            {
                //Debug.LogWarning("已经获取了冷却信息：" + coldID );                
                return child;
            }
        }
        //Debug.LogWarning("获取冷却信息：" + coldID + ",IsNull");
        return null;
    }

    public ColdWorkInfo GetColdWorkInfoClone(long MasterID, ColdWorkClass coldWorkClass, uint coldID)
    {
        foreach (ColdWorkInfo child in ColdWorkList)
        {            
            if (child.lMasterID == MasterID && child.ColdClass == coldWorkClass && child.ColdID == coldID)
            {                
                ColdWorkInfo clone = new ColdWorkInfo(child.lMasterID, (byte)child.ColdClass, child.ColdID, 0);
                clone.ColdTime = (uint)((child.ColdTime - (Time.realtimeSinceStartup - child.ColdTimeStart) * 1000));
                //TraceUtil.Log("[GetColdWorkInfo]ColdTime=" + clone.ColdTime + " , realtimeSinceStartup=" + Time.realtimeSinceStartup + " , ColdTimeStart=" + clone.ColdTimeStart);
                return clone;
            }
        }
        return null;
    }

    protected override void RegisterEventHandler()
    {
    }

    public void Instantiate()
    {
    }

    public void LifeOver()
    {

        ColdWorkList.Clear();
        //m_instance = null;
    }
}

public class ColdWorkInfo
{
    public long lMasterID;
    public float ColdTimeStart;
    public float ColdTimeEnd;
    public ColdWorkClass ColdClass;
    public uint ColdID;
    public uint ColdTime;

    public ColdWorkInfo(long masterID,byte byClass,uint coldID,uint coldTime)
    {
        this.lMasterID = masterID;
        this.ColdTimeStart = Time.realtimeSinceStartup;
        this.ColdTimeEnd = Time.realtimeSinceStartup + coldTime / 1000f;
        this.ColdClass = (ColdWorkClass)byClass;
        this.ColdID = coldID;
        this.ColdTime = coldTime;
    }
}

public enum ColdWorkType
{
    ContainerColdWork = 500001,//物品栏冷却
    GetXiuLianColdWork = 500002,//领取修为冷却
}

public enum ColdWorkClass
{
    Goods = 1,//物品冷却
    Chat = 2,//聊天
    Task = 3,//人物
    Skill = 4,//技能
    ChatLevel1 = 5,
    ChatLevel2 = 6,
    ChatLevel3 = 7,
    ChatLevel4 = 8,
    Meridians = 9,
    Online = 10,
    SirenEctype = 11,//封妖副本
    ECold_ClassID_SpecialSkill,					// 特殊技能组，如爆气
    ECold_ClassID_MODEL,						// 功能模块的冷却
    ECold_ClassID_Max,							// 最大
}
