using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvpRankingDataManager : ISingletonLifeCycle {
	private static PvpRankingDataManager m_instance;
	public static PvpRankingDataManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = new PvpRankingDataManager();
				SingletonManager.Instance.Add(m_instance);
			}
			return m_instance;
		}
	}

	private PvpRankingDataManager()
	{
		PvpRankingList=new Dictionary<int, List<SPVPRankingData>>();
		UpdateRankInterval=-1;
		DefultPvpRankingPage=0;
		PageCount=1;

	}
	public SMsgInteract_GetPlayerRanking_SC RankingDetail;
	public int LastReqPageIndex{get;private set;}
	public Dictionary<int,List<SPVPRankingData>> PvpRankingList{get;private set;}
	public int DefultPvpRankingPage{get;private set;}
	public int UpdateRankInterval{get; set;}
	public int MyPVPRanking{get;private set;}
	public int PageCount{get;private set;}
	public float RankUpateTimeSinceGameStart;

	public void SetRankingList(SMsgInteract_PvpRanking_SC sMsgInteract_PvpRanking_SC)
	{
		//Debug.Log("时间"+sMsgInteract_RankingList_SC.UpdateRankInterval+"榜"+sMsgInteract_RankingList_SC.byRankingType+"排名"+sMsgInteract_RankingList_SC.byActorRanking);
	
			if(PvpRankingList.Count==0)
			{

				UpdateRankInterval=(int)sMsgInteract_PvpRanking_SC.dwFreshInterval;
			    RankUpateTimeSinceGameStart=Time.realtimeSinceStartup;
				//UIEventManager.Instance.TriggerUIEvent(UIEventType.AddAirValue,null);
			    DefultPvpRankingPage=sMsgInteract_PvpRanking_SC.byIndex;
			    PageCount=sMsgInteract_PvpRanking_SC.byTotalIndex;
				
			}

		if(sMsgInteract_PvpRanking_SC.RankingDataList.Length>0&&!PvpRankingList.ContainsKey((int)sMsgInteract_PvpRanking_SC.byIndex))
			{
			PvpRankingList.Add((int)sMsgInteract_PvpRanking_SC.byIndex,new List<SPVPRankingData>(sMsgInteract_PvpRanking_SC.RankingDataList));
			}
	}
	
	
	public void ClearAllData()
	{
		PvpRankingList.Clear();
		//UpdateRankInterval=-1;
	}
	
	public bool IfNeedGetDataClearData()
	{
		bool res;
		if(PvpRankingList.Count==0)
		{
			res=false;
		}
		else
		{
			res=true;
		}
		return res;
	}
	
	public List<SPVPRankingData>GetPVPRankingListFromLocal(int PageIndex)
	{
		List<SPVPRankingData> list;
		PvpRankingList.TryGetValue(PageIndex,out list);
		return  list;
	}

	
	
	public void  GetListFromService(RankingType type,int PageIndex)
	{
		LastReqPageIndex=PageIndex;
		SMsgInteract_PvpRanking_CS msg=new SMsgInteract_PvpRanking_CS()
		{
			byIndex=(byte)PageIndex,
		};
		
		NetServiceManager.Instance.InteractService.SendGetPvpRankingList_CS(msg);
	}

	public void  GetPlayerDetailFromService(RankingType type,uint otherID,uint MyID)
	{
		SMsgInteract_GetPlayerRanking_CS msg=new SMsgInteract_GetPlayerRanking_CS()
		{
			byRankingType=(byte)type,
			dwActorID=MyID,
			dwRankActorID=otherID,
		};
		NetServiceManager.Instance.InteractService.SendSMsgInteract_GetPlayerRanking_CS(msg);
	}
	
	
	
	public void Instantiate()
	{
		
	}
	
	public void LifeOver()
	{
		m_instance=null;
	}
}