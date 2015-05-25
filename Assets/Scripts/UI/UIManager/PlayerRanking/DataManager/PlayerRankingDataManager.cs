using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerRankingDataManager : ISingletonLifeCycle {
   
    private static PlayerRankingDataManager m_instance;
    public static PlayerRankingDataManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new PlayerRankingDataManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }
    private PlayerRankingDataManager()
    {
        PlayerRankingListDic=new Dictionary<int, List<RankingActorFightData>>();
        SirenRankingListDic=new Dictionary<int, List<RankingYaoNvFightData>>();
        WeaponRankingListDic=new Dictionary<int, List<RankingEquipFightData>>();
        UpdateRankInterval=-1;
        DefultPlayerRankingPage=0;
        DefultSirenRankingPage=0;
        DefultWeaponRankingPage=0;
    }
    public SMsgInteract_GetPlayerRanking_SC RankingDetail;
    public int LastReqPageIndex{get;private set;}
    public RankingType LastReqRankingType{get;private set;}
    public Dictionary<int,List<RankingActorFightData>> PlayerRankingListDic{get;private set;}
    public Dictionary<int,List<RankingYaoNvFightData>> SirenRankingListDic{get;private set;}
    public Dictionary<int,List<RankingEquipFightData>> WeaponRankingListDic{get;private set;}
    public int DefultPlayerRankingPage{get;private set;}
    public int DefultSirenRankingPage{get;private set;}
    public int DefultWeaponRankingPage{get;private set;}
    public int UpdateRankInterval{get; set;}
    public int CurPlayerRanking{get;private set;}
    public int MyPlayerRanking{get;private set;} 
    public int MySirenRanking{get;private set;}
    public int MyWeaponRanking{get;private set;}
    public int PlayerRankingPageCount{get;private set;}
    public int SirenRankingPageCount{get;private set;}
    public int WeaponRankingPageCount{get;private set;}
    //public int PageCount{get;private set;}
    public float RankUpateTimeSinceGameStart;
    public void SetRankingList(SMsgInteract_RankingList_SC sMsgInteract_RankingList_SC)
    {
        //Debug.Log("时间"+sMsgInteract_RankingList_SC.UpdateRankInterval+"榜"+sMsgInteract_RankingList_SC.byRankingType+"排名"+sMsgInteract_RankingList_SC.byActorRanking);
        switch(sMsgInteract_RankingList_SC.byRankingType)
        {
            case (byte)RankingType.PlayerRanking:
                if(PlayerRankingListDic.Count==0)
                {
                    if(SirenRankingListDic.Count==0&&WeaponRankingListDic.Count==0)
                    {
                    UpdateRankInterval=(int)sMsgInteract_RankingList_SC.UpdateRankInterval;
                    RankUpateTimeSinceGameStart=Time.realtimeSinceStartup;
                   
                    }
                    //UIEventManager.Instance.TriggerUIEvent(UIEventType.AddAirValue,null);
                    DefultPlayerRankingPage=sMsgInteract_RankingList_SC.byIndex;
                    MyPlayerRanking=sMsgInteract_RankingList_SC.byActorRanking;
                    PlayerRankingPageCount=sMsgInteract_RankingList_SC.byTotalIndex;

                }
                if(sMsgInteract_RankingList_SC.rankingActorFightData.Length>0&&!PlayerRankingListDic.ContainsKey((int)sMsgInteract_RankingList_SC.byIndex))
                {
                    PlayerRankingListDic.Add((int)sMsgInteract_RankingList_SC.byIndex,new List<RankingActorFightData>(sMsgInteract_RankingList_SC.rankingActorFightData));
                }
               
                break;
            case (byte)RankingType.SirenRanking:
                if(SirenRankingListDic.Count==0)
                {
                    if(PlayerRankingListDic.Count==0&&WeaponRankingListDic.Count==0)
                    {
                        UpdateRankInterval=(int)sMsgInteract_RankingList_SC.UpdateRankInterval;
                        RankUpateTimeSinceGameStart=Time.realtimeSinceStartup;

                    }
                    DefultSirenRankingPage=sMsgInteract_RankingList_SC.byIndex;
                    MySirenRanking=sMsgInteract_RankingList_SC.byActorRanking;
                    SirenRankingPageCount=sMsgInteract_RankingList_SC.byTotalIndex;
                 
                }
                if(sMsgInteract_RankingList_SC.rankingYaoNvFightData.Length>0&&!SirenRankingListDic.ContainsKey((int)sMsgInteract_RankingList_SC.byIndex))
                {
                    SirenRankingListDic.Add((int)sMsgInteract_RankingList_SC.byIndex,new List<RankingYaoNvFightData>(sMsgInteract_RankingList_SC.rankingYaoNvFightData));
                }
               
                break;
            case (byte)RankingType.WeaponRanking:
                if(WeaponRankingListDic.Count==0)
                {
                    if(PlayerRankingListDic.Count==0&&SirenRankingListDic.Count==0)
                    {
                     UpdateRankInterval=(int)sMsgInteract_RankingList_SC.UpdateRankInterval;
                     RankUpateTimeSinceGameStart=Time.realtimeSinceStartup;
                    }
                    MyWeaponRanking=sMsgInteract_RankingList_SC.byActorRanking;
                    DefultWeaponRankingPage=sMsgInteract_RankingList_SC.byIndex;
                    WeaponRankingPageCount=sMsgInteract_RankingList_SC.byTotalIndex;
                }
                if(sMsgInteract_RankingList_SC.RankingEquipFightData.Length>0&&!WeaponRankingListDic.ContainsKey((int)sMsgInteract_RankingList_SC.byIndex))
                {
                    WeaponRankingListDic.Add((int)sMsgInteract_RankingList_SC.byIndex,new List<RankingEquipFightData>(sMsgInteract_RankingList_SC.RankingEquipFightData));
                }

                break;
        }
        CurPlayerRanking=sMsgInteract_RankingList_SC.byActorRanking;
    }


    public void ClearAllData()
    {
        PlayerRankingListDic.Clear();
        SirenRankingListDic.Clear();
        WeaponRankingListDic.Clear();
        //UpdateRankInterval=-1;
    }

    public bool IfNeedGetDataClearData()
    {
        bool res;
        if(PlayerRankingListDic.Count==0&&SirenRankingListDic.Count==0&&WeaponRankingListDic.Count==0)
        {
            res=false;
        }
        else
        {
            res=true;
        }
        return res;
    }

    public List<RankingActorFightData>GetPlayerRankingListFromLocal(int PageIndex)
    {
        List<RankingActorFightData> list;
        PlayerRankingListDic.TryGetValue(PageIndex,out list);
        return  list;
    }


    public List<RankingYaoNvFightData>GetSirenRankingListFromLocal(int PageIndex)
    {
        List<RankingYaoNvFightData> list;
        SirenRankingListDic.TryGetValue(PageIndex,out list);
        return  list;
      
    }


    public List<RankingEquipFightData>GetWeaponRankingListFromLocal(int PageIndex)
    {
        List<RankingEquipFightData> list;
        WeaponRankingListDic.TryGetValue(PageIndex,out list);
        return  list;
      
    }


    public void  GetListFromService(RankingType type,int PageIndex)
    {
        LastReqPageIndex=PageIndex;
        LastReqRankingType=type;
        SMsgInteract_RankingList_CS msg=new SMsgInteract_RankingList_CS()
        {
            byRankingType=(byte)type,
            byIndex=(byte)PageIndex,
        };

        NetServiceManager.Instance.InteractService.SendSMsgInteract_RankingList_CS(msg);
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
public enum RankingType : byte
{
    PlayerRanking=1,
    SirenRanking,
    WeaponRanking,
}