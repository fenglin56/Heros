using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IEntityManager
{
    EntityModel GetEntityMode(Int64 uid);
    void RegisteEntity(EntityModel playerDataModel);
    void UnRegisteEntity(Int64 uid);
}
/// <summary>
/// 实体接口，服务器创建实体的消息结构体需要实现这个接口
/// </summary>
public interface IEntityDataStruct
{
    SMsgPropCreateEntity_SC_Header SMsg_Header{get;}
    void UpdateValue(short index, int value);
}
public interface IEntityDataManager
{
    IEntityDataStruct GetDataModel();
}
public interface IEntityIndexCalc
{
    short ReCalcIndex(short sourceIndex);
}
public interface IPlayerDataStruct
{
    int PlayerActorID { get; }         //角色ID
    int PlayerX{get;}         //角色X坐标  除以10
    int PlayerY{get;}         //角色Y坐标  除以10
    SMsgPropCreateEntity_SC_BaseValue GetBaseValue();
    SMsgPropCreateEntity_SC_Player_UnitValue GetUnitValue();
    SMsgPropCreateEntity_SC_Player_CommonValue GetCommonValue();
}

