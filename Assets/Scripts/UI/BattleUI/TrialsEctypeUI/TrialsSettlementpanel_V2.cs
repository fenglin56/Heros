using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Battle
{

    public class TrialsSettlementpanel_V2 : MonoBehaviour
    {

        public TimeScale[] timeScalList;

        public UIPanel TitlePanel;
        public UIPanel ScorePanelComponent;
        public UILabel EctypeNameLabel;
        public UILabel LeftTimeLabel;
        public UISprite WinIconTitle;
        public GameObject WinElementTitle;
        public SingleButtonCallBack WaveLabel;

        public SingleButtonCallBack BackBtn;

        public GameObject BackgroundEffectPrefab;
        public Transform CreatEffectTransform;

        public List<SingleTrialSettlementItem_V2> SingleElementList;

        public Animation[] BackgroundEffecteObjAnim { get; private set; }

        SMSGEctypeTrialsTotalResult_SC sMSGEctypeTrialsTotalResult_SC;

        void Awake()
        {
            BackBtn.SetCallBackFuntion(OnBackBtnClick);
            WaveLabel.gameObject.SetActive(false);
            BackgroundEffecteObjAnim = CreatObjectToNGUI.InstantiateObj(BackgroundEffectPrefab, CreatEffectTransform).GetComponentsInChildren<Animation>();
            DoForTime.DoFunForTime(4, PauseEffectAnimation, null);
            EctypeNameLabel.alpha = 0;
            BackBtn.gameObject.SetActive(false);
        }

        void PauseEffectAnimation(object obj)
        {
            BackgroundEffecteObjAnim.ApplyAllItem(P => P[P.clip.name].speed = 0);
        }

        public void Show(SMSGEctypeTrialsTotalResult_SC sMSGEctypeTrialsTotalResult_SC)
        {
            TraceUtil.Log("显示结算:"+sMSGEctypeTrialsTotalResult_SC.byClearance);
            ScorePanelComponent.alpha = 0;
            this.sMSGEctypeTrialsTotalResult_SC = sMSGEctypeTrialsTotalResult_SC;
            if (sMSGEctypeTrialsTotalResult_SC.byClearance == 1)
            {
                StartCoroutine(StartTimeScale(0));
            }
            else
            {
                ShowPanel();
            }
        }

        IEnumerator StartTimeScale(int Step)
        {
            Time.timeScale = timeScalList[Step].timeSpeed;
            //TraceUtil.Log(Step + "," + timeScalList[Step].timeSpeed);
            yield return new WaitForSeconds(timeScalList[Step].Time * timeScalList[Step].timeSpeed);
            if (Step >= timeScalList.Length - 1)
            {
                Time.timeScale = 1;
                ShowPanel();
            }
            else
            {
                Step++;
                StartCoroutine(StartTimeScale(Step));
            }

        }

        void ShowPanel()
        {
            StartCoroutine(ShowTimeLeftLabel(15));
            DoForTime.DoFunForTime(0.8f, TweenShowWinElementTitle, null);
            DoForTime.DoFunForTime(0.3f, TweenShowTitle01, null);
            DoForTime.DoFunForTime(2.5f, ShowProgress, null);
            DoForTime.DoFunForTime(3, ShowScorePanel, null);
        }

        void TweenShowWinElementTitle(object obj)
        {
            WinElementTitle.transform.localPosition = new Vector3(-220, 60, -90);
            TweenAlpha.Begin(WinElementTitle, 0.2f, 0, 1, TweenMoveWinElementTitle01);
        }

        void TweenMoveWinElementTitle01(object obj)
        {
            Vector3 fromPosition = new Vector3(-220, 60, -90);
            Vector3 toPosition = new Vector3(-235, 60, -90);
            TweenPosition.Begin(WinElementTitle, 0.3f, fromPosition, toPosition);
        }

        void TweenShowTitle01(object obj)
        {
            TweenAlpha.Begin(TitlePanel.gameObject, 0.3f, 0, 1, TweenShowTitle02);
        }

        void TweenShowTitle02(object obj)
        {
            SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            var EctypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
            EctypeNameLabel.SetText(LanguageTextManager.GetString(EctypeData.lEctypeName));
            TweenAlpha.Begin(EctypeNameLabel.gameObject, 0.3f, 0, 1, TweenShowTitle03);
        }

        void TweenShowTitle03(object obj)
        {
            TweenAlpha.Begin(WinIconTitle.gameObject, 0.3f, 0, 1, null);
        }

        [ContextMenu("InitProgressItem")]
        void InitProgressItemList()
        {
            this.SingleElementList = transform.GetComponentsInChildren<SingleTrialSettlementItem_V2>().ToList();
        }

        void ShowProgress(object obj)//显示波数
        {
            WaveLabel.gameObject.SetActive(true);
            WaveLabel.SetButtonBackground(this.sMSGEctypeTrialsTotalResult_SC.dwProgress);
            //WaveLabel.SetButtonText(this.sMSGEctypeTrialsTotalResult_SC.dwProgress.ToString());
        }

        /// <summary>
        /// 显示评分面板
        /// </summary>
        /// <param name="obj"></param>
        void ShowScorePanel(object obj)
        {
            Vector3 fromPos = new Vector3(-100,-45,-103);
            Vector3 toPos = new Vector3(200,-45,-103);
            TweenAlpha.Begin(ScorePanelComponent.gameObject,0.3f,0,1,null);
            TweenPosition.Begin(ScorePanelComponent.gameObject,0.3f,fromPos,toPos);
            DoForTime.DoFunForTime(0.5f,ShowScoreElement,0);
        }

        IEnumerator ShowTimeLeftLabel(int leftTime)
        {
            LeftTimeLabel.SetText(leftTime);
            yield return new WaitForSeconds(1);
            if (leftTime > 0)
            {
                leftTime--;
                StartCoroutine(ShowTimeLeftLabel(leftTime));
            }
        }

        void ShowScoreElement(object obj)
        {
            int index = (int)obj;
            if (this.sMSGEctypeTrialsTotalResult_SC.dwEquipReward.Count > index)
            {
                bool isLast = this.sMSGEctypeTrialsTotalResult_SC.dwEquipReward.Count == (index + 1);
                var showItem = this.sMSGEctypeTrialsTotalResult_SC.dwEquipReward[index];
                SingleElementList[index].Show(index + 1, (int)showItem.dwEquipId, (int)showItem.dwEquipNum, isLast, this);
                DoForTime.DoFunForTime(0.1f, ShowScoreElement, index + 1);
            }
            else
            {
                BackBtn.gameObject.SetActive(true); 
            }
        }

        public void OnBackBtnClick(object obj)
        {
            TraceUtil.Log("返回城镇");
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            long UIDEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
            NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(UIDEntity); 
        }

        public void Close()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }
    }
}