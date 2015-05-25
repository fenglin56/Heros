using UnityEngine;
using System.Collections;
using System;

public class TaskNPCBehaviour : View
{
    public Action<StoryPersonInfo> ShowTaskNpcPanel;
    private GameObject m_talkPanelGo;
    private StoryPersonInfo m_storyPersonInfo;
    void Awake()
    {
        RegisterEventHandler();
    }	

    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.OnTouchInvoke.ToString(), OnTouchDown);
    }
    public void InitTaskNPCData(StoryPersonInfo storyPersonInfo)
    {
        m_storyPersonInfo = storyPersonInfo;
    }
    private void OnTouchDown(INotifyArgs e)
    {
        TouchInvoke touchInvoke = (TouchInvoke)e;
        if (touchInvoke.TouchGO == gameObject)
        {
            if(ShowTaskNpcPanel!=null && !TaskModel.Instance.isNpcTalking)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Task_QuickGuide ");
                ShowTaskNpcPanel(m_storyPersonInfo);
            }
        }
    }

    void Destroy()
    {
        RemoveAllEvent();
    }
}
