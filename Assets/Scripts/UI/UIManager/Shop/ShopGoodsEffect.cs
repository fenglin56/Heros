using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class ShopGoodsEffect : MonoBehaviour
    {


        public UISprite EffectIcon;
        public UILabel EffectNameLabel;

        public UISprite CompareIcon;
        public UILabel CurrentEffectLabel;
        public UILabel SelectEffectLabel;

        public void ShowEffect(EquipmentData itemData, int EffectIndex)
        {
            EffectData selectEffectData = null;
            EffectData currentEffectData = null;
            var equipItem = ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(1).FirstOrDefault(P => P.nPlace == int.Parse(itemData._vectEquipLoc) && P.uidGoods != 0);
            ItemFielInfo currentItemfile = ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(P => P.sSyncContainerGoods_SC.uidGoods == equipItem.uidGoods);
            EquipmentEntity currentEquipmentEntity = currentItemfile == null ? new EquipmentEntity() : currentItemfile.equipmentEntity;
            bool haveEquipItem = equipItem.uidGoods != 0;

            switch (EffectIndex)
            {
                case 0:
                    selectEffectData = ItemDataManager.Instance.EffectDatas._effects.First(P => P.m_SzName == itemData._vectEffects.Split('|')[0].Split('+')[0]);
                    currentEffectData = ItemDataManager.Instance.EffectDatas._effects.FirstOrDefault(P => P.m_IEquipmentID == currentEquipmentEntity.EQUIP_FIELD_EFFECTBASE0);
                    break;
                case 1:
                    selectEffectData = ItemDataManager.Instance.EffectDatas._effects.First(P => P.m_SzName == itemData._vectEffects.Split('|')[1].Split('+')[0]);
                    currentEffectData = ItemDataManager.Instance.EffectDatas._effects.FirstOrDefault(P => P.m_IEquipmentID == currentEquipmentEntity.EQUIP_FIELD_EFFECTBASE1);
                    break;
            }
            bool effectAtive = selectEffectData != null || currentEffectData != null;
            gameObject.SetActive(effectAtive);
            TraceUtil.Log(string.Format("设置主属性{0}显示：{1}", EffectIndex, effectAtive.ToString()));
            if (effectAtive == false)
                return;
            EffectIcon.spriteName = selectEffectData == null ? currentEffectData.EffectRes : selectEffectData.EffectRes;
            EffectNameLabel.SetText(LanguageTextManager.GetString(selectEffectData == null ? currentEffectData.IDS : selectEffectData.IDS));
            SelectEffectLabel.SetText(selectEffectData == null ? 0 : HeroAttributeScale.GetScaleAttribute(selectEffectData, int.Parse(itemData._vectEffects.Split('|')[EffectIndex].Split('+')[1])));
            CurrentEffectLabel.SetText(currentEffectData == null ? 0 : HeroAttributeScale.GetScaleAttribute(currentEffectData,NormalStrengthenNormalValue(currentItemfile, EffectIndex)));
            SetLabelColor(int.Parse(CurrentEffectLabel.text) <= int.Parse(SelectEffectLabel.text) ? Color.green : Color.red);
            TraceUtil.Log(string.Format("设置显示的属性：{0}>{1}", CurrentEffectLabel.text, SelectEffectLabel.text));
        }

        void SetLabelColor(Color color)
        {
            SelectEffectLabel.color = color;
            CompareIcon.color = color;
            CurrentEffectLabel.color = color;
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