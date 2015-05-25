using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UI.Team;
using UI.Friend;
using UI;

public class PvpViewMode : View {


	public GameObject HeroTitlePrefab;
	public Transform heroTitlePoint;
	public Vector3 HeroTitleOffset=new Vector3(0,350,0);
	public Vector3 InviteBtnOffset=new Vector3(0,0,0);
	public Vector3  StarMatchBtnOffset=new Vector3(0,0,0);
	public SingleButtonCallBack StarMatchBtn;
	public SingleButtonCallBack InviteBtn_left;
	public SingleButtonCallBack InviteBtn_Right;
	public SingleButtonCallBack CancelTeamBtn;
	public Transform InvitePanelPoint;
	public GameObject InvitePanel_prefab;
	private PVPTeamInvitepanel teamInvitePanel;
	private GameObject leftPos;
	private GameObject centerpos;
	private GameObject rightpos;
	private Camera PvpCamera;
	private PvpRoleInfo leftChar=new PvpRoleInfo();
	private PvpRoleInfo centerChar=new PvpRoleInfo();
	private PvpRoleInfo rightChar=new PvpRoleInfo();
	private int m_HeroVocationID;
	private int CreatOrder;
	private GameObject m_LeftheroTitle;
	private GameObject m_CenterheroTitle;
	private GameObject m_RightheroTitle;
	private Camera m_uiCamera;
	private SGroupMemberInfo m_MyInfo;
	private bool IsShowing;
	void Awake()
	{
		m_uiCamera=BattleManager.Instance.UICamera;
		FindObject ();
		creatHeroTiotle ();
		setBtnCallback ();
		SetMyInfo ();
		CreatMyHeroMode ();
		RegisterEventHandler();
	}

	void Start()
	{
		SetBtnPos();
	}

	#region implemented abstract members of View
	protected override void RegisterEventHandler ()
	{
		UIEventManager.Instance.RegisterUIEvent(UIEventType.pvpStartmatch,PvpStartMatch);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.pvpSyncTeam,PvpSyncTeam);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.pvpFriendCancelTeam,FriendCanceTeam);

	}




	void ButtonShow(GameObject buttonGo)
	{
		buttonGo.SetActive(true);
		TweenScale.Begin(buttonGo,0.1666f,new Vector3(0,0,0),new Vector3(1,1,1),null);
	}

	void FriendCanceTeam(object obj)
	{
		int FriendID=(int)obj;
		if(leftChar.ActorID==FriendID)
		{
			leftChar.currentRoleMode.SetActive(false);
			leftChar.currentRoleMode=null;
			leftChar.ActorID=0;
		}
		else if(rightChar.ActorID==FriendID)
		{
			rightChar.currentRoleMode.SetActive(false);
			rightChar.currentRoleMode=null;
			rightChar.ActorID=0;
		}

	}

	void PvpStartMatch(object obj)
	{
		if(PvpDataManager.Instance.IsTeamLeader)
		{
			StarMatchBtn.gameObject.SetActive(false);

		}
		else
		{
			CancelTeamBtn.gameObject.SetActive(false);
		}
	    if(leftChar.ActorID!=0)
		{
			PlayAnimation(leftChar,false);
		}
		if(centerChar.ActorID!=0)
		{
			PlayAnimation(centerChar,false);
		}
		if(rightChar.ActorID!=0)
		{
			PlayAnimation(rightChar,false);
		}
		UI.MessageBox.Instance.Show(1,null,"正在进行战前匹配....","取消匹配",CancelMatch);

	}
	void PvpSyncTeam(object obj)
	{
		if(!IsShowing)
			return ;
		var list=PvpDataManager.Instance.GetGroupmeberInfoList();
		foreach(var c in list )
		{
			int dwId=(int)c.dwActorID;
			if(dwId!=m_HeroVocationID&&dwId!=leftChar.ActorID&&dwId!=rightChar.ActorID)
			{
				if(leftChar.ActorID==0)
				{
					AddRoleMode(PvpRolePos.LeftPos,(int)c.dwVocation);
					leftChar.RoleControl.ChangeWearponAndFashion(leftChar.currentRoleMode,(int)c.dwWEAPON,(int)c.dwFashion,leftChar.playerGenerateConfigData.Avatar_WeaponPos);
					CreatOrder++;
					leftChar.ActorID=(int)c.dwActorID;
					PlayAnimation(leftChar,false);
					ShowHeroTitle(PvpRolePos.LeftPos,c);
					continue;
				}

				if(rightChar.ActorID==0)
				{
					
					AddRoleMode(PvpRolePos.RightPos,(int)c.dwVocation);
					leftChar.RoleControl.ChangeWearponAndFashion(rightChar.currentRoleMode,(int)c.dwWEAPON,(int)c.dwFashion,rightChar.playerGenerateConfigData.Avatar_WeaponPos);
					CreatOrder++;
					rightChar.ActorID=(int)c.dwActorID;
					PlayAnimation(rightChar,false);
					ShowHeroTitle(PvpRolePos.LeftPos,c);
					continue;
				}

			}
		}
		if(list.Count==3&&PvpDataManager.Instance.IsTeamLeader)
		{
			InviteBtn_left.gameObject.SetActive(false);
			InviteBtn_Right.gameObject.SetActive(false);
		}
	}
	void CancelMatch()
	{
		PvpDataManager.Instance.CancelMatch();
		PvpUiPanelManager.Instance.Close();

	}
	#endregion
	void CreatMyHeroMode ()
	{
		m_HeroVocationID = PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
	
		CreatRoleMode (centerChar, m_HeroVocationID, centerpos.transform);
	
	}

	void ChangeMyHeroWeaponAndFashion()
	{
		int fashionID = PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
		var WeaponInfo = ContainerInfomanager.Instance.GetCurrentWeaponItemInfo ();
		int weaponID = WeaponInfo.LocalItemData._goodID;
		centerChar.RoleControl.ChangeWearponAndFashion (centerChar.currentRoleMode, weaponID, fashionID, centerChar.playerGenerateConfigData.Avatar_WeaponPos);
		centerChar.ActorID=PlayerManager.Instance.FindHeroDataModel ().ActorID;
	}


	void FindObject ()
	{
		centerpos = GameObject.Find ("Player01_TMap_QingLHui");
		leftPos = GameObject.Find ("Player02_TMap_QingLHui");
		rightpos = GameObject.Find ("Player03_TMap_QingLHui");
		PvpCamera = GameObject.Find ("Camera_TMap_QingLHui").camera;
	}
	
	void setBtnCallback ()
	{
		StarMatchBtn.SetCallBackFuntion (OnStarMatchBtnClick);
		InviteBtn_left.SetCallBackFuntion (OnInviteBtnClick);
		InviteBtn_Right.SetCallBackFuntion (OnInviteBtnClick);
		CancelTeamBtn.SetCallBackFuntion (OnCancelTeamBtnClick);
	}
	
	void creatHeroTiotle ()
	{
		m_LeftheroTitle = UI.CreatObjectToNGUI.InstantiateObj (HeroTitlePrefab, heroTitlePoint);
		m_CenterheroTitle = UI.CreatObjectToNGUI.InstantiateObj (HeroTitlePrefab, heroTitlePoint);
		m_RightheroTitle = UI.CreatObjectToNGUI.InstantiateObj (HeroTitlePrefab, heroTitlePoint);
	}

	void SetMyInfo ()
	{
		m_MyInfo = new SGroupMemberInfo ();
		m_MyInfo.dwVocation =(uint) PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
		m_MyInfo.dwFighting =(uint) PlayerDataManager.Instance.GetHeroForce ();
		m_MyInfo.dwLevel = (uint)PlayerManager.Instance.FindHeroDataModel ().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
		m_MyInfo.szName = PlayerManager.Instance.FindHeroDataModel ().m_name;
	}
	

	
	void ResetHeroTitle ()
	{
		m_CenterheroTitle.SetActive (false);
		m_LeftheroTitle.SetActive (false);
		m_RightheroTitle.SetActive (false);
	}

	public void Show()
	{
		IsShowing=true;
		InitBtn();
		ResetHeroTitle();
		PvpCamera.cullingMask=1<<LayerMask.NameToLayer("Player");
		ChangeMyHeroWeaponAndFashion();
		PlayAnimation(centerChar,true);
		ShowHeroTitle(PvpRolePos.CenterPos,m_MyInfo);
		List<SGroupMemberInfo> grouplist=PvpDataManager.Instance.GetGroupmeberInfoList();
		if(grouplist.Count!=0)
		{
			foreach(SGroupMemberInfo c in  grouplist)
			{
				if(c.dwActorID!=m_HeroVocationID)
				{
					if(leftChar.ActorID==0)
					{
						AddRoleMode(PvpRolePos.LeftPos,(int)c.dwVocation);
						leftChar.RoleControl.ChangeWearponAndFashion(leftChar.currentRoleMode,(int)c.dwWEAPON,(int)c.dwFashion,leftChar.playerGenerateConfigData.Avatar_WeaponPos);
						CreatOrder++;
						leftChar.ActorID=(int)c.dwActorID;
						PlayAnimation(leftChar,true);
						ShowHeroTitle(PvpRolePos.LeftPos,c);
						continue;
					}
					 if(rightChar.ActorID==0)
					{
						
						AddRoleMode(PvpRolePos.RightPos,(int)c.dwVocation);
						leftChar.RoleControl.ChangeWearponAndFashion(rightChar.currentRoleMode,(int)c.dwWEAPON,(int)c.dwFashion,rightChar.playerGenerateConfigData.Avatar_WeaponPos);
						CreatOrder++;
						rightChar.ActorID=(int)c.dwActorID;
						PlayAnimation(rightChar,true);
						ShowHeroTitle(PvpRolePos.RightPos,c);
						continue;
					}
				

				}
			}
		}
	}
	/// <summary>
	/// Plaies the animation.
	/// </summary>
	/// <param name="roleInfo">Role info.</param>
	/// <param name="isopening">只有在show方法中isopening=true <c>true</c> isopening.</param>
	void PlayAnimation(PvpRoleInfo roleInfo,bool isopening)
	{
		if(PvpDataManager.Instance.CurrentViewState==PVPViewState.InPage)
		{
			var Config=roleInfo.pvpConfigData;
			switch(PvpDataManager.Instance.CurrentMatchState)
			{
			case PVPMatchState.defult:

				roleInfo.RoleControl.PlayAnimation(Config.In_WeaponPos,Config.In_Ani,Config.In_Eff,Config.NotAdapIdle_WeaponPos,Config.NotAdapIdle_Ani,Config.NotAdapIdle_Eff);
				break;
			case PVPMatchState.Team:
				if(isopening)
				{
					roleInfo.RoleControl.PlayAnimation(null,null,null,Config.NotAdapIdle_WeaponPos,Config.NotAdapIdle_Ani,Config.NotAdapIdle_Eff);
				}
				else
				{
					roleInfo.RoleControl.PlayAnimation(Config.EnterGroup_WeaponPos,Config.EnterGroup_Ani,Config.EnterGroup_Eff,Config.NotAdapIdle_WeaponPos,Config.NotAdapIdle_Ani,Config.NotAdapIdle_Eff);
				}
				break;
			case PVPMatchState.Match:
				if(isopening)
				{
					roleInfo.RoleControl.PlayAnimation(null,null,null,Config.AdapIdle_WeaponPos,Config.AdapIdle_Ani,Config.AdapIdle_Eff);
				}
				else
				{
					roleInfo.RoleControl.PlayAnimation(Config.AdapStar_WeaponPos,Config.AdapStar_Ani,Config.AdapStar_Eff,Config.AdapIdle_WeaponPos,Config.AdapIdle_Ani,Config.AdapIdle_Eff);
				}
				break;

			}
		}
	}
	void ShowHeroTitle(PvpRolePos pos,SGroupMemberInfo merberInfo)
	{
		PvpHeroTitle pvptitle=new PvpHeroTitle();
		switch(pos)
		{
		case PvpRolePos.CenterPos:
		{
			m_CenterheroTitle.SetActive(true);
			m_CenterheroTitle.transform.position=PvpCamPointToUICampoint(centerpos.transform);
			Vector3 localpos=m_CenterheroTitle.transform.localPosition;
			m_CenterheroTitle.transform.localPosition=new Vector3(localpos.x+HeroTitleOffset.x,localpos.y+HeroTitleOffset.y,HeroTitleOffset.z);
			pvptitle=m_CenterheroTitle.GetComponent<PvpHeroTitle>();
			break;
		}
		
		case PvpRolePos.LeftPos:
		{
			m_LeftheroTitle.SetActive(true);
			m_LeftheroTitle.transform.position=PvpCamPointToUICampoint(leftPos.transform);
			Vector3 localpos=m_LeftheroTitle.transform.localPosition;
			m_LeftheroTitle.transform.localPosition=new Vector3(localpos.x+HeroTitleOffset.x,localpos.y+HeroTitleOffset.y,HeroTitleOffset.z);
			pvptitle=m_LeftheroTitle.GetComponent<PvpHeroTitle>();
			break;
		}

		case PvpRolePos.RightPos:
		{
			m_RightheroTitle.SetActive(true);
			m_RightheroTitle.transform.position=PvpCamPointToUICampoint(rightpos.transform);
			Vector3 localpos=m_RightheroTitle.transform.localPosition;
			m_RightheroTitle.transform.localPosition=new Vector3(localpos.x+HeroTitleOffset.x,localpos.y+HeroTitleOffset.y,HeroTitleOffset.z);
			pvptitle=m_RightheroTitle.GetComponent<PvpHeroTitle>();
			break;
		}

		}
		pvptitle.ShowHeroTitle((int)merberInfo.dwVocation,(int)merberInfo.dwFighting,"LV"+merberInfo.dwLevel,Encoding.UTF8.GetString(merberInfo.szName));

	}



	Vector3 PvpCamPointToUICampoint(Transform tran)
	{
	 Vector3 viewpoint=	PvpCamera.WorldToViewportPoint(tran.position);
     Vector3 uipos= m_uiCamera.ViewportToWorldPoint(viewpoint); 
	 return  uipos;
	}


	public void Hide()
	{
		PvpCamera.cullingMask=0;
		IsShowing=false;
		//PvpDataManager.Instance.ClearGroupmeberInfoList();
	}


	public void AddRoleMode(PvpRolePos pos,int VocationID)
	{
		if(pos==PvpRolePos.LeftPos)
		{
			CreatRoleMode (leftChar,VocationID,leftPos.transform);
		}
		else if(pos==PvpRolePos.RightPos)
		{
			CreatRoleMode (rightChar,VocationID,rightpos.transform);
		}
	}


	void CreatRoleMode (PvpRoleInfo Char,int VocationID,Transform point)
	{
		var pvpConfigData=PlayerDataManager.Instance.GetPVPItemData(VocationID);
		Char.pvpConfigData=pvpConfigData;
		Char.playerGenerateConfigData = PlayerDataManager.Instance.GetUIItemData ((byte)VocationID);
		if (Char.playerGenerateConfigData.PlayerId == 1) 
		{

			if(Char.RoleMode_Char01==null)
			{
				Char.RoleMode_Char01 = RoleGenerate.GenerateRole (Char.playerGenerateConfigData.PlayerName, Char.playerGenerateConfigData.DefaultAvatar, true);
				Char.RoleMode_Char01.transform.parent = point;
				Char.RoleMode_Char01.transform.localPosition=Vector3.zero;
				Char.RoleMode_Char01.transform.localEulerAngles=new Vector3(0,180,0);
				RoleGenerate.AttachAnimation(Char.RoleMode_Char01, pvpConfigData.PlayerName, pvpConfigData.DefaultAnim, pvpConfigData.Animations);
				Char.RoleControl= Char.RoleMode_Char01.AddComponent<PvpRoleControl>();
			}
			else
			{
				Char.RoleMode_Char01.SetActive(true);
				Char.RoleControl=Char.RoleMode_Char01.GetComponent<PvpRoleControl>();
			}
			Char.currentRoleMode=Char.RoleMode_Char01;
		}
		else 
		{
			if(Char.RoleMode_Char04==null)
			{
				Char.RoleMode_Char04 = RoleGenerate.GenerateRole (Char.playerGenerateConfigData.PlayerName, Char.playerGenerateConfigData.DefaultAvatar, true);
				Char.RoleMode_Char04.transform.parent =point;
				Char.RoleMode_Char04.transform.localPosition=Vector3.zero;
				Char.RoleMode_Char04.transform.localEulerAngles=new Vector3(0,180,0);
				RoleGenerate.AttachAnimation(Char.RoleMode_Char04, pvpConfigData.PlayerName, pvpConfigData.DefaultAnim, pvpConfigData.Animations);
				Char.RoleControl=Char.RoleMode_Char04.AddComponent<PvpRoleControl>();
			}
			else
			{
				Char.RoleMode_Char04.SetActive(true);
				Char.RoleControl=Char.RoleMode_Char04.GetComponent<PvpRoleControl>();
			}
			Char.currentRoleMode=Char.RoleMode_Char04;
		}

	}


	void SetBtnPos()
	{
		InviteBtn_left.transform.position=PvpCamPointToUICampoint(leftPos.transform);
		Vector3 leftLocPos= InviteBtn_left.transform.localPosition;
		InviteBtn_left.transform.localPosition=new Vector3(leftLocPos.x+InviteBtnOffset.x,leftLocPos.y+InviteBtnOffset.y,InviteBtnOffset.z);

		StarMatchBtn.transform.position=PvpCamPointToUICampoint(centerpos.transform);
		Vector3 CenterLocPos= StarMatchBtn.transform.localPosition;
		StarMatchBtn.transform.localPosition=new Vector3(CenterLocPos.x+StarMatchBtnOffset.x,CenterLocPos.y+StarMatchBtnOffset.y,StarMatchBtnOffset.z);
		CancelTeamBtn.transform.position=StarMatchBtn.transform.transform.position; 

		InviteBtn_Right.transform.position=PvpCamPointToUICampoint(rightpos.transform);
		Vector3 RightLocPos= InviteBtn_Right.transform.localPosition;
		InviteBtn_Right.transform.localPosition=new Vector3(RightLocPos.x+InviteBtnOffset.x,RightLocPos.y+InviteBtnOffset.y,InviteBtnOffset.z);
	}
	void InitBtn()
	{
		if(PvpDataManager.Instance.IsTeamLeader)
		{
			ButtonShow(StarMatchBtn.gameObject);
			ButtonShow(InviteBtn_left.gameObject);
			ButtonShow(InviteBtn_Right.gameObject);
			CancelTeamBtn.gameObject.SetActive(false);
		}
		else
		{
			StarMatchBtn.gameObject.SetActive(false);
			InviteBtn_left.gameObject.SetActive(false);
			InviteBtn_Right.gameObject.SetActive(false);
			CancelTeamBtn.gameObject.SetActive(true);
		}
	}


	void OnStarMatchBtnClick(object obj)
	{
		PvpDataManager.Instance.PvpStartMatch();
	}
	
	void OnInviteBtnClick(object obj)
	{
		if(teamInvitePanel==null)
		{
			teamInvitePanel=UI.CreatObjectToNGUI.InstantiateObj(InvitePanel_prefab,InvitePanelPoint).GetComponent<PVPTeamInvitepanel>();
		}
		List<uint> filterList=new List<uint>();
		PvpDataManager.Instance.GetGroupmeberInfoList().ApplyAllItem(c=>filterList.Add(c.dwActorID));
		teamInvitePanel.Show(filterList);
	}
	
	void OnCancelTeamBtnClick(object obj)
	{
		MessageBox.Instance.Show(1,null,LanguageTextManager.GetString("IDS_I38_3"),"取消","退出",()=>{
			PvpDataManager.Instance.CancelTeam();},
		    ()=>{MessageBox.Instance.CloseMsgBox();
		});

	}


}
public enum PvpRolePos
{
	LeftPos,
	CenterPos,
	RightPos,
}
public class PvpRoleInfo
{
	public int ActorID;//角色id
	public GameObject RoleMode_Char01;//
	public GameObject RoleMode_Char04;
	public GameObject currentRoleMode;
	public bool IsVisiable;//是否正在展示
	public PvpRoleControl RoleControl;
	public PlayerGenerateConfigData playerGenerateConfigData;
	public PlayerPvpConfigData pvpConfigData;
}