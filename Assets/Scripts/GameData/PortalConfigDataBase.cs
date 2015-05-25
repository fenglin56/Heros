using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class PortalConfigData
{
    public int _SID;
    public int _portalType;
    public int[] _desMapID;
}

public class PortalConfigDataBase : ScriptableObject
{
    public PortalConfigData[] _dataTable;
}

