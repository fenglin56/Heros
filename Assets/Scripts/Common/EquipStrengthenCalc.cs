using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EquipStrengthenCalc
{
    public float NormalStrengthenAddition(float sourceValue,int normalStrengthenLv,float p1, float p2, float p3, float p4)
    {
        return sourceValue+Mathf.FloorToInt((p1 * Mathf.Pow(normalStrengthenLv, 2) + p2 * normalStrengthenLv + p3) / p4) * p4;
    }
    public float NormalStrengthenConsume(int normalStrengthenLv, float p1, float p2, float p3, float p4)
    {
        return Mathf.FloorToInt((p1 * Mathf.Pow(normalStrengthenLv, 2) + p2 * normalStrengthenLv + p3) / p4) * p4;
    }
    public float StarStrengthenAddition(float sourceValue,int normalStrengthenLv, int startStrengthenLv, float p1, float p2, float p3, float p4)
    {
        float startStrengthenPercent = startStrengthenLv * 0.05f;
        var normalAddition=NormalStrengthenAddition(sourceValue,normalStrengthenLv,p1,p2,p3,p4);
        return (sourceValue + normalAddition) * (1 + startStrengthenPercent);
    }
    public float StarStrengthenConsume(float p1, float p2, float p3, float p4)
    {
        return 0;
    }
}
