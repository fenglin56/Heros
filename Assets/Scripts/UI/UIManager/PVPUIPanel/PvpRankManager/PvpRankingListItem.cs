using UnityEngine;
using System.Collections;
using UI;
using System;
using System.Text;
using UI.Ranking;

public class PvpRankingListItem : MonoBehaviour {

	public Action<PvpRankingListItem> OnClickCallBack;
	public GameObject RankingUint_prefab;
	public Transform RankingUnitPoint;
	private RankingUnit SC_RankingUnit;
	
	public GameObject PvpUnit_prefab;
	public Transform  PvpUnitPoint;
	private  PVPUnit SC_PVPUnit;
	
	public GameObject PlayerHeadUnit_prefab;
	public Transform  PlayerHeadUnitPoint;
	private  PlayerHeadUnit SC_PlayerHeadUnit;

	public SingleButtonCallBack LookdetailBtn;
	private uint otherid;
	void Awake()
	{
		GetComponent<UIEventListener>().onClick=OnItemClick;
		SC_RankingUnit=CreatObjectToNGUI.InstantiateObj(RankingUint_prefab,RankingUnitPoint).GetComponent<RankingUnit>();
		SC_PlayerHeadUnit=CreatObjectToNGUI.InstantiateObj(PlayerHeadUnit_prefab,PlayerHeadUnitPoint).GetComponent<PlayerHeadUnit>();
		SC_PVPUnit=CreatObjectToNGUI.InstantiateObj(PvpUnit_prefab,PvpUnitPoint).GetComponent<PVPUnit>();
		LookdetailBtn.SetCallBackFuntion(DetailBtnClick);
	}
	
	void DetailBtnClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_Detail");
		PvpRankingPanelManager.GetInstance().ShowDetailePanel(RankingType.SirenRanking,otherid);
	}
	void OnItemClick(GameObject obj)
	{
		if(OnClickCallBack!=null)
		{
			OnClickCallBack(this);
		}
	}
	
	public  void BeSelected()
	{
		OnClickCallBack(this);
	}
	public  void OnGetFocus() 
	{
		//SelectSpring.gameObject.SetActive(true);
	}
	
	public  void OnLoseFocus() 
	{
		//SelectSpring.gameObject.SetActive(false);   
	}
	public void  InitItemData(SPVPRankingData data)
	{
		otherid=data.dwActorID;
		SC_RankingUnit.InitData(data.byIndex);
		SC_PVPUnit.ShowUnit((int)data.byGroupID,(int)data.dwHonorNum,(int)data.byWinRate);
		SC_PlayerHeadUnit.InitData(data.byKind,Encoding.UTF8.GetString(data.szActorName),(int)data.byLevel,(int)data.byVipLevel,data.dwFashionID);
		
	}
}
