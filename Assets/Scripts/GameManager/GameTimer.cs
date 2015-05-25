using UnityEngine;
using System.Collections;

public class GameTimer : View {
	void Start()
	{
		UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveActivityDataEvent,ActivityTimerStart);
		//AddEventHandler(EventTypeEnum.PlayerLevelUpdate.ToString(), OnPlayerLvUpdate);
		//AddEventHandler(EventTypeEnum.PlayerHoldMoneyUpdate.ToString(), OnHoldMoneyUpdate);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.PlayerLevelUpdate,OnPlayerLvUpdate);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.PlayerHoldMoneyUpdate,OnHoldMoneyUpdate);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods,CheckGetItemEvent);
	}

	#region activity
	void ActivityTimerStart(object obj)
	{
		//活动倒计时
		if (IsInvoking ("ActivityUpdate")) {
			CancelInvoke("ActivityUpdate");		
		}
		InvokeRepeating("ActivityUpdate", 0.5f, 30f);
	}
	private void ActivityUpdate()
	{
		DailySignModel.Instance.ActivityTimeUpdate ();
	}
	void OnPlayerLvUpdate(object obj)
	{
		DailySignModel.Instance.OnPlayerUpdate ();
		if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.CheckNewItem,null);
			SkillModel.Instance.DealSkillAdUpStrengthen();	
		}
	}
	void OnHoldMoneyUpdate(object obj)
	{
		if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN) {
			SkillModel.Instance.DealSkillAdUpStrengthen ();
		}
	}
	void CheckGetItemEvent(object obj)
	{
		if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
		{
			SkillModel.Instance.DealSkillAdUpStrengthen();	
		}
	}
	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveActivityDataEvent,ActivityTimerStart);
		//RemoveEventHandler(EventTypeEnum.PlayerLevelUpdate.ToString(), OnPlayerLvUpdate);
		//RemoveEventHandler(EventTypeEnum.PlayerHoldMoneyUpdate.ToString(), OnHoldMoneyUpdate);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.PlayerLevelUpdate,OnPlayerLvUpdate);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.PlayerHoldMoneyUpdate,OnHoldMoneyUpdate);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods,CheckGetItemEvent);
	}
	protected override void RegisterEventHandler()
	{

	}
	#endregion
}
