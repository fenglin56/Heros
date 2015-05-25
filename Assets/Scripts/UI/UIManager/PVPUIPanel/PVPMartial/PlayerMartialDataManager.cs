using UnityEngine;
using System.Collections.Generic;
using System.Linq;


//管理服务端下发的武学数据
public class PlayerMartialDataManager : ISingletonLifeCycle 
{
	private static PlayerMartialDataManager instance;
	public static PlayerMartialDataManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new PlayerMartialDataManager();
				SingletonManager.Instance.Add(instance);
			}
			return instance;
		}
	}

	private int MartialDataNum;	//武学数量
	private List<SSendWuXueData> MartialDataList = new List<SSendWuXueData>();	//当前已解锁的武学数据

	/// <summary>
	/// 接收到服务端下发的武学信息
	/// </summary>
	/// <param name="sMsgActionWuXue_SC">S message action wu xue_ S.</param>
	public void ReceivePlayerMartialData(SMsgActionWuXue_SC sMsgActionWuXue_SC)
	{
		TraceUtil.Log("收到服务端下发的武学信息");
		MartialDataNum = sMsgActionWuXue_SC.byWuXueNum;
		if(MartialDataNum > 0)
		{
			MartialDataList = sMsgActionWuXue_SC.WuXueDataList.ToList();
		}
	}

	/// <summary>
	/// 武学学习或升级结果，更新武学列表
	/// </summary>
	/// <param name="sMsgActionStudyWuXue_SC">S message action study wu xue_ S.</param>
	public void UpdatePlayerMartialData(SMsgAcitonStudyWuXue_SC sMsgActionStudyWuXue_SC)
	{
		int martialID = sMsgActionStudyWuXue_SC.dwWuXueID;
		byte martialLevel = sMsgActionStudyWuXue_SC.byWuXueLevel;

		MartialIndex martialIndex = new MartialIndex(){MartialArtsID = martialID, MartialArtsLevel = (int)martialLevel-1};
		PlayerMartialArtsData martialArtsData = PlayerDataManager.Instance.GetPlayerMartialArtConfigData(martialIndex);
		if(martialArtsData.MartialArtsUnlock != 0)	//如果有解锁新武学
		{
			MartialDataList.Add(new SSendWuXueData(){dwWuXueID = martialArtsData.MartialArtsUnlock, byWuXueLevel = 0});
		}

		for(int i = 0; i < MartialDataList.Count; i++)
		{
			if(MartialDataList[i].dwWuXueID == martialID)
			{
				SSendWuXueData tempData = new SSendWuXueData(){dwWuXueID = martialID, byWuXueLevel = martialLevel};
				MartialDataList[i] = tempData;
				break;
			}
		}
	}

	/// <summary>
	/// 根据武学类型获取相应列表
	/// </summary>
	/// <returns>The martial list by type.</returns>
	/// <param name="martialType">Martial type.</param>
	public List<SSendWuXueData> GetMartialListByType(MartialType martialType)
	{
		List<SSendWuXueData> tempWuXueData = new List<SSendWuXueData>();
		MartialDataList.ApplyAllItem(p=>{
			MartialIndex martialIndex = new MartialIndex(){MartialArtsID = p.dwWuXueID, MartialArtsLevel = (int)p.byWuXueLevel};
			PlayerMartialArtsData martialArtsData = PlayerDataManager.Instance.GetPlayerMartialArtConfigData(martialIndex);
			if(martialArtsData.MartialArtsType == (byte)martialType)
			{
				tempWuXueData.Add(p);
			}
		});
		return tempWuXueData;
	}

	/// <summary>
	/// 获取武学等级
	/// </summary>
	/// <returns>The martial level by I.</returns>
	/// <param name="martialID">Martial I.</param>
	public int GetMartialLevelByID(int martialID)
	{
		return  (int)MartialDataList.Single(p=>p.dwWuXueID == martialID).byWuXueLevel;
	}

	//指定ID的武学学习后是否解锁新武学
	public int UnlockNewMartialId(int martialID)
	{
		int level = GetMartialLevelByID(martialID);
		MartialIndex martialIndex = new MartialIndex(){MartialArtsID = martialID, MartialArtsLevel = level-1};
		PlayerMartialArtsData martialArtsData = PlayerDataManager.Instance.GetPlayerMartialArtConfigData(martialIndex);
		return martialArtsData.MartialArtsUnlock != 0 ? martialArtsData.MartialArtsUnlock:0;
	}

	public void Instantiate()
	{
		
	}
	public void LifeOver()
	{
		instance = null;
		//UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TownUIBtnLoadComplete, TownUISceneLoadComplete);
	}
}

/*
public class MartialDataModel
{
	public SSendWuXueData sSendWuXueData;	//服务端下发的
	public PlayerMartialArtsData playerMartialArtsData;	//本地配表的

	public bool IsLock	//是否已解锁
	{
		get{}
		set{}
	}

	public MartialDataModel(SSendWuXueData sSendWuXueData)
	{
		this.sSendWuXueData = sSendWuXueData;
		playerMartialArtsData = PlayerDataManager.Instance.GetPlayerMartialArtConfigData(sSendWuXueData.dwWuXueID);
	}

}
*/



