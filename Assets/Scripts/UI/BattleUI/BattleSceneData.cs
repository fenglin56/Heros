using UnityEngine;
using System.Collections;
/// <summary>
/// 后端用副本离开时的统计消息
/// </summary>
public class BattleSceneData : View {
	
	private static BattleSceneData instance;
	
	
	float nDelay;				//副本平均延迟
	int nFrames;			//副本平均帧数
	int nSkillBreak;		//技能打断次数
	int nAPM;				//副本APM值
	int nLoadingTime;		//关卡loading时间
	int[] ClickBtnInfoList; //按钮列表
	
	int GetDataNumber = 0;//获取数据次数
	private float CurrentSceneTime;
	
	public static BattleSceneData Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject newObj = new GameObject();
				newObj.name = "BattleSceneData";
				newObj.AddComponent<DontDestroy>();
				instance = newObj.AddComponent<BattleSceneData>();
			}
			return instance;
		}
	}
	
	void Awake()
	{
		//TraceUtil.Log("InieSceneData");
		ClickBtnInfoList = new int[8];
		UIEventManager.Instance.RegisterUIEvent(UIEventType.LoadingComplete, OnScenesLoadComplete);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.AddAPMNumber, AddAPMNumber);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.AddBattleButtonClickNumber, AddBattleButtonClickNumber);
		AddEventHandler(EventTypeEnum.SceneChange.ToString(), DestroyMySelf);
		AddEventHandler(EventTypeEnum.SkillBreakForStatistics.ToString(), AddSkillBreakNumber);
		//AddEventHandler(EventTypeEnum.EctypeSettleAccount.ToString(), BeginBattleSettle);
		AddEventHandler(EventTypeEnum.EctypeBattleStatistics.ToString(), BeginBattleSettle);
	}
	
	protected override void RegisterEventHandler ()
	{
	}
	
	public void InitMySelf()
	{
		CurrentSceneTime = Time.realtimeSinceStartup;
	}
	
	void OnScenesLoadComplete(object obj)
	{
		nLoadingTime = (int)((Time.realtimeSinceStartup - CurrentSceneTime)*1000);
		InvokeRepeating("UpdateData", 0.1f, 2);
		//TraceUtil.Log("LoadSceneComplete:" + nLoadingTime);
	}
	
	void UpdateData()
	{
		GetDataNumber++;
		float currentDelay = HeartFPSManager.Instance.GetSamplerDeltTime();
		nDelay = (nDelay * (GetDataNumber - 1) + currentDelay) / (float)GetDataNumber;
		//TraceUtil.Log("UpdateDelay:" + nDelay);
		float currentFrames = 1 / Time.deltaTime;
		nFrames = (int)(nFrames * (GetDataNumber - 1) + currentFrames) / GetDataNumber;
		//TraceUtil.Log("UpdateFarmes:" + nFrames);
	}
	
	public void AddSkillBreakNumber(INotifyArgs inotifyArgs)
	{
		nSkillBreak++;
	}
	
	public void AddAPMNumber(object obj)
	{
		nAPM++;
	}
	
	public void AddBattleButtonClickNumber(object obj)
	{
		//TraceUtil.Log("UpdateSkillBtnCLickNum");

		//这里之所以使用了判断是因为，普通的攻击做了特殊的处理
		UI.Battle.BattleSkillButton buttonInstance = obj as UI.Battle.BattleSkillButton;
		UI.Battle.SpecialSkillType buttonType = UI.Battle.SpecialSkillType.Normal;

		if(buttonInstance == null)
		{
			buttonType = (UI.Battle.SpecialSkillType)obj;
		}
		else
		{
			buttonType = buttonInstance.SpecialType;
		}
	
		switch (buttonType)
		{
		case UI.Battle.SpecialSkillType.Normal:
			ClickBtnInfoList[buttonInstance.MyBtnIndex]++;
			break;
		case UI.Battle.SpecialSkillType.NormalBtn: // 普通攻击
			ClickBtnInfoList[4]++;
			break;
		case UI.Battle.SpecialSkillType.Roll://翻滚
			ClickBtnInfoList[5]++;
			break;
		case UI.Battle.SpecialSkillType.Explode://爆气
			ClickBtnInfoList[6]++;
			break;
		case UI.Battle.SpecialSkillType.Meaning://奥义
			ClickBtnInfoList[7]++;
			break;
		default:
			break;
		}
	}
	
	/// <summary>
	/// 开始战斗结算, 向后台发送消息
	/// </summary>
	/// <param name="inotifyArgs">Inotify arguments.</param>
	void BeginBattleSettle(INotifyArgs inotifyArgs)
	{
		SMSGECTYPELEVEL_COLLECTINFO_CS sendData = new SMSGECTYPELEVEL_COLLECTINFO_CS()
		{
			nDelay = nDelay,
			nFrames = nFrames,
			nSkillBreak = nSkillBreak,
			nAPM = nAPM,
			nLoadingTime = nLoadingTime,
			byButtonNum = (byte)ClickBtnInfoList.Length,
			ClickBtnInfoList = ClickBtnInfoList,
		};

		NetServiceManager.Instance.EctypeService.SendBattleCollectToSever(sendData);

		TraceUtil.Log(SystemModel.Jiang,"SendBattleDataToSever");
	}
	
	void DestroyMySelf(INotifyArgs inotifyArgs)
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.LoadingComplete, OnScenesLoadComplete);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.AddAPMNumber, AddAPMNumber);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.AddBattleButtonClickNumber, AddBattleButtonClickNumber);
		RemoveEventHandler(EventTypeEnum.SceneChange.ToString(), DestroyMySelf);
		RemoveEventHandler(EventTypeEnum.SkillBreakForStatistics.ToString(), AddSkillBreakNumber);
		RemoveEventHandler(EventTypeEnum.EctypeSettleAccount.ToString(), BeginBattleSettle);
		Destroy(gameObject);
	}
} 