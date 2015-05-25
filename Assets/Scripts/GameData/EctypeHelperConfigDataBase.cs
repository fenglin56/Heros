using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class EctypeHelperConfigData
{
    public int _GuideID;
    public int _GuideOrder;
    public UIAtlas _UIAtlas;
    public string _TextureName;
}

public class EctypeHelperConfigDataBase : ScriptableObject
{
    public EctypeHelperConfigData[] _dataTable;
}
