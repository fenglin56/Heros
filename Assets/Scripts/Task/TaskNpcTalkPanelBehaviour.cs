using UnityEngine;
using System.Collections;

public class TaskNpcTalkPanelBehaviour : MonoBehaviour {

    public GameObject DialogPanelPoint;
    public SingleButtonCallBack BackBtn;

    void Awake()
    {
        BackBtn.SetCallBackFuntion((obj) =>
            {
				//send event to joystick of TownUI
				UIEventManager.Instance.TriggerUIEvent(UIEventType.OnNpcTalkCloseEvent, null);
                SoundManager.Instance.PlaySoundEffect("Sound_Button_TownMain_Leave");
                GameObject.Destroy(gameObject);
            });
    }
    public StoryDialogBehaviour Show(GameObject dialogBox, TalkIdConfigData talkIdConfigData)
    {
        var storyPanel = NGUITools.AddChild(gameObject, dialogBox);
        var storyPanelBehaviour = storyPanel.GetComponent<StoryDialogBehaviour>();
        storyPanelBehaviour.transform.localPosition = DialogPanelPoint.transform.localPosition;
        storyPanelBehaviour.Init(talkIdConfigData,false);
        storyPanelBehaviour.StoryGuideFinishAct = () =>
        {
			//send event to joystick of TownUI
			UIEventManager.Instance.TriggerUIEvent(UIEventType.OnNpcTalkCloseEvent, null);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_TownMain_Leave");
            GameObject.Destroy(gameObject);
        };
        return storyPanelBehaviour;
    }
}
