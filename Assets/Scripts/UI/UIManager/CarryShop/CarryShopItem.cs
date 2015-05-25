using UnityEngine;
using System.Collections;

public class CarryShopItem : MonoBehaviour {
	[HideInInspector]
	public ShopConfigData shopData;// GetShopData;
	//特效
	public GameObject effObj;
	//实体
	public GameObject markContent;
	public GameObject iconContent;
	public Transform iconParent;
	private GameObject iconObj;
	public UILabel labelGoodsCount;
	public SpriteSwith spriteQuality;
	public SpriteSwith spriteMoney;
	public UILabel labelMoney;
	//标题
	public UILabel labelTitle;
	//标记
	public GameObject markObj;
	public GameObject lockObj;
	public GameObject overObj;
	//数据列表中的下标
	[HideInInspector]
	public DCarryShopUint shopUint;
	//界面结点下标
	[HideInInspector]
	public int curIndex;
	private UI.MainUI.CarryShopPanel uiParent;
	private ItemData goodsData;
	[HideInInspector]
	public bool isItemLock;
	[HideInInspector]
	public bool isItemOver;
	public void Show(UI.MainUI.CarryShopPanel parentObj,DCarryShopUint shop,int index,bool isLock,bool isOver)
	{
		uiParent = parentObj;
		shopUint = shop;
		curIndex = index;
		isItemLock = isLock;
		isItemOver = isOver;
		shopData = ShopDataManager.Instance.GetShopData ((int)shopUint.dwShopID);
		effObj.SetActive (false);
		if (iconObj != null) {
			DestroyImmediate(iconObj);		
		}
		if (isLock) {
			SetLockInfo();
		} else {
			SetContent(isOver);		
		}
	}
	//播放特效
	public void PlayEff()
	{
		effObj.SetActive (true);
	}
	void SetContent(bool isOver)
	{
		if (isOver)
			SetOverInfo ();
		else
			SetNormal ();
		goodsData = ItemDataManager.Instance.GetItemData(shopData.GoodsID);
		labelTitle.text = LanguageTextManager.GetString (goodsData._szGoodsName);
		if (iconObj != null)
			DestroyImmediate (iconObj);
		iconObj = UI.CreatObjectToNGUI.InstantiateObj (goodsData._picPrefab,iconParent);
		spriteQuality.ChangeSprite (goodsData._ColorLevel+1);
		labelGoodsCount.text = shopData._goodsNum.ToString ();
		labelMoney.text = shopData.Price.ToString ();
		int changeIndex = 1;
		if (shopData.BuyType == 3) {
			changeIndex = 2;		
		}
		spriteMoney.ChangeSprite (changeIndex);
	}
	void SetNormal()
	{
		markContent.transform.localPosition = new Vector3 (0,0,-2);
		iconContent.SetActive (true);
		markObj.SetActive (false);
	}
	void SetLockInfo()
	{
		int price = CarryShopModel.Instance.GetUnLockPrice (shopUint.byIndex);
		labelMoney.text = price.ToString ();
		labelTitle.text = "";
		markContent.transform.localPosition = new Vector3 (0,0,-15);
		iconContent.SetActive (false);
		markObj.SetActive (true);
		lockObj.SetActive (true);
		overObj.SetActive (false);
	}
	void SetOverInfo()
	{
		markContent.transform.localPosition = new Vector3 (0,0,-2);
		iconContent.SetActive (true);
		markObj.SetActive (true);
		lockObj.SetActive (false);
		overObj.SetActive (true);
	}
	//点击选中
	void OnClick()
	{
		uiParent.OnSelectGoods (this);
	}
}
