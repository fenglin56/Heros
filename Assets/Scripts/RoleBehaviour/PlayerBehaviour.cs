using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Battle;

[RequireComponent(typeof(CharacterController))]
public class PlayerBehaviour : RoleBehaviour, ISendInfoToServer, IEntityDataManager
{
    private SMsgActionMove_SCS m_SMsgActionMove_SC;  //ÉúÎïÒÆ¶¯œá¹¹Ìå

    private List<SkillBase> m_skills;
    private FSMSystem m_FSMSystem;
    private CharacterController m_heroCharactorController;
    private int m_CurrentSkillID;
    private float? m_WalkSpeed;    
    private List<Renderer> m_playerRendererDatas;
    private Vector3 m_skillFirePos;
	private List<Vector3> nodes = new List<Vector3>();
    private float m_normalSkillCDTime = 0;

    private float m_scrollSkillCDTime = 0;

    private bool IsPlayerReady = false;

	public bool ClientMove = false;
    public Transform ClickPointEffect;
    public float HitFlyHeight;
    public int BeHitFlyBreakLV;
    public int BeBeAttackBreakLV;
    public SkinnedMeshRenderer PlayerModel;
    //public int LockRadius=30;   //Ëø¶šÄ¿±ê°ëŸ¶
    //public float LockAngle=120;   //Ëø¶šÄ¿±êœÇ¶È
	
	private SMsgActionMove_SCS m_lastNode;
	private SMsgActionMove_SCS m_currentNode;
	
	private Vector3 ?m_clientEndPos = null;
	public Vector3 ?ClientEndPos
	{
        get { return m_clientEndPos; }
    }
    /// <summary>
    /// 玩家头上Title引用
    /// </summary>
    public GameObject PlayerTitleRef { get; set; }
    public bool IsExplodeState { set; get; }
    /// <summary>
    /// ÊÇ·ñÓ°×ÓÍæŒÒ
    /// </summary>
    public bool IsCopy { get; set; }
    public bool IsDie 
    {
        get
        {
            var playerDataStruct = (IPlayerDataStruct)this.RoleDataModel;
            return ((CRT_STATE)playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UINT_FIELD_STATE) == CRT_STATE.enCrt_State_Die;
        }
    }
	
	//是否无敌
	public bool Invincible = false;
	public bool IronBody = false;
	
	public SMsgActionMove_SCS CurrentNode
	{
		get { return m_currentNode; }	
	}
	private MoveSmooth m_smooth = new MoveSmooth();
	
	public MoveSmooth Smooth
	{
		get {return m_smooth;}	
	}

    private Vector3 m_RunSearchDir;

    void Awake()
    {
        this.CacheTransform();
        
        m_heroCharactorController = ThisTransform.GetComponent<CharacterController>();
        RefreshRenderCach();
        m_skills = GetComponents<SkillBase>().ToList();
		NormalAttackButtonPress = false;        
    }
    // Use this for initialization
    void Start()
    {
        if (!IsCopy)
        {
            this.RegisterEventHandler();
            //Ïò¶šÊ±·¢ËÍ¹ÜÀíÆ÷×¢²áÖ÷œÇ£šÖ÷œÇ¶šÊ±·¢ËÍ×ÔÉí×ø±ê£©          
        }
		IsPlayerReady = true;
        if (IsHero)
        {
            //OnDestroy() to CancelInvoke("LockMonster") 
            InvokeRepeating("LockMonster", 1, CommonDefineManager.Instance.CommonDefine.SearchFrq / 1000f);            
        }
    }
    void LockMonster()
    {
        if (!IsDie)
        {
            MonsterManager.Instance.LockAttackTarget(ThisTransform);
        }
    }
    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyUp(KeyCode.A))
        //{
        //    GetSkillBySkillID(m_CurrentSkillID);
        //    SelectedSkillBase.AddSkillStateChangeDelegate(OnTransation);
        //    SelectedSkillBase.PrePare();
        //}
        //ÆÕÍš¹¥»÷ºÍ·­¹öŒŒÄÜCD
        if(IsHero)
        {
            SoundManager.Instance.transform.position=ThisTransform.position;
        }
        if (m_normalSkillCDTime > 0)
        {
            m_normalSkillCDTime -= Time.deltaTime;
        }
        if (MarchForwardCDTime > 0)
        {
            MarchForwardCDTime -= Time.deltaTime;
        }
        if (m_scrollSkillCDTime > 0)
        {
            m_scrollSkillCDTime -= Time.deltaTime;
        }
        if (ExecuteInitiativeSkillSelectedState)
        {
            this.m_initiativeSkillSelectedState.Reason();
        }
        m_FSMSystem.CurrentState.Reason();
        if (ExecuteInitiativeSkillSelectedState)
        {
            this.m_initiativeSkillSelectedState.Reason();
        }
        m_FSMSystem.CurrentState.Act();
        if (ExecuteInitiativeSkillSelectedState)
        {
            this.m_initiativeSkillSelectedState.Act();
        }
		UpdateAdsorbEx();
		
    }
	
	private float m_BeAdsorbExTime = 0;
	public bool IsBeAbsordEx
	{
		get { return  (m_BeAdsorbExTime > 0 
					  && Vector3.Distance(ThisTransform.position, m_AdsorbExCenterPos) > m_AdsorbExRadius);}	
	}
	
	private float m_AdsorbExSpeed = 0;
	public float AdsorbExSpeed
	{
		get {return m_AdsorbExSpeed;}	
	}
	
	private Vector3 m_AdsorbExCenterPos = Vector3.zero;
	public Vector3 AdsorbExCenterPos
	{
		get { return m_AdsorbExCenterPos; }
	}
	
	private float m_AdsorbExRadius = 0;
	public float AdsorbExRadius
	{
		get { return m_AdsorbExRadius; }	
	}
    public void InvokeEnterTriggerAreaCheck(bool flag)
    {
        if (flag)
        {
            InvokeRepeating("CheckPlayerEnterTriggerArea", 1, 0.5f);
        }
        else
        {
            CancelInvoke("CheckPlayerEnterTriggerArea");
        }
    }
    //检查是否进入首次进入触发区域，如果是，则通知服务器。每0.5秒检测一次
    void CheckPlayerEnterTriggerArea()
    {
        var areaId = SceneDataManager.Instance.GetMapTriggerAreaId(GameManager.Instance.GetCurSceneMapID, ThisTransform.position);
        if (areaId != -1)
        {
            //通知服务器
            NetServiceManager.Instance.EctypeService.SendEnterTriggerArea(areaId);

            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.ReachTriggerArea();
            }
        }
    }
    /// <summary>
    /// 20141011 动态阻挡传送结算 
    /// </summary>
    /// <param name="sMsgFightTeleport_CSC"></param>
    public void BeTeleport(SMsgFightTeleport_CSC sMsgFightTeleport_CSC)
    {
        //	传送前先打断目标所有动作，将其置为站立状态，无视霸体和无敌
        m_FSMSystem.PerformTransition(Transition.PlayerToIdle);
        ThisTransform.position = new Vector3(0.1f * sMsgFightTeleport_CSC.ptPosX, ThisTransform.position.y, -0.1f * sMsgFightTeleport_CSC.ptPosY);
        var euler = new Vector3(0.1f * sMsgFightTeleport_CSC.ptDirectX, 0, -0.1f * sMsgFightTeleport_CSC.ptDirectY);
        ThisTransform.rotation = Quaternion.Euler(euler);
    }
	public void BeAdsorbEx( SMsgFightAdsorptionEx_SC sMsgFightAdsorptionEx_SC)
	{
		m_BeAdsorbExTime = 0.001f * (float)sMsgFightAdsorptionEx_SC.dwTime;
		m_AdsorbExSpeed = 0.1f * sMsgFightAdsorptionEx_SC.dwSpeed;
		m_AdsorbExCenterPos = new Vector3(0.1f *sMsgFightAdsorptionEx_SC.ptCenterPosX, ThisTransform.position.y, -0.1f * sMsgFightAdsorptionEx_SC.ptCenterPosY);
		m_AdsorbExRadius = (float)sMsgFightAdsorptionEx_SC.dwRadius / 10.0f;
	}
	
	void UpdateAdsorbEx()
	{
		if(m_BeAdsorbExTime > 0)
		{
			m_BeAdsorbExTime -= Time.deltaTime;
			if(m_BeAdsorbExTime <=0)
			{
				m_AdsorbExSpeed = 0;
				m_AdsorbExCenterPos = Vector3.zero;
			}
		}
	}

    #region ¹«¹²ÊôÐÔ
    public byte PlayerKind { get; set; }
    private Vector3? m_walkToPosition;
	public Vector3? WalkToPosition
    {
        get { return m_walkToPosition; }
        set { m_walkToPosition = value; }
    }
	
    private Vector3? m_LastWalkToPosition;
	
	public Vector3? LastWalkToPosition
	{
		get { return m_LastWalkToPosition; }	
		set { m_walkToPosition = value; }
	}
	
	
    
    public ResourceType TargetType { get; set; }
    public TargetSelected TargetSelected { get; set; }
    public bool LockState { get; set; }
    public SkillSelectEffectController SkillSelectEffectController { get; set; }
    public SkillBase SelectedSkillBase { get; private set; }
    public CharacterController HeroCharactorController
    {
        get { return m_heroCharactorController; }
    }
    public Vector3 SkillFirePos
    {
        get { return m_skillFirePos; }
        set { m_skillFirePos = value; }
    }
    public float WalkSpeed
    {
        get
        {
            var playerDataStruct = (IPlayerDataStruct)this.RoleDataModel;
            this.m_WalkSpeed = playerDataStruct.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_SPEED / 10;
			
            //TraceUtil.Log("Player speed!!!!!!!!!!!!! "   + "  "  +  this.m_WalkSpeed.Value);
            return this.m_WalkSpeed.Value;
        }
    }
    #endregion

    #region ÎªÊµÏÖÈÌÕßŒŒÄÜ£¬ÐèÒª¿ØÖÆœÇÉ«µÄRender
    public List<Renderer> PlayerRendererDatas
    {
        get { return this.m_playerRendererDatas; }
    }
    /// <summary>
    /// Ë¢ÐÂÍæŒÒ×ÔÉíÓë×°±žµÄRendererÁÐ±í
    /// </summary>
    public void RefreshRenderCach()
    {
        List<PlayDataStruct<MeshRenderer>> partRenderer;
        List<PlayDataStruct<SkinnedMeshRenderer>> mainRenderer;
        List<PlayDataStruct<ParticleSystemRenderer>> particleRender;
        transform.RecursiveGetComponent<MeshRenderer>("MeshRenderer", out partRenderer);
        transform.RecursiveGetComponent<SkinnedMeshRenderer>("SkinnedMeshRenderer", out mainRenderer);
        transform.RecursiveGetComponent<ParticleSystemRenderer>("ParticleSystemRenderer",out particleRender);
        PlayerModel = transform.GetComponentInChildren<SkinnedMeshRenderer>();

        if (this.m_playerRendererDatas == null)
        {
            this.m_playerRendererDatas = new List<Renderer>();
        }
        this.m_playerRendererDatas.Clear();

        this.m_playerRendererDatas.AddRange(partRenderer.Select(P => (Renderer)P.AnimComponent));
        this.m_playerRendererDatas.AddRange(mainRenderer.Select(P =>(Renderer)P.AnimComponent));
        this.m_playerRendererDatas.AddRange(particleRender.Select(P =>(Renderer)P.AnimComponent));
    }
    /// <summary>
    /// žùŸÝŒŒÄÜ¶¯×÷ÀàÐÍ£¬ÒþÉí»òÏÔÉí
    /// </summary>
    /// <param name="moveType"></param>
    public void ChangeDisplayState(int moveType)
    {
		RefreshRenderCach();
        if (moveType == 0)
        {
            m_playerRendererDatas.ApplyAllItem(P => { if (P.enabled) P.enabled = false; });
        }
        else
        {
            m_playerRendererDatas.ApplyAllItem(P => 
            { 
                //TraceUtil.Log(P.name + ": "+ P.enabled);
                if (!P.enabled) P.enabled = true; 
            });
        }
    }
    #endregion

       
    #region 移动细分
    public void MoveTo(Vector3 motion)
    {
        Vector3 newPos = ThisTransform.position;
        if (motion.magnitude > 5)
        {
            int smoothCount = Mathf.CeilToInt(Time.deltaTime / 0.033f);
            Vector3 motionInTime = motion / smoothCount;
            for (int i = 0; i < smoothCount; i++)
            {
                if (SceneDataManager.Instance.IsPositionInBlock(newPos + motionInTime))
                {
                    TraceUtil.Log("[motionInTime]" + motionInTime);
                    //ThisTransform.position = newPos;
                    m_heroCharactorController.Move(newPos - ThisTransform.position);
                    return;
                }
                newPos += motionInTime;
            }
        }
        else
        {     
            newPos += motion;
            if (SceneDataManager.Instance.IsPositionInBlock(newPos))
            {
                return;
            }
        }
        m_heroCharactorController.Move(newPos - ThisTransform.position);

        //ThisTransform.position = newPos;
    }
    #endregion

    #region ŒŒÄÜžœŒÓ£¬ÒÆ³ý£¬Žò¶Ï£¬²éÕÒµÈŽŠÀí
    /// <summary>
    /// Žò¶ÏÖž¶šŒŒÄÜ
    /// </summary>
    /// <param name="skillID">±»Žò¶ÏµÄŒŒÄÜID</param>
    public void BreakCurrentSkill(int skillID)
    {
        if (this.m_CurrentSkillID == skillID)
        {
            (this.m_FSMSystem.CurrentState as PlayerState).IsSkillBeBreaked = false;
            BulletManager.Instance.TryDestroyBreakBullets(this.RoleDataModel.SMsg_Header.uidEntity);
        }
    }
    public void AddSkillBase(int skillID, bool isDefaultSkill)
    {
        if (!this.m_skills.Any(P => P.SkillId == skillID))
        {
            var newSkill = RoleGenerate.AttachSkill(gameObject, skillID);
            newSkill.AddSkillBulletFireDelegate(FireSkillBullet);
            newSkill.AddSkillEffectFireDelegate(FireSkillActionEffect);
            newSkill.AddSkillUIEffectFireDelegate(PlayUIEffect);
            newSkill.SetUserID = this.RoleDataModel.SMsg_Header.uidEntity;
            this.m_skills.Add(newSkill);
            if (isDefaultSkill)
            {
                m_CurrentSkillID = skillID;
            }
        }
    }
    public void AddSkillBase(int skillID)
    {
        this.AddSkillBase(skillID, false);
    }
    public List<SkillBase> GetPlayerSkills()
    {
        return this.m_skills;
    }
    public void RemoveSkillBase()
    {
        this.m_skills.ApplyAllItem(skill =>
        {
            if (skill.SkillId != m_CurrentSkillID)
            {
                skill.RemoveSkillBulletFireDelegate(FireSkillBullet);
                skill.RemoveSkillEffectFireDelegate(FireSkillActionEffect);
                skill.RemoveSkillUIEffectFireDelegate(PlayUIEffect);
                skill.Stop();
                Component.Destroy(skill);
            }
        });
        this.m_skills.RemoveAll(P => P.SkillId != m_CurrentSkillID);              
    }
    public void GetSkillByAniName(string aniString)
    {
        this.SelectedSkillBase = m_skills.SingleOrDefault(P => P.AniStr == aniString);
    }
    public void GetSkillBySkillID(int? skillID)
    {
        if (skillID.HasValue)
        {
            m_CurrentSkillID = skillID.Value;
            this.SelectedSkillBase = m_skills.SingleOrDefault(P => P.SkillId == skillID);
        }
        else
        {
            this.SelectedSkillBase = null;
        }
    }
    public void BreakSkill()
    {
        switch (this.FSMSystem.CurrentStateID)
        {
            case global::StateID.PlayerNormalSkill:
            case global::StateID.PlayerInitialtiveSkillSelect:
            case global::StateID.PlayerInitiativeSkill:
                var playerState = this.FSMSystem.CurrentState as PlayerState;
                if (playerState != null)
                {
                    if (this.SelectedSkillBase != null)
                    {
                        ///ÉèÖÃ±»Žò¶ÏŒŒÄÜÍŒ±êµÄ×ŽÌ¬
                        //BattleSkillButtonManager.Instance.SetButtonStatus(playerBehaviour.SelectedSkillBase.SkillId, SkillButtonStatus.Enable);
                        NetServiceManager.Instance.BattleService.SendBreakSkillCommand(new SMsgFightBreakSkill_CS() { SkillID = this.SelectedSkillBase.SkillId });
                        //playerState.IsSkillBeBreaked = true;                       
                        //TraceUtil.Log(playerState.StateID+ "  Žò¶Ï£¬µ÷ÓÃ×Óµ¯Ïú»Ù-FSM");
                        BulletManager.Instance.TryDestroyBreakBullets(this.RoleDataModel.SMsg_Header.uidEntity);
                    }
                }
                break;
            default:
                break;

        }

    }
    #endregion

    #region ŒŒÄÜÊ©·Å£¬¶¯×÷£¬×Óµ¯µÈŽŠÀí¡£°üÀšŒŒÄÜÑ¡Ôñ£¬ÍæŒÒ×ªÏò
    public bool SkillCanBeFire(UI.Battle.BattleSkillButton ButtonInstance)
    {
        if (IsDie || BattleManager.Instance.BlockPlayerToIdle)
        {
            return false;
        }


        bool fireSkill = false;
        //10.24 ÐÂÔöÐÞžÄ¡£ÔÊÐíÔÚ±»»÷ÍË£¬»÷·ÉÇé¿öÏÂÊ¹ÓÃŒŒÄÜ
        fireSkill = (m_FSMSystem.CurrentStateID == StateID.PlayerBeAttacked
            && BeBeAttackBreakLV <= ButtonInstance.skillConfigData.m_breakLevel)
            || (m_FSMSystem.CurrentStateID == StateID.PlayerBeHitFly
            && BeHitFlyBreakLV <= ButtonInstance.skillConfigData.m_breakLevel);
        //TraceUtil.Log("Ê©·ÅŒŒÄÜ£º" + m_FSMSystem.CurrentStateID + " IsfireSkill:" + fireSkill);
        if (!fireSkill)
        {
            if (BreakSkillCheck(ButtonInstance.skillConfigData))
            {
                if (m_FSMSystem.CurrentStateID == StateID.PlayerNormalSkill
                    || m_FSMSystem.CurrentStateID == StateID.PlayerInitiativeSkill
                    || m_FSMSystem.CurrentStateID == StateID.PlayerIdle
                    || m_FSMSystem.CurrentStateID == StateID.PlayerRun)
                {
                    fireSkill = true;
                }
            }
        }

        return fireSkill;
    }
    /// <summary>
    /// ŒŒÄÜÀžµã»÷»Øµ÷ŽŠÀí
    /// </summary>
    /// <param name="ButtonInstance"></param>
    public bool OnButtonCallBack(UI.Battle.BattleSkillButton ButtonInstance)
    {
		
        //按钮记忆处理
        //var clickBtn = ButtonInstance.GetComponent<SkillBtnRemember>();
        //if (clickBtn != null)
        //{
        //    clickBtn.OnSkillBtnClicked(fireSkill);
        //}
        var fireSkill=SkillCanBeFire(ButtonInstance);
        if (fireSkill)
        {
            switch (ButtonInstance.skillButtonStatus)
            {
                case SkillButtonStatus.Enable:
                    //²»žÄ±äµ±Ç°ŒŒÄÜ£¬»ºŽæÑ¡ÔñµÄŒŒÄÜ¡£ÔÚÖ÷¶¯ŒŒÄÜÊ©·ÅÊ±ÓÃGetSkillBySkillID·œ·šžÄ±äÒ»ÏÂÍæŒÒµ±Ç°ŒŒÄÜ
                    //GetSkillBySkillID(ButtonInstance.skillConfigData.m_skillId);
					UIEventManager.Instance.TriggerUIEvent(UIEventType.AddAPMNumber,null);
					UIEventManager.Instance.TriggerUIEvent(UIEventType.AddBattleButtonClickNumber,ButtonInstance);
                    this.m_nextSkillID = ButtonInstance.skillConfigData.m_skillId;
                    var prepareSkill = m_skills.SingleOrDefault(P => P.SkillId == this.m_nextSkillID);
                   
                    if (prepareSkill != null)
                    {
                        prepareSkill.AddSkillStateChangeDelegate(OnTransation);
                        prepareSkill.PrePare();
						
						if(!GameManager.Instance.m_gameSettings.DoubleClickSkill)
						{
							switch(ButtonInstance.skillConfigData.m_directionParam)
                        	{
                            case 1:        //·œÏòÐÍ
                                var position1 = transform.TransformPoint(Vector3.forward * ButtonInstance.skillConfigData.m_launchRange[0]);  //°ÑœÇÉ«ÕýÇ°·œŸàÀëŒŒÄÜÊÍ·Å·¶Î§Î»ÖÃ×÷ÎªÊ©·Åµã
                                m_initiativeSkillSelectedState.ChangeState(position1, TouchPhase.Began);
                                m_initiativeSkillSelectedState.ChangeState(position1, TouchPhase.Ended);
                                break;
                            case 2:        //·¶Î§
                                var position2 = transform.TransformPoint(Vector3.forward * ButtonInstance.skillConfigData.m_launchRange[0]*0.5f);  //°ÑœÇÉ«ÕýÇ°·œŸàÀëŒŒÄÜÊÍ·Å·¶Î§Î»ÖÃ×÷ÎªÊ©·Åµã
                                m_initiativeSkillSelectedState.ChangeState(position2, TouchPhase.Began);
                                m_initiativeSkillSelectedState.ChangeState(position2, TouchPhase.Ended);
                                break;
                       	 	}
						
						}
                    }
                    break;
                case SkillButtonStatus.Wait:
                    //Èç¹ûœÇÉ«ÕýÔÚŒŒÄÜÑ¡ÔñÖÐ¡£ÔòÅÐ¶ÏŒŒÄÜÀàÐÍ£¬ÈçÊÇ·œÏòÐÍ£¬Ôò³¯œÇÉ«ÕýÇ°·œÊ©·Å¡£
                    //Èç¹ûÊÇ·¶Î§ÐÍ£¬Ôò......
                    if (GameManager.Instance.m_gameSettings.DoubleClickSkill && ExecuteInitiativeSkillSelectedState == true)
					{
						UIEventManager.Instance.TriggerUIEvent(UIEventType.AddAPMNumber,null);
						UIEventManager.Instance.TriggerUIEvent(UIEventType.AddBattleButtonClickNumber,ButtonInstance);
                        switch(ButtonInstance.skillConfigData.m_directionParam)
                        {
                            case 1:        //·œÏòÐÍ
                                var position1 = transform.TransformPoint(Vector3.forward * ButtonInstance.skillConfigData.m_launchRange[0]);  //°ÑœÇÉ«ÕýÇ°·œŸàÀëŒŒÄÜÊÍ·Å·¶Î§Î»ÖÃ×÷ÎªÊ©·Åµã
                                m_initiativeSkillSelectedState.ChangeState(position1, TouchPhase.Began);
                                m_initiativeSkillSelectedState.ChangeState(position1, TouchPhase.Ended);
                                break;
                            case 2:        //·¶Î§
                                var position2 = transform.TransformPoint(Vector3.forward * ButtonInstance.skillConfigData.m_launchRange[0]*0.5f);  //°ÑœÇÉ«ÕýÇ°·œŸàÀëŒŒÄÜÊÍ·Å·¶Î§Î»ÖÃ×÷ÎªÊ©·Åµã
                                m_initiativeSkillSelectedState.ChangeState(position2, TouchPhase.Began);
                                m_initiativeSkillSelectedState.ChangeState(position2, TouchPhase.Ended);
                                break;
                        }
                    }
                    break;
            }
        }

        return fireSkill;
	}
    private bool BreakSkillCheck(SkillConfigData skillConfigData)
    {
        //bool invokeNextSkill = true;
        if (SelectedSkillBase != null)
        {
            if (SelectedSkillBase.SkillId != skillConfigData.m_skillId)
            {
                if (SelectedSkillBase.OnFire) //µ±Ç°ŒŒÄÜÊ©·ÅÖÐ
                {
                    if (IsExplodeState)
                    {
                        if (skillConfigData.m_breakLevel < SelectedSkillBase.CurrentActionThresHold)//SelectedSkillBase.SkillData.m_breakLevel)
                        {
                            //NetServiceManager.Instance.BattleService.SendBreakSkillCommand(new SMsgFightBreakSkill_CS() { SkillID = this.SelectedSkillBase.SkillId });
                            return false;
                        }
                    }
                    else
                    {
                        if (skillConfigData.m_breakLevel <= SelectedSkillBase.CurrentActionThresHold)   //Á¬ÕÐÐÞžÄ·œ°žÒªÇóµÈŒ¶ÏàÍ¬µÄŒŒÄÜÒ²¿ÉŽò¶Ï,¡Ÿ2013-11-22 ÓÖžÄ³ÉÏàÍ¬µÈŒ¶²»¿ÉŽò¶Ï¡¿
                        {
                            //NetServiceManager.Instance.BattleService.SendBreakSkillCommand(new SMsgFightBreakSkill_CS() { SkillID = this.SelectedSkillBase.SkillId });
                            return false;
                        }
                    }
                }
            }
        }
		
        return true;  
    }
    public void ChangeForward(Vector3 lookToDir)
    {
        ThisTransform.LookAt(new Vector3(lookToDir.x, ThisTransform.position.y, lookToDir.z));
    }
    private void FireSkillBullet(int bulletID, bool useFirePos)
    {
        //TraceUtil.Log("ÍæŒÒuseFirePos:" + useFirePos);  //ÐèÒªÖªµÀÊ©·Åµã--Ô²ÐÎŒŒÄÜÊ±ÓÃ
        if (SelectedSkillBase == null || GameManager.Instance.CurrentState != GameManager.GameState.GAME_STATE_BATTLE)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"SelectedSkillBase == null");
            return;
        }
		if(useFirePos)
		{
            BulletFactory.Instance.CreateBullet(bulletID, this.RoleDataModel.SMsg_Header.uidEntity, ThisTransform, m_skillFirePos, SelectedSkillBase.SkillData.m_skillId, -1);
		}
		else
		{
            BulletFactory.Instance.CreateBullet(bulletID, this.RoleDataModel.SMsg_Header.uidEntity, ThisTransform, SelectedSkillBase.SkillData.m_skillId, -1);
		}
    }
    private void FireSkillActionEffect(int actionId, int skillID)
    {
        GameManager.Instance.ActionEffectFactory.CreateActionEffect(actionId, skillID, this.RoleDataModel.SMsg_Header.uidEntity, ThisTransform);
    }

    private void PlayUIEffect(GameObject effectPrefab, Vector3 EffectPos, float duration)
    {
        GameObject effect = (GameObject)GameObject.Instantiate(effectPrefab);
        effect.transform.parent = BattleManager.Instance.UICamera.transform;
        effect.transform.localScale = Vector3.one;
        effect.transform.localPosition = EffectPos;
        var destroySelf = effect.AddComponent<DestroySelf>();
        destroySelf.Time = duration;
    }

    public void FightChangeDirect(float directX,float directY)
    {
        if(this.m_FSMSystem.CurrentStateID==StateID.PlayerInitiativeSkill)
        {
            Vector3 newDirect = new Vector3(directX, ThisTransform.position.y, -directY);
            ((PlayerInitiativeSkillFireState)this.m_FSMSystem.CurrentState).ChangeAttackDirect(newDirect);
        }
    }
    #endregion

    #region ×ŽÌ¬»ú¿ØÖÆ
    public FSMSystem FSMSystem
    {
        get { return m_FSMSystem; }
    }
    public void InitFSM()
    {
        CachEntityAnimation();
        m_FSMSystem = new FSMSystem(this, AllowStateChange);
        m_FSMSystem.OnPerformTransition += this.OnPlayerStateChangedHandle;
        PlayerIdleState idleState = new PlayerIdleState();
        m_FSMSystem.AddState(idleState);
        PlayerRunState runState = new PlayerRunState();
        m_FSMSystem.AddState(runState);
        PlayerBeAttackedState beAttackedState = new PlayerBeAttackedState();
        m_FSMSystem.AddState(beAttackedState);
        PlayerNormalSkillFireState normalSkillFireState = new PlayerNormalSkillFireState();
        m_FSMSystem.AddState(normalSkillFireState);
        PlayerInitiativeSkillFireState initiativeSkillFireState = new PlayerInitiativeSkillFireState();
        m_FSMSystem.AddState(initiativeSkillFireState);
        InitiativeSkillSelectedState initiativeSkillSelectState = new InitiativeSkillSelectedState();
        m_FSMSystem.AddState(initiativeSkillSelectState);
        m_initiativeSkillSelectedState = initiativeSkillSelectState;
        PlayerFindPathState findPathState = new PlayerFindPathState();
        m_FSMSystem.AddState(findPathState);

        PlayerDieState playerDieState = new PlayerDieState();
        m_FSMSystem.AddState(playerDieState);

        m_FSMSystem.AddState(new PlayerBeAdsorbState());

        m_FSMSystem.AddState(new PlayerScrollSkillFireState());
        m_FSMSystem.AddState(new PlayerBeHitFlyState());
        m_FSMSystem.AddState(new PlayerStandState());
        m_FSMSystem.AddState(new PlayerCastAbilityState());

        m_FSMSystem.PerformTransition(Transition.PlayerToIdle);
    }
    private void OnPlayerStateChangedHandle(Transition trans)
    {
        switch(trans)
        {
            case Transition.PlayerFireNormalSkill:
                m_LastWalkToPosition = null;
                m_normalSkillCDTime = SkillDataManager.Instance.GetSkillConfigData(PlayerDataManager.Instance.GetBattleItemData(this.PlayerKind).NormalSkillID[2]).m_coolDown;  //index 0,1 for march froward added for 20101408 skill modify
                break;
            case Transition.PlayerFireScrollSkill:
                m_LastWalkToPosition = null;
                m_normalSkillCDTime = SkillDataManager.Instance.GetSkillConfigData(PlayerDataManager.Instance.GetBattleItemData(this.PlayerKind).ScrollSkillID).m_coolDown;
                break;
            case Transition.PlayerToIdle:
                if (m_LastWalkToPosition.HasValue)
                {
                    //TraceUtil.Log("1:"+m_LastWalkToPosition);
                    //TraceUtil.Log("2:" + WalkToPosition);
                    WalkToPosition = m_LastWalkToPosition;
                    m_LastWalkToPosition = null;
                }
                break;
            default:
                m_LastWalkToPosition = null;
                break;
        }
    }
    public void OnTransation(Transition trans, SkillBase skillBase)
    {
        skillBase.RemoveSkillStateChangeDelegate(OnTransation);
        if (trans == Transition.PlayerInitialtiveSkillSelect)
        {
            //žùŸÝ²ß»®Á¬ÕÐÐÞžÄ·œ°ž£¬ŒŒÄÜÑ¡Ôñ×ŽÌ¬²»ÔÙ×÷ÎªÒ»žö¶ÀÁ¢×ŽÌ¬¡£
            m_initiativeSkillSelectedState.DoBeforeEntering();
        }
        else
        {            
            m_FSMSystem.PerformTransition(trans);
        }
    }
    private bool AllowStateChange(Transition trans)
    {       
        bool flag = true;
        if (trans == Transition.PlayerToDie)
        {
            flag = true;
        }
        else
        {
            if (IsDie)
            {
                flag = false;
            }
            else
            {
                switch (trans)
                {
                    case Transition.PlayerFireNormalSkill:
                        flag = m_normalSkillCDTime <= 0;                        
                        break;
                    case Transition.PlayerFireScrollSkill:
                        flag = m_normalSkillCDTime <= 0;
                        // var scrollBtn = BattleUIManager.Instance.FindRememberBtn(SkillBtnRemember.RememberBtnType.ScrollBtn);
                        // if (scrollBtn != null)
                        //{
                        //    scrollBtn.OnSkillBtnClicked(flag);
                        //}
                        break;
                }
            }
        }
       
        return flag;

    }

    /// <summary>
    /// 清除路点信息
    /// </summary>
    public void ClearWalkPositionInfo()
    {
        m_walkToPosition = null;
        m_LastWalkToPosition = null;        
    }

	
	private bool m_isJoyStickPress;
	public bool IsJoyStickPress
	{
		get { return m_isJoyStickPress; } 
	}
	
	
	private Vector3 m_JoyStickDir;
	public Vector3 JoyStickDir
	{
		get { return m_JoyStickDir; }	
		
	}
	
	public void PadPress(bool isPress, Vector3 dir)
	{
		m_isJoyStickPress = isPress;
		m_JoyStickDir = dir;
		if(isPress)
		{
			
			
		}
		else
		{
			
			
		}
	
	

    }
	
	private bool m_normalAttackButtonPress;
	
	public bool NormalAttackButtonPress
	{
		get { return m_normalAttackButtonPress; }	
		set { m_normalAttackButtonPress = value; }
	}
    /// <summary>
    /// Normal Skill[0] march forward CD
    /// </summary>
    public float MarchForwardCDTime{
        get;set;
    }
	
    #endregion

    #region ÍæŒÒËÀÍö£¬žŽ»î£¬¶³œá£¬ÒÆ¶¯,œÓÊÜË«»÷ŽŠÀí
    public void Die()
    {
        ExecuteInitiativeSkillSelectedState = false;  //ËÀÍöºó²»ÄÜÔÙŽÎœøÈëŒŒÄÜÑ¡Ôñ×ŽÌ¬
        if (IsHero )
        {
			if(SelectedSkillBase != null)
			{
            	BattleSkillButtonManager.Instance.SetButtonStatus(SelectedSkillBase.SkillId, SkillButtonStatus.Enable);
			}
			if(NextSkillBase != null)
			{
				BattleSkillButtonManager.Instance.SetButtonStatus(NextSkillBase.SkillId, SkillButtonStatus.Enable);
			}
        }

        if (null != this.SkillSelectEffectController)
        {
            this.SkillSelectEffectController.HideAll();
        }
        StartCoroutine(GotoHell());
    }
    private IEnumerator GotoHell()
    {
        while (!IsPlayerReady)
        {
            yield return null;
        }
		yield return null;
		yield return null;
        m_FSMSystem.PerformTransition(Transition.PlayerToDie);
		//animation.Play("Dead");
    }
    public void Relive()
    {        	
		//GameObject levelUpEffect = GameObjectPool.Instance.AcquireLocal(PlayerDataManager.Instance.LevelUpEffectPrefab, Vector3.zero, Quaternion.identity);
		GameObject levelUpEffect = Instantiate(PlayerDataManager.Instance.LevelUpEffectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		levelUpEffect.transform.parent = this.ThisTransform;
		levelUpEffect.transform.localPosition = Vector3.zero;
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Upgrade");

		m_FSMSystem.PerformTransition(Transition.PlayerToIdle);
        this.GetComponent<PlayerHurtFlash>().OnRelive(CommonDefineManager.Instance.CommonDefine.AddBuff_BornTime/1000.0f);
    }

    public void PlayerToWalk()
    {
        m_FSMSystem.PerformTransition(Transition.PlayerToTarget);
    }

    public void Freeze(bool flag)
    {
        this.HeroCharactorController.enabled = !flag;
    }
    /// <summary>
    /// ÊÕµœœÇÉ«ÒÆ¶¯Ð­Òé°ü
    /// </summary>
    /// <param name="notifyArgs"></param>
    public void EntityMove(SMsgActionMove_SCS sMsgActionMove_SC)
    {
        if (this.RoleDataModel.SMsg_Header.uidEntity == sMsgActionMove_SC.uidEntity)
        {
            //正常情况下这个实体移动消息不会发给主角，如果是主角说明服务器需要进行位置同步
            if (this.IsHero)
            {
                transform.position = new Vector3(0, 0.1f, 0);
                transform.position = transform.position.GetFromServer(sMsgActionMove_SC.floatX, sMsgActionMove_SC.floatY);

                return;
            }
			if(this.FSMSystem.CurrentStateID != StateID.PlayerIdle
				&& this.FSMSystem.CurrentStateID != StateID.PlayerRun)
			{
				return;	
			}

			if(this.IsDie)
			{
				return;
			}
			
			if(sMsgActionMove_SC.fSpeed > 0.1f)
			{
				if(m_FSMSystem.CurrentStateID == StateID.PlayerIdle)	
				{
					m_smooth.Init(ThisTransform.position,
							  ThisTransform.TransformDirection(Vector3.forward) * sMsgActionMove_SC.fSpeed / 10.0f, 
							  new Vector3(sMsgActionMove_SC.floatX/10.0f, 0, -sMsgActionMove_SC.floatY/10.0f),
                              new Vector3(sMsgActionMove_SC.fDirectX, 0, -sMsgActionMove_SC.fDirectY) * sMsgActionMove_SC.fSpeed / 10.0f, 
							  ConfigDefineManager.TIME_MOVE_SYNC_FOWARD);
					for(int i = 0; i<=10; i++)
					{
						Vector3 nodepos = m_smooth.GetCurrentPos(0.1f*i);
						nodes.Add(new Vector3(nodepos.x, 1, nodepos.z));
					}
										
					m_FSMSystem.PerformTransition(Transition.PlayerToTarget);	
					PlayerRunState state = (PlayerRunState)m_FSMSystem.CurrentState;
					state.MoveMode = GetMoveModeBySpeed(sMsgActionMove_SC.fSpeed / 10.0f);
					state.ResetSmooth(ConfigDefineManager.TIME_MOVE_SYNC_FOWARD);
					
				}
				else if(m_FSMSystem.CurrentStateID == StateID.PlayerRun)
				{
					PlayerRunState state = (PlayerRunState)m_FSMSystem.CurrentState;
					state.MoveMode = GetMoveModeBySpeed(sMsgActionMove_SC.fSpeed / 10.0f);
					state.ResetSmooth(ConfigDefineManager.TIME_MOVE_SYNC_FOWARD);
					m_smooth.Init(ThisTransform.position,
							  ThisTransform.TransformDirection(Vector3.forward) * sMsgActionMove_SC.fSpeed / 10.0f,
                              new Vector3(sMsgActionMove_SC.floatX / 10.0f, 0, -sMsgActionMove_SC.floatY / 10.0f),
                              new Vector3(sMsgActionMove_SC.fDirectX, 0, -sMsgActionMove_SC.fDirectY) * sMsgActionMove_SC.fSpeed / 10.0f, 
							  ConfigDefineManager.TIME_MOVE_SYNC_FOWARD);
					for(int i = 0; i<=10; i++)
					{
						Vector3 nodepos = m_smooth.GetCurrentPos(0.1f*i);
						nodes.Add(new Vector3(nodepos.x, 1, nodepos.z));
					}
				}
				m_clientEndPos = null;
				ClientMove = true;
			}
			else
			{
                m_clientEndPos = new Vector3(sMsgActionMove_SC.floatX / 10.0f, 0, sMsgActionMove_SC.floatY / -10.0f);
				ClientMove = false;
			}
			
        }
    }
	
	private PlayerRunState.PlayerMoveMode GetMoveModeBySpeed(float speed)
	{
		if(speed > (WalkSpeed -0.001f ))
		{
			return  PlayerRunState.PlayerMoveMode.RUN;
		}
		else
		{
			return PlayerRunState.PlayerMoveMode.WALK;	
		}
		
	}
	/*void TestFun()
	{
		if (npcTest == null) {
			npcTest = GameObject.Find("JH_Mod_NPC104(Clone)").transform;		
		}
		Vector3 pos1 = new Vector3 (3,0,0);
		Vector3 pos2 = new Vector3 (0,4,0);
		pos1 = pos1 - Vector3.Project (pos1,Vector3.up);
		pos2 = pos2 - Vector3.Project (pos2,Vector3.up);
		float angle1 = Vector3.Angle (pos1,pos2);
		float angle2 = Vector3.Angle (pos2,pos1);
		
		Vector3 pp1 = npcTest.position - Vector3.Project (npcTest.position,Vector3.up);
		Vector3 pp2 = transform.position - Vector3.Project (transform.position,Vector3.up);
		
		
		Debug.Log ("angle1="+angle1+"angle2="+angle2+"----------"+Vector3.Angle(pp1,pp2)+"--=="+(Vector3.Angle(transform.forward,(npcTest.position-transform.position))));
		
		
		
		Vector3 selfPos = npcTest.position;
		Vector3 poss = transform.InverseTransformPoint(selfPos);
		bool inAngle = false;
		float newAng = Vector3.Angle (Vector3.forward, poss);
		Debug.Log ("newAng="+newAng);
	}*/
    /// <summary>
    /// ÊÕµœµã»÷ÖžÁî£¬»ò·ÅŒŒÄÜ£¬»òÐÐ×ß
    /// </summary>
    /// <param name="touchInvoke"></param>
	//private Transform npcTest ;
    public void GetTouchDown(TouchInvoke touchInvoke)
    {
        if (this.IsDie || BattleManager.Instance.BlockPlayerToIdle)
        {
            return;
        }
		/*if (IsInvoking ("TestFun"))
			CancelInvoke ("TestFun");
		InvokeRepeating ("TestFun",2,2);*/
        if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_PLAYERROOM)
        { return; }
        if (touchInvoke.TouchGO.tag.ToLower() == "terrain")
        {
			if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
			{
				if(TaskModel.Instance.isNpcTalking)
					return;
			}
            this.m_LastWalkToPosition = touchInvoke.TouchPoint; //°Ñ×îºóÒ»žöµã»÷µãŒÇÂŒÏÂÀŽ
			UIEventManager.Instance.TriggerUIEvent(UIEventType.AddAPMNumber,null);
            StateID sId = m_FSMSystem.CurrentState.StateID;
            if (sId == StateID.PlayerInitialtiveSkillSelect
                || sId == StateID.PlayerInitiativeSkill)
            {
                if (sId == StateID.PlayerInitialtiveSkillSelect)
                {
                    InitiativeSkillSelectedState state = (InitiativeSkillSelectedState)(m_FSMSystem.CurrentState);
                    state.OnTouch(touchInvoke.TouchPoint);
                }
                else
                {
                    PlayerInitiativeSkillFireState state = (PlayerInitiativeSkillFireState)(m_FSMSystem.CurrentState);
                    state.OnTouch(touchInvoke.TouchPoint);
                }
            }           
            else if (touchInvoke.TouchCount == 1)
            {
                WalkToPosition = touchInvoke.TouchPoint;
                TargetType = ResourceType.Terrain;
				if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE
					&& m_FSMSystem.CurrentStateID == StateID.PlayerRun)
				{
					m_LastWalkToPosition = null;
					((PlayerRunState)m_FSMSystem.CurrentState).StartToRun(2.0f, 2.0f);
				}

                if (m_FSMSystem.CurrentStateID == StateID.PlayerFindPathing)
                {
                    transform.GetComponent<NavMeshAgent>().enabled = false;
                    m_FSMSystem.PerformTransition(Transition.PlayerToIdle);
                }
				
				/*
                if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE
                   && m_FSMSystem.CurrentStateID == StateID.PlayerBeAdsorb)
                {
                    ((PlayerBeAdsorbState)m_FSMSystem.CurrentState).Run(WalkToPosition.Value);
                }
                */
            }
            else if (touchInvoke.TouchCount == 2
                && GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE)
            {
				/*
                var skillId = PlayerDataManager.Instance.GetBattleItemData(this.PlayerKind).ScrollSkillID;
                var skillConfig = SkillDataManager.Instance.GetSkillConfigData(skillId);
                if (!BreakSkillCheck(skillConfig))
                {
                    return;
                }
                //Add by limanru
                if (PlayerGasSlotManager.Instance.IsCanRoll())
                {
                    PlayerGasSlotManager.Instance.ConsumeOneRollAirSlot(1);
                }
                else
                {
                    return;
                }
                ChangeForward(touchInvoke.TouchPoint);
                m_FSMSystem.PerformTransition(Transition.PlayerFireScrollSkill);
                */
            }
        }
    }
    #endregion

    #region IEntityDataManager ÊµÏÖ
    public IEntityDataStruct GetDataModel()
    {
        return this.RoleDataModel;
    }
    #endregion

    #region ISendInfoToServer ÊµÏÖ
    /// <summary>
    /// °ÑÖ÷œÇµÄ×ø±êÉú³ÉÊýŸÝ°ü
    /// </summary>
    /// <returns></returns>
    public Package GetSendInfoPkg()
    {          
        ThisTransform.position.SetToServer(out this.m_SMsgActionMove_SC.floatX, out this.m_SMsgActionMove_SC.floatY);

        var direct = ThisTransform.TransformDirection(Vector3.forward);
        this.m_SMsgActionMove_SC.fDirectX = direct.x;
        this.m_SMsgActionMove_SC.fDirectY= -direct.z;

        this.m_SMsgActionMove_SC.uidEntity = this.RoleDataModel.SMsg_Header.uidEntity;
        this.m_SMsgActionMove_SC.dwMapID = GameManager.Instance.GetCurSceneMapID;
		if(m_FSMSystem.CurrentStateID == StateID.PlayerIdle)
		{
			this.m_SMsgActionMove_SC.fSpeed = 0.0f;	
		}
		else if(m_FSMSystem.CurrentStateID == StateID.PlayerRun)
		{
			
			this.m_SMsgActionMove_SC.fSpeed =((PlayerRunState)m_FSMSystem.CurrentState).CurrentMoveSpeed*10.0f;
		}
		

        return this.m_SMsgActionMove_SC.GeneratePackage(MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_MOVE);
    }
    #endregion
	
	public void OnPlayerUpdate(INotifyArgs notifyArgs)
	{
		if(m_FSMSystem.CurrentStateID == StateID.PlayerDie)
		{
			return;	
		}
        int showUpdateLevel = CommonDefineManager.Instance.CommonDefineFile._dataTable.UpgradeAnimationStartLevel;
        int currentLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
		IEntityDataStruct data = (IEntityDataStruct)(notifyArgs);
        if (RoleDataModel.SMsg_Header.uidEntity == data.SMsg_Header.uidEntity && currentLevel >= showUpdateLevel)	
		{
			GameObject levelUpEffect = GameObjectPool.Instance.AcquireLocal( PlayerDataManager.Instance.LevelUpEffectPrefab, Vector3.zero, Quaternion.identity);
			levelUpEffect.transform.parent = this.ThisTransform;
			levelUpEffect.transform.localPosition = Vector3.zero;
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Upgrade");
		}
	}
    /// <summary>
    /// 角色复活
    /// </summary>
    public void OnPlayerResurrection(INotifyArgs notifyArgs)
    {
//        SMsgActionRelivePlayer_SC sMsgActionRelivePlayer_SC = (SMsgActionRelivePlayer_SC)notifyArgs;
//        if (((IPlayerDataStruct)m_roleDataModel).PlayerActorID != sMsgActionRelivePlayer_SC.actorTarget)
//            return;
//
//        //GameObject levelUpEffect = GameObjectPool.Instance.AcquireLocal(PlayerDataManager.Instance.LevelUpEffectPrefab, Vector3.zero, Quaternion.identity);
//		GameObject levelUpEffect = Instantiate(PlayerDataManager.Instance.LevelUpEffectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
//		levelUpEffect.transform.parent = this.ThisTransform;
//        levelUpEffect.transform.localPosition = Vector3.zero;
//        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Upgrade");
    }

    #region »ùÀà·œ·šÖØÐŽ
    public override ObjectType GetFHObjType()
    {
        return this.m_isHero ? ObjectType.Hero : ObjectType.Member;
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.PlayerLevelUpdate.ToString(), OnPlayerUpdate);
        AddEventHandler(EventTypeEnum.EntityRelive.ToString(), OnPlayerResurrection);
    }
    protected override void CheckRoleIsHeroOrNot()
    {
        if (!IsCopy)
        {
            if (this.m_isHero)
            {
                GameManager.Instance.TimedSendPackage.AddSendInfoObj(this);
            }
            else
            {
                m_smooth = new MoveSmooth();
            }
        }
    }
    #endregion

    public EntityModel ActionLockTarget { get; set; }
	
	
#if UNITY_EDITOR
	void OnDrawGizmos ()
	{
		for(int i = 0; i < nodes.Count -1; i++)
		{
			Gizmos.DrawLine(nodes[i], nodes[i+1]);
		}
		
	}	
#endif


    //žùŸÝ²ß»®Á¬ÕÐÐÞžÄ·œ°ž£¬ÓÃ»§ÔÚœøÈëŒŒÄÜÑ¡Ôñ×ŽÌ¬£¬²»ÖÐ¶Ïµ±Ç°Ê©·ÅµÄÆÕÍš¹¥»÷£¬ËùÒÔ°ÑŒŒÄÜÑ¡ÔñŽŠÀí·ÅÔÚÕâ¡£
    private InitiativeSkillSelectedState m_initiativeSkillSelectedState;
    //œÇÉ«Ñ¡ÔñŒŒÄÜ°ŽÅ¥£¬²»žÄ±äµ±Ç°ŒŒÄÜ£¬°ÑÑ¡ÔñµÄŒŒÄÜÏÈ»ºŽæ£¬ÔÚÊ©·ÅµÄÊ±ºò²ÅÕæÕýžÄ±ä¡£
    public int m_nextSkillID;
    public SkillBase NextSkillBase {
        get
        {
            return this.m_skills.SingleOrDefault(P => P.SkillId == this.m_nextSkillID);
        }
    }
    //ÔÚžÄ±ä×ŽÌ¬µÄÊ±ºòÀ¹œØTransition.PlayerInitialtiveSkillSelect£¬²»ÔÙžÄ±ä×ŽÌ¬¡£»»³Éµ÷ÓÃInitiativeSkillSelectedStateµÄÏà¹Ø·œ·š¡£
    //ÆäÖÐÒ»žöºÜÖØÒªµÄ±ä»¯ÊÇÔÚPlayerBehaviourÉèÖÃÒ»žö¿ª¹Ø£¬¿ØÖÆÊÇ·ñÔÚUpdateÖÐµ÷ÓÃInitiativeSkillSelectedState×ŽÌ¬µÄReason·œ·šºÍAct·œ·š¡£
    //Õâžö¿ª¹ØÓÉInitiativeSkillSelectedStateÄÚ²¿¿ØÖÆ¡£
    //DoBeforeEnteringŽò¿ª£šExecuteInitiativeSkillSelectedState = true£©¡£
    //DoBeforeLeaving¹Ø±Õ£šExecuteInitiativeSkillSelectedState = false£©¡£
    //œøÈë×ŽÌ¬¿É¿Ø£¬Àë¿ª×ŽÌ¬--È¡ÏûŒŒÄÜ/ŒŒÄÜÊ©·Å/ŒŒÄÜ±»Žò¶Ï
    public bool ExecuteInitiativeSkillSelectedState = false;

    public void LeaveInitiativeSkillSelectedState()
    {
        if (ExecuteInitiativeSkillSelectedState
            &&m_initiativeSkillSelectedState != null)
        {
            m_initiativeSkillSelectedState.DoBeforeLeaving();
        }
    }

    protected override void OnDestroy()
    {
        CancelInvoke("LockMonster");
        CancelInvoke("CheckPlayerEnterTriggerArea");
        base.OnDestroy();
    }
	
	public bool m_keepNormalSkillStep = false;
	
	public void StartCountResetNormalSkillStep()
	{
		StopCoroutine("CountResetNormalSkillStep");
		StartCoroutine("CountResetNormalSkillStep");
	}
	
	IEnumerator CountResetNormalSkillStep()
	{
		m_keepNormalSkillStep = true;
		
		yield return new WaitForSeconds(CommonDefineManager.Instance.CommonDefine.ATTACK_CONTINUEPERIOD);
		m_keepNormalSkillStep = false;
		PlayerManager.Instance.NormalAttackRemembering = false;  //直接赋值，不用下面的事件
		//RaiseEvent (EventTypeEnum.NormalContinueExpried.ToString (), null);
	}

}
