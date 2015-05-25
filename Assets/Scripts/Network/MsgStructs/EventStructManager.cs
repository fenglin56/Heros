using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//public struct SceneChangeStruct:INotifyArgs
//{
//    private int m_eventArgsType;
//    public GameManager.GameState OriginGameState;
//    public GameManager.GameState TargetGameState;


//    public int GetEventArgsType()
//    {
//        throw new NotImplementedException();
//    }
//}
public struct PlayerGotoSceneReadyStruct : INotifyArgs
{
    public PlayerGotoSceneReadyStruct(GameManager.GameState gameState)
    {
        GameState = gameState;
    }
    public GameManager.GameState GameState;
   

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
public struct SceneLoadedStruct : INotifyArgs
{
    private int m_eventArgsType;

    public GameManager.GameState CurrentState ;

  

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}

#region 组队事件消息体
/// <summary>
/// 进入组队页面按钮触发服务器返回事件
/// </summary>
public struct TeamButtonStruct : INotifyArgs
{
    

    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 创建队伍服务器返回事件
/// </summary>
public struct TeamCreateStruct : INotifyArgs
{


    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 加入队伍服务器返回事件
/// </summary>
public struct TeamJoinStruct : INotifyArgs
{


    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 踢人服务器返回事件
/// </summary>
public struct TeamCickStruct : INotifyArgs
{


    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 离开队伍服务器返回事件
/// </summary>
public struct TeamLeaveStruct : INotifyArgs
{


    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 解散队伍服务器返回事件
/// </summary>
public struct TeamDisbandStruct : INotifyArgs
{


    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 战斗准备服务器返回事件
/// </summary>
public struct TeamBattleReadyStruct : INotifyArgs
{


    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 挑战开始服务器返回事件
/// </summary>
public struct TeamChallengeStruct : INotifyArgs
{


    public int GetEventArgsType()
    {
        throw new NotImplementedException();
    }
}
#endregion

#region 错误信息消息体
public struct ServerError : INotifyArgs
{
    public short ErrorCode;


    public int GetEventArgsType()
    {
        return ErrorCode;
    }
}

public struct EctypeLevelError
{
    public uint[] LevelList;

    public static EctypeLevelError ParcePackage(byte[] dataBuff)
    {
        EctypeLevelError ectypeLevelError = new EctypeLevelError();
        if (dataBuff.Length > 0)
        {
            byte number = dataBuff[0];
            ectypeLevelError.LevelList = new uint[number];
            int off = 1;
            for (int i = 0; i < number; i++)
            {
                off += 2;
                off += PackageHelper.ReadData(dataBuff.Skip(off).ToArray(), out ectypeLevelError.LevelList[i]);
            }
        }
        return ectypeLevelError;
    }
}
#endregion


