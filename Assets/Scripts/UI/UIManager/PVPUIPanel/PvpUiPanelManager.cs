using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;
using System.Linq;

public class PvpUiPanelManager : BaseUIPanel {
	public PvpViewMode pvpViewMode;
	public SingleButtonCallBack BackBtn;
	public SingleButtonCallBack RecordBtn;
	public SingleButtonCallBack RankingBtn;
	public SingleButtonCallBack SkillBtn;
	public Transform SubPanelPoint;
	public GameObject QinglongMartialPanel_prefab;
	private QinglongMartialPanel MartialPanel;
	public GameObject PVPRankingPanel_prefab;
	private PvpRankingPanelManager pvpRankingPanel;
	private static PvpUiPanelManager instance;

	public PVPGroupListAwardDataBase GroupListAwardDatabase;
	public PVPGroupConfigDataBase GroupConfigDatabase;

	public GameObject RankAndHonourToShowEff;
	public GameObject RankAndSkillBtnToShowEff;
	public UILabel MyrankLable;
	public UILabel myHonourLable;
	public static PvpUiPanelManager Instance
	{
		get
		{
			if(instance==null)
			{
				if (!instance) {
					instance = (PvpUiPanelManager)GameObject.FindObjectOfType (typeof(PvpUiPanelManager));
					if (!instance)
						Debug.LogError ("没有附加PvpUiPanelManagerr脚本的gameobject在场景中");
				}
			}
			return instance;
		}
	}
	void Awake()
	{
		BackBtn.SetCallBackFuntion(OnBackBtnClick);
	
		RegisterEventHandler();

		RecordBtn.SetCallBackFuntion(OnRecordBtnClick);
		RankingBtn.SetCallBackFuntion(OnRankingBtnClick);
		SkillBtn.SetCallBackFuntion(OnSkillBtnClick);

	}

	void ShowEff ()
	{
		TweenPosition.Begin(RankAndHonourToShowEff,0.1666f,new Vector3(0,336,0),new Vector3(0,280,0),null);
		TweenPosition.Begin(RankAndSkillBtnToShowEff,0.1666f,new Vector3(0,-336,0),new Vector3(0,-280,0),null);
	}

	void InitText ()
	{
		var team= PvpGetGroupConfig(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_PVPGROUP_VALUE);

		//MyrankLable.SetText()
	}

	public PVPGroupConfig PvpGetGroupConfig(int GroupID)
	{
	  return 	GroupConfigDatabase._dataTable.SingleOrDefault(c=>c.PVPGroupID==GroupID);
	}

	public override void Show (params object[] value)
	{

		base.Show (value);
		InitText();
		ShowEff();
		PvpDataManager.Instance.IsInPvpView=true;
		if(PvpDataManager.Instance.GetGroupmeberInfoList().Count==0)
		{
			PvpDataManager.Instance.IsTeamLeader=true;
		}
		else
		{
			PvpDataManager.Instance.IsTeamLeader=false;
		}
		PvpDataManager.Instance.CurrentViewState=PVPViewState.InPage;
		pvpViewMode.Show();
		ChangeBackgroundMusic();
	}




	protected override void RegisterEventHandler ()
	{
		base.RegisterEventHandler ();
	}

	

	void OnRecordBtnClick(object obj)
	{

	}

	void OnRankingBtnClick(object obj)
	{
		if(pvpRankingPanel==null)
		{
			pvpRankingPanel=CreatObjectToNGUI.InstantiateObj(PVPRankingPanel_prefab,SubPanelPoint).GetComponent<PvpRankingPanelManager>();
		}
		pvpRankingPanel.Show();
	}

	void OnSkillBtnClick(object obj)
	{
		if(MartialPanel==null)
		{
			MartialPanel=CreatObjectToNGUI.InstantiateObj(QinglongMartialPanel_prefab,SubPanelPoint).GetComponent<QinglongMartialPanel>();
		}
		MartialPanel.Show();
	}

	void ChangeBackgroundMusic ()
	{
		SoundManager.Instance.PlayBGM("Music_UIBG_PVPGroup");
	}


	void OnBackBtnClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("");
		Close();
	}

	public override void Close ()
	{
		PvpDataManager.Instance.CurrentViewState=PVPViewState.NotInPage;
		SoundManager.Instance.PlayBGM("Music_BFBG_TownMap05");
		PvpDataManager.Instance.IsInPvpView=false;
		pvpViewMode.Hide();
		base.Close ();
	}
}
