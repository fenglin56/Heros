using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace UI.MainUI
{
    /// <summary>
    /// 背包容器控制
    /// </summary>
    public class ContainerPackList_V2 : DragComponentSlot
    {
        public UILabel UnlockBoxNumberLabel;
        public UILabel PageNumberLabel;
        public UILabel UnlockPackNumberLabel;
        //public UISprite LeftMark;
        //public UISprite RightMark;
        public SingleButtonCallBack NextPageBtn;
        public SingleButtonCallBack LastPageBtn;
        public ResetContainerButton ResetContainerPackBtn;
        public HeroEquiptItemList_V2 HeroEquiptItemListManager;

        public ContainerBoxSlot_V2[] ContainerBoxList;//背包格列表,手动关联

        public GameObject UnlockContainerMessageBox;

        public PackInfo_V3 MyParent { get; private set; }

        private UnlockContainerBoxTips unlockContainerBoxTips;

        private int CurrentPageNumber = 1;

        private int[] m_guideBtnID;

        bool IsRegGuideTurnPage = false;//是否为新手引导翻页，如果是，则不自动刷新页面

        public void Init(PackInfo_V3 myParent)
        {
            this.MyParent = myParent;
        }

        public override void Start()
        {
            base.Start();
            m_guideBtnID = new int[3];
            LastPageBtn.SetCallBackFuntion(OnLastBtnClick);
            NextPageBtn.SetCallBackFuntion(OnNextBtnClick);
            ResetContainerPackBtn.SetCallBackFuntion(OnResetContainerBtnClick);

            UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerPack, ResetContainerPackInfo);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods, ResetContainerPackInfo);
            //TODO GuideBtnManager.Instance.RegGuideButton(LastPageBtn.gameObject, UIType.Package, SubType.PackagePage, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(NextPageBtn.gameObject, UIType.Package, SubType.PackagePage, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(ResetContainerPackBtn.gameObject, UIType.Package, SubType.packageReset, out m_guideBtnID[2]);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerPack, ResetContainerPackInfo);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, ResetContainerPackInfo);

            for (int i = 0; i < m_guideBtnID.Length; i++ )
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        public void Show()
        {
            CurrentPageNumber = 1;
            SetBtnActive();
            if (!IsRegGuideTurnPage)
            {
                ResetContainerPackInfo(null);
            }
            else
            {
                IsRegGuideTurnPage = false; 
            }
            //OnLastBtnClick(null);
        }

        public void UpdateSlotSelectStatus(SingleContainerBox selectContainerBox)
        {
            ContainerBoxList.ApplyAllItem(P => P.SetSelectStatusActive(P.MyContainerBox!=null&&selectContainerBox == P.MyContainerBox));
        }

        void OnLastBtnClick(object obj)
        {
            if (CurrentPageNumber <= 1)
                return;
            UIEventManager.Instance.TriggerUIEvent(UIEventType.ResetDragComponentStatus, null);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
            CurrentPageNumber--;
            CurrentPageNumber = CurrentPageNumber < 1 ? 1 : CurrentPageNumber;
            SetBtnActive();
            ResetContainerPackInfo(null);
            ContainerBoxList.ApplyAllItem(P => P.OnTouchSlot());
        }

        void OnNextBtnClick(object obj)
        {
            if (CurrentPageNumber >= 16)
                return;
            UIEventManager.Instance.TriggerUIEvent(UIEventType.ResetDragComponentStatus, null);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
            CurrentPageNumber++;
            CurrentPageNumber = CurrentPageNumber > 16 ? 16 : CurrentPageNumber;
            SetBtnActive();
            ResetContainerPackInfo(null);
            ContainerBoxList.ApplyAllItem(P => P.OnTouchSlot());
        }

        void SetBtnActive()
        {
            bool canPageNext = CurrentPageNumber < 16;
            bool canPageLast = CurrentPageNumber > 1;
            Color disabelColor = new Color(255, 255, 255, 0.5f);
            Color enabelColor = new Color(255, 255, 255, 1);
            NextPageBtn.BackgroundSprite.color = canPageNext ? enabelColor : disabelColor;
            LastPageBtn.BackgroundSprite.color = canPageLast ? enabelColor : disabelColor;
            //RightMark.enabled = canPageNext;
            //LeftMark.enabled = canPageLast;
        }


        //刷新背包格显示的物品信息
        void ResetContainerPackInfo(object obj)
        {
            List<SSyncContainerGoods_SC> sSyncContainerGoods_SCs=(List<SSyncContainerGoods_SC>)obj;
            UpdateSlotSelectStatus(null);
            MyParent.CloseContainerTips();
            int CurrentBox = ContainerInfomanager.Instance.sBuildContainerClientContexts.First(P=>P.dwContainerName == 2).wMaxSize;
            UnlockPackNumberLabel.SetText(string.Format("{0}/{1}",ContainerInfomanager.Instance.GetExistPackBoxNumber(),CurrentBox));
            PageNumberLabel.SetText(string.Format("{0}/16",CurrentPageNumber));
            var currentPageDataList = GetCurrentPageContainerBoxSlotData();
            for (int i = 0; i < sSyncContainerGoods_SCs.Count; i++)
            {
              var place=  sSyncContainerGoods_SCs[i].nPlace;
                ContainerBoxList[place].Init(currentPageDataList[place],this);
            }
        }

        //void OnGUI()
        //{
        //    if (GUILayout.Button("JumpToGoods"))
        //    {
        //        var containerPackInco = ContainerInfomanager.Instance.sBuildContainerClientContexts.First(P => P.dwContainerName == 2);
        //        var getData = ContainerInfomanager.Instance.itemFielArrayInfo.FindAll(
        //            P => (P.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID == containerPackInco.dwContainerID));
        //        var datab = getData[Random.Range(0,getData.Count -1)];
        //        TraceUtil.Log(string.Format("TurnTo{0},Place:{1}",LanguageTextManager.GetString(datab.LocalItemData._szGoodsName)+datab.sSyncContainerGoods_SC.nPlace,TurningToPage(datab.LocalItemData._goodID)));
        //    }
        //}

        /// <summary>
        /// 获取对应物品按钮的id
        /// </summary>
        /// <param name="goodsID"></param>
        /// <returns></returns>
        public int GetTargetItemGuideBtnID(int goodsID)
        {
            return ContainerBoxList[TurningToPage(goodsID)].MyContainerBox.m_guideBtnID[1];
        }

        /// <summary>
        /// 翻页到对应物品页面并返回物品位置
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns>返回参数为0到4</returns>
        public int TurningToPage(int goodsId)
        {
            IsRegGuideTurnPage = true;
            var containerPackInco = ContainerInfomanager.Instance.sBuildContainerClientContexts.First(P => P.dwContainerName == 2);
            ItemFielInfo getData = ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(
                P => (P.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID == containerPackInco.dwContainerID)
                    && (P.LocalItemData._goodID == goodsId));
            if (getData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"查找的物品不在背包栏:"+goodsId);return -1; }
            int goodsPlace = getData.sSyncContainerGoods_SC.nPlace ;
            CurrentPageNumber = (goodsPlace) / 5+1;
            int currentPagePlace = (goodsPlace) % 5;

            SetBtnActive();
            ResetContainerPackInfo(null);
            ContainerBoxList.ApplyAllItem(P => P.OnTouchSlot());

            return currentPagePlace;
        }

        /// <summary>
        /// 获取当前页面的列表信息
        /// </summary>
        /// <returns></returns>
        List<ContainerBoxSlotData> GetCurrentPageContainerBoxSlotData()
        {
            List<ContainerBoxSlotData> getData = new List<ContainerBoxSlotData>();
            int currentMinBoxPlace = (CurrentPageNumber-1) * 5+1;
            int currentMaxBoxPlace = CurrentPageNumber * 5;
            int unlockBoxNumber = ContainerInfomanager.Instance.GetContainerClientContsext(2).wMaxSize;//当前最大背包容量
            var containerPackInco = ContainerInfomanager.Instance.sBuildContainerClientContexts.First(P => P.dwContainerName == 2);
            List<ItemFielInfo> CurrentPageContainerInfoList = ContainerInfomanager.Instance.itemFielArrayInfo.FindAll(//当前页面的物品列表
                P => (P.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID == containerPackInco.dwContainerID)
                    && (P.sSyncContainerGoods_SC.nPlace + 1) >= currentMinBoxPlace 
                    &&(P.sSyncContainerGoods_SC.nPlace + 1)<=currentMaxBoxPlace);
            for (int i = currentMinBoxPlace; i <= currentMaxBoxPlace; i++)
            {
                if (i > unlockBoxNumber)
                {
                    getData.Add(new ContainerBoxSlotData() {CurrentPlace = i, IsLock = true, itemfileInfo = null });
                }
                else
                {
                    getData.Add(new ContainerBoxSlotData() { CurrentPlace = i, IsLock = false, itemfileInfo = CurrentPageContainerInfoList.FirstOrDefault(P => (P.sSyncContainerGoods_SC.nPlace + 1) == i) });
                }
            }
            return getData;
        }

        /// <summary>
        /// 拖拽物体到我这里的某个槽上
        /// </summary>
        public void OnDragComponentToSlot(ContainerBoxSlot_V2 targetContainerBoxSlot,SingleContainerBox DragItem)
        {
            switch (DragItem.singleContainerBoxType)
            {
                case SingleContainerBoxType.HeroEquip:
                    HeroEquiptItemListManager.RemoveItem(DragItem);
                    var slotData  = targetContainerBoxSlot.MyContainerBoxSlotData;
                    slotData.itemfileInfo = DragItem.itemFielInfo;
                    targetContainerBoxSlot.Init(slotData, this);
                    SendUnloadEquipItemsToSever(DragItem.itemFielInfo, targetContainerBoxSlot);
                    break;
                case SingleContainerBoxType.Container:
                    var fromSlot = ContainerBoxList.First(P => P.MyContainerBox == DragItem as SingleContainerBox);
                    MoveItemToNewSlot(fromSlot,targetContainerBoxSlot);
                    break;
            }
        }

        /// <summary>
        /// 卸下装备
        /// </summary>
        /// <param name="itemFielInfo"></param>
        public void SendUnloadEquipItemsToSever(ItemFielInfo itemFielInfo, ContainerBoxSlot_V2 targetContainerBoxSlot)
        {
            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = itemFielInfo.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
            dataStruct.byPlace = (byte)itemFielInfo.sSyncContainerGoods_SC.nPlace;
            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
            dataStruct.desPlace = (targetContainerBoxSlot.MyContainerBoxSlotData.CurrentPlace - 1);
            dataStruct.byUseType = itemFielInfo.LocalItemData._GoodsClass == 2 ? (byte)1 : (byte)0;
            TraceUtil.Log(string.Format("发送卸下装备到背包栏：目标位置：{0}", dataStruct.desPlace));
            NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
        }

        public void SendUnloadEquipItemsToSever(ItemFielInfo itemFielInfo)
        {
            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = itemFielInfo.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
            dataStruct.byPlace = (byte)itemFielInfo.sSyncContainerGoods_SC.nPlace;
            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
            dataStruct.desPlace = -1;
            dataStruct.byUseType = itemFielInfo.LocalItemData._GoodsClass == 2 ? (byte)1 : (byte)0;
            TraceUtil.Log(string.Format("发送卸下装备到背包栏：目标位置：{0}", dataStruct.desPlace));
            NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
        }

        /// <summary>
        /// 将物品从本页的一个槽移动到另一个槽上
        /// </summary>
        void MoveItemToNewSlot(ContainerBoxSlot_V2 fromSlot,ContainerBoxSlot_V2 toSlot)
        {
            SendChangeItemPlaceToServer(fromSlot,toSlot);
            var toSlotData = toSlot.MyContainerBoxSlotData;
            var fromSlotData = fromSlot.MyContainerBoxSlotData;
            var chachePlace = toSlotData.CurrentPlace;
            toSlotData.CurrentPlace = fromSlotData.CurrentPlace;
            fromSlotData.CurrentPlace = chachePlace;
            toSlot.Init(fromSlotData,this);
            fromSlot.Init(toSlotData,this);
        }

        void SendChangeItemPlaceToServer(ContainerBoxSlot_V2 fromSlot, ContainerBoxSlot_V2 toSlot)
        {
             var dwContainerID =ContainerInfomanager.Instance.sBuildContainerClientContexts.First(P=>P.dwContainerName ==2).dwContainerID;
             SSyncContainerGoods_CS sSyncContainerGoods_CS = new SSyncContainerGoods_CS()
             {
                 ContainerID = dwContainerID,
                 dwSrcContainerID = dwContainerID,
                 bySrcPlace = (byte)(fromSlot.MyContainerBoxSlotData.CurrentPlace - 1),
                 dwDstContainerID = dwContainerID,
                 byDstPlace = (byte)(toSlot.MyContainerBoxSlotData.CurrentPlace - 1),
             };
             //TraceUtil.Log(string.Format("发送物品栏位置改变消息到服务器：{0},{1}->{2}", sSyncContainerGoods_CS.dwSrcContainerID, sSyncContainerGoods_CS.bySrcPlace, sSyncContainerGoods_CS.byDstPlace));
             NetServiceManager.Instance.ContainerService.SendSSyncContainerGoods(sSyncContainerGoods_CS); 
        }

        /// <summary>
        /// 移除装备栏里的物品
        /// </summary>
        public void RemoveItemFromSlot(DragComponent dragComponent)
        {
            var removeItemSlot = ContainerBoxList.FirstOrDefault(P => P.MyContainerBox == dragComponent as SingleContainerBox);
            if (removeItemSlot != null)
            {
                removeItemSlot.ClearUpItem();
                //removeItemSlot.OnTouchSlot();
            }
        }

        //public void ReplaceItem(DragComponent oldItem,DragComponent newItem)
        //{
        //    var removeItemSlot = ContainerBoxList.First(P => P.MyContainerBox == dragComponent as SingleContainerBox);
        //    removeItemSlot.ClearUpItem();
        //    removeItemSlot.OnTouchSlot();
        //}

        /// <summary>
        /// 重置背包按钮
        /// </summary>
        /// <param name="obj"></param>
        public void OnResetContainerBtnClick(object obj)
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.ResetDragComponentStatus,null);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            var ContainerListInfo = ContainerInfomanager.Instance.sBuildContainerClientContexts.First(P=>P.dwContainerName == 2);
            SMsgContainerTidy_CS dataStruct = new SMsgContainerTidy_CS();
            dataStruct.dwContainerID1 = ContainerListInfo.dwContainerID;
            dataStruct.dwContainerID2 = ContainerListInfo.dwContainerID;
            NetServiceManager.Instance.ContainerService.SendContainerTidy(dataStruct);
            CurrentPageNumber = 1;
            OnLastBtnClick(null);
            //print("整理背包！容器ID：" + ContainerListInfo.dwContainerID);
        }

        /// <summary>
        /// 点击解锁背包按钮
        /// </summary>
        public void OnUnlockContainerBtnClick()
        {
            string Msg = LanguageTextManager.GetString("IDS_H1_6");
            //MainUIController.Instance.PackPanel.GetComponent<PackInfo>().ShowUnlockContainerMessage(Msg, SendContainerChangeSize, null, LanguageTextManager.GetString("IDS_H2_55"), LanguageTextManager.GetString("IDS_H2_28"));
            if (unlockContainerBoxTips == null)
            {
                unlockContainerBoxTips = CreatObjectToNGUI.InstantiateObj(UnlockContainerMessageBox, transform.parent).GetComponent<UnlockContainerBoxTips>();
            }
            unlockContainerBoxTips.Show(Msg, SendContainerChangeSize, null, LanguageTextManager.GetString("IDS_H2_55"), LanguageTextManager.GetString("IDS_H2_28"));
        }

        public void SendContainerChangeSize(object obj)//向后台发送解锁背包消息
        {
            NetServiceManager.Instance.ContainerService.SendContainerChangeSize(ContainerInfomanager.Instance.GetContainerClientContsext(2).dwContainerID);
        }

        public override bool CheckIsPair(DragComponent dragChild)
        {
            bool flag = false;
            if ((dragChild as SingleContainerBox).singleContainerBoxType == SingleContainerBoxType.HeroEquip&&ContainerInfomanager.Instance.GetEmptyPackBoxNumber()>0)
            {
                flag = true;
            }
            if (flag)
            {
                var emptySlot = ContainerBoxList.FirstOrDefault(P => P.MyContainerBox == null&&!P.IsLock);
                if (emptySlot != null)
                {
                    dragChild.NewSlotPoint = emptySlot.transform;
                }
                else
                {
                    dragChild.NewSlotPoint = PageNumberLabel.transform;
                }
            } 
            //TraceUtil.Log("CheckCamMove:"+flag+","+dragChild.NewSlotPoint);
            return flag;
        }

        public override void MoveToHere(DragComponent enterComponent)
        {
            var emptySlot = ContainerBoxList.FirstOrDefault(P=>P.MyContainerBox==null&&!P.IsLock);
            if (emptySlot != null)
            {
                emptySlot.MoveToHere(enterComponent);
            }
            else
            {
                var dragComponent = enterComponent as SingleContainerBox;
                SendUnloadEquipItemsToSever(dragComponent.itemFielInfo);
            }
        }

    }

    public class ContainerBoxSlotData
    {
        public int CurrentPlace;
        public bool IsLock;
        public ItemFielInfo itemfileInfo;
    }
}