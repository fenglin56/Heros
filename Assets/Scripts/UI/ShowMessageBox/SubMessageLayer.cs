using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI{

    public class SubMessageLayer
    {
       //************************同一优先层下的所有消息队列**********************
        int MyMsgLayer { get; set; }
        List<SubMessage> MessageBoxArray;//该优先层下的所有消息
        //MessageBox messageBoxController;

        public SubMessageLayer(int msgLayer)
        {
            this.MyMsgLayer = msgLayer;
            MessageBoxArray = new List<SubMessage>();
            //MessageBox.MessageEvent += new MessageBox.MessageDelegate(ShowLayerMsgBox);
            //messageBoxController = GameObject.FindGameObjectWithTag("Message").GetComponent<MessageBox>();
        }

        //public void ShowLayerMsgBox(int ShowMsgLayer)//显示某一层级的提示框
        //{
        //    if (MyMsgLayer != ShowMsgLayer)
        //    {
        //        SetMyLayerBoxActive(false);
        //    }
        //    else
        //    {
        //        SetMyLayerBoxActive(true);
        //    }
        //}

        public void ShowMyLayerMsg()//显示我这一层的消息
        {
            if (MessageBoxArray != null && MessageBoxArray.Count > 0)
            {
                MessageBoxArray[0].SetMyObjectActive(true);
            }
        }

        public void CloseMyLayerMsg()//关闭我这一层的消息
        {
            if (MessageBoxArray != null && MessageBoxArray.Count > 0)
            {
                MessageBoxArray[0].SetMyObjectActive(false);
            }
        }

        public SubMessage getSubMesssage()//获取我这一层的消息
        {
            if (MessageBoxArray != null && MessageBoxArray.Count > 0)
            {
                return MessageBoxArray[0];
            }
            else
            {
                return null;
            }
        }

        public void AddSubMessage(SubMessage subMessage)//增加消息到我这一层中
        {
            MessageBoxArray.Add(subMessage);
        }

        public void SetMyLayerBoxActive(bool flag)//激活或冻结子消息
        {
            if (MessageBoxArray != null && MessageBoxArray.Count > 0)
            {
                MessageBoxArray[0].SetMyObjectActive(flag);
            }
        }

        public void DestoryMyChild(SubMessage Child)//删除出现并被确认过的消息
        {
            if (MessageBoxArray.Contains(Child))
            {
                MessageBoxArray.Remove(Child);
            }
            MessageBox.Instance.ShowRemainMsgBox();
        }

        public void RemoveAllSubMessage()
        {
            MessageBoxArray.Clear();
        }

        //add by lee
        public int GetMessageBoxArrayLength()
        {
            if (MessageBoxArray == null)
            {
                return 0;
            }
            return MessageBoxArray.Count;
        }
    }


}
