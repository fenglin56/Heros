using UnityEngine;
using System.Collections;
using UI.MainUI;

public class DailyTaskDataManager : MonoBehaviour 
{
	private static DailyTaskDataManager m_instance;
	public static DailyTaskDataManager Instance { get { return m_instance; } }
	public DailyTaskRewardConfigDataBase dailyTaskRewardConfigDataBase;

	void Awake()
	{
		m_instance = this;
	}

	void Start()
	{
		NetServiceManager.Instance.EquipStrengthenService.SendRequestActiveChestProgressCommand();//获取奖励进度
	}

	void OnDestroy()
	{
		m_instance = null;
	}

	public DailyTaskRewardConfigData[] GetDailyTaskRewardConfigArray()
	{
		return dailyTaskRewardConfigDataBase._dataTable;
	}

	public void CheckNewChest(int rewardProcess)
	{
		var playerData = PlayerManager.Instance.FindHeroDataModel();
		var dailyTaskRewardConfigData = dailyTaskRewardConfigDataBase._dataTable;
		for(int i=0;i<dailyTaskRewardConfigData.Length;i++)
		{
			if(playerData.PlayerValues.PLAYER_FIELD_ACTIVE_VALUE <= dailyTaskRewardConfigData[i]._requirementActiveValue)
			{
				if(rewardProcess == 0)
				{
					//UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim,UIType.DailyTaskPanel);
					return;
				}
				if(rewardProcess < dailyTaskRewardConfigData[i]._boxSequence)
				{
					
					//UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim,UIType.DailyTaskPanel);
				}
				else
				{
					//UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim,UIType.DailyTaskPanel);
				}
				break;
			}
		}
	}
}
