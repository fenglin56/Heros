using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 服务器通知生成场景资源的结构体,需要与服务器接口一致
/// 场景资源包括 主角，怪物，可破坏物，不可破坏物，陷井等（相关枚举值支持）
/// </summary>
public struct MakeElementNotifyStruct:INotifyArgs
{
    /// <summary>
    /// 事件类型
    /// </summary>
    private int m_eventArgsType;
    /// <summary>
    /// 场景资源类型
    /// </summary>
    public ResourceType ResourceType;
    /// <summary>
    /// 资源ID
    /// </summary>
    public int ResourceId;

    public Package GeneratePackage()
    {
        return new Package();
    }

    public int EventArgsType
    {
        get
        {
            return this.m_eventArgsType;
        }
        set
        {
            this.m_eventArgsType = value;
        }
    }
    /// <summary>
    /// 从服务器收到消息，解析消息，把相关数值赋予本地结构体
    /// </summary>
    /// <param name="dataBuffer"></param>
    public void ParsePackage(byte[] dataBuffer)
    {
    }

    public int GetEventArgsType()
    {
        return this.m_eventArgsType;
    }
}
