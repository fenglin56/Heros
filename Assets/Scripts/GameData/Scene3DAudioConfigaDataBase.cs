using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Scene3DAudioData {
    public int MapId;
    public Vector3 PointPos;
    public int Radius;
    public string Sound;
	
}

public class Scene3DAudioConfigaDataBase:ScriptableObject
{
    public Scene3DAudioData[] SoundList;
}
