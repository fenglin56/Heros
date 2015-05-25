using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class AwardDescriptionPanel : MonoBehaviour 
{
	private enum AwardTab
	{
		None = 0,
		Daily = 1,
		Season,
		LevelDescription,
	}

	public GameObject PVPAwardSingleItemPrefab;
	public UIDraggablePanel DraggablePanel;
	public UIGrid Grid;

	public SingleButtonCallBack DailyAwardTab;					// 日奖励按钮
	public SingleButtonCallBack SeasonAwardTab;				// 赛季奖励按钮
	public SingleButtonCallBack LevelDescriptionTab;			// 组别说明按钮
	public SingleButtonCallBack CloseBtn;							// 关闭按钮

	public Vector3 NormalScale;

	public UILabel RightTableHeader;									// 列表头右边显示

	private AwardTab currentTab;										// 当前面板类型
	private List<PVPAwardSingleItem> awardSingleItemList;

	void Awake()
	{
		awardSingleItemList = new List<PVPAwardSingleItem>();

		DailyAwardTab.SetCallBackFuntion(SwitchTab, AwardTab.Daily);
		SeasonAwardTab.SetCallBackFuntion(SwitchTab, AwardTab.Season);
		LevelDescriptionTab.SetCallBackFuntion(SwitchTab, AwardTab.LevelDescription);

		CloseBtn.SetCallBackFuntion(Hide);
	}

	[ContextMenu("Show")]
	public void Show()
	{
		// 播放放大动画
		// animation.Play();
		//gameObject.SetActive(true);
		currentTab = AwardTab.None;

		TweenScale.Begin(gameObject, 0.1666f, NormalScale,Vector3.one, null);
		TweenAlpha.Begin(gameObject, 0.1666f, 1);
		TweenAlpha.Begin(DraggablePanel.gameObject, 0.1666f, 1);

		SwitchTab(AwardTab.Daily);
	}

	public void Hide(object obj)
	{
		// 播放缩小动画
		// animation.Play();
		SoundManager.Instance.PlaySoundEffect("Sound_Button_PVG_ListAwardIntroductionClose");
		//gameObject.SetActive(false);
		TweenScale.Begin(gameObject, 0.1666f, Vector3.one, NormalScale, null);
		TweenAlpha.Begin(gameObject, 0.1666f, 0);
		TweenAlpha.Begin(DraggablePanel.gameObject, 0.1666f, 0);
	}

	private void SwitchTab(object obj)
	{
		if(currentTab == (AwardTab)obj)
		{
			return;
		}

		SoundManager.Instance.PlaySoundEffect("Sound_Button_PVG_ListAwardIntroductionChange");
		currentTab = (AwardTab)obj;
		// 更改按钮显示
		DailyAwardTab.spriteSwithList.ApplyAllItem(P => P.ChangeSprite(currentTab == AwardTab.Daily? 2: 1));
		SeasonAwardTab.spriteSwithList.ApplyAllItem(P => P.ChangeSprite(currentTab == AwardTab.Season? 2: 1));
		LevelDescriptionTab.spriteSwithList.ApplyAllItem(P => P.ChangeSprite(currentTab == AwardTab.LevelDescription? 2: 1));

		RightTableHeader.text = (currentTab == AwardTab.LevelDescription)? "晋升条件": "奖励";

		// 列表中item只创建一次
		if(awardSingleItemList != null && awardSingleItemList.Count == 0)
		{
			for(int index = 0, imax =PvpUiPanelManager.Instance.GroupConfigDatabase._dataTable.Length; index < imax; index++)
			{
				GameObject item = NGUITools.AddChild(Grid.gameObject, PVPAwardSingleItemPrefab);
				item.name = PVPAwardSingleItemPrefab.name + index.ToString().PadLeft(2, '0');
				PVPAwardSingleItem itemScript = item.GetComponent<PVPAwardSingleItem>();
				awardSingleItemList.Add(itemScript);
			}

			Grid.sorted = true;
			Grid.Reposition();
		}

		DraggablePanel.DisableSpring();
		DraggablePanel.ResetPosition();

		UpdateSingleItem();
	}

	private void UpdateSingleItem()
	{
		for(int index = 0, imax = awardSingleItemList.Count; index < imax; index++)
		{
			switch(currentTab)
			{
			case AwardTab.Daily:
			case AwardTab.Season:
				PVPGroupListAward award = GetListAwardData(currentTab, index + 1);
				if(award != null)
				{
					awardSingleItemList[index].AwardDesInit(index, award.ListAwardIcon, award.ListAwardName, award.ListAward01Icon, award.ListAward01Des, award.ListAward02Icon, award.ListAward02Des);
				}
				break;
			case AwardTab.LevelDescription:
				string levelName = PvpUiPanelManager.Instance.GroupConfigDatabase._dataTable[index].PVPGroupName;
				string levelDes = PvpUiPanelManager.Instance.GroupConfigDatabase._dataTable[index].GroupLevelUpIDS;

				awardSingleItemList[index].LevelDesInit(index, levelName, levelDes);
				break;
			}
		}	
	}

	private PVPGroupListAward GetListAwardData(AwardTab  awardType, int  level)
	{
		return PvpUiPanelManager.Instance.GroupListAwardDatabase._dataTable.SingleOrDefault(P => P.ListAwardGroup == level && P.ListAwardType == (int)awardType);
	}
}