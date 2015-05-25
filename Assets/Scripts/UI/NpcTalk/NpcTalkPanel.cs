using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;

public class NpcTalkPanel : MonoBehaviour {
	public Vector3 showPos ;
    public UILabel NpcName;
    public UILabel DialogLabel;
    //public UISprite NpcIcon;
    public MiddleButtonPanel MiddlePanel;
    public SingleButtonCallBack BackButton;
    //public Transform CreatBackBtnPoint;
    public Transform HeadPoint;
    private NPCTalkConfigData m_npcTalkConfig;
   // private CommonUIBottomButtonTool m_backButton;
    private bool m_isOpening;

    private int m_guideBtnID = 0;
    void Start()
    {
        BackButton.SetCallBackFuntion(OnBackButtonTapped);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI,ClosePanel);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, null);
    }
	
	private void OnBackButtonTapped(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_TownMain_Leave");
        UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
        ClosePanel(null);
		//send event to joystick of TownUI
		//UIEventManager.Instance.TriggerUIEvent(UIEventType.OnNpcTalkCloseEvent, null);
    }
	
    void OnDestroy()
    {
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, ClosePanel);
        GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);//TODO 81
    }

    public void InitTalkPanel(SMsgInteractCOMMONPackage sMsgInteractCOMMONPackage)
    {
        m_isOpening = true;
		//send event to joystick of TownUI
		UIEventManager.Instance.TriggerUIEvent(UIEventType.OnNpcTalkOpenEvent, null);
        if (BackButton != null)
        {
          //  m_backButton.ShowAnim();
        }
		transform.localPosition = showPos;//Vector3.zero;
        int npcID = (int)sMsgInteractCOMMONPackage.sMsgInteractCOMMON_SC.dwNPCID;
		NPCConfigData npcData = NPCConfigManager.Instance.NPCConfigList[npcID];
		
        UI.CreatObjectToNGUI.InstantiateObj(npcData.PortraitID,HeadPoint);
        var npcTalkData = GetNpcTalk(sMsgInteractCOMMONPackage);
		TypeID entityType;
		Transform npcTransform = EntityController.Instance.GetEntityModel(GameManager.Instance.MeetNpcEntityId, out entityType).GO.transform;
		if(null != npcTransform)
		{
			BattleManager.Instance.FollowCamera.SetSmoothMoveTarget(npcTransform, npcData.CameraOffset);	
		}
		
      
        NpcName.text = LanguageTextManager.GetString(npcData._szName);
        if (npcTalkData != null)
        {
            DialogLabel.text = LanguageTextManager.GetString(npcTalkData._szTalk);
            if (npcTalkData._szVoice != "0")
            {
                SoundManager.Instance.PlaySoundEffect(npcTalkData._szVoice);
            }
        }
        //this.NpcIcon.spriteName = NPCConfigManager.Instance.NPCConfigList[(int)npcID].PortraitID;
        
        SMsgInteractCOMMONBtn_SC[] MyButtonList = sMsgInteractCOMMONPackage.sMsgInteractCOMMONBtn_SC;

        if (MyButtonList.Length > 0)
        {
            MiddlePanel.CreateMiddleButton(sMsgInteractCOMMONPackage);
        }
    }

    private NPCTalkConfigData GetNpcTalk(SMsgInteractCOMMONPackage sMsgInteractCOMMONData)
    {
        var NPCTalkConfigList = NPCConfigManager.Instance.NPCTalkConfigList;
        foreach (KeyValuePair<int, NPCTalkConfigData> child in NPCTalkConfigList)
        {
            if (child.Key == sMsgInteractCOMMONData.sMsgInteractCOMMON_SC.dwParam)
            {
                return child.Value;
            }
        }
        return null;
    }
    private void ClosePanel(object obj)
    {
		if (!m_isOpening)
			return;
		//send event to joystick of TownUI[加在这里，因为可能会有弱引导]
		UIEventManager.Instance.TriggerUIEvent(UIEventType.OnNpcTalkCloseEvent, null);
        m_isOpening = false;
		Transform target = PlayerManager.Instance.FindCameraFollowTarget();
        if (null != target)
        {
            BattleManager.Instance.FollowCamera.SmoothMoveTargetOriginal(target);

        } 
        transform.localPosition = new Vector3(0, 0, -1000);
    }
}
