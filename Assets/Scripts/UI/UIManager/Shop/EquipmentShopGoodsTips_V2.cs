using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class EquipmentShopGoodsTips_V2 : BaseShopContainerTips_V2
    {

        public ShopGoodsEffect Effect1, Effect2;

        public UILabel LevelNeed;

        public override void Show(ShopSingleGoodsBox_V2 SelectGoods,ShopInfoUIManager_V2 MyParent)
        {
            LevelNeed.SetText(string.Format("{0}{1}", SelectGoods.LocalGoodsData._AllowLevel, LanguageTextManager.GetString("IDS_H1_156")));
            int currentLv = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            LevelNeed.color = currentLv >= SelectGoods.LocalGoodsData._AllowLevel ? Color.green : Color.red;
            base.Show(SelectGoods,MyParent);
            Effect1.ShowEffect(SelectGoods.LocalGoodsData as EquipmentData,0);
            Effect2.ShowEffect(SelectGoods.LocalGoodsData as EquipmentData,1);
        }

    }
}