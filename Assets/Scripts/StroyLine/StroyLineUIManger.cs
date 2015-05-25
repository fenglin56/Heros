using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NetworkCommon;

public class StroyLineUIManger : View {
    public LocalButtonCallBack DialogButton;
	public GameObject dialogAnchorParent;
    public List<GameObject> lineDialogList;
    //public GameObject TwoLineDialog;
   // public GameObject ThreeLineDialog;
    //public GameObject SignText;
    public GameObject CameraMask;
    public Camera UICamera;
    public LocalButtonCallBack SkipButton;

    private GameObject m_dialogPanel;
    //private GameObject m_signText;

    private StroyDialogConfigData[] m_dialogData;
    private bool m_isAction = false;
    //private bool m_startCameraMask = false;
    //private bool m_endCameraMask = false;
    private StroyDialogConfigData m_curDialogData;
    //private StroyNpcBehaviour m_curNpcBehaviour;
    private GameObject m_cameraEffect;



    private static StroyLineUIManger m_instance;
    public static StroyLineUIManger Instance
    {
        get { return m_instance; }
    }

    void FixedUpdate()
    {
        if (StroyLineDataManager.Instance.IsStartCameraMask)
            ShowStartCameraMask();
        if (StroyLineDataManager.Instance.IsEndCameraMask)
            ShowEndCameraMask();

        if (m_isAction)
        {
            if (m_dialogData.Length <= 0)
            {
                StroyUIEndHandle();
            }
            else
            {
                ShowDialogPanel(m_dialogData[0]);
            }

            m_isAction = false;
        }
    }

    //void ShowDialogUI()
    //{
    //    if (m_dialogData.Length <= 0)
    //    {
    //        StroyUIEndHandle();
    //    }
    //    else
    //    {
    //        ShowDialogPanel(m_dialogData[0]);
    //    }
    //}

    private int m_curDialogIndex = 0;

    void Awake()
    {
		DialogButton.SetCallBackFuntion(SkipBtnDialogHandle);
        SkipButton.SetCallBackFuntion(SkipButtonHandle);
    }

    void Start()
    {
        m_instance = this;
        m_curDialogIndex = 0;
		ResponseHandleInvoker.Instance.IsPaused = false;
        this.RegisterEventHandler();
        StroyLineManager.Instance.AddCameraEffectDelegate(CameraEffectAction);
//		//jamfing 进/出游戏剧情//        
//		if(StroyLineDataManager.Instance.GetStroyType != StroyLineType.None)
//			StroyLineManager.Instance.StartNextCameraGroup();
			
        StroyLineManager.Instance.AddDialogDelegate(UpdateDialogAction);
        
    }

    void SkipButtonHandle(object obj)
    {
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Story_Skip");  
        StroyLineDataManager.Instance.ChangeScene();
    }
	void SkipBtnDialogHandle(object obj)
	{
		DialogButtonHandle (null);
	}
    /// <summary>
    /// 单击对白效果
    /// </summary>
    /// <param name="obj"></param>
    void DialogButtonHandle(object obj)
    {

        if (m_dialogData == null)
        {
            return;
        }

        if (m_dialogPanel != null)
        {
			DestroyImmediate(m_dialogPanel);
			m_dialogPanel = null;
        }

        m_curDialogIndex += 1;

        if (m_curDialogIndex < m_dialogData.Length)
        {
            if (m_dialogData[m_curDialogIndex] != null)
            {
                ShowDialogPanel(m_dialogData[m_curDialogIndex]);
            }
            else
            {
                DialogButtonHandle(null);
                TraceUtil.Log("当前对白为空");
                //m_dialogText = "";
            }
        }
        else
        {
            TraceUtil.Log("当前对白执行完毕!!");
            StroyUIEndHandle();
        }
    }
	private bool isChangeCameraMark = false;
	private UISprite cameraMaskSprite ;
    public void SetCameraMaskAlphaVal(bool isStart)
	{
		if (cameraMaskSprite == null) {
			cameraMaskSprite = CameraMask.GetComponent<UISprite>();
		}
		//只要有来的就马上更新
		/*if (!isChangeCameraMark)
			return;
		isChangeCameraMark = false;*/
		if (isStart) {
			cameraMaskSprite.alpha = 1;
			StroyLineDataManager.Instance.IsEndCameraMask = false;
		} else {
			cameraMaskSprite.alpha = 0;
			StroyLineDataManager.Instance.IsStartCameraMask = false;
		}
	}
	public void ShowStartCameraMask()
    {
		if (cameraMaskSprite == null) {
			cameraMaskSprite = CameraMask.GetComponent<UISprite>();
		}
		cameraMaskSprite.enabled = true;
		cameraMaskSprite.alpha -= Time.deltaTime*0.33f;

		if (cameraMaskSprite.alpha <= 0)
        {
			isChangeCameraMark = true;
			cameraMaskSprite.enabled = false;
            StroyLineDataManager.Instance.IsStartCameraMask = false;
            //cameraMask.alpha = 1;
        }
    }
    void ShowEndCameraMask()
    {
		if (cameraMaskSprite == null) {
			cameraMaskSprite = CameraMask.GetComponent<UISprite>();
		}
		cameraMaskSprite.enabled = true;
		cameraMaskSprite.alpha += Time.deltaTime*0.33f;
		if (cameraMaskSprite.alpha >= 1)
        {
			isChangeCameraMark = true;
			//让它一直黑下去//
			if(!StroyLineManager.Instance.IsFinishCameraGroup())
			{
				cameraMaskSprite.enabled = false;
			}
            StroyLineDataManager.Instance.IsEndCameraMask = false;
            //cameraMask.alpha = 1;
        }
    }

    private void CameraEffectAction(GameObject effectGo)
    {
        if (effectGo == null)
            return;

        if (m_cameraEffect != null)
        {
            DestroyImmediate(m_cameraEffect);
        }

        m_cameraEffect = Instantiate(effectGo) as GameObject;
        m_cameraEffect.transform.parent = this.transform;
        m_cameraEffect.transform.localScale = Vector3.one;
    }

    public void UpdateDialogAction(List<StroyDialogConfigData> dialogData)
    {
        m_dialogData = new StroyDialogConfigData[dialogData.Count];

        for (int i = 0; i < dialogData.Count; i++)
        {
            m_dialogData[i] = new StroyDialogConfigData();

            if (dialogData == null)
            {
                m_dialogData[i] = null;
            }
            else
            {
                m_dialogData[i] = dialogData[i];
            }
        }

        m_isAction = true;
        //ShowDialogUI();
        if (dialogData.Count > 0)
        {
            DialogButton.SetButtonActive(true);
            m_curDialogIndex = 0;
        }
        
    }

    public void ShowDialogPanel(StroyDialogConfigData item)
    {
        m_curDialogData = item;
        
        /*if (m_signText == null)
        {
            m_signText = GameObject.Instantiate(SignText) as GameObject;
            m_signText.transform.parent = this.transform;
            m_signText.transform.localScale = Vector3.one;
        }*/

		/*if (StroyLineDataManager.Instance.GetNpcList.ContainsKey(m_curDialogData._NpcID))
			m_curNpcBehaviour = StroyLineDataManager.Instance.GetNpcList[m_curDialogData._NpcID];
        else
        {
            TraceUtil.Log("指定的NpcID不存在");
            m_curNpcBehaviour = null;
        }*/

        if (m_dialogPanel != null)
        {
            DestroyImmediate(m_dialogPanel);
        }
		m_dialogPanel = GameObject.Instantiate(lineDialogList[(int)item._DialogType-1],this.m_curDialogData._ViewOffset, Quaternion.identity) as GameObject;
        if (m_dialogPanel != null)
        {
			m_dialogPanel.transform.parent = dialogAnchorParent.transform;
			m_dialogPanel.transform.localPosition = this.m_curDialogData._ViewOffset;
            m_dialogPanel.transform.localScale = Vector3.one;
			//jamfing
            //m_dialogPanel.GetComponentsInChildren<UIAnchor>().ApplyAllItem(P => P.uiCamera = UICamera);
			TalkIdConfigData talkIdConfigData = new TalkIdConfigData();
			talkIdConfigData.DialogPrefab = (DialogBoxType)m_curDialogData._DialogType;
			talkIdConfigData.TalkType = (StoryTallType)m_curDialogData._NpcOrPlayer;
			if(talkIdConfigData.TalkType == StoryTallType.NPC)
			{
				talkIdConfigData.NPCName = m_curDialogData._NpcName;
				talkIdConfigData.TalkHead = m_curDialogData.npcIconPrefab;
			}
			else
			{
				talkIdConfigData.NPCName = PlayerManager.Instance.FindHeroDataModel().Name;
			}
			talkIdConfigData.TalkText = m_curDialogData._Content;
			talkIdConfigData.TalkID = 0;
			talkIdConfigData.TextPos = "0";
			talkIdConfigData.isTaskTalkMark = false;
			StoryDialogBehaviour storyPanelBehaviour = m_dialogPanel.GetComponent<StoryDialogBehaviour>();
			storyPanelBehaviour.StoryGuideFinishAct = () =>
			{
				SoundManager.Instance.PlaySoundEffect("Sound_Button_TaskStory_Next");  
				DialogButtonHandle(null);
			};
			storyPanelBehaviour.Init(talkIdConfigData);
			//m_dialogPanel.GetComponent<StroyLinePanel>().InitDialogPanel(item);
        }
    }

    void Update()
    {
		//???作用？？？
        if (m_dialogPanel != null)
        {
			m_dialogPanel.transform.localPosition = this.m_curDialogData._ViewOffset;//GetPopupPos(UICamera);
        }
    }


    /*public Vector3 GetPopupPos(Camera uiCamera)
    {
        if (m_curNpcBehaviour != null)
        {
            //TraceUtil.Log("###m_curNpcBehaviour.transform.position" + m_curNpcBehaviour.transform.position);
            var worldPos = Camera.main.WorldToViewportPoint(m_curNpcBehaviour.transform.position);
            //TraceUtil.Log("###worldPos" + worldPos);
            worldPos.x = worldPos.x + this.m_curDialogData._ViewOffset.x;
            worldPos.y = worldPos.y + this.m_curDialogData._ViewOffset.y;

            //TraceUtil.Log("@@@@@@@_ViewOffset" + this.m_dialogData[m_curDialogIndex]._ViewOffset);
            //TraceUtil.Log("@@@@@@@worldPos" + worldPos);

            var uipos = uiCamera.ViewportToWorldPoint(worldPos);
            //TraceUtil.Log("$$$$$$$$$$uipos" + uipos);
            uipos.z = 1;

            return uipos;
        }

        return Vector3.zero;
    }*/

    void OnDestroy()
    {
        if (StroyLineManager.Instance != null)
        {
            StroyLineManager.Instance.RemoveDialogDelegate(UpdateDialogAction);
            StroyLineManager.Instance.RemoveCameraEffectDelegate(CameraEffectAction);
        }
        m_instance = null;
    }


    protected override void RegisterEventHandler()
    {
        return;
    }

    void StroyUIEndHandle()
    {
        /*if (m_signText != null)
            DestroyImmediate(m_signText);*/

        if (m_dialogPanel != null)
            DestroyImmediate(m_dialogPanel);

        DialogButton.SetButtonActive(false);
        //CameraMask.GetComponent<UISprite>().alpha = 1;
        StroyLineManager.Instance.StroyUIHandle();
    }
}
