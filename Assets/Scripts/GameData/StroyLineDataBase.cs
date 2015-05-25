using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class StroyLineConfigData
{
    public int _StroyLineID;
    public int _SceneMapID;
    public string _BgMusic;
    public int _TriggerVocation;
    public int _TriggerCondition;
    public int _EctypeID;
    public List<int> _CameraGroup;
	public int WeaponType;
    public StroyLineConfigData Clone()
    {
        StroyLineConfigData tempObj = (StroyLineConfigData )this.MemberwiseClone();
        if (_CameraGroup != null)
        {
            tempObj._CameraGroup = new List<int>(this._CameraGroup);//(int[])this._CameraGroup.Clone();
        }
        return tempObj;
    }
}

public class StroyLineDataBase : ScriptableObject
{
    public StroyLineConfigData[] _dataTable;
}
