using UnityEngine;
using System.Collections;
using UI.Ranking;
using UI;
using UI.MainUI;

public class PvpRankingPanelManager : View {

	//public ItemInfoTipsManager TipsMananger; 
	public SingleButtonCallBack BackButton;
	public SingleButtonCallBack NextButton;
	public SingleButtonCallBack PerButton;
	public SingleButtonCallBack AwardButton;
	public SingleButtonCallBack RulesButton;

	public Transform subPanelPoint;
	public GameObject AwardPanel_prefab;
	public GameObject SeasonRulePanel;
	private AwardDescriptionPanel AwardPanel_SC;
	public UILabel RulesLabel;
	public UILabel TermLable;
	public UILabel MyRankLable;
	public UILabel MyHonourLable;
	public UILabel RankCountdownLable_des;
	public UILabel RankCountdownLable;

	public PlayerDetail_Ranking PlayerDetail;
	private RankingType CurrentRankingType=RankingType.PlayerRanking;
	private int CurrentPageindex;
	private int PerPageIndex;
	public  PvpRankingListPanel SC_PvpRankingListPanel;

	
	private static PvpRankingPanelManager instance;
	public static PvpRankingPanelManager GetInstance ()
	{
		if (!instance) {
			instance = (PvpRankingPanelManager)GameObject.FindObjectOfType (typeof(PvpRankingPanelManager));
			if (!instance)
				Debug.LogError ("没有附加JewelBesetManager脚本的gameobject在场景中");
		}
		return instance;
	}
	void Awake()
	{
		BackButton.SetCallBackFuntion(OnBackButtonClick);
		NextButton.SetCallBackFuntion(OnNextButtonClick);
		PerButton.SetCallBackFuntion(OnPerButtonClick);
		AwardButton.SetCallBackFuntion(OnAwardButtonClick);
		RulesButton.SetPressCallBack(OnRulesButtonPress);
		RegisterEventHandler();
		CreatListPanel();

		RulesLabel.text = LanguageTextManager.GetString("IDS_I38_16").Replace("\\n", "\n");
	}
	
	void CreatListPanel()
	{
		
		SC_PvpRankingListPanel.InitList(RankingType.PlayerRanking);

	}
	/// <summary>
	/// 是否显示下一页按钮，每次获取到数据后调用
	/// </summary>
	/// <param name="Ishow">If set to <c>true</c> ishow.</param>
	void CheckNextButton(bool Ishow)
	{
		if (Ishow)
		{
			NextButton.gameObject.SetActive(true);
		}
		else
		{
			NextButton.gameObject.SetActive(false);
		}
	}
	/// <summary>
	/// 是否显示上一页按钮，每次获取到数据后调用
	/// </summary>
	/// <param name="Ishow">If set to <c>true</c> ishow.</param>
	void CheckPerButton(bool Ishow)
	{
		if (Ishow)
		{
			PerButton.gameObject.SetActive(true);
		}
		else
		{
			PerButton.gameObject.SetActive(false);
		}
	}
	
	void GetCurrentPageData()
	{
	

			var list = PvpRankingDataManager.Instance.GetPVPRankingListFromLocal(CurrentPageindex);
			
			if (list == null || list.Count == 0)
			{
				//                        if(PlayerRankingDataManager.Instance.PlayerRankingListDic.Count==0)
				//                            CurrentPageindex=0;
				PvpRankingDataManager.Instance.GetListFromService(RankingType.PlayerRanking, CurrentPageindex);
				LoadingUI.Instance.Show();
				
			}
			else
			{
				SC_PvpRankingListPanel.StartRefershList(list);
				CheckNextButton(CurrentPageindex<PvpRankingDataManager.Instance.PageCount);
				CheckPerButton(CurrentPageindex!=1);
			}
			
   }
	

	void OnBackButtonClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_Exit");
		Close();
	}
	
	void OnNextButtonClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_SwitchPage");
		PerPageIndex=CurrentPageindex;
		CurrentPageindex++;
		GetCurrentPageData();
		if (CurrentPageindex == 10)
		{
			CheckNextButton(false);
		}
	}
	
	void OnPerButtonClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_SwitchPage");
		PerPageIndex=CurrentPageindex;
		CurrentPageindex--;
		GetCurrentPageData();
	}

	void OnAwardButtonClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_SwitchPage");
		if(AwardPanel_SC==null)
		{
			AwardPanel_SC=CreatObjectToNGUI.InstantiateObj(AwardPanel_prefab,subPanelPoint).GetComponent<AwardDescriptionPanel>();
		}
		AwardPanel_SC.Show();
	}

	void OnRulesButtonPress(bool isPress)
	{
		if(isPress)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_SwitchPage");
			TweenAlpha.Begin(SeasonRulePanel, 0.1666f, 1);
			TweenPosition.Begin(SeasonRulePanel, 0.1666f, new Vector3(0, -180, 0), new Vector3(0, -160, 0));
		}
		else
		{
			TweenAlpha.Begin(SeasonRulePanel, 0.1666f, 0);
			TweenPosition.Begin(SeasonRulePanel, 0.1666f, new Vector3(0, -160, 0), new Vector3(0, -180, 0));
		}

	}

	
	public void SetMyRanking(int rank)
	{
		if(rank<=0||rank>100)
		{
			//NotInRankingSpring.gameObject.SetActive(true);
			MyRankLable.gameObject.SetActive(false);
		}
		else
		{
			//NotInRankingSpring.gameObject.SetActive(false);
			MyRankLable.gameObject.SetActive(true);
			MyRankLable.SetText(rank);
			
		}
	}
	

	public  void Show()
	{	
		transform.localPosition=Vector3.zero;
		InvokeRepeating("UpdateTime",0,0.5f);
		GetCurrentPageData();
	}
	
	Vector3 GetSpriteSize(UISprite sprite)
	{
		Rect rect=  sprite.GetAtlasSprite().outer;
		return new Vector3(rect.width,rect.height,1);
	}

	
	
	float preTime = 0;
	void UpdateTime()
	{
		// float dddd = Time.realtimeSinceStartup - preTime;
		if (((Time.realtimeSinceStartup - PvpRankingDataManager.Instance.RankUpateTimeSinceGameStart) >= PvpRankingDataManager.Instance.UpdateRankInterval)&&PvpRankingDataManager.Instance.IfNeedGetDataClearData())
		{
			PvpRankingDataManager.Instance.ClearAllData();
			GetCurrentPageData();
			PvpRankingDataManager.Instance.RankUpateTimeSinceGameStart = Time.realtimeSinceStartup;
		} else
		{
			preTime=PvpRankingDataManager.Instance.UpdateRankInterval-(Time.realtimeSinceStartup - PvpRankingDataManager.Instance.RankUpateTimeSinceGameStart);
			int hour = (int)preTime/3600;
			int minue = ((int)preTime%3600)/60;
			int second = (int)preTime%3600%60;
			RankCountdownLable.text = string.Format("{0:d2}:{1:d2}:{2:d2}", hour, minue, second);
		}
	}
	
	public  void Close()
	{
		transform.localPosition=new Vector3(0,0,-1000);
		PlayerDetail.CloseDatailPanel();
		CancelInvoke("UpdateTime");
		// PlayerRankingDataManager.Instance.RankUpateTimeSinceGameStart=Time.realtimeSinceStartup;
	}
	
	
	void GetRankingListHandel(object obj)
	{
		LoadingUI.Instance.Close();
		SMsgInteract_PvpRanking_SC data=(SMsgInteract_PvpRanking_SC)obj;
		
		if(data.byRankingNum>0)
		{
			CurrentPageindex= data.byIndex;
			GetCurrentPageData();
		}
		else
		{
			CurrentPageindex=PerPageIndex;
		}
		
	}
	
	public void ShowDetailePanel(RankingType type,uint OtherId)
	{
		PlayerDetail.ShowPanel();
		PvpRankingDataManager.Instance.GetPlayerDetailFromService(type,OtherId,(uint)PlayerManager.Instance.FindHeroDataModel().ActorID);
	}
	protected override void RegisterEventHandler()
	{
		UIEventManager.Instance.RegisterUIEvent(UIEventType.PVPReceiveRankingListRes,GetRankingListHandel);
	}
	void OnDestroy()
	{
		instance=null;
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.PVPReceiveRankingListRes,GetRankingListHandel);
	}
}

