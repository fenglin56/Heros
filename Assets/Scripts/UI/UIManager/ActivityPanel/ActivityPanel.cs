using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace UI.MainUI
{
	public class ActivityPanel : BaseUIPanel {
		public GameObject leftObj;
		private Vector3 leftOriPos;
		public GameObject rightObj;
		private Vector3 rightOriPos;
		private Vector3 movePos = new Vector3(51,0,0);
		//左边功能栏
		public UIGrid grid;
		public GameObject itemPrefab;
		private List<ActivityItem> gridItemList = new List<ActivityItem> ();
		//右边功能栏
		public UILabel topInfo;
		public GameObject pointParent;
		public GameObject rewardItemPrefab;
		public UILabel conditionInfo;
		//领取奖励
		public SingleButtonCallBack getRewardBtn;
		public UILabel getRewardWord;
		//返回
		public SingleButtonCallBack btnBack;
		private ActivityConfigData activityConfig;
		private bool isRead = false;
		void Init()
		{
			//Test ();
			if (isRead)
				return;
			isRead = true;
			leftOriPos = leftObj.transform.localPosition;
			rightOriPos = rightObj.transform.localPosition;
			itemPrefab.SetActive(false);
			getRewardBtn.SetCallBackFuntion (OnClickGetRewardEvent);
			btnBack.SetCallBackFuntion (OnClickBackEvent);
			DailySignModel.Instance.curSelectActivityID = DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList [0].dwActiveID;
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ActivityTimeUpdate,OnActivityTimeUpdateHandler);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ActivityRewardEvetn,OnReceiveActivityGetRewardHandler);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveActivityDataEvent,OnReceiveActivityUpdateHandler);
			TaskGuideBtnRegister ();
		}
		void Test()
		{
			DailySignModel.Instance.sActiveMsgInteract_OpenUI = new SMsgInteract_OpenUI ();
			DailySignModel.Instance.sActiveMsgInteract_OpenUI.byActiveNum = 3;
			DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList = new DGameActiveData[3];
			DGameActiveData aa1 = new DGameActiveData ();
			aa1.dwActiveID = 101;
			aa1.dwActiveParam = 2;
			aa1.byIndex = 0;
			DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList [0] = aa1;
			DGameActiveData aa2 = new DGameActiveData ();
			aa2.dwActiveID = 102;
			aa2.dwActiveParam = 2;
			aa2.byIndex = 0;
			DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList [1] = aa2;
			DGameActiveData aa3 = new DGameActiveData ();
			aa3.dwActiveID = 103;
			aa3.dwActiveParam = 2;
			aa3.byIndex = 0;
			DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList [2] = aa3;
		}
		void CreateGridItem()
		{
			if (gridItemList.Count < (int)DailySignModel.Instance.sActiveMsgInteract_OpenUI.byActiveNum) {
				itemPrefab.SetActive(true);
				for(int i = gridItemList.Count; i < (int)DailySignModel.Instance.sActiveMsgInteract_OpenUI.byActiveNum; i++)
				{
					GameObject item = CreatObjectToNGUI.InstantiateObj(itemPrefab,grid.transform);
					item.name = string.Format("item{0:d2}",i);
					ActivityItem actItem = item.GetComponent<ActivityItem>();
					gridItemList.Add(actItem);

				}
				itemPrefab.SetActive(false);
			}
			int activeID = 0;
			for (int j = 0; j < gridItemList.Count; j++) {
				activeID = DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList[j].dwActiveID;
				gridItemList[j].Show(this,activeID);	
				gridItemList[j].gameObject.RegisterBtnMappingId(activeID,UIType.Activity, BtnMapId_Sub.Activity_ListItem);
			}
		}

		void ShowRightTop()
		{
			activityConfig = PlayerDataManager.Instance.GetActivityData(DailySignModel.Instance.curSelectActivityID);
			topInfo.text = LanguageTextManager.GetString(activityConfig.RuleDescription);
		}
		void ShowRightBottom()
		{
			int classVocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
			int getRewardMark = DailySignModel.Instance.reachConditionMap[DailySignModel.Instance.curSelectActivityID];
			int selSub = DailySignModel.Instance.GetActiveRewardSub (DailySignModel.Instance.curSelectActivityID);
			int byIndex = 0;
			if (getRewardMark == 2) {
				byIndex = DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList[selSub].byIndex;
			} else {
				byIndex = DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList[selSub].byIndex+1;
			}
			//bool isGetRewardMark = DailySignModel.Instance.GetCurActShowIndex (DailySignModel.Instance.curSelectActivityID,ref byIndex);
			ShowIcon (activityConfig.rewardList[byIndex][classVocation]);
			string strDes = LanguageTextManager.GetString (activityConfig.QualifiedDescription);
			int curVal = DailySignModel.Instance.sActiveMsgInteract_OpenUI.activeList [selSub].dwActiveParam;
			int maxVal = activityConfig.qualifiedList[byIndex];
			if (DailySignModel.Instance.curSelectActivityID == DailySignModel.Instance.timerActivityID) {
				curVal = curVal/60;
			}
			if (curVal > maxVal) {
				curVal = maxVal;			
			}
			//玩家角色登录游戏达到{1}天。（{2}/{3}）
			conditionInfo.text = string.Format(strDes,maxVal,curVal,maxVal);
			SetGetRewardBtn (getRewardMark);
		}
		void SetGetRewardBtn(int getRewardMark)
		{
			if (getRewardMark == 2) {
				//领取过
				getRewardBtn.Enable = false;
				getRewardWord.text = LanguageTextManager.GetString("IDS_D22_11");
			} else if (getRewardMark == 1) {
				//可以领取
				getRewardBtn.Enable = true;
				getRewardWord.text = LanguageTextManager.GetString("IDS_D22_10");
			} else {
				//不能领取		
				getRewardBtn.Enable = false;
				getRewardWord.text = LanguageTextManager.GetString("IDS_D22_10");
			}
		}
		private List<Transform> listPoint = new List<Transform> ();
		private void ShowIcon(List<CGoodsInfo> listGoods)
		{
			string awardPoint = "AwardPoint";
			if (listGoods == null)
				return;
			if (listPoint.Count == 0) {
				listPoint.Add(pointParent.transform.Find(awardPoint+"0"));
				listPoint.Add(pointParent.transform.Find(awardPoint+"21"));
				listPoint.Add(pointParent.transform.Find(awardPoint+"22"));
				listPoint.Add(pointParent.transform.Find(awardPoint+"31"));
				listPoint.Add(pointParent.transform.Find(awardPoint+"33"));
			}
			foreach (Transform trans in listPoint) {
				DestroyAllChild(trans);		
			}
			if (listGoods.Count == 1) {
				CreateIcon(listPoint[0],listGoods[0]);
			} else if (listGoods.Count == 2) {
				CreateIcon(listPoint[1],listGoods[0]);
				CreateIcon(listPoint[2],listGoods[1]);
			} else {
				CreateIcon(listPoint[3],listGoods[0]);
				CreateIcon(listPoint[0],listGoods[1]);
				CreateIcon(listPoint[4],listGoods[2]);
			}

		}
		void DestroyAllChild(Transform trans)
		{
			int count = trans.childCount;
			for (int i = count - 1; i >= 0; i--) {
				Destroy(trans.GetChild(i).gameObject);
			}
		}
		private void CreateIcon(Transform pointParent,CGoodsInfo goodItem)
		{
			GameObject go = UI.CreatObjectToNGUI.InstantiateObj(rewardItemPrefab,pointParent);
			go.GetComponent<AwardItemIcon>().Show(goodItem.itemID,goodItem.itemCount);
		}
		void ShowLeftView()
		{
			//currentSelectIndex = 0;
			CreateGridItem ();
		}
		void ShowRigthView()
		{
			ShowRightTop ();
			ShowRightBottom ();
		}
		public override void Show(params object[] value)
		{
			base.Show ();
			ShowPanel ();
			LeftMoveShow (true);
			RightMoveShow (true);
			if (value != null && value.Length > 0) {
				SetSelectItem((int)value[0]);		
			}
		}
		void ShowPanel()
		{
			Init ();
			DailySignModel.Instance.JudgeReachCondition();
			ShowLeftView ();
			ShowRigthView ();
		}
		public void SetSelectItem(int activeID)
		{
			DailySignModel.Instance.curSelectActivityID = activeID;
			RightMoveShow (true);
			ShowPanel ();
		}
		//倒计时更新
		void OnActivityTimeUpdateHandler(object obj)
		{
			if (!IsShow)
				return;
			ShowPanel ();		
		}
		//领取活动奖励 应答
		void OnReceiveActivityGetRewardHandler(object obj)
		{
			if (!IsShow)
				return;
			SMsgInteract_GetReward_SC data = (SMsgInteract_GetReward_SC)obj;
			ActivityConfigData config = PlayerDataManager.Instance.GetActivityData(data.dwRewardID);
			int classVocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
			List<CGoodsInfo> goodsList = config.rewardList[data.byIndex][classVocation];
			SoundManager.Instance.PlaySoundEffect ("Sound_Button_GetReward_success");
			ShowPanel ();
		}
		//服务器主动下发数据更新
		void OnReceiveActivityUpdateHandler(object obj)
		{
			if (!IsShow)
				return;
			ShowPanel ();
		}
		void LeftMoveShow(bool isOpen)
		{
			if(isOpen)
				TweenPosition.Begin(leftObj,0.167f,leftOriPos-movePos,leftOriPos,null);
			else
				TweenPosition.Begin(leftObj,0.167f,leftOriPos,leftOriPos-movePos,ClosePanel);
		}
		void RightMoveShow(bool isOpen)
		{
			if(isOpen)
				TweenPosition.Begin(rightObj,0.167f,rightOriPos+movePos,rightOriPos);
			else
				TweenPosition.Begin(rightObj,0.167f,rightOriPos,rightOriPos+movePos);
		}
		void ClosePanel(object obj)
		{
			Close ();		
		}
		void OnClickGetRewardEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_GetReward");
			NetServiceManager.Instance.EntityService.SendGetRewardRequest();
		}
		void OnClickBackEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default_Close");
			LeftMoveShow (false);
			RightMoveShow (false);
			//Close ();
		}
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ActivityTimeUpdate,OnActivityTimeUpdateHandler);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ActivityRewardEvetn,OnReceiveActivityGetRewardHandler);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveActivityDataEvent,OnReceiveActivityUpdateHandler);
		}
		//新手引导
		private void TaskGuideBtnRegister()
		{
			getRewardBtn.gameObject.RegisterBtnMappingId(UIType.Activity, BtnMapId_Sub.Activity_GetReward);
			btnBack.gameObject.RegisterBtnMappingId(UIType.Activity, BtnMapId_Sub.Activity_BtnBack);
		}
	}
}
