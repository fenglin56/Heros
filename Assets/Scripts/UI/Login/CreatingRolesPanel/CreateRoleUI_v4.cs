using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Login;
using UI;

public class CreateRoleUI_v4 : IUIPanel
{

    public enum RoleEventType
    {
        PlayAnim,
        PlaySoundEffect,
        PlayEffect
    }

    public class RoleEvent
    {
        public float EventTimeAfterLaunch;
        public RoleEventType Type;
        public int Param;
        public bool m_handled = false;
    }

    public GameObject CreateRoleRightUI;
    public GameObject CreateRoleLeftUI;
	public GameObject ContinueNextTips;

    public SingleButtonCallBack BackButton;

	private GameObject m_continueNextTips;
    private CreateRolePanelV4 m_createRolePanel;

    public delegate void DoActionDelegate(RoleState state, int vocation);
    public DoActionDelegate OnDoAction;

    private float m_elapseTime = 0;
    private CreateRoleUIData m_selectRoleData = null;
    private Dictionary<int, GameObject> m_heroList = new Dictionary<int, GameObject>();
    private List<RoleEvent> m_eventList = new List<RoleEvent>();
    private List<CreateRoleUIData> m_createRoleData = new List<CreateRoleUIData>();
	private List<GameObject> m_curRoleEffectList = new List<GameObject> ();
    private bool m_onSelect = false;

    private static CreateRoleUI_v4 m_instance;
    public static CreateRoleUI_v4 Instance
    {
        get { return m_instance; }
    }

    void Awake()
    {
        m_instance = this;
        UIEventManager.Instance.RegisterUIEvent(UIEventType.SelectRole, SelectRole);
        
        BackButton.SetCallBackFuntion(OnBackButtonClick);
    }

    void OnBackButtonClick(object obj)
    {
        CancelInvoke("CreateRoleUI");
        if (LoginManager.Instance.NewSSUserLoginRes.lActorNum == 0)
        {
            return;
        }
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        GameDataManager.Instance.ResetData(DataType.ActorSelector, LoginManager.Instance.NewSSUserLoginRes.SSActorInfos);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.SelectRole);
    }

    void Start()
    {
        m_createRoleData = LoginDataManager.Instance.GetCreateRoleUIData;
        InitCreateRoleUI(m_createRoleData);
		m_continueNextTips = CreatObjectToNGUI.InstantiateObj (ContinueNextTips, transform);
    }

    void OnDestroy()
    {
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.SelectRole, SelectRole);
        m_instance = null;
    }

    //public void SetCamera(Transform camera)
    //{
    //    var cameraData = LoginDataManager.Instance.GetCreateRoleUIData.Single(P => P._VocationID == 1);
    //    m_camera = camera;
    //    m_camera.gameObject.SetActive(true);
    //    m_camera.position = cameraData._CameraPosition;
    //    m_camera.LookAt(cameraData._CameraTarget);
    //}

	// Use this for initialization
	private void InitCreateRoleUI (List<CreateRoleUIData> uiData)
    {
        if (m_heroList.Count == 0)
        {
   
         foreach (var item in uiData)
            {
                var hero = GameObject.Instantiate(item._RoleModel, item._RolePosition, Quaternion.identity) as GameObject;
                hero.GetComponent<CreateRoleBehaviour>().SetRoleData = item;
                if (OnDoAction != null)
                {
                    OnDoAction(RoleState.Init, item._VocationID);
                }
                m_heroList.Add(item._VocationID, hero);
            }
        }
	}


    void SelectRole(object obj)
    {

        var vocation = (int)obj;
        BreakRoleEvent();
		
        for (int i = 0; i < m_createRoleData.Count; i++)
        {
            if (m_createRoleData[i]._VocationID == vocation)
                m_selectRoleData = m_createRoleData[i];
        }


        if (m_createRolePanel != null)
        {
            Destroy(m_createRolePanel.gameObject);
            m_createRolePanel = null;
        }

		for (int i = 0; i < m_curRoleEffectList.Count; i++) {
			if(m_curRoleEffectList[i] != null)
				Destroy(m_curRoleEffectList[i]);
		}
		m_curRoleEffectList.Clear ();
		CancelInvoke();
		Invoke ("CreateRoleUI", m_selectRoleData._UIDelayTime);

        m_eventList.Clear();

        GameObject.Destroy(m_continueNextTips);

        for (int i = 0; i < m_selectRoleData._AnimList.Length; i++)
        {
            RoleEvent evt = new RoleEvent();
            evt.Type = RoleEventType.PlayAnim;
            evt.m_handled = true;
            evt.EventTimeAfterLaunch = 0.0f;
            evt.Param = i;
            m_eventList.Add(evt);
        }


        RoleEvent evt1 = new RoleEvent();
        evt1.Type = RoleEventType.PlaySoundEffect;
        evt1.m_handled = true;
        evt1.Param = m_selectRoleData._SoundEffectList.Length == 0 ? 0 : Random.Range(0, m_selectRoleData._SoundEffectList.Length);
        evt1.EventTimeAfterLaunch = 0;
        m_eventList.Add(evt1);

        for (int i = 0; i < m_selectRoleData._EffectDelayTime.Length; i++)
        {
            RoleEvent evt = new RoleEvent();
            evt.Type = RoleEventType.PlayEffect;
            evt.m_handled = true;
            evt.Param = i;
            evt.EventTimeAfterLaunch = m_selectRoleData._EffectDelayTime[i] * 0.001f;
            m_eventList.Add(evt);
        }
        m_onSelect = true;
    }

	void CreateRoleUI()
	{
		if (m_createRolePanel != null)
		{
			Destroy(m_createRolePanel.gameObject);
			m_createRolePanel = null;
		}

        if (m_selectRoleData._VocationID == 1)
			m_createRolePanel = CreatObjectToNGUI.InstantiateObj(CreateRoleRightUI, transform).GetComponent<CreateRolePanelV4>();
		else
			m_createRolePanel = CreatObjectToNGUI.InstantiateObj(CreateRoleLeftUI, transform).GetComponent<CreateRolePanelV4>();
		
		if(m_createRolePanel != null)
			m_createRolePanel.InitData(m_selectRoleData);
	}

    public void BreakRoleEvent()
    {
        if (!m_onSelect)
        {
            return;
        }
        m_elapseTime = 0;
        m_onSelect = false;

        foreach (var item in m_eventList)
        {
            item.m_handled = false;
        }
    }


    void Update()
    {
        if (m_onSelect)
        {
            m_elapseTime += Time.deltaTime;

            foreach (var item in m_eventList)
            {
                if (item.m_handled && item.EventTimeAfterLaunch <= m_elapseTime)
                {
                    item.m_handled = false;
                    LaunchEvent(item);
                }
            }
        }
    }

    void LaunchEvent(RoleEvent evt)
    {
        switch (evt.Type)
        {
            case RoleEventType.PlayAnim:
                PlayAnimation(evt.Param);
                break;
            case RoleEventType.PlaySoundEffect:
                PlaySoundEffect(evt.Param);
                break;
            case RoleEventType.PlayEffect:
                PlayActionEffect(evt.Param);
                break;
            default:
                break;
        }
    }

    void PlayAnimation(int index)
    {
        if (OnDoAction != null)
        {
            OnDoAction(RoleState.Click, m_selectRoleData._VocationID);
        }
    }

    void PlaySoundEffect(int index)
    {
        SoundManager.Instance.PlaySoundEffect(m_selectRoleData._SoundEffectList[index]);
    }

    void PlayActionEffect(int index)
    {
		m_curRoleEffectList.Add( GameObject.Instantiate(m_selectRoleData._EffectList[index], 
		                                                m_selectRoleData._RolePosition, Quaternion.identity) as GameObject);
    }        

    public override void Show()
    {
        if (LoginManager.Instance.NewSSUserLoginRes.lActorNum == 0)
        {
            BackButton.gameObject.SetActive(false);
        }
        else
        {
            BackButton.gameObject.SetActive(true);
        }
        if (OnDoAction != null && m_selectRoleData != null)
        {
            OnDoAction(RoleState.Init, m_selectRoleData._VocationID);
        }

		if (m_continueNextTips != null)
			m_continueNextTips.SetActive (true);

		for (int i = 0; i < m_curRoleEffectList.Count; i++) {
			if(m_curRoleEffectList[i] != null)
				Destroy(m_curRoleEffectList[i]);
		}
    }

    public override void Close()
    {

        if (m_createRolePanel != null)
        {
            Destroy(m_createRolePanel.gameObject);
            m_createRolePanel = null;
        }

		if (m_continueNextTips != null)
			m_continueNextTips.SetActive (false);

        foreach (var item in m_heroList.Keys)
        {
            if (OnDoAction != null)
            {
                OnDoAction(RoleState.Init, item);
            }
        }
        BackButton.gameObject.SetActive(false);
    }

    public override void DestroyPanel()
    {
        
    }
}
