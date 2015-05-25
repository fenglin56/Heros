using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class ShopSelectGoodsNumberPanel : MonoBehaviour
    {
        ShopInfoUIManager_V2 MyParent;
        SMsgTradeOpenShopGoodsInfo_SC sMsgTradeOpenShopGoodsInfo_SC;

        public SingleButtonCallBack AddBtn;
        public SingleButtonCallBack CutBtn;
        public SingleButtonCallBack SureBtn;
        public SingleButtonCallBack CancelBtn;
        public ShopSingleGoodsBox_V2 ShopSingleGoodsBox; 
        public UIScrollBar SelectNumberScoreBar;

        public UILabel TitleLabel;

        public UIInput InputLabel;

        private int m_GoldMoney = 0;
        private int m_CopperCoin = 0;
        private int m_BindMoney = 0;
        private int BuyNumber = 1;

        private int[] m_guideBtnID = new int[7];

        void Awake()
        {
            AddBtn.SetPressCallBack(OnAddBtnPress);
            AddBtn.SetCallBackFuntion(OnAddBtnClick);
            CutBtn.SetPressCallBack(OnCutBtnPress);
            CutBtn.SetCallBackFuntion(OnCutBtnClick);
            SureBtn.SetCallBackFuntion(OnSureButtonClick);
            CancelBtn.SetCallBackFuntion(OnCancelButtonClick);

            SureBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_55"));
            CancelBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_28"));
            ClosePanel();
        }

        void Start()
        {
            SelectNumberScoreBar.onChange = OnSelectNumberScorllBarDrag;
            //TODO GuideBtnManager.Instance.RegGuideButton(AddBtn.gameObject, UIType.Shop, SubType.ShopInfoBuyTips, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(CutBtn.gameObject, UIType.Shop, SubType.ShopInfoBuyTips, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(SureBtn.gameObject, UIType.Shop, SubType.ShopInfoBuyTips, out m_guideBtnID[2]);
            //TODO GuideBtnManager.Instance.RegGuideButton(CancelBtn.gameObject, UIType.Shop, SubType.ShopInfoBuyTips, out m_guideBtnID[3]);
            //物品数量选择拖动条的引导管理
            //TODO GuideBtnManager.Instance.RegGuideButton(SelectNumberScoreBar.transform.GetChild(0).gameObject, UIType.Shop, SubType.ShopInfoBuyTips, out m_guideBtnID[4]);
            //TODO GuideBtnManager.Instance.RegGuideButton(SelectNumberScoreBar.transform.GetChild(1).gameObject, UIType.Shop, SubType.ShopInfoBuyTips, out m_guideBtnID[5]);
            //TODO GuideBtnManager.Instance.RegGuideButton(InputLabel.gameObject, UIType.Shop, SubType.ShopInfoBuyTips, out m_guideBtnID[6]);
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        public void Show(SMsgTradeOpenShopGoodsInfo_SC sMsgTradeOpenShopGoodsInfo_SC, ShopInfoUIManager_V2 MyParent)
        {
            SelectNumberScoreBar.scrollValue = 1f/99f;
            InputLabel.text = "";
            this.ShopSingleGoodsBox.Init(sMsgTradeOpenShopGoodsInfo_SC, null);
            BuyNumber = 1;
            this.sMsgTradeOpenShopGoodsInfo_SC = sMsgTradeOpenShopGoodsInfo_SC;
            this.MyParent = MyParent;
            var PlayerData = PlayerManager.Instance.FindHeroDataModel().PlayerValues;
            this.m_BindMoney = PlayerData.PLAYER_FIELD_CURRENCY_COWRY;
            this.m_GoldMoney = PlayerData.PLAYER_FIELD_BINDPAY;
            this.m_CopperCoin = PlayerData.PLAYER_FIELD_HOLDMONEY;
            transform.localPosition = new Vector3(0, 0, -200);
        }

        void OnAddBtnPress(bool IsPress)
        {
            if (IsPress)
            {
                StartCoroutine(AddNumber());
            }
            else
            {
                StopAllCoroutines();
            }
        }

        void OnCutBtnPress(bool IsPress)
        {
            if (IsPress)
            {
                StartCoroutine(CutNumber());
            }
            else
            {
                StopAllCoroutines();
            }
        }

        void OnAddBtnClick(object obj)
        {
            BuyNumber++;
            CheckNumber();
        }

        void OnCutBtnClick(object obj)
        {
            BuyNumber--;
            CheckNumber();
        }

        void OnSureButtonClick(object obj)
        {
            if (InputLabel.text.Length > 0)
            {
                BuyNumber = int.Parse(InputLabel.text);
            }
            if (BuyNumber == 0)
            {
                return;
            }
            int PileQtyNumber = ItemDataManager.Instance.GetItemData((int)this.sMsgTradeOpenShopGoodsInfo_SC.dGoodsID)._PileQty;
            if ((BuyNumber / PileQtyNumber) > (GetEmptyContainerBoxNumber()))
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_2"), 1);//背包已满
                return;
            }
            else
            {
                long costMoney = BuyNumber * this.sMsgTradeOpenShopGoodsInfo_SC.dPrice;
                if (GetPayPrice() < (BuyNumber * this.sMsgTradeOpenShopGoodsInfo_SC.dPrice))//钱不足
                {
                    bool IsGoldBuy = this.sMsgTradeOpenShopGoodsInfo_SC.bType == 1 ? false : true;
                    if (!IsGoldBuy)
                    {
                        MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_35"), 1);
                    }
                    else
                    {
                        MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_7"), LanguageTextManager.GetString("IDS_H2_57"), LanguageTextManager.GetString("IDS_H2_58"), MyParent.ShowTopUpPanel, null);
                    }
                }
                else
                {

                    MyParent.shopContainerTips.ShowCostTips(costMoney);
                    SendBuyGoodsToServer((uint)BuyNumber);
                    ClosePanel();
                }
            }
        }

        void OnCancelButtonClick(object obj)
        {
            ClosePanel();
        }

        IEnumerator AddNumber()
        {
            yield return new WaitForSeconds(0.25f);
            BuyNumber++;
            CheckNumber();
            StartCoroutine(AddNumber());
        }

        IEnumerator CutNumber()
        {
            yield return new WaitForSeconds(0.25f);
            BuyNumber--;
            CheckNumber();
            StartCoroutine(CutNumber());
        }

        void SendBuyGoodsToServer(uint BuyNumber)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Pay");
            var ShopInfo = MyParent.sMsgTradeOpenShop_SC;
            var GoodsInfo = this.sMsgTradeOpenShopGoodsInfo_SC;
            SMsgTradeBuyGoods_CS sMsgTradeBuyGoods_CS = new SMsgTradeBuyGoods_CS()
            {
                dwShopID = ShopInfo.dwShopID,
                uidNPC = ShopInfo.uidNPC,
                lShopGoodsID = GoodsInfo.dShopGoodsID,
                GoodsID = GoodsInfo.dGoodsID,
                GoodsNum = BuyNumber,
            };
            NetServiceManager.Instance.TradeService.SendTradeBuyGoods(sMsgTradeBuyGoods_CS);
            //print("购买物品：" + GoodsInfo.dGoodsID + ",Number:" + BuyNumber + ",UID:" + ShopInfo.uidNPC);
        }

        void CheckNumber()
        {
            if (BuyNumber >= 100)
            {
                BuyNumber = 99;
            }
            if (BuyNumber < 1)
            {
                BuyNumber = 1;
            }
            //int m_PayPrice = GetPayPrice();
            //if (m_PayPrice < BuyNumber * this.sMsgTradeOpenShopGoodsInfo_SC.dPrice)//金钱
            //{
            //    var number = m_PayPrice / this.sMsgTradeOpenShopGoodsInfo_SC.dPrice;
            //    BuyNumber = (int)number;
            //}

            //int PileQtyNumber = ItemDataManager.Instance.GetItemData((int)this.sMsgTradeOpenShopGoodsInfo_SC.dGoodsID)._PileQty;//叠加数
            //if ((BuyNumber / PileQtyNumber) > GetEmptyContainerBoxNumber())
            //{
            //    BuyNumber--;
            //}
            this.InputLabel.text = BuyNumber.ToString();
        }

        public void ClosePanel()
        {
            StopAllCoroutines();
            transform.localPosition = new Vector3(0, 0, -1000);
        }

        int GetPayPrice()
        {
            int m_PayPrice = 0;
            switch (this.sMsgTradeOpenShopGoodsInfo_SC.bType)
            {
                case 1:
                    m_PayPrice = m_CopperCoin;
                    break;
                case 2:
                    m_PayPrice = m_BindMoney;
                    break;
                case 3:
                    m_PayPrice = m_GoldMoney;
                    break;
                default:
                    break;
            }
            return m_PayPrice;
        }


        void LateUpdate()
        {
            if (InputLabel.text.Length == 0)
            {
                //if (TitleLabel.text.Length == 0)
                //{
                //    TitleLabel.text = LanguageTextManager.GetString("IDS_H1_206");
                //}
                return;
            }
            //if (TitleLabel.text.Length != 0)
            //{
            //    TitleLabel.text = "";
            //}
            if (InputLabel.text.IndexOf("-") != -1)
            {
                InputLabel.text = InputLabel.text.Substring(1);
            }
            BuyNumber = int.Parse(InputLabel.text);
            UpdateSelectNumberScorllBar();
        }

        int GetEmptyContainerBoxNumber()
        {
            var ContainerSize = UI.MainUI.ContainerInfomanager.Instance.GetContainerClientContsext(2);
            var ItemContainerGood = UI.MainUI.ContainerInfomanager.Instance.sSyncContainerGoods_SCs;
            int ItemCount = ItemContainerGood.FindAll((SSyncContainerGoods_SC P) => { return P.uidGoods > 0; }).Count;
            return ContainerSize.wMaxSize - ItemCount;
        }

        void OnSelectNumberScorllBarDrag(UIScrollBar dragScorllBar)
        {
            //TraceUtil.Log("SetSliderValue："+dragScorllBar.scrollValue);
            BuyNumber = (int)(1 + dragScorllBar.scrollValue * 98);
            InputLabel.text = BuyNumber.ToString();
            //TraceUtil.Log("BuyNumber：" + BuyNumber);
            CheckNumber();
        }

        void UpdateSelectNumberScorllBar()
        {
            SelectNumberScoreBar.scrollValue = (float)BuyNumber / 99f;
        }
    }
}