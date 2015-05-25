using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct TargetSelected:INotifyArgs
{
    public Transform Target;
    public ResourceType Type;

    
    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
public class FightResult : INotifyArgs
{
    public SMsgFightFightTo SMsgFightFightTo;
    public SCmdImpactData[] TargetDatas;
    public int GetEventArgsType()
    {
        return 0;
    }
}
public struct TouchInvoke : INotifyArgs
{
    public TouchInvoke(GameObject go, Vector3 touchPoint, byte touchCount)
    {
        TouchGO = go;
        TouchPoint = touchPoint;
        TouchCount = touchCount;
    }
    public GameObject TouchGO;
    public Vector3 TouchPoint;
    public byte TouchCount;   //点击数，1  单击   2 双击

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}

public struct PageChangedEventArg : INotifyArgs
{
    public int StartPage;
    public int PageSize;

    public PageChangedEventArg(int startPage, int pageSize)
    {
        this.StartPage = startPage;
        this.PageSize = pageSize;
    }
    public int GetEventArgsType()
    {
        return 0;
    }
}

