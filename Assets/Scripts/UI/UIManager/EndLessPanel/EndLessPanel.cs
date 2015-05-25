using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{
	public class EndLessPanel : BaseUIPanel
	{
		public GameObject leftEffPrefab;
		public GameObject leftEffParent;
		public SingleButtonCallBack btnBack;
		public CommonPanelTitle comPanelTitle;
		public GameObject leftObj;
		public GameObject effectParent;
		public UILabel effectLabel;
		public EndLessRankPanel rankPanel;
		public GameObject rigthObj;
		private EndLessBestPanel bestPanel;


		#region 初始化信息
		private bool isReady = false;
		void Init()
		{
			if (isReady)
				return;
			isReady = true;
			InitData ();
			rankPanel.Init ();
			bestPanel.Init ();
			RegisterEvent ();
			EctypeModel.Instance.GetEndLessID ();
			//comPanelTitle.Init (CommonTitleType.GoldIngot,CommonTitleType.Power);
			TaskGuideBtnRegister();
		}
		/// <summary>
		/// 引导按钮注入代码
		/// </summary>
		private void TaskGuideBtnRegister()
		{
			btnBack.gameObject.RegisterBtnMappingId(UIType.Trial, BtnMapId_Sub.Trial_Back);
			comPanelTitle.GoldMoneyLabel.gameObject.RegisterBtnMappingId(UIType.Trial, BtnMapId_Sub.Trial_BuyIngot);
			comPanelTitle.CopperLabel.gameObject.RegisterBtnMappingId(UIType.Trial, BtnMapId_Sub.Trial_BuyActivity);
		}
		void InitData()
		{
			effectLabel.text=LanguageTextManager.GetString("IDS_I20_3");
			bestPanel = rigthObj.GetComponent<EndLessBestPanel>();
			UI.CreatObjectToNGUI.InstantiateObj (leftEffPrefab,leftEffParent.transform);
			originalLeftPos = leftObj.transform.localPosition;
			originalRightPos = rigthObj.transform.localPosition;
		}
		void RegisterEvent()
		{
			btnBack.SetCallBackFuntion(OnBtnBackClick);
		}
		#endregion


		//每次界面打开时
		public override void Show (params object[] value)
		{
			base.Show (value);
			Init ();
			ShowPanel ();
		}
		void ShowPanel()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EndlessAppear");
			rankPanel.Show ();
			bestPanel.Show ();
			TweenShow();
			comPanelTitle.TweenShow ();
		}
		#region 事件响应
		void OnBtnBackClick(object obj)
		{
			Close ();
			//UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UIType.HeroInfo);
		}
		public override void Close()
		{
			if (!IsShow)
				return;
			base.Close();
			TweenClose();
			comPanelTitle.tweenClose ();
		}

		private Vector3 originalLeftPos ;
		private Vector3 originalRightPos ;
		private Vector3 movePos = new Vector3(51,0,0);
		private float moveTime = 0.167f;
		public void TweenShow()
		{
			TweenPosition.Begin(leftObj,moveTime,originalLeftPos-movePos,originalLeftPos);
			TweenPosition.Begin(rigthObj,moveTime,originalRightPos+movePos,originalRightPos);
		}
		
		public void TweenClose()
		{
			TweenPosition.Begin(leftObj,moveTime,originalLeftPos,originalLeftPos-movePos);
			TweenPosition.Begin(rigthObj,moveTime,originalRightPos,originalRightPos+movePos);
		}
		#endregion
	}
}
