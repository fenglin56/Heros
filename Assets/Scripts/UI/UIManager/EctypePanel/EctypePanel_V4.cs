using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace UI.MainUI
{

    public class EctypePanel_V4 : View
    {

        //public SingleButtonCallBack BackButton;
        //public SingleButtonCallBack BtnTeamWork;
        //public SingleButtonCallBack BtnTeamWorkSpeed;
        //public LocalButtonCallBack HelpBtn;
        //public GameObject HelpPanel;
        //public GameObject VigourBarPrefab;
        public SingleButtonCallBack BtnGo;
        public SingleButtonCallBack CostLabel;
        public UILabel NameLabel;
        public GameObject LocalEctypePanelListPrefab;
        public Transform Grid;
        public SpringPanel springPanel;
        public PageNumberTipsPanel PageNumberTips;
        public GameObject CostLabelPreafab;//点击开始战斗后花费tips
        public EctypeContainerIconPrefabDataBase EctypeContainerIconData;

        //public GameObject UITitlePrefab;
        public GameObject UIBottomBtnPrefab;
        public Transform CreatBottomBtnPoint;
        private CommonUIBottomButtonTool commonUIBottomButtonTool;
        //private GameObject m_helpPanel;

        List<LocalEctypePanelList_v3> LocalEctypePanelList = new List<LocalEctypePanelList_v3>();

        private LocalEctypePanel_v3 OnSelectEctypeCard;
        SMSGEctypeLevelData_SC sMSGEctypeLevelData_SC;

        public EctypePanelMessageBox ectypePanelMessageBox;

        public readonly int PageDistance = 1200;

        public EctypePanleManager MyParent;

        private int[] m_guideBtnID = new int[7];

        private SMSGEctypeSelect_SC sMSGEctypeSelect_SC;
        private int CurrentPageID = -1;
        private int CurrentPageNumber = 0;//默认第几页

        //妖气
        public UISlider Slider_Yaoqi;
        public UILabel Label_YaoqiValue;
        public SingleButtonCallBack Button_Unknown;
        public SingleButtonCallBack Button_Add;
        public GameObject YaoqiExplanation;
        public SingleButtonCallBack Button_Sure;
        public GameObject Animation_CutYaoqiItem;

        private int CurrentEctypeIndex = 0;//当前副本

        private GameObject TweenFloatControl = null;

        private bool IsShowCostLabel = false;


        public GameObject CommonToolPrefab;
        void Awake()
        {
            if (UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform); }
            BtnGo.SetCallBackFuntion(CheckQuickJionBackpack);
            //HelpBtn.SetCallBackFuntion(HelpButtonHandle);
            //BtnGo.SetButtonText(LanguageTextManager.GetString("IDS_H2_64"));
            Button_Unknown.SetCallBackFuntion(ShowYaoqiExplanation);
            Button_Add.SetCallBackFuntion(AddYaoqiValue);
            Button_Sure.SetCallBackFuntion(CloseYaoqiExplanation);
            InitPanel();
            springPanel.OnDragCallBack = this.OnPageTurning;
            
            this.RegisterEventHandler();
            ShowBottomBtn();

            //TODO GuideBtnManager.Instance.RegGuideButton(BtnGo.gameObject, UIType.Battle, SubType.EctypeBattleGo, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Unknown.gameObject, UIType.Battle, SubType.ShowYaoqiExp, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Add.gameObject, UIType.Battle, SubType.AddYaoqiValue, out m_guideBtnID[2]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Sure.gameObject, UIType.Battle, SubType.CloseYaoqiExp, out m_guideBtnID[3]);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypePageSkip, EctypePageSkip);

            GuideBtnManager.Instance.RegDraggablePanel(springPanel.gameObject);
            //CreatObjectToNGUI.InstantiateObj(UITitlePrefab,transform);
        }

        void ShowBottomBtn()
        {
            commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab,CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
            CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonTapped);
            CommonBtnInfo btnInfo1 = new CommonBtnInfo(1, "JH_UI_Button_1116_12", "JH_UI_Button_1116_00", OnTeamWorkBtnClick);
            CommonBtnInfo btnInfo2 = new CommonBtnInfo(2, "JH_UI_Button_1116_11", "JH_UI_Button_1116_00", OnTeamSpeedBtnClick);
            commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() {btnInfo,btnInfo1,btnInfo2});

            var btnInfoComponent = commonUIBottomButtonTool.GetButtonComponent(btnInfo);
            //if(btnInfoComponent != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfoComponent.gameObject, UIType.Battle, SubType.ButtomCommon, out m_guideBtnID[4]);
            var btnInfo1Component = commonUIBottomButtonTool.GetButtonComponent(btnInfo1);
            //if (btnInfo1Component != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfo1Component.gameObject, UIType.Battle, SubType.ButtomCommon, out m_guideBtnID[5]);
            var btnInfo2Component = commonUIBottomButtonTool.GetButtonComponent(btnInfo2);
            //if (btnInfo2Component != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfo2Component.gameObject, UIType.Battle, SubType.ButtomCommon, out m_guideBtnID[6]);
        }

        void OnBackButtonTapped(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
			GameManager.Instance.PlaySceneMusic();
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            MyParent.OnClosePanelBtnClick();
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            //this.RemoveEventHandler(EventTypeEnum.GuideGoToBattle.ToString(), GoToBattleHandle);
            RemoveEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RemoveColdWork, SirenColdWorkHandle);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypePageSkip, EctypePageSkip);

            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }

            GuideBtnManager.Instance.DelDraggable(springPanel.gameObject);
        }

        public void Init(EctypePanleManager myParent)
        {
            this.MyParent = myParent;
            transform.localPosition = Vector3.zero;
            if (commonUIBottomButtonTool != null)
            {
                commonUIBottomButtonTool.ShowAnim();
            }
        }

        public void InitPanel()
        {
            ResetPageTips();
            var PanelList = EctypeConfigManager.Instance.EctypeSelectConfigFile._dataTable.Where(P=>P.lEctypeType == 0).ToArray();
            if (LocalEctypePanelList.Count > 1)
                return;
            PageNumberTips.InitTips(PanelList.Length);
            for (int i = 0; i < PanelList.Length; i++)
            {
                LocalEctypePanelList_v3 ectypePanelParent = null;
                ectypePanelParent = CreatObjectToNGUI.InstantiateObj(LocalEctypePanelListPrefab, Grid).GetComponent<LocalEctypePanelList_v3>();
                ectypePanelParent.gameObject.SetActive(true);
                ectypePanelParent.InitPanel(i, PanelList[i], this);
                
                LocalEctypePanelList.Add(ectypePanelParent);
            }
        }

        public void UnlockPanel(SMSGEctypeSelect_SC sMSGEctypeSelect_SC)
        {
            //sMSGEctypeSelect_SC.sMSGEctypeData_SCs.ApplyAllItem(P => TraceUtil.Log("解锁的副本ID："+P.dwEctypeID+","+P.byDiff));
            this.sMSGEctypeSelect_SC = sMSGEctypeSelect_SC;
//            foreach (var child in sMSGEctypeSelect_SC.sMSGEctypeData_SCs)
//            {
//                var EctypeListPanel = LocalEctypePanelList.SingleOrDefault(P => P.ectypeSelectConfigData._lEctypeID == child.dwEctypeID);
//                if (EctypeListPanel != null)
//                {
//                    EctypeListPanel.UnlockMyPanel(child);
//                }
//            }
            ShowCurrentEctypeInfo(0);
            var yaoNvEctypeList = JudgeToCreateSirenEctypeCard();
            MoveToLastPanel(yaoNvEctypeList.ToArray());
            //本地判断是否开启封妖副本
            /*
            sMSGEctypeSelect_SC.sYaoqiProp.ApplyAllItem(p =>
                {
                    EctypeSelectConfigData ectypeSition;
                    EctypeConfigManager.Instance.EctypeSelectConfigList.TryGetValue((int)p.dwEctypeSection, out ectypeSition);
                    if (ectypeSition != null)
                    {
                        if (p.dwYaoqiValue >= ectypeSition._lEctypeYaoqiMax)
                        {
                            //创建妖女副本
                            TraceUtil.Log("[创建妖女副本]" + p.dwEctypeSection);
                            CreateSirenEctype((int)p.dwEctypeSection);
                        }
                    }
                });
             */            
            
            
        }

        //判断是否创建封妖副本 <返回 解锁的妖女副本区域id列表>
        private List<int> JudgeToCreateSirenEctypeCard()
        {
            List<int> ectypeSectionList = new List<int>();
            var targetUID = PlayerManager.Instance.FindHeroDataModel().UID;
			bool isPlaySirenAppearSound = false;
//            sMSGEctypeSelect_SC.sYaoqiProp.ApplyAllItem(p =>
//            {
//                ColdWorkInfo myColdWork = ColdWorkManager.Instance.GetColdWorkInfoClone(targetUID, ColdWorkClass.SirenEctype, p.dwEctypeSection);
//                if (myColdWork != null)
//                {
//                    CreateSirenEctype((int)p.dwEctypeSection, (int)myColdWork.ColdTime);
//                    //TraceUtil.Log("[妖女副本冷却信息]ColdTime= " + myColdWork.ColdTime + " , ColdTimeStart= " + myColdWork.ColdTimeStart + " , ColdTimeEnd= " + myColdWork.ColdTimeEnd);
//                    ectypeSectionList.Add((int)p.dwEctypeSection);
//					isPlaySirenAppearSound = true;
//                }
//            });
			if(isPlaySirenAppearSound)
			{
				SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SirenAppear");
			}
            return ectypeSectionList;
        }

        /// <summary>
        /// 跳转到最后解锁的界面并选中最后一个解锁副本
        /// </summary>
        void MoveToLastPanel(int[] yaoNvEctypeList)
        {
            //int maxEctypePanelID = 0;
//            foreach(var child in this.sMSGEctypeSelect_SC.sMSGEctypeData_SCs)
//            {
//                maxEctypePanelID = child.dwEctypeID>maxEctypePanelID?(int)child.dwEctypeID:maxEctypePanelID;
//            }
//            
//            //var LastEctypePanel = LocalEctypePanelList.First(P => P.ectypeSelectConfigData._lEctypeID == MaxEctypePanelID);
//            LocalEctypePanelList_v3 lastEctypePanel = LocalEctypePanelList.FirstOrDefault(P=>P.ectypeSelectConfigData._lEctypeID == maxEctypePanelID);
//            if (lastEctypePanel == null)
//            {
//                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到对应副本ID："+maxEctypePanelID);
//                return;
//            }
//            CurrentPageNumber = lastEctypePanel.PositionIndex;
//            foreach (var child in LocalEctypePanelList)
//            {
//                child.PositionIndex = child.PositionIndex - CurrentPageNumber;
//                child.SetMyPosition(child.PositionIndex);
//            }
//            var SelectPanelIDList = this.sMSGEctypeSelect_SC.sMSGEctypeData_SCs.Where(P=>P.dwEctypeID == maxEctypePanelID);
//            SelectPanelIDList.ApplyAllItem(P=>TraceUtil.Log("当前面板解锁ID："+P.dwEctypeID+","+P.byDiff));
//            int MaxSelectPanelDiff = 0;
//            foreach (var child in SelectPanelIDList)
//            {
//                MaxSelectPanelDiff = child.byDiff > MaxSelectPanelDiff ? child.byDiff: MaxSelectPanelDiff;
//            }
//
//            //add by lee 新增妖女副本判断
//            if (yaoNvEctypeList.Length > 0 && yaoNvEctypeList[yaoNvEctypeList.Length - 1] == maxEctypePanelID)
//            {                
//                if (MaxSelectPanelDiff == 1)
//                {
//                    int sirenEctypeID = lastEctypePanel.ectypeSelectConfigData._sirenEctypeContainerID;
//                    var vectContainer = lastEctypePanel.ectypeSelectConfigData.VectContainerList.SingleOrDefault(p => p.Value == sirenEctypeID);
//                    byte yaoNvEctypeDiff = (byte)vectContainer.Key;
//                    lastEctypePanel.SelectPanel(new SMSGEctypeData_SC()
//                    {
//                        dwEctypeID = (uint)maxEctypePanelID,
//                        byDiff = yaoNvEctypeDiff
//                    });
//                }
//                else
//                {
//                    lastEctypePanel.SelectPanel(SelectPanelIDList.First(P => P.byDiff == MaxSelectPanelDiff));
//                }                
//            }
//            else
//            {
//                lastEctypePanel.SelectPanel(SelectPanelIDList.First(P => P.byDiff == MaxSelectPanelDiff));                
//            }
//            
            OnPageTurning(null);
        }

        private void JudgeToSelectYaoNvPanel(List<int> yaoNvEctypeList)
        {
            /*
            int MaxEctypePanelID = 0;
            foreach(var child in this.sMSGEctypeSelect_SC.sMSGEctypeData_SCs)
            {
                MaxEctypePanelID = child.dwEctypeID>MaxEctypePanelID?(int)child.dwEctypeID:MaxEctypePanelID;
            }
            
            var LastEctypePanel = LocalEctypePanelList.First(P => P.ectypeSelectConfigData._lEctypeID == MaxEctypePanelID);            

            var SelectPanelIDList = this.sMSGEctypeSelect_SC.sMSGEctypeData_SCs.Where(P=>P.dwEctypeID == MaxEctypePanelID);
            int MaxSelectPanelDiff = 0;
            foreach (var child in SelectPanelIDList)
            {
                MaxSelectPanelDiff = child.byDiff > MaxSelectPanelDiff ? child.byDiff: MaxSelectPanelDiff;
            }

            //add by lee 新增妖女副本判断
            //TraceUtil.Log("[yaoNvEctypeList.Count]" + yaoNvEctypeList.Count + " , [yaoNvEctypeList.Last()]" + MaxEctypePanelID);
            if (yaoNvEctypeList.Count > 0 && yaoNvEctypeList.Last() == MaxEctypePanelID)
            {
                if (MaxSelectPanelDiff == 1)
                {
                    //TraceUtil.Log("[JudgeToSelectYaoNvPanel]");
                    int sirenEctypeID = LastEctypePanel.ectypeSelectConfigData._sirenEctypeContainerID;
                    var vectContainer = LastEctypePanel.ectypeSelectConfigData.VectContainerList.SingleOrDefault(p => p.Value == sirenEctypeID);
                    byte yaoNvEctypeDiff = (byte)vectContainer.Key;
                    LastEctypePanel.SelectPanel(new SMSGEctypeData_SC()
                    {
                        dwEctypeID = (uint)MaxEctypePanelID,
                        byDiff = yaoNvEctypeDiff
                    });
                    OnPageTurning(null);
                }
            }
             */
            
            if (yaoNvEctypeList.Count > 0)
            {
                if (OnSelectEctypeCard == null)
                    return;
//                int curContainerID = (int)OnSelectEctypeCard.sMSGEctypeData_SC.dwEctypeID;
//                var curEctypePanel = LocalEctypePanelList.SingleOrDefault(p=>p.ectypeSelectConfigData._lEctypeID==curContainerID);
//                TraceUtil.Log("[curEctypePanel]" + curEctypePanel.ectypeSelectConfigData._lEctypeID);
//                if (yaoNvEctypeList.Any(p => p == curContainerID))
//                {
//                    if (OnSelectEctypeCard.sMSGEctypeData_SC.byDiff == 1)
//                    {
//                        int sirenEctypeID = curEctypePanel.ectypeSelectConfigData._sirenEctypeContainerID;
//                        var vectContainer = curEctypePanel.ectypeSelectConfigData.VectContainerList.SingleOrDefault(p => p.Value == sirenEctypeID);
//                        byte yaoNvEctypeDiff = (byte)vectContainer.Key;
//                        curEctypePanel.SelectPanel(new SMSGEctypeData_SC()
//                        {
//                            dwEctypeID = (uint)curContainerID,
//                            byDiff = yaoNvEctypeDiff
//                        });
//                        OnPageTurning(null);
//                    }
//                }
            }
            
        }


        void EctypePageSkip(object obj)
        {
            int ectypeID = (int)obj;
            LocalEctypePanelList_v3 selectEctypePanel = LocalEctypePanelList.FirstOrDefault(P=>P.ectypeSelectConfigData._lEctypeID == ectypeID) ;
            if (selectEctypePanel == null)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到对应副本ID：" + ectypeID);
                return;
            }
            CurrentPageNumber = LocalEctypePanelList.IndexOf(selectEctypePanel);
            for (int i = 0; i < LocalEctypePanelList.Count;i++ )
            {
                int posIndex = i - CurrentPageNumber;
                LocalEctypePanelList[i].PositionIndex = posIndex;
                LocalEctypePanelList[i].SetMyPosition(posIndex);
            }

            OnPageTurning(null);
        }

        void ResetPageTips()
        {
            Vector3 listPosition = Grid.parent.localPosition;
            listPosition.x = 0;
            Grid.parent.localPosition = listPosition;
            Vector4 panelClip = Grid.parent.GetComponent<UIPanel>().clipRange;
            panelClip.x = 0;
            Grid.parent.GetComponent<UIPanel>().clipRange = panelClip;
        }

        public void OnPageTurning(object obj)
        {
            float distance = Grid.parent.localPosition.x;
            int PageID=0;
            if (distance < -100)
            {
                PageID = (int)((Mathf.Abs(distance) / PageDistance) + 0.5f) + CurrentPageNumber;
            }
            else
            {
                PageID = CurrentPageNumber - (int)((Mathf.Abs(distance) / PageDistance) + 0.5f);
            }
            if (PageID < 0 || PageID >= LocalEctypePanelList.Count||CurrentPageID == PageID)
                return;
            this.PageNumberTips.SetActivePageID(PageID + 1);
            ShowCurrentEctypeInfo(PageID);
            TraceUtil.Log("CurrentPageID：" + PageID+",CurrentPageNumber:"+CurrentPageNumber);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
            CurrentPageID = PageID;
        }
        private void ShowCurrentEctypeInfo(int EctypeIndex)
        {
            this.CurrentEctypeIndex = EctypeIndex;
            //string EctypeName = EctypeConfigManager.Instance.EctypeSelectConfigFile._dataTable[EctypeIndex]._szName;
            string EctypeName = LocalEctypePanelList[EctypeIndex].ectypeSelectConfigData._szName;
            this.NameLabel.SetText(LanguageTextManager.GetString(EctypeName));
            //add by lee )显示妖气值
            ShowYaoqiProp();
            //判断能不能点击增加妖气值按钮
            if (LocalEctypePanelList[EctypeIndex].IsFirstEctypeUnlock())
            {
                Button_Add.SetButtonColliderActive(true);
                Button_Add.SetImageColor(Color.white);
            }
            else
            {
                Button_Add.SetButtonColliderActive(false);
                Button_Add.SetImageColor(Color.grey);
            }
        }

        /// <summary>
        /// 显示妖气值
        /// </summary>
        private void ShowYaoqiProp()
        {
            //int maxYaoqiValue = EctypeConfigManager.Instance.EctypeSelectConfigFile._dataTable[CurrentEctypeIndex]._lEctypeYaoqiMax;
            int maxYaoqiValue = LocalEctypePanelList[CurrentEctypeIndex].ectypeSelectConfigData._lEctypeYaoqiMax;
//            if (sMSGEctypeSelect_SC.sYaoqiProp == null)
//                return;
//            if (this.TweenFloatControl != null)
//            {
//                Destroy(this.TweenFloatControl);
//                this.TweenFloatControl = null;
//            }
//            uint curYaoqiValue = this.sMSGEctypeSelect_SC.sYaoqiProp[CurrentEctypeIndex].dwYaoqiValue;            
//            if (maxYaoqiValue != 0)
//            {
//                Slider_Yaoqi.sliderValue = curYaoqiValue * 1f / maxYaoqiValue;
//            }
//            Label_YaoqiValue.text = curYaoqiValue.ToString() + "/" + maxYaoqiValue.ToString();

        }
        

        /// <summary>
        /// 出现妖女副本
        /// </summary>
        //private void AppearSirenEctype()
        //{
        //    int sirenEctypeID = EctypeConfigManager.Instance.EctypeSelectConfigFile._dataTable[CurrentEctypeIndex]._sirenEctypeContainerID;
        //    LocalEctypePanelList[CurrentEctypeIndex].AppearSirenEctype(sirenEctypeID);
        //}

        /// <summary>
        /// 创建妖女副本
        /// </summary>
        /// <param name="sitionID">区域id</param>
        private void CreateSirenEctype(int sitionID, int time)
        {
            var localEctypePanel = LocalEctypePanelList.SingleOrDefault(p => p.ectypeSelectConfigData._lEctypeID == sitionID);
            if (localEctypePanel != null)
            {
                int sirenID = localEctypePanel.ectypeSelectConfigData._sirenEctypeContainerID;                
                localEctypePanel.AppearSirenEctype(sitionID, sirenID, time);
            }
        }

        public void OnSelectEctype(LocalEctypePanel_v3 localEctypePanel_v3)
        {
            OnSelectEctypeCard = localEctypePanel_v3;

//            StroyLineDataManager.Instance.CurSelectEctype(OnSelectEctypeCard.sMSGEctypeData_SC);
//
//            ShowBtnCostInfo();
//            foreach (var child in LocalEctypePanelList)
//            {
//                child.OnSelectEctypeCard(OnSelectEctypeCard.sMSGEctypeData_SC);
//            }
        }
        /// <summary>
        /// 按钮位置显示消耗类型及数量
        /// </summary>
        void ShowBtnCostInfo()
        { 
            CostLabel.SetButtonBackground(OnSelectEctypeCard.ectypeContainerData.lCostType);
            CostLabel.SetButtonText(OnSelectEctypeCard.ectypeContainerData.lCostEnergy);
        }

        //public void HelpButtonHandle(object obj)
        //{
        //    if (m_helpPanel == null)
        //    {
        //        m_helpPanel = CreatObjectToNGUI.InstantiateObj(HelpPanel,transform);
        //    }

        //    List<EctypeHelperConfigData> helpList = NewbieGuideManager_V2.Instance.GuideHelpList;
        //    m_helpPanel.GetComponent<GuideHelpPanel>().InitEctypeGuidePanel(helpList);
        //}

        /// <summary>
        /// 当快速加入的时候检查背包
        /// </summary>
        public void CheckQuickJionBackpack(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            ushort maxNum = ContainerInfomanager.Instance.GetContainerClientContsext(2).wMaxSize;
            var backpack = ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(2).Where(p => p.uidGoods != 0).ToList();
            if (maxNum - backpack.Count < 2)
            {
                MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_201"), LanguageTextManager.GetString("IDS_H2_55"), LanguageTextManager.GetString("IDS_H2_28"), OnGoBtnClick, null);
            }
            else
            {
                OnGoBtnClick();
            }
        }

        /// <summary>
        /// 更新妖气值
        /// </summary>
        /// <param name="yaoqiProp"></param>
        public void UpdateYaoqiProp(SMSGEctypeYaoqiProp_SC yaoqiProp)
        {
//            if (this.sMSGEctypeSelect_SC.sYaoqiProp == null)
//            {
//                return;
//            }
//            int propLength = this.sMSGEctypeSelect_SC.sYaoqiProp.Length;
//            uint lastProp = 0;
//            for (int i = 0; i < propLength; i++)
//            {
//                if (this.sMSGEctypeSelect_SC.sYaoqiProp[i].dwEctypeSection == yaoqiProp.dwEctypeSection)
//                {
//                    lastProp = this.sMSGEctypeSelect_SC.sYaoqiProp[i].dwYaoqiValue;
//                    this.sMSGEctypeSelect_SC.sYaoqiProp[i] = yaoqiProp;                    
//                    break;
//                }
//            }
            if (this.TweenFloatControl != null)
            {
                Destroy(this.TweenFloatControl);
                this.TweenFloatControl = null;
            }
//            this.TweenFloatControl = TweenFloat.Begin(0.5f, lastProp, yaoqiProp.dwYaoqiValue, ChangeYaoqiPropLabel, UpdateFinish);

            //本地判断是否开启封妖副本
            /*
            EctypeSelectConfigData ectypeSition;
            EctypeConfigManager.Instance.EctypeSelectConfigList.TryGetValue((int)yaoqiProp.dwEctypeSection, out ectypeSition);
            if (ectypeSition != null)
            {
                if (yaoqiProp.dwYaoqiValue >= ectypeSition._lEctypeYaoqiMax)
                {
                    //创建妖女副本
                    TraceUtil.Log("[创建妖女副本]" + yaoqiProp.dwEctypeSection);
                    CreateSirenEctype((int)yaoqiProp.dwEctypeSection);
                }
            }
             */                       
        }
        void ChangeYaoqiPropLabel(float value)
        {
            int maxYaoqiValue = LocalEctypePanelList[CurrentEctypeIndex].ectypeSelectConfigData._lEctypeYaoqiMax;            
            Slider_Yaoqi.sliderValue = value * 1f / maxYaoqiValue;
            Label_YaoqiValue.text = ((int)value).ToString() + "/" + maxYaoqiValue.ToString();
        }
        void UpdateFinish(object obj)
        {
            var yaoNvEctypeList = JudgeToCreateSirenEctypeCard();
            JudgeToSelectYaoNvPanel(yaoNvEctypeList);
        }


        //显示妖气说明
        void ShowYaoqiExplanation(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            YaoqiExplanation.SetActive(true);
        }
        //关闭妖气说明
        void CloseYaoqiExplanation(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            YaoqiExplanation.SetActive(false);
        }

        //请求加妖气
        void AddYaoqiValue(object obj)
        {
            //是否妖气值已满
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            int maxYaoqiValue = LocalEctypePanelList[CurrentEctypeIndex].ectypeSelectConfigData._lEctypeYaoqiMax;            
            //uint curYaoqiValue = this.sMSGEctypeSelect_SC.sYaoqiProp[CurrentEctypeIndex].dwYaoqiValue;
//            if (curYaoqiValue >= maxYaoqiValue)
//            {
//                return;
//            }

            int jingqiID = CommonDefineManager.Instance.CommonDefine.ShortcutItem_Siren;
            int yaoqiItem = ContainerInfomanager.Instance.GetItemNumber(jingqiID);
            if (yaoqiItem > 0)
            {
                //int sectionID = EctypeConfigManager.Instance.EctypeSelectConfigFile._dataTable[CurrentEctypeIndex]._lEctypeID;
                int sectionID = LocalEctypePanelList[CurrentEctypeIndex].ectypeSelectConfigData._lEctypeID;
                TraceUtil.Log("[CurrentEctypeIndex]" + CurrentEctypeIndex);
                NetServiceManager.Instance.EctypeService.SendAddYaoqi(sectionID);
                TraceUtil.Log("[sectionID]" + sectionID);
                //播放动画
                GameObject cutYaoqiItem = (GameObject)Instantiate(Animation_CutYaoqiItem);
                cutYaoqiItem.transform.parent = Button_Add.transform.parent;
                cutYaoqiItem.transform.localScale = Vector3.one;
            }
            else
            {
                UIEventManager.Instance.TriggerUIEvent(UIEventType.QuickPurchase, jingqiID);
            }
        }

        void OnGoBtnClick()
        {
            TraceUtil.Log("GoBtnClick");
            //ShowCostMessageBox(CheckJoin);
            CheckGotoBattle(OnJoin);
        }
        void CheckJoin(object obj)
        {
            TraceUtil.Log("CheckJion");
            CheckGotoBattle(OnJoin);
        }

        void OnJoin(object obj)
        {
            TraceUtil.Log("Onjoin");

            GoToBattle();
        }
        void OnTeamWorkBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            //ShowCostMessageBox(CheckTeamWorkJoin);
            //CheckTeamWorkJoin(null);
            OnTeamWorkJoin(null);
        }
        void CheckTeamWorkJoin(object obj)
        {
            CheckGotoBattle(OnTeamWorkJoin);
        }
        void OnTeamWorkJoin(object obj)
        {
            var playerData = PlayerManager.Instance.FindHeroDataModel();
			int areaID = EctypeConfigManager.Instance.GetSelectContainerID((int)sMSGEctypeLevelData_SC.dwEctypeId);
			EctypeSelectConfigData escData = EctypeConfigManager.Instance.EctypeSelectConfigList[areaID];

			TeamManager.Instance.SetCurSelectEctypeContainerData(escData);

            NetServiceManager.Instance.TeamService.SendGetTeamListMsg(new SMSGGetTeamList_CS()
            {
                uidEntity = playerData.UID,
				dwEctypeID = (uint)escData._lEctypeID,
                byDifficulty = 0
            });


            //if (PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE <= 0)
            //{
            //    UIEventManager.Instance.TriggerUIEvent(UIEventType.NoEnoughActiveLife, null);
            //    return;
            //}
            MainUIController.Instance.OpenMainUI(UIType.TeamInfo);
        }

        void OnTeamSpeedBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            //CheckTeamSpeedJoin(null);
            OnTeamSpeedJoin(null);
        }

        void CheckTeamSpeedJoin(object obj)
        {
            CheckGotoBattle(OnTeamSpeedJoin);
        }

        void OnTeamSpeedJoin(object obj)
        {
            SMsgTeamFastJoin_CS sMsgTeamFastJoin_CS = new SMsgTeamFastJoin_CS()
            {
                dwActorId = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID,
//                dwEctypeId = OnSelectEctypeCard.sMSGEctypeData_SC.dwEctypeID,
//                byEctypeDiff = OnSelectEctypeCard.sMSGEctypeData_SC.byDiff,
            };

            NetServiceManager.Instance.TeamService.SendTeamFastJoinMsg(sMsgTeamFastJoin_CS);
            TraceUtil.Log("快速加入");
            LoadingUI.Instance.Show();
        }

        /// <summary>
        /// 显示花费提示信息
        /// </summary>
        void ShowCostMessageBox(ButtonCallBack SureBtnCallback)
        {
            EctypeContainerData SelectContainerData = OnSelectEctypeCard.ectypeContainerData;
            int Cost = int.Parse(SelectContainerData.lCostEnergy);
            if (SelectContainerData.lCostType != 1)
            {
                ectypePanelMessageBox.ShowPanel(LanguageTextManager.GetString("IDS_H1_319"), SelectContainerData.lCostType, Cost, SureBtnCallback);
            }
            else
            {
                SureBtnCallback(null);
            }
        }

        /// <summary>
        /// 检测是否足够资源加入副本
        /// </summary>
        bool CheckGotoBattle(ButtonCallBack SureBtnCallback)
        {
            bool Flag = true;
            EctypeContainerData SelectContainerData = OnSelectEctypeCard.ectypeContainerData;

//            if (OnSelectEctypeCard.sMSGEctypeData_SC.byMaxDayTimes != 0 && OnSelectEctypeCard.sMSGEctypeData_SC.byCurDayTimes == OnSelectEctypeCard.sMSGEctypeData_SC.byMaxDayTimes)
//            {
//                //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_321"), LanguageTextManager.GetString("IDS_H2_55"), null);
//                MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_H1_321"), 1);
//                //LoadingUI.Instance.Close();
//                return false;
//            }

            switch (SelectContainerData.lCostType)
            {
                case 1:
                    int Cost = int.Parse(SelectContainerData.lCostEnergy);
                    int PayMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
                    if (PayMoney >= Cost)
                    {
                        if (SureBtnCallback != null) { SureBtnCallback(null); }
                    }
                    else
                    {
                        UIEventManager.Instance.TriggerUIEvent(UIEventType.NoEnoughActiveLife, null);
                    }
                    break;
                case 2:
                    Cost = int.Parse(SelectContainerData.lCostEnergy);
                    PayMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
                    if (PayMoney < Cost)
                    {
                        //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H2_44"), LanguageTextManager.GetString("IDS_H2_55"), null);
                        MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"), 1);
                        Flag = false;
                    }
                    else
                    {
                        if (SureBtnCallback != null) { SureBtnCallback(null); }
                    }
                    break;
                case 3:
                    int CostGold = int.Parse(SelectContainerData.lCostEnergy);
                    int BINDPAY = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
                    if (BINDPAY < CostGold)
                    {
                        //MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_231"), LanguageTextManager.GetString("IDS_H2_55"), null);
                        MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_231"), 1);
                        Flag = false;
                    }
                    else
                    {
                        if (SureBtnCallback != null) { SureBtnCallback(null); }
                    }
                    break;
                default:
                    break;
            }
            return Flag;
        }

        private void GoToBattle()
        {
            ShowCostLabelInButton();
        }
        /// <summary>
        /// 在按钮上方显示花费tips
        /// </summary>
        void ShowCostLabelInButton()
        {
            if (IsShowCostLabel)
                return;
            EctypeContainerData SelectContainerData = OnSelectEctypeCard.ectypeContainerData;
            bool isShowCostType = true;
            int localCostNumber =int.Parse(OnSelectEctypeCard.ectypeContainerData.lCostEnergy);
            int costNumber = 0;
            switch (SelectContainerData.lCostType)
            {
                case 1:
                    costNumber = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
                    isShowCostType = costNumber > 0 && localCostNumber>0;
                    break;
                case 2:
                    costNumber = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
                    isShowCostType = costNumber > 0 && localCostNumber > 0;
                    break;
                case 3:
                    costNumber = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
                    isShowCostType = costNumber > 0 && localCostNumber > 0;
                    break;
                default:
                    break;
            }
            if (isShowCostType)
            {
                SingleButtonCallBack Tips = CreatObjectToNGUI.InstantiateObj(CostLabelPreafab, BtnGo.transform).GetComponent<SingleButtonCallBack>();
                Vector3 fromPoint = new Vector3(0, 50, -30);
                Vector3 toPoint = new Vector3(0, 0, -30);
                TweenPosition.Begin(Tips.gameObject, 0.5f, fromPoint, toPoint, null);
                TweenAlpha.Begin(Tips.gameObject, 0.5f, 1, 0, null);
                Tips.SetButtonBackground(OnSelectEctypeCard.ectypeContainerData.lCostType);
                Tips.SetButtonText(string.Format("-{0}", costNumber>localCostNumber?localCostNumber:costNumber));
            }
            //else
            //{
            //    SendGoBattleToServer(null);
            //}
            DoForTime.DoFunForTime(1.5f, SendGoBattleToServer, null);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Fight");
            StartCoroutine(SetShowCostLabelFalseForTime(1.5f));
			LoadingUI.Instance.Show();
        }

        IEnumerator SetShowCostLabelFalseForTime(float waitTime)
        {
            IsShowCostLabel = true;
            yield return new WaitForSeconds(waitTime);
            IsShowCostLabel = false; 
        }

        void SendGoBattleToServer(object obj)
        {
            Destroy(obj as GameObject);
            SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS()
			{
				//TODO:进入技能协议有更改，去掉副本iD和难度,需要从新修改
//                uidEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity,
//                dwEctypeId = (int)OnSelectEctypeCard.sMSGEctypeData_SC.dwEctypeID,
//                byDifficulty = OnSelectEctypeCard.sMSGEctypeData_SC.byDiff,
            };
			Debug.Log ("StroyLineDataManager.Instance.GetStroyType===="+StroyLineDataManager.Instance.GetStroyType);
            if (StroyLineDataManager.Instance.GetStroyType == StroyLineType.EctypeStart)
            {
                RaiseEvent(EventTypeEnum.GotoStroyLine.ToString(), null);
                return;
            }

            if (EctGuideManager.Instance.IsEctypeGuide)
            {
                NetServiceManager.Instance.EctypeService.SendEctypeGuideCreate(sMSGEctypeRequestCreate_CS);
            }
            else
            {
                NetServiceManager.Instance.EctypeService.SendEctypeRequest(sMSGEctypeRequestCreate_CS);
            }
//            TraceUtil.Log("进入副本:" + sMSGEctypeRequestCreate_CS.byDifficulty);
            LoadingUI.Instance.Show();
            #region Add by lee
//            TeamManager.Instance.SetEctypeData(new SMSGEctypeData_SC
//            {
//                dwEctypeID = OnSelectEctypeCard.sMSGEctypeData_SC.dwEctypeID,
//                byDiff = OnSelectEctypeCard.sMSGEctypeData_SC.byDiff,
//            });
            #endregion
        }

        //private void GoToBattleHandle(INotifyArgs notify)
        //{
        //    NewbieGuideManager.Instance.IsGuideFinish = true;
        //    GoToBattle();
        //}


        //public void OnClosePanel(object obj)
        //{
			
        //    //transform.localPosition = new Vector3(0, 0, -1000);
        //    Destroy(gameObject);
            
        //}


        //没有队伍列表
        void TeamNoFoundListHandle(INotifyArgs e)
        {
            LoadingUI.Instance.Close();
            //print("ShowCreatTeamMessage");
            MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_160"),
            LanguageTextManager.GetString("IDS_H2_4"), LanguageTextManager.GetString("IDS_H2_28"), SureCreateTeam, null);
        }
        private void SureCreateTeam()
        {
            OnTeamWorkBtnClick(null);
            NetServiceManager.Instance.TeamService.SendTeamCreateMsg();
        }
        //妖女副本冷却消息处理
        void SirenColdWorkHandle(object obj)
        {
            SMsgActionColdWork_SC sMsgActionColdWork_SC = (SMsgActionColdWork_SC)obj;
            var localEctypePanel = LocalEctypePanelList.SingleOrDefault(p => p.ectypeSelectConfigData._lEctypeID == sMsgActionColdWork_SC.dwColdID);
            if (localEctypePanel != null)
            {
                localEctypePanel.DeleteSirenEctype();
            }
            //var yaoNvEctypeList = JudgeToCreateSirenEctypeCard();
            //MoveToLastPanel(yaoNvEctypeList.ToArray());
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.TeamNoFoundList.ToString(), TeamNoFoundListHandle);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.RemoveColdWork, SirenColdWorkHandle);
            //this.AddEventHandler(EventTypeEnum.GuideGoToBattle.ToString(), GoToBattleHandle);
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.NewColdWorkFromSever, SirenColdWorkHandle);
        }
    }
}