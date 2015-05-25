using UnityEngine;
using System.Collections;
namespace UI.MainUI
{
public class TaskGuideItem : MonoBehaviour {
		public GameObject mainTaskEff;
		public GameObject task;
		public GameObject newFun;
		public UILabel labelNewFun;
		public SpriteSwith taskBg;
		public SpriteSwith taskMark;
		public UILabel taskName;
		public UILabel taskText;
		[HideInInspector]
		public ETaskShowType eTaskShowType = ETaskShowType.EMainTask;
		[HideInInspector]
		public TaskState curTaskState;
		private TaskGuidePanel myParent;
		private bool isRead = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;

		}
		public void OpenEff()
		{
			if (mainTaskEff != null) {
				mainTaskEff.SetActive(true);			
			}
		}
		public void CloseEff()
		{
			if (mainTaskEff != null) {
				mainTaskEff.SetActive(false);			
			}	
		}
		public void Show(TaskState taskState,ETaskShowType showType,TaskGuidePanel myParent)
		{
			gameObject.SetActive (true);
			curTaskState = taskState;
			this.eTaskShowType = showType;
			this.myParent = myParent;
			Init ();
			ShowTask();
		}
		void ShowTask()
		{
			if (mainTaskEff != null) {
				mainTaskEff.SetActive(false);			
			}
			string colorText = "[fffa6f]";
			if (eTaskShowType != ETaskShowType.ENewFunction) {
				task.SetActive(true);
				newFun.SetActive(false);
				taskMark.ChangeSprite(curTaskState.TaskNewConfigData.TaskSeries-1);
				if (eTaskShowType == ETaskShowType.EUnMainTask)
				{
					colorText = "[67b9ff]";
				}
				else
				{
					if (mainTaskEff != null) {
						mainTaskEff.SetActive(true);			
					}
				}
				taskName.text = colorText+LanguageTextManager.GetString(curTaskState.TaskNewConfigData.TaskTitle);
				taskText.text = LanguageTextManager.GetString(curTaskState.TaskNewConfigData.TaskGoals);
			} else {
				task.SetActive(false);
				newFun.SetActive(true);
				labelNewFun.text = LanguageTextManager.GetString(TaskModel.Instance.GetTaskNewFunction());
//				taskMark.ChangeSprite(7);
//				colorText = "[fc6cfa]";
//				string newFunName = TaskModel.Instance.GetTaskNewFunction();
//				taskName.text = colorText+LanguageTextManager.GetString(newFunName);
//				taskText.text = string.Format(LanguageTextManager.GetString("IDS_I16_2"),LanguageTextManager.GetString(newFunName));
			}
		}
		void OnPress(bool isPress)
		{
			if (eTaskShowType == ETaskShowType.ENewFunction) {
				return;
			}
			if (isPress) {
				taskBg.ChangeSprite (2);			
			} else {
				taskBg.ChangeSprite(1);			
			}
		}
		void OnClick ()				
		{ 
			if (eTaskShowType != ETaskShowType.ENewFunction) {
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Task_QuickGuide");
				TaskModel.Instance.ManualTriggerTask(curTaskState,true);
			}
		}
}
}
