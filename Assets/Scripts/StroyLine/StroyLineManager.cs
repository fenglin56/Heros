using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StroyActionType
{
    DoAction,
    PlayViewEffect,
    PlaySoundEffect,
    MoveCamera,
    Dialog,
}

[System.Serializable]
public class StroyLineEvent
{
    public float EventTimeAfterLaunch;
    public StroyActionType Type;
    public int ParentIndex;
    public int Index;
    private bool m_handled = false;
    public bool Handled
    {
        set { m_handled = value; }
        get { return m_handled; }
    }
}

public class StroyAction
{
    public int NpcID;
    public int RoleType;
    public int RoleResID;
    public StroyActionConfigData[] ActionList;
}

public class StroyLineManager : MonoBehaviour {

    public delegate void StroyCameraDelegate(List<StroyCameraConfigData> cameraData);
    public delegate void StroyDialogDelegate(List<StroyDialogConfigData> dialogData);
    public delegate void CameraEffectDelegate(GameObject cameraGroupEffect);

    private StroyCameraDelegate onCameraPlay;
    private StroyDialogDelegate onDialogPlay;
    private CameraEffectDelegate onCameraEffectPlay;

    public void AddCameraDelegate(StroyCameraDelegate del)
    {
        onCameraPlay += del;
    }

    public void RemoveCameraDelegate(StroyCameraDelegate del)
    {
        onCameraPlay -= del;
    }

    public void AddCameraEffectDelegate(CameraEffectDelegate del)
    {
        onCameraEffectPlay += del;
    }

    public void RemoveCameraEffectDelegate(CameraEffectDelegate del)
    {
        onCameraEffectPlay -= del;
    }

    public void AddDialogDelegate(StroyDialogDelegate del)
    {
        onDialogPlay += del;
    }

    public void RemoveDialogDelegate(StroyDialogDelegate del)
    {
        onDialogPlay -= del;
    }

    private Dictionary<int, StroyAction> m_stroyActionIDList = new Dictionary<int, StroyAction>();
    private List<StroyCameraConfigData> m_stroyCameraList = new List<StroyCameraConfigData>();
    private List<StroyDialogConfigData> m_stroyDialogList = new List<StroyDialogConfigData>();

    private List<StroyLineEvent> events = new List<StroyLineEvent>();
    private Dictionary<StroyActionType, StroyLineEvent> m_eventList = new Dictionary<StroyActionType, StroyLineEvent>();
	private Dictionary<int,string> sfxList = new Dictionary<int,string>();

    private float m_elapseTime;
    private CameraGroup m_cameraGroupComponent;
    private bool m_isAction = false;
    private int m_cameraGroupIndex = 0;
    private float m_actionMostTime = 0;
    private CameraGroupConfigData m_cameraGroupData;

    private static StroyLineManager m_instance;
    public static StroyLineManager Instance
    {
        get { return m_instance; }
    }

    public void Init(int cameraGroupID)
    {
        m_cameraGroupData = StroyLineConfigManager.Instance.GetCameraGroupConfig[cameraGroupID];
        StartCameraMask();

        if (m_cameraGroupData == null)
        {
            Debug.LogWarning("stroyline" + cameraGroupID);
            return;
        }

        if (onCameraEffectPlay != null)
        {
            onCameraEffectPlay(m_cameraGroupData._EffectGo);
        }

        int actionListCount = m_cameraGroupData._ActionList.Length;
        int cameraClipCount = m_cameraGroupData._CameraID.Count;
        int dialogCount = m_cameraGroupData._DialogGroupID.Length;

		m_stroyActionIDList.Clear();
		sfxList.Clear();
        for (int i = 0; i < actionListCount; i++)
        {
            var npcAction = m_cameraGroupData._ActionList[i];
 
            int stroyActionCount = npcAction.AnimID.Count;

            m_stroyActionIDList.Add(i, new StroyAction());
            m_stroyActionIDList[i].NpcID = npcAction.NpcID;
            m_stroyActionIDList[i].RoleType = npcAction.RoleType;
            m_stroyActionIDList[i].RoleResID = npcAction.RoleResID;
            m_stroyActionIDList[i].ActionList = new StroyActionConfigData[stroyActionCount];

			StroyLineEvent evt = new StroyLineEvent();

            evt.Type = StroyActionType.DoAction;
            evt.ParentIndex = i;
            evt.EventTimeAfterLaunch = 0f;
            events.Add(evt);
			
            for (int k = 0; k < stroyActionCount; k++)
            {    
                if (!StroyLineConfigManager.Instance.GetStroyActionConfig.ContainsKey(npcAction.AnimID[k]))
                {
					m_stroyActionIDList[i].ActionList[k] = null;
                    continue;
                }
                m_stroyActionIDList[i].ActionList[k] = StroyLineConfigManager.Instance.GetStroyActionConfig[npcAction.AnimID[k]];
                if (m_stroyActionIDList[i].ActionList[k]._SoundName != "0")
                {
					sfxList.Add(k,m_stroyActionIDList[i].ActionList[k]._SoundName);
                    StroyLineEvent sfxEvt = new StroyLineEvent();
                    sfxEvt.Type = StroyActionType.PlaySoundEffect;
                    sfxEvt.ParentIndex = i;
                    sfxEvt.Index = k;
                    sfxEvt.EventTimeAfterLaunch =  m_stroyActionIDList[i].ActionList[k]._SoundTime/1000;
                    events.Add(sfxEvt);
                }
				
				
                if (m_stroyActionIDList[i].ActionList[k]._EffectGo != null)
                {
                    StroyLineEvent efEvt = new StroyLineEvent();

                    efEvt.Type = StroyActionType.PlayViewEffect;
                    efEvt.ParentIndex = i;
                    efEvt.Index = k;
                    efEvt.EventTimeAfterLaunch =m_stroyActionIDList[i].ActionList[k]._EffectStartTime/1000;
                    events.Add(efEvt);
                }
            }
        }
        m_stroyCameraList = new List<StroyCameraConfigData>();
        for (int i = 0; i < cameraClipCount; i++)
        {
            int cameraID = m_cameraGroupData._CameraID[i];

            if (StroyLineConfigManager.Instance.GetCameraConfig.ContainsKey(cameraID))
                m_stroyCameraList.Add(StroyLineConfigManager.Instance.GetCameraConfig[cameraID]);
        }
		
		m_eventList.Clear();
        if (m_stroyCameraList.Count > 0)
        {
            StroyLineEvent cameraEvt = new StroyLineEvent();
            cameraEvt.Type = StroyActionType.MoveCamera;
            cameraEvt.EventTimeAfterLaunch = 0;
            m_eventList.Add(cameraEvt.Type, cameraEvt);
        }

        m_stroyDialogList.Clear();
        for (int i = 0; i < dialogCount; i++)
        {
			int dialogTextID = m_cameraGroupData._DialogGroupID[i];
            if (StroyLineConfigManager.Instance.GetStroyDialogConfig.ContainsKey(dialogTextID))
                m_stroyDialogList.Add(StroyLineConfigManager.Instance.GetStroyDialogConfig[dialogTextID]);
        }

        //if (m_stroyDialogList.Count > 0)
        {
            StroyLineEvent dialogEvt = new StroyLineEvent();
            dialogEvt.Type = StroyActionType.Dialog;
            dialogEvt.EventTimeAfterLaunch = 0;
            m_eventList.Add(dialogEvt.Type, dialogEvt);
        }
    }

    void Awake()
    {
        m_instance = this;
    }


	void Start () {
        m_cameraGroupComponent = new CameraGroup();
        //SetHeroActive(false);
	}

    void SetHeroActive(bool flag)
    {
        var hero = PlayerManager.Instance.FindHero();
        if (hero == null)
            return;

        hero.SetActive(flag);
        hero.GetComponent<PlayerBehaviour>().enabled = flag;
        //MedalManager.Instance.SetHeroMedal(flag);
    }
	//获取当前是否是最后镜头
	public bool IsFinishCameraGroup()
	{
		if (m_cameraGroupIndex >= StroyLineDataManager.Instance.GetCameraGroupID.Count)
			return true;
		return false;
	}
    /// <summary>
    /// 执行下一段镜头组
    /// </summary>
    public void StartNextCameraGroup()
    {

        if (m_cameraGroupIndex >= StroyLineDataManager.Instance.GetCameraGroupID.Count)
        {
            StroyLineDataManager.Instance.ChangeScene();
            SetSaveName = "这是最后一组镜头.....";
        }
        else
        {
            int curCameraGroupID = StroyLineDataManager.Instance.GetCameraGroupID[m_cameraGroupIndex];
            StroyLineDataManager.Instance.SetCameraClipID = curCameraGroupID;
            if (StroyLineConfigManager.Instance.GetCameraGroupConfig.ContainsKey(curCameraGroupID))
            {
                Init(curCameraGroupID);
                m_cameraGroupIndex += 1;

                
                StartAction();
            }
            else
            {
                StroyLineDataManager.Instance.ChangeScene();
            }
        }   
    }
	
	// Update is called once per frame
	void Update () {

        float timeDelta = Time.deltaTime;
        
        if (m_isAction)
        {
            m_elapseTime += timeDelta;

            foreach (StroyLineEvent evt in events)
            {
                if (!evt.Handled && evt.EventTimeAfterLaunch <= m_elapseTime)
                {
                    evt.Handled = true;
                    LaunchAction(evt);
                }
            }

            foreach (KeyValuePair<StroyActionType, StroyLineEvent> evt in m_eventList)
            {
                if (evt.Value.Type == StroyActionType.Dialog)
                {
                    evt.Value.EventTimeAfterLaunch = m_actionMostTime;
                }

                if (!evt.Value.Handled && evt.Value.EventTimeAfterLaunch <= m_elapseTime)
                {
                    evt.Value.Handled = true;
                    LaunchAction(evt.Value);
                }
            }
        }
	}

    public void ActionMostTime(float time)
    {
        if (m_actionMostTime < time)
            m_actionMostTime = time;
    }

    private void StartAction()
    {
        ResetEvents();
        m_elapseTime = 0;
        m_isAction = true;
    }

    public void ClearList()
    {
        m_stroyActionIDList.Clear();
        m_eventList.Clear();
        sfxList.Clear();
        events.Clear();
        m_stroyCameraList.Clear();
        m_stroyDialogList.Clear();
    }

    private void StartCameraMask()
    {
        if (m_cameraGroupData._IsCameraStartMask)
        {
            StroyLineDataManager.Instance.IsStartCameraMask = true;
			if(StroyLineUIManger.Instance != null)
				StroyLineUIManger.Instance.SetCameraMaskAlphaVal(true);
        }
    }

    public void EndCameraMask()
    {
        if (m_cameraGroupData._IsCameraEndMask)
        {
            StroyLineDataManager.Instance.IsEndCameraMask = true;
			if(StroyLineUIManger.Instance != null)
				StroyLineUIManger.Instance.SetCameraMaskAlphaVal(false);
        }
    }

    void ResetEvents()
    {
        foreach (StroyLineEvent evt in events)
        {
            evt.Handled = false;
        }
    }

    void LaunchAction(StroyLineEvent stroyEvent)
    {
        switch (stroyEvent.Type)
        {
            case StroyActionType.DoAction:
                PlayeAction(m_stroyActionIDList[stroyEvent.ParentIndex]);
                break;
            case StroyActionType.PlayViewEffect:
                StroyAction stroyaction = m_stroyActionIDList[stroyEvent.ParentIndex];
                PlayActionEffect(stroyaction.ActionList[stroyEvent.Index]._ActionID, stroyaction.NpcID);

                break;
            case StroyActionType.PlaySoundEffect:
                PlaySoundEffect(stroyEvent.Index);
                
                break;
            case StroyActionType.MoveCamera:
                if (null != onCameraPlay)
                {
                    onCameraPlay(m_stroyCameraList);
                }
                break;
            case StroyActionType.Dialog:
                if (null != onDialogPlay)
                {
                    onDialogPlay(m_stroyDialogList);
                }
                else if(IsShowEditorUI)
                {
                    m_dialogText = string.Empty;
                    for (int i = 0; i < m_stroyDialogList.Count; i++)
                        m_dialogText += string.Format("{0}\n",LanguageTextManager.GetString(m_stroyDialogList[i]._Content));
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ------------------------------------------------Start
    /// ########################################
    /// 供剧情编辑器用
    /// </summary>
    /// 
    private string m_dialogText = string.Empty;
    private string m_saveInfo = string.Empty;
    void OnGUI()
    {
        if (IsShowEditorUI)
        {
            StroyLineEditor.StroyEditorCommand.ReadData(new Rect(10, Screen.height / 2 - 50, 150, 20), ref m_cameraGroupIndex);

            if (GUI.Button(new Rect(10, Screen.height / 2 - 20, 150, 40), "Play Next Group"))
            {
                StroyUIHandle();
                Invoke("ClearDialogInfo", 0f);
            }
            if (SetSaveName.Length > 0)
            {
                GUI.Label(new Rect(10, Screen.height / 2 + 25, 220, 50), string.Format("{0}", SetSaveName));
                Invoke("ClearSaveInfo", 1.5f);
            }

            GUI.Label(new Rect(10, Screen.height / 2 + 70, 200, 200), m_dialogText);
        }
    }

    void ClearDialogInfo()
    {
        m_dialogText = string.Empty;
    }

    void ClearSaveInfo()
    {
        SetSaveName = string.Empty;
    }

    public void ResetCameraGroup()
    {
        m_cameraGroupIndex = 0;
    }

    public string SetSaveName { set { m_saveInfo = value; } get { return m_saveInfo; } }
    public bool IsShowEditorUI { set; private get; }
    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////
    /// -------------------------------------------------------end
    /// </summary>

    
    private void PlayeAction(StroyAction stroyAction)
    {
        for (int i = 0; i < stroyAction.ActionList.Length; i++)
        {
            if (stroyAction.ActionList[i] == null)
            {
                return;
            }
            
            if (stroyAction.ActionList[i]._ActionType == 1)
            {
                m_cameraGroupComponent.CreateRoleData(stroyAction, stroyAction.ActionList[i]._ActionID);
            }
			else
			{
				m_cameraGroupComponent.SetRolePosition(stroyAction, stroyAction.ActionList[i]._ActionID);
			}
        }
		
        m_cameraGroupComponent.Act(stroyAction);
    }

    private void PlaySoundEffect(int p)
    {
        SoundManager.Instance.PlaySoundEffect(sfxList[p]);
    }

    private void PlayActionEffect(int p, int npcId)
    {
        m_cameraGroupComponent.CreateActionEffect(p, npcId);
    }

    void OnDestroy()
    {
        SetHeroActive(true);
        onCameraPlay = null;
        onDialogPlay = null;
        m_instance = null;

        StroyLineDataManager.Instance.GetNpcList.Clear();
    }

    public void StroyUIHandle()
    {
        //EndCameraMask();
        ClearList();
        StartNextCameraGroup();
        m_actionMostTime = 0f;
    }
}
