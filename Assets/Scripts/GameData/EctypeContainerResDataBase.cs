using UnityEngine;
using System.Collections;
using System;

public class EctypeContainerResDataBase : ScriptableObject
{
    public EctypeContainerResData[] _dataTable;
}


[Serializable]
public class EctypeContainerResData
{
    public int lEctypeContainerID;//副本容器ID
    public GameObject bossHead;//Boss头像
    public GameObject bossAppearancePortrait;//BOSS出场动画的头像
	public EctypeContainerBossHeadRes[] BossHeadReses;
}

[Serializable]
public class EctypeContainerBossHeadRes
{
	public int BossHeadID;
	public GameObject BossHeadGO;
}
