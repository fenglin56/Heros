using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
using UI;
public class NpcTalk_v2 : View {

    private static NpcTalk_v2 m_instance;
    public static NpcTalk_v2 Instance { get { return m_instance; } }
    public GameObject TalkPanelGo;
    //public SystemFuntionButton systemFuntionButton;
    public SMsgInteractCOMMONPackage m_sMsgInteractCOMMONData { get; set; }
    private NpcTalkPanel m_npcTalkPanel;
	// Use this for initialization
	void Start () {
        this.RegisterEventHandler();
	}

    void Awake()
    {
        m_instance = this;
    }

    void OnDestroy()
    {
        m_instance = null;
        RemoveEventHandler(EventTypeEnum.NPCInteraction.ToString(), CreateNpcTalkPanel); 
    }

    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.NPCInteraction.ToString(), CreateNpcTalkPanel);
    }

    public void CreateNpcTalkPanel(INotifyArgs notifyArgs)
    {
       // systemFuntionButton.gameObject.SetActive(false);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UIType.Empty);
        m_sMsgInteractCOMMONData = (SMsgInteractCOMMONPackage)notifyArgs;

        if (m_npcTalkPanel == null)
        {
            m_npcTalkPanel = (Instantiate(TalkPanelGo) as GameObject).GetComponent<NpcTalkPanel>();
            m_npcTalkPanel.transform.parent = transform;
            m_npcTalkPanel.transform.localScale = Vector3.one;
            m_npcTalkPanel.InitTalkPanel(m_sMsgInteractCOMMONData);
        }
        else
        {
            m_npcTalkPanel.InitTalkPanel(m_sMsgInteractCOMMONData);
        }
    }

}
