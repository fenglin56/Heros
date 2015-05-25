using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum DataType
{  
    GameBufferData,//Buffer
    GameConfigData,//游戏设置信息

    LoadingSceneData,//加载场景信息

    SetUIActive,//设置UI

    ActorSelector,  //打开创建角色田界面
    EctypeTreasureReward,//结算翻牌消息

    LoadingSceneComplete,//加载场景完成消息

    InitializeEctype,//副本初始化

    //LastRoleLevel,//角色上一次保存的等级

    SingleTrialsSettlement,//单个试炼副本波数结算

    SelectRoleData,//选择的人物信息
    MissionFail,//副本失败
	CountDownUI,//副本结算
	DefenceEctypeResult,// 防守副本结算;
}

public class GameDataManager //: ISingletonLifeCycle
{

    private static GameDataManager m_instance;
    public static GameDataManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new GameDataManager();
                //SingletonManager.Instance.Add(m_instance); 
            }
            return m_instance;
        }
    }

    Dictionary<DataType, GameData> DataDitionary;
    public DataEvent dataEvent;

    public GameDataManager()
    {
        DataDitionary = new Dictionary<DataType, GameData>();
        dataEvent = new DataEvent();
    }

    public void AddData(DataType dataType, object obj)
    {
        GameData DataValue;
        if (DataDitionary.TryGetValue(dataType, out DataValue))
        {
            DataValue.AddData(obj);
        }
        else
        {
            DataDitionary.Add(dataType, new GameData(obj));
        }
        dataEvent.TriggerEvent(dataType,obj);
    }

    public void ResetData(DataType dataType,object obj)
    {
        ClearData(dataType);
        AddData(dataType,obj);
    }

    public object GetData(DataType DataType)
    {
        GameData DataValue;
        if (DataDitionary.TryGetValue(DataType, out DataValue))
        {
            return DataValue.GetData();
        }
        else
        {
            return null;
        }
    }

    public object PeekData(DataType DataType)
    {
        GameData DataValue;
        if (DataDitionary.TryGetValue(DataType, out DataValue))
        {
            return DataValue.PeekData();
        }
        else
        {
            return null;
        }
    }

    public bool DataIsNull(DataType dataType)
    {
        return PeekData(dataType) == null;
    }

    public void ClearData(DataType dataType)
    {
        GameData DataValue;
        if (DataDitionary.TryGetValue(dataType, out DataValue))
        {
            DataValue.CleaData();
        }
    }

    public object[] GetDataList(DataType DataType)
    {
        GameData DataValue;
        if (DataDitionary.TryGetValue(DataType, out DataValue))
        {
            return DataValue.GetAllData();
        }
        else
        {
            return null;
        }
    }

    public void ClearAllData()
    {
        DataDitionary.ApplyAllItem(P=>P.Value.CleaData());
        DataDitionary.Clear();
    }

    public void Instantiate()
    {
    }

    public void LifeOver()
    {
        ClearAllData();
        dataEvent.CleanUpEvent();
        m_instance = null;
    }
}
public class GameData
{
    Queue DataList;

    public GameData(object obj)
    {
        DataList = new Queue();
        AddData(obj);
    }

    public void AddData(object obj)
    {
        if (obj != null)
        {
            DataList.Enqueue(obj);
        }
    }

    public object GetData()
    {
        if (DataList.Count > 0)
        {
            return DataList.Dequeue();
        }
        else
        {
            return null;
        }
    }

    public object PeekData()
    {
        if (DataList.Count > 0)
        {
            return DataList.Peek();
        }
        else
        {
            return null;
        }
    }

    public object[] GetAllData()
    {
        return DataList.ToArray();
    }

    public void CleaData()
    {
        DataList.Clear();
    }

}

    public class DataEvent
    {

        Dictionary<DataType, DataEventClass> EventDictionary;

        public DataEvent()
        {
            this.EventDictionary = new Dictionary<DataType, DataEventClass>();
        }

        public void CleanUpEvent()
        {
            EventDictionary.Clear();
        }

        public void TriggerEvent(DataType MyEventType, object EventObj)
        {
            DataEventClass GetValue;
            if (EventDictionary.TryGetValue(MyEventType, out GetValue))
            {
                GetValue.RaiseEvent(EventObj);
            }
        }

        public void RegisterEvent(DataType MyEventType, DataEventDelegate dataEventDelegate)
        {
            DataEventClass GetValue;
            if (EventDictionary.TryGetValue(MyEventType,out GetValue))
            {
                GetValue.AddEvent(dataEventDelegate);
            }
            else
            {
                EventDictionary.Add(MyEventType, new DataEventClass(dataEventDelegate));
            }
        }

        public void RemoveEventHandel(DataType MyEventType, DataEventDelegate dataEventDelegate)
        {
            DataEventClass GetValue;
            if (EventDictionary.TryGetValue(MyEventType, out GetValue))
            {
                GetValue.RemoveEvent(dataEventDelegate);
            }
        }

    }
    public delegate void DataEventDelegate(object uiEventInsatance);
    public class DataEventClass
    {
        public event DataEventDelegate UIEvent;

        public DataEventClass(DataEventDelegate dataEventDelegate)
        {
            AddEvent(dataEventDelegate);
        }

        public void AddEvent(DataEventDelegate dataEventDelegate)
        {
            this.UIEvent += dataEventDelegate;
        }

        public void RemoveEvent(DataEventDelegate dataEventDelegate)
        {
            this.UIEvent -= dataEventDelegate;
        }

        public void RaiseEvent(object EventObj)
        {
            if (this.UIEvent != null)
            {
                this.UIEvent(EventObj);
            }
        }
    }
