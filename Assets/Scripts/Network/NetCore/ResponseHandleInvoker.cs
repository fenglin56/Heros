using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//#define WRITE_NET_MESSAGE

namespace NetworkCommon
{
    public struct ResponseHandleData
    {
        public CommondResponseHandle CommondResponseHandle { get; set; }
        public byte[] DataBuffer { get; set; }
        public int SocketId { get; set; }
    }
    public class ResponseHandleInvoker
    {
        //public event Test.TestGameBox.ShowMessageOnGUIHandle OnShowMessageOnGUI;

        private bool m_isPaused = false;
        private Queue<ResponseHandleData> m_works = new Queue<ResponseHandleData>();
        private bool IsQueuePrecessing=false;
        private static ResponseHandleInvoker m_instance;
        private ResponseHandleData m_currentHandleData;

        public bool IsPaused 
        {
            get { 
                return this.m_isPaused; 
            }
            set { 
                this.m_isPaused = value;
               // TraceUtil.Log("IsPause Change To "+value);
            }
        }
        
        public static ResponseHandleInvoker Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new ResponseHandleInvoker();
                    //UnityEngine.//TraceUtil.Log("启动网络消息响应Invoke机");
                }
                return m_instance;
            }
        }
        public void Add(CommondResponseHandle commondResponseHandle, byte[] dataBuffer, int socketId)
        {
            lock (m_works)
            {
                m_works.Enqueue(new ResponseHandleData() { CommondResponseHandle = commondResponseHandle, DataBuffer = dataBuffer, SocketId = socketId });
            }
        }
        public void Add(ResponseHandleData responseHandleData)
        {
            lock (m_works)
            {
                m_works.Enqueue(responseHandleData);
            }
        }


        public void DO()
        {
            if (IsPaused) return;
            if (!IsQueuePrecessing&&m_works.Count > 0)
            {
                //TODO:: disable isqueprecessing in publish version!!!! 
                //cause it make net message do not work after error occours!!!
                //this.IsQueuePrecessing = true;
                lock (m_works)
                {
                    while (m_works.Count > 0)
                    {
                        m_currentHandleData = m_works.Dequeue();
                        var returnValue=m_currentHandleData.CommondResponseHandle(m_currentHandleData.DataBuffer, m_currentHandleData.SocketId);
                        #if UNITY_EDITOR && WRITE_NET_MESSAGE
						Package pg;
						pg = PackageHelper.ParseReceiveData(m_currentHandleData.DataBuffer);
						String[] logText  = new String[3];
						logText[0] =  "Receive:" + System.DateTime.Now.ToLocalTime().ToString();
						logText[1] = pg.Head.MasterMsgType.ToString();
						logText[2] = pg.Head.SubMsgType.ToString();
						LogManager.Instance.WriteLog("NetResponse", logText);

                        #endif
                        if (returnValue == CommandCallbackType.Pause)
                        {
                            IsPaused = true;
                            break;
                        }
                    }
                }
                this.IsQueuePrecessing = false;
            }
        }
        public void ClearResponseQueue()
        {
            m_works.Clear();
        }
    }
}
