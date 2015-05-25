using UnityEngine;
using System.Collections;

namespace UI.Battle
{
    public enum TreasureChestsType
    {
        Normal = 0,
        CostMoney,
        VIP
    }

    public class SingleTreasureChests_V4 : MonoBehaviour
    {


        public GameObject TreasureChestCloseStatusOBj;
        public GameObject TreasureChestOpenStatusOBj;
        //public SpriteSwith CostIcon;
        public SingleButtonCallBack CostLabel;
        public Transform CreatTreasureChestsItemPoint;
        public UILabel TreasureChestsItemNameLabel;
        public GameObject NormalBoxEffect;
        public GameObject VIPBoxEffect;
        public GameObject OpenTreasureChestEffect;
        public Transform CreatTreasureChestEffectPoint;
        int CostMoneyType = 0;
        int CostMoney = 0;
        [HideInInspector]
        public bool
            IsTreasureChestsOpened = false;
        bool IsHero = false;

        public SingleRewardPanel_V4 MyParent{ get; private set; }

        public TreasureChestsType MyChestsType{ get; private set; }

        public EctypeContainerData EctypeData{ get; private set; }

        public void Init(TreasureChestsType chestsType, SingleRewardPanel_V4 myParent, bool isHero)
        {
            SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            EctypeData = EctypeConfigManager.Instance.EctypeContainerConfigList [sMSGEctypeInitialize_SC.dwEctypeContainerId];
            IsHero = isHero;
            MyParent = myParent;
            MyChestsType = chestsType;
            string costFormat = LanguageTextManager.GetString("IDS_I4_21");
            string costText = string.Format(costFormat, EctypeData.ByCost.ToString());
            bool canPay = false;
            switch (EctypeData.ByCostType)
            {
                case 0://元宝
                    int PayGoldMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
                    canPay = PayGoldMoney >= EctypeData.ByCost;
                    break;
                case 1://铜币
                    int PayCopperMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
                    canPay = PayCopperMoney >= EctypeData.ByCost;
                    break;
            }


            switch (chestsType)
            {
                case TreasureChestsType.Normal:
                    CreatTreasureChestEffectPoint.ClearChild();
                    //CreatTreasureChestEffectPoint.InstantiateNGUIObj(NormalBoxEffect);
                    CostLabel.gameObject.SetActive(false);
                    break;
                case TreasureChestsType.CostMoney:
                    CostLabel.gameObject.SetActive(isHero);
                    CostLabel.spriteSwith.ChangeSprite(2 - EctypeData.ByCostType);
					//CreatTreasureChestEffectPoint.InstantiateNGUIObj(NormalBoxEffect);
                    CostLabel.SetButtonText(costText);
                    if(canPay)
                    {
                        CostLabel.SetTextColor(Color.white);
                    }
                    else
                    {
                        CostLabel.SetTextColor(Color.red);
                    }
                    
                    break;
                case TreasureChestsType.VIP:
                    CreatTreasureChestEffectPoint.ClearChild();
					if(isHero &&PlayerDataManager.Instance.GetPlayerVIPLevel() < 5)
					{
						VIPBoxEffect.SetActive(true);
					}
					else
					{
						VIPBoxEffect.SetActive(false);    
					}
//					if(isHero && PlayerDataManager.Instance.GetMainEctypeRewardTimes() < 5)
//                    {						                  
//                        //vipLabel.SetText(string.Format(LanguageTextManager.GetString("IDS_I21_2"), PlayerDataManager.Instance.GetMainEctypeVIPRewardMinLevel()));
//                    }
//					else
//					{
//						VIPBoxEffect.SetActive(false);
//						//CreatTreasureChestEffectPoint.InstantiateNGUIObj(NormalBoxEffect);
//					}
                    CostLabel.gameObject.SetActive(isHero);
                    CostLabel.spriteSwith.ChangeSprite(2 - EctypeData.ByCostType);
                    CostLabel.SetButtonText(costText);
                    if(canPay)
                    {
                        CostLabel.SetTextColor(Color.white);
                    }
                    else
                    {
                        CostLabel.SetTextColor(Color.red);
                    }
                    break;
            }
        }

        public void Open(int itemID, int itemNum)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_AppraisalGetAward");
            IsTreasureChestsOpened = true;
            StartCoroutine(TweenOpenTreasureChests(itemID, itemNum));
        }
        
        IEnumerator TweenOpenTreasureChests(int itemID, int itemNum)
        {
            CreatTreasureChestEffectPoint.ClearChild();
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_ChooseAwardGoldenCard");
            GameObject effectOBj = CreatObjectToNGUI.InstantiateObj(OpenTreasureChestEffect, CreatTreasureChestEffectPoint);
            yield return new WaitForSeconds(1.3f);
            TreasureChestCloseStatusOBj.SetActive(false);
            TreasureChestOpenStatusOBj.SetActive(true);
            ItemData creatData = ItemDataManager.Instance.GetItemData(itemID);
            CreatObjectToNGUI.InstantiateObj(creatData._picPrefab, CreatTreasureChestsItemPoint);
            DoForTime.DoFunForTime(0.5f, TweenShowTreasureItemIcon, null);
            SetTreasureItemNameLabel(creatData, itemNum);
        }

        void SetTreasureItemNameLabel(ItemData itemData, int number)
        {
            TextColor NameTextColor = TextColor.white;
            switch (itemData._ColorLevel)//物品品质颜色
            {
                case 0:
                    NameTextColor = TextColor.EquipmentGreen;
                    break;
                case 1:
                    NameTextColor = TextColor.EquipmentBlue;
                    break;
                case 2:
                    NameTextColor = TextColor.EquipmentMagenta;
                    break;
                case 3:
                    NameTextColor = TextColor.EquipmentYellow;
                    break;
                default:
                    break;
            }
            string ItemName = LanguageTextManager.GetString(itemData._szGoodsName);
            string format = LanguageTextManager.GetString("IDS_I21_10");
            TreasureChestsItemNameLabel.SetText(NGUIColor.SetTxtColor(string.Format(format, ItemName, number), NameTextColor));
        }

        void TweenShowTreasureItemIcon(object obj)
        {
            Vector3 fromSclae = new Vector3(2, 2, 2);
            Vector3 toSclae = new Vector3(0.8f, 0.8f, 0.8f);
			if(CreatTreasureChestsItemPoint!=null)
			{
				TweenScale.Begin(CreatTreasureChestsItemPoint.gameObject, 0.3f, fromSclae, toSclae, null);
			}            
        }

        void OnClick()
        {

            if (!IsHero || IsTreasureChestsOpened)
            {
                return;
            }
            SoundManager.Instance.PlaySoundEffect("Sound_Button_AppraisalChooseAward");
            switch (MyChestsType)
            {
                case TreasureChestsType.Normal:
                    SendOpenTreasureChestsToSever(TreasureChestsType.Normal);
                    break;
                case TreasureChestsType.CostMoney:
                    bool canPay = false;
                    switch (EctypeData.ByCostType)
                    {
                        case 0://元宝
                            int PayGoldMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
                            canPay = PayGoldMoney >= EctypeData.ByCost;
                            if (!canPay)
                            {
                                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I21_4"), 1);
                            }
                            break;
                        case 1://铜币
                            int PayCopperMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
                            canPay = PayCopperMoney >= EctypeData.ByCost;
                            if (!canPay)
                            {
                                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I21_3"), 1);
                            }
                            break;
                    }
                    if (canPay)
                    {
                        SendOpenTreasureChestsToSever(TreasureChestsType.CostMoney);
                    }
                    break;
                case TreasureChestsType.VIP:
                    if (PlayerDataManager.Instance.GetMainEctypeRewardTimes() >= 3)
                    {
                        bool vipCanPay = false;
                        switch (EctypeData.ByCostType)
                        {
                            case 0://元宝
                                int PayGoldMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
                                vipCanPay = PayGoldMoney >= EctypeData.ByCost;
                                if (!vipCanPay)
                                {
                                    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I21_4"), 1);
                                }
                                break;
                            case 1://铜币
                                int PayCopperMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
                                vipCanPay = PayCopperMoney >= EctypeData.ByCost;
                                if (!vipCanPay)
                                {
                                    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I21_3"), 1);
                                }
                                break;
                        }
                        if (vipCanPay)
                        {
                            SendOpenTreasureChestsToSever(TreasureChestsType.VIP);
                        }
                    } 
                    else
                    {
                        string format  = LanguageTextManager.GetString("IDS_I21_6");
                        string msg = string.Format(format, PlayerDataManager.Instance.GetMainEctypeVIPRewardMinLevel());
                        MessageBox.Instance.ShowTips(3, msg, 1);
                    }
                    break;
            }
        }

        void SendOpenTreasureChestsToSever(TreasureChestsType type)
        {
//          if (UI.MainUI.ContainerInfomanager.Instance.GetEmptyPackBoxNumber() < 1)
//          {
//              MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I21_5"), 1);//背包已满,通过邮件发送
//          }
//          else
//          {
            TraceUtil.Log(SystemModel.Jiang, "发送打开宝箱请求");
			if(type ==  TreasureChestsType.Normal)
			{
				MyParent.MyParent.ShowFreeTreasureReward();
			}
			else
			{
				NetServiceManager.Instance.EctypeService.SendSMSGEctypeClickTreasure_CS((byte)type);
			}           
//          }
        }

    }
}