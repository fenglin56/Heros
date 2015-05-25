using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class PlayerRoomAccoutConfigData
{
    public int _roomTypeID;//房间类型ID
    public int _upperLimit;//房间修炼上限
    public int _basicsParam;//房间修炼基础修为
    public int _ownerAddition;//房主修炼加成
    public int _guestAddition;//单个房客修为增加值 百分比
    public int _roomLevel;//房间模版等级区间
    public Vector3 _camera;//
    public List<SirenPosInfo> SirenPosInfoList;
    public List<Vector3> PlayerPosList;
    public List<Vector3> PlayerAngleList;
    //public Dictionary<int, Vector3> SirenPosDict = new Dictionary<int, Vector3>();//妖女位置信息(妖女id， 妖女位置)
    
}

public class PlayerRoomAccoutConfigDataBase : ScriptableObject
{
    public PlayerRoomAccoutConfigData[] _dataTable;	
}

[System.Serializable]
public class SirenPosInfo
{
    public int sirenID;
    public Vector3 sirenPos;
}

[System.Serializable]
public class PlayerPosInfo
{
    public int sirenID;
    public Vector3 sirenPos;
}