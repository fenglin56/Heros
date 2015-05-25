using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class LoadingTipsData
{
    public int TipsID;
    public int Levels_Min;
    public int Levels_Max;
    public int Weights;
    public string LoadingIDS;
}


public class LoadingTipsDataBase : ScriptableObject{

    public LoadingTipsData[] LoadingDataList;	

}
