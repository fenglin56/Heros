using UnityEngine;
using System.Collections;
using System;

[System.AttributeUsage(System.AttributeTargets.Property |
    System.AttributeTargets.Field)]
public class GameDataPostFlag : System.Attribute
{
    public bool ClearFlag { get; private set; }
    public GameDataPostFlag(bool clearFlag)
    {
        this.ClearFlag = clearFlag;
    }
}