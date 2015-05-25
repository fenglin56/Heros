using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//该区域副本所以数据//
[Serializable]
public class EctypeSelectConfigData
{
    public int _lEctypeID;      //副本ID (区域ID)
    public string _szName;      //副本名字
    public int[] _vectDifficulty;    //副本难度列表
	public List<int> Difficult2Container;//宗师难度副本容器ID列表
    public int[] _vectContainer;     //普通难度副本容器ID列表
	public string _EctypeIcon;
	public GameObject _EctypeIconPrefab;//副本图标prefab
	public string EctypeRewardIcon;//宝箱名字
	public EctypeRewardItem[] AwardItem;//奖励装备
	//[GameDataPostFlag(true)]
    public int _lEctypeYaoqiMax;
    public int _sirenEctypeContainerID; //妖女副本id
	public int lEctypeType;//副本类型
	public Dictionary<int,int> VectContainerList = new Dictionary<int,int>();//副本难度对应的ID列表

	public int DefenceChallengeRemainNum{get;set;}//防守副本今日剩余挑战次数，当lEctypeType为8，即防守副本时使用
	//[Late]
    //public GameObject _prefab;
    //[HideInInspector]
    //public string _prefabId;

    //public GameObject Prefab
    //{
    //    get
    //    {
    //        if (_prefab != null)
    //        {
    //            return _prefab;
    //        }

    //        _prefab = AssetId.Resolve(_prefab, _prefabId);
    //        return _prefab;


    //        //return AssetId.Resolve(_prefab,_prefabId);
    //    }
    //}

    public void InitectContainer()
    {
        for (int i = 0; i < _vectContainer.Length; i++)
        {
            if (!VectContainerList.ContainsKey(_vectDifficulty[i]))
                VectContainerList.Add(_vectDifficulty[i], _vectContainer[i]);
            //else
            //{
            //    //TraceUtil.Log(_vectDifficulty[i]+"有冲突");
            //}
        }
        ////TraceUtil.Log("添加副本选择配表"+VectContainerList.Count);
    }

    /// <summary>
    /// 获得对应具体副本ID
    /// </summary>
    /// <param name="vectDifficulty">难度</param>
    /// <returns> 为0 表示不存在</returns>
    public int GetVectContainer(int vectDifficulty)
    {
        if (VectContainerList.ContainsKey(vectDifficulty))
        {
            return VectContainerList[vectDifficulty];
        }
        return 0;
    }
}

[Serializable]
public class EctypeRewardItem
{
	public int ItemID;
	public int ItemNum;
}


public class EctypeSelectConfigDataBase : ScriptableObject
{
    public EctypeSelectConfigData[] _dataTable;
}
