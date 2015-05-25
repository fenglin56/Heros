using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageConfigData
{
    public int _damageID;
    public string _damageName;
    public int _boxType;    //类型
    public int _correspondingItemID;    //盒子里对应物品ID
    //[GameDataPostFlag(true)]
    //public GameObject _damagePrefab;
    //[Late]
    public GameObject _damagePrefab;
    [HideInInspector]
    public string _damagePrefabId;

    public GameObject DamagePrefab
    {
        get
        {
            if (_damagePrefab != null)
            {
                return _damagePrefab;
            }

            _damagePrefab = AssetId.Resolve(_damagePrefab, _damagePrefabId);
            return _damagePrefab;


            //return AssetId.Resolve(_prefab,_prefabId);
        }
    }
}

public class DamageConfigDataBase : ScriptableObject 
{
    public DamageConfigData[] _dataTable;
}
