using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndLessRankPanel : MonoBehaviour {
	public UIGrid grid;
	public GameObject prefabItem;
	private List<EndLessRankItem> gridList = new List<EndLessRankItem>();
	public void Init()
	{
		prefabItem.SetActive (false);
	}
	
	public void Show()
	{
		ShowGrid ();
	}
	//当前可显示波数//
	private int canShowWave = 0;
	private int finishPassWave = 0;

	private void ShowGrid()
	{
		canShowWave = EctypeModel.Instance.GetCurCanShowWave ();
		if (canShowWave != gridList.Count) {
			prefabItem.SetActive(true);
			CreateItem();
			prefabItem.SetActive(false);
		}
		UpdateGrid ();
		SetShowPosition ();
	}
	//创建新的
	private void CreateItem()
	{
		List<EndLessEctypeConfigData> curList = EctypeModel.Instance.GetCurEctypeWave();
		int startCount = gridList.Count;
		for (int i = startCount; i < curList.Count && i < canShowWave; i++) {
			EndLessEctypeConfigData data = curList[i];
			GameObject item = UI.CreatObjectToNGUI.InstantiateObj(prefabItem,grid.transform);
			EndLessRankItem rankItem = item.GetComponent<EndLessRankItem>();
			ItemData getItem = ItemDataManager.Instance.GetItemData(data.rewardList[0].itemID);
			rankItem.Init(getItem,data.rewardList[0].itemCount,i+1);
			gridList.Add(rankItem);
		}
	}
	//显示
	private void UpdateGrid()
	{
		for (int i = 0; i < gridList.Count; i++) {
			gridList[i].gameObject.name = string.Format("item{0:d2}",canShowWave - i - 1);
			if(i < EctypeModel.Instance.historyBestLoopNum)
			{
				//已通关
				if(i+1 == EctypeModel.Instance.todayBestLoopNum)
				{
					gridList[i].Show(false,true);
				}
				else
				{
					gridList[i].Show(false,false);
				}
			}
			else
			{
				gridList[i].Show(true,false);
			}
		}
		grid.Reposition ();
	}
	//设置显示位置
	private void SetShowPosition()
	{

	}
}
