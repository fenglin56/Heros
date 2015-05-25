using UnityEngine;
using System.Collections;

public class VipAwardItemIcon : MonoBehaviour {
	public SingleButtonCallBack iconBtn;
	public UILabel labelName;
	//public UILabel labelCount;
	public SpriteSwith qualityColor;
	public GameObject iconParent;
	private GameObject iconObj;
	private int goodsID;
	void Start()
	{
		if(iconBtn != null)
			iconBtn.SetCallBackFuntion (OnClickBtnEvent);
	}
	// Use this for initialization
	public void Show (int goodID,int count) {
		goodsID = goodID;
		ItemData getItem = ItemDataManager.Instance.GetItemData(goodID);
		if (iconObj != null)
			DestroyImmediate (iconObj);
		iconObj = UI.CreatObjectToNGUI.InstantiateObj(getItem._picPrefab,iconParent.transform);
		qualityColor.ChangeSprite (getItem._ColorLevel+1);
		string nameStr = UI.NGUIColor.SetTxtColor (LanguageTextManager.GetString(getItem._szGoodsName),(UI.TextColor)getItem._ColorLevel);
		labelName.text = string.Format("{0}x{1}",nameStr,count);
		//labelCount.text = string.Format("x {0}",count);
	}
	//showTip直接把文本和颜色一起带过来//
	public void ShowGoods (int goodID,string showTip) {
		goodsID = goodID;
		ItemData getItem = ItemDataManager.Instance.GetItemData(goodID);
		if (iconObj != null)
			DestroyImmediate (iconObj);
		iconObj = UI.CreatObjectToNGUI.InstantiateObj(getItem._picPrefab,iconParent.transform);
		qualityColor.ChangeSprite (getItem._ColorLevel+1);
		labelName.text = showTip;
		//labelCount.text = string.Format("x {0}",count);
	}
	void OnClickBtnEvent(object obj)
	{
		UI.MainUI.ItemInfoTipsManager.Instance.Show (goodsID);
	}
}
