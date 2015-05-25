using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// BattleUI Scene BattleDataManager 
/// </summary>
public class TownEctypeResDataManager : MonoBehaviour
{
	public EctypeContainerIconPrefabDataBase EctypeContainerIconPrefabDataList;
	private Dictionary<int, EctypeContainerIconData> m_ectypeContainerIconPrefabDataDict = new Dictionary<int, EctypeContainerIconData>();
	
	private static TownEctypeResDataManager m_Instance;
	
	void Awake()
	{
		foreach (EctypeContainerIconData child in EctypeContainerIconPrefabDataList.iconDataList)
		{
			m_ectypeContainerIconPrefabDataDict.Add(child.lEctypeContainerID, child);
		}
		m_Instance = this;
	}
	
	void OnDestroy()
	{
		m_Instance = null;
	}
	
	public static TownEctypeResDataManager Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = FindObjectOfType(typeof(TownEctypeResDataManager)) as TownEctypeResDataManager;
			}
			return m_Instance;
		}
	}
	
	public EctypeContainerIconData GetEctypeContainerResData(int ectypeContainerID)
	{
		EctypeContainerIconData resData;
		m_ectypeContainerIconPrefabDataDict.TryGetValue(ectypeContainerID, out resData);        
		return resData;
	}
	
}
