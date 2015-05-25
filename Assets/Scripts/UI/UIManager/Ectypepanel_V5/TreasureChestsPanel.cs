using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class TreasureChestsPanel : BaseTweenShowPanel {

		public SingleButtonCallBack CloseButton;
		public SingleButtonCallBack SureButton;
		public UILabel DesLabel;
		public SingleTreasureChestsGetItem[] TreasureChestsItemList;
		EctypeRewardItem[] RewardItemList;

		public EctypeContainerListPanel MyParent{get;private set;}

		void Awake()
		{
			SureButton.SetCallBackFuntion(OnSureButtonClick);
			CloseButton.SetCallBackFuntion(OnCloseBtnClick);
			Close();
			DesLabel.SetText(LanguageTextManager.GetString("IDS_I5_6"));
			UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenTreasure,ShowGetItemTips);

            TaskGuideBtnRegister();
		}
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
			CloseButton.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_TreaureChestItem_Close);
			SureButton.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_TreaureChestItem_Get);
        }
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenTreasure,ShowGetItemTips);
		}

		public void Init(EctypeContainerListPanel myParent)
		{
			MyParent = myParent;
		}

		void OnCloseBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_EctypeBoxClose");
			TweenClose();
		}
		//领取奖励成功//
		void ShowGetItemTips(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EctypeGetAward");
			RewardItemList.ApplyAllItem(C=>GoodsMessageManager.Instance.Show(C.ItemID,C.ItemNum));
		}

		void OnSureButtonClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_EctypeOpenBoxAward");
			NetServiceManager.Instance.EctypeService.SendSmsgOpenTreasureInTown(MyParent.EctypeSelectData._lEctypeID);
			TweenClose();
		}

		void SetSureBtnActive(bool flag)
		{
			SureButton.SetMyButtonActive(flag);
			//SureButton.gameObject.GetComponent<BoxCollider>().enabled = flag;
			SureButton.SetButtonBackground(flag?1:2);
		}

		public void TweenShow (bool canOpen)
		{
			SetSureBtnActive(canOpen);
			var selectEctypeData = MyParent.EctypeSelectData;
			RewardItemList = selectEctypeData.AwardItem;
			for(int i = 0;i<TreasureChestsItemList.Length;i++)
			{
				if(RewardItemList.Length>i)
				{
					TreasureChestsItemList[i].gameObject.SetActive(true);
					TreasureChestsItemList[i].Init(RewardItemList[i]);
				}else
				{
					TreasureChestsItemList[i].gameObject.SetActive(false);
				}
			}
			base.TweenShow ();
		}


	}
}