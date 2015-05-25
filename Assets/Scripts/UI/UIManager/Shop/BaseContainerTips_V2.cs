using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class BaseShopContainerTips_V2 : MonoBehaviour
    {
        public GameObject CostTipsPrefab;
        public SingleButtonCallBack BuyBtn;
        public SpriteSwith CostTypeICon;
        public UILabel CostNumber;

        public ShopSingleGoodsBox_V2 GoodsTitle;

        ShopSingleGoodsBox_V2 SelectGoods;
        ShopInfoUIManager_V2 MyParent;

        private bool IsShow = false;

        private int m_guideBtnID = 0;

        void Awake()
        {
            BuyBtn.SetCallBackFuntion(OnBuyBtnClick);
        }

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(BuyBtn.gameObject, UIType.Shop, SubType.ShopInfoGoodInfo, out m_guideBtnID);
        }

        public virtual void Show(ShopSingleGoodsBox_V2 SelectGoods,ShopInfoUIManager_V2 MyParent)
        {
            IsShow = true;
            this.MyParent = MyParent;
            this.SelectGoods = SelectGoods;
            this.GoodsTitle.Init(SelectGoods.sMsgTradeOpenShopGoodsInfo_SC,null);
            this.CostTypeICon.ChangeSprite(SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.bType);
            this.CostNumber.SetText(SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.dPrice.ToString());
            CostNumber.color = CheckCanPay() ? Color.white : Color.red;
            transform.localPosition = Vector3.zero;
        }

        bool CheckCanPay()
        {
            bool flag = false;
            switch (SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.bType)
            {
                case 1:
                    flag = SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.dPrice <= PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
                    break;
                default:
                    flag = SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.dPrice <= PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
                    break;
            }
            return flag;
        }

        void OnBuyBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            if (!GetPayFlag())
            {
                
                bool IsGoldBuy = this.SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.bType == 1 ? false : true;
                MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString(IsGoldBuy ? "IDS_H2_44" : "IDS_H2_35"),1);
            }
            else
            {
                this.MyParent.BuyGoods(this.SelectGoods.sMsgTradeOpenShopGoodsInfo_SC);
            }
        }

        bool GetPayFlag()
        {
            bool CanPay = false;
            var PlayerData = PlayerManager.Instance.FindHeroDataModel().PlayerValues;
            switch (this.SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.bType)
            {
                case 1:
                    if (this.SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.dPrice <= PlayerData.PLAYER_FIELD_HOLDMONEY)
                    {
                        CanPay = true;
                    }
                    break;
                case 2:
                    if (this.SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.dPrice <= PlayerData.PLAYER_FIELD_CURRENCY_COWRY)
                    {
                        CanPay = true;
                    }
                    break;
                case 3:
                    if (this.SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.dPrice <= PlayerData.PLAYER_FIELD_BINDPAY)
                    {
                        CanPay = true;
                    }
                    break;
                default:
                    break;
            }
            return CanPay;
        }
        public void Close()
        {
            IsShow = false;
            transform.localPosition = new Vector3(0,0,-1000);
        }

        public void ShowCostTips(long costMoney)
        {
            if (!IsShow)
                return;
            SingleButtonCallBack Tips = CreatObjectToNGUI.InstantiateObj(CostTipsPrefab,BuyBtn.transform).GetComponent<SingleButtonCallBack>();
            Vector3 fromPoint = new Vector3(0, 50, -30);
            Vector3 toPoint = new Vector3(0, 0, -30);
            TweenPosition.Begin(Tips.gameObject, 0.5f, fromPoint, toPoint, null);
            TweenAlpha.Begin(Tips.gameObject, 0.5f, 1, 0, DestroyObj);
            Tips.SetButtonBackground(this.SelectGoods.sMsgTradeOpenShopGoodsInfo_SC.bType);
            Tips.SetButtonText(string.Format("-{0}", costMoney));
        }

        void DestroyObj(object obj)
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
            Destroy(obj as GameObject);
        }

    }
}