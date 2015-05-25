using UnityEngine;
using System.Collections;


namespace UI.MainUI
{
    /// <summary>
    /// 不可使用类
    /// </summary>
    public class ContainerTips_Materiel : BaseContainerTips_V2
    {
        public UILabel ItemDesLabel;
        //public UILabel levelTitleLabel;
        //public UILabel Levellabel;
        public UILabel OverlayNumberLabel;
        public UILabel PriceLabel;


        public SingleButtonCallBack SellBtn;
        public SingleButtonCallBack CloseBtn;
        //public SingleButtonCallBack CancleBtn;

        void Start()
        {;
            SellBtn.SetCallBackFuntion(OnSellBtnClick);
            CloseBtn.SetCallBackFuntion(OnCloseBtnClick);
            //CancleBtn.SetCallBackFuntion(OnCloseBtnClick);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.OnRoleViewClick,OnCloseBtnClick);

            base.RegBtn(SellBtn.gameObject, CloseBtn.gameObject);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnRoleViewClick, OnCloseBtnClick);
            base.UnRegBtn();
        }

        public override void Show(ItemFielInfo itemFielInfo)
        {
            ItemDesLabel.SetText(LanguageTextManager.GetString(itemFielInfo.LocalItemData._szDesc));
            //Levellabel.SetText(itemFielInfo.LocalItemData._AllowLevel + LanguageTextManager.GetString("IDS_H1_156"));
            //int currentLv = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            //Levellabel.color = currentLv > itemFielInfo.LocalItemData._AllowLevel ? Color.white : Color.red;
            OverlayNumberLabel.SetText(itemFielInfo.LocalItemData._PileQty);
            PriceLabel.SetText(itemFielInfo.LocalItemData._SaleCost);
            base.Show(itemFielInfo);
        }

        void OnSellBtnClick(object obj)
        {
            //Close();
            base.DiscardItems();
        }

        void OnCloseBtnClick(object obj)
        {
            Close();
        }
    }
}