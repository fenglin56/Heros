using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;

namespace UI.Team
{
	public class SelectAreaPanel_V2 : MonoBehaviour 
	{

		public enum PanelType
		{
			Filter = 0,
			Create,
		}

		public GameObject EctypeCardItemPrefab;
		public GameObject Child;

		public UIDraggablePanel DraggablePanel;
		public UIGrid Grid;

		public LocalButtonCallBack Button_Cancle;
		public LocalButtonCallBack Button_Sure;

		private bool m_isFreeze = false;
		private Dictionary<int, EctypeCardItem_V2> m_EctypeCardDict = new Dictionary<int, EctypeCardItem_V2>();

		public GameObject BattleEffectPrefab;
		private GameObject m_BattleEffect;

		public GameObject EctypeSelectFramePrefab;
		private GameObject m_SelectFrame;

		private int m_EctypeID;
		public int EctypeID
		{
			set {m_EctypeID = value;}
			get 
			{
				if(m_EctypeID == 0)
				{
					m_EctypeID = TeamManager.Instance.CurSelectEctypeAreaData._lEctypeID;
				}
				return m_EctypeID;
			}
		}

		private PanelType m_PanelType;

		void Awake()
		{
//			var ectypeSelectConfigList = EctypeConfigManager.Instance.EctypeSelectConfigList.Values;
//			ectypeSelectConfigList.ApplyAllItem(p=>{
//				GameObject ectypeCard = CreatObjectToNGUI.InstantiateObj(EctypeCardItemPrefab,Grid.transform);
//				EctypeCardItem_V2 ectypeCardItem_V2 = ectypeCard.GetComponent<EctypeCardItem_V2>();
//				ectypeCardItem_V2.Init( EctypeCardItem_V2.CardType.Area, p, OnSelectCardHandle );
//				m_EctypeCardDict.Add(p._lEctypeID, ectypeCardItem_V2);
//			});

			if(EctypeModel.Instance.sMSGEctypeSelect_SC.nEctypeCount == 0)
			{
				return;
			}

			//筛选区域
			List<int> areaIDList = new List<int>();
			EctypeModel.Instance.sMSGEctypeSelect_SC.sMSGEctypeData_SCs.ApplyAllItem(p=>{
				if(EctypeConfigManager.Instance.EctypeContainerConfigList[(int)p.dwEctypeContaienrID].AllowCreatTeam == 1)//判断是否多人副本
				{
					int areaID = EctypeConfigManager.Instance.GetSelectContainerID((int)p.dwEctypeContaienrID);
					if(areaID != 0 && !areaIDList.Contains(areaID))
					{
						areaIDList.Add(areaID);
					}
				}
			});
			//创建
			int num = 0;
			areaIDList.ApplyAllItem(p=>{
				num++;
				var ectypeSelectConfig = EctypeConfigManager.Instance.EctypeSelectConfigList[p];
				GameObject ectypeCard = CreatObjectToNGUI.InstantiateObj(EctypeCardItemPrefab,Grid.transform);
				EctypeCardItem_V2 ectypeCardItem_V2 = ectypeCard.GetComponent<EctypeCardItem_V2>();
				ectypeCardItem_V2.Init( EctypeCardItem_V2.CardType.Area, ectypeSelectConfig, OnSelectCardHandle );

				ectypeCard.RegisterBtnMappingId(ectypeSelectConfig._lEctypeID,UIType.TeamInfo, BtnMapId_Sub.TeamInfo_ChangeZone_Confirm);

				int type = num % 3;
				if(type == 1)
				{
					ectypeCardItem_V2.ShowLeftTip(true);
				}
				else if(type == 0)
				{
					ectypeCardItem_V2.ShowRightTip(true);
				}
				m_EctypeCardDict.Add(p, ectypeCardItem_V2);
			});

			Grid.Reposition();

			Button_Cancle.SetCallBackFuntion(OnCancleClick, null);
			Button_Sure.SetCallBackFuntion(OnSureClick, null);

			TaskGuideBtnRegister();
		}

		/// <summary>
		/// 引导按钮注入代码
		/// </summary>
		private void TaskGuideBtnRegister()
		{
			Button_Sure.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_ChangeZone_Confirm);
			Button_Cancle.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_ChangeZone_Cancel);		
		}

		void OnSelectCardHandle(int ectypeID)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamReplaceAreaChoice");
			m_EctypeID = ectypeID;
			if(m_SelectFrame != null)
				Destroy( m_SelectFrame);
			m_SelectFrame = CreatObjectToNGUI.InstantiateObj(EctypeSelectFramePrefab,Grid.transform);
			m_SelectFrame.transform.localPosition = m_EctypeCardDict[ectypeID].transform.localPosition;
			//Button_Sure.SetButtonActive(true);
			//Button_Sure.SetEnabled(true);
			//Button_Sure.GetComponent<UIImageButton>().isOn = true;
			StartCoroutine("LateSetButtonEnable",true);
			if(m_BattleEffect == null)
			{
				m_BattleEffect = CreatObjectToNGUI.InstantiateObj(BattleEffectPrefab,Button_Sure.transform);
			}
		}
		IEnumerator LateSetButtonEnable(bool isFlag)
		{
			yield return new WaitForEndOfFrame();
			Button_Sure.SetEnabled(isFlag);
		}

		public void Show(PanelType type)
		{
			m_PanelType = type;
			if(m_SelectFrame != null)
				Destroy( m_SelectFrame);
			//Button_Sure.SetButtonActive(false);
			//Button_Sure.SetEnabled(false);
			//Button_Sure.GetComponent<UIImageButton>().isOn = false;
			StartCoroutine("LateSetButtonEnable",false);
			if(m_BattleEffect!= null)
			{
				Destroy(m_BattleEffect);
			}
			PlayAppearAnimation();
			transform.localPosition = Vector3.zero;
		}

		public void Close()
		{
			transform.localPosition = Vector3.back * 800;
		}

		//出现动画
		private void PlayAppearAnimation()
		{
			m_isFreeze = true;
			Child.transform.localScale = Vector3.one * 0.1f;
			TweenScale.Begin(Child, 0.2f, Vector3.one * 0.1f, Vector3.one, UnfreezeHandle);
		}
		//消失动画
		private void PlayDisappearAnimation()
		{
			TweenScale.Begin(Child, 0.2f, Vector3.one , Vector3.one * 0.1f, OnCancleClick);
		}

		void OnCancleClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamReplaceAreaCancel");
			Close();
		}

		void OnSureClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamReplaceAreaConfirmation");

			TeamManager.Instance.SetCurSelectEctypeContainerData(EctypeConfigManager.Instance.EctypeSelectConfigList[m_EctypeID]);		

			switch(m_PanelType)
			{
			case PanelType.Filter:
				var player = PlayerManager.Instance.FindHeroDataModel();
				NetServiceManager.Instance.TeamService.SendGetTeamListMsg(new SMSGGetTeamList_CS() { 
					uidEntity = player.UID,
					dwEctypeID = (uint)m_EctypeID,
					byDifficulty = 0,
				});
				PlayDisappearAnimation();
				UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowTeamChildPanel, ChildPanel.RefreshTeamList);
				break;
			case PanelType.Create:
				UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowTeamChildPanel, ChildPanel.SelectEctype);
				Close();
				break;
			}
		}

		void UnfreezeHandle(object obj)
		{
			m_isFreeze = false;
		}
	}
}
