using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI.Siren
{ 
	/// <summary>
	/// 炼妖面板
	/// </summary>
	public class SirenPanelUIManager_V2 : BaseUIPanel
	{
		public UIAnchor Anchor_Left;
		public UIAnchor Anchor_Right;

		public Transform FirstAttributeItem;
		public GameObject AttributeItemPrefab;
		private SirenAttributeItem[] m_SirenAttributes;
		
		public Transform FirstConditionItem;
		public GameObject ConditionItemPrefab;
		private SirenConditionItem[] m_SirenConditions;

		//siren name zhanli
		public UILabel Label_Combat;
		
		//Title
		public TweenPosition TweenPos_CommonFrame;
		public Transform CommonPanelTitlePoint;
		public GameObject CommonPanelTitlePrefab;
		private BaseCommonPanelTitle m_CommonPanelTitle;
		
		//siren tip
		public UILabel Label_SirenTip;
		
		//siren Level || siren Introduction
		public Transform Introduction;
		public UILabel Label_Introduction;
		public Transform LevelInfo;
		public UILabel Label_SirenLevel;
		public UILabel Label_SirenNextLevelTxt;
		public UISlider Slider_SirenProcess;
		public UILabel Label_SirenProcess;

		//
		public SirenUnlockBox UnlockBox;
		
		public LocalButtonCallBack Button_Exit;
		public LocalButtonCallBack Button_Join;

		public LocalButtonCallBack Button_Touch;//新版 触摸妖女进行炼化

		public LocalButtonCallBack Button_Break;

		public SirenRefineryEffectControl EffControl_Refinery;
		public GameObject Mark_Refinery;
		public SirenModelViewControl ViewControl_Siren;
		public SirenDialogManager DialogManager_Siren;
		
		public Transform SirenNameTrans;
		public Transform SirenTitleTrans;
		private GameObject m_curSirenName = null;
		private GameObject m_curSirenTitle = null;

		//Effect
		public GameObject EffSirenTitlePrefab;
		public GameObject EffSirenLevelUpPrefab;
		public GameObject EffAttributeUpPrefab;
		public GameObject EffSirenCombatPrefab;
		public GameObject EffSirenBattleInPrefab;
		public Transform EffSirenTitleTrans;
		public Transform EffSirenLevelUpTrans;
		public Transform EffAttributeTrans;
		public Transform EffSirenCombatTrans;
		public Transform EffSirenBattleInTrans;
		//interface
		public GameObject Interface_Lock;   //未解锁
		public GameObject Interface_Unlock; //解锁
		public GameObject Button_Lock;
		public LocalButtonCallBack Button_Unlock;
		public SpriteSwith Swith_EffUnlock;
		//public UILabel Label_Explanation;
		public UILabel Label_SirenSkillExplanation;//奥义说明
		public UILabel Label_UseExplanation;//奥义使用说明
		public UILabel Label_BreakExplanation;//突破技能说明

		//public UISprite Image_CurJoinSiren;	//出战妖女
		public Transform SirenJoinNameTrans;
		private GameObject m_CurJoinSirenName = null;

		public GameObject RefineryComplete;
		public GameObject RefineryCost;

		
		//page
		public UISlicedSprite Sprite_SirenName;
		public SingleButtonCallBack Button_PageUp;		//翻页 上个妖女
		public SingleButtonCallBack Button_PageDown;	//翻页 下个妖女
		public UILabel Label_Pagination;

		//break
		public SirenBreakPanel BreakPanel;
		public GameObject[] StarStages = new GameObject[3];
		public GameObject Eff_BreakButton;	//提示可突破特效
		public GameObject Eff_BreakSuccess_Prefab;//突破成功特效 

		private int m_curSirenNo = 1;
		
		private const float EffUnlockFlashTime = 0.3f;
		
		private Dictionary<int, SirenItemControl_V3> m_SirenItemDict = new Dictionary<int, SirenItemControl_V3>();
		
		private int m_CurSelectedSirenItemID = 0;
		private int m_curSirenExperienceLastValue = 0;//存储当前妖女未炼化前经验值
		
		private const float RefineryUnderWayTime = 3f;
		
		private SMsgActionLianHua_SC? m_LianHuaResult = null;
		
		//循环默认对白
		private float m_defaultDialogColdTime = 0;
		private float m_defaultWordCd = 10000;
		private SirenDialogConfigData m_defaultDialogConfigData;

		private List<GameObject> m_LegacyEffectList = new List<GameObject>();
		private bool m_isLevelUp = false;//是否升级


		private int[] m_guideBtnID = new int[7];	

		void Awake()
		{
			Button_Exit.SetCallBackFuntion(OnExitClick, null);
			Button_Join.SetCallBackFuntion(OnJoinClick, null);		

			Button_Unlock.SetCallBackFuntion(OnRefineryClick, null);
			Button_Touch.SetCallBackFuntion(OnTouchToRefineryClick,null);

			Button_Break.SetCallBackFuntion(OnOpenBreakPanelClick,null);

			Button_PageUp.SetCallBackFuntion(OnPageUpClick, null);
			Button_PageDown.SetCallBackFuntion(OnPageDownClick, null);

			this.RegisterEventHandler();
				
			//移出摄像机
			ViewControl_Siren.transform.parent = null;
			ViewControl_Siren.transform.transform.localScale = Vector3.one;
			ViewControl_Siren.SetCallBack(OnRefineryClick);

			if(PopupObjManager.Instance.UICamera != null)
			{
				Anchor_Left.uiCamera = PopupObjManager.Instance.UICamera;
				Anchor_Right.uiCamera = PopupObjManager.Instance.UICamera;
			}

			GameObject commonTitle = UI.CreatObjectToNGUI.InstantiateObj(CommonPanelTitlePrefab,CommonPanelTitlePoint);
			m_CommonPanelTitle = commonTitle.GetComponent<BaseCommonPanelTitle>();
			m_CommonPanelTitle.HidePos = new Vector3(-100,0,0);
			m_CommonPanelTitle.ShowPos = Vector3.zero;
			m_CommonPanelTitle.Init(CommonTitleType.Practice , CommonTitleType.GoldIngot);

			Init();
			InitSirenList();

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            Button_Exit.gameObject.RegisterBtnMappingId(UIType.Siren, BtnMapId_Sub.Siren_Back);
            Button_Join.gameObject.RegisterBtnMappingId(UIType.Siren, BtnMapId_Sub.Siren_Join);
            Button_Unlock.gameObject.RegisterBtnMappingId(UIType.Siren, BtnMapId_Sub.Siren_SubdueSiren);
            Button_Touch.gameObject.RegisterBtnMappingId(UIType.Siren, BtnMapId_Sub.Siren_TouchPoint);
            Button_PageUp.gameObject.RegisterBtnMappingId(UIType.Siren, BtnMapId_Sub.Siren_Previour);
            Button_PageDown.gameObject.RegisterBtnMappingId(UIType.Siren, BtnMapId_Sub.Siren_Next);
			Button_Break.gameObject.RegisterBtnMappingId(UIType.Siren, BtnMapId_Sub.Siren_Break);
        }
		
		void OnDestroy()
		{
			for (int i = 0; i < m_guideBtnID.Length; i++)
			{
				//TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
			}
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.AddColdWork, AddColdWork);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RemoveColdWork, RemoveColdWork);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.YaoNvJoinSuccess, SirenJoinSuccessHandle);
			RemoveEventHandler(EventTypeEnum.LianHuaResult.ToString(), LianHuaResultHandle);  
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.IntellJumpSiren, SelectSirenByIDCallback);

		}
		
		public override void Show(params object[] value)
		{
			SoundManager.Instance.StopBGM(0.0f);
			SoundManager.Instance.PlayBGM("Music_UIBG_Siren", 0.0f);
			ViewControl_Siren.gameObject.SetActive(true);
			ViewControl_Siren.SetSirenSceneActive(true);

			m_CommonPanelTitle.TweenShow();
			TweenPos_CommonFrame.Reset();
			TweenPos_CommonFrame.Play(true);

			if(value.Length>0)
			{
				SYaoNvContext context = SirenManager.Instance.GetYaoNvList().SingleOrDefault(p=>p.byYaoNvID == (int)value[0]);
				if(context.byYaoNvID == 0)
				{
					m_CurSelectedSirenItemID = m_SirenItemDict.Keys.First();
				}
				else
				{
					m_CurSelectedSirenItemID = (int)value[0];
				}
//				if(SirenManager.Instance.GetYaoNvList().Any(p=>p.byAssembly == 1))
//				{
//					SYaoNvContext assemblyContext = SirenManager.Instance.GetYaoNvList().SingleOrDefault(p=>p.byAssembly == 1);
//					m_CurSelectedSirenItemID = assemblyContext.byYaoNvID;
//				}
//				else
//				{
//					m_CurSelectedSirenItemID = m_SirenItemDict.Keys.First();
//				}
			}
			else
			{
				List<SYaoNvContext> yaoNvList = SirenManager.Instance.GetYaoNvList();
				if (yaoNvList.Count > 0)
				{
					SYaoNvContext context = new SYaoNvContext();
					for(int i=0;i<yaoNvList.Count;i++)
					{
						if(context.byYaoNvID <yaoNvList[i].byYaoNvID)
						{
							context = yaoNvList[i];
						}
					}
					m_CurSelectedSirenItemID = context.byYaoNvID;
				}
				else//没有解锁 默认第一个妖女
				{
					m_CurSelectedSirenItemID = m_SirenItemDict.Keys.First();
				}
			}

			this.SirenBeSelectedHandle(m_CurSelectedSirenItemID);
			
			//更新页码
			var sirenList = SirenDataManager.Instance.GetPlayerSirenList();
			
			for (int i = 0; i < sirenList.Count; i++)
			{
				if (sirenList[i]._sirenID == m_CurSelectedSirenItemID)
				{
					m_curSirenNo = i + 1;
					PageUpdate();
					break;
				}
			}

			//cur join
			if(m_CurJoinSirenName != null)
				Destroy(m_CurJoinSirenName);
			if(SirenManager.Instance.GetYaoNvList().Any(p=>p.byAssembly == 1))
			{
				var sirenContext = SirenManager.Instance.GetYaoNvList().FirstOrDefault(p=>p.byAssembly == 1);
				var sirenData = sirenList.SingleOrDefault(p=>p._sirenID == sirenContext.byYaoNvID);
				if(sirenData!=null)
				{
					var sirenThisLevelData = sirenData._sirenConfigDataList.SingleOrDefault(level=>level._growthLevels == sirenContext.byLevel);
					m_CurJoinSirenName = UI.CreatObjectToNGUI.InstantiateObj(sirenThisLevelData._NamePrefab , SirenJoinNameTrans);
				}
			}

			//播放按钮动画
			PlayButtonAnimation();
			base.Show(value);
		}

		public override void Close()
		{
			if (!IsShow)
				return;
			UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, null);
			GameManager.Instance.PlaySceneMusic();
			UnlockBox.Close();
			ViewControl_Siren.SetSirenSceneActive(false);
			ViewControl_Siren.gameObject.SetActive(false);
			base.Close();
		}
		
		void Update()
		{
			if (transform.localPosition != Vector3.zero)
				return;
			
			m_defaultDialogColdTime += Time.deltaTime;
			if (m_defaultDialogColdTime >= m_defaultWordCd)
			{
				ShowSirenDialog(m_defaultDialogConfigData);
			}

		}
		
		void OnHelpClick(object obj)
		{

		}
		
		void OnPageUpClick(object obj)
		{
			if (m_curSirenNo <= 1)
				return;
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Siren_Page");
			m_curSirenNo--;
			PageUpdate();
			//int dictLength = m_SirenItemDict.Count;
			//var list = m_SirenItemDict.ToArray();
			//Label_Pagination.text = m_curSirenNo.ToString() + "/" + dictLength.ToString();
		}
		void OnPageDownClick(object obj)
		{
			int dictLength = m_SirenItemDict.Count;
			if (m_curSirenNo >= dictLength)
				return;
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Siren_Page");
			m_curSirenNo++;
			PageUpdate();
			//var list = m_SirenItemDict.ToArray();
			//Label_Pagination.text = m_curSirenNo.ToString() + "/" + dictLength.ToString();
		}

		//打开指定妖女ID界面
		void SelectSirenByIDCallback(object arg)
		{
			int sirenID = (int)arg;
			int dictLength = m_SirenItemDict.Count;
			if(sirenID > dictLength || sirenID < 1)
				return;
			m_curSirenNo = sirenID;
			PageUpdate();
		}

		void PageUpdate()
		{
			int dictLength = m_SirenItemDict.Count;
			m_SirenItemDict[m_curSirenNo].OnButtonClick(null);
			Button_PageUp.BackgroundSprite.alpha = m_curSirenNo <= 1 ? 0.5f : 1f;
			Button_PageDown.BackgroundSprite.alpha = m_curSirenNo >= dictLength ? 0.5f : 1f;
			//Label_Pagination.text = m_curSirenNo.ToString() + "/" + dictLength.ToString();
		}

		void OnNedanCollectionClick(object obj)
		{
			
		}
		void OnExitClick(object obj)
		{
			ClearLegacyEffects();
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Siren_Leave");
			Close();
			CleanUpUIStatus();
			//this.CloseSirenPanel();
		}
		void OnJoinClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Siren_Battle");
			NetServiceManager.Instance.EntityService.SendYaoNvJoin(m_CurSelectedSirenItemID);
		}	
		void PlaySirenAnimation(object obj)
		{            
			var sirenData = m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData();
			//TraceUtil.Log("[触摸妖女]" + sirenData._touchAnim);
			if (ViewControl_Siren.PlayAnimation(sirenData._touchAnim))
			{
				SoundManager.Instance.PlaySoundEffect(sirenData._touchSound);//播放触摸语音  
			}
			ShowSirenDialog(sirenData._touchWord); 
			  
		}
		void OnTouchToRefineryClick(object obj)
		{
			var sirenControl = m_SirenItemDict[m_CurSelectedSirenItemID];
			if(sirenControl.IsUnlock())
			{
				if(!m_SirenItemDict[m_CurSelectedSirenItemID].IsBreakStageMaxLevel())
				{
					OnRefineryClick(null);
				}
				else
				{
					PlaySirenAnimation(null);
				}
			}
		}

		void BreakPanelCallBack(object obj)
		{
			BreakPanel.Close();
			GameObject eff = UI.CreatObjectToNGUI.InstantiateObj(Eff_BreakSuccess_Prefab,transform);
			eff.transform.localPosition = Vector3.up * 100;
			eff.AddComponent<DestroySelf>();
			OnRefineryClick(null);
		}

		//强行收服妖女按钮回调
		void OnRefineryClick(object obj)
		{
			var sirenControl = m_SirenItemDict[m_CurSelectedSirenItemID];
			var playerData = PlayerManager.Instance.FindHeroDataModel();

			//玩家自身等级不足
			int limitLevel = sirenControl.GetSirenConfigData()._growthLvlLimit;
			if(playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL < limitLevel)
			{

				MessageBox.Instance.ShowTips(4,string.Format(LanguageTextManager.GetString("IDS_I2_12"),limitLevel.ToString()),1f);
				//MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I2_17"),1f);
				return;
			}

			//未解锁
			if(!sirenControl.IsUnlock())
			{
				Debug.Log("玩家自身等级不足");
				SoundManager.Instance.PlaySoundEffect("Sound_Button_Siren_Get");
				bool isConditionMeet = true;
				for(int i=0;i<m_SirenConditions.Length;i++)
				{
					if(m_SirenConditions[i].gameObject.activeInHierarchy && !m_SirenConditions[i].IsMeet )
					{
						isConditionMeet = false;
						break;
					}
				}
				//如果条件满足
				if(isConditionMeet)
				{
					NetServiceManager.Instance.EntityService.SendLianHua(m_CurSelectedSirenItemID, 
					                                                     EntityService.YaoNvOpType.unlockNormal,0);
					StartCoroutine(UnlockSiren());
				}
				else
				{
					//如果条件不满足
					//UnlockBox.Show(m_CurSelectedSirenItemID, sirenControl.GetPlayerSirenConfigData()._SirenPrice, UnlockSirenCallBack);
					//文钊修改，直接弹出文本提示框，SirenPrice字段不再生效SirenPrice字段不再生效(20141124细节修改)
					MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I2_17"),1f);
				}
			}
			else//解锁
			{
				if(!sirenControl.IsMaxLevel())
				{
					if(playerData.PlayerValues.PLAYER_FIELD_PRACTICE_NUM > 0)
					{
						//计算可以提交的修为值
						SYaoNvContext context = SirenManager.Instance.GetYaoNvList().SingleOrDefault(p=>p.byYaoNvID == m_CurSelectedSirenItemID);
						m_curSirenExperienceLastValue = context.lExperience;//记录之前的经验值
						int needExp = sirenControl.GetSirenConfigData()._growthCost - context.lExperience;
						int popExp = 0;
						if( playerData.PlayerValues.PLAYER_FIELD_PRACTICE_NUM >= needExp)
						{
							popExp = needExp;
						}
						else
						{
							popExp = playerData.PlayerValues.PLAYER_FIELD_PRACTICE_NUM;
						}
						
						NetServiceManager.Instance.EntityService.SendLianHua(m_CurSelectedSirenItemID, 
						                                                     EntityService.YaoNvOpType.upgrade,popExp);
						
						//特效
						//gEffRefineryUnderWay = EffControl_Refinery.PlayEff(SirenRefineryEffectControl.Refinery.Underway);
						//开启遮罩
						Mark_Refinery.SetActive(true);
						//开启计时
						StartCoroutine(RefineryUnderWay());
						//妖女表现
						var sirenData = sirenControl.GetSirenConfigData();
						ViewControl_Siren.PlayAnimation(sirenData._fearAnim);
						//DialogManager_Siren.CloseDialogImmediately();
						
						//振动效果
						ViewControl_Siren.ShakeCamera();
					}else
					{
						//没有修为，请到副本收集
						MessageBox.Instance.ShowTips(2,LanguageTextManager.GetString("IDS_I2_7"),1);
					}

				}
				else
				{
					//MessageBox.Instance.ShowTips(3,"Siren Level is Max , you cant upgrade it",1f);
					PlaySirenAnimation(null);
				}
			}
		
		
						
		}
		IEnumerator RefineryUnderWay()
		{		
			yield return new WaitForEndOfFrame();
								
			if (m_LianHuaResult != null)
			{
				Mark_Refinery.SetActive(false);
				if (m_LianHuaResult.Value.bySucess == 0)//炼化失败
				{
					SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SirenFail");
					//gEffRefineryResult = EffControl_Refinery.PlayEff(SirenRefineryEffectControl.Refinery.Fail);
					//妖女表现
					PlaySirenAnimation(null);
//					var sirenData = m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData();
//					ViewControl_Siren.PlayAnimation(sirenData._touchAnim);                    
//					ShowSirenDialog(sirenData._failWord);

				}
				else//炼化成功
				{
					m_isLevelUp = m_LianHuaResult.Value.dwCurXiuWeiNum==0;

					//更新界面
					SirenItemControl_V3 sirenItemControl  = m_SirenItemDict[m_LianHuaResult.Value.byYaoNvID];

					
					//List<SirenGrowthEffect> lastEffect = sirenItemControl.GetSirenGrowthEffect();
					sirenItemControl.UpdateView(m_LianHuaResult.Value.byLianHuaLevel);//更新等级 和 界面
					
					//完成进度条增长动画
					var yaoNvData = SirenManager.Instance.GetYaoNvList().SingleOrDefault(p=>p.byYaoNvID == m_CurSelectedSirenItemID);
					int curExp = yaoNvData.lExperience;
					int maxExp = m_SirenItemDict[m_CurSelectedSirenItemID].MaxExperience;
					StopAllCoroutines();
					StartCoroutine(PlaySirenProcessIncreaseAnimation(m_CurSelectedSirenItemID,m_curSirenExperienceLastValue*1f,curExp*1f,maxExp));
					
					
					//特效
					if (sirenItemControl.IsMaxLevel())
					{
						SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SirenComplete");
						//gEffRefineryResult = EffControl_Refinery.PlayEff(SirenRefineryEffectControl.Refinery.Complete);
						//满级 置灰按钮
						//Button_Refinery.SetEnabled(false);
					}
					else
					{
						SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SirenSuccess");
						//gEffRefineryResult = EffControl_Refinery.PlayEff(SirenRefineryEffectControl.Refinery.Success);
					}
					
					//妖女表现                    
					var sirenData = m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData();
					ViewControl_Siren.UpdateSiren(m_CurSelectedSirenItemID, sirenData);
					PlaySirenAnimation(null);

					//振动效果
					ViewControl_Siren.ShakeCamera();



				}
				m_LianHuaResult = null;
			}     
			else
			{
				StartCoroutine(RefineryUnderWay());
			}
		}

		void UnlockSirenCallBack(object obj)
		{
			StartCoroutine(UnlockSiren());
		}

		IEnumerator UnlockSiren()
		{
			yield return new WaitForEndOfFrame();
			if (m_LianHuaResult == null)
			{				
				StartCoroutine(UnlockSiren());
			}
			else
			{
				m_isLevelUp = m_LianHuaResult.Value.dwCurXiuWeiNum==0;
				//更新界面
				SirenItemControl_V3 sirenItemControl = m_SirenItemDict[m_LianHuaResult.Value.byYaoNvID];
				sirenItemControl.UpdateView(m_LianHuaResult.Value.byLianHuaLevel);
				//				m_SirenItemDict.TryGetValue(m_LianHuaResult.Value.byYaoNvID, out sirenItemControl);                
				//				if (sirenItemControl != null)
				//				{
				//					sirenItemControl.UpdateView(m_LianHuaResult.Value.byLianHuaLevel);
				//				}
				//妖女表现   

				var sirenData = m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData();
				ViewControl_Siren.UpdateSiren(m_CurSelectedSirenItemID, sirenData);
				PlaySirenAnimation(null);
//				ViewControl_Siren.UpdateSiren(m_CurSelectedSirenItemID, sirenData);
//				ViewControl_Siren.PlayAnimation(sirenData._touchAnim);
//				ShowSirenDialog(sirenData._successWord);

				m_LianHuaResult = null;
			}
		}

		void OnOpenBreakPanelClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Siren_Break");
			var nextLevelSirenConfig = m_SirenItemDict[m_CurSelectedSirenItemID].GetNextLevelSirenConfigData();
			BreakPanel.Show(BreakPanelCallBack,m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData(),nextLevelSirenConfig.BreakStageMaxLevel,
			                m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenGrowthEffect(),m_SirenItemDict[m_CurSelectedSirenItemID].GetNextSirenGrowthEffect());
		}

		private void Init()
		{
			var sirenDataList = SirenDataManager.Instance.GetPlayerSirenList();
			//属性
			m_SirenAttributes = new SirenAttributeItem[8];
			m_SirenAttributes[0] = FirstAttributeItem.GetComponent<SirenAttributeItem>();
			for(int i=1;i<=7;i++)
			{
				GameObject attributeObj =(GameObject)Instantiate(AttributeItemPrefab);
				attributeObj.transform.parent = FirstAttributeItem.parent;
				attributeObj.transform.localScale = Vector3.one;
				attributeObj.transform.localPosition = FirstAttributeItem.localPosition + Vector3.up * -35 * i;
				m_SirenAttributes[i] = attributeObj.GetComponent<SirenAttributeItem>();
			}
			//条件
			m_SirenConditions = new SirenConditionItem[5];
			m_SirenConditions[0] = FirstConditionItem.GetComponent<SirenConditionItem>();
			for(int i=1;i<=4;i++)
			{
				GameObject conditionObj =(GameObject)Instantiate(ConditionItemPrefab);
				conditionObj.transform.parent = FirstConditionItem.parent;
				conditionObj.transform.localScale = Vector3.one;
				conditionObj.transform.localPosition = FirstConditionItem.localPosition + Vector3.up * -58 * i;
				m_SirenConditions[i] = conditionObj.GetComponent<SirenConditionItem>();
			}
		}
		private void UpdateAttributeList(int sirenId)
		{
			var sirenGrowthEffectList = m_SirenItemDict[sirenId].GetSirenGrowthEffect();
			int listLenget = sirenGrowthEffectList.Count;
			for(int i=0;i<m_SirenAttributes.Length;i++)
			{
				if(i>=listLenget)
				{
					m_SirenAttributes[i].gameObject.SetActive(false);
				}
				else
				{
					m_SirenAttributes[i].gameObject.SetActive(true);
					m_SirenAttributes[i].Init(sirenGrowthEffectList[i].EffectData.IDS,sirenGrowthEffectList[i].EffectData.EffectRes,
					                          sirenGrowthEffectList[i].GrowthEffectValue,sirenGrowthEffectList[i].GrowthEffectMaxValue);
				}
				
			}
			
		}
		private void UpdateConditionList(int sirenID)
		{
			var sirenConditionTxt = m_SirenItemDict[sirenID].GetSirenUnlockTxt();
			var sirenUnolcokCondition = m_SirenItemDict[sirenID].GetSirenUnlockCondition();
			int txtLength = sirenConditionTxt.Length;
			SYaoNvCondtionInfo conditionArray = SirenManager.Instance.GetConditionList().SingleOrDefault(p=>(int)p.byYaoNvID == sirenID);
			for(int i = 0;i<m_SirenConditions.Length;i++)
			{
				if(i>=txtLength)
				{
					m_SirenConditions[i].gameObject.SetActive(false);
				}
				else
				{
					m_SirenConditions[i].gameObject.SetActive(true);
					bool isMeet = false;

					//条件判断

					switch( sirenUnolcokCondition[i].Type)
					{
					case 1://妖女ID+妖女等级
						var yaoNvList = SirenManager.Instance.GetYaoNvList();
						for(int j=0;j<yaoNvList.Count;j++)
						{
							if(yaoNvList[j].byYaoNvID == sirenUnolcokCondition[i].Condition1)
							{
								if(yaoNvList[j].byLevel >= sirenUnolcokCondition[i].Condition2)
								{
									isMeet = true;
								}
								break;
							}
						}
						break;
					case 2://副本ID+通关次数
						break;
					case 3://副本区域ID+通关次数
						break;
					case 4://花费元宝数+0
						int ingot = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_GOLD_TOTALCOST_VALUE;
						if(ingot >= sirenUnolcokCondition[i].Condition1)
						{
							isMeet = true;
						}
						break;
					case 5://当前战力+0
						int curCombat = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING;
						if(curCombat>=sirenUnolcokCondition[i].Condition1)
						{
							isMeet = true;
						}
						break;
					case 6://角色等级+0
						int level = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
						if(level>= sirenUnolcokCondition[i].Condition1)
						{
							isMeet = true;
						}
						break;
					case 7://VIP等级+0
						int vipLevel = PlayerManager.Instance.FindHeroDataModel().GetCommonValue().PLAYER_FIELD_VISIBLE_VIP;
						if(vipLevel>=sirenUnolcokCondition[i].Condition1)
						{
							isMeet = true;
						}
						break;
					case 8://完成指定副本 由下发数据为准
						if(conditionArray.byYaoNvID!=0)
						{
							isMeet = conditionArray.byCondition[i] == 2;
						}
						break;
					default:
						isMeet = true;
						break;
					}



					//赋值
					m_SirenConditions[i].Init(isMeet , LanguageTextManager.GetString(sirenConditionTxt[i]));
				}
			}
		}
		
		private void ShowSirenDialog(SirenDialogConfigData data)
		{
			m_defaultDialogColdTime = 0;
			//TraceUtil.Log("[SirenDialogConfigData.IDS]" + data.IDS);
			if (data.IDS != "0")
			{
				DialogManager_Siren.ShowDialog(data);
			}            
		}       
		private void ResetDefaultDialogConfigData(float cdTime, SirenDialogConfigData data)
		{
			m_defaultWordCd = cdTime / 1000;    //毫秒转换为秒
			m_defaultDialogConfigData = data;
		}
		//初始化女妖列表
		private void InitSirenList()
		{
			var sirenList = SirenDataManager.Instance.GetPlayerSirenList();
			sirenList.ApplyAllItem(p =>
			                       {
				//* SirenItemControl_V3 区别于 SirenItemControl 其他版本，不继承于monobehaviour
				SirenItemControl_V3 itemCtrl = new SirenItemControl_V3();
				itemCtrl.Init(p, SirenBeSelectedHandle);
				m_SirenItemDict.Add(p._sirenID, itemCtrl);
			});
			//ItemPageManager_Siren.InitPager(sirenList.Count, 1, 0);
			//			Button_PageUp.BackgroundSprite.alpha = m_curSirenNo <= 1 ? 0.5f : 1f;
			//			Label_Pagination.text = "1/" + m_SirenItemDict.Count.ToString();
		}
		//翻页
		void OnPageChanged(PageChangedEventArg arg)
		{
			//m_SirenItemDict.Values.ApplyAllItem(p =>
			//    {
			//        p.transform.position = new Vector3(-2000, 0, 0);
			//    });
			//int size = ItemPageManager_Siren.ItemBgs.Length;
			//var sirenArray = m_SirenItemDict.Values.Skip((arg.StartPage - 1) * size).Take(size).ToArray();
			//ItemPageManager_Siren.UpdateItems(sirenArray, "SirenList");
		}

		//选择女妖调用
		void SirenBeSelectedHandle(int sirenId)
		{
			//SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
			//安全判断
			if (m_SirenItemDict.ContainsKey(sirenId) == false)
			{
				TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"sirenId is null");
				return;
			}

			//清除之前的残留特效
			ClearLegacyEffects();

			bool isChangeSiren = false;
			if(m_CurSelectedSirenItemID != sirenId)
			{
				isChangeSiren = true;
				PlaySirenEffect(EffSirenTitlePrefab,EffSirenTitleTrans);
			}

			//更新当前选择的妖女id
			m_CurSelectedSirenItemID = sirenId;
							
			
			//获得妖女配置信息            
			var playerSirenConfigData = m_SirenItemDict[sirenId].GetPlayerSirenConfigData();
			var sirenData = m_SirenItemDict[sirenId].GetSirenConfigData();
			SirenDataManager.Instance.CurSelectSiren = m_SirenItemDict [sirenId];
			
			//重置默认对白信息
			ResetDefaultDialogConfigData(playerSirenConfigData._defaultWordCd, sirenData._defaultWord);
			
			if (m_SirenItemDict[sirenId].IsUnlock())//解锁
			{
				//界面
				Interface_Lock.SetActive(false);
				Interface_Unlock.SetActive(true);
				
				Introduction.gameObject.SetActive(false);
				LevelInfo.gameObject.SetActive(true);
				
				UpdateAttributeList(sirenId);
				//Button_Lock.SetActive(false);
				
				Label_SirenTip.text = LanguageTextManager.GetString("IDS_I2_2");
							

				//奥义说明
				Label_SirenSkillExplanation.text = "       "+LanguageTextManager.GetString("IDS_I2_10")
					+ LanguageTextManager.GetString(sirenData._SirenSkillIDText);
				//使用说明
				Label_UseExplanation.text = "   " + LanguageTextManager.GetString("IDS_I2_3");
				//团队技能说明
				Label_BreakExplanation.text = "       "+LanguageTextManager.GetString(sirenData.SirenTeamBuffText);

				//进阶
				for(int i=0;i<StarStages.Length;i++)
				{
					if(i < sirenData.SirenBreakStage)
					{
						StarStages[i].SetActive(true);
					}
					else
					{
						StarStages[i].SetActive(false);
					}
				}

				if(!isChangeSiren)//不是切换妖女的情况(升级 炼妖)
				{
					if(m_isLevelUp)
					{
						PlaySirenEffect(EffSirenCombatPrefab,EffSirenCombatTrans);
						PlaySirenEffect(EffAttributeUpPrefab,EffAttributeTrans);
						PlaySirenEffect(EffSirenLevelUpPrefab,EffSirenLevelUpTrans);
					}
					else
					{
						PlaySirenEffect(EffSirenLevelUpPrefab,EffSirenLevelUpTrans);
					}
				}
				else
				{
					PlaySirenEffect(EffSirenCombatPrefab,EffSirenCombatTrans);
					PlaySirenEffect(EffAttributeUpPrefab,EffAttributeTrans);
					PlaySirenEffect(EffSirenLevelUpPrefab,EffSirenLevelUpTrans);
				}
			}
			else//未解锁
			{
				//界面
				Interface_Lock.SetActive(true);
				Interface_Unlock.SetActive(false);
				
				Introduction.gameObject.SetActive(true);
				LevelInfo.gameObject.SetActive(false);
				
				Label_SirenTip.text = LanguageTextManager.GetString("IDS_I2_7");
				Label_Introduction.text = LanguageTextManager.GetString(playerSirenConfigData._SirenText);
				
				UpdateConditionList(sirenId);
				
			}
			//显示模型
			ViewControl_Siren.ShowSiren(sirenId, sirenData);
			
			//等级
			Label_SirenLevel.text = m_SirenItemDict[sirenId].GetProcessValue();

			//进度
			if(m_SirenItemDict[sirenId].IsMaxLevel())
			{
				Slider_SirenProcess.sliderValue = 1;
				Label_SirenProcess.text = "";
				//说明
				Label_SirenNextLevelTxt.text = LanguageTextManager.GetString("IDS_I2_11");
				if(Button_Break.gameObject.activeInHierarchy)
				{
					Button_Break.gameObject.SetActive(false);
				}
			}
			else
			{
				//进阶
				if(m_SirenItemDict[sirenId].IsBreakStageMaxLevel())
				{
					Button_Break.gameObject.SetActive(true);
					//判断是否满足突破条件
					bool isSatisfy = true;
					var breakConditions = m_SirenItemDict[sirenId].GetSirenConfigData().BreakCondition;
					for(int i=0;i<breakConditions.Length;i++)
					{
						isSatisfy = ContainerInfomanager.Instance.GetItemNumber(breakConditions[i].ItemID) >= breakConditions[i].ItemNum;
						if(!isSatisfy)
						{
							break;
						}
					}
					if(isSatisfy)
					{
						Eff_BreakButton.SetActive(true);
					}
				}
				else
				{
					if(Eff_BreakButton.activeInHierarchy)
					{
						Eff_BreakButton.SetActive(false);
					}
					Button_Break.gameObject.SetActive(false);
				}

				var yaoNvData = SirenManager.Instance.GetYaoNvList().SingleOrDefault(p=>p.byYaoNvID == sirenId);
				int curExp = yaoNvData.lExperience;
				int maxExp = m_SirenItemDict[sirenId].MaxExperience <= 0 ? 1 : m_SirenItemDict[sirenId].MaxExperience;
				Slider_SirenProcess.sliderValue = curExp*1f/maxExp;
				Label_SirenProcess.text = curExp.ToString()+"/"+maxExp.ToString();
				//说明
				Label_SirenNextLevelTxt.text = string.Format(LanguageTextManager.GetString("IDS_I2_9"),LanguageTextManager.GetString(m_SirenItemDict[sirenId].GetLevelUpText()));
			}
								
			//战力 
			int allAttributes = SirenDataManager.Instance.GetSirenCombatValue(sirenId);
			Label_Combat.text = allAttributes.ToString();
									
			
			//显示名字
			//Sprite_SirenName.spriteName = playerSirenConfigData._nameRes;
			//改成prefab
			if(m_curSirenName != null) Destroy(m_curSirenName);
			m_curSirenName = (GameObject)Instantiate(sirenData._NamePrefab);
			m_curSirenName.transform.parent = SirenNameTrans;
			m_curSirenName.transform.localPosition = Vector3.zero;
			m_curSirenName.transform.localScale = Vector3.one;
			if(m_curSirenTitle != null) Destroy(m_curSirenTitle);
			m_curSirenTitle = (GameObject)Instantiate(sirenData._TitlePrefab);
			m_curSirenTitle.transform.parent = SirenTitleTrans;
			m_curSirenTitle.transform.localPosition = Vector3.zero;
			m_curSirenTitle.transform.localScale = Vector3.one;

			//城镇妖女按钮显示是否可以升级或突破特效
			if(SirenManager.Instance.IsHasSirenSatisfyIncrease())
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Siren);				                                     
			}
			else
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UIType.Siren);
			}

		}
		
		IEnumerator PlaySirenProcessIncreaseAnimation(int curSirenID, float startExp, float endExp, int maxExp)
		{
			//var yaoNvData = SirenManager.Instance.GetYaoNvList().SingleOrDefault(p=>p.byYaoNvID = sirenId);
			int curLevel = m_SirenItemDict[m_CurSelectedSirenItemID].CurLevel;
			if(maxExp<=0)
				startExp=endExp;
			while(startExp<endExp && m_CurSelectedSirenItemID == curSirenID && curLevel == m_SirenItemDict[m_CurSelectedSirenItemID].CurLevel)
			{
				startExp += (endExp - startExp)*Time.deltaTime;
				Slider_SirenProcess.sliderValue = startExp/maxExp;
				yield return null;
			}
		}
		
		void EffUnlockFlashing()
		{
			if (Swith_EffUnlock.gameObject.activeInHierarchy == false)
			{
				CancelInvoke("EffUnlockFlashing");
			}
			else
			{
				int time = (int)(Time.time * (1 / EffUnlockFlashTime));
				int swith = time % 2;                
				Swith_EffUnlock.ChangeSprite(swith + 1);
			}
			
		}

		//清除之前的残留特效
		private void ClearLegacyEffects()
		{
			m_LegacyEffectList.ApplyAllItem(p=>
			                                {
				if(p!=null)
					Destroy(p);
			});
			m_LegacyEffectList.Clear();
		}
		//播放特效
		private void PlaySirenEffect(GameObject gameObj, Transform parentTrans)
		{
			gameObj.transform.localScale = parentTrans.lossyScale;
			GameObject newObj = (GameObject)Instantiate(gameObj, parentTrans.transform.position, gameObj.transform.rotation);
			DestroySelf ds = newObj.AddComponent<DestroySelf>();
			ds.Time = 5f;
			m_LegacyEffectList.Add(newObj);
		}

		//播放按钮动画
		private void PlayButtonAnimation()
		{
			var tweenSceleArray = Button_Exit.transform.parent.GetComponentsInChildren<TweenScale>();
			tweenSceleArray.ApplyAllItem(p =>
			                             {
				p.Reset();
				p.Play(true);
			});
		}
		
		//处理炼化结果
		void LianHuaResultHandle(INotifyArgs arg)
		{
			m_LianHuaResult = (SMsgActionLianHua_SC)arg;
			
		}
		
		void AddColdWork(object obj)
		{
		}
		
		//妖女参战成功
		void SirenJoinSuccessHandle(object obj)
		{
			PlaySirenEffect(EffSirenBattleInPrefab,EffSirenBattleInTrans);
			//SirenBeSelectedHandle(m_CurSelectedSirenItemID);
			SoundManager.Instance.PlaySoundEffect(m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData()._BattleVoice);

			if(m_CurJoinSirenName != null)
				Destroy(m_CurJoinSirenName);
			var sirenList = SirenDataManager.Instance.GetPlayerSirenList();
			if(SirenManager.Instance.GetYaoNvList().Any(p=>p.byAssembly == 1))
			{				
				var sirenContext = SirenManager.Instance.GetYaoNvList().FirstOrDefault(p=>p.byAssembly == 1);
				var sirenData = sirenList.SingleOrDefault(p=>p._sirenID == sirenContext.byYaoNvID);
				var sirenThisLevelData = sirenData._sirenConfigDataList.SingleOrDefault(level=>level._growthLevels == sirenContext.byLevel);
				if(sirenData!=null)
				{
					m_CurJoinSirenName = UI.CreatObjectToNGUI.InstantiateObj(sirenThisLevelData._NamePrefab , SirenJoinNameTrans);
				}
			}

			//
		}
		

		
		void RemoveColdWork(object obj)
		{
		}
		private string PraseClock(uint time)
		{
			string str = time.ToString();
			if (time < 10)
			{
				str = "0" + str;
			}
			return str;
		}

		protected override void RegisterEventHandler()
		{
			//ItemPageManager_Siren.OnPageChanged += this.OnPageChanged;
			//AddEventHandler(EventTypeEnum.SirenBeSelected.ToString(), SirenBeSelectedHandle);
			AddEventHandler(EventTypeEnum.LianHuaResult.ToString(), LianHuaResultHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.RemoveColdWork, RemoveColdWork);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.AddColdWork, AddColdWork);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.YaoNvJoinSuccess, SirenJoinSuccessHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.IntellJumpSiren, SelectSirenByIDCallback);
		}
	}
}