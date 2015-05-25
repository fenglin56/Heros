using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BuffConfigData
{
    public int _buffID;
    public string _iconID;  //Buffer的SpriteName
    public int _buffLv;
    public float _durativeTime;
    public string _buffName;
    //[GameDataPostFlag(true)]
    //public GameObject _buffEffect;
    //[Late]
    public GameObject _buffEffect;
    [HideInInspector]
    public string _buffEffectId;

    public string _buffEffMount;

	public string _buffSound;//\

    public GameObject _buffEffectPrefab
    {
        get
        {
            if (_buffEffect != null)
            {
                return _buffEffect;
            }

            _buffEffect = AssetId.Resolve(_buffEffect, _buffEffectId);
            return _buffEffect;


            //return AssetId.Resolve(_prefab,_prefabId);
        }
    }
    public string _buffAnimName;
}

public class BuffConfigDataBase : ScriptableObject
{
    public BuffConfigData[] _dataTable;
}

