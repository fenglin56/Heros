using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class OtherShopGoodsTips_V2 : BaseShopContainerTips_V2
    {

        public UILabel DesLabel;
        public UILabel LevelNeedLabel;
        public UILabel OverlayLabel;

        public override void Show(ShopSingleGoodsBox_V2 SelectGoods, ShopInfoUIManager_V2 MyParent)
        {
            this.DesLabel.SetText(LanguageTextManager.GetString(SelectGoods.LocalGoodsData._szDesc));
            this.LevelNeedLabel.SetText(SelectGoods.LocalGoodsData._AllowLevel);
            int currentLv = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            LevelNeedLabel.color = currentLv >= SelectGoods.LocalGoodsData._AllowLevel ? Color.green : Color.red;
            this.OverlayLabel.SetText(SelectGoods.LocalGoodsData._PileQty);
            base.Show(SelectGoods, MyParent);
        }

    }
}