using UnityEngine;
using System.Collections;

public class EndLessSceneManager : MonoBehaviour {
	public GameObject endLessScenePanel;
	public GameObject endLessSceneResultPanel;
	private EndLessScenePanel elScenePanel;
	private EndLessSceneResultPanel elResultPanel;
	private bool isOpenEndLess = false;
	public GameObject bossAppearEffPrefab;
	private GameObject bossAppearEff;
	private float screenPrecent;
	// Use this for initialization
	void Start () {
		if (GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
		{
			GameDataManager.Instance.dataEvent.RegisterEvent(DataType.InitializeEctype, InitReadEctypeType);
		}
		else
		{
			InitReadEctypeType(null);
		}
		screenPrecent = Screen.width*1f/Screen.height ;
		UIEventManager.Instance.RegisterUIEvent(UIEventType.OnBossShowEvent, OnBossShowEvent);
		//Invoke ("TTTTTT",10);
	}
	//test
	void TTTTTT()
	{
		OnEctypeFinishEvent (null);
	}

	void InitReadEctypeType(object obj)
	{
		GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.InitializeEctype, InitReadEctypeType);
		SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
		EctypeContainerData m_ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[sMSGEctypeInitialize_SC.dwEctypeContainerId];
		isOpenEndLess = m_ectypeData.lEctypeType==10;   //防守副本
		if (m_ectypeData.lEctypeType == 10) {
			isOpenEndLess = true;
			Init();
		} else {
			isOpenEndLess = false;		
		}
	}

	private void Init()
	{
		CreateEndLessPanel ();
		UIEventManager.Instance.RegisterUIEvent(UIEventType.EndLessFinishPassLoopData, OnEctypeFinishEvent);
	}
	//当无尽副本结束时
	private void OnEctypeFinishEvent(object obj)
	{
		if (!isOpenEndLess || !gameObject.activeInHierarchy)
			return;
		CreateEndLessResultPanel ();
	}
	
	private void CreateEndLessPanel()
	{
		GameObject panel;
		if (elScenePanel == null) {
			panel = UI.CreatObjectToNGUI.InstantiateObj(endLessScenePanel,transform);
			elScenePanel = panel.GetComponent<EndLessScenePanel>();
			elScenePanel.Init ();
		}
		elScenePanel.Show ();
	}
	private void CreateEndLessResultPanel()
	{
		GameObject panel;
		if (elResultPanel == null) {
			panel = UI.CreatObjectToNGUI.InstantiateObj(endLessSceneResultPanel,transform);
			elResultPanel = panel.GetComponent<EndLessSceneResultPanel>();
			elResultPanel.Init ();
		}
		elResultPanel.Show ();
		DestroyImmediate (elScenePanel.gameObject);
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EndlessResultIntro");
	}
	void OnBossShowEvent(object obj)
	{
		bossAppearEff = NGUITools.AddChild (gameObject,bossAppearEffPrefab);
		//Debug.Log ("222W="+Screen.width+"H="+Screen.height+"screenPrecent="+screenPrecent);
		Transform bossAppearScreen = bossAppearEff.transform.Find ("GameObject/Eff_jinggaobianyuan_01");
		Vector3 bossAppearOriScale = bossAppearScreen.localScale;
		bossAppearScreen.localScale = new Vector3 (bossAppearOriScale.y*screenPrecent,bossAppearOriScale.y,bossAppearOriScale.z);
		//Screen.width*1f/Screen.height
	}
	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OnBossShowEvent, OnBossShowEvent);
		if (isOpenEndLess) {
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EndLessFinishPassLoopData, OnEctypeFinishEvent);
			if(elResultPanel != null)
			{
				DestroyImmediate (elResultPanel.gameObject);
			}
		}
	}
}
