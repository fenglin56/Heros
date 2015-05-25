using UnityEngine;
using System.Collections;

namespace UI
{

    public enum MessageType
    {
        WithoutCallBack,
        WithAnParameter,
        WithTwoParameter,
        TipsMessage,
		UnlockTipsMessage,
		//两按钮外加显示消耗性货币
		WithTwoParamMoney,
    }
	public enum EMessageCoinType{
		//铜币
		ECuType = 1,
		//金币
		EGoldType = 3,

	}
    public class SubMessage
    {
        public string MsgTitle { get; private set; }
        public string MsgMainBody { get; private set; }
		public string MsgMoneyStr { get; private set; }
        public string EnsureBtnStr { get; private set; }
        public string CancelBtnStr { get; private set; }
        public float ShowTime { get; private set; }
		public EMessageCoinType ShowCoinType{ get; private set;}
        public MessageType messageType = MessageType.WithoutCallBack;
        //MessageBox MessageBoxController = null;
        public SubMessageLayer SubMsgLayer { get; set; }//自己所在组的实例
        public MessageBoxCallBack SureMessageCallBack;//确定按钮委托
        public MessageBoxCallBack CancelMessageCallBack;//取消按钮委托


        GameObject instanceMsgObject;//对应该消息的场景物体，用于判断是否已经弹出来了


        public delegate void SetObjectFlag(bool flag);
        public SetObjectFlag setObjectFlag;

		public SubMessage(SubMessageLayer subMessageLayer, string Message, float ShowTime, MessageType type)
        {
            this.SubMsgLayer = subMessageLayer;
            this.MsgMainBody = Message;
            this.ShowTime = ShowTime;
			messageType = type;
        }

        public SubMessage(SubMessageLayer subMsgLayer, string MsgTitle, string MsgMainBody, string EnsureBtnStr)
        {
            this.SubMsgLayer = subMsgLayer;
            this.MsgTitle = MsgTitle;
            this.MsgMainBody = MsgMainBody;
            this.EnsureBtnStr = EnsureBtnStr;
            messageType = MessageType.WithoutCallBack;
        }

        public SubMessage( SubMessageLayer subMsgLayer, string MsgTitle, string MsgMainBody,string EnsureBtnStr, MessageBoxCallBack SureMessageCallBack)//带确定返回参数
        {
            this.SubMsgLayer = subMsgLayer;
            this.MsgTitle = MsgTitle;
            this.MsgMainBody = MsgMainBody;
            this.EnsureBtnStr = EnsureBtnStr;
            this.SureMessageCallBack = SureMessageCallBack;
            messageType = MessageType.WithAnParameter;
        }

        public SubMessage(SubMessageLayer subMsgLayer, string MsgTitle, string MsgMainBody,string EnsureBtnStr,string CancelBtnStr, MessageBoxCallBack SureMessageCallBack, MessageBoxCallBack CancelMessageCallBack)//带确定取消返回参数
        {
            this.SubMsgLayer = subMsgLayer;
            this.MsgTitle = MsgTitle;
            this.MsgMainBody = MsgMainBody;
            this.EnsureBtnStr = EnsureBtnStr;
            this.CancelBtnStr = CancelBtnStr;
            this.SureMessageCallBack = SureMessageCallBack;
            this.CancelMessageCallBack = CancelMessageCallBack;
            messageType = MessageType.WithTwoParameter;
        }
		//带消耗性货币
		public SubMessage(SubMessageLayer subMsgLayer,EMessageCoinType coinType, string MsgMainBody,string MsgMoney,string EnsureBtnStr,string CancelBtnStr, MessageBoxCallBack SureMessageCallBack, MessageBoxCallBack CancelMessageCallBack)//带确定取消返回参数
		{
			this.SubMsgLayer = subMsgLayer;
			//界面中把标题当消耗货币使用//
			this.MsgMainBody = MsgMainBody;
			this.MsgMoneyStr = MsgMoney;
			this.ShowCoinType = coinType;
			this.EnsureBtnStr = EnsureBtnStr;
			this.CancelBtnStr = CancelBtnStr;
			this.SureMessageCallBack = SureMessageCallBack;
			this.CancelMessageCallBack = CancelMessageCallBack;
			messageType = MessageType.WithTwoParamMoney;
		}

        public void SetMyObjectActive(bool flag)
        {
            if (flag)
            {
                InstanceMsgBox();
            }
            if (setObjectFlag != null)
            {
                setObjectFlag(flag);
            }
        }

        public GameObject InstanceMsgBox()//将本条消息关联到场景的MessageBoxObject上
        {
            switch (this.messageType)
            {
                case MessageType.TipsMessage:
                    instanceMsgObject = MessageBox.Instance.GetTipsMessageInstance();
                    break;
                case MessageType.WithoutCallBack:
                    instanceMsgObject = MessageBox.Instance.GetOneButtonMessageInstance();
                    break;
                case MessageType.WithAnParameter:
                    instanceMsgObject = MessageBox.Instance.GetOneButtonMessageInstance();
                    break;
                case MessageType.WithTwoParameter:
                    instanceMsgObject = MessageBox.Instance.GetTwoButtonMessageInstance();
                    break;
				case MessageType.WithTwoParamMoney:
					instanceMsgObject = MessageBox.Instance.GetTwoButtonMoneyMessageInstance();
				break;
				case MessageType.UnlockTipsMessage:
					instanceMsgObject = MessageBox.Instance.GetUnlockTipsMessageInstance();
					break;

                default:
                    break;
            }
            MsgBoxInstance msgBoxScript = instanceMsgObject.GetComponent<MsgBoxInstance>();
            msgBoxScript.SetMsgAttribute(this);

            return instanceMsgObject;
        }

    }
}
