using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI{

	public class SingleTreasureChestsItem : MonoBehaviour {
		public enum TreasureItemStatus{isOpended,CanOpend,Close}

		public UISlider SliderBar;
		public UILabel GetStarNumLabel;
		public SpriteSwith BackgroundSwith;
		public GameObject EnabelEffect;
		public Transform EffectPos;

		public EctypeContainerListPanel MyParent{get;private set;}
		public int CurrentStarNum;
		public int curFullStarNum;
		void Awake()
		{
			UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeChessDataUpdate, OnEctypeChessUpdateEvent);
            TaskGuideBtnRegister();
		}
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            //SliderBar.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_Difficulty01);
        }
		//刷新宝箱开启界面
		private void OnEctypeChessUpdateEvent(object obj)
		{
			if (!gameObject.activeInHierarchy)
				return;
			UpdateStatus (MyParent);
		}
		/*private bool isInEctypeContainer(int ectypeID)
		{
				
		}*/
		public void UpdateStatus(EctypeContainerListPanel myParent)
		{
			this.MyParent = myParent;
			CurrentStarNum = 0;

			foreach(var child in EctypeModel.Instance.sMSGEctypeSelect_SC.sMSGEctypeData_SCs)
			{
				if(myParent.myEctypeContainerIDList.Contains((int)child.dwEctypeContaienrID) && child.byGrade == 6)//sss评分
				{
					CurrentStarNum++;
				}
			}
			curFullStarNum = myParent.EctypeSelectData.Difficult2Container.Count+myParent.EctypeSelectData._vectContainer.Length;
			GetStarNumLabel.SetText(string.Format("{0}/{1}",CurrentStarNum,curFullStarNum));
			SliderBar.sliderValue = CurrentStarNum/(float)curFullStarNum;
			EffectPos.ClearChild();
			switch (GetTreausreChestsStatus())
			{
			case TreasureItemStatus.Close:
				BackgroundSwith.ChangeSprite(1);
				break;
			case TreasureItemStatus.isOpended:
				BackgroundSwith.ChangeSprite(2);
				break;
			case TreasureItemStatus.CanOpend:
				BackgroundSwith.ChangeSprite(1);
				CreatObjectToNGUI.InstantiateObj(EnabelEffect,EffectPos);
				break;
			}
		}

		TreasureItemStatus GetTreausreChestsStatus()
		{
			TreasureItemStatus getStatus = TreasureItemStatus.Close;
			SMSGEctypeChest_SC sMSGEctypeChest_SC = EctypeModel.Instance.sMSGEctypeSelect_SC.SMSGEctypeChestStatus.FirstOrDefault(C=>C.dwEctypeID == MyParent.EctypeSelectData._lEctypeID);
			if(sMSGEctypeChest_SC.dwEctypeID == 0)
			{
				getStatus = TreasureItemStatus.Close;
			}else if(sMSGEctypeChest_SC.byHasOpen == 0&&CurrentStarNum==curFullStarNum)
			{
				getStatus = TreasureItemStatus.CanOpend;
			}else if(sMSGEctypeChest_SC.byHasOpen == 1)
			{
				getStatus = TreasureItemStatus.isOpended;
			}
			return getStatus;
		}

		void OnClick()
		{
//			if(GetTreausreChestsStatus()!= TreasureItemStatus.CanOpend)
//				return;
			SoundManager.Instance.PlaySoundEffect("Sound_Button_EctypeBox");
			MyParent.m_TreasureChestsPanel.TweenShow(GetTreausreChestsStatus()== TreasureItemStatus.CanOpend);
		}
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeChessDataUpdate, OnEctypeChessUpdateEvent);
		}
	}
}