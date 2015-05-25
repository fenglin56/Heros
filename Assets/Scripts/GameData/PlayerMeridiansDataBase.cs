using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class PlayerMeridiansData
{
    public int MeridiansLevel;
    public int KongfuLevel;
    public string KongfuName;
    public int LevelUpNeed;
    public string EffectAdd;
}

[Serializable]
public class PlayerKongfuData
{
    public int KongfuLevel;
    public int LevelNeed;
    public string[] MeridiansList;
    public string KongfuName;
    public string KongfuPic;
    public string KongfuEff;
    public string KongfuNameRes;

    public GameObject KongfuPicPrefab;
}

[Serializable]
public class MeridiansEffectPositionData
{
    public int effectID;
    public string[] position;
}

public class PlayerMeridiansDataBase : ScriptableObject 
{
    public PlayerMeridiansData[] PlayermeridiansDataList;
    public PlayerKongfuData[] PlayerKongfuDataList;
    public MeridiansEffectPositionData[] MeridiansEffectPositionDataList;
}
