using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class StroyCameraConfigData
{
    public int _CameraID;
    public int _TargetType;
    public Vector2 _TargetPos;
    public int _TargetID;
    public Vector3 _TargetOffset;
    public float _ActionTime;
    public int _CameraMask;
    public CameraParam[] _Params;
    public int _MoveMode; // 1,平移； 2,旋转

    public StroyCameraConfigData Clone()
    {
        StroyCameraConfigData tempObj = (StroyCameraConfigData )this.MemberwiseClone();
        if (_Params != null)
        {
            tempObj._Params = new CameraParam[this._Params.Length];
            for (int i = 0; i < this._Params.Length; i++)
            {
                tempObj._Params[i] = this._Params[i].Clone();
            }
        }
        return tempObj;
    }
}

[Serializable]
public class CameraParam
{
    public float _EquA;
    public float _EquB;
    public float _EquC;
    public float _EquD;

    public CameraParam Clone()
    {
        return (CameraParam)this.MemberwiseClone();
    }
}

public class StroyCameraDataBase : ScriptableObject
{
    public StroyCameraConfigData[] _dataTable;
}
