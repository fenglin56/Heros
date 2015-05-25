using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class StroyActionConfigData
{
    public int _ActionID;
    public string _ActionName;
    public Vector3 _StartPosition;
    public int _ActionType;
    public float _StartAngle;
    public float _ModelAngle;    //动作初始朝向
    public float _Speed;
    public float _Acceleration;
    public float _Duration;
    public GameObject _EffectGo;
    public float _EffectStartTime;
    public Vector3 _EffectPosition;
    public float _EffectStartAngle;
    public int _EffectLoopTimes;
    public float _SoundTime;
    public string _SoundName;

    public StroyActionConfigData Clone()
    {
        StroyActionConfigData tempObj = (StroyActionConfigData)this.MemberwiseClone();
        if (_EffectGo != null)
        {
            tempObj._EffectGo = this._EffectGo;
        }
        return tempObj;
    }
}


public class StroyActionDataBase : ScriptableObject
{
    public StroyActionConfigData[] _dataTable;
}
