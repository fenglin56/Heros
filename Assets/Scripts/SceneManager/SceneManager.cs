using UnityEngine;
using System.Collections;
using System;
using NetworkCommon;

/// <summary>
/// EnterPoint Scene GameManager 场景跳转管理，进度
/// </summary>
///
public class SceneManager : View {
		
	private string m_mainScene;
	private string[] m_adictiveScene;
	private AsyncOperation[] m_loadStatus;
	private LoadingIndication m_loadingIndication;
	private LoadingMode m_loadingMode;
	private bool m_checkSceneLoaded;
	private bool m_isLoadingScene;

    private LoadSceneData loadSceneData;

    public uint? CurrentMapId { get; private set; }
    public AsyncOperation[] LoadStatus { get { return m_loadStatus; } }
	
	public enum LoadingIndication
	{
		NONE,
		QUICK_LOAD,		
		FULL_LOAD,
		MENU_LOAD
	};
	
	public enum LoadingMode
	{
		DONT_UNLOAD_CURRENT,		
		UNLOAD_CURRENT				
	};

	public string _loadingSceneName="EmptyScene";

	void Awake()
	{		

	}

    protected override void RegisterEventHandler()
    {

    }
	
	private void DestroyLoadingObject()
	{
		
	}

	
	
	public void RandomLoadingTip()
	{
		
	}
	
	
	public void CreateLoadingObject()
	{
		if(m_loadingIndication == LoadingIndication.FULL_LOAD)
		{
			{
				RandomLoadingTip();
			}
		}
	}
	
	void Start()
	{
		
	}
		
	/// <summary>
	/// Load a new scene with adictive scenes
	/// </summary>
	/// <param name="mainScene">
	/// main scene
	/// </param>
	/// <param name="aditiveScene">
	/// adictive scenes over main scene
	/// </param>
	public void GotoScene(LoadingIndication loadingIndication, LoadingMode loadingMode, string mainScene, params string[] adictiveScene)
	{
		m_loadingIndication = loadingIndication;
		CreateLoadingObject();
	

		m_loadingMode = loadingMode;
		m_checkSceneLoaded = false;
		m_isLoadingScene = false;
		
		//store the params to be used after fade out
		m_mainScene = mainScene;
		
		m_adictiveScene = (string[])adictiveScene.Clone();
		m_loadStatus = new AsyncOperation[m_adictiveScene.Length + 1];
		
        //Change PlayerWeapon and Animations
        //TraceUtil.Log("根据当前游戏场景，改变玩家的技能，武器挂载等在SenceManager GotoSence");
        PlayerManager.Instance.PlayerChangeScene(GameManager.Instance.CurrentState);

		FadeStateChanged();
	}

    public GameManager.GameState GetCurSceneState(uint mapID)
    {
        int sceneType = EctypeConfigManager.Instance.SceneConfigList[(int)mapID]._sceneType;
        CurrentMapId = mapID;
        switch (sceneType)
        {
            //0=城镇；1=副本；2=跨服；3=练功房；4=新手剧情；
            case 0:   //风雨镇
                return GameManager.GameState.GAME_STATE_TOWN;
            case 1:
                return GameManager.GameState.GAME_STATE_BATTLE;
            case 2:  //前端目前无用
                return GameManager.GameState.INTRO;
            case 3:  
                return GameManager.GameState.GAME_STATE_PLAYERROOM;
            case 4:
                return GameManager.GameState.GAME_STATE_STORYLINE;
            default:
                return GameManager.GameState.INTRO;
        }
    }

		
	public void Update()
	{		
		if (m_checkSceneLoaded)
		{
			bool isDone = true;
            float Progress = 0;
			foreach(AsyncOperation ao in m_loadStatus)
			{
                Progress += ao.progress;
				isDone &= ao.isDone;
			}
            //TraceUtil.Log("AllProgress:"+Progress);
            Progress = Progress / m_loadStatus.Length;
            //TraceUtil.Log("LoadSceneProgress:"+Progress+",Leght:"+m_loadStatus.Length);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.LoadingProgress,Progress);
            //if (this.loadSceneData == null) { loadSceneData = GameDataManager.Instance.PeekData(DataType.LoadingSceneData) as LoadSceneData; }
            //else { loadSceneData.Progress = Progress; }
			if(m_loadingIndication != LoadingIndication.FULL_LOAD)
			{
			}
	
			//now the scenes is loaded, fade game in
			if(isDone)
			{
				if(m_loadingIndication != LoadingIndication.FULL_LOAD )
                {
				}
                //UIEventManager.Instance.TriggerUIEvent(UIEventType.LoadingComplete, null);
                if (this.loadSceneData != null)
                {
                    loadSceneData.Progress = 1;
                }                
				HideAll();
			
				m_checkSceneLoaded = false;
				m_isLoadingScene = false;
				
                //GameManager hxz
                GameManager.Instance.SceneLoaded = true;
			}
		}
	}
	
	public bool IsLoadingScene()
	{
		return m_isLoadingScene;
	}
	
	 void OnLevelWasLoaded(int level)
	{

		if(IsLoadingScene())
		{
			StartCoroutine("LoadNextScene");
		}
		
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}
	
	private IEnumerator LoadNextScene()
	{
		m_isLoadingScene = false;
		LoadScene();
		yield return null;
	}
	
	
	private float TOTAL_TIME = 5.0f;
	private float _beginTime = 0;
	private IEnumerator CheatLoadingBar()
	{
		_beginTime  = Time.realtimeSinceStartup;
		
		yield break;
	}
	
	
	private void FadeStateChanged()
	{
			if(m_loadingIndication != LoadingIndication.FULL_LOAD )
			{
				StartCoroutine(CheatLoadingBar());		
			}

			switch(m_loadingIndication)
			{
				
			case SceneManager.LoadingIndication.MENU_LOAD:
				
				break;
				
			case SceneManager.LoadingIndication.QUICK_LOAD:
				
			
				break;
			case SceneManager.LoadingIndication.FULL_LOAD:
				
				break;
			}
			
			switch(m_loadingMode)
			{				
			case LoadingMode.DONT_UNLOAD_CURRENT:				
				LoadScene();			
			break;				
			case LoadingMode.UNLOAD_CURRENT:				
				m_isLoadingScene = true;
				Application.LoadLevelAsync(_loadingSceneName);				
			break;				
			}
	}
	
	private void LoadScene()
	{	
		int levelIndex = 0;
		
		AsyncOperation ao = Application.LoadLevelAsync(m_mainScene);
    
		
		if(ao != null)
		{
			m_loadStatus[levelIndex++] = ao;
			
			foreach(string level in m_adictiveScene)
			{
				m_loadStatus[levelIndex++] = Application.LoadLevelAdditiveAsync(level);
			}			
		}
		
		m_checkSceneLoaded = true;
	}
	
	
 	IEnumerator HandleLoadingCamera(float time)
    {
        yield return null;
        yield return null;
        yield return new WaitForSeconds(time);
		
        UIEventManager.Instance.TriggerUIEvent(UIEventType.LoadingComplete, null);
        RaiseEvent(EventTypeEnum.SceneLoaded.ToString(), null);
        yield return null;

        if (GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_BATTLE) {
    //        Debug.Log ("SendLoadingCompleteStatusToSever==========");
            NetServiceManager.Instance.EctypeService.SendTeamateRequestEnterEctype ();
			//Debug.Log("IsTeamExist()=="+TeamManager.Instance.IsTeamExist()+" isTeamBattleMark="+GameManager.Instance.isTeamBattleMark);
			if (TeamManager.Instance.IsTeamExist())//GameManager.Instance.isTeamBattleMark) 
			{
                UIEventManager.Instance.TriggerUIEvent (UIEventType.LoadingStartDownTime, null);
            }
        }
		/*
		yield return new WaitForSeconds(time);
		 
		GameObject loadingCamera =  GameObject.FindGameObjectWithTag("LoadingCamera");
		 if(loadingCamera != null)
		{
			Camera  camera = loadingCamera.GetComponent<Camera>();
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.cullingMask = 0;
		}
		
		yield return new WaitForSeconds(time);
		DestroyLoadingObject();	
		*/
		yield break;
	}
	
	public void  HideAll()
	{
		DoHide();
	}
	
	public void DoHide()
	{
		StartCoroutine(HandleLoadingCamera(0.1f));
	}

}
