using System;
using UnityEngine;

/// <summary>
/// 标记此特征表示在读表过程中忽略
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class HideInDataReaderAttribute : Attribute
{

}
/// <summary>
/// 标记此特征表示从FieldOfPath中读取Prefab，赋与此Field。此Field需为GameObject
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class DataToObjectAttribute : Attribute
{
    public string PrefabPath;
}
/// <summary>
/// 标记此特征表示从FieldOfPath中读取Prefab，赋与此Field。此Field需为GameObject
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class DataSplitToObjectArrayAttribute : Attribute
{
    public string PrefabPath;
    public Char SplitChar;
    public Type CustomerType;
}
/// <summary>
/// 标记此特征表示从FieldOfPath中读取Prefab，赋与此Field。此Field需为GameObject
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class DataSplitToCustomerObjectArrayAttribute : Attribute
{
    public Type CustomerType;
    public Char ParSplitChar;

    public string InitMethodName;
    public string[] PrefabPath;
    public char SplitChar;
    public int Length;
    public Type[] Types;
}
/// <summary>
/// 标记此特征表示从FieldOfPath中读取Prefab，赋与此Field。此Field需为GameObject
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class CustomerParseAttribute : Attribute
{
    public string InitMethodName;
    public string[] PrefabPath;
    public char SplitChar;
    public int Length;
    public Type[] Types;
}

/// <summary>
/// 标记此特征表示此Field与对应的SheetName不一致，需要读CSV中SheetNameOfData的列
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class FieldMapAttribute : Attribute
{
    public string SheetNameOfData;
}
[AttributeUsage(AttributeTargets.Field)]
public class EnumMapAttribute : Attribute
{
}
public abstract class ConfigBase: ScriptableObject
{
    public abstract void Init(int length, object dataList);
}