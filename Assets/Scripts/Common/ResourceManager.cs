using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 场景资源管理器，负责解析本地的csv文件或XML文件
/// 负责根据资源类型和资源ID把资源实例化返回
/// </summary>
public class ResourceManager
{
    public void ParseConfigFile()
    {
    }
    public ResourceBirthInfo ResourceGenerator(ResourceType resourceType, int resourceId)
    {
        var resourceBirthInfo=new ResourceBirthInfo();
        switch (resourceType)
        {
            case ResourceType.Hero:
                //读取相关资源 按配置在主角出生地生成主角。
                break;
            case ResourceType.Monster:
                //读取相关资源 按配置在怪物出生地生成怪物。
                break;
        }
        return resourceBirthInfo;
    }
}
public struct ResourceBirthInfo
{
    public string ResourcePath;
    public Vector3 Position;
    public Vector3 Rotation;
}
