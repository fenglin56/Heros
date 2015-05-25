using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class NPCConfigData
{
    public int _NPCID;
    public string _szName;
    public string _npcTitle;
    public GameObject PortraitID;
	public Vector3 CameraOffset;
	
	
    //[GameDataPostFlag(true)]
    //public GameObject _NPCPrefab;
    //[Late]
    public GameObject _NPCPrefab;
    [HideInInspector]
    public string _NPCPrefabId;

    public GameObject NPCPrefab
    {
        get
        {
            if (_NPCPrefab != null)
            {
                return _NPCPrefab;
            }

            _NPCPrefab = AssetId.Resolve(_NPCPrefab, _NPCPrefabId);
            return _NPCPrefab;


            //return AssetId.Resolve(_prefab,_prefabId);
        }
    }
}

public class NPCConfigDataBase : ScriptableObject
{
    public NPCConfigData[] _dataTable;
}

