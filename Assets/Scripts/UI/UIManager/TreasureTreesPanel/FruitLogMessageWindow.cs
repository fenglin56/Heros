using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UI.MainUI
{
    public class FruitLogMessageWindow : MonoBehaviour, ISingletonLifeCycle
    {

        public TreasureTreesPanelManager MyParent { get; private set; }
        //public SingleButtonCallBack LeftBtn;
        //public SingleButtonCallBack RightBtn;
        public SingleButtonCallBack CloseBtn;
        //public UILabel MainMsgLabel;
        //public UILabel CurrentPageLabel;
        public GameObject m_SingleMessageItemPrefab;

        //消息面板信息列表
        public UIDraggablePanel m_msgListPanel;
        
        //Grid
        public UIGrid m_msgGrid;

        private Transform m_gridTransform;

        List<string> LogMsgList = new List<string>();

        private List<GameObject> objList = new List<GameObject>();


        void Awake()
        {
            ClosePanel(null);
            m_gridTransform = m_msgGrid.transform;
            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            CloseBtn.gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_MessageClose);
        }

        void Start()
        {
            CloseBtn.SetCallBackFuntion(ClosePanel);
        }

        public void Init(TreasureTreesPanelManager myParent)
        {
            this.MyParent = myParent;
        }

        void AddStringToMsg(string Msg)
        {
            this.LogMsgList.Insert(0, Msg);
            //ResetPageInfo();
            RefreshMsgList();
            //GameObject msgObj = UI.CreatObjectToNGUI.InstantiateObj(m_SingleMessageItemPrefab, m_gridTransform);
            //TreasureTreeMsgItem item = msgObj.GetComponent<TreasureTreeMsgItem>();
            //item.Setup(Msg);
            //m_msgGrid.Reposition();
            MyParent.SetNewTipShow(true);
        }

        void RefreshMsgList()
        {
            foreach(GameObject obj in objList)
            {
                DestroyImmediate(obj);
            }
            objList.Clear();

            foreach(string str in LogMsgList)
            {
                GameObject msgObj = UI.CreatObjectToNGUI.InstantiateObj(m_SingleMessageItemPrefab, m_gridTransform);
                TreasureTreeMsgItem item = msgObj.GetComponent<TreasureTreeMsgItem>();
                item.Setup(str);
                objList.Add(msgObj);
            }

            m_msgGrid.Reposition();
            m_msgListPanel.ResetPosition();
        }



        /// <summary>
        /// 添加使用仙露记录
        /// </summary>
        public void AddUserAmritaLogInfo(TreasureTreesFruitPoint treasureTreesFruitPoint)
        {
            return;
            string LogMsg = string.Format(LanguageTextManager.GetString("IDS_H1_458"), GetCurrentTime(), GetFruitName(treasureTreesFruitPoint.MyLocalFruitData));
            AddStringToMsg(LogMsg);
        }

        public void AddUserAmritaLogInfoFromServer(FruitData data)
        {
            string LogMsg = string.Format(LanguageTextManager.GetString("IDS_H1_458"), GetCurrentTime(), GetFruitName(data));
            AddStringToMsg(LogMsg);
        }

        /// <summary>
        /// 添加获取果实记录
        /// </summary>
        /// <param name="fruitPosition"></param>
        public void AddGetFruitLogInfo(TreasureTreesFruitPoint treasureTreesFruitPoint)
        {
            return;
            //TraceUtil.Log(PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL);
            //TraceUtil.Log(treasureTreesFruitPoint.MyLocalFruitData.RewardNumber[PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL - 1]);
            string LogMsg = string.Format(LanguageTextManager.GetString(treasureTreesFruitPoint.MyLocalFruitData.RewardType == 0 ? "IDS_H1_459" : "IDS_H1_460"), 
                GetCurrentTime(), GetFruitName(treasureTreesFruitPoint.MyLocalFruitData),
                treasureTreesFruitPoint.MyLocalFruitData.FruitReward[PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL - 1]);
            AddStringToMsg(LogMsg);
        }

        public void AddGetFruitLogInfoServer(FruitData data, int itemId, int itemCount)
        {
            ItemData itemData = ItemDataManager.Instance.GetItemData(itemId);
            string itemName = LanguageTextManager.GetString(itemData._szGoodsName);
            string LogMsg = string.Format(LanguageTextManager.GetString( "IDS_I28_5" ), 
                                          GetCurrentTime(), GetFruitName(data),
                                          itemName, itemCount);
            AddStringToMsg(LogMsg);
        }
        /// <summary>
        /// 添加浇水记录
        /// </summary>
        /// <param name="fruitPosition"></param>
        public void AddWateringFruitLogInfo(TreasureTreesFruitPoint treasureTreesFruitPoint)
        {
            string LogMsg = string.Format(LanguageTextManager.GetString("IDS_H1_461"), GetCurrentTime(), GetFruitName(treasureTreesFruitPoint.MyLocalFruitData));
            AddStringToMsg(LogMsg);
        }

        string GetCurrentTime()
        {
            string hourStr = DateTime.Now.Hour.ToString();
            int minute = DateTime.Now.Minute;
            string minuteStr = minute.ToString();
            if(minute < 10)
            {
                minuteStr = "0" + minuteStr;
            }
            string timeNow = string.Format("{0}:{1}" ,hourStr,minuteStr);
            return timeNow;
        }


        string GetFruitName(FruitData fruitData)
        {
            TextColor textColor = TextColor.white;
            switch (fruitData.FruitLevel)
            {
                case 0:
                    textColor = TextColor.EquipmentGreen;
                    break;
                case 1:
                    textColor = TextColor.EquipmentBlue;
                    break;
                case 2:
                    textColor = TextColor.EquipmentMagenta;
                    break;
                case 3:
                    textColor = TextColor.EquipmentYellow;
                    break;
                default:
                    break;
            }
            string name = NGUIColor.SetTxtColor(LanguageTextManager.GetString(fruitData.FruitName),textColor);
            return name;
        }

        public void ShowPanel(object obj)
        {
            m_msgListPanel.ResetPosition();
            MyParent.SetNewTipShow(false);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");

            transform.localPosition = new Vector3(0, 0, -80);
        }

        void ClosePanel(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Tree_DailyLeave");
            transform.localPosition = new Vector3(0,0,-1000);
        }

        public void Instantiate()
        {
        }

        public void LifeOver()
        {
            this.LogMsgList.Clear();
        }
    }
}