using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI{
	

	
	public enum EquipButtonType { CanEquip, LVNotEnough, ProfesionNotEnough };

	public class GetBestItem {


		public static ItemFielInfo GetBestItemInPlace(EquiptSlotType itemType)
		{
			ItemFielInfo bestItem = null;
			int place = (int) itemType;
			List<ItemFielInfo> myPlaceItem = ContainerInfomanager.Instance.GetPackItemList().FindAll(
				P=>P.LocalItemData._GoodsClass ==1&&P.LocalItemData._GoodsSubClass!=2&&int.Parse((P.LocalItemData as EquipmentData)._vectEquipLoc) == place);
            bestItem = ContainerInfomanager.Instance.GetEquiptItemList().FirstOrDefault(P=>P.sSyncContainerGoods_SC.nPlace == place);
			for(int i = 0;i<myPlaceItem.Count;i++)
			{
                bestItem = GetTheBestItem(bestItem,myPlaceItem[i],itemType);
			}
			return bestItem;
		}

        static ItemFielInfo GetTheBestItem(ItemFielInfo firstItemfile,ItemFielInfo scecondItemFielInfo,EquiptSlotType itemType)
		{
			bool flag = true;
			EffectData selectEffectData = null;
			EquipmentEntity selectEquipmentEntity = scecondItemFielInfo.equipmentEntity;
			EffectData currentEffectData = null;
//			var equipItem = ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(1).FirstOrDefault(P=>P.nPlace == int.Parse((scecondItemFielInfo.LocalItemData as EquipmentData)._vectEquipLoc)&&P.uidGoods!=0);
//			ItemFielInfo currentItemfile = ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(P => P.sSyncContainerGoods_SC.uidGoods == equipItem.uidGoods);
			EquipmentEntity currentEquipmentEntity = firstItemfile == null ? new EquipmentEntity() : firstItemfile.equipmentEntity;
			if (firstItemfile != null && firstItemfile == scecondItemFielInfo)
			{
				flag = false;
			}
			if (GetEquipItemStatus(scecondItemFielInfo) != EquipButtonType.CanEquip)
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
                    int currenteffect = currentEffectData == null ? 0 : EquipMainProp(firstItemfile.LocalItemData, currentEquipmentEntity, i, true, true,itemType);
                    int selecteffect = selectEffectData == null ? 0 : EquipMainProp(scecondItemFielInfo.LocalItemData, selectEquipmentEntity, i, true, true,itemType);
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
				}
				flag = effect1 && effect2;
			}
			return flag?scecondItemFielInfo:firstItemfile;
		}
		
		
		/// <summary>
		/// 检测是否能够装备
		/// </summary>
		/// <param name="itemFielInfo"></param>
		/// <returns></returns>
		static bool CheckCanEquipt(ItemFielInfo itemFielInfo)
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
		/// 物品能否装备
		/// </summary>}
		/// <param name="itemFielInfo"></param>
		/// <returns></returns>
		public static EquipButtonType GetEquipItemStatus(ItemFielInfo itemFielInfo)
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
        static int EquipMainProp(ItemData itemData, EquipmentEntity equipmentEntity, int index, bool isBefore, bool isNormal,EquiptSlotType itemType)
		{

            int normalStrengthenLv =PlayerDataManager.Instance.GetEquipmentStrengthLevel(itemType);
			int starStrengthenLv =PlayerDataManager.Instance.GetEquipmentStarLevel(itemType);
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
            StrengthParameter starStrengthParameter=equipItemData._StartStrengthParameter[index];
			int normalMainProAdd =normalStrengthenLv==0||normalStrengthenLv>CommonDefineManager.Instance.CommonDefine.StrengthLimit?0:strengthParameter.Value[normalStrengthenLv-1];
			int normalMainProAddForStar =starStrengthenLv==0||starStrengthenLv>CommonDefineManager.Instance.CommonDefine.StartStrengthLimit?0:starStrengthParameter.Value[starStrengthenLv-1];
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
            return sourceMainProValue + normalMainProAdd+normalMainProAddForStar;
		}
	}
}