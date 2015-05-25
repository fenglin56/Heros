using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class InitMainButtonData
{
    public int _EnableIndex;
    public UI.MainUI.UIType[] _MainButtonList;
   
}

public class InitMainButtonDataBase : ScriptableObject
{
    public InitMainButtonData[] _dataTable;
}

