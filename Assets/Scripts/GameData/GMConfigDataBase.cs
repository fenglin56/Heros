using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class GMConfigData
{
	public int m_gmType;
	public string m_name;
	public string m_desc;
}


public class GMConfigDataBase:ScriptableObject 
{
	public GMConfigData[] m_dataTable;

}
