using UnityEngine;
using System.Collections;

namespace UI.Friend
{

    public class NearlyPlayerElement_v2 : MonoBehaviour
    {


        public UILabel PlayerName;
        public UILabel PlayerLevel;
        public SpriteSwith PlayerProfassionIcon;

        public SingleButtonCallBack AddFriendBtn;    //加入队伍Button

        private ButtonType btnType = ButtonType.None;
        private uint m_curBtnID;

        private bool m_isSended = false;
        private float m_sendTime = 3.0f;

        private uint m_guideBtnID;

        void Start()
        {
            AddFriendBtn.SetCallBackFuntion(RightButtonHandle);
        }

        //public void SetAttribute(PanelElementDataModel friendData)
        //{
        //    this.PlayerName.text = friendData.sMsgRecvAnswerFriends_SC.Name;
        //    this.PlayerLevel.text = friendData.sMsgRecvAnswerFriends_SC.sActorLevel.ToString() + LanguageTextManager.GetString("IDS_H1_156");
        //    this.m_curBtnID = friendData.sMsgRecvAnswerFriends_SC.dwFriendID;
        //    this.PlayerIcon.ChangeSprite((int)friendData.sMsgRecvAnswerFriends_SC.dProfession);

        //    ////TODO GuideBtnManager.Instance.RegGuideButton(RightButton.gameObject, MainUI.UIType.SocialInfo, SubUIType.IncreaseFriend, out m_guideBtnID);
        //}

        public void SetAttribute(PanelElementDataModel friendData,bool IsFriend)
        {
            this.PlayerName.text = friendData.sMsgRecvAnswerFriends_SC.Name;
            this.PlayerLevel.text = friendData.sMsgRecvAnswerFriends_SC.sActorLevel.ToString();
            this.m_curBtnID = friendData.sMsgRecvAnswerFriends_SC.dwFriendID;
            this.btnType = friendData.BtnType;
            this.PlayerProfassionIcon.ChangeSprite((int)friendData.sMsgRecvAnswerFriends_SC.dProfession);
            AddFriendBtn.gameObject.SetActive(!IsFriend);
            ResetSendBtnStatus();
        }

        public void SetElementActive(bool flag)
        {
            PlayerName.enabled = flag;
            PlayerLevel.enabled = flag;
            PlayerProfassionIcon.ChangeSprite(flag?1:0);
            AddFriendBtn.gameObject.SetActive(flag);
        }

        void OnDestroy()
        {
            ////TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
            Destroy(this.gameObject);
        }

        /// <summary>
        /// 加为好友按钮
        /// </summary>
        /// <param name="obj"></param>
        void RightButtonHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            switch (btnType)
            {
                case ButtonType.None:
                    {
                        //TraceUtil.Log("====>>>>no");
                    }
                    break;
                case ButtonType.NearlyPlayer:
                    {
                        if (FriendDataManager.Instance.GetFriendListData.Count >= FriendUIConst.FRIENDLIST_MAX)
                        {
                            //MessageBox.Instance.Show(5, "", "你的好友数量已满！", "确定", null);
                            MessageBox.Instance.Show(5, "", LanguageTextManager.GetString("IDS_H1_88"), LanguageTextManager.GetString("IDS_H2_55"), null);
                        }
                        else
                        {
                            SMsgAddFriends_CS sMsgAddFriends_CS = new SMsgAddFriends_CS();
                            sMsgAddFriends_CS.dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID;
                            sMsgAddFriends_CS.dwFriendID = m_curBtnID;

                            NetServiceManager.Instance.FriendService.SendAddFriendRequst(sMsgAddFriends_CS);
                            TraceUtil.Log("已发送加好友信息");
                            m_isSended = true;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 发送加好友后，按钮状态设置
        /// </summary>
        void SendAddFriendStatus()
        {
            m_sendTime = m_sendTime - Time.deltaTime;
            //RightButton.ButtonText.text = "已发送(" + ((int)m_sendTime + 1) + ")";
            //RightButton.ButtonText.text = string.Format(LanguageTextManager.GetString("IDS_H1_90"), ((int)m_sendTime + 1));
            //RightButton.SetButtonStatus(true);
            //RightButton.SetButtonTextColor(Color.gray);
            //RightButton.SetButtonActive(false);
            AddFriendBtn.BackgroundSprite.color = Color.gray;
            if (m_sendTime <= 0)
            {
                //RightButton.ButtonText.text = "加为好友";
                //RightButton.ButtonText.text = LanguageTextManager.GetString("IDS_H2_17");
                AddFriendBtn.BackgroundSprite.color = Color.white;
                m_isSended = false;
                m_sendTime = 3.0f;
                //RightButton.SetButtonStatus(false);
                //RightButton.SetButtonTextColor(Color.white);
                //RightButton.SetButtonActive(true);
            }
        }

        void ResetSendBtnStatus()
        {
            m_isSended = false;
            m_sendTime = 3;
            AddFriendBtn.BackgroundSprite.color = Color.white;
        }

        void FixedUpdate()
        {
            if (m_isSended)
            {
                SendAddFriendStatus();
            }
        }


    }
}