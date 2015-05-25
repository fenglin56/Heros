using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class NPCSpecialConfigData
{
    public int _NPCID;
	public int _FunctionType;
    public string _FunctionDesc;
    public int Parameters;
    public string ShopIcon;
    public string ShopTitle;
}

public class NPCSpecialConfigDataBase : ScriptableObject
{
    public NPCSpecialConfigData[] _dataTable;
}

