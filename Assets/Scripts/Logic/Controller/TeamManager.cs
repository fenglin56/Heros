using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class TeamManager:ISingletonLifeCycle
{
    private static TeamManager m_instance;
    public static TeamManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new TeamManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }

	private Action m_WaitToFunc = null;

    //当前队伍信息
    private SMsgTeamProp_SC m_myTeamProp = new SMsgTeamProp_SC();
    public SMsgTeamProp_SC MyTeamProp
    {
        get { return m_myTeamProp; }
    }

	//当前选择副本信息
	private EctypeSelectConfigData m_curSelectEctypeAreaData = new EctypeSelectConfigData();
	public EctypeSelectConfigData CurSelectEctypeAreaData
	{
		get{return m_curSelectEctypeAreaData;}
	}

    //当前副本信息
    private SMSGEctypeData_SC m_currentEctypeData = new SMSGEctypeData_SC();
    public SMSGEctypeData_SC CurrentEctypeData
    {
        get { return m_currentEctypeData; }
    }
    //当前副本挑战信息
    private SMSGEctypeLevelData_SC m_currentEctypeLevelData = new SMSGEctypeLevelData_SC();
    public SMSGEctypeLevelData_SC CurrentEctypeLevelData
    {
        get { return m_currentEctypeLevelData; }
    }

    //private List<LocalTeamPropMember> m_MyTeamPropMemberList = new List<LocalTeamPropMember>();
    //public void RegisteMember(SMsgTeamProp_SC sMsgTeamProp)
    //{
    //    sMsgTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.ApplyAllItem(p =>
    //        {
    //            if (m_MemberList.Any(k => k.dwActorID != p.TeamMemberContext.dwActorID))
    //            {
    //                //如果没有相同的队员信息
    //                m_MemberList.Add(new LocalTeamPropMember(p));
    //            }
    //        });
    //}

    //注册队伍信息
    public void RegisteTeam(SMsgTeamProp_SC sMsgTeamProp)
    {
        //if(m_TeamList.Any(p => p.dwTeamID != sMsgTeamProp.TeamContext.dwId))
        //{
        //    //如果没有相同的队伍信息
        //    m_TeamList.Add(new LocalTeamProp(sMsgTeamProp.TeamContext));
        //}
        this.m_myTeamProp = sMsgTeamProp;
        //m_myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.ApplyAllItem(p =>
        //    {
        //        TraceUtil.Log("队员id="+p.TeamMemberContext.dwActorID+ " ,当前血量="+p.TeamMemberContext.nCurHP+ " ,最大血量="+p.TeamMemberContext.nMaxHP);
        //    });
    }

    //注销队伍信息
    public void UnRegisteTeam()
    {
        m_myTeamProp.Clear();
    }

    //移除队员
    public void RemoveMember(uint memberActorId)
    {
        var playerData = PlayerManager.Instance.FindHeroDataModel();
        if (memberActorId == playerData.ActorID)
        {
            this.UnRegisteTeam();
        }
        else
        {
            m_myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers =
                    m_myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.Where(p => p.TeamMemberContext.dwActorID != memberActorId).ToArray();            
        }
    }

    //是否存在队伍
    public bool IsTeamExist()
    {
		return m_myTeamProp.TeamContext.dwCaptainId != 0;
    }
	public void SetWaitExitTeamAction(Action action)
	{
		m_WaitToFunc = action;
	}
	public void DoWaitExitTeamAction()
	{
		if(m_WaitToFunc!=null)
		{
			m_WaitToFunc();
			m_WaitToFunc = null;
		}
	}

	//是否离开先前队伍
	Action m_LeaveTeamFunc = null;
	public void ShowLeaveTeamTip(Action action)
	{
		m_LeaveTeamFunc = action;
		UI.MessageBox.Instance.Show(4,"",LanguageTextManager.GetString("IDS_I13_48"),LanguageTextManager.GetString("IDS_I13_50"),LanguageTextManager.GetString("IDS_I13_49"),
		                            CancelLeave,SureLeave);
    }
	void CancelLeave()
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamDissolutionCancel");
	}
	void SureLeave()
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_TeamDissolutionConfirmation");
		m_LeaveTeamFunc();
	}


	/// <summary>
	/// 当前队伍副本类型
	/// </summary>
	/// <returns>The current ectype type.</returns>
	public int GetCurrentEctypeType()
	{
		int ectypeID = EctypeConfigManager.Instance.EctypeSelectConfigList[m_myTeamProp.TeamContext.dwEctypeId]._vectContainer[m_myTeamProp.TeamContext.dwEctypeIndex - 1];
		EctypeContainerData ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
		return ectypeData.lEctypeType;//副本类型,0=常规副本，1=封魔副本 2=pvp副本 3=封妖副本 4=练功房 5=试炼副本 6=新手副本 7= 8=防守副本 9=首领讨伐 10=无尽试炼
	}

    //public LocalTeamPropMember FindMember(int dwActorID)
    //{
    //    var targetPropMember = m_MemberList.SingleOrDefault(p => p.dwActorID == dwActorID);
    //    if (targetPropMember != null)
    //        return targetPropMember;
    //    return null;
    //}

	/// <summary>
	/// 当前选择区域
	/// </summary>
	/// <param name="data">Data.</param>
	public void SetCurSelectEctypeContainerData(EctypeSelectConfigData data)	
	{
		this.m_curSelectEctypeAreaData = data;
	}

    public void SetEctypeData(SMSGEctypeData_SC ectypeData)
    {
        this.m_currentEctypeData = ectypeData;
    }

    public void SetEctypeLevelData(SMSGEctypeLevelData_SC ectypeLevelData)
    {
        this.m_currentEctypeLevelData = ectypeLevelData;
    }



    public bool UpdateTeamValue(SMsgTeamUpdateProp_SC updateProp )
    {
        if (m_myTeamProp.TeamContext.dwId == updateProp.dwTeamID)
        {
            m_myTeamProp.TeamContext = m_myTeamProp.TeamContext.UpdateValue(updateProp.wProp, updateProp.nValue);
            return true;
        }
        return false;
    }

    public void SetTeamMemberReadyStatu(uint ActorID, int value)
    {
        int length = m_myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.Length;
        for (int i = 0; i < length; i++)
        {
            if (m_myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers[i].TeamMemberContext.dwActorID == ActorID)
            {
                m_myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers[i].TeamMemberContext.SetFightReadyValue(value);                
            }
        }
    }

    public void UpdateTeamMemberValue(SMsgTeamMemberUpdateProp_SC updateProp)
    {
        //TraceUtil.Log("更新: dwActorID = " + updateProp.dwActorID + " ,index = "+updateProp.wProp+" ,value = "+updateProp.nValue);
        if (m_myTeamProp.TeamContext.dwId == updateProp.dwTeamID)
        {
            //m_myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.ApplyAllItem(p =>
            //    {
            //        if (p.TeamMemberContext.dwActorID == updateProp.dwActorID)
            //        {
            //            p.TeamMemberContext.UpdateValue(updateProp.wProp, updateProp.nValue);
            //            UIEventManager.Instance.TriggerUIEvent(UIEventType.ReasetTeammateStatus, updateProp.dwActorID);
            //        }
            //    });
            int length = m_myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.Length;
            for (int i = 0; i < length; i++)
            {
                if (m_myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers[i].TeamMemberContext.dwActorID == updateProp.dwActorID)
                {
                    m_myTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers[i].TeamMemberContext.UpdateValue(updateProp.wProp, updateProp.nValue);
                    UIEventManager.Instance.TriggerUIEvent(UIEventType.ReasetTeammateStatus, updateProp.dwActorID);
                }
            }
        }

    }

    //public class LocalTeamPropMember
    //{
    //    public uint dwActorID;
    //    public SMsgTeamPropMember_SC Value;

    //    public LocalTeamPropMember(SMsgTeamPropMember_SC sMsgTeamPropMember_SC)
    //    {
    //        dwActorID = sMsgTeamPropMember_SC.TeamMemberContext.dwActorID;
    //        this.Value = sMsgTeamPropMember_SC;
    //    }
    //}

    //public class LocalTeamProp
    //{
    //    public uint dwTeamID;
    //    //public SMsgTeamProp_SC Value;
    //    public STeamContext Value;

    //    public LocalTeamProp(STeamContext sTeamContext)
    //    {
    //        dwTeamID = (uint)sTeamContext.dwId;
    //        this.Value = sTeamContext;
    //    }
    //}


    public void Instantiate()
    {

    }

    public void LifeOver()
    {       
        m_instance = null;
    }
}



