using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;

namespace UI.Team
{
	
	public class SelectEctypePanel_V2 : MonoBehaviour 
	{
		public UILabel Label_AreaName;

		public UIDraggablePanel DraggablePanel;
		public UIGrid Grid;
		public GameObject EctypeCardItemPrefab;

		public LocalButtonCallBack Button_Change;
		public LocalButtonCallBack Button_Create;
		public SpriteSwith Swith_Hard;
		public LocalButtonCallBack Button_Hard;
		public LocalButtonCallBack Button_Exit;

		public GameObject EctypeSelectFramePrefab;
		private GameObject m_SelectFrame;

		private const int ECTYPE_MAX_NUM = 8;
		private List<EctypeCardItem_V2> m_EctypeCardList = new List<EctypeCardItem_V2>();

		private EctypeSelectConfigData m_EctypeSelectConfigData;
		private int m_EctypeContainerID;
		private int m_EctypeHard = 0;

		void Awake()
		{
			for(int i = 0;i<ECTYPE_MAX_NUM;i++)
			{
				GameObject ectypeCard = CreatObjectToNGUI.InstantiateObj(EctypeCardItemPrefab,Grid.transform);
				EctypeCardItem_V2 ectypeCardItem_V2 = ectypeCard.GetComponent<EctypeCardItem_V2>();

				//ectypeCardItem_V2.Init( EctypeCardItem_V2.CardType.Ectype, p, OnSelectCardHandle );
				m_EctypeCardList.Add(ectypeCardItem_V2);

			}

			Button_Change.SetCallBackFuntion(OnChangeClick, null);
			Button_Create.SetCallBackFuntion(OnCreateClick,null);
			Button_Hard.SetCallBackFuntion(OnHardClick, null);
			Button_Exit.SetCallBackFuntion(OnExitClick, null);

			TaskGuideBtnRegister();
		}

		/// <summary>
		/// 引导按钮注入代码
		/// </summary>
		private void TaskGuideBtnRegister()
		{
			Button_Change.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_CreateTeam_ChangeZone);
			Button_Create.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_CreateTeam_Create);		
			Button_Hard.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_CreateTeam_Normal);
			Button_Exit.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_CreateTeam_Back);
		}

		public void Show(EctypeSelectConfigData selectData, EctypeContainerData[] datas)
		{
			m_EctypeSelectConfigData = selectData;

			for(int i = 0; i < ECTYPE_MAX_NUM; i++)
			{
				if(i >= datas.Length)
				{
					m_EctypeCardList[i].gameObject.SetActive(false);
				}
				else
				{
					m_EctypeCardList[i].Init( EctypeCardItem_V2.CardType.Ectype,datas[i],OnSelectCardHandle);
					m_EctypeContainerID = datas[i].lEctypeContainerID;
					m_EctypeCardList[i].gameObject.SetActive(true);
									
					m_EctypeCardList[i].gameObject.RegisterBtnMappingId(m_EctypeContainerID, UIType.TeamInfo, BtnMapId_Sub.TeamInfo_CreateTeam_EctypeItem);
				}
			}

			Label_AreaName.text = LanguageTextManager.GetString( selectData._szName);

			//默认选择最大
			OnSelectCardHandle(m_EctypeContainerID);
			//默认普通难度
			m_EctypeHard = 0;
			Swith_Hard.ChangeSprite(m_EctypeHard + 1);		

			transform.localPosition = Vector3.zero;
		}


		public void Close()
		{
			transform.localPosition = Vector3.back * 800;
		}

		void OnChangeClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamReplaceArea");

			UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowTeamChildPanel,ChildPanel.SelectAreaForCreate);
			Close();
		}

		void OnCreateClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamCreateConfirmation");
			//创建队伍
		
			//判断是否满足条件
			EctypeContainerData SelectContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[m_EctypeContainerID];
			bool isCanCreate = false;
			int localCostNumber = int.Parse(SelectContainerData.lCostEnergy);
			int costNumber = 0;
			switch (SelectContainerData.lCostType)
			{
			case 1:
				costNumber = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE;
				if(costNumber < localCostNumber)
				{
					PopupObjManager.Instance.ShowAddVigour();
					return;
				}
				break;
			case 2:
				costNumber = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
				if(costNumber < localCostNumber)
				{
					MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I28_18"),1f);
					return;
				}
				break;
			case 3:
				costNumber = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
				if(costNumber < localCostNumber)
				{
					MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_H1_231"),1f);
					return;
				}
				break;
			default:
				break;
			}

			int index = 0;
			for(int i = 0 ; i < m_EctypeSelectConfigData._vectContainer.Length ; i++)
			{
				if(m_EctypeSelectConfigData._vectContainer[i] == m_EctypeContainerID)
				{
					NetServiceManager.Instance.TeamService.SendTeamCreateMsg(m_EctypeSelectConfigData._lEctypeID, i+1, m_EctypeHard);
					Close();
					break;
				}
			}
		}
		void OnHardClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamDifficultyChoice");
			m_EctypeHard = (m_EctypeHard + 1)%2;
			Swith_Hard.ChangeSprite(m_EctypeHard + 1);
		}

		void OnExitClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamCreateCancel");
			Close();
		}

		void OnSelectCardHandle(int ectypeID)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamEctypeChoice");

			m_EctypeContainerID = ectypeID;
			if(m_SelectFrame != null)
				Destroy( m_SelectFrame);
			m_SelectFrame = CreatObjectToNGUI.InstantiateObj(EctypeSelectFramePrefab,Grid.transform);
			var cardItem = m_EctypeCardList.SingleOrDefault(p=>p.EctypeID == ectypeID);

			m_SelectFrame.transform.localPosition = cardItem.transform.localPosition;
		}

	}
}