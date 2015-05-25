using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;

namespace UI.Team
{
	/// <summary>
	/// 组队主面板
	/// </summary>
	public class TeamListPanel_V2 : View
	{
		public UIGrid MyUIGrid;                     //队伍信息UI的父体    
		public TeamInfoItem ATeamInfoItem;          //队伍信息项 

		public UIDraggablePanel Draggable;
		public UIGrid Grid;
		public GameObject WorldTeamItemPrefab;	


		public GameObject Eff_Team_ChangeCool_Prefab;
		//public UIPanel ClippingPanel;               //裁剪视口面板
		//private Vector4 mClipRange;                 //裁剪范围
		
		//public ItemPagerManager ItemPageManager_Team;
		
		public LocalButtonCallBack RefreshTeamInfoButtonCallBack;       //刷新按钮    
		public UIFilledSprite RefreshCDSprite;

		public LocalButtonCallBack Button_ChangeArea; 					//更换区域
		//public LocalButtonCallBack LookingForTeamButtonCallBack;        //寻找队伍
		public LocalButtonCallBack QuickJoinTeamButtonCallBack;         //快速加入
		public LocalButtonCallBack CreateTeamButtonCallBack;            //创建队伍
		public LocalButtonCallBack ChatButtonCallBack;                  //聊天
		//public LocalButtonCallBack LookingForCaptainButtonCallBack;     //寻找队长
		public LocalButtonCallBack ReturnButtonCallBack;                //返回
		
		public UILabel Label_AreaTitle;
		
		public Transform SearchingEff;  //搜索特效
		public GameObject NoneTeamTip;
		public UILabel Label_noneTeam;
		
		private int[] m_guideBtnID = new int[4];
		
		private List<WorldTeamItem> TeamInfoItemList = new List<WorldTeamItem>(); //储存回收TeamInfoItem

		private UIPanel m_thisPanel;

		private bool m_isQuickJoinTeamBtnCD = false;

		public enum PANEL_TYPE
		{
			Ectype = 0,
			Crusade,
		}
		public PANEL_TYPE LastPanel = PANEL_TYPE.Ectype;
	

		//\假设网络过来的信息:
		private int mItemNum = 3;
		
		private const float m_RefreshCDTime = 10f;
		
		void Awake()
		{
			//TODO GuideBtnManager.Instance.RegGuideButton(ReturnButtonCallBack.gameObject, MainUI.UIType.TeamInfo, SubType.ButtomCommon, out m_guideBtnID[0]);
			//TODO GuideBtnManager.Instance.RegGuideButton(QuickJoinTeamButtonCallBack.gameObject, MainUI.UIType.TeamInfo, SubType.ButtomCommon, out m_guideBtnID[1]);
			//TODO GuideBtnManager.Instance.RegGuideButton(CreateTeamButtonCallBack.gameObject, MainUI.UIType.TeamInfo, SubType.ButtomCommon, out m_guideBtnID[2]);
			////TODO GuideBtnManager.Instance.RegGuideButton(RefreshTeamInfoButtonCallBack.gameObject, MainUI.UIType.TeamInfo, SubType.ButtomCommon, out m_guideBtnID[3]);
			m_thisPanel = gameObject.GetComponent<UIPanel>();

			Label_noneTeam.text = LanguageTextManager.GetString("IDS_I13_6");
		}
		
		void Start()
		{            
			
			//mClipRange = ClippingPanel.clipRange;
			//RefreshTeamInfoButtonCallBack.SetCallBackFuntion(OnRefreshWorldTeamInfoClick, null);
			ReturnButtonCallBack.SetCallBackFuntion(OnCloseTeamMainPanel, null);
			CreateTeamButtonCallBack.SetCallBackFuntion(OnCreateTeamClick, null);
			QuickJoinTeamButtonCallBack.SetCallBackFuntion(OnQuickJoinTeamClick, null);
			ChatButtonCallBack.SetCallBackFuntion(OnChatClick, null);
			Button_ChangeArea.SetCallBackFuntion(OnChangeAreaClick, null);


			RegisterEventHandler();

			TaskGuideBtnRegister();			
		}

		/// <summary>
		/// 引导按钮注入代码
		/// </summary>
		private void TaskGuideBtnRegister()
		{
			ReturnButtonCallBack.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_Back);
			CreateTeamButtonCallBack.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_CreateTeam);		
			QuickJoinTeamButtonCallBack.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_QuickJoin);
			ChatButtonCallBack.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_Chat);
			Button_ChangeArea.gameObject.RegisterBtnMappingId(UIType.TeamInfo, BtnMapId_Sub.TeamInfo_ChangeZone);
		}
		
		public void ShowPanel()
		{
			//SoundManager.Instance.PlaySoundEffect("Sound_UIEff_TeamUIAppear");
			m_thisPanel.alpha = 0;
			transform.localPosition = Vector3.zero;			            
			TweenAlpha.Begin(m_thisPanel.gameObject, 0.2f, 1f);
			UpdateAreaTitleLabel();
			OnRefreshWorldTeamInfoClick(0);//刷新队伍列表
		}
		
		public void ClosePanel()
		{
			//TweenAlpha.Begin(m_thisPanel.gameObject, 0.2f, 1f, 0, LateCloseHandle);
			transform.localPosition = new Vector3(0, 0, -800);
		}
		void LateCloseHandle(object obj)
		{
			transform.localPosition = new Vector3(0, 0, -800);
		}

		/// <summary>
		/// 是否显示无队伍
		/// </summary>
		public void UpdateNoneTeamTip()
		{
			StartCoroutine("LabelUpdateNoneTeamTip");
		}
		IEnumerator LabelUpdateNoneTeamTip()
		{
			yield return new WaitForEndOfFrame();
			bool isNoneTeam = true;
			if(TeamInfoItemList.Count>0)
			{
				if(TeamInfoItemList.Any(p=>p!=null))
				{
					isNoneTeam = false;
				}
			}
			NoneTeamTip.SetActive(isNoneTeam);
		}

		public void UpdateAreaTitleLabel()
		{
			//更新副本区域名称显示
			var currentEctypeArea = TeamManager.Instance.CurSelectEctypeAreaData;
			Label_AreaTitle.text = LanguageTextManager.GetString(currentEctypeArea._szName);
			bool isNormalType = currentEctypeArea.lEctypeType != 9;	//9为讨伐副本
			Button_ChangeArea.gameObject.SetActive(isNormalType);
			//按钮动画
			//PlayButtonAnimation();
		}
		
		
		//刷新队伍
		public void OnRefreshWorldTeamInfoClick(object obj)
		{
			//SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");           
			
			//清掉之前的队伍信息
			CreateTeamInfoItems(new SMsgTeamNum_SC()
			                    {
				wTeamNum = 0,
			});

			//上发匹配队伍请求
			var player = PlayerManager.Instance.FindHeroDataModel();
			var currentEctypeArea = TeamManager.Instance.CurSelectEctypeAreaData;			
			NetServiceManager.Instance.TeamService.SendGetTeamListMsg(new SMSGGetTeamList_CS() 
			                                                          { 
				uidEntity = player.UID, 
				dwEctypeID = (uint)currentEctypeArea._lEctypeID, 
                byDifficulty = 0
			});
			
			if (obj == null)
			{
				StartCoroutine("RefreshCDTimeRestore");
				SearchingEff.gameObject.SetActive(true);                
				SearchingEff.animation.Play("JH_Eff_UI_TeamUpdate 1");
				StartCoroutine("Researching");
				NoneTeamTip.SetActive(false);
			}
		}
		
		IEnumerator RefreshCDTimeRestore()
		{
			RefreshCDSprite.fillAmount = 1;
			RefreshTeamInfoButtonCallBack.SetButtonActive(false);
			float i = 0;
			float rate = 1f / m_RefreshCDTime;
			while (i < 1f)
			{
				i += Time.deltaTime * rate;
				//
				RefreshCDSprite.fillAmount = 1 - i;
				
				yield return null;
			}
			RefreshCDSprite.fillAmount = 0;
			RefreshTeamInfoButtonCallBack.SetButtonActive(true);
		}
		IEnumerator Researching()
		{
			float time = CommonDefineManager.Instance.CommonDefine.LookingForTeamTime;
			yield return new WaitForSeconds(time);
			SearchingEff.animation.Play("JH_Eff_UI_TeamUpdate 2");
			time = SearchingEff.animation["JH_Eff_UI_TeamUpdate 2"].length;
			yield return new WaitForSeconds(time);
			//SearchingEff.animation.Stop();
			SearchingEff.gameObject.SetActive(false);
			if (TeamInfoItemList.Count <= 0)
			{
				NoneTeamTip.SetActive(true);
			}
		}
		
		void OnDestroy()
		{
			for (int i = 0; i < m_guideBtnID.Length; i++)
			{
				//TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
			}
		}
		//返回
		void OnCloseTeamMainPanel(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamBack");
			transform.parent.transform.localPosition = new Vector3(0, 0, -800);
			if(LastPanel == PANEL_TYPE.Ectype)
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UI.MainUI.UIType.Battle);
			}
			else
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UI.MainUI.UIType.Crusade);
			}
		}
		//快速加入队伍
		void OnQuickJoinTeamClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamQuickJoin");
			if (m_isQuickJoinTeamBtnCD)
				return;
			var player = PlayerManager.Instance.FindHeroDataModel();
			var ectypeData = TeamManager.Instance.CurSelectEctypeAreaData;
			NetServiceManager.Instance.TeamService.SendTeamFastJoinMsg(new SMsgTeamFastJoin_CS()
            {
				dwEctypeId = (uint)ectypeData._lEctypeID,
				dwActorId = (uint)player.ActorID,
				byEctypeDiff = 0,
			});
			CreateQuickJoinCoolEff();
			StartCoroutine("QuickJoinTeamBtnCD");
		}
		IEnumerator QuickJoinTeamBtnCD()
		{
			m_isQuickJoinTeamBtnCD = true;
			yield return new WaitForSeconds(2f);
			m_isQuickJoinTeamBtnCD = false;
		}
		
		//创建队伍
		void OnCreateTeamClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamCreate");

			UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseWorldChatWindow,null);//如果聊天窗口打开，关闭

			var currentEctypeArea = TeamManager.Instance.CurSelectEctypeAreaData;
			if(currentEctypeArea.lEctypeType == 0)
			{
				UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowTeamChildPanel, ChildPanel.SelectEctype);
			}
			else if(currentEctypeArea.lEctypeType == 9)
			{
				NetServiceManager.Instance.TeamService.SendTeamCreateMsg(currentEctypeArea._lEctypeID, 1, 0);
			}
		}
		
		void OnChatClick(object obj)
		{
			MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Chat);
			//UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenWorldChatWindow, null);
		}

		void OnChangeAreaClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamReplaceArea");
			UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseWorldChatWindow,null);//如果聊天窗口打开，关闭
			UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowTeamChildPanel,ChildPanel.SelectAreaForFilter);
		}

		/// <summary>
		/// 刷新队伍列表冷却
		/// </summary>
		public void CreateRefreshCoolEff()
		{
			GameObject eff = UI.CreatObjectToNGUI.InstantiateObj(Eff_Team_ChangeCool_Prefab, Button_ChangeArea.transform);
			eff.transform.localPosition += new Vector3(-29.8f,4.9f,-10f);
			//清除列表
			CreateTeamInfoItems(new SMsgTeamNum_SC(){
				wTeamNum = 0
			});
		}
		/// <summary>
		/// 快速加入队伍冷却
		/// </summary>
		public void CreateQuickJoinCoolEff()
		{
			GameObject eff = UI.CreatObjectToNGUI.InstantiateObj(Eff_Team_ChangeCool_Prefab, QuickJoinTeamButtonCallBack.transform);
			eff.transform.localPosition += new Vector3(-29.8f,4.9f,-10f);
		}

		public void CreateTeamInfoItems(SMsgTeamNum_SC sMsgTeamNum)
		{            
			mItemNum = sMsgTeamNum.wTeamNum;    //队伍数量

			if (mItemNum <= 0)
			{
				NoneTeamTip.SetActive(true);
			}
			else
			{
				NoneTeamTip.SetActive(false);
			}
			/*
            if (mItemNum > TeamInfoItemList.Count)
            {
                int addNum = mItemNum - TeamInfoItemList.Count;
                for (int i = 0; i < addNum; i++)
                {
                    GameObject obj = ((GameObject)Instantiate(ATeamInfoItem.gameObject));
                    TeamInfoItem item = obj.GetComponent<TeamInfoItem>();
                    item.InitInfo(i, MyUIGrid.transform);
                    TeamInfoItemList.Add(item);

                    //\
                    item.UpdateInfo(sMsgTeamNum.SMsgTeamProps[i]);
                }
                //TeamInfoItemList.ApplyAllItem(p => p.UpdateInfo());
            }
            else
            {
                int num = 0;
                TeamInfoItemList.ApplyAllItem(p =>
                {
                    if (num < mItemNum)
                    {
                        //\
                        p.UpdateInfo(sMsgTeamNum.SMsgTeamProps[num]);
                    }
                    else
                    {
                        p.Close();
                    }
                    num++;
                });
            }
            //排列
            MyUIGrid.repositionNow = true;

            */
			#region new page
			/*
            if (mItemNum > TeamInfoItemList.Count)
            {
                int addNum = mItemNum - TeamInfoItemList.Count;
                for (int i = 0; i < addNum; i++)
                {
                    GameObject obj = ((GameObject)Instantiate(ATeamInfoItem.gameObject));
                    TeamInfoItem item = obj.GetComponent<TeamInfoItem>();
                    item.InitInfo(i, ItemPageManager_Team.transform);
                    TeamInfoItemList.Add(item);
                    item.UpdateInfo(sMsgTeamNum.SMsgTeamProps[i]);
                }                
            }
            else
            {
                int num = 0;
                TeamInfoItemList.ApplyAllItem(p =>
                {
                    if (num < mItemNum)
                    {
                        p.UpdateInfo(sMsgTeamNum.SMsgTeamProps[num]);
                    }
                    else
                    {
                        p.Close();
                    }
                    num++;
                });
            }
            */
			#endregion
			
			TeamInfoItemList.ApplyAllItem(p =>
			                              {
				if(p!=null && p.gameObject!=null)
				{
					Destroy(p.gameObject);
				}
			});
			TeamInfoItemList.Clear();
			for (int i = 0; i < mItemNum; i++)
			{
				GameObject obj = UI.CreatObjectToNGUI.InstantiateObj(WorldTeamItemPrefab, Grid.transform);
				WorldTeamItem item = obj.GetComponent<WorldTeamItem>();
				//item.InitInfo(i);
				item.UpdateInfo(sMsgTeamNum.SMsgTeamProps[i]);
				item.PlayAppearAnimation((i - 1)*0.1f);
				TeamInfoItemList.Add(item);
			}
			
			StartCoroutine("LateGridReposition");			
		}
		/// <summary>
		/// 重置位置
		/// </summary>
		public void RepositionList()
		{
			StartCoroutine("LateGridReposition");
		}
		IEnumerator LateGridReposition()
		{
			yield return new WaitForEndOfFrame();
			//Draggable.ResetPosition();
			Grid.Reposition();	
			Vector3 pos = Draggable.transform.localPosition;
			Vector4 bound = Draggable.panel.clipRange;
			//Draggable.SetDragAmount(0, 0, false);
			pos.y = 0;
			bound.y = 0;
			Draggable.transform.localPosition = pos;
			Draggable.panel.clipRange = bound;
		}
		public void ItemPageChanged(PageChangedEventArg pageSmg)
		{
			TeamInfoItemList.ApplyAllItem(p =>
			                              {
				p.transform.position = new Vector3(-2000, 0, 0);
				//p.gameObject.SetActive(false);
			});
			//int size = ItemPageManager_Team.PagerSize;
			//var showTeamInfoArray = TeamInfoItemList.Skip((pageSmg.StartPage - 1) * size).Take(size).ToArray();
			//showTeamInfoArray.ApplyAllItem(p =>
			//    {
			//        p.gameObject.SetActive(true);
			//        var tweenScele = p.GetComponentInChildren<TweenScale>();
			//        tweenScele.Reset();
			//        tweenScele.Play(true);                                                    
			//    });
			//ItemPageManager_Team.UpdateItems(showTeamInfoArray, "teamList");
			
			//加入百叶窗效果            
//			int arrayLength = showTeamInfoArray.Length;
//			for (int i = 0; i < arrayLength; i++)
//			{
//				showTeamInfoArray[i].PlayShutterAnimation(i * 0.1f);
//			}
		}
		//播放按钮动画
		private void PlayButtonAnimation()
		{
			var tweenSceleArray =  ReturnButtonCallBack.transform.parent.GetComponentsInChildren<TweenScale>();
			tweenSceleArray.ApplyAllItem(p =>
			                             {
				p.Reset();
				p.Play(true);
			});
			var tweenPositionArray = ReturnButtonCallBack.transform.parent.GetComponentsInChildren<TweenPosition>();
			tweenPositionArray.ApplyAllItem(p =>
			                                {
				p.Reset();
				p.Play(true);
			});
		}
		
		protected override void RegisterEventHandler()
		{
			
		}
	}
}
