using UnityEngine;
using System.Collections;
using System.Linq;

public class QuickPurchasePanelUIManager : MonoBehaviour 
{
    public UISlicedSprite Sprite_ItemIcon;
    //public Transform ItemIcon;
    public UILabel Label_Amount;
    public UILabel Label_Description;
    public LocalButtonCallBack Button_Cut;
    public LocalButtonCallBack Button_Add;

    //购买
    public LocalButtonCallBack Button_Buy;
    public LocalButtonCallBack Button_Cancel;
    public UILabel Lable_cost;
    public SpriteSwith Switch_cost;
    public UIScrollBar BuyScrollBar;

    private ItemData m_curItemData;
    private ShopConfigData m_curShopConfigData;
    private int m_curAmount;
    private bool isCaps = false;    //上限
    private bool isNoEnoughMoney = false;
    //private GameObject m_ItemPic;

    void Awake()
    {
        Button_Cut.SetCallBackFuntion(CutItemAmountCallBack, null);
        Button_Add.SetCallBackFuntion(AddItemAmountCallBack, null);
        Button_Buy.SetCallBackFuntion(BuyCallBack, null);
        Button_Cancel.SetCallBackFuntion(ExitCallBack, null);

        BuyScrollBar.onChange = OnBuyScrollBarCallBack;

        ClosePanel();
        UIEventManager.Instance.RegisterUIEvent(UIEventType.QuickPurchase, ReceiveQuickPurchaseHandle);
        
    }

    void OnDestroy()
    {
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.QuickPurchase, ReceiveQuickPurchaseHandle);
    }

    public void OpenPanel()
    {
        Label_Amount.color = Color.white;
        transform.localPosition = new Vector3(0, 0, -200);        
    }
    public void ClosePanel()
    {
        transform.localPosition = new Vector3(0, 0, -800);
    }

    void ReceiveQuickPurchaseHandle(object obj)
    {
        if(obj is int ||obj is uint)
        {
            OpenPanel();
            InitQuickPurchasePanel((int)obj);
        }
    }

    void CutItemAmountCallBack(object obj)
    {        
        if (isCaps)
        {
            if (m_curAmount <= 1)
                return;

            isCaps = false;
            Label_Amount.color = Color.white;
        }
        
        m_curAmount--;
        m_curAmount = Mathf.Clamp(m_curAmount, 1, 99);

        this.UpdateDisplay();
        this.UpdateBuyScrollBar();
    }

    void AddItemAmountCallBack(object obj)
    {
        if (isCaps)
            return;
        m_curAmount++;
        m_curAmount = Mathf.Clamp(m_curAmount, 1, 99);
        int occupyPackBoxNum = Mathf.CeilToInt(m_curAmount * 1f / m_curItemData._PileQty);
        //TraceUtil.Log("[occupyPackBoxNum]" + occupyPackBoxNum);
        int emptyPackBoxNum = UI.MainUI.ContainerInfomanager.Instance.GetEmptyPackBoxNumber();
        //TraceUtil.Log("[GetEmptyPackBoxNumber]" + emptyPackBoxNum);
        if (occupyPackBoxNum > emptyPackBoxNum)
        {
            Label_Amount.color = Color.red;
            isCaps = true;
        }

        this.UpdateDisplay();
        this.UpdateBuyScrollBar();
    }

    void BuyCallBack(object obj)
    {
        if (isCaps)
        {
            ClosePanel();
            UI.MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_2"), 1);
            TraceUtil.Log("[物品栏空间不足]");
            return;
        }

        if (isNoEnoughMoney)
        {
            UI.MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H2_44"), 1f);
        }
        else
        {
            //NetServiceManager.Instance.TradeService.SendTradeQuickBuyGoods((uint)m_curItemData._goodID, (uint)m_curAmount);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Pay");
            NetServiceManager.Instance.TradeService.SendTradeBuyGoods(new SMsgTradeBuyGoods_CS()
            {
                dwShopID = (uint)m_curShopConfigData._shopID,
                uidNPC = 0,
                lShopGoodsID = (uint)m_curShopConfigData._shopGoodsID,
                GoodsID = (uint)m_curShopConfigData.GoodsID,
                GoodsNum = (uint)m_curAmount
            });
            ClosePanel();
        }        
    }

    void ExitCallBack(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        this.ClosePanel();
    }

    void OnBuyScrollBarCallBack(UIScrollBar dragScorllBar)
    {
        m_curAmount = 1 + (int)(dragScorllBar.scrollValue * 98);
        this.UpdateDisplay();
    }
    ///// <summary>
    ///// 初始化快速购买面板
    ///// </summary>
    ///// <param name="itemID">购买道具id</param>
    //public void InitQuickPurchasePanel(int itemID)
    //{
    //    var itemData = ItemDataManager.Instance.GetItemData(itemID);        
    //    if (itemData == null)
    //        return;
    //    var shopConfigData = ShopDataManager.Instance.shopConfigDataBase._dataTable.First(P => P.GoodsID == itemID);
    //    if (shopConfigData == null)
    //        return;

    //    m_curItemData = itemData;
    //    m_curShopConfigData = shopConfigData;

    //    m_curAmount = 0;
    //    isCaps = false;
    //    Sprite_ItemIcon.spriteName = itemData.smallDisplay;
    //    Switch_cost.ChangeSprite(shopConfigData.BuyType);

    //    Label_Description.text = LanguageTextManager.GetString(itemData._szDesc);
    //    this.AddItemAmountCallBack(null);
    //}

    /// <summary>
    /// 初始化快速购买面板
    /// </summary>
    /// <param name="itemID">购买道具id</param>
    public void InitQuickPurchasePanel(int shopID)
    {
        var shopConfigData = ShopDataManager.Instance.shopConfigDataBase._dataTable.First(P => P._shopGoodsID == shopID);
        if (shopConfigData == null)
            return;
        var itemData = ItemDataManager.Instance.GetItemData(shopConfigData.GoodsID);
        if (itemData == null)
            return;
        m_curItemData = itemData;
        m_curShopConfigData = shopConfigData;

        m_curAmount = 0;
        isCaps = false;
        Sprite_ItemIcon.spriteName = itemData.smallDisplay;
        Switch_cost.ChangeSprite(shopConfigData.BuyType);

        Label_Description.text = LanguageTextManager.GetString(itemData._szDesc);
        this.AddItemAmountCallBack(null);
    }

    //更新显示
    private void UpdateDisplay()
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        Label_Amount.text = m_curAmount.ToString();
        int needPrice = m_curShopConfigData.Price * m_curAmount;

        var playerData = PlayerManager.Instance.FindHeroDataModel();
        if (playerData.PlayerValues.PLAYER_FIELD_BINDPAY < needPrice)
        {
            isNoEnoughMoney = true;
            Lable_cost.color = Color.red;
        }
        else
        {
            isNoEnoughMoney = false;
            Lable_cost.color = Color.white;
        }
        Lable_cost.text = needPrice.ToString();
        
    }
    //更新滑动条
    private void UpdateBuyScrollBar()
    {
        BuyScrollBar.scrollValue = m_curAmount / 99f;
    }
}
