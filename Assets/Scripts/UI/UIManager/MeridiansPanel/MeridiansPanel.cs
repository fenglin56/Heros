using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class MeridiansPanel : BaseUIPanel
    {

        public SingleButtonCallBack MeridiansNumLabel;
        //public SingleButtonCallBack BackBtn;
        public MeridiansDesTipsLabel MeridiansDesLabel;//经脉介绍label
        //public SingleButtonCallBack PageLeftObj;//左箭头
        //public SingleButtonCallBack PageRightObj;//右箭头
        //public UISprite KongfuNameLabel;
        public UILabel HintLabel;//提示练功房label
        public PageNumberTipsPanel PageNumberTipsPanel;

        public GameObject[] MeridiansDragPanelPrefabList;//所有内功面板的预制体
        public List<MeridiansDragPanel> MeridiansDragPanelList = new List<MeridiansDragPanel>();
        public MeridiansAttributePanel meridiansAttributePanel;
        public PracticeButton practiceButton;
        public Transform Grid;
        public UIDraggablePanel DraggablePanel;
        //public GameObject UISliderEffect;//经脉条特效
        public Transform CreatKongfuBackgroundPoint;

        public GameObject UIBottomBtnPrefab;
        public Transform CreatBottomBtnPoint;

        public SpringPanel springPanel;
        //public UIPanel KongfuListPanel;
        public int CurrentPageID {get;private set;}

        public int MaxMeridiansID { get; private set; }//最大的经脉ID

        public GameObject KongfuSuccessEffect;//修炼成功动画Prefab
        public GameObject kongfuThroughEffect;//打通经脉动画prefab
        private GameObject KongfuThroughEffectInstance;//打通经脉的实例

        private CommonUIBottomButtonTool commonUIBottomButtonTool;

        public PlayerMeridiansDataManager PlayerMeridiansDataManager;

        private int[] m_guideBtnID;

        public GameObject CommonToolPrefab;
        void Awake()
        {
            if (UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform); }
            PlayerMeridiansDataManager.playerMeridiansDataBase.PlayermeridiansDataList.ApplyAllItem(P => MaxMeridiansID = MaxMeridiansID < P.MeridiansLevel ? P.MeridiansLevel : MaxMeridiansID);
            PageNumberTipsPanel.InitTips(MeridiansDragPanelPrefabList.Length);
            PageNumberTipsPanel.SetActivePageID(1);
            m_guideBtnID = new int[2];
            CurrentPageID = 0;
            //BackBtn.SetCallBackFuntion(OnBackBtnClick);
            this.springPanel.OnDragCallBack = OnPageTurning;
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), SetMeridiansOwnLabel);
            //TODO GuideBtnManager.Instance.RegGuideButton(practiceButton.gameObject, UIType.Meridians, SubType.ButtomCommon, out m_guideBtnID[0]);
        }

        void Start()
        {
            ShowPageArrowAndPracticeButtonActive();
        }

        void ShowBottomBtn()
        {
            CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonTapped);
            commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() { btnInfo });

            var btnInfoComponet = commonUIBottomButtonTool.GetButtonComponent(btnInfo);
            if (btnInfoComponet != null)
            {
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfoComponet.gameObject, UIType.Meridians, SubType.ButtomCommon, out m_guideBtnID[1]);
            }
            
        }

        void OnDestroy()
        {
            for (int i = 0 ; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), SetMeridiansOwnLabel);
        }

        /// <summary>
        /// 更新单实体属性
        /// </summary>
        /// <param name="obj"></param>
        void SetMeridiansOwnLabel(INotifyArgs iNotifyArgs)
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)iNotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                TraceUtil.Log("刷新面板属性");
                this.MeridiansNumLabel.SetButtonText(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_PRACTICE_NUM.ToString());
                MeridiansDragPanelList.ApplyAllItem(P=>P.InitPanel(this));
                meridiansAttributePanel.Show(this);
            }
        }

        void InitPanel()
        {
            meridiansAttributePanel.Show(this);
            practiceButton.InitMySelf(this);
            Grid.ClearChild();
            MeridiansDragPanelList.Clear();
            //DraggablePanel.ResetPosition();
            this.MeridiansNumLabel.SetButtonText(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_PRACTICE_NUM.ToString());
            //PlayerKongfuData[] kongfuDataList = PlayerMeridiansDataManager.Instance.PlayerKongfuDataBase.PlayerKongfuDataList;
            foreach (GameObject child in MeridiansDragPanelPrefabList)
            {
                MeridiansDragPanel meridiansDragPanel = CreatObjectToNGUI.InstantiateObj(child, Grid).GetComponent<MeridiansDragPanel>();
                meridiansDragPanel.InitPanel(this);
                MeridiansDragPanelList.Add(meridiansDragPanel);
            }
            ShowKonfuName();
            TurningToUnlockPage();
        }

        /// <summary>
        /// 跳转到最新的解锁界面
        /// </summary>
        void TurningToUnlockPage()
        {
            int NewPageID = 0;
            foreach (var child in MeridiansDragPanelList)
            {
                if (child.IsUnlock&&NewPageID<child.PanelPositionID)
                {
                    NewPageID = child.PanelPositionID;
                }
            }
            TraceUtil.Log("newPageID:"+NewPageID);
            Vector3 newPosition = Grid.parent.transform.localPosition;
            newPosition.x = -850 * NewPageID;
            Grid.parent.transform.localPosition = newPosition;

            UIPanel clipPanel =Grid.parent.transform.GetComponent<UIPanel>();
            Vector4 newClip = clipPanel.clipRange;
            newClip.x = -newPosition.x;
            clipPanel.clipRange = newClip;

            OnPageTurning(null);
        }

        /// <summary>
        ///  当正在翻页中
        /// </summary>
        /// <param name="obj"></param>
        void OnPageTurning(object obj)
        {
            float distance = Grid.parent.localPosition.x;
            int PageID = (int)((Mathf.Abs(distance) / 850) + 0.5f);
            //TraceUtil.Log("CurrentPageID"+PageID);
            if (PageID != CurrentPageID)
            {
                PageNumberTipsPanel.SetActivePageID(PageID+1);
                CurrentPageID = PageID;
                ShowMeridiansDesInfo(0,false,"0");
                ShowPageArrowAndPracticeButtonActive();
                MeridiansDragPanelList.ApplyAllItem(P => P.DisabelMyBtnsSelectActive());
                ShowKonfuName();
            }
        }

        void ShowKonfuName()
        {
            MeridiansDragPanel meridiansDragPanel = MeridiansDragPanelList.First(P => P.PanelPositionID == CurrentPageID);
            //this.KongfuNameLabel.spriteName = (meridiansDragPanel.playerKongfuData.KongfuNameRes);
            CreatKongfuBackgroundPoint.ClearChild();
            CreatObjectToNGUI.InstantiateObj(meridiansDragPanel.playerKongfuData.KongfuPicPrefab,CreatKongfuBackgroundPoint);
        }

        int KongfuNameToSpriteID()
        {
            int ID = 0;
            
            return ID;
        }

        /// <summary>
        /// 显示左右翻页提示箭头和修炼按钮显示的特效
        /// </summary>
        void ShowPageArrowAndPracticeButtonActive()
        {
            int currentMeridianID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MERIDIANS_LEVEL;
            MeridiansDragPanel meridiansDragPanel = MeridiansDragPanelList.FirstOrDefault(P => P.CheckIsInMyPanel(currentMeridianID) == true);
            if (meridiansDragPanel == null)
                return;
            int MaxPageNum = 0;
            foreach (MeridiansDragPanel child in MeridiansDragPanelList)
            {
                if (child.PanelPositionID > MaxPageNum)
                {
                    MaxPageNum = child.PanelPositionID;
                }
            }
            Color enabelColor = new Color(1, 1, 1, 1);
            Color disabelColor = new Color(1, 1, 1, 0.3f);
            //PageLeftObj.SetActive(CurrentPageID == 0 ? false : true);
            //PageRightObj.SetActive(CurrentPageID == MaxPageNum ? false : true);
            //PageLeftObj.BackgroundSprite.color = CurrentPageID == 0 ? disabelColor : enabelColor;
            //PageRightObj.BackgroundSprite.color = CurrentPageID == MaxPageNum ? disabelColor : enabelColor;
            practiceButton.SetEffectActive(meridiansDragPanel.PanelPositionID == CurrentPageID);
        }

        /// <summary>
        ///  当点击某一个经脉按钮的时候
        /// </summary>
        /// <param name="meridiansID"></param>
        public void OnMeridiansBtnClick(int meridiansID)
        {
            MeridiansDragPanelList.ApplyAllItem(P => P.OnMeridiansBtnClick(meridiansID));
        }

        /// <summary>
        /// 增加经脉修为
        /// </summary>
        /// <param name="Flag">true开始false停止</param>
        public void AddMeridians(bool Flag)
        {
            int currentAddMeridianID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MERIDIANS_LEVEL+1;
            MeridiansDragPanel meridiansDragPanel = MeridiansDragPanelList.FirstOrDefault(P => P.getMeridiansBtn(currentAddMeridianID) != null);
            if (meridiansDragPanel == null)
            {
                //已修炼完成所有提示
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_519"), 1);
                return;
            }
            if (CurrentPageID > meridiansDragPanel.PanelPositionID)
            {
                //需要修炼完xxx内功提示
                if (Flag)
                {
                    var LastMeridiansPanel = MeridiansDragPanelList.FirstOrDefault(P => P.PanelPositionID == CurrentPageID - 1);
                    if (LastMeridiansPanel != null)
                    {
                        string Msg = string.Format(LanguageTextManager.GetString("IDS_H1_375"), LanguageTextManager.GetString(LastMeridiansPanel.playerKongfuData.KongfuName));
                        MessageBox.Instance.ShowTips(3, Msg, 1);
                    }
                }
                return;
            }
            else if (CurrentPageID < meridiansDragPanel.PanelPositionID)
            {
                //已修炼完成提示
                if (Flag)
                {
                    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_373"),1);
                }
                return;
            }
            //else if (currentAddMeridianID == MaxMeridiansID && meridiansDragPanel.getMeridiansBtn(currentAddMeridianID).IsUnlock)
            //{
            //    //已修炼完成所有提示
            //    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_519"), 1);
            //}
            else
            {
                MeridiansDragPanelList.ApplyAllItem(P => P.OnAddMeridiansBtnClick(Flag));
            }
        }

        /// <summary>
        /// 显示经脉介绍文字
        /// </summary>
        public void ShowMeridiansDesInfo(int MeridiansID,bool IsUnlock,string needAddNumber)
        {
            if (MeridiansID == 0)
            {
                MeridiansDesLabel.gameObject.SetActive(false);
                return;
            }
            MeridiansDesLabel.gameObject.SetActive(true);
            PlayerMeridiansData playerMeridiansData = PlayerMeridiansDataManager.GetMeridiansData(MeridiansID);
            string meridiansName = LanguageTextManager.GetString(playerMeridiansData.KongfuName);
            string[] addEffect = playerMeridiansData.EffectAdd.Split('+');
            EffectData effectData = ItemDataManager.Instance.EffectDatas._effects.First(P=>P.m_SzName == addEffect[0]);
            string addEffName = LanguageTextManager.GetString(effectData.IDS);
            string addEffNum = NGUIColor.SetTxtColor(HeroAttributeScale.GetScaleAttribute(effectData,int.Parse(addEffect[1])),TextColor.white);
            if (!IsUnlock)
            {
                needAddNumber = needAddNumber == "0" ? playerMeridiansData.LevelUpNeed.ToString() : needAddNumber;
                needAddNumber = NGUIColor.SetTxtColor(needAddNumber, TextColor.white);
                MeridiansDesLabel.Show(meridiansName, addEffName, addEffNum, needAddNumber);
            }
            else
            {
                MeridiansDesLabel.Show(meridiansName, addEffName, addEffNum);
            }
        }

        void OnBackButtonTapped(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            Close();
            CleanUpUIStatus();
        }

        //public override void Show()
        //{
        //    transform.localPosition = Vector3.zero;
        //    InitPanel();
        //    if (commonUIBottomButtonTool == null)
        //    {
        //        commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
        //        ShowBottomBtn();
        //    }
        //    else
        //    {
        //        commonUIBottomButtonTool.ShowAnim();
        //    }
        //}

        public override void Show(params object[] value)
        {
            base.Show(value);
            InitPanel();
            if (commonUIBottomButtonTool == null)
            {
                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
                ShowBottomBtn();
            }
            else
            {
                commonUIBottomButtonTool.ShowAnim();
            }
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            base.Close();
        }
        
        //public GameObject KongfuSuccessEffect;//修炼成功动画Prefab
        //public GameObject kongfuThroughEffect;//打通经脉动画prefab
        /// <summary>
        /// 修炼成功动画
        /// </summary>
        /// <param name="creatTransfrom"></param>
        public void CreatMeridianSeccessEffect(Transform creatTransfrom)
        {
            GameObject CreatObj = CreatObjectToNGUI.InstantiateObj(KongfuSuccessEffect,creatTransfrom);
            StartCoroutine(DestroyEffectAnimForTime(5,CreatObj));
        }
        /// <summary>
        /// 打通经脉动画
        /// </summary>
        /// <param name="CreatTransform"></param>
        public void CreatKonfuThroughEffect(Transform CreatTransform)
        {
            if (KongfuThroughEffectInstance != null)
            {
                Destroy(KongfuThroughEffectInstance);
            }
            KongfuThroughEffectInstance = CreatObjectToNGUI.InstantiateObj(kongfuThroughEffect, CreatTransform);
            StartCoroutine(DestroyEffectAnimForTime(4, KongfuThroughEffectInstance));
        }

        IEnumerator DestroyEffectAnimForTime(float waitTime, GameObject DestroyObj)
        {
            yield return new WaitForSeconds(waitTime);
            if (DestroyObj != null)
            {
                Destroy(DestroyObj);
            }
        }

    }
}