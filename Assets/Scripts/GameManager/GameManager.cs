using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NetworkCommon;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using UI.MainUI;


public enum GameMode
{
	SINGLE_PLAYER = 0,
	MULTI_PLAYER,
}
//副本类型：0=常规副本，1=封魔副本 2=pvp副本 3=封妖副本 4=练功房 5=试炼副本 6=新手副本 7= 8=防守副本 9=首领讨伐 10=无尽试炼
public enum EEcytpeBattleType
{
	EEctypeNull = -1,
	EEctypeNormal = 0,
	EEctypeDevil,
	EEctypePvp,
	EEctypeFairy,
	EEctypeKongHu,
	EEctypeTrail,
	EEctypeNewBie,
	EEctypeDefence = 8,
	EEctypeCaptical,
	EEctypeEndless
}
public class GameManager : View
{
	public GameSettings m_gameSettings;
	
	
	GameMode m_currentGameMode = GameMode.MULTI_PLAYER;
	public GameMode CurrentGameMode
	{
		get {return m_currentGameMode;}
		set { m_currentGameMode = value; }
	}
	
	
	public enum GameState
	{
		INTRO = 0,
		GAME_STATE_LOGIN,
		GAME_STATE_TOWN,
		GAME_STATE_BATTLE,
        GAME_STATE_STORYLINE,
        GAME_STATE_PLAYERROOM,
		GAME_STATE_OPENANIMATION,
	}
	
	public bool ResponseHandlerOn
	{
		get {return !ResponseHandleInvoker.Instance.IsPaused; }
	}
    public bool IsLogin91Version=true;
    public bool EnableHeart=true;
    public bool ShowPlayerName = true;
    public bool IsNewbieGuide = true;
    public bool IsShowCameraAdjust = false;
	public bool IsShowBloodLabel = true;
    /// <summary>
    /// 登录平如类型，默认是本地
    /// </summary>
    public PlatformType PlatformType = PlatformType.Local;

    private bool m_showGoToBattleBtn = false;




	private GameState m_currentState = GameManager.GameState.INTRO;
	public GameState CurrentState { get { return m_currentState; } }
    public bool IsLockOperatorMode { get; set; }
	private GameState m_previousState = GameManager.GameState.INTRO;
	private GameState m_nextState;
	private bool m_changeState;
    private string m_curSceneName;
    private uint m_curSceneMapID;
    public bool IsPlayerRoomerLeave { get; set; }
    private SMsgActionNewWorld_SC m_newWorldMsg;
    //存储战斗副本的类型[应用：知道从哪类战斗副本回到主城镇]
	public EEcytpeBattleType ectypeType = EEcytpeBattleType.EEctypeNull;
	
	public SceneManager SceneManager;

    private TimedSendPackage m_timedSendPackage;
    private PlayerFactory m_playerFactory;
    //private MonsterFactory m_monsterFactory;
    private DamageFactory m_damageFactory;
    private TrapFactory m_trapFactory;
    private PortalFactory m_portalFactory;
    private NPCFactory m_npcFactory;
    private SenceAudioFactory m_SenceAudioFactory;
    //private BulletFactory m_bulletFactory;
    private ActionEffectFactory m_actionEffectFactory;
    [HideInInspector]
    private bool m_createEntityIM = false;  //是否立即创建实体
    public Action SceneLoadedAction;
    public bool CreateEntityIM
    {
        get { return m_createEntityIM; }
        set
        {
            m_createEntityIM = value;
            if (SceneLoadedAction != null)
            {
                SceneLoadedAction();
            }
        }
    }

    private static GameManager m_instance;
    public static GameManager Instance
    {
        get
        {
            return m_instance;
        }
    }
    /// <summary>
    /// 场景是否切换完成
    /// </summary>
    public bool SceneLoaded { get; set; }
    public TimedSendPackage TimedSendPackage
    {
        get { return this.m_timedSendPackage; }
    }
    public PlayerFactory PlayerFactory
    {
        get { return this.m_playerFactory; }
    }
    //public MonsterFactory MonsterFactory
    //{
    //    get { return this.m_monsterFactory; }
    //}
    public DamageFactory DamageFactory
    {
        get { return this.m_damageFactory; }
    }
    public TrapFactory TrapFactory
    {
        get { return this.m_trapFactory; }
    }
    public PortalFactory PortalFactory
    {
        get { return this.m_portalFactory; }
    }
    public NPCFactory NPCFactory
    {
        get { return this.m_npcFactory; }
    }
    public SenceAudioFactory SenceAudioFactory
    {
        get{return m_SenceAudioFactory;}
    }
    //public BulletFactory BulletFactory
    //{
    //    get { return this.m_bulletFactory; }
    //}
    public ActionEffectFactory ActionEffectFactory
    {
        get { return this.m_actionEffectFactory; }
    }
	
	
	//meet npc entityId;
	private Int64 m_meetNpcEntityId;
	public Int64 MeetNpcEntityId
	{
		get { return m_meetNpcEntityId; }
		set { m_meetNpcEntityId = value; }
	}

    public int MainButtonIndex { set; get; } 
	
	//for block show
	public bool m_showBlock;
	public bool ShowDynamicBlock;
	public GameObject m_blockPrefab;    
	private List<GameObject> m_blockObjectList;
	
    //for andriod exit game messagebox
    public GameObject MessageBoxExitGame;
    private GameObject m_MessageBoxExitGame;
	[HideInInspector]
	public bool isTeamBattleMark = false;
	public void AddBlockCell(Vector3 pos)
	{	
		if(null == m_blockObjectList)
		{
			m_blockObjectList = new List<GameObject>();	
		}
			
		GameObject obj = Instantiate(m_blockPrefab, pos, Quaternion.identity) as GameObject;
		
		m_blockObjectList.Add(obj);
	}
	public void ClearBlockList()
	{
		if(null == m_blockObjectList)
		{
			return;	
		}
		
		foreach(GameObject obj in m_blockObjectList)
		{
			Destroy(obj);	
		}
		m_blockObjectList.Clear();
	}

	public void ClearDynamicBlock()
	{

	}

    void Awake()
    {
       PlatformType=PlatformType.Local;
#if (UNITY_ANDROID && !UNITY_EDITOR)
#if ANDROID_OPPO            
            PlatformType=PlatformType.OPPO;
#elif ANDROID_UC
        PlatformType=PlatformType.UC;
#elif ANDROID_XIAOMI
        PlatformType=PlatformType.MI;
#elif ANDROID_TENCENT
		PlatformType = PlatformType.Tencent;
#endif
#endif
        TraceUtil.Log(PlatformType);
        Application.targetFrameRate = 60;
		
        m_instance = this;        
        this.m_timedSendPackage = GetComponent<TimedSendPackage>();
        this.m_playerFactory = GetComponent<PlayerFactory>();
        //this.m_monsterFactory = GetComponent<MonsterFactory>();
        this.m_damageFactory = GetComponent<DamageFactory>();
        this.m_trapFactory = GetComponent<TrapFactory>();
        this.m_portalFactory = GetComponent<PortalFactory>();
        this.m_npcFactory = GetComponent<NPCFactory>();
        this.m_SenceAudioFactory=GetComponent<SenceAudioFactory>();
        //this.m_bulletFactory = GetComponent<BulletFactory>();
        this.m_actionEffectFactory = GetComponent<ActionEffectFactory>();

        LoginManager.Instance.Init91();
        this.RegisterEventHandler();
		
		this.m_gameSettings = GetComponent<GameSettings>();

        //打印按钮Id
        TraceUtil.Log(SystemModel.Common,TaskBtnManager.Instance.GetBtnId());        
    }
	private string openingAnim = "openingAnim";
	// Use this for initialization
	void Start () {
        //DontDestroyOnLoad(this);
        //游戏启动时，注册网络Service
        NetServiceManager.Instance.RegisterService();
		//GotoState(GameState.GAME_STATE_LOGIN);
		if (JudgePlayOpeningAnimation()) {
			GotoState (GameState.GAME_STATE_LOGIN);
		} else {
			GotoState (GameState.GAME_STATE_OPENANIMATION);//
		}
	}
   
	// Update is called once per frame
    void Update()
    {
        //获得统计数据
        GetStatisticsData();
        //
#if (!UNITY_EDITOR&&(UNITY_ANDROID||UNITY_IPHONE))
        CheckTouchEvent();
#endif        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_MessageBoxExitGame == null)
            {
                m_MessageBoxExitGame = (GameObject)Instantiate(MessageBoxExitGame);
                if (PopupObjManager.Instance == null)
                {
                    m_MessageBoxExitGame.transform.parent = NGUITools.FindCameraForLayer(25).transform;
                }
                else
                {
                    m_MessageBoxExitGame.transform.parent = PopupObjManager.Instance.UICamera.transform;
                }                                
                m_MessageBoxExitGame.transform.localPosition = Vector3.zero;
                m_MessageBoxExitGame.transform.localScale = Vector3.one;
                MsgBoxExitGame msgScript = m_MessageBoxExitGame.GetComponent<MsgBoxExitGame>();
                msgScript.ShowBox(LanguageTextManager.GetString("IDS_D6_509"), OnExitGameHandle, OnCancelHandle);
            }            
            //UI.MessageBox.Instance.Show(5, "", LanguageTextManager.GetString("IDS_D6_509"), LanguageTextManager.GetString("IDS_H2_55"),
            //    LanguageTextManager.GetString("IDS_H2_28"), OnExitGameHandle, OnCancelHandle);
        }

        //启动网络消息队列处理
        
        //if (true)
        {
            ResponseHandleInvoker.Instance.DO();
        }

    }

    void OnExitGameHandle(object obj)
    {
        float waitTime = 0;
        var heroModel = PlayerManager.Instance.FindHeroEntityModel();
        if (heroModel != null && SceneManager.CurrentMapId.HasValue)
        {
            NetServiceManager.Instance.CommonService.SendPlayerLoginOut(heroModel.EntityDataStruct.SMsg_Header.uidEntity, SceneManager.CurrentMapId.Value);
            waitTime = 0.5f;
        }
        StartCoroutine(AppQuit(waitTime));
    }   
    private IEnumerator AppQuit(float time)
    {
        yield return new WaitForSeconds(time);
        Application.Quit();
    }
    void OnCancelHandle(object obj)
    {
        Destroy(m_MessageBoxExitGame);
        m_MessageBoxExitGame = null;
    }

    //private bool m_isDoubleClick;
    private void CheckClickEvent()
    {
        if (TouchAndClickEventValid())
        {
            return;
        }
        Event Mouse = Event.current;
        if (Mouse!=null && Mouse.isMouse && Mouse.type == EventType.MouseDown)            
        {
            //m_isDoubleClick = false;
            var clickPoint = Input.mousePosition;
            if (UICamera.currentCamera != null)
            {
                var uiRay = UICamera.currentCamera.ScreenPointToRay(clickPoint);
                RaycastHit uiRaycastHit;

                if (Physics.Raycast(uiRay, out uiRaycastHit, 100))
                {
                    return;
                }
            }
            RaycastHit m_hit;

            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(clickPoint); 
            
            if (Physics.Raycast(ray, out m_hit, Mathf.Infinity, ~(1 << 9 | 1 << 10 | 1 << 11 | 1 << 19 | 1<< 20)))
            {
                if (Mouse.clickCount == 2)
                {
                    RaiseEvent(EventTypeEnum.OnTouchInvoke.ToString(), new TouchInvoke(m_hit.transform.gameObject, m_hit.point, 1));
                }
                else 
                {
                    RaiseEvent(EventTypeEnum.OnTouchInvoke.ToString(), new TouchInvoke(m_hit.transform.gameObject, m_hit.point, 1));
                }
            }
           
        } 
    }
    /// <summary>
    /// 检查主摄像机的点击是否生效，如果生效并且处于弱引导状态，则需要抛出ClickOtherButton事件结束任务
    /// </summary>
    /// <returns></returns>
    private bool TouchAndClickEventValid()
    {
        bool inValid = Camera.main == null  //没有主摄像机
            || !TaskModel.Instance.ResponseClickEvent   //当前任务是强引导
            || !PVPBattleManager.Instance.IsCanMove();          //当前不能移动
        if (!inValid)
        {
            //if (TaskModel.Instance.TaskGuideType == TaskGuideType.Weak)
            //{
            //    UIEventManager.Instance.TriggerUIEvent(UIEventType.ClickOtherButton, null);
            //}
        }
        return inValid;
    }
    private void CheckTouchEvent()
    {       
        if (TouchAndClickEventValid())
        {
            return;
        }
       
        for (int i = 0; i < Input.touchCount; ++i)
        {
            var clickAmount = InputUtil.Instance.ClickAmount;
            var clickPoint = InputUtil.Instance.TouchPosition;
            if (UICamera.currentCamera != null)
            {
                var uiRay = UICamera.currentCamera.ScreenPointToRay(clickPoint);
                RaycastHit uiRaycastHit;

                if (Physics.Raycast(uiRay, out uiRaycastHit, 100))
                {
                    return;
                }
            }

            RaycastHit m_hit;
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(clickPoint);
            if (Physics.Raycast(ray, out m_hit, Mathf.Infinity, ~(1 << 9 | 1 << 10 | 1 << 11 | 1 << 19 | 1 << 20)))
            {
                if (clickAmount==1)
                {
                    RaiseEvent(EventTypeEnum.OnTouchInvoke.ToString(), new TouchInvoke(m_hit.transform.gameObject, m_hit.point, 1));
                }
                else if (clickAmount == 2)
                {
                    RaiseEvent(EventTypeEnum.OnTouchInvoke.ToString(), new TouchInvoke(m_hit.transform.gameObject, m_hit.point, 1));
                }
            }
        }
    }	

    /// <summary>
    /// 场景转换处理
    /// </summary>
    /// <param name="notifyArgs"></param>
    void SceneChangeHandle(INotifyArgs notifyArgs)
    {        
        PlayerManager.Instance.LockPlayOnSceneChange(false);
		//add by lee : stop player state
		PlayerManager.Instance.StopPlayersSkill();
        SMsgActionNewWorld_SC targetScene = (SMsgActionNewWorld_SC)notifyArgs;
        m_newWorldMsg = targetScene;
        //如果是需要换服，需要对一些数据进行管理
        switch ((eTeleportType)targetScene.byTeleportFlg)
        {
            case eTeleportType.TELEPORTTYPE_JUMPSERVER:
                HeartFPSManager.Instance.Clear();
                SingletonManager.Instance.Clear();
                NotifyManager.ClearEvents();
                EnegryColdWorkData.Instance.Re_registration();
                TownGuideManager.Instance.ResetEventHandler();
                RegisterEventHandler();								
                break;
        }
       
        GameState targetSceneState = SceneManager.GetCurSceneState(targetScene.dwMapId);
       
        //TraceUtil.Log(SystemModel.Rocky, "下一个地图ID:" + targetScene.dwMapId);
        //TraceUtil.Log(SystemModel.Rocky, "下一个dwMapParam:" + targetScene.dwMapParam);
        
        this.GotoState(targetSceneState, true);
		int returnMapID = DealRequestEctypeSceneChange (targetScene);
		if ( returnMapID != 0) {
			m_curSceneMapID = (uint)returnMapID;
		} else {
			//跳转生效//
			m_curSceneMapID = targetScene.dwMapId;
		}
        SceneDataManager.Instance.ResetMapTridderAreaInfo(m_curSceneMapID);
        //TraceUtil.Log(SystemModel.Rocky, "下一个地图ID(剧情过滤完成后):" + m_curSceneMapID);
		m_curSceneName = EctypeConfigManager.Instance.SceneConfigList[(int)m_curSceneMapID]._szSceneName;
		IsLockOperatorMode = EctypeConfigManager.Instance.SceneConfigList[(int)m_curSceneMapID]._isLockMode;

        //TraceUtil.Log(SystemModel.Rocky, "下一个Scene Name:" + m_curSceneName);
    }
	//保存服务器发送要播放剧情数据//
	public StroyLineConfigData GetStoryConfigData
	{
		get{
			return storyConfigData;
		}
	}
	private StroyLineConfigData storyConfigData ;
	//返回场景ID
	private int DealRequestEctypeSceneChange(SMsgActionNewWorld_SC targetScene)
	{
		//此时mapID是否生效问题，当其为0,1时再次请求，如果为2时
		if (targetScene.dwMapParam != 0)
		{
			if(IsCreateRoleStroyLine((int)targetScene.dwMapParam))
			{
				EctypeModel.Instance.SetPreEctypeID(CommonDefineManager.Instance.CommonDefine.TUTORIAL_ECTYPE_ID);
			}
			//当剧情ID不为0，说明存在要播放剧情//
			foreach (var stroyLineConfig in StroyLineConfigManager.Instance.GetStroyLineConfig) {
				if (stroyLineConfig.Value._StroyLineID == targetScene.dwMapParam) 
				{
					storyConfigData = stroyLineConfig.Value;
					StroyLineDataManager.Instance.curStroyLineKey = new StroyLineKey{VocationID = storyConfigData._TriggerVocation,
						ConditionID = storyConfigData._TriggerCondition, EctypeID = storyConfigData._EctypeID};
					return stroyLineConfig.Value._SceneMapID;
				}
			}
		}
		return 0;
	}
	bool IsCreateRoleStroyLine(int stroyID)
	{
		if (CommonDefineManager.Instance.CommonDefine.createClass1StoryLineID == stroyID || CommonDefineManager.Instance.CommonDefine.createClass4StoryLineID == stroyID) {
			return true;		
		}
		return false;
	}
	//当剧情播放完成后
	public void DealStroyOverRequestServer()
	{
		if (storyConfigData._TriggerCondition == 2) {
			//剧情完成时触发
			//发送新协议，模糊请求跳转
			EctypeModel.Instance.StoryOverRequestEctype();
		} else if (storyConfigData._TriggerCondition == 0 || storyConfigData._TriggerCondition == 1) {
			//1=进入触发；0=新手剧情
			//直接请求副本，1
			EctypeModel.Instance.SendGoBattleToServer ();
		}
	}
	//获取剧情数据
/*	public StroyLineConfigData? GetStroyConfigByStoryID(int storyID)
	{
		foreach (var data in StroyLineConfigManager.Instance.GetStroyLineConfig) {
			if(data.Value._StroyLineID == storyID)
			{
				return data.Value;
			}
		}
		return null;
	}*/
    /// <summary>
    /// 玩家被踢出处理
    /// 断开服务器连接
    /// 跳到Login场景
    /// 清理除各种单例(玩家，怪物)
    /// 清理事件
    /// </summary>
    /// <param name="notifyArgs"></param>
    void PlayerBeKickedHandle(INotifyArgs notifyArgs)
    {
        TraceUtil.Log("被踢出消息");
		if(this.SceneLoaded)
		{
			ServiceKickedoutMsg(null);
		}
		else
		{
			AddEventHandler(EventTypeEnum.SceneLoaded.ToString(), ServiceKickedoutMsg); 

		}       
    }
	private void ServiceKickedoutMsg(INotifyArgs args)
	{
		UI.MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_191"), LanguageTextManager.GetString("IDS_H2_13")
		                            , () =>
		                            {
			GameManager.Instance.GotoState(GameManager.GameState.GAME_STATE_LOGIN, true);
			//如果用户是被踢出，处理完被踢出消息会返回CommandCallbackType.Pause, 这时候ResponseHandleInvoker会不再处理任何网络消息。
			//在用户确认被踢出消息后，应该把开关打开，才能处理网络消息
			ResponseHandleInvoker.Instance.IsPaused = false;
			CheatManager.Instance.isIDKickedMark = false;
			this.ResetConnect();
		});
		RemoveEventHandler(EventTypeEnum.SceneLoaded.ToString(), ServiceKickedoutMsg); 
	}
    public void QuitToLogin()
    {
        float waitTime = 0;
        var heroModel=PlayerManager.Instance.FindHeroEntityModel();
        if(heroModel!=null&&SceneManager.CurrentMapId.HasValue)
        {
            NetServiceManager.Instance.CommonService.SendPlayerLoginOut(heroModel.EntityDataStruct.SMsg_Header.uidEntity, SceneManager.CurrentMapId.Value);
            waitTime = 0.5f;
        }
        //清任务数据
        //TownGuideManager.Instance.LifeOver();
        //TaskModel.Instance.LifeOver();

        StartCoroutine(QuitToLogin(waitTime));
    }
    private IEnumerator QuitToLogin(float time)
    {
        yield return new WaitForSeconds(time);
        this.ResetConnect();
		ChatRecordManager.Instance.LifeOver();//清除聊天记录
        GameManager.Instance.GotoState(GameManager.GameState.GAME_STATE_LOGIN, true);
    }
    /// <summary>
    /// 收到服务器场景切换，创建角色消息
    /// </summary>
    /// <param name="notifyArgs"></param>
    void ReceiveSceneLoadedHandle(INotifyArgs notifyArgs)
    {
        CreateEntityIM = true;
        SceneLoaded = true;
        this.PlayerFactory.CreatePlayerObject();
        //this.MonsterFactory.CreateMonsterObject();
        if (GameManager.Instance.CurrentState == GameState.GAME_STATE_BATTLE)
        {
            MonsterFactory.Instance.CreateMonsterObject();            
        }        
        this.DamageFactory.CreateDamageGameObject();
        this.TrapFactory.CreateTrapGameObject();
        this.PortalFactory.CreatePortalGameObject();
        this.NPCFactory.CreateNPCGameObject();
        if(CurrentState!=GameState.GAME_STATE_LOGIN)
        {
        this.SenceAudioFactory.CreateSceneAudioObject(GetCurSceneMapID);
        }
        PlayerManager.Instance.SceneLoadedHandle(notifyArgs);
        PlayerManager.Instance.PlayStartSkill();
    }
    public void ResetConnect()
    {
        ResponseHandleInvoker.Instance.ClearResponseQueue();
        ResponseHandleInvoker.Instance.IsPaused = false;
        HeartFPSManager.Instance.Clear();
        IpManager.InitServiceConfig();
        ServiceManager.DestorySockets();
        SingletonManager.Instance.Clear();
        NotifyManager.ClearEvents();
        ResetTaskInstance();
        RegisterEventHandler();       
        UI.MainUI.EnegryColdWorkData.Instance.ResetEventHadels();
    }
    /// <summary>
    /// 重置任务单例
    /// </summary>
    private void ResetTaskInstance()
    {
        TaskModel.Instance.LifeOver();
        TownGuideManager.Instance.LifeOver();
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.SceneChange.ToString(), SceneChangeHandle);
        AddEventHandler(EventTypeEnum.GotoStroyLine.ToString(), GotoStroyLineHandle);
        AddEventHandler(EventTypeEnum.PlayerBeKicked.ToString(), PlayerBeKickedHandle);
        AddEventHandler(EventTypeEnum.SceneLoaded.ToString(), ReceiveSceneLoadedHandle);    
    }
	//vip等级更新
	public void OnVipGradeUpdate(int vipValue)
	{
		//显示升级奖励界面
		PopupObjManager.Instance.OpenVipUpgradePanel (ERewardPopType.EVipUpgrade,PlayerDataManager.Instance.GetVipCurVocatReward ());
		//UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.VipUpgrade,PlayerDataManager.Instance.GetVipCurVocatReward ());
		UIEventManager.Instance.TriggerUIEvent(UIEventType.VipGradeUpdate, vipValue);
	}
    void GotoStroyLineHandle(INotifyArgs notifyArgs)
    {
        //切换到剧情
       // PlayerManager.Instance.LockPlayOnSceneChange(false);
        m_curSceneName = StroyLineDataManager.Instance.GetStroySceneName();
        //m_curSceneName = "StroyLine01";
        GotoState(GameState.GAME_STATE_STORYLINE, true);
    }	    

	public void GotoState(GameManager.GameState state)
	{	
		if(m_currentState != state)
        {
            ShowLoadSceneUI(state);
			m_nextState = state;
			m_changeState = true;
		}
	}

    /// <summary>
    /// 获取当前场景ID
    /// </summary>
    public uint GetCurSceneMapID
    {
        get { return m_curSceneMapID; }
    }
    /// <summary>
    /// 获取当前传送信息
    /// </summary>
    public SMsgActionNewWorld_SC GetNewWorldMsg
    {
        get { return m_newWorldMsg; }
    }

	public void GotoState(GameManager.GameState state, bool force)
	{
        CreateEntityIM = false;
		m_nextState = state;
        m_changeState = true;
        ShowLoadSceneUI(state);
	}
	
	public void LateUpdate()
	{
		if(m_changeState)
		{
			m_changeState = false;
			m_previousState = m_currentState;
			m_currentState = m_nextState;
            SceneLoaded = false;
			OnStateChanged(m_currentState);
			//StartCoroutine(DoStateChanged(_currentState));
		}
	}
	
	public void PlaySceneMusic()
	{
		 SoundManager.Instance.StopBGM ();
         string musicId = EctypeConfigManager.Instance.SceneConfigList[(int)m_curSceneMapID]._mapBGM;
         SoundManager.Instance.PlayBGM(musicId);
	}
	
	private void OnStateChanged(GameManager.GameState state)
	{
		GameObjectPool.Instance.ClearCache();
        SoundManager.Instance.OnSceneChanged();  
        RaiseEvent(EventTypeEnum.StateChange.ToString(),null);
		switch(state)
		{
			case GameState.INTRO:
                {                   
                    string mainScene = "Intro";                    
                    SceneManager.GotoScene(SceneManager.LoadingIndication.FULL_LOAD, SceneManager.LoadingMode.UNLOAD_CURRENT, mainScene);
                }   
			break;

			case GameState.GAME_STATE_OPENANIMATION:
			{                   
				string mainScene = "OpeningAnimation";                    
				SceneManager.GotoScene(SceneManager.LoadingIndication.FULL_LOAD, SceneManager.LoadingMode.UNLOAD_CURRENT, mainScene);
			}   
				break;

			case GameState.GAME_STATE_LOGIN:
            {
                SceneDataManager.Instance.ClearBlockCells();
				SceneDataManager.Instance.ClearDynamicBlockData();

                string[] addScene = new string[2];

                addScene[1] = "TMap_Login";
                addScene[0] = "LoginUI";
                SceneManager.GotoScene(SceneManager.LoadingIndication.FULL_LOAD, SceneManager.LoadingMode.UNLOAD_CURRENT, addScene[0], addScene[1]);
            }
			break;
	
			case GameState.GAME_STATE_BATTLE:
			{
                //UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.Loaing);
				string[] addScene = new string[2];
				
				
                addScene[0] = m_curSceneMapID.ToString() + "DataScene";
				addScene[1] = "BattleUI";

                string mainScene = m_curSceneName;
                m_showGoToBattleBtn = false;
				SceneManager.GotoScene(SceneManager.LoadingIndication.FULL_LOAD, SceneManager.LoadingMode.UNLOAD_CURRENT, mainScene, addScene);

                StartCoroutine(SceneDataManager.Instance.InitBlockCells(this.m_curSceneMapID));
			}
			break;
			
			case GameState.GAME_STATE_TOWN:
			{
				CurrentGameMode = GameMode.MULTI_PLAYER;
                SceneDataManager.Instance.ClearBlockCells();
				SceneDataManager.Instance.ClearDynamicBlockData();
                //UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.Loaing);
				string[] addScene = new string[3];

                addScene[0] = "TownUIScene";
				addScene[1] = "BMap_Siren";
			    addScene[2] = "TMap_QingLHui_01";
                //addScene[2] = "TMap_Login";

                string mainScene = m_curSceneName;
                m_showGoToBattleBtn = true;

                //任务测试数据 
                //TaskModel.Instance.MakeDemoData();

                #region add by lee
                PVPBattleManager.Instance.ClearPVPPlayerData();//清除pvp玩家数据
                #endregion

				SceneManager.GotoScene(SceneManager.LoadingIndication.FULL_LOAD, SceneManager.LoadingMode.UNLOAD_CURRENT, mainScene, addScene);                                
			}
				
			break;

            case GameState.GAME_STATE_STORYLINE:
            {
                SceneDataManager.Instance.ClearBlockCells();
				SceneDataManager.Instance.ClearDynamicBlockData();
                string[] addScene = new string[2];
                addScene[0] = m_curSceneMapID.ToString() + "DataScene";
                addScene[1] = "StroyLineUI";

                string mainScene = m_curSceneName;
                m_showGoToBattleBtn = true;
                SceneManager.GotoScene(SceneManager.LoadingIndication.FULL_LOAD, SceneManager.LoadingMode.UNLOAD_CURRENT, mainScene, addScene);
            }
            break;

            case GameState.GAME_STATE_PLAYERROOM:
            {     
				Debug.LogError("已撤销 PlayerRoomUI.Scene");
//                SceneDataManager.Instance.ClearBlockCells();
//				SceneDataManager.Instance.ClearDynamicBlockData();
//                string[] addScene = new string[1];
//
//                addScene[0] = "PlayerRoomUI";
//
//                string mainScene = m_curSceneName;
//                m_showGoToBattleBtn = false;
//                SceneManager.GotoScene(SceneManager.LoadingIndication.FULL_LOAD, SceneManager.LoadingMode.UNLOAD_CURRENT, mainScene, addScene);
            }
            break;
		}
        
	}


    //Add By Jiang
    void ShowLoadSceneUI(GameState state)
    {
        switch (state)
        {
			case GameState.GAME_STATE_OPENANIMATION:
				ShowLoadSceneUI(LoadSceneData.LoadSceneType.OpenAnimation);
				break;
            case GameState.GAME_STATE_BATTLE:
				BattleSceneData.Instance.InitMySelf();
                ShowLoadSceneUI(LoadSceneData.LoadSceneType.Battle);
                break;
            case GameState.GAME_STATE_LOGIN:
                ShowLoadSceneUI(LoadSceneData.LoadSceneType.Login);
                break;
            case GameState.GAME_STATE_PLAYERROOM:
                ShowLoadSceneUI(LoadSceneData.LoadSceneType.Town);
                break;
            case GameState.GAME_STATE_STORYLINE:
				BattleSceneData.Instance.InitMySelf();
                ShowLoadSceneUI(LoadSceneData.LoadSceneType.StoryLine);
                break;
            case GameState.GAME_STATE_TOWN:
			//BattleSceneData.Instance.InitMySelf();
                ShowLoadSceneUI(LoadSceneData.LoadSceneType.Town);
                break;
            case GameState.INTRO:
                ShowLoadSceneUI(LoadSceneData.LoadSceneType.Town);
                break;
        } 
    }

    void ShowLoadSceneUI(LoadSceneData.LoadSceneType loadSceneType)
    {
        var loadSceneData = GameDataManager.Instance.PeekData(DataType.LoadingSceneData) as LoadSceneData;
        if (loadSceneData == null) 
        {
            loadSceneData = new LoadSceneData(loadSceneType, 0); 
        }
        else
        {
            loadSceneData.loadSceneType = loadSceneType;
            loadSceneData.Progress = 0;
        }
        TraceUtil.Log("ShowLoadingUIPanel:"+loadSceneType);
        //UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.Loaing);
        GameDataManager.Instance.ResetData(DataType.LoadingSceneData,loadSceneData);
    }

#region 技能测试代码
    /// <summary>
    /// Raises the GU event.
    /// 直接跳转副本按钮....
    /// 给测试人员用
    /// </summary>
    private bool m_test = false; 


    public string LogText = "None";
    
    void OnGUI()
    {
#if UNITY_EDITOR
		if(m_currentState == GameManager.GameState.GAME_STATE_LOGIN)
		{
			if(GUI.Button(new Rect(10,150,120,40),"去除开场动画标记"))
			{
				DeletePlayOpeningAnimation();
			}
		}
        
#endif
        //if(GUI.Button(new Rect(10,150,120,40),"邮箱"))
        //{
        //    UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Mail,null);
        //}

        //if (IsLogin91Version && m_showGoToBattleBtn)
        //{
        //    if (GUI.Button(new Rect(Screen.width / 2 - 75, 5, 150, 50), "GoToBattle"))
        //    {
        //        GoToBattle(1, 120);
        //    }
        //}

        //if (GUILayout.Button("rotation"))
        //{
        //    m_test = !m_test;

        //}

        //if (m_test)
        //{
        //    var playerTrans = PlayerManager.Instance.FindHero().transform;
        //    BattleManager.Instance.FollowCamera.transform.(playerTrans.up, 20 * Time.deltaTime);
        //}


#if (UNITY_EDITOR||(!UNITY_ANDROID&&!UNITY_IPHONE))
        CheckClickEvent();



#endif


#if UNITY_EDITOR
//		if(GUI.Button(new Rect(Screen.width / 2 - 75, 5, 150, 50), "Save Net Log"))
//		{
//			LogManager.Instance.SaveTxt("NetResponse");
//		}s
		
#endif

        //GUIStyle style = new GUIStyle();
        //style.fontSize = 20;
        //GUI.Label(new Rect(5, 5, 300, 50), LogText);

    }

    void GoToBattle(int difficulty, int ectypeID)
    {
        SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS();
		//TODO:进入技能协议有更改，去掉副本iD和难度
//        sMSGEctypeRequestCreate_CS.uidEntity = ((PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour).RoleDataModel.SMsg_Header.uidEntity; //sMSGEctypeDifficultySelect_SC.uidEntity;
//        sMSGEctypeRequestCreate_CS.byDifficulty = (byte)difficulty; //????
//        sMSGEctypeRequestCreate_CS.dwEctypeId = ectypeID;              //?±±?ID

        NetServiceManager.Instance.EctypeService.SendEctypeRequest(sMSGEctypeRequestCreate_CS);
    }
    /// <summary>技能测试代码结束
    /// /////////////////////////////////////////////////////////////////////////
#endregion


	public void OnLevelWasLoaded(int level)
	{
		if(!SceneManager.IsLoadingScene() )
		{
			OnEnterState(m_currentState);           
		}
	}
	
	private void OnEnterState(GameManager.GameState state)
	{

        
		switch(state)
		{
			case GameState.INTRO:

			break;			
			
			
			case GameState.GAME_STATE_LOGIN:	
			{
				SoundManager.Instance.StopBGM(0.0f);
				SoundManager.Instance.PlayBGM("Music_UIBG_Login", 0.0f);
			}
			break;
			case GameState.GAME_STATE_TOWN:
			{
               // NewbieGuideManager.Instance.SetGuideState(true);
			}
			break;
			
			case GameState.GAME_STATE_BATTLE:
			{
                //NewbieGuideManager.Instance.IsGuideFinish = true;

				//m_responseHandlerOn = true;                
			}
			break;
            case GameState.GAME_STATE_STORYLINE:
            {
                //SoundManager.Instance.StopBGM(0.0f);
                //SoundManager.Instance.PlayBGM(StroyLineDataManager.Instance.GetMapBgMusic, 0.0f);
            }
            break;
		}
        
        

		//play music
        if(state == GameState.GAME_STATE_TOWN || state == GameState.GAME_STATE_BATTLE || state == GameState.GAME_STATE_PLAYERROOM || state == GameState.GAME_STATE_STORYLINE )
        {
           	PlaySceneMusic();
			//Change PlayerWeapon and Animations
        	//TraceUtil.Log("根据当前游戏场景，改变玩家的技能，武器挂载等在SenceManager GotoSence");
			PlayerManager.Instance.PlayerChangeScene(GameManager.Instance.CurrentState);
        }
	}
    void OnApplicationQuit()
    {
        //关闭socket
        ServiceManager.DestorySockets();
    }

    void TestPhpServer()
    {
        StartCoroutine(LoginManager.Instance.RequestPhpService(LoginManager.PHPSERVER_ADDRESS, LoginManager.Instance.PhpServerCallback));
    }

    #region 相机查找目标
    public void TryFindCameraTarget()
    {        
		if(m_currentState == GameState.GAME_STATE_STORYLINE)
			return;
        StopCoroutine("ReFindCameraTarget");
        StartCoroutine("ReFindCameraTarget");
    }
    IEnumerator ReFindCameraTarget()
    {
        bool isFind = false;
        Transform target = null;
        while (isFind == false || BattleManager.Instance == null)
        {
            target = PlayerManager.Instance.FindCameraFollowTarget();
            if (target != null)
            {
                isFind = true;
                BattleManager.Instance.SetCameraTarget(target);
            }
            yield return null;
        }
    }

    #endregion
	
	
	public bool UseJoyStick
	{
		get
		{
			//查看设置为摇杆还是触屏 注：在城镇界面UseJoyStick一直为false，城镇和这个变量一点关系都没有//
			if( GameManager.Instance.m_currentState == GameState.GAME_STATE_BATTLE && m_gameSettings.JoyStickMode && !IsLockOperatorMode)
			{
				return true;	
			}
			else
			{
				return false;	
			}
		}

    }
    #region internetReachability
    public static NetworkReachability InternetReachability;
    public static bool JoyStickMode;
    public static int GameViewLevel;

    private void GetStatisticsData()
    {
        if (m_gameSettings != null)
        {
            GameManager.InternetReachability = Application.internetReachability;
            GameManager.JoyStickMode = m_gameSettings.JoyStickMode;
            GameManager.GameViewLevel = m_gameSettings.GameViewLevel;
        }
    }
    #endregion

	#region Read/Write jhLocalData
	public Dictionary <string,string> jhDataMap = new Dictionary<string,string>();
	public bool JudgePlayOpeningAnimation()
	{
		if (PlayerPrefs.GetInt (openingAnim, 0) == 1) {
			return true;		
		} else {
			return false;		
		}
	 	/*ReadLocalText ();
		if (jhDataMap.ContainsKey (openingAnim)) {
			return true;
		}
		return false;*/
	}
	public void DeletePlayOpeningAnimation()
	{
		PlayerPrefs.SetInt (openingAnim, 0);
	}
	public void SavePlayOpeningAnimation()
	{
		PlayerPrefs.SetInt (openingAnim, 1);
		/*jhDataMap.Add (openingAnim,"1");
		WriteLocalText ();*/
	}
	private void ReadLocalText()
	{
		string path = string.Format ("{0}/JHData/{1}",Application.persistentDataPath, "JH.db");
		if (File.Exists (path)) {
			byte[] data = File.ReadAllBytes (path);
			string tempData = System.Text.Encoding.Default.GetString (data);
			if (tempData == null)
				return ;
			string[] valArray = tempData.Split ('|');
			if (valArray == null)
				return ;
			foreach(string infoStr in valArray)
			{
				string[] infoArray = infoStr.Split(';');
				if (infoArray == null || infoArray.Length != 2)
				{
					Debug.Log("read JH.db error!!!!!!!" + (infoArray != null ?infoArray.Length.ToString():"null"));
					continue;
				}
				jhDataMap.Add(infoArray[0],infoArray[1]);
			}
		}
	}

	private void WriteLocalText()
	{
		StringBuilder tempLibrary = new StringBuilder(); 
		foreach (var item in jhDataMap)
		{
			tempLibrary = tempLibrary.AppendFormat("{0};{1}|",item.Key, item.Value);
		}
		string tempData = tempLibrary.ToString();
		byte[] data = System.Text.Encoding.Default.GetBytes(tempData);
		string path = string.Format ("{0}/JHData/{1}",Application.persistentDataPath, "JH.db");
		File.WriteAllBytes(path, data);
	}
	#endregion

	#region DelayTime
	public delegate void VoidCallback();
	struct CallBack
	{
		public float delay;
		public VoidCallback fn;
	}
	public void DelayCall(float delay, VoidCallback callback)
	{
		StartCoroutine(call(new CallBack() { delay = delay, fn = callback }));
	}
	private IEnumerator call(CallBack cb)
	{
		if (cb.delay <= 0)
			yield return new WaitForEndOfFrame();
		else
			//if (cb.delay <= 0) cb.delay = 0.01f;
			yield return new WaitForSeconds(cb.delay);
		cb.fn();
	}
	#endregion
}
