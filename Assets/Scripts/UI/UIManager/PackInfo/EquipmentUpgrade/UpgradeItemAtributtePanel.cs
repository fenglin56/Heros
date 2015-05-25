using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI{

	public class UpgradeItemAtributtePanel : BaseTweenShowPanel {


		public UpgradItemMainProperty m_UpgradItemMainProperty;

		public UISprite Effect01Icon;
		public UISprite Effect02Icon;
		public Transform Effect01UpArrow;
		public Transform Effect02UpArrow;
		public UILabel Effect01NameLabel;
		public UILabel Effect02NameLabel;
		public UILabel Effect01NumLabel;
		public UILabel Effect02NumLabel;
		public UILabel Effect01AddLabel;
		public UILabel Effect02AddLabel;
		public UILabel FullLevelLabel;
		public GameObject CanUpgradeObj;

		public UILabel NeedItemDesLabel;//消耗物品介绍 
		public UILabel LevelNeedLabel;//等级需求
		public UILabel HelpTipsLabel;//帮助解释
		public UILabel PriceLabel;
        private EquipmentEntity m_EquipmentEntity;


		public EquipmentUpgradePanel MyParent{get;private set;}

		public void Show(ItemFielInfo itemFielInfo,EquipmentUpgradePanel myParent)
		{
            m_EquipmentEntity=itemFielInfo.equipmentEntity;
			MyParent = myParent;
			m_UpgradItemMainProperty.Init(itemFielInfo);
			if(itemFielInfo == null)
			{
				ClearUpPanel();
				return;
			}
			var normalItem = itemFielInfo.LocalItemData as EquipmentData;
			var targetItem = ItemDataManager.Instance.GetItemData(((itemFielInfo.LocalItemData)as EquipmentData).UpgradeID) as EquipmentData;
			ShowMainEffectLabel(normalItem,targetItem);
			EquipmentData itemData = itemFielInfo.LocalItemData as EquipmentData;
			string needItemStr = "";
			if(itemData.UpgradeCost!="")
			{
				string[] costItemStr = itemData.UpgradeCost.Split('|');
				foreach(var child in costItemStr)
				{
					string[] chacheStr = child.Split('+');
                    needItemStr+=string.Format("{0}x{1}\n",LanguageTextManager.GetString(ItemDataManager.Instance.GetItemData(int.Parse(chacheStr[0]))._szGoodsName),chacheStr[1]);

                    if(ContainerInfomanager.Instance.GetOwnMaterialCount(int.Parse(chacheStr[0]))<int.Parse(chacheStr[1]))
                    {
                        needItemStr=NGUIColor.SetTxtColor(needItemStr,TextColor.red);
                    }
                    else
                    {
                        needItemStr=NGUIColor.SetTxtColor(needItemStr,TextColor.white);
                    }
					
				}
			}
            if(itemData.UpgradeID!=0)
            {
                int targetLevel = ItemDataManager.Instance.GetItemData(itemData.UpgradeID)._AllowLevel;
             
                string Levelstr= string.Format("{0}{1}",targetLevel,LanguageTextManager.GetString("IDS_H1_156"));

                if(PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL<targetLevel)
                {
                    Levelstr= NGUIColor.SetTxtColor(Levelstr,TextColor.red);
                }
                else
                {
                    Levelstr= NGUIColor.SetTxtColor(Levelstr,TextColor.white);
                }
                LevelNeedLabel.SetText(Levelstr);
            }
			NeedItemDesLabel.SetText(needItemStr);
			HelpTipsLabel.SetText(LanguageTextManager.GetString("IDS_I10_4"));
            PriceLabel.SetText(normalItem._SaleCost+itemFielInfo.equipmentEntity.ITEM_FIELD_VISIBLE_COMM);
		}

		void ClearUpPanel()
		{
			Effect01UpArrow.gameObject.SetActive(false);
			Effect02UpArrow.gameObject.SetActive(false);
			Effect01NameLabel.SetText("");
			Effect02NameLabel.SetText("");
			Effect01NumLabel.SetText("");
			Effect02NumLabel.SetText("");
			Effect01AddLabel.SetText("");
			Effect02AddLabel.SetText("");
			NeedItemDesLabel.SetText("");
			LevelNeedLabel.SetText("");
			HelpTipsLabel.SetText("");
			PriceLabel.SetText("");
			FullLevelLabel.gameObject.SetActive(false);
			CanUpgradeObj.SetActive(true);
		}

		void ShowMainEffectLabel(EquipmentData normalItem,EquipmentData targetItem)
		{
			bool isFullLevel = targetItem == null;
			Effect01UpArrow.gameObject.SetActive(!isFullLevel);
			Effect02UpArrow.gameObject.SetActive(!isFullLevel);
			FullLevelLabel.gameObject.SetActive(isFullLevel);
			CanUpgradeObj.SetActive(!isFullLevel);
			EffectData targetEffect1data = null;
			EffectData targetEffect2data = null;
			EffectData normalEffect1data = null;
			EffectData normalEffect2data = null;
			int targetEffect1AddNum = 0;
			int targetEffect2AddNum = 0;
			int normalEffect1AddNum = 0;
			int normalEffect2AddNum = 0;
			GetEffectData(normalItem,out normalEffect1data,out normalEffect2data,out normalEffect1AddNum,out normalEffect2AddNum,true);
			if(targetItem != null)
			{
                GetEffectData(normalItem,out targetEffect1data,out targetEffect2data,out targetEffect1AddNum,out targetEffect2AddNum,false);
			}
			Effect01Icon.spriteName = normalEffect1data.EffectRes;
			Effect02Icon.spriteName = normalEffect2data.EffectRes;
			Effect01NameLabel.SetText(LanguageTextManager.GetString(normalEffect1data.IDS));
			Effect02NameLabel.SetText(LanguageTextManager.GetString(normalEffect2data.IDS));
			Effect01NumLabel.SetText(normalEffect1AddNum);
			Effect02NumLabel.SetText(normalEffect2AddNum);
			Effect01AddLabel.SetText(targetEffect1data==null?0: targetEffect1AddNum - normalEffect1AddNum);
			Effect02AddLabel.SetText(targetEffect2data==null?0:targetEffect2AddNum - normalEffect2AddNum);
		}

        void GetEffectData(EquipmentData equiptData,out EffectData effect1Data,out EffectData effect2Data,out int effect1AddNum,out int effect2AddNum,bool isBefore)
		{
			string[] effects = equiptData._vectEffects.Split('|');
			effect1Data = ItemDataManager.Instance.EffectDatas._effects.FirstOrDefault(C=>C.m_SzName == effects[0].Split('+')[0]);
			effect2Data = ItemDataManager.Instance.EffectDatas._effects.FirstOrDefault(C=>C.m_SzName == effects[1].Split('+')[0]);
            effect1AddNum = EquipMainProp(equiptData,m_EquipmentEntity,0,isBefore);
            effect2AddNum =EquipMainProp(equiptData,m_EquipmentEntity,1,isBefore);
		}
		
        /// <summary>
        /// 计算装备主属性值
        /// </summary>
        /// <param name="itemFielInfo">装备数据</param>
        /// <param name="index">装备主属性索引</param>
        /// <param name="isBefore">是否装备前的值</param>
        /// <param name="isNormal">是否普通强化</param>
        /// <returns></returns>
        private int EquipMainProp(ItemData itemData, EquipmentEntity equipmentEntity, int index, bool isBefore)
        {
            EquipmentData equipItemData;
            int normalStrengthenLv = equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL;
            int starStrengthenLv = equipmentEntity.EQUIP_FIELD_START_LEVEL;
            equipItemData= ItemDataManager.Instance.GetItemData(itemData._goodID) as EquipmentData;
            if (!isBefore)
            {
                equipItemData= ItemDataManager.Instance.GetItemData(equipItemData.UpgradeID) as EquipmentData;
            }
          
           
            StrengthParameter strengthParameter = equipItemData._StrengthParameter[index];
            StrengthParameter starStrengthParameter=equipItemData._StartStrengthParameter[index];

			int normalMainProAdd =normalStrengthenLv==0||normalStrengthenLv>CommonDefineManager.Instance.CommonDefine.StrengthLimit?0:strengthParameter.Value[normalStrengthenLv-1];
            
			int normalMainProAddForStar =starStrengthenLv==0||starStrengthenLv>CommonDefineManager.Instance.CommonDefine.StartStrengthLimit?0:starStrengthParameter.Value[starStrengthenLv-1];
     
            int sourceMainProValue = 0;

            string[] effects = equipItemData._vectEffects.Split('|');
            sourceMainProValue =int.Parse(effects[index].Split('+')[1]);
            return sourceMainProValue + normalMainProAdd+normalMainProAddForStar;
        }
		/// <summary>
		/// 字体转为淡黄色
		/// </summary>
		string GetEffectYellowText(string text)
		{
			return string.Format("[ece09e]{0}[-]",text);
		}

	}
}