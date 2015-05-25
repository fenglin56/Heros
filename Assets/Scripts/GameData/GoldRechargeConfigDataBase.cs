using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class GoldRechargeData
{
    public int RechargeID;
    public int RechargeType;
    public string RechargePosition;
    public string CurrencyPicture;
    public string GoldPicture;
	public GameObject goldPicturePrefab;
    public float CurrencyNumber;
    public int GoldNumber;
    public float Discount;
    public string TouchAnimation;
    public string TouchVoice;
}


public class GoldRechargeConfigDataBase : ScriptableObject{

    public GoldRechargeData[] GoledRechargeDataList;
}
