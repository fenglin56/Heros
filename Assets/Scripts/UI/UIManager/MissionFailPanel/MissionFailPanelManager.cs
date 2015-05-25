using UnityEngine;
using System.Collections;

namespace UI.MainUI
{
	
	public class MissionFailPanelManager : MonoBehaviour
	{
		public GameObject MissionFailPanelPrefab;
		BattleFailPanel battleFailPanel;
		MissionFailPanel MissionFailPanel;
		
		bool WaitTimeToShow = false;
		bool CheckShow = true;
		
		void Awake()
		{
			//GameDataManager.Instance.ResetData(DataType.MissionFail,"Fail");//Test
			//UIEventManager.Instance.RegisterUIEvent(UIEventType.ShowMissionFailPanelLate, ShowMissionFailPanelLate);
			
			//DoForTime.DoFunForFrame(60,CheckShowPanel, null);
		}
		void Start()
		{
			Invoke ("WaitFunction",1);
		}
		void WaitFunction()
		{
			DealBattleFailPanel ();
			SkillModel.Instance.DealSkillAdUpStrengthen();		
		}
		void OnDestroy()
		{
			//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ShowMissionFailPanelLate, ShowMissionFailPanelLate);
		}
		
		void ShowMissionFailPanelLate(object obj)
		{
			
			/*TraceUtil.Log("等待一段时间再检测是否显示失败建议面板");
            WaitTimeToShow = true;*/
		}
		void DealBattleFailPanel()
		{
			var failData = GameDataManager.Instance.GetData(DataType.MissionFail);
			if (failData != null) {
				//把当前任务要清空下，因为战斗失败//
				UIEventManager.Instance.TriggerUIEvent(UIEventType.ClickOtherButton, 0);
			}
			int currentLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
			if (failData != null && currentLevel >= CommonDefineManager.Instance.CommonDefine.Lose_BeShow) 
			{
				if (GameManager.Instance.ectypeType == EEcytpeBattleType.EEctypeNormal || GameManager.Instance.ectypeType == EEcytpeBattleType.EEctypeCaptical ||
				    GameManager.Instance.ectypeType == EEcytpeBattleType.EEctypeDefence) {
					if (battleFailPanel == null)
					{
						battleFailPanel = CreatObjectToNGUI.InstantiateObj(MissionFailPanelPrefab, transform).GetComponent<BattleFailPanel>();
					}
					battleFailPanel.Show();
				}
			}
		}
		
		void CheckShowPanel(object obj)
		{
			//TraceUtil.Log("检测是否显示失败面板指示:" + GuideBtnManager.Instance.IsEndGuide);
			var failData = GameDataManager.Instance.GetData(DataType.MissionFail);
			if (!CheckShow || TaskModel.Instance.TaskGuideType==TaskGuideType.Enforce)// NewbieGuideManager_V2.Instance.IsConstraintGuide == true)//已经显示过或者强引导
			{
				TraceUtil.Log("没有失败结算，无需显示");
				return;
			}
			CheckShow = false;
			if (failData != null)
			{
				if (WaitTimeToShow)
				{
					DoForTime.DoFunForTime(5, Show, null);
				}
				else
				{
					Show(null);
				}
			}
		}
		
		void Show(object obj)
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UIType.Empty);
			TraceUtil.Log("显示失败建议面板");
			if (MissionFailPanel == null)
			{
				MissionFailPanel = CreatObjectToNGUI.InstantiateObj(MissionFailPanelPrefab, transform).GetComponent<MissionFailPanel>();
			}
			MissionFailPanel.Show();
		}
		
	}
}