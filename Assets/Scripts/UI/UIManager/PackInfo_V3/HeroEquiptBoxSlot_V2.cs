using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class HeroEquiptBoxSlot_V2 : MonoBehaviour
    {
        public UISprite Background;
        public GameObject ContainerBoxPrefab;//需要生成的物品格预置物
        public Transform CreatContainerBoxPoint;

        public DragComponent MyItem { get; private set; }//装备到该槽上的装备
        public SingleContainerBox MyContainerBox { get { return MyItem as SingleContainerBox; } }

        SSyncContainerGoods_SC equipContainerInfo;
        private Vector3 BackGround_Scale;
        public int MyPositionID { get; private set; }
        private HeroEquiptItemList_V2 MyParent;
        private int m_guideBtnID;

        public bool CanDrag = true;

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(gameObject, UI.MainUI.UIType.Package, SubType.PackageHeroBoxSlot, out m_guideBtnID);
            BackGround_Scale = Background.transform.localScale;

            //强引导下不能够拖拽
            CanDrag = GuideBtnManager.Instance.IsEndGuide || TaskModel.Instance.TaskGuideType != TaskGuideType.Enforce;// !NewbieGuideManager_V2.Instance.IsConstraintGuide;
        }

        void OnDestroy()
        {
            //if (MyItem != null)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
            }
        }

        public void Show(int MyPositionID,HeroEquiptItemList_V2 MyParent)
        {
            this.MyParent = MyParent;
            this.MyPositionID = MyPositionID;
            var Equiplist = ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(1);
            this.equipContainerInfo = Equiplist.FirstOrDefault(P => ItemPlaceToIndex(P.nPlace)== MyPositionID);
            if (this.equipContainerInfo.uidGoods != 0)
            {
                ItemFielInfo creatItemInfo = ContainerInfomanager.Instance.itemFielArrayInfo.SingleOrDefault(P => P.sSyncContainerGoods_SC.uidGoods == equipContainerInfo.uidGoods);
                if (creatItemInfo != null)
                {
                    ShowItem(creatItemInfo);
                }
            }
            else
            {
                ClearUpItem();
            }
        }

         void ShowItem(ItemFielInfo itemFielInfo)
        {
            CreatContainerBoxPoint.ClearChild();
            SingleContainerBox creatItem = CreatObjectToNGUI.InstantiateObj(ContainerBoxPrefab, CreatContainerBoxPoint).GetComponent<SingleContainerBox>();
            creatItem.Init(itemFielInfo,SingleContainerBoxType.HeroEquip);
            //creatItem.gameObject.collider.enabled = CanDrag;
            creatItem.SetDragComponentEnabel(CanDrag);
            MyItem = creatItem;
        }


        public void ClearUpItem()
        {
            CreatContainerBoxPoint.ClearChild();
            MyItem = null;
        }

        /// <summary>
        /// 将位置转成数组索引
        /// </summary>
        /// <returns></returns>
        int ItemPlaceToIndex(int nPlace)
        {
            int ItemIndex = -1;
            switch (nPlace)
            {
                case 0://武器
                    ItemIndex = 2;
                    break;
                case 11://头饰
                    ItemIndex = 0;
                    break;
                case 12://衣服
                    ItemIndex = 1;
                    break;
                case 13://鞋子
                    ItemIndex = 4;
                    break;
                case 14://饰品
                    ItemIndex = 3;
                    break;
                //case 15://徽章
                //    ItemIndex = 0;
                    //break;
                default:
                    break;
            }
            return ItemIndex;
        }

        public void SetSelectStatus(bool flag)
        {
            if (MyContainerBox != null)
            {
                MyContainerBox.SetSelectStatus(flag);
            }
        }

        public void MoveToHere(DragComponent enterComponent)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equip");
            var itemData = enterComponent as SingleContainerBox;
            ItemFielInfo equiptItemInfo = itemData.itemFielInfo;
            ShowItem(equiptItemInfo);
            MyParent.EquiptItem(enterComponent);
            //OnTouchSlot();

            //NewbieGuideManager_V2.Instance.IsDragGuide(m_guideBtnID, itemData.GuideID);
        }


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