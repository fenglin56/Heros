using UnityEngine;
using System.Collections;

/// <summary>
/// EnterPoint Scene ColdWorkManager
/// </summary>
namespace UI.MainUI
{
    public class EnegryColdWorkData : View, ISingletonLifeCycle
    {

        static EnegryColdWorkData instance;
        public static EnegryColdWorkData Instance
        {
            get
            {
                return instance;
            }
        }


        int ChachePassTime = -1;
        int ChachePassEnegry = -1;
        int TargetTime = 0;
        //GameObject FloatObj;
        public int CurrentPassTime { get; private set; }
        bool IsUpdate = false;


        void Start()
        {
            InvokeRepeating("TriggerUpdateTimeEvent", 0.1f, 1);
            SingletonManager.Instance.Add(this,false); 
            TargetTime = CommonDefineManager.Instance.CommonDefineFile._dataTable.VitRecoverTime*60 ;
            instance = this;
            RegisterEventHandler();
            UIEventManager.Instance.RegisterUIEvent(UIEventType.LoadingComplete, UpdateChacheTime);
        }

        public void ResetEventHadels()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(),UpdateColdWorkData);
        }
        public int GetGoldTime()
        {
            return 0;
        }

        void UpdateColdWorkData(INotifyArgs inotifyArgs)
        {
            //TraceUtil.Log("UpdatePlayerValue:" + PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_LASTACTIVE_TIME);
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                UpdateChacheTime(null);
            }
        }

        void UpdateChacheTime(object obj)
        {
            int passTime = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_LASTACTIVE_TIME;
            int enegry = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
            if (ChachePassTime != passTime)
            {
                //Debug.LogWarning("刷新活力值，恢复已过时间:" + passTime+",SysTime:"+System.DateTime.Now);
                //IsUpdate = true;
                ChachePassTime = passTime;
              
                CurrentPassTime = passTime;
                ChachePassEnegry = enegry;
            } 
            if(ChachePassEnegry!=enegry)
            {
                ChachePassTime = 0;
                
                CurrentPassTime = 0;
                ChachePassEnegry = enegry;
            }
        }

        //void LateUpdate()
        //{
        //    if (IsUpdate)
        //    {
        //        UpdateData();
        //        IsUpdate = false;
        //    }
        //}

        //void UpdateData()
        //{
        //    //TraceUtil.Log("UpdateColdWorkData");
        //    //if (FloatObj != null)
        //    //{
        //    //    GameObject.Destroy(FloatObj);
        //    //}
        //    CurrentPassTime =ChachePassTime - 1;
        //}

        void TriggerUpdateTimeEvent()
        {
            //TraceUtil.Log("UpdateEnegryTime:"+CurrentPassTime);
            if (CurrentPassTime < TargetTime)
            {
                CurrentPassTime += 1;
            }
          
            UIEventManager.Instance.TriggerUIEvent(UIEventType.UpdateEnegryTimeEvent, null);
        }

        public void  Re_registration()
        {
            RegisterEventHandler();
        }
        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(),UpdateColdWorkData);
        }

        public void Instantiate()
        {
            //TraceUtil.Log("InstanceColdWorkData");
        }

        public void LifeOver()
        {
            //TraceUtil.Log("LiveOverColdWorkData");
            ChachePassTime = -1;
            //CancelInvoke();
        }
    }
}