using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class ShopInfoUIManager_V2 : BaseUIPanel
    {

        public UISprite TitleLabel;

        public SMsgTradeOpenShop_SC sMsgTradeOpenShop_SC;
        public ShopConfigData shopConfigData;

        public ShopContainerTips_V2 shopContainerTips;
        public ShopSelectGoodsNumberPanel shopSelectGoodsNumberPanel;

        public GameObject unlockContainerBoxTipsPrefab;
        private UnlockContainerBoxTips unlockContainerBoxTips;

        public ShopGoodsListPanel_V2 shopGoodsListPanel;

        public GameObject CommonToolPrefab;

        public GameObject UIBottomBtnPrefab;
        public Transform CreatBottomBtnPoint;
        private CommonUIBottomButtonTool commonUIBottomButtonTool;

        private int m_guideBtnID = 0;

        void Awake()
        {
            if (UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform); }
        }

        void OnDestroy()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        void ShowBottomBtn()
        {
            CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonClick);
            commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() { btnInfo });

            var btnInfoComponent = commonUIBottomButtonTool.GetButtonComponent(btnInfo);
            //if (btnInfoComponent != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfoComponent.gameObject, UIType.Shop, SubType.ButtomCommon, out m_guideBtnID);
            
        }

        public override void Show(params object[] value)
        {
            this.sMsgTradeOpenShop_SC = (SMsgTradeOpenShop_SC)value[0];
            this.shopConfigData = ShopDataManager.Instance.shopConfigDataBase._dataTable.FirstOrDefault(P => P._shopID == this.sMsgTradeOpenShop_SC.dwShopID);
            this.TitleLabel.spriteName = this.shopConfigData._shopName.Split('/')[1];
            this.shopGoodsListPanel.ShowGoodsList(this.sMsgTradeOpenShop_SC, this);
            this.shopSelectGoodsNumberPanel.ClosePanel();
            transform.localPosition = Vector3.zero;
            if (commonUIBottomButtonTool == null)
            {
                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
                ShowBottomBtn();
            }
            else
            {
                commonUIBottomButtonTool.ShowAnim();
            }
            base.Show(value);
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CheckNewItem,null);
            base.Close();
        }

        void OnPayButtonClick(object obj)
        {
            ShowTopUpPanel();
        }

        void OnBackButtonClick(object obj)
        {
            CleanUpUIStatus();
            Close();
        }

        /// <summary>
        /// 购买物品
        /// </summary>
        public void BuyGoods(SMsgTradeOpenShopGoodsInfo_SC sMsgTradeOpenShopGoodsInfo_SC)
        {
            var ContainerSize = UI.MainUI.ContainerInfomanager.Instance.GetContainerClientContsext(2);
            var ItemContainerGood = UI.MainUI.ContainerInfomanager.Instance.sSyncContainerGoods_SCs;
			int ItemCount = ContainerInfomanager.Instance.GetPackItemList().Count;// ItemContainerGood.FindAll((SSyncContainerGoods_SC P) => { return P.uidGoods > 0; }).Count;
            if (ItemCount < 80)//判断背包是否已满
            {
                if (!ContainerInfomanager.Instance.PackIsFull())
                {
                    ShowSelectGoodsNumberPanel(sMsgTradeOpenShopGoodsInfo_SC);
                }
                else
                {
                    //弹出解锁背包提示框
                    if (unlockContainerBoxTips == null)
                    {
                        unlockContainerBoxTips = CreatObjectToNGUI.InstantiateObj(unlockContainerBoxTipsPrefab,transform).GetComponent<UnlockContainerBoxTips>();
                    }
                    unlockContainerBoxTips.Show(LanguageTextManager.GetString("IDS_H1_205"), ShowUnLockContainerMessageBox, null,
                        LanguageTextManager.GetString("IDS_H2_56"), LanguageTextManager.GetString("IDS_H2_28"));
                    //ShowUnLockContainerMessageBox();//解锁新背包
                }
            }
            else
            {
                MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H2_2"), LanguageTextManager.GetString("IDS_H2_55"), null);
            }
        }

        void ShowUnLockContainerMessageBox(object obj)
        {
            var PlayerData = PlayerManager.Instance.FindHeroDataModel().PlayerValues;
            if (PlayerData.PLAYER_FIELD_BINDPAY > 10)
            {
                SendUnLockContainerBoxToServer();
            }
            else
            {
                ShowTopUpPanel();
            }
        }

        void SendUnLockContainerBoxToServer()
        {
            NetServiceManager.Instance.ContainerService.SendContainerChangeSize(ContainerInfomanager.Instance.GetContainerClientContsext(2).dwContainerID);
        }

        void ShowSelectGoodsNumberPanel(SMsgTradeOpenShopGoodsInfo_SC sMsgTradeOpenShopGoodsInfo_SC)
        {
            this.shopSelectGoodsNumberPanel.Show(sMsgTradeOpenShopGoodsInfo_SC,this);
        }

        /// <summary>
        /// 弹出充值界面
        /// </summary>
        public void ShowTopUpPanel()
        {
            //Debug.LogWarning("弹出充值界面");
            MainUIController.Instance.OpenMainUI(UIType.TopUp);
        }

        protected override void RegisterEventHandler()
        {
            throw new System.NotImplementedException();
        }
    }
}
