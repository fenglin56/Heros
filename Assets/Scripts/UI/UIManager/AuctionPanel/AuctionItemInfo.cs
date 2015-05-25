using UnityEngine;
using System.Collections;
//当前状态
using System.Text;


public enum EBuyType{
	//可竞拍
	ENoneBuyer,
	//您已出价
	ESelfBuyed,
	//已买断
	EBuyOver,
}
public class AuctionItemInfo : MonoBehaviour {
	public ItemIconInfo iconInfo;
	public UILabel iconName;
	//职业
	public SpriteSwith professionSprite;
	//类型
	public UILabel typeLabel;
	//价格
	public UILabel priceLabel;
	//出价者
	public UILabel buyerLabel;
	//竞拍
	public GameObject btnBuyObj;
	private SingleButtonCallBack btnBuy;
	//竞拍标记
	public SpriteSwith buyMark;
	private bool isRead = false;
	//其它数据
	[HideInInspector]
	public int curByIndex;
	private UI.MainUI.AuctionPanel uiParent;
	void Init()
	{
		if (isRead)
			return;
		isRead = true;
		btnBuy = btnBuyObj.GetComponent<SingleButtonCallBack>();
		btnBuy.SetCallBackFuntion (OnClickBuyEvent);
	}
	public void Show(UI.MainUI.AuctionPanel parent,int index)
	{
		uiParent = parent;
		curByIndex = index;
		Init ();
		ShowInfo ();
	}
	void ShowInfo()
	{
		DAuctionUint auctionData = AuctionModel.Instance.GetGoodsInfo (curByIndex);
		ItemData item = ItemDataManager.Instance.GetItemData((int)auctionData.dwGoodsID);
		//物品
		iconInfo.Init (item,"x"+auctionData.dwGoodsNum);
		iconName.text = UI.NGUIColor.SetTxtColor (LanguageTextManager.GetString(item._szGoodsName),(UI.TextColor)item._ColorLevel);
		//职业
		if (item._AllowProfession.Equals ("1")) {
			professionSprite.ChangeSprite (1);
		} else if (item._AllowProfession.Equals ("4")) {
			professionSprite.ChangeSprite (2);
		} else {
			professionSprite.ChangeSprite (3);
		}
		//类型
		typeLabel.text = LanguageTextManager.GetString(string.Format("IDS_A5_{0}{1}",item._GoodsClass,item._GoodsSubClass));
		//当前价格
		priceLabel.text = auctionData.dwCurPrice.ToString();
		//竞拍:当不是我，
		if (auctionData.dwAcotorID == 0) {
			//无人竞拍
			buyerLabel.text = LanguageTextManager.GetString ("IDS_I27_15");//Encoding.UTF8.GetString(auctionData.szName);
			btnBuyObj.gameObject.SetActive (true);
			buyMark.gameObject.SetActive (false);
		} else if (auctionData.dwAcotorID == PlayerManager.Instance.FindHeroDataModel ().ActorID) {
			buyerLabel.text = Encoding.UTF8.GetString (auctionData.szName);
			btnBuyObj.gameObject.SetActive (false);
			buyMark.gameObject.SetActive (true);
			buyMark.ChangeSprite (1);
		} else {
			buyerLabel.text = Encoding.UTF8.GetString (auctionData.szName);
			if(auctionData.dwCurPrice >= CommonDefineManager.Instance.CommonDefine.AuctionTopBid)
			{
				btnBuyObj.gameObject.SetActive (false);
				buyMark.gameObject.SetActive (true);
				buyMark.ChangeSprite (2);
			}
			else
			{
				btnBuyObj.gameObject.SetActive (true);
				buyMark.gameObject.SetActive (false);
			}
		}
	}
	void OnClickBuyEvent(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Auction_Bid");
		uiParent.OnClickBuyGoods (curByIndex);
	}
}
