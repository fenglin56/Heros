using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMeridiansDataManager : MonoBehaviour
{
    //private static PlayerMeridiansDataManager m_instance;
    //public static PlayerMeridiansDataManager Instance { get { return m_instance; } }

    public PlayerMeridiansDataBase playerMeridiansDataBase;
    public PlayerMeridiansDataBase PlayerKongfuDataBase;
    public PlayerMeridiansDataBase PlayerEffectPositionDataBase;

    private Dictionary<int, PlayerMeridiansData> PlayerMeridiansDataList = new Dictionary<int,PlayerMeridiansData>();
    private Dictionary<int, PlayerKongfuData> PlayerKongfuDataList = new Dictionary<int, PlayerKongfuData>();
    private Dictionary<int, MeridiansEffectPositionData> PlayerEffectPositionDataList = new Dictionary<int, MeridiansEffectPositionData>();

    void Awake()
    {
        //m_instance = this;
        InitMeridiansData();
        InitKonfuData();
        InitEffectPointData();
    }

    //void OnDestroy()
    //{
    //    m_instance = null;
    //}

    void InitMeridiansData()
    {
        foreach (var child in playerMeridiansDataBase.PlayermeridiansDataList)
        {
            PlayerMeridiansDataList.Add(child.MeridiansLevel,child);
        }
    }

    void InitKonfuData()
    {
        foreach (var child in PlayerKongfuDataBase.PlayerKongfuDataList)
        {
            PlayerKongfuDataList.Add(child.KongfuLevel,child);
        }
    }

    void InitEffectPointData()
    {
        foreach (var child in PlayerEffectPositionDataBase.MeridiansEffectPositionDataList)
        {
            PlayerEffectPositionDataList.Add(child.effectID, child);
        }
    }

    public PlayerKongfuData GetKonfuData(int kongfuLevel)
    {
        PlayerKongfuData kongfuData = null;
        PlayerKongfuDataList.TryGetValue(kongfuLevel,out kongfuData);
        return kongfuData;
    }

    public PlayerMeridiansData GetMeridiansData(int meridiansLevel)
    {
        PlayerMeridiansData meridiansData = null;
        PlayerMeridiansDataList.TryGetValue(meridiansLevel,out meridiansData);
        return meridiansData;
    }

    public MeridiansEffectPositionData GetMeridiansEffectPointData(int EffectID)
    {
        MeridiansEffectPositionData effectData = null;
        PlayerEffectPositionDataList.TryGetValue(EffectID, out effectData);
        return effectData;
    }
}
