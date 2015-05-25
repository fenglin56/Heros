using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class TrapConfigData
{
    public int _TrapID;
    public string _szName;
    //[GameDataPostFlag(true)]
    //public GameObject _TrapPrefab;
    //[Late]
    public GameObject _TrapPrefab;
    [HideInInspector]
    public string _TrapPrefabId;

    public GameObject TrapPrefab
    {
        get
        {
            if (_TrapPrefab != null)
            {
                return _TrapPrefab;
            }

            _TrapPrefab = AssetId.Resolve(_TrapPrefab, _TrapPrefabId);
            return _TrapPrefab;


            //return AssetId.Resolve(_prefab,_prefabId);
        }
    }
}

public class TrapConfigDataBase : ScriptableObject
{
    public TrapConfigData[] _dataTable;
}

