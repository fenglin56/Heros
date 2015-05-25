using UnityEngine;
using System.Collections;

public class ItemIconInfo : MonoBehaviour {
    public SingleButtonCallBack ItemBtn;
	public Transform iconParent;
	public UILabel labelCount;
	public SpriteSwith qualityColor;
	private GameObject iconObj;
	private ItemData itemInfo;
    void Awake()
    {
        if(ItemBtn!=null)
        {
        ItemBtn.SetCallBackFuntion(OnItemClick);
        }
    }
    void OnItemClick(object obj)
    {
        UI.MainUI.ItemInfoTipsManager.Instance.Show(itemInfo._goodID);
    }
	public void Init(ItemData item,string countStr)
	{
		itemInfo = item;
		labelCount.text = countStr;
		qualityColor.ChangeSprite (item._ColorLevel+1);
		if (iconObj == null) {
			iconObj = UI.CreatObjectToNGUI.InstantiateObj(item._picPrefab,iconParent);		
		}
	}
	public void Show(ItemData item,string countStr)
	{
		labelCount.text = countStr;
		//当不是同一个，
		if (itemInfo._goodID != item._goodID) {
			if(iconObj != null)
			{
				DestroyImmediate(iconObj);
			}
			iconObj = UI.CreatObjectToNGUI.InstantiateObj(item._picPrefab,iconParent);
			qualityColor.ChangeSprite (item._ColorLevel+1);
		}
	}
}
