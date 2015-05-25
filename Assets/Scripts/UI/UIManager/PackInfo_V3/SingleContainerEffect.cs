using UnityEngine;
using System.Collections;
using System.Linq;


namespace UI.MainUI
{

    public class SingleContainerEffect : MonoBehaviour
    {
        public UISprite EffectIcon;
        public UILabel EffectLabel;

        public void ShowEffect(ItemFielInfo itemFielInfo, int EffectIndex)
        {
            EffectData effectData = null;
            EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
            switch (EffectIndex)
            {
                case 0:
                    effectData = ItemDataManager.Instance.EffectDatas._effects.First(P => P.m_IEquipmentID == equipmentEntity.EQUIP_FIELD_EFFECTBASE0);
                    break;
                case 1:
                    effectData = ItemDataManager.Instance.EffectDatas._effects.First(P => P.m_IEquipmentID == equipmentEntity.EQUIP_FIELD_EFFECTBASE1);
                    break;
            }
            EffectIcon.spriteName = effectData.EffectRes;
            EffectLabel.SetText(HeroAttributeScale.GetScaleAttribute(effectData,NormalStrengthenNormalValue(itemFielInfo,EffectIndex)));
        }


        /// <summary>
        /// 计算普通强化主属性
        /// </summary>
        /// <param name="itemFielInfo"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private int NormalStrengthenNormalValue(ItemFielInfo itemFielInfo, int index)
        {
            ItemData itemData = itemFielInfo.LocalItemData;
            EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
            var normalValue = EquipMainProp(itemData, equipmentEntity, index, true, true);
            
            return normalValue;
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

            float startAddPercent = 0;  //新版装备强化，不再加星级加成数值
            //float startAddPercent = 0.05f * starStrengthenLv;

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
            var procValue = (itemData as EquipmentData)._lThresholdValue;

            normalMainProAdd += Mathf.FloorToInt((Mathf.Max(normalStrengthenLv, procValue) - procValue) * 0.05f * sourceMainProValue);

            //TraceUtil.Log("index:" + index + " 基础属性：" + sourceMainProValue + "  isBefore:" + isBefore + " AddValue:" + normalMainProAdd);
            return Mathf.FloorToInt((sourceMainProValue + normalMainProAdd) * (1 + startAddPercent));
        }
    }
}