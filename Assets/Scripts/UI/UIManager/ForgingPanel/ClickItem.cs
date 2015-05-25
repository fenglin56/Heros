using UnityEngine;
using System.Collections;

public class ClickItem : MonoBehaviour {
    private BoxCollider collider;
    private SingleButtonCallBack buttonCallback;
    private int m_goodsId;
	// Use this for initialization
	void Start () {
        collider=gameObject.AddComponent<BoxCollider>();
        collider.size=new Vector3(110,110,1);
        collider.center=new Vector3(0,0,-2);
        buttonCallback=gameObject.AddComponent<SingleButtonCallBack>();
        buttonCallback.SetCallBackFuntion(OnItemClick);
	}
    public void Init(int GoodsID)
    {
        m_goodsId=GoodsID;
    }
	void OnItemClick(object obj)
    {
        UI.MainUI.ItemInfoTipsManager.Instance.Show(m_goodsId);
    }
	
}
