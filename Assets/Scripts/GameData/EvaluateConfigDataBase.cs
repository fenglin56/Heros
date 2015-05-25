using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class EvaluateData
{
    public string Evaluate;
    public GameObject IconPrefab;
	public GameObject StarIconPrefab;
}

public class EvaluateConfigDataBase : ScriptableObject
{
    public EvaluateData[] EvaluateDataList;

}
