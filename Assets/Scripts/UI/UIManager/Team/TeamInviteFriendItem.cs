using UnityEngine;
using System.Collections;
using UI.Friend;
using System.Linq;
using UI.Team;

public class TeamInviteFriendItem : MonoBehaviour 
{
	enum TipType
	{
		EctypeIsLock = 1,
		InBattle = 2,
		NotEnoughLevel = 3,
		NotEnoughCrusadeTime = 4,
		NotEnoughCrusadeItem = 5,
	}

	public SpriteSwith AvatarIcon;
	public UISprite UI_Avatar;
	public UISprite UI_Profession;
	public UILabel Label_Name;
	public UILabel Label_Level;
	public UILabel Label_Combat;

	public LocalButtonCallBack InviteButton;

//	public GameObject Tip_Battling;
//	public GameObject Tip_DontMeet;
	public GameObject Tip_Restriction;
	public SpriteSwith Swith_Restriction;
	public UISprite UI_Restrication;

	private uint m_friendActorID = 0;
	public int FriendID
	{
		get{return (int)m_friendActorID;}
	}
	
	private Color m_greyColor = new Color(0.7176f, 0.7176f, 0.7176f);
	private Color m_cyanBlueColor = new Color(0.5176f, 0.7803f, 0.8117f);
	
	//private Transform m_thisTransform;
	private int m_tSecond = 0;
	
	void Start () 
	{        
		InviteButton.SetCallBackFuntion(OnInviteButtonClick, null);
	}
	
	
	public void UpdateInfo(PanelElementDataModel dataModel, EctypeContainerData ectypeData)
	{
		if (!transform.gameObject.activeInHierarchy)
		{
			transform.gameObject.SetActive(true);
		}
		
		m_friendActorID = dataModel.sMsgRecvAnswerFriends_SC.dwFriendID;
		
		Label_Name.text = dataModel.sMsgRecvAnswerFriends_SC.Name;
		Label_Level.text = LanguageTextManager.GetString("IDS_I12_10")+ dataModel.sMsgRecvAnswerFriends_SC.sActorLevel.ToString();
		Label_Combat.text = dataModel.sMsgRecvAnswerFriends_SC.dwFight.ToString();

		UI_Profession.spriteName = SpriteName.PROFESSION_CHAR + dataModel.sMsgRecvAnswerFriends_SC.dProfession.ToString();

		//UpdateRoleIcon(dataModel.sMsgRecvAnswerFriends_SC.bOnLine);
		//AvatarIcon.ChangeSprite((int)dataModel.sMsgRecvAnswerFriends_SC.dProfession);
		UpdateHeroIcon(dataModel.sMsgRecvAnswerFriends_SC.dProfession, dataModel.sMsgRecvAnswerFriends_SC.dbSysActorImageHeadID);
		
		//InviteButton.SetButtonStatus(false);
		//InviteButton.SetButtonText(LanguageTextManager.GetString("IDS_H2_43"));
		//InviteButton.SetButtonTextColor(m_cyanBlueColor);
		InviteButton.SetButtonTextColor(Color.white);
		InviteButton.SetEnabled(true);
		InviteButton.SetBoxCollider(true);

		bool isRestriction = false;
		switch(ectypeData.lEctypeType)
		{
		case 0:
			if(ectypeData.lEctypeContainerID > dataModel.sMsgRecvAnswerFriends_SC.dwEctypeLevel || ectypeData.lMinActorLevel > dataModel.sMsgRecvAnswerFriends_SC.sActorLevel)
			{
				isRestriction = true;
				Swith_Restriction.ChangeSprite((int)TipType.EctypeIsLock);
				UI_Restrication.MakePixelPerfect();
				break;
			}
			if(dataModel.sMsgRecvAnswerFriends_SC.dwInBattle == 1)
			{
				isRestriction = true;
				Swith_Restriction.ChangeSprite((int)TipType.InBattle);
				UI_Restrication.MakePixelPerfect();
				break;
			}
			break;
		case 9:
			if(ectypeData.lMinActorLevel > dataModel.sMsgRecvAnswerFriends_SC.sActorLevel)
			{
				isRestriction = true;
				Swith_Restriction.ChangeSprite((int)TipType.NotEnoughLevel);
				UI_Restrication.MakePixelPerfect();
				break;
			}
			break;
		}

		InviteButton.gameObject.SetActive(!isRestriction);
		Tip_Restriction.SetActive(isRestriction);

	}

	void UpdateHeroIcon(uint vocationID, uint fashionID)
	{        
		var resData = CommonDefineManager.Instance.CommonDefine.HeroIcon_MailFriend.FirstOrDefault(P => P.VocationID == vocationID && P.FashionID == fashionID);
		if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + fashionID); }
		UI_Avatar.spriteName = resData.ResName;
	}
	
	/// <summary>
	/// 暂时无法实现，好友信息里没有好友UID
	/// </summary>
	/// <param name="uid"></param>
	void UpdateRoleIcon(int uid)
	{
		TypeID typeID;
		var playerData = EntityController.Instance.GetEntityModel(uid,out typeID);
		SMsgPropCreateEntity_SC_OtherPlayer otherPlayerData = (SMsgPropCreateEntity_SC_OtherPlayer) playerData.EntityDataStruct;
		int vocationID = otherPlayerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
		int fashionID = otherPlayerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
		var resData = CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_BattleReward.FirstOrDefault(P => P.VocationID == vocationID && P.FashionID == fashionID);
		if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + fashionID); }
		//AvatarIcon.spriteName = resData.ResName;
	}
	
	public void Close()
	{
		transform.gameObject.SetActive(false);
	}
	
	void OnInviteButtonClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamInvitationChoice");
		var teamData = TeamManager.Instance.MyTeamProp;
		var ectypeData = TeamManager.Instance.CurrentEctypeData;
		//TraceUtil.Log("====>发送好友邀请，f id:" + m_friendActorID);
		if(FriendDataManager.Instance.GetOnlineFriendList.Any(p=>p.sMsgRecvAnswerFriends_SC.dwFriendID == m_friendActorID))
		{
			NetServiceManager.Instance.TeamService.SendTeamMemberInviteMsg(new SMsgTeamMemberInvite_SC()
			                                                               {
				dwTeamID = teamData.TeamContext.dwId,
				dwActorID = (uint)teamData.TeamContext.dwCaptainId,
				dwTargetActorID = m_friendActorID,
				//todo：onSelectEctypeData协议已经取消难度dwEctypeID和byDiff难度，如果使用需要从新更改
				//dwEctypeId = ectypeData.dwEctypeID,
				//byEctypDiff = ectypeData.byDiff
			});
		}
		else
		{
			if(FriendDataManager.Instance.GetFriendListData.Any(p=>p.sMsgRecvAnswerFriends_SC.dwFriendID == m_friendActorID))
			{
				UI.MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I13_38"),1f);
			}
			else
			{
				UI.MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I13_46"),1f);
			}

		}


		
		m_tSecond = 3;
		//InviteButton.SetButtonStatus(true);
		//InviteButton.SetButtonText(string.Format(LanguageTextManager.GetString("IDS_H1_90"), m_tSecond));
		//InviteButton.SetButtonTextColor(m_greyColor);
		//InviteButton.SetButtonActive(false);
		InviteButton.SetButtonTextColor(Color.grey);
		InviteButton.SetEnabled(false);
		//InviteButton.SetBoxCollider(false);
		//StartCoroutine("CutDown");
	}
	
	IEnumerator CutDown()
	{
		yield return new WaitForSeconds(1.0f);
		if (m_tSecond > 0)
		{
			m_tSecond -= 1;
			//InviteButton.SetButtonText(string.Format(LanguageTextManager.GetString("IDS_H1_90"), m_tSecond));
			StartCoroutine("CutDown");
		}
		else
		{
			//InviteButton.SetButtonStatus(false);
			//InviteButton.SetButtonText(LanguageTextManager.GetString("IDS_H2_43"));
			//InviteButton.SetButtonTextColor(m_cyanBlueColor);
			//InviteButton.SetButtonActive(true);
			InviteButton.SetButtonTextColor(Color.white);
			InviteButton.SetEnabled(true);
			//InviteButton.SetBoxCollider(true);

		}
	}

}
