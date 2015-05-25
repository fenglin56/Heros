using UnityEngine;
using System.Collections;

public class HeroTitle : View {    
    //private Vector3 m_showPosition = Vector3.zero;    
    //public UILabel NPCProfession;
    
    public UILabel NPCName;
    public Transform VipPoint;
	public Transform TitlePoint;
    private Transform mFollowTarget;
    private Vector3 m_titleHeight = new Vector3(0, 18.5f, 0);
    private bool m_showNpcTitle = false;
    private int m_currentVipLevel;
    private string m_Name;
    private bool m_isMyself;
    private Transform m_followTarget;
	private long m_uid = 0;
	private GameObject m_TitleGameObject;
    void Awake()
    {
        RegisterEventHandler();
    }
    
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
		AddEventHandler(EventTypeEnum.PlayerTitleUpdate.ToString(), UpdateTitleHandle);       
    }
    void ButtonFunc()
    {
        RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(),UpdateViaNotify);
    }
    private void UpdateViaNotify(INotifyArgs e)
    {
     
         EntityDataUpdateNotify notify = (EntityDataUpdateNotify)e;
       if( m_currentVipLevel!= PlayerDataManager.Instance.GetPlayerVIPLevel())
        {

            RefreshTitle();
        }		      
    }
	private void UpdateTitleHandle(INotifyArgs arg)
	{
		m_TitleGameObject = PlayerFactory.Instance.CreateTitle(m_uid,TitlePoint);
	}
    public void RefreshTitle()
    {
     SetTitle();
    }

	public void SetHeroTitle(long uid,string name, bool isMySelf, Transform followTarget)
	{
        if(GameManager.Instance.CurrentState==GameManager.GameState.GAME_STATE_TOWN)
        {
            m_titleHeight=new Vector3(0,CommonDefineManager.Instance.CommonDefine.TownNamePos,0);
        }
        else
        {
            m_titleHeight=new Vector3(0,CommonDefineManager.Instance.CommonDefine.BattleNamePos,0);
        }
		this.m_uid = uid;
        this.m_isMyself = isMySelf;
		m_TitleGameObject = PlayerFactory.Instance.CreateTitle(uid,TitlePoint);

		this.SetHeroTitle(name,isMySelf,followTarget);
	}

    private void SetHeroTitle(string name, bool isMySelf, Transform followTarget)
    {

        //string _name;
        //string _profession;
        mFollowTarget = followTarget;
        this.m_showNpcTitle = true;
        SetText(name, isMySelf);

        SetTitle();
    }
    void SetTitle()
    {
        GameObject VIP;
        GameObject go = null;

        if (m_isMyself)
        {
            m_currentVipLevel = PlayerDataManager.Instance.GetPlayerVIPLevel();
            go = PlayerDataManager.Instance.GetCurrentVipEmblemPrefab();
        } else
        {
			if(PlayerManager.Instance.GetEntityMode(m_uid) != null)
			{
				int level= ((SMsgPropCreateEntity_SC_OtherPlayer)PlayerManager.Instance.GetEntityMode(m_uid).EntityDataStruct).GetCommonValue().PLAYER_FIELD_VISIBLE_VIP;
				m_currentVipLevel=level;
				go = PlayerDataManager.Instance.GetCurrentVipEmblemPrefab(level);
			}           
        }
        if(go!=null)
        {
		    VipPoint.ClearChild();
            VIP= UI.CreatObjectToNGUI.InstantiateObj(go,VipPoint);
            VipPoint.localPosition=new Vector3(40,-28,0);
        }
      

    }

	/// <summary>
	/// 设置Vip等级
	/// </summary>
	/// <param name="level">Level.</param>
	public void SetVipLevel(int level)
	{
		m_currentVipLevel=level;
		GameObject go = PlayerDataManager.Instance.GetCurrentVipEmblemPrefab(level);
		VipPoint.ClearChild();
		if(go != null)
		{	
			VipPoint.ClearChild();
			UI.CreatObjectToNGUI.InstantiateObj(go,VipPoint);
			VipPoint.localPosition=new Vector3(40,-28,0);
		}
	}

    public void SetText(string name, bool isMySelf)
    {
        this.NPCName.SetText(name);

        if (isMySelf)
        {
            this.NPCName.color = ChatPanelUIManager.ColorMy;
        }        
        //this.NPCProfession.SetText(profession);
        //this.m_showPosition = position;
    }

    void LateUpdate()
    {
        if (m_showNpcTitle)
        {
            if (mFollowTarget != null)
            {
                transform.position = GetPopupPos(mFollowTarget.position + m_titleHeight, BattleManager.Instance.UICamera);
            }
            else
            {
                m_showNpcTitle = false;
                Destroy(gameObject);
            }
            
        }
    }

    public Vector3 GetPopupPos(Vector3 sPos, Camera uiCamera)
    {
        var worldPos = Camera.main.WorldToViewportPoint(sPos);
        var uipos = uiCamera.ViewportToWorldPoint(worldPos);

        uipos.z = 2;
        return uipos;
    }
}
