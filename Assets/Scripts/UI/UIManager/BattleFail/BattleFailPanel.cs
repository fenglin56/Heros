using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UI.MainUI
{
	public enum EFailType{
		EEquipBtn=0,
		EGemBtn,
		ESirenBtn,
		//技能
		EEsolericaBtn,
		EShopBtn
	}
	public class BattleFailPanel : BaseUIPanel {
		public SingleButtonCallBack backBtn;
		public UILabel titleInfo;
		public UILabel bottomInfo;
		public List<BattleFailItem> itemList = new List<BattleFailItem> ();
		private bool isRead = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			titleInfo.text = LanguageTextManager.GetString ("IDS_I35_1");
			bottomInfo.text = LanguageTextManager.GetString ("IDS_I35_3");
			for (int i = 0 ; i < itemList.Count; i++) {
				itemList[i].gameObject.GetComponent<SingleButtonCallBack>().SetCallBackFuntion(OnClickBtnEvent,(EFailType)i);
				int index = (int)BtnMapId_Sub.BattleFail_Equip+i;
				itemList[i].gameObject.gameObject.RegisterBtnMappingId(UIType.BattleFail, (BtnMapId_Sub)index);
			}
			backBtn.SetCallBackFuntion (OnBackEvent);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseAllUI, CloseUIHandle);
			backBtn.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.BattleFail_Close);
		}
		public override void Show(params object[] value)
		{
			base.Show ();
			Init ();
			ShowPanel ();
			if (TaskModel.Instance.CurrentTaskId.HasValue) {
				var runningTaskConfigData=TaskModel.Instance.FindRuningTaskState();
				//关闭所有界面//
				if (runningTaskConfigData.TaskNewConfigData.CloseUI == 1)//收起主功能按钮
				{
					Close();
					return;
				}
			}
		}
		void ShowPanel()
		{
			for (int i = 0 ; i < itemList.Count; i++) {
				itemList[i].Show((EFailType)i);			
			}
		}
		void OnClickBtnEvent(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default_Open");
			EFailType type = (EFailType)obj;
			UIType uitype = UIType.Package;
			switch (type) {
			case EFailType.EEquipBtn:
				MainUIController.Instance.OpenMainUI (UIType.EquipmentUpgrade);
				break;
			case EFailType.EGemBtn:
				MainUIController.Instance.OpenMainUI (UIType.Gem);
				break;
			case EFailType.ESirenBtn:
				MainUIController.Instance.OpenMainUI (UIType.Siren,0);
				break;
			case EFailType.EEsolericaBtn:
				MainUIController.Instance.OpenMainUI (UIType.Skill);
				break;
			case EFailType.EShopBtn:
				MainUIController.Instance.OpenMainUI (UIType.CarryShop);
				break;
			}
			CloseUIHandle (null);
		}
		void OnBackEvent(object obj)
		{
			CloseUIHandle (null);
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default_Close");
		}
		void CloseUIHandle(object obj)
		{
			Close ();
		}
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseAllUI, CloseUIHandle);
		}
	}
}