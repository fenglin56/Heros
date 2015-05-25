using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;


namespace Chat
{
    /// <summary>
    /// 私人聊天 对话人选项
    /// </summary>
    public class PrivateTalkerItem : MonoBehaviour
    {
        public LocalButtonCallBack Button_OnClick;
        public SpriteSwith Swith_Status;//1 默认 ,2 选中 ,3 闪烁  //1 默认 ,2 闪烁 ,3 选中

		public GameObject Eff_UnRead;
		private GameObject m_EffUnRead;

        private TalkerInfo m_TalkerInfo;
		public TalkerInfo TalkerInfo{get{return m_TalkerInfo;}}

        private const float NewMsgFlashTime = 0.3f;
		private bool m_isHasNewMsg = false;

		private float m_maxInputLength = 6;//

        void Awake()
        {
            Button_OnClick.SetCallBackFuntion(OnBeSelectTalker, null);
        }

        public void Init(int actorID, string name)
        {            
            m_TalkerInfo = new TalkerInfo()
            {
                ActorID = actorID,
                Name = name
            };
			Button_OnClick.SetButtonText(PraseFormat(name));

        }
		
		public void Show(bool isFlag)
		{
			Button_OnClick.SetBoxCollider(!isFlag);
			Button_OnClick.SetSwith(isFlag?2:1);
			Button_OnClick.SetButtonTextColor(isFlag?new Color(0.427f,0.204f,0):Color.white);
			if(isFlag)
			{
				CloseNewTip();
			}
		}

		public void ShowNewTip()
		{
			SetUnReadEffActive(true);
		}
		public void CloseNewTip()
		{
			SetUnReadEffActive(false);
		}
		public bool IsHasNewMessage
		{
			get
			{
				return m_isHasNewMsg;
			}
		}

        public void DestroySelf()
        {            
            Destroy(gameObject);
        }

        void OnBeSelectTalker(object obj)
        {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatChoice");
			SetUnReadEffActive(false);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenPrivateChatWindow, this.m_TalkerInfo);
        }

		private void SetUnReadEffActive(bool isFlag)
		{
			m_isHasNewMsg = isFlag;
			Eff_UnRead.SetActive(isFlag);
		}

		public string PraseFormat(string name)
		{
			string[] mSingleText = new string[name.Length];
			int myCByteLenght = 0;
			int myEByteLenght = 0;
			float getByteLenght;
			Regex rx = new Regex( "^[\u4e00-\u9fa5]$");
			for (int i = 0; i < mSingleText.Length; i++)
			{                       
				mSingleText[i] = name.Substring(i, 1);           
				if (rx.IsMatch(mSingleText[i]))
				{               
					myCByteLenght++;
				}
				else
				{               
					myEByteLenght++;
				}
				getByteLenght = myCByteLenght * 1.5f + myEByteLenght;
				if (getByteLenght > m_maxInputLength)
				{               
					name = name.Substring(0, i) + "..";
					break;
				}
			}
			return name;
		}
		
		
	}
}
