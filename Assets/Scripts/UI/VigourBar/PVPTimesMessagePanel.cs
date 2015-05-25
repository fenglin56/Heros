using UnityEngine;
using System.Collections;

namespace UI
{

    public class PVPTimesMessagePanel : MonoBehaviour
    {

        public SingleButtonCallBack Button_Buy;
        public SingleButtonCallBack Button_Cancel;
        public UILabel MainMsgLabel;

        public UILabel BuyLable;
        public UILabel BuyMoneyLable;

        public GameObject Button_BuyIconObject;

        //\应该通过读表赋值
        private int AddVigour = 5;
        private int TackMoney = 100;

        void Start()
        {
            Button_Buy.SetCallBackFuntion(OnBuyBtnClick);
            Button_Cancel.SetCallBackFuntion(OnCancelBtnClick);
            Button_Cancel.SetButtonText(LanguageTextManager.GetString("IDS_H2_28"));
        }

        public void Show(string ShowStr)
        {
            transform.localPosition = new Vector3(0, 0, -200);
            EnableBuyBtn();
            this.MainMsgLabel.text = ShowStr;
        }

        void OnBuyBtnClick(object obj)
        {
            LoadingUI.Instance.Close();
            if (PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY > TackMoney)
            {
                SendBuyVigourToServer();
                ClosePanel();
            }
            else
            {
                DisableBuyBtn();
            }
        }

        void SendBuyVigourToServer()
        {
            //int CurrentVigour = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
            //int MaxVigour = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAX_ACTIVELIFE;
            //if ((MaxVigour - CurrentVigour) < AddVigour)
            //{
            //    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_244"), 1);
            //    return;
            //}
            //else
            //{
            //    //TraceUtil.Log("购买元宝");
                
            //}
            NetServiceManager.Instance.TradeService.SendTradeQuickBuyGoods(50000002, (uint)AddVigour);
        }

        void OnCancelBtnClick(object obj)
        {
            LoadingUI.Instance.Close();
            ClosePanel();
        }

        void EnableBuyBtn()
        {
            Button_BuyIconObject.SetActive(true);
            Button_Buy.SetButtonText("");
            BuyLable.text = LanguageTextManager.GetString("IDS_H2_11");
            BuyMoneyLable.text = TackMoney.ToString();
            Button_Buy.SetImageButtonComponentActive(true);
            Button_Buy.SetButtonBackground(1);
        }

        void DisableBuyBtn()
        {
            Button_BuyIconObject.SetActive(false);
            Button_Buy.SetButtonText(LanguageTextManager.GetString("IDS_H2_44"));
            Button_Buy.SetImageButtonComponentActive(false);
            Button_Buy.SetButtonBackground(2);
        }

        void ClosePanel()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }
    }
}