using UnityEngine;
using System.Collections;

namespace UI
{

    public delegate void MessageBoxCallBack();

    public class MsgBoxInstance : MonoBehaviour
    {
        SubMessage MySubMessage;

        public UILabel MsgLable;
        public UILabel MsgMainTxt;
        public LocalButtonCallBack EnSurebuttonCallBack;
        public LocalButtonCallBack CancelbuttonCallBack;

        private int[] m_guideBtnID = new int[2];

        //GameObject Button_Yes;//有两个按钮，此为确认
        //GameObject Button_No;//有两个按钮时，此为取消
        //GameObject Button_Sure;//当只有一个确定按钮的时候

		private bool IsClicked = false;

        void Awake()
        {            
            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
            {
                if (EnSurebuttonCallBack != null)
                {
                    EnSurebuttonCallBack.gameObject.RegisterBtnMappingId(UI.MainUI.UIType.CommonButton, BtnMapId_Sub.CommonButton_Show_Ensure);
                    var guideBtnBehaviour = EnSurebuttonCallBack.GetComponent<GuideBtnBehaviour>();
                    if (guideBtnBehaviour != null)
                    {
                        TaskBtnManager.Instance.FindGuideBtnParamViaMappingId(guideBtnBehaviour.MappingId).BtnCollider.enabled = true;
                    }
                }

                if (CancelbuttonCallBack != null)
                {
                    CancelbuttonCallBack.gameObject.RegisterBtnMappingId(UI.MainUI.UIType.CommonButton, BtnMapId_Sub.CommonButton_Show_Cancel);
                    var guideBtnBehaviour = CancelbuttonCallBack.GetComponent<GuideBtnBehaviour>();
                    if (guideBtnBehaviour != null)
                    {
                        TaskBtnManager.Instance.FindGuideBtnParamViaMappingId(guideBtnBehaviour.MappingId).BtnCollider.enabled = true;
                    }
                }
            }
        }
        void Start()
        {
            if (EnSurebuttonCallBack != null) { EnSurebuttonCallBack.SetCallBackFuntion(OnSureClick,null); }
            if (CancelbuttonCallBack != null) { CancelbuttonCallBack.SetCallBackFuntion(OnCancelClick,null); }
        }

        public void SetObjectEnableFlag(bool Flag)//当有其他优先级窗口弹出时冻结此窗口及解冻此窗口
        {
            if (Flag)
            {
                EnableMyself();
            }
            else
			{
				DisabelMySelf();
            }
        }

		public void SetMsgAttribute(SubMessage messageClass)
		{
			IsClicked = false;
			MySubMessage = messageClass;
			MySubMessage.setObjectFlag = SetObjectEnableFlag;
			MsgMainTxt.text = MySubMessage.MsgMainBody;
			switch (messageClass.messageType)
			{
			case MessageType.UnlockTipsMessage:
			case MessageType.TipsMessage:
				StartCoroutine(CloseMyselfForSeconds(MySubMessage.ShowTime));
				break;
			default:
			{
				if(messageClass.messageType == MessageType.WithTwoParamMoney)
				{
					Transform showIcon = MsgLable.transform.parent.Find("Icon");
					int changeSprite = 0;
					if(messageClass.ShowCoinType == EMessageCoinType.EGoldType)
					{
						changeSprite = 1;
					}
					else
					{
						changeSprite = 2;
					}
					if(showIcon != null)
						showIcon.GetComponent<SpriteSwith>().ChangeSprite(changeSprite);
					MsgLable.text = MySubMessage.MsgMoneyStr;
				}
				else
				{
					MsgLable.text = MySubMessage.MsgTitle;
				}
				if (EnSurebuttonCallBack != null) { EnSurebuttonCallBack.SetButtonText(messageClass.EnsureBtnStr); }
				if (CancelbuttonCallBack != null) { CancelbuttonCallBack.SetButtonText(messageClass.CancelBtnStr); }
			}
				break;
			}
		}

        IEnumerator CloseMyselfForSeconds(float Seconds)
        {
			yield return new WaitForSeconds(Seconds);
			DisabelMySelf();
        }

        void OnSureClick(object obj)
		{
			if(IsClicked)
				return;
			IsClicked = true;
            //DestroyMySelf();
            DisabelMySelf();
            if (MySubMessage.SureMessageCallBack != null)
            {
                MySubMessage.SureMessageCallBack();
            }


        }

        void OnCancelClick(object obj)
		{
			if(IsClicked)
				return;
			IsClicked = true;
            if (MySubMessage.CancelMessageCallBack != null)
            {
                MySubMessage.CancelMessageCallBack();
            }
            //DestroyMySelf();
            DisabelMySelf();
        }

        void DestroyMySelf()
        {
			StartCoroutine(DisableMySelfForTime());
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++ )
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        IEnumerator DisableMySelfForTime()
        {
			yield return null;
			MySubMessage.SubMsgLayer.DestoryMyChild(MySubMessage);
            this.gameObject.transform.localPosition =new Vector3(40,0,-800);
        }

		void DisabelMySelf()
		{
            MySubMessage.SubMsgLayer.DestoryMyChild(MySubMessage);
			this.gameObject.transform.localPosition =new Vector3(40,0,-800);
		}

        void EnableMyself()
        {
            this.gameObject.transform.localPosition = new Vector3(40, 0, -150);
        }

    }
}
