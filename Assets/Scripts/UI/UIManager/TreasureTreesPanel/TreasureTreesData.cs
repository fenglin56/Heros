using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI.MainUI
{

    public class TreasureTreesData : ISingletonLifeCycle
    {
        private static TreasureTreesData m_instance;
        public static TreasureTreesData Instance { 
            get 
            {
                if (m_instance == null)
                {
                    m_instance = new TreasureTreesData();
                    SingletonManager.Instance.Add(m_instance); 
                }
                return m_instance;
            }
        }

        public List<SMsgActionFruitContext_SC> FruitDataList { get; private set; }

        public TreasureTreesData()
        {
            this.FruitDataList = new List<SMsgActionFruitContext_SC>();
            UIEventManager.Instance.RegisterUIEvent(UIEventType.LoadingComplete, CheckFruitIsRipen);
        }


        /// <summary>
        /// 刷新宝树果实信息
        /// </summary>
        /// <param name="sMsgActionFruitContext_SC"></param>
        public void ResetTreasureTreesDataInfo(SMsgActionFruitContext_SC sMsgActionFruitContext_SC)
        {
            TraceUtil.Log("收到刷新果实信息：" + sMsgActionFruitContext_SC.dwFruitID + "," + sMsgActionFruitContext_SC.byFruitPosition);
            //TraceUtil.Log("果实状态:" + sMsgActionFruitContext_SC.byFruitStatus + ",是否干旱：" + sMsgActionFruitContext_SC.byFruitDryStatus);
            //TraceUtil.Log(string.Format("{0}//{1}果实开始时间:{2},结果时间{3}",0, GetNowTimes(), sMsgActionFruitContext_SC.dwStartTimes, sMsgActionFruitContext_SC.dwEndTime));
            SMsgActionFruitContext_SC UpdateData = FruitDataList.FirstOrDefault(P => P.byFruitPosition == sMsgActionFruitContext_SC.byFruitPosition);
            if (UpdateData.dwFruitID != 0)
            {
                FruitDataList.Remove(UpdateData);
            }
            FruitDataList.Add(sMsgActionFruitContext_SC);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.UpdateTreasureTreesData, sMsgActionFruitContext_SC);
            DoForTime.DoFunForTime(1, CheckFruitIsRipen, null);
        }
        /// <summary>
        /// 检测是否有成熟果实
        /// </summary>
        void CheckFruitIsRipen(object obj)
        { 
            bool flag = false;
            FruitDataList.ApplyAllItem(P => flag|=(FruitPrucStatusType)P.byFruitStatus == FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE );
            //TraceUtil.Log("检测是否有成熟宝树果实:"+flag);
            if (flag)
            {
                UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim,UIType.Treasure);
            }
            else
            {
                UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UIType.Treasure);
            }
        }

        public DateTime GetNoralTime(string now)
        {
            string timeStamp = now;
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }
        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public long GetNowTimes()
        {
            DateTime time = DateTime.Now;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        public void Instantiate()
        {
        }

        public void LifeOver()
        {
            //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"TreasureTreesDataLifeOver");
            FruitDataList.Clear();
            m_instance = null;
        }
    }
}