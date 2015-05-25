using UnityEngine;
using System.Collections.Generic;

using UI.MainUI;

/// <summary>
/// 青龙武学图标显示区域
/// </summary>
public class QinglongMartialItemArea : MonoBehaviour 
{
	public GameObject MartialItemPrefab;		//武学图标预制
	//public List<MartialItem> AllMartialList;	//所有武学列表
	private List<MartialItem> CurrentShowList;	//当前显示的武学列表，根据武学分类

	public Transform[] MartialItemPoint;	//武学图标挂载点
	private MartialItemListPanel_V2 MyParent;
	private MartialItemListPanel_V2 ParentItemListPanel;

	void Awake()
	{
		CurrentShowList = new List<MartialItem>();
	}

	/// <summary>
	/// 显示初始化
	/// </summary>
	/// <param name="lineIndex">Line index.</param>
	/// <param name="perLineCount">Per line count.</param>
	/// <param name="parent">Parent.</param>
	public void Init(int lineIndex, int perLineCount, MartialItemListPanel_V2 parent)
	{
		MyParent = parent;
		ParentItemListPanel = parent.GetComponent<MartialItemListPanel_V2>();
		//LineIndex = lineIndex;
		for(int i = 0; i < perLineCount; i++)
		{
			GameObject item = NGUITools.AddChild(MartialItemPoint[i].gameObject, MartialItemPrefab);
			MartialItem martialItem = item.GetComponent<MartialItem>();
			SingleButtonCallBack btnCB = item.GetComponent<SingleButtonCallBack>();
			btnCB.SetCallBackFuntion(OnSelectItemCallback, parent.MyMartialDataList[lineIndex*4+i].dwWuXueID);
			martialItem.Init(parent.MyMartialDataList[lineIndex*4+i], parent.ThisParent.GetComponent<QinglongMartialPanel>().OnItemClick);
			CurrentShowList.Add(martialItem);
			if(lineIndex*4+i == 0)
			{
				ParentItemListPanel.ThisParent.GetComponent<QinglongMartialPanel>().OnItemClick(martialItem.MartialID);
			}
		}
	}

	/// <summary>
	/// 解锁新武学的时候添加一个武学图标
	/// </summary>
	/// <param name="lineIndex">Line index.</param>
	/// <param name="index">Index.</param>
	/// <param name="parent">Parent.</param>
	public void AddItem(int lineIndex, int index, MartialItemListPanel_V2 parent)
	{
		MyParent = parent;
		GameObject item = NGUITools.AddChild(MartialItemPoint[index].gameObject, MartialItemPrefab);
		MartialItem martialItem = item.GetComponent<MartialItem>();
		SingleButtonCallBack btnCB = item.GetComponent<SingleButtonCallBack>();
		btnCB.SetCallBackFuntion(OnSelectItemCallback, ParentItemListPanel.MyMartialDataList[lineIndex*4+index].dwWuXueID);
		martialItem.Init(ParentItemListPanel.MyMartialDataList[lineIndex*4+index], ParentItemListPanel.ThisParent.GetComponent<QinglongMartialPanel>().OnItemClick);
		CurrentShowList.Add(martialItem);
	}

	/// <summary>
	/// 武学图标被选中回调
	/// </summary>
	/// <param name="arg">Argument.</param>
	public void OnSelectItemCallback(object arg)
	{
		int martialId = (int)arg;
		CurrentShowList.ApplyAllItem(p=>{
			if(martialId == p.MartialID)
			{
				p.BeSelected(true);
			}
			else
			{
				p.BeSelected(false);
			}
		});
	}

	/// <summary>
	/// 武学学习或升级回调
	/// </summary>
	/// <param name="martialID">Martial I.</param>
	public void OnLearnOrUpdateCallback(int martialID)
	{
		CurrentShowList.ApplyAllItem(p=>{
			if(p.MartialID == martialID)
			{
				p.LevelUpdateCallback(martialID);
			}
		});
	}

	public void OnDestroy()
	{
		CurrentShowList.ApplyAllItem(p=>Destroy(p.gameObject));
		CurrentShowList.Clear();
	}
}


