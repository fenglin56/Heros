using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class StroyDialogConfigData
{
    public int _DialogID;
    public string _Content;
    public Vector3 _ViewOffset;
	//1玩家，2是npc
    public int _NpcOrPlayer;
    public int _DialogType;
    public string _NpcName;
    public string _NpcIconName;
	public GameObject npcIconPrefab;
}


public class StroyLineDialogDataBase : ScriptableObject
{
    public StroyDialogConfigData[] _dataTable;
}
