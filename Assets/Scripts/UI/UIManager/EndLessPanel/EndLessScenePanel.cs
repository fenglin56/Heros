using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EndLessScenePanel : MonoBehaviour {
	public GameObject startEnterEff;
	//顶部信息
	public GameObject topParent;
	public UILabel downWaveLabel;
	public UILabel downTimeLabel;
	public UILabel downNpcLabel;
	//波数开始提示
	public GameObject waveStartTip;
	private EndLessRewardTipRun waveStartTipScript;
	//public UILabel waveCountLabel;
	//根据特效给出的时间来做
	private int waveStartTipTime;
	//奖励翻滚提示
	public GameObject waveRewardPrefab;
	public GameObject waveRewardParent;
	private float curDownTime;
	private float preSinceStartUp = 0f;
	private float sinceTime = 0;
	private GameObject firstReward;
	private GameObject secondeReward;
	private bool isJumpWave = false;
	public GameObject jumpSceneTip;
	public UILabel labelTime;
	//波结束的动画
	public GameObject jumpWaveEff;
	//是否在播放奖励
	private bool isTipRunning = false;
	private float jumpWaitTime = 1f;
	private List<int> jumpWaveList = new List<int>();
	public void Init()
	{
		isCanShowWave = false;
		jumpSceneTip.SetActive (false);
		//UIEventManager.Instance.RegisterUIEvent(UIEventType.EndLessNpcCountUpdate, RefreshNpcCount);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.EndLessLoopNumUpdate, OnNewWaveUpdate);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.EndLessPassLoopNumUpdate, OnPassLoopNumEvent);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.EndLessAgainConnectTime, OnAgainConnectEvent);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.EndLessJumpSceneTime, ReceiveSceneJumpUpdate);
		waveRewardPrefab.SetActive (false);
		EctypeModel.Instance.ClearPreEctypeWave ();
		preSinceStartUp = Time.realtimeSinceStartup;
		topParent.SetActive (false);
		waveRewardParent.SetActive (false);

		downTimeLabel.text = string.Format ("{0:d2}:{1:d2}",0,0);
		downNpcLabel.text = "0";
		waveStartTip.SetActive (false);
		//Invoke ("TestTTTT",10);
	}
	private bool isStarted = false;
	void StartDownTime()
	{
		if (isStarted)
			return;
		isStarted = true;
		float tempTime = 0;
		if (EctypeModel.Instance.curLoopNum < 1) {
			tempTime = CommonDefineManager.Instance.CommonDefine.DefenceLevelStartTipDelay/1000f+3f;
		}
		curDownTime = (float)CommonDefineManager.Instance.CommonDefine.EndlessNextWaveTime+tempTime;
		StartInvoke ();
	}
	void StartInvoke()
	{
		StopInvoke ();
		InvokeRepeating("TimeUpdate",0 ,1f);
	}
	void StopInvoke()
	{
		if (IsInvoking ("TimeUpdate")) {
			CancelInvoke("TimeUpdate");
		}
	}
	private bool isAgainConnect = false;
	public void Show()
	{
		//Debug.Log ("Show==="+CommonDefineManager.Instance.CommonDefine.DefenceLevelStartTipDelay/1000f);
		if (EctypeModel.Instance.againConnectTime > 0)
			isAgainConnect = true;
		if (EctypeModel.Instance.curLoopNum > 1) {
			StartEnterEff(); 
		} else {
			Invoke ("StartEnterEff", CommonDefineManager.Instance.CommonDefine.DefenceLevelStartTipDelay / 1000f);
		}
		//StartTipDelay
		//ShowWave ();
	}
	void StartEnterEff()
	{
		if (EctypeModel.Instance.againConnectTime < 0 && EctypeModel.Instance.passLoopNum == 0) {
			SoundManager.Instance.PlaySoundEffect ("Sound_UIEff_EndlessIntro");
			NGUITools.AddChild (gameObject, startEnterEff);
			Invoke ("StartShowWave", 3);
		}
		else {
			StartShowWave();	
		}
	}
	private bool isCanShowWave = false;
	void StartShowWave()
	{
		//Debug.Log ("StartShowWave==isCanShowWave="+isCanShowWave);
		if (!isCanShowWave) {
			isCanShowWave = true;
			return;
		}
		topParent.SetActive (true);
		waveRewardParent.SetActive (true);
	}
	private void ShowWave()
	{
		//Debug.Log ("ShowWave===");
		curDownTime = (float)CommonDefineManager.Instance.CommonDefine.EndlessNextWaveTime;
		downWaveLabel.text = EctypeModel.Instance.curLoopNum.ToString();
		waveStartTip.SetActive (true);
		GameObject go = UI.CreatObjectToNGUI.InstantiateObj (waveStartTip,transform.Find("BottomParent"));
		waveStartTipScript = go.GetComponent<EndLessRewardTipRun>();
		waveStartTipScript.Show (this,EctypeModel.Instance.curLoopNum);
		waveStartTip.SetActive (false);
		//waveCountLabel.text = downWaveLabel.text;
	}

	private void UpdateTime()
	{
		sinceTime = Time.realtimeSinceStartup - preSinceStartUp;
		preSinceStartUp = Time.realtimeSinceStartup;
		curDownTime = curDownTime - sinceTime;
		if (curDownTime <= 0) {
			curDownTime = 0;
			//时间到//
		}
		downTimeLabel.text = string.Format ("{0:d2}:{1:d2}",(int)curDownTime/60,(int)curDownTime%60);
		downNpcLabel.text = MonsterManager.Instance.GetCurSceneActEntityCount ().ToString();
	}
	void NpcAllDie()
	{
		downNpcLabel.text = "0";
	}
	void TimeUpdate()
	{
		UpdateTime ();
	}
	//新一波开始,服务器发
	private bool isNewWave = false;
	//当不是跳波时，倒计时计算要除掉播放奖励时间//
	private bool isNotJumpResetTime = false;
	private void OnNewWaveUpdate(object obj)
	{
		//Debug.Log ("OnNewWaveUpdate==="+EctypeModel.Instance.curLoopNum+"waveRunCount=="+waveRunCount);
		//waveStartTip.SetActive (true);
		StartShowWave ();
		//开始播放新一波开始动画
		PlayerEff ();
		isNotJumpResetTime = false;
		//当第一次时，直接显示第一波开始，其它的要进行判定//
		if (EctypeModel.Instance.curLoopNum == 1 || waveRunCount == 0) {
			ShowWave ();
			StartDownTime ();//DownTimeReset (false,0);
		} else {
			isNewWave = true;
			if(jumpWaveList.Count == 0 && waveRunCount != 0)
			{
				isNotJumpResetTime = true;
				preSinceStartUp = Time.realtimeSinceStartup;
			}
		}
	}
	private void DownTimeReset(bool isStartTime,float newTimeVal)
	{
		if (isStartTime) {
			curDownTime = newTimeVal;
		} else {
			curDownTime = (float)CommonDefineManager.Instance.CommonDefine.EndlessNextWaveTime;
		}
		preSinceStartUp = Time.realtimeSinceStartup;
		UpdateTime ();
	}
	//npc数量更新
	private void RefreshNpcCount(object obj)
	{
		int count = (int)obj;
		downNpcLabel.text = count.ToString ();
	}
	//每次闯过一关时发送
	void OnPassLoopNumEvent(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EndlessReward");
		int curWave = (int)obj;
		//关闭新一波
		//waveStartTip.SetActive (false);
		isJumpWave = false;
		jumpWaveList.Add(curWave);
		if (!isTipRunning) {
			JumpWaveRun ();
		} else {
			isJumpWave = IsEndLessJumpWave();
		}
	}
	//断线重连是服务器先发送，故这里可以判定为断线重连//
	void OnAgainConnectEvent(object obj)
	{
		int timeVal = (int)obj/1000;
		DownTimeReset (true,timeVal);
	}
	//前一波
	int preLoopNum = -1;
	//跳波时，显示速度惊人
	void PlayerEff()
	{
		int loopNum = EctypeModel.Instance.curLoopNum - preLoopNum;
		//非断线重连//
		if (loopNum > 1 && preLoopNum != -1) {
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EndlessSkip");
			GameObject go = NGUITools.AddChild (gameObject, jumpWaveEff);
			EndLessPassWaveEff eff = go.GetComponent<EndLessPassWaveEff> ();
			eff.Show (string.Format (LanguageTextManager.GetString ("IDS_I20_16"), EctypeModel.Instance.curLoopNum),loopNum); 
		}
		preLoopNum = EctypeModel.Instance.curLoopNum;
	}
	//判定当前是否跳波(上一波)
	public bool IsEndLessJumpWave()
	{
		//当跳波时,至少有两波提示，第二波loopNumList是一定不会存的，断线除外//
		/*if (isAgainConnect || (!isAgainConnect && EctypeModel.Instance.loopNumList.Contains (EctypeModel.Instance.passLoopNum - 1))) {
			//断线只能在第一次起作用//
			isAgainConnect = false;
			return false;		
		}*/
		//Debug.Log ("IsEndLessJumpWave==passLoopNum="+EctypeModel.Instance.passLoopNum +"curLoopNum"+EctypeModel.Instance.curLoopNum);
		//只要发现当前通过关数大于当前波数，即为跳波
		if(EctypeModel.Instance.passLoopNum > EctypeModel.Instance.curLoopNum)
			return true;
		return false;
	}
	private int prePassLoopNum = 0;
	void JumpWaveRun()
	{
		if (jumpWaveList.Count == 0) {
			isTipRunning = false;
			return;
		}
		if (isJumpWave) {
			StopInvoke ();
			NpcAllDie();
		}
		CreateWaveRun (jumpWaveList [0]);
		jumpWaveList.RemoveAt (0);
		Invoke("JumpWaveRun",jumpWaitTime);	
	}
	/*void DealWaveRun()
	{
		if (jumpWaveList.Count == 0) {
			isJumping = false;
			return;
		}
		if (IsEndLessJumpWave (prePassLoopNum)) {
			StopInvoke ();
		}
		CreateWaveRun (jumpWaveList [0]);
		jumpWaveList.RemoveAt (0);
		Invoke("DealWaveRun",jumpWaitTime);
	}*/
	private int waveRunCount = 0;
	//
	void CreateWaveRun(int loopNum)
	{
		if (loopNum <= 0)
			return;
		prePassLoopNum = loopNum;
		isTipRunning = true;
		waveRunCount++;
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EndlessReward");
		waveRewardPrefab.SetActive (true);
		firstReward = NGUITools.AddChild (waveRewardParent,waveRewardPrefab);
		waveRewardPrefab.SetActive (false);
		EndLessRewardTipRun tipRun = firstReward.GetComponent<EndLessRewardTipRun>();
		List<CGoodsInfo> rewardGoods = EctypeModel.Instance.GetRewardByLoopNum (loopNum);
		tipRun.Show (this,loopNum,rewardGoods[0].itemID,rewardGoods[0].itemCount);
	}
	//当翻滚奖励条结束时，把回调发过来//
	public void RewardTipFinish()
	{
		waveRunCount--;
		//如果奖励列表小于0，且为存在新波发送过来，此时要进行播新的一波开始//
		if (isNewWave && waveRunCount == 0) {
			isNewWave = false;
			if(isNotJumpResetTime)
			{
				float sinceTime = (Time.realtimeSinceStartup-preSinceStartUp);
				if(sinceTime < 0)
				{
					sinceTime = 0;
				}
				curDownTime = (float)CommonDefineManager.Instance.CommonDefine.EndlessNextWaveTime - sinceTime;
				DownTimeReset (true,curDownTime);
			}
			else
			{
				DownTimeReset (false,0);
			}
			isNotJumpResetTime = false;
			ShowWave ();
			StartInvoke();
		}
	}
	private float preJumpSceneSinceTime ;
	private float jumpSceneTime;
	private void ReceiveSceneJumpUpdate(object obj)
	{
		int tempTime = (int)obj;
		jumpSceneTime = (float)tempTime;
		jumpSceneTip.SetActive (true);
		if (IsInvoking ("JumpSceneUpdate")) {
			CancelInvoke("JumpSceneUpdate");
		}
		Debug.Log ("teimeeee===="+tempTime);
		labelTime.text = string.Format(LanguageTextManager.GetString ("IDS_I20_25"),(int)jumpSceneTime);
		preJumpSceneSinceTime = Time.realtimeSinceStartup;
		InvokeRepeating ("JumpSceneUpdate",0,0.5f);
	}
	void JumpSceneUpdate()
	{
		float jumpTime = Time.realtimeSinceStartup - preJumpSceneSinceTime;
		preJumpSceneSinceTime = Time.realtimeSinceStartup;
		jumpSceneTime = jumpSceneTime - jumpTime;
		if (jumpSceneTime <= 0) {
			jumpSceneTime = 0;
			//时间到//
			CancelInvoke("JumpSceneUpdate");
		}
		labelTime.text = string.Format(LanguageTextManager.GetString ("IDS_I20_25"),(int)jumpSceneTime);
	}
	void OnDestroy()
	{
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EndLessNpcCountUpdate, RefreshNpcCount);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EndLessLoopNumUpdate, OnNewWaveUpdate);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EndLessPassLoopNumUpdate, OnPassLoopNumEvent);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EndLessAgainConnectTime, OnAgainConnectEvent);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EndLessJumpSceneTime, ReceiveSceneJumpUpdate);
		if (IsInvoking ("TimeUpdate")) {
			CancelInvoke("TimeUpdate");
		}
		if (IsInvoking ("TimeUpdate")) {
			CancelInvoke("TimeUpdate");
		}
	}
}