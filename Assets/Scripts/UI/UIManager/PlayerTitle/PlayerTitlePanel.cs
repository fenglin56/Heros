using UnityEngine;
using System.Collections;
using UI.PlayerTitle;
using System.Collections.Generic;
using System;
using UI.MainUI;
using System.Linq;

namespace UI
{
	public class PlayerTitlePanel : View 
	{
		public CommonPanelTitle CommonPanelTitle;

		public PlayerTitleEffectResItem[] eEffectResItems = new PlayerTitleEffectResItem[8];
		public LocalButtonCallBack Button_Return;

		public Transform Interface_Lock;
		public Transform Interface_Unlock;

		public LocalButtonCallBack Button_Use;
		public SpriteSwith Swith_UseFont;

		public UILabel Label_Combat;
		public UILabel Label_Effect;
		public SpriteSwith Swith_EffectiveData;
		public UILabel Label_Condition;

		public UIDraggablePanel DraggablePanel;
		public UIGrid Grid;

		public GameObject PlayerTitleItemPrefab;
		public GameObject Effect_Title_GetUI_Prefab;
		public GameObject Effect_Title_GetText01_Prefab;
		public GameObject Effect_Title_GetText02_Prefab;

		public Transform CurTitleIconTrans;

		public GameObject LeftPanel;
		public GameObject RightPanel;
		private Vector3 m_startPos_LeftPanel;
		private Vector3 m_startPos_RightPanel;

		private Dictionary<int, PlayerTitleItem> m_TitleItemDict = new Dictionary<int, PlayerTitleItem>();
		private Dictionary<int, List<PlayerTitleEffectData>> m_TitleEffectDict = new Dictionary<int, List<PlayerTitleEffectData>>();
		private int m_CurTitleID = 0;
		private GameObject m_CurTitleIcon = null;

		private List<UIPanel> m_PanelList =new List<UIPanel>();

		private List<Vector3> m_TitleTransList = new List<Vector3>();

		void Awake()
		{
			PlayerTitleConfigData[]  titleConfig = PlayerDataManager.Instance.GetPlayerTitleConfigArray();
			List<PlayerTitleItem> ownTitleList = new List<PlayerTitleItem>();
			List<PlayerTitleItem> dontHadTitleList = new List<PlayerTitleItem>();

			titleConfig.ApplyAllItem(p=>
			                         {
				GameObject titleGO = (GameObject)CreatObjectToNGUI.InstantiateObj(PlayerTitleItemPrefab, Grid.transform);
				PlayerTitleItem playerTitleItem = titleGO.GetComponent<PlayerTitleItem>();
				bool isUnlock = ContainerInfomanager.Instance.GetItemNumber(p._lGoodsID) > 0;
				playerTitleItem.Init(p._lGoodsID, isUnlock, p._lDisplayIdSmallPrefab, SelectTitle);
				m_TitleItemDict.Add(p._lGoodsID ,playerTitleItem);
				//加成属性
				List<string> effectStr = new List<string>();
				string[] vectEffectStr = p._vectEffects.Split('|');
				string[] addEffectStr = p._vectEffectsAdd.Split('|');
				effectStr.AddRange(vectEffectStr);
				effectStr.AddRange(addEffectStr);

				int effectLength = effectStr.Count;
				List<PlayerTitleEffectData> effectDataList = new List<PlayerTitleEffectData>();
				for(int i=0;i<effectLength;i++)
				{
					string[] growthEffect = effectStr[i].Split('+');
					var effectData = ItemDataManager.Instance.GetEffectData(growthEffect[0]);
					if (effectData != null)
					{
						PlayerTitleEffectData titleGrowEffextdata = new PlayerTitleEffectData()
						{
							EffectData = effectData,
							MinValue = Convert.ToInt32(growthEffect[1]),
							MaxValue = Convert.ToInt32(growthEffect[2])
						};
						effectDataList.Add(titleGrowEffextdata);

					}
				}
			 	m_TitleEffectDict.Add(p._lGoodsID, effectDataList);

			});
			Grid.Reposition();


			//SelectTitle(titleConfig[0]._lGoodsID);

			Button_Return.SetCallBackFuntion(OnReturnClick,null);
			Button_Use.SetCallBackFuntion(OnUseClick,null);
			m_startPos_LeftPanel = LeftPanel.transform.localPosition;
			m_startPos_RightPanel = RightPanel.transform.localPosition;
			m_PanelList.AddRange(gameObject.GetComponentsInChildren<UIPanel>());

			RegisterEventHandler();

			StartCoroutine("LateRecordPos");

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            Button_Return.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_PlayerTitlePanel_Return);
            Button_Use.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_PlayerTitlePanel_Use);
        }

//		void Start()
//		{
//			//记录位置
//			m_TitleItemDict.Values.ApplyAllItem(p=>{
//				Vector3 pos = p.transform.localPosition;
//				m_TitleTransList.Add(pos);
//			});
//		}

		IEnumerator LateRecordPos()
		{
			yield return new WaitForEndOfFrame();
			//记录位置
			m_TitleItemDict.Values.ApplyAllItem(p=>{
				Vector3 pos = p.transform.localPosition;
				m_TitleTransList.Add(pos);
			});
		}

		protected override void RegisterEventHandler ()
		{
			AddEventHandler(EventTypeEnum.PlayerTitleUpdate.ToString(), PlayerTitleUpdateHanlde);
		}

		void SelectTitle(object obj)
		{
			int titleID = (int)obj;
			this.m_CurTitleID = titleID;
			UpdateInterface(titleID);
			m_TitleItemDict.Values.ApplyAllItem(p=>
			                             {
				if(p.m_TitleID == titleID)
				{
					p.Reset(true);
				}
				else
				{
					p.Reset(false);
				}
			});
			bool isUnlock = ContainerInfomanager.Instance.GetItemNumber(titleID) > 0;
			Interface_Lock.gameObject.SetActive(!isUnlock);
			Interface_Unlock.gameObject.SetActive(isUnlock);
			int index = isUnlock == true ? 2 : 1;
			Swith_EffectiveData.ChangeSprite(index) ;


			if(m_CurTitleID == PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_TITLE)
			{
				//
				//Button_Use.SetEnabled(false);
				Swith_UseFont.ChangeSprite(2);
			}
			else
			{
				//Button_Use.SetEnabled(true);
				Swith_UseFont.ChangeSprite(1);
			}
		}

		void OnReturnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Title_Leave");
			ShowOut();
		}
		public void ClosePanel(object obj)
		{
			transform.localPosition = Vector3.forward * -800;
		}

		void OnUseClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Title_Click");
			if(m_CurTitleID == PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_TITLE)
			{
				//卸下
				SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
				var itemFlelInfo = ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(p=>p.LocalItemData._goodID == m_CurTitleID);
				dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = itemFlelInfo.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
				dataStruct.byPlace = (byte)itemFlelInfo.sSyncContainerGoods_SC.nPlace;
				dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
				//dataStruct.desPlace = (targetContainerBoxSlot.MyContainerBoxSlotData.CurrentPlace - 1);
				dataStruct.byUseType = 1;
				NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
			}
			else
			{
				//穿上
				SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();	
				var itemFlelInfo = ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(p=>p.LocalItemData._goodID == m_CurTitleID);
				
				dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = itemFlelInfo.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
				dataStruct.byPlace = (byte)itemFlelInfo.sSyncContainerGoods_SC.nPlace;
				dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
				NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
			}

		}

		private void UpdateInterface(int titleID)
		{
			PlayerTitleConfigData titleData = PlayerDataManager.Instance.GetPlayerTitleConfigData(titleID);
			if(titleData==null)
			{

				return;
			}
			List<PlayerTitleEffectData> effectDataList = m_TitleEffectDict[titleID];
			int dataLength = effectDataList.Count;
			int combatValue = 0;//战力
			for(int i = 0; i<eEffectResItems.Length;i++)
			{
				if(i >= dataLength)
				{
					eEffectResItems[i].gameObject.SetActive(false);
				}
				else
				{
					eEffectResItems[i].gameObject.SetActive(true);
					eEffectResItems[i].UpdateView(effectDataList[i].EffectData, effectDataList[i].MinValue);
					combatValue += effectDataList[i].MinValue * effectDataList[i].EffectData.CombatPara;
				}
			}
			//显示当前称号特效
			if(m_CurTitleIcon != null)
				Destroy(m_CurTitleIcon);
			m_CurTitleIcon = UI.CreatObjectToNGUI.InstantiateObj(titleData._ModelIdPrefab, CurTitleIconTrans);

			combatValue = (int)(combatValue/1000);
			Label_Combat.text = (combatValue * CommonDefineManager.Instance.CommonDefine.Display_Combat).ToString();
			Label_Effect.text = LanguageTextManager.GetString("IDS_I6_9")+ LanguageTextManager.GetString( titleData._lDisplayID);
			Label_Condition.text = LanguageTextManager.GetString("IDS_I6_8")+ LanguageTextManager.GetString( titleData._szDesc);
		}
		private void UpdateTitleInfo()
		{
			m_TitleItemDict.ApplyAllItem(p=>{
				bool isUnlock = ContainerInfomanager.Instance.GetItemNumber(p.Key) > 0;
				p.Value.UpdateInfo(isUnlock);
			});


//			var titleData = PlayerDataManager.Instance.GetPlayerTitleConfigData(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_TITLE);
//			if(titleData != null)
//			{
//				if(m_CurTitleIcon != null)
//					Destroy(m_CurTitleIcon);
//				m_CurTitleIcon = UI.CreatObjectToNGUI.InstantiateObj(titleData._ModelIdPrefab, CurTitleIconTrans);
//			}


			//重列位置
			List<PlayerTitleItem> ownList = new List<PlayerTitleItem>();
			List<PlayerTitleItem> dontHaveList = new List<PlayerTitleItem>();
			m_TitleItemDict.ApplyAllItem(p=>{
				if(ContainerInfomanager.Instance.GetItemNumber(p.Key) > 0)
				{
					ownList.Add(p.Value);
				}
				else
				{
					dontHaveList.Add(p.Value);
				}
			});
			ownList.AddRange(dontHaveList);
			for(int i =0 ; i< ownList.Count; i++)
			{
				ownList[i].transform.localPosition = m_TitleTransList[i];
			}
		}

		private void ShowIn()
		{
			float animTime = 0.1f;
			TweenPosition.Begin(LeftPanel,animTime,m_startPos_LeftPanel+Vector3.left * 200,m_startPos_LeftPanel);
			TweenPosition.Begin(RightPanel,animTime,m_startPos_RightPanel+Vector3.right * 200,m_startPos_RightPanel);
			//UIPanel uiPanel = gameObject.GetComponent<UIPanel>();
			TweenFloat.Begin(animTime,0,1,SetMyPanelAlpha);
			//TweenPosition.Begin(gameObject,animTime,IsLeftPos?LeftShowPos:RightShowPos);
			//if(TweenFloatObj!=null){DestroyImmediate(TweenFloatObj);}
			//TweenFloatObj = TweenFloat.Begin(animTime,MyPanelList[0].alpha,1,SetMyPanelAlpha);
		}
		private void ShowOut()
		{
			float animTime = 0.1f;
			TweenPosition.Begin(LeftPanel,animTime,m_startPos_LeftPanel,m_startPos_LeftPanel+Vector3.left * 200);
			TweenPosition.Begin(RightPanel,animTime,m_startPos_RightPanel,m_startPos_RightPanel+Vector3.right * 200);
			//UIPanel uiPanel = gameObject.GetComponent<UIPanel>();
			TweenFloat.Begin(animTime,1,0,SetMyPanelAlpha, ClosePanel);
		}
		void SetMyPanelAlpha(float value)
		{
			m_PanelList.ApplyAllItem(p=>
			                         {
				p.alpha = value;
			});
		}

		void PlayerTitleUpdateHanlde(INotifyArgs arg)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Title_Equip");
			UpdateTitleInfo();
			SelectTitle(m_CurTitleID);
		}

		public void Show()
		{
			CommonPanelTitle.TweenShow();

			transform.localPosition = Vector3.zero;
			ShowIn();
			UpdateTitleInfo();

			//已获得特效
			bool isHasPlay = false;
			m_TitleItemDict.ApplyAllItem(p=>{
				bool isUnlock = ContainerInfomanager.Instance.GetItemNumber(p.Key) > 0;
				if(isUnlock)
				{
					isHasPlay = true;
					var itemData = ContainerInfomanager.Instance.itemFielArrayInfo.SingleOrDefault(k=>k.LocalItemData._goodID == p.Key);
					if( itemData.equipmentEntity.ITEM_FIELD_VISIBLE_NEW == 0)
					{
						GameObject ownEff = (GameObject)CreatObjectToNGUI.InstantiateObj(Effect_Title_GetText01_Prefab, p.Value.transform);
						ownEff.transform.localPosition += Vector3.back * 10 + Vector3.down * 4;
						ownEff.AddComponent<DestroySelf>();
						GameObject ownEff2 = (GameObject)CreatObjectToNGUI.InstantiateObj(Effect_Title_GetText02_Prefab, p.Value.transform);
						ownEff2.transform.localPosition += Vector3.back * 10 + Vector3.down * 4;
						ownEff2.AddComponent<DestroySelf>();
						NetServiceManager.Instance.ContainerService.SendUpdateContainerGoodsNewStatu((int)itemData.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID,
						                                                                             (byte)itemData.sSyncContainerGoods_SC.nPlace);
					}
					else
					{
						GameObject ownEff = (GameObject)CreatObjectToNGUI.InstantiateObj(Effect_Title_GetUI_Prefab, p.Value.transform);
						ownEff.transform.localPosition += Vector3.back * 10 + Vector3.down * 4;
						ownEff.AddComponent<DestroySelf>();
					}
				}
			});
			if(isHasPlay)
			{
				SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Title_Get");
			}

			//选择第一个
			m_TitleItemDict.ApplyAllItem(p=>{
				if(p.Value.transform.localPosition == m_TitleTransList[0])
				{
					SelectTitle(p.Key);
				}
			});
		}
		
		

//		public void Close()		
//		{
//			transform.localPosition = Vector3.forward * -800;
//		}

		public class PlayerTitleEffectData
		{
			public EffectData EffectData;
			public int MinValue;
			public int MaxValue;
		}


	}
}