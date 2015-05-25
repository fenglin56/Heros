using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UI.Battle;

public class DefencePanel :View
{
	public GameObject BossStatusForDefence;
	public GameObject DefenceBattleStart;   //防守副本中心事件提示（开始）
	public GameObject DefenceBattleFailed;   //防守副本中心事件提示（失败）
	public GameObject DownTips;
	public GameObject FailEffectPrefab;	//副本失败特效
	public Transform CreateFialEffPrefab;	//失败特效出现位置 
	public TimeScale[] TimeScalList;
	public GameObject Center;
	public GameObject TopRightPos;
	public GameObject Prefab_DefenceSettleManager;

	public Animation PanelAnim;

	[HideInInspector]
	public bool ISDefenceEctype;// 是否防守副本
	[HideInInspector]
	public Dictionary<int,EntityModel> RoadblockEntityModes=new Dictionary<int,EntityModel>();

	private UILabel m_downTipsLabel;
	private GameObject m_upTips;
	private UILabel m_upTipsLabel;
	private float m_roadblockTipsCD;
	private float m_bossTipsCD;
	private float m_commonDefineCDTime;  //秒
	private float m_tipsDispearTime;
	private float m_maxLoopNum,m_currentLoop;  //最大波数
	private BossStatusForDefencePanel m_bossStatusForDefencePanel;
	private static DefencePanel m_instance;
	private EctypeContainerData m_ectypeData;
	private bool m_waitingDefenceEctypeData=false;

	public static DefencePanel Instance
	{
		get
		{
			if(m_instance==null)
			{
				m_instance=GameObject.FindObjectOfType<DefencePanel>();
			}
			return m_instance;
		}
	}

	void Awake()
	{
		m_instance=this;

		RegisterEventHandler();
		if (GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
		{
			GameDataManager.Instance.dataEvent.RegisterEvent(DataType.InitializeEctype, Init);
		}
		else
		{
			Init(null);
		}
		m_tipsDispearTime=CommonDefineManager.Instance.CommonDefine.DefenceLevelTipsAppearTime;
		m_commonDefineCDTime=CommonDefineManager.Instance.CommonDefine.DefenceLevelTipsCoolDown/1000f;
		m_downTipsLabel=DownTips.GetComponentInChildren<UILabel>();


		DownTips.SetActive(false);  
	}
	void Update()
	{
		if(m_roadblockTipsCD<m_commonDefineCDTime)
		{
			m_roadblockTipsCD+=Time.deltaTime;
		}
		if(m_bossTipsCD<m_commonDefineCDTime)
		{
			m_bossTipsCD+=Time.deltaTime;
		}
	}

	/// <summary>
	/// 收到死亡消息，对应的路障标记打上叉
	/// </summary>
	/// <param name="inotifyArgs">Inotify arguments.</param>
	public void ReceiveEntityDieHandle(INotifyArgs inotifyArgs)
	{
		if(ISDefenceEctype)
		{
			var sMsgActionDie_SC = (SMsgActionDie_SC)inotifyArgs;
			foreach(var monster in this.RoadblockEntityModes)
			{
				if(monster.Value.EntityDataStruct.SMsg_Header.uidEntity==sMsgActionDie_SC.uidEntity)
				{
					m_bossStatusForDefencePanel.RoadblockDamagedIcon[monster.Key].SetActive(true);
					//路障被摧毁事件提示
                    SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DefenceDownTips");
					StartCoroutine(ShowEventTips(true,"IDS_I15_10"));
					break;
				}
			}           
		}
	}
	/// <summary>
	/// 刷路障怪时，注册进来
	/// </summary>
	/// <param name="roadblockModel">Roadblock model.</param>
	/// <param name="monsterSubType">Monster sub type.只能为3，4，5</param> 
	private void SetRoadblockDataModel(EntityModel roadblockModel,int index,string tipIDS)
	{
        if (RoadblockEntityModes.ContainsKey(index))
        {
            RoadblockEntityModes.Remove(index);
        }
		roadblockModel.tipIDS = tipIDS;
		RoadblockEntityModes.Add(index,roadblockModel);
		if(m_bossStatusForDefencePanel != null)
       	 	m_bossStatusForDefencePanel.RoadblockDamagedIcon[index].SetActive(false);
	}
	public void DefenceMonsterCreated(EntityModel roadblockModel,MonsterConfigData monsterData)
	{
		int index = 0;
		foreach (var block in m_ectypeData.DefenceLevel_BlockMap) {
			if(block.Key == monsterData._monsterID)
			{
				SetRoadblockDataModel(roadblockModel,index,block.Value);  //路障怪
				return;
			}
			index++;
		}
		/*for(int i=0;i<m_ectypeData.DefenceLevel_BlockMap.Count;i++)
		{
			if(m_ectypeData.DefenceLevel_Block[i]==monsterData._monsterID)
			{
				SetRoadblockDataModel(roadblockModel,i);  //路障怪

				return;
			}
		}*/
		if (DefencePanel.Instance.ISDefenceEctype) {
			//敌方怪
			if (!string.IsNullOrEmpty (monsterData._upTips) && monsterData._upTips != "0") {
				StartCoroutine (ShowEventTips (false, monsterData._upTips));
			}
			if (!string.IsNullOrEmpty (monsterData._downTips) && monsterData._downTips != "0") {
				StartCoroutine (ShowEventTips (true, monsterData._downTips));
			}
		}
	}
	public void EctypeFinishHandle(INotifyArgs inotifyArgs)
	{
        if (ISDefenceEctype)
        {
            if (BattleUIManager.Instance != null)
            {
                BattleUIManager.Instance.SysSettingButton.Enable = false;
            }
			//新增 战斗失败 特效
			var sMsgActionDie_SC = (SMSGECTYPE_FINISH_SC)inotifyArgs;
			if(sMsgActionDie_SC.bySucess==0)
			{
				ShowFailEff();
			}
			else
			{
           
				StartCoroutine(StartTimeScale(0));
            	StartCoroutine(ShowSettleAccount(inotifyArgs));
			}
        }
	}
	private IEnumerator ShowSettleAccount(object arg)
	{
        yield return new WaitForSeconds(3);

		var sMsgActionDie_SC = (SMSGECTYPE_FINISH_SC)arg;
		Debug.Log("防守副本结算："+sMsgActionDie_SC.bySucess);
		//成功，失败
		if(sMsgActionDie_SC.bySucess==0)
		{
			//失败
			StartCoroutine(ShowDefenceEctypeIcon("Sound_UIEff_DefenceDownTips",2,30));

		}
		else
		{
			var peekData=GameDataManager.Instance.PeekData(DataType.DefenceEctypeResult);
			if(peekData!=null)
			{
				//成功并结算数据已到达,进入结算
				var defenceSettleManager=NGUITools.AddChild(gameObject,Prefab_DefenceSettleManager);
				defenceSettleManager.transform.localPosition=new Vector3(0,0,-30);//结算界面置前
				var settleBehaviour=defenceSettleManager.GetComponent<DefenceSettleManager>();
				settleBehaviour.PanelSharkAnim=PanelAnim;
				settleBehaviour.Init(m_ectypeData.lEctypeContainerID);
			}
			else
			{
				m_waitingDefenceEctypeData=true;
			}
		}
	}

	/// <summary>
	/// Defences the boss create.
	/// </summary>
	/// <param name="monsterDataModel">Monster data model.</param>
	void ShowFailEff()
	{
		GameObject effectObj = CreatObjectToNGUI.InstantiateObj(FailEffectPrefab, CreateFialEffPrefab);
		DoForTime.DoFunForTime(2.0f, p=>{Destroy(effectObj);}, null);
	}

	public void DefenceBossCreate(EntityModel monsterDataModel)
	{
		if (!MonsterManager.Instance.IsBossStatusPanelInit())
		{
			GameObject bossStatusPanel =NGUITools.AddChild( TopRightPos,BossStatusForDefence);
			bossStatusPanel.transform.localPosition = Vector3.zero;
			bossStatusPanel.transform.localScale = Vector3.one;
			//Edit by lee
			bossStatusPanel.transform.parent = UI.Battle.BattleUIManager.Instance.GetScreenTransform(UI.Battle.ScreenPositionType.TopRight);
			//赋值
			m_bossStatusForDefencePanel = bossStatusPanel.GetComponent<BossStatusForDefencePanel>();
			MonsterManager.Instance.SetBossStatusPanel(m_bossStatusForDefencePanel);

            SetMonsterLoopNum();

			m_bossStatusForDefencePanel.SetBloodNum(monsterDataModel);

			m_upTips=m_bossStatusForDefencePanel.SpecProperty;
			m_upTipsLabel=m_upTips.GetComponentInChildren<UILabel>();
			m_upTips.SetActive(false);

            //初始路障都打叉，在路障刷出来的时候点亮
            for(int i=0;i<m_bossStatusForDefencePanel.RoadblockDamagedIcon.Length;i++)
            {
                if (!RoadblockEntityModes.ContainsKey(i))
                {
                    m_bossStatusForDefencePanel.RoadblockDamagedIcon[i].SetActive(true);
                }
            }
		}
		else
		{
			BossStatusPanel_V3 bossStatusPanelScript = MonsterManager.Instance.GetBossStatusPanel();
			bossStatusPanelScript.SetDataModel(monsterDataModel);
		}
	}
	/// <summary>
	/// 检测路障被攻击事件
	/// </summary>
	/// <param name="inotifyArgs">Inotify arguments.</param>
	public void RoadblockBeHurt(INotifyArgs inotifyArgs)//设置各种属性
	{
		SMsgBattleCalculateEffect_SC fightResult = (SMsgBattleCalculateEffect_SC)inotifyArgs;
		if (ISDefenceEctype) {
			if (MonsterManager.Instance.IsMonsterBossType (fightResult.uidFighter)) {
				//提示大门正在受到攻击
				if (m_bossTipsCD >= m_commonDefineCDTime) {
					SoundManager.Instance.PlaySoundEffect ("Sound_UIEff_DefenceDownTips");
					StartCoroutine (ShowEventTips (true, "IDS_I15_11"));
					m_bossTipsCD = 0;
				}
				return;
			}
		}
		//这个所有副本//
		foreach(var monster in this.RoadblockEntityModes.Values)
		{
			if(monster.EntityDataStruct.SMsg_Header.uidEntity==fightResult.uidFighter)
			{
				//显示 路障正在被攻击 事件提示
				if(m_roadblockTipsCD>=m_commonDefineCDTime)
				{
					SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DefenceDownTips");
					StartCoroutine(ShowEventTips(true,monster.tipIDS));//"IDS_I15_9"));
					m_roadblockTipsCD=0;
				}
				break;
			}
		}
	}
	/// <summary>
	/// 事件提示
	/// </summary>
	/// <returns>The event tips.</returns>
	/// <param name="downOrUp">true表示下面提示面板， false表示上面提示面板</param>
	/// <param name="IDS">Language IDS</param>
	/// <param name="time">显示时长</param>
	private IEnumerator ShowEventTips(bool downOrUp,string IDS)
	{
		GameObject tipsPanel=null;
		if(downOrUp)
		{
			if(DownTips!=null)
			{
				DownTips.SetActive(true);
				m_downTipsLabel.text=LanguageTextManager.GetString(IDS);
				tipsPanel=DownTips;
			}
		}
		else
		{
			if(m_upTips!=null)
			{
				m_upTips.SetActive(true);
				m_upTipsLabel.text=LanguageTextManager.GetString(IDS);
				tipsPanel=m_upTips;
			}
		}
		yield return new WaitForSeconds(m_tipsDispearTime);
		if(tipsPanel!=null)
		{
			tipsPanel.SetActive(false);
		}
	}
	void Init(object obj)
	{
		GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.InitializeEctype, Init);
		SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
		m_ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
		ISDefenceEctype = m_ectypeData.lEctypeType==8;   //防守副本
		if (ISDefenceEctype)
		{
			LoadSceneData loadSceneData = GameDataManager.Instance.PeekData(DataType.LoadingSceneData) as LoadSceneData;
			var LoadSceneInfo = (SMsgActionNewWorld_SC)loadSceneData.LoadSceneInfo;
			if (m_ectypeData.vectMapID.Split('+')[0] == LoadSceneInfo.dwMapId.ToString())
			{
				StartCoroutine(ShowDefenceEctypeIcon("Sound_UIEff_DefenceUpTips",1,3));
			}
		}
	}
	void OnDestroy()
	{
		RemoveEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);  //实体死亡消息处理
		RemoveEventHandler(EventTypeEnum.S_CSMsgFightFightToResult.ToString(), RoadblockBeHurt);  //角色受击更
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.DefenceLoopNum,DefenceLoopNumHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.DefenceMaxLoopNum,DefenceMaxLoopNumHandle);
		RemoveEventHandler(EventTypeEnum.EctypeFinish.ToString(),EctypeFinishHandle);
		RemoveEventHandler(EventTypeEnum.DefenceEctypeSettleAccount.ToString(),DefenceEctypeSettleAccountHandle);
		m_instance=null;
	}
	/// <summary>
	/// 显示战斗提示信息 1，战斗开始，2 战斗失败
	/// </summary>
	/// <returns>The defence ectype icon.</returns>
	/// <param name="sound">Sound.</param>
	/// <param name="spriteId">1，战斗开始，2 战斗失败</param>
	/// <param name="waitSeconds">Wait seconds.</param>
	IEnumerator ShowDefenceEctypeIcon(string sound,int spriteId,float waitSeconds)
	{
		if(spriteId==1)  //战斗开始，图片需要延时出现
		{
			yield return new WaitForSeconds(CommonDefineManager.Instance.CommonDefine.DefenceLevelStartTipDelay/1000f);
		}

		SoundManager.Instance.PlaySoundEffect(sound);//
		var effct=NGUITools.AddChild(Center,spriteId==1?DefenceBattleStart:DefenceBattleFailed);
		yield return new WaitForSeconds(waitSeconds);
		GameObject.Destroy(effct);
	}
	IEnumerator StartTimeScale(int Step)
	{
		Time.timeScale = TimeScalList[Step].timeSpeed;
		yield return new WaitForSeconds(TimeScalList[Step].Time * TimeScalList[Step].timeSpeed);
		if (Step >= TimeScalList.Length - 1)
		{
			Time.timeScale = 1;
		}
		else
		{
			Step++;
			StartCoroutine(StartTimeScale(Step));
		}
	}
	private void DefenceEctypeSettleAccountHandle(INotifyArgs args)
	{
		if(m_waitingDefenceEctypeData)
		{
			m_waitingDefenceEctypeData=false;
			//成功并结算数据已到达,进入结算
			var defenceSettleManager=NGUITools.AddChild(gameObject,Prefab_DefenceSettleManager);
			var settleBehaviour=defenceSettleManager.GetComponent<DefenceSettleManager>();
			settleBehaviour.PanelSharkAnim=PanelAnim;
			settleBehaviour.Init(m_ectypeData.lEctypeContainerID);
		}
	}
	private void SetMonsterLoopNum()
	{
        if (m_bossStatusForDefencePanel != null)
        {
            m_bossStatusForDefencePanel.FightRound.text = string.Format("{0}/{1}", m_currentLoop, m_maxLoopNum);
        }
	}
		private void DefenceLoopNumHandle(object arg)
		{
		m_currentLoop=(uint)arg;
		SetMonsterLoopNum();
		}
		private void DefenceMaxLoopNumHandle(object arg)
		{
		m_maxLoopNum=(uint)arg;
		SetMonsterLoopNum();
		}
	protected override void RegisterEventHandler()
	{
		AddEventHandler(EventTypeEnum.EntityDie.ToString(), ReceiveEntityDieHandle);  //实体死亡消息处理
		AddEventHandler(EventTypeEnum.S_CSMsgFightFightToResult.ToString(), RoadblockBeHurt);  //角色受击更新
		UIEventManager.Instance.RegisterUIEvent(UIEventType.DefenceLoopNum,DefenceLoopNumHandle);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.DefenceMaxLoopNum,DefenceMaxLoopNumHandle);
		AddEventHandler(EventTypeEnum.EctypeFinish.ToString(),EctypeFinishHandle);
		AddEventHandler(EventTypeEnum.DefenceEctypeSettleAccount.ToString(),DefenceEctypeSettleAccountHandle);
	}
}