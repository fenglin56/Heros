using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Text;
using UI.Friend;
using System.Linq;
namespace UI.Ranking
{
    public class PlayerDetail_Ranking :  BaseSubUiPanel{
        public SingleButtonCallBack CloseButton;
        public RoleEquiptPanel_Readonly RoleEquiptPanel;
        public HeroAttributePanel_Ranking RoleAttributePanel;
        public SingleButtonCallBack ChatBtn;
        public SingleButtonCallBack FriendBtn;
        public SingleButtonCallBack EmailBtn;
        private string Name;
        private int FriendsID;

        void Awake()
        {
            RegisterEventHandler();
            CloseButton.SetCallBackFuntion(OnCloseButtonClick);
            ChatBtn.SetCallBackFuntion(OnChatClick);
            //FriendBtn.SetCallBackFuntion(OnAddFriendClick);
            EmailBtn.SetCallBackFuntion(OnEmailClick);

        }

        void DeleteFriendHandel(object obj)
        {
            SetIsNotFriend();
        }
        void UpdateDetailPanelHandel(object obj)
        {
            
            RoleEquiptPanel.ShowForTime(PlayerRankingDataManager.Instance.RankingDetail);
            RoleAttributePanel.UpdateAttribute(PlayerRankingDataManager.Instance.RankingDetail);
            FriendsID=(int)PlayerRankingDataManager.Instance.RankingDetail.dwActorID;
            Name=Encoding.UTF8.GetString(PlayerRankingDataManager.Instance.RankingDetail.szActorName);

            InitButtons();

        }

        void SetIsNotFriend()
        {
            FriendBtn.spriteSwithList.ApplyAllItem(p => p.ChangeSprite(2));
            FriendBtn.SetCallBackFuntion(OnAddFriendClick);
            FriendBtn.spriteSwithList.ApplyAllItem(p => p.GetComponent<UISprite>().color = Color.white);
            FriendBtn.SetMyButtonActive(true);
            EmailBtn.spriteSwithList.ApplyAllItem(p => p.GetComponent<UISprite>().color = Color.gray);
            EmailBtn.SetMyButtonActive(false);
        }

        void SetIsFriend()
        {
            //是好友
            FriendBtn.spriteSwithList.ApplyAllItem(p => p.ChangeSprite(1));
            FriendBtn.SetCallBackFuntion(OnDeleteFriendClick);
            FriendBtn.spriteSwithList.ApplyAllItem(p => p.GetComponent<UISprite>().color = Color.white);
            FriendBtn.SetMyButtonActive(true);
            EmailBtn.spriteSwithList.ApplyAllItem(p => p.GetComponent<UISprite>().color = Color.white);
            EmailBtn.SetMyButtonActive(true);
        }
        void SetIsSelf()
        {
            FriendBtn.spriteSwithList.ApplyAllItem(p => p.GetComponent<UISprite>().color = Color.gray);
            FriendBtn.SetMyButtonActive(false);
            EmailBtn.spriteSwithList.ApplyAllItem(p => p.GetComponent<UISprite>().color = Color.gray);
            EmailBtn.SetMyButtonActive(false);
            ChatBtn.spriteSwithList.ApplyAllItem(p => p.GetComponent<UISprite>().color = Color.gray);
            ChatBtn.SetMyButtonActive(false);
        }

        void SetIsNotSelf()
        {
            FriendBtn.spriteSwithList.ApplyAllItem(p => p.GetComponent<UISprite>().color = Color.white);
            FriendBtn.SetMyButtonActive(true);
            EmailBtn.spriteSwithList.ApplyAllItem(p => p.GetComponent<UISprite>().color = Color.white);
            EmailBtn.SetMyButtonActive(true);
            ChatBtn.spriteSwithList.ApplyAllItem(p => p.GetComponent<UISprite>().color = Color.white);
            ChatBtn.SetMyButtonActive(true);
        }
        void SetAddFriendBtnDesable()
        {
            FriendBtn.spriteSwithList.ApplyAllItem(p=>p.GetComponent<UISprite>().color=Color.gray);
            FriendBtn.SetMyButtonActive(false);
        }
        public void InitButtons()
        {
            if (PlayerRankingDataManager.Instance.RankingDetail.dwActorID == PlayerManager.Instance.FindHeroDataModel().ActorID)
            {
                SetIsSelf();
            }
            else
            {
                SetIsNotSelf();
                if( FriendDataManager.Instance.GetFriendListData.Where(p=>p.sMsgRecvAnswerFriends_SC.dwFriendID==PlayerRankingDataManager.Instance.RankingDetail.dwActorID).Count()==0)
                {
                    //不是好友
                    SetIsNotFriend();
                }
                else
                {
                    SetIsFriend();
                    
                }
              

            }
           
          
        }

    
        protected override void RegisterEventHandler()
        {
            base.RegisterEventHandler();
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveRankingDetailRes,UpdateDetailPanelHandel);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.DeleteFriendSuccess,DeleteFriendHandel);
            //RaiseEvent(EventTypeEnum.AddFriendSuccess.ToString(), null);
            AddEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccess);
        }
        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveRankingDetailRes,UpdateDetailPanelHandel);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.DeleteFriendSuccess,DeleteFriendHandel);
            RemoveEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccess);
        }
    
    	void AddFriendSuccess(INotifyArgs e)
        {
            SetIsFriend();
        }

        void OnCloseButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_DetailExit");
            CloseDatailPanel();
        }
        public void CloseDatailPanel()
        {
            RoleEquiptPanel.m_RoleModelPanel.Close();
            // RoleEquiptPanel.m_RoleModelPanel.transform.ClearChild();
            HidePanel();
        }
        void OnChatClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_Chat");
            MainUIController.Instance.OpenMainUI(UIType.Chat,Chat.WindowType.Private,FriendsID,Name);
            OnCloseButtonClick(null);
        }

        void OnEmailClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_Mail");
            MainUIController.Instance.OpenMainUI(UIType.Mail,EmaiSubPageStatus.EmailWrite,FriendsID,Name);
            OnCloseButtonClick(null);
        }

        void OnAddFriendClick(object obj)
        {
            SetAddFriendBtnDesable();
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_AddFriend");
            SMsgAddFriends_CS msg=new SMsgAddFriends_CS()
            {
                dwActorID=(uint)PlayerManager.Instance.FindHeroDataModel().ActorID,
                dwFriendID=(uint)FriendsID,
            };
            NetServiceManager.Instance.FriendService.SendAddFriendRequst(msg);
        }

        void CancelDeleteHandle()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendDeleteCancel");
        }

        void SureDeleteHandle()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendDeleteConfirmation");
            FriendDataManager.Instance.IsDelFriendIsMe = true;  
            SMsgDelFriends_CS msg=new SMsgDelFriends_CS()
            {
                dwActorID=(uint)PlayerManager.Instance.FindHeroDataModel().ActorID,
                dwbDelActorID=(uint)FriendsID,
            };
            NetServiceManager.Instance.FriendService.SendDelFriendRequst(msg);
        }

        void OnDeleteFriendClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_DelFriend");
            string str = string.Format(LanguageTextManager.GetString("IDS_I23_9"),Name);
            MessageBox.Instance.Show(4,"",str,LanguageTextManager.GetString("IDS_I23_10"),LanguageTextManager.GetString("IDS_I23_11"),CancelDeleteHandle,SureDeleteHandle);

        }
    }
}
