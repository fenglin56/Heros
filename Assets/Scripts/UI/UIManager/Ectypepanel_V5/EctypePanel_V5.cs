using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI{

    public class EctypePanel_V5 : BaseUIPanel
    {
		public SingleButtonCallBack BackButton;

		public EctypeDiffListPanel m_EctypeDiffListPanel;//左侧难度列表
		public EctypeContainerListPanel m_EctypeContainerListPanel;//右侧副本容器列表和宝箱
		public BaseCommonPanelTitle m_CommonPanelTitle;
		//public SMSGEctypeSelect_SC UnlockEctypeData{get;private set;}
		//public EctypeSelectConfigData
		public EctypePanleManager MyParent{get;private set;}
		[HideInInspector]
		public int jumpPointEctypeID = -1;
		bool isRead = false;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			BackButton.SetCallBackFuntion(OnBackBtnClick);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeNormalDataUpdate, OnEctypeNormalOpenListUpdateEvent);
            TaskGuideBtnRegister();
            m_CommonPanelTitle.Init (CommonTitleType.Power,CommonTitleType.GoldIngot);
			
		}
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
             BackButton.gameObject.RegisterBtnMappingId(UIType.Battle, BtnMapId_Sub.Battle_Back);
        }
		//刷新解锁列表界面
		private void OnEctypeNormalOpenListUpdateEvent(object obj)
		{
			if (!gameObject.activeInHierarchy)
				return;
			bool isOpenSweep = (bool)obj;
			UpdatePanel(isOpenSweep);
		}
		private void UpdatePanel(bool isOpenSweep)
		{
			gameObject.SetActive(true);
			//UnlockEctypeData = EctypeModel.Instance.sMSGEctypeSelect_SC;
			if(!isOpenSweep)
				StartCoroutine(m_EctypeDiffListPanel.Init(EctypeModel.Instance.sMSGEctypeSelect_SC,this,-1));
		}
		public void Show(SMSGEctypeSelect_SC unlockEctypeData,EctypePanleManager myParent,int ectypeID)
		{
			Init ();
			MyParent = myParent;
			gameObject.SetActive(true);
			jumpPointEctypeID = ectypeID;
			//UnlockEctypeData = unlockEctypeData;
			StartCoroutine( m_EctypeDiffListPanel.Init(unlockEctypeData,this,ectypeID));
			//m_EctypeContainerListPanel.TweenShow();
			//Invoke ("WaitTweenFun",0.13f);
			m_CommonPanelTitle.TweenShow();
			//SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EctypeUIAppear");
		}
		void WaitTweenFun()
		{
			m_EctypeContainerListPanel.TweenShow();
		}
		public void OnEctypdDiffItemSelect(SingleEctypeDiffItem selectItem)
		{
			m_EctypeContainerListPanel.Show(selectItem.MyConfigData,this);
			if (jumpPointEctypeID != -1) {
				jumpPointEctypeID = -1;			
			}
			m_EctypeContainerListPanel.TweenShow();
		}
		//获取区域副本ID
		public int GetEctypeDiffID(int ectypeContaienrID)
		{
			//TraceUtil.Log(SystemModel.Jiang,"GetEctypeDiff,ContaienrID:"+ectypeContaienrID);
			var ectypeDiffData = EctypeConfigManager.Instance.EctypeSelectConfigFile._dataTable.
				SingleOrDefault(C=>C._vectContainer.FirstOrDefault(p=>p==ectypeContaienrID)!=0||
				      C.Difficult2Container.FirstOrDefault(P=>P==ectypeContaienrID)!=0);
			if (ectypeDiffData == null) {
				TraceUtil.Log("ECTYPE is error!!");	
			}
			return ectypeDiffData._lEctypeID;
		}

		public void OnBackBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_EctypeBack");
			m_EctypeContainerListPanel.TweenClose();
			m_CommonPanelTitle.tweenClose();
			m_EctypeDiffListPanel.TweeenClose();
			DoForTime.DoFunForTime(0.3f,ClosePanel,null);
		}

		void ClosePanel(object obj)
		{
			gameObject.SetActive(false);
			MyParent.OnClosePanelBtnClick();
		}

		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeNormalDataUpdate, OnEctypeNormalOpenListUpdateEvent);
		}
	}

}