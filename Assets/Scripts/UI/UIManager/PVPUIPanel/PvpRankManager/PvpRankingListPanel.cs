using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvpRankingListPanel : MonoBehaviour {
	public GameObject PlayerRankingItem_Prefab;
	public UIGrid ItemTable;
	public SMsgInteract_RankingList_SC Data;
	[HideInInspector]
	public  List<PvpRankingListItem> PVPRankingItemList=new List<PvpRankingListItem>();
	[HideInInspector]
	public List<PvpRankingListItem> PVPRankingSelectedItemList=new List<PvpRankingListItem>();
	private GameObject Item_go;
	public UIDraggablePanel draggablePanel;
	public void InitList(RankingType type)
	{
		
		for(int i=0;i<10;i++)
		{
			Item_go=NGUITools.AddChild(ItemTable.gameObject,PlayerRankingItem_Prefab);
			Item_go.name=PlayerRankingItem_Prefab.name+i;
			Item_go.AddComponent<UIDragPanelContents>();
			PvpRankingListItem Sc_item=Item_go.GetComponent<PvpRankingListItem>();
			Item_go.SetActive(false);
			//Sc_item.OnClickCallBack=ItemSelectedEventHandle;
			PVPRankingItemList.Add(Sc_item);
		}
		ItemTable.Reposition();
	}
	public void StartRefershList( List<SPVPRankingData> datas)
	{
		StartCoroutine(RefershList(datas));
	}
	
	IEnumerator RefershList( List<SPVPRankingData> datas)
	{
		
		
		PVPRankingItemList.ApplyAllItem(p=>p.gameObject.SetActive(false));
		if(datas.Count>0)
		{
			for(int i=0;i<datas.Count;i++)
			{
				PVPRankingItemList[i].InitItemData(datas[i]);
				PVPRankingItemList[i].gameObject.SetActive(true);
			}
		}
		
		
		yield return null;
		draggablePanel.ResetPosition();
		ItemTable.Reposition();
		
	}

}
