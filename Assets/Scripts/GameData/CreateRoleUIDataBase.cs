using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateRoleUIDataBase : ScriptableObject {

    public CreateRoleUIData[] _dataTable;
}

[System.Serializable]
public class CreateRoleUIData
{
    public int _VocationID;
    public string _HeadIcon;
    public string _VocationIcon;
    public GameObject _RoleModel;
    public Vector3 _RolePosition;
    public string _InitAnim;
    public string[] _AnimList;
    public string _StopAnim;
    public string _BackAnim;
    public string _IntroText;
    public GameObject[] _EffectList;
    public float[] _EffectDelayTime;
    public string[] _SoundEffectList;
    public Vector3 _CameraPosition;
    public Vector3 _CameraTarget;
	public float _UIDelayTime;
    public List<int> PlayerAbility;//角色显示属性
}

[System.Serializable]
public class SelectSound
{
    public string SoundName;
    public float DalayTime;
}
