using UnityEngine;
using System.Collections;
using UI.Friend;
using System.Collections.Generic;
using UI.MainUI;
using UI;
using System.Linq;

public class FriendPanel_V3 : BaseUIPanel 
{
	public LocalButtonCallBack Button_Town;
	public LocalButtonCallBack Button_MyFriend;
	public LocalButtonCallBack Button_Request;
	public LocalButtonCallBack Button_Exit;

//	public UILabel Label_TownPeopleNum;
//	public UILabel Label_FriendNum;
//	public UILabel Label_RequestNum;

	public GameObject FriendItemPrefab;

	public FriendChildType[] FriendChildren;

	public GameObject NewRequestTip;

	private Dictionary<FriendBtnType, LocalButtonCallBack> m_ButtonDict = new Dictionary<FriendBtnType, LocalButtonCallBack>();
	private Dictionary<FriendBtnType,FriendChildPanel> m_ChildPanelDict = new Dictionary<FriendBtnType, FriendChildPanel>();
	private FriendBtnType m_curFriendChildPanelType;


	//town
	private List<NearlyItem> m_panelElementList = new List<NearlyItem>();
	
	void Awake()
	{
		Button_Town.SetCallBackFuntion(OnTownClick,null);
		Button_MyFriend.SetCallBackFuntion(OnMyFriendClick,null);
		Button_Request.SetCallBackFuntion(OnRequestClick,null);
		Button_Exit.SetCallBackFuntion(OnExitClick,null);

		m_ButtonDict.Add(FriendBtnType.Town,Button_Town);
		m_ButtonDict.Add(FriendBtnType.MyFriend,Button_MyFriend);
		m_ButtonDict.Add(FriendBtnType.Request,Button_Request);

		FriendChildren.ApplyAllItem(p=>{
			if(!m_ChildPanelDict.ContainsKey(p.Type))
			{
				m_ChildPanelDict.Add(p.Type,p.Panel);
			}
		});

		RegisterEventHandler();
	}

	public override void Show(params object[] value)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_FriendUIAppear");

		NetServiceManager.Instance.FriendService.SendNearbyPlayerRequst((uint)PlayerManager.Instance.FindHeroDataModel().ActorID);
		ShowMyFriendList();
		ShowRequestList();

		//如果有好友请求，优先处理好友请求
		bool isHadQuest = false;
		for(int i=0;i<FriendDataManager.Instance.GetRequestListData.Count;i++)
		{
			if( !FriendDataManager.Instance.GetFriendListData.Any(k=>k.sMsgRecvAnswerFriends_SC.dwFriendID
		  	 ==FriendDataManager.Instance.GetRequestListData[i].sMsgRecvAnswerFriends_SC.dwFriendID))
			{
				isHadQuest = true;
				break;
			}
		}
		if(isHadQuest)
		{
			ShowChildPanel(FriendBtnType.Request);
		}
		else
		{
			ShowChildPanel(FriendBtnType.MyFriend);
		}

		base.Show(value);
	}
	
	public override void Close()
	{
		if (!IsShow)
			return;
		if(MainUIController.Instance.NextUIStatus == UIType.Mail)
			return;
		base.Close();
	}

	protected override void RegisterEventHandler()
	{
		this.AddEventHandler(EventTypeEnum.AddFriendSuccess.ToString(), AddFriendSuccessHandle);
		this.AddEventHandler(EventTypeEnum.RefreshFriendList.ToString(),RefreshFriendListHandle);
		this.AddEventHandler(EventTypeEnum.RevNearlyPlayer.ToString(), ShowNearlyPlayerHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.EctypeLevelError, ShowFriendEctypeLockMsg);
		//UIEventManager.Instance.RegisterUIEvent(UIEventType.PlayMainBtnAnim, ReceivePlayAnimation);
	}
	
	void OnDestroy()
	{
		this.RemoveAllEvent();
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EctypeLevelError, ShowFriendEctypeLockMsg);
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.PlayMainBtnAnim, ReceivePlayAnimation);
	}

	#region  按钮事件
	void OnTownClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Friendlist");
		ShowChildPanel(FriendBtnType.Town);
	}
	void OnMyFriendClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Friendlist");
		ShowChildPanel(FriendBtnType.MyFriend);
	}
	void OnRequestClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Friendlist");
		ShowChildPanel(FriendBtnType.Request);
	}
	void OnExitClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_FriendBack");
		base.Close();
	}
	#endregion

	//显示子面板
	private void ShowChildPanel(FriendBtnType type)
	{
		m_curFriendChildPanelType = type;

		//开启关闭面板
		m_ChildPanelDict.ApplyAllItem(p=>{
			if(p.Key == type)
			{
				p.Value.Show();
			}
			else
			{
				p.Value.Close();
			}
		});
		//切换按钮状态
		m_ButtonDict.ApplyAllItem(p=>{
			if(p.Key == type)
			{
				p.Value.SetBoxCollider(false);
				p.Value.SetSwith(2);
			}
			else
			{
				p.Value.SetBoxCollider(true);
				p.Value.SetSwith(1);
			}
		});


		switch(type)
		{
		case FriendBtnType.Town:
			m_ChildPanelDict[FriendBtnType.Town].Clear();
			NetServiceManager.Instance.FriendService.SendNearbyPlayerRequst((uint)PlayerManager.Instance.FindHeroDataModel().ActorID);
			break;
		case FriendBtnType.MyFriend:
			ShowMyFriendList();
			break;
		case FriendBtnType.Request:
			ShowRequestList();
			break;
		}

	}

	private void ShowMyFriendList()
	{
		List<PanelElementDataModel> online = new List<PanelElementDataModel>();
		List<PanelElementDataModel> unline = new List<PanelElementDataModel>();
		List<PanelElementDataModel> list = new List<PanelElementDataModel>();
		FriendDataManager.Instance.GetFriendListData.ApplyAllItem(p=>{
			if(p.sMsgRecvAnswerFriends_SC.bOnLine == 1)
			{
				online.Add(p);
			}
		});
		online.Sort(new FriendComparerByLevel());
		FriendDataManager.Instance.GetFriendListData.ApplyAllItem(p=>{
			if(p.sMsgRecvAnswerFriends_SC.bOnLine == 0)
			{
				unline.Add(p);
			}
		});
		unline.Sort(new FriendComparerByLevel());
		list.AddRange(online);
		list.AddRange(unline);
		m_ChildPanelDict[FriendBtnType.MyFriend].CreateItems(list, FriendBtnType.MyFriend);		 
	}
	private void ShowRequestList()
	{
		List<PanelElementDataModel> list = new List<PanelElementDataModel>();
		FriendDataManager.Instance.GetRequestListData.ApplyAllItem(p=>{
			if(!FriendDataManager.Instance.GetFriendListData.Any(k=>k.sMsgRecvAnswerFriends_SC.dwFriendID == p.sMsgRecvAnswerFriends_SC.dwFriendID))
			{
				if(list.Count < CommonDefineManager.Instance.CommonDefine.FriendRequestLimit)
				{
					list.Add(p);	
				}
			}
		});
		NewRequestTip.SetActive(list.Count>0);
		m_ChildPanelDict[FriendBtnType.Request].CreateItems(list, FriendBtnType.Request);	
	}

	void AddFriendSuccessHandle(INotifyArgs notifyArgs)
	{
		//刷新
		m_ChildPanelDict[FriendBtnType.Town].Clear();
		NetServiceManager.Instance.FriendService.SendNearbyPlayerRequst((uint)PlayerManager.Instance.FindHeroDataModel().ActorID);
		ShowMyFriendList();
		ShowRequestList();
//		if (IsShow)
//		{
//			ShowChildPanel(FriendBtnType.MyFriend);
//		}
	}
	void RefreshFriendListHandle(INotifyArgs args)
	{
		ShowMyFriendList();
	}
	void ShowNearlyPlayerHandle(INotifyArgs notifyArgs)
	{
		if (LoadingUI.Instance != null)
		{
			LoadingUI.Instance.Close();
		}
		var NearlySMsgGetActorListHead = (SMsgGetActorListHead)notifyArgs;
		m_panelElementList.Clear();
		for (int i = 0; i < NearlySMsgGetActorListHead.dwFriendNum; i++)
		{
			PanelElementDataModel playerElementData = new PanelElementDataModel();
			playerElementData.sMsgRecvAnswerFriends_SC = NearlySMsgGetActorListHead.sMsgRecvAnswerFriends_SC[i];
			playerElementData.BtnType = ButtonType.NearlyPlayer;
			if (!m_panelElementList.Exists(P => P.dwFriendID == playerElementData.sMsgRecvAnswerFriends_SC.dwFriendID))
			{
				
				NearlyItem nearlyItem = new NearlyItem();
				nearlyItem.Index = i;
				nearlyItem.dwFriendID = playerElementData.sMsgRecvAnswerFriends_SC.dwFriendID;
				nearlyItem.element = playerElementData;
				nearlyItem.m_isFriend = FriendDataManager.Instance.GetFriendListData.Exists(P => P.sMsgRecvAnswerFriends_SC.dwFriendID == playerElementData.sMsgRecvAnswerFriends_SC.dwFriendID);
				m_panelElementList.Add(nearlyItem);
			}
		}

//		if(m_curFriendChildPanelType == FriendBtnType.Town)
//		{
			List<PanelElementDataModel> list = new List<PanelElementDataModel>();
			List<PanelElementDataModel> myFriendList = FriendDataManager.Instance.GetOnlineFriendList;
			m_panelElementList.ApplyAllItem(p=>{
				if(!myFriendList.Any(k=>k.sMsgRecvAnswerFriends_SC.dwFriendID == p.dwFriendID))
				{
					list.Add(p.element);
				}
			});
			list.Sort(new FriendComparerByLevel());
			m_ChildPanelDict[FriendBtnType.Town].CreateItems(list, FriendBtnType.Town);
//		}

	}
	
	/// <summary>
	/// 好友副本未解锁
	/// </summary>
	void ShowFriendEctypeLockMsg(object obj)
	{
		if (IsShow)
		{
			MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_84"), 1);
		}
	}
	
	public class FriendComparerByLevel : IComparer<PanelElementDataModel>
	{
		public int Compare(PanelElementDataModel x, PanelElementDataModel y)
		{
			int compareResult = 1;
			
			if(x.sMsgRecvAnswerFriends_SC.sActorLevel > y.sMsgRecvAnswerFriends_SC.sActorLevel)
			{
				compareResult = -1;
			}
			else if(x.sMsgRecvAnswerFriends_SC.sActorLevel == y.sMsgRecvAnswerFriends_SC.sActorLevel)
			{
				compareResult = x.sMsgRecvAnswerFriends_SC.dwFriendID < y.sMsgRecvAnswerFriends_SC.dwFriendID ? -1: 1;
			}
			
			return compareResult;
		}
	}

	
	[System.Serializable]
	public class FriendChildType
	{
		public FriendBtnType Type;
		public FriendChildPanel Panel;
	}

	class NearlyItem
	{
		public int Index;
		public uint dwFriendID; 
		public bool m_isFriend;
		public PanelElementDataModel element;
	}
	
}
