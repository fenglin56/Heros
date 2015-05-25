using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{
    /// <summary>
    /// 选择炼化装备界面
    /// </summary>
    public class SelectArtificeGoodsPanel : MonoBehaviour
    {
        public UILabel PageLabel;
        public SingleButtonCallBack OperateArtificeBtn;//炼化按钮
        public SingleButtonCallBack CloseBtn;
        public SingleButtonCallBack LastPageBtn;
        public SingleButtonCallBack NextPageBtn;

        public GameObject SingleSelectArtificeGoodsDragPanelPreafab;
        public OperateArtificeMessagePanel OperateArtificeMessagePanel;//确认是否进行炼化消息窗口
        public SingleArtificeGoodsSlot[] SingleArtificeGoodsSlotList;//装备槽列表


        public GoodsOperateArtificePanel MyParent { get;private set;}
        public ItemFielInfo SelectItemFileInfo { get; private set; }
        public List<ArtificeItemFielInfo> CurrentItemList { get;private set; }

        int CurrentPageNumber = 1;

        public bool IsShow { get; private set; }

        Vector3 CloseScale = new Vector3(0.8f,0.8f,0.8f);
        Vector3 ShowScale = new Vector3(1,1,1);

		private int[] m_guideID;

        void Start()
        {
            CloseBtn.SetCallBackFuntion(MyParent.OnEquipmentOperateArtificeBtnClick);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods, UpdatePanel);
            LastPageBtn.SetCallBackFuntion(OnLastPageBtnClick);
            NextPageBtn.SetCallBackFuntion(OnNextPageBtnClick);
			m_guideID = new int[4];
			//TODO GuideBtnManager.Instance.RegGuideButton (CloseBtn.gameObject, UIType.EquipStrengthen, SubType.EquipStrenSelectArtifice, out m_guideID [0]);
			//TODO GuideBtnManager.Instance.RegGuideButton (LastPageBtn.gameObject, UIType.EquipStrengthen, SubType.EquipStrenSelectArtifice, out m_guideID [1]);
			//TODO GuideBtnManager.Instance.RegGuideButton (NextPageBtn.gameObject, UIType.EquipStrengthen, SubType.EquipStrenSelectArtifice, out m_guideID [2]);
			//TODO GuideBtnManager.Instance.RegGuideButton (OperateArtificeBtn.gameObject, UIType.EquipStrengthen, SubType.EquipStrenSelectArtifice, out m_guideID [3]);
        }
        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, UpdatePanel);
            if (m_guideID != null)
            {
                for (int i = 0; i < m_guideID.Length; i++)
                {
                    //TODO GuideBtnManager.Instance.DelGuideButton(m_guideID[i]);
                }
            }
        }

        public void Init(GoodsOperateArtificePanel myParent)
        {
            this.MyParent = myParent;
            OperateArtificeBtn.SetCallBackFuntion(OnOperateArtificeBtnClick);

            transform.localScale = CloseScale;
            gameObject.SetActive(false);
            transform.localPosition = new Vector3(428,-54,-35);
        }

        public void TweenShow(ItemFielInfo selectItemFileInfo)
        {
            this.SelectItemFileInfo = selectItemFileInfo;
            IsShow = true;
            gameObject.SetActive(true);
            TweenScale.Begin(gameObject,0.1f,transform.localScale,ShowScale,TweenShowComplete);
            UpdatePanel(null);
        }

        void OnCloseBtnClick(object obj)
        {
            TweenClose();
        }

        public void TweenClose()
        {
            IsShow = false;
            CurrentItemList.Clear();
            TweenScale.Begin(gameObject, 0.1f, transform.localScale, CloseScale, TweenCloseComplete);
        }

        void TweenShowComplete(object obj)
        {
        }

        void TweenCloseComplete(object obj)
        {
            gameObject.SetActive(false);
        }

        void OnLastPageBtnClick(object obj)
        {
            CurrentPageNumber--;
            CurrentPageNumber = CurrentPageNumber < 1 ? 1 : CurrentPageNumber;
            UpdateSlotList(null);
        }

        void OnNextPageBtnClick(object obj)
        {
            int maxPageNumber = GetPageCount();
            CurrentPageNumber++;
            CurrentPageNumber = CurrentPageNumber >= maxPageNumber ? maxPageNumber : CurrentPageNumber;
            UpdateSlotList(null);
        }

        int GetPageCount()
        {
            int pageCount = CurrentItemList.Count / 6 + (CurrentItemList.Count % 6 > 0 ? 1 : 0);
            pageCount = pageCount == 0 ? 1 : pageCount;
            return pageCount;
        }

        void UpdatePanel(object obj)
        {
            CurrentPageNumber = 1;
            CurrentItemList = GetPairItemList();
            UpdateSlotList(null);
        }

        public void UpdateSlotList(object obj)
        {
            if (!IsShow)
                return;
            SingleArtificeGoodsSlotList.ApplyAllItem(P=>P.Cleanup());
            if (CurrentItemList.Count > 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int currentItemIndex = i + (CurrentPageNumber - 1) * 6;
                    SingleArtificeGoodsSlotList[i].Show(CurrentItemList.Count > currentItemIndex ? CurrentItemList[currentItemIndex] : null, this);
                }
            }
            else
            {
                SingleArtificeGoodsSlotList[0].ShowSirenItem(this);
            }
            Color enabelColor = new Color(1,1,1,1);
            Color disabelColor = new Color(1,1,1,0.5f);
            LastPageBtn.BackgroundSprite.color = CurrentPageNumber == 1 ? disabelColor : enabelColor;
            NextPageBtn.BackgroundSprite.color = CurrentPageNumber == GetPageCount() ? disabelColor : enabelColor;
            PageLabel.SetText(string.Format("{0}/{1}",CurrentPageNumber,GetPageCount()));
        }

        /// <summary>
        /// 获取配对的物品列表
        /// </summary>
        /// <returns></returns>
        List<ArtificeItemFielInfo> GetPairItemList()
        {
            var SelectItemList = ContainerInfomanager.Instance.itemFielArrayInfo.FindAll(P => (P.LocalItemData._GoodsClass == SelectItemFileInfo.LocalItemData._GoodsClass &&
                P.LocalItemData._GoodsSubClass == SelectItemFileInfo.LocalItemData._GoodsSubClass)||
            (P.LocalItemData._GoodsClass == 3 && P.LocalItemData._GoodsSubClass == 9));
            var equiptItem = ContainerInfomanager.Instance.GetEquiptItemList().FirstOrDefault(P=>P.LocalItemData._GoodsClass == SelectItemFileInfo.LocalItemData._GoodsClass&&
                P.LocalItemData._GoodsSubClass == SelectItemFileInfo.LocalItemData._GoodsSubClass);
            if (equiptItem != null&&SelectItemList.Contains(equiptItem))
            {
                SelectItemList.Remove(equiptItem);
            }
            SelectItemList.Remove(SelectItemFileInfo);
            SelectItemList.Sort((left, right) =>
            {
                if (left.LocalItemData._goodID > right.LocalItemData._goodID)
                {
                    return 1;
                }
                else if (left.LocalItemData._goodID == right.LocalItemData._goodID)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            });
            List<ArtificeItemFielInfo> getItemFileInfoList = new List<ArtificeItemFielInfo>();
            SelectItemList.ApplyAllItem(P=>getItemFileInfoList.Add(new ArtificeItemFielInfo(P)));
            return getItemFileInfoList;
        }

        void OnOperateArtificeBtnClick(object obj)
        {
            var selectItemList = GetSelectItemList();
            if (MyParent.CheckIsFullLevel(MyParent.SelectItemData))
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_526"), 1);
                return;
            }
            if (selectItemList.Count == 0)
            {
                //MessageBox.Instance.ShowTips(3, "请选择炼化装备", 1);
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_521"),1);
            }
            else
            {
                //弹出炼化确认界面
                OperateArtificeMessagePanel.TweenShow(selectItemList,this);
            }
        }

        public List<ItemFielInfo> GetSelectItemList()
        {
            List<ItemFielInfo> selectItemList = new List<ItemFielInfo>();
            if (CurrentItemList != null)
            {
                foreach (var child in CurrentItemList)
                {
                    if (child.IsSelect)
                    {
                        selectItemList.Add(child.MyItemfielInfo);
                    }
                }
            }
            return selectItemList;
        }

        public bool CheckCanSelect()
        {
            return GetSelectItemList().Count < 6;
        }
    }

    public class ArtificeItemFielInfo
    {
        public ItemFielInfo MyItemfielInfo;
        public bool IsSelect = false;

        public ArtificeItemFielInfo(ItemFielInfo myItemFielInfo)
        {
            this.MyItemfielInfo = myItemFielInfo;
        }
    }
}