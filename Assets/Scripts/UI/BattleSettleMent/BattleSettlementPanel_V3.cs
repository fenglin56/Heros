using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Battle
{

    public class BattleSettlementPanel_V3 : MonoBehaviour
    {

        public UIPanel TitlePanel;
        public GameObject WinElementTitle_1;
        public GameObject WinElementTitle_2;
        public UILabel EctypeNameLabel;
        public UISprite WinIconTitle;//战斗获胜标题
        public SingleButtonCallBack SuggestionPesionLabel;

        public GameObject BackgroundEffectPrefab;
        public Transform CreatEffectTransform;
        public Transform CreateEvaluateObjTransform;
        public Animation[] BackgroundEffecteObjAnim { get; private set; }

        public GameObject ScorePanelPrefab;//评分面板
        public BattleSettlementScorePanel_V3 ScorePanel { get; private set; }
        public GameObject RewardPanelPrefab;//奖励面板
		public BattleSettlementRewardPanel_V4 RewardPanel { get; private set; }

        public EvaluateConfigDataBase EvaluateData;
        public SMSGEctypeSettleAccounts sMSGEctypeSettleAccounts { get; private set; }
        public EctypeContainerData EctypeData { get; private set; }

        void Awake()
        {
            TitlePanel.alpha = 0;
            EctypeNameLabel.alpha = 0;
            WinIconTitle.alpha = 0;

            WinElementTitle_1.GetComponent<UIPanel>().alpha = 0;
            WinElementTitle_2.GetComponent<UIPanel>().alpha = 0;

            BackgroundEffecteObjAnim = CreatObjectToNGUI.InstantiateObj(BackgroundEffectPrefab, CreatEffectTransform).GetComponentsInChildren<Animation>();
            DoForTime.DoFunForTime(4, PauseEffectAnimation, null);
            if (GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
            {
                GameDataManager.Instance.dataEvent.RegisterEvent(DataType.InitializeEctype, Init);
            }
            else
            {
                Init(null);
            }
        }

        void Init(object obj)
        {
            GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.InitializeEctype, Init);
            SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
            EctypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
            EctypeNameLabel.SetText(LanguageTextManager.GetString(EctypeData.lEctypeName));
            SuggestionPesionLabel.SetButtonText(string.Format(LanguageTextManager.GetString("IDS_H1_501"), EctypeData.PlayerNum));
        }


        void PauseEffectAnimation(object obj)
        {
            BackgroundEffecteObjAnim.ApplyAllItem(P=>P[P.clip.name].speed = 0);
            //foreach (AnimationState child in BackgroundEffecteObjAnim[0])
            //{
            //    child.speed = 0;
            //}
        }

        public void Show(SMSGEctypeSettleAccounts sMSGEctypeSettleAccounts)
        {
            SoundManager.Instance.StopBGM(0f);
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalStar");
            this.sMSGEctypeSettleAccounts = sMSGEctypeSettleAccounts;
            DoForTime.DoFunForTime(0.3f, TweenShowWinElementTitle01, null);
            DoForTime.DoFunForTime(0.3f, TweenShowTitle01, null);
            DoForTime.DoFunForTime(2.5f, ShowGrad, null);
            DoForTime.DoFunForTime(2,PlayGradSound,null);
            DoForTime.DoFunForTime(1, ShowScorePanel, null);

			float delayTime = EctypeData.BattleVictoryLotteryTime -5 -3 -EctypeData.ResultAppearDelay;//5秒慢镜+3秒胜利动作+结算延迟出现			
			StartCoroutine("ShowFreeTreasureRewardDelay", delayTime);
        }

		IEnumerator ShowFreeTreasureRewardDelay(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			RewardPanel.ShowFreeTreasureReward();
		}

        void TweenShowWinElementTitle01(object obj)
        {
            WinElementTitle_1.transform.localPosition = new Vector3(-220, 60, 10);
            TweenAlpha.Begin(WinElementTitle_1,0.2f,0,1,TweenMoveWinElementTitle01);
        }

        void TweenMoveWinElementTitle01(object obj)
        {
            Vector3 fromPosition = new Vector3(-220,60,10);
            Vector3 toPosition = new Vector3(-235,60,10);
            TweenPosition.Begin(WinElementTitle_1, 0.3f, fromPosition, toPosition,PlayElemet01Sound);
        }

        void PlayElemet01Sound(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalNew");
        }

        void TweenShowTitle01(object obj)
        {
            TweenAlpha.Begin(TitlePanel.gameObject, 0.3f, 0, 1, TweenShowTitle02);
        }

        void TweenShowTitle02(object obj)
        {
            TweenAlpha.Begin(EctypeNameLabel.gameObject, 0.3f, 0, 1, TweenShowTitle03);
        }

        void TweenShowTitle03(object obj)
        {
            TweenAlpha.Begin(WinIconTitle.gameObject, 0.3f, 0, 1, null);
        }

        void PlayGradSound(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalNew");
        }

        void ShowGrad(object obj)//显示评级
        {
            GameObject objPrefab = EvaluateData.EvaluateDataList.FirstOrDefault(P=>P.Evaluate == this.sMSGEctypeSettleAccounts.sGrade).IconPrefab;
            if (objPrefab == null)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"无该评级对应字体：" + sMSGEctypeSettleAccounts.sGrade);
                return;
            }
            GameObject creatObj = CreatObjectToNGUI.InstantiateObj(objPrefab, CreateEvaluateObjTransform);
            //DoForTime.DoFunForTime(5, TweenRemoveGrad, creatObj);
        }
        /// <summary>
        /// 显示第一个评分面板
        /// </summary>
        /// <param name="obj"></param>
        void ShowScorePanel(object obj)
        {
            TraceUtil.Log("显示第一个评分面板：" + Time.time);
            ScorePanel = CreatObjectToNGUI.InstantiateObj(ScorePanelPrefab, transform).GetComponent<BattleSettlementScorePanel_V3>();
            ScorePanel.Show(this.sMSGEctypeSettleAccounts,this);
        }

        /// <summary>
        /// 显示第一个评分面板完毕，显示第二个奖励面板
        /// </summary>
        /// <param name="obj"></param>
        public void ShowPanel01Complete(object obj)
        {
            SuggestionPesionLabel.gameObject.SetActive(false);
            TweenAlpha.Begin(WinElementTitle_1.gameObject, 0.1f, 1, 0, null);
            Vector3 fromPosition = new Vector3(-235,60,10);
            Vector3 toPosition = new Vector3(-425,60,10);
            TweenPosition.Begin(WinElementTitle_1.gameObject,0.1f,fromPosition,toPosition);
            BackgroundEffecteObjAnim.ApplyAllItem(P => P[P.clip.name].speed = 1);
            DoForTime.DoFunForTime(1, TweenRemoveGrad, null);
            DoForTime.DoFunForTime(1.5f, ShowPanel02, null);
            //ShowPanel02(null);
        }
        void TweenRemoveGrad(object obj)//移除评级字体
        {
            CreateEvaluateObjTransform.ClearChild();
        }

        void ShowPanel02(object obj)
        {
            if (RewardPanel == null)
            {
				RewardPanel = CreatObjectToNGUI.InstantiateObj(RewardPanelPrefab,transform).GetComponent<BattleSettlementRewardPanel_V4>();
            }
            //TweenShowWinElementTitle02(null);
            DoForTime.DoFunForTime(0.5f,TweenShowRewadPanel,null);
        }

        void TweenShowRewadPanel(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalSecondUI");
            TraceUtil.Log("开始显示第二个抽奖面板：" + Time.time);
            RewardPanel.Show(this);
        }

        void TweenShowWinElementTitle02(object obj)
        {
            WinElementTitle_2.transform.localPosition = new Vector3(-220, 60, 10);
            TweenAlpha.Begin(WinElementTitle_2, 0.2f, 0, 1, TweenMoveWinElementTitle02);
        }

        void TweenMoveWinElementTitle02(object obj)
        {
            Vector3 fromPosition = new Vector3(-220, 60, 10);
            Vector3 toPosition = new Vector3(-235, 60, 10);
            TweenPosition.Begin(WinElementTitle_2, 0.3f, fromPosition, toPosition,PlayElemet02Sound);
        }

        void PlayElemet02Sound(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_AppraisalNew"); 
        }

    }
}