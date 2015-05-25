using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class NPCTalkConfigData
{
    public int _SID;
    public string _szTalk;
    public string _szVoice;
}

public class NPCTalkConfigDataBase : ScriptableObject
{
    public NPCTalkConfigData[] _dataTable;
}

