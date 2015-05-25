using UnityEngine;
using System.Collections;
using UI.Friend;
using System.Linq;

public class TeamFriendInfoItem : MonoBehaviour ,IPagerItem
{
    public SpriteSwith AvatarIcon;
    public UISprite AvatarSprite;
    public UILabel NameLabel;
    public UILabel LevelLabel;

    public LocalButtonCallBack InviteButton;

    private uint m_friendActorID = 0;

    private Color m_greyColor = new Color(0.7176f, 0.7176f, 0.7176f);
    private Color m_cyanBlueColor = new Color(0.5176f, 0.7803f, 0.8117f);
    
    //private Transform m_thisTransform;
    private int m_tSecond = 0;

	void Start () 
    {        
        InviteButton.SetCallBackFuntion(OnInviteButtonClick, null);
	}


    public void UpdateInfo(PanelElementDataModel dataModel)
    {
        if (!transform.gameObject.activeInHierarchy)
        {
            transform.gameObject.SetActive(true);
        }
        
        m_friendActorID = dataModel.sMsgRecvAnswerFriends_SC.dwFriendID;

        NameLabel.text = dataModel.sMsgRecvAnswerFriends_SC.Name;
        LevelLabel.text = dataModel.sMsgRecvAnswerFriends_SC.sActorLevel.ToString() + LanguageTextManager.GetString("IDS_H1_156");
        //UpdateRoleIcon(dataModel.sMsgRecvAnswerFriends_SC.bOnLine);
        //AvatarIcon.ChangeSprite((int)dataModel.sMsgRecvAnswerFriends_SC.dProfession);
        UpdateHeroIcon(dataModel.sMsgRecvAnswerFriends_SC.dProfession, dataModel.sMsgRecvAnswerFriends_SC.dbSysActorImageHeadID);

        //InviteButton.SetButtonStatus(false);
        //InviteButton.SetButtonText(LanguageTextManager.GetString("IDS_H2_43"));
        //InviteButton.SetButtonTextColor(m_cyanBlueColor);
        InviteButton.SetButtonTextureColor(Color.white);
        InviteButton.SetButtonActive(true);
    }

    void UpdateHeroIcon(uint vocationID, uint fashionID)
    {        
        var resData = CommonDefineManager.Instance.CommonDefine.HeroIcon_MailFriend.FirstOrDefault(P => P.VocationID == vocationID && P.FashionID == fashionID);
        if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + fashionID); }
        AvatarSprite.spriteName = resData.ResName;
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
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        var teamData = TeamManager.Instance.MyTeamProp;
        var ectypeData = TeamManager.Instance.CurrentEctypeData;
        //TraceUtil.Log("====>发送好友邀请，f id:" + m_friendActorID);
        NetServiceManager.Instance.TeamService.SendTeamMemberInviteMsg(new SMsgTeamMemberInvite_SC()
        {
            dwTeamID = teamData.TeamContext.dwId,
            dwActorID = (uint)teamData.TeamContext.dwCaptainId,
			dwTargetActorID = m_friendActorID,
			//todo：onSelectEctypeData协议已经取消难度dwEctypeID和byDiff难度，如果使用需要从新更改
            //dwEctypeId = ectypeData.dwEctypeID,
            //byEctypDiff = ectypeData.byDiff
        });

        m_tSecond = 3;
        //InviteButton.SetButtonStatus(true);
        //InviteButton.SetButtonText(string.Format(LanguageTextManager.GetString("IDS_H1_90"), m_tSecond));
        //InviteButton.SetButtonTextColor(m_greyColor);
        //InviteButton.SetButtonActive(false);
        InviteButton.SetButtonTextureColor(Color.grey);
        InviteButton.SetButtonActive(false);

        StartCoroutine("CutDown");
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
            InviteButton.SetButtonTextureColor(Color.white);
            InviteButton.SetButtonActive(true);
        }
    }


    public void OnGetFocus()
    {        
    }

    public void OnLoseFocus()
    {
    }

    public void OnBeSelected()
    {
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
