using UnityEngine;
using System.Collections;

[System.Serializable]
public class ProfessionConfigData
{
    public int _professionID;
    public string _professionName;
    [GameDataPostFlag(true)]
    public GameObject _playerIcon;
}

public class ProfessionConfigDataBase : ScriptableObject
{
    public ProfessionConfigData[] _dataTable;
}
