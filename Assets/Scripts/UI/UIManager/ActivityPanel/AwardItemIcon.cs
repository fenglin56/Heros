using UnityEngine;
using System.Collections;

public class AwardItemIcon : MonoBehaviour {
    public SingleButtonCallBack ItemBtn;
	public UILabel labelName;
	//public UILabel labelCount;
	public SpriteSwith qualityColor;
	public GameObject iconParent;
	private GameObject iconObj;
    private int m_GoodsID;
	// Use this for initialization
    void Awake()
    {
        if(ItemBtn!=null)
        {
            ItemBtn.SetCallBackFuntion(OnItemClick);
        }
    }
    void OnItemClick(object obj)
    {
        UI.MainUI.ItemInfoTipsManager.Instance.Show(m_GoodsID);
    }
	public void Show (int goodID,int count) {
        m_GoodsID=goodID;
		ItemData getItem = ItemDataManager.Instance.GetItemData(goodID);
		iconObj = UI.CreatObjectToNGUI.InstantiateObj(getItem._picPrefab,iconParent.transform);
		qualityColor.ChangeSprite (getItem._ColorLevel+1);
		//string nameStr = UI.NGUIColor.SetTxtColor (LanguageTextManager.GetString(getItem._szGoodsName),(UI.TextColor)getItem._ColorLevel);
		labelName.text = string.Format("x{0}",count);
		//labelCount.text = string.Format("x {0}",count);
	}
}
