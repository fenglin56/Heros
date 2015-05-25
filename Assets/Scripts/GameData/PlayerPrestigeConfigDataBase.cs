using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerPrestigeConfigData
{
    public int _pvpLevel;
    public int _pvpGroup;
    public string _titleName;
    public int _pvpExp;
    public string _groupName;
    public int _pvpInsignia;
    public string _title_ID;

    public GameObject _titlePrefab;
    //
    public string[] _ProvocationWord;
    public string[] _WinWord;
    public string[] _LoseWord;
}

public class PlayerPrestigeConfigDataBase : ScriptableObject
{
    public PlayerPrestigeConfigData[] _dataTable;
}