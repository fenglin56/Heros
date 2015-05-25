using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UI.Friend
{
    public class FriendDataManager : ISingletonLifeCycle
    {
        private List<PanelElementDataModel> m_friendList = new List<PanelElementDataModel>();
        private List<PanelElementDataModel> m_requestList = new List<PanelElementDataModel>();

        //private Queue m_curFriendQueue = new Queue();
        //private List<PanelElementDataModel> m_curFriendQueueList = new List<PanelElementDataModel>();

        private static FriendDataManager m_instance;
        public static FriendDataManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new FriendDataManager();
					SingletonManager.Instance.Add(m_instance);
                }
                return m_instance;
            }
        }

		private int m_FriendMaxNum = 10;
		public int FriendMaxNum
		{
			get
			{
				return m_FriendMaxNum;
			}
		}
		/// <summary>
		/// 网络下发设置好友最大数
		/// </summary>
		/// <param name="value">Value.</param>
		public void SetFriendMaxNum(int value)
		{
			m_FriendMaxNum = value;
		}

        public bool IsDelFriendIsMe
        {
            set;
            get;
        }

        //是否创建好友列表UI
        public bool IsCreateFriendUI
        {
            set;
            get;
        }

        //是否更新好友列表
        public bool IsUpdateFriendList
        {
            set;
            get;
        }

        /// <summary>
        /// 当前面板的状态
        /// </summary>
        public PanelState CurPanelState
        {
            set;
            get;
        }

        //更新显示加好友消息数目
        public bool IsUpdateMsgNum
        {
            set;
            get;
        }

        /// <summary>
        /// 更新好友数据
        /// </summary>
        /// <param name="sMsgUpdateFriendProp_SC">更新好友数据的结构体</param>
        public void UpdateFriendProp(SMsgUpdateFriendProp_SC sMsgUpdateFriendProp_SC)
        {
            for (int i = 0; i < m_friendList.Count; i++)
            {
                if (sMsgUpdateFriendProp_SC.dwFriendActorID == m_friendList[i].sMsgRecvAnswerFriends_SC.dwFriendID)
                {
                    m_friendList[i].sMsgRecvAnswerFriends_SC.SetValue(sMsgUpdateFriendProp_SC.byIndex, sMsgUpdateFriendProp_SC.dwPropNum);                   
                }
            }
        }

        /// <summary>
        /// 将申请好友消息加入申请加入好友列表
        /// </summary>
        /// <param name="playerID">申请者的ID</param>
        /// <param name="panelData">申请者的数据</param>
        public void RegRequestData(PanelElementDataModel panelData)
        {
            if (panelData == null)
                return;

            if (m_requestList.Exists(P => P.sMsgRecvAnswerFriends_SC.dwFriendID == panelData.sMsgRecvAnswerFriends_SC.dwFriendID))
                return;

            m_requestList.Add(panelData);
        }

        /// <summary>
        /// 添加好友数据
        /// </summary>
        /// <param name="playerID">好友的ID</param>
        /// <param name="panelData">好友的数据</param>
        public void RegFriendData(PanelElementDataModel panelData)
        {
            if (panelData != null)
            {
                if (!m_friendList.Any(p => p.sMsgRecvAnswerFriends_SC.dwFriendID == panelData.sMsgRecvAnswerFriends_SC.dwFriendID))
                {
                    m_friendList.Add(panelData);
                }
            }                
        }

		/// <summary>
		/// 根据ID删除好友信息
		/// </summary>
		/// <param name="friendID">好友ID</param>
		public void DeleteFriendData(uint friendID)
		{
			for (int i = 0; i < m_friendList.Count; i++)
			{
				if (m_friendList[i].sMsgRecvAnswerFriends_SC.dwFriendID == friendID)
				{
					m_friendList.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// 删除请求信息
		/// </summary>
		public void DeleteRequest(uint friendID)
		{
			for (int i = 0; i < m_requestList.Count; i++)
			{
				if (m_requestList[i].sMsgRecvAnswerFriends_SC.dwFriendID == friendID)
				{
					m_requestList.RemoveAt(i);
				}
			}
		}


        /// <summary>
        /// 设置是否显示我的好友UI面板
        /// </summary>
        /// <param name="zDepth">UI面板的深度值FriendUIConst.UI_SHOW_ZDEPTH为显示，FriendUIConst.UI_NO_SHOW_ZDEPTH为不显示</param>
        public void SetFriendListDepth(int zDepth)
        {
            int i = 0;

            for (; i < m_requestList.Count; i++)
            {
                //if (null != m_addFriendList[i].BtnObj)
                //{
                //    m_addFriendList[i].BtnObj.transform.localPosition = new Vector3(0, 150 - 150 * i, zDepth);
                //}
                    m_requestList[i].PositionID = i;
            }

            for (int j = 0; j < m_friendList.Count; j++)
            {
                //if (null != m_friendList[j].BtnObj)
                //{
                //    m_friendList[j].BtnObj.transform.localPosition = new Vector3(0, 150 - 150 * (j + i), zDepth);
                //}
                m_friendList[j].PositionID = j + i;
            }
        }


        /// <summary>
        /// 根据好友是否在线对好友列表进行排序
        /// </summary>
        public void SortFriendList()
        {
            m_friendList.Sort(new FriendComparer());
        }

        /// <summary>
        /// 获取在线好友列表
        /// </summary>
        public List<PanelElementDataModel> GetOnlineFriendList
        {
            get { return m_friendList.FindAll(P => P.sMsgRecvAnswerFriends_SC.bOnLine == 1); }
        }

        /// <summary>
        /// 获取当前好友列表<字典>
        /// </summary>
        public List<PanelElementDataModel> GetFriendListData
        {
            get { return m_friendList; }
        }

        /// <summary>
        /// 获取当前申请加好友列表<字典>
        /// </summary>
        public List<PanelElementDataModel> GetRequestListData
        {
            get { return m_requestList; }
        }

		#region ISingletonLifeCycle implementation

		public void Instantiate ()
		{
		}

		public void LifeOver ()
		{
			m_instance = null;
		}

		#endregion
    }

    /// <summary>
    /// 单个面板数据结构
    /// </summary>
    public class PanelElementDataModel
    {
        public SMsgRecvAnswerFriends_SC sMsgRecvAnswerFriends_SC;
        //public GameObject BtnObj;
        public UInt64 RequestTime;
        public ButtonType BtnType;
        public int PositionID;
    }

    public enum ButtonType
    {
        None,
        AddFriend,
        FriendList,
        NearlyPlayer,
    }

    public enum TeamType
    {
        NoTeam,
        TeamLeader,
        TeamMember,
    }
}
