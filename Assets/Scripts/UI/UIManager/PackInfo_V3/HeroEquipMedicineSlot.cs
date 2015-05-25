using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class HeroEquipMedicineSlot : MonoBehaviour
    {

        public UISprite Background;
        public GameObject ContainerBoxPrefab;//需要生成的物品格预置物
        public Transform CreatContainerBoxPoint;

        public DragComponent MyItem { get; private set; }//装备到该槽上的药品
        public SingleContainerBox MyContainerBox { get { return MyItem as SingleContainerBox; } }

        private HeroEquiptItemList_V2 MyParent;
        private Vector3 BackGround_Scale;
        private int m_guideBtnID;

        public bool CanDrag = true;

        public void Start()
        {
            BackGround_Scale = Background.transform.localScale;
            //TODO GuideBtnManager.Instance.RegGuideButton(gameObject, UI.MainUI.UIType.Package, SubType.PackageHeroBoxSlot, out m_guideBtnID);
            //base.Start();
        }

        void OnDestroy()
        {
            //if (MyItem != null)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
            }
        }

        public void Show(HeroEquiptItemList_V2 MyParent)
        {
            this.MyParent = MyParent;
            var currentPack = ContainerInfomanager.Instance.sBuildContainerClientContexts.FirstOrDefault(P=>P.dwContainerName == 3);
            if (currentPack.SMsgActionSCHead.uidEntity == 0)
                return;
            var beLinkItem = ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(P => P.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID == currentPack.dwContainerID);
            if (beLinkItem != null)
            {
                ShowItem(beLinkItem);
            }
            else
            {
                ClearUpItem();
            }
            //if (MyItem != null)
            //{
            //    //TODO GuideBtnManager.Instance.RegGuideButton(MyItem.gameObject, UI.MainUI.UIType.PackInfo, SubType.PackageHeroBoxSlot, out m_guideBtnID);
            //}
        }

        void ShowItem(ItemFielInfo itemFielInfo)
        {
            CreatContainerBoxPoint.ClearChild();
            SingleContainerBox creatItem = CreatObjectToNGUI.InstantiateObj(ContainerBoxPrefab, CreatContainerBoxPoint).GetComponent<SingleContainerBox>();
            creatItem.Init(itemFielInfo, SingleContainerBoxType.HeroEquip);
            creatItem.SetDragComponentEnabel(CanDrag);
            MyItem = creatItem;
        }

        public void ClearUpItem()
        {
            CreatContainerBoxPoint.ClearChild();
            MyItem = null;
        }


        //public override void OnDragComponentHover()
        //{
        //    OnTouchSlot();
        //}

        public void SetSelectStatus(bool flag)
        {
            if (MyContainerBox != null)
            {
                MyContainerBox.SetSelectStatus(flag);
            }
        }

        public void MoveToHere(DragComponent enterComponent)
        {
            var itemData = enterComponent as SingleContainerBox;
            ItemFielInfo equiptItemInfo = itemData.itemFielInfo;
            ShowItem(equiptItemInfo);
            MyParent.EquiptItem(enterComponent);
            OnTouchSlot();

            //NewbieGuideManager_V2.Instance.IsDragGuide(m_guideBtnID, itemData.GuideID);
        }

        //public override bool CheckIsPair(DragComponent dragChild)
        //{
        //    bool flag = false;
        //    ItemFielInfo equiptItemInfo = (dragChild as SingleContainerBox).itemFielInfo;
        //    if (equiptItemInfo.LocalItemData._GoodsClass == 2 &&
        //        MyParent.CheckCanEquipt(equiptItemInfo))
        //    {
        //        flag = true;
        //    }
        //    return flag;
        //}

        void OnTouchSlot()
        {
            TweenScale.Begin(Background.gameObject, 0.1f, BackGround_Scale, BackGround_Scale + new Vector3(0, 10, 0), ScaleComplete);
        }

        void ScaleComplete(object obj)
        {
            TweenScale.Begin(Background.gameObject, 0.1f, BackGround_Scale);
        }
    }
}