using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class MainButtonConfigData
{
    public int _ButtonID;
    /// <summary>
    /// 功能按钮类型
    /// </summary>
    public UI.MainUI.UIType _ButtonFunc;
    /// <summary>
    /// 按钮区域
    /// </summary>
    public UI.MainBtnArea _ButtonArea;
    public Vector2 _ButtonIndex;
    public float _ButtonRadius;
    public bool _IsEnable;
    
    //[Late]
    public GameObject _ButtonPrefab;
    [HideInInspector]
    public string _ButtonPrefabId;

    public GameObject TrapPrefab
    {
        get
        {
            if (_ButtonPrefab != null)
            {
                return _ButtonPrefab;
            }

            _ButtonPrefab = AssetId.Resolve(_ButtonPrefab, _ButtonPrefabId);
            return _ButtonPrefab;
        }
    }
}

public class MainButtonComparer : IComparer<MainButtonConfigData>
{
    public int Compare(MainButtonConfigData x, MainButtonConfigData y)
    {
        return CompareButtonIndex(x, y);
    }

    private int CompareButtonIndex(MainButtonConfigData x, MainButtonConfigData y)
    {
        int index = 0;
//        if (x._ButtonIndex != y._ButtonIndex)
//        {
//            index = x._ButtonIndex < y._ButtonIndex
//                ? -1 : 1;
//        }

        if (index == 0)
        {
            //如果在线相等 则比较成为好友的先后顺序
        }

        return index;
    }

}

public class MainButtonConfigDataBase : ScriptableObject
{
    public MainButtonConfigData[] _dataTable;
}

