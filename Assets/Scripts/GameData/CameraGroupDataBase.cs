using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CameraGroupConfigData
{
    public int _CameraGroupID;
    public NpcAction[] _ActionList;
    public List<int> _CameraID;
    public int[] _DialogGroupID;
    public GameObject _EffectGo;
    public bool _IsCameraStartMask;
    public bool _IsCameraEndMask;

    public CameraGroupConfigData Clone()
    {
        CameraGroupConfigData tempObj = (CameraGroupConfigData)this.MemberwiseClone();
        if (_CameraID != null)
        {
            tempObj._CameraID = new List<int>(this._CameraID);//(int[])this._CameraGroup.Clone();
        }

        if (_ActionList != null)
        {
            tempObj._ActionList = (NpcAction[])_ActionList.Clone();
            
            for (int i = 0; i < _ActionList.Length; i++ )
            {
                tempObj._ActionList[i] = _ActionList[i].Clone();
            }
        }

        return tempObj;
    }
}

[Serializable]
public class NpcAction
{
    public int NpcID;
    public int RoleResID;
    public int RoleType;
    public List<int> AnimID;

    public NpcAction Clone()
    {
        NpcAction tempObj = (NpcAction)this.MemberwiseClone();
        if (AnimID != null)
        {
            tempObj.AnimID = new List<int>(this.AnimID);//(int[])this._CameraGroup.Clone();
        }
        return tempObj;
    }
}

public class CameraGroupDataBase : ScriptableObject
{
    public CameraGroupConfigData[] _dataTable;
}

