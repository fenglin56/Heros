using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.Battle
{

    public class BattleSceneTrialsEctypeUIManager : MonoBehaviour
    {
        private static BattleSceneTrialsEctypeUIManager m_instance;
        public static BattleSceneTrialsEctypeUIManager Instance { get { return m_instance; } }

        public GameObject TrialsEctypeIconPrefab;
        public GameObject SingleTrialsSettlementPanelPrefab;
        public GameObject TrialsSettlementPanelPrefab;

        public SingleButtonCallBack ProgressLabel;

        public bool ISTrialsEctype { get; private set; }

        //private bool IsFrstTrialsEctype = false;

        SingleTrialsSettlementPanel SingleTrialsSettlementPanel;
        TrialsSettlementpanel_V2 TrialsSettlementpanel;


        void Awake()
        {
            m_instance = this;
            if (GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
            {
                GameDataManager.Instance.dataEvent.RegisterEvent(DataType.InitializeEctype, Init);
            }
            else
            {
                Init(null);
            }
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.SingleTrialsSettlement, ShwoSingleTrialsSettlement);
            GameDataManager.Instance.dataEvent.RegisterEvent(DataType.SingleTrialsSettlement, ShwoSingleTrialsSettlement);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.TrialSettlement,ShowTrialsSettlement);
        }

        void Start()
        {
            object singleTrialsSettlementObj = GameDataManager.Instance.PeekData(DataType.SingleTrialsSettlement);
            if (singleTrialsSettlementObj != null)
            {
                ShwoSingleTrialsSettlement(singleTrialsSettlementObj);
            }
        }

        void OnDestroy()
        {
            m_instance = null;
            //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.SingleTrialsSettlement, ShwoSingleTrialsSettlement);
            GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.SingleTrialsSettlement, ShwoSingleTrialsSettlement);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TrialSettlement, ShowTrialsSettlement);
            GameDataManager.Instance.ClearData(DataType.SingleTrialsSettlement);
        }

        void Init(object obj)
        {
            GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.InitializeEctype, Init);
            SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            EctypeContainerData ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
            ISTrialsEctype = ectypeData.MapType == 5;
            if (ectypeData.MapType != 5)
            {
                ProgressLabel.gameObject.SetActive(false);
                return;
            }
            ProgressLabel.gameObject.SetActive(true);
            ProgressLabel.SetButtonText("1");
            LoadSceneData loadSceneData = GameDataManager.Instance.PeekData(DataType.LoadingSceneData) as LoadSceneData;
            var LoadSceneInfo = (SMsgActionNewWorld_SC)loadSceneData.LoadSceneInfo;
            //var TrialsSceneList = EctypeConfigManager.Instance.EctypeContainerConfigFile.ectypeContainerDataList.Where(P => P.MapType == 5).ToArray();
            if (ectypeData.vectMapID.Split('+')[0] == LoadSceneInfo.dwMapId.ToString())
            {
                //IsFrstTrialsEctype = true;
                StartCoroutine(ShowTrialsEctypeIcon(2));
            }
        }

        IEnumerator ShowTrialsEctypeIcon(float waitSeconds)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_TrialsStar");
            GameObject IconPrefab =  CreatObjectToNGUI.InstantiateObj(TrialsEctypeIconPrefab, transform);
            yield return new WaitForSeconds(waitSeconds);
            Destroy(IconPrefab);
        }

        public void ShwoSingleTrialsSettlement(object obj)
        {
            SMSGEctypeTrialsSubResult_SC sMSGEctypeTrialsSubResult_SC = (SMSGEctypeTrialsSubResult_SC)obj;
            TraceUtil.Log("收到单个试炼副本结算消息：" + sMSGEctypeTrialsSubResult_SC.dwProgress);
            ProgressLabel.SetButtonText((sMSGEctypeTrialsSubResult_SC.dwProgress).ToString());
            if (SingleTrialsSettlementPanel == null)
            {
                SingleTrialsSettlementPanel = CreatObjectToNGUI.InstantiateObj(SingleTrialsSettlementPanelPrefab,transform).GetComponent<SingleTrialsSettlementPanel>();
            }
            SingleTrialsSettlementPanel.Show(sMSGEctypeTrialsSubResult_SC);
        }

        public void ShowTrialsSettlement(object obj)
        {
            SMSGEctypeTrialsTotalResult_SC sMSGEctypeTrialsTotalResult_SC = (SMSGEctypeTrialsTotalResult_SC)obj;
            if (TrialsSettlementpanel == null)
            {
                TrialsSettlementpanel = CreatObjectToNGUI.InstantiateObj(TrialsSettlementPanelPrefab, transform).GetComponent<TrialsSettlementpanel_V2>();
            }
            TrialsSettlementpanel.Show(sMSGEctypeTrialsTotalResult_SC);
            if (SingleTrialsSettlementPanel != null)
            {
                SingleTrialsSettlementPanel.Close();
            }
        }
    }
}