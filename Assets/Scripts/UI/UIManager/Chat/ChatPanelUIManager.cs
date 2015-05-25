using UnityEngine;
using System.Collections;
using Chat;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;

public class ChatPanelUIManager : BaseUIPanel
{
    public PrivateChatWindowManager PrivateChatWindowMgr;


    public LocalButtonCallBack Button_OpenWorldPanel;
    public GameObject NewChat_prompt;

    public GameObject WorldChild;
    public SingleButtonCallBack Button_Send;
    public UIInput Input_Chat;
    //public UILabel Label_Test;
    public ChatInfoItemControl ChatInfoItemPrefab;
    public UIDraggablePanel DraggablePanel;
    //public UIGrid Grid_Chat;
    public UITable Table_Chat;
    public ClickTalkerBoxControl TalkerBoxControl;    

    private uint iTalkToWorldID = 0;

    private List<ChatInfoItemControl> ChatRecordList = new List<ChatInfoItemControl>();

    private static string ColorChannel = "[F5C779]"; //落日黄
    private static string ColorName = "[01FE05]";//绿
    //private static string ColorWorldContent = "[90EE90]";//绿
    //private static string ColorMyContent = "[CD3278]";//粉
    private static string ColorEnd = "[-]";

    public static Color ColorMy = new Color(0.98f, 0.917f, 0);
    public static Color ColorOther = Color.white;
    private int m_guideBtnID = 0;

    void Awake()
    {
        Button_OpenWorldPanel.SetCallBackFuntion(OpenWorldPanel, null);        
        Button_Send.SetCallBackFuntion(SendChat, null);

        //TODO GuideBtnManager.Instance.RegGuideButton(Button_OpenWorldPanel.gameObject, UIType.Chat, SubType.ButtomCommon, out m_guideBtnID);

        RegisterEventHandler();

        //OpenWorldPanel(null);//默认开始打开

        InitLastChat();
    }

    //void Start()
    //{
    //    MainUIController.Instance.SetPanelActivEvent += new MainUIController.SetPanelDelegate(SetPanelEnable);
    //    MainUIController.Instance.SaveUIStatusEvent += new MainUIController.SaveUIStatusDelegate(SaveUIStatus);     
    //}

    //void SetPanelEnable(int[] UIStatus)
    //{
    //    if (UIStatus[0] == (int)UIType.Chat)
    //    {
    //        ShowChatPanel();
    //    }
    //    else
    //    {
    //        CloseChatPanel();
    //    }
    //}

    public override void Show(params object[] value)
    {
        base.Show(value);
    }

    public override void Close()
    {
        if (!IsShow)
            return;
        PrivateChatWindowMgr.ClosePanel();
        CloseWorldChatPanel();
        base.Close();
    }

    //void ShowChatPanel()
    //{
    //    //ChatPanelChild.transform.localPosition = Vector3.zero;
    //}
    //void CloseChatPanel()
    //{
    //    //ChatPanelChild.transform.localPosition = new Vector3(0, 0, -800);
    //    PrivateChatWindowMgr.ClosePanel();
    //    CloseWorldChatPanel();
    //}


    private void ShowWorldChatPanel()
    {
        WorldChild.transform.localPosition = Vector3.zero;
        if (NewChat_prompt.activeInHierarchy)
        {
            NewChat_prompt.SetActive(false);
        }
    }

    private void CloseWorldChatPanel()
    {
        WorldChild.transform.localPosition = new Vector3(-2000, 0, 0);
    }


    void OpenWorldPanel(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        if (WorldChild.transform.localPosition == Vector3.zero)
        {
            CloseWorldChatPanel();
        }
        else
        {
            ShowWorldChatPanel();
        }
    }

    void CloseWorldPanelHandle(object arg)
    {
        CloseWorldChatPanel();
        PrivateChatWindowMgr.ClosePanel();
    }

    void SendChat(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        string chat = Input_Chat.text;
        if (chat == "")
        {
            return;
        }
        var playerData = PlayerManager.Instance.FindHeroDataModel();
        NetServiceManager.Instance.ChatService.SendChat((uint)playerData.ActorID, iTalkToWorldID, chat,0, Chat.ChatDefine.MSG_CHAT_WORLD);

        Input_Chat.text = "";
    }
    void ShowWorldChatHandle(object obj)
    {
        if (WorldChild.transform.localPosition != Vector3.zero)
        {
            NewChat_prompt.SetActive(true);
        }

        SMsgChat_SC sMsgChat_SC = (SMsgChat_SC)obj;
        this.CreateChat(sMsgChat_SC);

        Table_Chat.Reposition();
        StartCoroutine("SetDragAmount");
    }
    void CloseUIHandle(object obj)
    {
        this.CloseWorldChatPanel();
    }

    private void InitLastChat()
    {
        var recordList = ChatRecordManager.Instance.GetWorldChatRecordList();
        recordList.ApplyAllItem(p =>
        {
            this.CreateChat(p);
        });
    }
    private void CreateChat(SMsgChat_SC sMsgChat_SC)
    {
        string chatContent = "";
        chatContent = ColoringChannel("[世界]") + ColoringName(sMsgChat_SC.SenderName + " : ") + sMsgChat_SC.Chat;
        GameObject chat = (GameObject)Instantiate(ChatInfoItemPrefab.gameObject);
        chat.transform.parent = Table_Chat.transform;
        chat.transform.localScale = Vector3.one;
        chat.transform.localPosition = Vector3.zero;
        var chatControl = chat.GetComponent<ChatInfoItemControl>();
		//chatControl.Init(sMsgChat_SC.L_Channel,sMsgChat_SC.senderActorID, sMsgChat_SC.SenderName, chatContent, sMsgChat_SC.bChatType, sMsgChat_SC.friendTeamID, ClickChatTargetCallBack);
        AddChatRecord(chatControl);
    }

    IEnumerator SetDragAmount()
    {
        yield return new WaitForEndOfFrame();
        if (DraggablePanel.shouldMove)
        {
            DraggablePanel.SetDragAmount(0, 1, false);
        }   
    }

    private void AddChatRecord(ChatInfoItemControl item)
    {
        if (ChatRecordList.Count >= 100)
        {
            ChatRecordList[0].DestroySelf();
            ChatRecordList.RemoveAt(0);
        }
        ChatRecordList.Add(item);        
    }

    //[ContextMenu("SetDraggablePanel")]
    //void SetDraggablePanel()
    //{
    //    TraceUtil.Log(DraggablePanel.shouldMove);
    //    DraggablePanel.SetDragAmount(0, 1, false);
    //}


    void OpenPrivateChatWindowHandle(object obj)
    {
        TalkerInfo talkerInfo = (TalkerInfo)obj;
        //TraceUtil.Log("[OpenPrivateChatWindowHandle talkerInfo]" + talkerInfo.ActorID + " , " + talkerInfo.Name);
        PrivateChatWindowMgr.OpenPrivateWindow(talkerInfo.ActorID, talkerInfo.Name);
    }

    void OpenWorldChatWindowHandle(object obj)
    {
        this.OpenWorldPanel(obj);
    }

    void ClickChatTargetCallBack(object talkerInfo, Transform boxTrans)
    {
        TalkerInfo info = (TalkerInfo)talkerInfo;
        
        TalkerBoxControl.transform.position = boxTrans.position;
        TalkerBoxControl.gameObject.SetActive(true);

        TalkerBoxControl.SetTargetTalkerInfo(info);
        //this.sTalkTargetName = name;
        //Label_ToChatTarger.text = "@" + sTalkTargetName + ":";
    }

    public static string ColoringChannel(string str)
    {
        return ColorChannel + str + ColorEnd;
    }
    public static string ColoringName(string str)
    {
        return ColorName + str + ColorEnd;
    }
    //public static string ColoringWorldContent(string str)
    //{
    //    return ColorWorldContent + str + ColorEnd;
    //}
    //public static string ColoringMyContent(string str)
    //{
    //    return ColorMyContent + str + ColorEnd;
    //}

    protected override void RegisterEventHandler()
    {
        UIEventManager.Instance.RegisterUIEvent(UIEventType.WorldChatMsg, ShowWorldChatHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseAllUI, CloseUIHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenPrivateChatWindow, OpenPrivateChatWindowHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseWorldChatWindow, CloseWorldPanelHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI, CloseWorldPanelHandle);
        //UIEventManager.Instance.RegisterUIEvent(UIEventType.PrivateChatMsg, ShowPrivateChatHandle);
    }

    void OnDestroy()
    {
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.WorldChatMsg, ShowWorldChatHandle);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseAllUI, CloseUIHandle);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenPrivateChatWindow, OpenPrivateChatWindowHandle);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenWorldChatWindow, OpenWorldChatWindowHandle);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseWorldChatWindow, CloseWorldPanelHandle);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, CloseWorldPanelHandle);
        //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
    }
}
