using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class GetNewItemTipsPanel : MonoBehaviour
    {

        public GameObject GetNewItemTipsPrefab; 
        //private GetNewItemTips getNewItemTips;
		public Vector3 newItemCenterPos;
        private ItemFielInfo EquipmentItemFielInfo;

        public void Start()
        {
            CheckGetBestItem(null);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.CheckNewItem,CheckGetBestItem);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods,CheckGetBestItem);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CheckNewItem, CheckGetBestItem);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods,CheckGetBestItem);
        }

        //public void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.C))
        //    {
        //        CheckGetBestItem(null);
        //    }
        //}
        public void CheckGetBestItem(object obj)
        {
            transform.ClearChild();
			//old
			/*
            var playerLv = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            if (playerLv <= CommonDefineManager.Instance.CommonDefine.EquipmentTipsStartLevel)  //当玩家等级小于指定等级，不弹出新装备面板
                return;
            var itemList = ContainerInfomanager.Instance.itemFielArrayInfo;
			List<int> canShowItemList = new List<int> ();
            for (int i = 0; i < itemList.Count; i++)
            {
				if (itemList[i] != null && itemList[i].LocalItemData._GoodsClass == 1 && itemList[i].LocalItemData._GoodsSubClass != 2 
				    && itemList[i].LocalItemData._GoodsSubClass != 7&& CheckIsBestItem(itemList[i]))
                {
					if(PlayerDataManager.Instance.CanPopTip(EViewType.ENewEquipType))
					{
						canShowItemList.Add(i);
                    	//ShowGetBestItemMsg(itemList[i],i);
					}
                }
            }
			if (canShowItemList.Count != 0) {
				int showItemIndex = Random.Range (0,canShowItemList.Count);
				int index = canShowItemList[showItemIndex];
				ShowGetBestItemMsg(itemList[index],0);
			}
			*/
			//new
			Dictionary<EquiptSlotType,ItemFielInfo> equipMap = ContainerInfomanager.Instance.GetBestItemList ();
			List<ItemFielInfo> goodsList = ContainerInfomanager.Instance.GetAllUsableGoods ();
			int index = equipMap.Count+goodsList.Count;
		//	Debug.Log ("CheckGetBestItem====count="+index);
			foreach (var equip in equipMap) {
				ShowGetBestItemMsg (equip.Value, index--);
			}
			foreach (ItemFielInfo info in goodsList) {
				ShowGetBestItemMsg (info, index--);			
			}
        }

        bool CheckIsBestItem(ItemFielInfo itemFielInfo)
        {
            bool flag = true;
            EffectData selectEffectData = null;
            EquipmentEntity selectEquipmentEntity = itemFielInfo.equipmentEntity;
            EffectData currentEffectData = null;
			TraceUtil.Log(SystemModel.Jiang,"GetItemID:"+(itemFielInfo.LocalItemData._goodID).ToString()+","+LanguageTextManager.GetString(itemFielInfo.LocalItemData._szGoodsName));
            var equipItem = ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(1).FirstOrDefault(P=>P.nPlace == int.Parse((itemFielInfo.LocalItemData as EquipmentData)._vectEquipLoc)&&P.uidGoods!=0);
            ItemFielInfo currentItemfile = ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(P => P.sSyncContainerGoods_SC.uidGoods == equipItem.uidGoods);
            EquipmentEntity currentEquipmentEntity = currentItemfile == null ? new EquipmentEntity() : currentItemfile.equipmentEntity;
            if (currentItemfile != null && currentItemfile == itemFielInfo)
            {
                flag = false;
            }
            if (GetEquipItemStatus(itemFielInfo) != EquipButtonType.CanEquip)
            {
                flag = false;
            }
            if (flag)
            {
                bool effect1 = false;
                bool effect2 = false;
                for (int i = 0; i < 2; i++)
                {
                    selectEffectData = ItemDataManager.Instance.EffectDatas._effects.FirstOrDefault(P => P.m_IEquipmentID == (i == 0 ? selectEquipmentEntity.EQUIP_FIELD_EFFECTBASE0 : selectEquipmentEntity.EQUIP_FIELD_EFFECTBASE1));
                    currentEffectData = ItemDataManager.Instance.EffectDatas._effects.FirstOrDefault(P => P.m_IEquipmentID == (i == 0 ? currentEquipmentEntity.EQUIP_FIELD_EFFECTBASE0 : currentEquipmentEntity.EQUIP_FIELD_EFFECTBASE1));
                    //if (currentEffectData == null && selectEffectData != null)
                    //{
                    //    flag = false;
                    //    break;
                    //}
                    //if (currentEffectData != null && selectEffectData != null)
                    //{
                    int currenteffect = currentEffectData == null ? 0 : EquipMainProp(currentItemfile.LocalItemData, currentEquipmentEntity, i, true, true);
                    int selecteffect = selectEffectData == null ? 0 : EquipMainProp(itemFielInfo.LocalItemData, selectEquipmentEntity, i, true, true);
                    if (selecteffect > currenteffect)
                    {
                        if (i == 0)
                        {
                            effect1 = true;
                        }
                        else
                        {
                            effect2 = true;
                        }
                    }
                    //}
                }
                flag = effect1 && effect2;
            }
            return flag;
        }

        public void ShowGetBestItemMsg(ItemFielInfo itemFielInfo,int posIndex)
        {
            EquipmentItemFielInfo = itemFielInfo;
            GetNewItemTips getNewItemTips = CreatObjectToNGUI.InstantiateObj(GetNewItemTipsPrefab, transform).GetComponent<GetNewItemTips>();
			getNewItemTips.transform.localPosition = new Vector3(newItemCenterPos.x, newItemCenterPos.y, newItemCenterPos.z-12*posIndex);
			getNewItemTips.Show(itemFielInfo);

            //MessageBox.Instance.Show(3, "", string.Format("获得了一件更好的装备：{0},穿上试试吧", LanguageTextManager.GetString(itemFielInfo.LocalItemData._szGoodsName)), "确定", SendEquiptItemToserver);
            //TraceUtil.Log(string.Format("获得了一件更好的装备：{0}",LanguageTextManager.GetString(itemFielInfo.LocalItemData._szGoodsName)));
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
        public void SendEquiptItemToserver()
        {
            ItemFielInfo equipmentItem = EquipmentItemFielInfo;
            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = equipmentItem.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
            dataStruct.byPlace = (byte)equipmentItem.sSyncContainerGoods_SC.nPlace;
            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
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
        /// 计算装备主属性值
        /// </summary>
        /// <param name="itemFielInfo">装备数据</param>
        /// <param name="index">装备主属性索引</param>
        /// <param name="isBefore">是否装备前的值</param>
        /// <param name="isNormal">是否普通强化</param>
        /// <returns></returns>
        private int EquipMainProp(ItemData itemData, EquipmentEntity equipmentEntity, int index, bool isBefore, bool isNormal)
        {
            int normalStrengthenLv = equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL;
            int starStrengthenLv = equipmentEntity.EQUIP_FIELD_START_LEVEL;
            if (!isBefore)
            {
                if (isNormal)
                {
                    normalStrengthenLv += 1;
                }
                else
                {
                    starStrengthenLv += 1;
                }
            }

            var equipItemData = ItemDataManager.Instance.GetItemData(itemData._goodID) as EquipmentData;
            StrengthParameter strengthParameter = equipItemData._StrengthParameter[index];

			int normalMainProAdd =normalStrengthenLv==0||normalStrengthenLv>CommonDefineManager.Instance.CommonDefine.StrengthLimit?0:strengthParameter.Value[normalStrengthenLv-1]; 
            //float startAddPercent = 0.05f * starStrengthenLv;
            float startAddPercent = 0;
            int sourceMainProValue = 0;
            switch (index)
            {
                case 0:
                    sourceMainProValue = equipmentEntity.EQUIP_FIELD_EFFECTBASE0_VALUE;
                    break;
                case 1:
                    sourceMainProValue = equipmentEntity.EQUIP_FIELD_EFFECTBASE1_VALUE;
                    break;
            }
            return Mathf.FloorToInt((sourceMainProValue + normalMainProAdd) * (1 + startAddPercent));
        }
    }
}