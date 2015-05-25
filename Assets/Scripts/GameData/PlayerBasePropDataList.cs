using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class PlayerBasePropData
{
    public int PlayerKind;
    public int BasePropID;
    public string BaseProp;
    public float ParamA;
    public float ParamB;
    public float ParamC;
    public float ParamD;
	public int nPropID;
	public int nSettleID;
}

public class PlayerBasePropDataList : ScriptableObject {

    public PlayerBasePropData[] playerBasePropDatalist;
	
}
