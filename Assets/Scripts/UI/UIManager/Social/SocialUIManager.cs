using UnityEngine;
using System.Collections;
using UI.MainUI;

namespace UI.Friend
{

    public class SocialUIManager : BaseUIPanel
    {
        /// <summary>
        /// 好友列表和附近玩家面板，一级面板
        /// </summary>

        public FriendListPanel FriendListPanel;//好友列表信息，需手动关联
        public NearlyPlayerListPanel NearlyPlayerListPanel;//好友列表信息，需手动关联

        public GameObject CommonToolPrefab;

        void Awake()
        {
            if (UI.MainUI.MainUIController.Instance != null&&UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform); }
            this.RegisterEventHandler();

            if (FriendListPanel == null)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"好友列表Prefab未指定！");
            }

            this.NearlyPlayerListPanel.Init(this);
            this.FriendListPanel.Init(this);

            UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeLevelError, ShowFriendEctypeLockMsg);
        }

        protected override void RegisterEventHandler()
        {
            this.AddEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccessHandle);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeLevelError, ShowFriendEctypeLockMsg);
            this.RemoveEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccessHandle);
        }

        public override void Show(params object[] value)
        {
            ShowFriendListPanel(null);
            base.Show(value);
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            base.Close();
        }

        public void OnBackBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            Close();
            CleanUpUIStatus();
        }

        public void OnBackButtonTapped(object obj)
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            OnBackBtnClick(obj);
        }

        public void ShowFriendListPanel(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            FriendDataManager.Instance.CurPanelState = PanelState.MYFRIENDLIST;
            transform.localPosition = Vector3.zero;
            NearlyPlayerListPanel.CloseNearlyPlayerPanel();
            FriendListPanel.ShowFriendList();
        }

        public void ShowNearlyPlayerPanel(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            FriendDataManager.Instance.CurPanelState = PanelState.NEARLYPLAYER;
            transform.localPosition = Vector3.zero;
            FriendListPanel.CloseFriendPanel();
            NearlyPlayerListPanel.Show();
        }


        public void AddFriendSuccessHandle(INotifyArgs notifyArgs)
        {
            if (IsShow)
            {
                ShowFriendListPanel(null);
            }
        }

        /// <summary>
        /// 好友副本未解锁
        /// </summary>
        void ShowFriendEctypeLockMsg(object obj)
        {
            if (IsShow)
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_84"), 1);
            }
        }
    }


    public enum PanelState
    {
        NO_PANEL,
        MYFRIENDLIST,
        NEARLYPLAYER,
    }

    public class FriendUIConst
    {
        public const short UI_NO_SHOW_ZDEPTH = -1000;
        public const short UI_SHOW_ZDEPTH = 0;
        public const short FRIENDLIST_MAX = 30;
    }
}
