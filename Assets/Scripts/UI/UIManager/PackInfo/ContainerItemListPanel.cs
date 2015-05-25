using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


namespace UI.MainUI
{

    public class ContainerItemListPanel : BaseTweenShowPanel
    {
		public enum ShowItemBtnType{All,Equipt,Jewel,Other}
		public SingleButtonCallBack AllItemButton;//所有物品列表按钮
		public SingleButtonCallBack EquiptItemButton;//装备列表按钮
		public SingleButtonCallBack OtherItemButton;//道具列表按钮
		public SingleButtonCallBack MorItemSellButton;//批量出售按钮
		public ResetContainerButton m_ResetContainerButton;//整理背包按钮
		public SingleButtonCallBack JewelItemButton;//器魂列表按钮
		public GameObject SingleItemLineAreaPrefab;
		public GameObject UnLockContainerBoxPrefab;
		public Transform Grid;
		public UIPanel m_ItemListUIPanel;
		public List<SingleItemLineArea> MyItemLineAreaList{get;private set;}
		public List<ItemFielInfo> MyPackItemList{get;private set;}
		public ShowItemBtnType CurrentShowType{get;private set;}
		public PackInfoPanel MyParent{get;private set;}
		public Dictionary<EquiptSlotType,ItemFielInfo> BestItemList = new Dictionary<EquiptSlotType, ItemFielInfo>();

		private UnlockContainerBoxTips UnLockContaienrBoxObj;
        private bool m_shouldMove;
        private float m_noticeToDragAmount;
        private UIDraggablePanel m_dragPanelComp ;
		void Awake()
		{
			MyItemLineAreaList = new List<SingleItemLineArea>();
			AllItemButton.SetCallBackFuntion(OnShowItemTypeButtonClick,ShowItemBtnType.All);
			EquiptItemButton.SetCallBackFuntion(OnShowItemTypeButtonClick,ShowItemBtnType.Equipt);
			OtherItemButton.SetCallBackFuntion(OnShowItemTypeButtonClick,ShowItemBtnType.Other);
			JewelItemButton.SetCallBackFuntion (OnShowItemTypeButtonClick, ShowItemBtnType.Jewel);
			MorItemSellButton.SetCallBackFuntion(OnMorItemBtnClick);
			m_ResetContainerButton.SetCallBackFuntion(OnResetContainerBtnClick);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerPack, ReciveUnLockPackMsg);//收到后台下发解锁背包格消息
            m_dragPanelComp = Grid.parent.GetComponent<UIDraggablePanel>();

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            AllItemButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_TabAll);
            EquiptItemButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_TabEquip);
            OtherItemButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_TabItem);
            MorItemSellButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_BatchingSell);
            m_ResetContainerButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Tidy);
            JewelItemButton.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_TabGem);
        }
		
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerPack, ReciveUnLockPackMsg);
		}

		public void Show(PackInfoPanel myParent)
		{
           
			base.Close();
			base.TweenShow();
			MyParent = myParent;
            SetTitleBtnActive(true);
            m_ResetContainerButton.OnShow();
			DoForTime.DoFunForFrame(1,OnShowItemTypeButtonClick,ShowItemBtnType.All);
        }
		/// <summary>
		/// 刷新背包栏
		/// </summary>
		public void UpdateItemList(object obj)
		{
			ShowItem(CurrentShowType);
		}

		/// <summary>
		/// 物品显示类型按钮点击
		/// </summary>
		/// <param name="obj">Object.</param>
		public void OnShowItemTypeButtonClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageClassification");
			ShowItemBtnType btnType = (ShowItemBtnType) obj;
			AllItemButton.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(btnType == ShowItemBtnType.All?2:1));
			EquiptItemButton.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(btnType == ShowItemBtnType.Equipt?2:1));
			OtherItemButton.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(btnType == ShowItemBtnType.Other?2:1));
			JewelItemButton.spriteSwithList.ApplyAllItem (p => p.ChangeSprite (btnType == ShowItemBtnType.Jewel ? 2 : 1));
			ShowItem(btnType);
		}

        void ShowItem(ShowItemBtnType shotItemType)
        {
            CurrentShowType = shotItemType;
            GetBestItemList();
            //BestItemList.ApplyAllItem(P=>TraceUtil.Log(String.Format("BestItem:{0},{1}",P.Key,LanguageTextManager.GetString(P.Value.LocalItemData._szGoodsName))));
            MyItemLineAreaList.ApplyAllItem(P => Destroy(P.gameObject));
            MyItemLineAreaList.Clear();
            switch (shotItemType)
            {
                case ShowItemBtnType.All:
                    MyPackItemList = ContainerInfomanager.Instance.GetPackItemList();
                    break;
                case ShowItemBtnType.Equipt:
                    MyPackItemList = ContainerInfomanager.Instance.GetPackItemList().FindAll(P => P.LocalItemData._GoodsClass == 1 && P.LocalItemData._GoodsSubClass <= 6);
                    break;
                case ShowItemBtnType.Other:
                    MyPackItemList = ContainerInfomanager.Instance.GetPackItemList().FindAll(P => P.LocalItemData._GoodsClass == 2 || (P.LocalItemData._GoodsClass == 3 && P.LocalItemData._GoodsSubClass != 3));
                    break;
                case ShowItemBtnType.Jewel:
                    MyPackItemList = ContainerInfomanager.Instance.GetPackItemList().FindAll(P => (P.LocalItemData._GoodsClass == 3 && P.LocalItemData._GoodsSubClass == 3));
                    break;
            }
            MyPackItemList.Sort((left, right) =>
            {
                if (left.sSyncContainerGoods_SC.nPlace < right.sSyncContainerGoods_SC.nPlace)
                {
                    return -1;
                }
                else if (left.sSyncContainerGoods_SC.nPlace == right.sSyncContainerGoods_SC.nPlace)
                {
                    return 0;
                }
                else return 1;
            });

            int allPackNum = ContainerInfomanager.Instance.GetAllPackMaxNum();
            int itemLineAreaNum = allPackNum / 4 + (allPackNum % 4 > 0 ? 1 : 0);
            m_shouldMove = itemLineAreaNum>3;
            for (int i = 0; i <= itemLineAreaNum; i++)
            {
				// 等于最大的包裹数时，不需要解锁栏
				if(i >= 80 / 4)
				{
					break;
				}
                bool isLock = i == itemLineAreaNum;
                SingleItemLineArea newItemLineArea = GetNewSingleItemLineArea(i);
                newItemLineArea.Init(i, itemLineAreaNum, isLock, this, NoticeToDragSlerp);
				MyItemLineAreaList.Add(newItemLineArea);
            }
            ResetItemListPanelPosition();

            if (m_noticeToDragAmount != 0)
            {
                StartCoroutine(DragAmountSlerp(m_noticeToDragAmount));
                m_noticeToDragAmount = 0;
            }
            else
            {               
                if (HasGuideArrow)
                {
                    m_dragPanelComp.LockDraggable = true;
                }
                else
                {
                    m_dragPanelComp.LockDraggable = false;
                }
            }
        }       
        /// <summary>
        /// 是否有引导箭头
        /// </summary>
        /// <returns></returns>
        private bool HasGuideArrow
        {
            get
            {
                foreach (var item in MyItemLineAreaList)
                {
                    if (item.HasGuideArrow)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// 记下要自动滚动到的位置
        /// </summary>
        /// <param name="targetAmount"></param>
        private void NoticeToDragSlerp(float targetAmount)
        {
            m_noticeToDragAmount = targetAmount;
        }

        private IEnumerator DragAmountSlerp(float targeAmount)
        {
            yield return null;
            if (m_shouldMove)
            {
                float smoothTime = 0.3f, currentSmoothTime = 0; ;
                float currentAmount = 0;
                while (true)
                {
                    currentSmoothTime += Time.deltaTime;
                    currentAmount = Mathf.Lerp(currentAmount, targeAmount, Time.deltaTime * 20);
                    m_dragPanelComp.SetDragAmount(0, currentAmount, false);
                    yield return null;
                    if ((targeAmount - currentAmount) <= float.Epsilon || currentSmoothTime >= smoothTime)
                    {
                        //如果有引导箭头，不允许拖动
                        if (HasGuideArrow)
                        {
                            m_dragPanelComp.LockDraggable = true;
                        }
                        else
                        {
                            m_dragPanelComp.LockDraggable = false;
                        }
                        break;
                    }
                }
            }
        }
        void Update()
        {
            if (m_dragPanelComp != null && m_dragPanelComp.LockDraggable)
            {
                m_dragPanelComp.LockDraggable = HasGuideArrow;
            }
        }
		SingleItemLineArea GetNewSingleItemLineArea(int index)
		{
			SingleItemLineArea newItemLineArea = CreatObjectToNGUI.InstantiateObj(SingleItemLineAreaPrefab,Grid).GetComponent<SingleItemLineArea>();
			newItemLineArea.transform.localPosition = new Vector3(0,120 - 120*index,0);
			return newItemLineArea;
		}

		void GetBestItemList()
		{
			BestItemList.Clear();
			foreach(EquiptSlotType child in Enum.GetValues(typeof(EquiptSlotType)))
			{
				if(child == EquiptSlotType.Medicine)
					continue;
				var bestItem = GetBestItem.GetBestItemInPlace(child);
				if(bestItem!=null)
				{
					BestItemList.Add(child,bestItem);
				}
			}
		}

		/// <summary>
		/// 重置物品列表位置
		/// </summary>
		void ResetItemListPanelPosition()
		{
			m_ItemListUIPanel.transform.localPosition = new Vector3(0,20,0);
			m_ItemListUIPanel.clipRange = new Vector4(0,0,370,375);
		}

		/// <summary>
		/// 整理背包按钮
		/// </summary>
		/// <param name="obj"></param>
		public void OnResetContainerBtnClick(object obj)
		{
            OnShowItemTypeButtonClick(ShowItemBtnType.All);
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ResetDragComponentStatus,null);
			SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageArrangement");
			var ContainerListInfo = ContainerInfomanager.Instance.sBuildContainerClientContexts.First(P=>P.dwContainerName == 2);
			SMsgContainerTidy_CS dataStruct = new SMsgContainerTidy_CS();
			dataStruct.dwContainerID1 = ContainerListInfo.dwContainerID;
			dataStruct.dwContainerID2 = ContainerListInfo.dwContainerID;
			NetServiceManager.Instance.ContainerService.SendContainerTidy(dataStruct);
		}
		/// <summary>
		/// 点击解锁背包格
		/// </summary>
		public void OnUnLockContainerBoxBtnClick()
		{
			if(UnLockContaienrBoxObj == null){UnLockContaienrBoxObj = CreatObjectToNGUI.InstantiateObj(UnLockContainerBoxPrefab,transform.parent).GetComponent<UnlockContainerBoxTips>();}
			string Msg = LanguageTextManager.GetString("IDS_I1_4");
            UnLockContaienrBoxObj.Show(Msg, SendContainerChangeSize, c=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageUnlockCancel");}, LanguageTextManager.GetString("IDS_H2_55"), LanguageTextManager.GetString("IDS_H2_28"));
		}
			
			public void SendContainerChangeSize(object obj)//向后台发送解锁背包消息
		    {
			    SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageUnlockConfirmation");
				NetServiceManager.Instance.ContainerService.SendContainerChangeSize(ContainerInfomanager.Instance.GetContainerClientContsext(2).dwContainerID);
			}

			
			/// <summary>
			/// 收到解锁新背包格消息
			/// </summary>
		void ReciveUnLockPackMsg(object obj)
		{
			SingleItemLineArea lockItemArea = MyItemLineAreaList.FirstOrDefault(P=>P.IsLock);
			if(lockItemArea!=null)
			{
				int newIndex = MyItemLineAreaList.Count;

	            lockItemArea.UnLockMySelf();
				// 包裹的最大数目是 80，
				if(newIndex < 80/4)
				{
					SingleItemLineArea newItemLineArea = GetNewSingleItemLineArea(newIndex);
	                newItemLineArea.Init(newIndex, newIndex, true, this, NoticeToDragSlerp);
					MyItemLineAreaList.Add(newItemLineArea);
                }

			}
		}
		/// <summary>
		/// 设置顶部三个按钮的激活与否
		/// </summary>
		/// <param name="flag">If set to <c>true</c> flag.</param>
		public void SetTitleBtnActive(bool flag)
		{
			AllItemButton.gameObject.SetActive(flag);
			EquiptItemButton.gameObject.SetActive(flag);
			OtherItemButton.gameObject.SetActive(flag);
            JewelItemButton.gameObject.SetActive(flag);
		}

		public void SetItemSelectStatus(object obj)
		{
			MyItemLineAreaList.ApplyAllItem(P=>P.SetItemSelectStatus(obj));
		}

		/// <summary>
		/// 点击批量出售按钮 
		/// </summary>
		/// <param name="obj">Object.</param>
		public void OnMorItemBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageBulkSaleEnter");
			MyParent.m_SellItemsPanel.TweenShow(MyParent);
		}

        public void Close()
        {
        }

    }
}