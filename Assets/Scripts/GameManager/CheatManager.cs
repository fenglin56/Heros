using UnityEngine;
using System.Collections;
using System;
using System.Text;

/// <summary>
/// EnterPoint Scene CheatManager
/// </summary>
public class CheatManager : View 
{
    public float HeartRate = 1;  //心跳频率
	// Use this for initialization
	private bool m_showMemFps = false;
	private bool m_showCheat = false;
    private bool m_showProtocalMenu = false;
	private bool m_showGM = false;
	private bool m_showNetWorkDelay = false;
	
	private float m_openCheatsMenuTimer = 0.0f;
	private float m_openCheatsMenuTime = 3.0f;
	
	
	public GameObject MemFpsPrefab;
	private GameObject m_memFpsObj;
	private FPSMemChecker m_fpsMenChecker;
	
	
	public GMConfigDataBase GMConfig;
	
	
	public static CheatManager m_instance;
	
	public static CheatManager Instance
	{
		get { return m_instance; }	
	}
	
	void Awake()
	{
		m_instance = this;
		InvokeRepeating ("HeartFPS", UnityEngine.Random.Range (0, HeartRate), HeartRate);
		//InvokeRepeating("CheckNetWork", 1, 1);
	}
	
	void Start () 
	{
		m_memFpsObj = Instantiate(MemFpsPrefab) as GameObject;
		m_memFpsObj.SetActive(false);
		m_fpsMenChecker = m_memFpsObj.GetComponent<FPSMemChecker>();
        RegisterEventHandler();
		//ShowGM();
	}
    
	void OnDestroy()
	{
		Destroy(m_memFpsObj);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//ProcessCheatMenuVisibility();
        ProcessProtocalMenuVisibility();



        if (Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.RightAlt))
        {
            LostConect();
        }
	}
	#region 网络状态判定
	//只有登录后方可判定
	public bool isLogined = false;
	public SERVICE_CODE netWorkCode = SERVICE_CODE.SUCCESS;
	//账号被踢
	public bool isIDKickedMark = false;
	//网络状态
	void CheckNetWork()
	{
		return;
		if (!isLogined)
			return;
		//NetworkReachability type = Application.internetReachability;
		//if (type == NetworkReachability.NotReachable)
		if(netWorkCode == SERVICE_CODE.ERROR_NOFOUND)
		{
			//没有网络，弹出断网提示，跳转到登录界面//
			//Debug.Log("=================SERVICE_CODE.ERROR_NOFOUND==================");
			isLogined = false;
			LostConect();
		}
		//Debug.Log("=================type=================="+type);
	}
	#endregion
	public int connectDelayTime = 0;
    void HeartFPS()
    {
        if (!HeartFPSManager.Instance.VerifyLoseConnection())
        {
            var index = HeartFPSManager.Instance.MakeHeartFps();
            if (index !=0)
            {
                ushort delay = HeartFPSManager.Instance.GetLastDelay(index);
				netWorkCode = NetServiceManager.Instance.EntityService.SendActionHeartFPS(index,delay);
				//Debug.Log("111111netWorkCode===="+netWorkCode);
				CheckNetWork();
                //NetServiceManager.Instance.EntityService.SendActionHeartFPS(index);
            }
        }
        else
        {
			//Debug.Log("2222222netWorkCode===="+netWorkCode);
			if(isIDKickedMark)
				return;
            RaiseEvent(EventTypeEnum.LostConect.ToString(), null);
            LostConect();
        }
    }

    private void LostConect()
    {        
        GameManager.Instance.ResetConnect();
		UIEventManager.Instance.TriggerUIEvent(UIEventType.OnLostConectEvent,null);
        UI.MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_182"), LanguageTextManager.GetString("IDS_H2_13")
          , () => GameManager.Instance.GotoState(GameManager.GameState.GAME_STATE_LOGIN, true));
    }
	public void ShowGM()
	{
		
		m_showGM = true;
		m_showCheat = false;
	}
	
	void HideGM()
	{
		
		m_showGM = false;	
	}
	
	void OnGUI()
	{
		if(m_showCheat)
		{
			GUI.BeginGroup (new Rect (0, 0, Screen.width, Screen.height));
			
			GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
			
			GUI.Box (new Rect (0, 0, Screen.width, Screen.height), "", new GUIStyle(GUI.skin.box));
			GUIStyle labelStyle = GUI.skin.GetStyle ("Label");
			labelStyle.alignment = TextAnchor.MiddleCenter;
			
			GUIStyle vSilideStyle = GUI.skin.GetStyle("");

            if(null != BulletFactory.Instance)
            {
            //add by lee
                BulletFactory.Instance.isShowSquareBullet = GUI.Toggle(new Rect(10, 10, 150, 30),
                BulletFactory.Instance.isShowSquareBullet, "Is show SquareBullet");
                BulletFactory.Instance.isShowFanBullet = GUI.Toggle(new Rect(10, 50, 150, 30),
                BulletFactory.Instance.isShowFanBullet, "Is show FanBullet");
            }

			GUILayout.BeginHorizontal ();
			ProcessCloseCheatMenu();
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GameManager.Instance.IsShowCameraAdjust = GUILayout.Toggle(GameManager.Instance.IsShowCameraAdjust
                , "Open Camera Scale", GUILayout.ExpandWidth(true), GUILayout.MinHeight(30));
            GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			
			ProcessDisplayFps();
			
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			ProcessShowNetWorkDelay();
			
			GUILayout.EndHorizontal();
			
			GUILayout.EndArea ();
			GUI.EndGroup ();
		}
		
		if(m_showGM)
		{
			GUILayout.BeginArea (new Rect( 100, 100, 500, 400 ));
			ProcessGM();
			GUILayout.EndArea();
			
		}

        if(m_showProtocalMenu)
        {
            GUILayout.BeginArea (new Rect( 100, 100, 500, 400 ));
            ProcessProtocalMenu();
            GUILayout.EndArea();
        }
		
		if(m_showNetWorkDelay)
		{
			GUILayout.BeginArea (new Rect( Screen.width-200, Screen.height- 80, 200, 80 ));
			GUILayout.Label(HeartFPSManager.Instance.GetSamplerDeltTime().ToString(), GUILayout.ExpandWidth(true), GUILayout.MinHeight(80));
			GUILayout.EndArea();
			
		}
	}
	
	private void ProcessShowNetWorkDelay()
	{
		if (m_showNetWorkDelay) 
		{
			if (GUILayout.Button ("Hide Network Delay", GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true)))
			{
				m_showNetWorkDelay = false;
			}
		}
		else 
		{
			if (GUILayout.Button ("Show Network Delay", GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true))) 
			{
				
				m_showNetWorkDelay = true;
			}				
		}	
	}
	
	
	private Vector2 m_gmScrollPos = Vector2.zero;
	private string[] m_strContents;
	private void ProcessGM()
	{
		GUI.skin.verticalScrollbar.fixedWidth = 80;
		GUI.skin.verticalScrollbarThumb.fixedWidth = 80;
		
		GUIStyle verticleBarStyle = new GUIStyle();
		verticleBarStyle.fixedWidth = 100;
		m_gmScrollPos = GUILayout.BeginScrollView(m_gmScrollPos, false, true,GUILayout.ExpandWidth (true), GUILayout.ExpandWidth (true) );//GUIStyle.none, verticleBarStyle, GUILayout.ExpandWidth (true), GUILayout.ExpandWidth (true) ); //GUILayout.ExpandWidth (true), GUILayout.ExpandWidth (true));
		GUILayout.BeginVertical();
		
		if(GUILayout.Button("Close GM", GUILayout.ExpandWidth (true), GUILayout.MinHeight(80)))
		{
			m_showGM = false;	
		}
		
		if(null == m_strContents)
		{
			m_strContents = new string[GMConfig.m_dataTable.Length];	
			for(int i =0; i < m_strContents.Length; i++)
			{
				m_strContents[i] = "";	
			}
		}
		
		for(int i = 0; i < GMConfig.m_dataTable.Length; i++)
		{
			GMConfigData data = GMConfig.m_dataTable[i];
			
			GUILayout.BeginHorizontal();
			
			
			m_strContents[i] = GUILayout.TextField(m_strContents[i], GUILayout.MinWidth(100), GUILayout.ExpandWidth (true), GUILayout.MinHeight(80));
			GUILayout.Label(data.m_desc, GUILayout.ExpandWidth(true), GUILayout.MinHeight(80));
			
			if( GUILayout.Button(data.m_name, GUILayout.ExpandWidth (true),GUILayout.MinHeight(80), GUILayout.MinWidth(100)) )
			{
				
				SMsgInteractCOMMON_CS msgInteract;
                msgInteract.dwNPCID = NpcTalk_v2.Instance.m_sMsgInteractCOMMONData.sMsgInteractCOMMON_SC.dwNPCID;
				msgInteract.byOperateType = 3;
				msgInteract.dwParam1 = (uint)(data.m_gmType);
				msgInteract.dwParam2 = 0;
				msgInteract.byIsContext = 1;
				
				SMsgInteractCOMMONContext_CS msgContext;
				msgContext.szContext= new byte[32]; 
				Encoding.Default.GetBytes(m_strContents[i]).CopyTo(msgContext.szContext, 0);
				
				NetServiceManager.Instance.InteractService.SendInteractCOMMON(msgInteract, msgContext);
				
				
				//TraceUtil.Log("sendGM:" + data.m_name + "    " +  m_strContents[i]);
			}
			
			
			GUILayout.EndHorizontal();
			
		}
		
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
	}

    private Vector2 m_pmScrollPos = Vector2.zero;

    string [] protocalMsg;
    private void ProcessProtocalMenu()
    {
        return;
        if(null == protocalMsg)
        {
            protocalMsg = new string[128];    
            for(int i =0; i < 128; i++)
            {
                protocalMsg[i] = "";  
            }
        }


        GUI.skin.verticalScrollbar.fixedWidth = 80;
        GUI.skin.verticalScrollbarThumb.fixedWidth = 80;
        
        GUIStyle verticleBarStyle = new GUIStyle();
        verticleBarStyle.fixedWidth = 100;
        m_gmScrollPos = GUILayout.BeginScrollView(m_gmScrollPos, false, true,GUILayout.ExpandWidth (true), GUILayout.ExpandWidth (true) );//GUIStyle.none, verticleBarStyle, GUILayout.ExpandWidth (true), GUILayout.ExpandWidth (true) ); //GUILayout.ExpandWidth (true), GUILayout.ExpandWidth (true));
        GUILayout.BeginVertical();
        
        if(GUILayout.Button("Close PM", GUILayout.ExpandWidth (true), GUILayout.MinHeight(80)))
        {
            m_showProtocalMenu = false;   
        }
   
        int index = 0;
        //发送排行列表请求
        GUILayout.BeginHorizontal();
        protocalMsg[index] = GUILayout.TextField(protocalMsg[index], GUILayout.MinWidth(100), GUILayout.ExpandWidth (true), GUILayout.MinHeight(80));
        if( GUILayout.Button("发送排行列表请求", GUILayout.ExpandWidth (true),GUILayout.MinHeight(80), GUILayout.MinWidth(100)) )
        {
            string[] splitMsg = protocalMsg[index].Split('+');
            if(splitMsg.Length == 2)
            {
                SMsgInteract_RankingList_CS sMsgInteract_RankingList_CS = new SMsgInteract_RankingList_CS();
                sMsgInteract_RankingList_CS.byRankingType = Convert.ToByte(splitMsg[0]);
                sMsgInteract_RankingList_CS.byIndex = Convert.ToByte(splitMsg[1]);
                NetServiceManager.Instance.InteractService.SendSMsgInteract_RankingList_CS(sMsgInteract_RankingList_CS);
            }
        }
        index++;
        GUILayout.EndHorizontal();


        //发送玩家详细排行信息
        GUILayout.BeginHorizontal();
        protocalMsg[index] = GUILayout.TextField(protocalMsg[index], GUILayout.MinWidth(100), GUILayout.ExpandWidth (true), GUILayout.MinHeight(80));
        if( GUILayout.Button("发送玩家详细排行信息", GUILayout.ExpandWidth (true),GUILayout.MinHeight(80), GUILayout.MinWidth(100)) )
        {
            string[] splitMsg = protocalMsg[index].Split('+');
            if(splitMsg.Length == 3)
            {
                SMsgInteract_GetPlayerRanking_CS sMsgInteract_GetPlayerRanking_CS = new SMsgInteract_GetPlayerRanking_CS();
                sMsgInteract_GetPlayerRanking_CS.byRankingType = Convert.ToByte(splitMsg[0]);
                sMsgInteract_GetPlayerRanking_CS.dwActorID = Convert.ToUInt32(splitMsg[1]);
                sMsgInteract_GetPlayerRanking_CS.dwRankActorID = Convert.ToUInt32(splitMsg[2]);
                NetServiceManager.Instance.InteractService.SendSMsgInteract_GetPlayerRanking_CS(sMsgInteract_GetPlayerRanking_CS);
            }
        }
        index++;
        GUILayout.EndHorizontal();


        //发送玩家竞拍UI请求
        GUILayout.BeginHorizontal();
        protocalMsg[index] = GUILayout.TextField(protocalMsg[index], GUILayout.MinWidth(100), GUILayout.ExpandWidth (true), GUILayout.MinHeight(80));
        if( GUILayout.Button("发送玩家竞拍UI请求", GUILayout.ExpandWidth (true),GUILayout.MinHeight(80), GUILayout.MinWidth(100)) )
        {

            NetServiceManager.Instance.TradeService.SendMsg_Trade_Auction_UI();
        }
        index++;
        GUILayout.EndHorizontal();

        //发送玩家竞拍请求
        GUILayout.BeginHorizontal();
        protocalMsg[index] = GUILayout.TextField(protocalMsg[index], GUILayout.MinWidth(100), GUILayout.ExpandWidth (true), GUILayout.MinHeight(80));
        if( GUILayout.Button("发送玩家竞拍请求", GUILayout.ExpandWidth (true),GUILayout.MinHeight(80), GUILayout.MinWidth(100)) )
        {
            string[] splitMsg = protocalMsg[index].Split('+');
            if(splitMsg.Length == 3)
            {
                SAuctionGoods_CS sAuctionGoods_CS = new SAuctionGoods_CS();
                sAuctionGoods_CS.dwActorID = Convert.ToUInt32(splitMsg[0]);
                sAuctionGoods_CS.byIndex = Convert.ToByte(splitMsg[1]);
                sAuctionGoods_CS.dwAuctionMoney = Convert.ToUInt32(splitMsg[2]);
                NetServiceManager.Instance.TradeService.SendSAuctionGoods_CS(sAuctionGoods_CS);
            }
        }
        index++;
        GUILayout.EndHorizontal();



        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
	
	private void ProcessDisplayFps ()
	{
		if (m_showMemFps) 
		{
			if (GUILayout.Button ("Display Mem&Fps:   ON", GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true)))
			{
				m_memFpsObj.SetActive(false);
				m_showMemFps = false;
			}
		}
		else 
		{
			if (GUILayout.Button ("Display Mem&Fps: OFF", GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true))) 
			{
				m_memFpsObj.SetActive(true);
				m_showMemFps = true;
			}				
		}	
		
		if(GUILayout.Button("Reset low fps", GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true)) )
		{
			m_fpsMenChecker.ResetLowMem();
		}
	}
	
	private void ProcessCheatMenuVisibility ()
	{
		if (!m_showCheat) 
		{
			#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
			
			if (Input.GetMouseButton (0)) 
			{
				Vector3 inputPos = Input.mousePosition;
				inputPos.y = Screen.height - inputPos.y;
				
			#else
				
			if( (Input.touches.Length > 0) &&
				((Input.GetTouch(0).phase != TouchPhase.Ended) || 
				 (Input.GetTouch(0).phase != TouchPhase.Canceled)) )
			{
				Vector3 inputPos = Input.GetTouch(0).position;
				inputPos.y = Screen.height - inputPos.y;
						
			#endif
					
				float yPos = 0.0f;
				float xPos = Screen.width - 100;
				Rect rect = new Rect (xPos, yPos, 100, 100);
				if ((inputPos.x > rect.x) &&
			    (inputPos.x < rect.x + rect.width) &&
			    (inputPos.y > rect.y) &&
			    (inputPos.y < rect.y + rect.height)) 
				{
					m_openCheatsMenuTimer += Time.deltaTime;		
					
					if (m_openCheatsMenuTimer > m_openCheatsMenuTime) 
					{
						m_openCheatsMenuTimer = 0.0f;	
						
						m_showCheat = true;
						m_showGM = false;
					}							
				}
			} 
			else 
			{
				m_openCheatsMenuTimer = 0.0f;	
			}
		}
	}

        private void ProcessProtocalMenuVisibility ()
        {
            return;
            if (!m_showProtocalMenu) 
            {
                #if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
                
                if (Input.GetMouseButton (0)) 
                {
                    Vector3 inputPos = Input.mousePosition;
                    inputPos.y = Screen.height - inputPos.y;
                    
                    #else
                    
                    if( (Input.touches.Length > 0) &&
                       ((Input.GetTouch(0).phase != TouchPhase.Ended) || 
                     (Input.GetTouch(0).phase != TouchPhase.Canceled)) )
                    {
                        Vector3 inputPos = Input.GetTouch(0).position;
                        inputPos.y = Screen.height - inputPos.y;
                        
                        #endif
                        
                        float yPos = Screen.height - 100;
                        float xPos = Screen.width - 100;
                        Rect rect = new Rect (xPos, yPos, 100, 100);
                        if ((inputPos.x > rect.x) &&
                            (inputPos.x < rect.x + rect.width) &&
                            (inputPos.y > rect.y) &&
                            (inputPos.y < rect.y + rect.height)) 
                        {
                            m_openCheatsMenuTimer += Time.deltaTime;        
                            
                            if (m_openCheatsMenuTimer > m_openCheatsMenuTime) 
                            {
                                m_openCheatsMenuTimer = 0.0f;   

                                m_showProtocalMenu = true;
                                m_showCheat = false;
                                m_showGM = false;
                            }                           
                        }
                    } 
                    else 
                    {
                        m_openCheatsMenuTimer = 0.0f;   
                    }
                }       
            }
		
	private void ProcessCloseCheatMenu ()
	{
		GUI.color = Color.green;
		
		if (GUILayout.Button ("Close Cheats", GUILayout.ExpandWidth (true), GUILayout.MinHeight(70))) 
		{
			m_showCheat = false;
		}
		GUI.color = Color.white;
	}

    protected override void RegisterEventHandler()
    {
    }
}