using UnityEngine;
using System.Collections;
using UI;

public class EctGuideRootPanel : View {

    private EctGuideStepConfigData m_stepItem;
    private int m_picIndex;
    private LocalButtonCallBack m_guidePicBtn;
    
	/// <summary>
    /// 显示剧情对话内容
    /// </summary>
    /// <param name="newGuideConfigData"></param>
    private void StartPicStep(EctGuideStepConfigData stepItem)
    {
        if (stepItem.GuidePicture == null)
        {
            return;
        }
        m_stepItem = stepItem;
        m_picIndex = 0;
        if (m_picIndex < stepItem.GuidePicture.Length)
        {
            var picPrefab = m_stepItem.GuidePicture[m_picIndex];
            if (picPrefab != null)
            {
                m_picIndex = 1;
                m_guidePicBtn = CreatObjectToNGUI.InstantiateObj(picPrefab, this.transform).GetComponent<LocalButtonCallBack>();
                m_guidePicBtn.SetCallBackFuntion((obj) =>
                    {
                        if (m_guidePicBtn != null)
                        {
                            Destroy(m_guidePicBtn.gameObject);
                        }
                        m_picIndex++;
                        StartPicStep(stepItem);
                    });
            }
        }
        else
        {
            if (m_guidePicBtn != null)
            {
                Destroy(m_guidePicBtn.gameObject);
            }
            m_picIndex = 0;
            SMsgInteractCOMMON_CS msgInteract;
            msgInteract.dwNPCID = 0;
            msgInteract.byOperateType = 2;
            msgInteract.dwParam1 = 2;
            msgInteract.dwParam2 = 0;
            msgInteract.byIsContext = 0;

            SMsgInteractCOMMONContext_CS msgContext;
            msgContext.szContext = new byte[32];
            NetServiceManager.Instance.InteractService.SendInteractCOMMON(msgInteract, msgContext);
        }
    }
    protected override void RegisterEventHandler()
    {
    }
}
