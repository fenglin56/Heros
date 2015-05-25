using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BroadcastConfigData
{
	public int BroadcastId;
	public int BroadcastType;//填写广播类型，1=收服妖女；2=获得指定道具；3=通过指定关卡；4=获得称号；5=完成指定任务
	public int BroadcastConditions;
	public string BroadcastContent;
}

public class BroadcastConfigDataBase : ScriptableObject
{
	public BroadcastConfigData[] _dataTable;
}
