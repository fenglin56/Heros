using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class EctypeContainerIconData
{
    public int lEctypeContainerID;//副本容器ID
    public string lEctypeName;//副本名称
    public int lDifficulty;//副本难度
    public GameObject EctypeIconPrefab;//副本图片prefab
}

public class EctypeContainerIconPrefabDataBase : ScriptableObject{

    public EctypeContainerIconData[] iconDataList;

    public EctypeContainerIconData GetIconData(int ectypeContainerID,int Difficulty)
    {
        return iconDataList.FirstOrDefault(P=>P.lEctypeContainerID == ectypeContainerID&&P.lDifficulty == Difficulty);
    }
}
