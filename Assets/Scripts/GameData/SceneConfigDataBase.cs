using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class SceneConfigData
{
	public int _lMapID;
    public string _szSceneName;
	public string _szMapFile;
	public int _mapWidth;
	public int _mapHeight;
    public string _szNameID;
    public string _szMapIcon;
    public GameObject MpIconPrefab;
	public string _mapBGM;
    public int _sceneType;
    public bool _isLockMode;
    public Vector3[] _TriggerAreaPoint;
    public float[] _TriggerAreaRadius;
}



public class SceneConfigDataBase : ScriptableObject
{
    public SceneConfigData[] _dataTable;
}
