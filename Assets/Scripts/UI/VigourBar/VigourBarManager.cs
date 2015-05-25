using UnityEngine;
using System.Collections;

namespace UI
{

    public class VigourBarManager : View
    {

        public SingleButtonCallBack Button_AddVigour;
        public UISlider VigourBar;
        public UILabel VigourLabel;

        public GameObject BuyVigourMessagePrefab;
        private VigourMessagePanel BuyVigourMessagePanel;

        private float CurrentVigour;
        private float MaxVigour;

        //private uint m_guideBtnID;

        void Awake()
        {
            ////TODO GuideBtnManager.Instance.RegGuideButton(Button_AddVigour.gameObject, MainUI.UIType.EctypeInfo, SubUIType.NoSubType, out m_guideBtnID);
        }

        void Start()
        {
            Button_AddVigour.SetCallBackFuntion(OnAddVigourBtnClick);
            RegisterEventHandler();
            SetVigourValue();
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateValues);
            //AddEventHandler(EventTypeEnum.TeamActiveLifeNotEnough.ToString(), ShowNoEnoughVigourPanel);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.NoEnoughActiveLife, ShowNoEnoughVigourPanel);
        }

        void OnDestroy()
        {
            ////TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateValues);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NoEnoughActiveLife, ShowNoEnoughVigourPanel);
        }


        void UpdateValues(INotifyArgs inotifyArgs)
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                SetVigourValue();
            }
        }

        void SetVigourValue()
        {
            string Title = LanguageTextManager.GetString("IDS_H1_120");
            if (
            CurrentVigour != PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE ||
            MaxVigour != PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAX_ACTIVELIFE)
            {
                CurrentVigour = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
                MaxVigour = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAX_ACTIVELIFE;
                VigourLabel.text = string.Format("{0} {1}/{2}", Title, CurrentVigour, MaxVigour);
                VigourBar.sliderValue = CurrentVigour / MaxVigour;
            }
        }

        void OnAddVigourBtnClick(object obj)
        {
            if (BuyVigourMessagePanel == null) { BuyVigourMessagePanel = CreatObjectToNGUI.InstantiateObj(BuyVigourMessagePrefab, transform).GetComponent<VigourMessagePanel>(); }
            string ShowStr = string.Format(LanguageTextManager.GetString("IDS_H1_158"), 40);
            BuyVigourMessagePanel.Show(string.Format("{0}\n{1}", LanguageTextManager.GetString("IDS_H1_166"), ShowStr));
        }

        void ShowNoEnoughVigourPanel(object obj)
        {   
            if (BuyVigourMessagePanel == null) { BuyVigourMessagePanel = CreatObjectToNGUI.InstantiateObj(BuyVigourMessagePrefab, transform).GetComponent<VigourMessagePanel>(); }
            string ShowStr = string.Format(LanguageTextManager.GetString("IDS_H1_158"), 40);
            BuyVigourMessagePanel.Show(string.Format("{0}\n{1}", LanguageTextManager.GetString("IDS_H1_161"), ShowStr));
        }

        //Add by Lee
        public void ShowNoEnoughVigourPanel()
        {
            this.ShowNoEnoughVigourPanel(null);
        }

    }
}