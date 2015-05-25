using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.Crusade
{

	public class CrusadeSettlementManager : View
	{
		//public CrusadeSettlementPanel CrusadeSettlementPanel;
		public GameObject CrusadeSettlementPanelPrefab;
		public GameObject Eff_CrusadeBattleStart;
		public GameObject FailEffectPrefab;	//失败特效
		public Transform CreateFailEffTransform;	//失败特效出点位置 

		public GameObject Timing;
		public UILabel Label_Timing;

		private bool m_isTiming = false;
		private float m_time = 0;

		public TimeScale[] timeScalList;	//
		//const float SCALE_SPEED = 0.1f;
		//const float SCALE_TIME = 0.3f;	//慢镜两秒

		private EctypeContainerData m_ectypeData;
		private SMSGECTYPE_CRUSADERESULT_SC m_CrusadeResult;
		private List<IEntityDataStruct> m_DataStruceList = new List<IEntityDataStruct>();

		void Start()
		{
			if(GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
				return;

			SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
			var ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];

			if(ectypeData.lEctypeType==9)
			{
				//初始化讨伐副本
				UI.CreatObjectToNGUI.InstantiateObj(Eff_CrusadeBattleStart,transform);
				Timing.SetActive(true);

				StartCoroutine("LateTiming");				
				RegisterEventHandler();

				//储存所有玩家信息
				PlayerManager.Instance.PlayerList.ApplyAllItem(p=>{
					IEntityDataStruct ds = p.EntityDataStruct;
					m_DataStruceList.Add(ds);
				});
			}

		}

		IEnumerator LateTiming()
		{
			yield return new WaitForSeconds(5f);
			m_isTiming = true;
		}

		void ReceiveCrusadeResultHandle(object obj)
		{
			m_isTiming = false;
			m_CrusadeResult = (SMSGECTYPE_CRUSADERESULT_SC)obj;
		}
		void ReceiveCrusadeTimingHandle(object obj)
		{
			m_time = ((SMSGECTYPE_CRUSADETIME_SC)obj).dwTime/1000f;
			m_isTiming = true;
		}

		void FixedUpdate()
		{
			if(m_isTiming)
			{
				m_time += Time.fixedDeltaTime;

				Label_Timing.text = ToClock(m_time);
			}
		}

		private string ToClock(float time)
		{
			int min = (int)(time/60);
			int sec = (int)(time%60);
			return ToString(min)+":"+ToString(sec);
		}
		private string ToString(int number)
		{
			string str = "";
			if(number <= 9)
			{
				str = "0"+number.ToString();
			}
			else
			{
				str = number.ToString();
			}
			return str;
		}
//
//		void BossDeath(object obj)
//		{
//			TraceUtil.Log(SystemModel.Lee,"开始慢镜头:" + Time.time);
//			StartCoroutine("StartTimeScale");
//		}

		/*
		IEnumerator StartTimeScale()
		{
			Time.timeScale = SCALE_SPEED;
			yield return new WaitForSeconds(SCALE_TIME);
			Time.timeScale = 1;
			yield return new WaitForSeconds(EctypeManager.Instance.GetCurrentEctypeData().ResultAppearDelay);
			GameObject panelObj = UI.CreatObjectToNGUI.InstantiateObj(CrusadeSettlementPanelPrefab, transform);
			CrusadeSettlementPanel crusadeSettlementPanel = panelObj.GetComponent<CrusadeSettlementPanel>();
//			if(m_CrusadeResult != null)
//			{
//				crusadeSettlementPanel.Show(m_CrusadeResult);
//			}
			crusadeSettlementPanel.Show(m_DataStruceList, m_CrusadeResult);
			yield return null;
		}
		*/
		
		IEnumerator StartTimeScale(int Step)
		{
			Time.timeScale = timeScalList[Step].timeSpeed;
			//TraceUtil.Log(Step + "," + timeScalList[Step].timeSpeed);
			yield return new WaitForSeconds(timeScalList[Step].Time * timeScalList[Step].timeSpeed);
			if (Step >= timeScalList.Length - 1)
			{
				Time.timeScale = 1;

				yield return new WaitForSeconds(EctypeManager.Instance.GetCurrentEctypeData().ResultAppearDelay);
				GameObject panelObj = UI.CreatObjectToNGUI.InstantiateObj(CrusadeSettlementPanelPrefab, transform);
				CrusadeSettlementPanel crusadeSettlementPanel = panelObj.GetComponent<CrusadeSettlementPanel>();
				crusadeSettlementPanel.Show(m_DataStruceList, m_CrusadeResult);
			}
			else
			{
				Debug.Log("StartTimeScale time = " + timeScalList[Step].Time + " timeSpeed = " + timeScalList[Step].timeSpeed);
				Step++;
				StartCoroutine(StartTimeScale(Step));
			}
		}

		void EctypeFinishHandle(INotifyArgs arg)
		{	
			var sMsgActionDie_SC = (SMSGECTYPE_FINISH_SC)arg;
			SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
			m_ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
			if(m_ectypeData.lEctypeType == 9)
			{
				//成功
				if(sMsgActionDie_SC.bySucess==1)
				{
					Debug.Log("CrusadeSettlementManager StartTimeScale");
					TraceUtil.Log(SystemModel.Lee,"开始慢镜头:" + Time.time);
					//StartCoroutine("StartTimeScale");
					StartCoroutine(StartTimeScale(0));
				}
				else
				{
					ShowFailEff();
				}
			}

		}

		
		//显示副本失败特效
		void ShowFailEff()
		{
			GameObject effectObj = CreatObjectToNGUI.InstantiateObj(FailEffectPrefab, CreateFailEffTransform);
			DoForTime.DoFunForTime(1.0f, p=>{Destroy(effectObj);}, null);
		}

		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CrusadeSettlement,ReceiveCrusadeResultHandle);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CrusadeTiming,ReceiveCrusadeTimingHandle);
			//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.BossDeathMsg,BossDeath);
			RemoveEventHandler(EventTypeEnum.EctypeFinish.ToString(),EctypeFinishHandle);
		}

		protected override void RegisterEventHandler()
		{
			UIEventManager.Instance.RegisterUIEvent(UIEventType.CrusadeSettlement, ReceiveCrusadeResultHandle);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.CrusadeTiming,ReceiveCrusadeTimingHandle);
			//UIEventManager.Instance.RegisterUIEvent(UIEventType.BossDeathMsg,BossDeath);
			AddEventHandler(EventTypeEnum.EctypeFinish.ToString(),EctypeFinishHandle);

		}

	}
}
