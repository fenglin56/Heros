using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UI.MainUI
{
	public enum ETaskShowType
	{
		EMainTask = 0,
		EUnMainTask,
		ENewFunction,
	}
	public class TaskGuidePanel : BaseUIPanel {
		public GameObject moveObj;
		public SingleButtonCallBack dicBtn;
		public GameObject dicFg;
		public SingleButtonCallBack taskBtn;
		public List<TaskGuideItem> taskList;
		private int taskCount = 0;
		private bool isRead = false;
		private bool isCurClose = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			dicBtn.SetCallBackFuntion (OnDicBtnEvent);
			taskBtn.SetCallBackFuntion (OnTaskBtnEvent);
			taskBtn.gameObject.RegisterBtnMappingId(UIType.Task, BtnMapId_Sub.Empty);
			dicBtn.gameObject.RegisterBtnMappingId(UIType.NewbieGuide, BtnMapId_Sub.NewbieGuide_Arrow);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.NpcTalkTaskDealUI, OnNpcTalkDealUI);

			UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcTalkOpenEvent, OnOpenTalkUIEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcTalkCloseEvent, OnCloseTalkUIEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcGuideStartEvent, OnOpenTalkUIEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OnNpcGuideStopEvent, OnCloseTalkUIEvent);
		}
		
		public override void Show(params object[] value)
		{
			isCurClose = false;
			Init ();
			GetTask ();
			TweenShow ();
		}
		public void ShowPanel()
		{
			Init ();
			if (!isCurClose) {
				GetTask ();		
			}
		}
		void GetTask()
		{
			taskCount = 0;
			TaskState curTaskState = TaskModel.Instance.MainTaskState;
			if (curTaskState == null) {
				//Debug.LogError("main task is null!!!!!!!!!");
				return;
				}
			taskList [taskCount++].Show (curTaskState,ETaskShowType.EMainTask,this);
			curTaskState = null;
			curTaskState = TaskModel.Instance.GetShowUnMainTask ();
			if (curTaskState != null) {
				taskList [taskCount++].Show (curTaskState,ETaskShowType.EUnMainTask,this);
				curTaskState = TaskModel.Instance.GetShowUnMainTask (curTaskState.dwTaskID);
				if (curTaskState != null) {
					taskList [taskCount++].Show (curTaskState,ETaskShowType.EUnMainTask,this);	
				}
			}
			string newFun = TaskModel.Instance.GetTaskNewFunction();
			if (!newFun.Equals ("0")) {
				taskList [taskCount++].Show (curTaskState,ETaskShowType.ENewFunction,this);		
			}
			for (int i = taskCount; i < 4; i++) {
				taskList [i].gameObject.SetActive(false);
			}
		}
		void DicShow()
		{
			if (isCurClose) {
				dicFg.transform.localRotation = new Quaternion(0,180,0,1);// Euler(new Vector3(0,180,0));
			} else {
				dicFg.transform.localRotation = new Quaternion(0,0,0,1);//Quaternion.Euler(new Vector3(0,0,0));
			}
		}
		void MoveShow()
		{
			if (isCurClose) {
				TweenClose();
			} else {
				TweenShow();
			}
		}
		void OnDicBtnEvent(object obj)
		{
			isCurClose = !isCurClose;
			if (isCurClose) {
				SoundManager.Instance.PlaySoundEffect("Sound_Button_TaskPanel_Close");
						} else {
				SoundManager.Instance.PlaySoundEffect("Sound_Button_TaskPanel_Open");
			}
			DicShow ();
			MoveShow ();
		}
		void OnTaskBtnEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Task_Enter");
			MainUIController.Instance.OpenMainUI (UI.MainUI.UIType.Task);
		}
		public override void Close()
		{
			if (!IsShow)
				return;
			TweenClose ();
			base.Close();
		}
		float moveTime = 0.17f;
		Vector3 originalPos = Vector3.zero;
		Vector3 movePos = new Vector3(2.17f,0,0);
		public void TweenShow()
		{
			TweenPosition.Begin(moveObj,moveTime,originalPos-movePos,originalPos,null);
			TweenAlpha.Begin (moveObj,moveTime,0,1,null);
			foreach (TaskGuideItem item in taskList) {
				item.OpenEff();			
			}
		}
		
		public void TweenClose()
		{
			TweenPosition.Begin(moveObj,moveTime,originalPos,originalPos-movePos,null);
			TweenAlpha.Begin (moveObj,moveTime,1,0,null);
			foreach (TaskGuideItem item in taskList) {
				item.CloseEff();			
			}
		}
		void OnNpcTalkDealUI(object obj)
		{
			bool isShow = (bool)obj;
			if (isShow) {
				gameObject.SetActive(true);
			} else {
				gameObject.SetActive(false);
			}
		}
		void OnOpenTalkUIEvent(object obj)
		{
			gameObject.SetActive(false);
		}
		void OnCloseTalkUIEvent(object obj)
		{
			gameObject.SetActive(true);
		}

		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NpcTalkTaskDealUI, OnNpcTalkDealUI);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcTalkOpenEvent, OnOpenTalkUIEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcTalkCloseEvent, OnCloseTalkUIEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcGuideStartEvent, OnOpenTalkUIEvent);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnNpcGuideStopEvent, OnCloseTalkUIEvent);
		}
	}
}