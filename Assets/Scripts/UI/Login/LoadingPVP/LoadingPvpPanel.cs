using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LoadingPvpPanel : IUIPanel {
	public GameObject leftItem;
	private List<PvpLoadingItem> leftItemList = new List<PvpLoadingItem>();
	public GameObject rightItem;
	private List<PvpLoadingItem> rightItemList = new List<PvpLoadingItem>();
	public List<Transform> showItemLPosList ;//"ItemLPos31" "ItemLPos32" "ItemLPos33","ItemLPos21","ItemLPos22","ItemLPos11"
	public List<Transform> showItemRPosList ;//"ItemRPos"同上
	private List<SGroupMemberInfo> myGroup ;
	private List<SGroupMemberInfo> enemyGroup ;
	GameObject DontDestroyObj;
	void Awake()
	{
		Show ();
	}
	private bool isRead = false;
	void Init () {
		if (isRead)
			return;
		isRead = true;
		UIEventManager.Instance.RegisterUIEvent(UIEventType.pvpItemDataUpdate, OnUpdateSuccess);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.LoadingComplete, OnSceneLoadingComplete);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OnLostConectEvent,OnLostConectEvent);
	}
	//public void Show (List<SGroupMemberInfo> myG,List<SGroupMemberInfo> enemyG) 
	public override void Show()
	{
		Init ();
		myGroup = PvpDataManager.Instance.GetGroupmeberInfoList ();
		enemyGroup = PvpDataManager.Instance.GetGroupmeberInfoList ();
		UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UI.MainUI.UIType.Empty);
		transform.localPosition = Vector3.zero;
		ShowPanel (true);
		ShowPanel (false);
	}
	void GetUIRoot(Transform Trs)
	{
		if (Trs.parent != null)
		{
			GetUIRoot(Trs.parent);
		}
		else
		{
			this.DontDestroyObj = Trs.gameObject;
			this.DontDestroyObj.AddComponent<DontDestroy>();
		}
	}
	void ShowPanel(bool isLeft)
	{
		List<PvpLoadingItem> itemList = leftItemList;
		List<Transform> showPosList = showItemLPosList;
		List<SGroupMemberInfo> dataList = myGroup;
		GameObject item = leftItem;
		string posSplitName = "ItemLPos";
		if (!isLeft) {
			itemList = rightItemList;
			showPosList = showItemRPosList;
			dataList = enemyGroup;
			posSplitName = "ItemRPos";
		}
		item.SetActive (true);
		int i = 0; 
		for (i = 0; i < dataList.Count; i++) {
			PvpLoadingItem iconItem;
			Transform parentPos = GetItemShowPos(dataList.Count,i+1,posSplitName,showPosList);
			if(itemList.Count <= i)
			{
				iconItem = NGUITools.AddChild(parentPos.gameObject,item).GetComponent<PvpLoadingItem>();
				itemList.Add(iconItem);
			}
			else
			{
				iconItem = itemList[i];
				iconItem.gameObject.SetActive(true);
				iconItem.transform.parent = parentPos;
			}
			iconItem.Show(dataList[i]);
		}
		for(; i < itemList.Count; i++)
		{
			itemList[i].gameObject.SetActive(false);
		}
		item.SetActive (false);
	}
	Transform GetItemShowPos(int allCount,int index,string splitName,List<Transform> showPosList)
	{
		splitName = string.Format("{0}{1}{2}",splitName , allCount , index);
		foreach (Transform trans in showPosList) {
			if(trans.name.Equals(splitName))
			{
				return trans;
			}
		}
		TraceUtil.Log (SystemModel.Common,TraceLevel.Error,"loadingPvpPanel not find postiton of panel");
		return null;
	}
	void OnUpdateSuccess(object obj)
	{
		bool isFind = false;
		int roleID = (int)obj;
		foreach (PvpLoadingItem roleItem in leftItemList) {
			if(roleItem.roleInfo.dwActorID == roleID)
			{
				roleItem.UpdateInfo(true);
				isFind = true;
			}
		}
		if (isFind)
			return;
		foreach (PvpLoadingItem roleItem in rightItemList) {
			if(roleItem.roleInfo.dwActorID == roleID)
			{
				roleItem.UpdateInfo(true);
			}
		}
	}

	void OnSceneLoadingComplete(object obj)
	{
		DestroyPanel();
	}
	void OnLostConectEvent(object obj)
	{
		Close ();	
	}
	public override void Close()
	{
		transform.localPosition = new Vector3(0, 0, -1000);
	}
	public override void DestroyPanel()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.pvpItemDataUpdate, OnUpdateSuccess);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.LoadingComplete, OnSceneLoadingComplete);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnLostConectEvent,OnLostConectEvent);
		Destroy(DontDestroyObj);
	}
}
