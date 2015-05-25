using UnityEngine;
using System.Collections;

public class EmailBtnEff : View {
    public GameObject FullEff;
    public GameObject UnReaderEff;
    private emEMAIL_STATE_TYPE perState=emEMAIL_STATE_TYPE.EMAIL_NOREAD_STATE_TYPE;
    void Awake()
    {
        RegisterEventHandler();
        ShowMailBtnEffectHandel(EmailDataManager.Instance.EmailStateType);
    }
    void OnDestroy()
    {
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ChangemailBtnEffect,ShowMailBtnEffectHandel);
    }
   protected override void RegisterEventHandler()
    {
        UIEventManager.Instance.RegisterUIEvent(UIEventType.ChangemailBtnEffect,ShowMailBtnEffectHandel);
    }
	
    void ShowMailBtnEffectHandel(object obj)
    {
        emEMAIL_STATE_TYPE type = (emEMAIL_STATE_TYPE)obj;
        if (type != perState)
        {
            switch (type)
            {
                case emEMAIL_STATE_TYPE.EMAIL_NONE_STATE_TYPE:
                    FullEff.SetActive(false);
                    UnReaderEff.SetActive(false);
                    perState=emEMAIL_STATE_TYPE.EMAIL_NONE_STATE_TYPE;
                    break;
                case emEMAIL_STATE_TYPE.EMAIL_FULL_STATE_TYPE:
                    FullEff.SetActive(true);
                    UnReaderEff.SetActive(false);
                    perState=emEMAIL_STATE_TYPE.EMAIL_FULL_STATE_TYPE;
                    break;
                case emEMAIL_STATE_TYPE.EMAIL_NOREAD_STATE_TYPE:
                    FullEff.SetActive(false);
                    UnReaderEff.SetActive(true);
                    perState=emEMAIL_STATE_TYPE.EMAIL_NOREAD_STATE_TYPE;
                    break;
            }
        }
    }
}
public enum MailBtnEffMsgType
{
    HideAll,
    ShowFullEff,
    ShowHasUnread,

}