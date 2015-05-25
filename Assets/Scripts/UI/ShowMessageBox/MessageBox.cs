using UnityEngine;
using System.Collections.Generic;

namespace UI
{

    public class MessageBox : View
    {

        const int MaxMsgLayer = 10;//最大显示层级
        public SubMessageLayer[] MessageArray = new SubMessageLayer[MaxMsgLayer];//messageBox的优先层，1为最高优先级，往后越来越低

        public Transform MessageInstanceParent;//被实例出来后的父对象
        public GameObject MessageObject_OneButton;
        public GameObject MessageObject_TwoButton;
		public GameObject MessageObject_TwoBtnMoney;
        public GameObject TipsObject;
		public GameObject UnlockTipsObject;

        private GameObject MsgObjWtOneBtnInstance = null;
        private GameObject MsgObjWtTwoBtnInstance = null;
		private GameObject MsgObjWtTwoBtnMoneyInstance = null;
        private GameObject TipsMessagePanelInstance = null;
		private GameObject UnlockTipsMessagePanelInstance = null;

        static MessageBox m_instance;

        public static MessageBox Instance { get { return m_instance; } }

        void Awake()
        {
            m_instance = this;

            RegisterEventHandler();
        }

        void OnDestroy()
        {
            m_instance = null;
        }
        /// <summary>
        /// show方法，显示提示框
        /// </summary>
        /// <param name="MsgLayer">优先级，1为最高，10为最低</param>
        /// <param name="MsgTitle">信息标题</param>
        /// <param name="Msg">主信息</param>
        /// <param name="EnsureBtnStr">按钮文字</param>
        public void Show(int MsgLayer, string MsgTitle, string Msg, string EnsureBtnStr)
        {
            if (MessageArray[MsgLayer] == null) 
            {
                MessageArray[MsgLayer] = new SubMessageLayer(MsgLayer); 
            }
            MessageArray[MsgLayer].AddSubMessage(new SubMessage(MessageArray[MsgLayer], MsgTitle, Msg, EnsureBtnStr));
            ShowLayerMsgBox();
        }
        /// <summary>
        /// show方法，显示提示框
        /// </summary>
        /// <param name="MsgLayer">优先级，1为最高，10为最低</param>
        /// <param name="MsgTitle">信息标题</param>
        /// <param name="Msg">主信息</param>
        /// <param name="EnsureBtnStr">按钮文字</param>
        /// <param name="SureButtonCallBack">确定按钮回调</param>
        public void Show(int MsgLayer, string MsgTitle, string Msg, string EnsureBtnStr, MessageBoxCallBack SureButtonCallBack)
        {
            if (MessageArray[MsgLayer] == null)
            {
                MessageArray[MsgLayer] = new SubMessageLayer(MsgLayer);
            }
            MessageArray[MsgLayer].AddSubMessage(new SubMessage(MessageArray[MsgLayer], MsgTitle, Msg, EnsureBtnStr, SureButtonCallBack));
            ShowLayerMsgBox();
        }
        /// <summary>
        /// show方法，显示提示框
        /// </summary>
        /// <param name="MsgLayer">优先级，1为最高，10为最低</param>
        /// <param name="MsgTitle">信息标题</param>
        /// <param name="Msg">主信息</param>
        /// <param name="EnsureBtnStr">确定按钮的文字</param>
        /// <param name="CancelBtnStr">取消按钮的文字</param>
        /// <param name="SureButtonCallBack">确定按钮的回调</param>
        /// <param name="CancelButtonCallBack">取消按钮的回调</param>
        public void Show(int MsgLayer, string MsgTitle, string Msg, string EnsureBtnStr, string CancelBtnStr, MessageBoxCallBack SureButtonCallBack, MessageBoxCallBack CancelButtonCallBack)
        {
            if (MessageArray[MsgLayer] == null)
            {
                MessageArray[MsgLayer] = new SubMessageLayer(MsgLayer);
            }
            MessageArray[MsgLayer].AddSubMessage(new SubMessage(MessageArray[MsgLayer], MsgTitle, Msg, EnsureBtnStr, CancelBtnStr, SureButtonCallBack, CancelButtonCallBack));
            ShowLayerMsgBox();
        }
		/// <summary>
		/// show方法，显示提示框(带消耗货币)
		/// </summary>
		/// <param name="MsgLayer">优先级，1为最高，10为最低</param>
		/// <param name="MsgTitle">信息标题</param>
		/// <param name="Msg">主信息</param>
		/// <param name="EnsureBtnStr">确定按钮的文字</param>
		/// <param name="CancelBtnStr">取消按钮的文字</param>
		/// <param name="SureButtonCallBack">确定按钮的回调</param>
		/// <param name="CancelButtonCallBack">取消按钮的回调</param>
		public void Show(int MsgLayer, EMessageCoinType coinType, string MsgText,int money, string EnsureBtnStr, string CancelBtnStr, MessageBoxCallBack SureButtonCallBack, MessageBoxCallBack CancelButtonCallBack)
		{
			if (MessageArray[MsgLayer] == null)
			{
				MessageArray[MsgLayer] = new SubMessageLayer(MsgLayer);
			}
			string strMoney = money.ToString();
			if ((coinType == EMessageCoinType.EGoldType && !PlayerManager.Instance.IsBindPayEnough (money)) || (coinType == EMessageCoinType.ECuType && !PlayerManager.Instance.IsMoneyEnough (money))) {
				strMoney = "[ff0000]"+money+"[-]";
			}
			MessageArray[MsgLayer].AddSubMessage(new SubMessage(MessageArray[MsgLayer], coinType,MsgText, strMoney, EnsureBtnStr, CancelBtnStr, SureButtonCallBack, CancelButtonCallBack));
			ShowLayerMsgBox();
		}
        /// <summary>
        /// 显示tips
        /// </summary>
        /// <param name="MsgLayer"></param>
        /// <param name="TipsInfo"></param>
        /// <param name="ShowTime"></param>
        public void ShowTips(int MsgLayer, string TipsInfo, float ShowTime)
        {
            if (MessageArray[MsgLayer] == null)
            {
                MessageArray[MsgLayer] = new SubMessageLayer(MsgLayer);
            }
            MessageArray[MsgLayer].AddSubMessage(new SubMessage(MessageArray[MsgLayer], TipsInfo,ShowTime, MessageType.TipsMessage));
            ShowLayerMsgBox();
        }

		/// <summary>
		/// 显示不锁屏类型tips
		/// </summary>
		public void ShowUnlockTips(int MsgLayer, string TipsInfo, float ShowTime)
		{
			if (MessageArray[MsgLayer] == null)
			{
				MessageArray[MsgLayer] = new SubMessageLayer(MsgLayer);
			}
			MessageArray[MsgLayer].AddSubMessage(new SubMessage(MessageArray[MsgLayer], TipsInfo,ShowTime, MessageType.UnlockTipsMessage));
			ShowLayerMsgBox();
		}
            
		/// <summary>
		/// 显示元宝不足提示
		/// </summary>
		public void ShowNotEnoughGoldMoneyMsg()
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Tree_BuyFail");
			Show(3,"",LanguageTextManager.GetString("IDS_I1_23"),LanguageTextManager.GetString("IDS_I1_5"),LanguageTextManager.GetString("IDS_I1_6"),
			     ()=>{UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UI.MainUI.UIType.TopUp);
				SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyIngotConfirmation");
			},()=>{
				SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyIngotCancel");
			});
		}
		public delegate void MoneyLessCallBack();
		/// <summary>
		/// 显示元宝不足提示
		/// </summary>
		public void ShowNotEnoughGoldMoneyMsg(MoneyLessCallBack sureBtnCb)
		{
			Show(3,"",LanguageTextManager.GetString("IDS_I1_23"),LanguageTextManager.GetString("IDS_I1_5"),LanguageTextManager.GetString("IDS_I1_6"),
			     ()=>{UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UI.MainUI.UIType.TopUp);
				SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyIngotConfirmation");
				if(sureBtnCb != null)
					sureBtnCb();
			},()=>{
				SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyIngotCancel");
			});
		}

		/// <summary>
		/// 显示铜币不足提示。。直接跳到快速购买界面
		/// </summary>
		public void ShowNotEnoughMoneyMsg(MoneyLessCallBack cancelBtnCb)
		{
			Show(1,"", LanguageTextManager.GetString("IDS_I3_50"),LanguageTextManager.GetString("IDS_I1_5"),LanguageTextManager.GetString("IDS_I1_6"),
			     ()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Confirm");
				PopupObjManager.Instance.NotEnoughMoneyPanel ();}
			,()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");
				if(cancelBtnCb != null)
					cancelBtnCb();
			});
		}

        internal void ShowRemainMsgBox()//显示剩下的提示框，为每一个提示框被关闭后调用
        {
            ShowLayerMsgBox();
        }

        public void CloseMsgBox()
        {
            foreach (SubMessageLayer child in MessageArray)
            {
                if (child != null)
                {
                    child.CloseMyLayerMsg();
                    child.RemoveAllSubMessage();                    
                }
            }
        }

        void ShowLayerMsgBox()
        {
            foreach (SubMessageLayer child in MessageArray)
            {
                if (child != null)
                {
                    child.CloseMyLayerMsg();
                }
            }
            for (int i = 0; i < MessageArray.Length; i++)
            {
                if (MessageArray[i] != null && MessageArray[i].getSubMesssage() != null)
                {
                    MessageArray[i].ShowMyLayerMsg();
                    break;
                }
            }
        }

        public GameObject GetOneButtonMessageInstance()
        {
            if (this.MsgObjWtOneBtnInstance == null)
            {
                this.MsgObjWtOneBtnInstance =CreatObjectToNGUI.InstantiateObj(MessageObject_OneButton,MessageInstanceParent);
            }
            return this.MsgObjWtOneBtnInstance;
        }

        public GameObject GetTwoButtonMessageInstance()
        {
            if (this.MsgObjWtTwoBtnInstance == null)
            {
                this.MsgObjWtTwoBtnInstance = CreatObjectToNGUI.InstantiateObj(MessageObject_TwoButton, MessageInstanceParent);
            }
            return this.MsgObjWtTwoBtnInstance;
        }
		public GameObject GetTwoButtonMoneyMessageInstance()
		{
			if (this.MsgObjWtTwoBtnMoneyInstance == null)
			{
				this.MsgObjWtTwoBtnMoneyInstance = CreatObjectToNGUI.InstantiateObj(MessageObject_TwoBtnMoney, MessageInstanceParent);
			}
			return this.MsgObjWtTwoBtnMoneyInstance;
		}
        public GameObject GetTipsMessageInstance()
        {
            if (this.TipsMessagePanelInstance == null)
            {
                this.TipsMessagePanelInstance = CreatObjectToNGUI.InstantiateObj(TipsObject, MessageInstanceParent);
            }
            return this.TipsMessagePanelInstance;
        }
		public GameObject GetUnlockTipsMessageInstance()
		{
			if(this.UnlockTipsMessagePanelInstance == null)
			{
				this.UnlockTipsMessagePanelInstance = CreatObjectToNGUI.InstantiateObj(UnlockTipsObject,MessageInstanceParent);
			}
			return this.UnlockTipsMessagePanelInstance;
		}
        private void ErrorHandle(INotifyArgs e)
        {
            ServerError serverError = (ServerError)e;
            switch (serverError.ErrorCode)
            {
                case SystemErrorCodeDefine.ERROR_CODE_DBRET_ERROR_LOGIC:// <=该值的为逻辑错误(输入异常 数据库返回)，具体由DB定义(对应DBRET_ERROR_LOGIC)
                    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_547"), LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                    break;
                case SystemErrorCodeDefine.ERROR_CODE_INVALIDUSER://无此用户
                    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_548"), LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                    break;
                case SystemErrorCodeDefine.ERROR_CODE_KEYERROR://平台Key或者SID错误
                    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_549"),LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                    break;
                case SystemErrorCodeDefine.ERROR_CODE_CREATEFAILED_NULLNAME://创建人物失败 角色名为空
                    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_550"), LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                    break;
                case SystemErrorCodeDefine.ERROR_CODE_CREATEFAILED_LOGGEDIN://创建人物(登录)失败 账号已登录 倒计时x秒 重新操作(服务器在踢人)
                    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_551"), LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                    break;
                ////case SystemErrorCodeDefine.ERROR_CODE_CREATEFAILED_DUPLICATE://创建人物失败 角色名重复
                //    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_552"), LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                //    break;
                ////case SystemErrorCodeDefine.ERROR_CODE_CREATEFAILED_MAXNUM://创建人物失败 角色达到上限
                //    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_553"), LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                //    break;
                case SystemErrorCodeDefine.ERROR_CODE_NOACTOR://无此角色
                    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_554"), LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                    break;
                case SystemErrorCodeDefine.ERROR_CODE_PADLOCK://帐号被禁
                    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_555"), LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                    break;
                case SystemErrorCodeDefine.ERROR_CODE_PADLOCKACTOR: //角色已被禁止登陆
                    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_556"), LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                    break;
                case SystemErrorCodeDefine.ERROR_CODE_DELETEFAILED://删除人物失败
                    UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_557"), LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
                    break;
				case SystemErrorCodeDefine.ERROR_CODE_VERSION_NOTSUPPORTED:		// 服务端不支持客户端的服务器
					UI.MessageBox.Instance.Show(3, "", "服务器版本不支持 。。。", LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
					break;
            }
        }
        void OnLoginFaildMessageBox()
        {
            LoginManager.Instance.ResetLoginButtonState();
            GameManager.Instance.QuitToLogin();
        }
        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.ReceiveDefaultErrorCode.ToString(), ErrorHandle);
        }
    }
}