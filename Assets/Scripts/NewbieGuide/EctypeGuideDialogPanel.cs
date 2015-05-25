using UnityEngine;
using System.Collections;
using System.Linq;
using UI.Battle;

public class EctypeGuideDialogPanel : MonoBehaviour {

	// Use this for initialization
    public GameObject Layer;
    public UILabel NameText;
    public UILabel DialogText;
    public UILabel ClickSign;
    public Transform NpcIconPoint;
    public LocalButtonCallBack DialogBtn;
    public UILabel TaskName;
    private int m_curDialogIndex = 0;
    private int m_curGroupIndex = 0;
    private EctGuideStepConfigData m_curGuideData;
    private Vector3 m_originCamPos = Vector3.zero;

    private GuideDialogInfo m_guideDialogInfo;
    // Use this for initialization
    void Awake()
    {
        DialogBtn.SetCallBackFuntion(DialogButtonHandle);
    }   

    // 初始化对话数据
    public void InitDialogPanel(EctGuideStepConfigData item)
    {
        m_curGroupIndex = 0;
        BattleUIManager.Instance.ShowStoryCover(true);
        m_curGuideData = item;
        ClickSign.text = LanguageTextManager.GetString("IDS_H1_218");
        m_originCamPos = BattleManager.Instance.FollowCamera.transform.position;
        BattleManager.Instance.BlockPlayerToIdle = true;
        StartCoroutine(ChangeCameraToDialog());
    }
    private IEnumerator ChangeCameraToDialog()
    {
        Layer.SetActive(false);
        if (m_curGuideData.GuideDialogInfos != null)
        {
            if (m_curGroupIndex < m_curGuideData.GuideDialogInfos.Length)
            {
                float time = 0;
                m_guideDialogInfo = m_curGuideData.GuideDialogInfos[m_curGroupIndex++];
                var cameraPos = m_guideDialogInfo.CameraPos;
                if (cameraPos != Vector3.zero)
                {
                    if (cameraPos == Vector3.one)
                    {
                        cameraPos = m_originCamPos;
                    }
                    time = m_guideDialogInfo.CameraMoveTime/1000f;
                    BattleManager.Instance.FollowCamera.SetFixed(cameraPos, time);
                }
                yield return new WaitForSeconds(time);
                if (m_guideDialogInfo.StepTalkIds != null && m_guideDialogInfo.StepTalkIds.Length > 0)
                {
                    ShowDialogContent();
                }
            }
            else
            {
                //镜头回来
                var time = m_curGuideData.CamraBackTime/1000f;
                BattleManager.Instance.FollowCamera.SetFixed(m_originCamPos, time);
                yield return new WaitForSeconds(time);
                //恢复镜头移动[强制回来]
                BattleManager.Instance.FollowCamera.ResetSetTarget();
                if (m_curGuideData.StepType == 0)
                    NetServiceManager.Instance.InteractService.SendEctypeDialogOver();
                BattleUIManager.Instance.ShowStoryCover(false);

                BattleManager.Instance.BlockPlayerToIdle = false;
                OnDestroy();
            }
        }
        else
        {
            BattleManager.Instance.BlockPlayerToIdle = false;
            yield break;
        }
    }
    private void ShowDialogContent()
    {
        if(!Layer.activeSelf)
            Layer.SetActive(true);
        DialogBtn.SetButtonActive(true);
        int talkId = m_guideDialogInfo.StepTalkIds[m_curDialogIndex++];
        var stepDialogInfo = GuideConfigManager.Instance.EctGuideTalkDataBase.Datas.SingleOrDefault(P => P.TalkID == talkId);
        if (stepDialogInfo != null)
        {
            string npcName = "";
            GameObject npcIcon = null;
            if (stepDialogInfo.TalkType==1)
            {
                npcName = PlayerManager.Instance.FindHeroDataModel().Name;
                var playerDataStruct = (SMsgPropCreateEntity_SC_MainPlayer)PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct;
                var fashionId = playerDataStruct.GetCommonValue().PLAYER_FIELD_VISIBLE_FASHION;
                var vocationId = playerDataStruct.GetPlayerKind();
                var npcTackData = CommonDefineManager.Instance.CommonDefine.HeroIcon_Dialogue.SingleOrDefault(P => P.FashionID == fashionId && P.VocationID == vocationId);
                if (npcTackData != null)
                {
                    npcIcon = npcTackData.IconPrefab;
                }
            }
            else
            {
                npcName = LanguageTextManager.GetString(stepDialogInfo.NPCName);
                npcIcon = stepDialogInfo.TalkHead;
            }
            NameText.text = npcName;
            DialogText.text = LanguageTextManager.GetString(stepDialogInfo.TalkText);
            NpcIconPoint.ClearChild();
            if (npcIcon != null)
            {
                var npcIconIns = NGUITools.AddChild(NpcIconPoint.gameObject, npcIcon);
                npcIconIns.transform.localScale = new Vector3(95, 95, 1);
            }
            transform.localPosition = stepDialogInfo.OffsetVect;
        }        
    }
    /// <summary>
    /// Button事件函数
    /// </summary>
    void DialogButtonHandle(object obj)
    {
        DialogBtn.SetButtonActive(false);
        if (m_curDialogIndex < m_guideDialogInfo.StepTalkIds.Length)
        {
            ShowDialogContent();
        }
        else
        {
            m_curDialogIndex = 0;
            StartCoroutine(ChangeCameraToDialog());            
        }
    }

    public void OnDestroy()
    {
        m_curGroupIndex = 0;
        m_curDialogIndex = 0;
        Destroy(this.gameObject);
    }

}
