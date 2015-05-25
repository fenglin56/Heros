using UnityEngine;
using System.Collections;
using System;
using UI.MainUI;


[Serializable]
public class MissionFailData
{
    public int BtnID;
    public int SysModule;
    public string NameIDS;
    public string ButtonExplainIDS;
    public UIType BtnType;
    //public int NeedTask;

    public GameObject IconPrefab;
}

public class MissionFailBtnDataBase : ScriptableObject{

    public MissionFailData[] MissionFailDataTable;

}
