using UnityEngine;
using System.Collections;
using System;

public class SkillCameraConfigDataBase : ScriptableObject 
{
    public SkillCameraData[] _dataTable;
}

[Serializable]
public class SkillCameraData
{
    public int _CameraID;
    public float _CameraDuration;
    public int _StartType;
    public Vector3 _CameraOffset;
    public Vector3 _TargetOffset;
    public int _CameraType;
    public SkillCameraParam[] _CameraParams;
    public float _CameraResetTime;
    public float[] _ShakeStartTime;
    public string[] _ShakeAnimName;
}



[Serializable]
public class SkillCameraParam
{
    public float _EquA;
    public float _EquB;

    public SkillCameraParam Clone()
    {
        return (SkillCameraParam)this.MemberwiseClone();
    }
}