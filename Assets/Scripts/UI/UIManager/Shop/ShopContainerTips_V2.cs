using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class ShopContainerTips_V2 : MonoBehaviour
    {

        public GameObject ShopContaienrTipsPrefab_Equipment;
        public GameObject ShopContaienrTipsPrefab_Other;

        private EquipmentShopGoodsTips_V2 shopContainerTips_EquipMent;
        private OtherShopGoodsTips_V2 shopContainerTips_Other;

        public void ShowGoodsInfo(ShopSingleGoodsBox_V2 SelectGoods, ShopInfoUIManager_V2 MyParent)
        {
            switch (SelectGoods.LocalGoodsData._GoodsClass)
            {
                case 1:
                    if (shopContainerTips_EquipMent == null)
                    {
                        shopContainerTips_EquipMent = CreatObjectToNGUI.InstantiateObj(ShopContaienrTipsPrefab_Equipment, transform).GetComponent<EquipmentShopGoodsTips_V2>();
                    }
                    if (shopContainerTips_Other != null)
                    {
                        shopContainerTips_Other.Close();
                    }
                    shopContainerTips_EquipMent.Show(SelectGoods, MyParent);
                    break;
                default:
                    if (shopContainerTips_Other == null)
                    {
                        shopContainerTips_Other = CreatObjectToNGUI.InstantiateObj(ShopContaienrTipsPrefab_Other, transform).GetComponent<OtherShopGoodsTips_V2>();
                    }
                    if (shopContainerTips_EquipMent != null)
                    {
                        shopContainerTips_EquipMent.Close();
                    }
                    shopContainerTips_Other.Show(SelectGoods, MyParent);
                    break;
            }
        }

        public void ShowCostTips(long costMoney)
        {
            if (shopContainerTips_EquipMent != null)
            {
                shopContainerTips_EquipMent.ShowCostTips(costMoney);
            }
            if (shopContainerTips_Other != null)
            {
                shopContainerTips_Other.ShowCostTips(costMoney);
            }
        }

    }
}