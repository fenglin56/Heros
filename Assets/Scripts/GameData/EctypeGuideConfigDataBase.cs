using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class EctypeGuideConfigData
{
    public int _EctypeID;
    public int[] _StepList;
    public int[] _JoyStickStepList;
}

public class EctypeGuideConfigDataBase : ScriptableObject
{
    public EctypeGuideConfigData[] _dataTable;
}
