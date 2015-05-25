using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{

    public class ShopGoodsListPanel_V2 : MonoBehaviour
    {
        public List<ShopSingleGoodsBox_V2> shopSingleGoodsBoxList;
        public UILabel PageNumberLabel;

        public SingleButtonCallBack NextPageBtn;
        public SingleButtonCallBack LastPageBtn;

        ShopInfoUIManager_V2 MyParent;

        SMsgTradeOpenShop_SC sMsgTradeOpenShop_SC;

        private int CurrentPageNumber = 1;

        private List<int> m_guideBtnIDList = new List<int>();
        private int[] m_guideBtnID = new int[2];

        void Awake()
        {
            NextPageBtn.SetCallBackFuntion(OnNextPageBtnClick);
            LastPageBtn.SetCallBackFuntion(OnLastPageBtnClick);
        }

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(NextPageBtn.gameObject, UIType.Shop, SubType.ShopInfoNextPage, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(LastPageBtn.gameObject, UIType.Shop, SubType.ShopInfoNextPage, out m_guideBtnID[1]);
        }

        public void ShowGoodsList(SMsgTradeOpenShop_SC sMsgTradeOpenShop_SC, ShopInfoUIManager_V2 MyParent)
        {
            this.sMsgTradeOpenShop_SC = sMsgTradeOpenShop_SC;
            this.MyParent = MyParent;
            CurrentPageNumber = 1;
            ResetPageInfo();
        }

        void OnNextPageBtnClick(object obj)
        {
            if (CurrentPageNumber * 4 < this.sMsgTradeOpenShop_SC.bShopGoodsNum)
            {
                CurrentPageNumber++;
                ResetPageInfo();
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            }
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnIDList.Count; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnIDList[i]);
            }

            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        void OnLastPageBtnClick(object obj)
        {
            if (CurrentPageNumber >1)
            {
                CurrentPageNumber--;
                ResetPageInfo();
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            }
        }

        void ResetPageInfo()
        {
            for (int i = 0; i < m_guideBtnIDList.Count; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnIDList[i]);
            }
            m_guideBtnIDList.Clear();
            
            for (int i = 0; i < shopSingleGoodsBoxList.Count; i++)
            {
                int currentItemIndex = i + (CurrentPageNumber-1)*4 ;
                if (currentItemIndex < this.sMsgTradeOpenShop_SC.bShopGoodsNum)
                {
                    int guideID = 0;
                    //TODO GuideBtnManager.Instance.RegGuideButton(shopSingleGoodsBoxList[i].gameObject, UIType.Shop, SubType.ShopInfoGoodItem, out guideID);
                    m_guideBtnIDList.Add(guideID);
                    shopSingleGoodsBoxList[i].gameObject.SetActive(true);
                    shopSingleGoodsBoxList[i].Init(this.sMsgTradeOpenShop_SC.ShopGoodsInfo[currentItemIndex],this);
                }
                else
                {
                    shopSingleGoodsBoxList[i].gameObject.SetActive(false);
                }
                PageNumberLabel.SetText(string.Format("{0}/{1}", CurrentPageNumber, this.sMsgTradeOpenShop_SC.bShopGoodsNum / 4 + (sMsgTradeOpenShop_SC.bShopGoodsNum % 4 > 0 ? 1 : 0)));
            }
            Color enableColor = new Color(1, 1, 1, 1);
            Color disableColor = new Color(1, 1, 1, 0.3f);
            NextPageBtn.BackgroundSprite.color = CurrentPageNumber * 4 < this.sMsgTradeOpenShop_SC.bShopGoodsNum ? enableColor : disableColor;
            LastPageBtn.BackgroundSprite.color = CurrentPageNumber > 1 ? enableColor : disableColor;
            shopSingleGoodsBoxList[0].OnClick();
        }

        public void OnShopGoodsSelect(ShopSingleGoodsBox_V2 SelectGoods)
        {
            shopSingleGoodsBoxList.ApplyAllItem(P=>P.SetSelectStatus(false));
            SelectGoods.SetSelectStatus(true);
            MyParent.shopContainerTips.ShowGoodsInfo(SelectGoods,MyParent);
        }
    }
}