using UnityEngine;
using System.Collections;

namespace UI.MainUI
{
    public class SingleTopUpCard : MonoBehaviour
    {
		//全价，打折
		public GameObject DiscountTip;
		//vip界面使用
        private UILabel DiscountLabel;
		//快速购买
		private SpriteSwith quickBuyLabel;
        //public UISprite GoldRechargeIcon;
		//商品数量
        public UILabel GoldNumberLabel;
		public SpriteSwith MoneyIcon;
		//商品价格
        public UILabel PayMoneyLabel;
        public GameObject PayIconParent;
		private GameObject payIcon;
        //public SpriteSwith BackgroundSprite;
		[HideInInspector]
		public int shopIndex;
		private bool isVipPanel = false;
		[HideInInspector]
		public int PositionX;
		[HideInInspector]
		public int PositionY;
        public GoldRechargeData goldRechargeData { get; private set; }
		public TopUpPanel_V2 MyVipParent { get; private set; }
		private QuickBuy myQuickBuyParent;
        public bool IsSelect { get; private set; }
        bool IsPlaySound = false;
		public SpriteSwith spriteMoney;
		public void Init(bool isVipMark,TopUpPanel_V2 vipParent,QuickBuy myQuickParent,int index)
        {
			shopIndex = index;//int.Parse(transform.parent.name.Substring (9,1));
			isVipPanel = isVipMark;
			if (isVipPanel) {
				DiscountLabel = DiscountTip.GetComponent<UILabel>();
				MyVipParent = vipParent;	
			} else {
				myQuickBuyParent = myQuickParent;
				quickBuyLabel = DiscountTip.GetComponent<SpriteSwith>();
			}
            IsSelect = false;
			if(MoneyIcon != null)
			{
#if (ANDROID_TENCENT && !UNITY_EDITOR)
			MoneyIcon.ChangeSprite(2);
#else
			MoneyIcon.ChangeSprite(1);
#endif
			}
        }
		public void ShowVip(GoldRechargeData goldRechargeData)
		{
			gameObject.SetActive(true);
			this.goldRechargeData = goldRechargeData;
			//this.DiscountLabel.gameObject.SetActive(goldRechargeData.Discount < 10 ? true : false);
			if( goldRechargeData.Discount <= 0 )
			{
				this.DiscountLabel.text = LanguageTextManager.GetString("IDS_I4_20");// (string.Format(LanguageTextManager.GetString("IDS_H1_454"), goldRechargeData.Discount.ToString()));
			}
			else
			{
				this.DiscountLabel.text = string.Format(LanguageTextManager.GetString("IDS_I4_19"),goldRechargeData.Discount);
			}
			SetShowView (goldRechargeData.GoldNumber,goldRechargeData.CurrencyNumber,goldRechargeData.goldPicturePrefab);
		}
		public void ShowQuickTip(ShopConfigData shopData)
		{
			quickBuyLabel.ChangeSprite (shopIndex);
			int changeIndex = 1;
			if (shopData.BuyType == 1) {
				changeIndex = 1;
			} else if (shopData.BuyType == 3) {
				changeIndex = 2;			
			}
			if(spriteMoney != null)
				spriteMoney.ChangeSprite (changeIndex);
			SetShowView (shopData._goodsNum,shopData.Price,shopData.goodsPicturePrefab);
		}
		//钱数还float？？
		void SetShowView(int goodCount,float moneyCount,GameObject goodIconPrefab)
		{
			this.GoldNumberLabel.SetText(string.Format("x{0}", goodCount));
			/*if(isVipPanel)
				this.GoldNumberLabel.SetText(string.Format("x{0}", goodCount));
			else
				this.GoldNumberLabel.text = goodCount.ToString();*/
			this.PayMoneyLabel.SetText(moneyCount.ToString());
			if (payIcon == null) {
				payIcon = UI.CreatObjectToNGUI.InstantiateObj(goodIconPrefab,PayIconParent.transform);			
			}

		}
        void OnClick()
        {
            //SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
			if (isVipPanel) {
				MyVipParent.OnTopUpCardSelect (this);	
			} else {
				myQuickBuyParent.OnTopUpCardSelect(this);
			}
        }
		private AudioSource audioSource;
		public void OnTopUpCardSelect()//SingleTopUpCard selectCard)
		{
			if (isVipPanel) {
				IsSelect = true;// == selectCard;
				if (this.goldRechargeData != null) {
					/*//jamfing
					 * if (IsPlaySound) {
						SoundManager.Instance.StopSoundEffect (this.goldRechargeData.TouchVoice);
						IsPlaySound = false;
					}*/
					if (IsSelect) {
						if(audioSource != null)
						{
							if(audioSource.isPlaying)
							{
								audioSource.Stop();
							}
						}
						audioSource = SoundManager.Instance.PlaySoundEffect (this.goldRechargeData.TouchVoice);
						IsPlaySound = true;
						DoForTime.DoFunForTime (3, disabelSoundForTime, null);
					}
				}
			}
			//BackgroundSprite.ChangeSprite(IsSelect ? 2 : 1);
		}

        void disabelSoundForTime(object obj)
        {
            IsPlaySound = false; 
        }

        public void Close()
        {
            gameObject.SetActive(false);
            IsSelect = false;
            //BackgroundSprite.ChangeSprite(1);
        }

    }
}