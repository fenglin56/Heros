using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class LocalTreasureTreesData
{
    public int TreeID;
    public int PositionID;
    public int UnlockLevel;
    public int UnlockCost;
    public string CreatFruit;
}

[Serializable]
public class FruitData
{
    public int FruitID;
    public string FruitName;
    public string SeedModelID;
    public string FlowerModelID;
    public string GrowModelID;
    public string RipenModelID;
    public int FruitLevel;
    public int FlowerTime;
    public int GrowTime;
    public int RipenTime;
    public int RewardType;
    public string RewardNumber;
    public string[] FruitReward;
}

public class TreasureTreesDataBase : ScriptableObject{

    public LocalTreasureTreesData[] TreasureTreesDataList;
    public FruitData[] FruitDataList;
}
