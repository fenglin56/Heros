using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace UI.Friend
{
    public class FriendElement_V2 : MonoBehaviour
    {


        public UILabel PlayerName;
        public UILabel PlayerLevel;
        public SpriteSwith ProfessionIcon;
        public SpriteSwith IsOnlineIcon;
        public SpriteSwith BackGruond;
        //public SpriteSwith PlayerIcon;
        //public UISprite EmailIcon;

        //public SingleButtonCallBack LeftButton;    //好友删除Button
        //public SingleButtonCallBack RightButton;    //加入队伍Button


        public SingleButtonCallBack DeleteFriendBtn;
        public SingleButtonCallBack JoinTeamBtn;
        public SingleButtonCallBack SureBtn;
        public SingleButtonCallBack TurnDownBtn;
        public SingleButtonCallBack SendMsgBtn;
        private ButtonType m_btnType = ButtonType.None;
        private uint m_curBtnID;
        private uint m_curPlayerLevel;
        private string m_profession;

        private uint[] guideBtnID = new uint[4];

        void Start()
        {
            DeleteFriendBtn.SetCallBackFuntion(OnDeleteFriendBtnClick);
            JoinTeamBtn.SetCallBackFuntion(OnJoinTeamBtnClick);
            SureBtn.SetCallBackFuntion(OnSureBtnClick);
            TurnDownBtn.SetCallBackFuntion(OnTurnDownBtnClick);
            SendMsgBtn.SetCallBackFuntion(OnSendMsgBtnClick);
            FriendDataManager.Instance.IsDelFriendIsMe = false;
        }

        public void SetAttribute(PanelElementDataModel friendData)
        {
            this.PlayerName.text = friendData.sMsgRecvAnswerFriends_SC.Name;
            this.m_curBtnID = friendData.sMsgRecvAnswerFriends_SC.dwFriendID;
            this.m_btnType = friendData.BtnType;
            this.m_curPlayerLevel = friendData.sMsgRecvAnswerFriends_SC.sActorLevel;
            PlayerLevel.SetText(m_curPlayerLevel.ToString());
            //this.m_profession = GetProfession(friendData.sMsgRecvAnswerFriends_SC.dProfession);
            this.ProfessionIcon.ChangeSprite((int)friendData.sMsgRecvAnswerFriends_SC.dProfession);
            IsOnlineIcon.ChangeSprite(friendData.sMsgRecvAnswerFriends_SC.IsOnLine?1:2);
            BackGruond.ChangeSprite(friendData.sMsgRecvAnswerFriends_SC.IsOnLine?1:2);
            ShowBtns(friendData);
        }

        public void SetElementActive(bool flag)
        {
            DeleteFriendBtn.gameObject.SetActive(flag);
            JoinTeamBtn.gameObject.SetActive(flag);
            SureBtn.gameObject.SetActive(flag);
            TurnDownBtn.gameObject.SetActive(flag);
            SendMsgBtn.gameObject.SetActive(flag);
            PlayerName.enabled = flag;
            PlayerLevel.enabled = flag;
            ProfessionIcon.ChangeSprite(flag?1:0);
            IsOnlineIcon.ChangeSprite(flag ? 1 : 0);
        }

        void ShowBtns(PanelElementDataModel friendData)
        {
            if (m_btnType == ButtonType.AddFriend)
            {
                DeleteFriendBtn.gameObject.SetActive(false);
                JoinTeamBtn.gameObject.SetActive(false);
                SureBtn.gameObject.SetActive(true);
                TurnDownBtn.gameObject.SetActive(true);
                SendMsgBtn.gameObject.SetActive(false);
            }
            else
            {
                DeleteFriendBtn.gameObject.SetActive(true);
                SureBtn.gameObject.SetActive(false);
                TurnDownBtn.gameObject.SetActive(false);
                
                if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_PLAYERROOM)
                {
                    SendMsgBtn.gameObject.SetActive(false);
                    JoinTeamBtn.gameObject.SetActive(false);
                }
                else
                {
                    SendMsgBtn.gameObject.SetActive(true);
                    if (friendData.sMsgRecvAnswerFriends_SC.IsOnLine && GetTeamType(friendData.sMsgRecvAnswerFriends_SC.dwTeamType) == TeamType.TeamLeader)
                    {
                        JoinTeamBtn.gameObject.SetActive(true);
                    }
                    else
                    {
                        JoinTeamBtn.gameObject.SetActive(false);
                    }
                }                
            }
        }

        private TeamType GetTeamType(uint dwTeamType)
        {
            switch (dwTeamType)
            {
                case 0:
                    return TeamType.NoTeam;
                case 2:
                    return TeamType.TeamMember;
                case 1:
                    return TeamType.TeamLeader;
                default:
                    return TeamType.NoTeam;
            }
        }

        private string GetProfession(uint dProfession)
        {
            switch (dProfession)
            {
                case 1:
                    return LanguageTextManager.GetString("IDS_D2_11"); //"刀客";
                case 2:
                    return LanguageTextManager.GetString("IDS_D2_12"); //"天师";
                case 3:
                    return LanguageTextManager.GetString("IDS_D2_13"); //"琴师";
                case 4:
                    return LanguageTextManager.GetString("IDS_D2_14"); //"刺客";
                default:
                    return "Error Return Result";
            }
        }

        /// <summary>
        /// 显示邮件图标
        /// </summary>
        /// <param name="isShow"></param>
        public void IsShowEmailIcon(bool isShow)
        {
            //this.EmailIcon.enabled = isShow;
        }

        void OnDestroy()
        {
            Destroy(this.gameObject);
        }

        /// <summary>
        /// 删除好友按钮
        /// </summary>
        /// <param name="obj"></param>
        void OnDeleteFriendBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            //MessageBox.Instance.Show(5, "", "确定要删除好友" + PlayerName.text , "删除好友", "取消", DelFriendMsgBox, null);
            MessageBox.Instance.Show(5, "", string.Format(LanguageTextManager.GetString("IDS_H1_94"), PlayerName.text, m_profession, m_curPlayerLevel.ToString()),
                LanguageTextManager.GetString("IDS_H2_30"), LanguageTextManager.GetString("IDS_H2_28"), DelFriendMsgBox, null);
        }
        /// <summary>
        /// 加入队伍按钮
        /// </summary>
        /// <param name="obj"></param>
        void OnJoinTeamBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            NetServiceManager.Instance.TeamService.SendJoinFriendTeamMsg(
                (uint)PlayerManager.Instance.FindHeroDataModel().ActorID, m_curBtnID);

        }
        /// <summary>
        /// 同意按钮
        /// </summary>
        /// <param name="obj"></param>
        void OnSureBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            if (FriendDataManager.Instance.GetFriendListData.Count >= FriendUIConst.FRIENDLIST_MAX)
            {
                //MessageBox.Instance.Show(5, "", "你的好友数量已满！", "确定", FriendFullMsgBox);
                MessageBox.Instance.Show(5, "", LanguageTextManager.GetString("IDS_H1_88"), LanguageTextManager.GetString("IDS_H2_55"), FriendFullMsgBox);
            }
            else
            {
                SMsgAnswerFriends_CS sMsgAnswerFriends_CS = new SMsgAnswerFriends_CS();
                sMsgAnswerFriends_CS.bAnswer = 1;  //0代表拒绝， 1代表同意
                sMsgAnswerFriends_CS.dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID;
                sMsgAnswerFriends_CS.dwbAnswerActorID = this.m_curBtnID;
                NetServiceManager.Instance.FriendService.SendAnswerFriendRequst(sMsgAnswerFriends_CS);
                //MessageBox.Instance.Show(5, "", "添加好友成功！", "确定");
            }

            DelCurFriendAddElement();
        }
        /// <summary>
        /// 拒绝按钮
        /// </summary>
        /// <param name="obj"></param>
        void OnTurnDownBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            SMsgAnswerFriends_CS sMsgAnswerFriends_CS = new SMsgAnswerFriends_CS();
            sMsgAnswerFriends_CS.bAnswer = 0;  //0代表拒绝， 1代表同意
            sMsgAnswerFriends_CS.dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID;
            sMsgAnswerFriends_CS.dwbAnswerActorID = this.m_curBtnID;

            NetServiceManager.Instance.FriendService.SendAnswerFriendRequst(sMsgAnswerFriends_CS);
            DelCurFriendAddElement();
            //MessageBox.Instance.Show(5, "", "已拒绝对方的请求！", "确定");
            MessageBox.Instance.Show(5, "", LanguageTextManager.GetString("IDS_H1_91"), LanguageTextManager.GetString("IDS_H2_55"));
        }
        //发送私聊
        void OnSendMsgBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            MainUI.MainUIController.Instance.CloseAllPanel();
            UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenPrivateChatWindow, new Chat.TalkerInfo()
            {
                ActorID = (int)m_curBtnID,
                Name = PlayerName.text
            });            
        }
        /// <summary>
        ///删除好友确认调用函数 
        /// </summary>
        void DelFriendMsgBox()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            FriendDataManager.Instance.IsDelFriendIsMe = true;
            ////TODO GuideBtnManager.Instance.DelGuideButton(guideBtnID[2]);
            SMsgDelFriends_CS sMsgDelFriends_CS = new SMsgDelFriends_CS();
            sMsgDelFriends_CS.dwActorID = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID;
            sMsgDelFriends_CS.dwbDelActorID = this.m_curBtnID;
            NetServiceManager.Instance.FriendService.SendDelFriendRequst(sMsgDelFriends_CS);
            DelFriendListItem();
        }

        /// <summary>
        /// 删除当前好友的UI元素，并排序
        /// </summary>
        void DelFriendListItem()
        {
            List<PanelElementDataModel> tempList = FriendDataManager.Instance.GetFriendListData;
            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i].sMsgRecvAnswerFriends_SC.dwFriendID == m_curBtnID)
                {
                    //Destroy(tempList[i].BtnObj);
                    tempList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 好友已满处理函数
        /// </summary>
        void FriendFullMsgBox()
        {
            DelCurFriendAddElement();
        }

        /// <summary>
        /// 删除当前好友申请的UI元素，并排序
        /// </summary>
        void DelCurFriendAddElement()
        {
            ////TODO GuideBtnManager.Instance.DelGuideButton(guideBtnID[0]);
            ////TODO GuideBtnManager.Instance.DelGuideButton(guideBtnID[1]);

            List<PanelElementDataModel> tempList = FriendDataManager.Instance.GetRequestListData;
            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i].sMsgRecvAnswerFriends_SC.dwFriendID == m_curBtnID)
                {
                    //Destroy(tempList[i].BtnObj);
                    tempList.RemoveAt(i);
                }
            }

            FriendDataManager.Instance.SetFriendListDepth(FriendUIConst.UI_SHOW_ZDEPTH);
            FriendDataManager.Instance.IsUpdateMsgNum = true;
            FriendDataManager.Instance.IsUpdateFriendList = true;
        }
    }
}