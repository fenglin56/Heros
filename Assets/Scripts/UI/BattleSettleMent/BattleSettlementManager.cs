using UnityEngine;
using System.Collections;
using System;

namespace UI.Battle
{

    public class BattleSettlementManager : View
    {
        int timeStep =0;
        public TimeScale[] timeScalList;

        public GameObject WinEffectPrefab;
		public GameObject FaildEffectPrefab;
        public Transform CreatWinEffectTransform;
		public Transform CreateFialedEffTransform;

        public GameObject BattleSettlementPanelPreafab;
        public BattleSettlementPanel_V3 BattleSettlementPanel { get; private set; }
        SMSGEctypeSettleAccounts sMSGEctypeSettleAccounts;

        public bool IsGameSettlement{get;private set;}//是否正在结算
		private EctypeContainerData m_ectypeData;

        private static BattleSettlementManager m_Instance;
        public static BattleSettlementManager Instance()
        {
            return m_Instance;
        }

        void Awake()
        {
            m_Instance = this;
        }

        void Start()
        {
            RegisterEventHandler();
        }

        protected override void RegisterEventHandler()
        {
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.BossDeathMsg,BossDeath);
			AddEventHandler(EventTypeEnum.EctypeFinish.ToString(),EctypeFinishHandle);
            AddEventHandler(EventTypeEnum.EctypeSettleAccount.ToString(), Show);
        }

        void OnDestroy()
        {
            //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.BossDeathMsg, BossDeath);    
			RemoveEventHandler(EventTypeEnum.EctypeFinish.ToString(), EctypeFinishHandle);
			RemoveEventHandler(EventTypeEnum.EctypeSettleAccount.ToString(), Show);
            m_Instance = null;
            Time.timeScale = 1;
        }
        /// <summary>
        /// boss死亡，开始播放慢镜头
        /// </summary>
        /// <param name="obj"></param>
        void BossDeath(object obj)
        {
           
        }

		void EctypeFinishHandle(INotifyArgs arg)
		{	
			var sMsgActionDie_SC = (SMSGECTYPE_FINISH_SC)arg;
			SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
			m_ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
			if(m_ectypeData.lEctypeType == 0)
			{
				//成功
				if(sMsgActionDie_SC.bySucess==1)
				{
					//Debug.LogWarning("开始慢镜头:" + Time.time);
					StartCoroutine(StartTimeScale(0));
				}
				else
				{
					ShowFaildEff();
				}
			}

			//自动拾取
			StartCoroutine(AllPickUpDelay(m_ectypeData.PickupDelay-3f));
		}

		IEnumerator AllPickUpDelay(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			DamageManager.Instance.PickUpAllEquip();
		}

		//显示副本失败特效
		void ShowFaildEff()
		{
			GameObject effectObj = CreatObjectToNGUI.InstantiateObj(FaildEffectPrefab, CreateFialedEffTransform);
			DoForTime.DoFunForTime(2.0f, p=>{Destroy(effectObj);}, null);
		}

        void Show(INotifyArgs inotifyArgs)
        {
            IsGameSettlement = true;
//            Debug.LogWarning("开始结算:" + Time.time);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.ReceiveSettleAccount,inotifyArgs);
            this.sMSGEctypeSettleAccounts = (SMSGEctypeSettleAccounts)inotifyArgs;
            ShowWinEffect();
        }

        IEnumerator StartTimeScale(int Step)
        {
            Time.timeScale = timeScalList[Step].timeSpeed;
            //TraceUtil.Log(Step + "," + timeScalList[Step].timeSpeed);
            yield return new WaitForSeconds(timeScalList[Step].Time * timeScalList[Step].timeSpeed);
            if (Step >= timeScalList.Length - 1)
            {
                Time.timeScale = 1;
                //this.ectypeSettleAccountsManager = CreatObjectToNGUI.InstantiateObj(EctypeSettleAccountsObj, transform).GetComponent<EctypeSettleAccountsManager>();
                //ectypeSettleAccountsManager.Show(sMSGEctypeSettleAccounts);
            }
            else
            {
                Step++;
                StartCoroutine(StartTimeScale(Step));
            }
        }
        //显示胜利动画
        void ShowWinEffect()
        {
            TraceUtil.Log("显示胜利标题：" + Time.time);
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_BattleWin"); 
            GameObject effectObj = CreatObjectToNGUI.InstantiateObj(WinEffectPrefab, CreatWinEffectTransform);
            DoForTime.DoFunForTime(3 ,PlayWinSkill,effectObj);
        }
        /// <summary>
        /// 显示胜利动作技能
        /// </summary>
        /// <param name="obj"></param>
        void PlayWinSkill(object obj)
        {
            Destroy(obj as GameObject);
            TraceUtil.Log("播放胜利动作:" + Time.time);

			DoForTime.DoFunForTime(EctypeManager.Instance.GetCurrentEctypeData().ResultAppearDelay,ShowSettleAccountsPanel,null);
        }
        /// <summary>
        /// 显示结算面板
        /// </summary>
        void ShowSettleAccountsPanel(object obj)
        {
            TraceUtil.Log("开始显示结算面板：" + Time.time);
            BattleSettlementPanel = CreatObjectToNGUI.InstantiateObj(BattleSettlementPanelPreafab, transform).GetComponent<BattleSettlementPanel_V3>();
            BattleSettlementPanel.Show(sMSGEctypeSettleAccounts);
        }

        [ContextMenu("ShowPanel")]
        void ShowPanel()
        {
            Show(new SMSGEctypeSettleAccounts()
            {
               // dwKillPercent = 89,
                dwHighestCombo = 23,
                sGrade = "SS",
                dwTime = 295713,
                dwGradeExp = 345,
                dwGradeMoney = 3286,
//                RoleAccountList = new System.Collections.Generic.List<SMSGEctypeSettleAccounts2_SC>() { new SMSGEctypeSettleAccounts2_SC(), new SMSGEctypeSettleAccounts2_SC()},
            });
        }
    }

}

[Serializable]
public class TimeScale
{
    public float Time;
    public float timeSpeed;
}