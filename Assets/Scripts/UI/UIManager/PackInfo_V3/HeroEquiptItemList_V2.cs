using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{
    /// <summary>
    /// 人物装备栏控制器
    /// </summary>
    public class HeroEquiptItemList_V2 : DragComponentSlot
    {

        public ContainerPackList_V2 ContainerPackListManager;

        public HeroEquiptBoxSlot_V2[] HeroEquipContainerList;//人物装备栏列表,手动关联
        public HeroEquipMedicineSlot heroEquipMedicineSlot;//人物药品装备栏

        public PackInfo_V3 MyParent { get; private set; }

        public void Init(PackInfo_V3 myParent)
        {
            this.MyParent = myParent;
        }

        void Awake()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods, ResetPanel);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, ResetPanel);
        }

        public void UpdateSelectStatus(SingleContainerBox selectBox)
        {
            HeroEquipContainerList.ApplyAllItem(P => P.SetSelectStatus(P.MyContainerBox != null && P.MyContainerBox == selectBox));
            heroEquipMedicineSlot.SetSelectStatus(heroEquipMedicineSlot.MyContainerBox != null && heroEquipMedicineSlot.MyContainerBox == selectBox);
        }

        /// <summary>
        /// 刷新面板信息
        /// </summary>
        /// <param name="obj"></param>
        public void ResetPanel(object obj)
        {
            UpdateSelectStatus(null);
            MyParent.CloseContainerTips();
            //ContainerInfomanager.Instance.GetContainerClientContsext
            for (int i = 0; i < HeroEquipContainerList.Length; i++)
            {
                HeroEquipContainerList[i].Show(i,this);
            }
            heroEquipMedicineSlot.Show(this);
        }
        /// <summary>
        /// 移除某个拖拽的物件
        /// </summary>
        /// <param name="dragComponent"></param>
        public void RemoveItem(DragComponent dragComponent)
        {
            SingleContainerBox DragItem= dragComponent as SingleContainerBox;
            HeroEquiptBoxSlot_V2 ClearSlot = HeroEquipContainerList.FirstOrDefault(P=>P.MyItem == dragComponent);
            if (ClearSlot == null)
            {
                if (heroEquipMedicineSlot.MyItem == dragComponent)
                {
                    heroEquipMedicineSlot.ClearUpItem();
                }
            }
            if (ClearSlot != null)
            {
                ClearSlot.ClearUpItem();
            }
        }
        /// <summary>
        /// 检测是否能够装备
        /// </summary>
        /// <param name="itemFielInfo"></param>
        /// <returns></returns>
        public bool CheckCanEquipt(ItemFielInfo itemFielInfo)
        {
            bool flag = false;
            EquipButtonType equipButtonType = GetEquipItemStatus(itemFielInfo);
            switch (equipButtonType)
            {
                case EquipButtonType.CanEquip:
                    flag = true;
                    break;
                case EquipButtonType.ProfesionNotEnough:
                    break;
                case EquipButtonType.LVNotEnough:
                    break;
                default:
                    break;
            }
            return flag;
        }
        /// <summary>
        /// 装备物品
        /// </summary>
        /// <param name="itemFielInfo"></param>
        public void EquiptItem(DragComponent dragComponent)
        {
            ContainerPackListManager.RemoveItemFromSlot(dragComponent);
            ItemFielInfo equipmentItem = (dragComponent as SingleContainerBox).itemFielInfo;
            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = equipmentItem.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
            dataStruct.byPlace = (byte)equipmentItem.sSyncContainerGoods_SC.nPlace;
            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
            dataStruct.byUseType = equipmentItem.LocalItemData._GoodsClass == 2 ? (byte)1 : (byte)0;
            NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
        }

        /// <summary>
        /// 物品能否装备
        /// </summary>
        /// <param name="itemFielInfo"></param>
        /// <returns></returns>
        public EquipButtonType GetEquipItemStatus(ItemFielInfo itemFielInfo)
        {
            //print("EquipItemChild");
            int ItemEquipLevel = itemFielInfo.LocalItemData._AllowLevel;
            int HeroLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            string[] ItemVocation = itemFielInfo.LocalItemData._AllowProfession.Split('+');
            int HeroVocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            if (HeroLevel >= ItemEquipLevel)
            {
                foreach (string child in ItemVocation)
                {
                    int Vocation = int.Parse(child);
                    if (Vocation == 5 || HeroVocation == Vocation)
                    {//装备
                        return EquipButtonType.CanEquip;
                    }
                }
                //职业不符
                return EquipButtonType.ProfesionNotEnough;
            }
            else
            {
                //等级不符
                return EquipButtonType.LVNotEnough;
            }
        }
        /// <summary>
        /// 检测是否能够装备
        /// </summary>
        /// <param name="dragChild"></param>
        /// <returns></returns>
        public override bool CheckIsPair(DragComponent dragChild)
        {
            bool flag = false;
            ItemFielInfo equiptItemInfo = (dragChild as SingleContainerBox).itemFielInfo;
            if (equiptItemInfo.LocalItemData._GoodsClass == 1 &&
                ItemPlaceToIndex(int.Parse((equiptItemInfo.LocalItemData as EquipmentData)._vectEquipLoc)) != -1 &&
                CheckCanEquipt(equiptItemInfo))//装备类
            {
                flag = true;
                dragChild.NewSlotPoint = HeroEquipContainerList.First(P => P.MyPositionID == ItemPlaceToIndex(int.Parse((equiptItemInfo.LocalItemData as EquipmentData)._vectEquipLoc))).transform;
            }
            else if (equiptItemInfo.LocalItemData._GoodsClass == 2 && CheckCanEquipt(equiptItemInfo))//药品类
            {
                dragChild.NewSlotPoint = heroEquipMedicineSlot.transform;
                flag = true;
            }
            return flag;
        }

        public override void MoveToHere(DragComponent enterComponent)
        {
            ItemFielInfo equiptItemInfo = (enterComponent as SingleContainerBox).itemFielInfo;
            if (CheckIsEquiptItem(enterComponent))
            {
                (enterComponent as SingleContainerBox).UpdatePos();
                //MyParent.CloseContainerTips();
                return;
            }
            if (equiptItemInfo.LocalItemData._GoodsClass == 2)//装备药品
            {
                heroEquipMedicineSlot.MoveToHere(enterComponent);
            }
            else
            {
                HeroEquipContainerList.First(P => P.MyPositionID == ItemPlaceToIndex(int.Parse((equiptItemInfo.LocalItemData as EquipmentData)._vectEquipLoc))).MoveToHere(enterComponent);
            }
        }

        /// <summary>
        /// 检测是否已经装备了的装备
        /// </summary>
        /// <returns></returns>
        public bool CheckIsEquiptItem(DragComponent dragComponent)
        {
            bool flag = false;
            flag = HeroEquipContainerList.FirstOrDefault(P => P.MyItem == dragComponent) !=null;
            flag = flag ? flag : heroEquipMedicineSlot.MyItem == dragComponent;
            return flag;
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
    }
}