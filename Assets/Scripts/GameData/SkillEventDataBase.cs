using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SkillEventData
{
	public string m_aniName;
	public string[] m_soundEffectList;
    [GameDataPostFlag(true)]
	public GameObject[] m_viewEffectList;

	public SkillEvent[] m_eventList;
}

[Serializable]
public class SkillEventDataGroup
{
	public string m_skillId;
	public SkillEventData m_prepareData;
	public SkillEventData m_fireData;
	
}

public class SkillEventDataBase : ScriptableObject
{
	public SkillEventDataGroup[] m_dataTable;
}
