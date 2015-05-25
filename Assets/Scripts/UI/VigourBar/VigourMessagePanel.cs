using UnityEngine;
using System.Collections;

namespace UI
{

    public class VigourMessagePanel : MonoBehaviour
    {

        public SingleButtonCallBack Button_Buy;
        public SingleButtonCallBack Button_Cancel;
        public UILabel MainMsgLabel;

        public UILabel TipsTitle_des;
        public UILabel TipsBuyLable;
        public UILabel TipsRecoverLable;

       // public GameObject Button_BuyIconObject;

        private int AddVigour = 20;
        private int TackMoney = 20;

        void Awake()
        {
            AddVigour = CommonDefineManager.Instance.CommonDefine.EnergyAdd;
            TackMoney = CommonDefineManager.Instance.CommonDefine.EnergyPay;
            TipsTitle_des.SetText(LanguageTextManager.GetString("IDS_I5_8"));
            Button_Buy.SetCallBackFuntion(OnBuyBtnClick);
            Button_Cancel.SetCallBackFuntion(OnCancelBtnClick);
			UIEventManager.Instance.RegisterUIEvent (UIEventType.CloseAllUI,OnCloseAllUIEvent);
         //   Button_Cancel.SetButtonText(LanguageTextManager.GetString("IDS_H2_28"));
        }

        public void Show(string ShowStr)
        {
            transform.localPosition = new Vector3(0,0,-200);
            //EnableBuyBtn();
			this.MainMsgLabel.text = string.Format(LanguageTextManager.GetString("IDS_I5_7"),EctypeModel.Instance.GetActiveEnergyHaveGold(),CommonDefineManager.Instance.CommonDefine.EnergyAdd);
            TipsBuyLable.text=string.Format( LanguageTextManager.GetString("IDS_I5_10"),PlayerDataManager.Instance.GetPlayerVIPLevel(), 
			                                PlayerDataManager.Instance.GetenergyPurchaseTimes(),
			                                (PlayerDataManager.Instance.GetenergyPurchaseTimes()-PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CANBUYACTIVE_NUM));
            TipsRecoverLable.text=LanguageTextManager.GetString("IDS_I5_9");
        }

        void OnBuyBtnClick(object obj)
        {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Pay");
			TackMoney = EctypeModel.Instance.GetActiveEnergyHaveGold ();
            LoadingUI.Instance.Close();
            if (PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY >= TackMoney)
            {
                if (PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CANBUYACTIVE_NUM <= 0)
                {
                    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_517"), 1);
                }
                else
                {
                    SendBuyVigourToServer();
                }
                ClosePanel();
            }
            else
            {
				//元宝不足，弹出提示
                //DisableBuyBtn();
				MessageBox.Instance.ShowNotEnoughGoldMoneyMsg(ClosePanel);
            }
        }

        void SendBuyVigourToServer()
        {
            int CurrentVigour = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
            int MaxVigour = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAX_ACTIVELIFE;
            if ((MaxVigour - CurrentVigour) < AddVigour)
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_244"), 1);
                return;
            }
            else
            {
                //TraceUtil.Log("购买元宝");
                NetServiceManager.Instance.TradeService.SendTradeQuickBuyGoods(50000001, (uint)AddVigour);
            }
        }

        void OnCancelBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            LoadingUI.Instance.Close();
            ClosePanel();
        }

        void EnableBuyBtn()
        {
            ///Button_BuyIconObject.SetActive(true);
           // Button_Buy.SetButtonText("");
           // BuyLable.text = LanguageTextManager.GetString("IDS_H2_11");
            //BuyMoneyLable.text = TackMoney.ToString();
            Button_Buy.SetButtonBackground(1);
            Button_Buy.SetButtonColliderActive(true);
        }

        void DisableBuyBtn()
        {
            //Button_BuyIconObject.SetActive(false);
            //Button_Buy.SetButtonText(LanguageTextManager.GetString("IDS_H2_44"));
            Button_Buy.SetButtonBackground(2);
            Button_Buy.SetButtonColliderActive(false);
        }
		void OnCloseAllUIEvent(object obj)
		{
			ClosePanel ();	
		}

        void ClosePanel()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel (UIEventType.CloseAllUI,OnCloseAllUIEvent);
		}
    }
}