using UnityEngine;
using System.Collections;

public class TrapBehaviour :View , IEntityDataManager{

    private IEntityDataStruct m_trapDataModel;
    public IEntityDataStruct TrapDataModel
    {
        get { return this.m_trapDataModel; }
        set { this.m_trapDataModel = value; }
    }


    protected override void RegisterEventHandler()
    {
        throw new System.NotImplementedException();
    }

    public IEntityDataStruct GetDataModel()
    {
        return this.TrapDataModel;
    }
}
